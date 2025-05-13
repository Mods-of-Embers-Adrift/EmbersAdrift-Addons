using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Transactions;
using SoL.Networking;
using SoL.Networking.Objects;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Trading
{
	// Token: 0x02000649 RID: 1609
	public class ServerTradeManager : MonoBehaviour
	{
		// Token: 0x0600320C RID: 12812 RVA: 0x0015ED38 File Offset: 0x0015CF38
		private void Start()
		{
			this.m_timeOfNextMonitor = DateTime.UtcNow.AddSeconds(1.0);
		}

		// Token: 0x0600320D RID: 12813 RVA: 0x0015ED64 File Offset: 0x0015CF64
		private void Update()
		{
			DateTime utcNow = DateTime.UtcNow;
			if (utcNow < this.m_timeOfNextMonitor)
			{
				return;
			}
			this.m_timeOfNextMonitor = utcNow.AddSeconds(1.0);
			for (int i = 0; i < this.m_trades.Count; i++)
			{
				Trade trade = this.m_trades[i];
				if (trade != null)
				{
					this.UpdateTrade(trade);
					if (trade.Complete && this.m_trades.Remove(trade.Id))
					{
						StaticPool<Trade>.ReturnToPool(trade);
						i--;
					}
				}
			}
		}

		// Token: 0x0600320E RID: 12814 RVA: 0x0015EDF0 File Offset: 0x0015CFF0
		private void UpdateTrade(Trade trade)
		{
			if (trade.Complete)
			{
				return;
			}
			if (!(trade.Source != null) || !(trade.Target != null))
			{
				this.CancelTrade(trade, TradeCompletionCode.InvalidSourceOrTarget);
				return;
			}
			double totalSeconds = (DateTime.UtcNow - trade.Timestamp).TotalSeconds;
			if (trade.SourceStatus == TradeStatus.Requested && totalSeconds > 10.0)
			{
				this.CancelTrade(trade, TradeCompletionCode.Timeout);
				return;
			}
			if ((trade.Source.gameObject.transform.position - trade.Target.gameObject.transform.position).sqrMagnitude > 64f)
			{
				this.CancelTrade(trade, TradeCompletionCode.Distance);
				return;
			}
		}

		// Token: 0x0600320F RID: 12815 RVA: 0x000629E5 File Offset: 0x00060BE5
		private void ToggleTradeFlag(NetworkEntity entity, bool enabled)
		{
			if (enabled)
			{
				this.SetTradeFlag(entity);
				return;
			}
			this.UnsetTradeFlag(entity);
		}

		// Token: 0x06003210 RID: 12816 RVA: 0x0015EEAC File Offset: 0x0015D0AC
		private void SetTradeFlag(NetworkEntity entity)
		{
			if (entity && entity.GameEntity && entity.GameEntity.CharacterData)
			{
				entity.GameEntity.CharacterData.CharacterFlags.Value |= PlayerFlags.InTrade;
			}
		}

		// Token: 0x06003211 RID: 12817 RVA: 0x0015EF00 File Offset: 0x0015D100
		private void UnsetTradeFlag(NetworkEntity entity)
		{
			if (entity && entity.GameEntity && entity.GameEntity.CharacterData)
			{
				entity.GameEntity.CharacterData.CharacterFlags.Value &= ~PlayerFlags.InTrade;
			}
		}

		// Token: 0x06003212 RID: 12818 RVA: 0x000629F9 File Offset: 0x00060BF9
		private void CancelTrade(Trade trade, TradeCompletionCode tcc)
		{
			this.CancelTradeForEntity(trade.Id, trade.Source, tcc);
			this.CancelTradeForEntity(trade.Id, trade.Target, tcc);
			this.MarkTradeAsComplete(trade);
		}

		// Token: 0x06003213 RID: 12819 RVA: 0x0015EF54 File Offset: 0x0015D154
		private void CancelTradeForEntity(UniqueId tradeId, NetworkEntity entity, TradeCompletionCode tcc)
		{
			ContainerInstance containerInstance;
			ContainerInstance target;
			if (entity && entity.GameEntity && entity.GameEntity.CollectionController != null && entity.GameEntity.CollectionController.TryGetInstance(ContainerType.TradeOutgoing, out containerInstance) && entity.GameEntity.CollectionController.TryGetInstance(ContainerType.Inventory, out target))
			{
				TakeAllResponse response = containerInstance.MoveContentsToContainerInstance(target, true, true);
				response.Op = OpCodes.Ok;
				response.TransactionId = tradeId;
				response.SourceContainerId = ContainerType.TradeOutgoing.ToString();
				if (entity.PlayerRpcHandler)
				{
					entity.PlayerRpcHandler.Server_TradeTransactionConcluded(response, tcc);
				}
				entity.GameEntity.CollectionController.TradeId = null;
				this.UnsetTradeFlag(entity);
			}
		}

		// Token: 0x06003214 RID: 12820 RVA: 0x0015F028 File Offset: 0x0015D228
		private void CompleteTrade(Trade trade)
		{
			ContainerInstance containerInstance;
			ContainerInstance containerInstance2;
			ContainerInstance containerInstance3;
			ContainerInstance containerInstance4;
			if (trade.Source.GameEntity.CollectionController.TryGetInstance(ContainerType.TradeOutgoing, out containerInstance) && trade.Source.GameEntity.CollectionController.TryGetInstance(ContainerType.Inventory, out containerInstance2) && trade.Target.GameEntity.CollectionController.TryGetInstance(ContainerType.TradeOutgoing, out containerInstance3) && trade.Target.GameEntity.CollectionController.TryGetInstance(ContainerType.Inventory, out containerInstance4))
			{
				if (containerInstance4.Count + containerInstance.Count > containerInstance4.GetMaxCapacity() || containerInstance2.Count + containerInstance3.Count > containerInstance2.GetMaxCapacity())
				{
					this.CancelTrade(trade, TradeCompletionCode.Error);
					return;
				}
				TakeAllResponse response = containerInstance.MoveContentsToContainerInstance(containerInstance4, true, false);
				response.TransactionId = trade.Id;
				response.Op = OpCodes.Ok;
				response.SourceContainerId = ContainerType.TradeIncoming.ToString();
				trade.Target.PlayerRpcHandler.Server_TradeTransactionConcluded(response, TradeCompletionCode.Success);
				trade.Target.GameEntity.CollectionController.TradeId = null;
				this.UnsetTradeFlag(trade.Target);
				TakeAllResponse response2 = containerInstance3.MoveContentsToContainerInstance(containerInstance2, true, false);
				response2.TransactionId = trade.Id;
				response2.Op = OpCodes.Ok;
				response2.SourceContainerId = ContainerType.TradeIncoming.ToString();
				trade.Source.PlayerRpcHandler.Server_TradeTransactionConcluded(response2, TradeCompletionCode.Success);
				trade.Source.GameEntity.CollectionController.TradeId = null;
				this.UnsetTradeFlag(trade.Source);
			}
			this.MarkTradeAsComplete(trade);
		}

		// Token: 0x06003215 RID: 12821 RVA: 0x00062A28 File Offset: 0x00060C28
		private void MarkTradeAsComplete(Trade trade)
		{
			trade.Complete = true;
		}

		// Token: 0x06003216 RID: 12822 RVA: 0x0015F1D4 File Offset: 0x0015D3D4
		private bool EligibleForTrade(NetworkEntity entity)
		{
			return entity && entity.GameEntity && !entity.GameEntity.IsTrial && !entity.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.MissingBag) && !entity.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.InTrade) && entity.GameEntity.Vitals.GetCurrentHealthState() == HealthState.Alive;
		}

		// Token: 0x06003217 RID: 12823 RVA: 0x0015F258 File Offset: 0x0015D458
		public void Server_RequestTrade(NetworkEntity sourceEntity, NetworkEntity targetEntity, UniqueId clientTradeId)
		{
			if (sourceEntity == null || targetEntity == null)
			{
				if (sourceEntity != null)
				{
					sourceEntity.PlayerRpcHandler.Server_RequestTrade_Response(clientTradeId, clientTradeId);
				}
				return;
			}
			if (!this.EligibleForTrade(sourceEntity))
			{
				sourceEntity.PlayerRpcHandler.Server_RequestTrade_Response(clientTradeId, clientTradeId);
				return;
			}
			if (!this.EligibleForTrade(targetEntity))
			{
				sourceEntity.PlayerRpcHandler.Server_RequestTrade_Response(clientTradeId, clientTradeId);
				return;
			}
			Trade fromPool = StaticPool<Trade>.GetFromPool();
			fromPool.Init(sourceEntity, targetEntity);
			fromPool.SourceStatus = TradeStatus.Requested;
			fromPool.TargetStatus = TradeStatus.Requested;
			this.m_trades.Add(fromPool.Id, fromPool);
			this.SetTradeFlag(sourceEntity);
			sourceEntity.PlayerRpcHandler.Server_RequestTrade_Response(clientTradeId, fromPool.Id);
			targetEntity.PlayerRpcHandler.Server_RequestTrade(fromPool.Id, fromPool.Source);
		}

		// Token: 0x06003218 RID: 12824 RVA: 0x0015F31C File Offset: 0x0015D51C
		public void Server_ProposedTrade_Response(UniqueId tradeId, bool response)
		{
			Trade trade;
			if (!this.m_trades.TryGetValue(tradeId, out trade))
			{
				return;
			}
			if (trade.Source == null || trade.Target == null)
			{
				response = false;
			}
			if (trade.Source != null)
			{
				trade.Source.PlayerRpcHandler.Server_CompleteTradeHandshake(trade.Id, response);
				this.ToggleTradeFlag(trade.Source, response);
			}
			if (trade.Target != null)
			{
				trade.Target.PlayerRpcHandler.Server_CompleteTradeHandshake(trade.Id, response);
				this.ToggleTradeFlag(trade.Target, response);
			}
			if (response)
			{
				trade.SourceStatus = TradeStatus.Pending;
				trade.TargetStatus = TradeStatus.Pending;
				trade.Source.GameEntity.CollectionController.TradeId = new UniqueId?(tradeId);
				trade.Target.GameEntity.CollectionController.TradeId = new UniqueId?(tradeId);
				return;
			}
			this.MarkTradeAsComplete(trade);
		}

		// Token: 0x06003219 RID: 12825 RVA: 0x0015F40C File Offset: 0x0015D60C
		public void Server_ClientAcceptedTradeTerms(UniqueId tradeId, NetworkEntity acceptEntity)
		{
			Trade trade;
			if (this.m_trades.TryGetValue(tradeId, out trade))
			{
				if (trade.Source == acceptEntity)
				{
					trade.SourceStatus = TradeStatus.Accepted;
				}
				else if (trade.Target == acceptEntity)
				{
					trade.TargetStatus = TradeStatus.Accepted;
				}
				if (trade.SourceStatus == TradeStatus.Accepted && trade.TargetStatus == TradeStatus.Accepted)
				{
					this.CompleteTrade(trade);
					return;
				}
				trade.Source.PlayerRpcHandler.Server_TradeTermsAccepted(tradeId, acceptEntity);
				trade.Target.PlayerRpcHandler.Server_TradeTermsAccepted(tradeId, acceptEntity);
			}
		}

		// Token: 0x0600321A RID: 12826 RVA: 0x0015F494 File Offset: 0x0015D694
		public void Server_ClientCancelTrade(UniqueId tradeId, NetworkEntity cancelEntity)
		{
			Trade trade;
			if (this.m_trades.TryGetValue(tradeId, out trade))
			{
				this.CancelTrade(trade, TradeCompletionCode.Cancelled);
			}
		}

		// Token: 0x0600321B RID: 12827 RVA: 0x0015F4BC File Offset: 0x0015D6BC
		public void Server_ItemAdded(UniqueId tradeId, NetworkEntity entity, ArchetypeInstance instance)
		{
			Trade trade;
			if (this.m_trades.TryGetValue(tradeId, out trade))
			{
				((trade.Source == entity) ? trade.Target : trade.Source).PlayerRpcHandler.Server_TradeItemAdded(tradeId, instance);
				trade.SourceStatus = TradeStatus.Pending;
				trade.TargetStatus = TradeStatus.Pending;
			}
		}

		// Token: 0x0600321C RID: 12828 RVA: 0x0015F510 File Offset: 0x0015D710
		public void Server_ItemRemoved(UniqueId tradeId, NetworkEntity entity, UniqueId instanceId)
		{
			Trade trade;
			if (this.m_trades.TryGetValue(tradeId, out trade))
			{
				((trade.Source == entity) ? trade.Target : trade.Source).PlayerRpcHandler.Server_TradeItemRemoved(tradeId, instanceId);
				trade.SourceStatus = TradeStatus.Pending;
				trade.TargetStatus = TradeStatus.Pending;
			}
		}

		// Token: 0x0600321D RID: 12829 RVA: 0x0015F564 File Offset: 0x0015D764
		public void Server_ItemsSwapped(UniqueId tradeId, NetworkEntity entity, UniqueId instanceIdA, UniqueId instanceIdB)
		{
			Trade trade;
			if (this.m_trades.TryGetValue(tradeId, out trade))
			{
				((trade.Source == entity) ? trade.Target : trade.Source).PlayerRpcHandler.Server_TradeItemsSwapped(tradeId, instanceIdA, instanceIdB);
				trade.SourceStatus = TradeStatus.Pending;
				trade.TargetStatus = TradeStatus.Pending;
			}
		}

		// Token: 0x0600321E RID: 12830 RVA: 0x0015F5BC File Offset: 0x0015D7BC
		public void Server_ResetTradeAgreement(UniqueId tradeId, NetworkEntity entity)
		{
			Trade trade;
			if (this.m_trades.TryGetValue(tradeId, out trade))
			{
				((trade.Source == entity) ? trade.Target : trade.Source).PlayerRpcHandler.Server_ResetTradeAgreement(tradeId);
				trade.SourceStatus = TradeStatus.Pending;
				trade.TargetStatus = TradeStatus.Pending;
			}
		}

		// Token: 0x0600321F RID: 12831 RVA: 0x0015F610 File Offset: 0x0015D810
		public void Server_CurrencyChanged(UniqueId tradeId, NetworkEntity entity, ulong newValue)
		{
			Trade trade;
			if (this.m_trades.TryGetValue(tradeId, out trade))
			{
				((trade.Source == entity) ? trade.Target : trade.Source).PlayerRpcHandler.Server_CurrencyChanged(tradeId, newValue);
				trade.SourceStatus = TradeStatus.Pending;
				trade.TargetStatus = TradeStatus.Pending;
			}
		}

		// Token: 0x06003220 RID: 12832 RVA: 0x0015F664 File Offset: 0x0015D864
		public void Server_ItemCountChanged(UniqueId tradeId, NetworkEntity entity, UniqueId itemInstanceId, int newCount)
		{
			Trade trade;
			if (this.m_trades.TryGetValue(tradeId, out trade))
			{
				((trade.Source == entity) ? trade.Target : trade.Source).PlayerRpcHandler.Server_ItemCountChanged(tradeId, itemInstanceId, newCount);
				trade.SourceStatus = TradeStatus.Pending;
				trade.TargetStatus = TradeStatus.Pending;
			}
		}

		// Token: 0x040030AC RID: 12460
		private const float kTradeRequestTimeout = 10f;

		// Token: 0x040030AD RID: 12461
		private const float kTradeDistanceMax = 8f;

		// Token: 0x040030AE RID: 12462
		private const float kTradeDistanceMaxSqr = 64f;

		// Token: 0x040030AF RID: 12463
		private const float kMonitorUpdateRate = 1f;

		// Token: 0x040030B0 RID: 12464
		private readonly DictionaryList<UniqueId, Trade> m_trades = new DictionaryList<UniqueId, Trade>(default(UniqueIdComparer), false);

		// Token: 0x040030B1 RID: 12465
		private DateTime m_timeOfNextMonitor = DateTime.MinValue;
	}
}
