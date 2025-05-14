using System;
using Animancer;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D78 RID: 3448
	public class AnimatorController : GameEntityComponent, IAnimationController
	{
		// Token: 0x060067DC RID: 26588 RVA: 0x00213624 File Offset: 0x00211824
		private void Awake()
		{
			if (base.GameEntity != null)
			{
				base.GameEntity.AnimancerController = this;
			}
			this.m_replicator = base.gameObject.GetComponentInParent<AnimancerReplicator>();
			if (this.m_animator == null)
			{
				this.m_animator = base.gameObject.GetComponent<Animator>();
			}
			this.m_deathLayerIndex = this.m_animator.GetLayerIndex("Death Layer");
			if (AnimatorController.m_locoForwardKey == 0)
			{
				AnimatorController.m_attackIndexKey = Animator.StringToHash("AttackIndex");
				AnimatorController.m_locoForwardKey = Animator.StringToHash("LocomotionForward");
				AnimatorController.m_locoRightKey = Animator.StringToHash("LocomotionRight");
				AnimatorController.m_isDeadKey = Animator.StringToHash("IsDead");
				AnimatorController.m_isHitKey = Animator.StringToHash("Hit");
				AnimatorController.m_attackKey = Animator.StringToHash("Attack");
				AnimatorController.m_speedKey = Animator.StringToHash("Speed");
			}
		}

		// Token: 0x060067DD RID: 26589 RVA: 0x00213704 File Offset: 0x00211904
		private void Start()
		{
			if (base.GameEntity != null && base.GameEntity.VitalsReplicator != null)
			{
				base.GameEntity.VitalsReplicator.CurrentHealthState.Changed += this.HealthStateOnChanged;
				this.HealthStateOnChanged(base.GameEntity.VitalsReplicator.CurrentHealthState);
			}
		}

		// Token: 0x060067DE RID: 26590 RVA: 0x00213770 File Offset: 0x00211970
		private void OnDestroy()
		{
			if (base.GameEntity != null && base.GameEntity.VitalsReplicator != null)
			{
				base.GameEntity.VitalsReplicator.CurrentHealthState.Changed -= this.HealthStateOnChanged;
			}
		}

		// Token: 0x060067DF RID: 26591 RVA: 0x002137C0 File Offset: 0x002119C0
		private void Update()
		{
			if (this.m_replicator == null)
			{
				return;
			}
			this.m_locomotion = Vector2.MoveTowards(this.m_locomotion, this.m_replicator.RawLocomotion, 3f * Time.deltaTime);
			this.m_rotation = Mathf.MoveTowards(this.m_rotation, this.m_replicator.RawRotation, 3f * Time.deltaTime);
			this.m_animator.SetFloat(AnimatorController.m_locoForwardKey, this.m_locomotion.y);
			this.m_animator.SetFloat(AnimatorController.m_locoRightKey, this.m_locomotion.x);
			this.m_animator.SetFloat(AnimatorController.m_speedKey, this.m_replicator.Speed);
		}

		// Token: 0x060067E0 RID: 26592 RVA: 0x00085E16 File Offset: 0x00084016
		private void HealthStateOnChanged(HealthState obj)
		{
			if (obj == HealthState.Dead && base.GameEntity.Type == GameEntityType.Npc)
			{
				this.m_animator.SetLayerWeight(this.m_deathLayerIndex, 1f);
				this.m_animator.SetBool(AnimatorController.m_isDeadKey, true);
			}
		}

		// Token: 0x060067E1 RID: 26593 RVA: 0x0004475B File Offset: 0x0004295B
		void IAnimationController.SetCurrentStanceId(UniqueId stanceId, bool bypassTransitions)
		{
		}

		// Token: 0x170018F0 RID: 6384
		// (get) Token: 0x060067E2 RID: 26594 RVA: 0x00049FFA File Offset: 0x000481FA
		IAnimancerAnimation IAnimationController.CurrentAnimationSet
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170018F1 RID: 6385
		// (get) Token: 0x060067E3 RID: 26595 RVA: 0x00085E51 File Offset: 0x00084051
		Vector2 IAnimationController.MovementLocomotionVector
		{
			get
			{
				return Vector2.zero;
			}
		}

		// Token: 0x060067E4 RID: 26596 RVA: 0x0004475B File Offset: 0x0004295B
		void IAnimationController.FadeOutState(AnimancerState state)
		{
		}

		// Token: 0x060067E5 RID: 26597 RVA: 0x0004475B File Offset: 0x0004295B
		void IAnimationController.FadeOutState(AnimancerState state, float fadeDuration)
		{
		}

		// Token: 0x060067E6 RID: 26598 RVA: 0x0021387C File Offset: 0x00211A7C
		AnimancerState IAnimationController.StartSequence(AnimationSequence sequence, IAnimancerStateTracker stateTracker)
		{
			if (sequence == null)
			{
				this.m_animator.SetInteger(AnimatorController.m_attackIndexKey, 0);
				this.m_animator.SetTrigger(AnimatorController.m_attackKey);
			}
			else
			{
				this.m_animator.SetInteger(AnimatorController.m_attackIndexKey, 1);
				this.m_animator.SetTrigger(AnimatorController.m_attackKey);
			}
			return null;
		}

		// Token: 0x060067E7 RID: 26599 RVA: 0x00049FFA File Offset: 0x000481FA
		AnimancerState IAnimationController.StartAlchemySequence(IAnimancerStateTracker stateTracker, AlchemyPowerLevel alchemyPowerLevel)
		{
			return null;
		}

		// Token: 0x060067E8 RID: 26600 RVA: 0x002138D4 File Offset: 0x00211AD4
		private bool TriggerHit()
		{
			DateTime utcNow = DateTime.UtcNow;
			if ((utcNow - this.m_timeOfLastHit).TotalSeconds > (double)GlobalSettings.Values.Animation.MinTimeBetweenHitAnims)
			{
				this.m_animator.SetTrigger(AnimatorController.m_isHitKey);
				this.m_timeOfLastHit = utcNow;
				return true;
			}
			return false;
		}

		// Token: 0x060067E9 RID: 26601 RVA: 0x00085E58 File Offset: 0x00084058
		bool IAnimationController.TriggerHit()
		{
			return this.TriggerHit();
		}

		// Token: 0x060067EA RID: 26602 RVA: 0x0004475B File Offset: 0x0004295B
		void IAnimationController.TriggerBlock()
		{
		}

		// Token: 0x060067EB RID: 26603 RVA: 0x0004475B File Offset: 0x0004295B
		void IAnimationController.TriggerParry()
		{
		}

		// Token: 0x060067EC RID: 26604 RVA: 0x0004475B File Offset: 0x0004295B
		void IAnimationController.TriggerRiposte()
		{
		}

		// Token: 0x060067ED RID: 26605 RVA: 0x0004475B File Offset: 0x0004295B
		void IAnimationController.TriggerAvoid()
		{
		}

		// Token: 0x060067EE RID: 26606 RVA: 0x0004475B File Offset: 0x0004295B
		void IAnimationController.TriggerJump()
		{
		}

		// Token: 0x060067EF RID: 26607 RVA: 0x00085E60 File Offset: 0x00084060
		bool IAnimationController.TriggerEvent(AnimationEventTriggerType type)
		{
			return type != AnimationEventTriggerType.Hit || this.TriggerHit();
		}

		// Token: 0x060067F0 RID: 26608 RVA: 0x0004475B File Offset: 0x0004295B
		void IAnimationController.AssignSex(CharacterSex sex)
		{
		}

		// Token: 0x060067F1 RID: 26609 RVA: 0x0004475B File Offset: 0x0004295B
		void IAnimationController.PlayEmote(Emote emote)
		{
		}

		// Token: 0x170018F2 RID: 6386
		// (get) Token: 0x060067F2 RID: 26610 RVA: 0x00085E6E File Offset: 0x0008406E
		// (set) Token: 0x060067F3 RID: 26611 RVA: 0x00085E76 File Offset: 0x00084076
		bool IAnimationController.PreventIdleTicks { get; set; }

		// Token: 0x060067F5 RID: 26613 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IAnimationController.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04005A36 RID: 23094
		private const string kLocoForwardKey = "LocomotionForward";

		// Token: 0x04005A37 RID: 23095
		private const string kLocoRightKey = "LocomotionRight";

		// Token: 0x04005A38 RID: 23096
		private const string kHitKey = "Hit";

		// Token: 0x04005A39 RID: 23097
		private const string kIsDeadKey = "IsDead";

		// Token: 0x04005A3A RID: 23098
		private const string kAttackKey = "Attack";

		// Token: 0x04005A3B RID: 23099
		private const string kAttackIndexKey = "AttackIndex";

		// Token: 0x04005A3C RID: 23100
		private const string kSpeedKey = "Speed";

		// Token: 0x04005A3D RID: 23101
		private const float kMovementLerpRate = 3f;

		// Token: 0x04005A3E RID: 23102
		private const float kRotationLerpRate = 3f;

		// Token: 0x04005A3F RID: 23103
		private const string kDeathLayerName = "Death Layer";

		// Token: 0x04005A40 RID: 23104
		[SerializeField]
		private Animator m_animator;

		// Token: 0x04005A41 RID: 23105
		private AnimancerReplicator m_replicator;

		// Token: 0x04005A42 RID: 23106
		private Vector2 m_locomotion;

		// Token: 0x04005A43 RID: 23107
		private float m_rotation;

		// Token: 0x04005A44 RID: 23108
		private int m_deathLayerIndex;

		// Token: 0x04005A45 RID: 23109
		private DateTime m_timeOfLastHit = DateTime.MinValue;

		// Token: 0x04005A46 RID: 23110
		private static int m_attackIndexKey;

		// Token: 0x04005A47 RID: 23111
		private static int m_locoForwardKey;

		// Token: 0x04005A48 RID: 23112
		private static int m_locoRightKey;

		// Token: 0x04005A49 RID: 23113
		private static int m_isDeadKey;

		// Token: 0x04005A4A RID: 23114
		private static int m_isHitKey;

		// Token: 0x04005A4B RID: 23115
		private static int m_attackKey;

		// Token: 0x04005A4C RID: 23116
		private static int m_speedKey;
	}
}
