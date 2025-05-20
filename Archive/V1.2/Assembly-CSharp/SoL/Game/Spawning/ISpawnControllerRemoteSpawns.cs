using System;

namespace SoL.Game.Spawning
{
	// Token: 0x0200067E RID: 1662
	public interface ISpawnControllerRemoteSpawns : ISpawnController
	{
		// Token: 0x17000B17 RID: 2839
		// (get) Token: 0x0600336C RID: 13164
		// (set) Token: 0x0600336D RID: 13165
		int Level { get; set; }

		// Token: 0x17000B18 RID: 2840
		// (get) Token: 0x0600336E RID: 13166
		// (set) Token: 0x0600336F RID: 13167
		SpawnBehaviorType BehaviorType { get; set; }

		// Token: 0x06003370 RID: 13168
		SpawnProfile GetSpawnProfile(string spawnName);

		// Token: 0x06003371 RID: 13169
		string GetRemoteNames(string filter);
	}
}
