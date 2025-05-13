using System;
using System.Collections.Generic;
using AwesomeTechnologies.VegetationSystem.Biomes;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200001B RID: 27
[RequireComponent(typeof(MeshFilter))]
public class RamSpline : MonoBehaviour
{
	// Token: 0x06000061 RID: 97 RVA: 0x000449FF File Offset: 0x00042BFF
	public void Start()
	{
		if (this.generateOnStart)
		{
			this.GenerateSpline(null);
		}
	}

	// Token: 0x06000062 RID: 98 RVA: 0x0008DE40 File Offset: 0x0008C040
	public static RamSpline CreateSpline(Material splineMaterial = null, List<Vector4> positions = null, string name = "RamSpline")
	{
		GameObject gameObject = new GameObject(name);
		gameObject.layer = LayerMask.NameToLayer("Water");
		RamSpline ramSpline = gameObject.AddComponent<RamSpline>();
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.receiveShadows = false;
		meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		if (splineMaterial != null)
		{
			meshRenderer.sharedMaterial = splineMaterial;
		}
		if (positions != null)
		{
			for (int i = 0; i < positions.Count; i++)
			{
				ramSpline.AddPoint(positions[i]);
			}
		}
		return ramSpline;
	}

	// Token: 0x06000063 RID: 99 RVA: 0x0008DEB0 File Offset: 0x0008C0B0
	public void AddPoint(Vector4 position)
	{
		if (position.w == 0f)
		{
			if (this.controlPoints.Count > 0)
			{
				position.w = this.controlPoints[this.controlPoints.Count - 1].w;
			}
			else
			{
				position.w = this.width;
			}
		}
		this.controlPointsRotations.Add(Quaternion.identity);
		this.controlPoints.Add(position);
		this.controlPointsSnap.Add(0f);
		this.controlPointsMeshCurves.Add(new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 0f)
		}));
	}

	// Token: 0x06000064 RID: 100 RVA: 0x0008DF7C File Offset: 0x0008C17C
	public void AddPointAfter(int i)
	{
		Vector4 vector;
		if (i == -1)
		{
			vector = this.controlPoints[0];
		}
		else
		{
			vector = this.controlPoints[i];
		}
		if (i < this.controlPoints.Count - 1 && this.controlPoints.Count > i + 1)
		{
			Vector4 vector2 = this.controlPoints[i + 1];
			if (Vector3.Distance(vector2, vector) > 0f)
			{
				vector = (vector + vector2) * 0.5f;
			}
			else
			{
				vector.x += 1f;
			}
		}
		else if (this.controlPoints.Count > 1 && i == this.controlPoints.Count - 1)
		{
			Vector4 vector3 = this.controlPoints[i - 1];
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
		this.controlPoints.Insert(i + 1, vector);
		this.controlPointsRotations.Insert(i + 1, Quaternion.identity);
		this.controlPointsSnap.Insert(i + 1, 0f);
		this.controlPointsMeshCurves.Insert(i + 1, new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 0f)
		}));
	}

	// Token: 0x06000065 RID: 101 RVA: 0x00044A10 File Offset: 0x00042C10
	public void ChangePointPosition(int i, Vector3 position)
	{
		this.ChangePointPosition(i, new Vector4(position.x, position.y, position.z, 0f));
	}

	// Token: 0x06000066 RID: 102 RVA: 0x0008E10C File Offset: 0x0008C30C
	public void ChangePointPosition(int i, Vector4 position)
	{
		Vector4 vector = this.controlPoints[i];
		if (position.w == 0f)
		{
			position.w = vector.w;
		}
		this.controlPoints[i] = position;
	}

	// Token: 0x06000067 RID: 103 RVA: 0x00044A35 File Offset: 0x00042C35
	public void RemovePoint(int i)
	{
		if (i < this.controlPoints.Count)
		{
			this.controlPoints.RemoveAt(i);
			this.controlPointsRotations.RemoveAt(i);
			this.controlPointsMeshCurves.RemoveAt(i);
			this.controlPointsSnap.RemoveAt(i);
		}
	}

	// Token: 0x06000068 RID: 104 RVA: 0x0008E150 File Offset: 0x0008C350
	public void RemovePoints(int fromID = -1)
	{
		for (int i = this.controlPoints.Count - 1; i > fromID; i--)
		{
			this.RemovePoint(i);
		}
	}

	// Token: 0x06000069 RID: 105 RVA: 0x0008E17C File Offset: 0x0008C37C
	public void GenerateBeginningParentBased()
	{
		this.vertsInShape = (int)Mathf.Round((float)(this.beginningSpline.vertsInShape - 1) * (this.beginningMaxWidth - this.beginningMinWidth) + 1f);
		if (this.vertsInShape < 1)
		{
			this.vertsInShape = 1;
		}
		this.beginningConnectionID = this.beginningSpline.points.Count - 1;
		Vector4 vector = this.beginningSpline.controlPoints[this.beginningSpline.controlPoints.Count - 1];
		float num = vector.w;
		num *= this.beginningMaxWidth - this.beginningMinWidth;
		vector = Vector3.Lerp(this.beginningSpline.pointsDown[this.beginningConnectionID], this.beginningSpline.pointsUp[this.beginningConnectionID], this.beginningMinWidth + (this.beginningMaxWidth - this.beginningMinWidth) * 0.5f) + this.beginningSpline.transform.position - base.transform.position;
		vector.w = num;
		this.controlPoints[0] = vector;
		if (!this.uvScaleOverride)
		{
			this.uvScale = this.beginningSpline.uvScale;
		}
	}

	// Token: 0x0600006A RID: 106 RVA: 0x0008E2C0 File Offset: 0x0008C4C0
	public void GenerateEndingParentBased()
	{
		if (this.beginningSpline == null)
		{
			this.vertsInShape = (int)Mathf.Round((float)(this.endingSpline.vertsInShape - 1) * (this.endingMaxWidth - this.endingMinWidth) + 1f);
			if (this.vertsInShape < 1)
			{
				this.vertsInShape = 1;
			}
		}
		this.endingConnectionID = 0;
		Vector4 vector = this.endingSpline.controlPoints[0];
		float num = vector.w;
		num *= this.endingMaxWidth - this.endingMinWidth;
		vector = Vector3.Lerp(this.endingSpline.pointsDown[this.endingConnectionID], this.endingSpline.pointsUp[this.endingConnectionID], this.endingMinWidth + (this.endingMaxWidth - this.endingMinWidth) * 0.5f) + this.endingSpline.transform.position - base.transform.position;
		vector.w = num;
		this.controlPoints[this.controlPoints.Count - 1] = vector;
	}

	// Token: 0x0600006B RID: 107 RVA: 0x0008E3E0 File Offset: 0x0008C5E0
	public void GenerateSpline(List<RamSpline> generatedSplines = null)
	{
		if (generatedSplines == null)
		{
			generatedSplines = new List<RamSpline>();
		}
		if (this.beginningSpline != null && this.beginningSpline.endingSpline != null)
		{
			Debug.LogError("River can't be ending spline and have beginning spline");
			return;
		}
		if (this.endingSpline != null && this.endingSpline.beginningSpline != null)
		{
			Debug.LogError("River can't be begining spline and have ending spline");
			return;
		}
		if (this.beginningSpline)
		{
			this.GenerateBeginningParentBased();
		}
		if (this.endingSpline)
		{
			this.GenerateEndingParentBased();
		}
		List<Vector4> list = new List<Vector4>();
		for (int i = 0; i < this.controlPoints.Count; i++)
		{
			if (i > 0)
			{
				if (Vector3.Distance(this.controlPoints[i], this.controlPoints[i - 1]) > 0f)
				{
					list.Add(this.controlPoints[i]);
				}
			}
			else
			{
				list.Add(this.controlPoints[i]);
			}
		}
		Mesh mesh = new Mesh();
		this.meshfilter = base.GetComponent<MeshFilter>();
		if (list.Count < 2)
		{
			mesh.Clear();
			this.meshfilter.mesh = mesh;
			return;
		}
		this.controlPointsOrientation = new List<Quaternion>();
		this.lerpValues.Clear();
		this.snaps.Clear();
		this.points.Clear();
		this.pointsUp.Clear();
		this.pointsDown.Clear();
		this.carvePointsUp.Clear();
		this.carvePointsDown.Clear();
		this.orientations.Clear();
		this.tangents.Clear();
		this.normalsList.Clear();
		this.widths.Clear();
		this.controlPointsUp.Clear();
		this.controlPointsDown.Clear();
		this.verticesBeginning.Clear();
		this.verticesEnding.Clear();
		this.normalsBeginning.Clear();
		this.normalsEnding.Clear();
		if (this.beginningSpline != null && this.beginningSpline.controlPointsRotations.Count > 0)
		{
			this.controlPointsRotations[0] = Quaternion.identity;
		}
		if (this.endingSpline != null && this.endingSpline.controlPointsRotations.Count > 0)
		{
			this.controlPointsRotations[this.controlPointsRotations.Count - 1] = Quaternion.identity;
		}
		for (int j = 0; j < list.Count; j++)
		{
			if (j <= list.Count - 2)
			{
				this.CalculateCatmullRomSideSplines(list, j);
			}
		}
		if (this.beginningSpline != null && this.beginningSpline.controlPointsRotations.Count > 0)
		{
			this.controlPointsRotations[0] = Quaternion.Inverse(this.controlPointsOrientation[0]) * this.beginningSpline.controlPointsOrientation[this.beginningSpline.controlPointsOrientation.Count - 1];
		}
		if (this.endingSpline != null && this.endingSpline.controlPointsRotations.Count > 0)
		{
			this.controlPointsRotations[this.controlPointsRotations.Count - 1] = Quaternion.Inverse(this.controlPointsOrientation[this.controlPointsOrientation.Count - 1]) * this.endingSpline.controlPointsOrientation[0];
		}
		this.controlPointsOrientation = new List<Quaternion>();
		this.controlPointsUp.Clear();
		this.controlPointsDown.Clear();
		for (int k = 0; k < list.Count; k++)
		{
			if (k <= list.Count - 2)
			{
				this.CalculateCatmullRomSideSplines(list, k);
			}
		}
		for (int l = 0; l < list.Count; l++)
		{
			if (l <= list.Count - 2)
			{
				this.CalculateCatmullRomSplineParameters(list, l, false);
			}
		}
		for (int m = 0; m < this.controlPointsUp.Count; m++)
		{
			if (m <= this.controlPointsUp.Count - 2)
			{
				this.CalculateCatmullRomSpline(this.controlPointsUp, m, ref this.pointsUp);
			}
		}
		for (int n = 0; n < this.controlPointsDown.Count; n++)
		{
			if (n <= this.controlPointsDown.Count - 2)
			{
				this.CalculateCatmullRomSpline(this.controlPointsDown, n, ref this.pointsDown);
			}
		}
		this.traingleDensity /= this.terrainMeshSmoothX;
		for (int num = 0; num < this.controlPointsUp.Count; num++)
		{
			if (num <= this.controlPointsUp.Count - 2)
			{
				this.CalculateCatmullRomSpline(this.controlPointsUp, num, ref this.carvePointsUp);
			}
		}
		for (int num2 = 0; num2 < this.controlPointsDown.Count; num2++)
		{
			if (num2 <= this.controlPointsDown.Count - 2)
			{
				this.CalculateCatmullRomSpline(this.controlPointsDown, num2, ref this.carvePointsDown);
			}
		}
		this.traingleDensity *= this.terrainMeshSmoothX;
		this.GenerateMesh(ref mesh);
		if (generatedSplines != null)
		{
			generatedSplines.Add(this);
			foreach (RamSpline ramSpline in this.beginnigChildSplines)
			{
				if (ramSpline != null && !generatedSplines.Contains(ramSpline) && (ramSpline.beginningSpline == this || ramSpline.endingSpline == this))
				{
					ramSpline.GenerateSpline(generatedSplines);
				}
			}
			foreach (RamSpline ramSpline2 in this.endingChildSplines)
			{
				if (ramSpline2 != null && !generatedSplines.Contains(ramSpline2) && (ramSpline2.beginningSpline == this || ramSpline2.endingSpline == this))
				{
					ramSpline2.GenerateSpline(generatedSplines);
				}
			}
		}
	}

	// Token: 0x0600006C RID: 108 RVA: 0x0008E9E4 File Offset: 0x0008CBE4
	private void CalculateCatmullRomSideSplines(List<Vector4> controlPoints, int pos)
	{
		Vector3 p = controlPoints[pos];
		Vector3 p2 = controlPoints[pos];
		Vector3 p3 = controlPoints[this.ClampListPos(pos + 1)];
		Vector3 p4 = controlPoints[this.ClampListPos(pos + 1)];
		if (pos > 0)
		{
			p = controlPoints[this.ClampListPos(pos - 1)];
		}
		if (pos < controlPoints.Count - 2)
		{
			p4 = controlPoints[this.ClampListPos(pos + 2)];
		}
		int num = 0;
		if (pos == controlPoints.Count - 2)
		{
			num = 1;
		}
		for (int i = 0; i <= num; i++)
		{
			Vector3 catmullRomPosition = this.GetCatmullRomPosition((float)i, p, p2, p3, p4);
			Vector3 normalized = this.GetCatmullRomTangent((float)i, p, p2, p3, p4).normalized;
			Vector3 normalized2 = this.CalculateNormal(normalized, Vector3.up).normalized;
			Quaternion quaternion;
			if (normalized2 == normalized && normalized2 == Vector3.zero)
			{
				quaternion = Quaternion.identity;
			}
			else
			{
				quaternion = Quaternion.LookRotation(normalized, normalized2);
			}
			quaternion *= Quaternion.Lerp(this.controlPointsRotations[pos], this.controlPointsRotations[this.ClampListPos(pos + 1)], (float)i);
			this.controlPointsOrientation.Add(quaternion);
			Vector3 item = catmullRomPosition + quaternion * (0.5f * controlPoints[pos + i].w * Vector3.right);
			Vector3 item2 = catmullRomPosition + quaternion * (0.5f * controlPoints[pos + i].w * Vector3.left);
			this.controlPointsUp.Add(item);
			this.controlPointsDown.Add(item2);
		}
	}

	// Token: 0x0600006D RID: 109 RVA: 0x0008EBB4 File Offset: 0x0008CDB4
	private void CalculateCatmullRomSplineParameters(List<Vector4> controlPoints, int pos, bool initialPoints = false)
	{
		Vector3 p = controlPoints[pos];
		Vector3 p2 = controlPoints[pos];
		Vector3 p3 = controlPoints[this.ClampListPos(pos + 1)];
		Vector3 p4 = controlPoints[this.ClampListPos(pos + 1)];
		if (pos > 0)
		{
			p = controlPoints[this.ClampListPos(pos - 1)];
		}
		if (pos < controlPoints.Count - 2)
		{
			p4 = controlPoints[this.ClampListPos(pos + 2)];
		}
		int num = Mathf.FloorToInt(1f / this.traingleDensity);
		float num2 = 0f;
		if (pos > 0)
		{
			num2 = 1f;
		}
		float num3;
		for (num3 = num2; num3 <= (float)num; num3 += 1f)
		{
			float t = num3 * this.traingleDensity;
			this.CalculatePointParameters(controlPoints, pos, p, p2, p3, p4, t);
		}
		if (num3 < (float)num)
		{
			num3 = (float)num;
			float t2 = num3 * this.traingleDensity;
			this.CalculatePointParameters(controlPoints, pos, p, p2, p3, p4, t2);
		}
	}

	// Token: 0x0600006E RID: 110 RVA: 0x0008ECC0 File Offset: 0x0008CEC0
	private void CalculateCatmullRomSpline(List<Vector3> controlPoints, int pos, ref List<Vector3> points)
	{
		Vector3 p = controlPoints[pos];
		Vector3 p2 = controlPoints[pos];
		Vector3 p3 = controlPoints[this.ClampListPos(pos + 1)];
		Vector3 p4 = controlPoints[this.ClampListPos(pos + 1)];
		if (pos > 0)
		{
			p = controlPoints[this.ClampListPos(pos - 1)];
		}
		if (pos < controlPoints.Count - 2)
		{
			p4 = controlPoints[this.ClampListPos(pos + 2)];
		}
		int num = Mathf.FloorToInt(1f / this.traingleDensity);
		float num2 = 0f;
		if (pos > 0)
		{
			num2 = 1f;
		}
		float num3;
		for (num3 = num2; num3 <= (float)num; num3 += 1f)
		{
			float t = num3 * this.traingleDensity;
			this.CalculatePointPosition(controlPoints, pos, p, p2, p3, p4, t, ref points);
		}
		if (num3 < (float)num)
		{
			num3 = (float)num;
			float t2 = num3 * this.traingleDensity;
			this.CalculatePointPosition(controlPoints, pos, p, p2, p3, p4, t2, ref points);
		}
	}

	// Token: 0x0600006F RID: 111 RVA: 0x0008EDB0 File Offset: 0x0008CFB0
	private void CalculatePointPosition(List<Vector3> controlPoints, int pos, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, ref List<Vector3> points)
	{
		Vector3 catmullRomPosition = this.GetCatmullRomPosition(t, p0, p1, p2, p3);
		points.Add(catmullRomPosition);
		Vector3 normalized = this.GetCatmullRomTangent(t, p0, p1, p2, p3).normalized;
		Vector3 normalized2 = this.CalculateNormal(normalized, Vector3.up).normalized;
	}

	// Token: 0x06000070 RID: 112 RVA: 0x0008EE04 File Offset: 0x0008D004
	private void CalculatePointParameters(List<Vector4> controlPoints, int pos, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		Vector3 catmullRomPosition = this.GetCatmullRomPosition(t, p0, p1, p2, p3);
		this.widths.Add(Mathf.Lerp(controlPoints[pos].w, controlPoints[this.ClampListPos(pos + 1)].w, t));
		if (this.controlPointsSnap.Count > pos + 1)
		{
			this.snaps.Add(Mathf.Lerp(this.controlPointsSnap[pos], this.controlPointsSnap[this.ClampListPos(pos + 1)], t));
		}
		else
		{
			this.snaps.Add(0f);
		}
		this.lerpValues.Add((float)pos + t);
		this.points.Add(catmullRomPosition);
		Vector3 normalized = this.GetCatmullRomTangent(t, p0, p1, p2, p3).normalized;
		Vector3 vector = this.CalculateNormal(normalized, Vector3.up).normalized;
		Quaternion quaternion;
		if (vector == normalized && vector == Vector3.zero)
		{
			quaternion = Quaternion.identity;
		}
		else
		{
			quaternion = Quaternion.LookRotation(normalized, vector);
		}
		quaternion *= Quaternion.Lerp(this.controlPointsRotations[pos], this.controlPointsRotations[this.ClampListPos(pos + 1)], t);
		this.orientations.Add(quaternion);
		this.tangents.Add(normalized);
		if (this.normalsList.Count > 0 && Vector3.Angle(this.normalsList[this.normalsList.Count - 1], vector) > 90f)
		{
			vector *= -1f;
		}
		this.normalsList.Add(vector);
	}

	// Token: 0x06000071 RID: 113 RVA: 0x00044A75 File Offset: 0x00042C75
	private int ClampListPos(int pos)
	{
		if (pos < 0)
		{
			pos = this.controlPoints.Count - 1;
		}
		if (pos > this.controlPoints.Count)
		{
			pos = 1;
		}
		else if (pos > this.controlPoints.Count - 1)
		{
			pos = 0;
		}
		return pos;
	}

	// Token: 0x06000072 RID: 114 RVA: 0x00089B74 File Offset: 0x00087D74
	private Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		Vector3 a = 2f * p1;
		Vector3 a2 = p2 - p0;
		Vector3 a3 = 2f * p0 - 5f * p1 + 4f * p2 - p3;
		Vector3 a4 = -p0 + 3f * p1 - 3f * p2 + p3;
		return 0.5f * (a + a2 * t + a3 * t * t + a4 * t * t * t);
	}

	// Token: 0x06000073 RID: 115 RVA: 0x0008EFAC File Offset: 0x0008D1AC
	private Vector3 GetCatmullRomTangent(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		return 0.5f * (-p0 + p2 + 2f * (2f * p0 - 5f * p1 + 4f * p2 - p3) * t + 3f * (-p0 + 3f * p1 - 3f * p2 + p3) * t * t);
	}

	// Token: 0x06000074 RID: 116 RVA: 0x0008F064 File Offset: 0x0008D264
	private Vector3 CalculateNormal(Vector3 tangent, Vector3 up)
	{
		Vector3 rhs = Vector3.Cross(up, tangent);
		return Vector3.Cross(tangent, rhs);
	}

	// Token: 0x06000075 RID: 117 RVA: 0x0008F080 File Offset: 0x0008D280
	private void GenerateMesh(ref Mesh mesh)
	{
		this.terrainsUnder.Clear();
		MeshRenderer component = base.gameObject.GetComponent<MeshRenderer>();
		if (component != null)
		{
			component.receiveShadows = this.receiveShadows;
			component.shadowCastingMode = this.shadowCastingMode;
		}
		foreach (Transform transform in this.meshesPartTransforms)
		{
			if (transform != null)
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(transform.gameObject);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(transform.gameObject);
				}
				UnityEngine.Object.Destroy(transform.gameObject);
			}
		}
		int num = this.points.Count - 1;
		int count = this.points.Count;
		int num2 = this.vertsInShape * count;
		List<int> list = new List<int>();
		Vector3[] array = new Vector3[num2];
		Vector3[] array2 = new Vector3[num2];
		Vector2[] array3 = new Vector2[num2];
		Vector2[] array4 = new Vector2[num2];
		Vector2[] array5 = new Vector2[num2];
		if (this.colors == null || this.colors.Length != num2)
		{
			this.colors = new Color[num2];
			for (int i = 0; i < this.colors.Length; i++)
			{
				this.colors[i] = Color.black;
			}
		}
		if (this.colorsFlowMap.Count != num2)
		{
			this.colorsFlowMap.Clear();
		}
		this.length = 0f;
		this.fulllength = 0f;
		if (this.beginningSpline != null)
		{
			this.length = this.beginningSpline.length;
		}
		this.minMaxWidth = 1f;
		this.uvWidth = 1f;
		this.uvBeginning = 0f;
		if (this.beginningSpline != null)
		{
			this.minMaxWidth = this.beginningMaxWidth - this.beginningMinWidth;
			this.uvWidth = this.minMaxWidth * this.beginningSpline.uvWidth;
			this.uvBeginning = this.beginningSpline.uvWidth * this.beginningMinWidth + this.beginningSpline.uvBeginning;
		}
		else if (this.endingSpline != null)
		{
			this.minMaxWidth = this.endingMaxWidth - this.endingMinWidth;
			this.uvWidth = this.minMaxWidth * this.endingSpline.uvWidth;
			this.uvBeginning = this.endingSpline.uvWidth * this.endingMinWidth + this.endingSpline.uvBeginning;
		}
		for (int j = 0; j < this.pointsDown.Count; j++)
		{
			float num3 = this.widths[j];
			if (j > 0)
			{
				this.fulllength += this.uvWidth * Vector3.Distance(this.pointsDown[j], this.pointsDown[j - 1]) / (this.uvScale * num3);
			}
		}
		float num4 = Mathf.Round(this.fulllength);
		for (int k = 0; k < this.pointsDown.Count; k++)
		{
			float num5 = this.widths[k];
			int num6 = k * this.vertsInShape;
			if (k > 0)
			{
				this.length += this.uvWidth * Vector3.Distance(this.pointsDown[k], this.pointsDown[k - 1]) / (this.uvScale * num5) / this.fulllength * num4;
			}
			float num7 = 0f;
			float num8 = 0f;
			for (int l = 0; l < this.vertsInShape; l++)
			{
				int num9 = num6 + l;
				float num10 = (float)l / (float)(this.vertsInShape - 1);
				if (num10 < 0.5f)
				{
					num10 *= this.minVal * 2f;
				}
				else
				{
					num10 = ((num10 - 0.5f) * (1f - this.maxVal) + 0.5f * this.maxVal) * 2f;
				}
				if (k == 0 && this.beginningSpline != null && this.beginningSpline.verticesEnding != null && this.beginningSpline.normalsEnding != null)
				{
					int num11 = (int)((float)this.beginningSpline.vertsInShape * this.beginningMinWidth);
					array[num9] = this.beginningSpline.verticesEnding[Mathf.Clamp(l + num11, 0, this.beginningSpline.verticesEnding.Count - 1)] + this.beginningSpline.transform.position - base.transform.position;
				}
				else if (k == this.pointsDown.Count - 1 && this.endingSpline != null && this.endingSpline.verticesBeginning != null && this.endingSpline.verticesBeginning.Count > 0 && this.endingSpline.normalsBeginning != null)
				{
					int num12 = (int)((float)this.endingSpline.vertsInShape * this.endingMinWidth);
					array[num9] = this.endingSpline.verticesBeginning[Mathf.Clamp(l + num12, 0, this.endingSpline.verticesBeginning.Count - 1)] + this.endingSpline.transform.position - base.transform.position;
				}
				else
				{
					array[num9] = Vector3.Lerp(this.pointsDown[k], this.pointsUp[k], num10);
					RaycastHit raycastHit;
					if (Physics.Raycast(array[num9] + base.transform.position + Vector3.up * 5f, Vector3.down, out raycastHit, 1000f, this.snapMask.value))
					{
						Terrain component2 = raycastHit.collider.gameObject.GetComponent<Terrain>();
						if (component2 && !this.terrainsUnder.Contains(component2))
						{
							this.terrainsUnder.Add(component2);
						}
						array[num9] = Vector3.Lerp(array[num9], raycastHit.point - base.transform.position + new Vector3(0f, 0.1f, 0f), (Mathf.Sin(3.1415927f * this.snaps[k] - 1.5707964f) + 1f) * 0.5f);
					}
					RaycastHit raycastHit2;
					if (this.normalFromRaycast && Physics.Raycast(this.points[k] + base.transform.position + Vector3.up * 5f, Vector3.down, out raycastHit2, 1000f, this.snapMask.value))
					{
						array2[num9] = raycastHit2.normal;
					}
					array[num9] += this.orientations[k] * Vector3.up * Mathf.Lerp(this.controlPointsMeshCurves[Mathf.FloorToInt(this.lerpValues[k])].Evaluate(num10), this.controlPointsMeshCurves[Mathf.CeilToInt(this.lerpValues[k])].Evaluate(num10), this.lerpValues[k] - Mathf.Floor(this.lerpValues[k]));
				}
				if (k > 0 && k < 5 && this.beginningSpline != null && this.beginningSpline.verticesEnding != null)
				{
					array[num9].y = (array[num9].y + array[num9 - this.vertsInShape].y) * 0.5f;
				}
				if (k == this.pointsDown.Count - 1 && this.endingSpline != null && this.endingSpline.verticesBeginning != null)
				{
					for (int m = 1; m < 5; m++)
					{
						array[num9 - this.vertsInShape * m].y = (array[num9 - this.vertsInShape * (m - 1)].y + array[num9 - this.vertsInShape * m].y) * 0.5f;
					}
				}
				if (k == 0)
				{
					this.verticesBeginning.Add(array[num9]);
				}
				if (k == this.pointsDown.Count - 1)
				{
					this.verticesEnding.Add(array[num9]);
				}
				if (!this.normalFromRaycast)
				{
					array2[num9] = this.orientations[k] * Vector3.up;
				}
				if (k == 0)
				{
					this.normalsBeginning.Add(array2[num9]);
				}
				if (k == this.pointsDown.Count - 1)
				{
					this.normalsEnding.Add(array2[num9]);
				}
				if (l > 0)
				{
					num7 = num10 * this.uvWidth;
					num8 = num10;
				}
				if (this.beginningSpline != null || this.endingSpline != null)
				{
					num7 += this.uvBeginning;
				}
				num7 /= this.uvScale;
				float num13 = this.FlowCalculate(num8, array2[num9].y, array[num9]);
				int num14 = 10;
				if (this.beginnigChildSplines.Count > 0 && k <= num14)
				{
					float num15 = 0f;
					foreach (RamSpline ramSpline in this.beginnigChildSplines)
					{
						if (!(ramSpline == null) && Mathf.CeilToInt(ramSpline.endingMaxWidth * (float)(this.vertsInShape - 1)) >= l && l >= Mathf.CeilToInt(ramSpline.endingMinWidth * (float)(this.vertsInShape - 1)))
						{
							num15 = (float)(l - Mathf.CeilToInt(ramSpline.endingMinWidth * (float)(this.vertsInShape - 1))) / (float)(Mathf.CeilToInt(ramSpline.endingMaxWidth * (float)(this.vertsInShape - 1)) - Mathf.CeilToInt(ramSpline.endingMinWidth * (float)(this.vertsInShape - 1)));
							num15 = this.FlowCalculate(num15, array2[num9].y, array[num9]);
						}
					}
					if (k > 0)
					{
						num13 = Mathf.Lerp(num13, num15, 1f - (float)k / (float)num14);
					}
					else
					{
						num13 = num15;
					}
				}
				if (k >= this.pointsDown.Count - num14 - 1 && this.endingChildSplines.Count > 0)
				{
					float num16 = 0f;
					foreach (RamSpline ramSpline2 in this.endingChildSplines)
					{
						if (!(ramSpline2 == null) && Mathf.CeilToInt(ramSpline2.beginningMaxWidth * (float)(this.vertsInShape - 1)) >= l && l >= Mathf.CeilToInt(ramSpline2.beginningMinWidth * (float)(this.vertsInShape - 1)))
						{
							num16 = (float)(l - Mathf.CeilToInt(ramSpline2.beginningMinWidth * (float)(this.vertsInShape - 1))) / (float)(Mathf.CeilToInt(ramSpline2.beginningMaxWidth * (float)(this.vertsInShape - 1)) - Mathf.CeilToInt(ramSpline2.beginningMinWidth * (float)(this.vertsInShape - 1)));
							num16 = this.FlowCalculate(num16, array2[num9].y, array[num9]);
						}
					}
					if (k < this.pointsDown.Count - 1)
					{
						num13 = Mathf.Lerp(num13, num16, (float)(k - (this.pointsDown.Count - num14 - 1)) / (float)num14);
					}
					else
					{
						num13 = num16;
					}
				}
				float num17 = -(num8 - 0.5f) * 0.01f;
				this.uv3length = this.length / this.fulllength;
				if (this.beginningSpline != null)
				{
					this.uv3length = (this.length - this.beginningSpline.length) / this.fulllength + this.beginningSpline.uv3length;
				}
				foreach (RamSpline ramSpline3 in this.beginnigChildSplines)
				{
					if (!(ramSpline3 == null))
					{
						this.uv3length = this.length / this.fulllength + ramSpline3.uv3length;
						break;
					}
				}
				if (this.uvRotation)
				{
					if (!this.invertUVDirection)
					{
						array3[num9] = new Vector2(1f - this.length, num7);
						array4[num9] = new Vector2(1f - this.uv3length, num8);
						array5[num9] = new Vector2(num13, num17);
					}
					else
					{
						array3[num9] = new Vector2(1f + this.length, num7);
						array4[num9] = new Vector2(1f + this.uv3length, num8);
						array5[num9] = new Vector2(num13, num17);
					}
				}
				else if (!this.invertUVDirection)
				{
					array3[num9] = new Vector2(num7, 1f - this.length);
					array4[num9] = new Vector2(num8, 1f - this.uv3length);
					array5[num9] = new Vector2(num17, num13);
				}
				else
				{
					array3[num9] = new Vector2(num7, 1f + this.length);
					array4[num9] = new Vector2(num8, 1f + this.uv3length);
					array5[num9] = new Vector2(num17, num13);
				}
				float num18 = (float)((int)(array5[num9].x * 100f));
				array5[num9].x = num18 * 0.01f;
				num18 = (float)((int)(array5[num9].y * 100f));
				array5[num9].y = num18 * 0.01f;
				if (this.colorsFlowMap.Count <= num9)
				{
					this.colorsFlowMap.Add(array5[num9]);
				}
				else if (!this.overrideFlowMap)
				{
					this.colorsFlowMap[num9] = array5[num9];
				}
			}
		}
		for (int n = 0; n < num; n++)
		{
			int num19 = n * this.vertsInShape;
			for (int num20 = 0; num20 < this.vertsInShape - 1; num20++)
			{
				int item = num19 + num20;
				int item2 = num19 + num20 + this.vertsInShape;
				int item3 = num19 + num20 + 1 + this.vertsInShape;
				int item4 = num19 + num20 + 1;
				list.Add(item);
				list.Add(item2);
				list.Add(item3);
				list.Add(item3);
				list.Add(item4);
				list.Add(item);
			}
		}
		this.verticeDirection.Clear();
		for (int num21 = 0; num21 < array.Length - this.vertsInShape; num21++)
		{
			Vector3 normalized = (array[num21 + this.vertsInShape] - array[num21]).normalized;
			if (this.uvRotation)
			{
				normalized = new Vector3(normalized.z, 0f, -normalized.x);
			}
			this.verticeDirection.Add(normalized);
		}
		for (int num22 = array.Length - this.vertsInShape; num22 < array.Length; num22++)
		{
			Vector3 normalized2 = (array[num22] - array[num22 - this.vertsInShape]).normalized;
			if (this.uvRotation)
			{
				normalized2 = new Vector3(normalized2.z, 0f, -normalized2.x);
			}
			this.verticeDirection.Add(normalized2);
		}
		mesh = new Mesh();
		mesh.Clear();
		mesh.vertices = array;
		mesh.normals = array2;
		mesh.uv = array3;
		mesh.uv3 = array4;
		mesh.uv4 = this.colorsFlowMap.ToArray();
		mesh.triangles = list.ToArray();
		mesh.colors = this.colors;
		mesh.RecalculateTangents();
		this.meshfilter.mesh = mesh;
		base.GetComponent<MeshRenderer>().enabled = true;
		if (this.generateMeshParts)
		{
			this.GenerateMeshParts(mesh);
		}
	}

	// Token: 0x06000076 RID: 118 RVA: 0x0009018C File Offset: 0x0008E38C
	public void GenerateMeshParts(Mesh baseMesh)
	{
		foreach (Transform transform in this.meshesPartTransforms)
		{
			if (transform != null)
			{
				UnityEngine.Object.DestroyImmediate(transform.gameObject);
			}
		}
		Vector3[] vertices = baseMesh.vertices;
		Vector3[] normals = baseMesh.normals;
		Vector2[] uv = baseMesh.uv;
		Vector2[] uv2 = baseMesh.uv3;
		base.GetComponent<MeshRenderer>().enabled = false;
		int num = Mathf.RoundToInt((float)(vertices.Length / this.vertsInShape) / (float)this.meshPartsCount) * this.vertsInShape;
		for (int i = 0; i < this.meshPartsCount; i++)
		{
			GameObject gameObject = new GameObject(base.gameObject.name + "- Mesh part " + i.ToString());
			gameObject.transform.SetParent(base.gameObject.transform, false);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localEulerAngles = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
			this.meshesPartTransforms.Add(gameObject.transform);
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.sharedMaterial = base.GetComponent<MeshRenderer>().sharedMaterial;
			meshRenderer.receiveShadows = this.receiveShadows;
			meshRenderer.shadowCastingMode = this.shadowCastingMode;
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			Mesh mesh = new Mesh();
			mesh.Clear();
			List<Vector3> list = new List<Vector3>();
			List<Vector3> list2 = new List<Vector3>();
			List<Vector2> list3 = new List<Vector2>();
			List<Vector2> list4 = new List<Vector2>();
			List<Vector2> list5 = new List<Vector2>();
			List<Color> list6 = new List<Color>();
			List<int> list7 = new List<int>();
			int num2 = num * i + ((i > 0) ? (-this.vertsInShape) : 0);
			while ((num2 < num * (i + 1) && num2 < vertices.Length) || (i == this.meshPartsCount - 1 && num2 < vertices.Length))
			{
				list.Add(vertices[num2]);
				list2.Add(normals[num2]);
				list3.Add(uv[num2]);
				list4.Add(uv2[num2]);
				list5.Add(this.colorsFlowMap[num2]);
				list6.Add(this.colors[num2]);
				num2++;
			}
			if (list.Count > 0)
			{
				Vector3 b = list[0];
				for (int j = 0; j < list.Count; j++)
				{
					list[j] -= b;
				}
				for (int k = 0; k < list.Count / this.vertsInShape - 1; k++)
				{
					int num3 = k * this.vertsInShape;
					for (int l = 0; l < this.vertsInShape - 1; l++)
					{
						int item = num3 + l;
						int item2 = num3 + l + this.vertsInShape;
						int item3 = num3 + l + 1 + this.vertsInShape;
						int item4 = num3 + l + 1;
						list7.Add(item);
						list7.Add(item2);
						list7.Add(item3);
						list7.Add(item3);
						list7.Add(item4);
						list7.Add(item);
					}
				}
				gameObject.transform.position += b;
				mesh.vertices = list.ToArray();
				mesh.triangles = list7.ToArray();
				mesh.normals = list2.ToArray();
				mesh.uv = list3.ToArray();
				mesh.uv3 = list4.ToArray();
				mesh.uv4 = list5.ToArray();
				mesh.colors = list6.ToArray();
				mesh.RecalculateTangents();
				meshFilter.mesh = mesh;
			}
		}
	}

	// Token: 0x06000077 RID: 119 RVA: 0x00090574 File Offset: 0x0008E774
	public void AddNoiseToWidths()
	{
		for (int i = 0; i < this.controlPoints.Count; i++)
		{
			Vector4 vector = this.controlPoints[i];
			vector.w += (this.noiseWidth ? (this.noiseMultiplierWidth * (Mathf.PerlinNoise(this.noiseSizeWidth * (float)i, 0f) - 0.5f)) : 0f);
			if (vector.w < 0f)
			{
				vector.w = 0f;
			}
			this.controlPoints[i] = vector;
		}
	}

	// Token: 0x06000078 RID: 120 RVA: 0x0009060C File Offset: 0x0008E80C
	public void SimulateRiver(bool generate = true)
	{
		if (this.meshGO != null)
		{
			if (Application.isEditor)
			{
				UnityEngine.Object.DestroyImmediate(this.meshGO);
			}
			else
			{
				UnityEngine.Object.Destroy(this.meshGO);
			}
		}
		if (this.controlPoints.Count == 0)
		{
			Debug.Log("Add one point to start Simulating River");
			return;
		}
		Ray ray = default(Ray);
		Vector3 vector = base.transform.TransformPoint(this.controlPoints[this.controlPoints.Count - 1]);
		List<Vector3> list = new List<Vector3>();
		if (this.controlPoints.Count > 1)
		{
			list.Add(base.transform.TransformPoint(this.controlPoints[this.controlPoints.Count - 2]));
			list.Add(vector);
		}
		List<Vector3> list2 = new List<Vector3>();
		list2.Add(vector);
		float num = 0f;
		int i = -1;
		int num2 = 0;
		bool flag = false;
		float num3 = 0f;
		if (this.controlPoints.Count > 0)
		{
			num3 = this.controlPoints[this.controlPoints.Count - 1].w;
		}
		else
		{
			num3 = this.width;
		}
		do
		{
			i++;
			if (i > 0)
			{
				Vector3 vector2 = Vector3.zero;
				float num4 = float.MinValue;
				bool flag2 = false;
				for (float num5 = this.simulatedMinStepSize; num5 < 10f; num5 += 0.1f)
				{
					for (int j = 0; j < 36; j++)
					{
						float x = num5 * Mathf.Cos((float)j);
						float z = num5 * Mathf.Sin((float)j);
						ray.origin = vector + new Vector3(0f, 1000f, 0f) + new Vector3(x, 0f, z);
						ray.direction = Vector3.down;
						RaycastHit raycastHit;
						if (Physics.Raycast(ray, out raycastHit, 10000f) && raycastHit.distance > num4)
						{
							bool flag3 = true;
							foreach (Vector3 a in list)
							{
								if (Vector3.Distance(a, vector) > Vector3.Distance(a, raycastHit.point) + 0.5f)
								{
									flag3 = false;
									break;
								}
							}
							if (flag3)
							{
								flag2 = true;
								num4 = raycastHit.distance;
								vector2 = raycastHit.point;
							}
						}
					}
					if (flag2)
					{
						break;
					}
				}
				if (!flag2)
				{
					break;
				}
				if (vector2.y > vector.y)
				{
					if (this.simulatedNoUp)
					{
						vector2.y = vector.y;
					}
					if (this.simulatedBreakOnUp)
					{
						flag = true;
					}
				}
				num += Vector3.Distance(vector2, vector);
				if (i % this.simulatedRiverPoints == 0 || this.simulatedRiverLength <= num || flag)
				{
					list2.Add(vector2);
					if (generate)
					{
						num2++;
						Vector4 item = vector2 - base.transform.position;
						item.w = num3 + (this.noiseWidth ? (this.noiseMultiplierWidth * (Mathf.PerlinNoise(this.noiseSizeWidth * (float)num2, 0f) - 0.5f)) : 0f);
						this.controlPointsRotations.Add(Quaternion.identity);
						this.controlPoints.Add(item);
						this.controlPointsSnap.Add(0f);
						this.controlPointsMeshCurves.Add(new AnimationCurve(this.meshCurve.keys));
					}
				}
				list.Add(vector);
				vector = vector2;
			}
		}
		while (this.simulatedRiverLength > num && !flag);
		if (!generate)
		{
			if (this.controlPoints.Count > 0)
			{
				num3 = this.controlPoints[this.controlPoints.Count - 1].w;
			}
			else
			{
				num3 = this.width;
			}
			List<List<Vector4>> list3 = new List<List<Vector4>>();
			Vector3 a2 = default(Vector3);
			float d;
			for (i = 0; i < list2.Count - 1; i++)
			{
				d = num3 + (this.noiseWidth ? (this.noiseMultiplierWidth * (Mathf.PerlinNoise(this.noiseSizeWidth * (float)i, 0f) - 0.5f)) : 0f);
				a2 = Vector3.Cross(list2[i + 1] - list2[i], Vector3.up).normalized;
				if (i > 0)
				{
					Vector3 normalized = Vector3.Cross(list2[i] - list2[i - 1], Vector3.up).normalized;
					a2 = (a2 + normalized).normalized;
				}
				list3.Add(new List<Vector4>
				{
					list2[i] + a2 * d * 0.5f,
					list2[i] - a2 * d * 0.5f
				});
			}
			d = num3 + (this.noiseWidth ? (this.noiseMultiplierWidth * (Mathf.PerlinNoise(this.noiseSizeWidth * (float)i, 0f) - 0.5f)) : 0f);
			list3.Add(new List<Vector4>
			{
				list2[i] + a2 * d * 0.5f,
				list2[i] - a2 * d * 0.5f
			});
			Mesh mesh = new Mesh();
			mesh.indexFormat = IndexFormat.UInt32;
			List<Vector3> list4 = new List<Vector3>();
			List<int> list5 = new List<int>();
			foreach (List<Vector4> list6 in list3)
			{
				foreach (Vector4 v in list6)
				{
					list4.Add(v);
				}
			}
			for (i = 0; i < list3.Count - 1; i++)
			{
				int count = list3[i].Count;
				for (int k = 0; k < count - 1; k++)
				{
					list5.Add(k + i * count);
					list5.Add(k + (i + 1) * count);
					list5.Add(k + 1 + i * count);
					list5.Add(k + 1 + i * count);
					list5.Add(k + (i + 1) * count);
					list5.Add(k + 1 + (i + 1) * count);
				}
			}
			mesh.SetVertices(list4);
			mesh.SetTriangles(list5, 0);
			mesh.RecalculateNormals();
			mesh.RecalculateTangents();
			mesh.RecalculateBounds();
			this.meshGO = new GameObject("TerrainMesh");
			this.meshGO.hideFlags = HideFlags.HideAndDontSave;
			this.meshGO.AddComponent<MeshFilter>();
			this.meshGO.transform.parent = base.transform;
			MeshRenderer meshRenderer = this.meshGO.AddComponent<MeshRenderer>();
			meshRenderer.sharedMaterial = new Material(Shader.Find("Debug Terrain Carve"));
			meshRenderer.sharedMaterial.color = new Color(0f, 0.5f, 0f);
			this.meshGO.transform.position = Vector3.zero;
			this.meshGO.GetComponent<MeshFilter>().sharedMesh = mesh;
		}
	}

	// Token: 0x06000079 RID: 121 RVA: 0x00090DE4 File Offset: 0x0008EFE4
	public void ShowTerrainCarve(float differentSize = 0f)
	{
		if (Application.isEditor && this.meshGO == null)
		{
			Transform transform = base.transform.Find("TerrainMesh");
			if (transform != null)
			{
				this.meshGO = transform.gameObject;
			}
		}
		if (this.meshGO != null)
		{
			if (Application.isEditor)
			{
				UnityEngine.Object.DestroyImmediate(this.meshGO);
			}
			else
			{
				UnityEngine.Object.Destroy(this.meshGO);
			}
		}
		Mesh sharedMesh = this.meshfilter.sharedMesh;
		int num = (int)this.terrainMeshSmoothZ;
		if (differentSize == 0f)
		{
			this.terrainAdditionalWidth = this.distSmooth + this.distSmoothStart;
		}
		else
		{
			this.terrainAdditionalWidth = differentSize;
		}
		List<List<Vector4>> list = new List<List<Vector4>>();
		for (int i = 0; i < this.carvePointsDown.Count - 1; i++)
		{
			List<Vector4> list2 = new List<Vector4>();
			Vector3 vector = this.carvePointsDown[i];
			Vector3 vector2 = this.carvePointsUp[i];
			Vector3 a = vector - vector2;
			float magnitude = a.magnitude;
			vector += a * 0.05f;
			vector2 -= a * 0.05f;
			a.Normalize();
			Vector3 a2 = vector + a * this.terrainAdditionalWidth * 0.5f;
			Vector3 b = vector2 - a * this.terrainAdditionalWidth * 0.5f;
			if (this.terrainAdditionalWidth > 0f)
			{
				for (int j = 0; j < num; j++)
				{
					Vector3 vector3 = Vector3.Lerp(a2, vector, (float)j / (float)num) + base.transform.position;
					RaycastHit raycastHit;
					if (Physics.Raycast(vector3 + Vector3.up * 500f, Vector3.down, out raycastHit, 10000f, this.maskCarve.value))
					{
						float num2;
						if (this.noiseCarve)
						{
							num2 = Mathf.PerlinNoise(vector3.x * this.noiseSizeX, vector3.z * this.noiseSizeZ) * this.noiseMultiplierOutside - this.noiseMultiplierOutside * 0.5f;
						}
						else
						{
							num2 = 0f;
						}
						float num3 = 1f - (float)j / (float)num;
						num3 *= this.terrainAdditionalWidth;
						float num4 = vector3.y + this.terrainCarve.Evaluate(-num3) + this.terrainCarve.Evaluate(-num3) * num2;
						float num5 = (float)j / (float)num;
						num5 = Mathf.Pow(num5, this.terrainSmoothMultiplier);
						num4 = Mathf.Lerp(raycastHit.point.y, num4, num5);
						Vector4 item = new Vector4(raycastHit.point.x, num4, raycastHit.point.z, -num3);
						list2.Add(item);
					}
					else
					{
						list2.Add(vector3);
					}
				}
			}
			for (int k = 0; k <= num; k++)
			{
				Vector3 vector3 = Vector3.Lerp(vector, vector2, (float)k / (float)num) + base.transform.position;
				RaycastHit raycastHit;
				if (Physics.Raycast(vector3 + Vector3.up * 500f, Vector3.down, out raycastHit, 10000f, this.maskCarve.value))
				{
					float num2;
					if (this.noiseCarve)
					{
						num2 = Mathf.PerlinNoise(vector3.x * this.noiseSizeX, vector3.z * this.noiseSizeZ) * this.noiseMultiplierInside - this.noiseMultiplierInside * 0.5f;
					}
					else
					{
						num2 = 0f;
					}
					float num6 = magnitude * (0.5f - Mathf.Abs(0.5f - (float)k / (float)num));
					float num7 = vector3.y + this.terrainCarve.Evaluate(num6) + this.terrainCarve.Evaluate(num6) * num2;
					Mathf.Pow(1f - 2f * Mathf.Abs((float)k / (float)num - 0.5f), this.terrainSmoothMultiplier);
					num7 = Mathf.Lerp(raycastHit.point.y, num7, 1f);
					Vector4 item2 = new Vector4(raycastHit.point.x, num7, raycastHit.point.z, num6);
					list2.Add(item2);
				}
				else
				{
					list2.Add(vector3);
				}
			}
			if (this.terrainAdditionalWidth > 0f)
			{
				for (int l = 1; l <= num; l++)
				{
					Vector3 vector3 = Vector3.Lerp(vector2, b, (float)l / (float)num) + base.transform.position;
					RaycastHit raycastHit;
					if (Physics.Raycast(vector3 + Vector3.up * 50f, Vector3.down, out raycastHit, 10000f, this.maskCarve.value))
					{
						float num2;
						if (this.noiseCarve)
						{
							num2 = Mathf.PerlinNoise(vector3.x * this.noiseSizeX, vector3.z * this.noiseSizeZ) * this.noiseMultiplierOutside - this.noiseMultiplierOutside * 0.5f;
						}
						else
						{
							num2 = 0f;
						}
						float num8 = (float)l / (float)num;
						num8 *= this.terrainAdditionalWidth;
						float num9 = vector3.y + this.terrainCarve.Evaluate(-num8) + this.terrainCarve.Evaluate(-num8) * num2;
						float num10 = 1f - (float)l / (float)num;
						num10 = Mathf.Pow(num10, this.terrainSmoothMultiplier);
						num9 = Mathf.Lerp(raycastHit.point.y, num9, num10);
						Vector4 item3 = new Vector4(raycastHit.point.x, num9, raycastHit.point.z, -num8);
						list2.Add(item3);
					}
					else
					{
						list2.Add(vector3);
					}
				}
			}
			list.Add(list2);
		}
		Mesh mesh = new Mesh();
		mesh.indexFormat = IndexFormat.UInt32;
		List<Vector3> list3 = new List<Vector3>();
		List<int> list4 = new List<int>();
		List<Vector2> list5 = new List<Vector2>();
		foreach (List<Vector4> list6 in list)
		{
			foreach (Vector4 v in list6)
			{
				list3.Add(v);
			}
		}
		for (int m = 0; m < list.Count - 1; m++)
		{
			int count = list[m].Count;
			for (int n = 0; n < count - 1; n++)
			{
				list4.Add(n + m * count);
				list4.Add(n + (m + 1) * count);
				list4.Add(n + 1 + m * count);
				list4.Add(n + 1 + m * count);
				list4.Add(n + (m + 1) * count);
				list4.Add(n + 1 + (m + 1) * count);
			}
		}
		foreach (List<Vector4> list7 in list)
		{
			foreach (Vector4 vector4 in list7)
			{
				list5.Add(new Vector2(vector4.w, 0f));
			}
		}
		mesh.SetVertices(list3);
		mesh.SetTriangles(list4, 0);
		mesh.SetUVs(0, list5);
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();
		mesh.RecalculateBounds();
		this.meshGO = new GameObject("TerrainMesh");
		this.meshGO.transform.parent = base.transform;
		this.meshGO.hideFlags = HideFlags.HideAndDontSave;
		this.meshGO.AddComponent<MeshFilter>();
		this.meshGO.transform.parent = base.transform;
		MeshRenderer meshRenderer = this.meshGO.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = new Material(Shader.Find("Debug Terrain Carve"));
		meshRenderer.sharedMaterial.color = new Color(0f, 0.5f, 0f);
		this.meshGO.transform.position = Vector3.zero;
		this.meshGO.GetComponent<MeshFilter>().sharedMesh = mesh;
		if (this.overrideRiverRender)
		{
			this.meshGO.GetComponent<MeshRenderer>().sharedMaterial.renderQueue = 5000;
			return;
		}
		this.meshGO.GetComponent<MeshRenderer>().sharedMaterial.renderQueue = 2980;
	}

	// Token: 0x0600007A RID: 122 RVA: 0x000916C0 File Offset: 0x0008F8C0
	public void TerrainCarve(Terrain singleTerrain = null)
	{
		bool flag = false;
		bool autoSyncTransforms = Physics.autoSyncTransforms;
		Physics.autoSyncTransforms = false;
		foreach (Terrain terrain in Terrain.activeTerrains)
		{
			if (!(singleTerrain != null) || !(terrain != singleTerrain))
			{
				TerrainData terrainData = terrain.terrainData;
				float y = terrain.transform.position.y;
				float x = terrain.terrainData.size.x;
				float y2 = terrain.terrainData.size.y;
				float z = terrain.terrainData.size.z;
				float num = 1f / z * (float)(terrainData.heightmapResolution - 1);
				float num2 = 1f / x * (float)(terrainData.heightmapResolution - 1);
				MeshCollider meshCollider = this.meshGO.gameObject.AddComponent<MeshCollider>();
				List<Vector3> list = new List<Vector3>();
				List<Vector3> list2 = new List<Vector3>();
				int num3 = 5;
				Vector3 zero = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				for (int j = 0; j < this.pointsUp.Count; j = Mathf.Clamp(j + num3 - 1, 0, this.pointsUp.Count))
				{
					int num4 = Mathf.Min(j + num3, this.pointsUp.Count);
					list.Clear();
					list2.Clear();
					for (int k = j; k < num4; k++)
					{
						list.Add(base.transform.TransformPoint(this.pointsUp[k]));
						list2.Add(base.transform.TransformPoint(this.pointsDown[k]));
					}
					float num5 = float.MaxValue;
					float num6 = float.MinValue;
					float num7 = float.MaxValue;
					float num8 = float.MinValue;
					for (int l = 0; l < list.Count; l++)
					{
						Vector3 vector = list[l];
						if (num5 > vector.x)
						{
							num5 = vector.x;
						}
						if (num6 < vector.x)
						{
							num6 = vector.x;
						}
						if (num7 > vector.z)
						{
							num7 = vector.z;
						}
						if (num8 < vector.z)
						{
							num8 = vector.z;
						}
					}
					for (int m = 0; m < list2.Count; m++)
					{
						Vector3 vector2 = list2[m];
						if (num5 > vector2.x)
						{
							num5 = vector2.x;
						}
						if (num6 < vector2.x)
						{
							num6 = vector2.x;
						}
						if (num7 > vector2.z)
						{
							num7 = vector2.z;
						}
						if (num8 < vector2.z)
						{
							num8 = vector2.z;
						}
					}
					num5 -= terrain.transform.position.x + this.distSmooth;
					num6 -= terrain.transform.position.x - this.distSmooth;
					num7 -= terrain.transform.position.z + this.distSmooth;
					num8 -= terrain.transform.position.z - this.distSmooth;
					num5 *= num2;
					num6 *= num2;
					num7 *= num;
					num8 *= num;
					num6 = Mathf.Ceil(Mathf.Clamp(num6 + 1f, 0f, (float)terrainData.heightmapResolution));
					num7 = Mathf.Floor(Mathf.Clamp(num7, 0f, (float)terrainData.heightmapResolution));
					num8 = Mathf.Ceil(Mathf.Clamp(num8 + 1f, 0f, (float)terrainData.heightmapResolution));
					num5 = Mathf.Floor(Mathf.Clamp(num5, 0f, (float)terrainData.heightmapResolution));
					float[,] heights = terrainData.GetHeights((int)num5, (int)num7, (int)(num6 - num5), (int)(num8 - num7));
					Vector3 zero3 = Vector3.zero;
					Vector3 zero4 = Vector3.zero;
					for (int n = 0; n < heights.GetLength(0); n++)
					{
						for (int num9 = 0; num9 < heights.GetLength(1); num9++)
						{
							zero3.x = ((float)num9 + num5) / num2 + terrain.transform.position.x;
							zero3.z = ((float)n + num7) / num + terrain.transform.position.z;
							Ray ray = new Ray(zero3 + Vector3.up * 3000f, Vector3.down);
							RaycastHit raycastHit;
							if (meshCollider.Raycast(ray, out raycastHit, 10000f))
							{
								float num10 = raycastHit.point.y - y;
								heights[n, num9] = num10 / y2;
								if (flag)
								{
									Debug.DrawLine(raycastHit.point, raycastHit.point + Vector3.up * 0.1f, Color.magenta, 10f);
								}
							}
						}
					}
					terrainData.SetHeightsDelayLOD((int)num5, (int)num7, heights);
				}
				UnityEngine.Object.DestroyImmediate(meshCollider);
				terrainData.SyncHeightmap();
				terrain.Flush();
			}
		}
		Physics.autoSyncTransforms = autoSyncTransforms;
		if (this.meshGO != null)
		{
			UnityEngine.Object.DestroyImmediate(this.meshGO);
		}
	}

	// Token: 0x0600007B RID: 123 RVA: 0x00091BE8 File Offset: 0x0008FDE8
	public void TerrainPaintMeshBased(Terrain singleTerrain = null)
	{
		bool autoSyncTransforms = Physics.autoSyncTransforms;
		Physics.autoSyncTransforms = false;
		foreach (Terrain terrain in Terrain.activeTerrains)
		{
			if (!(singleTerrain != null) || !(terrain != singleTerrain))
			{
				TerrainData terrainData = terrain.terrainData;
				float x = terrain.terrainData.size.x;
				Vector3 size = terrain.terrainData.size;
				float z = terrain.terrainData.size.z;
				float num = 1f / z * (float)(terrainData.alphamapWidth - 1);
				float num2 = 1f / x * (float)(terrainData.alphamapHeight - 1);
				MeshCollider meshCollider = this.meshGO.gameObject.AddComponent<MeshCollider>();
				List<Vector3> list = new List<Vector3>();
				List<Vector3> list2 = new List<Vector3>();
				int num3 = 5;
				Vector3 zero = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				for (int j = 0; j < this.pointsUp.Count; j = Mathf.Clamp(j + num3 - 1, 0, this.pointsUp.Count))
				{
					int num4 = Mathf.Min(j + num3, this.pointsUp.Count);
					list.Clear();
					list2.Clear();
					for (int k = j; k < num4; k++)
					{
						list.Add(base.transform.TransformPoint(this.pointsUp[k]));
						list2.Add(base.transform.TransformPoint(this.pointsDown[k]));
					}
					float num5 = float.MaxValue;
					float num6 = float.MinValue;
					float num7 = float.MaxValue;
					float num8 = float.MinValue;
					for (int l = 0; l < list.Count; l++)
					{
						Vector3 vector = list[l];
						if (num5 > vector.x)
						{
							num5 = vector.x;
						}
						if (num6 < vector.x)
						{
							num6 = vector.x;
						}
						if (num7 > vector.z)
						{
							num7 = vector.z;
						}
						if (num8 < vector.z)
						{
							num8 = vector.z;
						}
					}
					for (int m = 0; m < list2.Count; m++)
					{
						Vector3 vector2 = list2[m];
						if (num5 > vector2.x)
						{
							num5 = vector2.x;
						}
						if (num6 < vector2.x)
						{
							num6 = vector2.x;
						}
						if (num7 > vector2.z)
						{
							num7 = vector2.z;
						}
						if (num8 < vector2.z)
						{
							num8 = vector2.z;
						}
					}
					num5 -= terrain.transform.position.x + this.distSmooth;
					num6 -= terrain.transform.position.x - this.distSmooth;
					num7 -= terrain.transform.position.z + this.distSmooth;
					num8 -= terrain.transform.position.z - this.distSmooth;
					num5 *= num2;
					num6 *= num2;
					num7 *= num;
					num8 *= num;
					num5 = Mathf.Floor(Mathf.Clamp(num5, 0f, (float)terrainData.alphamapWidth));
					num6 = Mathf.Ceil(Mathf.Clamp(num6 + 1f, 0f, (float)terrainData.alphamapWidth));
					num7 = Mathf.Floor(Mathf.Clamp(num7, 0f, (float)terrainData.alphamapHeight));
					num8 = Mathf.Ceil(Mathf.Clamp(num8 + 1f, 0f, (float)terrainData.alphamapHeight));
					float[,,] alphamaps = terrainData.GetAlphamaps((int)num5, (int)num7, (int)(num6 - num5), (int)(num8 - num7));
					if (alphamaps.GetLength(2) <= this.currentSplatMap)
					{
						Debug.LogWarning("RAM: Terrain \"" + terrain.name + "\" doesn't have layer: " + this.currentSplatMap.ToString());
						break;
					}
					Vector3 zero3 = Vector3.zero;
					Vector3 zero4 = Vector3.zero;
					for (int n = 0; n < alphamaps.GetLength(0); n++)
					{
						for (int num9 = 0; num9 < alphamaps.GetLength(1); num9++)
						{
							zero3.x = ((float)num9 + num5) / num2 + terrain.transform.position.x;
							zero3.z = ((float)n + num7) / num + terrain.transform.position.z;
							Ray ray = new Ray(zero3 + Vector3.up * 3000f, Vector3.down);
							RaycastHit raycastHit;
							if (meshCollider.Raycast(ray, out raycastHit, 10000f))
							{
								float x2 = raycastHit.textureCoord.x;
								if (!this.mixTwoSplatMaps)
								{
									float num10;
									if (this.noisePaint)
									{
										if (x2 >= 0f)
										{
											num10 = Mathf.PerlinNoise(raycastHit.point.x * this.noiseSizeXPaint, raycastHit.point.z * this.noiseSizeZPaint) * this.noiseMultiplierInsidePaint - this.noiseMultiplierInsidePaint * 0.5f;
										}
										else
										{
											num10 = Mathf.PerlinNoise(raycastHit.point.x * this.noiseSizeXPaint, raycastHit.point.z * this.noiseSizeZPaint) * this.noiseMultiplierOutsidePaint - this.noiseMultiplierOutsidePaint * 0.5f;
										}
									}
									else
									{
										num10 = 0f;
									}
									float num11 = alphamaps[n, num9, this.currentSplatMap];
									alphamaps[n, num9, this.currentSplatMap] = Mathf.Clamp01(Mathf.Lerp(alphamaps[n, num9, this.currentSplatMap], 1f, this.terrainPaintCarve.Evaluate(x2) + this.terrainPaintCarve.Evaluate(x2) * num10));
									for (int num12 = 0; num12 < terrainData.terrainLayers.Length; num12++)
									{
										if (num12 != this.currentSplatMap)
										{
											alphamaps[n, num9, num12] = ((num11 == 1f) ? 0f : Mathf.Clamp01(alphamaps[n, num9, num12] * ((1f - alphamaps[n, num9, this.currentSplatMap]) / (1f - num11))));
										}
									}
								}
								else
								{
									float num10;
									if (x2 >= 0f)
									{
										num10 = Mathf.PerlinNoise(raycastHit.point.x * this.noiseSizeXPaint, raycastHit.point.z * this.noiseSizeZPaint) * this.noiseMultiplierInsidePaint - this.noiseMultiplierInsidePaint * 0.5f;
									}
									else
									{
										num10 = Mathf.PerlinNoise(raycastHit.point.x * this.noiseSizeXPaint, raycastHit.point.z * this.noiseSizeZPaint) * this.noiseMultiplierOutsidePaint - this.noiseMultiplierOutsidePaint * 0.5f;
									}
									float num13 = alphamaps[n, num9, this.currentSplatMap];
									alphamaps[n, num9, this.currentSplatMap] = Mathf.Clamp01(Mathf.Lerp(alphamaps[n, num9, this.currentSplatMap], 1f, this.terrainPaintCarve.Evaluate(x2)));
									for (int num14 = 0; num14 < terrainData.terrainLayers.Length; num14++)
									{
										if (num14 != this.currentSplatMap)
										{
											alphamaps[n, num9, num14] = ((num13 == 1f) ? 0f : Mathf.Clamp01(alphamaps[n, num9, num14] * ((1f - alphamaps[n, num9, this.currentSplatMap]) / (1f - num13))));
										}
									}
									if (num10 > 0f)
									{
										num13 = alphamaps[n, num9, this.secondSplatMap];
										alphamaps[n, num9, this.secondSplatMap] = Mathf.Clamp01(Mathf.Lerp(alphamaps[n, num9, this.secondSplatMap], 1f, num10));
										for (int num15 = 0; num15 < terrainData.terrainLayers.Length; num15++)
										{
											if (num15 != this.secondSplatMap)
											{
												alphamaps[n, num9, num15] = ((num13 == 1f) ? 0f : Mathf.Clamp01(alphamaps[n, num9, num15] * ((1f - alphamaps[n, num9, this.secondSplatMap]) / (1f - num13))));
											}
										}
									}
								}
								if (this.addCliffSplatMap)
								{
									if (x2 >= 0f)
									{
										if (Vector3.Angle(raycastHit.normal, Vector3.up) > this.cliffAngle)
										{
											float num16 = alphamaps[n, num9, this.cliffSplatMap];
											alphamaps[n, num9, this.cliffSplatMap] = this.cliffBlend;
											for (int num17 = 0; num17 < terrainData.terrainLayers.Length; num17++)
											{
												if (num17 != this.cliffSplatMap)
												{
													alphamaps[n, num9, num17] = ((num16 == 1f) ? 0f : Mathf.Clamp01(alphamaps[n, num9, num17] * ((1f - alphamaps[n, num9, this.cliffSplatMap]) / (1f - num16))));
												}
											}
										}
									}
									else if (Vector3.Angle(raycastHit.normal, Vector3.up) > this.cliffAngleOutside)
									{
										float num18 = alphamaps[n, num9, this.cliffSplatMapOutside];
										alphamaps[n, num9, this.cliffSplatMapOutside] = this.cliffBlendOutside;
										for (int num19 = 0; num19 < terrainData.terrainLayers.Length; num19++)
										{
											if (num19 != this.cliffSplatMapOutside)
											{
												alphamaps[n, num9, num19] = ((num18 == 1f) ? 0f : Mathf.Clamp01(alphamaps[n, num9, num19] * ((1f - alphamaps[n, num9, this.cliffSplatMapOutside]) / (1f - num18))));
											}
										}
									}
								}
							}
						}
					}
					terrainData.SetAlphamaps((int)num5, (int)num7, alphamaps);
				}
				UnityEngine.Object.DestroyImmediate(meshCollider);
				terrain.Flush();
			}
		}
		Physics.autoSyncTransforms = autoSyncTransforms;
		if (this.meshGO != null)
		{
			UnityEngine.Object.DestroyImmediate(this.meshGO);
		}
	}

	// Token: 0x0600007C RID: 124 RVA: 0x00092604 File Offset: 0x00090804
	public void TerrainClearFoliage(bool details = true)
	{
		bool autoSyncTransforms = Physics.autoSyncTransforms;
		Physics.autoSyncTransforms = false;
		foreach (Terrain terrain in Terrain.activeTerrains)
		{
			TerrainData terrainData = terrain.terrainData;
			Transform transform = terrain.transform;
			Vector3 position = terrain.transform.position;
			float x = terrain.terrainData.size.x;
			Vector3 size = terrain.terrainData.size;
			float z = terrain.terrainData.size.z;
			float num = 1f / z * (float)terrainData.detailWidth;
			float num2 = 1f / x * (float)terrainData.detailHeight;
			MeshCollider meshCollider = this.meshGO.gameObject.AddComponent<MeshCollider>();
			List<Vector3> list = new List<Vector3>();
			List<Vector3> list2 = new List<Vector3>();
			int num3 = 5;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			Vector3 zero3 = Vector3.zero;
			if (details)
			{
				for (int j = 0; j < this.pointsUp.Count; j = Mathf.Clamp(j + num3 - 1, 0, this.pointsUp.Count))
				{
					int num4 = Mathf.Min(j + num3, this.pointsUp.Count);
					Mathf.Min(num3, this.pointsUp.Count - j);
					list.Clear();
					list2.Clear();
					for (int k = j; k < num4; k++)
					{
						list.Add(base.transform.TransformPoint(this.pointsUp[k]));
						list2.Add(base.transform.TransformPoint(this.pointsDown[k]));
					}
					float num5 = float.MaxValue;
					float num6 = float.MinValue;
					float num7 = float.MaxValue;
					float num8 = float.MinValue;
					for (int l = 0; l < list.Count; l++)
					{
						Vector3 vector = list[l];
						if (num5 > vector.x)
						{
							num5 = vector.x;
						}
						if (num6 < vector.x)
						{
							num6 = vector.x;
						}
						if (num7 > vector.z)
						{
							num7 = vector.z;
						}
						if (num8 < vector.z)
						{
							num8 = vector.z;
						}
					}
					for (int m = 0; m < list2.Count; m++)
					{
						Vector3 vector2 = list2[m];
						if (num5 > vector2.x)
						{
							num5 = vector2.x;
						}
						if (num6 < vector2.x)
						{
							num6 = vector2.x;
						}
						if (num7 > vector2.z)
						{
							num7 = vector2.z;
						}
						if (num8 < vector2.z)
						{
							num8 = vector2.z;
						}
					}
					num5 -= transform.position.x + this.distanceClearFoliage;
					num6 -= transform.position.x - this.distanceClearFoliage;
					num7 -= transform.position.z + this.distanceClearFoliage;
					num8 -= transform.position.z - this.distanceClearFoliage;
					num5 *= num2;
					num6 *= num2;
					num7 *= num;
					num8 *= num;
					num5 = Mathf.Floor(Mathf.Clamp(num5, 0f, (float)terrainData.detailWidth));
					num6 = Mathf.Ceil(Mathf.Clamp(num6 + 1f, 0f, (float)terrainData.detailWidth));
					num7 = Mathf.Floor(Mathf.Clamp(num7, 0f, (float)terrainData.detailHeight));
					num8 = Mathf.Ceil(Mathf.Clamp(num8 + 1f, 0f, (float)terrainData.detailHeight));
					if (num6 - num5 > 0f && num8 - num7 > 0f)
					{
						for (int n = 0; n < terrainData.detailPrototypes.Length; n++)
						{
							int[,] detailLayer = terrainData.GetDetailLayer((int)num5, (int)num7, (int)(num6 - num5), (int)(num8 - num7), n);
							for (int num9 = 0; num9 < detailLayer.GetLength(0); num9++)
							{
								for (int num10 = 0; num10 < detailLayer.GetLength(1); num10++)
								{
									zero3.x = ((float)num10 + num5) / num2 + terrain.transform.position.x;
									zero3.z = ((float)num9 + num7) / num + terrain.transform.position.z;
									Ray ray = new Ray(zero3 + Vector3.up * 3000f, Vector3.down);
									RaycastHit raycastHit;
									if (meshCollider.Raycast(ray, out raycastHit, 10000f))
									{
										detailLayer[num9, num10] = 0;
									}
								}
							}
							terrainData.SetDetailLayer((int)num5, (int)num7, n, detailLayer);
						}
					}
				}
			}
			else
			{
				List<TreeInstance> list3 = new List<TreeInstance>();
				foreach (TreeInstance treeInstance in terrainData.treeInstances)
				{
					zero3.x = treeInstance.position.x * x + transform.position.x;
					zero3.z = treeInstance.position.z * z + transform.position.z;
					Ray ray2 = new Ray(zero3 + Vector3.up * 3000f, Vector3.down);
					RaycastHit raycastHit2;
					if (!meshCollider.Raycast(ray2, out raycastHit2, 10000f))
					{
						list3.Add(treeInstance);
					}
				}
				terrainData.treeInstances = list3.ToArray();
			}
			UnityEngine.Object.DestroyImmediate(meshCollider);
			terrain.Flush();
		}
		Physics.autoSyncTransforms = autoSyncTransforms;
		if (this.meshGO != null)
		{
			UnityEngine.Object.DestroyImmediate(this.meshGO);
		}
	}

	// Token: 0x0600007D RID: 125 RVA: 0x00092BC0 File Offset: 0x00090DC0
	private float FlowCalculate(float u, float normalY, Vector3 vertice)
	{
		float num = (this.noiseflowMap ? (Mathf.PerlinNoise(vertice.x * this.noiseSizeXflowMap, vertice.z * this.noiseSizeZflowMap) * this.noiseMultiplierflowMap - this.noiseMultiplierflowMap * 0.5f) : 0f) * Mathf.Pow(Mathf.Clamp(normalY, 0f, 1f), 5f);
		return Mathf.Lerp(this.flowWaterfall.Evaluate(u), this.flowFlat.Evaluate(u) + num, Mathf.Clamp(normalY, 0f, 1f));
	}

	// Token: 0x040000E0 RID: 224
	public SplineProfile currentProfile;

	// Token: 0x040000E1 RID: 225
	public SplineProfile oldProfile;

	// Token: 0x040000E2 RID: 226
	public List<RamSpline> beginnigChildSplines = new List<RamSpline>();

	// Token: 0x040000E3 RID: 227
	public List<RamSpline> endingChildSplines = new List<RamSpline>();

	// Token: 0x040000E4 RID: 228
	public RamSpline beginningSpline;

	// Token: 0x040000E5 RID: 229
	public RamSpline endingSpline;

	// Token: 0x040000E6 RID: 230
	public int beginningConnectionID;

	// Token: 0x040000E7 RID: 231
	public int endingConnectionID;

	// Token: 0x040000E8 RID: 232
	public float beginningMinWidth = 0.5f;

	// Token: 0x040000E9 RID: 233
	public float beginningMaxWidth = 1f;

	// Token: 0x040000EA RID: 234
	public float endingMinWidth = 0.5f;

	// Token: 0x040000EB RID: 235
	public float endingMaxWidth = 1f;

	// Token: 0x040000EC RID: 236
	public int toolbarInt;

	// Token: 0x040000ED RID: 237
	public bool invertUVDirection;

	// Token: 0x040000EE RID: 238
	public bool uvRotation = true;

	// Token: 0x040000EF RID: 239
	public MeshFilter meshfilter;

	// Token: 0x040000F0 RID: 240
	public List<Vector4> controlPoints = new List<Vector4>();

	// Token: 0x040000F1 RID: 241
	public List<Quaternion> controlPointsRotations = new List<Quaternion>();

	// Token: 0x040000F2 RID: 242
	public List<Quaternion> controlPointsOrientation = new List<Quaternion>();

	// Token: 0x040000F3 RID: 243
	public List<Vector3> controlPointsUp = new List<Vector3>();

	// Token: 0x040000F4 RID: 244
	public List<Vector3> controlPointsDown = new List<Vector3>();

	// Token: 0x040000F5 RID: 245
	public List<float> controlPointsSnap = new List<float>();

	// Token: 0x040000F6 RID: 246
	public AnimationCurve meshCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x040000F7 RID: 247
	public List<AnimationCurve> controlPointsMeshCurves = new List<AnimationCurve>();

	// Token: 0x040000F8 RID: 248
	public bool normalFromRaycast;

	// Token: 0x040000F9 RID: 249
	public bool snapToTerrain;

	// Token: 0x040000FA RID: 250
	public LayerMask snapMask = 1;

	// Token: 0x040000FB RID: 251
	public List<Vector3> points = new List<Vector3>();

	// Token: 0x040000FC RID: 252
	public List<Vector3> pointsUp = new List<Vector3>();

	// Token: 0x040000FD RID: 253
	public List<Vector3> pointsDown = new List<Vector3>();

	// Token: 0x040000FE RID: 254
	public List<Vector3> carvePointsUp = new List<Vector3>();

	// Token: 0x040000FF RID: 255
	public List<Vector3> carvePointsDown = new List<Vector3>();

	// Token: 0x04000100 RID: 256
	public List<Vector3> points2 = new List<Vector3>();

	// Token: 0x04000101 RID: 257
	public List<Vector3> verticesBeginning = new List<Vector3>();

	// Token: 0x04000102 RID: 258
	public List<Vector3> verticesEnding = new List<Vector3>();

	// Token: 0x04000103 RID: 259
	public List<Vector3> normalsBeginning = new List<Vector3>();

	// Token: 0x04000104 RID: 260
	public List<Vector3> normalsEnding = new List<Vector3>();

	// Token: 0x04000105 RID: 261
	public List<float> widths = new List<float>();

	// Token: 0x04000106 RID: 262
	public List<float> snaps = new List<float>();

	// Token: 0x04000107 RID: 263
	public List<float> lerpValues = new List<float>();

	// Token: 0x04000108 RID: 264
	public List<Quaternion> orientations = new List<Quaternion>();

	// Token: 0x04000109 RID: 265
	public List<Vector3> tangents = new List<Vector3>();

	// Token: 0x0400010A RID: 266
	public List<Vector3> normalsList = new List<Vector3>();

	// Token: 0x0400010B RID: 267
	public Color[] colors;

	// Token: 0x0400010C RID: 268
	public List<Vector2> colorsFlowMap = new List<Vector2>();

	// Token: 0x0400010D RID: 269
	public List<Vector3> verticeDirection = new List<Vector3>();

	// Token: 0x0400010E RID: 270
	public float floatSpeed = 10f;

	// Token: 0x0400010F RID: 271
	public bool generateOnStart;

	// Token: 0x04000110 RID: 272
	public float minVal = 0.5f;

	// Token: 0x04000111 RID: 273
	public float maxVal = 0.5f;

	// Token: 0x04000112 RID: 274
	public float width = 4f;

	// Token: 0x04000113 RID: 275
	public int vertsInShape = 3;

	// Token: 0x04000114 RID: 276
	public float traingleDensity = 0.2f;

	// Token: 0x04000115 RID: 277
	public float uvScale = 3f;

	// Token: 0x04000116 RID: 278
	public Material oldMaterial;

	// Token: 0x04000117 RID: 279
	public bool showVertexColors;

	// Token: 0x04000118 RID: 280
	public bool showFlowMap;

	// Token: 0x04000119 RID: 281
	public bool overrideFlowMap;

	// Token: 0x0400011A RID: 282
	public bool drawOnMesh;

	// Token: 0x0400011B RID: 283
	public bool drawOnMeshFlowMap;

	// Token: 0x0400011C RID: 284
	public bool uvScaleOverride;

	// Token: 0x0400011D RID: 285
	public bool debug;

	// Token: 0x0400011E RID: 286
	public bool debugNormals;

	// Token: 0x0400011F RID: 287
	public bool debugTangents;

	// Token: 0x04000120 RID: 288
	public bool debugBitangent;

	// Token: 0x04000121 RID: 289
	public bool debugFlowmap;

	// Token: 0x04000122 RID: 290
	public bool debugPoints;

	// Token: 0x04000123 RID: 291
	public bool debugPointsConnect;

	// Token: 0x04000124 RID: 292
	public bool debugMesh = true;

	// Token: 0x04000125 RID: 293
	public float distanceToDebug = 5f;

	// Token: 0x04000126 RID: 294
	public Color drawColor = Color.black;

	// Token: 0x04000127 RID: 295
	public bool drawColorR = true;

	// Token: 0x04000128 RID: 296
	public bool drawColorG = true;

	// Token: 0x04000129 RID: 297
	public bool drawColorB = true;

	// Token: 0x0400012A RID: 298
	public bool drawColorA = true;

	// Token: 0x0400012B RID: 299
	public bool drawOnMultiple;

	// Token: 0x0400012C RID: 300
	public float flowSpeed = 1f;

	// Token: 0x0400012D RID: 301
	public float flowDirection;

	// Token: 0x0400012E RID: 302
	public AnimationCurve flowFlat = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0.025f),
		new Keyframe(0.5f, 0.05f),
		new Keyframe(1f, 0.025f)
	});

	// Token: 0x0400012F RID: 303
	public AnimationCurve flowWaterfall = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0.25f),
		new Keyframe(1f, 0.25f)
	});

	// Token: 0x04000130 RID: 304
	public bool noiseflowMap;

	// Token: 0x04000131 RID: 305
	public float noiseMultiplierflowMap = 0.1f;

	// Token: 0x04000132 RID: 306
	public float noiseSizeXflowMap = 2f;

	// Token: 0x04000133 RID: 307
	public float noiseSizeZflowMap = 2f;

	// Token: 0x04000134 RID: 308
	public float opacity = 0.1f;

	// Token: 0x04000135 RID: 309
	public float drawSize = 1f;

	// Token: 0x04000136 RID: 310
	public float length;

	// Token: 0x04000137 RID: 311
	public float fulllength;

	// Token: 0x04000138 RID: 312
	public float uv3length;

	// Token: 0x04000139 RID: 313
	public float minMaxWidth;

	// Token: 0x0400013A RID: 314
	public float uvWidth;

	// Token: 0x0400013B RID: 315
	public float uvBeginning;

	// Token: 0x0400013C RID: 316
	public bool receiveShadows;

	// Token: 0x0400013D RID: 317
	public ShadowCastingMode shadowCastingMode;

	// Token: 0x0400013E RID: 318
	public bool generateMeshParts;

	// Token: 0x0400013F RID: 319
	public int meshPartsCount = 3;

	// Token: 0x04000140 RID: 320
	public List<Transform> meshesPartTransforms = new List<Transform>();

	// Token: 0x04000141 RID: 321
	public float simulatedRiverLength = 100f;

	// Token: 0x04000142 RID: 322
	public int simulatedRiverPoints = 10;

	// Token: 0x04000143 RID: 323
	public float simulatedMinStepSize = 1f;

	// Token: 0x04000144 RID: 324
	public bool simulatedNoUp;

	// Token: 0x04000145 RID: 325
	public bool simulatedBreakOnUp = true;

	// Token: 0x04000146 RID: 326
	public float terrainAdditionalWidth = 2f;

	// Token: 0x04000147 RID: 327
	public float terrainMeshSmoothX = 2f;

	// Token: 0x04000148 RID: 328
	public float terrainMeshSmoothZ = 10f;

	// Token: 0x04000149 RID: 329
	public float terrainSmoothMultiplier = 5f;

	// Token: 0x0400014A RID: 330
	public bool overrideRiverRender;

	// Token: 0x0400014B RID: 331
	public bool noiseWidth;

	// Token: 0x0400014C RID: 332
	public float noiseMultiplierWidth = 4f;

	// Token: 0x0400014D RID: 333
	public float noiseSizeWidth = 0.5f;

	// Token: 0x0400014E RID: 334
	public bool noiseCarve;

	// Token: 0x0400014F RID: 335
	public float noiseMultiplierInside = 1f;

	// Token: 0x04000150 RID: 336
	public float noiseMultiplierOutside = 0.25f;

	// Token: 0x04000151 RID: 337
	public float noiseSizeX = 0.2f;

	// Token: 0x04000152 RID: 338
	public float noiseSizeZ = 0.2f;

	// Token: 0x04000153 RID: 339
	public bool noisePaint;

	// Token: 0x04000154 RID: 340
	public float noiseMultiplierInsidePaint = 0.25f;

	// Token: 0x04000155 RID: 341
	public float noiseMultiplierOutsidePaint = 0.25f;

	// Token: 0x04000156 RID: 342
	public float noiseSizeXPaint = 0.2f;

	// Token: 0x04000157 RID: 343
	public float noiseSizeZPaint = 0.2f;

	// Token: 0x04000158 RID: 344
	public Terrain workTerrain;

	// Token: 0x04000159 RID: 345
	public List<Terrain> terrainsUnder = new List<Terrain>();

	// Token: 0x0400015A RID: 346
	public int currentWorkTerrain;

	// Token: 0x0400015B RID: 347
	public LayerMask maskCarve = 1;

	// Token: 0x0400015C RID: 348
	public AnimationCurve terrainCarve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0.5f),
		new Keyframe(10f, -4f)
	});

	// Token: 0x0400015D RID: 349
	public float distSmooth = 5f;

	// Token: 0x0400015E RID: 350
	public float distSmoothStart = 1f;

	// Token: 0x0400015F RID: 351
	public AnimationCurve terrainPaintCarve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04000160 RID: 352
	public int currentSplatMap = 1;

	// Token: 0x04000161 RID: 353
	public bool mixTwoSplatMaps;

	// Token: 0x04000162 RID: 354
	public int secondSplatMap = 1;

	// Token: 0x04000163 RID: 355
	public bool addCliffSplatMap;

	// Token: 0x04000164 RID: 356
	public int cliffSplatMap = 1;

	// Token: 0x04000165 RID: 357
	public float cliffAngle = 45f;

	// Token: 0x04000166 RID: 358
	public float cliffBlend = 1f;

	// Token: 0x04000167 RID: 359
	public int cliffSplatMapOutside = 1;

	// Token: 0x04000168 RID: 360
	public float cliffAngleOutside = 45f;

	// Token: 0x04000169 RID: 361
	public float cliffBlendOutside = 1f;

	// Token: 0x0400016A RID: 362
	public float distanceClearFoliage = 1f;

	// Token: 0x0400016B RID: 363
	public float distanceClearFoliageTrees = 1f;

	// Token: 0x0400016C RID: 364
	public float biomMaskResolution = 0.5f;

	// Token: 0x0400016D RID: 365
	public float vegetationMaskSize = 3f;

	// Token: 0x0400016E RID: 366
	public float vegetationBlendDistance = 1f;

	// Token: 0x0400016F RID: 367
	public BiomeMaskArea biomeMaskArea;

	// Token: 0x04000170 RID: 368
	public bool refreshMask;

	// Token: 0x04000171 RID: 369
	public GameObject meshGO;
}
