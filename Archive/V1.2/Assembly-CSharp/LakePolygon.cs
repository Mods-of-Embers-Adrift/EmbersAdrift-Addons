using System;
using System.Collections.Generic;
using AwesomeTechnologies.VegetationSystem.Biomes;
using TriangleNet;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Topology;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000014 RID: 20
[RequireComponent(typeof(MeshFilter))]
public class LakePolygon : MonoBehaviour
{
	// Token: 0x0600003B RID: 59 RVA: 0x000448A4 File Offset: 0x00042AA4
	public void AddPoint(Vector3 position)
	{
		this.points.Add(position);
	}

	// Token: 0x0600003C RID: 60 RVA: 0x000890F8 File Offset: 0x000872F8
	public void AddPointAfter(int i)
	{
		Vector3 vector = this.points[i];
		if (i < this.points.Count - 1 && this.points.Count > i + 1)
		{
			Vector3 vector2 = this.points[i + 1];
			if (Vector3.Distance(vector2, vector) > 0f)
			{
				vector = (vector + vector2) * 0.5f;
			}
			else
			{
				vector.x += 1f;
			}
		}
		else if (this.points.Count > 1 && i == this.points.Count - 1)
		{
			Vector3 vector3 = this.points[i - 1];
			if (Vector3.Distance(vector3, vector) > 0f)
			{
				vector += vector - vector3;
			}
			else
			{
				vector.x += 1f;
			}
		}
		else
		{
			vector.x += 1f;
		}
		this.points.Insert(i + 1, vector);
	}

	// Token: 0x0600003D RID: 61 RVA: 0x000448B2 File Offset: 0x00042AB2
	public void ChangePointPosition(int i, Vector3 position)
	{
		this.points[i] = position;
	}

	// Token: 0x0600003E RID: 62 RVA: 0x000448C1 File Offset: 0x00042AC1
	public void RemovePoint(int i)
	{
		if (i < this.points.Count)
		{
			this.points.RemoveAt(i);
		}
	}

	// Token: 0x0600003F RID: 63 RVA: 0x000891F4 File Offset: 0x000873F4
	public void RemovePoints(int fromID = -1)
	{
		for (int i = this.points.Count - 1; i > fromID; i--)
		{
			this.RemovePoint(i);
		}
	}

	// Token: 0x06000040 RID: 64 RVA: 0x00089220 File Offset: 0x00087420
	private void CenterPivot()
	{
		Vector3 position = base.transform.position;
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < this.points.Count; i++)
		{
			vector += this.points[i];
		}
		vector /= (float)this.points.Count;
		for (int j = 0; j < this.points.Count; j++)
		{
			Vector3 value = this.points[j];
			value.x -= vector.x;
			value.y -= vector.y;
			value.z -= vector.z;
			this.points[j] = value;
		}
		base.transform.position += vector;
	}

	// Token: 0x06000041 RID: 65 RVA: 0x000892F8 File Offset: 0x000874F8
	public void GeneratePolygon(bool quick = false)
	{
		MeshRenderer component = base.gameObject.GetComponent<MeshRenderer>();
		if (component != null)
		{
			component.receiveShadows = this.receiveShadows;
			component.shadowCastingMode = this.shadowCastingMode;
		}
		if (this.lockHeight)
		{
			for (int i = 1; i < this.points.Count; i++)
			{
				Vector3 value = this.points[i];
				value.y = this.points[0].y;
				this.points[i] = value;
			}
		}
		if (this.points.Count < 3)
		{
			return;
		}
		this.CenterPivot();
		this.splinePoints.Clear();
		for (int j = 0; j < this.points.Count; j++)
		{
			this.CalculateCatmullRomSpline(j);
		}
		List<Vector3> list = new List<Vector3>();
		List<Vector3> list2 = new List<Vector3>();
		List<int> list3 = new List<int>();
		list.AddRange(this.splinePoints.ToArray());
		Polygon polygon = new Polygon();
		List<Vertex> list4 = new List<Vertex>();
		for (int k = 0; k < list.Count; k++)
		{
			list4.Add(new Vertex((double)list[k].x, (double)list[k].z)
			{
				z = (double)list[k].y
			});
		}
		polygon.Add(new Contour(list4), false);
		ConstraintOptions options = new ConstraintOptions
		{
			ConformingDelaunay = true
		};
		QualityOptions quality = new QualityOptions
		{
			MinimumAngle = 90.0,
			MaximumArea = (double)this.maximumTriangleSize
		};
		TriangleNet.Mesh mesh = (TriangleNet.Mesh)polygon.Triangulate(options, quality);
		polygon.Triangulate(options, quality);
		list3.Clear();
		foreach (Triangle triangle in mesh.triangles)
		{
			Vertex vertex = mesh.vertices[triangle.vertices[2].id];
			Vector3 item = new Vector3((float)vertex.x, (float)vertex.z, (float)vertex.y);
			vertex = mesh.vertices[triangle.vertices[1].id];
			Vector3 item2 = new Vector3((float)vertex.x, (float)vertex.z, (float)vertex.y);
			vertex = mesh.vertices[triangle.vertices[0].id];
			Vector3 item3 = new Vector3((float)vertex.x, (float)vertex.z, (float)vertex.y);
			list3.Add(list2.Count);
			list3.Add(list2.Count + 1);
			list3.Add(list2.Count + 2);
			list2.Add(item);
			list2.Add(item2);
			list2.Add(item3);
		}
		Vector3[] array = list2.ToArray();
		int num = array.Length;
		Vector3[] array2 = new Vector3[num];
		Vector2[] array3 = new Vector2[num];
		this.colors = new Color[num];
		for (int l = 0; l < num; l++)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(array[l] + base.transform.position + Vector3.up * 10f, Vector3.down, out raycastHit, 1000f, this.snapMask.value) && this.snapToTerrain)
			{
				array[l] = raycastHit.point - base.transform.position + new Vector3(0f, 0.1f, 0f);
			}
			Vector3[] array4 = array;
			int num2 = l;
			array4[num2].y = array4[num2].y + this.yOffset;
			if (this.normalFromRaycast)
			{
				array2[l] = raycastHit.normal;
			}
			else
			{
				array2[l] = Vector3.up;
			}
			array3[l] = new Vector2(array[l].x, array[l].z) * 0.01f * this.uvScale;
			this.colors[l] = Color.black;
		}
		if (this.overrideFlowMap || quick)
		{
			while (this.colorsFlowMap.Count < num)
			{
				this.colorsFlowMap.Add(new Vector2(0f, 1f));
			}
			while (this.colorsFlowMap.Count > num)
			{
				this.colorsFlowMap.RemoveAt(this.colorsFlowMap.Count - 1);
			}
		}
		else
		{
			List<Vector2> list5 = new List<Vector2>();
			List<Vector2> list6 = new List<Vector2>();
			for (int m = 0; m < this.splinePoints.Count; m++)
			{
				list5.Add(new Vector2(this.splinePoints[m].x, this.splinePoints[m].z));
			}
			for (int n = 0; n < array.Length; n++)
			{
				list6.Add(new Vector2(array[n].x, array[n].z));
			}
			this.colorsFlowMap.Clear();
			Vector2 item4 = Vector2.zero;
			for (int num3 = 0; num3 < num; num3++)
			{
				float num4 = float.MaxValue;
				Vector2 a = list6[num3];
				for (int num5 = 0; num5 < this.splinePoints.Count; num5++)
				{
					int index = num5;
					int index2 = (num5 + 1) % list5.Count;
					Vector2 vector;
					float num6 = this.DistancePointLine(list6[num3], list5[index], list5[index2], out vector);
					if (num4 > num6)
					{
						num4 = num6;
						a = vector;
					}
				}
				item4 = -(a - list6[num3]).normalized * (this.automaticFlowMapScale + (this.noiseflowMap ? (Mathf.PerlinNoise(list6[num3].x * this.noiseSizeXflowMap, list6[num3].y * this.noiseSizeZflowMap) * this.noiseMultiplierflowMap - this.noiseMultiplierflowMap * 0.5f) : 0f));
				this.colorsFlowMap.Add(item4);
			}
		}
		this.currentMesh = new UnityEngine.Mesh();
		this.vertsGenerated = (float)num;
		if (num > 65000)
		{
			this.currentMesh.indexFormat = IndexFormat.UInt32;
		}
		this.currentMesh.vertices = array;
		this.currentMesh.subMeshCount = 1;
		this.currentMesh.SetTriangles(list3, 0);
		this.currentMesh.uv = array3;
		this.currentMesh.uv4 = this.colorsFlowMap.ToArray();
		this.currentMesh.normals = array2;
		this.currentMesh.colors = this.colors;
		this.currentMesh.RecalculateTangents();
		this.currentMesh.RecalculateBounds();
		this.currentMesh.RecalculateTangents();
		this.currentMesh.RecalculateBounds();
		this.trianglesGenerated = (float)(list3.Count / 3);
		this.meshfilter = base.GetComponent<MeshFilter>();
		this.meshfilter.sharedMesh = this.currentMesh;
		MeshCollider component2 = base.GetComponent<MeshCollider>();
		if (component2 != null)
		{
			component2.sharedMesh = this.currentMesh;
		}
	}

	// Token: 0x06000042 RID: 66 RVA: 0x00089A80 File Offset: 0x00087C80
	public static LakePolygon CreatePolygon(Material material, List<Vector3> positions = null)
	{
		GameObject gameObject = new GameObject("Lake Polygon");
		gameObject.layer = LayerMask.NameToLayer("Water");
		LakePolygon lakePolygon = gameObject.AddComponent<LakePolygon>();
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.receiveShadows = false;
		meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		if (material != null)
		{
			meshRenderer.sharedMaterial = material;
		}
		if (positions != null)
		{
			for (int i = 0; i < positions.Count; i++)
			{
				lakePolygon.AddPoint(positions[i]);
			}
		}
		return lakePolygon;
	}

	// Token: 0x06000043 RID: 67 RVA: 0x00089AF4 File Offset: 0x00087CF4
	private void CalculateCatmullRomSpline(int pos)
	{
		Vector3 p = this.points[this.ClampListPos(pos - 1)];
		Vector3 p2 = this.points[pos];
		Vector3 p3 = this.points[this.ClampListPos(pos + 1)];
		Vector3 p4 = this.points[this.ClampListPos(pos + 2)];
		int num = Mathf.FloorToInt(1f / this.traingleDensity);
		for (int i = 1; i <= num; i++)
		{
			float t = (float)i * this.traingleDensity;
			this.splinePoints.Add(this.GetCatmullRomPosition(t, p, p2, p3, p4));
		}
	}

	// Token: 0x06000044 RID: 68 RVA: 0x000448DD File Offset: 0x00042ADD
	public int ClampListPos(int pos)
	{
		if (pos < 0)
		{
			pos = this.points.Count - 1;
		}
		if (pos > this.points.Count)
		{
			pos = 1;
		}
		else if (pos > this.points.Count - 1)
		{
			pos = 0;
		}
		return pos;
	}

	// Token: 0x06000045 RID: 69 RVA: 0x00089B94 File Offset: 0x00087D94
	private Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		Vector3 a = 2f * p1;
		Vector3 a2 = p2 - p0;
		Vector3 a3 = 2f * p0 - 5f * p1 + 4f * p2 - p3;
		Vector3 a4 = -p0 + 3f * p1 - 3f * p2 + p3;
		return 0.5f * (a + a2 * t + a3 * t * t + a4 * t * t * t);
	}

	// Token: 0x06000046 RID: 70 RVA: 0x00044919 File Offset: 0x00042B19
	public float DistancePointLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd, out Vector2 pointProject)
	{
		pointProject = this.ProjectPointLine(point, lineStart, lineEnd);
		return Vector2.Distance(pointProject, point);
	}

	// Token: 0x06000047 RID: 71 RVA: 0x00089C5C File Offset: 0x00087E5C
	public Vector2 ProjectPointLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
	{
		Vector2 rhs = point - lineStart;
		Vector2 vector = lineEnd - lineStart;
		float magnitude = vector.magnitude;
		Vector2 vector2 = vector;
		if (magnitude > 1E-06f)
		{
			vector2 /= magnitude;
		}
		float d = Mathf.Clamp(Vector2.Dot(vector2, rhs), 0f, magnitude);
		return lineStart + vector2 * d;
	}

	// Token: 0x06000048 RID: 72 RVA: 0x00089CB8 File Offset: 0x00087EB8
	public void TerrainCarve(bool terrainShow = false)
	{
		Terrain[] activeTerrains = Terrain.activeTerrains;
		Physics.autoSyncTransforms = false;
		if (this.meshGOs != null && this.meshGOs.Count > 0)
		{
			foreach (GameObject obj in this.meshGOs)
			{
				UnityEngine.Object.DestroyImmediate(obj);
			}
			this.meshGOs.Clear();
		}
		foreach (Terrain terrain in activeTerrains)
		{
			TerrainData terrainData = terrain.terrainData;
			float y = base.transform.position.y;
			float y2 = terrain.transform.position.y;
			float x = terrain.terrainData.size.x;
			float y3 = terrain.terrainData.size.y;
			float z = terrain.terrainData.size.z;
			float[,] heights;
			if (this.lakePolygonCarveData == null || this.distSmooth != this.lakePolygonCarveData.distSmooth)
			{
				float num = float.MaxValue;
				float num2 = float.MinValue;
				float num3 = float.MaxValue;
				float num4 = float.MinValue;
				for (int j = 0; j < this.splinePoints.Count; j++)
				{
					Vector3 vector = base.transform.TransformPoint(this.splinePoints[j]);
					if (num > vector.x)
					{
						num = vector.x;
					}
					if (num2 < vector.x)
					{
						num2 = vector.x;
					}
					if (num3 > vector.z)
					{
						num3 = vector.z;
					}
					if (num4 < vector.z)
					{
						num4 = vector.z;
					}
				}
				float num5 = 1f / z * (float)(terrainData.heightmapResolution - 1);
				float num6 = 1f / x * (float)(terrainData.heightmapResolution - 1);
				num -= terrain.transform.position.x + this.distSmooth;
				num2 -= terrain.transform.position.x - this.distSmooth;
				num3 -= terrain.transform.position.z + this.distSmooth;
				num4 -= terrain.transform.position.z - this.distSmooth;
				num *= num6;
				num2 *= num6;
				num3 *= num5;
				num4 *= num5;
				num = (float)((int)Mathf.Clamp(num, 0f, (float)terrainData.heightmapResolution));
				num2 = (float)((int)Mathf.Clamp(num2, 0f, (float)terrainData.heightmapResolution));
				num3 = (float)((int)Mathf.Clamp(num3, 0f, (float)terrainData.heightmapResolution));
				num4 = (float)((int)Mathf.Clamp(num4, 0f, (float)terrainData.heightmapResolution));
				heights = terrainData.GetHeights((int)num, (int)num3, (int)(num2 - num), (int)(num4 - num3));
				Vector4[,] array2 = new Vector4[heights.GetLength(0), heights.GetLength(1)];
				MeshCollider meshCollider = base.gameObject.AddComponent<MeshCollider>();
				Transform transform = terrain.transform;
				Vector3 zero = Vector3.zero;
				zero.y = y;
				for (int k = 0; k < heights.GetLength(0); k++)
				{
					for (int l = 0; l < heights.GetLength(1); l++)
					{
						zero.x = ((float)l + num) / num6 + transform.position.x;
						zero.z = ((float)k + num3) / num5 + transform.position.z;
						Ray ray = new Ray(zero + Vector3.up * 1000f, Vector3.down);
						RaycastHit raycastHit;
						if (meshCollider.Raycast(ray, out raycastHit, 10000f))
						{
							float num7 = float.MaxValue;
							for (int m = 0; m < this.splinePoints.Count; m++)
							{
								int index = m;
								int index2 = (m + 1) % this.splinePoints.Count;
								float num8 = LakePolygon.DistancePointLine(raycastHit.point, base.transform.TransformPoint(this.splinePoints[index]), base.transform.TransformPoint(this.splinePoints[index2]));
								if (num7 > num8)
								{
									num7 = num8;
								}
							}
							array2[k, l] = new Vector3(raycastHit.point.x, num7, raycastHit.point.z);
						}
						else
						{
							float num9 = float.MaxValue;
							for (int n = 0; n < this.splinePoints.Count; n++)
							{
								int index3 = n;
								int index4 = (n + 1) % this.splinePoints.Count;
								float num10 = LakePolygon.DistancePointLine(zero, base.transform.TransformPoint(this.splinePoints[index3]), base.transform.TransformPoint(this.splinePoints[index4]));
								if (num9 > num10)
								{
									num9 = num10;
								}
							}
							array2[k, l] = new Vector3(zero.x, -num9, zero.z);
						}
					}
				}
				UnityEngine.Object.DestroyImmediate(meshCollider);
				this.lakePolygonCarveData = new LakePolygonCarveData();
				this.lakePolygonCarveData.minX = num;
				this.lakePolygonCarveData.maxX = num2;
				this.lakePolygonCarveData.minZ = num3;
				this.lakePolygonCarveData.maxZ = num4;
				this.lakePolygonCarveData.distances = array2;
			}
			heights = terrainData.GetHeights((int)this.lakePolygonCarveData.minX, (int)this.lakePolygonCarveData.minZ, (int)(this.lakePolygonCarveData.maxX - this.lakePolygonCarveData.minX), (int)(this.lakePolygonCarveData.maxZ - this.lakePolygonCarveData.minZ));
			List<List<Vector4>> list = new List<List<Vector4>>();
			for (int num11 = 0; num11 < heights.GetLength(0); num11++)
			{
				List<Vector4> list2 = new List<Vector4>();
				for (int num12 = 0; num12 < heights.GetLength(1); num12++)
				{
					Vector3 vector2 = this.lakePolygonCarveData.distances[num11, num12];
					if (vector2.y > 0f)
					{
						float num13;
						if (this.noiseCarve)
						{
							num13 = Mathf.PerlinNoise((float)num11 * this.noiseSizeX, (float)num12 * this.noiseSizeZ) * this.noiseMultiplierInside - this.noiseMultiplierInside * 0.5f;
						}
						else
						{
							num13 = 0f;
						}
						float y4 = vector2.y;
						heights[num11, num12] = (num13 + y + this.terrainCarve.Evaluate(y4) - y2) / y3;
						list2.Add(new Vector4(vector2.x, heights[num11, num12] * y3 + y2, vector2.z, 1f));
					}
					else if (-vector2.y <= this.distSmooth)
					{
						float num13;
						if (this.noiseCarve)
						{
							num13 = Mathf.PerlinNoise((float)num11 * this.noiseSizeX, (float)num12 * this.noiseSizeZ) * this.noiseMultiplierOutside - this.noiseMultiplierOutside * 0.5f;
						}
						else
						{
							num13 = 0f;
						}
						float b = heights[num11, num12] * y3 + y2;
						float num14 = y + this.terrainCarve.Evaluate(vector2.y);
						float num15 = -vector2.y / this.distSmooth;
						num15 = Mathf.Pow(num15, this.terrainSmoothMultiplier);
						num14 = num13 + Mathf.Lerp(num14, b, num15) - y2;
						heights[num11, num12] = num14 / y3;
						list2.Add(new Vector4(vector2.x, heights[num11, num12] * y3 + y2, vector2.z, Mathf.Pow(1f + vector2.y / this.distSmooth, 0.5f)));
					}
					else
					{
						list2.Add(new Vector4(vector2.x, heights[num11, num12] * y3 + y2, vector2.z, 0f));
					}
				}
				list.Add(list2);
			}
			if (terrainShow)
			{
				UnityEngine.Mesh mesh = new UnityEngine.Mesh();
				mesh.indexFormat = IndexFormat.UInt32;
				List<Vector3> list3 = new List<Vector3>();
				List<int> list4 = new List<int>();
				List<Color> list5 = new List<Color>();
				foreach (List<Vector4> list6 in list)
				{
					foreach (Vector4 vector3 in list6)
					{
						list3.Add(vector3);
						list5.Add(new Color(vector3.w, vector3.w, vector3.w, vector3.w));
					}
				}
				for (int num16 = 0; num16 < list.Count - 1; num16++)
				{
					List<Vector4> list7 = list[num16];
					for (int num17 = 0; num17 < list7.Count - 1; num17++)
					{
						list4.Add(num17 + num16 * list7.Count);
						list4.Add(num17 + (num16 + 1) * list7.Count);
						list4.Add(num17 + 1 + num16 * list7.Count);
						list4.Add(num17 + 1 + num16 * list7.Count);
						list4.Add(num17 + (num16 + 1) * list7.Count);
						list4.Add(num17 + 1 + (num16 + 1) * list7.Count);
					}
				}
				mesh.SetVertices(list3);
				mesh.SetTriangles(list4, 0);
				mesh.SetColors(list5);
				mesh.RecalculateNormals();
				mesh.RecalculateTangents();
				mesh.RecalculateBounds();
				GameObject gameObject = new GameObject("TerrainMesh");
				gameObject.transform.parent = base.transform;
				gameObject.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
				meshRenderer.sharedMaterial = new Material(Shader.Find("Debug Terrain Carve"));
				meshRenderer.sharedMaterial.color = new Color(0f, 0.5f, 0f);
				gameObject.transform.position = Vector3.zero;
				gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;
				if (this.overrideLakeRender)
				{
					gameObject.GetComponent<MeshRenderer>().sharedMaterial.renderQueue = 5000;
				}
				else
				{
					gameObject.GetComponent<MeshRenderer>().sharedMaterial.renderQueue = 2980;
				}
				this.meshGOs.Add(gameObject);
			}
			else
			{
				if (this.meshGOs != null && this.meshGOs.Count > 0)
				{
					foreach (GameObject obj2 in this.meshGOs)
					{
						UnityEngine.Object.DestroyImmediate(obj2);
					}
					this.meshGOs.Clear();
				}
				terrainData.SetHeights((int)this.lakePolygonCarveData.minX, (int)this.lakePolygonCarveData.minZ, heights);
				terrain.Flush();
				this.lakePolygonCarveData = null;
			}
		}
		Physics.autoSyncTransforms = true;
	}

	// Token: 0x06000049 RID: 73 RVA: 0x0008A814 File Offset: 0x00088A14
	public void TerrainPaint(bool terrainShow = false)
	{
		Terrain[] activeTerrains = Terrain.activeTerrains;
		Physics.autoSyncTransforms = false;
		if (this.meshGOs != null && this.meshGOs.Count > 0)
		{
			foreach (GameObject obj in this.meshGOs)
			{
				UnityEngine.Object.DestroyImmediate(obj);
			}
			this.meshGOs.Clear();
		}
		float num = this.distSmooth;
		float num2 = float.MaxValue;
		foreach (Keyframe keyframe in this.terrainPaintCarve.keys)
		{
			if (keyframe.time < num2)
			{
				num2 = keyframe.time;
			}
		}
		if (num2 < 0f)
		{
			num = -num2;
		}
		foreach (Terrain terrain in activeTerrains)
		{
			TerrainData terrainData = terrain.terrainData;
			float y = base.transform.position.y;
			float x = terrain.terrainData.size.x;
			float z = terrain.terrainData.size.z;
			float[,,] alphamaps;
			if (this.lakePolygonPaintData == null || num != this.lakePolygonPaintData.distSmooth)
			{
				float num3 = float.MaxValue;
				float num4 = float.MinValue;
				float num5 = float.MaxValue;
				float num6 = float.MinValue;
				for (int j = 0; j < this.splinePoints.Count; j++)
				{
					Vector3 vector = base.transform.TransformPoint(this.splinePoints[j]);
					if (num3 > vector.x)
					{
						num3 = vector.x;
					}
					if (num4 < vector.x)
					{
						num4 = vector.x;
					}
					if (num5 > vector.z)
					{
						num5 = vector.z;
					}
					if (num6 < vector.z)
					{
						num6 = vector.z;
					}
				}
				float num7 = 1f / z * (float)(terrainData.alphamapWidth - 1);
				float num8 = 1f / x * (float)(terrainData.alphamapHeight - 1);
				Debug.Log(num7.ToString() + " " + num8.ToString());
				num3 -= terrain.transform.position.x + num;
				num4 -= terrain.transform.position.x - num;
				num5 -= terrain.transform.position.z + num;
				num6 -= terrain.transform.position.z - num;
				num3 *= num8;
				num4 *= num8;
				num5 *= num7;
				num6 *= num7;
				num3 = (float)((int)Mathf.Clamp(num3, 0f, (float)terrainData.alphamapWidth));
				num4 = (float)((int)Mathf.Clamp(num4, 0f, (float)terrainData.alphamapWidth));
				num5 = (float)((int)Mathf.Clamp(num5, 0f, (float)terrainData.alphamapHeight));
				num6 = (float)((int)Mathf.Clamp(num6, 0f, (float)terrainData.alphamapHeight));
				alphamaps = terrainData.GetAlphamaps((int)num3, (int)num5, (int)(num4 - num3), (int)(num6 - num5));
				Vector4[,] array2 = new Vector4[alphamaps.GetLength(0), alphamaps.GetLength(1)];
				MeshCollider meshCollider = base.gameObject.AddComponent<MeshCollider>();
				Transform transform = terrain.transform;
				Vector3 zero = Vector3.zero;
				zero.y = y;
				for (int k = 0; k < alphamaps.GetLength(0); k++)
				{
					for (int l = 0; l < alphamaps.GetLength(1); l++)
					{
						zero.x = ((float)l + num3) / num8 + transform.position.x;
						zero.z = ((float)k + num5) / num7 + transform.position.z;
						Ray ray = new Ray(zero + Vector3.up * 1000f, Vector3.down);
						RaycastHit raycastHit;
						if (meshCollider.Raycast(ray, out raycastHit, 10000f))
						{
							float num9 = float.MaxValue;
							for (int m = 0; m < this.splinePoints.Count; m++)
							{
								int index = m;
								int index2 = (m + 1) % this.splinePoints.Count;
								float num10 = LakePolygon.DistancePointLine(raycastHit.point, base.transform.TransformPoint(this.splinePoints[index]), base.transform.TransformPoint(this.splinePoints[index2]));
								if (num9 > num10)
								{
									num9 = num10;
								}
							}
							float w = 0f;
							if (this.addCliffSplatMap)
							{
								ray = new Ray(zero + Vector3.up * 1000f, Vector3.down);
								if (terrain.GetComponent<TerrainCollider>().Raycast(ray, out raycastHit, 10000f))
								{
									w = Vector3.Angle(raycastHit.normal, Vector3.up);
								}
							}
							array2[k, l] = new Vector4(raycastHit.point.x, num9, raycastHit.point.z, w);
						}
						else
						{
							float num11 = float.MaxValue;
							for (int n = 0; n < this.splinePoints.Count; n++)
							{
								int index3 = n;
								int index4 = (n + 1) % this.splinePoints.Count;
								float num12 = LakePolygon.DistancePointLine(zero, base.transform.TransformPoint(this.splinePoints[index3]), base.transform.TransformPoint(this.splinePoints[index4]));
								if (num11 > num12)
								{
									num11 = num12;
								}
							}
							float w2 = 0f;
							if (this.addCliffSplatMap)
							{
								ray = new Ray(zero + Vector3.up * 1000f, Vector3.down);
								if (terrain.GetComponent<TerrainCollider>().Raycast(ray, out raycastHit, 10000f))
								{
									w2 = Vector3.Angle(raycastHit.normal, Vector3.up);
								}
							}
							array2[k, l] = new Vector4(zero.x, -num11, zero.z, w2);
						}
					}
				}
				UnityEngine.Object.DestroyImmediate(meshCollider);
				this.lakePolygonPaintData = new LakePolygonCarveData();
				this.lakePolygonPaintData.minX = num3;
				this.lakePolygonPaintData.maxX = num4;
				this.lakePolygonPaintData.minZ = num5;
				this.lakePolygonPaintData.maxZ = num6;
				this.lakePolygonPaintData.distances = array2;
			}
			alphamaps = terrainData.GetAlphamaps((int)this.lakePolygonPaintData.minX, (int)this.lakePolygonPaintData.minZ, (int)(this.lakePolygonPaintData.maxX - this.lakePolygonPaintData.minX), (int)(this.lakePolygonPaintData.maxZ - this.lakePolygonPaintData.minZ));
			new List<List<Vector4>>();
			for (int num13 = 0; num13 < alphamaps.GetLength(0); num13++)
			{
				new List<Vector4>();
				for (int num14 = 0; num14 < alphamaps.GetLength(1); num14++)
				{
					Vector4 vector2 = this.lakePolygonPaintData.distances[num13, num14];
					if (-vector2.y <= num || vector2.y > 0f)
					{
						if (!this.mixTwoSplatMaps)
						{
							float num15;
							if (this.noisePaint)
							{
								if (vector2.y > 0f)
								{
									num15 = Mathf.PerlinNoise((float)num13 * this.noiseSizeXPaint, (float)num14 * this.noiseSizeZPaint) * this.noiseMultiplierInsidePaint - this.noiseMultiplierInsidePaint * 0.5f;
								}
								else
								{
									num15 = Mathf.PerlinNoise((float)num13 * this.noiseSizeXPaint, (float)num14 * this.noiseSizeZPaint) * this.noiseMultiplierOutsidePaint - this.noiseMultiplierOutsidePaint * 0.5f;
								}
							}
							else
							{
								num15 = 0f;
							}
							float num16 = alphamaps[num13, num14, this.currentSplatMap];
							alphamaps[num13, num14, this.currentSplatMap] = Mathf.Clamp01(Mathf.Lerp(alphamaps[num13, num14, this.currentSplatMap], 1f, this.terrainPaintCarve.Evaluate(vector2.y) + num15 * this.terrainPaintCarve.Evaluate(vector2.y)));
							for (int num17 = 0; num17 < terrainData.terrainLayers.Length; num17++)
							{
								if (num17 != this.currentSplatMap)
								{
									alphamaps[num13, num14, num17] = ((num16 == 1f) ? 0f : Mathf.Clamp01(alphamaps[num13, num14, num17] * ((1f - alphamaps[num13, num14, this.currentSplatMap]) / (1f - num16))));
								}
							}
						}
						else
						{
							float num15;
							if (vector2.y > 0f)
							{
								num15 = Mathf.PerlinNoise((float)num13 * this.noiseSizeXPaint, (float)num14 * this.noiseSizeZPaint) * this.noiseMultiplierInsidePaint - this.noiseMultiplierInsidePaint * 0.5f;
							}
							else
							{
								num15 = Mathf.PerlinNoise((float)num13 * this.noiseSizeXPaint, (float)num14 * this.noiseSizeZPaint) * this.noiseMultiplierOutsidePaint - this.noiseMultiplierOutsidePaint * 0.5f;
							}
							float num18 = alphamaps[num13, num14, this.currentSplatMap];
							alphamaps[num13, num14, this.currentSplatMap] = Mathf.Clamp01(Mathf.Lerp(alphamaps[num13, num14, this.currentSplatMap], 1f, this.terrainPaintCarve.Evaluate(vector2.y)));
							for (int num19 = 0; num19 < terrainData.terrainLayers.Length; num19++)
							{
								if (num19 != this.currentSplatMap)
								{
									alphamaps[num13, num14, num19] = ((num18 == 1f) ? 0f : Mathf.Clamp01(alphamaps[num13, num14, num19] * ((1f - alphamaps[num13, num14, this.currentSplatMap]) / (1f - num18))));
								}
							}
							if (num15 > 0f)
							{
								num18 = alphamaps[num13, num14, this.secondSplatMap];
								alphamaps[num13, num14, this.secondSplatMap] = Mathf.Clamp01(Mathf.Lerp(alphamaps[num13, num14, this.secondSplatMap], 1f, num15));
								for (int num20 = 0; num20 < terrainData.terrainLayers.Length; num20++)
								{
									if (num20 != this.secondSplatMap)
									{
										alphamaps[num13, num14, num20] = ((num18 == 1f) ? 0f : Mathf.Clamp01(alphamaps[num13, num14, num20] * ((1f - alphamaps[num13, num14, this.secondSplatMap]) / (1f - num18))));
									}
								}
							}
						}
						if (this.addCliffSplatMap)
						{
							float num21 = alphamaps[num13, num14, this.cliffSplatMap];
							if (vector2.y > 0f)
							{
								if (vector2.w > this.cliffAngle)
								{
									alphamaps[num13, num14, this.cliffSplatMap] = this.cliffBlend;
									for (int num22 = 0; num22 < terrainData.terrainLayers.Length; num22++)
									{
										if (num22 != this.cliffSplatMap)
										{
											alphamaps[num13, num14, num22] = ((num21 == 1f) ? 0f : Mathf.Clamp01(alphamaps[num13, num14, num22] * ((1f - alphamaps[num13, num14, this.cliffSplatMap]) / (1f - num21))));
										}
									}
								}
							}
							else if (vector2.w > this.cliffAngleOutside)
							{
								alphamaps[num13, num14, this.cliffSplatMapOutside] = this.cliffBlendOutside;
								for (int num23 = 0; num23 < terrainData.terrainLayers.Length; num23++)
								{
									if (num23 != this.cliffSplatMapOutside)
									{
										alphamaps[num13, num14, num23] = ((num21 == 1f) ? 0f : Mathf.Clamp01(alphamaps[num13, num14, num23] * ((1f - alphamaps[num13, num14, this.cliffSplatMapOutside]) / (1f - num21))));
									}
								}
							}
						}
					}
				}
			}
			if (this.meshGOs != null && this.meshGOs.Count > 0)
			{
				foreach (GameObject obj2 in this.meshGOs)
				{
					UnityEngine.Object.DestroyImmediate(obj2);
				}
				this.meshGOs.Clear();
			}
			terrainData.SetAlphamaps((int)this.lakePolygonPaintData.minX, (int)this.lakePolygonPaintData.minZ, alphamaps);
			terrain.Flush();
			this.lakePolygonPaintData = null;
		}
		Physics.autoSyncTransforms = true;
	}

	// Token: 0x0600004A RID: 74 RVA: 0x0008B4B8 File Offset: 0x000896B8
	public void TerrainClearTrees(bool details = true)
	{
		Terrain[] activeTerrains = Terrain.activeTerrains;
		Physics.autoSyncTransforms = false;
		if (this.meshGOs != null && this.meshGOs.Count > 0)
		{
			foreach (GameObject obj in this.meshGOs)
			{
				UnityEngine.Object.DestroyImmediate(obj);
			}
			this.meshGOs.Clear();
		}
		foreach (Terrain terrain in activeTerrains)
		{
			TerrainData terrainData = terrain.terrainData;
			Transform transform = terrain.transform;
			float y = base.transform.position.y;
			Vector3 position = terrain.transform.position;
			float x = terrain.terrainData.size.x;
			Vector3 size = terrain.terrainData.size;
			float z = terrain.terrainData.size.z;
			float num = float.MaxValue;
			float num2 = float.MinValue;
			float num3 = float.MaxValue;
			float num4 = float.MinValue;
			for (int j = 0; j < this.splinePoints.Count; j++)
			{
				Vector3 vector = base.transform.TransformPoint(this.splinePoints[j]);
				if (num > vector.x)
				{
					num = vector.x;
				}
				if (num2 < vector.x)
				{
					num2 = vector.x;
				}
				if (num3 > vector.z)
				{
					num3 = vector.z;
				}
				if (num4 < vector.z)
				{
					num4 = vector.z;
				}
			}
			float num5 = 1f / z * (float)(terrainData.detailWidth - 1);
			float num6 = 1f / x * (float)(terrainData.detailHeight - 1);
			num -= terrain.transform.position.x + this.distanceClearFoliage;
			num2 -= terrain.transform.position.x - this.distanceClearFoliage;
			num3 -= terrain.transform.position.z + this.distanceClearFoliage;
			num4 -= terrain.transform.position.z - this.distanceClearFoliage;
			num *= num6;
			num2 *= num6;
			num3 *= num5;
			num4 *= num5;
			num = (float)((int)Mathf.Clamp(num, 0f, (float)terrainData.detailWidth));
			num2 = (float)((int)Mathf.Clamp(num2, 0f, (float)terrainData.detailWidth));
			num3 = (float)((int)Mathf.Clamp(num3, 0f, (float)terrainData.detailHeight));
			num4 = (float)((int)Mathf.Clamp(num4, 0f, (float)terrainData.detailHeight));
			int[,] detailLayer = terrainData.GetDetailLayer((int)num, (int)num3, (int)(num2 - num), (int)(num4 - num3), 0);
			Vector4[,] array2 = new Vector4[detailLayer.GetLength(0), detailLayer.GetLength(1)];
			MeshCollider meshCollider = base.gameObject.AddComponent<MeshCollider>();
			Vector3 zero = Vector3.zero;
			zero.y = y;
			for (int k = 0; k < detailLayer.GetLength(0); k++)
			{
				for (int l = 0; l < detailLayer.GetLength(1); l++)
				{
					zero.x = ((float)l + num) / num6 + transform.position.x;
					zero.z = ((float)k + num3) / num5 + transform.position.z;
					Ray ray = new Ray(zero + Vector3.up * 1000f, Vector3.down);
					RaycastHit raycastHit;
					if (meshCollider.Raycast(ray, out raycastHit, 10000f))
					{
						float num7 = float.MaxValue;
						for (int m = 0; m < this.splinePoints.Count; m++)
						{
							int index = m;
							int index2 = (m + 1) % this.splinePoints.Count;
							float num8 = LakePolygon.DistancePointLine(raycastHit.point, base.transform.TransformPoint(this.splinePoints[index]), base.transform.TransformPoint(this.splinePoints[index2]));
							if (num7 > num8)
							{
								num7 = num8;
							}
						}
						float w = 0f;
						array2[k, l] = new Vector4(raycastHit.point.x, num7, raycastHit.point.z, w);
					}
					else
					{
						float num9 = float.MaxValue;
						for (int n = 0; n < this.splinePoints.Count; n++)
						{
							int index3 = n;
							int index4 = (n + 1) % this.splinePoints.Count;
							float num10 = LakePolygon.DistancePointLine(zero, base.transform.TransformPoint(this.splinePoints[index3]), base.transform.TransformPoint(this.splinePoints[index4]));
							if (num9 > num10)
							{
								num9 = num10;
							}
						}
						float w2 = 0f;
						array2[k, l] = new Vector4(zero.x, -num9, zero.z, w2);
					}
				}
			}
			if (!details)
			{
				List<TreeInstance> list = new List<TreeInstance>();
				TreeInstance[] treeInstances = terrainData.treeInstances;
				zero.y = y;
				foreach (TreeInstance treeInstance in treeInstances)
				{
					zero.x = treeInstance.position.x * x + transform.position.x;
					zero.z = treeInstance.position.z * z + transform.position.z;
					Ray ray2 = new Ray(zero + Vector3.up * 1000f, Vector3.down);
					RaycastHit raycastHit2;
					if (!meshCollider.Raycast(ray2, out raycastHit2, 10000f))
					{
						float num12 = float.MaxValue;
						for (int num13 = 0; num13 < this.splinePoints.Count; num13++)
						{
							int index5 = num13;
							int index6 = (num13 + 1) % this.splinePoints.Count;
							float num14 = LakePolygon.DistancePointLine(zero, base.transform.TransformPoint(this.splinePoints[index5]), base.transform.TransformPoint(this.splinePoints[index6]));
							if (num12 > num14)
							{
								num12 = num14;
							}
						}
						if (num12 > this.distanceClearFoliageTrees)
						{
							list.Add(treeInstance);
						}
					}
				}
				terrainData.treeInstances = list.ToArray();
				UnityEngine.Object.DestroyImmediate(meshCollider);
			}
			this.lakePolygonClearData = new LakePolygonCarveData();
			this.lakePolygonClearData.minX = num;
			this.lakePolygonClearData.maxX = num2;
			this.lakePolygonClearData.minZ = num3;
			this.lakePolygonClearData.maxZ = num4;
			this.lakePolygonClearData.distances = array2;
			if (details)
			{
				for (int num15 = 0; num15 < terrainData.detailPrototypes.Length; num15++)
				{
					detailLayer = terrainData.GetDetailLayer((int)this.lakePolygonClearData.minX, (int)this.lakePolygonClearData.minZ, (int)(this.lakePolygonClearData.maxX - this.lakePolygonClearData.minX), (int)(this.lakePolygonClearData.maxZ - this.lakePolygonClearData.minZ), num15);
					new List<List<Vector4>>();
					for (int num16 = 0; num16 < detailLayer.GetLength(0); num16++)
					{
						new List<Vector4>();
						for (int num17 = 0; num17 < detailLayer.GetLength(1); num17++)
						{
							Vector4 vector2 = this.lakePolygonClearData.distances[num16, num17];
							if (-vector2.y <= this.distanceClearFoliage || vector2.y > 0f)
							{
								detailLayer[num16, num17];
								detailLayer[num16, num17] = 0;
							}
						}
					}
					terrainData.SetDetailLayer((int)this.lakePolygonClearData.minX, (int)this.lakePolygonClearData.minZ, num15, detailLayer);
				}
			}
			if (this.meshGOs != null && this.meshGOs.Count > 0)
			{
				foreach (GameObject obj2 in this.meshGOs)
				{
					UnityEngine.Object.DestroyImmediate(obj2);
				}
				this.meshGOs.Clear();
			}
			terrain.Flush();
			this.lakePolygonClearData = null;
		}
		Physics.autoSyncTransforms = true;
	}

	// Token: 0x0600004B RID: 75 RVA: 0x0008BD08 File Offset: 0x00089F08
	public void Simulation()
	{
		List<Vector3> list = new List<Vector3>();
		list.Add(base.transform.TransformPoint(this.points[0]));
		int num = 1;
		for (int i = 0; i < num; i++)
		{
			List<Vector3> list2 = new List<Vector3>();
			foreach (Vector3 origin in list)
			{
				for (int j = 0; j <= 360; j += this.angleSimulation)
				{
					Ray ray = new Ray(origin, new Vector3(Mathf.Cos((float)j * 0.017453292f), 0f, Mathf.Sin((float)j * 0.017453292f)).normalized);
					RaycastHit raycastHit;
					if (Physics.Raycast(ray, out raycastHit, this.checkDistanceSimulation))
					{
						bool flag = false;
						Vector3 point = raycastHit.point;
						foreach (Vector3 b in list)
						{
							if (Vector3.Distance(point, b) < this.closeDistanceSimulation)
							{
								flag = true;
								break;
							}
						}
						foreach (Vector3 b2 in list2)
						{
							if (Vector3.Distance(point, b2) < this.closeDistanceSimulation)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							list2.Add(point + ray.direction * 0.3f);
						}
					}
					else
					{
						bool flag2 = false;
						Vector3 vector = ray.origin + ray.direction * 50f;
						foreach (Vector3 b3 in list)
						{
							if (Vector3.Distance(vector, b3) < this.closeDistanceSimulation)
							{
								flag2 = true;
								break;
							}
						}
						foreach (Vector3 b4 in list2)
						{
							if (Vector3.Distance(vector, b4) < this.closeDistanceSimulation)
							{
								flag2 = true;
								break;
							}
						}
						if (!flag2)
						{
							list2.Add(vector);
						}
					}
				}
			}
			if (i == 0)
			{
				list.AddRange(list2);
			}
			else
			{
				for (int k = 0; k < list2.Count; k++)
				{
					float num2 = float.MaxValue;
					int num3 = -1;
					Vector3 vector2 = list2[k];
					for (int l = 0; l < list.Count; l++)
					{
						Vector3 l1_p = list[l];
						Vector3 vector3 = list[(l + 1) % list.Count];
						bool flag3 = false;
						for (int m = 0; m < list.Count; m++)
						{
							if (l != m)
							{
								Vector3 l2_p = list[m];
								Vector3 l2_p2 = list[(m + 1) % list.Count];
								if (LakePolygon.AreLinesIntersecting(l1_p, vector2, l2_p, l2_p2, true) || LakePolygon.AreLinesIntersecting(vector2, vector3, l2_p, l2_p2, true))
								{
									flag3 = true;
									break;
								}
							}
						}
						if (!flag3)
						{
							float num4 = Vector3.Distance(vector2, vector3);
							if (num2 > num4)
							{
								num2 = num4;
								num3 = (l + 1) % list.Count;
							}
						}
					}
					if (num3 > -1)
					{
						list.Insert(num3, vector2);
					}
				}
			}
			if (i == 0 && this.removeFirstPointSimulation)
			{
				list.RemoveAt(0);
			}
		}
		this.points.Clear();
		foreach (Vector3 position in list)
		{
			this.points.Add(base.transform.InverseTransformPoint(position));
		}
		this.GeneratePolygon(false);
	}

	// Token: 0x0600004C RID: 76 RVA: 0x0008C164 File Offset: 0x0008A364
	public static bool AreLinesIntersecting(Vector3 l1_p1, Vector3 l1_p2, Vector3 l2_p1, Vector3 l2_p2, bool shouldIncludeEndPoints = true)
	{
		float num = 1E-05f;
		bool result = false;
		float num2 = (l2_p2.z - l2_p1.z) * (l1_p2.x - l1_p1.x) - (l2_p2.x - l2_p1.x) * (l1_p2.z - l1_p1.z);
		if (num2 != 0f)
		{
			float num3 = ((l2_p2.x - l2_p1.x) * (l1_p1.z - l2_p1.z) - (l2_p2.z - l2_p1.z) * (l1_p1.x - l2_p1.x)) / num2;
			float num4 = ((l1_p2.x - l1_p1.x) * (l1_p1.z - l2_p1.z) - (l1_p2.z - l1_p1.z) * (l1_p1.x - l2_p1.x)) / num2;
			if (shouldIncludeEndPoints)
			{
				if (num3 >= 0f + num && num3 <= 1f - num && num4 >= 0f + num && num4 <= 1f - num)
				{
					result = true;
				}
			}
			else if (num3 > 0f + num && num3 < 1f - num && num4 > 0f + num && num4 < 1f - num)
			{
				result = true;
			}
		}
		return result;
	}

	// Token: 0x0600004D RID: 77 RVA: 0x00044938 File Offset: 0x00042B38
	public static float DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
	{
		return Vector3.Distance(LakePolygon.ProjectPointLine(point, lineStart, lineEnd), point);
	}

	// Token: 0x0600004E RID: 78 RVA: 0x0008C290 File Offset: 0x0008A490
	public static Vector3 ProjectPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
	{
		Vector3 rhs = point - lineStart;
		Vector3 vector = lineEnd - lineStart;
		float magnitude = vector.magnitude;
		Vector3 vector2 = vector;
		if (magnitude > 1E-06f)
		{
			vector2 /= magnitude;
		}
		float d = Mathf.Clamp(Vector3.Dot(vector2, rhs), 0f, magnitude);
		return lineStart + vector2 * d;
	}

	// Token: 0x04000042 RID: 66
	public int toolbarInt;

	// Token: 0x04000043 RID: 67
	public LakePolygonProfile currentProfile;

	// Token: 0x04000044 RID: 68
	public LakePolygonProfile oldProfile;

	// Token: 0x04000045 RID: 69
	public List<Vector3> points = new List<Vector3>();

	// Token: 0x04000046 RID: 70
	public List<Vector3> splinePoints = new List<Vector3>();

	// Token: 0x04000047 RID: 71
	public AnimationCurve terrainCarve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(10f, -2f)
	});

	// Token: 0x04000048 RID: 72
	public float distSmooth = 5f;

	// Token: 0x04000049 RID: 73
	public float terrainSmoothMultiplier = 5f;

	// Token: 0x0400004A RID: 74
	public bool overrideLakeRender;

	// Token: 0x0400004B RID: 75
	public float uvScale = 1f;

	// Token: 0x0400004C RID: 76
	public bool receiveShadows;

	// Token: 0x0400004D RID: 77
	public ShadowCastingMode shadowCastingMode;

	// Token: 0x0400004E RID: 78
	public AnimationCurve terrainPaintCarve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x0400004F RID: 79
	public int currentSplatMap = 1;

	// Token: 0x04000050 RID: 80
	public float distanceClearFoliage = 1f;

	// Token: 0x04000051 RID: 81
	public float distanceClearFoliageTrees = 1f;

	// Token: 0x04000052 RID: 82
	public bool mixTwoSplatMaps;

	// Token: 0x04000053 RID: 83
	public int secondSplatMap = 1;

	// Token: 0x04000054 RID: 84
	public bool addCliffSplatMap;

	// Token: 0x04000055 RID: 85
	public int cliffSplatMap = 1;

	// Token: 0x04000056 RID: 86
	public float cliffAngle = 25f;

	// Token: 0x04000057 RID: 87
	public float cliffBlend = 1f;

	// Token: 0x04000058 RID: 88
	public int cliffSplatMapOutside = 1;

	// Token: 0x04000059 RID: 89
	public float cliffAngleOutside = 45f;

	// Token: 0x0400005A RID: 90
	public float cliffBlendOutside = 1f;

	// Token: 0x0400005B RID: 91
	public bool noiseCarve;

	// Token: 0x0400005C RID: 92
	public float noiseMultiplierInside = 1f;

	// Token: 0x0400005D RID: 93
	public float noiseMultiplierOutside = 0.25f;

	// Token: 0x0400005E RID: 94
	public float noiseSizeX = 0.2f;

	// Token: 0x0400005F RID: 95
	public float noiseSizeZ = 0.2f;

	// Token: 0x04000060 RID: 96
	public bool noisePaint;

	// Token: 0x04000061 RID: 97
	public float noiseMultiplierInsidePaint = 1f;

	// Token: 0x04000062 RID: 98
	public float noiseMultiplierOutsidePaint = 0.5f;

	// Token: 0x04000063 RID: 99
	public float noiseSizeXPaint = 0.2f;

	// Token: 0x04000064 RID: 100
	public float noiseSizeZPaint = 0.2f;

	// Token: 0x04000065 RID: 101
	public float maximumTriangleSize = 50f;

	// Token: 0x04000066 RID: 102
	public float traingleDensity = 0.2f;

	// Token: 0x04000067 RID: 103
	public float height;

	// Token: 0x04000068 RID: 104
	public bool lockHeight = true;

	// Token: 0x04000069 RID: 105
	public float yOffset;

	// Token: 0x0400006A RID: 106
	public float trianglesGenerated;

	// Token: 0x0400006B RID: 107
	public float vertsGenerated;

	// Token: 0x0400006C RID: 108
	public UnityEngine.Mesh currentMesh;

	// Token: 0x0400006D RID: 109
	public MeshFilter meshfilter;

	// Token: 0x0400006E RID: 110
	public bool showVertexColors;

	// Token: 0x0400006F RID: 111
	public bool showFlowMap;

	// Token: 0x04000070 RID: 112
	public bool overrideFlowMap;

	// Token: 0x04000071 RID: 113
	public float automaticFlowMapScale = 0.2f;

	// Token: 0x04000072 RID: 114
	public bool noiseflowMap;

	// Token: 0x04000073 RID: 115
	public float noiseMultiplierflowMap = 1f;

	// Token: 0x04000074 RID: 116
	public float noiseSizeXflowMap = 0.2f;

	// Token: 0x04000075 RID: 117
	public float noiseSizeZflowMap = 0.2f;

	// Token: 0x04000076 RID: 118
	public bool drawOnMesh;

	// Token: 0x04000077 RID: 119
	public bool drawOnMeshFlowMap;

	// Token: 0x04000078 RID: 120
	public Color drawColor = Color.black;

	// Token: 0x04000079 RID: 121
	public bool drawColorR = true;

	// Token: 0x0400007A RID: 122
	public bool drawColorG = true;

	// Token: 0x0400007B RID: 123
	public bool drawColorB = true;

	// Token: 0x0400007C RID: 124
	public bool drawColorA = true;

	// Token: 0x0400007D RID: 125
	public bool drawOnMultiple;

	// Token: 0x0400007E RID: 126
	public float opacity = 0.1f;

	// Token: 0x0400007F RID: 127
	public float drawSize = 1f;

	// Token: 0x04000080 RID: 128
	public Material oldMaterial;

	// Token: 0x04000081 RID: 129
	public Color[] colors;

	// Token: 0x04000082 RID: 130
	public List<Vector2> colorsFlowMap = new List<Vector2>();

	// Token: 0x04000083 RID: 131
	public float floatSpeed = 10f;

	// Token: 0x04000084 RID: 132
	public float flowSpeed = 1f;

	// Token: 0x04000085 RID: 133
	public float flowDirection;

	// Token: 0x04000086 RID: 134
	public float closeDistanceSimulation = 5f;

	// Token: 0x04000087 RID: 135
	public int angleSimulation = 5;

	// Token: 0x04000088 RID: 136
	public float checkDistanceSimulation = 50f;

	// Token: 0x04000089 RID: 137
	public bool removeFirstPointSimulation = true;

	// Token: 0x0400008A RID: 138
	public bool normalFromRaycast;

	// Token: 0x0400008B RID: 139
	public bool snapToTerrain;

	// Token: 0x0400008C RID: 140
	public LayerMask snapMask = 1;

	// Token: 0x0400008D RID: 141
	public float biomMaskResolution = 0.5f;

	// Token: 0x0400008E RID: 142
	public float vegetationBlendDistance = 1f;

	// Token: 0x0400008F RID: 143
	public float vegetationMaskSize = 3f;

	// Token: 0x04000090 RID: 144
	public BiomeMaskArea biomeMaskArea;

	// Token: 0x04000091 RID: 145
	public bool refreshMask;

	// Token: 0x04000092 RID: 146
	public LakePolygonCarveData lakePolygonCarveData;

	// Token: 0x04000093 RID: 147
	public LakePolygonCarveData lakePolygonPaintData;

	// Token: 0x04000094 RID: 148
	public LakePolygonCarveData lakePolygonClearData;

	// Token: 0x04000095 RID: 149
	public List<GameObject> meshGOs = new List<GameObject>();
}
