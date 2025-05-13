using System;
using Cysharp.Text;
using UnityEngine;

namespace SoL.Utilities.Extensions
{
	// Token: 0x0200033B RID: 827
	public static class TextExtensions
	{
		// Token: 0x060016A3 RID: 5795 RVA: 0x00051D75 File Offset: 0x0004FF75
		public static string Italicize(this string text)
		{
			return ZString.Format<string>("<i>{0}</i>", text);
		}

		// Token: 0x060016A4 RID: 5796 RVA: 0x00051D82 File Offset: 0x0004FF82
		public static string Bold(this string text)
		{
			return ZString.Format<string>("<b>{0}</b>", text);
		}

		// Token: 0x060016A5 RID: 5797 RVA: 0x00051D8F File Offset: 0x0004FF8F
		public static string BoldItalicize(this string text)
		{
			return ZString.Format<string>("<b><i>{0}</i></b>", text);
		}

		// Token: 0x060016A6 RID: 5798 RVA: 0x00051D9C File Offset: 0x0004FF9C
		public static string Strikethrough(this string text)
		{
			return ZString.Format<string>("<s>{0}</s>", text);
		}

		// Token: 0x060016A7 RID: 5799 RVA: 0x00051DA9 File Offset: 0x0004FFA9
		public static string Underline(this string text)
		{
			return ZString.Format<string>("<u>{0}</u>", text);
		}

		// Token: 0x060016A8 RID: 5800 RVA: 0x00051DB6 File Offset: 0x0004FFB6
		public static string Indent(this string text, int percent)
		{
			return ZString.Format<int, string>("<indent={0}%>{1}</indent>", percent, text);
		}

		// Token: 0x060016A9 RID: 5801 RVA: 0x00051DC4 File Offset: 0x0004FFC4
		public static string Color(this string text, Color color)
		{
			return ZString.Format<string, string>("<color={0}>{1}</color>", color.ToHex(), text);
		}

		// Token: 0x060016AA RID: 5802 RVA: 0x00051DD7 File Offset: 0x0004FFD7
		public static string NoParse(this string text)
		{
			return ZString.Format<string>("<noparse>{0}</noparse>", text);
		}

		// Token: 0x060016AB RID: 5803 RVA: 0x00051DE4 File Offset: 0x0004FFE4
		public static bool IsAscii(this char ch)
		{
			return ch < '\u007f';
		}
	}
}
