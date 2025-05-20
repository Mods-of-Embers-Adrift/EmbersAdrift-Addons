using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000AF RID: 175
	public class LightningBoltSegmentGroup
	{
		// Token: 0x17000279 RID: 633
		// (get) Token: 0x06000694 RID: 1684 RVA: 0x00047849 File Offset: 0x00045A49
		public int SegmentCount
		{
			get
			{
				return this.Segments.Count - this.StartIndex;
			}
		}

		// Token: 0x06000695 RID: 1685 RVA: 0x0004785D File Offset: 0x00045A5D
		public void Reset()
		{
			this.LightParameters = null;
			this.Segments.Clear();
			this.Lights.Clear();
			this.StartIndex = 0;
		}

		// Token: 0x040007C8 RID: 1992
		public float LineWidth;

		// Token: 0x040007C9 RID: 1993
		public int StartIndex;

		// Token: 0x040007CA RID: 1994
		public int Generation;

		// Token: 0x040007CB RID: 1995
		public float Delay;

		// Token: 0x040007CC RID: 1996
		public float PeakStart;

		// Token: 0x040007CD RID: 1997
		public float PeakEnd;

		// Token: 0x040007CE RID: 1998
		public float LifeTime;

		// Token: 0x040007CF RID: 1999
		public float EndWidthMultiplier;

		// Token: 0x040007D0 RID: 2000
		public Color32 Color;

		// Token: 0x040007D1 RID: 2001
		private const int kStartingListCapacity = 100;

		// Token: 0x040007D2 RID: 2002
		public readonly List<LightningBoltSegment> Segments = new List<LightningBoltSegment>(100);

		// Token: 0x040007D3 RID: 2003
		public readonly List<Light> Lights = new List<Light>(100);

		// Token: 0x040007D4 RID: 2004
		public LightningLightParameters LightParameters;
	}
}
