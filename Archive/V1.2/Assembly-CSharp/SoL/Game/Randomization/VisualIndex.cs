using System;
using SoL.Game.Spawning;
using UnityEngine;

namespace SoL.Game.Randomization
{
	// Token: 0x02000776 RID: 1910
	[Serializable]
	public class VisualIndex
	{
		// Token: 0x06003865 RID: 14437 RVA: 0x0016D854 File Offset: 0x0016BA54
		public bool TryGetVisualIndex(SpawnablePrefab prefab, out int index)
		{
			index = 0;
			if (this.m_indexType == VisualIndex.IndexType.Default || prefab == null || prefab.IndexDescriptions == null || prefab.IndexDescriptions.Length == 0)
			{
				return false;
			}
			VisualIndex.IndexType indexType = this.m_indexType;
			if (indexType != VisualIndex.IndexType.Override)
			{
				if (indexType != VisualIndex.IndexType.Randomize)
				{
					return false;
				}
				index = UnityEngine.Random.Range(0, prefab.IndexDescriptions.Length);
			}
			else
			{
				index = this.m_index;
			}
			return true;
		}

		// Token: 0x0400373C RID: 14140
		[SerializeField]
		private SpawnablePrefab m_prefabReference;

		// Token: 0x0400373D RID: 14141
		[SerializeField]
		private VisualIndex.IndexType m_indexType;

		// Token: 0x0400373E RID: 14142
		[SerializeField]
		private int m_index;

		// Token: 0x02000777 RID: 1911
		private enum IndexType
		{
			// Token: 0x04003740 RID: 14144
			Default,
			// Token: 0x04003741 RID: 14145
			Override,
			// Token: 0x04003742 RID: 14146
			Randomize
		}
	}
}
