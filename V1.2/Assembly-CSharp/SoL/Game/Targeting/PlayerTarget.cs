using System;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Managers;
using SoL.Networking.Objects;
using SoL.Networking.Replication;
using UnityEngine;

namespace SoL.Game.Targeting
{
	// Token: 0x02000657 RID: 1623
	internal class PlayerTarget
	{
		// Token: 0x17000AC6 RID: 2758
		// (get) Token: 0x06003282 RID: 12930 RVA: 0x00062CC1 File Offset: 0x00060EC1
		public TargetType TargetType
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x17000AC7 RID: 2759
		// (get) Token: 0x06003283 RID: 12931 RVA: 0x00062CC9 File Offset: 0x00060EC9
		// (set) Token: 0x06003284 RID: 12932 RVA: 0x00062CD1 File Offset: 0x00060ED1
		public NameplateControllerUI NameplateUI { get; set; }

		// Token: 0x17000AC8 RID: 2760
		// (get) Token: 0x06003285 RID: 12933 RVA: 0x00062CDA File Offset: 0x00060EDA
		// (set) Token: 0x06003286 RID: 12934 RVA: 0x00160438 File Offset: 0x0015E638
		public ITargetable Target
		{
			get
			{
				return this.m_target;
			}
			set
			{
				if (this.m_target == value)
				{
					return;
				}
				if (this.m_type == TargetType.Offensive)
				{
					this.Unsubscribe();
				}
				this.m_target = value;
				if (this.m_syncId != null)
				{
					if (this.m_target == null)
					{
						this.m_syncId.Value = 0U;
					}
					else
					{
						this.m_syncId.Value = this.m_target.Entity.NetworkEntity.NetworkId.Value;
					}
				}
				TargetType type = this.m_type;
				if (type != TargetType.Offensive)
				{
					if (type == TargetType.Defensive)
					{
						BaseTargetController controller = this.m_controller;
						ITargetable target = this.m_target;
						controller.DefensiveTarget = ((target != null) ? target.Entity : null);
					}
				}
				else
				{
					BaseTargetController controller2 = this.m_controller;
					ITargetable target2 = this.m_target;
					controller2.OffensiveTarget = ((target2 != null) ? target2.Entity : null);
					this.Subscribe();
				}
				if (this.NameplateUI)
				{
					this.NameplateUI.Init(this.m_target);
				}
				if (this.m_target != null)
				{
					this.m_lastValidTargetId = this.m_target.Entity.NetworkEntity.NetworkId.Value;
				}
				if (this.m_targetReticle)
				{
					this.m_targetReticle.UpdateSize(this.m_target);
				}
			}
		}

		// Token: 0x06003287 RID: 12935 RVA: 0x00160564 File Offset: 0x0015E764
		public PlayerTarget(BaseTargetController controller, SynchronizedUInt syncId, TargetType targetType)
		{
			this.m_controller = controller;
			this.m_syncId = syncId;
			this.m_type = targetType;
			NetworkEntity networkEntity;
			if (syncId.Value != 0U && NetworkManager.EntityManager.TryGetNetworkEntity(syncId.Value, out networkEntity))
			{
				this.Target = networkEntity.GameEntity.Targetable;
			}
			else
			{
				this.Target = null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(GlobalSettings.Values.Player.ReticlePrefab, Vector3.zero, Quaternion.identity);
			this.m_targetReticle = gameObject.GetComponent<TargetReticle>();
			this.m_targetReticle.gameObject.SetActive(false);
			this.m_targetReticle.Init(targetType);
		}

		// Token: 0x06003288 RID: 12936 RVA: 0x0016060C File Offset: 0x0015E80C
		public void ValidateTarget()
		{
			if (this.Target != null)
			{
				bool flag = false;
				if (this.Target.Entity && this.Target.Entity.CharacterData && this.Target.Entity.gameObject && !this.Target.Entity.IsDestroying)
				{
					PlayerFlags value = this.Target.Entity.CharacterData.CharacterFlags.Value;
					TargetType type = this.m_type;
					if (type != TargetType.Offensive)
					{
						if (type == TargetType.Defensive)
						{
							flag = !value.HasBitFlag(PlayerFlags.Invisible);
							if (flag && this.Target.Entity.Type == GameEntityType.Player && this.Target.Entity != LocalPlayer.GameEntity)
							{
								bool flag2 = value.HasBitFlag(PlayerFlags.Pvp);
								bool isPvp = LocalPlayer.IsPvp;
								if (flag2 != isPvp)
								{
									flag = false;
								}
								else if (flag2 && isPvp)
								{
									flag = ClientGroupManager.IsMyGroup(this.Target.Entity.CharacterData.GroupId);
								}
							}
						}
					}
					else
					{
						flag = ((this.m_controller.GameEntity.gameObject.transform.position - this.Target.Entity.gameObject.transform.position).sqrMagnitude <= this.m_controller.GameEntity.Vitals.MaxTargetDistanceSqr);
						if (flag && this.Target.Entity.Type == GameEntityType.Player)
						{
							flag = (!value.HasBitFlag(PlayerFlags.Invisible) && value.HasBitFlag(PlayerFlags.Pvp) && LocalPlayer.IsPvp && this.Target.Entity != LocalPlayer.GameEntity && !ClientGroupManager.IsMyGroup(this.Target.Entity.CharacterData.GroupId));
						}
					}
				}
				if (!flag)
				{
					this.Target = null;
					this.m_lastTargetInvalid = Time.time;
				}
			}
		}

		// Token: 0x06003289 RID: 12937 RVA: 0x00160824 File Offset: 0x0015EA24
		public void RefreshReticle()
		{
			if (this.Target == null || UIManager.UiHidden)
			{
				if (this.m_targetReticle.gameObject.activeSelf)
				{
					this.m_targetReticle.gameObject.SetActive(false);
					return;
				}
			}
			else
			{
				this.m_targetReticle.UpdatePosition(this.m_target);
				if (!this.m_targetReticle.gameObject.activeSelf)
				{
					this.m_targetReticle.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x0600328A RID: 12938 RVA: 0x00160898 File Offset: 0x0015EA98
		private void Subscribe()
		{
			if (this.m_target != null && this.m_target.Entity && this.m_target.Entity.VitalsReplicator)
			{
				this.m_target.Entity.VitalsReplicator.CurrentHealthState.Changed += this.CurrentHealthStateOnChanged;
			}
		}

		// Token: 0x0600328B RID: 12939 RVA: 0x001608FC File Offset: 0x0015EAFC
		private void Unsubscribe()
		{
			if (this.m_target != null && this.m_target.Entity && this.m_target.Entity.VitalsReplicator)
			{
				this.m_target.Entity.VitalsReplicator.CurrentHealthState.Changed -= this.CurrentHealthStateOnChanged;
			}
		}

		// Token: 0x0600328C RID: 12940 RVA: 0x00062CE2 File Offset: 0x00060EE2
		private void CurrentHealthStateOnChanged(HealthState obj)
		{
			if (obj == HealthState.Dead && Options.GameOptions.DisableAutoAttackOnTargetDeath.Value && UIManager.AutoAttackButton)
			{
				UIManager.AutoAttackButton.DisableAutoAttack();
			}
		}

		// Token: 0x0600328D RID: 12941 RVA: 0x00160960 File Offset: 0x0015EB60
		public void ReplacementNetworkEntitySpawned(uint isReplacingId, NetworkEntity incomingReplacement)
		{
			if (this.m_type == TargetType.Offensive && isReplacingId == this.m_lastValidTargetId && (this.m_target != null || Time.time - this.m_lastTargetInvalid <= 5f) && incomingReplacement && incomingReplacement.GameEntity && incomingReplacement.GameEntity.Targetable != null)
			{
				this.Target = incomingReplacement.GameEntity.Targetable;
			}
		}

		// Token: 0x040030E9 RID: 12521
		private BaseTargetController m_controller;

		// Token: 0x040030EA RID: 12522
		private SynchronizedUInt m_syncId;

		// Token: 0x040030EB RID: 12523
		private ITargetable m_target;

		// Token: 0x040030EC RID: 12524
		private TargetType m_type;

		// Token: 0x040030ED RID: 12525
		private TargetReticle m_targetReticle;

		// Token: 0x040030EF RID: 12527
		private uint m_lastValidTargetId;

		// Token: 0x040030F0 RID: 12528
		private float m_lastTargetInvalid;

		// Token: 0x040030F1 RID: 12529
		private const float kReplacementThreshold = 5f;
	}
}
