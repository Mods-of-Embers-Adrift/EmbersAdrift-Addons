using System;
using SoL.Game.Objects.Containers;
using SoL.Game.Settings;
using SoL.Utilities;
using SoL.Utilities.Logging;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B89 RID: 2953
	public class InteractiveEssenceConverter : BaseNetworkInteractiveStation
	{
		// Token: 0x1700154C RID: 5452
		// (get) Token: 0x06005B0F RID: 23311 RVA: 0x0007D34F File Offset: 0x0007B54F
		protected override string m_tooltipText
		{
			get
			{
				return "Essence Converter";
			}
		}

		// Token: 0x1700154D RID: 5453
		// (get) Token: 0x06005B10 RID: 23312 RVA: 0x000701E6 File Offset: 0x0006E3E6
		protected override ContainerType m_containerType
		{
			get
			{
				return ContainerType.RuneCollector;
			}
		}

		// Token: 0x06005B11 RID: 23313 RVA: 0x001EE0C8 File Offset: 0x001EC2C8
		public void PurchaseRequest(GameEntity interactionSource, int purchaseAmount)
		{
			if (!base.EnabledForBranch())
			{
				this.SendErrorMessage(interactionSource, "Invalid merchant!");
				return;
			}
			if (!interactionSource || purchaseAmount <= 0)
			{
				this.SendErrorMessage(interactionSource, "Unknown Error!");
				return;
			}
			if (interactionSource.CollectionController == null || interactionSource.CollectionController.Record == null || interactionSource.CollectionController.Record.EmberStoneData == null)
			{
				this.SendErrorMessage(interactionSource, "Unknown Error!");
				return;
			}
			int num;
			ulong num2;
			GlobalSettings.Values.Ashen.GetTravelEssenceConversionValues(purchaseAmount, out num, out num2);
			CurrencySources currencySources;
			ulong availableCurrency = base.GetAvailableCurrency(interactionSource, out currencySources);
			if (num2 > availableCurrency)
			{
				this.SendErrorMessage(interactionSource, "Insufficient funds!");
				return;
			}
			if (interactionSource.CollectionController.Record.EmberStoneData.Count < num)
			{
				this.SendErrorMessage(interactionSource, "Not enough Ember Essence!");
				return;
			}
			if (!base.TryRemoveCurrency(interactionSource, num2))
			{
				this.SendErrorMessage(interactionSource, "Unknown error attempting to purchase Travel Essence!");
				return;
			}
			interactionSource.CollectionController.PurchaseTravelEssence(purchaseAmount, num);
			if (interactionSource.User != null && interactionSource.CollectionController != null && interactionSource.CollectionController.Record != null && interactionSource.CharacterData)
			{
				if (InteractiveEssenceConverter.m_converterArguments == null)
				{
					InteractiveEssenceConverter.m_converterArguments = new object[8];
				}
				InteractiveEssenceConverter.m_converterArguments[0] = "PurchaseTravelEssence";
				InteractiveEssenceConverter.m_converterArguments[1] = interactionSource.User.Id;
				InteractiveEssenceConverter.m_converterArguments[2] = interactionSource.CollectionController.Record.Id;
				InteractiveEssenceConverter.m_converterArguments[3] = interactionSource.CollectionController.Record.Name;
				InteractiveEssenceConverter.m_converterArguments[4] = interactionSource.CharacterData.AdventuringLevel;
				InteractiveEssenceConverter.m_converterArguments[5] = purchaseAmount;
				InteractiveEssenceConverter.m_converterArguments[6] = num;
				InteractiveEssenceConverter.m_converterArguments[7] = num2;
				SolDebug.LogToIndex(LogLevel.Information, LogIndex.Economy, "{@EventType}.{@UserId}.{@CharacterId}.{@PlayerName}.{@Level} has purchased {@TravelEssence} with {@EmberEssence} and {@Currency}", InteractiveEssenceConverter.m_converterArguments);
			}
		}

		// Token: 0x06005B12 RID: 23314 RVA: 0x0007D356 File Offset: 0x0007B556
		private void SendErrorMessage(GameEntity interactionSource, string msg)
		{
			if (interactionSource != null)
			{
				interactionSource.NetworkEntity.PlayerRpcHandler.SendChatNotification(msg);
			}
		}

		// Token: 0x04004FAC RID: 20396
		private static object[] m_converterArguments;
	}
}
