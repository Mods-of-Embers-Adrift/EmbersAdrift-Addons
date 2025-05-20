using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x020000A1 RID: 161
	public class RTBranchContainer
	{
		// Token: 0x0600064D RID: 1613 RVA: 0x000A7CA4 File Offset: 0x000A5EA4
		public RTBranchContainer(BranchContainer branchContainer, IvyParameters ivyParameters, RTIvyContainer rtIvyContainer, GameObject ivyGO, RTMeshData[] leavesMeshesByChosenLeaf)
		{
			this.totalLength = branchContainer.totalLenght;
			this.growDirection = branchContainer.growDirection;
			this.randomizeHeight = branchContainer.randomizeHeight;
			this.heightVar = branchContainer.heightVar;
			this.newHeight = branchContainer.newHeight;
			this.heightParameter = branchContainer.heightParameter;
			this.deltaHeight = branchContainer.deltaHeight;
			this.currentHeight = branchContainer.currentHeight;
			this.branchSense = branchContainer.branchSense;
			this.falling = branchContainer.falling;
			this.rotationOnFallIteration = branchContainer.rotationOnFallIteration;
			this.branchNumber = branchContainer.branchNumber;
			this.branchPoints = new List<RTBranchPoint>(branchContainer.branchPoints.Count);
			for (int i = 0; i < branchContainer.branchPoints.Count; i++)
			{
				RTBranchPoint rtbranchPoint = new RTBranchPoint(branchContainer.branchPoints[i], this);
				rtbranchPoint.CalculateCenterLoop(ivyGO);
				rtbranchPoint.PreInit(ivyParameters);
				rtbranchPoint.CalculateVerticesLoop(ivyParameters, rtIvyContainer, ivyGO);
				this.branchPoints.Add(rtbranchPoint);
			}
			branchContainer.PrepareRTLeavesDict();
			if (ivyParameters.generateLeaves)
			{
				this.leavesOrderedByInitSegment = new RTLeafPoint[this.branchPoints.Count][];
				for (int j = 0; j < this.branchPoints.Count; j++)
				{
					List<LeafPoint> list = branchContainer.dictRTLeavesByInitSegment[j];
					int num = 0;
					if (list != null)
					{
						num = list.Count;
					}
					this.leavesOrderedByInitSegment[j] = new RTLeafPoint[num];
					for (int k = 0; k < num; k++)
					{
						RTLeafPoint rtleafPoint = new RTLeafPoint(list[k], ivyParameters);
						RTMeshData leafMeshData = leavesMeshesByChosenLeaf[rtleafPoint.chosenLeave];
						rtleafPoint.CreateVertices(ivyParameters, leafMeshData, ivyGO);
						this.leavesOrderedByInitSegment[j][k] = rtleafPoint;
					}
				}
			}
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x000A7E5C File Offset: 0x000A605C
		public Vector2 GetLastUV(IvyParameters ivyParameters)
		{
			return new Vector2(this.totalLength * ivyParameters.uvScale.y + ivyParameters.uvOffset.y, 0.5f * ivyParameters.uvScale.x + ivyParameters.uvOffset.x);
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x00047519 File Offset: 0x00045719
		public RTBranchContainer(int numPoints, int numLeaves)
		{
			this.Init(numPoints, numLeaves);
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x000A7EAC File Offset: 0x000A60AC
		private void Init(int numPoints, int numLeaves)
		{
			this.branchPoints = new List<RTBranchPoint>(numPoints);
			this.leavesOrderedByInitSegment = new RTLeafPoint[numPoints][];
			for (int i = 0; i < numPoints; i++)
			{
				this.leavesOrderedByInitSegment[i] = new RTLeafPoint[1];
			}
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x00047529 File Offset: 0x00045729
		public void AddBranchPoint(RTBranchPoint rtBranchPoint, float deltaLength)
		{
			this.totalLength += deltaLength;
			rtBranchPoint.length = this.totalLength;
			rtBranchPoint.index = this.branchPoints.Count;
			rtBranchPoint.branchContainer = this;
			this.branchPoints.Add(rtBranchPoint);
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x00047569 File Offset: 0x00045769
		public RTBranchPoint GetLastBranchPoint()
		{
			return this.branchPoints[this.branchPoints.Count - 1];
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x000A7EEC File Offset: 0x000A60EC
		public void AddLeaf(RTLeafPoint leafAdded)
		{
			if (leafAdded.initSegmentIdx >= this.leavesOrderedByInitSegment.Length)
			{
				Array.Resize<RTLeafPoint[]>(ref this.leavesOrderedByInitSegment, this.leavesOrderedByInitSegment.Length * 2);
				for (int i = leafAdded.initSegmentIdx; i < this.leavesOrderedByInitSegment.Length; i++)
				{
					this.leavesOrderedByInitSegment[i] = new RTLeafPoint[1];
				}
			}
			this.leavesOrderedByInitSegment[leafAdded.initSegmentIdx][0] = leafAdded;
		}

		// Token: 0x04000738 RID: 1848
		public List<RTBranchPoint> branchPoints;

		// Token: 0x04000739 RID: 1849
		public RTLeafPoint[][] leavesOrderedByInitSegment;

		// Token: 0x0400073A RID: 1850
		public float totalLength;

		// Token: 0x0400073B RID: 1851
		public Vector3 growDirection;

		// Token: 0x0400073C RID: 1852
		public float randomizeHeight;

		// Token: 0x0400073D RID: 1853
		public float heightVar;

		// Token: 0x0400073E RID: 1854
		public float newHeight;

		// Token: 0x0400073F RID: 1855
		public float heightParameter;

		// Token: 0x04000740 RID: 1856
		public float deltaHeight;

		// Token: 0x04000741 RID: 1857
		public float currentHeight;

		// Token: 0x04000742 RID: 1858
		public int branchSense;

		// Token: 0x04000743 RID: 1859
		public bool falling;

		// Token: 0x04000744 RID: 1860
		public Quaternion rotationOnFallIteration;

		// Token: 0x04000745 RID: 1861
		public float fallIteration;

		// Token: 0x04000746 RID: 1862
		public int branchNumber;
	}
}
