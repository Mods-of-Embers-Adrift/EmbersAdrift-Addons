using System;
using System.Collections.Generic;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C76 RID: 3190
	public static class StatusEffectExtensions
	{
		// Token: 0x17001755 RID: 5973
		// (get) Token: 0x0600615B RID: 24923 RVA: 0x00081A15 File Offset: 0x0007FC15
		public static StatusEffectType[] StatusEffectTypes
		{
			get
			{
				if (StatusEffectExtensions.m_statusEffectTypes == null)
				{
					StatusEffectExtensions.m_statusEffectTypes = (StatusEffectType[])Enum.GetValues(typeof(StatusEffectType));
				}
				return StatusEffectExtensions.m_statusEffectTypes;
			}
		}

		// Token: 0x0600615C RID: 24924 RVA: 0x00081A3C File Offset: 0x0007FC3C
		public static bool HasSubTypes(this StatusEffectType channel)
		{
			return channel == StatusEffectType.OutgoingDamage || channel - StatusEffectType.IncomingDamageResist <= 3;
		}

		// Token: 0x0600615D RID: 24925 RVA: 0x00081A4A File Offset: 0x0007FC4A
		public static bool HasDamageTypes(this StatusEffectType channel)
		{
			return channel == StatusEffectType.OutgoingDamage || channel == StatusEffectType.IncomingDamageResist;
		}

		// Token: 0x0600615E RID: 24926 RVA: 0x001FF7F4 File Offset: 0x001FD9F4
		public static StatusEffectSubType[] GetSubType(this StatusEffectType channel)
		{
			switch (channel)
			{
			case StatusEffectType.OutgoingDamage:
			case StatusEffectType.IncomingDamageResist:
				return StatusEffectExtensions.m_damageTypeSubTypes;
			case StatusEffectType.IncomingActiveDefense:
				return StatusEffectExtensions.m_activeDefenseSubTypes;
			case StatusEffectType.IncomingStatusEffectResist:
				return StatusEffectExtensions.m_incomingStatusEffectResistTypes;
			case StatusEffectType.Regen:
				return StatusEffectExtensions.m_regenSubTypes;
			}
			throw new ArgumentException("channel");
		}

		// Token: 0x0600615F RID: 24927 RVA: 0x00081A56 File Offset: 0x0007FC56
		public static DamageType[] GetDamageTypes(this StatusEffectSubType channel)
		{
			switch (channel)
			{
			case StatusEffectSubType.Melee:
				return DamageTypeExtensions.MeleeDamageChannels;
			case StatusEffectSubType.Ranged:
				return DamageTypeExtensions.RangedDamageChannels;
			case StatusEffectSubType.Natural:
				return DamageTypeExtensions.NaturalDamageChannels;
			case StatusEffectSubType.Elemental:
				return DamageTypeExtensions.ElementalDamageChannels;
			default:
				throw new ArgumentException("channel");
			}
		}

		// Token: 0x06006160 RID: 24928 RVA: 0x001FF848 File Offset: 0x001FDA48
		public static StatusEffectSubType GetResistChannel(this StatusEffectType channel)
		{
			switch (channel)
			{
			case StatusEffectType.OutgoingDamage:
				return StatusEffectSubType.StatusEffectResist_Damage;
			case StatusEffectType.OutgoingHit:
				return StatusEffectSubType.StatusEffectResist_Hit;
			case StatusEffectType.OutgoingPenetration:
				return StatusEffectSubType.StatusEffectResist_Penetration;
			case StatusEffectType.IncomingDamageResist:
				return StatusEffectSubType.StatusEffectResist_DamageResist;
			case StatusEffectType.IncomingActiveDefense:
				return StatusEffectSubType.StatusEffectResist_ActiveDefense;
			case StatusEffectType.Regen:
				return StatusEffectSubType.StatusEffectResist_Regen;
			case StatusEffectType.Resilience:
				return StatusEffectSubType.StatusEffectResist_Resilience;
			case StatusEffectType.Haste:
				return StatusEffectSubType.StatusEffectResist_Haste;
			case StatusEffectType.Movement:
				return StatusEffectSubType.StatusEffectResist_Movement;
			}
			throw new ArgumentException("channel");
		}

		// Token: 0x06006161 RID: 24929 RVA: 0x00081A92 File Offset: 0x0007FC92
		public static bool IsStatusEffectResist(this StatusEffectSubType subType)
		{
			return subType - StatusEffectSubType.StatusEffectResist_Damage <= 15;
		}

		// Token: 0x06006162 RID: 24930 RVA: 0x001FF8B0 File Offset: 0x001FDAB0
		public static StatusEffectSubType GetSubChannel(this DamageType damageType)
		{
			switch (damageType)
			{
			case DamageType.Melee_Slashing:
			case DamageType.Melee_Crushing:
			case DamageType.Melee_Piercing:
				return StatusEffectSubType.Melee;
			case DamageType.Ranged_Auditory:
			case DamageType.Ranged_Crushing:
			case DamageType.Ranged_Piercing:
				return StatusEffectSubType.Ranged;
			case DamageType.Natural_Life:
			case DamageType.Natural_Death:
			case DamageType.Natural_Chemical:
			case DamageType.Natural_Spirit:
				return StatusEffectSubType.Natural;
			case DamageType.Elemental_Air:
			case DamageType.Elemental_Earth:
			case DamageType.Elemental_Fire:
			case DamageType.Elemental_Water:
				return StatusEffectSubType.Elemental;
			default:
				throw new ArgumentException("damageType");
			}
		}

		// Token: 0x06006163 RID: 24931 RVA: 0x001FF910 File Offset: 0x001FDB10
		public static Dictionary<StatusEffectType, EffectValueContainer> GetStatusEffectValueContainerCollection()
		{
			Dictionary<StatusEffectType, EffectValueContainer> dictionary = new Dictionary<StatusEffectType, EffectValueContainer>(StatusEffectExtensions.StatusEffectTypes.Length, default(StatusEffectTypeComparer));
			for (int i = 0; i < StatusEffectExtensions.StatusEffectTypes.Length; i++)
			{
				dictionary.Add(StatusEffectExtensions.StatusEffectTypes[i], new EffectValueContainer(StatusEffectExtensions.StatusEffectTypes[i]));
			}
			return dictionary;
		}

		// Token: 0x06006164 RID: 24932 RVA: 0x001FF964 File Offset: 0x001FDB64
		public static string Abbreviation(this StatusEffectType type)
		{
			switch (type)
			{
			case StatusEffectType.OutgoingDamage:
				return "DMG";
			case StatusEffectType.OutgoingHit:
				return "HIT";
			case StatusEffectType.OutgoingPenetration:
				return "PEN";
			case StatusEffectType.IncomingDamageResist:
				return "RES";
			case StatusEffectType.IncomingActiveDefense:
				return "DEF";
			case StatusEffectType.IncomingStatusEffectResist:
				return "SER";
			case StatusEffectType.Regen:
				return "REG";
			default:
				return type.ToString().ToUpper();
			}
		}

		// Token: 0x06006165 RID: 24933 RVA: 0x001FF9D4 File Offset: 0x001FDBD4
		public static string GetDisplayDescription(this StatusEffectType type)
		{
			switch (type)
			{
			case StatusEffectType.OutgoingDamage:
				return "Damage";
			case StatusEffectType.OutgoingHit:
				return "Hit";
			case StatusEffectType.OutgoingPenetration:
				return "Penetration";
			case StatusEffectType.IncomingDamageResist:
				return "Damage Resists";
			case StatusEffectType.IncomingActiveDefense:
				return "Defense";
			case StatusEffectType.IncomingStatusEffectResist:
				return "Status Effect Resists";
			case StatusEffectType.SafeFall:
				return "Safe Fall";
			case StatusEffectType.CombatMovement:
				return "<size=80%><sprite=\"SolIcons\" name=\"Swords\" tint=1></size> Movement";
			}
			return type.ToString();
		}

		// Token: 0x06006166 RID: 24934 RVA: 0x001FFA58 File Offset: 0x001FDC58
		public static string GetTooltipDescription(this StatusEffectType type)
		{
			switch (type)
			{
			case StatusEffectType.OutgoingHit:
				return "+Hit increases your chance of landing a heavy or critical hit, while also reducing your chances for a glancing blow or a miss.";
			case StatusEffectType.OutgoingPenetration:
				return "+Penetration allows your attacks to ignore some of your enemy's armor dealing more damage to their health";
			case StatusEffectType.Resilience:
				return "+Resilience acts as a saving throw. When your health drops to zero a die is rolled and if successful you are healed instead of knocked out. Larger values of resilience increase the chance of success.";
			case StatusEffectType.SafeFall:
				return "+Safe Fall decreases the damage you take from falling";
			case StatusEffectType.Haste:
				return "+Haste reduces the amount of time it takes for abilities and auto attack to refresh";
			case StatusEffectType.Movement:
				return "+Movement increases your overall movement speed while not in combat stance";
			case StatusEffectType.CombatMovement:
				return "+Combat Movement increases your overall movement speed while in combat stance";
			}
			return string.Empty;
		}

		// Token: 0x06006167 RID: 24935 RVA: 0x001FFACC File Offset: 0x001FDCCC
		public static string GetDisplayDescription(this StatusEffectSubType subType)
		{
			switch (subType)
			{
			case StatusEffectSubType.StatusEffectResist_Damage:
				return "Damage";
			case StatusEffectSubType.StatusEffectResist_Hit:
				return "Hit";
			case StatusEffectSubType.StatusEffectResist_Penetration:
				return "Penetration";
			case StatusEffectSubType.StatusEffectResist_DamageResist:
				return "Resists";
			case StatusEffectSubType.StatusEffectResist_ActiveDefense:
				return "Active Defense";
			case StatusEffectSubType.StatusEffectResist_Regen:
				return "Regen";
			case StatusEffectSubType.StatusEffectResist_Resilience:
				return "Resilience";
			case StatusEffectSubType.StatusEffectResist_Haste:
				return "Haste";
			case StatusEffectSubType.StatusEffectResist_Movement:
				return "Movement";
			case StatusEffectSubType.StatusEffectResist_Stun:
				return "Stun";
			case StatusEffectSubType.StatusEffectResist_Fear:
				return "Fear";
			case StatusEffectSubType.StatusEffectResist_Charm:
				return "Charm";
			case StatusEffectSubType.StatusEffectResist_Daze:
				return "Daze";
			case StatusEffectSubType.StatusEffectResist_Enrage:
				return "Enrage";
			case StatusEffectSubType.StatusEffectResist_Confuse:
				return "Confuse";
			case StatusEffectSubType.StatusEffectResist_Kinematic:
				return "Kinematic";
			default:
				return subType.ToString();
			}
		}

		// Token: 0x06006168 RID: 24936 RVA: 0x001FFB90 File Offset: 0x001FDD90
		public static string GetTooltipDescription(this StatusEffectSubType subType)
		{
			switch (subType)
			{
			case StatusEffectSubType.Avoid:
				return "+Avoid increases the chance you avoid incoming attacks negating all damage and effects";
			case StatusEffectSubType.Block:
				return "+Block increases the chance you block incoming attacks negating all damage and effects";
			case StatusEffectSubType.Parry:
				return "+Parry increases the chance you parry incoming attacks negating all damage and effects";
			case StatusEffectSubType.Riposte:
				return "+Riposte increases the chance you counter attack after a successful parry";
			default:
				if (subType == StatusEffectSubType.Health)
				{
					return "+Health Regen increases your health regeneration rate";
				}
				if (subType != StatusEffectSubType.Stamina)
				{
					return string.Empty;
				}
				return "+Stamina Regen increases your stamina regeneration rate";
			}
		}

		// Token: 0x06006169 RID: 24937 RVA: 0x001FFBEC File Offset: 0x001FDDEC
		public static StatusEffectSubType ValidateSubType(this StatusEffectType type, StatusEffectSubType subType)
		{
			StatusEffectSubType[] validSubTypes = type.GetValidSubTypes();
			if (validSubTypes != null && validSubTypes.Length != 0)
			{
				for (int i = 0; i < validSubTypes.Length; i++)
				{
					if (validSubTypes[i] == subType)
					{
						return subType;
					}
				}
				return validSubTypes[0];
			}
			return subType;
		}

		// Token: 0x0600616A RID: 24938 RVA: 0x00081A9E File Offset: 0x0007FC9E
		public static StatusEffectSubType[] GetValidSubTypes(this StatusEffectType type)
		{
			if (!type.HasSubTypes())
			{
				return null;
			}
			return type.GetSubType();
		}

		// Token: 0x0600616B RID: 24939 RVA: 0x00081AB0 File Offset: 0x0007FCB0
		public static bool ModifiedByArmorCapacity(this StatusEffectType type)
		{
			return type - StatusEffectType.Resilience <= 4;
		}

		// Token: 0x0600616C RID: 24940 RVA: 0x00081ABB File Offset: 0x0007FCBB
		public static bool IsCombatOnly(this StatusEffectType type)
		{
			return type != StatusEffectType.Regen && type - StatusEffectType.SafeFall > 2;
		}

		// Token: 0x0600616D RID: 24941 RVA: 0x001FFC24 File Offset: 0x001FDE24
		public static bool GetCorrespondingStatType(this StatusEffectType primaryType, StatusEffectSubType subType, out StatType statType, out string msg)
		{
			statType = StatType.None;
			msg = string.Empty;
			switch (primaryType)
			{
			case StatusEffectType.OutgoingDamage:
				switch (subType)
				{
				case StatusEffectSubType.Melee:
					statType = StatType.Damage1H;
					return true;
				case StatusEffectSubType.Ranged:
					statType = StatType.DamageRanged;
					return true;
				case StatusEffectSubType.Natural:
					statType = StatType.DamageMental;
					return true;
				case StatusEffectSubType.Elemental:
					statType = StatType.DamageEmber;
					return true;
				}
				break;
			case StatusEffectType.OutgoingHit:
				statType = StatType.Hit;
				return true;
			case StatusEffectType.OutgoingPenetration:
				statType = StatType.Penetration;
				return true;
			case StatusEffectType.IncomingDamageResist:
				switch (subType)
				{
				case StatusEffectSubType.Melee:
				case StatusEffectSubType.Ranged:
					statType = StatType.ResistDamagePhysical;
					return true;
				case StatusEffectSubType.Natural:
					statType = StatType.ResistDamageChemical;
					return true;
				case StatusEffectSubType.Elemental:
					statType = StatType.ResistDamageEmber;
					return true;
				}
				break;
			case StatusEffectType.IncomingActiveDefense:
				switch (subType)
				{
				case StatusEffectSubType.Avoid:
					statType = StatType.Avoid;
					return true;
				case StatusEffectSubType.Block:
					statType = StatType.Block;
					return true;
				case StatusEffectSubType.Parry:
					statType = StatType.Parry;
					return true;
				case StatusEffectSubType.Riposte:
					statType = StatType.Riposte;
					return true;
				}
				break;
			case StatusEffectType.IncomingStatusEffectResist:
				switch (subType)
				{
				case StatusEffectSubType.StatusEffectResist_Damage:
					statType = StatType.ResistDebuffPhysical;
					return true;
				case StatusEffectSubType.StatusEffectResist_Hit:
					statType = StatType.ResistDebuffMental;
					return true;
				case StatusEffectSubType.StatusEffectResist_Penetration:
					statType = StatType.ResistDebuffPhysical;
					return true;
				case StatusEffectSubType.StatusEffectResist_DamageResist:
					statType = StatType.ResistDebuffPhysical;
					return true;
				case StatusEffectSubType.StatusEffectResist_ActiveDefense:
					statType = StatType.ResistDebuffMental;
					return true;
				case StatusEffectSubType.StatusEffectResist_Regen:
					statType = StatType.ResistDebuffChemical;
					return true;
				case StatusEffectSubType.StatusEffectResist_Resilience:
					statType = StatType.ResistDebuffPhysical;
					return true;
				case StatusEffectSubType.StatusEffectResist_Haste:
					statType = StatType.ResistDebuffPhysical;
					return true;
				case StatusEffectSubType.StatusEffectResist_Movement:
					statType = StatType.ResistDebuffMovement;
					return true;
				case StatusEffectSubType.StatusEffectResist_Stun:
					statType = StatType.ResistStun;
					return true;
				case StatusEffectSubType.StatusEffectResist_Fear:
					statType = StatType.ResistFear;
					return true;
				case StatusEffectSubType.StatusEffectResist_Daze:
					statType = StatType.ResistDaze;
					return true;
				case StatusEffectSubType.StatusEffectResist_Enrage:
					statType = StatType.ResistEnrage;
					return true;
				case StatusEffectSubType.StatusEffectResist_Confuse:
					statType = StatType.ResistConfuse;
					return true;
				}
				break;
			case StatusEffectType.Regen:
				if (subType == StatusEffectSubType.Health)
				{
					statType = StatType.RegenHealth;
					return true;
				}
				if (subType == StatusEffectSubType.Stamina)
				{
					statType = StatType.RegenStamina;
					return true;
				}
				break;
			case StatusEffectType.Resilience:
				statType = StatType.Resilience;
				return true;
			case StatusEffectType.SafeFall:
				statType = StatType.SafeFall;
				return true;
			case StatusEffectType.Haste:
				statType = StatType.Haste;
				return true;
			case StatusEffectType.Movement:
				statType = StatType.Movement;
				return true;
			case StatusEffectType.CombatMovement:
				statType = StatType.CombatMovement;
				return true;
			}
			return false;
		}

		// Token: 0x040054D3 RID: 21715
		private static readonly StatusEffectSubType[] m_damageTypeSubTypes = new StatusEffectSubType[]
		{
			StatusEffectSubType.Melee,
			StatusEffectSubType.Ranged,
			StatusEffectSubType.Natural,
			StatusEffectSubType.Elemental
		};

		// Token: 0x040054D4 RID: 21716
		private static readonly StatusEffectSubType[] m_activeDefenseSubTypes = new StatusEffectSubType[]
		{
			StatusEffectSubType.Avoid,
			StatusEffectSubType.Block,
			StatusEffectSubType.Parry,
			StatusEffectSubType.Riposte
		};

		// Token: 0x040054D5 RID: 21717
		private static readonly StatusEffectSubType[] m_incomingStatusEffectResistTypes = new StatusEffectSubType[]
		{
			StatusEffectSubType.StatusEffectResist_Damage,
			StatusEffectSubType.StatusEffectResist_Hit,
			StatusEffectSubType.StatusEffectResist_Penetration,
			StatusEffectSubType.StatusEffectResist_DamageResist,
			StatusEffectSubType.StatusEffectResist_ActiveDefense,
			StatusEffectSubType.StatusEffectResist_Regen,
			StatusEffectSubType.StatusEffectResist_Resilience,
			StatusEffectSubType.StatusEffectResist_Haste,
			StatusEffectSubType.StatusEffectResist_Movement,
			StatusEffectSubType.StatusEffectResist_Stun,
			StatusEffectSubType.StatusEffectResist_Fear,
			StatusEffectSubType.StatusEffectResist_Charm,
			StatusEffectSubType.StatusEffectResist_Daze,
			StatusEffectSubType.StatusEffectResist_Enrage,
			StatusEffectSubType.StatusEffectResist_Confuse,
			StatusEffectSubType.StatusEffectResist_Kinematic
		};

		// Token: 0x040054D6 RID: 21718
		private static StatusEffectType[] m_statusEffectTypes = null;

		// Token: 0x040054D7 RID: 21719
		private static readonly StatusEffectSubType[] m_regenSubTypes = new StatusEffectSubType[]
		{
			StatusEffectSubType.Health,
			StatusEffectSubType.Stamina
		};

		// Token: 0x040054D8 RID: 21720
		private const string kCombatMovementName = "<size=80%><sprite=\"SolIcons\" name=\"Swords\" tint=1></size> Movement";
	}
}
