using System;
using System.Collections.Generic;
using System.Globalization;
using Cysharp.Text;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.SolServer;
using UnityEngine;

namespace SoL.Utilities.Logging
{
	// Token: 0x0200031E RID: 798
	public static class SolDebug
	{
		// Token: 0x1700054C RID: 1356
		// (get) Token: 0x0600161B RID: 5659 RVA: 0x000517BB File Offset: 0x0004F9BB
		private static LogIndex[] IndexNames
		{
			get
			{
				if (SolDebug.m_indexNames == null)
				{
					SolDebug.m_indexNames = (LogIndex[])Enum.GetValues(typeof(LogIndex));
				}
				return SolDebug.m_indexNames;
			}
		}

		// Token: 0x0600161D RID: 5661 RVA: 0x000FE8F4 File Offset: 0x000FCAF4
		public static void Initialize()
		{
			if (SolDebug.m_logIndexes != null)
			{
				return;
			}
			string text = "http://10.0.0.101:9200";
			if (GameManager.IsServer)
			{
				ServerConfig serverConfig = ConfigHelpers.GetServerConfig();
				if (serverConfig != null)
				{
					if (string.IsNullOrEmpty(serverConfig.ElasticUri))
					{
						Debug.LogError("INVALID SERVER CONFIG! NullOrEmpty ServerConfig.ElasticUrl");
					}
					else
					{
						text = serverConfig.ElasticUri;
					}
				}
				else
				{
					Debug.LogError("INVALID SERVER CONFIG! Null ServerConfig");
				}
				Debug.Log("CONNECTING TO ELASTIC VIA: " + text);
			}
			SolDebug.m_logIndexes = new Dictionary<LogIndex, Serilog.ILogger>(default(LogIndexComparer));
			for (int i = 0; i < SolDebug.IndexNames.Length; i++)
			{
				if (SolDebug.IndexNames[i] != LogIndex.None)
				{
					Serilog.ILogger logger;
					if (GameManager.IsServer)
					{
						string indexFormat = "embers-" + GlobalSettings.Values.Configs.Data.DeploymentBranch.ToLower() + "-" + SolDebug.IndexNames[i].ToString().ToLower() + "-{0:yyyy.MM}";
						Uri node = new Uri(text);
						LoggerConfiguration loggerConfiguration = new LoggerConfiguration().WriteTo.Elasticsearch(new ElasticsearchSinkOptions(node)
						{
							AutoRegisterTemplate = true,
							AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
							IndexFormat = indexFormat
						});
						if (SolDebug.IndexNames[i] == LogIndex.Loot)
						{
							loggerConfiguration = loggerConfiguration.Destructure.ByTransforming<ItemLootLogEntry>((ItemLootLogEntry ille) => new
							{
								ille.ArchetypeId,
								ille.ArchetypeName,
								ille.DisplayName,
								ille.Quantity
							});
						}
						logger = loggerConfiguration.CreateLogger();
					}
					else
					{
						logger = new LoggerConfiguration().WriteTo.ClientSink(new ElasticsearchJsonFormatter(false, null, true, null, null, false, true, false), SolDebug.IndexNames[i], LogEventLevel.Verbose, null, null).CreateLogger();
					}
					if (logger != null)
					{
						SolDebug.m_logIndexes.Add(SolDebug.IndexNames[i], logger);
					}
				}
			}
		}

		// Token: 0x0600161E RID: 5662 RVA: 0x000FEABC File Offset: 0x000FCCBC
		public static void LogToIndex(LogLevel logLevel, LogIndex logIndex, string messageTemplate, object[] propertyValues)
		{
			if (logIndex == LogIndex.None)
			{
				throw new ArgumentException("logIndex");
			}
			Serilog.ILogger logger;
			if (!SolDebug.m_logIndexes.TryGetValue(logIndex, out logger))
			{
				return;
			}
			switch (logLevel)
			{
			case LogLevel.Verbose:
				logger.Verbose(messageTemplate, propertyValues);
				return;
			case LogLevel.Information:
				logger.Information(messageTemplate, propertyValues);
				return;
			case LogLevel.Warning:
				logger.Warning(messageTemplate, propertyValues);
				return;
			case LogLevel.Error:
				logger.Error(messageTemplate, propertyValues);
				return;
			case LogLevel.Fatal:
				logger.Fatal(messageTemplate, propertyValues);
				return;
			}
			logger.Debug(messageTemplate, propertyValues);
		}

		// Token: 0x0600161F RID: 5663 RVA: 0x000FEB3C File Offset: 0x000FCD3C
		public static void LogWithTime(string msg, bool useUtc = true)
		{
			DateTime dateTime = useUtc ? DateTime.UtcNow : DateTime.Now;
			Debug.Log(GameManager.IsServer ? ZString.Format<string, string>("[{0}] {1}", dateTime.ToString(CultureInfo.InvariantCulture), msg) : ZString.Format<string, int, string>("[{0}][{1}] {2}", dateTime.ToString(CultureInfo.InvariantCulture), Time.frameCount, msg));
		}

		// Token: 0x04001E2A RID: 7722
		private static Dictionary<LogIndex, Serilog.ILogger> m_logIndexes;

		// Token: 0x04001E2B RID: 7723
		private static LogIndex[] m_indexNames;
	}
}
