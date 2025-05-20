using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace DisruptorUnity3d
{
	// Token: 0x020001FB RID: 507
	public static class Volatile
	{
		// Token: 0x04000EDC RID: 3804
		private const int CacheLineSize = 64;

		// Token: 0x020001FC RID: 508
		[StructLayout(LayoutKind.Explicit, Size = 128)]
		public struct PaddedLong
		{
			// Token: 0x06001172 RID: 4466 RVA: 0x0004E73B File Offset: 0x0004C93B
			public PaddedLong(long value)
			{
				this._value = value;
			}

			// Token: 0x06001173 RID: 4467 RVA: 0x0004E744 File Offset: 0x0004C944
			public long ReadUnfenced()
			{
				return this._value;
			}

			// Token: 0x06001174 RID: 4468 RVA: 0x0004E74C File Offset: 0x0004C94C
			public long ReadAcquireFence()
			{
				long value = this._value;
				Thread.MemoryBarrier();
				return value;
			}

			// Token: 0x06001175 RID: 4469 RVA: 0x0004E759 File Offset: 0x0004C959
			public long ReadFullFence()
			{
				Thread.MemoryBarrier();
				return this._value;
			}

			// Token: 0x06001176 RID: 4470 RVA: 0x0004E744 File Offset: 0x0004C944
			[MethodImpl(MethodImplOptions.NoOptimization)]
			public long ReadCompilerOnlyFence()
			{
				return this._value;
			}

			// Token: 0x06001177 RID: 4471 RVA: 0x0004E766 File Offset: 0x0004C966
			public void WriteReleaseFence(long newValue)
			{
				Thread.MemoryBarrier();
				this._value = newValue;
			}

			// Token: 0x06001178 RID: 4472 RVA: 0x0004E766 File Offset: 0x0004C966
			public void WriteFullFence(long newValue)
			{
				Thread.MemoryBarrier();
				this._value = newValue;
			}

			// Token: 0x06001179 RID: 4473 RVA: 0x0004E73B File Offset: 0x0004C93B
			[MethodImpl(MethodImplOptions.NoOptimization)]
			public void WriteCompilerOnlyFence(long newValue)
			{
				this._value = newValue;
			}

			// Token: 0x0600117A RID: 4474 RVA: 0x0004E73B File Offset: 0x0004C93B
			public void WriteUnfenced(long newValue)
			{
				this._value = newValue;
			}

			// Token: 0x0600117B RID: 4475 RVA: 0x0004E774 File Offset: 0x0004C974
			public bool AtomicCompareExchange(long newValue, long comparand)
			{
				return Interlocked.CompareExchange(ref this._value, newValue, comparand) == comparand;
			}

			// Token: 0x0600117C RID: 4476 RVA: 0x0004E786 File Offset: 0x0004C986
			public long AtomicExchange(long newValue)
			{
				return Interlocked.Exchange(ref this._value, newValue);
			}

			// Token: 0x0600117D RID: 4477 RVA: 0x0004E794 File Offset: 0x0004C994
			public long AtomicAddAndGet(long delta)
			{
				return Interlocked.Add(ref this._value, delta);
			}

			// Token: 0x0600117E RID: 4478 RVA: 0x0004E7A2 File Offset: 0x0004C9A2
			public long AtomicIncrementAndGet()
			{
				return Interlocked.Increment(ref this._value);
			}

			// Token: 0x0600117F RID: 4479 RVA: 0x0004E7AF File Offset: 0x0004C9AF
			public long AtomicDecrementAndGet()
			{
				return Interlocked.Decrement(ref this._value);
			}

			// Token: 0x06001180 RID: 4480 RVA: 0x000E3BE0 File Offset: 0x000E1DE0
			public override string ToString()
			{
				return this.ReadFullFence().ToString();
			}

			// Token: 0x04000EDD RID: 3805
			[FieldOffset(64)]
			private long _value;
		}
	}
}
