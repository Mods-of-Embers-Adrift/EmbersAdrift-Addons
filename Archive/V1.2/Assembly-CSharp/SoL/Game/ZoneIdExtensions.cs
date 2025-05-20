using System;
using System.Collections.Generic;
using SoL.Game.Settings;
using SoL.Utilities.Extensions;

namespace SoL.Game
{
	// Token: 0x0200060A RID: 1546
	public static class ZoneIdExtensions
	{
		// Token: 0x17000A79 RID: 2681
		// (get) Token: 0x06003136 RID: 12598 RVA: 0x00061E3F File Offset: 0x0006003F
		public static ZoneId[] ZoneIds
		{
			get
			{
				if (ZoneIdExtensions.m_zoneIds == null)
				{
					ZoneIdExtensions.m_zoneIds = (ZoneId[])Enum.GetValues(typeof(ZoneId));
				}
				return ZoneIdExtensions.m_zoneIds;
			}
		}

		// Token: 0x17000A7A RID: 2682
		// (get) Token: 0x06003137 RID: 12599 RVA: 0x0015C444 File Offset: 0x0015A644
		public static Dictionary<int, ZoneId> ZoneIdDict
		{
			get
			{
				if (ZoneIdExtensions.m_zoneIdDict == null)
				{
					ZoneIdExtensions.m_zoneIdDict = new Dictionary<int, ZoneId>(ZoneIdExtensions.ZoneIds.Length);
					for (int i = 0; i < ZoneIdExtensions.ZoneIds.Length; i++)
					{
						ZoneIdExtensions.m_zoneIdDict.Add((int)ZoneIdExtensions.ZoneIds[i], ZoneIdExtensions.ZoneIds[i]);
					}
				}
				return ZoneIdExtensions.m_zoneIdDict;
			}
		}

		// Token: 0x06003138 RID: 12600 RVA: 0x0015C498 File Offset: 0x0015A698
		public static bool IsMatchingZoneForMap(this ZoneId currentZone, ZoneId zoneQuery)
		{
			if (currentZone == zoneQuery)
			{
				return true;
			}
			if (currentZone <= ZoneId.Dryfoot)
			{
				if (currentZone == ZoneId.RedshoreForest)
				{
					goto IL_45;
				}
				if (currentZone != ZoneId.Dryfoot)
				{
					return false;
				}
			}
			else if (currentZone != ZoneId.DryfootStronghold)
			{
				if (currentZone != ZoneId.RedshoreRidge)
				{
					return false;
				}
				goto IL_45;
			}
			return zoneQuery == ZoneId.Dryfoot || zoneQuery == ZoneId.DryfootStronghold;
			IL_45:
			return zoneQuery == ZoneId.RedshoreForest || zoneQuery == ZoneId.RedshoreRidge;
		}

		// Token: 0x06003139 RID: 12601 RVA: 0x00061E66 File Offset: 0x00060066
		public static bool IsAvailable(this ZoneId zoneId)
		{
			return GlobalSettings.Values != null && GlobalSettings.Values.General.AvailableZones.Contains(zoneId);
		}

		// Token: 0x0600313A RID: 12602 RVA: 0x00061E8C File Offset: 0x0006008C
		public static bool AvoidCampDelay(this ZoneId zoneId)
		{
			return zoneId == ZoneId.NewhavenCity || zoneId < ZoneId.NewhavenValley;
		}

		// Token: 0x0600313B RID: 12603 RVA: 0x00061EA0 File Offset: 0x000600A0
		public static bool PreventPortableCraftingStation(this ZoneId zoneId)
		{
			return zoneId == ZoneId.NewhavenCity;
		}

		// Token: 0x0600313C RID: 12604 RVA: 0x0015C500 File Offset: 0x0015A700
		public static string GetSubZoneDisplayName(this SubZoneId subZoneId)
		{
			if (subZoneId <= SubZoneId.Fortress)
			{
				switch (subZoneId)
				{
				case SubZoneId.CentralVeinsNorth:
					return "North";
				case SubZoneId.CentralVeinsSouth:
					return "South";
				case SubZoneId.CentralVeinsTunnel:
					return "Osric's Rest";
				default:
					if (subZoneId != SubZoneId.Stronghold)
					{
						if (subZoneId != SubZoneId.Fortress)
						{
							goto IL_FB;
						}
						return "Dryfoot Fortress";
					}
					break;
				}
			}
			else if (subZoneId <= SubZoneId.Ridge)
			{
				if (subZoneId - SubZoneId.Karst > 1)
				{
					if (subZoneId != SubZoneId.Ridge)
					{
						goto IL_FB;
					}
					return "Redshore Ridge";
				}
			}
			else
			{
				if (subZoneId == SubZoneId.Freehold)
				{
					return "Exile Freehold";
				}
				switch (subZoneId)
				{
				case SubZoneId.TorchbugTunnels:
				case SubZoneId.NoxiousBog:
				case SubZoneId.DerelictSanctuary:
				case SubZoneId.SpinnersRecluse:
				case SubZoneId.CrawlingBurrows:
				case SubZoneId.BasiliskRun:
				case SubZoneId.FloodedRuins:
				case SubZoneId.CragscaleCaverns:
				case SubZoneId.BlightRootGrove:
				case SubZoneId.ShriekingBarrows:
					return subZoneId.ToStringWithSpaces();
				case SubZoneId.WatchersFolly:
					return "Watcher's Folly";
				case SubZoneId.ArkhosLair:
					return "Arkhos' Lair";
				case (SubZoneId)61:
				case (SubZoneId)62:
				case (SubZoneId)63:
				case (SubZoneId)64:
				case (SubZoneId)65:
				case (SubZoneId)66:
				case (SubZoneId)67:
				case (SubZoneId)68:
				case (SubZoneId)69:
					goto IL_FB;
				default:
					goto IL_FB;
				}
			}
			return subZoneId.ToString();
			IL_FB:
			return string.Empty;
		}

		// Token: 0x0600313D RID: 12605 RVA: 0x00061EAD File Offset: 0x000600AD
		public static bool IncludeZoneNameInDisplayName(this SubZoneId subZoneId)
		{
			if (subZoneId <= SubZoneId.Fortress)
			{
				if (subZoneId - SubZoneId.CentralVeinsNorth <= 1)
				{
					return true;
				}
				if (subZoneId == SubZoneId.CentralVeinsTunnel)
				{
					return false;
				}
				if (subZoneId - SubZoneId.Stronghold <= 1)
				{
					return false;
				}
			}
			else
			{
				if (subZoneId - SubZoneId.Karst <= 1)
				{
					return true;
				}
				if (subZoneId - SubZoneId.Ridge <= 1)
				{
					return false;
				}
				if (subZoneId - SubZoneId.TorchbugTunnels <= 10)
				{
					return true;
				}
			}
			return true;
		}

		// Token: 0x04002F92 RID: 12178
		private static ZoneId[] m_zoneIds = null;

		// Token: 0x04002F93 RID: 12179
		private static Dictionary<int, ZoneId> m_zoneIdDict = null;

		// Token: 0x04002F94 RID: 12180
		public static readonly ZoneId[] MapDropdownOrder = new ZoneId[]
		{
			ZoneId.NewhavenValley,
			ZoneId.NewhavenCity,
			ZoneId.Northreach,
			ZoneId.Meadowlands,
			ZoneId.Dryfoot,
			ZoneId.RedshoreForest,
			ZoneId.HighlandHills,
			ZoneId.GrimstoneCanyon,
			ZoneId.GrizzledPeaks
		};
	}
}
