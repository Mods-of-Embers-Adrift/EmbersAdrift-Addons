using System;
using SoL.Game;
using SoL.Networking.Managers;
using SoL.Networking.Objects;
using SoL.Utilities;
using Unity.Mathematics;

namespace SoL.Networking.Proximity
{
	// Token: 0x020004AD RID: 1197
	public class DistanceProximitySensor : BaseProximitySensor
	{
		// Token: 0x0600216B RID: 8555 RVA: 0x00123CF8 File Offset: 0x00121EF8
		private void Start()
		{
			DistanceBand[] array;
			if (base.GameEntity != null && base.GameEntity.Type == GameEntityType.Npc && ZoneSettings.SettingsProfile != null && ZoneSettings.SettingsProfile.TryGetNpcDistanceBandOverrides(out array))
			{
				this.m_distanceBands = new DistanceBand[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					this.m_distanceBands[i] = new DistanceBand(array[i].Distance, array[i].UpdateTime);
					this.m_distanceBands[i].Index = i;
				}
			}
			this.m_sqrDistances = new float[this.m_distanceBands.Length];
			for (int j = 0; j < this.m_distanceBands.Length; j++)
			{
				this.m_sqrDistances[j] = this.m_distanceBands[j].Distance * this.m_distanceBands[j].Distance;
			}
		}

		// Token: 0x0600216C RID: 8556 RVA: 0x00123DCC File Offset: 0x00121FCC
		protected override void RebuildObserversInternal()
		{
			float3 x = new float3(base.gameObject.transform.position);
			for (int i = 0; i < BaseNetworkEntityManager.Peers.Count; i++)
			{
				NetworkEntity networkEntity = BaseNetworkEntityManager.Peers[i];
				if (networkEntity && !(this.m_owner == networkEntity))
				{
					float3 y = new float3(networkEntity.gameObject.transform.position);
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
		}

		// Token: 0x0600216D RID: 8557 RVA: 0x00123F18 File Offset: 0x00122118
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

		// Token: 0x040025D7 RID: 9687
		private float[] m_sqrDistances;
	}
}
