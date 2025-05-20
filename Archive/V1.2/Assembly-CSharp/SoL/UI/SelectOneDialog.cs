using System;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.UI
{
	// Token: 0x02000355 RID: 853
	public class SelectOneDialog : BaseDialog<SelectOneOptions>
	{
		// Token: 0x17000584 RID: 1412
		// (get) Token: 0x06001746 RID: 5958 RVA: 0x00052453 File Offset: 0x00050653
		protected override object Result
		{
			get
			{
				return this.GetSelectedIndex();
			}
		}

		// Token: 0x06001747 RID: 5959 RVA: 0x00052460 File Offset: 0x00050660
		protected override void Start()
		{
			base.Start();
			this.InitToggles();
		}

		// Token: 0x06001748 RID: 5960 RVA: 0x0010217C File Offset: 0x0010037C
		private void Update()
		{
			if (!base.Visible)
			{
				return;
			}
			bool flag = this.GetSelectedIndex() > -1;
			if (!flag && this.m_confirm.interactable)
			{
				this.m_confirm.interactable = false;
				return;
			}
			if (flag && !this.m_confirm.interactable)
			{
				this.m_confirm.interactable = true;
			}
		}

		// Token: 0x06001749 RID: 5961 RVA: 0x001021D8 File Offset: 0x001003D8
		private void InitToggles()
		{
			this.m_toggleGroup.allowSwitchOff = true;
			for (int i = 0; i < this.m_toggles.Length; i++)
			{
				this.m_toggleGroup.RegisterToggle(this.m_toggles[i]);
				this.m_toggles[i].group = this.m_toggleGroup;
			}
			this.m_toggleGroup.SetAllTogglesOff(true);
		}

		// Token: 0x0600174A RID: 5962 RVA: 0x00102238 File Offset: 0x00100438
		protected override void InitInternal()
		{
			this.m_toggleGroup.SetAllTogglesOff(true);
			for (int i = 0; i < this.m_toggles.Length; i++)
			{
				this.m_toggles[i].isOn = false;
				if (i >= this.m_currentOptions.Choices.Length)
				{
					this.m_toggles[i].gameObject.SetActive(false);
				}
				else
				{
					this.m_toggles[i].text = this.m_currentOptions.Choices[i];
					this.m_toggles[i].gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x0600174B RID: 5963 RVA: 0x001022C4 File Offset: 0x001004C4
		private int GetSelectedIndex()
		{
			for (int i = 0; i < this.m_toggles.Length; i++)
			{
				if (this.m_toggles[i].isOn)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x04001F09 RID: 7945
		[SerializeField]
		private ToggleGroup m_toggleGroup;

		// Token: 0x04001F0A RID: 7946
		[SerializeField]
		private SolToggle[] m_toggles;
	}
}
