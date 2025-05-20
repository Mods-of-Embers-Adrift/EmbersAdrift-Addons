using System;

namespace SoL.Game.UI.Dialog
{
	// Token: 0x0200098B RID: 2443
	public static class DialogFlagsExt
	{
		// Token: 0x060048C5 RID: 18629 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this DialogFlags A, DialogFlags B)
		{
			return (A & B) == B;
		}

		// Token: 0x060048C6 RID: 18630 RVA: 0x000578B5 File Offset: 0x00055AB5
		public static DialogFlags AddBitFlag(this DialogFlags A, DialogFlags B)
		{
			return A | B;
		}

		// Token: 0x060048C7 RID: 18631 RVA: 0x000578BA File Offset: 0x00055ABA
		public static DialogFlags RemoveBitFlag(this DialogFlags A, DialogFlags B)
		{
			return A & ~B;
		}

		// Token: 0x060048C8 RID: 18632 RVA: 0x001AB01C File Offset: 0x001A921C
		public static bool RemainOnSameLine(this DialogFlags flags, DialogFlags prevFlags)
		{
			return ((!flags.HasBitFlag(DialogFlags.Emotive) && prevFlags.HasBitFlag(DialogFlags.Emotive)) || (!prevFlags.HasBitFlag(DialogFlags.Emotive) && flags.HasBitFlag(DialogFlags.Emotive))) && !flags.HasBitFlag(DialogFlags.Player) && !prevFlags.HasBitFlag(DialogFlags.Player) && !flags.HasBitFlag(DialogFlags.ForceNewline);
		}
	}
}
