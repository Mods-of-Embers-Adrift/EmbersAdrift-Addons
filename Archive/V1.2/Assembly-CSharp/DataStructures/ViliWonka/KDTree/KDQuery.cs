using System;
using System.Collections.Generic;
using DataStructures.ViliWonka.Heap;
using UnityEngine;

namespace DataStructures.ViliWonka.KDTree
{
	// Token: 0x02000158 RID: 344
	public class KDQuery
	{
		// Token: 0x06000BA9 RID: 2985 RVA: 0x000CD03C File Offset: 0x000CB23C
		private KDQueryNode PushGetQueue()
		{
			KDQueryNode result;
			if (this.count < this.queueArray.Length)
			{
				if (this.queueArray[this.count] == null)
				{
					result = (this.queueArray[this.count] = new KDQueryNode());
				}
				else
				{
					result = this.queueArray[this.count];
				}
			}
			else
			{
				Array.Resize<KDQueryNode>(ref this.queueArray, this.queueArray.Length * 2);
				result = (this.queueArray[this.count] = new KDQueryNode());
			}
			this.count++;
			return result;
		}

		// Token: 0x06000BAA RID: 2986 RVA: 0x0004A879 File Offset: 0x00048A79
		protected void PushToQueue(KDNode node, Vector3 tempClosestPoint)
		{
			KDQueryNode kdqueryNode = this.PushGetQueue();
			kdqueryNode.node = node;
			kdqueryNode.tempClosestPoint = tempClosestPoint;
		}

		// Token: 0x06000BAB RID: 2987 RVA: 0x000CD0CC File Offset: 0x000CB2CC
		protected void PushToHeap(KDNode node, Vector3 tempClosestPoint, Vector3 queryPosition)
		{
			KDQueryNode kdqueryNode = this.PushGetQueue();
			kdqueryNode.node = node;
			kdqueryNode.tempClosestPoint = tempClosestPoint;
			float num = Vector3.SqrMagnitude(tempClosestPoint - queryPosition);
			kdqueryNode.distance = num;
			this.minHeap.PushObj(kdqueryNode, num);
		}

		// Token: 0x1700036C RID: 876
		// (get) Token: 0x06000BAC RID: 2988 RVA: 0x0004A88E File Offset: 0x00048A8E
		protected int LeftToProcess
		{
			get
			{
				return this.count - this.queryIndex;
			}
		}

		// Token: 0x06000BAD RID: 2989 RVA: 0x0004A89D File Offset: 0x00048A9D
		protected KDQueryNode PopFromQueue()
		{
			KDQueryNode result = this.queueArray[this.queryIndex];
			this.queryIndex++;
			return result;
		}

		// Token: 0x06000BAE RID: 2990 RVA: 0x000CD110 File Offset: 0x000CB310
		protected KDQueryNode PopFromHeap()
		{
			KDQueryNode kdqueryNode = this.minHeap.PopObj();
			this.queueArray[this.queryIndex] = kdqueryNode;
			this.queryIndex++;
			return kdqueryNode;
		}

		// Token: 0x06000BAF RID: 2991 RVA: 0x0004A8BA File Offset: 0x00048ABA
		protected void Reset()
		{
			this.count = 0;
			this.queryIndex = 0;
			this.minHeap.Clear();
		}

		// Token: 0x06000BB0 RID: 2992 RVA: 0x0004A8D5 File Offset: 0x00048AD5
		public KDQuery(int queryNodesContainersInitialSize = 2048)
		{
			this.queueArray = new KDQueryNode[queryNodesContainersInitialSize];
			this.minHeap = new MinHeap<KDQueryNode>(queryNodesContainersInitialSize);
		}

		// Token: 0x06000BB1 RID: 2993 RVA: 0x000CD148 File Offset: 0x000CB348
		public void DrawLastQuery()
		{
			Color red = Color.red;
			Color green = Color.green;
			red.a = 0.25f;
			green.a = 0.25f;
			for (int i = 0; i < this.queryIndex; i++)
			{
				float t = (float)i / (float)this.queryIndex;
				Gizmos.color = Color.Lerp(green, red, t);
				Bounds bounds = this.queueArray[i].node.bounds.Bounds;
				Gizmos.DrawWireCube(bounds.center, bounds.size);
			}
		}

		// Token: 0x06000BB2 RID: 2994 RVA: 0x000CD1D0 File Offset: 0x000CB3D0
		public void ClosestPoint(KDTree tree, Vector3 queryPosition, List<int> resultIndices, List<float> resultDistances = null)
		{
			this.Reset();
			Vector3[] points = tree.Points;
			int[] permutation = tree.Permutation;
			int item = 0;
			float num = float.PositiveInfinity;
			KDNode rootNode = tree.RootNode;
			Vector3 tempClosestPoint = rootNode.bounds.ClosestPoint(queryPosition);
			this.PushToHeap(rootNode, tempClosestPoint, queryPosition);
			while (this.minHeap.Count > 0)
			{
				KDQueryNode kdqueryNode = this.PopFromHeap();
				if (kdqueryNode.distance <= num)
				{
					KDNode node = kdqueryNode.node;
					if (!node.Leaf)
					{
						int partitionAxis = node.partitionAxis;
						float partitionCoordinate = node.partitionCoordinate;
						Vector3 tempClosestPoint2 = kdqueryNode.tempClosestPoint;
						if (tempClosestPoint2[partitionAxis] - partitionCoordinate < 0f)
						{
							this.PushToHeap(node.negativeChild, tempClosestPoint2, queryPosition);
							tempClosestPoint2[partitionAxis] = partitionCoordinate;
							if (node.positiveChild.Count != 0)
							{
								this.PushToHeap(node.positiveChild, tempClosestPoint2, queryPosition);
							}
						}
						else
						{
							this.PushToHeap(node.positiveChild, tempClosestPoint2, queryPosition);
							tempClosestPoint2[partitionAxis] = partitionCoordinate;
							if (node.positiveChild.Count != 0)
							{
								this.PushToHeap(node.negativeChild, tempClosestPoint2, queryPosition);
							}
						}
					}
					else
					{
						for (int i = node.start; i < node.end; i++)
						{
							int num2 = permutation[i];
							float num3 = Vector3.SqrMagnitude(points[num2] - queryPosition);
							if (num3 <= num)
							{
								num = num3;
								item = num2;
							}
						}
					}
				}
			}
			resultIndices.Add(item);
			if (resultDistances != null)
			{
				resultDistances.Add(num);
			}
		}

		// Token: 0x06000BB3 RID: 2995 RVA: 0x000CD360 File Offset: 0x000CB560
		public void Interval(KDTree tree, Vector3 min, Vector3 max, List<int> resultIndices)
		{
			this.Reset();
			Vector3[] points = tree.Points;
			int[] permutation = tree.Permutation;
			KDNode rootNode = tree.RootNode;
			this.PushToQueue(rootNode, rootNode.bounds.ClosestPoint((min + max) / 2f));
			while (this.LeftToProcess > 0)
			{
				KDQueryNode kdqueryNode = this.PopFromQueue();
				KDNode node = kdqueryNode.node;
				if (!node.Leaf)
				{
					int partitionAxis = node.partitionAxis;
					float partitionCoordinate = node.partitionCoordinate;
					Vector3 tempClosestPoint = kdqueryNode.tempClosestPoint;
					if (tempClosestPoint[partitionAxis] - partitionCoordinate < 0f)
					{
						this.PushToQueue(node.negativeChild, tempClosestPoint);
						tempClosestPoint[partitionAxis] = partitionCoordinate;
						if (node.positiveChild.Count != 0 && tempClosestPoint[partitionAxis] <= max[partitionAxis])
						{
							this.PushToQueue(node.positiveChild, tempClosestPoint);
						}
					}
					else
					{
						this.PushToQueue(node.positiveChild, tempClosestPoint);
						tempClosestPoint[partitionAxis] = partitionCoordinate;
						if (node.negativeChild.Count != 0 && tempClosestPoint[partitionAxis] >= min[partitionAxis])
						{
							this.PushToQueue(node.negativeChild, tempClosestPoint);
						}
					}
				}
				else if (node.bounds.min[0] >= min[0] && node.bounds.min[1] >= min[1] && node.bounds.min[2] >= min[2] && node.bounds.max[0] <= max[0] && node.bounds.max[1] <= max[1] && node.bounds.max[2] <= max[2])
				{
					for (int i = node.start; i < node.end; i++)
					{
						resultIndices.Add(permutation[i]);
					}
				}
				else
				{
					for (int j = node.start; j < node.end; j++)
					{
						int num = permutation[j];
						Vector3 vector = points[num];
						if (vector[0] >= min[0] && vector[1] >= min[1] && vector[2] >= min[2] && vector[0] <= max[0] && vector[1] <= max[1] && vector[2] <= max[2])
						{
							resultIndices.Add(num);
						}
					}
				}
			}
		}

		// Token: 0x06000BB4 RID: 2996 RVA: 0x000CD63C File Offset: 0x000CB83C
		public void KNearest(KDTree tree, Vector3 queryPosition, int k, List<int> resultIndices, List<float> resultDistances = null)
		{
			KSmallestHeap<int> ksmallestHeap;
			this._heaps.TryGetValue(k, out ksmallestHeap);
			if (ksmallestHeap == null)
			{
				ksmallestHeap = new KSmallestHeap<int>(k);
				this._heaps.Add(k, ksmallestHeap);
			}
			ksmallestHeap.Clear();
			this.Reset();
			Vector3[] points = tree.Points;
			int[] permutation = tree.Permutation;
			float num = float.PositiveInfinity;
			KDNode rootNode = tree.RootNode;
			Vector3 tempClosestPoint = rootNode.bounds.ClosestPoint(queryPosition);
			this.PushToHeap(rootNode, tempClosestPoint, queryPosition);
			while (this.minHeap.Count > 0)
			{
				KDQueryNode kdqueryNode = this.PopFromHeap();
				if (kdqueryNode.distance <= num)
				{
					KDNode node = kdqueryNode.node;
					if (!node.Leaf)
					{
						int partitionAxis = node.partitionAxis;
						float partitionCoordinate = node.partitionCoordinate;
						Vector3 tempClosestPoint2 = kdqueryNode.tempClosestPoint;
						if (tempClosestPoint2[partitionAxis] - partitionCoordinate < 0f)
						{
							this.PushToHeap(node.negativeChild, tempClosestPoint2, queryPosition);
							tempClosestPoint2[partitionAxis] = partitionCoordinate;
							if (node.positiveChild.Count != 0)
							{
								this.PushToHeap(node.positiveChild, tempClosestPoint2, queryPosition);
							}
						}
						else
						{
							this.PushToHeap(node.positiveChild, tempClosestPoint2, queryPosition);
							tempClosestPoint2[partitionAxis] = partitionCoordinate;
							if (node.positiveChild.Count != 0)
							{
								this.PushToHeap(node.negativeChild, tempClosestPoint2, queryPosition);
							}
						}
					}
					else
					{
						for (int i = node.start; i < node.end; i++)
						{
							int num2 = permutation[i];
							float num3 = Vector3.SqrMagnitude(points[num2] - queryPosition);
							if (num3 <= num)
							{
								ksmallestHeap.PushObj(num2, num3);
								if (ksmallestHeap.Full)
								{
									num = ksmallestHeap.HeadValue;
								}
							}
						}
					}
				}
			}
			ksmallestHeap.FlushResult(resultIndices, resultDistances);
		}

		// Token: 0x06000BB5 RID: 2997 RVA: 0x000CD800 File Offset: 0x000CBA00
		public void Radius(KDTree tree, Vector3 queryPosition, float queryRadius, List<int> resultIndices)
		{
			this.Reset();
			Vector3[] points = tree.Points;
			int[] permutation = tree.Permutation;
			float num = queryRadius * queryRadius;
			KDNode rootNode = tree.RootNode;
			this.PushToQueue(rootNode, rootNode.bounds.ClosestPoint(queryPosition));
			while (this.LeftToProcess > 0)
			{
				KDQueryNode kdqueryNode = this.PopFromQueue();
				KDNode node = kdqueryNode.node;
				if (!node.Leaf)
				{
					int partitionAxis = node.partitionAxis;
					float partitionCoordinate = node.partitionCoordinate;
					Vector3 tempClosestPoint = kdqueryNode.tempClosestPoint;
					if (tempClosestPoint[partitionAxis] - partitionCoordinate < 0f)
					{
						this.PushToQueue(node.negativeChild, tempClosestPoint);
						tempClosestPoint[partitionAxis] = partitionCoordinate;
						float num2 = Vector3.SqrMagnitude(tempClosestPoint - queryPosition);
						if (node.positiveChild.Count != 0 && num2 <= num)
						{
							this.PushToQueue(node.positiveChild, tempClosestPoint);
						}
					}
					else
					{
						this.PushToQueue(node.positiveChild, tempClosestPoint);
						tempClosestPoint[partitionAxis] = partitionCoordinate;
						float num3 = Vector3.SqrMagnitude(tempClosestPoint - queryPosition);
						if (node.negativeChild.Count != 0 && num3 <= num)
						{
							this.PushToQueue(node.negativeChild, tempClosestPoint);
						}
					}
				}
				else
				{
					for (int i = node.start; i < node.end; i++)
					{
						int num4 = permutation[i];
						if (Vector3.SqrMagnitude(points[num4] - queryPosition) <= num)
						{
							resultIndices.Add(num4);
						}
					}
				}
			}
		}

		// Token: 0x04000B49 RID: 2889
		protected KDQueryNode[] queueArray;

		// Token: 0x04000B4A RID: 2890
		protected MinHeap<KDQueryNode> minHeap;

		// Token: 0x04000B4B RID: 2891
		protected int count;

		// Token: 0x04000B4C RID: 2892
		protected int queryIndex;

		// Token: 0x04000B4D RID: 2893
		private SortedList<int, KSmallestHeap<int>> _heaps = new SortedList<int, KSmallestHeap<int>>();
	}
}
