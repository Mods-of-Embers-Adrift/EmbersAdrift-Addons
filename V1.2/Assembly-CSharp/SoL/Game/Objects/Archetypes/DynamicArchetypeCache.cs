using System;
using System.Collections.Generic;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A35 RID: 2613
	public static class DynamicArchetypeCache
	{
		// Token: 0x060050E4 RID: 20708 RVA: 0x001CDF5C File Offset: 0x001CC15C
		public static BaseArchetype GetOrCreate(ArchetypeInstance instance, BaseArchetype archetype)
		{
			DateTime now = DateTime.Now;
			DynamicArchetypeCache.DynamicArchetypeCacheEntry dynamicArchetypeCacheEntry;
			BaseArchetype result;
			if (DynamicArchetypeCache.m_cache.TryGetValue(instance.InstanceId, out dynamicArchetypeCacheEntry))
			{
				dynamicArchetypeCacheEntry.Expiration = now.AddHours(12.0);
				result = dynamicArchetypeCacheEntry.Archetype;
			}
			else
			{
				BaseArchetype baseArchetype = archetype.BuildDynamic(instance);
				DynamicArchetypeCache.DynamicArchetypeCacheEntry fromPool = StaticPool<DynamicArchetypeCache.DynamicArchetypeCacheEntry>.GetFromPool();
				fromPool.Archetype = baseArchetype;
				fromPool.Expiration = now.AddHours(12.0);
				DynamicArchetypeCache.m_cache.Add(instance.InstanceId, fromPool);
				result = baseArchetype;
			}
			DynamicArchetypeCache.Clean(now);
			return result;
		}

		// Token: 0x060050E5 RID: 20709 RVA: 0x001CDFF0 File Offset: 0x001CC1F0
		private static void Clean(DateTime currentTime)
		{
			foreach (KeyValuePair<UniqueId, DynamicArchetypeCache.DynamicArchetypeCacheEntry> keyValuePair in DynamicArchetypeCache.m_cache)
			{
				if (keyValuePair.Value.Expiration <= currentTime)
				{
					DynamicArchetypeCache.m_cleanupManifest.Add(keyValuePair.Key);
				}
			}
			foreach (UniqueId key in DynamicArchetypeCache.m_cleanupManifest)
			{
				UnityEngine.Object.Destroy(DynamicArchetypeCache.m_cache[key].Archetype);
				StaticPool<DynamicArchetypeCache.DynamicArchetypeCacheEntry>.ReturnToPool(DynamicArchetypeCache.m_cache[key]);
				DynamicArchetypeCache.m_cache.Remove(key);
			}
			DynamicArchetypeCache.m_cleanupManifest.Clear();
		}

		// Token: 0x0400486A RID: 18538
		private const int kDefaultExpirationInHours = 12;

		// Token: 0x0400486B RID: 18539
		private static readonly Dictionary<UniqueId, DynamicArchetypeCache.DynamicArchetypeCacheEntry> m_cache = new Dictionary<UniqueId, DynamicArchetypeCache.DynamicArchetypeCacheEntry>(default(UniqueIdComparer));

		// Token: 0x0400486C RID: 18540
		private static readonly List<UniqueId> m_cleanupManifest = new List<UniqueId>();

		// Token: 0x02000A36 RID: 2614
		private class DynamicArchetypeCacheEntry : IPoolable
		{
			// Token: 0x1700120F RID: 4623
			// (get) Token: 0x060050E6 RID: 20710 RVA: 0x00076130 File Offset: 0x00074330
			// (set) Token: 0x060050E7 RID: 20711 RVA: 0x00076138 File Offset: 0x00074338
			public BaseArchetype Archetype { get; set; }

			// Token: 0x17001210 RID: 4624
			// (get) Token: 0x060050E8 RID: 20712 RVA: 0x00076141 File Offset: 0x00074341
			// (set) Token: 0x060050E9 RID: 20713 RVA: 0x00076149 File Offset: 0x00074349
			public DateTime Expiration { get; set; }

			// Token: 0x060050EA RID: 20714 RVA: 0x00076152 File Offset: 0x00074352
			public void Reset()
			{
				this.Archetype = null;
				this.Expiration = DateTime.MinValue;
			}

			// Token: 0x17001211 RID: 4625
			// (get) Token: 0x060050EB RID: 20715 RVA: 0x00076166 File Offset: 0x00074366
			// (set) Token: 0x060050EC RID: 20716 RVA: 0x0007616E File Offset: 0x0007436E
			bool IPoolable.InPool
			{
				get
				{
					return this.m_inPool;
				}
				set
				{
					this.m_inPool = value;
				}
			}

			// Token: 0x0400486D RID: 18541
			private bool m_inPool;
		}
	}
}
