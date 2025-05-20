using System;
using UnityEngine;

namespace DataStructures.ViliWonka.KDTree
{
	// Token: 0x02000159 RID: 345
	public class KDQueryNode
	{
		// Token: 0x06000BB6 RID: 2998 RVA: 0x00044765 File Offset: 0x00042965
		public KDQueryNode()
		{
		}

		// Token: 0x06000BB7 RID: 2999 RVA: 0x0004A900 File Offset: 0x00048B00
		public KDQueryNode(KDNode node, Vector3 tempClosestPoint)
		{
			this.node = node;
			this.tempClosestPoint = tempClosestPoint;
		}

		// Token: 0x04000B4E RID: 2894
		public KDNode node;

		// Token: 0x04000B4F RID: 2895
		public Vector3 tempClosestPoint;

		// Token: 0x04000B50 RID: 2896
		public float distance;
	}
}
