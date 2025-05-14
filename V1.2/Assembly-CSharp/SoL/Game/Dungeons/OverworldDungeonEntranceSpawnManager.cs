using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SoL.Game.NPCs;
using SoL.Game.Spawning;
using SoL.Managers;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.Game.Dungeons
{
	// Token: 0x02000C98 RID: 3224
	public class OverworldDungeonEntranceSpawnManager : MonoBehaviour
	{
		// Token: 0x060061E5 RID: 25061 RVA: 0x00081F81 File Offset: 0x00080181
		private void Awake()
		{
			if (OverworldDungeonEntranceSpawnManager.Instance != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			OverworldDungeonEntranceSpawnManager.Instance = this;
		}

		// Token: 0x060061E6 RID: 25062 RVA: 0x00201D54 File Offset: 0x001FFF54
		private void Start()
		{
			if (!GameManager.IsServer)
			{
				return;
			}
			this.m_dungeonEntrancePrefab = GameManager.NetworkManager.NetworkedPrefabs.GetPrefabForIdOrName("EmberDrift_GamePlacement");
			if (this.m_dungeonEntrancePrefab == null)
			{
				Debug.LogWarning("Unable to load Dungeon Entrance prefab!");
				base.enabled = false;
				return;
			}
			if (SceneCompositionManager.IsLoading)
			{
				SceneCompositionManager.ZoneLoaded += this.SceneCompositionManagerOnZoneLoaded;
			}
			else
			{
				this.ExternalRefresh();
			}
			this.m_timeOfNextExpirationCheck = Time.time + 60f;
		}

		// Token: 0x060061E7 RID: 25063 RVA: 0x00081FA2 File Offset: 0x000801A2
		private void SceneCompositionManagerOnZoneLoaded(ZoneId obj)
		{
			SceneCompositionManager.ZoneLoaded -= this.SceneCompositionManagerOnZoneLoaded;
			this.ExternalRefresh();
		}

		// Token: 0x060061E8 RID: 25064 RVA: 0x00201DD4 File Offset: 0x001FFFD4
		private void Update()
		{
			if (Time.time < this.m_timeOfNextExpirationCheck)
			{
				return;
			}
			this.m_timeOfNextExpirationCheck = Time.time + 60f;
			DateTime utcNow = DateTime.UtcNow;
			bool flag = false;
			int num = 0;
			for (int i = 0; i < this.m_entrances.Count; i++)
			{
				if (this.m_entrances[i] && this.m_entrances[i].Record != null)
				{
					if (this.m_entrances[i].Record.IsActive())
					{
						num++;
					}
					else
					{
						this.DestroyDungeonEntrance(this.m_entrances[i], "local expiration");
						flag = true;
						i--;
					}
				}
			}
			if (flag)
			{
				Debug.Log("[DUNGEON] Active Entrances: " + num.ToString());
			}
		}

		// Token: 0x060061E9 RID: 25065 RVA: 0x00201E9C File Offset: 0x0020009C
		public void ExternalRefresh()
		{
			OverworldDungeonEntranceSpawnManager.<ExternalRefresh>d__12 <ExternalRefresh>d__;
			<ExternalRefresh>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ExternalRefresh>d__.<>4__this = this;
			<ExternalRefresh>d__.<>1__state = -1;
			<ExternalRefresh>d__.<>t__builder.Start<OverworldDungeonEntranceSpawnManager.<ExternalRefresh>d__12>(ref <ExternalRefresh>d__);
		}

		// Token: 0x060061EA RID: 25066 RVA: 0x00201ED4 File Offset: 0x002000D4
		private void DestroyDungeonEntrance(BaseDungeonEntrance entrance, string context)
		{
			if (entrance.SpawnLocation != null)
			{
				entrance.SpawnLocation.Occupied = false;
			}
			Debug.Log("[DUNGEON] DESTROYING Entrance " + entrance.Record.Id + " for context " + context);
			this.m_entrances.Remove(entrance.Record.Id);
			UnityEngine.Object.Destroy(entrance.gameObject);
		}

		// Token: 0x060061EB RID: 25067 RVA: 0x00201F38 File Offset: 0x00200138
		private ISpawnLocation GetSpawnLocationForTier(SpawnTier tier)
		{
			ISpawnLocation result = null;
			List<OverworldDungeonEntranceSpawnController> list;
			if (this.m_spawnControllers.TryGetValue(tier, out list) && list.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, list.Count);
				result = list[index].GetSpawnLocation();
			}
			return result;
		}

		// Token: 0x060061EC RID: 25068 RVA: 0x00201F7C File Offset: 0x0020017C
		public void RegisterSpawnController(OverworldDungeonEntranceSpawnController controller)
		{
			List<OverworldDungeonEntranceSpawnController> list;
			if (this.m_spawnControllers.TryGetValue(controller.Tier, out list))
			{
				if (!list.Contains(controller))
				{
					list.Add(controller);
					return;
				}
			}
			else
			{
				list = new List<OverworldDungeonEntranceSpawnController>
				{
					controller
				};
				this.m_spawnControllers.Add(controller.Tier, list);
			}
		}

		// Token: 0x060061ED RID: 25069 RVA: 0x00201FD0 File Offset: 0x002001D0
		public void UnregisterSpawnController(OverworldDungeonEntranceSpawnController controller)
		{
			List<OverworldDungeonEntranceSpawnController> list;
			if (this.m_spawnControllers.TryGetValue(controller.Tier, out list))
			{
				list.Remove(controller);
			}
		}

		// Token: 0x060061EE RID: 25070 RVA: 0x00201FFC File Offset: 0x002001FC
		public CharacterLocation GetInactiveLocation(int sourceZoneId, int targetIndex)
		{
			for (int i = 0; i < this.m_allEntranceRecords.Count; i++)
			{
				if (this.m_allEntranceRecords[i].Status == DungeonEntranceStatus.Inactive && this.m_allEntranceRecords[i].DungeonZoneId == sourceZoneId && this.m_allEntranceRecords[i].ZonePointIndex == targetIndex)
				{
					return this.m_allEntranceRecords[i].Location;
				}
			}
			return null;
		}

		// Token: 0x0400555B RID: 21851
		private const string kPrefabName = "EmberDrift_GamePlacement";

		// Token: 0x0400555C RID: 21852
		private const float kExpirationTickRate = 60f;

		// Token: 0x0400555D RID: 21853
		public static OverworldDungeonEntranceSpawnManager Instance;

		// Token: 0x0400555E RID: 21854
		private readonly DictionaryList<string, BaseDungeonEntrance> m_entrances = new DictionaryList<string, BaseDungeonEntrance>(false);

		// Token: 0x0400555F RID: 21855
		private readonly Dictionary<SpawnTier, List<OverworldDungeonEntranceSpawnController>> m_spawnControllers = new Dictionary<SpawnTier, List<OverworldDungeonEntranceSpawnController>>();

		// Token: 0x04005560 RID: 21856
		private List<DungeonEntranceRecord> m_allEntranceRecords;

		// Token: 0x04005561 RID: 21857
		private GameObject m_dungeonEntrancePrefab;

		// Token: 0x04005562 RID: 21858
		private float m_timeOfNextExpirationCheck = float.MaxValue;
	}
}
