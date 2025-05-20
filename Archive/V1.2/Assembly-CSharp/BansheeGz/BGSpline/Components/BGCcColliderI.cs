using System;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001AB RID: 427
	public interface BGCcColliderI
	{
		// Token: 0x17000415 RID: 1045
		// (get) Token: 0x06000F22 RID: 3874
		// (set) Token: 0x06000F23 RID: 3875
		GameObject ChildPrefab { get; set; }

		// Token: 0x17000416 RID: 1046
		// (get) Token: 0x06000F24 RID: 3876
		bool RequireGameObjects { get; }
	}
}
