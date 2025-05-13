using System;
using MongoDB.Driver;
using SoL.Networking.SolServer;
using UnityEngine;

namespace SoL.Networking.Database
{
	// Token: 0x02000452 RID: 1106
	public static class ExternalGameDatabase
	{
		// Token: 0x17000675 RID: 1653
		// (get) Token: 0x06001F2F RID: 7983 RVA: 0x00057039 File Offset: 0x00055239
		public static ReplaceOptions ReplaceOptions_Upstart
		{
			get
			{
				return new ReplaceOptions
				{
					IsUpsert = true
				};
			}
		}

		// Token: 0x17000676 RID: 1654
		// (get) Token: 0x06001F30 RID: 7984 RVA: 0x00057047 File Offset: 0x00055247
		private static ServerConfig Config
		{
			get
			{
				if (ExternalGameDatabase.m_config == null)
				{
					ExternalGameDatabase.m_config = ConfigHelpers.GetServerConfig();
				}
				return ExternalGameDatabase.m_config;
			}
		}

		// Token: 0x17000677 RID: 1655
		// (get) Token: 0x06001F31 RID: 7985 RVA: 0x0011EE64 File Offset: 0x0011D064
		private static MongoClient Client
		{
			get
			{
				if (ExternalGameDatabase.m_client == null)
				{
					if (ExternalGameDatabase.Config == null)
					{
						throw new NullReferenceException("Unable to load ServerConfig!");
					}
					Debug.Log("Connecting: " + string.Format("mongodb://{0}:{1}@{2}:{3}/{4}", new object[]
					{
						ExternalGameDatabase.Config.Username,
						new string('*', ExternalGameDatabase.Config.Password.Length),
						ExternalGameDatabase.Config.Host,
						ExternalGameDatabase.Config.Port,
						ExternalGameDatabase.Config.Database
					}));
					ExternalGameDatabase.m_client = new MongoClient(string.Format("mongodb://{0}:{1}@{2}:{3}/{4}", new object[]
					{
						ExternalGameDatabase.Config.Username,
						ExternalGameDatabase.Config.Password,
						ExternalGameDatabase.Config.Host,
						ExternalGameDatabase.Config.Port,
						ExternalGameDatabase.Config.Database
					}));
				}
				return ExternalGameDatabase.m_client;
			}
		}

		// Token: 0x17000678 RID: 1656
		// (get) Token: 0x06001F32 RID: 7986 RVA: 0x0011EF5C File Offset: 0x0011D15C
		public static IMongoDatabase Database
		{
			get
			{
				if (ExternalGameDatabase.m_database == null)
				{
					if (ExternalGameDatabase.Config == null)
					{
						throw new NullReferenceException("Unable to load ServerConfig!");
					}
					Debug.Log("Database: " + ExternalGameDatabase.Config.Database);
					ExternalGameDatabase.m_database = ExternalGameDatabase.Client.GetDatabase(ExternalGameDatabase.Config.Database, null);
				}
				return ExternalGameDatabase.m_database;
			}
		}

		// Token: 0x040024A4 RID: 9380
		private const string kConnectionString = "mongodb://{0}:{1}@{2}:{3}/{4}";

		// Token: 0x040024A5 RID: 9381
		private static ServerConfig m_config;

		// Token: 0x040024A6 RID: 9382
		private static MongoClient m_client;

		// Token: 0x040024A7 RID: 9383
		private static IMongoDatabase m_database;
	}
}
