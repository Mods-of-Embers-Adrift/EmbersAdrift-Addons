using System;
using System.Collections.Generic;
using AwesomeTechnologies.VegetationSystem;
using UnityEngine;

namespace SoL.Utilities.VSP
{
	// Token: 0x02000305 RID: 773
	public class VspBaker : MonoBehaviour
	{
		// Token: 0x04001DB2 RID: 7602
		[SerializeField]
		private GameObject m_imposterParent;

		// Token: 0x04001DB3 RID: 7603
		[SerializeField]
		private bool m_populateTreesOnly;

		// Token: 0x02000306 RID: 774
		[Serializable]
		private class VeggieData
		{
			// Token: 0x04001DB4 RID: 7604
			public bool ExcludeImposterBake;

			// Token: 0x04001DB5 RID: 7605
			public GameObject Prefab;

			// Token: 0x04001DB6 RID: 7606
			public List<string> VeggieIds;

			// Token: 0x04001DB7 RID: 7607
			public VegetationType Type;

			// Token: 0x04001DB8 RID: 7608
			public int Index;
		}

		// Token: 0x02000307 RID: 775
		private struct InstanceData
		{
			// Token: 0x04001DB9 RID: 7609
			public Vector3 Pos;

			// Token: 0x04001DBA RID: 7610
			public Quaternion Rot;

			// Token: 0x04001DBB RID: 7611
			public Vector3 Scale;
		}
	}
}
