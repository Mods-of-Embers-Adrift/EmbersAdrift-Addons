using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Utilities;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A7A RID: 2682
	public static class EquipmentExtensions
	{
		// Token: 0x170012D1 RID: 4817
		// (get) Token: 0x060052E6 RID: 21222 RVA: 0x0007747D File Offset: 0x0007567D
		public static EquipmentSlot[] EquipmentSlots
		{
			get
			{
				if (EquipmentExtensions.m_equipmentSlots == null)
				{
					EquipmentExtensions.m_equipmentSlots = (EquipmentSlot[])Enum.GetValues(typeof(EquipmentSlot));
				}
				return EquipmentExtensions.m_equipmentSlots;
			}
		}

		// Token: 0x170012D2 RID: 4818
		// (get) Token: 0x060052E7 RID: 21223 RVA: 0x001D5BA8 File Offset: 0x001D3DA8
		public static HashSet<int> ValidEquipmentSlotIndexes
		{
			get
			{
				if (EquipmentExtensions.m_validEquipmentSlotIndexes == null)
				{
					EquipmentExtensions.m_validEquipmentSlotIndexes = new HashSet<int>(EquipmentExtensions.EquipmentSlots.Length);
					for (int i = 0; i < EquipmentExtensions.EquipmentSlots.Length; i++)
					{
						EquipmentExtensions.m_validEquipmentSlotIndexes.Add((int)EquipmentExtensions.EquipmentSlots[i]);
					}
				}
				return EquipmentExtensions.m_validEquipmentSlotIndexes;
			}
		}

		// Token: 0x170012D3 RID: 4819
		// (get) Token: 0x060052E8 RID: 21224 RVA: 0x000774A4 File Offset: 0x000756A4
		public static EquipmentType[] EquipmentTypes
		{
			get
			{
				if (EquipmentExtensions.m_equipmentSlots == null)
				{
					EquipmentExtensions.m_equipmentTypes = (EquipmentType[])Enum.GetValues(typeof(EquipmentType));
				}
				return EquipmentExtensions.m_equipmentTypes;
			}
		}

		// Token: 0x060052E9 RID: 21225 RVA: 0x000774CB File Offset: 0x000756CB
		public static bool IsCompatible(this EquipmentSlot slotType, EquipmentType eqType)
		{
			return eqType.GetAllCompatibleSlots(false).HasBitFlag(slotType);
		}

		// Token: 0x060052EA RID: 21226 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this EquipmentSlot a, EquipmentSlot b)
		{
			return (a & b) == b;
		}

		// Token: 0x060052EB RID: 21227 RVA: 0x000774DA File Offset: 0x000756DA
		public static bool HasAnyBitFlag(this EquipmentSlot a, EquipmentSlot b)
		{
			return (a & b) > EquipmentSlot.None;
		}

		// Token: 0x060052EC RID: 21228 RVA: 0x001D5BF8 File Offset: 0x001D3DF8
		public static bool IsVisible(this EquipmentType type)
		{
			switch (type)
			{
			case EquipmentType.Weapon_Melee_1H:
			case EquipmentType.Weapon_Melee_2H:
			case EquipmentType.Weapon_Shield:
			case EquipmentType.Weapon_Ranged:
			case EquipmentType.Weapon_OffhandAccessory:
			case EquipmentType.LightSource:
				return true;
			case EquipmentType.Ammo_Ranged:
			case (EquipmentType)7:
			case (EquipmentType)8:
			case (EquipmentType)9:
				break;
			case EquipmentType.Tool:
				return true;
			default:
				switch (type)
				{
				case EquipmentType.Head:
				case EquipmentType.Mask:
				case EquipmentType.Back:
				case EquipmentType.Waist:
					return true;
				case EquipmentType.Clothing_Chest:
				case EquipmentType.Clothing_Hands:
				case EquipmentType.Clothing_Legs:
				case EquipmentType.Clothing_Feet:
					return true;
				case EquipmentType.Armor_Shoulders:
				case EquipmentType.Armor_Chest:
				case EquipmentType.Armor_Hands:
				case EquipmentType.Armor_Legs:
				case EquipmentType.Armor_Feet:
					return true;
				}
				break;
			}
			return false;
		}

		// Token: 0x060052ED RID: 21229 RVA: 0x001D5CB4 File Offset: 0x001D3EB4
		public static bool IsHandheldMounted(this EquipmentSlot slot)
		{
			if (slot <= EquipmentSlot.Tool1)
			{
				if (slot <= EquipmentSlot.SecondaryWeapon_MainHand)
				{
					if (slot - EquipmentSlot.PrimaryWeapon_MainHand > 1 && slot != EquipmentSlot.SecondaryWeapon_MainHand)
					{
						return false;
					}
				}
				else if (slot != EquipmentSlot.SecondaryWeapon_OffHand && slot != EquipmentSlot.Tool1)
				{
					return false;
				}
			}
			else if (slot <= EquipmentSlot.Tool3)
			{
				if (slot != EquipmentSlot.Tool2 && slot != EquipmentSlot.Tool3)
				{
					return false;
				}
			}
			else if (slot != EquipmentSlot.Tool4 && slot != EquipmentSlot.LightSource && slot != EquipmentSlot.EmberStone)
			{
				return false;
			}
			return true;
		}

		// Token: 0x060052EE RID: 21230 RVA: 0x000774E2 File Offset: 0x000756E2
		public static bool ContributesArmorClass(this EquipmentType type)
		{
			if (type <= EquipmentType.Waist)
			{
				if (type == EquipmentType.Head || type - EquipmentType.Back <= 1)
				{
					return true;
				}
			}
			else
			{
				if (type - EquipmentType.Clothing_Chest <= 3)
				{
					return true;
				}
				if (type - EquipmentType.Armor_Shoulders <= 4)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060052EF RID: 21231 RVA: 0x0007750E File Offset: 0x0007570E
		public static bool ContributesArmorProfile(this EquipmentType type)
		{
			if (type <= EquipmentType.Head)
			{
				if (type == EquipmentType.Weapon_Shield)
				{
					return true;
				}
				if (type != EquipmentType.Head)
				{
					return false;
				}
			}
			else if (type - EquipmentType.Back > 1)
			{
				if (type - EquipmentType.Clothing_Chest <= 3)
				{
					return true;
				}
				if (type - EquipmentType.Armor_Shoulders > 4)
				{
					return false;
				}
				return true;
			}
			return true;
		}

		// Token: 0x060052F0 RID: 21232 RVA: 0x001D5D10 File Offset: 0x001D3F10
		public static string GetCommaSeparatedListOfCosmeticSlots()
		{
			List<string> fromPool = StaticListPool<string>.GetFromPool();
			for (int i = 0; i < EquipmentExtensions.CosmeticEquipmentTypes.Count; i++)
			{
				fromPool.Add(EquipmentExtensions.CosmeticEquipmentTypes[i].GetDisplayName());
			}
			string result = string.Join(", ", fromPool);
			StaticListPool<string>.ReturnToPool(fromPool);
			return result;
		}

		// Token: 0x060052F1 RID: 21233 RVA: 0x001D5D60 File Offset: 0x001D3F60
		public static EquipmentSlot GetAllCompatibleSlots(this EquipmentType type, bool hasDualWield = false)
		{
			switch (type)
			{
			case EquipmentType.Weapon_Melee_1H:
				return EquipmentSlot.PrimaryWeapon_MainHand | EquipmentSlot.PrimaryWeapon_OffHand | EquipmentSlot.SecondaryWeapon_MainHand | EquipmentSlot.SecondaryWeapon_OffHand;
			case EquipmentType.Weapon_Melee_2H:
			case EquipmentType.Weapon_Ranged:
				return EquipmentSlot.PrimaryWeapon_MainHand | EquipmentSlot.SecondaryWeapon_MainHand;
			case EquipmentType.Weapon_Shield:
			case EquipmentType.Weapon_OffhandAccessory:
			case EquipmentType.Ammo_Ranged:
				return EquipmentSlot.PrimaryWeapon_OffHand | EquipmentSlot.SecondaryWeapon_OffHand;
			case (EquipmentType)7:
			case (EquipmentType)8:
			case (EquipmentType)9:
			case (EquipmentType)13:
			case (EquipmentType)14:
			case (EquipmentType)15:
			case (EquipmentType)16:
			case (EquipmentType)17:
			case (EquipmentType)18:
			case (EquipmentType)19:
				break;
			case EquipmentType.Tool:
				return EquipmentSlot.Tool1 | EquipmentSlot.Tool2 | EquipmentSlot.Tool3 | EquipmentSlot.Tool4;
			case EquipmentType.LightSource:
				return EquipmentSlot.LightSource;
			case EquipmentType.EmberStone:
				return EquipmentSlot.EmberStone;
			case EquipmentType.Jewelry_Necklace:
				return EquipmentSlot.Neck;
			case EquipmentType.Jewelry_Ring:
				return EquipmentSlot.Finger_Left | EquipmentSlot.Finger_Right;
			case EquipmentType.Jewelry_Earring:
				return EquipmentSlot.Ear_Left | EquipmentSlot.Ear_Right;
			default:
				switch (type)
				{
				case EquipmentType.Head:
					return EquipmentSlot.Head;
				case EquipmentType.Mask:
					break;
				case EquipmentType.Back:
					return EquipmentSlot.Back;
				case EquipmentType.Waist:
					return EquipmentSlot.Waist;
				default:
					switch (type)
					{
					case EquipmentType.Clothing_Chest:
						return EquipmentSlot.Clothing_Chest;
					case EquipmentType.Clothing_Hands:
						return EquipmentSlot.Clothing_Hands;
					case EquipmentType.Clothing_Legs:
						return EquipmentSlot.Clothing_Legs;
					case EquipmentType.Clothing_Feet:
						return EquipmentSlot.Clothing_Feet;
					case EquipmentType.Armor_Shoulders:
						return EquipmentSlot.Armor_Shoulder_L | EquipmentSlot.Armor_Shoulder_R;
					case EquipmentType.Armor_Chest:
						return EquipmentSlot.Armor_Chest;
					case EquipmentType.Armor_Hands:
						return EquipmentSlot.Armor_Hand_L | EquipmentSlot.Armor_Hand_R;
					case EquipmentType.Armor_Legs:
						return EquipmentSlot.Armor_Legs;
					case EquipmentType.Armor_Feet:
						return EquipmentSlot.Armor_Feet_L | EquipmentSlot.Armor_Feet_R;
					}
					break;
				}
				break;
			}
			return EquipmentSlot.None;
		}

		// Token: 0x060052F2 RID: 21234 RVA: 0x001D5EA4 File Offset: 0x001D40A4
		public static IEnumerable<EquipmentSlot> GetCachedCompatibleSlots(this EquipmentType type)
		{
			if (EquipmentExtensions.m_compatibleSlotEnumerable == null)
			{
				EquipmentExtensions.m_compatibleSlotEnumerable = new Dictionary<EquipmentType, IEnumerable<EquipmentSlot>>(default(EquipmentTypeComparer));
			}
			IEnumerable<EquipmentSlot> compatibleSlots;
			if (!EquipmentExtensions.m_compatibleSlotEnumerable.TryGetValue(type, out compatibleSlots))
			{
				compatibleSlots = type.GetCompatibleSlots(false);
				EquipmentExtensions.m_compatibleSlotEnumerable.Add(type, compatibleSlots);
			}
			return compatibleSlots;
		}

		// Token: 0x060052F3 RID: 21235 RVA: 0x00077540 File Offset: 0x00075740
		private static IEnumerable<EquipmentSlot> GetCompatibleSlots(this EquipmentType type, bool hasDualWield = false)
		{
			switch (type)
			{
			case EquipmentType.Weapon_Melee_1H:
				yield return EquipmentSlot.PrimaryWeapon_MainHand;
				yield return EquipmentSlot.PrimaryWeapon_OffHand;
				yield return EquipmentSlot.SecondaryWeapon_MainHand;
				yield return EquipmentSlot.SecondaryWeapon_OffHand;
				yield break;
			case EquipmentType.Weapon_Melee_2H:
			case EquipmentType.Weapon_Ranged:
				yield return EquipmentSlot.PrimaryWeapon_MainHand;
				yield return EquipmentSlot.SecondaryWeapon_MainHand;
				yield break;
			case EquipmentType.Weapon_Shield:
			case EquipmentType.Weapon_OffhandAccessory:
			case EquipmentType.Ammo_Ranged:
				yield return EquipmentSlot.PrimaryWeapon_OffHand;
				yield return EquipmentSlot.SecondaryWeapon_OffHand;
				yield break;
			case (EquipmentType)7:
			case (EquipmentType)8:
			case (EquipmentType)9:
			case (EquipmentType)13:
			case (EquipmentType)14:
			case (EquipmentType)15:
			case (EquipmentType)16:
			case (EquipmentType)17:
			case (EquipmentType)18:
			case (EquipmentType)19:
				break;
			case EquipmentType.Tool:
				yield return EquipmentSlot.Tool1;
				yield return EquipmentSlot.Tool2;
				yield return EquipmentSlot.Tool3;
				yield return EquipmentSlot.Tool4;
				yield break;
			case EquipmentType.LightSource:
				yield return EquipmentSlot.LightSource;
				yield break;
			case EquipmentType.EmberStone:
				yield return EquipmentSlot.EmberStone;
				yield break;
			case EquipmentType.Jewelry_Necklace:
				yield return EquipmentSlot.Neck;
				yield break;
			case EquipmentType.Jewelry_Ring:
				yield return EquipmentSlot.Finger_Left;
				yield return EquipmentSlot.Finger_Right;
				yield break;
			case EquipmentType.Jewelry_Earring:
				yield return EquipmentSlot.Ear_Left;
				yield return EquipmentSlot.Ear_Right;
				yield break;
			default:
				switch (type)
				{
				case EquipmentType.Head:
					yield return EquipmentSlot.Head;
					yield break;
				case EquipmentType.Mask:
					break;
				case EquipmentType.Back:
					yield return EquipmentSlot.Back;
					yield break;
				case EquipmentType.Waist:
					yield return EquipmentSlot.Waist;
					yield break;
				default:
					switch (type)
					{
					case EquipmentType.Clothing_Chest:
						yield return EquipmentSlot.Clothing_Chest;
						yield break;
					case EquipmentType.Clothing_Hands:
						yield return EquipmentSlot.Clothing_Hands;
						yield break;
					case EquipmentType.Clothing_Legs:
						yield return EquipmentSlot.Clothing_Legs;
						yield break;
					case EquipmentType.Clothing_Feet:
						yield return EquipmentSlot.Clothing_Feet;
						yield break;
					case EquipmentType.Armor_Shoulders:
						yield return EquipmentSlot.Armor_Shoulder_L;
						yield return EquipmentSlot.Armor_Shoulder_R;
						yield break;
					case EquipmentType.Armor_Chest:
						yield return EquipmentSlot.Armor_Chest;
						yield break;
					case EquipmentType.Armor_Hands:
						yield return EquipmentSlot.Armor_Hand_L;
						yield return EquipmentSlot.Armor_Hand_R;
						yield break;
					case EquipmentType.Armor_Legs:
						yield return EquipmentSlot.Armor_Legs;
						yield break;
					case EquipmentType.Armor_Feet:
						yield return EquipmentSlot.Armor_Feet_L;
						yield return EquipmentSlot.Armor_Feet_R;
						yield break;
					}
					break;
				}
				break;
			}
			yield return EquipmentSlot.None;
			yield break;
		}

		// Token: 0x060052F4 RID: 21236 RVA: 0x001D5EF4 File Offset: 0x001D40F4
		public static string GetDisplayName(this EquipmentType type)
		{
			switch (type)
			{
			case EquipmentType.Weapon_Melee_1H:
				return "1H";
			case EquipmentType.Weapon_Melee_2H:
				return "2H";
			case EquipmentType.Weapon_Shield:
				return "Shield";
			case EquipmentType.Weapon_Ranged:
				return "Ranged";
			case EquipmentType.Weapon_OffhandAccessory:
				return "Offhand Accessory";
			case EquipmentType.Ammo_Ranged:
				return "Ammo";
			case (EquipmentType)7:
			case (EquipmentType)8:
			case (EquipmentType)9:
			case (EquipmentType)13:
			case (EquipmentType)14:
			case (EquipmentType)15:
			case (EquipmentType)16:
			case (EquipmentType)17:
			case (EquipmentType)18:
			case (EquipmentType)19:
				break;
			case EquipmentType.Tool:
				return "Tool";
			case EquipmentType.LightSource:
				return "Light";
			case EquipmentType.EmberStone:
				return "Ember Stone";
			case EquipmentType.Jewelry_Necklace:
				return "Necklace";
			case EquipmentType.Jewelry_Ring:
				return "Ring";
			case EquipmentType.Jewelry_Earring:
				return "Earring";
			default:
				switch (type)
				{
				case EquipmentType.Head:
					return "Head";
				case EquipmentType.Mask:
					return "Mask";
				case EquipmentType.Back:
					return "Utility";
				case EquipmentType.Waist:
					return "Waist";
				case EquipmentType.Clothing_Chest:
					return "Chest";
				case EquipmentType.Clothing_Hands:
					return "Hands";
				case EquipmentType.Clothing_Legs:
					return "Legs";
				case EquipmentType.Clothing_Feet:
					return "Feet";
				case EquipmentType.Armor_Shoulders:
					return "Pauldron";
				case EquipmentType.Armor_Chest:
					return "Cuirass";
				case EquipmentType.Armor_Hands:
					return "Vambrace";
				case EquipmentType.Armor_Legs:
					return "Faulds";
				case EquipmentType.Armor_Feet:
					return "Greaves";
				}
				break;
			}
			return "Unknown";
		}

		// Token: 0x060052F5 RID: 21237 RVA: 0x001D6070 File Offset: 0x001D4270
		public static string GetDisplayName(this EquipmentSlot slot)
		{
			if (slot <= EquipmentSlot.Head)
			{
				if (slot <= EquipmentSlot.LightSource)
				{
					if (slot <= EquipmentSlot.Tool1)
					{
						switch (slot)
						{
						case EquipmentSlot.PrimaryWeapon_MainHand:
							return "Primary Main Hand";
						case EquipmentSlot.PrimaryWeapon_OffHand:
							return "Primary Off Hand";
						case EquipmentSlot.PrimaryWeapon_MainHand | EquipmentSlot.PrimaryWeapon_OffHand:
							goto IL_2A3;
						case EquipmentSlot.SecondaryWeapon_MainHand:
							return "Secondary Main Hand";
						default:
							if (slot == EquipmentSlot.SecondaryWeapon_OffHand)
							{
								return "Secondary Off Hand";
							}
							if (slot != EquipmentSlot.Tool1)
							{
								goto IL_2A3;
							}
							break;
						}
					}
					else if (slot <= EquipmentSlot.Tool3)
					{
						if (slot != EquipmentSlot.Tool2 && slot != EquipmentSlot.Tool3)
						{
							goto IL_2A3;
						}
					}
					else if (slot != EquipmentSlot.Tool4)
					{
						if (slot != EquipmentSlot.LightSource)
						{
							goto IL_2A3;
						}
						return EquipmentType.LightSource.GetDisplayName();
					}
					return EquipmentType.Tool.GetDisplayName();
				}
				if (slot > EquipmentSlot.Ear_Left)
				{
					if (slot <= EquipmentSlot.Finger_Left)
					{
						if (slot == EquipmentSlot.Ear_Right)
						{
							goto IL_215;
						}
						if (slot != EquipmentSlot.Finger_Left)
						{
							goto IL_2A3;
						}
					}
					else if (slot != EquipmentSlot.Finger_Right)
					{
						if (slot != EquipmentSlot.Head)
						{
							goto IL_2A3;
						}
						return EquipmentType.Head.GetDisplayName();
					}
					return EquipmentType.Jewelry_Ring.GetDisplayName();
				}
				if (slot == EquipmentSlot.EmberStone)
				{
					return EquipmentType.EmberStone.GetDisplayName();
				}
				if (slot == EquipmentSlot.Neck)
				{
					return EquipmentType.Jewelry_Necklace.GetDisplayName();
				}
				if (slot != EquipmentSlot.Ear_Left)
				{
					goto IL_2A3;
				}
				IL_215:
				return EquipmentType.Jewelry_Earring.GetDisplayName();
			}
			else if (slot <= EquipmentSlot.Clothing_Feet)
			{
				if (slot <= EquipmentSlot.Waist)
				{
					if (slot == EquipmentSlot.Cosmetic)
					{
						return "Cosmetic";
					}
					if (slot == EquipmentSlot.Back)
					{
						return EquipmentType.Back.GetDisplayName();
					}
					if (slot == EquipmentSlot.Waist)
					{
						return EquipmentType.Waist.GetDisplayName();
					}
				}
				else if (slot <= EquipmentSlot.Clothing_Hands)
				{
					if (slot == EquipmentSlot.Clothing_Chest)
					{
						return EquipmentType.Clothing_Chest.GetDisplayName();
					}
					if (slot == EquipmentSlot.Clothing_Hands)
					{
						return EquipmentType.Clothing_Hands.GetDisplayName();
					}
				}
				else
				{
					if (slot == EquipmentSlot.Clothing_Legs)
					{
						return EquipmentType.Clothing_Legs.GetDisplayName();
					}
					if (slot == EquipmentSlot.Clothing_Feet)
					{
						return EquipmentType.Clothing_Feet.GetDisplayName();
					}
				}
			}
			else if (slot <= EquipmentSlot.Armor_Hand_L)
			{
				if (slot <= EquipmentSlot.Armor_Shoulder_R)
				{
					if (slot == EquipmentSlot.Armor_Shoulder_L)
					{
						return EquipmentType.Armor_Shoulders.GetDisplayName();
					}
					if (slot == EquipmentSlot.Armor_Shoulder_R)
					{
						return EquipmentType.Armor_Shoulders.GetDisplayName();
					}
				}
				else
				{
					if (slot == EquipmentSlot.Armor_Chest)
					{
						return EquipmentType.Armor_Chest.GetDisplayName();
					}
					if (slot == EquipmentSlot.Armor_Hand_L)
					{
						return EquipmentType.Armor_Hands.GetDisplayName();
					}
				}
			}
			else if (slot <= EquipmentSlot.Armor_Legs)
			{
				if (slot == EquipmentSlot.Armor_Hand_R)
				{
					return EquipmentType.Armor_Hands.GetDisplayName();
				}
				if (slot == EquipmentSlot.Armor_Legs)
				{
					return EquipmentType.Armor_Legs.GetDisplayName();
				}
			}
			else
			{
				if (slot == EquipmentSlot.Armor_Feet_L)
				{
					return EquipmentType.Armor_Feet.GetDisplayName();
				}
				if (slot == EquipmentSlot.Armor_Feet_R)
				{
					return EquipmentType.Armor_Feet.GetDisplayName();
				}
			}
			IL_2A3:
			return "Unknown";
		}

		// Token: 0x060052F6 RID: 21238 RVA: 0x00077550 File Offset: 0x00075750
		public static bool TriggerWeaponSwapCooldownOnChange(this EquipmentSlot slot)
		{
			return slot - EquipmentSlot.PrimaryWeapon_MainHand <= 1 || slot == EquipmentSlot.SecondaryWeapon_MainHand || slot == EquipmentSlot.SecondaryWeapon_OffHand;
		}

		// Token: 0x060052F7 RID: 21239 RVA: 0x00077563 File Offset: 0x00075763
		public static bool HasLeftRightRecipe(this EquipmentType equipType)
		{
			switch (equipType)
			{
			case EquipmentType.Armor_Shoulders:
			case EquipmentType.Armor_Hands:
			case EquipmentType.Armor_Feet:
				return true;
			}
			return false;
		}

		// Token: 0x060052F8 RID: 21240 RVA: 0x001D6328 File Offset: 0x001D4528
		public static EquipmentSlot GetLeftSlotVariant(this EquipmentSlot equipmentSlot)
		{
			if (equipmentSlot <= EquipmentSlot.Armor_Hand_L)
			{
				if (equipmentSlot == EquipmentSlot.Armor_Shoulder_L || equipmentSlot == EquipmentSlot.Armor_Shoulder_R)
				{
					return EquipmentSlot.Armor_Shoulder_L;
				}
				if (equipmentSlot != EquipmentSlot.Armor_Hand_L)
				{
					return EquipmentSlot.None;
				}
			}
			else if (equipmentSlot != EquipmentSlot.Armor_Hand_R)
			{
				if (equipmentSlot == EquipmentSlot.Armor_Feet_L || equipmentSlot == EquipmentSlot.Armor_Feet_R)
				{
					return EquipmentSlot.Armor_Feet_L;
				}
				return EquipmentSlot.None;
			}
			return EquipmentSlot.Armor_Hand_L;
		}

		// Token: 0x060052F9 RID: 21241 RVA: 0x001D6384 File Offset: 0x001D4584
		public static EquipmentSlot GetRightSlotVariant(this EquipmentSlot equipmentSlot)
		{
			if (equipmentSlot <= EquipmentSlot.Armor_Hand_L)
			{
				if (equipmentSlot == EquipmentSlot.Armor_Shoulder_L || equipmentSlot == EquipmentSlot.Armor_Shoulder_R)
				{
					return EquipmentSlot.Armor_Shoulder_R;
				}
				if (equipmentSlot != EquipmentSlot.Armor_Hand_L)
				{
					return EquipmentSlot.None;
				}
			}
			else if (equipmentSlot != EquipmentSlot.Armor_Hand_R)
			{
				if (equipmentSlot == EquipmentSlot.Armor_Feet_L || equipmentSlot == EquipmentSlot.Armor_Feet_R)
				{
					return EquipmentSlot.Armor_Feet_R;
				}
				return EquipmentSlot.None;
			}
			return EquipmentSlot.Armor_Hand_R;
		}

		// Token: 0x060052FA RID: 21242 RVA: 0x00077587 File Offset: 0x00075787
		public static bool IsLeftVariant(this EquipmentSlot equipmentSlot)
		{
			return equipmentSlot == EquipmentSlot.Armor_Shoulder_L || equipmentSlot == EquipmentSlot.Armor_Hand_L || equipmentSlot == EquipmentSlot.Armor_Feet_L;
		}

		// Token: 0x060052FB RID: 21243 RVA: 0x000775A4 File Offset: 0x000757A4
		public static bool IsRightVariant(this EquipmentSlot equipmentSlot)
		{
			return equipmentSlot == EquipmentSlot.Armor_Shoulder_R || equipmentSlot == EquipmentSlot.Armor_Hand_R || equipmentSlot == EquipmentSlot.Armor_Feet_R;
		}

		// Token: 0x060052FC RID: 21244 RVA: 0x001D63E0 File Offset: 0x001D45E0
		public static bool HasArmorCost(this EquipmentSlot equipmentSlot)
		{
			if (equipmentSlot <= EquipmentSlot.Armor_Chest)
			{
				if (equipmentSlot <= EquipmentSlot.Armor_Shoulder_L)
				{
					if (equipmentSlot != EquipmentSlot.Head && equipmentSlot != EquipmentSlot.Armor_Shoulder_L)
					{
						return false;
					}
				}
				else if (equipmentSlot != EquipmentSlot.Armor_Shoulder_R && equipmentSlot != EquipmentSlot.Armor_Chest)
				{
					return false;
				}
			}
			else if (equipmentSlot <= EquipmentSlot.Armor_Hand_R)
			{
				if (equipmentSlot != EquipmentSlot.Armor_Hand_L && equipmentSlot != EquipmentSlot.Armor_Hand_R)
				{
					return false;
				}
			}
			else if (equipmentSlot != EquipmentSlot.Armor_Legs && equipmentSlot != EquipmentSlot.Armor_Feet_L && equipmentSlot != EquipmentSlot.Armor_Feet_R)
			{
				return false;
			}
			return true;
		}

		// Token: 0x060052FD RID: 21245 RVA: 0x001D6458 File Offset: 0x001D4658
		public static bool ShowArmorWeightOnTooltip(this EquipmentSlot equipmentSlot)
		{
			if (equipmentSlot <= EquipmentSlot.Tool1)
			{
				if (equipmentSlot <= EquipmentSlot.SecondaryWeapon_MainHand)
				{
					if (equipmentSlot - EquipmentSlot.PrimaryWeapon_MainHand > 1 && equipmentSlot != EquipmentSlot.SecondaryWeapon_MainHand)
					{
						return true;
					}
				}
				else if (equipmentSlot != EquipmentSlot.SecondaryWeapon_OffHand && equipmentSlot != EquipmentSlot.Tool1)
				{
					return true;
				}
			}
			else if (equipmentSlot <= EquipmentSlot.Tool3)
			{
				if (equipmentSlot != EquipmentSlot.Tool2 && equipmentSlot != EquipmentSlot.Tool3)
				{
					return true;
				}
			}
			else if (equipmentSlot != EquipmentSlot.Tool4 && equipmentSlot != EquipmentSlot.LightSource && equipmentSlot != EquipmentSlot.Cosmetic)
			{
				return true;
			}
			return false;
		}

		// Token: 0x060052FE RID: 21246 RVA: 0x000775C1 File Offset: 0x000757C1
		public static bool HasArmorCost(this EquipmentType equipmentType)
		{
			return equipmentType == EquipmentType.Head || equipmentType - EquipmentType.Armor_Shoulders <= 4;
		}

		// Token: 0x060052FF RID: 21247 RVA: 0x001D64B4 File Offset: 0x001D46B4
		public static bool IsWeaponSlot(this EquipmentType equipmentType)
		{
			EquipmentSlot allCompatibleSlots = equipmentType.GetAllCompatibleSlots(false);
			return allCompatibleSlots != EquipmentSlot.None && (allCompatibleSlots & (EquipmentSlot.PrimaryWeapon_MainHand | EquipmentSlot.PrimaryWeapon_OffHand | EquipmentSlot.SecondaryWeapon_MainHand | EquipmentSlot.SecondaryWeapon_OffHand)) > EquipmentSlot.None;
		}

		// Token: 0x06005300 RID: 21248 RVA: 0x000775D2 File Offset: 0x000757D2
		public static bool IsWeaponSlot(this EquipmentSlot slot)
		{
			return slot != EquipmentSlot.None && (slot & (EquipmentSlot.PrimaryWeapon_MainHand | EquipmentSlot.PrimaryWeapon_OffHand | EquipmentSlot.SecondaryWeapon_MainHand | EquipmentSlot.SecondaryWeapon_OffHand)) > EquipmentSlot.None;
		}

		// Token: 0x06005301 RID: 21249 RVA: 0x000775E0 File Offset: 0x000757E0
		public static bool BlockOffhandSlot(this EquipmentType equipmentType)
		{
			return equipmentType == EquipmentType.Weapon_Melee_2H;
		}

		// Token: 0x06005302 RID: 21250 RVA: 0x000775E9 File Offset: 0x000757E9
		public static bool IsToolSlot(this EquipmentSlot slot)
		{
			if (slot <= EquipmentSlot.Tool2)
			{
				if (slot != EquipmentSlot.Tool1 && slot != EquipmentSlot.Tool2)
				{
					return false;
				}
			}
			else if (slot != EquipmentSlot.Tool3 && slot != EquipmentSlot.Tool4)
			{
				return false;
			}
			return true;
		}

		// Token: 0x06005303 RID: 21251 RVA: 0x0007760C File Offset: 0x0007580C
		public static bool IncludeInInspection(this EquipmentSlot slot)
		{
			if (slot <= EquipmentSlot.Tool3)
			{
				if (slot != EquipmentSlot.Tool1 && slot != EquipmentSlot.Tool2 && slot != EquipmentSlot.Tool3)
				{
					return true;
				}
			}
			else if (slot != EquipmentSlot.Tool4 && slot != EquipmentSlot.LightSource && slot != EquipmentSlot.EmberStone)
			{
				return true;
			}
			return false;
		}

		// Token: 0x170012D4 RID: 4820
		// (get) Token: 0x06005304 RID: 21252 RVA: 0x001D64D8 File Offset: 0x001D46D8
		public static Dictionary<int, EquipmentSlot> IndexToSlotDict
		{
			get
			{
				if (EquipmentExtensions.m_indexToSlotDict == null)
				{
					EquipmentExtensions.m_indexToSlotDict = new Dictionary<int, EquipmentSlot>(EquipmentExtensions.EquipmentSlots.Length);
					for (int i = 0; i < EquipmentExtensions.EquipmentSlots.Length; i++)
					{
						EquipmentExtensions.m_indexToSlotDict.Add((int)EquipmentExtensions.EquipmentSlots[i], EquipmentExtensions.EquipmentSlots[i]);
					}
				}
				return EquipmentExtensions.m_indexToSlotDict;
			}
		}

		// Token: 0x06005305 RID: 21253 RVA: 0x001D652C File Offset: 0x001D472C
		public static string GetSlotDescriptionForRepairTooltip(this EquipmentSlot slot)
		{
			if (slot <= EquipmentSlot.Head)
			{
				if (slot <= EquipmentSlot.Tool4)
				{
					if (slot <= EquipmentSlot.SecondaryWeapon_OffHand)
					{
						if (slot - EquipmentSlot.PrimaryWeapon_MainHand <= 1 || slot == EquipmentSlot.SecondaryWeapon_MainHand || slot == EquipmentSlot.SecondaryWeapon_OffHand)
						{
							return slot.GetDisplayName();
						}
					}
					else if (slot <= EquipmentSlot.Tool2)
					{
						if (slot == EquipmentSlot.Tool1)
						{
							return "Tool 1";
						}
						if (slot == EquipmentSlot.Tool2)
						{
							return "Tool 2";
						}
					}
					else
					{
						if (slot == EquipmentSlot.Tool3)
						{
							return "Tool 3";
						}
						if (slot == EquipmentSlot.Tool4)
						{
							return "Tool 4";
						}
					}
				}
				else if (slot <= EquipmentSlot.Ear_Left)
				{
					if (slot == EquipmentSlot.LightSource)
					{
						return "Light Source";
					}
					if (slot == EquipmentSlot.Neck)
					{
						return "Necklace";
					}
					if (slot == EquipmentSlot.Ear_Left)
					{
						return ZString.Format<string>("Left {0}", EquipmentType.Jewelry_Earring.GetDisplayName());
					}
				}
				else if (slot <= EquipmentSlot.Finger_Left)
				{
					if (slot == EquipmentSlot.Ear_Right)
					{
						return ZString.Format<string>("Right {0}", EquipmentType.Jewelry_Earring.GetDisplayName());
					}
					if (slot == EquipmentSlot.Finger_Left)
					{
						return ZString.Format<string>("Left {0}", EquipmentType.Jewelry_Ring.GetDisplayName());
					}
				}
				else
				{
					if (slot == EquipmentSlot.Finger_Right)
					{
						return ZString.Format<string>("Right {0}", EquipmentType.Jewelry_Ring.GetDisplayName());
					}
					if (slot == EquipmentSlot.Head)
					{
						return EquipmentType.Head.GetDisplayName();
					}
				}
			}
			else if (slot <= EquipmentSlot.Clothing_Feet)
			{
				if (slot <= EquipmentSlot.Waist)
				{
					if (slot == EquipmentSlot.Cosmetic)
					{
						return "Cosmetic";
					}
					if (slot == EquipmentSlot.Back)
					{
						return EquipmentType.Back.GetDisplayName();
					}
					if (slot == EquipmentSlot.Waist)
					{
						return EquipmentType.Waist.GetDisplayName();
					}
				}
				else if (slot <= EquipmentSlot.Clothing_Hands)
				{
					if (slot == EquipmentSlot.Clothing_Chest)
					{
						return EquipmentType.Clothing_Chest.GetDisplayName();
					}
					if (slot == EquipmentSlot.Clothing_Hands)
					{
						return EquipmentType.Clothing_Hands.GetDisplayName();
					}
				}
				else
				{
					if (slot == EquipmentSlot.Clothing_Legs)
					{
						return EquipmentType.Clothing_Legs.GetDisplayName();
					}
					if (slot == EquipmentSlot.Clothing_Feet)
					{
						return EquipmentType.Clothing_Feet.GetDisplayName();
					}
				}
			}
			else if (slot <= EquipmentSlot.Armor_Hand_L)
			{
				if (slot <= EquipmentSlot.Armor_Shoulder_R)
				{
					if (slot == EquipmentSlot.Armor_Shoulder_L)
					{
						return ZString.Format<string>("Left {0}", EquipmentType.Armor_Shoulders.GetDisplayName());
					}
					if (slot == EquipmentSlot.Armor_Shoulder_R)
					{
						return ZString.Format<string>("Right {0}", EquipmentType.Armor_Shoulders.GetDisplayName());
					}
				}
				else
				{
					if (slot == EquipmentSlot.Armor_Chest)
					{
						return EquipmentType.Armor_Chest.GetDisplayName();
					}
					if (slot == EquipmentSlot.Armor_Hand_L)
					{
						return ZString.Format<string>("Left {0}", EquipmentType.Armor_Hands.GetDisplayName());
					}
				}
			}
			else if (slot <= EquipmentSlot.Armor_Legs)
			{
				if (slot == EquipmentSlot.Armor_Hand_R)
				{
					return ZString.Format<string>("Right {0}", EquipmentType.Armor_Hands.GetDisplayName());
				}
				if (slot == EquipmentSlot.Armor_Legs)
				{
					return EquipmentType.Armor_Legs.GetDisplayName();
				}
			}
			else
			{
				if (slot == EquipmentSlot.Armor_Feet_L)
				{
					return ZString.Format<string>("Left {0}", EquipmentType.Armor_Feet.GetDisplayName());
				}
				if (slot == EquipmentSlot.Armor_Feet_R)
				{
					return ZString.Format<string>("Right {0}", EquipmentType.Armor_Feet.GetDisplayName());
				}
			}
			return slot.GetDisplayName();
		}

		// Token: 0x04004A38 RID: 19000
		private static EquipmentSlot[] m_equipmentSlots = null;

		// Token: 0x04004A39 RID: 19001
		private static HashSet<int> m_validEquipmentSlotIndexes = null;

		// Token: 0x04004A3A RID: 19002
		private static EquipmentType[] m_equipmentTypes = null;

		// Token: 0x04004A3B RID: 19003
		public static readonly List<EquipmentType> CosmeticEquipmentTypes = new List<EquipmentType>
		{
			EquipmentType.Head,
			EquipmentType.Clothing_Chest,
			EquipmentType.Clothing_Hands,
			EquipmentType.Clothing_Legs,
			EquipmentType.Clothing_Feet,
			EquipmentType.Armor_Shoulders,
			EquipmentType.Armor_Chest,
			EquipmentType.Armor_Hands,
			EquipmentType.Armor_Legs,
			EquipmentType.Armor_Feet
		};

		// Token: 0x04004A3C RID: 19004
		private static Dictionary<EquipmentType, IEnumerable<EquipmentSlot>> m_compatibleSlotEnumerable = null;

		// Token: 0x04004A3D RID: 19005
		public const EquipmentSlot kWeaponSlots = EquipmentSlot.PrimaryWeapon_MainHand | EquipmentSlot.PrimaryWeapon_OffHand | EquipmentSlot.SecondaryWeapon_MainHand | EquipmentSlot.SecondaryWeapon_OffHand;

		// Token: 0x04004A3E RID: 19006
		public const EquipmentSlot HelmSlot = EquipmentSlot.Head;

		// Token: 0x04004A3F RID: 19007
		public const int HelmIndex = 32768;

		// Token: 0x04004A40 RID: 19008
		public const EquipmentSlot CosmeticSlot = EquipmentSlot.Cosmetic;

		// Token: 0x04004A41 RID: 19009
		public const int CosmeticSlotIndex = 65536;

		// Token: 0x04004A42 RID: 19010
		private static Dictionary<int, EquipmentSlot> m_indexToSlotDict = null;
	}
}
