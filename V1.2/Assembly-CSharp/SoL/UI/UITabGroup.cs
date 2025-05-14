using System;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x02000394 RID: 916
	public class UITabGroup : MonoBehaviour
	{
		// Token: 0x0600190F RID: 6415 RVA: 0x00105E40 File Offset: 0x00104040
		private void Awake()
		{
			for (int i = 0; i < this.m_tabs.Length; i++)
			{
				this.m_tabs[i].Initialize(this, i);
			}
		}

		// Token: 0x06001910 RID: 6416 RVA: 0x00053968 File Offset: 0x00051B68
		private void Start()
		{
			this.TabSelected(0);
		}

		// Token: 0x06001911 RID: 6417 RVA: 0x00105E70 File Offset: 0x00104070
		public void TabSelected(int index)
		{
			for (int i = 0; i < this.m_tabs.Length; i++)
			{
				this.m_tabs[i].SetTabActive(i == index);
			}
		}

		// Token: 0x0400201A RID: 8218
		[SerializeField]
		private UITab[] m_tabs;
	}
}
