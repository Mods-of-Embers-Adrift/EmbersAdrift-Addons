using System;
using System.Collections.Generic;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C31 RID: 3121
	[Serializable]
	public class StatModifierScaling : StatModifierBase
	{
		// Token: 0x06006043 RID: 24643 RVA: 0x00080C91 File Offset: 0x0007EE91
		public StatModifierScaling(StatType statType) : base(statType)
		{
		}

		// Token: 0x06006044 RID: 24644 RVA: 0x001FC250 File Offset: 0x001FA450
		public void ModifyValue(Dictionary<StatType, int> effects, bool adding, int level)
		{
			int value = this.GetValue((float)level);
			base.ModifyValueInternal(effects, value, adding);
		}

		// Token: 0x06006045 RID: 24645 RVA: 0x00080CB9 File Offset: 0x0007EEB9
		private int GetValue(float level)
		{
			return Mathf.FloorToInt(this.m_valueCurve.Evaluate(level));
		}

		// Token: 0x06006046 RID: 24646 RVA: 0x001FC270 File Offset: 0x001FA470
		public void AddToTooltipBlock(TooltipTextBlock block, int? level)
		{
			MinMaxAnimationCurveValues minMaxAnimationCurveValues;
			if (!this.m_valueCurve.TryGetMinMaxValues(out minMaxAnimationCurveValues))
			{
				return;
			}
			if (level != null)
			{
				int value = this.GetValue((float)level.Value);
				string text = (value > 0) ? "+" : "";
				block.AppendLine(string.Concat(new string[]
				{
					text,
					value.ToString(),
					" ",
					base.GetTypeString(),
					" <size=80%>(",
					minMaxAnimationCurveValues.ValueMin.ToString(),
					" to ",
					minMaxAnimationCurveValues.ValueMax.ToString(),
					")</size>"
				}), 0);
				return;
			}
			string text2 = (minMaxAnimationCurveValues.ValueMax > 0) ? "+" : "";
			block.AppendLine(string.Concat(new string[]
			{
				text2,
				minMaxAnimationCurveValues.ValueMin.ToString(),
				" to ",
				text2,
				minMaxAnimationCurveValues.ValueMax.ToString(),
				" ",
				base.GetTypeString()
			}), 0);
		}

		// Token: 0x06006047 RID: 24647 RVA: 0x001FC38C File Offset: 0x001FA58C
		protected override string GetValueString()
		{
			MinMaxAnimationCurveValues minMaxAnimationCurveValues;
			if (!this.m_valueCurve.TryGetMinMaxValues(out minMaxAnimationCurveValues))
			{
				return "Invalid Curve!";
			}
			return string.Concat(new string[]
			{
				"[lvl ",
				minMaxAnimationCurveValues.LevelMin.ToString(),
				"-",
				minMaxAnimationCurveValues.LevelMax.ToString(),
				"] ",
				minMaxAnimationCurveValues.ValueMin.ToString(),
				" to ",
				minMaxAnimationCurveValues.ValueMax.ToString()
			});
		}

		// Token: 0x040052EE RID: 21230
		[SerializeField]
		private AnimationCurve m_valueCurve = AnimationCurve.Linear(1f, 0f, 50f, 0f);
	}
}
