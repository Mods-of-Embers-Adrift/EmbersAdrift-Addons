using System;

namespace DataStructures.ViliWonka.Heap
{
	// Token: 0x0200015E RID: 350
	public class MaxHeap : BaseHeap
	{
		// Token: 0x06000BEC RID: 3052 RVA: 0x0004AA36 File Offset: 0x00048C36
		public MaxHeap(int initialSize = 2048) : base(initialSize)
		{
		}

		// Token: 0x06000BED RID: 3053 RVA: 0x0004AAB1 File Offset: 0x00048CB1
		public override void PushValue(float h)
		{
			if (this.nodesCount == this.maxSize)
			{
				this.UpsizeHeap();
			}
			this.nodesCount++;
			this.heap[this.nodesCount] = h;
			base.BubbleUpMax(this.nodesCount);
		}

		// Token: 0x06000BEE RID: 3054 RVA: 0x000CE66C File Offset: 0x000CC86C
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
	}
}
