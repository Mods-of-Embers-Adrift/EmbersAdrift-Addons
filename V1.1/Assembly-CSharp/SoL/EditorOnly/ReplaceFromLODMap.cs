using System;
using UnityEngine;

namespace SoL.EditorOnly
{
	// Token: 0x02000DCE RID: 3534
	public class ReplaceFromLODMap : MonoBehaviour
	{
		// Token: 0x04005BBF RID: 23487
		[SerializeField]
		private LODObjectMap m_map;

		// Token: 0x04005BC0 RID: 23488
		[SerializeField]
		private LODObjectMap.LODIndex m_sourceIndex;

		// Token: 0x04005BC1 RID: 23489
		[SerializeField]
		private LODObjectMap.LODIndex m_targetIndex = LODObjectMap.LODIndex.k1;
	}
}
