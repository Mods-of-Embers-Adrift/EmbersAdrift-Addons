using System;
using UnityEngine;

namespace SoL.Game.Objects
{
	// Token: 0x020009FE RID: 2558
	public class MinMaxFloatRangeSliderAttribute : PropertyAttribute
	{
		// Token: 0x06004DC2 RID: 19906 RVA: 0x00074A2E File Offset: 0x00072C2E
		public MinMaxFloatRangeSliderAttribute(float min, float max, float inputWidth = 50f)
		{
			this.min = min;
			this.max = max;
			this.inputWidth = inputWidth;
		}

		// Token: 0x0400473A RID: 18234
		public float min;

		// Token: 0x0400473B RID: 18235
		public float max;

		// Token: 0x0400473C RID: 18236
		public float inputWidth;
	}
}
