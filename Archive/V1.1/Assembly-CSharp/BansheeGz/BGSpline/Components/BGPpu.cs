using System;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001D3 RID: 467
	[Serializable]
	public struct BGPpu
	{
		// Token: 0x17000490 RID: 1168
		// (get) Token: 0x060010A6 RID: 4262 RVA: 0x0004DDF1 File Offset: 0x0004BFF1
		// (set) Token: 0x060010A7 RID: 4263 RVA: 0x0004DDF9 File Offset: 0x0004BFF9
		public int X
		{
			get
			{
				return this.x;
			}
			set
			{
				this.x = value;
			}
		}

		// Token: 0x17000491 RID: 1169
		// (get) Token: 0x060010A8 RID: 4264 RVA: 0x0004DE02 File Offset: 0x0004C002
		// (set) Token: 0x060010A9 RID: 4265 RVA: 0x0004DE0A File Offset: 0x0004C00A
		public int Y
		{
			get
			{
				return this.y;
			}
			set
			{
				this.y = value;
			}
		}

		// Token: 0x060010AA RID: 4266 RVA: 0x0004DE13 File Offset: 0x0004C013
		public BGPpu(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		// Token: 0x04000DE6 RID: 3558
		private int x;

		// Token: 0x04000DE7 RID: 3559
		private int y;
	}
}
