using System;
using SoL.Managers;
using SoL.Networking.Database;

namespace SoL.Networking.Replication
{
	// Token: 0x0200047E RID: 1150
	public class LocationReplicator : SyncVarReplicator
	{
		// Token: 0x0600202B RID: 8235 RVA: 0x000577CF File Offset: 0x000559CF
		private void Awake()
		{
			if (base.GameEntity)
			{
				base.GameEntity.LocationReplicator = this;
			}
		}

		// Token: 0x0600202C RID: 8236 RVA: 0x000577EA File Offset: 0x000559EA
		private void Start()
		{
			if (!GameManager.IsServer)
			{
				this.m_location.Changed += this.LocationOnChanged;
				this.RefreshLocation();
			}
		}

		// Token: 0x0600202D RID: 8237 RVA: 0x00057810 File Offset: 0x00055A10
		protected override void OnDestroy()
		{
			if (!GameManager.IsServer)
			{
				this.m_location.Changed -= this.LocationOnChanged;
			}
			base.OnDestroy();
		}

		// Token: 0x0600202E RID: 8238 RVA: 0x00121B10 File Offset: 0x0011FD10
		public void ServerInit()
		{
			if (GameManager.IsServer)
			{
				this.m_location.Value = new CharacterLocation
				{
					ZoneId = ((LocalZoneManager.ZoneRecord != null) ? LocalZoneManager.ZoneRecord.ZoneId : 0),
					x = base.gameObject.transform.position.x,
					y = base.gameObject.transform.position.y,
					z = base.gameObject.transform.position.z,
					h = base.gameObject.transform.rotation.eulerAngles.y
				};
			}
		}

		// Token: 0x0600202F RID: 8239 RVA: 0x00057836 File Offset: 0x00055A36
		private void LocationOnChanged(CharacterLocation obj)
		{
			this.RefreshLocation();
		}

		// Token: 0x06002030 RID: 8240 RVA: 0x00121BC8 File Offset: 0x0011FDC8
		private void RefreshLocation()
		{
			if (this.m_location.Value != null)
			{
				base.gameObject.transform.position = this.m_location.Value.GetPosition();
				base.gameObject.transform.rotation = this.m_location.Value.GetRotation();
			}
		}

		// Token: 0x06002031 RID: 8241 RVA: 0x00121C24 File Offset: 0x0011FE24
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.m_location);
			this.m_location.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x04002580 RID: 9600
		private readonly SynchronizedLocation m_location = new SynchronizedLocation();
	}
}
