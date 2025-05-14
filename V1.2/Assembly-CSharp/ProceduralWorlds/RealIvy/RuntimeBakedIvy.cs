using System;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x0200009C RID: 156
	public class RuntimeBakedIvy : RTIvy
	{
		// Token: 0x0600060A RID: 1546 RVA: 0x000A6568 File Offset: 0x000A4768
		public override bool IsGrowingFinished()
		{
			bool flag = true;
			if (this.rtIvyContainer.branches.Count > this.rtBuildingIvyContainer.branches.Count)
			{
				flag = false;
			}
			else
			{
				for (int i = 0; i < this.activeBakedBranches.Count; i++)
				{
					flag = (flag && this.rtBuildingIvyContainer.branches[i].branchPoints.Count >= this.activeBakedBranches[i].branchPoints.Count);
				}
			}
			return flag;
		}

		// Token: 0x0600060B RID: 1547 RVA: 0x00047270 File Offset: 0x00045470
		protected override void Init(IvyContainer ivyContainer, IvyParameters ivyParameters)
		{
			base.Init(ivyContainer, ivyParameters);
			this.CalculateLifetime();
		}

		// Token: 0x0600060C RID: 1548 RVA: 0x000A65F4 File Offset: 0x000A47F4
		private void CalculateLifetime()
		{
			float num = 0f;
			for (int i = 0; i < this.rtIvyContainer.branches.Count; i++)
			{
				num += this.rtIvyContainer.branches[i].totalLength;
			}
			this.currentLifetime = num / this.growthParameters.growthSpeed;
			this.currentLifetime *= 2f;
		}

		// Token: 0x0600060D RID: 1549 RVA: 0x00047280 File Offset: 0x00045480
		protected override float GetNormalizedLifeTime()
		{
			return Mathf.Clamp(this.rtBuildingIvyContainer.branches[0].totalLength / this.rtIvyContainer.branches[0].totalLength, 0.1f, 1f);
		}

		// Token: 0x0600060E RID: 1550 RVA: 0x000472BE File Offset: 0x000454BE
		protected override void InitializeMeshesData(Mesh bakedMesh, int numBranches)
		{
			this.meshBuilder.InitializeMeshesDataBaked(bakedMesh, numBranches);
		}

		// Token: 0x0600060F RID: 1551 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override int GetMaxNumPoints()
		{
			return 0;
		}

		// Token: 0x06000610 RID: 1552 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override int GetMaxNumLeaves()
		{
			return 0;
		}

		// Token: 0x06000611 RID: 1553 RVA: 0x000472CD File Offset: 0x000454CD
		public override void InitIvy(RuntimeGrowthParameters growthParameters, IvyContainer ivyContainer, IvyParameters ivyParameters)
		{
			this.growthParameters = growthParameters;
			this.Init(ivyContainer, ivyParameters);
			base.InitMeshBuilder();
			this.AddFirstBranch();
		}

		// Token: 0x06000612 RID: 1554 RVA: 0x000472EA File Offset: 0x000454EA
		public void InitIvyEditor(RuntimeGrowthParameters growthParameters, IvyContainer ivyContainer, IvyParameters ivyParameters)
		{
			this.growthParameters = growthParameters;
			this.Init(ivyContainer, ivyParameters);
		}
	}
}
