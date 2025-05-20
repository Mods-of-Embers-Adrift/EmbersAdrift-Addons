using System;
using Animancer;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Archetypes;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D81 RID: 3457
	public interface IAnimationController
	{
		// Token: 0x17001905 RID: 6405
		// (get) Token: 0x0600681D RID: 26653
		GameObject gameObject { get; }

		// Token: 0x0600681E RID: 26654
		void SetCurrentStanceId(UniqueId stanceId, bool bypassTransitions);

		// Token: 0x17001906 RID: 6406
		// (get) Token: 0x0600681F RID: 26655
		IAnimancerAnimation CurrentAnimationSet { get; }

		// Token: 0x17001907 RID: 6407
		// (get) Token: 0x06006820 RID: 26656
		Vector2 MovementLocomotionVector { get; }

		// Token: 0x06006821 RID: 26657
		void FadeOutState(AnimancerState state);

		// Token: 0x06006822 RID: 26658
		void FadeOutState(AnimancerState state, float fadeDuration);

		// Token: 0x06006823 RID: 26659
		AnimancerState StartSequence(AnimationSequence sequence, IAnimancerStateTracker stateTracker);

		// Token: 0x06006824 RID: 26660
		AnimancerState StartAlchemySequence(IAnimancerStateTracker stateTracker, AlchemyPowerLevel alchemyPowerLevel);

		// Token: 0x06006825 RID: 26661
		bool TriggerHit();

		// Token: 0x06006826 RID: 26662
		void TriggerBlock();

		// Token: 0x06006827 RID: 26663
		void TriggerParry();

		// Token: 0x06006828 RID: 26664
		void TriggerRiposte();

		// Token: 0x06006829 RID: 26665
		void TriggerAvoid();

		// Token: 0x0600682A RID: 26666
		void TriggerJump();

		// Token: 0x0600682B RID: 26667
		bool TriggerEvent(AnimationEventTriggerType type);

		// Token: 0x0600682C RID: 26668
		void AssignSex(CharacterSex sex);

		// Token: 0x0600682D RID: 26669
		void PlayEmote(Emote emote);

		// Token: 0x17001908 RID: 6408
		// (get) Token: 0x0600682E RID: 26670
		// (set) Token: 0x0600682F RID: 26671
		bool PreventIdleTicks { get; set; }
	}
}
