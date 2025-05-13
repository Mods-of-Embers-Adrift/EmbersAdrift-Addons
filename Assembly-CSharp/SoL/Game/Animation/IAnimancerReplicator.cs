using System;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D80 RID: 3456
	public interface IAnimancerReplicator
	{
		// Token: 0x17001902 RID: 6402
		// (get) Token: 0x06006817 RID: 26647
		// (set) Token: 0x06006818 RID: 26648
		Vector2 RawLocomotion { get; set; }

		// Token: 0x17001903 RID: 6403
		// (get) Token: 0x06006819 RID: 26649
		// (set) Token: 0x0600681A RID: 26650
		float RawRotation { get; set; }

		// Token: 0x17001904 RID: 6404
		// (get) Token: 0x0600681B RID: 26651
		// (set) Token: 0x0600681C RID: 26652
		float Speed { get; set; }
	}
}
