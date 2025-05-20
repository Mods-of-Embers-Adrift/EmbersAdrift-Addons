using System;
using System.Collections.Generic;
using SoL.Game.Login.Client.Creation.NewCreation;
using SoL.Game.Settings;
using UMA;
using UMA.CharacterSystem;
using UnityEngine;

namespace SoL.Networking.Database
{
	// Token: 0x0200042A RID: 1066
	public static class CharacterColorTypeExtensions
	{
		// Token: 0x06001E84 RID: 7812 RVA: 0x0011AEA8 File Offset: 0x001190A8
		private static string ToStringCached(this CharacterColorType colorType)
		{
			if (CharacterColorTypeExtensions.m_colorTypeStringDict == null)
			{
				CharacterColorTypeExtensions.m_colorTypeStringDict = new Dictionary<CharacterColorType, string>(default(CharacterColorTypeComparer));
			}
			string text;
			if (!CharacterColorTypeExtensions.m_colorTypeStringDict.TryGetValue(colorType, out text))
			{
				text = colorType.ToString();
				CharacterColorTypeExtensions.m_colorTypeStringDict.Add(colorType, text);
			}
			return text;
		}

		// Token: 0x06001E85 RID: 7813 RVA: 0x0011AF00 File Offset: 0x00119100
		public static void SetDcaSharedColor(this CharacterColorType colorType, DynamicCharacterAvatar dca, string colorHex)
		{
			Color color;
			if (ColorUtility.TryParseHtmlString(colorHex, out color))
			{
				colorType.SetDcaSharedColor(dca, color);
			}
		}

		// Token: 0x06001E86 RID: 7814 RVA: 0x0011AF20 File Offset: 0x00119120
		public static void SetDcaSharedColor(this CharacterColorType colorType, DynamicCharacterAvatar dca, Color color)
		{
			if (dca)
			{
				string text = colorType.ToStringCached();
				Color correctedUMAColor = colorType.GetCorrectedUMAColor(color);
				dca.SetColor(text, correctedUMAColor, default(Color), 0f, false);
				if (colorType == CharacterColorType.Hair)
				{
					CharacterColorTypeExtensions.UpdateHairSpecularColor(dca, text, correctedUMAColor);
				}
			}
		}

		// Token: 0x06001E87 RID: 7815 RVA: 0x0011AF68 File Offset: 0x00119168
		private static void UpdateHairSpecularColor(DynamicCharacterAvatar dca, string colorTypeString, Color correctedColor)
		{
			OverlayColorData color = dca.GetColor(colorTypeString);
			if (color.PropertyBlock == null)
			{
				color.PropertyBlock = new UMAMaterialPropertyBlock();
			}
			UMAColorProperty umacolorProperty = null;
			foreach (UMAProperty umaproperty in color.PropertyBlock.shaderProperties)
			{
				if (umaproperty.name == "_SpecularColor")
				{
					umacolorProperty = (umaproperty as UMAColorProperty);
					break;
				}
			}
			if (umacolorProperty == null)
			{
				umacolorProperty = (color.PropertyBlock.AddProperty<UMAColorProperty>("_SpecularColor") as UMAColorProperty);
			}
			if (umacolorProperty != null)
			{
				umacolorProperty.Value = correctedColor;
			}
		}

		// Token: 0x06001E88 RID: 7816 RVA: 0x0011B014 File Offset: 0x00119214
		private static Color GetCorrectedUMAColor(this CharacterColorType colorType, Color color)
		{
			switch (colorType)
			{
			case CharacterColorType.Skin:
				color *= GlobalSettings.Values.Uma.SkinColorCorrection;
				break;
			case CharacterColorType.Hair:
				color *= GlobalSettings.Values.Uma.HairColorCorrection;
				break;
			case CharacterColorType.Eyes:
				color *= GlobalSettings.Values.Uma.EyeColorCorrection;
				break;
			case CharacterColorType.Favorite:
				color *= GlobalSettings.Values.Uma.FavoriteColorCorrection;
				break;
			}
			color.r = Mathf.Clamp01(color.r);
			color.g = Mathf.Clamp01(color.g);
			color.b = Mathf.Clamp01(color.b);
			color.a = Mathf.Clamp01(color.a);
			return color;
		}

		// Token: 0x06001E89 RID: 7817 RVA: 0x0011B0E8 File Offset: 0x001192E8
		public static ColorSampler GetColorSampler(this CharacterColorType colorType)
		{
			ColorSampler result = null;
			switch (colorType)
			{
			case CharacterColorType.Skin:
				result = GlobalSettings.Values.Uma.SkinColorSampler;
				break;
			case CharacterColorType.Hair:
				result = GlobalSettings.Values.Uma.HairColorSampler;
				break;
			case CharacterColorType.Eyes:
				result = GlobalSettings.Values.Uma.EyeColorSampler;
				break;
			case CharacterColorType.Favorite:
				result = GlobalSettings.Values.Uma.FavoriteColorSampler;
				break;
			}
			return result;
		}

		// Token: 0x040023E5 RID: 9189
		private static Dictionary<CharacterColorType, string> m_colorTypeStringDict;
	}
}
