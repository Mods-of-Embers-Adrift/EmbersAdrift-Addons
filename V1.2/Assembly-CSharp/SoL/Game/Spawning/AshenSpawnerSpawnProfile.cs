using System;
using SoL.Game.Objects;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x0200066B RID: 1643
	[CreateAssetMenu(menuName = "SoL/Profiles/Ashen Spawner Spawn Profile")]
	public class AshenSpawnerSpawnProfile : SpawnProfile
	{
		// Token: 0x0600330E RID: 13070 RVA: 0x00161F3C File Offset: 0x0016013C
		protected override void SpawnInternal(ISpawnController controller, GameEntity gameEntity)
		{
			base.SpawnInternal(controller, gameEntity);
			if (gameEntity)
			{
				if (gameEntity.LocationReplicator)
				{
					gameEntity.LocationReplicator.ServerInit();
				}
				if (gameEntity.AshenController)
				{
					gameEntity.AshenController.ChanceToAshen = this.m_chanceToAshen;
				}
			}
		}

		// Token: 0x04003149 RID: 12617
		[SerializeField]
		private MinMaxFloatRange m_chanceToAshen = new MinMaxFloatRange(0.4f, 0.6f);
	}
}
