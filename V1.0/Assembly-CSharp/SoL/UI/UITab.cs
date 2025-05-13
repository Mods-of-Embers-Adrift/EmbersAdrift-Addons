using System;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.UI
{
	// Token: 0x02000393 RID: 915
	public class UITab : ToggleController
	{
		// Token: 0x06001909 RID: 6409 RVA: 0x000538EE File Offset: 0x00051AEE
		private void Awake()
		{
			this.m_button.onClick.AddListener(new UnityAction(this.OnTabClicked));
		}

		// Token: 0x0600190A RID: 6410 RVA: 0x0005390C File Offset: 0x00051B0C
		private void OnDestroy()
		{
			this.m_button.onClick.RemoveAllListeners();
		}

		// Token: 0x0600190B RID: 6411 RVA: 0x0005391E File Offset: 0x00051B1E
		public void Initialize(UITabGroup group, int index)
		{
			this.m_group = group;
			this.m_index = index;
		}

		// Token: 0x0600190C RID: 6412 RVA: 0x0005392E File Offset: 0x00051B2E
		private void OnTabClicked()
		{
			this.m_group.TabSelected(this.m_index);
		}

		// Token: 0x0600190D RID: 6413 RVA: 0x00053941 File Offset: 0x00051B41
		public void SetTabActive(bool enabled)
		{
			base.Toggle(enabled);
			this.m_button.interactable = !enabled;
		}

		// Token: 0x04002017 RID: 8215
		[SerializeField]
		private Button m_button;

		// Token: 0x04002018 RID: 8216
		private UITabGroup m_group;

		// Token: 0x04002019 RID: 8217
		private int m_index = -1;
	}
}
