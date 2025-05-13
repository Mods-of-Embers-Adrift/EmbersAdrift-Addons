using System;

namespace SoL.Game.Loot
{
	// Token: 0x02000B09 RID: 2825
	public static class LootRollChoiceExtensions
	{
		// Token: 0x06005728 RID: 22312 RVA: 0x0007A0F5 File Offset: 0x000782F5
		public static string GetLootRollChoiceDescription(this LootRollChoice choice)
		{
			switch (choice)
			{
			case LootRollChoice.Pass:
				return "PASS";
			case LootRollChoice.Need:
				return "NEED";
			case LootRollChoice.Greed:
				return "GREED";
			default:
				return string.Empty;
			}
		}

		// Token: 0x06005729 RID: 22313 RVA: 0x0007A124 File Offset: 0x00078324
		public static bool TryGetAutoRollValue(int incomingValue, out AutoRollOption outgoingValue)
		{
			outgoingValue = AutoRollOption.None;
			if (incomingValue != 1)
			{
				if (incomingValue == 2)
				{
					outgoingValue = AutoRollOption.Pass;
				}
			}
			else
			{
				outgoingValue = AutoRollOption.Greed;
			}
			return outgoingValue > AutoRollOption.None;
		}

		// Token: 0x0600572A RID: 22314 RVA: 0x0007A140 File Offset: 0x00078340
		public static LootRollChoice GetChoiceForOption(this AutoRollOption option)
		{
			if (option == AutoRollOption.Greed)
			{
				return LootRollChoice.Greed;
			}
			if (option != AutoRollOption.Pass)
			{
				return LootRollChoice.Unanswered;
			}
			return LootRollChoice.Pass;
		}
	}
}
