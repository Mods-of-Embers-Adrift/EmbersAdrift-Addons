using System;

namespace DataStructures.ViliWonka.KDTree
{
	// Token: 0x02000157 RID: 343
	public class KDNode
	{
		// Token: 0x1700036A RID: 874
		// (get) Token: 0x06000BA6 RID: 2982 RVA: 0x0004A850 File Offset: 0x00048A50
		public int Count
		{
			get
			{
				return this.end - this.start;
			}
		}

		// Token: 0x1700036B RID: 875
		// (get) Token: 0x06000BA7 RID: 2983 RVA: 0x0004A85F File Offset: 0x00048A5F
		public bool Leaf
		{
			get
			{
				return this.partitionAxis == -1;
			}
		}

		// Token: 0x04000B42 RID: 2882
		public float partitionCoordinate;

		// Token: 0x04000B43 RID: 2883
		public int partitionAxis = -1;

		// Token: 0x04000B44 RID: 2884
		public KDNode negativeChild;

		// Token: 0x04000B45 RID: 2885
		public KDNode positiveChild;

		// Token: 0x04000B46 RID: 2886
		public int start;

		// Token: 0x04000B47 RID: 2887
		public int end;

		// Token: 0x04000B48 RID: 2888
		public KDBounds bounds;
	}
}
