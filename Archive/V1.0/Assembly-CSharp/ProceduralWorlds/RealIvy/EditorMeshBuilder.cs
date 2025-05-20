using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x0200008C RID: 140
	public class EditorMeshBuilder : ScriptableObject
	{
		// Token: 0x060005AC RID: 1452 RVA: 0x000A18F8 File Offset: 0x0009FAF8
		public void InitLeavesData()
		{
			if (this.infoPool.ivyContainer.ivyGO)
			{
				this.infoPool.meshBuilder.typesByMat = new List<List<int>>();
				this.infoPool.meshBuilder.leavesMaterials = new List<Material>();
				if (this.infoPool.ivyParameters.generateLeaves)
				{
					for (int i = 0; i < this.infoPool.ivyParameters.leavesPrefabs.Length; i++)
					{
						bool flag = false;
						for (int j = 0; j < this.infoPool.meshBuilder.leavesMaterials.Count; j++)
						{
							if (this.infoPool.meshBuilder.leavesMaterials[j] == this.infoPool.ivyParameters.leavesPrefabs[i].GetComponent<MeshRenderer>().sharedMaterial)
							{
								this.infoPool.meshBuilder.typesByMat[j].Add(i);
								flag = true;
							}
						}
						if (!flag)
						{
							this.infoPool.meshBuilder.leavesMaterials.Add(this.infoPool.ivyParameters.leavesPrefabs[i].GetComponent<MeshRenderer>().sharedMaterial);
							this.infoPool.meshBuilder.typesByMat.Add(new List<int>());
							this.infoPool.meshBuilder.typesByMat[this.infoPool.meshBuilder.typesByMat.Count - 1].Add(i);
						}
					}
					Material[] array = new Material[this.leavesMaterials.Count + 1];
					for (int k = 0; k < array.Length; k++)
					{
						if (k == 0)
						{
							array[k] = this.infoPool.ivyContainer.ivyGO.GetComponent<MeshRenderer>().sharedMaterial;
						}
						else
						{
							array[k] = this.infoPool.meshBuilder.leavesMaterials[k - 1];
						}
					}
					this.infoPool.ivyContainer.ivyGO.GetComponent<MeshRenderer>().sharedMaterials = array;
				}
				else
				{
					this.infoPool.ivyContainer.ivyGO.GetComponent<MeshRenderer>().sharedMaterials = new Material[]
					{
						this.infoPool.ivyParameters.branchesMaterial
					};
				}
				this.leavesDataInitialized = true;
			}
		}

		// Token: 0x060005AD RID: 1453 RVA: 0x000A1B38 File Offset: 0x0009FD38
		public void Initialize()
		{
			this.infoPool.meshBuilder.trisLeaves = new List<List<int>>();
			for (int i = 0; i < this.infoPool.meshBuilder.leavesMaterials.Count; i++)
			{
				this.infoPool.meshBuilder.trisLeaves.Add(new List<int>());
			}
			this.ivyMesh.Clear();
			if (this.infoPool.ivyParameters.buffer32Bits)
			{
				this.ivyMesh.indexFormat = IndexFormat.UInt32;
			}
			this.ivyMesh.name = "Ivy Mesh";
			this.ivyMesh.subMeshCount = this.leavesMaterials.Count + 1;
			this.branchesLeavesIndices.Clear();
			int num = 0;
			int num2 = 0;
			if (this.infoPool.ivyParameters.generateBranches)
			{
				for (int j = 0; j < this.infoPool.ivyContainer.branches.Count; j++)
				{
					if (this.infoPool.ivyContainer.branches[j].branchPoints.Count > 1)
					{
						num += (this.infoPool.ivyContainer.branches[j].branchPoints.Count - 1) * (this.infoPool.ivyParameters.sides + 1) + 1;
						num2 += (this.infoPool.ivyContainer.branches[j].branchPoints.Count - 2) * this.infoPool.ivyParameters.sides * 2 * 3 + this.infoPool.ivyParameters.sides * 3;
					}
				}
			}
			if (this.infoPool.ivyParameters.generateLeaves && this.infoPool.ivyParameters.leavesPrefabs.Length != 0)
			{
				for (int k = 0; k < this.infoPool.ivyContainer.branches.Count; k++)
				{
					if (this.infoPool.ivyContainer.branches[k].branchPoints.Count > 1)
					{
						for (int l = 0; l < this.infoPool.ivyContainer.branches[k].leaves.Count; l++)
						{
							BranchContainer branchContainer = this.infoPool.ivyContainer.branches[k];
							MeshFilter component = this.infoPool.ivyParameters.leavesPrefabs[branchContainer.leaves[l].chosenLeave].GetComponent<MeshFilter>();
							num += component.sharedMesh.vertexCount;
						}
					}
				}
			}
			this.verts = new Vector3[num];
			this.normals = new Vector3[num];
			this.uvs = new Vector2[num];
			this.vColor = new Color[num];
			this.trisBranches = new int[Mathf.Max(num2, 0)];
			if (!this.infoPool.ivyParameters.halfgeom)
			{
				this.angle = 360f / (float)this.infoPool.ivyParameters.sides;
				return;
			}
			this.angle = 360f / (float)this.infoPool.ivyParameters.sides / 2f;
		}

		// Token: 0x060005AE RID: 1454 RVA: 0x000A1E70 File Offset: 0x000A0070
		private void BuildLeaves(int b, ref int vertCount)
		{
			for (int i = 0; i < this.leavesMaterials.Count; i++)
			{
				UnityEngine.Random.InitState(b + this.infoPool.ivyParameters.randomSeed + i);
				for (int j = 0; j < this.infoPool.ivyContainer.branches[b].leaves.Count; j++)
				{
					LeafPoint leafPoint = this.infoPool.ivyContainer.branches[b].leaves[j];
					if (this.typesByMat[i].Contains(leafPoint.chosenLeave))
					{
						leafPoint.verticesLeaves = new List<RTVertexData>();
						Mesh sharedMesh = this.infoPool.ivyParameters.leavesPrefabs[leafPoint.chosenLeave].GetComponent<MeshFilter>().sharedMesh;
						int num = vertCount;
						Vector3 vector;
						Vector3 vector2;
						if (!this.infoPool.ivyParameters.globalOrientation)
						{
							vector = leafPoint.lpForward;
							vector2 = leafPoint.left;
						}
						else
						{
							vector = this.infoPool.ivyParameters.globalRotation;
							vector2 = Vector3.Normalize(Vector3.Cross(this.infoPool.ivyParameters.globalRotation, leafPoint.lpUpward));
						}
						Quaternion quaternion = Quaternion.LookRotation(leafPoint.lpUpward, vector);
						quaternion = Quaternion.AngleAxis(this.infoPool.ivyParameters.rotation.x, vector2) * Quaternion.AngleAxis(this.infoPool.ivyParameters.rotation.y, leafPoint.lpUpward) * Quaternion.AngleAxis(this.infoPool.ivyParameters.rotation.z, vector) * quaternion;
						quaternion = Quaternion.AngleAxis(UnityEngine.Random.Range(-this.infoPool.ivyParameters.randomRotation.x, this.infoPool.ivyParameters.randomRotation.x), vector2) * Quaternion.AngleAxis(UnityEngine.Random.Range(-this.infoPool.ivyParameters.randomRotation.y, this.infoPool.ivyParameters.randomRotation.y), leafPoint.lpUpward) * Quaternion.AngleAxis(UnityEngine.Random.Range(-this.infoPool.ivyParameters.randomRotation.z, this.infoPool.ivyParameters.randomRotation.z), vector) * quaternion;
						quaternion = leafPoint.forwarRot * quaternion;
						float num2 = UnityEngine.Random.Range(this.infoPool.ivyParameters.minScale, this.infoPool.ivyParameters.maxScale);
						leafPoint.leafScale = num2;
						num2 *= Mathf.InverseLerp(this.infoPool.ivyContainer.branches[b].totalLenght, this.infoPool.ivyContainer.branches[b].totalLenght - this.infoPool.ivyParameters.tipInfluence, leafPoint.lpLength);
						leafPoint.leafRotation = quaternion;
						leafPoint.dstScale = num2;
						for (int k = 0; k < sharedMesh.triangles.Length; k++)
						{
							int item = sharedMesh.triangles[k] + vertCount;
							this.trisLeaves[i].Add(item);
						}
						for (int l = 0; l < sharedMesh.vertexCount; l++)
						{
							Vector3 b2 = vector2 * this.infoPool.ivyParameters.offset.x + leafPoint.lpUpward * this.infoPool.ivyParameters.offset.y + leafPoint.lpForward * this.infoPool.ivyParameters.offset.z;
							this.verts[vertCount] = quaternion * sharedMesh.vertices[l] * num2 + leafPoint.point + b2;
							this.normals[vertCount] = quaternion * sharedMesh.normals[l];
							this.uvs[vertCount] = sharedMesh.uv[l];
							this.vColor[vertCount] = sharedMesh.colors[l];
							this.normals[vertCount] = Quaternion.Inverse(this.infoPool.ivyContainer.ivyGO.transform.rotation) * this.normals[vertCount];
							this.verts[vertCount] -= this.infoPool.ivyContainer.ivyGO.transform.position;
							this.verts[vertCount] = Quaternion.Inverse(this.infoPool.ivyContainer.ivyGO.transform.rotation) * this.verts[vertCount];
							RTVertexData item2 = new RTVertexData(this.verts[vertCount], this.normals[vertCount], this.uvs[vertCount], Vector2.zero, this.vColor[vertCount]);
							leafPoint.verticesLeaves.Add(item2);
							leafPoint.leafCenter = leafPoint.point - this.infoPool.ivyContainer.ivyGO.transform.position;
							leafPoint.leafCenter = Quaternion.Inverse(this.infoPool.ivyContainer.ivyGO.transform.rotation) * leafPoint.leafCenter;
							vertCount++;
						}
						int[] value = new int[]
						{
							num,
							vertCount - 1
						};
						this.branchesLeavesIndices.Add(this.branchesLeavesIndices.Count, value);
					}
				}
			}
		}

		// Token: 0x060005AF RID: 1455 RVA: 0x000A245C File Offset: 0x000A065C
		public void BuildGeometry()
		{
			if (this.leavesDataInitialized)
			{
				this.Initialize();
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < this.infoPool.ivyContainer.branches.Count; i++)
				{
					int num3 = num;
					UnityEngine.Random.InitState(i + this.infoPool.ivyParameters.randomSeed);
					if (this.infoPool.ivyContainer.branches[i].branchPoints.Count > 1)
					{
						int num4 = 0;
						for (int j = 0; j < this.infoPool.ivyContainer.branches[i].branchPoints.Count; j++)
						{
							BranchPoint branchPoint = this.infoPool.ivyContainer.branches[i].branchPoints[j];
							branchPoint.verticesLoop = new List<RTVertexData>();
							Vector3 vector = branchPoint.point - this.infoPool.ivyContainer.ivyGO.transform.position;
							vector = Quaternion.Inverse(this.infoPool.ivyContainer.ivyGO.transform.rotation) * vector;
							float num5 = this.CalculateRadius(branchPoint.length, this.infoPool.ivyContainer.branches[i].totalLenght);
							branchPoint.radius = num5;
							if (j != this.infoPool.ivyContainer.branches[i].branchPoints.Count - 1)
							{
								Vector3[] array = this.CalculateVectors(this.infoPool.ivyContainer.branches[i].branchPoints[j].point, j, i);
								branchPoint.firstVector = array[0];
								branchPoint.axis = array[1];
								for (int k = 0; k < this.infoPool.ivyParameters.sides + 1; k++)
								{
									if (this.infoPool.ivyParameters.generateBranches)
									{
										float tipInfluence = this.GetTipInfluence(branchPoint.length, this.infoPool.ivyContainer.branches[i].totalLenght);
										this.infoPool.ivyContainer.branches[i].branchPoints[j].radius = num5;
										Vector3 vector2 = Quaternion.AngleAxis(this.angle * (float)k, array[1]) * array[0];
										if (this.infoPool.ivyParameters.halfgeom && this.infoPool.ivyParameters.sides == 1)
										{
											this.normals[num] = -this.infoPool.ivyContainer.branches[i].branchPoints[j].grabVector;
										}
										else
										{
											this.normals[num] = vector2;
										}
										Vector3 vertex = vector2 * num5 + vector;
										this.verts[num] = vector2 * num5 * tipInfluence + this.infoPool.ivyContainer.branches[i].branchPoints[j].point;
										this.verts[num] -= this.infoPool.ivyContainer.ivyGO.transform.position;
										this.verts[num] = Quaternion.Inverse(this.infoPool.ivyContainer.ivyGO.transform.rotation) * this.verts[num];
										this.uvs[num] = new Vector2(branchPoint.length * this.infoPool.ivyParameters.uvScale.y + this.infoPool.ivyParameters.uvOffset.y - this.infoPool.ivyParameters.stepSize, 1f / (float)this.infoPool.ivyParameters.sides * (float)k * this.infoPool.ivyParameters.uvScale.x + this.infoPool.ivyParameters.uvOffset.x);
										this.normals[num] = Quaternion.Inverse(this.infoPool.ivyContainer.ivyGO.transform.rotation) * this.normals[num];
										RTVertexData item = new RTVertexData(vertex, this.normals[num], this.uvs[num], Vector2.zero, this.vColor[num]);
										branchPoint.verticesLoop.Add(item);
										num++;
										num4++;
									}
								}
							}
							else if (this.infoPool.ivyParameters.generateBranches)
							{
								this.verts[num] = this.infoPool.ivyContainer.branches[i].branchPoints[j].point;
								this.verts[num] -= this.infoPool.ivyContainer.ivyGO.transform.position;
								this.verts[num] = Quaternion.Inverse(this.infoPool.ivyContainer.ivyGO.transform.rotation) * this.verts[num];
								if (this.infoPool.ivyParameters.halfgeom && this.infoPool.ivyParameters.sides == 1)
								{
									this.normals[num] = -this.infoPool.ivyContainer.branches[i].branchPoints[j].grabVector;
								}
								else
								{
									this.normals[num] = Vector3.Normalize(this.infoPool.ivyContainer.branches[i].branchPoints[j].point - this.infoPool.ivyContainer.branches[i].branchPoints[j - 1].point);
								}
								this.uvs[num] = new Vector2(this.infoPool.ivyContainer.branches[i].totalLenght * this.infoPool.ivyParameters.uvScale.y + this.infoPool.ivyParameters.uvOffset.y, 0.5f * this.infoPool.ivyParameters.uvScale.x + this.infoPool.ivyParameters.uvOffset.x);
								this.normals[num] = Quaternion.Inverse(this.infoPool.ivyContainer.ivyGO.transform.rotation) * this.normals[num];
								Vector3 vertex2 = vector;
								RTVertexData item2 = new RTVertexData(vertex2, this.normals[num], this.uvs[num], Vector2.zero, this.vColor[num]);
								branchPoint.verticesLoop.Add(item2);
								num++;
								num4++;
								this.TriangulateBranch(i, ref num2, num, num4);
							}
						}
					}
					int[] value = new int[]
					{
						num3,
						num - 1
					};
					this.branchesLeavesIndices.Add(this.branchesLeavesIndices.Count, value);
					if (this.infoPool.ivyParameters.generateLeaves)
					{
						this.BuildLeaves(i, ref num);
					}
				}
				this.ivyMesh.vertices = this.verts;
				this.ivyMesh.normals = this.normals;
				this.ivyMesh.uv = this.uvs;
				this.ivyMesh.colors = this.vColor;
				this.ivyMesh.SetTriangles(this.trisBranches, 0);
				for (int l = 0; l < this.leavesMaterials.Count; l++)
				{
					this.ivyMesh.SetTriangles(this.trisLeaves[l], l + 1);
				}
				this.ivyMesh.RecalculateTangents();
				this.ivyMesh.RecalculateBounds();
			}
		}

		// Token: 0x060005B0 RID: 1456 RVA: 0x000A2CE4 File Offset: 0x000A0EE4
		private Vector3[] CalculateVectors(Vector3 branchPoint, int p, int b)
		{
			Vector3 vector;
			Vector3 vector2;
			if (b == 0 && p == 0)
			{
				vector = this.infoPool.ivyContainer.ivyGO.transform.up;
				if (!this.infoPool.ivyParameters.halfgeom)
				{
					vector2 = this.infoPool.ivyContainer.firstVertexVector;
				}
				else
				{
					vector2 = Quaternion.AngleAxis(90f, vector) * this.infoPool.ivyContainer.firstVertexVector;
				}
			}
			else
			{
				if (p == 0)
				{
					vector = this.infoPool.ivyContainer.branches[b].branchPoints[p + 1].point - this.infoPool.ivyContainer.branches[b].branchPoints[p].point;
				}
				else
				{
					vector = Vector3.Normalize(Vector3.Lerp(this.infoPool.ivyContainer.branches[b].branchPoints[p].point - this.infoPool.ivyContainer.branches[b].branchPoints[p - 1].point, this.infoPool.ivyContainer.branches[b].branchPoints[p + 1].point - this.infoPool.ivyContainer.branches[b].branchPoints[p].point, 0.5f));
				}
				if (!this.infoPool.ivyParameters.halfgeom)
				{
					vector2 = Vector3.Normalize(Vector3.ProjectOnPlane(this.infoPool.ivyContainer.branches[b].branchPoints[p].grabVector, vector));
				}
				else
				{
					vector2 = Quaternion.AngleAxis(90f, vector) * Vector3.Normalize(Vector3.ProjectOnPlane(this.infoPool.ivyContainer.branches[b].branchPoints[p].grabVector, vector));
				}
			}
			return new Vector3[]
			{
				vector2,
				vector
			};
		}

		// Token: 0x060005B1 RID: 1457 RVA: 0x000A2F10 File Offset: 0x000A1110
		private float CalculateRadius(float lenght, float totalLenght)
		{
			float t = (Mathf.Sin(lenght * this.infoPool.ivyParameters.radiusVarFreq + this.infoPool.ivyParameters.radiusVarOffset) + 1f) / 2f;
			return Mathf.Lerp(this.infoPool.ivyParameters.minRadius, this.infoPool.ivyParameters.maxRadius, t);
		}

		// Token: 0x060005B2 RID: 1458 RVA: 0x000A2F78 File Offset: 0x000A1178
		private float GetTipInfluence(float lenght, float totalLenght)
		{
			float result = 1f;
			if (lenght - 0.1f >= totalLenght - this.infoPool.ivyParameters.tipInfluence)
			{
				result = Mathf.InverseLerp(totalLenght, totalLenght - this.infoPool.ivyParameters.tipInfluence, lenght - 0.1f);
			}
			return result;
		}

		// Token: 0x060005B3 RID: 1459 RVA: 0x000A2FC8 File Offset: 0x000A11C8
		private void TriangulateBranch(int b, ref int triCount, int vertCount, int lastVertCount)
		{
			for (int i = 0; i < this.infoPool.ivyContainer.branches[b].branchPoints.Count - 2; i++)
			{
				for (int j = 0; j < this.infoPool.ivyParameters.sides; j++)
				{
					this.trisBranches[triCount] = j + i * (this.infoPool.ivyParameters.sides + 1) + vertCount - lastVertCount;
					this.trisBranches[triCount + 1] = j + i * (this.infoPool.ivyParameters.sides + 1) + 1 + vertCount - lastVertCount;
					this.trisBranches[triCount + 2] = j + i * (this.infoPool.ivyParameters.sides + 1) + this.infoPool.ivyParameters.sides + 1 + vertCount - lastVertCount;
					this.trisBranches[triCount + 3] = j + i * (this.infoPool.ivyParameters.sides + 1) + 1 + vertCount - lastVertCount;
					this.trisBranches[triCount + 4] = j + i * (this.infoPool.ivyParameters.sides + 1) + this.infoPool.ivyParameters.sides + 2 + vertCount - lastVertCount;
					this.trisBranches[triCount + 5] = j + i * (this.infoPool.ivyParameters.sides + 1) + this.infoPool.ivyParameters.sides + 1 + vertCount - lastVertCount;
					triCount += 6;
				}
			}
			int k = 0;
			int num = 0;
			while (k < this.infoPool.ivyParameters.sides * 3)
			{
				this.trisBranches[triCount] = vertCount - 1;
				this.trisBranches[triCount + 1] = vertCount - 3 - num;
				this.trisBranches[triCount + 2] = vertCount - 2 - num;
				triCount += 3;
				k += 3;
				num++;
			}
		}

		// Token: 0x04000635 RID: 1589
		public InfoPool infoPool;

		// Token: 0x04000636 RID: 1590
		public Mesh ivyMesh;

		// Token: 0x04000637 RID: 1591
		private Dictionary<int, int[]> branchesLeavesIndices = new Dictionary<int, int[]>();

		// Token: 0x04000638 RID: 1592
		public Vector3[] verts;

		// Token: 0x04000639 RID: 1593
		private Vector3[] normals;

		// Token: 0x0400063A RID: 1594
		private Vector2[] uvs;

		// Token: 0x0400063B RID: 1595
		private Color[] vColor;

		// Token: 0x0400063C RID: 1596
		private int[] trisBranches;

		// Token: 0x0400063D RID: 1597
		private List<List<int>> trisLeaves;

		// Token: 0x0400063E RID: 1598
		private float angle;

		// Token: 0x0400063F RID: 1599
		public List<Material> leavesMaterials;

		// Token: 0x04000640 RID: 1600
		public List<List<int>> typesByMat;

		// Token: 0x04000641 RID: 1601
		public Rect[] uv2Rects = new Rect[0];

		// Token: 0x04000642 RID: 1602
		public bool leavesDataInitialized;
	}
}
