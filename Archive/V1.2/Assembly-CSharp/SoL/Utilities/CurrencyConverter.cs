using System;
using Cysharp.Text;

namespace SoL.Utilities
{
	// Token: 0x0200025B RID: 603
	public struct CurrencyConverter : IEquatable<CurrencyConverter>
	{
		// Token: 0x0600135F RID: 4959 RVA: 0x000F688C File Offset: 0x000F4A8C
		public override string ToString()
		{
			string result = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				if (this.Gold > 0UL)
				{
					utf16ValueStringBuilder.AppendFormat<ulong>("{0}gp", this.Gold);
				}
				if (this.Silver > 0U)
				{
					string arg = (this.Gold > 0UL) ? " " : "";
					utf16ValueStringBuilder.AppendFormat<string, uint>("{0}{1}sp", arg, this.Silver);
				}
				if (this.Copper > 0U)
				{
					string arg2 = (this.Gold > 0UL || this.Silver > 0U) ? " " : "";
					utf16ValueStringBuilder.AppendFormat<string, uint>("{0}{1}cp", arg2, this.Copper);
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06001360 RID: 4960 RVA: 0x0004FAB2 File Offset: 0x0004DCB2
		public string GetFullValueString()
		{
			return ZString.Format<ulong, uint, uint>("{0}gp {1:00}sp {2:00}cp", this.Gold, this.Silver, this.Copper);
		}

		// Token: 0x06001361 RID: 4961 RVA: 0x000F6960 File Offset: 0x000F4B60
		public CurrencyConverter(ulong currency)
		{
			double num = currency / 10000.0;
			ulong num2 = (ulong)(num - num % 1.0);
			ulong num3 = currency - num2 * 10000UL;
			double num4 = num3 / 100.0;
			ulong num5 = (ulong)(num4 - num4 % 1.0);
			num3 -= num5 * 100UL;
			this.Copper = (uint)num3;
			this.Silver = (uint)num5;
			this.Gold = num2;
			this.TotalCurrency = currency;
		}

		// Token: 0x06001362 RID: 4962 RVA: 0x0004FAD0 File Offset: 0x0004DCD0
		public CurrencyConverter(uint copper, uint silver, ulong gold)
		{
			this.Copper = copper;
			this.Silver = silver;
			this.Gold = gold;
			this.TotalCurrency = (ulong)copper + (ulong)(silver * 100U) + gold * 10000UL;
		}

		// Token: 0x06001363 RID: 4963 RVA: 0x0004FAFE File Offset: 0x0004DCFE
		public bool Equals(CurrencyConverter other)
		{
			return this.TotalCurrency == other.TotalCurrency;
		}

		// Token: 0x06001364 RID: 4964 RVA: 0x000F69D8 File Offset: 0x000F4BD8
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is CurrencyConverter)
			{
				CurrencyConverter other = (CurrencyConverter)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06001365 RID: 4965 RVA: 0x000F6A04 File Offset: 0x000F4C04
		public override int GetHashCode()
		{
			return this.TotalCurrency.GetHashCode();
		}

		// Token: 0x04001B91 RID: 7057
		public const int kCopperPerGold = 10000;

		// Token: 0x04001B92 RID: 7058
		public const int kCopperPerSilver = 100;

		// Token: 0x04001B93 RID: 7059
		public readonly uint Copper;

		// Token: 0x04001B94 RID: 7060
		public readonly uint Silver;

		// Token: 0x04001B95 RID: 7061
		public readonly ulong Gold;

		// Token: 0x04001B96 RID: 7062
		public readonly ulong TotalCurrency;
	}
}
