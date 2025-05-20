using System;
using UnityEngine;

namespace SoL.Animation.Generator
{
	// Token: 0x02000235 RID: 565
	[Serializable]
	public struct IdleAnimationSet : IEquatable<IdleAnimationSet>
	{
		// Token: 0x060012D2 RID: 4818 RVA: 0x0004F681 File Offset: 0x0004D881
		public bool Equals(IdleAnimationSet other)
		{
			return this.GetHashCode() == other.GetHashCode();
		}

		// Token: 0x060012D3 RID: 4819 RVA: 0x0004F69E File Offset: 0x0004D89E
		public override bool Equals(object obj)
		{
			return obj != null && obj is IdleAnimationSet && this.Equals((IdleAnimationSet)obj);
		}

		// Token: 0x060012D4 RID: 4820 RVA: 0x000E7EFC File Offset: 0x000E60FC
		public override int GetHashCode()
		{
			return (((this.Idle != null) ? this.Idle.GetHashCode() : 0) * 397 ^ ((this.TurnLeft != null) ? this.TurnLeft.GetHashCode() : 0)) * 397 ^ ((this.TurnRight != null) ? this.TurnRight.GetHashCode() : 0);
		}

		// Token: 0x060012D5 RID: 4821 RVA: 0x0004F6BB File Offset: 0x0004D8BB
		public static bool operator ==(IdleAnimationSet x, IdleAnimationSet y)
		{
			return x.Equals(y);
		}

		// Token: 0x060012D6 RID: 4822 RVA: 0x0004F6C5 File Offset: 0x0004D8C5
		public static bool operator !=(IdleAnimationSet x, IdleAnimationSet y)
		{
			return !x.Equals(y);
		}

		// Token: 0x0400108F RID: 4239
		public Motion Idle;

		// Token: 0x04001090 RID: 4240
		public Motion TurnLeft;

		// Token: 0x04001091 RID: 4241
		public Motion TurnRight;

		// Token: 0x04001092 RID: 4242
		public static readonly IdleAnimationSet Empty;
	}
}
