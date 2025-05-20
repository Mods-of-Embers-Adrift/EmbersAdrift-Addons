using System;
using RootMotion;
using RootMotion.FinalIK;
using SoL.Game.Settings;
using SoL.Managers;
using UMA;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game
{
	// Token: 0x02000591 RID: 1425
	public class UMAIKController : IKController
	{
		// Token: 0x17000966 RID: 2406
		// (get) Token: 0x06002C84 RID: 11396 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool HasIkAssignedOnAwake
		{
			get
			{
				return false;
			}
		}

		// Token: 0x14000090 RID: 144
		// (add) Token: 0x06002C85 RID: 11397 RVA: 0x00149FC4 File Offset: 0x001481C4
		// (remove) Token: 0x06002C86 RID: 11398 RVA: 0x00149FFC File Offset: 0x001481FC
		public event Action IkSetupCompleteEvent;

		// Token: 0x17000967 RID: 2407
		// (get) Token: 0x06002C87 RID: 11399 RVA: 0x0005EE41 File Offset: 0x0005D041
		public bool IkSetupComplete
		{
			get
			{
				return this.m_ikSetupComplete;
			}
		}

		// Token: 0x06002C88 RID: 11400 RVA: 0x0014A034 File Offset: 0x00148234
		protected override void Start()
		{
			base.Start();
			if (base.GameEntity != null)
			{
				if (base.GameEntity.DCAController != null)
				{
					base.GameEntity.DCAController.DCA.CharacterCreated.AddListener(new UnityAction<UMAData>(this.UmaCreated));
				}
				this.m_hasTargetController = (base.GameEntity.TargetController != null);
			}
		}

		// Token: 0x06002C89 RID: 11401 RVA: 0x0005EE49 File Offset: 0x0005D049
		protected override void LateUpdate()
		{
			if (this.m_umaInitialized && !this.m_ikSetupComplete && this.m_umaData)
			{
				this.SetupIfWithinRange();
			}
			base.LateUpdate();
		}

		// Token: 0x06002C8A RID: 11402 RVA: 0x0005EE74 File Offset: 0x0005D074
		private void UmaCreated(UMAData umaData)
		{
			base.GameEntity.DCAController.DCA.CharacterCreated.RemoveListener(new UnityAction<UMAData>(this.UmaCreated));
			this.m_umaInitialized = true;
			this.m_umaData = umaData;
		}

		// Token: 0x06002C8B RID: 11403 RVA: 0x0014A0A8 File Offset: 0x001482A8
		private void SetupIfWithinRange()
		{
			if (this.m_ikSetupComplete || !this.m_umaInitialized || !this.m_umaData)
			{
				return;
			}
			if (base.GameEntity && LocalPlayer.GameEntity && base.GameEntity.GetCachedSqrDistanceFromCamera() > LocalPlayer.GameEntity.Vitals.MaxTargetDistanceSqr)
			{
				return;
			}
			UMAData umaData = this.m_umaData;
			BipedReferences bipedReferences = null;
			BipedReferences.AutoDetectReferences(ref bipedReferences, umaData.gameObject.transform, BipedReferences.AutoDetectParams.Default);
			if (base.GameEntity && base.GameEntity.CharacterData && base.GameEntity.CharacterData.ReferencePoints != null)
			{
				if (base.GameEntity.CharacterData.ReferencePoints.Value.LeftMount != null)
				{
					bipedReferences.leftHand = IKController.CreateHandIk("LeftHandIK", base.GameEntity.CharacterData.ReferencePoints.Value.LeftMount.transform).transform;
				}
				if (base.GameEntity.CharacterData.ReferencePoints.Value.RightMount != null)
				{
					bipedReferences.rightHand = IKController.CreateHandIk("RightHandIK", base.GameEntity.CharacterData.ReferencePoints.Value.RightMount.transform).transform;
				}
			}
			FullBodyBipedIK fullBodyBipedIK = umaData.gameObject.AddComponent<FullBodyBipedIK>();
			fullBodyBipedIK.SetReferences(bipedReferences, null);
			fullBodyBipedIK.solver.iterations = 0;
			this.m_main = fullBodyBipedIK;
			this.m_fbbik = fullBodyBipedIK;
			GrounderFBBIK grounderFBBIK = umaData.gameObject.AddComponent<GrounderFBBIK>();
			grounderFBBIK.ik = fullBodyBipedIK;
			grounderFBBIK.solver.layers = base.GroundLayers.value;
			grounderFBBIK.solver.maxFootRotationAngle = GlobalSettings.Values.Uma.MaxFootRotationAngle;
			this.m_grounder = grounderFBBIK;
			LookAtIK lookAtIK = umaData.gameObject.AddComponent<LookAtIK>();
			lookAtIK.solver.SetChain(fullBodyBipedIK.references.spine, fullBodyBipedIK.references.head, fullBodyBipedIK.references.eyes, fullBodyBipedIK.references.root);
			lookAtIK.solver.bodyWeight = GlobalSettings.Values.Uma.LookBodyWeight;
			lookAtIK.solver.headWeight = GlobalSettings.Values.Uma.LookHeadWeight;
			lookAtIK.solver.eyesWeight = GlobalSettings.Values.Uma.LookEyeWeight;
			this.m_look = lookAtIK;
			base.SetupIKData();
			GameObject gameObject = null;
			this.m_isLocal = (base.GameEntity != null && base.GameEntity.NetworkEntity != null && base.GameEntity.NetworkEntity.IsLocal);
			if (gameObject == null)
			{
				gameObject = new GameObject("LOOK_AT_TARGET");
			}
			if (!this.m_isLocal)
			{
				gameObject.transform.parent = umaData.gameObject.transform;
			}
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localPosition = new Vector3(0f, fullBodyBipedIK.references.head.transform.position.y - umaData.transform.position.y, 10f);
			this.m_source = fullBodyBipedIK.references.head;
			this.m_lookSource = this.m_source;
			this.m_otherLookAtTarget = this.m_source;
			this.m_look.solver.target = gameObject.transform;
			this.m_lookAtTarget = gameObject;
			base.RefreshCullingDistance();
			this.m_ikSetupComplete = true;
			Action ikSetupCompleteEvent = this.IkSetupCompleteEvent;
			if (ikSetupCompleteEvent == null)
			{
				return;
			}
			ikSetupCompleteEvent();
		}

		// Token: 0x06002C8C RID: 11404 RVA: 0x0014A480 File Offset: 0x00148680
		private void UpdateLookTarget()
		{
			Vector3 vector = Vector3.zero;
			bool flag = false;
			bool flag2 = false;
			if (this.m_hasTargetController && base.GameEntity.TargetController.OffensiveTarget != null)
			{
				vector = ((base.GameEntity.TargetController.OffensiveTarget.IKController != null) ? base.GameEntity.TargetController.OffensiveTarget.IKController.LookAtMe.position : base.GameEntity.TargetController.OffensiveTarget.gameObject.transform.position);
				flag = true;
				flag2 = true;
			}
			else if (this.m_hasTargetController && base.GameEntity.TargetController.DefensiveTarget != null)
			{
				vector = ((base.GameEntity.TargetController.DefensiveTarget.IKController != null) ? base.GameEntity.TargetController.DefensiveTarget.IKController.LookAtMe.position : base.GameEntity.TargetController.DefensiveTarget.gameObject.transform.position);
				flag = true;
				flag2 = true;
			}
			else if (base.GameEntity.NetworkEntity.IsLocal)
			{
				vector = ClientGameManager.MainCamera.ScreenPointToRay(new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f)).GetPoint(GlobalSettings.Values.Ik.HeadLookDistance);
				flag2 = true;
			}
			else
			{
				vector = this.m_source.forward * GlobalSettings.Values.Ik.HeadLookDistance;
			}
			Vector3 vector2 = vector - this.m_source.position;
			float magnitude = vector2.magnitude;
			vector2 /= magnitude;
			if (!flag && Vector3.Dot(vector2, Vector3.up) < 0f)
			{
				vector2.y = 0f;
				vector2.Normalize();
				vector = new Vector3(vector.x, this.m_source.position.y, vector.z);
			}
			float num = Vector3.Angle(base.transform.forward, vector2);
			float headLookAngle = GlobalSettings.Values.Ik.HeadLookAngle;
			float num2 = GlobalSettings.Values.Ik.HeadLookDistance * 1.1f;
			bool flag3 = flag2 && num < headLookAngle && magnitude < num2;
			this.m_look.solver.IKPositionWeight = Mathf.Lerp(this.m_look.solver.IKPositionWeight, flag3 ? 1f : 0f, Time.deltaTime * GlobalSettings.Values.Ik.HeadLookWeightLerpRate);
			this.m_lookAtTarget.transform.position = Vector3.Lerp(this.m_lookAtTarget.transform.position, vector, Time.deltaTime * GlobalSettings.Values.Ik.HeadLookTargetMoveRate);
		}

		// Token: 0x04002C42 RID: 11330
		private bool m_ikSetupComplete;

		// Token: 0x04002C43 RID: 11331
		private bool m_umaInitialized;

		// Token: 0x04002C44 RID: 11332
		private UMAData m_umaData;

		// Token: 0x04002C45 RID: 11333
		private bool m_isLocal;

		// Token: 0x04002C46 RID: 11334
		private Transform m_source;
	}
}
