using System;
using SoL.Game.Settings;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D54 RID: 3412
	public class HumanoidAnimancerController : BaseAnimancerController
	{
		// Token: 0x17001889 RID: 6281
		// (get) Token: 0x060066B2 RID: 26290 RVA: 0x000851FD File Offset: 0x000833FD
		protected override CharacterSex Sex
		{
			get
			{
				return this.m_sex;
			}
		}

		// Token: 0x060066B3 RID: 26291 RVA: 0x00211940 File Offset: 0x0020FB40
		protected override void Start()
		{
			base.Start();
			this.m_defaultGetUpSequence.SetMaskAndLoop(AnimationClipMask.Death, false);
			if (this.m_initializeIdleSetOnStart && base.GameEntity && base.GameEntity.Type == GameEntityType.Npc && base.CurrentAnimationStance == null)
			{
				base.SetCurrentStanceId(GlobalSettings.Values.Animation.IdleSetPair.Id);
				return;
			}
			base.SetInitialStanceId();
		}

		// Token: 0x060066B4 RID: 26292 RVA: 0x002119AC File Offset: 0x0020FBAC
		protected override void AssignSex(CharacterSex sex)
		{
			if (this.m_sex.IsUnassigned())
			{
				this.m_sex = sex;
				base.SetCurrentStanceId(this.SexUnassignedQueuedStanceId.IsEmpty ? GlobalSettings.Values.Animation.IdleSetPair.Id : this.SexUnassignedQueuedStanceId);
				this.SexUnassignedQueuedStanceId = UniqueId.Empty;
			}
		}

		// Token: 0x060066B5 RID: 26293 RVA: 0x00085205 File Offset: 0x00083405
		protected override void InitializeStateTrackers()
		{
			base.InitializeStateTrackers();
			this.m_getUpStateTracker = AnimancerStateTracker.GetFromPool();
		}

		// Token: 0x060066B6 RID: 26294 RVA: 0x00085218 File Offset: 0x00083418
		protected override void ReturnStateTrackers()
		{
			base.ReturnStateTrackers();
			this.m_getUpStateTracker.ReturnToPool();
			this.m_getUpStateTracker = null;
		}

		// Token: 0x060066B7 RID: 26295 RVA: 0x00211A08 File Offset: 0x0020FC08
		protected override void CurrentHealthStateOnChanged(HealthState obj)
		{
			switch (obj)
			{
			case HealthState.Unconscious:
				base.StartAnimationSequence(this.m_defaultDeathSequence, this.m_deathStateTracker);
				break;
			case HealthState.WakingUp:
				base.StartAnimationSequence(this.m_defaultGetUpSequence, this.m_getUpStateTracker);
				break;
			case HealthState.Dead:
				if (base.GameEntity.Type == GameEntityType.Npc)
				{
					base.StartAnimationSequence(this.m_defaultDeathSequence, this.m_deathStateTracker);
				}
				break;
			default:
				if (this.m_healthState == HealthState.WakingUp && this.m_getUpStateTracker.AnimancerState != null)
				{
					base.FadeOutState(this.m_getUpStateTracker.AnimancerState);
				}
				break;
			}
			base.CurrentHealthStateOnChanged(obj);
		}

		// Token: 0x060066B8 RID: 26296 RVA: 0x00085232 File Offset: 0x00083432
		protected override void TriggerJump()
		{
			base.TriggerJump();
			base.StartAnimationSequence(this.m_defaultJumpSequence, null);
		}

		// Token: 0x060066B9 RID: 26297 RVA: 0x00211AA8 File Offset: 0x0020FCA8
		private void SetToNpcStance()
		{
			if (GlobalSettings.Values && GlobalSettings.Values.Animation != null && GlobalSettings.Values.Animation.IdleSetPair)
			{
				base.SetCurrentStanceId(GlobalSettings.Values.Animation.IdleSetPair.Id);
			}
		}

		// Token: 0x0400593D RID: 22845
		[SerializeField]
		private bool m_initializeIdleSetOnStart;

		// Token: 0x0400593E RID: 22846
		[SerializeField]
		private AnimationSequenceWithOverride m_defaultGetUpSequence;

		// Token: 0x0400593F RID: 22847
		[SerializeField]
		private AnimationSequenceWithOverride m_defaultJumpSequence;

		// Token: 0x04005940 RID: 22848
		private CharacterSex m_sex;

		// Token: 0x04005941 RID: 22849
		private AnimancerStateTracker m_getUpStateTracker;
	}
}
