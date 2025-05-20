using System;
using System.Collections.Generic;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Example
{
	// Token: 0x020001E0 RID: 480
	public class BGTestCurveMath : MonoBehaviour
	{
		// Token: 0x06001104 RID: 4356 RVA: 0x000E1EBC File Offset: 0x000E00BC
		private void Start()
		{
			this.testCurves = new BGTestCurveMath.TestCurves(base.GetComponent<BGCurve>(), new BGCurveBaseMath.Config(BGCurveBaseMath.Fields.PositionAndTangent), this.ObjectToMove, this.LineRendererMaterial);
			this.testCurves.Add(new BGTestCurveMath.CurveData(this.testCurves, "BGBaseStraightLines", "Base, OptimizeStraightLines = true", base.transform.position + new Vector3(-4f, 1f), new BGCurveBaseMath.Config(BGCurveBaseMath.Fields.PositionAndTangent)
			{
				OptimizeStraightLines = true
			}, BGTestCurveMath.CurveData.MathTypeEnum.Base));
			this.testCurves.Add(new BGTestCurveMath.CurveData(this.testCurves, "BGBasePos2Tangents", "Base, UsePointPositionsToCalcTangents = true", base.transform.position + new Vector3(-4f, 4f), new BGCurveBaseMath.Config(BGCurveBaseMath.Fields.PositionAndTangent)
			{
				UsePointPositionsToCalcTangents = true
			}, BGTestCurveMath.CurveData.MathTypeEnum.Base));
			this.testCurves.Add(new BGTestCurveMath.CurveData(this.testCurves, "BGAdaptive", "Adaptive", base.transform.position + new Vector3(4f, 4f), new BGCurveAdaptiveMath.ConfigAdaptive(BGCurveBaseMath.Fields.PositionAndTangent), BGTestCurveMath.CurveData.MathTypeEnum.Adaptive));
			this.testCurves.Add(new BGTestCurveMath.CurveData(this.testCurves, "BGFormula", "Formula", base.transform.position + new Vector3(4f, 1f), new BGCurveBaseMath.Config(BGCurveBaseMath.Fields.PositionAndTangent), BGTestCurveMath.CurveData.MathTypeEnum.Formula));
		}

		// Token: 0x06001105 RID: 4357 RVA: 0x0004E1B5 File Offset: 0x0004C3B5
		private void Update()
		{
			this.testCurves.Update();
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				this.testCurves.MoveLeft();
			}
			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				this.testCurves.MoveRight();
			}
		}

		// Token: 0x06001106 RID: 4358 RVA: 0x000E2014 File Offset: 0x000E0214
		private void OnGUI()
		{
			if (this.style == null)
			{
				this.style = new GUIStyle(GUI.skin.label)
				{
					fontSize = 18
				};
			}
			GUI.Label(new Rect(0f, 24f, 800f, 30f), "Left Arrow - move left, Right Arrow - move right", this.style);
			GUI.Label(new Rect(0f, 48f, 800f, 30f), "Comparing with: " + this.testCurves.CurrentToString(), this.style);
		}

		// Token: 0x04000E36 RID: 3638
		[Tooltip("Material to use with LineRenderer")]
		public Material LineRendererMaterial;

		// Token: 0x04000E37 RID: 3639
		[Tooltip("Object to move along a curve")]
		public MeshRenderer ObjectToMove;

		// Token: 0x04000E38 RID: 3640
		private const float Period = 3f;

		// Token: 0x04000E39 RID: 3641
		private const int ObjectsCount = 4;

		// Token: 0x04000E3A RID: 3642
		private const float ObjectsSpeed = 0.3f;

		// Token: 0x04000E3B RID: 3643
		private BGTestCurveMath.TestCurves testCurves;

		// Token: 0x04000E3C RID: 3644
		private GUIStyle style;

		// Token: 0x020001E1 RID: 481
		private abstract class CurveDataAbstract
		{
			// Token: 0x17000495 RID: 1173
			// (get) Token: 0x06001108 RID: 4360 RVA: 0x0004E1F0 File Offset: 0x0004C3F0
			// (set) Token: 0x06001109 RID: 4361 RVA: 0x0004E1F8 File Offset: 0x0004C3F8
			public BGCurve Curve
			{
				get
				{
					return this.curve;
				}
				protected set
				{
					this.curve = value;
					this.curve.Changed += delegate(object sender, BGCurveChangedArgs args)
					{
						this.UpdateLineRenderer();
					};
				}
			}

			// Token: 0x0600110A RID: 4362 RVA: 0x000E20A8 File Offset: 0x000E02A8
			protected CurveDataAbstract(GameObject gameObject, Material lineRendererMaterial, Color color)
			{
				this.GameObject = gameObject;
				this.LineRendererMaterial = lineRendererMaterial;
				this.objectToMoveMaterial = UnityEngine.Object.Instantiate<Material>(lineRendererMaterial);
				this.objectToMoveMaterial.SetColor("_TintColor", color);
				this.lineRenderer = gameObject.AddComponent<LineRenderer>();
				this.lineRenderer.material = lineRendererMaterial;
				this.lineRenderer.startWidth = (this.lineRenderer.endWidth = 0.05f);
				LineRenderer lineRenderer = this.lineRenderer;
				this.lineRenderer.endColor = color;
				lineRenderer.startColor = color;
			}

			// Token: 0x0600110B RID: 4363 RVA: 0x000E2144 File Offset: 0x000E0344
			private void UpdateLineRenderer()
			{
				Vector3[] array = new Vector3[100];
				for (int i = 0; i < 100; i++)
				{
					float distanceRatio = (float)i / 99f;
					array[i] = this.Math.CalcByDistanceRatio(BGCurveBaseMath.Field.Position, distanceRatio, false);
				}
				this.lineRenderer.positionCount = 100;
				this.lineRenderer.SetPositions(array);
			}

			// Token: 0x0600110C RID: 4364 RVA: 0x000E21A0 File Offset: 0x000E03A0
			protected void AddObjects(int count, MeshRenderer pattern, Transform parent)
			{
				for (int i = 0; i < count; i++)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(pattern.gameObject);
					gameObject.transform.parent = parent;
					this.AddObject(gameObject);
				}
			}

			// Token: 0x0600110D RID: 4365 RVA: 0x0004E218 File Offset: 0x0004C418
			protected void AddObject(GameObject obj)
			{
				obj.GetComponent<MeshRenderer>().sharedMaterial = this.objectToMoveMaterial;
				this.objectsToMove.Add(obj.gameObject);
			}

			// Token: 0x0600110E RID: 4366 RVA: 0x000E21D8 File Offset: 0x000E03D8
			protected void UpdateObjects(List<float> distanceRatios)
			{
				for (int i = 0; i < this.objectsToMove.Count; i++)
				{
					Vector3 vector = this.Math.CalcByDistanceRatio(BGCurveBaseMath.Field.Position, distanceRatios[i], false);
					Vector3 b = this.Math.CalcByDistanceRatio(BGCurveBaseMath.Field.Tangent, distanceRatios[i], false);
					this.objectsToMove[i].transform.position = vector;
					this.objectsToMove[i].transform.LookAt(vector + b);
				}
			}

			// Token: 0x0600110F RID: 4367
			public abstract void Update();

			// Token: 0x04000E3D RID: 3645
			private readonly List<GameObject> objectsToMove = new List<GameObject>();

			// Token: 0x04000E3E RID: 3646
			private readonly Material objectToMoveMaterial;

			// Token: 0x04000E3F RID: 3647
			private readonly LineRenderer lineRenderer;

			// Token: 0x04000E40 RID: 3648
			public readonly Material LineRendererMaterial;

			// Token: 0x04000E41 RID: 3649
			protected readonly GameObject GameObject;

			// Token: 0x04000E42 RID: 3650
			protected BGCurveBaseMath Math;

			// Token: 0x04000E43 RID: 3651
			private BGCurve curve;
		}

		// Token: 0x020001E2 RID: 482
		private sealed class TestCurves : BGTestCurveMath.CurveDataAbstract
		{
			// Token: 0x06001111 RID: 4369 RVA: 0x000E225C File Offset: 0x000E045C
			public TestCurves(BGCurve curve, BGCurveBaseMath.Config config, MeshRenderer objectToMove, Material lineRendererMaterial) : base(curve.gameObject, lineRendererMaterial, Color.green)
			{
				base.Curve = curve;
				this.Math = new BGCurveBaseMath(curve, config);
				this.ObjectToMove = objectToMove;
				base.AddObject(objectToMove.gameObject);
				base.AddObjects(3, objectToMove, curve.transform);
				for (int i = 0; i < 4; i++)
				{
					this.DistanceRatios.Add((float)i * 0.25f);
				}
			}

			// Token: 0x06001112 RID: 4370 RVA: 0x0004E244 File Offset: 0x0004C444
			public void MoveRight()
			{
				this.currentCurveIndex++;
				if (this.currentCurveIndex == this.curves.Count)
				{
					this.currentCurveIndex = 0;
				}
			}

			// Token: 0x06001113 RID: 4371 RVA: 0x0004E26E File Offset: 0x0004C46E
			public void MoveLeft()
			{
				this.currentCurveIndex--;
				if (this.currentCurveIndex < 0)
				{
					this.currentCurveIndex = this.curves.Count - 1;
				}
			}

			// Token: 0x06001114 RID: 4372 RVA: 0x000E22F8 File Offset: 0x000E04F8
			public override void Update()
			{
				if (Time.time - this.startTime > 3f)
				{
					this.startTime = Time.time;
					this.fromRotation = base.Curve.transform.rotation;
					this.toRotation = Quaternion.Euler(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f));
				}
				float t = (Time.time - this.startTime) / 3f;
				base.Curve.transform.rotation = Quaternion.Lerp(this.fromRotation, this.toRotation, t);
				for (int i = 0; i < this.DistanceRatios.Count; i++)
				{
					List<float> distanceRatios = this.DistanceRatios;
					int index = i;
					distanceRatios[index] += 0.3f * Time.deltaTime;
					if (this.DistanceRatios[i] > 1f)
					{
						this.DistanceRatios[i] = 0f;
					}
				}
				base.UpdateObjects(this.DistanceRatios);
				foreach (BGTestCurveMath.CurveData curveData in this.curves)
				{
					curveData.Update();
				}
			}

			// Token: 0x06001115 RID: 4373 RVA: 0x0004E29A File Offset: 0x0004C49A
			public bool IsCurrent(BGTestCurveMath.CurveData curve)
			{
				return this.currentCurveIndex >= 0 && this.currentCurveIndex < this.curves.Count && this.curves[this.currentCurveIndex] == curve;
			}

			// Token: 0x06001116 RID: 4374 RVA: 0x0004E2CE File Offset: 0x0004C4CE
			public void Add(BGTestCurveMath.CurveData curveData)
			{
				this.curves.Add(curveData);
			}

			// Token: 0x06001117 RID: 4375 RVA: 0x0004E2DC File Offset: 0x0004C4DC
			public string CurrentToString()
			{
				if (this.currentCurveIndex >= 0)
				{
					return this.curves[this.currentCurveIndex].Description;
				}
				return "None";
			}

			// Token: 0x04000E44 RID: 3652
			public readonly List<float> DistanceRatios = new List<float>();

			// Token: 0x04000E45 RID: 3653
			public readonly MeshRenderer ObjectToMove;

			// Token: 0x04000E46 RID: 3654
			private readonly List<BGTestCurveMath.CurveData> curves = new List<BGTestCurveMath.CurveData>();

			// Token: 0x04000E47 RID: 3655
			private float startTime = -6f;

			// Token: 0x04000E48 RID: 3656
			private Quaternion fromRotation;

			// Token: 0x04000E49 RID: 3657
			private Quaternion toRotation;

			// Token: 0x04000E4A RID: 3658
			private int currentCurveIndex = -1;
		}

		// Token: 0x020001E3 RID: 483
		private sealed class CurveData : BGTestCurveMath.CurveDataAbstract
		{
			// Token: 0x17000496 RID: 1174
			// (get) Token: 0x06001118 RID: 4376 RVA: 0x0004E303 File Offset: 0x0004C503
			public string Description
			{
				get
				{
					return this.description;
				}
			}

			// Token: 0x06001119 RID: 4377 RVA: 0x000E2454 File Offset: 0x000E0654
			public CurveData(BGTestCurveMath.TestCurves testCurves, string name, string description, Vector3 position, BGCurveBaseMath.Config config, BGTestCurveMath.CurveData.MathTypeEnum mathType) : base(new GameObject(name), testCurves.LineRendererMaterial, Color.magenta)
			{
				this.testCurves = testCurves;
				this.description = description;
				this.GameObject.transform.position = position;
				this.origin = position;
				base.Curve = this.GameObject.AddComponent<BGCurve>();
				base.Curve.Closed = testCurves.Curve.Closed;
				for (int i = 0; i < testCurves.Curve.PointsCount; i++)
				{
					BGCurvePointI bgcurvePointI = testCurves.Curve[i];
					BGCurvePoint point = new BGCurvePoint(base.Curve, bgcurvePointI.PositionLocal, bgcurvePointI.ControlType, bgcurvePointI.ControlFirstLocal, bgcurvePointI.ControlSecondLocal, false);
					base.Curve.AddPoint(point);
				}
				switch (mathType)
				{
				case BGTestCurveMath.CurveData.MathTypeEnum.Base:
					this.Math = new BGCurveBaseMath(base.Curve, config);
					break;
				case BGTestCurveMath.CurveData.MathTypeEnum.Formula:
					this.Math = new BGCurveFormulaMath(base.Curve, config);
					break;
				case BGTestCurveMath.CurveData.MathTypeEnum.Adaptive:
					this.Math = new BGCurveAdaptiveMath(base.Curve, (BGCurveAdaptiveMath.ConfigAdaptive)config);
					break;
				default:
					throw new ArgumentOutOfRangeException("mathType", mathType, null);
				}
				base.AddObjects(4, testCurves.ObjectToMove, this.GameObject.transform);
				this.GameObject.transform.localScale = this.originalScale;
			}

			// Token: 0x0600111A RID: 4378 RVA: 0x000E25D0 File Offset: 0x000E07D0
			public override void Update()
			{
				Transform transform = base.Curve.gameObject.transform;
				Transform transform2 = this.testCurves.Curve.transform;
				transform.rotation = transform2.rotation;
				float num = 10f * Time.deltaTime;
				bool flag = this.testCurves.IsCurrent(this);
				transform.position = Vector3.MoveTowards(transform.position, flag ? transform2.position : this.origin, num);
				transform.localScale = Vector3.MoveTowards(transform.localScale, flag ? transform2.transform.localScale : this.originalScale, num / 4f);
				base.UpdateObjects(this.testCurves.DistanceRatios);
			}

			// Token: 0x04000E4B RID: 3659
			private readonly Vector3 origin;

			// Token: 0x04000E4C RID: 3660
			private readonly BGTestCurveMath.TestCurves testCurves;

			// Token: 0x04000E4D RID: 3661
			private readonly Vector3 originalScale = new Vector3(0.7f, 0.7f, 0.7f);

			// Token: 0x04000E4E RID: 3662
			private readonly string description;

			// Token: 0x020001E4 RID: 484
			public enum MathTypeEnum
			{
				// Token: 0x04000E50 RID: 3664
				Base,
				// Token: 0x04000E51 RID: 3665
				Formula,
				// Token: 0x04000E52 RID: 3666
				Adaptive
			}
		}
	}
}
