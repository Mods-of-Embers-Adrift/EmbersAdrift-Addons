using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Text;
using SoL.Game.Animation;
using SoL.Game.EffectSystem;
using SoL.Game.Messages;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Settings;
using SoL.Game.Targeting;
using SoL.Game.UI.Chat;
using SoL.Managers;
using SoL.Networking.Managers;
using SoL.Networking.Objects;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Pooling
{
	// Token: 0x020007C9 RID: 1993
	public class CombatTextManager : MonoBehaviour
	{
		// Token: 0x06003A46 RID: 14918
		private int GetIntegerAdjustedDisplayValue(float value)
		{
			return NumberExtensions.ToDisplayInt(Mathf.Abs(value));
		}

		// Token: 0x06003A47 RID: 14919
		private void Start()
		{
			ClientGameManager.CombatTextManager = this;
		}

		// Token: 0x06003A48 RID: 14920
		private void Update()
		{
			for (int i = 0; i < this.m_text.Count; i++)
			{
				PooledCombatText pooledCombatText = this.m_text[i];
				if (pooledCombatText.UpdateExternal())
				{
					pooledCombatText.ReturnToPool();
					this.m_text.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x06003A49 RID: 14921
		public void InitializeCombatText(NetworkEntity entity, EffectApplicationResult ear)
		{
			if (!entity || ear == null)
			{
				return;
			}
			if (ear.HealthAdjustment != null && ear.HealthAdjustment.Value < 0f && !string.IsNullOrEmpty(ear.SourceName))
			{
				UserDpsTracker.RecordDamage(ear.SourceName, -ear.HealthAdjustment.Value);
			}
			this.m_parenthesisOpen = false;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			string text = "?";
			string text2 = "?";
			string text3 = "?";
			bool flag5 = ear.Flags.PlayHitAnimation();
			CombatTextManager.m_eventCache.Reset();
			BaseArchetype baseArchetype = null;
			NetworkEntity networkEntity;
			if (NetworkManager.EntityManager.TryGetNetworkEntity(ear.SourceId, out networkEntity) && networkEntity.GameEntity != null && networkEntity.GameEntity.CharacterData != null)
			{
				text = networkEntity.GameEntity.CharacterData.Name.Value;
				if (networkEntity == LocalPlayer.NetworkEntity)
				{
					flag2 = true;
				}
			}
			else if (!string.IsNullOrEmpty(ear.SourceName))
			{
				text = ear.SourceName;
				flag2 = (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData && ear.SourceName == LocalPlayer.GameEntity.CharacterData.Name.Value);
			}
			NetworkEntity networkEntity2;
			if (NetworkManager.EntityManager.TryGetNetworkEntity(ear.TargetId, out networkEntity2))
			{
				if (networkEntity2 == LocalPlayer.NetworkEntity)
				{
					flag = true;
				}
				if (networkEntity2.GameEntity)
				{
					if (flag && !ear.Flags.HasBitFlag(EffectApplicationFlags.Positive) && networkEntity && networkEntity.GameEntity && networkEntity.GameEntity.Type == GameEntityType.Npc && networkEntity.GameEntity.Targetable != null && networkEntity.GameEntity.Targetable.Faction.GetPlayerTargetType() == TargetType.Offensive && (!ear.Flags.HasBitFlag(EffectApplicationFlags.OverTime) || ear.OverTimeAdjustment != null))
					{
						if (LocalPlayer.Animancer && LocalPlayer.Animancer.Stance == Stance.Sit)
						{
							LocalPlayer.Animancer.SetStance(Stance.Idle);
						}
						if (networkEntity2.GameEntity.TargetController && networkEntity2.GameEntity.TargetController.OffensiveTarget == null && networkEntity.GameEntity.Vitals && networkEntity.GameEntity.Vitals.GetCurrentHealthState() == HealthState.Alive)
						{
							networkEntity2.GameEntity.TargetController.SetTarget(TargetType.Offensive, networkEntity.GameEntity.Targetable);
						}
					}
					if (networkEntity2.GameEntity.CharacterData)
					{
						text2 = networkEntity2.GameEntity.CharacterData.Name.Value;
					}
					if (networkEntity2.GameEntity.AnimancerController != null)
					{
						AnimationEventTriggerType animationEventTriggerType = AnimationEventTriggerType.None;
						if (ear.Flags.HasBitFlag(EffectApplicationFlags.Avoid))
						{
							animationEventTriggerType = AnimationEventTriggerType.Avoid;
						}
						else if (ear.Flags.HasBitFlag(EffectApplicationFlags.Riposte))
						{
							animationEventTriggerType = AnimationEventTriggerType.Riposte;
						}
						else if (ear.Flags.HasBitFlag(EffectApplicationFlags.Parry))
						{
							animationEventTriggerType = AnimationEventTriggerType.Parry;
						}
						else if (ear.Flags.HasBitFlag(EffectApplicationFlags.Block))
						{
							animationEventTriggerType = AnimationEventTriggerType.Block;
						}
						else if (flag5)
						{
							animationEventTriggerType = AnimationEventTriggerType.Hit;
						}
						if (animationEventTriggerType != AnimationEventTriggerType.None)
						{
							CombatTextManager.m_eventCache.HitAnimEvent = new CombatTextManager.AnimEvent?(new CombatTextManager.AnimEvent(networkEntity2.GameEntity, animationEventTriggerType));
						}
					}
					if (flag5 && networkEntity2.GameEntity.AudioEventController)
					{
						string audioEvent = ear.Flags.GetAudioEvent();
						if (!string.IsNullOrEmpty(audioEvent))
						{
							float num = ear.Flags.GetAudioEventVolumeFraction();
							if (!flag2 && !flag)
							{
								num *= 0.5f;
							}
							if (num > 0f)
							{
								CombatTextManager.m_eventCache.WeaponHitEvent = new CombatTextManager.AudioEvent?(new CombatTextManager.AudioEvent(networkEntity2.GameEntity, audioEvent, num));
							}
							if (audioEvent != "Hit")
							{
								num = ((flag2 || flag) ? 1f : 0.5f);
								CombatTextManager.m_eventCache.HitEvent = new CombatTextManager.AudioEvent?(new CombatTextManager.AudioEvent(networkEntity2.GameEntity, "Hit", num));
							}
						}
					}
					if (ClientGameManager.DelayedEventManager)
					{
						float volumeFraction = (flag2 || flag) ? 1f : 0.5f;
						CombatTextManager.AudioEvent? audioEvent2 = null;
						CombatTextManager.AudioEvent? audioEvent3 = null;
						if (ear.Flags.HasBitFlag(EffectApplicationFlags.Riposte))
						{
							audioEvent2 = new CombatTextManager.AudioEvent?(new CombatTextManager.AudioEvent(networkEntity2.GameEntity, "Riposte", volumeFraction));
						}
						else if (ear.Flags.HasBitFlag(EffectApplicationFlags.Parry))
						{
							audioEvent2 = new CombatTextManager.AudioEvent?(new CombatTextManager.AudioEvent(networkEntity2.GameEntity, "Parry", volumeFraction));
						}
						if (ear.Flags.HasBitFlag(EffectApplicationFlags.Block))
						{
							audioEvent3 = new CombatTextManager.AudioEvent?(new CombatTextManager.AudioEvent(networkEntity2.GameEntity, "Block", volumeFraction));
						}
						ClientGameManager.DelayedEventManager.RegisterDelayedDefendEvent(networkEntity2.NetworkId.Value, ref audioEvent2, ref audioEvent3);
					}
				}
			}
			System.Random seededRandom;
			if (ear.Flags.HasBitFlag(EffectApplicationFlags.Killed) && networkEntity && networkEntity.GameEntity && networkEntity.GameEntity.Type == GameEntityType.Npc && networkEntity2 && networkEntity2.GameEntity && networkEntity2.GameEntity.Type == GameEntityType.Player && ear.PlayNpcLevelUpVfx(out seededRandom))
			{
				GlobalSettings.Values.Progression.InitLevelUpVfxForEntity(networkEntity.GameEntity, MessageEventType.LevelUpAdventuring);
				GlobalSettings.Values.Npcs.LevelUpEmote(text, seededRandom);
			}
			if ((flag2 || flag) && (!flag2 || !flag) && ear.Flags.IsValidCombat() && LocalPlayer.GameEntity && LocalPlayer.GameEntity.Vitals)
			{
				LocalPlayer.GameEntity.Vitals.UpdateLastCombatTimestamp();
			}
			if (InternalGameDatabase.Archetypes.TryGetAsType<BaseArchetype>(ear.ArchetypeId, out baseArchetype))
			{
				flag4 = (baseArchetype is AutoAttackAbility || baseArchetype is ScriptableOffHandAutoAttack);
				if (baseArchetype == GlobalSettings.Values.Npcs.UndonesWrathArchetype)
				{
					string content = text + " cannot reach you!";
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content);
				}
				text3 = baseArchetype.DisplayName;
				flag3 = baseArchetype.IsWarlordSong;
				AbilityArchetype abilityArchetype;
				if (baseArchetype.TryGetAsType(out abilityArchetype))
				{
					text3 = abilityArchetype.GetAlchemyDisplayName(ear.AlchemyPowerLevel);
				}
				IVfxSource vfxSource;
				AbilityVFX abilityVFX;
				if (baseArchetype.TryGetAsType(out vfxSource) && vfxSource.TryGetEffects((int)ear.AbilityLevel, ear.AlchemyPowerLevel, ear.IsSecondary, out abilityVFX))
				{
					if (networkEntity && networkEntity.GameEntity && networkEntity.GameEntity.Type == GameEntityType.Npc)
					{
						if (abilityVFX.TargetApplication != null && networkEntity2 && networkEntity2.GameEntity)
						{
							abilityVFX.TargetApplication.GetPooledInstance<PooledVFX>().Initialize(networkEntity2.GameEntity, 5f, networkEntity.GameEntity);
						}
						if (abilityVFX.SourceApplication != null)
						{
							abilityVFX.SourceApplication.GetPooledInstance<PooledVFX>().Initialize(networkEntity.GameEntity, 5f, null);
						}
					}
					if (ear.Flags.HasBitFlag(EffectApplicationFlags.InitialApplication) && abilityVFX.TargetAoeApplication != null && networkEntity2 && networkEntity2.GameEntity)
					{
						abilityVFX.TargetAoeApplication.GetPooledInstance<PooledVFX>().Initialize(networkEntity2.GameEntity, 5f, networkEntity ? networkEntity.GameEntity : null);
					}
				}
			}
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				string flagText = ear.Flags.GetFlagText(flag);
				CombatTextManager.m_adjustmentCount = 0;
				if (ear.HealthAdjustment != null)
				{
					CombatTextManager.m_adjustmentTypes[CombatTextManager.m_adjustmentCount] = "health";
					CombatTextManager.m_adjustmentSuffixes[CombatTextManager.m_adjustmentCount] = string.Empty;
					CombatTextManager.m_adjustmentValues[CombatTextManager.m_adjustmentCount] = this.GetIntegerAdjustedDisplayValue(ear.HealthAdjustment.Value);
					CombatTextManager.m_adjustmentCount++;
				}
				if (ear.HealthWoundAdjustment != null)
				{
					CombatTextManager.m_adjustmentTypes[CombatTextManager.m_adjustmentCount] = "health wound";
					CombatTextManager.m_adjustmentSuffixes[CombatTextManager.m_adjustmentCount] = "%";
					CombatTextManager.m_adjustmentValues[CombatTextManager.m_adjustmentCount] = this.GetIntegerAdjustedDisplayValue(ear.HealthWoundAdjustment.Value);
					CombatTextManager.m_adjustmentCount++;
				}
				if (ear.StaminaAdjustment != null)
				{
					CombatTextManager.m_adjustmentTypes[CombatTextManager.m_adjustmentCount] = "stamina";
					CombatTextManager.m_adjustmentSuffixes[CombatTextManager.m_adjustmentCount] = "%";
					CombatTextManager.m_adjustmentValues[CombatTextManager.m_adjustmentCount] = this.GetIntegerAdjustedDisplayValue(ear.StaminaAdjustment.Value);
					CombatTextManager.m_adjustmentCount++;
				}
				if (ear.StaminaWoundAdjustment != null)
				{
					CombatTextManager.m_adjustmentTypes[CombatTextManager.m_adjustmentCount] = "battle fatigue";
					CombatTextManager.m_adjustmentSuffixes[CombatTextManager.m_adjustmentCount] = "%";
					CombatTextManager.m_adjustmentValues[CombatTextManager.m_adjustmentCount] = this.GetIntegerAdjustedDisplayValue(ear.StaminaWoundAdjustment.Value);
					CombatTextManager.m_adjustmentCount++;
				}
				int resourceAdjustmentValue = (CombatTextManager.m_adjustmentCount > 0) ? CombatTextManager.m_adjustmentValues[0] : 0;
				if (this.ConvertToNormalHit(ear, resourceAdjustmentValue))
				{
					flagText = EffectApplicationFlags.Normal.GetFlagText(flag);
				}
				string text4 = string.Empty;
				if (ear.Flags.HasBitFlag(EffectApplicationFlags.EmberDamage))
				{
					text4 = "Ember Damage";
				}
				else if (ear.Flags.HasBitFlag(EffectApplicationFlags.MeleePhysical) || ear.Flags.HasBitFlag(EffectApplicationFlags.RangedPhysical))
				{
					text4 = "Physical Damage";
				}
				else if (ear.Flags.HasBitFlag(EffectApplicationFlags.MentalDamage))
				{
					text4 = "Mental Damage";
				}
				else if (ear.Flags.HasBitFlag(EffectApplicationFlags.ChemicalDamage))
				{
					text4 = "Chemical Damage";
				}
				string text5 = ear.Flags.HasBitFlag(EffectApplicationFlags.Positive) ? "green" : "red";
				MessageType messageType;
				if (ear.Flags.HasBitFlag(EffectApplicationFlags.Avoid))
				{
					if (flag)
					{
						messageType = MessageType.MyCombatIn;
						utf16ValueStringBuilder.AppendFormat<string, string, string>("You <color=\"green\">{0}</color> <i>{1}'s</i> {2}", flagText, text, text3);
					}
					else if (flag2)
					{
						messageType = MessageType.MyCombatOut;
						utf16ValueStringBuilder.AppendFormat<string, string, string>("<i>{0}</i> <color=\"red\">{1}</color> <color=\"green\"><i>your</i></color> {2}", text2, flagText, text3);
					}
					else
					{
						messageType = MessageType.OtherCombat;
						utf16ValueStringBuilder.AppendFormat<string, string, string, string>("<i>{0}</i> {1} <i>{2}'s</i> {3}", text2, ear.Flags.GetFlagText(false), text, text3);
					}
				}
				else if (flag && flag2)
				{
					messageType = MessageType.MyCombatIn;
					if (this.IsApplicationOnly(ear, resourceAdjustmentValue))
					{
						utf16ValueStringBuilder.AppendFormat<string>("You apply {0} to <color=\"green\"><i>yourself</i></color>", text3);
					}
					else if (ear.Flags.HasBitFlag(EffectApplicationFlags.Positive))
					{
						utf16ValueStringBuilder.AppendFormat<string, string>("Your {0} {1}restores ", text3, ear.Flags.GetPositiveFlagVerbiage());
						for (int i = 0; i < CombatTextManager.m_adjustmentCount; i++)
						{
							if (i > 0)
							{
								utf16ValueStringBuilder.Append(" and ");
							}
							utf16ValueStringBuilder.AppendFormat<int, string, string>("{0}{1} of <color=\"green\"><i>your</i></color> {2}", CombatTextManager.m_adjustmentValues[i], CombatTextManager.m_adjustmentSuffixes[i], CombatTextManager.m_adjustmentTypes[i]);
						}
					}
					else
					{
						utf16ValueStringBuilder.AppendFormat<string, string, string>("{0} {1} <color=\"{2}\"><i>you</i></color> for ", text3, flagText, text5);
						for (int j = 0; j < CombatTextManager.m_adjustmentCount; j++)
						{
							if (j > 0)
							{
								utf16ValueStringBuilder.Append(" and ");
							}
							if (string.IsNullOrEmpty(text4))
							{
								utf16ValueStringBuilder.AppendFormat<int, string, string>("{0}{1} {2}", CombatTextManager.m_adjustmentValues[j], CombatTextManager.m_adjustmentSuffixes[j], CombatTextManager.m_adjustmentTypes[j]);
							}
							else
							{
								utf16ValueStringBuilder.AppendFormat<int, string, string, string>("<link=\"text:{3}\">{0}{1} {2}</link>", CombatTextManager.m_adjustmentValues[j], CombatTextManager.m_adjustmentSuffixes[j], CombatTextManager.m_adjustmentTypes[j], text4);
							}
						}
					}
				}
				else if (flag)
				{
					messageType = MessageType.MyCombatIn;
					if (this.IsLevelResisted(ear) || this.IsFullyResisted(ear))
					{
						utf16ValueStringBuilder.AppendFormat<string, string, string>("<i>{0}'s</i> {1} is RESISTED by <color=\"{2}\"><i>you</i></color>", text, text3, text5);
					}
					else if (this.IsApplicationOnly(ear, resourceAdjustmentValue))
					{
						utf16ValueStringBuilder.AppendFormat<string, string, string, string>("<i>{0}'s</i> {1} is {2} to <color=\"{3}\"><i>you</i></color>", text, text3, flagText, text5);
					}
					else if (ear.Flags.HasBitFlag(EffectApplicationFlags.Positive))
					{
						utf16ValueStringBuilder.AppendFormat<string, string, string>("<i>{0}'s</i> {1} {2}restores ", text, text3, ear.Flags.GetPositiveFlagVerbiage());
						for (int k = 0; k < CombatTextManager.m_adjustmentCount; k++)
						{
							if (k > 0)
							{
								utf16ValueStringBuilder.Append(" and ");
							}
							utf16ValueStringBuilder.AppendFormat<int, string, string>("{0}{1} of <color=\"green\"><i>your</i></color> {2}", CombatTextManager.m_adjustmentValues[k], CombatTextManager.m_adjustmentSuffixes[k], CombatTextManager.m_adjustmentTypes[k]);
						}
					}
					else
					{
						utf16ValueStringBuilder.AppendFormat<string, string, string, string>("<i>{0}'s</i> {1} {2} <color=\"{3}\"><i>you</i></color> for ", text, text3, flagText, text5);
						for (int l = 0; l < CombatTextManager.m_adjustmentCount; l++)
						{
							if (l > 0)
							{
								utf16ValueStringBuilder.Append(" and ");
							}
							if (string.IsNullOrEmpty(text4))
							{
								utf16ValueStringBuilder.AppendFormat<int, string, string>("{0}{1} {2}", CombatTextManager.m_adjustmentValues[l], CombatTextManager.m_adjustmentSuffixes[l], CombatTextManager.m_adjustmentTypes[l]);
							}
							else
							{
								utf16ValueStringBuilder.AppendFormat<int, string, string, string>("<link=\"text:{3}\">{0}{1} {2}</link>", CombatTextManager.m_adjustmentValues[l], CombatTextManager.m_adjustmentSuffixes[l], CombatTextManager.m_adjustmentTypes[l], text4);
							}
						}
					}
				}
				else if (flag2)
				{
					messageType = MessageType.MyCombatOut;
					if (this.IsLevelResisted(ear) || this.IsFullyResisted(ear))
					{
						utf16ValueStringBuilder.AppendFormat<string, string, string>("<color=\"{0}\"><i>Your</i></color> {1} is RESISTED by <i>{2}</i>", text5, text3, text2);
					}
					else if (this.IsApplicationOnly(ear, resourceAdjustmentValue))
					{
						utf16ValueStringBuilder.AppendFormat<string, string, string, string>("<color=\"{0}\"><i>Your</i></color> {1} is {2} to <i>{3}</i>", text5, text3, flagText, text2);
					}
					else if (ear.Flags.HasBitFlag(EffectApplicationFlags.Positive))
					{
						if (CombatTextManager.m_adjustmentCount == 1)
						{
							utf16ValueStringBuilder.AppendFormat<string, string, string, int, string, string, string>("<color=\"{0}\"><i>Your</i></color> {1} {2}restores {3}{4} of {5}'s {6}", text5, text3, ear.Flags.GetPositiveFlagVerbiage(), CombatTextManager.m_adjustmentValues[0], CombatTextManager.m_adjustmentSuffixes[0], text2, CombatTextManager.m_adjustmentTypes[0]);
						}
						else
						{
							utf16ValueStringBuilder.AppendFormat<string, string, string>("<color=\"{0}\"><i>Your</i></color> {1} {2}restores ", text5, text3, ear.Flags.GetPositiveFlagVerbiage());
							for (int m = 0; m < CombatTextManager.m_adjustmentCount; m++)
							{
								if (m > 0)
								{
									utf16ValueStringBuilder.Append(" and ");
								}
								utf16ValueStringBuilder.AppendFormat<int, string, string>("{0}{1} {2}", CombatTextManager.m_adjustmentValues[m], CombatTextManager.m_adjustmentSuffixes[m], CombatTextManager.m_adjustmentTypes[m]);
							}
							utf16ValueStringBuilder.AppendFormat<string>(" for {0}", text2);
						}
					}
					else
					{
						utf16ValueStringBuilder.AppendFormat<string, string, string, string>("<color=\"{0}\"><i>Your</i></color> {1} {2} <i>{3}</i> for ", text5, text3, flagText, text2);
						for (int n = 0; n < CombatTextManager.m_adjustmentCount; n++)
						{
							if (n > 0)
							{
								utf16ValueStringBuilder.Append(" and ");
							}
							if (string.IsNullOrEmpty(text4))
							{
								utf16ValueStringBuilder.AppendFormat<int, string, string>("{0}{1} {2}", CombatTextManager.m_adjustmentValues[n], CombatTextManager.m_adjustmentSuffixes[n], CombatTextManager.m_adjustmentTypes[n]);
							}
							else
							{
								utf16ValueStringBuilder.AppendFormat<int, string, string, string>("<link=\"text:{3}\">{0}{1} {2}</link>", CombatTextManager.m_adjustmentValues[n], CombatTextManager.m_adjustmentSuffixes[n], CombatTextManager.m_adjustmentTypes[n], text4);
							}
						}
					}
				}
				else
				{
					messageType = MessageType.OtherCombat;
					if (this.IsLevelResisted(ear) || this.IsFullyResisted(ear))
					{
						utf16ValueStringBuilder.AppendFormat<string, string, string>("<i>{0}'s</i> {1} is RESISTED by <i>{2}</i>", text, text3, text2);
					}
					else if (this.IsApplicationOnly(ear, resourceAdjustmentValue))
					{
						utf16ValueStringBuilder.AppendFormat<string, string, string, string>("<i>{0}'s</i> {1} is {2} to <i>{3}</i>", text, text3, flagText, text2);
					}
					else if (ear.Flags.HasBitFlag(EffectApplicationFlags.Positive))
					{
						if (CombatTextManager.m_adjustmentCount == 1)
						{
							utf16ValueStringBuilder.AppendFormat<string, string, string, int, string, string, string>("<i>{0}'s</i> {1} {2}restores {3}{4} of {5}'s {6}", text, text3, ear.Flags.GetPositiveFlagVerbiage(), CombatTextManager.m_adjustmentValues[0], CombatTextManager.m_adjustmentSuffixes[0], text2, CombatTextManager.m_adjustmentTypes[0]);
						}
						else
						{
							utf16ValueStringBuilder.AppendFormat<string, string, string>("<i>{0}'s</i> {1} {2}restores ", text, text3, ear.Flags.GetPositiveFlagVerbiage());
							for (int num2 = 0; num2 < CombatTextManager.m_adjustmentCount; num2++)
							{
								if (num2 > 0)
								{
									utf16ValueStringBuilder.Append(" and ");
								}
								utf16ValueStringBuilder.AppendFormat<int, string, string>("{0}{1} {2}", CombatTextManager.m_adjustmentValues[num2], CombatTextManager.m_adjustmentSuffixes[num2], CombatTextManager.m_adjustmentTypes[num2]);
							}
							utf16ValueStringBuilder.AppendFormat<string>(" for {0}", text2);
						}
					}
					else
					{
						utf16ValueStringBuilder.AppendFormat<string, string, string, string>("<i>{0}'s</i> {1} {2} <i>{3}</i> for ", text, text3, flagText, text2);
						for (int num3 = 0; num3 < CombatTextManager.m_adjustmentCount; num3++)
						{
							if (num3 > 0)
							{
								utf16ValueStringBuilder.Append(" and ");
							}
							if (string.IsNullOrEmpty(text4))
							{
								utf16ValueStringBuilder.AppendFormat<int, string, string>("{0}{1} {2}", CombatTextManager.m_adjustmentValues[num3], CombatTextManager.m_adjustmentSuffixes[num3], CombatTextManager.m_adjustmentTypes[num3]);
							}
							else
							{
								utf16ValueStringBuilder.AppendFormat<int, string, string, string>("<link=\"text:{3}\">{0}{1} {2}</link>", CombatTextManager.m_adjustmentValues[num3], CombatTextManager.m_adjustmentSuffixes[num3], CombatTextManager.m_adjustmentTypes[num3], text4);
							}
						}
					}
				}
				if (flag3)
				{
					messageType |= MessageType.WarlordSong;
				}
				if (ear.Defended != null)
				{
					bool flag6 = ear.Flags.HasBitFlag(EffectApplicationFlags.Block);
					bool flag7 = ear.Flags.HasBitFlag(EffectApplicationFlags.Parry);
					int arg = NumberExtensions.ToDisplayInt(ear.Defended.Value);
					if (flag6 && flag7)
					{
						utf16ValueStringBuilder.Append(this.GetParenthesisText());
						utf16ValueStringBuilder.AppendFormat<int, string>("{0} <link=\"{1}:This attack was partially blocked and parried\"><b><color=\"green\">Defended</color></b></link>", arg, "text");
					}
					else if (flag7)
					{
						utf16ValueStringBuilder.Append(this.GetParenthesisText());
						utf16ValueStringBuilder.AppendFormat<int, string>("{0} <link=\"{1}:This attack was partially blocked by a parry\"><b><color=\"green\">Parried</color></b></link>", arg, "text");
					}
					else if (flag6)
					{
						utf16ValueStringBuilder.Append(this.GetParenthesisText());
						utf16ValueStringBuilder.AppendFormat<int, string>("{0} <link=\"{1}:This attack was partially blocked\"><b><color=\"green\">Blocked</color></b></link>", arg, "text");
					}
					if (flag7 && ear.Flags.HasBitFlag(EffectApplicationFlags.Riposte))
					{
						utf16ValueStringBuilder.Append(this.GetParenthesisText());
						utf16ValueStringBuilder.AppendFormat<string>("<link=\"{0}:Riposte!\"><color=\"green\">RIP</color></link>", "text");
					}
				}
				bool flag8 = ear.Flags.HasBitFlag(EffectApplicationFlags.Advantage);
				bool flag9 = ear.Flags.HasBitFlag(EffectApplicationFlags.Disadvantage);
				if (flag8 && flag9)
				{
					utf16ValueStringBuilder.Append(this.GetParenthesisText());
					utf16ValueStringBuilder.AppendFormat<string>("<link=\"{0}:This attack had an Advantage but was cancelled by a Disadvantage\"><s>ADV</s></link>", "text");
				}
				else if (flag8)
				{
					utf16ValueStringBuilder.Append(this.GetParenthesisText());
					utf16ValueStringBuilder.AppendFormat<string>("<link=\"{0}:This attack had an Advantage\">ADV</link>", "text");
				}
				else if (flag9)
				{
					utf16ValueStringBuilder.Append(this.GetParenthesisText());
					utf16ValueStringBuilder.AppendFormat<string>("<link=\"{0}:This attack had a Disadvantage\">DIS</link>", "text");
				}
				if (ear.Flags.HasBitFlag(EffectApplicationFlags.Resist))
				{
					utf16ValueStringBuilder.Append(this.GetParenthesisText());
					utf16ValueStringBuilder.AppendFormat<string>("<link=\"{0}:Resisted\">RES</link>", "text");
				}
				else if (ear.Flags.HasBitFlag(EffectApplicationFlags.PartialResist))
				{
					utf16ValueStringBuilder.Append(this.GetParenthesisText());
					utf16ValueStringBuilder.AppendFormat<string>("<link=\"{0}:Partially Resisted\">PRES</link>", "text");
				}
				if (ear.Flags.HasBitFlag(EffectApplicationFlags.Diminished))
				{
					utf16ValueStringBuilder.Append(this.GetParenthesisText());
					utf16ValueStringBuilder.AppendFormat<string>("<link=\"{0}:Diminished\">DIM</link>", "text");
				}
				if (ear.Absorbed != null && ear.Absorbed.Value > 0f)
				{
					int value = NumberExtensions.ToDisplayInt(ear.Absorbed.Value);
					utf16ValueStringBuilder.Append(this.GetParenthesisText());
					utf16ValueStringBuilder.Append(value);
					utf16ValueStringBuilder.AppendFormat<string>(" <link=\"{0}:Absorbed by armor\">ABS</link>", "text");
				}
				if (ear.Threat != null && (ear.Threat.Value < 0f || ear.Threat.Value > 0f))
				{
					int value2 = NumberExtensions.ToDisplayInt(ear.Threat.Value);
					utf16ValueStringBuilder.Append(this.GetParenthesisText());
					utf16ValueStringBuilder.Append(value2);
					utf16ValueStringBuilder.AppendFormat<string>(" <link=\"{0}:Threat\">THR</link>", "text");
				}
				if (ear.Flags.HasBitFlag(EffectApplicationFlags.Applied) && ear.Flags.HasBitFlag(EffectApplicationFlags.OverTime) && ear.OverTimeAdjustment != null)
				{
					int arg2 = Mathf.Abs(ear.OverTimeAdjustment.Value);
					utf16ValueStringBuilder.Append(this.GetParenthesisText());
					utf16ValueStringBuilder.AppendFormat<int, string>("{0} <link=\"{1}:Applied via over time effect\">AOT</link>", arg2, "text");
				}
				if (this.m_parenthesisOpen)
				{
					utf16ValueStringBuilder.Append(")</size>");
					this.m_parenthesisOpen = false;
				}
				if (ear.Flags.HasBitFlag(EffectApplicationFlags.Killed))
				{
					utf16ValueStringBuilder.Append(" <color=\"red\">KILLING BLOW</color>!");
				}
				if (messageType == MessageType.OtherCombat)
				{
					utf16ValueStringBuilder.Insert(0, "<size=80%>");
					utf16ValueStringBuilder.Append("</size>");
				}
				if (flag4 && (CombatTextManager.m_cachedAutoAttackColorPrefix || ChatListItem.DefaultChatColor != null))
				{
					if (!CombatTextManager.m_cachedAutoAttackColorPrefix)
					{
						Color color = ChatListItem.DefaultChatColor.Value * GlobalSettings.Values.Chat.AutoAttackColorMultiplier;
						color.a = 1f;
						CombatTextManager.m_autoAttackColorPrefix = ZString.Format<string>("<color={0}>", color.ToHex());
					}
					utf16ValueStringBuilder.Insert(0, CombatTextManager.m_autoAttackColorPrefix);
					utf16ValueStringBuilder.Append("</color>");
				}
				MessageManager.CombatQueue.AddToQueue(messageType, utf16ValueStringBuilder.ToString());
			}
			uint? delayedKey = null;
			if (networkEntity)
			{
				delayedKey = new uint?(networkEntity.NetworkId.Value);
			}
			if (flag2 || flag)
			{
				this.InitializeOverheadCombatTextInternal(entity, ear, baseArchetype, delayedKey);
			}
			if (ear.TriggerOnAnimEvent && delayedKey != null && ClientGameManager.DelayedEventManager)
			{
				ClientGameManager.DelayedEventManager.RegisterDelayedEvent(delayedKey.Value, ref CombatTextManager.m_eventCache.WeaponHitEvent, ref CombatTextManager.m_eventCache.HitEvent, ref CombatTextManager.m_eventCache.HitAnimEvent, ref CombatTextManager.m_eventCache.CombatText);
			}
			else
			{
				bool flag10 = false;
				if (CombatTextManager.m_eventCache.HitAnimEvent != null && CombatTextManager.m_eventCache.HitAnimEvent.Value.Entity && CombatTextManager.m_eventCache.HitAnimEvent.Value.Entity.AnimancerController != null)
				{
					bool flag11 = CombatTextManager.m_eventCache.HitAnimEvent.Value.Entity.AnimancerController.TriggerEvent(CombatTextManager.m_eventCache.HitAnimEvent.Value.TriggerType);
					if (CombatTextManager.m_eventCache.HitAnimEvent.Value.TriggerType == AnimationEventTriggerType.Hit)
					{
						flag10 = flag11;
					}
				}
				if (CombatTextManager.m_eventCache.WeaponHitEvent != null && CombatTextManager.m_eventCache.WeaponHitEvent.Value.Entity && CombatTextManager.m_eventCache.WeaponHitEvent.Value.Entity.AudioEventController)
				{
					CombatTextManager.m_eventCache.WeaponHitEvent.Value.Entity.AudioEventController.PlayAudioEvent(CombatTextManager.m_eventCache.WeaponHitEvent.Value.EventName, CombatTextManager.m_eventCache.WeaponHitEvent.Value.VolumeFraction);
				}
				if (CombatTextManager.m_eventCache.HitEvent != null && CombatTextManager.m_eventCache.HitEvent.Value.Entity && CombatTextManager.m_eventCache.HitEvent.Value.Entity.AudioEventController && (flag10 || UnityEngine.Random.Range(0f, 1f) <= GlobalSettings.Values.Audio.TriggerHitAudioEventOnHitChance))
				{
					CombatTextManager.m_eventCache.HitEvent.Value.Entity.AudioEventController.PlayAudioEvent(CombatTextManager.m_eventCache.HitEvent.Value.EventName, CombatTextManager.m_eventCache.HitEvent.Value.VolumeFraction);
				}
			}
			StaticPool<EffectApplicationResult>.ReturnToPool(ear);
		}

		// Token: 0x06003A4A RID: 14922
		private bool IsApplicationOnly(EffectApplicationResult ear, int resourceAdjustmentValue)
		{
			return !ear.Flags.HasAnyBitFlag(EffectApplicationFlags.Miss | EffectApplicationFlags.Glancing | EffectApplicationFlags.Normal | EffectApplicationFlags.Heavy | EffectApplicationFlags.Critical) && resourceAdjustmentValue == 0;
		}

		// Token: 0x06003A4B RID: 14923
		private bool IsLevelResisted(EffectApplicationResult ear)
		{
			return ear.Flags.HasBitFlag(EffectApplicationFlags.Resist) && ear.Flags.HasBitFlag(EffectApplicationFlags.Disadvantage);
		}

		// Token: 0x06003A4C RID: 14924
		private bool IsFullyResisted(EffectApplicationResult ear)
		{
			return (ear.Flags & ~EffectApplicationFlags.PartialResist & ~EffectApplicationFlags.InitialApplication & ~EffectApplicationFlags.Diminished) == EffectApplicationFlags.Resist;
		}

		// Token: 0x06003A4D RID: 14925
		private bool ConvertToNormalHit(EffectApplicationResult ear, int resourceAdjustmentValue)
		{
			return !ear.Flags.HasAnyBitFlag(EffectApplicationFlags.Miss | EffectApplicationFlags.Glancing | EffectApplicationFlags.Normal | EffectApplicationFlags.Heavy | EffectApplicationFlags.Critical) && resourceAdjustmentValue != 0;
		}

		// Token: 0x06003A4E RID: 14926
		private void PlayAudioEvent(GameEntity sourceEntity, string eventName, float volumeFraction, bool isSource, bool isTarget)
		{
			if (sourceEntity == null || sourceEntity.AudioEventController == null || string.IsNullOrEmpty(eventName))
			{
				return;
			}
			if (isSource || isTarget)
			{
				volumeFraction *= 0.5f;
			}
			if (volumeFraction > 0f)
			{
				sourceEntity.AudioEventController.PlayAudioEvent(eventName, volumeFraction);
			}
		}

		// Token: 0x06003A4F RID: 14927
		private bool TryGetThreatDeltaValue(float? threat, float resourceAdjustment, out int threatValue)
		{
			threatValue = 0;
			if (threat != null)
			{
				if (NumberExtensions.OppositeSigns(threat.Value, resourceAdjustment))
				{
					threatValue = Mathf.FloorToInt(threat.Value);
					return true;
				}
				if (threat.Value != resourceAdjustment)
				{
					float f = resourceAdjustment - threat.Value;
					threatValue = Mathf.FloorToInt(f);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003A50 RID: 14928
		private string GetParenthesisText()
		{
			if (this.m_parenthesisOpen)
			{
				return ", ";
			}
			this.m_parenthesisOpen = true;
			return " <size=70%>(";
		}

		// Token: 0x06003A51 RID: 14929
		private void InitializeOverheadCombatTextInternal(NetworkEntity entity, EffectApplicationResult ear, BaseArchetype archetype, uint? delayedKey)
		{
			if (UIManager.UiHidden || !Options.GameOptions.ShowOverheadCombatText.Value)
			{
				return;
			}
			bool flag = ear.SourceId == LocalPlayer.NetworkEntity.NetworkId.Value;
			bool flag2 = ear.TargetId == LocalPlayer.NetworkEntity.NetworkId.Value;
			if (!flag && !flag2)
			{
				return;
			}
			bool flag3 = false;
			bool flag4 = false;
			float f = 0f;
			if (ear.HealthAdjustment != null)
			{
				f = ear.HealthAdjustment.Value;
			}
			else if (ear.HealthWoundAdjustment != null)
			{
				f = ear.HealthWoundAdjustment.Value;
				flag3 = true;
			}
			else if (ear.StaminaAdjustment != null)
			{
				f = ear.StaminaAdjustment.Value;
				flag4 = true;
			}
			else if (ear.StaminaWoundAdjustment != null)
			{
				f = ear.StaminaWoundAdjustment.Value;
				flag4 = true;
				flag3 = true;
			}
			int num = NumberExtensions.ToDisplayInt(Mathf.Abs(f));
			if (num == 0)
			{
				return;
			}
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				bool flag5 = ear.Flags.HasBitFlag(EffectApplicationFlags.Positive);
				string arg = flag5 ? "+" : "-";
				OverheadType type;
				if (flag)
				{
					type = (flag5 ? OverheadType.OverheadSelfPositive : OverheadType.OverheadSelfNegative);
				}
				else
				{
					type = (flag5 ? OverheadType.OverheadTargetPositive : OverheadType.OverheadTargetNegative);
				}
				string arg2 = type.GetColor().ToHex();
				utf16ValueStringBuilder.AppendFormat<string, string, int>("<b><color={0}>{1}{2}", arg2, arg, num);
				if (flag3)
				{
					utf16ValueStringBuilder.Append("%");
				}
				if (flag4)
				{
					utf16ValueStringBuilder.Append(" STA");
				}
				utf16ValueStringBuilder.Append("</color></b>");
				bool flag6 = false;
				if (ear.Absorbed != null && ear.Absorbed.Value > 0f)
				{
					int num2 = Mathf.FloorToInt(ear.Absorbed.Value);
					if (num2 != 0)
					{
						if (!flag6)
						{
							utf16ValueStringBuilder.Append("<size=50%><color=#008080>(");
							flag6 = true;
						}
						utf16ValueStringBuilder.AppendFormat<int>(" {0} ABS", num2);
					}
				}
				if (flag6)
				{
					utf16ValueStringBuilder.Append(")</color></size>");
				}
				string overheadCombatText = ear.Flags.GetOverheadCombatText();
				if (!string.IsNullOrEmpty(overheadCombatText))
				{
					utf16ValueStringBuilder.AppendFormat<string, string>("\n<size=60%><i><color={0}>{1}</color></i></size>", arg2, overheadCombatText);
				}
				Sprite icon = (archetype != null && !(archetype is AutoAttackAbility)) ? archetype.Icon : null;
				if (ear.TriggerOnAnimEvent && delayedKey != null && ClientGameManager.DelayedEventManager)
				{
					CombatTextManager.m_eventCache.CombatText = new CombatTextManager.CombatTextEvent?(new CombatTextManager.CombatTextEvent(entity.GameEntity, utf16ValueStringBuilder.ToString(), Color.white, icon));
				}
				else
				{
					this.InitializeOverheadCombatText(utf16ValueStringBuilder.ToString(), entity.GameEntity, Color.white, icon);
				}
			}
		}

		// Token: 0x06003A52 RID: 14930
		public void InitializeOverheadCombatText(string txt, GameEntity entity, Color color, Sprite icon)
		{
			if (UIManager.UiHidden)
			{
				return;
			}
			Vector3 pos = (entity.NameplateHeightOffset != null) ? entity.NameplateHeightOffset.Value : WorldSpaceOverheadController.kDefaultHeightOffset;
			this.InitializeOverheadCombatText(txt, entity.gameObject.transform, pos, Quaternion.identity, color, icon);
		}

		// Token: 0x06003A53 RID: 14931
		public void InitializeOverheadCombatText(string txt, Transform parent, Vector3 pos, Quaternion rot, Color color, Sprite icon)
		{
			if (UIManager.UiHidden)
			{
				return;
			}
			PooledCombatText pooledInstance = this.m_prefab.GetPooledInstance<PooledCombatText>();
			pooledInstance.Init(txt, parent, pos, rot, color, icon);
			this.m_text.Add(pooledInstance);
		}

		// Token: 0x06003A54 RID: 14932
		[ContextMenu("Test some stuff")]
		private void Test()
		{
			this.InitializeOverheadCombatText("Testing", LocalPlayer.GameEntity.transform, Vector3.zero, Quaternion.identity, Color.white, null);
		}

		// Token: 0x040038AC RID: 14508
		private const string kSpacerText = ", ";

		// Token: 0x040038AD RID: 14509
		[SerializeField]
		private PooledCombatText m_prefab;

		// Token: 0x040038AE RID: 14510
		private readonly List<PooledCombatText> m_text = new List<PooledCombatText>();

		// Token: 0x040038AF RID: 14511
		private readonly StringBuilder m_sb = new StringBuilder();

		// Token: 0x040038B0 RID: 14512
		private bool m_parenthesisOpen;

		// Token: 0x040038B1 RID: 14513
		private const int kMaxResults = 4;

		// Token: 0x040038B2 RID: 14514
		private static int m_adjustmentCount = 1;

		// Token: 0x040038B3 RID: 14515
		private static readonly string[] m_adjustmentTypes = new string[4];

		// Token: 0x040038B4 RID: 14516
		private static readonly string[] m_adjustmentSuffixes = new string[4];

		// Token: 0x040038B5 RID: 14517
		private static readonly int[] m_adjustmentValues = new int[4];

		// Token: 0x040038B6 RID: 14518
		private const string kAndText = " and ";

		// Token: 0x040038B7 RID: 14519
		private const string kDamageValueFormat = "{0}{1} {2}";

		// Token: 0x040038B8 RID: 14520
		private const string kDamageValueWithTooltip = "<link=\"text:{3}\">{0}{1} {2}</link>";

		// Token: 0x040038B9 RID: 14521
		private static readonly CombatTextManager.EventCache m_eventCache = new CombatTextManager.EventCache();

		// Token: 0x040038BA RID: 14522
		private static bool m_cachedAutoAttackColorPrefix = false;

		// Token: 0x040038BB RID: 14523
		private static string m_autoAttackColorPrefix = null;

		// Token: 0x040038BC RID: 14524
		private const EffectApplicationFlags kHitFlags = EffectApplicationFlags.Miss | EffectApplicationFlags.Glancing | EffectApplicationFlags.Normal | EffectApplicationFlags.Heavy | EffectApplicationFlags.Critical;

		// Token: 0x020007CA RID: 1994
		public struct AudioEvent
		{
			// Token: 0x06003A57 RID: 14935
			public AudioEvent(GameEntity entity, string eventName, float volumeFraction)
			{
				this.Entity = entity;
				this.EventName = eventName;
				this.VolumeFraction = volumeFraction;
			}

			// Token: 0x040038BD RID: 14525
			public readonly GameEntity Entity;

			// Token: 0x040038BE RID: 14526
			public readonly string EventName;

			// Token: 0x040038BF RID: 14527
			public readonly float VolumeFraction;
		}

		// Token: 0x020007CB RID: 1995
		public struct AnimEvent
		{
			// Token: 0x06003A58 RID: 14936
			public AnimEvent(GameEntity entity, AnimationEventTriggerType triggerType)
			{
				this.Entity = entity;
				this.TriggerType = triggerType;
			}

			// Token: 0x040038C0 RID: 14528
			public readonly GameEntity Entity;

			// Token: 0x040038C1 RID: 14529
			public readonly AnimationEventTriggerType TriggerType;
		}

		// Token: 0x020007CC RID: 1996
		public struct CombatTextEvent
		{
			// Token: 0x06003A59 RID: 14937
			public CombatTextEvent(GameEntity entity, string text, Color color, Sprite icon)
			{
				this.Entity = entity;
				this.Text = text;
				this.Color = color;
				this.Icon = icon;
			}

			// Token: 0x040038C2 RID: 14530
			public readonly GameEntity Entity;

			// Token: 0x040038C3 RID: 14531
			public readonly string Text;

			// Token: 0x040038C4 RID: 14532
			public readonly Color Color;

			// Token: 0x040038C5 RID: 14533
			public readonly Sprite Icon;
		}

		// Token: 0x020007CD RID: 1997
		private class EventCache
		{
			// Token: 0x06003A5A RID: 14938
			public void Reset()
			{
				this.WeaponHitEvent = null;
				this.HitEvent = null;
				this.HitAnimEvent = null;
				this.CombatText = null;
			}

			// Token: 0x040038C6 RID: 14534
			public CombatTextManager.AudioEvent? WeaponHitEvent;

			// Token: 0x040038C7 RID: 14535
			public CombatTextManager.AudioEvent? HitEvent;

			// Token: 0x040038C8 RID: 14536
			public CombatTextManager.AnimEvent? HitAnimEvent;

			// Token: 0x040038C9 RID: 14537
			public CombatTextManager.CombatTextEvent? CombatText;
		}
	}
}
