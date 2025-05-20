using System;

namespace SoL.Game.Spawning
{
	// Token: 0x020006CD RID: 1741
	public class SpawnControllerMovable : SpawnController
	{
		// Token: 0x17000B9C RID: 2972
		// (get) Token: 0x060034F1 RID: 13553 RVA: 0x00064456 File Offset: 0x00062656
		protected override int SpawnIndex
		{
			get
			{
				return this.m_spawnIndex;
			}
		}

		// Token: 0x060034F2 RID: 13554 RVA: 0x001667B0 File Offset: 0x001649B0
		protected override void MidSpawnMonitorCycle()
		{
			base.MidSpawnMonitorCycle();
			int num = 0;
			for (int i = this.m_activeSpawns.Count - 1; i >= 0; i--)
			{
				if (this.m_activeSpawns[i].Index != this.SpawnIndex && (this.m_activeSpawns[i].Entity.NetworkEntity.NObservers <= 0 || !SpawnController.HasPlayersNearby(this.m_activeSpawns[i].Entity)))
				{
					this.m_activeSpawns[i].Despawn();
					this.m_activeSpawns.RemoveAt(i);
					num++;
					if (num >= 3)
					{
						break;
					}
				}
			}
		}

		// Token: 0x060034F3 RID: 13555 RVA: 0x0006445E File Offset: 0x0006265E
		internal void IncrementSpawnIndex()
		{
			this.m_spawnIndex++;
		}

		// Token: 0x04003323 RID: 13091
		private const int kMaxDespawnPerCycle = 3;

		// Token: 0x04003324 RID: 13092
		private int m_spawnIndex;
	}
}
