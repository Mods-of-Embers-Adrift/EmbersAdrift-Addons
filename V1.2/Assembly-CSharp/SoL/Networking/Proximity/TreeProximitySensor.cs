using System;
using System.Collections.Generic;
using SoL.Game;
using SoL.Managers;
using SoL.Networking.Objects;
using SoL.Utilities;
using Unity.Mathematics;

namespace SoL.Networking.Proximity
{
	// Token: 0x020004B3 RID: 1203
	public class TreeProximitySensor : BaseProximitySensor
	{
		// Token: 0x0600218B RID: 8587 RVA: 0x00124068 File Offset: 0x00122268
		protected override void Awake()
		{
			base.Awake();
			this.m_sqrDistances = new float[this.m_distanceBands.Length];
			for (int i = 0; i < this.m_distanceBands.Length; i++)
			{
				this.m_sqrDistances[i] = this.m_distanceBands[i].Distance * this.m_distanceBands[i].Distance;
				if (this.m_distanceBands[i].Distance > this.m_maxDistance)
				{
					this.m_maxDistance = this.m_distanceBands[i].Distance;
				}
			}
		}

		// Token: 0x0600218C RID: 8588 RVA: 0x001240EC File Offset: 0x001222EC
		protected override void RebuildObserversInternal()
		{
			ServerGameManager.SpatialManager.QueryRadius(base.gameObject.transform.position, this.m_maxDistance, TreeProximitySensor.m_entitiesWithinRange);
			float3 x = new float3(base.gameObject.transform.position);
			for (int i = 0; i < TreeProximitySensor.m_entitiesWithinRange.Count; i++)
			{
				NetworkEntity networkEntity = TreeProximitySensor.m_entitiesWithinRange[i];
				if (networkEntity && networkEntity.GameEntity && networkEntity.GameEntity.Type == GameEntityType.Player)
				{
					float3 y = new float3(networkEntity.transform.position);
					float sqrD = math.distancesq(x, y);
					int bandIndex = this.GetBandIndex(sqrD);
					if (bandIndex != -1)
					{
						DistanceBand distanceBand = this.m_distanceBands[bandIndex];
						Observer observer = null;
						if (this.Observers.TryGetValue((int)networkEntity.NetworkId.Value, out observer))
						{
							observer.Remove = false;
							if (!observer.DistanceBand.Equals(distanceBand))
							{
								distanceBand.AddObserver(observer);
							}
						}
						else
						{
							observer = StaticPool<Observer>.GetFromPool();
							observer.NetworkEntity = networkEntity;
							observer.Id = (int)networkEntity.NetworkId.Value;
							observer.Added = true;
							observer.Remove = false;
							distanceBand.AddObserver(observer);
							this.Observers.Add((int)networkEntity.NetworkId.Value, observer);
						}
					}
				}
			}
			TreeProximitySensor.m_entitiesWithinRange.Clear();
		}

		// Token: 0x0600218D RID: 8589 RVA: 0x00124274 File Offset: 0x00122474
		private int GetBandIndex(float sqrD)
		{
			for (int i = 0; i < this.m_sqrDistances.Length; i++)
			{
				if (sqrD <= this.m_sqrDistances[i])
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x040025EB RID: 9707
		private float[] m_sqrDistances;

		// Token: 0x040025EC RID: 9708
		private float m_maxDistance;

		// Token: 0x040025ED RID: 9709
		private static readonly List<NetworkEntity> m_entitiesWithinRange = new List<NetworkEntity>();
	}
}
