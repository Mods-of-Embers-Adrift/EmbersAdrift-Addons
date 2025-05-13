using System;
using Cysharp.Text;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AA5 RID: 2725
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Shield")]
	public class ShieldItem : WeaponItem, IArmorClass
	{
		// Token: 0x17001358 RID: 4952
		// (get) Token: 0x0600543A RID: 21562 RVA: 0x00062532 File Offset: 0x00060732
		public override EquipmentType Type
		{
			get
			{
				return EquipmentType.Weapon_Shield;
			}
		}

		// Token: 0x17001359 RID: 4953
		// (get) Token: 0x0600543B RID: 21563 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool m_showWeaponDataOnTooltip
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600543C RID: 21564 RVA: 0x00077D2B File Offset: 0x00075F2B
		public override EquipmentSlot GetTargetEquipmentSlot(GameEntity entity)
		{
			if (!entity.CharacterData.OffHand_SecondaryActive)
			{
				return EquipmentSlot.PrimaryWeapon_OffHand;
			}
			return EquipmentSlot.SecondaryWeapon_OffHand;
		}

		// Token: 0x0600543D RID: 21565 RVA: 0x001DA374 File Offset: 0x001D8574
		public override bool TryGetSalePrice(ArchetypeInstance instance, out ulong value)
		{
			if (!base.TryGetSalePrice(instance, out value))
			{
				return false;
			}
			float f = value * base.GetCurrentDurability((float)instance.ItemData.Durability.Absorbed);
			value = (ulong)((long)Mathf.FloorToInt(f));
			return true;
		}

		// Token: 0x1700135A RID: 4954
		// (get) Token: 0x0600543E RID: 21566 RVA: 0x00078545 File Offset: 0x00076745
		int IArmorClass.BaseArmorClass
		{
			get
			{
				return this.m_baseArmorClass;
			}
		}

		// Token: 0x1700135B RID: 4955
		// (get) Token: 0x0600543F RID: 21567 RVA: 0x0007854D File Offset: 0x0007674D
		int IArmorClass.MaxDamageAbsorption
		{
			get
			{
				return this.m_maxDamageAbsorption;
			}
		}

		// Token: 0x1700135C RID: 4956
		// (get) Token: 0x06005440 RID: 21568 RVA: 0x00045BCA File Offset: 0x00043DCA
		int IArmorClass.ArmorCost
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06005441 RID: 21569 RVA: 0x000766CB File Offset: 0x000748CB
		int IArmorClass.GetCurrentArmorClass(float damageAbsorbed)
		{
			return this.GetArmorClass(damageAbsorbed);
		}

		// Token: 0x06005442 RID: 21570 RVA: 0x00078555 File Offset: 0x00076755
		float IArmorClass.GetCurrentDurability(float damageAbsorbed)
		{
			return base.GetCurrentDurability(damageAbsorbed);
		}

		// Token: 0x06005443 RID: 21571 RVA: 0x001DA3B4 File Offset: 0x001D85B4
		public override void AddWeaponDataToTooltip(ArchetypeTooltip tooltip, bool isAutoAttack = false)
		{
			base.AddWeaponDataToTooltip(tooltip, isAutoAttack);
			if (tooltip && !Mathf.Approximately(base.Mods.ThreatMultiplier, 1f))
			{
				float value = (base.Mods.ThreatMultiplier > 1f) ? (base.Mods.ThreatMultiplier - 1f) : (1f - base.Mods.ThreatMultiplier);
				string arg = (base.Mods.ThreatMultiplier > 1f) ? "+" : "-";
				string threatDescription = TooltipExtensions.GetThreatDescription();
				tooltip.StatsBlock.AppendLine(ZString.Format<string, string, string, string>("<color={0}><b>{1}{2}%</b></color> {3}", UIManager.BlueColor.ToHex(), arg, value.GetAsPercentage(), threatDescription), 0);
			}
		}

		// Token: 0x06005444 RID: 21572 RVA: 0x0007855E File Offset: 0x0007675E
		public override bool IsAssignerHandled(ComponentEffectAssignerName assignerName)
		{
			return assignerName == ComponentEffectAssignerName.BaseArmorClass || base.IsAssignerHandled(assignerName);
		}

		// Token: 0x06005445 RID: 21573 RVA: 0x0007856D File Offset: 0x0007676D
		public override bool PopulateDynamicValue(ComponentEffectAssignerName assignerName, float value, ComponentEffectOutputType type, MinMaxFloatRange? rangeOverride)
		{
			if (assignerName == ComponentEffectAssignerName.BaseArmorClass)
			{
				this.m_baseArmorClass = ComponentEffectAssigners.Apply(assignerName, value, type, rangeOverride, this.m_baseArmorClass);
				return true;
			}
			return base.PopulateDynamicValue(assignerName, value, type, rangeOverride);
		}

		// Token: 0x04004B04 RID: 19204
		[SerializeField]
		private int m_baseArmorClass;
	}
}
