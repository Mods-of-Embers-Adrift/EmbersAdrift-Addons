using System;
using SoL.Game.Messages;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Transactions;
using SoL.Game.UI;
using SoL.Managers;
using SoL.Networking.Objects;
using SoL.UI;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Trading
{
	// Token: 0x02000647 RID: 1607
	public class ClientTradeManager
	{
		// Token: 0x17000A9E RID: 2718
		// (get) Token: 0x060031F8 RID: 12792 RVA: 0x000628F7 File Offset: 0x00060AF7
		public NetworkEntity TradePartner
		{
			get
			{
				if (this.m_currentTrade == null)
				{
					return null;
				}
				if (!(this.m_currentTrade.Source == LocalPlayer.NetworkEntity))
				{
					return this.m_currentTrade.Source;
				}
				return this.m_currentTrade.Target;
			}
		}

		// Token: 0x060031F9 RID: 12793 RVA: 0x00062931 File Offset: 0x00060B31
		private void ReturnTradeToPool()
		{
			if (this.m_currentTrade != null)
			{
				StaticPool<Trade>.ReturnToPool(this.m_currentTrade);
				this.m_currentTrade = null;
			}
		}

		// Token: 0x060031FA RID: 12794 RVA: 0x0015E6B0 File Offset: 0x0015C8B0
		private bool Eligible(NetworkEntity entity)
		{
			bool flag = entity == LocalPlayer.NetworkEntity;
			if (flag && SessionData.User != null && SessionData.User.IsTrial())
			{
				return false;
			}
			if (entity.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.MissingBag))
			{
				DialogOptions dialogOptions = new DialogOptions
				{
					Title = "Failure",
					Text = (flag ? "You do not have access to your inventory!  Find your bag first." : (entity.GameEntity.CharacterData.Name.Value + " does not have access to their inventory!  Help them find their bag first.")),
					ConfirmationText = "Ok"
				};
				DialogOptions opts = dialogOptions;
				ClientGameManager.UIManager.InformationDialog.Init(opts);
				return false;
			}
			if (entity.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.InTrade))
			{
				DialogOptions dialogOptions = new DialogOptions
				{
					Title = "Failure",
					Text = (flag ? "You are already in the midst of a trade transaction!" : (entity.GameEntity.CharacterData.Name.Value + " is already in the midst of a trade transaction!")),
					ConfirmationText = "Ok"
				};
				DialogOptions opts2 = dialogOptions;
				ClientGameManager.UIManager.InformationDialog.Init(opts2);
				return false;
			}
			if (entity.GameEntity.Vitals.GetCurrentHealthState() != HealthState.Alive)
			{
				DialogOptions dialogOptions = new DialogOptions
				{
					Title = "Failure",
					Text = (flag ? "You are not in a position to trade..." : (entity.GameEntity.CharacterData.Name.Value + " is not in a position to trade...")),
					ConfirmationText = "Ok"
				};
				DialogOptions opts3 = dialogOptions;
				ClientGameManager.UIManager.InformationDialog.Init(opts3);
				return false;
			}
			return true;
		}

		// Token: 0x060031FB RID: 12795 RVA: 0x0015E860 File Offset: 0x0015CA60
		public void Client_RequestTrade(NetworkEntity targetEntity)
		{
			if (!this.Eligible(LocalPlayer.NetworkEntity))
			{
				return;
			}
			if (!this.Eligible(targetEntity))
			{
				return;
			}
			Trade fromPool = StaticPool<Trade>.GetFromPool();
			fromPool.Init(LocalPlayer.NetworkEntity, targetEntity);
			this.m_currentTrade = fromPool;
			LocalPlayer.NetworkEntity.PlayerRpcHandler.Client_RequestTrade(fromPool.Id, fromPool.Target);
		}

		// Token: 0x060031FC RID: 12796 RVA: 0x0015E8BC File Offset: 0x0015CABC
		public void Client_RequestTrade_Response(UniqueId clientTradeId, UniqueId newTradeId)
		{
			if (clientTradeId == newTradeId || this.m_currentTrade == null)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Trade request failed.");
				this.ReturnTradeToPool();
				return;
			}
			MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Trade request sent.");
			this.m_currentTrade.Id = newTradeId;
			this.m_currentTrade.SourceStatus = TradeStatus.Requested;
			this.m_currentTrade.TargetStatus = TradeStatus.Requested;
		}

		// Token: 0x060031FD RID: 12797 RVA: 0x0015E928 File Offset: 0x0015CB28
		public void Client_IncomingTradeRequest(UniqueId tradeId, NetworkEntity sourceEntity)
		{
			if (sourceEntity == null || LocalPlayer.NetworkEntity == null)
			{
				return;
			}
			Trade fromPool = StaticPool<Trade>.GetFromPool();
			fromPool.Init(tradeId, sourceEntity, LocalPlayer.NetworkEntity);
			fromPool.SourceStatus = TradeStatus.Requested;
			fromPool.TargetStatus = TradeStatus.Requested;
			this.m_currentTrade = fromPool;
			DialogOptions opts = new DialogOptions
			{
				Title = "Trade Request",
				Text = this.m_currentTrade.Source.GameEntity.CharacterData.Name.Value + " has requested a trade.",
				ConfirmationText = "Accept",
				CancelText = "Decline",
				Callback = delegate(bool answer, object obj)
				{
					LocalPlayer.NetworkEntity.PlayerRpcHandler.Client_ProposedTrade_Response(tradeId, answer);
				}
			};
			ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
		}

		// Token: 0x060031FE RID: 12798 RVA: 0x0015EA08 File Offset: 0x0015CC08
		public void Client_CompleteTradeHandshake(UniqueId tradeId, bool proceed)
		{
			if (this.m_currentTrade == null)
			{
				Debug.LogError("m_currentTrade was null for TradeId: " + tradeId.ToString());
				return;
			}
			this.m_currentTrade.SourceStatus = TradeStatus.Pending;
			this.m_currentTrade.TargetStatus = TradeStatus.Pending;
			if (!proceed)
			{
				if (this.m_currentTrade.Source == LocalPlayer.NetworkEntity)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Trade declined.");
				}
				this.ReturnTradeToPool();
				return;
			}
			ClientGameManager.UIManager.Trade.Show(false);
		}

		// Token: 0x060031FF RID: 12799 RVA: 0x0015EA94 File Offset: 0x0015CC94
		public void Client_TradeTransactionConcluded(TakeAllResponse response, TradeCompletionCode code)
		{
			PlayerCollectionController playerCollectionController;
			if (!LocalPlayer.GameEntity.CollectionController.TryGetAsType(out playerCollectionController))
			{
				return;
			}
			TakeAllRequest request = default(TakeAllRequest);
			playerCollectionController.ProcessTakeAllResponse(request, response);
			ContainerInstance containerInstance;
			if (LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.TradeIncoming, out containerInstance))
			{
				containerInstance.DestroyContents();
			}
			ContainerInstance containerInstance2;
			if (LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.TradeOutgoing, out containerInstance2))
			{
				containerInstance2.DestroyContents();
			}
			this.ReturnTradeToPool();
			if (ClientGameManager.UIManager.Trade.Visible)
			{
				ClientGameManager.UIManager.Trade.Hide(false);
			}
			MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Trade result: " + code.ToString());
		}

		// Token: 0x06003200 RID: 12800 RVA: 0x0006294D File Offset: 0x00060B4D
		public void Client_AcceptTradeClicked()
		{
			if (this.m_currentTrade != null)
			{
				LocalPlayer.NetworkEntity.PlayerRpcHandler.Client_AcceptCancelTrade(this.m_currentTrade.Id, true);
			}
		}

		// Token: 0x06003201 RID: 12801 RVA: 0x00062972 File Offset: 0x00060B72
		public void Client_CancelTradeClicked()
		{
			if (this.m_currentTrade != null)
			{
				LocalPlayer.NetworkEntity.PlayerRpcHandler.Client_AcceptCancelTrade(this.m_currentTrade.Id, false);
			}
		}

		// Token: 0x06003202 RID: 12802 RVA: 0x00062997 File Offset: 0x00060B97
		public void Client_TradeAccepted(UniqueId tradeId, NetworkEntity entity)
		{
			ClientGameManager.UIManager.Trade.TradeAccepted(entity);
		}

		// Token: 0x06003203 RID: 12803 RVA: 0x0015EB48 File Offset: 0x0015CD48
		public void Client_ItemAdded(UniqueId tradeId, ArchetypeInstance instance)
		{
			ContainerInstance containerInstance;
			if (LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.TradeIncoming, out containerInstance))
			{
				instance.CreateItemInstanceUI();
				instance.InstanceUI.ToggleLock(true);
				containerInstance.Add(instance, true);
				ClientGameManager.UIManager.Trade.ResetTradeAccepted();
			}
		}

		// Token: 0x06003204 RID: 12804 RVA: 0x0015EB98 File Offset: 0x0015CD98
		public void Client_ItemRemoved(UniqueId tradeId, UniqueId instanceId)
		{
			ContainerInstance containerInstance;
			if (LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.TradeIncoming, out containerInstance))
			{
				containerInstance.RemoveAndDestroy(instanceId);
				ClientGameManager.UIManager.Trade.ResetTradeAccepted();
			}
		}

		// Token: 0x06003205 RID: 12805 RVA: 0x0015EBD4 File Offset: 0x0015CDD4
		public void Client_TradeItemsSwapped(UniqueId tradeId, UniqueId instanceIdA, UniqueId instanceIdB)
		{
			ContainerInstance containerInstance;
			ArchetypeInstance archetypeInstance;
			ArchetypeInstance archetypeInstance2;
			if (LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.TradeIncoming, out containerInstance) && containerInstance.TryGetInstanceForInstanceId(instanceIdA, out archetypeInstance) && containerInstance.TryGetInstanceForInstanceId(instanceIdB, out archetypeInstance2))
			{
				int index = archetypeInstance2.Index;
				int index2 = archetypeInstance.Index;
				archetypeInstance.Index = index;
				archetypeInstance2.Index = index2;
				if (archetypeInstance.InstanceUI && archetypeInstance2.InstanceUI)
				{
					ContainerSlotUI slotUI = archetypeInstance.InstanceUI.SlotUI;
					ContainerSlotUI slotUI2 = archetypeInstance2.InstanceUI.SlotUI;
					archetypeInstance.InstanceUI.AssignSlotUI(slotUI2);
					archetypeInstance2.InstanceUI.AssignSlotUI(slotUI);
				}
				ClientGameManager.UIManager.Trade.ResetTradeAccepted();
			}
		}

		// Token: 0x06003206 RID: 12806 RVA: 0x000629A9 File Offset: 0x00060BA9
		public void Client_ResetTradeAgreement()
		{
			if (this.m_currentTrade != null)
			{
				LocalPlayer.NetworkEntity.PlayerRpcHandler.Client_ResetTradeAgreement(this.m_currentTrade.Id);
			}
		}

		// Token: 0x06003207 RID: 12807 RVA: 0x0015EC90 File Offset: 0x0015CE90
		public void Client_CurrencyChanged(UniqueId tradeId, ulong newValue)
		{
			ContainerInstance containerInstance;
			if (LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.TradeIncoming, out containerInstance))
			{
				if (containerInstance.Currency > newValue)
				{
					ulong currency = containerInstance.Currency - newValue;
					containerInstance.RemoveCurrency(currency);
				}
				else
				{
					ulong currency2 = newValue - containerInstance.Currency;
					containerInstance.AddCurrency(currency2);
				}
				ClientGameManager.UIManager.Trade.ResetTradeAccepted();
			}
		}

		// Token: 0x06003208 RID: 12808 RVA: 0x0015ECEC File Offset: 0x0015CEEC
		public void Client_ItemCountChanged(UniqueId tradeId, UniqueId itemInstanceId, int newCount)
		{
			ContainerInstance containerInstance;
			ArchetypeInstance archetypeInstance;
			if (LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.TradeIncoming, out containerInstance) && containerInstance.TryGetInstanceForInstanceId(itemInstanceId, out archetypeInstance) && archetypeInstance.ItemData != null && archetypeInstance.ItemData.Count != null)
			{
				archetypeInstance.ItemData.Count = new int?(newCount);
			}
			ClientGameManager.UIManager.Trade.ResetTradeAccepted();
		}

		// Token: 0x040030AA RID: 12458
		private Trade m_currentTrade;
	}
}
