using System;
using System.Globalization;
using Cysharp.Text;
using TMPro;
using UnityEngine;

namespace SoL.Utilities.Extensions
{
	// Token: 0x02000334 RID: 820
	public static class NumberExtensions
	{
		// Token: 0x06001666 RID: 5734 RVA: 0x00051A32 File Offset: 0x0004FC32
		public static float PercentModification(this float value, float percentage)
		{
			return value + value * percentage;
		}

		// Token: 0x06001667 RID: 5735 RVA: 0x00051A39 File Offset: 0x0004FC39
		public static float PercentModification(this int value, float percentage)
		{
			return (float)value + (float)value * percentage;
		}

		// Token: 0x06001668 RID: 5736 RVA: 0x00051A42 File Offset: 0x0004FC42
		public static int ToIntTowardsZero(float value)
		{
			if (value < 0f)
			{
				return Mathf.CeilToInt(value);
			}
			return Mathf.FloorToInt(value);
		}

		// Token: 0x06001669 RID: 5737 RVA: 0x00051A59 File Offset: 0x0004FC59
		public static int ToIntAwayFromZero(float value)
		{
			if (value < 0f)
			{
				return Mathf.FloorToInt(value);
			}
			return Mathf.CeilToInt(value);
		}

		// Token: 0x0600166A RID: 5738 RVA: 0x00100774 File Offset: 0x000FE974
		public static int ToDisplayInt(float value)
		{
			float num = Mathf.Abs(value);
			int num2 = Mathf.FloorToInt(num);
			int num3 = (num - (float)num2 >= 0.5f) ? Mathf.CeilToInt(num) : num2;
			int num4 = (value >= 0f) ? 1 : -1;
			return num3 * num4;
		}

		// Token: 0x0600166B RID: 5739 RVA: 0x001007B4 File Offset: 0x000FE9B4
		public static float ConvertToDecibel(this float value)
		{
			if (value <= 0f)
			{
				return -80f;
			}
			float f = Mathf.Pow(value, 2f);
			return 20f * Mathf.Log10(f);
		}

		// Token: 0x0600166C RID: 5740 RVA: 0x00051A70 File Offset: 0x0004FC70
		public static float ConvertFromDecibel(this float value)
		{
			if (value <= -80f)
			{
				return 0f;
			}
			return Mathf.Pow(Mathf.Pow(10f, value / 20f), 0.5f);
		}

		// Token: 0x0600166D RID: 5741 RVA: 0x00051A9B File Offset: 0x0004FC9B
		public static float Remap(this float value, float from1, float to1, float from2, float to2)
		{
			return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
		}

		// Token: 0x0600166E RID: 5742 RVA: 0x00051AAB File Offset: 0x0004FCAB
		public static int Remap(this int value, int from1, int to1, int from2, int to2)
		{
			return NumberExtensions.ToDisplayInt((float)(value - from1) / (float)(to1 - from1) * (float)(to2 - from2) + (float)from2);
		}

		// Token: 0x0600166F RID: 5743 RVA: 0x00051AC4 File Offset: 0x0004FCC4
		public static string FormattedSingleDecimalPlace(this float value)
		{
			return value.ToString("0.#", CultureInfo.InvariantCulture) ?? "";
		}

		// Token: 0x06001670 RID: 5744 RVA: 0x00051AE0 File Offset: 0x0004FCE0
		private static string FormattedSingleDecimalPlace(this double value)
		{
			return ((float)value).FormattedSingleDecimalPlace();
		}

		// Token: 0x06001671 RID: 5745 RVA: 0x00051AE9 File Offset: 0x0004FCE9
		private static string FormattedTwoDecimalPlaces(this float value)
		{
			return value.ToString("0.##", CultureInfo.InvariantCulture) ?? "";
		}

		// Token: 0x06001672 RID: 5746 RVA: 0x00051B05 File Offset: 0x0004FD05
		private static string FormattedTwoDecimalPlaces(this double value)
		{
			return ((float)value).FormattedTwoDecimalPlaces();
		}

		// Token: 0x06001673 RID: 5747 RVA: 0x00051B0E File Offset: 0x0004FD0E
		public static string GetAngleDisplay(this float value)
		{
			if (value == 0f)
			{
				return string.Empty;
			}
			return ZString.Format<int, string>("<b>{0}{1}</b> Angle", Mathf.FloorToInt(value), "°");
		}

		// Token: 0x06001674 RID: 5748 RVA: 0x00051B33 File Offset: 0x0004FD33
		public static string FormattedSeconds(this float value)
		{
			return value.FormattedSingleDecimalPlace();
		}

		// Token: 0x06001675 RID: 5749 RVA: 0x00051B3B File Offset: 0x0004FD3B
		public static string FormattedSeconds(this double value)
		{
			return value.FormattedSingleDecimalPlace();
		}

		// Token: 0x06001676 RID: 5750 RVA: 0x00051B33 File Offset: 0x0004FD33
		public static string FormattedArmorClass(this float value)
		{
			return value.FormattedSingleDecimalPlace();
		}

		// Token: 0x06001677 RID: 5751 RVA: 0x001007E8 File Offset: 0x000FE9E8
		public static string GetAsPercentage(this float value)
		{
			if (value >= 0f)
			{
				return Mathf.FloorToInt(value * 100f).ToString();
			}
			return Mathf.CeilToInt(value * 100f).ToString();
		}

		// Token: 0x06001678 RID: 5752 RVA: 0x00051B43 File Offset: 0x0004FD43
		public static string GetAsPercentage(this double value)
		{
			return ((float)value).GetAsPercentage();
		}

		// Token: 0x06001679 RID: 5753 RVA: 0x00051B4C File Offset: 0x0004FD4C
		public static string FormattedTime(this float value, int places = 1)
		{
			return ((double)value).FormattedTime(places);
		}

		// Token: 0x0600167A RID: 5754 RVA: 0x00100828 File Offset: 0x000FEA28
		private static string FormattedTime(this double value, int places = 1)
		{
			string str = "s";
			if (value >= 86400.0)
			{
				value /= 86400.0;
				str = "days";
			}
			else if (value >= 3600.0)
			{
				value /= 3600.0;
				str = "hr";
			}
			else if (value > 60.0)
			{
				value /= 60.0;
				str = "m";
			}
			if (places == 0)
			{
				return value.ToString("0", CultureInfo.InvariantCulture) + str;
			}
			return value.ToString("0.#", CultureInfo.InvariantCulture) + str;
		}

		// Token: 0x0600167B RID: 5755 RVA: 0x001008D0 File Offset: 0x000FEAD0
		public static string AsCurrency(this ulong value)
		{
			CurrencyConverter currencyConverter = new CurrencyConverter(value);
			return currencyConverter.ToString();
		}

		// Token: 0x0600167C RID: 5756 RVA: 0x00051B56 File Offset: 0x0004FD56
		public static string AsCurrency(this uint value)
		{
			return ((ulong)value).AsCurrency();
		}

		// Token: 0x0600167D RID: 5757 RVA: 0x00051B5F File Offset: 0x0004FD5F
		public static void SetFormattedTime(this TMP_Text tmp, int value, bool fullTime = false)
		{
			tmp.SetFormattedTime((float)value, fullTime);
		}

		// Token: 0x0600167E RID: 5758 RVA: 0x00051B5F File Offset: 0x0004FD5F
		public static void SetFormattedTime(this TMP_Text tmp, double value, bool fullTime = false)
		{
			tmp.SetFormattedTime((float)value, fullTime);
		}

		// Token: 0x0600167F RID: 5759 RVA: 0x001008F4 File Offset: 0x000FEAF4
		public static void SetFormattedTime(this TMP_Text tmp, float value, bool fullTime = false)
		{
			float num = value / 86400f;
			int num2 = Mathf.FloorToInt(num);
			float num3 = value / 3600f;
			int num4 = Mathf.FloorToInt(num3);
			float num5 = value / 60f;
			int num6 = Mathf.FloorToInt(num5);
			int num7 = Mathf.CeilToInt(value);
			if (num2 > 0)
			{
				if (fullTime)
				{
					num3 = (num - (float)num2) * 24f;
					num4 = Mathf.FloorToInt(num3);
					num5 = (num3 - (float)num4) * 60f;
					num6 = Mathf.FloorToInt(num5);
					float f = (num5 - (float)num6) * 60f;
					num7 = Mathf.FloorToInt(f);
					tmp.SetTextFormat("{0}d {1}hr {2}m {3}s", num2, num4, num6, num7);
					return;
				}
				tmp.SetTextFormat("{0}d", num2 + 1);
				return;
			}
			else if (num4 > 0)
			{
				if (fullTime)
				{
					num5 = (num3 - (float)num4) * 60f;
					num6 = Mathf.FloorToInt(num5);
					float f = (num5 - (float)num6) * 60f;
					num7 = Mathf.FloorToInt(f);
					tmp.SetTextFormat("{0}hr {1}m {2}s", num4, num6, num7);
					return;
				}
				tmp.SetTextFormat("{0}hr", num4 + 1);
				return;
			}
			else if (num6 > 0)
			{
				if (fullTime)
				{
					float f = (num5 - (float)num6) * 60f;
					num7 = Mathf.FloorToInt(f);
					tmp.SetTextFormat("{0}m {1}s", num6, num7);
					return;
				}
				tmp.SetTextFormat("{0}m", num6 + 1);
				return;
			}
			else
			{
				if (num7 <= 0 && value > 0f)
				{
					tmp.SetTextFormat("{0:0.#}s", value);
					return;
				}
				tmp.SetTextFormat("{0}s", num7);
				return;
			}
		}

		// Token: 0x06001680 RID: 5760 RVA: 0x00051B6A File Offset: 0x0004FD6A
		public static string GetFormattedTime(this int value, bool fullTime = false)
		{
			return ((float)value).GetFormattedTime(fullTime);
		}

		// Token: 0x06001681 RID: 5761 RVA: 0x00051B6A File Offset: 0x0004FD6A
		public static string GetFormattedTime(this double value, bool fullTime = false)
		{
			return ((float)value).GetFormattedTime(fullTime);
		}

		// Token: 0x06001682 RID: 5762 RVA: 0x00100A60 File Offset: 0x000FEC60
		public static string GetFormattedTime(this float value, bool fullTime = false)
		{
			float num = value / 86400f;
			int num2 = Mathf.FloorToInt(num);
			float num3 = value / 3600f;
			int num4 = Mathf.FloorToInt(num3);
			float num5 = value / 60f;
			int num6 = Mathf.FloorToInt(num5);
			int num7 = Mathf.FloorToInt(value);
			if (num2 > 0)
			{
				if (fullTime)
				{
					num3 = (num - (float)num2) * 24f;
					num4 = Mathf.FloorToInt(num3);
					num5 = (num3 - (float)num4) * 60f;
					num6 = Mathf.FloorToInt(num5);
					float f = (num5 - (float)num6) * 60f;
					num7 = Mathf.FloorToInt(f);
					return ZString.Format<int, int, int, int>("{0}d {1}hr {2}m {3}s", num2, num4, num6, num7);
				}
				return ZString.Format<int>("{0}d", num2);
			}
			else if (num4 > 0)
			{
				if (fullTime)
				{
					num5 = (num3 - (float)num4) * 60f;
					num6 = Mathf.FloorToInt(num5);
					float f = (num5 - (float)num6) * 60f;
					num7 = Mathf.FloorToInt(f);
					return ZString.Format<int, int, int>("{0}hr {1}m {2}s", num4, num6, num7);
				}
				return ZString.Format<int>("{0}hr", num4);
			}
			else if (num6 > 0)
			{
				if (!fullTime)
				{
					return ZString.Format<int>("{0}m", num6);
				}
				float f = (num5 - (float)num6) * 60f;
				num7 = Mathf.FloorToInt(f);
				if (num7 == 0)
				{
					return ZString.Format<int>("{0}m", num6);
				}
				return ZString.Format<int, int>("{0}m {1}s", num6, num7);
			}
			else
			{
				if (num7 > 0 || value <= 0f)
				{
					return ZString.Format<int>("{0}s", num7);
				}
				return ZString.Format<float>("{0:0.#}s", value);
			}
		}

		// Token: 0x06001683 RID: 5763 RVA: 0x00051B74 File Offset: 0x0004FD74
		public static bool OppositeSigns(int a, int b)
		{
			return (a ^ b) < 0;
		}

		// Token: 0x06001684 RID: 5764 RVA: 0x00051B7C File Offset: 0x0004FD7C
		public static bool SameSigns(int a, int b)
		{
			return !NumberExtensions.OppositeSigns(a, b);
		}

		// Token: 0x06001685 RID: 5765 RVA: 0x00051B88 File Offset: 0x0004FD88
		public static bool OppositeSigns(float a, float b)
		{
			return !NumberExtensions.SameSigns(a, b);
		}

		// Token: 0x06001686 RID: 5766 RVA: 0x00051B94 File Offset: 0x0004FD94
		public static bool SameSigns(float a, float b)
		{
			return a * b >= 0f;
		}

		// Token: 0x06001687 RID: 5767 RVA: 0x00051BA3 File Offset: 0x0004FDA3
		public static bool TryGetDamageResist(int physicalResist, out int result)
		{
			result = 0;
			if (physicalResist != 0)
			{
				result = physicalResist;
			}
			return result != 0;
		}

		// Token: 0x06001688 RID: 5768 RVA: 0x00100BD0 File Offset: 0x000FEDD0
		public static bool TryGetDamageResistEmber(int chemicalResist, int mentalResist, int emberResist, out int result)
		{
			result = 0;
			if (chemicalResist != 0 || mentalResist != 0 || emberResist != 0)
			{
				float floatResult = ((float)chemicalResist * 15f + (float)mentalResist * 10f + (float)emberResist * 20f) / 30f;
				result = NumberExtensions.ClampedFloatResult(floatResult);
			}
			return result != 0;
		}

		// Token: 0x06001689 RID: 5769 RVA: 0x00051BA3 File Offset: 0x0004FDA3
		public static bool TryGetDamageReduction(int physicalResist, out int result)
		{
			result = 0;
			if (physicalResist != 0)
			{
				result = physicalResist;
			}
			return result != 0;
		}

		// Token: 0x0600168A RID: 5770 RVA: 0x00100C18 File Offset: 0x000FEE18
		public static bool TryGetDamageReductionEmber(int mentalResist, int chemicalResist, int emberResist, out int result)
		{
			result = 0;
			if (mentalResist != 0 || chemicalResist != 0 || emberResist != 0)
			{
				float floatResult = ((float)chemicalResist * 15f + (float)mentalResist * 10f + (float)emberResist * 20f) / 30f;
				result = NumberExtensions.ClampedFloatResult(floatResult);
			}
			return result != 0;
		}

		// Token: 0x0600168B RID: 5771 RVA: 0x00100C60 File Offset: 0x000FEE60
		public static bool TryGetDebuffResist(int physicalDebuffResist, int chemicalDebuffResist, out int result)
		{
			result = 0;
			if (physicalDebuffResist != 0 || chemicalDebuffResist != 0)
			{
				float floatResult = ((float)chemicalDebuffResist * 8f + (float)physicalDebuffResist * 8f) / 20f;
				result = NumberExtensions.ClampedFloatResult(floatResult);
			}
			return result != 0;
		}

		// Token: 0x0600168C RID: 5772 RVA: 0x00100C9C File Offset: 0x000FEE9C
		public static bool TryGetDebuffResistEmber(int mentalDebuffResist, int emberDebuffResist, out int result)
		{
			result = 0;
			if (mentalDebuffResist != 0 || emberDebuffResist != 0)
			{
				float floatResult = ((float)mentalDebuffResist * 10f + (float)emberDebuffResist * 10f) / 20f;
				result = NumberExtensions.ClampedFloatResult(floatResult);
			}
			return result != 0;
		}

		// Token: 0x0600168D RID: 5773 RVA: 0x00051BB3 File Offset: 0x0004FDB3
		private static int ClampedFloatResult(float floatResult)
		{
			if (floatResult >= 0f)
			{
				return Mathf.Clamp(Mathf.FloorToInt(floatResult), 1, int.MaxValue);
			}
			return Mathf.Clamp(Mathf.CeilToInt(floatResult), int.MinValue, -1);
		}

		// Token: 0x04001E58 RID: 7768
		private const float kDecibelPower = 2f;

		// Token: 0x04001E59 RID: 7769
		private const float kDecibelScalar = 20f;
	}
}
