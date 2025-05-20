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
	// Token: 0x02000D50 RID: 3408
	public abstract class BaseAnimancerController : GameEntityComponent, IAnimationController
	{
		// Token: 0x17001882 RID: 6274
		// (get) Token: 0x06006672 RID: 26226 RVA: 0x00084F47 File Offset: 0x00083147
		// (set) Token: 0x06006673 RID: 26227 RVA: 0x00084F4F File Offset: 0x0008314F
		public bool PreventIdleTicks
		{
			get
			{
				return this.m_preventIdleTicks;
			}
			set
			{
				this.m_preventIdleTicks = value;
				this.m_timeIdle = 0f;
				if (this.m_preventIdleTicks)
				{
					this.CancelIdleTick();
				}
			}
		}

		// Token: 0x17001883 RID: 6275
		// (get) Token: 0x06006674 RID: 26228 RVA: 0x00084F71 File Offset: 0x00083171
		// (set) Token: 0x06006675 RID: 26229 RVA: 0x00210894 File Offset: 0x0020EA94
		protected BaseAnimancerController.AnimationStance CurrentAnimationStance
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
				if (flag && this.m_bypassTransitions)
				{
					flag = false;
				}
				if (flag && this.m_animationStance != null && this.m_animationStance.Set != null)
				{
					this.PlayTransition(this.m_animationStance.Set.ExitTransitionSequence);
				}
				if (this.m_currentPose != null)
				{
					this.FadeOutLayer(this.m_currentPose.LayerIndex, 0.3f);
				}
				this.m_animationStance = value;
				if (flag && this.m_animationStance != null && this.m_animationStance.Set != null)
				{
					this.PlayTransition(this.m_animationStance.Set.EnterTransitionSequence);
				}
				if (this.m_animationStance != null && this.m_animationStance.Set != null && this.m_animationStance.Set.PoseSequence != null)
				{
					this.m_currentPose = this.StartAnimationSequence(this.m_animationStance.Set.PoseSequence, null);
				}
				this.StanceChanged();
			}
		}

		// Token: 0x06006676 RID: 26230 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void StanceChanged()
		{
		}

		// Token: 0x06006677 RID: 26231 RVA: 0x002109B4 File Offset: 0x0020EBB4
		private void PlayTransition(AnimationSequence sequence)
		{
			AnimancerState animancerState = this.StartAnimationSequence(sequence, null);
			if (animancerState != null)
			{
				this.m_currentTransition = new BaseAnimancerController.TransitionSequence?(new BaseAnimancerController.TransitionSequence
				{
					State = animancerState,
					CancelOnMovement = sequence.CancelOnMovement,
					CancelOnMovementFadeDuration = sequence.CancelOnMovementFadeDuration
				});
			}
		}

		// Token: 0x17001884 RID: 6276
		// (get) Token: 0x06006678 RID: 26232 RVA: 0x00084F79 File Offset: 0x00083179
		// (set) Token: 0x06006679 RID: 26233 RVA: 0x00084F81 File Offset: 0x00083181
		public int? IdleTickLoopIndex
		{
			get
			{
				return this.m_idleTickLoopIndex;
			}
			set
			{
				this.m_idleTickLoopIndex = value;
				this.SetNextTimeIdleForTick();
			}
		}

		// Token: 0x17001885 RID: 6277
		// (get) Token: 0x0600667A RID: 26234 RVA: 0x00084F90 File Offset: 0x00083190
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

		// Token: 0x17001886 RID: 6278
		// (get) Token: 0x0600667B RID: 26235
		protected abstract CharacterSex Sex { get; }

		// Token: 0x0600667C RID: 26236 RVA: 0x00210A04 File Offset: 0x0020EC04
		private void Awake()
		{
			if (GameManager.IsOnline && (GameManager.IsServer || base.GameEntity == null))
			{
				base.enabled = false;
				return;
			}
			if (base.GameEntity != null)
			{
				base.GameEntity.AnimancerController = this;
			}
			this.m_random = new System.Random(base.gameObject.GetHashCode());
			this.m_timeIdleForTick = GlobalSettings.Values.Animation.IdleTickRate.RandomWithinRange(this.m_random) + this.m_extraTimeBetweenIdleTicks;
			this.InitializeStateTrackers();
			AnimancerPlayable.LayerList.DefaultCapacity = 10;
			this.AddMask(AnimationClipMask.None, null, false);
			this.AddMask(AnimationClipMask.GetHit, null, false);
			this.AddMask(AnimationClipMask.AutoAttack, null, false);
			this.AddMask(AnimationClipMask.AbilityAttack, null, false);
			for (int i = 0; i < this.m_masks.Length; i++)
			{
				if (!this.m_masks[i].Type.AddedByDefault())
				{
					this.AddMask(this.m_masks[i].Type, this.m_masks[i].Mask, this.m_masks[i].IsAdditive);
				}
			}
			this.AddMask(AnimationClipMask.FullBodyOverride, null, false);
			this.AddMask(AnimationClipMask.Death, null, false);
			this.m_onEndClip = new Action(this.OnEndClip);
		}

		// Token: 0x0600667D RID: 26237 RVA: 0x00210B38 File Offset: 0x0020ED38
		protected virtual void Start()
		{
			this.m_defaultDeathSequence.SetMaskAndLoop(AnimationClipMask.Death, true);
			this.m_defaultStunSequence.SetMaskAndLoop(AnimationClipMask.FullBodyOverride, true);
			if (base.GameEntity != null)
			{
				this.m_replicator = base.GameEntity.AnimatorReplicator;
				if (base.GameEntity.VitalsReplicator != null)
				{
					base.GameEntity.VitalsReplicator.CurrentHealthState.Changed += this.CurrentHealthStateOnChanged;
					this.CurrentHealthStateOnChanged(base.GameEntity.VitalsReplicator.CurrentHealthState);
					base.GameEntity.VitalsReplicator.BehaviorFlags.Changed += this.BehaviorFlagsOnChanged;
					this.BehaviorFlagsOnChanged(base.GameEntity.VitalsReplicator.BehaviorFlags);
				}
			}
		}

		// Token: 0x0600667E RID: 26238 RVA: 0x00210C10 File Offset: 0x0020EE10
		protected virtual void OnDestroy()
		{
			if (base.GameEntity != null && base.GameEntity.VitalsReplicator != null)
			{
				base.GameEntity.VitalsReplicator.CurrentHealthState.Changed -= this.CurrentHealthStateOnChanged;
				base.GameEntity.VitalsReplicator.BehaviorFlags.Changed -= this.BehaviorFlagsOnChanged;
			}
			this.ReturnStateTrackers();
		}

		// Token: 0x0600667F RID: 26239 RVA: 0x00210C88 File Offset: 0x0020EE88
		protected virtual void Update()
		{
			if (this.Sex.IsUnassigned() || this.CurrentAnimationStance == null || this.m_healthState != HealthState.Alive)
			{
				return;
			}
			if (this.m_replicator != null)
			{
				Vector2 vector = this.m_replicator.RawLocomotion;
				if (base.GameEntity && base.GameEntity.CharacterData && base.GameEntity.CharacterData.TransformScale != null)
				{
					float d = 1.PercentModification(base.GameEntity.CharacterData.TransformScale.Value);
					vector /= d;
				}
				if (this.CurrentAnimationStance.Set.DisableLateralMovement)
				{
					vector.x = 0f;
				}
				this.m_locomotion = Vector2.MoveTowards(this.m_locomotion, vector, GlobalSettings.Values.Animation.MovementLerpRate * Time.deltaTime);
				float num = this.GetTargetRotation();
				float num2 = GlobalSettings.Values.Animation.RotationLerpRate;
				if (num >= 1f && num <= 1f)
				{
					num = 0f;
					num2 *= GlobalSettings.Values.Animation.RotationLerpRateToZeroMultiplier;
				}
				this.m_rotation = Mathf.MoveTowards(this.m_rotation, num, num2 * Time.deltaTime);
				this.CurrentAnimationStance.Locomotion.Speed = this.m_replicator.Speed;
				this.CurrentAnimationStance.Locomotion.Parameter = this.m_locomotion;
				this.CurrentAnimationStance.Idle.Parameter = this.m_rotation;
			}
			bool flag = false;
			if (this.m_locomotion != Vector2.zero)
			{
				if (this.m_currentTransition != null && this.m_currentTransition.Value.CancelOnMovement && this.m_currentTransition.Value.State != null)
				{
					this.FadeOutState(this.m_currentTransition.Value.State, this.m_currentTransition.Value.CancelOnMovementFadeDuration);
					this.m_currentTransition = null;
				}
				this.m_animancer.Play(this.CurrentAnimationStance.Locomotion, 0.3f, FadeMode.FixedSpeed);
				this.CurrentAnimationStance.ApplyLocomotionFootIK();
			}
			else
			{
				AnimancerState animancerState = this.m_animancer.Play(this.CurrentAnimationStance.Idle, 0.3f, FadeMode.FixedSpeed);
				flag = (Mathf.Abs(this.m_rotation) < 0.01f);
				if (this.m_randomizeIdleTime && this.m_previousStance != this.CurrentAnimationStance && animancerState != null)
				{
					animancerState.NormalizedTime = (float)this.m_random.NextDouble();
				}
			}
			flag = (flag && this.m_previousStance == this.CurrentAnimationStance);
			this.m_previousStance = this.CurrentAnimationStance;
			if (flag)
			{
				this.CheckIdleTick();
			}
			else
			{
				this.CancelIdleTick();
				this.CancelEmote();
			}
			this.UpdateInternal();
		}

		// Token: 0x06006680 RID: 26240 RVA: 0x00210F54 File Offset: 0x0020F154
		protected void SetInitialStanceId()
		{
			if (base.GameEntity && base.GameEntity.CharacterData && !base.GameEntity.CharacterData.CurrentStanceData.Value.StanceId.IsEmpty && (this.CurrentAnimationStance == null || this.CurrentAnimationStance.Set == null || this.CurrentAnimationStance.Set.Id != base.GameEntity.CharacterData.CurrentStanceData.Value.StanceId))
			{
				this.SetCurrentStanceId(base.GameEntity.CharacterData.CurrentStanceData.Value.StanceId, true);
			}
		}

		// Token: 0x06006681 RID: 26241 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void UpdateInternal()
		{
		}

		// Token: 0x06006682 RID: 26242 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void AssignSex(CharacterSex sex)
		{
		}

		// Token: 0x06006683 RID: 26243 RVA: 0x00084FAB File Offset: 0x000831AB
		protected virtual void InitializeStateTrackers()
		{
			this.m_emoteStateTracker = AnimancerStateTracker.GetFromPool();
			this.m_idleTickStateTracker = AnimancerStateTracker.GetFromPool();
			this.m_stunnedStateTracker = AnimancerStateTracker.GetFromPool();
			this.m_deathStateTracker = AnimancerStateTracker.GetFromPool();
		}

		// Token: 0x06006684 RID: 26244 RVA: 0x00211014 File Offset: 0x0020F214
		protected virtual void ReturnStateTrackers()
		{
			this.m_emoteStateTracker.ReturnToPool();
			this.m_idleTickStateTracker.ReturnToPool();
			this.m_stunnedStateTracker.ReturnToPool();
			this.m_deathStateTracker.ReturnToPool();
			this.m_emoteStateTracker = null;
			this.m_idleTickStateTracker = null;
			this.m_stunnedStateTracker = null;
			this.m_deathStateTracker = null;
		}

		// Token: 0x06006685 RID: 26245 RVA: 0x0021106C File Offset: 0x0020F26C
		private void AddMask(AnimationClipMask maskType, AvatarMask mask, bool isAdditive)
		{
			if (mask != null)
			{
				this.m_animancer.Layers.SetMask(this.m_index, mask);
			}
			this.m_animancer.Layers.SetAdditive(this.m_index, isAdditive);
			this.m_maskToIndex.Add(maskType, this.m_index);
			this.m_index++;
		}

		// Token: 0x06006686 RID: 26246 RVA: 0x00084FD9 File Offset: 0x000831D9
		protected virtual float GetTargetRotation()
		{
			if (this.m_replicator == null)
			{
				return 0f;
			}
			return this.m_replicator.RawRotation;
		}

		// Token: 0x06006687 RID: 26247 RVA: 0x002110D0 File Offset: 0x0020F2D0
		protected AnimancerState StartAnimationSequence(AnimationSequence sequence, IAnimancerStateTracker stateTracker = null)
		{
			int layerIndex;
			if (sequence == null || sequence.IsEmpty || !this.m_maskToIndex.TryGetValue(sequence.Mask, out layerIndex))
			{
				return null;
			}
			this.CancelIdleTick();
			return this.PlayAnimationSequence(sequence, stateTracker, layerIndex, 0);
		}

		// Token: 0x06006688 RID: 26248 RVA: 0x00211110 File Offset: 0x0020F310
		private unsafe AnimancerState PlayAnimationSequence(AnimationSequence sequence, IAnimancerStateTracker stateTracker, int layerIndex, int clipIndex)
		{
			if (sequence == null || sequence.ClipData == null || !this.m_animancer || !this.m_animancer.gameObject)
			{
				return null;
			}
			if (clipIndex >= sequence.ClipData.Length)
			{
				return null;
			}
			AnimationClipData animationClipData = sequence.ClipData[clipIndex];
			int num = HashCode.Combine<AnimationClip, int>(animationClipData.Clip, layerIndex);
			AnimancerState orCreate = this.m_animancer.States.GetOrCreate(num.ToString(), animationClipData.Clip, false);
			AnimancerState animancerState = this.m_animancer.Layers[layerIndex].Play(orCreate, animationClipData.FadeDuration, FadeMode.FixedSpeed);
			animancerState.ApplyFootIK = sequence.FootIk;
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

		// Token: 0x06006689 RID: 26249 RVA: 0x00084FF4 File Offset: 0x000831F4
		private void FadeOutLayer(int layerIndex, float fadeDuration)
		{
			this.m_animancer.Layers[layerIndex].StartFade(0f, fadeDuration);
		}

		// Token: 0x0600668A RID: 26250 RVA: 0x00085012 File Offset: 0x00083212
		public void FadeOutState(AnimancerState state)
		{
			this.FadeOutState(state, 0.3f);
		}

		// Token: 0x0600668B RID: 26251 RVA: 0x00211238 File Offset: 0x0020F438
		public void FadeOutState(AnimancerState state, float fadeDuration)
		{
			AnimancerStateSettings animancerStateSettings;
			if (this.Playing.TryGetValue(state.Key, out animancerStateSettings))
			{
				this.FadeOutLayer(animancerStateSettings.LayerIndex, fadeDuration);
				this.Playing.Remove(state.Key);
			}
		}

		// Token: 0x0600668C RID: 26252 RVA: 0x0021127C File Offset: 0x0020F47C
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
			if (this.m_currentTransition != null && this.m_currentTransition.Value.State != null && this.m_currentTransition.Value.State.Key == AnimancerEvent.CurrentState.Key)
			{
				this.m_currentTransition = null;
			}
		}

		// Token: 0x0600668D RID: 26253 RVA: 0x0021135C File Offset: 0x0020F55C
		private void SetNextTimeIdleForTick()
		{
			this.m_timeIdle = 0f;
			this.m_timeIdleForTick = ((this.IdleTickLoopIndex != null) ? 5f : (GlobalSettings.Values.Animation.IdleTickRate.RandomWithinRange(this.m_random) + this.m_extraTimeBetweenIdleTicks));
		}

		// Token: 0x0600668E RID: 26254 RVA: 0x00085020 File Offset: 0x00083220
		private void ForceIdleTick()
		{
			this.m_timeIdle = this.m_timeIdleForTick;
			this.CheckIdleTick();
		}

		// Token: 0x0600668F RID: 26255 RVA: 0x002113B4 File Offset: 0x0020F5B4
		private void CheckIdleTick()
		{
			if (this.m_emoteStateTracker.AnimancerState != null && this.m_emoteStateTracker.AnimancerState.IsPlaying)
			{
				return;
			}
			this.m_timeIdle += Time.deltaTime;
			if (!this.PreventIdleTicks && this.m_timeIdle >= this.m_timeIdleForTick)
			{
				this.SetNextTimeIdleForTick();
				if (this.CurrentAnimationStance == null || this.CurrentAnimationStance.Set == null)
				{
					return;
				}
				AnimationSequence animationSequence = (this.IdleTickLoopIndex != null) ? this.CurrentAnimationStance.Set.GetIndexedIdleTickSequence(this.IdleTickLoopIndex.Value) : this.CurrentAnimationStance.Set.GetNextIdleTickSequence();
				if (animationSequence != null)
				{
					this.m_timeIdleForTick += animationSequence.GetTotalDuration();
					this.StartAnimationSequence(animationSequence, this.m_idleTickStateTracker);
				}
			}
		}

		// Token: 0x06006690 RID: 26256 RVA: 0x00211490 File Offset: 0x0020F690
		private void CancelIdleTick()
		{
			if (this.m_idleTickStateTracker != null && this.m_idleTickStateTracker.AnimancerState != null)
			{
				this.FadeOutState(this.m_idleTickStateTracker.AnimancerState, 0.1f);
				this.m_idleTickStateTracker.AnimancerState = null;
			}
			this.m_timeIdle = 0f;
		}

		// Token: 0x06006691 RID: 26257 RVA: 0x002114E0 File Offset: 0x0020F6E0
		private void CancelEmote()
		{
			if (this.m_emoteStateTracker != null && this.m_emoteStateTracker.AnimancerState != null)
			{
				this.FadeOutState(this.m_emoteStateTracker.AnimancerState, 0.1f);
				this.m_emoteStateTracker.AnimancerState = null;
				if (this.m_emoteStateTracker.DeferredHandIk && base.GameEntity && base.GameEntity.HandheldMountController)
				{
					base.GameEntity.HandheldMountController.CancelDeferredHandIk();
				}
				this.m_emoteStateTracker.DeferredHandIk = false;
			}
		}

		// Token: 0x06006692 RID: 26258 RVA: 0x00085034 File Offset: 0x00083234
		protected virtual void CurrentHealthStateOnChanged(HealthState obj)
		{
			this.m_healthState = obj;
			if (obj == HealthState.Dead && this.CurrentAnimationStance != null)
			{
				this.CurrentAnimationStance.Locomotion.Parameter = Vector2.zero;
				this.CurrentAnimationStance.Idle.Parameter = 0f;
			}
		}

		// Token: 0x06006693 RID: 26259 RVA: 0x0021156C File Offset: 0x0020F76C
		private void BehaviorFlagsOnChanged(BehaviorEffectTypeFlags obj)
		{
			if (obj.HasBitFlag(BehaviorEffectTypeFlags.Stunned))
			{
				if (this.m_stunnedStateTracker.AnimancerState == null)
				{
					this.StartAnimationSequence(this.m_defaultStunSequence, this.m_stunnedStateTracker);
					return;
				}
			}
			else if (this.m_stunnedStateTracker.AnimancerState != null)
			{
				this.FadeOutState(this.m_stunnedStateTracker.AnimancerState);
				this.m_stunnedStateTracker.AnimancerState = null;
			}
		}

		// Token: 0x06006694 RID: 26260 RVA: 0x00085073 File Offset: 0x00083273
		private bool CanTrigger()
		{
			return this.m_healthState == HealthState.Alive && this.CurrentAnimationStance != null && this.CurrentAnimationStance.Set != null;
		}

		// Token: 0x06006695 RID: 26261 RVA: 0x002115D0 File Offset: 0x0020F7D0
		private bool TriggerHit()
		{
			if (this.CanTrigger() && Time.time - this.m_timeOfLastHit > GlobalSettings.Values.Animation.MinTimeBetweenHitAnims)
			{
				this.StartAnimationSequence(this.CurrentAnimationStance.Set.GetHitSequence, null);
				this.m_timeOfLastHit = Time.time;
				return true;
			}
			return false;
		}

		// Token: 0x06006696 RID: 26262 RVA: 0x00085096 File Offset: 0x00083296
		private void TriggerBlock()
		{
			if (this.CanTrigger())
			{
				this.StartAnimationSequence(this.CurrentAnimationStance.Set.BlockSequence, null);
			}
		}

		// Token: 0x06006697 RID: 26263 RVA: 0x000850B8 File Offset: 0x000832B8
		private void TriggerParry()
		{
			if (this.CanTrigger())
			{
				this.StartAnimationSequence(this.CurrentAnimationStance.Set.ParrySequence, null);
			}
		}

		// Token: 0x06006698 RID: 26264 RVA: 0x000850DA File Offset: 0x000832DA
		private void TriggerRiposte()
		{
			if (this.CanTrigger())
			{
				this.StartAnimationSequence(this.CurrentAnimationStance.Set.RiposteSequence, null);
			}
		}

		// Token: 0x06006699 RID: 26265 RVA: 0x000850FC File Offset: 0x000832FC
		private void TriggerAvoid()
		{
			if (this.CanTrigger())
			{
				this.StartAnimationSequence(this.CurrentAnimationStance.Set.AvoidSequence, null);
			}
		}

		// Token: 0x0600669A RID: 26266 RVA: 0x0008511E File Offset: 0x0008331E
		private void TriggerDeath()
		{
			if (this.CanTrigger())
			{
				this.StartAnimationSequence(this.m_defaultDeathSequence, null);
			}
		}

		// Token: 0x0600669B RID: 26267 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void TriggerJump()
		{
		}

		// Token: 0x0600669C RID: 26268 RVA: 0x00085136 File Offset: 0x00083336
		private void SetCurrentStanceId(UniqueId stanceId, bool bypassTransitions)
		{
			this.m_bypassTransitions = bypassTransitions;
			this.SetCurrentStanceId(stanceId);
			this.m_bypassTransitions = false;
		}

		// Token: 0x0600669D RID: 26269 RVA: 0x00211628 File Offset: 0x0020F828
		protected void SetCurrentStanceId(UniqueId id)
		{
			if (this.Sex.IsUnassigned())
			{
				this.SexUnassignedQueuedStanceId = id;
				return;
			}
			IAnimancerAnimation set;
			if (!InternalGameDatabase.Archetypes.TryGetAsType<IAnimancerAnimation>(id, out set))
			{
				return;
			}
			BaseAnimancerController.AnimationStance animationStance = null;
			if (!this.m_stanceDict.TryGetValue(id, out animationStance))
			{
				animationStance = new BaseAnimancerController.AnimationStance(this.m_animancer, set, this.Sex);
				this.m_stanceDict.Add(id, animationStance);
				if (this.m_stanceDict.Count == 1)
				{
					this.m_animancer.TryPlay(animationStance.Set, 0.3f, FadeMode.FixedSpeed);
				}
			}
			this.CurrentAnimationStance = animationStance;
		}

		// Token: 0x0600669E RID: 26270 RVA: 0x0008514D File Offset: 0x0008334D
		void IAnimationController.SetCurrentStanceId(UniqueId stanceId, bool bypassTransitions)
		{
			this.SetCurrentStanceId(stanceId, bypassTransitions);
		}

		// Token: 0x17001887 RID: 6279
		// (get) Token: 0x0600669F RID: 26271 RVA: 0x00085157 File Offset: 0x00083357
		IAnimancerAnimation IAnimationController.CurrentAnimationSet
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

		// Token: 0x17001888 RID: 6280
		// (get) Token: 0x060066A0 RID: 26272 RVA: 0x0008517B File Offset: 0x0008337B
		Vector2 IAnimationController.MovementLocomotionVector
		{
			get
			{
				return this.m_locomotion;
			}
		}

		// Token: 0x060066A1 RID: 26273 RVA: 0x00085183 File Offset: 0x00083383
		void IAnimationController.FadeOutState(AnimancerState state, float fadeDuration)
		{
			this.FadeOutState(state, fadeDuration);
		}

		// Token: 0x060066A2 RID: 26274 RVA: 0x0008518D File Offset: 0x0008338D
		AnimancerState IAnimationController.StartSequence(AnimationSequence sequence, IAnimancerStateTracker stateTracker)
		{
			return this.StartAnimationSequence(sequence, stateTracker);
		}

		// Token: 0x060066A3 RID: 26275 RVA: 0x002116BC File Offset: 0x0020F8BC
		AnimancerState IAnimationController.StartAlchemySequence(IAnimancerStateTracker stateTracker, AlchemyPowerLevel alchemyPowerLevel)
		{
			AnimancerState result = null;
			if (alchemyPowerLevel != AlchemyPowerLevel.None && this.CurrentAnimationStance != null && this.CurrentAnimationStance.Set != null)
			{
				AnimationSequence alchemySequence = this.CurrentAnimationStance.Set.GetAlchemySequence(alchemyPowerLevel);
				if (alchemySequence != null && !alchemySequence.IsEmpty)
				{
					result = this.StartAnimationSequence(alchemySequence, stateTracker);
				}
			}
			return result;
		}

		// Token: 0x060066A4 RID: 26276 RVA: 0x00085197 File Offset: 0x00083397
		bool IAnimationController.TriggerHit()
		{
			return this.TriggerHit();
		}

		// Token: 0x060066A5 RID: 26277 RVA: 0x0008519F File Offset: 0x0008339F
		void IAnimationController.TriggerBlock()
		{
			this.TriggerBlock();
		}

		// Token: 0x060066A6 RID: 26278 RVA: 0x000851A7 File Offset: 0x000833A7
		void IAnimationController.TriggerParry()
		{
			this.TriggerParry();
		}

		// Token: 0x060066A7 RID: 26279 RVA: 0x000851AF File Offset: 0x000833AF
		void IAnimationController.TriggerRiposte()
		{
			this.TriggerRiposte();
		}

		// Token: 0x060066A8 RID: 26280 RVA: 0x000851B7 File Offset: 0x000833B7
		void IAnimationController.TriggerAvoid()
		{
			this.TriggerAvoid();
		}

		// Token: 0x060066A9 RID: 26281 RVA: 0x000851BF File Offset: 0x000833BF
		void IAnimationController.TriggerJump()
		{
			this.TriggerJump();
		}

		// Token: 0x060066AA RID: 26282 RVA: 0x0021170C File Offset: 0x0020F90C
		bool IAnimationController.TriggerEvent(AnimationEventTriggerType type)
		{
			bool result = true;
			switch (type)
			{
			case AnimationEventTriggerType.Jump:
				this.TriggerJump();
				break;
			case AnimationEventTriggerType.Avoid:
				this.TriggerAvoid();
				break;
			case AnimationEventTriggerType.Riposte:
				this.TriggerRiposte();
				break;
			case AnimationEventTriggerType.Parry:
				this.TriggerParry();
				break;
			case AnimationEventTriggerType.Block:
				this.TriggerBlock();
				break;
			case AnimationEventTriggerType.Hit:
				result = this.TriggerHit();
				break;
			}
			return result;
		}

		// Token: 0x060066AB RID: 26283 RVA: 0x000851C7 File Offset: 0x000833C7
		void IAnimationController.AssignSex(CharacterSex sex)
		{
			this.AssignSex(sex);
		}

		// Token: 0x060066AC RID: 26284 RVA: 0x00211770 File Offset: 0x0020F970
		void IAnimationController.PlayEmote(Emote emote)
		{
			if (emote == null)
			{
				return;
			}
			if (this.m_emoteStateTracker.AnimancerState != null)
			{
				this.FadeOutState(this.m_emoteStateTracker.AnimancerState, 0f);
				this.m_emoteStateTracker.AnimancerState = null;
			}
			AnimationSequenceWithOverride animationSequence = emote.GetAnimationSequence(base.GameEntity.CharacterData.Sex);
			AnimancerState animancerState = this.StartAnimationSequence(animationSequence, this.m_emoteStateTracker);
			if (emote.DeferHandIk && emote.DeferHandIkDuration > 0f && animancerState != null && base.GameEntity && base.GameEntity.HandheldMountController)
			{
				base.GameEntity.HandheldMountController.DeferHandIk(emote.DeferHandIkDuration);
				this.m_emoteStateTracker.DeferredHandIk = true;
			}
		}

		// Token: 0x060066AE RID: 26286 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IAnimationController.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04005911 RID: 22801
		private bool m_preventIdleTicks;

		// Token: 0x04005912 RID: 22802
		private bool m_bypassTransitions;

		// Token: 0x04005913 RID: 22803
		private BaseAnimancerController.TransitionSequence? m_currentTransition;

		// Token: 0x04005914 RID: 22804
		private AnimancerState m_currentPose;

		// Token: 0x04005915 RID: 22805
		private BaseAnimancerController.AnimationStance m_previousStance;

		// Token: 0x04005916 RID: 22806
		private BaseAnimancerController.AnimationStance m_animationStance;

		// Token: 0x04005917 RID: 22807
		private const float kIdleTickLoopTime = 5f;

		// Token: 0x04005918 RID: 22808
		private int? m_idleTickLoopIndex;

		// Token: 0x04005919 RID: 22809
		private Vector2 m_locomotion;

		// Token: 0x0400591A RID: 22810
		private float m_rotation;

		// Token: 0x0400591B RID: 22811
		private Action m_onEndClip;

		// Token: 0x0400591C RID: 22812
		private IAnimancerReplicator m_replicator;

		// Token: 0x0400591D RID: 22813
		protected HealthState m_healthState = HealthState.Alive;

		// Token: 0x0400591E RID: 22814
		private float m_timeOfLastHit;

		// Token: 0x0400591F RID: 22815
		private System.Random m_random;

		// Token: 0x04005920 RID: 22816
		private float m_timeIdle;

		// Token: 0x04005921 RID: 22817
		private float m_timeIdleForTick = 30f;

		// Token: 0x04005922 RID: 22818
		private AnimancerStateTracker m_emoteStateTracker;

		// Token: 0x04005923 RID: 22819
		private AnimancerStateTracker m_idleTickStateTracker;

		// Token: 0x04005924 RID: 22820
		private AnimancerStateTracker m_stunnedStateTracker;

		// Token: 0x04005925 RID: 22821
		protected AnimancerStateTracker m_deathStateTracker;

		// Token: 0x04005926 RID: 22822
		private readonly Dictionary<UniqueId, BaseAnimancerController.AnimationStance> m_stanceDict = new Dictionary<UniqueId, BaseAnimancerController.AnimationStance>(default(UniqueIdComparer));

		// Token: 0x04005927 RID: 22823
		private readonly Dictionary<AnimationClipMask, int> m_maskToIndex = new Dictionary<AnimationClipMask, int>(default(AnimationClipMaskComparer));

		// Token: 0x04005928 RID: 22824
		private Dictionary<object, AnimancerStateSettings> m_playing;

		// Token: 0x04005929 RID: 22825
		protected UniqueId SexUnassignedQueuedStanceId = UniqueId.Empty;

		// Token: 0x0400592A RID: 22826
		[SerializeField]
		private bool m_randomizeIdleTime;

		// Token: 0x0400592B RID: 22827
		[SerializeField]
		private float m_extraTimeBetweenIdleTicks;

		// Token: 0x0400592C RID: 22828
		[SerializeField]
		protected AnimancerComponent m_animancer;

		// Token: 0x0400592D RID: 22829
		[SerializeField]
		private BaseAnimancerController.MaskSettings[] m_masks;

		// Token: 0x0400592E RID: 22830
		[SerializeField]
		protected AnimationSequenceWithOverride m_defaultDeathSequence;

		// Token: 0x0400592F RID: 22831
		[SerializeField]
		private AnimationSequenceWithOverride m_defaultStunSequence;

		// Token: 0x04005930 RID: 22832
		private int m_index = 1;

		// Token: 0x04005931 RID: 22833
		private const string kTriggerGroup = "Triggers";

		// Token: 0x02000D51 RID: 3409
		[Serializable]
		private class MaskSettings
		{
			// Token: 0x04005932 RID: 22834
			public AnimationClipMask Type;

			// Token: 0x04005933 RID: 22835
			public AvatarMask Mask;

			// Token: 0x04005934 RID: 22836
			public bool IsAdditive;
		}

		// Token: 0x02000D52 RID: 3410
		protected class AnimationStance
		{
			// Token: 0x060066B0 RID: 26288 RVA: 0x002118A0 File Offset: 0x0020FAA0
			public AnimationStance(AnimancerComponent animancer, IAnimancerAnimation set, CharacterSex sex = CharacterSex.None)
			{
				this.Idle = (animancer.States.GetOrCreate(set.GetIdleBlend(sex)) as LinearMixerState);
				if (this.Idle == null)
				{
					throw new NullReferenceException("Idle did not come back as a LinearMixerState!");
				}
				this.Idle.DontSynchronizeChildren();
				this.Idle.ExtrapolateSpeed = false;
				this.Locomotion = (animancer.States.GetOrCreate(set.GetLocomotionBlend(sex)) as MixerState<Vector2>);
				if (this.Locomotion == null)
				{
					throw new NullReferenceException("Locomotion did not come back as a MixerState<Vector2>!");
				}
				this.m_enableFootIkForLocomotion = set.EnableFootIkForLocomotion(sex);
				this.Set = set;
			}

			// Token: 0x060066B1 RID: 26289 RVA: 0x000851D0 File Offset: 0x000833D0
			public void ApplyLocomotionFootIK()
			{
				if (this.m_enableFootIkForLocomotion && !this.m_appliedFootIk && this.Locomotion != null)
				{
					this.Locomotion.ApplyFootIK = true;
					this.m_appliedFootIk = true;
				}
			}

			// Token: 0x04005935 RID: 22837
			public readonly LinearMixerState Idle;

			// Token: 0x04005936 RID: 22838
			public readonly MixerState<Vector2> Locomotion;

			// Token: 0x04005937 RID: 22839
			public readonly IAnimancerAnimation Set;

			// Token: 0x04005938 RID: 22840
			private readonly bool m_enableFootIkForLocomotion;

			// Token: 0x04005939 RID: 22841
			private bool m_appliedFootIk;
		}

		// Token: 0x02000D53 RID: 3411
		private struct TransitionSequence
		{
			// Token: 0x0400593A RID: 22842
			public AnimancerState State;

			// Token: 0x0400593B RID: 22843
			public bool CancelOnMovement;

			// Token: 0x0400593C RID: 22844
			public float CancelOnMovementFadeDuration;
		}
	}
}
