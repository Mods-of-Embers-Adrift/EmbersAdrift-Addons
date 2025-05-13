using System;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000243 RID: 579
	[CreateAssetMenu(menuName = "SoL/ArmorSetEditor")]
	public class ArmorSetEditor : ScriptableObject
	{
		// Token: 0x040010DF RID: 4319
		[SerializeField]
		private ArmorItem m_helm;

		// Token: 0x040010E0 RID: 4320
		[SerializeField]
		private ArmorItem m_shoulder;

		// Token: 0x040010E1 RID: 4321
		[SerializeField]
		private ArmorItem m_chest;

		// Token: 0x040010E2 RID: 4322
		[SerializeField]
		private ArmorItem m_wrist;

		// Token: 0x040010E3 RID: 4323
		[SerializeField]
		private ArmorItem m_gloves;

		// Token: 0x040010E4 RID: 4324
		[SerializeField]
		private ArmorItem m_legs;

		// Token: 0x040010E5 RID: 4325
		[SerializeField]
		private ArmorItem m_boots;
	}
}
