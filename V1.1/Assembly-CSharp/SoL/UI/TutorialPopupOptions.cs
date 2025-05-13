using System;
using SoL.Game.Messages;
using SoL.Game.Notifications;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x0200035D RID: 861
	[Serializable]
	public struct TutorialPopupOptions : IDialogOptions
	{
		// Token: 0x170005A6 RID: 1446
		// (get) Token: 0x06001783 RID: 6019 RVA: 0x0005270C File Offset: 0x0005090C
		string IDialogOptions.Title
		{
			get
			{
				return this.Title;
			}
		}

		// Token: 0x170005A7 RID: 1447
		// (get) Token: 0x06001784 RID: 6020 RVA: 0x00052714 File Offset: 0x00050914
		string IDialogOptions.Text
		{
			get
			{
				return this.Text;
			}
		}

		// Token: 0x170005A8 RID: 1448
		// (get) Token: 0x06001785 RID: 6021 RVA: 0x0005271C File Offset: 0x0005091C
		string IDialogOptions.ConfirmationText
		{
			get
			{
				if (!string.IsNullOrEmpty(this.ConfirmationText))
				{
					return this.ConfirmationText;
				}
				return "Ok";
			}
		}

		// Token: 0x170005A9 RID: 1449
		// (get) Token: 0x06001786 RID: 6022 RVA: 0x00049FFA File Offset: 0x000481FA
		string IDialogOptions.CancelText
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170005AA RID: 1450
		// (get) Token: 0x06001787 RID: 6023 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IDialogOptions.HideCancelButton
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170005AB RID: 1451
		// (get) Token: 0x06001788 RID: 6024 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IDialogOptions.ShowCloseButton
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170005AC RID: 1452
		// (get) Token: 0x06001789 RID: 6025 RVA: 0x0004479C File Offset: 0x0004299C
		bool IDialogOptions.AllowDragging
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170005AD RID: 1453
		// (get) Token: 0x0600178A RID: 6026 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IDialogOptions.BlockInteractions
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170005AE RID: 1454
		// (get) Token: 0x0600178B RID: 6027 RVA: 0x000522F9 File Offset: 0x000504F9
		Color IDialogOptions.BackgroundBlockerColor
		{
			get
			{
				return Color.white;
			}
		}

		// Token: 0x170005AF RID: 1455
		// (get) Token: 0x0600178C RID: 6028 RVA: 0x00049FFA File Offset: 0x000481FA
		Action<bool, object> IDialogOptions.Callback
		{
			get
			{
				return null;
			}
		}

		// Token: 0x04001F39 RID: 7993
		public string Title;

		// Token: 0x04001F3A RID: 7994
		[TextArea(0, 20)]
		public string Text;

		// Token: 0x04001F3B RID: 7995
		public MessageType MessageType;

		// Token: 0x04001F3C RID: 7996
		public string ConfirmationText;

		// Token: 0x04001F3D RID: 7997
		public bool PreventDragging;

		// Token: 0x04001F3E RID: 7998
		public BaseNotification Tutorial;
	}
}
