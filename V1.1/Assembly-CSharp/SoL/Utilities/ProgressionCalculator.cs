using System;
using System.Collections.Generic;
using SoL.Game;
using SoL.Game.Messages;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Settings;
using SoL.Game.Spawning;
using SoL.Managers;
using SoL.Utilities.Extensions;
using SoL.Utilities.Logging;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002AF RID: 687
	public static class ProgressionCalculator
	{
		// Token: 0x0600147C RID: 5244 RVA: 0x0005047F File Offset: 0x0004E67F
		public static void OnGatheringSuccess(GameEntity entity, ArchetypeInstance masteryInstance, int targetLevel, float experienceMultiplier)
		{
			ProgressionCalculator.OnTradeSkillSuccess(entity, masteryInstance, targetLevel, GlobalSettings.Values.Progression.GatheringLevelCurve, experienceMultiplier, true);
		}

		// Token: 0x0600147D RID: 5245 RVA: 0x0005049A File Offset: 0x0004E69A
		public static void OnCraftingSuccess(GameEntity entity, ArchetypeInstance masteryInstance, int targetLevel, float experienceMultiplier, bool progressSpecialization)
		{
			ProgressionCalculator.OnTradeSkillSuccess(entity, masteryInstance, targetLevel, GlobalSettings.Values.Progression.CraftingLevelCurve, experienceMultiplier, progressSpecialization);
		}

		// Token: 0x0600147E RID: 5246 RVA: 0x000504B6 File Offset: 0x0004E6B6
		public static void OnDeconstructSuccess(GameEntity entity, ArchetypeInstance masteryInstance, int targetLevel, bool progressSpecialization)
		{
			ProgressionCalculator.OnTradeSkillSuccess(entity, masteryInstance, targetLevel, GlobalSettings.Values.Progression.CraftingLevelCurve, GlobalSettings.Values.Crafting.DeconstructExperienceMultiplier, progressSpecialization);
		}

		// Token: 0x0600147F RID: 5247 RVA: 0x000FA5B8 File Offset: 0x000F87B8
		private static void OnTradeSkillSuccess(GameEntity entity, ArchetypeInstance masteryInstance, int targetLevel, SuccessRewardCurve levelCurve, float experienceMultiplier, bool progressSpecialization)
		{
			if (!entity || masteryInstance == null || masteryInstance.MasteryData == null || targetLevel <= 0 || levelCurve == null)
			{
				return;
			}
			float associatedLevel = masteryInstance.GetAssociatedLevel(entity);
			int level = Mathf.Min(Mathf.FloorToInt(associatedLevel), targetLevel);
			float num = levelCurve.GetRewardForLevel(level) * experienceMultiplier;
			if (associatedLevel > (float)targetLevel)
			{
				float num2 = 50f - (float)targetLevel;
				float time = (associatedLevel - (float)targetLevel) / num2;
				num *= GlobalSettings.Values.Progression.ExperienceFalloff.Evaluate(time);
			}
			int maxTradeLevel = GlobalSettings.Values.Crafting.GetMaxTradeLevel(entity.CharacterData.AdventuringLevel);
			ProgressionCalculator.OnExperienceAwarded(entity, masteryInstance, (double)num, levelCurve, (float)maxTradeLevel, progressSpecialization, false);
		}

		// Token: 0x06001480 RID: 5248 RVA: 0x000FA65C File Offset: 0x000F885C
		public static void OnNpcKilled(GameEntity entity, ulong experience, int targetLevel, bool hasBonusXp)
		{
			if (entity && entity.CharacterData && !entity.CharacterData.BaseRoleId.IsEmpty)
			{
				ICollectionController collectionController = entity.CollectionController;
				if (((collectionController != null) ? collectionController.Masteries : null) != null && !entity.CharacterData.PauseAdventuringExperience)
				{
					BaseRole baseRole;
					ArchetypeInstance archetypeInstance;
					if (InternalGameDatabase.Archetypes.TryGetAsType<BaseRole>(entity.CharacterData.BaseRoleId, out baseRole) && baseRole.Type.GetMasterySphere() == MasterySphere.Adventuring && entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(baseRole.Id, out archetypeInstance) && archetypeInstance.MasteryData != null)
					{
						double experience2 = ProgressionCalculator.ModifyAdventuringExperienceBasedOnLevel(archetypeInstance.GetAssociatedLevelInteger(entity), targetLevel, experience);
						ProgressionCalculator.OnExperienceAwarded(entity, archetypeInstance, experience2, GlobalSettings.Values.Progression.AdventuringLevelCurve, 50f, true, hasBonusXp);
					}
					return;
				}
			}
		}

		// Token: 0x06001481 RID: 5249 RVA: 0x000FA72C File Offset: 0x000F892C
		public static void OnAdventuringTaskCompleted(GameEntity entity, int taskLevel, int playerLevel)
		{
			if (entity && entity.CharacterData && !entity.CharacterData.BaseRoleId.IsEmpty)
			{
				ICollectionController collectionController = entity.CollectionController;
				if (((collectionController != null) ? collectionController.Masteries : null) != null && !entity.CharacterData.PauseAdventuringExperience)
				{
					BaseRole baseRole;
					ArchetypeInstance archetypeInstance;
					if (InternalGameDatabase.Archetypes.TryGetAsType<BaseRole>(entity.CharacterData.BaseRoleId, out baseRole) && baseRole.Type.GetMasterySphere() == MasterySphere.Adventuring && entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(baseRole.Id, out archetypeInstance) && archetypeInstance.MasteryData != null)
					{
						float adventuringTaskExperience = GlobalSettings.Values.Progression.GetAdventuringTaskExperience(taskLevel, playerLevel);
						ProgressionCalculator.OnExperienceAwarded(entity, archetypeInstance, (double)adventuringTaskExperience, GlobalSettings.Values.Progression.AdventuringLevelCurve, 50f, true, false);
					}
					return;
				}
			}
		}

		// Token: 0x06001482 RID: 5250 RVA: 0x000FA800 File Offset: 0x000F8A00
		public static void OnGatheringCraftingTaskCompleted(GameEntity entity, int taskLevel, int playerLevel, BaseRole baseRole)
		{
			if (baseRole && entity && entity.CharacterData && !entity.CharacterData.BaseRoleId.IsEmpty)
			{
				ICollectionController collectionController = entity.CollectionController;
				if (((collectionController != null) ? collectionController.Masteries : null) != null)
				{
					ArchetypeInstance archetypeInstance;
					if (entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(baseRole.Id, out archetypeInstance) && archetypeInstance.MasteryData != null)
					{
						SuccessRewardCurve levelCurve = null;
						float num = 0f;
						MasteryType type = baseRole.Type;
						if (type != MasteryType.Trade)
						{
							if (type == MasteryType.Harvesting)
							{
								levelCurve = GlobalSettings.Values.Progression.GatheringLevelCurve;
								num = GlobalSettings.Values.Progression.GetGatheringTaskExperience(taskLevel, playerLevel);
							}
						}
						else
						{
							levelCurve = GlobalSettings.Values.Progression.CraftingLevelCurve;
							num = GlobalSettings.Values.Progression.GetCraftingTaskExperience(taskLevel, playerLevel);
						}
						int maxTradeLevel = GlobalSettings.Values.Crafting.GetMaxTradeLevel(entity.CharacterData.AdventuringLevel);
						ProgressionCalculator.OnExperienceAwarded(entity, archetypeInstance, (double)num, levelCurve, (float)maxTradeLevel, true, false);
					}
					return;
				}
			}
		}

		// Token: 0x06001483 RID: 5251 RVA: 0x000FA908 File Offset: 0x000F8B08
		private static void OnExperienceAwarded(GameEntity entity, ArchetypeInstance masteryInstance, double experience, SuccessRewardCurve levelCurve, float maxLevel, bool progressSpecialization, bool hasBonusXp = false)
		{
			MasteryArchetype masteryArchetype;
			if (!entity || masteryInstance == null || masteryInstance.Archetype == null || !masteryInstance.Archetype.TryGetAsType(out masteryArchetype) || levelCurve == null || entity.Type != GameEntityType.Player || entity.CollectionController == null || entity.CollectionController.Abilities == null || entity.CollectionController.Masteries == null)
			{
				return;
			}
			if (experience <= 0.0)
			{
				return;
			}
			float associatedLevel = masteryInstance.GetAssociatedLevel(entity);
			float num;
			if (ProgressionCalculator.AddExperience(associatedLevel, experience, maxLevel, levelCurve, out num) || ProgressionCalculator.ShouldAddSpecializationExperience(experience, masteryInstance.MasteryData, maxLevel))
			{
				masteryInstance.MasteryData.BaseLevel = num;
				LevelProgressionUpdate levelProgressionUpdate = new LevelProgressionUpdate
				{
					HasBonusXp = hasBonusXp,
					ArchetypeId = masteryInstance.ArchetypeId,
					BaseLevel = num
				};
				if (Mathf.FloorToInt(associatedLevel) < Mathf.FloorToInt(num))
				{
					ServerGameManager.SpatialManager.MessageNearbyPlayers(entity, 30f, MessageType.Emote | MessageType.PreFormatted, entity.CharacterData.Name.Value + " has gained a level!", masteryArchetype.Type.GetMessageEventType(), true);
					ProgressionCalculator.LogLevelProgressionEvent(entity, masteryInstance, num);
				}
				entity.Vitals.MasteryLevelChanged(masteryInstance, associatedLevel, num);
				entity.CharacterData.UpdateHighestMasteryLevel(masteryInstance);
				float num2;
				if (progressSpecialization && masteryInstance.MasteryData.Specialization != null && ProgressionCalculator.AddExperience(masteryInstance.MasteryData.SpecializationLevel, experience * (double)GlobalSettings.Values.Progression.SpecializationExperienceMultiplier, num, levelCurve, out num2))
				{
					if (num2 > num)
					{
						num2 = num;
					}
					masteryInstance.MasteryData.SpecializationLevel = num2;
					levelProgressionUpdate.SpecializationLevel = new float?(num2);
				}
				entity.NetworkEntity.PlayerRpcHandler.Server_LevelProgressionUpdate(levelProgressionUpdate);
			}
		}

		// Token: 0x06001484 RID: 5252 RVA: 0x000FAAB8 File Offset: 0x000F8CB8
		private static bool AddExperience(float initialLevel, double experience, float maxLevel, SuccessRewardCurve levelCurve, out float resultLevel)
		{
			if (experience <= 0.0 || initialLevel >= maxLevel)
			{
				resultLevel = initialLevel;
				return false;
			}
			float num = initialLevel;
			int nextLevel = ProgressionCalculator.GetNextLevel(num);
			float experienceRequiredForLevel = ProgressionCalculator.GetExperienceRequiredForLevel(nextLevel, levelCurve);
			double num2;
			for (num2 = experience / (double)experienceRequiredForLevel; num2 >= 1.0; num2 = experience / (double)experienceRequiredForLevel)
			{
				double num3 = num2 - 1.0;
				experience -= (double)experienceRequiredForLevel * num3;
				num = (float)nextLevel;
				nextLevel = ProgressionCalculator.GetNextLevel(num);
				experienceRequiredForLevel = ProgressionCalculator.GetExperienceRequiredForLevel(nextLevel, levelCurve);
			}
			if ((double)num + num2 >= (double)nextLevel)
			{
				float num4 = (float)nextLevel - num;
				experience -= (double)(experienceRequiredForLevel * num4);
				num = (float)nextLevel;
				nextLevel = ProgressionCalculator.GetNextLevel(num);
				experienceRequiredForLevel = ProgressionCalculator.GetExperienceRequiredForLevel(nextLevel, levelCurve);
				num2 = experience / (double)experienceRequiredForLevel;
			}
			resultLevel = Mathf.Clamp((float)((double)num + num2), 1f, maxLevel);
			return initialLevel != resultLevel;
		}

		// Token: 0x06001485 RID: 5253 RVA: 0x000FAB78 File Offset: 0x000F8D78
		private static bool ShouldAddSpecializationExperience(double experience, MasteryInstanceData masteryData, float maxLevel)
		{
			return experience > 0.0 && masteryData != null && masteryData.BaseLevel >= maxLevel && masteryData.Specialization != null && masteryData.SpecializationLevel < maxLevel;
		}

		// Token: 0x06001486 RID: 5254 RVA: 0x000504DF File Offset: 0x0004E6DF
		private static int GetNextLevel(float level)
		{
			return Mathf.FloorToInt(level + 1f);
		}

		// Token: 0x06001487 RID: 5255 RVA: 0x000504ED File Offset: 0x0004E6ED
		private static float GetExperienceRequiredForLevel(int level, SuccessRewardCurve levelCurve)
		{
			if (levelCurve == null)
			{
				throw new ArgumentNullException("levelCurve");
			}
			return levelCurve.GetTotalForLevel(level);
		}

		// Token: 0x06001488 RID: 5256 RVA: 0x000FABB8 File Offset: 0x000F8DB8
		public static double ModifyAdventuringExperienceBasedOnLevel(int playerLevel, int targetLevel, double experience)
		{
			float xpMultiplier = ProgressionCalculator.GetXpMultiplier(playerLevel, targetLevel);
			if (playerLevel < targetLevel)
			{
				int num = targetLevel - playerLevel;
				if (num > 2)
				{
					float rewardForLevel = GlobalSettings.Values.Progression.AdventuringLevelCurve.GetRewardForLevel(targetLevel);
					double num2 = experience / (double)rewardForLevel;
					num = Mathf.Min(num, 5);
					int num3 = num - 2;
					int level = Mathf.Clamp(playerLevel - num3, 1, 50);
					experience = (double)GlobalSettings.Values.Progression.AdventuringLevelCurve.GetRewardForLevel(level) * num2;
				}
			}
			return experience * (double)xpMultiplier;
		}

		// Token: 0x06001489 RID: 5257 RVA: 0x000FAC30 File Offset: 0x000F8E30
		public static float GetXpMultiplier(int playerLevel, int targetLevel)
		{
			float result = 1f;
			if (playerLevel != targetLevel)
			{
				if (playerLevel > targetLevel)
				{
					int num = playerLevel - targetLevel;
					if (num > 10)
					{
						result = 0f;
					}
					else if (num > 2)
					{
						result = ProgressionCalculator.GetPlayerHigherLevelXpFraction(num);
					}
				}
				else if (playerLevel >= targetLevel)
				{
					result = 0f;
					Debug.LogError("How did I get here??");
				}
			}
			return result;
		}

		// Token: 0x0600148A RID: 5258 RVA: 0x000FAC80 File Offset: 0x000F8E80
		private static float GetPlayerHigherLevelXpFraction(int levelDelta)
		{
			if (levelDelta > 10)
			{
				return 0f;
			}
			if (levelDelta < 3)
			{
				return 1f;
			}
			float result;
			if (!ProgressionCalculator.DiminishingXpFraction.TryGetValue(levelDelta, out result))
			{
				return 1f;
			}
			return result;
		}

		// Token: 0x0600148B RID: 5259 RVA: 0x000FACB8 File Offset: 0x000F8EB8
		public static ulong GetExperiencePerGroupMember(float totalXp, int groupCount, ChallengeRating challengeRating)
		{
			float num = 1f + ((float)groupCount - 1f) * 0.4f;
			float num2 = totalXp * num * GlobalSettings.Values.Progression.GetGroupSplitMod(groupCount) / (float)groupCount;
			if (challengeRating - ChallengeRating.CR1 <= 1)
			{
				int num3 = (challengeRating == ChallengeRating.CR2) ? 3 : 1;
				if (groupCount > num3)
				{
					float num4 = GlobalSettings.Values.Progression.DiminishingExperienceMultiplier * challengeRating.GetDefaultExperienceModifier();
					int num5 = groupCount - num3;
					num2 *= 1f - num4 * (float)num5;
				}
			}
			return (ulong)Mathf.Clamp(num2, 1f, 2.1474836E+09f);
		}

		// Token: 0x0600148C RID: 5260 RVA: 0x000FAD40 File Offset: 0x000F8F40
		private static void LogLevelProgressionEvent(GameEntity entity, ArchetypeInstance masteryInstance, float resultLevel)
		{
			if (!entity || masteryInstance == null)
			{
				return;
			}
			BaseRole baseRole;
			MasteryType masteryType = masteryInstance.Archetype.TryGetAsType(out baseRole) ? baseRole.Type : MasteryType.None;
			string text = "None";
			string text2 = text;
			if (masteryInstance.MasteryData.Specialization != null)
			{
				text = masteryInstance.MasteryData.Specialization.Value.ToString();
				SpecializedRole specializedRole;
				if (InternalGameDatabase.Archetypes.TryGetAsType<SpecializedRole>(masteryInstance.MasteryData.Specialization.Value, out specializedRole))
				{
					text2 = specializedRole.DisplayName;
				}
			}
			ProgressionCalculator.ProgressionEventArray[0] = LocalZoneManager.ZoneRecord.DisplayName;
			ProgressionCalculator.ProgressionEventArray[1] = Mathf.FloorToInt(resultLevel);
			ProgressionCalculator.ProgressionEventArray[2] = masteryType.ToString();
			ProgressionCalculator.ProgressionEventArray[3] = masteryInstance.ArchetypeId.ToString();
			ProgressionCalculator.ProgressionEventArray[4] = masteryInstance.Archetype.DisplayName;
			ProgressionCalculator.ProgressionEventArray[5] = text;
			ProgressionCalculator.ProgressionEventArray[6] = text2;
			ProgressionCalculator.ProgressionEventArray[7] = entity.GM;
			ProgressionCalculator.ProgressionEventArray[8] = entity.User.Id;
			ProgressionCalculator.ProgressionEventArray[9] = entity.User.UserName;
			ProgressionCalculator.ProgressionEventArray[10] = entity.CollectionController.Record.Id;
			ProgressionCalculator.ProgressionEventArray[11] = entity.CollectionController.Record.Name;
			ProgressionCalculator.ProgressionEventArray[12] = entity.ServerPlayerController.GetTotalTimePlayed().TotalDays;
			ProgressionCalculator.ProgressionEventArray[13] = entity.ServerPlayerController.GetSessionTimePlayed().TotalDays;
			SolDebug.LogToIndex(LogLevel.Information, LogIndex.Progression, "{@Zone} || {@Level} || {@RoleType} || {@RoleId} || {@RoleName} || {@SpecId} || {@SpecName} || {@GM} || {@UserId} || {@UserName} || {@CharacterId} || {@CharacterName} || {@TotalTimePlayed} || {@SessionTimePlayed}", ProgressionCalculator.ProgressionEventArray);
		}

		// Token: 0x04001CC5 RID: 7365
		private static readonly Dictionary<int, float> DiminishingXpFraction = new Dictionary<int, float>
		{
			{
				3,
				0.9f
			},
			{
				4,
				0.8f
			},
			{
				5,
				0.7f
			},
			{
				6,
				0.6f
			},
			{
				7,
				0.5f
			},
			{
				8,
				0.45f
			},
			{
				9,
				0.4f
			},
			{
				10,
				0.35f
			}
		};

		// Token: 0x04001CC6 RID: 7366
		private const string kProgressionTemplate = "{@Zone} || {@Level} || {@RoleType} || {@RoleId} || {@RoleName} || {@SpecId} || {@SpecName} || {@GM} || {@UserId} || {@UserName} || {@CharacterId} || {@CharacterName} || {@TotalTimePlayed} || {@SessionTimePlayed}";

		// Token: 0x04001CC7 RID: 7367
		private static readonly object[] ProgressionEventArray = new object[14];
	}
}
