using System;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x0200061B RID: 1563
	public static class HumanoidReferencePointsExtensions
	{
		// Token: 0x06003192 RID: 12690 RVA: 0x0015D0DC File Offset: 0x0015B2DC
		public static Transform GetParent(this HumanoidReferencePoints referencePoints, MountPosition mountPosition, EquipmentSlot slot)
		{
			switch (mountPosition)
			{
			case MountPosition.DynamicHand:
				switch (slot)
				{
				case EquipmentSlot.PrimaryWeapon_MainHand:
				case EquipmentSlot.SecondaryWeapon_MainHand:
					return referencePoints.RightMount.transform;
				case EquipmentSlot.PrimaryWeapon_OffHand:
					break;
				case EquipmentSlot.PrimaryWeapon_MainHand | EquipmentSlot.PrimaryWeapon_OffHand:
					goto IL_73;
				default:
					if (slot != EquipmentSlot.SecondaryWeapon_OffHand && slot != EquipmentSlot.LightSource)
					{
						goto IL_73;
					}
					break;
				}
				return referencePoints.LeftMount.transform;
				IL_73:
				throw new ArgumentException("Invalid SlotType for " + mountPosition.ToString() + "! slot");
			case MountPosition.LeftHand:
				return referencePoints.LeftMount.transform;
			case MountPosition.RightHand:
				return referencePoints.RightMount.transform;
			case MountPosition.DynamicHip:
				switch (slot)
				{
				case EquipmentSlot.PrimaryWeapon_MainHand:
				case EquipmentSlot.SecondaryWeapon_MainHand:
					return referencePoints.LeftHipMount.transform;
				case EquipmentSlot.PrimaryWeapon_OffHand:
					break;
				case EquipmentSlot.PrimaryWeapon_MainHand | EquipmentSlot.PrimaryWeapon_OffHand:
					goto IL_EB;
				default:
					if (slot != EquipmentSlot.SecondaryWeapon_OffHand && slot != EquipmentSlot.LightSource)
					{
						goto IL_EB;
					}
					break;
				}
				return referencePoints.RightHipMount.transform;
				IL_EB:
				throw new ArgumentException("Invalid SlotType for " + mountPosition.ToString() + "! slot");
			case MountPosition.LeftHip:
				return referencePoints.LeftHipMount.transform;
			case MountPosition.RightHip:
				return referencePoints.RightHipMount.transform;
			case MountPosition.DynamicShoulder:
				switch (slot)
				{
				case EquipmentSlot.PrimaryWeapon_MainHand:
				case EquipmentSlot.SecondaryWeapon_MainHand:
					return referencePoints.RightShoulderMount.transform;
				case EquipmentSlot.PrimaryWeapon_OffHand:
					break;
				case EquipmentSlot.PrimaryWeapon_MainHand | EquipmentSlot.PrimaryWeapon_OffHand:
					goto IL_163;
				default:
					if (slot != EquipmentSlot.SecondaryWeapon_OffHand && slot != EquipmentSlot.LightSource)
					{
						goto IL_163;
					}
					break;
				}
				return referencePoints.LeftShoulderMount.transform;
				IL_163:
				throw new ArgumentException("Invalid SlotType for " + mountPosition.ToString() + "! slot");
			case MountPosition.LeftShoulder:
				return referencePoints.LeftShoulderMount.transform;
			case MountPosition.RightShoulder:
				return referencePoints.RightShoulderMount.transform;
			case MountPosition.Back:
				return referencePoints.BackMount.transform;
			default:
				return null;
			}
		}
	}
}
