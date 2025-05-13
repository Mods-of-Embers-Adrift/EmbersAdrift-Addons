using System;
using SoL.Game.Audio;
using SoL.Game.EffectSystem;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000ADF RID: 2783
	[Obsolete]
	[CreateAssetMenu(menuName = "SoL/Profiles/Armor")]
	public class ArmorProfile : BaseArchetype
	{
		// Token: 0x060055CD RID: 21965 RVA: 0x00049FFA File Offset: 0x000481FA
		public override ArchetypeInstance CreateNewInstance()
		{
			return null;
		}

		// Token: 0x170013DF RID: 5087
		// (get) Token: 0x060055CE RID: 21966 RVA: 0x0007942A File Offset: 0x0007762A
		public override AudioClipCollection DragDropAudio
		{
			get
			{
				return this.m_dragDropAudio;
			}
		}

		// Token: 0x060055CF RID: 21967 RVA: 0x001DFA4C File Offset: 0x001DDC4C
		public bool TryGetArmorPieceProfile(EquipmentType eqType, out ArmorPieceProfile profile)
		{
			profile = null;
			switch (eqType)
			{
			case EquipmentType.Jewelry_Necklace:
				profile = this.m_neck;
				break;
			case EquipmentType.Jewelry_Ring:
				profile = this.m_finger;
				break;
			case EquipmentType.Jewelry_Earring:
				profile = this.m_ear;
				break;
			default:
				switch (eqType)
				{
				case EquipmentType.Head:
					profile = this.m_helm;
					break;
				case EquipmentType.Mask:
					break;
				case EquipmentType.Back:
					profile = this.m_back;
					break;
				case EquipmentType.Waist:
					profile = this.m_belt;
					break;
				default:
					switch (eqType)
					{
					case EquipmentType.Clothing_Chest:
					case EquipmentType.Armor_Chest:
						profile = this.m_chest;
						break;
					case EquipmentType.Clothing_Hands:
					case EquipmentType.Armor_Hands:
						profile = this.m_gloves;
						break;
					case EquipmentType.Clothing_Legs:
					case EquipmentType.Armor_Legs:
						profile = this.m_pants;
						break;
					case EquipmentType.Clothing_Feet:
					case EquipmentType.Armor_Feet:
						profile = this.m_boots;
						break;
					case EquipmentType.Armor_Shoulders:
						profile = this.m_shoulders;
						break;
					}
					break;
				}
				break;
			}
			return profile != null;
		}

		// Token: 0x060055D0 RID: 21968 RVA: 0x001DFB48 File Offset: 0x001DDD48
		private void AssignToAllProfiles()
		{
			this.AssignValuesToProfile(this.m_helm);
			this.AssignValuesToProfile(this.m_chest);
			this.AssignValuesToProfile(this.m_gloves);
			this.AssignValuesToProfile(this.m_shoulders);
			this.AssignValuesToProfile(this.m_pants);
			this.AssignValuesToProfile(this.m_boots);
			this.AssignValuesToProfile(this.m_back);
			this.AssignValuesToProfile(this.m_belt);
			this.AssignValuesToProfile(this.m_wrists);
			this.AssignValuesToProfile(this.m_neck);
			this.AssignValuesToProfile(this.m_finger);
			this.AssignValuesToProfile(this.m_ear);
		}

		// Token: 0x060055D1 RID: 21969 RVA: 0x0004475B File Offset: 0x0004295B
		private void AssignValuesToProfile(ArmorPieceProfile profile)
		{
		}

		// Token: 0x04004C1C RID: 19484
		[SerializeField]
		private ArmorPieceProfile m_helm;

		// Token: 0x04004C1D RID: 19485
		[SerializeField]
		private ArmorPieceProfile m_chest;

		// Token: 0x04004C1E RID: 19486
		[SerializeField]
		private ArmorPieceProfile m_gloves;

		// Token: 0x04004C1F RID: 19487
		[SerializeField]
		private ArmorPieceProfile m_shoulders;

		// Token: 0x04004C20 RID: 19488
		[SerializeField]
		private ArmorPieceProfile m_pants;

		// Token: 0x04004C21 RID: 19489
		[SerializeField]
		private ArmorPieceProfile m_boots;

		// Token: 0x04004C22 RID: 19490
		[SerializeField]
		private ArmorPieceProfile m_back;

		// Token: 0x04004C23 RID: 19491
		[SerializeField]
		private ArmorPieceProfile m_belt;

		// Token: 0x04004C24 RID: 19492
		[SerializeField]
		private ArmorPieceProfile m_wrists;

		// Token: 0x04004C25 RID: 19493
		[SerializeField]
		private ArmorPieceProfile m_neck;

		// Token: 0x04004C26 RID: 19494
		[SerializeField]
		private ArmorPieceProfile m_finger;

		// Token: 0x04004C27 RID: 19495
		[SerializeField]
		private ArmorPieceProfile m_ear;

		// Token: 0x04004C28 RID: 19496
		[SerializeField]
		private AudioClipCollection m_dragDropAudio;

		// Token: 0x04004C29 RID: 19497
		[SerializeField]
		private StatModifier[] m_statModifiersToAssign;
	}
}
