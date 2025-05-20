using System;
using Drawing;
using UnityEngine;

namespace SoL.Utilities.DungeonGizmos
{
	// Token: 0x02000343 RID: 835
	public class DungeonPathDisplay : MonoBehaviourGizmos
	{
		// Token: 0x04001EA6 RID: 7846
		[SerializeField]
		private Color m_color = Color.white;

		// Token: 0x04001EA7 RID: 7847
		[SerializeField]
		private float m_thickness = 1f;

		// Token: 0x04001EA8 RID: 7848
		[SerializeField]
		private GameObject m_cap;

		// Token: 0x04001EA9 RID: 7849
		[SerializeField]
		private GameObject[] m_path;
	}
}
