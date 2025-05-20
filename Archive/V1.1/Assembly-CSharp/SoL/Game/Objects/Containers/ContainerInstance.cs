using System;
using System.Collections.Generic;
using SoL.Game.Interactives;
using SoL.Game.Messages;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Game.Transactions;
using SoL.Game.UI;
using SoL.Managers;
using SoL.Networking;
using SoL.Networking.Database;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Containers
{
	// Token: 0x02000A08 RID: 2568
	public class ContainerInstance
	{
		// Token: 0x140000F6 RID: 246
		// (add) Token: 0x06004E25 RID: 20005 RVA: 0x001C1410 File Offset: 0x001BF610
		// (remove) Token: 0x06004E26 RID: 20006 RVA: 0x001C1448 File Offset: 0x001BF648
		public event Action<ArchetypeInstance> InstanceAdded;

		// Token: 0x140000F7 RID: 247
		// (add) Token: 0x06004E27 RID: 20007 RVA: 0x001C1480 File Offset: 0x001BF680
		// (remove) Token: 0x06004E28 RID: 20008 RVA: 0x001C14B8 File Offset: 0x001BF6B8
		public event Action<ArchetypeInstance> InstanceRemoved;

		// Token: 0x140000F8 RID: 248
		// (add) Token: 0x06004E29 RID: 20009 RVA: 0x001C14F0 File Offset: 0x001BF6F0
		// (remove) Token: 0x06004E2A RID: 20010 RVA: 0x001C1528 File Offset: 0x001BF728
		public event Action<ulong> CurrencyChanged;

		// Token: 0x140000F9 RID: 249
		// (add) Token: 0x06004E2B RID: 20011 RVA: 0x001C1560 File Offset: 0x001BF760
		// (remove) Token: 0x06004E2C RID: 20012 RVA: 0x001C1598 File Offset: 0x001BF798
		public event Action ContentsChanged;

		// Token: 0x140000FA RID: 250
		// (add) Token: 0x06004E2D RID: 20013 RVA: 0x001C15D0 File Offset: 0x001BF7D0
		// (remove) Token: 0x06004E2E RID: 20014 RVA: 0x001C1608 File Offset: 0x001BF808
		public event Action LockFlagsChanged;

		// Token: 0x140000FB RID: 251
		// (add) Token: 0x06004E2F RID: 20015 RVA: 0x001C1640 File Offset: 0x001BF840
		// (remove) Token: 0x06004E30 RID: 20016 RVA: 0x001C1678 File Offset: 0x001BF878
		public event Action QuantityOfItemChanged;

		// Token: 0x140000FC RID: 252
		// (add) Token: 0x06004E31 RID: 20017 RVA: 0x001C16B0 File Offset: 0x001BF8B0
		// (remove) Token: 0x06004E32 RID: 20018 RVA: 0x001C16E8 File Offset: 0x001BF8E8
		public event Action ToggleChanged;

		// Token: 0x140000FD RID: 253
		// (add) Token: 0x06004E33 RID: 20019 RVA: 0x001C1720 File Offset: 0x001BF920
		// (remove) Token: 0x06004E34 RID: 20020 RVA: 0x001C1758 File Offset: 0x001BF958
		public event Action ItemRepaired;

		// Token: 0x1700114C RID: 4428
		// (get) Token: 0x06004E35 RID: 20021 RVA: 0x00074D44 File Offset: 0x00072F44
		// (set) Token: 0x06004E36 RID: 20022 RVA: 0x00074D4C File Offset: 0x00072F4C
		public ContainerLockFlags LockFlags
		{
			get
			{
				return this.m_lockFlags;
			}
			set
			{
				if (this.m_lockFlags == value)
				{
					return;
				}
				this.m_lockFlags = value;
				Action lockFlagsChanged = this.LockFlagsChanged;
				if (lockFlagsChanged == null)
				{
					return;
				}
				lockFlagsChanged();
			}
		}

		// Token: 0x1700114D RID: 4429
		// (get) Token: 0x06004E37 RID: 20023 RVA: 0x00074D6F File Offset: 0x00072F6F
		public ContainerProfile ContainerProfile
		{
			get
			{
				return this.m_containerProfile;
			}
		}

		// Token: 0x1700114E RID: 4430
		// (get) Token: 0x06004E38 RID: 20024 RVA: 0x00074D77 File Offset: 0x00072F77
		// (set) Token: 0x06004E39 RID: 20025 RVA: 0x00074D7F File Offset: 0x00072F7F
		public IInteractive Interactive { get; set; }

		// Token: 0x1700114F RID: 4431
		// (get) Token: 0x06004E3A RID: 20026 RVA: 0x00074D88 File Offset: 0x00072F88
		// (set) Token: 0x06004E3B RID: 20027 RVA: 0x00074D90 File Offset: 0x00072F90
		public IContainerUI ContainerUI { get; private set; }

		// Token: 0x17001150 RID: 4432
		// (get) Token: 0x06004E3C RID: 20028 RVA: 0x00074D99 File Offset: 0x00072F99
		public ulong Currency
		{
			get
			{
				if (this.m_record.Currency == null)
				{
					return 0UL;
				}
				return this.m_record.Currency.Value;
			}
		}

		// Token: 0x17001151 RID: 4433
		// (get) Token: 0x06004E3D RID: 20029 RVA: 0x00074DC0 File Offset: 0x00072FC0
		public ContainerType ContainerType
		{
			get
			{
				return this.m_record.Type;
			}
		}

		// Token: 0x17001152 RID: 4434
		// (get) Token: 0x06004E3E RID: 20030 RVA: 0x001C1790 File Offset: 0x001BF990
		public int Count
		{
			get
			{
				int num = this.m_instances.Count;
				if (this.m_symbolicLinks != null)
				{
					num += this.m_symbolicLinks.Count;
				}
				return num;
			}
		}

		// Token: 0x17001153 RID: 4435
		// (get) Token: 0x06004E3F RID: 20031 RVA: 0x00074DCD File Offset: 0x00072FCD
		public int RecordCount
		{
			get
			{
				if (this.m_record != null)
				{
					return this.m_record.Instances.Count;
				}
				return 0;
			}
		}

		// Token: 0x17001154 RID: 4436
		// (get) Token: 0x06004E40 RID: 20032 RVA: 0x00074DE9 File Offset: 0x00072FE9
		public int ExpansionsPurchased
		{
			get
			{
				if (this.m_record == null || this.m_record.ExpansionsPurchased == null)
				{
					return 0;
				}
				return this.m_record.ExpansionsPurchased.Value;
			}
		}

		// Token: 0x06004E41 RID: 20033 RVA: 0x001C17C0 File Offset: 0x001BF9C0
		public ContainerInstance(ICollectionController controller, ContainerRecord record, IContainerUI containerUI)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			if (record == null)
			{
				throw new ArgumentNullException("record");
			}
			this.m_controller = controller;
			this.m_record = record;
			this.ContainerUI = containerUI;
			this.Id = record.GetId();
			this.m_instances = new DictionaryList<UniqueId, ArchetypeInstance>(default(UniqueIdComparer), this.m_record.Type.AllowReplacement());
			this.m_archetypes = new Dictionary<UniqueId, ArchetypeInstance>(default(UniqueIdComparer));
			if (this.ContainerType.CanContainSymbolicLinks())
			{
				this.m_symbolicLinks = new DictionaryList<int, ArchetypeInstanceSymbolicLink>(false);
			}
			this.Initialize();
		}

		// Token: 0x06004E42 RID: 20034 RVA: 0x001C187C File Offset: 0x001BFA7C
		private void Initialize()
		{
			ContainerType type = this.m_record.Type;
			if (type != ContainerType.PersonalBank)
			{
				if (type == ContainerType.Bank)
				{
					InternalGameDatabase.Archetypes.TryGetAsType<ContainerProfile>(new UniqueId(this.Id), out this.m_containerProfile);
				}
			}
			else
			{
				this.m_containerProfile = GlobalSettings.Values.Player.PersonalBankProfile;
			}
			if (this.m_containerProfile)
			{
				this.m_bankProfile = (this.m_containerProfile as BankProfile);
			}
			IContainerUI containerUI = this.ContainerUI;
			if (containerUI != null)
			{
				containerUI.Initialize(this);
			}
			for (int i = 0; i < this.m_record.Instances.Count; i++)
			{
				this.m_record.Instances[i].CreateItemInstanceUI();
				this.Add(this.m_record.Instances[i], false);
			}
			type = this.m_record.Type;
			if (type <= ContainerType.Masteries)
			{
				if (type != ContainerType.Equipment)
				{
					if (type == ContainerType.Masteries)
					{
						if (this.m_controller.GameEntity.Type == GameEntityType.Player)
						{
							MasteryArchetype.AddDynamicMasteries(this);
						}
					}
				}
				else if (this.m_controller.GameEntity.CharacterData != null)
				{
					this.m_controller.GameEntity.CharacterData.VisibleEquipment.ResetDirty();
					if (!GameManager.IsServer && this.m_controller.GameEntity.DCAController != null && this.m_controller.GameEntity.DCAController.DCA != null)
					{
						this.m_controller.GameEntity.DCAController.DCA.Refresh(true, true, true);
					}
				}
			}
			else if (type != ContainerType.Abilities)
			{
				if (type != ContainerType.TradeIncoming)
				{
					if (type == ContainerType.Inspection)
					{
						this.LockFlags |= ContainerLockFlags.Inspection;
					}
				}
				else
				{
					this.LockFlags |= ContainerLockFlags.Trade;
				}
			}
			else
			{
				GlobalSettings.Values.Combat.AutoAttack.DynamicallyLoad(this);
				for (int j = 0; j < this.m_instances.Count; j++)
				{
					if (this.m_instances[j] != null && this.m_instances[j].AbilityData != null && this.m_instances[j].AbilityData.Cooldown_Base.Elapsed != null && !this.m_instances[j].AbilityData.IsDynamic)
					{
						int cooldown = this.m_instances[j].Ability.GetCooldown(this.m_instances[j].GetAssociatedLevel(this.m_controller.GameEntity));
						float num = (this.m_instances[j].AbilityData.Cooldown_Base.Elapsed != null) ? this.m_instances[j].AbilityData.Cooldown_Base.Elapsed.Value : 0f;
						DateTime timeOfLastUse = this.m_instances[j].AbilityData.TimeOfLastUse;
						float num2 = (float)(DateTime.UtcNow - timeOfLastUse).TotalSeconds;
						if (num2 >= (float)cooldown)
						{
							this.m_instances[j].AbilityData.Cooldown_Base.Reset();
						}
						else if (num2 > num)
						{
							this.m_instances[j].AbilityData.Cooldown_Base.Elapsed = new float?(num2);
						}
					}
				}
			}
			IContainerUI containerUI2 = this.ContainerUI;
			if (containerUI2 != null)
			{
				containerUI2.PostInit();
			}
			this.Subscribe();
		}

		// Token: 0x06004E43 RID: 20035 RVA: 0x001C1C24 File Offset: 0x001BFE24
		private void Subscribe()
		{
			if (this.m_controller != null && this.m_controller.GameEntity && this.m_controller.GameEntity.Type == GameEntityType.Player)
			{
				if (this.m_record.Type.LockedInCombatStance() && this.m_controller.GameEntity.VitalsReplicator)
				{
					this.m_controller.GameEntity.VitalsReplicator.CurrentStance.Changed += this.CurrentStanceOnChanged;
					this.CurrentStanceOnChanged(this.m_controller.GameEntity.VitalsReplicator.CurrentStance.Value);
				}
				if (this.m_record.Type.LockedInDeath() && this.m_controller.GameEntity.CharacterData)
				{
					this.m_controller.GameEntity.CharacterData.CharacterFlags.Changed += this.PlayerFlagsOnChanged;
					this.PlayerFlagsOnChanged(this.m_controller.GameEntity.CharacterData.CharacterFlags.Value);
				}
				if (this.m_record.Type.LockedWhenNotAlive() && this.m_controller.GameEntity.VitalsReplicator)
				{
					this.m_controller.GameEntity.VitalsReplicator.CurrentHealthState.Changed += this.CurrentHealthStateOnChanged;
					this.CurrentHealthStateOnChanged(this.m_controller.GameEntity.VitalsReplicator.CurrentHealthState.Value);
				}
			}
		}

		// Token: 0x06004E44 RID: 20036 RVA: 0x001C1DB4 File Offset: 0x001BFFB4
		public void Unsubscribe()
		{
			if (this.m_controller != null && this.m_controller.GameEntity && this.m_controller.GameEntity.Type == GameEntityType.Player)
			{
				if (this.m_record.Type.LockedInCombatStance() && this.m_controller.GameEntity.VitalsReplicator)
				{
					this.m_controller.GameEntity.VitalsReplicator.CurrentStance.Changed -= this.CurrentStanceOnChanged;
				}
				if (this.m_record.Type.LockedInDeath() && this.m_controller.GameEntity.CharacterData)
				{
					this.m_controller.GameEntity.CharacterData.CharacterFlags.Changed -= this.PlayerFlagsOnChanged;
				}
				if (this.m_record.Type.LockedWhenNotAlive() && this.m_controller.GameEntity.VitalsReplicator)
				{
					this.m_controller.GameEntity.VitalsReplicator.CurrentHealthState.Changed -= this.CurrentHealthStateOnChanged;
				}
			}
		}

		// Token: 0x06004E45 RID: 20037 RVA: 0x0004475B File Offset: 0x0004295B
		private void ClearContainerInstanceReferences()
		{
		}

		// Token: 0x06004E46 RID: 20038 RVA: 0x0004475B File Offset: 0x0004295B
		private void ReturnInstancesToPool()
		{
		}

		// Token: 0x06004E47 RID: 20039 RVA: 0x001C1EE4 File Offset: 0x001C00E4
		private void ClearLocalReferences()
		{
			if (NullifyMemoryLeakSettings.CleanContainerInstance)
			{
				DictionaryList<UniqueId, ArchetypeInstance> instances = this.m_instances;
				if (instances != null)
				{
					instances.Clear();
				}
				Dictionary<UniqueId, ArchetypeInstance> archetypes = this.m_archetypes;
				if (archetypes != null)
				{
					archetypes.Clear();
				}
				HashSet<int> indexes = this.m_indexes;
				if (indexes != null)
				{
					indexes.Clear();
				}
				this.m_controller = null;
				this.m_record = null;
				this.m_containerProfile = null;
				this.m_bankProfile = null;
				this.Interactive = null;
				this.ContainerUI = null;
			}
		}

		// Token: 0x06004E48 RID: 20040 RVA: 0x00074E17 File Offset: 0x00073017
		public void CloseRemoteContainer()
		{
			this.OnDestroy();
		}

		// Token: 0x06004E49 RID: 20041 RVA: 0x001C1F58 File Offset: 0x001C0158
		public void OnDestroy()
		{
			this.Unsubscribe();
			if (this.ContainerType.DestroyContentsOnClose())
			{
				ContainerRecord record = this.m_record;
				if (record != null)
				{
					record.ReturnInstancesToPoolAndNullifyList();
				}
				this.ReturnInstancesToPool();
			}
			else
			{
				ContainerRecord record2 = this.m_record;
				if (record2 != null)
				{
					record2.RemoveContainerInstanceReferences();
				}
				this.ClearContainerInstanceReferences();
			}
			this.ClearLocalReferences();
		}

		// Token: 0x06004E4A RID: 20042 RVA: 0x001C1FB0 File Offset: 0x001C01B0
		public void AddCurrency(ulong currency)
		{
			if (!this.m_record.Type.HasCurrency())
			{
				throw new ArgumentException("Record does not have currency!  Type:" + this.m_record.Type.ToString() + " ID:" + this.Id);
			}
			if (this.m_record.Currency != null)
			{
				ulong value = this.m_record.Currency.Value + currency;
				this.m_record.Currency = new ulong?(value);
			}
			else
			{
				this.m_record.Currency = new ulong?(currency);
			}
			if (GameManager.IsServer && this.m_record.Type == ContainerType.TradeOutgoing && this.m_controller.GameEntity.CollectionController.TradeId != null)
			{
				ServerGameManager.TradeManager.Server_CurrencyChanged(this.m_controller.GameEntity.CollectionController.TradeId.Value, this.m_controller.GameEntity.NetworkEntity, this.Currency);
			}
			Action<ulong> currencyChanged = this.CurrencyChanged;
			if (currencyChanged != null)
			{
				currencyChanged(this.Currency);
			}
			if (!GameManager.IsServer && (this.m_record.Type == ContainerType.Inventory || this.m_record.Type == ContainerType.PersonalBank))
			{
				UIManager.InvokeAvailableCurrencyChanged();
			}
		}

		// Token: 0x06004E4B RID: 20043 RVA: 0x001C20FC File Offset: 0x001C02FC
		public void RemoveCurrency(ulong currency)
		{
			if (!this.m_record.Type.HasCurrency())
			{
				throw new ArgumentException("Record does not have currency!  Type:" + this.m_record.Type.ToString() + " ID:" + this.Id);
			}
			if (this.m_record.Currency == null)
			{
				throw new ArgumentException("Record has no currency to remove! Type:" + this.m_record.Type.ToString() + " ID:" + this.Id);
			}
			ulong num = (this.m_record.Currency.Value >= currency) ? (this.m_record.Currency.Value - currency) : 0UL;
			if (num == 0UL)
			{
				this.m_record.Currency = null;
			}
			else
			{
				this.m_record.Currency = new ulong?(num);
			}
			if (GameManager.IsServer && this.m_record.Type == ContainerType.TradeOutgoing && this.m_controller.GameEntity.CollectionController.TradeId != null)
			{
				ServerGameManager.TradeManager.Server_CurrencyChanged(this.m_controller.GameEntity.CollectionController.TradeId.Value, this.m_controller.GameEntity.NetworkEntity, this.Currency);
			}
			Action<ulong> currencyChanged = this.CurrencyChanged;
			if (currencyChanged == null)
			{
				return;
			}
			currencyChanged(this.Currency);
		}

		// Token: 0x06004E4C RID: 20044 RVA: 0x001C2268 File Offset: 0x001C0468
		public void ModifyCurrency(ulong currency)
		{
			if (!this.m_record.Type.HasCurrency())
			{
				throw new ArgumentException("Record does not have currency!  Type:" + this.m_record.Type.ToString() + " ID:" + this.Id);
			}
			this.m_record.Currency = new ulong?(currency);
			Action<ulong> currencyChanged = this.CurrencyChanged;
			if (currencyChanged == null)
			{
				return;
			}
			currencyChanged(this.Currency);
		}

		// Token: 0x06004E4D RID: 20045 RVA: 0x001C22E0 File Offset: 0x001C04E0
		public bool Add(ArchetypeInstance instance, bool update)
		{
			if (this.m_instances.ContainsKey(instance.InstanceId))
			{
				return false;
			}
			if (this.ContainerType.RequiresSymbolicLinkForPlacement())
			{
				this.AddSymbolicLink(instance);
			}
			instance.ContainerInstance = this;
			this.m_instances.Add(instance.InstanceId, instance);
			this.m_archetypes.AddOrReplace(instance.ArchetypeId, instance);
			this.m_indexes.Add(instance.Index);
			ContainerType type = this.m_record.Type;
			EquipableItem equipableItem;
			if (type != ContainerType.Equipment)
			{
				if (type != ContainerType.Abilities)
				{
					if (type == ContainerType.TradeOutgoing)
					{
						if (update && GameManager.IsServer && this.m_controller.GameEntity.CollectionController.TradeId != null)
						{
							ServerGameManager.TradeManager.Server_ItemAdded(this.m_controller.GameEntity.CollectionController.TradeId.Value, this.m_controller.GameEntity.NetworkEntity, instance);
						}
					}
				}
			}
			else if (InternalGameDatabase.Archetypes.TryGetAsType<EquipableItem>(instance.ArchetypeId, out equipableItem) && instance.Archetype != null)
			{
				equipableItem.OnEquip();
				if (GameManager.IsServer && equipableItem.Type.IsVisible() && this.m_controller.GameEntity.CharacterData != null && (instance.Index != 32768 || !this.m_controller.Record.Settings.HideHelm) && !this.m_controller.GameEntity.CharacterData.VisibleEquipment.ContainsKey(instance.Index))
				{
					this.m_controller.GameEntity.CharacterData.VisibleEquipment.Add(instance.Index, new EquipableItemVisualData(instance));
				}
			}
			IContainerUI containerUI = this.ContainerUI;
			if (containerUI != null)
			{
				containerUI.AddInstance(instance);
			}
			if (update)
			{
				if (!this.m_record.Instances.Contains(instance))
				{
					this.m_record.Instances.Add(instance);
				}
				Action<ArchetypeInstance> instanceAdded = this.InstanceAdded;
				if (instanceAdded != null)
				{
					instanceAdded(instance);
				}
				Action contentsChanged = this.ContentsChanged;
				if (contentsChanged != null)
				{
					contentsChanged();
				}
			}
			return true;
		}

		// Token: 0x06004E4E RID: 20046 RVA: 0x001C2514 File Offset: 0x001C0714
		public bool RemoveAndDestroy(UniqueId id)
		{
			ArchetypeInstance archetypeInstance = this.Remove(id);
			bool flag = archetypeInstance != null;
			if (flag)
			{
				if (archetypeInstance.InstanceUI != null)
				{
					archetypeInstance.InstanceUI.ExternalOnDestroy();
					UnityEngine.Object.Destroy(archetypeInstance.InstanceUI.gameObject);
				}
				StaticPool<ArchetypeInstance>.ReturnToPool(archetypeInstance);
			}
			return flag;
		}

		// Token: 0x06004E4F RID: 20047 RVA: 0x00074E1F File Offset: 0x0007301F
		public ArchetypeInstance Remove(UniqueId id)
		{
			return this.GetRemove(id, this.m_record.Type.AllowRemoval(), true);
		}

		// Token: 0x06004E50 RID: 20048 RVA: 0x00074E39 File Offset: 0x00073039
		public bool TryRemove(UniqueId instanceId)
		{
			return this.GetRemove(instanceId, this.m_record.Type.AllowRemoval(), true) != null;
		}

		// Token: 0x06004E51 RID: 20049 RVA: 0x001C2560 File Offset: 0x001C0760
		private ArchetypeInstance GetRemove(UniqueId id, bool remove, bool update)
		{
			ArchetypeInstance archetypeInstance;
			if (this.m_instances.TryGetValue(id, out archetypeInstance) && remove)
			{
				if (archetypeInstance.SymbolicLink != null)
				{
					this.RemoveSymbolicLink(archetypeInstance);
				}
				archetypeInstance.PreviousContainerInstance = this;
				archetypeInstance.PreviousIndex = archetypeInstance.Index;
				archetypeInstance.ContainerInstance = null;
				this.m_instances.Remove(id);
				this.m_archetypes.Remove(archetypeInstance.ArchetypeId);
				this.m_indexes.Remove(archetypeInstance.Index);
				for (int i = 0; i < this.m_instances.Count; i++)
				{
					if (this.m_instances[i].ArchetypeId == archetypeInstance.ArchetypeId)
					{
						this.m_archetypes.Add(this.m_instances[i].ArchetypeId, this.m_instances[i]);
						break;
					}
				}
				ContainerType type = this.m_record.Type;
				EquipableItem equipableItem;
				if (type != ContainerType.Equipment)
				{
					if (type != ContainerType.Abilities)
					{
						if (type == ContainerType.TradeOutgoing)
						{
							if (update && GameManager.IsServer && this.m_controller.GameEntity.CollectionController.TradeId != null)
							{
								ServerGameManager.TradeManager.Server_ItemRemoved(this.m_controller.GameEntity.CollectionController.TradeId.Value, this.m_controller.GameEntity.NetworkEntity, archetypeInstance.InstanceId);
							}
						}
					}
				}
				else if (InternalGameDatabase.Archetypes.TryGetAsType<EquipableItem>(archetypeInstance.ArchetypeId, out equipableItem))
				{
					equipableItem.OnUnequip();
					if (GameManager.IsServer && equipableItem.Type.IsVisible() && this.m_controller.GameEntity.CharacterData != null)
					{
						this.m_controller.GameEntity.CharacterData.VisibleEquipment.Remove(archetypeInstance.Index);
					}
				}
				IContainerUI containerUI = this.ContainerUI;
				if (containerUI != null)
				{
					containerUI.RemoveInstance(archetypeInstance);
				}
				if (update)
				{
					this.m_record.Instances.Remove(archetypeInstance);
					Action<ArchetypeInstance> instanceRemoved = this.InstanceRemoved;
					if (instanceRemoved != null)
					{
						instanceRemoved(archetypeInstance);
					}
					Action contentsChanged = this.ContentsChanged;
					if (contentsChanged != null)
					{
						contentsChanged();
					}
				}
			}
			return archetypeInstance;
		}

		// Token: 0x06004E52 RID: 20050 RVA: 0x001C2784 File Offset: 0x001C0984
		public bool InternalSwap(UniqueId instanceIdA, UniqueId instanceIdB)
		{
			ArchetypeInstance archetypeInstance;
			ArchetypeInstance archetypeInstance2;
			if (this.m_instances.TryGetValue(instanceIdA, out archetypeInstance) && this.m_instances.TryGetValue(instanceIdB, out archetypeInstance2))
			{
				int index = archetypeInstance2.Index;
				int index2 = archetypeInstance.Index;
				archetypeInstance.Index = index;
				archetypeInstance2.Index = index2;
				if (archetypeInstance.InstanceUI && archetypeInstance2.InstanceUI)
				{
					ContainerSlotUI slotUI = archetypeInstance.InstanceUI.SlotUI;
					ContainerSlotUI slotUI2 = archetypeInstance2.InstanceUI.SlotUI;
					slotUI.InstanceRemoved(archetypeInstance);
					slotUI2.InstanceRemoved(archetypeInstance2);
					slotUI.InstanceAdded(archetypeInstance2);
					slotUI2.InstanceAdded(archetypeInstance);
				}
				IContainerUI containerUI = this.ContainerUI;
				if (containerUI != null)
				{
					containerUI.ItemsSwapped();
				}
				if (GameManager.IsServer && this.ContainerType == ContainerType.TradeOutgoing && this.m_controller.GameEntity.CollectionController.TradeId != null)
				{
					ServerGameManager.TradeManager.Server_ItemsSwapped(this.m_controller.GameEntity.CollectionController.TradeId.Value, this.m_controller.GameEntity.NetworkEntity, archetypeInstance.InstanceId, archetypeInstance2.InstanceId);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06004E53 RID: 20051 RVA: 0x001C28AC File Offset: 0x001C0AAC
		private void AddSymbolicLink(ArchetypeInstance instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (instance.PreviousContainerInstance == null)
			{
				throw new ArgumentNullException("PreviousContainerInstance");
			}
			if (instance.SymbolicLink != null && instance.SymbolicLink.Freeze)
			{
				return;
			}
			ArchetypeInstanceSymbolicLink fromPool = StaticPool<ArchetypeInstanceSymbolicLink>.GetFromPool();
			fromPool.Freeze = false;
			fromPool.Instance = instance;
			fromPool.PreviousContainer = instance.PreviousContainerInstance;
			fromPool.PreviousIndex = instance.PreviousIndex;
			DictionaryList<int, ArchetypeInstanceSymbolicLink> symbolicLinks = fromPool.PreviousContainer.m_symbolicLinks;
			if (symbolicLinks != null)
			{
				symbolicLinks.Add(fromPool.PreviousIndex, fromPool);
			}
			instance.SymbolicLink = fromPool;
			if (this.ContainerUI != null && instance.InstanceUI)
			{
				fromPool.InitUI();
			}
		}

		// Token: 0x06004E54 RID: 20052 RVA: 0x001C295C File Offset: 0x001C0B5C
		private void RemoveSymbolicLink(ArchetypeInstance instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (instance.SymbolicLink == null)
			{
				throw new ArgumentNullException("SymbolicLink");
			}
			if (instance.SymbolicLink.Freeze)
			{
				return;
			}
			ContainerInstance previousContainer = instance.SymbolicLink.PreviousContainer;
			if (previousContainer != null)
			{
				DictionaryList<int, ArchetypeInstanceSymbolicLink> symbolicLinks = previousContainer.m_symbolicLinks;
				if (symbolicLinks != null)
				{
					symbolicLinks.Remove(instance.SymbolicLink.PreviousIndex);
				}
			}
			StaticPool<ArchetypeInstanceSymbolicLink>.ReturnToPool(instance.SymbolicLink);
			instance.SymbolicLink = null;
		}

		// Token: 0x06004E55 RID: 20053 RVA: 0x001C29D8 File Offset: 0x001C0BD8
		public TakeAllResponse MoveContentsToContainerInstance(ContainerInstance target, bool transferCurrency = true, bool useSymbolicLinks = false)
		{
			if (ContainerInstance.m_takeAllItems == null)
			{
				ContainerInstance.m_takeAllItems = new List<TakeAllItem>(this.m_instances.Count);
			}
			ContainerInstance.m_takeAllItems.Clear();
			bool flag = false;
			for (int i = 0; i < this.m_instances.Count; i++)
			{
				if (this.m_instances[i] != null)
				{
					int index = -1;
					ContainerInstance containerInstance = null;
					if (useSymbolicLinks && this.m_instances[i].SymbolicLink != null)
					{
						index = this.m_instances[i].SymbolicLink.PreviousIndex;
						containerInstance = this.m_instances[i].SymbolicLink.PreviousContainer;
					}
					ArchetypeInstance remove = this.GetRemove(this.m_instances[i].InstanceId, true, false);
					if (containerInstance == null)
					{
						containerInstance = (target.HasRoom() ? target : this.m_controller.LostAndFound);
						index = containerInstance.GetFirstAvailableIndex();
					}
					remove.Index = index;
					if (containerInstance.Add(remove, true))
					{
						ContainerInstance.m_takeAllItems.Add(new TakeAllItem
						{
							InstanceId = remove.InstanceId,
							ContainerId = containerInstance.Id,
							Index = remove.Index
						});
					}
					else
					{
						PlayerCollectionController.AddInstanceToInvalid(this.m_controller.Record, remove, containerInstance.ContainerType, "InvalidMoveContentsToContainerInstance", false);
						flag = true;
					}
					i--;
				}
			}
			if (flag)
			{
				this.m_controller.Record.UpdateInvalidItems(ExternalGameDatabase.Database);
			}
			ulong currency = 0UL;
			if (transferCurrency)
			{
				currency = this.Currency;
				if (this.m_record.Currency != null)
				{
					target.AddCurrency(currency);
					this.m_record.Currency = new ulong?(0UL);
				}
			}
			this.ManualUpdate();
			return new TakeAllResponse
			{
				Currency = currency,
				Items = ContainerInstance.m_takeAllItems.ToArray()
			};
		}

		// Token: 0x06004E56 RID: 20054 RVA: 0x001C2BB8 File Offset: 0x001C0DB8
		public List<ArchetypeInstance> RemoveAllInstances()
		{
			List<ArchetypeInstance> fromPool = StaticListPool<ArchetypeInstance>.GetFromPool();
			for (int i = 0; i < this.m_instances.Count; i++)
			{
				ArchetypeInstance archetypeInstance = this.Remove(this.m_instances[i].InstanceId);
				if (archetypeInstance != null)
				{
					fromPool.Add(archetypeInstance);
					i--;
				}
			}
			return fromPool;
		}

		// Token: 0x06004E57 RID: 20055 RVA: 0x001C2C08 File Offset: 0x001C0E08
		public void DestroyContents()
		{
			for (int i = 0; i < this.m_instances.Count; i++)
			{
				this.RemoveAndDestroy(this.m_instances[i].InstanceId);
				i--;
			}
			if (this.m_record.Currency != null)
			{
				this.RemoveCurrency(this.Currency);
			}
		}

		// Token: 0x06004E58 RID: 20056 RVA: 0x00074E56 File Offset: 0x00073056
		private IEnumerable<ArchetypeInstance> GetInstances()
		{
			int num;
			for (int i = 0; i < this.m_instances.Count; i = num + 1)
			{
				yield return this.m_instances[i];
				num = i;
			}
			yield break;
		}

		// Token: 0x17001155 RID: 4437
		// (get) Token: 0x06004E59 RID: 20057 RVA: 0x00074E66 File Offset: 0x00073066
		public IEnumerable<ArchetypeInstance> Instances
		{
			get
			{
				if (this.m_instanceEnumerable == null)
				{
					this.m_instanceEnumerable = this.GetInstances();
				}
				return this.m_instanceEnumerable;
			}
		}

		// Token: 0x06004E5A RID: 20058 RVA: 0x00074E82 File Offset: 0x00073082
		public ArchetypeInstance GetIndex(int index)
		{
			if (index < this.m_instances.Count)
			{
				return this.m_instances[index];
			}
			return null;
		}

		// Token: 0x06004E5B RID: 20059 RVA: 0x00074EA0 File Offset: 0x000730A0
		public void ShuffleInstances()
		{
			this.m_instances.ShuffleList();
		}

		// Token: 0x06004E5C RID: 20060 RVA: 0x00074EAD File Offset: 0x000730AD
		public void SaveRecord()
		{
			if (GameManager.IsServer && this.m_record.Type.SaveToStorage())
			{
				this.m_record.UpdateRecord(ExternalGameDatabase.Database);
			}
		}

		// Token: 0x06004E5D RID: 20061 RVA: 0x00074ED8 File Offset: 0x000730D8
		public bool Contains(UniqueId id)
		{
			return this.m_instances.ContainsKey(id);
		}

		// Token: 0x06004E5E RID: 20062 RVA: 0x001C2C68 File Offset: 0x001C0E68
		public bool TryGetInstanceForIndex(int index, out ArchetypeInstance instance)
		{
			instance = null;
			for (int i = 0; i < this.m_instances.Count; i++)
			{
				if (this.m_instances[i].Index == index)
				{
					instance = this.m_instances[i];
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004E5F RID: 20063 RVA: 0x00074E82 File Offset: 0x00073082
		public ArchetypeInstance GetInstanceForListIndex(int index)
		{
			if (index < this.m_instances.Count)
			{
				return this.m_instances[index];
			}
			return null;
		}

		// Token: 0x06004E60 RID: 20064 RVA: 0x00074EE6 File Offset: 0x000730E6
		public bool TryGetInstanceForInstanceId(UniqueId instanceId, out ArchetypeInstance instance)
		{
			return this.m_instances.TryGetValue(instanceId, out instance);
		}

		// Token: 0x06004E61 RID: 20065 RVA: 0x00074EF5 File Offset: 0x000730F5
		public bool TryGetInstanceForArchetypeId(UniqueId archetypeId, out ArchetypeInstance instance)
		{
			return this.m_archetypes.TryGetValue(archetypeId, out instance);
		}

		// Token: 0x06004E62 RID: 20066 RVA: 0x00074F04 File Offset: 0x00073104
		private bool IsAvailableIndex(int index)
		{
			return !this.m_indexes.Contains(index) && (this.m_symbolicLinks == null || !this.m_symbolicLinks.ContainsKey(index));
		}

		// Token: 0x06004E63 RID: 20067 RVA: 0x001C2CB4 File Offset: 0x001C0EB4
		public int GetFirstAvailableIndex()
		{
			if (this.m_record.Type == ContainerType.Equipment)
			{
				throw new Exception("Unable to ask for first available index on equipment!");
			}
			if (this.m_record.Type == ContainerType.Abilities)
			{
				return -1;
			}
			if (this.m_instances.Count <= 0)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < this.m_instances.Count; i++)
			{
				if (this.m_instances[i].Index > num)
				{
					num = this.m_instances[i].Index;
				}
				if (this.IsAvailableIndex(i))
				{
					return i;
				}
			}
			int maxCapacity = this.GetMaxCapacity();
			for (int j = num + 1; j < maxCapacity; j++)
			{
				if (this.IsAvailableIndex(j))
				{
					return j;
				}
			}
			Debug.LogWarning("RETURNING MAX CAPACITY?");
			return maxCapacity;
		}

		// Token: 0x06004E64 RID: 20068 RVA: 0x001C2D70 File Offset: 0x001C0F70
		public bool TryGetFirstAvailableIndex(GameEntity entity, out int index)
		{
			index = -1;
			if (this.m_record.Type == ContainerType.Equipment)
			{
				throw new Exception("Unable to ask for first available index on equipment!");
			}
			if (!entity)
			{
				throw new ArgumentNullException("entity");
			}
			if (this.m_record.Type == ContainerType.Abilities)
			{
				return false;
			}
			bool subscriber = entity.Subscriber;
			int maxCapacity = this.GetMaxCapacity();
			for (int i = 0; i < maxCapacity; i++)
			{
				if (this.IsAvailableIndex(i) && (subscriber || !this.IsSubscriberOnlySlot(i)))
				{
					index = i;
					break;
				}
			}
			return index > -1;
		}

		// Token: 0x06004E65 RID: 20069 RVA: 0x001C2DF8 File Offset: 0x001C0FF8
		public ArchetypeInstance GetRandomInstance()
		{
			if (this.m_instances.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, this.m_instances.Count);
				return this.m_instances[index];
			}
			return null;
		}

		// Token: 0x06004E66 RID: 20070 RVA: 0x001C2E34 File Offset: 0x001C1034
		public bool TryGetRandomDurabilityInstance(out ArchetypeInstance instance, out IDurability durability)
		{
			if (ContainerInstance.m_durabilityIndexes == null)
			{
				ContainerInstance.m_durabilityIndexes = new List<int>();
			}
			instance = null;
			durability = null;
			if (this.m_instances.Count > 0)
			{
				ContainerInstance.m_durabilityIndexes.Clear();
				for (int i = 0; i < this.m_instances.Count; i++)
				{
					ContainerInstance.m_durabilityIndexes.Add(i);
				}
				ContainerInstance.m_durabilityIndexes.Shuffle<int>();
				for (int j = 0; j < ContainerInstance.m_durabilityIndexes.Count; j++)
				{
					int index = ContainerInstance.m_durabilityIndexes[j];
					ArchetypeInstance archetypeInstance = this.m_instances[index];
					if (archetypeInstance != null && (this.ContainerType != ContainerType.Equipment || archetypeInstance.Index != 65536))
					{
						ItemInstanceData itemData = archetypeInstance.ItemData;
						if (((itemData != null) ? itemData.Durability : null) != null && archetypeInstance.Archetype.TryGetAsType(out durability) && durability.DegradeOnHit)
						{
							instance = this.m_instances[index];
							ContainerInstance.m_durabilityIndexes.Clear();
							return true;
						}
					}
				}
				ContainerInstance.m_durabilityIndexes.Clear();
			}
			return false;
		}

		// Token: 0x06004E67 RID: 20071 RVA: 0x001C2F38 File Offset: 0x001C1138
		public void ExpansionPurchased()
		{
			if (this.m_record != null)
			{
				if (this.m_record.ExpansionsPurchased != null)
				{
					this.m_record.ExpansionsPurchased++;
				}
				else
				{
					this.m_record.ExpansionsPurchased = new int?(1);
				}
				this.SaveRecord();
			}
		}

		// Token: 0x06004E68 RID: 20072 RVA: 0x00074F2F File Offset: 0x0007312F
		private void PlayerFlagsOnChanged(PlayerFlags obj)
		{
			if (obj.HasBitFlag(PlayerFlags.MissingBag))
			{
				this.LockFlags |= ContainerLockFlags.MissingBag;
				return;
			}
			this.LockFlags &= ~ContainerLockFlags.MissingBag;
		}

		// Token: 0x06004E69 RID: 20073 RVA: 0x00074F58 File Offset: 0x00073158
		private void CurrentStanceOnChanged(Stance obj)
		{
			if (this.m_record.Type.LockedInCombatStance())
			{
				if (obj == Stance.Combat)
				{
					this.LockFlags |= ContainerLockFlags.Combat;
					return;
				}
				this.LockFlags &= ~ContainerLockFlags.Combat;
			}
		}

		// Token: 0x06004E6A RID: 20074 RVA: 0x00074F90 File Offset: 0x00073190
		private void CurrentHealthStateOnChanged(HealthState obj)
		{
			if (obj != HealthState.Alive)
			{
				this.LockFlags |= ContainerLockFlags.NotAlive;
				return;
			}
			this.LockFlags &= ~ContainerLockFlags.NotAlive;
		}

		// Token: 0x06004E6B RID: 20075 RVA: 0x0004475B File Offset: 0x0004295B
		public void ManualUpdate()
		{
		}

		// Token: 0x06004E6C RID: 20076 RVA: 0x00074FB5 File Offset: 0x000731B5
		public bool IsLocked()
		{
			return this.m_lockFlags.IsLocked();
		}

		// Token: 0x06004E6D RID: 20077 RVA: 0x00074FC2 File Offset: 0x000731C2
		public bool IsUnlocked()
		{
			return !this.IsLocked();
		}

		// Token: 0x06004E6E RID: 20078 RVA: 0x001C2FAC File Offset: 0x001C11AC
		public bool CanPlace(ArchetypeInstance instance, int targetIndex)
		{
			if (this.m_lockFlags.IsLocked() || this.m_record == null || instance == null || instance.ContainerInstance == null)
			{
				return false;
			}
			if (this.m_record.Type != ContainerType.Bank && instance.ContainerInstance.ContainerType == ContainerType.Bank && instance.IsItem && instance.ItemData.IsSoulbound && instance.ItemData.SoulboundPlayerId != this.m_controller.Record.Id)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "This item cannot be removed by you!");
				return false;
			}
			if (!this.m_record.Type.IsValidInstanceForContainer(instance))
			{
				return false;
			}
			ArchetypeInstanceSymbolicLink archetypeInstanceSymbolicLink;
			if (this.ContainerType.CanContainSymbolicLinks() && this.m_symbolicLinks != null && this.m_symbolicLinks.TryGetValue(targetIndex, out archetypeInstanceSymbolicLink) && archetypeInstanceSymbolicLink.Instance != instance)
			{
				return false;
			}
			ContainerType type = this.m_record.Type;
			switch (type)
			{
			case ContainerType.Equipment:
			{
				IEquipable equipable;
				if (instance.Archetype != null && instance.Archetype.TryGetAsType(out equipable))
				{
					if (targetIndex == 65536)
					{
						if (!EquipmentExtensions.CosmeticEquipmentTypes.Contains(equipable.Type))
						{
							return false;
						}
						if (!this.m_controller.GameEntity.Subscriber)
						{
							MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Reserved for Subscribers only!");
							return false;
						}
					}
					else if (!equipable.Type.GetAllCompatibleSlots(false).HasBitFlag((EquipmentSlot)targetIndex))
					{
						return false;
					}
					switch (targetIndex)
					{
					case 1:
					case 4:
					{
						if (!equipable.Type.BlockOffhandSlot())
						{
							goto IL_296;
						}
						EquipmentSlot index = (targetIndex == 1) ? EquipmentSlot.PrimaryWeapon_OffHand : EquipmentSlot.SecondaryWeapon_OffHand;
						ArchetypeInstance archetypeInstance;
						if (this.TryGetInstanceForIndex((int)index, out archetypeInstance))
						{
							if (!GameManager.IsServer)
							{
								MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Off hand is not free!");
							}
							return false;
						}
						goto IL_296;
					}
					case 2:
						break;
					case 3:
						goto IL_296;
					default:
						if (targetIndex != 8)
						{
							goto IL_296;
						}
						break;
					}
					EquipmentSlot index2 = (targetIndex == 2) ? EquipmentSlot.PrimaryWeapon_MainHand : EquipmentSlot.SecondaryWeapon_MainHand;
					ArchetypeInstance archetypeInstance2;
					IEquipable equipable2;
					if (this.TryGetInstanceForIndex((int)index2, out archetypeInstance2) && archetypeInstance2.Archetype.TryGetAsType(out equipable2) && equipable2.Type.BlockOffhandSlot())
					{
						if (!GameManager.IsServer)
						{
							MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Main hand blocks off hand slot!");
						}
						return false;
					}
					IL_296:
					return (instance.ItemData == null || instance.ItemData.Augment == null || !instance.ItemData.Augment.AugmentItemRef || instance.ItemData.Augment.AugmentItemRef.MeetsLevelRequirement(this.m_controller.GameEntity)) && equipable.CanEquip(this.m_controller.GameEntity);
				}
				return false;
			}
			case ContainerType.Inventory:
				return this.m_record.Type.AllowPlacement() && instance.IsItem;
			case (ContainerType)3:
			case ContainerType.LostAndFound:
			case (ContainerType)9:
			case ContainerType.Masteries:
				break;
			case ContainerType.Pouch:
				return instance.IsItem && this.m_record.Type.AllowPlacement() && instance.Archetype != null && instance.Archetype.CanPlaceInPouch && !instance.Archetype.IsReagent;
			case ContainerType.ReagentPouch:
				return instance.IsItem && this.m_record.Type.AllowPlacement() && instance.Archetype != null && instance.Archetype.IsReagent;
			case ContainerType.PersonalBank:
			{
				bool flag = instance.IsItem && this.m_record.Type.AllowRemoval();
				if (!flag)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "This item cannot be placed in a bank!");
				}
				if (!this.m_controller.GameEntity.Subscriber && this.IsSubscriberOnlySlot(targetIndex))
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Reserved for Subscribers only!");
					flag = false;
				}
				return flag;
			}
			case ContainerType.Gathering:
				return instance.IsItem && this.m_record.Type.AllowPlacement() && instance.Archetype != null && instance.Archetype.CanPlaceInGathering;
			case ContainerType.Abilities:
				return instance.IsAbility && this.m_record.Type.AllowPlacement();
			default:
				switch (type)
				{
				case ContainerType.TradeOutgoing:
					return instance.IsItem && this.m_record.Type.AllowPlacement() && !instance.ItemData.ItemFlags.HasBitFlag(ItemFlags.NoTrade) && (instance.ContainerInstance.ContainerType == ContainerType.Inventory || instance.ContainerInstance.ContainerType == ContainerType.Gathering || instance.ContainerInstance.ContainerType == ContainerType.TradeOutgoing);
				case ContainerType.TradeIncoming:
				case ContainerType.PostIncoming:
					break;
				case ContainerType.MerchantOutgoing:
				{
					ItemArchetype itemArchetype;
					if (!this.m_record.Type.AllowPlacement() || !instance.IsItem || !instance.Archetype.TryGetAsType(out itemArchetype))
					{
						return false;
					}
					ulong num;
					if (!itemArchetype.TryGetSalePrice(instance, out num) || num <= 0UL)
					{
						if (!GameManager.IsServer)
						{
							MessageManager.ChatQueue.AddToQueue(MessageType.Notification, instance.Archetype.GetModifiedDisplayName(instance) + " cannot be sold!");
						}
						return false;
					}
					return true;
				}
				case ContainerType.BlacksmithOutgoing:
					return this.m_record.Type.AllowPlacement() && instance.IsItem && instance.ItemData.Durability != null;
				case ContainerType.RuneCollector:
					return this.m_record.Type.AllowPlacement() && instance.IsItem && instance.Archetype is RunicBattery;
				case ContainerType.PostOutgoing:
				case ContainerType.AuctionOutgoing:
				{
					bool flag2 = instance.IsItem && this.m_record.Type.AllowPlacement() && !instance.ItemData.ItemFlags.HasBitFlag(ItemFlags.NoTrade);
					if (!flag2)
					{
						UIManager.TriggerCannotPerform("Cannot place here!");
						string content = (this.m_record.Type == ContainerType.PostOutgoing) ? "This item cannot be sent via post!" : "This item cannot be auctioned!";
						MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content);
					}
					if (flag2 && instance.ContainerInstance.ContainerType != ContainerType.Inventory && instance.ContainerInstance.ContainerType != ContainerType.Gathering && instance.ContainerInstance.ContainerType != ContainerType.PostOutgoing)
					{
						string text = (this.m_record.Type == ContainerType.PostOutgoing) ? "You may only send items via post from your bag or gathering bag!" : "You may only auction items from your bag or gathering bag!";
						UIManager.TriggerCannotPerform(text);
						text = ((this.m_record.Type == ContainerType.PostOutgoing) ? "You may only send items via post from your inventory!" : "You may only auction items from your inventory!");
						MessageManager.ChatQueue.AddToQueue(MessageType.Notification, text);
						flag2 = false;
					}
					if (flag2 && !this.m_controller.GameEntity.Subscriber && this.IsSubscriberOnlySlot(targetIndex))
					{
						UIManager.TriggerCannotPerform("Reserved for Subscribers only!");
						MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Reserved for Subscribers only!");
						flag2 = false;
					}
					return flag2;
				}
				default:
					if (type == ContainerType.Bank)
					{
						bool flag3 = instance.IsItem && this.m_record.Type.AllowRemoval() && !instance.ItemData.ItemFlags.HasBitFlag(ItemFlags.NoSharedBank);
						if (!flag3)
						{
							UIManager.TriggerCannotPerform("Cannot place here!");
							MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "This item cannot be placed in a shared bank!");
						}
						if (!this.m_controller.GameEntity.Subscriber && this.IsSubscriberOnlySlot(targetIndex))
						{
							UIManager.TriggerCannotPerform("Reserved for Subscribers only!");
							MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Reserved for Subscribers only!");
							flag3 = false;
						}
						return flag3;
					}
					break;
				}
				break;
			}
			return this.m_record.Type.AllowPlacement();
		}

		// Token: 0x06004E6F RID: 20079 RVA: 0x001C36C8 File Offset: 0x001C18C8
		public bool IsSubscriberOnlySlot(int targetSlot)
		{
			if (targetSlot == -1)
			{
				return false;
			}
			ContainerType containerType = this.ContainerType;
			if (containerType <= ContainerType.PersonalBank)
			{
				if (containerType != ContainerType.Equipment)
				{
					if (containerType != ContainerType.PersonalBank)
					{
						return false;
					}
					goto IL_35;
				}
			}
			else
			{
				if (containerType == ContainerType.PostOutgoing)
				{
					return targetSlot > 0;
				}
				if (containerType != ContainerType.Inspection)
				{
					if (containerType != ContainerType.Bank)
					{
						return false;
					}
					goto IL_35;
				}
			}
			return targetSlot == 65536;
			IL_35:
			if (this.m_bankProfile == null)
			{
				return false;
			}
			int constraintCount = this.m_bankProfile.ConstraintCount;
			int num = targetSlot + 1;
			if (num % constraintCount == 0)
			{
				int num2 = num / constraintCount - 1;
				num -= num2 * constraintCount;
			}
			else if (num >= constraintCount)
			{
				int num3 = Mathf.FloorToInt((float)num / (float)constraintCount);
				num -= num3 * constraintCount;
			}
			return num >= constraintCount - 3;
		}

		// Token: 0x06004E70 RID: 20080 RVA: 0x00074FCD File Offset: 0x000731CD
		public bool CanSplit()
		{
			return this.m_record.Type.AllowSplitting() && this.m_record.Type.AllowRemoval() && !this.m_lockFlags.IsLocked();
		}

		// Token: 0x06004E71 RID: 20081 RVA: 0x00075003 File Offset: 0x00073203
		public bool CanSplitSubscriberSlotCheck(int index)
		{
			return (this.m_controller != null && this.m_controller.GameEntity && this.m_controller.GameEntity.Subscriber) || !this.IsSubscriberOnlySlot(index);
		}

		// Token: 0x06004E72 RID: 20082 RVA: 0x0007503D File Offset: 0x0007323D
		public bool CanDestroyItem()
		{
			return this.m_record.Type.AllowDestruction() && !this.m_lockFlags.IsLocked();
		}

		// Token: 0x06004E73 RID: 20083 RVA: 0x00075061 File Offset: 0x00073261
		public bool HasRoom()
		{
			return this.Count < this.GetMaxCapacity();
		}

		// Token: 0x06004E74 RID: 20084 RVA: 0x001C3764 File Offset: 0x001C1964
		public string GetHeaderString()
		{
			ContainerType type = this.m_record.Type;
			if (type == ContainerType.Inventory)
			{
				return "Bag";
			}
			if (type == ContainerType.Gathering)
			{
				return "Gathering";
			}
			switch (type)
			{
			case ContainerType.TradeOutgoing:
				return this.m_controller.GameEntity.CharacterData.Name.Value;
			case ContainerType.TradeIncoming:
				if (ClientGameManager.TradeManager != null && ClientGameManager.TradeManager.TradePartner != null)
				{
					return ClientGameManager.TradeManager.TradePartner.GameEntity.CharacterData.Name.Value;
				}
				return null;
			case ContainerType.MerchantOutgoing:
				return "Merchant";
			case ContainerType.BlacksmithOutgoing:
				return "Blacksmith";
			case ContainerType.RuneCollector:
				return "Rune Collector";
			default:
				if (!(this.ContainerProfile != null))
				{
					return this.m_record.Type.ToString();
				}
				return this.ContainerProfile.DisplayName;
			}
		}

		// Token: 0x06004E75 RID: 20085 RVA: 0x00075071 File Offset: 0x00073271
		public int GetMaxCapacity()
		{
			if (!(this.ContainerProfile != null))
			{
				return this.m_record.Type.GetMaxCapacity();
			}
			return this.ContainerProfile.GetMaxCapacity(this.m_record);
		}

		// Token: 0x06004E76 RID: 20086 RVA: 0x000750A3 File Offset: 0x000732A3
		public void InvokeContentsChanged()
		{
			Action contentsChanged = this.ContentsChanged;
			if (contentsChanged == null)
			{
				return;
			}
			contentsChanged();
		}

		// Token: 0x06004E77 RID: 20087 RVA: 0x000750B5 File Offset: 0x000732B5
		public void InvokeQuantityOfItemChanged()
		{
			Action quantityOfItemChanged = this.QuantityOfItemChanged;
			if (quantityOfItemChanged == null)
			{
				return;
			}
			quantityOfItemChanged();
		}

		// Token: 0x06004E78 RID: 20088 RVA: 0x000750C7 File Offset: 0x000732C7
		public void InvokeItemRepaired()
		{
			Action itemRepaired = this.ItemRepaired;
			if (itemRepaired == null)
			{
				return;
			}
			itemRepaired();
		}

		// Token: 0x06004E79 RID: 20089 RVA: 0x001C3850 File Offset: 0x001C1A50
		public void SetToggle(int index, bool value)
		{
			if (this.ContainerType != ContainerType.ReagentPouch)
			{
				return;
			}
			this.InitializeToggles();
			if (index < this.m_record.Toggles.Length)
			{
				bool flag = this.m_record.Toggles[index];
				this.m_record.Toggles[index] = value;
				if (flag != value)
				{
					Action toggleChanged = this.ToggleChanged;
					if (toggleChanged == null)
					{
						return;
					}
					toggleChanged();
				}
			}
		}

		// Token: 0x06004E7A RID: 20090 RVA: 0x001C38AC File Offset: 0x001C1AAC
		public bool GetToggle(int index)
		{
			if (this.ContainerType != ContainerType.ReagentPouch)
			{
				return false;
			}
			this.InitializeToggles();
			return this.m_record != null && this.m_record.Toggles != null && this.m_record.Toggles.Length != 0 && index < this.m_record.Toggles.Length && this.m_record.Toggles[index];
		}

		// Token: 0x06004E7B RID: 20091 RVA: 0x001C390C File Offset: 0x001C1B0C
		private void InitializeToggles()
		{
			if (this.ContainerType != ContainerType.ReagentPouch || this.m_togglesInitialized)
			{
				return;
			}
			int maxCapacity = this.ContainerType.GetMaxCapacity();
			if (this.m_record.Toggles == null)
			{
				this.m_record.Toggles = new bool[maxCapacity];
				for (int i = 0; i < maxCapacity; i++)
				{
					this.m_record.Toggles[i] = true;
				}
			}
			else if (this.m_record.Toggles.Length != maxCapacity)
			{
				bool[] array = new bool[maxCapacity];
				for (int j = 0; j < maxCapacity; j++)
				{
					array[j] = true;
				}
				for (int k = 0; k < this.m_record.Toggles.Length; k++)
				{
					if (k < this.m_record.Toggles.Length)
					{
						array[k] = this.m_record.Toggles[k];
					}
				}
				this.m_record.Toggles = array;
			}
			this.m_togglesInitialized = true;
		}

		// Token: 0x0400477A RID: 18298
		private static List<TakeAllItem> m_takeAllItems;

		// Token: 0x0400477B RID: 18299
		private readonly DictionaryList<UniqueId, ArchetypeInstance> m_instances;

		// Token: 0x0400477C RID: 18300
		private readonly Dictionary<UniqueId, ArchetypeInstance> m_archetypes;

		// Token: 0x0400477D RID: 18301
		private readonly HashSet<int> m_indexes = new HashSet<int>();

		// Token: 0x0400477E RID: 18302
		private DictionaryList<int, ArchetypeInstanceSymbolicLink> m_symbolicLinks;

		// Token: 0x0400477F RID: 18303
		private ICollectionController m_controller;

		// Token: 0x04004780 RID: 18304
		private ContainerRecord m_record;

		// Token: 0x04004781 RID: 18305
		private ContainerProfile m_containerProfile;

		// Token: 0x04004782 RID: 18306
		private BankProfile m_bankProfile;

		// Token: 0x04004783 RID: 18307
		public readonly string Id;

		// Token: 0x04004784 RID: 18308
		private ContainerLockFlags m_lockFlags;

		// Token: 0x04004787 RID: 18311
		private IEnumerable<ArchetypeInstance> m_instanceEnumerable;

		// Token: 0x04004788 RID: 18312
		private static List<int> m_durabilityIndexes;

		// Token: 0x04004789 RID: 18313
		private const string kIsSubscriberOnlyNotification = "Reserved for Subscribers only!";

		// Token: 0x0400478A RID: 18314
		private const int kSlotCountToLock = 4;

		// Token: 0x0400478B RID: 18315
		private const int kSlotLockDelta = 3;

		// Token: 0x0400478C RID: 18316
		private bool m_togglesInitialized;
	}
}
