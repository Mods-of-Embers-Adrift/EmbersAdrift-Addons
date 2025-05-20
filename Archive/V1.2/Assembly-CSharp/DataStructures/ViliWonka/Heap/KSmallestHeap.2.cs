using System;
using System.Collections.Generic;

namespace DataStructures.ViliWonka.Heap
{
	// Token: 0x0200015D RID: 349
	public class KSmallestHeap<T> : KSmallestHeap
	{
		// Token: 0x06000BE3 RID: 3043 RVA: 0x0004AA4F File Offset: 0x00048C4F
		public KSmallestHeap(int maxEntries) : base(maxEntries)
		{
			this.objs = new T[maxEntries + 1];
		}

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x06000BE4 RID: 3044 RVA: 0x0004AA66 File Offset: 0x00048C66
		public T HeadHeapObject
		{
			get
			{
				return this.objs[1];
			}
		}

		// Token: 0x06000BE5 RID: 3045 RVA: 0x000CE7E0 File Offset: 0x000CC9E0
		protected override void Swap(int A, int B)
		{
			this.tempHeap = this.heap[A];
			this.tempObjs = this.objs[A];
			this.heap[A] = this.heap[B];
			this.objs[A] = this.objs[B];
			this.heap[B] = this.tempHeap;
			this.objs[B] = this.tempObjs;
		}

		// Token: 0x06000BE6 RID: 3046 RVA: 0x0004AA74 File Offset: 0x00048C74
		public override void PushValue(float h)
		{
			throw new ArgumentException("Use Push(T, float)!");
		}

		// Token: 0x06000BE7 RID: 3047 RVA: 0x000CE858 File Offset: 0x000CCA58
		public void PushObj(T obj, float h)
		{
			if (this.nodesCount != this.maxSize)
			{
				this.nodesCount++;
				this.heap[this.nodesCount] = h;
				this.objs[this.nodesCount] = obj;
				base.BubbleUpMax(this.nodesCount);
				return;
			}
			if (base.HeadValue < h)
			{
				return;
			}
			this.heap[1] = h;
			this.objs[1] = obj;
			base.BubbleDownMax(1);
		}

		// Token: 0x06000BE8 RID: 3048 RVA: 0x0004AA80 File Offset: 0x00048C80
		public override float PopValue()
		{
			throw new ArgumentException("Use PopObj()!");
		}

		// Token: 0x06000BE9 RID: 3049 RVA: 0x000CE8D8 File Offset: 0x000CCAD8
		public T PopObj()
		{
			if (this.nodesCount == 0)
			{
				throw new ArgumentException("Heap is empty!");
			}
			T result = this.objs[1];
			this.heap[1] = this.heap[this.nodesCount];
			this.objs[1] = this.objs[this.nodesCount];
			this.nodesCount--;
			base.BubbleDownMax(1);
			return result;
		}

		// Token: 0x06000BEA RID: 3050 RVA: 0x0004AA8C File Offset: 0x00048C8C
		public T PopObj(ref float heapValue)
		{
			if (this.nodesCount == 0)
			{
				throw new ArgumentException("Heap is empty!");
			}
			heapValue = this.heap[1];
			return this.PopObj();
		}

		// Token: 0x06000BEB RID: 3051 RVA: 0x000CE94C File Offset: 0x000CCB4C
		public void FlushResult(List<T> resultList, List<float> heapList = null)
		{
			int num = this.nodesCount + 1;
			if (heapList == null)
			{
				for (int i = 1; i < num; i++)
				{
					resultList.Add(this.PopObj());
				}
				return;
			}
			float item = 0f;
			for (int j = 1; j < num; j++)
			{
				resultList.Add(this.PopObj(ref item));
				heapList.Add(item);
			}
		}

		// Token: 0x04000B5C RID: 2908
		private T[] objs;

		// Token: 0x04000B5D RID: 2909
		private T tempObjs;
	}
}
