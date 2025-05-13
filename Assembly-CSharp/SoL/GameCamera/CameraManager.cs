using System;
using System.Collections.Generic;
using SoL.Game;
using SoL.Game.Settings;
using SoL.Game.UI;
using SoL.Managers;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.GameCamera
{
	// Token: 0x02000DE1 RID: 3553
	public class CameraManager : GameEntityComponent
	{
		// Token: 0x1700193C RID: 6460
		// (get) Token: 0x060069D4 RID: 27092 RVA: 0x00086F5A File Offset: 0x0008515A
		// (set) Token: 0x060069D5 RID: 27093 RVA: 0x00086F61 File Offset: 0x00085161
		public static ActiveCameraTypes ActiveType { get; private set; }

		// Token: 0x060069D6 RID: 27094 RVA: 0x00218944 File Offset: 0x00216B44
		public static Vector2 GetXYDelta()
		{
			float num = 4f;
			float num2 = 0.04f;
			if (GlobalSettings.Values != null && GlobalSettings.Values.Camera != null)
			{
				num = GlobalSettings.Values.Camera.LookInputSpeedX;
				num2 = GlobalSettings.Values.Camera.LookInputSpeedY;
			}
			float num3 = 0f;
			float num4 = 0f;
			if (ClientGameManager.InputManager != null)
			{
				num3 = ClientGameManager.InputManager.LookInput.x * num;
				num4 = ClientGameManager.InputManager.LookInput.y * num2;
			}
			if (SolInput.GetAxis(48) != 0f || SolInput.GetAxis(47) != 0f)
			{
				num3 *= Options.GameOptions.XMouseSensitivity.Value * 2f;
				num4 *= Options.GameOptions.YMouseSensitivity.Value;
				if (!Options.GameOptions.InvertMouse.Value)
				{
					num4 *= -1f;
				}
			}
			return new Vector2(num3, num4);
		}

		// Token: 0x14000125 RID: 293
		// (add) Token: 0x060069D7 RID: 27095 RVA: 0x00218A24 File Offset: 0x00216C24
		// (remove) Token: 0x060069D8 RID: 27096 RVA: 0x00218A5C File Offset: 0x00216C5C
		internal event Action ActiveCameraChanged;

		// Token: 0x1700193D RID: 6461
		// (get) Token: 0x060069D9 RID: 27097 RVA: 0x00086F69 File Offset: 0x00085169
		// (set) Token: 0x060069DA RID: 27098 RVA: 0x00086F71 File Offset: 0x00085171
		public float? LastActiveCameraXValue { get; private set; }

		// Token: 0x1700193E RID: 6462
		// (get) Token: 0x060069DB RID: 27099 RVA: 0x00086F7A File Offset: 0x0008517A
		public Transform Focus
		{
			get
			{
				if (!GameManager.IsOffline)
				{
					return base.GameEntity.gameObject.transform;
				}
				return base.gameObject.transform.root;
			}
		}

		// Token: 0x1700193F RID: 6463
		// (get) Token: 0x060069DC RID: 27100 RVA: 0x00086FA4 File Offset: 0x000851A4
		// (set) Token: 0x060069DD RID: 27101 RVA: 0x00218A94 File Offset: 0x00216C94
		public ActiveCameraTypes ActiveCameraType
		{
			get
			{
				return this.m_activeCameraType;
			}
			private set
			{
				if (value == this.m_activeCameraType)
				{
					return;
				}
				ICamera activeCamera;
				if (!this.m_cameras.TryGetValue(value, out activeCamera))
				{
					Debug.LogWarning("Could not find " + value.ToString() + " camera!");
					return;
				}
				ICamera activeCamera2 = this.m_activeCamera;
				this.LastActiveCameraXValue = ((activeCamera2 != null) ? new float?(activeCamera2.XValue) : null);
				this.m_activeCamera = activeCamera;
				this.m_activeCameraType = value;
				CameraManager.ActiveType = this.m_activeCameraType;
				if (base.GameEntity)
				{
					bool isFirstPerson = this.m_activeCamera is FirstPersonCamera;
					if (base.GameEntity.DCAController)
					{
						base.GameEntity.DCAController.ToggleFirstPerson(isFirstPerson);
					}
					if (base.GameEntity.HandheldMountController)
					{
						base.GameEntity.HandheldMountController.ToggleFirstPerson(isFirstPerson);
					}
					if (base.GameEntity.UpperLineOfSightTarget)
					{
						base.GameEntity.UpperLineOfSightTarget.RefreshHeadCollider();
					}
				}
				Action activeCameraChanged = this.ActiveCameraChanged;
				if (activeCameraChanged != null)
				{
					activeCameraChanged();
				}
				this.RefreshCenterReticle();
			}
		}

		// Token: 0x17001940 RID: 6464
		// (get) Token: 0x060069DE RID: 27102 RVA: 0x00086FAC File Offset: 0x000851AC
		public int ZoomTransitionFrameCount
		{
			get
			{
				return this.m_zoomTransitionFrameCount;
			}
		}

		// Token: 0x060069DF RID: 27103 RVA: 0x00218BB8 File Offset: 0x00216DB8
		private void Start()
		{
			this.m_leftDragPrevention = new MouseDragPrevention(0);
			this.m_rightDragPrevention = new MouseDragPrevention(1);
			if (LocalPlayer.GameEntity != null && LocalPlayer.GameEntity.VitalsReplicator)
			{
				LocalPlayer.GameEntity.VitalsReplicator.CurrentHealthState.Changed += this.HealthStateOnChanged;
			}
			this.SetActiveCameraBasedOnOptions();
		}

		// Token: 0x060069E0 RID: 27104 RVA: 0x00218C24 File Offset: 0x00216E24
		private void OnDestroy()
		{
			if (LocalPlayer.GameEntity != null && LocalPlayer.GameEntity.VitalsReplicator)
			{
				LocalPlayer.GameEntity.VitalsReplicator.CurrentHealthState.Changed -= this.HealthStateOnChanged;
			}
			ICamera camera;
			if (this.m_cameras.TryGetValue(ActiveCameraTypes.FirstPerson, out camera))
			{
				camera.ExternalDestroy();
			}
		}

		// Token: 0x060069E1 RID: 27105 RVA: 0x00218C88 File Offset: 0x00216E88
		private void Update()
		{
			if (ClientGameManager.InputManager == null || this.m_activeCamera == null)
			{
				return;
			}
			this.m_leftDragPrevention.UpdateExternal();
			this.m_rightDragPrevention.UpdateExternal();
			bool flag = !this.m_leftDragPrevention.Prevent && !this.m_rightDragPrevention.Prevent && (ClientGameManager.InputManager.HoldingLMBRaw || ClientGameManager.InputManager.HoldingRMBRaw);
			bool flag2 = ClientGameManager.UIManager && ClientGameManager.UIManager.IsDragging;
			bool flag3 = ClientGameManager.UIManager && ClientGameManager.UIManager.IsUsingSlider;
			bool flag4 = flag && !flag2 && !flag3;
			CursorManager.SetCursorHidden(flag4);
			if (flag4 && this.m_dragTime > GlobalSettings.Values.Camera.CameraClickDragTimeThreshold)
			{
				CursorManager.SetCursorLockState(CursorLockMode.Locked);
			}
			bool flag5 = this.m_activeCamera.AllowMovementX();
			float xMultiplier = (flag4 && flag5) ? 1f : 0f;
			float yMultiplier = flag4 ? 1f : 0f;
			this.m_activeCamera.SetMaxSpeeds(xMultiplier, yMultiplier);
			this.m_activeCamera.UpdateVerticalOffset();
			this.m_activeCamera.ExternalUpdate();
			if (flag4)
			{
				if (this.m_activeCameraType.MatchAvatarRotationToCamera() && LocalPlayer.FollowTarget)
				{
					LocalPlayer.ClearFollowTarget();
				}
			}
			else if (GameManager.IsOnline && ClientGameManager.InputManager.LookInput != Vector2.zero)
			{
				if (this.m_activeCameraType.MatchAvatarRotationToCamera() && LocalPlayer.FollowTarget)
				{
					LocalPlayer.ClearFollowTarget();
				}
				Vector2 xydelta = CameraManager.GetXYDelta();
				if (flag5)
				{
					this.m_activeCamera.XValue += xydelta.x;
				}
				this.m_activeCamera.YValue += xydelta.y;
			}
			else if (this.m_activeCameraType != ActiveCameraTypes.Death && LocalPlayer.FollowTarget && LocalPlayer.GameEntity)
			{
				float num = Mathf.Repeat(LocalPlayer.GameEntity.gameObject.transform.eulerAngles.y + 180f, 360f) - 180f;
				float xvalue = this.m_activeCamera.XValue;
				this.m_activeCamera.XValue = (this.m_activeCameraType.MatchAvatarRotationToCamera() ? num : Mathf.MoveTowardsAngle(xvalue, num, Time.deltaTime * GlobalSettings.Values.Camera.FollowTargetCameraRecenterSpeed));
			}
			this.UpdateDragTime(flag);
			this.CameraSwap();
		}

		// Token: 0x060069E2 RID: 27106 RVA: 0x00218F04 File Offset: 0x00217104
		private void HealthStateOnChanged(HealthState obj)
		{
			if (obj != HealthState.Unconscious)
			{
				if (obj == HealthState.WakingUp)
				{
					this.SetActiveCameraBasedOnOptions();
					ICamera camera;
					if (this.m_activeCamera != null && this.m_cameras.TryGetValue(ActiveCameraTypes.Death, out camera))
					{
						if (this.m_activeCamera.Type.SetPosition())
						{
							this.m_activeCamera.SetPosition(camera.GetPosition());
						}
						this.m_activeCamera.XValue = camera.XValue;
						this.m_activeCamera.YValue = camera.YValue;
					}
				}
			}
			else
			{
				ICamera camera2;
				if (this.m_activeCamera != null && this.m_cameras.TryGetValue(ActiveCameraTypes.Death, out camera2))
				{
					if (camera2.Type.SetPosition())
					{
						camera2.SetPosition(this.m_activeCamera.GetPosition());
					}
					camera2.XValue = this.m_activeCamera.XValue;
					camera2.YValue = 1f;
				}
				this.ActiveCameraType = ActiveCameraTypes.Death;
			}
			this.RefreshCenterReticle();
		}

		// Token: 0x060069E3 RID: 27107 RVA: 0x00218FE4 File Offset: 0x002171E4
		internal void RegisterCamera(ICamera playerCamera)
		{
			if (playerCamera == null || playerCamera.Type == ActiveCameraTypes.None)
			{
				return;
			}
			if (this.m_cameras == null)
			{
				this.m_cameras = new Dictionary<ActiveCameraTypes, ICamera>(default(ActiveCameraTypeComparer));
			}
			this.m_cameras.AddOrReplace(playerCamera.Type, playerCamera);
		}

		// Token: 0x060069E4 RID: 27108 RVA: 0x00086FB4 File Offset: 0x000851B4
		private void UpdateDragTime(bool holdingMouseButton)
		{
			if (holdingMouseButton)
			{
				this.m_dragTime += Time.deltaTime;
				return;
			}
			this.m_dragTime = 0f;
		}

		// Token: 0x060069E5 RID: 27109 RVA: 0x00219030 File Offset: 0x00217230
		private void CameraSwap()
		{
			if (!GameManager.IsOnline)
			{
				if (Input.GetKeyDown(KeyCode.V))
				{
					this.ActiveCameraType = ((this.ActiveCameraType == ActiveCameraTypes.FirstPerson) ? ActiveCameraTypes.FreeLook : ActiveCameraTypes.FirstPerson);
				}
				return;
			}
			if (this.ActiveCameraType == ActiveCameraTypes.Death || ClientGameManager.InputManager == null)
			{
				return;
			}
			if (ClientGameManager.InputManager.PreventInput && ClientGameManager.InputManager.InputPreventionFlags != InputPreventionFlags.HealthState)
			{
				return;
			}
			if (SolInput.GetButtonDown(105))
			{
				Options.GameOptions.SelectedCamera.Value = ((this.ActiveCameraType == ActiveCameraTypes.FirstPerson) ? 0 : 2);
				this.SetActiveCameraBasedOnOptions();
				return;
			}
			if (SolInput.GetButtonDown(61))
			{
				Options.GameOptions.SelectedCamera.Value = ((this.ActiveCameraType == ActiveCameraTypes.OverTheShoulder) ? 0 : 1);
				this.SetActiveCameraBasedOnOptions();
			}
		}

		// Token: 0x060069E6 RID: 27110 RVA: 0x002190DC File Offset: 0x002172DC
		private void SetActiveCameraBasedOnOptions()
		{
			int value = Options.GameOptions.SelectedCamera.Value;
			if (value == 1)
			{
				this.ActiveCameraType = ActiveCameraTypes.OverTheShoulder;
				return;
			}
			if (value != 2)
			{
				this.ActiveCameraType = ActiveCameraTypes.FreeLook;
				return;
			}
			this.ActiveCameraType = ActiveCameraTypes.FirstPerson;
		}

		// Token: 0x060069E7 RID: 27111 RVA: 0x00219118 File Offset: 0x00217318
		private void SaveActiveCameraOption()
		{
			ActiveCameraTypes activeCameraType = this.ActiveCameraType;
			if (activeCameraType == ActiveCameraTypes.OverTheShoulder)
			{
				Options.GameOptions.SelectedCamera.Value = 1;
				return;
			}
			if (activeCameraType != ActiveCameraTypes.FirstPerson)
			{
				Options.GameOptions.SelectedCamera.Value = 0;
				return;
			}
			Options.GameOptions.SelectedCamera.Value = 2;
		}

		// Token: 0x060069E8 RID: 27112 RVA: 0x0004475B File Offset: 0x0004295B
		private void RefreshCenterReticle()
		{
		}

		// Token: 0x060069E9 RID: 27113 RVA: 0x00086FD7 File Offset: 0x000851D7
		internal void ZoomToFirstPerson()
		{
			this.m_lastPreZoomActiveCamera = this.m_activeCamera;
			this.ActiveCameraType = ActiveCameraTypes.FirstPerson;
			this.SaveActiveCameraOption();
		}

		// Token: 0x060069EA RID: 27114 RVA: 0x00086FF2 File Offset: 0x000851F2
		internal void ZoomOutFromFirstPerson()
		{
			this.ActiveCameraType = ((this.m_lastPreZoomActiveCamera != null) ? this.m_lastPreZoomActiveCamera.Type : ActiveCameraTypes.FreeLook);
			this.SaveActiveCameraOption();
		}

		// Token: 0x04005C20 RID: 23584
		public const int kThirdPersonCamera = 0;

		// Token: 0x04005C21 RID: 23585
		public const int kOverTheShoulderCamera = 1;

		// Token: 0x04005C22 RID: 23586
		public const int kFirstPersonCamera = 2;

		// Token: 0x04005C26 RID: 23590
		private Dictionary<ActiveCameraTypes, ICamera> m_cameras;

		// Token: 0x04005C27 RID: 23591
		private float m_dragTime;

		// Token: 0x04005C28 RID: 23592
		private MouseDragPrevention m_leftDragPrevention;

		// Token: 0x04005C29 RID: 23593
		private MouseDragPrevention m_rightDragPrevention;

		// Token: 0x04005C2A RID: 23594
		private ICamera m_activeCamera;

		// Token: 0x04005C2B RID: 23595
		private ICamera m_lastPreZoomActiveCamera;

		// Token: 0x04005C2C RID: 23596
		private ActiveCameraTypes m_activeCameraType;

		// Token: 0x04005C2D RID: 23597
		[SerializeField]
		private int m_zoomTransitionFrameCount = 2;
	}
}
