using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C5F RID: 3167
	public static class EffectChannelTypeExtensions
	{
		// Token: 0x06006137 RID: 24887 RVA: 0x0008184B File Offset: 0x0007FA4B
		public static EffectSubChannelType[] GetSubChannelTypes(this EffectChannelType channelType)
		{
			if (channelType <= EffectChannelType.Resist)
			{
				return EffectSubChannelTypeExtensions.DamageSubChannels;
			}
			if (channelType != EffectChannelType.Regen)
			{
				return null;
			}
			return EffectSubChannelTypeExtensions.RegenSubChannels;
		}

		// Token: 0x17001754 RID: 5972
		// (get) Token: 0x06006138 RID: 24888 RVA: 0x00081864 File Offset: 0x0007FA64
		public static EffectChannelType[] EffectChannelTypes
		{
			get
			{
				if (EffectChannelTypeExtensions.m_effectChannelTypes == null)
				{
					EffectChannelTypeExtensions.m_effectChannelTypes = (EffectChannelType[])Enum.GetValues(typeof(EffectChannelType));
				}
				return EffectChannelTypeExtensions.m_effectChannelTypes;
			}
		}

		// Token: 0x06006139 RID: 24889 RVA: 0x0008188B File Offset: 0x0007FA8B
		public static bool HasSubChannels(this EffectChannelType channel)
		{
			return channel <= EffectChannelType.Regen;
		}

		// Token: 0x0600613A RID: 24890 RVA: 0x001FF46C File Offset: 0x001FD66C
		public static string Abbreviation(this EffectChannelType type)
		{
			switch (type)
			{
			case EffectChannelType.Damage:
				return "DMG";
			case EffectChannelType.Hit:
				return "HIT";
			case EffectChannelType.Penetration:
				return "PEN";
			case EffectChannelType.Resist:
				return "RES";
			case EffectChannelType.Regen:
				return "REG";
			default:
				return type.ToString().ToUpper();
			}
		}

		// Token: 0x04005459 RID: 21593
		private static EffectChannelType[] m_effectChannelTypes = null;

		// Token: 0x0400545A RID: 21594
		public static readonly EffectChannelType[] ValidBuffChannels = new EffectChannelType[]
		{
			EffectChannelType.Damage,
			EffectChannelType.Hit,
			EffectChannelType.Penetration,
			EffectChannelType.Resist,
			EffectChannelType.Regen,
			EffectChannelType.Avoid,
			EffectChannelType.OffensiveShield,
			EffectChannelType.DefensiveShield,
			EffectChannelType.SafeFall,
			EffectChannelType.Health,
			EffectChannelType.Stamina,
			EffectChannelType.Haste,
			EffectChannelType.Movement,
			EffectChannelType.Behavior
		};

		// Token: 0x0400545B RID: 21595
		public static readonly EffectChannelType[] ValidDebuffChannelsForBase = new EffectChannelType[]
		{
			EffectChannelType.Damage,
			EffectChannelType.Hit,
			EffectChannelType.Penetration,
			EffectChannelType.Resist,
			EffectChannelType.Regen,
			EffectChannelType.Avoid,
			EffectChannelType.OffensiveShield,
			EffectChannelType.DefensiveShield,
			EffectChannelType.Health,
			EffectChannelType.Stamina,
			EffectChannelType.Haste,
			EffectChannelType.Movement
		};

		// Token: 0x0400545C RID: 21596
		public static readonly EffectChannelType[] ValidDebuffChannelsForAppliable = new EffectChannelType[]
		{
			EffectChannelType.Damage,
			EffectChannelType.Hit,
			EffectChannelType.Penetration,
			EffectChannelType.Resist,
			EffectChannelType.Regen,
			EffectChannelType.Avoid,
			EffectChannelType.OffensiveShield,
			EffectChannelType.DefensiveShield,
			EffectChannelType.Health,
			EffectChannelType.Stamina
		};

		// Token: 0x0400545D RID: 21597
		public static readonly EffectChannelType[] ValidCrowdControlsChannels = new EffectChannelType[]
		{
			EffectChannelType.Haste,
			EffectChannelType.Behavior,
			EffectChannelType.Movement
		};

		// Token: 0x0400545E RID: 21598
		public static readonly EffectChannelType[] ValidInstantTickChannels = new EffectChannelType[1];
	}
}
