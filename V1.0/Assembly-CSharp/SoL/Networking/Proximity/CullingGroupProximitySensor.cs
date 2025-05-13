using System;
using SoL.Networking.Managers;
using SoL.Networking.Objects;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Networking.Proximity
{
	// Token: 0x020004AB RID: 1195
	public class CullingGroupProximitySensor : BaseProximitySensor
	{
		// Token: 0x06002154 RID: 8532 RVA: 0x00123AD0 File Offset: 0x00121CD0
		protected override void Awake()
		{
			base.Awake();
			if (this.m_distanceBands == null || this.m_distanceBands.Length == 0)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			float[] array = new float[this.m_distanceBands.Length];
			for (int i = 0; i < this.m_distanceBands.Length; i++)
			{
				array[i] = this.m_distanceBands[i].Distance;
			}
			this.m_currentCount = this.m_server.CurrentSphereCount;
			this.m_cullingGroup = new CullingGroup();
			this.m_cullingGroup.SetBoundingDistances(array);
			this.m_cullingGroup.SetBoundingSpheres(this.m_server.PlayerSpheres);
			this.m_cullingGroup.SetBoundingSphereCount(this.m_currentCount);
			this.m_cullingGroup.SetDistanceReferencePoint(base.gameObject.transform);
			this.m_server.SphereCountUpdated += this.MServerOnSphereCountUpdated;
		}

		// Token: 0x06002155 RID: 8533 RVA: 0x00058188 File Offset: 0x00056388
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.m_server.SphereCountUpdated -= this.MServerOnSphereCountUpdated;
			this.m_cullingGroup.Dispose();
		}

		// Token: 0x06002156 RID: 8534 RVA: 0x00123BB0 File Offset: 0x00121DB0
		protected override void RebuildObserversInternal()
		{
			for (int i = this.m_distanceBands.Length - 1; i >= 0; i--)
			{
				DistanceBand distanceBand = this.m_distanceBands[i];
				int num = this.m_cullingGroup.QueryIndices(distanceBand.Index, this.m_query, 0);
				if (num == 0)
				{
					return;
				}
				for (int j = 0; j < num; j++)
				{
					int num2 = this.m_query[j];
					if (this.m_owner.BoundingSphereIndex == null || this.m_owner.BoundingSphereIndex.Value != num2)
					{
						Observer observer = null;
						if (this.Observers.TryGetValue(num2, out observer))
						{
							observer.Remove = false;
							if (!observer.DistanceBand.Equals(distanceBand))
							{
								distanceBand.AddObserver(observer);
							}
						}
						else
						{
							NetworkEntity entityForSphereIndex = NetworkManager.EntityManager.GetEntityForSphereIndex(num2);
							if (entityForSphereIndex != null)
							{
								observer = StaticPool<Observer>.GetFromPool();
								observer.NetworkEntity = entityForSphereIndex;
								observer.Id = num2;
								observer.Added = true;
								observer.Remove = false;
								distanceBand.AddObserver(observer);
								this.Observers.Add(observer.Id, observer);
							}
						}
					}
				}
			}
		}

		// Token: 0x06002157 RID: 8535 RVA: 0x000581B2 File Offset: 0x000563B2
		private void MServerOnSphereCountUpdated(int value)
		{
			this.m_currentCount = value;
			this.m_cullingGroup.SetBoundingSphereCount(this.m_currentCount);
		}

		// Token: 0x040025CD RID: 9677
		private CullingGroup m_cullingGroup;

		// Token: 0x040025CE RID: 9678
		private int m_currentCount;

		// Token: 0x040025CF RID: 9679
		private readonly int[] m_query = new int[300];
	}
}
