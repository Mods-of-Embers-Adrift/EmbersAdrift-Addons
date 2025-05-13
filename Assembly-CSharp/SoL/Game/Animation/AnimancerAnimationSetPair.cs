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
	// Token: 0x02000D5F RID: 3423
	[CreateAssetMenu(menuName = "SoL/Animation/Animancer Locomotion Set Pair")]
	public class AnimancerAnimationSetPair : BaseArchetype, IAnimancerAnimation
	{
		// Token: 0x170018B5 RID: 6325
		// (get) Token: 0x06006718 RID: 26392 RVA: 0x0008550E File Offset: 0x0008370E
		LinearMixerTransition IAnimancerAnimation.IdleBlend
		{
			get
			{
				return this.m_primary.IdleBlend;
			}
		}

		// Token: 0x06006719 RID: 26393 RVA: 0x0008551B File Offset: 0x0008371B
		LinearMixerTransition IAnimancerAnimation.GetIdleBlend(CharacterSex sex)
		{
			if (sex != CharacterSex.Female)
			{
				return this.m_primary.IdleBlend;
			}
			return this.m_secondary.IdleBlend;
		}

		// Token: 0x170018B6 RID: 6326
		// (get) Token: 0x0600671A RID: 26394 RVA: 0x00085538 File Offset: 0x00083738
		MixerTransition2D IAnimancerAnimation.LocomotionBlend
		{
			get
			{
				return this.m_primary.LocomotionBlend;
			}
		}

		// Token: 0x0600671B RID: 26395 RVA: 0x00085545 File Offset: 0x00083745
		MixerTransition2D IAnimancerAnimation.GetLocomotionBlend(CharacterSex sex)
		{
			if (sex != CharacterSex.Female)
			{
				return this.m_primary.LocomotionBlend;
			}
			return this.m_secondary.LocomotionBlend;
		}

		// Token: 0x0600671C RID: 26396 RVA: 0x00085562 File Offset: 0x00083762
		bool IAnimancerAnimation.EnableFootIkForLocomotion(CharacterSex sex)
		{
			if (sex != CharacterSex.Female)
			{
				return this.m_primary.EnableIkForLocomotion;
			}
			return this.m_secondary.EnableIkForLocomotion;
		}

		// Token: 0x170018B7 RID: 6327
		// (get) Token: 0x0600671D RID: 26397 RVA: 0x0008557F File Offset: 0x0008377F
		bool IAnimancerAnimation.IsCombatStance
		{
			get
			{
				return this.m_primary.IsCombatStance;
			}
		}

		// Token: 0x170018B8 RID: 6328
		// (get) Token: 0x0600671E RID: 26398 RVA: 0x0008558C File Offset: 0x0008378C
		bool IAnimancerAnimation.DisableLateralMovement
		{
			get
			{
				return this.m_primary.DisableLateralMovement;
			}
		}

		// Token: 0x170018B9 RID: 6329
		// (get) Token: 0x0600671F RID: 26399 RVA: 0x00085599 File Offset: 0x00083799
		AnimationSequence IAnimancerAnimation.EnterTransitionSequence
		{
			get
			{
				return this.m_primary.EnterTransitionSequence;
			}
		}

		// Token: 0x170018BA RID: 6330
		// (get) Token: 0x06006720 RID: 26400 RVA: 0x000855A6 File Offset: 0x000837A6
		AnimationSequence IAnimancerAnimation.ExitTransitionSequence
		{
			get
			{
				return this.m_primary.ExitTransitionSequence;
			}
		}

		// Token: 0x06006721 RID: 26401 RVA: 0x000855B3 File Offset: 0x000837B3
		AnimationSequence IAnimancerAnimation.GetNextAutoAttackSequence(AnimationFlags animFlags)
		{
			return this.m_primary.GetNextAutoAttackSequence(animFlags);
		}

		// Token: 0x06006722 RID: 26402 RVA: 0x000855C1 File Offset: 0x000837C1
		AnimationSequence IAnimancerAnimation.GetNextIdleTickSequence()
		{
			return this.m_primary.GetNextIdleTickSequence();
		}

		// Token: 0x06006723 RID: 26403 RVA: 0x000855CE File Offset: 0x000837CE
		AnimationSequence IAnimancerAnimation.GetIndexedIdleTickSequence(int index)
		{
			return this.m_primary.GetIndexedIdleTickSequence(index);
		}

		// Token: 0x170018BB RID: 6331
		// (get) Token: 0x06006724 RID: 26404 RVA: 0x000855DC File Offset: 0x000837DC
		AnimationSequence IAnimancerAnimation.PoseSequence
		{
			get
			{
				return this.m_primary.PoseSequence;
			}
		}

		// Token: 0x170018BC RID: 6332
		// (get) Token: 0x06006725 RID: 26405 RVA: 0x000855E9 File Offset: 0x000837E9
		AnimationSequence IAnimancerAnimation.GetHitSequence
		{
			get
			{
				return this.m_primary.GetHitSequence;
			}
		}

		// Token: 0x170018BD RID: 6333
		// (get) Token: 0x06006726 RID: 26406 RVA: 0x000855F6 File Offset: 0x000837F6
		AnimationSequence IAnimancerAnimation.AvoidSequence
		{
			get
			{
				return this.m_primary.AvoidSequence;
			}
		}

		// Token: 0x170018BE RID: 6334
		// (get) Token: 0x06006727 RID: 26407 RVA: 0x00085603 File Offset: 0x00083803
		AnimationSequence IAnimancerAnimation.BlockSequence
		{
			get
			{
				return this.m_primary.BlockSequence;
			}
		}

		// Token: 0x170018BF RID: 6335
		// (get) Token: 0x06006728 RID: 26408 RVA: 0x00085610 File Offset: 0x00083810
		AnimationSequence IAnimancerAnimation.ParrySequence
		{
			get
			{
				return this.m_primary.ParrySequence;
			}
		}

		// Token: 0x170018C0 RID: 6336
		// (get) Token: 0x06006729 RID: 26409 RVA: 0x0008561D File Offset: 0x0008381D
		AnimationSequence IAnimancerAnimation.RiposteSequence
		{
			get
			{
				return this.m_primary.RiposteSequence;
			}
		}

		// Token: 0x0600672A RID: 26410 RVA: 0x0008562A File Offset: 0x0008382A
		AnimationSequence IAnimancerAnimation.GetAlchemySequence(AlchemyPowerLevel alchemyPowerLevel)
		{
			return this.m_primary.GetAlchemySequence(alchemyPowerLevel);
		}

		// Token: 0x0600672B RID: 26411 RVA: 0x00085638 File Offset: 0x00083838
		AbilityAnimation IAnimancerAnimation.GetAbilityAnimation(AnimationExecutionTime exeTime, int index)
		{
			return this.m_primary.GetAbilityAnimation(exeTime, index);
		}

		// Token: 0x170018C1 RID: 6337
		// (get) Token: 0x0600672C RID: 26412 RVA: 0x000636CE File Offset: 0x000618CE
		private IEnumerable GetSets
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<AnimancerAnimationSet>();
			}
		}

		// Token: 0x0400599E RID: 22942
		[SerializeField]
		private AnimancerAnimationSet m_primary;

		// Token: 0x0400599F RID: 22943
		[SerializeField]
		private AnimancerAnimationSet m_secondary;
	}
}
