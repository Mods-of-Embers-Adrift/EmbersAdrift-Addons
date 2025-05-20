using System;

namespace SoL.Game.Spawning
{
	// Token: 0x020006CC RID: 1740
	public class SpawnControllerInstant : SpawnController
	{
		// Token: 0x060034EF RID: 13551 RVA: 0x00064421 File Offset: 0x00062621
		protected override bool UnregisterSpawnLocationEarly(GameEntity entity)
		{
			if (!entity || !entity.VitalsReplicator)
			{
				return base.UnregisterSpawnLocationEarly(entity);
			}
			return entity.VitalsReplicator.CurrentHealthState.Value != HealthState.Alive;
		}
	}
}
