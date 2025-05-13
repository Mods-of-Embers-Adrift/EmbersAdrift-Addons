using System;
using System.Collections.Generic;
using Animancer;
using SoL.Game.EffectSystem;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D60 RID: 3424
	[Obsolete]
	public class AnimancerController : GameEntityComponent, IAnimationController
	{
		// Token: 0x170018C2 RID: 6338
		// (get) Token: 0x0600672E RID: 26414 RVA: 0x00085647 File Offset: 0x00083847
		private Dictionary<object, AnimancerStateSettings> Playing
		{
			get
			{
				if (this.m_playing == null)
				{
					this.m_playing = new Dictionary<object, AnimancerStateSettings>();
				}
				return this.m_playing;
			}
		}

		// Token: 0x170018C3 RID: 6339
		// (get) Token: 0x0600672F RID: 26415 RVA: 0x00085662 File Offset: 0x00083862
		public Vector2 MovementLocomotionVector
		{
			get
			{
				return this.m_locomotion;
			}
		}

		// Token: 0x170018C4 RID: 6340
		// (get) Token: 0x06006730 RID: 26416 RVA: 0x0008566A File Offset: 0x0008386A
		public IAnimancerAnimation CurrentAnimationSet
		{
			get
			{
				if (this.CurrentAnimationStance == null || this.CurrentAnimationStance.Set == null)
				{
					return null;
				}
				return this.CurrentAnimationStance.Set;
			}
		}

		// Token: 0x170018C5 RID: 6341
		// (get) Token: 0x06006731 RID: 26417 RVA: 0x0008568E File Offset: 0x0008388E
		// (set) Token: 0x06006732 RID: 26418 RVA: 0x00212408 File Offset: 0x00210608
		private AnimancerController.AnimationStance CurrentAnimationStance
		{
			get
			{
				return this.m_animationStance;
			}
			set
			{
				if (this.m_animationStance == value)
				{
					return;
				}
				bool flag = this.m_animationStance == null || !this.m_animationStance.Set.IsCombatStance || value == null || !value.Set.IsCombatStance;
				if (flag && this.m_animationStance != null)
				{
					this.StartAnimationSequence(this.m_animationStance.Set.ExitTransitionSequence, null);
				}
				if (this.m_currentPose != null)
				{
					this.FadeOutLayer(this.m_currentPose.LayerIndex, 0.3f);
				}
				this.m_animationStance = value;
				if (flag && this.m_animationStance != null && this.m_animationStance.Set != null)
				{
					this.StartAnimationSequence(this.m_animationStance.Set.EnterTransitionSequence, null);
				}
				if (this.m_animationStance != null && this.m_animationStance.Set != null && this.m_animationStance.Set.PoseSequence != null)
				{
					this.m_currentPose = this.StartAnimationSequence(this.m_animationStance.Set.PoseSequence, null);
				}
			}
		}

		// Token: 0x06006733 RID: 26419 RVA: 0x00212508 File Offset: 0x00210708
		private void Awake()
		{
			AnimancerPlayable.LayerList.DefaultCapacity = 10;
			if (base.GameEntity != null)
			{
				base.GameEntity.AnimancerController = this;
			}
			if (!GameManager.IsServer)
			{
				this.m_animancer.Layers.SetMinCount(this.m_masks.Length + 1);
				for (int i = 0; i < this.m_masks.Length; i++)
				{
					int num = i + 1;
					if (this.m_masks[i].Mask != null)
					{
						this.m_animancer.Layers.SetMask(num, this.m_masks[i].Mask);
					}
					this.m_animancer.Layers.SetAdditive(num, this.m_masks[i].IsAdditive);
					this.m_maskToIndex.Add(this.m_masks[i].Type, num);
				}
				this.m_onEndClip = new Action(this.OnEndClip);
			}
		}

		// Token: 0x06006734 RID: 26420 RVA: 0x002125F0 File Offset: 0x002107F0
		private void Start()
		{
			if (GameManager.IsServer || base.GameEntity == null)
			{
				base.enabled = false;
				return;
			}
			this.m_replicator = base.GameEntity.AnimatorReplicator;
			if (base.GameEntity.VitalsReplicator != null)
			{
				base.GameEntity.VitalsReplicator.CurrentHealthState.Changed += this.HealthStateOnChanged;
				this.HealthStateOnChanged(base.GameEntity.VitalsReplicator.CurrentHealthState);
				base.GameEntity.VitalsReplicator.BehaviorFlags.Changed += this.BehaviorFlagsOnChanged;
				this.BehaviorFlagsOnChanged(base.GameEntity.VitalsReplicator.BehaviorFlags.Value);
			}
			if (base.GameEntity.Type == GameEntityType.Npc && this.CurrentAnimationStance == null)
			{
				this.SetCurrentStanceId(this.m_npcSet.Id);
			}
		}

		// Token: 0x06006735 RID: 26421 RVA: 0x002126E0 File Offset: 0x002108E0
		private void Update()
		{
			if (this.CurrentAnimationStance == null)
			{
				return;
			}
			Vector2 target = (base.GameEntity.Type == GameEntityType.Npc) ? (this.m_replicator.RawLocomotion * 2f) : this.m_replicator.RawLocomotion;
			this.m_locomotion = Vector2.MoveTowards(this.m_locomotion, target, GlobalSettings.Values.Animation.MovementLerpRate * Time.deltaTime);
			this.m_rotation = Mathf.MoveTowards(this.m_rotation, this.m_replicator.RawRotation, GlobalSettings.Values.Animation.RotationLerpRate * Time.deltaTime);
			this.CurrentAnimationStance.Locomotion.Speed = this.m_replicator.Speed;
			this.CurrentAnimationStance.Locomotion.Parameter = this.m_locomotion;
			this.CurrentAnimationStance.Idle.Parameter = this.m_rotation;
			bool flag = false;
			if (this.m_locomotion != Vector2.zero)
			{
				this.m_animancer.Play(this.CurrentAnimationStance.Locomotion, 0.3f, FadeMode.FixedSpeed);
			}
			else
			{
				this.m_animancer.Play(this.CurrentAnimationStance.Idle, 0.3f, FadeMode.FixedSpeed);
				flag = (this.m_rotation == 0f);
			}
			flag = (flag && this.m_previousStance == this.CurrentAnimationStance);
			this.m_previousStance = this.CurrentAnimationStance;
			if (flag)
			{
				this.CheckIdleTick();
				return;
			}
			this.CancelIdleTick();
		}

		// Token: 0x06006736 RID: 26422 RVA: 0x00212858 File Offset: 0x00210A58
		private void OnDestroy()
		{
			if (base.GameEntity.VitalsReplicator != null)
			{
				base.GameEntity.VitalsReplicator.CurrentHealthState.Changed -= this.HealthStateOnChanged;
				base.GameEntity.VitalsReplicator.BehaviorFlags.Changed -= this.BehaviorFlagsOnChanged;
			}
		}

		// Token: 0x06006737 RID: 26423 RVA: 0x002128BC File Offset: 0x00210ABC
		private void CheckIdleTick()
		{
			this.m_timeIdle += Time.deltaTime;
			if (this.m_timeIdle >= this.m_timeIdleForTick)
			{
				this.m_timeIdle = 0f;
				this.m_timeIdleForTick = GlobalSettings.Values.Animation.IdleTickRate.RandomWithinRange();
				if (this.CurrentAnimationStance == null || this.CurrentAnimationStance.Set == null)
				{
					return;
				}
				AnimationSequence nextIdleTickSequence = this.CurrentAnimationStance.Set.GetNextIdleTickSequence();
				if (nextIdleTickSequence != null)
				{
					this.m_timeIdleForTick += nextIdleTickSequence.GetTotalDuration();
					this.m_currentIdleTick = this.StartAnimationSequence(nextIdleTickSequence, null);
				}
			}
		}

		// Token: 0x06006738 RID: 26424 RVA: 0x00085696 File Offset: 0x00083896
		private void CancelIdleTick()
		{
			if (this.m_currentIdleTick != null)
			{
				this.FadeOutState(this.m_currentIdleTick, 0.1f);
				this.m_currentIdleTick = null;
			}
			this.m_timeIdle = 0f;
		}

		// Token: 0x06006739 RID: 26425 RVA: 0x0021295C File Offset: 0x00210B5C
		public void SetCurrentStanceId(UniqueId id)
		{
			IAnimancerAnimation set;
			if (InternalGameDatabase.Archetypes.TryGetAsType<IAnimancerAnimation>(id, out set))
			{
				AnimancerController.AnimationStance animationStance = null;
				if (!this.m_stanceDict.TryGetValue(id, out animationStance))
				{
					animationStance = new AnimancerController.AnimationStance(this.m_animancer, set);
					this.m_stanceDict.Add(id, animationStance);
					if (this.m_stanceDict.Count == 1)
					{
						this.m_animancer.TryPlay(animationStance.Set, 0.3f, FadeMode.FixedSpeed);
					}
				}
				this.CurrentAnimationStance = animationStance;
			}
		}

		// Token: 0x0600673A RID: 26426 RVA: 0x002129D4 File Offset: 0x00210BD4
		private void OnEndClip()
		{
			AnimancerStateSettings animancerStateSettings;
			if (AnimancerEvent.CurrentState == null || !this.Playing.TryGetValue(AnimancerEvent.CurrentState.Key, out animancerStateSettings))
			{
				return;
			}
			if (animancerStateSettings.Sequence != null)
			{
				int num = animancerStateSettings.ClipIndex + 1;
				if (num >= animancerStateSettings.Sequence.ClipData.Length)
				{
					this.FadeOutLayer(animancerStateSettings.LayerIndex, 0.3f);
				}
				else
				{
					this.PlayAnimationSequence(animancerStateSettings.Sequence, animancerStateSettings.StateTracker, animancerStateSettings.LayerIndex, num);
				}
			}
			this.Playing.Remove(AnimancerEvent.CurrentState.Key);
		}

		// Token: 0x0600673B RID: 26427 RVA: 0x000856C3 File Offset: 0x000838C3
		private void FadeOutLayer(int layerIndex, float fadeDuration)
		{
			this.m_animancer.Layers[layerIndex].StartFade(0f, fadeDuration);
		}

		// Token: 0x0600673C RID: 26428 RVA: 0x000856E1 File Offset: 0x000838E1
		public AnimancerState StartSequence(AnimationSequence sequence, IAnimancerStateTracker stateTracker = null)
		{
			return this.StartAnimationSequence(sequence, stateTracker);
		}

		// Token: 0x0600673D RID: 26429 RVA: 0x00049FFA File Offset: 0x000481FA
		AnimancerState IAnimationController.StartAlchemySequence(IAnimancerStateTracker stateTracker, AlchemyPowerLevel alchemyPowerLevel)
		{
			return null;
		}

		// Token: 0x0600673E RID: 26430 RVA: 0x000856EB File Offset: 0x000838EB
		public void FadeOutState(AnimancerState state)
		{
			this.FadeOutState(state, 0.3f);
		}

		// Token: 0x0600673F RID: 26431 RVA: 0x00212A68 File Offset: 0x00210C68
		public void FadeOutState(AnimancerState state, float fadeDuration)
		{
			AnimancerStateSettings animancerStateSettings;
			if (this.Playing.TryGetValue(state.Key, out animancerStateSettings))
			{
				this.FadeOutLayer(animancerStateSettings.LayerIndex, fadeDuration);
				this.Playing.Remove(state.Key);
			}
		}

		// Token: 0x06006740 RID: 26432 RVA: 0x00212AAC File Offset: 0x00210CAC
		private AnimancerState StartAnimationSequence(AnimationSequence sequence, IAnimancerStateTracker stateTracker = null)
		{
			int layerIndex;
			if (sequence == null || sequence.IsEmpty || !this.m_maskToIndex.TryGetValue(sequence.Mask, out layerIndex))
			{
				return null;
			}
			this.CancelIdleTick();
			return this.PlayAnimationSequence(sequence, stateTracker, layerIndex, 0);
		}

		// Token: 0x06006741 RID: 26433 RVA: 0x00212AEC File Offset: 0x00210CEC
		private unsafe AnimancerState PlayAnimationSequence(AnimationSequence sequence, IAnimancerStateTracker stateTracker, int layerIndex, int clipIndex)
		{
			if (clipIndex >= sequence.ClipData.Length)
			{
				return null;
			}
			AnimationClipData animationClipData = sequence.ClipData[clipIndex];
			AnimancerState animancerState = this.m_animancer.Layers[layerIndex].Play(animationClipData.Clip, animationClipData.FadeDuration, FadeMode.FixedSpeed);
			animancerState.Speed = animationClipData.PlaySpeed;
			if (animancerState.Speed < 0f)
			{
				animancerState.NormalizedTime = 1f;
			}
			bool flag = clipIndex == sequence.ClipData.Length - 1;
			if (!flag || !sequence.LoopFinal)
			{
				*animancerState.Events.OnEnd = this.m_onEndClip;
			}
			AnimancerStateSettings value = new AnimancerStateSettings(animancerState, sequence, stateTracker, layerIndex, clipIndex, flag && sequence.LoopFinal);
			this.Playing.AddOrReplace(animancerState.Key, value);
			return animancerState;
		}

		// Token: 0x06006742 RID: 26434 RVA: 0x00212BB4 File Offset: 0x00210DB4
		private void HealthStateOnChanged(HealthState obj)
		{
			switch (obj)
			{
			case HealthState.Unconscious:
				this.StartSequence(this.m_defaultDeathSequence, null);
				break;
			case HealthState.WakingUp:
				this.StartSequence(this.m_defaultGetUpSequence, null);
				break;
			case HealthState.Dead:
				if (base.GameEntity.Type == GameEntityType.Npc)
				{
					this.StartSequence(this.m_defaultDeathSequence, null);
				}
				break;
			}
			this.m_healthState = obj;
		}

		// Token: 0x06006743 RID: 26435 RVA: 0x00212C1C File Offset: 0x00210E1C
		private void BehaviorFlagsOnChanged(BehaviorEffectTypeFlags obj)
		{
			if (obj.HasBitFlag(BehaviorEffectTypeFlags.Stunned))
			{
				if (this.m_stunnedState == null)
				{
					this.m_stunnedState = this.StartSequence(this.m_defaultStunSequence, null);
					return;
				}
			}
			else if (this.m_stunnedState != null)
			{
				this.FadeOutState(this.m_stunnedState);
				this.m_stunnedState = null;
			}
		}

		// Token: 0x06006744 RID: 26436 RVA: 0x0004475B File Offset: 0x0004295B
		private void ExecuteAbility()
		{
		}

		// Token: 0x06006745 RID: 26437 RVA: 0x00212C6C File Offset: 0x00210E6C
		public bool TriggerHit()
		{
			if (this.m_healthState == HealthState.Alive && this.CurrentAnimationStance != null && this.CurrentAnimationStance.Set != null && this.CurrentAnimationStance.Set.GetHitSequence != null)
			{
				DateTime utcNow = DateTime.UtcNow;
				if ((utcNow - this.m_timeOfLastHit).TotalSeconds > (double)GlobalSettings.Values.Animation.MinTimeBetweenHitAnims)
				{
					this.StartAnimationSequence(this.CurrentAnimationStance.Set.GetHitSequence, null);
					this.m_timeOfLastHit = utcNow;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06006746 RID: 26438 RVA: 0x000856F9 File Offset: 0x000838F9
		public void TriggerJump()
		{
			this.StartSequence(this.m_defaultJumpSequence, null);
		}

		// Token: 0x06006747 RID: 26439 RVA: 0x00085709 File Offset: 0x00083909
		void IAnimationController.SetCurrentStanceId(UniqueId stanceId, bool bypassTransitions)
		{
			this.SetCurrentStanceId(stanceId);
		}

		// Token: 0x170018C6 RID: 6342
		// (get) Token: 0x06006748 RID: 26440 RVA: 0x00085712 File Offset: 0x00083912
		IAnimancerAnimation IAnimationController.CurrentAnimationSet
		{
			get
			{
				return this.CurrentAnimationSet;
			}
		}

		// Token: 0x170018C7 RID: 6343
		// (get) Token: 0x06006749 RID: 26441 RVA: 0x0008571A File Offset: 0x0008391A
		Vector2 IAnimationController.MovementLocomotionVector
		{
			get
			{
				return this.MovementLocomotionVector;
			}
		}

		// Token: 0x0600674A RID: 26442 RVA: 0x00085722 File Offset: 0x00083922
		void IAnimationController.FadeOutState(AnimancerState state)
		{
			this.FadeOutState(state);
		}

		// Token: 0x0600674B RID: 26443 RVA: 0x0008572B File Offset: 0x0008392B
		void IAnimationController.FadeOutState(AnimancerState state, float fadeDuration)
		{
			this.FadeOutState(state, fadeDuration);
		}

		// Token: 0x0600674C RID: 26444 RVA: 0x00085735 File Offset: 0x00083935
		AnimancerState IAnimationController.StartSequence(AnimationSequence sequence, IAnimancerStateTracker stateTracker)
		{
			return this.StartSequence(sequence, stateTracker);
		}

		// Token: 0x0600674D RID: 26445 RVA: 0x0008573F File Offset: 0x0008393F
		bool IAnimationController.TriggerHit()
		{
			return this.TriggerHit();
		}

		// Token: 0x0600674E RID: 26446 RVA: 0x0004475B File Offset: 0x0004295B
		void IAnimationController.TriggerBlock()
		{
		}

		// Token: 0x0600674F RID: 26447 RVA: 0x0004475B File Offset: 0x0004295B
		void IAnimationController.TriggerParry()
		{
		}

		// Token: 0x06006750 RID: 26448 RVA: 0x0004475B File Offset: 0x0004295B
		void IAnimationController.TriggerRiposte()
		{
		}

		// Token: 0x06006751 RID: 26449 RVA: 0x0004475B File Offset: 0x0004295B
		void IAnimationController.TriggerAvoid()
		{
		}

		// Token: 0x06006752 RID: 26450 RVA: 0x00085747 File Offset: 0x00083947
		void IAnimationController.TriggerJump()
		{
			this.TriggerJump();
		}

		// Token: 0x06006753 RID: 26451 RVA: 0x00212CF8 File Offset: 0x00210EF8
		bool IAnimationController.TriggerEvent(AnimationEventTriggerType type)
		{
			bool result = true;
			if (type != AnimationEventTriggerType.Jump)
			{
				if (type == AnimationEventTriggerType.Hit)
				{
					result = this.TriggerHit();
				}
			}
			else
			{
				this.TriggerJump();
			}
			return result;
		}

		// Token: 0x06006754 RID: 26452 RVA: 0x0004475B File Offset: 0x0004295B
		void IAnimationController.AssignSex(CharacterSex sex)
		{
		}

		// Token: 0x06006755 RID: 26453 RVA: 0x0004475B File Offset: 0x0004295B
		void IAnimationController.PlayEmote(Emote emote)
		{
		}

		// Token: 0x170018C8 RID: 6344
		// (get) Token: 0x06006756 RID: 26454 RVA: 0x0008574F File Offset: 0x0008394F
		// (set) Token: 0x06006757 RID: 26455 RVA: 0x00085757 File Offset: 0x00083957
		bool IAnimationController.PreventIdleTicks { get; set; }

		// Token: 0x06006759 RID: 26457 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IAnimationController.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040059A0 RID: 22944
		private Action m_onEndClip;

		// Token: 0x040059A1 RID: 22945
		private Vector2 m_locomotion;

		// Token: 0x040059A2 RID: 22946
		private Vector2 m_movementLocomotion;

		// Token: 0x040059A3 RID: 22947
		private float m_rotation;

		// Token: 0x040059A4 RID: 22948
		private HealthState m_healthState = HealthState.Alive;

		// Token: 0x040059A5 RID: 22949
		private DateTime m_timeOfLastHit = DateTime.MinValue;

		// Token: 0x040059A6 RID: 22950
		private readonly Dictionary<UniqueId, AnimancerController.AnimationStance> m_stanceDict = new Dictionary<UniqueId, AnimancerController.AnimationStance>(default(UniqueIdComparer));

		// Token: 0x040059A7 RID: 22951
		private readonly Dictionary<AnimationClipMask, int> m_maskToIndex = new Dictionary<AnimationClipMask, int>(default(AnimationClipMaskComparer));

		// Token: 0x040059A8 RID: 22952
		[SerializeField]
		private AnimationSequenceWithOverride m_defaultDeathSequence;

		// Token: 0x040059A9 RID: 22953
		[SerializeField]
		private AnimationSequenceWithOverride m_defaultGetUpSequence;

		// Token: 0x040059AA RID: 22954
		[SerializeField]
		private AnimationSequenceWithOverride m_defaultJumpSequence;

		// Token: 0x040059AB RID: 22955
		[SerializeField]
		private AnimationSequenceWithOverride m_defaultStunSequence;

		// Token: 0x040059AC RID: 22956
		private Dictionary<object, AnimancerStateSettings> m_playing;

		// Token: 0x040059AD RID: 22957
		[SerializeField]
		private AnimancerComponent m_animancer;

		// Token: 0x040059AE RID: 22958
		private AnimancerReplicator m_replicator;

		// Token: 0x040059AF RID: 22959
		[SerializeField]
		private AnimancerController.MaskSettings[] m_masks;

		// Token: 0x040059B0 RID: 22960
		[SerializeField]
		private AnimancerAnimationSet m_npcSet;

		// Token: 0x040059B1 RID: 22961
		private AnimancerState m_currentPose;

		// Token: 0x040059B2 RID: 22962
		private AnimancerState m_currentIdleTick;

		// Token: 0x040059B3 RID: 22963
		private float m_timeIdle;

		// Token: 0x040059B4 RID: 22964
		private float m_timeIdleForTick = 30f;

		// Token: 0x040059B5 RID: 22965
		private AnimancerController.AnimationStance m_previousStance;

		// Token: 0x040059B6 RID: 22966
		private AnimancerController.AnimationStance m_animationStance;

		// Token: 0x040059B7 RID: 22967
		private AnimancerState m_stunnedState;

		// Token: 0x02000D61 RID: 3425
		[Serializable]
		private class MaskSettings
		{
			// Token: 0x040059B9 RID: 22969
			public AnimationClipMask Type;

			// Token: 0x040059BA RID: 22970
			public AvatarMask Mask;

			// Token: 0x040059BB RID: 22971
			public bool IsAdditive;
		}

		// Token: 0x02000D62 RID: 3426
		private class AnimationStance
		{
			// Token: 0x0600675B RID: 26459 RVA: 0x00212D88 File Offset: 0x00210F88
			public AnimationStance(AnimancerComponent animancer, IAnimancerAnimation set)
			{
				this.Idle = (animancer.States.GetOrCreate(set.IdleBlend) as LinearMixerState);
				if (this.Idle == null)
				{
					throw new NullReferenceException("Idle did not come back as a LinearMixerState!");
				}
				this.Idle.DontSynchronizeChildren();
				this.Idle.ExtrapolateSpeed = false;
				this.Locomotion = (animancer.States.GetOrCreate(set.LocomotionBlend) as MixerState<Vector2>);
				if (this.Locomotion == null)
				{
					throw new NullReferenceException("Locomotion did not come back as a MixerState<Vector2>!");
				}
				this.Set = set;
			}

			// Token: 0x040059BC RID: 22972
			public readonly LinearMixerState Idle;

			// Token: 0x040059BD RID: 22973
			public readonly MixerState<Vector2> Locomotion;

			// Token: 0x040059BE RID: 22974
			public readonly IAnimancerAnimation Set;
		}
	}
}
