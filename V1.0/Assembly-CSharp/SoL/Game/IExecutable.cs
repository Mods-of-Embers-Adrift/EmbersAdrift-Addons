using System;
using SoL.Game.Animation;
using SoL.Game.EffectSystem;

namespace SoL.Game
{
	// Token: 0x020005C0 RID: 1472
	public interface IExecutable : IVfxSource
	{
		// Token: 0x170009DF RID: 2527
		// (get) Token: 0x06002E8F RID: 11919
		EffectSourceType Type { get; }

		// Token: 0x06002E90 RID: 11920
		ExecutionParams GetExecutionParams(float level, AlchemyPowerLevel alchemyPowerLevel = AlchemyPowerLevel.None);

		// Token: 0x06002E91 RID: 11921
		bool PreExecution(ExecutionCache executionCache);

		// Token: 0x06002E92 RID: 11922
		bool ContinuedExecution(ExecutionCache executionCache, float executionProgress);

		// Token: 0x06002E93 RID: 11923
		void PostExecution(ExecutionCache executionCache);

		// Token: 0x170009E0 RID: 2528
		// (get) Token: 0x06002E94 RID: 11924
		bool AllowStaminaRegenDuringExecution { get; }

		// Token: 0x170009E1 RID: 2529
		// (get) Token: 0x06002E95 RID: 11925
		bool AllowAlchemy { get; }

		// Token: 0x06002E96 RID: 11926
		bool HasValidAlchemyOverride(float level, AlchemyPowerLevel alchemyPowerLevel);

		// Token: 0x170009E2 RID: 2530
		// (get) Token: 0x06002E97 RID: 11927
		string DisplayName { get; }

		// Token: 0x06002E98 RID: 11928
		bool UseAutoAttackAnimation();

		// Token: 0x06002E99 RID: 11929
		bool TryGetAbilityAnimation(GameEntity entity, out AbilityAnimation animation);

		// Token: 0x170009E3 RID: 2531
		// (get) Token: 0x06002E9A RID: 11930
		bool TriggerGlobalCooldown { get; }

		// Token: 0x06002E9B RID: 11931
		bool MeetsRequirementsForUI(GameEntity entity, float level);

		// Token: 0x170009E4 RID: 2532
		// (get) Token: 0x06002E9C RID: 11932
		float? DeferHandIkDuration { get; }

		// Token: 0x170009E5 RID: 2533
		// (get) Token: 0x06002E9D RID: 11933
		bool IsLearning { get; }

		// Token: 0x170009E6 RID: 2534
		// (get) Token: 0x06002E9E RID: 11934
		StanceFlags ValidStances { get; }

		// Token: 0x170009E7 RID: 2535
		// (get) Token: 0x06002E9F RID: 11935
		AutoAttackStateChange AutoAttackState { get; }

		// Token: 0x170009E8 RID: 2536
		// (get) Token: 0x06002EA0 RID: 11936
		bool PreventRotation { get; }

		// Token: 0x170009E9 RID: 2537
		// (get) Token: 0x06002EA1 RID: 11937
		float ExecutionTime { get; }
	}
}
