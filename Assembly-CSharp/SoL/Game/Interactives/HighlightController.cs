using System;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B7B RID: 2939
	public class HighlightController : MonoBehaviour
	{
		// Token: 0x1700151A RID: 5402
		// (get) Token: 0x06005A8A RID: 23178 RVA: 0x00045BCA File Offset: 0x00043DCA
		// (set) Token: 0x06005A8B RID: 23179 RVA: 0x0004475B File Offset: 0x0004295B
		public bool HighlightEnabled
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		// Token: 0x06005A8C RID: 23180 RVA: 0x0007CC56 File Offset: 0x0007AE56
		private void ToggleHighlight()
		{
			this.HighlightEnabled = !this.HighlightEnabled;
		}
	}
}
