using System;
using UnityEngine;

namespace SoL.Game.Scenes
{
	// Token: 0x02000753 RID: 1875
	[Serializable]
	public class SceneConfigurationOptions
	{
		// Token: 0x040036C9 RID: 14025
		[SerializeField]
		private bool m_development;

		// Token: 0x040036CA RID: 14026
		[SerializeField]
		private bool m_allowDebugging;

		// Token: 0x040036CB RID: 14027
		[SerializeField]
		private bool m_deepProfilingSupport;

		// Token: 0x040036CC RID: 14028
		[SerializeField]
		private SceneConfigurationOptions.CompressionMethod m_compressionMethod;

		// Token: 0x02000754 RID: 1876
		private enum CompressionMethod
		{
			// Token: 0x040036CE RID: 14030
			Default,
			// Token: 0x040036CF RID: 14031
			LZ4,
			// Token: 0x040036D0 RID: 14032
			LC4HC
		}
	}
}
