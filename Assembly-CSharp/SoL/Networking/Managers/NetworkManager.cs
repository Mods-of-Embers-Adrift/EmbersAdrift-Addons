using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DisruptorUnity3d;
using ENet;
using NetStack.Quantization;
using SoL.Game;
using SoL.Managers;
using SoL.Networking.Objects;
using SoL.Networking.SolServer;
using UnityEngine;
using UnityEngine.Profiling;

namespace SoL.Networking.Managers
{
	// Token: 0x020004C5 RID: 1221
	public abstract class NetworkManager : MonoBehaviour, INetworkManager
	{
		// Token: 0x170006FD RID: 1789
		// (get) Token: 0x0600223D RID: 8765 RVA: 0x00058B60 File Offset: 0x00056D60
		// (set) Token: 0x0600223E RID: 8766 RVA: 0x00058B67 File Offset: 0x00056D67
		public static BaseNetworkEntityManager EntityManager { get; protected set; }

		// Token: 0x170006FE RID: 1790
		// (get) Token: 0x0600223F RID: 8767 RVA: 0x00058B6F File Offset: 0x00056D6F
		// (set) Token: 0x06002240 RID: 8768 RVA: 0x00058B76 File Offset: 0x00056D76
		public static Host MyHost { get; private set; }

		// Token: 0x170006FF RID: 1791
		// (get) Token: 0x06002241 RID: 8769 RVA: 0x00058B7E File Offset: 0x00056D7E
		// (set) Token: 0x06002242 RID: 8770 RVA: 0x00058B85 File Offset: 0x00056D85
		public static Peer MyPeer { get; protected set; }

		// Token: 0x17000700 RID: 1792
		// (get) Token: 0x06002243 RID: 8771 RVA: 0x00058B8D File Offset: 0x00056D8D
		public static NetworkEntity MyEntity
		{
			get
			{
				return LocalPlayer.NetworkEntity;
			}
		}

		// Token: 0x17000701 RID: 1793
		// (get) Token: 0x06002244 RID: 8772 RVA: 0x00058B94 File Offset: 0x00056D94
		// (set) Token: 0x06002245 RID: 8773 RVA: 0x00058B9B File Offset: 0x00056D9B
		public static EnetConfig Config { get; set; }

		// Token: 0x17000702 RID: 1794
		// (get) Token: 0x06002246 RID: 8774 RVA: 0x00058BA3 File Offset: 0x00056DA3
		public static bool IsServer
		{
			get
			{
				return GameManager.IsServer;
			}
		}

		// Token: 0x17000703 RID: 1795
		// (get) Token: 0x06002247 RID: 8775 RVA: 0x00058BAA File Offset: 0x00056DAA
		public static NetworkedAnimatorParamCollection ParamSettingsCollection
		{
			get
			{
				if (NetworkManager.m_paramSettingsCollection == null)
				{
					NetworkManager.m_paramSettingsCollection = Resources.Load<NetworkedAnimatorParamCollection>("NetworkedAnimatorParams");
				}
				return NetworkManager.m_paramSettingsCollection;
			}
		}

		// Token: 0x17000704 RID: 1796
		// (get) Token: 0x06002248 RID: 8776 RVA: 0x00058BCD File Offset: 0x00056DCD
		// (set) Token: 0x06002249 RID: 8777 RVA: 0x00058BD5 File Offset: 0x00056DD5
		public NetworkedPrefabCollection NetworkedPrefabs { get; private set; }

		// Token: 0x0600224A RID: 8778 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void PreConnect()
		{
		}

		// Token: 0x0600224B RID: 8779
		protected abstract void Func_StartHost(Host host, NetworkCommand command);

		// Token: 0x0600224C RID: 8780
		protected abstract void Func_StopHost(Host host, NetworkCommand command);

		// Token: 0x0600224D RID: 8781
		protected abstract void Func_DisconnectClient(Host host, NetworkCommand command);

		// Token: 0x0600224E RID: 8782
		protected abstract bool Func_Send(Host host, NetworkCommand command);

		// Token: 0x0600224F RID: 8783
		protected abstract void Func_BroadcastAll(Host host, NetworkCommand command);

		// Token: 0x06002250 RID: 8784
		protected abstract void Func_BroadcastOthers(Host host, NetworkCommand command);

		// Token: 0x06002251 RID: 8785
		protected abstract void Func_BroadcastGroup(Host host, NetworkCommand command);

		// Token: 0x06002252 RID: 8786
		protected abstract void Connect(ENet.Event netEvent);

		// Token: 0x06002253 RID: 8787
		protected abstract void Disconnect(ENet.Event netEvent);

		// Token: 0x06002254 RID: 8788
		protected abstract void ProcessPacket(ENet.Event netEvent);

		// Token: 0x06002255 RID: 8789
		protected abstract void LoadZoneRecord(ZoneId zoneId);

		// Token: 0x17000705 RID: 1797
		// (get) Token: 0x06002256 RID: 8790 RVA: 0x00058BDE File Offset: 0x00056DDE
		public bool ConnectionIsActive
		{
			get
			{
				return this.m_networkThreadActive;
			}
		}

		// Token: 0x06002257 RID: 8791 RVA: 0x00126848 File Offset: 0x00124A48
		protected virtual void Awake()
		{
			if (NetworkManager.Instance != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			NetworkManager.Instance = this;
			this.NetworkedPrefabs = Resources.Load<NetworkedPrefabCollection>("NetworkedPrefabs");
			SceneCompositionManager.ZoneLoaded += this.SceneCompositionManagerOnZoneLoaded;
		}

		// Token: 0x06002258 RID: 8792 RVA: 0x00058BE8 File Offset: 0x00056DE8
		private void OnDestroy()
		{
			SceneCompositionManager.ZoneLoaded -= this.SceneCompositionManagerOnZoneLoaded;
			if (this.m_networkThreadActive)
			{
				this.Disconnect();
			}
		}

		// Token: 0x06002259 RID: 8793 RVA: 0x00126898 File Offset: 0x00124A98
		protected virtual void Update()
		{
			ENet.Event netEvent;
			while (this.m_logicEventQueue.TryDequeue(out netEvent))
			{
				switch (netEvent.Type)
				{
				case ENet.EventType.Connect:
					this.Connect(netEvent);
					break;
				case ENet.EventType.Disconnect:
					this.Disconnect(netEvent);
					break;
				case ENet.EventType.Receive:
					this.ProcessPacket(netEvent);
					netEvent.Packet.Dispose();
					break;
				case ENet.EventType.Timeout:
					this.Disconnect(netEvent);
					break;
				}
			}
		}

		// Token: 0x0600225A RID: 8794 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void LoadEnetConfig()
		{
		}

		// Token: 0x0600225B RID: 8795 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void LoadServerConfig()
		{
		}

		// Token: 0x0600225C RID: 8796 RVA: 0x0004479C File Offset: 0x0004299C
		protected virtual bool ValidateZoneConnectionData()
		{
			return true;
		}

		// Token: 0x0600225D RID: 8797 RVA: 0x00058C0B File Offset: 0x00056E0B
		private void SceneCompositionManagerOnZoneLoaded(ZoneId zoneId)
		{
			this.LoadEnetConfig();
			this.LoadServerConfig();
			this.LoadZoneRecord(zoneId);
			if (this.ValidateZoneConnectionData())
			{
				this.InitializeConnection();
			}
		}

		// Token: 0x0600225E RID: 8798 RVA: 0x0012690C File Offset: 0x00124B0C
		protected void InitializeConnection()
		{
			if (LocalZoneManager.ZoneRecord == null)
			{
				throw new ArgumentNullException("Invalid ZoneRecord");
			}
			if (NetworkManager.Config == null)
			{
				throw new ArgumentNullException("Invalid EnetConfig");
			}
			if (this.m_initializeConnectionCo != null)
			{
				Debug.LogWarning("Connection already initializing!");
				return;
			}
			this.m_initializeConnectionCo = this.InitializeConnectionCo();
			base.StartCoroutine(this.m_initializeConnectionCo);
		}

		// Token: 0x0600225F RID: 8799 RVA: 0x00058C2E File Offset: 0x00056E2E
		private IEnumerator InitializeConnectionCo()
		{
			if (this.m_networkHostIsSet)
			{
				Debug.Log("Waiting on MyHost to no longer be set...");
			}
			while (this.m_networkHostIsSet)
			{
				yield return null;
			}
			this.FlushThreadCommandQueue();
			this.FlushUpdateLogicEventQueue();
			string host = LocalZoneManager.ZoneRecord.Address;
			if (SolServerConnectionManager.Config != null && !string.IsNullOrEmpty(SolServerConnectionManager.Config.ZoneHost))
			{
				host = SolServerConnectionManager.Config.ZoneHost;
			}
			ConnectionSettings connection = new ConnectionSettings
			{
				Host = host,
				Port = (ushort)LocalZoneManager.ZoneRecord.Port,
				ZoneRecordId = LocalZoneManager.ZoneRecord.Id,
				ChannelCount = NetworkManager.Config.ChannelCount,
				PeerLimit = NetworkManager.Config.PeerLimit,
				UpdateTime = NetworkManager.Config.UpdateTime
			};
			this.Connect(connection);
			this.m_initializeConnectionCo = null;
			yield break;
		}

		// Token: 0x06002260 RID: 8800 RVA: 0x0012696C File Offset: 0x00124B6C
		public void Connect(ConnectionSettings connection)
		{
			this.PreConnect();
			this.m_networkThread = this.NetworkThread();
			this.m_networkThreadActive = true;
			this.m_networkThread.Start();
			NetworkCommand networkCommand = NetworkCommandPool.GetNetworkCommand();
			networkCommand.Type = CommandType.StartHost;
			networkCommand.ConnectionSettings = connection;
			this.m_commandQueue.Enqueue(networkCommand);
		}

		// Token: 0x06002261 RID: 8801 RVA: 0x001269C0 File Offset: 0x00124BC0
		public void Disconnect()
		{
			NetworkCommand networkCommand = NetworkCommandPool.GetNetworkCommand();
			networkCommand.Type = CommandType.StopHost;
			this.m_commandQueue.Enqueue(networkCommand);
		}

		// Token: 0x06002262 RID: 8802 RVA: 0x00058C3D File Offset: 0x00056E3D
		protected void CloseConnection()
		{
			this.m_networkThreadActive = false;
		}

		// Token: 0x06002263 RID: 8803 RVA: 0x00058C48 File Offset: 0x00056E48
		private Thread NetworkThread()
		{
			return new Thread(delegate()
			{
				int timeout = 33;
				this.m_staleCommands = new List<NetworkCommand>(10);
				using (Host host = new Host())
				{
					this.m_networkHostIsSet = true;
					NetworkManager.MyHost = host;
					while (this.m_networkThreadActive)
					{
						NetworkCommand networkCommand = null;
						while (this.m_commandQueue.TryDequeue(out networkCommand))
						{
							if (!this.m_networkThreadActive)
							{
								networkCommand.Packet.Dispose();
								this.m_staleCommands.Add(networkCommand);
							}
							else
							{
								switch (networkCommand.Type)
								{
								case CommandType.StartHost:
									timeout = networkCommand.ConnectionSettings.UpdateTime;
									this.Func_StartHost(host, networkCommand);
									break;
								case CommandType.StopHost:
									this.Func_StopHost(host, networkCommand);
									break;
								case CommandType.DisconnectClient:
									this.Func_DisconnectClient(host, networkCommand);
									break;
								case CommandType.Send:
									if (!this.Func_Send(host, networkCommand))
									{
										networkCommand.Packet.Dispose();
									}
									break;
								case CommandType.BroadcastAll:
									this.Func_BroadcastAll(host, networkCommand);
									break;
								case CommandType.BroadcastOthers:
									this.Func_BroadcastOthers(host, networkCommand);
									break;
								case CommandType.BroadcastGroup:
									this.Func_BroadcastGroup(host, networkCommand);
									break;
								default:
									throw new ArgumentException(string.Format("Invalid Command.Type ({0})", networkCommand.Type));
								}
								this.m_staleCommands.Add(networkCommand);
							}
						}
						for (int i = 0; i < this.m_staleCommands.Count; i++)
						{
							this.m_staleCommands[i].ReturnToPool();
						}
						this.m_staleCommands.Clear();
						if (host.IsSet)
						{
							ENet.Event item;
							host.Service(timeout, out item);
							if (item.Type != ENet.EventType.None)
							{
								this.m_logicEventQueue.Enqueue(item);
							}
						}
						Profiler.EndThreadProfiling();
					}
					host.Flush();
					host.Dispose();
					this.m_networkHostIsSet = false;
				}
			});
		}

		// Token: 0x06002264 RID: 8804 RVA: 0x00058C5B File Offset: 0x00056E5B
		public void AddCommandToQueue(NetworkCommand command)
		{
			if (command.Type == CommandType.None)
			{
				throw new ArgumentException("Null CommandType received!");
			}
			this.m_commandQueue.Enqueue(command);
		}

		// Token: 0x06002265 RID: 8805 RVA: 0x001269E8 File Offset: 0x00124BE8
		private void FlushUpdateLogicEventQueue()
		{
			ENet.Event @event;
			while (this.m_logicEventQueue.TryDequeue(out @event))
			{
				@event.Packet.Dispose();
			}
		}

		// Token: 0x06002266 RID: 8806 RVA: 0x00126A18 File Offset: 0x00124C18
		protected void FlushThreadCommandQueue()
		{
			NetworkCommand networkCommand;
			while (this.m_commandQueue.TryDequeue(out networkCommand))
			{
				networkCommand.Packet.Dispose();
				networkCommand.ReturnToPool();
			}
		}

		// Token: 0x04002657 RID: 9815
		public const float kMaxRange = 32f;

		// Token: 0x04002658 RID: 9816
		public static NetworkManager Instance = null;

		// Token: 0x0400265D RID: 9821
		private static int kRingBufferSize = 1024;

		// Token: 0x0400265E RID: 9822
		public static BoundedRange[] Range = new BoundedRange[]
		{
			new BoundedRange(-32f, 32f, 0.05f),
			new BoundedRange(-32f, 32f, 0.05f),
			new BoundedRange(-32f, 32f, 0.05f)
		};

		// Token: 0x0400265F RID: 9823
		private static NetworkedAnimatorParamCollection m_paramSettingsCollection = null;

		// Token: 0x04002661 RID: 9825
		protected Peer m_peer;

		// Token: 0x04002662 RID: 9826
		private List<NetworkCommand> m_staleCommands;

		// Token: 0x04002663 RID: 9827
		private Thread m_networkThread;

		// Token: 0x04002664 RID: 9828
		private volatile bool m_networkThreadActive;

		// Token: 0x04002665 RID: 9829
		private volatile bool m_networkHostIsSet;

		// Token: 0x04002666 RID: 9830
		protected readonly RingBuffer<NetworkCommand> m_commandQueue = new RingBuffer<NetworkCommand>(NetworkManager.kRingBufferSize);

		// Token: 0x04002667 RID: 9831
		protected readonly RingBuffer<ENet.Event> m_logicEventQueue = new RingBuffer<ENet.Event>(NetworkManager.kRingBufferSize);

		// Token: 0x04002668 RID: 9832
		private IEnumerator m_initializeConnectionCo;
	}
}
