using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NetStack.Serialization;
using Newtonsoft.Json;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Objects.Containers;
using SoL.Game.UI.Archetypes;
using SoL.Managers;
using SoL.Networking;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A26 RID: 2598
	[BsonIgnoreExtraElements]
	[Serializable]
	public class ArchetypeInstance : IPoolable, INetworkSerializable
	{
		// Token: 0x170011D4 RID: 4564
		// (get) Token: 0x06005064 RID: 20580 RVA: 0x00075DA9 File Offset: 0x00073FA9
		// (set) Token: 0x06005065 RID: 20581 RVA: 0x00075DB1 File Offset: 0x00073FB1
		[BsonIgnore]
		[JsonIgnore]
		public ArchetypeInstanceSymbolicLink SymbolicLink { get; set; }

		// Token: 0x06005066 RID: 20582 RVA: 0x001CD198 File Offset: 0x001CB398
		public SymbolicLinkData? GetSymbolicLinkData()
		{
			if (this.SymbolicLink != null)
			{
				return new SymbolicLinkData?(new SymbolicLinkData
				{
					PreviousContainer = this.SymbolicLink.PreviousContainer,
					PreviousIndex = this.SymbolicLink.PreviousIndex
				});
			}
			return null;
		}

		// Token: 0x170011D5 RID: 4565
		// (get) Token: 0x06005067 RID: 20583 RVA: 0x00075DBA File Offset: 0x00073FBA
		// (set) Token: 0x06005068 RID: 20584 RVA: 0x00075DC2 File Offset: 0x00073FC2
		bool IPoolable.InPool
		{
			get
			{
				return this.m_inPool;
			}
			set
			{
				this.m_inPool = value;
			}
		}

		// Token: 0x06005069 RID: 20585 RVA: 0x00075DCB File Offset: 0x00073FCB
		public void InitializeNew(UniqueId archetypeId)
		{
			this.InstanceId = UniqueId.GenerateFromGuid();
			this.ArchetypeId = archetypeId;
			this.m_initialized = true;
		}

		// Token: 0x170011D6 RID: 4566
		// (get) Token: 0x0600506A RID: 20586 RVA: 0x00075DE6 File Offset: 0x00073FE6
		// (set) Token: 0x0600506B RID: 20587 RVA: 0x001CD1EC File Offset: 0x001CB3EC
		[JsonProperty]
		public int Index
		{
			get
			{
				return this.m_index;
			}
			set
			{
				if (this.m_index == value)
				{
					return;
				}
				if (this.IsAbility)
				{
					if (value == -1)
					{
						this.AbilityData.MemorizationTimestamp = null;
					}
					else if (this.m_index == -1)
					{
						this.AbilityData.MemorizationTimestamp = new DateTime?(DateTime.UtcNow);
					}
				}
				this.m_index = value;
			}
		}

		// Token: 0x170011D7 RID: 4567
		// (get) Token: 0x0600506C RID: 20588 RVA: 0x00075DEE File Offset: 0x00073FEE
		// (set) Token: 0x0600506D RID: 20589 RVA: 0x00075DF6 File Offset: 0x00073FF6
		[BsonIgnore]
		[JsonIgnore]
		public ContainerInstance PreviousContainerInstance { get; set; }

		// Token: 0x170011D8 RID: 4568
		// (get) Token: 0x0600506E RID: 20590 RVA: 0x00075DFF File Offset: 0x00073FFF
		// (set) Token: 0x0600506F RID: 20591 RVA: 0x00075E07 File Offset: 0x00074007
		[BsonIgnore]
		[JsonIgnore]
		public int PreviousIndex { get; set; } = -1;

		// Token: 0x06005070 RID: 20592 RVA: 0x001CD24C File Offset: 0x001CB44C
		public void Reset()
		{
			this.InstanceId = UniqueId.Empty;
			this.ArchetypeId = UniqueId.Empty;
			this.m_index = -1;
			this.Created = null;
			this.Modified = null;
			this.ItemData = null;
			this.MasteryData = null;
			this.AbilityData = null;
			this.LevelData = null;
			this.m_archetype = null;
			this.m_ability = null;
			this.m_dynamicAbility = null;
			this.m_mastery = null;
			this.InstanceUI = null;
			this.ContainerInstance = null;
			this.PreviousContainerInstance = null;
			this.TimeOfLastClick = DateTime.MinValue;
			this.m_initialized = false;
			if (this.SymbolicLink != null)
			{
				StaticPool<ArchetypeInstanceSymbolicLink>.ReturnToPool(this.SymbolicLink);
				this.SymbolicLink = null;
			}
		}

		// Token: 0x170011D9 RID: 4569
		// (get) Token: 0x06005071 RID: 20593 RVA: 0x001CD308 File Offset: 0x001CB508
		[BsonIgnore]
		[JsonIgnore]
		public BaseArchetype Archetype
		{
			get
			{
				if (!this.m_archetype && !this.ArchetypeId.IsEmpty)
				{
					InternalGameDatabase.Archetypes.TryGetAsType<BaseArchetype>(this.ArchetypeId, out this.m_archetype);
					if (this.m_archetype)
					{
						if (this.m_archetype.HasDynamicValues && (this.ItemData == null || this.ItemData.HasComponents))
						{
							this.m_archetype = DynamicArchetypeCache.GetOrCreate(this, this.m_archetype);
						}
					}
					else
					{
						Debug.LogError(string.Concat(new string[]
						{
							"Unable to locate Archetype for ArchetypeId: ",
							this.ArchetypeId.ToString(),
							"  (initialized=",
							this.m_initialized.ToString(),
							")"
						}));
					}
				}
				return this.m_archetype;
			}
		}

		// Token: 0x170011DA RID: 4570
		// (get) Token: 0x06005072 RID: 20594 RVA: 0x001CD3E0 File Offset: 0x001CB5E0
		[BsonIgnore]
		[JsonIgnore]
		public AbilityArchetype Ability
		{
			get
			{
				if (!this.m_ability)
				{
					InternalGameDatabase.Archetypes.TryGetAsType<AbilityArchetype>(this.ArchetypeId, out this.m_ability);
					if (this.m_ability && this.m_ability.HasDynamicValues)
					{
						this.m_ability = (AbilityArchetype)DynamicArchetypeCache.GetOrCreate(this, this.m_ability);
					}
				}
				return this.m_ability;
			}
		}

		// Token: 0x170011DB RID: 4571
		// (get) Token: 0x06005073 RID: 20595 RVA: 0x00075E10 File Offset: 0x00074010
		[BsonIgnore]
		[JsonIgnore]
		public DynamicAbility DynamicAbility
		{
			get
			{
				if (!this.m_dynamicAbility)
				{
					InternalGameDatabase.Archetypes.TryGetAsType<DynamicAbility>(this.ArchetypeId, out this.m_dynamicAbility);
				}
				return this.m_dynamicAbility;
			}
		}

		// Token: 0x170011DC RID: 4572
		// (get) Token: 0x06005074 RID: 20596 RVA: 0x001CD448 File Offset: 0x001CB648
		[BsonIgnore]
		[JsonIgnore]
		public MasteryArchetype Mastery
		{
			get
			{
				if (!this.m_mastery)
				{
					InternalGameDatabase.Archetypes.TryGetAsType<MasteryArchetype>(this.ArchetypeId, out this.m_mastery);
					if (this.m_mastery && this.m_mastery.HasDynamicValues)
					{
						this.m_mastery = (MasteryArchetype)DynamicArchetypeCache.GetOrCreate(this, this.m_mastery);
					}
				}
				return this.m_mastery;
			}
		}

		// Token: 0x170011DD RID: 4573
		// (get) Token: 0x06005075 RID: 20597 RVA: 0x00075E3C File Offset: 0x0007403C
		// (set) Token: 0x06005076 RID: 20598 RVA: 0x00075E44 File Offset: 0x00074044
		[BsonIgnore]
		[JsonIgnore]
		public ContainerInstance ContainerInstance { get; set; }

		// Token: 0x170011DE RID: 4574
		// (get) Token: 0x06005077 RID: 20599 RVA: 0x00075E4D File Offset: 0x0007404D
		// (set) Token: 0x06005078 RID: 20600 RVA: 0x00075E55 File Offset: 0x00074055
		[BsonIgnore]
		[JsonIgnore]
		public DateTime TimeOfLastClick { get; set; }

		// Token: 0x170011DF RID: 4575
		// (get) Token: 0x06005079 RID: 20601 RVA: 0x00075E5E File Offset: 0x0007405E
		// (set) Token: 0x0600507A RID: 20602 RVA: 0x00075E66 File Offset: 0x00074066
		[BsonIgnore]
		[JsonIgnore]
		public ArchetypeInstanceUI InstanceUI { get; private set; }

		// Token: 0x0600507B RID: 20603 RVA: 0x001CD4B0 File Offset: 0x001CB6B0
		public ArchetypeInstanceUI CreateItemInstanceUI()
		{
			if (this.IsMastery || GameManager.IsServer)
			{
				return null;
			}
			if (this.Archetype == null)
			{
				Debug.LogWarning("Null Archetype for " + this.ArchetypeId.ToString() + "?!");
				return null;
			}
			GameObject instanceUIPrefabReference = this.Archetype.GetInstanceUIPrefabReference();
			if (!instanceUIPrefabReference)
			{
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(instanceUIPrefabReference);
			if (!gameObject)
			{
				return null;
			}
			ArchetypeInstanceUI component = gameObject.GetComponent<ArchetypeInstanceUI>();
			if (!component)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
			else
			{
				component.Initialize(this);
			}
			this.InstanceUI = component;
			return component;
		}

		// Token: 0x170011E0 RID: 4576
		// (get) Token: 0x0600507C RID: 20604 RVA: 0x00075E6F File Offset: 0x0007406F
		[BsonIgnore]
		[JsonIgnore]
		public bool IsItem
		{
			get
			{
				return this.ItemData != null;
			}
		}

		// Token: 0x170011E1 RID: 4577
		// (get) Token: 0x0600507D RID: 20605 RVA: 0x00075E7A File Offset: 0x0007407A
		[BsonIgnore]
		[JsonIgnore]
		public bool IsMastery
		{
			get
			{
				return this.MasteryData != null;
			}
		}

		// Token: 0x170011E2 RID: 4578
		// (get) Token: 0x0600507E RID: 20606 RVA: 0x00075E85 File Offset: 0x00074085
		[BsonIgnore]
		[JsonIgnore]
		public bool IsAbility
		{
			get
			{
				return this.AbilityData != null;
			}
		}

		// Token: 0x170011E3 RID: 4579
		// (get) Token: 0x0600507F RID: 20607 RVA: 0x00075E90 File Offset: 0x00074090
		[BsonIgnore]
		[JsonIgnore]
		public int CombinedTypeCode
		{
			get
			{
				if (!this.IsItem)
				{
					return this.ArchetypeId.GetHashCode();
				}
				return this.ArchetypeId.GetHashCode() * 397 ^ this.ItemData.GetTreeFingerprint();
			}
		}

		// Token: 0x06005080 RID: 20608 RVA: 0x001CD550 File Offset: 0x001CB750
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.InstanceId);
			buffer.AddUniqueId(this.ArchetypeId);
			buffer.AddInt(this.Index);
			if (this.IsItem)
			{
				buffer.AddInt(0);
				this.ItemData.PackData(buffer);
			}
			else if (this.IsMastery)
			{
				buffer.AddInt(1);
				this.MasteryData.PackData(buffer);
			}
			else if (this.IsAbility)
			{
				buffer.AddInt(2);
				this.AbilityData.PackData(buffer);
			}
			else
			{
				buffer.AddInt(-1);
			}
			return buffer;
		}

		// Token: 0x06005081 RID: 20609 RVA: 0x001CD5EC File Offset: 0x001CB7EC
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.InstanceId = buffer.ReadUniqueId();
			this.ArchetypeId = buffer.ReadUniqueId();
			this.Index = buffer.ReadInt();
			switch (buffer.ReadInt())
			{
			case 0:
				if (this.ItemData == null)
				{
					this.ItemData = new ItemInstanceData();
				}
				this.ItemData.ReadData(buffer);
				break;
			case 1:
				if (this.MasteryData == null)
				{
					this.MasteryData = new MasteryInstanceData();
				}
				this.MasteryData.ReadData(buffer);
				break;
			case 2:
				if (this.AbilityData == null)
				{
					this.AbilityData = new AbilityInstanceData(false);
				}
				this.AbilityData.ReadData(buffer);
				break;
			}
			this.m_initialized = true;
			return buffer;
		}

		// Token: 0x06005082 RID: 20610 RVA: 0x001CD6A8 File Offset: 0x001CB8A8
		public BitBuffer PackData_BinaryIDs(BitBuffer buffer)
		{
			this.InstanceId.PackData_Binary(buffer);
			this.ArchetypeId.PackData_Binary(buffer);
			buffer.AddInt(this.Index);
			if (this.IsItem)
			{
				buffer.AddInt(0);
				this.ItemData.PackData_BinaryIDs(buffer);
			}
			else if (this.IsMastery)
			{
				buffer.AddInt(1);
				this.MasteryData.PackData(buffer);
			}
			else if (this.IsAbility)
			{
				buffer.AddInt(2);
				this.AbilityData.PackData(buffer);
			}
			else
			{
				buffer.AddInt(-1);
			}
			return buffer;
		}

		// Token: 0x06005083 RID: 20611 RVA: 0x001CD744 File Offset: 0x001CB944
		public BitBuffer ReadData_BinaryIDs(BitBuffer buffer)
		{
			this.InstanceId.ReadData_Binary(buffer, true);
			this.ArchetypeId.ReadData_Binary(buffer, false);
			this.Index = buffer.ReadInt();
			switch (buffer.ReadInt())
			{
			case 0:
				if (this.ItemData == null)
				{
					this.ItemData = new ItemInstanceData();
				}
				this.ItemData.ReadData_BinaryIDs(buffer);
				break;
			case 1:
				if (this.MasteryData == null)
				{
					this.MasteryData = new MasteryInstanceData();
				}
				this.MasteryData.ReadData(buffer);
				break;
			case 2:
				if (this.AbilityData == null)
				{
					this.AbilityData = new AbilityInstanceData(false);
				}
				this.AbilityData.ReadData(buffer);
				break;
			}
			this.m_initialized = true;
			return buffer;
		}

		// Token: 0x06005084 RID: 20612 RVA: 0x001CD804 File Offset: 0x001CBA04
		public void CopyDataFrom(ArchetypeInstance other)
		{
			this.InstanceId = other.InstanceId;
			this.ArchetypeId = other.ArchetypeId;
			this.Created = other.Created;
			this.Modified = other.Modified;
			this.m_index = other.m_index;
			this.m_archetype = other.m_archetype;
			this.m_ability = other.m_ability;
			this.m_dynamicAbility = other.m_dynamicAbility;
			this.m_mastery = other.m_mastery;
			if (other.IsItem)
			{
				this.ItemData = new ItemInstanceData();
				this.ItemData.CopyDataFrom(other.ItemData);
			}
			else if (other.IsMastery)
			{
				this.MasteryData = new MasteryInstanceData();
				this.MasteryData.CopyDataFrom(other.MasteryData);
			}
			else if (other.IsAbility)
			{
				this.AbilityData = new AbilityInstanceData(false);
				this.AbilityData.CopyDataFrom(other.AbilityData);
			}
			this.m_initialized = other.m_initialized;
		}

		// Token: 0x06005085 RID: 20613 RVA: 0x00075ECF File Offset: 0x000740CF
		public bool CanMergeWith(ArchetypeInstance incoming)
		{
			return this.IsItem && (this.ArchetypeId == incoming.ArchetypeId && this.Archetype is IStackable) && this.ItemData.CanMergeWith(incoming.ItemData);
		}

		// Token: 0x06005086 RID: 20614 RVA: 0x001CD8FC File Offset: 0x001CBAFC
		public float GetAssociatedLevel(GameEntity entity)
		{
			float result = 1f;
			if (this.IsMastery)
			{
				result = this.MasteryData.BaseLevel;
			}
			else if (this.IsAbility)
			{
				result = this.AbilityData.GetAssociatedLevel(this.Ability, entity);
			}
			return result;
		}

		// Token: 0x06005087 RID: 20615 RVA: 0x00075F0E File Offset: 0x0007410E
		public int GetAssociatedLevelInteger(GameEntity entity)
		{
			return Mathf.FloorToInt(this.GetAssociatedLevel(entity));
		}

		// Token: 0x0400483D RID: 18493
		[BsonIgnore]
		[JsonIgnore]
		private bool m_inPool;

		// Token: 0x0400483E RID: 18494
		[BsonIgnore]
		[JsonIgnore]
		private bool m_initialized;

		// Token: 0x0400483F RID: 18495
		public UniqueId InstanceId;

		// Token: 0x04004840 RID: 18496
		public UniqueId ArchetypeId;

		// Token: 0x04004841 RID: 18497
		private int m_index;

		// Token: 0x04004844 RID: 18500
		[BsonIgnoreIfNull]
		[BsonDateTimeOptions(Representation = BsonType.DateTime)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public DateTime? Created;

		// Token: 0x04004845 RID: 18501
		[BsonIgnoreIfNull]
		[BsonDateTimeOptions(Representation = BsonType.DateTime)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public DateTime? Modified;

		// Token: 0x04004846 RID: 18502
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ItemInstanceData ItemData;

		// Token: 0x04004847 RID: 18503
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public MasteryInstanceData MasteryData;

		// Token: 0x04004848 RID: 18504
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public AbilityInstanceData AbilityData;

		// Token: 0x04004849 RID: 18505
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public LevelInstanceData LevelData;

		// Token: 0x0400484A RID: 18506
		[BsonIgnore]
		[JsonIgnore]
		private BaseArchetype m_archetype;

		// Token: 0x0400484B RID: 18507
		[BsonIgnore]
		[JsonIgnore]
		private AbilityArchetype m_ability;

		// Token: 0x0400484C RID: 18508
		[BsonIgnore]
		[JsonIgnore]
		private DynamicAbility m_dynamicAbility;

		// Token: 0x0400484D RID: 18509
		[BsonIgnore]
		[JsonIgnore]
		private MasteryArchetype m_mastery;
	}
}
