using System;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x020000A7 RID: 167
	[Serializable]
	public class RuntimeGrowthParameters
	{
		// Token: 0x06000677 RID: 1655 RVA: 0x000A88C0 File Offset: 0x000A6AC0
		public RuntimeGrowthParameters()
		{
			this.growthSpeed = 25f;
			this.lifetime = 5f;
			this.speedOverLifetimeEnabled = false;
			this.speedOverLifetimeCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(0.2f, 1f),
				new Keyframe(0.8f, 1f),
				new Keyframe(1f, 0f)
			});
			this.delay = 0f;
			this.startGrowthOnAwake = true;
		}

		// Token: 0x04000771 RID: 1905
		public float growthSpeed;

		// Token: 0x04000772 RID: 1906
		public float lifetime;

		// Token: 0x04000773 RID: 1907
		public bool speedOverLifetimeEnabled;

		// Token: 0x04000774 RID: 1908
		public AnimationCurve speedOverLifetimeCurve;

		// Token: 0x04000775 RID: 1909
		public float delay;

		// Token: 0x04000776 RID: 1910
		public bool startGrowthOnAwake;
	}
}
