using System;
using UnityEngine;

namespace SoL.Game.SkyDome
{
	// Token: 0x020006FD RID: 1789
	public interface ISkyDomeController
	{
		// Token: 0x140000B0 RID: 176
		// (add) Token: 0x060035E6 RID: 13798
		// (remove) Token: 0x060035E7 RID: 13799
		event Action DayNightChanged;

		// Token: 0x17000BED RID: 3053
		// (get) Token: 0x060035E8 RID: 13800
		bool IsDay { get; }

		// Token: 0x060035E9 RID: 13801
		void SetTime(DateTime time);

		// Token: 0x060035EA RID: 13802
		DateTime GetTime();

		// Token: 0x17000BEE RID: 3054
		// (get) Token: 0x060035EB RID: 13803
		Light Light { get; }

		// Token: 0x17000BEF RID: 3055
		// (get) Token: 0x060035EC RID: 13804
		WindZone Wind { get; }

		// Token: 0x17000BF0 RID: 3056
		// (get) Token: 0x060035ED RID: 13805
		// (set) Token: 0x060035EE RID: 13806
		bool ProgressTime { get; set; }

		// Token: 0x17000BF1 RID: 3057
		// (get) Token: 0x060035EF RID: 13807
		Color LightColor { get; }

		// Token: 0x17000BF2 RID: 3058
		// (get) Token: 0x060035F0 RID: 13808
		float LightTemperature { get; }

		// Token: 0x17000BF3 RID: 3059
		// (get) Token: 0x060035F1 RID: 13809
		float SunAltitude { get; }

		// Token: 0x17000BF4 RID: 3060
		// (get) Token: 0x060035F2 RID: 13810
		bool IsIndoors { get; }

		// Token: 0x060035F3 RID: 13811
		void ResetSkybox();
	}
}
