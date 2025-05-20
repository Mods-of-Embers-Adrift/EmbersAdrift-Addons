using System;
using SoL.Game.Pooling;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C05 RID: 3077
	[Serializable]
	public class AbilityVFX
	{
		// Token: 0x040051F1 RID: 20977
		public const string kGroupName = "VFX";

		// Token: 0x040051F2 RID: 20978
		[Tooltip("Applies to the source ON execution and times out (triggered via Pending)")]
		public PooledVFX SourceExecution;

		// Token: 0x040051F3 RID: 20979
		[Tooltip("Applies to the source ON application and times out (triggered via Pending)")]
		public PooledVFX SourceApplication;

		// Token: 0x040051F4 RID: 20980
		[Tooltip("Applies to the target ON execution and times out (triggered via Pending)")]
		public PooledVFX TargetExecution;

		// Token: 0x040051F5 RID: 20981
		[Tooltip("Applies to the target ON application and times out (triggered via Pending)")]
		public PooledVFX TargetApplication;

		// Token: 0x040051F6 RID: 20982
		[Tooltip("Applies to the target ON application and lasts for the duration (triggered via Pending)")]
		public PooledVFX TargetLasting;

		// Token: 0x040051F7 RID: 20983
		[Tooltip("Applies to the target ON application and times out (triggered via Combat Text results)")]
		public PooledVFX TargetAoeApplication;

		// Token: 0x040051F8 RID: 20984
		public PooledProjectile Projectile;
	}
}
