using System;

namespace SoL.Utilities
{
	// Token: 0x020002E6 RID: 742
	public interface IPoolable
	{
		// Token: 0x17000523 RID: 1315
		// (get) Token: 0x06001541 RID: 5441
		// (set) Token: 0x06001542 RID: 5442
		bool InPool { get; set; }

		// Token: 0x06001543 RID: 5443
		void Reset();
	}
}
