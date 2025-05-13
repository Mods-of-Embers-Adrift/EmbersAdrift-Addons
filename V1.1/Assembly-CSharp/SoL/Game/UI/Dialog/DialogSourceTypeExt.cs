using System;

namespace SoL.Game.UI.Dialog
{
	// Token: 0x02000988 RID: 2440
	public static class DialogSourceTypeExt
	{
		// Token: 0x060048C3 RID: 18627 RVA: 0x00070E98 File Offset: 0x0006F098
		public static bool StripNewLines(this DialogSourceType sourceType)
		{
			return sourceType != DialogSourceType.Object;
		}
	}
}
