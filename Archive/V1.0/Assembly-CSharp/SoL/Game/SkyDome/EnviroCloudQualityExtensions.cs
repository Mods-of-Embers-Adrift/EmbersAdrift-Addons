using System;

namespace SoL.Game.SkyDome
{
	// Token: 0x020006F3 RID: 1779
	public static class EnviroCloudQualityExtensions
	{
		// Token: 0x060035B0 RID: 13744 RVA: 0x00064C16 File Offset: 0x00062E16
		public static string GetDescription(this EnviroCloudQuality quality)
		{
			if (quality == EnviroCloudQuality.VeryLow)
			{
				return "Very Low";
			}
			return quality.ToString();
		}
	}
}
