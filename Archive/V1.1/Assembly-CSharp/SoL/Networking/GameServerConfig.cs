using System;
using Cysharp.Text;
using MongoDB.Bson.Serialization.Attributes;
using SoL.Networking.Database;
using SoL.Utilities;
using UnityEngine.Serialization;

namespace SoL.Networking
{
	// Token: 0x020003B6 RID: 950
	[Serializable]
	public class GameServerConfig : ConfigRecordBase
	{
		// Token: 0x060019C7 RID: 6599 RVA: 0x001077E4 File Offset: 0x001059E4
		public static GameServerConfig GetConfigFromDB()
		{
			ConfigRecord configRecord;
			return ConfigRecord.GetDeserializedConfigRecord<GameServerConfig>(ExternalGameDatabase.Database, "gameServer", out configRecord);
		}

		// Token: 0x060019C8 RID: 6600 RVA: 0x00107804 File Offset: 0x00105A04
		public string GetStartupString()
		{
			string result = string.Empty;
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				utf16ValueStringBuilder.AppendLine("[Game Server Config]");
				utf16ValueStringBuilder.AppendFormat<int>("                    MinFramerate : {0}\n", this.MinFramerate);
				utf16ValueStringBuilder.AppendFormat<int>("                    MaxFramerate : {0}\n", this.MaxFramerate);
				utf16ValueStringBuilder.AppendFormat<int>("             MaxNpcTicksPerFrame : {0}\n", this.MaxNpcTicksPerFrame);
				utf16ValueStringBuilder.AppendFormat<int>("                   StartingLevel : {0}\n", this.StartingLevel);
				utf16ValueStringBuilder.AppendFormat<int>("   PathfindingIterationsPerFrame : {0}\n", this.PathfindingIterationsPerFrame);
				utf16ValueStringBuilder.AppendFormat<bool>("     CachePathfindingDestination : {0}\n", this.CachePathfindingDestination);
				utf16ValueStringBuilder.AppendFormat<float>("        DungeonEntranceCycleTime : {0}\n", this.DungeonEntranceCycleTime);
				utf16ValueStringBuilder.AppendFormat<bool>("             UpscaleGroupMembers : {0}\n", this.UpscaleGroupMembers);
				utf16ValueStringBuilder.AppendFormat<int>("     UpscaleGroupMembersMaxLevel : {0}", this.UpscaleGroupMembersMaxLevel);
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x040020BE RID: 8382
		[BsonIgnore]
		private const string kKey = "gameServer";

		// Token: 0x040020BF RID: 8383
		[FormerlySerializedAs("LevelCurve")]
		public SuccessRewardCurve AdventuringLevelCurve;

		// Token: 0x040020C0 RID: 8384
		public SuccessRewardCurve GatheringLevelCurve;

		// Token: 0x040020C1 RID: 8385
		public SuccessRewardCurve CraftingLevelCurve;

		// Token: 0x040020C2 RID: 8386
		public int MinFramerate;

		// Token: 0x040020C3 RID: 8387
		public int MaxFramerate;

		// Token: 0x040020C4 RID: 8388
		public int MaxNpcTicksPerFrame;

		// Token: 0x040020C5 RID: 8389
		public int StartingLevel;

		// Token: 0x040020C6 RID: 8390
		public int PathfindingIterationsPerFrame;

		// Token: 0x040020C7 RID: 8391
		public bool CachePathfindingDestination;

		// Token: 0x040020C8 RID: 8392
		public float DungeonEntranceCycleTime;

		// Token: 0x040020C9 RID: 8393
		public bool UpscaleGroupMembers;

		// Token: 0x040020CA RID: 8394
		public int UpscaleGroupMembersMaxLevel;

		// Token: 0x040020CB RID: 8395
		public NullifyMemoryLeakSettings MemoryLeakSettings;
	}
}
