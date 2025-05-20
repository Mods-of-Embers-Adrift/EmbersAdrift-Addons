using System;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI.Archetypes
{
	// Token: 0x020009C4 RID: 2500
	public class AutoAttackEventsUI : AbilityEventsUI<AutoAttackCooldownUI>
	{
		// Token: 0x170010CB RID: 4299
		// (get) Token: 0x06004C12 RID: 19474 RVA: 0x0007371B File Offset: 0x0007191B
		public bool HandSwapActive
		{
			get
			{
				return this.m_handSwap.IsActive;
			}
		}

		// Token: 0x170010CC RID: 4300
		// (get) Token: 0x06004C13 RID: 19475 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool CanPlace
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170010CD RID: 4301
		// (get) Token: 0x06004C14 RID: 19476 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool CanModify
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170010CE RID: 4302
		// (get) Token: 0x06004C15 RID: 19477 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool ContextualDisablePanel
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170010CF RID: 4303
		// (get) Token: 0x06004C16 RID: 19478 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool IsEffectedByDaze
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170010D0 RID: 4304
		// (get) Token: 0x06004C17 RID: 19479 RVA: 0x00073728 File Offset: 0x00071928
		// (set) Token: 0x06004C18 RID: 19480 RVA: 0x001BBCA8 File Offset: 0x001B9EA8
		private ArchetypeInstance MainHandInstance
		{
			get
			{
				return this.m_mainHandInstance;
			}
			set
			{
				if (this.m_mainHandInstance == value)
				{
					return;
				}
				if (this.m_mainHandInstance != null && this.m_mainHandInstance.IsItem && this.m_mainHandInstance.Archetype is RunicBattery)
				{
					this.m_mainHandInstance.ItemData.ChargesChanged -= this.RefreshChargeLabel;
				}
				this.m_mainHandInstance = value;
				if (this.m_mainHandInstance != null && this.m_mainHandInstance.IsItem && this.m_mainHandInstance.Archetype is RunicBattery)
				{
					this.m_mainHandInstance.ItemData.ChargesChanged += this.RefreshChargeLabel;
				}
				this.RefreshChargeLabel();
			}
		}

		// Token: 0x170010D1 RID: 4305
		// (get) Token: 0x06004C19 RID: 19481 RVA: 0x00073730 File Offset: 0x00071930
		// (set) Token: 0x06004C1A RID: 19482 RVA: 0x00073738 File Offset: 0x00071938
		private bool Disabled
		{
			get
			{
				return this.m_disabled;
			}
			set
			{
				if (this.m_disabled == value)
				{
					return;
				}
				this.m_disabled = value;
				if (this.m_disabled && this.m_autoAttackEnabled)
				{
					this.m_autoAttackEnabled = false;
					this.m_enabledBorder.enabled = false;
					this.ToggleAutoAttackOverlay(false);
				}
			}
		}

		// Token: 0x06004C1B RID: 19483 RVA: 0x00073775 File Offset: 0x00071975
		private bool CanAutoAttack()
		{
			return !LocalPlayer.IsStunned && LocalPlayer.IsAlive && (!(LocalPlayer.Animancer != null) || LocalPlayer.Animancer.Stance != Stance.Swim);
		}

		// Token: 0x06004C1C RID: 19484 RVA: 0x000737A4 File Offset: 0x000719A4
		private bool PreventAutoAttack()
		{
			return !this.CanAutoAttack();
		}

		// Token: 0x06004C1D RID: 19485 RVA: 0x001BBD54 File Offset: 0x001B9F54
		protected override void Init(ArchetypeInstanceUI instanceUI)
		{
			base.Init(instanceUI);
			this.RefreshIcon();
			Color redColor = UIManager.RedColor;
			if (this.m_enabledBorder)
			{
				this.m_enabledBorder.color = redColor;
			}
			if (ClientGameManager.UIManager && ClientGameManager.UIManager.ActionBar && ClientGameManager.UIManager.ActionBar.AutoAttackSlot && ClientGameManager.UIManager.ActionBar.AutoAttackSlot.OverlayFrame)
			{
				ClientGameManager.UIManager.ActionBar.AutoAttackSlot.OverlayFrame.color = redColor;
			}
		}

		// Token: 0x06004C1E RID: 19486 RVA: 0x001BBDF8 File Offset: 0x001B9FF8
		private void RefreshIcon()
		{
			this.m_upIndicator.enabled = !LocalPlayer.GameEntity.CharacterData.MainHand_SecondaryActive;
			this.m_downIndicator.enabled = LocalPlayer.GameEntity.CharacterData.MainHand_SecondaryActive;
			ArchetypeInstance mainHandInstance;
			IHandheldItem handheldItem_MainHand = LocalPlayer.GameEntity.GetHandheldItem_MainHand(out mainHandInstance);
			this.MainHandInstance = mainHandInstance;
			this.m_ui.Icon.overrideSprite = handheldItem_MainHand.Icon;
			this.m_primary.RefreshWeapon();
		}

		// Token: 0x06004C1F RID: 19487 RVA: 0x001BBE74 File Offset: 0x001BA074
		private void RefreshChargeLabel()
		{
			this.m_chargeLabel.text = ((this.MainHandInstance != null && this.MainHandInstance.IsItem && this.MainHandInstance.ItemData.Charges != null) ? this.MainHandInstance.ItemData.Charges.Value.ToString() : string.Empty);
		}

		// Token: 0x06004C20 RID: 19488 RVA: 0x000737AF File Offset: 0x000719AF
		protected override void HandConfigurationChanged()
		{
			base.HandConfigurationChanged();
			this.RefreshIcon();
		}

		// Token: 0x06004C21 RID: 19489 RVA: 0x001BBEE4 File Offset: 0x001BA0E4
		protected override void Update()
		{
			base.Update();
			if (this.PreventAutoAttack())
			{
				return;
			}
			if (!ClientGameManager.InputManager.PreventInput && !ClientGameManager.InputManager.HoldingShift && SolInput.GetButtonDown(20))
			{
				this.ForceCombat();
				this.ToggleAutoAttack();
			}
			if (this.m_autoAttackEnabled && !this.m_ui.Locked && base.m_instance.AbilityData.Cooldown_Base.Elapsed == null && !LocalPlayer.GameEntity.SkillsController.PendingIsActive && !LocalPlayer.GameEntity.SkillsController.AutoAttackPending && LocalPlayer.GameEntity.SkillsController.AutoAttackServerFailedDelayUntil == null && LocalPlayer.GameEntity.TargetController.OffensiveTarget && LocalPlayer.GameEntity.TargetController.OffensiveTarget.VitalsReplicator && LocalPlayer.GameEntity.TargetController.OffensiveTarget.VitalsReplicator.CurrentHealthState.Value == HealthState.Alive)
			{
				bool flag = Time.time - this.m_timeOfLastFailureMessage > 5f;
				if (LocalPlayer.GameEntity.SkillsController.ExecuteAutoAttack(base.m_instance, flag) == null && flag)
				{
					this.m_timeOfLastFailureMessage = Time.time;
				}
			}
		}

		// Token: 0x06004C22 RID: 19490 RVA: 0x000737BD File Offset: 0x000719BD
		protected override void RefreshDisabledPanel()
		{
			base.RefreshDisabledPanel();
			this.Disabled = (this.m_ui.DisabledPanel.alpha >= 1f);
		}

		// Token: 0x06004C23 RID: 19491 RVA: 0x000737E5 File Offset: 0x000719E5
		protected override void EquipmentOnContentsChanged()
		{
			base.EquipmentOnContentsChanged();
			this.RefreshIcon();
		}

		// Token: 0x06004C24 RID: 19492 RVA: 0x000737F3 File Offset: 0x000719F3
		protected override bool OverrideOnPointerUp(PointerEventData eventData)
		{
			if (this.CanAutoAttack())
			{
				this.ForceCombat();
				this.ToggleAutoAttack();
			}
			return true;
		}

		// Token: 0x06004C25 RID: 19493 RVA: 0x0007380A File Offset: 0x00071A0A
		private void ToggleAutoAttack()
		{
			if (this.PreventAutoAttack())
			{
				return;
			}
			if (!this.Disabled)
			{
				this.m_autoAttackEnabled = !this.m_autoAttackEnabled;
				this.m_enabledBorder.enabled = this.m_autoAttackEnabled;
				this.ToggleAutoAttackOverlay(this.m_autoAttackEnabled);
			}
		}

		// Token: 0x06004C26 RID: 19494 RVA: 0x00073849 File Offset: 0x00071A49
		private void ForceCombat()
		{
			if (this.PreventAutoAttack())
			{
				return;
			}
			if (!this.m_autoAttackEnabled)
			{
				LocalPlayer.Animancer.ForceCombat(false);
			}
		}

		// Token: 0x06004C27 RID: 19495 RVA: 0x00073867 File Offset: 0x00071A67
		public void InitiateCombat()
		{
			if (this.PreventAutoAttack())
			{
				return;
			}
			if (!this.m_autoAttackEnabled)
			{
				this.ForceCombat();
				this.ToggleAutoAttack();
			}
		}

		// Token: 0x06004C28 RID: 19496 RVA: 0x00073886 File Offset: 0x00071A86
		public void DisableAutoAttack()
		{
			if (this.m_autoAttackEnabled)
			{
				this.ToggleAutoAttack();
			}
		}

		// Token: 0x06004C29 RID: 19497 RVA: 0x00073896 File Offset: 0x00071A96
		protected override void CurrentStanceOnChanged(Stance stance)
		{
			base.CurrentStanceOnChanged(stance);
			if (stance != Stance.Combat)
			{
				this.Disabled = true;
			}
		}

		// Token: 0x06004C2A RID: 19498 RVA: 0x000738AA File Offset: 0x00071AAA
		private void Awake()
		{
			this.m_chargeLabel.text = string.Empty;
			UIManager.AutoAttackButton = this;
		}

		// Token: 0x06004C2B RID: 19499 RVA: 0x000738C2 File Offset: 0x00071AC2
		protected override bool OverrideFillActionsGetTitle(out string result)
		{
			result = string.Empty;
			return true;
		}

		// Token: 0x06004C2C RID: 19500 RVA: 0x001BC048 File Offset: 0x001BA248
		private void ToggleAutoAttackOverlay(bool isEnabled)
		{
			if (ClientGameManager.UIManager && ClientGameManager.UIManager.ActionBar && ClientGameManager.UIManager.ActionBar.AutoAttackSlot && ClientGameManager.UIManager.ActionBar.AutoAttackSlot.OverlayFrame)
			{
				ClientGameManager.UIManager.ActionBar.AutoAttackSlot.OverlayFrame.enabled = isEnabled;
			}
		}

		// Token: 0x0400463B RID: 17979
		private bool m_disabled = true;

		// Token: 0x0400463C RID: 17980
		private bool m_autoAttackEnabled;

		// Token: 0x0400463D RID: 17981
		[SerializeField]
		private Image m_enabledBorder;

		// Token: 0x0400463E RID: 17982
		[SerializeField]
		private Image m_upIndicator;

		// Token: 0x0400463F RID: 17983
		[SerializeField]
		private Image m_downIndicator;

		// Token: 0x04004640 RID: 17984
		[SerializeField]
		private TextMeshProUGUI m_chargeLabel;

		// Token: 0x04004641 RID: 17985
		private ArchetypeInstance m_mainHandInstance;

		// Token: 0x04004642 RID: 17986
		private const float kTimeBetweenFailureMessages = 5f;

		// Token: 0x04004643 RID: 17987
		private float m_timeOfLastFailureMessage;
	}
}
