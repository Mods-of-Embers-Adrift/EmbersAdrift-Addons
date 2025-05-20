using System;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Transactions
{
	// Token: 0x0200063B RID: 1595
	public struct ArchetypeAddRemoveTransaction : INetworkSerializable
	{
		// Token: 0x060031E0 RID: 12768 RVA: 0x0015E028 File Offset: 0x0015C228
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.Op);
			int num = (this.DestructionTransactions == null) ? 0 : this.DestructionTransactions.Length;
			buffer.AddInt(num);
			if (num > 0)
			{
				for (int i = 0; i < this.DestructionTransactions.Length; i++)
				{
					this.DestructionTransactions[i].PackData(buffer);
				}
			}
			num = ((this.AddedTransactions == null) ? 0 : this.AddedTransactions.Length);
			buffer.AddInt(num);
			if (num > 0)
			{
				for (int j = 0; j < this.AddedTransactions.Length; j++)
				{
					this.AddedTransactions[j].PackData(buffer);
				}
			}
			return buffer;
		}

		// Token: 0x060031E1 RID: 12769 RVA: 0x0015E0D0 File Offset: 0x0015C2D0
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Op = buffer.ReadEnum<OpCodes>();
			int num = buffer.ReadInt();
			if (num > 0)
			{
				this.DestructionTransactions = new ItemDestructionTransaction[num];
				for (int i = 0; i < num; i++)
				{
					ItemDestructionTransaction itemDestructionTransaction = default(ItemDestructionTransaction);
					itemDestructionTransaction.ReadData(buffer);
					this.DestructionTransactions[i] = itemDestructionTransaction;
				}
			}
			num = buffer.ReadInt();
			if (num > 0)
			{
				this.AddedTransactions = new ArchetypeAddedTransaction[num];
				for (int j = 0; j < num; j++)
				{
					ArchetypeAddedTransaction archetypeAddedTransaction = default(ArchetypeAddedTransaction);
					archetypeAddedTransaction.ReadData(buffer);
					this.AddedTransactions[j] = archetypeAddedTransaction;
				}
			}
			return buffer;
		}

		// Token: 0x04003080 RID: 12416
		public OpCodes Op;

		// Token: 0x04003081 RID: 12417
		public ItemDestructionTransaction[] DestructionTransactions;

		// Token: 0x04003082 RID: 12418
		public ArchetypeAddedTransaction[] AddedTransactions;
	}
}
