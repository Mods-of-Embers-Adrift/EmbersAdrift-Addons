using System;
using Cysharp.Text;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A30 RID: 2608
	public static class ArchetypeInterfaceExtensions
	{
		// Token: 0x060050AB RID: 20651 RVA: 0x001CDA0C File Offset: 0x001CBC0C
		public static float GetDurabilityMultiplierForRepair(this IDurability source, ArchetypeInstance instance)
		{
			float result = 1f;
			if (source != null && instance != null)
			{
				IArmorClass armorClass = source as IArmorClass;
				if (armorClass != null && instance.ItemData != null && instance.ItemData.Durability != null)
				{
					int armorClass2 = armorClass.GetArmorClass((float)instance.ItemData.Durability.Absorbed);
					result = ((armorClass2 == armorClass.BaseArmorClass) ? 1f : ((float)armorClass2 / (float)armorClass.BaseArmorClass));
				}
				else if (source is WeaponItem)
				{
					result = source.GetWeaponDamageMultiplier(instance);
				}
			}
			return result;
		}

		// Token: 0x060050AC RID: 20652 RVA: 0x001CDA8C File Offset: 0x001CBC8C
		public static void AppendDurabilityToTooltipBlock(this IDurability source, TooltipTextBlock block, ArchetypeInstance instance)
		{
			if (source == null || !block)
			{
				return;
			}
			int num = (instance != null && instance.ItemData != null && instance.ItemData.Durability != null) ? Mathf.FloorToInt((float)(source.MaxDamageAbsorption - instance.ItemData.Durability.Absorbed)) : source.MaxDamageAbsorption;
			float value = (float)num / (float)source.MaxDamageAbsorption;
			bool flag = source.GetDurabilityMultiplierForRepair(instance) < 1f;
			string text = flag ? num.ToString().Color(UIManager.RedColor) : num.ToString();
			block.AppendLine("Durability", string.Concat(new string[]
			{
				text,
				"/",
				source.MaxDamageAbsorption.ToString(),
				" (",
				value.GetAsPercentage(),
				"%)"
			}));
			if (flag)
			{
				string arg = "Effectiveness diminished until repaired.";
				if (source is WeaponItem)
				{
					arg = "Weapon damage diminished until repaired.";
				}
				else if (source is ArmorItem)
				{
					arg = "Protection diminished until repaired.";
				}
				block.AppendLine("", ZString.Format<string, string>("<size=90%><i><color={0}>({1})</color></i></size>", UIManager.RedColor.ToHex(), arg));
			}
		}

		// Token: 0x060050AD RID: 20653 RVA: 0x001CDBB4 File Offset: 0x001CBDB4
		public static void AppendArmorClassToTooltipBlock(this IArmorClass source, TooltipTextBlock block, ArchetypeInstance instance)
		{
			if (source == null || !block)
			{
				return;
			}
			int num = (instance == null || instance.ItemData == null || instance.ItemData.Durability == null) ? source.BaseArmorClass : source.GetCurrentArmorClass((float)instance.ItemData.Durability.Absorbed);
			int baseArmorClass = source.BaseArmorClass;
			float value = ((float)baseArmorClass > 0f) ? ((float)num / (float)baseArmorClass) : 0f;
			string text = (num < baseArmorClass) ? num.ToString().Color(UIManager.RedColor) : num.ToString();
			block.AppendLine("Armor Class", string.Concat(new string[]
			{
				text,
				"/",
				baseArmorClass.ToString(),
				" (",
				value.GetAsPercentage(),
				"%)"
			}));
		}

		// Token: 0x060050AE RID: 20654 RVA: 0x001CDC88 File Offset: 0x001CBE88
		public static int GetArmorClass(this IArmorClass source, float dmgAbsorbed)
		{
			if (source == null)
			{
				return 0;
			}
			float degradationMultiplier = ArchetypeInterfaceExtensions.GetDegradationMultiplier(dmgAbsorbed, (float)source.MaxDamageAbsorption, GlobalSettings.Values.Player.ArmorDegradationAllowedFraction);
			return Mathf.CeilToInt((float)source.BaseArmorClass * degradationMultiplier);
		}

		// Token: 0x060050AF RID: 20655 RVA: 0x00075F82 File Offset: 0x00074182
		public static bool IsWeaponBroken(this IDurability source, ArchetypeInstance weaponInstance)
		{
			return source != null && weaponInstance != null && weaponInstance.ItemData != null && weaponInstance.ItemData.Durability != null && source.GetCurrentDurability((float)weaponInstance.ItemData.Durability.Absorbed) <= 0f;
		}

		// Token: 0x060050B0 RID: 20656 RVA: 0x001CDCC8 File Offset: 0x001CBEC8
		public static float GetWeaponDamageMultiplier(this IDurability source, ArchetypeInstance weaponInstance)
		{
			if (source == null || weaponInstance == null || weaponInstance.ItemData == null || weaponInstance.ItemData.Durability == null)
			{
				return 1f;
			}
			return ArchetypeInterfaceExtensions.GetDegradationMultiplier((float)weaponInstance.ItemData.Durability.Absorbed, (float)source.MaxDamageAbsorption, GlobalSettings.Values.Player.WeaponDegradationAllowedFraction);
		}

		// Token: 0x060050B1 RID: 20657 RVA: 0x001CDD24 File Offset: 0x001CBF24
		private static float GetDegradationMultiplier(float dmgAbsorbed, float maxDmgAbsorption, float allowedFraction)
		{
			float num = maxDmgAbsorption * allowedFraction;
			float num2 = maxDmgAbsorption - num;
			float result = 1f;
			if (dmgAbsorbed > num)
			{
				float num3 = (dmgAbsorbed - num) / num2;
				result = 1f - num3;
			}
			return result;
		}
	}
}
