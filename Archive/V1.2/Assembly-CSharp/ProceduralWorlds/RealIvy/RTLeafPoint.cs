using System;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x020000A4 RID: 164
	public class RTLeafPoint
	{
		// Token: 0x06000665 RID: 1637 RVA: 0x0004475B File Offset: 0x0004295B
		public void InitializeRuntime()
		{
		}

		// Token: 0x06000666 RID: 1638 RVA: 0x00044765 File Offset: 0x00042965
		public RTLeafPoint()
		{
		}

		// Token: 0x06000667 RID: 1639 RVA: 0x000A826C File Offset: 0x000A646C
		public RTLeafPoint(LeafPoint leafPoint, IvyParameters ivyParameters)
		{
			this.point = leafPoint.point;
			this.lpLength = leafPoint.lpLength;
			this.left = leafPoint.left;
			this.lpForward = leafPoint.lpForward;
			this.lpUpward = leafPoint.lpUpward;
			this.initSegmentIdx = leafPoint.initSegmentIdx;
			this.endSegmentIdx = leafPoint.endSegmentIdx;
			this.chosenLeave = leafPoint.chosenLeave;
			this.vertices = leafPoint.verticesLeaves.ToArray();
			this.leafCenter = leafPoint.leafCenter;
			this.leafRotation = leafPoint.leafRotation;
			this.leafScale = leafPoint.leafScale;
			this.CalculateLeafRotation(ivyParameters);
		}

		// Token: 0x06000668 RID: 1640 RVA: 0x0004767A File Offset: 0x0004587A
		public void PreInit(int numVertices)
		{
			this.vertices = new RTVertexData[numVertices];
		}

		// Token: 0x06000669 RID: 1641 RVA: 0x00047688 File Offset: 0x00045888
		public void PreInit(RTMeshData leafMeshData)
		{
			this.vertices = new RTVertexData[leafMeshData.vertices.Length];
		}

		// Token: 0x0600066A RID: 1642 RVA: 0x000A831C File Offset: 0x000A651C
		public void SetValues(Vector3 point, float lpLength, Vector3 lpForward, Vector3 lpUpward, int chosenLeave, RTBranchPoint initSegment, RTBranchPoint endSegment, float leafScale, IvyParameters ivyParameters)
		{
			this.point = point;
			this.lpLength = lpLength;
			this.lpForward = lpForward;
			this.lpUpward = lpUpward;
			this.chosenLeave = chosenLeave;
			this.initSegmentIdx = initSegment.index;
			this.endSegmentIdx = endSegment.index;
			this.leafScale = leafScale;
			this.left = Vector3.Cross(lpForward, lpUpward).normalized;
			this.CalculateLeafRotation(ivyParameters);
		}

		// Token: 0x0600066B RID: 1643 RVA: 0x000A8390 File Offset: 0x000A6590
		private void CalculateLeafRotation(IvyParameters ivyParameters)
		{
			Vector3 globalRotation;
			Vector3 axis;
			if (!ivyParameters.globalOrientation)
			{
				globalRotation = this.lpForward;
				axis = this.left;
			}
			else
			{
				globalRotation = ivyParameters.globalRotation;
				axis = Vector3.Normalize(Vector3.Cross(ivyParameters.globalRotation, this.lpUpward));
			}
			this.leafRotation = Quaternion.LookRotation(this.lpUpward, globalRotation);
			this.leafRotation = Quaternion.AngleAxis(ivyParameters.rotation.x, axis) * Quaternion.AngleAxis(ivyParameters.rotation.y, this.lpUpward) * Quaternion.AngleAxis(ivyParameters.rotation.z, globalRotation) * this.leafRotation;
			this.leafRotation = Quaternion.AngleAxis(UnityEngine.Random.Range(-ivyParameters.randomRotation.x, ivyParameters.randomRotation.x), axis) * Quaternion.AngleAxis(UnityEngine.Random.Range(-ivyParameters.randomRotation.y, ivyParameters.randomRotation.y), this.lpUpward) * Quaternion.AngleAxis(UnityEngine.Random.Range(-ivyParameters.randomRotation.z, ivyParameters.randomRotation.z), globalRotation) * this.leafRotation;
		}

		// Token: 0x0600066C RID: 1644 RVA: 0x000A84C0 File Offset: 0x000A66C0
		public void CreateVertices(IvyParameters ivyParameters, RTMeshData leafMeshData, GameObject ivyGO)
		{
			Vector3 vector = Vector3.zero;
			Vector3 normal = Vector3.zero;
			Vector2 uv = Vector2.zero;
			Color color = Color.black;
			Quaternion rotation = Quaternion.Inverse(ivyGO.transform.rotation);
			this.leafCenter = ivyGO.transform.InverseTransformPoint(this.point);
			for (int i = 0; i < leafMeshData.vertices.Length; i++)
			{
				Vector3 b = this.left * ivyParameters.offset.x + this.lpUpward * ivyParameters.offset.y + this.lpForward * ivyParameters.offset.z;
				vector = this.leafRotation * leafMeshData.vertices[i] * this.leafScale + this.point + b;
				vector -= ivyGO.transform.position;
				vector = rotation * vector;
				normal = this.leafRotation * leafMeshData.normals[i];
				normal = rotation * normal;
				uv = leafMeshData.uv[i];
				color = leafMeshData.colors[i];
				this.vertices[i] = new RTVertexData(vector, normal, uv, Vector2.zero, color);
			}
		}

		// Token: 0x04000757 RID: 1879
		public Vector3 point;

		// Token: 0x04000758 RID: 1880
		public float lpLength;

		// Token: 0x04000759 RID: 1881
		public Vector3 left;

		// Token: 0x0400075A RID: 1882
		public Vector3 lpForward;

		// Token: 0x0400075B RID: 1883
		public Vector3 lpUpward;

		// Token: 0x0400075C RID: 1884
		public int initSegmentIdx;

		// Token: 0x0400075D RID: 1885
		public int endSegmentIdx;

		// Token: 0x0400075E RID: 1886
		public int chosenLeave;

		// Token: 0x0400075F RID: 1887
		public RTVertexData[] vertices;

		// Token: 0x04000760 RID: 1888
		public Vector3 leafCenter;

		// Token: 0x04000761 RID: 1889
		public Quaternion leafRotation;

		// Token: 0x04000762 RID: 1890
		public float leafScale;
	}
}
