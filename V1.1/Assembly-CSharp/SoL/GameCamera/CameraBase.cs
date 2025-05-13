using System;
using Cinemachine;
using SoL.Game;
using UnityEngine;

namespace SoL.GameCamera
{
	// Token: 0x02000DDD RID: 3549
	public abstract class CameraBase<T> : MonoBehaviour, ICamera where T : CinemachineVirtualCameraBase
	{
		// Token: 0x0600699B RID: 27035
		protected abstract void InitializeCamera();

		// Token: 0x0600699C RID: 27036
		protected abstract void InvertMouseOnChanged();

		// Token: 0x0600699D RID: 27037
		protected abstract bool AllowMovementX();

		// Token: 0x17001932 RID: 6450
		// (get) Token: 0x0600699E RID: 27038
		// (set) Token: 0x0600699F RID: 27039
		protected abstract float XValue { get; set; }

		// Token: 0x17001933 RID: 6451
		// (get) Token: 0x060069A0 RID: 27040
		// (set) Token: 0x060069A1 RID: 27041
		protected abstract float YValue { get; set; }

		// Token: 0x060069A2 RID: 27042
		protected abstract void SetMaxSpeeds(float xMultiplier, float yMultiplier);

		// Token: 0x060069A3 RID: 27043
		protected abstract void SetXValueOnActive();

		// Token: 0x060069A4 RID: 27044
		protected abstract void UpdateVerticalOffset();

		// Token: 0x060069A5 RID: 27045 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void UpdateExternal()
		{
		}

		// Token: 0x060069A6 RID: 27046 RVA: 0x00217E74 File Offset: 0x00216074
		protected bool TryGetStanceOffset(out float offset)
		{
			offset = 0f;
			return LocalPlayer.GameEntity && LocalPlayer.GameEntity.Vitals && this.m_settings.TryGetStanceOffset(LocalPlayer.GameEntity.Vitals.GetCurrentStance(), out offset);
		}

		// Token: 0x060069A7 RID: 27047 RVA: 0x00217EC4 File Offset: 0x002160C4
		protected virtual void Awake()
		{
			if (this.m_manager == null || this.m_camera == null)
			{
				base.gameObject.SetActive(false);
				return;
			}
			this.m_manager.RegisterCamera(this);
			this.m_manager.ActiveCameraChanged += this.ManagerOnActiveCameraChanged;
			Options.GameOptions.InvertMouse.Changed += this.InvertMouseOnChanged;
			PlayerMotorController.ResetCameraRotation += this.ResetXRotation;
		}

		// Token: 0x060069A8 RID: 27048 RVA: 0x00086D5A File Offset: 0x00084F5A
		private void Start()
		{
			this.InitializeCamera();
			this.InvertMouseOnChanged();
			this.ResetXRotation();
			this.ResetYRotation();
		}

		// Token: 0x060069A9 RID: 27049 RVA: 0x00217F4C File Offset: 0x0021614C
		private void OnDestroy()
		{
			this.m_manager.ActiveCameraChanged -= this.ManagerOnActiveCameraChanged;
			Options.GameOptions.InvertMouse.Changed -= this.InvertMouseOnChanged;
			PlayerMotorController.ResetCameraRotation -= this.ResetXRotation;
		}

		// Token: 0x060069AA RID: 27050 RVA: 0x00217F98 File Offset: 0x00216198
		private void ManagerOnActiveCameraChanged()
		{
			bool flag = this.m_manager && this.m_manager.ActiveCameraType == this.m_type;
			if (flag)
			{
				this.SetXValueOnActive();
			}
			this.m_camera.gameObject.SetActive(flag);
		}

		// Token: 0x060069AB RID: 27051 RVA: 0x00086D74 File Offset: 0x00084F74
		private void ResetXRotation()
		{
			if (this.m_manager && this.m_manager.Focus)
			{
				this.XValue = this.m_manager.Focus.eulerAngles.y;
			}
		}

		// Token: 0x060069AC RID: 27052 RVA: 0x00086DB0 File Offset: 0x00084FB0
		private void ResetYRotation()
		{
			this.YValue = 0.5f;
		}

		// Token: 0x17001934 RID: 6452
		// (get) Token: 0x060069AD RID: 27053 RVA: 0x00086DBD File Offset: 0x00084FBD
		ActiveCameraTypes ICamera.Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x17001935 RID: 6453
		// (get) Token: 0x060069AE RID: 27054 RVA: 0x00086DC5 File Offset: 0x00084FC5
		// (set) Token: 0x060069AF RID: 27055 RVA: 0x00086DCD File Offset: 0x00084FCD
		float ICamera.XValue
		{
			get
			{
				return this.XValue;
			}
			set
			{
				this.XValue = value;
			}
		}

		// Token: 0x17001936 RID: 6454
		// (get) Token: 0x060069B0 RID: 27056 RVA: 0x00086DD6 File Offset: 0x00084FD6
		// (set) Token: 0x060069B1 RID: 27057 RVA: 0x00086DDE File Offset: 0x00084FDE
		float ICamera.YValue
		{
			get
			{
				return this.YValue;
			}
			set
			{
				this.YValue = value;
			}
		}

		// Token: 0x060069B2 RID: 27058 RVA: 0x00086DE7 File Offset: 0x00084FE7
		bool ICamera.AllowMovementX()
		{
			return this.AllowMovementX();
		}

		// Token: 0x060069B3 RID: 27059 RVA: 0x00086DEF File Offset: 0x00084FEF
		void ICamera.SetMaxSpeeds(float xMultiplier, float yMultiplier)
		{
			this.SetMaxSpeeds(xMultiplier, yMultiplier);
		}

		// Token: 0x060069B4 RID: 27060 RVA: 0x00086DF9 File Offset: 0x00084FF9
		void ICamera.SetPosition(Vector3 position)
		{
			this.m_camera.gameObject.transform.position = position;
		}

		// Token: 0x060069B5 RID: 27061 RVA: 0x00086E16 File Offset: 0x00085016
		Vector3 ICamera.GetPosition()
		{
			return this.m_camera.gameObject.transform.position;
		}

		// Token: 0x17001937 RID: 6455
		// (get) Token: 0x060069B6 RID: 27062 RVA: 0x00086E32 File Offset: 0x00085032
		string ICamera.CameraName
		{
			get
			{
				return this.m_camera.gameObject.name;
			}
		}

		// Token: 0x060069B7 RID: 27063 RVA: 0x00086E49 File Offset: 0x00085049
		void ICamera.ExternalDestroy()
		{
			if (this && base.gameObject)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x060069B8 RID: 27064 RVA: 0x00086E6B File Offset: 0x0008506B
		void ICamera.UpdateVerticalOffset()
		{
			this.UpdateVerticalOffset();
		}

		// Token: 0x060069B9 RID: 27065 RVA: 0x00086E73 File Offset: 0x00085073
		void ICamera.ExternalUpdate()
		{
			this.UpdateExternal();
		}

		// Token: 0x04005C02 RID: 23554
		[SerializeField]
		protected CameraManager m_manager;

		// Token: 0x04005C03 RID: 23555
		[SerializeField]
		protected ActiveCameraTypes m_type;

		// Token: 0x04005C04 RID: 23556
		[SerializeField]
		protected T m_camera;

		// Token: 0x04005C05 RID: 23557
		[SerializeField]
		protected CameraInitializationSettings m_settings;
	}
}
