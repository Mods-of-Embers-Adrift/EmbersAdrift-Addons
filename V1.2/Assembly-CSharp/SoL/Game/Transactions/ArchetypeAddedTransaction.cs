using System;
using NetStack.Serialization;
using SoL.Game.Objects.Archetypes;
using SoL.Networking;
using SoL.Utilities;

namespace SoL.Game.Transactions
{
	// Token: 0x0200063A RID: 1594
	public struct ArchetypeAddedTransaction : INetworkSerializable
	{
		// Token: 0x060031DE RID: 12766 RVA: 0x0015DF7C File Offset: 0x0015C17C
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.Op);
			buffer.AddBool(this.Instance != null);
			ArchetypeInstance instance = this.Instance;
			if (instance != null)
			{
				instance.PackData(buffer);
			}
			buffer.AddString(this.TargetContainer);
			buffer.AddEnum(this.Context);
			return buffer;
		}

		// Token: 0x060031DF RID: 12767 RVA: 0x0015DFD4 File Offset: 0x0015C1D4
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Op = buffer.ReadEnum<OpCodes>();
			if (buffer.ReadBool())
			{
				this.Instance = StaticPool<ArchetypeInstance>.GetFromPool();
				this.Instance.ReadData(buffer);
			}
			this.TargetContainer = buffer.ReadString();
			this.Context = buffer.ReadEnum<ItemAddContext>();
			return buffer;
		}

		// Token: 0x0400307C RID: 12412
		public OpCodes Op;

		// Token: 0x0400307D RID: 12413
		public ArchetypeInstance Instance;

		// Token: 0x0400307E RID: 12414
		public string TargetContainer;

		// Token: 0x0400307F RID: 12415
		public ItemAddContext Context;
	}
}
