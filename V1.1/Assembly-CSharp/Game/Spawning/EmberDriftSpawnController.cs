using System;
using SoL.Game;
using SoL.Game.Dungeons;
using SoL.Game.Spawning;
using UnityEngine;

namespace Game.Spawning
{
	// Token: 0x020001FD RID: 509
	public class EmberDriftSpawnController : MonoBehaviour
	{
		// Token: 0x06001181 RID: 4481 RVA: 0x0004E7BC File Offset: 0x0004C9BC
		private void Awake()
		{
			if (this.m_spawnController == null)
			{
				base.enabled = false;
				return;
			}
			this.m_spawnController.gameObject.SetActive(false);
		}

		// Token: 0x06001182 RID: 4482 RVA: 0x000E3BDC File Offset: 0x000E1DDC
		private void Start()
		{
			if (this.m_spawnController == null)
			{
				return;
			}
			if (EmberDriftSpawnController.DungeonEntrances == null)
			{
				EmberDriftSpawnController.DungeonEntrances = UnityEngine.Object.FindObjectsOfType<OverworldDungeonEntranceAlwaysOn>();
			}
			if (EmberDriftSpawnController.DungeonEntrances != null)
			{
				for (int i = 0; i < EmberDriftSpawnController.DungeonEntrances.Length; i++)
				{
					if (EmberDriftSpawnController.DungeonEntrances[i].SubZone == this.m_subZoneId)
					{
						EmberDriftSpawnController.DungeonEntrances[i].RegisterSpawnController(this.m_spawnController);
					}
				}
			}
		}

		// Token: 0x04000EDE RID: 3806
		private static OverworldDungeonEntranceAlwaysOn[] DungeonEntrances;

		// Token: 0x04000EDF RID: 3807
		[SerializeField]
		private SubZoneId m_subZoneId;

		// Token: 0x04000EE0 RID: 3808
		[SerializeField]
		private SpawnControllerMovable m_spawnController;
	}
}
