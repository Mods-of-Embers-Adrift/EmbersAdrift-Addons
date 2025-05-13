using System;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace SoL.Utilities.Logging
{
	// Token: 0x02000316 RID: 790
	public static class ClientSinkExtensions
	{
		// Token: 0x06001605 RID: 5637 RVA: 0x00051703 File Offset: 0x0004F903
		public static LoggerConfiguration ClientSink(this LoggerSinkConfiguration sinkConfiguration, ITextFormatter formatter, LogIndex logIndex, LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose, LoggingLevelSwitch levelSwitch = null, LogEventLevel? standardErrorFromLevel = null)
		{
			return sinkConfiguration.Sink(new ClientSink(formatter, logIndex, standardErrorFromLevel), restrictedToMinimumLevel, levelSwitch);
		}
	}
}
