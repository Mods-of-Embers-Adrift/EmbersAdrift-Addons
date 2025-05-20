using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.UI;
using TMPro;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x02000852 RID: 2130
	public class ArmorClassPanel : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06003D80 RID: 15744 RVA: 0x00069A80 File Offset: 0x00067C80
		private void Awake()
		{
			this.m_armorBar.CenterText.text = "";
		}

		// Token: 0x06003D81 RID: 15745 RVA: 0x00182D08 File Offset: 0x00180F08
		public void UpdateData()
		{
			float armorClassPercent = LocalPlayer.GameEntity.Vitals.GetArmorClassPercent();
			int arg = Mathf.Clamp(Mathf.FloorToInt(armorClassPercent * 100f), 0, 100);
			int armorClass = LocalPlayer.GameEntity.Vitals.ArmorClass;
			int maxArmorClass = LocalPlayer.GameEntity.Vitals.MaxArmorClass;
			if (armorClass <= 0)
			{
				this.m_label.SetTextFormat("0 / {0}", maxArmorClass);
			}
			else
			{
				this.m_label.SetTextFormat("{0} / {1}", armorClass, maxArmorClass);
			}
			this.m_armorBar.SetFill(armorClassPercent, 0f);
			this.m_armorBar.CenterText.SetTextFormat("{0}%", arg);
		}

		// Token: 0x06003D82 RID: 15746 RVA: 0x00069A97 File Offset: 0x00067C97
		private ITooltipParameter GetTooltipParameter()
		{
			return new ObjectTextTooltipParameter(this, "Armor Class reduces the amount of incoming direct damage applied to your health. As your armor absorbs damage it loses durability making you more vulnerable. Armor can be repaired at anvils or with repair kits.", false);
		}

		// Token: 0x17000E3A RID: 3642
		// (get) Token: 0x06003D83 RID: 15747 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E3B RID: 3643
		// (get) Token: 0x06003D84 RID: 15748 RVA: 0x00069AAA File Offset: 0x00067CAA
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000E3C RID: 3644
		// (get) Token: 0x06003D85 RID: 15749 RVA: 0x00069AB8 File Offset: 0x00067CB8
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06003D87 RID: 15751 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003C25 RID: 15397
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003C26 RID: 15398
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x04003C27 RID: 15399
		[SerializeField]
		private NameplateControllerStatBarUI m_armorBar;

		// Token: 0x04003C28 RID: 15400
		private const string kArmorDescription = "Armor Class reduces the amount of incoming direct damage applied to your health. As your armor absorbs damage it loses durability making you more vulnerable. Armor can be repaired at anvils or with repair kits.";
	}
}
