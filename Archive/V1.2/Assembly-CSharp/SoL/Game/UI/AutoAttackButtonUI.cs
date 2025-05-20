using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.UI.Archetypes;
using SoL.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x02000854 RID: 2132
	[Obsolete]
	public class AutoAttackButtonUI : ArchetypeInstanceUI
	{
		// Token: 0x17000E3D RID: 3645
		// (get) Token: 0x06003D8E RID: 15758 RVA: 0x00045BCA File Offset: 0x00043DCA
		public override bool CanModify
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E3E RID: 3646
		// (get) Token: 0x06003D8F RID: 15759 RVA: 0x00069AC8 File Offset: 0x00067CC8
		// (set) Token: 0x06003D90 RID: 15760 RVA: 0x00182FF8 File Offset: 0x001811F8
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

		// Token: 0x17000E3F RID: 3647
		// (get) Token: 0x06003D91 RID: 15761 RVA: 0x00069AD0 File Offset: 0x00067CD0
		// (set) Token: 0x06003D92 RID: 15762 RVA: 0x00069AD8 File Offset: 0x00067CD8
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
				}
			}
		}

		// Token: 0x06003D93 RID: 15763 RVA: 0x001830A4 File Offset: 0x001812A4
		private void RefreshIcon()
		{
			this.m_upIndicator.enabled = !LocalPlayer.GameEntity.CharacterData.MainHand_SecondaryActive;
			this.m_downIndicator.enabled = LocalPlayer.GameEntity.CharacterData.MainHand_SecondaryActive;
			ArchetypeInstance mainHandInstance;
			IHandheldItem handheldItem_MainHand = LocalPlayer.GameEntity.GetHandheldItem_MainHand(out mainHandInstance);
			this.MainHandInstance = mainHandInstance;
			this.m_icon.overrideSprite = handheldItem_MainHand.Icon;
		}

		// Token: 0x06003D94 RID: 15764 RVA: 0x00183110 File Offset: 0x00181310
		private void RefreshChargeLabel()
		{
			this.m_chargeLabel.text = ((this.MainHandInstance != null && this.MainHandInstance.IsItem && this.MainHandInstance.ItemData.Charges != null) ? this.MainHandInstance.ItemData.Charges.Value.ToString() : "");
		}

		// Token: 0x06003D95 RID: 15765 RVA: 0x00069B0E File Offset: 0x00067D0E
		protected override void OnDestroy()
		{
			base.OnDestroy();
			LocalPlayer.GameEntity.CollectionController.Equipment.ContentsChanged -= this.EquipmentOnContentsChanged;
		}

		// Token: 0x06003D96 RID: 15766 RVA: 0x00183180 File Offset: 0x00181380
		protected void Update()
		{
			if (!ClientGameManager.InputManager.PreventInput && !ClientGameManager.InputManager.HoldingShift && SolInput.GetButtonDown(20))
			{
				this.ForceCombat();
				this.ToggleAutoAttack();
			}
			if (this.m_autoAttackEnabled && !base.Locked && !LocalPlayer.GameEntity.SkillsController.PendingIsActive && !LocalPlayer.GameEntity.SkillsController.AutoAttackPending)
			{
				DateTime.UtcNow >= this.m_nextAvailableAttack;
			}
		}

		// Token: 0x06003D97 RID: 15767 RVA: 0x00069B36 File Offset: 0x00067D36
		private void EquipmentOnContentsChanged()
		{
			this.RefreshIcon();
		}

		// Token: 0x06003D98 RID: 15768 RVA: 0x00069B3E File Offset: 0x00067D3E
		protected override void OnPointerUp(PointerEventData eventData)
		{
			this.ForceCombat();
			this.ToggleAutoAttack();
		}

		// Token: 0x06003D99 RID: 15769 RVA: 0x00069B4C File Offset: 0x00067D4C
		private void ToggleAutoAttack()
		{
			if (!this.Disabled)
			{
				this.m_autoAttackEnabled = !this.m_autoAttackEnabled;
				this.m_enabledBorder.enabled = this.m_autoAttackEnabled;
			}
		}

		// Token: 0x06003D9A RID: 15770 RVA: 0x00069B76 File Offset: 0x00067D76
		private void ForceCombat()
		{
			if (!this.m_autoAttackEnabled)
			{
				LocalPlayer.Animancer.ForceCombat(false);
			}
		}

		// Token: 0x06003D9B RID: 15771 RVA: 0x00069B8B File Offset: 0x00067D8B
		public void InitiateCombat()
		{
			if (!this.m_autoAttackEnabled)
			{
				this.ForceCombat();
				this.ToggleAutoAttack();
			}
		}

		// Token: 0x06003D9C RID: 15772 RVA: 0x00069BA1 File Offset: 0x00067DA1
		protected override void Awake()
		{
			base.Awake();
			this.m_chargeLabel.text = string.Empty;
		}

		// Token: 0x06003D9D RID: 15773 RVA: 0x00049FFA File Offset: 0x000481FA
		public override string FillActionsGetTitle()
		{
			return null;
		}

		// Token: 0x04003C2D RID: 15405
		private bool m_disabled = true;

		// Token: 0x04003C2E RID: 15406
		private bool m_autoAttackEnabled;

		// Token: 0x04003C2F RID: 15407
		[SerializeField]
		private Image m_enabledBorder;

		// Token: 0x04003C30 RID: 15408
		[SerializeField]
		private Image m_upIndicator;

		// Token: 0x04003C31 RID: 15409
		[SerializeField]
		private Image m_downIndicator;

		// Token: 0x04003C32 RID: 15410
		[SerializeField]
		private TextMeshProUGUI m_chargeLabel;

		// Token: 0x04003C33 RID: 15411
		private const float kMinSecondsBetweenAttempts = 1f;

		// Token: 0x04003C34 RID: 15412
		private DateTime m_nextAvailableAttack = DateTime.MinValue;

		// Token: 0x04003C35 RID: 15413
		private ArchetypeInstance m_mainHandInstance;
	}
}
