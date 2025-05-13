using System;
using SoL.Utilities;

namespace SoL.Game.GM
{
	// Token: 0x02000BE8 RID: 3048
	internal class DpsMeterInfo : IPoolable
	{
		// Token: 0x17001646 RID: 5702
		// (get) Token: 0x06005E43 RID: 24131 RVA: 0x0007F623 File Offset: 0x0007D823
		// (set) Token: 0x06005E44 RID: 24132 RVA: 0x0007F62B File Offset: 0x0007D82B
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

		// Token: 0x06005E45 RID: 24133 RVA: 0x0004475B File Offset: 0x0004295B
		public void Reset()
		{
		}

		// Token: 0x04005191 RID: 20881
		internal const int kValueCount = 7;

		// Token: 0x04005192 RID: 20882
		private bool m_inPool;
	}
}
