using System;
using SoL.Game.Messages;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x0200034D RID: 845
	[Serializable]
	public struct CenterScreenAnnouncementOptions : IDialogOptions
	{
		// Token: 0x1700056C RID: 1388
		// (get) Token: 0x0600171E RID: 5918 RVA: 0x000522E9 File Offset: 0x000504E9
		string IDialogOptions.Title
		{
			get
			{
				return this.Title;
			}
		}

		// Token: 0x1700056D RID: 1389
		// (get) Token: 0x0600171F RID: 5919 RVA: 0x000522F1 File Offset: 0x000504F1
		string IDialogOptions.Text
		{
			get
			{
				return this.Text;
			}
		}

		// Token: 0x1700056E RID: 1390
		// (get) Token: 0x06001720 RID: 5920 RVA: 0x00049FFA File Offset: 0x000481FA
		string IDialogOptions.ConfirmationText
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700056F RID: 1391
		// (get) Token: 0x06001721 RID: 5921 RVA: 0x00049FFA File Offset: 0x000481FA
		string IDialogOptions.CancelText
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000570 RID: 1392
		// (get) Token: 0x06001722 RID: 5922 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IDialogOptions.HideCancelButton
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000571 RID: 1393
		// (get) Token: 0x06001723 RID: 5923 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IDialogOptions.ShowCloseButton
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000572 RID: 1394
		// (get) Token: 0x06001724 RID: 5924 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IDialogOptions.AllowDragging
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000573 RID: 1395
		// (get) Token: 0x06001725 RID: 5925 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IDialogOptions.BlockInteractions
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000574 RID: 1396
		// (get) Token: 0x06001726 RID: 5926 RVA: 0x000522F9 File Offset: 0x000504F9
		Color IDialogOptions.BackgroundBlockerColor
		{
			get
			{
				return Color.white;
			}
		}

		// Token: 0x17000575 RID: 1397
		// (get) Token: 0x06001727 RID: 5927 RVA: 0x00049FFA File Offset: 0x000481FA
		Action<bool, object> IDialogOptions.Callback
		{
			get
			{
				return null;
			}
		}

		// Token: 0x04001EDD RID: 7901
		public string Title;

		// Token: 0x04001EDE RID: 7902
		[TextArea(0, 20)]
		public string Text;

		// Token: 0x04001EDF RID: 7903
		public float TimeShown;

		// Token: 0x04001EE0 RID: 7904
		public float ShowDelay;

		// Token: 0x04001EE1 RID: 7905
		public MessageType MessageType;

		// Token: 0x04001EE2 RID: 7906
		public UniqueId? SourceId;
	}
}
