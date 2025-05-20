using System;
using SoL.Game.NPCs;
using SoL.Game.Spawning;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Dungeons
{
	// Token: 0x02000C97 RID: 3223
	public class OverworldDungeonEntranceSpawnController : SpawnController
	{
		// Token: 0x17001768 RID: 5992
		// (get) Token: 0x060061E0 RID: 25056 RVA: 0x00081F28 File Offset: 0x00080128
		public SpawnTier Tier
		{
			get
			{
				return this.m_tier;
			}
		}

		// Token: 0x060061E1 RID: 25057 RVA: 0x00081F30 File Offset: 0x00080130
		protected override void Start()
		{
			if (GameManager.IsServer && OverworldDungeonEntranceSpawnManager.Instance != null)
			{
				OverworldDungeonEntranceSpawnManager.Instance.RegisterSpawnController(this);
			}
		}

		// Token: 0x060061E2 RID: 25058 RVA: 0x00081F51 File Offset: 0x00080151
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (GameManager.IsServer && OverworldDungeonEntranceSpawnManager.Instance != null)
			{
				OverworldDungeonEntranceSpawnManager.Instance.UnregisterSpawnController(this);
			}
		}

		// Token: 0x060061E3 RID: 25059 RVA: 0x00081F78 File Offset: 0x00080178
		public ISpawnLocation GetSpawnLocation()
		{
			return base.SelectSpawnPoint(null);
		}

		// Token: 0x0400555A RID: 21850
		[SerializeField]
		private SpawnTier m_tier;
	}
}
