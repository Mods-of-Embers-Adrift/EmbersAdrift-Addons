using System;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000C8 RID: 200
	[Serializable]
	public struct RangeOfFloats
	{
		// Token: 0x06000758 RID: 1880 RVA: 0x0004802A File Offset: 0x0004622A
		public float Random()
		{
			return UnityEngine.Random.Range(this.Minimum, this.Maximum);
		}

		// Token: 0x06000759 RID: 1881 RVA: 0x0004803D File Offset: 0x0004623D
		public float Random(System.Random r)
		{
			return this.Minimum + (float)r.NextDouble() * (this.Maximum - this.Minimum);
		}

		// Token: 0x040008BB RID: 2235
		[Tooltip("Minimum value (inclusive)")]
		public float Minimum;

		// Token: 0x040008BC RID: 2236
		[Tooltip("Maximum value (inclusive)")]
		public float Maximum;
	}
}
