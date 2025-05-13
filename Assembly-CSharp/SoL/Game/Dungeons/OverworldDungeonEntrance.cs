using System;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.Replication;

namespace SoL.Game.Dungeons
{
	// Token: 0x02000C94 RID: 3220
	public class OverworldDungeonEntrance : BaseDungeonEntrance
	{
		// Token: 0x060061C2 RID: 25026 RVA: 0x0020183C File Offset: 0x001FFA3C
		protected override void InitZonePoint()
		{
			base.InitZonePoint();
			if (this.m_zonePoint != null)
			{
				this.m_zonePoint.SetZoneData(this.m_dungeonZoneId.Value, this.m_zonePointIndex.Value);
				this.m_zonePoint.enabled = true;
			}
			if (!GameManager.IsServer)
			{
				base.gameObject.transform.parent = BaseDungeonEntrance.DungeonEntranceParent.transform;
				base.gameObject.transform.SetPositionAndRotation(this.m_location.Value.GetPosition(), this.m_location.Value.GetRotation());
			}
		}

		// Token: 0x060061C3 RID: 25027 RVA: 0x002018DC File Offset: 0x001FFADC
		public override void RefreshSyncVarsFromRecord()
		{
			base.RefreshSyncVarsFromRecord();
			if (!GameManager.IsServer)
			{
				return;
			}
			if (base.Record == null)
			{
				this.m_dungeonZoneId.Value = ZoneId.None;
				this.m_zonePointIndex.Value = -1;
				return;
			}
			this.m_dungeonZoneId.Value = (ZoneId)base.Record.DungeonZoneId;
			this.m_zonePointIndex.Value = base.Record.ZonePointIndex;
			this.m_location.Value = base.Record.Location;
		}

		// Token: 0x060061C4 RID: 25028 RVA: 0x0020195C File Offset: 0x001FFB5C
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.m_dungeonZoneId);
			this.m_dungeonZoneId.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_location);
			this.m_location.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.m_zonePointIndex);
			this.m_zonePointIndex.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x04005548 RID: 21832
		private readonly SynchronizedEnum<ZoneId> m_dungeonZoneId = new SynchronizedEnum<ZoneId>();

		// Token: 0x04005549 RID: 21833
		private readonly SynchronizedInt m_zonePointIndex = new SynchronizedInt();

		// Token: 0x0400554A RID: 21834
		private readonly SynchronizedLocation m_location = new SynchronizedLocation();
	}
}
