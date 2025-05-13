using System;
using Expanse;

namespace SoL.Game.SkyDome
{
	// Token: 0x020006F5 RID: 1781
	public static class ExpanseExtensions
	{
		// Token: 0x060035B2 RID: 13746 RVA: 0x00064C2E File Offset: 0x00062E2E
		public static string GetDescription(this Datatypes.Quality quality)
		{
			switch (quality)
			{
			case Datatypes.Quality.Potato:
				return "Low";
			case Datatypes.Quality.Low:
				return "Medium";
			case Datatypes.Quality.Medium:
				return "High";
			default:
				return quality.ToString();
			}
		}

		// Token: 0x060035B3 RID: 13747 RVA: 0x00064C63 File Offset: 0x00062E63
		public static bool IncludeInCloudQualityOptions(this Datatypes.Quality quality)
		{
			return quality != Datatypes.Quality.RippingThroughTheMetaverse;
		}

		// Token: 0x040033B2 RID: 13234
		public static readonly Datatypes.Quality[] AvailableQualities = new Datatypes.Quality[]
		{
			Datatypes.Quality.Potato,
			Datatypes.Quality.Low,
			Datatypes.Quality.Medium
		};
	}
}
