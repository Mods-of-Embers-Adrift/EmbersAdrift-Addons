using System;
using SoL.Game.NPCs;
using SoL.Game.Spawning;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.Replication;
using UnityEngine;

namespace SoL.Game.Dungeons
{
	// Token: 0x02000C87 RID: 3207
	public abstract class BaseDungeonEntrance : SyncVarReplicator
	{
		// Token: 0x1700175E RID: 5982
		// (get) Token: 0x0600618C RID: 24972 RVA: 0x002000C8 File Offset: 0x001FE2C8
		public static GameObject DungeonEntranceParent
		{
			get
			{
				if (BaseDungeonEntrance.m_dungeonEntranceParent == null)
				{
					BaseDungeonEntrance.m_dungeonEntranceParent = new GameObject("DungeonEntrances");
					BaseDungeonEntrance.m_dungeonEntranceParent.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
					BaseDungeonEntrance.m_dungeonEntranceParent.transform.localScale = Vector3.one;
				}
				return BaseDungeonEntrance.m_dungeonEntranceParent;
			}
		}

		// Token: 0x1700175F RID: 5983
		// (get) Token: 0x0600618D RID: 24973 RVA: 0x00081C11 File Offset: 0x0007FE11
		// (set) Token: 0x0600618E RID: 24974 RVA: 0x00081C19 File Offset: 0x0007FE19
		public DungeonEntranceRecord Record { get; set; }

		// Token: 0x17001760 RID: 5984
		// (get) Token: 0x0600618F RID: 24975 RVA: 0x00081C22 File Offset: 0x0007FE22
		// (set) Token: 0x06006190 RID: 24976 RVA: 0x00081C2A File Offset: 0x0007FE2A
		public ISpawnLocation SpawnLocation { get; set; }

		// Token: 0x17001761 RID: 5985
		// (get) Token: 0x06006191 RID: 24977 RVA: 0x00081C33 File Offset: 0x0007FE33
		public ZonePoint ZonePoint
		{
			get
			{
				return this.m_zonePoint;
			}
		}

		// Token: 0x06006192 RID: 24978 RVA: 0x00081C3B File Offset: 0x0007FE3B
		private void Start()
		{
			if (GameManager.IsServer)
			{
				return;
			}
			if (base.GameEntity.NetworkEntity.IsInitialized)
			{
				this.Initialize();
				return;
			}
			base.GameEntity.NetworkEntity.OnStartClient += this.NetworkEntityOnOnStartClient;
		}

		// Token: 0x06006193 RID: 24979 RVA: 0x00081C7A File Offset: 0x0007FE7A
		private void NetworkEntityOnOnStartClient()
		{
			base.GameEntity.NetworkEntity.OnStartClient -= this.NetworkEntityOnOnStartClient;
			this.Initialize();
		}

		// Token: 0x06006194 RID: 24980 RVA: 0x00081C9E File Offset: 0x0007FE9E
		public void Initialize()
		{
			if (GameManager.IsServer)
			{
				this.RefreshSyncVarsFromRecord();
			}
			this.InitZonePoint();
		}

		// Token: 0x06006195 RID: 24981 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void InitZonePoint()
		{
		}

		// Token: 0x06006196 RID: 24982 RVA: 0x00200124 File Offset: 0x001FE324
		public virtual void RefreshSyncVarsFromRecord()
		{
			if (!GameManager.IsServer)
			{
				return;
			}
			if (this.Record == null)
			{
				this.Status.Value = DungeonEntranceStatus.None;
				this.Tier.Value = SpawnTier.Normal;
				this.ActivationTime.Value = DateTime.MinValue;
				this.DeactivationTime.Value = null;
				return;
			}
			this.Status.Value = this.Record.Status;
			this.Tier.Value = this.Record.Tier;
			this.ActivationTime.Value = this.Record.ActivationTime;
			this.DeactivationTime.Value = this.Record.DeactivationTime;
		}

		// Token: 0x06006197 RID: 24983 RVA: 0x002001D8 File Offset: 0x001FE3D8
		protected override int RegisterSyncs()
		{
			int num = base.RegisterSyncs();
			this.m_syncs.Add(this.ActivationTime);
			this.ActivationTime.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.DeactivationTime);
			this.DeactivationTime.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.Status);
			this.Status.BitFlag = 1 << num;
			num++;
			this.m_syncs.Add(this.Tier);
			this.Tier.BitFlag = 1 << num;
			return num + 1;
		}

		// Token: 0x04005502 RID: 21762
		private static GameObject m_dungeonEntranceParent;

		// Token: 0x04005503 RID: 21763
		public readonly SynchronizedEnum<DungeonEntranceStatus> Status = new SynchronizedEnum<DungeonEntranceStatus>();

		// Token: 0x04005504 RID: 21764
		public readonly SynchronizedDateTime ActivationTime = new SynchronizedDateTime();

		// Token: 0x04005505 RID: 21765
		public readonly SynchronizedNullableDateTime DeactivationTime = new SynchronizedNullableDateTime();

		// Token: 0x04005506 RID: 21766
		public readonly SynchronizedEnum<SpawnTier> Tier = new SynchronizedEnum<SpawnTier>();

		// Token: 0x04005509 RID: 21769
		[SerializeField]
		protected ZonePoint m_zonePoint;
	}
}
