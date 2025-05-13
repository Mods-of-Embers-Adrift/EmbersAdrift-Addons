using System;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000CA RID: 202
	public class SingleLineClampAttribute : SingleLineAttribute
	{
		// Token: 0x0600075D RID: 1885 RVA: 0x0004807B File Offset: 0x0004627B
		public SingleLineClampAttribute(string tooltip, double minValue, double maxValue) : base(tooltip)
		{
			this.MinValue = minValue;
			this.MaxValue = maxValue;
		}

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x0600075E RID: 1886 RVA: 0x00048092 File Offset: 0x00046292
		// (set) Token: 0x0600075F RID: 1887 RVA: 0x0004809A File Offset: 0x0004629A
		public double MinValue { get; private set; }

		// Token: 0x1700029F RID: 671
		// (get) Token: 0x06000760 RID: 1888 RVA: 0x000480A3 File Offset: 0x000462A3
		// (set) Token: 0x06000761 RID: 1889 RVA: 0x000480AB File Offset: 0x000462AB
		public double MaxValue { get; private set; }
	}
}
