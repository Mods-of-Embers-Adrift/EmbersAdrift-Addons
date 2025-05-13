using System;
using UnityEngine;

namespace SoL.EditorOnly
{
	// Token: 0x02000DCB RID: 3531
	[CreateAssetMenu(menuName = "SoL/Migration/LOD Object Map")]
	public class LODObjectMap : ScriptableObject
	{
		// Token: 0x17001923 RID: 6435
		// (get) Token: 0x06006956 RID: 26966 RVA: 0x00086A21 File Offset: 0x00084C21
		internal LODObjectMap.LODMap[] Maps
		{
			get
			{
				return this.m_maps;
			}
		}

		// Token: 0x04005BB3 RID: 23475
		private const char kSplitChar = '_';

		// Token: 0x04005BB4 RID: 23476
		[SerializeField]
		private LODObjectMap.LODMap[] m_maps;

		// Token: 0x04005BB5 RID: 23477
		[SerializeField]
		private string m_k0Folder;

		// Token: 0x04005BB6 RID: 23478
		[SerializeField]
		private string m_k1Folder;

		// Token: 0x04005BB7 RID: 23479
		[SerializeField]
		private string m_k2Folder;

		// Token: 0x02000DCC RID: 3532
		internal enum LODIndex
		{
			// Token: 0x04005BB9 RID: 23481
			k0,
			// Token: 0x04005BBA RID: 23482
			k1,
			// Token: 0x04005BBB RID: 23483
			k2
		}

		// Token: 0x02000DCD RID: 3533
		[Serializable]
		internal class LODMap
		{
			// Token: 0x06006958 RID: 26968 RVA: 0x00086A29 File Offset: 0x00084C29
			public GameObject GetObjectForIndex(LODObjectMap.LODIndex index)
			{
				switch (index)
				{
				case LODObjectMap.LODIndex.k0:
					return this.LOD0;
				case LODObjectMap.LODIndex.k1:
					return this.LOD1;
				case LODObjectMap.LODIndex.k2:
					return this.LOD2;
				default:
					throw new ArgumentException("index");
				}
			}

			// Token: 0x04005BBC RID: 23484
			public GameObject LOD0;

			// Token: 0x04005BBD RID: 23485
			public GameObject LOD1;

			// Token: 0x04005BBE RID: 23486
			public GameObject LOD2;
		}
	}
}
