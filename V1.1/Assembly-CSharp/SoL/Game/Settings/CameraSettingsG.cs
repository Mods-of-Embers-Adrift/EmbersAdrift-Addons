using System;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x0200071E RID: 1822
	[Serializable]
	public class CameraSettingsG
	{
		// Token: 0x17000C27 RID: 3111
		// (get) Token: 0x060036C4 RID: 14020 RVA: 0x000657CA File Offset: 0x000639CA
		public float LookInputSpeedX
		{
			get
			{
				return this.m_lookInputSpeed.x;
			}
		}

		// Token: 0x17000C28 RID: 3112
		// (get) Token: 0x060036C5 RID: 14021 RVA: 0x000657D7 File Offset: 0x000639D7
		public float LookInputSpeedY
		{
			get
			{
				return this.m_lookInputSpeed.y;
			}
		}

		// Token: 0x17000C29 RID: 3113
		// (get) Token: 0x060036C6 RID: 14022 RVA: 0x000657E4 File Offset: 0x000639E4
		public float BodyAxisDamping
		{
			get
			{
				return this.m_bodyAxisDamping;
			}
		}

		// Token: 0x17000C2A RID: 3114
		// (get) Token: 0x060036C7 RID: 14023 RVA: 0x000657EC File Offset: 0x000639EC
		public float AimHorizontalDamping
		{
			get
			{
				return this.m_aimHorizontalDamping;
			}
		}

		// Token: 0x17000C2B RID: 3115
		// (get) Token: 0x060036C8 RID: 14024 RVA: 0x000657F4 File Offset: 0x000639F4
		public float AimVerticalDamping
		{
			get
			{
				return this.m_aimVerticalDamping;
			}
		}

		// Token: 0x17000C2C RID: 3116
		// (get) Token: 0x060036C9 RID: 14025 RVA: 0x000657FC File Offset: 0x000639FC
		public float FollowTargetCameraRecenterSpeed
		{
			get
			{
				return this.m_followTargetCameraRecenterSpeed;
			}
		}

		// Token: 0x040034E9 RID: 13545
		public float CameraClickDragTimeThreshold = 0.3f;

		// Token: 0x040034EA RID: 13546
		private const string kDampingGroup = "Damping";

		// Token: 0x040034EB RID: 13547
		[SerializeField]
		private float m_bodyAxisDamping = 0.3f;

		// Token: 0x040034EC RID: 13548
		[SerializeField]
		private float m_aimHorizontalDamping = 0.2f;

		// Token: 0x040034ED RID: 13549
		[SerializeField]
		private float m_aimVerticalDamping = 1f;

		// Token: 0x040034EE RID: 13550
		[SerializeField]
		private Vector2 m_lookInputSpeed = new Vector2(4f, 0.04f);

		// Token: 0x040034EF RID: 13551
		[SerializeField]
		private float m_followTargetCameraRecenterSpeed = 80f;
	}
}
