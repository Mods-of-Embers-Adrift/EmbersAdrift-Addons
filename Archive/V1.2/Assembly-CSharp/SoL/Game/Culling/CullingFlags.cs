using System;

namespace SoL.Game.Culling
{
	// Token: 0x02000CC5 RID: 3269
	[Flags]
	public enum CullingFlags
	{
		// Token: 0x04005642 RID: 22082
		None = 0,
		// Token: 0x04005643 RID: 22083
		Manual = 1,
		// Token: 0x04005644 RID: 22084
		Distance = 2,
		// Token: 0x04005645 RID: 22085
		DayNight = 4,
		// Token: 0x04005646 RID: 22086
		LightShadowDistance = 8,
		// Token: 0x04005647 RID: 22087
		LightShadowLimit = 16,
		// Token: 0x04005648 RID: 22088
		ObjectShadowLimit = 32,
		// Token: 0x04005649 RID: 22089
		IKLimit = 64,
		// Token: 0x0400564A RID: 22090
		UmaFeatureLimit = 128,
		// Token: 0x0400564B RID: 22091
		Physics = 256
	}
}
