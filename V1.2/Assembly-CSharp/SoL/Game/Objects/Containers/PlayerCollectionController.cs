using System;
using System.Collections.Generic;
using Cysharp.Text;
using ENet;
using NetStack.Serialization;
using SoL.Game.Crafting;
using SoL.Game.HuntingLog;
using SoL.Game.Interactives;
using SoL.Game.Messages;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Player;
using SoL.Game.Quests.Objectives;
using SoL.Game.Settings;
using SoL.Game.Transactions;
using SoL.Game.UI;
using SoL.Managers;
using SoL.Networking;
using SoL.Networking.Database;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using SoL.Utilities.Logging;
using UnityEngine;

namespace SoL.Game.Objects.Containers
{
	// Token: 0x02000A1A RID: 2586
	public class PlayerCollectionController : BaseCollectionController
	{
		// Token: 0x17001173 RID: 4467
		// (get) Token: 0x06004F23 RID: 20259 RVA: 0x0004479C File Offset: 0x0004299C
		protected override GameEntityType EntityType
		{
			get
			{
				return GameEntityType.Player;
			}
		}

		// Token: 0x17001174 RID: 4468
		// (get) Token: 0x06004F24 RID: 20260 RVA: 0x000754BB File Offset: 0x000736BB
		public static HashSet<int> CachedIndexes
		{
			get
			{
				if (PlayerCollectionController.m_cachedIndexes == null)
				{
					PlayerCollectionController.m_cachedIndexes = new HashSet<int>(10);
				}
				return PlayerCollectionController.m_cachedIndexes;
			}
		}

		// Token: 0x14000103 RID: 259
		// (add) Token: 0x06004F25 RID: 20261 RVA: 0x001C4DA0 File Offset: 0x001C2FA0
		// (remove) Token: 0x06004F26 RID: 20262 RVA: 0x001C4DD4 File Offset: 0x001C2FD4
		public static event Action AbilityContentsChanged;

		// Token: 0x06004F27 RID: 20263 RVA: 0x001C4E08 File Offset: 0x001C3008
		protected override void OnDestroy()
		{
			if (GameManager.IsServer)
			{
				if (base.GameEntity && base.GameEntity.ServerPlayerController)
				{
					base.GameEntity.ServerPlayerController.FinalizeDbExternal();
				}
			}
			else if (this.m_subscribedToAbilityContentsChanged && this.m_abilities != null)
			{
				this.m_abilities.ContentsChanged -= this.LocalAbilitiesOnContentsChanged;
				this.m_subscribedToAbilityContentsChanged = false;
			}
			base.OnDestroy();
		}

		// Token: 0x17001175 RID: 4469
		// (get) Token: 0x06004F28 RID: 20264 RVA: 0x001C4E84 File Offset: 0x001C3084
		// (set) Token: 0x06004F29 RID: 20265 RVA: 0x001C4EF4 File Offset: 0x001C30F4
		public override EmberStone CurrentEmberStone
		{
			get
			{
				if (this.m_emberStone == null && base.Record != null && base.Record.EmberStoneData != null && !base.Record.EmberStoneData.StoneId.IsEmpty)
				{
					InternalGameDatabase.Archetypes.TryGetAsType<EmberStone>(base.Record.EmberStoneData.StoneId, out this.m_emberStone);
				}
				return this.m_emberStone;
			}
			set
			{
				if (value == this.CurrentEmberStone || base.Record == null)
				{
					return;
				}
				if (value == null)
				{
					this.m_emberStone = null;
					base.Record.EmberStoneData = null;
				}
				else
				{
					int value2 = 0;
					int value3 = 0;
					if (base.Record.EmberStoneData != null)
					{
						value2 = base.Record.EmberStoneData.Count;
						value3 = base.Record.EmberStoneData.TravelCount;
					}
					this.m_emberStone = value;
					base.Record.EmberStoneData = new EmberStoneInstanceData
					{
						StoneId = this.m_emberStone.Id,
						Count = Mathf.Clamp(value2, 0, this.m_emberStone.MaxCapacity),
						TravelCount = Mathf.Clamp(value3, 0, this.m_emberStone.MaxCapacity)
					};
				}
				base.InvokeEmberStoneChangedEvent();
			}
		}

		// Token: 0x06004F2A RID: 20266 RVA: 0x000754D5 File Offset: 0x000736D5
		public override int GetEmberEssenceCount()
		{
			if (base.Record == null || base.Record.EmberStoneData == null)
			{
				return 0;
			}
			return base.Record.EmberStoneData.Count;
		}

		// Token: 0x06004F2B RID: 20267 RVA: 0x0004475B File Offset: 0x0004295B
		public override void AdjustEmberEssenceCount(int delta)
		{
		}

		// Token: 0x06004F2C RID: 20268 RVA: 0x0004475B File Offset: 0x0004295B
		public override void AdjustTravelEssenceCount(int delta)
		{
		}

		// Token: 0x06004F2D RID: 20269 RVA: 0x001C4FCC File Offset: 0x001C31CC
		public void SetEmberEssenceCount(int updatedCount)
		{
			if (GameManager.IsServer || base.Record == null || base.Record.EmberStoneData == null || this.CurrentEmberStone == null)
			{
				return;
			}
			base.Record.EmberStoneData.Count = updatedCount;
			base.InvokeEmberStoneChangedEvent();
		}

		// Token: 0x06004F2E RID: 20270 RVA: 0x000754FE File Offset: 0x000736FE
		public override int GetAvailableEmberEssenceForTravel()
		{
			if (base.Record == null || base.Record.EmberStoneData == null)
			{
				return 0;
			}
			return base.Record.EmberStoneData.Count + base.Record.EmberStoneData.TravelCount;
		}

		// Token: 0x06004F2F RID: 20271 RVA: 0x00075538 File Offset: 0x00073738
		public override int GetDisplayValueForTravelEssence()
		{
			if (base.Record == null || base.Record.EmberStoneData == null)
			{
				return 0;
			}
			return base.Record.EmberStoneData.TravelCount;
		}

		// Token: 0x06004F30 RID: 20272 RVA: 0x001C501C File Offset: 0x001C321C
		public override ValueTuple<int, int> GetEmberAndTravelEssenceCounts()
		{
			if (base.Record == null || base.Record.EmberStoneData == null)
			{
				return new ValueTuple<int, int>(0, 0);
			}
			return new ValueTuple<int, int>(base.Record.EmberStoneData.Count, base.Record.EmberStoneData.TravelCount);
		}

		// Token: 0x06004F31 RID: 20273 RVA: 0x0004475B File Offset: 0x0004295B
		public override void PurchaseTravelEssence(int travelToAdd, int essenceCost)
		{
		}

		// Token: 0x06004F32 RID: 20274 RVA: 0x0004475B File Offset: 0x0004295B
		public void UseEssenceForTravel(int essenceCost, bool useTravelEssence)
		{
		}

		// Token: 0x06004F33 RID: 20275 RVA: 0x001C506C File Offset: 0x001C326C
		public override void SetEmberEssenceCountForTravel(int updatedCount, int updatedTravelCount)
		{
			if (GameManager.IsServer || base.Record == null || base.Record.EmberStoneData == null || this.CurrentEmberStone == null)
			{
				return;
			}
			base.Record.EmberStoneData.Count = updatedCount;
			base.Record.EmberStoneData.TravelCount = updatedTravelCount;
			base.InvokeEmberStoneChangedEvent();
		}

		// Token: 0x14000104 RID: 260
		// (add) Token: 0x06004F34 RID: 20276 RVA: 0x001C50CC File Offset: 0x001C32CC
		// (remove) Token: 0x06004F35 RID: 20277 RVA: 0x001C5100 File Offset: 0x001C3300
		public static event Action HuntingLogEntryRemoved;

		// Token: 0x14000105 RID: 261
		// (add) Token: 0x06004F36 RID: 20278 RVA: 0x001C5134 File Offset: 0x001C3334
		// (remove) Token: 0x06004F37 RID: 20279 RVA: 0x001C5168 File Offset: 0x001C3368
		public static event Action HuntingLogEntryAdded;

		// Token: 0x14000106 RID: 262
		// (add) Token: 0x06004F38 RID: 20280 RVA: 0x001C519C File Offset: 0x001C339C
		// (remove) Token: 0x06004F39 RID: 20281 RVA: 0x001C51D0 File Offset: 0x001C33D0
		public static event Action HuntingLogEntryModified;

		// Token: 0x06004F3A RID: 20282 RVA: 0x00075561 File Offset: 0x00073761
		public override void InvokeHuntingLogEntryRemoved()
		{
			base.InvokeHuntingLogEntryRemoved();
			Action huntingLogEntryRemoved = PlayerCollectionController.HuntingLogEntryRemoved;
			if (huntingLogEntryRemoved == null)
			{
				return;
			}
			huntingLogEntryRemoved();
		}

		// Token: 0x06004F3B RID: 20283 RVA: 0x00075578 File Offset: 0x00073778
		public override void InvokeHuntingLogEntryModified()
		{
			base.InvokeHuntingLogEntryModified();
			Action huntingLogEntryModified = PlayerCollectionController.HuntingLogEntryModified;
			if (huntingLogEntryModified == null)
			{
				return;
			}
			huntingLogEntryModified();
		}

		// Token: 0x06004F3C RID: 20284 RVA: 0x001C5204 File Offset: 0x001C3404
		public override bool IncrementHuntingLog(HuntingLogProfile profile, int npcLevel)
		{
			base.IncrementHuntingLog(profile, npcLevel);
			if (this.m_record == null || profile == null || !profile.ShowInLog)
			{
				return false;
			}
			if (GameManager.IsServer && LocalZoneManager.ZoneRecord != null && LocalZoneManager.ZoneRecord.ZoneId == 180 && base.GameEntity && base.GameEntity.CharacterData && base.GameEntity.CharacterData.AdventuringLevel > npcLevel && base.GameEntity.CharacterData.AdventuringLevel - npcLevel > 10)
			{
				if (this.m_excludeValleyCount <= 0 && base.GameEntity.NetworkEntity && base.GameEntity.NetworkEntity.PlayerRpcHandler)
				{
					if (string.IsNullOrEmpty(PlayerCollectionController.kExcludeMessage))
					{
						PlayerCollectionController.kExcludeMessage = ZString.Format<string>("Kills in {0} for higher level adventurers do not contribute to your hunting log!", LocalZoneManager.ZoneRecord.DisplayName);
					}
					base.GameEntity.NetworkEntity.PlayerRpcHandler.SendChatNotification(PlayerCollectionController.kExcludeMessage);
				}
				if (this.m_excludeValleyCount >= 20)
				{
					this.m_excludeValleyCount = 0;
				}
				else
				{
					this.m_excludeValleyCount++;
				}
				return false;
			}
			if (this.m_record.HuntingLog == null)
			{
				this.m_record.HuntingLogVersion = GlobalSettings.Values.HuntingLog.Version;
				this.m_record.HuntingLog = new Dictionary<UniqueId, HuntingLogEntry>(default(UniqueIdComparer));
			}
			int num = 0;
			HuntingLogEntry huntingLogEntry;
			if (this.m_record.HuntingLog.TryGetValue(profile.Id, out huntingLogEntry))
			{
				num = huntingLogEntry.PerkCount;
				huntingLogEntry.IncrementCount();
				Action huntingLogEntryModified = PlayerCollectionController.HuntingLogEntryModified;
				if (huntingLogEntryModified != null)
				{
					huntingLogEntryModified();
				}
			}
			else
			{
				huntingLogEntry = new HuntingLogEntry
				{
					ProfileId = profile.Id,
					Version = profile.Version,
					SettingsVersion = profile.SettingsVersion
				};
				huntingLogEntry.IncrementCount();
				this.m_record.HuntingLog.Add(profile.Id, huntingLogEntry);
				Action huntingLogEntryAdded = PlayerCollectionController.HuntingLogEntryAdded;
				if (huntingLogEntryAdded != null)
				{
					huntingLogEntryAdded();
				}
			}
			base.InvokeHuntingLogUpdatedEvent();
			if (!GameManager.IsServer && num != huntingLogEntry.PerkCount && base.GameEntity && base.GameEntity.NetworkEntity && base.GameEntity.NetworkEntity.IsLocal && profile.ShowInLog)
			{
				bool flag = false;
				string str;
				if (profile.TitleUnlocked(huntingLogEntry.PerkCount, out str))
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Title \"" + str + "\" unlocked!");
					TitleManager.InvokeTitlesChangedEvent();
					flag = true;
				}
				string text;
				if (profile.PerkUnlocked(huntingLogEntry.PerkCount, out text))
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, string.Concat(new string[]
					{
						"A new ",
						profile.TitlePrefix,
						" ",
						text,
						" Perk is available!"
					}));
					flag = true;
				}
				if (flag)
				{
					UIManager.InvokeTriggerControlPanelUsageHighlight(WindowToggler.WindowType.Log);
				}
			}
			return true;
		}

		// Token: 0x06004F3D RID: 20285 RVA: 0x001C5510 File Offset: 0x001C3710
		protected override void ValidateContainers(CharacterRecord record)
		{
			if (record == null || record.Storage == null)
			{
				return;
			}
			for (int i = 0; i < ContainerTypeExtensions.ContainerTypesToValidate.Length; i++)
			{
				ContainerType containerType = ContainerTypeExtensions.ContainerTypesToValidate[i];
				if (!record.Storage.ContainsKey(containerType))
				{
					ContainerRecord value = new ContainerRecord
					{
						Id = containerType.ToString(),
						Type = containerType,
						Instances = new List<ArchetypeInstance>()
					};
					record.Storage.Add(containerType, value);
				}
			}
		}

		// Token: 0x06004F3E RID: 20286 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void ValidateContainerContents(CharacterRecord record)
		{
		}

		// Token: 0x06004F3F RID: 20287 RVA: 0x001C558C File Offset: 0x001C378C
		public static void AddInstanceToInvalid(CharacterRecord record, ArchetypeInstance instance, ContainerType containerType, string eventType, bool updateRecord = false)
		{
			if (record == null || instance == null)
			{
				return;
			}
			if (record.InvalidItems == null)
			{
				record.InvalidItems = new List<ArchetypeInstance>(10);
			}
			record.InvalidItems.Add(instance);
			if (updateRecord)
			{
				record.UpdateInvalidItems(ExternalGameDatabase.Database);
			}
			string text = containerType.ToString();
			BaseCollectionController.InvalidInstanceArguments[0] = record.Name;
			BaseCollectionController.InvalidInstanceArguments[1] = record.Id;
			BaseCollectionController.InvalidInstanceArguments[2] = instance.ArchetypeId.Value;
			BaseCollectionController.InvalidInstanceArguments[3] = text;
			BaseCollectionController.InvalidInstanceArguments[4] = eventType;
			SolDebug.LogToIndex(LogLevel.Error, LogIndex.Error, "{@CharacterName} ({@CharacterId}) had an {@EventType} ({@ItemId}) in {@ContainerType}!", BaseCollectionController.InvalidInstanceArguments);
		}

		// Token: 0x06004F40 RID: 20288 RVA: 0x001C562C File Offset: 0x001C382C
		private void AddInstanceToLostAndFound(CharacterRecord record, ArchetypeInstance instance, ContainerType containerType, ContainerRecord lostAndFoundRecord)
		{
			if (record == null || instance == null || lostAndFoundRecord == null)
			{
				return;
			}
			int? firstAvailableIndex = lostAndFoundRecord.GetFirstAvailableIndex(int.MaxValue);
			if (firstAvailableIndex != null)
			{
				int index = instance.Index;
				instance.Index = firstAvailableIndex.Value;
				lostAndFoundRecord.Instances.Add(instance);
				string text = containerType.ToString();
				BaseCollectionController.InvalidIndexArguments[0] = record.Name;
				BaseCollectionController.InvalidIndexArguments[1] = record.Id;
				BaseCollectionController.InvalidIndexArguments[2] = instance.ArchetypeId.Value;
				BaseCollectionController.InvalidIndexArguments[3] = text;
				BaseCollectionController.InvalidIndexArguments[4] = index;
				SolDebug.LogToIndex(LogLevel.Error, LogIndex.Error, "{@CharacterName} ({@CharacterId}) had an invalid index ({@ItemIndex} {@ItemId}) in {@ContainerType} and has been moved to Lost and Found!", BaseCollectionController.InvalidIndexArguments);
				return;
			}
			PlayerCollectionController.AddInstanceToInvalid(record, instance, containerType, "InvalidIndex", false);
		}

		// Token: 0x06004F41 RID: 20289 RVA: 0x001C56F0 File Offset: 0x001C38F0
		protected override void RefreshBuybackItems()
		{
			base.RefreshBuybackItems();
			if (base.Record == null || !GameManager.IsServer)
			{
				return;
			}
			this.RefreshBuybackCollection(base.Record.MerchantBuybackItems);
			this.RefreshBuybackCollection(base.Record.BagBuybackItems);
			if (base.Record.MerchantBuybackItems != null && base.Record.MerchantBuybackItems.Count <= 0)
			{
				StaticListPool<MerchantBuybackItem>.ReturnToPool(base.Record.MerchantBuybackItems);
				base.Record.MerchantBuybackItems = null;
			}
			if (base.Record.BagBuybackItems != null && base.Record.BagBuybackItems.Count <= 0)
			{
				StaticListPool<MerchantBuybackItem>.ReturnToPool(base.Record.BagBuybackItems);
				base.Record.BagBuybackItems = null;
			}
		}

		// Token: 0x06004F42 RID: 20290 RVA: 0x001C57B0 File Offset: 0x001C39B0
		private void RefreshBuybackCollection(List<MerchantBuybackItem> buybackItems)
		{
			if (buybackItems == null || buybackItems.Count <= 0)
			{
				return;
			}
			DateTime utcNow = DateTime.UtcNow;
			for (int i = 0; i < buybackItems.Count; i++)
			{
				MerchantBuybackItem merchantBuybackItem = buybackItems[i];
				if (utcNow >= merchantBuybackItem.ExpirationTime)
				{
					StaticPool<ArchetypeInstance>.ReturnToPool(merchantBuybackItem.Instance);
					StaticPool<MerchantBuybackItem>.ReturnToPool(merchantBuybackItem);
					buybackItems.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x06004F43 RID: 20291 RVA: 0x001C5814 File Offset: 0x001C3A14
		protected override void InitInternal()
		{
			if (!GameManager.IsServer && this.m_equipment != null && this.m_equipment.ContainerUI != null)
			{
				EquipmentUI equipmentUI = this.m_equipment.ContainerUI as EquipmentUI;
				if (equipmentUI != null)
				{
					equipmentUI.RefreshAvailableCharges();
				}
			}
			ContainerRecord record = new ContainerRecord
			{
				Type = ContainerType.TradeOutgoing,
				Instances = new List<ArchetypeInstance>(),
				Currency = new ulong?(0UL)
			};
			TradeContainerHalfUI containerUI = GameManager.IsServer ? null : ClientGameManager.UIManager.Trade.Outgoing;
			ContainerInstance containerInstance = new ContainerInstance(this, record, containerUI);
			this.m_collections.Add(containerInstance.Id, containerInstance);
			ContainerRecord record2 = new ContainerRecord
			{
				Type = ContainerType.TradeIncoming,
				Instances = new List<ArchetypeInstance>(),
				Currency = new ulong?(0UL)
			};
			TradeContainerHalfUI containerUI2 = GameManager.IsServer ? null : ClientGameManager.UIManager.Trade.Incoming;
			ContainerInstance containerInstance2 = new ContainerInstance(this, record2, containerUI2);
			this.m_collections.Add(containerInstance2.Id, containerInstance2);
			ContainerRecord record3 = new ContainerRecord
			{
				Type = ContainerType.PostOutgoing,
				Instances = new List<ArchetypeInstance>(),
				Currency = new ulong?(0UL)
			};
			UniversalContainerUI containerUI3 = GameManager.IsServer ? null : ClientGameManager.UIManager.MailboxUI.Attachments;
			ContainerInstance containerInstance3 = new ContainerInstance(this, record3, containerUI3);
			this.m_collections.Add(containerInstance3.Id, containerInstance3);
			ContainerRecord record4 = new ContainerRecord
			{
				Type = ContainerType.AuctionOutgoing,
				Instances = new List<ArchetypeInstance>(),
				Currency = null
			};
			UniversalContainerUI containerUI4 = GameManager.IsServer ? null : ClientGameManager.UIManager.AuctionHouseUI.NewAuction;
			ContainerInstance containerInstance4 = new ContainerInstance(this, record4, containerUI4);
			this.m_collections.Add(containerInstance4.Id, containerInstance4);
			if (!GameManager.IsServer)
			{
				ContainerRecord record5 = new ContainerRecord
				{
					Type = ContainerType.PostIncoming,
					Instances = new List<ArchetypeInstance>(),
					Currency = new ulong?(0UL)
				};
				UniversalContainerUI containerUI5 = GameManager.IsServer ? null : ClientGameManager.UIManager.MailboxUI.MailDetail.Attachments;
				ContainerInstance containerInstance5 = new ContainerInstance(this, record5, containerUI5);
				this.m_collections.Add(containerInstance5.Id, containerInstance5);
				ContainerRecord record6 = new ContainerRecord
				{
					Type = ContainerType.Inspection,
					Instances = new List<ArchetypeInstance>()
				};
				InspectionUI containerUI6 = GameManager.IsServer ? null : ClientGameManager.UIManager.Inspection;
				ContainerInstance containerInstance6 = new ContainerInstance(this, record6, containerUI6);
				this.m_collections.Add(containerInstance6.Id, containerInstance6);
			}
			this.InitializeLearnables();
			this.RefreshBuybackItems();
			this.RefundHuntingLogVersionMismatches();
			if (!GameManager.IsServer && this.m_abilities != null && base.GameEntity && base.GameEntity.NetworkEntity && base.GameEntity.NetworkEntity.IsLocal)
			{
				this.m_abilities.ContentsChanged += this.LocalAbilitiesOnContentsChanged;
				this.m_subscribedToAbilityContentsChanged = true;
			}
		}

		// Token: 0x06004F44 RID: 20292 RVA: 0x0007558F File Offset: 0x0007378F
		private void LocalAbilitiesOnContentsChanged()
		{
			Action abilityContentsChanged = PlayerCollectionController.AbilityContentsChanged;
			if (abilityContentsChanged == null)
			{
				return;
			}
			abilityContentsChanged();
		}

		// Token: 0x06004F45 RID: 20293 RVA: 0x001C5B10 File Offset: 0x001C3D10
		private void InitializeLearnables()
		{
			this.m_learnableCollections = new Dictionary<string, LearnableContainerInstance>();
			if (base.Record.Learnables == null)
			{
				base.Record.Learnables = new Dictionary<ContainerType, LearnableContainerRecord>(default(ContainerTypeComparer));
			}
			this.AddLearnableContainerIfAbsent(ContainerType.Recipes);
			this.AddLearnableContainerIfAbsent(ContainerType.Emotes);
			this.AddLearnableContainerIfAbsent(ContainerType.Titles);
			foreach (KeyValuePair<ContainerType, LearnableContainerRecord> keyValuePair in base.Record.Learnables)
			{
				if (keyValuePair.Key == ContainerType.Emotes && GlobalSettings.Values != null && GlobalSettings.Values.Player != null && GlobalSettings.Values.Player.StartingData != null && GlobalSettings.Values.Player.StartingData.Emotes != null)
				{
					foreach (Emote emote in GlobalSettings.Values.Player.StartingData.Emotes)
					{
						if (!keyValuePair.Value.LearnableIds.Contains(emote.Id))
						{
							keyValuePair.Value.LearnableIds.Add(emote.Id);
						}
					}
				}
				LearnableContainerInstance learnableContainerInstance = new LearnableContainerInstance(this, keyValuePair.Value);
				this.m_learnableCollections.Add(learnableContainerInstance.Id, learnableContainerInstance);
				switch (keyValuePair.Key)
				{
				case ContainerType.Recipes:
					this.m_recipes = learnableContainerInstance;
					break;
				case ContainerType.Emotes:
					this.m_emotes = learnableContainerInstance;
					break;
				case ContainerType.Titles:
					this.m_titles = learnableContainerInstance;
					break;
				}
			}
		}

		// Token: 0x06004F46 RID: 20294 RVA: 0x001C5CD8 File Offset: 0x001C3ED8
		private void AddLearnableContainerIfAbsent(ContainerType containerType)
		{
			if (!base.Record.Learnables.ContainsKey(containerType))
			{
				LearnableContainerRecord value = new LearnableContainerRecord
				{
					Type = containerType,
					LearnableIds = new List<UniqueId>()
				};
				base.Record.Learnables.Add(containerType, value);
			}
		}

		// Token: 0x06004F47 RID: 20295 RVA: 0x001C5D24 File Offset: 0x001C3F24
		private void RefundHuntingLogVersionMismatches()
		{
			if (!GameManager.IsServer || base.Record == null || base.Record.HuntingLog == null || base.Record.HuntingLog.Count <= 0)
			{
				return;
			}
			if (base.Record.HuntingLogVersion != GlobalSettings.Values.HuntingLog.Version)
			{
				base.Record.HuntingLogVersion = GlobalSettings.Values.HuntingLog.Version;
				using (Dictionary<UniqueId, HuntingLogEntry>.Enumerator enumerator = base.Record.HuntingLog.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<UniqueId, HuntingLogEntry> keyValuePair = enumerator.Current;
						HuntingLogProfile huntingLogProfile;
						if (InternalGameDatabase.Archetypes.TryGetAsType<HuntingLogProfile>(keyValuePair.Key, out huntingLogProfile))
						{
							keyValuePair.Value.ActivePerks = null;
							keyValuePair.Value.Version = huntingLogProfile.Version;
							keyValuePair.Value.SettingsVersion = huntingLogProfile.SettingsVersion;
						}
					}
					return;
				}
			}
			foreach (KeyValuePair<UniqueId, HuntingLogEntry> keyValuePair2 in base.Record.HuntingLog)
			{
				HuntingLogProfile huntingLogProfile2;
				if (InternalGameDatabase.Archetypes.TryGetAsType<HuntingLogProfile>(keyValuePair2.Key, out huntingLogProfile2))
				{
					if (keyValuePair2.Value.SettingsVersion <= 0)
					{
						keyValuePair2.Value.SettingsVersion = huntingLogProfile2.SettingsVersion;
					}
					if (huntingLogProfile2.Version != keyValuePair2.Value.Version || huntingLogProfile2.SettingsVersion != keyValuePair2.Value.SettingsVersion)
					{
						keyValuePair2.Value.ActivePerks = null;
						keyValuePair2.Value.Version = huntingLogProfile2.Version;
						keyValuePair2.Value.SettingsVersion = huntingLogProfile2.SettingsVersion;
					}
				}
			}
		}

		// Token: 0x06004F48 RID: 20296 RVA: 0x001C5F04 File Offset: 0x001C4104
		private void AddContainer(ContainerRecord record, IContainerUI containerUi)
		{
			ContainerInstance containerInstance = new ContainerInstance(this, record, containerUi);
			this.m_collections.Add(containerInstance.Id, containerInstance);
		}

		// Token: 0x06004F49 RID: 20297 RVA: 0x001C5F2C File Offset: 0x001C412C
		public void RemoveInstanceFromRemoteClient(ArchetypeInstance instance, ItemDestructionContext context)
		{
			if (!GameManager.IsServer)
			{
				return;
			}
			ContainerInstance containerInstance = instance.ContainerInstance;
			ArchetypeInstance archetypeInstance = containerInstance.Remove(instance.InstanceId);
			ItemDestructionTransaction response = new ItemDestructionTransaction
			{
				Op = OpCodes.Ok,
				InstanceId = archetypeInstance.InstanceId,
				SourceContainer = containerInstance.Id,
				Context = context
			};
			base.GameEntity.NetworkEntity.PlayerRpcHandler.DestroyItemRequestResponse(response);
			StaticPool<ArchetypeInstance>.ReturnToPool(archetypeInstance);
		}

		// Token: 0x06004F4A RID: 20298 RVA: 0x001C5FA8 File Offset: 0x001C41A8
		public void RemoveInstances(List<ArchetypeInstance> instances, ItemDestructionContext context, out ItemMultiDestructionTransaction transaction, ReplicationFlags replicationFlags = ReplicationFlags.Client | ReplicationFlags.Server)
		{
			transaction = default(ItemMultiDestructionTransaction);
			if (!GameManager.IsServer || instances == null || instances.Count == 0)
			{
				return;
			}
			transaction = new ItemMultiDestructionTransaction
			{
				Op = OpCodes.Ok,
				Items = new ValueTuple<UniqueId, string>[instances.Count],
				Context = context
			};
			for (int i = 0; i < instances.Count; i++)
			{
				transaction.Items[i].Item1 = instances[i].InstanceId;
				transaction.Items[i].Item2 = instances[i].ContainerInstance.Id;
				if (replicationFlags.HasBitFlag(ReplicationFlags.Server))
				{
					instances[i].ContainerInstance.Remove(instances[i].InstanceId);
					StaticPool<ArchetypeInstance>.ReturnToPool(instances[i]);
				}
			}
			if (replicationFlags.HasBitFlag(ReplicationFlags.Client))
			{
				base.GameEntity.NetworkEntity.PlayerRpcHandler.DestroyMultiItemRequestResponse(transaction);
			}
		}

		// Token: 0x06004F4B RID: 20299 RVA: 0x001C60AC File Offset: 0x001C42AC
		public void AddInstanceToRemoteClient(ArchetypeInstance instance, ContainerInstance containerInstance)
		{
			if (!GameManager.IsServer)
			{
				return;
			}
			instance.Index = containerInstance.GetFirstAvailableIndex();
			containerInstance.Add(instance, true);
			ArchetypeAddedTransaction archetypeAddedTransaction = default(ArchetypeAddedTransaction);
			archetypeAddedTransaction.Op = OpCodes.Ok;
			archetypeAddedTransaction.Instance = instance;
			archetypeAddedTransaction.TargetContainer = containerInstance.Id;
		}

		// Token: 0x06004F4C RID: 20300 RVA: 0x001C60FC File Offset: 0x001C42FC
		public void OpenRemoteContainer(IInteractive interactive, ContainerRecord record)
		{
			UniversalContainerUI containerUI = (!GameManager.IsServer && record != null && ClientGameManager.UIManager) ? ClientGameManager.UIManager.GetRemoteContainerUI(record.Type) : null;
			if (!GameManager.IsServer && record != null && record.Type == ContainerType.Loot && (record.Instances == null || record.Instances.Count <= 0))
			{
				InteractiveNpc interactiveNpc;
				string content = (interactive.TryGetAsType(out interactiveNpc) && interactiveNpc.GameEntity && interactiveNpc.GameEntity.CharacterData) ? (interactiveNpc.GameEntity.CharacterData.Name.Value + " was Empty!") : "Empty!";
				MessageManager.ChatQueue.AddToQueue(MessageType.Loot, content);
				ClientGameManager.UIManager.PlayRandomClip(GlobalSettings.Values.Audio.EmptyLootBagAudioCollection, null);
				interactive.EndInteraction(base.GameEntity, true);
				if (Options.GameOptions.LeaveCombatAfterLooting.Value && LocalPlayer.Animancer != null && LocalPlayer.Animancer.Stance == Stance.Combat)
				{
					LocalPlayer.Animancer.SetStance(Stance.Idle);
				}
				return;
			}
			ContainerInstance containerInstance = new ContainerInstance(this, record, containerUI);
			containerInstance.Interactive = interactive;
			if (this.m_collections.ContainsKey(containerInstance.Id))
			{
				this.CloseRemoteContainer(containerInstance.Id);
			}
			this.m_collections.Add(containerInstance.Id, containerInstance);
			IContainerUI containerUI2 = containerInstance.ContainerUI;
			if (containerUI2 != null)
			{
				containerUI2.Show();
			}
			this.m_remoteContainer = containerInstance;
			if (!GameManager.IsServer && record != null && record.Type == ContainerType.Loot)
			{
				if (containerInstance.Count <= 0)
				{
					containerInstance.Interactive.EndInteraction(base.GameEntity, true);
					return;
				}
				LocalPlayer.LootInteractive = interactive;
				LocalPlayer.Animancer.ToggleLooting(true);
			}
		}

		// Token: 0x06004F4D RID: 20301 RVA: 0x001C62C8 File Offset: 0x001C44C8
		public void CloseRemoteContainer(string recordId)
		{
			ContainerInstance containerInstance;
			if (this.m_collections.TryGetValue(recordId, out containerInstance))
			{
				if (this.m_remoteContainer == containerInstance)
				{
					this.m_remoteContainer = null;
				}
				IContainerUI containerUI = containerInstance.ContainerUI;
				if (containerUI != null)
				{
					containerUI.Hide();
				}
				if (GameManager.IsServer)
				{
					containerInstance.SaveRecord();
				}
				else
				{
					if (containerInstance.ContainerType == ContainerType.Loot)
					{
						LocalPlayer.Animancer.ToggleLooting(false);
						LocalPlayer.LootInteractive = null;
					}
					containerInstance.DestroyContents();
					containerInstance.Unsubscribe();
				}
				this.m_collections.Remove(recordId);
			}
		}

		// Token: 0x06004F4E RID: 20302 RVA: 0x001C634C File Offset: 0x001C454C
		private bool CanRemoveFromContainer(ArchetypeInstance instance)
		{
			return !instance.IsItem || !instance.ItemData.IsSoulbound || instance.ContainerInstance == null || (instance.ContainerInstance.ContainerType != ContainerType.Bank && (instance.ContainerInstance.ContainerType == ContainerType.Loot || !instance.ContainerInstance.ContainerType.IsRemote()));
		}

		// Token: 0x06004F4F RID: 20303 RVA: 0x001C63B0 File Offset: 0x001C45B0
		public void ProcessMergeRequest(MergeRequest request)
		{
			OpCodes op = OpCodes.Error;
			int num = 0;
			ContainerInstance containerInstance;
			ContainerInstance containerInstance2;
			ArchetypeInstance archetypeInstance;
			ArchetypeInstance archetypeInstance2;
			if (this.m_collections.TryGetValue(request.SourceContainer, out containerInstance) && this.m_collections.TryGetValue(request.TargetContainer, out containerInstance2) && containerInstance.TryGetInstanceForInstanceId(request.SourceInstanceId, out archetypeInstance) && containerInstance2.TryGetInstanceForInstanceId(request.TargetInstanceId, out archetypeInstance2) && archetypeInstance2.CanMergeWith(archetypeInstance))
			{
				op = OpCodes.Ok;
				if (containerInstance.ContainerType == ContainerType.Loot)
				{
					this.SendGroupLootMessage(archetypeInstance);
				}
				int num2 = 1;
				if (archetypeInstance.ItemData.Count != null)
				{
					num2 = archetypeInstance.ItemData.Count.Value;
				}
				int num3 = 1;
				if (archetypeInstance2.ItemData.Count != null)
				{
					num3 = archetypeInstance2.ItemData.Count.Value;
				}
				num = num3 + num2;
				archetypeInstance2.ItemData.Count = new int?(num);
				containerInstance.Remove(archetypeInstance.InstanceId);
				archetypeInstance.ReturnToPool();
				this.UpdateOutgoingCurrency(containerInstance.ContainerType);
				this.UpdateOutgoingCurrency(containerInstance2.ContainerType);
				if (containerInstance.ContainerType == ContainerType.Loot && base.GameEntity.CharacterData.ObjectiveOrders.HasOrders<GatherObjective>())
				{
					List<ValueTuple<UniqueId, GatherObjective>> pooledOrderList = base.GameEntity.CharacterData.ObjectiveOrders.GetPooledOrderList<GatherObjective>();
					foreach (ValueTuple<UniqueId, GatherObjective> valueTuple in pooledOrderList)
					{
						if (valueTuple.Item2.Matches(archetypeInstance2))
						{
							Debug.Log(string.Format("sourceCount: {0}", num2));
							valueTuple.Item2.TryAdvance(valueTuple.Item1, base.GameEntity, (byte)num2);
						}
					}
					base.GameEntity.CharacterData.ObjectiveOrders.ReturnPooledOrderList<GatherObjective>(pooledOrderList);
				}
				if (GameManager.IsServer && containerInstance2.ContainerType == ContainerType.TradeOutgoing && base.TradeId != null)
				{
					ServerGameManager.TradeManager.Server_ItemCountChanged(base.TradeId.Value, base.GameEntity.NetworkEntity, archetypeInstance2.InstanceId, archetypeInstance2.ItemData.Count.Value);
				}
			}
			MergeResponse response = new MergeResponse
			{
				Op = op,
				TransactionId = request.TransactionId,
				NewTargetCount = num
			};
			base.GameEntity.NetworkEntity.PlayerRpcHandler.MergeRequestResponse(response);
		}

		// Token: 0x06004F50 RID: 20304 RVA: 0x001C6658 File Offset: 0x001C4858
		public void ProcessSplitRequest(SplitRequest request)
		{
			OpCodes op = OpCodes.Error;
			ArchetypeInstance archetypeInstance = null;
			string targetContainer = "";
			ContainerInstance containerInstance;
			ArchetypeInstance archetypeInstance2;
			ContainerInstance containerInstance2;
			int index;
			if (request.SplitCount > 0 && this.m_collections.TryGetValue(request.SourceContainer, out containerInstance) && containerInstance.CanSplit() && containerInstance.TryGetInstanceForInstanceId(request.InstanceId, out archetypeInstance2) && containerInstance.CanSplitSubscriberSlotCheck(archetypeInstance2.Index) && archetypeInstance2.Archetype is IStackable && archetypeInstance2.ItemData.Count != null && archetypeInstance2.ItemData.Count.Value > request.SplitCount && this.m_collections.TryGetValue(request.TargetContainer, out containerInstance2) && containerInstance2.HasRoom() && containerInstance2.IsUnlocked() && containerInstance2.TryGetFirstAvailableIndex(base.GameEntity, out index))
			{
				op = OpCodes.Ok;
				targetContainer = containerInstance2.Id;
				archetypeInstance = archetypeInstance2.Archetype.CreateNewInstance();
				archetypeInstance.ItemData.CopyDataFrom(archetypeInstance2.ItemData);
				archetypeInstance.ItemData.Count = new int?(request.SplitCount);
				archetypeInstance.Index = index;
				containerInstance2.Add(archetypeInstance, true);
				archetypeInstance2.ItemData.Count = archetypeInstance2.ItemData.Count - request.SplitCount;
				this.UpdateOutgoingCurrency(containerInstance.ContainerType);
				if (GameManager.IsServer && containerInstance.ContainerType == ContainerType.TradeOutgoing && base.TradeId != null)
				{
					ServerGameManager.TradeManager.Server_ItemCountChanged(base.TradeId.Value, base.GameEntity.NetworkEntity, archetypeInstance2.InstanceId, archetypeInstance2.ItemData.Count.Value);
				}
			}
			SplitResponse response = new SplitResponse
			{
				Op = op,
				TransactionId = request.TransactionId,
				Instance = archetypeInstance,
				TargetContainer = targetContainer
			};
			base.GameEntity.NetworkEntity.PlayerRpcHandler.SplitRequestResponse(response);
		}

		// Token: 0x06004F51 RID: 20305 RVA: 0x001C68A0 File Offset: 0x001C4AA0
		public void ProcessTransferRequest(TransferRequest request)
		{
			OpCodes op = OpCodes.Error;
			int num = -1;
			ArchetypeInstance instance = null;
			ContainerInstance containerInstance;
			ContainerInstance containerInstance2;
			ArchetypeInstance archetypeInstance;
			if (this.m_collections.TryGetValue(request.SourceContainer, out containerInstance) && this.m_collections.TryGetValue(request.TargetContainer, out containerInstance2) && containerInstance2.HasRoom() && containerInstance.TryGetInstanceForInstanceId(request.InstanceId, out archetypeInstance))
			{
				bool flag = containerInstance.Id == containerInstance2.Id;
				if (flag && archetypeInstance != null && archetypeInstance.SymbolicLink != null)
				{
					archetypeInstance.SymbolicLink.Freeze = true;
				}
				archetypeInstance = ((flag || this.CanRemoveFromContainer(archetypeInstance)) ? containerInstance.Remove(request.InstanceId) : null);
				if (archetypeInstance != null)
				{
					num = request.TargetIndex;
					if (request.TargetIndex == -1)
					{
						int num2;
						num = (containerInstance2.TryGetFirstAvailableIndex(base.GameEntity, out num2) ? num2 : containerInstance2.GetFirstAvailableIndex());
					}
					archetypeInstance.Index = num;
					containerInstance2.Add(archetypeInstance, true);
					op = OpCodes.Ok;
					if (flag && archetypeInstance.SymbolicLink != null)
					{
						archetypeInstance.SymbolicLink.Freeze = false;
					}
					if (containerInstance.ContainerType == ContainerType.Loot)
					{
						this.SendGroupLootMessage(archetypeInstance);
						if (archetypeInstance.Archetype && base.GameEntity && base.GameEntity.CharacterData && LocalZoneManager.ZoneRecord != null)
						{
							PlayerCollectionController.LootObjectArray[0] = "Looted";
							PlayerCollectionController.LootObjectArray[1] = archetypeInstance.InstanceId.Value;
							PlayerCollectionController.LootObjectArray[2] = archetypeInstance.Archetype.DisplayName;
							PlayerCollectionController.LootObjectArray[3] = base.GameEntity.CharacterData.CharacterId.Value;
							PlayerCollectionController.LootObjectArray[4] = base.GameEntity.CharacterData.Name.Value;
							PlayerCollectionController.LootObjectArray[5] = LocalZoneManager.ZoneRecord.DisplayName;
							SolDebug.LogToIndex(LogLevel.Information, LogIndex.Item, "{@EventType} {@InstanceId} {@Item} looted by {@CharacterId} {@CharacterName} in {@Zone}", PlayerCollectionController.LootObjectArray);
						}
					}
					if (containerInstance.ContainerType == ContainerType.Loot && base.GameEntity.CharacterData.ObjectiveOrders.HasOrders<GatherObjective>())
					{
						List<ValueTuple<UniqueId, GatherObjective>> pooledOrderList = base.GameEntity.CharacterData.ObjectiveOrders.GetPooledOrderList<GatherObjective>();
						foreach (ValueTuple<UniqueId, GatherObjective> valueTuple in pooledOrderList)
						{
							if (valueTuple.Item2.Matches(archetypeInstance))
							{
								GatherObjective item = valueTuple.Item2;
								UniqueId item2 = valueTuple.Item1;
								GameEntity gameEntity = base.GameEntity;
								ItemInstanceData itemData = archetypeInstance.ItemData;
								item.TryAdvance(item2, gameEntity, (byte)(((itemData != null) ? itemData.Count : null) ?? 1));
							}
						}
						base.GameEntity.CharacterData.ObjectiveOrders.ReturnPooledOrderList<GatherObjective>(pooledOrderList);
					}
					this.ApplyItemFlagsForTransfer(containerInstance, archetypeInstance);
					this.UpdateOutgoingCurrency(containerInstance2.ContainerType);
					this.UpdateOutgoingCurrency(containerInstance.ContainerType);
					if (!containerInstance.ContainerType.IsLocal())
					{
						instance = archetypeInstance;
					}
				}
			}
			TransferResponse response = new TransferResponse
			{
				Op = op,
				TransactionId = request.TransactionId,
				TargetIndex = num,
				Instance = instance
			};
			base.GameEntity.NetworkEntity.PlayerRpcHandler.TransferRequestResponse(response);
		}

		// Token: 0x06004F52 RID: 20306 RVA: 0x001C6C04 File Offset: 0x001C4E04
		public void ProcessSwapRequest(SwapRequest request)
		{
			OpCodes op = OpCodes.Error;
			ContainerInstance containerInstance;
			ContainerInstance containerInstance2;
			ContainerInstance containerInstance3;
			ArchetypeInstance archetypeInstance;
			ArchetypeInstance archetypeInstance2;
			if (request.SourceContainerA == request.SourceContainerB && this.m_collections.TryGetValue(request.SourceContainerA, out containerInstance) && containerInstance != null && containerInstance.ContainerType.AllowInternalSwap())
			{
				if (containerInstance.InternalSwap(request.InstanceIdA, request.InstanceIdB))
				{
					op = OpCodes.Ok;
					this.UpdateOutgoingCurrency(containerInstance.ContainerType);
				}
			}
			else if (this.m_collections.TryGetValue(request.SourceContainerA, out containerInstance2) && this.m_collections.TryGetValue(request.SourceContainerB, out containerInstance3) && containerInstance2.TryGetInstanceForInstanceId(request.InstanceIdA, out archetypeInstance) && containerInstance3.TryGetInstanceForInstanceId(request.InstanceIdB, out archetypeInstance2))
			{
				SymbolicLinkData? symbolicLinkData = null;
				SymbolicLinkData? symbolicLinkData2 = null;
				if (containerInstance2.Id == containerInstance3.Id || (this.CanRemoveFromContainer(archetypeInstance) && this.CanRemoveFromContainer(archetypeInstance2)))
				{
					if (containerInstance2.ContainerType.RequiresSymbolicLinkForPlacement() || containerInstance3.ContainerType.RequiresSymbolicLinkForPlacement())
					{
						symbolicLinkData = archetypeInstance.GetSymbolicLinkData();
						symbolicLinkData2 = archetypeInstance2.GetSymbolicLinkData();
					}
					archetypeInstance = containerInstance2.Remove(request.InstanceIdA);
					archetypeInstance2 = containerInstance3.Remove(request.InstanceIdB);
				}
				else
				{
					archetypeInstance = null;
					archetypeInstance2 = null;
				}
				if (archetypeInstance != null && archetypeInstance2 != null)
				{
					int index = archetypeInstance2.Index;
					int index2 = archetypeInstance.Index;
					archetypeInstance.Index = index;
					archetypeInstance2.Index = index2;
					ContainerInstance containerInstance4 = containerInstance3;
					ContainerInstance containerInstance5 = containerInstance2;
					if (symbolicLinkData != null)
					{
						if (symbolicLinkData.Value.PreviousContainer == containerInstance3)
						{
							archetypeInstance2.PreviousContainerInstance = symbolicLinkData.Value.PreviousContainer;
							archetypeInstance2.PreviousIndex = symbolicLinkData.Value.PreviousIndex;
						}
						else
						{
							containerInstance4 = symbolicLinkData.Value.PreviousContainer;
							archetypeInstance.Index = symbolicLinkData.Value.PreviousIndex;
						}
					}
					if (symbolicLinkData2 != null)
					{
						if (symbolicLinkData2.Value.PreviousContainer == containerInstance2)
						{
							archetypeInstance.PreviousContainerInstance = symbolicLinkData2.Value.PreviousContainer;
							archetypeInstance.PreviousIndex = symbolicLinkData2.Value.PreviousIndex;
						}
						else
						{
							containerInstance5 = symbolicLinkData2.Value.PreviousContainer;
							archetypeInstance2.Index = symbolicLinkData2.Value.PreviousIndex;
						}
					}
					containerInstance5.Add(archetypeInstance2, true);
					containerInstance4.Add(archetypeInstance, true);
					op = OpCodes.Ok;
					this.UpdateOutgoingCurrency(containerInstance2.ContainerType);
					this.UpdateOutgoingCurrency(containerInstance3.ContainerType);
				}
			}
			SwapResponse response = new SwapResponse
			{
				Op = op,
				TransactionId = request.TransactionId
			};
			base.GameEntity.NetworkEntity.PlayerRpcHandler.SwapRequestResponse(response);
		}

		// Token: 0x06004F53 RID: 20307 RVA: 0x001C6EBC File Offset: 0x001C50BC
		public void ProcessTakeAllRequest(TakeAllRequest request)
		{
			TakeAllResponse response = new TakeAllResponse
			{
				Op = OpCodes.Ok,
				TransactionId = request.TransactionId
			};
			base.GameEntity.NetworkEntity.PlayerRpcHandler.TakeAllRequestResponse(response);
		}

		// Token: 0x06004F54 RID: 20308 RVA: 0x001C6F00 File Offset: 0x001C5100
		public void ProcessItemDestructionRequest(ItemDestructionTransaction request)
		{
			request.Op = OpCodes.Error;
			ContainerInstance containerInstance;
			ArchetypeInstance archetypeInstance;
			if (this.m_collections.TryGetValue(request.SourceContainer, out containerInstance) && containerInstance.CanDestroyItem() && containerInstance.TryGetInstanceForInstanceId(request.InstanceId, out archetypeInstance) && (archetypeInstance.ItemData.ItemFlags.AllowDestruction() || base.GameEntity.GM))
			{
				string value = archetypeInstance.InstanceId.Value;
				string modifiedDisplayName = archetypeInstance.Archetype.GetModifiedDisplayName(archetypeInstance);
				if (containerInstance.RemoveAndDestroy(archetypeInstance.InstanceId))
				{
					request.Op = OpCodes.Ok;
					SolDebug.LogToIndex(LogLevel.Information, LogIndex.Item, "{@EventType} {@InstanceId} {@ItemName} destroyed by {@CharacterId} {@CharacterName} in {@Zone}", new object[]
					{
						"Destroyed",
						value,
						modifiedDisplayName,
						base.GameEntity.CharacterData.CharacterId.Value,
						base.GameEntity.CharacterData.Name.Value,
						LocalZoneManager.ZoneRecord.DisplayName
					});
				}
			}
			base.GameEntity.NetworkEntity.PlayerRpcHandler.DestroyItemRequestResponse(request);
		}

		// Token: 0x06004F55 RID: 20309 RVA: 0x001C7018 File Offset: 0x001C5218
		public void ProcessItemMultiDestructionRequest(ItemMultiDestructionTransaction request)
		{
			request.Op = OpCodes.Ok;
			foreach (ValueTuple<UniqueId, string> valueTuple in request.Items)
			{
				ContainerInstance containerInstance;
				ArchetypeInstance archetypeInstance;
				if (this.m_collections.TryGetValue(valueTuple.Item2, out containerInstance) && containerInstance.CanDestroyItem() && containerInstance.TryGetInstanceForInstanceId(valueTuple.Item1, out archetypeInstance) && (archetypeInstance.ItemData.ItemFlags.AllowDestruction() || base.GameEntity.GM))
				{
					string value = archetypeInstance.InstanceId.Value;
					string modifiedDisplayName = archetypeInstance.Archetype.GetModifiedDisplayName(archetypeInstance);
					if (containerInstance.RemoveAndDestroy(archetypeInstance.InstanceId))
					{
						SolDebug.LogToIndex(LogLevel.Information, LogIndex.Item, "{@EventType} {@InstanceId} {@ItemName} destroyed by {@CharacterId} {@CharacterName} in {@Zone}", new object[]
						{
							"Destroyed",
							value,
							modifiedDisplayName,
							base.GameEntity.CharacterData.CharacterId.Value,
							base.GameEntity.CharacterData.Name.Value,
							LocalZoneManager.ZoneRecord.DisplayName
						});
					}
					else
					{
						request.Op = OpCodes.Error;
					}
				}
			}
			base.GameEntity.NetworkEntity.PlayerRpcHandler.DestroyMultiItemRequestResponse(request);
		}

		// Token: 0x06004F56 RID: 20310 RVA: 0x001C715C File Offset: 0x001C535C
		public void ProcessForfeitInventoryRequest()
		{
			OpCodes op = OpCodes.Error;
			if (base.Record != null && base.Record.Corpse != null && base.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.MissingBag) && this.m_inventory != null)
			{
				if (GlobalSettings.Values.Player.BagBuybackEnabled)
				{
					List<ArchetypeInstance> list = this.m_inventory.RemoveAllInstances();
					if (list.Count > 0)
					{
						if (base.Record.BagBuybackItems == null)
						{
							base.Record.BagBuybackItems = StaticListPool<MerchantBuybackItem>.GetFromPool();
						}
						for (int i = 0; i < list.Count; i++)
						{
							if (list[i] != null)
							{
								MerchantBuybackItem fromPool = StaticPool<MerchantBuybackItem>.GetFromPool();
								fromPool.ExpirationTime = DateTime.UtcNow.AddSeconds(7200.0);
								fromPool.Cost = GlobalSettings.Values.Player.GetBagBuybackCost(base.GameEntity, list[i]);
								fromPool.Instance = list[i];
								base.Record.BagBuybackItems.Add(fromPool);
							}
						}
					}
					StaticListPool<ArchetypeInstance>.ReturnToPool(list);
				}
				this.m_inventory.DestroyContents();
				CorpseManager.RemoveCorpseForPlayer(base.GameEntity);
				op = OpCodes.Ok;
			}
			base.GameEntity.NetworkEntity.PlayerRpcHandler.ForfeitInventoryResponse(op);
		}

		// Token: 0x06004F57 RID: 20311 RVA: 0x001C72AC File Offset: 0x001C54AC
		private void RemoveAssociatedItemsFromCollection(UniqueId masteryId, List<MerchantBuybackItem> collection)
		{
			if (collection == null || collection.Count <= 0 || masteryId.IsEmpty)
			{
				return;
			}
			for (int i = 0; i < collection.Count; i++)
			{
				MerchantBuybackItem merchantBuybackItem = collection[i];
				if (merchantBuybackItem != null)
				{
					ArchetypeInstance instance = merchantBuybackItem.Instance;
					if (instance != null && instance.IsItem && instance.ItemData.AssociatedMasteryId != null && instance.ItemData.AssociatedMasteryId.Value == masteryId)
					{
						StaticPool<ArchetypeInstance>.ReturnToPool(instance);
						StaticPool<MerchantBuybackItem>.ReturnToPool(merchantBuybackItem);
						collection.RemoveAt(i);
						i--;
					}
				}
			}
		}

		// Token: 0x06004F58 RID: 20312 RVA: 0x001C7340 File Offset: 0x001C5540
		public void ProcessForgetMasteryRequest(UniqueId instanceId)
		{
			if (base.GameEntity.SkillsController.PendingIsActive || !base.GameEntity.Vitals.Stance.CanForgetMastery())
			{
				base.GameEntity.NetworkEntity.PlayerRpcHandler.SendChatNotification("Unable to forget mastery at this time.");
				return;
			}
			ArchetypeInstance archetypeInstance;
			if (this.m_masteries.TryGetInstanceForInstanceId(instanceId, out archetypeInstance))
			{
				BaseArchetype archetype = archetypeInstance.Archetype;
				List<ArchetypeInstance> list = new List<ArchetypeInstance>(1);
				foreach (ArchetypeInstance archetypeInstance2 in this.m_abilities.Instances)
				{
					AbilityArchetype abilityArchetype;
					if (archetypeInstance2.Archetype.TryGetAsType(out abilityArchetype) && abilityArchetype.Mastery.Id == archetypeInstance.ArchetypeId)
					{
						list.Add(archetypeInstance2);
					}
				}
				foreach (KeyValuePair<string, ContainerInstance> keyValuePair in this.m_collections)
				{
					foreach (ArchetypeInstance archetypeInstance3 in keyValuePair.Value.Instances)
					{
						if (archetypeInstance3.IsItem && archetypeInstance3.ItemData.AssociatedMasteryId != null && archetypeInstance3.ItemData.AssociatedMasteryId.Value == archetype.Id)
						{
							list.Add(archetypeInstance3);
						}
					}
				}
				this.RemoveAssociatedItemsFromCollection(archetype.Id, base.Record.MerchantBuybackItems);
				this.RemoveAssociatedItemsFromCollection(archetype.Id, base.Record.BagBuybackItems);
				list.Add(archetypeInstance);
				ArchetypeAddRemoveTransaction archetypeAddRemoveTransaction = new ArchetypeAddRemoveTransaction
				{
					Op = OpCodes.Ok,
					DestructionTransactions = new ItemDestructionTransaction[list.Count]
				};
				for (int i = 0; i < list.Count; i++)
				{
					ItemDestructionTransaction itemDestructionTransaction = new ItemDestructionTransaction
					{
						Op = OpCodes.Ok,
						InstanceId = list[i].InstanceId,
						SourceContainer = list[i].ContainerInstance.Id
					};
					list[i].ContainerInstance.RemoveAndDestroy(list[i].InstanceId);
					archetypeAddRemoveTransaction.DestructionTransactions[i] = itemDestructionTransaction;
				}
				base.GameEntity.NetworkEntity.PlayerRpcHandler.AddRemoveItems(archetypeAddRemoveTransaction);
				base.GameEntity.NetworkEntity.PlayerRpcHandler.SendChatNotification("You have forgotten " + archetype.DisplayName + " and all associated abilities.");
				MasteryArchetype.RefreshHighestLevelMastery(base.GameEntity);
				base.GameEntity.NetworkEntity.PlayerRpcHandler.RemoteRefreshHighestLevelMastery();
				base.GameEntity.SkillsController.ClearAbilityToMasteryCache();
				if (archetype.Id == base.GameEntity.CharacterData.BaseRoleId.Value)
				{
					base.GameEntity.CharacterData.BaseRoleId = UniqueId.Empty;
					base.GameEntity.CharacterData.SpecializedRoleId = UniqueId.Empty;
				}
			}
		}

		// Token: 0x06004F59 RID: 20313 RVA: 0x001C7694 File Offset: 0x001C5894
		public void ProcessCurrencyTransferRequest(CurrencyTransaction toRemove, CurrencyTransaction toAdd)
		{
			OpCodes op = OpCodes.Error;
			ContainerInstance containerInstance;
			ContainerInstance containerInstance2;
			if (this.m_collections.TryGetValue(toRemove.TargetContainer, out containerInstance) && containerInstance.ContainerType.HasCurrency() && containerInstance.Currency >= toRemove.Amount && this.m_collections.TryGetValue(toAdd.TargetContainer, out containerInstance2) && containerInstance2.ContainerType.HasCurrency() && toRemove.Amount == toAdd.Amount)
			{
				containerInstance.RemoveCurrency(toRemove.Amount);
				containerInstance2.AddCurrency(toAdd.Amount);
				op = OpCodes.Ok;
			}
			base.GameEntity.NetworkEntity.PlayerRpcHandler.CurrencyTransferRequestResponse(op, toRemove, toAdd);
		}

		// Token: 0x06004F5A RID: 20314 RVA: 0x001C7738 File Offset: 0x001C5938
		private void SendGroupLootMessage(ArchetypeInstance instance)
		{
			if (!base.GameEntity || !base.GameEntity.CharacterData || base.GameEntity.CharacterData.GroupId.IsEmpty)
			{
				return;
			}
			List<GameEntity> list = base.GameEntity.CharacterData.RaidId.IsEmpty ? base.GameEntity.CharacterData.NearbyGroupMembers : base.GameEntity.CharacterData.NearbyRaidMembers;
			if (list == null || list.Count <= 0)
			{
				return;
			}
			Peer[] array = PeerArrayPool.GetArray(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				array[i] = list[i].NetworkEntity.NetworkId.Peer;
			}
			BitBuffer fromPool = BitBufferExtensions.GetFromPool();
			fromPool.AddHeader(base.GameEntity.NetworkEntity, OpCodes.ChatMessage, true);
			instance.PackData(fromPool);
			Packet packetFromBuffer_ReturnBufferToPool = fromPool.GetPacketFromBuffer_ReturnBufferToPool(PacketFlags.Reliable);
			NetworkCommand networkCommand = NetworkCommandPool.GetNetworkCommand();
			networkCommand.Type = CommandType.BroadcastGroup;
			networkCommand.Channel = NetworkChannel.LootMessage;
			networkCommand.Packet = packetFromBuffer_ReturnBufferToPool;
			networkCommand.TargetGroup = array;
			GameManager.NetworkManager.AddCommandToQueue(networkCommand);
		}

		// Token: 0x06004F5B RID: 20315 RVA: 0x001C7870 File Offset: 0x001C5A70
		protected override void UpdateOutgoingCurrency(ContainerType containerType)
		{
			ContainerInstance containerInstance;
			if (GameManager.IsServer && containerType.UpdateOutgoingCurrency() && base.TryGetInstance(containerType, out containerInstance))
			{
				ulong currency = containerInstance.Currency;
				ulong num;
				switch (containerType)
				{
				case ContainerType.MerchantOutgoing:
					num = this.GetMerchantOwed(containerInstance);
					break;
				case ContainerType.BlacksmithOutgoing:
					num = this.GetRepairOwed(containerInstance);
					break;
				case ContainerType.RuneCollector:
					num = this.GetRuneExchangeOwed(containerInstance);
					break;
				default:
					Debug.LogError("Attempting to update currency for " + containerType.ToString() + "?!");
					return;
				}
				if (currency == num)
				{
					return;
				}
				containerInstance.ModifyCurrency(num);
				CurrencyModifyTransaction transaction = new CurrencyModifyTransaction
				{
					Amount = num,
					ContainerId = containerInstance.Id
				};
				base.GameEntity.NetworkEntity.PlayerRpcHandler.CurrencyModifyEvent(transaction);
			}
		}

		// Token: 0x06004F5C RID: 20316 RVA: 0x001C7944 File Offset: 0x001C5B44
		private ulong GetMerchantOwed(ContainerInstance merchantOutgoingInstance)
		{
			ulong num = 0UL;
			foreach (ArchetypeInstance archetypeInstance in merchantOutgoingInstance.Instances)
			{
				ItemArchetype itemArchetype;
				ulong num2;
				if (archetypeInstance.Archetype.TryGetAsType(out itemArchetype) && itemArchetype.TryGetSalePrice(archetypeInstance, out num2))
				{
					num += num2;
				}
			}
			return num;
		}

		// Token: 0x06004F5D RID: 20317 RVA: 0x001C79B0 File Offset: 0x001C5BB0
		private ulong GetRepairOwed(ContainerInstance blacksmithOutgoingInstance)
		{
			ulong num = 0UL;
			foreach (ArchetypeInstance archetypeInstance in blacksmithOutgoingInstance.Instances)
			{
				if (archetypeInstance.IsItem && archetypeInstance.ItemData.Durability != null)
				{
					num += (ulong)archetypeInstance.GetRepairCost();
				}
			}
			return num;
		}

		// Token: 0x06004F5E RID: 20318 RVA: 0x001C7A1C File Offset: 0x001C5C1C
		private ulong GetRuneExchangeOwed(ContainerInstance runeCollectorOutgoingInstance)
		{
			ArchetypeInstance archetypeInstance;
			RunicBattery runicBattery;
			if (runeCollectorOutgoingInstance.TryGetInstanceForIndex(0, out archetypeInstance) && archetypeInstance.IsItem && archetypeInstance.ItemData.Charges != null && archetypeInstance.ItemData.Charges.Value > 0 && archetypeInstance.Archetype.TryGetAsType(out runicBattery))
			{
				return (ulong)((long)(archetypeInstance.ItemData.Charges.Value * InternalGameDatabase.GlobalSettings.RuneExchangeCost));
			}
			return 0UL;
		}

		// Token: 0x06004F5F RID: 20319 RVA: 0x001C7A98 File Offset: 0x001C5C98
		public void ProcessMerchantItemSellRequest(UniqueId itemInstanceId, ContainerType sourceContainerType)
		{
			OpCodes op = OpCodes.Error;
			ulong num = 0UL;
			ContainerInstance containerInstance;
			ArchetypeInstance instance;
			if (base.TryGetInstance(sourceContainerType, out containerInstance) && containerInstance.TryGetInstanceForInstanceId(itemInstanceId, out instance))
			{
				num = instance.GetSellPrice();
				if (num > 0UL)
				{
					ArchetypeInstance archetypeInstance = containerInstance.Remove(itemInstanceId);
					if (archetypeInstance != null)
					{
						GlobalCounters.ItemsSold += 1U;
						ContainerInstance containerInstance2 = base.GameEntity.IsMissingBag ? this.m_personalBank : this.m_inventory;
						if (containerInstance2 != null)
						{
							containerInstance2.AddCurrency(num);
						}
						op = OpCodes.Ok;
						if (sourceContainerType == ContainerType.Equipment)
						{
							base.GameEntity.Vitals.RefreshStats();
							base.GameEntity.Vitals.RecalculateTotalArmorClass();
						}
						if (base.Record.MerchantBuybackItems == null)
						{
							base.Record.MerchantBuybackItems = StaticListPool<MerchantBuybackItem>.GetFromPool();
						}
						MerchantBuybackItem fromPool = StaticPool<MerchantBuybackItem>.GetFromPool();
						fromPool.ExpirationTime = DateTime.UtcNow.AddSeconds(3600.0);
						fromPool.Cost = num;
						fromPool.Instance = archetypeInstance;
						base.Record.MerchantBuybackItems.Add(fromPool);
						try
						{
							if (PlayerCollectionController.m_sellArguments == null)
							{
								PlayerCollectionController.m_sellArguments = new object[9];
							}
							PlayerCollectionController.m_sellArguments[0] = "SellItem";
							PlayerCollectionController.m_sellArguments[1] = base.GameEntity.User.Id;
							PlayerCollectionController.m_sellArguments[2] = base.Record.Id;
							PlayerCollectionController.m_sellArguments[3] = base.Record.Name;
							PlayerCollectionController.m_sellArguments[4] = base.GameEntity.CharacterData.AdventuringLevel;
							PlayerCollectionController.m_sellArguments[5] = ((fromPool.Instance != null && fromPool.Instance.Archetype) ? fromPool.Instance.Archetype.DisplayName : "UNKNOWN");
							PlayerCollectionController.m_sellArguments[6] = num;
							PlayerCollectionController.m_sellArguments[7] = ((fromPool.Instance != null && fromPool.Instance.ItemData != null && fromPool.Instance.ItemData.Quantity != null) ? fromPool.Instance.ItemData.Quantity.Value : 1);
							PlayerCollectionController.m_sellArguments[8] = ((fromPool.Instance != null && fromPool.Instance.Archetype) ? fromPool.Instance.Archetype.Id : "UNKNOWN");
							SolDebug.LogToIndex(LogLevel.Information, LogIndex.Economy, "{@EventType}.{@UserId}.{@CharacterId}.{@PlayerName}.{@Level} sold {@ItemName} for {@Currency} ({@Quantity} {@ArchetypeId})", PlayerCollectionController.m_sellArguments);
						}
						catch
						{
						}
					}
				}
			}
			base.GameEntity.NetworkEntity.PlayerRpcHandler.MerchantItemSellResponse(op, itemInstanceId, sourceContainerType, num);
		}

		// Token: 0x06004F60 RID: 20320 RVA: 0x001C7D4C File Offset: 0x001C5F4C
		public void ProcessMerchantBuybackRequest(InteractiveMerchant merchant, UniqueId instanceId)
		{
			if (base.Record == null || merchant == null || instanceId.IsEmpty)
			{
				return;
			}
			MerchantType merchantType = merchant.MerchantType;
			List<MerchantBuybackItem> list = (merchantType == MerchantType.Standard) ? base.Record.MerchantBuybackItems : base.Record.BagBuybackItems;
			if (list == null)
			{
				return;
			}
			int i = 0;
			while (i < list.Count)
			{
				MerchantBuybackItem merchantBuybackItem = list[i];
				if (merchantBuybackItem.Cost > 0UL && merchantBuybackItem.Instance != null && merchantBuybackItem.Instance.InstanceId == instanceId)
				{
					ulong cost = merchantBuybackItem.Cost;
					CurrencySources currencySources;
					ulong availableCurrency = merchant.GetAvailableCurrency(base.GameEntity, out currencySources);
					if (cost > availableCurrency)
					{
						base.GameEntity.NetworkEntity.PlayerRpcHandler.SendChatNotification("Insufficient funds!");
						return;
					}
					string text = (merchantBuybackItem.Instance != null && merchantBuybackItem.Instance.Archetype) ? merchantBuybackItem.Instance.Archetype.DisplayName : "UNKNOWN";
					if (this.AddItemInstanceToPlayer(merchantBuybackItem.Instance, ItemAddContext.Merchant, -1, false) != null && merchant.TryRemoveCurrency(base.GameEntity, merchantBuybackItem.Cost))
					{
						StaticPool<MerchantBuybackItem>.ReturnToPool(merchantBuybackItem);
						list.RemoveAt(i);
						base.GameEntity.NetworkEntity.PlayerRpcHandler.MerchantBuybackInventoryUpdate(merchantType, new BuybackItemData
						{
							Items = list
						});
					}
					if (PlayerCollectionController.m_buybackArguments == null)
					{
						PlayerCollectionController.m_buybackArguments = new object[8];
					}
					PlayerCollectionController.m_buybackArguments[0] = "PurchaseBuyback";
					PlayerCollectionController.m_buybackArguments[1] = base.GameEntity.User.Id;
					PlayerCollectionController.m_buybackArguments[2] = base.Record.Id;
					PlayerCollectionController.m_buybackArguments[3] = base.Record.Name;
					PlayerCollectionController.m_buybackArguments[4] = base.GameEntity.CharacterData.AdventuringLevel;
					PlayerCollectionController.m_buybackArguments[5] = text;
					PlayerCollectionController.m_buybackArguments[6] = merchantType;
					PlayerCollectionController.m_buybackArguments[7] = cost;
					SolDebug.LogToIndex(LogLevel.Information, LogIndex.Economy, "{@EventType}.{@UserId}.{@CharacterId}.{@PlayerName}.{@Level} purchased {@ItemName} from a {@MerchantType} buyback merchant for {@Currency}", PlayerCollectionController.m_buybackArguments);
					return;
				}
				else
				{
					i++;
				}
			}
		}

		// Token: 0x06004F61 RID: 20321 RVA: 0x001C7F5C File Offset: 0x001C615C
		public void ProcessDeconstructRequest(UniqueId itemInstanceId)
		{
			bool flag = false;
			ArchetypeInstance archetypeInstance;
			if (this.m_inventory != null && this.m_inventory.TryGetInstanceForInstanceId(itemInstanceId, out archetypeInstance) && archetypeInstance.ItemData != null && archetypeInstance.ItemData.ItemComponentTree != null && this.m_refinementStation != null && archetypeInstance.CanDeconstruct(base.GameEntity))
			{
				int index = archetypeInstance.Index;
				ContainerInstance containerInstance = archetypeInstance.ContainerInstance;
				UniqueId archetypeId = archetypeInstance.ArchetypeId;
				itemInstanceId = archetypeInstance.InstanceId;
				ItemArchetype itemArchetype = null;
				ItemComponentData itemComponentData = null;
				bool flag2 = UnityEngine.Random.Range(0, 4) != 0;
				ItemArchetype itemArchetype2 = (ItemArchetype)archetypeInstance.Archetype;
				int targetLevel = (itemArchetype2 != null && itemArchetype2.MinimumMaterialLevel != null) ? itemArchetype2.MinimumMaterialLevel.Value : 1;
				if (itemArchetype2.DeconstructItem != null && flag2)
				{
					itemArchetype = itemArchetype2.DeconstructItem;
				}
				else if (itemArchetype2.DeconstructItem == null)
				{
					List<ItemAndComponent> archetypeLeafListWithComponents = archetypeInstance.ItemData.ItemComponentTree.GetArchetypeLeafListWithComponents(null);
					int index2 = UnityEngine.Random.Range(0, archetypeLeafListWithComponents.Count);
					ItemAndComponent itemAndComponent = archetypeLeafListWithComponents[index2];
					RawComponent rawComponent;
					if (InternalGameDatabase.Archetypes.TryGetAsType<RawComponent>(itemAndComponent.ArchetypeId, out rawComponent) && rawComponent.FailureResult != null)
					{
						itemArchetype = rawComponent.FailureResult;
					}
					StaticListPool<ItemAndComponent>.ReturnToPool(archetypeLeafListWithComponents);
				}
				if (itemArchetype == null && GlobalSettings.Values.Crafting.DefaultDeconstructItem != null)
				{
					itemArchetype = GlobalSettings.Values.Crafting.DefaultDeconstructItem;
				}
				if (itemArchetype != null && containerInstance.RemoveAndDestroy(itemInstanceId))
				{
					ItemDestructionTransaction destructionTransaction = new ItemDestructionTransaction
					{
						Op = OpCodes.Ok,
						Context = ItemDestructionContext.Request,
						InstanceId = itemInstanceId,
						SourceContainer = containerInstance.Id
					};
					base.GameEntity.NetworkEntity.PlayerRpcHandler.DeconstructResponse(destructionTransaction);
					ArchetypeInstance archetypeInstance2;
					if (this.TryAddItemToPlayer(itemArchetype, ItemAddContext.Crafting, out archetypeInstance2, 1, -1, ItemFlags.None, false) && itemComponentData != null)
					{
						archetypeInstance2.AddItemFlag(ItemFlags.Crafted, base.Record);
						archetypeInstance2.ItemData.ItemComponentTree = new ItemComponentTree();
						archetypeInstance2.ItemData.ItemComponentTree.CopyDataFrom(itemComponentData);
					}
					Recipe recipe = null;
					foreach (LearnableArchetype learnableArchetype in this.m_recipes.Learnables)
					{
						Recipe recipe2 = learnableArchetype as Recipe;
						if (recipe2 != null)
						{
							List<ItemArchetype> allPossibleOutputItems = recipe2.GetAllPossibleOutputItems();
							using (List<ItemArchetype>.Enumerator enumerator2 = allPossibleOutputItems.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									if (enumerator2.Current.Id == archetypeId)
									{
										recipe = recipe2;
										break;
									}
								}
							}
							StaticListPool<ItemArchetype>.ReturnToPool(allPossibleOutputItems);
							if (recipe != null)
							{
								break;
							}
						}
					}
					ArchetypeInstance masteryInstance;
					if (recipe != null && this.m_masteries.TryGetInstanceForArchetypeId(recipe.Mastery.Id, out masteryInstance))
					{
						bool progressSpecialization = recipe.Ability != null && recipe.Ability.Specialization != null;
						ProgressionCalculator.OnDeconstructSuccess(base.GameEntity, masteryInstance, targetLevel, progressSpecialization);
					}
					else
					{
						Debug.LogWarning("No recipe found for deconstructed item with ArchetypeId:" + archetypeId.ToString() + ", no experience awarded.");
					}
					flag = true;
				}
			}
			if (!flag)
			{
				base.GameEntity.NetworkEntity.PlayerRpcHandler.SendChatNotification("Unable to deconstruct");
			}
		}

		// Token: 0x06004F62 RID: 20322 RVA: 0x001C82F0 File Offset: 0x001C64F0
		public void ProcessLearnAbilityRequest(UniqueId abilityId)
		{
			AbilityArchetype abilityArchetype;
			ArchetypeInstance archetypeInstance;
			ArchetypeInstance archetypeInstance2;
			if (!InternalGameDatabase.Archetypes.TryGetAsType<AbilityArchetype>(abilityId, out abilityArchetype) || !this.m_masteries.TryGetInstanceForArchetypeId(abilityArchetype.Mastery.Id, out archetypeInstance) || this.m_abilities.TryGetInstanceForArchetypeId(abilityId, out archetypeInstance2))
			{
				return;
			}
			if (!abilityArchetype.CanLearn(base.GameEntity))
			{
				return;
			}
			ArchetypeInstance archetypeInstance3 = abilityArchetype.CreateNewInstance();
			this.m_abilities.Add(archetypeInstance3, true);
			ArchetypeAddedTransaction response = new ArchetypeAddedTransaction
			{
				Op = OpCodes.Ok,
				Instance = archetypeInstance3,
				TargetContainer = archetypeInstance3.ContainerInstance.Id,
				Context = ItemAddContext.Merchant
			};
			base.GameEntity.NetworkEntity.PlayerRpcHandler.AddItemResponse(response);
		}

		// Token: 0x06004F63 RID: 20323 RVA: 0x001C83A8 File Offset: 0x001C65A8
		public void ProcessTrainSpecializationRequest(UniqueId masteryInstanceId, UniqueId specializationArchetypeId)
		{
			OpCodes op = OpCodes.Error;
			SpecializedRole specializedRole = null;
			float specLevel = 0f;
			ArchetypeInstance archetypeInstance;
			BaseRole baseRole;
			if (!specializationArchetypeId.IsEmpty && this.m_masteries.TryGetInstanceForInstanceId(masteryInstanceId, out archetypeInstance) && archetypeInstance.Mastery != null && archetypeInstance.Mastery.TryGetAsType(out baseRole) && archetypeInstance.MasteryData != null && archetypeInstance.MasteryData.Specialization == null && archetypeInstance.GetAssociatedLevel(base.GameEntity) >= 6f)
			{
				int i = 0;
				while (i < baseRole.Specializations.Length)
				{
					if (baseRole.Specializations[i].Id == specializationArchetypeId)
					{
						op = OpCodes.Ok;
						archetypeInstance.MasteryData.Specialization = new UniqueId?(baseRole.Specializations[i].Id);
						specLevel = archetypeInstance.MasteryData.SpecializationLevel;
						specializedRole = baseRole.Specializations[i];
						base.GameEntity.Vitals.RefreshStats();
						if (baseRole.Type == MasteryType.Combat)
						{
							base.GameEntity.CharacterData.SpecializedRoleId = specializationArchetypeId;
							break;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
			base.GameEntity.NetworkEntity.PlayerRpcHandler.TrainSpecializationResponse(op, masteryInstanceId, specializationArchetypeId, specLevel);
			if (specializedRole != null && specializedRole.Abilities != null && specializedRole.Abilities.Length != 0)
			{
				for (int j = 0; j < specializedRole.Abilities.Length; j++)
				{
					ArchetypeInstance archetypeInstance2 = specializedRole.Abilities[j].CreateNewInstance();
					this.m_abilities.Add(archetypeInstance2, true);
					ArchetypeAddedTransaction response = new ArchetypeAddedTransaction
					{
						Op = OpCodes.Ok,
						Instance = archetypeInstance2,
						TargetContainer = archetypeInstance2.ContainerInstance.Id,
						Context = ItemAddContext.Training
					};
					base.GameEntity.NetworkEntity.PlayerRpcHandler.AddItemResponse(response);
				}
			}
		}

		// Token: 0x06004F64 RID: 20324 RVA: 0x001C8598 File Offset: 0x001C6798
		public void ProcessForgetSpecializationRequest(UniqueId masteryInstanceId, UniqueId specializationArchetypeId)
		{
			OpCodes op = OpCodes.Error;
			ArchetypeAddRemoveTransaction? archetypeAddRemoveTransaction = null;
			ArchetypeInstance archetypeInstance;
			BaseRole baseRole;
			if (!specializationArchetypeId.IsEmpty && this.m_masteries.TryGetInstanceForInstanceId(masteryInstanceId, out archetypeInstance) && archetypeInstance.Mastery != null && archetypeInstance.Mastery.TryGetAsType(out baseRole) && archetypeInstance.MasteryData != null && archetypeInstance.MasteryData.Specialization != null && archetypeInstance.MasteryData.Specialization.Value == specializationArchetypeId)
			{
				op = OpCodes.Ok;
				archetypeInstance.MasteryData.Specialization = null;
				if (base.GameEntity.CharacterData.SpecializedRoleId == specializationArchetypeId)
				{
					base.GameEntity.CharacterData.SpecializedRoleId = UniqueId.Empty;
				}
				if (this.m_abilities != null)
				{
					List<ArchetypeInstance> list = null;
					for (int i = 0; i < this.m_abilities.Count; i++)
					{
						ArchetypeInstance index = this.m_abilities.GetIndex(i);
						if (index.Ability != null && index.Ability.Specialization != null && index.Ability.Specialization.Id == specializationArchetypeId)
						{
							if (list == null)
							{
								list = new List<ArchetypeInstance>(1);
							}
							list.Add(index);
							if (index.Ability is AuraAbility && base.GameEntity.EffectController != null && base.GameEntity.EffectController.SourceAura != null && base.GameEntity.EffectController.SourceAura.AuraAbility == index.Ability)
							{
								base.GameEntity.EffectController.SourceAura = null;
							}
						}
					}
					if (list != null && list.Count > 0)
					{
						archetypeAddRemoveTransaction = new ArchetypeAddRemoveTransaction?(new ArchetypeAddRemoveTransaction
						{
							Op = OpCodes.Ok,
							DestructionTransactions = new ItemDestructionTransaction[list.Count]
						});
						for (int j = 0; j < list.Count; j++)
						{
							ItemDestructionTransaction itemDestructionTransaction = new ItemDestructionTransaction
							{
								Op = OpCodes.Ok,
								InstanceId = list[j].InstanceId,
								SourceContainer = list[j].ContainerInstance.Id
							};
							list[j].ContainerInstance.RemoveAndDestroy(list[j].InstanceId);
							archetypeAddRemoveTransaction.Value.DestructionTransactions[j] = itemDestructionTransaction;
						}
					}
				}
			}
			base.GameEntity.NetworkEntity.PlayerRpcHandler.ForgetSpecializationResponse(op, masteryInstanceId, specializationArchetypeId);
			if (archetypeAddRemoveTransaction != null)
			{
				base.GameEntity.NetworkEntity.PlayerRpcHandler.AddRemoveItems(archetypeAddRemoveTransaction.Value);
			}
			MasteryArchetype.RefreshHighestLevelMastery(base.GameEntity);
			base.GameEntity.NetworkEntity.PlayerRpcHandler.RemoteRefreshHighestLevelMastery();
			base.GameEntity.Vitals.RefreshStats();
		}

		// Token: 0x06004F65 RID: 20325 RVA: 0x001C88B8 File Offset: 0x001C6AB8
		public void ProcessPurchaseContainerExpansionRequest(string containerId)
		{
			PurchaseContainerExpansionTransaction transaction = new PurchaseContainerExpansionTransaction
			{
				Op = OpCodes.Error,
				ContainerId = containerId
			};
			ContainerInstance containerInstance;
			BankProfile bankProfile;
			ulong num;
			if (this.m_collections.TryGetValue(containerId, out containerInstance) && containerInstance.ContainerProfile != null && containerInstance.ContainerProfile.TryGetAsType(out bankProfile) && bankProfile.CanPurchaseMore(containerInstance.ExpansionsPurchased, out num))
			{
				ulong num2 = containerInstance.Currency;
				if (!base.GameEntity.IsMissingBag)
				{
					num2 += this.m_inventory.Currency;
				}
				if (num2 >= num)
				{
					if (containerInstance.Currency >= num)
					{
						transaction.CurrencyToRemoveFromContainer = new ulong?(num);
						containerInstance.RemoveCurrency(num);
					}
					else if (!base.GameEntity.IsMissingBag)
					{
						ulong num3 = num - containerInstance.Currency;
						if (containerInstance.Currency > 0UL)
						{
							transaction.CurrencyToRemoveFromContainer = new ulong?(containerInstance.Currency);
							containerInstance.RemoveCurrency(containerInstance.Currency);
						}
						transaction.CurrencyToRemoveFromInventory = new ulong?(num3);
						this.m_inventory.RemoveCurrency(num3);
					}
					containerInstance.ExpansionPurchased();
					transaction.Op = OpCodes.Ok;
					try
					{
						if (PlayerCollectionController.m_buyExpansionArguments == null)
						{
							PlayerCollectionController.m_buyExpansionArguments = new object[7];
						}
						PlayerCollectionController.m_buyExpansionArguments[0] = "PurchaseBagExpansion";
						PlayerCollectionController.m_buyExpansionArguments[1] = base.GameEntity.User.Id;
						PlayerCollectionController.m_buyExpansionArguments[2] = base.Record.Id;
						PlayerCollectionController.m_buyExpansionArguments[3] = base.Record.Name;
						PlayerCollectionController.m_buyExpansionArguments[4] = base.GameEntity.CharacterData.AdventuringLevel;
						PlayerCollectionController.m_buyExpansionArguments[5] = containerId;
						PlayerCollectionController.m_buyExpansionArguments[6] = num;
						SolDebug.LogToIndex(LogLevel.Information, LogIndex.Economy, "{@EventType}.{@UserId}.{@CharacterId}.{@PlayerName}.{@Level} purchased a {@ContainerId} expansion for {@Currency}", PlayerCollectionController.m_buyExpansionArguments);
					}
					catch
					{
					}
				}
			}
			base.GameEntity.NetworkEntity.PlayerRpcHandler.PurchaseContainerExpansionResponse(transaction);
		}

		// Token: 0x06004F66 RID: 20326 RVA: 0x001C8AA8 File Offset: 0x001C6CA8
		private void LogBlacksmithRepair(GameEntity entity, ulong cost)
		{
			try
			{
				if (PlayerCollectionController.m_blacksmithLogArguments == null)
				{
					PlayerCollectionController.m_blacksmithLogArguments = new object[4];
				}
				PlayerCollectionController.m_blacksmithLogArguments[0] = entity.User.Id;
				PlayerCollectionController.m_blacksmithLogArguments[1] = entity.CollectionController.Record.Id;
				PlayerCollectionController.m_blacksmithLogArguments[2] = entity.CollectionController.Record.Name;
				PlayerCollectionController.m_blacksmithLogArguments[3] = cost;
				SolDebug.LogToIndex(LogLevel.Information, LogIndex.Economy, "{@UserId}.{@CharacterId}.{@PlayerName} has paid {@Currency} to have items repaired.", PlayerCollectionController.m_blacksmithLogArguments);
			}
			catch
			{
			}
		}

		// Token: 0x06004F67 RID: 20327 RVA: 0x001C8B3C File Offset: 0x001C6D3C
		protected override void ModifyEventCurrency(ulong delta, bool removing)
		{
			base.ModifyEventCurrency(delta, removing);
			if (delta > 0UL && base.GameEntity && base.GameEntity.User != null)
			{
				UserRecord user = base.GameEntity.User;
				ulong? eventCurrency = user.EventCurrency;
				ulong num = (user.EventCurrency != null) ? user.EventCurrency.Value : 0UL;
				if (removing)
				{
					user.EventCurrency = new ulong?((delta > num) ? 0UL : (num - delta));
				}
				else
				{
					user.EventCurrency = new ulong?(num + delta);
				}
				if (user.EventCurrency != null && user.EventCurrency.Value <= 0UL)
				{
					user.EventCurrency = null;
				}
				ulong? num2 = eventCurrency;
				ulong? eventCurrency2 = user.EventCurrency;
				if (!(num2.GetValueOrDefault() == eventCurrency2.GetValueOrDefault() & num2 != null == (eventCurrency2 != null)))
				{
					base.GameEntity.NetworkEntity.PlayerRpcHandler.ProcessEventCurrencyTransaction(user.EventCurrency);
					base.GameEntity.ServerPlayerController.QueueUserRecordForEventCurrencyUpdate(user);
				}
			}
		}

		// Token: 0x06004F68 RID: 20328 RVA: 0x001C8C50 File Offset: 0x001C6E50
		public void ProcessTransferResponse(TransferRequest request, TransferResponse response)
		{
			if (response.Op != OpCodes.Ok)
			{
				string text = string.Empty;
				ContainerInstance containerInstance;
				ArchetypeInstance archetypeInstance;
				if (this.m_collections.TryGetValue(request.SourceContainer, out containerInstance) && containerInstance.TryGetInstanceForInstanceId(request.InstanceId, out archetypeInstance))
				{
					if (archetypeInstance.InstanceUI != null)
					{
						archetypeInstance.InstanceUI.ResetUI();
					}
					if (archetypeInstance.Archetype != null)
					{
						text = archetypeInstance.Archetype.GetModifiedDisplayName(archetypeInstance);
					}
				}
				string content = string.IsNullOrEmpty(text) ? "Transfer failed!" : ZString.Format<string>("Transfer for {0} failed!", text);
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content);
				return;
			}
			ContainerInstance containerInstance2;
			this.m_collections.TryGetValue(request.SourceContainer, out containerInstance2);
			ContainerInstance containerInstance3;
			this.m_collections.TryGetValue(request.TargetContainer, out containerInstance3);
			if (containerInstance2 != null && containerInstance3 != null)
			{
				bool flag = containerInstance2.Id == containerInstance3.Id;
				ArchetypeInstance archetypeInstance2;
				if (flag && containerInstance2.TryGetInstanceForInstanceId(request.InstanceId, out archetypeInstance2) && archetypeInstance2.SymbolicLink != null)
				{
					archetypeInstance2.SymbolicLink.Freeze = true;
				}
				archetypeInstance2 = containerInstance2.Remove(request.InstanceId);
				if (response.Instance != null)
				{
					archetypeInstance2.Index = response.Instance.Index;
					archetypeInstance2.ItemData.ItemFlags = response.Instance.ItemData.ItemFlags;
				}
				else
				{
					archetypeInstance2.Index = response.TargetIndex;
					this.ApplyItemFlagsForTransfer(containerInstance2, archetypeInstance2);
				}
				containerInstance3.Add(archetypeInstance2, true);
				if (flag && archetypeInstance2.SymbolicLink != null)
				{
					archetypeInstance2.SymbolicLink.Freeze = false;
				}
				if (containerInstance2.ContainerType == ContainerType.Loot)
				{
					UIManager.InvokeItemAddedToContainer(containerInstance3.ContainerType);
					this.ItemAddedLog(archetypeInstance2, ItemAddContext.Loot);
					if (containerInstance2.Count <= 0)
					{
						IContainerUI containerUI = containerInstance2.ContainerUI;
						if (containerUI != null)
						{
							containerUI.CloseButtonPressed();
						}
					}
				}
				if (containerInstance2.ContainerType == ContainerType.Abilities || containerInstance3.ContainerType == ContainerType.Abilities)
				{
					UIManager.InvokeAbilityContainerChanged();
					return;
				}
			}
			else if (containerInstance3 != null && response.Instance != null)
			{
				response.Instance.CreateItemInstanceUI();
				containerInstance3.Add(response.Instance, true);
				if (request.SourceContainer == ContainerType.Loot.ToString())
				{
					this.ItemAddedLog(response.Instance, ItemAddContext.Loot);
				}
				if (containerInstance3.ContainerType == ContainerType.Abilities)
				{
					UIManager.InvokeAbilityContainerChanged();
					return;
				}
			}
			else
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Unknown transfer error!");
			}
		}

		// Token: 0x06004F69 RID: 20329 RVA: 0x001C8EA8 File Offset: 0x001C70A8
		public void ProcessSwapResponse(SwapRequest request, SwapResponse response)
		{
			ContainerInstance containerInstance;
			ContainerInstance containerInstance2;
			if (this.m_collections.TryGetValue(request.SourceContainerA, out containerInstance) && this.m_collections.TryGetValue(request.SourceContainerB, out containerInstance2))
			{
				if (response.Op == OpCodes.Ok)
				{
					if (request.SourceContainerA == request.SourceContainerB && containerInstance != null && containerInstance.ContainerType.AllowInternalSwap())
					{
						containerInstance.InternalSwap(request.InstanceIdA, request.InstanceIdB);
					}
					else
					{
						SymbolicLinkData? symbolicLinkData = null;
						SymbolicLinkData? symbolicLinkData2 = null;
						ArchetypeInstance archetypeInstance;
						ArchetypeInstance archetypeInstance2;
						if ((containerInstance.ContainerType.RequiresSymbolicLinkForPlacement() || containerInstance2.ContainerType.RequiresSymbolicLinkForPlacement()) && containerInstance.TryGetInstanceForInstanceId(request.InstanceIdA, out archetypeInstance) && containerInstance2.TryGetInstanceForInstanceId(request.InstanceIdB, out archetypeInstance2))
						{
							symbolicLinkData = archetypeInstance.GetSymbolicLinkData();
							symbolicLinkData2 = archetypeInstance2.GetSymbolicLinkData();
						}
						ArchetypeInstance archetypeInstance3 = containerInstance.Remove(request.InstanceIdA);
						ArchetypeInstance archetypeInstance4 = containerInstance2.Remove(request.InstanceIdB);
						int index = archetypeInstance4.Index;
						int index2 = archetypeInstance3.Index;
						archetypeInstance3.Index = index;
						archetypeInstance4.Index = index2;
						ContainerInstance containerInstance3 = containerInstance2;
						ContainerInstance containerInstance4 = containerInstance;
						if (symbolicLinkData != null)
						{
							if (symbolicLinkData.Value.PreviousContainer == containerInstance2)
							{
								archetypeInstance4.PreviousContainerInstance = symbolicLinkData.Value.PreviousContainer;
								archetypeInstance4.PreviousIndex = symbolicLinkData.Value.PreviousIndex;
							}
							else
							{
								containerInstance3 = symbolicLinkData.Value.PreviousContainer;
								archetypeInstance3.Index = symbolicLinkData.Value.PreviousIndex;
							}
						}
						if (symbolicLinkData2 != null)
						{
							if (symbolicLinkData2.Value.PreviousContainer == containerInstance)
							{
								archetypeInstance3.PreviousContainerInstance = symbolicLinkData2.Value.PreviousContainer;
								archetypeInstance3.PreviousIndex = symbolicLinkData2.Value.PreviousIndex;
							}
							else
							{
								containerInstance4 = symbolicLinkData2.Value.PreviousContainer;
								archetypeInstance4.Index = symbolicLinkData2.Value.PreviousIndex;
							}
						}
						containerInstance4.Add(archetypeInstance4, true);
						containerInstance3.Add(archetypeInstance3, true);
					}
					if (containerInstance.ContainerType == ContainerType.Abilities || containerInstance2.ContainerType == ContainerType.Abilities)
					{
						UIManager.InvokeAbilityContainerChanged();
						return;
					}
				}
				else
				{
					ArchetypeInstance archetypeInstance5;
					if (containerInstance.TryGetInstanceForInstanceId(request.InstanceIdA, out archetypeInstance5) && archetypeInstance5.InstanceUI != null)
					{
						archetypeInstance5.InstanceUI.ResetUI();
					}
					ArchetypeInstance archetypeInstance6;
					if (containerInstance2.TryGetInstanceForInstanceId(request.InstanceIdB, out archetypeInstance6) && archetypeInstance6.InstanceUI != null)
					{
						archetypeInstance6.InstanceUI.ResetUI();
					}
					string content = "Swap failed!";
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content);
				}
			}
		}

		// Token: 0x06004F6A RID: 20330 RVA: 0x001C912C File Offset: 0x001C732C
		public void ProcessTakeAllResponse(TakeAllRequest request, TakeAllResponse response)
		{
			ContainerInstance containerInstance;
			ContainerInstance containerInstance2;
			if (this.m_collections != null && this.m_collections.TryGetValue(response.SourceContainerId, out containerInstance) && this.m_collections.TryGetValue(ContainerType.Inventory.ToString(), out containerInstance2))
			{
				if (response.Op == OpCodes.Ok)
				{
					if (response.Items != null)
					{
						for (int i = 0; i < response.Items.Length; i++)
						{
							ArchetypeInstance archetypeInstance = containerInstance.Remove(response.Items[i].InstanceId);
							archetypeInstance.Index = response.Items[i].Index;
							archetypeInstance.InstanceUI.ToggleLock(false);
							ContainerInstance containerInstance3;
							if (this.m_collections.TryGetValue(response.Items[i].ContainerId, out containerInstance3))
							{
								containerInstance3.Add(archetypeInstance, true);
								if (containerInstance3.ContainerType == ContainerType.LostAndFound)
								{
									MessageManager.ChatQueue.AddToQueue(MessageType.Notification, archetypeInstance.Archetype.GetModifiedDisplayName(archetypeInstance) + " was moved to your Lost & Found!");
								}
							}
							else
							{
								MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Unknown transfer error!");
							}
						}
					}
					containerInstance2.AddCurrency(response.Currency);
					if (!GameManager.IsServer && response.Currency > 0UL)
					{
						MessageManager.ChatQueue.AddToQueue(MessageType.Notification, string.Concat(new string[]
						{
							response.Currency.AsCurrency(),
							" transferred from ",
							containerInstance.ContainerType.ToString(),
							" to ",
							containerInstance2.ContainerType.ToString(),
							"."
						}));
						ClientGameManager.UIManager.PlayMoneyAudio();
						return;
					}
				}
				else
				{
					string content = "Take all request failed!";
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content);
				}
			}
		}

		// Token: 0x06004F6B RID: 20331 RVA: 0x001C9308 File Offset: 0x001C7508
		public void ProcessItemDestructionResponse(ItemDestructionTransaction request)
		{
			if (request.Op != OpCodes.Ok)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"ItemDestructionRequest for ",
					request.InstanceId.ToString(),
					" failed with an OpCode = ",
					request.Op.ToString(),
					"!"
				}));
				return;
			}
			ContainerInstance containerInstance;
			ArchetypeInstance archetypeInstance;
			if (this.m_collections.TryGetValue(request.SourceContainer, out containerInstance) && containerInstance.TryGetInstanceForInstanceId(request.InstanceId, out archetypeInstance))
			{
				string modifiedDisplayName = archetypeInstance.Archetype.GetModifiedDisplayName(archetypeInstance);
				if (containerInstance.RemoveAndDestroy(archetypeInstance.InstanceId))
				{
					this.ItemRemovedLog(modifiedDisplayName, request.Context);
				}
				if (containerInstance.Count <= 0 && containerInstance.ContainerType == ContainerType.Loot)
				{
					IContainerUI containerUI = containerInstance.ContainerUI;
					if (containerUI == null)
					{
						return;
					}
					containerUI.CloseButtonPressed();
				}
			}
		}

		// Token: 0x06004F6C RID: 20332 RVA: 0x001C93E4 File Offset: 0x001C75E4
		public void ProcessItemMultiDestructionResponse(ItemMultiDestructionTransaction request)
		{
			if (request.Op != OpCodes.Ok)
			{
				Debug.LogWarning(string.Format("ItemMultiDestructionRequest for {0} item{1} failed with an OpCode = {2}!", request.Items.Length, (request.Items.Length != 1) ? "s" : string.Empty, request.Op));
				return;
			}
			foreach (ValueTuple<UniqueId, string> valueTuple in request.Items)
			{
				ContainerInstance containerInstance;
				ArchetypeInstance archetypeInstance;
				if (this.m_collections.TryGetValue(valueTuple.Item2, out containerInstance) && containerInstance.TryGetInstanceForInstanceId(valueTuple.Item1, out archetypeInstance))
				{
					string modifiedDisplayName = archetypeInstance.Archetype.GetModifiedDisplayName(archetypeInstance);
					if (containerInstance.RemoveAndDestroy(archetypeInstance.InstanceId))
					{
						this.ItemRemovedLog(modifiedDisplayName, request.Context);
					}
					if (containerInstance.Count <= 0 && containerInstance.ContainerType == ContainerType.Loot)
					{
						IContainerUI containerUI = containerInstance.ContainerUI;
						if (containerUI != null)
						{
							containerUI.CloseButtonPressed();
						}
					}
				}
			}
		}

		// Token: 0x06004F6D RID: 20333 RVA: 0x001C94D4 File Offset: 0x001C76D4
		public void ProcessItemAdded(ArchetypeAddedTransaction response)
		{
			if (response.Op != OpCodes.Ok)
			{
				Debug.LogWarning("ItemAdded failed with an OpCode = " + response.Op.ToString() + "!");
				return;
			}
			ContainerInstance containerInstance;
			if (this.m_collections.TryGetValue(response.TargetContainer, out containerInstance))
			{
				response.Instance.CreateItemInstanceUI();
				containerInstance.Add(response.Instance, true);
				this.ItemAddedLog(response.Instance, response);
				ItemAddContext context = response.Context;
				if (context - ItemAddContext.Loot <= 1 || context - ItemAddContext.Merchant <= 2)
				{
					UIManager.InvokeItemAddedToContainer(containerInstance.ContainerType);
				}
			}
		}

		// Token: 0x06004F6E RID: 20334 RVA: 0x001C956C File Offset: 0x001C776C
		public void ProcessAddRemoveItems(ArchetypeAddRemoveTransaction transaction)
		{
			if (transaction.Op != OpCodes.Ok)
			{
				Debug.LogWarning("AddRemoveItems failed with an OpCode = " + transaction.Op.ToString() + "!");
				return;
			}
			if (transaction.DestructionTransactions != null)
			{
				for (int i = 0; i < transaction.DestructionTransactions.Length; i++)
				{
					ItemDestructionTransaction itemDestructionTransaction = transaction.DestructionTransactions[i];
					ContainerInstance containerInstance;
					ArchetypeInstance archetypeInstance;
					if (this.m_collections.TryGetValue(itemDestructionTransaction.SourceContainer, out containerInstance) && containerInstance.TryGetInstanceForInstanceId(itemDestructionTransaction.InstanceId, out archetypeInstance))
					{
						bool isMastery = archetypeInstance.IsMastery;
						string modifiedDisplayName = archetypeInstance.Archetype.GetModifiedDisplayName(archetypeInstance);
						if (containerInstance.RemoveAndDestroy(archetypeInstance.InstanceId))
						{
							this.ItemRemovedLog(modifiedDisplayName, itemDestructionTransaction.Context);
							if (isMastery)
							{
								base.GameEntity.SkillsController.ClearAbilityToMasteryCache();
							}
						}
					}
				}
			}
			if (transaction.AddedTransactions != null)
			{
				for (int j = 0; j < transaction.AddedTransactions.Length; j++)
				{
					ArchetypeAddedTransaction archetypeAddedTransaction = transaction.AddedTransactions[j];
					ContainerInstance containerInstance2;
					if (this.m_collections.TryGetValue(archetypeAddedTransaction.TargetContainer, out containerInstance2))
					{
						archetypeAddedTransaction.Instance.CreateItemInstanceUI();
						containerInstance2.Add(archetypeAddedTransaction.Instance, true);
						this.ItemAddedLog(archetypeAddedTransaction.Instance, archetypeAddedTransaction);
					}
				}
			}
		}

		// Token: 0x06004F6F RID: 20335 RVA: 0x001C96B4 File Offset: 0x001C78B4
		public void ProcessLearnablesAdded(LearnablesAddedTransaction transaction)
		{
			if (transaction.Op != OpCodes.Ok)
			{
				Debug.LogWarning("LearnablesAdded failed with an OpCode = " + transaction.Op.ToString() + "!");
				return;
			}
			for (int i = 0; i < transaction.LearnableIds.Length; i++)
			{
				LearnableContainerInstance learnableContainerInstance;
				LearnableArchetype learnable;
				if (this.m_learnableCollections.TryGetValue(transaction.TargetContainer, out learnableContainerInstance) && !learnableContainerInstance.Contains(transaction.LearnableIds[i]) && InternalGameDatabase.Archetypes.TryGetAsType<LearnableArchetype>(transaction.LearnableIds[i], out learnable))
				{
					learnableContainerInstance.Add(learnable, true);
				}
			}
		}

		// Token: 0x06004F70 RID: 20336 RVA: 0x001C9750 File Offset: 0x001C7950
		public void ProcessSplitResponse(SplitRequest request, SplitResponse response)
		{
			if (response.Op != OpCodes.Ok)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"SplitRequest for ",
					request.InstanceId.ToString(),
					" failed with an OpCode = ",
					response.Op.ToString(),
					"!"
				}));
				return;
			}
			ContainerInstance containerInstance;
			ArchetypeInstance archetypeInstance;
			ContainerInstance containerInstance2;
			if (this.m_collections.TryGetValue(request.SourceContainer, out containerInstance) && containerInstance.TryGetInstanceForInstanceId(request.InstanceId, out archetypeInstance) && archetypeInstance.Archetype is IStackable && archetypeInstance.ItemData.Count != null && archetypeInstance.ItemData.Count.Value > request.SplitCount && base.TryGetInstance(response.TargetContainer, out containerInstance2))
			{
				archetypeInstance.ItemData.Count = archetypeInstance.ItemData.Count - request.SplitCount;
				response.Instance.CreateItemInstanceUI();
				containerInstance2.Add(response.Instance, true);
				if (containerInstance.ContainerType == ContainerType.TradeOutgoing)
				{
					ClientGameManager.UIManager.Trade.ResetTradeAccepted();
				}
			}
		}

		// Token: 0x06004F71 RID: 20337 RVA: 0x001C98B0 File Offset: 0x001C7AB0
		public void ProcessMergeResponse(MergeRequest request, MergeResponse response)
		{
			if (response.Op != OpCodes.Ok)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"MergeResponse for ",
					request.TransactionId.ToString(),
					" failed with an OpCode = ",
					response.Op.ToString(),
					"!"
				}));
				return;
			}
			bool flag = false;
			ContainerInstance containerInstance;
			ArchetypeInstance archetypeInstance;
			if (this.m_collections.TryGetValue(request.SourceContainer, out containerInstance) && containerInstance.TryGetInstanceForInstanceId(request.SourceInstanceId, out archetypeInstance))
			{
				flag = (containerInstance.ContainerType == ContainerType.Loot);
				if (flag)
				{
					this.ItemAddedLog(archetypeInstance, ItemAddContext.Loot);
				}
				containerInstance.RemoveAndDestroy(archetypeInstance.InstanceId);
				if (flag && containerInstance.Count <= 0)
				{
					IContainerUI containerUI = containerInstance.ContainerUI;
					if (containerUI != null)
					{
						containerUI.CloseButtonPressed();
					}
				}
			}
			else if (request.SourceContainer == ContainerType.Loot.ToString() && !string.IsNullOrEmpty(request.LocalSourceDisplayName))
			{
				this.ItemAddedLog(request.LocalSourceDisplayName, ItemAddContext.Loot, request.LocalSourceQuantity);
			}
			ContainerInstance containerInstance2;
			ArchetypeInstance archetypeInstance2;
			if (this.m_collections.TryGetValue(request.TargetContainer, out containerInstance2) && containerInstance2.TryGetInstanceForInstanceId(request.TargetInstanceId, out archetypeInstance2) && archetypeInstance2.ItemData != null)
			{
				int num = (archetypeInstance2.ItemData.Count != null) ? archetypeInstance2.ItemData.Count.Value : 0;
				archetypeInstance2.ItemData.Count = new int?(response.NewTargetCount);
				containerInstance2.InvokeQuantityOfItemChanged();
				if (containerInstance2.ContainerType == ContainerType.TradeOutgoing)
				{
					ClientGameManager.UIManager.Trade.ResetTradeAccepted();
				}
				if (flag)
				{
					int num2 = num;
					int? count = archetypeInstance2.ItemData.Count;
					if (num2 < count.GetValueOrDefault() & count != null)
					{
						UIManager.InvokeItemAddedToContainer(containerInstance2.ContainerType);
					}
				}
			}
		}

		// Token: 0x06004F72 RID: 20338 RVA: 0x001C9A90 File Offset: 0x001C7C90
		public void ProcessForfeitInventoryResponse(OpCodes op)
		{
			if (op != OpCodes.Ok)
			{
				Debug.LogWarning("ForfeitInventoryResponse returned " + op.ToString() + "!");
				return;
			}
			ContainerInstance containerInstance;
			if (this.m_collections.TryGetValue(ContainerType.Inventory.ToString(), out containerInstance))
			{
				containerInstance.DestroyContents();
			}
		}

		// Token: 0x06004F73 RID: 20339 RVA: 0x001C9AE8 File Offset: 0x001C7CE8
		public void ProcessCurrencyTransaction(CurrencyTransaction transaction)
		{
			ContainerInstance containerInstance;
			if (this.m_collections.TryGetValue(transaction.TargetContainer, out containerInstance))
			{
				string text;
				if (transaction.Add)
				{
					containerInstance.AddCurrency(transaction.Amount);
					text = "added to";
				}
				else
				{
					containerInstance.RemoveCurrency(transaction.Amount);
					text = "removed from";
				}
				MessageType type = MessageType.Notification;
				switch (transaction.Context)
				{
				case CurrencyContext.Loot:
					type = MessageType.Loot;
					break;
				case CurrencyContext.Quest:
					type = MessageType.Quest;
					break;
				case CurrencyContext.Post:
					type = MessageType.Loot;
					break;
				}
				MessageManager.ChatQueue.AddToQueue(type, string.Concat(new string[]
				{
					transaction.Amount.AsCurrency(),
					" ",
					text,
					" ",
					transaction.TargetContainer,
					" via ",
					transaction.Message
				}));
				ClientGameManager.UIManager.PlayMoneyAudio();
			}
		}

		// Token: 0x06004F74 RID: 20340 RVA: 0x001C9BDC File Offset: 0x001C7DDC
		public void ProcessMultiContainerCurrencyTransaction(MultiContainerCurrencyTransaction transaction)
		{
			foreach (CurrencyAdjustment currencyAdjustment in transaction.Adjustments)
			{
				ContainerInstance containerInstance;
				if (this.m_collections.TryGetValue(currencyAdjustment.TargetContainer, out containerInstance))
				{
					string text;
					if (currencyAdjustment.Add)
					{
						containerInstance.AddCurrency(currencyAdjustment.Amount);
						text = "added to";
					}
					else
					{
						containerInstance.RemoveCurrency(currencyAdjustment.Amount);
						text = "removed from";
					}
					MessageType type = MessageType.Notification;
					switch (transaction.Context)
					{
					case CurrencyContext.Loot:
						type = MessageType.Loot;
						break;
					case CurrencyContext.Quest:
						type = MessageType.Quest;
						break;
					case CurrencyContext.Post:
						type = MessageType.Loot;
						break;
					}
					MessageManager.ChatQueue.AddToQueue(type, string.Concat(new string[]
					{
						currencyAdjustment.Amount.AsCurrency(),
						" ",
						text,
						" ",
						currencyAdjustment.TargetContainer,
						" via ",
						transaction.Message
					}));
				}
			}
			ClientGameManager.UIManager.PlayMoneyAudio();
		}

		// Token: 0x06004F75 RID: 20341 RVA: 0x001C9CF8 File Offset: 0x001C7EF8
		public void ProcessInteractiveStationCurrencyTransaction(ulong? inventoryCurrency, ulong? personalBankCurrency)
		{
			ulong num = 0UL;
			if (this.m_inventory != null && inventoryCurrency != null)
			{
				num += this.m_inventory.Currency - inventoryCurrency.Value;
				this.m_inventory.ModifyCurrency(inventoryCurrency.Value);
			}
			if (this.m_personalBank != null && personalBankCurrency != null)
			{
				num += this.m_personalBank.Currency - personalBankCurrency.Value;
				this.m_personalBank.ModifyCurrency(personalBankCurrency.Value);
			}
			if (num > 0UL)
			{
				string str = this.m_interactionStation ? this.m_interactionStation.CurrencyRemovalMessage : string.Empty;
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, num.AsCurrency() + " removed " + str);
				ClientGameManager.UIManager.PlayMoneyAudio();
			}
		}

		// Token: 0x06004F76 RID: 20342 RVA: 0x001C9DC8 File Offset: 0x001C7FC8
		public void ProcessCurrencyTransferResponse(OpCodes op, CurrencyTransaction toRemove, CurrencyTransaction toAdd)
		{
			if (op != OpCodes.Ok)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Error with currency transaction request! " + op.ToString());
				return;
			}
			ContainerInstance containerInstance;
			ContainerInstance containerInstance2;
			if (this.m_collections.TryGetValue(toRemove.TargetContainer, out containerInstance) && this.m_collections.TryGetValue(toAdd.TargetContainer, out containerInstance2))
			{
				containerInstance.RemoveCurrency(toRemove.Amount);
				containerInstance2.AddCurrency(toAdd.Amount);
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, string.Concat(new string[]
				{
					toRemove.Amount.AsCurrency(),
					" transferred from ",
					containerInstance.ContainerType.ToString(),
					" to ",
					containerInstance2.ContainerType.ToString(),
					"."
				}));
				ClientGameManager.UIManager.PlayMoneyAudio();
			}
		}

		// Token: 0x06004F77 RID: 20343 RVA: 0x001C9EBC File Offset: 0x001C80BC
		public void ProcessCurrencyModifiedEvent(CurrencyModifyTransaction transaction)
		{
			ContainerInstance containerInstance;
			if (this.m_collections.TryGetValue(transaction.ContainerId, out containerInstance))
			{
				containerInstance.ModifyCurrency(transaction.Amount);
			}
		}

		// Token: 0x06004F78 RID: 20344 RVA: 0x001C9EEC File Offset: 0x001C80EC
		public void ProcessEventCurrencyTransaction(ulong? eventCurrency)
		{
			if (base.GameEntity && base.GameEntity.User != null)
			{
				UserRecord user = base.GameEntity.User;
				ulong num = (user.EventCurrency != null) ? user.EventCurrency.Value : 0UL;
				ulong num2 = (eventCurrency != null) ? eventCurrency.Value : 0UL;
				MessageType type = MessageType.Notification;
				string arg;
				ulong arg2;
				if (num2 < num)
				{
					arg = "used";
					arg2 = num - num2;
				}
				else
				{
					arg = "earned";
					arg2 = num2 - num;
					type = MessageType.Loot;
				}
				user.EventCurrency = eventCurrency;
				string arg3 = this.m_interactionStation ? this.m_interactionStation.CurrencyRemovalMessage : string.Empty;
				string content = ZString.Format<ulong, string, string, string>("{0} {1} {2} {3}", arg2, "Bloops", arg, arg3);
				MessageManager.ChatQueue.AddToQueue(type, content);
				if (ClientGameManager.UIManager)
				{
					ClientGameManager.UIManager.PlayEventCurrencyAudio();
					if (ClientGameManager.UIManager.StatPanel)
					{
						ClientGameManager.UIManager.StatPanel.UpdateEventCurrency();
					}
				}
				UIManager.InvokeEventCurrencyChanged();
			}
		}

		// Token: 0x06004F79 RID: 20345 RVA: 0x001CA008 File Offset: 0x001C8208
		public void ProcessModifyEquipmentAbsorbed(UniqueId instanceId, int absorbed)
		{
			ArchetypeInstance archetypeInstance;
			if (base.GameEntity && base.GameEntity.Vitals && this.m_equipment != null && this.m_equipment.TryGetInstanceForInstanceId(instanceId, out archetypeInstance) && archetypeInstance != null && archetypeInstance.ItemData != null && archetypeInstance.ItemData.Durability != null)
			{
				archetypeInstance.ItemData.Durability.Absorbed = absorbed;
				if (archetypeInstance.Archetype && archetypeInstance.Archetype is ShieldItem)
				{
					base.GameEntity.Vitals.RefreshShieldArmor();
				}
			}
		}

		// Token: 0x06004F7A RID: 20346 RVA: 0x001CA0A0 File Offset: 0x001C82A0
		public void ProcessItemCountUpdated(ItemCountUpdatedTransaction transaction)
		{
			ContainerInstance containerInstance;
			ArchetypeInstance archetypeInstance;
			if (this.m_collections.TryGetValue(transaction.Container, out containerInstance) && containerInstance.TryGetInstanceForInstanceId(transaction.InstanceId, out archetypeInstance))
			{
				int? count = archetypeInstance.ItemData.Count;
				archetypeInstance.ItemData.Count = new int?(transaction.NewCount);
				containerInstance.InvokeQuantityOfItemChanged();
				int? num = count;
				int? count2 = archetypeInstance.ItemData.Count;
				if (num.GetValueOrDefault() < count2.GetValueOrDefault() & (num != null & count2 != null))
				{
					UIManager.InvokeItemAddedToContainer(containerInstance.ContainerType);
				}
			}
		}

		// Token: 0x06004F7B RID: 20347 RVA: 0x001CA134 File Offset: 0x001C8334
		public void ProcessMerchantItemSellResponse(OpCodes op, UniqueId itemInstanceId, ContainerType sourceContainerType, ulong sellPrice)
		{
			ContainerInstance containerInstance;
			ArchetypeInstance archetypeInstance;
			if (op == OpCodes.Ok && base.TryGetInstance(sourceContainerType, out containerInstance) && containerInstance.TryGetInstanceForInstanceId(itemInstanceId, out archetypeInstance))
			{
				ContainerInstance containerInstance2 = base.GameEntity.IsMissingBag ? this.m_personalBank : this.m_inventory;
				string content = (containerInstance2 != null) ? ZString.Format<string, string, string>("{0} <i>added</i> to your {1} from the sale of {2}.", sellPrice.AsCurrency(), containerInstance2.GetHeaderString(), archetypeInstance.Archetype.GetModifiedDisplayName(archetypeInstance)) : ZString.Format<string, string>("{0} <i>added</i> from the sale of {1}.", sellPrice.AsCurrency(), archetypeInstance.Archetype.GetModifiedDisplayName(archetypeInstance));
				if (containerInstance.RemoveAndDestroy(itemInstanceId))
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content);
					if (containerInstance2 != null)
					{
						containerInstance2.AddCurrency(sellPrice);
					}
					ClientGameManager.UIManager.PlayMoneyAudio();
				}
			}
		}

		// Token: 0x06004F7C RID: 20348 RVA: 0x001CA1F4 File Offset: 0x001C83F4
		public void ProcessBlacksmithItemRepairResponse(OpCodes op, UniqueId itemInstanceId, ContainerType sourceContainerType)
		{
			ContainerInstance containerInstance;
			ArchetypeInstance archetypeInstance;
			if (op == OpCodes.Ok && base.TryGetInstance(sourceContainerType, out containerInstance) && containerInstance.TryGetInstanceForInstanceId(itemInstanceId, out archetypeInstance) && archetypeInstance.IsItem && archetypeInstance.ItemData.Durability != null)
			{
				archetypeInstance.ItemData.Durability.RepairItem();
				ClientGameManager.UIManager.PlayRandomClip(GlobalSettings.Values.Audio.RepairModeClipCollection, null);
				containerInstance.InvokeItemRepaired();
			}
		}

		// Token: 0x06004F7D RID: 20349 RVA: 0x001CA268 File Offset: 0x001C8468
		public void ProcessBlacksmithContainerRepairResponse(OpCodes op, ContainerType sourceContainerType)
		{
			ContainerInstance containerInstance;
			if (op == OpCodes.Ok && base.TryGetInstance(sourceContainerType, out containerInstance))
			{
				if (sourceContainerType == ContainerType.Equipment)
				{
					EquippedRepairIcon.PauseRefresh = true;
				}
				for (int i = 0; i < containerInstance.Count; i++)
				{
					ArchetypeInstance instanceForListIndex = containerInstance.GetInstanceForListIndex(i);
					if (instanceForListIndex != null && instanceForListIndex.IsItem && instanceForListIndex.ItemData.Durability != null)
					{
						instanceForListIndex.ItemData.Durability.RepairItem();
					}
				}
				ClientGameManager.UIManager.PlayRandomClip(GlobalSettings.Values.Audio.RepairModeClipCollection, null);
				containerInstance.InvokeItemRepaired();
				if (sourceContainerType == ContainerType.Equipment)
				{
					EquippedRepairIcon.PauseRefresh = false;
					if (UIManager.EquippedRepairIcon)
					{
						UIManager.EquippedRepairIcon.ForceRefreshRepairIcon();
					}
				}
			}
		}

		// Token: 0x06004F7E RID: 20350 RVA: 0x001CA320 File Offset: 0x001C8520
		public void UpdateArchetypeInstanceLock(string containerId, UniqueId instanceId, bool lockState)
		{
			ContainerInstance containerInstance;
			ArchetypeInstance archetypeInstance;
			if (base.TryGetInstance(containerId, out containerInstance) && containerInstance.TryGetInstanceForInstanceId(instanceId, out archetypeInstance) && archetypeInstance.IsItem)
			{
				archetypeInstance.ItemData.Locked = lockState;
			}
		}

		// Token: 0x06004F7F RID: 20351 RVA: 0x001CA358 File Offset: 0x001C8558
		public void ProcessTrainSpecializationResponse(OpCodes op, UniqueId masteryInstanceId, UniqueId specializationArchetypeId, float specLevel)
		{
			ArchetypeInstance archetypeInstance;
			if (op != OpCodes.Ok || !this.m_masteries.TryGetInstanceForInstanceId(masteryInstanceId, out archetypeInstance) || archetypeInstance.MasteryData == null)
			{
				return;
			}
			archetypeInstance.MasteryData.Specialization = new UniqueId?(specializationArchetypeId);
			archetypeInstance.MasteryData.SpecializationLevel = specLevel;
			base.GameEntity.Vitals.RefreshStats();
		}

		// Token: 0x06004F80 RID: 20352 RVA: 0x001CA3B0 File Offset: 0x001C85B0
		public void ProcessForgetSpecializationResponse(OpCodes op, UniqueId masteryInstanceId, UniqueId specializationArchetypeId)
		{
			ArchetypeInstance archetypeInstance;
			if (op != OpCodes.Ok || !this.m_masteries.TryGetInstanceForInstanceId(masteryInstanceId, out archetypeInstance) || archetypeInstance.MasteryData == null)
			{
				return;
			}
			archetypeInstance.MasteryData.Specialization = null;
			this.m_masteries.InvokeContentsChanged();
			base.GameEntity.Vitals.RefreshStats();
		}

		// Token: 0x06004F81 RID: 20353 RVA: 0x001CA40C File Offset: 0x001C860C
		public void ProcessContainerExpansionResponse(PurchaseContainerExpansionTransaction transaction)
		{
			bool flag = false;
			ContainerInstance containerInstance;
			if (transaction.Op != OpCodes.Ok)
			{
				Debug.LogWarning("Error in PurchaseAdditionalSpaceRequest!");
			}
			else if (this.m_collections.TryGetValue(transaction.ContainerId, out containerInstance))
			{
				containerInstance.ExpansionPurchased();
				if (transaction.CurrencyToRemoveFromContainer != null)
				{
					containerInstance.RemoveCurrency(transaction.CurrencyToRemoveFromContainer.Value);
				}
				if (transaction.CurrencyToRemoveFromInventory != null)
				{
					this.m_inventory.RemoveCurrency(transaction.CurrencyToRemoveFromInventory.Value);
				}
				flag = true;
			}
			if (ClientGameManager.UIManager != null)
			{
				if (flag)
				{
					ClientGameManager.UIManager.PlayMoneyAudio();
				}
				if (ClientGameManager.UIManager.RemoteInventory != null)
				{
					ClientGameManager.UIManager.RemoteInventory.RefreshPurchaseContainerExpansionUI(flag);
				}
				if (ClientGameManager.UIManager.PersonalBankUI != null && ClientGameManager.UIManager.PersonalBankUI.OutgoingUI != null)
				{
					ClientGameManager.UIManager.PersonalBankUI.OutgoingUI.RefreshPurchaseContainerExpansionUI(flag);
				}
			}
		}

		// Token: 0x06004F82 RID: 20354 RVA: 0x001CA50C File Offset: 0x001C870C
		private void ApplyItemFlagsForTransfer(ContainerInstance sourceContainerInstance, ArchetypeInstance archetypeInstance)
		{
			InteractiveRegenerativeChest interactiveRegenerativeChest;
			if (sourceContainerInstance.Interactive != null && sourceContainerInstance.Interactive.TryGetAsType(out interactiveRegenerativeChest) && interactiveRegenerativeChest.ItemFlagsToSet != ItemFlags.None)
			{
				archetypeInstance.AddItemFlag(interactiveRegenerativeChest.ItemFlagsToSet, base.Record);
			}
		}

		// Token: 0x06004F83 RID: 20355 RVA: 0x000755A0 File Offset: 0x000737A0
		private void ItemAddedLog(ArchetypeInstance instance, ArchetypeAddedTransaction transaction)
		{
			this.ItemAddedLog(instance, transaction.Context);
		}

		// Token: 0x06004F84 RID: 20356 RVA: 0x001CA54C File Offset: 0x001C874C
		private void ItemAddedLog(ArchetypeInstance instance, ItemAddContext context)
		{
			if (instance != null && instance.Archetype != null)
			{
				string modifiedDisplayName = instance.Archetype.GetModifiedDisplayName(instance);
				int? quantity = (instance.ItemData != null && instance.ItemData.Quantity != null) ? new int?(instance.ItemData.Quantity.Value) : null;
				this.ItemAddedLog(modifiedDisplayName, context, quantity);
			}
		}

		// Token: 0x06004F85 RID: 20357 RVA: 0x001CA5C4 File Offset: 0x001C87C4
		private void ItemAddedLog(string itemName, ItemAddContext context, int? quantity = null)
		{
			string text = "";
			string str = (quantity != null) ? (" (" + quantity.Value.ToString() + ")") : string.Empty;
			MessageType type = MessageType.Notification;
			switch (context)
			{
			case ItemAddContext.Loot:
				text = itemName + str + " looted";
				type = MessageType.Loot;
				break;
			case ItemAddContext.Quest:
				text = itemName + str + " added via quest";
				type = MessageType.Quest;
				break;
			case ItemAddContext.Crafting:
				text = itemName + str + " crafted";
				break;
			case ItemAddContext.Forage:
				text = itemName + str + " found via foraging";
				type = MessageType.Loot;
				break;
			case ItemAddContext.Merchant:
				text = itemName + str + " added via merchant purchase";
				type = MessageType.Loot;
				break;
			case ItemAddContext.Training:
				text = itemName + str + " added via training";
				type = MessageType.Skills;
				break;
			case ItemAddContext.Post:
				text = itemName + str + " added via post";
				type = MessageType.Loot;
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				MessageManager.ChatQueue.AddToQueue(type, text);
			}
		}

		// Token: 0x06004F86 RID: 20358 RVA: 0x001CA6D0 File Offset: 0x001C88D0
		private void ItemRemovedLog(string itemName, ItemDestructionContext context)
		{
			string text = "";
			MessageType type = MessageType.Notification;
			switch (context)
			{
			case ItemDestructionContext.Charges:
				text = itemName + " destroyed due to no charges!";
				break;
			case ItemDestructionContext.Count:
				text = itemName + " destroyed due to a count of 0!";
				break;
			case ItemDestructionContext.Request:
				text = itemName + " destroyed due to a request!";
				break;
			case ItemDestructionContext.Quest:
				text = itemName + " removed due to a quest!";
				type = MessageType.Quest;
				break;
			case ItemDestructionContext.Durability:
				text = itemName + " destroyed due to 0 durability!";
				break;
			case ItemDestructionContext.Consumption:
				text = itemName + " is consumed!";
				break;
			case ItemDestructionContext.Post:
				text = itemName + " sent via post!";
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				MessageManager.ChatQueue.AddToQueue(type, text);
			}
		}

		// Token: 0x040047E7 RID: 18407
		public const int kGetFirstAvailableTargetIndexValue = -1;

		// Token: 0x040047E8 RID: 18408
		private static HashSet<int> m_cachedIndexes = null;

		// Token: 0x040047EA RID: 18410
		public bool m_subscribedToAbilityContentsChanged;

		// Token: 0x040047EB RID: 18411
		private EmberStone m_emberStone;

		// Token: 0x040047EF RID: 18415
		private const int kExcludeKillReminderCount = 20;

		// Token: 0x040047F0 RID: 18416
		private static string kExcludeMessage = null;

		// Token: 0x040047F1 RID: 18417
		private int m_excludeValleyCount;

		// Token: 0x040047F2 RID: 18418
		private const string kLootTemplate = "{@EventType} {@InstanceId} {@Item} looted by {@CharacterId} {@CharacterName} in {@Zone}";

		// Token: 0x040047F3 RID: 18419
		private static object[] LootObjectArray = new object[6];

		// Token: 0x040047F4 RID: 18420
		private static object[] m_buybackArguments = null;

		// Token: 0x040047F5 RID: 18421
		private static object[] m_sellArguments = null;

		// Token: 0x040047F6 RID: 18422
		private static object[] m_buyExpansionArguments = null;

		// Token: 0x040047F7 RID: 18423
		private static object[] m_blacksmithLogArguments = null;
	}
}
