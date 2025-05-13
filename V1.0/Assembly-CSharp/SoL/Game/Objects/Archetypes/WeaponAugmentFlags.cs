using System;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AB0 RID: 2736
	[Flags]
	public enum WeaponAugmentFlags
	{
		// Token: 0x04004B1B RID: 19227
		None = 0,
		// Token: 0x04004B1C RID: 19228
		DiceMod = 1,
		// Token: 0x04004B1D RID: 19229
		DamagePercent = 2,
		// Token: 0x04004B1E RID: 19230
		Hit = 4,
		// Token: 0x04004B1F RID: 19231
		[InspectorName("Positional")]
		Penetration = 8,
		// Token: 0x04004B20 RID: 19232
		Flanking = 16
	}
}
