using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C02 RID: 3074
	public static class CombatEffectExtensions
	{
		// Token: 0x06005EC2 RID: 24258 RVA: 0x001F72B8 File Offset: 0x001F54B8
		public static void FillTooltipEffectsBlock(UniqueId archetypeId, CombatEffect combatEffect, TargetingParams targetParams, ReagentItem reagentItem, ArchetypeTooltip tooltip, GameEntity entity, bool showSecondary, byte? stackCount)
		{
			CombatEffectExtensions.FillEffectData(archetypeId, combatEffect, targetParams, reagentItem, tooltip, entity, true, false, null, null, stackCount);
			if (showSecondary)
			{
				CombatEffectWithSecondary combatEffectWithSecondary = combatEffect as CombatEffectWithSecondary;
				if (combatEffectWithSecondary != null && combatEffectWithSecondary.HasSecondary)
				{
					tooltip.EffectsBlock.AddSpacer(20);
					string txt;
					if (combatEffectWithSecondary.SecondaryTriggerCondition.TryGetChanceDescription(out txt))
					{
						tooltip.EffectsBlock.AppendLine(txt, 0);
					}
					string txt2;
					if (tooltip.ShowConditionals && combatEffectWithSecondary.Effects != null && combatEffectWithSecondary.Effects.ShowLastingConditional && combatEffectWithSecondary.Effects.LastingConditional.TryGetChanceDescription(out txt2))
					{
						tooltip.EffectsBlock.AppendLine(txt2, 0);
					}
					tooltip.EffectsBlock.AppendLine("<u><i><b>Secondary</b></i></u>", 0);
					CombatEffectExtensions.FillEffectData(archetypeId, combatEffectWithSecondary.SecondaryCombatEffect, combatEffectWithSecondary.SecondaryTargetingParams, null, tooltip, entity, false, false, null, null, stackCount);
				}
			}
		}

		// Token: 0x06005EC3 RID: 24259 RVA: 0x001F73A8 File Offset: 0x001F55A8
		public static void FillTooltipEffectsBlock(ICombatEffectSource effectSource, ArchetypeTooltip tooltip, GameEntity entity, float effectiveLevel)
		{
			CombatEffect combatEffect = effectSource.GetCombatEffect(effectiveLevel, AlchemyPowerLevel.None);
			TargetingParams targetingParams = effectSource.GetTargetingParams(effectiveLevel, AlchemyPowerLevel.None);
			CombatEffectExtensions.FillEffectData(effectSource.ArchetypeId, combatEffect, targetingParams, null, tooltip, entity, true, false, null, null, null);
			CombatEffectWithSecondary combatEffectWithSecondary = combatEffect as CombatEffectWithSecondary;
			if (combatEffectWithSecondary != null && combatEffectWithSecondary.HasSecondary)
			{
				tooltip.EffectsBlock.AddSpacer(20);
				string txt;
				if (combatEffectWithSecondary.SecondaryTriggerCondition.TryGetChanceDescription(out txt))
				{
					tooltip.EffectsBlock.AppendLine(txt, 0);
				}
				string txt2;
				if (tooltip.ShowConditionals && combatEffectWithSecondary.Effects != null && combatEffectWithSecondary.Effects.ShowLastingConditional && combatEffectWithSecondary.Effects.LastingConditional.TryGetChanceDescription(out txt2))
				{
					tooltip.EffectsBlock.AppendLine(txt2, 0);
				}
				tooltip.EffectsBlock.AppendLine("<u><i><b>Secondary</b></i></u>", 0);
				CombatEffectExtensions.FillEffectData(effectSource.ArchetypeId, combatEffectWithSecondary.SecondaryCombatEffect, combatEffectWithSecondary.SecondaryTargetingParams, null, tooltip, entity, false, false, null, null, null);
			}
		}

		// Token: 0x06005EC4 RID: 24260 RVA: 0x0007FBF0 File Offset: 0x0007DDF0
		private static Color GetColor(bool isValid)
		{
			if (!isValid)
			{
				return UIManager.RequirementsNotMetColor;
			}
			return UIManager.RequirementsMetColor;
		}

		// Token: 0x06005EC5 RID: 24261 RVA: 0x001F74B0 File Offset: 0x001F56B0
		private static void FillEffectData(UniqueId archetypeId, CombatEffect effect, TargetingParams targeting, ReagentItem reagentItem, ArchetypeTooltip tooltip, GameEntity entity, bool isPrimary, bool isTriggered, TooltipTextBlock blockOverride = null, UniqueId? sourceArchetypeId = null, byte? stackCount = null)
		{
			if (tooltip == null)
			{
				return;
			}
			if (effect != null && effect.IsLasting && effect.PreserveOnUnconscious)
			{
				tooltip.AddLineToLeftSubHeader("<i>Persists through Unconscious</i>");
			}
			string text = string.Empty;
			if (tooltip.ShowTargeting && targeting != null)
			{
				bool flag = targeting.ShowOnTooltip;
				if (flag && effect != null && effect.Effects != null && effect.Effects.HasTriggerEffects && !effect.Effects.HasInstantVitals && !effect.Effects.HasStatusEffects && !effect.Effects.HasBehaviorEffects)
				{
					flag = false;
				}
				TooltipTextBlock tooltipTextBlock = (blockOverride == null) ? tooltip.EffectsBlock : blockOverride;
				if (targeting.TargetType.IsAOE() || targeting.TargetType.IsChain())
				{
					int num = targeting.AoeMaxTargets;
					if (reagentItem != null)
					{
						num += reagentItem.GetAoeTargetMod();
					}
					string text2 = num.ToString();
					if (reagentItem != null && reagentItem.ColorAoeTargetMod)
					{
						text2 = text2.Color(UIManager.ReagentBonusColor);
					}
					string arg = string.Empty;
					if (targeting.AoeGroupOnly)
					{
						arg = "Group";
					}
					else if (effect != null)
					{
						arg = ((effect.Polarity == Polarity.Positive) ? "Friendly" : "Hostile");
					}
					string arg2 = ZString.Format<int>("{0}m", Mathf.FloorToInt(targeting.GetAoeRadius(entity, null)));
					string arg3 = targeting.TargetType.CheckAngle() ? ZString.Format<int, string>(" & {0}{1}", Mathf.FloorToInt(targeting.GetAoeAngle(entity, null)), "°") : string.Empty;
					text = (targeting.TargetType.IsChain() ? ZString.Format<string, string>("<i>{0} Chained {1} Targets</i>", text2, arg) : ZString.Format<string, string>("<i>{0} {1} Targets</i>", text2, arg));
					if (flag)
					{
						tooltipTextBlock.AppendLine(text, ZString.Format<string, string>("{0}{1}", arg2, arg3));
					}
				}
				else
				{
					text = ZString.Format<string>("<i>{0} Target</i>", targeting.TargetType.GetOffensiveDefensiveDescription());
					string arg4 = targeting.ExcludeSelf ? ZString.Format<string>(" {0}", "<i><size=80%>(Excluding Self)</size></i>") : string.Empty;
					if (flag)
					{
						tooltipTextBlock.AppendLine(ZString.Format<string, string>("{0}{1}", text, arg4), 0);
					}
				}
			}
			if (effect != null && effect.Effects != null)
			{
				TooltipTextBlock tooltipTextBlock = (blockOverride == null) ? tooltip.EffectsBlock : blockOverride;
				string text3 = (effect.Polarity == Polarity.Positive) ? "+" : "-";
				Effects effects = effect.Effects;
				if (tooltip.ShowInstant && effects.HasInstantVitals)
				{
					for (int i = 0; i < effects.Instant.Length; i++)
					{
						if (!effects.Instant[i].HideOnTooltip)
						{
							int num2 = i + 1;
							VitalsEffect_Instant offHandInstant = (num2 < effects.Instant.Length && effects.Instant[num2].ApplyOffHandDamageMultiplier) ? effects.Instant[num2] : null;
							effects.Instant[i].AddInstantToTooltip(tooltipTextBlock, entity, effect.Polarity, ref archetypeId, reagentItem, offHandInstant, sourceArchetypeId, i);
						}
					}
				}
				string txt;
				if (tooltip.ShowConditionals && effects.ShowLastingConditional && effects.LastingConditional.TryGetChanceDescription(out txt))
				{
					tooltipTextBlock.AppendLine(txt, 0);
				}
				if (effect.ThreatScalingType != CombatEffect.ThreatScalingTypes.None)
				{
					string asPercentage = effect.ThreatScalingFraction.GetAsPercentage();
					string arg5;
					string arg6;
					string arg7;
					if (effect.ThreatScalingType == CombatEffect.ThreatScalingTypes.Increase)
					{
						arg5 = "+";
						arg6 = "are <i>NOT</i>";
						arg7 = "highest";
					}
					else
					{
						arg5 = "-";
						arg6 = "<i>ARE</i>";
						arg7 = "next highest";
					}
					if (UIManager.TooltipShowMore)
					{
						string txt2 = ZString.Format<string, string, string, string>("<size=80%><b>T</b>hreat <b>S</b>caling <b>B</b>onus provides additional <b>{0}Threat</b> when you {1} the top threat. Equal to {2}% of the difference between you and the {3} threat.</size>", arg5, arg6, asPercentage, arg7);
						tooltipTextBlock.AppendLine(txt2, 0);
					}
					else
					{
						tooltipTextBlock.AppendLine(ZString.Format<string, string>("{0}{1}% TSB", arg5, asPercentage), 0);
					}
				}
				ThreatParams threat = effect.Threat;
				if (threat != null)
				{
					threat.FillEffectData(tooltip);
				}
				if (effects.HasLasting)
				{
					if (effects.HasOverTimeVitals)
					{
						int num3 = effects.TickRate;
						if (reagentItem)
						{
							num3 += reagentItem.GetTickRateMod();
						}
						string text4 = num3.GetFormattedTime(false);
						if (reagentItem && reagentItem.ColorTickRateMod)
						{
							text4 = text4.Color(UIManager.ReagentBonusColor);
						}
						TooltipExtensions.ToCombine.Clear();
						int j = 0;
						while (j < effects.OverTime.Length)
						{
							VitalsEffect_OverTime vitalsEffect_OverTime = effects.OverTime[j];
							int num4 = vitalsEffect_OverTime.Value;
							if (stackCount != null)
							{
								num4 *= (int)stackCount.Value;
							}
							if (reagentItem)
							{
								num4 += reagentItem.GetOverTimeMod();
							}
							string arg8 = string.Empty;
							string arg9 = string.Empty;
							string text5 = vitalsEffect_OverTime.ResourceType.ToString();
							string arg10 = string.Empty;
							EffectResourceType resourceType = vitalsEffect_OverTime.ResourceType;
							if (resourceType <= EffectResourceType.Stamina)
							{
								if (resourceType != EffectResourceType.Health)
								{
									if (resourceType != EffectResourceType.Stamina)
									{
										goto IL_4F1;
									}
									arg8 = text3;
									arg9 = "%";
								}
								else if (vitalsEffect_OverTime.ValueIsHealthFraction)
								{
									arg8 = text3;
									arg9 = "%";
									text5 = "Health";
								}
								else if (effect.Polarity == Polarity.Negative)
								{
									text5 = "Dmg";
								}
								else
								{
									arg8 = text3;
								}
							}
							else
							{
								if (resourceType == EffectResourceType.Armor)
								{
									goto IL_4F1;
								}
								if (resourceType != EffectResourceType.Threat)
								{
									goto IL_4F1;
								}
							}
							IL_4FC:
							if (vitalsEffect_OverTime.ModifyWounds)
							{
								text5 = ZString.Format<string>(" {0} Wounds", text5);
							}
							string text6 = ZString.Format<string, int, string>("<b>{0}{1}{2}</b>", arg8, num4, arg9);
							if (reagentItem && reagentItem.ColorOverTimeMod)
							{
								text6 = text6.Color(UIManager.ReagentBonusColor);
							}
							string item = ZString.Format<string, string, string>("{0}{1} {2}", text6, arg10, text5);
							TooltipExtensions.ToCombine.Add(item);
							j++;
							continue;
							IL_4F1:
							arg8 = text3;
							arg10 = " to";
							goto IL_4FC;
						}
						tooltipTextBlock.AppendLine(ZString.Format<string, string>("{0} every <b>{1}</b>", string.Join(", ", TooltipExtensions.ToCombine), text4), 0);
					}
					string text7 = string.Empty;
					if (effects.HasBehaviorEffects)
					{
						text7 = ZString.Format<BehaviorEffectTypes>("Applies <b>{0}</b>", effects.BehaviorType);
					}
					string text8 = string.Empty;
					if (effects.HasCombatFlags)
					{
						TooltipExtensions.ToCombine.Clear();
						if (effects.CombatFlags.HasBitFlag(CombatFlags.Advantage))
						{
							TooltipExtensions.ToCombine.Add(UIManager.TooltipShowMore ? CombatFlags.Advantage.ToString() : CombatFlags.Advantage.GetShortTooltipDescription());
						}
						else if (effects.CombatFlags.HasBitFlag(CombatFlags.Disadvantage))
						{
							TooltipExtensions.ToCombine.Add(UIManager.TooltipShowMore ? CombatFlags.Disadvantage.ToString() : CombatFlags.Disadvantage.GetShortTooltipDescription());
						}
						if (effects.CombatFlags.HasBitFlag(CombatFlags.IgnoreActiveDefenses))
						{
							TooltipExtensions.ToCombine.Add(UIManager.TooltipShowMore ? "Ignore Active Defenses" : CombatFlags.IgnoreActiveDefenses.GetShortTooltipDescription());
						}
						text8 = ZString.Format<string>("Provides <b>{0}</b>", string.Join("/", TooltipExtensions.ToCombine));
					}
					bool flag2 = !string.IsNullOrEmpty(text7);
					bool flag3 = !string.IsNullOrEmpty(text8);
					if (flag2 && flag3)
					{
						tooltipTextBlock.AppendLine(ZString.Format<string, string>("{0}, {1}", text7, text8), 0);
					}
					else if (flag2)
					{
						tooltipTextBlock.AppendLine(text7, 0);
					}
					else if (flag3)
					{
						tooltipTextBlock.AppendLine(text8, 0);
					}
					if (effects.HasStatusEffects)
					{
						string description = effects.StatusEffect.GetDescription(effect.Polarity == Polarity.Positive, reagentItem, stackCount);
						tooltipTextBlock.AppendLine(description, 0);
					}
					if (!isTriggered && effects.HasTriggerEffects && effects.TriggeredEffect != null)
					{
						if (!string.IsNullOrEmpty(text))
						{
							tooltipTextBlock.AppendLine(ZString.Format<string>("Applies <i><b>Triggered</b></i> to {0}", text), 0);
						}
						tooltip.SubEffectsBlock.Title = ZString.Format<string, string>("{0} {1}", "Triggered", effect.TriggerParams.GetTriggerDescription());
						CombatEffectExtensions.TriggerChain.Clear();
						int num5 = 0;
						TriggeredParams triggerParams = effect.TriggerParams;
						ScriptableCombatEffect triggeredEffect = effects.TriggeredEffect;
						for (;;)
						{
							CombatEffectExtensions.TriggerChain.Add(new CombatEffectExtensions.TriggerChainParameters(triggerParams, triggeredEffect));
							if (!(triggeredEffect != null) || triggeredEffect.Effect == null || triggeredEffect.Effect.Effects == null || !triggeredEffect.Effect.Effects.HasTriggerEffects || !(triggeredEffect.Effect.Effects.TriggeredEffect != null))
							{
								break;
							}
							triggerParams = triggeredEffect.Effect.TriggerParams;
							triggeredEffect = triggeredEffect.Effect.Effects.TriggeredEffect;
							num5++;
						}
					}
					if (stackCount != null)
					{
						tooltipTextBlock.AppendLine(ZString.Format<byte>("({0} stacks)", stackCount.Value), 0);
					}
					string text9 = string.Empty;
					AuraAbility auraAbility;
					if (InternalGameDatabase.Archetypes.TryGetAsType<AuraAbility>(archetypeId, out auraAbility))
					{
						text9 = "<i>until cancelled</i>";
					}
					else
					{
						int num6 = effect.Expiration.MaxDuration;
						if (reagentItem)
						{
							num6 += reagentItem.GetDurationMod();
						}
						string text10 = num6.GetFormattedTime(false);
						if (reagentItem && reagentItem.ColorDurationMod)
						{
							text10 = text10.Color(UIManager.ReagentBonusColor);
						}
						text9 = ZString.Format<string, string>("{0}<b>{1}</b>", "for ", text10);
						if (effect.Expiration.HasTriggerCount)
						{
							int num7 = effect.Expiration.TriggerCount;
							if (reagentItem)
							{
								num7 += reagentItem.GetTriggerCountMod();
							}
							string text11 = num7.ToString();
							if (reagentItem && reagentItem.ColorTriggerCountMod)
							{
								text11 = text11.Color(UIManager.ReagentBonusColor);
							}
							text9 = ZString.Format<string, string, string>("{0} <i>or</i> <b>{1}x</b> {2}", text9, text11, effect.TriggerParams.GetTriggerDescription());
							string arg11;
							if (UIManager.TooltipShowMore && effect.TriggerParams.TryGetTriggerConditionDescription(out arg11))
							{
								text9 = ZString.Format<string, string, string, string, string, string>("{0}\n<color={1}>{2}</color>{3}({4}){5}", text9, "#00000000", "for ", "<i><size=80%>", arg11, "</size></i>");
							}
						}
					}
					tooltipTextBlock.AppendLine(text9, 0);
					if (!isTriggered && CombatEffectExtensions.TriggerChain.Count > 0)
					{
						for (int k = 0; k < CombatEffectExtensions.TriggerChain.Count; k++)
						{
							TriggeredParams @params = CombatEffectExtensions.TriggerChain[k].Params;
							ScriptableCombatEffect effect2 = CombatEffectExtensions.TriggerChain[k].Effect;
							string text12;
							if (@params.TryGetTriggerConditionDescription(out text12))
							{
								string arg12 = text12.Contains("%") ? " to apply" : string.Empty;
								tooltip.SubEffectsBlock.AppendLine(ZString.Format<string, string, string>("{0} {1}{2}", "<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>", text12, arg12), 0);
							}
							CombatEffectExtensions.FillEffectData(effect2.Id, effect2.Effect, null, reagentItem, tooltip, entity, true, true, tooltip.SubEffectsBlock, new UniqueId?(archetypeId), stackCount);
						}
						CombatEffectExtensions.TriggerChain.Clear();
					}
				}
			}
		}

		// Token: 0x040051E8 RID: 20968
		private const string kSecondaryLine = "<u><i><b>Secondary</b></i></u>";

		// Token: 0x040051E9 RID: 20969
		private static readonly List<CombatEffectExtensions.TriggerChainParameters> TriggerChain = new List<CombatEffectExtensions.TriggerChainParameters>(5);

		// Token: 0x02000C03 RID: 3075
		private struct TriggerChainParameters
		{
			// Token: 0x06005EC7 RID: 24263 RVA: 0x0007FC0D File Offset: 0x0007DE0D
			public TriggerChainParameters(TriggeredParams triggeredParams, ScriptableCombatEffect effect)
			{
				this.Params = triggeredParams;
				this.Effect = effect;
			}

			// Token: 0x040051EA RID: 20970
			public readonly TriggeredParams Params;

			// Token: 0x040051EB RID: 20971
			public readonly ScriptableCombatEffect Effect;
		}
	}
}
