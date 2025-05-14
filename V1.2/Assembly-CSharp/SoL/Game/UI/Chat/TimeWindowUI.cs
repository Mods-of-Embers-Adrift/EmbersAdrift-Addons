using System;
using Cysharp.Text;
using SoL.Game.SkyDome;
using SoL.UI;
using TMPro;
using UnityEngine;

namespace SoL.Game.UI.Chat
{
	// Token: 0x020009BC RID: 2492
	public class TimeWindowUI : MonoBehaviour
	{
		// Token: 0x1700108E RID: 4238
		// (get) Token: 0x06004B59 RID: 19289 RVA: 0x00072FEA File Offset: 0x000711EA
		internal UIWindow Window
		{
			get
			{
				return this.m_window;
			}
		}

		// Token: 0x06004B5A RID: 19290 RVA: 0x00072FF2 File Offset: 0x000711F2
		private void Update()
		{
			if (this.m_window && this.m_window.Visible)
			{
				this.m_label.ZStringSetText(SkyDomeManager.GetFullTimeDisplay());
			}
		}

		// Token: 0x06004B5B RID: 19291 RVA: 0x0007301E File Offset: 0x0007121E
		public void ToggleWindow()
		{
			if (this.m_window)
			{
				if (this.m_window.Visible)
				{
					this.m_window.Hide(false);
					return;
				}
				this.m_window.Show(false);
			}
		}

		// Token: 0x040045E1 RID: 17889
		[SerializeField]
		private UIWindow m_window;

		// Token: 0x040045E2 RID: 17890
		[SerializeField]
		private TextMeshProUGUI m_label;
	}
}
