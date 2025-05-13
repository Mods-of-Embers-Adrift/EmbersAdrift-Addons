using System;
using System.Collections.Generic;
using SoL.Game.AuctionHouse;
using SoL.Game.Crafting;
using SoL.Game.HuntingLog;
using SoL.Game.Interactives;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Quests;
using SoL.Game.UI;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Utilities;

namespace SoL.Game.Objects.Containers
{
	// Token: 0x02000A06 RID: 2566
	public abstract class BaseCollectionController : GameEntityComponent, ICollectionController
	{
		// Token: 0x140000F3 RID: 243
		// (add) Token: 0x06004DDA RID: 19930 RVA: 0x001C0F40 File Offset: 0x001BF140
		// (remove) Token: 0x06004DDB RID: 19931 RVA: 0x001C0F74 File Offset: 0x001BF174
		public static event Action InteractiveStationChanged;

		// Token: 0x17001130 RID: 4400
		// (get) Token: 0x06004DDC RID: 19932 RVA: 0x00074B3D File Offset: 0x00072D3D
		public static object[] InvalidInstanceArguments
		{
			get
			{
				if (BaseCollectionController.m_invalidInstanceArguments == null)
				{
					BaseCollectionController.m_invalidInstanceArguments = new object[5];
				}
				return BaseCollectionController.m_invalidInstanceArguments;
			}
		}

		// Token: 0x17001131 RID: 4401
		// (get) Token: 0x06004DDD RID: 19933 RVA: 0x00074B56 File Offset: 0x00072D56
		protected static object[] InvalidIndexArguments
		{
			get
			{
				if (BaseCollectionController.m_invalidIndexArguments == null)
				{
					BaseCollectionController.m_invalidIndexArguments = new object[5];
				}
				return BaseCollectionController.m_invalidIndexArguments;
			}
		}

		// Token: 0x17001132 RID: 4402
		// (get) Token: 0x06004DDE RID: 19934
		protected abstract GameEntityType EntityType { get; }

		// Token: 0x140000F4 RID: 244
		// (add) Token: 0x06004DDF RID: 19935 RVA: 0x001C0FA8 File Offset: 0x001BF1A8
		// (remove) Token: 0x06004DE0 RID: 19936 RVA: 0x001C0FE0 File Offset: 0x001BF1E0
		public event Action EmberStoneChanged;

		// Token: 0x17001133 RID: 4403
		// (get) Token: 0x06004DE1 RID: 19937 RVA: 0x00049FFA File Offset: 0x000481FA
		// (set) Token: 0x06004DE2 RID: 19938 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual EmberStone CurrentEmberStone
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x06004DE3 RID: 19939 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual int GetEmberEssenceCount()
		{
			return 0;
		}

		// Token: 0x06004DE4 RID: 19940 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void AdjustEmberEssenceCount(int delta)
		{
		}

		// Token: 0x06004DE5 RID: 19941 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void AdjustTravelEssenceCount(int delta)
		{
		}

		// Token: 0x06004DE6 RID: 19942 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void PurchaseTravelEssence(int travelToAdd, int essenceCost)
		{
		}

		// Token: 0x06004DE7 RID: 19943 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual int GetAvailableEmberEssenceForTravel()
		{
			return 0;
		}

		// Token: 0x06004DE8 RID: 19944 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual int GetDisplayValueForTravelEssence()
		{
			return 0;
		}

		// Token: 0x06004DE9 RID: 19945 RVA: 0x00074B6F File Offset: 0x00072D6F
		public virtual ValueTuple<int, int> GetEmberAndTravelEssenceCounts()
		{
			return new ValueTuple<int, int>(0, 0);
		}

		// Token: 0x06004DEA RID: 19946 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void SetEmberEssenceCountForTravel(int updatedCount, int updatedTravelCount)
		{
		}

		// Token: 0x06004DEB RID: 19947 RVA: 0x00074B78 File Offset: 0x00072D78
		protected void InvokeEmberStoneChangedEvent()
		{
			Action emberStoneChanged = this.EmberStoneChanged;
			if (emberStoneChanged == null)
			{
				return;
			}
			emberStoneChanged();
		}

		// Token: 0x140000F5 RID: 245
		// (add) Token: 0x06004DEC RID: 19948 RVA: 0x001C1018 File Offset: 0x001BF218
		// (remove) Token: 0x06004DED RID: 19949 RVA: 0x001C1050 File Offset: 0x001BF250
		public event Action HuntingLogUpdated;

		// Token: 0x06004DEE RID: 19950 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void InvokeHuntingLogEntryRemoved()
		{
		}

		// Token: 0x06004DEF RID: 19951 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void InvokeHuntingLogEntryModified()
		{
		}

		// Token: 0x06004DF0 RID: 19952 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool IncrementHuntingLog(HuntingLogProfile profile, int npcLevel)
		{
			return false;
		}

		// Token: 0x06004DF1 RID: 19953 RVA: 0x00074B8A File Offset: 0x00072D8A
		protected void InvokeHuntingLogUpdatedEvent()
		{
			Action huntingLogUpdated = this.HuntingLogUpdated;
			if (huntingLogUpdated == null)
			{
				return;
			}
			huntingLogUpdated();
		}

		// Token: 0x17001134 RID: 4404
		// (get) Token: 0x06004DF2 RID: 19954 RVA: 0x00074B9C File Offset: 0x00072D9C
		public List<BBTask> LocalTaskDiscard
		{
			get
			{
				if (this.m_localTaskDiscard == null)
				{
					this.m_localTaskDiscard = StaticListPool<BBTask>.GetFromPool();
				}
				return this.m_localTaskDiscard;
			}
		}

		// Token: 0x17001135 RID: 4405
		// (get) Token: 0x06004DF3 RID: 19955 RVA: 0x00074BB7 File Offset: 0x00072DB7
		public CharacterRecord Record
		{
			get
			{
				return this.m_record;
			}
		}

		// Token: 0x17001136 RID: 4406
		// (get) Token: 0x06004DF4 RID: 19956 RVA: 0x00074BBF File Offset: 0x00072DBF
		// (set) Token: 0x06004DF5 RID: 19957 RVA: 0x00074BC7 File Offset: 0x00072DC7
		public UniqueId? TradeId
		{
			get
			{
				return this.m_tradeId;
			}
			set
			{
				this.m_tradeId = value;
			}
		}

		// Token: 0x06004DF6 RID: 19958 RVA: 0x00074BD0 File Offset: 0x00072DD0
		private void Awake()
		{
			base.GameEntity.CollectionController = this;
		}

		// Token: 0x06004DF7 RID: 19959 RVA: 0x001C1088 File Offset: 0x001BF288
		protected virtual void OnDestroy()
		{
			if (this.m_collections != null)
			{
				foreach (KeyValuePair<string, ContainerInstance> keyValuePair in this.m_collections)
				{
					if (keyValuePair.Value != null)
					{
						if (this.EntityType == GameEntityType.Player)
						{
							keyValuePair.Value.SaveRecord();
						}
						keyValuePair.Value.OnDestroy();
					}
				}
				this.m_collections.Clear();
				this.m_collections = null;
			}
			if (this.m_learnableCollections != null)
			{
				foreach (KeyValuePair<string, LearnableContainerInstance> keyValuePair2 in this.m_learnableCollections)
				{
					LearnableContainerInstance value = keyValuePair2.Value;
					if (value != null)
					{
						value.Clear();
					}
				}
				this.m_learnableCollections.Clear();
				this.m_learnableCollections = null;
			}
			this.m_masteries = null;
			this.m_abilities = null;
			this.m_equipment = null;
			this.m_inventory = null;
			this.m_pouch = null;
			this.m_reagentPouch = null;
			this.m_remoteContainer = null;
			this.m_personalBank = null;
			this.m_lostAndFound = null;
			LearnableContainerInstance recipes = this.m_recipes;
			if (recipes != null)
			{
				recipes.Clear();
			}
			this.m_recipes = null;
			LearnableContainerInstance emotes = this.m_emotes;
			if (emotes != null)
			{
				emotes.Clear();
			}
			this.m_emotes = null;
			LearnableContainerInstance titles = this.m_titles;
			if (titles != null)
			{
				titles.Clear();
			}
			this.m_titles = null;
			if (this.m_localTaskDiscard != null)
			{
				StaticListPool<BBTask>.ReturnToPool(this.m_localTaskDiscard);
				this.m_localTaskDiscard = null;
			}
		}

		// Token: 0x06004DF8 RID: 19960 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void ValidateContainers(CharacterRecord record)
		{
		}

		// Token: 0x06004DF9 RID: 19961 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void ValidateContainerContents(CharacterRecord record)
		{
		}

		// Token: 0x06004DFA RID: 19962 RVA: 0x001C1220 File Offset: 0x001BF420
		private void Init(CharacterRecord record)
		{
			this.m_record = record;
			this.m_collections = new Dictionary<string, ContainerInstance>();
			this.ValidateContainers(record);
			this.ValidateContainerContents(record);
			foreach (KeyValuePair<ContainerType, ContainerRecord> keyValuePair in record.Storage)
			{
				IContainerUI containerUI = null;
				if (!GameManager.IsServer && base.GameEntity.Type == GameEntityType.Player)
				{
					containerUI = ClientGameManager.UIManager.GetContainerUI(keyValuePair.Key);
				}
				ContainerInstance containerInstance = new ContainerInstance(this, keyValuePair.Value, containerUI);
				this.m_collections.Add(containerInstance.Id, containerInstance);
				switch (keyValuePair.Key)
				{
				case ContainerType.Equipment:
					this.m_equipment = containerInstance;
					break;
				case ContainerType.Inventory:
					this.m_inventory = containerInstance;
					break;
				case ContainerType.Pouch:
					this.m_pouch = containerInstance;
					break;
				case ContainerType.ReagentPouch:
					this.m_reagentPouch = containerInstance;
					break;
				case ContainerType.PersonalBank:
					this.m_personalBank = containerInstance;
					break;
				case ContainerType.Gathering:
					this.m_gathering = containerInstance;
					break;
				case ContainerType.LostAndFound:
					this.m_lostAndFound = containerInstance;
					break;
				case ContainerType.Masteries:
					this.m_masteries = containerInstance;
					break;
				case ContainerType.Abilities:
					this.m_abilities = containerInstance;
					break;
				}
			}
			this.InitInternal();
		}

		// Token: 0x06004DFB RID: 19963 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void InitInternal()
		{
		}

		// Token: 0x06004DFC RID: 19964 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void UpdateOutgoingCurrency(ContainerType containerType)
		{
		}

		// Token: 0x06004DFD RID: 19965 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void RefreshBuybackItems()
		{
		}

		// Token: 0x06004DFE RID: 19966 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void ModifyEventCurrency(ulong delta, bool removing)
		{
		}

		// Token: 0x06004DFF RID: 19967 RVA: 0x00074BDE File Offset: 0x00072DDE
		public bool TryGetInstance(string containerId, out ContainerInstance containerInstance)
		{
			if (this.m_collections == null)
			{
				containerInstance = null;
				return false;
			}
			return this.m_collections.TryGetValue(containerId, out containerInstance);
		}

		// Token: 0x06004E00 RID: 19968 RVA: 0x00074BFA File Offset: 0x00072DFA
		public bool TryGetInstance(ContainerType containerType, out ContainerInstance containerInstance)
		{
			if (this.m_collections == null)
			{
				containerInstance = null;
				return false;
			}
			return this.m_collections.TryGetValue(containerType.ToString(), out containerInstance);
		}

		// Token: 0x06004E01 RID: 19969 RVA: 0x00074C22 File Offset: 0x00072E22
		public bool TryGetLearnableInstance(string containerId, out LearnableContainerInstance containerInstance)
		{
			if (this.m_learnableCollections == null)
			{
				containerInstance = null;
				return false;
			}
			return this.m_learnableCollections.TryGetValue(containerId, out containerInstance);
		}

		// Token: 0x06004E02 RID: 19970 RVA: 0x00074C3E File Offset: 0x00072E3E
		public bool TryGetLearnableInstance(ContainerType containerType, out LearnableContainerInstance containerInstance)
		{
			if (this.m_learnableCollections == null)
			{
				containerInstance = null;
				return false;
			}
			return this.m_learnableCollections.TryGetValue(containerType.ToString(), out containerInstance);
		}

		// Token: 0x17001137 RID: 4407
		// (get) Token: 0x06004E03 RID: 19971 RVA: 0x00074C66 File Offset: 0x00072E66
		// (set) Token: 0x06004E04 RID: 19972 RVA: 0x001C1374 File Offset: 0x001BF574
		BaseNetworkInteractiveStation ICollectionController.InteractiveStation
		{
			get
			{
				return this.m_interactionStation;
			}
			set
			{
				BaseNetworkInteractiveStation interactionStation = this.m_interactionStation;
				this.m_interactionStation = value;
				Action interactiveStationChanged = BaseCollectionController.InteractiveStationChanged;
				if (interactiveStationChanged != null)
				{
					interactiveStationChanged();
				}
				if (GameManager.IsServer && ServerGameManager.AuctionHouseManager)
				{
					bool flag = interactionStation != null && interactionStation is InteractiveAuctionHouse;
					bool flag2 = this.m_interactionStation != null && this.m_interactionStation is InteractiveAuctionHouse;
					if (flag || flag2)
					{
						if (flag2)
						{
							ServerGameManager.AuctionHouseManager.AddListener(base.GameEntity);
							return;
						}
						ServerGameManager.AuctionHouseManager.RemoveListener(base.GameEntity);
					}
				}
			}
		}

		// Token: 0x17001138 RID: 4408
		// (get) Token: 0x06004E05 RID: 19973 RVA: 0x00074C6E File Offset: 0x00072E6E
		// (set) Token: 0x06004E06 RID: 19974 RVA: 0x00074C76 File Offset: 0x00072E76
		InteractiveRefinementStation ICollectionController.RefinementStation
		{
			get
			{
				return this.m_refinementStation;
			}
			set
			{
				this.m_refinementStation = value;
			}
		}

		// Token: 0x17001139 RID: 4409
		// (get) Token: 0x06004E07 RID: 19975 RVA: 0x00074C7F File Offset: 0x00072E7F
		// (set) Token: 0x06004E08 RID: 19976 RVA: 0x00074C87 File Offset: 0x00072E87
		IGatheringNode ICollectionController.GatheringNode
		{
			get
			{
				return this.m_gatheringNode;
			}
			set
			{
				this.m_gatheringNode = value;
			}
		}

		// Token: 0x1700113A RID: 4410
		// (get) Token: 0x06004E09 RID: 19977 RVA: 0x00074C90 File Offset: 0x00072E90
		ContainerInstance ICollectionController.Masteries
		{
			get
			{
				return this.m_masteries;
			}
		}

		// Token: 0x1700113B RID: 4411
		// (get) Token: 0x06004E0A RID: 19978 RVA: 0x00074C98 File Offset: 0x00072E98
		ContainerInstance ICollectionController.Abilities
		{
			get
			{
				return this.m_abilities;
			}
		}

		// Token: 0x1700113C RID: 4412
		// (get) Token: 0x06004E0B RID: 19979 RVA: 0x00074CA0 File Offset: 0x00072EA0
		ContainerInstance ICollectionController.Equipment
		{
			get
			{
				return this.m_equipment;
			}
		}

		// Token: 0x1700113D RID: 4413
		// (get) Token: 0x06004E0C RID: 19980 RVA: 0x00074CA8 File Offset: 0x00072EA8
		ContainerInstance ICollectionController.Inventory
		{
			get
			{
				return this.m_inventory;
			}
		}

		// Token: 0x1700113E RID: 4414
		// (get) Token: 0x06004E0D RID: 19981 RVA: 0x00074CB0 File Offset: 0x00072EB0
		ContainerInstance ICollectionController.Gathering
		{
			get
			{
				return this.m_gathering;
			}
		}

		// Token: 0x1700113F RID: 4415
		// (get) Token: 0x06004E0E RID: 19982 RVA: 0x00074CB8 File Offset: 0x00072EB8
		ContainerInstance ICollectionController.Pouch
		{
			get
			{
				return this.m_pouch;
			}
		}

		// Token: 0x17001140 RID: 4416
		// (get) Token: 0x06004E0F RID: 19983 RVA: 0x00074CC0 File Offset: 0x00072EC0
		ContainerInstance ICollectionController.ReagentPouch
		{
			get
			{
				return this.m_reagentPouch;
			}
		}

		// Token: 0x17001141 RID: 4417
		// (get) Token: 0x06004E10 RID: 19984 RVA: 0x00074CC8 File Offset: 0x00072EC8
		ContainerInstance ICollectionController.PersonalBank
		{
			get
			{
				return this.m_personalBank;
			}
		}

		// Token: 0x17001142 RID: 4418
		// (get) Token: 0x06004E11 RID: 19985 RVA: 0x00074CD0 File Offset: 0x00072ED0
		ContainerInstance ICollectionController.LostAndFound
		{
			get
			{
				return this.m_lostAndFound;
			}
		}

		// Token: 0x17001143 RID: 4419
		// (get) Token: 0x06004E12 RID: 19986 RVA: 0x00074CD8 File Offset: 0x00072ED8
		ContainerInstance ICollectionController.RemoteContainer
		{
			get
			{
				return this.m_remoteContainer;
			}
		}

		// Token: 0x17001144 RID: 4420
		// (get) Token: 0x06004E13 RID: 19987 RVA: 0x00074CE0 File Offset: 0x00072EE0
		LearnableContainerInstance ICollectionController.Recipes
		{
			get
			{
				return this.m_recipes;
			}
		}

		// Token: 0x17001145 RID: 4421
		// (get) Token: 0x06004E14 RID: 19988 RVA: 0x00074CE8 File Offset: 0x00072EE8
		LearnableContainerInstance ICollectionController.Emotes
		{
			get
			{
				return this.m_emotes;
			}
		}

		// Token: 0x17001146 RID: 4422
		// (get) Token: 0x06004E15 RID: 19989 RVA: 0x00074CF0 File Offset: 0x00072EF0
		LearnableContainerInstance ICollectionController.Titles
		{
			get
			{
				return this.m_titles;
			}
		}

		// Token: 0x17001147 RID: 4423
		// (get) Token: 0x06004E16 RID: 19990 RVA: 0x00074BB7 File Offset: 0x00072DB7
		CharacterRecord ICollectionController.Record
		{
			get
			{
				return this.m_record;
			}
		}

		// Token: 0x17001148 RID: 4424
		// (get) Token: 0x06004E17 RID: 19991 RVA: 0x00074BBF File Offset: 0x00072DBF
		// (set) Token: 0x06004E18 RID: 19992 RVA: 0x00074BC7 File Offset: 0x00072DC7
		UniqueId? ICollectionController.TradeId
		{
			get
			{
				return this.m_tradeId;
			}
			set
			{
				this.m_tradeId = value;
			}
		}

		// Token: 0x06004E19 RID: 19993 RVA: 0x00074CF8 File Offset: 0x00072EF8
		bool ICollectionController.TryGetInstance(string containerId, out ContainerInstance containerInstance)
		{
			return this.TryGetInstance(containerId, out containerInstance);
		}

		// Token: 0x06004E1A RID: 19994 RVA: 0x00074D02 File Offset: 0x00072F02
		bool ICollectionController.TryGetInstance(ContainerType containerType, out ContainerInstance containerInstance)
		{
			return this.TryGetInstance(containerType, out containerInstance);
		}

		// Token: 0x06004E1B RID: 19995 RVA: 0x00074D0C File Offset: 0x00072F0C
		bool ICollectionController.TryGetLearnableInstance(string containerId, out LearnableContainerInstance containerInstance)
		{
			return this.TryGetLearnableInstance(containerId, out containerInstance);
		}

		// Token: 0x06004E1C RID: 19996 RVA: 0x00074D16 File Offset: 0x00072F16
		bool ICollectionController.TryGetLearnableInstance(ContainerType containerType, out LearnableContainerInstance containerInstance)
		{
			return this.TryGetLearnableInstance(containerType, out containerInstance);
		}

		// Token: 0x06004E1D RID: 19997 RVA: 0x00074D20 File Offset: 0x00072F20
		void ICollectionController.Initialize(CharacterRecord record)
		{
			this.Init(record);
		}

		// Token: 0x06004E1E RID: 19998 RVA: 0x00074D29 File Offset: 0x00072F29
		void ICollectionController.UpdateOutgoingCurrency(ContainerType containerType)
		{
			this.UpdateOutgoingCurrency(containerType);
		}

		// Token: 0x06004E1F RID: 19999 RVA: 0x00074D32 File Offset: 0x00072F32
		void ICollectionController.RefreshBuybackItems()
		{
			this.RefreshBuybackItems();
		}

		// Token: 0x06004E20 RID: 20000 RVA: 0x00074D3A File Offset: 0x00072F3A
		void ICollectionController.ModifyEventCurrency(ulong delta, bool removing)
		{
			this.ModifyEventCurrency(delta, removing);
		}

		// Token: 0x04004757 RID: 18263
		public const string kInvalidInstanceTemplate = "{@CharacterName} ({@CharacterId}) had an {@EventType} ({@ItemId}) in {@ContainerType}!";

		// Token: 0x04004758 RID: 18264
		private static object[] m_invalidInstanceArguments;

		// Token: 0x04004759 RID: 18265
		protected const string kInvalidIndexTemplate = "{@CharacterName} ({@CharacterId}) had an invalid index ({@ItemIndex} {@ItemId}) in {@ContainerType} and has been moved to Lost and Found!";

		// Token: 0x0400475A RID: 18266
		private static object[] m_invalidIndexArguments;

		// Token: 0x0400475B RID: 18267
		protected BaseNetworkInteractiveStation m_interactionStation;

		// Token: 0x0400475C RID: 18268
		protected InteractiveRefinementStation m_refinementStation;

		// Token: 0x0400475D RID: 18269
		protected IGatheringNode m_gatheringNode;

		// Token: 0x0400475E RID: 18270
		protected Dictionary<string, ContainerInstance> m_collections;

		// Token: 0x0400475F RID: 18271
		protected ContainerInstance m_masteries;

		// Token: 0x04004760 RID: 18272
		protected ContainerInstance m_abilities;

		// Token: 0x04004761 RID: 18273
		protected ContainerInstance m_equipment;

		// Token: 0x04004762 RID: 18274
		protected ContainerInstance m_inventory;

		// Token: 0x04004763 RID: 18275
		protected ContainerInstance m_gathering;

		// Token: 0x04004764 RID: 18276
		protected ContainerInstance m_pouch;

		// Token: 0x04004765 RID: 18277
		protected ContainerInstance m_reagentPouch;

		// Token: 0x04004766 RID: 18278
		protected ContainerInstance m_remoteContainer;

		// Token: 0x04004767 RID: 18279
		protected ContainerInstance m_personalBank;

		// Token: 0x04004768 RID: 18280
		protected ContainerInstance m_lostAndFound;

		// Token: 0x0400476B RID: 18283
		private List<BBTask> m_localTaskDiscard;

		// Token: 0x0400476C RID: 18284
		protected Dictionary<string, LearnableContainerInstance> m_learnableCollections;

		// Token: 0x0400476D RID: 18285
		protected LearnableContainerInstance m_recipes;

		// Token: 0x0400476E RID: 18286
		protected LearnableContainerInstance m_emotes;

		// Token: 0x0400476F RID: 18287
		protected LearnableContainerInstance m_titles;

		// Token: 0x04004770 RID: 18288
		protected CharacterRecord m_record;

		// Token: 0x04004771 RID: 18289
		private UniqueId? m_tradeId;
	}
}
