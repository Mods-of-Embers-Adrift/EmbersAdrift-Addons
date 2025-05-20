using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using NetStack.Serialization;
using Newtonsoft.Json;
using SoL.Networking;

namespace SoL.Game.Quests
{
	// Token: 0x0200077C RID: 1916
	public class BBTaskProgressionData : INetworkSerializable
	{
		// Token: 0x06003889 RID: 14473 RVA: 0x0016DF98 File Offset: 0x0016C198
		public BitBuffer PackData(BitBuffer buffer)
		{
			List<ObjectiveProgressionData> objectives = this.Objectives;
			buffer.AddInt((objectives != null) ? objectives.Count : 0);
			if (this.Objectives != null)
			{
				foreach (ObjectiveProgressionData objectiveProgressionData in this.Objectives)
				{
					objectiveProgressionData.PackData(buffer);
				}
			}
			return buffer;
		}

		// Token: 0x0600388A RID: 14474 RVA: 0x0016E00C File Offset: 0x0016C20C
		public BitBuffer ReadData(BitBuffer buffer)
		{
			int num = buffer.ReadInt();
			if (num > 0)
			{
				this.Objectives = new List<ObjectiveProgressionData>(num);
				for (int i = 0; i < num; i++)
				{
					ObjectiveProgressionData objectiveProgressionData = new ObjectiveProgressionData();
					objectiveProgressionData.ReadData(buffer);
					this.Objectives.Add(objectiveProgressionData);
				}
			}
			return buffer;
		}

		// Token: 0x0600388B RID: 14475 RVA: 0x0016E058 File Offset: 0x0016C258
		public bool TryGetObjective(UniqueId id, out ObjectiveProgressionData data)
		{
			if (this.Objectives != null)
			{
				foreach (ObjectiveProgressionData objectiveProgressionData in this.Objectives)
				{
					if (objectiveProgressionData.ObjectiveId == id)
					{
						data = objectiveProgressionData;
						return true;
					}
				}
			}
			data = null;
			return false;
		}

		// Token: 0x04003757 RID: 14167
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<ObjectiveProgressionData> Objectives;
	}
}
