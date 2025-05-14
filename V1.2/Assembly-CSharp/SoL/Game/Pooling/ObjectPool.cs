using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Game.Pooling
{
	// Token: 0x020007D5 RID: 2005
	public class ObjectPool : MonoBehaviour
	{
		// Token: 0x06003A76 RID: 14966 RVA: 0x00067984 File Offset: 0x00065B84
		public static ObjectPool GetPool(PooledObject prefab, bool delayedDestruction)
		{
			GameObject gameObject = new GameObject("POOL_" + prefab.name);
			gameObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
			ObjectPool objectPool = gameObject.AddComponent<ObjectPool>();
			objectPool.InitInternal(prefab, delayedDestruction);
			return objectPool;
		}

		// Token: 0x06003A77 RID: 14967 RVA: 0x000679BD File Offset: 0x00065BBD
		private void InitInternal(PooledObject prefab, bool hasDelayedDestruction)
		{
			this.m_prefab = prefab;
			this.m_delayedDestruction = hasDelayedDestruction;
			if (this.m_delayedDestruction)
			{
				base.InvokeRepeating("DelayedDestructionCheck", 60f, 60f);
			}
		}

		// Token: 0x06003A78 RID: 14968 RVA: 0x000679EA File Offset: 0x00065BEA
		private void OnDestroy()
		{
			if (this.m_delayedDestruction)
			{
				base.CancelInvoke("DelayedDestructionCheck");
			}
		}

		// Token: 0x06003A79 RID: 14969 RVA: 0x00177F24 File Offset: 0x00176124
		public PooledObject GetFromPool()
		{
			PooledObject pooledObject = null;
			while (this.m_pool.Count > 0 && !pooledObject)
			{
				int index = this.m_pool.Count - 1;
				pooledObject = this.m_pool[index];
				this.m_pool.RemoveAt(index);
			}
			if (!pooledObject)
			{
				pooledObject = UnityEngine.Object.Instantiate<PooledObject>(this.m_prefab);
				pooledObject.Pool = this;
			}
			pooledObject.IsInPool = false;
			pooledObject.gameObject.transform.SetParent(null);
			pooledObject.gameObject.SetActive(true);
			return pooledObject;
		}

		// Token: 0x06003A7A RID: 14970 RVA: 0x00177FB4 File Offset: 0x001761B4
		public void ReturnToPool(PooledObject obj)
		{
			if (!obj || obj.IsInPool)
			{
				return;
			}
			if (this.m_pool.Count < 128)
			{
				try
				{
					obj.ResetPooledObject();
					obj.IsInPool = true;
					this.m_pool.Add(obj);
					return;
				}
				catch (Exception arg)
				{
					Debug.LogWarning(string.Format("Exception when attempting to return to pool!  {0}", arg));
					return;
				}
			}
			UnityEngine.Object.Destroy(obj.gameObject);
		}

		// Token: 0x06003A7B RID: 14971 RVA: 0x00178030 File Offset: 0x00176230
		private void DelayedDestructionCheck()
		{
			for (int i = 0; i < this.m_pool.Count; i++)
			{
				PooledObject pooledObject = this.m_pool[i];
				if (pooledObject && pooledObject.DelayedDestruction && Time.time - pooledObject.TimeReturnedToPool > pooledObject.DelayedDestructionTime)
				{
					this.m_pool.RemoveAt(i);
					UnityEngine.Object.Destroy(pooledObject.gameObject);
					i--;
				}
			}
		}

		// Token: 0x040038F6 RID: 14582
		private const int kMaxPoolSize = 128;

		// Token: 0x040038F7 RID: 14583
		private const string kPoolPrefix = "POOL";

		// Token: 0x040038F8 RID: 14584
		private const float kDelayedDestructionCadence = 60f;

		// Token: 0x040038F9 RID: 14585
		private PooledObject m_prefab;

		// Token: 0x040038FA RID: 14586
		private readonly List<PooledObject> m_pool = new List<PooledObject>(128);

		// Token: 0x040038FB RID: 14587
		private bool m_delayedDestruction;
	}
}
