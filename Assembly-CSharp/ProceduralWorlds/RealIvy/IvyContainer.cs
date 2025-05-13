using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x02000093 RID: 147
	[Serializable]
	public class IvyContainer : ScriptableObject
	{
		// Token: 0x060005C4 RID: 1476 RVA: 0x00046FC9 File Offset: 0x000451C9
		private IvyContainer()
		{
			this.branches = new List<BranchContainer>();
			this.lastNumberAssigned = 0;
		}

		// Token: 0x060005C5 RID: 1477 RVA: 0x00046FE3 File Offset: 0x000451E3
		public void Clear()
		{
			this.lastNumberAssigned = 0;
			this.branches.Clear();
		}

		// Token: 0x060005C6 RID: 1478 RVA: 0x00046FF7 File Offset: 0x000451F7
		public void RemoveBranch(BranchContainer branchToDelete)
		{
			if (branchToDelete.originPointOfThisBranch != null)
			{
				branchToDelete.originPointOfThisBranch.branchContainer.ReleasePoint(branchToDelete.originPointOfThisBranch.index);
			}
			this.branches.Remove(branchToDelete);
		}

		// Token: 0x060005C7 RID: 1479 RVA: 0x000A38BC File Offset: 0x000A1ABC
		public BranchContainer GetBranchContainerByBranchNumber(int branchNumber)
		{
			BranchContainer result = null;
			for (int i = 0; i < this.branches.Count; i++)
			{
				if (this.branches[i].branchNumber == branchNumber)
				{
					result = this.branches[i];
					break;
				}
			}
			return result;
		}

		// Token: 0x060005C8 RID: 1480 RVA: 0x000A3908 File Offset: 0x000A1B08
		public BranchPoint[] GetNearestSegmentSSBelowDistance(Vector2 pointSS, float distanceThreshold)
		{
			BranchPoint[] result = null;
			BranchPoint branchPoint = null;
			BranchPoint branchPoint2 = null;
			float num = distanceThreshold;
			for (int i = 0; i < this.branches.Count; i++)
			{
				for (int j = 1; j < this.branches[i].branchPoints.Count; j++)
				{
					BranchPoint branchPoint3 = this.branches[i].branchPoints[j - 1];
					BranchPoint branchPoint4 = this.branches[i].branchPoints[j];
					float num2 = RealIvyMathUtils.DistanceBetweenPointAndSegmentSS(pointSS, branchPoint3.pointSS, branchPoint4.pointSS);
					if (num2 <= num)
					{
						num = num2;
						branchPoint = branchPoint3;
						branchPoint2 = branchPoint4;
					}
				}
			}
			if (branchPoint != null && branchPoint2 != null)
			{
				result = new BranchPoint[]
				{
					branchPoint,
					branchPoint2
				};
			}
			return result;
		}

		// Token: 0x060005C9 RID: 1481 RVA: 0x00047029 File Offset: 0x00045229
		public BranchPoint[] GetNearestSegmentSS(Vector2 pointSS)
		{
			return this.GetNearestSegmentSSBelowDistance(pointSS, float.MaxValue);
		}

		// Token: 0x060005CA RID: 1482 RVA: 0x00047037 File Offset: 0x00045237
		public void AddBranch(BranchContainer newBranchContainer)
		{
			newBranchContainer.branchNumber = this.lastNumberAssigned;
			this.lastNumberAssigned++;
			this.branches.Add(newBranchContainer);
		}

		// Token: 0x060005CB RID: 1483 RVA: 0x000A39D8 File Offset: 0x000A1BD8
		public BranchPoint GetNearestPointAllBranchesSSFrom(Vector2 pointSS)
		{
			BranchPoint result = null;
			float num = float.MaxValue;
			for (int i = 0; i < this.branches.Count; i++)
			{
				for (int j = 0; j < this.branches[i].branchPoints.Count; j++)
				{
					float sqrMagnitude = (this.branches[i].branchPoints[j].pointSS - pointSS).sqrMagnitude;
					if (sqrMagnitude <= num)
					{
						result = this.branches[i].branchPoints[j];
						num = sqrMagnitude;
					}
				}
			}
			return result;
		}

		// Token: 0x04000680 RID: 1664
		public int lastNumberAssigned;

		// Token: 0x04000681 RID: 1665
		public GameObject ivyGO;

		// Token: 0x04000682 RID: 1666
		public List<BranchContainer> branches;

		// Token: 0x04000683 RID: 1667
		public Vector3 firstVertexVector;
	}
}
