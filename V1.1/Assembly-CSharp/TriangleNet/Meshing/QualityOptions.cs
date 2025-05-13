using System;
using TriangleNet.Geometry;

namespace TriangleNet.Meshing
{
	// Token: 0x02000123 RID: 291
	public class QualityOptions
	{
		// Token: 0x17000325 RID: 805
		// (get) Token: 0x06000A47 RID: 2631 RVA: 0x00049C37 File Offset: 0x00047E37
		// (set) Token: 0x06000A48 RID: 2632 RVA: 0x00049C3F File Offset: 0x00047E3F
		public double MaximumAngle { get; set; }

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x06000A49 RID: 2633 RVA: 0x00049C48 File Offset: 0x00047E48
		// (set) Token: 0x06000A4A RID: 2634 RVA: 0x00049C50 File Offset: 0x00047E50
		public double MinimumAngle { get; set; }

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x06000A4B RID: 2635 RVA: 0x00049C59 File Offset: 0x00047E59
		// (set) Token: 0x06000A4C RID: 2636 RVA: 0x00049C61 File Offset: 0x00047E61
		public double MaximumArea { get; set; }

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x06000A4D RID: 2637 RVA: 0x00049C6A File Offset: 0x00047E6A
		// (set) Token: 0x06000A4E RID: 2638 RVA: 0x00049C72 File Offset: 0x00047E72
		public Func<ITriangle, double, bool> UserTest { get; set; }

		// Token: 0x17000329 RID: 809
		// (get) Token: 0x06000A4F RID: 2639 RVA: 0x00049C7B File Offset: 0x00047E7B
		// (set) Token: 0x06000A50 RID: 2640 RVA: 0x00049C83 File Offset: 0x00047E83
		public bool VariableArea { get; set; }

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x06000A51 RID: 2641 RVA: 0x00049C8C File Offset: 0x00047E8C
		// (set) Token: 0x06000A52 RID: 2642 RVA: 0x00049C94 File Offset: 0x00047E94
		public int SteinerPoints { get; set; }
	}
}
