using System;
using System.Threading;

namespace DisruptorUnity3d
{
	// Token: 0x020001FA RID: 506
	public class RingBuffer<T>
	{
		// Token: 0x06001169 RID: 4457 RVA: 0x0004E6C3 File Offset: 0x0004C8C3
		public RingBuffer(int capacity)
		{
			capacity = RingBuffer<T>.NextPowerOfTwo(capacity);
			this._modMask = capacity - 1;
			this._entries = new T[capacity];
		}

		// Token: 0x1700049B RID: 1179
		// (get) Token: 0x0600116A RID: 4458 RVA: 0x0004E6E8 File Offset: 0x0004C8E8
		public int Capacity
		{
			get
			{
				return this._entries.Length;
			}
		}

		// Token: 0x1700049C RID: 1180
		public T this[long index]
		{
			get
			{
				return this._entries[(int)(checked((IntPtr)(index & unchecked((long)this._modMask))))];
			}
			set
			{
				this._entries[(int)(checked((IntPtr)(index & unchecked((long)this._modMask))))] = value;
			}
		}

		// Token: 0x0600116D RID: 4461 RVA: 0x000E3ADC File Offset: 0x000E1CDC
		public T Dequeue()
		{
			long num = this._consumerCursor.ReadAcquireFence() + 1L;
			while (this._producerCursor.ReadAcquireFence() < num)
			{
				Thread.SpinWait(1);
			}
			T result = this[num];
			this._consumerCursor.WriteReleaseFence(num);
			return result;
		}

		// Token: 0x0600116E RID: 4462 RVA: 0x000E3B24 File Offset: 0x000E1D24
		public bool TryDequeue(out T obj)
		{
			long num = this._consumerCursor.ReadAcquireFence() + 1L;
			if (this._producerCursor.ReadAcquireFence() < num)
			{
				obj = default(T);
				return false;
			}
			obj = this.Dequeue();
			return true;
		}

		// Token: 0x0600116F RID: 4463 RVA: 0x000E3B64 File Offset: 0x000E1D64
		public void Enqueue(T item)
		{
			long num = this._producerCursor.ReadAcquireFence() + 1L;
			long num2 = num - (long)this._entries.Length;
			long num3 = this._consumerCursor.ReadAcquireFence();
			while (num2 > num3)
			{
				num3 = this._consumerCursor.ReadAcquireFence();
				Thread.SpinWait(1);
			}
			this[num] = item;
			this._producerCursor.WriteReleaseFence(num);
		}

		// Token: 0x1700049D RID: 1181
		// (get) Token: 0x06001170 RID: 4464 RVA: 0x0004E721 File Offset: 0x0004C921
		public int Count
		{
			get
			{
				return (int)(this._producerCursor.ReadFullFence() - this._consumerCursor.ReadFullFence());
			}
		}

		// Token: 0x06001171 RID: 4465 RVA: 0x000E3BC4 File Offset: 0x000E1DC4
		private static int NextPowerOfTwo(int x)
		{
			int i;
			for (i = 2; i < x; i <<= 1)
			{
			}
			return i;
		}

		// Token: 0x04000ED8 RID: 3800
		private readonly T[] _entries;

		// Token: 0x04000ED9 RID: 3801
		private readonly int _modMask;

		// Token: 0x04000EDA RID: 3802
		private Volatile.PaddedLong _consumerCursor;

		// Token: 0x04000EDB RID: 3803
		private Volatile.PaddedLong _producerCursor;
	}
}
