using System;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001CC RID: 460
	[RequireComponent(typeof(BGCcMath))]
	public abstract class BGCcWithMath : BGCc
	{
		// Token: 0x1700048C RID: 1164
		// (get) Token: 0x06001083 RID: 4227 RVA: 0x0004DCD5 File Offset: 0x0004BED5
		// (set) Token: 0x06001084 RID: 4228 RVA: 0x0004DCF7 File Offset: 0x0004BEF7
		public BGCcMath Math
		{
			get
			{
				if (this.math == null)
				{
					this.math = base.GetComponent<BGCcMath>();
				}
				return this.math;
			}
			set
			{
				if (value == null)
				{
					return;
				}
				this.math = value;
				base.SetParent(value);
			}
		}

		// Token: 0x1700048D RID: 1165
		// (get) Token: 0x06001085 RID: 4229 RVA: 0x0004DD11 File Offset: 0x0004BF11
		public override string Error
		{
			get
			{
				if (!(this.Math == null))
				{
					return null;
				}
				return "Math is null";
			}
		}

		// Token: 0x04000DCD RID: 3533
		private BGCcMath math;
	}
}
