using System;

namespace SoL.Game.UI
{
	// Token: 0x020008E5 RID: 2277
	public class TogglableKeybindTooltip : KeybindTooltip
	{
		// Token: 0x17000F25 RID: 3877
		// (get) Token: 0x060042A6 RID: 17062 RVA: 0x0006D023 File Offset: 0x0006B223
		protected override bool IncludeKeybind
		{
			get
			{
				return this.KeybindTooltipEnabled;
			}
		}

		// Token: 0x17000F26 RID: 3878
		// (get) Token: 0x060042A7 RID: 17063 RVA: 0x0006D02B File Offset: 0x0006B22B
		// (set) Token: 0x060042A8 RID: 17064 RVA: 0x0006D033 File Offset: 0x0006B233
		public bool KeybindTooltipEnabled { get; set; }
	}
}
