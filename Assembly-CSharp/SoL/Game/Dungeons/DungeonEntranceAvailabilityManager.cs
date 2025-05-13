using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SoL.Game.NPCs;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.REST;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.Game.Dungeons
{
	// Token: 0x02000C88 RID: 3208
	public class DungeonEntranceAvailabilityManager : MonoBehaviour
	{
		// Token: 0x06006199 RID: 24985 RVA: 0x00081CE7 File Offset: 0x0007FEE7
		private void Start()
		{
			if (!GameManager.IsServer)
			{
				base.enabled = false;
				return;
			}
		}

		// Token: 0x0600619A RID: 24986 RVA: 0x00081CF8 File Offset: 0x0007FEF8
		private void OnDestroy()
		{
			if (this.m_initialized)
			{
				base.CancelInvoke("CycleEntrances");
			}
		}

		// Token: 0x0600619B RID: 24987 RVA: 0x00200288 File Offset: 0x001FE488
		private bool EntranceIsEnabled(DungeonEntranceAvailabilityManager.EntranceData entranceData)
		{
			return entranceData.Entrance && entranceData.Entrance.ZonePoint && entranceData.Entrance.ZonePoint.enabled && entranceData.Entrance.ZonePoint.gameObject.activeSelf;
		}

		// Token: 0x0600619C RID: 24988 RVA: 0x002002E0 File Offset: 0x001FE4E0
		private void SceneCompositionManagerOnZoneLoaded(ZoneId obj)
		{
			DungeonEntranceAvailabilityManager.<SceneCompositionManagerOnZoneLoaded>d__16 <SceneCompositionManagerOnZoneLoaded>d__;
			<SceneCompositionManagerOnZoneLoaded>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<SceneCompositionManagerOnZoneLoaded>d__.<>4__this = this;
			<SceneCompositionManagerOnZoneLoaded>d__.<>1__state = -1;
			<SceneCompositionManagerOnZoneLoaded>d__.<>t__builder.Start<DungeonEntranceAvailabilityManager.<SceneCompositionManagerOnZoneLoaded>d__16>(ref <SceneCompositionManagerOnZoneLoaded>d__);
		}

		// Token: 0x0600619D RID: 24989 RVA: 0x00200318 File Offset: 0x001FE518
		private int GetActiveCount()
		{
			int num = 0;
			for (int i = 0; i < this.m_entrances.Count; i++)
			{
				if (this.m_entrances[i].Record.Status == DungeonEntranceStatus.Active)
				{
					num++;
				}
			}
			if (num != this.m_activeEntrances.Count)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"[DUNGEON to ",
					this.m_overworldZoneName,
					"] Count mismatch!  nActive:",
					num.ToString(),
					" vs ActiveQueueCount:",
					this.m_activeEntrances.Count.ToString()
				}));
			}
			return num;
		}

		// Token: 0x0600619E RID: 24990 RVA: 0x002003BC File Offset: 0x001FE5BC
		private void InitializeEntrances()
		{
			DungeonEntranceAvailabilityManager.<InitializeEntrances>d__18 <InitializeEntrances>d__;
			<InitializeEntrances>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<InitializeEntrances>d__.<>4__this = this;
			<InitializeEntrances>d__.<>1__state = -1;
			<InitializeEntrances>d__.<>t__builder.Start<DungeonEntranceAvailabilityManager.<InitializeEntrances>d__18>(ref <InitializeEntrances>d__);
		}

		// Token: 0x0600619F RID: 24991 RVA: 0x002003F4 File Offset: 0x001FE5F4
		private void CycleEntrances()
		{
			DungeonEntranceAvailabilityManager.<CycleEntrances>d__19 <CycleEntrances>d__;
			<CycleEntrances>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<CycleEntrances>d__.<>4__this = this;
			<CycleEntrances>d__.<>1__state = -1;
			<CycleEntrances>d__.<>t__builder.Start<DungeonEntranceAvailabilityManager.<CycleEntrances>d__19>(ref <CycleEntrances>d__);
		}

		// Token: 0x060061A0 RID: 24992 RVA: 0x0020042C File Offset: 0x001FE62C
		private Task ActivateEntrance(BaseDungeonEntrance entrance, bool enqueue, bool fullTime = true)
		{
			DungeonEntranceAvailabilityManager.<ActivateEntrance>d__20 <ActivateEntrance>d__;
			<ActivateEntrance>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ActivateEntrance>d__.<>4__this = this;
			<ActivateEntrance>d__.entrance = entrance;
			<ActivateEntrance>d__.enqueue = enqueue;
			<ActivateEntrance>d__.fullTime = fullTime;
			<ActivateEntrance>d__.<>1__state = -1;
			<ActivateEntrance>d__.<>t__builder.Start<DungeonEntranceAvailabilityManager.<ActivateEntrance>d__20>(ref <ActivateEntrance>d__);
			return <ActivateEntrance>d__.<>t__builder.Task;
		}

		// Token: 0x060061A1 RID: 24993 RVA: 0x00200488 File Offset: 0x001FE688
		private Task DeactivateForCount()
		{
			DungeonEntranceAvailabilityManager.<DeactivateForCount>d__21 <DeactivateForCount>d__;
			<DeactivateForCount>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DeactivateForCount>d__.<>4__this = this;
			<DeactivateForCount>d__.<>1__state = -1;
			<DeactivateForCount>d__.<>t__builder.Start<DungeonEntranceAvailabilityManager.<DeactivateForCount>d__21>(ref <DeactivateForCount>d__);
			return <DeactivateForCount>d__.<>t__builder.Task;
		}

		// Token: 0x060061A2 RID: 24994 RVA: 0x002004CC File Offset: 0x001FE6CC
		private string GetMinutesString(float seconds)
		{
			return (seconds / 60f).ToString("0.##", CultureInfo.InvariantCulture) + " minutes";
		}

		// Token: 0x060061A3 RID: 24995 RVA: 0x002004FC File Offset: 0x001FE6FC
		private void PrintActiveEntrances()
		{
			Debug.Log(string.Concat(new string[]
			{
				"[DUNGEON to ",
				this.m_overworldZoneName,
				"] Active Entrances: ",
				this.GetActiveCount().ToString(),
				", Iteration: ",
				(this.m_iteration + 1).ToString(),
				"/",
				this.m_entrances.Count.ToString()
			}));
		}

		// Token: 0x060061A4 RID: 24996 RVA: 0x0020057C File Offset: 0x001FE77C
		private void NotifyOverworldZones()
		{
			foreach (ZoneId targetZone in this.m_zonesToNotify)
			{
				ServerCommunicator.GET(targetZone, "refreshDungeons", null);
			}
		}

		// Token: 0x060061A5 RID: 24997 RVA: 0x002005D4 File Offset: 0x001FE7D4
		private void RefreshDungeonEntranceVisibility()
		{
			DungeonEntranceAvailabilityManager.<RefreshDungeonEntranceVisibility>d__25 <RefreshDungeonEntranceVisibility>d__;
			<RefreshDungeonEntranceVisibility>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<RefreshDungeonEntranceVisibility>d__.<>4__this = this;
			<RefreshDungeonEntranceVisibility>d__.<>1__state = -1;
			<RefreshDungeonEntranceVisibility>d__.<>t__builder.Start<DungeonEntranceAvailabilityManager.<RefreshDungeonEntranceVisibility>d__25>(ref <RefreshDungeonEntranceVisibility>d__);
		}

		// Token: 0x0400550A RID: 21770
		private static float kCycleTime = 600f;

		// Token: 0x0400550B RID: 21771
		[FormerlySerializedAs("m_sourceZoneId")]
		[SerializeField]
		private ZoneId m_overworldZoneId;

		// Token: 0x0400550C RID: 21772
		[SerializeField]
		private int m_nMaxActiveEntrances = 2;

		// Token: 0x0400550D RID: 21773
		[SerializeField]
		private List<DungeonEntranceAvailabilityManager.EntranceData> m_entranceData = new List<DungeonEntranceAvailabilityManager.EntranceData>();

		// Token: 0x0400550E RID: 21774
		private HashSet<ZoneId> m_zonesToNotify;

		// Token: 0x0400550F RID: 21775
		private List<BaseDungeonEntrance> m_entrances;

		// Token: 0x04005510 RID: 21776
		private Queue<BaseDungeonEntrance> m_activeEntrances;

		// Token: 0x04005511 RID: 21777
		private int m_iteration;

		// Token: 0x04005512 RID: 21778
		private bool m_initialized;

		// Token: 0x04005513 RID: 21779
		private string m_overworldZoneName;

		// Token: 0x02000C89 RID: 3209
		[Serializable]
		private class EntranceData
		{
			// Token: 0x04005514 RID: 21780
			public BaseDungeonEntrance Entrance;

			// Token: 0x04005515 RID: 21781
			public SpawnTier Tier;
		}

		// Token: 0x02000C8A RID: 3210
		private struct ZonePointPair : IEquatable<DungeonEntranceAvailabilityManager.ZonePointPair>
		{
			// Token: 0x060061A9 RID: 25001 RVA: 0x00081D33 File Offset: 0x0007FF33
			public ZonePointPair(int targetZoneId, int targetIndex)
			{
				this.TargetZoneId = targetZoneId;
				this.TargetIndex = targetIndex;
			}

			// Token: 0x060061AA RID: 25002 RVA: 0x00081D43 File Offset: 0x0007FF43
			public ZonePointPair(ZonePoint zonePoint)
			{
				this.TargetZoneId = (int)zonePoint.TargetZone;
				this.TargetIndex = zonePoint.TargetZonePointIndex;
			}

			// Token: 0x060061AB RID: 25003 RVA: 0x00081D5D File Offset: 0x0007FF5D
			public bool Equals(DungeonEntranceAvailabilityManager.ZonePointPair other)
			{
				return this.TargetZoneId == other.TargetZoneId && this.TargetIndex == other.TargetIndex;
			}

			// Token: 0x060061AC RID: 25004 RVA: 0x0020060C File Offset: 0x001FE80C
			public override bool Equals(object obj)
			{
				if (obj is DungeonEntranceAvailabilityManager.ZonePointPair)
				{
					DungeonEntranceAvailabilityManager.ZonePointPair other = (DungeonEntranceAvailabilityManager.ZonePointPair)obj;
					return this.Equals(other);
				}
				return false;
			}

			// Token: 0x060061AD RID: 25005 RVA: 0x00081D7D File Offset: 0x0007FF7D
			public override int GetHashCode()
			{
				return this.TargetZoneId * 397 ^ this.TargetIndex;
			}

			// Token: 0x04005516 RID: 21782
			public readonly int TargetZoneId;

			// Token: 0x04005517 RID: 21783
			public readonly int TargetIndex;
		}

		// Token: 0x02000C8B RID: 3211
		private struct ZonePointPairComparer : IEqualityComparer<DungeonEntranceAvailabilityManager.ZonePointPair>
		{
			// Token: 0x060061AE RID: 25006 RVA: 0x00081D92 File Offset: 0x0007FF92
			public bool Equals(DungeonEntranceAvailabilityManager.ZonePointPair x, DungeonEntranceAvailabilityManager.ZonePointPair y)
			{
				return x.TargetZoneId == y.TargetZoneId && x.TargetIndex == y.TargetIndex;
			}

			// Token: 0x060061AF RID: 25007 RVA: 0x00081DB2 File Offset: 0x0007FFB2
			public int GetHashCode(DungeonEntranceAvailabilityManager.ZonePointPair obj)
			{
				return obj.TargetZoneId * 397 ^ obj.TargetIndex;
			}
		}
	}
}
