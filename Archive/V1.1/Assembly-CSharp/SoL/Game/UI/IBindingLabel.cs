using System;
using TMPro;

namespace SoL.Game.UI
{
	// Token: 0x02000857 RID: 2135
	public interface IBindingLabel
	{
		// Token: 0x17000E47 RID: 3655
		// (get) Token: 0x06003DAF RID: 15791
		BindingType Type { get; }

		// Token: 0x17000E48 RID: 3656
		// (get) Token: 0x06003DB0 RID: 15792
		int Index { get; }

		// Token: 0x17000E49 RID: 3657
		// (get) Token: 0x06003DB1 RID: 15793
		TextMeshProUGUI Label { get; }

		// Token: 0x17000E4A RID: 3658
		// (get) Token: 0x06003DB2 RID: 15794
		string FormattedString { get; }
	}
}
