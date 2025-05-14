using System;
using System.Collections.Generic;
using SoL.Game;
using SoL.Game.Objects;
using SoL.Game.Objects.Containers;
using SoL.Game.Settings;
using SoL.Game.Transactions;
using SoL.Managers;
using SoL.Utilities.Logging;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x0200025D RID: 605
	public static class CurrencyDistributor
	{
		// Token: 0x06001368 RID: 4968 RVA: 0x000F6A20 File Offset: 0x000F4C20
		public static void DistributeCurrency(GameEntity target, string sourceName, int sourceLevel, string message, CurrencyContext context, MinMaxIntRange currencyRange, bool groupRaidSplit)
		{
			ulong num = (ulong)((long)currencyRange.RandomWithinRange());
			if (GlobalSettings.Values && GlobalSettings.Values.Player != null)
			{
				float droppedCurrencyMultiplier = GlobalSettings.Values.Player.GetDroppedCurrencyMultiplier(sourceLevel);
				if (droppedCurrencyMultiplier < 1f || droppedCurrencyMultiplier > 1f)
				{
					num = (ulong)((long)Mathf.FloorToInt(Mathf.Clamp(num * droppedCurrencyMultiplier, 0f, float.MaxValue)));
				}
			}
			CurrencyDistributor.DistributeCurrency(target, sourceName, sourceLevel, message, context, num, groupRaidSplit);
		}

		// Token: 0x06001369 RID: 4969 RVA: 0x000F6A98 File Offset: 0x000F4C98
		private static void DistributeCurrency(GameEntity target, string sourceName, int sourceLevel, string message, CurrencyContext context, ulong currency, bool groupRaidSplit)
		{
			if (currency <= 0UL || !target || !target.CharacterData || target.Type != GameEntityType.Player)
			{
				return;
			}
			ulong num = 0UL;
			bool flag = !target.IsMissingBag;
			CurrencyTransaction currencyTransaction = new CurrencyTransaction
			{
				Add = true,
				Amount = currency,
				TargetContainer = ContainerType.Inventory.ToString(),
				Message = message,
				Context = context
			};
			if (groupRaidSplit && !target.CharacterData.GroupId.IsEmpty && (target.CharacterData.NearbyGroupMembers.Count > 0 || (!target.CharacterData.RaidId.IsEmpty && target.CharacterData.NearbyRaidMembers.Count > 0)))
			{
				if (CurrencyDistributor.m_validMembers == null)
				{
					CurrencyDistributor.m_validMembers = new List<GameEntity>(6);
				}
				CurrencyDistributor.m_validMembers.Clear();
				List<GameEntity> list = (!target.CharacterData.RaidId.IsEmpty) ? target.CharacterData.NearbyRaidMembers : target.CharacterData.NearbyGroupMembers;
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] != null && !list[i].IsMissingBag)
					{
						CurrencyDistributor.m_validMembers.Add(list[i]);
					}
				}
				int num2 = flag ? 1 : 0;
				currency = (ulong)(currency / (double)(CurrencyDistributor.m_validMembers.Count + num2));
				if (currency <= 0UL)
				{
					currency = 1UL;
				}
				currencyTransaction.Amount = currency;
				currencyTransaction.Message += " (Group)";
				for (int j = 0; j < CurrencyDistributor.m_validMembers.Count; j++)
				{
					if (CurrencyDistributor.AddCurrencyToEntity(CurrencyDistributor.m_validMembers[j], ref currencyTransaction))
					{
						num += currencyTransaction.Amount;
					}
				}
			}
			if (flag && CurrencyDistributor.AddCurrencyToEntity(target, ref currencyTransaction))
			{
				num += currencyTransaction.Amount;
			}
			if (num > 0UL)
			{
				CurrencyDistributor.m_logArguments[0] = "CurrencyGenerated";
				CurrencyDistributor.m_logArguments[1] = num;
				CurrencyDistributor.m_logArguments[2] = sourceName;
				CurrencyDistributor.m_logArguments[3] = sourceLevel;
				CurrencyDistributor.m_logArguments[4] = LocalZoneManager.ZoneRecord.DisplayName;
				SolDebug.LogToIndex(LogLevel.Information, LogIndex.Economy, "{@EventType} {@Currency} generated from {@Object} {@Level} in {@Zone}", CurrencyDistributor.m_logArguments);
			}
		}

		// Token: 0x0600136A RID: 4970 RVA: 0x000F6D00 File Offset: 0x000F4F00
		public static bool AddCurrencyToEntity(GameEntity entity, ref CurrencyTransaction transaction)
		{
			if (entity != null)
			{
				ICollectionController collectionController = entity.CollectionController;
				if (((collectionController != null) ? collectionController.Inventory : null) != null && entity.NetworkEntity != null && entity.NetworkEntity.PlayerRpcHandler != null)
				{
					entity.CollectionController.Inventory.AddCurrency(transaction.Amount);
					entity.NetworkEntity.PlayerRpcHandler.ProcessCurrencyTransaction(transaction);
					return true;
				}
			}
			return false;
		}

		// Token: 0x04001B9A RID: 7066
		private static List<GameEntity> m_validMembers = null;

		// Token: 0x04001B9B RID: 7067
		private static readonly object[] m_logArguments = new object[5];
	}
}
