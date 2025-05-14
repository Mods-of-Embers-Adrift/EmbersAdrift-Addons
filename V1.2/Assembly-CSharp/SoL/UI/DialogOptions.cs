using System;
using SoL.Game.Objects.Archetypes;
using TMPro;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x0200034B RID: 843
	[Serializable]
	public struct DialogOptions : IDialogOptions
	{
		// Token: 0x1700055F RID: 1375
		// (get) Token: 0x06001706 RID: 5894 RVA: 0x000521C9 File Offset: 0x000503C9
		string IDialogOptions.Title
		{
			get
			{
				return this.Title;
			}
		}

		// Token: 0x17000560 RID: 1376
		// (get) Token: 0x06001707 RID: 5895 RVA: 0x000521D1 File Offset: 0x000503D1
		string IDialogOptions.Text
		{
			get
			{
				return this.Text;
			}
		}

		// Token: 0x17000561 RID: 1377
		// (get) Token: 0x06001708 RID: 5896 RVA: 0x000521D9 File Offset: 0x000503D9
		string IDialogOptions.ConfirmationText
		{
			get
			{
				return this.ConfirmationText;
			}
		}

		// Token: 0x17000562 RID: 1378
		// (get) Token: 0x06001709 RID: 5897 RVA: 0x000521E1 File Offset: 0x000503E1
		string IDialogOptions.CancelText
		{
			get
			{
				return this.CancelText;
			}
		}

		// Token: 0x17000563 RID: 1379
		// (get) Token: 0x0600170A RID: 5898 RVA: 0x000521E9 File Offset: 0x000503E9
		bool IDialogOptions.HideCancelButton
		{
			get
			{
				return this.HideCancelButton;
			}
		}

		// Token: 0x17000564 RID: 1380
		// (get) Token: 0x0600170B RID: 5899 RVA: 0x000521F1 File Offset: 0x000503F1
		bool IDialogOptions.ShowCloseButton
		{
			get
			{
				return this.ShowCloseButton;
			}
		}

		// Token: 0x17000565 RID: 1381
		// (get) Token: 0x0600170C RID: 5900 RVA: 0x000521F9 File Offset: 0x000503F9
		bool IDialogOptions.AllowDragging
		{
			get
			{
				return this.AllowDragging;
			}
		}

		// Token: 0x17000566 RID: 1382
		// (get) Token: 0x0600170D RID: 5901 RVA: 0x00052201 File Offset: 0x00050401
		bool IDialogOptions.BlockInteractions
		{
			get
			{
				return this.BlockInteractions;
			}
		}

		// Token: 0x17000567 RID: 1383
		// (get) Token: 0x0600170E RID: 5902 RVA: 0x00052209 File Offset: 0x00050409
		Color IDialogOptions.BackgroundBlockerColor
		{
			get
			{
				return this.BackgroundBlockerColor;
			}
		}

		// Token: 0x17000568 RID: 1384
		// (get) Token: 0x0600170F RID: 5903 RVA: 0x00052211 File Offset: 0x00050411
		Action<bool, object> IDialogOptions.Callback
		{
			get
			{
				return this.Callback;
			}
		}

		// Token: 0x04001EC2 RID: 7874
		public bool ShowCloseButton;

		// Token: 0x04001EC3 RID: 7875
		public bool HideCancelButton;

		// Token: 0x04001EC4 RID: 7876
		public bool AllowDragging;

		// Token: 0x04001EC5 RID: 7877
		public bool BlockInteractions;

		// Token: 0x04001EC6 RID: 7878
		public bool AsciiOnly;

		// Token: 0x04001EC7 RID: 7879
		public Color BackgroundBlockerColor;

		// Token: 0x04001EC8 RID: 7880
		public bool AllowRichText;

		// Token: 0x04001EC9 RID: 7881
		public int CharacterLimit;

		// Token: 0x04001ECA RID: 7882
		public int LineLimit;

		// Token: 0x04001ECB RID: 7883
		public TMP_InputField.ContentType? ContentType;

		// Token: 0x04001ECC RID: 7884
		public TMP_InputField.LineType? LineType;

		// Token: 0x04001ECD RID: 7885
		public string Title;

		// Token: 0x04001ECE RID: 7886
		public string Text;

		// Token: 0x04001ECF RID: 7887
		public string ConfirmationText;

		// Token: 0x04001ED0 RID: 7888
		public string CancelText;

		// Token: 0x04001ED1 RID: 7889
		public Action<bool, object> Callback;

		// Token: 0x04001ED2 RID: 7890
		public Func<bool> AutoCancel;

		// Token: 0x04001ED3 RID: 7891
		public ArchetypeInstance Instance;

		// Token: 0x04001ED4 RID: 7892
		public ulong? Currency;

		// Token: 0x04001ED5 RID: 7893
		public string CurrencyLabel;
	}
}
