using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x020006C6 RID: 1734
	public class SpawnControllerDatabase : SpawnController
	{
		// Token: 0x060034CC RID: 13516 RVA: 0x00064262 File Offset: 0x00062462
		protected override void Awake()
		{
			base.Awake();
			base.enabled = false;
			base.gameObject.AddComponent<OldestInstanceToggler>().ExternalInit(this);
		}

		// Token: 0x060034CD RID: 13517 RVA: 0x00064282 File Offset: 0x00062482
		protected override WaitForSeconds GetStartupWait()
		{
			if (this.m_respawnTimesRecord == null)
			{
				return base.GetStartupWait();
			}
			return SpawnController.m_instantWait;
		}

		// Token: 0x060034CE RID: 13518 RVA: 0x00165E1C File Offset: 0x0016401C
		protected override void SpawnMonitorCoStartup()
		{
			base.SpawnMonitorCoStartup();
			if (!this.m_profile || this.m_respawnTimesRecord != null)
			{
				return;
			}
			try
			{
				this.m_respawnTimesRecord = RespawnTimesRecord.Load(ExternalGameDatabase.Database, this.m_profile.Id.Value);
				if (this.m_respawnTimesRecord == null)
				{
					DateTime utcNow = DateTime.UtcNow;
					this.m_respawnTimesRecord = new RespawnTimesRecord
					{
						Id = this.m_profile.Id,
						Updated = utcNow,
						Description = this.m_profile.Description,
						RespawnTimes = new List<DateTime>(this.m_targetPopulation)
					};
					float num = this.m_customInitialSpawnTime ? this.m_initialSpawnTime.RandomWithinRange() : this.m_monitorFrequency;
					DateTime item = utcNow.AddSeconds((double)num);
					for (int i = 0; i < this.m_targetPopulation; i++)
					{
						this.m_respawnTimesRecord.RespawnTimes.Add(item);
					}
					this.m_respawnTimesRecord.Save(ExternalGameDatabase.Database);
				}
				if (this.m_respawnTimesRecord.RespawnTimes != null && this.m_respawnTimesRecord.RespawnTimes.Count > 0)
				{
					this.m_respawnTimes.AddRange(this.m_respawnTimesRecord.RespawnTimes);
				}
			}
			catch (Exception message)
			{
				Debug.LogWarning(message);
			}
		}

		// Token: 0x060034CF RID: 13519 RVA: 0x00064298 File Offset: 0x00062498
		protected override void NotifyOfDeath(GameEntity entity)
		{
			base.NotifyOfDeath(entity);
			if (entity)
			{
				entity.RemoveFromActiveSpawns = true;
			}
			base.MonitorActiveSpawns();
			this.AfterSpawnMonitorCycle();
		}

		// Token: 0x060034D0 RID: 13520 RVA: 0x00165F7C File Offset: 0x0016417C
		protected override void AfterSpawnMonitorCycle()
		{
			base.AfterSpawnMonitorCycle();
			if (!this.m_profile || this.m_respawnTimesRecord == null)
			{
				return;
			}
			if (this.m_respawnTimesRecord.RespawnTimes == null)
			{
				this.m_respawnTimesRecord.RespawnTimes = new List<DateTime>(10);
			}
			else
			{
				this.m_respawnTimesRecord.RespawnTimes.Clear();
			}
			this.m_respawnTimesRecord.RespawnTimes.AddRange(this.m_respawnTimes);
			this.m_respawnTimesRecord.Description = this.m_profile.Description;
			this.UpdateRecord();
		}

		// Token: 0x060034D1 RID: 13521 RVA: 0x00166008 File Offset: 0x00164208
		private void UpdateRecord()
		{
			SpawnControllerDatabase.<UpdateRecord>d__7 <UpdateRecord>d__;
			<UpdateRecord>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<UpdateRecord>d__.<>4__this = this;
			<UpdateRecord>d__.<>1__state = -1;
			<UpdateRecord>d__.<>t__builder.Start<SpawnControllerDatabase.<UpdateRecord>d__7>(ref <UpdateRecord>d__);
		}

		// Token: 0x04003304 RID: 13060
		[SerializeField]
		private SpawnControllerDbProfile m_profile;

		// Token: 0x04003305 RID: 13061
		private RespawnTimesRecord m_respawnTimesRecord;
	}
}
