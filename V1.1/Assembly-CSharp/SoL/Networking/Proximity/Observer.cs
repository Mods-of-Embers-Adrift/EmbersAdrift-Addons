using System;
using SoL.Networking.Objects;
using SoL.Utilities;

namespace SoL.Networking.Proximity
{
	// Token: 0x020004AE RID: 1198
	public class Observer : IEquatable<Observer>, IPoolable
	{
		// Token: 0x0600216F RID: 8559 RVA: 0x0005835A File Offset: 0x0005655A
		public void Reset()
		{
			this.Id = -1;
			this.DistanceBand = null;
			this.Added = false;
			this.Remove = false;
			this.NetworkEntity = null;
		}

		// Token: 0x170006DC RID: 1756
		// (get) Token: 0x06002170 RID: 8560 RVA: 0x0005837F File Offset: 0x0005657F
		// (set) Token: 0x06002171 RID: 8561 RVA: 0x00058387 File Offset: 0x00056587
		bool IPoolable.InPool
		{
			get
			{
				return this.m_inPool;
			}
			set
			{
				this.m_inPool = value;
			}
		}

		// Token: 0x06002172 RID: 8562 RVA: 0x00058390 File Offset: 0x00056590
		public void PreRebuild()
		{
			this.Added = false;
			this.Remove = true;
		}

		// Token: 0x06002173 RID: 8563 RVA: 0x000583A0 File Offset: 0x000565A0
		public bool Equals(Observer other)
		{
			return this.Id == other.Id;
		}

		// Token: 0x06002174 RID: 8564 RVA: 0x000583B0 File Offset: 0x000565B0
		public override bool Equals(object obj)
		{
			return obj != null && obj is Observer && this.Equals((Observer)obj);
		}

		// Token: 0x06002175 RID: 8565 RVA: 0x000583CD File Offset: 0x000565CD
		public override int GetHashCode()
		{
			return this.Id;
		}

		// Token: 0x06002176 RID: 8566 RVA: 0x000583D5 File Offset: 0x000565D5
		public static bool operator ==(Observer a, Observer b)
		{
			return a.Equals(b);
		}

		// Token: 0x06002177 RID: 8567 RVA: 0x000583DE File Offset: 0x000565DE
		public static bool operator !=(Observer a, Observer b)
		{
			return !a.Equals(b);
		}

		// Token: 0x040025D8 RID: 9688
		public int Id;

		// Token: 0x040025D9 RID: 9689
		public DistanceBand DistanceBand;

		// Token: 0x040025DA RID: 9690
		public bool Added;

		// Token: 0x040025DB RID: 9691
		public bool Remove;

		// Token: 0x040025DC RID: 9692
		public NetworkEntity NetworkEntity;

		// Token: 0x040025DD RID: 9693
		private bool m_inPool;
	}
}
