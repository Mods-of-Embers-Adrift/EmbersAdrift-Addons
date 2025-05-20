using System;
using UMA.CharacterSystem;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005E2 RID: 1506
	[CreateAssetMenu(menuName = "SoL/UMA/Wardrobe Recipe Pair Assigner", order = 5)]
	public class WardrobePairAssigner : ScriptableObject
	{
		// Token: 0x06002FCE RID: 12238 RVA: 0x0004475B File Offset: 0x0004295B
		private void Assign()
		{
		}

		// Token: 0x06002FCF RID: 12239 RVA: 0x0004475B File Offset: 0x0004295B
		private void Read()
		{
		}

		// Token: 0x06002FD0 RID: 12240 RVA: 0x0004475B File Offset: 0x0004295B
		private void Clear()
		{
		}

		// Token: 0x04002EB7 RID: 11959
		[SerializeField]
		private WardrobePairAssigner.PairReferences[] m_references;

		// Token: 0x020005E3 RID: 1507
		[Serializable]
		private class PairReferences
		{
			// Token: 0x04002EB8 RID: 11960
			public WardrobeRecipePair Pair;

			// Token: 0x04002EB9 RID: 11961
			public UMAWardrobeRecipe Male;

			// Token: 0x04002EBA RID: 11962
			public UMAWardrobeRecipe Female;
		}
	}
}
