using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

namespace SoL.Tests
{
	// Token: 0x02000DAF RID: 3503
	public class NavMeshPathDisplaySpawnPoints : MonoBehaviour
	{
		// Token: 0x04005B40 RID: 23360
		[SerializeField]
		private int m_maxIterations = 20;

		// Token: 0x04005B41 RID: 23361
		[SerializeField]
		private float m_maxSampleDistance = 10f;

		// Token: 0x04005B42 RID: 23362
		private List<NavMeshPathDisplaySpawnPoints.NavMeshPathCalculation> m_calculations;

		// Token: 0x04005B43 RID: 23363
		private bool m_sorted;

		// Token: 0x02000DB0 RID: 3504
		private class NavMeshPathCalculation
		{
			// Token: 0x17001916 RID: 6422
			// (get) Token: 0x060068E8 RID: 26856 RVA: 0x0008668F File Offset: 0x0008488F
			// (set) Token: 0x060068E9 RID: 26857 RVA: 0x00086697 File Offset: 0x00084897
			public float PathDistance { get; private set; }

			// Token: 0x17001917 RID: 6423
			// (get) Token: 0x060068EA RID: 26858 RVA: 0x000866A0 File Offset: 0x000848A0
			// (set) Token: 0x060068EB RID: 26859 RVA: 0x000866A8 File Offset: 0x000848A8
			public float RawDistance { get; private set; }

			// Token: 0x17001918 RID: 6424
			// (get) Token: 0x060068EC RID: 26860 RVA: 0x000866B1 File Offset: 0x000848B1
			// (set) Token: 0x060068ED RID: 26861 RVA: 0x000866B9 File Offset: 0x000848B9
			public bool IsCalculating { get; private set; }

			// Token: 0x17001919 RID: 6425
			// (get) Token: 0x060068EE RID: 26862 RVA: 0x000866C2 File Offset: 0x000848C2
			public Vector3 TargetPos
			{
				get
				{
					return this.m_targetPos;
				}
			}

			// Token: 0x060068EF RID: 26863 RVA: 0x00215EB0 File Offset: 0x002140B0
			public NavMeshPathCalculation(NavMeshPathDisplaySpawnPoints parent, Vector3 sourcePos, Vector3 targetPos)
			{
				this.m_parent = parent;
				this.m_sourcePos = sourcePos;
				this.m_targetPos = targetPos;
				this.m_pathCompleteTime = null;
				this.IsCalculating = false;
				this.m_path = new NavMeshPath();
			}

			// Token: 0x060068F0 RID: 26864 RVA: 0x00215F04 File Offset: 0x00214104
			public void Update(bool isCalculating)
			{
				if (this.m_pathCompleteTime == null)
				{
					if (!isCalculating)
					{
						this.CalculateFullPath();
						return;
					}
				}
				else
				{
					this.CalculateDistances();
					for (int i = 0; i < this.m_fullPath.Count - 1; i++)
					{
						UnityEngine.Debug.DrawLine(this.m_fullPath[i], this.m_fullPath[i + 1], Color.red);
					}
				}
			}

			// Token: 0x060068F1 RID: 26865 RVA: 0x00215F6C File Offset: 0x0021416C
			private void CalculateDistances()
			{
				if (this.m_distancesCalculated)
				{
					return;
				}
				float num = 0f;
				for (int i = 0; i < this.m_fullPath.Count - 1; i++)
				{
					num += Vector3.Distance(this.m_fullPath[i], this.m_fullPath[i + 1]);
				}
				this.PathDistance = num;
				this.RawDistance = Vector3.Distance(this.m_sourcePos, this.m_targetPos);
				this.m_distancesCalculated = true;
			}

			// Token: 0x060068F2 RID: 26866 RVA: 0x00215FE8 File Offset: 0x002141E8
			private void CalculateFullPath()
			{
				this.IsCalculating = true;
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				this.m_pathCompleteTime = null;
				this.m_fullPath.Clear();
				NavMeshHit navMeshHit;
				if (!NavMesh.SamplePosition(this.m_sourcePos, out navMeshHit, this.m_parent.m_maxSampleDistance, -1))
				{
					stopwatch.Stop();
					UnityEngine.Debug.LogWarning("Invalid source sample!");
					return;
				}
				NavMeshHit navMeshHit2;
				if (!NavMesh.SamplePosition(this.m_targetPos, out navMeshHit2, this.m_parent.m_maxSampleDistance, -1))
				{
					stopwatch.Stop();
					UnityEngine.Debug.LogWarning("Invalid target sample!");
					return;
				}
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
					if (num > this.m_parent.m_maxIterations)
					{
						break;
					}
				}
				stopwatch.Stop();
				double totalMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
				this.m_pathCompleteTime = new float?((float)totalMilliseconds);
				UnityEngine.Debug.Log(string.Concat(new string[]
				{
					"Elapsed Stopwatch Time: ",
					totalMilliseconds.ToString(),
					" for ",
					num.ToString(),
					" iterations"
				}));
				this.IsCalculating = false;
			}

			// Token: 0x060068F3 RID: 26867 RVA: 0x002161A0 File Offset: 0x002143A0
			private void AddCornersToList()
			{
				for (int i = 0; i < this.m_path.corners.Length; i++)
				{
					this.m_fullPath.Add(this.m_path.corners[i]);
				}
			}

			// Token: 0x04005B44 RID: 23364
			private NavMeshPath m_path;

			// Token: 0x04005B45 RID: 23365
			private readonly List<Vector3> m_fullPath = new List<Vector3>(10);

			// Token: 0x04005B46 RID: 23366
			private readonly NavMeshPathDisplaySpawnPoints m_parent;

			// Token: 0x04005B47 RID: 23367
			private readonly Vector3 m_sourcePos;

			// Token: 0x04005B48 RID: 23368
			private readonly Vector3 m_targetPos;

			// Token: 0x04005B49 RID: 23369
			private float? m_pathCompleteTime;

			// Token: 0x04005B4A RID: 23370
			private bool m_distancesCalculated;
		}
	}
}
