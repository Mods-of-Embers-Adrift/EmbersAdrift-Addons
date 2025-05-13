using System;

namespace DataStructures.ViliWonka.Heap
{
	// Token: 0x02000160 RID: 352
	public class MinHeap : BaseHeap
	{
		// Token: 0x06000BF9 RID: 3065 RVA: 0x0004AA36 File Offset: 0x00048C36
		public MinHeap(int initialSize = 2048) : base(initialSize)
		{
		}

		// Token: 0x06000BFA RID: 3066 RVA: 0x0004AB60 File Offset: 0x00048D60
		public override void PushValue(float h)
		{
			if (this.nodesCount == this.maxSize)
			{
				this.UpsizeHeap();
			}
			this.nodesCount++;
			this.heap[this.nodesCount] = h;
			base.BubbleUpMin(this.nodesCount);
		}

		// Token: 0x06000BFB RID: 3067 RVA: 0x000CEB48 File Offset: 0x000CCD48
		public override float PopValue()
		{
			if (this.nodesCount == 0)
			{
				throw new ArgumentException("Heap is empty!");
			}
			float result = this.heap[1];
			this.heap[1] = this.heap[this.nodesCount];
			this.nodesCount--;
			if (this.nodesCount != 0)
			{
				base.BubbleDownMin(1);
			}
			return result;
		}
	}
}
