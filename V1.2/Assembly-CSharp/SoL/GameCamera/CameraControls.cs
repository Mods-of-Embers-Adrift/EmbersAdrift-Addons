using System;
using Cinemachine;
using SoL.Game;
using SoL.Game.Settings;
using SoL.Managers;
using UnityEngine;

namespace SoL.GameCamera
{
	// Token: 0x02000DDE RID: 3550
	[Obsolete]
	public class CameraControls : GameEntityComponent
	{
		// Token: 0x17001938 RID: 6456
		// (get) Token: 0x060069BB RID: 27067 RVA: 0x00086E7B File Offset: 0x0008507B
		// (set) Token: 0x060069BC RID: 27068 RVA: 0x00086E82 File Offset: 0x00085082
		public static ActiveCameraTypes ActiveType { get; private set; } = ActiveCameraTypes.None;

		// Token: 0x17001939 RID: 6457
		// (get) Token: 0x060069BD RID: 27069 RVA: 0x00086E8A File Offset: 0x0008508A
		// (set) Token: 0x060069BE RID: 27070 RVA: 0x00218048 File Offset: 0x00216248
		private ActiveCameraTypes ActiveCameraType
		{
			get
			{
				return this.m_activeCameraType;
			}
			set
			{
				if (value == this.m_activeCameraType)
				{
					return;
				}
				this.m_activeCameraType = value;
				CameraControls.ActiveType = this.m_activeCameraType;
				switch (this.m_activeCameraType)
				{
				case ActiveCameraTypes.FreeLook:
					this.m_freeLook.m_XAxis.Value = this.m_focus.eulerAngles.y;
					this.m_freeLook.gameObject.SetActive(true);
					this.m_overTheShoulder.gameObject.SetActive(false);
					this.m_deathFreeLook.gameObject.SetActive(false);
					this.m_activeCamera = this.m_freeLook;
					break;
				case ActiveCameraTypes.OverTheShoulder:
					this.m_overTheShoulder.m_XAxis.Value = this.m_focus.eulerAngles.y;
					this.m_freeLook.gameObject.SetActive(false);
					this.m_overTheShoulder.gameObject.SetActive(true);
					this.m_deathFreeLook.gameObject.SetActive(false);
					this.m_activeCamera = this.m_overTheShoulder;
					break;
				case ActiveCameraTypes.Death:
					if (this.m_useDeathFreeLook)
					{
						this.m_deathFreeLook.gameObject.SetActive(true);
						this.m_activeCamera = this.m_deathFreeLook;
					}
					break;
				}
				this.RefreshCenterReticle();
			}
		}

		// Token: 0x060069BF RID: 27071 RVA: 0x00086E92 File Offset: 0x00085092
		private void Awake()
		{
			CameraControls.FreeLookCameraName = this.m_freeLook.gameObject.name;
			CameraControls.OverTheShoulderCameraName = this.m_overTheShoulder.gameObject.name;
			PlayerMotorController.ResetCameraRotation += this.ResetCameraXRotation;
		}

		// Token: 0x060069C0 RID: 27072 RVA: 0x00218184 File Offset: 0x00216384
		private void Start()
		{
			this.m_focus = (GameManager.IsOffline ? base.gameObject.transform.root : base.GameEntity.gameObject.transform);
			Options.GameOptions.InvertMouse.Changed += this.InvertMouseOnChanged;
			Options.GameOptions.XMouseSensitivity.Changed += this.XMouseSensitivityOnChanged;
			Options.GameOptions.YMouseSensitivity.Changed += this.YMouseSensitivityOnChanged;
			Options.GameOptions.OverTheShoulderCameraActive.Changed += this.OverTheShoulderCameraActiveOnChanged;
			this.OverTheShoulderCameraActiveOnChanged();
			this.XMouseSensitivityOnChanged();
			this.YMouseSensitivityOnChanged();
			this.InvertMouseOnChanged();
			this.SetupFreeLookParameters(this.m_freeLook);
			this.SetupFreeLookParameters(this.m_overTheShoulder);
			this.SetupFreeLookParameters(this.m_deathFreeLook);
			this.ResetCameraXRotation();
			this.ResetCameraYRotation();
			this.m_freeLook.Priority = 10;
			this.m_overTheShoulder.Priority = 11;
			this.m_death.gameObject.SetActive(false);
			this.m_death.Priority = 12;
			this.m_death.gameObject.transform.localPosition = new Vector3(0f, 10f, 0f);
			this.m_death.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
			this.m_deathFreeLook.gameObject.SetActive(false);
			this.m_deathFreeLook.Priority = 12;
			if (LocalPlayer.GameEntity != null && LocalPlayer.GameEntity.VitalsReplicator)
			{
				LocalPlayer.GameEntity.VitalsReplicator.CurrentHealthState.Changed += this.HealthStateOnChanged;
			}
			this.m_leftDragPrevention = new MouseDragPrevention(0);
			this.m_rightDragPrevention = new MouseDragPrevention(1);
		}

		// Token: 0x060069C1 RID: 27073 RVA: 0x00218364 File Offset: 0x00216564
		private void OnDestroy()
		{
			Options.GameOptions.InvertMouse.Changed -= this.InvertMouseOnChanged;
			Options.GameOptions.XMouseSensitivity.Changed -= this.XMouseSensitivityOnChanged;
			Options.GameOptions.YMouseSensitivity.Changed -= this.YMouseSensitivityOnChanged;
			Options.GameOptions.OverTheShoulderCameraActive.Changed -= this.OverTheShoulderCameraActiveOnChanged;
			PlayerMotorController.ResetCameraRotation -= this.ResetCameraXRotation;
			if (LocalPlayer.GameEntity != null && LocalPlayer.GameEntity.VitalsReplicator)
			{
				LocalPlayer.GameEntity.VitalsReplicator.CurrentHealthState.Changed -= this.HealthStateOnChanged;
			}
		}

		// Token: 0x060069C2 RID: 27074 RVA: 0x00218418 File Offset: 0x00216618
		private void Update()
		{
			if (ClientGameManager.InputManager == null)
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
			bool flag5 = this.AllowMovementX();
			this.m_activeCamera.m_XAxis.m_MaxSpeed = ((flag4 && flag5) ? (this.m_xSpeed * this.m_xSpeedMod) : 0f);
			this.m_activeCamera.m_YAxis.m_MaxSpeed = (flag4 ? (this.m_ySpeed * this.m_ySpeedMod) : 0f);
			if (!flag4 && GameManager.IsOnline && ClientGameManager.InputManager.LookInput != Vector2.zero)
			{
				Vector2 xydelta = CameraManager.GetXYDelta();
				if (flag5)
				{
					CinemachineFreeLook activeCamera = this.m_activeCamera;
					activeCamera.m_XAxis.Value = activeCamera.m_XAxis.Value + xydelta.x;
				}
				CinemachineFreeLook activeCamera2 = this.m_activeCamera;
				activeCamera2.m_YAxis.Value = activeCamera2.m_YAxis.Value + xydelta.y;
			}
			this.UpdateDragTime(flag);
			this.CameraSwap();
		}

		// Token: 0x060069C3 RID: 27075 RVA: 0x002185AC File Offset: 0x002167AC
		private void SetupFreeLookParameters(CinemachineFreeLook freeLook)
		{
			freeLook.m_XAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;
			freeLook.m_XAxis.m_AccelTime = this.m_accelerationTime;
			freeLook.m_XAxis.m_DecelTime = this.m_decelerationTime;
			freeLook.m_YAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;
			freeLook.m_YAxis.m_AccelTime = this.m_accelerationTime;
			freeLook.m_YAxis.m_DecelTime = this.m_decelerationTime;
		}

		// Token: 0x060069C4 RID: 27076 RVA: 0x00218618 File Offset: 0x00216818
		public void ResetCameraXRotation()
		{
			if (this.m_focus)
			{
				if (this.m_freeLook)
				{
					this.m_freeLook.m_XAxis.Value = this.m_focus.eulerAngles.y;
				}
				if (this.m_overTheShoulder)
				{
					this.m_overTheShoulder.m_XAxis.Value = this.m_focus.eulerAngles.y;
				}
			}
		}

		// Token: 0x060069C5 RID: 27077 RVA: 0x0021868C File Offset: 0x0021688C
		private void ResetCameraYRotation()
		{
			if (this.m_freeLook)
			{
				this.m_freeLook.m_YAxis.Value = 0.5f;
			}
			if (this.m_overTheShoulder)
			{
				this.m_overTheShoulder.m_YAxis.Value = 0.5f;
			}
		}

		// Token: 0x060069C6 RID: 27078 RVA: 0x002186E0 File Offset: 0x002168E0
		private void CameraSwap()
		{
			if (this.ActiveCameraType == ActiveCameraTypes.Death || ClientGameManager.InputManager == null || ClientGameManager.InputManager.PreventInput)
			{
				return;
			}
			if (GameManager.IsOnline ? SolInput.GetButtonDown(61) : Input.GetKeyDown(KeyCode.V))
			{
				Options.GameOptions.OverTheShoulderCameraActive.Value = !Options.GameOptions.OverTheShoulderCameraActive.Value;
			}
		}

		// Token: 0x060069C7 RID: 27079 RVA: 0x0021873C File Offset: 0x0021693C
		private bool AllowMovementX()
		{
			if (GameManager.IsOnline && (!LocalPlayer.GameEntity || !LocalPlayer.GameEntity.Vitals))
			{
				return false;
			}
			switch (this.ActiveCameraType)
			{
			case ActiveCameraTypes.FreeLook:
				return true;
			case ActiveCameraTypes.OverTheShoulder:
			{
				Stance currentStance = LocalPlayer.GameEntity.Vitals.GetCurrentStance();
				return currentStance != Stance.Sit && currentStance - Stance.Unconscious > 1;
			}
			case ActiveCameraTypes.Death:
				return true;
			default:
				return false;
			}
		}

		// Token: 0x060069C8 RID: 27080 RVA: 0x00086ECF File Offset: 0x000850CF
		private void UpdateDragTime(bool holdingMB)
		{
			if (holdingMB)
			{
				this.m_dragTime += Time.deltaTime;
				return;
			}
			this.m_dragTime = 0f;
		}

		// Token: 0x060069C9 RID: 27081 RVA: 0x002187B0 File Offset: 0x002169B0
		private void InvertMouseOnChanged()
		{
			bool invertInput = !Options.GameOptions.InvertMouse.Value;
			this.m_freeLook.m_YAxis.m_InvertInput = invertInput;
			this.m_overTheShoulder.m_YAxis.m_InvertInput = invertInput;
			this.m_deathFreeLook.m_YAxis.m_InvertInput = invertInput;
		}

		// Token: 0x060069CA RID: 27082 RVA: 0x00086EF2 File Offset: 0x000850F2
		private void YMouseSensitivityOnChanged()
		{
			this.m_ySpeedMod = Options.GameOptions.YMouseSensitivity;
		}

		// Token: 0x060069CB RID: 27083 RVA: 0x00086F04 File Offset: 0x00085104
		private void XMouseSensitivityOnChanged()
		{
			this.m_xSpeedMod = Options.GameOptions.XMouseSensitivity;
		}

		// Token: 0x060069CC RID: 27084 RVA: 0x00086F16 File Offset: 0x00085116
		private void OverTheShoulderCameraActiveOnChanged()
		{
			this.ActiveCameraType = (Options.GameOptions.OverTheShoulderCameraActive.Value ? ActiveCameraTypes.OverTheShoulder : ActiveCameraTypes.FreeLook);
		}

		// Token: 0x060069CD RID: 27085 RVA: 0x00218800 File Offset: 0x00216A00
		private void HealthStateOnChanged(HealthState obj)
		{
			if (obj != HealthState.Unconscious)
			{
				if (obj == HealthState.WakingUp)
				{
					if (this.m_useDeathFreeLook)
					{
						this.ActiveCameraType = (Options.GameOptions.OverTheShoulderCameraActive.Value ? ActiveCameraTypes.OverTheShoulder : ActiveCameraTypes.FreeLook);
						this.m_activeCamera.gameObject.transform.position = this.m_deathFreeLook.gameObject.transform.position;
						this.m_activeCamera.m_XAxis.Value = this.m_deathFreeLook.m_XAxis.Value;
						this.m_activeCamera.m_YAxis.Value = this.m_deathFreeLook.m_YAxis.Value;
					}
					else
					{
						this.m_death.gameObject.SetActive(false);
					}
				}
			}
			else if (this.m_useDeathFreeLook)
			{
				this.m_deathFreeLook.gameObject.transform.position = this.m_activeCamera.gameObject.transform.position;
				this.m_deathFreeLook.m_XAxis.Value = this.m_activeCamera.m_XAxis.Value;
				this.m_deathFreeLook.m_YAxis.Value = 1f;
				this.ActiveCameraType = ActiveCameraTypes.Death;
			}
			else
			{
				this.m_death.gameObject.SetActive(true);
			}
			this.RefreshCenterReticle();
		}

		// Token: 0x060069CE RID: 27086 RVA: 0x0004475B File Offset: 0x0004295B
		private void RefreshCenterReticle()
		{
		}

		// Token: 0x04005C07 RID: 23559
		[SerializeField]
		private CinemachineFreeLook m_freeLook;

		// Token: 0x04005C08 RID: 23560
		[SerializeField]
		private CinemachineFreeLook m_overTheShoulder;

		// Token: 0x04005C09 RID: 23561
		[SerializeField]
		private bool m_useDeathFreeLook;

		// Token: 0x04005C0A RID: 23562
		[SerializeField]
		private CinemachineVirtualCamera m_death;

		// Token: 0x04005C0B RID: 23563
		[SerializeField]
		private CinemachineFreeLook m_deathFreeLook;

		// Token: 0x04005C0C RID: 23564
		[SerializeField]
		private float m_xSpeed = 8f;

		// Token: 0x04005C0D RID: 23565
		[SerializeField]
		private float m_ySpeed = 0.1f;

		// Token: 0x04005C0E RID: 23566
		[SerializeField]
		private float m_accelerationTime = 0.1f;

		// Token: 0x04005C0F RID: 23567
		[SerializeField]
		private float m_decelerationTime = 0.1f;

		// Token: 0x04005C10 RID: 23568
		private float m_xSpeedMod = 1f;

		// Token: 0x04005C11 RID: 23569
		private float m_ySpeedMod = 1f;

		// Token: 0x04005C12 RID: 23570
		private float m_dragTime;

		// Token: 0x04005C13 RID: 23571
		private MouseDragPrevention m_leftDragPrevention;

		// Token: 0x04005C14 RID: 23572
		private MouseDragPrevention m_rightDragPrevention;

		// Token: 0x04005C15 RID: 23573
		public static string FreeLookCameraName = string.Empty;

		// Token: 0x04005C16 RID: 23574
		public static string OverTheShoulderCameraName = string.Empty;

		// Token: 0x04005C17 RID: 23575
		private Transform m_focus;

		// Token: 0x04005C18 RID: 23576
		private CinemachineFreeLook m_activeCamera;

		// Token: 0x04005C19 RID: 23577
		private ActiveCameraTypes m_activeCameraType;
	}
}
