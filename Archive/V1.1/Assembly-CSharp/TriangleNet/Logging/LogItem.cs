using System;

namespace TriangleNet.Logging
{
	// Token: 0x02000138 RID: 312
	public class LogItem : ILogItem
	{
		// Token: 0x17000338 RID: 824
		// (get) Token: 0x06000AAC RID: 2732 RVA: 0x00049EA5 File Offset: 0x000480A5
		public DateTime Time
		{
			get
			{
				return this.time;
			}
		}

		// Token: 0x17000339 RID: 825
		// (get) Token: 0x06000AAD RID: 2733 RVA: 0x00049EAD File Offset: 0x000480AD
		public LogLevel Level
		{
			get
			{
				return this.level;
			}
		}

		// Token: 0x1700033A RID: 826
		// (get) Token: 0x06000AAE RID: 2734 RVA: 0x00049EB5 File Offset: 0x000480B5
		public string Message
		{
			get
			{
				return this.message;
			}
		}

		// Token: 0x1700033B RID: 827
		// (get) Token: 0x06000AAF RID: 2735 RVA: 0x00049EBD File Offset: 0x000480BD
		public string Info
		{
			get
			{
				return this.info;
			}
		}

		// Token: 0x06000AB0 RID: 2736 RVA: 0x00049EC5 File Offset: 0x000480C5
		public LogItem(LogLevel level, string message) : this(level, message, "")
		{
		}

		// Token: 0x06000AB1 RID: 2737 RVA: 0x00049ED4 File Offset: 0x000480D4
		public LogItem(LogLevel level, string message, string info)
		{
			this.time = DateTime.Now;
			this.level = level;
			this.message = message;
			this.info = info;
		}

		// Token: 0x04000AF2 RID: 2802
		private DateTime time;

		// Token: 0x04000AF3 RID: 2803
		private LogLevel level;

		// Token: 0x04000AF4 RID: 2804
		private string message;

		// Token: 0x04000AF5 RID: 2805
		private string info;
	}
}
