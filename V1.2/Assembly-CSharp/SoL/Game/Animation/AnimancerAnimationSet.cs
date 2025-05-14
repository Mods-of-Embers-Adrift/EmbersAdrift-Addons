using System;
using System.Collections.Generic;
using Animancer;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Archetypes;
using SoL.Networking.Database;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.Game.Animation
{
	// Token: 0x02000D5D RID: 3421
	[CreateAssetMenu(menuName = "SoL/Animation/Animancer Locomotion Set")]
	public class AnimancerAnimationSet : BaseArchetype, IAnimancerAnimation
	{
		// Token: 0x1700189C RID: 6300
		// (get) Token: 0x060066EA RID: 26346 RVA: 0x00049FFA File Offset: 0x000481FA
		internal AnimationSequence PoseSequence
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700189D RID: 6301
		// (get) Token: 0x060066EB RID: 26347 RVA: 0x000853B5 File Offset: 0x000835B5
		internal LinearMixerTransition IdleBlend
		{
			get
			{
				return this.m_idle;
			}
		}

		// Token: 0x1700189E RID: 6302
		// (get) Token: 0x060066EC RID: 26348 RVA: 0x000853BD File Offset: 0x000835BD
		internal MixerTransition2D LocomotionBlend
		{
			get
			{
				return this.m_locomotion;
			}
		}

		// Token: 0x1700189F RID: 6303
		// (get) Token: 0x060066ED RID: 26349 RVA: 0x000853C5 File Offset: 0x000835C5
		internal bool IsCombatStance
		{
			get
			{
				return this.m_isCombatStance;
			}
		}

		// Token: 0x170018A0 RID: 6304
		// (get) Token: 0x060066EE RID: 26350 RVA: 0x000853CD File Offset: 0x000835CD
		internal bool DisableLateralMovement
		{
			get
			{
				return this.m_disableLateralMovement;
			}
		}

		// Token: 0x170018A1 RID: 6305
		// (get) Token: 0x060066EF RID: 26351 RVA: 0x000853D5 File Offset: 0x000835D5
		internal bool EnableIkForLocomotion
		{
			get
			{
				return this.m_enableFootIkForLocomotion;
			}
		}

		// Token: 0x170018A2 RID: 6306
		// (get) Token: 0x060066F0 RID: 26352 RVA: 0x000853DD File Offset: 0x000835DD
		internal AnimationSequence EnterTransitionSequence
		{
			get
			{
				return this.m_enterTransitionSequence;
			}
		}

		// Token: 0x170018A3 RID: 6307
		// (get) Token: 0x060066F1 RID: 26353 RVA: 0x000853E5 File Offset: 0x000835E5
		internal AnimationSequence ExitTransitionSequence
		{
			get
			{
				return this.m_exitTransitionSequence;
			}
		}

		// Token: 0x170018A4 RID: 6308
		// (get) Token: 0x060066F2 RID: 26354 RVA: 0x000853ED File Offset: 0x000835ED
		internal AnimationSequence GetHitSequence
		{
			get
			{
				return this.m_getHitSequence;
			}
		}

		// Token: 0x170018A5 RID: 6309
		// (get) Token: 0x060066F3 RID: 26355 RVA: 0x000853F5 File Offset: 0x000835F5
		internal AnimationSequence AvoidSequence
		{
			get
			{
				return this.m_avoidSequence;
			}
		}

		// Token: 0x170018A6 RID: 6310
		// (get) Token: 0x060066F4 RID: 26356 RVA: 0x000853FD File Offset: 0x000835FD
		internal AnimationSequence BlockSequence
		{
			get
			{
				return this.m_blockSequence;
			}
		}

		// Token: 0x170018A7 RID: 6311
		// (get) Token: 0x060066F5 RID: 26357 RVA: 0x00085405 File Offset: 0x00083605
		internal AnimationSequence ParrySequence
		{
			get
			{
				return this.m_parrySequence;
			}
		}

		// Token: 0x170018A8 RID: 6312
		// (get) Token: 0x060066F6 RID: 26358 RVA: 0x0008540D File Offset: 0x0008360D
		internal AnimationSequence RiposteSequence
		{
			get
			{
				return this.m_riposteSequence;
			}
		}

		// Token: 0x060066F7 RID: 26359 RVA: 0x00085415 File Offset: 0x00083615
		internal AnimationSequence GetAlchemySequence(AlchemyPowerLevel alchemyPowerLevel)
		{
			if (alchemyPowerLevel == AlchemyPowerLevel.I)
			{
				return this.m_alchemyISequence;
			}
			if (alchemyPowerLevel != AlchemyPowerLevel.II)
			{
				return null;
			}
			return this.m_alchemyIISequence;
		}

		// Token: 0x060066F8 RID: 26360 RVA: 0x000853B5 File Offset: 0x000835B5
		internal virtual LinearMixerTransition GetIdleBlend(CharacterSex sex)
		{
			return this.m_idle;
		}

		// Token: 0x060066F9 RID: 26361 RVA: 0x000853BD File Offset: 0x000835BD
		internal virtual MixerTransition2D GetLocomotionBlend(CharacterSex sex)
		{
			return this.m_locomotion;
		}

		// Token: 0x170018A9 RID: 6313
		// (get) Token: 0x060066FA RID: 26362 RVA: 0x000853B5 File Offset: 0x000835B5
		LinearMixerTransition IAnimancerAnimation.IdleBlend
		{
			get
			{
				return this.m_idle;
			}
		}

		// Token: 0x060066FB RID: 26363 RVA: 0x000853B5 File Offset: 0x000835B5
		LinearMixerTransition IAnimancerAnimation.GetIdleBlend(CharacterSex sex)
		{
			return this.m_idle;
		}

		// Token: 0x170018AA RID: 6314
		// (get) Token: 0x060066FC RID: 26364 RVA: 0x000853BD File Offset: 0x000835BD
		MixerTransition2D IAnimancerAnimation.LocomotionBlend
		{
			get
			{
				return this.m_locomotion;
			}
		}

		// Token: 0x060066FD RID: 26365 RVA: 0x000853BD File Offset: 0x000835BD
		MixerTransition2D IAnimancerAnimation.GetLocomotionBlend(CharacterSex sex)
		{
			return this.m_locomotion;
		}

		// Token: 0x060066FE RID: 26366 RVA: 0x000853D5 File Offset: 0x000835D5
		bool IAnimancerAnimation.EnableFootIkForLocomotion(CharacterSex sex)
		{
			return this.m_enableFootIkForLocomotion;
		}

		// Token: 0x170018AB RID: 6315
		// (get) Token: 0x060066FF RID: 26367 RVA: 0x000853C5 File Offset: 0x000835C5
		bool IAnimancerAnimation.IsCombatStance
		{
			get
			{
				return this.m_isCombatStance;
			}
		}

		// Token: 0x170018AC RID: 6316
		// (get) Token: 0x06006700 RID: 26368 RVA: 0x000853CD File Offset: 0x000835CD
		bool IAnimancerAnimation.DisableLateralMovement
		{
			get
			{
				return this.m_disableLateralMovement;
			}
		}

		// Token: 0x170018AD RID: 6317
		// (get) Token: 0x06006701 RID: 26369 RVA: 0x000853DD File Offset: 0x000835DD
		AnimationSequence IAnimancerAnimation.EnterTransitionSequence
		{
			get
			{
				return this.m_enterTransitionSequence;
			}
		}

		// Token: 0x170018AE RID: 6318
		// (get) Token: 0x06006702 RID: 26370 RVA: 0x000853E5 File Offset: 0x000835E5
		AnimationSequence IAnimancerAnimation.ExitTransitionSequence
		{
			get
			{
				return this.m_exitTransitionSequence;
			}
		}

		// Token: 0x06006703 RID: 26371 RVA: 0x00085430 File Offset: 0x00083630
		AnimationSequence IAnimancerAnimation.GetNextAutoAttackSequence(AnimationFlags animFlags)
		{
			return this.GetNextAutoAttackSequence(animFlags);
		}

		// Token: 0x06006704 RID: 26372 RVA: 0x00085439 File Offset: 0x00083639
		AnimationSequence IAnimancerAnimation.GetNextIdleTickSequence()
		{
			return this.GetNextIdleTickSequence();
		}

		// Token: 0x06006705 RID: 26373 RVA: 0x00085441 File Offset: 0x00083641
		AnimationSequence IAnimancerAnimation.GetIndexedIdleTickSequence(int index)
		{
			return this.GetIndexedIdleTickSequence(index);
		}

		// Token: 0x170018AF RID: 6319
		// (get) Token: 0x06006706 RID: 26374 RVA: 0x00049FFA File Offset: 0x000481FA
		AnimationSequence IAnimancerAnimation.PoseSequence
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170018B0 RID: 6320
		// (get) Token: 0x06006707 RID: 26375 RVA: 0x000853ED File Offset: 0x000835ED
		AnimationSequence IAnimancerAnimation.GetHitSequence
		{
			get
			{
				return this.m_getHitSequence;
			}
		}

		// Token: 0x170018B1 RID: 6321
		// (get) Token: 0x06006708 RID: 26376 RVA: 0x000853F5 File Offset: 0x000835F5
		AnimationSequence IAnimancerAnimation.AvoidSequence
		{
			get
			{
				return this.m_avoidSequence;
			}
		}

		// Token: 0x170018B2 RID: 6322
		// (get) Token: 0x06006709 RID: 26377 RVA: 0x000853FD File Offset: 0x000835FD
		AnimationSequence IAnimancerAnimation.BlockSequence
		{
			get
			{
				return this.m_blockSequence;
			}
		}

		// Token: 0x170018B3 RID: 6323
		// (get) Token: 0x0600670A RID: 26378 RVA: 0x00085405 File Offset: 0x00083605
		AnimationSequence IAnimancerAnimation.ParrySequence
		{
			get
			{
				return this.m_parrySequence;
			}
		}

		// Token: 0x170018B4 RID: 6324
		// (get) Token: 0x0600670B RID: 26379 RVA: 0x0008540D File Offset: 0x0008360D
		AnimationSequence IAnimancerAnimation.RiposteSequence
		{
			get
			{
				return this.m_riposteSequence;
			}
		}

		// Token: 0x0600670C RID: 26380 RVA: 0x0008544A File Offset: 0x0008364A
		AnimationSequence IAnimancerAnimation.GetAlchemySequence(AlchemyPowerLevel alchemyPowerLevel)
		{
			return this.GetAlchemySequence(alchemyPowerLevel);
		}

		// Token: 0x0600670D RID: 26381 RVA: 0x00085453 File Offset: 0x00083653
		AbilityAnimation IAnimancerAnimation.GetAbilityAnimation(AnimationExecutionTime exeTime, int index)
		{
			return this.GetAbilityAnimation(exeTime, index);
		}

		// Token: 0x0600670E RID: 26382 RVA: 0x0008545D File Offset: 0x0008365D
		internal AnimationSequence GetNextIdleTickSequence()
		{
			if (this.m_idleTickShuffler == null)
			{
				this.m_idleTickShuffler = new ArrayShuffler<AnimationSequence>(this.m_idleTickSequences);
			}
			return this.m_idleTickShuffler.GetNext();
		}

		// Token: 0x0600670F RID: 26383 RVA: 0x00085483 File Offset: 0x00083683
		internal AnimationSequence GetIndexedIdleTickSequence(int index)
		{
			if (index < 0 || index >= this.m_idleTickSequences.Length)
			{
				return null;
			}
			return this.m_idleTickSequences[index];
		}

		// Token: 0x06006710 RID: 26384 RVA: 0x00212188 File Offset: 0x00210388
		internal AnimationSequence GetNextAutoAttackSequence(AnimationFlags animFlags)
		{
			if (this.m_autoAttackShuffler == null)
			{
				List<AnimationSequence> list = null;
				for (int i = 0; i < this.m_attackAnimationData.Length; i++)
				{
					if (!this.m_attackAnimationData[i].Ignore && this.m_attackAnimationData[i].ExeTime == AnimationExecutionTime.k0)
					{
						for (int j = 0; j < this.m_attackAnimationData[i].AttackAnimations.Length; j++)
						{
							AbilityAnimation abilityAnimation = this.m_attackAnimationData[i].AttackAnimations[j];
							if (abilityAnimation != null)
							{
								AnimationSequence animationSequence = abilityAnimation.GetAnimationSequence(AbilityAnimationType.Start);
								if (animationSequence != null && animationSequence.AllowAsAutoAttack)
								{
									if (list == null)
									{
										list = new List<AnimationSequence>(10);
									}
									list.Add(animationSequence.DuplicateForAutoAttack());
								}
								else
								{
									AnimationSequence animationSequence2 = abilityAnimation.GetAnimationSequence(AbilityAnimationType.End);
									if (animationSequence2 != null && animationSequence2.AllowAsAutoAttack)
									{
										if (list == null)
										{
											list = new List<AnimationSequence>(10);
										}
										list.Add(animationSequence2.DuplicateForAutoAttack());
									}
								}
							}
						}
						break;
					}
				}
				if (list != null && list.Count > 0)
				{
					this.m_dynamicAutoAttackSequences = list.ToArray();
					this.m_autoAttackShuffler = new ArrayShuffler<AnimationSequence>(this.m_dynamicAutoAttackSequences);
				}
				else
				{
					this.m_autoAttackShuffler = new ArrayShuffler<AnimationSequence>(this.m_autoAttackSequences);
				}
			}
			if (!this.m_evaluateAnimationFlags || animFlags == AnimationFlags.None)
			{
				return this.m_autoAttackShuffler.GetNext();
			}
			AnimationSequence[] array = (this.m_dynamicAutoAttackSequences != null) ? this.m_dynamicAutoAttackSequences : this.m_autoAttackSequences;
			for (int k = 0; k < array.Length; k++)
			{
				if (array[k].AnimationFlags == animFlags)
				{
					return array[k];
				}
			}
			return null;
		}

		// Token: 0x06006711 RID: 26385 RVA: 0x0008549E File Offset: 0x0008369E
		[ContextMenu("Reset Auto Attack Cache")]
		private void CacheAutoAttackSequences()
		{
			this.m_dynamicAutoAttackSequences = null;
			this.m_autoAttackShuffler = null;
		}

		// Token: 0x06006712 RID: 26386 RVA: 0x000854AE File Offset: 0x000836AE
		public List<AnimationClip> GetLocomotionClips()
		{
			if (!this.m_overrideThresholds)
			{
				return this.m_locomotionSet.GetAllClips();
			}
			return this.m_locomotionSetData.GetAllClips();
		}

		// Token: 0x06006713 RID: 26387 RVA: 0x00212304 File Offset: 0x00210504
		public void UpdateLeftRightMountData(ref Vector3 leftPos, ref Quaternion leftRot, ref Vector3 rightPos, ref Quaternion rightRot)
		{
			if (this.m_overrideLeftMountPos)
			{
				leftPos = this.m_leftMountPos;
			}
			if (this.m_overrideLeftMountRot)
			{
				leftRot = Quaternion.Euler(this.m_leftMountRot);
			}
			if (this.m_overrideRightMountPos)
			{
				rightPos = this.m_rightMountPos;
			}
			if (this.m_overrideRightMountRot)
			{
				rightRot = Quaternion.Euler(this.m_rightMountRot);
			}
		}

		// Token: 0x06006714 RID: 26388 RVA: 0x0021236C File Offset: 0x0021056C
		public MountPosition GetMountPosition(EquipmentSlot slot, bool isAttached)
		{
			switch (slot)
			{
			case EquipmentSlot.PrimaryWeapon_MainHand:
			case EquipmentSlot.SecondaryWeapon_MainHand:
				if (!isAttached)
				{
					return this.m_primaryStashSlot;
				}
				return this.m_primaryMountSlot;
			case EquipmentSlot.PrimaryWeapon_OffHand:
				break;
			case EquipmentSlot.PrimaryWeapon_MainHand | EquipmentSlot.PrimaryWeapon_OffHand:
				return MountPosition.None;
			default:
				if (slot != EquipmentSlot.SecondaryWeapon_OffHand)
				{
					if (slot != EquipmentSlot.LightSource)
					{
						return MountPosition.None;
					}
					if (!isAttached)
					{
						return this.m_secondaryStashSlot;
					}
					return this.m_secondaryMountSlot;
				}
				break;
			}
			if (!isAttached)
			{
				return this.m_secondaryStashSlot;
			}
			return this.m_secondaryMountSlot;
		}

		// Token: 0x06006715 RID: 26389 RVA: 0x002123D4 File Offset: 0x002105D4
		internal AbilityAnimation GetAbilityAnimation(AnimationExecutionTime exeTime, int index)
		{
			if (this.m_attackAnimationDataDict == null)
			{
				this.m_attackAnimationDataDict = new Dictionary<AnimationExecutionTime, AbilityAnimation[]>(default(AnimationExecutionTimeComparer));
				for (int i = 0; i < this.m_attackAnimationData.Length; i++)
				{
					if (!this.m_attackAnimationData[i].Ignore)
					{
						this.m_attackAnimationDataDict.Add(this.m_attackAnimationData[i].ExeTime, this.m_attackAnimationData[i].AttackAnimations);
					}
				}
			}
			AbilityAnimation[] array;
			if (this.m_attackAnimationDataDict.TryGetValue(exeTime, out array) && index >= 0 && index < array.Length)
			{
				return array[index];
			}
			return null;
		}

		// Token: 0x04005965 RID: 22885
		private Dictionary<AnimationExecutionTime, AbilityAnimation[]> m_attackAnimationDataDict;

		// Token: 0x04005966 RID: 22886
		private ArrayShuffler<AnimationSequence> m_idleTickShuffler;

		// Token: 0x04005967 RID: 22887
		private ArrayShuffler<AnimationSequence> m_autoAttackShuffler;

		// Token: 0x04005968 RID: 22888
		private const string kAnimancerMixerGroupName = "Animancer Mixers";

		// Token: 0x04005969 RID: 22889
		[SerializeField]
		private LinearMixerTransition m_idle;

		// Token: 0x0400596A RID: 22890
		[SerializeField]
		private MixerTransition2D m_locomotion;

		// Token: 0x0400596B RID: 22891
		private const string kTransitionGroupName = "Transitions";

		// Token: 0x0400596C RID: 22892
		[SerializeField]
		private AnimationSequence m_enterTransitionSequence;

		// Token: 0x0400596D RID: 22893
		[SerializeField]
		private AnimationSequence m_exitTransitionSequence;

		// Token: 0x0400596E RID: 22894
		private const string kInternalDataGroupName = "Data";

		// Token: 0x0400596F RID: 22895
		[SerializeField]
		private DummyClass m_dummy;

		// Token: 0x04005970 RID: 22896
		[SerializeField]
		private IdleSet m_idleSet;

		// Token: 0x04005971 RID: 22897
		[SerializeField]
		private AnimationSequence[] m_idleTickSequences;

		// Token: 0x04005972 RID: 22898
		private const string kRunGroupName = "Data/Run";

		// Token: 0x04005973 RID: 22899
		private const string kLocomotionGroupName = "Locomotion Clips";

		// Token: 0x04005974 RID: 22900
		[SerializeField]
		private bool m_disableLateralMovement;

		// Token: 0x04005975 RID: 22901
		[SerializeField]
		private bool m_enableFootIkForLocomotion;

		// Token: 0x04005976 RID: 22902
		[SerializeField]
		private bool m_overrideThresholds;

		// Token: 0x04005977 RID: 22903
		[FormerlySerializedAs("m_setArray")]
		[SerializeField]
		private LocomotionSetArray m_locomotionSet;

		// Token: 0x04005978 RID: 22904
		[SerializeField]
		private LocomotionSetArrayWithThresholds m_locomotionSetData;

		// Token: 0x04005979 RID: 22905
		private const string kMountGroupName = "Mounts";

		// Token: 0x0400597A RID: 22906
		private const string kLeftMountGroupName = "Mounts/Left";

		// Token: 0x0400597B RID: 22907
		private const string kRightMountGroupName = "Mounts/Right";

		// Token: 0x0400597C RID: 22908
		[SerializeField]
		private string m_mountGroupName = string.Empty;

		// Token: 0x0400597D RID: 22909
		[SerializeField]
		private MountPosition m_primaryMountSlot;

		// Token: 0x0400597E RID: 22910
		[SerializeField]
		private MountPosition m_secondaryMountSlot;

		// Token: 0x0400597F RID: 22911
		[SerializeField]
		private MountPosition m_primaryStashSlot;

		// Token: 0x04005980 RID: 22912
		[SerializeField]
		private MountPosition m_secondaryStashSlot;

		// Token: 0x04005981 RID: 22913
		[SerializeField]
		private DummyClass m_dummyRight;

		// Token: 0x04005982 RID: 22914
		[SerializeField]
		private bool m_overrideRightMountPos;

		// Token: 0x04005983 RID: 22915
		[SerializeField]
		private Vector3 m_rightMountPos = Vector3.zero;

		// Token: 0x04005984 RID: 22916
		[SerializeField]
		private bool m_overrideRightMountRot;

		// Token: 0x04005985 RID: 22917
		[SerializeField]
		private Vector3 m_rightMountRot = Vector3.zero;

		// Token: 0x04005986 RID: 22918
		[SerializeField]
		private DummyClass m_dummyLeft;

		// Token: 0x04005987 RID: 22919
		[SerializeField]
		private bool m_overrideLeftMountPos;

		// Token: 0x04005988 RID: 22920
		[SerializeField]
		private Vector3 m_leftMountPos = Vector3.zero;

		// Token: 0x04005989 RID: 22921
		[SerializeField]
		private bool m_overrideLeftMountRot;

		// Token: 0x0400598A RID: 22922
		[SerializeField]
		private Vector3 m_leftMountRot = Vector3.zero;

		// Token: 0x0400598B RID: 22923
		private const string kCombatGroupName = "Data/Combat";

		// Token: 0x0400598C RID: 22924
		private const string kCombatGroupTitle = "Combat";

		// Token: 0x0400598D RID: 22925
		[SerializeField]
		private bool m_isCombatStance;

		// Token: 0x0400598E RID: 22926
		[SerializeField]
		private bool m_evaluateAnimationFlags;

		// Token: 0x0400598F RID: 22927
		[SerializeField]
		private AnimationSequence[] m_autoAttackSequences;

		// Token: 0x04005990 RID: 22928
		[SerializeField]
		private AnimancerAnimationSet.AbilityAnimationData[] m_attackAnimationData;

		// Token: 0x04005991 RID: 22929
		[NonSerialized]
		private AnimationSequence[] m_dynamicAutoAttackSequences;

		// Token: 0x04005992 RID: 22930
		private const string kMiscGroupName = "Misc";

		// Token: 0x04005993 RID: 22931
		[SerializeField]
		private AnimationSequence m_getHitSequence;

		// Token: 0x04005994 RID: 22932
		[SerializeField]
		private AnimationSequence m_avoidSequence;

		// Token: 0x04005995 RID: 22933
		[SerializeField]
		private AnimationSequence m_blockSequence;

		// Token: 0x04005996 RID: 22934
		[SerializeField]
		private AnimationSequence m_parrySequence;

		// Token: 0x04005997 RID: 22935
		[SerializeField]
		private AnimationSequence m_riposteSequence;

		// Token: 0x04005998 RID: 22936
		private const string kAlchemyGroupName = "Alchemy";

		// Token: 0x04005999 RID: 22937
		[SerializeField]
		private AnimationSequence m_alchemyISequence;

		// Token: 0x0400599A RID: 22938
		[SerializeField]
		private AnimationSequence m_alchemyIISequence;

		// Token: 0x02000D5E RID: 3422
		[Serializable]
		private class AbilityAnimationData
		{
			// Token: 0x0400599B RID: 22939
			public bool Ignore;

			// Token: 0x0400599C RID: 22940
			public AnimationExecutionTime ExeTime;

			// Token: 0x0400599D RID: 22941
			public AbilityAnimation[] AttackAnimations;
		}
	}
}
