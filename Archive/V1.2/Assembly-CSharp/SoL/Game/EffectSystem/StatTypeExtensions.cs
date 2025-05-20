using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Settings;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C4C RID: 3148
	public static class StatTypeExtensions
	{
		// Token: 0x1700174B RID: 5963
		// (get) Token: 0x060060EF RID: 24815 RVA: 0x00081421 File Offset: 0x0007F621
		public static StatType[] StatTypes
		{
			get
			{
				if (StatTypeExtensions.m_statTypes == null)
				{
					StatTypeExtensions.m_statTypes = (StatType[])Enum.GetValues(typeof(StatType));
				}
				return StatTypeExtensions.m_statTypes;
			}
		}

		// Token: 0x1700174C RID: 5964
		// (get) Token: 0x060060F0 RID: 24816 RVA: 0x00081448 File Offset: 0x0007F648
		public static StatPanel[] StatPanels
		{
			get
			{
				if (StatTypeExtensions.m_statPanels == null)
				{
					StatTypeExtensions.m_statPanels = (StatPanel[])Enum.GetValues(typeof(StatPanel));
				}
				return StatTypeExtensions.m_statPanels;
			}
		}

		// Token: 0x060060F1 RID: 24817 RVA: 0x0008146F File Offset: 0x0007F66F
		public static Dictionary<string, List<StatType>> GetCategories(this StatPanel panelType)
		{
			switch (panelType)
			{
			case StatPanel.STATS:
				return StatTypeExtensions.m_statPanelCategories;
			case StatPanel.COMBAT:
				return StatTypeExtensions.m_combatPanelCategories;
			case StatPanel.RESISTS:
				return StatTypeExtensions.m_resistPanelCategories;
			default:
				throw new ArgumentException("panelType");
			}
		}

		// Token: 0x1700174D RID: 5965
		// (get) Token: 0x060060F2 RID: 24818 RVA: 0x000814A1 File Offset: 0x0007F6A1
		public static StatType[] ValidDamageTypes
		{
			get
			{
				return StatTypeExtensions.m_validDamageTypes;
			}
		}

		// Token: 0x1700174E RID: 5966
		// (get) Token: 0x060060F3 RID: 24819 RVA: 0x000814A8 File Offset: 0x0007F6A8
		public static StatType[] ValidDebuffTypes
		{
			get
			{
				return StatTypeExtensions.m_validDebuffTypes;
			}
		}

		// Token: 0x060060F4 RID: 24820 RVA: 0x000814AF File Offset: 0x0007F6AF
		public static bool HasResists(this StatType type)
		{
			return type - StatType.Damage1H <= 5;
		}

		// Token: 0x060060F5 RID: 24821 RVA: 0x000814BB File Offset: 0x0007F6BB
		public static StatType GetDamageResist(this StatType type)
		{
			switch (type)
			{
			case StatType.Damage1H:
			case StatType.Damage2H:
			case StatType.DamageRanged:
				return StatType.ResistDamagePhysical;
			case StatType.DamageMental:
				return StatType.ResistDamageMental;
			case StatType.DamageChemical:
				return StatType.ResistDamageChemical;
			case StatType.DamageEmber:
				return StatType.ResistDamageEmber;
			default:
				return StatType.None;
			}
		}

		// Token: 0x060060F6 RID: 24822 RVA: 0x000814ED File Offset: 0x0007F6ED
		public static StatType GetDamageReductionType(this StatType type)
		{
			if (type - StatType.Damage1H <= 4)
			{
				return StatType.DamageReduction;
			}
			if (type != StatType.DamageEmber)
			{
				return StatType.None;
			}
			return StatType.DamageReductionEmber;
		}

		// Token: 0x060060F7 RID: 24823 RVA: 0x0004479C File Offset: 0x0004299C
		public static bool AllowDamageReduction(this StatType type)
		{
			return true;
		}

		// Token: 0x060060F8 RID: 24824 RVA: 0x00081504 File Offset: 0x0007F704
		public static StatType GetDebuffResist(this StatType type)
		{
			switch (type)
			{
			case StatType.Damage1H:
			case StatType.Damage2H:
			case StatType.DamageRanged:
				return StatType.ResistDebuffPhysical;
			case StatType.DamageMental:
				return StatType.ResistDebuffMental;
			case StatType.DamageChemical:
				return StatType.ResistDebuffChemical;
			case StatType.DamageEmber:
				return StatType.ResistDebuffEmber;
			default:
				return StatType.None;
			}
		}

		// Token: 0x060060F9 RID: 24825 RVA: 0x00081536 File Offset: 0x0007F736
		public static DiminishingReturnType GetDiminishingReturnType(this StatType type)
		{
			if (type == StatType.Movement)
			{
				return DiminishingReturnType.Movement;
			}
			if (type == StatType.Haste)
			{
				return DiminishingReturnType.Haste;
			}
			return DiminishingReturnType.None;
		}

		// Token: 0x060060FA RID: 24826 RVA: 0x00081547 File Offset: 0x0007F747
		public static bool IsValid(this StatType type)
		{
			return type - StatType.DamageResist > 5 && type > StatType.None;
		}

		// Token: 0x060060FB RID: 24827 RVA: 0x00081556 File Offset: 0x0007F756
		public static bool IsInvalid(this StatType type)
		{
			return !type.IsValid();
		}

		// Token: 0x060060FC RID: 24828 RVA: 0x00081561 File Offset: 0x0007F761
		public static bool IsCombatOnly(this StatType type)
		{
			if (type <= StatType.Resilience)
			{
				if (type != StatType.CombatMovement && type != StatType.Resilience)
				{
					return false;
				}
			}
			else if (type - StatType.Avoid > 3 && type - StatType.Hit > 2 && type - StatType.DamagePhysical > 1)
			{
				return false;
			}
			return true;
		}

		// Token: 0x060060FD RID: 24829 RVA: 0x000814AF File Offset: 0x0007F6AF
		private static bool AlwaysShowAsCombatOnly(this StatType type)
		{
			return type - StatType.Damage1H <= 5;
		}

		// Token: 0x060060FE RID: 24830 RVA: 0x0008158C File Offset: 0x0007F78C
		public static bool ShowAsCombatOnly(this StatType type)
		{
			return type.IsCombatOnly() || type.AlwaysShowAsCombatOnly();
		}

		// Token: 0x060060FF RID: 24831 RVA: 0x0008159E File Offset: 0x0007F79E
		public static bool ModifiedByArmorCapacity(this StatType type)
		{
			return type - StatType.Movement <= 4 || type == StatType.Flanking;
		}

		// Token: 0x06006100 RID: 24832 RVA: 0x000815AF File Offset: 0x0007F7AF
		public static bool IsMeleePhysical(this StatType type)
		{
			return type - StatType.Damage1H <= 1 || type == StatType.DamageEmber;
		}

		// Token: 0x06006101 RID: 24833 RVA: 0x000815C0 File Offset: 0x0007F7C0
		public static bool IsRangedPhysical(this StatType type)
		{
			return type == StatType.DamageRanged;
		}

		// Token: 0x06006102 RID: 24834 RVA: 0x000815CA File Offset: 0x0007F7CA
		public static bool CanBeDefended(this StatType type)
		{
			return type - StatType.Damage1H <= 6;
		}

		// Token: 0x06006103 RID: 24835 RVA: 0x001FE55C File Offset: 0x001FC75C
		private static void InitializeStatTypeCollection(Dictionary<StatType, int> collection)
		{
			if (collection != null)
			{
				collection.Clear();
				for (int i = 0; i < StatTypeExtensions.StatTypes.Length; i++)
				{
					collection.Add(StatTypeExtensions.StatTypes[i], 0);
				}
			}
		}

		// Token: 0x06006104 RID: 24836 RVA: 0x001FE594 File Offset: 0x001FC794
		public static Dictionary<StatType, int> GetStatTypeCollection()
		{
			if (StatTypeExtensions.m_statTypeCollectionStack != null && StatTypeExtensions.m_statTypeCollectionStack.Count > 0)
			{
				return StatTypeExtensions.m_statTypeCollectionStack.Pop();
			}
			Dictionary<StatType, int> dictionary = new Dictionary<StatType, int>(StatTypeExtensions.StatTypes.Length, default(StatTypeComparer));
			StatTypeExtensions.InitializeStatTypeCollection(dictionary);
			return dictionary;
		}

		// Token: 0x06006105 RID: 24837 RVA: 0x000815D6 File Offset: 0x0007F7D6
		public static void ReturnStatTypeCollection(Dictionary<StatType, int> collection)
		{
			if (collection == null)
			{
				return;
			}
			StatTypeExtensions.InitializeStatTypeCollection(collection);
			if (StatTypeExtensions.m_statTypeCollectionStack == null)
			{
				StatTypeExtensions.m_statTypeCollectionStack = new Stack<Dictionary<StatType, int>>(100);
			}
			StatTypeExtensions.m_statTypeCollectionStack.Push(collection);
		}

		// Token: 0x06006106 RID: 24838 RVA: 0x001FE5E0 File Offset: 0x001FC7E0
		public static string GetStatPanelDisplay(this StatType type)
		{
			if (type <= StatType.CombatMovement)
			{
				if (type == StatType.RegenHealth)
				{
					return "Health Regen";
				}
				if (type == StatType.RegenStamina)
				{
					return "Stamina Regen";
				}
				if (type == StatType.CombatMovement)
				{
					return "Combat Mov.";
				}
			}
			else
			{
				if (type == StatType.Flanking)
				{
					return "Positional";
				}
				switch (type)
				{
				case StatType.Damage1H:
					return "1H Weapons";
				case StatType.Damage2H:
					return "2H Weapons";
				case StatType.DamageRanged:
					return "Ranged Weapons";
				case StatType.DamageMental:
					return "Mental";
				case StatType.DamageChemical:
					return "Chemical";
				case StatType.DamageEmber:
					return "Ember";
				case StatType.DamagePhysical:
				case StatType.DamageRaw:
				case (StatType)58:
				case (StatType)59:
				case (StatType)64:
				case (StatType)65:
				case (StatType)66:
				case (StatType)67:
				case (StatType)68:
				case (StatType)69:
					break;
				case StatType.ResistDamagePhysical:
					return "Physical";
				case StatType.ResistDamageMental:
					return "Mental";
				case StatType.ResistDamageChemical:
					return "Chemical";
				case StatType.ResistDamageEmber:
					return "Ember";
				case StatType.ResistDebuffPhysical:
					return "Physical";
				case StatType.ResistDebuffMental:
					return "Mental";
				case StatType.ResistDebuffChemical:
					return "Chemical";
				case StatType.ResistDebuffEmber:
					return "Ember";
				case StatType.ResistDebuffMovement:
					return "Movement";
				default:
					switch (type)
					{
					case StatType.ResistStun:
						return "Stun";
					case StatType.ResistFear:
						return "Fear";
					case StatType.ResistDaze:
						return "Daze";
					case StatType.ResistEnrage:
						return "Enrage";
					case StatType.ResistConfuse:
						return "Confuse";
					case StatType.ResistLull:
						return "Lull";
					case StatType.DamageResist:
					case StatType.DebuffResist:
						return "Physical";
					case StatType.DamageResistEmber:
					case StatType.DebuffResistEmber:
						return "Ember";
					case StatType.DamageReduction:
						return "Physical Reduction";
					case StatType.DamageReductionEmber:
						return "Ember Reduction";
					}
					break;
				}
			}
			return type.ToString();
		}

		// Token: 0x06006107 RID: 24839 RVA: 0x001FE790 File Offset: 0x001FC990
		public static string GetTooltipDescription(this StatType type, int statValue)
		{
			string text = string.Empty;
			if (type <= StatType.ResistDebuffMovement)
			{
				if (type <= StatType.Flanking)
				{
					switch (type)
					{
					case StatType.RegenHealth:
						text = "Percent modification to your health regeneration rate";
						goto IL_24F;
					case StatType.RegenStamina:
						text = "Percent modification to your stamina regeneration rate";
						goto IL_24F;
					case StatType.Healing:
						text = "Percent modification to your outgoing healing";
						goto IL_24F;
					case (StatType)13:
					case (StatType)14:
					case (StatType)15:
					case (StatType)16:
					case (StatType)17:
					case (StatType)18:
					case (StatType)19:
						break;
					case StatType.Movement:
						text = "Percent modification to your overall movement speed while not in combat stance";
						goto IL_24F;
					case StatType.CombatMovement:
						text = "Percent modification to your overall movement speed while in combat stance";
						goto IL_24F;
					case StatType.Haste:
						text = "Modifies the amount of time it takes for abilities and auto attack to refresh";
						goto IL_24F;
					case StatType.SafeFall:
						text = "Reduces the damage you take from falling";
						goto IL_24F;
					case StatType.Resilience:
						text = "Percent chance at a saving throw. When your health drops to zero this stat determines if you are healed instead of knocked out. It also reduces the bonus damage applied from Heavy and Critical hits";
						goto IL_24F;
					default:
						switch (type)
						{
						case StatType.Avoid:
							text = "Percent chance of avoiding an incoming attack negating all damage and effects";
							goto IL_24F;
						case StatType.Block:
							text = "Percent chance of blocking incoming attacks which negate damage based on your weapon or shield";
							goto IL_24F;
						case StatType.Parry:
							text = "Percent chance of parrying incoming attacks which impose a disadvantage on your attacker and mitigate some damage";
							goto IL_24F;
						case StatType.Riposte:
							text = "Percent chance of executing a counter attack after a successful parry";
							goto IL_24F;
						case StatType.Hit:
							text = "Modifies your probability of missing, landing glancing, heavy, or critical hit.";
							goto IL_24F;
						case StatType.Penetration:
							text = "Percent of armor your attacks ignore in combat";
							goto IL_24F;
						case StatType.Flanking:
							text = "Modifies the value of your weapon positional bonuses";
							goto IL_24F;
						}
						break;
					}
				}
				else
				{
					switch (type)
					{
					case StatType.Damage1H:
						text = "Percent modification to your damage output with 1 handed melee weapons";
						goto IL_24F;
					case StatType.Damage2H:
						text = "Percent modification to your damage output with 2 handed melee weapons";
						goto IL_24F;
					case StatType.DamageRanged:
						text = "Percent modification to your damage output with ranged weapons";
						goto IL_24F;
					default:
						switch (type)
						{
						case StatType.ResistDamagePhysical:
							text = "Percent reduction in physical damage taken";
							goto IL_24F;
						case StatType.ResistDamageMental:
							text = "Percent reduction in mental damage taken";
							goto IL_24F;
						case StatType.ResistDamageChemical:
							text = "Percent reduction in chemical damage taken";
							goto IL_24F;
						case StatType.ResistDamageEmber:
							text = "Percent reduction in ember damage taken";
							goto IL_24F;
						case StatType.ResistDebuffPhysical:
							text = "Chance to fully resist incoming physical debuff\nIf not fully resisted acts as a percent reduction to the duration";
							goto IL_24F;
						case StatType.ResistDebuffMental:
							text = "Chance to fully resist incoming mental debuff\nIf not fully resisted acts as a percent reduction to the duration";
							goto IL_24F;
						case StatType.ResistDebuffChemical:
							text = "Chance to fully resist incoming chemical debuff\nIf not fully resisted acts as a percent reduction to the duration";
							goto IL_24F;
						case StatType.ResistDebuffEmber:
							text = "Chance to fully resist incoming Ember debuff\nIf not fully resisted acts as a percent reduction to the duration";
							goto IL_24F;
						case StatType.ResistDebuffMovement:
							text = "Chance to fully resist incoming movement debuff\nIf not fully resisted acts as a percent reduction to the duration";
							goto IL_24F;
						}
						break;
					}
				}
			}
			else if (type <= StatType.ResistDaze)
			{
				if (type == StatType.ResistStun)
				{
					text = "Chance to fully resist incoming stun effects\nIf not fully resisted acts as a percent reduction to the duration";
					goto IL_24F;
				}
				if (type == StatType.ResistDaze)
				{
					text = "Chance to fully resist incoming daze effects\nIf not fully resisted acts as a percent reduction to the duration";
					goto IL_24F;
				}
			}
			else
			{
				if (type == StatType.ResistConfuse)
				{
					text = "Chance to fully resist incoming confuse effects\nIf not fully resisted acts as a percent reduction to the duration";
					goto IL_24F;
				}
				if (type == StatType.DamageReduction)
				{
					text = StatTypeExtensions.kDmgReductionTooltip;
					goto IL_24F;
				}
			}
			return text;
			IL_24F:
			if (type.ShowAsCombatOnly())
			{
				text = ZString.Format<string, string>("{0} {1}", text, "(This stat is only active in combat stance)");
			}
			StatSettings.ClampSettings settingForStat = GlobalSettings.Values.Stats.GetSettingForStat(type);
			if (settingForStat != null)
			{
				text = ZString.Format<string, int, int>("{0}\nMax Equipped: {1}\nMax Total: {2}", text, settingForStat.MaxEquipped, settingForStat.MaxTotal);
			}
			return text;
		}

		// Token: 0x06006108 RID: 24840 RVA: 0x001FEA34 File Offset: 0x001FCC34
		public static string GetTooltipDisplay(this StatType type)
		{
			if (StatTypeExtensions.m_tooltipDisplayValues == null)
			{
				StatTypeExtensions.m_tooltipDisplayValues = new Dictionary<StatType, string>(default(StatTypeComparer));
			}
			string text = string.Empty;
			if (!StatTypeExtensions.m_tooltipDisplayValues.TryGetValue(type, out text))
			{
				text = type.GetTooltipDisplayInternal();
				StatTypeExtensions.m_tooltipDisplayValues.Add(type, text);
			}
			return text;
		}

		// Token: 0x06006109 RID: 24841 RVA: 0x00081600 File Offset: 0x0007F800
		public static string GetWeaponDamageTypeDisplay(this StatType type)
		{
			return type.GetTooltipDisplayInternal();
		}

		// Token: 0x0600610A RID: 24842 RVA: 0x001FEA8C File Offset: 0x001FCC8C
		private static string GetTooltipDisplayInternal(this StatType type)
		{
			if (type <= StatType.CombatMovement)
			{
				if (type == StatType.RegenHealth)
				{
					return "Health Regen";
				}
				if (type == StatType.RegenStamina)
				{
					return "Stamina Regen";
				}
				if (type == StatType.CombatMovement)
				{
					return "Combat Mov.";
				}
			}
			else
			{
				if (type == StatType.SafeFall)
				{
					return "Safe Fall";
				}
				switch (type)
				{
				case StatType.Flanking:
					return "Positional";
				case (StatType)43:
				case (StatType)44:
				case (StatType)45:
				case (StatType)46:
				case (StatType)47:
				case (StatType)48:
				case (StatType)49:
				case StatType.DamagePhysical:
				case (StatType)58:
				case (StatType)59:
				case (StatType)64:
				case (StatType)65:
				case (StatType)66:
				case (StatType)67:
				case (StatType)68:
				case (StatType)69:
					break;
				case StatType.Damage1H:
					return "1H Weapon Dmg";
				case StatType.Damage2H:
					return "2H Weapon Dmg";
				case StatType.DamageRanged:
					return "Ranged Dmg";
				case StatType.DamageMental:
					return "Mental Dmg";
				case StatType.DamageChemical:
					return "Chemical Dmg";
				case StatType.DamageEmber:
					return "Ember Dmg";
				case StatType.DamageRaw:
					return "Dmg";
				case StatType.ResistDamagePhysical:
					return "Physical Dmg Resists";
				case StatType.ResistDamageMental:
					return "Mental Dmg Resists";
				case StatType.ResistDamageChemical:
					return "Chemical Dmg Resists";
				case StatType.ResistDamageEmber:
					return "Ember Dmg Resists";
				case StatType.ResistDebuffPhysical:
					return "Physical Debuff Resists";
				case StatType.ResistDebuffMental:
					return "Mental Debuff Resists";
				case StatType.ResistDebuffChemical:
					return "Chemical Debuff Resists";
				case StatType.ResistDebuffEmber:
					return "Ember Debuff Resists";
				case StatType.ResistDebuffMovement:
					return "Movement Debuff Resists";
				default:
					switch (type)
					{
					case StatType.ResistStun:
						return "Stun Resists";
					case StatType.ResistFear:
						return "Fear Resists";
					case StatType.ResistDaze:
						return "Daze Resists";
					case StatType.ResistEnrage:
						return "Enrage Resists";
					case StatType.ResistConfuse:
						return "Confuse Resists";
					case StatType.ResistLull:
						return "Lull Resists";
					case StatType.DamageResist:
						return "Physical Dmg Resists";
					case StatType.DamageResistEmber:
						return "Ember Dmg Resists";
					case StatType.DamageReduction:
						return "Physical Dmg Reduction";
					case StatType.DamageReductionEmber:
						return "Ember Dmg Reduction";
					case StatType.DebuffResist:
						return "Physical Debuff Resists";
					case StatType.DebuffResistEmber:
						return "Ember Debuff Resists";
					}
					break;
				}
			}
			return type.ToString();
		}

		// Token: 0x0600610B RID: 24843 RVA: 0x001FEC74 File Offset: 0x001FCE74
		public static bool IsPercentBased(this StatType type)
		{
			if (type <= StatType.Riposte)
			{
				if (type <= StatType.CombatMovement)
				{
					if (type - StatType.RegenHealth > 2 && type - StatType.Movement > 1)
					{
						return false;
					}
				}
				else if (type != StatType.Resilience && type - StatType.Avoid > 3)
				{
					return false;
				}
			}
			else if (type <= StatType.ResistDebuffMovement)
			{
				switch (type)
				{
				case StatType.Penetration:
				case StatType.Damage1H:
				case StatType.Damage2H:
				case StatType.DamageRanged:
				case StatType.DamageMental:
				case StatType.DamageChemical:
				case StatType.DamageEmber:
				case StatType.DamagePhysical:
				case StatType.ResistDamagePhysical:
				case StatType.ResistDamageMental:
				case StatType.ResistDamageChemical:
				case StatType.ResistDamageEmber:
					break;
				case StatType.Flanking:
				case (StatType)43:
				case (StatType)44:
				case (StatType)45:
				case (StatType)46:
				case (StatType)47:
				case (StatType)48:
				case (StatType)49:
				case StatType.DamageRaw:
				case (StatType)58:
				case (StatType)59:
					return false;
				default:
					if (type - StatType.ResistDebuffPhysical > 4)
					{
						return false;
					}
					break;
				}
			}
			else if (type - StatType.ResistStun > 1 && type - StatType.ResistDaze > 3)
			{
				return false;
			}
			return true;
		}

		// Token: 0x0600610C RID: 24844 RVA: 0x00081608 File Offset: 0x0007F808
		public static string GetIndexName(this StatType type)
		{
			if (type == StatType.CombatMovement)
			{
				return "Combat Movement";
			}
			return type.GetTooltipDisplayInternal();
		}

		// Token: 0x0600610D RID: 24845 RVA: 0x001FED40 File Offset: 0x001FCF40
		public static float GetHasteAdjustedTimeRemaining(float totalTime, float haste, float percentComplete, bool clampHasteTo100)
		{
			if (haste > 0f && clampHasteTo100)
			{
				haste = Mathf.Clamp01(haste);
			}
			else if (haste < 0f)
			{
				float t = Mathf.Clamp(haste, -1f, 0f) + 1f;
				haste = Mathf.Lerp(-0.8f, 0f, t);
			}
			float num = totalTime / (1f + haste);
			return num - num * percentComplete;
		}

		// Token: 0x0600610E RID: 24846 RVA: 0x001FEDA4 File Offset: 0x001FCFA4
		public static int GetPvpResist(this StatType type)
		{
			if (GlobalSettings.Values == null || GlobalSettings.Values.Combat == null)
			{
				return 0;
			}
			switch (type)
			{
			case StatType.ResistDamagePhysical:
			case StatType.ResistDamageMental:
			case StatType.ResistDamageChemical:
			case StatType.ResistDamageEmber:
				goto IL_BB;
			case (StatType)64:
			case (StatType)65:
			case (StatType)66:
			case (StatType)67:
			case (StatType)68:
			case (StatType)69:
				return 0;
			case StatType.ResistDebuffPhysical:
			case StatType.ResistDebuffMental:
			case StatType.ResistDebuffChemical:
			case StatType.ResistDebuffEmber:
				goto IL_CB;
			case StatType.ResistDebuffMovement:
				break;
			default:
				switch (type)
				{
				case StatType.ResistStun:
				case StatType.ResistFear:
				case StatType.ResistDaze:
				case StatType.ResistEnrage:
				case StatType.ResistConfuse:
				case StatType.ResistLull:
					break;
				case (StatType)92:
				case (StatType)97:
				case (StatType)98:
				case (StatType)99:
				case StatType.DamageReduction:
				case StatType.DamageReductionEmber:
					return 0;
				case StatType.DamageResist:
				case StatType.DamageResistEmber:
					goto IL_BB;
				case StatType.DebuffResist:
				case StatType.DebuffResistEmber:
					goto IL_CB;
				default:
					return 0;
				}
				break;
			}
			return GlobalSettings.Values.Combat.PvpResistControl;
			IL_BB:
			return GlobalSettings.Values.Combat.PvpResistDamage;
			IL_CB:
			return GlobalSettings.Values.Combat.PvpResistDebuff;
		}

		// Token: 0x040053C2 RID: 21442
		public const string kUnusedStat = "[UNUSED]";

		// Token: 0x040053C3 RID: 21443
		public const string kFlankingDescription = "Positional";

		// Token: 0x040053C4 RID: 21444
		private static string kDmgReductionTooltip = "Reduces incoming physical damage taken by up to this amount, capped at " + 50.ToString() + "% of the incoming physical damage.";

		// Token: 0x040053C5 RID: 21445
		private static StatType[] m_statTypes = null;

		// Token: 0x040053C6 RID: 21446
		private static StatPanel[] m_statPanels = null;

		// Token: 0x040053C7 RID: 21447
		public const string kNoCategoryKey = "NONE";

		// Token: 0x040053C8 RID: 21448
		private static readonly Dictionary<string, List<StatType>> m_statPanelCategories = new Dictionary<string, List<StatType>>
		{
			{
				"NONE",
				new List<StatType>
				{
					StatType.Movement,
					StatType.CombatMovement,
					StatType.Haste,
					StatType.SafeFall
				}
			},
			{
				"Vitals",
				new List<StatType>
				{
					StatType.RegenHealth,
					StatType.RegenStamina,
					StatType.Healing
				}
			}
		};

		// Token: 0x040053C9 RID: 21449
		private static readonly Dictionary<string, List<StatType>> m_combatPanelCategories = new Dictionary<string, List<StatType>>
		{
			{
				"NONE",
				new List<StatType>()
			},
			{
				"Defense",
				new List<StatType>
				{
					StatType.Resilience,
					StatType.Avoid,
					StatType.Block,
					StatType.Parry,
					StatType.Riposte
				}
			},
			{
				"Offense",
				new List<StatType>
				{
					StatType.Hit,
					StatType.Penetration,
					StatType.Flanking
				}
			},
			{
				"Damage",
				new List<StatType>
				{
					StatType.Damage1H,
					StatType.Damage2H,
					StatType.DamageRanged,
					StatType.DamageMental,
					StatType.DamageChemical,
					StatType.DamageEmber
				}
			}
		};

		// Token: 0x040053CA RID: 21450
		private static readonly Dictionary<string, List<StatType>> m_resistPanelCategories = new Dictionary<string, List<StatType>>
		{
			{
				"NONE",
				new List<StatType>()
			},
			{
				"Damage",
				new List<StatType>
				{
					StatType.ResistDamagePhysical,
					StatType.ResistDamageMental,
					StatType.ResistDamageChemical,
					StatType.ResistDamageEmber
				}
			},
			{
				"Debuffs",
				new List<StatType>
				{
					StatType.ResistDebuffPhysical,
					StatType.ResistDebuffMental,
					StatType.ResistDebuffChemical,
					StatType.ResistDebuffEmber
				}
			},
			{
				"Control",
				new List<StatType>
				{
					StatType.ResistStun,
					StatType.ResistDaze,
					StatType.ResistDebuffMovement,
					StatType.ResistConfuse
				}
			}
		};

		// Token: 0x040053CB RID: 21451
		private static readonly StatType[] m_validDamageTypes = new StatType[]
		{
			StatType.Damage1H,
			StatType.Damage2H,
			StatType.DamageRanged,
			StatType.DamageMental,
			StatType.DamageChemical,
			StatType.DamageEmber,
			StatType.DamagePhysical,
			StatType.DamageRaw
		};

		// Token: 0x040053CC RID: 21452
		private static readonly StatType[] m_validDebuffTypes = new StatType[]
		{
			StatType.ResistDamagePhysical,
			StatType.ResistDamageMental,
			StatType.ResistDamageChemical,
			StatType.ResistDamageEmber,
			StatType.ResistDebuffPhysical,
			StatType.ResistDebuffMental,
			StatType.ResistDebuffChemical,
			StatType.ResistDebuffEmber,
			StatType.ResistDebuffMovement,
			StatType.ResistStun,
			StatType.ResistFear,
			StatType.ResistDaze,
			StatType.ResistEnrage,
			StatType.ResistConfuse,
			StatType.ResistLull,
			StatType.DamageResist,
			StatType.DamageResistEmber,
			StatType.DebuffResist,
			StatType.DebuffResistEmber
		};

		// Token: 0x040053CD RID: 21453
		private static readonly StatType[] m_validResistDamageTypes = new StatType[]
		{
			StatType.ResistDamagePhysical,
			StatType.ResistDamageMental,
			StatType.ResistDamageChemical,
			StatType.ResistDamageEmber
		};

		// Token: 0x040053CE RID: 21454
		private static readonly StatType[] m_validResistDebuffTypes = new StatType[]
		{
			StatType.ResistDebuffPhysical,
			StatType.ResistDebuffMental,
			StatType.ResistDebuffChemical,
			StatType.ResistDebuffEmber
		};

		// Token: 0x040053CF RID: 21455
		private static readonly StatType[] m_validBehaviorResistTypes = new StatType[]
		{
			StatType.ResistStun,
			StatType.ResistFear,
			StatType.ResistDaze,
			StatType.ResistEnrage,
			StatType.ResistConfuse,
			StatType.ResistLull
		};

		// Token: 0x040053D0 RID: 21456
		private static readonly StatType[] m_validBuffTypes = new StatType[]
		{
			StatType.RegenHealth,
			StatType.RegenStamina,
			StatType.Movement,
			StatType.CombatMovement,
			StatType.Haste,
			StatType.SafeFall,
			StatType.Resilience,
			StatType.Avoid,
			StatType.Block,
			StatType.Parry,
			StatType.Riposte,
			StatType.Hit,
			StatType.Penetration,
			StatType.Flanking
		};

		// Token: 0x040053D1 RID: 21457
		private static Stack<Dictionary<StatType, int>> m_statTypeCollectionStack = null;

		// Token: 0x040053D2 RID: 21458
		private const string kCombatMovementName = "Combat Mov.";

		// Token: 0x040053D3 RID: 21459
		private const string kCombatOnlyTooltip = "(This stat is only active in combat stance)";

		// Token: 0x040053D4 RID: 21460
		private const string kRegenText = "Regen";

		// Token: 0x040053D5 RID: 21461
		public const string kDamageText = "Dmg";

		// Token: 0x040053D6 RID: 21462
		public const string kResistsText = "Resists";

		// Token: 0x040053D7 RID: 21463
		public const string kDebuffText = "Debuff";

		// Token: 0x040053D8 RID: 21464
		private static Dictionary<StatType, string> m_tooltipDisplayValues = null;
	}
}
