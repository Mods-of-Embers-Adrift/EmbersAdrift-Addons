using System;
using System.Collections.Generic;
using ENet;
using NetStack.Serialization;
using SoL.Game;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.Objects;
using SoL.Networking.SolServer;
using SoL.Utilities.Logging;
using UnityEngine;

namespace SoL.Networking.Managers
{
	// Token: 0x020004C7 RID: 1223
	public sealed class ServerNetworkManager : NetworkManager
	{
		// Token: 0x1400003C RID: 60
		// (add) Token: 0x06002270 RID: 8816 RVA: 0x00126DA0 File Offset: 0x00124FA0
		// (remove) Token: 0x06002271 RID: 8817 RVA: 0x00126DD4 File Offset: 0x00124FD4
		public static event Action ZoneRecordLoaded;

		// Token: 0x17000708 RID: 1800
		// (get) Token: 0x06002272 RID: 8818 RVA: 0x00058919 File Offset: 0x00056B19
		private static PlayerCollection m_peers
		{
			get
			{
				return BaseNetworkEntityManager.Peers;
			}
		}

		// Token: 0x06002273 RID: 8819 RVA: 0x00058CBB File Offset: 0x00056EBB
		private void Start()
		{
			NetworkManager.EntityManager = (ServerGameManager.ServerNetworkEntityManager = base.gameObject.AddComponent<ServerNetworkEntityManager>());
		}

		// Token: 0x06002274 RID: 8820 RVA: 0x00126E08 File Offset: 0x00125008
		private bool AttemptConnect(Host host, NetworkCommand command, ushort port, bool printException)
		{
			try
			{
				Address value = new Address
				{
					Port = port
				};
				host.Create(new Address?(value), command.ConnectionSettings.PeerLimit, command.ConnectionSettings.ChannelCount);
				Debug.Log(string.Format("Server started on port: {0} for {1} users with {2} channels.", port.ToString(), command.ConnectionSettings.PeerLimit, command.ConnectionSettings.ChannelCount));
				ServerNetworkManager.Port = port;
				return true;
			}
			catch (Exception message)
			{
				if (printException)
				{
					Debug.LogError(message);
				}
			}
			return false;
		}

		// Token: 0x06002275 RID: 8821 RVA: 0x00058CD3 File Offset: 0x00056ED3
		protected override void LoadEnetConfig()
		{
			NetworkManager.Config = EnetConfig.GetConfigFromDB();
			if (NetworkManager.Config != null)
			{
				Debug.Log("Loaded ENetConfig: " + NetworkManager.Config.ToString());
			}
		}

		// Token: 0x06002276 RID: 8822 RVA: 0x00126EAC File Offset: 0x001250AC
		protected override void LoadServerConfig()
		{
			ServerConfig serverConfig = ConfigHelpers.GetServerConfig();
			if (serverConfig != null)
			{
				ServerNetworkManager.MinPortRange = serverConfig.PortRangeMin;
				ServerNetworkManager.MaxPortRange = serverConfig.PortRangeMax;
			}
		}

		// Token: 0x06002277 RID: 8823 RVA: 0x00058CFF File Offset: 0x00056EFF
		protected override void LoadZoneRecord(ZoneId zoneId)
		{
			LocalZoneManager.ZoneRecord = ZoneRecord.LoadZoneId(ExternalGameDatabase.Database, zoneId);
			Action zoneRecordLoaded = ServerNetworkManager.ZoneRecordLoaded;
			if (zoneRecordLoaded == null)
			{
				return;
			}
			zoneRecordLoaded();
		}

		// Token: 0x06002278 RID: 8824 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void Func_StartHost(Host host, NetworkCommand command)
		{
		}

		// Token: 0x06002279 RID: 8825 RVA: 0x00126EDC File Offset: 0x001250DC
		protected override void Func_StopHost(Host host, NetworkCommand command)
		{
			Debug.Log("STOPPING SERVER FROM NETWORK THREAD");
			for (int i = 0; i < ServerNetworkManager.m_peers.Count; i++)
			{
				if (ServerNetworkManager.m_peers[i].NetworkId.Peer.IsSet)
				{
					ServerNetworkManager.m_peers[i].NetworkId.Peer.Disconnect(0U);
				}
			}
			base.CloseConnection();
		}

		// Token: 0x0600227A RID: 8826 RVA: 0x00058D20 File Offset: 0x00056F20
		protected override void Func_DisconnectClient(Host host, NetworkCommand command)
		{
			if (command.Target.IsSet)
			{
				command.Target.Disconnect(66U);
			}
		}

		// Token: 0x0600227B RID: 8827 RVA: 0x00058D3C File Offset: 0x00056F3C
		protected override bool Func_Send(Host host, NetworkCommand command)
		{
			return command != null && command.Target.IsSet && command.Target.Send(command.Channel.GetByte(), ref command.Packet);
		}

		// Token: 0x0600227C RID: 8828 RVA: 0x00058D6C File Offset: 0x00056F6C
		protected override void Func_BroadcastAll(Host host, NetworkCommand command)
		{
			host.Broadcast(command.Channel.GetByte(), ref command.Packet);
		}

		// Token: 0x0600227D RID: 8829 RVA: 0x00058D85 File Offset: 0x00056F85
		protected override void Func_BroadcastOthers(Host host, NetworkCommand command)
		{
			host.Broadcast(command.Channel.GetByte(), ref command.Packet, command.Source);
		}

		// Token: 0x0600227E RID: 8830 RVA: 0x00126F54 File Offset: 0x00125154
		protected override void Func_BroadcastGroup(Host host, NetworkCommand command)
		{
			if (NetworkManager.Config.UseSendInsteadOfBroadcast)
			{
				for (int i = 0; i < command.TargetGroup.Length; i++)
				{
					if (command.TargetGroup[i].IsSet)
					{
						command.TargetGroup[i].Send(command.Channel.GetByte(), ref command.Packet);
					}
				}
				return;
			}
			host.Broadcast(command.Channel.GetByte(), ref command.Packet, command.TargetGroup);
		}

		// Token: 0x0600227F RID: 8831 RVA: 0x00126FD4 File Offset: 0x001251D4
		private static void LogEnetConnectionEvent(string eventType, string ipAddress, uint peerId, uint reason, ENet.EventType enetEventType)
		{
			ServerNetworkManager.m_enetConnectionEvent[0] = eventType;
			ServerNetworkManager.m_enetConnectionEvent[1] = LocalZoneManager.ZoneRecord.DisplayName;
			ServerNetworkManager.m_enetConnectionEvent[2] = ipAddress;
			ServerNetworkManager.m_enetConnectionEvent[3] = peerId;
			ServerNetworkManager.m_enetConnectionEvent[4] = reason;
			ServerNetworkManager.m_enetConnectionEvent[5] = enetEventType;
			SolDebug.LogToIndex(LogLevel.Information, LogIndex.EnetConnection, "{@EventType} || {@Zone} || {@Host} || {@PeerId} || {@Reason} || {@EnetEventType}", ServerNetworkManager.m_enetConnectionEvent);
		}

		// Token: 0x06002280 RID: 8832 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void Connect(ENet.Event netEvent)
		{
		}

		// Token: 0x06002281 RID: 8833 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void Disconnect(ENet.Event netEvent)
		{
		}

		// Token: 0x06002282 RID: 8834 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void ProcessPacket(ENet.Event netEvent)
		{
		}

		// Token: 0x06002283 RID: 8835 RVA: 0x0004475B File Offset: 0x0004295B
		private void DisconnectClientWithError(Peer peer, OpCodes code, NetworkChannel channel, string err)
		{
		}

		// Token: 0x06002284 RID: 8836 RVA: 0x0004475B File Offset: 0x0004295B
		private static void CleanupActive()
		{
		}

		// Token: 0x06002285 RID: 8837 RVA: 0x0004475B File Offset: 0x0004295B
		private static void RemoveFromActive(UserRecord userRecord, CharacterRecord characterRecord)
		{
		}

		// Token: 0x06002286 RID: 8838 RVA: 0x0004475B File Offset: 0x0004295B
		public static void RemoveFromActive(NetworkEntity netEntity)
		{
		}

		// Token: 0x0400266C RID: 9836
		private readonly BitBuffer m_buffer = new BitBuffer(375);

		// Token: 0x0400266E RID: 9838
		private static volatile int MinPortRange = 15001;

		// Token: 0x0400266F RID: 9839
		private static volatile int MaxPortRange = 18000;

		// Token: 0x04002670 RID: 9840
		public const int kRESTPortOffset = 10000;

		// Token: 0x04002671 RID: 9841
		public static volatile ushort Port = 0;

		// Token: 0x04002672 RID: 9842
		public static int InstanceId = 0;

		// Token: 0x04002673 RID: 9843
		private const string kConnectionTemplate = "{@EventType} || {@Zone} || {@Host} || {@Session} || {@UserId} || {@CharacterId} || {@CharacterName}";

		// Token: 0x04002674 RID: 9844
		private readonly object[] m_connectionEvent = new object[7];

		// Token: 0x04002675 RID: 9845
		private const string kEnetConnectionTemplate = "{@EventType} || {@Zone} || {@Host} || {@PeerId} || {@Reason} || {@EnetEventType}";

		// Token: 0x04002676 RID: 9846
		private static readonly object[] m_enetConnectionEvent = new object[6];

		// Token: 0x04002677 RID: 9847
		private const int kPeerSize = 512;

		// Token: 0x04002678 RID: 9848
		private static readonly Dictionary<string, Peer> ActiveCharacters = new Dictionary<string, Peer>(512);

		// Token: 0x04002679 RID: 9849
		private static readonly List<string> ActiveUsersToCleanup = new List<string>(512);

		// Token: 0x0400267A RID: 9850
		private static readonly Dictionary<string, Peer> ActiveUsers = new Dictionary<string, Peer>(512);

		// Token: 0x0400267B RID: 9851
		private static readonly List<string> ActiveCharactersToCleanup = new List<string>(512);
	}
}
