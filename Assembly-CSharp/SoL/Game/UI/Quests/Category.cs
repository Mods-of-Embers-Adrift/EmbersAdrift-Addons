using System;
using System.Collections.Generic;
using SoL.Game.Objects;

namespace SoL.Game.UI.Quests
{
	// Token: 0x02000944 RID: 2372
	public struct Category<TData> where TData : Identifiable
	{
		// Token: 0x0400423B RID: 16955
		public string Name;

		// Token: 0x0400423C RID: 16956
		public List<TData> Data;
	}
}
