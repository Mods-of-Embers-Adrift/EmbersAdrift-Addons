using System;
using System.Collections.Generic;
using Cinemachine;
using SoL.Game;
using SoL.Game.Objects;
using SoL.Game.Settings;
using UnityEngine;

namespace SoL.GameCamera
{
	// Token: 0x02000DE3 RID: 3555
	public class CameraSettings : MonoBehaviour
	{
		// Token: 0x14000126 RID: 294
		// (add) Token: 0x060069EE RID: 27118 RVA: 0x0021915C File Offset: 0x0021735C
		// (remove) Token: 0x060069EF RID: 27119 RVA: 0x00219190 File Offset: 0x00217390
		private static event Action DisableCameraEvent;

		// Token: 0x14000127 RID: 295
		// (add) Token: 0x060069F0 RID: 27120 RVA: 0x002191C4 File Offset: 0x002173C4
		// (remove) Token: 0x060069F1 RID: 27121 RVA: 0x002191F8 File Offset: 0x002173F8
		private static event Action EnableCameraEvent;

		// Token: 0x060069F2 RID: 27122 RVA: 0x00087025 File Offset: 0x00085225
		public static void DisableCameras()
		{
			Action disableCameraEvent = CameraSettings.DisableCameraEvent;
			if (disableCameraEvent == null)
			{
				return;
			}
			disableCameraEvent();
		}

		// Token: 0x060069F3 RID: 27123 RVA: 0x00087036 File Offset: 0x00085236
		public static void EnableCameras()
		{
			Action enableCameraEvent = CameraSettings.EnableCameraEvent;
			if (enableCameraEvent == null)
			{
				return;
			}
			enableCameraEvent();
		}

		// Token: 0x14000128 RID: 296
		// (add) Token: 0x060069F4 RID: 27124 RVA: 0x0021922C File Offset: 0x0021742C
		// (remove) Token: 0x060069F5 RID: 27125 RVA: 0x00219260 File Offset: 0x00217460
		public static event Action ClipPlaneDistanceChanged;

		// Token: 0x17001941 RID: 6465
		// (get) Token: 0x060069F6 RID: 27126 RVA: 0x00087047 File Offset: 0x00085247
		// (set) Token: 0x060069F7 RID: 27127 RVA: 0x0008704E File Offset: 0x0008524E
		public static float ClipPlaneDistance
		{
			get
			{
				return CameraSettings.m_clipPlaneDistance;
			}
			set
			{
				if (Mathf.Approximately(CameraSettings.m_clipPlaneDistance, value))
				{
					return;
				}
				CameraSettings.m_clipPlaneDistance = value;
				Action clipPlaneDistanceChanged = CameraSettings.ClipPlaneDistanceChanged;
				if (clipPlaneDistanceChanged == null)
				{
					return;
				}
				clipPlaneDistanceChanged();
			}
		}

		// Token: 0x060069F8 RID: 27128 RVA: 0x00219294 File Offset: 0x00217494
		private void Start()
		{
			if (ZoneSettings.Instance != null && ZoneSettings.Instance.Profile != null)
			{
				this.m_clipPlaneRange = new MinMaxFloatRange?(ZoneSettings.Instance.Profile.GetClipRange(this.m_lensSettings.FarClipPlane));
				this.m_lensSettings.FarClipPlane = this.m_clipPlaneRange.Value.Max;
			}
			for (int i = 0; i < this.m_virtualCameras.Length; i++)
			{
				this.m_virtualCameras[i].m_Lens = this.m_lensSettings;
			}
			bool flag = false;
			this.m_cinemachineColliders = new List<CinemachineCollider>(this.m_freeLookCameras.Length);
			for (int j = 0; j < this.m_freeLookCameras.Length; j++)
			{
				this.m_freeLookCameras[j].m_CommonLens = true;
				this.m_freeLookCameras[j].m_Lens = this.m_lensSettings;
				CinemachineCollider component = this.m_freeLookCameras[j].gameObject.GetComponent<CinemachineCollider>();
				if (component)
				{
					this.m_cinemachineColliders.Add(component);
					if (!flag)
					{
						this.m_defaultSmoothingTime = component.m_SmoothingTime;
						this.m_defaultDamping = component.m_Damping;
						this.m_defaultDampingWhenOccluded = component.m_DampingWhenOccluded;
						flag = true;
					}
				}
			}
			this.ViewDistanceOnChanged();
			Options.VideoOptions.ViewDistance.Changed += this.ViewDistanceOnChanged;
			this.CameraFieldOfViewOnChanged();
			Options.VideoOptions.CameraFieldOfView.Changed += this.CameraFieldOfViewOnChanged;
			this.EnableCameraDampingOnChanged();
			Options.GameOptions.EnableCameraDamping.Changed += this.EnableCameraDampingOnChanged;
			CameraSettings.EnableCameraEvent += this.OnEnableCameraEvent;
			CameraSettings.DisableCameraEvent += this.OnDisableCameraEvent;
		}

		// Token: 0x060069F9 RID: 27129 RVA: 0x00219444 File Offset: 0x00217644
		private void OnDestroy()
		{
			Options.VideoOptions.ViewDistance.Changed -= this.ViewDistanceOnChanged;
			Options.GameOptions.EnableCameraDamping.Changed -= this.EnableCameraDampingOnChanged;
			CameraSettings.EnableCameraEvent -= this.OnEnableCameraEvent;
			CameraSettings.DisableCameraEvent -= this.OnDisableCameraEvent;
		}

		// Token: 0x060069FA RID: 27130 RVA: 0x002194A0 File Offset: 0x002176A0
		private void ViewDistanceOnChanged()
		{
			float a = 250f;
			float b = this.m_lensSettings.FarClipPlane;
			if (this.m_clipPlaneRange != null)
			{
				a = this.m_clipPlaneRange.Value.Min;
				b = this.m_clipPlaneRange.Value.Max;
			}
			float num = Mathf.Lerp(a, b, Options.VideoOptions.ViewDistance.Value);
			CameraSettings.ClipPlaneDistance = num;
			for (int i = 0; i < this.m_freeLookCameras.Length; i++)
			{
				this.m_freeLookCameras[i].m_Lens.FarClipPlane = num;
			}
			for (int j = 0; j < this.m_virtualCameras.Length; j++)
			{
				this.m_virtualCameras[j].m_Lens.FarClipPlane = num;
			}
		}

		// Token: 0x060069FB RID: 27131 RVA: 0x00219564 File Offset: 0x00217764
		private void CameraFieldOfViewOnChanged()
		{
			int value = Options.VideoOptions.CameraFieldOfView.Value;
			for (int i = 0; i < this.m_freeLookCameras.Length; i++)
			{
				this.m_freeLookCameras[i].m_Lens.FieldOfView = (float)value;
			}
			for (int j = 0; j < this.m_virtualCameras.Length; j++)
			{
				this.m_virtualCameras[j].m_Lens.FieldOfView = (float)value;
			}
		}

		// Token: 0x060069FC RID: 27132 RVA: 0x002195CC File Offset: 0x002177CC
		private void EnableCameraDampingOnChanged()
		{
			float num = GlobalSettings.Values.Camera.BodyAxisDamping;
			float horizontalDamping = GlobalSettings.Values.Camera.AimHorizontalDamping;
			float verticalDamping = GlobalSettings.Values.Camera.AimVerticalDamping;
			if (!Options.GameOptions.EnableCameraDamping.Value)
			{
				num = 0f;
				horizontalDamping = 0f;
				verticalDamping = 0f;
			}
			for (int i = 0; i < this.m_freeLookCameras.Length; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					CinemachineVirtualCamera rig = this.m_freeLookCameras[i].GetRig(j);
					if (rig != null)
					{
						CinemachineTransposer cinemachineComponent = rig.GetCinemachineComponent<CinemachineTransposer>();
						if (cinemachineComponent != null)
						{
							cinemachineComponent.m_XDamping = num;
							cinemachineComponent.m_YDamping = num;
							cinemachineComponent.m_ZDamping = num;
						}
						CinemachineComposer cinemachineComponent2 = rig.GetCinemachineComponent<CinemachineComposer>();
						if (cinemachineComponent2 != null)
						{
							cinemachineComponent2.m_HorizontalDamping = horizontalDamping;
							cinemachineComponent2.m_VerticalDamping = verticalDamping;
						}
					}
				}
			}
		}

		// Token: 0x060069FD RID: 27133 RVA: 0x002196B8 File Offset: 0x002178B8
		private void DisableCameraCollisionSmoothingOnChanged()
		{
			for (int i = 0; i < this.m_cinemachineColliders.Count; i++)
			{
				if (this.m_cinemachineColliders[i])
				{
					if (Options.GameOptions.DisableCameraCollisionSmoothing.Value)
					{
						this.m_cinemachineColliders[i].m_SmoothingTime = 0f;
						this.m_cinemachineColliders[i].m_Damping = 0f;
						this.m_cinemachineColliders[i].m_DampingWhenOccluded = 0f;
					}
					else
					{
						this.m_cinemachineColliders[i].m_SmoothingTime = this.m_defaultSmoothingTime;
						this.m_cinemachineColliders[i].m_Damping = this.m_defaultDamping;
						this.m_cinemachineColliders[i].m_DampingWhenOccluded = this.m_defaultDampingWhenOccluded;
					}
				}
			}
		}

		// Token: 0x060069FE RID: 27134 RVA: 0x00087073 File Offset: 0x00085273
		private void OnDisableCameraEvent()
		{
			this.ToggleCameras(false);
		}

		// Token: 0x060069FF RID: 27135 RVA: 0x0008707C File Offset: 0x0008527C
		private void OnEnableCameraEvent()
		{
			this.ToggleCameras(true);
		}

		// Token: 0x06006A00 RID: 27136 RVA: 0x0021978C File Offset: 0x0021798C
		private void ToggleCameras(bool isEnabled)
		{
			for (int i = 0; i < this.m_freeLookCameras.Length; i++)
			{
				if (this.m_freeLookCameras[i])
				{
					this.m_freeLookCameras[i].enabled = isEnabled;
				}
			}
			for (int j = 0; j < this.m_virtualCameras.Length; j++)
			{
				if (this.m_virtualCameras[j])
				{
					this.m_virtualCameras[j].enabled = isEnabled;
				}
			}
		}

		// Token: 0x04005C2E RID: 23598
		public const float kDefaultMinClipPlane = 250f;

		// Token: 0x04005C2F RID: 23599
		public const float kDefaultMaxClipPlane = 1000f;

		// Token: 0x04005C33 RID: 23603
		private static float m_clipPlaneDistance;

		// Token: 0x04005C34 RID: 23604
		[SerializeField]
		private CinemachineFreeLook[] m_freeLookCameras;

		// Token: 0x04005C35 RID: 23605
		[SerializeField]
		private CinemachineVirtualCamera[] m_virtualCameras;

		// Token: 0x04005C36 RID: 23606
		[SerializeField]
		private LensSettings m_lensSettings = LensSettings.Default;

		// Token: 0x04005C37 RID: 23607
		private MinMaxFloatRange? m_clipPlaneRange;

		// Token: 0x04005C38 RID: 23608
		private List<CinemachineCollider> m_cinemachineColliders;

		// Token: 0x04005C39 RID: 23609
		private float m_defaultSmoothingTime = 0.1f;

		// Token: 0x04005C3A RID: 23610
		private float m_defaultDamping = 1f;

		// Token: 0x04005C3B RID: 23611
		private float m_defaultDampingWhenOccluded = 0.3f;
	}
}
