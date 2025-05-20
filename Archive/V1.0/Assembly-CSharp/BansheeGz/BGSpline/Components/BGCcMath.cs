using System;
using BansheeGz.BGSpline.Curve;
using UnityEngine;
using UnityEngine.Events;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001B8 RID: 440
	[HelpURL("http://www.bansheegz.com/BGCurve/Cc/BGCcMath")]
	[DisallowMultipleComponent]
	[BGCc.CcDescriptor(Description = "Math solver for the curve (position, tangent, total distance, position by closest point). With this component you can use math functions.", Name = "Math", Icon = "BGCcMath123")]
	[AddComponentMenu("BansheeGz/BGCurve/Components/BGCcMath")]
	public class BGCcMath : BGCc, BGCurveMathI
	{
		// Token: 0x14000010 RID: 16
		// (add) Token: 0x06000FAB RID: 4011 RVA: 0x000DD7E8 File Offset: 0x000DB9E8
		// (remove) Token: 0x06000FAC RID: 4012 RVA: 0x000DD820 File Offset: 0x000DBA20
		public event EventHandler ChangedMath;

		// Token: 0x17000440 RID: 1088
		// (get) Token: 0x06000FAD RID: 4013 RVA: 0x0004D218 File Offset: 0x0004B418
		// (set) Token: 0x06000FAE RID: 4014 RVA: 0x0004D220 File Offset: 0x0004B420
		public BGCcMath.MathTypeEnum MathType
		{
			get
			{
				return this.mathType;
			}
			set
			{
				base.ParamChanged<BGCcMath.MathTypeEnum>(ref this.mathType, value);
			}
		}

		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x06000FAF RID: 4015 RVA: 0x0004D230 File Offset: 0x0004B430
		// (set) Token: 0x06000FB0 RID: 4016 RVA: 0x0004D240 File Offset: 0x0004B440
		public int SectionParts
		{
			get
			{
				return Mathf.Clamp(this.sectionParts, 1, 100);
			}
			set
			{
				base.ParamChanged<int>(ref this.sectionParts, Mathf.Clamp(value, 1, 100));
			}
		}

		// Token: 0x17000442 RID: 1090
		// (get) Token: 0x06000FB1 RID: 4017 RVA: 0x0004D258 File Offset: 0x0004B458
		// (set) Token: 0x06000FB2 RID: 4018 RVA: 0x0004D260 File Offset: 0x0004B460
		public bool OptimizeStraightLines
		{
			get
			{
				return this.optimizeStraightLines;
			}
			set
			{
				base.ParamChanged<bool>(ref this.optimizeStraightLines, value);
			}
		}

		// Token: 0x17000443 RID: 1091
		// (get) Token: 0x06000FB3 RID: 4019 RVA: 0x0004D270 File Offset: 0x0004B470
		// (set) Token: 0x06000FB4 RID: 4020 RVA: 0x0004D278 File Offset: 0x0004B478
		public float Tolerance
		{
			get
			{
				return this.tolerance;
			}
			set
			{
				base.ParamChanged<float>(ref this.tolerance, value);
			}
		}

		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x06000FB5 RID: 4021 RVA: 0x0004D288 File Offset: 0x0004B488
		// (set) Token: 0x06000FB6 RID: 4022 RVA: 0x0004D290 File Offset: 0x0004B490
		public BGCurveBaseMath.Fields Fields
		{
			get
			{
				return this.fields;
			}
			set
			{
				base.ParamChanged<BGCurveBaseMath.Fields>(ref this.fields, value);
			}
		}

		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x06000FB7 RID: 4023 RVA: 0x0004D2A0 File Offset: 0x0004B4A0
		// (set) Token: 0x06000FB8 RID: 4024 RVA: 0x0004D2A8 File Offset: 0x0004B4A8
		public bool UsePositionToCalculateTangents
		{
			get
			{
				return this.usePositionToCalculateTangents;
			}
			set
			{
				base.ParamChanged<bool>(ref this.usePositionToCalculateTangents, value);
			}
		}

		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x06000FB9 RID: 4025 RVA: 0x0004D2B8 File Offset: 0x0004B4B8
		// (set) Token: 0x06000FBA RID: 4026 RVA: 0x0004D2C0 File Offset: 0x0004B4C0
		public BGCcMath.UpdateModeEnum UpdateMode
		{
			get
			{
				return this.updateMode;
			}
			set
			{
				base.ParamChanged<BGCcMath.UpdateModeEnum>(ref this.updateMode, value);
			}
		}

		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x06000FBB RID: 4027 RVA: 0x0004D2D0 File Offset: 0x0004B4D0
		// (set) Token: 0x06000FBC RID: 4028 RVA: 0x0004D2D8 File Offset: 0x0004B4D8
		public Renderer RendererForUpdateCheck
		{
			get
			{
				return this.rendererForUpdateCheck;
			}
			set
			{
				base.ParamChanged<Renderer>(ref this.rendererForUpdateCheck, value);
			}
		}

		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x06000FBD RID: 4029 RVA: 0x0004D2E8 File Offset: 0x0004B4E8
		public override string Error
		{
			get
			{
				if (this.updateMode != BGCcMath.UpdateModeEnum.RendererVisible || !(this.RendererForUpdateCheck == null))
				{
					return null;
				}
				return "Update mode is set to " + this.updateMode.ToString() + ", however the RendererForUpdateCheck is null";
			}
		}

		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x06000FBE RID: 4030 RVA: 0x0004D323 File Offset: 0x0004B523
		public override string Warning
		{
			get
			{
				if (base.Curve.SnapType == BGCurve.SnapTypeEnum.Curve && this.Fields == BGCurveBaseMath.Fields.PositionAndTangent && !this.UsePositionToCalculateTangents)
				{
					return "Your curve's snap mode is Curve, and you are calculating tangents. However you use formula for tangents, instead of points positions.This may result in wrong tangents. Set UsePositionToCalculateTangents to true.";
				}
				return null;
			}
		}

		// Token: 0x1700044A RID: 1098
		// (get) Token: 0x06000FBF RID: 4031 RVA: 0x000DD858 File Offset: 0x000DBA58
		public override string Info
		{
			get
			{
				if (this.Math != null)
				{
					return "Math uses " + this.Math.PointsCount.ToString() + " points";
				}
				return null;
			}
		}

		// Token: 0x1700044B RID: 1099
		// (get) Token: 0x06000FC0 RID: 4032 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool SupportHandles
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700044C RID: 1100
		// (get) Token: 0x06000FC1 RID: 4033 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool SupportHandlesSettings
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700044D RID: 1101
		// (get) Token: 0x06000FC2 RID: 4034 RVA: 0x0004D34B File Offset: 0x0004B54B
		public BGCurveBaseMath Math
		{
			get
			{
				if (this.math == null)
				{
					this.InitMath(null, null);
				}
				return this.math;
			}
		}

		// Token: 0x1700044E RID: 1102
		// (get) Token: 0x06000FC3 RID: 4035 RVA: 0x000DD894 File Offset: 0x000DBA94
		private bool NewMathRequired
		{
			get
			{
				return this.math == null || (this.mathType == BGCcMath.MathTypeEnum.Base && this.math.GetType() != typeof(BGCurveBaseMath)) || (this.mathType == BGCcMath.MathTypeEnum.Adaptive && this.math.GetType() != typeof(BGCurveAdaptiveMath));
			}
		}

		// Token: 0x06000FC4 RID: 4036 RVA: 0x0004D363 File Offset: 0x0004B563
		public override void Start()
		{
			base.Curve.Changed += this.SendEventsIfMathIsNotCreated;
		}

		// Token: 0x06000FC5 RID: 4037 RVA: 0x000DD8F4 File Offset: 0x000DBAF4
		public override void OnDestroy()
		{
			if (this.math != null)
			{
				this.math.Changed -= this.MathWasChanged;
				this.math.ChangeRequested -= this.MathOnChangeRequested;
				this.math.Dispose();
			}
			base.ChangedParams -= this.InitMath;
		}

		// Token: 0x06000FC6 RID: 4038 RVA: 0x0004D37C File Offset: 0x0004B57C
		public void EnsureMathIsCreated()
		{
			BGCurveBaseMath bgcurveBaseMath = this.Math;
		}

		// Token: 0x06000FC7 RID: 4039 RVA: 0x0004D385 File Offset: 0x0004B585
		public void Recalculate(bool force = false)
		{
			this.Math.Recalculate(force);
		}

		// Token: 0x06000FC8 RID: 4040 RVA: 0x000DD954 File Offset: 0x000DBB54
		public bool IsCalculated(BGCurveBaseMath.Field field)
		{
			BGCurveBaseMath bgcurveBaseMath = this.Math;
			return bgcurveBaseMath != null && bgcurveBaseMath.IsCalculated(field);
		}

		// Token: 0x06000FC9 RID: 4041 RVA: 0x000DD974 File Offset: 0x000DBB74
		public float ClampDistance(float distance)
		{
			BGCurveBaseMath bgcurveBaseMath = this.Math;
			if (distance < 0f)
			{
				return 0f;
			}
			if (distance <= bgcurveBaseMath.GetDistance(-1))
			{
				return distance;
			}
			return bgcurveBaseMath.GetDistance(-1);
		}

		// Token: 0x06000FCA RID: 4042 RVA: 0x0004D393 File Offset: 0x0004B593
		public float GetDistance(int pointIndex = -1)
		{
			return this.Math.GetDistance(pointIndex);
		}

		// Token: 0x06000FCB RID: 4043 RVA: 0x0004D3A1 File Offset: 0x0004B5A1
		public Vector3 CalcByDistanceRatio(BGCurveBaseMath.Field field, float ratio, bool useLocal = false)
		{
			return this.Math.CalcByDistanceRatio(field, ratio, useLocal);
		}

		// Token: 0x06000FCC RID: 4044 RVA: 0x0004D3B1 File Offset: 0x0004B5B1
		public Vector3 CalcByDistanceRatio(float distanceRatio, out Vector3 tangent, bool useLocal = false)
		{
			return this.Math.CalcByDistanceRatio(distanceRatio, out tangent, useLocal);
		}

		// Token: 0x06000FCD RID: 4045 RVA: 0x0004D3C1 File Offset: 0x0004B5C1
		public Vector3 CalcPositionByDistanceRatio(float ratio, bool useLocal = false)
		{
			return this.Math.CalcByDistanceRatio(BGCurveBaseMath.Field.Position, ratio, useLocal);
		}

		// Token: 0x06000FCE RID: 4046 RVA: 0x0004D3D1 File Offset: 0x0004B5D1
		public Vector3 CalcTangentByDistanceRatio(float ratio, bool useLocal = false)
		{
			return this.Math.CalcByDistanceRatio(BGCurveBaseMath.Field.Tangent, ratio, useLocal);
		}

		// Token: 0x06000FCF RID: 4047 RVA: 0x0004D3E1 File Offset: 0x0004B5E1
		public Vector3 CalcPositionAndTangentByDistanceRatio(float distanceRatio, out Vector3 tangent, bool useLocal = false)
		{
			return this.Math.CalcPositionAndTangentByDistanceRatio(distanceRatio, out tangent, useLocal);
		}

		// Token: 0x06000FD0 RID: 4048 RVA: 0x0004D3F1 File Offset: 0x0004B5F1
		public Vector3 CalcByDistance(BGCurveBaseMath.Field field, float distance, bool useLocal = false)
		{
			return this.Math.CalcByDistance(field, distance, useLocal);
		}

		// Token: 0x06000FD1 RID: 4049 RVA: 0x0004D401 File Offset: 0x0004B601
		public Vector3 CalcByDistance(float distance, out Vector3 tangent, bool useLocal = false)
		{
			return this.Math.CalcByDistance(distance, out tangent, useLocal);
		}

		// Token: 0x06000FD2 RID: 4050 RVA: 0x0004D411 File Offset: 0x0004B611
		public Vector3 CalcPositionByDistance(float distance, bool useLocal = false)
		{
			return this.Math.CalcByDistance(BGCurveBaseMath.Field.Position, distance, useLocal);
		}

		// Token: 0x06000FD3 RID: 4051 RVA: 0x0004D421 File Offset: 0x0004B621
		public Vector3 CalcTangentByDistance(float distance, bool useLocal = false)
		{
			return this.Math.CalcByDistance(BGCurveBaseMath.Field.Tangent, distance, useLocal);
		}

		// Token: 0x06000FD4 RID: 4052 RVA: 0x0004D431 File Offset: 0x0004B631
		public Vector3 CalcPositionAndTangentByDistance(float distance, out Vector3 tangent, bool useLocal = false)
		{
			return this.Math.CalcPositionAndTangentByDistance(distance, out tangent, useLocal);
		}

		// Token: 0x1700044F RID: 1103
		public BGCurveBaseMath.SectionInfo this[int i]
		{
			get
			{
				return this.Math[i];
			}
		}

		// Token: 0x06000FD6 RID: 4054 RVA: 0x0004D44F File Offset: 0x0004B64F
		public Vector3 CalcPositionByClosestPoint(Vector3 point, out float distance, out Vector3 tangent, bool skipSectionsOptimization = false, bool skipPointsOptimization = false)
		{
			return this.Math.CalcPositionByClosestPoint(point, out distance, out tangent, skipSectionsOptimization, skipPointsOptimization);
		}

		// Token: 0x06000FD7 RID: 4055 RVA: 0x0004D463 File Offset: 0x0004B663
		public Vector3 CalcPositionByClosestPoint(Vector3 point, out float distance, bool skipSectionsOptimization = false, bool skipPointsOptimization = false)
		{
			return this.Math.CalcPositionByClosestPoint(point, out distance, skipSectionsOptimization, skipPointsOptimization);
		}

		// Token: 0x06000FD8 RID: 4056 RVA: 0x0004D475 File Offset: 0x0004B675
		public Vector3 CalcPositionByClosestPoint(Vector3 point, bool skipSectionsOptimization = false, bool skipPointsOptimization = false)
		{
			return this.Math.CalcPositionByClosestPoint(point, skipSectionsOptimization, skipPointsOptimization);
		}

		// Token: 0x06000FD9 RID: 4057 RVA: 0x0004D485 File Offset: 0x0004B685
		public int CalcSectionIndexByDistance(float distance)
		{
			return this.Math.CalcSectionIndexByDistance(distance);
		}

		// Token: 0x06000FDA RID: 4058 RVA: 0x0004D493 File Offset: 0x0004B693
		public int CalcSectionIndexByDistanceRatio(float distanceRatio)
		{
			return this.Math.CalcSectionIndexByDistanceRatio(distanceRatio);
		}

		// Token: 0x06000FDB RID: 4059 RVA: 0x0004D4A1 File Offset: 0x0004B6A1
		private void SendEventsIfMathIsNotCreated(object sender, BGCurveChangedArgs e)
		{
			if (this.math != null)
			{
				base.Curve.Changed -= this.SendEventsIfMathIsNotCreated;
				return;
			}
			this.MathWasChanged(sender, e);
		}

		// Token: 0x06000FDC RID: 4060 RVA: 0x000DD9AC File Offset: 0x000DBBAC
		private void InitMath(object sender, EventArgs e)
		{
			BGCurveBaseMath.Config config;
			if (this.mathType != BGCcMath.MathTypeEnum.Adaptive)
			{
				(config = new BGCurveBaseMath.Config(this.fields)).Parts = this.sectionParts;
			}
			else
			{
				(config = new BGCurveAdaptiveMath.ConfigAdaptive(this.fields)).Tolerance = this.tolerance;
			}
			BGCurveBaseMath.Config config2 = config;
			config2.UsePointPositionsToCalcTangents = this.usePositionToCalculateTangents;
			config2.OptimizeStraightLines = this.optimizeStraightLines;
			config2.Fields = this.fields;
			if (this.updateMode != BGCcMath.UpdateModeEnum.Always && Application.isPlaying)
			{
				BGCcMath.UpdateModeEnum updateModeEnum = this.updateMode;
				if (updateModeEnum != BGCcMath.UpdateModeEnum.AabbVisible)
				{
					if (updateModeEnum == BGCcMath.UpdateModeEnum.RendererVisible)
					{
						this.InitRendererVisible(config2);
					}
				}
				else
				{
					this.InitAabbVisibleBefore(config2);
				}
			}
			if (this.NewMathRequired)
			{
				bool flag = this.math == null;
				if (flag)
				{
					base.ChangedParams += this.InitMath;
				}
				else
				{
					this.math.ChangeRequested -= this.MathOnChangeRequested;
					this.math.Changed -= this.MathWasChanged;
					this.math.Dispose();
				}
				if (this.MathType == BGCcMath.MathTypeEnum.Base)
				{
					this.math = new BGCurveBaseMath(base.Curve, config2);
				}
				else
				{
					this.math = new BGCurveAdaptiveMath(base.Curve, (BGCurveAdaptiveMath.ConfigAdaptive)config2);
				}
				this.math.Changed += this.MathWasChanged;
				if (!flag)
				{
					this.MathWasChanged(this, null);
				}
			}
			else
			{
				this.math.ChangeRequested -= this.MathOnChangeRequested;
				this.math.Init(config2);
			}
			if (this.updateMode == BGCcMath.UpdateModeEnum.AabbVisible)
			{
				this.InitAabbVisibleAfter();
			}
		}

		// Token: 0x06000FDD RID: 4061 RVA: 0x000DDB34 File Offset: 0x000DBD34
		private void InitAabbVisibleBefore(BGCurveBaseMath.Config config)
		{
			if (!Application.isPlaying)
			{
				return;
			}
			if (this.meshFilter != null)
			{
				UnityEngine.Object.Destroy(this.meshFilter.gameObject);
			}
			GameObject gameObject = new GameObject("AabbBox");
			gameObject.transform.parent = base.transform;
			MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
			this.meshFilter = gameObject.AddComponent<MeshFilter>();
			this.meshFilter.mesh = new Mesh();
			this.InitVisibilityCheck(config, renderer);
			this.MathOnChangeRequested(this, null);
		}

		// Token: 0x06000FDE RID: 4062 RVA: 0x0004D4CB File Offset: 0x0004B6CB
		private void InitAabbVisibleAfter()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			this.math.ChangeRequested += this.MathOnChangeRequested;
		}

		// Token: 0x06000FDF RID: 4063 RVA: 0x000DDBB8 File Offset: 0x000DBDB8
		private void MathOnChangeRequested(object sender, EventArgs eventArgs)
		{
			if (!Application.isPlaying)
			{
				return;
			}
			if (this.visibilityCheck == null || this.visibilityCheck.Visible)
			{
				return;
			}
			BGCurvePointI[] points = base.Curve.Points;
			Mesh sharedMesh = this.meshFilter.sharedMesh;
			int num = points.Length;
			if (num == 0)
			{
				sharedMesh.vertices = BGCcMath.EmptyVertices;
				return;
			}
			if (num != 1)
			{
				Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
				Vector3 positionWorld = points[0].PositionWorld;
				int num2 = points.Length;
				int num3 = num2 - 1;
				bool closed = base.Curve.Closed;
				Vector3 vector = positionWorld;
				Vector3 vector2 = positionWorld;
				for (int i = 0; i < num2; i++)
				{
					BGCurvePointI bgcurvePointI = points[i];
					Vector3 positionLocal = bgcurvePointI.PositionLocal;
					Vector3 vector3 = localToWorldMatrix.MultiplyPoint(positionLocal);
					if (vector.x > vector3.x)
					{
						vector.x = vector3.x;
					}
					if (vector.y > vector3.y)
					{
						vector.y = vector3.y;
					}
					if (vector.z > vector3.z)
					{
						vector.z = vector3.z;
					}
					if (vector2.x < vector3.x)
					{
						vector2.x = vector3.x;
					}
					if (vector2.y < vector3.y)
					{
						vector2.y = vector3.y;
					}
					if (vector2.z < vector3.z)
					{
						vector2.z = vector3.z;
					}
					if (bgcurvePointI.ControlType != BGCurvePoint.ControlTypeEnum.Absent)
					{
						if (closed || i != 0)
						{
							Vector3 vector4 = localToWorldMatrix.MultiplyPoint(bgcurvePointI.ControlFirstLocal + positionLocal);
							if (vector.x > vector4.x)
							{
								vector.x = vector4.x;
							}
							if (vector.y > vector4.y)
							{
								vector.y = vector4.y;
							}
							if (vector.z > vector4.z)
							{
								vector.z = vector4.z;
							}
							if (vector2.x < vector4.x)
							{
								vector2.x = vector4.x;
							}
							if (vector2.y < vector4.y)
							{
								vector2.y = vector4.y;
							}
							if (vector2.z < vector4.z)
							{
								vector2.z = vector4.z;
							}
						}
						if (closed || i != num3)
						{
							Vector3 vector5 = localToWorldMatrix.MultiplyPoint(bgcurvePointI.ControlSecondLocal + positionLocal);
							if (vector.x > vector5.x)
							{
								vector.x = vector5.x;
							}
							if (vector.y > vector5.y)
							{
								vector.y = vector5.y;
							}
							if (vector.z > vector5.z)
							{
								vector.z = vector5.z;
							}
							if (vector2.x < vector5.x)
							{
								vector2.x = vector5.x;
							}
							if (vector2.y < vector5.y)
							{
								vector2.y = vector5.y;
							}
							if (vector2.z < vector5.z)
							{
								vector2.z = vector5.z;
							}
						}
					}
				}
				this.vertices[0] = vector;
				this.vertices[1] = vector2;
				sharedMesh.vertices = this.vertices;
				return;
			}
			this.vertices[0] = points[0].PositionWorld;
			this.vertices[1] = this.vertices[0];
			sharedMesh.vertices = this.vertices;
		}

		// Token: 0x06000FE0 RID: 4064 RVA: 0x0004D4EC File Offset: 0x0004B6EC
		private void InitRendererVisible(BGCurveBaseMath.Config config)
		{
			this.InitVisibilityCheck(config, this.RendererForUpdateCheck);
		}

		// Token: 0x06000FE1 RID: 4065 RVA: 0x000DDF4C File Offset: 0x000DC14C
		private void InitVisibilityCheck(BGCurveBaseMath.Config config, Renderer renderer)
		{
			if (this.visibilityCheck != null)
			{
				this.visibilityCheck.BecameVisible -= this.BecameVisible;
				UnityEngine.Object.Destroy(this.visibilityCheck);
			}
			if (renderer == null)
			{
				return;
			}
			this.visibilityCheck = renderer.gameObject.AddComponent<BGCcMath.VisibilityCheck>();
			this.visibilityCheck.BecameVisible += this.BecameVisible;
			config.ShouldUpdate = (() => this.visibilityCheck.Visible);
		}

		// Token: 0x06000FE2 RID: 4066 RVA: 0x0004D4FB File Offset: 0x0004B6FB
		private void BecameVisible(object sender, EventArgs e)
		{
			this.math.Configuration.FireUpdate();
		}

		// Token: 0x06000FE3 RID: 4067 RVA: 0x0004D50D File Offset: 0x0004B70D
		private void MathWasChanged(object sender, EventArgs e)
		{
			if (this.ChangedMath != null)
			{
				this.ChangedMath(this, null);
			}
			if (this.mathChangedEvent.GetPersistentEventCount() > 0)
			{
				this.mathChangedEvent.Invoke();
			}
		}

		// Token: 0x04000D60 RID: 3424
		private const int PartsMax = 100;

		// Token: 0x04000D61 RID: 3425
		private static readonly Vector3[] EmptyVertices = new Vector3[0];

		// Token: 0x04000D63 RID: 3427
		[SerializeField]
		[Tooltip("Which fields you want to use.")]
		private BGCurveBaseMath.Fields fields = BGCurveBaseMath.Fields.Position;

		// Token: 0x04000D64 RID: 3428
		[SerializeField]
		[Tooltip("Math type to use.\r\nBase - uses uniformely split sections;\r\n Adaptive - uses non-uniformely split sections, based on the curvature. Expiremental.")]
		private BGCcMath.MathTypeEnum mathType;

		// Token: 0x04000D65 RID: 3429
		[SerializeField]
		[Tooltip("The number of equal parts for each section, used by Base math.")]
		[Range(1f, 100f)]
		private int sectionParts = 30;

		// Token: 0x04000D66 RID: 3430
		[SerializeField]
		[Tooltip("Use only 2 points for straight lines. Tangents may be calculated slightly different. Used by Base math.")]
		private bool optimizeStraightLines;

		// Token: 0x04000D67 RID: 3431
		[SerializeField]
		[Tooltip("Tolerance, used by Adaptive Math. The bigger the tolerance- the lesser splits. Note: The final tolerance used by Math is based on this value but different.")]
		[Range(0.1f, 0.999975f)]
		private float tolerance = 0.2f;

		// Token: 0x04000D68 RID: 3432
		[SerializeField]
		[Tooltip("Points position will be used for tangent calculation. This can gain some performance")]
		private bool usePositionToCalculateTangents;

		// Token: 0x04000D69 RID: 3433
		[SerializeField]
		[Tooltip("Updating math takes some resources. You can fine-tune in which cases math is updated.\r\n1) Always- always update\r\n2) AabbVisible- update only if AABB (Axis Aligned Bounding Box) around points and controls is visible\r\n3) RendererVisible- update only if some renderer is visible")]
		private BGCcMath.UpdateModeEnum updateMode;

		// Token: 0x04000D6A RID: 3434
		[SerializeField]
		[Tooltip("Renderer to check for updating math. Math will be updated only if renderer is visible")]
		private Renderer rendererForUpdateCheck;

		// Token: 0x04000D6B RID: 3435
		[SerializeField]
		[Tooltip("Event is fired, then math is recalculated")]
		private BGCcMath.MathChangedEvent mathChangedEvent = new BGCcMath.MathChangedEvent();

		// Token: 0x04000D6C RID: 3436
		private BGCurveBaseMath math;

		// Token: 0x04000D6D RID: 3437
		private BGCcMath.VisibilityCheck visibilityCheck;

		// Token: 0x04000D6E RID: 3438
		private MeshFilter meshFilter;

		// Token: 0x04000D6F RID: 3439
		private readonly Vector3[] vertices = new Vector3[2];

		// Token: 0x020001B9 RID: 441
		public enum MathTypeEnum
		{
			// Token: 0x04000D71 RID: 3441
			Base,
			// Token: 0x04000D72 RID: 3442
			Adaptive
		}

		// Token: 0x020001BA RID: 442
		public enum UpdateModeEnum
		{
			// Token: 0x04000D74 RID: 3444
			Always,
			// Token: 0x04000D75 RID: 3445
			AabbVisible,
			// Token: 0x04000D76 RID: 3446
			RendererVisible
		}

		// Token: 0x020001BB RID: 443
		private sealed class VisibilityCheck : MonoBehaviour
		{
			// Token: 0x14000011 RID: 17
			// (add) Token: 0x06000FE7 RID: 4071 RVA: 0x000DDFD0 File Offset: 0x000DC1D0
			// (remove) Token: 0x06000FE8 RID: 4072 RVA: 0x000DE008 File Offset: 0x000DC208
			public event EventHandler BecameVisible;

			// Token: 0x17000450 RID: 1104
			// (get) Token: 0x06000FE9 RID: 4073 RVA: 0x0004D590 File Offset: 0x0004B790
			// (set) Token: 0x06000FEA RID: 4074 RVA: 0x0004D598 File Offset: 0x0004B798
			public bool Visible { get; private set; }

			// Token: 0x06000FEB RID: 4075 RVA: 0x0004D5A1 File Offset: 0x0004B7A1
			private void OnBecameVisible()
			{
				this.Visible = true;
				if (this.BecameVisible != null)
				{
					this.BecameVisible(this, null);
				}
			}

			// Token: 0x06000FEC RID: 4076 RVA: 0x0004D5BF File Offset: 0x0004B7BF
			private void OnBecameInvisible()
			{
				this.Visible = false;
			}
		}

		// Token: 0x020001BC RID: 444
		[Serializable]
		public class MathChangedEvent : UnityEvent
		{
		}
	}
}
