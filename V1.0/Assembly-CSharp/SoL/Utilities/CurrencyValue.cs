using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x0200025C RID: 604
	[Serializable]
	public class CurrencyValue
	{
		// Token: 0x06001366 RID: 4966 RVA: 0x0004FB0E File Offset: 0x0004DD0E
		public ulong GetCurrency()
		{
			return new CurrencyConverter(this.m_copper, this.m_silver, (ulong)this.m_gold).TotalCurrency;
		}

		// Token: 0x04001B97 RID: 7063
		[SerializeField]
		private uint m_gold;

		// Token: 0x04001B98 RID: 7064
		[SerializeField]
		private uint m_silver;

		// Token: 0x04001B99 RID: 7065
		[SerializeField]
		private uint m_copper;
	}
}
