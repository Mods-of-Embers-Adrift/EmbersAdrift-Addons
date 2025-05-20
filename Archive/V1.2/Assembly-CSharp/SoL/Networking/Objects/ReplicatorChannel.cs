using System;
using System.Collections.Generic;
using ENet;
using NetStack.Serialization;
using SoL.Managers;
using SoL.Networking.Managers;
using SoL.Networking.Proximity;
using SoL.Networking.Replication;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Networking.Objects
{
	// Token: 0x020004BD RID: 1213
	[Serializable]
	public class ReplicatorChannel
	{
		// Token: 0x170006F5 RID: 1781
		// (get) Token: 0x060021ED RID: 8685 RVA: 0x00058852 File Offset: 0x00056A52
		// (set) Token: 0x060021EE RID: 8686 RVA: 0x0005885A File Offset: 0x00056A5A
		public List<IReplicator> Replicators { get; private set; }

		// Token: 0x060021EF RID: 8687 RVA: 0x0012595C File Offset: 0x00123B5C
		public void Init(bool syncVars, INetworkManager network, NetworkEntity networkEntity)
		{
			this.m_network = network;
			this.m_networkEntity = networkEntity;
			if (syncVars)
			{
				this.m_includeSelf = true;
				this.m_opCode = OpCodes.SyncUpdate;
				this.m_packetFlags = PacketFlags.Reliable;
				this.m_networkChannel = (GameManager.IsServer ? NetworkChannel.SyncVar_Server : NetworkChannel.SyncVar_Client);
			}
			else
			{
				this.m_opCode = OpCodes.StateUpdate;
				this.m_networkChannel = (GameManager.IsServer ? NetworkChannel.State_Server : NetworkChannel.State_Client);
			}
			this.Replicators = StaticListPool<IReplicator>.GetFromPool();
			if (this.m_replicators == null || this.m_replicators.Length == 0)
			{
				return;
			}
			for (int i = 0; i < this.m_replicators.Length; i++)
			{
				ClientServerReplicator.SlotType type = this.m_replicators[i].Type;
				Replicator replicator;
				if (type != ClientServerReplicator.SlotType.Universal)
				{
					if (type != ClientServerReplicator.SlotType.Individual)
					{
						throw new ArgumentException("Invalid type " + this.m_replicators[i].Type.ToString());
					}
					if (this.m_networkEntity.IsServer)
					{
						replicator = this.m_replicators[i].Server;
					}
					else if (this.m_networkEntity.IsLocal)
					{
						replicator = this.m_replicators[i].LocalClient;
					}
					else
					{
						replicator = this.m_replicators[i].Client;
					}
				}
				else
				{
					replicator = this.m_replicators[i].Universal;
				}
				if (replicator == null)
				{
					throw new NullReferenceException(string.Concat(new string[]
					{
						"Missing Replicator reference on ",
						networkEntity.gameObject.name,
						"!  (",
						networkEntity.gameObject.transform.GetPath(),
						")"
					}));
				}
				replicator.Init(i, network, networkEntity);
				this.Replicators.Add(replicator);
			}
		}

		// Token: 0x060021F0 RID: 8688 RVA: 0x00058863 File Offset: 0x00056A63
		public void OnDestroy()
		{
			if (this.Replicators != null)
			{
				StaticListPool<IReplicator>.ReturnToPool(this.Replicators);
			}
			this.m_network = null;
			this.m_networkEntity = null;
		}

		// Token: 0x060021F1 RID: 8689 RVA: 0x00125B10 File Offset: 0x00123D10
		public void AddInitialState(BitBuffer buffer)
		{
			for (int i = 0; i < this.Replicators.Count; i++)
			{
				this.Replicators[i].PackInitialData(buffer);
			}
		}

		// Token: 0x060021F2 RID: 8690 RVA: 0x00125B48 File Offset: 0x00123D48
		public void ReadInitialState(BitBuffer buffer)
		{
			for (int i = 0; i < this.Replicators.Count; i++)
			{
				this.Replicators[i].ReadInitialData(buffer);
			}
		}

		// Token: 0x060021F3 RID: 8691 RVA: 0x00125B80 File Offset: 0x00123D80
		public void ReadReplicationData(BitBuffer buffer)
		{
			int num = buffer.ReadInt();
			for (int i = 0; i < this.Replicators.Count; i++)
			{
				int num2 = 1 << i;
				if ((num & num2) == num2)
				{
					this.Replicators[i].ReadData(buffer);
				}
			}
		}

		// Token: 0x060021F4 RID: 8692 RVA: 0x00125BCC File Offset: 0x00123DCC
		public void WriteReplicationData(DateTime timestamp, DistanceBand band, BitBuffer buffer)
		{
			int num = 0;
			for (int i = 0; i < this.Replicators.Count; i++)
			{
				if (!this.m_networkEntity.IsLocal || this.Replicators[i].Type.LocalClientIsAuthority())
				{
					this.Replicators[i].SetDirtyFlags(timestamp);
					if (this.Replicators[i].Dirty)
					{
						num |= 1 << i;
					}
				}
			}
			if (num == 0)
			{
				return;
			}
			Peer[] array = null;
			CommandType type;
			if (this.m_networkEntity.UseProximity && band != null)
			{
				type = CommandType.BroadcastGroup;
				array = this.m_networkEntity.GetObserversFromDistanceBand(band, this.m_includeSelf);
				if (array == null)
				{
					return;
				}
				if (array.Length == 0)
				{
					array.ReturnToPool();
					return;
				}
			}
			else
			{
				type = (this.m_networkEntity.IsLocal ? CommandType.Send : CommandType.BroadcastAll);
			}
			buffer.AddHeader(this.m_networkEntity, this.m_opCode, true);
			buffer.AddInt(num);
			for (int j = 0; j < this.Replicators.Count; j++)
			{
				if (this.Replicators[j].Dirty)
				{
					this.Replicators[j].PackData(buffer);
				}
			}
			Packet packetFromBuffer = buffer.GetPacketFromBuffer(this.m_packetFlags);
			NetworkCommand networkCommand = NetworkCommandPool.GetNetworkCommand();
			networkCommand.Packet = packetFromBuffer;
			networkCommand.Channel = this.m_networkChannel;
			networkCommand.Source = this.m_networkEntity.NetworkId.Peer;
			networkCommand.Type = type;
			networkCommand.TargetGroup = array;
			this.m_network.AddCommandToQueue(networkCommand);
		}

		// Token: 0x04002634 RID: 9780
		[SerializeField]
		private ClientServerReplicator[] m_replicators;

		// Token: 0x04002636 RID: 9782
		private bool m_includeSelf;

		// Token: 0x04002637 RID: 9783
		private INetworkManager m_network;

		// Token: 0x04002638 RID: 9784
		private NetworkEntity m_networkEntity;

		// Token: 0x04002639 RID: 9785
		private OpCodes m_opCode;

		// Token: 0x0400263A RID: 9786
		private NetworkChannel m_networkChannel;

		// Token: 0x0400263B RID: 9787
		private PacketFlags m_packetFlags;
	}
}
