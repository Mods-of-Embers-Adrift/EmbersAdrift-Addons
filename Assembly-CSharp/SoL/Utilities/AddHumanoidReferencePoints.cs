using System;
using RootMotion;
using RootMotion.FinalIK;
using SoL.Game;
using SoL.Game.Animation;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Pooling;
using SoL.Networking.Database;
using SoL.Tests;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x0200023D RID: 573
	public class AddHumanoidReferencePoints : MonoBehaviour
	{
		// Token: 0x060012EC RID: 4844 RVA: 0x000E84A4 File Offset: 0x000E66A4
		private void AddThem()
		{
			if (this.m_animator != null)
			{
				if (this.m_referencePoints != null)
				{
					this.m_referencePoints.Value.DestroyReferencePoints();
					this.m_referencePoints = null;
				}
				this.m_referencePoints = new HumanoidReferencePoints?(this.m_animator.GetReferencePoints(this.m_sex, this.m_correctMountPoints));
				if (this.m_fbbik)
				{
					BipedReferences references = this.m_fbbik.references;
					if (this.m_referencePoints.Value.LeftMount != null)
					{
						references.leftHand = IKController.CreateHandIk("LeftHandIK", this.m_referencePoints.Value.LeftMount.transform).transform;
					}
					if (this.m_referencePoints.Value.RightMount != null)
					{
						references.rightHand = IKController.CreateHandIk("RightHandIK", this.m_referencePoints.Value.RightMount.transform).transform;
					}
					this.m_fbbik.SetReferences(references, null);
				}
			}
		}

		// Token: 0x060012ED RID: 4845 RVA: 0x0004F767 File Offset: 0x0004D967
		private void StateChanged()
		{
			this.m_animator.SetInteger("State", this.State);
		}

		// Token: 0x060012EE RID: 4846 RVA: 0x0004F77F File Offset: 0x0004D97F
		private void SubStateChanged()
		{
			this.m_animator.SetInteger("SubState", this.SubState);
		}

		// Token: 0x060012EF RID: 4847 RVA: 0x0004F797 File Offset: 0x0004D997
		private void AimChanged()
		{
			this.m_animator.SetBool("Aim", this.Aim);
		}

		// Token: 0x060012F0 RID: 4848 RVA: 0x0004F7AF File Offset: 0x0004D9AF
		private void SpeedChanged()
		{
			this.m_animator.SetFloat("AnimSpeed", this.Speed);
		}

		// Token: 0x060012F1 RID: 4849 RVA: 0x0004475B File Offset: 0x0004295B
		private void RefreshHandTargets()
		{
		}

		// Token: 0x060012F2 RID: 4850 RVA: 0x000E85C0 File Offset: 0x000E67C0
		private void UpdateEffectorTarget(IKSolverFullBodyBiped solver, FullBodyBipedEffector effectorType, Transform target, float targetWeightFraction)
		{
			if (solver == null)
			{
				return;
			}
			IKEffector effector = solver.GetEffector(effectorType);
			float num = (target == null) ? 0f : 1f;
			num *= targetWeightFraction;
			effector.positionWeight = num;
			effector.target = target;
			solver.GetLimbMapping(effectorType).maintainRotationWeight = 1f;
		}

		// Token: 0x060012F3 RID: 4851 RVA: 0x000E8614 File Offset: 0x000E6814
		private void MountItem()
		{
			if (this.m_item == null)
			{
				return;
			}
			if (this.m_referencePoints == null)
			{
				this.AddThem();
			}
			EquipmentSlot slot = this.m_mainHand ? EquipmentSlot.PrimaryWeapon_MainHand : EquipmentSlot.PrimaryWeapon_OffHand;
			this.m_item.gameObject.transform.SetParentResetScale(base.gameObject.transform);
			GameEntity component = base.gameObject.GetComponent<GameEntity>();
			AnimancerAnimationSet combatSet;
			if (this.State == 1 && AnimancerSetIndex.AnimancerIndexDict.TryGetValue(this.SubState, out combatSet))
			{
				this.m_item.MountItem(this.m_referencePoints, slot, this.m_isAttached, combatSet, component);
			}
			else
			{
				this.m_item.MountItem(this.m_referencePoints, slot, this.m_isAttached, null, component);
			}
			this.RefreshHandTargets();
		}

		// Token: 0x060012F4 RID: 4852 RVA: 0x0004F7C7 File Offset: 0x0004D9C7
		private void UnmountItem()
		{
			if (this.m_item == null)
			{
				return;
			}
			this.RefreshHandTargets();
		}

		// Token: 0x060012F5 RID: 4853 RVA: 0x000E86D8 File Offset: 0x000E68D8
		private void AdjustMounts()
		{
			if (this.m_referencePoints == null)
			{
				return;
			}
			Vector3 handMountPos_L = AnimatorExtensions.HandMountPos_L;
			Quaternion handMountRot_L = AnimatorExtensions.HandMountRot_L;
			Vector3 handMountPos_R = AnimatorExtensions.HandMountPos_R;
			Quaternion handMountRot_R = AnimatorExtensions.HandMountRot_R;
			AnimancerAnimationSet animancerAnimationSet;
			if (this.State == 1 && AnimancerSetIndex.AnimancerIndexDict.TryGetValue(this.SubState, out animancerAnimationSet))
			{
				animancerAnimationSet.UpdateLeftRightMountData(ref handMountPos_L, ref handMountRot_L, ref handMountPos_R, ref handMountRot_R);
			}
			this.m_referencePoints.Value.LeftMount.transform.localPosition = handMountPos_L;
			this.m_referencePoints.Value.LeftMount.transform.localRotation = handMountRot_L;
			this.m_referencePoints.Value.RightMount.transform.localPosition = handMountPos_R;
			this.m_referencePoints.Value.RightMount.transform.localRotation = handMountRot_R;
		}

		// Token: 0x060012F6 RID: 4854 RVA: 0x000E87A4 File Offset: 0x000E69A4
		private void ResetMounts()
		{
			if (this.m_referencePoints == null)
			{
				return;
			}
			this.m_referencePoints.Value.LeftMount.transform.localPosition = AnimatorExtensions.HandMountPos_L;
			this.m_referencePoints.Value.LeftMount.transform.localRotation = AnimatorExtensions.HandMountRot_L;
			this.m_referencePoints.Value.RightMount.transform.localPosition = AnimatorExtensions.HandMountPos_R;
			this.m_referencePoints.Value.RightMount.transform.localRotation = AnimatorExtensions.HandMountRot_R;
		}

		// Token: 0x040010C1 RID: 4289
		private const string kAnimatorGroup = "Animator";

		// Token: 0x040010C2 RID: 4290
		private const string kAnimatorHorizontalGroup = "Animator/Buttons";

		// Token: 0x040010C3 RID: 4291
		[SerializeField]
		private Animator m_animator;

		// Token: 0x040010C4 RID: 4292
		[SerializeField]
		private CharacterSex m_sex = CharacterSex.Male;

		// Token: 0x040010C5 RID: 4293
		[SerializeField]
		private bool m_correctMountPoints;

		// Token: 0x040010C6 RID: 4294
		private HumanoidReferencePoints? m_referencePoints;

		// Token: 0x040010C7 RID: 4295
		public int State;

		// Token: 0x040010C8 RID: 4296
		public int SubState;

		// Token: 0x040010C9 RID: 4297
		public bool Aim;

		// Token: 0x040010CA RID: 4298
		[Range(0f, 1f)]
		public float Speed = 1f;

		// Token: 0x040010CB RID: 4299
		private const string kMountGroup = "Mounts";

		// Token: 0x040010CC RID: 4300
		private const string kMountGroupButtons = "Mounts/Buttons";

		// Token: 0x040010CD RID: 4301
		[SerializeField]
		private FullBodyBipedIK m_fbbik;

		// Token: 0x040010CE RID: 4302
		[SerializeField]
		private PooledHandheldItem m_item;

		// Token: 0x040010CF RID: 4303
		[SerializeField]
		private bool m_isAttached = true;

		// Token: 0x040010D0 RID: 4304
		[SerializeField]
		private bool m_mainHand = true;
	}
}
