using System;
using UnityEngine;

namespace SoL.Game.Objects
{
	// Token: 0x020009FF RID: 2559
	public class MinMaxIntRangeSliderAttribute : PropertyAttribute
	{
		// Token: 0x06004DC3 RID: 19907 RVA: 0x00074A4B File Offset: 0x00072C4B
		public MinMaxIntRangeSliderAttribute(int min, int max, float inputWidth = 50f)
		{
			this.min = min;
			this.max = max;
			this.inputWidth = inputWidth;
		}

		// Token: 0x0400473D RID: 18237
		public int min;

		// Token: 0x0400473E RID: 18238
		public int max;

		// Token: 0x0400473F RID: 18239
		public float inputWidth;
	}
}
