using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Settings;
using SoL.Utilities.Extensions;

namespace SoL.Game.UI.Archetypes
{
	// Token: 0x020009C8 RID: 2504
	public class AutoAttackCooldownUI : AbilityCooldownUI
	{
		// Token: 0x170010D6 RID: 4310
		// (get) Token: 0x06004C43 RID: 19523 RVA: 0x0007395E File Offset: 0x00071B5E
		protected override bool ConsiderHaste
		{
			get
			{
				return this.m_autoAttackAbility && this.m_autoAttackAbility.ConsiderHaste;
			}
		}

		// Token: 0x170010D7 RID: 4311
		// (get) Token: 0x06004C44 RID: 19524 RVA: 0x0007397A File Offset: 0x00071B7A
		protected override bool ClampHasteTo100
		{
			get
			{
				return this.m_autoAttackAbility && this.m_autoAttackAbility.ClampHasteTo100;
			}
		}

		// Token: 0x06004C45 RID: 19525 RVA: 0x001BC5E8 File Offset: 0x001BA7E8
		protected override bool DoInternalUpdate()
		{
			return (this.m_autoAttackAbility || this.m_instance.Archetype.TryGetAsType(out this.m_autoAttackAbility)) && (this.m_instance != null && this.m_instance.AbilityData != null) && this.m_instance.AbilityData.Cooldown_Base.Elapsed != null;
		}

		// Token: 0x06004C46 RID: 19526 RVA: 0x00073996 File Offset: 0x00071B96
		protected override float GetCooldown()
		{
			if (!this.m_currentWeaponItem)
			{
				return 1f;
			}
			return (float)this.m_currentWeaponItem.Delay;
		}

		// Token: 0x06004C47 RID: 19527 RVA: 0x001BC650 File Offset: 0x001BA850
		public void RefreshWeapon()
		{
			if (!LocalPlayer.GameEntity)
			{
				this.m_currentWeaponItem = null;
				this.m_currentWeaponInstance = null;
				return;
			}
			if (!LocalPlayer.GameEntity.TryGetHandheldItem_MainHandAsType(out this.m_currentWeaponInstance, out this.m_currentWeaponItem))
			{
				this.m_currentWeaponItem = GlobalSettings.Values.Combat.FallbackWeapon;
				this.m_currentWeaponInstance = null;
			}
		}

		// Token: 0x06004C48 RID: 19528 RVA: 0x000739B7 File Offset: 0x00071BB7
		public override void ResetCooldown()
		{
			base.ResetCooldown();
			this.m_autoAttackAbility = null;
			this.m_currentWeaponItem = null;
			this.m_currentWeaponInstance = null;
		}

		// Token: 0x04004654 RID: 18004
		private AutoAttackAbility m_autoAttackAbility;

		// Token: 0x04004655 RID: 18005
		private WeaponItem m_currentWeaponItem;

		// Token: 0x04004656 RID: 18006
		private ArchetypeInstance m_currentWeaponInstance;
	}
}
