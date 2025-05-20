using System;
using System.Runtime.InteropServices;

namespace SoL.Networking
{
	// Token: 0x020003D5 RID: 981
	[StructLayout(LayoutKind.Explicit)]
	internal struct UIntFloat
	{
		// Token: 0x04002146 RID: 8518
		[FieldOffset(0)]
		public float floatValue;

		// Token: 0x04002147 RID: 8519
		[FieldOffset(0)]
		public uint uintValue;

		// Token: 0x04002148 RID: 8520
		[FieldOffset(0)]
		public double doubleValue;

		// Token: 0x04002149 RID: 8521
		[FieldOffset(0)]
		public ulong ulongValue;
	}
}
