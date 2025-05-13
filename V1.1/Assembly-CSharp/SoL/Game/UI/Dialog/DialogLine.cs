using System;
using SoL.Utilities.Extensions;

namespace SoL.Game.UI.Dialog
{
	// Token: 0x0200098A RID: 2442
	public struct DialogLine
	{
		// Token: 0x060048C4 RID: 18628 RVA: 0x001AAF20 File Offset: 0x001A9120
		public DialogLine(string line, DialogFlags flags, DialogSourceType sourceType)
		{
			this.Flags = flags;
			if (sourceType.StripNewLines())
			{
				line = line.TrimEnd('\n');
			}
			if (this.Flags.HasBitFlag(DialogFlags.Player))
			{
				line = line.Color(InkDriver.m_selectedChoiceColor);
			}
			else if (this.Flags.HasBitFlag(DialogFlags.Warning))
			{
				line = line.Color(InkDriver.m_warningColor);
			}
			else if (this.Flags.HasBitFlag(DialogFlags.Emotive))
			{
				line = line.Color(InkDriver.m_emotiveColor);
			}
			else if (this.Flags.HasBitFlag(DialogFlags.StageDirection))
			{
				line = line.Color(InkDriver.m_stageDirectionColor);
			}
			this.Text = line;
		}

		// Token: 0x040043F1 RID: 17393
		public string Text;

		// Token: 0x040043F2 RID: 17394
		public DialogFlags Flags;
	}
}
