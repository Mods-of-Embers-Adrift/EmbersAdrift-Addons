using System;
using Cysharp.Text;
using SoL.Game.EffectSystem;
using SoL.Game.Messages;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.UI.Archetypes
{
	// Token: 0x020009BD RID: 2493
	public abstract class AbilityEventsUI<T> : ArchetypeInstanceEventsUI<T> where T : BaseCooldownUI
	{
		// Token: 0x1700108F RID: 4239
		// (get) Token: 0x06004B5D RID: 19293 RVA: 0x00073053 File Offset: 0x00071253
		protected override bool CanModify
		{
			get
			{
				return base.CanModify && !this.m_handSwap.IsActive;
			}
		}

		// Token: 0x17001090 RID: 4240
		// (get) Token: 0x06004B5E RID: 19294 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool IsEffectedByDaze
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06004B5F RID: 19295 RVA: 0x0007306D File Offset: 0x0007126D
		protected override bool EarlyOutOfUpdate()
		{
			return base.EarlyOutOfUpdate() || this.m_isAura;
		}

		// Token: 0x06004B60 RID: 19296 RVA: 0x0007307F File Offset: 0x0007127F
		protected override void Init(ArchetypeInstanceUI instanceUI)
		{
			base.Init(instanceUI);
			this.m_memorization.Init(this.m_ui, AbilityCooldownFlags.Memorization);
			this.m_handSwap.Init(this.m_ui, AbilityCooldownFlags.WeaponRuneSwap);
		}

		// Token: 0x06004B61 RID: 19297 RVA: 0x001B86EC File Offset: 0x001B68EC
		protected override void ResetInternal()
		{
			base.ResetInternal();
			this.m_memorization.ResetCooldown();
			this.m_handSwap.ResetCooldown();
			this.m_isAura = false;
			this.m_unlinkOffhandSwap = false;
			this.m_currentStance = Stance.Idle;
			this.m_executable = null;
			this.m_runicAbility = null;
			this.m_reagentAbility = null;
		}

		// Token: 0x06004B62 RID: 19298 RVA: 0x000730AD File Offset: 0x000712AD
		protected override bool CooldownsActive()
		{
			return base.CooldownsActive() || this.m_memorization.IsActive || this.m_handSwap.IsActive;
		}

		// Token: 0x06004B63 RID: 19299 RVA: 0x001B8740 File Offset: 0x001B6940
		protected override void Subscribe()
		{
			base.Subscribe();
			if (base.m_instance == null || !base.m_instance.IsAbility || !LocalPlayer.GameEntity)
			{
				return;
			}
			this.m_executable = base.m_instance.Ability;
			base.m_instance.AbilityData.TimeOfLastUseChanged += this.AbilityTimeOfLastUseChanged;
			base.m_instance.AbilityData.MemorizationTimestampChanged += this.AbilityMemorizationTimestampChanged;
			LocalPlayer.GameEntity.VitalsReplicator.StaminaSyncVar.Changed += this.StaminaOnChanged;
			LocalPlayer.GameEntity.VitalsReplicator.CurrentStance.Changed += this.CurrentStanceOnChanged;
			LocalPlayer.GameEntity.VitalsReplicator.BehaviorFlags.Changed += this.BehaviorFlagsOnChanged;
			LocalPlayer.GameEntity.Vitals.StatsChanged += this.VitalsOnStatsChanged;
			LocalPlayer.GameEntity.CharacterData.MasteryConfigurationChanged += this.MasteryConfigurationOnChanged;
			LocalPlayer.GameEntity.CharacterData.HandConfigurationChanged += this.HandConfigurationChanged;
			LocalPlayer.GameEntity.CollectionController.Equipment.ContentsChanged += this.EquipmentOnContentsChanged;
			this.StaminaOnChanged(LocalPlayer.GameEntity.VitalsReplicator.StaminaSyncVar.Value);
			this.BehaviorFlagsOnChanged(LocalPlayer.GameEntity.VitalsReplicator.BehaviorFlags.Value);
			LocalClientSkillsController.MasteryLevelChangedEvent += this.OnMasteryLevelChanged;
			AbilityArchetype abilityArchetype;
			ArchetypeInstance masteryInstance;
			if (base.m_instance.Archetype.TryGetAsType(out abilityArchetype) && LocalPlayer.GameEntity.CollectionController.Masteries.TryGetInstanceForArchetypeId(abilityArchetype.Mastery.Id, out masteryInstance))
			{
				this.OnMasteryLevelChanged(masteryInstance);
			}
			EquipmentSlotUI.HandEquipmentUpdated += this.HandEquipmentUpdated;
			LocalPlayer.GameEntity.SkillsController.PendingExecutionChanged += base.SkillsControllerOnPendingExecutionChanged;
			LocalPlayer.GameEntity.SkillsController.TriggerGlobalCooldown += base.SkillsControllerOnTriggerGlobalCooldown;
			this.m_isAura = (base.m_instance.Ability as AuraAbility);
			if (this.m_isAura)
			{
				LocalPlayer.ActiveAuraChanged += this.LocalPlayerOnActiveAuraChanged;
			}
			this.m_runicAbility = (base.m_instance.Ability as RunicAbility);
			if (this.m_runicAbility)
			{
				this.EquipmentUIOnAvailableChargesChanged();
				EquipmentUI.AvailableChargesChanged += this.EquipmentUIOnAvailableChargesChanged;
			}
			this.m_reagentAbility = (base.m_instance.Ability as ReagentAbility);
			if (this.m_reagentAbility)
			{
				this.PouchOnContentsChanged();
				LocalPlayer.GameEntity.CollectionController.ReagentPouch.QuantityOfItemChanged += this.ReagentPouchOnQuantityOfItemChanged;
				LocalPlayer.GameEntity.CollectionController.ReagentPouch.ToggleChanged += this.ReagentPouchOnQuantityOfItemChanged;
				LocalPlayer.GameEntity.CollectionController.ReagentPouch.ContentsChanged += this.PouchOnContentsChanged;
			}
		}

		// Token: 0x06004B64 RID: 19300 RVA: 0x001B8A58 File Offset: 0x001B6C58
		protected override void Unsubscribe()
		{
			base.Unsubscribe();
			if (base.m_instance != null && base.m_instance.IsAbility)
			{
				base.m_instance.AbilityData.TimeOfLastUseChanged -= this.AbilityTimeOfLastUseChanged;
				base.m_instance.AbilityData.MemorizationTimestampChanged -= this.AbilityMemorizationTimestampChanged;
			}
			if (LocalPlayer.GameEntity)
			{
				if (LocalPlayer.GameEntity.VitalsReplicator)
				{
					LocalPlayer.GameEntity.VitalsReplicator.StaminaSyncVar.Changed -= this.StaminaOnChanged;
					LocalPlayer.GameEntity.VitalsReplicator.CurrentStance.Changed -= this.CurrentStanceOnChanged;
					LocalPlayer.GameEntity.VitalsReplicator.BehaviorFlags.Changed -= this.BehaviorFlagsOnChanged;
				}
				if (LocalPlayer.GameEntity.Vitals)
				{
					LocalPlayer.GameEntity.Vitals.StatsChanged -= this.VitalsOnStatsChanged;
				}
				if (LocalPlayer.GameEntity.CharacterData)
				{
					LocalPlayer.GameEntity.CharacterData.MasteryConfigurationChanged -= this.MasteryConfigurationOnChanged;
					LocalPlayer.GameEntity.CharacterData.HandConfigurationChanged -= this.HandConfigurationChanged;
				}
				if (LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Equipment != null)
				{
					LocalPlayer.GameEntity.CollectionController.Equipment.ContentsChanged -= this.EquipmentOnContentsChanged;
				}
			}
			LocalClientSkillsController.MasteryLevelChangedEvent -= this.OnMasteryLevelChanged;
			EquipmentSlotUI.HandEquipmentUpdated -= this.HandEquipmentUpdated;
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.SkillsController)
			{
				LocalPlayer.GameEntity.SkillsController.PendingExecutionChanged -= base.SkillsControllerOnPendingExecutionChanged;
				LocalPlayer.GameEntity.SkillsController.TriggerGlobalCooldown -= base.SkillsControllerOnTriggerGlobalCooldown;
			}
			if (this.m_isAura)
			{
				LocalPlayer.ActiveAuraChanged -= this.LocalPlayerOnActiveAuraChanged;
			}
			if (this.m_runicAbility)
			{
				EquipmentUI.AvailableChargesChanged -= this.EquipmentUIOnAvailableChargesChanged;
			}
			if (this.m_reagentAbility && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Pouch != null)
			{
				LocalPlayer.GameEntity.CollectionController.ReagentPouch.QuantityOfItemChanged -= this.ReagentPouchOnQuantityOfItemChanged;
				LocalPlayer.GameEntity.CollectionController.ReagentPouch.ToggleChanged -= this.ReagentPouchOnQuantityOfItemChanged;
				LocalPlayer.GameEntity.CollectionController.ReagentPouch.ContentsChanged -= this.PouchOnContentsChanged;
			}
		}

		// Token: 0x06004B65 RID: 19301 RVA: 0x001B8D34 File Offset: 0x001B6F34
		private void EquipmentUIOnAvailableChargesChanged()
		{
			RuneMasteryArchetype runeMasteryArchetype;
			int num;
			if (this.m_runicAbility && this.m_runicAbility.Mastery.TryGetAsType(out runeMasteryArchetype) && EquipmentUI.TryGetAvailableCharges(runeMasteryArchetype.RuneSource, out num))
			{
				int arg = (num > 0 && this.m_runicAbility.UseCost > 0) ? Mathf.FloorToInt((float)num / (float)this.m_runicAbility.UseCost) : 0;
				this.m_ui.UpperRightLabel.SetTextFormat("{0}", arg);
			}
		}

		// Token: 0x06004B66 RID: 19302 RVA: 0x000730D1 File Offset: 0x000712D1
		private void PouchOnContentsChanged()
		{
			this.RefreshDisabledPanelInternal();
			this.ReagentPouchOnQuantityOfItemChanged();
		}

		// Token: 0x06004B67 RID: 19303 RVA: 0x001B8DB4 File Offset: 0x001B6FB4
		private void ReagentPouchOnQuantityOfItemChanged()
		{
			if (!this.m_reagentAbility || this.m_executable == null)
			{
				return;
			}
			int adventuringLevel = LocalPlayer.GameEntity.CharacterData.AdventuringLevel;
			ArchetypeInstance reagentForEntity = this.m_reagentAbility.Type.GetReagentForEntity(LocalPlayer.GameEntity, (float)adventuringLevel);
			int num = (reagentForEntity != null && reagentForEntity.ItemData != null && reagentForEntity.ItemData.Quantity != null) ? reagentForEntity.ItemData.Quantity.Value : 0;
			if (num <= 0)
			{
				this.m_ui.UpperRightLabel.ZStringSetText("-");
				return;
			}
			this.m_ui.UpperRightLabel.SetTextFormat("{0}", num);
		}

		// Token: 0x06004B68 RID: 19304 RVA: 0x000730DF File Offset: 0x000712DF
		private void RefreshDisabledPanelInternal()
		{
			this.m_ui.RefreshDisabledPanel();
		}

		// Token: 0x06004B69 RID: 19305 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void AbilityTimeOfLastUseChanged()
		{
		}

		// Token: 0x06004B6A RID: 19306 RVA: 0x001B8E68 File Offset: 0x001B7068
		private void AbilityMemorizationTimestampChanged()
		{
			ArchetypeInstance instance = base.m_instance;
			if (instance != null && instance.AbilityData.MemorizationTimestamp != null)
			{
				this.m_memorization.StartCooldown((float)base.m_instance.Ability.Memorization);
				return;
			}
			this.m_memorization.ClearCooldown();
		}

		// Token: 0x06004B6B RID: 19307 RVA: 0x001B8EC0 File Offset: 0x001B70C0
		private void StaminaOnChanged(byte value)
		{
			if (this.m_ui.LowStaminaCanvas && !base.m_instance.AbilityData.IsDynamic)
			{
				this.m_ui.LowStaminaCanvas.alpha = (base.m_instance.Ability.HasEnoughStamina(LocalPlayer.GameEntity, base.m_instance.GetAssociatedLevel(LocalPlayer.GameEntity)) ? 0f : 1f);
			}
		}

		// Token: 0x06004B6C RID: 19308 RVA: 0x001B8F34 File Offset: 0x001B7134
		private void BehaviorFlagsOnChanged(BehaviorEffectTypeFlags obj)
		{
			if (this.m_ui.BehaviorCanvas)
			{
				float alpha = 0f;
				if (obj.HasBitFlag(BehaviorEffectTypeFlags.Stunned))
				{
					alpha = 1f;
				}
				else if (this.IsEffectedByDaze && obj.HasBitFlag(BehaviorEffectTypeFlags.Dazed))
				{
					alpha = 1f;
				}
				this.m_ui.BehaviorCanvas.alpha = alpha;
			}
		}

		// Token: 0x06004B6D RID: 19309 RVA: 0x000730EC File Offset: 0x000712EC
		protected virtual void CurrentStanceOnChanged(Stance stance)
		{
			this.m_ui.RefreshDisabledPanel();
			this.m_currentStance = stance;
			if (base.m_instance.IsAbility)
			{
				this.m_ui.ToggleLock(stance == Stance.Combat);
			}
		}

		// Token: 0x06004B6E RID: 19310 RVA: 0x0004475B File Offset: 0x0004295B
		private void VitalsOnStatsChanged()
		{
		}

		// Token: 0x06004B6F RID: 19311 RVA: 0x000730DF File Offset: 0x000712DF
		private void MasteryConfigurationOnChanged()
		{
			this.m_ui.RefreshDisabledPanel();
		}

		// Token: 0x06004B70 RID: 19312 RVA: 0x0007311C File Offset: 0x0007131C
		protected virtual void HandConfigurationChanged()
		{
			this.m_ui.RefreshDisabledPanel();
			if (this.m_unlinkOffhandSwap)
			{
				this.m_unlinkOffhandSwap = false;
				return;
			}
			this.m_handSwap.StartCooldown(2f);
		}

		// Token: 0x06004B71 RID: 19313 RVA: 0x000730DF File Offset: 0x000712DF
		protected virtual void EquipmentOnContentsChanged()
		{
			this.m_ui.RefreshDisabledPanel();
		}

		// Token: 0x06004B72 RID: 19314 RVA: 0x001B8F94 File Offset: 0x001B7194
		private void OnMasteryLevelChanged(ArchetypeInstance masteryInstance)
		{
			if (masteryInstance != null && base.m_instance != null && base.m_instance.Ability && base.m_instance.Ability.Mastery && base.m_instance.Ability.Mastery.Id == masteryInstance.ArchetypeId)
			{
				float alpha = this.m_ui.LowLevelCanvas.alpha;
				float num = (base.m_instance.GetAssociatedLevel(LocalPlayer.GameEntity) < (float)base.m_instance.Ability.MinimumLevel) ? 1f : 0f;
				this.m_ui.LowLevelCanvas.alpha = num;
				if (alpha >= 1f && num <= 0f)
				{
					string text = "You can now use " + base.m_instance.Archetype.GetModifiedDisplayName(base.m_instance) + "!";
					CenterScreenAnnouncementOptions opts = new CenterScreenAnnouncementOptions
					{
						Title = "New Ability Unlocked",
						TimeShown = 3f,
						ShowDelay = 0f,
						Text = text,
						MessageType = MessageType.Skills
					};
					ClientGameManager.UIManager.InitCenterScreenAnnouncement(opts);
				}
				this.ReagentPouchOnQuantityOfItemChanged();
			}
		}

		// Token: 0x06004B73 RID: 19315 RVA: 0x00073149 File Offset: 0x00071349
		private void HandEquipmentUpdated()
		{
			this.m_handSwap.StartCooldown(2f);
		}

		// Token: 0x06004B74 RID: 19316 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void LocalPlayerOnActiveAuraChanged(UniqueId previous, UniqueId current)
		{
		}

		// Token: 0x040045E3 RID: 17891
		private const float kWeaponRuneSwapCooldown = 2f;

		// Token: 0x040045E4 RID: 17892
		private const float kWeaponRuneUnlinkedOffhandSwapCooldown = 1f;

		// Token: 0x040045E5 RID: 17893
		[SerializeField]
		private FixedCooldownUI m_memorization;

		// Token: 0x040045E6 RID: 17894
		[SerializeField]
		protected FixedCooldownUI m_handSwap;

		// Token: 0x040045E7 RID: 17895
		private bool m_isAura;

		// Token: 0x040045E8 RID: 17896
		private bool m_unlinkOffhandSwap;

		// Token: 0x040045E9 RID: 17897
		private Stance m_currentStance;

		// Token: 0x040045EA RID: 17898
		private IExecutable m_executable;

		// Token: 0x040045EB RID: 17899
		private RunicAbility m_runicAbility;

		// Token: 0x040045EC RID: 17900
		private ReagentAbility m_reagentAbility;
	}
}
