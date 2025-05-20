using System;
using System.Collections;
using SoL.Game.Animation;
using SoL.Game.EffectSystem;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000ACE RID: 2766
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Masteries/CombatMastery")]
	public class CombatMasteryArchetype : MasteryArchetype
	{
		// Token: 0x170013BC RID: 5052
		// (get) Token: 0x06005568 RID: 21864 RVA: 0x000790E8 File Offset: 0x000772E8
		protected override StatModifierScaling[] Stats
		{
			get
			{
				return this.m_statModifiers;
			}
		}

		// Token: 0x06005569 RID: 21865 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool MeetsHandheldRequirements(GameEntity entity)
		{
			return false;
		}

		// Token: 0x0600556A RID: 21866 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool MeetsHandheldRequirements(HandheldFlagConfig config)
		{
			return false;
		}

		// Token: 0x0600556B RID: 21867 RVA: 0x001DE9B8 File Offset: 0x001DCBB8
		public bool EntityHasCompatibleWeapons(IHandHeldItems handHeldItems)
		{
			if (handHeldItems == null)
			{
				throw new ArgumentNullException("handHeldItems");
			}
			HandheldFlagConfig handheldFlagConfig = handHeldItems.GetHandheldFlagConfig();
			return this.MeetsHandheldRequirements(handheldFlagConfig);
		}

		// Token: 0x170013BD RID: 5053
		// (get) Token: 0x0600556C RID: 21868 RVA: 0x000790F0 File Offset: 0x000772F0
		public AnimancerAnimationSet Stance
		{
			get
			{
				return this.m_stance;
			}
		}

		// Token: 0x0600556D RID: 21869 RVA: 0x000790F8 File Offset: 0x000772F8
		public virtual bool HasCompatibleWeapons(GameEntity entity, out string compatibleWeaponString)
		{
			compatibleWeaponString = string.Empty;
			return false;
		}

		// Token: 0x0600556E RID: 21870 RVA: 0x000636CE File Offset: 0x000618CE
		private IEnumerable GetAnimSets()
		{
			return SolOdinUtilities.GetDropdownItems<AnimancerAnimationSet>();
		}

		// Token: 0x0600556F RID: 21871 RVA: 0x001DE9E4 File Offset: 0x001DCBE4
		public bool CanSelectMastery(GameEntity entity, bool primary)
		{
			HandheldItemFlags mainHand = HandheldItemFlags.Empty;
			HandheldItemFlags offHand = HandheldItemFlags.Empty;
			EquipmentSlot index = primary ? EquipmentSlot.PrimaryWeapon_MainHand : EquipmentSlot.SecondaryWeapon_MainHand;
			EquipmentSlot index2 = primary ? EquipmentSlot.PrimaryWeapon_OffHand : EquipmentSlot.SecondaryWeapon_OffHand;
			if (entity.CollectionController != null && entity.CollectionController.Equipment != null)
			{
				ArchetypeInstance archetypeInstance;
				IHandheldItem handheldItem;
				if (entity.CollectionController.Equipment.TryGetInstanceForIndex((int)index, out archetypeInstance) && archetypeInstance.Archetype.TryGetAsType(out handheldItem))
				{
					mainHand = handheldItem.HandheldItemFlag;
				}
				if (entity.CollectionController.Equipment.TryGetInstanceForIndex((int)index2, out archetypeInstance) && archetypeInstance.Archetype.TryGetAsType(out handheldItem))
				{
					offHand = handheldItem.HandheldItemFlag;
				}
			}
			HandheldFlagConfig config = new HandheldFlagConfig
			{
				MainHand = mainHand,
				OffHand = offHand
			};
			return this.MeetsHandheldRequirements(config);
		}

		// Token: 0x06005570 RID: 21872 RVA: 0x001DEA98 File Offset: 0x001DCC98
		public override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			if (!entity)
			{
				return;
			}
			base.FillTooltipBlocks(tooltip, instance, entity);
			TooltipTextBlock dataBlock = tooltip.DataBlock;
			int num = Mathf.FloorToInt(base.GetCurrentLevel(entity));
			int? level = null;
			if (num > 0 && instance != null)
			{
				level = new int?(num);
			}
			IStatModifierExtensions.FillTooltipBlock(dataBlock, level, this.m_statModifiers);
		}

		// Token: 0x04004BD7 RID: 19415
		private const string kActiveModifiersGroupName = "Modifiers/Active (combat stance)";

		// Token: 0x04004BD8 RID: 19416
		[SerializeField]
		private StatModifierScaling[] m_statModifiers;

		// Token: 0x04004BD9 RID: 19417
		[SerializeField]
		private AnimancerAnimationSet m_stance;

		// Token: 0x04004BDA RID: 19418
		private const string kWeaponGroupName = "Weapon Requirements";

		// Token: 0x04004BDB RID: 19419
		private const string kPrimaryWeaponGroupName = "Weapon Requirements/Primary";

		// Token: 0x04004BDC RID: 19420
		private const string kSecondaryWeaponGroupName = "Weapon Requirements/Secondary";
	}
}
