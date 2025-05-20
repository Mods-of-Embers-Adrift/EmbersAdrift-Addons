using System;
using SoL.UI;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000DC6 RID: 3526
	public class WindowTesting : MonoBehaviour
	{
		// Token: 0x06006943 RID: 26947 RVA: 0x00217450 File Offset: 0x00215650
		private void Show()
		{
			for (int i = 0; i < this.m_windows.Length; i++)
			{
				this.m_windows[i].Show(false);
			}
		}

		// Token: 0x06006944 RID: 26948 RVA: 0x00217480 File Offset: 0x00215680
		private void Hide()
		{
			for (int i = 0; i < this.m_windows.Length; i++)
			{
				this.m_windows[i].Hide(false);
			}
		}

		// Token: 0x04005BA2 RID: 23458
		[SerializeField]
		private UIWindow[] m_windows;
	}
}
