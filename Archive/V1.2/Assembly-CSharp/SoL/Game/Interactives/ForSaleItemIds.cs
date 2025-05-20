using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B99 RID: 2969
	public struct ForSaleItemIds : INetworkSerializable
	{
		// Token: 0x06005B80 RID: 23424 RVA: 0x001EEEB0 File Offset: 0x001ED0B0
		public BitBuffer PackData(BitBuffer buffer)
		{
			if (this.ItemIds == null)
			{
				buffer.AddInt(0);
			}
			else
			{
				buffer.AddInt(this.ItemIds.Length);
				for (int i = 0; i < this.ItemIds.Length; i++)
				{
					buffer.AddUniqueId(this.ItemIds[i]);
				}
			}
			return buffer;
		}

		// Token: 0x06005B81 RID: 23425 RVA: 0x001EEF08 File Offset: 0x001ED108
		public BitBuffer ReadData(BitBuffer buffer)
		{
			int num = buffer.ReadInt();
			this.ItemIds = new UniqueId[num];
			for (int i = 0; i < num; i++)
			{
				this.ItemIds[i] = buffer.ReadUniqueId();
			}
			return buffer;
		}

		// Token: 0x04004FDB RID: 20443
		public UniqueId[] ItemIds;
	}
}
