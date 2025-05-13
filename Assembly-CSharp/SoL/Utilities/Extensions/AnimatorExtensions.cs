using System;
using SoL.Game;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.Utilities.Extensions
{
	// Token: 0x02000322 RID: 802
	public static class AnimatorExtensions
	{
		// Token: 0x1700054D RID: 1357
		// (get) Token: 0x06001624 RID: 5668 RVA: 0x0005180D File Offset: 0x0004FA0D
		// (set) Token: 0x06001625 RID: 5669 RVA: 0x00051814 File Offset: 0x0004FA14
		public static Vector3 HandMountPos_L { get; private set; }

		// Token: 0x1700054E RID: 1358
		// (get) Token: 0x06001626 RID: 5670 RVA: 0x0005181C File Offset: 0x0004FA1C
		// (set) Token: 0x06001627 RID: 5671 RVA: 0x00051823 File Offset: 0x0004FA23
		public static Vector3 HandMountPos_R { get; private set; }

		// Token: 0x1700054F RID: 1359
		// (get) Token: 0x06001628 RID: 5672 RVA: 0x0005182B File Offset: 0x0004FA2B
		// (set) Token: 0x06001629 RID: 5673 RVA: 0x00051832 File Offset: 0x0004FA32
		public static Quaternion HandMountRot_L { get; private set; }

		// Token: 0x17000550 RID: 1360
		// (get) Token: 0x0600162A RID: 5674 RVA: 0x0005183A File Offset: 0x0004FA3A
		// (set) Token: 0x0600162B RID: 5675 RVA: 0x00051841 File Offset: 0x0004FA41
		public static Quaternion HandMountRot_R { get; private set; }

		// Token: 0x0600162C RID: 5676 RVA: 0x000FEC0C File Offset: 0x000FCE0C
		public static HumanoidReferencePoints GetReferencePoints(this Animator animator, CharacterSex sex, bool correctMountPositions = false)
		{
			Transform transform = animator.gameObject.transform;
			Transform boneTransform = animator.GetBoneTransform(HumanBodyBones.RightEye);
			GameObject gameObject = new GameObject("EyeHeight");
			gameObject.transform.SetParent(boneTransform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.rotation = Quaternion.identity;
			GameObject gameObject2 = new GameObject("Overhead");
			gameObject2.transform.SetParent(gameObject.transform);
			gameObject2.transform.localPosition = new Vector3(0f, 0.25f, 0f);
			gameObject2.transform.localRotation = Quaternion.identity;
			gameObject.transform.SetParent(animator.transform);
			gameObject.transform.localPosition = new Vector3(0f, gameObject.transform.localPosition.y, 0f);
			gameObject.transform.localRotation = Quaternion.identity;
			Transform transform2 = animator.gameObject.transform.Find("Root/Global/Position/Hips/Spine1/Spine2/Chest");
			if (transform2 == null)
			{
				transform2 = animator.GetBoneTransform(HumanBodyBones.Spine);
			}
			Quaternion localRot = Quaternion.identity;
			Quaternion localRot2 = Quaternion.identity;
			Quaternion localRot3 = Quaternion.identity;
			Quaternion localRot4 = Quaternion.identity;
			Quaternion localRot5 = Quaternion.identity;
			if (sex != CharacterSex.Male)
			{
				if (sex == CharacterSex.Female)
				{
					localRot = Quaternion.Euler(new Vector3(-4.573f, -7.807f, 78.117f));
					localRot2 = Quaternion.Euler(new Vector3(4.573f, -7.807f, 101.883f));
					localRot3 = Quaternion.Euler(new Vector3(0f, 0f, -5f));
					localRot4 = Quaternion.Euler(new Vector3(0f, 0f, -4.486f));
					localRot5 = Quaternion.Euler(new Vector3(0f, 0f, 4.486f));
				}
			}
			else
			{
				localRot = Quaternion.Euler(new Vector3(-15.217f, -8.024f, 95.111f));
				localRot2 = Quaternion.Euler(new Vector3(15.219f, -8.024f, 84.889f));
				localRot3 = Quaternion.Euler(new Vector3(0f, 0f, 7.268f));
				localRot4 = Quaternion.Euler(new Vector3(0f, 0f, -18.01f));
				localRot5 = Quaternion.Euler(new Vector3(0f, 0f, 18.01f));
			}
			HumanoidReferencePoints humanoidReferencePoints = new HumanoidReferencePoints
			{
				LeftMount = AnimatorExtensions.GetNewMountPosition(transform, "Root/Global/Position/Hips/Spine1/Spine2/Chest/LeftCollarbone/LeftArmUp/LeftArmDn/LeftHand/Mount_Hand-Left", "LeftMountParent", "LeftMount", Vector3.zero, localRot),
				RightMount = AnimatorExtensions.GetNewMountPosition(transform, "Root/Global/Position/Hips/Spine1/Spine2/Chest/RightCollarbone/RightArmUp/RightArmDn/RightHand/Mount_Hand-Right", "RightMountParent", "RightMount", Vector3.zero, localRot2),
				BackMount = AnimatorExtensions.GetNewMountPosition(transform, "Root/Global/Position/Hips/Spine1/Spine2/Chest/Mount_Back", "BackMountParent", "BackMount", new Vector3(0f, 0.04f, 0f), localRot3),
				LeftShoulderMount = AnimatorExtensions.GetNewMountPosition(transform, "Root/Global/Position/Hips/Spine1/Spine2/Chest/Mount_Shoulder-Left", "LeftShoulderMountParent", "LeftShoulderMount", new Vector3(0f, -0.05f, 0f), localRot4),
				RightShoulderMount = AnimatorExtensions.GetNewMountPosition(transform, "Root/Global/Position/Hips/Spine1/Spine2/Chest/Mount_Shoulder-Right", "RightShoulderMountParent", "RightShoulderMount", new Vector3(0f, 0.07f, 0f), localRot5),
				LeftHipMount = AnimatorExtensions.GetMountPointForBone(transform, "Root/Global/Position/Hips/Mount_Hip-Left"),
				RightHipMount = AnimatorExtensions.GetMountPointForBone(transform, "Root/Global/Position/Hips/Mount_Hip-Right"),
				EyeHeight = gameObject,
				Overhead = gameObject2,
				DamageTarget = transform2.gameObject
			};
			if (correctMountPositions)
			{
				Transform transform3 = transform.Find("Root/Global/Position/Hips/Spine1/Spine2/Chest");
				if (transform3 != null)
				{
					humanoidReferencePoints.BackMount.transform.localPosition = AnimatorExtensions.GetCorrectedYPos(humanoidReferencePoints.BackMount.transform, transform3);
					Transform transform4 = transform3.Find("LeftCollarbone");
					if (transform4 != null)
					{
						humanoidReferencePoints.LeftShoulderMount.transform.localPosition = AnimatorExtensions.GetCorrectedYPos(humanoidReferencePoints.LeftShoulderMount.transform, transform4);
					}
					Transform transform5 = transform3.Find("RightCollarbone");
					if (transform5 != null)
					{
						humanoidReferencePoints.RightShoulderMount.transform.localPosition = AnimatorExtensions.GetCorrectedYPos(humanoidReferencePoints.RightShoulderMount.transform, transform5);
					}
				}
			}
			humanoidReferencePoints.BackMount.transform.localRotation *= AnimatorExtensions.m_mountRotationCorrection;
			humanoidReferencePoints.LeftHipMount.transform.localRotation *= AnimatorExtensions.m_mountRotationCorrection;
			humanoidReferencePoints.RightHipMount.transform.localRotation *= AnimatorExtensions.m_mountRotationCorrection;
			humanoidReferencePoints.LeftShoulderMount.transform.localRotation *= AnimatorExtensions.m_mountRotationCorrection;
			humanoidReferencePoints.RightShoulderMount.transform.localRotation *= AnimatorExtensions.m_mountRotationCorrection;
			if (!AnimatorExtensions.m_handDataCached)
			{
				AnimatorExtensions.HandMountPos_L = Vector3.zero;
				AnimatorExtensions.HandMountPos_R = Vector3.zero;
				AnimatorExtensions.HandMountRot_L = Quaternion.identity;
				AnimatorExtensions.HandMountRot_R = Quaternion.identity;
				AnimatorExtensions.m_handDataCached = true;
			}
			return humanoidReferencePoints;
		}

		// Token: 0x0600162D RID: 5677 RVA: 0x000FF130 File Offset: 0x000FD330
		private static GameObject GetMountPointForBone(Transform source, string bonePath)
		{
			Transform transform = source.Find(bonePath);
			if (!transform)
			{
				return null;
			}
			GameObject gameObject = new GameObject("MountPoint");
			gameObject.transform.SetParent(transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			return gameObject;
		}

		// Token: 0x0600162E RID: 5678 RVA: 0x000FF198 File Offset: 0x000FD398
		private static Vector3 GetCorrectedYPos(Transform sourceObject, Transform targetObject)
		{
			Vector3 position = new Vector3(sourceObject.position.x, targetObject.position.y, sourceObject.position.z);
			return sourceObject.InverseTransformPoint(position);
		}

		// Token: 0x0600162F RID: 5679 RVA: 0x000FF1D4 File Offset: 0x000FD3D4
		private static GameObject GetNewMountPosition(Transform trans, string parentBone, string mountParentName, string mountName, Vector3 localPos, Quaternion localRot)
		{
			Transform parent = trans.Find(parentBone);
			GameObject gameObject = new GameObject(mountParentName);
			gameObject.transform.SetParent(parent);
			gameObject.transform.localPosition = localPos;
			gameObject.transform.localRotation = localRot;
			gameObject.transform.localScale = Vector3.one;
			GameObject gameObject2 = new GameObject(mountName);
			gameObject2.transform.SetParent(gameObject.transform);
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localRotation = Quaternion.identity;
			gameObject2.transform.localScale = Vector3.one;
			return gameObject2;
		}

		// Token: 0x04001E32 RID: 7730
		private static bool m_handDataCached = false;

		// Token: 0x04001E37 RID: 7735
		private const string kHipsBoneName = "Root/Global/Position/Hips";

		// Token: 0x04001E38 RID: 7736
		private const string kChestBoneName = "Root/Global/Position/Hips/Spine1/Spine2/Chest";

		// Token: 0x04001E39 RID: 7737
		private const string kLeftHipMount = "Root/Global/Position/Hips/Mount_Hip-Left";

		// Token: 0x04001E3A RID: 7738
		private const string kRightHipMount = "Root/Global/Position/Hips/Mount_Hip-Right";

		// Token: 0x04001E3B RID: 7739
		private const string kLeftShoulderMount = "Root/Global/Position/Hips/Spine1/Spine2/Chest/Mount_Shoulder-Left";

		// Token: 0x04001E3C RID: 7740
		private const string kRightShoulderMount = "Root/Global/Position/Hips/Spine1/Spine2/Chest/Mount_Shoulder-Right";

		// Token: 0x04001E3D RID: 7741
		private const string kBackMount = "Root/Global/Position/Hips/Spine1/Spine2/Chest/Mount_Back";

		// Token: 0x04001E3E RID: 7742
		private const string kLeftHand = "Root/Global/Position/Hips/Spine1/Spine2/Chest/LeftCollarbone/LeftArmUp/LeftArmDn/LeftHand";

		// Token: 0x04001E3F RID: 7743
		private const string kLeftHandMount = "Root/Global/Position/Hips/Spine1/Spine2/Chest/LeftCollarbone/LeftArmUp/LeftArmDn/LeftHand/Mount_Hand-Left";

		// Token: 0x04001E40 RID: 7744
		private const string kRightHand = "Root/Global/Position/Hips/Spine1/Spine2/Chest/RightCollarbone/RightArmUp/RightArmDn/RightHand";

		// Token: 0x04001E41 RID: 7745
		private const string kRightHandMount = "Root/Global/Position/Hips/Spine1/Spine2/Chest/RightCollarbone/RightArmUp/RightArmDn/RightHand/Mount_Hand-Right";

		// Token: 0x04001E42 RID: 7746
		private static readonly Quaternion m_mountRotationCorrection = Quaternion.Euler(new Vector3(0f, 0f, 90f));

		// Token: 0x04001E43 RID: 7747
		private static readonly Vector3 m_backMountLocalPosition = new Vector3(0.022f, -0.159f, 0.002f);

		// Token: 0x04001E44 RID: 7748
		private static readonly Vector3 m_shoulderMountLocalPosition = new Vector3(0.02f, -0.165f, 0.029f);
	}
}
