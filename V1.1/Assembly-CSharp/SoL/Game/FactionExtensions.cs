using System;
using SoL.Game.Targeting;

namespace SoL.Game
{
	// Token: 0x02000577 RID: 1399
	public static class FactionExtensions
	{
		// Token: 0x06002B2E RID: 11054 RVA: 0x00146410 File Offset: 0x00144610
		public static bool IsHostileTo(this Faction sourceFaction, Faction targetFaction)
		{
			if (sourceFaction == targetFaction)
			{
				return false;
			}
			switch (sourceFaction)
			{
			case Faction.Player:
				return targetFaction == Faction.Bandit;
			case Faction.Guard:
				return targetFaction != Faction.Player && targetFaction != Faction.Neutral;
			case Faction.Bandit:
				return targetFaction == Faction.Player || targetFaction == Faction.Guard;
			case Faction.AnimalAggressive:
				return targetFaction != Faction.Neutral && targetFaction != Faction.Bandit && targetFaction != Faction.AnimalPassive;
			case Faction.AnimalPassive:
				return false;
			default:
				return true;
			}
		}

		// Token: 0x06002B2F RID: 11055 RVA: 0x0005DF95 File Offset: 0x0005C195
		public static TargetType GetPlayerTargetType(this Faction faction)
		{
			if (faction - Faction.Player <= 1)
			{
				return TargetType.Defensive;
			}
			return TargetType.Offensive;
		}
	}
}
