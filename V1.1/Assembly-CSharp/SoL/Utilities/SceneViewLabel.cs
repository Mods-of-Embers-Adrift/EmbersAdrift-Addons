using System;
using Drawing;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002C2 RID: 706
	public class SceneViewLabel : MonoBehaviourGizmos
	{
		// Token: 0x04001CF9 RID: 7417
		[SerializeField]
		private bool m_nameAsLabel;

		// Token: 0x04001CFA RID: 7418
		[SerializeField]
		private string m_label;

		// Token: 0x04001CFB RID: 7419
		[SerializeField]
		private Color m_color = Color.white;

		// Token: 0x04001CFC RID: 7420
		[SerializeField]
		private SceneViewLabel.IconType m_icon;

		// Token: 0x020002C3 RID: 707
		private enum IconType
		{
			// Token: 0x04001CFE RID: 7422
			None,
			// Token: 0x04001CFF RID: 7423
			crevice
		}
	}
}
