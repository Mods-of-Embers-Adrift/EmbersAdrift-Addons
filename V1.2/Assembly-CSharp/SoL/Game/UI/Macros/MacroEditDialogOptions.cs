using System;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.UI.Macros
{
	// Token: 0x0200097D RID: 2429
	[Serializable]
	public struct MacroEditDialogOptions : IDialogOptions
	{
		// Token: 0x17001009 RID: 4105
		// (get) Token: 0x06004855 RID: 18517 RVA: 0x00070AC8 File Offset: 0x0006ECC8
		string IDialogOptions.Title
		{
			get
			{
				return this.Title;
			}
		}

		// Token: 0x1700100A RID: 4106
		// (get) Token: 0x06004856 RID: 18518 RVA: 0x00070AD0 File Offset: 0x0006ECD0
		string IDialogOptions.Text
		{
			get
			{
				return this.Text;
			}
		}

		// Token: 0x1700100B RID: 4107
		// (get) Token: 0x06004857 RID: 18519 RVA: 0x00070AD8 File Offset: 0x0006ECD8
		string IDialogOptions.ConfirmationText
		{
			get
			{
				return this.ConfirmationText;
			}
		}

		// Token: 0x1700100C RID: 4108
		// (get) Token: 0x06004858 RID: 18520 RVA: 0x00070AE0 File Offset: 0x0006ECE0
		string IDialogOptions.CancelText
		{
			get
			{
				return this.CancelText;
			}
		}

		// Token: 0x1700100D RID: 4109
		// (get) Token: 0x06004859 RID: 18521 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IDialogOptions.HideCancelButton
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700100E RID: 4110
		// (get) Token: 0x0600485A RID: 18522 RVA: 0x00070AE8 File Offset: 0x0006ECE8
		bool IDialogOptions.ShowCloseButton
		{
			get
			{
				return this.ShowCloseButton;
			}
		}

		// Token: 0x1700100F RID: 4111
		// (get) Token: 0x0600485B RID: 18523 RVA: 0x00070AF0 File Offset: 0x0006ECF0
		bool IDialogOptions.AllowDragging
		{
			get
			{
				return this.AllowDragging;
			}
		}

		// Token: 0x17001010 RID: 4112
		// (get) Token: 0x0600485C RID: 18524 RVA: 0x00070AF8 File Offset: 0x0006ECF8
		bool IDialogOptions.BlockInteractions
		{
			get
			{
				return this.BlockInteractions;
			}
		}

		// Token: 0x17001011 RID: 4113
		// (get) Token: 0x0600485D RID: 18525 RVA: 0x00070B00 File Offset: 0x0006ED00
		Color IDialogOptions.BackgroundBlockerColor
		{
			get
			{
				return this.BackgroundBlockerColor;
			}
		}

		// Token: 0x17001012 RID: 4114
		// (get) Token: 0x0600485E RID: 18526 RVA: 0x00070B08 File Offset: 0x0006ED08
		Action<bool, object> IDialogOptions.Callback
		{
			get
			{
				return this.Callback;
			}
		}

		// Token: 0x040043A4 RID: 17316
		public string Title;

		// Token: 0x040043A5 RID: 17317
		public string Text;

		// Token: 0x040043A6 RID: 17318
		public string ConfirmationText;

		// Token: 0x040043A7 RID: 17319
		public string CancelText;

		// Token: 0x040043A8 RID: 17320
		public bool ShowCloseButton;

		// Token: 0x040043A9 RID: 17321
		public bool AllowDragging;

		// Token: 0x040043AA RID: 17322
		public bool BlockInteractions;

		// Token: 0x040043AB RID: 17323
		public Color BackgroundBlockerColor;

		// Token: 0x040043AC RID: 17324
		public Action<bool, object> Callback;

		// Token: 0x040043AD RID: 17325
		public Func<bool> AutoCancel;

		// Token: 0x040043AE RID: 17326
		public MacroData Data;
	}
}
