using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using SoL.Game.Discovery;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Networking.Database
{
	// Token: 0x02000417 RID: 1047
	[BsonIgnoreExtraElements]
	public class ActiveMonolithRecord : IEquatable<ActiveMonolithRecord>
	{
		// Token: 0x06001E45 RID: 7749 RVA: 0x00119AD8 File Offset: 0x00117CD8
		public static ActiveMonolithRecord Load(IMongoDatabase db, string id)
		{
			IMongoCollection<ActiveMonolithRecord> collection = db.GetCollection<ActiveMonolithRecord>("active_monoliths", null);
			FilterDefinition<ActiveMonolithRecord> filter = Builders<ActiveMonolithRecord>.Filter.Eq<string>((ActiveMonolithRecord amr) => amr.Id, id);
			return collection.Find(filter, null).FirstOrDefault(default(CancellationToken));
		}

		// Token: 0x06001E46 RID: 7750 RVA: 0x00119B48 File Offset: 0x00117D48
		public static IEnumerable<ActiveMonolithRecord> LoadAll(IMongoDatabase db)
		{
			return db.GetCollection<ActiveMonolithRecord>("active_monoliths", null).Find(FilterDefinition<ActiveMonolithRecord>.Empty, null).ToList(default(CancellationToken));
		}

		// Token: 0x06001E47 RID: 7751 RVA: 0x00119B7C File Offset: 0x00117D7C
		public static ActiveMonolithRecord StoreNew(IMongoDatabase db, MonolithProfile profile)
		{
			if (!profile || !profile.RequiresActiveRecord)
			{
				return null;
			}
			ActiveMonolithRecord activeMonolithRecord = new ActiveMonolithRecord
			{
				Id = profile.Id,
				Description = profile.Description,
				Updated = DateTime.UtcNow,
				ExpirationTime = DateTime.UtcNow.AddDays((double)profile.ActiveTimeInDays)
			};
			db.GetCollection<ActiveMonolithRecord>("active_monoliths", null).InsertOne(activeMonolithRecord, null, default(CancellationToken));
			string text = ZString.Format<string, string>("[MONGO] Inserted ActiveMonolithRecord for {0} ({1})", activeMonolithRecord.Description, activeMonolithRecord.Id);
			Debug.Log(text);
			if (GameManager.Instance && DeploymentBranchFlagsExtensions.GetBranchFlags() == DeploymentBranchFlags.LIVE)
			{
				GameManager.Instance.SendMessageToStatusChannel(text);
			}
			return activeMonolithRecord;
		}

		// Token: 0x06001E48 RID: 7752 RVA: 0x00119C3C File Offset: 0x00117E3C
		public bool Delete(IMongoDatabase db)
		{
			IMongoCollection<ActiveMonolithRecord> collection = db.GetCollection<ActiveMonolithRecord>("active_monoliths", null);
			FilterDefinition<ActiveMonolithRecord> filter = Builders<ActiveMonolithRecord>.Filter.Eq<string>((ActiveMonolithRecord amr) => amr.Id, this.Id);
			DeleteResult deleteResult = collection.DeleteOne(filter, default(CancellationToken));
			string text = ZString.Format<string, string, bool>("[MONGO] Deleted ActiveMonolithRecord for {0} ({1}) {2}", this.Description, this.Id, deleteResult.DeletedCount > 0L);
			Debug.Log(text);
			if (GameManager.Instance && DeploymentBranchFlagsExtensions.GetBranchFlags() == DeploymentBranchFlags.LIVE)
			{
				GameManager.Instance.SendMessageToStatusChannel(text);
			}
			return deleteResult.DeletedCount > 0L;
		}

		// Token: 0x06001E49 RID: 7753 RVA: 0x000569ED File Offset: 0x00054BED
		public bool Equals(ActiveMonolithRecord other)
		{
			return other != null && (this == other || this.Id == other.Id);
		}

		// Token: 0x06001E4A RID: 7754 RVA: 0x00056A0B File Offset: 0x00054C0B
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((ActiveMonolithRecord)obj)));
		}

		// Token: 0x06001E4B RID: 7755 RVA: 0x00056A39 File Offset: 0x00054C39
		public override int GetHashCode()
		{
			if (this.Id == null)
			{
				return 0;
			}
			return this.Id.GetHashCode();
		}

		// Token: 0x04002386 RID: 9094
		[BsonId]
		[BsonRepresentation(BsonType.String)]
		public string Id;

		// Token: 0x04002387 RID: 9095
		public string Description;

		// Token: 0x04002388 RID: 9096
		public DateTime Updated;

		// Token: 0x04002389 RID: 9097
		public DateTime ExpirationTime;

		// Token: 0x0400238A RID: 9098
		private const string kTableName = "active_monoliths";
	}
}
