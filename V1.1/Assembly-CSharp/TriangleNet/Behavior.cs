using System;
using TriangleNet.Geometry;

namespace TriangleNet
{
	// Token: 0x020000E1 RID: 225
	internal class Behavior
	{
		// Token: 0x0600080A RID: 2058 RVA: 0x0004860F File Offset: 0x0004680F
		public Behavior(bool quality = false, double minAngle = 20.0)
		{
			if (quality)
			{
				this.quality = true;
				this.minAngle = minAngle;
				this.Update();
			}
		}

		// Token: 0x0600080B RID: 2059 RVA: 0x000B071C File Offset: 0x000AE91C
		private void Update()
		{
			this.quality = true;
			if (this.minAngle < 0.0 || this.minAngle > 60.0)
			{
				this.minAngle = 0.0;
				this.quality = false;
				Log.Instance.Warning("Invalid quality option (minimum angle).", "Mesh.Behavior");
			}
			if (this.maxAngle != 0.0 && (this.maxAngle < 60.0 || this.maxAngle > 180.0))
			{
				this.maxAngle = 0.0;
				this.quality = false;
				Log.Instance.Warning("Invalid quality option (maximum angle).", "Mesh.Behavior");
			}
			this.useSegments = (this.Poly || this.Quality || this.Convex);
			this.goodAngle = Math.Cos(this.MinAngle * 3.141592653589793 / 180.0);
			this.maxGoodAngle = Math.Cos(this.MaxAngle * 3.141592653589793 / 180.0);
			if (this.goodAngle == 1.0)
			{
				this.offconstant = 0.0;
			}
			else
			{
				this.offconstant = 0.475 * Math.Sqrt((1.0 + this.goodAngle) / (1.0 - this.goodAngle));
			}
			this.goodAngle *= this.goodAngle;
		}

		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x0600080C RID: 2060 RVA: 0x0004864B File Offset: 0x0004684B
		// (set) Token: 0x0600080D RID: 2061 RVA: 0x00048652 File Offset: 0x00046852
		public static bool NoExact { get; set; }

		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x0600080E RID: 2062 RVA: 0x0004865A File Offset: 0x0004685A
		// (set) Token: 0x0600080F RID: 2063 RVA: 0x00048662 File Offset: 0x00046862
		public bool Quality
		{
			get
			{
				return this.quality;
			}
			set
			{
				this.quality = value;
				if (this.quality)
				{
					this.Update();
				}
			}
		}

		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x06000810 RID: 2064 RVA: 0x00048679 File Offset: 0x00046879
		// (set) Token: 0x06000811 RID: 2065 RVA: 0x00048681 File Offset: 0x00046881
		public double MinAngle
		{
			get
			{
				return this.minAngle;
			}
			set
			{
				this.minAngle = value;
				this.Update();
			}
		}

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x06000812 RID: 2066 RVA: 0x00048690 File Offset: 0x00046890
		// (set) Token: 0x06000813 RID: 2067 RVA: 0x00048698 File Offset: 0x00046898
		public double MaxAngle
		{
			get
			{
				return this.maxAngle;
			}
			set
			{
				this.maxAngle = value;
				this.Update();
			}
		}

		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x06000814 RID: 2068 RVA: 0x000486A7 File Offset: 0x000468A7
		// (set) Token: 0x06000815 RID: 2069 RVA: 0x000486AF File Offset: 0x000468AF
		public double MaxArea
		{
			get
			{
				return this.maxArea;
			}
			set
			{
				this.maxArea = value;
				this.fixedArea = (value > 0.0);
			}
		}

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x06000816 RID: 2070 RVA: 0x000486CA File Offset: 0x000468CA
		// (set) Token: 0x06000817 RID: 2071 RVA: 0x000486D2 File Offset: 0x000468D2
		public bool VarArea
		{
			get
			{
				return this.varArea;
			}
			set
			{
				this.varArea = value;
			}
		}

		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x06000818 RID: 2072 RVA: 0x000486DB File Offset: 0x000468DB
		// (set) Token: 0x06000819 RID: 2073 RVA: 0x000486E3 File Offset: 0x000468E3
		public bool Poly
		{
			get
			{
				return this.poly;
			}
			set
			{
				this.poly = value;
			}
		}

		// Token: 0x170002BA RID: 698
		// (get) Token: 0x0600081A RID: 2074 RVA: 0x000486EC File Offset: 0x000468EC
		// (set) Token: 0x0600081B RID: 2075 RVA: 0x000486F4 File Offset: 0x000468F4
		public Func<ITriangle, double, bool> UserTest
		{
			get
			{
				return this.usertest;
			}
			set
			{
				this.usertest = value;
			}
		}

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x0600081C RID: 2076 RVA: 0x000486FD File Offset: 0x000468FD
		// (set) Token: 0x0600081D RID: 2077 RVA: 0x00048705 File Offset: 0x00046905
		public bool Convex
		{
			get
			{
				return this.convex;
			}
			set
			{
				this.convex = value;
			}
		}

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x0600081E RID: 2078 RVA: 0x0004870E File Offset: 0x0004690E
		// (set) Token: 0x0600081F RID: 2079 RVA: 0x00048716 File Offset: 0x00046916
		public bool ConformingDelaunay
		{
			get
			{
				return this.conformDel;
			}
			set
			{
				this.conformDel = value;
			}
		}

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x06000820 RID: 2080 RVA: 0x0004871F File Offset: 0x0004691F
		// (set) Token: 0x06000821 RID: 2081 RVA: 0x00048727 File Offset: 0x00046927
		public int NoBisect
		{
			get
			{
				return this.noBisect;
			}
			set
			{
				this.noBisect = value;
				if (this.noBisect < 0 || this.noBisect > 2)
				{
					this.noBisect = 0;
				}
			}
		}

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x06000822 RID: 2082 RVA: 0x00048749 File Offset: 0x00046949
		// (set) Token: 0x06000823 RID: 2083 RVA: 0x00048751 File Offset: 0x00046951
		public bool UseBoundaryMarkers
		{
			get
			{
				return this.boundaryMarkers;
			}
			set
			{
				this.boundaryMarkers = value;
			}
		}

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x06000824 RID: 2084 RVA: 0x0004875A File Offset: 0x0004695A
		// (set) Token: 0x06000825 RID: 2085 RVA: 0x00048762 File Offset: 0x00046962
		public bool NoHoles
		{
			get
			{
				return this.noHoles;
			}
			set
			{
				this.noHoles = value;
			}
		}

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x06000826 RID: 2086 RVA: 0x0004876B File Offset: 0x0004696B
		// (set) Token: 0x06000827 RID: 2087 RVA: 0x00048773 File Offset: 0x00046973
		public bool Jettison
		{
			get
			{
				return this.jettison;
			}
			set
			{
				this.jettison = value;
			}
		}

		// Token: 0x04000958 RID: 2392
		private bool poly;

		// Token: 0x04000959 RID: 2393
		private bool quality;

		// Token: 0x0400095A RID: 2394
		private bool varArea;

		// Token: 0x0400095B RID: 2395
		private bool convex;

		// Token: 0x0400095C RID: 2396
		private bool jettison;

		// Token: 0x0400095D RID: 2397
		private bool boundaryMarkers = true;

		// Token: 0x0400095E RID: 2398
		private bool noHoles;

		// Token: 0x0400095F RID: 2399
		private bool conformDel;

		// Token: 0x04000960 RID: 2400
		private Func<ITriangle, double, bool> usertest;

		// Token: 0x04000961 RID: 2401
		private int noBisect;

		// Token: 0x04000962 RID: 2402
		private double minAngle;

		// Token: 0x04000963 RID: 2403
		private double maxAngle;

		// Token: 0x04000964 RID: 2404
		private double maxArea = -1.0;

		// Token: 0x04000965 RID: 2405
		internal bool fixedArea;

		// Token: 0x04000966 RID: 2406
		internal bool useSegments = true;

		// Token: 0x04000967 RID: 2407
		internal bool useRegions;

		// Token: 0x04000968 RID: 2408
		internal double goodAngle;

		// Token: 0x04000969 RID: 2409
		internal double maxGoodAngle;

		// Token: 0x0400096A RID: 2410
		internal double offconstant;
	}
}
