using System;
using System.Collections.Generic;

namespace DataStructures.ViliWonka.Heap
{
	// Token: 0x0200015F RID: 351
	public class MaxHeap<T> : MaxHeap
	{
		// Token: 0x06000BEF RID: 3055 RVA: 0x0004AAEF File Offset: 0x00048CEF
		public MaxHeap(int maxNodes) : base(maxNodes)
		{
			this.objs = new T[maxNodes + 1];
		}

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x06000BF0 RID: 3056 RVA: 0x0004AB06 File Offset: 0x00048D06
		public T HeadHeapObject
		{
			get
			{
				return this.objs[1];
			}
		}

		// Token: 0x06000BF1 RID: 3057 RVA: 0x000CE988 File Offset: 0x000CCB88
		protected override void Swap(int A, int B)
		{
			this.tempHeap = this.heap[A];
			this.tempObjs = this.objs[A];
			this.heap[A] = this.heap[B];
			this.objs[A] = this.objs[B];
			this.heap[B] = this.tempHeap;
			this.objs[B] = this.tempObjs;
		}

		// Token: 0x06000BF2 RID: 3058 RVA: 0x0004AB14 File Offset: 0x00048D14
		public override void PushValue(float h)
		{
			throw new ArgumentException("Use PushObj(T, float)!");
		}

		// Token: 0x06000BF3 RID: 3059 RVA: 0x0004AA74 File Offset: 0x00048C74
		public override float PopValue()
		{
			throw new ArgumentException("Use Push(T, float)!");
		}

		// Token: 0x06000BF4 RID: 3060 RVA: 0x000CEA00 File Offset: 0x000CCC00
		public void PushObj(T obj, float h)
		{
			if (this.nodesCount == this.maxSize)
			{
				this.UpsizeHeap();
			}
			this.nodesCount++;
			this.heap[this.nodesCount] = h;
			this.objs[this.nodesCount] = obj;
			base.BubbleUpMin(this.nodesCount);
		}

		// Token: 0x06000BF5 RID: 3061 RVA: 0x000CEA5C File Offset: 0x000CCC5C
		public T PopObj()
		{
			if (this.nodesCount == 0)
			{
				throw new ArgumentException("Heap is empty!");
			}
			T result = this.objs[1];
			this.heap[1] = this.heap[this.nodesCount];
			this.objs[1] = this.objs[this.nodesCount];
			this.objs[this.nodesCount] = default(T);
			this.nodesCount--;
			base.BubbleDownMin(1);
			return result;
		}

		// Token: 0x06000BF6 RID: 3062 RVA: 0x0004AB20 File Offset: 0x00048D20
		public T PopObj(ref float heapValue)
		{
			if (this.nodesCount == 0)
			{
				throw new ArgumentException("Heap is empty!");
			}
			heapValue = this.heap[1];
			return this.PopObj();
		}

		// Token: 0x06000BF7 RID: 3063 RVA: 0x0004AB45 File Offset: 0x00048D45
		protected override void UpsizeHeap()
		{
			base.UpsizeHeap();
			Array.Resize<T>(ref this.objs, this.maxSize + 1);
		}

		// Token: 0x06000BF8 RID: 3064 RVA: 0x000CEAEC File Offset: 0x000CCCEC
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

		// Token: 0x04000B5E RID: 2910
		private T[] objs;

		// Token: 0x04000B5F RID: 2911
		private T tempObjs;
	}
}
