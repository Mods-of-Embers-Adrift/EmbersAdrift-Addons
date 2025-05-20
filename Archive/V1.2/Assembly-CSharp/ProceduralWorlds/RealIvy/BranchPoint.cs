using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x02000088 RID: 136
	[Serializable]
	public class BranchPoint
	{
		// Token: 0x06000587 RID: 1415 RVA: 0x000A04B8 File Offset: 0x0009E6B8
		public void SetValues(Vector3 point, Vector3 grabVector, Vector2 pointSS, BranchContainer branchContainer, int index, bool blocked, bool newBranch, int newBranchNumber, float length)
		{
			this.point = point;
			this.grabVector = grabVector;
			this.pointSS = pointSS;
			this.branchContainer = branchContainer;
			this.index = index;
			this.newBranch = newBranch;
			this.newBranchNumber = newBranchNumber;
			this.radius = 1f;
			this.currentRadius = 1f;
			this.length = length;
			this.initialGrowDir = Vector3.zero;
			if (index >= 1)
			{
				this.initialGrowDir = (point - branchContainer.branchPoints[index - 1].point).normalized;
			}
		}

		// Token: 0x06000588 RID: 1416 RVA: 0x00044765 File Offset: 0x00042965
		public BranchPoint()
		{
		}

		// Token: 0x06000589 RID: 1417 RVA: 0x00046E2C File Offset: 0x0004502C
		public void InitializeRuntime(IvyParameters ivyParameters)
		{
			this.verticesLoop = new List<RTVertexData>(ivyParameters.sides + 1);
		}

		// Token: 0x0600058A RID: 1418 RVA: 0x000A0554 File Offset: 0x0009E754
		public BranchPoint(Vector3 point, Vector3 grabVector, int index, bool newBranch, int newBranchNumber, float length, BranchContainer branchContainer)
		{
			this.SetValues(point, grabVector, Vector3.zero, branchContainer, index, false, newBranch, newBranchNumber, length);
		}

		// Token: 0x0600058B RID: 1419 RVA: 0x000A0584 File Offset: 0x0009E784
		public BranchPoint(Vector3 point, Vector3 grabVector, int index, float length, BranchContainer branchContainer)
		{
			this.SetValues(point, grabVector, Vector3.zero, branchContainer, index, false, false, -1, length);
		}

		// Token: 0x0600058C RID: 1420 RVA: 0x000A05B4 File Offset: 0x0009E7B4
		public BranchPoint(Vector3 point, int index, float length, BranchContainer branchContainer)
		{
			this.SetValues(point, Vector3.zero, Vector3.zero, branchContainer, index, false, false, -1, length);
		}

		// Token: 0x0600058D RID: 1421 RVA: 0x00046E41 File Offset: 0x00045041
		public void SetOriginalPoint()
		{
			this.originalPoint = this.point;
		}

		// Token: 0x0600058E RID: 1422 RVA: 0x000A05E4 File Offset: 0x0009E7E4
		public BranchPoint GetNextPoint()
		{
			BranchPoint result = null;
			if (this.index < this.branchContainer.branchPoints.Count - 1)
			{
				result = this.branchContainer.branchPoints[this.index + 1];
			}
			return result;
		}

		// Token: 0x0600058F RID: 1423 RVA: 0x000A0628 File Offset: 0x0009E828
		public BranchPoint GetPreviousPoint()
		{
			BranchPoint result = null;
			if (this.index > 0)
			{
				result = this.branchContainer.branchPoints[this.index - 1];
			}
			return result;
		}

		// Token: 0x06000590 RID: 1424 RVA: 0x00046E4F File Offset: 0x0004504F
		public void Move(Vector3 newPosition)
		{
			this.point = newPosition;
		}

		// Token: 0x06000591 RID: 1425 RVA: 0x00046E58 File Offset: 0x00045058
		public void InitBranchInThisPoint(int branchNumber)
		{
			this.newBranch = true;
			this.newBranchNumber = branchNumber;
		}

		// Token: 0x06000592 RID: 1426 RVA: 0x00046E68 File Offset: 0x00045068
		public void ReleasePoint()
		{
			this.newBranch = false;
			this.newBranchNumber = -1;
		}

		// Token: 0x0400060E RID: 1550
		public Vector3 originalPoint;

		// Token: 0x0400060F RID: 1551
		public Vector3 point;

		// Token: 0x04000610 RID: 1552
		public Vector3 grabVector;

		// Token: 0x04000611 RID: 1553
		public Vector2 pointSS;

		// Token: 0x04000612 RID: 1554
		public float length;

		// Token: 0x04000613 RID: 1555
		public Vector3 initialGrowDir;

		// Token: 0x04000614 RID: 1556
		public BranchContainer branchContainer;

		// Token: 0x04000615 RID: 1557
		public int index;

		// Token: 0x04000616 RID: 1558
		public bool newBranch;

		// Token: 0x04000617 RID: 1559
		public int newBranchNumber;

		// Token: 0x04000618 RID: 1560
		public float radius;

		// Token: 0x04000619 RID: 1561
		public float currentRadius;

		// Token: 0x0400061A RID: 1562
		public Quaternion forwardRotation;

		// Token: 0x0400061B RID: 1563
		public List<RTVertexData> verticesLoop;

		// Token: 0x0400061C RID: 1564
		public Vector3 firstVector;

		// Token: 0x0400061D RID: 1565
		public Vector3 axis;
	}
}
