using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x02000087 RID: 135
	[Serializable]
	public class BranchContainer : ScriptableObject
	{
		// Token: 0x06000566 RID: 1382 RVA: 0x00046D4D File Offset: 0x00044F4D
		public int GetNumLeaves()
		{
			return this.leaves.Count;
		}

		// Token: 0x06000567 RID: 1383 RVA: 0x0009FA80 File Offset: 0x0009DC80
		public void SetValues(Vector3 growDirection, float randomizeHeight, float currentHeight, float heightParameter, int branchSense, BranchPoint originPointOfThisBranch)
		{
			this.branchPoints = new List<BranchPoint>(1000);
			this.growDirection = growDirection;
			this.leaves = new List<LeafPoint>(1000);
			this.totalLenght = 0f;
			this.fallIteration = 0f;
			this.falling = false;
			this.rotationOnFallIteration = Quaternion.identity;
			this.branchSense = branchSense;
			this.heightParameter = heightParameter;
			this.randomizeHeight = randomizeHeight;
			this.heightVar = 0f;
			this.currentHeight = currentHeight;
			this.deltaHeight = 0f;
			this.newHeight = 0f;
			this.originPointOfThisBranch = originPointOfThisBranch;
			this.branchNumber = -1;
		}

		// Token: 0x06000568 RID: 1384 RVA: 0x00046D5A File Offset: 0x00044F5A
		public void AddBranchPoint(BranchPoint branchPoint, float length, float stepSize)
		{
			branchPoint.branchContainer = this;
			this.branchPoints.Add(branchPoint);
		}

		// Token: 0x06000569 RID: 1385 RVA: 0x00046D6F File Offset: 0x00044F6F
		public void Init(int branchPointsSize, int numLeaves)
		{
			this.branchPoints = new List<BranchPoint>(branchPointsSize * 2);
			this.leaves = new List<LeafPoint>(numLeaves * 2);
		}

		// Token: 0x0600056A RID: 1386 RVA: 0x00046D8D File Offset: 0x00044F8D
		public void Init()
		{
			this.Init(0, 0);
		}

		// Token: 0x0600056B RID: 1387 RVA: 0x0009FB2C File Offset: 0x0009DD2C
		public void PrepareRTLeavesDict()
		{
			this.dictRTLeavesByInitSegment = new Dictionary<int, List<LeafPoint>>();
			for (int i = 0; i < this.branchPoints.Count; i++)
			{
				List<LeafPoint> list = new List<LeafPoint>();
				this.GetLeavesInSegment(this.branchPoints[i], list);
				this.dictRTLeavesByInitSegment[i] = list;
			}
		}

		// Token: 0x0600056C RID: 1388 RVA: 0x0009FB80 File Offset: 0x0009DD80
		public void UpdateLeavesDictEntry(int initSegmentIdx, LeafPoint leaf)
		{
			if (this.dictRTLeavesByInitSegment.ContainsKey(initSegmentIdx))
			{
				this.dictRTLeavesByInitSegment[initSegmentIdx].Add(leaf);
				return;
			}
			List<LeafPoint> list = new List<LeafPoint>();
			list.Add(leaf);
			this.dictRTLeavesByInitSegment[initSegmentIdx] = list;
		}

		// Token: 0x0600056D RID: 1389 RVA: 0x00046D97 File Offset: 0x00044F97
		public void AddBranchPoint(BranchPoint branchPoint)
		{
			branchPoint.index = this.branchPoints.Count;
			branchPoint.newBranch = false;
			branchPoint.newBranchNumber = -1;
			branchPoint.branchContainer = this;
			branchPoint.length = this.totalLenght;
			this.branchPoints.Add(branchPoint);
		}

		// Token: 0x0600056E RID: 1390 RVA: 0x00046DD7 File Offset: 0x00044FD7
		public void AddBranchPoint(Vector3 point, Vector3 grabVector)
		{
			this.AddBranchPoint(point, grabVector, false, -1);
		}

		// Token: 0x0600056F RID: 1391 RVA: 0x0009FBC8 File Offset: 0x0009DDC8
		public void AddBranchPoint(Vector3 point, Vector3 grabVector, bool isNewBranch, int newBranchIndex)
		{
			BranchPoint item = new BranchPoint(point, grabVector, this.branchPoints.Count, isNewBranch, newBranchIndex, this.totalLenght, this);
			this.branchPoints.Add(item);
		}

		// Token: 0x06000570 RID: 1392 RVA: 0x0009FC00 File Offset: 0x0009DE00
		public BranchPoint InsertBranchPoint(Vector3 point, Vector3 grabVector, int index)
		{
			float length = Mathf.Lerp(this.branchPoints[index - 1].length, this.branchPoints[index].length, 0.5f);
			BranchPoint branchPoint = new BranchPoint(point, grabVector, index, length, this);
			this.branchPoints.Insert(index, branchPoint);
			for (int i = index + 1; i < this.branchPoints.Count; i++)
			{
				this.branchPoints[i].index++;
			}
			return branchPoint;
		}

		// Token: 0x06000571 RID: 1393 RVA: 0x0009FC88 File Offset: 0x0009DE88
		public void GetLeavesInSegmentRT(int initSegmentIdx, int endSegmentIdx, List<LeafPoint> res)
		{
			for (int i = initSegmentIdx; i <= endSegmentIdx; i++)
			{
				if (this.dictRTLeavesByInitSegment.ContainsKey(i))
				{
					res.AddRange(this.dictRTLeavesByInitSegment[i]);
				}
			}
		}

		// Token: 0x06000572 RID: 1394 RVA: 0x0009FCC4 File Offset: 0x0009DEC4
		public void GetLeavesInSegment(BranchPoint initSegment, List<LeafPoint> res)
		{
			for (int i = 0; i < this.leaves.Count; i++)
			{
				if (this.leaves[i].initSegmentIdx == initSegment.index)
				{
					res.Add(this.leaves[i]);
				}
			}
		}

		// Token: 0x06000573 RID: 1395 RVA: 0x0009FD14 File Offset: 0x0009DF14
		public List<LeafPoint> GetLeavesInSegment(BranchPoint initSegment)
		{
			List<LeafPoint> list = new List<LeafPoint>();
			this.GetLeavesInSegment(initSegment, list);
			return list;
		}

		// Token: 0x06000574 RID: 1396 RVA: 0x0009FD30 File Offset: 0x0009DF30
		public LeafPoint AddRandomLeaf(Vector3 pointWS, BranchPoint initSegment, BranchPoint endSegment, int leafIndex, InfoPool infoPool)
		{
			int chosenLeave = UnityEngine.Random.Range(0, infoPool.ivyParameters.leavesPrefabs.Length);
			Vector3 initialGrowDir = initSegment.initialGrowDir;
			float lpLength = initSegment.length + Vector3.Distance(pointWS, initSegment.point);
			return this.InsertLeaf(pointWS, lpLength, initialGrowDir, -initSegment.grabVector, chosenLeave, leafIndex, initSegment, endSegment);
		}

		// Token: 0x06000575 RID: 1397 RVA: 0x0009FD88 File Offset: 0x0009DF88
		public void RepositionLeavesAfterAdd02(BranchPoint newPoint)
		{
			BranchPoint previousPoint = newPoint.GetPreviousPoint();
			BranchPoint nextPoint = newPoint.GetNextPoint();
			List<LeafPoint> list = new List<LeafPoint>();
			this.GetLeavesInSegment(previousPoint, list);
			Vector3 normalized = (newPoint.point - previousPoint.point).normalized;
			Vector3 normalized2 = (nextPoint.point - newPoint.point).normalized;
			for (int i = 0; i < list.Count; i++)
			{
				Vector3 lhs = list[i].point - this.branchPoints[list[i].initSegmentIdx].point;
				Vector3 lhs2 = list[i].point - this.branchPoints[list[i].endSegmentIdx].point;
				Vector3 vector = previousPoint.point + normalized * Vector3.Dot(lhs, normalized);
				Vector3 point = nextPoint.point + normalized2 * Vector3.Dot(lhs2, normalized2);
				if (Vector3.Dot(newPoint.point - vector, normalized) >= 0f)
				{
					list[i].SetValues(vector, list[i].lpLength, normalized, list[i].lpUpward, list[i].chosenLeave, previousPoint, newPoint);
				}
				else
				{
					list[i].SetValues(point, list[i].lpLength, normalized2, list[i].lpUpward, list[i].chosenLeave, newPoint, nextPoint);
				}
			}
		}

		// Token: 0x06000576 RID: 1398 RVA: 0x0009FF2C File Offset: 0x0009E12C
		public void RepositionLeavesAfterRemove02(BranchPoint removedPoint)
		{
			BranchPoint previousPoint = removedPoint.GetPreviousPoint();
			BranchPoint nextPoint = removedPoint.GetNextPoint();
			List<LeafPoint> leavesInSegment = this.GetLeavesInSegment(previousPoint);
			leavesInSegment.AddRange(this.GetLeavesInSegment(removedPoint));
			for (int i = 0; i < leavesInSegment.Count; i++)
			{
				Vector3 lhs = leavesInSegment[i].point - previousPoint.point;
				Vector3 normalized = (nextPoint.point - previousPoint.point).normalized;
				float d = Vector3.Dot(lhs, normalized);
				Vector3 point = previousPoint.point + normalized * d;
				leavesInSegment[i].SetValues(point, leavesInSegment[i].lpLength, previousPoint.initialGrowDir, -previousPoint.grabVector, leavesInSegment[i].chosenLeave, previousPoint, nextPoint);
			}
		}

		// Token: 0x06000577 RID: 1399 RVA: 0x000A0000 File Offset: 0x0009E200
		public void RemoveBranchPoint(int indexToRemove)
		{
			this.RepositionLeavesAfterRemove02(this.branchPoints[indexToRemove]);
			for (int i = indexToRemove + 1; i < this.branchPoints.Count; i++)
			{
				List<LeafPoint> list = new List<LeafPoint>();
				this.GetLeavesInSegment(this.branchPoints[i], list);
				for (int j = 0; j < list.Count; j++)
				{
					list[j].initSegmentIdx--;
					list[j].endSegmentIdx--;
				}
				this.branchPoints[i].index--;
			}
			this.branchPoints.RemoveAt(indexToRemove);
		}

		// Token: 0x06000578 RID: 1400 RVA: 0x000A00B0 File Offset: 0x0009E2B0
		public void RemoveRange(int index, int count)
		{
			List<LeafPoint> list = new List<LeafPoint>();
			for (int i = index; i < index + count; i++)
			{
				this.GetLeavesInSegment(this.branchPoints[i], list);
			}
			for (int j = 0; j < list.Count; j++)
			{
				this.leaves.Remove(list[j]);
			}
			for (int k = index + count; k < this.branchPoints.Count; k++)
			{
				this.branchPoints[k].index--;
			}
			this.totalLenght = this.branchPoints[index - 1].length;
			this.branchPoints.RemoveRange(index, count);
			if (this.leaves[this.leaves.Count - 1].endSegmentIdx >= this.branchPoints.Count)
			{
				this.leaves.RemoveAt(this.leaves.Count - 1);
			}
		}

		// Token: 0x06000579 RID: 1401 RVA: 0x000A01A4 File Offset: 0x0009E3A4
		public BranchPoint GetNearestPointFrom(Vector3 from)
		{
			BranchPoint result = null;
			float num = float.MaxValue;
			for (int i = 0; i < this.branchPoints.Count; i++)
			{
				float sqrMagnitude = (this.branchPoints[i].point - from).sqrMagnitude;
				if (sqrMagnitude <= num)
				{
					result = this.branchPoints[i];
					num = sqrMagnitude;
				}
			}
			return result;
		}

		// Token: 0x0600057A RID: 1402 RVA: 0x000A01A4 File Offset: 0x0009E3A4
		public BranchPoint GetNearestPointWSFrom(Vector3 from)
		{
			BranchPoint result = null;
			float num = float.MaxValue;
			for (int i = 0; i < this.branchPoints.Count; i++)
			{
				float sqrMagnitude = (this.branchPoints[i].point - from).sqrMagnitude;
				if (sqrMagnitude <= num)
				{
					result = this.branchPoints[i];
					num = sqrMagnitude;
				}
			}
			return result;
		}

		// Token: 0x0600057B RID: 1403 RVA: 0x000A0204 File Offset: 0x0009E404
		public BranchPoint GetNearestPointSSFrom(Vector2 from)
		{
			BranchPoint result = null;
			float num = float.MaxValue;
			for (int i = 0; i < this.branchPoints.Count; i++)
			{
				float sqrMagnitude = (this.branchPoints[i].pointSS - from).sqrMagnitude;
				if (sqrMagnitude <= num)
				{
					result = this.branchPoints[i];
					num = sqrMagnitude;
				}
			}
			return result;
		}

		// Token: 0x0600057C RID: 1404 RVA: 0x000A0264 File Offset: 0x0009E464
		public Vector3[] GetSegmentPoints(Vector3 worldPoint)
		{
			Vector3[] array = new Vector3[2];
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			BranchPoint nearestPointFrom = this.GetNearestPointFrom(worldPoint);
			BranchPoint nextPoint = nearestPointFrom.GetNextPoint();
			BranchPoint previousPoint = nearestPointFrom.GetPreviousPoint();
			if (nextPoint != null && previousPoint != null)
			{
				float magnitude = (worldPoint - nextPoint.point).magnitude;
				float magnitude2 = (worldPoint - previousPoint.point).magnitude;
				if (magnitude <= magnitude2)
				{
					vector = nearestPointFrom.point;
					vector2 = nextPoint.point;
				}
				else
				{
					vector = previousPoint.point;
					vector2 = nearestPointFrom.point;
				}
			}
			array[0] = vector;
			array[1] = vector2;
			return array;
		}

		// Token: 0x0600057D RID: 1405 RVA: 0x00046DE3 File Offset: 0x00044FE3
		public BranchPoint GetLastBranchPoint()
		{
			return this.branchPoints[this.branchPoints.Count - 1];
		}

		// Token: 0x0600057E RID: 1406 RVA: 0x00046DFD File Offset: 0x00044FFD
		public void AddLeaf(LeafPoint leafPoint)
		{
			this.leaves.Add(leafPoint);
		}

		// Token: 0x0600057F RID: 1407 RVA: 0x000A0304 File Offset: 0x0009E504
		public LeafPoint AddLeaf(Vector3 leafPoint, float lpLength, Vector3 lpForward, Vector3 lpUpward, int chosenLeave, BranchPoint initSegment, BranchPoint endSegment)
		{
			LeafPoint leafPoint2 = new LeafPoint(leafPoint, lpLength, lpForward, lpUpward, chosenLeave, initSegment, endSegment);
			this.leaves.Add(leafPoint2);
			return leafPoint2;
		}

		// Token: 0x06000580 RID: 1408 RVA: 0x000A0330 File Offset: 0x0009E530
		public LeafPoint InsertLeaf(Vector3 leafPoint, float lpLength, Vector3 lpForward, Vector3 lpUpward, int chosenLeave, int leafIndex, BranchPoint initSegment, BranchPoint endSegment)
		{
			LeafPoint leafPoint2 = new LeafPoint(leafPoint, lpLength, lpForward, lpUpward, chosenLeave, initSegment, endSegment);
			int index = Mathf.Clamp(leafIndex, 0, int.MaxValue);
			this.leaves.Insert(index, leafPoint2);
			return leafPoint2;
		}

		// Token: 0x06000581 RID: 1409 RVA: 0x000A036C File Offset: 0x0009E56C
		public void RemoveLeaves(List<LeafPoint> leaves)
		{
			for (int i = 0; i < leaves.Count; i++)
			{
				this.leaves.Remove(leaves[i]);
			}
		}

		// Token: 0x06000582 RID: 1410 RVA: 0x000A03A0 File Offset: 0x0009E5A0
		public void DrawLeavesVectors(List<BranchPoint> branchPointsToFilter)
		{
			for (int i = 0; i < this.leaves.Count; i++)
			{
				this.leaves[i].DrawVectors();
			}
		}

		// Token: 0x06000583 RID: 1411 RVA: 0x000A03D4 File Offset: 0x0009E5D4
		public void GetInitIdxEndIdxLeaves(int initIdxBranchPoint, float stepSize, out int initIdxLeaves, out int endIdxLeaves)
		{
			bool flag = false;
			bool flag2 = false;
			initIdxLeaves = -1;
			endIdxLeaves = -1;
			for (int i = 0; i < this.leaves.Count; i++)
			{
				if (!flag && this.leaves[i].lpLength > (float)initIdxBranchPoint * stepSize)
				{
					flag = true;
					initIdxLeaves = i;
				}
				if (!flag2 && this.leaves[i].lpLength >= this.totalLenght)
				{
					endIdxLeaves = i;
					return;
				}
			}
		}

		// Token: 0x06000584 RID: 1412 RVA: 0x00046E0B File Offset: 0x0004500B
		public void ReleasePoint(int indexPoint)
		{
			if (indexPoint < this.branchPoints.Count)
			{
				this.branchPoints[indexPoint].ReleasePoint();
			}
		}

		// Token: 0x06000585 RID: 1413 RVA: 0x000A0444 File Offset: 0x0009E644
		public void GetInitIdxEndIdxLeaves(int initIdxBranchPoint, int endIdxBranchPoint, float stepSize, out int initIdxLeaves, out int endIdxLeaves)
		{
			bool flag = false;
			bool flag2 = false;
			initIdxLeaves = -1;
			endIdxLeaves = -1;
			for (int i = 0; i < this.leaves.Count; i++)
			{
				if (!flag && this.leaves[i].lpLength >= (float)initIdxBranchPoint * stepSize)
				{
					flag = true;
					initIdxLeaves = i;
				}
				if (!flag2 && this.leaves[i].lpLength >= (float)endIdxBranchPoint * stepSize)
				{
					endIdxLeaves = i - 1;
					return;
				}
			}
		}

		// Token: 0x040005FD RID: 1533
		public List<BranchPoint> branchPoints;

		// Token: 0x040005FE RID: 1534
		public Vector3 growDirection;

		// Token: 0x040005FF RID: 1535
		public List<LeafPoint> leaves;

		// Token: 0x04000600 RID: 1536
		public float totalLenght;

		// Token: 0x04000601 RID: 1537
		public float fallIteration;

		// Token: 0x04000602 RID: 1538
		public bool falling;

		// Token: 0x04000603 RID: 1539
		public Quaternion rotationOnFallIteration;

		// Token: 0x04000604 RID: 1540
		public int branchSense;

		// Token: 0x04000605 RID: 1541
		public float heightParameter;

		// Token: 0x04000606 RID: 1542
		public float randomizeHeight;

		// Token: 0x04000607 RID: 1543
		public float heightVar;

		// Token: 0x04000608 RID: 1544
		public float currentHeight;

		// Token: 0x04000609 RID: 1545
		public float deltaHeight;

		// Token: 0x0400060A RID: 1546
		public float newHeight;

		// Token: 0x0400060B RID: 1547
		public BranchPoint originPointOfThisBranch;

		// Token: 0x0400060C RID: 1548
		public int branchNumber;

		// Token: 0x0400060D RID: 1549
		public Dictionary<int, List<LeafPoint>> dictRTLeavesByInitSegment;
	}
}
