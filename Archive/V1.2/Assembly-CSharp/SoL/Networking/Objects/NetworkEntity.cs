using System;
using System.Collections;
using System.Collections.Generic;
using ENet;
using NetStack.Serialization;
using SoL.Game;
using SoL.Game.EffectSystem;
using SoL.Game.Messages;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Game.Settings;
using SoL.Game.Spawning;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.Game;
using SoL.Networking.Managers;
using SoL.Networking.Proximity;
using SoL.Networking.Replication;
using SoL.Networking.RPC;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using SoL.Utilities.Logging;
using UnityEngine;

namespace SoL.Networking.Objects
{
	// Token: 0x020004B4 RID: 1204
	public sealed class NetworkEntity : GameEntityComponent
	{
		// Token: 0x06002190 RID: 8592 RVA: 0x00058497 File Offset: 0x00056697
		private static NetworkId GetNewNetworkId(Peer peer)
		{
			NetworkId result = new NetworkId(NetworkEntity.m_currentNetworkIdValue, peer);
			NetworkEntity.m_currentNetworkIdValue += 1U;
			return result;
		}

		// Token: 0x170006E0 RID: 1760
		// (get) Token: 0x06002191 RID: 8593 RVA: 0x000584B0 File Offset: 0x000566B0
		// (set) Token: 0x06002192 RID: 8594 RVA: 0x000584B8 File Offset: 0x000566B8
		public NetworkId NetworkId { get; private set; }

		// Token: 0x170006E1 RID: 1761
		// (get) Token: 0x06002193 RID: 8595 RVA: 0x000584C1 File Offset: 0x000566C1
		// (set) Token: 0x06002194 RID: 8596 RVA: 0x000584C9 File Offset: 0x000566C9
		public UniqueId SpawnId
		{
			get
			{
				return this.m_spawnId;
			}
			private set
			{
				this.m_spawnId = value;
			}
		}

		// Token: 0x170006E2 RID: 1762
		// (get) Token: 0x06002195 RID: 8597 RVA: 0x000584D2 File Offset: 0x000566D2
		public UniqueId PrefabId
		{
			get
			{
				return this.m_prefabId;
			}
		}

		// Token: 0x170006E3 RID: 1763
		// (get) Token: 0x06002196 RID: 8598 RVA: 0x000584DA File Offset: 0x000566DA
		public bool UseProximity
		{
			get
			{
				return this.m_proximitySensor != null;
			}
		}

		// Token: 0x170006E4 RID: 1764
		// (get) Token: 0x06002197 RID: 8599 RVA: 0x000584E8 File Offset: 0x000566E8
		public RpcHandler RpcHandler
		{
			get
			{
				return this.m_rpcHandler;
			}
		}

		// Token: 0x170006E5 RID: 1765
		// (get) Token: 0x06002198 RID: 8600 RVA: 0x000584F0 File Offset: 0x000566F0
		public int NObservers
		{
			get
			{
				if (!this.UseProximity)
				{
					return 1;
				}
				return this.m_proximitySensor.Observers.Count;
			}
		}

		// Token: 0x170006E6 RID: 1766
		// (get) Token: 0x06002199 RID: 8601 RVA: 0x0005850C File Offset: 0x0005670C
		// (set) Token: 0x0600219A RID: 8602 RVA: 0x00058514 File Offset: 0x00056714
		public bool IsServer { get; private set; }

		// Token: 0x170006E7 RID: 1767
		// (get) Token: 0x0600219B RID: 8603 RVA: 0x0005851D File Offset: 0x0005671D
		// (set) Token: 0x0600219C RID: 8604 RVA: 0x00058525 File Offset: 0x00056725
		public bool IsClient { get; private set; }

		// Token: 0x170006E8 RID: 1768
		// (get) Token: 0x0600219D RID: 8605 RVA: 0x0005852E File Offset: 0x0005672E
		// (set) Token: 0x0600219E RID: 8606 RVA: 0x00058536 File Offset: 0x00056736
		public bool IsLocal { get; private set; }

		// Token: 0x170006E9 RID: 1769
		// (get) Token: 0x0600219F RID: 8607 RVA: 0x0005853F File Offset: 0x0005673F
		public bool IsInitialized
		{
			get
			{
				return this.m_initialized;
			}
		}

		// Token: 0x170006EA RID: 1770
		// (get) Token: 0x060021A0 RID: 8608 RVA: 0x00058547 File Offset: 0x00056747
		public PlayerRpcHandler PlayerRpcHandler
		{
			get
			{
				if (this.m_playerRpcHandler == null && this.m_rpcHandler != null)
				{
					this.m_rpcHandler.TryGetAsType(out this.m_playerRpcHandler);
				}
				return this.m_playerRpcHandler;
			}
		}

		// Token: 0x170006EB RID: 1771
		// (get) Token: 0x060021A1 RID: 8609 RVA: 0x0005857D File Offset: 0x0005677D
		public NpcRpcHandler NpcRpcHandler
		{
			get
			{
				if (this.m_npcRpcHandler == null && this.m_rpcHandler != null)
				{
					this.m_rpcHandler.TryGetAsType(out this.m_npcRpcHandler);
				}
				return this.m_npcRpcHandler;
			}
		}

		// Token: 0x170006EC RID: 1772
		// (get) Token: 0x060021A2 RID: 8610 RVA: 0x000585B3 File Offset: 0x000567B3
		// (set) Token: 0x060021A3 RID: 8611 RVA: 0x000585BB File Offset: 0x000567BB
		public bool BulkSpawnSent { get; set; }

		// Token: 0x14000033 RID: 51
		// (add) Token: 0x060021A4 RID: 8612 RVA: 0x001242A4 File Offset: 0x001224A4
		// (remove) Token: 0x060021A5 RID: 8613 RVA: 0x001242DC File Offset: 0x001224DC
		public event Action OnBeforeStart;

		// Token: 0x14000034 RID: 52
		// (add) Token: 0x060021A6 RID: 8614 RVA: 0x00124314 File Offset: 0x00122514
		// (remove) Token: 0x060021A7 RID: 8615 RVA: 0x0012434C File Offset: 0x0012254C
		public event Action OnStartServer;

		// Token: 0x14000035 RID: 53
		// (add) Token: 0x060021A8 RID: 8616 RVA: 0x00124384 File Offset: 0x00122584
		// (remove) Token: 0x060021A9 RID: 8617 RVA: 0x001243BC File Offset: 0x001225BC
		public event Action OnStartClient;

		// Token: 0x14000036 RID: 54
		// (add) Token: 0x060021AA RID: 8618 RVA: 0x001243F4 File Offset: 0x001225F4
		// (remove) Token: 0x060021AB RID: 8619 RVA: 0x0012442C File Offset: 0x0012262C
		public event Action OnStartLocalClient;

		// Token: 0x14000037 RID: 55
		// (add) Token: 0x060021AC RID: 8620 RVA: 0x00124464 File Offset: 0x00122664
		// (remove) Token: 0x060021AD RID: 8621 RVA: 0x0012449C File Offset: 0x0012269C
		public event Action<OpCodes, BitBuffer> ProcessIncomingPacket;

		// Token: 0x14000038 RID: 56
		// (add) Token: 0x060021AE RID: 8622 RVA: 0x001244D4 File Offset: 0x001226D4
		// (remove) Token: 0x060021AF RID: 8623 RVA: 0x00124508 File Offset: 0x00122708
		public static event Action<NetworkEntity> Initialized;

		// Token: 0x14000039 RID: 57
		// (add) Token: 0x060021B0 RID: 8624 RVA: 0x0012453C File Offset: 0x0012273C
		// (remove) Token: 0x060021B1 RID: 8625 RVA: 0x00124570 File Offset: 0x00122770
		public static event Action<NetworkEntity> Destroyed;

		// Token: 0x1400003A RID: 58
		// (add) Token: 0x060021B2 RID: 8626 RVA: 0x001245A4 File Offset: 0x001227A4
		// (remove) Token: 0x060021B3 RID: 8627 RVA: 0x001245DC File Offset: 0x001227DC
		public event Action PacketReceived;

		// Token: 0x060021B4 RID: 8628 RVA: 0x000585C4 File Offset: 0x000567C4
		private void Awake()
		{
			if (base.GameEntity != null)
			{
				base.GameEntity.NetworkEntity = this;
			}
		}

		// Token: 0x060021B5 RID: 8629 RVA: 0x000585E0 File Offset: 0x000567E0
		private void Start()
		{
			if (!this.m_initialized && NetworkManager.EntityManager != null)
			{
				this.m_isWorldEntity = true;
				NetworkManager.EntityManager.RegisterWorldEntity(this);
			}
		}

		// Token: 0x060021B6 RID: 8630 RVA: 0x00124614 File Offset: 0x00122814
		private void OnDestroy()
		{
			this.DeregisterEntity();
			this.SendBulkDestroy();
			if (this.IsLocal)
			{
				ClientGameManager.UIManager.OnDestroyLocalPlayerUI();
				LocalPlayer.InvokeLocalPlayerDestroyed();
			}
			if (this.IsServer)
			{
				ServerGameManager.SpatialManager.UnregisterEntity(this);
				if (base.GameEntity.CharacterData != null && !base.GameEntity.CharacterData.GroupId.IsEmpty)
				{
					ServerGameManager.GroupManager.UnregisterPlayer(this, base.GameEntity.CharacterData.GroupId);
				}
			}
			this.m_syncReplicatorChannel.OnDestroy();
			this.m_stateReplicatorChannel.OnDestroy();
			BitBuffer buffer = this.m_buffer;
			if (buffer != null)
			{
				buffer.ReturnToPool();
			}
			this.m_buffer = null;
			this.m_proximitySensor = null;
			this.m_network = null;
			this.m_rpcHandler = null;
			this.m_playerRpcHandler = null;
			this.m_npcRpcHandler = null;
			Action<NetworkEntity> destroyed = NetworkEntity.Destroyed;
			if (destroyed == null)
			{
				return;
			}
			destroyed(this);
		}

		// Token: 0x060021B7 RID: 8631 RVA: 0x00124700 File Offset: 0x00122900
		public void ServerInit(Peer peer, bool fromPrefab, bool nullifyProximity = false)
		{
			if (fromPrefab)
			{
				this.SpawnId = this.PrefabId;
			}
			if (nullifyProximity)
			{
				this.m_proximityPrefab = null;
			}
			this.m_lastWriteTimestamp = DateTime.UtcNow;
			this.m_nextUpdate = this.m_lastWriteTimestamp.AddSeconds((double)NetworkEntity.m_updateOffset);
			NetworkEntity.m_updateOffset += 0.1f;
			if (NetworkEntity.m_updateOffset > 5f)
			{
				NetworkEntity.m_updateOffset = 0f;
			}
			this.NetworkId = NetworkEntity.GetNewNetworkId(peer);
			this.m_network = NetworkManager.Instance;
			this.m_buffer = BitBufferExtensions.GetFromPool();
			this.IsServer = true;
			this.InitDetectionCollider();
			this.InitReplicationLayer();
			Action onBeforeStart = this.OnBeforeStart;
			if (onBeforeStart != null)
			{
				onBeforeStart();
			}
			this.InitProximity();
			this.RegisterEntity();
			this.InitObjects();
			Action onStartServer = this.OnStartServer;
			if (onStartServer != null)
			{
				onStartServer();
			}
			ServerGameManager.SpatialManager.RegisterEntity(this);
			this.EditorInit();
			this.m_initialized = true;
		}

		// Token: 0x060021B8 RID: 8632 RVA: 0x001247F0 File Offset: 0x001229F0
		public void ClientInit(INetworkManager network, uint networkId, BitBuffer inBuffer, NetworkChannel channel)
		{
			this.NetworkId = new NetworkId(networkId);
			this.m_network = network;
			this.IsClient = true;
			this.IsLocal = (channel == NetworkChannel.Spawn_Self);
			if (this.IsLocal)
			{
				this.m_buffer = BitBufferExtensions.GetFromPool();
				LocalPlayer.NetworkEntity = this;
				LocalPlayer.GameEntity = base.GameEntity;
				SolDebug.LogWithTime("NetworkEntity Local Init", true);
			}
			this.InitReplicationLayer();
			Action onBeforeStart = this.OnBeforeStart;
			if (onBeforeStart != null)
			{
				onBeforeStart();
			}
			this.InitObjects();
			this.ReadInitialState(inBuffer);
			this.RegisterEntity();
			Action onStartClient = this.OnStartClient;
			if (onStartClient != null)
			{
				onStartClient();
			}
			if (this.IsLocal)
			{
				IEnumerator routine = this.ClientInitLocalCo(inBuffer);
				base.StartCoroutine(routine);
				return;
			}
			this.PostLocalInit();
		}

		// Token: 0x060021B9 RID: 8633 RVA: 0x00058609 File Offset: 0x00056809
		private IEnumerator ClientInitLocalCo(BitBuffer inBuffer)
		{
			if (UIManager.LoadingScreenUI)
			{
				UIManager.LoadingScreenUI.SetLoadingStatus("Initializing character");
				UIManager.LoadingScreenUI.SetLoadingPercent(0.92f);
				yield return null;
			}
			else
			{
				Debug.LogWarning("Could not find UIManager.LoadingScreenUI?!");
			}
			SolDebug.LogWithTime("Initializing CharacterRecord", true);
			CharacterRecord record = new CharacterRecord();
			record.ReadData(inBuffer);
			inBuffer.ReturnToPool();
			SolDebug.LogWithTime("Initializing Character Data", true);
			base.GameEntity.CharacterData.InitializeFromRecord(record);
			SolDebug.LogWithTime("Initializing TargetController", true);
			base.GameEntity.TargetController.Initialize();
			if (UIManager.LoadingScreenUI)
			{
				UIManager.LoadingScreenUI.SetLoadingStatus("Initializing UI");
				UIManager.LoadingScreenUI.SetLoadingPercent(0.94f);
				yield return null;
			}
			SolDebug.LogWithTime("Initializing PlayerUI", true);
			yield return ClientGameManager.UIManager.InitializePlayerUI();
			if (UIManager.LoadingScreenUI)
			{
				UIManager.LoadingScreenUI.SetLoadingStatus("Initializing character");
				UIManager.LoadingScreenUI.SetLoadingPercent(0.97f);
				yield return null;
			}
			SolDebug.LogWithTime("Initializing SkillsController", true);
			base.GameEntity.SkillsController.Initialize(record);
			SolDebug.LogWithTime("Initializing CollectionController", true);
			base.GameEntity.CollectionController.Initialize(record);
			SolDebug.LogWithTime("Initializing Vitals", true);
			base.GameEntity.Vitals.Init(record);
			SolDebug.LogWithTime("Initializing StatPanel", true);
			ClientGameManager.UIManager.StatPanel.Init();
			SolDebug.LogWithTime("Initializing HighestLevelM", true);
			MasteryArchetype.RefreshHighestLevelMastery(base.GameEntity);
			if (ClientGameManager.GroupManager && ClientGameManager.GroupManager.IsGrouped)
			{
				SolDebug.LogWithTime("Initializing GroupId", true);
				this.PlayerRpcHandler.SetGroupId(ClientGameManager.GroupManager.GroupId);
			}
			if (ClientGameManager.SocialManager)
			{
				if (ClientGameManager.SocialManager.IsInGuild)
				{
					SolDebug.LogWithTime("Initializing Guild", true);
					base.GameEntity.CharacterData.GuildName.Value = ClientGameManager.SocialManager.Guild.Name;
				}
				if (ClientGameManager.SocialManager.IsInRaid)
				{
					SolDebug.LogWithTime("Initializing Raid", true);
					this.PlayerRpcHandler.SetRaidId(ClientGameManager.SocialManager.RaidId);
				}
			}
			if (UIManager.LoadingScreenUI)
			{
				UIManager.LoadingScreenUI.SetLoadingStatus("Finalizing");
				UIManager.LoadingScreenUI.SetLoadingPercent(0.98f);
			}
			yield return null;
			if (GameManager.QuestManager)
			{
				GameManager.QuestManager.ManuallyInvokeLocalPlayerInitialized();
				yield return null;
			}
			SolDebug.LogWithTime("Invoking OnStartLocalClient", true);
			Action onStartLocalClient = this.OnStartLocalClient;
			if (onStartLocalClient != null)
			{
				onStartLocalClient();
			}
			if (base.GameEntity.CharacterData)
			{
				SolDebug.LogWithTime("Initializing PresenceFlags", true);
				base.GameEntity.CharacterData.InitPresenceFlags();
			}
			yield return LocalPlayer.InvokeLocalPlayerInitialized();
			this.PostLocalInit();
			SolDebug.LogWithTime("Requesting bulk spawn", true);
			if (UIManager.LoadingScreenUI)
			{
				UIManager.LoadingScreenUI.SetLoadingStatus("Requesting spawn data");
				UIManager.LoadingScreenUI.SetLoadingPercent(0.99f);
			}
			this.m_buffer.AddHeader(this, OpCodes.BulkSpawn, true);
			Packet packetFromBuffer = this.m_buffer.GetPacketFromBuffer(PacketFlags.Reliable);
			NetworkCommand networkCommand = NetworkCommandPool.GetNetworkCommand();
			networkCommand.Type = CommandType.Send;
			networkCommand.Packet = packetFromBuffer;
			networkCommand.Channel = NetworkChannel.Spawn_Self;
			GameManager.NetworkManager.AddCommandToQueue(networkCommand);
			while (!ClientNetworkManager.HasReceivedBulkSpawn)
			{
				yield return null;
			}
			yield return null;
			LocalPlayer.ShowUiFadeLoading();
			yield break;
		}

		// Token: 0x060021BA RID: 8634 RVA: 0x001248B0 File Offset: 0x00122AB0
		private void PostLocalInit()
		{
			this.InitDetectionCollider();
			if (!this.IsLocal && base.GameEntity.Vitals)
			{
				base.GameEntity.Vitals.InitRemote();
			}
			if (base.GameEntity.OverheadNameplate != null)
			{
				if (this.IsLocal)
				{
					SolDebug.LogWithTime("Initializing OverheadNameplate", true);
				}
				base.GameEntity.OverheadNameplate.Initialize();
			}
			this.EditorInit();
			this.m_initialized = true;
			if (this.IsLocal)
			{
				SolDebug.LogWithTime("Invoking Initialized", true);
			}
			this.InvokeInitialized();
			if (this.IsReplacingId != null && LocalPlayer.GameEntity && LocalPlayer.GameEntity.TargetController)
			{
				LocalPlayer.GameEntity.TargetController.ReplacementNetworkEntitySpawned(this.IsReplacingId.Value, this);
			}
		}

		// Token: 0x060021BB RID: 8635 RVA: 0x0004475B File Offset: 0x0004295B
		private void EditorInit()
		{
		}

		// Token: 0x060021BC RID: 8636 RVA: 0x00124988 File Offset: 0x00122B88
		private void InitDetectionCollider()
		{
			DetectionCollider componentInChildren = base.gameObject.GetComponentInChildren<DetectionCollider>(true);
			if (componentInChildren)
			{
				componentInChildren.Init(this);
			}
		}

		// Token: 0x060021BD RID: 8637 RVA: 0x001249B4 File Offset: 0x00122BB4
		private void InitObjects()
		{
			NetworkInclusionFlags b;
			if (this.IsServer)
			{
				b = NetworkInclusionFlags.Server;
			}
			else if (this.IsLocal)
			{
				b = NetworkInclusionFlags.LocalClient;
			}
			else
			{
				b = NetworkInclusionFlags.RemoteClient;
			}
			for (int i = 0; i < this.m_objects.Length; i++)
			{
				if (!(this.m_objects[i].Object == null))
				{
					if (this.m_objects[i].InclusionFlags == NetworkInclusionFlags.None)
					{
						this.m_objects[i].Object.SetActive(false);
					}
					else
					{
						this.m_objects[i].Object.SetActive(this.m_objects[i].InclusionFlags.HasBitFlag(b));
					}
				}
			}
		}

		// Token: 0x060021BE RID: 8638 RVA: 0x00124A64 File Offset: 0x00122C64
		public void DisableObjects()
		{
			if (!this.IsServer)
			{
				for (int i = 0; i < this.m_objects.Length; i++)
				{
					if (!(this.m_objects[i].Object == null) && this.m_objects[i].InclusionFlags != NetworkInclusionFlags.None && (this.m_objects[i].InclusionFlags.HasBitFlag(NetworkInclusionFlags.LocalClient) || this.m_objects[i].InclusionFlags.HasBitFlag(NetworkInclusionFlags.RemoteClient)))
					{
						this.m_objects[i].Object.SetActive(false);
					}
				}
			}
		}

		// Token: 0x060021BF RID: 8639 RVA: 0x00124B08 File Offset: 0x00122D08
		private void InitReplicationLayer()
		{
			this.m_rpcHandler = base.gameObject.GetComponent<NetworkEntityRpcs>();
			if (this.m_rpcHandler != null)
			{
				this.m_rpcHandler.Init(this.m_network, this, this.m_buffer, this.m_updateRate);
			}
			this.m_syncReplicatorChannel.Init(true, this.m_network, this);
			this.m_stateReplicatorChannel.Init(false, this.m_network, this);
			this.m_nextUpdate = DateTime.UtcNow.AddSeconds((double)this.m_updateRate);
		}

		// Token: 0x060021C0 RID: 8640 RVA: 0x0005861F File Offset: 0x0005681F
		private void RegisterEntity()
		{
			NetworkManager.EntityManager.RegisterEntity(this);
		}

		// Token: 0x060021C1 RID: 8641 RVA: 0x0005862C File Offset: 0x0005682C
		private void DeregisterEntity()
		{
			if (NetworkManager.EntityManager)
			{
				NetworkManager.EntityManager.DeregisterEntity(this);
				if (this.m_isWorldEntity)
				{
					NetworkManager.EntityManager.DeregisterWorldEntity(this);
				}
			}
		}

		// Token: 0x060021C2 RID: 8642 RVA: 0x00058658 File Offset: 0x00056858
		public void InvokeInitialized()
		{
			Action<NetworkEntity> initialized = NetworkEntity.Initialized;
			if (initialized == null)
			{
				return;
			}
			initialized(this);
		}

		// Token: 0x060021C3 RID: 8643 RVA: 0x00124B94 File Offset: 0x00122D94
		public void ProcessPacket(OpCodes op, NetworkChannel channel, BitBuffer inBuffer)
		{
			switch (op)
			{
			case OpCodes.StateUpdate:
			case OpCodes.SyncUpdate:
				if (channel - NetworkChannel.State_Client <= 1)
				{
					this.m_stateReplicatorChannel.ReadReplicationData(inBuffer);
					goto IL_1EF;
				}
				if (channel - NetworkChannel.SyncVar_Client <= 1)
				{
					this.m_syncReplicatorChannel.ReadReplicationData(inBuffer);
					goto IL_1EF;
				}
				goto IL_1EF;
			case OpCodes.ClientSyncUpdate:
			{
				int index = inBuffer.ReadInt();
				SyncVarReplicator syncVarReplicator = this.m_syncReplicatorChannel.Replicators[index] as SyncVarReplicator;
				if (syncVarReplicator != null)
				{
					syncVarReplicator.ReadClientVariable(inBuffer);
					goto IL_1EF;
				}
				goto IL_1EF;
			}
			case OpCodes.ChatMessage:
				switch (channel)
				{
				case NetworkChannel.CombatResults:
				{
					EffectApplicationResult fromPool = StaticPool<EffectApplicationResult>.GetFromPool();
					fromPool.ReadData(inBuffer);
					if (ClientGameManager.CombatTextManager)
					{
						ClientGameManager.CombatTextManager.InitializeCombatText(this, fromPool);
						goto IL_1EF;
					}
					goto IL_1EF;
				}
				case NetworkChannel.LootMessage:
				{
					ArchetypeInstance fromPool2 = StaticPool<ArchetypeInstance>.GetFromPool();
					fromPool2.ReadData(inBuffer);
					string content = string.Concat(new string[]
					{
						"<i>",
						TextMeshProExtensions.CreatePlayerLink(base.GameEntity.CharacterData.Name.Value),
						"</i> has looted <u>",
						TextMeshProExtensions.CreateInstanceLink(fromPool2),
						"</u>"
					});
					MessageManager.AddLinkedInstance(fromPool2, false);
					MessageManager.ChatQueue.AddToQueue(MessageType.Loot, content);
					goto IL_1EF;
				}
				case NetworkChannel.UtilityResult:
				{
					UtilityResult result = default(UtilityResult);
					result.ReadData(inBuffer);
					UtilityAbility.ProcessUtilityAbilityResult(this, result);
					goto IL_1EF;
				}
				case NetworkChannel.LootRoll:
					goto IL_1EF;
				case NetworkChannel.GenericChatMessage:
				{
					MessageDelivery messageDelivery = default(MessageDelivery);
					messageDelivery.ReadData(inBuffer);
					MessageManager.ChatQueue.AddToQueue(messageDelivery.Type, messageDelivery.Text);
					if (messageDelivery.EventType != MessageEventType.None)
					{
						GlobalSettings.Values.Progression.InitLevelUpVfxForEntity(base.GameEntity, messageDelivery.EventType);
						goto IL_1EF;
					}
					goto IL_1EF;
				}
				default:
					goto IL_1EF;
				}
				break;
			case OpCodes.LootRoll:
				if (!GameManager.IsServer)
				{
					ClientGameManager.UIManager.LootRollUI.UpdateLootRoll(inBuffer);
					goto IL_1EF;
				}
				goto IL_1EF;
			}
			Action<OpCodes, BitBuffer> processIncomingPacket = this.ProcessIncomingPacket;
			if (processIncomingPacket != null)
			{
				processIncomingPacket(op, inBuffer);
			}
			IL_1EF:
			Action packetReceived = this.PacketReceived;
			if (packetReceived == null)
			{
				return;
			}
			packetReceived();
		}

		// Token: 0x060021C4 RID: 8644 RVA: 0x0005866A File Offset: 0x0005686A
		public void ServerProcessUpdates()
		{
			if (this.IsServer && this.m_initialized)
			{
				this.ProcessUpdates();
			}
		}

		// Token: 0x060021C5 RID: 8645 RVA: 0x00058682 File Offset: 0x00056882
		public void ProcessLocalUpdates()
		{
			if (this.IsLocal)
			{
				this.ProcessUpdates();
			}
		}

		// Token: 0x060021C6 RID: 8646 RVA: 0x00058692 File Offset: 0x00056892
		public void FlushObservers()
		{
			if (this.IsServer && this.UseProximity)
			{
				this.m_proximitySensor.RebuildObservers();
			}
		}

		// Token: 0x060021C7 RID: 8647 RVA: 0x00124DA0 File Offset: 0x00122FA0
		private void ProcessUpdates()
		{
			DateTime utcNow = DateTime.UtcNow;
			if (this.m_nextUpdate > utcNow)
			{
				return;
			}
			this.m_nextUpdate = utcNow.AddSeconds((double)this.m_updateRate);
			if (this.IsServer && this.UseProximity)
			{
				int num = this.m_proximitySensor.DistanceBands.Length;
				if (num == 1)
				{
					for (int i = 0; i < this.m_proximitySensor.DistanceBands.Length; i++)
					{
						DistanceBand distanceBand = this.m_proximitySensor.DistanceBands[i];
						DateTime timeOfLastUpdate = distanceBand.TimeOfLastUpdate;
						bool flag = distanceBand.CanUpdate();
						bool hasObservers = distanceBand.HasObservers;
						if (flag)
						{
							distanceBand.MarkUpdateTime(utcNow);
							if (hasObservers)
							{
								this.WriteReplicators(timeOfLastUpdate, distanceBand);
							}
						}
					}
					if (utcNow >= this.m_nextObserverRebuild)
					{
						this.m_proximitySensor.RebuildObservers();
						this.m_nextObserverRebuild = utcNow.AddSeconds(1.0);
					}
				}
				else
				{
					bool flag2 = false;
					int num2 = num - 1;
					for (int j = num2; j >= 0; j--)
					{
						DistanceBand distanceBand2 = this.m_proximitySensor.DistanceBands[j];
						DateTime timeOfLastUpdate2 = distanceBand2.TimeOfLastUpdate;
						bool flag3 = distanceBand2.CanUpdate();
						bool hasObservers2 = distanceBand2.HasObservers;
						if (flag3 || flag2)
						{
							distanceBand2.MarkUpdateTime(utcNow);
							if (hasObservers2)
							{
								this.WriteReplicators(timeOfLastUpdate2, distanceBand2);
							}
							distanceBand2.UpdateCount++;
							if (j == num2 && distanceBand2.UpdateCount > 1)
							{
								flag2 = true;
							}
							if (flag2)
							{
								distanceBand2.UpdateCount = 1;
							}
						}
					}
					if (flag2)
					{
						this.m_proximitySensor.RebuildObservers();
					}
				}
			}
			else
			{
				this.WriteReplicators(this.m_lastWriteTimestamp, null);
			}
			this.m_lastWriteTimestamp = utcNow;
		}

		// Token: 0x060021C8 RID: 8648 RVA: 0x000586AF File Offset: 0x000568AF
		private void WriteReplicators(DateTime timestamp, DistanceBand band)
		{
			this.m_syncReplicatorChannel.WriteReplicationData(timestamp, band, this.m_buffer);
			this.m_stateReplicatorChannel.WriteReplicationData(timestamp, band, this.m_buffer);
		}

		// Token: 0x060021C9 RID: 8649 RVA: 0x000586D7 File Offset: 0x000568D7
		public BitBuffer AddInitialState(BitBuffer outBuffer)
		{
			outBuffer.AddUniqueId(this.SpawnId);
			this.m_syncReplicatorChannel.AddInitialState(outBuffer);
			this.m_stateReplicatorChannel.AddInitialState(outBuffer);
			outBuffer.AddNullableUInt(this.IsReplacingId);
			return outBuffer;
		}

		// Token: 0x060021CA RID: 8650 RVA: 0x0005870C File Offset: 0x0005690C
		private BitBuffer ReadInitialState(BitBuffer inBuffer)
		{
			this.m_syncReplicatorChannel.ReadInitialState(inBuffer);
			this.m_stateReplicatorChannel.ReadInitialState(inBuffer);
			this.IsReplacingId = inBuffer.ReadNullableUInt();
			return inBuffer;
		}

		// Token: 0x060021CB RID: 8651 RVA: 0x00124F3C File Offset: 0x0012313C
		private void SendBulkDestroy()
		{
			if (this.IsServer)
			{
				this.m_buffer.AddHeader(this, OpCodes.Destroy, true);
				Packet packetFromBuffer = this.m_buffer.GetPacketFromBuffer(PacketFlags.Reliable);
				NetworkCommand networkCommand = NetworkCommandPool.GetNetworkCommand();
				networkCommand.Type = CommandType.BroadcastAll;
				networkCommand.Channel = NetworkChannel.Destroy_Server;
				networkCommand.Packet = packetFromBuffer;
				this.m_network.AddCommandToQueue(networkCommand);
			}
		}

		// Token: 0x060021CC RID: 8652 RVA: 0x00124F98 File Offset: 0x00123198
		private void InitProximity()
		{
			if (this.m_proximityPrefab == null)
			{
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_proximityPrefab, base.gameObject.transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			this.m_proximitySensor = gameObject.GetComponent<BaseProximitySensor>();
			this.m_proximitySensor.Init(this);
		}

		// Token: 0x060021CD RID: 8653 RVA: 0x00125004 File Offset: 0x00123204
		public Peer[] GetObserversFromDistanceBand(DistanceBand band, bool shouldIncludeSelf)
		{
			bool flag = shouldIncludeSelf && this.NetworkId.HasPeer && band.Index == 0;
			Peer[] array = PeerArrayPool.GetArray(flag ? (band.Observers.Count + 1) : band.Observers.Count);
			int num = 0;
			if (flag)
			{
				array[num] = this.NetworkId.Peer;
				num++;
			}
			for (int i = 0; i < band.Observers.Count; i++)
			{
				if (band.Observers[i].NetworkEntity != null && band.Observers[i].NetworkEntity != this && band.Observers[i].NetworkEntity.NetworkId.HasPeer)
				{
					array[num] = band.Observers[i].NetworkEntity.NetworkId.Peer;
					num++;
				}
			}
			return array;
		}

		// Token: 0x060021CE RID: 8654 RVA: 0x00125108 File Offset: 0x00123308
		public Peer[] GetAllObservers(bool includeSelf)
		{
			if (!this.UseProximity)
			{
				return null;
			}
			int num = this.m_proximitySensor.Observers.Count;
			if (includeSelf)
			{
				num++;
			}
			int num2 = 0;
			Peer[] array = PeerArrayPool.GetArray(num);
			if (includeSelf)
			{
				array[num2] = this.NetworkId.Peer;
				num2++;
			}
			for (int i = 0; i < this.m_proximitySensor.Observers.Count; i++)
			{
				if (this.m_proximitySensor.Observers[i].NetworkEntity != null && this.m_proximitySensor.Observers[i].NetworkEntity != this && this.m_proximitySensor.Observers[i].NetworkEntity.NetworkId.HasPeer)
				{
					array[num2] = this.m_proximitySensor.Observers[i].NetworkEntity.NetworkId.Peer;
					num2++;
				}
			}
			return array;
		}

		// Token: 0x060021CF RID: 8655 RVA: 0x00125218 File Offset: 0x00123418
		public Peer[] GetObserversWithinRange(float radius)
		{
			if (!GameManager.IsServer)
			{
				return null;
			}
			if (NetworkEntity.m_observersWithinRange == null)
			{
				NetworkEntity.m_observersWithinRange = new List<NetworkEntity>(10);
			}
			else
			{
				NetworkEntity.m_observersWithinRange.Clear();
			}
			ServerGameManager.SpatialManager.PhysicsQueryRadius(base.gameObject.transform.position, radius, NetworkEntity.m_observersWithinRange, true, null);
			if (NetworkEntity.m_observersWithinRange.Count <= 0)
			{
				return null;
			}
			int num = 0;
			for (int i = 0; i < NetworkEntity.m_observersWithinRange.Count; i++)
			{
				if (NetworkEntity.m_observersWithinRange[i] != null && NetworkEntity.m_observersWithinRange[i].NetworkId.HasPeer)
				{
					num++;
				}
			}
			if (num <= 0)
			{
				return null;
			}
			int num2 = 0;
			Peer[] array = PeerArrayPool.GetArray(num);
			for (int j = 0; j < NetworkEntity.m_observersWithinRange.Count; j++)
			{
				if (NetworkEntity.m_observersWithinRange[j] != null && NetworkEntity.m_observersWithinRange[j].NetworkId.HasPeer)
				{
					array[num2] = NetworkEntity.m_observersWithinRange[j].NetworkId.Peer;
					num2++;
				}
			}
			NetworkEntity.m_observersWithinRange.Clear();
			return array;
		}

		// Token: 0x060021D0 RID: 8656 RVA: 0x00125350 File Offset: 0x00123550
		public void BroadcastSpawnPacket()
		{
			this.m_buffer.AddHeader(this, OpCodes.Spawn, true);
			this.m_buffer.AddInitialState(this);
			Packet packetFromBuffer = this.m_buffer.GetPacketFromBuffer(PacketFlags.Reliable);
			NetworkCommand networkCommand = NetworkCommandPool.GetNetworkCommand();
			networkCommand.Type = CommandType.BroadcastAll;
			networkCommand.Channel = NetworkChannel.Spawn_Other;
			networkCommand.Packet = packetFromBuffer;
			this.m_network.AddCommandToQueue(networkCommand);
		}

		// Token: 0x060021D1 RID: 8657 RVA: 0x001253B0 File Offset: 0x001235B0
		public void SendSpawnPacket(NetworkEntity netEntity)
		{
			if (netEntity == this)
			{
				return;
			}
			this.m_buffer.AddHeader(this, OpCodes.Spawn, true);
			this.m_buffer.AddInitialState(this);
			Packet packetFromBuffer = this.m_buffer.GetPacketFromBuffer(PacketFlags.Reliable);
			NetworkCommand networkCommand = NetworkCommandPool.GetNetworkCommand();
			networkCommand.Type = CommandType.Send;
			networkCommand.Target = netEntity.NetworkId.Peer;
			networkCommand.Channel = NetworkChannel.Spawn_Other;
			networkCommand.Packet = packetFromBuffer;
			this.m_network.AddCommandToQueue(networkCommand);
		}

		// Token: 0x060021D2 RID: 8658 RVA: 0x0012542C File Offset: 0x0012362C
		public void SendDestroyPacket(NetworkEntity netEntity)
		{
			if (netEntity == this)
			{
				return;
			}
			this.m_buffer.AddHeader(this, OpCodes.Destroy, true);
			Packet packetFromBuffer = this.m_buffer.GetPacketFromBuffer(PacketFlags.Reliable);
			NetworkCommand networkCommand = NetworkCommandPool.GetNetworkCommand();
			networkCommand.Type = CommandType.Send;
			networkCommand.Target = netEntity.NetworkId.Peer;
			networkCommand.Channel = NetworkChannel.Destroy_Server;
			networkCommand.Packet = packetFromBuffer;
			this.m_network.AddCommandToQueue(networkCommand);
		}

		// Token: 0x170006ED RID: 1773
		// (get) Token: 0x060021D3 RID: 8659 RVA: 0x00058733 File Offset: 0x00056933
		// (set) Token: 0x060021D4 RID: 8660 RVA: 0x0005873B File Offset: 0x0005693B
		public SpawnController SpawnController { get; set; }

		// Token: 0x040025EE RID: 9710
		private const float kOffsetMax = 5f;

		// Token: 0x040025EF RID: 9711
		private const float kOffsetDelta = 0.1f;

		// Token: 0x040025F0 RID: 9712
		private static float m_updateOffset = 0f;

		// Token: 0x040025F1 RID: 9713
		private static uint m_currentNetworkIdValue = 1U;

		// Token: 0x040025F3 RID: 9715
		public int? BoundingSphereIndex;

		// Token: 0x040025F4 RID: 9716
		[SerializeField]
		private UniqueId m_spawnId;

		// Token: 0x040025F5 RID: 9717
		[SerializeField]
		private UniqueId m_prefabId = UniqueId.Empty;

		// Token: 0x040025F6 RID: 9718
		public uint? IsReplacingId;

		// Token: 0x040025F7 RID: 9719
		[SerializeField]
		private GameObject m_proximityPrefab;

		// Token: 0x040025F8 RID: 9720
		[SerializeField]
		private float m_updateRate = 0.1f;

		// Token: 0x040025F9 RID: 9721
		[SerializeField]
		private NetworkObjectInitSettings[] m_objects;

		// Token: 0x040025FA RID: 9722
		[SerializeField]
		private ReplicatorChannel m_syncReplicatorChannel;

		// Token: 0x040025FB RID: 9723
		[SerializeField]
		private ReplicatorChannel m_stateReplicatorChannel;

		// Token: 0x040025FC RID: 9724
		private BaseProximitySensor m_proximitySensor;

		// Token: 0x040025FD RID: 9725
		private BitBuffer m_buffer;

		// Token: 0x040025FE RID: 9726
		private INetworkManager m_network;

		// Token: 0x040025FF RID: 9727
		private RpcHandler m_rpcHandler;

		// Token: 0x04002600 RID: 9728
		private bool m_isWorldEntity;

		// Token: 0x04002601 RID: 9729
		private bool m_initialized;

		// Token: 0x04002602 RID: 9730
		private DateTime m_lastWriteTimestamp = DateTime.MinValue;

		// Token: 0x04002603 RID: 9731
		private DateTime m_nextUpdate = DateTime.MinValue;

		// Token: 0x04002604 RID: 9732
		private DateTime m_nextObserverRebuild = DateTime.MinValue;

		// Token: 0x04002608 RID: 9736
		private PlayerRpcHandler m_playerRpcHandler;

		// Token: 0x04002609 RID: 9737
		private NpcRpcHandler m_npcRpcHandler;

		// Token: 0x04002613 RID: 9747
		private static List<NetworkEntity> m_observersWithinRange = null;

		// Token: 0x020004B5 RID: 1205
		private class ReplicatorConfig
		{
			// Token: 0x04002615 RID: 9749
			public List<IReplicator> Replicators;

			// Token: 0x04002616 RID: 9750
			public OpCodes OpCode;

			// Token: 0x04002617 RID: 9751
			public NetworkChannel Channel;

			// Token: 0x04002618 RID: 9752
			public PacketFlags PacketFlags;

			// Token: 0x04002619 RID: 9753
			public bool ConsiderProximity;

			// Token: 0x0400261A RID: 9754
			public bool IncludeSelf;
		}
	}
}
