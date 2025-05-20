using System;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C7F RID: 3199
	[Serializable]
	public class VitalScalingValue
	{
		// Token: 0x17001757 RID: 5975
		// (get) Token: 0x06006178 RID: 24952 RVA: 0x00081B73 File Offset: 0x0007FD73
		public VitalType Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x17001758 RID: 5976
		// (get) Token: 0x06006179 RID: 24953 RVA: 0x001FFF58 File Offset: 0x001FE158
		public string IndexName
		{
			get
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
					this.m_type.GetDisplayName(),
					": ",
					minMaxAnimationCurveValues.ValueMin.ToString(),
					" to ",
					minMaxAnimationCurveValues.ValueMax.ToString()
				});
			}
		}

		// Token: 0x0600617A RID: 24954 RVA: 0x00081B7B File Offset: 0x0007FD7B
		public int GetValueForLevel(float level)
		{
			return Mathf.FloorToInt(this.m_valueCurve.Evaluate(level));
		}

		// Token: 0x0600617B RID: 24955 RVA: 0x001FFFFC File Offset: 0x001FE1FC
		public void AddToTooltipBlock(TooltipTextBlock block, int? level)
		{
			MinMaxAnimationCurveValues minMaxAnimationCurveValues;
			if (this.m_type == VitalType.None || !this.m_valueCurve.TryGetMinMaxValues(out minMaxAnimationCurveValues))
			{
				return;
			}
			if (level != null)
			{
				int valueForLevel = this.GetValueForLevel((float)level.Value);
				string text = (valueForLevel > 0) ? "+" : "";
				block.AppendLine(string.Concat(new string[]
				{
					text,
					valueForLevel.ToString(),
					" ",
					this.m_type.GetDisplayName(),
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
				this.m_type.GetDisplayName()
			}), 0);
		}

		// Token: 0x040054F6 RID: 21750
		[SerializeField]
		private VitalType m_type;

		// Token: 0x040054F7 RID: 21751
		[SerializeField]
		private AnimationCurve m_valueCurve = AnimationCurve.Linear(1f, 0f, 50f, 0f);
	}
}
