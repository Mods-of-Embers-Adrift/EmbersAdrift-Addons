using System;
using Cysharp.Text;
using SoL.Game.Messages;
using SoL.Game.Settings;
using SoL.Game.Spawning;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Targeting
{
	// Token: 0x02000653 RID: 1619
	public static class ITargetableExtensions
	{
		// Token: 0x06003274 RID: 12916 RVA: 0x0015FC4C File Offset: 0x0015DE4C
		public static DifficultyRating GetDifficultyRating(this ITargetable targetable, bool useGelLevel = false)
		{
			DifficultyRating result = DifficultyRating.None;
			if (targetable != null && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData)
			{
				int num = useGelLevel ? LocalPlayer.GameEntity.CharacterData.GetGroupedLevel() : LocalPlayer.GameEntity.CharacterData.AdventuringLevel;
				int level = targetable.Level;
				if (level == 0)
				{
					result = DifficultyRating.Normal;
				}
				else if (level == num)
				{
					result = DifficultyRating.Normal;
				}
				else if (level > num)
				{
					int num2 = level - num;
					if ((float)num2 >= 5f)
					{
						result = DifficultyRating.TooDifficult;
					}
					else if (num2 > 2)
					{
						result = DifficultyRating.VeryDifficult;
					}
					else
					{
						result = DifficultyRating.Difficult;
					}
				}
				else if (num > level)
				{
					int num3 = num - level;
					if (num3 > 10)
					{
						result = DifficultyRating.TooEasy;
					}
					else if (num3 > 5)
					{
						result = DifficultyRating.VeryEasy;
					}
					else if (num3 > 2)
					{
						result = DifficultyRating.RelativelyEasy;
					}
					else
					{
						result = DifficultyRating.Easy;
					}
				}
			}
			return result;
		}

		// Token: 0x06003275 RID: 12917 RVA: 0x00062C31 File Offset: 0x00060E31
		public static ChallengeRating GetChallengeRating(this ITargetable targetable)
		{
			if (targetable == null || !targetable.Entity || !targetable.Entity.CharacterData)
			{
				return ChallengeRating.CR0;
			}
			return targetable.Entity.CharacterData.ChallengeRating;
		}

		// Token: 0x06003276 RID: 12918 RVA: 0x0015FD08 File Offset: 0x0015DF08
		public static void EchoChallengeTextToChat(ITargetable targetable)
		{
			if (targetable == null)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Emote, "Nothing to consider!");
				return;
			}
			if (GlobalSettings.Values && GlobalSettings.Values.Npcs != null)
			{
				ChallengeRating challengeRating = targetable.GetChallengeRating();
				DifficultyRating difficultyRating = targetable.GetDifficultyRating(false);
				Color difficultyRatingColor = GlobalSettings.Values.Npcs.GetDifficultyRatingColor(difficultyRating);
				string difficultyChallengeText = GlobalSettings.Values.Npcs.GetDifficultyChallengeText(difficultyRating, challengeRating);
				string content = ZString.Format<string, string>("<color={0}>{1}</color>", difficultyRatingColor.ToHex(), difficultyChallengeText);
				MessageManager.ChatQueue.AddToQueue(MessageType.Emote | MessageType.PreFormatted, content);
				return;
			}
			MessageManager.ChatQueue.AddToQueue(MessageType.Emote, "Unable to consider!");
		}

		// Token: 0x040030D8 RID: 12504
		private const MessageType kConsiderMessageType = MessageType.Emote;
	}
}
