using System;
using System.Collections.Generic;
using SoL.Networking.Objects;
using UnityEngine;

namespace DataStructures.ViliWonka.KDTree
{
	// Token: 0x0200015A RID: 346
	public class KDTree
	{
		// Token: 0x1700036D RID: 877
		// (get) Token: 0x06000BB8 RID: 3000 RVA: 0x0004A916 File Offset: 0x00048B16
		// (set) Token: 0x06000BB9 RID: 3001 RVA: 0x0004A91E File Offset: 0x00048B1E
		public KDNode RootNode { get; private set; }

		// Token: 0x1700036E RID: 878
		// (get) Token: 0x06000BBA RID: 3002 RVA: 0x0004A927 File Offset: 0x00048B27
		public Vector3[] Points
		{
			get
			{
				return this.points;
			}
		}

		// Token: 0x1700036F RID: 879
		// (get) Token: 0x06000BBB RID: 3003 RVA: 0x0004A92F File Offset: 0x00048B2F
		public int[] Permutation
		{
			get
			{
				return this.permutation;
			}
		}

		// Token: 0x17000370 RID: 880
		// (get) Token: 0x06000BBC RID: 3004 RVA: 0x0004A937 File Offset: 0x00048B37
		// (set) Token: 0x06000BBD RID: 3005 RVA: 0x0004A93F File Offset: 0x00048B3F
		public int Count { get; private set; }

		// Token: 0x06000BBE RID: 3006 RVA: 0x000CD988 File Offset: 0x000CBB88
		public KDTree(int maxPointsPerLeafNode = 16)
		{
			this.Count = 0;
			this.points = new Vector3[0];
			this.permutation = new int[0];
			this.kdNodesStack = new KDNode[64];
			this.maxPointsPerLeafNode = maxPointsPerLeafNode;
		}

		// Token: 0x06000BBF RID: 3007 RVA: 0x000CD9D8 File Offset: 0x000CBBD8
		public KDTree(Vector3[] points, int maxPointsPerLeafNode = 16)
		{
			this.points = points;
			this.permutation = new int[points.Length];
			this.Count = points.Length;
			this.kdNodesStack = new KDNode[64];
			this.maxPointsPerLeafNode = maxPointsPerLeafNode;
			this.Rebuild(-1);
		}

		// Token: 0x06000BC0 RID: 3008 RVA: 0x000CDA2C File Offset: 0x000CBC2C
		public void Build(Vector3[] newPoints, int maxPointsPerLeafNode = -1)
		{
			this.SetCount(newPoints.Length);
			for (int i = 0; i < this.Count; i++)
			{
				this.points[i] = newPoints[i];
			}
			this.Rebuild(maxPointsPerLeafNode);
		}

		// Token: 0x06000BC1 RID: 3009 RVA: 0x000CDA70 File Offset: 0x000CBC70
		public void Build(List<Vector3> newPoints, int maxPointsPerLeafNode = -1)
		{
			this.SetCount(newPoints.Count);
			for (int i = 0; i < this.Count; i++)
			{
				this.points[i] = newPoints[i];
			}
			this.Rebuild(maxPointsPerLeafNode);
		}

		// Token: 0x06000BC2 RID: 3010 RVA: 0x000CDAB4 File Offset: 0x000CBCB4
		public void Rebuild(int maxPointsPerLeafNode = -1)
		{
			this.SetCount(this.Count);
			for (int i = 0; i < this.Count; i++)
			{
				this.permutation[i] = i;
			}
			if (maxPointsPerLeafNode > 0)
			{
				this.maxPointsPerLeafNode = maxPointsPerLeafNode;
			}
			this.BuildTree();
		}

		// Token: 0x06000BC3 RID: 3011 RVA: 0x000CDAF8 File Offset: 0x000CBCF8
		public void Rebuild(List<NetworkEntity> entities, int maxPointsPerLeafNode = -1)
		{
			this.SetCount(entities.Count);
			for (int i = 0; i < entities.Count; i++)
			{
				this.points[i] = entities[i].gameObject.transform.position;
			}
			this.Rebuild(maxPointsPerLeafNode);
		}

		// Token: 0x06000BC4 RID: 3012 RVA: 0x0004A948 File Offset: 0x00048B48
		public void SetCount(int newSize)
		{
			this.Count = newSize;
			if (this.Count > this.points.Length)
			{
				Array.Resize<Vector3>(ref this.points, this.Count);
				Array.Resize<int>(ref this.permutation, this.Count);
			}
		}

		// Token: 0x06000BC5 RID: 3013 RVA: 0x000CDB4C File Offset: 0x000CBD4C
		private void BuildTree()
		{
			this.ResetKDNodeStack();
			this.RootNode = this.GetKDNode();
			this.RootNode.bounds = this.MakeBounds();
			this.RootNode.start = 0;
			this.RootNode.end = this.Count;
			this.SplitNode(this.RootNode);
		}

		// Token: 0x06000BC6 RID: 3014 RVA: 0x000CDBA8 File Offset: 0x000CBDA8
		private KDNode GetKDNode()
		{
			KDNode kdnode;
			if (this.kdNodesCount < this.kdNodesStack.Length)
			{
				if (this.kdNodesStack[this.kdNodesCount] == null)
				{
					kdnode = (this.kdNodesStack[this.kdNodesCount] = new KDNode());
				}
				else
				{
					kdnode = this.kdNodesStack[this.kdNodesCount];
					kdnode.partitionAxis = -1;
				}
			}
			else
			{
				Array.Resize<KDNode>(ref this.kdNodesStack, this.kdNodesStack.Length * 2);
				kdnode = (this.kdNodesStack[this.kdNodesCount] = new KDNode());
			}
			this.kdNodesCount++;
			return kdnode;
		}

		// Token: 0x06000BC7 RID: 3015 RVA: 0x0004A983 File Offset: 0x00048B83
		private void ResetKDNodeStack()
		{
			this.kdNodesCount = 0;
		}

		// Token: 0x06000BC8 RID: 3016 RVA: 0x000CDC40 File Offset: 0x000CBE40
		private KDBounds MakeBounds()
		{
			Vector3 vector = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			Vector3 vector2 = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			int num = this.Count & -2;
			for (int i = 0; i < num; i += 2)
			{
				int num2 = i + 1;
				if (this.points[i].x > this.points[num2].x)
				{
					if (this.points[num2].x < vector2.x)
					{
						vector2.x = this.points[num2].x;
					}
					if (this.points[i].x > vector.x)
					{
						vector.x = this.points[i].x;
					}
				}
				else
				{
					if (this.points[i].x < vector2.x)
					{
						vector2.x = this.points[i].x;
					}
					if (this.points[num2].x > vector.x)
					{
						vector.x = this.points[num2].x;
					}
				}
				if (this.points[i].y > this.points[num2].y)
				{
					if (this.points[num2].y < vector2.y)
					{
						vector2.y = this.points[num2].y;
					}
					if (this.points[i].y > vector.y)
					{
						vector.y = this.points[i].y;
					}
				}
				else
				{
					if (this.points[i].y < vector2.y)
					{
						vector2.y = this.points[i].y;
					}
					if (this.points[num2].y > vector.y)
					{
						vector.y = this.points[num2].y;
					}
				}
				if (this.points[i].z > this.points[num2].z)
				{
					if (this.points[num2].z < vector2.z)
					{
						vector2.z = this.points[num2].z;
					}
					if (this.points[i].z > vector.z)
					{
						vector.z = this.points[i].z;
					}
				}
				else
				{
					if (this.points[i].z < vector2.z)
					{
						vector2.z = this.points[i].z;
					}
					if (this.points[num2].z > vector.z)
					{
						vector.z = this.points[num2].z;
					}
				}
			}
			if (num != this.Count)
			{
				if (vector2.x > this.points[num].x)
				{
					vector2.x = this.points[num].x;
				}
				if (vector.x < this.points[num].x)
				{
					vector.x = this.points[num].x;
				}
				if (vector2.y > this.points[num].y)
				{
					vector2.y = this.points[num].y;
				}
				if (vector.y < this.points[num].y)
				{
					vector.y = this.points[num].y;
				}
				if (vector2.z > this.points[num].z)
				{
					vector2.z = this.points[num].z;
				}
				if (vector.z < this.points[num].z)
				{
					vector.z = this.points[num].z;
				}
			}
			return new KDBounds
			{
				min = vector2,
				max = vector
			};
		}

		// Token: 0x06000BC9 RID: 3017 RVA: 0x000CE0D0 File Offset: 0x000CC2D0
		private void SplitNode(KDNode parent)
		{
			KDBounds bounds = parent.bounds;
			Vector3 size = bounds.size;
			int num = 0;
			float num2 = size.x;
			if (num2 < size.y)
			{
				num = 1;
				num2 = size.y;
			}
			if (num2 < size.z)
			{
				num = 2;
			}
			float boundsStart = bounds.min[num];
			float boundsEnd = bounds.max[num];
			float num3 = this.CalculatePivot(parent.start, parent.end, boundsStart, boundsEnd, num);
			parent.partitionAxis = num;
			parent.partitionCoordinate = num3;
			int num4 = this.Partition(parent.start, parent.end, num3, num);
			Vector3 max = bounds.max;
			max[num] = num3;
			KDNode kdnode = this.GetKDNode();
			kdnode.bounds = bounds;
			kdnode.bounds.max = max;
			kdnode.start = parent.start;
			kdnode.end = num4;
			parent.negativeChild = kdnode;
			Vector3 min = bounds.min;
			min[num] = num3;
			KDNode kdnode2 = this.GetKDNode();
			kdnode2.bounds = bounds;
			kdnode2.bounds.min = min;
			kdnode2.start = num4;
			kdnode2.end = parent.end;
			parent.positiveChild = kdnode2;
			if (kdnode.Count != 0 && kdnode2.Count != 0)
			{
				if (this.ContinueSplit(kdnode))
				{
					this.SplitNode(kdnode);
				}
				if (this.ContinueSplit(kdnode2))
				{
					this.SplitNode(kdnode2);
				}
			}
		}

		// Token: 0x06000BCA RID: 3018 RVA: 0x000CE244 File Offset: 0x000CC444
		private float CalculatePivot(int start, int end, float boundsStart, float boundsEnd, int axis)
		{
			float num = (boundsStart + boundsEnd) / 2f;
			bool flag = false;
			bool flag2 = false;
			float num2 = float.MinValue;
			float num3 = float.MaxValue;
			for (int i = start; i < end; i++)
			{
				if (this.points[this.permutation[i]][axis] < num)
				{
					flag = true;
				}
				else
				{
					flag2 = true;
				}
				if (flag && flag2)
				{
					return num;
				}
			}
			if (flag)
			{
				for (int j = start; j < end; j++)
				{
					if (num2 < this.points[this.permutation[j]][axis])
					{
						num2 = this.points[this.permutation[j]][axis];
					}
				}
				return num2;
			}
			for (int k = start; k < end; k++)
			{
				if (num3 > this.points[this.permutation[k]][axis])
				{
					num3 = this.points[this.permutation[k]][axis];
				}
			}
			return num3;
		}

		// Token: 0x06000BCB RID: 3019 RVA: 0x000CE348 File Offset: 0x000CC548
		private int Partition(int start, int end, float partitionPivot, int axis)
		{
			int num = start - 1;
			int num2 = end;
			for (;;)
			{
				num++;
				if (num >= num2 || this.points[this.permutation[num]][axis] >= partitionPivot)
				{
					do
					{
						num2--;
					}
					while (num < num2 && this.points[this.permutation[num2]][axis] >= partitionPivot);
					if (num >= num2)
					{
						break;
					}
					int num3 = this.permutation[num];
					this.permutation[num] = this.permutation[num2];
					this.permutation[num2] = num3;
				}
			}
			return num;
		}

		// Token: 0x06000BCC RID: 3020 RVA: 0x0004A98C File Offset: 0x00048B8C
		private bool ContinueSplit(KDNode node)
		{
			return node.Count > this.maxPointsPerLeafNode;
		}

		// Token: 0x04000B52 RID: 2898
		private Vector3[] points;

		// Token: 0x04000B53 RID: 2899
		private int[] permutation;

		// Token: 0x04000B55 RID: 2901
		private int maxPointsPerLeafNode = 32;

		// Token: 0x04000B56 RID: 2902
		private KDNode[] kdNodesStack;

		// Token: 0x04000B57 RID: 2903
		private int kdNodesCount;
	}
}
