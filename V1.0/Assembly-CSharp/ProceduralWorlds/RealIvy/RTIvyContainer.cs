using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x020000A3 RID: 163
	[Serializable]
	public class RTIvyContainer
	{
		// Token: 0x0600065F RID: 1631 RVA: 0x0004762A File Offset: 0x0004582A
		public void Initialize(Vector3 firstVertexVector)
		{
			this.lastBranchNumberAssigned = 0;
			this.firstVertexVector = firstVertexVector;
			this.branches = new List<RTBranchContainer>();
		}

		// Token: 0x06000660 RID: 1632 RVA: 0x000A8194 File Offset: 0x000A6394
		public void Initialize(IvyContainer ivyContainer, IvyParameters ivyParameters, GameObject ivyGO, RTMeshData[] leavesMeshesByChosenLeaf, Vector3 firstVertexVector)
		{
			this.lastBranchNumberAssigned = 0;
			this.branches = new List<RTBranchContainer>(ivyContainer.branches.Count);
			for (int i = 0; i < ivyContainer.branches.Count; i++)
			{
				RTBranchContainer item = new RTBranchContainer(ivyContainer.branches[i], ivyParameters, this, ivyGO, leavesMeshesByChosenLeaf);
				this.branches.Add(item);
			}
			this.firstVertexVector = firstVertexVector;
		}

		// Token: 0x06000661 RID: 1633 RVA: 0x00047645 File Offset: 0x00045845
		public void Initialize()
		{
			this.branches = new List<RTBranchContainer>();
		}

		// Token: 0x06000662 RID: 1634 RVA: 0x00047652 File Offset: 0x00045852
		public void AddBranch(RTBranchContainer rtBranch)
		{
			rtBranch.branchNumber = this.lastBranchNumberAssigned;
			this.branches.Add(rtBranch);
			this.lastBranchNumberAssigned++;
		}

		// Token: 0x06000663 RID: 1635 RVA: 0x000A8200 File Offset: 0x000A6400
		public RTBranchContainer GetBranchContainerByBranchNumber(int newBranchNumber)
		{
			RTBranchContainer result = null;
			for (int i = 0; i < this.branches.Count; i++)
			{
				if (this.branches[i].branchNumber == newBranchNumber)
				{
					result = this.branches[i];
					break;
				}
			}
			return result;
		}

		// Token: 0x04000754 RID: 1876
		public int lastBranchNumberAssigned;

		// Token: 0x04000755 RID: 1877
		public List<RTBranchContainer> branches;

		// Token: 0x04000756 RID: 1878
		public Vector3 firstVertexVector;
	}
}
