using System;
using System.Collections;
using Cysharp.Text;
using SoL.Game.EffectSystem;
using SoL.Game.Flanking;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AB5 RID: 2741
	public abstract class WeaponItem : EquipableItem, IHandheldItem, IDamageSource, IDurability
	{
		// Token: 0x1700136E RID: 4974
		// (get) Token: 0x0600549E RID: 21662 RVA: 0x0007887B File Offset: 0x00076A7B
		public WeaponFlankingBonusWithOverride FlankingBonus
		{
			get
			{
				return this.m_flankingBonus;
			}
		}

		// Token: 0x1700136F RID: 4975
		// (get) Token: 0x0600549F RID: 21663 RVA: 0x00078883 File Offset: 0x00076A83
		private bool m_showWeaponProfile
		{
			get
			{
				return this.m_weaponProfileOverride == null;
			}
		}

		// Token: 0x17001370 RID: 4976
		// (get) Token: 0x060054A0 RID: 21664 RVA: 0x00078891 File Offset: 0x00076A91
		public VitalMods Mods
		{
			get
			{
				return this.m_mods;
			}
		}

		// Token: 0x17001371 RID: 4977
		// (get) Token: 0x060054A1 RID: 21665 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool ShowGroundGizmos
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001372 RID: 4978
		// (get) Token: 0x060054A2 RID: 21666 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool m_showWeaponDataOnTooltip
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17001373 RID: 4979
		// (get) Token: 0x060054A3 RID: 21667 RVA: 0x00078899 File Offset: 0x00076A99
		public int Delay
		{
			get
			{
				return this.m_delay;
			}
		}

		// Token: 0x17001374 RID: 4980
		// (get) Token: 0x060054A4 RID: 21668 RVA: 0x000788A1 File Offset: 0x00076AA1
		public bool CanBlock
		{
			get
			{
				return this.m_canBlock && !this.m_blockValue.IsZero;
			}
		}

		// Token: 0x17001375 RID: 4981
		// (get) Token: 0x060054A5 RID: 21669 RVA: 0x000788BB File Offset: 0x00076ABB
		public MinMaxIntRange BlockValue
		{
			get
			{
				return this.m_blockValue;
			}
		}

		// Token: 0x17001376 RID: 4982
		// (get) Token: 0x060054A6 RID: 21670 RVA: 0x000788C3 File Offset: 0x00076AC3
		protected override bool CanBlockInternal
		{
			get
			{
				return this.CanBlock;
			}
		}

		// Token: 0x17001377 RID: 4983
		// (get) Token: 0x060054A7 RID: 21671 RVA: 0x000788CB File Offset: 0x00076ACB
		protected override MinMaxIntRange BlockValueInternal
		{
			get
			{
				return this.BlockValue;
			}
		}

		// Token: 0x060054A8 RID: 21672 RVA: 0x001DAF08 File Offset: 0x001D9108
		private string GetDecadalScalingDescription()
		{
			if (!this.m_useDynamicDiceSet)
			{
				return "NOT DYNAMIC";
			}
			string text = string.Empty;
			for (int i = 0; i < GlobalSettings.kDecades.Length; i++)
			{
				int num = GlobalSettings.kDecades[i];
				DiceSet diceSet = this.m_dynamicDiceSet.GenerateDiceSet((float)num);
				int num2;
				int num3;
				float num4;
				float num5;
				float averageDps = diceSet.GetAverageDps(this.m_delay, out num2, out num3, out num4, out num5);
				text += string.Format("[{0:00}] {1}\n - MinDPS: {2:F02}, AvgDPS: {3:F02}, MaxDps: {4:F02}", new object[]
				{
					GlobalSettings.kDecades[i],
					diceSet.ToString(),
					num4,
					averageDps,
					num5
				});
				text += "\n------------------------------";
				if (i != GlobalSettings.kDecades.Length - 1)
				{
					text += "\n";
				}
			}
			return text;
		}

		// Token: 0x060054A9 RID: 21673 RVA: 0x00063B23 File Offset: 0x00061D23
		private string GetDetails()
		{
			return "Decadal Breakdown";
		}

		// Token: 0x060054AA RID: 21674 RVA: 0x001DAFE8 File Offset: 0x001D91E8
		public void GenerateDynamicDiceSet(ArchetypeInstance instance, float level)
		{
			if (this.m_useDynamicDiceSet && this.m_dynamicDiceSet != null && ((instance != null) ? instance.ItemData : null) != null && level > 0f)
			{
				instance.ItemData.WeaponDynamicDiceSet = new DiceSet?(this.m_dynamicDiceSet.GenerateDiceSet(level));
			}
		}

		// Token: 0x060054AB RID: 21675 RVA: 0x001DB038 File Offset: 0x001D9238
		public DiceSet GetDynamicDiceSet(ArchetypeInstance instance)
		{
			if (!this.m_useDynamicDiceSet || instance == null || instance.ItemData == null || instance.ItemData.WeaponDynamicDiceSet == null)
			{
				return this.m_dice;
			}
			return instance.ItemData.WeaponDynamicDiceSet.Value;
		}

		// Token: 0x060054AC RID: 21676 RVA: 0x000788D3 File Offset: 0x00076AD3
		public bool CanExecuteWith(IHandHeldItems handHeldItems)
		{
			return !this.RequiresFreeOffHand || handHeldItems.OffHand.Instance == null || handHeldItems.OffHand.Instance.Archetype == GlobalSettings.Values.Combat.FallbackWeapon;
		}

		// Token: 0x060054AD RID: 21677 RVA: 0x001DB088 File Offset: 0x001D9288
		public bool HasProperAmmo(IHandHeldItems handHeldItems)
		{
			if (this.RequiresAmmo)
			{
				if (handHeldItems.OffHand.RangedAmmo && handHeldItems.OffHand.RangedAmmo.AmmoType == this.RequiredAmmoType && handHeldItems.OffHand.Instance != null && handHeldItems.OffHand.Instance.ItemData != null && handHeldItems.OffHand.Instance.ItemData.Count != null)
				{
					int? count = handHeldItems.OffHand.Instance.ItemData.Count;
					int num = 0;
					if (!(count.GetValueOrDefault() <= num & count != null))
					{
						return true;
					}
				}
				return false;
			}
			return true;
		}

		// Token: 0x17001378 RID: 4984
		// (get) Token: 0x060054AE RID: 21678 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool IsAugmentable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060054AF RID: 21679 RVA: 0x00077208 File Offset: 0x00075408
		public override void OnInstanceCreated(ArchetypeInstance instance)
		{
			base.OnInstanceCreated(instance);
			instance.ItemData.Durability = new ItemDamage();
		}

		// Token: 0x060054B0 RID: 21680 RVA: 0x001DB140 File Offset: 0x001D9340
		public override EquipmentSlot GetTargetEquipmentSlot(GameEntity entity)
		{
			EquipmentSlot equipmentSlot = entity.CharacterData.MainHand_SecondaryActive ? EquipmentSlot.SecondaryWeapon_MainHand : EquipmentSlot.PrimaryWeapon_MainHand;
			if (this.GetWeaponType().IsOneHandedMelee() && GlobalSettings.Values.Roles.CanDualWield(entity.CharacterData.BaseRoleId))
			{
				EquipmentSlot index = equipmentSlot;
				ArchetypeInstance archetypeInstance;
				WeaponItem weaponItem;
				if (entity.CollectionController.Equipment.TryGetInstanceForIndex((int)index, out archetypeInstance) && archetypeInstance.Archetype && archetypeInstance.Archetype.TryGetAsType(out weaponItem) && weaponItem.GetWeaponType().IsOneHandedMelee())
				{
					EquipmentSlot equipmentSlot2 = entity.CharacterData.MainHand_SecondaryActive ? EquipmentSlot.SecondaryWeapon_OffHand : EquipmentSlot.PrimaryWeapon_OffHand;
					ArchetypeInstance archetypeInstance2;
					WeaponItem weaponItem2;
					if (!entity.CollectionController.Equipment.TryGetInstanceForIndex((int)equipmentSlot2, out archetypeInstance2))
					{
						equipmentSlot = equipmentSlot2;
					}
					else if (ClientGameManager.InputManager.HoldingShift && archetypeInstance2.Archetype && archetypeInstance2.Archetype.TryGetAsType(out weaponItem2) && weaponItem2.GetWeaponType().IsOneHandedMelee())
					{
						equipmentSlot = equipmentSlot2;
					}
				}
			}
			return equipmentSlot;
		}

		// Token: 0x060054B1 RID: 21681 RVA: 0x00078912 File Offset: 0x00076B12
		protected float GetCurrentDurability(float dmgAbsorbed)
		{
			return 1f - dmgAbsorbed / (float)this.m_maxDamageAbsorption;
		}

		// Token: 0x060054B2 RID: 21682 RVA: 0x00078923 File Offset: 0x00076B23
		public MinMaxFloatRange GetWeaponDistance()
		{
			if (!this.m_weaponProfileOverride)
			{
				return this.m_weaponProfile.Distance;
			}
			return this.m_weaponProfileOverride.Distance;
		}

		// Token: 0x060054B3 RID: 21683 RVA: 0x00078949 File Offset: 0x00076B49
		public float GetWeaponAngle()
		{
			return (float)(this.m_weaponProfileOverride ? this.m_weaponProfileOverride.Angle : this.m_weaponProfile.Angle);
		}

		// Token: 0x060054B4 RID: 21684 RVA: 0x00078971 File Offset: 0x00076B71
		public float GetWeaponAoeRadius()
		{
			if (!this.m_weaponProfileOverride)
			{
				return this.m_weaponProfile.AoeRadius;
			}
			return this.m_weaponProfileOverride.AoeRadius;
		}

		// Token: 0x060054B5 RID: 21685 RVA: 0x00078997 File Offset: 0x00076B97
		public float GetWeaponAoeAngle()
		{
			if (!this.m_weaponProfileOverride)
			{
				return this.m_weaponProfile.AoeAngle;
			}
			return this.m_weaponProfileOverride.AoeAngle;
		}

		// Token: 0x060054B6 RID: 21686 RVA: 0x000789BD File Offset: 0x00076BBD
		public WeaponTypes GetWeaponType()
		{
			if (!this.m_weaponProfileOverride)
			{
				return this.m_weaponProfile.WeaponType;
			}
			return this.m_weaponProfileOverride.WeaponType;
		}

		// Token: 0x060054B7 RID: 21687 RVA: 0x000789E3 File Offset: 0x00076BE3
		public StatType GetDamageType()
		{
			if (!this.m_weaponProfileOverride)
			{
				return this.m_weaponProfile.GetDamageType();
			}
			return this.m_weaponProfileOverride.GetDamageType();
		}

		// Token: 0x060054B8 RID: 21688 RVA: 0x00078A09 File Offset: 0x00076C09
		public float GetOffHandDamageMultiplier()
		{
			if (!this.m_weaponProfileOverride)
			{
				return this.m_weaponProfile.OffHandDamageMultiplier;
			}
			return this.m_weaponProfileOverride.OffHandDamageMultiplier;
		}

		// Token: 0x060054B9 RID: 21689 RVA: 0x00078A2F File Offset: 0x00076C2F
		public string GetDamageTypeDisplay()
		{
			return this.GetEquipmentTypeDisplay();
		}

		// Token: 0x060054BA RID: 21690 RVA: 0x001DB240 File Offset: 0x001D9440
		protected override string GetEquipmentTypeDisplay()
		{
			if (this.m_overrideWeaponTypeDescription)
			{
				return this.m_weaponTypeDescription;
			}
			WeaponTypes weaponType = this.GetWeaponType();
			if (weaponType <= WeaponTypes.Hammer1H)
			{
				if (weaponType <= WeaponTypes.Axe2H)
				{
					switch (weaponType)
					{
					case WeaponTypes.None:
						break;
					case WeaponTypes.Sword1H:
						goto IL_AA;
					case WeaponTypes.Sword2H:
						goto IL_D3;
					default:
						if (weaponType == WeaponTypes.Axe1H)
						{
							goto IL_AA;
						}
						if (weaponType == WeaponTypes.Axe2H)
						{
							goto IL_D3;
						}
						break;
					}
				}
				else if (weaponType <= WeaponTypes.Mace1H)
				{
					if (weaponType == WeaponTypes.Polearm)
					{
						goto IL_FC;
					}
					if (weaponType == WeaponTypes.Mace1H)
					{
						goto IL_AA;
					}
				}
				else
				{
					if (weaponType == WeaponTypes.Mace2H)
					{
						goto IL_D3;
					}
					if (weaponType == WeaponTypes.Hammer1H)
					{
						goto IL_AA;
					}
				}
			}
			else if (weaponType <= WeaponTypes.Staff2H)
			{
				if (weaponType <= WeaponTypes.Spear)
				{
					if (weaponType == WeaponTypes.Hammer2H)
					{
						goto IL_D3;
					}
					if (weaponType - WeaponTypes.Dagger <= 2)
					{
						goto IL_FC;
					}
				}
				else
				{
					if (weaponType == WeaponTypes.Staff1H)
					{
						goto IL_AA;
					}
					if (weaponType == WeaponTypes.Staff2H)
					{
						goto IL_D3;
					}
				}
			}
			else if (weaponType <= WeaponTypes.Crossbow)
			{
				if (weaponType == WeaponTypes.Bow || weaponType == WeaponTypes.Crossbow)
				{
					goto IL_FC;
				}
			}
			else
			{
				if (weaponType == WeaponTypes.Shield)
				{
					goto IL_FC;
				}
				if (weaponType == WeaponTypes.OffhandAccessory)
				{
					return "Offhand Accessory";
				}
			}
			return base.GetEquipmentTypeDisplay();
			IL_AA:
			string arg = weaponType.ToString().Replace("1H", "");
			return ZString.Format<string>("1H {0}", arg);
			IL_D3:
			string arg2 = weaponType.ToString().Replace("2H", "");
			return ZString.Format<string>("2H {0}", arg2);
			IL_FC:
			return weaponType.ToString();
		}

		// Token: 0x060054BB RID: 21691 RVA: 0x001DB35C File Offset: 0x001D955C
		public override void AddWeaponDataToTooltip(ArchetypeTooltip tooltip, bool isAutoAttack = false)
		{
			base.AddWeaponDataToTooltip(tooltip, false);
			if (this.m_showWeaponDataOnTooltip)
			{
				TooltipTextBlock combatBlock = tooltip.CombatBlock;
				int num = this.m_dice.GetMinValue();
				int num2 = this.m_dice.GetMaxValue();
				num += this.m_mods.ValueAdditive;
				num2 += this.m_mods.ValueAdditive;
				num = Mathf.FloorToInt((float)num * this.m_mods.ValueMultiplier);
				num2 = Mathf.FloorToInt((float)num2 * this.m_mods.ValueMultiplier);
				string text = ZString.Format<int, int, string>("{0}-{1} {2}", num, num2, "Dmg");
				string arg;
				if (this.m_mods.TryGetAlwaysShowModifierLine(out arg, null))
				{
					text = ZString.Format<string, string>("{0} {1}", text, arg);
				}
				string right = ZString.Format<string>("Delay {0}", this.m_delay.GetFormattedTime(true));
				if (UIManager.TooltipShowMore)
				{
					using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
					{
						utf16ValueStringBuilder.Append(text);
						utf16ValueStringBuilder.Append(" ");
						utf16ValueStringBuilder.Append("<i><size=80%>");
						utf16ValueStringBuilder.Append("(");
						utf16ValueStringBuilder.Append(this.m_dice.ToString());
						string value;
						if (this.m_mods.TryGetValueModLines(out value, null))
						{
							utf16ValueStringBuilder.Append(", ");
							utf16ValueStringBuilder.Append(value);
						}
						utf16ValueStringBuilder.Append(")");
						utf16ValueStringBuilder.Append("</size></i>");
						text = utf16ValueStringBuilder.ToString();
					}
				}
				combatBlock.AppendLine(text, right);
				if (!isAutoAttack)
				{
					string rangeDisplay = this.GetWeaponDistance().GetRangeDisplay();
					string angleDisplay = this.GetWeaponAngle().GetAngleDisplay();
					combatBlock.AppendLine(rangeDisplay, angleDisplay);
				}
				if (!isAutoAttack && this.GetWeaponType().ShowOffHandAbilityDamage())
				{
					string asPercentage = this.GetOffHandDamageMultiplier().GetAsPercentage();
					string text2 = ZString.Format<string>("{0}% OHAC", asPercentage);
					if (UIManager.TooltipShowMore)
					{
						text2 = ZString.Format<string, string, string, string>("{0} {1}(Off Hand Ability Contribution): When wielded as an off hand weapon {2}% of this weapon's damage roll is added to the ability result.{3}", text2, "<i><size=80%>", asPercentage, "</size></i>");
					}
					tooltip.CombatBlock.AppendLine(text2, 0);
				}
			}
			if (this.RequiresFreeOffHand && this.RequiresAmmo)
			{
				tooltip.RequirementsBlock.AppendLine("Empty Off Hand", "Ammo");
				return;
			}
			if (this.RequiresFreeOffHand)
			{
				tooltip.RequirementsBlock.AppendLine("Empty Off Hand", 0);
				return;
			}
			if (this.RequiresAmmo)
			{
				tooltip.RequirementsBlock.AppendLine("Ammo", 0);
			}
		}

		// Token: 0x060054BC RID: 21692 RVA: 0x00078A37 File Offset: 0x00076C37
		protected override void AddFlankingBonusToTooltip(ArchetypeTooltip tooltip)
		{
			base.AddFlankingBonusToTooltip(tooltip);
			WeaponFlankingBonusWithOverride flankingBonus = this.FlankingBonus;
			if (flankingBonus == null)
			{
				return;
			}
			flankingBonus.FillTooltip(tooltip);
		}

		// Token: 0x17001379 RID: 4985
		// (get) Token: 0x060054BD RID: 21693 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool RequiresAmmo
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700137A RID: 4986
		// (get) Token: 0x060054BE RID: 21694 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual RangedAmmoType RequiredAmmoType
		{
			get
			{
				return RangedAmmoType.None;
			}
		}

		// Token: 0x1700137B RID: 4987
		// (get) Token: 0x060054BF RID: 21695 RVA: 0x00078A51 File Offset: 0x00076C51
		protected virtual HandheldItemFlags m_handheldItemFlags
		{
			get
			{
				return this.GetWeaponType().GetHandheldItemFlagsForWeaponType();
			}
		}

		// Token: 0x1700137C RID: 4988
		// (get) Token: 0x060054C0 RID: 21696 RVA: 0x00045BCA File Offset: 0x00043DCA
		DamageType IDamageSource.Type
		{
			get
			{
				return DamageType.Melee_Slashing;
			}
		}

		// Token: 0x1700137D RID: 4989
		// (get) Token: 0x060054C1 RID: 21697 RVA: 0x00078A5E File Offset: 0x00076C5E
		public bool RequiresFreeOffHand
		{
			get
			{
				return this.GetWeaponType().RequiresFreeOffHand();
			}
		}

		// Token: 0x1700137E RID: 4990
		// (get) Token: 0x060054C2 RID: 21698 RVA: 0x00078A6B File Offset: 0x00076C6B
		public bool AlternateAnimationSet
		{
			get
			{
				return this.m_alternateAnimationSet;
			}
		}

		// Token: 0x1700137F RID: 4991
		// (get) Token: 0x060054C3 RID: 21699 RVA: 0x00078A73 File Offset: 0x00076C73
		HandheldItemFlags IHandheldItem.HandheldItemFlag
		{
			get
			{
				if (!(base.Id == GlobalSettings.Values.Combat.FallbackWeapon.Id))
				{
					return this.m_handheldItemFlags;
				}
				return HandheldItemFlags.Empty;
			}
		}

		// Token: 0x17001380 RID: 4992
		// (get) Token: 0x060054C4 RID: 21700 RVA: 0x0007854D File Offset: 0x0007674D
		int IDurability.MaxDamageAbsorption
		{
			get
			{
				return this.m_maxDamageAbsorption;
			}
		}

		// Token: 0x060054C5 RID: 21701 RVA: 0x00078555 File Offset: 0x00076755
		float IDurability.GetCurrentDurability(float dmgAbsorbed)
		{
			return this.GetCurrentDurability(dmgAbsorbed);
		}

		// Token: 0x17001381 RID: 4993
		// (get) Token: 0x060054C6 RID: 21702 RVA: 0x0004479C File Offset: 0x0004299C
		bool IDurability.DegradeOnHit
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060054C7 RID: 21703 RVA: 0x00078A9E File Offset: 0x00076C9E
		private IEnumerable GetWeaponProfile()
		{
			return SolOdinUtilities.GetDropdownItems<ScriptableWeaponProfile>();
		}

		// Token: 0x060054C8 RID: 21704 RVA: 0x001DB5D0 File Offset: 0x001D97D0
		public override void PrepareDynamicArchetype()
		{
			this.m_mods = new VitalMods(this.m_mods);
			if (this.m_weaponProfileOverride)
			{
				this.m_weaponProfile = new WeaponProfile();
				this.m_weaponProfile.CopyValuesFrom(this.m_weaponProfileOverride);
				this.m_weaponProfileOverride = null;
				return;
			}
			WeaponProfile weaponProfile = new WeaponProfile();
			weaponProfile.CopyValuesFrom(this.m_weaponProfile);
			this.m_weaponProfile = weaponProfile;
			this.m_weaponProfileOverride = null;
		}

		// Token: 0x060054C9 RID: 21705 RVA: 0x00078AA5 File Offset: 0x00076CA5
		public override bool IsAssignerHandled(ComponentEffectAssignerName assignerName)
		{
			return assignerName == ComponentEffectAssignerName.MaxDamageAbsorption || assignerName == ComponentEffectAssignerName.Delay || this.m_weaponProfile.IsAssignerHandled(assignerName) || this.m_dice.IsAssignerHandled(assignerName) || this.m_mods.IsAssignerHandled(assignerName) || base.IsAssignerHandled(assignerName);
		}

		// Token: 0x060054CA RID: 21706 RVA: 0x001DB640 File Offset: 0x001D9840
		public override bool PopulateDynamicValue(ComponentEffectAssignerName assignerName, float value, ComponentEffectOutputType type, MinMaxFloatRange? rangeOverride)
		{
			if (assignerName == ComponentEffectAssignerName.MaxDamageAbsorption)
			{
				this.m_maxDamageAbsorption = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_maxDamageAbsorption);
				return true;
			}
			if (assignerName == ComponentEffectAssignerName.Delay)
			{
				this.m_delay = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_delay);
				return true;
			}
			if (this.m_weaponProfile.IsAssignerHandled(assignerName))
			{
				return this.m_weaponProfile.PopulateDynamicValue(assignerName, value, type, rangeOverride);
			}
			if (this.m_dice.IsAssignerHandled(assignerName))
			{
				return this.m_dice.PopulateDynamicValue(assignerName, value, type, rangeOverride);
			}
			if (this.m_mods.IsAssignerHandled(assignerName))
			{
				return this.m_mods.PopulateDynamicValue(assignerName, value, type, rangeOverride);
			}
			return base.PopulateDynamicValue(assignerName, value, type, rangeOverride);
		}

		// Token: 0x04004B2B RID: 19243
		[SerializeField]
		private ScriptableWeaponProfile m_weaponProfileOverride;

		// Token: 0x04004B2C RID: 19244
		[SerializeField]
		private WeaponProfile m_weaponProfile = new WeaponProfile();

		// Token: 0x04004B2D RID: 19245
		[SerializeField]
		private WeaponFlankingBonusWithOverride m_flankingBonus;

		// Token: 0x04004B2E RID: 19246
		private const string kWeaponPropertiesGroupName = "Dice & Delay";

		// Token: 0x04004B2F RID: 19247
		[SerializeField]
		private DiceSet m_dice = new DiceSet(1, 4);

		// Token: 0x04004B30 RID: 19248
		[SerializeField]
		private int m_delay = 6;

		// Token: 0x04004B31 RID: 19249
		[SerializeField]
		protected int m_maxDamageAbsorption = 1000;

		// Token: 0x04004B32 RID: 19250
		[SerializeField]
		private VitalMods m_mods;

		// Token: 0x04004B33 RID: 19251
		[Tooltip("This should only be active for Longswords")]
		[SerializeField]
		private bool m_alternateAnimationSet;

		// Token: 0x04004B34 RID: 19252
		private const string kDescriptionGroup = "Description";

		// Token: 0x04004B35 RID: 19253
		[Tooltip("Equipment Type tooltip becomes the defined value rather than the one from the weapon profile")]
		[SerializeField]
		private bool m_overrideWeaponTypeDescription;

		// Token: 0x04004B36 RID: 19254
		[SerializeField]
		private string m_weaponTypeDescription = string.Empty;

		// Token: 0x04004B37 RID: 19255
		private const string kBlockValue = "Block Value";

		// Token: 0x04004B38 RID: 19256
		[SerializeField]
		private bool m_canBlock;

		// Token: 0x04004B39 RID: 19257
		[SerializeField]
		private MinMaxIntRange m_blockValue = new MinMaxIntRange(0, 0);

		// Token: 0x04004B3A RID: 19258
		private const string kNpcDynamicDiceSet = "Npc Dynamic Dice Set";

		// Token: 0x04004B3B RID: 19259
		[Tooltip("Only used for NPCs!")]
		[SerializeField]
		private bool m_useDynamicDiceSet;

		// Token: 0x04004B3C RID: 19260
		[SerializeField]
		private DynamicDiceSet m_dynamicDiceSet;

		// Token: 0x04004B3D RID: 19261
		[SerializeField]
		private DummyClass m_scalingDummy;
	}
}
