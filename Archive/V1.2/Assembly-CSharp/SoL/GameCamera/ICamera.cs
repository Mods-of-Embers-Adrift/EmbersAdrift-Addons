using System;
using UnityEngine;

namespace SoL.GameCamera
{
	// Token: 0x02000DE6 RID: 3558
	public interface ICamera
	{
		// Token: 0x17001945 RID: 6469
		// (get) Token: 0x06006A1A RID: 27162
		ActiveCameraTypes Type { get; }

		// Token: 0x17001946 RID: 6470
		// (get) Token: 0x06006A1B RID: 27163
		string CameraName { get; }

		// Token: 0x17001947 RID: 6471
		// (get) Token: 0x06006A1C RID: 27164
		// (set) Token: 0x06006A1D RID: 27165
		float XValue { get; set; }

		// Token: 0x17001948 RID: 6472
		// (get) Token: 0x06006A1E RID: 27166
		// (set) Token: 0x06006A1F RID: 27167
		float YValue { get; set; }

		// Token: 0x06006A20 RID: 27168
		void SetPosition(Vector3 position);

		// Token: 0x06006A21 RID: 27169
		Vector3 GetPosition();

		// Token: 0x06006A22 RID: 27170
		bool AllowMovementX();

		// Token: 0x06006A23 RID: 27171
		void SetMaxSpeeds(float xMultiplier, float yMultiplier);

		// Token: 0x06006A24 RID: 27172
		void ExternalDestroy();

		// Token: 0x06006A25 RID: 27173
		void UpdateVerticalOffset();

		// Token: 0x06006A26 RID: 27174
		void ExternalUpdate();
	}
}
