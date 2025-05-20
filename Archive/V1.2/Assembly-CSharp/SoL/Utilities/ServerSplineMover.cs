using System;
using BansheeGz.BGSpline.Components;
using SoL.Managers;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002C8 RID: 712
	public class ServerSplineMover : MonoBehaviour
	{
		// Token: 0x060014CE RID: 5326 RVA: 0x000FB8A4 File Offset: 0x000F9AA4
		private void Start()
		{
			if (this.m_cursor == null)
			{
				base.enabled = false;
				return;
			}
			if (Application.isPlaying && GameManager.IsOnline && !GameManager.IsServer)
			{
				base.enabled = false;
				if (this.m_cursor)
				{
					this.m_cursor.gameObject.SetActive(false);
				}
				return;
			}
		}

		// Token: 0x04001D0D RID: 7437
		[SerializeField]
		private bool m_randomizeStartPosition;

		// Token: 0x04001D0E RID: 7438
		[SerializeField]
		private float m_speed = 2f;

		// Token: 0x04001D0F RID: 7439
		[SerializeField]
		private BGCcCursor m_cursor;
	}
}
