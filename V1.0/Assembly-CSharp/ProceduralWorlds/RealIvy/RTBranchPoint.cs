using System;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x020000A2 RID: 162
	public class RTBranchPoint
	{
		// Token: 0x06000654 RID: 1620 RVA: 0x00044765 File Offset: 0x00042965
		public RTBranchPoint()
		{
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x000A7F34 File Offset: 0x000A6134
		public RTBranchPoint(BranchPoint branchPoint, RTBranchContainer rtBranchContainer)
		{
			this.point = branchPoint.point;
			this.grabVector = branchPoint.grabVector;
			this.length = branchPoint.length;
			this.index = branchPoint.index;
			this.newBranch = branchPoint.newBranch;
			this.newBranchNumber = branchPoint.newBranchNumber;
			this.branchContainer = rtBranchContainer;
			this.radius = branchPoint.radius;
			this.firstVector = branchPoint.firstVector;
			this.axis = branchPoint.axis;
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x00047583 File Offset: 0x00045783
		public void PreInit(IvyParameters ivyParameters)
		{
			this.verticesLoop = new RTVertexData[ivyParameters.sides + 1];
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x00047598 File Offset: 0x00045798
		public void SetValues(Vector3 point, Vector3 grabVector)
		{
			this.SetValues(point, grabVector, false, -1);
		}

		// Token: 0x06000658 RID: 1624 RVA: 0x000475A4 File Offset: 0x000457A4
		public void SetValues(Vector3 point, Vector3 grabVector, bool newBranch, int newBranchNumber)
		{
			this.point = point;
			this.grabVector = grabVector;
			this.newBranch = newBranch;
			this.newBranchNumber = newBranchNumber;
		}

		// Token: 0x06000659 RID: 1625 RVA: 0x000475C3 File Offset: 0x000457C3
		public void InitBranchInThisPoint(int branchNumber)
		{
			this.newBranch = true;
			this.newBranchNumber = branchNumber;
		}

		// Token: 0x0600065A RID: 1626 RVA: 0x000475D3 File Offset: 0x000457D3
		public void CalculateVerticesLoop(IvyParameters ivyParameters, RTIvyContainer rtIvyContainer, GameObject ivyGO, Vector3 firstVector, Vector3 axis, float radius)
		{
			this.firstVector = firstVector;
			this.axis = axis;
			this.radius = radius;
			this.CalculateVerticesLoop(ivyParameters, rtIvyContainer, ivyGO);
		}

		// Token: 0x0600065B RID: 1627 RVA: 0x000A7FBC File Offset: 0x000A61BC
		public void CalculateVerticesLoop(IvyParameters ivyParameters, RTIvyContainer rtIvyContainer, GameObject ivyGO)
		{
			float num;
			if (!ivyParameters.halfgeom)
			{
				num = 360f / (float)ivyParameters.sides;
			}
			else
			{
				num = 360f / (float)ivyParameters.sides / 2f;
			}
			Vector3 vector = Vector3.zero;
			Vector3 normal = Vector3.zero;
			Vector2 zero = Vector2.zero;
			Quaternion identity = Quaternion.identity;
			Vector3 vector2 = Vector3.zero;
			Quaternion rotation = Quaternion.Inverse(ivyGO.transform.rotation);
			for (int i = 0; i < ivyParameters.sides + 1; i++)
			{
				vector2 = Quaternion.AngleAxis(num * (float)i, this.axis) * this.firstVector;
				if (ivyParameters.halfgeom && ivyParameters.sides == 1)
				{
					normal = -this.grabVector;
				}
				else
				{
					normal = vector2;
				}
				normal = rotation * normal;
				vector = vector2 * this.radius + this.point;
				vector -= ivyGO.transform.position;
				vector = rotation * vector;
				zero = new Vector2(this.length * ivyParameters.uvScale.y + ivyParameters.uvOffset.y - ivyParameters.stepSize, 1f / (float)ivyParameters.sides * (float)i * ivyParameters.uvScale.x + ivyParameters.uvOffset.x);
				this.verticesLoop[i] = new RTVertexData(vector, normal, zero, Vector2.zero, Color.black);
			}
		}

		// Token: 0x0600065C RID: 1628 RVA: 0x000A813C File Offset: 0x000A633C
		public void CalculateCenterLoop(GameObject ivyGO)
		{
			this.centerLoop = Quaternion.Inverse(ivyGO.transform.rotation) * (this.point - ivyGO.transform.position);
			this.lastVectorNormal = ivyGO.transform.InverseTransformVector(this.grabVector);
		}

		// Token: 0x0600065D RID: 1629 RVA: 0x000475F6 File Offset: 0x000457F6
		public RTBranchPoint GetNextPoint()
		{
			return this.branchContainer.branchPoints[this.index + 1];
		}

		// Token: 0x0600065E RID: 1630 RVA: 0x00047610 File Offset: 0x00045810
		public RTBranchPoint GetPreviousPoint()
		{
			return this.branchContainer.branchPoints[this.index - 1];
		}

		// Token: 0x04000747 RID: 1863
		public Vector3 point;

		// Token: 0x04000748 RID: 1864
		public Vector3 grabVector;

		// Token: 0x04000749 RID: 1865
		public float length;

		// Token: 0x0400074A RID: 1866
		public int index;

		// Token: 0x0400074B RID: 1867
		public bool newBranch;

		// Token: 0x0400074C RID: 1868
		public int newBranchNumber;

		// Token: 0x0400074D RID: 1869
		public float radius;

		// Token: 0x0400074E RID: 1870
		public Vector3 firstVector;

		// Token: 0x0400074F RID: 1871
		public Vector3 axis;

		// Token: 0x04000750 RID: 1872
		public Vector3 centerLoop;

		// Token: 0x04000751 RID: 1873
		public RTBranchContainer branchContainer;

		// Token: 0x04000752 RID: 1874
		public RTVertexData[] verticesLoop;

		// Token: 0x04000753 RID: 1875
		public Vector3 lastVectorNormal;
	}
}
