using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using NetStack.Serialization;
using Newtonsoft.Json;
using SoL.Networking;

namespace SoL.Game.Quests
{
	// Token: 0x0200078B RID: 1931
	[Serializable]
	public class QuestProgressionData : INetworkSerializable
	{
		// Token: 0x06003904 RID: 14596 RVA: 0x0017176C File Offset: 0x0016F96C
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.CurrentNodeId);
			List<ObjectiveProgressionData> objectives = this.Objectives;
			buffer.AddInt((objectives != null) ? objectives.Count : 0);
			if (this.Objectives != null)
			{
				foreach (ObjectiveProgressionData objectiveProgressionData in this.Objectives)
				{
					objectiveProgressionData.PackData(buffer);
				}
			}
			buffer.AddBool(this.Muted);
			return buffer;
		}

		// Token: 0x06003905 RID: 14597 RVA: 0x001717FC File Offset: 0x0016F9FC
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.CurrentNodeId = buffer.ReadUniqueId();
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
			this.Muted = buffer.ReadBool();
			return buffer;
		}

		// Token: 0x06003906 RID: 14598 RVA: 0x00171860 File Offset: 0x0016FA60
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

		// Token: 0x040037DA RID: 14298
		public UniqueId CurrentNodeId;

		// Token: 0x040037DB RID: 14299
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<ObjectiveProgressionData> Objectives;

		// Token: 0x040037DC RID: 14300
		[BsonIgnoreIfDefault]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
		public bool Muted;
	}
}
