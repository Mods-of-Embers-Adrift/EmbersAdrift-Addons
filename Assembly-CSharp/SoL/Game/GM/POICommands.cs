using System;
using System.Collections.Generic;
using SoL.Game.Messages;
using SoL.Game.Player;
using SoL.Managers;
using SoL.Utilities;

namespace SoL.Game.GM
{
	// Token: 0x02000BEF RID: 3055
	public static class POICommands
	{
		// Token: 0x06005E5B RID: 24155 RVA: 0x0007F6DD File Offset: 0x0007D8DD
		private static string GetPossiblePOIs()
		{
			return string.Join(", ", POICommands.DebugLocs.Keys);
		}

		// Token: 0x06005E5C RID: 24156 RVA: 0x0007F6F3 File Offset: 0x0007D8F3
		private static bool LocalNetworkEntityExists()
		{
			return LocalPlayer.NetworkEntity;
		}

		// Token: 0x06005E5D RID: 24157 RVA: 0x0007F6FF File Offset: 0x0007D8FF
		private static bool IsNearEmberRing()
		{
			return LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData && LocalPlayer.GameEntity.CharacterData.CharacterFlags.Value.HasBitFlag(PlayerFlags.InCampfire);
		}

		// Token: 0x06005E5E RID: 24158 RVA: 0x001F6520 File Offset: 0x001F4720
		public static void RegisterPOICommands()
		{
			if (DeploymentBranchFlagsExtensions.IsQA() || (SessionData.User != null && SessionData.User.IsGM()))
			{
				CommandRegistry.Register("poi", delegate(string[] args)
				{
					if (!POICommands.LocalNetworkEntityExists())
					{
						return;
					}
					if (!POICommands.IsNearEmberRing())
					{
						POICommands.Log("Must be near an ember ring to use /poi!");
						return;
					}
					if (args == null || args.Length == 0)
					{
						POICommands.Log("Invalid argument to use /poi!  Use one of: " + POICommands.GetPossiblePOIs());
						return;
					}
					string request = args[0].ToLowerInvariant();
					string value;
					if (!POICommands.DebugLocs.TryGetValue(request, out value))
					{
						POICommands.Log("Invalid request for /poi!  Use one of: " + POICommands.GetPossiblePOIs());
						return;
					}
					DebugLocation debugLocation = new DebugLocation(value);
					if (!debugLocation.Valid)
					{
						POICommands.Log("Invalid Debug Location!");
						return;
					}
					int zoneId2 = debugLocation.ZoneId;
					ZoneId zoneId;
					if (!ZoneIdExtensions.ZoneIdDict.TryGetValue(zoneId2, out zoneId))
					{
						POICommands.Log("Invalid zone id!");
						return;
					}
					if (LocalZoneManager.ZoneRecord == null || LocalZoneManager.ZoneRecord.ZoneId != zoneId2)
					{
						POICommands.Log("Requesting zone to " + zoneId.ToString());
						LoginApiManager.PerformZoneCheck(zoneId, delegate(bool response)
						{
							if (response && POICommands.LocalNetworkEntityExists())
							{
								LocalPlayer.NetworkEntity.PlayerRpcHandler.QA_RequestZoneToPOI(request);
								return;
							}
							POICommands.Log("Failed to zone to " + zoneId.ToString() + "!");
						});
						return;
					}
					if (POICommands.LocalNetworkEntityExists())
					{
						LocalPlayer.NetworkEntity.PlayerRpcHandler.QA_RequestZoneToPOI(request);
						return;
					}
				}, "Teleport to a pre-defined POI", "Teleport to a pre-defined POI (Usage: /poi [poi name])", null);
			}
		}

		// Token: 0x06005E5F RID: 24159 RVA: 0x0007F73A File Offset: 0x0007D93A
		public static void UnregisterPOICommands()
		{
			CommandRegistry.UnRegister("poi");
		}

		// Token: 0x06005E60 RID: 24160 RVA: 0x0007F746 File Offset: 0x0007D946
		private static void Log(string txt)
		{
			MessageManager.ChatQueue.AddToQueue(MessageType.Notification, txt);
		}

		// Token: 0x040051A0 RID: 20896
		private const string kPoiCommand = "poi";

		// Token: 0x040051A1 RID: 20897
		public static readonly Dictionary<string, string> DebugLocs = new Dictionary<string, string>
		{
			{
				"drifterslanding",
				"MTgwfCg1NzYuMTY2LCAtMTAuOTc3LCAtODc4LjY5MSwgMCk="
			},
			{
				"ravenrock",
				"MTgxfCg0NjUuMTU2LCAwLjMsIDgzOS4wMTEsIDAp"
			},
			{
				"stronghold",
				"NDAwfCg1ODguOTI0LCAyNTQuNTcyLCAtMTEyNS45ODcsIDAp"
			},
			{
				"fortress",
				"NDAwfCg4MS45OTksIDEzMi4yNTYsIC0zNTguNzExLCAxMy43NjEp"
			},
			{
				"brookhollow",
				"MjAxfCgtODIzLjY1OCwgOTQuOTI3LCA0MjQuNTA3LCAwKQ=="
			},
			{
				"emberoasis",
				"MjAxfCgyNzIuMzc0LCAxODcuODczLCAtMTQyMy4wMzYsIDAp"
			},
			{
				"redshorebeach",
				"MjAzfCgtMTE0MC40OTEsIDI2Ljk5MiwgLTE3MDkuMjA3LCAwKQ=="
			},
			{
				"freehold",
				"NDAxfCgtMTAzNi42NjEsIDEzNy41NTEsIDU3OS41MTMsIDAp"
			},
			{
				"cv1east",
				"MzAyfCgyNDMuMTMyLCAyMS41MzEsIC05Ni4yNDEsIDAp"
			},
			{
				"cv1west",
				"MzAyfCgtOTEuMzMzLCAyMS42MTcsIDI0MC4yMDksIDAp"
			},
			{
				"cv2south",
				"MzAyfCg5NTMuMTE3LCAyMS40MjIsIDkwLjcxLCAwKQ=="
			},
			{
				"cv2north",
				"MzAyfCg3MDguMjY0LCAyNi44NjIsIDQwMC43MjYsIDAp"
			},
			{
				"osricsrest",
				"MzAyfCgzNTYuNjkxLCAxOC44OTQsIDU4Ny45NzIsIDE2NC4zNTIp"
			},
			{
				"karst",
				"NDAyfCg0OC40MzUsIDI2LjE1MywgMzAyLjAxMywgMCk="
			},
			{
				"undercroft",
				"NDAyfCg1NS43MTcsIDM3Mi4xNzksIC01NTIuNjUxLCAwKQ=="
			},
			{
				"molebear",
				"NDAyfCgtMzAuNzE2LCAyMi44MDUsIC03MDMuMDExLCAwKQ=="
			},
			{
				"fd1",
				"NDAzfCg2Ni4xMzksIDQxLjU5NCwgMjcuOTMyLCAwKQ=="
			},
			{
				"fd2",
				"NDAzfCg5NS40ODcsIC0xOS44MTEsIC02MS44MjIsIDAp"
			},
			{
				"ev.torchbugtunnels",
				"MzAwfCgxOTQuOTc2LCAyMi43NjMsIDI5My4wOTgsIDAp"
			},
			{
				"ev.watchersfolly",
				"MzAwfCgtMTE0Ljk4MiwgMzAuMzgxLCA2OTAuNTU2LCAwKQ=="
			},
			{
				"ev.noxiousbog",
				"MzAwfCgtNTQ5LjQyMywgMjMuNDk0LCAtMy44NzYsIDAp"
			},
			{
				"ev.derelictsanctuary",
				"MzAwfCgzNjQuNjA1LCAyOS45NjUsIDc0My43MTksIDAp"
			},
			{
				"ev.spinnersrecluse",
				"MzAwfCgtNTE0LjgxMSwgMjguNzEsIDY5Mi4wMzMsIDAp"
			},
			{
				"ev.crawlingburrows",
				"MzAwfCg0NTYuODU1LCAyOC44MjYsIC0xMTUuOTMzLCAwKQ=="
			},
			{
				"ev.basiliskrun",
				"MzAwfCg2MDQuMTg0LCAyNi42MTIsIC02NDIuMDU4LCAwKQ=="
			},
			{
				"ev.floodedruins",
				"MzAwfCgtNDc4Ljk2NCwgOC4zNDYsIC0zOTAuNzM5LCAwKQ=="
			},
			{
				"ev.cragscalecaverns",
				"MzAwfCgxMDUuNjUzLCAyMi44MjgsIC0zNDEuNzY1LCAwKQ=="
			},
			{
				"ev.arkhoslair",
				"MzAwfCg2NDIuMjQ4LCAyNi4zNDcsIDI0Ni41LCAwKQ=="
			},
			{
				"ev.blightrootgrove",
				"MzAwfCgtMTI2LjI0MiwgMjMuMTE0LCAxMTcuOTAxLCAwKQ=="
			},
			{
				"ev.shriekingbarrows",
				"MzAwfCgtMjAxLjAwNiwgMjIuMjQyLCAtNzk5LjM2NSwgMCk="
			}
		};
	}
}
