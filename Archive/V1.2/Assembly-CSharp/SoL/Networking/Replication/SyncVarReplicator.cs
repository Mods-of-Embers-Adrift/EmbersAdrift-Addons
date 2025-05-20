using System;
using System.Collections.Generic;
using ENet;
using NetStack.Serialization;

namespace SoL.Networking.Replication
{
	// Token: 0x020004A7 RID: 1191
	public abstract class SyncVarReplicator : Replicator
	{
		// Token: 0x170006CD RID: 1741
		// (get) Token: 0x0600212E RID: 8494 RVA: 0x000580DD File Offset: 0x000562DD
		public override ReplicatorTypes Type
		{
			get
			{
				return ReplicatorTypes.SyncVar;
			}
		}

		// Token: 0x0600212F RID: 8495 RVA: 0x00122CF0 File Offset: 0x00120EF0
		protected override void OnDestroy()
		{
			if (NullifyMemoryLeakSettings.CleanSyncVarMonoRefs)
			{
				for (int i = 0; i < this.m_syncs.Count; i++)
				{
					this.m_syncs[i].ClearMonoReferences();
				}
				this.m_syncs.Clear();
			}
			base.OnDestroy();
		}

		// Token: 0x06002130 RID: 8496 RVA: 0x00122D3C File Offset: 0x00120F3C
		public override void SetDirtyFlags(DateTime timestamp)
		{
			if (this.m_netEntity == null || !this.m_netEntity.IsServer)
			{
				base.Dirty = false;
				return;
			}
			this.m_dirtyBits = this.GetDirtyBits(timestamp);
			base.Dirty = (this.m_dirtyBits != 0);
		}

		// Token: 0x06002131 RID: 8497 RVA: 0x00122D88 File Offset: 0x00120F88
		private int GetDirtyBits(DateTime timestamp)
		{
			int num = 0;
			for (int i = 0; i < this.m_syncs.Count; i++)
			{
				ISynchronizedVariable synchronizedVariable = this.m_syncs[i];
				synchronizedVariable.SetDirtyFlags(timestamp);
				if (synchronizedVariable.Dirty)
				{
					num |= synchronizedVariable.BitFlag;
				}
			}
			return num;
		}

		// Token: 0x06002132 RID: 8498 RVA: 0x00122DD4 File Offset: 0x00120FD4
		public override BitBuffer PackData(BitBuffer outBuffer)
		{
			outBuffer = base.PackData(outBuffer);
			outBuffer.AddInt(this.m_dirtyBits);
			for (int i = 0; i < this.m_syncs.Count; i++)
			{
				if (this.m_syncs[i].Dirty)
				{
					outBuffer.AddSyncData(this.m_syncs[i]);
				}
			}
			return outBuffer;
		}

		// Token: 0x06002133 RID: 8499 RVA: 0x00122E34 File Offset: 0x00121034
		public override BitBuffer ReadData(BitBuffer inBuffer)
		{
			inBuffer = base.ReadData(inBuffer);
			int num = inBuffer.ReadInt();
			if (num != 0)
			{
				for (int i = 0; i < this.m_syncs.Count; i++)
				{
					if ((num & this.m_syncs[i].BitFlag) == this.m_syncs[i].BitFlag)
					{
						this.m_syncs[i].ReadData(inBuffer);
					}
				}
			}
			return inBuffer;
		}

		// Token: 0x06002134 RID: 8500 RVA: 0x00122EA4 File Offset: 0x001210A4
		public override BitBuffer PackInitialData(BitBuffer outBuffer)
		{
			outBuffer = base.PackInitialData(outBuffer);
			int num = 0;
			for (int i = 0; i < this.m_syncs.Count; i++)
			{
				if (!this.m_syncs[i].IsDefault)
				{
					num |= this.m_syncs[i].BitFlag;
				}
			}
			outBuffer.AddInt(num);
			for (int j = 0; j < this.m_syncs.Count; j++)
			{
				if ((num & this.m_syncs[j].BitFlag) == this.m_syncs[j].BitFlag)
				{
					this.m_syncs[j].PackInitialData(outBuffer);
				}
			}
			return outBuffer;
		}

		// Token: 0x06002135 RID: 8501 RVA: 0x00122F54 File Offset: 0x00121154
		public override BitBuffer ReadInitialData(BitBuffer inBuffer)
		{
			inBuffer = base.ReadInitialData(inBuffer);
			int num = inBuffer.ReadInt();
			for (int i = 0; i < this.m_syncs.Count; i++)
			{
				if ((num & this.m_syncs[i].BitFlag) == this.m_syncs[i].BitFlag)
				{
					this.m_syncs[i].ReadData(inBuffer);
				}
			}
			return inBuffer;
		}

		// Token: 0x06002136 RID: 8502 RVA: 0x00122FC4 File Offset: 0x001211C4
		public void SetClientVariable(ISynchronizedVariable syncVar)
		{
			BitBuffer fromPool = BitBufferExtensions.GetFromPool();
			fromPool.AddHeader(this.m_netEntity, OpCodes.ClientSyncUpdate, true);
			fromPool.AddInt(this.m_index);
			fromPool.AddInt(syncVar.BitFlag);
			syncVar.PackData(fromPool);
			Packet packetFromBuffer_ReturnBufferToPool = fromPool.GetPacketFromBuffer_ReturnBufferToPool(PacketFlags.Reliable);
			NetworkCommand networkCommand = NetworkCommandPool.GetNetworkCommand();
			networkCommand.Type = CommandType.Send;
			networkCommand.Packet = packetFromBuffer_ReturnBufferToPool;
			networkCommand.Channel = NetworkChannel.SyncVar_Client;
			networkCommand.Source = this.m_netEntity.NetworkId.Peer;
			this.m_network.AddCommandToQueue(networkCommand);
		}

		// Token: 0x06002137 RID: 8503 RVA: 0x00123050 File Offset: 0x00121250
		public void ReadClientVariable(BitBuffer buffer)
		{
			int num = buffer.ReadInt();
			for (int i = 0; i < this.m_syncs.Count; i++)
			{
				if (this.m_syncs[i].BitFlag == num)
				{
					this.m_syncs[i].ReadDataFromClient(buffer);
					return;
				}
			}
		}

		// Token: 0x040025AE RID: 9646
		protected readonly List<ISynchronizedVariable> m_syncs = new List<ISynchronizedVariable>(20);

		// Token: 0x040025AF RID: 9647
		private int m_dirtyBits;
	}
}
