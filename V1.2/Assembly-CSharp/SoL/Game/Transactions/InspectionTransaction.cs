using System;
using System.Collections.Generic;
using NetStack.Serialization;
using SoL.Game.Objects.Archetypes;
using SoL.Networking;
using SoL.Utilities;

namespace SoL.Game.Transactions
{
	// Token: 0x02000644 RID: 1604
	public struct InspectionTransaction : INetworkSerializable
	{
		// Token: 0x060031F2 RID: 12786 RVA: 0x0015E43C File Offset: 0x0015C63C
		public BitBuffer PackData(BitBuffer buffer)
		{
			buffer.AddEnum(this.Op);
			buffer.AddString(this.TargetName);
			if (this.Instances != null && this.Instances.Count > 0)
			{
				buffer.AddBool(true);
				buffer.AddInt(this.Instances.Count);
				for (int i = 0; i < this.Instances.Count; i++)
				{
					this.Instances[i].PackData(buffer);
				}
			}
			else
			{
				buffer.AddBool(false);
			}
			return buffer;
		}

		// Token: 0x060031F3 RID: 12787 RVA: 0x0015E4C8 File Offset: 0x0015C6C8
		public BitBuffer ReadData(BitBuffer buffer)
		{
			this.Op = buffer.ReadEnum<OpCodes>();
			this.TargetName = buffer.ReadString();
			if (buffer.ReadBool())
			{
				int num = buffer.ReadInt();
				if (this.Instances != null)
				{
					this.Instances.Clear();
				}
				else
				{
					this.Instances = StaticListPool<ArchetypeInstance>.GetFromPool();
				}
				for (int i = 0; i < num; i++)
				{
					ArchetypeInstance fromPool = StaticPool<ArchetypeInstance>.GetFromPool();
					fromPool.ReadData(buffer);
					this.Instances.Add(fromPool);
				}
			}
			return buffer;
		}

		// Token: 0x0400309D RID: 12445
		public OpCodes Op;

		// Token: 0x0400309E RID: 12446
		public string TargetName;

		// Token: 0x0400309F RID: 12447
		public List<ArchetypeInstance> Instances;
	}
}
