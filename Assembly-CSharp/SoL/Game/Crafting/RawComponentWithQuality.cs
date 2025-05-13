using System;
using SoL.Utilities;

namespace SoL.Game.Crafting
{
	// Token: 0x02000CE8 RID: 3304
	public class RawComponentWithQuality : IPoolable
	{
		// Token: 0x170017F8 RID: 6136
		// (get) Token: 0x06006408 RID: 25608 RVA: 0x000835B1 File Offset: 0x000817B1
		// (set) Token: 0x06006409 RID: 25609 RVA: 0x000835B9 File Offset: 0x000817B9
		public RawComponent RawComponent { get; set; }

		// Token: 0x170017F9 RID: 6137
		// (get) Token: 0x0600640A RID: 25610 RVA: 0x000835C2 File Offset: 0x000817C2
		// (set) Token: 0x0600640B RID: 25611 RVA: 0x000835CA File Offset: 0x000817CA
		public int Quality { get; set; }

		// Token: 0x0600640C RID: 25612 RVA: 0x000835D3 File Offset: 0x000817D3
		public void Reset()
		{
			this.RawComponent = null;
			this.Quality = 100;
		}

		// Token: 0x170017FA RID: 6138
		// (get) Token: 0x0600640D RID: 25613 RVA: 0x000835E4 File Offset: 0x000817E4
		// (set) Token: 0x0600640E RID: 25614 RVA: 0x000835EC File Offset: 0x000817EC
		bool IPoolable.InPool
		{
			get
			{
				return this.m_inPool;
			}
			set
			{
				this.m_inPool = value;
			}
		}

		// Token: 0x040056F7 RID: 22263
		private bool m_inPool;
	}
}
