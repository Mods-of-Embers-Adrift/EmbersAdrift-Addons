using System;
using SoL.Game.Animation;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Game.SkyDome;
using UnityEngine;

namespace SoL.Game.NPCs
{
	// Token: 0x02000818 RID: 2072
	public class NpcStanceManager : GameEntityComponent
	{
		// Token: 0x17000DCF RID: 3535
		// (get) Token: 0x06003C05 RID: 15365 RVA: 0x00068AD2 File Offset: 0x00066CD2
		// (set) Token: 0x06003C06 RID: 15366 RVA: 0x00068ADA File Offset: 0x00066CDA
		public CombatMasteryArchetype CombatMastery { private get; set; }

		// Token: 0x17000DD0 RID: 3536
		// (get) Token: 0x06003C07 RID: 15367 RVA: 0x00068AE3 File Offset: 0x00066CE3
		// (set) Token: 0x06003C08 RID: 15368 RVA: 0x00068AEB File Offset: 0x00066CEB
		public AnimancerAnimationSet CombatStance { private get; set; }

		// Token: 0x17000DD1 RID: 3537
		// (get) Token: 0x06003C09 RID: 15369 RVA: 0x00068AF4 File Offset: 0x00066CF4
		// (set) Token: 0x06003C0A RID: 15370 RVA: 0x00068AFC File Offset: 0x00066CFC
		public bool WeaponsAlwaysMounted { get; set; }

		// Token: 0x17000DD2 RID: 3538
		// (get) Token: 0x06003C0B RID: 15371 RVA: 0x00068B05 File Offset: 0x00066D05
		// (set) Token: 0x06003C0C RID: 15372 RVA: 0x00068B0D File Offset: 0x00066D0D
		public bool HasLight { get; set; }

		// Token: 0x06003C0D RID: 15373 RVA: 0x00068B16 File Offset: 0x00066D16
		private void Awake()
		{
			if (!base.GameEntity)
			{
				base.enabled = false;
				return;
			}
			base.GameEntity.NpcStanceManager = this;
		}

		// Token: 0x06003C0E RID: 15374 RVA: 0x0004475B File Offset: 0x0004295B
		private void OnDestroy()
		{
		}

		// Token: 0x06003C0F RID: 15375 RVA: 0x0017DEB4 File Offset: 0x0017C0B4
		private void Update()
		{
			if (!base.GameEntity || !base.GameEntity.Vitals || !base.GameEntity.CharacterData || base.GameEntity.Vitals.Health <= 0f || base.GameEntity.IsStunned)
			{
				return;
			}
			this.ValidateItemsAttached();
			int hostileTargetCount = this.m_targetController.HostileTargetCount;
			bool flag = this.HasLight && this.m_serverNpcController && this.m_serverNpcController.Interactive && this.m_serverNpcController.Interactive.UseLightItemAtNight && !SkyDomeManager.IsDay();
			bool isSwimming = base.GameEntity.CharacterData.IsSwimming;
			if (flag != this.m_previousUseLight || hostileTargetCount != this.m_previousHostileTargetCount || isSwimming != this.m_previousIsSwimming)
			{
				this.UpdateStanceId(hostileTargetCount, flag, isSwimming);
			}
			this.m_previousUseLight = flag;
			this.m_previousHostileTargetCount = hostileTargetCount;
			this.m_previousIsSwimming = isSwimming;
		}

		// Token: 0x06003C10 RID: 15376 RVA: 0x0017DFB8 File Offset: 0x0017C1B8
		private void UpdateStanceId(int currentHostileCount, bool useLight, bool isSwimming)
		{
			UniqueId uniqueId = base.GameEntity.CharacterData.CurrentStanceData.Value.StanceId;
			if (isSwimming)
			{
				uniqueId = GlobalSettings.Values.Stance.SwimmingStance.AnimationSet.Id;
			}
			else if (currentHostileCount > 0)
			{
				if (this.CombatStance)
				{
					uniqueId = this.CombatStance.Id;
				}
				else if (this.CombatMastery && this.CombatMastery.Stance)
				{
					uniqueId = this.CombatMastery.Stance.Id;
				}
				else
				{
					uniqueId = GlobalSettings.Values.Animation.FallbackCombatSet.Id;
				}
			}
			else
			{
				uniqueId = (useLight ? GlobalSettings.Values.Animation.TorchPose.Id : GlobalSettings.Values.Animation.IdleSetPair.Id);
			}
			if (uniqueId != base.GameEntity.CharacterData.CurrentStanceData.Value.StanceId)
			{
				this.m_itemsAttachedValidateTime = new float?(Time.time + 1.1f);
			}
			base.GameEntity.CharacterData.CurrentStanceData.Value = new StanceData
			{
				BypassTransition = false,
				StanceId = uniqueId
			};
		}

		// Token: 0x06003C11 RID: 15377 RVA: 0x0017E100 File Offset: 0x0017C300
		private void ValidateItemsAttached()
		{
			if (this.m_itemsAttachedValidateTime != null && Time.time >= this.m_itemsAttachedValidateTime.Value)
			{
				ItemsAttached itemsAttached = ItemsAttached.None;
				if (this.m_targetController.HostileTargetCount > 0 || this.WeaponsAlwaysMounted)
				{
					itemsAttached |= ItemsAttached.Weapons;
				}
				else if (base.GameEntity.CharacterData.CurrentStanceData.Value.StanceId == GlobalSettings.Values.Animation.TorchPose.Id)
				{
					itemsAttached |= ItemsAttached.Light;
				}
				base.GameEntity.CharacterData.ItemsAttached.Value = itemsAttached;
				this.m_itemsAttachedValidateTime = null;
			}
		}

		// Token: 0x04003ABC RID: 15036
		private bool m_previousUseLight;

		// Token: 0x04003ABD RID: 15037
		private bool m_previousIsSwimming;

		// Token: 0x04003ABE RID: 15038
		private int m_previousHostileTargetCount;

		// Token: 0x04003ABF RID: 15039
		private float? m_itemsAttachedValidateTime;

		// Token: 0x04003AC0 RID: 15040
		[SerializeField]
		private ServerNpcController m_serverNpcController;

		// Token: 0x04003AC1 RID: 15041
		[SerializeField]
		private NpcTargetController m_targetController;
	}
}
