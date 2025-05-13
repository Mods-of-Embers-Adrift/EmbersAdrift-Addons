using System;
using System.Collections.Generic;
using NetStack.Serialization;
using SoL.Networking;
using SoL.Networking.Database;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B9B RID: 2971
	public struct BuybackItemData : INetworkSerializable
	{
		// Token: 0x06005B82 RID: 23426 RVA: 0x001EEEE8 File Offset: 0x001ED0E8
		public BitBuffer PackData(BitBuffer buffer)
		{
			if (this.Items == null || this.Items.Count == 0)
			{
				buffer.AddInt(0);
			}
			else
			{
				buffer.AddInt(this.Items.Count);
				for (int i = 0; i < this.Items.Count; i++)
				{
					this.Items[i].PackData(buffer);
				}
			}
			return buffer;
		}

		// Token: 0x06005B83 RID: 23427 RVA: 0x001EEF50 File Offset: 0x001ED150
		public BitBuffer ReadData(BitBuffer buffer)
		{
			int num = buffer.ReadInt();
			this.Items = new List<MerchantBuybackItem>(num);
			for (int i = 0; i < num; i++)
			{
				MerchantBuybackItem merchantBuybackItem = new MerchantBuybackItem();
				merchantBuybackItem.ReadData(buffer);
				this.Items.Add(merchantBuybackItem);
			}
			return buffer;
		}

		// Token: 0x04004FE0 RID: 20448
		public List<MerchantBuybackItem> Items;
	}
}
