using System;
using System.Collections.Generic;
using BansheeGz.BGSpline.Components;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Example
{
	// Token: 0x020001DE RID: 478
	public class BGTestCurveClosestPoint : MonoBehaviour
	{
		// Token: 0x17000494 RID: 1172
		// (get) Token: 0x060010F2 RID: 4338 RVA: 0x0004E10E File Offset: 0x0004C30E
		private bool HasError
		{
			get
			{
				return this.ErrorPointIndex >= 0;
			}
		}

		// Token: 0x060010F3 RID: 4339 RVA: 0x000E1510 File Offset: 0x000DF710
		private void Start()
		{
			this.curve = base.gameObject.AddComponent<BGCurve>();
			this.curve.Closed = true;
			this.math = base.gameObject.AddComponent<BGCcMath>();
			base.gameObject.AddComponent<BGCcVisualizationLineRenderer>();
			LineRenderer component = base.gameObject.GetComponent<LineRenderer>();
			component.sharedMaterial = this.LineRendererMaterial;
			Color endColor = new Color(0.2f, 0.2f, 0.2f, 1f);
			component.startWidth = (component.endWidth = 0.03f);
			component.startColor = (component.endColor = endColor);
			this.math.SectionParts = this.NumberOfSplits;
			for (int i = 0; i < this.NumberOfCurvePoints; i++)
			{
				int num = UnityEngine.Random.Range(0, 3);
				BGCurvePoint.ControlTypeEnum controlType = BGCurvePoint.ControlTypeEnum.Absent;
				if (num != 1)
				{
					if (num == 2)
					{
						controlType = BGCurvePoint.ControlTypeEnum.BezierSymmetrical;
					}
				}
				else
				{
					controlType = BGCurvePoint.ControlTypeEnum.BezierIndependant;
				}
				this.curve.AddPoint(new BGCurvePoint(this.curve, Vector3.zero, controlType, BGTestCurveClosestPoint.RandomVector() * 0.3f, BGTestCurveClosestPoint.RandomVector() * 0.3f, false));
			}
			this.oldPointPos = new Vector3[this.NumberOfPointsToSeek];
			this.newPointPos = new Vector3[this.NumberOfPointsToSeek];
			this.oldCurvePointPos = new Vector3[this.NumberOfCurvePoints];
			this.newCurvePointPos = new Vector3[this.NumberOfCurvePoints];
			BGTestCurveClosestPoint.InitArray(this.newCurvePointPos, this.oldCurvePointPos);
			BGTestCurveClosestPoint.InitArray(this.newPointPos, this.oldPointPos);
			this.objects = new GameObject[this.NumberOfPointsToSeek];
			for (int j = 0; j < this.NumberOfPointsToSeek; j++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.PointIndicator);
				gameObject.transform.parent = base.transform;
				this.objects[j] = gameObject;
			}
			this.PointIndicator.SetActive(false);
			this.InitCycle();
		}

		// Token: 0x060010F4 RID: 4340 RVA: 0x000E16F4 File Offset: 0x000DF8F4
		private void OnGUI()
		{
			if (this.style == null)
			{
				this.style = new GUIStyle(GUI.skin.label)
				{
					fontSize = 20
				};
			}
			GUI.Label(new Rect(0f, 30f, 600f, 30f), "Turn on Gizmos to see Debug lines", this.style);
		}

		// Token: 0x060010F5 RID: 4341 RVA: 0x0004E11C File Offset: 0x0004C31C
		private void InitCycle()
		{
			BGTestCurveClosestPoint.InitArray(this.oldCurvePointPos, this.newCurvePointPos);
			BGTestCurveClosestPoint.InitArray(this.oldPointPos, this.newPointPos);
		}

		// Token: 0x060010F6 RID: 4342 RVA: 0x000E1750 File Offset: 0x000DF950
		private void Update()
		{
			if (this.HasError)
			{
				this.Process(this.ErrorPointIndex, true);
				return;
			}
			this.Calculate(null, null);
			float num = Time.time - this.startTime;
			if (num > (float)this.Period)
			{
				this.startTime = Time.time;
				this.InitCycle();
				num = 0f;
			}
			float t = num / (float)this.Period;
			for (int i = 0; i < this.NumberOfCurvePoints; i++)
			{
				this.curve[i].PositionLocal = Vector3.Lerp(this.oldCurvePointPos[i], this.newCurvePointPos[i], t);
			}
			for (int j = 0; j < this.NumberOfPointsToSeek; j++)
			{
				this.objects[j].transform.localPosition = Vector3.Lerp(this.oldPointPos[j], this.newPointPos[j], t);
			}
		}

		// Token: 0x060010F7 RID: 4343 RVA: 0x000E1834 File Offset: 0x000DFA34
		private void Calculate(object sender, EventArgs e)
		{
			for (int i = 0; i < this.NumberOfPointsToSeek; i++)
			{
				this.Process(i, false);
				if (this.HasError)
				{
					break;
				}
			}
		}

		// Token: 0x060010F8 RID: 4344 RVA: 0x000E1864 File Offset: 0x000DFA64
		private void Process(int i, bool suppressWarning = false)
		{
			Vector3 position = this.objects[i].transform.position;
			float num;
			Vector3 vector = this.math.CalcPositionByClosestPoint(position, out num, false, false);
			Debug.DrawLine(position, vector, Color.yellow);
			if (!this.CheckResults)
			{
				return;
			}
			float num2;
			Vector3 vector2 = BGTestCurveClosestPoint.CalcPositionByClosestPoint(this.math, position, out num2);
			Debug.DrawLine(position, vector2, Color.blue);
			bool flag = Math.Abs(num - num2) > 0.01f;
			bool flag2 = Vector3.Magnitude(vector - vector2) > 0.001f;
			if ((flag || flag2) && Mathf.Abs((position - vector).magnitude - (position - vector2).magnitude) > 1E-05f)
			{
				this.ErrorPointIndex = i;
				if (!suppressWarning)
				{
					Debug.Log("Error detected. Simulation stopped, but erroneous iteration's still running. Use debugger to debug the issue.");
					string[] array = new string[8];
					array[0] = "!!! Discrepancy detected while calculating pos by closest point: 1) [Using math] pos=";
					int num3 = 1;
					Vector3 vector3 = vector;
					array[num3] = vector3.ToString();
					array[2] = ", distance=";
					array[3] = num.ToString();
					array[4] = "  2) [Using check method] pos=";
					int num4 = 5;
					vector3 = vector2;
					array[num4] = vector3.ToString();
					array[6] = ", distance=";
					array[7] = num2.ToString();
					Debug.Log(string.Concat(array));
					if (flag2)
					{
						Debug.Log("Reason: Result points varies more than " + 1E-05f.ToString() + ". Difference=" + Vector3.Magnitude(vector - vector2).ToString());
					}
					if (flag)
					{
						Debug.Log("Reason: Distances varies more than 1cm. Difference=" + Math.Abs(num - num2).ToString());
					}
					Vector3 a = this.math.CalcByDistance(BGCurveBaseMath.Field.Position, num, false);
					Vector3 a2 = this.math.CalcByDistance(BGCurveBaseMath.Field.Position, num2, false);
					Debug.Log("Distance check: 1) [Using math] check=" + ((Vector3.SqrMagnitude(a - vector) < 1E-05f) ? "passed" : "failed") + "  2) [Using check method] check=" + ((Vector3.SqrMagnitude(a2 - vector2) < 1E-05f) ? "passed" : "failed"));
					float num5 = Vector3.Distance(position, vector);
					float num6 = Vector3.Distance(position, vector2);
					Debug.Log(string.Concat(new string[]
					{
						"Actual distance: 1) [Using math] Dist=",
						num5.ToString(),
						"  2) [Using check method] Dist=",
						num6.ToString(),
						(Math.Abs(num5 - num6) > 1E-05f) ? (". And the winner is " + ((num5 < num6) ? "math" : "check method")) : ""
					}));
				}
			}
		}

		// Token: 0x060010F9 RID: 4345 RVA: 0x000E1B00 File Offset: 0x000DFD00
		private static Vector3 CalcPositionByClosestPoint(BGCcMath math, Vector3 targetPoint, out float distance)
		{
			List<BGCurveBaseMath.SectionInfo> sectionInfos = math.Math.SectionInfos;
			int count = sectionInfos.Count;
			Vector3 result = sectionInfos[0][0].Position;
			float num = Vector3.SqrMagnitude(sectionInfos[0][0].Position - targetPoint);
			distance = 0f;
			for (int i = 0; i < count; i++)
			{
				BGCurveBaseMath.SectionInfo sectionInfo = sectionInfos[i];
				List<BGCurveBaseMath.SectionPointInfo> points = sectionInfo.Points;
				int count2 = points.Count;
				for (int j = 1; j < count2; j++)
				{
					BGCurveBaseMath.SectionPointInfo sectionPointInfo = points[j];
					float num2;
					Vector3 vector = BGTestCurveClosestPoint.CalcClosestPointToLine(points[j - 1].Position, sectionPointInfo.Position, targetPoint, out num2);
					float num3 = Vector3.SqrMagnitude(targetPoint - vector);
					if (num > num3)
					{
						num = num3;
						result = vector;
						if (num2 == 1f)
						{
							int index = i;
							int i2 = j;
							if (j == count2 - 1 && i < count - 1)
							{
								index = i + 1;
								i2 = 0;
							}
							distance = sectionInfos[index].DistanceFromStartToOrigin + sectionInfos[index][i2].DistanceToSectionStart;
						}
						else
						{
							distance = sectionInfos[i].DistanceFromStartToOrigin + Mathf.Lerp(sectionInfo[j - 1].DistanceToSectionStart, sectionPointInfo.DistanceToSectionStart, num2);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060010FA RID: 4346 RVA: 0x000E1C68 File Offset: 0x000DFE68
		private static Vector3 RandomVector()
		{
			return new Vector3(UnityEngine.Random.Range(BGTestCurveClosestPoint.min.x, BGTestCurveClosestPoint.max.x), UnityEngine.Random.Range(BGTestCurveClosestPoint.min.y, BGTestCurveClosestPoint.max.y), UnityEngine.Random.Range(BGTestCurveClosestPoint.min.z, BGTestCurveClosestPoint.max.z));
		}

		// Token: 0x060010FB RID: 4347 RVA: 0x000E1CC8 File Offset: 0x000DFEC8
		private static void InitArray(Vector3[] oldArray, Vector3[] newArray)
		{
			for (int i = 0; i < oldArray.Length; i++)
			{
				oldArray[i] = newArray[i];
				newArray[i] = BGTestCurveClosestPoint.RandomVector();
			}
		}

		// Token: 0x060010FC RID: 4348 RVA: 0x000E1D00 File Offset: 0x000DFF00
		private static Vector3 CalcClosestPointToLine(Vector3 a, Vector3 b, Vector3 p, out float ratio)
		{
			Vector3 lhs = p - a;
			Vector3 vector = b - a;
			float sqrMagnitude = vector.sqrMagnitude;
			if (Math.Abs(sqrMagnitude) < 1E-05f)
			{
				ratio = 1f;
				return b;
			}
			float num = Vector3.Dot(lhs, vector) / sqrMagnitude;
			if (num < 0f)
			{
				ratio = 0f;
				return a;
			}
			if (num > 1f)
			{
				ratio = 1f;
				return b;
			}
			ratio = num;
			return a + vector * num;
		}

		// Token: 0x04000E1C RID: 3612
		[Tooltip("Line renderer material")]
		public Material LineRendererMaterial;

		// Token: 0x04000E1D RID: 3613
		[Tooltip("Object to use for point's indication")]
		public GameObject PointIndicator;

		// Token: 0x04000E1E RID: 3614
		[Range(1f, 100f)]
		[Tooltip("How much points to use with search")]
		public int NumberOfPointsToSeek = 10;

		// Token: 0x04000E1F RID: 3615
		[Range(2f, 100f)]
		[Tooltip("How much points to add to the curve")]
		public int NumberOfCurvePoints = 100;

		// Token: 0x04000E20 RID: 3616
		[Range(1f, 30f)]
		[Tooltip("How much sections to use for splitting each curve's segment")]
		public int NumberOfSplits = 30;

		// Token: 0x04000E21 RID: 3617
		[Range(1f, 5f)]
		[Tooltip("Transition period")]
		public int Period = 4;

		// Token: 0x04000E22 RID: 3618
		[Tooltip("Use slow check method to validate results")]
		public bool CheckResults;

		// Token: 0x04000E23 RID: 3619
		private BGCurve curve;

		// Token: 0x04000E24 RID: 3620
		private BGCcMath math;

		// Token: 0x04000E25 RID: 3621
		private static Vector3 min = new Vector3(-10f, 0f, -2f);

		// Token: 0x04000E26 RID: 3622
		private static Vector3 max = new Vector3(10f, 10f, 2f);

		// Token: 0x04000E27 RID: 3623
		private GameObject[] objects;

		// Token: 0x04000E28 RID: 3624
		private Vector3[] oldCurvePointPos;

		// Token: 0x04000E29 RID: 3625
		private Vector3[] newCurvePointPos;

		// Token: 0x04000E2A RID: 3626
		private Vector3[] oldPointPos;

		// Token: 0x04000E2B RID: 3627
		private Vector3[] newPointPos;

		// Token: 0x04000E2C RID: 3628
		private float startTime = -100000f;

		// Token: 0x04000E2D RID: 3629
		private int ErrorPointIndex = -1;

		// Token: 0x04000E2E RID: 3630
		private GUIStyle style;
	}
}
