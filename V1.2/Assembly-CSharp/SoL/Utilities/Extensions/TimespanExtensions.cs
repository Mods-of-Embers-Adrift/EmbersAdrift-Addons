using System;
using Cysharp.Text;

namespace SoL.Utilities.Extensions
{
	// Token: 0x0200033D RID: 829
	public static class TimespanExtensions
	{
		// Token: 0x060016B6 RID: 5814 RVA: 0x00100F30 File Offset: 0x000FF130
		public static string GetFormatted(this TimeSpan timeSpan)
		{
			string result;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				bool flag = false;
				bool flag2 = timeSpan.Days > 0;
				bool flag3 = timeSpan.Hours > 0;
				bool flag4 = timeSpan.Minutes > 0;
				bool flag5 = !flag4 && !flag3 && !flag2;
				if (flag2)
				{
					string arg = (timeSpan.Days == 1) ? "day" : "days";
					utf16ValueStringBuilder.AppendFormat<int, string>("{0} {1}", timeSpan.Days, arg);
					flag = true;
				}
				if (flag3)
				{
					if (flag)
					{
						utf16ValueStringBuilder.Append(", ");
					}
					string arg2 = (timeSpan.Hours == 1) ? "hour" : "hours";
					utf16ValueStringBuilder.AppendFormat<int, string>("{0} {1}", timeSpan.Hours, arg2);
					flag = true;
				}
				if (flag4)
				{
					if (flag)
					{
						utf16ValueStringBuilder.Append(", ");
					}
					string arg3 = (timeSpan.Minutes == 1) ? "minute" : "minutes";
					utf16ValueStringBuilder.AppendFormat<int, string>("{0} {1}", timeSpan.Minutes, arg3);
					flag = true;
				}
				if (flag5)
				{
					if (flag)
					{
						utf16ValueStringBuilder.Append(", ");
					}
					string arg4 = (timeSpan.Seconds == 1) ? "second" : "seconds";
					utf16ValueStringBuilder.AppendFormat<int, string>("{0} {1}", timeSpan.Seconds, arg4);
				}
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}
	}
}
