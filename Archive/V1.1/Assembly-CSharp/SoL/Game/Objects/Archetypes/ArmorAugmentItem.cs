using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.EffectSystem;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AA7 RID: 2727
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Armor Item")]
	public class ArmorAugmentItem : AugmentItem
	{
		// Token: 0x1700135F RID: 4959
		// (get) Token: 0x0600544C RID: 21580 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool RefreshStats
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600544D RID: 21581 RVA: 0x000785ED File Offset: 0x000767ED
		protected override string GetExpirationLabel()
		{
			return "Time in combat";
		}

		// Token: 0x0600544E RID: 21582 RVA: 0x000785F4 File Offset: 0x000767F4
		protected override string GetExpirationSuffixLabel()
		{
			return "minutes";
		}

		// Token: 0x0600544F RID: 21583 RVA: 0x000785FB File Offset: 0x000767FB
		protected override bool IsValidAugment()
		{
			return this.m_armorType != ArmorAugmentItem.ArmorType.None && this.m_augments != null && this.m_augments.Length != 0;
		}

		// Token: 0x17001360 RID: 4960
		// (get) Token: 0x06005450 RID: 21584 RVA: 0x00078619 File Offset: 0x00076819
		public StatModifier[] Augments
		{
			get
			{
				return this.m_augments;
			}
		}

		// Token: 0x06005451 RID: 21585 RVA: 0x001DA474 File Offset: 0x001D8674
		protected override bool IsValidItem(ArchetypeInstance targetInstance)
		{
			if (!base.IsValidItem(targetInstance))
			{
				return false;
			}
			ArmorAugmentItem.ArmorType armorType = this.m_armorType;
			if (armorType != ArmorAugmentItem.ArmorType.ArmorInner)
			{
				ArmorItem armorItem;
				return armorType == ArmorAugmentItem.ArmorType.ArmorOuter && (targetInstance.Archetype.TryGetAsType(out armorItem) && armorItem.Type.HasArmorCost()) && this.MatchesTypeRestrictions(armorItem.Type);
			}
			ArmorItem armorItem2;
			return targetInstance.Archetype.TryGetAsType(out armorItem2) && !armorItem2.Type.HasArmorCost() && this.MatchesTypeRestrictions(armorItem2.Type);
		}

		// Token: 0x06005452 RID: 21586 RVA: 0x00078621 File Offset: 0x00076821
		private bool MatchesTypeRestrictions(EquipmentType equipmentType)
		{
			return this.m_typeRestrictions == null || this.m_typeRestrictions.Count <= 0 || this.m_typeRestrictions.Contains(equipmentType);
		}

		// Token: 0x06005453 RID: 21587 RVA: 0x00061BE2 File Offset: 0x0005FDE2
		public override CursorType GetCursorType()
		{
			return CursorType.IdentifyingGlassCursor;
		}

		// Token: 0x06005454 RID: 21588 RVA: 0x001DA4F4 File Offset: 0x001D86F4
		protected override string GetRemainingText(ArchetypeInstance instance)
		{
			if (instance != null && instance.ItemData != null && instance.ItemData.Augment != null)
			{
				int arg = this.m_expirationAmount * (int)instance.ItemData.Augment.StackCount - instance.ItemData.Augment.Count;
				return ZString.Format<string, int>("<color={0}>{1}m</color>", UIManager.AugmentColor.ToHex(), arg);
			}
			return string.Empty;
		}

		// Token: 0x06005455 RID: 21589 RVA: 0x001DA560 File Offset: 0x001D8760
		protected override void AddAugmentStatsToTooltip(ArchetypeTooltip tooltip)
		{
			if (this.m_augments != null)
			{
				for (int i = 0; i < this.m_augments.Length; i++)
				{
					this.m_augments[i].AddToTooltipBlock(tooltip.StatsBlock, new Color?(UIManager.AugmentColor));
				}
			}
		}

		// Token: 0x04004B07 RID: 19207
		[SerializeField]
		private ArmorAugmentItem.ArmorType m_armorType;

		// Token: 0x04004B08 RID: 19208
		[SerializeField]
		private List<EquipmentType> m_typeRestrictions;

		// Token: 0x04004B09 RID: 19209
		[SerializeField]
		private StatModifier[] m_augments;

		// Token: 0x02000AA8 RID: 2728
		private enum ArmorType
		{
			// Token: 0x04004B0B RID: 19211
			None,
			// Token: 0x04004B0C RID: 19212
			ArmorInner,
			// Token: 0x04004B0D RID: 19213
			ArmorOuter
		}
	}
}
