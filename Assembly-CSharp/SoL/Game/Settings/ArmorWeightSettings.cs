using System;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x02000719 RID: 1817
	[Serializable]
	public class ArmorWeightSettings
	{
		// Token: 0x06003687 RID: 13959 RVA: 0x00169CBC File Offset: 0x00167EBC
		public int GetArmorWeight(EquipmentType type, int interpolator)
		{
			if (!type.HasArmorCost())
			{
				return 0;
			}
			MinMaxIntRange rangeForSlot = this.GetRangeForSlot(type);
			if (interpolator <= 0)
			{
				return rangeForSlot.Min;
			}
			if (interpolator >= 100)
			{
				return rangeForSlot.Max;
			}
			return Mathf.FloorToInt(Mathf.Lerp((float)rangeForSlot.Min, (float)rangeForSlot.Max, (float)interpolator * 0.01f));
		}

		// Token: 0x06003688 RID: 13960 RVA: 0x00169D18 File Offset: 0x00167F18
		public MinMaxIntRange GetRangeForSlot(EquipmentType type)
		{
			if (type == EquipmentType.Head)
			{
				return this.m_head;
			}
			switch (type)
			{
			case EquipmentType.Armor_Shoulders:
				return this.m_shoulder;
			case EquipmentType.Armor_Chest:
				return this.m_chest;
			case EquipmentType.Armor_Hands:
				return this.m_hands;
			case EquipmentType.Armor_Legs:
				return this.m_legs;
			case EquipmentType.Armor_Feet:
				return this.m_feet;
			default:
				return default(MinMaxIntRange);
			}
		}

		// Token: 0x06003689 RID: 13961 RVA: 0x00169D7C File Offset: 0x00167F7C
		public MinMaxIntRange? GetRangeForSlot(EquipmentSlot slot)
		{
			if (slot <= EquipmentSlot.Armor_Chest)
			{
				if (slot <= EquipmentSlot.Armor_Shoulder_L)
				{
					if (slot == EquipmentSlot.Head)
					{
						return new MinMaxIntRange?(this.m_head);
					}
					if (slot != EquipmentSlot.Armor_Shoulder_L)
					{
						goto IL_B3;
					}
				}
				else if (slot != EquipmentSlot.Armor_Shoulder_R)
				{
					if (slot != EquipmentSlot.Armor_Chest)
					{
						goto IL_B3;
					}
					return new MinMaxIntRange?(this.m_chest);
				}
				return new MinMaxIntRange?(this.m_shoulder);
			}
			if (slot <= EquipmentSlot.Armor_Hand_R)
			{
				if (slot == EquipmentSlot.Armor_Hand_L || slot == EquipmentSlot.Armor_Hand_R)
				{
					return new MinMaxIntRange?(this.m_hands);
				}
			}
			else
			{
				if (slot == EquipmentSlot.Armor_Legs)
				{
					return new MinMaxIntRange?(this.m_legs);
				}
				if (slot == EquipmentSlot.Armor_Feet_L || slot == EquipmentSlot.Armor_Feet_R)
				{
					return new MinMaxIntRange?(this.m_feet);
				}
			}
			IL_B3:
			return null;
		}

		// Token: 0x04003481 RID: 13441
		public const float kArmorWeightModifierCap = -80f;

		// Token: 0x04003482 RID: 13442
		public const int kMinArmorInterpolator = 0;

		// Token: 0x04003483 RID: 13443
		public const int kMaxArmorInterpolator = 100;

		// Token: 0x04003484 RID: 13444
		private const float kMaxArmorInterpolatorInverted = 0.01f;

		// Token: 0x04003485 RID: 13445
		[SerializeField]
		private MinMaxIntRange m_head = new MinMaxIntRange(1, 10);

		// Token: 0x04003486 RID: 13446
		[SerializeField]
		private MinMaxIntRange m_shoulder = new MinMaxIntRange(1, 10);

		// Token: 0x04003487 RID: 13447
		[SerializeField]
		private MinMaxIntRange m_chest = new MinMaxIntRange(1, 10);

		// Token: 0x04003488 RID: 13448
		[SerializeField]
		private MinMaxIntRange m_hands = new MinMaxIntRange(1, 10);

		// Token: 0x04003489 RID: 13449
		[SerializeField]
		private MinMaxIntRange m_legs = new MinMaxIntRange(1, 10);

		// Token: 0x0400348A RID: 13450
		[SerializeField]
		private MinMaxIntRange m_feet = new MinMaxIntRange(1, 10);
	}
}
