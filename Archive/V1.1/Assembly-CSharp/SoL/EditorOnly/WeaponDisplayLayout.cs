using System;
using UnityEngine;

namespace SoL.EditorOnly
{
	// Token: 0x02000DD8 RID: 3544
	public class WeaponDisplayLayout : MonoBehaviour
	{
		// Token: 0x04005BF1 RID: 23537
		[SerializeField]
		private GameObject[] m_objects;

		// Token: 0x04005BF2 RID: 23538
		[SerializeField]
		private WeaponDisplayLayout.ReplacementData[] m_replacements;

		// Token: 0x02000DD9 RID: 3545
		[Serializable]
		private class ReplacementData
		{
			// Token: 0x04005BF3 RID: 23539
			public GameObject ToReplace;

			// Token: 0x04005BF4 RID: 23540
			public GameObject Replacement;
		}
	}
}
