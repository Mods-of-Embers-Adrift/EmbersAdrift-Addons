using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000019 RID: 25
public class MeshColoringRam : MonoBehaviour
{
	// Token: 0x06000058 RID: 88 RVA: 0x0008D174 File Offset: 0x0008B374
	private void Start()
	{
		if (this.colorMeshLive)
		{
			if (MeshColoringRam.ramSplines == null)
			{
				MeshColoringRam.ramSplines = UnityEngine.Object.FindObjectsOfType<RamSpline>();
			}
			if (MeshColoringRam.lakePolygons == null)
			{
				MeshColoringRam.lakePolygons = UnityEngine.Object.FindObjectsOfType<LakePolygon>();
			}
			this.colored = false;
			this.meshFilters = base.gameObject.GetComponentsInChildren<MeshFilter>();
		}
	}

	// Token: 0x06000059 RID: 89 RVA: 0x000449B0 File Offset: 0x00042BB0
	private void Update()
	{
		if (this.colorMeshLive)
		{
			this.ColorMeshLive();
		}
	}

	// Token: 0x0600005A RID: 90 RVA: 0x0008D1C4 File Offset: 0x0008B3C4
	public void ColorMeshLive()
	{
		this.colored = true;
		Ray ray = default(Ray);
		ray.direction = Vector3.up;
		Vector3 b = -Vector3.up * (this.height + this.threshold);
		Color white = Color.white;
		List<MeshCollider> list = new List<MeshCollider>();
		foreach (RamSpline ramSpline in MeshColoringRam.ramSplines)
		{
			list.Add(ramSpline.gameObject.AddComponent<MeshCollider>());
		}
		foreach (LakePolygon lakePolygon in MeshColoringRam.lakePolygons)
		{
			list.Add(lakePolygon.gameObject.AddComponent<MeshCollider>());
		}
		bool queriesHitBackfaces = Physics.queriesHitBackfaces;
		Physics.queriesHitBackfaces = true;
		foreach (MeshFilter meshFilter in this.meshFilters)
		{
			Mesh mesh = meshFilter.sharedMesh;
			if (meshFilter.sharedMesh != null)
			{
				if (!this.colored)
				{
					mesh = UnityEngine.Object.Instantiate<Mesh>(meshFilter.sharedMesh);
					meshFilter.sharedMesh = mesh;
					this.colored = true;
				}
				int num = mesh.vertices.Length;
				Vector3[] vertices = mesh.vertices;
				Color[] array4 = mesh.colors;
				Transform transform = meshFilter.transform;
				float num2 = float.MaxValue;
				Vector3 origin = vertices[0];
				for (int j = 0; j < num; j++)
				{
					vertices[j] = transform.TransformPoint(vertices[j]) + b;
					if (vertices[j].y < num2)
					{
						num2 = vertices[j].y;
						origin = vertices[j];
					}
				}
				if (array4.Length == 0)
				{
					array4 = new Color[num];
					for (int k = 0; k < array4.Length; k++)
					{
						array4[k] = white;
					}
				}
				ray.origin = origin;
				num2 = float.MinValue;
				RaycastHit raycastHit;
				if (Physics.Raycast(ray, out raycastHit, 100f, this.layer))
				{
					num2 = raycastHit.point.y;
				}
				for (int l = 0; l < num; l++)
				{
					if (vertices[l].y < num2)
					{
						float num3 = Mathf.Abs(vertices[l].y - num2);
						if (num3 > this.threshold)
						{
							array4[l].r = 0f;
						}
						else
						{
							array4[l].r = Mathf.Lerp(1f, 0f, num3 / this.threshold);
						}
					}
					else
					{
						array4[l] = white;
					}
				}
				mesh.colors = array4;
			}
		}
		foreach (MeshCollider obj in list)
		{
			UnityEngine.Object.Destroy(obj);
		}
		Physics.queriesHitBackfaces = queriesHitBackfaces;
	}

	// Token: 0x040000C6 RID: 198
	public float height = 0.5f;

	// Token: 0x040000C7 RID: 199
	public float threshold = 0.5f;

	// Token: 0x040000C8 RID: 200
	public bool autoColor = true;

	// Token: 0x040000C9 RID: 201
	public bool newMesh = true;

	// Token: 0x040000CA RID: 202
	public Vector3 oldPosition = Vector3.zero;

	// Token: 0x040000CB RID: 203
	public bool colorMeshLive;

	// Token: 0x040000CC RID: 204
	public LayerMask layer;

	// Token: 0x040000CD RID: 205
	private MeshFilter[] meshFilters;

	// Token: 0x040000CE RID: 206
	private bool colored;

	// Token: 0x040000CF RID: 207
	private static RamSpline[] ramSplines;

	// Token: 0x040000D0 RID: 208
	private static LakePolygon[] lakePolygons;
}
