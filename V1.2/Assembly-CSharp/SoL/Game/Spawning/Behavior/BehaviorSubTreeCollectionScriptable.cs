using System;
using UnityEngine;

namespace SoL.Game.Spawning.Behavior
{
	// Token: 0x020006E1 RID: 1761
	[CreateAssetMenu(menuName = "SoL/Profiles/Behavior SubTree Collection")]
	public class BehaviorSubTreeCollectionScriptable : ScriptableObject
	{
		// Token: 0x17000BC6 RID: 3014
		// (get) Token: 0x06003549 RID: 13641 RVA: 0x0006475A File Offset: 0x0006295A
		internal BehaviorSubTree[] BehaviorSubTrees
		{
			get
			{
				BehaviorSubTreeCollection collection = this.m_collection;
				if (collection == null)
				{
					return null;
				}
				return collection.BehaviorSubTrees;
			}
		}

		// Token: 0x04003366 RID: 13158
		[SerializeField]
		private BehaviorSubTreeCollection m_collection;
	}
}
