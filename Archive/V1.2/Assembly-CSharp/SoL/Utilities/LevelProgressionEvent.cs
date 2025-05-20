using System;
using System.Collections.Generic;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Utilities
{
	// Token: 0x020002B1 RID: 689
	public struct LevelProgressionEvent : INetworkSerializable
	{
		// Token: 0x06001490 RID: 5264 RVA: 0x000FAFB4 File Offset: 0x000F91B4
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddInt(this.EventData.Count);
			for (int i = 0; i < this.EventData.Count; i++)
			{
				this.EventData[i].PackData(buffer);
			}
			return buffer;
		}

		// Token: 0x06001491 RID: 5265 RVA: 0x000FB000 File Offset: 0x000F9200
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.EventData = StaticListPool<LevelProgressionData>.GetFromPool();
			int num = buffer.ReadInt();
			for (int i = 0; i < num; i++)
			{
				LevelProgressionData item = default(LevelProgressionData);
				item.ReadData(buffer);
				this.EventData.Add(item);
			}
			return buffer;
		}

		// Token: 0x04001CCA RID: 7370
		public List<LevelProgressionData> EventData;
	}
}
