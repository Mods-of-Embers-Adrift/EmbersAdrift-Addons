using System;
using SoL.Game.Messages;
using SoL.Managers;
using SoL.Networking.Objects;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Dueling
{
	// Token: 0x02000C9E RID: 3230
	public class Duel : IPoolable
	{
		// Token: 0x17001769 RID: 5993
		// (get) Token: 0x060061F8 RID: 25080 RVA: 0x0008201B File Offset: 0x0008021B
		// (set) Token: 0x060061F9 RID: 25081 RVA: 0x00202D0C File Offset: 0x00200F0C
		public DuelStatus Status
		{
			get
			{
				return this.m_status;
			}
			set
			{
				if (this.m_status == value)
				{
					return;
				}
				this.m_status = value;
				this.SetEntityDuelStates(this.m_status);
				switch (this.m_status)
				{
				case DuelStatus.Requested:
					break;
				case DuelStatus.Accepted:
				case DuelStatus.Declined:
					if (this.m_status == DuelStatus.Accepted && this.m_source && this.m_source.GameEntity)
					{
						this.NotifyAroundSource(this.m_source, -1);
						return;
					}
					this.SendChatNotification(true, this.m_opponentName + " has declined your duel request.");
					this.SendChatNotification(false, "You have declined " + this.m_sourceName + "'s duel request.");
					return;
				case DuelStatus.Cancelled:
					this.SendChatNotification(true, "Pending duel with " + this.m_opponentName + " cancelled.");
					this.SendChatNotification(false, "Pending duel with " + this.m_sourceName + " cancelled.");
					return;
				case DuelStatus.Expired:
					this.SendChatNotification(true, "Your duel request to " + this.m_opponentName + " has expired.");
					this.SendChatNotification(false, "Duel request from " + this.m_sourceName + " has expired.");
					break;
				case DuelStatus.Forfeited:
					if (this.m_sourceForfeit)
					{
						this.NotifyAroundSource(this.m_opponent, -1);
						return;
					}
					if (this.m_opponentForfeit)
					{
						this.NotifyAroundSource(this.m_source, -1);
						return;
					}
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x060061FA RID: 25082 RVA: 0x00202E68 File Offset: 0x00201068
		public void Init(NetworkEntity source, NetworkEntity opponent)
		{
			if (!source)
			{
				throw new ArgumentNullException("source");
			}
			if (!source.PlayerRpcHandler)
			{
				throw new ArgumentNullException("PlayerRpcHandler");
			}
			if (!opponent)
			{
				throw new ArgumentNullException("opponent");
			}
			if (!opponent.PlayerRpcHandler)
			{
				throw new ArgumentNullException("PlayerRpcHandler");
			}
			this.Id = UniqueId.GenerateFromGuid();
			this.m_timestamp = DateTime.UtcNow;
			this.m_position = source.gameObject.transform.position;
			this.m_source = source;
			this.m_sourceId = source.NetworkId.Value;
			this.m_sourceName = ((source.GameEntity && source.GameEntity.CharacterData) ? source.GameEntity.CharacterData.Name : "UNKNOWN");
			this.m_opponent = opponent;
			this.m_opponentId = opponent.NetworkId.Value;
			this.m_opponentName = ((opponent.GameEntity && opponent.GameEntity.CharacterData) ? opponent.GameEntity.CharacterData.Name : "UNKNOWN");
			this.Status = DuelStatus.Requested;
			opponent.PlayerRpcHandler.Server_DuelRequest(this.Id, this.m_sourceName);
		}

		// Token: 0x060061FB RID: 25083 RVA: 0x00202FD0 File Offset: 0x002011D0
		public void Update()
		{
			if (!this.m_source || !this.EligibleForDuel(this.m_source))
			{
				DuelStatus forfeitStatus = this.Status.GetForfeitStatus();
				this.m_sourceForfeit = true;
				this.m_opponentForfeit = false;
				this.Status = forfeitStatus;
				return;
			}
			if (!this.m_opponent || !this.EligibleForDuel(this.m_opponent))
			{
				DuelStatus forfeitStatus2 = this.Status.GetForfeitStatus();
				this.m_sourceForfeit = false;
				this.m_opponentForfeit = true;
				this.Status = forfeitStatus2;
				return;
			}
			DuelStatus status = this.Status;
			if (status != DuelStatus.Requested)
			{
				if (status == DuelStatus.Accepted)
				{
					this.Status = DuelStatus.Executing;
					return;
				}
				if (status != DuelStatus.Executing)
				{
					return;
				}
				if (Time.time - this.m_lastRoll > 4f)
				{
					this.ExecuteNextRoll();
				}
			}
			else if ((DateTime.UtcNow - this.m_timestamp).TotalSeconds > 30.0)
			{
				this.Status = DuelStatus.Expired;
				return;
			}
		}

		// Token: 0x060061FC RID: 25084 RVA: 0x002030B8 File Offset: 0x002012B8
		private bool EligibleForDuel(NetworkEntity entity)
		{
			return entity && entity.GameEntity && entity.GameEntity.Vitals && entity.GameEntity.Vitals.GetCurrentHealthState() == HealthState.Alive && (entity.gameObject.transform.position - this.m_position).sqrMagnitude <= 100f;
		}

		// Token: 0x060061FD RID: 25085 RVA: 0x00203130 File Offset: 0x00201330
		private DuelRoll GetDuelRoll(int rollResult)
		{
			return new DuelRoll
			{
				Status = this.m_status,
				NSides = this.m_currentRoll,
				RollResult = rollResult,
				RollCount = this.m_rollCount,
				IsSourceRoll = this.m_isSourceRoll,
				IsSourceForfeit = this.m_sourceForfeit,
				SourceName = this.m_sourceName,
				SourceId = this.m_sourceId,
				OpponentName = this.m_opponentName,
				OpponentId = this.m_opponentId
			};
		}

		// Token: 0x060061FE RID: 25086 RVA: 0x00082023 File Offset: 0x00080223
		private void NotifyAroundSource(NetworkEntity source, int rollResult)
		{
			if (ServerGameManager.SpatialManager && source && source.GameEntity)
			{
				ServerGameManager.SpatialManager.DuelResultNearbyPlayers(source.GameEntity, 30f, this.GetDuelRoll(rollResult));
			}
		}

		// Token: 0x060061FF RID: 25087 RVA: 0x002031C4 File Offset: 0x002013C4
		private void SendChatNotification(bool toSource, string msg)
		{
			NetworkEntity networkEntity = toSource ? this.m_source : this.m_opponent;
			if (networkEntity && networkEntity.PlayerRpcHandler)
			{
				networkEntity.PlayerRpcHandler.SendChatNotification(msg);
			}
		}

		// Token: 0x06006200 RID: 25088 RVA: 0x00082062 File Offset: 0x00080262
		private string GetResponseMessage(DuelStatus status)
		{
			if (status == DuelStatus.Accepted)
			{
				return "accepted";
			}
			if (status != DuelStatus.Declined)
			{
				return string.Empty;
			}
			return "declined";
		}

		// Token: 0x06006201 RID: 25089 RVA: 0x00203204 File Offset: 0x00201404
		private void SetEntityDuelStates(DuelStatus state)
		{
			if (this.m_source && this.m_source.GameEntity)
			{
				this.m_source.GameEntity.DuelState = state;
			}
			if (this.m_opponent && this.m_opponent.GameEntity)
			{
				this.m_opponent.GameEntity.DuelState = state;
			}
		}

		// Token: 0x06006202 RID: 25090 RVA: 0x0008207F File Offset: 0x0008027F
		public bool IsFinished()
		{
			bool flag = this.Status == DuelStatus.Complete || this.Status == DuelStatus.Declined || this.Status == DuelStatus.Forfeited || this.Status == DuelStatus.Cancelled || this.Status == DuelStatus.Expired;
			if (flag)
			{
				this.SetEntityDuelStates(DuelStatus.None);
			}
			return flag;
		}

		// Token: 0x06006203 RID: 25091 RVA: 0x00203274 File Offset: 0x00201474
		private void ExecuteNextRoll()
		{
			int num = UnityEngine.Random.Range(1, this.m_currentRoll + 1);
			this.m_rollCount++;
			this.NotifyAroundSource(this.m_source, num);
			if (num == 1)
			{
				NetworkEntity networkEntity = this.m_isSourceRoll ? this.m_source : this.m_opponent;
				if (networkEntity && networkEntity.GameEntity && networkEntity.GameEntity.Vitals)
				{
					float num2 = networkEntity.GameEntity.Vitals.Health * 0.99f;
					networkEntity.GameEntity.Vitals.AlterHealth(-1f * num2);
				}
				this.Status = DuelStatus.Complete;
				if (ServerGameManager.SpatialManager)
				{
					ServerGameManager.SpatialManager.DuelResultNotifyAllPlayers(this.m_source.GameEntity, this.GetDuelRoll(num));
				}
			}
			else
			{
				NetworkEntity networkEntity2 = this.m_isSourceRoll ? this.m_opponent : this.m_source;
				if (networkEntity2 && networkEntity2.GameEntity && networkEntity2.GameEntity.Vitals)
				{
					float num3 = Mathf.Clamp(1f - (float)num / (float)this.m_currentRoll, 0f, 0.99f);
					float num4 = networkEntity2.GameEntity.Vitals.Health * num3;
					networkEntity2.GameEntity.Vitals.AlterHealth(-1f * num4);
				}
				this.m_currentRoll = num;
				this.m_isSourceRoll = !this.m_isSourceRoll;
			}
			this.m_lastRoll = Time.time;
		}

		// Token: 0x1700176A RID: 5994
		// (get) Token: 0x06006204 RID: 25092 RVA: 0x000820BB File Offset: 0x000802BB
		// (set) Token: 0x06006205 RID: 25093 RVA: 0x000820C3 File Offset: 0x000802C3
		bool IPoolable.InPool
		{
			get
			{
				return this.m_inPool;
			}
			set
			{
				this.m_inPool = value;
			}
		}

		// Token: 0x06006206 RID: 25094 RVA: 0x00203404 File Offset: 0x00201604
		void IPoolable.Reset()
		{
			this.Id = UniqueId.Empty;
			this.m_source = null;
			this.m_sourceId = 0U;
			this.m_sourceName = string.Empty;
			this.m_opponent = null;
			this.m_opponentId = 0U;
			this.m_opponentName = string.Empty;
			this.m_timestamp = DateTime.MinValue;
			this.Status = DuelStatus.None;
			this.m_currentRoll = 100;
			this.m_isSourceRoll = false;
			this.m_lastRoll = float.MinValue;
			this.m_sourceForfeit = false;
			this.m_opponentForfeit = false;
			this.m_position = Vector3.zero;
			this.m_rollCount = 0;
		}

		// Token: 0x04005591 RID: 21905
		private const float kMaxDuelDistance = 10f;

		// Token: 0x04005592 RID: 21906
		public const float kMaxDuelDistanceSqr = 100f;

		// Token: 0x04005593 RID: 21907
		private const int kInitialRoll = 100;

		// Token: 0x04005594 RID: 21908
		private const float kExpirationTime = 30f;

		// Token: 0x04005595 RID: 21909
		private const float kNotificationRadius = 30f;

		// Token: 0x04005596 RID: 21910
		private const float kTimeBetweenRolls = 4f;

		// Token: 0x04005597 RID: 21911
		private const float kHealthFraction = 0.99f;

		// Token: 0x04005598 RID: 21912
		internal const MessageType kMessageType = MessageType.Emote;

		// Token: 0x04005599 RID: 21913
		internal const string kPrefix = "<sprite=\"SolIcons\" name=\"Swords\" tint=1><sprite=\"SolIcons\" name=\"NeedIcon\" tint=1>";

		// Token: 0x0400559A RID: 21914
		internal const string kCannotDuelYourself = "You cannot duel yourself!";

		// Token: 0x0400559B RID: 21915
		public UniqueId Id;

		// Token: 0x0400559C RID: 21916
		private DateTime m_timestamp;

		// Token: 0x0400559D RID: 21917
		private Vector3 m_position;

		// Token: 0x0400559E RID: 21918
		private int m_rollCount;

		// Token: 0x0400559F RID: 21919
		private NetworkEntity m_source;

		// Token: 0x040055A0 RID: 21920
		private uint m_sourceId;

		// Token: 0x040055A1 RID: 21921
		private string m_sourceName;

		// Token: 0x040055A2 RID: 21922
		private bool m_sourceForfeit;

		// Token: 0x040055A3 RID: 21923
		private NetworkEntity m_opponent;

		// Token: 0x040055A4 RID: 21924
		private uint m_opponentId;

		// Token: 0x040055A5 RID: 21925
		private string m_opponentName;

		// Token: 0x040055A6 RID: 21926
		private bool m_opponentForfeit;

		// Token: 0x040055A7 RID: 21927
		private int m_currentRoll = 100;

		// Token: 0x040055A8 RID: 21928
		private bool m_isSourceRoll;

		// Token: 0x040055A9 RID: 21929
		private float m_lastRoll = float.MinValue;

		// Token: 0x040055AA RID: 21930
		private DuelStatus m_status;

		// Token: 0x040055AB RID: 21931
		private bool m_inPool;
	}
}
