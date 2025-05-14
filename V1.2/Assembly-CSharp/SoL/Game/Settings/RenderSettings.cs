using System;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x0200073D RID: 1853
	[Serializable]
	public class RenderSettings
	{
		// Token: 0x17000C79 RID: 3193
		// (get) Token: 0x0600376B RID: 14187 RVA: 0x00065ECC File Offset: 0x000640CC
		public SingleUnityLayer GlobalResourceNodeLayer
		{
			get
			{
				return this.m_globalResourceNodeLayer;
			}
		}

		// Token: 0x0600376C RID: 14188 RVA: 0x0016BA54 File Offset: 0x00169C54
		public void SetLayerCullingDistanceArray(Camera camera)
		{
			if (camera == null || this.m_distanceCullingLayers == null || this.m_distanceCullingLayers.Length == 0)
			{
				return;
			}
			float[] array = new float[32];
			for (int i = 0; i < this.m_distanceCullingLayers.Length; i++)
			{
				int layerIndex = this.m_distanceCullingLayers[i].LayerIndex;
				array[layerIndex] = this.m_distanceCullingLayers[i].Distance;
			}
			camera.layerCullDistances = array;
		}

		// Token: 0x04003639 RID: 13881
		[SerializeField]
		private RenderSettings.DistanceCullingLayer[] m_distanceCullingLayers;

		// Token: 0x0400363A RID: 13882
		[SerializeField]
		private SingleUnityLayer m_globalResourceNodeLayer;

		// Token: 0x0200073E RID: 1854
		[Serializable]
		private class DistanceCullingLayer
		{
			// Token: 0x17000C7A RID: 3194
			// (get) Token: 0x0600376E RID: 14190 RVA: 0x00065ED4 File Offset: 0x000640D4
			public int LayerIndex
			{
				get
				{
					return this.m_layer.LayerIndex;
				}
			}

			// Token: 0x17000C7B RID: 3195
			// (get) Token: 0x0600376F RID: 14191 RVA: 0x00065EE1 File Offset: 0x000640E1
			public float Distance
			{
				get
				{
					return this.m_distance;
				}
			}

			// Token: 0x0400363B RID: 13883
			[SerializeField]
			private SingleUnityLayer m_layer;

			// Token: 0x0400363C RID: 13884
			[SerializeField]
			private float m_distance;
		}
	}
}
