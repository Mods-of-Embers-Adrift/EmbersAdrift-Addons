using System;
using System.Collections.Generic;
using SoL.Utilities;

namespace SoL.UI
{
	// Token: 0x02000347 RID: 839
	public class ContextMenuAction : IPoolable
	{
		// Token: 0x060016E5 RID: 5861 RVA: 0x0005205E File Offset: 0x0005025E
		public void Reset()
		{
			this.Text = null;
			this.Enabled = true;
			this.Callback = null;
			this.NestedActions = null;
			this.InteractiveCheck = null;
		}

		// Token: 0x17000554 RID: 1364
		// (get) Token: 0x060016E6 RID: 5862 RVA: 0x00052083 File Offset: 0x00050283
		// (set) Token: 0x060016E7 RID: 5863 RVA: 0x0005208B File Offset: 0x0005028B
		bool IPoolable.InPool
		{
			get
			{
				return this.m_inPool;
			}
			set
			{
				this.m_inPool = value;
			}
		}

		// Token: 0x04001EB1 RID: 7857
		private bool m_inPool;

		// Token: 0x04001EB2 RID: 7858
		public string Text;

		// Token: 0x04001EB3 RID: 7859
		public bool Enabled = true;

		// Token: 0x04001EB4 RID: 7860
		public Action Callback;

		// Token: 0x04001EB5 RID: 7861
		public List<ContextMenuAction> NestedActions;

		// Token: 0x04001EB6 RID: 7862
		public Func<bool> InteractiveCheck;
	}
}
