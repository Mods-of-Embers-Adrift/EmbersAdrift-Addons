using System;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x02000358 RID: 856
	public struct SteamSubscriptionDialogOptions : IDialogOptions
	{
		// Token: 0x17000590 RID: 1424
		// (get) Token: 0x0600175E RID: 5982 RVA: 0x0005252B File Offset: 0x0005072B
		string IDialogOptions.Title
		{
			get
			{
				return this.Title;
			}
		}

		// Token: 0x17000591 RID: 1425
		// (get) Token: 0x0600175F RID: 5983 RVA: 0x00052533 File Offset: 0x00050733
		string IDialogOptions.Text
		{
			get
			{
				return this.Text;
			}
		}

		// Token: 0x17000592 RID: 1426
		// (get) Token: 0x06001760 RID: 5984 RVA: 0x0005253B File Offset: 0x0005073B
		string IDialogOptions.ConfirmationText
		{
			get
			{
				return this.ConfirmationText;
			}
		}

		// Token: 0x17000593 RID: 1427
		// (get) Token: 0x06001761 RID: 5985 RVA: 0x00052543 File Offset: 0x00050743
		string IDialogOptions.CancelText
		{
			get
			{
				return this.CancelText;
			}
		}

		// Token: 0x17000594 RID: 1428
		// (get) Token: 0x06001762 RID: 5986 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IDialogOptions.HideCancelButton
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000595 RID: 1429
		// (get) Token: 0x06001763 RID: 5987 RVA: 0x0005254B File Offset: 0x0005074B
		bool IDialogOptions.ShowCloseButton
		{
			get
			{
				return this.ShowCloseButton;
			}
		}

		// Token: 0x17000596 RID: 1430
		// (get) Token: 0x06001764 RID: 5988 RVA: 0x00052553 File Offset: 0x00050753
		bool IDialogOptions.AllowDragging
		{
			get
			{
				return this.AllowDragging;
			}
		}

		// Token: 0x17000597 RID: 1431
		// (get) Token: 0x06001765 RID: 5989 RVA: 0x0005255B File Offset: 0x0005075B
		bool IDialogOptions.BlockInteractions
		{
			get
			{
				return this.BlockInteractions;
			}
		}

		// Token: 0x17000598 RID: 1432
		// (get) Token: 0x06001766 RID: 5990 RVA: 0x00052563 File Offset: 0x00050763
		Color IDialogOptions.BackgroundBlockerColor
		{
			get
			{
				return this.BackgroundBlockerColor;
			}
		}

		// Token: 0x17000599 RID: 1433
		// (get) Token: 0x06001767 RID: 5991 RVA: 0x0005256B File Offset: 0x0005076B
		Action<bool, object> IDialogOptions.Callback
		{
			get
			{
				return this.Callback;
			}
		}

		// Token: 0x04001F1B RID: 7963
		public bool ShowCloseButton;

		// Token: 0x04001F1C RID: 7964
		public bool AllowDragging;

		// Token: 0x04001F1D RID: 7965
		public bool BlockInteractions;

		// Token: 0x04001F1E RID: 7966
		public Color BackgroundBlockerColor;

		// Token: 0x04001F1F RID: 7967
		public string Title;

		// Token: 0x04001F20 RID: 7968
		public string Text;

		// Token: 0x04001F21 RID: 7969
		public string ConfirmationText;

		// Token: 0x04001F22 RID: 7970
		public string CancelText;

		// Token: 0x04001F23 RID: 7971
		public Action<bool, object> Callback;

		// Token: 0x04001F24 RID: 7972
		public Func<bool> AutoCancel;
	}
}
