using System;

namespace SoL.Utilities
{
	// Token: 0x0200025F RID: 607
	public static class CurrencySourcesExtensions
	{
		// Token: 0x0600136C RID: 4972 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this CurrencySources a, CurrencySources b)
		{
			return (a & b) == b;
		}
	}
}
