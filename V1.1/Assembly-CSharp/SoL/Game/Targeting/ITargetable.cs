using System;
using SoL.Networking.Replication;

namespace SoL.Game.Targeting
{
	// Token: 0x02000652 RID: 1618
	public interface ITargetable
	{
		// Token: 0x17000ABB RID: 2747
		// (get) Token: 0x06003269 RID: 12905
		GameEntity Entity { get; }

		// Token: 0x17000ABC RID: 2748
		// (get) Token: 0x0600326A RID: 12906
		Faction Faction { get; }

		// Token: 0x17000ABD RID: 2749
		// (get) Token: 0x0600326B RID: 12907
		bool IsPlayer { get; }

		// Token: 0x17000ABE RID: 2750
		// (get) Token: 0x0600326C RID: 12908
		bool IsNpc { get; }

		// Token: 0x17000ABF RID: 2751
		// (get) Token: 0x0600326D RID: 12909
		int Level { get; }

		// Token: 0x17000AC0 RID: 2752
		// (get) Token: 0x0600326E RID: 12910
		float DistanceBuffer { get; }

		// Token: 0x17000AC1 RID: 2753
		// (get) Token: 0x0600326F RID: 12911
		float? ReticleRadiusOverride { get; }

		// Token: 0x17000AC2 RID: 2754
		// (get) Token: 0x06003270 RID: 12912
		SynchronizedString Name { get; }

		// Token: 0x17000AC3 RID: 2755
		// (get) Token: 0x06003271 RID: 12913
		SynchronizedString Title { get; }

		// Token: 0x17000AC4 RID: 2756
		// (get) Token: 0x06003272 RID: 12914
		SynchronizedString Guild { get; }

		// Token: 0x17000AC5 RID: 2757
		// (get) Token: 0x06003273 RID: 12915
		SynchronizedEnum<PlayerFlags> PlayerFlags { get; }
	}
}
