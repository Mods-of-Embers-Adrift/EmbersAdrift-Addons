using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x0200009B RID: 155
	public abstract class RTIvy : MonoBehaviour
	{
		// Token: 0x060005F2 RID: 1522 RVA: 0x0004717D File Offset: 0x0004537D
		public void AwakeInit()
		{
			this.bakedMesh = this.meshFilter.sharedMesh;
			this.meshFilter.sharedMesh = null;
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x000A5BE0 File Offset: 0x000A3DE0
		protected virtual void Init(IvyContainer ivyContainer, IvyParameters ivyParameters)
		{
			this.rtIvyContainer = new RTIvyContainer();
			this.ivyParameters = new IvyParameters();
			this.ivyParameters.CopyFrom(ivyParameters);
			this.CreateLeavesDict();
			if (ivyContainer != null)
			{
				this.rtIvyContainer.Initialize(ivyContainer, ivyParameters, base.gameObject, this.leavesMeshesByChosenLeaf, ivyContainer.firstVertexVector);
			}
			else
			{
				this.rtIvyContainer.Initialize();
			}
			this.SetUpMaxBranches(ivyContainer);
			this.activeBakedBranches = new List<RTBranchContainer>(this.maxBranches);
			this.activeBuildingBranches = new List<RTBranchContainer>(this.maxBranches);
			this.rtBuildingIvyContainer = new RTIvyContainer();
			Vector3 firstVertexVector = (ivyContainer == null) ? this.CalculateFirstVertexVector() : ivyContainer.firstVertexVector;
			this.rtBuildingIvyContainer.Initialize(firstVertexVector);
			this.lastIdxActiveBranch = -1;
			this.leafLengthCorrrectionFactor = 1f;
			int subMeshCount = ivyParameters.leavesPrefabs.Length + 1;
			this.processedMesh = new Mesh();
			this.processedMesh.subMeshCount = subMeshCount;
			this.mfProcessedMesh.sharedMesh = this.processedMesh;
			this.refreshProcessedMesh = false;
			this.backtrackingPoints = this.GetBacktrackingPoints();
			if (this.bakedMesh == null)
			{
				this.bakedMesh = new Mesh();
				this.bakedMesh.subMeshCount = subMeshCount;
			}
			this.lastCopiedIndexPerBranch = new List<int>(this.maxBranches);
			this.leavesToCopyMesh = new List<LeafPoint>(50);
			this.srcPoints = new List<Vector3>(this.maxBranches);
			this.dstPoints = new List<Vector3>(this.maxBranches);
			this.growingFactorPerBranch = new List<float>(this.maxBranches);
			this.srcTotalLengthPerBranch = new List<float>(this.maxBranches);
			this.dstTotalLengthPerBranch = new List<float>(this.maxBranches);
			this.lengthPerBranch = new List<float>(this.maxBranches);
			for (int i = 0; i < this.maxBranches; i++)
			{
				this.srcPoints.Add(Vector3.zero);
				this.dstPoints.Add(Vector3.zero);
				this.growingFactorPerBranch.Add(0f);
				this.srcTotalLengthPerBranch.Add(0f);
				this.dstTotalLengthPerBranch.Add(0f);
				this.lastCopiedIndexPerBranch.Add(-1);
				this.lengthPerBranch.Add(0f);
				int maxNumPoints = this.GetMaxNumPoints();
				int maxNumLeaves = this.GetMaxNumLeaves();
				RTBranchContainer item = new RTBranchContainer(maxNumPoints, maxNumLeaves);
				this.activeBuildingBranches.Add(item);
			}
		}

		// Token: 0x060005F4 RID: 1524 RVA: 0x0004719C File Offset: 0x0004539C
		private void SetUpMaxBranches(IvyContainer ivyContainer)
		{
			this.maxBranches = this.ivyParameters.maxBranchs;
			if (ivyContainer != null)
			{
				this.maxBranches = Mathf.Max(this.ivyParameters.maxBranchs, ivyContainer.branches.Count);
			}
		}

		// Token: 0x060005F5 RID: 1525 RVA: 0x000A5E48 File Offset: 0x000A4048
		protected void InitMeshBuilder()
		{
			this.meshBuilder = new RTBakedMeshBuilder(this.rtIvyContainer, base.gameObject);
			this.meshBuilder.InitializeMeshBuilder(this.ivyParameters, this.rtBuildingIvyContainer, this.rtIvyContainer, base.gameObject, this.bakedMesh, this.meshRenderer, this.meshFilter, this.maxBranches, this.processedMesh, this.growthParameters.growthSpeed, this.mrProcessedMesh, this.backtrackingPoints, this.submeshByChoseLeaf, this.leavesMeshesByChosenLeaf, this.leavesMaterials.ToArray());
			this.InitializeMeshesData(this.bakedMesh, this.maxBranches);
		}

		// Token: 0x060005F6 RID: 1526 RVA: 0x000471D9 File Offset: 0x000453D9
		protected virtual void AddFirstBranch()
		{
			this.AddNextBranch(0);
		}

		// Token: 0x060005F7 RID: 1527 RVA: 0x000471E2 File Offset: 0x000453E2
		private int GetBacktrackingPoints()
		{
			return Mathf.CeilToInt(this.ivyParameters.tipInfluence / this.ivyParameters.stepSize);
		}

		// Token: 0x060005F8 RID: 1528 RVA: 0x000A5EF0 File Offset: 0x000A40F0
		public virtual void UpdateIvy(float deltaTime)
		{
			this.UpdateGrowthSpeed();
			for (int i = 0; i < this.activeBakedBranches.Count; i++)
			{
				this.Growing(i, deltaTime);
			}
			this.currentTimer += deltaTime;
			this.RefreshGeometry();
			if (this.refreshProcessedMesh)
			{
				this.meshBuilder.RefreshProcessedMesh();
				this.refreshProcessedMesh = false;
			}
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x000A5F50 File Offset: 0x000A4150
		protected virtual void Growing(int branchIndex, float deltaTime)
		{
			RTBranchContainer rtbranchContainer = this.activeBuildingBranches[branchIndex];
			this.CalculateFactors(this.srcPoints[branchIndex], this.dstPoints[branchIndex]);
			this.meshBuilder.SetLeafLengthCorrectionFactor(this.leafLengthCorrrectionFactor);
			List<float> list = this.growingFactorPerBranch;
			list[branchIndex] += this.currentSpeed * deltaTime;
			this.growingFactorPerBranch[branchIndex] = Mathf.Clamp(this.growingFactorPerBranch[branchIndex], 0f, 1f);
			rtbranchContainer.totalLength = Mathf.Lerp(this.srcTotalLengthPerBranch[branchIndex], this.dstTotalLengthPerBranch[branchIndex], this.growingFactorPerBranch[branchIndex]);
			RTBranchPoint lastBranchPoint = rtbranchContainer.GetLastBranchPoint();
			lastBranchPoint.length = rtbranchContainer.totalLength;
			lastBranchPoint.point = Vector3.Lerp(this.srcPoints[branchIndex], this.dstPoints[branchIndex], this.growingFactorPerBranch[branchIndex]);
			if (this.growingFactorPerBranch[branchIndex] >= 1f)
			{
				this.RefreshGeometry();
				this.NextPoints(branchIndex);
			}
		}

		// Token: 0x060005FA RID: 1530 RVA: 0x000A6070 File Offset: 0x000A4270
		protected virtual void NextPoints(int branchIndex)
		{
			if (this.rtBuildingIvyContainer.branches[branchIndex].branchPoints.Count > 0)
			{
				RTBranchPoint lastBranchPoint = this.rtBuildingIvyContainer.branches[branchIndex].GetLastBranchPoint();
				if (lastBranchPoint.index < this.activeBakedBranches[branchIndex].branchPoints.Count - 1)
				{
					int num = lastBranchPoint.index;
					num++;
					RTBranchPoint rtbranchPoint = this.activeBakedBranches[branchIndex].branchPoints[num];
					this.rtBuildingIvyContainer.branches[branchIndex].AddBranchPoint(rtbranchPoint, this.ivyParameters.stepSize);
					if (rtbranchPoint.newBranch && this.rtIvyContainer.GetBranchContainerByBranchNumber(rtbranchPoint.newBranchNumber).branchPoints.Count >= 2)
					{
						this.AddNextBranch(rtbranchPoint.newBranchNumber);
					}
					this.UpdateGrowingPoints(branchIndex);
					if (this.rtBuildingIvyContainer.branches[branchIndex].branchPoints.Count > this.backtrackingPoints)
					{
						if (!this.IsVertexLimitReached())
						{
							this.meshBuilder.CheckCopyMesh(branchIndex, this.activeBakedBranches);
							this.refreshProcessedMesh = true;
							return;
						}
						Debug.LogWarning("Limit vertices reached! --> " + 65535.ToString() + " vertices", this.meshBuilder.ivyGO);
					}
				}
			}
		}

		// Token: 0x060005FB RID: 1531 RVA: 0x000A61C8 File Offset: 0x000A43C8
		private void CalculateFactors(Vector3 srcPoint, Vector3 dstPoint)
		{
			float num = Vector3.Distance(srcPoint, dstPoint) / this.ivyParameters.stepSize;
			num = 1f / num;
			this.currentSpeed = num * this.currentGrowthSpeed;
			this.leafLengthCorrrectionFactor = Mathf.Lerp(0.92f, 1f, num);
		}

		// Token: 0x060005FC RID: 1532 RVA: 0x000A6218 File Offset: 0x000A4418
		protected virtual void AddNextBranch(int branchNumber)
		{
			this.lastIdxActiveBranch++;
			RTBranchContainer rtbranchContainer = this.activeBuildingBranches[this.lastIdxActiveBranch];
			RTBranchContainer branchContainerByBranchNumber = this.rtIvyContainer.GetBranchContainerByBranchNumber(branchNumber);
			rtbranchContainer.AddBranchPoint(branchContainerByBranchNumber.branchPoints[0], this.ivyParameters.stepSize);
			rtbranchContainer.AddBranchPoint(branchContainerByBranchNumber.branchPoints[1], this.ivyParameters.stepSize);
			rtbranchContainer.leavesOrderedByInitSegment = branchContainerByBranchNumber.leavesOrderedByInitSegment;
			this.rtBuildingIvyContainer.AddBranch(rtbranchContainer);
			this.activeBakedBranches.Add(branchContainerByBranchNumber);
			this.activeBuildingBranches.Add(rtbranchContainer);
			this.meshBuilder.activeBranches.Add(rtbranchContainer);
			this.UpdateGrowingPoints(this.rtBuildingIvyContainer.branches.Count - 1);
			RTBranchPoint lastBranchPoint = rtbranchContainer.GetLastBranchPoint();
			if (lastBranchPoint.newBranch)
			{
				this.AddNextBranch(lastBranchPoint.newBranchNumber);
			}
		}

		// Token: 0x060005FD RID: 1533 RVA: 0x000A6300 File Offset: 0x000A4500
		private void UpdateGrowingPoints(int branchIndex)
		{
			if (this.rtBuildingIvyContainer.branches[branchIndex].branchPoints.Count > 0)
			{
				RTBranchPoint lastBranchPoint = this.rtBuildingIvyContainer.branches[branchIndex].GetLastBranchPoint();
				if (lastBranchPoint.index < this.activeBakedBranches[branchIndex].branchPoints.Count - 1)
				{
					RTBranchPoint rtbranchPoint = this.activeBakedBranches[branchIndex].branchPoints[lastBranchPoint.index + 1];
					this.growingFactorPerBranch[branchIndex] = 0f;
					this.srcPoints[branchIndex] = lastBranchPoint.point;
					this.dstPoints[branchIndex] = rtbranchPoint.point;
					this.srcTotalLengthPerBranch[branchIndex] = lastBranchPoint.length;
					this.dstTotalLengthPerBranch[branchIndex] = lastBranchPoint.length + this.ivyParameters.stepSize;
				}
			}
		}

		// Token: 0x060005FE RID: 1534 RVA: 0x00047200 File Offset: 0x00045400
		private void RefreshGeometry()
		{
			this.meshBuilder.BuildGeometry02(this.activeBakedBranches, this.activeBuildingBranches);
		}

		// Token: 0x060005FF RID: 1535 RVA: 0x000A63EC File Offset: 0x000A45EC
		private void UpdateGrowthSpeed()
		{
			this.currentGrowthSpeed = this.growthParameters.growthSpeed;
			if (this.growthParameters.speedOverLifetimeEnabled)
			{
				float normalizedLifeTime = this.GetNormalizedLifeTime();
				this.currentGrowthSpeed = this.growthParameters.growthSpeed * this.growthParameters.speedOverLifetimeCurve.Evaluate(normalizedLifeTime);
			}
		}

		// Token: 0x06000600 RID: 1536 RVA: 0x00047219 File Offset: 0x00045419
		public bool IsVertexLimitReached()
		{
			return this.meshBuilder.processedMeshData.VertexCount() + this.ivyParameters.sides + 1 >= 65535;
		}

		// Token: 0x06000601 RID: 1537 RVA: 0x00047243 File Offset: 0x00045443
		private Vector3 CalculateFirstVertexVector()
		{
			return Quaternion.AngleAxis(UnityEngine.Random.value * 360f, base.transform.up) * base.transform.forward;
		}

		// Token: 0x06000602 RID: 1538 RVA: 0x000A6444 File Offset: 0x000A4644
		private void CreateLeavesDict()
		{
			new List<List<int>>();
			this.leavesMaterials = new List<Material>();
			this.leavesMeshesByChosenLeaf = new RTMeshData[this.ivyParameters.leavesPrefabs.Length];
			this.leavesMaterials.Add(this.ivyParameters.branchesMaterial);
			this.submeshByChoseLeaf = new int[this.ivyParameters.leavesPrefabs.Length];
			int num = 0;
			for (int i = 0; i < this.ivyParameters.leavesPrefabs.Length; i++)
			{
				MeshRenderer component = this.ivyParameters.leavesPrefabs[i].GetComponent<MeshRenderer>();
				MeshFilter component2 = this.ivyParameters.leavesPrefabs[i].GetComponent<MeshFilter>();
				if (!this.leavesMaterials.Contains(component.sharedMaterial))
				{
					this.leavesMaterials.Add(component.sharedMaterial);
					num++;
				}
				this.submeshByChoseLeaf[i] = num;
				RTMeshData rtmeshData = new RTMeshData(component2.sharedMesh);
				this.leavesMeshesByChosenLeaf[i] = rtmeshData;
			}
			Material[] sharedMaterials = this.leavesMaterials.ToArray();
			this.mrProcessedMesh.sharedMaterials = sharedMaterials;
		}

		// Token: 0x06000603 RID: 1539
		protected abstract void InitializeMeshesData(Mesh bakedMesh, int numBranches);

		// Token: 0x06000604 RID: 1540
		protected abstract float GetNormalizedLifeTime();

		// Token: 0x06000605 RID: 1541
		protected abstract int GetMaxNumPoints();

		// Token: 0x06000606 RID: 1542
		protected abstract int GetMaxNumLeaves();

		// Token: 0x06000607 RID: 1543
		public abstract bool IsGrowingFinished();

		// Token: 0x06000608 RID: 1544
		public abstract void InitIvy(RuntimeGrowthParameters growthParameters, IvyContainer ivyContainer, IvyParameters ivyParameters);

		// Token: 0x040006F9 RID: 1785
		protected IvyParameters ivyParameters;

		// Token: 0x040006FA RID: 1786
		protected RTIvyContainer rtIvyContainer;

		// Token: 0x040006FB RID: 1787
		protected RTIvyContainer rtBuildingIvyContainer;

		// Token: 0x040006FC RID: 1788
		public MeshFilter meshFilter;

		// Token: 0x040006FD RID: 1789
		public MeshRenderer meshRenderer;

		// Token: 0x040006FE RID: 1790
		public MeshRenderer mrProcessedMesh;

		// Token: 0x040006FF RID: 1791
		public MeshFilter mfProcessedMesh;

		// Token: 0x04000700 RID: 1792
		protected List<RTBranchContainer> activeBakedBranches;

		// Token: 0x04000701 RID: 1793
		protected List<RTBranchContainer> activeBuildingBranches;

		// Token: 0x04000702 RID: 1794
		protected int lastIdxActiveBranch;

		// Token: 0x04000703 RID: 1795
		public List<float> srcTotalLengthPerBranch;

		// Token: 0x04000704 RID: 1796
		public List<float> dstTotalLengthPerBranch;

		// Token: 0x04000705 RID: 1797
		public List<float> growingFactorPerBranch;

		// Token: 0x04000706 RID: 1798
		public List<float> lengthPerBranch;

		// Token: 0x04000707 RID: 1799
		protected List<int> lastCopiedIndexPerBranch;

		// Token: 0x04000708 RID: 1800
		protected List<Vector3> srcPoints;

		// Token: 0x04000709 RID: 1801
		protected List<Vector3> dstPoints;

		// Token: 0x0400070A RID: 1802
		protected List<LeafPoint> leavesToCopyMesh;

		// Token: 0x0400070B RID: 1803
		public RTBakedMeshBuilder meshBuilder;

		// Token: 0x0400070C RID: 1804
		private Mesh bakedMesh;

		// Token: 0x0400070D RID: 1805
		private Mesh processedMesh;

		// Token: 0x0400070E RID: 1806
		private bool refreshProcessedMesh;

		// Token: 0x0400070F RID: 1807
		private int backtrackingPoints;

		// Token: 0x04000710 RID: 1808
		protected float currentLifetime;

		// Token: 0x04000711 RID: 1809
		protected float currentSpeed;

		// Token: 0x04000712 RID: 1810
		protected float currentGrowthSpeed;

		// Token: 0x04000713 RID: 1811
		protected float leafLengthCorrrectionFactor;

		// Token: 0x04000714 RID: 1812
		protected float currentTimer;

		// Token: 0x04000715 RID: 1813
		protected RuntimeGrowthParameters growthParameters;

		// Token: 0x04000716 RID: 1814
		protected List<Material> leavesMaterials;

		// Token: 0x04000717 RID: 1815
		protected RTMeshData[] leavesMeshesByChosenLeaf;

		// Token: 0x04000718 RID: 1816
		protected int[] submeshByChoseLeaf;

		// Token: 0x04000719 RID: 1817
		protected int maxBranches;
	}
}
