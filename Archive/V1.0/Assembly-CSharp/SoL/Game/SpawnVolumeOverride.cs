using System;
using System.Collections.Generic;
using SoL.Managers;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000603 RID: 1539
	public class SpawnVolumeOverride : BaseVolumeOverride
	{
		// Token: 0x17000A76 RID: 2678
		// (get) Token: 0x0600311C RID: 12572 RVA: 0x00061D42 File Offset: 0x0005FF42
		public string ZoneNameSuffix
		{
			get
			{
				return this.m_zoneNameSuffix;
			}
		}

		// Token: 0x17000A77 RID: 2679
		// (get) Token: 0x0600311D RID: 12573 RVA: 0x00061D4A File Offset: 0x0005FF4A
		public SubZoneId SubZoneId
		{
			get
			{
				return this.m_subZoneId;
			}
		}

		// Token: 0x0600311E RID: 12574 RVA: 0x00061D52 File Offset: 0x0005FF52
		protected override void Register()
		{
			base.Register();
			LocalZoneManager.RegisterSpawnVolumeOverride(this);
		}

		// Token: 0x0600311F RID: 12575 RVA: 0x00061D60 File Offset: 0x0005FF60
		protected override void Deregister()
		{
			base.Deregister();
			LocalZoneManager.DeregisterSpawnVolumeOverride(this);
		}

		// Token: 0x06003120 RID: 12576 RVA: 0x00061D6E File Offset: 0x0005FF6E
		public bool IsWithinBounds(Vector3 pos, out PlayerSpawn playerSpawn)
		{
			playerSpawn = null;
			if (base.IsWithinBounds(pos))
			{
				playerSpawn = this.m_spawn;
				return true;
			}
			return false;
		}

		// Token: 0x06003121 RID: 12577 RVA: 0x0015C184 File Offset: 0x0015A384
		public bool IsActive(GameEntity entity, out PlayerSpawn playerSpawn)
		{
			playerSpawn = null;
			if (entity && entity.CollectionController != null && entity.CollectionController.Record != null)
			{
				Vector3 position = entity.gameObject.transform.position;
				return this.IsActive(position, entity.CollectionController.Record, out playerSpawn);
			}
			return false;
		}

		// Token: 0x06003122 RID: 12578 RVA: 0x0015C1D8 File Offset: 0x0015A3D8
		public bool IsActive(Vector3 pos, CharacterRecord record, out PlayerSpawn playerSpawn)
		{
			playerSpawn = null;
			if (base.IsWithinBounds(pos))
			{
				if (!this.m_requireDiscovery)
				{
					playerSpawn = this.m_spawn;
					return true;
				}
				ZoneId key;
				List<UniqueId> list;
				if (this.m_spawn && !this.m_spawn.DiscoveryId.IsEmpty && LocalZoneManager.ZoneRecord != null && ZoneIdExtensions.ZoneIdDict.TryGetValue(LocalZoneManager.ZoneRecord.ZoneId, out key) && record != null && record.Discoveries != null && record.Discoveries.TryGetValue(key, out list) && list != null && list.Contains(this.m_spawn.DiscoveryId))
				{
					playerSpawn = this.m_spawn;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003123 RID: 12579 RVA: 0x00061D87 File Offset: 0x0005FF87
		private void OnDrawGizmosSelected()
		{
			if (this.m_spawn != null)
			{
				Gizmos.DrawLine(base.gameObject.transform.position, this.m_spawn.GetTargetPosition());
			}
		}

		// Token: 0x04002F58 RID: 12120
		[SerializeField]
		private PlayerSpawn m_spawn;

		// Token: 0x04002F59 RID: 12121
		[SerializeField]
		private string m_zoneNameSuffix;

		// Token: 0x04002F5A RID: 12122
		[SerializeField]
		private SubZoneId m_subZoneId;

		// Token: 0x04002F5B RID: 12123
		[SerializeField]
		private bool m_requireDiscovery;
	}
}
