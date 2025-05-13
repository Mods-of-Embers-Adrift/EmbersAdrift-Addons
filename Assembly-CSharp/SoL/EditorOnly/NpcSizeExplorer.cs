using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.Objects;
using SoL.Game.Spawning;
using SoL.Utilities;
using UnityEngine;

namespace SoL.EditorOnly
{
	// Token: 0x02000DCF RID: 3535
	[ExecuteInEditMode]
	public class NpcSizeExplorer : MonoBehaviour
	{
		// Token: 0x0600695B RID: 26971 RVA: 0x00086A6D File Offset: 0x00084C6D
		private IEnumerable GetPrefabReferences()
		{
			return SolOdinUtilities.GetDropdownItems<SpawnablePrefab>((SpawnablePrefab a) => a != null);
		}

		// Token: 0x04005BC2 RID: 23490
		[SerializeField]
		private bool m_update;

		// Token: 0x04005BC3 RID: 23491
		[SerializeField]
		private bool m_reverse;

		// Token: 0x04005BC4 RID: 23492
		[SerializeField]
		private bool m_showSizes;

		// Token: 0x04005BC5 RID: 23493
		[SerializeField]
		private GameObject m_prefabReference;

		// Token: 0x04005BC6 RID: 23494
		[SerializeField]
		private int m_number = 3;

		// Token: 0x04005BC7 RID: 23495
		[SerializeField]
		private float m_spacing = 1f;

		// Token: 0x04005BC8 RID: 23496
		[SerializeField]
		private MinMaxFloatRange m_minMaxSize = new MinMaxFloatRange(1f, 1f);

		// Token: 0x04005BC9 RID: 23497
		[SerializeField]
		private List<GameObject> m_npcs = new List<GameObject>();

		// Token: 0x04005BCA RID: 23498
		[SerializeField]
		protected SpawnablePrefab m_spawnableRef;

		// Token: 0x04005BCB RID: 23499
		private const string kPrefabRefGroupName = "PrefabReference";

		// Token: 0x04005BCC RID: 23500
		private float m_sizeDelta;
	}
}
