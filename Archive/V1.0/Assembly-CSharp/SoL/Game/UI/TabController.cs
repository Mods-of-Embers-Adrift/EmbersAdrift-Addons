using System;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI
{
	// Token: 0x020008DD RID: 2269
	public class TabController : MonoBehaviour
	{
		// Token: 0x140000C3 RID: 195
		// (add) Token: 0x06004255 RID: 16981 RVA: 0x0019253C File Offset: 0x0019073C
		// (remove) Token: 0x06004256 RID: 16982 RVA: 0x00192574 File Offset: 0x00190774
		public event Action TabChanged;

		// Token: 0x140000C4 RID: 196
		// (add) Token: 0x06004257 RID: 16983 RVA: 0x001925AC File Offset: 0x001907AC
		// (remove) Token: 0x06004258 RID: 16984 RVA: 0x001925E4 File Offset: 0x001907E4
		public event Action<int> TabIndexActivated;

		// Token: 0x06004259 RID: 16985 RVA: 0x0019261C File Offset: 0x0019081C
		private void Awake()
		{
			for (int i = 0; i < this.m_tabs.Length; i++)
			{
				this.m_tabs[i].Init(this, i, this.m_toggleGroup);
			}
			int num = 0;
			if (this.m_saveActiveTab && !string.IsNullOrEmpty(this.m_savedTabKey))
			{
				num = PlayerPrefs.GetInt(this.m_savedTabKey, 0);
			}
			if (num > this.m_tabs.Length - 1 || !this.m_tabs[num].Toggle.interactable)
			{
				num = this.FirstInteractableTabToggle();
			}
			if (this.m_tabs.Length != 0)
			{
				this.m_tabs[num].Toggle.isOn = true;
			}
		}

		// Token: 0x0600425A RID: 16986 RVA: 0x001926BC File Offset: 0x001908BC
		private void OnDestroy()
		{
			for (int i = 0; i < this.m_tabs.Length; i++)
			{
				this.m_tabs[i].OnDestroy();
			}
		}

		// Token: 0x0600425B RID: 16987 RVA: 0x0006CC5D File Offset: 0x0006AE5D
		public void SwitchToTab(int index)
		{
			if (this.m_tabs.Length == 0 || index < 0 || index >= this.m_tabs.Length)
			{
				return;
			}
			this.m_tabs[index].Toggle.isOn = true;
			Action<int> tabIndexActivated = this.TabIndexActivated;
			if (tabIndexActivated == null)
			{
				return;
			}
			tabIndexActivated(index);
		}

		// Token: 0x0600425C RID: 16988 RVA: 0x001926EC File Offset: 0x001908EC
		private void ToggleModified(TabController.Tab source)
		{
			Action tabChanged = this.TabChanged;
			if (tabChanged != null)
			{
				tabChanged();
			}
			Action<int> tabIndexActivated = this.TabIndexActivated;
			if (tabIndexActivated != null)
			{
				tabIndexActivated(source.Index);
			}
			if (this.m_saveActiveTab && !string.IsNullOrEmpty(this.m_savedTabKey))
			{
				PlayerPrefs.SetInt(this.m_savedTabKey, source.Index);
			}
		}

		// Token: 0x0600425D RID: 16989 RVA: 0x00192748 File Offset: 0x00190948
		private int FirstInteractableTabToggle()
		{
			for (int i = 0; i < this.m_tabs.Length; i++)
			{
				if (this.m_tabs[i].Toggle.interactable)
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x04003F69 RID: 16233
		[SerializeField]
		private ToggleGroup m_toggleGroup;

		// Token: 0x04003F6A RID: 16234
		[SerializeField]
		private TabController.Tab[] m_tabs;

		// Token: 0x04003F6B RID: 16235
		[SerializeField]
		private bool m_saveActiveTab;

		// Token: 0x04003F6C RID: 16236
		[SerializeField]
		private string m_savedTabKey;

		// Token: 0x020008DE RID: 2270
		[Serializable]
		private class Tab
		{
			// Token: 0x17000F14 RID: 3860
			// (get) Token: 0x0600425F RID: 16991 RVA: 0x0006CC9C File Offset: 0x0006AE9C
			public SolToggle Toggle
			{
				get
				{
					return this.m_toggle;
				}
			}

			// Token: 0x17000F15 RID: 3861
			// (get) Token: 0x06004260 RID: 16992 RVA: 0x0006CCA4 File Offset: 0x0006AEA4
			public int Index
			{
				get
				{
					return this.m_index;
				}
			}

			// Token: 0x06004261 RID: 16993 RVA: 0x00192780 File Offset: 0x00190980
			public void Init(TabController controller, int index, ToggleGroup group)
			{
				this.m_controller = controller;
				this.m_index = index;
				this.ToggleContent(false);
				this.m_toggle.isOn = false;
				this.m_toggle.group = group;
				this.m_toggle.onValueChanged.AddListener(new UnityAction<bool>(this.ToggleChanged));
			}

			// Token: 0x06004262 RID: 16994 RVA: 0x0006CCAC File Offset: 0x0006AEAC
			public void OnDestroy()
			{
				this.m_toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.ToggleChanged));
			}

			// Token: 0x06004263 RID: 16995 RVA: 0x0006CCCA File Offset: 0x0006AECA
			private void ToggleChanged(bool isOn)
			{
				this.ToggleContent(isOn);
				if (isOn)
				{
					this.m_controller.ToggleModified(this);
				}
			}

			// Token: 0x06004264 RID: 16996 RVA: 0x0006CCE2 File Offset: 0x0006AEE2
			private void ToggleContent(bool isOn)
			{
				if (this.m_content)
				{
					this.m_content.SetActive(isOn);
				}
				if (this.m_canvasGroup)
				{
					this.m_canvasGroup.SetActive(isOn);
				}
			}

			// Token: 0x04003F6D RID: 16237
			[SerializeField]
			private SolToggle m_toggle;

			// Token: 0x04003F6E RID: 16238
			[SerializeField]
			private GameObject m_content;

			// Token: 0x04003F6F RID: 16239
			[SerializeField]
			private CanvasGroup m_canvasGroup;

			// Token: 0x04003F70 RID: 16240
			private TabController m_controller;

			// Token: 0x04003F71 RID: 16241
			private int m_index;
		}
	}
}
