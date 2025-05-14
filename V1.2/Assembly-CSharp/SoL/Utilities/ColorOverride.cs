using System;
using System.Collections;
using SoL.Game.Login.Client.Creation.NewCreation;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000252 RID: 594
	[Serializable]
	public class ColorOverride
	{
		// Token: 0x170004E6 RID: 1254
		// (get) Token: 0x06001348 RID: 4936 RVA: 0x0004F99A File Offset: 0x0004DB9A
		public bool IsRandom
		{
			get
			{
				return this.m_overrideColor == ColorOverride.OverrideType.Random && this.m_randomSampler != null;
			}
		}

		// Token: 0x170004E7 RID: 1255
		// (get) Token: 0x06001349 RID: 4937 RVA: 0x0004F9B4 File Offset: 0x0004DBB4
		public bool UsesIndex
		{
			get
			{
				return this.m_showRandomSampler && this.m_randomSampler != null;
			}
		}

		// Token: 0x170004E8 RID: 1256
		// (get) Token: 0x0600134A RID: 4938 RVA: 0x0004F9CC File Offset: 0x0004DBCC
		public int IndexCount
		{
			get
			{
				if (!this.UsesIndex)
				{
					return 0;
				}
				return this.m_randomSampler.ColorCount;
			}
		}

		// Token: 0x170004E9 RID: 1257
		// (get) Token: 0x0600134B RID: 4939 RVA: 0x0004F9E3 File Offset: 0x0004DBE3
		private bool m_showRandomSampler
		{
			get
			{
				return this.m_overrideColor == ColorOverride.OverrideType.Random || this.m_overrideColor == ColorOverride.OverrideType.Index;
			}
		}

		// Token: 0x0600134C RID: 4940 RVA: 0x000E9264 File Offset: 0x000E7464
		public Color? GetColorOverride(byte? colorIndex)
		{
			ColorOverride.OverrideType overrideColor = this.m_overrideColor;
			switch (overrideColor)
			{
			case ColorOverride.OverrideType.Custom:
				return new Color?(this.m_customColor);
			case ColorOverride.OverrideType.Leather:
				return new Color?(this.m_leatherColor);
			case ColorOverride.OverrideType.Favorite:
				return new Color?(this.m_favoriteColor);
			case ColorOverride.OverrideType.Metal:
				return new Color?(this.m_metalColor);
			case ColorOverride.OverrideType.Cloth:
				return new Color?(this.m_clothColor);
			default:
				if (overrideColor - ColorOverride.OverrideType.Random > 1)
				{
					return null;
				}
				if (!(this.m_randomSampler != null) || colorIndex == null || (int)colorIndex.Value >= this.m_randomSampler.ColorCount)
				{
					return null;
				}
				return new Color?(this.m_randomSampler.GetColorByIndex((int)colorIndex.Value));
			}
		}

		// Token: 0x0600134D RID: 4941 RVA: 0x000E9334 File Offset: 0x000E7534
		public string GetColorName(byte? colorIndex)
		{
			if (!(this.m_randomSampler != null) || colorIndex == null || (int)colorIndex.Value >= this.m_randomSampler.ColorCount)
			{
				return string.Empty;
			}
			return this.m_randomSampler.GetNameByIndex((int)colorIndex.Value);
		}

		// Token: 0x170004EA RID: 1258
		// (get) Token: 0x0600134E RID: 4942 RVA: 0x0004F9FB File Offset: 0x0004DBFB
		private IEnumerable GetColorValues
		{
			get
			{
				return SolOdinUtilities.GetColorValues();
			}
		}

		// Token: 0x170004EB RID: 1259
		// (get) Token: 0x0600134F RID: 4943 RVA: 0x0004FA02 File Offset: 0x0004DC02
		private IEnumerable GetColorSamplers
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<ColorSampler>();
			}
		}

		// Token: 0x04001102 RID: 4354
		private const string kGroupName = "Color Override";

		// Token: 0x04001103 RID: 4355
		private const string kLeatherPalette = "Leather";

		// Token: 0x04001104 RID: 4356
		private const string kFavoritePalette = "Favorite";

		// Token: 0x04001105 RID: 4357
		private const string kMetalPalette = "Metal";

		// Token: 0x04001106 RID: 4358
		private const string kClothPalette = "Cloth";

		// Token: 0x04001107 RID: 4359
		[SerializeField]
		private ColorOverride.OverrideType m_overrideColor;

		// Token: 0x04001108 RID: 4360
		[SerializeField]
		private Color m_customColor = Color.white;

		// Token: 0x04001109 RID: 4361
		[SerializeField]
		private Color m_leatherColor = Color.white;

		// Token: 0x0400110A RID: 4362
		[SerializeField]
		private Color m_favoriteColor = Color.white;

		// Token: 0x0400110B RID: 4363
		[SerializeField]
		private Color m_metalColor = Color.white;

		// Token: 0x0400110C RID: 4364
		[SerializeField]
		private Color m_clothColor = Color.white;

		// Token: 0x0400110D RID: 4365
		[SerializeField]
		private ColorSampler m_randomSampler;

		// Token: 0x02000253 RID: 595
		private enum OverrideType
		{
			// Token: 0x0400110F RID: 4367
			None,
			// Token: 0x04001110 RID: 4368
			Custom,
			// Token: 0x04001111 RID: 4369
			Leather,
			// Token: 0x04001112 RID: 4370
			Favorite,
			// Token: 0x04001113 RID: 4371
			Metal,
			// Token: 0x04001114 RID: 4372
			Cloth,
			// Token: 0x04001115 RID: 4373
			Random = 100,
			// Token: 0x04001116 RID: 4374
			Index
		}
	}
}
