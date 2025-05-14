using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Rewired;

namespace SoL.Utilities.Extensions
{
	// Token: 0x02000338 RID: 824
	public static class StringExtensions
	{
		// Token: 0x06001695 RID: 5781 RVA: 0x00100CD8 File Offset: 0x000FEED8
		public static string Between(this string value, string a, string b)
		{
			int num = value.IndexOf(a);
			int num2 = value.LastIndexOf(b);
			if (num == -1)
			{
				return "";
			}
			if (num2 == -1)
			{
				return "";
			}
			int num3 = num + a.Length;
			if (num3 >= num2)
			{
				return "";
			}
			return value.Substring(num3, num2 - num3);
		}

		// Token: 0x06001696 RID: 5782 RVA: 0x00100D28 File Offset: 0x000FEF28
		public static string Before(this string value, string a)
		{
			int num = value.IndexOf(a);
			if (num == -1)
			{
				return "";
			}
			return value.Substring(0, num);
		}

		// Token: 0x06001697 RID: 5783 RVA: 0x00100D50 File Offset: 0x000FEF50
		public static string After(this string value, string a)
		{
			int num = value.LastIndexOf(a);
			if (num == -1)
			{
				return "";
			}
			int num2 = num + a.Length;
			if (num2 >= value.Length)
			{
				return "";
			}
			return value.Substring(num2);
		}

		// Token: 0x06001698 RID: 5784 RVA: 0x00051C95 File Offset: 0x0004FE95
		public static string ToTitleCase(this string value)
		{
			return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value.ToLowerInvariant());
		}

		// Token: 0x06001699 RID: 5785 RVA: 0x00100D90 File Offset: 0x000FEF90
		public static bool ContainsIgnoreCase(this List<string> list, string value)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (string.Equals(list[i], value, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600169A RID: 5786 RVA: 0x00051CAC File Offset: 0x0004FEAC
		public static string ReplaceActionMappings(this string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
				text = StringExtensions.m_actionPattern.Replace(text, (Match match) => SolInput.Mapper.GetPrimaryBindingNameForAction(ReInput.mapping.GetActionId(match.Groups[1].Value)) ?? "Unbound");
			}
			return text;
		}

		// Token: 0x0600169B RID: 5787 RVA: 0x00051CE3 File Offset: 0x0004FEE3
		public static int TitleCompare(this string a, string b)
		{
			return a.Titlify().CompareTo(b.Titlify());
		}

		// Token: 0x0600169C RID: 5788 RVA: 0x00051CF6 File Offset: 0x0004FEF6
		public static string Titlify(this string title)
		{
			return StringExtensions.m_titleWordPattern.Replace(title, "$2, $1");
		}

		// Token: 0x04001E5C RID: 7772
		private static Regex m_actionPattern = new Regex("{action:([\\w\\d]+)}", RegexOptions.Compiled);

		// Token: 0x04001E5D RID: 7773
		private static Regex m_titleWordPattern = new Regex("^(The|A|An) ([\\S\\s]+)$", RegexOptions.Compiled);

		// Token: 0x02000339 RID: 825
		public struct StringComparerInvariantCultureIgnoreCase : IEqualityComparer<string>
		{
			// Token: 0x0600169E RID: 5790 RVA: 0x00051D2A File Offset: 0x0004FF2A
			bool IEqualityComparer<string>.Equals(string x, string y)
			{
				return x != null && x.Equals(y, StringComparison.InvariantCultureIgnoreCase);
			}

			// Token: 0x0600169F RID: 5791 RVA: 0x00050A5F File Offset: 0x0004EC5F
			int IEqualityComparer<string>.GetHashCode(string obj)
			{
				return obj.GetHashCode();
			}
		}
	}
}
