using System;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x02000871 RID: 2161
	[Serializable]
	public struct SelectCurrencyOptions : IDialogOptions
	{
		// Token: 0x17000E7E RID: 3710
		// (get) Token: 0x06003EC2 RID: 16066 RVA: 0x0006A7A8 File Offset: 0x000689A8
		string IDialogOptions.Title
		{
			get
			{
				return this.Title;
			}
		}

		// Token: 0x17000E7F RID: 3711
		// (get) Token: 0x06003EC3 RID: 16067 RVA: 0x0006A7B0 File Offset: 0x000689B0
		string IDialogOptions.Text
		{
			get
			{
				return this.Text;
			}
		}

		// Token: 0x17000E80 RID: 3712
		// (get) Token: 0x06003EC4 RID: 16068 RVA: 0x0006A7B8 File Offset: 0x000689B8
		string IDialogOptions.ConfirmationText
		{
			get
			{
				return this.ConfirmationText;
			}
		}

		// Token: 0x17000E81 RID: 3713
		// (get) Token: 0x06003EC5 RID: 16069 RVA: 0x0006A7C0 File Offset: 0x000689C0
		string IDialogOptions.CancelText
		{
			get
			{
				return this.CancelText;
			}
		}

		// Token: 0x17000E82 RID: 3714
		// (get) Token: 0x06003EC6 RID: 16070 RVA: 0x00045BCA File Offset: 0x00043DCA
		bool IDialogOptions.HideCancelButton
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E83 RID: 3715
		// (get) Token: 0x06003EC7 RID: 16071 RVA: 0x0006A7C8 File Offset: 0x000689C8
		bool IDialogOptions.ShowCloseButton
		{
			get
			{
				return this.ShowCloseButton;
			}
		}

		// Token: 0x17000E84 RID: 3716
		// (get) Token: 0x06003EC8 RID: 16072 RVA: 0x0006A7D0 File Offset: 0x000689D0
		bool IDialogOptions.AllowDragging
		{
			get
			{
				return this.AllowDragging;
			}
		}

		// Token: 0x17000E85 RID: 3717
		// (get) Token: 0x06003EC9 RID: 16073 RVA: 0x0006A7D8 File Offset: 0x000689D8
		bool IDialogOptions.BlockInteractions
		{
			get
			{
				return this.BlockInteractions;
			}
		}

		// Token: 0x17000E86 RID: 3718
		// (get) Token: 0x06003ECA RID: 16074 RVA: 0x0006A7E0 File Offset: 0x000689E0
		Color IDialogOptions.BackgroundBlockerColor
		{
			get
			{
				return this.BackgroundBlockerColor;
			}
		}

		// Token: 0x17000E87 RID: 3719
		// (get) Token: 0x06003ECB RID: 16075 RVA: 0x0006A7E8 File Offset: 0x000689E8
		Action<bool, object> IDialogOptions.Callback
		{
			get
			{
				return this.Callback;
			}
		}

		// Token: 0x04003CBE RID: 15550
		public bool ShowCloseButton;

		// Token: 0x04003CBF RID: 15551
		public bool AllowDragging;

		// Token: 0x04003CC0 RID: 15552
		public bool BlockInteractions;

		// Token: 0x04003CC1 RID: 15553
		public Color BackgroundBlockerColor;

		// Token: 0x04003CC2 RID: 15554
		public string Title;

		// Token: 0x04003CC3 RID: 15555
		public string Text;

		// Token: 0x04003CC4 RID: 15556
		public string ConfirmationText;

		// Token: 0x04003CC5 RID: 15557
		public string CancelText;

		// Token: 0x04003CC6 RID: 15558
		public UIWindow ParentWindow;

		// Token: 0x04003CC7 RID: 15559
		public Action<bool, object> Callback;

		// Token: 0x04003CC8 RID: 15560
		public ulong AllowableCurrency;

		// Token: 0x04003CC9 RID: 15561
		public ulong InitialCurrency;

		// Token: 0x04003CCA RID: 15562
		public ulong MinimumCurrency;

		// Token: 0x04003CCB RID: 15563
		public bool HideSlider;

		// Token: 0x04003CCC RID: 15564
		public Vector3? PosOverride;

		// Token: 0x04003CCD RID: 15565
		public Action<int> ConfirmationCallback;
	}
}
