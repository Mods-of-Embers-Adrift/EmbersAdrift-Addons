using System;
using SoL.Game.Spawning;
using SoL.Game.Targeting;
using UnityEngine;

namespace SoL.Game.NPCs
{
	// Token: 0x020007FA RID: 2042
	public class FlatwormController : GameEntityComponent, INpcTargetController
	{
		// Token: 0x06003B45 RID: 15173 RVA: 0x0004475B File Offset: 0x0004295B
		public void Initialize(int flatwormLevel, float radius, FlatwormSpawnProfile.ScriptableEffectsByLevel[] effectsByLevel)
		{
		}

		// Token: 0x17000D8E RID: 3470
		// (get) Token: 0x06003B46 RID: 15174 RVA: 0x00068231 File Offset: 0x00066431
		int INpcTargetController.HostileTargetCount
		{
			get
			{
				return this.m_hostileCount;
			}
		}

		// Token: 0x17000D8F RID: 3471
		// (get) Token: 0x06003B47 RID: 15175 RVA: 0x00045BCA File Offset: 0x00043DCA
		int INpcTargetController.AlertCount
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000D90 RID: 3472
		// (get) Token: 0x06003B48 RID: 15176 RVA: 0x00045BCA File Offset: 0x00043DCA
		int INpcTargetController.NeutralTargetCount
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06003B49 RID: 15177 RVA: 0x0006109C File Offset: 0x0005F29C
		float INpcTargetController.GetHostileSpeedPercentage()
		{
			return 1f;
		}

		// Token: 0x17000D91 RID: 3473
		// (get) Token: 0x06003B4A RID: 15178 RVA: 0x00164B40 File Offset: 0x00162D40
		Vector3? INpcTargetController.ResetPosition
		{
			get
			{
				return null;
			}
		}

		// Token: 0x040039BA RID: 14778
		[SerializeField]
		private FlatwormApplicator m_applicator;

		// Token: 0x040039BB RID: 14779
		private int m_hostileCount;
	}
}
