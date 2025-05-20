using System;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x02000356 RID: 854
	[Serializable]
	public struct SelectValueOptions : IDialogOptions
	{
		// Token: 0x17000585 RID: 1413
		// (get) Token: 0x0600174D RID: 5965 RVA: 0x00052476 File Offset: 0x00050676
		string IDialogOptions.Title
		{
			get
			{
				return this.Title;
			}
		}

		// Token: 0x17000586 RID: 1414
		// (get) Token: 0x0600174E RID: 5966 RVA: 0x0005247E File Offset: 0x0005067E
		string IDialogOptions.Text
		{
			get
			{
				return this.Text;
			}
		}

		// Token: 0x17000587 RID: 1415
		// (get) Token: 0x0600174F RID: 5967 RVA: 0x00052486 File Offset: 0x00050686
		string IDialogOptions.ConfirmationText
		{
			get
			{
				return this.ConfirmationText;
			}
		}

		// Token: 0x17000588 RID: 1416
		// (get) Token: 0x06001750 RID: 5968 RVA: 0x0005248E File Offset: 0x0005068E
		string IDialogOptions.CancelText
		{
			get
			{
				return this.CancelText;
			}
		}

		// Token: 0x17000589 RID: 1417
		// (get) Token: 0x06001751 RID: 5969 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IDialogOptions.HideCancelButton
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700058A RID: 1418
		// (get) Token: 0x06001752 RID: 5970 RVA: 0x00052496 File Offset: 0x00050696
		bool IDialogOptions.ShowCloseButton
		{
			get
			{
				return this.ShowCloseButton;
			}
		}

		// Token: 0x1700058B RID: 1419
		// (get) Token: 0x06001753 RID: 5971 RVA: 0x0005249E File Offset: 0x0005069E
		bool IDialogOptions.AllowDragging
		{
			get
			{
				return this.AllowDragging;
			}
		}

		// Token: 0x1700058C RID: 1420
		// (get) Token: 0x06001754 RID: 5972 RVA: 0x000524A6 File Offset: 0x000506A6
		bool IDialogOptions.BlockInteractions
		{
			get
			{
				return this.BlockInteractions;
			}
		}

		// Token: 0x1700058D RID: 1421
		// (get) Token: 0x06001755 RID: 5973 RVA: 0x000524AE File Offset: 0x000506AE
		Color IDialogOptions.BackgroundBlockerColor
		{
			get
			{
				return this.BackgroundBlockerColor;
			}
		}

		// Token: 0x1700058E RID: 1422
		// (get) Token: 0x06001756 RID: 5974 RVA: 0x000524B6 File Offset: 0x000506B6
		Action<bool, object> IDialogOptions.Callback
		{
			get
			{
				return this.Callback;
			}
		}

		// Token: 0x04001F0B RID: 7947
		public bool ShowCloseButton;

		// Token: 0x04001F0C RID: 7948
		public bool AllowDragging;

		// Token: 0x04001F0D RID: 7949
		public bool BlockInteractions;

		// Token: 0x04001F0E RID: 7950
		public Color BackgroundBlockerColor;

		// Token: 0x04001F0F RID: 7951
		public string Title;

		// Token: 0x04001F10 RID: 7952
		public string Text;

		// Token: 0x04001F11 RID: 7953
		public string ConfirmationText;

		// Token: 0x04001F12 RID: 7954
		public string CancelText;

		// Token: 0x04001F13 RID: 7955
		public Action<bool, object> Callback;

		// Token: 0x04001F14 RID: 7956
		public int DefaultValue;

		// Token: 0x04001F15 RID: 7957
		public int MinValue;

		// Token: 0x04001F16 RID: 7958
		public int MaxValue;

		// Token: 0x04001F17 RID: 7959
		public Action<int> ConfirmationCallback;
	}
}
