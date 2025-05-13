using System;
using UnityEngine;

namespace DataStructures.ViliWonka.KDTree
{
	// Token: 0x02000156 RID: 342
	public struct KDBounds
	{
		// Token: 0x17000368 RID: 872
		// (get) Token: 0x06000BA3 RID: 2979 RVA: 0x0004A80A File Offset: 0x00048A0A
		public Vector3 size
		{
			get
			{
				return this.max - this.min;
			}
		}

		// Token: 0x17000369 RID: 873
		// (get) Token: 0x06000BA4 RID: 2980 RVA: 0x0004A81D File Offset: 0x00048A1D
		public Bounds Bounds
		{
			get
			{
				return new Bounds((this.min + this.max) / 2f, this.max - this.min);
			}
		}

		// Token: 0x06000BA5 RID: 2981 RVA: 0x000CD000 File Offset: 0x000CB200
		public Vector3 ClosestPoint(Vector3 point)
		{
			return this.Bounds.ClosestPoint(point);
		}

		// Token: 0x04000B40 RID: 2880
		public Vector3 min;

		// Token: 0x04000B41 RID: 2881
		public Vector3 max;
	}
}
