using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SoL.Game.Dungeons;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Utilities.Extensions;
using SoL.Utilities.Logging;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000573 RID: 1395
	public class ServerPlayerController : GameEntityComponent
	{
		// Token: 0x170008F0 RID: 2288
		// (get) Token: 0x06002B15 RID: 11029 RVA: 0x0005DE93 File Offset: 0x0005C093
		public CharacterRecord Record
		{
			get
			{
				return this.m_record;
			}
		}

		// Token: 0x06002B16 RID: 11030 RVA: 0x0005DE9B File Offset: 0x0005C09B
		private void Awake()
		{
			base.GameEntity.ServerPlayerController = this;
		}

		// Token: 0x06002B17 RID: 11031 RVA: 0x0005DEA9 File Offset: 0x0005C0A9
		private void OnDestroy()
		{
			this.FinalizeDb();
		}

		// Token: 0x06002B18 RID: 11032 RVA: 0x0005DEB1 File Offset: 0x0005C0B1
		public void Initialize(Transform trans, CharacterRecord record)
		{
			this.m_transform = trans;
			this.m_record = record;
			this.InitializeInternal();
			this.m_initialized = true;
		}

		// Token: 0x06002B19 RID: 11033 RVA: 0x00145688 File Offset: 0x00143888
		private void InitializeInternal()
		{
			float time = 30f;
			Vector3 position = this.m_record.Location.GetPosition();
			Quaternion rotation = this.m_record.Location.GetRotation();
			this.m_sessionStart = DateTime.UtcNow;
			this.m_timePlayed = new TimePlayed(this.m_record);
			if (this.m_record.ZoningState == null)
			{
				this.m_record.ZoningState = new CharacterZoningState
				{
					State = ZoningState.NewCharacter
				};
			}
			if (LocalZoneManager.DefaultPlayerSpawn == null)
			{
				throw new ArgumentException("No default player spawn!");
			}
			switch (this.m_record.ZoningState.State)
			{
			case ZoningState.NewCharacter:
			{
				time = 0f;
				position = LocalZoneManager.DefaultPlayerSpawn.GetPosition();
				rotation = LocalZoneManager.DefaultPlayerSpawn.GetRotation();
				this.m_record.ZoningState.State = ZoningState.None;
				this.m_record.ZoningState.SourceZoneId = LocalZoneManager.ZoneRecord.ZoneId;
				BaseRole firstRole = this.m_record.GetFirstRole();
				this.m_record.InitializeCharacterStorage();
				if (firstRole)
				{
					firstRole.AddStartingEquipment(this.m_record);
					this.m_record.Settings.TrackedMastery = firstRole.Id;
					base.GameEntity.CharacterData.BaseRoleId = firstRole.Id;
					base.GameEntity.CharacterData.SpecializedRoleId = UniqueId.Empty;
				}
				break;
			}
			case ZoningState.Zoning:
				time = 0f;
				if (this.m_record.ZoningState.SourceZoneId != LocalZoneManager.ZoneRecord.ZoneId)
				{
					ZonePoint zonePoint = LocalZoneManager.GetZonePoint((ZoneId)this.m_record.ZoningState.SourceZoneId, this.m_record.ZoningState.TargetIndex);
					if (zonePoint == null)
					{
						CharacterLocation characterLocation = null;
						if (OverworldDungeonEntranceSpawnManager.Instance)
						{
							characterLocation = OverworldDungeonEntranceSpawnManager.Instance.GetInactiveLocation(this.m_record.ZoningState.SourceZoneId, this.m_record.ZoningState.TargetIndex);
							if (characterLocation != null)
							{
								position = characterLocation.GetPosition();
								rotation = characterLocation.GetRotation();
							}
						}
						if (characterLocation == null)
						{
							position = LocalZoneManager.DefaultPlayerSpawn.GetPosition();
							rotation = LocalZoneManager.DefaultPlayerSpawn.GetRotation();
							Debug.LogWarning(string.Format("Unknown zonePoint!!  SourceZone: {0}, TargetIndex: {1}", this.m_record.ZoningState.SourceZoneId, this.m_record.ZoningState.TargetIndex));
						}
					}
					else
					{
						position = zonePoint.GetPosition();
						rotation = zonePoint.GetRotation();
					}
				}
				this.m_record.ZoningState.State = ZoningState.None;
				this.m_record.ZoningState.SourceZoneId = LocalZoneManager.ZoneRecord.ZoneId;
				break;
			case ZoningState.GMPort:
				time = 0f;
				position = this.m_record.Location.GetPosition();
				rotation = this.m_record.Location.GetRotation();
				this.m_record.ZoningState.State = ZoningState.None;
				this.m_record.ZoningState.SourceZoneId = LocalZoneManager.ZoneRecord.ZoneId;
				break;
			case ZoningState.GMZoning:
				time = 0f;
				position = LocalZoneManager.DefaultPlayerSpawn.GetPosition();
				rotation = LocalZoneManager.DefaultPlayerSpawn.GetRotation();
				this.m_record.ZoningState.State = ZoningState.None;
				this.m_record.ZoningState.SourceZoneId = LocalZoneManager.ZoneRecord.ZoneId;
				break;
			case ZoningState.DiscoveryTeleport:
			case ZoningState.GroupTeleport:
				time = 0f;
				if (this.m_record.ZoningState.SourceZoneId != LocalZoneManager.ZoneRecord.ZoneId)
				{
					TargetPosition targetPosition = LocalZoneManager.GetTargetPosition(this.m_record.ZoningState);
					int essenceCost = this.m_record.ZoningState.EssenceCost;
					if (targetPosition != null && essenceCost > 0 && this.m_record.EmberStoneData != null)
					{
						if (this.m_record.ZoningState.UseTravelEssence)
						{
							if (this.m_record.EmberStoneData.TravelCount >= essenceCost)
							{
								this.m_record.EmberStoneData.TravelCount -= essenceCost;
							}
							else if (this.m_record.EmberStoneData.TravelCount <= 0)
							{
								this.m_record.EmberStoneData.Count -= essenceCost;
							}
							else
							{
								int num = essenceCost - this.m_record.EmberStoneData.TravelCount;
								this.m_record.EmberStoneData.TravelCount = 0;
								this.m_record.EmberStoneData.Count -= num;
							}
						}
						else
						{
							this.m_record.EmberStoneData.Count -= essenceCost;
						}
						if (this.m_record.EmberStoneData.Count < 0)
						{
							this.m_record.EmberStoneData.Count = 0;
						}
						if (this.m_record.EmberStoneData.TravelCount < 0)
						{
							this.m_record.EmberStoneData.TravelCount = 0;
						}
					}
					if (targetPosition == null)
					{
						position = LocalZoneManager.DefaultPlayerSpawn.GetPosition();
						rotation = LocalZoneManager.DefaultPlayerSpawn.GetRotation();
						Debug.LogWarning(string.Format("Unknown zonePoint!!  SourceZone: {0}, TargetIndex: {1}", this.m_record.ZoningState.SourceZoneId, this.m_record.ZoningState.TargetIndex));
					}
					else
					{
						position = targetPosition.GetPosition();
						rotation = targetPosition.GetRotation();
					}
					this.m_record.ZoningState.State = ZoningState.None;
					this.m_record.ZoningState.SourceZoneId = LocalZoneManager.ZoneRecord.ZoneId;
					this.m_record.ZoningState.TargetDiscoveryId = UniqueId.Empty;
					this.m_record.ZoningState.EssenceCost = 0;
					this.m_record.ZoningState.UseTravelEssence = false;
				}
				break;
			}
			if (this.m_record.Vitals != null && this.m_record.Vitals.Health.Value <= 0f)
			{
				PlayerSpawn respawnPoint = LocalZoneManager.GetRespawnPoint(position, this.m_record, base.GameEntity);
				position = respawnPoint.GetPosition();
				rotation = respawnPoint.GetRotation();
				this.m_record.Vitals.Health.Value = 1f;
				List<EffectRecord> effects = this.m_record.Effects;
				if (effects != null)
				{
					effects.Clear();
				}
			}
			if (LocalZoneManager.ZoneBounds.OutOfBounds(position))
			{
				Debug.Log("Out of bounds!  Moving to a safe spot");
				time = 0f;
				position = LocalZoneManager.DefaultPlayerSpawn.GetPosition();
			}
			this.m_transform.SetPositionAndRotation(position, rotation);
			base.InvokeRepeating("LogPosition", 30f, 60f);
			base.InvokeRepeating("UpdateRecordAsync", time, 30f);
		}

		// Token: 0x06002B1A RID: 11034 RVA: 0x00145D10 File Offset: 0x00143F10
		public void ZonePlayer(ZoneId targetZoneId, int targetZonePointIndex, ZoningState state)
		{
			this.m_record.ZoningState.State = state;
			this.m_record.ZoningState.SourceZoneId = LocalZoneManager.ZoneRecord.ZoneId;
			this.m_record.ZoningState.TargetZoneId = (int)targetZoneId;
			this.m_record.ZoningState.TargetIndex = targetZonePointIndex;
			this.m_record.ZoningState.TargetDiscoveryId = UniqueId.Empty;
			this.m_record.ZoningState.EssenceCost = 0;
			this.m_record.ZoningState.UseTravelEssence = false;
			this.FinalizeDb();
		}

		// Token: 0x06002B1B RID: 11035 RVA: 0x00145DA8 File Offset: 0x00143FA8
		public void ZonePlayer(ZoneId targetZoneId, UniqueId targetDiscovery, ZoningState state, int essenceCost, bool useTravelEssence)
		{
			this.m_record.ZoningState.State = state;
			this.m_record.ZoningState.SourceZoneId = LocalZoneManager.ZoneRecord.ZoneId;
			this.m_record.ZoningState.TargetZoneId = (int)targetZoneId;
			this.m_record.ZoningState.TargetIndex = 0;
			this.m_record.ZoningState.TargetDiscoveryId = targetDiscovery;
			this.m_record.ZoningState.EssenceCost = essenceCost;
			this.m_record.ZoningState.UseTravelEssence = useTravelEssence;
			this.FinalizeDb();
		}

		// Token: 0x06002B1C RID: 11036 RVA: 0x0005DECE File Offset: 0x0005C0CE
		public void ZonePlayerToCustomLocation(CharacterLocation location)
		{
			this.m_preventLocationUpdate = true;
			this.m_record.Location = location;
			this.ZonePlayer((ZoneId)location.ZoneId, 0, ZoningState.GMPort);
		}

		// Token: 0x06002B1D RID: 11037 RVA: 0x0005DEF1 File Offset: 0x0005C0F1
		public void QueueUserRecordForEventCurrencyUpdate(UserRecord record)
		{
			if (record != null)
			{
				this.m_userRecordQueuedForEventCurrencyUpdate = record;
			}
		}

		// Token: 0x06002B1E RID: 11038 RVA: 0x0005DEA9 File Offset: 0x0005C0A9
		public void FinalizeDbExternal()
		{
			this.FinalizeDb();
		}

		// Token: 0x06002B1F RID: 11039 RVA: 0x00145E40 File Offset: 0x00144040
		private void FinalizeDb()
		{
			if (this.m_finalized || !this.m_initialized)
			{
				return;
			}
			base.CancelInvoke("LogPosition");
			base.CancelInvoke("UpdateRecordAsync");
			if (base.GameEntity)
			{
				try
				{
					if (base.GameEntity.Vitals)
					{
						base.GameEntity.Vitals.FinalizeExternal();
					}
					if (base.GameEntity.CollectionController != null)
					{
						if (base.GameEntity.CollectionController.TradeId != null)
						{
							ServerGameManager.TradeManager.Server_ClientCancelTrade(base.GameEntity.CollectionController.TradeId.Value, base.GameEntity.NetworkEntity);
						}
						if (base.GameEntity.CollectionController.InteractiveStation)
						{
							base.GameEntity.CollectionController.InteractiveStation.EndInteraction(base.GameEntity, false);
						}
					}
				}
				catch
				{
				}
			}
			this.UpdateRecord();
			this.CleanupRecord();
			CorpseManager.RemoveWorldCorpse(this.m_record);
			this.m_finalized = true;
		}

		// Token: 0x06002B20 RID: 11040 RVA: 0x00145F60 File Offset: 0x00144160
		private void UpdateRecordAsync()
		{
			ServerPlayerController.<UpdateRecordAsync>d__21 <UpdateRecordAsync>d__;
			<UpdateRecordAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<UpdateRecordAsync>d__.<>4__this = this;
			<UpdateRecordAsync>d__.<>1__state = -1;
			<UpdateRecordAsync>d__.<>t__builder.Start<ServerPlayerController.<UpdateRecordAsync>d__21>(ref <UpdateRecordAsync>d__);
		}

		// Token: 0x06002B21 RID: 11041 RVA: 0x00145F98 File Offset: 0x00144198
		private void UpdateRecord()
		{
			if (this.m_record == null || this.m_transform == null)
			{
				Debug.LogWarning(string.Format("CharacterRecord Null? {0}    Transform Null? {1}", this.m_record == null, this.m_transform == null));
				return;
			}
			this.UpdateCharacterRecordCharacterLocation();
			this.m_record.UpdateRecord(ExternalGameDatabase.Database);
			if (this.m_userRecordQueuedForEventCurrencyUpdate != null)
			{
				this.m_userRecordQueuedForEventCurrencyUpdate.UpdateEventCurrency(ExternalGameDatabase.Database);
				this.m_userRecordQueuedForEventCurrencyUpdate = null;
			}
		}

		// Token: 0x06002B22 RID: 11042 RVA: 0x0005DEFD File Offset: 0x0005C0FD
		private void CleanupRecord()
		{
			CharacterRecord record = this.m_record;
			if (record == null)
			{
				return;
			}
			record.CleanupReferences();
		}

		// Token: 0x06002B23 RID: 11043 RVA: 0x00146024 File Offset: 0x00144224
		private void UpdateCharacterRecordCharacterLocation()
		{
			if (this.m_preventLocationUpdate)
			{
				return;
			}
			this.m_timePlayed.GetUpdateTimePlayed();
			this.m_record.Location.UpdateFromTransform(this.m_transform);
			this.m_record.Location.ZoneId = LocalZoneManager.ZoneRecord.ZoneId;
		}

		// Token: 0x06002B24 RID: 11044 RVA: 0x00146078 File Offset: 0x00144278
		private void LogPosition()
		{
			if (this.m_positionArguments == null)
			{
				this.m_positionArguments = new object[8];
				this.m_positionArguments[0] = LocalZoneManager.ZoneRecord.ZoneId;
				this.m_positionArguments[1] = base.GameEntity.User.Id;
				this.m_positionArguments[2] = this.m_record.Id;
			}
			if ((base.GameEntity.gameObject.transform.position - this.m_previousPosition).sqrMagnitude >= 4f)
			{
				this.m_previousPosition = base.gameObject.transform.position;
				this.m_positionArguments[3] = (base.GameEntity.CharacterData.GroupId.IsEmpty ? "None" : base.GameEntity.CharacterData.GroupId.Value);
				this.m_positionArguments[4] = base.GameEntity.gameObject.transform.position.x;
				this.m_positionArguments[5] = base.GameEntity.gameObject.transform.position.y;
				this.m_positionArguments[6] = base.GameEntity.gameObject.transform.position.z;
				this.m_positionArguments[7] = base.GameEntity.gameObject.transform.eulerAngles.y;
				SolDebug.LogToIndex(LogLevel.Information, LogIndex.Position, "{@ZoneId} {@UserId} {@CharacterId} {@GroupId} {@Xpos} {@Ypos} {@Zpos} {@Heading}", this.m_positionArguments);
			}
		}

		// Token: 0x06002B25 RID: 11045 RVA: 0x0005DF0F File Offset: 0x0005C10F
		public TimeSpan GetTotalTimePlayed()
		{
			return this.m_timePlayed.GetUpdateTimePlayed();
		}

		// Token: 0x06002B26 RID: 11046 RVA: 0x0005DF1C File Offset: 0x0005C11C
		public TimeSpan GetSessionTimePlayed()
		{
			return DateTime.UtcNow - this.m_sessionStart;
		}

		// Token: 0x04002B48 RID: 11080
		public const float kCharacterRecordUpdateRate = 30f;

		// Token: 0x04002B49 RID: 11081
		private bool m_initialized;

		// Token: 0x04002B4A RID: 11082
		private bool m_finalized;

		// Token: 0x04002B4B RID: 11083
		private bool m_preventLocationUpdate;

		// Token: 0x04002B4C RID: 11084
		private Transform m_transform;

		// Token: 0x04002B4D RID: 11085
		private CharacterRecord m_record;

		// Token: 0x04002B4E RID: 11086
		private TimePlayed m_timePlayed;

		// Token: 0x04002B4F RID: 11087
		private DateTime m_sessionStart;

		// Token: 0x04002B50 RID: 11088
		private UserRecord m_userRecordQueuedForEventCurrencyUpdate;

		// Token: 0x04002B51 RID: 11089
		private object[] m_positionArguments;

		// Token: 0x04002B52 RID: 11090
		private Vector3 m_previousPosition = Vector3.one * -100f;
	}
}
