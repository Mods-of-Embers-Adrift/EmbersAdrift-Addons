using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace SoL.Networking.SolServer
{
	// Token: 0x020003D7 RID: 983
	public static class ConfigHelpers
	{
		// Token: 0x06001A5F RID: 6751 RVA: 0x001085C4 File Offset: 0x001067C4
		private static T GetFromFile<T>(string filePath) where T : class
		{
			if (File.Exists(filePath))
			{
				string json;
				using (StreamReader streamReader = new StreamReader(filePath))
				{
					json = streamReader.ReadToEnd();
				}
				T t = ConfigHelpers.ProcessJson<T>(json);
				if (t != null)
				{
					if (Application.isPlaying)
					{
						Debug.Log("Processed config from " + filePath + "\nContents: " + t.ToString());
					}
					return t;
				}
			}
			return default(T);
		}

		// Token: 0x06001A60 RID: 6752 RVA: 0x00108644 File Offset: 0x00106844
		private static T ProcessJson<T>(string json) where T : class
		{
			T result;
			try
			{
				result = JsonConvert.DeserializeObject<T>(json);
			}
			catch (JsonReaderException arg)
			{
				try
				{
					Debug.LogWarning(string.Format("Extraneous Characters from HTTP! Stripping response: {0}", arg));
					string text = json.Substring(json.IndexOf('{'));
					result = JsonConvert.DeserializeObject<T>(text.Substring(0, text.LastIndexOf('}') + 1));
				}
				catch (Exception ex)
				{
					string str = "Caught exception while processing config!  ";
					Exception ex2 = ex;
					Debug.LogError(str + ((ex2 != null) ? ex2.ToString() : null));
					result = default(T);
				}
			}
			catch (Exception ex3)
			{
				string str2 = "Caught exception while processing config!  ";
				Exception ex4 = ex3;
				Debug.LogError(str2 + ((ex4 != null) ? ex4.ToString() : null));
				result = default(T);
			}
			return result;
		}

		// Token: 0x06001A61 RID: 6753 RVA: 0x00108718 File Offset: 0x00106918
		private static T GetConfig<T>(SolConfigType configType, bool isServer = false) where T : class
		{
			string configPath = ConfigHelpers.GetConfigPath(configType, isServer, false);
			if (!string.IsNullOrEmpty(configPath))
			{
				return ConfigHelpers.GetFromFile<T>(configPath);
			}
			return default(T);
		}

		// Token: 0x06001A62 RID: 6754 RVA: 0x00108748 File Offset: 0x00106948
		public static string GetConfigPath(SolConfigType configType, bool isServer = false, bool relativePath = false)
		{
			string text = isServer ? "solserverconfig.json" : "solclientconfig.json";
			if (!relativePath)
			{
				return Path.Combine(Application.streamingAssetsPath, text);
			}
			return "Assets/StreamingAssets/" + text;
		}

		// Token: 0x06001A63 RID: 6755 RVA: 0x00045BCA File Offset: 0x00043DCA
		public static SolConfigType GetConfigType(bool isServer)
		{
			return SolConfigType.Default;
		}

		// Token: 0x06001A64 RID: 6756 RVA: 0x0005487A File Offset: 0x00052A7A
		public static ClientConfig GetClientConfig()
		{
			return ConfigHelpers.GetConfig<ClientConfig>(ConfigHelpers.GetConfigType(false), false);
		}

		// Token: 0x06001A65 RID: 6757 RVA: 0x00054888 File Offset: 0x00052A88
		public static ServerConfig GetServerConfig()
		{
			return ConfigHelpers.GetConfig<ServerConfig>(ConfigHelpers.GetConfigType(true), true);
		}

		// Token: 0x0400214D RID: 8525
		public const string kClientConfigFilename = "solclientconfig.json";

		// Token: 0x0400214E RID: 8526
		public const string kServerConfigFilename = "solserverconfig.json";

		// Token: 0x0400214F RID: 8527
		public static string kConfigAbsolutePath = Path.Combine(Application.dataPath, "SolData/_Configs");

		// Token: 0x04002150 RID: 8528
		public static string kConfigRelativePath = "Assets/SolData/_Configs";
	}
}
