using System;
using UnityEngine;

namespace SoL.GameCamera
{
	// Token: 0x02000DE0 RID: 3552
	[Serializable]
	internal class CameraEffectOverride
	{
		// Token: 0x1700193A RID: 6458
		// (get) Token: 0x060069D1 RID: 27089 RVA: 0x00086F4A File Offset: 0x0008514A
		public CameraEffectTypes Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x1700193B RID: 6459
		// (get) Token: 0x060069D2 RID: 27090 RVA: 0x00086F52 File Offset: 0x00085152
		public GameObject Obj
		{
			get
			{
				return this.m_gameObject;
			}
		}

		// Token: 0x04005C1E RID: 23582
		[SerializeField]
		private CameraEffectTypes m_type;

		// Token: 0x04005C1F RID: 23583
		[SerializeField]
		private GameObject m_gameObject;
	}
}
