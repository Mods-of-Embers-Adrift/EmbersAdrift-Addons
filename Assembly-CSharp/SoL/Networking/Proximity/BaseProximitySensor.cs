using System;
using SoL.Game;
using SoL.Managers;
using SoL.Networking.Managers;
using SoL.Networking.Objects;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Networking.Proximity
{
	// Token: 0x020004AA RID: 1194
	public abstract class BaseProximitySensor : GameEntityComponent
	{
		// Token: 0x0600214D RID: 8525
		protected abstract void RebuildObserversInternal();

		// Token: 0x170006D4 RID: 1748
		// (get) Token: 0x0600214E RID: 8526 RVA: 0x0005815C File Offset: 0x0005635C
		public DistanceBand[] DistanceBands
		{
			get
			{
				return this.m_distanceBands;
			}
		}

		// Token: 0x0600214F RID: 8527 RVA: 0x00123914 File Offset: 0x00121B14
		protected virtual void Awake()
		{
			if (ServerGameManager.PlayerProximityConfig != null && base.GameEntity != null && base.GameEntity.Type == GameEntityType.Player)
			{
				this.m_distanceBands = new DistanceBand[ServerGameManager.PlayerProximityConfig.DistanceBandConfigs.Length];
				for (int i = 0; i < this.m_distanceBands.Length; i++)
				{
					this.m_distanceBands[i] = new DistanceBand(ServerGameManager.PlayerProximityConfig.DistanceBandConfigs[i].Distance, ServerGameManager.PlayerProximityConfig.DistanceBandConfigs[i].UpdateTime);
				}
			}
			this.m_server = (NetworkManager.EntityManager as ServerNetworkEntityManager);
			for (int j = 0; j < this.m_distanceBands.Length; j++)
			{
				this.m_distanceBands[j].Index = j;
			}
		}

		// Token: 0x06002150 RID: 8528 RVA: 0x00058164 File Offset: 0x00056364
		protected virtual void OnDestroy()
		{
			this.m_owner = null;
			this.m_server = null;
		}

		// Token: 0x06002151 RID: 8529 RVA: 0x001239D0 File Offset: 0x00121BD0
		public void Init(NetworkEntity parent)
		{
			this.m_owner = parent;
			for (int i = 0; i < this.m_distanceBands.Length; i++)
			{
				this.m_distanceBands[i].TimeOfLastUpdate = DateTime.UtcNow;
			}
		}

		// Token: 0x06002152 RID: 8530 RVA: 0x00123A0C File Offset: 0x00121C0C
		public void RebuildObservers()
		{
			for (int i = 0; i < this.Observers.Count; i++)
			{
				this.Observers[i].PreRebuild();
			}
			this.RebuildObserversInternal();
			for (int j = 0; j < this.Observers.Count; j++)
			{
				Observer observer = this.Observers[j];
				if (observer.Added)
				{
					this.m_owner.SendSpawnPacket(observer.NetworkEntity);
				}
				else if (observer.Remove)
				{
					this.m_owner.SendDestroyPacket(observer.NetworkEntity);
					DistanceBand distanceBand = observer.DistanceBand;
					if (distanceBand != null)
					{
						distanceBand.RemoveObserver(observer);
					}
					this.Observers.Remove(observer.Id);
					StaticPool<Observer>.ReturnToPool(observer);
					j--;
				}
			}
		}

		// Token: 0x040025C9 RID: 9673
		[SerializeField]
		protected DistanceBand[] m_distanceBands;

		// Token: 0x040025CA RID: 9674
		public readonly DictionaryList<int, Observer> Observers = new DictionaryList<int, Observer>(false);

		// Token: 0x040025CB RID: 9675
		protected NetworkEntity m_owner;

		// Token: 0x040025CC RID: 9676
		protected ServerNetworkEntityManager m_server;
	}
}
