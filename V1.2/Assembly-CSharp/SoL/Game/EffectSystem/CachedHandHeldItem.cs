using System;
using SoL.Game.Objects.Archetypes;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C0A RID: 3082
	public class CachedHandHeldItem
	{
		// Token: 0x17001680 RID: 5760
		// (get) Token: 0x06005EDF RID: 24287 RVA: 0x0007FD55 File Offset: 0x0007DF55
		public ArchetypeInstance Instance
		{
			get
			{
				this.Cache();
				return this.m_instance;
			}
		}

		// Token: 0x17001681 RID: 5761
		// (get) Token: 0x06005EE0 RID: 24288 RVA: 0x0007FD63 File Offset: 0x0007DF63
		public IHandheldItem HandHeldItem
		{
			get
			{
				this.Cache();
				return this.m_handHeldItem;
			}
		}

		// Token: 0x17001682 RID: 5762
		// (get) Token: 0x06005EE1 RID: 24289 RVA: 0x0007FD71 File Offset: 0x0007DF71
		public WeaponItem WeaponItem
		{
			get
			{
				this.Cache();
				return this.m_weaponItem;
			}
		}

		// Token: 0x17001683 RID: 5763
		// (get) Token: 0x06005EE2 RID: 24290 RVA: 0x0007FD7F File Offset: 0x0007DF7F
		public RunicBattery RunicBattery
		{
			get
			{
				this.Cache();
				return this.m_runicBattery;
			}
		}

		// Token: 0x17001684 RID: 5764
		// (get) Token: 0x06005EE3 RID: 24291 RVA: 0x0007FD8D File Offset: 0x0007DF8D
		public RangedAmmoItem RangedAmmo
		{
			get
			{
				this.Cache();
				return this.m_rangedAmmo;
			}
		}

		// Token: 0x06005EE4 RID: 24292 RVA: 0x0007FD9B File Offset: 0x0007DF9B
		public CachedHandHeldItem(bool isPrimary)
		{
			this.m_primary = isPrimary;
		}

		// Token: 0x06005EE5 RID: 24293 RVA: 0x0007FDAA File Offset: 0x0007DFAA
		public void Init(GameEntity sourceEntity)
		{
			this.Reset();
			this.m_sourceEntity = sourceEntity;
		}

		// Token: 0x06005EE6 RID: 24294 RVA: 0x0007FDB9 File Offset: 0x0007DFB9
		public void AccessReset(GameEntity sourceEntity)
		{
			if (this.m_sourceEntity == null || this.m_sourceEntity.Type != GameEntityType.Npc)
			{
				this.Reset();
			}
			this.m_sourceEntity = sourceEntity;
		}

		// Token: 0x06005EE7 RID: 24295 RVA: 0x0007FDE4 File Offset: 0x0007DFE4
		public void Reset()
		{
			this.m_cached = false;
			this.m_sourceEntity = null;
			this.m_instance = null;
			this.m_handHeldItem = null;
			this.m_weaponItem = null;
			this.m_runicBattery = null;
			this.m_rangedAmmo = null;
		}

		// Token: 0x06005EE8 RID: 24296 RVA: 0x001F8034 File Offset: 0x001F6234
		public void CopyFromExecutionCache(ExecutionCache executionCache)
		{
			this.Reset();
			CachedHandHeldItem cachedHandHeldItem = this.m_primary ? executionCache.MainHand : executionCache.OffHand;
			this.m_cached = cachedHandHeldItem.m_cached;
			this.m_sourceEntity = cachedHandHeldItem.m_sourceEntity;
			this.m_instance = cachedHandHeldItem.m_instance;
			this.m_handHeldItem = cachedHandHeldItem.m_handHeldItem;
			this.m_weaponItem = cachedHandHeldItem.m_weaponItem;
			this.m_runicBattery = cachedHandHeldItem.m_runicBattery;
			this.m_rangedAmmo = cachedHandHeldItem.m_rangedAmmo;
		}

		// Token: 0x06005EE9 RID: 24297 RVA: 0x001F80B4 File Offset: 0x001F62B4
		private void Cache()
		{
			if (this.m_cached)
			{
				return;
			}
			if (this.m_primary)
			{
				this.m_sourceEntity.TryGetHandheldItem_MainHandAsType(out this.m_instance, out this.m_handHeldItem);
			}
			else
			{
				this.m_sourceEntity.TryGetHandheldItem_OffHandAsType(out this.m_instance, out this.m_handHeldItem);
			}
			if (this.m_handHeldItem != null)
			{
				this.m_weaponItem = (this.m_handHeldItem as WeaponItem);
				this.m_runicBattery = (this.m_handHeldItem as RunicBattery);
				this.m_rangedAmmo = (this.m_handHeldItem as RangedAmmoItem);
			}
			this.m_cached = true;
		}

		// Token: 0x04005203 RID: 20995
		private readonly bool m_primary;

		// Token: 0x04005204 RID: 20996
		private bool m_cached;

		// Token: 0x04005205 RID: 20997
		private GameEntity m_sourceEntity;

		// Token: 0x04005206 RID: 20998
		private ArchetypeInstance m_instance;

		// Token: 0x04005207 RID: 20999
		private IHandheldItem m_handHeldItem;

		// Token: 0x04005208 RID: 21000
		private WeaponItem m_weaponItem;

		// Token: 0x04005209 RID: 21001
		private RunicBattery m_runicBattery;

		// Token: 0x0400520A RID: 21002
		private RangedAmmoItem m_rangedAmmo;
	}
}
