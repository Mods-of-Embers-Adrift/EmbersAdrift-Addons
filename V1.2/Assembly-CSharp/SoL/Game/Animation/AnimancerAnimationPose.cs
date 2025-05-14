using System;
using System.Collections;
using Animancer;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Archetypes;
using SoL.Networking.Database;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D5A RID: 3418
	[CreateAssetMenu(menuName = "SoL/Animation/Animancer Pose Set")]
	public class AnimancerAnimationPose : BaseArchetype, IAnimancerAnimation
	{
		// Token: 0x1700188D RID: 6285
		// (get) Token: 0x060066D0 RID: 26320 RVA: 0x00212134 File Offset: 0x00210334
		private IAnimancerAnimation AnimancerAnimation
		{
			get
			{
				if (this.m_animancerAnimation == null)
				{
					if (this.m_pair != null)
					{
						this.m_animancerAnimation = this.m_pair;
					}
					else if (this.m_set != null)
					{
						this.m_animancerAnimation = this.m_set;
					}
				}
				return this.m_animancerAnimation;
			}
		}

		// Token: 0x1700188E RID: 6286
		// (get) Token: 0x060066D1 RID: 26321 RVA: 0x000852B4 File Offset: 0x000834B4
		LinearMixerTransition IAnimancerAnimation.IdleBlend
		{
			get
			{
				IAnimancerAnimation animancerAnimation = this.AnimancerAnimation;
				if (animancerAnimation == null)
				{
					return null;
				}
				return animancerAnimation.IdleBlend;
			}
		}

		// Token: 0x060066D2 RID: 26322 RVA: 0x000852C7 File Offset: 0x000834C7
		LinearMixerTransition IAnimancerAnimation.GetIdleBlend(CharacterSex sex)
		{
			IAnimancerAnimation animancerAnimation = this.AnimancerAnimation;
			if (animancerAnimation == null)
			{
				return null;
			}
			return animancerAnimation.GetIdleBlend(sex);
		}

		// Token: 0x1700188F RID: 6287
		// (get) Token: 0x060066D3 RID: 26323 RVA: 0x000852DB File Offset: 0x000834DB
		MixerTransition2D IAnimancerAnimation.LocomotionBlend
		{
			get
			{
				IAnimancerAnimation animancerAnimation = this.AnimancerAnimation;
				if (animancerAnimation == null)
				{
					return null;
				}
				return animancerAnimation.LocomotionBlend;
			}
		}

		// Token: 0x060066D4 RID: 26324 RVA: 0x000852EE File Offset: 0x000834EE
		MixerTransition2D IAnimancerAnimation.GetLocomotionBlend(CharacterSex sex)
		{
			IAnimancerAnimation animancerAnimation = this.AnimancerAnimation;
			if (animancerAnimation == null)
			{
				return null;
			}
			return animancerAnimation.GetLocomotionBlend(sex);
		}

		// Token: 0x060066D5 RID: 26325 RVA: 0x00085302 File Offset: 0x00083502
		bool IAnimancerAnimation.EnableFootIkForLocomotion(CharacterSex sex)
		{
			return this.AnimancerAnimation != null && this.AnimancerAnimation.EnableFootIkForLocomotion(sex);
		}

		// Token: 0x17001890 RID: 6288
		// (get) Token: 0x060066D6 RID: 26326 RVA: 0x0008531A File Offset: 0x0008351A
		bool IAnimancerAnimation.IsCombatStance
		{
			get
			{
				return this.AnimancerAnimation != null && this.AnimancerAnimation.IsCombatStance;
			}
		}

		// Token: 0x17001891 RID: 6289
		// (get) Token: 0x060066D7 RID: 26327 RVA: 0x00085331 File Offset: 0x00083531
		bool IAnimancerAnimation.DisableLateralMovement
		{
			get
			{
				return this.AnimancerAnimation != null && this.AnimancerAnimation.DisableLateralMovement;
			}
		}

		// Token: 0x17001892 RID: 6290
		// (get) Token: 0x060066D8 RID: 26328 RVA: 0x00085348 File Offset: 0x00083548
		AnimationSequence IAnimancerAnimation.EnterTransitionSequence
		{
			get
			{
				return this.m_enterTransitionSequence;
			}
		}

		// Token: 0x17001893 RID: 6291
		// (get) Token: 0x060066D9 RID: 26329 RVA: 0x00085350 File Offset: 0x00083550
		AnimationSequence IAnimancerAnimation.ExitTransitionSequence
		{
			get
			{
				return this.m_exitTransitionSequence;
			}
		}

		// Token: 0x060066DA RID: 26330 RVA: 0x00085358 File Offset: 0x00083558
		AnimationSequence IAnimancerAnimation.GetNextAutoAttackSequence(AnimationFlags animFlags)
		{
			IAnimancerAnimation animancerAnimation = this.AnimancerAnimation;
			if (animancerAnimation == null)
			{
				return null;
			}
			return animancerAnimation.GetNextAutoAttackSequence(animFlags);
		}

		// Token: 0x060066DB RID: 26331 RVA: 0x0008536C File Offset: 0x0008356C
		AnimationSequence IAnimancerAnimation.GetNextIdleTickSequence()
		{
			if (this.m_idleTickShuffler == null)
			{
				this.m_idleTickShuffler = new ArrayShuffler<AnimationSequence>(this.m_idleTickSequences);
			}
			return this.m_idleTickShuffler.GetNext();
		}

		// Token: 0x060066DC RID: 26332 RVA: 0x00085392 File Offset: 0x00083592
		AnimationSequence IAnimancerAnimation.GetIndexedIdleTickSequence(int index)
		{
			if (index < 0 || index >= this.m_idleTickSequences.Length)
			{
				return null;
			}
			return this.m_idleTickSequences[index];
		}

		// Token: 0x17001894 RID: 6292
		// (get) Token: 0x060066DD RID: 26333 RVA: 0x000853AD File Offset: 0x000835AD
		AnimationSequence IAnimancerAnimation.PoseSequence
		{
			get
			{
				return this.m_poseSequence;
			}
		}

		// Token: 0x17001895 RID: 6293
		// (get) Token: 0x060066DE RID: 26334 RVA: 0x00049FFA File Offset: 0x000481FA
		AnimationSequence IAnimancerAnimation.GetHitSequence
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001896 RID: 6294
		// (get) Token: 0x060066DF RID: 26335 RVA: 0x00049FFA File Offset: 0x000481FA
		AnimationSequence IAnimancerAnimation.AvoidSequence
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001897 RID: 6295
		// (get) Token: 0x060066E0 RID: 26336 RVA: 0x00049FFA File Offset: 0x000481FA
		AnimationSequence IAnimancerAnimation.BlockSequence
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001898 RID: 6296
		// (get) Token: 0x060066E1 RID: 26337 RVA: 0x00049FFA File Offset: 0x000481FA
		AnimationSequence IAnimancerAnimation.ParrySequence
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001899 RID: 6297
		// (get) Token: 0x060066E2 RID: 26338 RVA: 0x00049FFA File Offset: 0x000481FA
		AnimationSequence IAnimancerAnimation.RiposteSequence
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060066E3 RID: 26339 RVA: 0x00049FFA File Offset: 0x000481FA
		AnimationSequence IAnimancerAnimation.GetAlchemySequence(AlchemyPowerLevel alchemyPowerLevel)
		{
			return null;
		}

		// Token: 0x060066E4 RID: 26340 RVA: 0x00049FFA File Offset: 0x000481FA
		AbilityAnimation IAnimancerAnimation.GetAbilityAnimation(AnimationExecutionTime exeTime, int index)
		{
			return null;
		}

		// Token: 0x1700189A RID: 6298
		// (get) Token: 0x060066E5 RID: 26341 RVA: 0x000636CE File Offset: 0x000618CE
		private IEnumerable GetSets
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<AnimancerAnimationSet>();
			}
		}

		// Token: 0x1700189B RID: 6299
		// (get) Token: 0x060066E6 RID: 26342 RVA: 0x0006557E File Offset: 0x0006377E
		private IEnumerable GetPairs
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<AnimancerAnimationSetPair>();
			}
		}

		// Token: 0x04005958 RID: 22872
		[SerializeField]
		private AnimancerAnimationSetPair m_pair;

		// Token: 0x04005959 RID: 22873
		[SerializeField]
		private AnimancerAnimationSet m_set;

		// Token: 0x0400595A RID: 22874
		[SerializeField]
		private AnimationSequence m_poseSequence;

		// Token: 0x0400595B RID: 22875
		[SerializeField]
		private AnimationSequence[] m_idleTickSequences;

		// Token: 0x0400595C RID: 22876
		[SerializeField]
		private AnimationSequence m_enterTransitionSequence;

		// Token: 0x0400595D RID: 22877
		[SerializeField]
		private AnimationSequence m_exitTransitionSequence;

		// Token: 0x0400595E RID: 22878
		private ArrayShuffler<AnimationSequence> m_idleTickShuffler;

		// Token: 0x0400595F RID: 22879
		private IAnimancerAnimation m_animancerAnimation;
	}
}
