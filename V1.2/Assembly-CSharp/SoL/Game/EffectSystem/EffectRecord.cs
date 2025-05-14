using System;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C3D RID: 3133
	[Serializable]
	public class EffectRecord : IEquatable<EffectRecord>, IPoolable
	{
		// Token: 0x17001736 RID: 5942
		// (get) Token: 0x060060B5 RID: 24757 RVA: 0x0008120C File Offset: 0x0007F40C
		[BsonIgnore]
		[JsonIgnore]
		public int LevelDelta
		{
			get
			{
				if (this.m_levelDelta == null)
				{
					this.m_levelDelta = new int?(this.SourceData.Level - this.TargetData.Level);
				}
				return this.m_levelDelta.Value;
			}
		}

		// Token: 0x17001737 RID: 5943
		// (get) Token: 0x060060B6 RID: 24758 RVA: 0x001FDFCC File Offset: 0x001FC1CC
		[BsonIgnore]
		[JsonIgnore]
		public ICombatEffectSource EffectSource
		{
			get
			{
				if (!this.m_cachedEffectSource && this.m_effectSource == null && !this.ArchetypeId.IsEmpty)
				{
					InternalGameDatabase.Archetypes.TryGetAsType<ICombatEffectSource>(this.ArchetypeId, out this.m_effectSource);
					this.m_cachedEffectSource = true;
				}
				return this.m_effectSource;
			}
		}

		// Token: 0x17001738 RID: 5944
		// (get) Token: 0x060060B7 RID: 24759 RVA: 0x001FE01C File Offset: 0x001FC21C
		[BsonIgnore]
		[JsonIgnore]
		public CombatEffect CombatEffect
		{
			get
			{
				if (!this.m_cachedCombatEffect && this.EffectSource != null)
				{
					CombatEffect combatEffect = this.EffectSource.GetCombatEffect((float)this.AbilityLevel, this.AlchemyPowerLevel);
					this.m_combatEffect = (this.IsSecondary ? combatEffect.SecondaryCombatEffect : combatEffect);
					this.m_cachedCombatEffect = true;
				}
				return this.m_combatEffect;
			}
		}

		// Token: 0x17001739 RID: 5945
		// (get) Token: 0x060060B8 RID: 24760 RVA: 0x001FE078 File Offset: 0x001FC278
		[BsonIgnore]
		[JsonIgnore]
		public ReagentItem ReagentItem
		{
			get
			{
				if (!this.m_cachedReagentItem && this.CombatEffectReagentId != null && this.m_reagentItem == null)
				{
					InternalGameDatabase.Archetypes.TryGetAsType<ReagentItem>(this.CombatEffectReagentId.Value, out this.m_reagentItem);
					this.m_cachedReagentItem = true;
				}
				return this.m_reagentItem;
			}
		}

		// Token: 0x060060B9 RID: 24761 RVA: 0x001FE0D4 File Offset: 0x001FC2D4
		public void Reset()
		{
			this.SourceNetworkId = 0U;
			this.SourceData.Reset();
			this.TargetData.Reset();
			this.HealthMod.Reset();
			this.StaminaMod.Reset();
			this.AbsorbedMod = 0;
			this.ThreatMod = 0;
			this.TimingData.Reset();
			this.InstanceId = UniqueId.Empty;
			this.ArchetypeId = UniqueId.Empty;
			this.CombatEffectReagentId = null;
			this.TimestampApplied = DateTime.MinValue;
			this.IsSecondary = false;
			this.BehaviorFlags = BehaviorEffectTypeFlags.None;
			this.AlchemyPowerLevel = AlchemyPowerLevel.None;
			this.DamageChannel = null;
			this.DamageChannelStatType = null;
			this.StackCount = 0;
			this.TriggerCount = 0;
			this.AbilityLevel = 0;
			this.m_levelDelta = null;
			this.m_cachedEffectSource = false;
			this.m_effectSource = null;
			this.m_cachedCombatEffect = false;
			this.m_combatEffect = null;
		}

		// Token: 0x1700173A RID: 5946
		// (get) Token: 0x060060BA RID: 24762 RVA: 0x00081248 File Offset: 0x0007F448
		// (set) Token: 0x060060BB RID: 24763 RVA: 0x00081250 File Offset: 0x0007F450
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

		// Token: 0x060060BC RID: 24764 RVA: 0x00081259 File Offset: 0x0007F459
		public bool Equals(EffectRecord other)
		{
			return other != null && (this == other || this.InstanceId.Equals(other.InstanceId));
		}

		// Token: 0x060060BD RID: 24765 RVA: 0x00081277 File Offset: 0x0007F477
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((EffectRecord)obj)));
		}

		// Token: 0x060060BE RID: 24766 RVA: 0x000812A5 File Offset: 0x0007F4A5
		public override int GetHashCode()
		{
			return this.InstanceId.GetHashCode();
		}

		// Token: 0x060060BF RID: 24767 RVA: 0x000812B8 File Offset: 0x0007F4B8
		public static bool operator ==(EffectRecord left, EffectRecord right)
		{
			return object.Equals(left, right);
		}

		// Token: 0x060060C0 RID: 24768 RVA: 0x000812C1 File Offset: 0x0007F4C1
		public static bool operator !=(EffectRecord left, EffectRecord right)
		{
			return !object.Equals(left, right);
		}

		// Token: 0x04005342 RID: 21314
		private bool m_inPool;

		// Token: 0x04005343 RID: 21315
		public uint SourceNetworkId;

		// Token: 0x04005344 RID: 21316
		public GameEntityData SourceData = new GameEntityData();

		// Token: 0x04005345 RID: 21317
		public GameEntityData TargetData = new GameEntityData();

		// Token: 0x04005346 RID: 21318
		public VitalModification HealthMod = new VitalModification();

		// Token: 0x04005347 RID: 21319
		public VitalModification StaminaMod = new VitalModification();

		// Token: 0x04005348 RID: 21320
		public int AbsorbedMod;

		// Token: 0x04005349 RID: 21321
		public int ThreatMod;

		// Token: 0x0400534A RID: 21322
		public EffectTimingData TimingData = new EffectTimingData();

		// Token: 0x0400534B RID: 21323
		public UniqueId InstanceId;

		// Token: 0x0400534C RID: 21324
		public UniqueId ArchetypeId;

		// Token: 0x0400534D RID: 21325
		public UniqueId? CombatEffectReagentId;

		// Token: 0x0400534E RID: 21326
		public DateTime TimestampApplied;

		// Token: 0x0400534F RID: 21327
		public bool IsSecondary;

		// Token: 0x04005350 RID: 21328
		public BehaviorEffectTypeFlags BehaviorFlags;

		// Token: 0x04005351 RID: 21329
		public AlchemyPowerLevel AlchemyPowerLevel;

		// Token: 0x04005352 RID: 21330
		public DamageType? DamageChannel;

		// Token: 0x04005353 RID: 21331
		public StatType? DamageChannelStatType;

		// Token: 0x04005354 RID: 21332
		public int StackCount;

		// Token: 0x04005355 RID: 21333
		public int TriggerCount;

		// Token: 0x04005356 RID: 21334
		public int AbilityLevel;

		// Token: 0x04005357 RID: 21335
		[BsonIgnore]
		[JsonIgnore]
		private int? m_levelDelta;

		// Token: 0x04005358 RID: 21336
		[BsonIgnore]
		[JsonIgnore]
		private bool m_cachedEffectSource;

		// Token: 0x04005359 RID: 21337
		[BsonIgnore]
		[JsonIgnore]
		private ICombatEffectSource m_effectSource;

		// Token: 0x0400535A RID: 21338
		[BsonIgnore]
		[JsonIgnore]
		private bool m_cachedCombatEffect;

		// Token: 0x0400535B RID: 21339
		[BsonIgnore]
		[JsonIgnore]
		private CombatEffect m_combatEffect;

		// Token: 0x0400535C RID: 21340
		[BsonIgnore]
		[JsonIgnore]
		private bool m_cachedReagentItem;

		// Token: 0x0400535D RID: 21341
		[BsonIgnore]
		[JsonIgnore]
		private ReagentItem m_reagentItem;
	}
}
