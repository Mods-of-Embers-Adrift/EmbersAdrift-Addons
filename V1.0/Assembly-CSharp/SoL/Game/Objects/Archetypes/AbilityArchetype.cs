using System;
using System.Collections;
using System.Text;
using Cysharp.Text;
using SoL.Game.Animation;
using SoL.Game.Audio;
using SoL.Game.EffectSystem;
using SoL.Game.NPCs;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Settings;
using SoL.Game.Transactions;
using SoL.Managers;
using SoL.Networking;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A20 RID: 2592
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Abilities/Ability")]
	public class AbilityArchetype : BaseArchetype, IExecutable, IVfxSource, IMasterySource, IAbilityCooldown, IMerchantInventory
	{
		// Token: 0x1700118D RID: 4493
		// (get) Token: 0x06004FC1 RID: 20417 RVA: 0x00075817 File Offset: 0x00073A17
		private bool m_showVfx
		{
			get
			{
				return this.m_vfxOverride == null;
			}
		}

		// Token: 0x1700118E RID: 4494
		// (get) Token: 0x06004FC2 RID: 20418 RVA: 0x00075825 File Offset: 0x00073A25
		private IEnumerable GetVfxOverrides
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<AbilityVFXScriptable>();
			}
		}

		// Token: 0x1700118F RID: 4495
		// (get) Token: 0x06004FC3 RID: 20419 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool ShowAbilityAnimation
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17001190 RID: 4496
		// (get) Token: 0x06004FC4 RID: 20420 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool UseAutoAttackAnimation
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06004FC5 RID: 20421 RVA: 0x0007582C File Offset: 0x00073A2C
		protected virtual AbilityAnimation GetAbilityAnimation(GameEntity entity)
		{
			return this.m_animation;
		}

		// Token: 0x17001191 RID: 4497
		// (get) Token: 0x06004FC6 RID: 20422 RVA: 0x00075834 File Offset: 0x00073A34
		protected bool RequireCombatStance
		{
			get
			{
				return this.m_baseParams != null && this.m_baseParams.ExecutionParams != null && this.m_baseParams.ExecutionParams.ValidStances.HasBitFlag(StanceFlags.Combat);
			}
		}

		// Token: 0x17001192 RID: 4498
		// (get) Token: 0x06004FC7 RID: 20423 RVA: 0x00075863 File Offset: 0x00073A63
		protected bool IsOnlyCombatStance
		{
			get
			{
				return this.m_baseParams != null && this.m_baseParams.ExecutionParams != null && this.m_baseParams.ExecutionParams.ValidStances == StanceFlags.Combat;
			}
		}

		// Token: 0x17001193 RID: 4499
		// (get) Token: 0x06004FC8 RID: 20424 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool ShowTargetingParameters
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001194 RID: 4500
		// (get) Token: 0x06004FC9 RID: 20425 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool ShowKinematicParameters
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001195 RID: 4501
		// (get) Token: 0x06004FCA RID: 20426 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool ShowCombatParameters
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06004FCB RID: 20427 RVA: 0x001CB4CC File Offset: 0x001C96CC
		public string GetAlchemyDisplayName(AlchemyPowerLevel alchemyPowerLevel)
		{
			if (alchemyPowerLevel != AlchemyPowerLevel.I)
			{
				if (alchemyPowerLevel == AlchemyPowerLevel.II)
				{
					if (this.m_alchemyDataII != null && !string.IsNullOrEmpty(this.m_alchemyDataII.DisplayName))
					{
						return this.m_alchemyDataII.DisplayName;
					}
				}
			}
			else if (this.m_alchemyDataI != null && !string.IsNullOrEmpty(this.m_alchemyDataI.DisplayName))
			{
				return this.m_alchemyDataI.DisplayName;
			}
			return this.DisplayName;
		}

		// Token: 0x06004FCC RID: 20428 RVA: 0x001CB538 File Offset: 0x001C9738
		public string GetAlchemyDescription(AlchemyPowerLevel alchemyPowerLevel)
		{
			if (alchemyPowerLevel != AlchemyPowerLevel.I)
			{
				if (alchemyPowerLevel == AlchemyPowerLevel.II)
				{
					if (this.m_alchemyDataII != null && !string.IsNullOrEmpty(this.m_alchemyDataII.Description))
					{
						return this.m_alchemyDataII.Description;
					}
				}
			}
			else if (this.m_alchemyDataI != null && !string.IsNullOrEmpty(this.m_alchemyDataI.Description))
			{
				return this.m_alchemyDataI.Description;
			}
			return this.Description;
		}

		// Token: 0x17001196 RID: 4502
		// (get) Token: 0x06004FCD RID: 20429 RVA: 0x0007588F File Offset: 0x00073A8F
		public EmotiveCalls NpcEmotes
		{
			get
			{
				return this.m_npcEmotes;
			}
		}

		// Token: 0x17001197 RID: 4503
		// (get) Token: 0x06004FCE RID: 20430 RVA: 0x00075897 File Offset: 0x00073A97
		public AbilityArchetype.NpcUseCases NpcUseCase
		{
			get
			{
				return this.m_npcUseCase;
			}
		}

		// Token: 0x06004FCF RID: 20431 RVA: 0x001CB5A4 File Offset: 0x001C97A4
		protected TieredAbilityParameters GetTieredAbilityParameter(TieredAbilityParameterType type, float level)
		{
			for (int i = this.m_overrideParams.Length - 1; i >= 0; i--)
			{
				if ((float)this.m_overrideParams[i].LevelThreshold <= level && this.m_overrideParams[i].IsOverridden(type))
				{
					return this.m_overrideParams[i];
				}
			}
			return this.m_baseParams;
		}

		// Token: 0x06004FD0 RID: 20432 RVA: 0x001CB5F8 File Offset: 0x001C97F8
		protected TieredAbilityParameters GetTieredAbilityParameter(TieredAbilityParameterType type, float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			TieredAbilityParameters tieredAbilityParameters = null;
			if (Application.isPlaying && GameManager.AllowAlchemy)
			{
				if (alchemyPowerLevel != AlchemyPowerLevel.I)
				{
					if (alchemyPowerLevel == AlchemyPowerLevel.II)
					{
						if (this.m_alchemyDataII != null)
						{
							tieredAbilityParameters = this.GetOverriddenParams(this.m_alchemyDataII.Overrides, type, level);
						}
					}
				}
				else if (this.m_alchemyDataI != null)
				{
					tieredAbilityParameters = this.GetOverriddenParams(this.m_alchemyDataI.Overrides, type, level);
				}
			}
			if (tieredAbilityParameters == null)
			{
				tieredAbilityParameters = this.GetOverriddenParams(this.m_overrideParams, type, level);
			}
			if (tieredAbilityParameters == null)
			{
				return this.m_baseParams;
			}
			return tieredAbilityParameters;
		}

		// Token: 0x06004FD1 RID: 20433 RVA: 0x001CB678 File Offset: 0x001C9878
		private TieredAbilityParameters GetOverriddenParams(TieredAbilityParametersOverride[] paramArray, TieredAbilityParameterType type, float level)
		{
			if (paramArray != null && paramArray.Length != 0)
			{
				for (int i = paramArray.Length - 1; i >= 0; i--)
				{
					if ((float)paramArray[i].LevelThreshold <= level && paramArray[i].IsOverridden(type))
					{
						return paramArray[i];
					}
				}
			}
			return null;
		}

		// Token: 0x06004FD2 RID: 20434 RVA: 0x001CB6B8 File Offset: 0x001C98B8
		private bool HasValidAlchemyOverride(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			bool result = false;
			if (alchemyPowerLevel != AlchemyPowerLevel.None && Application.isPlaying && GameManager.AllowAlchemy)
			{
				if (alchemyPowerLevel != AlchemyPowerLevel.I)
				{
					if (alchemyPowerLevel == AlchemyPowerLevel.II)
					{
						result = (this.m_alchemyDataII != null && this.HasAnyOverrideParams(this.m_alchemyDataII.Overrides, level));
					}
				}
				else
				{
					result = (this.m_alchemyDataI != null && this.HasAnyOverrideParams(this.m_alchemyDataI.Overrides, level));
				}
			}
			return result;
		}

		// Token: 0x06004FD3 RID: 20435 RVA: 0x001CB724 File Offset: 0x001C9924
		private bool HasAnyOverrideParams(TieredAbilityParametersOverride[] paramArray, float level)
		{
			if (paramArray != null && paramArray.Length != 0)
			{
				for (int i = paramArray.Length - 1; i >= 0; i--)
				{
					if ((float)paramArray[i].LevelThreshold <= level && paramArray[i].HasAnyOverrides())
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06004FD4 RID: 20436 RVA: 0x0007589F File Offset: 0x00073A9F
		protected CombatEffect GetCombatEffect(float level, AlchemyPowerLevel alchemyPowerLevel = AlchemyPowerLevel.None)
		{
			return this.GetTieredAbilityParameter(TieredAbilityParameterType.Effect, level, alchemyPowerLevel).CombatEffect;
		}

		// Token: 0x06004FD5 RID: 20437 RVA: 0x000758AF File Offset: 0x00073AAF
		protected TargetingParams GetTargetingParams(float level, AlchemyPowerLevel alchemyPowerLevel = AlchemyPowerLevel.None)
		{
			return this.GetTieredAbilityParameter(TieredAbilityParameterType.Targeting, level, alchemyPowerLevel).TargetingParams;
		}

		// Token: 0x06004FD6 RID: 20438 RVA: 0x001CB764 File Offset: 0x001C9964
		public string GetTierDisplayLevel(ArchetypeInstance instance)
		{
			if (this.m_overrideParams != null && this.m_overrideParams.Length != 0 && instance != null && instance.IsAbility && LocalPlayer.GameEntity != null)
			{
				float num = instance.AbilityData.GetAssociatedLevel(this, LocalPlayer.GameEntity);
				if (UIManager.TooltipShowNext)
				{
					num = (float)this.GetNextTierLevel(Mathf.FloorToInt(num));
				}
				return this.GetTierDisplayLevel(num);
			}
			return string.Empty;
		}

		// Token: 0x06004FD7 RID: 20439 RVA: 0x001CB7D0 File Offset: 0x001C99D0
		public string GetTierDisplayLevel(float level)
		{
			for (int i = this.m_overrideParams.Length - 1; i >= 0; i--)
			{
				if ((float)this.m_overrideParams[i].LevelThreshold <= level)
				{
					switch (i + 2)
					{
					case 1:
						return "I";
					case 2:
						return "II";
					case 3:
						return "III";
					case 4:
						return "IV";
					case 5:
						return "V";
					case 6:
						return "VI";
					case 7:
						return "VII";
					case 8:
						return "VIII";
					case 9:
						return "IX";
					case 10:
						return "X";
					case 11:
						return "XI";
					case 12:
						return "XII";
					}
				}
			}
			if (this.m_overrideParams.Length < 1)
			{
				return string.Empty;
			}
			return "I";
		}

		// Token: 0x17001198 RID: 4504
		// (get) Token: 0x06004FD8 RID: 20440 RVA: 0x000758BF File Offset: 0x00073ABF
		public AbilityArchetype NextTier
		{
			get
			{
				return this.m_nextTier;
			}
		}

		// Token: 0x17001199 RID: 4505
		// (get) Token: 0x06004FD9 RID: 20441 RVA: 0x000758C7 File Offset: 0x00073AC7
		public AbilityArchetype PreviousTier
		{
			get
			{
				return this.m_previousTier;
			}
		}

		// Token: 0x1700119A RID: 4506
		// (get) Token: 0x06004FDA RID: 20442 RVA: 0x0004479C File Offset: 0x0004299C
		public virtual bool ShowProgress
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06004FDB RID: 20443 RVA: 0x001CB8A8 File Offset: 0x001C9AA8
		public virtual int GetStartingLevel()
		{
			return this.LevelRange.Min;
		}

		// Token: 0x1700119B RID: 4507
		// (get) Token: 0x06004FDC RID: 20444 RVA: 0x000758CF File Offset: 0x00073ACF
		public MasteryArchetype Mastery
		{
			get
			{
				return this.m_mastery;
			}
		}

		// Token: 0x1700119C RID: 4508
		// (get) Token: 0x06004FDD RID: 20445 RVA: 0x000758D7 File Offset: 0x00073AD7
		public SpecializedRole Specialization
		{
			get
			{
				return this.m_specialization;
			}
		}

		// Token: 0x1700119D RID: 4509
		// (get) Token: 0x06004FDE RID: 20446 RVA: 0x000758DF File Offset: 0x00073ADF
		public int MinimumLevel
		{
			get
			{
				return this.m_levelRange.Min;
			}
		}

		// Token: 0x1700119E RID: 4510
		// (get) Token: 0x06004FDF RID: 20447 RVA: 0x000758EC File Offset: 0x00073AEC
		public int Memorization
		{
			get
			{
				return this.m_memorizationTime;
			}
		}

		// Token: 0x06004FE0 RID: 20448 RVA: 0x000758F4 File Offset: 0x00073AF4
		public int GetCooldown(float level)
		{
			return this.GetTieredAbilityParameter(TieredAbilityParameterType.Execution, level).ExecutionParams.Cooldown;
		}

		// Token: 0x1700119F RID: 4511
		// (get) Token: 0x06004FE1 RID: 20449 RVA: 0x00075908 File Offset: 0x00073B08
		public MinMaxIntRange LevelRange
		{
			get
			{
				return this.m_levelRange;
			}
		}

		// Token: 0x170011A0 RID: 4512
		// (get) Token: 0x06004FE2 RID: 20450 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected virtual bool m_allowStaminaRegenDuringExecution
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170011A1 RID: 4513
		// (get) Token: 0x06004FE3 RID: 20451 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool m_triggerGlobalCooldown
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170011A2 RID: 4514
		// (get) Token: 0x06004FE4 RID: 20452 RVA: 0x0006109C File Offset: 0x0005F29C
		protected virtual float m_creditFactor
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x170011A3 RID: 4515
		// (get) Token: 0x06004FE5 RID: 20453 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool m_addGroupBonus
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06004FE6 RID: 20454 RVA: 0x0004479C File Offset: 0x0004299C
		private bool HasAvailableAbilityPoint(GameEntity entity)
		{
			return true;
		}

		// Token: 0x06004FE7 RID: 20455 RVA: 0x00075910 File Offset: 0x00073B10
		public bool CanTrain(GameEntity entity)
		{
			return this.CanLearn(entity) && this.HasAvailableAbilityPoint(entity);
		}

		// Token: 0x06004FE8 RID: 20456 RVA: 0x001CB8C4 File Offset: 0x001C9AC4
		public bool CanLearn(GameEntity entity)
		{
			bool flag = false;
			ArchetypeInstance archetypeInstance;
			if (entity.CollectionController.Masteries != null && entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(this.m_mastery.Id, out archetypeInstance))
			{
				flag = (archetypeInstance.GetAssociatedLevel(entity) >= (float)this.LevelRange.Min);
			}
			if (flag && this.m_previousTier)
			{
				ArchetypeInstance archetypeInstance2;
				return entity.CollectionController.Abilities != null && entity.CollectionController.Abilities.TryGetInstanceForArchetypeId(this.m_previousTier.Id, out archetypeInstance2) && archetypeInstance2.GetAssociatedLevel(entity) >= (float)this.m_previousTier.LevelRange.Max;
			}
			return flag;
		}

		// Token: 0x06004FE9 RID: 20457 RVA: 0x00045BCA File Offset: 0x00043DCA
		public bool CanUnTrain(GameEntity entity)
		{
			return false;
		}

		// Token: 0x06004FEA RID: 20458 RVA: 0x001CB97C File Offset: 0x001C9B7C
		public bool CanLearnWithDetails(GameEntity entity, out string info, bool showPreviousTierName = true)
		{
			info = string.Empty;
			StringBuilder fromPool = StringBuilderExtensions.GetFromPool();
			bool flag = false;
			ArchetypeInstance archetypeInstance;
			if (entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(this.m_mastery.Id, out archetypeInstance))
			{
				flag = (archetypeInstance.GetAssociatedLevel(entity) >= (float)this.LevelRange.Min);
				if (!flag)
				{
					fromPool.AppendLine("Mastery level too low!  Requires " + this.LevelRange.Min.ToString());
				}
			}
			else
			{
				fromPool.AppendLine("Requires " + this.m_mastery.DisplayName + " mastery");
			}
			if (this.m_previousTier)
			{
				ArchetypeInstance archetypeInstance2;
				if (!entity.CollectionController.Abilities.TryGetInstanceForArchetypeId(this.m_previousTier.Id, out archetypeInstance2))
				{
					string str = showPreviousTierName ? this.m_previousTier.DisplayName : "UNKOWN";
					fromPool.AppendLine("Must know " + str + " at level " + this.m_previousTier.LevelRange.Max.ToString());
					flag = false;
				}
				else if (archetypeInstance2.GetAssociatedLevel(entity) < (float)this.m_previousTier.LevelRange.Max)
				{
					fromPool.AppendLine(this.m_previousTier.DisplayName + " ability must be at level " + this.m_previousTier.LevelRange.Max.ToString());
					flag = false;
				}
			}
			if (fromPool.Length > 0)
			{
				info = fromPool.ToString_ReturnToPool().Color(flag ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor);
			}
			else
			{
				fromPool.ReturnToPool();
			}
			return flag;
		}

		// Token: 0x06004FEB RID: 20459 RVA: 0x001CBB28 File Offset: 0x001C9D28
		public bool CanTrainWithDetails(GameEntity entity, out string info)
		{
			string text;
			bool flag = this.CanLearnWithDetails(entity, out text, true);
			bool flag2 = this.HasAvailableAbilityPoint(entity);
			info = text;
			if (!flag2)
			{
				string text2 = "<size=110%>Insufficient Ability Points</size>";
				info = (string.IsNullOrEmpty(info) ? text2 : (info + "\n" + text2));
			}
			bool flag3 = flag && flag2;
			info = info.Color(flag3 ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor);
			return flag3;
		}

		// Token: 0x06004FEC RID: 20460 RVA: 0x001CBB8C File Offset: 0x001C9D8C
		private bool TimeElapsedExceedsCooldown(float cooldown, DateTime timestamp)
		{
			double totalSeconds = (DateTime.UtcNow - timestamp).TotalSeconds;
			double num = (double)cooldown - totalSeconds;
			if (!GameManager.IsServer)
			{
				return totalSeconds >= (double)cooldown;
			}
			return totalSeconds >= (double)cooldown || Mathf.Abs((float)num) < 0.1f;
		}

		// Token: 0x06004FED RID: 20461 RVA: 0x001CBBD8 File Offset: 0x001C9DD8
		private bool IsMemorized(ArchetypeInstance instance)
		{
			return instance.IsAbility && instance.AbilityData.MemorizationTimestamp != null && this.TimeElapsedExceedsCooldown((float)this.m_memorizationTime, instance.AbilityData.MemorizationTimestamp.Value);
		}

		// Token: 0x06004FEE RID: 20462 RVA: 0x001CBC24 File Offset: 0x001C9E24
		private static bool IsCooledDown(ArchetypeInstance instance)
		{
			return instance.IsAbility && instance.AbilityData.Cooldown_Base.Elapsed == null;
		}

		// Token: 0x06004FEF RID: 20463 RVA: 0x00075924 File Offset: 0x00073B24
		protected virtual bool InRequiredStance(GameEntity entity)
		{
			return entity && entity.Vitals && entity.Vitals.Stance.CanExecute(this.m_baseParams.ExecutionParams.ValidStances);
		}

		// Token: 0x06004FF0 RID: 20464 RVA: 0x001CBC58 File Offset: 0x001C9E58
		public bool HasEnoughStamina(GameEntity entity, float level)
		{
			if (!entity || !entity.Vitals)
			{
				throw new ArgumentException("No Entity or Vitals for stamina check!");
			}
			TieredAbilityParameters tieredAbilityParameter = this.GetTieredAbilityParameter(TieredAbilityParameterType.Execution, level);
			return this.HasEnoughStamina(entity, tieredAbilityParameter.ExecutionParams);
		}

		// Token: 0x06004FF1 RID: 20465 RVA: 0x0007595D File Offset: 0x00073B5D
		private bool HasEnoughStamina(ExecutionCache cache)
		{
			return this.HasEnoughStamina(cache.SourceEntity, cache.ExecutionParams);
		}

		// Token: 0x06004FF2 RID: 20466 RVA: 0x00075971 File Offset: 0x00073B71
		private bool HasEnoughStamina(GameEntity entity, ExecutionParams exeParams)
		{
			if (!entity || !entity.Vitals)
			{
				throw new ArgumentException("No Entity or Vitals for stamina check!");
			}
			return entity.Vitals.Stamina >= (float)exeParams.StaminaCost;
		}

		// Token: 0x06004FF3 RID: 20467 RVA: 0x000759AA File Offset: 0x00073BAA
		protected virtual bool MeetsRequirementsForUI(GameEntity entity, float level)
		{
			return this.InRequiredStance(entity);
		}

		// Token: 0x06004FF4 RID: 20468 RVA: 0x000759B3 File Offset: 0x00073BB3
		protected virtual void PostExecution(ExecutionCache executionCache)
		{
			ArchetypeInstance instance = executionCache.Instance;
			if (instance != null)
			{
				AbilityInstanceData abilityData = instance.AbilityData;
				if (abilityData != null)
				{
					abilityData.AbilityExecuted(executionCache.AlchemyPowerLevel);
				}
			}
			executionCache.PerformReduction();
		}

		// Token: 0x06004FF5 RID: 20469 RVA: 0x001CBC9C File Offset: 0x001C9E9C
		protected virtual bool ExecutionCheck(ExecutionCache executionCache, float executionProgress)
		{
			bool initial = executionProgress <= 0f;
			if (executionCache == null)
			{
				Debug.LogError("ExecutionCheck: executionCache is null! " + this.GetErrorLogItemDescription());
				return false;
			}
			if (executionCache.Instance == null)
			{
				executionCache.Message = "Error";
				Debug.LogError("ExecutionCheck: instance is null!");
				return false;
			}
			if (!executionCache.SourceEntity || !executionCache.SourceEntity.Vitals || !executionCache.SourceEntity.VitalsReplicator || !executionCache.SourceEntity.SkillsController)
			{
				executionCache.Message = "Error";
				Debug.LogError("ExecutionCheck: nulls abound! " + this.GetErrorLogItemDescription());
				return false;
			}
			if (executionCache.Instance.AbilityData == null)
			{
				executionCache.Message = "Error";
				Debug.LogError("ExecutionCheck: instance.AbilityData null! " + this.GetErrorLogItemDescription());
				return false;
			}
			if (executionCache.SourceEntity.VitalsReplicator.CurrentHealthState.Value != HealthState.Alive)
			{
				executionCache.Message = "Not awake!";
				return false;
			}
			if (this.MustBeMemorizedToExecute && (executionCache.Instance.Index == -1 || executionCache.Instance.AbilityData.MemorizationTimestamp == null))
			{
				executionCache.Message = "Not memorized!";
				return false;
			}
			ArchetypeInstance archetypeInstance;
			if (!executionCache.SourceEntity.SkillsController.TryGetCachedMasteryInstance(executionCache.Instance, out archetypeInstance))
			{
				executionCache.Message = "Invalid Mastery!";
				return false;
			}
			executionCache.MasteryInstance = archetypeInstance;
			if (executionCache.MasteryInstance == null)
			{
				executionCache.Message = "Error";
				Debug.LogError("ExecutionCheck: masteryInstance nulls abound! " + this.GetErrorLogItemDescription());
				return false;
			}
			if (executionCache.MasteryInstance.MasteryData == null)
			{
				executionCache.Message = "Error";
				Debug.LogError("ExecutionCheck: masteryInstance.MasteryData nulls abound! " + this.GetErrorLogItemDescription());
				return false;
			}
			if (this.m_specialization != null)
			{
				if (archetypeInstance.MasteryData.Specialization == null || archetypeInstance.MasteryData.Specialization.Value != this.m_specialization.Id)
				{
					executionCache.Message = "Invalid Specialization!";
					return false;
				}
				if (executionCache.Instance.GetAssociatedLevel(executionCache.SourceEntity) < (float)this.MinimumLevel)
				{
					executionCache.Message = "Specialization level requirements not met!";
					return false;
				}
			}
			else if (executionCache.MasteryInstance.GetAssociatedLevel(executionCache.SourceEntity) < (float)this.MinimumLevel)
			{
				executionCache.Message = "Mastery level requirements not met!";
				return false;
			}
			if (this.MustBeMemorizedToExecute && !this.IsMemorized(executionCache.Instance))
			{
				executionCache.Message = "Not finished memorizing!";
				return false;
			}
			if (!AbilityArchetype.IsCooledDown(executionCache.Instance))
			{
				executionCache.Message = "Cooldown not met!";
				return false;
			}
			string message;
			if (executionCache.AlchemyPowerLevel != AlchemyPowerLevel.None && !AlchemyExtensions.AlchemyPowerLevelAvailable(executionCache.SourceEntity, executionCache.Instance, executionCache.AlchemyPowerLevel, out message))
			{
				executionCache.Message = message;
				return false;
			}
			if (!executionCache.ExecutionParams.ExecutionCheck(executionCache, initial))
			{
				return false;
			}
			if (executionCache.SourceEntity.VitalsReplicator.BehaviorFlags.Value.CancelExecutionForFlag())
			{
				executionCache.Message = executionCache.SourceEntity.VitalsReplicator.BehaviorFlags.Value.CancelExecutionForFlagDescription();
				return false;
			}
			return true;
		}

		// Token: 0x06004FF6 RID: 20470 RVA: 0x001CBFCC File Offset: 0x001CA1CC
		private string GetErrorLogItemDescription()
		{
			return string.Concat(new string[]
			{
				this.DisplayName,
				" (",
				base.Id.ToString(),
				", ",
				base.name,
				")"
			});
		}

		// Token: 0x170011A4 RID: 4516
		// (get) Token: 0x06004FF7 RID: 20471 RVA: 0x0004479C File Offset: 0x0004299C
		public virtual bool ConsiderHaste
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170011A5 RID: 4517
		// (get) Token: 0x06004FF8 RID: 20472 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool ClampHasteTo100
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170011A6 RID: 4518
		// (get) Token: 0x06004FF9 RID: 20473 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool MustBeMemorizedToExecute
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170011A7 RID: 4519
		// (get) Token: 0x06004FFA RID: 20474 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IAbilityCooldown.PauseWhileHandSwapActive
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170011A8 RID: 4520
		// (get) Token: 0x06004FFB RID: 20475 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IAbilityCooldown.PauseWhileExecuting
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170011A9 RID: 4521
		// (get) Token: 0x06004FFC RID: 20476 RVA: 0x000759DD File Offset: 0x00073BDD
		bool IAbilityCooldown.ConsiderHaste
		{
			get
			{
				return this.ConsiderHaste;
			}
		}

		// Token: 0x170011AA RID: 4522
		// (get) Token: 0x06004FFD RID: 20477 RVA: 0x000759E5 File Offset: 0x00073BE5
		bool IAbilityCooldown.ClampHasteTo100
		{
			get
			{
				return this.ClampHasteTo100;
			}
		}

		// Token: 0x06004FFE RID: 20478 RVA: 0x000759ED File Offset: 0x00073BED
		float IAbilityCooldown.GetCooldown(GameEntity entity, ExecutionCache executionCache, float level)
		{
			return (float)this.GetCooldown(level);
		}

		// Token: 0x170011AB RID: 4523
		// (get) Token: 0x06004FFF RID: 20479 RVA: 0x000759F7 File Offset: 0x00073BF7
		public virtual bool AllowAlchemy
		{
			get
			{
				return GameManager.AllowAlchemy;
			}
		}

		// Token: 0x170011AC RID: 4524
		// (get) Token: 0x06005000 RID: 20480 RVA: 0x0004479C File Offset: 0x0004299C
		EffectSourceType IExecutable.Type
		{
			get
			{
				return EffectSourceType.Ability;
			}
		}

		// Token: 0x170011AD RID: 4525
		// (get) Token: 0x06005001 RID: 20481 RVA: 0x000759FE File Offset: 0x00073BFE
		string IExecutable.DisplayName
		{
			get
			{
				return this.DisplayName;
			}
		}

		// Token: 0x170011AE RID: 4526
		// (get) Token: 0x06005002 RID: 20482 RVA: 0x00075A06 File Offset: 0x00073C06
		bool IExecutable.TriggerGlobalCooldown
		{
			get
			{
				return this.m_triggerGlobalCooldown;
			}
		}

		// Token: 0x170011AF RID: 4527
		// (get) Token: 0x06005003 RID: 20483 RVA: 0x00075A0E File Offset: 0x00073C0E
		bool IExecutable.AllowStaminaRegenDuringExecution
		{
			get
			{
				return this.m_allowStaminaRegenDuringExecution;
			}
		}

		// Token: 0x170011B0 RID: 4528
		// (get) Token: 0x06005004 RID: 20484 RVA: 0x00075A16 File Offset: 0x00073C16
		bool IExecutable.AllowAlchemy
		{
			get
			{
				return this.AllowAlchemy;
			}
		}

		// Token: 0x06005005 RID: 20485 RVA: 0x00075A1E File Offset: 0x00073C1E
		bool IExecutable.UseAutoAttackAnimation()
		{
			return this.UseAutoAttackAnimation;
		}

		// Token: 0x06005006 RID: 20486 RVA: 0x00075A26 File Offset: 0x00073C26
		bool IExecutable.TryGetAbilityAnimation(GameEntity entity, out AbilityAnimation animation)
		{
			animation = this.GetAbilityAnimation(entity);
			return animation != null;
		}

		// Token: 0x06005007 RID: 20487 RVA: 0x00075A36 File Offset: 0x00073C36
		void IExecutable.PostExecution(ExecutionCache executionCache)
		{
			this.PostExecution(executionCache);
		}

		// Token: 0x06005008 RID: 20488 RVA: 0x00075A3F File Offset: 0x00073C3F
		bool IExecutable.PreExecution(ExecutionCache executionCache)
		{
			return this.ExecutionCheck(executionCache, 0f);
		}

		// Token: 0x06005009 RID: 20489 RVA: 0x00075A4D File Offset: 0x00073C4D
		bool IExecutable.ContinuedExecution(ExecutionCache executionCache, float executionProgress)
		{
			return this.ExecutionCheck(executionCache, executionProgress);
		}

		// Token: 0x0600500A RID: 20490 RVA: 0x00075A57 File Offset: 0x00073C57
		bool IExecutable.MeetsRequirementsForUI(GameEntity entity, float level)
		{
			return this.MeetsRequirementsForUI(entity, level);
		}

		// Token: 0x0600500B RID: 20491 RVA: 0x00075A61 File Offset: 0x00073C61
		ExecutionParams IExecutable.GetExecutionParams(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return this.GetTieredAbilityParameter(TieredAbilityParameterType.Execution, level, alchemyPowerLevel).ExecutionParams;
		}

		// Token: 0x0600500C RID: 20492 RVA: 0x00075A71 File Offset: 0x00073C71
		bool IExecutable.HasValidAlchemyOverride(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return this.HasValidAlchemyOverride(level, alchemyPowerLevel);
		}

		// Token: 0x170011B1 RID: 4529
		// (get) Token: 0x0600500D RID: 20493 RVA: 0x001CC028 File Offset: 0x001CA228
		float? IExecutable.DeferHandIkDuration
		{
			get
			{
				if (!this.m_deferHandIk)
				{
					return null;
				}
				return new float?(this.m_deferHandIkDuration);
			}
		}

		// Token: 0x170011B2 RID: 4530
		// (get) Token: 0x0600500E RID: 20494 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IExecutable.IsLearning
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170011B3 RID: 4531
		// (get) Token: 0x0600500F RID: 20495 RVA: 0x00075A7B File Offset: 0x00073C7B
		StanceFlags IExecutable.ValidStances
		{
			get
			{
				if (this.m_baseParams == null || this.m_baseParams.ExecutionParams == null)
				{
					return StanceFlags.None;
				}
				return this.m_baseParams.ExecutionParams.ValidStances;
			}
		}

		// Token: 0x170011B4 RID: 4532
		// (get) Token: 0x06005010 RID: 20496 RVA: 0x00075AA4 File Offset: 0x00073CA4
		AutoAttackStateChange IExecutable.AutoAttackState
		{
			get
			{
				if (this.m_baseParams == null || this.m_baseParams.ExecutionParams == null)
				{
					return AutoAttackStateChange.DoNothing;
				}
				return this.m_baseParams.ExecutionParams.AutoAttackState;
			}
		}

		// Token: 0x170011B5 RID: 4533
		// (get) Token: 0x06005011 RID: 20497 RVA: 0x00075ACD File Offset: 0x00073CCD
		bool IExecutable.PreventRotation
		{
			get
			{
				return this.m_baseParams != null && this.m_baseParams.ExecutionParams != null && this.m_baseParams.ExecutionParams.PreventRotation;
			}
		}

		// Token: 0x170011B6 RID: 4534
		// (get) Token: 0x06005012 RID: 20498 RVA: 0x00075AF6 File Offset: 0x00073CF6
		float IExecutable.ExecutionTime
		{
			get
			{
				if (this.m_baseParams == null || this.m_baseParams.ExecutionParams == null)
				{
					return 10f;
				}
				return this.m_baseParams.ExecutionParams.ExecutionTime;
			}
		}

		// Token: 0x06005013 RID: 20499 RVA: 0x001CC054 File Offset: 0x001CA254
		bool IVfxSource.TryGetEffects(int abilityLevel, AlchemyPowerLevel alchemyPowerLevel, bool isSecondary, out AbilityVFX effects)
		{
			effects = null;
			if (alchemyPowerLevel != AlchemyPowerLevel.I)
			{
				if (alchemyPowerLevel == AlchemyPowerLevel.II)
				{
					if (this.m_alchemyDataII != null)
					{
						TieredAbilityParameterType type = isSecondary ? TieredAbilityParameterType.VfxSecondary : TieredAbilityParameterType.Vfx;
						TieredAbilityParameters overriddenParams = this.GetOverriddenParams(this.m_alchemyDataII.Overrides, type, (float)abilityLevel);
						if (overriddenParams != null)
						{
							effects = (isSecondary ? overriddenParams.VfxSecondary : overriddenParams.Vfx);
						}
						if (effects == null && this.m_alchemyDataII.OverrideVfx)
						{
							effects = this.m_alchemyDataII.VFX;
						}
					}
				}
			}
			else if (this.m_alchemyDataI != null)
			{
				TieredAbilityParameterType type2 = isSecondary ? TieredAbilityParameterType.VfxSecondary : TieredAbilityParameterType.Vfx;
				TieredAbilityParameters overriddenParams2 = this.GetOverriddenParams(this.m_alchemyDataI.Overrides, type2, (float)abilityLevel);
				if (overriddenParams2 != null)
				{
					effects = (isSecondary ? overriddenParams2.VfxSecondary : overriddenParams2.Vfx);
				}
				if (effects == null && this.m_alchemyDataI.OverrideVfx)
				{
					effects = this.m_alchemyDataI.VFX;
				}
			}
			if (effects == null)
			{
				effects = (this.m_vfxOverride ? this.m_vfxOverride.VFX : this.m_vfx);
			}
			return effects != null;
		}

		// Token: 0x06005014 RID: 20500 RVA: 0x00075B23 File Offset: 0x00073D23
		bool IMasterySource.TryGetMastery(GameEntity entity, out MasteryArchetype mastery)
		{
			mastery = this.Mastery;
			return mastery != null;
		}

		// Token: 0x170011B7 RID: 4535
		// (get) Token: 0x06005015 RID: 20501 RVA: 0x00075B35 File Offset: 0x00073D35
		float IMasterySource.CreditFactor
		{
			get
			{
				return this.m_creditFactor;
			}
		}

		// Token: 0x170011B8 RID: 4536
		// (get) Token: 0x06005016 RID: 20502 RVA: 0x00075B3D File Offset: 0x00073D3D
		bool IMasterySource.AddGroupBonus
		{
			get
			{
				return this.m_addGroupBonus;
			}
		}

		// Token: 0x06005017 RID: 20503 RVA: 0x00075607 File Offset: 0x00073807
		public IEnumerable GetAbilities()
		{
			return SolOdinUtilities.GetDropdownItems<AbilityArchetype>();
		}

		// Token: 0x06005018 RID: 20504 RVA: 0x00053971 File Offset: 0x00051B71
		private IEnumerable GetAudioClipCollections()
		{
			return SolOdinUtilities.GetDropdownItems<AudioClipCollection>();
		}

		// Token: 0x170011B9 RID: 4537
		// (get) Token: 0x06005019 RID: 20505 RVA: 0x00053500 File Offset: 0x00051700
		public override ArchetypeCategory Category
		{
			get
			{
				return ArchetypeCategory.Ability;
			}
		}

		// Token: 0x170011BA RID: 4538
		// (get) Token: 0x0600501A RID: 20506 RVA: 0x00075B45 File Offset: 0x00073D45
		public override AudioClipCollection DragDropAudio
		{
			get
			{
				return this.m_dragDropAudioOverride;
			}
		}

		// Token: 0x0600501B RID: 20507 RVA: 0x00075B4D File Offset: 0x00073D4D
		public override void OnInstanceCreated(ArchetypeInstance instance)
		{
			base.OnInstanceCreated(instance);
			instance.Index = -1;
			instance.AbilityData = new AbilityInstanceData(false);
		}

		// Token: 0x0600501C RID: 20508 RVA: 0x001CC160 File Offset: 0x001CA360
		private int GetCurrentTierLevel(int currentLevel)
		{
			for (int i = this.m_overrideParams.Length - 1; i >= 0; i--)
			{
				if (currentLevel >= this.m_overrideParams[i].LevelThreshold)
				{
					return this.m_overrideParams[i].LevelThreshold;
				}
			}
			return this.LevelRange.Min;
		}

		// Token: 0x0600501D RID: 20509 RVA: 0x001CC1B0 File Offset: 0x001CA3B0
		private int GetNextTierLevel(int currentLevel)
		{
			int num = 1;
			for (int i = 0; i < this.m_overrideParams.Length; i++)
			{
				if (currentLevel >= num && currentLevel < this.m_overrideParams[i].LevelThreshold)
				{
					return this.m_overrideParams[i].LevelThreshold;
				}
				num = this.m_overrideParams[i].LevelThreshold;
			}
			return this.GetMaxTierLevel();
		}

		// Token: 0x0600501E RID: 20510 RVA: 0x001CC208 File Offset: 0x001CA408
		private int GetMaxTierLevel()
		{
			if (this.m_overrideParams == null || this.m_overrideParams.Length == 0)
			{
				return this.LevelRange.Min;
			}
			return this.m_overrideParams[this.m_overrideParams.Length - 1].LevelThreshold;
		}

		// Token: 0x0600501F RID: 20511 RVA: 0x00075B69 File Offset: 0x00073D69
		private static void ResetTooltipTieredParameters()
		{
			AbilityArchetype.tt_alchemyPowerLevel = AlchemyPowerLevel.None;
			AbilityArchetype.tt_ExecutionParams = null;
			AbilityArchetype.tt_targetingParams = null;
			AbilityArchetype.tt_kinematicParams = null;
			AbilityArchetype.tt_combatEffect = null;
			AbilityArchetype.tt_reagentItem = null;
		}

		// Token: 0x06005020 RID: 20512 RVA: 0x00075B8F File Offset: 0x00073D8F
		protected virtual void FillTooltipTieredParams(GameEntity entity, float level)
		{
			AbilityArchetype.tt_ExecutionParams = this.GetTieredAbilityParameter(TieredAbilityParameterType.Execution, level, AbilityArchetype.tt_alchemyPowerLevel).ExecutionParams;
		}

		// Token: 0x06005021 RID: 20513 RVA: 0x00075BA8 File Offset: 0x00073DA8
		public sealed override void FillAbilityTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity, AlchemyPowerLevel alchemyPowerLevel)
		{
			this.FillTooltipBlocksInternal(tooltip, instance, entity, alchemyPowerLevel);
		}

		// Token: 0x06005022 RID: 20514 RVA: 0x00075BB5 File Offset: 0x00073DB5
		public sealed override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			this.FillTooltipBlocksInternal(tooltip, instance, entity, AlchemyPowerLevel.None);
		}

		// Token: 0x06005023 RID: 20515 RVA: 0x001CC24C File Offset: 0x001CA44C
		private void FillTooltipBlocksInternal(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity, AlchemyPowerLevel alchemyPowerLevel)
		{
			if (!entity)
			{
				return;
			}
			base.FillTooltipBlocks(tooltip, instance, entity);
			int masteryLevel = 1;
			ArchetypeInstance archetypeInstance;
			if (this.m_mastery && entity && entity.CollectionController != null && entity.CollectionController.Masteries != null && entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(this.m_mastery.Id, out archetypeInstance))
			{
				masteryLevel = archetypeInstance.GetAssociatedLevelInteger(entity);
			}
			int num = (instance != null) ? instance.GetAssociatedLevelInteger(entity) : 1;
			if (UIManager.TooltipShowNext)
			{
				num = this.GetNextTierLevel(num);
			}
			AbilityArchetype.ResetTooltipTieredParameters();
			AbilityArchetype.tt_alchemyPowerLevel = alchemyPowerLevel;
			int num2 = (AbilityArchetype.tt_alchemyPowerLevel != AlchemyPowerLevel.None) ? Mathf.Clamp(num, this.m_levelRange.Min, 50) : num;
			this.FillTooltipTieredParams(entity, (float)num2);
			this.FillTooltipBlocksInternal(tooltip, instance, entity, masteryLevel, num);
		}

		// Token: 0x06005024 RID: 20516 RVA: 0x001CC31C File Offset: 0x001CA51C
		protected virtual void FillTooltipBlocksInternal(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity, int masteryLevel, int abilityLevel)
		{
			TooltipTextBlock dataBlock = tooltip.DataBlock;
			bool flag = true;
			string arg = string.Empty;
			string text = string.Empty;
			if (instance != null)
			{
				if (AbilityArchetype.tt_ExecutionParams.Cooldown > 0)
				{
					float value = (float)AbilityArchetype.tt_ExecutionParams.Cooldown;
					if (instance.AbilityData.Cooldown_Base.Elapsed != null)
					{
						float num = (float)AbilityArchetype.tt_ExecutionParams.Cooldown;
						float value2 = instance.AbilityData.Cooldown_Base.Elapsed.Value;
						value = num - value2;
						if (instance.Ability != null && instance.Ability.ConsiderHaste && LocalPlayer.GameEntity && LocalPlayer.GameEntity.Vitals)
						{
							float num2 = (float)LocalPlayer.GameEntity.Vitals.GetHaste() * 0.01f;
							if (num2 > 0f || num2 < 0f)
							{
								float percentComplete = value2 / num;
								value = StatTypeExtensions.GetHasteAdjustedTimeRemaining(num, num2, percentComplete, instance.Ability.ClampHasteTo100);
							}
						}
					}
					text = value.GetFormattedTime(true);
				}
				else
				{
					text = "No";
				}
				float value3;
				if (ArchetypeTooltip.IsAlchemyOnlyTooltip && AbilityArchetype.tt_alchemyPowerLevel != AlchemyPowerLevel.None && GlobalSettings.Values.Ashen.TryGetAlchemyCooldownTime(AbilityArchetype.tt_alchemyPowerLevel, out value3))
				{
					text = value3.GetFormattedTime(true).Color(UIManager.EmberColor);
				}
				if (instance.AbilityData != null && UIManager.TooltipShowMore)
				{
					int usageCount = instance.AbilityData.GetUsageCount(AlchemyPowerLevel.None);
					if (usageCount > 0)
					{
						tooltip.AddLineToLeftSubHeader(ZString.Format<int>("{0} Executions", usageCount));
					}
					int usageCount2 = instance.AbilityData.GetUsageCount(AlchemyPowerLevel.I);
					if (usageCount2 > 0)
					{
						string arg2 = GlobalSettings.Values.Ashen.GetUIHighlightColor(AlchemyPowerLevel.I).ToHex();
						tooltip.AddLineToLeftSubHeader(ZString.Format<int, string>("{0} <color={1}>Alchemy I</color> Executions", usageCount2, arg2));
					}
					int usageCount3 = instance.AbilityData.GetUsageCount(AlchemyPowerLevel.II);
					if (usageCount3 > 0)
					{
						string arg3 = GlobalSettings.Values.Ashen.GetUIHighlightColor(AlchemyPowerLevel.II).ToHex();
						tooltip.AddLineToLeftSubHeader(ZString.Format<int, string>("{0} <color={1}>Alchemy II</color> Executions", usageCount3, arg3));
					}
				}
				int memorizationTime = this.m_memorizationTime;
				double num3 = (instance.AbilityData.MemorizationTimestamp != null) ? (GameTimeReplicator.GetServerCorrectedDateTimeUtc() - instance.AbilityData.MemorizationTimestamp.Value).TotalSeconds : 0.0;
				double num4 = (double)memorizationTime - num3;
				arg = ((num4 < 0.0) ? ((double)memorizationTime) : num4).GetFormattedTime(true);
				flag = (instance.AbilityData.CooldownFlags.HasBitFlag(AbilityCooldownFlags.Memorization) || instance.Index == -1);
			}
			else
			{
				arg = this.m_memorizationTime.GetFormattedTime(false);
				text = ((AbilityArchetype.tt_ExecutionParams.Cooldown > 0) ? AbilityArchetype.tt_ExecutionParams.Cooldown.GetFormattedTime(false) : "No");
			}
			if (instance != null && instance.IsAbility)
			{
				instance.AbilityData.Cooldown_AlchemyI.AddAlchemyCooldownToTooltip(dataBlock, AlchemyPowerLevel.I);
				instance.AbilityData.Cooldown_AlchemyII.AddAlchemyCooldownToTooltip(dataBlock, AlchemyPowerLevel.II);
			}
			string text2;
			if (AbilityArchetype.tt_alchemyPowerLevel == AlchemyPowerLevel.None)
			{
				text2 = (AbilityArchetype.tt_ExecutionParams.IsInstant ? "<b>Instant</b> Execution" : ZString.Format<string>("<b>{0}</b> Execution", AbilityArchetype.tt_ExecutionParams.ExecutionTime.GetFormattedTime(false)));
			}
			else
			{
				float num5 = AbilityArchetype.tt_ExecutionParams.IsInstant ? 0f : AbilityArchetype.tt_ExecutionParams.ExecutionTime;
				num5 += AbilityArchetype.tt_alchemyPowerLevel.GetAddedExecutionTime();
				text2 = ZString.Format<string, string>("<color={0}><b>{1}</b> Execution</color>", UIManager.EmberColor.ToHex(), num5.GetFormattedTime(false));
			}
			if (!string.IsNullOrEmpty(text))
			{
				string right = ZString.Format<string>("<b>{0}</b> Cooldown", text);
				dataBlock.AppendLine(text2, right);
			}
			else
			{
				dataBlock.AppendLine(text2, 0);
			}
			string text3 = string.Empty;
			if (flag)
			{
				text3 = ZString.Format<string>("<b>{0}</b> Memorization", arg);
			}
			string left;
			if (AbilityArchetype.tt_ExecutionParams.TryGetValidMovementTooltip(out left) || !string.IsNullOrEmpty(text3))
			{
				dataBlock.AppendLine(left, text3);
			}
			AbilityArchetype.tt_ExecutionParams.FillTooltip(tooltip);
			if (AbilityArchetype.tt_alchemyPowerLevel != AlchemyPowerLevel.None)
			{
				int alchemyEssenceCost = GlobalSettings.Values.Ashen.GetAlchemyEssenceCost(AbilityArchetype.tt_alchemyPowerLevel);
				Color color = (entity && entity.CollectionController != null && entity.CollectionController.GetEmberEssenceCount() >= alchemyEssenceCost) ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor;
				tooltip.RequirementsBlock.AppendLine(ZString.Format<string, int, string>("<color={0}>{1}</color> <color={2}>Ember Essence</color>", color.ToHex(), alchemyEssenceCost, UIManager.EmberColor.ToHex()), 0);
			}
			string roleLevelRequirementString = this.GetRoleLevelRequirementString(entity, abilityLevel);
			string staminaCostTooltip = AbilityArchetype.tt_ExecutionParams.GetStaminaCostTooltip(this.HasEnoughStamina(entity, AbilityArchetype.tt_ExecutionParams));
			tooltip.RequirementsBlock.AppendLine(roleLevelRequirementString, staminaCostTooltip);
			string left2;
			string right2;
			if (this.TryGetTargetDistanceAngleForTooltip(entity, out left2, out right2))
			{
				tooltip.RequirementsBlock.AppendLine(left2, right2);
			}
			string txt;
			if (AbilityArchetype.tt_ExecutionParams.TryGetValidStanceTooltip(entity, out txt))
			{
				tooltip.RequirementsBlock.AppendLine(txt, 0);
			}
		}

		// Token: 0x06005025 RID: 20517 RVA: 0x001CC800 File Offset: 0x001CAA00
		private string GetRoleLevelRequirementString(GameEntity entity, int abilityLevel)
		{
			bool flag = false;
			if (entity && entity.CharacterData)
			{
				flag = (this.m_mastery && entity.CharacterData.ActiveMasteryId == this.m_mastery.Id);
				if (flag && this.m_specialization)
				{
					flag = (entity.CharacterData.SpecializedRoleId == this.m_specialization.Id);
				}
			}
			Color color = UIManager.RequirementsNotMetColor;
			int arg = this.MinimumLevel;
			string text = (this.m_specialization != null) ? this.m_specialization.DisplayName : this.m_mastery.DisplayName;
			string text2 = string.Empty;
			string arg2 = string.Empty;
			if (flag)
			{
				int currentTierLevel = this.GetCurrentTierLevel(abilityLevel);
				if (ClientGameManager.InputManager.HoldingShift && abilityLevel < this.GetMaxTierLevel())
				{
					color = Colors.Gold;
					arg = abilityLevel;
					text2 = ZString.Format<string, string>("{0} (<color={1}>next tier</color>)", text, Colors.Gold.ToHex());
				}
				else
				{
					color = ((abilityLevel >= this.MinimumLevel) ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor);
					arg = currentTierLevel;
					if (UIManager.TooltipShowMore)
					{
						int nextTierLevel = this.GetNextTierLevel(abilityLevel);
						if (currentTierLevel != nextTierLevel)
						{
							arg2 = ZString.Format<string, int>(" (<color={0}>next tier at {1}</color>)", Colors.Gold.ToHex(), nextTierLevel);
						}
					}
				}
			}
			if (string.IsNullOrEmpty(text2))
			{
				text2 = text.Color(color);
			}
			string arg3 = ZString.Format<string, int>("<color={0}>{1}</color>", color.ToHex(), arg);
			return ZString.Format<string, string, string>("<b>{0}</b> {1}{2}", arg3, text2, arg2);
		}

		// Token: 0x06005026 RID: 20518 RVA: 0x00075BC1 File Offset: 0x00073DC1
		protected virtual bool TryGetTargetDistanceAngleForTooltip(GameEntity entity, out string distance, out string angle)
		{
			distance = string.Empty;
			angle = string.Empty;
			return false;
		}

		// Token: 0x06005027 RID: 20519 RVA: 0x00075BD2 File Offset: 0x00073DD2
		public override GameObject GetInstanceUIPrefabReference()
		{
			return ClientGameManager.UIManager.AbilityInstanceUIPrefab;
		}

		// Token: 0x06005028 RID: 20520 RVA: 0x001CC980 File Offset: 0x001CAB80
		private bool CanLearn(GameEntity entity, out string errorMessage)
		{
			errorMessage = null;
			if (entity == null)
			{
				return false;
			}
			ArchetypeInstance archetypeInstance;
			if (entity.CollectionController.Abilities.TryGetInstanceForArchetypeId(this.m_id, out archetypeInstance))
			{
				errorMessage = "You already know this ability!";
				return false;
			}
			ArchetypeInstance archetypeInstance2;
			if (!entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(this.Mastery.Id, out archetypeInstance2))
			{
				errorMessage = "You do not know the required role!";
				return false;
			}
			if (this.Specialization != null && (archetypeInstance2.MasteryData.Specialization == null || archetypeInstance2.MasteryData.Specialization.Value != this.Specialization.Id))
			{
				errorMessage = "You are not specialized in the proper specialization!";
				return false;
			}
			if (archetypeInstance2.GetAssociatedLevel(entity) < (float)this.LevelRange.Min)
			{
				errorMessage = this.Mastery.DisplayName + " level not high enough!";
				return false;
			}
			return true;
		}

		// Token: 0x06005029 RID: 20521 RVA: 0x001CCA6C File Offset: 0x001CAC6C
		private bool AddToPlayer(GameEntity entity, ItemAddContext context, out ArchetypeInstance instance)
		{
			instance = null;
			string text;
			if (!GameManager.IsServer || !this.CanLearn(entity, out text))
			{
				return false;
			}
			instance = this.CreateNewInstance();
			entity.CollectionController.Abilities.Add(instance, true);
			ArchetypeAddedTransaction response = new ArchetypeAddedTransaction
			{
				Op = OpCodes.Ok,
				Context = context,
				Instance = instance,
				TargetContainer = instance.ContainerInstance.Id
			};
			entity.NetworkEntity.PlayerRpcHandler.AddItemResponse(response);
			return true;
		}

		// Token: 0x170011BB RID: 4539
		// (get) Token: 0x0600502A RID: 20522 RVA: 0x0004BC2B File Offset: 0x00049E2B
		BaseArchetype IMerchantInventory.Archetype
		{
			get
			{
				return this;
			}
		}

		// Token: 0x0600502B RID: 20523 RVA: 0x00045BCD File Offset: 0x00043DCD
		ulong IMerchantInventory.GetSellPrice(GameEntity entity)
		{
			return 0UL;
		}

		// Token: 0x0600502C RID: 20524 RVA: 0x00045BCD File Offset: 0x00043DCD
		ulong IMerchantInventory.GetEventCost(GameEntity entity)
		{
			return 0UL;
		}

		// Token: 0x0600502D RID: 20525 RVA: 0x00075BDE File Offset: 0x00073DDE
		bool IMerchantInventory.EntityCanAcquire(GameEntity entity, out string errorMessage)
		{
			return this.CanLearn(entity, out errorMessage);
		}

		// Token: 0x0600502E RID: 20526 RVA: 0x00075BE8 File Offset: 0x00073DE8
		bool IMerchantInventory.AddToPlayer(GameEntity entity, ItemAddContext context, uint quantity, ItemFlags itemFlags, bool markAsSoulbound, out ArchetypeInstance instance)
		{
			return this.AddToPlayer(entity, context, out instance);
		}

		// Token: 0x04004804 RID: 18436
		private const float kServerCooldownThreshold = 0.1f;

		// Token: 0x04004805 RID: 18437
		[SerializeField]
		private int m_memorizationTime = 10;

		// Token: 0x04004806 RID: 18438
		[SerializeField]
		private MinMaxIntRange m_levelRange = new MinMaxIntRange(1, 100);

		// Token: 0x04004807 RID: 18439
		[SerializeField]
		protected MasteryArchetype m_mastery;

		// Token: 0x04004808 RID: 18440
		[SerializeField]
		protected SpecializedRole m_specialization;

		// Token: 0x04004809 RID: 18441
		[SerializeField]
		private AudioClipCollection m_dragDropAudioOverride;

		// Token: 0x0400480A RID: 18442
		[SerializeField]
		private AbilityArchetype m_nextTier;

		// Token: 0x0400480B RID: 18443
		[SerializeField]
		private AbilityArchetype m_previousTier;

		// Token: 0x0400480C RID: 18444
		[SerializeField]
		protected AbilityVFXScriptable m_vfxOverride;

		// Token: 0x0400480D RID: 18445
		[SerializeField]
		protected AbilityVFX m_vfx;

		// Token: 0x0400480E RID: 18446
		protected const string kAnimGroupName = "Animations";

		// Token: 0x0400480F RID: 18447
		[SerializeField]
		protected AbilityAnimation m_animation;

		// Token: 0x04004810 RID: 18448
		[SerializeField]
		private bool m_deferHandIk;

		// Token: 0x04004811 RID: 18449
		[SerializeField]
		private float m_deferHandIkDuration;

		// Token: 0x04004812 RID: 18450
		[SerializeField]
		private DummyClass m_dummy;

		// Token: 0x04004813 RID: 18451
		[SerializeField]
		protected TieredAbilityParameters m_baseParams;

		// Token: 0x04004814 RID: 18452
		[SerializeField]
		protected TieredAbilityParametersOverride[] m_overrideParams;

		// Token: 0x04004815 RID: 18453
		private const string kAlchemyI = "Alchemy I";

		// Token: 0x04004816 RID: 18454
		private const string kAlchemyII = "Alchemy II";

		// Token: 0x04004817 RID: 18455
		[SerializeField]
		private AbilityArchetype.AlchemyData m_alchemyDataI;

		// Token: 0x04004818 RID: 18456
		[SerializeField]
		private AbilityArchetype.AlchemyData m_alchemyDataII;

		// Token: 0x04004819 RID: 18457
		private const string kNpcGroup = "NPCs";

		// Token: 0x0400481A RID: 18458
		[SerializeField]
		private EmotiveCalls m_npcEmotes;

		// Token: 0x0400481B RID: 18459
		[SerializeField]
		private AbilityArchetype.NpcUseCases m_npcUseCase;

		// Token: 0x0400481C RID: 18460
		private const string kNoCooldownText = "No";

		// Token: 0x0400481D RID: 18461
		protected static AlchemyPowerLevel tt_alchemyPowerLevel;

		// Token: 0x0400481E RID: 18462
		protected static ExecutionParams tt_ExecutionParams;

		// Token: 0x0400481F RID: 18463
		protected static TargetingParams tt_targetingParams;

		// Token: 0x04004820 RID: 18464
		protected static KinematicParameters tt_kinematicParams;

		// Token: 0x04004821 RID: 18465
		protected static CombatEffect tt_combatEffect;

		// Token: 0x04004822 RID: 18466
		protected static ReagentItem tt_reagentItem;

		// Token: 0x02000A21 RID: 2593
		[Serializable]
		private class AlchemyData
		{
			// Token: 0x170011BC RID: 4540
			// (get) Token: 0x06005030 RID: 20528 RVA: 0x00075C12 File Offset: 0x00073E12
			private bool m_showVfx
			{
				get
				{
					return this.m_overrideVfx && this.m_vfxOverride == null;
				}
			}

			// Token: 0x170011BD RID: 4541
			// (get) Token: 0x06005031 RID: 20529 RVA: 0x00075825 File Offset: 0x00073A25
			private IEnumerable GetVfxOverrides
			{
				get
				{
					return SolOdinUtilities.GetDropdownItems<AbilityVFXScriptable>();
				}
			}

			// Token: 0x170011BE RID: 4542
			// (get) Token: 0x06005032 RID: 20530 RVA: 0x00075C2A File Offset: 0x00073E2A
			internal bool OverrideVfx
			{
				get
				{
					return this.m_overrideVfx;
				}
			}

			// Token: 0x170011BF RID: 4543
			// (get) Token: 0x06005033 RID: 20531 RVA: 0x00075C32 File Offset: 0x00073E32
			internal string DisplayName
			{
				get
				{
					return this.m_displayName;
				}
			}

			// Token: 0x170011C0 RID: 4544
			// (get) Token: 0x06005034 RID: 20532 RVA: 0x00075C3A File Offset: 0x00073E3A
			internal string Description
			{
				get
				{
					return this.m_description;
				}
			}

			// Token: 0x170011C1 RID: 4545
			// (get) Token: 0x06005035 RID: 20533 RVA: 0x00075C42 File Offset: 0x00073E42
			internal TieredAbilityParametersOverride[] Overrides
			{
				get
				{
					return this.m_overrideParams;
				}
			}

			// Token: 0x170011C2 RID: 4546
			// (get) Token: 0x06005036 RID: 20534 RVA: 0x00075C4A File Offset: 0x00073E4A
			internal AbilityVFX VFX
			{
				get
				{
					if (!this.m_vfxOverride)
					{
						return this.m_vfx;
					}
					return this.m_vfxOverride.VFX;
				}
			}

			// Token: 0x04004823 RID: 18467
			[SerializeField]
			private string m_displayName;

			// Token: 0x04004824 RID: 18468
			[TextArea]
			[SerializeField]
			private string m_description;

			// Token: 0x04004825 RID: 18469
			[SerializeField]
			private TieredAbilityParametersOverride[] m_overrideParams;

			// Token: 0x04004826 RID: 18470
			[SerializeField]
			private bool m_overrideVfx;

			// Token: 0x04004827 RID: 18471
			[SerializeField]
			protected AbilityVFXScriptable m_vfxOverride;

			// Token: 0x04004828 RID: 18472
			[SerializeField]
			protected AbilityVFX m_vfx;
		}

		// Token: 0x02000A22 RID: 2594
		public enum NpcUseCases
		{
			// Token: 0x0400482A RID: 18474
			CombatOnly,
			// Token: 0x0400482B RID: 18475
			PursueOnly,
			// Token: 0x0400482C RID: 18476
			Both = 10
		}
	}
}
