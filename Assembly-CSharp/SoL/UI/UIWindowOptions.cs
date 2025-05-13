using System;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x020003A0 RID: 928
	public class UIWindowOptions : UIWindow
	{
		// Token: 0x06001974 RID: 6516 RVA: 0x00053E94 File Offset: 0x00052094
		private void Update()
		{
			if (!base.Visible)
			{
				return;
			}
			if (Input.GetMouseButtonUp(0) && !base.CursorInside)
			{
				this.Hide(false);
			}
		}
	}
}
