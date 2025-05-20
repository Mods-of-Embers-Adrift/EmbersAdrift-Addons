using System;
using System.Collections.Generic;
using System.Diagnostics;
using SoL.Game;
using SoL.Managers;
using UnityEngine;
using UnityEngine.AI;

namespace SoL.Tests
{
	// Token: 0x02000DAE RID: 3502
	[ExecuteInEditMode]
	public class NavMeshPathDisplay : MonoBehaviour
	{
		// Token: 0x17001914 RID: 6420
		// (get) Token: 0x060068DC RID: 26844 RVA: 0x000865FE File Offset: 0x000847FE
		// (set) Token: 0x060068DD RID: 26845 RVA: 0x00086606 File Offset: 0x00084806
		public float PathDistance { get; private set; }

		// Token: 0x17001915 RID: 6421
		// (get) Token: 0x060068DE RID: 26846 RVA: 0x0008660F File Offset: 0x0008480F
		// (set) Token: 0x060068DF RID: 26847 RVA: 0x00086617 File Offset: 0x00084817
		public float ActualDistance { get; private set; }

		// Token: 0x060068E0 RID: 26848 RVA: 0x00086620 File Offset: 0x00084820
		private void Recalculate()
		{
			this.m_pathCompleteTime = null;
		}

		// Token: 0x060068E1 RID: 26849 RVA: 0x00215BE8 File Offset: 0x00213DE8
		private void Update()
		{
			if (!this.m_target)
			{
				return;
			}
			if (this.m_path == null)
			{
				this.m_path = new NavMeshPath();
			}
			if (this.m_pathCompleteTime == null)
			{
				this.CalculateFullPath();
			}
			else
			{
				float num = 0f;
				for (int i = 0; i < this.m_fullPath.Count - 1; i++)
				{
					num += Vector3.Distance(this.m_fullPath[i], this.m_fullPath[i + 1]);
					UnityEngine.Debug.DrawLine(this.m_fullPath[i], this.m_fullPath[i + 1], Color.red);
				}
				this.PathDistance = num;
			}
			this.ActualDistance = Vector3.Distance(base.gameObject.transform.position, this.m_target.transform.position);
		}

		// Token: 0x060068E2 RID: 26850 RVA: 0x00215CC4 File Offset: 0x00213EC4
		private void CalculateFullPath()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			this.m_pathCompleteTime = null;
			this.m_fullPath.Clear();
			NavMeshHit navMeshHit;
			if (!NavMesh.SamplePosition(base.gameObject.transform.position, out navMeshHit, this.MaxSampleDistance, -1))
			{
				stopwatch.Stop();
				UnityEngine.Debug.LogWarning("Invalid source sample!");
				return;
			}
			NavMeshHit navMeshHit2;
			if (!NavMesh.SamplePosition(this.m_target.transform.position, out navMeshHit2, this.MaxSampleDistance, -1))
			{
				stopwatch.Stop();
				UnityEngine.Debug.LogWarning("Invalid target sample!");
				return;
			}
			base.gameObject.transform.position = navMeshHit.position;
			NavMesh.CalculatePath(navMeshHit.position, navMeshHit2.position, -1, this.m_path);
			this.AddCornersToList();
			int num = 1;
			float num2 = float.MaxValue;
			while (this.m_path.status == NavMeshPathStatus.PathPartial && num2 > 100f)
			{
				NavMesh.CalculatePath(this.m_path.corners[this.m_path.corners.Length - 1], navMeshHit2.position, -1, this.m_path);
				this.AddCornersToList();
				num++;
				num2 = (navMeshHit2.position - this.m_fullPath[this.m_fullPath.Count - 1]).sqrMagnitude;
				if (num > this.MaxIterations)
				{
					break;
				}
			}
			this.m_pathCompleteTime = new DateTime?(DateTime.UtcNow);
			stopwatch.Stop();
			UnityEngine.Debug.Log(string.Concat(new string[]
			{
				"Elapsed Stopwatch Time: ",
				stopwatch.Elapsed.TotalMilliseconds.ToString(),
				" for ",
				num.ToString(),
				" iterations"
			}));
		}

		// Token: 0x060068E3 RID: 26851 RVA: 0x00215E8C File Offset: 0x0021408C
		private void AddCornersToList()
		{
			for (int i = 0; i < this.m_path.corners.Length; i++)
			{
				this.m_fullPath.Add(this.m_path.corners[i]);
			}
		}

		// Token: 0x060068E4 RID: 26852 RVA: 0x0008662E File Offset: 0x0008482E
		private void Cache()
		{
			if (Application.isPlaying)
			{
				LocalZoneManager.CacheClosestPlayerSpawn(base.gameObject.transform.position);
			}
		}

		// Token: 0x060068E5 RID: 26853 RVA: 0x00215ED0 File Offset: 0x002140D0
		private void Sample()
		{
			if (Application.isPlaying)
			{
				PlayerSpawn closestPlayerSpawn = LocalZoneManager.GetClosestPlayerSpawn(base.gameObject.transform.position);
				if (closestPlayerSpawn != null)
				{
					this.m_target = closestPlayerSpawn.gameObject;
				}
			}
		}

		// Token: 0x04005B38 RID: 23352
		[SerializeField]
		private GameObject m_target;

		// Token: 0x04005B39 RID: 23353
		private NavMeshPath m_path;

		// Token: 0x04005B3A RID: 23354
		private DateTime? m_pathCompleteTime;

		// Token: 0x04005B3B RID: 23355
		private readonly List<Vector3> m_fullPath = new List<Vector3>(10);

		// Token: 0x04005B3C RID: 23356
		public int MaxIterations = 20;

		// Token: 0x04005B3D RID: 23357
		public float MaxSampleDistance = 50f;
	}
}
