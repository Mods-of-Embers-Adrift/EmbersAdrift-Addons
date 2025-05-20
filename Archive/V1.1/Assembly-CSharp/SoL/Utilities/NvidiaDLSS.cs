using System;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Utilities
{
	// Token: 0x020002A9 RID: 681
	public static class NvidiaDLSS
	{
		// Token: 0x17000507 RID: 1287
		// (get) Token: 0x06001447 RID: 5191 RVA: 0x00050333 File Offset: 0x0004E533
		public static string[] QualityNames
		{
			get
			{
				if (NvidiaDLSS.m_qualityNames == null)
				{
					NvidiaDLSS.m_qualityNames = Enum.GetNames(typeof(NvidiaDLSSQuality));
				}
				return NvidiaDLSS.m_qualityNames;
			}
		}

		// Token: 0x17000508 RID: 1288
		// (get) Token: 0x06001448 RID: 5192 RVA: 0x00050355 File Offset: 0x0004E555
		public static bool SupportsNvidiaDLSS
		{
			get
			{
				return HDDynamicResolutionPlatformCapabilities.DLSSDetected;
			}
		}

		// Token: 0x06001449 RID: 5193 RVA: 0x0005035C File Offset: 0x0004E55C
		public static int GetOptionValue()
		{
			if (Options.VideoOptions.NvidiaDLSSQuality.Value >= NvidiaDLSS.QualityNames.Length)
			{
				Options.VideoOptions.NvidiaDLSSQuality.Value = 0;
			}
			return Options.VideoOptions.NvidiaDLSSQuality.Value;
		}

		// Token: 0x04001CA1 RID: 7329
		private static string[] m_qualityNames;
	}
}
