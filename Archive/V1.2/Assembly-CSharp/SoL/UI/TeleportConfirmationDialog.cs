using System;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x0200035B RID: 859
	public class TeleportConfirmationDialog : BaseDialog<TeleportConfirmationOptions>
	{
		// Token: 0x170005A4 RID: 1444
		// (get) Token: 0x06001775 RID: 6005 RVA: 0x000525FE File Offset: 0x000507FE
		protected override object Result
		{
			get
			{
				return this.m_useTravelEssence;
			}
		}

		// Token: 0x06001776 RID: 6006 RVA: 0x00102570 File Offset: 0x00100770
		protected override void Start()
		{
			base.Start();
			if (this.m_teleportButtonPanel)
			{
				this.m_teleportButtonPanel.TravelClicked += this.UseTravelClicked;
				this.m_teleportButtonPanel.EssenceClicked += this.UseEssenceClicked;
			}
		}

		// Token: 0x06001777 RID: 6007 RVA: 0x001025C0 File Offset: 0x001007C0
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (this.m_teleportButtonPanel)
			{
				this.m_teleportButtonPanel.TravelClicked -= this.UseTravelClicked;
				this.m_teleportButtonPanel.EssenceClicked -= this.UseEssenceClicked;
			}
		}

		// Token: 0x06001778 RID: 6008 RVA: 0x0005260B File Offset: 0x0005080B
		private void Update()
		{
			if (base.Visible && this.m_currentOptions.AutoCancel != null && this.m_currentOptions.AutoCancel())
			{
				base.Cancel();
				this.m_currentOptions.AutoCancel = null;
			}
		}

		// Token: 0x06001779 RID: 6009 RVA: 0x00052646 File Offset: 0x00050846
		protected override void InitInternal()
		{
			base.InitInternal();
			this.m_useTravelEssence = true;
			if (this.m_teleportButtonPanel)
			{
				this.m_teleportButtonPanel.InitButtons(this.m_currentOptions.EssenceCost);
			}
		}

		// Token: 0x0600177A RID: 6010 RVA: 0x00052678 File Offset: 0x00050878
		private void UseTravelClicked()
		{
			this.m_useTravelEssence = true;
			this.Confirm();
		}

		// Token: 0x0600177B RID: 6011 RVA: 0x00052687 File Offset: 0x00050887
		private void UseEssenceClicked()
		{
			this.m_useTravelEssence = false;
			this.Confirm();
		}

		// Token: 0x04001F34 RID: 7988
		[SerializeField]
		private TeleportButtonPanel m_teleportButtonPanel;

		// Token: 0x04001F35 RID: 7989
		private bool m_useTravelEssence = true;
	}
}
