using System;
using System.Collections.Generic;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A54 RID: 2644
	public static class ComponentEffectAssigners
	{
		// Token: 0x060051EC RID: 20972 RVA: 0x00076AF9 File Offset: 0x00074CF9
		public static IEnumerable<ComponentEffectAssignerName> GetAllAssignerNames()
		{
			return (IEnumerable<ComponentEffectAssignerName>)typeof(ComponentEffectAssignerName).GetEnumValues();
		}

		// Token: 0x060051ED RID: 20973 RVA: 0x00076B0F File Offset: 0x00074D0F
		public static MinMaxFloatRange GetRange(ComponentEffectAssignerName name)
		{
			if (!ComponentEffectAssigners.kComponentEffectRanges.ContainsKey(name))
			{
				return ComponentEffectAssigners.kDefaultRange;
			}
			return ComponentEffectAssigners.kComponentEffectRanges[name];
		}

		// Token: 0x060051EE RID: 20974 RVA: 0x001D1F9C File Offset: 0x001D019C
		public static float Apply(ComponentEffectAssignerName name, float value, ComponentEffectOutputType type, MinMaxFloatRange? rangeOverride, float existingValue)
		{
			float num = 0f;
			switch (type)
			{
			case ComponentEffectOutputType.Assign:
				num = value;
				break;
			case ComponentEffectOutputType.Add:
				num = existingValue + value;
				break;
			case ComponentEffectOutputType.Subtract:
				num = existingValue - value;
				break;
			case ComponentEffectOutputType.Multiply:
				num = existingValue * value;
				break;
			case ComponentEffectOutputType.Divide:
				num = ((value != 0f) ? (existingValue / value) : value);
				break;
			}
			if (ComponentEffectAssigners.IsClamped(name))
			{
				num = ((rangeOverride == null) ? ComponentEffectAssigners.GetRange(name) : rangeOverride.Value).Clamp(num);
			}
			return num;
		}

		// Token: 0x060051EF RID: 20975 RVA: 0x001D2020 File Offset: 0x001D0220
		public static int Apply(ComponentEffectAssignerName name, float value, ComponentEffectOutputType type, MinMaxFloatRange? rangeOverride, int existingValue)
		{
			int num = 0;
			switch (type)
			{
			case ComponentEffectOutputType.Assign:
				num = (int)value;
				break;
			case ComponentEffectOutputType.Add:
				num = (int)((float)existingValue + value);
				break;
			case ComponentEffectOutputType.Subtract:
				num = (int)((float)existingValue - value);
				break;
			case ComponentEffectOutputType.Multiply:
				num = (int)((float)existingValue * value);
				break;
			case ComponentEffectOutputType.Divide:
				num = ((value != 0f) ? ((int)((float)existingValue / value)) : ((int)value));
				break;
			}
			if (ComponentEffectAssigners.IsClamped(name))
			{
				num = (int)((rangeOverride == null) ? ComponentEffectAssigners.GetRange(name) : rangeOverride.Value).Clamp((float)num);
			}
			return num;
		}

		// Token: 0x060051F0 RID: 20976 RVA: 0x001D20AC File Offset: 0x001D02AC
		public static uint Apply(ComponentEffectAssignerName name, float value, ComponentEffectOutputType type, MinMaxFloatRange? rangeOverride, uint existingValue)
		{
			uint num = 0U;
			switch (type)
			{
			case ComponentEffectOutputType.Assign:
				num = (uint)value;
				break;
			case ComponentEffectOutputType.Add:
				num = (uint)(existingValue + value);
				break;
			case ComponentEffectOutputType.Subtract:
				num = (uint)(existingValue - value);
				break;
			case ComponentEffectOutputType.Multiply:
				num = (uint)(existingValue * value);
				break;
			case ComponentEffectOutputType.Divide:
				num = ((value != 0f) ? ((uint)(existingValue / value)) : ((uint)value));
				break;
			}
			if (ComponentEffectAssigners.IsClamped(name))
			{
				num = (uint)((rangeOverride == null) ? ComponentEffectAssigners.GetRange(name) : rangeOverride.Value).Clamp(num);
			}
			return num;
		}

		// Token: 0x060051F1 RID: 20977 RVA: 0x00076B2F File Offset: 0x00074D2F
		public static bool IsClamped(ComponentEffectAssignerName name)
		{
			return name != ComponentEffectAssignerName.MovementModifier && name != ComponentEffectAssignerName.ProfileFraction && name != ComponentEffectAssignerName.ArmorCostMultiplier;
		}

		// Token: 0x04004964 RID: 18788
		private static readonly MinMaxFloatRange kDefaultRange = new MinMaxFloatRange(0f, 1000f);

		// Token: 0x04004965 RID: 18789
		private static readonly IDictionary<ComponentEffectAssignerName, MinMaxFloatRange> kComponentEffectRanges = new Dictionary<ComponentEffectAssignerName, MinMaxFloatRange>
		{
			{
				ComponentEffectAssignerName.Weight,
				new MinMaxFloatRange(0f, 100f)
			},
			{
				ComponentEffectAssignerName.BaseWorth,
				new MinMaxFloatRange(0f, 1000f)
			},
			{
				ComponentEffectAssignerName.BaseArmorClass,
				new MinMaxFloatRange(0f, 10000f)
			},
			{
				ComponentEffectAssignerName.MaxDamageAbsorption,
				new MinMaxFloatRange(0f, 100000f)
			},
			{
				ComponentEffectAssignerName.MinimumToolEffectiveness,
				new MinMaxFloatRange(0f, 1f)
			},
			{
				ComponentEffectAssignerName.MaxStackCount,
				new MinMaxFloatRange(0f, 100f)
			},
			{
				ComponentEffectAssignerName.ExecutionTime,
				new MinMaxFloatRange(0f, 100f)
			},
			{
				ComponentEffectAssignerName.MovementModifier,
				new MinMaxFloatRange(0f, 1f)
			},
			{
				ComponentEffectAssignerName.Cooldown,
				new MinMaxFloatRange(0f, 100f)
			},
			{
				ComponentEffectAssignerName.FireDuration,
				new MinMaxFloatRange(0f, 1000f)
			},
			{
				ComponentEffectAssignerName.ProfileFraction,
				new MinMaxFloatRange(0f, 1f)
			},
			{
				ComponentEffectAssignerName.Angle,
				new MinMaxFloatRange(0f, 360f)
			},
			{
				ComponentEffectAssignerName.Delay,
				new MinMaxFloatRange(0f, 100f)
			},
			{
				ComponentEffectAssignerName.DiceCount,
				new MinMaxFloatRange(0f, 100f)
			},
			{
				ComponentEffectAssignerName.DiceSides,
				new MinMaxFloatRange(0f, 100f)
			},
			{
				ComponentEffectAssignerName.MinimumDistance,
				new MinMaxFloatRange(0f, 100f)
			},
			{
				ComponentEffectAssignerName.MaximumDistance,
				new MinMaxFloatRange(0f, 100f)
			},
			{
				ComponentEffectAssignerName.HitModifier,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.ArmorModifier,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.Avoidance,
				new MinMaxFloatRange(0f, 100f)
			},
			{
				ComponentEffectAssignerName.Block,
				new MinMaxFloatRange(0f, 100f)
			},
			{
				ComponentEffectAssignerName.Parry,
				new MinMaxFloatRange(0f, 100f)
			},
			{
				ComponentEffectAssignerName.Riposte,
				new MinMaxFloatRange(0f, 100f)
			},
			{
				ComponentEffectAssignerName.ProficiencyRequirement,
				new MinMaxFloatRange(1f, 100f)
			},
			{
				ComponentEffectAssignerName.ArmorCostMultiplier,
				new MinMaxFloatRange(-1f, 1f)
			},
			{
				ComponentEffectAssignerName.Resilience,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.SafeFall,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.Haste,
				new MinMaxFloatRange(-100f, 1000f)
			},
			{
				ComponentEffectAssignerName.Movement,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.CombatMovement,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.ArmorWeightOverride,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.ArmorWeightInterpolator,
				new MinMaxFloatRange(0f, 100f)
			},
			{
				ComponentEffectAssignerName.RegenHealth,
				new MinMaxFloatRange(-100f, 1000f)
			},
			{
				ComponentEffectAssignerName.RegenStamina,
				new MinMaxFloatRange(-100f, 1000f)
			},
			{
				ComponentEffectAssignerName.Healing,
				new MinMaxFloatRange(-100f, 1000f)
			},
			{
				ComponentEffectAssignerName.MaxDuration,
				new MinMaxFloatRange(0f, 10000f)
			},
			{
				ComponentEffectAssignerName.DiceModifier,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.Hit,
				new MinMaxFloatRange(-100f, 1000f)
			},
			{
				ComponentEffectAssignerName.Penetration,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.Flanking,
				new MinMaxFloatRange(-100f, 1000f)
			},
			{
				ComponentEffectAssignerName.Damage1H,
				new MinMaxFloatRange(-100f, 1000f)
			},
			{
				ComponentEffectAssignerName.Damage2H,
				new MinMaxFloatRange(-100f, 1000f)
			},
			{
				ComponentEffectAssignerName.DamageRanged,
				new MinMaxFloatRange(-100f, 1000f)
			},
			{
				ComponentEffectAssignerName.DamageMental,
				new MinMaxFloatRange(-100f, 1000f)
			},
			{
				ComponentEffectAssignerName.DamageChemical,
				new MinMaxFloatRange(-100f, 1000f)
			},
			{
				ComponentEffectAssignerName.DamageEmber,
				new MinMaxFloatRange(-100f, 1000f)
			},
			{
				ComponentEffectAssignerName.StunStatusResist,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.FearStatusResist,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.DazeStatusResist,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.EnrageStatusResist,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.ConfuseStatusResist,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.LullStatusResist,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.ResistDamagePhysical,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.ResistDamageMental,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.ResistDamageChemical,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.ResistDamageEmber,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.ResistDebuffPhysical,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.ResistDebuffMental,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.ResistDebuffChemical,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.ResistDebuffEmber,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.ResistDebuffMovement,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.DamageResist,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.DamageResistEmber,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.DamageReduction,
				new MinMaxFloatRange(-500f, 500f)
			},
			{
				ComponentEffectAssignerName.DamageReductionEmber,
				new MinMaxFloatRange(-500f, 500f)
			},
			{
				ComponentEffectAssignerName.DebuffResist,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.DebuffResistEmber,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.AllDamage,
				new MinMaxFloatRange(-100f, 1000f)
			},
			{
				ComponentEffectAssignerName.AllPhysicalDamage,
				new MinMaxFloatRange(-100f, 1000f)
			},
			{
				ComponentEffectAssignerName.AllNonPhysicalDamage,
				new MinMaxFloatRange(-100f, 1000f)
			},
			{
				ComponentEffectAssignerName.AllMeleeDamage,
				new MinMaxFloatRange(-100f, 1000f)
			},
			{
				ComponentEffectAssignerName.AllActiveDefense,
				new MinMaxFloatRange(0f, 100f)
			},
			{
				ComponentEffectAssignerName.AllDamageResist,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.AllDebuffResist,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.AllStatusResist,
				new MinMaxFloatRange(-100f, 100f)
			},
			{
				ComponentEffectAssignerName.ExecutionTimeMultiplier,
				new MinMaxFloatRange(0.1f, 2f)
			}
		};
	}
}
