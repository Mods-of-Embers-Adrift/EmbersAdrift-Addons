using System;
using System.Collections.Generic;

namespace DataStructures.ViliWonka.Heap
{
	// Token: 0x0200015B RID: 347
	public abstract class BaseHeap
	{
		// Token: 0x06000BCD RID: 3021 RVA: 0x0004A99C File Offset: 0x00048B9C
		protected BaseHeap(int initialSize)
		{
			this.maxSize = initialSize;
			this.heap = new float[initialSize + 1];
		}

		// Token: 0x17000371 RID: 881
		// (get) Token: 0x06000BCE RID: 3022 RVA: 0x0004A9B9 File Offset: 0x00048BB9
		public int Count
		{
			get
			{
				return this.nodesCount;
			}
		}

		// Token: 0x17000372 RID: 882
		// (get) Token: 0x06000BCF RID: 3023 RVA: 0x0004A9C1 File Offset: 0x00048BC1
		public float HeadValue
		{
			get
			{
				return this.heap[1];
			}
		}

		// Token: 0x06000BD0 RID: 3024 RVA: 0x0004A9CB File Offset: 0x00048BCB
		public void Clear()
		{
			this.nodesCount = 0;
		}

		// Token: 0x06000BD1 RID: 3025 RVA: 0x0004A9D4 File Offset: 0x00048BD4
		protected int Parent(int index)
		{
			return index >> 1;
		}

		// Token: 0x06000BD2 RID: 3026 RVA: 0x0004A9D9 File Offset: 0x00048BD9
		protected int Left(int index)
		{
			return index << 1;
		}

		// Token: 0x06000BD3 RID: 3027 RVA: 0x0004A9DE File Offset: 0x00048BDE
		protected int Right(int index)
		{
			return index << 1 | 1;
		}

		// Token: 0x06000BD4 RID: 3028 RVA: 0x000CE3D0 File Offset: 0x000CC5D0
		protected void BubbleDownMax(int index)
		{
			int num = this.Left(index);
			for (int i = this.Right(index); i <= this.nodesCount; i = this.Right(index))
			{
				if (this.heap[index] < this.heap[num])
				{
					if (this.heap[num] < this.heap[i])
					{
						this.Swap(index, i);
						index = i;
					}
					else
					{
						this.Swap(index, num);
						index = num;
					}
				}
				else
				{
					if (this.heap[index] >= this.heap[i])
					{
						index = num;
						num = this.Left(index);
						break;
					}
					this.Swap(index, i);
					index = i;
				}
				num = this.Left(index);
			}
			if (num <= this.nodesCount && this.heap[index] < this.heap[num])
			{
				this.Swap(index, num);
			}
		}

		// Token: 0x06000BD5 RID: 3029 RVA: 0x000CE498 File Offset: 0x000CC698
		protected void BubbleUpMax(int index)
		{
			int num = this.Parent(index);
			while (num > 0 && this.heap[num] < this.heap[index])
			{
				this.Swap(num, index);
				index = num;
				num = this.Parent(index);
			}
		}

		// Token: 0x06000BD6 RID: 3030 RVA: 0x000CE4D8 File Offset: 0x000CC6D8
		protected void BubbleDownMin(int index)
		{
			int num = this.Left(index);
			for (int i = this.Right(index); i <= this.nodesCount; i = this.Right(index))
			{
				if (this.heap[index] > this.heap[num])
				{
					if (this.heap[num] > this.heap[i])
					{
						this.Swap(index, i);
						index = i;
					}
					else
					{
						this.Swap(index, num);
						index = num;
					}
				}
				else
				{
					if (this.heap[index] <= this.heap[i])
					{
						index = num;
						num = this.Left(index);
						break;
					}
					this.Swap(index, i);
					index = i;
				}
				num = this.Left(index);
			}
			if (num <= this.nodesCount && this.heap[index] > this.heap[num])
			{
				this.Swap(index, num);
			}
		}

		// Token: 0x06000BD7 RID: 3031 RVA: 0x000CE5A0 File Offset: 0x000CC7A0
		protected void BubbleUpMin(int index)
		{
			int num = this.Parent(index);
			while (num > 0 && this.heap[num] > this.heap[index])
			{
				this.Swap(num, index);
				index = num;
				num = this.Parent(index);
			}
		}

		// Token: 0x06000BD8 RID: 3032 RVA: 0x0004A9E5 File Offset: 0x00048BE5
		protected virtual void Swap(int A, int B)
		{
			this.tempHeap = this.heap[A];
			this.heap[A] = this.heap[B];
			this.heap[B] = this.tempHeap;
		}

		// Token: 0x06000BD9 RID: 3033 RVA: 0x0004AA13 File Offset: 0x00048C13
		protected virtual void UpsizeHeap()
		{
			this.maxSize *= 2;
			Array.Resize<float>(ref this.heap, this.maxSize + 1);
		}

		// Token: 0x06000BDA RID: 3034 RVA: 0x00048A92 File Offset: 0x00046C92
		public virtual void PushValue(float h)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000BDB RID: 3035 RVA: 0x00048A92 File Offset: 0x00046C92
		public virtual float PopValue()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000BDC RID: 3036 RVA: 0x000CE5E0 File Offset: 0x000CC7E0
		public void FlushHeapResult(List<float> heapList)
		{
			for (int i = 1; i < this.Count; i++)
			{
				heapList.Add(this.heap[i]);
			}
		}

		// Token: 0x04000B58 RID: 2904
		protected int nodesCount;

		// Token: 0x04000B59 RID: 2905
		protected int maxSize;

		// Token: 0x04000B5A RID: 2906
		protected float[] heap;

		// Token: 0x04000B5B RID: 2907
		protected float tempHeap;
	}
}
