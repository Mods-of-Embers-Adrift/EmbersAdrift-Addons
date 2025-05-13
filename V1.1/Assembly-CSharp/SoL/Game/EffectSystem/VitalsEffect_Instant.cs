using System;
using Cysharp.Text;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C1B RID: 3099
	[Serializable]
	public class VitalsEffect_Instant : VitalsEffect_Base
	{
		// Token: 0x170016D0 RID: 5840
		// (get) Token: 0x06005F95 RID: 24469 RVA: 0x00080584 File Offset: 0x0007E784
		private bool m_showCustomDiceOptions
		{
			get
			{
				return this.m_diceSource == SourceType.Custom;
			}
		}

		// Token: 0x170016D1 RID: 5841
		// (get) Token: 0x06005F96 RID: 24470 RVA: 0x0008058F File Offset: 0x0007E78F
		private bool m_showCustomDice
		{
			get
			{
				return this.m_diceSource == SourceType.Custom || this.m_overrideDice;
			}
		}

		// Token: 0x170016D2 RID: 5842
		// (get) Token: 0x06005F97 RID: 24471 RVA: 0x000805A1 File Offset: 0x0007E7A1
		public bool ForceNormalHit
		{
			get
			{
				return this.m_forceNormalHit;
			}
		}

		// Token: 0x170016D3 RID: 5843
		// (get) Token: 0x06005F98 RID: 24472 RVA: 0x000805A9 File Offset: 0x0007E7A9
		public bool HideOnTooltip
		{
			get
			{
				return this.m_hideOnTooltip;
			}
		}

		// Token: 0x170016D4 RID: 5844
		// (get) Token: 0x06005F99 RID: 24473 RVA: 0x000805B1 File Offset: 0x0007E7B1
		public bool ShowOnTooltip
		{
			get
			{
				return !this.m_hideOnTooltip;
			}
		}

		// Token: 0x170016D5 RID: 5845
		// (get) Token: 0x06005F9A RID: 24474 RVA: 0x000805BC File Offset: 0x0007E7BC
		public CombatTriggerCondition Condition
		{
			get
			{
				return this.m_condition;
			}
		}

		// Token: 0x170016D6 RID: 5846
		// (get) Token: 0x06005F9B RID: 24475 RVA: 0x000805C4 File Offset: 0x0007E7C4
		public VitalMods Mods
		{
			get
			{
				return this.m_mods;
			}
		}

		// Token: 0x170016D7 RID: 5847
		// (get) Token: 0x06005F9C RID: 24476 RVA: 0x000805CC File Offset: 0x0007E7CC
		public bool ApplyOffHandDamageMultiplier
		{
			get
			{
				return this.m_diceSource == SourceType.OffHandWeapon && this.m_applyOffHandDamageMultiplier;
			}
		}

		// Token: 0x06005F9D RID: 24477 RVA: 0x001FA6C0 File Offset: 0x001F88C0
		public bool TryGetDiceSet(IHandHeldItems handHeldItems, bool ignoreFallbackWeapon, out WeaponItem weaponItem, out ArchetypeInstance weaponItemInstance, out DiceSet diceSet)
		{
			weaponItem = null;
			weaponItemInstance = null;
			diceSet = default(DiceSet);
			SourceType diceSource = this.m_diceSource;
			if (diceSource == SourceType.Custom)
			{
				diceSet = this.m_diceSet;
				return true;
			}
			if (diceSource - SourceType.MainHandWeapon > 1)
			{
				throw new ArgumentException("m_diceSource");
			}
			CachedHandHeldItem cachedHandHeldItem = (this.m_diceSource == SourceType.MainHandWeapon) ? handHeldItems.MainHand : handHeldItems.OffHand;
			if (!cachedHandHeldItem.WeaponItem)
			{
				return false;
			}
			if (ignoreFallbackWeapon && cachedHandHeldItem.WeaponItem == GlobalSettings.Values.Combat.FallbackWeapon)
			{
				return false;
			}
			weaponItem = cachedHandHeldItem.WeaponItem;
			weaponItemInstance = cachedHandHeldItem.Instance;
			diceSet = (this.m_overrideDice ? this.m_diceSet : weaponItem.GetDynamicDiceSet(weaponItemInstance));
			return true;
		}

		// Token: 0x06005F9E RID: 24478 RVA: 0x001FA788 File Offset: 0x001F8988
		public StatType GetDamageChannel(IHandHeldItems handHeldItems)
		{
			switch (this.m_diceSource)
			{
			case SourceType.Custom:
				return this.m_damageChannelStatType;
			case SourceType.MainHandWeapon:
				if (handHeldItems.MainHand.WeaponItem == null)
				{
					throw new ArgumentException("MainHand WeaponItem is null and hence has no damage type!");
				}
				return handHeldItems.MainHand.WeaponItem.GetDamageType();
			case SourceType.OffHandWeapon:
				if (handHeldItems.OffHand.WeaponItem == null)
				{
					throw new ArgumentException("OffHand WeaponItem is null and hence has no damage type!");
				}
				return handHeldItems.OffHand.WeaponItem.GetDamageType();
			default:
				throw new ArgumentException("m_diceSource");
			}
		}

		// Token: 0x170016D8 RID: 5848
		// (get) Token: 0x06005F9F RID: 24479 RVA: 0x00080579 File Offset: 0x0007E779
		protected override bool AllowHealthFractionBonus
		{
			get
			{
				return this.m_resourceType == EffectResourceType.Health;
			}
		}

		// Token: 0x170016D9 RID: 5849
		// (get) Token: 0x06005FA0 RID: 24480 RVA: 0x00079557 File Offset: 0x00077757
		private StatType[] m_validDamageChannels
		{
			get
			{
				return StatTypeExtensions.ValidDamageTypes;
			}
		}

		// Token: 0x06005FA1 RID: 24481 RVA: 0x001FA820 File Offset: 0x001F8A20
		public bool Migrate()
		{
			switch (this.m_damageChannel)
			{
			case DamageType.Melee_Slashing:
			case DamageType.Melee_Crushing:
			case DamageType.Melee_Piercing:
				this.m_damageChannelStatType = StatType.Damage1H;
				break;
			case DamageType.Ranged_Auditory:
				this.m_damageChannelStatType = StatType.DamageMental;
				break;
			case DamageType.Ranged_Crushing:
			case DamageType.Ranged_Piercing:
				this.m_damageChannelStatType = StatType.DamageRanged;
				break;
			case DamageType.Natural_Life:
			case DamageType.Natural_Death:
			case DamageType.Natural_Chemical:
			case DamageType.Natural_Spirit:
				this.m_damageChannelStatType = StatType.DamageChemical;
				break;
			case DamageType.Elemental_Air:
			case DamageType.Elemental_Earth:
			case DamageType.Elemental_Fire:
			case DamageType.Elemental_Water:
				this.m_damageChannelStatType = StatType.DamageEmber;
				break;
			default:
				return false;
			}
			return true;
		}

		// Token: 0x170016DA RID: 5850
		// (get) Token: 0x06005FA2 RID: 24482 RVA: 0x000805DF File Offset: 0x0007E7DF
		public SourceType DiceSource
		{
			get
			{
				return this.m_diceSource;
			}
		}

		// Token: 0x170016DB RID: 5851
		// (get) Token: 0x06005FA3 RID: 24483 RVA: 0x000805E7 File Offset: 0x0007E7E7
		public bool OverrideDice
		{
			get
			{
				return this.m_overrideDice;
			}
		}

		// Token: 0x170016DC RID: 5852
		// (get) Token: 0x06005FA4 RID: 24484 RVA: 0x000805EF File Offset: 0x0007E7EF
		public DamageType DamageChannel
		{
			get
			{
				return this.m_damageChannel;
			}
		}

		// Token: 0x170016DD RID: 5853
		// (get) Token: 0x06005FA5 RID: 24485 RVA: 0x000805F7 File Offset: 0x0007E7F7
		public StatType DamageChannelStatType
		{
			get
			{
				return this.m_damageChannelStatType;
			}
		}

		// Token: 0x170016DE RID: 5854
		// (get) Token: 0x06005FA6 RID: 24486 RVA: 0x000805FF File Offset: 0x0007E7FF
		public DiceSet DiceSet
		{
			get
			{
				return this.m_diceSet;
			}
		}

		// Token: 0x06005FA7 RID: 24487 RVA: 0x001FA8AC File Offset: 0x001F8AAC
		public void AddInstantToTooltip(TooltipTextBlock block, GameEntity entity, Polarity polarity, ref UniqueId archetypeId, ReagentItem reagentItem, VitalsEffect_Instant offHandInstant, UniqueId? sourceArchetypeId, int index)
		{
			if (this.m_applyOffHandDamageMultiplier)
			{
				return;
			}
			DiceSet? diceSet = null;
			string text = string.Empty;
			DiceSet? diceSet2 = null;
			float? num = null;
			SourceType diceSource = this.m_diceSource;
			EffectResourceType resourceType;
			if (diceSource != SourceType.Custom)
			{
				if (diceSource - SourceType.MainHandWeapon <= 1)
				{
					ArchetypeInstance archetypeInstance = null;
					WeaponItem weaponItem = null;
					WeaponItem weaponItem2 = null;
					if (entity)
					{
						weaponItem = ((this.m_diceSource == SourceType.MainHandWeapon) ? (entity.GetHandheldItem_MainHand(out archetypeInstance) as WeaponItem) : (entity.GetHandheldItem_OffHand(out archetypeInstance) as WeaponItem));
						if (offHandInstant != null)
						{
							ArchetypeInstance archetypeInstance2;
							weaponItem2 = (entity.GetHandheldItem_OffHand(out archetypeInstance2) as WeaponItem);
						}
					}
					bool flag = false;
					if (weaponItem && weaponItem != GlobalSettings.Values.Combat.FallbackWeapon)
					{
						UniqueId id = (sourceArchetypeId != null) ? sourceArchetypeId.Value : archetypeId;
						AppliableEffectAbility appliableEffectAbility;
						CombatMasteryArchetype combatMasteryArchetype;
						if (InternalGameDatabase.Archetypes.TryGetAsType<AppliableEffectAbility>(id, out appliableEffectAbility) && appliableEffectAbility.Mastery.TryGetAsType(out combatMasteryArchetype))
						{
							flag = combatMasteryArchetype.EntityHasCompatibleWeapons(entity.HandHeldItemCache);
						}
					}
					if (flag)
					{
						diceSet = new DiceSet?(this.m_overrideDice ? this.m_diceSet : weaponItem.GetDynamicDiceSet(null));
						text = ZString.Format<string, string>("{0} {1}", weaponItem.GetDamageTypeDisplay(), "Dmg");
						if (weaponItem2 && weaponItem2 != GlobalSettings.Values.Combat.FallbackWeapon)
						{
							diceSet2 = new DiceSet?(weaponItem2.GetDynamicDiceSet(null));
							num = new float?(weaponItem2.GetOffHandDamageMultiplier());
						}
					}
					else if (this.m_overrideDice)
					{
						diceSet = new DiceSet?(this.m_diceSet);
					}
				}
			}
			else
			{
				diceSet = new DiceSet?(this.m_diceSet);
				text = this.m_damageChannelStatType.GetTooltipDisplay();
				if (polarity == Polarity.Positive)
				{
					resourceType = this.m_resourceType;
					if (resourceType != EffectResourceType.Health)
					{
						if (resourceType == EffectResourceType.Stamina)
						{
							text = "Stamina";
						}
					}
					else
					{
						text = "Health";
					}
				}
				if (this.m_resourceType.HasWounds() && this.m_modifyWounds)
				{
					text = ZString.Format<string>("{0} Wounds", text);
				}
			}
			string text2 = string.Empty;
			resourceType = this.m_resourceType;
			if (resourceType <= EffectResourceType.Stamina)
			{
				if (resourceType != EffectResourceType.Health)
				{
					if (resourceType == EffectResourceType.Stamina)
					{
						if (text != "Stamina")
						{
							text2 = " to Stamina";
						}
					}
				}
			}
			else if (resourceType != EffectResourceType.Armor)
			{
				if (resourceType == EffectResourceType.Threat)
				{
					text = "Threat";
				}
			}
			else
			{
				text2 = " to Armor";
			}
			string text3 = string.Empty;
			string diceText = string.Empty;
			if (diceSet != null)
			{
				ReagentItem.ReagentInstantMods reagentInstantMods = reagentItem ? reagentItem.GetInstantMods() : null;
				bool flag2 = reagentInstantMods != null && (reagentInstantMods.ValueAdditive != 0 || reagentInstantMods.ValueMultiplier != 0f);
				float num2;
				float num3;
				VitalsEffect_Instant.GetMinMaxValue(diceSet.Value, this.m_mods, reagentInstantMods, out num2, out num3);
				if (offHandInstant != null && diceSet2 != null && num != null)
				{
					float num4;
					float num5;
					VitalsEffect_Instant.GetMinMaxValue(diceSet2.Value, offHandInstant.m_mods, reagentInstantMods, out num4, out num5);
					num4 *= num.Value;
					num5 *= num.Value;
					num2 += num4;
					num3 += num5;
				}
				int num6 = Mathf.FloorToInt(num2);
				int num7 = Mathf.FloorToInt(num3);
				string text4 = (num6 == num7) ? num6.ToString() : ZString.Format<int, int>("{0}-{1}", num6, num7);
				if (base.ValueIsHealthFraction)
				{
					text4 = ZString.Format<string>("{0}%", text4);
				}
				text3 = (flag2 ? ZString.Format<string, string, string, string>("<b><color={0}>{1}</color></b> {2}{3}", UIManager.ReagentBonusColor.ToHex(), text4, text, text2) : ZString.Format<string, string, string>("<b>{0}</b> {1}{2}", text4, text, text2));
				diceText = diceSet.Value.ToString();
			}
			else
			{
				text3 = ZString.Format<string, string>("<i>Weapon {0}</i>{1}", "Dmg", text2);
			}
			string arg;
			bool flag3 = this.m_mods.TryGetAlwaysShowModifierLine(out arg, null);
			string text5 = UIManager.TooltipShowMore ? "+Health Fraction Bonus" : "+HFB";
			if (flag3 && base.ApplyHealthFractionBonus)
			{
				text3 = ZString.Format<string, string, string, string, string>("{0} {1}({2}, {3}){4}", text3, "<i><size=80%>", arg, text5, "</size></i>");
			}
			else if (flag3)
			{
				text3 = ZString.Format<string, string, string, string>("{0} {1}({2}){3}", text3, "<i><size=80%>", arg, "</size></i>");
			}
			else if (base.ApplyHealthFractionBonus)
			{
				text3 = ZString.Format<string, string, string, string>("{0} {1}({2}){3}", text3, "<i><size=80%>", text5, "</size></i>");
			}
			string txt;
			if ((this.m_condition.Chance < 100f || (index == 0 && this.m_condition.Flags != EffectApplicationFlags.ValidHit) || (index != 0 && this.m_condition.Flags != EffectApplicationFlags.None)) && this.m_condition.TryGetChanceDescription(out txt))
			{
				block.AppendLine(txt, 0);
			}
			block.AppendLine(text3, 0);
			if (UIManager.TooltipShowMore)
			{
				string text6 = VitalsEffect_Instant.GetAltText(this, diceText, reagentItem);
				if (offHandInstant != null && diceSet2 != null && num != null)
				{
					string altText = VitalsEffect_Instant.GetAltText(offHandInstant, diceSet2.Value.ToString(), reagentItem);
					text6 = ZString.Format<string, string, string>("MH ({0}) + {1}% OH ({2})", text6, num.Value.GetAsPercentage(), altText);
				}
				if (!string.IsNullOrEmpty(text6))
				{
					block.AppendLine(ZString.Format<string, string, string>("{0}({1}){2}", "<i><size=80%>", text6, "</size></i>"), 0);
				}
				if (base.ApplyHealthFractionBonus)
				{
					float? healthFractionBonus = base.GetHealthFractionBonus(0.01f);
					float num8 = (healthFractionBonus != null) ? healthFractionBonus.Value : 0f;
					float? healthFractionBonus2 = base.GetHealthFractionBonus(1f);
					float num9 = (healthFractionBonus2 != null) ? healthFractionBonus2.Value : 0f;
					if (num8 != num9)
					{
						string arg2 = ZString.Format<string, string>("HFB: {0}% bonus at max health and {1}% bonus at min health", num9.GetAsPercentage(), num8.GetAsPercentage());
						block.AppendLine(ZString.Format<string, string, string>("{0}({1}){2}", "<i><size=80%>", arg2, "</size></i>"), 0);
					}
				}
			}
		}

		// Token: 0x06005FA8 RID: 24488 RVA: 0x001FAE6C File Offset: 0x001F906C
		private static void GetMinMaxValue(DiceSet diceSet, VitalMods mods, ReagentItem.ReagentInstantMods reagentMods, out float minValue, out float maxValue)
		{
			minValue = (float)diceSet.GetMinValue();
			maxValue = (float)diceSet.GetMaxValue();
			if (mods == null)
			{
				return;
			}
			int num = mods.ValueAdditive;
			if (reagentMods != null)
			{
				num += reagentMods.ValueAdditive;
			}
			minValue += (float)num;
			maxValue += (float)num;
			float num2 = mods.ValueMultiplier;
			if (reagentMods != null)
			{
				num2 += reagentMods.ValueMultiplier;
			}
			minValue *= num2;
			maxValue *= num2;
		}

		// Token: 0x06005FA9 RID: 24489 RVA: 0x001FAED8 File Offset: 0x001F90D8
		private static string GetAltText(VitalsEffect_Instant vitals, string diceText, ReagentItem reagentItem)
		{
			if (vitals == null)
			{
				return string.Empty;
			}
			bool flag = !string.IsNullOrEmpty(diceText);
			string text;
			bool flag2 = vitals.m_mods.TryGetValueModLines(out text, reagentItem);
			if (flag && flag2)
			{
				return ZString.Format<string, string>("{0}, {1}", diceText, text);
			}
			if (flag)
			{
				return diceText;
			}
			if (flag2)
			{
				return text;
			}
			return string.Empty;
		}

		// Token: 0x04005280 RID: 21120
		private const string kDiceGroup = "Dice";

		// Token: 0x04005281 RID: 21121
		private const string kModifierGroup = "Modifiers";

		// Token: 0x04005282 RID: 21122
		[SerializeField]
		private CombatTriggerCondition m_condition;

		// Token: 0x04005283 RID: 21123
		[SerializeField]
		private SourceType m_diceSource;

		// Token: 0x04005284 RID: 21124
		[SerializeField]
		private bool m_applyOffHandDamageMultiplier;

		// Token: 0x04005285 RID: 21125
		[SerializeField]
		private bool m_overrideDice;

		// Token: 0x04005286 RID: 21126
		[SerializeField]
		private DamageType m_damageChannel;

		// Token: 0x04005287 RID: 21127
		[SerializeField]
		private StatType m_damageChannelStatType = StatType.Damage1H;

		// Token: 0x04005288 RID: 21128
		[SerializeField]
		private DiceSet m_diceSet;

		// Token: 0x04005289 RID: 21129
		[SerializeField]
		private VitalMods m_mods;

		// Token: 0x0400528A RID: 21130
		[SerializeField]
		private bool m_forceNormalHit;

		// Token: 0x0400528B RID: 21131
		[SerializeField]
		private bool m_hideOnTooltip;
	}
}
