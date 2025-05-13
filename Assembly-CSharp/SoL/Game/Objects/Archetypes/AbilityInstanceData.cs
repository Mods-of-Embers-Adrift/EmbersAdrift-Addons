using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using NetStack.Serialization;
using Newtonsoft.Json;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A25 RID: 2597
	[BsonIgnoreExtraElements]
	[Serializable]
	public class AbilityInstanceData : INetworkSerializable
	{
		// Token: 0x14000107 RID: 263
		// (add) Token: 0x06005040 RID: 20544 RVA: 0x001CCAF4 File Offset: 0x001CACF4
		// (remove) Token: 0x06005041 RID: 20545 RVA: 0x001CCB2C File Offset: 0x001CAD2C
		public event Action TimeOfLastUseChanged;

		// Token: 0x14000108 RID: 264
		// (add) Token: 0x06005042 RID: 20546 RVA: 0x001CCB64 File Offset: 0x001CAD64
		// (remove) Token: 0x06005043 RID: 20547 RVA: 0x001CCB9C File Offset: 0x001CAD9C
		public event Action MemorizationTimestampChanged;

		// Token: 0x14000109 RID: 265
		// (add) Token: 0x06005044 RID: 20548 RVA: 0x001CCBD4 File Offset: 0x001CADD4
		// (remove) Token: 0x06005045 RID: 20549 RVA: 0x001CCC0C File Offset: 0x001CAE0C
		public event Action AlchemyCooldownStatusChanged;

		// Token: 0x1400010A RID: 266
		// (add) Token: 0x06005046 RID: 20550 RVA: 0x001CCC44 File Offset: 0x001CAE44
		// (remove) Token: 0x06005047 RID: 20551 RVA: 0x001CCC7C File Offset: 0x001CAE7C
		public event Action UsageCountChanged;

		// Token: 0x06005048 RID: 20552 RVA: 0x00075C6B File Offset: 0x00073E6B
		public AbilityInstanceData(bool isDynamic = false)
		{
			this.IsDynamic = isDynamic;
		}

		// Token: 0x170011C9 RID: 4553
		// (get) Token: 0x06005049 RID: 20553 RVA: 0x00075C9A File Offset: 0x00073E9A
		// (set) Token: 0x0600504A RID: 20554 RVA: 0x00075CA2 File Offset: 0x00073EA2
		[BsonIgnore]
		[JsonIgnore]
		public Vector2 HealthFractionRange { get; set; } = new Vector2(0f, 1f);

		// Token: 0x170011CA RID: 4554
		// (get) Token: 0x0600504B RID: 20555 RVA: 0x00075CAB File Offset: 0x00073EAB
		[BsonIgnore]
		[JsonIgnore]
		public bool IsDynamic { get; }

		// Token: 0x170011CB RID: 4555
		// (get) Token: 0x0600504C RID: 20556 RVA: 0x0004479C File Offset: 0x0004299C
		[BsonIgnore]
		[JsonIgnore]
		public bool Trained
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170011CC RID: 4556
		// (get) Token: 0x0600504D RID: 20557 RVA: 0x00075CB3 File Offset: 0x00073EB3
		// (set) Token: 0x0600504E RID: 20558 RVA: 0x00075CBB File Offset: 0x00073EBB
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public DateTime? MemorizationTimestamp
		{
			get
			{
				return this.m_memorizationTimestamp;
			}
			set
			{
				this.m_memorizationTimestamp = value;
				Action memorizationTimestampChanged = this.MemorizationTimestampChanged;
				if (memorizationTimestampChanged == null)
				{
					return;
				}
				memorizationTimestampChanged();
			}
		}

		// Token: 0x170011CD RID: 4557
		// (get) Token: 0x0600504F RID: 20559 RVA: 0x00075CD4 File Offset: 0x00073ED4
		// (set) Token: 0x06005050 RID: 20560 RVA: 0x00075CDC File Offset: 0x00073EDC
		public DateTime TimeOfLastUse
		{
			get
			{
				return this.m_timeOfLastUse;
			}
			private set
			{
				this.m_timeOfLastUse = value;
				Action timeOfLastUseChanged = this.TimeOfLastUseChanged;
				if (timeOfLastUseChanged == null)
				{
					return;
				}
				timeOfLastUseChanged();
			}
		}

		// Token: 0x170011CE RID: 4558
		// (get) Token: 0x06005051 RID: 20561 RVA: 0x00075CF5 File Offset: 0x00073EF5
		[BsonIgnore]
		[JsonIgnore]
		public CooldownData Cooldown_Base
		{
			get
			{
				if (this.m_cooldownBase == null)
				{
					this.m_cooldownBase = new CooldownData();
				}
				return this.m_cooldownBase;
			}
		}

		// Token: 0x170011CF RID: 4559
		// (get) Token: 0x06005052 RID: 20562 RVA: 0x00075D10 File Offset: 0x00073F10
		[BsonIgnore]
		[JsonIgnore]
		public CooldownData Cooldown_AlchemyI
		{
			get
			{
				if (this.m_cooldownAlchemyI == null)
				{
					this.m_cooldownAlchemyI = new CooldownData();
				}
				return this.m_cooldownAlchemyI;
			}
		}

		// Token: 0x170011D0 RID: 4560
		// (get) Token: 0x06005053 RID: 20563 RVA: 0x00075D2B File Offset: 0x00073F2B
		[BsonIgnore]
		[JsonIgnore]
		public CooldownData Cooldown_AlchemyII
		{
			get
			{
				if (this.m_cooldownAlchemyII == null)
				{
					this.m_cooldownAlchemyII = new CooldownData();
				}
				return this.m_cooldownAlchemyII;
			}
		}

		// Token: 0x06005054 RID: 20564 RVA: 0x00075D46 File Offset: 0x00073F46
		public void TriggerAlchemyCooldownStatusChanged()
		{
			Action alchemyCooldownStatusChanged = this.AlchemyCooldownStatusChanged;
			if (alchemyCooldownStatusChanged == null)
			{
				return;
			}
			alchemyCooldownStatusChanged();
		}

		// Token: 0x170011D1 RID: 4561
		// (get) Token: 0x06005055 RID: 20565 RVA: 0x00075D58 File Offset: 0x00073F58
		// (set) Token: 0x06005056 RID: 20566 RVA: 0x00075D65 File Offset: 0x00073F65
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public float? Elapsed_Base
		{
			get
			{
				return this.Cooldown_Base.Elapsed;
			}
			set
			{
				this.Cooldown_Base.Elapsed = value;
			}
		}

		// Token: 0x170011D2 RID: 4562
		// (get) Token: 0x06005057 RID: 20567 RVA: 0x00075D73 File Offset: 0x00073F73
		// (set) Token: 0x06005058 RID: 20568 RVA: 0x00075D80 File Offset: 0x00073F80
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public float? Elapsed_AlchemyI
		{
			get
			{
				return this.Cooldown_AlchemyI.Elapsed;
			}
			set
			{
				this.Cooldown_AlchemyI.Elapsed = value;
			}
		}

		// Token: 0x170011D3 RID: 4563
		// (get) Token: 0x06005059 RID: 20569 RVA: 0x00075D8E File Offset: 0x00073F8E
		// (set) Token: 0x0600505A RID: 20570 RVA: 0x00075D9B File Offset: 0x00073F9B
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public float? Elapsed_AlchemyII
		{
			get
			{
				return this.Cooldown_AlchemyII.Elapsed;
			}
			set
			{
				this.Cooldown_AlchemyII.Elapsed = value;
			}
		}

		// Token: 0x0600505B RID: 20571 RVA: 0x001CCCB4 File Offset: 0x001CAEB4
		public int GetUsageCount(AlchemyPowerLevel alchemyPowerLevel)
		{
			if (this.UsageCounts == null)
			{
				return 0;
			}
			if (this.UsageCounts.Count <= (int)alchemyPowerLevel)
			{
				return 0;
			}
			return this.UsageCounts[(int)alchemyPowerLevel];
		}

		// Token: 0x0600505C RID: 20572 RVA: 0x001CCCEC File Offset: 0x001CAEEC
		private void IncrementUsageCount(AlchemyPowerLevel alchemyPowerLevel)
		{
			int num = AlchemyExtensions.AlchemyPowerLevels.Length;
			if (this.UsageCounts == null)
			{
				this.UsageCounts = new List<int>(num);
				for (int i = 0; i < num; i++)
				{
					this.UsageCounts.Add(0);
				}
			}
			else if (this.UsageCounts.Count < num)
			{
				int num2 = num - this.UsageCounts.Count;
				for (int j = 0; j < num2; j++)
				{
					this.UsageCounts.Add(0);
				}
			}
			if ((int)alchemyPowerLevel < this.UsageCounts.Count)
			{
				int num3 = this.UsageCounts[(int)alchemyPowerLevel];
				int num4 = num3 + 1;
				this.UsageCounts[(int)alchemyPowerLevel] = num4;
				Action usageCountChanged = this.UsageCountChanged;
				if (usageCountChanged != null)
				{
					usageCountChanged();
				}
				if (!GameManager.IsServer)
				{
					int alchemyUsageThreshold = GlobalSettings.Values.Ashen.GetAlchemyUsageThreshold(AlchemyPowerLevel.II);
					if (num3 < alchemyUsageThreshold && num4 >= alchemyUsageThreshold)
					{
						LocalPlayer.InvokeAlchemyIIUnlocked();
					}
				}
			}
		}

		// Token: 0x0600505D RID: 20573 RVA: 0x001CCDD4 File Offset: 0x001CAFD4
		public void AbilityExecuted(AlchemyPowerLevel alchemyPowerLevel = AlchemyPowerLevel.None)
		{
			this.Cooldown_Base.Elapsed = new float?(0f);
			if (alchemyPowerLevel != AlchemyPowerLevel.I)
			{
				if (alchemyPowerLevel == AlchemyPowerLevel.II)
				{
					this.Cooldown_AlchemyII.Cooldown = GlobalSettings.Values.Ashen.GetAlchemyCooldownTime(alchemyPowerLevel);
					this.Cooldown_AlchemyII.Elapsed = new float?(0f);
					this.TriggerAlchemyCooldownStatusChanged();
				}
			}
			else
			{
				this.Cooldown_AlchemyI.Cooldown = GlobalSettings.Values.Ashen.GetAlchemyCooldownTime(alchemyPowerLevel);
				this.Cooldown_AlchemyI.Elapsed = new float?(0f);
				this.TriggerAlchemyCooldownStatusChanged();
			}
			this.TimeOfLastUse = DateTime.UtcNow;
			this.IncrementUsageCount(alchemyPowerLevel);
		}

		// Token: 0x0600505E RID: 20574 RVA: 0x0004475B File Offset: 0x0004295B
		public void GM_ResetCooldown()
		{
		}

		// Token: 0x0600505F RID: 20575 RVA: 0x0004475B File Offset: 0x0004295B
		public void GM_AdjustUseCount(AlchemyPowerLevel alchemyPowerLevel, int delta)
		{
		}

		// Token: 0x06005060 RID: 20576 RVA: 0x001CCE80 File Offset: 0x001CB080
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddNullableDateTime(this.MemorizationTimestamp);
			buffer.AddDateTime(this.TimeOfLastUse);
			buffer.AddNullableFloat(this.Cooldown_Base.Elapsed);
			buffer.AddNullableFloat(this.Cooldown_AlchemyI.Elapsed);
			buffer.AddNullableFloat(this.Cooldown_AlchemyII.Elapsed);
			if (this.UsageCounts != null)
			{
				int count = this.UsageCounts.Count;
				buffer.AddByte((byte)count);
				for (int i = 0; i < this.UsageCounts.Count; i++)
				{
					buffer.AddInt(this.UsageCounts[i]);
				}
			}
			else
			{
				buffer.AddByte(0);
			}
			return buffer;
		}

		// Token: 0x06005061 RID: 20577 RVA: 0x001CCF30 File Offset: 0x001CB130
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.MemorizationTimestamp = buffer.ReadNullableDateTime();
			this.TimeOfLastUse = buffer.ReadDateTime();
			this.Cooldown_Base.Elapsed = buffer.ReadNullableFloat();
			this.Cooldown_AlchemyI.Elapsed = buffer.ReadNullableFloat();
			this.Cooldown_AlchemyII.Elapsed = buffer.ReadNullableFloat();
			byte b = buffer.ReadByte();
			if (b > 0)
			{
				this.UsageCounts = new List<int>((int)b);
				for (int i = 0; i < (int)b; i++)
				{
					this.UsageCounts.Add(buffer.ReadInt());
				}
			}
			return buffer;
		}

		// Token: 0x06005062 RID: 20578 RVA: 0x001CCFC0 File Offset: 0x001CB1C0
		public void CopyDataFrom(AbilityInstanceData other)
		{
			this.m_memorizationTimestamp = other.m_memorizationTimestamp;
			this.m_timeOfLastUse = other.m_timeOfLastUse;
			this.Cooldown_Base.CopyFrom(other.Cooldown_Base);
			this.Cooldown_AlchemyI.CopyFrom(other.Cooldown_AlchemyI);
			this.Cooldown_AlchemyII.CopyFrom(other.Cooldown_AlchemyII);
			if (other.UsageCounts != null)
			{
				this.UsageCounts = new List<int>(other.UsageCounts.Count);
				for (int i = 0; i < other.UsageCounts.Count; i++)
				{
					this.UsageCounts.Add(other.UsageCounts[i]);
				}
			}
		}

		// Token: 0x06005063 RID: 20579 RVA: 0x001CD064 File Offset: 0x001CB264
		internal float GetAssociatedLevel(AbilityArchetype ability, GameEntity entity)
		{
			if (this.IsDynamic || !ability || !entity || entity.CollectionController == null || entity.CollectionController.Masteries == null)
			{
				return 1f;
			}
			if (this.m_cachedMasteryInstance == null && !entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(ability.Mastery.Id, out this.m_cachedMasteryInstance))
			{
				return 1f;
			}
			MasteryInstanceData masteryData = this.m_cachedMasteryInstance.MasteryData;
			if (masteryData == null)
			{
				return 1f;
			}
			if (!(ability.Specialization != null))
			{
				return masteryData.BaseLevel;
			}
			if (masteryData.Specialization == null || !(masteryData.Specialization.Value == ability.Specialization.Id))
			{
				return 1f;
			}
			return masteryData.SpecializationLevel;
		}

		// Token: 0x04004832 RID: 18482
		[BsonIgnore]
		[JsonIgnore]
		public IAnimationCurve HealthFractionProbabilityCurve;

		// Token: 0x04004834 RID: 18484
		[BsonIgnore]
		[JsonIgnore]
		public AbilityCooldownFlags CooldownFlags;

		// Token: 0x04004835 RID: 18485
		private DateTime? m_memorizationTimestamp;

		// Token: 0x04004836 RID: 18486
		private DateTime m_timeOfLastUse = DateTime.MinValue;

		// Token: 0x04004837 RID: 18487
		private CooldownData m_cooldownBase;

		// Token: 0x04004838 RID: 18488
		private CooldownData m_cooldownAlchemyI;

		// Token: 0x04004839 RID: 18489
		private CooldownData m_cooldownAlchemyII;

		// Token: 0x0400483A RID: 18490
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		private List<int> UsageCounts;

		// Token: 0x0400483B RID: 18491
		[NonSerialized]
		private ArchetypeInstance m_cachedMasteryInstance;
	}
}
