using System;
using UMA.CharacterSystem;
using UnityEngine;

namespace SoL.Game.UMA
{
	// Token: 0x02000626 RID: 1574
	public class WardrobeRecipePairSampler : MonoBehaviour
	{
		// Token: 0x04003022 RID: 12322
		[SerializeField]
		private DynamicCharacterAvatar m_male;

		// Token: 0x04003023 RID: 12323
		[SerializeField]
		private DynamicCharacterAvatar m_female;

		// Token: 0x04003024 RID: 12324
		[SerializeField]
		private WardrobeRecipePair[] m_pairs;
	}
}
