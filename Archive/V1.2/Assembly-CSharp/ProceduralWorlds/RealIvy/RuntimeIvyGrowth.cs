using System;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x0200009F RID: 159
	public class RuntimeIvyGrowth
	{
		// Token: 0x06000623 RID: 1571 RVA: 0x000A689C File Offset: 0x000A4A9C
		public void Init(RTIvyContainer ivyContainer, IvyParameters ivyParameters, GameObject ivyGO, RTMeshData[] leavesMeshesByChosenLeaf, int numPoints, int numLeaves, int maxNumVerticesPerLeaf)
		{
			this.rtIvyContainer = ivyContainer;
			this.ivyParameters = ivyParameters;
			this.ivyGO = ivyGO;
			this.leavesMeshesByChosenLeaf = leavesMeshesByChosenLeaf;
			this.numPoints = numPoints;
			this.numLeaves = numLeaves;
			this.maxNumVerticesPerLeaf = maxNumVerticesPerLeaf;
			this.branchPointsPool = new RTBranchPoint[numPoints];
			this.branchPointPoolIndex = 0;
			for (int i = 0; i < numPoints; i++)
			{
				RTBranchPoint rtbranchPoint = new RTBranchPoint();
				rtbranchPoint.PreInit(ivyParameters);
				this.branchPointsPool[i] = rtbranchPoint;
			}
			this.leavesPool = new RTLeafPoint[numLeaves];
			this.leavesPoolIndex = 0;
			for (int j = 0; j < numLeaves; j++)
			{
				RTLeafPoint rtleafPoint = new RTLeafPoint();
				rtleafPoint.PreInit(maxNumVerticesPerLeaf);
				this.leavesPool[j] = rtleafPoint;
			}
			this.branchesPool = new RTBranchContainer[ivyParameters.maxBranchs];
			for (int k = 0; k < ivyParameters.maxBranchs; k++)
			{
				this.branchesPool[k] = new RTBranchContainer(numPoints, numLeaves);
			}
			UnityEngine.Random.InitState(Environment.TickCount);
			RTBranchContainer nextBranchContainer = this.GetNextBranchContainer();
			ivyContainer.AddBranch(nextBranchContainer);
			RTBranchPoint nextFreeBranchPoint = this.GetNextFreeBranchPoint();
			nextFreeBranchPoint.SetValues(ivyGO.transform.position, -ivyGO.transform.up, false, 0);
			nextBranchContainer.AddBranchPoint(nextFreeBranchPoint, ivyParameters.stepSize);
			this.CalculateVerticesLastPoint(nextBranchContainer);
			ivyContainer.branches[0].growDirection = Quaternion.AngleAxis(UnityEngine.Random.value * 360f, ivyGO.transform.up) * ivyGO.transform.forward;
			ivyContainer.firstVertexVector = ivyContainer.branches[0].growDirection;
			ivyContainer.branches[0].randomizeHeight = UnityEngine.Random.Range(4f, 8f);
			this.CalculateNewHeight(ivyContainer.branches[0]);
			ivyContainer.branches[0].branchSense = this.ChooseBranchSense();
			this.randomstate = UnityEngine.Random.state;
		}

		// Token: 0x06000624 RID: 1572 RVA: 0x000A6A8C File Offset: 0x000A4C8C
		private void CalculateNewHeight(RTBranchContainer branch)
		{
			branch.heightVar = (Mathf.Sin(branch.heightParameter * this.ivyParameters.DTSFrequency - 45f) + 1f) / 2f;
			branch.newHeight = Mathf.Lerp(this.ivyParameters.minDistanceToSurface, this.ivyParameters.maxDistanceToSurface, branch.heightVar);
			branch.newHeight += (Mathf.Sin(branch.heightParameter * this.ivyParameters.DTSFrequency * branch.randomizeHeight) + 1f) / 2f * this.ivyParameters.maxDistanceToSurface / 4f * this.ivyParameters.DTSRandomness;
			branch.deltaHeight = branch.currentHeight - branch.newHeight;
			branch.currentHeight = branch.newHeight;
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x00046E78 File Offset: 0x00045078
		private int ChooseBranchSense()
		{
			if (UnityEngine.Random.value < 0.5f)
			{
				return -1;
			}
			return 1;
		}

		// Token: 0x06000626 RID: 1574 RVA: 0x000A6B64 File Offset: 0x000A4D64
		public void Step()
		{
			UnityEngine.Random.state = this.randomstate;
			for (int i = 0; i < this.rtIvyContainer.branches.Count; i++)
			{
				this.rtIvyContainer.branches[i].heightParameter += this.ivyParameters.stepSize;
				this.CalculateNewPoint(this.rtIvyContainer.branches[i]);
			}
			this.randomstate = UnityEngine.Random.state;
		}

		// Token: 0x06000627 RID: 1575 RVA: 0x000473B0 File Offset: 0x000455B0
		private void CalculateNewPoint(RTBranchContainer branch)
		{
			if (!branch.falling)
			{
				this.CalculateNewHeight(branch);
				this.CheckWall(branch);
				return;
			}
			this.CheckFall(branch);
		}

		// Token: 0x06000628 RID: 1576 RVA: 0x000A6BE4 File Offset: 0x000A4DE4
		private void CheckWall(RTBranchContainer branch)
		{
			RTBranchPoint nextFreeBranchPoint = this.GetNextFreeBranchPoint();
			nextFreeBranchPoint.point = branch.GetLastBranchPoint().point + branch.growDirection * this.ivyParameters.stepSize + branch.GetLastBranchPoint().grabVector * branch.deltaHeight;
			nextFreeBranchPoint.index = branch.branchPoints.Count;
			Vector3 direction = nextFreeBranchPoint.point - branch.GetLastBranchPoint().point;
			RaycastHit raycastHit;
			if (!Physics.Raycast(new Ray(branch.branchPoints[branch.branchPoints.Count - 1].point, direction), out raycastHit, this.ivyParameters.stepSize * 1.15f, this.ivyParameters.layerMask.value))
			{
				this.CheckFloor(branch, nextFreeBranchPoint, -branch.GetLastBranchPoint().grabVector);
				return;
			}
			this.NewGrowDirectionAfterWall(branch, -branch.GetLastBranchPoint().grabVector, raycastHit.normal);
			this.AddPoint(branch, raycastHit.point, raycastHit.normal);
		}

		// Token: 0x06000629 RID: 1577 RVA: 0x000A6D00 File Offset: 0x000A4F00
		private void CheckFloor(RTBranchContainer branch, RTBranchPoint potentialPoint, Vector3 oldSurfaceNormal)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(new Ray(potentialPoint.point, -oldSurfaceNormal), out raycastHit, branch.currentHeight * 2f, this.ivyParameters.layerMask.value))
			{
				this.AddPoint(branch, raycastHit.point, raycastHit.normal);
				this.NewGrowDirection(branch);
				branch.fallIteration = 0f;
				branch.falling = false;
				return;
			}
			if (UnityEngine.Random.value < this.ivyParameters.grabProvabilityOnFall)
			{
				this.CheckCorner(branch, potentialPoint, oldSurfaceNormal);
				return;
			}
			this.AddFallingPoint(branch);
			branch.fallIteration += 1f - this.ivyParameters.stiffness;
			branch.falling = true;
			branch.currentHeight = 0f;
			branch.heightParameter = -45f;
		}

		// Token: 0x0600062A RID: 1578 RVA: 0x000A6DD0 File Offset: 0x000A4FD0
		private void CheckCorner(RTBranchContainer branch, RTBranchPoint potentialPoint, Vector3 oldSurfaceNormal)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(new Ray(potentialPoint.point + branch.branchPoints[branch.branchPoints.Count - 1].grabVector * 2f * branch.currentHeight, -branch.growDirection), out raycastHit, this.ivyParameters.stepSize * 1.15f, this.ivyParameters.layerMask.value))
			{
				this.AddPoint(branch, potentialPoint.point, oldSurfaceNormal);
				this.AddPoint(branch, raycastHit.point, raycastHit.normal);
				this.NewGrowDirectionAfterCorner(branch, oldSurfaceNormal, raycastHit.normal);
				return;
			}
			this.AddFallingPoint(branch);
			branch.fallIteration += 1f - this.ivyParameters.stiffness;
			branch.falling = true;
			branch.currentHeight = 0f;
			branch.heightParameter = -45f;
		}

		// Token: 0x0600062B RID: 1579 RVA: 0x000A6ECC File Offset: 0x000A50CC
		private void CheckFall(RTBranchContainer branch)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(new Ray(branch.branchPoints[branch.branchPoints.Count - 1].point, branch.growDirection), out raycastHit, this.ivyParameters.stepSize * 1.15f, this.ivyParameters.layerMask.value))
			{
				this.NewGrowDirectionAfterFall(branch, raycastHit.normal);
				this.AddPoint(branch, raycastHit.point, raycastHit.normal);
				branch.fallIteration = 0f;
				branch.falling = false;
				return;
			}
			if (UnityEngine.Random.value < this.ivyParameters.grabProvabilityOnFall)
			{
				this.CheckGrabPoint(branch);
				return;
			}
			this.NewGrowDirectionFalling(branch);
			this.AddFallingPoint(branch);
			branch.fallIteration += 1f - this.ivyParameters.stiffness;
			branch.falling = true;
		}

		// Token: 0x0600062C RID: 1580 RVA: 0x000A6FB0 File Offset: 0x000A51B0
		private void CheckGrabPoint(RTBranchContainer branch)
		{
			for (int i = 0; i < 6; i++)
			{
				float angle = 60f * (float)i;
				RaycastHit raycastHit;
				if (Physics.Raycast(new Ray(branch.branchPoints[branch.branchPoints.Count - 1].point + branch.growDirection * this.ivyParameters.stepSize, Quaternion.AngleAxis(angle, branch.growDirection) * branch.GetLastBranchPoint().grabVector), out raycastHit, this.ivyParameters.stepSize * 2f, this.ivyParameters.layerMask.value))
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
					branch.fallIteration += 1f - this.ivyParameters.stiffness;
					branch.falling = true;
				}
			}
		}

		// Token: 0x0600062D RID: 1581 RVA: 0x000A70C8 File Offset: 0x000A52C8
		public void AddPoint(RTBranchContainer branch, Vector3 point, Vector3 normal)
		{
			branch.totalLength += this.ivyParameters.stepSize;
			RTBranchPoint nextFreeBranchPoint = this.GetNextFreeBranchPoint();
			nextFreeBranchPoint.SetValues(point + normal * branch.currentHeight, -normal);
			branch.AddBranchPoint(nextFreeBranchPoint, this.ivyParameters.stepSize);
			this.CalculateVerticesLastPoint(branch);
			if (UnityEngine.Random.value < this.ivyParameters.branchProvability && this.rtIvyContainer.branches.Count < this.ivyParameters.maxBranchs)
			{
				this.AddBranch(branch, branch.GetLastBranchPoint(), branch.branchPoints[branch.branchPoints.Count - 1].point, normal);
			}
			if (this.ivyParameters.generateLeaves)
			{
				this.AddLeave(branch);
			}
		}

		// Token: 0x0600062E RID: 1582 RVA: 0x000A719C File Offset: 0x000A539C
		private float CalculateRadius(float lenght)
		{
			float t = (Mathf.Sin(lenght * this.ivyParameters.radiusVarFreq + this.ivyParameters.radiusVarOffset) + 1f) / 2f;
			return Mathf.Lerp(this.ivyParameters.minRadius, this.ivyParameters.maxRadius, t);
		}

		// Token: 0x0600062F RID: 1583 RVA: 0x000A71F0 File Offset: 0x000A53F0
		private float CalculateLeafScale(BranchContainer branch, LeafPoint leafPoint)
		{
			float num = UnityEngine.Random.Range(this.ivyParameters.minScale, this.ivyParameters.maxScale);
			if (leafPoint.lpLength - 0.1f >= branch.totalLenght - this.ivyParameters.tipInfluence)
			{
				num *= Mathf.InverseLerp(branch.totalLenght, branch.totalLenght - this.ivyParameters.tipInfluence, leafPoint.lpLength);
			}
			return num;
		}

		// Token: 0x06000630 RID: 1584 RVA: 0x000A7260 File Offset: 0x000A5460
		private Quaternion CalculateLeafRotation(LeafPoint leafPoint)
		{
			Vector3 vector;
			Vector3 axis;
			if (!this.ivyParameters.globalOrientation)
			{
				vector = leafPoint.lpForward;
				axis = leafPoint.left;
			}
			else
			{
				vector = this.ivyParameters.globalRotation;
				axis = Vector3.Normalize(Vector3.Cross(this.ivyParameters.globalRotation, leafPoint.lpUpward));
			}
			Quaternion rhs = Quaternion.LookRotation(leafPoint.lpUpward, vector);
			rhs = Quaternion.AngleAxis(this.ivyParameters.rotation.x, axis) * Quaternion.AngleAxis(this.ivyParameters.rotation.y, leafPoint.lpUpward) * Quaternion.AngleAxis(this.ivyParameters.rotation.z, vector) * rhs;
			return Quaternion.AngleAxis(UnityEngine.Random.Range(-this.ivyParameters.randomRotation.x, this.ivyParameters.randomRotation.x), axis) * Quaternion.AngleAxis(UnityEngine.Random.Range(-this.ivyParameters.randomRotation.y, this.ivyParameters.randomRotation.y), leafPoint.lpUpward) * Quaternion.AngleAxis(UnityEngine.Random.Range(-this.ivyParameters.randomRotation.z, this.ivyParameters.randomRotation.z), vector) * rhs;
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x000A73B4 File Offset: 0x000A55B4
		private void AddFallingPoint(RTBranchContainer branch)
		{
			Vector3 grabVector = branch.rotationOnFallIteration * branch.GetLastBranchPoint().grabVector;
			RTBranchPoint nextFreeBranchPoint = this.GetNextFreeBranchPoint();
			nextFreeBranchPoint.point = branch.branchPoints[branch.branchPoints.Count - 1].point + branch.growDirection * this.ivyParameters.stepSize;
			nextFreeBranchPoint.grabVector = grabVector;
			branch.AddBranchPoint(nextFreeBranchPoint, this.ivyParameters.stepSize);
			this.CalculateVerticesLastPoint(branch);
			if (UnityEngine.Random.value < this.ivyParameters.branchProvability && this.rtIvyContainer.branches.Count < this.ivyParameters.maxBranchs)
			{
				this.AddBranch(branch, branch.GetLastBranchPoint(), branch.branchPoints[branch.branchPoints.Count - 1].point, -branch.GetLastBranchPoint().grabVector);
			}
			if (this.ivyParameters.generateLeaves)
			{
				this.AddLeave(branch);
			}
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x000A74BC File Offset: 0x000A56BC
		private void CalculateVerticesLastPoint(RTBranchContainer rtBranchContainer)
		{
			if (rtBranchContainer.branchPoints.Count > 1)
			{
				RTBranchPoint rtbranchPoint = rtBranchContainer.branchPoints[rtBranchContainer.branchPoints.Count - 2];
				float radius = this.CalculateRadius(rtbranchPoint.length);
				Vector3 loopAxis = this.GetLoopAxis(rtbranchPoint, rtBranchContainer, this.rtIvyContainer, this.ivyGO);
				Vector3 firstVector = this.GetFirstVector(rtbranchPoint, rtBranchContainer, this.rtIvyContainer, this.ivyParameters, loopAxis);
				rtbranchPoint.CalculateCenterLoop(this.ivyGO);
				rtbranchPoint.CalculateVerticesLoop(this.ivyParameters, this.rtIvyContainer, this.ivyGO, firstVector, loopAxis, radius);
			}
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x000A7550 File Offset: 0x000A5750
		private void AddLeave(RTBranchContainer branch)
		{
			if (branch.branchPoints.Count % (this.ivyParameters.leaveEvery + UnityEngine.Random.Range(0, this.ivyParameters.randomLeaveEvery)) == 0)
			{
				int chosenLeave = UnityEngine.Random.Range(0, this.ivyParameters.leavesPrefabs.Length);
				RTBranchPoint rtbranchPoint = branch.branchPoints[branch.branchPoints.Count - 2];
				RTBranchPoint rtbranchPoint2 = branch.branchPoints[branch.branchPoints.Count - 1];
				Vector3 point = Vector3.Lerp(rtbranchPoint.point, rtbranchPoint2.point, 0.5f);
				float leafScale = UnityEngine.Random.Range(this.ivyParameters.minScale, this.ivyParameters.maxScale);
				RTLeafPoint nextLeafPoint = this.GetNextLeafPoint();
				nextLeafPoint.SetValues(point, branch.totalLength, branch.growDirection, -branch.GetLastBranchPoint().grabVector, chosenLeave, rtbranchPoint, rtbranchPoint2, leafScale, this.ivyParameters);
				RTMeshData leafMeshData = this.leavesMeshesByChosenLeaf[nextLeafPoint.chosenLeave];
				nextLeafPoint.CreateVertices(this.ivyParameters, leafMeshData, this.ivyGO);
				branch.AddLeaf(nextLeafPoint);
			}
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x000473D0 File Offset: 0x000455D0
		public void DeleteLastBranch()
		{
			this.rtIvyContainer.branches.RemoveAt(this.rtIvyContainer.branches.Count - 1);
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x000A766C File Offset: 0x000A586C
		public void AddBranch(RTBranchContainer branch, RTBranchPoint originBranchPoint, Vector3 point, Vector3 normal)
		{
			RTBranchContainer nextBranchContainer = this.GetNextBranchContainer();
			RTBranchPoint nextFreeBranchPoint = this.GetNextFreeBranchPoint();
			nextFreeBranchPoint.SetValues(point, -normal);
			nextBranchContainer.AddBranchPoint(nextFreeBranchPoint, this.ivyParameters.stepSize);
			nextBranchContainer.growDirection = Vector3.Normalize(Vector3.ProjectOnPlane(branch.growDirection, normal));
			nextBranchContainer.randomizeHeight = UnityEngine.Random.Range(4f, 8f);
			nextBranchContainer.currentHeight = branch.currentHeight;
			nextBranchContainer.heightParameter = branch.heightParameter;
			nextBranchContainer.branchSense = this.ChooseBranchSense();
			this.rtIvyContainer.AddBranch(nextBranchContainer);
			originBranchPoint.InitBranchInThisPoint(nextBranchContainer.branchNumber);
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x000A7710 File Offset: 0x000A5910
		private void NewGrowDirection(RTBranchContainer branch)
		{
			branch.growDirection = Vector3.Normalize(Vector3.ProjectOnPlane(Quaternion.AngleAxis(Mathf.Sin((float)branch.branchSense * branch.totalLength * this.ivyParameters.directionFrequency * (1f + UnityEngine.Random.Range(-this.ivyParameters.directionRandomness, this.ivyParameters.directionRandomness))) * this.ivyParameters.directionAmplitude * this.ivyParameters.stepSize * 10f * Mathf.Max(this.ivyParameters.directionRandomness, 1f), branch.GetLastBranchPoint().grabVector) * branch.growDirection, branch.GetLastBranchPoint().grabVector));
		}

		// Token: 0x06000637 RID: 1591 RVA: 0x000473F4 File Offset: 0x000455F4
		private void NewGrowDirectionAfterWall(RTBranchContainer branch, Vector3 oldSurfaceNormal, Vector3 newSurfaceNormal)
		{
			branch.growDirection = Vector3.Normalize(Vector3.ProjectOnPlane(oldSurfaceNormal, newSurfaceNormal));
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x000A77CC File Offset: 0x000A59CC
		private void NewGrowDirectionFalling(RTBranchContainer branch)
		{
			Vector3 vector = Vector3.Lerp(branch.growDirection, this.ivyParameters.gravity, branch.fallIteration / 10f);
			vector = Quaternion.AngleAxis(Mathf.Sin((float)branch.branchSense * branch.totalLength * this.ivyParameters.directionFrequency * (1f + UnityEngine.Random.Range(-this.ivyParameters.directionRandomness / 8f, this.ivyParameters.directionRandomness / 8f))) * this.ivyParameters.directionAmplitude * this.ivyParameters.stepSize * 5f * Mathf.Max(this.ivyParameters.directionRandomness / 8f, 1f), branch.GetLastBranchPoint().grabVector) * vector;
			vector = Quaternion.AngleAxis(Mathf.Sin((float)branch.branchSense * branch.totalLength * this.ivyParameters.directionFrequency / 2f * (1f + UnityEngine.Random.Range(-this.ivyParameters.directionRandomness / 8f, this.ivyParameters.directionRandomness / 8f))) * this.ivyParameters.directionAmplitude * this.ivyParameters.stepSize * 5f * Mathf.Max(this.ivyParameters.directionRandomness / 8f, 1f), Vector3.Cross(branch.GetLastBranchPoint().grabVector, branch.growDirection)) * vector;
			branch.rotationOnFallIteration = Quaternion.FromToRotation(branch.growDirection, vector);
			branch.growDirection = vector;
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x00047408 File Offset: 0x00045608
		private void NewGrowDirectionAfterFall(RTBranchContainer branch, Vector3 newSurfaceNormal)
		{
			branch.growDirection = Vector3.Normalize(Vector3.ProjectOnPlane(-branch.GetLastBranchPoint().grabVector, newSurfaceNormal));
		}

		// Token: 0x0600063A RID: 1594 RVA: 0x0004742B File Offset: 0x0004562B
		private void NewGrowDirectionAfterGrab(RTBranchContainer branch, Vector3 newSurfaceNormal)
		{
			branch.growDirection = Vector3.Normalize(Vector3.ProjectOnPlane(branch.growDirection, newSurfaceNormal));
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x00047444 File Offset: 0x00045644
		private void NewGrowDirectionAfterCorner(RTBranchContainer branch, Vector3 oldSurfaceNormal, Vector3 newSurfaceNormal)
		{
			branch.growDirection = Vector3.Normalize(Vector3.ProjectOnPlane(-oldSurfaceNormal, newSurfaceNormal));
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x000A7964 File Offset: 0x000A5B64
		public Vector3 GetFirstVector(RTBranchPoint rtBranchPoint, RTBranchContainer rtBranchContainer, RTIvyContainer rtIvyContainer, IvyParameters ivyParameters, Vector3 axis)
		{
			Vector3 result = Vector3.zero;
			if (rtBranchContainer.branchNumber == 0 && rtBranchPoint.index == 0)
			{
				if (!ivyParameters.halfgeom)
				{
					result = rtIvyContainer.firstVertexVector;
				}
				else
				{
					result = Quaternion.AngleAxis(90f, axis) * rtIvyContainer.firstVertexVector;
				}
			}
			else if (!ivyParameters.halfgeom)
			{
				result = Vector3.Normalize(Vector3.ProjectOnPlane(rtBranchPoint.grabVector, axis));
			}
			else
			{
				result = Quaternion.AngleAxis(90f, axis) * Vector3.Normalize(Vector3.ProjectOnPlane(rtBranchPoint.grabVector, axis));
			}
			return result;
		}

		// Token: 0x0600063D RID: 1597 RVA: 0x000A79F8 File Offset: 0x000A5BF8
		public Vector3 GetLoopAxis(RTBranchPoint rtBranchPoint, RTBranchContainer rtBranchContainer, RTIvyContainer rtIvyContainer, GameObject ivyGo)
		{
			Vector3 result = Vector3.zero;
			if (rtBranchPoint.index == 0 && rtBranchContainer.branchNumber == 0)
			{
				result = ivyGo.transform.up;
			}
			else if (rtBranchPoint.index == 0)
			{
				result = rtBranchPoint.GetNextPoint().point - rtBranchPoint.point;
			}
			else
			{
				result = Vector3.Normalize(Vector3.Lerp(rtBranchPoint.point - rtBranchPoint.GetPreviousPoint().point, rtBranchPoint.GetNextPoint().point - rtBranchPoint.point, 0.5f));
			}
			return result;
		}

		// Token: 0x0600063E RID: 1598 RVA: 0x000A7A88 File Offset: 0x000A5C88
		private RTBranchPoint GetNextFreeBranchPoint()
		{
			RTBranchPoint result = this.branchPointsPool[this.branchPointPoolIndex];
			this.branchPointPoolIndex++;
			if (this.branchPointPoolIndex >= this.branchPointsPool.Length)
			{
				Array.Resize<RTBranchPoint>(ref this.branchPointsPool, this.branchPointsPool.Length * 2);
				for (int i = this.branchPointPoolIndex; i < this.branchPointsPool.Length; i++)
				{
					RTBranchPoint rtbranchPoint = new RTBranchPoint();
					rtbranchPoint.PreInit(this.ivyParameters);
					this.branchPointsPool[i] = rtbranchPoint;
				}
			}
			return result;
		}

		// Token: 0x0600063F RID: 1599 RVA: 0x000A7B0C File Offset: 0x000A5D0C
		private RTLeafPoint GetNextLeafPoint()
		{
			RTLeafPoint result = this.leavesPool[this.leavesPoolIndex];
			this.leavesPoolIndex++;
			if (this.leavesPoolIndex >= this.leavesPool.Length)
			{
				Array.Resize<RTLeafPoint>(ref this.leavesPool, this.leavesPool.Length * 2);
				for (int i = this.leavesPoolIndex; i < this.leavesPool.Length; i++)
				{
					this.leavesPool[i] = new RTLeafPoint();
					this.leavesPool[i].PreInit(this.maxNumVerticesPerLeaf);
				}
			}
			return result;
		}

		// Token: 0x06000640 RID: 1600 RVA: 0x0004745D File Offset: 0x0004565D
		private RTBranchContainer GetNextBranchContainer()
		{
			RTBranchContainer result = this.branchesPool[this.branchesPoolIndex];
			this.branchesPoolIndex++;
			return result;
		}

		// Token: 0x04000729 RID: 1833
		private RTIvyContainer rtIvyContainer;

		// Token: 0x0400072A RID: 1834
		private IvyParameters ivyParameters;

		// Token: 0x0400072B RID: 1835
		private GameObject ivyGO;

		// Token: 0x0400072C RID: 1836
		private RTMeshData[] leavesMeshesByChosenLeaf;

		// Token: 0x0400072D RID: 1837
		private RTBranchContainer[] branchesPool;

		// Token: 0x0400072E RID: 1838
		private int branchesPoolIndex;

		// Token: 0x0400072F RID: 1839
		private RTBranchPoint[] branchPointsPool;

		// Token: 0x04000730 RID: 1840
		private int branchPointPoolIndex;

		// Token: 0x04000731 RID: 1841
		private RTLeafPoint[] leavesPool;

		// Token: 0x04000732 RID: 1842
		private int leavesPoolIndex;

		// Token: 0x04000733 RID: 1843
		private int numPoints;

		// Token: 0x04000734 RID: 1844
		private int numLeaves;

		// Token: 0x04000735 RID: 1845
		private int maxNumVerticesPerLeaf;

		// Token: 0x04000736 RID: 1846
		public UnityEngine.Random.State randomstate;
	}
}
