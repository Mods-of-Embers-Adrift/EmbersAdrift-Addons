using System;
using Cysharp.Text;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AB1 RID: 2737
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Weapon Augment")]
	public class WeaponAugmentItem : AugmentItem
	{
		// Token: 0x1700136B RID: 4971
		// (get) Token: 0x0600548D RID: 21645 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool RefreshStats
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600548E RID: 21646 RVA: 0x0007883B File Offset: 0x00076A3B
		protected override string GetExpirationLabel()
		{
			return "Number of hits";
		}

		// Token: 0x0600548F RID: 21647 RVA: 0x00078842 File Offset: 0x00076A42
		protected override string GetExpirationSuffixLabel()
		{
			return "#";
		}

		// Token: 0x06005490 RID: 21648 RVA: 0x00078849 File Offset: 0x00076A49
		protected override bool IsValidAugment()
		{
			return this.m_weaponType != WeaponAugmentItem.WeaponType.None && this.m_augments != null && this.m_augments.Length != 0;
		}

		// Token: 0x06005491 RID: 21649 RVA: 0x001DAC10 File Offset: 0x001D8E10
		public WeaponAugmentFlags GetWeaponAugmentFlags()
		{
			if (this.m_augmentFlags == null)
			{
				this.m_augmentFlags = new WeaponAugmentFlags?(WeaponAugmentFlags.None);
				for (int i = 0; i < this.m_augments.Length; i++)
				{
					if (this.m_augments[i].Value != 0)
					{
						this.m_augmentFlags |= this.m_augments[i].Flag;
					}
				}
			}
			return this.m_augmentFlags.Value;
		}

		// Token: 0x06005492 RID: 21650 RVA: 0x001DACA0 File Offset: 0x001D8EA0
		public int GetWeaponAugmentValue(WeaponAugmentFlags flag)
		{
			for (int i = 0; i < this.m_augments.Length; i++)
			{
				if (this.m_augments[i].Flag.HasBitFlag(flag))
				{
					return this.m_augments[i].Value;
				}
			}
			return 0;
		}

		// Token: 0x06005493 RID: 21651 RVA: 0x001DACE4 File Offset: 0x001D8EE4
		protected override bool IsValidItem(ArchetypeInstance targetInstance)
		{
			if (!base.IsValidItem(targetInstance))
			{
				return false;
			}
			switch (this.m_weaponType)
			{
			case WeaponAugmentItem.WeaponType.Weapon1H:
			{
				WeaponItem weaponItem;
				return targetInstance.Archetype.TryGetAsType(out weaponItem) && this.IsValidEquipmentType(weaponItem) && weaponItem.GetWeaponType().IsOneHandedMelee();
			}
			case WeaponAugmentItem.WeaponType.Weapon2H:
			{
				WeaponItem weaponItem2;
				return targetInstance.Archetype.TryGetAsType(out weaponItem2) && this.IsValidEquipmentType(weaponItem2) && weaponItem2.GetWeaponType().IsTwoHandedMelee();
			}
			case WeaponAugmentItem.WeaponType.WeaponRanged:
			{
				WeaponItem weaponItem3;
				return targetInstance.Archetype.TryGetAsType(out weaponItem3) && this.IsValidEquipmentType(weaponItem3) && weaponItem3.GetWeaponType().IsRanged();
			}
			default:
				return false;
			}
		}

		// Token: 0x06005494 RID: 21652 RVA: 0x001DAD8C File Offset: 0x001D8F8C
		private bool IsValidEquipmentType(WeaponItem weaponItem)
		{
			EquipmentType type = weaponItem.Type;
			return type != EquipmentType.Weapon_Shield && type - EquipmentType.Weapon_OffhandAccessory > 1;
		}

		// Token: 0x06005495 RID: 21653 RVA: 0x00078867 File Offset: 0x00076A67
		public override CursorType GetCursorType()
		{
			return CursorType.SwordCursor3;
		}

		// Token: 0x06005496 RID: 21654 RVA: 0x001DADB0 File Offset: 0x001D8FB0
		protected override string GetRemainingText(ArchetypeInstance instance)
		{
			if (instance != null && instance.ItemData != null && instance.ItemData.Augment != null)
			{
				int arg = this.m_expirationAmount * (int)instance.ItemData.Augment.StackCount - instance.ItemData.Augment.Count;
				return ZString.Format<string, int>("<color={0}>{1}</color> hits", UIManager.AugmentColor.ToHex(), arg);
			}
			return string.Empty;
		}

		// Token: 0x06005497 RID: 21655 RVA: 0x001DAE1C File Offset: 0x001D901C
		protected override void AddAugmentStatsToTooltip(ArchetypeTooltip tooltip)
		{
			if (this.m_augments != null && tooltip)
			{
				for (int i = 0; i < this.m_augments.Length; i++)
				{
					if (this.m_augments[i] != null)
					{
						this.m_augments[i].AddToTooltipBlock(tooltip.CombatBlock, new Color?(UIManager.AugmentColor));
					}
				}
			}
		}

		// Token: 0x04004B21 RID: 19233
		[SerializeField]
		private WeaponAugmentItem.WeaponType m_weaponType;

		// Token: 0x04004B22 RID: 19234
		[SerializeField]
		private WeaponAugmentItem.AugmentData[] m_augments;

		// Token: 0x04004B23 RID: 19235
		private WeaponAugmentFlags? m_augmentFlags;

		// Token: 0x02000AB2 RID: 2738
		private enum WeaponType
		{
			// Token: 0x04004B25 RID: 19237
			None,
			// Token: 0x04004B26 RID: 19238
			[InspectorName("Weapon 1H")]
			Weapon1H,
			// Token: 0x04004B27 RID: 19239
			[InspectorName("Weapon 2H")]
			Weapon2H,
			// Token: 0x04004B28 RID: 19240
			WeaponRanged
		}

		// Token: 0x02000AB3 RID: 2739
		[Serializable]
		private class AugmentData
		{
			// Token: 0x1700136C RID: 4972
			// (get) Token: 0x06005499 RID: 21657 RVA: 0x0007886B File Offset: 0x00076A6B
			public WeaponAugmentFlags Flag
			{
				get
				{
					return this.m_augment;
				}
			}

			// Token: 0x1700136D RID: 4973
			// (get) Token: 0x0600549A RID: 21658 RVA: 0x00078873 File Offset: 0x00076A73
			public int Value
			{
				get
				{
					return this.m_value;
				}
			}

			// Token: 0x0600549B RID: 21659 RVA: 0x001DAE74 File Offset: 0x001D9074
			public void AddToTooltipBlock(TooltipTextBlock block, Color? colorOverride = null)
			{
				if (this.m_value != 0)
				{
					string arg = (this.m_value > 0) ? "+" : "";
					Color color = (this.m_value > 0) ? UIManager.BlueColor : UIManager.RedColor;
					if (colorOverride != null)
					{
						color = colorOverride.Value;
					}
					string arg2 = this.m_augment.ToString().Replace("Flanking", "Positional");
					block.AppendLine(ZString.Format<string, string, int, string>("<color={0}>{1}{2}</color> {3}", color.ToHex(), arg, this.m_value, arg2), 0);
				}
			}

			// Token: 0x04004B29 RID: 19241
			[SerializeField]
			private WeaponAugmentFlags m_augment;

			// Token: 0x04004B2A RID: 19242
			[SerializeField]
			private int m_value;
		}
	}
}
