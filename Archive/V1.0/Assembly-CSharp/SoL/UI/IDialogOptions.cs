using System;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x0200034A RID: 842
	public interface IDialogOptions
	{
		// Token: 0x17000555 RID: 1365
		// (get) Token: 0x060016FC RID: 5884
		string Title { get; }

		// Token: 0x17000556 RID: 1366
		// (get) Token: 0x060016FD RID: 5885
		string Text { get; }

		// Token: 0x17000557 RID: 1367
		// (get) Token: 0x060016FE RID: 5886
		string ConfirmationText { get; }

		// Token: 0x17000558 RID: 1368
		// (get) Token: 0x060016FF RID: 5887
		string CancelText { get; }

		// Token: 0x17000559 RID: 1369
		// (get) Token: 0x06001700 RID: 5888
		bool HideCancelButton { get; }

		// Token: 0x1700055A RID: 1370
		// (get) Token: 0x06001701 RID: 5889
		bool ShowCloseButton { get; }

		// Token: 0x1700055B RID: 1371
		// (get) Token: 0x06001702 RID: 5890
		bool AllowDragging { get; }

		// Token: 0x1700055C RID: 1372
		// (get) Token: 0x06001703 RID: 5891
		bool BlockInteractions { get; }

		// Token: 0x1700055D RID: 1373
		// (get) Token: 0x06001704 RID: 5892
		Color BackgroundBlockerColor { get; }

		// Token: 0x1700055E RID: 1374
		// (get) Token: 0x06001705 RID: 5893
		Action<bool, object> Callback { get; }
	}
}
