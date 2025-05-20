using System;
using System.Collections;
using Cysharp.Text;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A91 RID: 2705
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Reagent")]
	public class ReagentItem : StackableItem
	{
		// Token: 0x1700131F RID: 4895
		// (get) Token: 0x060053B9 RID: 21433 RVA: 0x00045BCA File Offset: 0x00043DCA
		public override bool CanPlaceInPouch
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001320 RID: 4896
		// (get) Token: 0x060053BA RID: 21434 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool IsReagent
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17001321 RID: 4897
		// (get) Token: 0x060053BB RID: 21435 RVA: 0x00077D9A File Offset: 0x00075F9A
		public ReagentType Type
		{
			get
			{
				if (!(this.m_reagentAbility != null))
				{
					return ReagentType.None;
				}
				return this.m_reagentAbility.Type;
			}
		}

		// Token: 0x17001322 RID: 4898
		// (get) Token: 0x060053BC RID: 21436 RVA: 0x00077DB7 File Offset: 0x00075FB7
		public ReagentAbility AssociatedAbility
		{
			get
			{
				return this.m_reagentAbility;
			}
		}

		// Token: 0x17001323 RID: 4899
		// (get) Token: 0x060053BD RID: 21437 RVA: 0x001D8838 File Offset: 0x001D6A38
		private string ReagentInfo
		{
			get
			{
				if (this.m_reagentAbility == null)
				{
					return "NONE";
				}
				string result = string.Empty;
				using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
				{
					utf16ValueStringBuilder.Append("Reagent Type: " + this.m_reagentAbility.Type.ToString() + "\n");
					utf16ValueStringBuilder.AppendLine();
					utf16ValueStringBuilder.Append("Ability: " + this.m_reagentAbility.DisplayName + "\n");
					utf16ValueStringBuilder.Append("Description: " + this.m_reagentAbility.Description);
					result = utf16ValueStringBuilder.ToString();
				}
				return result;
			}
		}

		// Token: 0x17001324 RID: 4900
		// (get) Token: 0x060053BE RID: 21438 RVA: 0x00077DBF File Offset: 0x00075FBF
		public int LevelRequirementLevel
		{
			get
			{
				if (this.m_levelRequirement == null)
				{
					return 0;
				}
				return this.m_levelRequirement.Level;
			}
		}

		// Token: 0x17001325 RID: 4901
		// (get) Token: 0x060053BF RID: 21439 RVA: 0x001D8908 File Offset: 0x001D6B08
		private string TargetingInfo
		{
			get
			{
				if (!this.m_showTargeting)
				{
					return "NONE";
				}
				TargetingParams cachedTargetingParams = this.m_cachedTargetingParams;
				string result = string.Empty;
				using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
				{
					utf16ValueStringBuilder.Append("\t\t BASE \t COMBINED \n");
					if (this.m_showAoe)
					{
						utf16ValueStringBuilder.Append(string.Concat(new string[]
						{
							"AoeMaxTargets:\t ",
							cachedTargetingParams.AoeMaxTargets.ToString(),
							" \t ",
							(cachedTargetingParams.AoeMaxTargets + this.m_aoeMaxTargetMod).ToString(),
							"\n"
						}));
					}
					result = utf16ValueStringBuilder.ToString();
				}
				return result;
			}
		}

		// Token: 0x17001326 RID: 4902
		// (get) Token: 0x060053C0 RID: 21440 RVA: 0x001D89CC File Offset: 0x001D6BCC
		private string InstantInfo
		{
			get
			{
				if (!this.m_showInstantVitals || this.m_instantMods == null)
				{
					return "NONE";
				}
				CombatFlags combatFlags = this.m_cachedCombatEffect.Effects.CombatFlags;
				VitalMods mods = this.m_cachedCombatEffect.Effects.Instant[0].Mods;
				ReagentItem.ReagentInstantMods instantMods = this.m_instantMods;
				string result = string.Empty;
				using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
				{
					utf16ValueStringBuilder.Append("\t\t BASE \t COMBINED \n");
					utf16ValueStringBuilder.Append(string.Concat(new string[]
					{
						"CombatFlags:\t ",
						combatFlags.ToString(),
						" \t ",
						(combatFlags | instantMods.Flags).ToString(),
						"\n"
					}));
					utf16ValueStringBuilder.Append(string.Format("ValueAdd:\t {0:F1} \t {1:F1}\n", mods.ValueAdditive, mods.ValueAdditive + instantMods.ValueAdditive));
					utf16ValueStringBuilder.Append(string.Format("ValueMult:\t {0:F1} \t {1:F1}\n", mods.ValueMultiplier, mods.ValueMultiplier + instantMods.ValueMultiplier));
					utf16ValueStringBuilder.Append(string.Format("ThreatMult:\t {0:F1} \t {1:F1}", mods.ThreatMultiplier, mods.ThreatMultiplier + instantMods.ThreatMultiplier));
					result = utf16ValueStringBuilder.ToString();
				}
				return result;
			}
		}

		// Token: 0x17001327 RID: 4903
		// (get) Token: 0x060053C1 RID: 21441 RVA: 0x001D8B48 File Offset: 0x001D6D48
		private string OverTimeInfo
		{
			get
			{
				if (!this.m_showOverTime)
				{
					return "NONE";
				}
				Effects effects = this.m_cachedCombatEffect.Effects;
				VitalsEffect_OverTime vitalsEffect_OverTime = effects.OverTime[0];
				string result = string.Empty;
				using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
				{
					utf16ValueStringBuilder.Append("\t\t BASE \t COMBINED \n");
					utf16ValueStringBuilder.Append(string.Concat(new string[]
					{
						"TickRateMod:\t ",
						effects.TickRate.ToString(),
						" \t ",
						(effects.TickRate + this.m_tickRateMod).ToString(),
						"\n"
					}));
					utf16ValueStringBuilder.Append("  ValueMod:\t " + vitalsEffect_OverTime.Value.ToString() + " \t " + (vitalsEffect_OverTime.Value + this.m_overTimeMod).ToString());
					if (this.m_tickRateMod != 0 || this.m_overTimeMod != 0 || this.m_durationMod != 0)
					{
						utf16ValueStringBuilder.AppendLine();
						utf16ValueStringBuilder.AppendLine();
						utf16ValueStringBuilder.Append("---- BASE TICK ----\n");
						utf16ValueStringBuilder.Append(this.m_cachedCombatEffect.GetOverTimeDetails(0, 0, 0));
						utf16ValueStringBuilder.AppendLine();
						utf16ValueStringBuilder.AppendLine();
						utf16ValueStringBuilder.Append("---- REAGENT TICK ----\n");
						utf16ValueStringBuilder.Append(this.m_cachedCombatEffect.GetOverTimeDetails(this.m_tickRateMod, this.m_overTimeMod, this.m_durationMod));
					}
					result = utf16ValueStringBuilder.ToString();
				}
				return result;
			}
		}

		// Token: 0x17001328 RID: 4904
		// (get) Token: 0x060053C2 RID: 21442 RVA: 0x001D8CE0 File Offset: 0x001D6EE0
		private string StatusEffectInfo
		{
			get
			{
				if (!this.m_showStatusEffects)
				{
					return "NONE";
				}
				StatusEffect statusEffect = this.m_cachedCombatEffect.Effects.StatusEffect;
				string result = string.Empty;
				using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
				{
					utf16ValueStringBuilder.Append("\t\t BASE \t COMBINED \n");
					for (int i = 0; i < statusEffect.Values.Length; i++)
					{
						StatType type = statusEffect.Values[i].Type;
						int value = statusEffect.Values[i].Value;
						utf16ValueStringBuilder.Append(string.Concat(new string[]
						{
							"  ValueMod:\t ",
							value.ToString(),
							" \t ",
							(value + this.m_statusEffectMod).ToString(),
							" \t\t ",
							type.ToString()
						}));
						if (i < statusEffect.Values.Length - 1)
						{
							utf16ValueStringBuilder.AppendLine();
						}
					}
					result = utf16ValueStringBuilder.ToString();
				}
				return result;
			}
		}

		// Token: 0x17001329 RID: 4905
		// (get) Token: 0x060053C3 RID: 21443 RVA: 0x00077DD6 File Offset: 0x00075FD6
		private bool m_showExpirationInfo
		{
			get
			{
				return this.m_showDuration || this.m_showTrigger;
			}
		}

		// Token: 0x1700132A RID: 4906
		// (get) Token: 0x060053C4 RID: 21444 RVA: 0x001D8DF8 File Offset: 0x001D6FF8
		private string ExpirationInfo
		{
			get
			{
				if (!this.m_showExpirationInfo)
				{
					return "NONE";
				}
				ExpirationParams expiration = this.m_cachedCombatEffect.Expiration;
				string result = string.Empty;
				using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
				{
					utf16ValueStringBuilder.Append("\t\t BASE \t COMBINED \n");
					if (this.m_showDuration)
					{
						utf16ValueStringBuilder.Append("  Duration:\t " + expiration.MaxDuration.ToString() + " \t " + (expiration.MaxDuration + this.m_durationMod).ToString());
					}
					if (this.m_showTrigger)
					{
						if (this.m_showTrigger)
						{
							utf16ValueStringBuilder.AppendLine();
						}
						utf16ValueStringBuilder.Append("  Triggers:\t " + expiration.TriggerCount.ToString() + " \t " + (expiration.TriggerCount + this.m_triggerMod).ToString());
					}
					result = utf16ValueStringBuilder.ToString();
				}
				return result;
			}
		}

		// Token: 0x060053C5 RID: 21445 RVA: 0x00077DE8 File Offset: 0x00075FE8
		private int GetMinLevel()
		{
			if (this.m_levelRequirement == null)
			{
				return 1;
			}
			return this.m_levelRequirement.Level;
		}

		// Token: 0x060053C6 RID: 21446 RVA: 0x001D8EF8 File Offset: 0x001D70F8
		private void CacheParameters()
		{
			if (Application.isPlaying && this.m_cachedParameters)
			{
				return;
			}
			if (this.m_reagentAbility == null)
			{
				this.m_cachedExecutionParameters = null;
				this.m_cachedTargetingParams = null;
				this.m_cachedKinematicParams = null;
				this.m_cachedCombatEffect = null;
			}
			else
			{
				float level = (float)this.m_sampleLevel;
				IExecutable executable;
				if (this.m_reagentAbility.TryGetAsType(out executable))
				{
					this.m_cachedExecutionParameters = executable.GetExecutionParams(level, AlchemyPowerLevel.None);
				}
				ICombatEffectSource combatEffectSource;
				if (this.m_reagentAbility.TryGetAsType(out combatEffectSource))
				{
					this.m_cachedCombatEffect = combatEffectSource.GetCombatEffect(level, AlchemyPowerLevel.None);
					this.m_cachedTargetingParams = combatEffectSource.GetTargetingParams(level, AlchemyPowerLevel.None);
					this.m_cachedKinematicParams = combatEffectSource.GetKinematicParams(level, AlchemyPowerLevel.None);
				}
			}
			this.m_cachedParameters = Application.isPlaying;
		}

		// Token: 0x1700132B RID: 4907
		// (get) Token: 0x060053C7 RID: 21447 RVA: 0x00077DFF File Offset: 0x00075FFF
		private bool m_hasSecondary
		{
			get
			{
				return this.m_cachedCombatEffect != null && this.m_cachedCombatEffect.HasSecondary;
			}
		}

		// Token: 0x1700132C RID: 4908
		// (get) Token: 0x060053C8 RID: 21448 RVA: 0x00077E16 File Offset: 0x00076016
		private bool m_showTargeting
		{
			get
			{
				return this.m_showAoe;
			}
		}

		// Token: 0x1700132D RID: 4909
		// (get) Token: 0x060053C9 RID: 21449 RVA: 0x00077E1E File Offset: 0x0007601E
		private bool m_showAoe
		{
			get
			{
				return this.m_cachedTargetingParams != null && this.m_cachedTargetingParams.TargetType.IsAOE();
			}
		}

		// Token: 0x1700132E RID: 4910
		// (get) Token: 0x060053CA RID: 21450 RVA: 0x00077E3A File Offset: 0x0007603A
		private bool m_showInstantVitals
		{
			get
			{
				CombatEffect cachedCombatEffect = this.m_cachedCombatEffect;
				return ((cachedCombatEffect != null) ? cachedCombatEffect.Effects : null) != null && this.m_cachedCombatEffect.Effects.HasInstantVitals;
			}
		}

		// Token: 0x1700132F RID: 4911
		// (get) Token: 0x060053CB RID: 21451 RVA: 0x00077E62 File Offset: 0x00076062
		private bool m_showStatusEffects
		{
			get
			{
				CombatEffect cachedCombatEffect = this.m_cachedCombatEffect;
				return ((cachedCombatEffect != null) ? cachedCombatEffect.Effects : null) != null && this.m_cachedCombatEffect.Effects.HasStatusEffects;
			}
		}

		// Token: 0x17001330 RID: 4912
		// (get) Token: 0x060053CC RID: 21452 RVA: 0x001D8FAC File Offset: 0x001D71AC
		private bool m_showOverTime
		{
			get
			{
				CombatEffect cachedCombatEffect = this.m_cachedCombatEffect;
				return ((cachedCombatEffect != null) ? cachedCombatEffect.Effects : null) != null && this.m_cachedCombatEffect.Effects.HasOverTimeVitals && this.m_cachedCombatEffect.Effects.OverTime != null && this.m_cachedCombatEffect.Effects.OverTime.Length != 0;
			}
		}

		// Token: 0x17001331 RID: 4913
		// (get) Token: 0x060053CD RID: 21453 RVA: 0x00077E8A File Offset: 0x0007608A
		private bool m_showDuration
		{
			get
			{
				return this.m_cachedCombatEffect != null && this.m_cachedCombatEffect.Effects.HasLasting;
			}
		}

		// Token: 0x17001332 RID: 4914
		// (get) Token: 0x060053CE RID: 21454 RVA: 0x00077EA6 File Offset: 0x000760A6
		private bool m_showTrigger
		{
			get
			{
				return this.m_cachedCombatEffect != null && this.m_cachedCombatEffect.IsTriggerBased;
			}
		}

		// Token: 0x17001333 RID: 4915
		// (get) Token: 0x060053CF RID: 21455 RVA: 0x00077EBD File Offset: 0x000760BD
		private bool m_isValid
		{
			get
			{
				return this.m_reagentAbility != null;
			}
		}

		// Token: 0x17001334 RID: 4916
		// (get) Token: 0x060053D0 RID: 21456 RVA: 0x00077ECB File Offset: 0x000760CB
		private IEnumerable GetAbilities
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<ReagentAbility>();
			}
		}

		// Token: 0x17001335 RID: 4917
		// (get) Token: 0x060053D1 RID: 21457 RVA: 0x00077ED2 File Offset: 0x000760D2
		public bool ColorInstantMods
		{
			get
			{
				return this.m_reagentAbility != null && this.m_reagentAbility.HasInstant && this.m_instantMods != null && this.m_instantMods.HasMods();
			}
		}

		// Token: 0x17001336 RID: 4918
		// (get) Token: 0x060053D2 RID: 21458 RVA: 0x00077F04 File Offset: 0x00076104
		public bool ColorDurationMod
		{
			get
			{
				return this.m_reagentAbility != null && this.m_reagentAbility.HasDuration && this.m_durationMod != 0;
			}
		}

		// Token: 0x17001337 RID: 4919
		// (get) Token: 0x060053D3 RID: 21459 RVA: 0x00077F2C File Offset: 0x0007612C
		public bool ColorOverTimeMod
		{
			get
			{
				return this.m_reagentAbility != null && this.m_reagentAbility.HasOverTime && this.m_overTimeMod != 0;
			}
		}

		// Token: 0x17001338 RID: 4920
		// (get) Token: 0x060053D4 RID: 21460 RVA: 0x00077F54 File Offset: 0x00076154
		public bool ColorTickRateMod
		{
			get
			{
				return this.m_reagentAbility != null && this.m_reagentAbility.HasOverTime && this.m_tickRateMod != 0;
			}
		}

		// Token: 0x17001339 RID: 4921
		// (get) Token: 0x060053D5 RID: 21461 RVA: 0x00077F7C File Offset: 0x0007617C
		public bool ColorStatusEffectMod
		{
			get
			{
				return this.m_reagentAbility != null && this.m_reagentAbility.HasStatusEffects && this.m_statusEffectMod != 0;
			}
		}

		// Token: 0x1700133A RID: 4922
		// (get) Token: 0x060053D6 RID: 21462 RVA: 0x00077FA4 File Offset: 0x000761A4
		public bool ColorAoeTargetMod
		{
			get
			{
				return this.m_reagentAbility != null && this.m_reagentAbility.HasAoe && this.m_aoeMaxTargetMod != 0;
			}
		}

		// Token: 0x1700133B RID: 4923
		// (get) Token: 0x060053D7 RID: 21463 RVA: 0x00077FCC File Offset: 0x000761CC
		public bool ColorTriggerCountMod
		{
			get
			{
				return this.m_reagentAbility != null && this.m_reagentAbility.IsTriggerBased && this.m_triggerMod != 0;
			}
		}

		// Token: 0x060053D8 RID: 21464 RVA: 0x00077FF4 File Offset: 0x000761F4
		public bool MeetsRequirements(GameEntity entity)
		{
			return this.m_levelRequirement != null && this.m_levelRequirement.MeetsAllRequirements(entity);
		}

		// Token: 0x060053D9 RID: 21465 RVA: 0x001D9008 File Offset: 0x001D7208
		public bool HasRequiredRoleAndSpec(GameEntity entity)
		{
			if (this.AssociatedAbility && this.AssociatedAbility.Mastery && entity && entity.CharacterData)
			{
				if (!entity.CharacterData.ActiveMasteryId.IsEmpty && entity.CharacterData.ActiveMasteryId == this.AssociatedAbility.Mastery.Id)
				{
					return true;
				}
				if (this.AssociatedAbility.Specialization && !entity.CharacterData.SpecializedRoleId.IsEmpty && entity.CharacterData.SpecializedRoleId == this.AssociatedAbility.Specialization.Id)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060053DA RID: 21466 RVA: 0x0007800C File Offset: 0x0007620C
		public ReagentItem.ReagentInstantMods GetInstantMods()
		{
			if (!(this.m_reagentAbility != null) || !this.m_reagentAbility.HasInstant)
			{
				return null;
			}
			return this.m_instantMods;
		}

		// Token: 0x060053DB RID: 21467 RVA: 0x00078031 File Offset: 0x00076231
		public int GetDurationMod()
		{
			if (!(this.m_reagentAbility != null) || !this.m_reagentAbility.HasDuration)
			{
				return 0;
			}
			return this.m_durationMod;
		}

		// Token: 0x060053DC RID: 21468 RVA: 0x00078056 File Offset: 0x00076256
		public int GetOverTimeMod()
		{
			if (!(this.m_reagentAbility != null) || !this.m_reagentAbility.HasOverTime)
			{
				return 0;
			}
			return this.m_overTimeMod;
		}

		// Token: 0x060053DD RID: 21469 RVA: 0x0007807B File Offset: 0x0007627B
		public int GetTickRateMod()
		{
			if (!(this.m_reagentAbility != null) || !this.m_reagentAbility.HasOverTime)
			{
				return 0;
			}
			return this.m_tickRateMod;
		}

		// Token: 0x060053DE RID: 21470 RVA: 0x000780A0 File Offset: 0x000762A0
		public int GetStatusEffectMod()
		{
			if (!(this.m_reagentAbility != null) || !this.m_reagentAbility.HasStatusEffects)
			{
				return 0;
			}
			return this.m_statusEffectMod;
		}

		// Token: 0x060053DF RID: 21471 RVA: 0x000780C5 File Offset: 0x000762C5
		public int GetAoeTargetMod()
		{
			if (!(this.m_reagentAbility != null) || !this.m_reagentAbility.HasAoe)
			{
				return 0;
			}
			return this.m_aoeMaxTargetMod;
		}

		// Token: 0x060053E0 RID: 21472 RVA: 0x000780EA File Offset: 0x000762EA
		public int GetTriggerCountMod()
		{
			if (!(this.m_reagentAbility != null) || !this.m_reagentAbility.IsTriggerBased)
			{
				return 0;
			}
			return this.m_triggerMod;
		}

		// Token: 0x060053E1 RID: 21473 RVA: 0x001D90DC File Offset: 0x001D72DC
		public override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			base.FillTooltipBlocks(tooltip, instance, entity);
			if (this.m_reagentAbility == null)
			{
				return;
			}
			TooltipExtensions.AddRoleLevelRequirementForReagent(tooltip, entity, this.m_levelRequirement, this);
			TooltipTextBlock reagentBlock = tooltip.ReagentBlock;
			string title = this.m_reagentAbility ? ZString.Format<string, string>("{0} for {1}", this.Type.ToStringWithSpaces(), this.m_reagentAbility.DisplayName) : this.Type.ToStringWithSpaces();
			reagentBlock.Title = title;
			if (this.m_reagentAbility.HasInstant && this.m_instantMods != null && this.m_instantMods.HasMods())
			{
				string description = this.m_overrideDisplayDescription ? this.m_displayDescription : "Damage";
				this.AddLineToBlock(reagentBlock, this.m_instantMods.ValueAdditive, string.Empty, string.Empty, description);
				if (this.m_instantMods.ValueMultiplier != 0f)
				{
					int num = NumberExtensions.ToIntTowardsZero(this.m_instantMods.ValueMultiplier * 100f);
					if (num > 0)
					{
						num -= 100;
					}
					this.AddLineToBlock(reagentBlock, num, string.Empty, "%", description);
				}
				if (this.m_instantMods.ThreatMultiplier != 0f)
				{
					int num2 = NumberExtensions.ToIntTowardsZero(this.m_instantMods.ThreatMultiplier * 100f);
					if (num2 > 0)
					{
						num2 -= 100;
					}
					this.AddLineToBlock(reagentBlock, num2, string.Empty, "%", "Threat");
				}
			}
			if (this.m_reagentAbility.HasDuration)
			{
				this.AddLineToBlock(reagentBlock, this.m_durationMod, string.Empty, "s", "Duration");
			}
			if (this.m_reagentAbility.HasOverTime)
			{
				string arg = this.m_overrideDisplayDescription ? this.m_displayDescription : "Damage";
				this.AddLineToBlock(reagentBlock, this.m_overTimeMod, string.Empty, string.Empty, ZString.Format<string>("{0} per tick", arg));
				this.AddLineToBlock(reagentBlock, this.m_tickRateMod, string.Empty, string.Empty, "Tick Rate");
			}
			if (this.m_reagentAbility.HasStatusEffects)
			{
				this.AddLineToBlock(reagentBlock, this.m_statusEffectMod, string.Empty, string.Empty, "Status Effect Mod");
			}
			if (this.m_reagentAbility.HasAoe)
			{
				string description2 = (this.m_aoeMaxTargetMod > 1) ? "AoE Targets" : "AoE Target";
				this.AddLineToBlock(reagentBlock, this.m_aoeMaxTargetMod, string.Empty, string.Empty, description2);
			}
			if (this.m_reagentAbility.IsTriggerBased)
			{
				this.AddLineToBlock(reagentBlock, this.m_triggerMod, string.Empty, string.Empty, "Trigger Count");
			}
		}

		// Token: 0x060053E2 RID: 21474 RVA: 0x001D9370 File Offset: 0x001D7570
		private void AddToTooltip(TooltipTextBlock block, string leftSide, int rightSide, string prefix, string suffix)
		{
			if (rightSide != 0)
			{
				string str = (((rightSide > 0) ? "+" : "") + rightSide.ToString()).Color(UIManager.ReagentBonusColor);
				block.AppendLine(leftSide, prefix + str + suffix);
			}
		}

		// Token: 0x060053E3 RID: 21475 RVA: 0x001D93B8 File Offset: 0x001D75B8
		private void AddToTooltip(TooltipTextBlock block, string leftSide, float rightSide, string prefix, string suffix)
		{
			if (rightSide != 0f)
			{
				string arg = (rightSide > 0f) ? "+" : "";
				string str = string.Format("{0}{1:F1}", arg, rightSide).Color(UIManager.ReagentBonusColor);
				block.AppendLine(leftSide, prefix + str + suffix);
			}
		}

		// Token: 0x060053E4 RID: 21476 RVA: 0x0007810F File Offset: 0x0007630F
		private void AddLineToBlock(TooltipTextBlock block, int value, string prefix, string suffix, string description)
		{
			if (value != 0)
			{
				if (string.IsNullOrEmpty(prefix))
				{
					prefix = ((value > 0) ? "+" : "");
				}
				block.AppendLine(ZString.Format<string, string, int, string, string>("<color={0}>{1}{2}{3}</color> {4}", UIManager.ReagentBonusColor.ToHex(), prefix, value, suffix, description), 0);
			}
		}

		// Token: 0x060053E5 RID: 21477 RVA: 0x001D9410 File Offset: 0x001D7610
		private void AddLineToBlock(TooltipTextBlock block, float value, string prefix, string suffix, string description)
		{
			if (value != 0f)
			{
				if (string.IsNullOrEmpty(prefix))
				{
					prefix = ((value > 0f) ? "+" : "");
				}
				block.AppendLine(ZString.Format<string, string, float, string, string>("<color={0}>{1}{2:F1}{3}</color> {4}", UIManager.ReagentBonusColor.ToHex(), prefix, value, suffix, description), 0);
			}
		}

		// Token: 0x04004A9F RID: 19103
		[SerializeField]
		private int m_sampleLevel = 1;

		// Token: 0x04004AA0 RID: 19104
		[SerializeField]
		private ReagentAbility m_reagentAbility;

		// Token: 0x04004AA1 RID: 19105
		private const string kDescriptionGroup = "Reagent/Description";

		// Token: 0x04004AA2 RID: 19106
		[SerializeField]
		private bool m_overrideDisplayDescription;

		// Token: 0x04004AA3 RID: 19107
		[SerializeField]
		private string m_displayDescription = string.Empty;

		// Token: 0x04004AA4 RID: 19108
		[SerializeField]
		private LevelRequirement m_levelRequirement;

		// Token: 0x04004AA5 RID: 19109
		[SerializeField]
		private int m_aoeMaxTargetMod;

		// Token: 0x04004AA6 RID: 19110
		[SerializeField]
		private ReagentItem.ReagentInstantMods m_instantMods;

		// Token: 0x04004AA7 RID: 19111
		[SerializeField]
		private int m_tickRateMod;

		// Token: 0x04004AA8 RID: 19112
		[SerializeField]
		private int m_overTimeMod;

		// Token: 0x04004AA9 RID: 19113
		[SerializeField]
		private int m_statusEffectMod;

		// Token: 0x04004AAA RID: 19114
		[SerializeField]
		private int m_durationMod;

		// Token: 0x04004AAB RID: 19115
		[SerializeField]
		private int m_triggerMod;

		// Token: 0x04004AAC RID: 19116
		private const string kComparisonHeader = "\t\t BASE \t COMBINED \n";

		// Token: 0x04004AAD RID: 19117
		private const string kDetails = "Details";

		// Token: 0x04004AAE RID: 19118
		private const string kNone = "NONE";

		// Token: 0x04004AAF RID: 19119
		private const string kReagent = "Reagent";

		// Token: 0x04004AB0 RID: 19120
		private const string kLevelReq = "Reagent/Level Requirement";

		// Token: 0x04004AB1 RID: 19121
		private const string kTargeting = "Reagent/Targeting";

		// Token: 0x04004AB2 RID: 19122
		private const string kInstant = "Reagent/Instant Vitals";

		// Token: 0x04004AB3 RID: 19123
		private const string kOverTime = "Reagent/Over Time";

		// Token: 0x04004AB4 RID: 19124
		private const string kStatusEffect = "Reagent/Status Effect";

		// Token: 0x04004AB5 RID: 19125
		private const string kConditional = "Reagent/Conditional Mods";

		// Token: 0x04004AB6 RID: 19126
		private const string kExpiration = "Reagent/Expiration";

		// Token: 0x04004AB7 RID: 19127
		[NonSerialized]
		private bool m_cachedParameters;

		// Token: 0x04004AB8 RID: 19128
		[NonSerialized]
		private ExecutionParams m_cachedExecutionParameters;

		// Token: 0x04004AB9 RID: 19129
		[NonSerialized]
		private TargetingParams m_cachedTargetingParams;

		// Token: 0x04004ABA RID: 19130
		[NonSerialized]
		private KinematicParameters m_cachedKinematicParams;

		// Token: 0x04004ABB RID: 19131
		[NonSerialized]
		private CombatEffect m_cachedCombatEffect;

		// Token: 0x02000A92 RID: 2706
		[Serializable]
		public class ReagentInstantMods
		{
			// Token: 0x1700133C RID: 4924
			// (get) Token: 0x060053E7 RID: 21479 RVA: 0x00078169 File Offset: 0x00076369
			public CombatFlags Flags
			{
				get
				{
					return this.m_flags;
				}
			}

			// Token: 0x1700133D RID: 4925
			// (get) Token: 0x060053E8 RID: 21480 RVA: 0x00078171 File Offset: 0x00076371
			public int ValueAdditive
			{
				get
				{
					return this.m_valueAdditiveMod;
				}
			}

			// Token: 0x1700133E RID: 4926
			// (get) Token: 0x060053E9 RID: 21481 RVA: 0x00078179 File Offset: 0x00076379
			public float ValueMultiplier
			{
				get
				{
					return this.m_valueMultiplierMod;
				}
			}

			// Token: 0x1700133F RID: 4927
			// (get) Token: 0x060053EA RID: 21482 RVA: 0x00078181 File Offset: 0x00076381
			public float ThreatMultiplier
			{
				get
				{
					return this.m_threatMultiplierMod;
				}
			}

			// Token: 0x060053EB RID: 21483 RVA: 0x00078189 File Offset: 0x00076389
			public bool HasMods()
			{
				return this.m_flags != CombatFlags.None || this.m_valueAdditiveMod != 0 || this.m_valueMultiplierMod != 0f || this.m_threatMultiplierMod != 0f;
			}

			// Token: 0x04004ABC RID: 19132
			[SerializeField]
			private CombatFlags m_flags;

			// Token: 0x04004ABD RID: 19133
			[SerializeField]
			private int m_valueAdditiveMod;

			// Token: 0x04004ABE RID: 19134
			[SerializeField]
			private float m_valueMultiplierMod;

			// Token: 0x04004ABF RID: 19135
			[SerializeField]
			private float m_threatMultiplierMod;
		}
	}
}
