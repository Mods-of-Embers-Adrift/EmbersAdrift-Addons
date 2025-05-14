using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.AI;

namespace SoL.Managers
{
	// Token: 0x02000546 RID: 1350
	public class SpawnPointDistanceManager : MonoBehaviour
	{
		// Token: 0x06002916 RID: 10518 RVA: 0x0013FCB8 File Offset: 0x0013DEB8
		private void Awake()
		{
			if (this.m_closestForIndex == null)
			{
				this.m_closestForIndex = new Dictionary<int, List<SpawnPointDistanceManager.StoredPlayerSpawn>>(50);
				this.m_updateQueue = new Queue<IndexedVector3>();
				this.m_fullPathCorners = new List<Vector3>(10);
				this.m_path = new NavMeshPath();
				this.m_getPlayerSpawns = LocalZoneManager.GetPlayerSpawns();
				this.m_updateCo = this.UpdateCo();
				base.StartCoroutine(this.m_updateCo);
			}
		}

		// Token: 0x06002917 RID: 10519 RVA: 0x0005C6C9 File Offset: 0x0005A8C9
		private void OnDestroy()
		{
			if (this.m_updateCo != null)
			{
				base.StopCoroutine(this.m_updateCo);
				this.m_updateCo = null;
			}
		}

		// Token: 0x06002918 RID: 10520 RVA: 0x0005C6E6 File Offset: 0x0005A8E6
		private float GetCellSize()
		{
			if (!(ZoneSettings.Instance != null) || !(ZoneSettings.Instance.Profile != null))
			{
				return 10f;
			}
			return ZoneSettings.Instance.Profile.GetNavigationCellSize();
		}

		// Token: 0x06002919 RID: 10521 RVA: 0x0013FD24 File Offset: 0x0013DF24
		public void CacheClosest(Vector3 pos)
		{
			IndexedVector3 indexedVector = new IndexedVector3(pos, 512, this.GetCellSize());
			if (!this.m_closestForIndex.ContainsKey(indexedVector.Index))
			{
				this.m_updateQueue.Enqueue(indexedVector);
			}
		}

		// Token: 0x0600291A RID: 10522 RVA: 0x0013FD64 File Offset: 0x0013DF64
		public bool TryGetClosest(Vector3 pos, out PlayerSpawn nearestSpawn)
		{
			nearestSpawn = null;
			if (this.m_closestForIndex == null)
			{
				Debug.LogWarning("Asking for closet player spawn before any positions have been cached!");
				return false;
			}
			IndexedVector3 indexedVector = new IndexedVector3(pos, 512, this.GetCellSize());
			List<SpawnPointDistanceManager.StoredPlayerSpawn> list;
			if (this.m_closestForIndex.TryGetValue(indexedVector.Index, out list) && list.Count > 0)
			{
				nearestSpawn = list[0].PlayerSpawn;
			}
			return nearestSpawn != null;
		}

		// Token: 0x0600291B RID: 10523 RVA: 0x0013FDD0 File Offset: 0x0013DFD0
		public bool TryGetClosestKnown(Vector3 pos, List<UniqueId> knownIds, out PlayerSpawn nearestKnown)
		{
			nearestKnown = null;
			if (this.m_closestForIndex == null)
			{
				Debug.LogWarning("Asking for closet player spawn before any positions have been cached!");
				return false;
			}
			IndexedVector3 indexedVector = new IndexedVector3(pos, 512, this.GetCellSize());
			List<SpawnPointDistanceManager.StoredPlayerSpawn> list;
			if (this.m_closestForIndex.TryGetValue(indexedVector.Index, out list))
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].PlayerSpawn && !list[i].PlayerSpawn.DiscoveryId.IsEmpty && knownIds.Contains(list[i].PlayerSpawn.DiscoveryId))
					{
						nearestKnown = list[i].PlayerSpawn;
						break;
					}
				}
			}
			return nearestKnown != null;
		}

		// Token: 0x0600291C RID: 10524 RVA: 0x0013FE8C File Offset: 0x0013E08C
		private PlayerSpawn GetClosestSpawnByRawDistance(Vector3 pos)
		{
			PlayerSpawn playerSpawn = LocalZoneManager.DefaultPlayerSpawn;
			float num = (pos - playerSpawn.gameObject.transform.position).sqrMagnitude;
			foreach (PlayerSpawn playerSpawn2 in this.m_getPlayerSpawns)
			{
				if (playerSpawn2 != LocalZoneManager.DefaultPlayerSpawn)
				{
					float sqrMagnitude = (pos - playerSpawn2.gameObject.transform.position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						playerSpawn = playerSpawn2;
						num = sqrMagnitude;
					}
				}
			}
			return playerSpawn;
		}

		// Token: 0x0600291D RID: 10525 RVA: 0x0005C71C File Offset: 0x0005A91C
		private IEnumerator UpdateCo()
		{
			for (;;)
			{
				if (this.m_updateQueue.Count <= 0)
				{
					yield return null;
				}
				else
				{
					IndexedVector3 query = this.m_updateQueue.Dequeue();
					if (!this.m_closestForIndex.ContainsKey(query.Index))
					{
						Vector3 pos = query.Pos;
						int count = 0;
						NavMeshHit sourceNavHit;
						if (NavMesh.SamplePosition(pos, out sourceNavHit, 50f, -1))
						{
							List<SpawnPointDistanceManager.StoredPlayerSpawn> spawnList = new List<SpawnPointDistanceManager.StoredPlayerSpawn>(LocalZoneManager.PlayerSpawnCount);
							foreach (PlayerSpawn playerSpawn in this.m_getPlayerSpawns)
							{
								Vector3 targetPosition = playerSpawn.GetTargetPosition();
								float? totalDistanceToTarget = null;
								this.m_fullPathCorners.Clear();
								NavMeshHit targetNavHit;
								if (NavMesh.SamplePosition(targetPosition, out targetNavHit, 50f, -1))
								{
									NavMesh.CalculatePath(sourceNavHit.position, targetNavHit.position, -1, this.m_path);
									this.AddCornersToList();
									int iterations = 0;
									float num = float.MaxValue;
									while (this.m_path.status == NavMeshPathStatus.PathPartial && num > 100f)
									{
										yield return null;
										NavMesh.CalculatePath(this.m_fullPathCorners[this.m_fullPathCorners.Count - 1], targetNavHit.position, -1, this.m_path);
										this.AddCornersToList();
										iterations++;
										num = (targetNavHit.position - this.m_fullPathCorners[this.m_fullPathCorners.Count - 1]).sqrMagnitude;
										if (iterations >= 20)
										{
											break;
										}
									}
									if (this.m_path.status == NavMeshPathStatus.PathComplete || num < 100f)
									{
										float num2 = 0f;
										for (int i = 0; i < this.m_fullPathCorners.Count - 1; i++)
										{
											num2 += Vector3.Distance(this.m_fullPathCorners[i], this.m_fullPathCorners[i + 1]);
										}
										if (num2 > 0f && this.m_fullPathCorners.Count > 0)
										{
											totalDistanceToTarget = new float?(num2);
										}
									}
									this.m_fullPathCorners.Clear();
									if (totalDistanceToTarget != null)
									{
										SpawnPointDistanceManager.StoredPlayerSpawn item = new SpawnPointDistanceManager.StoredPlayerSpawn(query, playerSpawn)
										{
											RawDistance = false,
											DistanceToIndexedPos = totalDistanceToTarget.Value
										};
										spawnList.Add(item);
									}
									count++;
									if (count < LocalZoneManager.PlayerSpawnCount)
									{
										yield return null;
									}
									totalDistanceToTarget = null;
									targetNavHit = default(NavMeshHit);
									playerSpawn = null;
								}
							}
							IEnumerator<PlayerSpawn> enumerator = null;
							if (spawnList.Count > 0)
							{
								spawnList.Sort((SpawnPointDistanceManager.StoredPlayerSpawn a, SpawnPointDistanceManager.StoredPlayerSpawn b) => a.DistanceToIndexedPos.CompareTo(b.DistanceToIndexedPos));
							}
							this.m_closestForIndex.Add(query.Index, spawnList);
							yield return null;
							query = default(IndexedVector3);
							sourceNavHit = default(NavMeshHit);
							spawnList = null;
						}
					}
				}
			}
			yield break;
			yield break;
		}

		// Token: 0x0600291E RID: 10526 RVA: 0x0013FF34 File Offset: 0x0013E134
		private void AddCornersToList()
		{
			int cornersNonAlloc = this.m_path.GetCornersNonAlloc(this.m_cornerCache);
			for (int i = 0; i < cornersNonAlloc; i++)
			{
				this.m_fullPathCorners.Add(this.m_cornerCache[i]);
			}
		}

		// Token: 0x04002A09 RID: 10761
		public const float kCellSize = 10f;

		// Token: 0x04002A0A RID: 10762
		private const int kMaxIterations = 20;

		// Token: 0x04002A0B RID: 10763
		private const int kSize = 512;

		// Token: 0x04002A0C RID: 10764
		private const float kMaxSampleDistance = 50f;

		// Token: 0x04002A0D RID: 10765
		private const float kSqrDistanceToFinishThreshold = 100f;

		// Token: 0x04002A0E RID: 10766
		private Dictionary<int, List<SpawnPointDistanceManager.StoredPlayerSpawn>> m_closestForIndex;

		// Token: 0x04002A0F RID: 10767
		private Queue<IndexedVector3> m_updateQueue;

		// Token: 0x04002A10 RID: 10768
		private List<Vector3> m_fullPathCorners;

		// Token: 0x04002A11 RID: 10769
		private readonly Vector3[] m_cornerCache = new Vector3[200];

		// Token: 0x04002A12 RID: 10770
		private NavMeshPath m_path;

		// Token: 0x04002A13 RID: 10771
		private IEnumerator m_updateCo;

		// Token: 0x04002A14 RID: 10772
		private IEnumerable<PlayerSpawn> m_getPlayerSpawns;

		// Token: 0x02000547 RID: 1351
		private struct StoredPlayerSpawn
		{
			// Token: 0x06002920 RID: 10528 RVA: 0x0005C743 File Offset: 0x0005A943
			public StoredPlayerSpawn(IndexedVector3 indexedPos, PlayerSpawn ps)
			{
				this.RawDistance = true;
				this.PlayerSpawn = ps;
				this.DistanceToIndexedPos = float.MaxValue;
				this.IndexedPos = indexedPos;
			}

			// Token: 0x04002A15 RID: 10773
			public bool RawDistance;

			// Token: 0x04002A16 RID: 10774
			public PlayerSpawn PlayerSpawn;

			// Token: 0x04002A17 RID: 10775
			public float DistanceToIndexedPos;

			// Token: 0x04002A18 RID: 10776
			public readonly IndexedVector3 IndexedPos;
		}
	}
}
