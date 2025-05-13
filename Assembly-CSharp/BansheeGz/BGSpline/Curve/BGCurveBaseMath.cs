using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGSpline.Curve
{
	// Token: 0x02000187 RID: 391
	public class BGCurveBaseMath : BGCurveMathI, IDisposable
	{
		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06000D3C RID: 3388 RVA: 0x000D4D2C File Offset: 0x000D2F2C
		// (remove) Token: 0x06000D3D RID: 3389 RVA: 0x000D4D64 File Offset: 0x000D2F64
		public event EventHandler ChangeRequested;

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06000D3E RID: 3390 RVA: 0x000D4D9C File Offset: 0x000D2F9C
		// (remove) Token: 0x06000D3F RID: 3391 RVA: 0x000D4DD4 File Offset: 0x000D2FD4
		public event EventHandler Changed;

		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x06000D40 RID: 3392 RVA: 0x0004B893 File Offset: 0x00049A93
		// (set) Token: 0x06000D41 RID: 3393 RVA: 0x0004B89B File Offset: 0x00049A9B
		public bool SuppressWarning { get; set; }

		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x06000D42 RID: 3394 RVA: 0x0004B8A4 File Offset: 0x00049AA4
		public BGCurve Curve
		{
			get
			{
				return this.curve;
			}
		}

		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x06000D43 RID: 3395 RVA: 0x0004B8AC File Offset: 0x00049AAC
		public List<BGCurveBaseMath.SectionInfo> SectionInfos
		{
			get
			{
				return this.cachedSectionInfos;
			}
		}

		// Token: 0x170003A3 RID: 931
		public BGCurveBaseMath.SectionInfo this[int i]
		{
			get
			{
				return this.cachedSectionInfos[i];
			}
		}

		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x06000D45 RID: 3397 RVA: 0x0004B8C2 File Offset: 0x00049AC2
		public int SectionsCount
		{
			get
			{
				return this.cachedSectionInfos.Count;
			}
		}

		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x06000D46 RID: 3398 RVA: 0x0004B8CF File Offset: 0x00049ACF
		public BGCurveBaseMath.Config Configuration
		{
			get
			{
				return this.config;
			}
		}

		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x06000D47 RID: 3399 RVA: 0x0004B8D7 File Offset: 0x00049AD7
		protected bool NeedTangentFormula
		{
			get
			{
				return !this.config.UsePointPositionsToCalcTangents && this.cacheTangent;
			}
		}

		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x06000D48 RID: 3400 RVA: 0x000D4E0C File Offset: 0x000D300C
		public int PointsCount
		{
			get
			{
				if (this.SectionsCount == 0)
				{
					return 0;
				}
				int num = 0;
				int count = this.cachedSectionInfos.Count;
				for (int i = 0; i < count; i++)
				{
					num += this.cachedSectionInfos[i].PointsCount;
				}
				return num;
			}
		}

		// Token: 0x06000D49 RID: 3401 RVA: 0x0004B8EE File Offset: 0x00049AEE
		public BGCurveBaseMath(BGCurve curve) : this(curve, new BGCurveBaseMath.Config(BGCurveBaseMath.Fields.Position))
		{
		}

		// Token: 0x06000D4A RID: 3402 RVA: 0x000D4E54 File Offset: 0x000D3054
		public BGCurveBaseMath(BGCurve curve, BGCurveBaseMath.Config config)
		{
			this.curve = curve;
			curve.Changed += this.CurveChanged;
			this.Init(config ?? new BGCurveBaseMath.Config(BGCurveBaseMath.Fields.Position));
		}

		// Token: 0x06000D4B RID: 3403 RVA: 0x0004B8FD File Offset: 0x00049AFD
		[Obsolete("Use another constructors")]
		public BGCurveBaseMath(BGCurve curve, bool traceChanges, int parts = 30, bool usePointPositionsToCalcTangents = false) : this(curve, new BGCurveBaseMath.Config(BGCurveBaseMath.Fields.Position)
		{
			Parts = parts,
			UsePointPositionsToCalcTangents = usePointPositionsToCalcTangents
		})
		{
		}

		// Token: 0x06000D4C RID: 3404 RVA: 0x000D4EBC File Offset: 0x000D30BC
		public virtual void Init(BGCurveBaseMath.Config config)
		{
			if (this.config != null)
			{
				this.ignoreSectionChangedCheck = (this.config.Fields == BGCurveBaseMath.Fields.Position && config.Fields == BGCurveBaseMath.Fields.PositionAndTangent);
				this.config.Update -= this.ConfigOnUpdate;
			}
			else
			{
				this.ignoreSectionChangedCheck = false;
			}
			this.config = config;
			config.Parts = Mathf.Clamp(config.Parts, 1, 1000);
			this.config.Update += this.ConfigOnUpdate;
			this.createdAtFrame = Time.frameCount;
			this.cachePosition = BGCurveBaseMath.Field.Position.In(config.Fields.Val());
			this.cacheTangent = BGCurveBaseMath.Field.Tangent.In(config.Fields.Val());
			if (!this.cachePosition && !this.cacheTangent)
			{
				throw new UnityException("No fields were chosen. Create math like this: new BGCurveBaseMath(curve, new BGCurveBaseMath.Config(BGCurveBaseMath.Fields.Position))");
			}
			this.AfterInit(config);
			this.Recalculate(true);
		}

		// Token: 0x06000D4D RID: 3405 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void AfterInit(BGCurveBaseMath.Config config)
		{
		}

		// Token: 0x06000D4E RID: 3406 RVA: 0x000D4FA8 File Offset: 0x000D31A8
		public virtual Vector3 CalcPositionByT(BGCurvePoint from, BGCurvePoint to, float t, bool useLocal = false)
		{
			t = Mathf.Clamp01(t);
			Vector3 vector = useLocal ? from.PositionLocal : from.PositionWorld;
			Vector3 vector2 = useLocal ? to.PositionLocal : to.PositionWorld;
			Vector3 result;
			if (from.ControlType == BGCurvePoint.ControlTypeEnum.Absent && to.ControlType == BGCurvePoint.ControlTypeEnum.Absent)
			{
				result = vector + (vector2 - vector) * t;
			}
			else
			{
				Vector3 vector3 = useLocal ? (from.ControlSecondLocal + vector) : from.ControlSecondWorld;
				Vector3 vector4 = useLocal ? (to.ControlFirstLocal + vector2) : to.ControlFirstWorld;
				result = ((from.ControlType != BGCurvePoint.ControlTypeEnum.Absent && to.ControlType != BGCurvePoint.ControlTypeEnum.Absent) ? BGCurveFormulas.BezierCubic(t, vector, vector3, vector4, vector2) : BGCurveFormulas.BezierQuadratic(t, vector, (from.ControlType == BGCurvePoint.ControlTypeEnum.Absent) ? vector4 : vector3, vector2));
			}
			return result;
		}

		// Token: 0x06000D4F RID: 3407 RVA: 0x000D5070 File Offset: 0x000D3270
		public virtual Vector3 CalcTangentByT(BGCurvePoint from, BGCurvePoint to, float t, bool useLocal = false)
		{
			if (this.Curve.PointsCount < 2)
			{
				return Vector3.zero;
			}
			t = Mathf.Clamp01(t);
			Vector3 vector = useLocal ? from.PositionLocal : from.PositionWorld;
			Vector3 vector2 = useLocal ? to.PositionLocal : to.PositionWorld;
			Vector3 vector3;
			if (from.ControlType == BGCurvePoint.ControlTypeEnum.Absent && to.ControlType == BGCurvePoint.ControlTypeEnum.Absent)
			{
				vector3 = vector2 - vector;
			}
			else
			{
				Vector3 vector4 = useLocal ? (from.ControlSecondLocal + vector) : from.ControlSecondWorld;
				Vector3 vector5 = useLocal ? (to.ControlFirstLocal + vector2) : to.ControlFirstWorld;
				vector3 = ((from.ControlType != BGCurvePoint.ControlTypeEnum.Absent && to.ControlType != BGCurvePoint.ControlTypeEnum.Absent) ? BGCurveFormulas.BezierCubicDerivative(t, vector, vector4, vector5, vector2) : BGCurveFormulas.BezierQuadraticDerivative(t, vector, (from.ControlType == BGCurvePoint.ControlTypeEnum.Absent) ? vector5 : vector4, vector2));
			}
			return vector3.normalized;
		}

		// Token: 0x06000D50 RID: 3408 RVA: 0x0004B91B File Offset: 0x00049B1B
		public virtual Vector3 CalcByDistanceRatio(float distanceRatio, out Vector3 tangent, bool useLocal = false)
		{
			return this.CalcByDistance(this.cachedLength * distanceRatio, out tangent, useLocal);
		}

		// Token: 0x06000D51 RID: 3409 RVA: 0x000D5148 File Offset: 0x000D3348
		public virtual Vector3 CalcByDistance(float distance, out Vector3 tangent, bool useLocal = false)
		{
			Vector3 vector;
			this.BinarySearchByDistance(distance, out vector, out tangent, true, true);
			if (useLocal)
			{
				vector = this.curve.transform.InverseTransformPoint(vector);
				tangent = this.curve.transform.InverseTransformDirection(tangent);
			}
			return vector;
		}

		// Token: 0x06000D52 RID: 3410 RVA: 0x0004B92D File Offset: 0x00049B2D
		public virtual Vector3 CalcByDistanceRatio(BGCurveBaseMath.Field field, float distanceRatio, bool useLocal = false)
		{
			return this.CalcByDistance(field, this.cachedLength * distanceRatio, useLocal);
		}

		// Token: 0x06000D53 RID: 3411 RVA: 0x000D5194 File Offset: 0x000D3394
		public virtual Vector3 CalcByDistance(BGCurveBaseMath.Field field, float distance, bool useLocal = false)
		{
			bool flag = field == BGCurveBaseMath.Field.Position;
			Vector3 vector;
			Vector3 vector2;
			this.BinarySearchByDistance(distance, out vector, out vector2, flag, !flag);
			if (useLocal)
			{
				if (field != BGCurveBaseMath.Field.Position)
				{
					if (field == BGCurveBaseMath.Field.Tangent)
					{
						vector2 = this.curve.transform.InverseTransformDirection(vector2);
					}
				}
				else
				{
					vector = this.curve.transform.InverseTransformPoint(vector);
				}
			}
			if (!flag)
			{
				return vector2;
			}
			return vector;
		}

		// Token: 0x06000D54 RID: 3412 RVA: 0x0004B93F File Offset: 0x00049B3F
		public virtual Vector3 CalcPositionAndTangentByDistanceRatio(float distanceRatio, out Vector3 tangent, bool useLocal = false)
		{
			return this.CalcByDistanceRatio(distanceRatio, out tangent, useLocal);
		}

		// Token: 0x06000D55 RID: 3413 RVA: 0x0004B94A File Offset: 0x00049B4A
		public virtual Vector3 CalcPositionAndTangentByDistance(float distance, out Vector3 tangent, bool useLocal = false)
		{
			return this.CalcByDistance(distance, out tangent, useLocal);
		}

		// Token: 0x06000D56 RID: 3414 RVA: 0x0004B955 File Offset: 0x00049B55
		public virtual Vector3 CalcPositionByDistanceRatio(float distanceRatio, bool useLocal = false)
		{
			return this.CalcByDistanceRatio(BGCurveBaseMath.Field.Position, distanceRatio, useLocal);
		}

		// Token: 0x06000D57 RID: 3415 RVA: 0x0004B960 File Offset: 0x00049B60
		public virtual Vector3 CalcPositionByDistance(float distance, bool useLocal = false)
		{
			return this.CalcByDistance(BGCurveBaseMath.Field.Position, distance, useLocal);
		}

		// Token: 0x06000D58 RID: 3416 RVA: 0x0004B96B File Offset: 0x00049B6B
		public virtual Vector3 CalcTangentByDistanceRatio(float distanceRatio, bool useLocal = false)
		{
			return this.CalcByDistanceRatio(BGCurveBaseMath.Field.Tangent, distanceRatio, useLocal);
		}

		// Token: 0x06000D59 RID: 3417 RVA: 0x0004B976 File Offset: 0x00049B76
		public virtual Vector3 CalcTangentByDistance(float distance, bool useLocal = false)
		{
			return this.CalcByDistance(BGCurveBaseMath.Field.Tangent, distance, useLocal);
		}

		// Token: 0x06000D5A RID: 3418 RVA: 0x0004B981 File Offset: 0x00049B81
		public int CalcSectionIndexByDistance(float distance)
		{
			return this.FindSectionIndexByDistance(this.ClampDistance(distance));
		}

		// Token: 0x06000D5B RID: 3419 RVA: 0x0004B990 File Offset: 0x00049B90
		public int CalcSectionIndexByDistanceRatio(float ratio)
		{
			return this.FindSectionIndexByDistance(this.DistanceByRatio(ratio));
		}

		// Token: 0x06000D5C RID: 3420 RVA: 0x000D51F0 File Offset: 0x000D33F0
		public Vector3 CalcPositionByClosestPoint(Vector3 point, bool skipSectionsOptimization = false, bool skipPointsOptimization = false)
		{
			if (this.closestPointCalculator == null)
			{
				this.closestPointCalculator = new BGCurveCalculatorClosestPoint(this);
			}
			float num;
			Vector3 vector;
			return this.closestPointCalculator.CalcPositionByClosestPoint(point, out num, out vector, skipSectionsOptimization, skipPointsOptimization);
		}

		// Token: 0x06000D5D RID: 3421 RVA: 0x000D5224 File Offset: 0x000D3424
		public Vector3 CalcPositionByClosestPoint(Vector3 point, out float distance, bool skipSectionsOptimization = false, bool skipPointsOptimization = false)
		{
			if (this.closestPointCalculator == null)
			{
				this.closestPointCalculator = new BGCurveCalculatorClosestPoint(this);
			}
			Vector3 vector;
			return this.closestPointCalculator.CalcPositionByClosestPoint(point, out distance, out vector, skipSectionsOptimization, skipPointsOptimization);
		}

		// Token: 0x06000D5E RID: 3422 RVA: 0x0004B99F File Offset: 0x00049B9F
		public Vector3 CalcPositionByClosestPoint(Vector3 point, out float distance, out Vector3 tangent, bool skipSectionsOptimization = false, bool skipPointsOptimization = false)
		{
			if (this.closestPointCalculator == null)
			{
				this.closestPointCalculator = new BGCurveCalculatorClosestPoint(this);
			}
			return this.closestPointCalculator.CalcPositionByClosestPoint(point, out distance, out tangent, skipSectionsOptimization, skipPointsOptimization);
		}

		// Token: 0x06000D5F RID: 3423 RVA: 0x0004B9C7 File Offset: 0x00049BC7
		public virtual float GetDistance(int pointIndex = -1)
		{
			if (pointIndex < 0)
			{
				return this.cachedLength;
			}
			if (pointIndex == 0)
			{
				return 0f;
			}
			return this.cachedSectionInfos[pointIndex - 1].DistanceFromEndToOrigin;
		}

		// Token: 0x06000D60 RID: 3424 RVA: 0x000D5258 File Offset: 0x000D3458
		public Vector3 GetPosition(int pointIndex)
		{
			int count = this.cachedSectionInfos.Count;
			if (count == 0 || count <= pointIndex)
			{
				return this.curve[pointIndex].PositionWorld;
			}
			if (pointIndex >= count)
			{
				return this.cachedSectionInfos[pointIndex - 1].OriginalTo;
			}
			return this.cachedSectionInfos[pointIndex].OriginalFrom;
		}

		// Token: 0x06000D61 RID: 3425 RVA: 0x000D52B4 File Offset: 0x000D34B4
		public Vector3 GetControlFirst(int pointIndex)
		{
			int count = this.cachedSectionInfos.Count;
			if (count == 0)
			{
				return this.curve[pointIndex].ControlFirstWorld;
			}
			if (pointIndex != 0)
			{
				return this.cachedSectionInfos[pointIndex - 1].OriginalToControl;
			}
			if (!this.curve.Closed)
			{
				return this.curve[0].ControlFirstWorld;
			}
			return this.cachedSectionInfos[count - 1].OriginalToControl;
		}

		// Token: 0x06000D62 RID: 3426 RVA: 0x000D532C File Offset: 0x000D352C
		public Vector3 GetControlSecond(int pointIndex)
		{
			int count = this.cachedSectionInfos.Count;
			if (count == 0)
			{
				return this.curve[pointIndex].ControlSecondWorld;
			}
			if (pointIndex != count)
			{
				return this.cachedSectionInfos[pointIndex].OriginalFromControl;
			}
			if (!this.curve.Closed)
			{
				return this.curve[pointIndex].ControlSecondWorld;
			}
			return this.cachedSectionInfos[count - 1].OriginalFromControl;
		}

		// Token: 0x06000D63 RID: 3427 RVA: 0x0004B9F0 File Offset: 0x00049BF0
		public virtual bool IsCalculated(BGCurveBaseMath.Field field)
		{
			return (field & (BGCurveBaseMath.Field)this.config.Fields) > (BGCurveBaseMath.Field)0;
		}

		// Token: 0x06000D64 RID: 3428 RVA: 0x000D53A4 File Offset: 0x000D35A4
		public virtual void Dispose()
		{
			this.curve.Changed -= this.CurveChanged;
			this.config.Update -= this.ConfigOnUpdate;
			this.cachedSectionInfos.Clear();
			this.poolSectionInfos.Clear();
		}

		// Token: 0x06000D65 RID: 3429 RVA: 0x000D53F8 File Offset: 0x000D35F8
		public Bounds GetBoundingBox(int sectionIndex, BGCurveBaseMath.SectionInfo section)
		{
			bool flag = section.OriginalFromControlType == BGCurvePoint.ControlTypeEnum.Absent;
			bool flag2 = section.OriginalToControlType == BGCurvePoint.ControlTypeEnum.Absent;
			Vector3 originalFrom = section.OriginalFrom;
			Vector3 originalTo = section.OriginalTo;
			Vector3 originalToControl = section.OriginalToControl;
			float num = (originalFrom.x > originalTo.x) ? originalTo.x : originalFrom.x;
			float num2 = (originalFrom.y > originalTo.y) ? originalTo.y : originalFrom.y;
			float num3 = (originalFrom.z > originalTo.z) ? originalTo.z : originalFrom.z;
			float num4 = (originalFrom.x < originalTo.x) ? originalTo.x : originalFrom.x;
			float num5 = (originalFrom.y < originalTo.y) ? originalTo.y : originalFrom.y;
			float num6 = (originalFrom.z < originalTo.z) ? originalTo.z : originalFrom.z;
			float num7;
			float num8;
			float num9;
			float num10;
			float num11;
			float num12;
			if (flag)
			{
				if (flag2)
				{
					num7 = num;
					num8 = num2;
					num9 = num3;
					num10 = num4;
					num11 = num5;
					num12 = num6;
				}
				else
				{
					num7 = ((num > originalToControl.x) ? originalToControl.x : num);
					num8 = ((num2 > originalToControl.y) ? originalToControl.y : num2);
					num9 = ((num3 > originalToControl.z) ? originalToControl.z : num3);
					num10 = ((num4 < originalToControl.x) ? originalToControl.x : num4);
					num11 = ((num5 < originalToControl.y) ? originalToControl.y : num5);
					num12 = ((num6 < originalToControl.z) ? originalToControl.z : num6);
				}
			}
			else
			{
				Vector3 originalFromControl = section.OriginalFromControl;
				if (flag2)
				{
					num7 = ((num > originalFromControl.x) ? originalFromControl.x : num);
					num8 = ((num2 > originalFromControl.y) ? originalFromControl.y : num2);
					num9 = ((num3 > originalFromControl.z) ? originalFromControl.z : num3);
					num10 = ((num4 < originalFromControl.x) ? originalFromControl.x : num4);
					num11 = ((num5 < originalFromControl.y) ? originalFromControl.y : num5);
					num12 = ((num6 < originalFromControl.z) ? originalFromControl.z : num6);
				}
				else
				{
					float num13 = (num > originalToControl.x) ? originalToControl.x : num;
					float num14 = (num2 > originalToControl.y) ? originalToControl.y : num2;
					float num15 = (num3 > originalToControl.z) ? originalToControl.z : num3;
					float num16 = (num4 < originalToControl.x) ? originalToControl.x : num4;
					float num17 = (num5 < originalToControl.y) ? originalToControl.y : num5;
					float num18 = (num6 < originalToControl.z) ? originalToControl.z : num6;
					num7 = ((num13 > originalFromControl.x) ? originalFromControl.x : num13);
					num8 = ((num14 > originalFromControl.y) ? originalFromControl.y : num14);
					num9 = ((num15 > originalFromControl.z) ? originalFromControl.z : num15);
					num10 = ((num16 < originalFromControl.x) ? originalFromControl.x : num16);
					num11 = ((num17 < originalFromControl.y) ? originalFromControl.y : num17);
					num12 = ((num18 < originalFromControl.z) ? originalFromControl.z : num18);
				}
			}
			float num19 = num10 - num7;
			float num20 = num11 - num8;
			float num21 = num12 - num9;
			Vector3 vector = new Vector3(num19 * 0.5f, num20 * 0.5f, num21 * 0.5f);
			return new Bounds
			{
				extents = vector,
				center = new Vector3(num7 + vector.x, num8 + vector.y, num9 + vector.z)
			};
		}

		// Token: 0x06000D66 RID: 3430 RVA: 0x000D57B8 File Offset: 0x000D39B8
		public virtual void Recalculate(bool force = false)
		{
			if (this.ChangeRequested != null)
			{
				this.ChangeRequested(this, null);
			}
			force = (force || this.Curve.SnapType == BGCurve.SnapTypeEnum.Curve);
			if (!force && this.config.ShouldUpdate != null && !this.config.ShouldUpdate())
			{
				return;
			}
			int count = this.cachedSectionInfos.Count;
			if (this.curve.PointsCount < 2)
			{
				this.cachedLength = 0f;
				if (count > 0)
				{
					this.cachedSectionInfos.Clear();
				}
				if (this.Changed != null)
				{
					this.Changed(this, null);
				}
				return;
			}
			int frameCount = Time.frameCount;
			if (this.recalculatedAtFrame == frameCount && frameCount != this.createdAtFrame)
			{
				this.Warning("We noticed you are updating math more than once per frame. This is not optimal. If you use curve.ImmediateChangeEvents by some reason, try to use curve.Transaction to wrap all the changes to one single event.", true, null);
			}
			this.recalculatedAtFrame = frameCount;
			int pointsCount = this.curve.PointsCount;
			int num = this.curve.Closed ? pointsCount : (pointsCount - 1);
			this.h = 1.0 / (double)this.config.Parts;
			this.h2 = this.h * this.h;
			this.h3 = this.h2 * this.h;
			if (count != num)
			{
				if (count < num)
				{
					int num2 = num - count;
					int count2 = this.poolSectionInfos.Count;
					int num3 = num2;
					int num4 = count2 - 1;
					while (num4 >= 0 && num2 > 0)
					{
						this.cachedSectionInfos.Add(this.poolSectionInfos[num4]);
						num4--;
						num2--;
					}
					int num5 = num3 - num2;
					if (num5 != 0)
					{
						this.poolSectionInfos.RemoveRange(this.poolSectionInfos.Count - num5, num5);
					}
					if (num2 > 0)
					{
						for (int i = 0; i < num2; i++)
						{
							this.cachedSectionInfos.Add(new BGCurveBaseMath.SectionInfo());
						}
					}
				}
				else
				{
					int num6 = count - num;
					for (int j = num; j < count; j++)
					{
						this.poolSectionInfos.Add(this.cachedSectionInfos[j]);
					}
					this.cachedSectionInfos.RemoveRange(count - num6, num6);
				}
			}
			for (int k = 0; k < pointsCount - 1; k++)
			{
				this.CalculateSection(k, this.cachedSectionInfos[k], (k == 0) ? null : this.cachedSectionInfos[k - 1], this.curve[k], this.curve[k + 1]);
			}
			BGCurveBaseMath.SectionInfo sectionInfo = this.cachedSectionInfos[num - 1];
			if (this.curve.Closed)
			{
				this.CalculateSection(num - 1, sectionInfo, this.cachedSectionInfos[num - 2], this.curve[pointsCount - 1], this.curve[0]);
				if (this.cacheTangent)
				{
					this.AdjustBoundaryPointsTangents(this.cachedSectionInfos[0], sectionInfo);
				}
			}
			this.cachedLength = sectionInfo.DistanceFromEndToOrigin;
			if (this.Changed != null)
			{
				this.Changed(this, null);
			}
		}

		// Token: 0x06000D67 RID: 3431 RVA: 0x0004BA02 File Offset: 0x00049C02
		protected virtual void Warning(string message, bool condition = true, Action callback = null)
		{
			if (!condition || !Application.isPlaying)
			{
				return;
			}
			if (!this.SuppressWarning)
			{
				Debug.Log("BGCurve[BGCurveBaseMath] Warning! " + message + ". You can suppress all warnings by using BGCurveBaseMath.SuppressWarning=true;");
			}
			if (callback != null)
			{
				callback();
			}
		}

		// Token: 0x06000D68 RID: 3432 RVA: 0x000D5AB4 File Offset: 0x000D3CB4
		protected virtual void CalculateSection(int index, BGCurveBaseMath.SectionInfo section, BGCurveBaseMath.SectionInfo prevSection, BGCurvePointI from, BGCurvePointI to)
		{
			if (section == null)
			{
				section = new BGCurveBaseMath.SectionInfo();
			}
			section.DistanceFromStartToOrigin = ((prevSection == null) ? 0f : prevSection.DistanceFromEndToOrigin);
			bool flag = this.config.OptimizeStraightLines && from.ControlType == BGCurvePoint.ControlTypeEnum.Absent && to.ControlType == BGCurvePoint.ControlTypeEnum.Absent;
			int pointsCount = flag ? 2 : (this.config.Parts + 1);
			if (this.Reset(section, from, to, pointsCount) || this.Curve.SnapType == BGCurve.SnapTypeEnum.Curve)
			{
				if (flag)
				{
					this.Resize(section.points, 2);
					BGCurveBaseMath.SectionPointInfo sectionPointInfo = section.points[0];
					BGCurveBaseMath.SectionPointInfo sectionPointInfo2 = section.points[1];
					sectionPointInfo.Position = section.OriginalFrom;
					sectionPointInfo2.Position = section.OriginalTo;
					sectionPointInfo2.DistanceToSectionStart = Vector3.Distance(section.OriginalFrom, section.OriginalTo);
					if (this.cacheTangent)
					{
						sectionPointInfo.Tangent = (sectionPointInfo2.Tangent = (sectionPointInfo2.Position - sectionPointInfo.Position).normalized);
					}
				}
				else
				{
					this.CalculateSplitSection(section, from, to);
				}
				if (this.cacheTangent)
				{
					section.OriginalFirstPointTangent = section[0].Tangent;
					section.OriginalLastPointTangent = section[section.PointsCount - 1].Tangent;
				}
			}
			if (this.cacheTangent && prevSection != null)
			{
				this.AdjustBoundaryPointsTangents(section, prevSection);
			}
			section.DistanceFromEndToOrigin = section.DistanceFromStartToOrigin + section[section.PointsCount - 1].DistanceToSectionStart;
		}

		// Token: 0x06000D69 RID: 3433 RVA: 0x000D5C3C File Offset: 0x000D3E3C
		private void AdjustBoundaryPointsTangents(BGCurveBaseMath.SectionInfo section, BGCurveBaseMath.SectionInfo prevSection)
		{
			if (!this.IsUseDistanceToAdjustTangents(section, prevSection))
			{
				section[0].Tangent = (prevSection[prevSection.PointsCount - 1].Tangent = Vector3.Normalize(new Vector3(section.OriginalFirstPointTangent.x + prevSection.OriginalLastPointTangent.x, section.OriginalFirstPointTangent.y + prevSection.OriginalLastPointTangent.y, section.OriginalFirstPointTangent.z + prevSection.OriginalLastPointTangent.z)));
				return;
			}
			float num = Vector3.SqrMagnitude(section[0].Position - section[1].Position);
			float num2 = Vector3.SqrMagnitude(prevSection[prevSection.PointsCount - 1].Position - prevSection[prevSection.PointsCount - 2].Position);
			float num3 = num + num2;
			if (Math.Abs(num3) < 1E-05f)
			{
				return;
			}
			float num4 = num / num3;
			float num5 = 1f - num4;
			section[0].Tangent = (prevSection[prevSection.PointsCount - 1].Tangent = Vector3.Normalize(new Vector3(section.OriginalFirstPointTangent.x * num4 + prevSection.OriginalLastPointTangent.x * num5, section.OriginalFirstPointTangent.y * num4 + prevSection.OriginalLastPointTangent.y * num5, section.OriginalFirstPointTangent.z * num4 + prevSection.OriginalLastPointTangent.z * num5)));
		}

		// Token: 0x06000D6A RID: 3434 RVA: 0x0004BA35 File Offset: 0x00049C35
		protected virtual bool IsUseDistanceToAdjustTangents(BGCurveBaseMath.SectionInfo section, BGCurveBaseMath.SectionInfo prevSection)
		{
			return this.config.OptimizeStraightLines && section.OriginalFromControlType == BGCurvePoint.ControlTypeEnum.Absent && (section.OriginalToControlType == BGCurvePoint.ControlTypeEnum.Absent || prevSection.OriginalFromControlType == BGCurvePoint.ControlTypeEnum.Absent);
		}

		// Token: 0x06000D6B RID: 3435 RVA: 0x0004BA61 File Offset: 0x00049C61
		protected virtual bool Reset(BGCurveBaseMath.SectionInfo section, BGCurvePointI from, BGCurvePointI to, int pointsCount)
		{
			return section.Reset(from, to, pointsCount, this.ignoreSectionChangedCheck);
		}

		// Token: 0x06000D6C RID: 3436 RVA: 0x000D5DC0 File Offset: 0x000D3FC0
		protected virtual void CalculateSplitSection(BGCurveBaseMath.SectionInfo section, BGCurvePointI from, BGCurvePointI to)
		{
			int parts = this.config.Parts;
			this.Resize(section.points, parts + 1);
			List<BGCurveBaseMath.SectionPointInfo> points = section.points;
			Vector3 originalFrom = section.OriginalFrom;
			Vector3 originalTo = section.OriginalTo;
			Vector3 vector = section.OriginalFromControl;
			Vector3 originalToControl = section.OriginalToControl;
			bool flag = from.ControlType == BGCurvePoint.ControlTypeEnum.Absent;
			bool flag2 = to.ControlType == BGCurvePoint.ControlTypeEnum.Absent;
			bool flag3 = flag && flag2;
			bool flag4 = !flag && !flag2;
			if (!flag3 && !flag4 && flag)
			{
				vector = originalToControl;
			}
			bool flag5 = this.curve.SnapType == BGCurve.SnapTypeEnum.Curve;
			BGCurveBaseMath.SectionPointInfo sectionPointInfo;
			if ((sectionPointInfo = points[0]) == null)
			{
				sectionPointInfo = (section.points[0] = new BGCurveBaseMath.SectionPointInfo());
			}
			BGCurveBaseMath.SectionPointInfo sectionPointInfo2 = sectionPointInfo;
			sectionPointInfo2.Position = originalFrom;
			BGCurveBaseMath.SectionPointInfo sectionPointInfo3;
			if ((sectionPointInfo3 = points[parts]) == null)
			{
				sectionPointInfo3 = (section.points[parts] = new BGCurveBaseMath.SectionPointInfo());
			}
			BGCurveBaseMath.SectionPointInfo sectionPointInfo4 = sectionPointInfo3;
			sectionPointInfo4.Position = originalTo;
			if (flag3)
			{
				double num = (double)originalFrom.x;
				double num2 = (double)originalFrom.y;
				double num3 = (double)originalFrom.z;
				double num4 = ((double)originalTo.x - (double)originalFrom.x) / (double)parts;
				double num5 = ((double)originalTo.y - (double)originalFrom.y) / (double)parts;
				double num6 = ((double)originalTo.z - (double)originalFrom.z) / (double)parts;
				Vector3 tangent = Vector3.zero;
				if (this.cacheTangent)
				{
					tangent = (originalTo - originalFrom).normalized;
				}
				sectionPointInfo4.DistanceToSectionStart = Vector3.Distance(originalTo, originalFrom);
				float num7 = sectionPointInfo4.DistanceToSectionStart / (float)parts;
				for (int i = 1; i < parts; i++)
				{
					BGCurveBaseMath.SectionPointInfo sectionPointInfo5 = points[i];
					num += num4;
					num2 += num5;
					num3 += num6;
					Vector3 vector2 = new Vector3((float)num, (float)num2, (float)num3);
					if (flag5)
					{
						this.curve.ApplySnapping(ref vector2);
					}
					sectionPointInfo5.Position = vector2;
					if (this.cacheTangent)
					{
						if (this.config.UsePointPositionsToCalcTangents)
						{
							BGCurveBaseMath.SectionPointInfo sectionPointInfo6 = section[i - 1];
							Vector3 position = sectionPointInfo6.Position;
							Vector3 vector3 = new Vector3(vector2.x - position.x, vector2.y - position.y, vector2.z - position.z);
							float num8 = (float)Math.Sqrt((double)vector3.x * (double)vector3.x + (double)vector3.y * (double)vector3.y + (double)vector3.z * (double)vector3.z);
							vector3 = (((double)num8 > 9.99999974737875E-06) ? new Vector3(vector3.x / num8, vector3.y / num8, vector3.z / num8) : Vector3.zero);
							sectionPointInfo6.Tangent = (sectionPointInfo5.Tangent = vector3);
						}
						else
						{
							sectionPointInfo5.Tangent = tangent;
						}
					}
					sectionPointInfo5.DistanceToSectionStart = num7 * (float)i;
				}
				if (this.cacheTangent)
				{
					sectionPointInfo2.Tangent = (sectionPointInfo4.Tangent = tangent);
				}
			}
			else
			{
				double num9 = 0.0;
				double num10 = 0.0;
				double num11 = 0.0;
				double num12 = 0.0;
				double num13 = 0.0;
				double num14 = 0.0;
				if (flag4)
				{
					double num15 = 3.0 * ((double)vector.x - (double)originalFrom.x);
					double num16 = 3.0 * ((double)vector.y - (double)originalFrom.y);
					double num17 = 3.0 * ((double)vector.z - (double)originalFrom.z);
					double num18 = 3.0 * ((double)originalToControl.x - (double)vector.x) - num15;
					double num19 = 3.0 * ((double)originalToControl.y - (double)vector.y) - num16;
					double num20 = 3.0 * ((double)originalToControl.z - (double)vector.z) - num17;
					double num21 = (double)originalTo.x - (double)originalFrom.x - num15 - num18;
					double num22 = (double)originalTo.y - (double)originalFrom.y - num16 - num19;
					double num23 = (double)originalTo.z - (double)originalFrom.z - num17 - num20;
					double num24 = (double)originalFrom.x;
					double num25 = (double)originalFrom.y;
					double num26 = (double)originalFrom.z;
					double num27 = num21 * this.h3;
					double num28 = 6.0 * num27;
					double num29 = num22 * this.h3;
					double num30 = 6.0 * num29;
					double num31 = num23 * this.h3;
					double num32 = 6.0 * num31;
					double num33 = num18 * this.h2;
					double num34 = num19 * this.h2;
					double num35 = num20 * this.h2;
					double num36 = num27 + num33 + num15 * this.h;
					double num37 = num29 + num34 + num16 * this.h;
					double num38 = num31 + num35 + num17 * this.h;
					double num39 = num28 + 2.0 * num33;
					double num40 = num30 + 2.0 * num34;
					double num41 = num32 + 2.0 * num35;
					double num42 = num28;
					double num43 = num30;
					double num44 = num32;
					double num45 = 0.0;
					double num46 = 0.0;
					double num47 = 0.0;
					if (this.cacheTangent && !this.config.UsePointPositionsToCalcTangents)
					{
						double num48 = 6.0 * ((double)originalFrom.x - (double)(2f * vector.x) + (double)originalToControl.x);
						double num49 = 6.0 * ((double)originalFrom.y - (double)(2f * vector.y) + (double)originalToControl.y);
						double num50 = 6.0 * ((double)originalFrom.z - (double)(2f * vector.z) + (double)originalToControl.z);
						double num51 = 3.0 * ((double)(-(double)originalFrom.x) + (double)(3f * vector.x) - (double)(3f * originalToControl.x) + (double)originalTo.x);
						double num52 = 3.0 * ((double)(-(double)originalFrom.y) + (double)(3f * vector.y) - (double)(3f * originalToControl.y) + (double)originalTo.y);
						double num53 = 3.0 * ((double)(-(double)originalFrom.z) + (double)(3f * vector.z) - (double)(3f * originalToControl.z) + (double)originalTo.z);
						double num54 = num51 * this.h2;
						double num55 = num52 * this.h2;
						double num56 = num53 * this.h2;
						num12 = num54 + num48 * this.h;
						num13 = num55 + num49 * this.h;
						num14 = num56 + num50 * this.h;
						num45 = 2.0 * num54;
						num46 = 2.0 * num55;
						num47 = 2.0 * num56;
						num9 = num15;
						num10 = num16;
						num11 = num17;
						double num57 = Math.Sqrt(num9 * num9 + num10 * num10 + num11 * num11);
						sectionPointInfo2.Tangent = ((num57 > 9.99999974737875E-06) ? new Vector3((float)(num9 / num57), (float)(num10 / num57), (float)(num11 / num57)) : Vector3.zero);
					}
					for (int j = 1; j < parts; j++)
					{
						BGCurveBaseMath.SectionPointInfo sectionPointInfo7 = points[j];
						num24 += num36;
						num25 += num37;
						num26 += num38;
						num36 += num39;
						num37 += num40;
						num38 += num41;
						num39 += num42;
						num40 += num43;
						num41 += num44;
						Vector3 vector4 = new Vector3((float)num24, (float)num25, (float)num26);
						if (flag5)
						{
							this.curve.ApplySnapping(ref vector4);
						}
						sectionPointInfo7.Position = vector4;
						if (this.cacheTangent)
						{
							if (this.config.UsePointPositionsToCalcTangents)
							{
								BGCurveBaseMath.SectionPointInfo sectionPointInfo8 = section[j - 1];
								Vector3 position2 = sectionPointInfo8.Position;
								Vector3 vector5 = new Vector3(vector4.x - position2.x, vector4.y - position2.y, vector4.z - position2.z);
								float num58 = (float)Math.Sqrt((double)vector5.x * (double)vector5.x + (double)vector5.y * (double)vector5.y + (double)vector5.z * (double)vector5.z);
								vector5 = (((double)num58 > 9.99999974737875E-06) ? new Vector3(vector5.x / num58, vector5.y / num58, vector5.z / num58) : Vector3.zero);
								sectionPointInfo8.Tangent = (sectionPointInfo7.Tangent = vector5);
							}
							else
							{
								num9 += num12;
								num10 += num13;
								num11 += num14;
								num12 += num45;
								num13 += num46;
								num14 += num47;
								double num59 = Math.Sqrt(num9 * num9 + num10 * num10 + num11 * num11);
								sectionPointInfo7.Tangent = ((num59 > 9.99999974737875E-06) ? new Vector3((float)(num9 / num59), (float)(num10 / num59), (float)(num11 / num59)) : Vector3.zero);
							}
						}
						Vector3 position3 = section[j - 1].Position;
						double num60 = (double)(vector4.x - position3.x);
						double num61 = (double)(vector4.y - position3.y);
						double num62 = (double)(vector4.z - position3.z);
						sectionPointInfo7.DistanceToSectionStart = section[j - 1].DistanceToSectionStart + (float)Math.Sqrt(num60 * num60 + num61 * num61 + num62 * num62);
					}
				}
				else
				{
					double num63 = 2.0 * ((double)vector.x - (double)originalFrom.x);
					double num64 = 2.0 * ((double)vector.y - (double)originalFrom.y);
					double num65 = 2.0 * ((double)vector.z - (double)originalFrom.z);
					double num66 = (double)originalFrom.x - (double)(2f * vector.x) + (double)originalTo.x;
					double num67 = (double)originalFrom.y - (double)(2f * vector.y) + (double)originalTo.y;
					double num68 = (double)originalFrom.z - (double)(2f * vector.z) + (double)originalTo.z;
					double num69 = num66 * this.h2 + num63 * this.h;
					double num70 = num67 * this.h2 + num64 * this.h;
					double num71 = num68 * this.h2 + num65 * this.h;
					double num72 = 2.0 * num66 * this.h2;
					double num73 = 2.0 * num67 * this.h2;
					double num74 = 2.0 * num68 * this.h2;
					double num75 = (double)originalFrom.x;
					double num76 = (double)originalFrom.y;
					double num77 = (double)originalFrom.z;
					if (this.cacheTangent && !this.config.UsePointPositionsToCalcTangents)
					{
						double num78 = 2.0 * ((double)originalFrom.x - (double)(2f * vector.x) + (double)originalTo.x);
						double num79 = 2.0 * ((double)originalFrom.y - (double)(2f * vector.y) + (double)originalTo.y);
						double num80 = 2.0 * ((double)originalFrom.z - (double)(2f * vector.z) + (double)originalTo.z);
						num12 = num78 * this.h;
						num13 = num79 * this.h;
						num14 = num80 * this.h;
						num9 = 2.0 * ((double)vector.x - (double)originalFrom.x);
						num10 = 2.0 * ((double)vector.y - (double)originalFrom.y);
						num11 = 2.0 * ((double)vector.z - (double)originalFrom.z);
						double num81 = Math.Sqrt(num9 * num9 + num10 * num10 + num11 * num11);
						sectionPointInfo2.Tangent = ((num81 > 9.99999974737875E-06) ? new Vector3((float)(num9 / num81), (float)(num10 / num81), (float)(num11 / num81)) : Vector3.zero);
					}
					for (int k = 1; k < parts; k++)
					{
						BGCurveBaseMath.SectionPointInfo sectionPointInfo9 = points[k];
						num75 += num69;
						num76 += num70;
						num77 += num71;
						num69 += num72;
						num70 += num73;
						num71 += num74;
						Vector3 vector6 = new Vector3((float)num75, (float)num76, (float)num77);
						if (flag5)
						{
							this.curve.ApplySnapping(ref vector6);
						}
						sectionPointInfo9.Position = vector6;
						if (this.cacheTangent)
						{
							if (this.config.UsePointPositionsToCalcTangents)
							{
								BGCurveBaseMath.SectionPointInfo sectionPointInfo10 = section[k - 1];
								Vector3 position4 = sectionPointInfo10.Position;
								Vector3 vector7 = new Vector3(vector6.x - position4.x, vector6.y - position4.y, vector6.z - position4.z);
								float num82 = (float)Math.Sqrt((double)vector7.x * (double)vector7.x + (double)vector7.y * (double)vector7.y + (double)vector7.z * (double)vector7.z);
								vector7 = (((double)num82 > 9.99999974737875E-06) ? new Vector3(vector7.x / num82, vector7.y / num82, vector7.z / num82) : Vector3.zero);
								sectionPointInfo10.Tangent = (sectionPointInfo9.Tangent = vector7);
							}
							else
							{
								num9 += num12;
								num10 += num13;
								num11 += num14;
								double num83 = Math.Sqrt(num9 * num9 + num10 * num10 + num11 * num11);
								sectionPointInfo9.Tangent = ((num83 > 9.99999974737875E-06) ? new Vector3((float)(num9 / num83), (float)(num10 / num83), (float)(num11 / num83)) : Vector3.zero);
							}
						}
						Vector3 position5 = section[k - 1].Position;
						double num84 = (double)(vector6.x - position5.x);
						double num85 = (double)(vector6.y - position5.y);
						double num86 = (double)(vector6.z - position5.z);
						sectionPointInfo9.DistanceToSectionStart = section[k - 1].DistanceToSectionStart + (float)Math.Sqrt(num84 * num84 + num85 * num85 + num86 * num86);
					}
				}
				if (this.cacheTangent && !this.config.UsePointPositionsToCalcTangents)
				{
					num9 += num12;
					num10 += num13;
					num11 += num14;
					double num87 = Math.Sqrt(num9 * num9 + num10 * num10 + num11 * num11);
					sectionPointInfo4.Tangent = ((num87 > 9.99999974737875E-06) ? new Vector3((float)(num9 / num87), (float)(num10 / num87), (float)(num11 / num87)) : Vector3.zero);
				}
			}
			BGCurveBaseMath.SectionPointInfo sectionPointInfo11 = section[parts - 1];
			Vector3 position6 = sectionPointInfo11.Position;
			Vector3 position7 = sectionPointInfo4.Position;
			double num88 = (double)(position7.x - position6.x);
			double num89 = (double)(position7.y - position6.y);
			double num90 = (double)(position7.z - position6.z);
			sectionPointInfo4.DistanceToSectionStart = sectionPointInfo11.DistanceToSectionStart + (float)Math.Sqrt(num88 * num88 + num89 * num89 + num90 * num90);
			if (this.cacheTangent && this.config.UsePointPositionsToCalcTangents)
			{
				Vector3 vector8 = new Vector3((float)num88, (float)num89, (float)num90);
				float num91 = (float)Math.Sqrt((double)vector8.x * (double)vector8.x + (double)vector8.y * (double)vector8.y + (double)vector8.z * (double)vector8.z);
				vector8 = (((double)num91 > 9.99999974737875E-06) ? new Vector3(vector8.x / num91, vector8.y / num91, vector8.z / num91) : Vector3.zero);
				sectionPointInfo4.Tangent = vector8;
			}
		}

		// Token: 0x06000D6D RID: 3437 RVA: 0x000D6E04 File Offset: 0x000D5004
		protected virtual void BinarySearchByDistance(float distance, out Vector3 position, out Vector3 tangent, bool calculatePosition, bool calculateTangent)
		{
			int pointsCount = this.curve.PointsCount;
			if (pointsCount == 0)
			{
				position = Vector3.zero;
				tangent = Vector3.zero;
				return;
			}
			if (pointsCount == 1)
			{
				position = this.curve[0].PositionWorld;
				tangent = Vector3.zero;
				return;
			}
			if (this.cachedSectionInfos.Count == 0)
			{
				position = Vector3.zero;
				tangent = Vector3.zero;
				return;
			}
			if (distance < 0f)
			{
				distance = 0f;
			}
			else if (distance > this.cachedLength)
			{
				distance = this.cachedLength;
			}
			if (calculateTangent && ((BGCurveBaseMath.Fields)2 & this.config.Fields) == (BGCurveBaseMath.Fields)0)
			{
				throw new UnityException("Can not calculate tangent, cause it was not included in the 'fields' constructor parameter. For example, use new BGCurveBaseMath(curve, new BGCurveBaseMath.Config(BGCurveBaseMath.Fields.PositionAndTangent))to calculate world's position and tangent");
			}
			BGCurveBaseMath.SectionInfo sectionInfo = this.cachedSectionInfos[this.FindSectionIndexByDistance(distance)];
			sectionInfo.CalcByDistance(distance - sectionInfo.DistanceFromStartToOrigin, out position, out tangent, calculatePosition, calculateTangent);
		}

		// Token: 0x06000D6E RID: 3438 RVA: 0x000D6EEC File Offset: 0x000D50EC
		protected int FindSectionIndexByDistance(float distance)
		{
			int i = 0;
			int num = 0;
			int num2 = this.cachedSectionInfos.Count;
			int num3 = 0;
			while (i < num2)
			{
				num = i + num2 >> 1;
				BGCurveBaseMath.SectionInfo sectionInfo = this.cachedSectionInfos[num];
				if (distance >= sectionInfo.DistanceFromStartToOrigin && distance <= sectionInfo.DistanceFromEndToOrigin)
				{
					break;
				}
				if (distance < sectionInfo.DistanceFromStartToOrigin)
				{
					num2 = num;
				}
				else
				{
					i = num + 1;
				}
				if (num3++ > 100)
				{
					throw new UnityException("Something wrong: more than 100 iterations inside BinarySearch");
				}
			}
			return num;
		}

		// Token: 0x06000D6F RID: 3439 RVA: 0x0004BA73 File Offset: 0x00049C73
		protected float DistanceByRatio(float distanceRatio)
		{
			return this.GetDistance(-1) * Mathf.Clamp01(distanceRatio);
		}

		// Token: 0x06000D70 RID: 3440 RVA: 0x0004BA83 File Offset: 0x00049C83
		protected float ClampDistance(float distance)
		{
			return Mathf.Clamp(distance, 0f, this.GetDistance(-1));
		}

		// Token: 0x06000D71 RID: 3441 RVA: 0x000D6F60 File Offset: 0x000D5160
		public override string ToString()
		{
			string str = "Base Math for curve (";
			BGCurve bgcurve = this.Curve;
			return str + ((bgcurve != null) ? bgcurve.ToString() : null) + "), sections=" + this.SectionsCount.ToString();
		}

		// Token: 0x06000D72 RID: 3442 RVA: 0x000D6F9C File Offset: 0x000D519C
		protected void Resize(List<BGCurveBaseMath.SectionPointInfo> points, int size)
		{
			int count = points.Count;
			if (count == size)
			{
				return;
			}
			if (count < size)
			{
				int num = this.poolPointInfos.Count - 1;
				for (int i = count; i < size; i++)
				{
					points.Add((num >= 0) ? this.poolPointInfos[num--] : new BGCurveBaseMath.SectionPointInfo());
				}
				if (num != this.poolPointInfos.Count - 1)
				{
					this.poolPointInfos.RemoveRange(num + 1, this.poolPointInfos.Count - 1 - num);
					return;
				}
			}
			else
			{
				for (int j = size; j < count; j++)
				{
					this.poolPointInfos.Add(points[j]);
				}
				points.RemoveRange(size, count - size);
			}
		}

		// Token: 0x06000D73 RID: 3443 RVA: 0x0004BA97 File Offset: 0x00049C97
		private void CurveChanged(object sender, BGCurveChangedArgs e)
		{
			this.ignoreSectionChangedCheck = (e != null && e.ChangeType == BGCurveChangedArgs.ChangeTypeEnum.Snap);
			this.Recalculate(false);
			this.ignoreSectionChangedCheck = false;
		}

		// Token: 0x06000D74 RID: 3444 RVA: 0x0004BABC File Offset: 0x00049CBC
		private void ConfigOnUpdate(object sender, EventArgs eventArgs)
		{
			this.Recalculate(true);
		}

		// Token: 0x04000C6F RID: 3183
		protected readonly BGCurve curve;

		// Token: 0x04000C70 RID: 3184
		protected BGCurveBaseMath.Config config;

		// Token: 0x04000C71 RID: 3185
		protected readonly List<BGCurveBaseMath.SectionInfo> cachedSectionInfos = new List<BGCurveBaseMath.SectionInfo>();

		// Token: 0x04000C72 RID: 3186
		protected readonly List<BGCurveBaseMath.SectionInfo> poolSectionInfos = new List<BGCurveBaseMath.SectionInfo>();

		// Token: 0x04000C73 RID: 3187
		protected readonly List<BGCurveBaseMath.SectionPointInfo> poolPointInfos = new List<BGCurveBaseMath.SectionPointInfo>();

		// Token: 0x04000C74 RID: 3188
		protected float cachedLength;

		// Token: 0x04000C75 RID: 3189
		protected bool cachePosition;

		// Token: 0x04000C76 RID: 3190
		protected bool cacheTangent;

		// Token: 0x04000C77 RID: 3191
		protected BGCurveCalculatorClosestPoint closestPointCalculator;

		// Token: 0x04000C78 RID: 3192
		private int recalculatedAtFrame = -1;

		// Token: 0x04000C79 RID: 3193
		private int createdAtFrame;

		// Token: 0x04000C7A RID: 3194
		protected bool ignoreSectionChangedCheck;

		// Token: 0x04000C7B RID: 3195
		private double h;

		// Token: 0x04000C7C RID: 3196
		private double h2;

		// Token: 0x04000C7D RID: 3197
		private double h3;

		// Token: 0x02000188 RID: 392
		public enum Field
		{
			// Token: 0x04000C80 RID: 3200
			Position = 1,
			// Token: 0x04000C81 RID: 3201
			Tangent
		}

		// Token: 0x02000189 RID: 393
		public enum Fields
		{
			// Token: 0x04000C83 RID: 3203
			Position = 1,
			// Token: 0x04000C84 RID: 3204
			PositionAndTangent = 3
		}

		// Token: 0x0200018A RID: 394
		public class Config
		{
			// Token: 0x1400000B RID: 11
			// (add) Token: 0x06000D75 RID: 3445 RVA: 0x000D704C File Offset: 0x000D524C
			// (remove) Token: 0x06000D76 RID: 3446 RVA: 0x000D7084 File Offset: 0x000D5284
			public event EventHandler Update;

			// Token: 0x06000D77 RID: 3447 RVA: 0x0004BAC5 File Offset: 0x00049CC5
			public Config()
			{
			}

			// Token: 0x06000D78 RID: 3448 RVA: 0x0004BADC File Offset: 0x00049CDC
			public Config(BGCurveBaseMath.Fields fields)
			{
				this.Fields = fields;
			}

			// Token: 0x06000D79 RID: 3449 RVA: 0x0004BAFA File Offset: 0x00049CFA
			protected bool Equals(BGCurveBaseMath.Config other)
			{
				return this.Fields == other.Fields && this.Parts == other.Parts && this.UsePointPositionsToCalcTangents == other.UsePointPositionsToCalcTangents && this.OptimizeStraightLines == other.OptimizeStraightLines;
			}

			// Token: 0x06000D7A RID: 3450 RVA: 0x0004BB36 File Offset: 0x00049D36
			public override bool Equals(object obj)
			{
				return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((BGCurveBaseMath.Config)obj)));
			}

			// Token: 0x06000D7B RID: 3451 RVA: 0x0004BB64 File Offset: 0x00049D64
			public override int GetHashCode()
			{
				return (int)(((this.Fields * (BGCurveBaseMath.Fields)397 ^ (BGCurveBaseMath.Fields)this.Parts) * (BGCurveBaseMath.Fields)397 ^ (BGCurveBaseMath.Fields)this.UsePointPositionsToCalcTangents.GetHashCode()) * (BGCurveBaseMath.Fields)397 ^ (BGCurveBaseMath.Fields)this.OptimizeStraightLines.GetHashCode());
			}

			// Token: 0x06000D7C RID: 3452 RVA: 0x0004BB9D File Offset: 0x00049D9D
			public void FireUpdate()
			{
				if (this.Update != null)
				{
					this.Update(this, null);
				}
			}

			// Token: 0x04000C85 RID: 3205
			public BGCurveBaseMath.Fields Fields = BGCurveBaseMath.Fields.Position;

			// Token: 0x04000C86 RID: 3206
			public int Parts = 30;

			// Token: 0x04000C87 RID: 3207
			public bool UsePointPositionsToCalcTangents;

			// Token: 0x04000C88 RID: 3208
			public bool OptimizeStraightLines;

			// Token: 0x04000C89 RID: 3209
			public Func<bool> ShouldUpdate;
		}

		// Token: 0x0200018B RID: 395
		public class SectionInfo
		{
			// Token: 0x170003A8 RID: 936
			// (get) Token: 0x06000D7D RID: 3453 RVA: 0x0004BBB4 File Offset: 0x00049DB4
			public List<BGCurveBaseMath.SectionPointInfo> Points
			{
				get
				{
					return this.points;
				}
			}

			// Token: 0x170003A9 RID: 937
			// (get) Token: 0x06000D7E RID: 3454 RVA: 0x0004BBBC File Offset: 0x00049DBC
			public int PointsCount
			{
				get
				{
					return this.points.Count;
				}
			}

			// Token: 0x170003AA RID: 938
			// (get) Token: 0x06000D7F RID: 3455 RVA: 0x0004BBC9 File Offset: 0x00049DC9
			public float Distance
			{
				get
				{
					return this.DistanceFromEndToOrigin - this.DistanceFromStartToOrigin;
				}
			}

			// Token: 0x06000D80 RID: 3456 RVA: 0x000D70BC File Offset: 0x000D52BC
			public override string ToString()
			{
				return "Section distance=(" + this.Distance.ToString() + ")";
			}

			// Token: 0x170003AB RID: 939
			public BGCurveBaseMath.SectionPointInfo this[int i]
			{
				get
				{
					return this.points[i];
				}
				set
				{
					this.points[i] = value;
				}
			}

			// Token: 0x06000D83 RID: 3459 RVA: 0x000D70E8 File Offset: 0x000D52E8
			protected internal bool Reset(BGCurvePointI fromPoint, BGCurvePointI toPoint, int pointsCount, bool skipCheck)
			{
				Vector3 positionWorld = fromPoint.PositionWorld;
				Vector3 positionWorld2 = toPoint.PositionWorld;
				Vector3 controlSecondWorld = fromPoint.ControlSecondWorld;
				Vector3 controlFirstWorld = toPoint.ControlFirstWorld;
				if (!skipCheck && this.points.Count == pointsCount && this.OriginalFromControlType == fromPoint.ControlType && this.OriginalToControlType == toPoint.ControlType && Vector3.SqrMagnitude(new Vector3(this.OriginalFrom.x - positionWorld.x, this.OriginalFrom.y - positionWorld.y, this.OriginalFrom.z - positionWorld.z)) < 1E-06f && Vector3.SqrMagnitude(new Vector3(this.OriginalTo.x - positionWorld2.x, this.OriginalTo.y - positionWorld2.y, this.OriginalTo.z - positionWorld2.z)) < 1E-06f && Vector3.SqrMagnitude(new Vector3(this.OriginalFromControl.x - controlSecondWorld.x, this.OriginalFromControl.y - controlSecondWorld.y, this.OriginalFromControl.z - controlSecondWorld.z)) < 1E-06f && Vector3.SqrMagnitude(new Vector3(this.OriginalToControl.x - controlFirstWorld.x, this.OriginalToControl.y - controlFirstWorld.y, this.OriginalToControl.z - controlFirstWorld.z)) < 1E-06f)
				{
					return false;
				}
				this.OriginalFrom = positionWorld;
				this.OriginalTo = positionWorld2;
				this.OriginalFromControlType = fromPoint.ControlType;
				this.OriginalToControlType = toPoint.ControlType;
				this.OriginalFromControl = controlSecondWorld;
				this.OriginalToControl = controlFirstWorld;
				return true;
			}

			// Token: 0x06000D84 RID: 3460 RVA: 0x000D72A4 File Offset: 0x000D54A4
			public int FindPointIndexByDistance(float distanceWithinSection)
			{
				int num = this.points.Count - 1;
				int i = 0;
				int num2 = 0;
				int num3 = this.points.Count;
				int num4 = 0;
				while (i < num3)
				{
					num2 = i + num3 >> 1;
					BGCurveBaseMath.SectionPointInfo sectionPointInfo = this.points[num2];
					if (distanceWithinSection >= sectionPointInfo.DistanceToSectionStart && (num2 == num || this.points[num2 + 1].DistanceToSectionStart >= distanceWithinSection))
					{
						break;
					}
					if (distanceWithinSection < sectionPointInfo.DistanceToSectionStart)
					{
						num3 = num2;
					}
					else
					{
						i = num2 + 1;
					}
					if (num4++ > 100)
					{
						throw new UnityException("Something wrong: more than 100 iterations inside BinarySearch");
					}
				}
				return num2;
			}

			// Token: 0x06000D85 RID: 3461 RVA: 0x000D733C File Offset: 0x000D553C
			public void CalcByDistance(float distanceWithinSection, out Vector3 position, out Vector3 tangent, bool calculatePosition, bool calculateTangent)
			{
				position = Vector3.zero;
				tangent = Vector3.zero;
				if (this.points.Count == 2)
				{
					BGCurveBaseMath.SectionPointInfo sectionPointInfo = this.points[0];
					if (Math.Abs(this.Distance) < 1E-05f)
					{
						if (calculatePosition)
						{
							position = sectionPointInfo.Position;
						}
						if (calculateTangent)
						{
							tangent = sectionPointInfo.Tangent;
							return;
						}
					}
					else
					{
						float t = distanceWithinSection / this.Distance;
						BGCurveBaseMath.SectionPointInfo sectionPointInfo2 = this.points[1];
						if (calculatePosition)
						{
							position = Vector3.Lerp(sectionPointInfo.Position, sectionPointInfo2.Position, t);
						}
						if (calculateTangent)
						{
							tangent = Vector3.Lerp(sectionPointInfo.Tangent, sectionPointInfo2.Tangent, t);
							return;
						}
					}
				}
				else
				{
					int num = this.FindPointIndexByDistance(distanceWithinSection);
					BGCurveBaseMath.SectionPointInfo sectionPointInfo3 = this.points[num];
					if (num == this.points.Count - 1)
					{
						if (calculatePosition)
						{
							position = sectionPointInfo3.Position;
						}
						if (calculateTangent)
						{
							tangent = sectionPointInfo3.Tangent;
							return;
						}
					}
					else
					{
						BGCurveBaseMath.SectionPointInfo sectionPointInfo4 = this.points[num + 1];
						float num2 = sectionPointInfo4.DistanceToSectionStart - sectionPointInfo3.DistanceToSectionStart;
						float num3 = distanceWithinSection - sectionPointInfo3.DistanceToSectionStart;
						float t2 = (Math.Abs(num2) < 1E-05f) ? 0f : (num3 / num2);
						if (calculatePosition)
						{
							position = Vector3.Lerp(sectionPointInfo3.Position, sectionPointInfo4.Position, t2);
						}
						if (calculateTangent)
						{
							tangent = Vector3.Lerp(sectionPointInfo3.Tangent, sectionPointInfo4.Tangent, t2);
						}
					}
				}
			}

			// Token: 0x04000C8B RID: 3211
			public float DistanceFromStartToOrigin;

			// Token: 0x04000C8C RID: 3212
			public float DistanceFromEndToOrigin;

			// Token: 0x04000C8D RID: 3213
			protected internal readonly List<BGCurveBaseMath.SectionPointInfo> points = new List<BGCurveBaseMath.SectionPointInfo>();

			// Token: 0x04000C8E RID: 3214
			public Vector3 OriginalFrom;

			// Token: 0x04000C8F RID: 3215
			public Vector3 OriginalTo;

			// Token: 0x04000C90 RID: 3216
			public BGCurvePoint.ControlTypeEnum OriginalFromControlType;

			// Token: 0x04000C91 RID: 3217
			public BGCurvePoint.ControlTypeEnum OriginalToControlType;

			// Token: 0x04000C92 RID: 3218
			public Vector3 OriginalFromControl;

			// Token: 0x04000C93 RID: 3219
			public Vector3 OriginalToControl;

			// Token: 0x04000C94 RID: 3220
			public Vector3 OriginalFirstPointTangent;

			// Token: 0x04000C95 RID: 3221
			public Vector3 OriginalLastPointTangent;
		}

		// Token: 0x0200018C RID: 396
		public class SectionPointInfo
		{
			// Token: 0x06000D87 RID: 3463 RVA: 0x000D74D8 File Offset: 0x000D56D8
			internal Vector3 GetField(BGCurveBaseMath.Field field)
			{
				Vector3 result;
				if (field != BGCurveBaseMath.Field.Position)
				{
					if (field != BGCurveBaseMath.Field.Tangent)
					{
						throw new UnityException("Unknown field=" + field.ToString());
					}
					result = this.Tangent;
				}
				else
				{
					result = this.Position;
				}
				return result;
			}

			// Token: 0x06000D88 RID: 3464 RVA: 0x0004BC08 File Offset: 0x00049E08
			internal Vector3 LerpTo(BGCurveBaseMath.Field field, BGCurveBaseMath.SectionPointInfo to, float ratio)
			{
				return Vector3.Lerp(this.GetField(field), to.GetField(field), ratio);
			}

			// Token: 0x06000D89 RID: 3465 RVA: 0x000D7520 File Offset: 0x000D5720
			public override string ToString()
			{
				string str = "Point at (";
				Vector3 position = this.Position;
				return str + position.ToString() + ")";
			}

			// Token: 0x04000C96 RID: 3222
			public Vector3 Position;

			// Token: 0x04000C97 RID: 3223
			public float DistanceToSectionStart;

			// Token: 0x04000C98 RID: 3224
			public Vector3 Tangent;
		}
	}
}
