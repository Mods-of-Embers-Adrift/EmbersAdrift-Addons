using System;
using SoL.Game.Messages;

namespace SoL.Game.UI.Chat
{
	// Token: 0x020009B4 RID: 2484
	public static class CombatFilterExtensions
	{
		// Token: 0x06004B02 RID: 19202 RVA: 0x00072A69 File Offset: 0x00070C69
		public static MessageType GetMessageType(this CombatFilter filter)
		{
			switch (filter)
			{
			case CombatFilter.MyCombatOut:
				return MessageType.MyCombatOut;
			case CombatFilter.MyCombatIn:
				return MessageType.MyCombatIn;
			case CombatFilter.MyCombatOut | CombatFilter.MyCombatIn:
				break;
			case CombatFilter.OtherCombat:
				return MessageType.OtherCombat;
			default:
				if (filter == CombatFilter.WarlordSongs)
				{
					return MessageType.WarlordSong;
				}
				break;
			}
			return MessageType.None;
		}

		// Token: 0x06004B03 RID: 19203 RVA: 0x001B7784 File Offset: 0x001B5984
		public static MessageType GetMessageTypeFlags(this CombatFilter filter)
		{
			MessageType messageType = MessageType.System;
			if (filter.HasBitFlag(CombatFilter.MyCombatOut))
			{
				messageType |= MessageType.MyCombatOut;
			}
			if (filter.HasBitFlag(CombatFilter.MyCombatIn))
			{
				messageType |= MessageType.MyCombatIn;
			}
			if (filter.HasBitFlag(CombatFilter.OtherCombat))
			{
				messageType |= MessageType.OtherCombat;
			}
			if (filter.HasBitFlag(CombatFilter.WarlordSongs))
			{
				messageType |= MessageType.WarlordSong;
			}
			return messageType;
		}

		// Token: 0x17001081 RID: 4225
		// (get) Token: 0x06004B04 RID: 19204 RVA: 0x00072AA2 File Offset: 0x00070CA2
		public static CombatFilter[] CombatFilters
		{
			get
			{
				if (CombatFilterExtensions.m_combatFilters == null)
				{
					CombatFilterExtensions.m_combatFilters = (CombatFilter[])Enum.GetValues(typeof(CombatFilter));
				}
				return CombatFilterExtensions.m_combatFilters;
			}
		}

		// Token: 0x06004B05 RID: 19205 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this CombatFilter a, CombatFilter b)
		{
			return (a & b) == b;
		}

		// Token: 0x040045AC RID: 17836
		private static CombatFilter[] m_combatFilters;
	}
}
