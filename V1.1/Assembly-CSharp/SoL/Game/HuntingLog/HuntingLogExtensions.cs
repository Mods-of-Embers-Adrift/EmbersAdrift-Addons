using System;
using Cysharp.Text;
using SoL.Game.EffectSystem;
using SoL.Game.Interactives;

namespace SoL.Game.HuntingLog
{
	// Token: 0x02000BCA RID: 3018
	public static class HuntingLogExtensions
	{
		// Token: 0x06005D39 RID: 23865 RVA: 0x0007EACE File Offset: 0x0007CCCE
		public static bool IsTitle(this HuntingLogPerkType type)
		{
			return type <= HuntingLogPerkType.Title9;
		}

		// Token: 0x06005D3A RID: 23866 RVA: 0x001F30B0 File Offset: 0x001F12B0
		public static HuntingLogPerkType? GetHuntingLogPerkType(this StatType statType)
		{
			HuntingLogPerkType? result = null;
			if (statType <= StatType.DamageRanged)
			{
				if (statType <= StatType.Hit)
				{
					switch (statType)
					{
					case StatType.Avoid:
						result = new HuntingLogPerkType?(HuntingLogPerkType.Avoid);
						break;
					case StatType.Block:
						result = new HuntingLogPerkType?(HuntingLogPerkType.Block);
						break;
					case StatType.Parry:
						result = new HuntingLogPerkType?(HuntingLogPerkType.Parry);
						break;
					default:
						if (statType == StatType.Hit)
						{
							result = new HuntingLogPerkType?(HuntingLogPerkType.Hit);
						}
						break;
					}
				}
				else if (statType != StatType.Penetration)
				{
					if (statType - StatType.Damage1H <= 2)
					{
						result = new HuntingLogPerkType?(HuntingLogPerkType.Dmg);
					}
				}
				else
				{
					result = new HuntingLogPerkType?(HuntingLogPerkType.Pen);
				}
			}
			else if (statType <= StatType.ResistStun)
			{
				switch (statType)
				{
				case StatType.ResistDebuffPhysical:
					result = new HuntingLogPerkType?(HuntingLogPerkType.ResistDebuffPhysical);
					break;
				case StatType.ResistDebuffMental:
					result = new HuntingLogPerkType?(HuntingLogPerkType.ResistDebuffMental);
					break;
				case StatType.ResistDebuffChemical:
					result = new HuntingLogPerkType?(HuntingLogPerkType.ResistDebuffChemical);
					break;
				case StatType.ResistDebuffEmber:
					result = new HuntingLogPerkType?(HuntingLogPerkType.ResistDebuffEmber);
					break;
				case StatType.ResistDebuffMovement:
					result = new HuntingLogPerkType?(HuntingLogPerkType.ResistDebuffMovement);
					break;
				default:
					if (statType == StatType.ResistStun)
					{
						result = new HuntingLogPerkType?(HuntingLogPerkType.ResistStun);
					}
					break;
				}
			}
			else if (statType != StatType.ResistDaze)
			{
				if (statType == StatType.ResistConfuse)
				{
					result = new HuntingLogPerkType?(HuntingLogPerkType.ResistConfuse);
				}
			}
			else
			{
				result = new HuntingLogPerkType?(HuntingLogPerkType.ResistDaze);
			}
			return result;
		}

		// Token: 0x06005D3B RID: 23867 RVA: 0x0007EAD8 File Offset: 0x0007CCD8
		public static bool IsCombatStat(this HuntingLogPerkType type)
		{
			return !type.IsTitle();
		}

		// Token: 0x06005D3C RID: 23868 RVA: 0x001F31E4 File Offset: 0x001F13E4
		public static HuntingLogPerkType GetTitlePerkType(int index)
		{
			switch (index)
			{
			case 0:
				return HuntingLogPerkType.Title0;
			case 1:
				return HuntingLogPerkType.Title1;
			case 2:
				return HuntingLogPerkType.Title2;
			case 3:
				return HuntingLogPerkType.Title3;
			case 4:
				return HuntingLogPerkType.Title4;
			case 5:
				return HuntingLogPerkType.Title5;
			case 6:
				return HuntingLogPerkType.Title6;
			case 7:
				return HuntingLogPerkType.Title7;
			case 8:
				return HuntingLogPerkType.Title8;
			case 9:
				return HuntingLogPerkType.Title9;
			default:
				throw new ArgumentException("index");
			}
		}

		// Token: 0x06005D3D RID: 23869 RVA: 0x001F3240 File Offset: 0x001F1440
		public static HuntingLogEntry GetHuntingLogEntry(GameEntity playerEntity, GameEntity npcEntity)
		{
			if (!playerEntity || playerEntity.Type != GameEntityType.Player || !npcEntity || npcEntity.Type != GameEntityType.Npc)
			{
				return null;
			}
			HuntingLogEntry result = null;
			InteractiveNpc interactiveNpc;
			HuntingLogEntry huntingLogEntry;
			if (npcEntity.Interactive != null && npcEntity.Interactive.TryGetAsType(out interactiveNpc) && interactiveNpc.HuntingLogProfile && playerEntity.CollectionController != null && playerEntity.CollectionController.Record != null && playerEntity.CollectionController.Record.HuntingLog != null && playerEntity.CollectionController.Record.HuntingLog.TryGetValue(interactiveNpc.HuntingLogProfile.Id, out huntingLogEntry))
			{
				result = huntingLogEntry;
			}
			return result;
		}

		// Token: 0x06005D3E RID: 23870 RVA: 0x001F32E8 File Offset: 0x001F14E8
		public static string GetPerkDescription(this HuntingLogPerkType perkType, HuntingLogSettingsProfile settings)
		{
			int arg = (settings != null) ? settings.GetPerkMultiplier(perkType) : 1;
			return ZString.Format<int, string>("+{0} {1}", arg, perkType.GetTooltipDisplay(false));
		}

		// Token: 0x06005D3F RID: 23871 RVA: 0x001F331C File Offset: 0x001F151C
		public static string GetTooltipDisplay(this HuntingLogPerkType perkType, bool shortened = false)
		{
			switch (perkType)
			{
			case HuntingLogPerkType.Dmg:
				if (!shortened)
				{
					return "Physical Damage";
				}
				return "Physical DMG";
			case HuntingLogPerkType.Hit:
				if (!shortened)
				{
					return StatType.Hit.GetTooltipDisplay();
				}
				return "HIT";
			case HuntingLogPerkType.Pen:
				if (!shortened)
				{
					return StatType.Penetration.GetTooltipDisplay();
				}
				return "PEN";
			default:
				switch (perkType)
				{
				case HuntingLogPerkType.Avoid:
					return StatType.Avoid.GetTooltipDisplay();
				case HuntingLogPerkType.Block:
					return StatType.Block.GetTooltipDisplay();
				case HuntingLogPerkType.Parry:
					return StatType.Parry.GetTooltipDisplay();
				case (HuntingLogPerkType)23:
				case (HuntingLogPerkType)24:
				case (HuntingLogPerkType)25:
				case (HuntingLogPerkType)26:
				case (HuntingLogPerkType)27:
				case (HuntingLogPerkType)28:
				case (HuntingLogPerkType)29:
					break;
				case HuntingLogPerkType.ResistDebuffPhysical:
					return StatType.ResistDebuffPhysical.GetTooltipDisplay();
				case HuntingLogPerkType.ResistDebuffMental:
					return StatType.ResistDebuffMental.GetTooltipDisplay();
				case HuntingLogPerkType.ResistDebuffChemical:
					return StatType.ResistDebuffChemical.GetTooltipDisplay();
				case HuntingLogPerkType.ResistDebuffEmber:
					return StatType.ResistDebuffEmber.GetTooltipDisplay();
				case HuntingLogPerkType.ResistDebuffMovement:
					return StatType.ResistDebuffMovement.GetTooltipDisplay();
				default:
					switch (perkType)
					{
					case HuntingLogPerkType.ResistStun:
						return StatType.ResistStun.GetTooltipDisplay();
					case HuntingLogPerkType.ResistDaze:
						return StatType.ResistDaze.GetTooltipDisplay();
					case HuntingLogPerkType.ResistConfuse:
						return StatType.ResistConfuse.GetTooltipDisplay();
					}
					break;
				}
				throw new ArgumentException("perkType");
			}
		}

		// Token: 0x040050B5 RID: 20661
		public const int kCacheSize = 43;

		// Token: 0x040050B6 RID: 20662
		public static readonly HuntingLogPerkType[] StatPerks = new HuntingLogPerkType[]
		{
			HuntingLogPerkType.Dmg,
			HuntingLogPerkType.Hit,
			HuntingLogPerkType.Pen,
			HuntingLogPerkType.Avoid,
			HuntingLogPerkType.Block,
			HuntingLogPerkType.Parry,
			HuntingLogPerkType.ResistDebuffPhysical,
			HuntingLogPerkType.ResistDebuffMental,
			HuntingLogPerkType.ResistDebuffChemical,
			HuntingLogPerkType.ResistDebuffEmber,
			HuntingLogPerkType.ResistDebuffMovement,
			HuntingLogPerkType.ResistStun,
			HuntingLogPerkType.ResistDaze,
			HuntingLogPerkType.ResistConfuse
		};
	}
}
