using System;
using SoL.Game.Objects;
using SoL.Networking.Replication;
using UnityEngine;

namespace SoL.Game.NPCs.Others
{
	// Token: 0x02000835 RID: 2101
	public class OtherVisibility : SyncVarReplicator
	{
		// Token: 0x17000E01 RID: 3585
		// (get) Token: 0x06003CD0 RID: 15568 RVA: 0x0006933A File Offset: 0x0006753A
		public SynchronizedBool IsHidden
		{
			get
			{
				return this.m_isHidden;
			}
		}

		// Token: 0x06003CD1 RID: 15569 RVA: 0x001811B0 File Offset: 0x0017F3B0
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.m_isHidden);
			this.m_isHidden.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x04003BA5 RID: 15269
		private readonly SynchronizedBool m_isHidden = new SynchronizedBool();

		// Token: 0x04003BA6 RID: 15270
		private float? m_nextAllowedVisibility;

		// Token: 0x04003BA7 RID: 15271
		[SerializeField]
		private MinMaxFloatRange m_visibilityCooldown = new MinMaxFloatRange(120f, 200f);

		// Token: 0x04003BA8 RID: 15272
		[SerializeField]
		private float m_nearbyRadiusCheck = 30f;

		// Token: 0x04003BA9 RID: 15273
		[SerializeField]
		private GameObject m_visuals;
	}
}
