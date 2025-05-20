using System;
using System.Collections.Generic;

namespace DataStructures.ViliWonka.Heap
{
	// Token: 0x02000161 RID: 353
	public class MinHeap<T> : MinHeap
	{
		// Token: 0x06000BFC RID: 3068 RVA: 0x0004AB9E File Offset: 0x00048D9E
		public MinHeap(int maxNodes = 2048) : base(maxNodes)
		{
			this.objs = new T[maxNodes + 1];
		}

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x06000BFD RID: 3069 RVA: 0x0004ABB5 File Offset: 0x00048DB5
		public T HeadHeapObject
		{
			get
			{
				return this.objs[1];
			}
		}

		// Token: 0x06000BFE RID: 3070 RVA: 0x000CEBC4 File Offset: 0x000CCDC4
		protected override void Swap(int A, int B)
		{
			this.tempHeap = this.heap[A];
			this.tempObjs = this.objs[A];
			this.heap[A] = this.heap[B];
			this.objs[A] = this.objs[B];
			this.heap[B] = this.tempHeap;
			this.objs[B] = this.tempObjs;
		}

		// Token: 0x06000BFF RID: 3071 RVA: 0x0004AA74 File Offset: 0x00048C74
		public override void PushValue(float h)
		{
			throw new ArgumentException("Use Push(T, float)!");
		}

		// Token: 0x06000C00 RID: 3072 RVA: 0x0004AA74 File Offset: 0x00048C74
		public override float PopValue()
		{
			throw new ArgumentException("Use Push(T, float)!");
		}

		// Token: 0x06000C01 RID: 3073 RVA: 0x000CEC3C File Offset: 0x000CCE3C
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

		// Token: 0x06000C02 RID: 3074 RVA: 0x000CEC98 File Offset: 0x000CCE98
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
			if (this.nodesCount != 0)
			{
				base.BubbleDownMin(1);
			}
			return result;
		}

		// Token: 0x06000C03 RID: 3075 RVA: 0x0004ABC3 File Offset: 0x00048DC3
		public T PopObj(ref float heapValue)
		{
			if (this.nodesCount == 0)
			{
				throw new ArgumentException("Heap is empty!");
			}
			heapValue = this.heap[1];
			return this.PopObj();
		}

		// Token: 0x06000C04 RID: 3076 RVA: 0x0004ABE8 File Offset: 0x00048DE8
		protected override void UpsizeHeap()
		{
			base.UpsizeHeap();
			Array.Resize<T>(ref this.objs, this.maxSize + 1);
		}

		// Token: 0x06000C05 RID: 3077 RVA: 0x000CED30 File Offset: 0x000CCF30
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

		// Token: 0x04000B60 RID: 2912
		private T[] objs;

		// Token: 0x04000B61 RID: 2913
		private T tempObjs;
	}
}
