using System;

namespace SoL.UI
{
	// Token: 0x02000350 RID: 848
	public class ConfirmationDialog : BaseDialog<DialogOptions>
	{
		// Token: 0x06001736 RID: 5942 RVA: 0x000523C0 File Offset: 0x000505C0
		private void Update()
		{
			if (base.Visible && this.m_currentOptions.AutoCancel != null && this.m_currentOptions.AutoCancel())
			{
				base.Cancel();
				this.m_currentOptions.AutoCancel = null;
			}
		}
	}
}
