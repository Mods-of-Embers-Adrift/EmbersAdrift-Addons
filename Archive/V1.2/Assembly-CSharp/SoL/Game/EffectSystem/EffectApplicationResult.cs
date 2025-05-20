using System;
using Cysharp.Text;
using NetStack.Serialization;
using SoL.Networking;
using SoL.Utilities;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C3B RID: 3131
	public class EffectApplicationResult : INetworkSerializable, IPoolable
	{
		// Token: 0x060060A8 RID: 24744 RVA: 0x000811BD File Offset: 0x0007F3BD
		public override string ToString()
		{
			return ZString.Format<uint, uint, string, EffectApplicationFlags, UniqueId, byte, AlchemyPowerLevel>("SourceId: {0}, TargetId: {1}, SourceName: {2}, Flags: {3}, ArchetypeId: {4}, AbilityLevel: {5}, AlchemyPowerLevel: {6}", this.SourceId, this.TargetId, this.SourceName, this.Flags, this.ArchetypeId, this.AbilityLevel, this.AlchemyPowerLevel);
		}

		// Token: 0x060060A9 RID: 24745 RVA: 0x001FDAE4 File Offset: 0x001FBCE4
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddBool(this.TriggerOnAnimEvent);
			buffer.AddUInt(this.SourceId);
			buffer.AddUInt(this.TargetId);
			buffer.AddNullableString(this.SourceName);
			buffer.AddEnum(this.Flags);
			buffer.AddUniqueId(this.ArchetypeId);
			buffer.AddBool(this.IsSecondary);
			buffer.AddByte(this.AbilityLevel);
			buffer.AddEnum(this.AlchemyPowerLevel);
			buffer.AddNullableFloat(this.HealthAdjustment);
			buffer.AddNullableFloat(this.HealthWoundAdjustment);
			buffer.AddNullableFloat(this.StaminaAdjustment);
			buffer.AddNullableFloat(this.StaminaWoundAdjustment);
			buffer.AddNullableFloat(this.Absorbed);
			buffer.AddNullableFloat(this.Threat);
			buffer.AddNullableFloat(this.Defended);
			buffer.AddNullableInt(this.OverTimeAdjustment);
			return buffer;
		}

		// Token: 0x060060AA RID: 24746 RVA: 0x001FDBD0 File Offset: 0x001FBDD0
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.TriggerOnAnimEvent = buffer.ReadBool();
			this.SourceId = buffer.ReadUInt();
			this.TargetId = buffer.ReadUInt();
			this.SourceName = buffer.ReadNullableString();
			this.Flags = buffer.ReadEnum<EffectApplicationFlags>();
			this.ArchetypeId = buffer.ReadUniqueId();
			this.IsSecondary = buffer.ReadBool();
			this.AbilityLevel = buffer.ReadByte();
			this.AlchemyPowerLevel = buffer.ReadEnum<AlchemyPowerLevel>();
			this.HealthAdjustment = buffer.ReadNullableFloat();
			this.HealthWoundAdjustment = buffer.ReadNullableFloat();
			this.StaminaAdjustment = buffer.ReadNullableFloat();
			this.StaminaWoundAdjustment = buffer.ReadNullableFloat();
			this.Absorbed = buffer.ReadNullableFloat();
			this.Threat = buffer.ReadNullableFloat();
			this.Defended = buffer.ReadNullableFloat();
			this.OverTimeAdjustment = buffer.ReadNullableInt();
			return buffer;
		}

		// Token: 0x060060AB RID: 24747 RVA: 0x001FDCAC File Offset: 0x001FBEAC
		public void ResetAll()
		{
			this.TriggerOnAnimEvent = false;
			this.SourceId = 0U;
			this.TargetId = 0U;
			this.SourceName = string.Empty;
			this.Flags = EffectApplicationFlags.None;
			this.ArchetypeId = UniqueId.Empty;
			this.IsSecondary = false;
			this.AbilityLevel = 0;
			this.AlchemyPowerLevel = AlchemyPowerLevel.None;
			this.ResetValues();
		}

		// Token: 0x060060AC RID: 24748 RVA: 0x001FDD08 File Offset: 0x001FBF08
		public void ResetValues()
		{
			this.HealthAdjustment = null;
			this.HealthWoundAdjustment = null;
			this.StaminaAdjustment = null;
			this.StaminaWoundAdjustment = null;
			this.Absorbed = null;
			this.Threat = null;
			this.Defended = null;
			this.OverTimeAdjustment = null;
		}

		// Token: 0x060060AD RID: 24749 RVA: 0x000811F3 File Offset: 0x0007F3F3
		void IPoolable.Reset()
		{
			this.ResetAll();
		}

		// Token: 0x17001735 RID: 5941
		// (get) Token: 0x060060AE RID: 24750 RVA: 0x000811FB File Offset: 0x0007F3FB
		// (set) Token: 0x060060AF RID: 24751 RVA: 0x00081203 File Offset: 0x0007F403
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

		// Token: 0x060060B0 RID: 24752 RVA: 0x001FDD78 File Offset: 0x001FBF78
		public bool PlayNpcLevelUpVfx(out Random random)
		{
			int num = this.ArchetypeId.GetHashCode();
			num = (num * 397 ^ (int)this.SourceId);
			num = (num * 397 ^ (int)this.TargetId);
			num = (num * 397 ^ this.GetAdjustmentHashCode());
			random = new Random(num);
			return random.NextDouble() <= 0.20000000298023224;
		}

		// Token: 0x060060B1 RID: 24753 RVA: 0x001FDDE4 File Offset: 0x001FBFE4
		private int GetAdjustmentHashCode()
		{
			if (this.HealthAdjustment != null)
			{
				return this.HealthAdjustment.Value.GetHashCode();
			}
			if (this.HealthWoundAdjustment != null)
			{
				return this.HealthWoundAdjustment.Value.GetHashCode();
			}
			if (this.StaminaAdjustment != null)
			{
				return this.StaminaAdjustment.Value.GetHashCode();
			}
			if (this.StaminaWoundAdjustment != null)
			{
				return this.StaminaWoundAdjustment.Value.GetHashCode();
			}
			if (this.Absorbed != null)
			{
				return this.Absorbed.Value.GetHashCode();
			}
			if (this.Threat != null)
			{
				return this.Threat.Value.GetHashCode();
			}
			if (this.Defended != null)
			{
				return this.Defended.Value.GetHashCode();
			}
			if (this.OverTimeAdjustment != null)
			{
				return this.OverTimeAdjustment.Value.GetHashCode();
			}
			return 0;
		}

		// Token: 0x04005329 RID: 21289
		private bool m_inPool;

		// Token: 0x0400532A RID: 21290
		public bool TriggerOnAnimEvent;

		// Token: 0x0400532B RID: 21291
		public uint SourceId;

		// Token: 0x0400532C RID: 21292
		public uint TargetId;

		// Token: 0x0400532D RID: 21293
		public string SourceName;

		// Token: 0x0400532E RID: 21294
		public EffectApplicationFlags Flags;

		// Token: 0x0400532F RID: 21295
		public UniqueId ArchetypeId;

		// Token: 0x04005330 RID: 21296
		public bool IsSecondary;

		// Token: 0x04005331 RID: 21297
		public byte AbilityLevel;

		// Token: 0x04005332 RID: 21298
		public AlchemyPowerLevel AlchemyPowerLevel;

		// Token: 0x04005333 RID: 21299
		public float? HealthAdjustment;

		// Token: 0x04005334 RID: 21300
		public float? HealthWoundAdjustment;

		// Token: 0x04005335 RID: 21301
		public float? StaminaAdjustment;

		// Token: 0x04005336 RID: 21302
		public float? StaminaWoundAdjustment;

		// Token: 0x04005337 RID: 21303
		public float? Absorbed;

		// Token: 0x04005338 RID: 21304
		public float? Threat;

		// Token: 0x04005339 RID: 21305
		public float? Defended;

		// Token: 0x0400533A RID: 21306
		public int? OverTimeAdjustment;
	}
}
