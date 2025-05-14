using System;
using System.Collections.Generic;
using ENet;
using NetStack.Serialization;
using SoL.Game;
using SoL.Game.Dueling;
using SoL.Managers;
using SoL.Networking.Objects;
using SoL.Networking.RPC;
using SoL.Utilities;
using SoL.Utilities.Logging;
using UnityEngine;

namespace SoL.Networking.Managers
{
	// Token: 0x020004C1 RID: 1217
	public sealed class ClientNetworkManager : NetworkManager
	{
		// Token: 0x170006FB RID: 1787
		// (get) Token: 0x06002221 RID: 8737 RVA: 0x00126168 File Offset: 0x00124368
		public static bool HasReceivedBulkSpawn
		{
			get
			{
				if (NetworkManager.Instance)
				{
					ClientNetworkManager clientNetworkManager = NetworkManager.Instance as ClientNetworkManager;
					if (clientNetworkManager != null)
					{
						return clientNetworkManager.m_bulkSpawnReceived;
					}
				}
				return false;
			}
		}

		// Token: 0x06002222 RID: 8738 RVA: 0x000589C2 File Offset: 0x00056BC2
		protected override void PreConnect()
		{
			base.PreConnect();
			this.m_bulkSpawnReceived = false;
		}

		// Token: 0x06002223 RID: 8739 RVA: 0x000589D1 File Offset: 0x00056BD1
		private void Start()
		{
			NetworkManager.EntityManager = base.gameObject.AddComponent<ClientNetworkEntityManager>();
		}

		// Token: 0x06002224 RID: 8740 RVA: 0x000589E3 File Offset: 0x00056BE3
		protected override void Update()
		{
			base.Update();
			this.ProcessSpawnQueue();
		}

		// Token: 0x06002225 RID: 8741 RVA: 0x000589F1 File Offset: 0x00056BF1
		protected override void LoadZoneRecord(ZoneId zoneId)
		{
			LocalZoneManager.ZoneRecord = SessionData.GetZoneRecord(zoneId);
		}

		// Token: 0x06002226 RID: 8742 RVA: 0x000589FE File Offset: 0x00056BFE
		protected override bool ValidateZoneConnectionData()
		{
			if (LocalZoneManager.ZoneRecord.Port == 0)
			{
				LoginApiManager.PerformZoneCheck((ZoneId)LocalZoneManager.ZoneRecord.ZoneId, new Action<bool>(this.ZoneCheckCallback));
				return false;
			}
			return true;
		}

		// Token: 0x06002227 RID: 8743 RVA: 0x00126198 File Offset: 0x00124398
		protected override void Func_StartHost(Host host, NetworkCommand command)
		{
			this.m_disconnectSent = false;
			Debug.Log("STARTING CLIENT FROM NETWORK THREAD");
			try
			{
				Address address = new Address
				{
					Port = command.ConnectionSettings.Port
				};
				address.SetHost(command.ConnectionSettings.Host);
				host.Create();
				this.m_peer = host.Connect(address, command.ConnectionSettings.ChannelCount);
				NetworkManager.MyPeer = this.m_peer;
				Debug.Log(string.Format("Client connecting at {0}:{1} with {2} channels.", command.ConnectionSettings.Host, command.ConnectionSettings.Port, command.ConnectionSettings.ChannelCount));
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}

		// Token: 0x06002228 RID: 8744 RVA: 0x00058A2A File Offset: 0x00056C2A
		protected override void Func_StopHost(Host host, NetworkCommand command)
		{
			this.m_disconnectSent = true;
			Debug.Log("STOPPING CLIENT FROM NETWORK THREAD");
			if (this.m_peer.IsSet && this.m_peer.State == PeerState.Connected)
			{
				this.m_peer.Disconnect(5U);
			}
		}

		// Token: 0x06002229 RID: 8745 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void Func_DisconnectClient(Host host, NetworkCommand command)
		{
		}

		// Token: 0x0600222A RID: 8746 RVA: 0x00058A64 File Offset: 0x00056C64
		protected override bool Func_Send(Host host, NetworkCommand command)
		{
			return !this.m_disconnectSent && command != null && this.m_peer.IsSet && this.m_peer.Send(command.Channel.GetByte(), ref command.Packet);
		}

		// Token: 0x0600222B RID: 8747 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void Func_BroadcastAll(Host host, NetworkCommand command)
		{
		}

		// Token: 0x0600222C RID: 8748 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void Func_BroadcastOthers(Host host, NetworkCommand command)
		{
		}

		// Token: 0x0600222D RID: 8749 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void Func_BroadcastGroup(Host host, NetworkCommand command)
		{
		}

		// Token: 0x0600222E RID: 8750 RVA: 0x00058A9C File Offset: 0x00056C9C
		protected override void Connect(ENet.Event netEvent)
		{
			ClientNetworkManager.DisconnectInitiatedByClient = false;
		}

		// Token: 0x0600222F RID: 8751 RVA: 0x00126264 File Offset: 0x00124464
		protected override void Disconnect(ENet.Event netEvent)
		{
			base.CloseConnection();
			Debug.Log("Disconnected - Reason: " + netEvent.Data.ToString() + " EventType: " + netEvent.Type.ToString());
			if (!ClientNetworkManager.DisconnectInitiatedByClient)
			{
				GameManager.SceneCompositionManager.ReloadStartupScene();
			}
		}

		// Token: 0x06002230 RID: 8752 RVA: 0x001262C0 File Offset: 0x001244C0
		protected override void ProcessPacket(ENet.Event netEvent)
		{
			NetworkChannel channel = NetworkChannelExtensions.GetChannel(netEvent.ChannelID);
			int length = netEvent.Packet.Length;
			BitBuffer bufferFromPacket = netEvent.Packet.GetBufferFromPacket(null);
			PacketHeader header = bufferFromPacket.GetHeader();
			OpCodes opCode = header.OpCode;
			uint id = header.Id;
			bool flag = true;
			NetworkEntity networkEntity = null;
			switch (opCode)
			{
			case OpCodes.ConnectionEvent:
			{
				OpCodes opCodes = (OpCodes)bufferFromPacket.ReadUInt();
				if (opCodes != OpCodes.Ok)
				{
					if (opCodes == OpCodes.Error)
					{
						Debug.LogError(bufferFromPacket.ReadString());
						base.Disconnect();
					}
				}
				else
				{
					Debug.Log("Requesting Character");
					if (UIManager.LoadingScreenUI)
					{
						UIManager.LoadingScreenUI.SetLoadingStatus("Requesting character");
						UIManager.LoadingScreenUI.SetLoadingPercent(0.9f);
					}
					bufferFromPacket.AddHeader(this.m_peer, OpCodes.Spawn, true);
					bufferFromPacket.AddString(SessionData.User.Id);
					bufferFromPacket.AddString(SessionData.SessionKey);
					bufferFromPacket.AddString(SessionData.GetSelectedCharacterId());
					Packet packetFromBuffer = bufferFromPacket.GetPacketFromBuffer(PacketFlags.Reliable);
					NetworkCommand networkCommand = NetworkCommandPool.GetNetworkCommand();
					networkCommand.Type = CommandType.Send;
					networkCommand.Packet = packetFromBuffer;
					networkCommand.Channel = NetworkChannel.Spawn_Self;
					this.m_commandQueue.Enqueue(networkCommand);
				}
				break;
			}
			case OpCodes.Spawn:
				if (channel == NetworkChannel.Spawn_Self)
				{
					SolDebug.LogWithTime("Received SelfSpawn packet of size " + length.ToString() + " bytes!", true);
					if (NetworkManager.EntityManager.InitEntity(base.NetworkedPrefabs, header.Id, bufferFromPacket, netEvent.ChannelID))
					{
						flag = false;
					}
					else
					{
						Debug.LogError("UNABLE TO SPAWN SELF?");
					}
				}
				else
				{
					this.EnqueueSpawn(id, netEvent.ChannelID, bufferFromPacket);
					flag = false;
				}
				break;
			case OpCodes.BulkSpawn:
			{
				int num = bufferFromPacket.ReadInt();
				SolDebug.LogWithTime(string.Concat(new string[]
				{
					"Received BulkSpawn packet of size ",
					length.ToString(),
					" bytes for ",
					num.ToString(),
					" entities!"
				}), true);
				for (int i = 0; i < num; i++)
				{
					header = bufferFromPacket.GetHeader();
					NetworkManager.EntityManager.InitEntity(base.NetworkedPrefabs, header.Id, bufferFromPacket, netEvent.ChannelID);
				}
				this.m_bulkSpawnReceived = true;
				break;
			}
			case OpCodes.Destroy:
				if (NetworkManager.EntityManager.TryGetNetworkEntity(id, out networkEntity))
				{
					UnityEngine.Object.Destroy(networkEntity.gameObject);
				}
				else
				{
					this.InvalidateQueuedSpawn(id);
				}
				break;
			case OpCodes.StateUpdate:
			case OpCodes.SyncUpdate:
			case OpCodes.ChatMessage:
			case OpCodes.LootRoll:
			{
				ClientNetworkManager.QueuedSpawn queuedSpawn;
				if (NetworkManager.EntityManager.TryGetNetworkEntity(id, out networkEntity))
				{
					networkEntity.ProcessPacket(opCode, channel, bufferFromPacket);
				}
				else if (this.m_spawnQueueDict.TryGetValue(id, out queuedSpawn))
				{
					queuedSpawn.PostOps.Enqueue(new ClientNetworkManager.PostSpawnOperation(opCode, channel, bufferFromPacket));
					flag = false;
				}
				else if (this.m_bulkSpawnReceived)
				{
					Debug.LogWarning(string.Concat(new string[]
					{
						"Entity ID:",
						id.ToString(),
						" not found for OP:",
						opCode.ToString(),
						" on CHANNEL:",
						channel.ToString()
					}));
				}
				break;
			}
			case OpCodes.RPC:
			{
				ClientNetworkManager.QueuedSpawn queuedSpawn2;
				if (NetworkManager.EntityManager.TryGetNetworkEntity(id, out networkEntity))
				{
					RpcHandler.HandleRpc(null, networkEntity, bufferFromPacket);
				}
				else if (!this.m_spawnQueueDict.TryGetValue(id, out queuedSpawn2) && this.m_bulkSpawnReceived)
				{
					Debug.LogWarning(string.Concat(new string[]
					{
						"Entity ID:",
						id.ToString(),
						" not found for OP:",
						opCode.ToString(),
						" on CHANNEL:",
						channel.ToString()
					}));
				}
				break;
			}
			case OpCodes.ServerTime:
				GameTimeReplicator.ProcessServerTimeUpdate(bufferFromPacket, netEvent.Peer.RoundTripTime);
				break;
			case OpCodes.DuelRoll:
			{
				DuelRoll duelRoll = default(DuelRoll);
				duelRoll.ReadData(bufferFromPacket);
				duelRoll.Notify();
				break;
			}
			case OpCodes.LocationEvent:
				if (!GameManager.IsServer)
				{
					LocationEvent locationEvent = default(LocationEvent);
					locationEvent.ReadData(bufferFromPacket);
					locationEvent.ExecuteEvent();
				}
				break;
			case OpCodes.AuctionHouse:
				if (ClientGameManager.UIManager && ClientGameManager.UIManager.AuctionHouseUI)
				{
					ClientGameManager.UIManager.AuctionHouseUI.ProcessAuctionHouseUpdate(bufferFromPacket);
				}
				break;
			}
			if (flag)
			{
				bufferFromPacket.ReturnToPool();
			}
		}

		// Token: 0x06002231 RID: 8753 RVA: 0x00058AA4 File Offset: 0x00056CA4
		private void ZoneCheckCallback(bool isAuthorized)
		{
			if (isAuthorized)
			{
				base.InitializeConnection();
				return;
			}
			GameManager.SceneCompositionManager.ReloadStartupScene();
		}

		// Token: 0x06002232 RID: 8754 RVA: 0x00126738 File Offset: 0x00124938
		private void EnqueueSpawn(uint netId, byte channelId, BitBuffer buffer)
		{
			ClientNetworkManager.QueuedSpawn fromPool = StaticPool<ClientNetworkManager.QueuedSpawn>.GetFromPool();
			fromPool.Init(netId, channelId, buffer);
			this.InvalidateQueuedSpawn(netId);
			this.m_spawnQueue.Enqueue(fromPool);
			this.m_spawnQueueDict.Add(netId, fromPool);
		}

		// Token: 0x06002233 RID: 8755 RVA: 0x00126774 File Offset: 0x00124974
		private void ProcessSpawnQueue()
		{
			if (this.m_spawnQueue.Count <= 0)
			{
				return;
			}
			NetworkEntity networkEntity = null;
			ClientNetworkManager.QueuedSpawn queuedSpawn = this.m_spawnQueue.Dequeue();
			if (queuedSpawn.IsValid)
			{
				this.m_spawnQueueDict.Remove(queuedSpawn.NetworkId);
				networkEntity = NetworkManager.EntityManager.InitEntity(base.NetworkedPrefabs, queuedSpawn.NetworkId, queuedSpawn.Buffer, queuedSpawn.ChannelID);
			}
			while (queuedSpawn.PostOps.Count > 0)
			{
				ClientNetworkManager.PostSpawnOperation postSpawnOperation = queuedSpawn.PostOps.Dequeue();
				if (networkEntity)
				{
					networkEntity.ProcessPacket(postSpawnOperation.Op, postSpawnOperation.NetChannel, postSpawnOperation.Buffer);
				}
				postSpawnOperation.Buffer.ReturnToPool();
			}
			queuedSpawn.Buffer.ReturnToPool();
			StaticPool<ClientNetworkManager.QueuedSpawn>.ReturnToPool(queuedSpawn);
		}

		// Token: 0x06002234 RID: 8756 RVA: 0x00126834 File Offset: 0x00124A34
		private void InvalidateQueuedSpawn(uint netId)
		{
			ClientNetworkManager.QueuedSpawn queuedSpawn;
			if (this.m_spawnQueueDict.TryGetValue(netId, out queuedSpawn))
			{
				queuedSpawn.IsValid = false;
				this.m_spawnQueueDict.Remove(netId);
			}
		}

		// Token: 0x04002649 RID: 9801
		public static bool DisconnectInitiatedByClient;

		// Token: 0x0400264A RID: 9802
		private bool m_disconnectSent;

		// Token: 0x0400264B RID: 9803
		private bool m_bulkSpawnReceived;

		// Token: 0x0400264C RID: 9804
		private readonly Queue<ClientNetworkManager.QueuedSpawn> m_spawnQueue = new Queue<ClientNetworkManager.QueuedSpawn>(100);

		// Token: 0x0400264D RID: 9805
		private readonly Dictionary<uint, ClientNetworkManager.QueuedSpawn> m_spawnQueueDict = new Dictionary<uint, ClientNetworkManager.QueuedSpawn>(100);

		// Token: 0x020004C2 RID: 1218
		private struct PostSpawnOperation
		{
			// Token: 0x06002236 RID: 8758 RVA: 0x00058ADC File Offset: 0x00056CDC
			public PostSpawnOperation(OpCodes op, NetworkChannel channel, BitBuffer buffer)
			{
				this.Op = op;
				this.NetChannel = channel;
				this.Buffer = buffer;
			}

			// Token: 0x0400264E RID: 9806
			public OpCodes Op;

			// Token: 0x0400264F RID: 9807
			public NetworkChannel NetChannel;

			// Token: 0x04002650 RID: 9808
			public BitBuffer Buffer;
		}

		// Token: 0x020004C3 RID: 1219
		private class QueuedSpawn : IPoolable
		{
			// Token: 0x06002237 RID: 8759 RVA: 0x00058AF3 File Offset: 0x00056CF3
			public void Init(uint netId, byte channelId, BitBuffer buffer)
			{
				this.IsValid = true;
				this.NetworkId = netId;
				this.ChannelID = channelId;
				this.Buffer = buffer;
			}

			// Token: 0x170006FC RID: 1788
			// (get) Token: 0x06002238 RID: 8760 RVA: 0x00058B11 File Offset: 0x00056D11
			// (set) Token: 0x06002239 RID: 8761 RVA: 0x00058B19 File Offset: 0x00056D19
			bool IPoolable.InPool
			{
				get
				{
					return this.m_inPool;
				}
				set
				{
					this.m_inPool = value;
				}
			}

			// Token: 0x0600223A RID: 8762 RVA: 0x00058B22 File Offset: 0x00056D22
			void IPoolable.Reset()
			{
				this.IsValid = false;
				this.NetworkId = 0U;
				this.ChannelID = 0;
				this.Buffer = null;
				this.PostOps.Clear();
			}

			// Token: 0x04002651 RID: 9809
			private bool m_inPool;

			// Token: 0x04002652 RID: 9810
			public bool IsValid;

			// Token: 0x04002653 RID: 9811
			public uint NetworkId;

			// Token: 0x04002654 RID: 9812
			public byte ChannelID;

			// Token: 0x04002655 RID: 9813
			public BitBuffer Buffer;

			// Token: 0x04002656 RID: 9814
			public readonly Queue<ClientNetworkManager.PostSpawnOperation> PostOps = new Queue<ClientNetworkManager.PostSpawnOperation>(10);
		}
	}
}
