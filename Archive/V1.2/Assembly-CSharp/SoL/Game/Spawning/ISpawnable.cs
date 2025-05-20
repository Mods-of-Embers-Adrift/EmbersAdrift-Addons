using System;

namespace SoL.Game.Spawning
{
	// Token: 0x0200067C RID: 1660
	public interface ISpawnable
	{
		// Token: 0x17000B09 RID: 2825
		// (get) Token: 0x06003357 RID: 13143
		bool BypassCanReachCheck { get; }

		// Token: 0x17000B0A RID: 2826
		// (get) Token: 0x06003358 RID: 13144
		GameEntity GameEntity { get; }

		// Token: 0x06003359 RID: 13145
		void Spawned();
	}
}
