using System;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A23 RID: 2595
	public interface IAbilityCooldown
	{
		// Token: 0x170011C3 RID: 4547
		// (get) Token: 0x06005038 RID: 20536
		bool PauseWhileHandSwapActive { get; }

		// Token: 0x170011C4 RID: 4548
		// (get) Token: 0x06005039 RID: 20537
		bool PauseWhileExecuting { get; }

		// Token: 0x170011C5 RID: 4549
		// (get) Token: 0x0600503A RID: 20538
		bool ConsiderHaste { get; }

		// Token: 0x170011C6 RID: 4550
		// (get) Token: 0x0600503B RID: 20539
		bool ClampHasteTo100 { get; }

		// Token: 0x0600503C RID: 20540
		float GetCooldown(GameEntity entity, ExecutionCache executionCache, float level);
	}
}
