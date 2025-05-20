using System;
using UnityEngine;

namespace DataStructures.ViliWonka.Heap
{
	// Token: 0x0200015C RID: 348
	public class KSmallestHeap : BaseHeap
	{
		// Token: 0x06000BDD RID: 3037 RVA: 0x0004AA36 File Offset: 0x00048C36
		public KSmallestHeap(int maxEntries) : base(maxEntries)
		{
		}

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x06000BDE RID: 3038 RVA: 0x0004AA3F File Offset: 0x00048C3F
		public bool Full
		{
			get
			{
				return this.maxSize == this.nodesCount;
			}
		}

		// Token: 0x06000BDF RID: 3039 RVA: 0x000CE60C File Offset: 0x000CC80C
		public override void PushValue(float h)
		{
			if (this.nodesCount != this.maxSize)
			{
				this.nodesCount++;
				this.heap[this.nodesCount] = h;
				base.BubbleUpMax(this.nodesCount);
				return;
			}
			if (base.HeadValue < h)
			{
				return;
			}
			this.heap[1] = h;
			base.BubbleDownMax(1);
		}

		// Token: 0x06000BE0 RID: 3040 RVA: 0x000CE66C File Offset: 0x000CC86C
		public override float PopValue()
		{
			if (this.nodesCount == 0)
			{
				throw new ArgumentException("Heap is empty!");
			}
			float result = this.heap[1];
			this.heap[1] = this.heap[this.nodesCount];
			this.nodesCount--;
			base.BubbleDownMax(1);
			return result;
		}

		// Token: 0x06000BE1 RID: 3041 RVA: 0x000CE6C0 File Offset: 0x000CC8C0
		public void Print()
		{
			Debug.Log("HeapPropertyHolds? " + this.HeapPropertyHolds(1, 0).ToString());
		}

		// Token: 0x06000BE2 RID: 3042 RVA: 0x000CE6EC File Offset: 0x000CC8EC
		public bool HeapPropertyHolds(int index, int depth = 0)
		{
			if (index > this.nodesCount)
			{
				return true;
			}
			Debug.Log(this.heap[index]);
			int num = base.Left(index);
			int num2 = base.Right(index);
			bool flag = true;
			if (num <= this.nodesCount)
			{
				Debug.Log(this.heap[index].ToString() + " => " + this.heap[num].ToString());
				if (this.heap[index] < this.heap[num])
				{
					flag = false;
				}
			}
			if (num2 <= this.nodesCount)
			{
				Debug.Log(this.heap[index].ToString() + " => " + this.heap[num2].ToString());
				if (flag && this.heap[index] < this.heap[num2])
				{
					flag = false;
				}
			}
			return flag & this.HeapPropertyHolds(num, depth + 1) & this.HeapPropertyHolds(num2, depth + 1);
		}
	}
}
