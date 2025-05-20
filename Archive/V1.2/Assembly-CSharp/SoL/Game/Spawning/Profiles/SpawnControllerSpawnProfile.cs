using System;
using UnityEngine;

namespace SoL.Game.Spawning.Profiles
{
	// Token: 0x020006DA RID: 1754
	[CreateAssetMenu(menuName = "SoL/Profiles/Spawn Controller Spawn")]
	public class SpawnControllerSpawnProfile : NpcSpawnProfileV2
	{
		// Token: 0x17000BBF RID: 3007
		// (get) Token: 0x06003531 RID: 13617 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool m_assignPositionUpdater
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003532 RID: 13618 RVA: 0x00166BD4 File Offset: 0x00164DD4
		protected override void SpawnInternal(ISpawnController controller, GameEntity gameEntity)
		{
			base.SpawnInternal(controller, gameEntity);
			if (gameEntity.SpawnController != null)
			{
				gameEntity.SpawnController.UpdateSpawnParameters(controller);
				gameEntity.SpawnController.ReplaceSpawnProfiles(this.m_spawnProfiles);
				gameEntity.SpawnController.ReplaceTargetPopulationThresholds(this.m_targetPopulationThresholds);
			}
		}

		// Token: 0x04003358 RID: 13144
		private const string kGrouping = "Spawn Controller";

		// Token: 0x04003359 RID: 13145
		[SerializeField]
		private SpawnProfileData m_spawnProfiles;

		// Token: 0x0400335A RID: 13146
		[SerializeField]
		private TargetPopulationThreshold[] m_targetPopulationThresholds;
	}
}
