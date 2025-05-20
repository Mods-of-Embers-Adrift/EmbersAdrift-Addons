using System;
using System.Collections.Generic;
using AwesomeTechnologies;
using AwesomeTechnologies.VegetationSystem.Biomes;
using Cysharp.Text;
using NetStack.Quantization;
using SoL.Game;
using SoL.Game.Discovery;
using SoL.Game.Interactives;
using SoL.Game.SkyDome;
using SoL.Networking.Database;
using SoL.Networking.Managers;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Managers
{
	// Token: 0x02000529 RID: 1321
	public static class LocalZoneManager
	{
		// Token: 0x17000838 RID: 2104
		// (get) Token: 0x060027AF RID: 10159 RVA: 0x0005BD3F File Offset: 0x00059F3F
		// (set) Token: 0x060027B0 RID: 10160 RVA: 0x0005BD46 File Offset: 0x00059F46
		public static Bounds ZoneBounds { get; private set; }

		// Token: 0x17000839 RID: 2105
		// (get) Token: 0x060027B1 RID: 10161 RVA: 0x0005BD4E File Offset: 0x00059F4E
		public static bool IsZoneLoaded
		{
			get
			{
				return LocalZoneManager.ZoneRecord != null;
			}
		}

		// Token: 0x14000078 RID: 120
		// (add) Token: 0x060027B2 RID: 10162 RVA: 0x00138A64 File Offset: 0x00136C64
		// (remove) Token: 0x060027B3 RID: 10163 RVA: 0x00138A98 File Offset: 0x00136C98
		public static event Action ZoneRecordUpdated;

		// Token: 0x1700083A RID: 2106
		// (get) Token: 0x060027B4 RID: 10164 RVA: 0x0005BD58 File Offset: 0x00059F58
		private static SpawnPointDistanceManager SpawnPointDistanceManager
		{
			get
			{
				if (LocalZoneManager.m_spawnPointDistanceManager == null)
				{
					LocalZoneManager.m_spawnPointDistanceManager = new GameObject("SpawnPointDistanceManager").AddComponent<SpawnPointDistanceManager>();
				}
				return LocalZoneManager.m_spawnPointDistanceManager;
			}
		}

		// Token: 0x1700083B RID: 2107
		// (get) Token: 0x060027B5 RID: 10165 RVA: 0x0005BD80 File Offset: 0x00059F80
		// (set) Token: 0x060027B6 RID: 10166 RVA: 0x0005BD87 File Offset: 0x00059F87
		public static ZoneRecord ZoneRecord
		{
			get
			{
				return LocalZoneManager.m_zoneRecord;
			}
			set
			{
				LocalZoneManager.m_zoneRecord = value;
				Action zoneRecordUpdated = LocalZoneManager.ZoneRecordUpdated;
				if (zoneRecordUpdated == null)
				{
					return;
				}
				zoneRecordUpdated();
			}
		}

		// Token: 0x060027B7 RID: 10167 RVA: 0x0005BD9E File Offset: 0x00059F9E
		public static PlayerSpawn GetDefaultPlayerSpawn(GameEntity entity)
		{
			if (!entity || !entity.LocalDefaultPlayerSpawn)
			{
				return LocalZoneManager.DefaultPlayerSpawn;
			}
			return entity.LocalDefaultPlayerSpawn;
		}

		// Token: 0x1700083C RID: 2108
		// (get) Token: 0x060027B8 RID: 10168 RVA: 0x0005BDC1 File Offset: 0x00059FC1
		// (set) Token: 0x060027B9 RID: 10169 RVA: 0x0005BDC8 File Offset: 0x00059FC8
		public static PlayerSpawn DefaultPlayerSpawn { get; private set; }

		// Token: 0x060027BA RID: 10170 RVA: 0x00138ACC File Offset: 0x00136CCC
		public static bool RegisterZonePoint(ZonePoint zp)
		{
			if (zp.TargetZone == ZoneId.None)
			{
				return false;
			}
			LocalZoneManager.ZoneConnection zoneConnection;
			if (!LocalZoneManager.m_zoneConnections.TryGetValue(zp.TargetZone, out zoneConnection))
			{
				zoneConnection = new LocalZoneManager.ZoneConnection(zp.TargetZone);
				LocalZoneManager.m_zoneConnections.Add(zoneConnection.TargetZone, zoneConnection);
			}
			bool flag = zoneConnection.AddZonePoint(zp);
			if (!flag)
			{
				LocalZoneManager.CleanupConnection(zoneConnection);
			}
			return flag;
		}

		// Token: 0x060027BB RID: 10171 RVA: 0x00138B24 File Offset: 0x00136D24
		public static void DeregisterZonePoint(ZonePoint zp)
		{
			if (zp.TargetZone == ZoneId.None)
			{
				return;
			}
			LocalZoneManager.ZoneConnection zoneConnection;
			if (LocalZoneManager.m_zoneConnections.TryGetValue(zp.TargetZone, out zoneConnection))
			{
				zoneConnection.RemoveZonePoint(zp);
				LocalZoneManager.CleanupConnection(zoneConnection);
			}
		}

		// Token: 0x060027BC RID: 10172 RVA: 0x0005BDD0 File Offset: 0x00059FD0
		private static void CleanupConnection(LocalZoneManager.ZoneConnection connection)
		{
			if (connection.NZonePoints <= 0)
			{
				LocalZoneManager.m_zoneConnections.Remove(connection.TargetZone);
			}
		}

		// Token: 0x060027BD RID: 10173 RVA: 0x0005BDEC File Offset: 0x00059FEC
		public static void RegisterPlayerSpawn(PlayerSpawn ps)
		{
			LocalZoneManager.m_playerSpawns.Add(ps);
			if (ps.IsDefault)
			{
				LocalZoneManager.DefaultPlayerSpawn = ps;
			}
		}

		// Token: 0x060027BE RID: 10174 RVA: 0x0005BE07 File Offset: 0x0005A007
		public static void DeregisterPlayerSpawn(PlayerSpawn ps)
		{
			LocalZoneManager.m_playerSpawns.Remove(ps);
			if (ps.IsDefault)
			{
				LocalZoneManager.DefaultPlayerSpawn = null;
			}
		}

		// Token: 0x060027BF RID: 10175 RVA: 0x0005BE23 File Offset: 0x0005A023
		public static void RegisterSpawnVolumeOverride(SpawnVolumeOverride svo)
		{
			LocalZoneManager.m_spawnVolumeOverrides.Add(svo);
		}

		// Token: 0x060027C0 RID: 10176 RVA: 0x0005BE30 File Offset: 0x0005A030
		public static void DeregisterSpawnVolumeOverride(SpawnVolumeOverride svo)
		{
			LocalZoneManager.m_spawnVolumeOverrides.Remove(svo);
		}

		// Token: 0x060027C1 RID: 10177 RVA: 0x0005BE3E File Offset: 0x0005A03E
		public static void RegisterBackpackVolumeOverride(BackpackVolumeOverride bvo)
		{
			LocalZoneManager.m_backpackVolumeOverrides.Add(bvo);
		}

		// Token: 0x060027C2 RID: 10178 RVA: 0x0005BE4B File Offset: 0x0005A04B
		public static void DeregisterBackpackVolume(BackpackVolumeOverride bvo)
		{
			LocalZoneManager.m_backpackVolumeOverrides.Remove(bvo);
		}

		// Token: 0x060027C3 RID: 10179 RVA: 0x00138B5C File Offset: 0x00136D5C
		public static bool TryGetBackpackVolumeOverride(Vector3 pos, out BackpackVolumeOverride backpackVolumeOverride)
		{
			backpackVolumeOverride = null;
			for (int i = 0; i < LocalZoneManager.m_backpackVolumeOverrides.Count; i++)
			{
				if (LocalZoneManager.m_backpackVolumeOverrides[i].IsWithinBounds(pos))
				{
					backpackVolumeOverride = LocalZoneManager.m_backpackVolumeOverrides[i];
					return true;
				}
			}
			return false;
		}

		// Token: 0x060027C4 RID: 10180 RVA: 0x0005BE59 File Offset: 0x0005A059
		public static void RegisterBackpackRelocationVolume(BackpackRelocationVolume relocationVolume)
		{
			LocalZoneManager.m_backpackRelocationVolumes.Add(relocationVolume);
		}

		// Token: 0x060027C5 RID: 10181 RVA: 0x0005BE66 File Offset: 0x0005A066
		public static void DeregisterBackpackRelocationVolume(BackpackRelocationVolume relocationVolume)
		{
			LocalZoneManager.m_backpackRelocationVolumes.Remove(relocationVolume);
		}

		// Token: 0x060027C6 RID: 10182 RVA: 0x00138BA4 File Offset: 0x00136DA4
		public static bool TryRelocationBackpack(Vector3 corpsePosition, out Vector3 newPosition)
		{
			newPosition = corpsePosition;
			if (LocalZoneManager.m_backpackRelocationVolumes != null && LocalZoneManager.m_backpackRelocationVolumes.Count > 0)
			{
				for (int i = 0; i < LocalZoneManager.m_backpackRelocationVolumes.Count; i++)
				{
					if (LocalZoneManager.m_backpackRelocationVolumes[i] && LocalZoneManager.m_backpackRelocationVolumes[i].RelocationTarget && LocalZoneManager.m_backpackRelocationVolumes[i].IsWithinBounds(corpsePosition))
					{
						newPosition = LocalZoneManager.m_backpackRelocationVolumes[i].RelocationTarget.GetPosition();
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060027C7 RID: 10183 RVA: 0x0005BE74 File Offset: 0x0005A074
		public static void RegisterPvpCollider(PvpCollider pvpCollider)
		{
			LocalZoneManager.m_pvpColliders.Add(pvpCollider);
		}

		// Token: 0x060027C8 RID: 10184 RVA: 0x0005BE81 File Offset: 0x0005A081
		public static void DeregisterPvpCollider(PvpCollider pvpCollider)
		{
			LocalZoneManager.m_pvpColliders.Remove(pvpCollider);
		}

		// Token: 0x060027C9 RID: 10185 RVA: 0x00138C3C File Offset: 0x00136E3C
		public static bool IsWithinPvpCollider(Vector3 pos)
		{
			if (LocalZoneManager.m_pvpColliders != null && LocalZoneManager.m_pvpColliders.Count > 0)
			{
				for (int i = 0; i < LocalZoneManager.m_pvpColliders.Count; i++)
				{
					if (LocalZoneManager.m_pvpColliders[i] && LocalZoneManager.m_pvpColliders[i].IsWithinBounds(pos))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x1700083D RID: 2109
		// (get) Token: 0x060027CA RID: 10186 RVA: 0x0005BE8F File Offset: 0x0005A08F
		public static int PlayerSpawnCount
		{
			get
			{
				return LocalZoneManager.m_playerSpawns.Count;
			}
		}

		// Token: 0x060027CB RID: 10187 RVA: 0x0005BE9B File Offset: 0x0005A09B
		public static IEnumerable<PlayerSpawn> GetPlayerSpawns()
		{
			int num;
			for (int i = 0; i < LocalZoneManager.m_playerSpawns.Count; i = num + 1)
			{
				yield return LocalZoneManager.m_playerSpawns[i];
				num = i;
			}
			yield break;
		}

		// Token: 0x060027CC RID: 10188 RVA: 0x0005BEA4 File Offset: 0x0005A0A4
		public static void CacheClosestPlayerSpawn(Vector3 pos)
		{
			LocalZoneManager.SpawnPointDistanceManager.CacheClosest(pos);
		}

		// Token: 0x060027CD RID: 10189 RVA: 0x00138C9C File Offset: 0x00136E9C
		public static PlayerSpawn GetClosestPlayerSpawn(Vector3 pos)
		{
			PlayerSpawn result;
			if (!LocalZoneManager.SpawnPointDistanceManager.TryGetClosest(pos, out result))
			{
				return LocalZoneManager.DefaultPlayerSpawn;
			}
			return result;
		}

		// Token: 0x060027CE RID: 10190 RVA: 0x00138CC0 File Offset: 0x00136EC0
		public static bool TryGetSpawnVolumeOverride(Vector3 position, out SpawnVolumeOverride spawnVolumeOverride)
		{
			spawnVolumeOverride = null;
			for (int i = 0; i < LocalZoneManager.m_spawnVolumeOverrides.Count; i++)
			{
				PlayerSpawn playerSpawn;
				if (LocalZoneManager.m_spawnVolumeOverrides[i] != null && LocalZoneManager.m_spawnVolumeOverrides[i].IsWithinBounds(position, out playerSpawn))
				{
					spawnVolumeOverride = LocalZoneManager.m_spawnVolumeOverrides[i];
					break;
				}
			}
			return spawnVolumeOverride != null;
		}

		// Token: 0x060027CF RID: 10191 RVA: 0x00138D24 File Offset: 0x00136F24
		private static bool TryGetSpawnVolumeOverrideSpawn(GameEntity entity, out PlayerSpawn spawn)
		{
			spawn = null;
			for (int i = 0; i < LocalZoneManager.m_spawnVolumeOverrides.Count; i++)
			{
				PlayerSpawn playerSpawn;
				if (LocalZoneManager.m_spawnVolumeOverrides[i] != null && LocalZoneManager.m_spawnVolumeOverrides[i].IsActive(entity, out playerSpawn))
				{
					spawn = playerSpawn;
					break;
				}
			}
			return spawn != null;
		}

		// Token: 0x060027D0 RID: 10192 RVA: 0x00138D80 File Offset: 0x00136F80
		private static bool TryGetSpawnVolumeOverrideSpawn(Vector3 pos, CharacterRecord record, out PlayerSpawn spawn)
		{
			spawn = null;
			for (int i = 0; i < LocalZoneManager.m_spawnVolumeOverrides.Count; i++)
			{
				PlayerSpawn playerSpawn;
				if (LocalZoneManager.m_spawnVolumeOverrides[i] != null && LocalZoneManager.m_spawnVolumeOverrides[i].IsActive(pos, record, out playerSpawn))
				{
					spawn = playerSpawn;
					break;
				}
			}
			return spawn != null;
		}

		// Token: 0x060027D1 RID: 10193 RVA: 0x00138DDC File Offset: 0x00136FDC
		public static PlayerSpawn GetRespawnPoint(GameEntity entity)
		{
			if (!entity)
			{
				return LocalZoneManager.DefaultPlayerSpawn;
			}
			PlayerSpawn result;
			if (LocalZoneManager.TryGetSpawnVolumeOverrideSpawn(entity, out result))
			{
				return result;
			}
			return LocalZoneManager.GetClosestDiscoveredPlayerSpawn(entity);
		}

		// Token: 0x060027D2 RID: 10194 RVA: 0x00138E0C File Offset: 0x0013700C
		public static PlayerSpawn GetRespawnPoint(Vector3 pos, CharacterRecord record, GameEntity entity)
		{
			PlayerSpawn result;
			if (LocalZoneManager.TryGetSpawnVolumeOverrideSpawn(pos, record, out result))
			{
				return result;
			}
			return LocalZoneManager.GetClosestDiscoveredPlayerSpawn(pos, record, entity);
		}

		// Token: 0x060027D3 RID: 10195 RVA: 0x0005BEB1 File Offset: 0x0005A0B1
		private static PlayerSpawn GetClosestDiscoveredPlayerSpawn(GameEntity entity)
		{
			if (!entity || entity.CollectionController == null)
			{
				return LocalZoneManager.GetDefaultPlayerSpawn(entity);
			}
			return LocalZoneManager.GetClosestDiscoveredPlayerSpawn(entity.gameObject.transform.position, entity.CollectionController.Record, entity);
		}

		// Token: 0x060027D4 RID: 10196 RVA: 0x00138E30 File Offset: 0x00137030
		public static PlayerSpawn GetClosestDiscoveredPlayerSpawn(Vector3 pos, CharacterRecord record, GameEntity entity)
		{
			List<UniqueId> list;
			PlayerSpawn result;
			if (record != null && record.Discoveries != null && record.Discoveries.TryGetValue((ZoneId)LocalZoneManager.ZoneRecord.ZoneId, out list) && list != null && list.Count > 0 && LocalZoneManager.SpawnPointDistanceManager.TryGetClosestKnown(pos, list, out result))
			{
				return result;
			}
			if (!(entity != null))
			{
				return LocalZoneManager.DefaultPlayerSpawn;
			}
			return LocalZoneManager.GetDefaultPlayerSpawn(entity);
		}

		// Token: 0x060027D5 RID: 10197 RVA: 0x00138E94 File Offset: 0x00137094
		public static void RegisterZoneBoundary(Bounds bounds)
		{
			LocalZoneManager.ZoneBounds = bounds;
			Vector3 vector = bounds.center - bounds.extents;
			Vector3 vector2 = bounds.center + bounds.extents;
			LocalZoneManager.Range = new BoundedRange[]
			{
				new BoundedRange(vector.x, vector2.x, LocalZoneManager.kPrecision),
				new BoundedRange(vector.y, vector2.y, LocalZoneManager.kPrecision),
				new BoundedRange(vector.z, vector2.z, LocalZoneManager.kPrecision)
			};
			NetworkManager.Range = LocalZoneManager.Range;
			Debug.Log(string.Format("Setting zone range to :{0} --> {1}  With a precision of: {2}", vector, vector2, LocalZoneManager.kPrecision));
		}

		// Token: 0x060027D6 RID: 10198 RVA: 0x00138F54 File Offset: 0x00137154
		public static ZonePoint GetZonePoint(ZoneId sourceZone, int targetIndex)
		{
			LocalZoneManager.ZoneConnection zoneConnection;
			if (LocalZoneManager.m_zoneConnections.TryGetValue(sourceZone, out zoneConnection))
			{
				return zoneConnection.GetZonePointForIndex(targetIndex);
			}
			return null;
		}

		// Token: 0x060027D7 RID: 10199 RVA: 0x0005BEEB File Offset: 0x0005A0EB
		public static void RegisterTeleporter(InteractiveZonePointTeleporter teleporter)
		{
			if (teleporter)
			{
				LocalZoneManager.m_teleporters.AddOrReplace(teleporter.DiscoveryId, teleporter);
			}
		}

		// Token: 0x060027D8 RID: 10200 RVA: 0x0005BF06 File Offset: 0x0005A106
		public static void DeregisterTeleporter(InteractiveZonePointTeleporter teleporter)
		{
			if (teleporter)
			{
				LocalZoneManager.m_teleporters.Remove(teleporter.DiscoveryId);
			}
		}

		// Token: 0x060027D9 RID: 10201 RVA: 0x00138F7C File Offset: 0x0013717C
		public static ZonePoint GetZonePointForDiscovery(UniqueId targetDiscoveryId)
		{
			InteractiveZonePointTeleporter interactiveZonePointTeleporter;
			if (LocalZoneManager.TryGetTeleporter(targetDiscoveryId, out interactiveZonePointTeleporter))
			{
				return interactiveZonePointTeleporter.ZonePoint;
			}
			return null;
		}

		// Token: 0x060027DA RID: 10202 RVA: 0x00138F9C File Offset: 0x0013719C
		public static TargetPosition GetTargetPosition(CharacterZoningState zoningState)
		{
			if (zoningState != null)
			{
				ZoningState state = zoningState.State;
				if (state == ZoningState.DiscoveryTeleport)
				{
					return LocalZoneManager.GetZonePointForDiscovery(zoningState.TargetDiscoveryId);
				}
				if (state == ZoningState.GroupTeleport)
				{
					DiscoveryMapTeleportDestination discoveryMapTeleportDestination;
					if (LocalZoneManager.TryGetMapTeleportDestination(zoningState.TargetDiscoveryId, out discoveryMapTeleportDestination) && discoveryMapTeleportDestination)
					{
						return discoveryMapTeleportDestination.TargetPosition;
					}
				}
			}
			return null;
		}

		// Token: 0x060027DB RID: 10203 RVA: 0x0005BF21 File Offset: 0x0005A121
		public static bool TryGetTeleporter(UniqueId discoveryId, out InteractiveZonePointTeleporter teleporter)
		{
			return LocalZoneManager.m_teleporters.TryGetValue(discoveryId, out teleporter);
		}

		// Token: 0x060027DC RID: 10204 RVA: 0x00138FE8 File Offset: 0x001371E8
		public static void RegisterMapTeleportDestination(DiscoveryMapTeleportDestination mapTeleportDestination)
		{
			if (mapTeleportDestination && mapTeleportDestination.DiscoveryProfile && mapTeleportDestination.DiscoveryProfile.Id != UniqueId.Empty)
			{
				LocalZoneManager.m_mapTeleportDestinations.AddOrReplace(mapTeleportDestination.DiscoveryProfile.Id, mapTeleportDestination);
			}
		}

		// Token: 0x060027DD RID: 10205 RVA: 0x00139038 File Offset: 0x00137238
		public static void DeregisterMapTeleportDestination(DiscoveryMapTeleportDestination mapTeleportDestination)
		{
			if (mapTeleportDestination && mapTeleportDestination.DiscoveryProfile && mapTeleportDestination.DiscoveryProfile.Id != UniqueId.Empty)
			{
				LocalZoneManager.m_mapTeleportDestinations.Remove(mapTeleportDestination.DiscoveryProfile.Id);
			}
		}

		// Token: 0x060027DE RID: 10206 RVA: 0x0005BF2F File Offset: 0x0005A12F
		public static bool TryGetMapTeleportDestination(UniqueId discoveryId, out DiscoveryMapTeleportDestination destination)
		{
			return LocalZoneManager.m_mapTeleportDestinations.TryGetValue(discoveryId, out destination);
		}

		// Token: 0x060027DF RID: 10207 RVA: 0x0005BF3D File Offset: 0x0005A13D
		public static void RegisterMonolith(Monolith monolith)
		{
			LocalZoneManager.m_monoliths.Add(monolith);
		}

		// Token: 0x060027E0 RID: 10208 RVA: 0x0005BF4A File Offset: 0x0005A14A
		public static void DeregisterMonolith(Monolith monolith)
		{
			LocalZoneManager.m_monoliths.Remove(monolith);
		}

		// Token: 0x060027E1 RID: 10209 RVA: 0x00139088 File Offset: 0x00137288
		public static bool HasMonolithInRange(GameEntity entity)
		{
			if (entity && entity.gameObject && entity.gameObject.transform)
			{
				for (int i = 0; i < LocalZoneManager.m_monoliths.Count; i++)
				{
					if (!LocalZoneManager.m_monoliths[i] || !LocalZoneManager.m_monoliths[i].gameObject || !LocalZoneManager.m_monoliths[i].gameObject.transform)
					{
						LocalZoneManager.m_monoliths.RemoveAt(i);
						i--;
					}
					else if ((entity.gameObject.transform.position - LocalZoneManager.m_monoliths[i].gameObject.transform.position).sqrMagnitude <= 36f)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060027E2 RID: 10210 RVA: 0x00139178 File Offset: 0x00137378
		public static void RegisterWorldObject(IWorldObject worldObj)
		{
			if (LocalZoneManager.m_worldObjects == null)
			{
				LocalZoneManager.m_worldObjects = new Dictionary<UniqueId, IWorldObject>(default(UniqueIdComparer));
			}
			LocalZoneManager.m_worldObjects.AddOrReplace(worldObj.WorldId, worldObj);
		}

		// Token: 0x060027E3 RID: 10211 RVA: 0x0005BF58 File Offset: 0x0005A158
		public static void DeregisterWorldObject(IWorldObject worldObj)
		{
			Dictionary<UniqueId, IWorldObject> worldObjects = LocalZoneManager.m_worldObjects;
			if (worldObjects == null)
			{
				return;
			}
			worldObjects.Remove(worldObj.WorldId);
		}

		// Token: 0x060027E4 RID: 10212 RVA: 0x0005BF70 File Offset: 0x0005A170
		public static bool TryGetWorldObject(UniqueId id, out IWorldObject obj)
		{
			if (LocalZoneManager.m_worldObjects == null)
			{
				obj = null;
				return false;
			}
			return LocalZoneManager.m_worldObjects.TryGetValue(id, out obj);
		}

		// Token: 0x060027E5 RID: 10213 RVA: 0x001391B8 File Offset: 0x001373B8
		public static void RefreshVegetationStudio()
		{
			if (SkyDomeManager.VSPManager == null)
			{
				return;
			}
			BiomeMaskArea[] array = UnityEngine.Object.FindObjectsOfType<BiomeMaskArea>();
			int num = (array == null) ? 0 : array.Length;
			if (num > 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i].UpdateBiomeMask();
				}
			}
			VegetationMask[] array2 = UnityEngine.Object.FindObjectsOfType<VegetationMask>();
			int num2 = (array2 == null) ? 0 : array2.Length;
			if (num2 > 0)
			{
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j].UpdateVegetationMask();
				}
			}
			if (num > 0 || num2 > 0)
			{
				Debug.Log(string.Concat(new string[]
				{
					"Refreshed ",
					num.ToString(),
					" biomes and ",
					num2.ToString(),
					" masks for VSP."
				}));
			}
		}

		// Token: 0x060027E6 RID: 10214 RVA: 0x0005BF8A File Offset: 0x0005A18A
		public static string GetFormattedZoneName(string zoneName, string zoneNameSuffix)
		{
			if (!string.IsNullOrEmpty(zoneNameSuffix))
			{
				return ZString.Format<string, string>("{0}: {1}", zoneName, zoneNameSuffix);
			}
			return zoneName;
		}

		// Token: 0x060027E7 RID: 10215 RVA: 0x00139278 File Offset: 0x00137478
		public static string GetFormattedZoneName(string zoneName, SubZoneId subZoneId)
		{
			if (subZoneId == SubZoneId.None)
			{
				return zoneName;
			}
			string subZoneDisplayName = subZoneId.GetSubZoneDisplayName();
			if (!subZoneId.IncludeZoneNameInDisplayName())
			{
				return subZoneDisplayName;
			}
			return ZString.Format<string, string>("{0}: {1}", zoneName, subZoneDisplayName);
		}

		// Token: 0x060027E8 RID: 10216 RVA: 0x001392A8 File Offset: 0x001374A8
		public static string GetFormattedZoneName(ZoneId zoneId, SubZoneId subZoneId)
		{
			ZoneRecord zoneRecord = SessionData.GetZoneRecord(zoneId);
			if (zoneRecord == null)
			{
				return "UNKNOWN";
			}
			return LocalZoneManager.GetFormattedZoneName(zoneRecord.DisplayName, subZoneId);
		}

		// Token: 0x04002953 RID: 10579
		private static readonly Dictionary<ZoneId, LocalZoneManager.ZoneConnection> m_zoneConnections = new Dictionary<ZoneId, LocalZoneManager.ZoneConnection>(default(ZoneIdComparer));

		// Token: 0x04002954 RID: 10580
		private static readonly Dictionary<UniqueId, InteractiveZonePointTeleporter> m_teleporters = new Dictionary<UniqueId, InteractiveZonePointTeleporter>(default(UniqueIdComparer));

		// Token: 0x04002955 RID: 10581
		private static readonly Dictionary<UniqueId, DiscoveryMapTeleportDestination> m_mapTeleportDestinations = new Dictionary<UniqueId, DiscoveryMapTeleportDestination>(default(UniqueIdComparer));

		// Token: 0x04002956 RID: 10582
		private static readonly List<SpawnVolumeOverride> m_spawnVolumeOverrides = new List<SpawnVolumeOverride>(10);

		// Token: 0x04002957 RID: 10583
		private static readonly List<BackpackVolumeOverride> m_backpackVolumeOverrides = new List<BackpackVolumeOverride>(10);

		// Token: 0x04002958 RID: 10584
		private static readonly List<BackpackRelocationVolume> m_backpackRelocationVolumes = new List<BackpackRelocationVolume>(10);

		// Token: 0x04002959 RID: 10585
		private static readonly List<PvpCollider> m_pvpColliders = new List<PvpCollider>(10);

		// Token: 0x0400295A RID: 10586
		private static readonly List<Monolith> m_monoliths = new List<Monolith>(10);

		// Token: 0x0400295B RID: 10587
		private static float kPrecision = 0.05f;

		// Token: 0x0400295C RID: 10588
		public static BoundedRange[] Range = null;

		// Token: 0x0400295F RID: 10591
		private static ZoneRecord m_zoneRecord = null;

		// Token: 0x04002960 RID: 10592
		private static SpawnPointDistanceManager m_spawnPointDistanceManager = null;

		// Token: 0x04002962 RID: 10594
		private static List<PlayerSpawn> m_playerSpawns = new List<PlayerSpawn>();

		// Token: 0x04002963 RID: 10595
		private static Dictionary<UniqueId, IWorldObject> m_worldObjects = null;

		// Token: 0x0200052A RID: 1322
		private class ZoneConnection
		{
			// Token: 0x1700083E RID: 2110
			// (get) Token: 0x060027EA RID: 10218 RVA: 0x0005BFA2 File Offset: 0x0005A1A2
			public int NZonePoints
			{
				get
				{
					return this.m_zonePoints.Count;
				}
			}

			// Token: 0x060027EB RID: 10219 RVA: 0x0005BFAF File Offset: 0x0005A1AF
			public ZoneConnection(ZoneId targetZone)
			{
				this.m_zonePoints = new Dictionary<int, ZonePoint>();
				this.TargetZone = targetZone;
			}

			// Token: 0x060027EC RID: 10220 RVA: 0x00139394 File Offset: 0x00137594
			public bool AddZonePoint(ZonePoint zp)
			{
				if (zp.TargetZone != this.TargetZone)
				{
					throw new ArgumentException("Mismatched zone point?  Incoming: " + zp.TargetZone.ToString() + " vs. Called: " + this.TargetZone.ToString());
				}
				if (this.m_zonePoints.ContainsKey(zp.TargetZonePointIndex))
				{
					Debug.LogWarning("ZoneConnection for " + this.TargetZone.ToString() + " already has an entry for index " + zp.TargetZonePointIndex.ToString());
					return false;
				}
				this.m_zonePoints.Add(zp.TargetZonePointIndex, zp);
				return true;
			}

			// Token: 0x060027ED RID: 10221 RVA: 0x00139444 File Offset: 0x00137644
			public void RemoveZonePoint(ZonePoint zp)
			{
				if (zp.TargetZone != this.TargetZone)
				{
					throw new ArgumentException("Mismatched zone point?  Incoming: " + zp.TargetZone.ToString() + " vs. Called: " + this.TargetZone.ToString());
				}
				this.m_zonePoints.Remove(zp.TargetZonePointIndex);
			}

			// Token: 0x060027EE RID: 10222 RVA: 0x001394AC File Offset: 0x001376AC
			public ZonePoint GetZonePointForIndex(int index)
			{
				ZonePoint result;
				this.m_zonePoints.TryGetValue(index, out result);
				return result;
			}

			// Token: 0x04002964 RID: 10596
			private readonly Dictionary<int, ZonePoint> m_zonePoints;

			// Token: 0x04002965 RID: 10597
			public readonly ZoneId TargetZone;
		}
	}
}
