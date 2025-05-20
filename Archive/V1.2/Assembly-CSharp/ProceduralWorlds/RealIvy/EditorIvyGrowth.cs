using System;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x0200008B RID: 139
	public class EditorIvyGrowth : GrowthBuilder
	{
		// Token: 0x06000594 RID: 1428 RVA: 0x000A065C File Offset: 0x0009E85C
		public void Initialize(Vector3 firstPoint, Vector3 firstGrabVector)
		{
			UnityEngine.Random.InitState(this.infoPool.ivyParameters.randomSeed);
			BranchContainer branchContainer = ScriptableObject.CreateInstance<BranchContainer>();
			branchContainer.Init();
			branchContainer.currentHeight = this.infoPool.ivyParameters.minDistanceToSurface;
			this.infoPool.ivyContainer.AddBranch(branchContainer);
			this.infoPool.ivyContainer.branches[0].AddBranchPoint(firstPoint, firstGrabVector, true, branchContainer.branchNumber);
			this.infoPool.ivyContainer.branches[0].growDirection = Quaternion.AngleAxis(UnityEngine.Random.value * 360f, this.infoPool.ivyContainer.ivyGO.transform.up) * this.infoPool.ivyContainer.ivyGO.transform.forward;
			this.infoPool.ivyContainer.firstVertexVector = this.infoPool.ivyContainer.branches[0].growDirection;
			this.infoPool.ivyContainer.branches[0].randomizeHeight = UnityEngine.Random.Range(4f, 8f);
			this.CalculateNewHeight(this.infoPool.ivyContainer.branches[0]);
			this.infoPool.ivyContainer.branches[0].branchSense = this.ChooseBranchSense();
			this.randomstate = UnityEngine.Random.state;
		}

		// Token: 0x06000595 RID: 1429 RVA: 0x000A07D8 File Offset: 0x0009E9D8
		private void CalculateNewHeight(BranchContainer branch)
		{
			branch.heightVar = (Mathf.Sin(branch.heightParameter * this.infoPool.ivyParameters.DTSFrequency - 45f) + 1f) / 2f;
			branch.newHeight = Mathf.Lerp(this.infoPool.ivyParameters.minDistanceToSurface, this.infoPool.ivyParameters.maxDistanceToSurface, branch.heightVar);
			branch.newHeight += (Mathf.Sin(branch.heightParameter * this.infoPool.ivyParameters.DTSFrequency * branch.randomizeHeight) + 1f) / 2f * this.infoPool.ivyParameters.maxDistanceToSurface / 4f * this.infoPool.ivyParameters.DTSRandomness;
			branch.newHeight = Mathf.Clamp(branch.newHeight, this.infoPool.ivyParameters.minDistanceToSurface, this.infoPool.ivyParameters.maxDistanceToSurface);
			branch.deltaHeight = branch.currentHeight - branch.newHeight;
			branch.currentHeight = branch.newHeight;
		}

		// Token: 0x06000596 RID: 1430 RVA: 0x00046E78 File Offset: 0x00045078
		private int ChooseBranchSense()
		{
			if (UnityEngine.Random.value < 0.5f)
			{
				return -1;
			}
			return 1;
		}

		// Token: 0x06000597 RID: 1431 RVA: 0x000A0900 File Offset: 0x0009EB00
		public void Step()
		{
			UnityEngine.Random.state = this.randomstate;
			for (int i = 0; i < this.infoPool.ivyContainer.branches.Count; i++)
			{
				this.infoPool.ivyContainer.branches[i].heightParameter += this.infoPool.ivyParameters.stepSize;
				this.CalculateNewPoint(this.infoPool.ivyContainer.branches[i]);
			}
			this.randomstate = UnityEngine.Random.state;
		}

		// Token: 0x06000598 RID: 1432 RVA: 0x00046E89 File Offset: 0x00045089
		private void CalculateNewPoint(BranchContainer branch)
		{
			if (!branch.falling)
			{
				this.CalculateNewHeight(branch);
				this.CheckWall(branch);
				return;
			}
			this.CheckFall(branch);
		}

		// Token: 0x06000599 RID: 1433 RVA: 0x000A0994 File Offset: 0x0009EB94
		private void CheckWall(BranchContainer branch)
		{
			BranchPoint branchPoint = new BranchPoint(branch.GetLastBranchPoint().point + branch.growDirection * this.infoPool.ivyParameters.stepSize + branch.GetLastBranchPoint().grabVector * branch.deltaHeight, branch.branchPoints.Count, 0f, branch);
			Vector3 direction = branchPoint.point - branch.GetLastBranchPoint().point;
			RaycastHit raycastHit;
			if (!Physics.Raycast(new Ray(branch.branchPoints[branch.branchPoints.Count - 1].point, direction), out raycastHit, this.infoPool.ivyParameters.stepSize * 1.15f, this.infoPool.ivyParameters.layerMask.value))
			{
				this.CheckFloor(branch, branchPoint, -branch.GetLastBranchPoint().grabVector);
				return;
			}
			this.NewGrowDirectionAfterWall(branch, -branch.GetLastBranchPoint().grabVector, raycastHit.normal);
			this.AddPoint(branch, raycastHit.point, raycastHit.normal);
		}

		// Token: 0x0600059A RID: 1434 RVA: 0x000A0AB8 File Offset: 0x0009ECB8
		private void CheckFloor(BranchContainer branch, BranchPoint potentialPoint, Vector3 oldSurfaceNormal)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(new Ray(potentialPoint.point, -oldSurfaceNormal), out raycastHit, branch.currentHeight * 2f, this.infoPool.ivyParameters.layerMask.value))
			{
				this.AddPoint(branch, raycastHit.point, raycastHit.normal);
				this.NewGrowDirection(branch);
				branch.fallIteration = 0f;
				branch.falling = false;
				return;
			}
			if (UnityEngine.Random.value < this.infoPool.ivyParameters.grabProvabilityOnFall)
			{
				this.CheckCorner(branch, potentialPoint, oldSurfaceNormal);
				return;
			}
			this.AddFallingPoint(branch);
			branch.fallIteration += 1f - this.infoPool.ivyParameters.stiffness;
			branch.falling = true;
			branch.currentHeight = 0f;
			branch.heightParameter = -45f;
		}

		// Token: 0x0600059B RID: 1435 RVA: 0x000A0B98 File Offset: 0x0009ED98
		private void CheckCorner(BranchContainer branch, BranchPoint potentialPoint, Vector3 oldSurfaceNormal)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(new Ray(potentialPoint.point + branch.branchPoints[branch.branchPoints.Count - 1].grabVector * 2f * branch.currentHeight, -branch.growDirection), out raycastHit, this.infoPool.ivyParameters.stepSize * 1.15f, this.infoPool.ivyParameters.layerMask.value))
			{
				this.AddPoint(branch, potentialPoint.point, oldSurfaceNormal);
				this.AddPoint(branch, raycastHit.point, raycastHit.normal);
				this.NewGrowDirectionAfterCorner(branch, oldSurfaceNormal, raycastHit.normal);
				return;
			}
			this.AddFallingPoint(branch);
			branch.fallIteration += 1f - this.infoPool.ivyParameters.stiffness;
			branch.falling = true;
			branch.currentHeight = 0f;
			branch.heightParameter = -45f;
		}

		// Token: 0x0600059C RID: 1436 RVA: 0x000A0CA0 File Offset: 0x0009EEA0
		private void CheckFall(BranchContainer branch)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(new Ray(branch.branchPoints[branch.branchPoints.Count - 1].point, branch.growDirection), out raycastHit, this.infoPool.ivyParameters.stepSize * 1.15f, this.infoPool.ivyParameters.layerMask.value))
			{
				this.NewGrowDirectionAfterFall(branch, raycastHit.normal);
				this.AddPoint(branch, raycastHit.point, raycastHit.normal);
				branch.fallIteration = 0f;
				branch.falling = false;
				return;
			}
			if (UnityEngine.Random.value < this.infoPool.ivyParameters.grabProvabilityOnFall)
			{
				this.CheckGrabPoint(branch);
				return;
			}
			this.NewGrowDirectionFalling(branch);
			this.AddFallingPoint(branch);
			branch.fallIteration += 1f - this.infoPool.ivyParameters.stiffness;
			branch.falling = true;
		}

		// Token: 0x0600059D RID: 1437 RVA: 0x000A0D98 File Offset: 0x0009EF98
		private void CheckGrabPoint(BranchContainer branch)
		{
			for (int i = 0; i < 6; i++)
			{
				float angle = 60f * (float)i;
				RaycastHit raycastHit;
				if (Physics.Raycast(new Ray(branch.branchPoints[branch.branchPoints.Count - 1].point + branch.growDirection * this.infoPool.ivyParameters.stepSize, Quaternion.AngleAxis(angle, branch.growDirection) * branch.GetLastBranchPoint().grabVector), out raycastHit, this.infoPool.ivyParameters.stepSize * 2f, this.infoPool.ivyParameters.layerMask.value))
				{
					this.AddPoint(branch, raycastHit.point, raycastHit.normal);
					this.NewGrowDirectionAfterGrab(branch, raycastHit.normal);
					branch.fallIteration = 0f;
					branch.falling = false;
					return;
				}
				if (i == 5)
				{
					this.AddFallingPoint(branch);
					this.NewGrowDirectionFalling(branch);
					branch.fallIteration += 1f - this.infoPool.ivyParameters.stiffness;
					branch.falling = true;
				}
			}
		}

		// Token: 0x0600059E RID: 1438 RVA: 0x000A0EC4 File Offset: 0x0009F0C4
		public void AddPoint(BranchContainer branch, Vector3 point, Vector3 normal)
		{
			branch.totalLenght += this.infoPool.ivyParameters.stepSize;
			branch.heightParameter += this.infoPool.ivyParameters.stepSize;
			branch.AddBranchPoint(point + normal * branch.currentHeight, -normal);
			if (this.growing && UnityEngine.Random.value < this.infoPool.ivyParameters.branchProvability && this.infoPool.ivyContainer.branches.Count < this.infoPool.ivyParameters.maxBranchs)
			{
				this.AddBranch(branch, branch.GetLastBranchPoint(), branch.branchPoints[branch.branchPoints.Count - 1].point, normal);
			}
			this.AddLeave(branch);
		}

		// Token: 0x0600059F RID: 1439 RVA: 0x000A0FA4 File Offset: 0x0009F1A4
		private void AddFallingPoint(BranchContainer branch)
		{
			Vector3 grabVector = branch.rotationOnFallIteration * branch.GetLastBranchPoint().grabVector;
			branch.totalLenght += this.infoPool.ivyParameters.stepSize;
			branch.AddBranchPoint(branch.branchPoints[branch.branchPoints.Count - 1].point + branch.growDirection * this.infoPool.ivyParameters.stepSize, grabVector);
			if (UnityEngine.Random.value < this.infoPool.ivyParameters.branchProvability && this.infoPool.ivyContainer.branches.Count < this.infoPool.ivyParameters.maxBranchs)
			{
				this.AddBranch(branch, branch.GetLastBranchPoint(), branch.branchPoints[branch.branchPoints.Count - 1].point, -branch.GetLastBranchPoint().grabVector);
			}
			this.AddLeave(branch);
		}

		// Token: 0x060005A0 RID: 1440 RVA: 0x000A10A8 File Offset: 0x0009F2A8
		private void AddLeave(BranchContainer branch)
		{
			if (branch.branchPoints.Count % (this.infoPool.ivyParameters.leaveEvery + UnityEngine.Random.Range(0, this.infoPool.ivyParameters.randomLeaveEvery)) == 0)
			{
				float[] array = new float[this.infoPool.ivyParameters.leavesPrefabs.Length];
				int chosenLeave = 0;
				float num = 0f;
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = UnityEngine.Random.Range(0f, this.infoPool.ivyParameters.leavesProb[i]);
				}
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j] >= num)
					{
						num = array[j];
						chosenLeave = j;
					}
				}
				BranchPoint branchPoint = branch.branchPoints[branch.branchPoints.Count - 2];
				BranchPoint branchPoint2 = branch.branchPoints[branch.branchPoints.Count - 1];
				Vector3 leafPoint = Vector3.Lerp(branchPoint.point, branchPoint2.point, 0.5f);
				branch.AddLeaf(leafPoint, branch.totalLenght, branch.growDirection, -branch.GetLastBranchPoint().grabVector, chosenLeave, branchPoint, branchPoint2);
			}
		}

		// Token: 0x060005A1 RID: 1441 RVA: 0x00046EA9 File Offset: 0x000450A9
		public void DeleteLastBranch()
		{
			this.infoPool.ivyContainer.branches.RemoveAt(this.infoPool.ivyContainer.branches.Count - 1);
		}

		// Token: 0x060005A2 RID: 1442 RVA: 0x000A11D8 File Offset: 0x0009F3D8
		public void AddBranch(BranchContainer branch, BranchPoint originBranchPoint, Vector3 point, Vector3 normal)
		{
			BranchContainer branchContainer = ScriptableObject.CreateInstance<BranchContainer>();
			branchContainer.Init();
			branchContainer.AddBranchPoint(point, -normal);
			branchContainer.growDirection = Vector3.Normalize(Vector3.ProjectOnPlane(branch.growDirection, normal));
			branchContainer.randomizeHeight = UnityEngine.Random.Range(4f, 8f);
			branchContainer.currentHeight = branch.currentHeight;
			branchContainer.heightParameter = branch.heightParameter;
			branchContainer.branchSense = this.ChooseBranchSense();
			branchContainer.originPointOfThisBranch = originBranchPoint;
			this.infoPool.ivyContainer.AddBranch(branchContainer);
			originBranchPoint.InitBranchInThisPoint(branchContainer.branchNumber);
		}

		// Token: 0x060005A3 RID: 1443 RVA: 0x000A1274 File Offset: 0x0009F474
		private void NewGrowDirection(BranchContainer branch)
		{
			branch.growDirection = Vector3.Normalize(Vector3.ProjectOnPlane(Quaternion.AngleAxis(Mathf.Sin((float)branch.branchSense * branch.totalLenght * this.infoPool.ivyParameters.directionFrequency * (1f + UnityEngine.Random.Range(-this.infoPool.ivyParameters.directionRandomness, this.infoPool.ivyParameters.directionRandomness))) * this.infoPool.ivyParameters.directionAmplitude * this.infoPool.ivyParameters.stepSize * 10f * Mathf.Max(this.infoPool.ivyParameters.directionRandomness, 1f), branch.GetLastBranchPoint().grabVector) * branch.growDirection, branch.GetLastBranchPoint().grabVector));
		}

		// Token: 0x060005A4 RID: 1444 RVA: 0x00046ED7 File Offset: 0x000450D7
		private void NewGrowDirectionAfterWall(BranchContainer branch, Vector3 oldSurfaceNormal, Vector3 newSurfaceNormal)
		{
			branch.growDirection = Vector3.Normalize(Vector3.ProjectOnPlane(oldSurfaceNormal, newSurfaceNormal));
		}

		// Token: 0x060005A5 RID: 1445 RVA: 0x000A134C File Offset: 0x0009F54C
		private void NewGrowDirectionFalling(BranchContainer branch)
		{
			Vector3 vector = Vector3.Lerp(branch.growDirection, this.infoPool.ivyParameters.gravity, branch.fallIteration / 10f);
			vector = Quaternion.AngleAxis(Mathf.Sin((float)branch.branchSense * branch.totalLenght * this.infoPool.ivyParameters.directionFrequency * (1f + UnityEngine.Random.Range(-this.infoPool.ivyParameters.directionRandomness / 8f, this.infoPool.ivyParameters.directionRandomness / 8f))) * this.infoPool.ivyParameters.directionAmplitude * this.infoPool.ivyParameters.stepSize * 5f * Mathf.Max(this.infoPool.ivyParameters.directionRandomness / 8f, 1f), branch.GetLastBranchPoint().grabVector) * vector;
			vector = Quaternion.AngleAxis(Mathf.Sin((float)branch.branchSense * branch.totalLenght * this.infoPool.ivyParameters.directionFrequency / 2f * (1f + UnityEngine.Random.Range(-this.infoPool.ivyParameters.directionRandomness / 8f, this.infoPool.ivyParameters.directionRandomness / 8f))) * this.infoPool.ivyParameters.directionAmplitude * this.infoPool.ivyParameters.stepSize * 5f * Mathf.Max(this.infoPool.ivyParameters.directionRandomness / 8f, 1f), Vector3.Cross(branch.GetLastBranchPoint().grabVector, branch.growDirection)) * vector;
			branch.rotationOnFallIteration = Quaternion.FromToRotation(branch.growDirection, vector);
			branch.growDirection = vector;
		}

		// Token: 0x060005A6 RID: 1446 RVA: 0x00046EEB File Offset: 0x000450EB
		private void NewGrowDirectionAfterFall(BranchContainer branch, Vector3 newSurfaceNormal)
		{
			branch.growDirection = Vector3.Normalize(Vector3.ProjectOnPlane(-branch.GetLastBranchPoint().grabVector, newSurfaceNormal));
		}

		// Token: 0x060005A7 RID: 1447 RVA: 0x00046F0E File Offset: 0x0004510E
		private void NewGrowDirectionAfterGrab(BranchContainer branch, Vector3 newSurfaceNormal)
		{
			branch.growDirection = Vector3.Normalize(Vector3.ProjectOnPlane(branch.growDirection, newSurfaceNormal));
		}

		// Token: 0x060005A8 RID: 1448 RVA: 0x00046F27 File Offset: 0x00045127
		private void NewGrowDirectionAfterCorner(BranchContainer branch, Vector3 oldSurfaceNormal, Vector3 newSurfaceNormal)
		{
			branch.growDirection = Vector3.Normalize(Vector3.ProjectOnPlane(-oldSurfaceNormal, newSurfaceNormal));
		}

		// Token: 0x060005A9 RID: 1449 RVA: 0x000A1528 File Offset: 0x0009F728
		private void Refine(BranchContainer branch)
		{
			if (branch.branchPoints.Count > 3)
			{
				if (Vector3.Distance(branch.branchPoints[branch.branchPoints.Count - 2].point, branch.branchPoints[branch.branchPoints.Count - 3].point) < this.infoPool.ivyParameters.stepSize * 0.7f || Vector3.Distance(branch.branchPoints[branch.branchPoints.Count - 2].point, branch.branchPoints[branch.branchPoints.Count - 1].point) < this.infoPool.ivyParameters.stepSize * 0.7f)
				{
					branch.RemoveBranchPoint(branch.branchPoints.Count - 2);
				}
				if (Vector3.Angle(branch.branchPoints[branch.branchPoints.Count - 1].point - branch.branchPoints[branch.branchPoints.Count - 2].point, branch.branchPoints[branch.branchPoints.Count - 2].point - branch.branchPoints[branch.branchPoints.Count - 3].point) > 25f)
				{
					Vector3 a = branch.branchPoints[branch.branchPoints.Count - 1].point - branch.branchPoints[branch.branchPoints.Count - 2].point;
					Vector3 a2 = branch.branchPoints[branch.branchPoints.Count - 3].point - branch.branchPoints[branch.branchPoints.Count - 2].point;
					branch.InsertBranchPoint(branch.branchPoints[branch.branchPoints.Count - 2].point + a2 / 2f, branch.branchPoints[branch.branchPoints.Count - 2].grabVector, branch.branchPoints.Count - 2);
					branch.InsertBranchPoint(branch.branchPoints[branch.branchPoints.Count - 2].point + a / 2f, branch.branchPoints[branch.branchPoints.Count - 2].grabVector, branch.branchPoints.Count - 1);
					branch.RemoveBranchPoint(branch.branchPoints.Count - 3);
				}
			}
		}

		// Token: 0x060005AA RID: 1450 RVA: 0x000A17E4 File Offset: 0x0009F9E4
		public void Optimize()
		{
			foreach (BranchContainer branchContainer in this.infoPool.ivyContainer.branches)
			{
				for (int i = 1; i < branchContainer.branchPoints.Count - 2; i++)
				{
					if (Vector3.Distance(branchContainer.branchPoints[i - 1].point, branchContainer.branchPoints[i].point) < this.infoPool.ivyParameters.stepSize * 0.7f)
					{
						branchContainer.RemoveBranchPoint(i);
					}
					if (Vector3.Angle(branchContainer.branchPoints[i - 1].point - branchContainer.branchPoints[i].point, branchContainer.branchPoints[i].point - branchContainer.branchPoints[i + 1].point) < this.infoPool.ivyParameters.optAngleBias)
					{
						branchContainer.RemoveBranchPoint(i);
					}
				}
			}
		}

		// Token: 0x04000634 RID: 1588
		public UnityEngine.Random.State randomstate;
	}
}
