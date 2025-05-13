using System;

namespace SoL.GameCamera
{
	// Token: 0x02000DEA RID: 3562
	public static class ActiveCameraTypeExtension
	{
		// Token: 0x06006A31 RID: 27185 RVA: 0x00087233 File Offset: 0x00085433
		public static int GetPriorityForType(this ActiveCameraTypes type)
		{
			switch (type)
			{
			case ActiveCameraTypes.FreeLook:
				return 10;
			case ActiveCameraTypes.OverTheShoulder:
				return 12;
			case ActiveCameraTypes.Death:
				return 13;
			case ActiveCameraTypes.FirstPerson:
				return 11;
			default:
				return 10;
			}
		}

		// Token: 0x06006A32 RID: 27186 RVA: 0x0008725D File Offset: 0x0008545D
		public static bool SetAngleOnActive(this ActiveCameraTypes type)
		{
			return type != ActiveCameraTypes.Death;
		}

		// Token: 0x06006A33 RID: 27187 RVA: 0x00087266 File Offset: 0x00085466
		public static bool SetPosition(this ActiveCameraTypes type)
		{
			return type != ActiveCameraTypes.FirstPerson;
		}

		// Token: 0x06006A34 RID: 27188 RVA: 0x0008726F File Offset: 0x0008546F
		public static bool MatchAvatarRotationToCamera(this ActiveCameraTypes type)
		{
			return type == ActiveCameraTypes.OverTheShoulder || type == ActiveCameraTypes.FirstPerson;
		}
	}
}
