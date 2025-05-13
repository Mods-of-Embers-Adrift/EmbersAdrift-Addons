using System;
using System.Text;
using SoL.Game.EffectSystem;
using SoL.UI;
using TMPro;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x020008D9 RID: 2265
	[Obsolete]
	public class StatPanelElementUI : MonoBehaviour
	{
		// Token: 0x06004239 RID: 16953 RVA: 0x0006CAFD File Offset: 0x0006ACFD
		private void Awake()
		{
			this.m_accordionElement = base.gameObject.GetComponent<UIAccordionElement>();
		}

		// Token: 0x0600423A RID: 16954 RVA: 0x0006CB10 File Offset: 0x0006AD10
		public void SetTitle(string txt)
		{
			this.m_title.text = txt;
		}

		// Token: 0x0600423B RID: 16955 RVA: 0x0006CB1E File Offset: 0x0006AD1E
		public void SetText(string txt)
		{
			this.m_text.text = txt;
		}

		// Token: 0x0600423C RID: 16956 RVA: 0x0006CB2C File Offset: 0x0006AD2C
		public void InitMod(EffectChannelType channelType)
		{
			this.m_channelType = channelType;
			this.SetTitle(this.m_channelType.ToString());
			this.RefreshPanel();
		}

		// Token: 0x0600423D RID: 16957 RVA: 0x0006CB52 File Offset: 0x0006AD52
		public void InitStats()
		{
			this.m_isStats = true;
			this.SetTitle("Stats");
			this.RefreshPanel();
		}

		// Token: 0x0600423E RID: 16958 RVA: 0x0006CB6C File Offset: 0x0006AD6C
		public void RefreshPanel()
		{
			if (!this.m_isStats)
			{
				this.PopulatePanelForMod();
			}
		}

		// Token: 0x0600423F RID: 16959 RVA: 0x0004475B File Offset: 0x0004295B
		private void PopulatePanelForMod()
		{
		}

		// Token: 0x04003F4C RID: 16204
		private static StringBuilder m_sb = new StringBuilder();

		// Token: 0x04003F4D RID: 16205
		[SerializeField]
		private TextMeshProUGUI m_title;

		// Token: 0x04003F4E RID: 16206
		[SerializeField]
		private TextMeshProUGUI m_text;

		// Token: 0x04003F4F RID: 16207
		private UIAccordionElement m_accordionElement;

		// Token: 0x04003F50 RID: 16208
		private EffectChannelType m_channelType;

		// Token: 0x04003F51 RID: 16209
		private bool m_isStats;
	}
}
