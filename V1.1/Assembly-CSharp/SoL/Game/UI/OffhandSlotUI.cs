using System;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x020008BA RID: 2234
	public class OffhandSlotUI : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x17000EF2 RID: 3826
		// (get) Token: 0x06004170 RID: 16752 RVA: 0x0006C32A File Offset: 0x0006A52A
		// (set) Token: 0x06004171 RID: 16753 RVA: 0x0018F4AC File Offset: 0x0018D6AC
		private ArchetypeInstance Instance
		{
			get
			{
				return this.m_instance;
			}
			set
			{
				if (this.m_instance == value)
				{
					return;
				}
				if (this.m_instance != null)
				{
					this.m_instance.ItemData.CountChanged -= this.RefreshCountChargeLabel;
					this.m_instance.ItemData.ChargesChanged -= this.RefreshCountChargeLabel;
				}
				this.m_instance = value;
				if (this.m_instance != null)
				{
					this.m_instance.ItemData.CountChanged += this.RefreshCountChargeLabel;
					this.m_instance.ItemData.ChargesChanged += this.RefreshCountChargeLabel;
				}
				this.RefreshCountChargeLabel();
			}
		}

		// Token: 0x06004172 RID: 16754 RVA: 0x0006C332 File Offset: 0x0006A532
		private void Awake()
		{
			LocalPlayer.NetworkEntity.OnStartLocalClient += this.InitInternal;
			this.m_countChargeLabel.text = "";
		}

		// Token: 0x06004173 RID: 16755 RVA: 0x0018F550 File Offset: 0x0018D750
		private void OnDestroy()
		{
			if (LocalPlayer.GameEntity)
			{
				if (LocalPlayer.GameEntity.CharacterData)
				{
					LocalPlayer.GameEntity.CharacterData.HandConfigurationChanged -= this.OnSettingsChanged;
				}
				if (LocalPlayer.GameEntity.CollectionController != null)
				{
					LocalPlayer.GameEntity.CollectionController.Equipment.ContentsChanged -= this.EquipmentOnContentsChanged;
				}
				if (LocalPlayer.GameEntity.VitalsReplicator)
				{
					LocalPlayer.GameEntity.VitalsReplicator.CurrentStance.Changed -= this.CurrentStanceOnChanged;
				}
			}
		}

		// Token: 0x06004174 RID: 16756 RVA: 0x0018F5F8 File Offset: 0x0018D7F8
		private void InitInternal()
		{
			this.RefreshIcon();
			LocalPlayer.NetworkEntity.OnStartLocalClient -= this.InitInternal;
			LocalPlayer.GameEntity.CharacterData.HandConfigurationChanged += this.OnSettingsChanged;
			LocalPlayer.GameEntity.VitalsReplicator.CurrentStance.Changed += this.CurrentStanceOnChanged;
			LocalPlayer.GameEntity.CollectionController.Equipment.ContentsChanged += this.EquipmentOnContentsChanged;
		}

		// Token: 0x06004175 RID: 16757 RVA: 0x0006C35A File Offset: 0x0006A55A
		private void EquipmentOnContentsChanged()
		{
			this.RefreshIcon();
		}

		// Token: 0x06004176 RID: 16758 RVA: 0x0006C35A File Offset: 0x0006A55A
		private void OnSettingsChanged()
		{
			this.RefreshIcon();
		}

		// Token: 0x06004177 RID: 16759 RVA: 0x0006C35A File Offset: 0x0006A55A
		private void CurrentStanceOnChanged(Stance obj)
		{
			this.RefreshIcon();
		}

		// Token: 0x06004178 RID: 16760 RVA: 0x0018F67C File Offset: 0x0018D87C
		private void RefreshIcon()
		{
			ArchetypeInstance archetypeInstance;
			LocalPlayer.GameEntity.GetHandheldItem_OffHand(out archetypeInstance);
			if (archetypeInstance == null)
			{
				ArchetypeInstance archetypeInstance2;
				IHandheldItem handheldItem_MainHand = LocalPlayer.GameEntity.GetHandheldItem_MainHand(out archetypeInstance2);
				if (handheldItem_MainHand != null && handheldItem_MainHand.RequiresFreeOffHand)
				{
					archetypeInstance = archetypeInstance2;
				}
			}
			this.Instance = archetypeInstance;
			this.m_image.overrideSprite = ((this.Instance == null) ? GlobalSettings.Values.Combat.FallbackWeapon.Icon : this.Instance.Archetype.Icon);
			this.m_upIndicator.enabled = !LocalPlayer.GameEntity.CharacterData.OffHand_SecondaryActive;
			this.m_downIndicator.enabled = LocalPlayer.GameEntity.CharacterData.OffHand_SecondaryActive;
		}

		// Token: 0x06004179 RID: 16761 RVA: 0x0018F72C File Offset: 0x0018D92C
		private void RefreshCountChargeLabel()
		{
			if (this.Instance == null)
			{
				this.m_countChargeLabel.text = "";
				return;
			}
			if (this.Instance.Archetype.ArchetypeHasCount() && this.Instance.ItemData.Count != null)
			{
				this.m_countChargeLabel.text = this.Instance.ItemData.Count.Value.ToString();
				return;
			}
			if (this.Instance.Archetype.ArchetypeHasCharges() && this.Instance.ItemData.Charges != null)
			{
				this.m_countChargeLabel.text = this.Instance.ItemData.Charges.Value.ToString();
				return;
			}
			this.m_countChargeLabel.text = "";
		}

		// Token: 0x0600417A RID: 16762 RVA: 0x0018F814 File Offset: 0x0018DA14
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.Instance == null)
			{
				return null;
			}
			return new ArchetypeTooltipParameter
			{
				Instance = this.Instance
			};
		}

		// Token: 0x17000EF3 RID: 3827
		// (get) Token: 0x0600417B RID: 16763 RVA: 0x0006C362 File Offset: 0x0006A562
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000EF4 RID: 3828
		// (get) Token: 0x0600417C RID: 16764 RVA: 0x0006C370 File Offset: 0x0006A570
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17000EF5 RID: 3829
		// (get) Token: 0x0600417D RID: 16765 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600417F RID: 16767 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003EC4 RID: 16068
		[SerializeField]
		private Image m_image;

		// Token: 0x04003EC5 RID: 16069
		[SerializeField]
		private Image m_upIndicator;

		// Token: 0x04003EC6 RID: 16070
		[SerializeField]
		private Image m_downIndicator;

		// Token: 0x04003EC7 RID: 16071
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003EC8 RID: 16072
		[SerializeField]
		private TextMeshProUGUI m_countChargeLabel;

		// Token: 0x04003EC9 RID: 16073
		private ArchetypeInstance m_instance;
	}
}
