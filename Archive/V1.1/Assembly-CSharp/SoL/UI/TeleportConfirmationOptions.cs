using System;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x0200035A RID: 858
	public struct TeleportConfirmationOptions : IDialogOptions
	{
		// Token: 0x1700059A RID: 1434
		// (get) Token: 0x0600176B RID: 5995 RVA: 0x000525B6 File Offset: 0x000507B6
		string IDialogOptions.Title
		{
			get
			{
				return this.Title;
			}
		}

		// Token: 0x1700059B RID: 1435
		// (get) Token: 0x0600176C RID: 5996 RVA: 0x000525BE File Offset: 0x000507BE
		string IDialogOptions.Text
		{
			get
			{
				return this.Text;
			}
		}

		// Token: 0x1700059C RID: 1436
		// (get) Token: 0x0600176D RID: 5997 RVA: 0x000525C6 File Offset: 0x000507C6
		string IDialogOptions.ConfirmationText
		{
			get
			{
				return this.ConfirmationText;
			}
		}

		// Token: 0x1700059D RID: 1437
		// (get) Token: 0x0600176E RID: 5998 RVA: 0x000525CE File Offset: 0x000507CE
		string IDialogOptions.CancelText
		{
			get
			{
				return this.CancelText;
			}
		}

		// Token: 0x1700059E RID: 1438
		// (get) Token: 0x0600176F RID: 5999 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IDialogOptions.HideCancelButton
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700059F RID: 1439
		// (get) Token: 0x06001770 RID: 6000 RVA: 0x000525D6 File Offset: 0x000507D6
		bool IDialogOptions.ShowCloseButton
		{
			get
			{
				return this.ShowCloseButton;
			}
		}

		// Token: 0x170005A0 RID: 1440
		// (get) Token: 0x06001771 RID: 6001 RVA: 0x000525DE File Offset: 0x000507DE
		bool IDialogOptions.AllowDragging
		{
			get
			{
				return this.AllowDragging;
			}
		}

		// Token: 0x170005A1 RID: 1441
		// (get) Token: 0x06001772 RID: 6002 RVA: 0x000525E6 File Offset: 0x000507E6
		bool IDialogOptions.BlockInteractions
		{
			get
			{
				return this.BlockInteractions;
			}
		}

		// Token: 0x170005A2 RID: 1442
		// (get) Token: 0x06001773 RID: 6003 RVA: 0x000525EE File Offset: 0x000507EE
		Color IDialogOptions.BackgroundBlockerColor
		{
			get
			{
				return this.BackgroundBlockerColor;
			}
		}

		// Token: 0x170005A3 RID: 1443
		// (get) Token: 0x06001774 RID: 6004 RVA: 0x000525F6 File Offset: 0x000507F6
		Action<bool, object> IDialogOptions.Callback
		{
			get
			{
				return this.Callback;
			}
		}

		// Token: 0x04001F29 RID: 7977
		public bool ShowCloseButton;

		// Token: 0x04001F2A RID: 7978
		public bool AllowDragging;

		// Token: 0x04001F2B RID: 7979
		public bool BlockInteractions;

		// Token: 0x04001F2C RID: 7980
		public Color BackgroundBlockerColor;

		// Token: 0x04001F2D RID: 7981
		public string Title;

		// Token: 0x04001F2E RID: 7982
		public string Text;

		// Token: 0x04001F2F RID: 7983
		public string ConfirmationText;

		// Token: 0x04001F30 RID: 7984
		public string CancelText;

		// Token: 0x04001F31 RID: 7985
		public Action<bool, object> Callback;

		// Token: 0x04001F32 RID: 7986
		public Func<bool> AutoCancel;

		// Token: 0x04001F33 RID: 7987
		public int EssenceCost;
	}
}
