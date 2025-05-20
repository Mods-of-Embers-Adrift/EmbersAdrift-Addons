using System;
using System.Collections;
using SoL.Networking.Database;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D57 RID: 3415
	public class NonHumanoidAnimancerController : BaseAnimancerController
	{
		// Token: 0x1700188B RID: 6283
		// (get) Token: 0x060066BF RID: 26303 RVA: 0x0004479C File Offset: 0x0004299C
		protected override CharacterSex Sex
		{
			get
			{
				return CharacterSex.Male;
			}
		}

		// Token: 0x1700188C RID: 6284
		// (get) Token: 0x060066C0 RID: 26304 RVA: 0x0008525F File Offset: 0x0008345F
		// (set) Token: 0x060066C1 RID: 26305 RVA: 0x00085267 File Offset: 0x00083467
		private bool IsOverWater
		{
			get
			{
				return this.m_isOverWater;
			}
			set
			{
				if (this.m_isOverWater == value)
				{
					return;
				}
				this.m_isOverWater = value;
				this.RefreshStance();
			}
		}

		// Token: 0x060066C2 RID: 26306 RVA: 0x00211C74 File Offset: 0x0020FE74
		protected override void Start()
		{
			base.Start();
			if (this.m_base == null)
			{
				base.enabled = false;
				throw new ArgumentException("NULL BASE STANCE ON " + base.gameObject.name);
			}
			this.m_baseStance = new BaseAnimancerController.AnimationStance(this.m_animancer, this.m_base, CharacterSex.None);
			this.m_combatStance = ((this.m_combat == null || this.m_combat == this.m_base) ? this.m_baseStance : new BaseAnimancerController.AnimationStance(this.m_animancer, this.m_combat, CharacterSex.None));
			if (this.m_swim != null)
			{
				if (base.GameEntity)
				{
					this.m_groundSampler = base.GameEntity.GroundSampler;
				}
				if (this.m_groundSampler)
				{
					this.m_swimStance = new BaseAnimancerController.AnimationStance(this.m_animancer, this.m_swim, CharacterSex.None);
				}
			}
			GameEntity obj = null;
			if (base.GameEntity != null && base.GameEntity.TargetController != null)
			{
				base.GameEntity.TargetController.OffensiveTargetChanged += this.TargetControllerOnOffensiveTargetChanged;
				obj = base.GameEntity.TargetController.OffensiveTarget;
			}
			this.TargetControllerOnOffensiveTargetChanged(obj);
			if (!this.m_openingSequence.IsEmpty)
			{
				base.StartAnimationSequence(this.m_openingSequence, null);
				return;
			}
			base.SetInitialStanceId();
		}

		// Token: 0x060066C3 RID: 26307 RVA: 0x00211DDC File Offset: 0x0020FFDC
		protected override void OnDestroy()
		{
			if (base.GameEntity != null && base.GameEntity.TargetController != null)
			{
				base.GameEntity.TargetController.OffensiveTargetChanged -= this.TargetControllerOnOffensiveTargetChanged;
			}
			if (this.m_swim != null && this.m_swimStance != null)
			{
				base.CancelInvoke("CheckForWater");
			}
			base.OnDestroy();
		}

		// Token: 0x060066C4 RID: 26308 RVA: 0x00211E50 File Offset: 0x00210050
		protected override void UpdateInternal()
		{
			base.UpdateInternal();
			if (base.GameEntity && base.GameEntity.CharacterData)
			{
				this.IsOverWater = base.GameEntity.CharacterData.IsSwimming;
				if (base.GameEntity.IKController)
				{
					base.GameEntity.IKController.IsOverWater = this.IsOverWater;
				}
			}
		}

		// Token: 0x060066C5 RID: 26309 RVA: 0x00085280 File Offset: 0x00083480
		protected override float GetTargetRotation()
		{
			return Mathf.Clamp(base.GetTargetRotation(), -10f, 10f);
		}

		// Token: 0x060066C6 RID: 26310 RVA: 0x00085297 File Offset: 0x00083497
		private void TargetControllerOnOffensiveTargetChanged(GameEntity obj)
		{
			this.m_combatTarget = obj;
			this.RefreshStance();
		}

		// Token: 0x060066C7 RID: 26311 RVA: 0x00211EC0 File Offset: 0x002100C0
		private void RefreshStance()
		{
			BaseAnimancerController.AnimationStance animationStance = this.m_combatTarget ? this.m_combatStance : this.m_baseStance;
			if (this.IsOverWater && this.m_swimStance != null)
			{
				animationStance = this.m_swimStance;
			}
			else if (animationStance == null)
			{
				animationStance = this.m_baseStance;
			}
			base.CurrentAnimationStance = animationStance;
		}

		// Token: 0x060066C8 RID: 26312 RVA: 0x000852A6 File Offset: 0x000834A6
		protected override void StanceChanged()
		{
			base.StanceChanged();
			this.RefreshStanceToggles();
		}

		// Token: 0x060066C9 RID: 26313 RVA: 0x00211F14 File Offset: 0x00210114
		private void ToggleLocomotionIdles(bool isPlaying)
		{
			if (base.CurrentAnimationStance != null)
			{
				if (base.CurrentAnimationStance.Locomotion != null)
				{
					base.CurrentAnimationStance.Locomotion.IsPlaying = isPlaying;
				}
				if (base.CurrentAnimationStance.Idle != null)
				{
					base.CurrentAnimationStance.Idle.IsPlaying = isPlaying;
				}
			}
		}

		// Token: 0x060066CA RID: 26314 RVA: 0x00211F68 File Offset: 0x00210168
		private void RefreshStanceToggles()
		{
			if (this.m_toggles == null)
			{
				return;
			}
			for (int i = 0; i < this.m_toggles.Length; i++)
			{
				if (this.m_toggles[i] != null && this.m_toggles[i].Obj)
				{
					AnimancerAnimationSet animancerAnimationSet = null;
					switch (this.m_toggles[i].SetType)
					{
					case NonHumanoidAnimancerController.SetType.Base:
						animancerAnimationSet = this.m_base;
						break;
					case NonHumanoidAnimancerController.SetType.Combat:
						animancerAnimationSet = this.m_combat;
						break;
					case NonHumanoidAnimancerController.SetType.Swim:
						animancerAnimationSet = this.m_swim;
						break;
					}
					bool active = !this.m_isDead && animancerAnimationSet && base.CurrentAnimationStance != null && base.CurrentAnimationStance.Set != null && base.CurrentAnimationStance.Set.Equals(animancerAnimationSet);
					this.m_toggles[i].Obj.SetActive(active);
				}
			}
		}

		// Token: 0x060066CB RID: 26315 RVA: 0x00212048 File Offset: 0x00210248
		protected override void CurrentHealthStateOnChanged(HealthState obj)
		{
			if (obj == HealthState.Unconscious || obj == HealthState.Dead)
			{
				this.m_isDead = true;
				base.StartAnimationSequence(this.m_defaultDeathSequence, this.m_deathStateTracker);
				this.RefreshStanceToggles();
				this.ToggleLocomotionIdles(false);
			}
			else
			{
				this.ToggleLocomotionIdles(true);
			}
			base.CurrentHealthStateOnChanged(obj);
		}

		// Token: 0x060066CC RID: 26316 RVA: 0x000636CE File Offset: 0x000618CE
		private IEnumerable GetAnimancerSets()
		{
			return SolOdinUtilities.GetDropdownItems<AnimancerAnimationSet>();
		}

		// Token: 0x060066CD RID: 26317 RVA: 0x00212094 File Offset: 0x00210294
		private void CheckForWater()
		{
			if (this.m_groundSampler)
			{
				this.m_groundSampler.TimeLimitedSampleGround();
				this.IsOverWater = (this.m_groundSampler.LastHit.collider != null && this.m_groundSampler.LastHit.collider.CompareTag("Water"));
				if (base.GameEntity && base.GameEntity.IKController != null)
				{
					base.GameEntity.IKController.IsOverWater = this.IsOverWater;
				}
			}
		}

		// Token: 0x04005946 RID: 22854
		[SerializeField]
		private AnimancerAnimationSet m_base;

		// Token: 0x04005947 RID: 22855
		[SerializeField]
		private AnimancerAnimationSet m_combat;

		// Token: 0x04005948 RID: 22856
		[SerializeField]
		private AnimancerAnimationSet m_swim;

		// Token: 0x04005949 RID: 22857
		[SerializeField]
		private NonHumanoidAnimancerController.AnimationSetToggle[] m_toggles;

		// Token: 0x0400594A RID: 22858
		[SerializeField]
		private AnimationSequenceWithOverride m_openingSequence;

		// Token: 0x0400594B RID: 22859
		private BaseAnimancerController.AnimationStance m_baseStance;

		// Token: 0x0400594C RID: 22860
		private BaseAnimancerController.AnimationStance m_combatStance;

		// Token: 0x0400594D RID: 22861
		private BaseAnimancerController.AnimationStance m_swimStance;

		// Token: 0x0400594E RID: 22862
		private GameEntity m_combatTarget;

		// Token: 0x0400594F RID: 22863
		private GroundSampler m_groundSampler;

		// Token: 0x04005950 RID: 22864
		private bool m_isOverWater;

		// Token: 0x04005951 RID: 22865
		private bool m_isDead;

		// Token: 0x02000D58 RID: 3416
		private enum SetType
		{
			// Token: 0x04005953 RID: 22867
			Base,
			// Token: 0x04005954 RID: 22868
			Combat,
			// Token: 0x04005955 RID: 22869
			Swim
		}

		// Token: 0x02000D59 RID: 3417
		[Serializable]
		private class AnimationSetToggle
		{
			// Token: 0x04005956 RID: 22870
			public GameObject Obj;

			// Token: 0x04005957 RID: 22871
			public NonHumanoidAnimancerController.SetType SetType;
		}
	}
}
