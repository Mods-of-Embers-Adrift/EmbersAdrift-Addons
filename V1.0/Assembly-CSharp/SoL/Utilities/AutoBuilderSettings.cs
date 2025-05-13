using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000248 RID: 584
	[CreateAssetMenu(menuName = "SoL/Auto Builder Settings")]
	public class AutoBuilderSettings : ScriptableObject
	{
		// Token: 0x040010EC RID: 4332
		public Shader[] ClientShadersToInclude;
	}
}
