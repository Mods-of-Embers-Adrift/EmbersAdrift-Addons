using System;
using UnityEngine;

namespace SoL.Animation.Generator
{
	// Token: 0x02000236 RID: 566
	[Serializable]
	public struct LocomotionAnimationSet : IEquatable<LocomotionAnimationSet>
	{
		// Token: 0x060012D8 RID: 4824 RVA: 0x0004F6D2 File Offset: 0x0004D8D2
		public bool Equals(LocomotionAnimationSet other)
		{
			return this.GetHashCode() == other.GetHashCode();
		}

		// Token: 0x060012D9 RID: 4825 RVA: 0x0004F6EF File Offset: 0x0004D8EF
		public override bool Equals(object obj)
		{
			return obj != null && obj is LocomotionAnimationSet && this.Equals((LocomotionAnimationSet)obj);
		}

		// Token: 0x060012DA RID: 4826 RVA: 0x000E7F8C File Offset: 0x000E618C
		public override int GetHashCode()
		{
			return ((((((((this.Forward != null) ? this.Forward.GetHashCode() : 0) * 397 ^ ((this.ForwardLeft != null) ? this.ForwardLeft.GetHashCode() : 0)) * 397 ^ ((this.ForwardRight != null) ? this.ForwardRight.GetHashCode() : 0)) * 397 ^ ((this.Backward != null) ? this.Backward.GetHashCode() : 0)) * 397 ^ ((this.BackwardLeft != null) ? this.BackwardLeft.GetHashCode() : 0)) * 397 ^ ((this.BackwardRight != null) ? this.BackwardRight.GetHashCode() : 0)) * 397 ^ ((this.Left != null) ? this.Left.GetHashCode() : 0)) * 397 ^ ((this.Right != null) ? this.Right.GetHashCode() : 0);
		}

		// Token: 0x060012DB RID: 4827 RVA: 0x0004F70C File Offset: 0x0004D90C
		public static bool operator ==(LocomotionAnimationSet x, LocomotionAnimationSet y)
		{
			return x.Equals(y);
		}

		// Token: 0x060012DC RID: 4828 RVA: 0x0004F716 File Offset: 0x0004D916
		public static bool operator !=(LocomotionAnimationSet x, LocomotionAnimationSet y)
		{
			return !x.Equals(y);
		}

		// Token: 0x04001093 RID: 4243
		[Header("Forward")]
		public Motion Forward;

		// Token: 0x04001094 RID: 4244
		public Motion ForwardLeft;

		// Token: 0x04001095 RID: 4245
		public Motion ForwardRight;

		// Token: 0x04001096 RID: 4246
		[Header("Backward")]
		public Motion Backward;

		// Token: 0x04001097 RID: 4247
		public Motion BackwardLeft;

		// Token: 0x04001098 RID: 4248
		public Motion BackwardRight;

		// Token: 0x04001099 RID: 4249
		[Header("Strafe")]
		public Motion Left;

		// Token: 0x0400109A RID: 4250
		public Motion Right;

		// Token: 0x0400109B RID: 4251
		public static readonly LocomotionAnimationSet Empty;
	}
}
