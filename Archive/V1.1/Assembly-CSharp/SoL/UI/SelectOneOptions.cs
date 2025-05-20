using System;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x02000354 RID: 852
	[Serializable]
	public struct SelectOneOptions : IDialogOptions
	{
		// Token: 0x1700057A RID: 1402
		// (get) Token: 0x0600173C RID: 5948 RVA: 0x0005240B File Offset: 0x0005060B
		string IDialogOptions.Title
		{
			get
			{
				return this.Title;
			}
		}

		// Token: 0x1700057B RID: 1403
		// (get) Token: 0x0600173D RID: 5949 RVA: 0x00052413 File Offset: 0x00050613
		string IDialogOptions.Text
		{
			get
			{
				return this.Text;
			}
		}

		// Token: 0x1700057C RID: 1404
		// (get) Token: 0x0600173E RID: 5950 RVA: 0x0005241B File Offset: 0x0005061B
		string IDialogOptions.ConfirmationText
		{
			get
			{
				return this.ConfirmationText;
			}
		}

		// Token: 0x1700057D RID: 1405
		// (get) Token: 0x0600173F RID: 5951 RVA: 0x00052423 File Offset: 0x00050623
		string IDialogOptions.CancelText
		{
			get
			{
				return this.CancelText;
			}
		}

		// Token: 0x1700057E RID: 1406
		// (get) Token: 0x06001740 RID: 5952 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IDialogOptions.HideCancelButton
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700057F RID: 1407
		// (get) Token: 0x06001741 RID: 5953 RVA: 0x0005242B File Offset: 0x0005062B
		bool IDialogOptions.ShowCloseButton
		{
			get
			{
				return this.ShowCloseButton;
			}
		}

		// Token: 0x17000580 RID: 1408
		// (get) Token: 0x06001742 RID: 5954 RVA: 0x00052433 File Offset: 0x00050633
		bool IDialogOptions.AllowDragging
		{
			get
			{
				return this.AllowDragging;
			}
		}

		// Token: 0x17000581 RID: 1409
		// (get) Token: 0x06001743 RID: 5955 RVA: 0x0005243B File Offset: 0x0005063B
		bool IDialogOptions.BlockInteractions
		{
			get
			{
				return this.BlockInteractions;
			}
		}

		// Token: 0x17000582 RID: 1410
		// (get) Token: 0x06001744 RID: 5956 RVA: 0x00052443 File Offset: 0x00050643
		Color IDialogOptions.BackgroundBlockerColor
		{
			get
			{
				return this.BackgroundBlockerColor;
			}
		}

		// Token: 0x17000583 RID: 1411
		// (get) Token: 0x06001745 RID: 5957 RVA: 0x0005244B File Offset: 0x0005064B
		Action<bool, object> IDialogOptions.Callback
		{
			get
			{
				return this.Callback;
			}
		}

		// Token: 0x04001EFF RID: 7935
		public bool ShowCloseButton;

		// Token: 0x04001F00 RID: 7936
		public bool AllowDragging;

		// Token: 0x04001F01 RID: 7937
		public bool BlockInteractions;

		// Token: 0x04001F02 RID: 7938
		public Color BackgroundBlockerColor;

		// Token: 0x04001F03 RID: 7939
		public string Title;

		// Token: 0x04001F04 RID: 7940
		public string Text;

		// Token: 0x04001F05 RID: 7941
		public string ConfirmationText;

		// Token: 0x04001F06 RID: 7942
		public string CancelText;

		// Token: 0x04001F07 RID: 7943
		public Action<bool, object> Callback;

		// Token: 0x04001F08 RID: 7944
		public string[] Choices;
	}
}
