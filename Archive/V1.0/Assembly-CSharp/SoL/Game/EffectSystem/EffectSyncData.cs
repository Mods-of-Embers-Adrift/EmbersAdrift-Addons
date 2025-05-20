using System;
using System.Globalization;
using NetStack.Serialization;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Networking;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C41 RID: 3137
	public struct EffectSyncData : IEquatable<EffectSyncData>, INetworkSerializable
	{
		// Token: 0x060060C8 RID: 24776 RVA: 0x001FE164 File Offset: 0x001FC364
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.InstanceId);
			buffer.AddUniqueId(this.ArchetypeId);
			buffer.AddNullableUniqueId(this.CombatEffectReagentId);
			buffer.AddString(this.ApplicatorName);
			buffer.AddUInt(this.SourceNetworkId);
			buffer.AddByte(this.Level);
			buffer.AddInt(this.Duration);
			buffer.AddDateTime(this.ExpirationTime);
			buffer.AddBool(this.Dismissible);
			buffer.AddBool(this.IsSecondary);
			buffer.AddBool(this.Diminished);
			buffer.AddBool(this.SourceIsPlayer);
			buffer.AddNullableByte(this.StackCount);
			buffer.AddNullableByte(this.TriggerCount);
			buffer.AddEnum(this.AlchemyPowerLevel);
			return buffer;
		}

		// Token: 0x060060C9 RID: 24777 RVA: 0x001FE238 File Offset: 0x001FC438
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.InstanceId = buffer.ReadUniqueId();
			this.ArchetypeId = buffer.ReadUniqueId();
			this.CombatEffectReagentId = buffer.ReadNullableUniqueId();
			this.ApplicatorName = buffer.ReadString();
			this.SourceNetworkId = buffer.ReadUInt();
			this.Level = buffer.ReadByte();
			this.Duration = buffer.ReadInt();
			this.ExpirationTime = buffer.ReadDateTime();
			this.Dismissible = buffer.ReadBool();
			this.IsSecondary = buffer.ReadBool();
			this.Diminished = buffer.ReadBool();
			this.SourceIsPlayer = buffer.ReadBool();
			this.StackCount = buffer.ReadNullableByte();
			this.TriggerCount = buffer.ReadNullableByte();
			this.AlchemyPowerLevel = buffer.ReadEnum<AlchemyPowerLevel>();
			return buffer;
		}

		// Token: 0x060060CA RID: 24778 RVA: 0x0008136C File Offset: 0x0007F56C
		public bool Equals(EffectSyncData other)
		{
			return this.InstanceId.Equals(other.InstanceId);
		}

		// Token: 0x060060CB RID: 24779 RVA: 0x001FE2FC File Offset: 0x001FC4FC
		public override bool Equals(object obj)
		{
			if (obj is EffectSyncData)
			{
				EffectSyncData other = (EffectSyncData)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x060060CC RID: 24780 RVA: 0x0008137F File Offset: 0x0007F57F
		public override int GetHashCode()
		{
			return this.InstanceId.GetHashCode();
		}

		// Token: 0x060060CD RID: 24781 RVA: 0x001FE324 File Offset: 0x001FC524
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"InstanceId: ",
				this.InstanceId.ToString(),
				", ArchetypeId: ",
				this.ArchetypeId.ToString(),
				", AppName: ",
				this.ApplicatorName,
				", Level: ",
				this.Level.ToString(),
				", Expiration: ",
				this.ExpirationTime.ToString(CultureInfo.InvariantCulture)
			});
		}

		// Token: 0x1700173B RID: 5947
		// (get) Token: 0x060060CE RID: 24782 RVA: 0x001FE3B8 File Offset: 0x001FC5B8
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

		// Token: 0x1700173C RID: 5948
		// (get) Token: 0x060060CF RID: 24783 RVA: 0x001FE408 File Offset: 0x001FC608
		public CombatEffect CombatEffect
		{
			get
			{
				if (!this.m_cachedCombatEffect && this.EffectSource != null)
				{
					CombatEffect combatEffect = this.EffectSource.GetCombatEffect((float)Mathf.Abs((int)this.Level), this.AlchemyPowerLevel);
					this.m_combatEffect = (this.IsSecondary ? combatEffect.SecondaryCombatEffect : combatEffect);
					this.m_cachedCombatEffect = true;
				}
				return this.m_combatEffect;
			}
		}

		// Token: 0x1700173D RID: 5949
		// (get) Token: 0x060060D0 RID: 24784 RVA: 0x001FE468 File Offset: 0x001FC668
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

		// Token: 0x060060D1 RID: 24785 RVA: 0x001FE4C4 File Offset: 0x001FC6C4
		public float GetTimeRemaining()
		{
			return Mathf.Clamp((float)(this.ExpirationTime - GameTimeReplicator.GetServerCorrectedDateTimeUtc()).TotalSeconds, 0f, float.MaxValue);
		}

		// Token: 0x04005368 RID: 21352
		public UniqueId InstanceId;

		// Token: 0x04005369 RID: 21353
		public UniqueId ArchetypeId;

		// Token: 0x0400536A RID: 21354
		public UniqueId? CombatEffectReagentId;

		// Token: 0x0400536B RID: 21355
		public string ApplicatorName;

		// Token: 0x0400536C RID: 21356
		public uint SourceNetworkId;

		// Token: 0x0400536D RID: 21357
		public byte Level;

		// Token: 0x0400536E RID: 21358
		public int Duration;

		// Token: 0x0400536F RID: 21359
		public DateTime ExpirationTime;

		// Token: 0x04005370 RID: 21360
		public bool Dismissible;

		// Token: 0x04005371 RID: 21361
		public bool IsSecondary;

		// Token: 0x04005372 RID: 21362
		public bool Diminished;

		// Token: 0x04005373 RID: 21363
		public bool SourceIsPlayer;

		// Token: 0x04005374 RID: 21364
		public byte? StackCount;

		// Token: 0x04005375 RID: 21365
		public byte? TriggerCount;

		// Token: 0x04005376 RID: 21366
		public AlchemyPowerLevel AlchemyPowerLevel;

		// Token: 0x04005377 RID: 21367
		private bool m_cachedEffectSource;

		// Token: 0x04005378 RID: 21368
		private ICombatEffectSource m_effectSource;

		// Token: 0x04005379 RID: 21369
		private bool m_cachedCombatEffect;

		// Token: 0x0400537A RID: 21370
		private CombatEffect m_combatEffect;

		// Token: 0x0400537B RID: 21371
		private bool m_cachedReagentItem;

		// Token: 0x0400537C RID: 21372
		private ReagentItem m_reagentItem;
	}
}
