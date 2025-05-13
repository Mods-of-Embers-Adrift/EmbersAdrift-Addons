using System;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x0200038C RID: 908
	[Serializable]
	public struct TooltipSettings
	{
		// Token: 0x04001FE3 RID: 8163
		public bool UseOverrideDelay;

		// Token: 0x04001FE4 RID: 8164
		public float Delay;

		// Token: 0x04001FE5 RID: 8165
		public Transform Anchor;

		// Token: 0x04001FE6 RID: 8166
		public TextAnchor PivotForAnchor;
	}
}
