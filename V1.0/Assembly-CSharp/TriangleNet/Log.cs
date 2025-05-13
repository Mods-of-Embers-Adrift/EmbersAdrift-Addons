using System;
using System.Collections.Generic;
using TriangleNet.Logging;

namespace TriangleNet
{
	// Token: 0x020000EA RID: 234
	public sealed class Log : ILog<LogItem>
	{
		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x06000838 RID: 2104 RVA: 0x000487F6 File Offset: 0x000469F6
		// (set) Token: 0x06000839 RID: 2105 RVA: 0x000487FD File Offset: 0x000469FD
		public static bool Verbose { get; set; }

		// Token: 0x0600083B RID: 2107 RVA: 0x00048811 File Offset: 0x00046A11
		private Log()
		{
		}

		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x0600083C RID: 2108 RVA: 0x00048824 File Offset: 0x00046A24
		public static ILog<LogItem> Instance
		{
			get
			{
				return Log.instance;
			}
		}

		// Token: 0x0600083D RID: 2109 RVA: 0x0004882B File Offset: 0x00046A2B
		public void Add(LogItem item)
		{
			this.log.Add(item);
		}

		// Token: 0x0600083E RID: 2110 RVA: 0x00048839 File Offset: 0x00046A39
		public void Clear()
		{
			this.log.Clear();
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x00048846 File Offset: 0x00046A46
		public void Info(string message)
		{
			this.log.Add(new LogItem(LogLevel.Info, message));
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x0004885A File Offset: 0x00046A5A
		public void Warning(string message, string location)
		{
			this.log.Add(new LogItem(LogLevel.Warning, message, location));
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x0004886F File Offset: 0x00046A6F
		public void Error(string message, string location)
		{
			this.log.Add(new LogItem(LogLevel.Error, message, location));
		}

		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x06000842 RID: 2114 RVA: 0x00048884 File Offset: 0x00046A84
		public IList<LogItem> Data
		{
			get
			{
				return this.log;
			}
		}

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x06000843 RID: 2115 RVA: 0x0004888C File Offset: 0x00046A8C
		public LogLevel Level
		{
			get
			{
				return this.level;
			}
		}

		// Token: 0x0400098B RID: 2443
		private List<LogItem> log = new List<LogItem>();

		// Token: 0x0400098C RID: 2444
		private LogLevel level;

		// Token: 0x0400098D RID: 2445
		private static readonly Log instance = new Log();
	}
}
