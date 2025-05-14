using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x02000097 RID: 151
	[Serializable]
	public class LeafPoint
	{
		// Token: 0x060005D6 RID: 1494 RVA: 0x0004709A File Offset: 0x0004529A
		public void InitializeRuntime()
		{
			this.verticesLeaves = new List<RTVertexData>(4);
		}

		// Token: 0x060005D7 RID: 1495 RVA: 0x00044765 File Offset: 0x00042965
		public LeafPoint()
		{
		}

		// Token: 0x060005D8 RID: 1496 RVA: 0x000470A8 File Offset: 0x000452A8
		public LeafPoint(Vector3 point, float lpLength, Vector3 lpForward, Vector3 lpUpward, int chosenLeave, BranchPoint initSegment, BranchPoint endSegment)
		{
			this.SetValues(point, lpLength, lpForward, lpUpward, chosenLeave, initSegment, endSegment);
		}

		// Token: 0x060005D9 RID: 1497 RVA: 0x000A466C File Offset: 0x000A286C
		public void SetValues(Vector3 point, float lpLength, Vector3 lpForward, Vector3 lpUpward, int chosenLeave, BranchPoint initSegment, BranchPoint endSegment)
		{
			this.point = point;
			this.lpLength = lpLength;
			this.lpForward = lpForward;
			this.lpUpward = lpUpward;
			this.chosenLeave = chosenLeave;
			this.initSegmentIdx = initSegment.index;
			this.endSegmentIdx = endSegment.index;
			this.forwarRot = Quaternion.identity;
			float magnitude = (initSegment.point - endSegment.point).magnitude;
			float value = (point - initSegment.point).magnitude / magnitude;
			this.displacementFromInitSegment = Mathf.Clamp(value, 0.01f, 0.99f);
			this.left = Vector3.Cross(lpForward, lpUpward).normalized;
		}

		// Token: 0x060005DA RID: 1498 RVA: 0x000A4724 File Offset: 0x000A2924
		public void DrawVectors()
		{
			Debug.DrawLine(this.point, this.point + this.lpForward * 0.25f, Color.red, 5f);
			Debug.DrawLine(this.point, this.point + this.lpUpward * 0.25f, Color.blue, 5f);
			Debug.DrawLine(this.point, this.point + this.left * 0.25f, Color.green, 5f);
		}

		// Token: 0x060005DB RID: 1499 RVA: 0x000470C1 File Offset: 0x000452C1
		public float GetLengthFactor(BranchContainer branchContainer, float correctionFactor)
		{
			if (this.lpLength > branchContainer.totalLenght * 1.15f * correctionFactor)
			{
				return 0f;
			}
			return 1f;
		}

		// Token: 0x060005DC RID: 1500 RVA: 0x000A47C4 File Offset: 0x000A29C4
		public void CreateVertices(IvyParameters ivyParameters, RTMeshData leafMeshData, GameObject ivyGO)
		{
			Vector3 globalRotation;
			Vector3 vector;
			if (!ivyParameters.globalOrientation)
			{
				globalRotation = this.lpForward;
				vector = this.left;
			}
			else
			{
				globalRotation = ivyParameters.globalRotation;
				vector = Vector3.Normalize(Vector3.Cross(ivyParameters.globalRotation, this.lpUpward));
			}
			Quaternion quaternion = Quaternion.LookRotation(this.lpUpward, globalRotation);
			quaternion = Quaternion.AngleAxis(ivyParameters.rotation.x, vector) * Quaternion.AngleAxis(ivyParameters.rotation.y, this.lpUpward) * Quaternion.AngleAxis(ivyParameters.rotation.z, globalRotation) * quaternion;
			quaternion = Quaternion.AngleAxis(UnityEngine.Random.Range(-ivyParameters.randomRotation.x, ivyParameters.randomRotation.x), vector) * Quaternion.AngleAxis(UnityEngine.Random.Range(-ivyParameters.randomRotation.y, ivyParameters.randomRotation.y), this.lpUpward) * Quaternion.AngleAxis(UnityEngine.Random.Range(-ivyParameters.randomRotation.z, ivyParameters.randomRotation.z), globalRotation) * quaternion;
			quaternion = this.forwarRot * quaternion;
			float d = UnityEngine.Random.Range(ivyParameters.minScale, ivyParameters.maxScale);
			this.leafRotation = quaternion;
			this.leafCenter = this.point - ivyGO.transform.position;
			this.leafCenter = Quaternion.Inverse(ivyGO.transform.rotation) * this.leafCenter;
			if (this.verticesLeaves == null)
			{
				this.verticesLeaves = new List<RTVertexData>(4);
			}
			Vector3 vertex = Vector3.zero;
			Vector3 normal = Vector3.zero;
			Vector2 uv = Vector2.zero;
			Color color = Color.black;
			Quaternion rotation = Quaternion.Inverse(ivyGO.transform.rotation);
			for (int i = 0; i < leafMeshData.vertices.Length; i++)
			{
				Vector3 b = vector * ivyParameters.offset.x + this.lpUpward * ivyParameters.offset.y + this.lpForward * ivyParameters.offset.z;
				vertex = quaternion * leafMeshData.vertices[i] * d + this.leafCenter + b;
				normal = quaternion * leafMeshData.normals[i];
				normal = rotation * normal;
				uv = leafMeshData.uv[i];
				color = leafMeshData.colors[i];
				RTVertexData item = new RTVertexData(vertex, normal, uv, Vector2.zero, color);
				this.verticesLeaves.Add(item);
			}
		}

		// Token: 0x040006B5 RID: 1717
		public Vector3 point;

		// Token: 0x040006B6 RID: 1718
		public Vector2 pointSS;

		// Token: 0x040006B7 RID: 1719
		public float lpLength;

		// Token: 0x040006B8 RID: 1720
		public Vector3 left;

		// Token: 0x040006B9 RID: 1721
		public Vector3 lpForward;

		// Token: 0x040006BA RID: 1722
		public Vector3 lpUpward;

		// Token: 0x040006BB RID: 1723
		public int chosenLeave;

		// Token: 0x040006BC RID: 1724
		public Quaternion forwarRot;

		// Token: 0x040006BD RID: 1725
		public int initSegmentIdx;

		// Token: 0x040006BE RID: 1726
		public int endSegmentIdx;

		// Token: 0x040006BF RID: 1727
		public float displacementFromInitSegment;

		// Token: 0x040006C0 RID: 1728
		public Quaternion leafRotation;

		// Token: 0x040006C1 RID: 1729
		public float currentScale;

		// Token: 0x040006C2 RID: 1730
		public float dstScale;

		// Token: 0x040006C3 RID: 1731
		public Vector3 leafCenter;

		// Token: 0x040006C4 RID: 1732
		public List<RTVertexData> verticesLeaves;

		// Token: 0x040006C5 RID: 1733
		public float leafScale;
	}
}
