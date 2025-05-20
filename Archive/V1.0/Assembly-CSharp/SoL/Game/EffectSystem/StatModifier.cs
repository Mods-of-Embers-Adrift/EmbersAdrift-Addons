using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C2E RID: 3118
	[Serializable]
	public class StatModifier : StatModifierBase
	{
		// Token: 0x17001715 RID: 5909
		// (get) Token: 0x06006029 RID: 24617 RVA: 0x00080B61 File Offset: 0x0007ED61
		public int Value
		{
			get
			{
				return this.m_value;
			}
		}

		// Token: 0x0600602A RID: 24618 RVA: 0x00080B69 File Offset: 0x0007ED69
		public StatModifier()
		{
		}

		// Token: 0x0600602B RID: 24619 RVA: 0x00080B71 File Offset: 0x0007ED71
		public StatModifier(StatType statType, int value) : base(statType)
		{
			this.m_value = value;
		}

		// Token: 0x0600602C RID: 24620 RVA: 0x00080B81 File Offset: 0x0007ED81
		public StatModifier(StatusEffectType type, StatusEffectSubType subType, bool specifyDamageType, DamageType damageType, int value) : base(type, subType, specifyDamageType, damageType)
		{
			this.m_value = value;
		}

		// Token: 0x0600602D RID: 24621 RVA: 0x00080B96 File Offset: 0x0007ED96
		public void AddValue(Dictionary<StatType, int> effects)
		{
			this.ModifyValue(effects, true);
		}

		// Token: 0x0600602E RID: 24622 RVA: 0x00080BA0 File Offset: 0x0007EDA0
		public void RemoveValue(Dictionary<StatType, int> effects)
		{
			this.ModifyValue(effects, false);
		}

		// Token: 0x0600602F RID: 24623 RVA: 0x00080BAA File Offset: 0x0007EDAA
		public void ModifyValue(Dictionary<StatType, int> effects, bool adding)
		{
			base.ModifyValueInternal(effects, this.m_value, adding);
		}

		// Token: 0x06006030 RID: 24624 RVA: 0x001FC070 File Offset: 0x001FA270
		public void AddToTooltipBlock(TooltipTextBlock block, Color? colorOverride = null)
		{
			if (this.m_value != 0)
			{
				string arg = (this.m_value > 0) ? "+" : "";
				Color color = (this.m_value > 0) ? UIManager.BlueColor : UIManager.RedColor;
				if (colorOverride != null)
				{
					color = colorOverride.Value;
				}
				block.AppendLine(ZString.Format<string, string, int, string>("<color={0}>{1}{2}</color> {3}", color.ToHex(), arg, this.m_value, base.GetTypeString()), 0);
			}
		}

		// Token: 0x06006031 RID: 24625 RVA: 0x00080BBA File Offset: 0x0007EDBA
		protected override string GetValueString()
		{
			return this.m_value.ToString();
		}

		// Token: 0x040052E8 RID: 21224
		[SerializeField]
		private int m_value;
	}
}
