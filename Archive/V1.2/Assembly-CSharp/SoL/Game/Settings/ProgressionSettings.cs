using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using SoL.Game.Messages;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Pooling;
using SoL.Game.Spawning;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x0200073A RID: 1850
	[Serializable]
	public class ProgressionSettings
	{
		// Token: 0x17000C76 RID: 3190
		// (get) Token: 0x06003755 RID: 14165 RVA: 0x00065DAA File Offset: 0x00063FAA
		public float SpecializationExperienceMultiplier
		{
			get
			{
				return this.m_specializationExperienceMultiplier;
			}
		}

		// Token: 0x17000C77 RID: 3191
		// (get) Token: 0x06003756 RID: 14166 RVA: 0x00065DB2 File Offset: 0x00063FB2
		public float DiminishingExperienceMultiplier
		{
			get
			{
				return this.m_diminishingExperienceMultiplier;
			}
		}

		// Token: 0x06003757 RID: 14167 RVA: 0x00065DBA File Offset: 0x00063FBA
		public float GetChallengeMod(ChallengeRating rating)
		{
			return this.m_challengeMods.GetMod(rating);
		}

		// Token: 0x06003758 RID: 14168 RVA: 0x00065DC8 File Offset: 0x00063FC8
		public float GetGroupSplitMod(int groupSize)
		{
			return this.m_groupSplitMods.GetMod(groupSize);
		}

		// Token: 0x06003759 RID: 14169 RVA: 0x0016B6AC File Offset: 0x001698AC
		public string GetExperienceGainedChatMessage(MasteryType masteryType, bool hasBonusXp)
		{
			if (string.IsNullOrEmpty(this.m_formattedAdventuringXpMsg) || string.IsNullOrEmpty(this.m_formattedTradeXpMsg))
			{
				Color color;
				MessageType.Skills.GetColor(out color, false);
				string text = string.Format("You gain {0} experience", "adventuring");
				this.m_formattedAdventuringXpMsg = string.Concat(new string[]
				{
					"<i><color=",
					color.ToHex(),
					">",
					text,
					"</color></i>"
				});
				this.m_formattedAdventuringXpWithBonusMsg = string.Concat(new string[]
				{
					"<i><color=",
					color.ToHex(),
					">",
					text,
					" with a bonus</color></i>"
				});
				text = string.Format("You gain {0} experience", "trade");
				this.m_formattedTradeXpMsg = string.Concat(new string[]
				{
					"<i><color=",
					color.ToHex(),
					">",
					text,
					"</color></i>"
				});
				this.m_formattedTradeXpWithBonusMsg = string.Concat(new string[]
				{
					"<i><color=",
					color.ToHex(),
					">",
					text,
					" with a bonus</color></i>"
				});
			}
			if (hasBonusXp)
			{
				if (masteryType.GetMasterySphere() != MasterySphere.Adventuring)
				{
					return this.m_formattedTradeXpWithBonusMsg;
				}
				return this.m_formattedAdventuringXpWithBonusMsg;
			}
			else
			{
				if (masteryType.GetMasterySphere() != MasterySphere.Adventuring)
				{
					return this.m_formattedTradeXpMsg;
				}
				return this.m_formattedAdventuringXpMsg;
			}
		}

		// Token: 0x0600375A RID: 14170 RVA: 0x0016B808 File Offset: 0x00169A08
		public void InitLevelUpVfxForEntity(GameEntity entity, MessageEventType messageEventType)
		{
			PooledVFX pooledVFX = null;
			if (messageEventType != MessageEventType.LevelUpAdventuring)
			{
				if (messageEventType == MessageEventType.LevelUpGatheringCrafting)
				{
					pooledVFX = this.m_tradeLevelUpVfx;
				}
			}
			else
			{
				pooledVFX = this.m_levelUpVfx;
			}
			if (pooledVFX)
			{
				pooledVFX.GetPooledInstance<PooledVFX>().Initialize(entity, 5f, null);
			}
		}

		// Token: 0x17000C78 RID: 3192
		// (get) Token: 0x0600375B RID: 14171 RVA: 0x00065DD6 File Offset: 0x00063FD6
		public EmberStone StartingEmberStone
		{
			get
			{
				return this.m_startingEmberStone;
			}
		}

		// Token: 0x0600375C RID: 14172 RVA: 0x00065DDE File Offset: 0x00063FDE
		public EmberStone GetNextEmberStone(EmberStone currentEmberStone)
		{
			if (!(currentEmberStone == null))
			{
				return currentEmberStone.NextEmberStone;
			}
			return this.m_startingEmberStone;
		}

		// Token: 0x0600375D RID: 14173 RVA: 0x00065DF6 File Offset: 0x00063FF6
		public float GetAdventuringTaskExperience(int taskLevel, int playerLevel)
		{
			return this.GetTaskExperience(this.AdventuringLevelCurve, taskLevel, playerLevel);
		}

		// Token: 0x0600375E RID: 14174 RVA: 0x00065E06 File Offset: 0x00064006
		public float GetGatheringTaskExperience(int taskLevel, int playerLevel)
		{
			return this.GetTaskExperience(this.GatheringLevelCurve, taskLevel, playerLevel);
		}

		// Token: 0x0600375F RID: 14175 RVA: 0x00065E16 File Offset: 0x00064016
		public float GetCraftingTaskExperience(int taskLevel, int playerLevel)
		{
			return this.GetTaskExperience(this.CraftingLevelCurve, taskLevel, playerLevel);
		}

		// Token: 0x06003760 RID: 14176 RVA: 0x0016B84C File Offset: 0x00169A4C
		private float GetTaskExperience(SuccessRewardCurve curve, int taskLevel, int playerLevel)
		{
			if (curve == null)
			{
				return 0f;
			}
			float totalForLevel = curve.GetTotalForLevel(taskLevel + 1);
			float num = this.m_taskXpFraction.Evaluate((float)playerLevel) + 0.001f;
			float num2 = 1f;
			if (playerLevel > taskLevel)
			{
				int num3 = playerLevel - taskLevel;
				float num4;
				if (num3 >= 6)
				{
					num2 = 0f;
				}
				else if (ProgressionSettings.DiminishingXpFraction.TryGetValue(num3, out num4))
				{
					num2 = num4;
				}
			}
			return totalForLevel * num * num2;
		}

		// Token: 0x06003761 RID: 14177 RVA: 0x00065E26 File Offset: 0x00064026
		public void AdventuringToJson()
		{
			this.CurveToJson(this.AdventuringLevelCurve);
		}

		// Token: 0x06003762 RID: 14178 RVA: 0x00065E34 File Offset: 0x00064034
		public void GatheringToJson()
		{
			this.CurveToJson(this.GatheringLevelCurve);
		}

		// Token: 0x06003763 RID: 14179 RVA: 0x00065E42 File Offset: 0x00064042
		public void CraftingToJson()
		{
			this.CurveToJson(this.CraftingLevelCurve);
		}

		// Token: 0x06003764 RID: 14180 RVA: 0x0016B8B0 File Offset: 0x00169AB0
		private void CurveToJson(SuccessRewardCurve curve)
		{
			Debug.Log(curve.ToJson(null, null, null, default(BsonSerializationArgs)));
		}

		// Token: 0x04003618 RID: 13848
		public const float kGroupMemberExperienceBonus = 0.4f;

		// Token: 0x04003619 RID: 13849
		public AnimationCurve ExperienceFalloff;

		// Token: 0x0400361A RID: 13850
		public SuccessRewardCurve AdventuringLevelCurve;

		// Token: 0x0400361B RID: 13851
		public SuccessRewardCurve GatheringLevelCurve;

		// Token: 0x0400361C RID: 13852
		public SuccessRewardCurve CraftingLevelCurve;

		// Token: 0x0400361D RID: 13853
		[SerializeField]
		private ProgressionSettings.ChallengeMods m_challengeMods;

		// Token: 0x0400361E RID: 13854
		[SerializeField]
		private ProgressionSettings.GroupSplitMods m_groupSplitMods;

		// Token: 0x0400361F RID: 13855
		[SerializeField]
		private float m_specializationExperienceMultiplier = 1.5f;

		// Token: 0x04003620 RID: 13856
		[SerializeField]
		private float m_diminishingExperienceMultiplier = 0.125f;

		// Token: 0x04003621 RID: 13857
		[SerializeField]
		private EmberStone m_startingEmberStone;

		// Token: 0x04003622 RID: 13858
		[SerializeField]
		private PooledVFX m_levelUpVfx;

		// Token: 0x04003623 RID: 13859
		[SerializeField]
		private PooledVFX m_tradeLevelUpVfx;

		// Token: 0x04003624 RID: 13860
		private const string kExperienceGainChatMessage = "You gain {0} experience";

		// Token: 0x04003625 RID: 13861
		private const string kBonusMessage = "with a bonus";

		// Token: 0x04003626 RID: 13862
		[NonSerialized]
		private string m_formattedAdventuringXpMsg;

		// Token: 0x04003627 RID: 13863
		[NonSerialized]
		private string m_formattedAdventuringXpWithBonusMsg;

		// Token: 0x04003628 RID: 13864
		[NonSerialized]
		private string m_formattedTradeXpMsg;

		// Token: 0x04003629 RID: 13865
		[NonSerialized]
		private string m_formattedTradeXpWithBonusMsg;

		// Token: 0x0400362A RID: 13866
		private static readonly Dictionary<int, float> DiminishingXpFraction = new Dictionary<int, float>
		{
			{
				1,
				0.9f
			},
			{
				2,
				0.7f
			},
			{
				3,
				0.5f
			},
			{
				4,
				0.3f
			},
			{
				5,
				0.1f
			},
			{
				6,
				0f
			}
		};

		// Token: 0x0400362B RID: 13867
		[SerializeField]
		private AnimationCurve m_taskXpFraction = AnimationCurve.Linear(0f, 0.2f, 50f, 0.2f);

		// Token: 0x0400362C RID: 13868
		private const string kToJsonGroup = "To Json";

		// Token: 0x0200073B RID: 1851
		[Serializable]
		private class GroupSplitMods
		{
			// Token: 0x06003767 RID: 14183 RVA: 0x0016B934 File Offset: 0x00169B34
			public float GetMod(int groupSize)
			{
				switch (groupSize)
				{
				case 1:
					return this.m_one;
				case 2:
					return this.m_two;
				case 3:
					return this.m_three;
				case 4:
					return this.m_four;
				case 5:
					return this.m_five;
				case 6:
					return this.m_six;
				default:
					return 1f;
				}
			}

			// Token: 0x0400362D RID: 13869
			[SerializeField]
			private float m_one = 1f;

			// Token: 0x0400362E RID: 13870
			[SerializeField]
			private float m_two = 1.25f;

			// Token: 0x0400362F RID: 13871
			[SerializeField]
			private float m_three = 1.5f;

			// Token: 0x04003630 RID: 13872
			[SerializeField]
			private float m_four = 1.6f;

			// Token: 0x04003631 RID: 13873
			[SerializeField]
			private float m_five = 1.7f;

			// Token: 0x04003632 RID: 13874
			[SerializeField]
			private float m_six = 2f;
		}

		// Token: 0x0200073C RID: 1852
		[Serializable]
		private class ChallengeMods
		{
			// Token: 0x06003769 RID: 14185 RVA: 0x0016B9EC File Offset: 0x00169BEC
			public float GetMod(ChallengeRating rating)
			{
				switch (rating)
				{
				case ChallengeRating.CR0:
					return this.m_cr0;
				case ChallengeRating.CR1:
					return this.m_cr1;
				case ChallengeRating.CR2:
					return this.m_cr2;
				case ChallengeRating.CR3:
					return this.m_cr3;
				case ChallengeRating.CR4:
					return this.m_cr4;
				case ChallengeRating.CRB:
					return this.m_crB;
				}
				return 1f;
			}

			// Token: 0x04003633 RID: 13875
			[SerializeField]
			private float m_cr0;

			// Token: 0x04003634 RID: 13876
			[SerializeField]
			private float m_cr1 = 1f;

			// Token: 0x04003635 RID: 13877
			[SerializeField]
			private float m_cr2 = 2f;

			// Token: 0x04003636 RID: 13878
			[SerializeField]
			private float m_cr3 = 3f;

			// Token: 0x04003637 RID: 13879
			[SerializeField]
			private float m_cr4 = 4f;

			// Token: 0x04003638 RID: 13880
			[SerializeField]
			private float m_crB = 4f;
		}
	}
}
