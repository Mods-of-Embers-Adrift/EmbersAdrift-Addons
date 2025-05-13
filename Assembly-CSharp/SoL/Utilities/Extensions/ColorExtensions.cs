using System;
using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;

namespace SoL.Utilities.Extensions
{
	// Token: 0x0200032C RID: 812
	public static class ColorExtensions
	{
		// Token: 0x06001652 RID: 5714 RVA: 0x00100310 File Offset: 0x000FE510
		public static string ToHex(this Color color)
		{
			string text;
			if (!ColorExtensions.m_colorToHexDict.TryGetValue(color, out text))
			{
				string arg = ColorUtility.ToHtmlStringRGB(color);
				text = ZString.Format<string>("#{0}", arg);
				ColorExtensions.m_colorToHexDict.Add(color, text);
			}
			return text;
		}

		// Token: 0x06001653 RID: 5715 RVA: 0x00051950 File Offset: 0x0004FB50
		public static Color FromHexLiteral(uint hexLiteral)
		{
			return new Color32((byte)(hexLiteral >> 24), (byte)(hexLiteral >> 16), (byte)(hexLiteral >> 8), (byte)hexLiteral);
		}

		// Token: 0x06001654 RID: 5716 RVA: 0x0010034C File Offset: 0x000FE54C
		public static Color GetRandomColor(bool randomizeAlpha = false)
		{
			return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), randomizeAlpha ? UnityEngine.Random.Range(0f, 1f) : 1f);
		}

		// Token: 0x06001655 RID: 5717 RVA: 0x0005196C File Offset: 0x0004FB6C
		public static Color GetColorWithAlpha(this Color color, float alpha)
		{
			color.a = Mathf.Clamp(alpha, 0f, 1f);
			return color;
		}

		// Token: 0x04001E4B RID: 7755
		private static readonly Dictionary<Color, string> m_colorToHexDict = new Dictionary<Color, string>(default(ColorComparer));
	}
}
