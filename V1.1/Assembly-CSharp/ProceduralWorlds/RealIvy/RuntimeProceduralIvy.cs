using System;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x020000A0 RID: 160
	public class RuntimeProceduralIvy : RTIvy
	{
		// Token: 0x06000642 RID: 1602 RVA: 0x000A7B74 File Offset: 0x000A5D74
		protected override void Init(IvyContainer ivyContainer, IvyParameters ivyParameters)
		{
			base.Init(ivyContainer, ivyParameters);
			this.rtIvyGrowth = new RuntimeIvyGrowth();
			this.rtIvyGrowth.Init(this.rtIvyContainer, ivyParameters, base.gameObject, this.leavesMeshesByChosenLeaf, this.GetMaxNumPoints(), this.GetMaxNumLeaves(), this.GetMaxNumVerticesPerLeaf());
			for (int i = 0; i < 10; i++)
			{
				this.rtIvyGrowth.Step();
			}
			this.currentLifetime = this.growthParameters.lifetime;
		}

		// Token: 0x06000643 RID: 1603 RVA: 0x0004747A File Offset: 0x0004567A
		protected override void NextPoints(int branchIndex)
		{
			base.NextPoints(branchIndex);
			this.rtIvyGrowth.Step();
		}

		// Token: 0x06000644 RID: 1604 RVA: 0x0004748E File Offset: 0x0004568E
		public override bool IsGrowingFinished()
		{
			return this.currentTimer > this.currentLifetime;
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x0004749E File Offset: 0x0004569E
		protected override float GetNormalizedLifeTime()
		{
			return Mathf.Clamp(this.currentTimer / this.growthParameters.lifetime, 0.1f, 1f);
		}

		// Token: 0x06000646 RID: 1606 RVA: 0x000474C1 File Offset: 0x000456C1
		public void SetIvyParameters(IvyPreset ivyPreset)
		{
			this.ivyParameters.CopyFrom(ivyPreset);
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x000474CF File Offset: 0x000456CF
		protected override void InitializeMeshesData(Mesh bakedMesh, int numBranches)
		{
			this.meshBuilder.InitializeMeshesDataProcedural(bakedMesh, numBranches, this.growthParameters.lifetime, this.growthParameters.growthSpeed);
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x000A7BF0 File Offset: 0x000A5DF0
		protected override int GetMaxNumPoints()
		{
			float num = this.ivyParameters.stepSize / this.growthParameters.growthSpeed;
			Mathf.CeilToInt(this.growthParameters.lifetime / num);
			int maxBranchs = this.ivyParameters.maxBranchs;
			return 20;
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x000474F4 File Offset: 0x000456F4
		protected override int GetMaxNumLeaves()
		{
			return this.GetMaxNumPoints();
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x000474FC File Offset: 0x000456FC
		public override void InitIvy(RuntimeGrowthParameters growthParameters, IvyContainer ivyContainer, IvyParameters ivyParameters)
		{
			this.growthParameters = growthParameters;
			this.Init(null, ivyParameters);
			base.InitMeshBuilder();
			this.AddFirstBranch();
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x000A7C38 File Offset: 0x000A5E38
		private int GetMaxNumVerticesPerLeaf()
		{
			int num = 0;
			for (int i = 0; i < this.ivyParameters.leavesPrefabs.Length; i++)
			{
				if (num <= this.leavesMeshesByChosenLeaf[i].vertices.Length)
				{
					num = this.leavesMeshesByChosenLeaf[i].vertices.Length;
				}
			}
			return num;
		}

		// Token: 0x04000737 RID: 1847
		private RuntimeIvyGrowth rtIvyGrowth;
	}
}
