using System;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000B0 RID: 176
	public struct LightningBoltSegment
	{
		// Token: 0x06000697 RID: 1687 RVA: 0x000478A5 File Offset: 0x00045AA5
		public override string ToString()
		{
			return this.Start.ToString() + ", " + this.End.ToString();
		}

		// Token: 0x040007D5 RID: 2005
		public Vector3 Start;

		// Token: 0x040007D6 RID: 2006
		public Vector3 End;
	}
}
