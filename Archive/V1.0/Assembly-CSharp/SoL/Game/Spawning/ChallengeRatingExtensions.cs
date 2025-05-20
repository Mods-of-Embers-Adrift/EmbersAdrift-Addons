using System;
using SoL.Game.Settings;

namespace SoL.Game.Spawning
{
	// Token: 0x02000675 RID: 1653
	internal static class ChallengeRatingExtensions
	{
		// Token: 0x06003344 RID: 13124 RVA: 0x0006357D File Offset: 0x0006177D
		internal static float GetDefaultExperienceModifier(this ChallengeRating rating)
		{
			return GlobalSettings.Values.Progression.GetChallengeMod(rating);
		}

		// Token: 0x06003345 RID: 13125 RVA: 0x0006358F File Offset: 0x0006178F
		internal static int GetEmberEssenceBonus(this ChallengeRating rating, int levelDelta)
		{
			switch (rating)
			{
			case ChallengeRating.CR2:
				return 2;
			case ChallengeRating.CR3:
				return 3;
			case ChallengeRating.CR4:
			case ChallengeRating.CRB:
				return 4;
			default:
				return 0;
			}
		}
	}
}
