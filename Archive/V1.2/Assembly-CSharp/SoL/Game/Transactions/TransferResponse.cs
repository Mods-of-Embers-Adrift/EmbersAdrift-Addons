using System;
using NetStack.Serialization;
using SoL.Game.Objects.Archetypes;
using SoL.Networking;
using SoL.Utilities;

namespace SoL.Game.Transactions
{
	// Token: 0x02000632 RID: 1586
	public struct TransferResponse : INetworkSerializable
	{
		// Token: 0x060031CA RID: 12746 RVA: 0x0015DC88 File Offset: 0x0015BE88
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddUniqueId(this.TransactionId);
			buffer.AddEnum(this.Op);
			buffer.AddInt(this.TargetIndex);
			bool flag = this.Instance != null;
			buffer.AddBool(flag);
			if (flag)
			{
				this.Instance.PackData(buffer);
			}
			return buffer;
		}

		// Token: 0x060031CB RID: 12747 RVA: 0x0015DCE0 File Offset: 0x0015BEE0
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.TransactionId = buffer.ReadUniqueId();
			this.Op = buffer.ReadEnum<OpCodes>();
			this.TargetIndex = buffer.ReadInt();
			if (buffer.ReadBool())
			{
				this.Instance = StaticPool<ArchetypeInstance>.GetFromPool();
				this.Instance.ReadData(buffer);
			}
			return buffer;
		}

		// Token: 0x04003060 RID: 12384
		public OpCodes Op;

		// Token: 0x04003061 RID: 12385
		public UniqueId TransactionId;

		// Token: 0x04003062 RID: 12386
		public int TargetIndex;

		// Token: 0x04003063 RID: 12387
		public ArchetypeInstance Instance;
	}
}
