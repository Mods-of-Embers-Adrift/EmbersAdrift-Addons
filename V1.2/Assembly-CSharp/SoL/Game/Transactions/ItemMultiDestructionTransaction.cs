using System;
using System.Runtime.CompilerServices;
using NetStack.Serialization;
using SoL.Networking;

namespace SoL.Game.Transactions
{
	// Token: 0x02000639 RID: 1593
	public struct ItemMultiDestructionTransaction : INetworkSerializable
	{
		// Token: 0x060031DC RID: 12764 RVA: 0x0015DE88 File Offset: 0x0015C088
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.Op);
			ValueTuple<UniqueId, string>[] items = this.Items;
			buffer.AddInt((items != null) ? items.Length : 0);
			if (this.Items != null)
			{
				for (int i = 0; i < this.Items.Length; i++)
				{
					buffer.AddUniqueId(this.Items[i].Item1);
					buffer.AddString(this.Items[i].Item2);
				}
			}
			buffer.AddEnum(this.Context);
			return buffer;
		}

		// Token: 0x060031DD RID: 12765 RVA: 0x0015DF14 File Offset: 0x0015C114
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Op = buffer.ReadEnum<OpCodes>();
			int num = buffer.ReadInt();
			if (num > 0)
			{
				this.Items = new ValueTuple<UniqueId, string>[num];
			}
			for (int i = 0; i < num; i++)
			{
				this.Items[i] = new ValueTuple<UniqueId, string>(buffer.ReadUniqueId(), buffer.ReadString());
			}
			this.Context = buffer.ReadEnum<ItemDestructionContext>();
			return buffer;
		}

		// Token: 0x04003079 RID: 12409
		public OpCodes Op;

		// Token: 0x0400307A RID: 12410
		[TupleElementNames(new string[]
		{
			"InstanceId",
			"SourceContainer"
		})]
		public ValueTuple<UniqueId, string>[] Items;

		// Token: 0x0400307B RID: 12411
		public ItemDestructionContext Context;
	}
}
