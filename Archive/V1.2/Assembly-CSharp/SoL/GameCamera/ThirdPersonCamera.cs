using System;
using Cinemachine;
using SoL.Game;
using SoL.Managers;

namespace SoL.GameCamera
{
	// Token: 0x02000DEE RID: 3566
	public class ThirdPersonCamera : CameraBase<CinemachineFreeLook>
	{
		// Token: 0x06006A48 RID: 27208 RVA: 0x0021A844 File Offset: 0x00218A44
		protected override void Awake()
		{
			this.m_composers = new CinemachineComposer[3];
			for (int i = 0; i < 3; i++)
			{
				this.m_composers[i] = this.m_camera.GetRig(i).GetCinemachineComponent<CinemachineComposer>();
				if (i == 1)
				{
					this.m_defaultOffset = this.m_composers[i].m_TrackedObjectOffset.y;
					this.m_currentOffset = this.m_defaultOffset;
				}
			}
			base.Awake();
		}

		// Token: 0x06006A49 RID: 27209 RVA: 0x0021A8B0 File Offset: 0x00218AB0
		protected override void InitializeCamera()
		{
			this.m_camera.Priority = this.m_type.GetPriorityForType();
			this.m_camera.m_XAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;
			this.m_camera.m_XAxis.m_AccelTime = this.m_settings.AccelerationTime;
			this.m_camera.m_XAxis.m_DecelTime = this.m_settings.DecelerationTime;
			this.m_camera.m_XAxis.m_MaxSpeed = 0f;
			this.m_camera.m_YAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;
			this.m_camera.m_YAxis.m_AccelTime = this.m_settings.AccelerationTime;
			this.m_camera.m_YAxis.m_DecelTime = this.m_settings.DecelerationTime;
			this.m_camera.m_YAxis.m_MaxSpeed = 0f;
		}

		// Token: 0x06006A4A RID: 27210 RVA: 0x000873ED File Offset: 0x000855ED
		protected override void InvertMouseOnChanged()
		{
			this.m_camera.m_YAxis.m_InvertInput = !Options.GameOptions.InvertMouse.Value;
		}

		// Token: 0x1700194F RID: 6479
		// (get) Token: 0x06006A4B RID: 27211 RVA: 0x0008740C File Offset: 0x0008560C
		// (set) Token: 0x06006A4C RID: 27212 RVA: 0x0008741E File Offset: 0x0008561E
		protected override float XValue
		{
			get
			{
				return this.m_camera.m_XAxis.Value;
			}
			set
			{
				this.m_camera.m_XAxis.Value = value;
			}
		}

		// Token: 0x17001950 RID: 6480
		// (get) Token: 0x06006A4D RID: 27213 RVA: 0x00087431 File Offset: 0x00085631
		// (set) Token: 0x06006A4E RID: 27214 RVA: 0x00087443 File Offset: 0x00085643
		protected override float YValue
		{
			get
			{
				return this.m_camera.m_YAxis.Value;
			}
			set
			{
				this.m_camera.m_YAxis.Value = value;
			}
		}

		// Token: 0x06006A4F RID: 27215 RVA: 0x0021A98C File Offset: 0x00218B8C
		protected override bool AllowMovementX()
		{
			if (GameManager.IsOnline && (!LocalPlayer.GameEntity || !LocalPlayer.GameEntity.Vitals))
			{
				return false;
			}
			switch (this.m_type)
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

		// Token: 0x06006A50 RID: 27216 RVA: 0x0021AA00 File Offset: 0x00218C00
		protected override void SetMaxSpeeds(float xMultiplier, float yMultiplier)
		{
			this.m_camera.m_XAxis.m_MaxSpeed = xMultiplier * this.m_settings.XSpeed * Options.GameOptions.XMouseSensitivity.Value;
			this.m_camera.m_YAxis.m_MaxSpeed = yMultiplier * this.m_settings.YSpeed * Options.GameOptions.YMouseSensitivity.Value;
		}

		// Token: 0x06006A51 RID: 27217 RVA: 0x00087456 File Offset: 0x00085656
		protected override void SetXValueOnActive()
		{
			if (this.m_type.SetAngleOnActive() && this.m_manager)
			{
				this.XValue = this.m_manager.Focus.eulerAngles.y;
			}
		}

		// Token: 0x06006A52 RID: 27218 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void UpdateVerticalOffset()
		{
		}

		// Token: 0x04005C7A RID: 23674
		private CinemachineComposer[] m_composers;

		// Token: 0x04005C7B RID: 23675
		private float m_defaultOffset;

		// Token: 0x04005C7C RID: 23676
		private float m_currentOffset;
	}
}
