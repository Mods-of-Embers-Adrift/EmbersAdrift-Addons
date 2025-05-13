using System;
using System.Text.RegularExpressions;

namespace SoL
{
	// Token: 0x0200022D RID: 557
	public static class CommandParser
	{
		// Token: 0x060012A2 RID: 4770 RVA: 0x000E7678 File Offset: 0x000E5878
		public static bool TryParseArgs<T1>(this string args, out T1 p1)
		{
			bool result;
			p1 = CommandParser.TryParse<T1>(args, out result);
			return result;
		}

		// Token: 0x060012A3 RID: 4771 RVA: 0x000E7694 File Offset: 0x000E5894
		public static bool TryParseArgs<T1, T2>(this string args, out T1 p1, out T2 p2)
		{
			p1 = default(T1);
			p2 = default(T2);
			bool flag;
			string[] array = CommandParser.breakApart(args, 2, out flag);
			if (!flag)
			{
				return false;
			}
			p1 = CommandParser.TryParse<T1>(array[0], out flag);
			if (!flag)
			{
				return false;
			}
			p2 = CommandParser.TryParse<T2>(array[1], out flag);
			return flag;
		}

		// Token: 0x060012A4 RID: 4772 RVA: 0x000E76E4 File Offset: 0x000E58E4
		public static bool TryParseArgs<T1, T2, T3>(this string args, out T1 p1, out T2 p2, out T3 p3)
		{
			p1 = default(T1);
			p2 = default(T2);
			p3 = default(T3);
			bool flag;
			string[] array = CommandParser.breakApart(args, 3, out flag);
			if (!flag)
			{
				return false;
			}
			p1 = CommandParser.TryParse<T1>(array[0], out flag);
			if (!flag)
			{
				return false;
			}
			p2 = CommandParser.TryParse<T2>(array[1], out flag);
			if (!flag)
			{
				return false;
			}
			p3 = CommandParser.TryParse<T3>(array[2], out flag);
			return flag;
		}

		// Token: 0x060012A5 RID: 4773 RVA: 0x000E7750 File Offset: 0x000E5950
		public static bool CmdSplit(this string command, out string cmd, out string param)
		{
			cmd = string.Empty;
			param = string.Empty;
			bool flag;
			string[] array = CommandParser.breakApart(command, 2, out flag);
			if (flag)
			{
				cmd = array[0];
				param = array[1];
			}
			return flag;
		}

		// Token: 0x060012A6 RID: 4774 RVA: 0x000E7784 File Offset: 0x000E5984
		public static bool CmdSplit<T1>(this string command, out string cmd, out T1 p1)
		{
			cmd = string.Empty;
			p1 = default(T1);
			bool flag;
			string[] array = CommandParser.breakApart(command, 2, out flag);
			if (!flag)
			{
				return flag;
			}
			cmd = array[0];
			p1 = CommandParser.TryParse<T1>(array[1], out flag);
			return flag;
		}

		// Token: 0x060012A7 RID: 4775 RVA: 0x000E77C4 File Offset: 0x000E59C4
		public static bool CmdSplit<T1, T2>(this string command, out string cmd, out T1 p1, out T2 p2)
		{
			cmd = string.Empty;
			p1 = default(T1);
			p2 = default(T2);
			bool flag;
			string[] array = CommandParser.breakApart(command, 3, out flag);
			cmd = array[0];
			if (!flag)
			{
				return flag;
			}
			p1 = CommandParser.TryParse<T1>(array[1], out flag);
			if (!flag)
			{
				return flag;
			}
			p2 = CommandParser.TryParse<T2>(array[2], out flag);
			return flag;
		}

		// Token: 0x060012A8 RID: 4776 RVA: 0x000E7820 File Offset: 0x000E5A20
		public static bool CmdSplit<T1, T2, T3>(this string command, out string cmd, out T1 p1, out T2 p2, out T3 p3)
		{
			cmd = string.Empty;
			p1 = default(T1);
			p2 = default(T2);
			p3 = default(T3);
			bool flag;
			string[] array = CommandParser.breakApart(command, 4, out flag);
			if (!flag)
			{
				return flag;
			}
			cmd = array[0];
			p1 = CommandParser.TryParse<T1>(array[1], out flag);
			if (!flag)
			{
				return flag;
			}
			p2 = CommandParser.TryParse<T2>(array[2], out flag);
			if (!flag)
			{
				return flag;
			}
			p3 = CommandParser.TryParse<T3>(array[3], out flag);
			return flag;
		}

		// Token: 0x060012A9 RID: 4777 RVA: 0x000E789C File Offset: 0x000E5A9C
		public static bool CmdSplit<T1, T2, T3, T4>(this string command, out string cmd, out T1 p1, out T2 p2, out T3 p3, out T4 p4)
		{
			cmd = string.Empty;
			p1 = default(T1);
			p2 = default(T2);
			p3 = default(T3);
			p4 = default(T4);
			bool flag;
			string[] array = CommandParser.breakApart(command, 5, out flag);
			if (!flag)
			{
				return flag;
			}
			cmd = array[0];
			p1 = CommandParser.TryParse<T1>(array[1], out flag);
			if (!flag)
			{
				return flag;
			}
			p2 = CommandParser.TryParse<T2>(array[2], out flag);
			if (!flag)
			{
				return flag;
			}
			p3 = CommandParser.TryParse<T3>(array[3], out flag);
			if (!flag)
			{
				return flag;
			}
			p4 = CommandParser.TryParse<T4>(array[4], out flag);
			return flag;
		}

		// Token: 0x060012AA RID: 4778 RVA: 0x000E7934 File Offset: 0x000E5B34
		public static bool CmdSplit<T1, T2, T3, T4, T5>(this string command, out string cmd, out T1 p1, out T2 p2, out T3 p3, out T4 p4, out T5 p5)
		{
			cmd = string.Empty;
			p1 = default(T1);
			p2 = default(T2);
			p3 = default(T3);
			p4 = default(T4);
			p5 = default(T5);
			bool flag;
			string[] array = CommandParser.breakApart(command, 6, out flag);
			if (!flag)
			{
				return flag;
			}
			cmd = array[0];
			p1 = CommandParser.TryParse<T1>(array[1], out flag);
			if (!flag)
			{
				return flag;
			}
			p2 = CommandParser.TryParse<T2>(array[2], out flag);
			if (!flag)
			{
				return flag;
			}
			p3 = CommandParser.TryParse<T3>(array[3], out flag);
			if (!flag)
			{
				return flag;
			}
			p4 = CommandParser.TryParse<T4>(array[4], out flag);
			if (!flag)
			{
				return flag;
			}
			p5 = CommandParser.TryParse<T5>(array[5], out flag);
			return flag;
		}

		// Token: 0x060012AB RID: 4779 RVA: 0x000E79EC File Offset: 0x000E5BEC
		private static T TryParse<T>(string value, out bool isValid)
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(float))
			{
				float num;
				isValid = float.TryParse(value, out num);
				return (T)((object)num);
			}
			if (typeFromHandle == typeof(int))
			{
				int num2;
				isValid = int.TryParse(value, out num2);
				return (T)((object)num2);
			}
			if (typeFromHandle == typeof(uint))
			{
				uint num3;
				isValid = uint.TryParse(value, out num3);
				return (T)((object)num3);
			}
			if (typeFromHandle == typeof(bool))
			{
				bool flag;
				isValid = bool.TryParse(value, out flag);
				return (T)((object)flag);
			}
			isValid = true;
			return (T)((object)value);
		}

		// Token: 0x060012AC RID: 4780 RVA: 0x000E7AB0 File Offset: 0x000E5CB0
		private static string[] breakApart(string input, int paramCount, out bool isValid)
		{
			if (string.IsNullOrEmpty(input))
			{
				isValid = false;
				return null;
			}
			if (CommandParser.m_regex == null)
			{
				CommandParser.m_regex = new Regex("[\\\"].+?[\\\"]|[^ ]+");
			}
			MatchCollection matchCollection = CommandParser.m_regex.Matches(input);
			if (paramCount > 2 && matchCollection.Count < paramCount)
			{
				isValid = false;
				return null;
			}
			string[] array = new string[paramCount];
			for (int i = 0; i < matchCollection.Count; i++)
			{
				if (i >= paramCount)
				{
					string[] array2 = array;
					int num = paramCount - 1;
					array2[num] = array2[num] + " " + matchCollection[i].Value.Replace("\"", "");
				}
				else
				{
					array[i] = matchCollection[i].Value.Replace("\"", "");
				}
			}
			isValid = true;
			return array;
		}

		// Token: 0x0400107B RID: 4219
		private static Regex m_regex;
	}
}
