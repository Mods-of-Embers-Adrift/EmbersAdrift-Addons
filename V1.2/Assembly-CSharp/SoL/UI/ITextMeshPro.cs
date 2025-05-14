using System;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x02000368 RID: 872
	public interface ITextMeshPro
	{
		// Token: 0x170005BB RID: 1467
		// (get) Token: 0x060017D2 RID: 6098
		// (set) Token: 0x060017D3 RID: 6099
		string text { get; set; }

		// Token: 0x060017D4 RID: 6100
		void SetTextColor(string hex);

		// Token: 0x060017D5 RID: 6101
		void SetTextColor(Color c);
	}
}
