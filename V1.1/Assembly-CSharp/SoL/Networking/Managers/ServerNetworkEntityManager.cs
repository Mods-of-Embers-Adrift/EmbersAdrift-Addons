using System;
using System.Collections.Generic;
using ENet;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.Objects;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Networking.Managers
{
	// Token: 0x020004C0 RID: 1216
	public class ServerNetworkEntityManager : BaseNetworkEntityManager
	{
		// Token: 0x170006F7 RID: 1783
		// (get) Token: 0x06002209 RID: 8713 RVA: 0x00058908 File Offset: 0x00056B08
		private bool m_playersUseProximity
		{
			get
			{
				return GlobalSettings.Values.Player.PlayersUseProximity;
			}
		}

		// Token: 0x170006F8 RID: 1784
		// (get) Token: 0x0600220A RID: 8714 RVA: 0x00058919 File Offset: 0x00056B19
		private PlayerCollection m_peers
		{
			get
			{
				return BaseNetworkEntityManager.Peers;
			}
		}

		// Token: 0x1400003B RID: 59
		// (add) Token: 0x0600220B RID: 8715 RVA: 0x00125EEC File Offset: 0x001240EC
		// (remove) Token: 0x0600220C RID: 8716 RVA: 0x00125F24 File Offset: 0x00124124
		public event Action<int> SphereCountUpdated;

		// Token: 0x170006F9 RID: 1785
		// (get) Token: 0x0600220D RID: 8717 RVA: 0x00058920 File Offset: 0x00056B20
		public int CurrentSphereCount
		{
			get
			{
				return this.m_currentIndex + 1;
			}
		}

		// Token: 0x170006FA RID: 1786
		// (get) Token: 0x0600220E RID: 8718 RVA: 0x0005892A File Offset: 0x00056B2A
		// (set) Token: 0x0600220F RID: 8719 RVA: 0x00058932 File Offset: 0x00056B32
		public SolPerformanceTimer PerformanceTimer { get; private set; }

		// Token: 0x06002210 RID: 8720 RVA: 0x0005893B File Offset: 0x00056B3B
		private void Awake()
		{
			BaseNetworkEntityManager.Peers = new PlayerCollection(true);
			this.PerformanceTimer = new SolPerformanceTimer();
		}

		// Token: 0x06002211 RID: 8721 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void UpdateBoundingSpheres()
		{
		}

		// Token: 0x06002212 RID: 8722 RVA: 0x00125F5C File Offset: 0x0012415C
		public override NetworkEntity GetEntityForSphereIndex(int index)
		{
			NetworkEntity result = null;
			this.m_sphereIndexToPlayer.TryGetValue(index, out result);
			return result;
		}

		// Token: 0x06002213 RID: 8723 RVA: 0x00125F7C File Offset: 0x0012417C
		public override void RegisterWorldEntity(NetworkEntity entity)
		{
			entity.ServerInit(default(Peer), false, false);
		}

		// Token: 0x06002214 RID: 8724 RVA: 0x0004475B File Offset: 0x0004295B
		public override void DeregisterWorldEntity(NetworkEntity entity)
		{
		}

		// Token: 0x06002215 RID: 8725 RVA: 0x00125F9C File Offset: 0x0012419C
		public override void RegisterEntity(NetworkEntity entity)
		{
			base.RegisterEntity(entity);
			if (entity.NetworkId.HasPeer)
			{
				this.m_peers.Add(entity.NetworkId.Value, entity);
				this.UpdateFrameLimiter();
				if (this.m_freeIndexes.Count > 0)
				{
					entity.BoundingSphereIndex = new int?(this.m_freeIndexes[0]);
					this.m_freeIndexes.RemoveAt(0);
				}
				else
				{
					entity.BoundingSphereIndex = new int?(this.m_currentIndex);
					this.m_currentIndex++;
					Action<int> sphereCountUpdated = this.SphereCountUpdated;
					if (sphereCountUpdated != null)
					{
						sphereCountUpdated(this.CurrentSphereCount);
					}
				}
				this.m_sphereIndexToPlayer.Add(entity.BoundingSphereIndex.Value, entity);
				this.PlayerSpheres[entity.BoundingSphereIndex.Value].position = entity.gameObject.transform.position;
				this.PlayerSpheres[entity.BoundingSphereIndex.Value].radius = 1f;
			}
		}

		// Token: 0x06002216 RID: 8726 RVA: 0x001260B0 File Offset: 0x001242B0
		public override void DeregisterEntity(NetworkEntity entity)
		{
			base.DeregisterEntity(entity);
			if (this.m_peers.Remove(entity.NetworkId.Value))
			{
				this.UpdateFrameLimiter();
			}
			if (entity.BoundingSphereIndex != null)
			{
				this.m_freeIndexes.Add(entity.BoundingSphereIndex.Value);
				this.m_sphereIndexToPlayer.Remove(entity.BoundingSphereIndex.Value);
				this.PlayerSpheres[entity.BoundingSphereIndex.Value] = default(BoundingSphere);
				entity.BoundingSphereIndex = null;
			}
		}

		// Token: 0x06002217 RID: 8727 RVA: 0x00058953 File Offset: 0x00056B53
		private void UpdateFrameLimiter()
		{
			Application.targetFrameRate = ((this.m_peers.Count > 0) ? ServerGameManager.GameServerConfig.MaxFramerate : ServerGameManager.GameServerConfig.MinFramerate);
		}

		// Token: 0x06002218 RID: 8728 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void UpdateStates()
		{
		}

		// Token: 0x06002219 RID: 8729 RVA: 0x0004475B File Offset: 0x0004295B
		private void FlushObservers()
		{
		}

		// Token: 0x0600221A RID: 8730 RVA: 0x0004475B File Offset: 0x0004295B
		public override void SpawnRemoteEntity(Peer peer, UserRecord user, CharacterRecord record, GameObject prefab)
		{
		}

		// Token: 0x0600221B RID: 8731 RVA: 0x0004475B File Offset: 0x0004295B
		internal void SpawnOthersForRemoteEntity(NetworkEntity netEntity)
		{
		}

		// Token: 0x0600221C RID: 8732 RVA: 0x0004475B File Offset: 0x0004295B
		public void SpawnNetworkEntityForRemoteClients(NetworkEntity entity)
		{
		}

		// Token: 0x0600221D RID: 8733 RVA: 0x0005897E File Offset: 0x00056B7E
		public static bool TryGetNetworkEntityByName(string playerName, out NetworkEntity netEntity)
		{
			netEntity = null;
			return false;
		}

		// Token: 0x0600221E RID: 8734 RVA: 0x00045BCA File Offset: 0x00043DCA
		public static bool DisconnectNetworkEntity(NetworkEntity netEntity)
		{
			return false;
		}

		// Token: 0x0600221F RID: 8735 RVA: 0x00045BCA File Offset: 0x00043DCA
		public static int DisconnectAll()
		{
			return 0;
		}

		// Token: 0x04002640 RID: 9792
		public const int kBoundingSpheres = 300;

		// Token: 0x04002642 RID: 9794
		public readonly BoundingSphere[] PlayerSpheres = new BoundingSphere[300];

		// Token: 0x04002643 RID: 9795
		private readonly Dictionary<int, NetworkEntity> m_sphereIndexToPlayer = new Dictionary<int, NetworkEntity>();

		// Token: 0x04002644 RID: 9796
		private readonly List<int> m_freeIndexes = new List<int>();

		// Token: 0x04002645 RID: 9797
		private int m_currentIndex;

		// Token: 0x04002647 RID: 9799
		private readonly List<NetworkEntity> m_bulkSpawnEntities = new List<NetworkEntity>(128);

		// Token: 0x04002648 RID: 9800
		private bool m_flushedObservers;
	}
}
