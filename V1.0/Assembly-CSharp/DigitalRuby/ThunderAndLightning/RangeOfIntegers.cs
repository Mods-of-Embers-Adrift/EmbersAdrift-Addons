using System;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000C7 RID: 199
	[Serializable]
	public struct RangeOfIntegers
	{
		// Token: 0x06000756 RID: 1878 RVA: 0x00047FFF File Offset: 0x000461FF
		public int Random()
		{
			return UnityEngine.Random.Range(this.Minimum, this.Maximum + 1);
		}

		// Token: 0x06000757 RID: 1879 RVA: 0x00048014 File Offset: 0x00046214
		public int Random(System.Random r)
		{
			return r.Next(this.Minimum, this.Maximum + 1);
		}

		// Token: 0x040008B9 RID: 2233
		[Tooltip("Minimum value (inclusive)")]
		public int Minimum;

		// Token: 0x040008BA RID: 2234
		[Tooltip("Maximum value (inclusive)")]
		public int Maximum;
	}
}
