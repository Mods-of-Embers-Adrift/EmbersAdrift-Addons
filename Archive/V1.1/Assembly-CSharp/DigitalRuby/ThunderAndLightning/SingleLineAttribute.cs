using System;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000C9 RID: 201
	public class SingleLineAttribute : PropertyAttribute
	{
		// Token: 0x0600075A RID: 1882 RVA: 0x0004805B File Offset: 0x0004625B
		public SingleLineAttribute(string tooltip)
		{
			this.Tooltip = tooltip;
		}

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x0600075B RID: 1883 RVA: 0x0004806A File Offset: 0x0004626A
		// (set) Token: 0x0600075C RID: 1884 RVA: 0x00048072 File Offset: 0x00046272
		public string Tooltip { get; private set; }
	}
}
