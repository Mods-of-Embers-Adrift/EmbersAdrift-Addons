using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002B8 RID: 696
	public class ReparentOnAwake : MonoBehaviour
	{
		// Token: 0x060014A7 RID: 5287 RVA: 0x000FB468 File Offset: 0x000F9668
		private void Awake()
		{
			if (this.m_targetParent)
			{
				base.gameObject.transform.SetParent(this.m_targetParent);
				if (this.m_resetPosition)
				{
					base.gameObject.transform.localPosition = Vector3.zero;
				}
				if (this.m_resetRotation)
				{
					base.gameObject.transform.localRotation = Quaternion.identity;
				}
			}
		}

		// Token: 0x04001CE3 RID: 7395
		[SerializeField]
		private bool m_resetPosition;

		// Token: 0x04001CE4 RID: 7396
		[SerializeField]
		private bool m_resetRotation;

		// Token: 0x04001CE5 RID: 7397
		[SerializeField]
		private Transform m_targetParent;
	}
}
