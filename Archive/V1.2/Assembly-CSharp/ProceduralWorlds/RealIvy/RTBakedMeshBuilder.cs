using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x0200009A RID: 154
	public class RTBakedMeshBuilder
	{
		// Token: 0x060005DE RID: 1502 RVA: 0x000A4B78 File Offset: 0x000A2D78
		public void InitializeMeshBuilder(IvyParameters ivyParameters, RTIvyContainer ivyContainer, RTIvyContainer bakedIvyContainer, GameObject ivyGO, Mesh bakedMesh, MeshRenderer meshRenderer, MeshFilter meshFilter, int numBranches, Mesh processedMesh, float growSpeed, MeshRenderer mrProcessedMesh, int backtrackingPoints, int[] submeshByChoseLeaf, RTMeshData[] leavesMeshesByChosenLeaf, Material[] materials)
		{
			this.ivyParameters = ivyParameters;
			this.rtIvyContainer = ivyContainer;
			this.rtBakedIvyContainer = bakedIvyContainer;
			this.ivyGO = ivyGO;
			this.meshRenderer = meshRenderer;
			this.meshFilter = meshFilter;
			this.processedMesh = processedMesh;
			this.processedMesh.indexFormat = IndexFormat.UInt16;
			this.mrProcessedMesh = mrProcessedMesh;
			this.submeshByChoseLeaf = submeshByChoseLeaf;
			this.leavesMeshesByChosenLeaf = leavesMeshesByChosenLeaf;
			this.activeBranches = new List<RTBranchContainer>();
			this.fromTo = new int[2];
			this.vectors = new Vector3[2];
			this.growthSpeed = growSpeed;
			this.backtrackingPoints = backtrackingPoints;
			this.submeshCount = meshRenderer.sharedMaterials.Length;
			this.vertCountsPerBranch = new int[numBranches];
			this.lastTriangleIndexPerBranch = new int[numBranches];
			this.vertCountLeavesPerBranch = new int[numBranches];
			this.processedVerticesIndicesPerBranch = new List<List<int>>(numBranches);
			this.processedBranchesVerticesIndicesPerBranch = new List<List<int>>(numBranches);
			for (int i = 0; i < numBranches; i++)
			{
				this.processedVerticesIndicesPerBranch.Add(new List<int>());
				this.processedBranchesVerticesIndicesPerBranch.Add(new List<int>());
			}
			this.vertCount = 0;
			this.ivyMesh = new Mesh();
			this.ivyMesh.subMeshCount = this.submeshCount;
			this.ivyMesh.name = "IvyMesh";
			meshFilter.mesh = this.ivyMesh;
			ivyGO.GetComponent<MeshRenderer>().sharedMaterials = materials;
			mrProcessedMesh.sharedMaterials = materials;
			this.leavesDataInitialized = true;
			this.ivyGoPosition = ivyGO.transform.position;
			this.ivyGoRotation = ivyGO.transform.rotation;
			this.ivyGoInverseRotation = Quaternion.Inverse(ivyGO.transform.rotation);
			this.zeroVector3 = Vector3.zero;
			this.zeroVector2 = Vector2.zero;
			this.blackColor = Color.black;
		}

		// Token: 0x060005DF RID: 1503 RVA: 0x000470E4 File Offset: 0x000452E4
		public void InitializeMeshesDataBaked(Mesh bakedMesh, int numBranches)
		{
			this.CreateBuildingMeshData(bakedMesh, numBranches);
			this.CreateBakedMeshData(bakedMesh);
			this.CreateProcessedMeshData(bakedMesh);
			bakedMesh.Clear();
		}

		// Token: 0x060005E0 RID: 1504 RVA: 0x00047102 File Offset: 0x00045302
		public void InitializeMeshesDataProcedural(Mesh bakedMesh, int numBranches, float lifetime, float velocity)
		{
			this.CreateBuildingMeshData(bakedMesh, numBranches);
			this.CreateBakedMeshData(bakedMesh);
			this.CreateProcessedMeshDataProcedural(bakedMesh, lifetime, velocity);
			bakedMesh.Clear();
		}

		// Token: 0x060005E1 RID: 1505 RVA: 0x000A4D48 File Offset: 0x000A2F48
		public void CreateBuildingMeshData(Mesh bakedMesh, int numBranches)
		{
			int num = this.ivyParameters.sides + 1;
			int num2 = this.backtrackingPoints * num + this.backtrackingPoints * 2 * 8;
			num2 *= numBranches;
			int subMeshCount = bakedMesh.subMeshCount;
			List<int> list = new List<int>();
			int num3 = (this.backtrackingPoints - 2) * (this.ivyParameters.sides * 6) + this.ivyParameters.sides * 3;
			num3 *= numBranches;
			list.Add(num3);
			for (int i = 1; i < subMeshCount; i++)
			{
				int item = this.backtrackingPoints * 6 * numBranches;
				list.Add(item);
			}
			this.buildingMeshData = new RTMeshData(num2, subMeshCount, list);
		}

		// Token: 0x060005E2 RID: 1506 RVA: 0x00047123 File Offset: 0x00045323
		public void CreateBakedMeshData(Mesh bakedMesh)
		{
			this.bakedMeshData = new RTMeshData(bakedMesh);
		}

		// Token: 0x060005E3 RID: 1507 RVA: 0x000A4DF0 File Offset: 0x000A2FF0
		public void CreateProcessedMeshDataProcedural(Mesh bakedMesh, float lifetime, float velocity)
		{
			int num = Mathf.CeilToInt(lifetime / velocity * 200f);
			int numVertices = num * (this.ivyParameters.sides + 1);
			int subMeshCount = bakedMesh.subMeshCount;
			List<int> list = new List<int>();
			for (int i = 0; i < subMeshCount; i++)
			{
				int item = this.ivyParameters.sides * num * 9;
				list.Add(item);
			}
			this.processedMeshData = new RTMeshData(numVertices, subMeshCount, list);
		}

		// Token: 0x060005E4 RID: 1508 RVA: 0x000A4E64 File Offset: 0x000A3064
		public void CreateProcessedMeshData(Mesh bakedMesh)
		{
			int vertexCount = bakedMesh.vertexCount;
			int subMeshCount = bakedMesh.subMeshCount;
			List<int> list = new List<int>();
			for (int i = 0; i < subMeshCount; i++)
			{
				int item = bakedMesh.GetTriangles(i).Length;
				list.Add(item);
			}
			this.processedMeshData = new RTMeshData(vertexCount, subMeshCount, list);
		}

		// Token: 0x060005E5 RID: 1509 RVA: 0x00047131 File Offset: 0x00045331
		public void SetLeafLengthCorrectionFactor(float leafLengthCorrrectionFactor)
		{
			this.leafLengthCorrrectionFactor = leafLengthCorrrectionFactor;
		}

		// Token: 0x060005E6 RID: 1510 RVA: 0x0004713A File Offset: 0x0004533A
		public void ClearMesh()
		{
			this.ivyMesh.Clear();
		}

		// Token: 0x060005E7 RID: 1511 RVA: 0x00047147 File Offset: 0x00045347
		public RTBakedMeshBuilder()
		{
		}

		// Token: 0x060005E8 RID: 1512 RVA: 0x0004715B File Offset: 0x0004535B
		public RTBakedMeshBuilder(RTIvyContainer ivyContainer, GameObject ivyGo)
		{
			this.rtIvyContainer = ivyContainer;
			this.ivyGO = ivyGo;
		}

		// Token: 0x060005E9 RID: 1513 RVA: 0x000A4EB4 File Offset: 0x000A30B4
		private void ClearTipMesh()
		{
			this.buildingMeshData.Clear();
			for (int i = 0; i < this.vertCountsPerBranch.Length; i++)
			{
				this.vertCountsPerBranch[i] = 0;
				this.lastTriangleIndexPerBranch[i] = 0;
				this.vertCountLeavesPerBranch[i] = 0;
			}
			this.vertCount = 0;
			this.triCount = 0;
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x000A4F08 File Offset: 0x000A3108
		public void CheckCopyMesh(int branchIndex, List<RTBranchContainer> bakedBranches)
		{
			RTBranchContainer rtbranchContainer = this.rtIvyContainer.branches[branchIndex];
			RTBranchContainer bakedBranchContainer = bakedBranches[branchIndex];
			int initSegmentIdx;
			int endSegmentIdx = (initSegmentIdx = Mathf.Clamp(rtbranchContainer.branchPoints.Count - this.backtrackingPoints - 1, 0, int.MaxValue)) + 1;
			this.CopyToFixedMesh(branchIndex, initSegmentIdx, endSegmentIdx, rtbranchContainer, bakedBranchContainer);
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x000A4F60 File Offset: 0x000A3160
		public void BuildGeometry02(List<RTBranchContainer> activeBakedBranches, List<RTBranchContainer> activeBuildingBranches)
		{
			if (!this.ivyParameters.halfgeom)
			{
				this.angle = 360f / (float)this.ivyParameters.sides;
			}
			else
			{
				this.angle = 360f / (float)this.ivyParameters.sides / 2f;
			}
			if (this.leavesDataInitialized)
			{
				this.ClearTipMesh();
				for (int i = 0; i < this.rtIvyContainer.branches.Count; i++)
				{
					int num = this.vertCount;
					RTBranchContainer rtbranchContainer = activeBuildingBranches[i];
					if (rtbranchContainer.branchPoints.Count > 1)
					{
						this.lastVertCount = 0;
						int num2 = rtbranchContainer.branchPoints.Count - this.backtrackingPoints;
						num2 = Mathf.Clamp(num2, 0, int.MaxValue);
						int count = rtbranchContainer.branchPoints.Count;
						for (int j = num2; j < count; j++)
						{
							RTBranchPoint rtbranchPoint = rtbranchContainer.branchPoints[j];
							Vector3 vector = this.ivyGO.transform.InverseTransformPoint(rtbranchPoint.point);
							Vector3 vertexValue = this.zeroVector3;
							Vector3 vector2 = this.zeroVector3;
							Vector2 lastUV = this.zeroVector2;
							Vector2 vector3 = this.zeroVector2;
							Color color = this.blackColor;
							float t = Mathf.InverseLerp(rtbranchContainer.totalLength, rtbranchContainer.totalLength - this.ivyParameters.tipInfluence, rtbranchPoint.length);
							if (j < rtbranchContainer.branchPoints.Count - 1)
							{
								for (int k = 0; k < rtbranchPoint.verticesLoop.Length; k++)
								{
									if (this.ivyParameters.generateBranches)
									{
										vertexValue = Vector3.LerpUnclamped(rtbranchPoint.centerLoop, rtbranchPoint.verticesLoop[k].vertex, t);
										this.buildingMeshData.AddVertex(vertexValue, rtbranchPoint.verticesLoop[k].normal, rtbranchPoint.verticesLoop[k].uv, rtbranchPoint.verticesLoop[k].color);
										this.vertCountsPerBranch[i]++;
										this.vertCount++;
										this.lastVertCount++;
									}
								}
							}
							else
							{
								vertexValue = vector;
								vector2 = Vector3.Normalize(rtbranchPoint.point - rtbranchPoint.GetPreviousPoint().point);
								vector2 = this.ivyGO.transform.InverseTransformVector(vector2);
								lastUV = rtbranchContainer.GetLastUV(this.ivyParameters);
								this.buildingMeshData.AddVertex(vertexValue, vector2, lastUV, Color.black);
								this.vertCountsPerBranch[i]++;
								this.vertCount++;
								this.lastVertCount++;
							}
						}
						this.SetTriangles(rtbranchContainer, this.vertCount, num2, i);
					}
					this.fromTo[0] = num;
					this.fromTo[1] = this.vertCount - 1;
					if (this.ivyParameters.generateLeaves)
					{
						this.BuildLeaves(i, activeBuildingBranches[i], activeBakedBranches[i]);
					}
				}
				this.RefreshMesh();
			}
		}

		// Token: 0x060005EC RID: 1516 RVA: 0x000A5278 File Offset: 0x000A3478
		private float CalculateRadius(BranchPoint branchPoint, BranchContainer buildingBranchContainer)
		{
			float num = Mathf.InverseLerp(branchPoint.branchContainer.totalLenght, branchPoint.branchContainer.totalLenght - this.ivyParameters.tipInfluence, branchPoint.length - 0.1f);
			branchPoint.currentRadius = branchPoint.radius * num;
			return branchPoint.currentRadius;
		}

		// Token: 0x060005ED RID: 1517 RVA: 0x000A52D0 File Offset: 0x000A34D0
		private void SetTriangles(RTBranchContainer branch, int vertCount, int initIndex, int branchIndex)
		{
			int num = 0;
			int num2 = Mathf.Min(branch.branchPoints.Count - 2, branch.branchPoints.Count - initIndex - 2);
			for (int i = num; i < num2; i++)
			{
				for (int j = 0; j < this.ivyParameters.sides; j++)
				{
					int num3 = vertCount - this.lastVertCount;
					int value = j + i * (this.ivyParameters.sides + 1) + num3;
					int value2 = j + i * (this.ivyParameters.sides + 1) + 1 + num3;
					int value3 = j + i * (this.ivyParameters.sides + 1) + this.ivyParameters.sides + 1 + num3;
					int value4 = j + i * (this.ivyParameters.sides + 1) + 1 + num3;
					int value5 = j + i * (this.ivyParameters.sides + 1) + this.ivyParameters.sides + 2 + num3;
					int value6 = j + i * (this.ivyParameters.sides + 1) + this.ivyParameters.sides + 1 + num3;
					this.buildingMeshData.AddTriangle(0, value);
					this.buildingMeshData.AddTriangle(0, value2);
					this.buildingMeshData.AddTriangle(0, value3);
					this.buildingMeshData.AddTriangle(0, value4);
					this.buildingMeshData.AddTriangle(0, value5);
					this.buildingMeshData.AddTriangle(0, value6);
					this.triCount += 6;
				}
			}
			int k = 0;
			int num4 = 0;
			while (k < this.ivyParameters.sides * 3)
			{
				this.buildingMeshData.AddTriangle(0, vertCount - 1);
				this.buildingMeshData.AddTriangle(0, vertCount - 3 - num4);
				this.buildingMeshData.AddTriangle(0, vertCount - 2 - num4);
				this.triCount += 3;
				k += 3;
				num4++;
			}
			this.lastTriangleIndexPerBranch[branchIndex] = vertCount - 1;
		}

		// Token: 0x060005EE RID: 1518 RVA: 0x000A54B8 File Offset: 0x000A36B8
		private void BuildLeaves(int branchIndex, RTBranchContainer buildingBranchContainer, RTBranchContainer bakedBranchContainer)
		{
			for (int i = Mathf.Clamp(buildingBranchContainer.branchPoints.Count - this.backtrackingPoints, 0, int.MaxValue); i < buildingBranchContainer.branchPoints.Count; i++)
			{
				RTLeafPoint[] array = bakedBranchContainer.leavesOrderedByInitSegment[i];
				RTBranchPoint rtbranchPoint = buildingBranchContainer.branchPoints[i];
				foreach (RTLeafPoint rtleafPoint in array)
				{
					if (rtleafPoint != null)
					{
						float t = Mathf.InverseLerp(buildingBranchContainer.totalLength, buildingBranchContainer.totalLength - this.ivyParameters.tipInfluence, rtbranchPoint.length);
						RTMeshData rtmeshData = this.leavesMeshesByChosenLeaf[rtleafPoint.chosenLeave];
						for (int k = 0; k < rtmeshData.triangles[0].Length; k++)
						{
							int value = rtmeshData.triangles[0][k] + this.vertCount;
							int sumbesh = this.submeshByChoseLeaf[rtleafPoint.chosenLeave];
							this.buildingMeshData.AddTriangle(sumbesh, value);
						}
						for (int l = 0; l < rtleafPoint.vertices.Length; l++)
						{
							Vector3 vertexValue = Vector3.LerpUnclamped(rtleafPoint.leafCenter, rtleafPoint.vertices[l].vertex, t);
							this.buildingMeshData.AddVertex(vertexValue, rtleafPoint.vertices[l].normal, rtleafPoint.vertices[l].uv, rtleafPoint.vertices[l].color);
							this.vertCountLeavesPerBranch[branchIndex]++;
							this.vertCountsPerBranch[branchIndex]++;
							this.vertCount++;
						}
					}
				}
			}
		}

		// Token: 0x060005EF RID: 1519 RVA: 0x000A5670 File Offset: 0x000A3870
		public void CopyToFixedMesh(int branchIndex, int initSegmentIdx, int endSegmentIdx, RTBranchContainer branchContainer, RTBranchContainer bakedBranchContainer)
		{
			int num = this.ivyParameters.sides + 1;
			int sides = this.ivyParameters.sides;
			int num2 = this.vertCountsPerBranch[branchIndex];
			int num3 = this.vertCountLeavesPerBranch[branchIndex];
			int num4 = 0;
			for (int i = 1; i <= branchIndex; i++)
			{
				num4 += this.vertCountsPerBranch[branchIndex];
			}
			int num5;
			if (this.processedBranchesVerticesIndicesPerBranch[branchIndex].Count <= 0)
			{
				num5 = 2;
			}
			else
			{
				num5 = 1;
				num4 += num;
			}
			for (int j = num5 - 1; j >= 0; j--)
			{
				int index = branchContainer.branchPoints.Count - this.backtrackingPoints - j;
				RTBranchPoint rtbranchPoint = branchContainer.branchPoints[index];
				for (int k = 0; k < rtbranchPoint.verticesLoop.Length; k++)
				{
					RTVertexData rtvertexData = rtbranchPoint.verticesLoop[k];
					this.processedMeshData.AddVertex(rtvertexData.vertex, rtvertexData.normal, rtvertexData.uv, rtvertexData.color);
					this.processedBranchesVerticesIndicesPerBranch[branchIndex].Add(this.processedMeshData.VertexCount() - 1);
				}
			}
			if (branchIndex > 0)
			{
				int num6 = this.lastTriangleIndexPerBranch[branchIndex];
			}
			if (this.processedBranchesVerticesIndicesPerBranch[branchIndex].Count >= num * 2)
			{
				int num7 = this.processedBranchesVerticesIndicesPerBranch[branchIndex].Count - num * 2;
				for (int l = 0; l < this.ivyParameters.sides; l++)
				{
					int value = this.processedBranchesVerticesIndicesPerBranch[branchIndex][l + num7];
					int value2 = this.processedBranchesVerticesIndicesPerBranch[branchIndex][l + 1 + num7];
					int value3 = this.processedBranchesVerticesIndicesPerBranch[branchIndex][l + this.ivyParameters.sides + 1 + num7];
					int value4 = this.processedBranchesVerticesIndicesPerBranch[branchIndex][l + 1 + num7];
					int value5 = this.processedBranchesVerticesIndicesPerBranch[branchIndex][l + this.ivyParameters.sides + 2 + num7];
					int value6 = this.processedBranchesVerticesIndicesPerBranch[branchIndex][l + this.ivyParameters.sides + 1 + num7];
					this.processedMeshData.AddTriangle(0, value);
					this.processedMeshData.AddTriangle(0, value2);
					this.processedMeshData.AddTriangle(0, value3);
					this.processedMeshData.AddTriangle(0, value4);
					this.processedMeshData.AddTriangle(0, value5);
					this.processedMeshData.AddTriangle(0, value6);
				}
			}
			if (this.ivyParameters.generateLeaves)
			{
				int num8 = this.processedMeshData.VertexCount();
				int num9 = 0;
				for (int m = initSegmentIdx; m < endSegmentIdx; m++)
				{
					foreach (RTLeafPoint rtleafPoint in bakedBranchContainer.leavesOrderedByInitSegment[m])
					{
						if (rtleafPoint != null)
						{
							RTMeshData rtmeshData = this.leavesMeshesByChosenLeaf[rtleafPoint.chosenLeave];
							int sumbesh = this.submeshByChoseLeaf[rtleafPoint.chosenLeave];
							for (int num10 = 0; num10 < rtmeshData.triangles[0].Length; num10++)
							{
								int value7 = rtmeshData.triangles[0][num10] + num8;
								this.processedMeshData.AddTriangle(sumbesh, value7);
							}
							for (int num11 = 0; num11 < rtleafPoint.vertices.Length; num11++)
							{
								RTVertexData rtvertexData2 = rtleafPoint.vertices[num11];
								this.processedMeshData.AddVertex(rtvertexData2.vertex, rtvertexData2.normal, rtvertexData2.uv, rtvertexData2.color);
								this.processedVerticesIndicesPerBranch[branchIndex].Add(this.processedMeshData.VertexCount() - 1);
								num8++;
							}
							num9++;
						}
					}
				}
			}
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x000A5A3C File Offset: 0x000A3C3C
		public void RefreshProcessedMesh()
		{
			this.processedMesh.MarkDynamic();
			this.processedMesh.subMeshCount = this.submeshCount;
			this.processedMesh.vertices = this.processedMeshData.vertices;
			this.processedMesh.normals = this.processedMeshData.normals;
			this.processedMesh.colors = this.processedMeshData.colors;
			this.processedMesh.uv = this.processedMeshData.uv;
			this.processedMesh.SetTriangles(this.processedMeshData.triangles[0], 0);
			if (this.ivyParameters.generateLeaves)
			{
				for (int i = 1; i < this.submeshCount; i++)
				{
					this.processedMesh.SetTriangles(this.processedMeshData.triangles[i], i);
				}
			}
			this.processedMesh.RecalculateBounds();
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x000A5B18 File Offset: 0x000A3D18
		private void RefreshMesh()
		{
			this.ivyMesh.Clear();
			this.ivyMesh.subMeshCount = this.submeshCount;
			this.ivyMesh.MarkDynamic();
			this.ivyMesh.vertices = this.buildingMeshData.vertices;
			this.ivyMesh.normals = this.buildingMeshData.normals;
			this.ivyMesh.colors = this.buildingMeshData.colors;
			this.ivyMesh.uv = this.buildingMeshData.uv;
			this.ivyMesh.SetTriangles(this.buildingMeshData.triangles[0], 0);
			if (this.ivyParameters.generateLeaves)
			{
				for (int i = 1; i < this.submeshCount; i++)
				{
					this.ivyMesh.SetTriangles(this.buildingMeshData.triangles[i], i);
				}
			}
			this.ivyMesh.RecalculateBounds();
		}

		// Token: 0x040006C8 RID: 1736
		public IvyParameters ivyParameters;

		// Token: 0x040006C9 RID: 1737
		public RTIvyContainer rtIvyContainer;

		// Token: 0x040006CA RID: 1738
		public RTIvyContainer rtBakedIvyContainer;

		// Token: 0x040006CB RID: 1739
		public GameObject ivyGO;

		// Token: 0x040006CC RID: 1740
		public MeshRenderer meshRenderer;

		// Token: 0x040006CD RID: 1741
		public MeshFilter meshFilter;

		// Token: 0x040006CE RID: 1742
		private bool onOptimizedStretch;

		// Token: 0x040006CF RID: 1743
		private MeshFilter leavesMeshFilter;

		// Token: 0x040006D0 RID: 1744
		private MeshRenderer leavesMeshRenderer;

		// Token: 0x040006D1 RID: 1745
		private MeshRenderer mrProcessedMesh;

		// Token: 0x040006D2 RID: 1746
		private Mesh processedMesh;

		// Token: 0x040006D3 RID: 1747
		private Mesh ivyMesh;

		// Token: 0x040006D4 RID: 1748
		public List<RTBranchContainer> activeBranches;

		// Token: 0x040006D5 RID: 1749
		public RTMeshData bakedMeshData;

		// Token: 0x040006D6 RID: 1750
		public RTMeshData buildingMeshData;

		// Token: 0x040006D7 RID: 1751
		public RTMeshData processedMeshData;

		// Token: 0x040006D8 RID: 1752
		public List<List<int>> processedVerticesIndicesPerBranch;

		// Token: 0x040006D9 RID: 1753
		public List<List<int>> processedBranchesVerticesIndicesPerBranch;

		// Token: 0x040006DA RID: 1754
		private int[] vertCountsPerBranch;

		// Token: 0x040006DB RID: 1755
		private int[] lastTriangleIndexPerBranch;

		// Token: 0x040006DC RID: 1756
		private int[] vertCountLeavesPerBranch;

		// Token: 0x040006DD RID: 1757
		private int lastPointCopied;

		// Token: 0x040006DE RID: 1758
		private int vertCount;

		// Token: 0x040006DF RID: 1759
		private int lastVertCount;

		// Token: 0x040006E0 RID: 1760
		private int triCount;

		// Token: 0x040006E1 RID: 1761
		private int lastVerticesCountProcessed;

		// Token: 0x040006E2 RID: 1762
		private int lastLeafVertProcessed;

		// Token: 0x040006E3 RID: 1763
		private int submeshCount;

		// Token: 0x040006E4 RID: 1764
		private int[] submeshByChoseLeaf;

		// Token: 0x040006E5 RID: 1765
		private int initIdxLeaves;

		// Token: 0x040006E6 RID: 1766
		private int endIdxLeaves;

		// Token: 0x040006E7 RID: 1767
		private int backtrackingPoints;

		// Token: 0x040006E8 RID: 1768
		private int[] fromTo;

		// Token: 0x040006E9 RID: 1769
		private Vector3[] vectors;

		// Token: 0x040006EA RID: 1770
		private RTMeshData[] leavesMeshesByChosenLeaf;

		// Token: 0x040006EB RID: 1771
		private int lastVertexIndex;

		// Token: 0x040006EC RID: 1772
		private float angle;

		// Token: 0x040006ED RID: 1773
		public List<Material> leavesMaterials;

		// Token: 0x040006EE RID: 1774
		public List<List<int>> typesByMat;

		// Token: 0x040006EF RID: 1775
		public Rect[] uv2Rects = new Rect[0];

		// Token: 0x040006F0 RID: 1776
		public bool leavesDataInitialized;

		// Token: 0x040006F1 RID: 1777
		private float growthSpeed;

		// Token: 0x040006F2 RID: 1778
		private float leafLengthCorrrectionFactor;

		// Token: 0x040006F3 RID: 1779
		private Vector3 ivyGoPosition;

		// Token: 0x040006F4 RID: 1780
		private Quaternion ivyGoRotation;

		// Token: 0x040006F5 RID: 1781
		private Quaternion ivyGoInverseRotation;

		// Token: 0x040006F6 RID: 1782
		private Vector3 zeroVector3;

		// Token: 0x040006F7 RID: 1783
		private Vector2 zeroVector2;

		// Token: 0x040006F8 RID: 1784
		private Color blackColor;
	}
}
