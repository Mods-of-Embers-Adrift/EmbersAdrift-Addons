using System;
using System.Collections.Generic;
using DataStructures.ViliWonka.KDTree;
using ENet;
using NetStack.Serialization;
using SoL.Game;
using SoL.Game.Dueling;
using SoL.Game.Messages;
using SoL.Networking;
using SoL.Networking.Managers;
using SoL.Networking.Objects;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Managers
{
	// Token: 0x02000545 RID: 1349
	public class SpatialManager : MonoBehaviour
	{
		// Token: 0x06002904 RID: 10500 RVA: 0x0005C5FC File Offset: 0x0005A7FC
		private void Start()
		{
			this.m_nextRebuildTime = this.m_nextRebuildTime.AddSeconds(1.0);
		}

		// Token: 0x06002905 RID: 10501 RVA: 0x0005C618 File Offset: 0x0005A818
		private void Update()
		{
			if (DateTime.UtcNow >= this.m_nextRebuildTime)
			{
				this.RebuildTree();
			}
		}

		// Token: 0x06002906 RID: 10502 RVA: 0x0005C632 File Offset: 0x0005A832
		public void RegisterEntity(NetworkEntity entity)
		{
			this.m_entities.Add(entity);
		}

		// Token: 0x06002907 RID: 10503 RVA: 0x0005C640 File Offset: 0x0005A840
		public void UnregisterEntity(NetworkEntity entity)
		{
			this.m_entities.Remove(entity);
		}

		// Token: 0x06002908 RID: 10504 RVA: 0x0005C64F File Offset: 0x0005A84F
		public void ForceRebuild()
		{
			this.RebuildTree();
		}

		// Token: 0x06002909 RID: 10505 RVA: 0x0013F6B4 File Offset: 0x0013D8B4
		private void RebuildTree()
		{
			int count = this.m_entities.Count;
			this.m_tree.Rebuild(this.m_entities, -1);
			this.m_nextRebuildTime = DateTime.UtcNow.AddSeconds(1.0);
		}

		// Token: 0x0600290A RID: 10506 RVA: 0x0013F700 File Offset: 0x0013D900
		public void QueryRadius(Vector3 sourcePoint, float radius, List<NetworkEntity> results)
		{
			this.m_resultIndicies.Clear();
			this.m_query.Radius(this.m_tree, sourcePoint, radius, this.m_resultIndicies);
			results.Clear();
			for (int i = 0; i < this.m_resultIndicies.Count; i++)
			{
				int num = this.m_resultIndicies[i];
				if (num < this.m_entities.Count)
				{
					results.Add(this.m_entities[num]);
				}
			}
		}

		// Token: 0x0600290B RID: 10507 RVA: 0x0005C657 File Offset: 0x0005A857
		public List<NetworkEntity> QueryRadius(Vector3 sourcePoint, float radius)
		{
			SpatialManager.m_nearbyQuery.Clear();
			this.QueryRadius(sourcePoint, radius, SpatialManager.m_nearbyQuery);
			return SpatialManager.m_nearbyQuery;
		}

		// Token: 0x0600290C RID: 10508 RVA: 0x0013F77C File Offset: 0x0013D97C
		public void PhysicsQueryRadius(Vector3 sourcePoint, float radius, List<NetworkEntity> results, bool playerOnly, Collider[] hitColliderOverride = null)
		{
			results.Clear();
			Collider[] array = (hitColliderOverride != null && hitColliderOverride.Length != 0) ? hitColliderOverride : Hits.Colliders100;
			int num = Physics.OverlapSphereNonAlloc(sourcePoint, radius, array, LayerMap.Detection.LayerMask, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < num; i++)
			{
				GameEntity gameEntity;
				if (array[i] && DetectionCollider.TryGetEntityForCollider(array[i], out gameEntity) && gameEntity.NetworkEntity && (!playerOnly || gameEntity.Type == GameEntityType.Player))
				{
					results.Add(gameEntity.NetworkEntity);
				}
			}
		}

		// Token: 0x0600290D RID: 10509 RVA: 0x0013F804 File Offset: 0x0013DA04
		private void BroadcastNearby(OpCodes op, NetworkChannel netChannel, INetworkSerializable data, GameEntity sourceEntity, float radius, bool excludeSource, bool usePhysicsQuery)
		{
			if (!sourceEntity)
			{
				throw new ArgumentNullException("sourceEntity");
			}
			if (!sourceEntity.NetworkEntity)
			{
				throw new ArgumentNullException("NetworkEntity");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			SpatialManager.m_nearbyEntities.Clear();
			if (usePhysicsQuery)
			{
				this.PhysicsQueryRadius(sourceEntity.gameObject.transform.position, radius, SpatialManager.m_nearbyQuery, false, null);
			}
			else
			{
				this.QueryRadius(sourceEntity.gameObject.transform.position, radius, SpatialManager.m_nearbyQuery);
			}
			for (int i = 0; i < SpatialManager.m_nearbyQuery.Count; i++)
			{
				if (SpatialManager.m_nearbyQuery[i] != null && SpatialManager.m_nearbyQuery[i].GameEntity.Type == GameEntityType.Player && SpatialManager.m_nearbyQuery[i].NetworkId.HasPeer && (!excludeSource || SpatialManager.m_nearbyQuery[i].GameEntity != sourceEntity))
				{
					SpatialManager.m_nearbyEntities.Add(SpatialManager.m_nearbyQuery[i]);
				}
			}
			if (SpatialManager.m_nearbyEntities.Count <= 0)
			{
				return;
			}
			Peer[] array = PeerArrayPool.GetArray(SpatialManager.m_nearbyEntities.Count);
			for (int j = 0; j < SpatialManager.m_nearbyEntities.Count; j++)
			{
				array[j] = SpatialManager.m_nearbyEntities[j].NetworkId.Peer;
			}
			BitBuffer fromPool = BitBufferExtensions.GetFromPool();
			fromPool.AddHeader(sourceEntity.NetworkEntity, op, true);
			data.PackData(fromPool);
			Packet packetFromBuffer_ReturnBufferToPool = fromPool.GetPacketFromBuffer_ReturnBufferToPool(PacketFlags.Reliable);
			NetworkCommand networkCommand = NetworkCommandPool.GetNetworkCommand();
			networkCommand.Channel = netChannel;
			networkCommand.Packet = packetFromBuffer_ReturnBufferToPool;
			networkCommand.Type = CommandType.BroadcastGroup;
			networkCommand.TargetGroup = array;
			GameManager.NetworkManager.AddCommandToQueue(networkCommand);
			SpatialManager.m_nearbyEntities.Clear();
		}

		// Token: 0x0600290E RID: 10510 RVA: 0x0013F9EC File Offset: 0x0013DBEC
		private void BroadcastAll(OpCodes op, NetworkChannel netChannel, INetworkSerializable data, GameEntity sourceEntity)
		{
			if (!sourceEntity)
			{
				throw new ArgumentNullException("sourceEntity");
			}
			if (!sourceEntity.NetworkEntity)
			{
				throw new ArgumentNullException("NetworkEntity");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (BaseNetworkEntityManager.PlayerConnectedCount <= 0)
			{
				return;
			}
			Peer[] array = PeerArrayPool.GetArray(BaseNetworkEntityManager.Peers.Count);
			for (int i = 0; i < BaseNetworkEntityManager.Peers.Count; i++)
			{
				NetworkEntity networkEntity = BaseNetworkEntityManager.Peers[i];
				if (networkEntity && networkEntity.NetworkId.HasPeer)
				{
					array[i] = networkEntity.NetworkId.Peer;
				}
			}
			BitBuffer fromPool = BitBufferExtensions.GetFromPool();
			fromPool.AddHeader(sourceEntity.NetworkEntity, op, true);
			data.PackData(fromPool);
			Packet packetFromBuffer_ReturnBufferToPool = fromPool.GetPacketFromBuffer_ReturnBufferToPool(PacketFlags.Reliable);
			NetworkCommand networkCommand = NetworkCommandPool.GetNetworkCommand();
			networkCommand.Channel = netChannel;
			networkCommand.Packet = packetFromBuffer_ReturnBufferToPool;
			networkCommand.Type = CommandType.BroadcastGroup;
			networkCommand.TargetGroup = array;
			GameManager.NetworkManager.AddCommandToQueue(networkCommand);
			SpatialManager.m_nearbyEntities.Clear();
		}

		// Token: 0x0600290F RID: 10511 RVA: 0x0013FB04 File Offset: 0x0013DD04
		private void BroadcastAll(OpCodes op, NetworkChannel netChannel, INetworkSerializable data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (BaseNetworkEntityManager.PlayerConnectedCount <= 0)
			{
				return;
			}
			Peer[] array = PeerArrayPool.GetArray(BaseNetworkEntityManager.Peers.Count);
			for (int i = 0; i < BaseNetworkEntityManager.Peers.Count; i++)
			{
				NetworkEntity networkEntity = BaseNetworkEntityManager.Peers[i];
				if (networkEntity && networkEntity.NetworkId.HasPeer)
				{
					array[i] = networkEntity.NetworkId.Peer;
				}
			}
			BitBuffer fromPool = BitBufferExtensions.GetFromPool();
			fromPool.Clear();
			fromPool.AddUShort((ushort)op);
			fromPool.AddUInt(0U);
			data.PackData(fromPool);
			Packet packetFromBuffer_ReturnBufferToPool = fromPool.GetPacketFromBuffer_ReturnBufferToPool(PacketFlags.Reliable);
			NetworkCommand networkCommand = NetworkCommandPool.GetNetworkCommand();
			networkCommand.Channel = netChannel;
			networkCommand.Packet = packetFromBuffer_ReturnBufferToPool;
			networkCommand.Type = CommandType.BroadcastGroup;
			networkCommand.TargetGroup = array;
			GameManager.NetworkManager.AddCommandToQueue(networkCommand);
			SpatialManager.m_nearbyEntities.Clear();
		}

		// Token: 0x06002910 RID: 10512 RVA: 0x0013FBF8 File Offset: 0x0013DDF8
		public void MessageNearbyPlayers(GameEntity sourceEntity, float radius, MessageType msgType, string msg, MessageEventType messageEventType = MessageEventType.None, bool excludeSource = false)
		{
			MessageDelivery messageDelivery = new MessageDelivery
			{
				Text = msg,
				Type = msgType,
				EventType = messageEventType
			};
			this.BroadcastNearby(OpCodes.ChatMessage, NetworkChannel.GenericChatMessage, messageDelivery, sourceEntity, radius, excludeSource, false);
		}

		// Token: 0x06002911 RID: 10513 RVA: 0x0005C675 File Offset: 0x0005A875
		public void DuelResultNearbyPlayers(GameEntity sourceEntity, float radius, DuelRoll roll)
		{
			this.BroadcastNearby(OpCodes.DuelRoll, NetworkChannel.GenericChatMessage, roll, sourceEntity, radius, false, true);
		}

		// Token: 0x06002912 RID: 10514 RVA: 0x0005C68B File Offset: 0x0005A88B
		public void DuelResultNotifyAllPlayers(GameEntity sourceEntity, DuelRoll roll)
		{
			this.BroadcastAll(OpCodes.DuelRoll, NetworkChannel.GenericChatMessage, roll, sourceEntity);
		}

		// Token: 0x06002913 RID: 10515 RVA: 0x0005C69E File Offset: 0x0005A89E
		public void LocationEventAllPlayers(GameEntity sourceEntity, LocationEvent locationEvent)
		{
			this.BroadcastAll(OpCodes.LocationEvent, NetworkChannel.State_Server, locationEvent);
		}

		// Token: 0x04002A01 RID: 10753
		private const float kCadence = 1f;

		// Token: 0x04002A02 RID: 10754
		private readonly KDTree m_tree = new KDTree(16);

		// Token: 0x04002A03 RID: 10755
		private readonly KDQuery m_query = new KDQuery(2048);

		// Token: 0x04002A04 RID: 10756
		private readonly List<int> m_resultIndicies = new List<int>(100);

		// Token: 0x04002A05 RID: 10757
		private readonly List<NetworkEntity> m_entities = new List<NetworkEntity>(100);

		// Token: 0x04002A06 RID: 10758
		private DateTime m_nextRebuildTime = DateTime.MinValue;

		// Token: 0x04002A07 RID: 10759
		private static readonly List<NetworkEntity> m_nearbyQuery = new List<NetworkEntity>(100);

		// Token: 0x04002A08 RID: 10760
		private static readonly List<NetworkEntity> m_nearbyEntities = new List<NetworkEntity>(100);
	}
}
