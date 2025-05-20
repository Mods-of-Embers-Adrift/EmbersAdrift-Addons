using System;
using System.Collections.Generic;
using NetStack.Serialization;
using SoL.Game.Objects.Archetypes;
using SoL.Networking;
using SoL.Utilities;

namespace SoL.Game.Transactions
{
	// Token: 0x02000646 RID: 1606
	public struct SendMailResponse : INetworkSerializable
	{
		// Token: 0x060031F6 RID: 12790 RVA: 0x0015E574 File Offset: 0x0015C774
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.OpCode);
			if (this.OpCode == OpCodes.Ok)
			{
				buffer.AddString(this.MailId);
				buffer.AddString(this.Recipient);
				if (this.ItemAttachments != null)
				{
					int count = this.ItemAttachments.Count;
					buffer.AddInt(count);
					for (int i = 0; i < count; i++)
					{
						this.ItemAttachments[i].PackData(buffer);
					}
				}
				else
				{
					buffer.AddInt(0);
				}
			}
			else
			{
				buffer.AddString(this.Error);
			}
			return buffer;
		}

		// Token: 0x060031F7 RID: 12791 RVA: 0x0015E608 File Offset: 0x0015C808
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.OpCode = buffer.ReadEnum<OpCodes>();
			if (this.OpCode == OpCodes.Ok)
			{
				this.MailId = buffer.ReadString();
				this.Recipient = buffer.ReadString();
				int num = buffer.ReadInt();
				if (num > 0)
				{
					this.ItemAttachments = StaticListPool<ArchetypeInstance>.GetFromPool();
					for (int i = 0; i < num; i++)
					{
						ArchetypeInstance fromPool = StaticPool<ArchetypeInstance>.GetFromPool();
						fromPool.ReadData(buffer);
						this.ItemAttachments.Add(fromPool);
					}
				}
			}
			else
			{
				this.Error = buffer.ReadString();
			}
			return buffer;
		}

		// Token: 0x040030A5 RID: 12453
		public OpCodes OpCode;

		// Token: 0x040030A6 RID: 12454
		public string MailId;

		// Token: 0x040030A7 RID: 12455
		public string Recipient;

		// Token: 0x040030A8 RID: 12456
		public List<ArchetypeInstance> ItemAttachments;

		// Token: 0x040030A9 RID: 12457
		public string Error;
	}
}
