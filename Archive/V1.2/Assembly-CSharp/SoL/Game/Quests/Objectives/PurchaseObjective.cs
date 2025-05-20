using System;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Quests.Objectives
{
	// Token: 0x020007B0 RID: 1968
	[CreateAssetMenu(menuName = "SoL/Quests/Objectives/PurchaseObjective")]
	public class PurchaseObjective : QuestObjective
	{
		// Token: 0x060039E0 RID: 14816 RVA: 0x00174C70 File Offset: 0x00172E70
		public override bool Validate(GameEntity entity, ObjectiveIterationCache cache, out string message)
		{
			if (cache.IterationsRequested > 1)
			{
				message = "Too many iterations requested.";
				return false;
			}
			if (!this.HasEnoughMoney(entity))
			{
				ulong currency = this.m_amountRequired.GetCurrency();
				if (entity.IsMissingBag && entity.CollectionController.Inventory.Currency >= currency)
				{
					message = "Insufficient funds without your bag! Try recovering your bag first.";
				}
				else
				{
					message = "Insufficient funds!";
				}
				return false;
			}
			message = string.Empty;
			return true;
		}

		// Token: 0x060039E1 RID: 14817 RVA: 0x00174CDC File Offset: 0x00172EDC
		public bool HasEnoughMoney(GameEntity entity)
		{
			CurrencySources currencySources;
			ulong availableCurrency = entity.GetAvailableCurrency(out currencySources, CurrencySources.Inventory | CurrencySources.PersonalBank);
			return availableCurrency > 0UL && availableCurrency >= this.m_amountRequired.GetCurrency();
		}

		// Token: 0x04003879 RID: 14457
		[SerializeField]
		private CurrencyValue m_amountRequired;
	}
}
