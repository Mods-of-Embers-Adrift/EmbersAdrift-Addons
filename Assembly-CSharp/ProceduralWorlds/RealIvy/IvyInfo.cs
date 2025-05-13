using System;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x02000094 RID: 148
	public class IvyInfo : MonoBehaviour
	{
		// Token: 0x060005CC RID: 1484 RVA: 0x0004705F File Offset: 0x0004525F
		public void Setup(InfoPool infoPool, MeshFilter meshFilter, MeshRenderer meshRenderer, IvyPreset originalPreset)
		{
			this.infoPool = infoPool;
			this.meshFilter = meshFilter;
			this.meshRenderer = meshRenderer;
			this.originalPreset = originalPreset;
		}

		// Token: 0x04000684 RID: 1668
		public IvyPreset originalPreset;

		// Token: 0x04000685 RID: 1669
		public InfoPool infoPool;

		// Token: 0x04000686 RID: 1670
		public MeshFilter meshFilter;

		// Token: 0x04000687 RID: 1671
		public MeshRenderer meshRenderer;
	}
}
