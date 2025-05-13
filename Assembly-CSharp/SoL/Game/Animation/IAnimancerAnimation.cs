using System;
using Animancer;
using SoL.Game.EffectSystem;
using SoL.Networking.Database;

namespace SoL.Game.Animation
{
	// Token: 0x02000D7F RID: 3455
	public interface IAnimancerAnimation
	{
		// Token: 0x170018F5 RID: 6389
		// (get) Token: 0x06006802 RID: 26626
		UniqueId Id { get; }

		// Token: 0x170018F6 RID: 6390
		// (get) Token: 0x06006803 RID: 26627
		LinearMixerTransition IdleBlend { get; }

		// Token: 0x06006804 RID: 26628
		LinearMixerTransition GetIdleBlend(CharacterSex sex);

		// Token: 0x170018F7 RID: 6391
		// (get) Token: 0x06006805 RID: 26629
		MixerTransition2D LocomotionBlend { get; }

		// Token: 0x06006806 RID: 26630
		MixerTransition2D GetLocomotionBlend(CharacterSex sex);

		// Token: 0x06006807 RID: 26631
		bool EnableFootIkForLocomotion(CharacterSex sex);

		// Token: 0x170018F8 RID: 6392
		// (get) Token: 0x06006808 RID: 26632
		bool IsCombatStance { get; }

		// Token: 0x170018F9 RID: 6393
		// (get) Token: 0x06006809 RID: 26633
		bool DisableLateralMovement { get; }

		// Token: 0x170018FA RID: 6394
		// (get) Token: 0x0600680A RID: 26634
		AnimationSequence EnterTransitionSequence { get; }

		// Token: 0x170018FB RID: 6395
		// (get) Token: 0x0600680B RID: 26635
		AnimationSequence ExitTransitionSequence { get; }

		// Token: 0x0600680C RID: 26636
		AnimationSequence GetNextAutoAttackSequence(AnimationFlags animFlags);

		// Token: 0x0600680D RID: 26637
		AnimationSequence GetNextIdleTickSequence();

		// Token: 0x0600680E RID: 26638
		AnimationSequence GetIndexedIdleTickSequence(int index);

		// Token: 0x0600680F RID: 26639
		AbilityAnimation GetAbilityAnimation(AnimationExecutionTime exeTime, int index);

		// Token: 0x170018FC RID: 6396
		// (get) Token: 0x06006810 RID: 26640
		AnimationSequence PoseSequence { get; }

		// Token: 0x170018FD RID: 6397
		// (get) Token: 0x06006811 RID: 26641
		AnimationSequence GetHitSequence { get; }

		// Token: 0x170018FE RID: 6398
		// (get) Token: 0x06006812 RID: 26642
		AnimationSequence AvoidSequence { get; }

		// Token: 0x170018FF RID: 6399
		// (get) Token: 0x06006813 RID: 26643
		AnimationSequence BlockSequence { get; }

		// Token: 0x17001900 RID: 6400
		// (get) Token: 0x06006814 RID: 26644
		AnimationSequence ParrySequence { get; }

		// Token: 0x17001901 RID: 6401
		// (get) Token: 0x06006815 RID: 26645
		AnimationSequence RiposteSequence { get; }

		// Token: 0x06006816 RID: 26646
		AnimationSequence GetAlchemySequence(AlchemyPowerLevel alchemyPowerLevel);
	}
}
