using System;

namespace SoL.Utilities
{
	// Token: 0x02000255 RID: 597
	public static class ColorPaletteTypeExtensions
	{
		// Token: 0x06001351 RID: 4945 RVA: 0x000E9384 File Offset: 0x000E7584
		public static string GetColorPaletteName(this ColorPaletteTypes type)
		{
			if (type <= ColorPaletteTypes.Rock)
			{
				if (type == ColorPaletteTypes.Waypoints)
				{
					return "Waypoints";
				}
				if (type == ColorPaletteTypes.Rock)
				{
					return "Rock";
				}
			}
			else
			{
				switch (type)
				{
				case ColorPaletteTypes.Skin:
					return "Skin Colors";
				case ColorPaletteTypes.Hair:
					return "Hair Colors";
				case ColorPaletteTypes.Eyes:
					return "Eye Colors";
				default:
					if (type == ColorPaletteTypes.Favorite)
					{
						return "Favorite";
					}
					if (type == ColorPaletteTypes.Leather)
					{
						return "Leather";
					}
					break;
				}
			}
			return "Unknown";
		}
	}
}
