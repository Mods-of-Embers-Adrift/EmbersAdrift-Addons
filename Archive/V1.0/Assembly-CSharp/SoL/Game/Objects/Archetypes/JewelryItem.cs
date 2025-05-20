using System;
using SoL.Game.Objects.Containers;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A89 RID: 2697
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Jewelry")]
	public class JewelryItem : EquipableItem
	{
		// Token: 0x17001308 RID: 4872
		// (get) Token: 0x0600538C RID: 21388 RVA: 0x00077C35 File Offset: 0x00075E35
		public override EquipmentType Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x0600538D RID: 21389 RVA: 0x001D83E8 File Offset: 0x001D65E8
		public override EquipmentSlot GetTargetEquipmentSlot(GameEntity entity)
		{
			ContainerInstance containerInstance = null;
			switch (this.Type)
			{
			case EquipmentType.Jewelry_Necklace:
				return EquipmentSlot.Neck;
			case EquipmentType.Jewelry_Ring:
				if (entity.CollectionController.TryGetInstance(ContainerType.Equipment, out containerInstance))
				{
					ArchetypeInstance archetypeInstance;
					if (!containerInstance.TryGetInstanceForIndex(8192, out archetypeInstance))
					{
						return EquipmentSlot.Finger_Left;
					}
					ArchetypeInstance archetypeInstance2;
					if (!containerInstance.TryGetInstanceForIndex(16384, out archetypeInstance2))
					{
						return EquipmentSlot.Finger_Right;
					}
					if (!ClientGameManager.InputManager.HoldingShift)
					{
						return EquipmentSlot.Finger_Left;
					}
					return EquipmentSlot.Finger_Right;
				}
				break;
			case EquipmentType.Jewelry_Earring:
				if (entity.CollectionController.TryGetInstance(ContainerType.Equipment, out containerInstance))
				{
					ArchetypeInstance archetypeInstance3;
					if (!containerInstance.TryGetInstanceForIndex(2048, out archetypeInstance3))
					{
						return EquipmentSlot.Ear_Left;
					}
					ArchetypeInstance archetypeInstance4;
					if (!containerInstance.TryGetInstanceForIndex(4096, out archetypeInstance4))
					{
						return EquipmentSlot.Ear_Right;
					}
					if (!ClientGameManager.InputManager.HoldingShift)
					{
						return EquipmentSlot.Ear_Left;
					}
					return EquipmentSlot.Ear_Right;
				}
				break;
			}
			return EquipmentSlot.None;
		}

		// Token: 0x04004A91 RID: 19089
		[SerializeField]
		private EquipmentType m_type;

		// Token: 0x04004A92 RID: 19090
		private static EquipmentType[] kValidTypes = new EquipmentType[]
		{
			EquipmentType.Jewelry_Ring,
			EquipmentType.Jewelry_Earring,
			EquipmentType.Jewelry_Necklace
		};
	}
}
