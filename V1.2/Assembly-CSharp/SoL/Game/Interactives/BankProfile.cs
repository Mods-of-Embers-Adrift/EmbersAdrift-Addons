using System;
using SoL.Game.Objects.Containers;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B76 RID: 2934
	[CreateAssetMenu(menuName = "SoL/Profiles/Bank")]
	public class BankProfile : ContainerProfile
	{
		// Token: 0x170014FF RID: 5375
		// (get) Token: 0x06005A46 RID: 23110 RVA: 0x0007C9AC File Offset: 0x0007ABAC
		private string GetPurchaseLabel
		{
			get
			{
				if (this.m_constraint != GridLayoutGroup.Constraint.FixedColumnCount)
				{
					return "Purchasable Columns";
				}
				return "Purchasable Rows";
			}
		}

		// Token: 0x17001500 RID: 5376
		// (get) Token: 0x06005A47 RID: 23111 RVA: 0x0007C9C2 File Offset: 0x0007ABC2
		public GridLayoutGroup.Constraint Constraint
		{
			get
			{
				return this.m_constraint;
			}
		}

		// Token: 0x17001501 RID: 5377
		// (get) Token: 0x06005A48 RID: 23112 RVA: 0x0007C9CA File Offset: 0x0007ABCA
		public int ConstraintCount
		{
			get
			{
				return this.m_constraintCount;
			}
		}

		// Token: 0x17001502 RID: 5378
		// (get) Token: 0x06005A49 RID: 23113 RVA: 0x0007C9D2 File Offset: 0x0007ABD2
		public bool IsPersonalBank
		{
			get
			{
				return this.m_isPersonalBank;
			}
		}

		// Token: 0x17001503 RID: 5379
		// (get) Token: 0x06005A4A RID: 23114 RVA: 0x0007C9DA File Offset: 0x0007ABDA
		public int RawMaxCapacity
		{
			get
			{
				return this.m_maxCapacity;
			}
		}

		// Token: 0x06005A4B RID: 23115 RVA: 0x001EC900 File Offset: 0x001EAB00
		public override int GetMaxCapacity(ContainerRecord containerRecord)
		{
			int result = this.m_maxCapacity;
			if (this.m_purchaseAmounts != null && this.m_purchaseAmounts.Length != 0)
			{
				int num = Mathf.FloorToInt((float)this.m_maxCapacity / (float)this.m_constraintCount);
				int num2 = this.m_purchaseAmounts.Length;
				int num3 = (containerRecord != null && containerRecord.ExpansionsPurchased != null) ? containerRecord.ExpansionsPurchased.Value : 0;
				result = Mathf.Clamp((num - num2 + num3) * this.m_constraintCount, 0, this.m_maxCapacity);
			}
			return result;
		}

		// Token: 0x06005A4C RID: 23116 RVA: 0x0007C9E2 File Offset: 0x0007ABE2
		public bool CanPurchaseMore(int currentPurchaseCount, out ulong costOfNextPurchase)
		{
			costOfNextPurchase = 0UL;
			if (this.m_purchaseAmounts == null || this.m_purchaseAmounts.Length == 0 || currentPurchaseCount >= this.m_purchaseAmounts.Length)
			{
				return false;
			}
			costOfNextPurchase = this.m_purchaseAmounts[currentPurchaseCount].GetCurrency();
			return true;
		}

		// Token: 0x06005A4D RID: 23117 RVA: 0x001EC97C File Offset: 0x001EAB7C
		private string GetLongDescription()
		{
			int num = Mathf.FloorToInt((float)this.m_maxCapacity / (float)this.m_constraintCount);
			string text = (this.m_constraint == GridLayoutGroup.Constraint.FixedColumnCount) ? "rows" : "columns";
			string text2 = (this.m_constraint == GridLayoutGroup.Constraint.FixedColumnCount) ? (this.m_constraintCount.ToString() + "x" + num.ToString()) : (num.ToString() + "x" + this.m_constraintCount.ToString());
			if (this.m_purchaseAmounts == null || this.m_purchaseAmounts.Length == 0)
			{
				return string.Concat(new string[]
				{
					text2,
					"\n",
					num.ToString(),
					" ",
					text
				});
			}
			int num2 = this.m_purchaseAmounts.Length;
			int num3 = num - num2;
			ulong num4 = 0UL;
			for (int i = 0; i < num2; i++)
			{
				num4 += this.m_purchaseAmounts[i].GetCurrency();
			}
			CurrencyConverter currencyConverter = new CurrencyConverter(num4);
			return string.Concat(new string[]
			{
				text2,
				"\n",
				num3.ToString(),
				" starting ",
				text,
				"\n",
				num2.ToString(),
				" purchasable ",
				text,
				"\nTotal Cost: ",
				currencyConverter.ToString()
			});
		}

		// Token: 0x06005A4E RID: 23118 RVA: 0x0007CA16 File Offset: 0x0007AC16
		private string GetShortDescription()
		{
			return "Info";
		}

		// Token: 0x04004F5F RID: 20319
		[SerializeField]
		private bool m_isPersonalBank;

		// Token: 0x04004F60 RID: 20320
		[SerializeField]
		private GridLayoutGroup.Constraint m_constraint;

		// Token: 0x04004F61 RID: 20321
		[SerializeField]
		private int m_constraintCount = 6;

		// Token: 0x04004F62 RID: 20322
		[SerializeField]
		private CurrencyValue[] m_purchaseAmounts;

		// Token: 0x04004F63 RID: 20323
		[SerializeField]
		private DummyClass m_dummy;
	}
}
