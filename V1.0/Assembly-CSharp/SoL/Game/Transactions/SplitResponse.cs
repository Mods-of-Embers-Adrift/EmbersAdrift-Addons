using System;
using NetStack.Serialization;
using SoL.Game.Objects.Archetypes;
using SoL.Networking;
using SoL.Utilities;

namespace SoL.Game.Transactions
{
	// Token: 0x02000630 RID: 1584
	public struct SplitResponse : INetworkSerializable
	{
		// Token: 0x060031C5 RID: 12741 RVA: 0x0015DB6C File Offset: 0x0015BD6C
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.Op);
			buffer.AddUniqueId(this.TransactionId);
			buffer.AddBool(this.Instance != null);
			ArchetypeInstance instance = this.Instance;
			if (instance != null)
			{
				instance.PackData(buffer);
			}
			buffer.AddString(this.TargetContainer);
			return buffer;
		}

		// Token: 0x060031C6 RID: 12742 RVA: 0x0015DBC4 File Offset: 0x0015BDC4
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Op = buffer.ReadEnum<OpCodes>();
			this.TransactionId = buffer.ReadUniqueId();
			if (buffer.ReadBool())
			{
				this.Instance = StaticPool<ArchetypeInstance>.GetFromPool();
				this.Instance.ReadData(buffer);
			}
			this.TargetContainer = buffer.ReadString();
			return buffer;
		}

		// Token: 0x04003056 RID: 12374
		public OpCodes Op;

		// Token: 0x04003057 RID: 12375
		public UniqueId TransactionId;

		// Token: 0x04003058 RID: 12376
		public ArchetypeInstance Instance;

		// Token: 0x04003059 RID: 12377
		public string TargetContainer;
	}
}
