using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Transactions
{
	// Token: 0x0200063C RID: 1596
	public struct LearnablesAddedTransaction : INetworkSerializable
	{
		// Token: 0x060031E2 RID: 12770 RVA: 0x0015E170 File Offset: 0x0015C370
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.Op);
			int num = (this.LearnableIds == null) ? 0 : this.LearnableIds.Length;
			buffer.AddInt(num);
			if (num > 0)
			{
				for (int i = 0; i < this.LearnableIds.Length; i++)
				{
					buffer.AddUniqueId(this.LearnableIds[i]);
				}
			}
			buffer.AddString(this.TargetContainer);
			return buffer;
		}

		// Token: 0x060031E3 RID: 12771 RVA: 0x0015E1E0 File Offset: 0x0015C3E0
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Op = buffer.ReadEnum<OpCodes>();
			int num = buffer.ReadInt();
			if (num > 0)
			{
				this.LearnableIds = new UniqueId[num];
				for (int i = 0; i < num; i++)
				{
					this.LearnableIds[i] = buffer.ReadUniqueId();
				}
			}
			this.TargetContainer = buffer.ReadString();
			return buffer;
		}

		// Token: 0x04003083 RID: 12419
		public OpCodes Op;

		// Token: 0x04003084 RID: 12420
		public UniqueId[] LearnableIds;

		// Token: 0x04003085 RID: 12421
		public string TargetContainer;
	}
}
