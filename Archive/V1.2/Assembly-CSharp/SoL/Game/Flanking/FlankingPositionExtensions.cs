using System;
using Cysharp.Text;

namespace SoL.Game.Flanking
{
	// Token: 0x02000BF5 RID: 3061
	public static class FlankingPositionExtensions
	{
		// Token: 0x06005E6F RID: 24175 RVA: 0x0007F7DD File Offset: 0x0007D9DD
		public static FlankingPosition GetFlankingPosition(float angle)
		{
			if (angle <= 45f)
			{
				return FlankingPosition.Front;
			}
			if (angle <= 135f)
			{
				return FlankingPosition.Sides;
			}
			return FlankingPosition.Rear;
		}

		// Token: 0x06005E70 RID: 24176 RVA: 0x0007F7F4 File Offset: 0x0007D9F4
		public static bool AllowActiveDefenses(this FlankingPosition flank)
		{
			return flank == FlankingPosition.Front;
		}

		// Token: 0x06005E71 RID: 24177 RVA: 0x0007F7FC File Offset: 0x0007D9FC
		public static string GetArrowString(this FlankingPosition position)
		{
			switch (position)
			{
			case FlankingPosition.Front:
				return "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";
			case FlankingPosition.Sides:
				return "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";
			case FlankingPosition.Rear:
				return "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>";
			default:
				return string.Empty;
			}
		}

		// Token: 0x06005E72 RID: 24178 RVA: 0x0007F829 File Offset: 0x0007DA29
		public static string GetPositionDescription(this FlankingPosition position)
		{
			switch (position)
			{
			case FlankingPosition.Front:
				return "Front Position";
			case FlankingPosition.Sides:
				return "Side Position";
			case FlankingPosition.Rear:
				return "Rear Position";
			default:
				return string.Empty;
			}
		}

		// Token: 0x06005E73 RID: 24179 RVA: 0x001F69E8 File Offset: 0x001F4BE8
		public static string GetRotatedArrow(this FlankingPosition position, float value, float min, float max)
		{
			string text = string.Empty;
			if (min != 0f || max != 0f)
			{
				if (value >= max)
				{
					text = "<sprite=\"SolIcons\" name=\"ArrowFull\" tint=1>";
				}
				else if (value <= min)
				{
					text = "<sprite=\"SolIcons\" name=\"ArrowEmpty\" tint=1>";
				}
				else
				{
					text = "<sprite=\"SolIcons\" name=\"ArrowHalf\" tint=1>";
				}
				if (position != FlankingPosition.Sides)
				{
					if (position == FlankingPosition.Rear)
					{
						text = ZString.Format<float, string>("<rotate=\"{0}\">{1}</rotate>", 180f, text);
					}
				}
				else
				{
					text = ZString.Format<float, string>("<rotate=\"{0}\">{1}</rotate>", 90f, text);
				}
			}
			return text;
		}

		// Token: 0x040051AE RID: 20910
		public const float kFrontAngle = 0f;

		// Token: 0x040051AF RID: 20911
		public const float kSideAngle = 90f;

		// Token: 0x040051B0 RID: 20912
		public const float kRearAngle = 180f;

		// Token: 0x040051B1 RID: 20913
		public static readonly float[] BonusArray = new float[3];
	}
}
