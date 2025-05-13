using System;
using ENet;
using NetStack.Serialization;
using SoL.Networking.Database;
using SoL.Networking.Objects;
using UnityEngine;

namespace SoL.Networking.Managers
{
	// Token: 0x020004BE RID: 1214
	public abstract class BaseNetworkEntityManager : MonoBehaviour
	{
		// Token: 0x060021F6 RID: 8694
		public abstract void RegisterWorldEntity(NetworkEntity entity);

		// Token: 0x060021F7 RID: 8695
		public abstract void DeregisterWorldEntity(NetworkEntity entity);

		// Token: 0x060021F8 RID: 8696 RVA: 0x00125D3C File Offset: 0x00123F3C
		public virtual void RegisterEntity(NetworkEntity entity)
		{
			this.m_networkEntities.Add(entity.NetworkId.Value, entity);
		}

		// Token: 0x060021F9 RID: 8697 RVA: 0x00125D64 File Offset: 0x00123F64
		public virtual void DeregisterEntity(NetworkEntity entity)
		{
			this.m_networkEntities.Remove(entity.NetworkId.Value);
		}

		// Token: 0x060021FA RID: 8698 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void SpawnRemoteEntity(Peer peer, UserRecord user, CharacterRecord record, GameObject prefab)
		{
		}

		// Token: 0x060021FB RID: 8699 RVA: 0x00049FFA File Offset: 0x000481FA
		public virtual NetworkEntity InitEntity(NetworkedPrefabCollection networkedPrefabs, uint id, BitBuffer inBuffer, byte channel)
		{
			return null;
		}

		// Token: 0x060021FC RID: 8700 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void UpdateBoundingSpheres()
		{
		}

		// Token: 0x060021FD RID: 8701 RVA: 0x00049FFA File Offset: 0x000481FA
		public virtual NetworkEntity GetEntityForSphereIndex(int index)
		{
			return null;
		}

		// Token: 0x060021FE RID: 8702
		protected abstract void UpdateStates();

		// Token: 0x170006F6 RID: 1782
		// (get) Token: 0x060021FF RID: 8703 RVA: 0x00058886 File Offset: 0x00056A86
		public static int PlayerConnectedCount
		{
			get
			{
				if (BaseNetworkEntityManager.Peers != null)
				{
					return BaseNetworkEntityManager.Peers.Count;
				}
				return 0;
			}
		}

		// Token: 0x06002200 RID: 8704 RVA: 0x00125D8C File Offset: 0x00123F8C
		public NetworkEntity GetNetEntityForId(uint id)
		{
			NetworkEntity result = null;
			this.m_networkEntities.TryGetValue(id, out result);
			return result;
		}

		// Token: 0x06002201 RID: 8705 RVA: 0x0005889B File Offset: 0x00056A9B
		public bool TryGetNetworkEntity(uint id, out NetworkEntity netEntity)
		{
			return this.m_networkEntities.TryGetValue(id, out netEntity);
		}

		// Token: 0x06002202 RID: 8706 RVA: 0x000588AA File Offset: 0x00056AAA
		private void Update()
		{
			this.UpdateStates();
			this.UpdateBoundingSpheres();
		}

		// Token: 0x0400263C RID: 9788
		protected readonly BitBuffer m_buffer = new BitBuffer(375);

		// Token: 0x0400263D RID: 9789
		protected readonly DictionaryList<uint, NetworkEntity> m_networkEntities = new DictionaryList<uint, NetworkEntity>(true);

		// Token: 0x0400263E RID: 9790
		public static PlayerCollection Peers;
	}
}
