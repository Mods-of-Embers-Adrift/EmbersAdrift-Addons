using System;
using System.IO;
using System.Text;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using SoL.Managers;
using SoL.Networking.SolServer;

namespace SoL.Utilities.Logging
{
	// Token: 0x02000315 RID: 789
	public class ClientSink : ILogEventSink
	{
		// Token: 0x06001603 RID: 5635 RVA: 0x000516DB File Offset: 0x0004F8DB
		public ClientSink(ITextFormatter formatter, LogIndex logIndex, LogEventLevel? standardErrorFromLevel)
		{
			this.m_formatter = formatter;
			this.m_logIndex = logIndex;
			this.m_standardErrorFromLevel = standardErrorFromLevel;
			this.m_stringBuilder = new StringBuilder();
		}

		// Token: 0x06001604 RID: 5636 RVA: 0x000FE3C0 File Offset: 0x000FC5C0
		public void Emit(LogEvent logEvent)
		{
			if (GameManager.IsServer)
			{
				return;
			}
			this.m_stringBuilder.Clear();
			StringWriter stringWriter = new StringWriter(this.m_stringBuilder);
			this.m_formatter.Format(logEvent, stringWriter);
			string value = stringWriter.ToString();
			stringWriter.Dispose();
			if (!string.IsNullOrEmpty(value))
			{
				LogIndex logIndex = this.m_logIndex;
				CommandType cmdType;
				if (logIndex != LogIndex.Error)
				{
					if (logIndex != LogIndex.ClientPerformance)
					{
						return;
					}
					cmdType = CommandType.performance;
				}
				else
				{
					cmdType = CommandType.error;
				}
				new SolServerCommand(CommandClass.client, cmdType)
				{
					Args = 
					{
						{
							"data",
							value
						}
					}
				}.Send();
			}
		}

		// Token: 0x04001DFB RID: 7675
		private readonly LogEventLevel? m_standardErrorFromLevel;

		// Token: 0x04001DFC RID: 7676
		private readonly ITextFormatter m_formatter;

		// Token: 0x04001DFD RID: 7677
		private readonly StringBuilder m_stringBuilder;

		// Token: 0x04001DFE RID: 7678
		private readonly LogIndex m_logIndex;
	}
}
