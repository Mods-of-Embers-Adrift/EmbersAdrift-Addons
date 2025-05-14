using System;
using System.Collections;
using SoL.Managers;
using SoL.Networking.Managers;
using UnityEngine;

namespace SoL.Utilities.Logging
{
	// Token: 0x02000317 RID: 791
	public class ServerMemoryMonitor : MonoBehaviour
	{
		// Token: 0x17000546 RID: 1350
		// (get) Token: 0x06001606 RID: 5638 RVA: 0x00051717 File Offset: 0x0004F917
		private bool HostIsValid
		{
			get
			{
				return LocalZoneManager.ZoneRecord != null && NetworkManager.MyHost != null && NetworkManager.MyHost.IsSet;
			}
		}

		// Token: 0x06001607 RID: 5639 RVA: 0x00051733 File Offset: 0x0004F933
		private void Start()
		{
			this.m_updateCo = this.UpdateCo();
			base.StartCoroutine(this.UpdateCo());
		}

		// Token: 0x06001608 RID: 5640 RVA: 0x0005174E File Offset: 0x0004F94E
		private void OnDestroy()
		{
			if (this.m_updateCo != null)
			{
				base.StopCoroutine(this.m_updateCo);
			}
		}

		// Token: 0x06001609 RID: 5641 RVA: 0x00051764 File Offset: 0x0004F964
		private IEnumerator UpdateCo()
		{
			this.m_waitForRetry = new WaitForSeconds(30f);
			this.m_waitAfterLog = new WaitForSeconds(this.LogCadence);
			for (;;)
			{
				if (this.CanUpdate())
				{
					this.LogToIndex();
					yield return this.m_waitAfterLog;
				}
				else
				{
					yield return this.m_waitForRetry;
				}
			}
			yield break;
		}

		// Token: 0x17000547 RID: 1351
		// (get) Token: 0x0600160A RID: 5642 RVA: 0x00051627 File Offset: 0x0004F827
		private float LogCadence
		{
			get
			{
				return 300f;
			}
		}

		// Token: 0x0600160B RID: 5643 RVA: 0x000FE470 File Offset: 0x000FC670
		private bool CanUpdate()
		{
			int previousConnectedCount = this.m_previousConnectedCount;
			int playerConnectedCount = BaseNetworkEntityManager.PlayerConnectedCount;
			this.m_previousConnectedCount = previousConnectedCount;
			if (this.m_sentZeroCount || previousConnectedCount <= 0 || playerConnectedCount > 0)
			{
				if (playerConnectedCount > 0)
				{
					this.m_sentZeroCount = false;
				}
				return playerConnectedCount > 0 && this.HostIsValid;
			}
			if (this.HostIsValid)
			{
				this.m_sentZeroCount = true;
				return true;
			}
			return false;
		}

		// Token: 0x0600160C RID: 5644 RVA: 0x000FE4CC File Offset: 0x000FC6CC
		private void LogToIndex()
		{
			this.m_objectArray[0] = LocalZoneManager.ZoneRecord.DisplayName;
			float num = (float)GC.GetTotalMemory(false) / 1000000f;
			this.m_objectArray[1] = num;
			this.m_objectArray[2] = GlobalCounters.SpawnedPlayers;
			this.m_objectArray[3] = GlobalCounters.SpawnedNpcs;
			this.m_objectArray[4] = GlobalCounters.SpawnedNodes;
			this.m_objectArray[5] = GlobalCounters.LootGenerated;
			this.m_objectArray[6] = GlobalCounters.ItemsCrafted;
			this.m_objectArray[7] = GlobalCounters.ItemsSold;
			this.m_objectArray[8] = GlobalCounters.ItemsPurchased;
			this.m_objectArray[9] = GlobalCounters.EffectApplicators;
			this.m_objectArray[10] = GlobalCounters.CharQuestWrites;
			this.m_objectArray[11] = GlobalCounters.CharLearnableWrites;
			this.m_objectArray[12] = GlobalCounters.CharDiscoveryWrites;
			this.m_objectArray[13] = GlobalCounters.CharStorageWrites;
			this.m_objectArray[14] = GlobalCounters.CharTitleWrites;
			this.m_objectArray[15] = GlobalCounters.CharRecordWrites;
			this.m_objectArray[16] = GlobalCounters.ContainerLoads;
			this.m_objectArray[17] = GlobalCounters.ContainerSaves;
			SolDebug.LogToIndex(LogLevel.Information, LogIndex.ServerMemory, "{@Zone} || {@MemoryUsed} || {@SpawnedPlayers} {@SpawnedNpcs} {@SpawnedNodes} {@LootGenerated} {@ItemsCrafted} {@ItemsSold} {@ItemsPurchased} {@EffectApplicator} {@CharQuestWrites} {@CharLearnableWrites} {@CharDiscoveryWrites} {@CharStorageWrites} {@CharTitleWrites} {@CharRecordWrites} {@ContainerLoads} {@ContainerSaves}", this.m_objectArray);
		}

		// Token: 0x04001DFF RID: 7679
		private const string kTemplate = "{@Zone} || {@MemoryUsed} || {@SpawnedPlayers} {@SpawnedNpcs} {@SpawnedNodes} {@LootGenerated} {@ItemsCrafted} {@ItemsSold} {@ItemsPurchased} {@EffectApplicator} {@CharQuestWrites} {@CharLearnableWrites} {@CharDiscoveryWrites} {@CharStorageWrites} {@CharTitleWrites} {@CharRecordWrites} {@ContainerLoads} {@ContainerSaves}";

		// Token: 0x04001E00 RID: 7680
		private readonly object[] m_objectArray = new object[18];

		// Token: 0x04001E01 RID: 7681
		private int m_previousConnectedCount;

		// Token: 0x04001E02 RID: 7682
		private bool m_sentZeroCount;

		// Token: 0x04001E03 RID: 7683
		private IEnumerator m_updateCo;

		// Token: 0x04001E04 RID: 7684
		private WaitForSeconds m_waitAfterLog;

		// Token: 0x04001E05 RID: 7685
		private WaitForSeconds m_waitForRetry;
	}
}
