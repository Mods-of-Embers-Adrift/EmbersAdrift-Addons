using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x02000851 RID: 2129
	public class ArmorBudgetPanel : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x06003D79 RID: 15737 RVA: 0x00182BE4 File Offset: 0x00180DE4
		public void UpdateData()
		{
			if (LocalPlayer.GameEntity == null || LocalPlayer.GameEntity.Vitals == null)
			{
				return;
			}
			int armorWeightCapacity = LocalPlayer.GameEntity.Vitals.ArmorWeightCapacity;
			int armorCost = LocalPlayer.GameEntity.Vitals.ArmorCost;
			string text = armorCost.ToString();
			string arg = string.Empty;
			if (armorCost > armorWeightCapacity)
			{
				text = text.Color(Colors.Crimson);
				arg = " (" + LocalPlayer.GameEntity.Vitals.ArmorCostModifier.ToString().Color(Colors.Crimson) + ")";
			}
			this.m_label.SetTextFormat("{0} / {1}{2}", text, armorWeightCapacity, arg);
		}

		// Token: 0x06003D7A RID: 15738 RVA: 0x00182C94 File Offset: 0x00180E94
		private ITooltipParameter GetTooltipParameter()
		{
			if (LocalPlayer.GameEntity == null || LocalPlayer.GameEntity.Vitals == null)
			{
				return null;
			}
			int armorWeightCapacity = LocalPlayer.GameEntity.Vitals.ArmorWeightCapacity;
			int armorCost = LocalPlayer.GameEntity.Vitals.ArmorCost;
			string txt = string.Empty;
			if (armorCost <= armorWeightCapacity)
			{
				txt = "You are under your armor weight capacity. If you exceed it you will suffer stat penalties.";
			}
			else
			{
				txt = string.Concat(new string[]
				{
					"You are ",
					Mathf.FloorToInt(Mathf.Clamp01((float)(armorCost - armorWeightCapacity) / (float)armorWeightCapacity) * 100f).ToString(),
					"% over your armor weight budget! This is imposing a ",
					LocalPlayer.GameEntity.Vitals.ArmorCostModifier.ToString(),
					" stat penalty."
				});
			}
			return new ObjectTextTooltipParameter(this, txt, false);
		}

		// Token: 0x17000E37 RID: 3639
		// (get) Token: 0x06003D7B RID: 15739 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E38 RID: 3640
		// (get) Token: 0x06003D7C RID: 15740 RVA: 0x00069A6A File Offset: 0x00067C6A
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000E39 RID: 3641
		// (get) Token: 0x06003D7D RID: 15741 RVA: 0x00069A78 File Offset: 0x00067C78
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06003D7F RID: 15743 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04003C23 RID: 15395
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x04003C24 RID: 15396
		[SerializeField]
		private TextMeshProUGUI m_label;
	}
}
