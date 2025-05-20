using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.EditorOnly
{
	// Token: 0x02000DD1 RID: 3537
	[CreateAssetMenu(menuName = "SoL/Utils/Prefab Replacement Map")]
	public class PrefabReplacementMap : ScriptableObject
	{
		// Token: 0x04005BCF RID: 23503
		[SerializeField]
		private bool m_dryRun;

		// Token: 0x04005BD0 RID: 23504
		[FormerlySerializedAs("Pairs")]
		[SerializeField]
		private PrefabReplacementMap.PrefabPair[] m_pairs;

		// Token: 0x02000DD2 RID: 3538
		[Serializable]
		private class PrefabPair
		{
			// Token: 0x04005BD1 RID: 23505
			public GameObject Source;

			// Token: 0x04005BD2 RID: 23506
			public GameObject Target;
		}
	}
}
