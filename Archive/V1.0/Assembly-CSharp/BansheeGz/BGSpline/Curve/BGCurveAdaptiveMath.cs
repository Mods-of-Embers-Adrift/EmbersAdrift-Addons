using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGSpline.Curve
{
	// Token: 0x02000185 RID: 389
	public class BGCurveAdaptiveMath : BGCurveBaseMath
	{
		// Token: 0x06000D33 RID: 3379 RVA: 0x0004B854 File Offset: 0x00049A54
		public BGCurveAdaptiveMath(BGCurve curve, BGCurveAdaptiveMath.ConfigAdaptive config) : base(curve, config)
		{
		}

		// Token: 0x06000D34 RID: 3380 RVA: 0x000D4150 File Offset: 0x000D2350
		public override void Init(BGCurveBaseMath.Config config)
		{
			BGCurveAdaptiveMath.ConfigAdaptive configAdaptive = (BGCurveAdaptiveMath.ConfigAdaptive)config;
			this.tolerance = Mathf.Clamp(configAdaptive.Tolerance, 0.1f, 0.999975f);
			this.tolerance *= this.tolerance;
			this.tolerance *= this.tolerance;
			this.toleranceRatio = 1f / (1f - this.tolerance);
			this.toleranceRatioSquared = this.toleranceRatio * this.toleranceRatio;
			this.ignoreSectionChangedCheckOverride = (this.config == null || Math.Abs(((BGCurveAdaptiveMath.ConfigAdaptive)this.config).Tolerance - this.tolerance) > 1E-05f);
			base.Init(config);
		}

		// Token: 0x06000D35 RID: 3381 RVA: 0x0004B85E File Offset: 0x00049A5E
		protected override bool Reset(BGCurveBaseMath.SectionInfo section, BGCurvePointI from, BGCurvePointI to, int pointsCount)
		{
			return section.Reset(from, to, section.PointsCount, this.ignoreSectionChangedCheck || this.ignoreSectionChangedCheckOverride);
		}

		// Token: 0x06000D36 RID: 3382 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool IsUseDistanceToAdjustTangents(BGCurveBaseMath.SectionInfo section, BGCurveBaseMath.SectionInfo prevSection)
		{
			return true;
		}

		// Token: 0x06000D37 RID: 3383 RVA: 0x000D420C File Offset: 0x000D240C
		protected override void CalculateSplitSection(BGCurveBaseMath.SectionInfo section, BGCurvePointI from, BGCurvePointI to)
		{
			bool flag = this.cacheTangent && !this.config.UsePointPositionsToCalcTangents;
			List<BGCurveBaseMath.SectionPointInfo> points = section.points;
			int count = points.Count;
			for (int i = 0; i < count; i++)
			{
				this.poolPointInfos.Add(points[i]);
			}
			points.Clear();
			Vector3 originalFrom = section.OriginalFrom;
			Vector3 originalFromControl = section.OriginalFromControl;
			Vector3 originalToControl = section.OriginalToControl;
			Vector3 originalTo = section.OriginalTo;
			int num = ((section.OriginalFromControlType != BGCurvePoint.ControlTypeEnum.Absent) ? 2 : 0) + ((section.OriginalToControlType != BGCurvePoint.ControlTypeEnum.Absent) ? 1 : 0);
			int num2 = this.poolPointInfos.Count - 1;
			BGCurveBaseMath.SectionPointInfo sectionPointInfo;
			if (num2 >= 0)
			{
				sectionPointInfo = this.poolPointInfos[num2];
				this.poolPointInfos.RemoveAt(num2);
			}
			else
			{
				sectionPointInfo = new BGCurveBaseMath.SectionPointInfo();
			}
			sectionPointInfo.Position = originalFrom;
			sectionPointInfo.DistanceToSectionStart = 0f;
			points.Add(sectionPointInfo);
			switch (num)
			{
			case 1:
				this.RecursiveQuadraticSplit(section, (double)originalFrom.x, (double)originalFrom.y, (double)originalFrom.z, (double)originalToControl.x, (double)originalToControl.y, (double)originalToControl.z, (double)originalTo.x, (double)originalTo.y, (double)originalTo.z, 0, true, flag, 0.0, 1.0);
				break;
			case 2:
				this.RecursiveQuadraticSplit(section, (double)originalFrom.x, (double)originalFrom.y, (double)originalFrom.z, (double)originalFromControl.x, (double)originalFromControl.y, (double)originalFromControl.z, (double)originalTo.x, (double)originalTo.y, (double)originalTo.z, 0, false, flag, 0.0, 1.0);
				break;
			case 3:
				this.RecursiveCubicSplit(section, (double)originalFrom.x, (double)originalFrom.y, (double)originalFrom.z, (double)originalFromControl.x, (double)originalFromControl.y, (double)originalFromControl.z, (double)originalToControl.x, (double)originalToControl.y, (double)originalToControl.z, (double)originalTo.x, (double)originalTo.y, (double)originalTo.z, 0, flag, 0.0, 1.0);
				break;
			}
			num2 = this.poolPointInfos.Count - 1;
			BGCurveBaseMath.SectionPointInfo sectionPointInfo2;
			if (num2 >= 0)
			{
				sectionPointInfo2 = this.poolPointInfos[num2];
				this.poolPointInfos.RemoveAt(num2);
			}
			else
			{
				sectionPointInfo2 = new BGCurveBaseMath.SectionPointInfo();
			}
			sectionPointInfo2.Position = originalTo;
			points.Add(sectionPointInfo2);
			flag = (this.cacheTangent && this.config.UsePointPositionsToCalcTangents);
			BGCurveBaseMath.SectionPointInfo sectionPointInfo3 = points[0];
			for (int j = 1; j < points.Count; j++)
			{
				BGCurveBaseMath.SectionPointInfo sectionPointInfo4 = points[j];
				Vector3 position = sectionPointInfo4.Position;
				Vector3 position2 = sectionPointInfo3.Position;
				double num3 = (double)position.x - (double)position2.x;
				double num4 = (double)position.y - (double)position2.y;
				double num5 = (double)position.z - (double)position2.z;
				sectionPointInfo4.DistanceToSectionStart = sectionPointInfo3.DistanceToSectionStart + (float)Math.Sqrt(num3 * num3 + num4 * num4 + num5 * num5);
				if (flag)
				{
					sectionPointInfo4.Tangent = Vector3.Normalize(position - position2);
				}
				sectionPointInfo3 = sectionPointInfo4;
			}
			if (this.cacheTangent)
			{
				if (this.config.UsePointPositionsToCalcTangents)
				{
					sectionPointInfo.Tangent = (points[1].Position - sectionPointInfo.Position).normalized;
					sectionPointInfo2.Tangent = points[points.Count - 2].Tangent;
					return;
				}
				switch (num)
				{
				case 0:
					sectionPointInfo.Tangent = (sectionPointInfo2.Tangent = (sectionPointInfo2.Position - sectionPointInfo.Position).normalized);
					return;
				case 1:
					sectionPointInfo.Tangent = Vector3.Normalize(section.OriginalToControl - section.OriginalFrom);
					sectionPointInfo2.Tangent = Vector3.Normalize(section.OriginalTo - section.OriginalToControl);
					return;
				case 2:
					sectionPointInfo.Tangent = Vector3.Normalize(section.OriginalFromControl - section.OriginalFrom);
					sectionPointInfo2.Tangent = Vector3.Normalize(section.OriginalTo - section.OriginalFromControl);
					return;
				case 3:
					sectionPointInfo.Tangent = Vector3.Normalize(section.OriginalFromControl - section.OriginalFrom);
					sectionPointInfo2.Tangent = Vector3.Normalize(section.OriginalTo - section.OriginalToControl);
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06000D38 RID: 3384 RVA: 0x000D46C4 File Offset: 0x000D28C4
		private void RecursiveQuadraticSplit(BGCurveBaseMath.SectionInfo section, double x0, double y0, double z0, double x1, double y1, double z1, double x2, double y2, double z2, int level, bool useSecond, bool calcTangents, double fromT, double toT)
		{
			if (level > 24)
			{
				return;
			}
			double num = x0 - x1;
			double num2 = y0 - y1;
			double num3 = z0 - z1;
			double num4 = x1 - x2;
			double num5 = y1 - y2;
			double num6 = z1 - z2;
			double num7 = x0 - x2;
			double num8 = y0 - y2;
			double num9 = z0 - z2;
			double num10 = num7 * num7 + num8 * num8 + num9 * num9;
			double num11 = num * num + num2 * num2 + num3 * num3;
			double num12 = num4 * num4 + num5 * num5 + num6 * num6;
			double num13 = num10 * (double)this.toleranceRatioSquared - num11 - num12;
			if (4.0 * num11 * num12 < num13 * num13 || num10 + num11 + num12 < 0.009999999776482582)
			{
				return;
			}
			double num14 = (x0 + x1) * 0.5;
			double num15 = (y0 + y1) * 0.5;
			double num16 = (z0 + z1) * 0.5;
			double num17 = (x1 + x2) * 0.5;
			double num18 = (y1 + y2) * 0.5;
			double num19 = (z1 + z2) * 0.5;
			double num20 = (num14 + num17) * 0.5;
			double num21 = (num15 + num18) * 0.5;
			double num22 = (num16 + num19) * 0.5;
			double num23 = calcTangents ? ((fromT + toT) * 0.5) : 0.0;
			Vector3 position = new Vector3((float)num20, (float)num21, (float)num22);
			if (this.curve.SnapType == BGCurve.SnapTypeEnum.Curve)
			{
				this.curve.ApplySnapping(ref position);
			}
			this.RecursiveQuadraticSplit(section, x0, y0, z0, num14, num15, num16, num20, num21, num22, level + 1, useSecond, calcTangents, fromT, num23);
			int num24 = this.poolPointInfos.Count - 1;
			BGCurveBaseMath.SectionPointInfo sectionPointInfo;
			if (num24 >= 0)
			{
				sectionPointInfo = this.poolPointInfos[num24];
				this.poolPointInfos.RemoveAt(num24);
			}
			else
			{
				sectionPointInfo = new BGCurveBaseMath.SectionPointInfo();
			}
			sectionPointInfo.Position = position;
			section.points.Add(sectionPointInfo);
			if (calcTangents)
			{
				Vector3 vector = useSecond ? section.OriginalToControl : section.OriginalFromControl;
				sectionPointInfo.Tangent = Vector3.Normalize(2f * (1f - (float)num23) * (vector - section.OriginalFrom) + 2f * (float)num23 * (section.OriginalTo - vector));
			}
			this.RecursiveQuadraticSplit(section, num20, num21, num22, num17, num18, num19, x2, y2, z2, level + 1, useSecond, calcTangents, num23, toT);
		}

		// Token: 0x06000D39 RID: 3385 RVA: 0x000D4954 File Offset: 0x000D2B54
		private void RecursiveCubicSplit(BGCurveBaseMath.SectionInfo section, double x0, double y0, double z0, double x1, double y1, double z1, double x2, double y2, double z2, double x3, double y3, double z3, int level, bool calcTangents, double fromT, double toT)
		{
			if (level > 24)
			{
				return;
			}
			double num = x0 - x1;
			double num2 = y0 - y1;
			double num3 = z0 - z1;
			double num4 = x1 - x2;
			double num5 = y1 - y2;
			double num6 = z1 - z2;
			double num7 = x2 - x3;
			double num8 = y2 - y3;
			double num9 = z2 - z3;
			double num10 = x0 - x3;
			double num11 = y0 - y3;
			double num12 = z0 - z3;
			double num13 = num10 * num10 + num11 * num11 + num12 * num12;
			double num14 = num * num + num2 * num2 + num3 * num3;
			double num15 = num4 * num4 + num5 * num5 + num6 * num6;
			double num16 = num7 * num7 + num8 * num8 + num9 * num9;
			if (Math.Sqrt(num14 * num15) + Math.Sqrt(num15 * num16) + Math.Sqrt(num14 * num16) < (num13 * (double)this.toleranceRatioSquared - num14 - num15 - num16) * 0.5 || num13 + num14 + num15 + num16 < 0.009999999776482582)
			{
				return;
			}
			double num17 = (x0 + x1) * 0.5;
			double num18 = (y0 + y1) * 0.5;
			double num19 = (z0 + z1) * 0.5;
			double num20 = (x1 + x2) * 0.5;
			double num21 = (y1 + y2) * 0.5;
			double num22 = (z1 + z2) * 0.5;
			double num23 = (x2 + x3) * 0.5;
			double num24 = (y2 + y3) * 0.5;
			double num25 = (z2 + z3) * 0.5;
			double num26 = (num17 + num20) * 0.5;
			double num27 = (num18 + num21) * 0.5;
			double num28 = (num19 + num22) * 0.5;
			double num29 = (num20 + num23) * 0.5;
			double num30 = (num21 + num24) * 0.5;
			double num31 = (num22 + num25) * 0.5;
			double num32 = (num26 + num29) * 0.5;
			double num33 = (num27 + num30) * 0.5;
			double num34 = (num28 + num31) * 0.5;
			double num35 = calcTangents ? ((fromT + toT) * 0.5) : 0.0;
			Vector3 position = new Vector3((float)num32, (float)num33, (float)num34);
			if (this.curve.SnapType == BGCurve.SnapTypeEnum.Curve)
			{
				this.curve.ApplySnapping(ref position);
			}
			this.RecursiveCubicSplit(section, x0, y0, z0, num17, num18, num19, num26, num27, num28, num32, num33, num34, level + 1, calcTangents, fromT, num35);
			int num36 = this.poolPointInfos.Count - 1;
			BGCurveBaseMath.SectionPointInfo sectionPointInfo;
			if (num36 >= 0)
			{
				sectionPointInfo = this.poolPointInfos[num36];
				this.poolPointInfos.RemoveAt(num36);
			}
			else
			{
				sectionPointInfo = new BGCurveBaseMath.SectionPointInfo();
			}
			sectionPointInfo.Position = position;
			section.points.Add(sectionPointInfo);
			if (calcTangents)
			{
				double num37 = 1.0 - num35;
				sectionPointInfo.Tangent = Vector3.Normalize(3f * (float)(num37 * num37) * (section.OriginalFromControl - section.OriginalFrom) + 6f * (float)(num37 * num35) * (section.OriginalToControl - section.OriginalFromControl) + 3f * (float)(num35 * num35) * (section.OriginalTo - section.OriginalToControl));
			}
			this.RecursiveCubicSplit(section, num32, num33, num34, num29, num30, num31, num23, num24, num25, x3, y3, z3, level + 1, calcTangents, num35, toT);
		}

		// Token: 0x06000D3A RID: 3386 RVA: 0x000D4CF0 File Offset: 0x000D2EF0
		public override string ToString()
		{
			string str = "Adaptive Math for curve (";
			BGCurve curve = base.Curve;
			return str + ((curve != null) ? curve.ToString() : null) + "), sections=" + base.SectionsCount.ToString();
		}

		// Token: 0x04000C64 RID: 3172
		public const float MinTolerance = 0.1f;

		// Token: 0x04000C65 RID: 3173
		public const float MaxTolerance = 0.999975f;

		// Token: 0x04000C66 RID: 3174
		public const float DistanceTolerance = 0.01f;

		// Token: 0x04000C67 RID: 3175
		private const int RecursionLimit = 24;

		// Token: 0x04000C68 RID: 3176
		private bool ignoreSectionChangedCheckOverride;

		// Token: 0x04000C69 RID: 3177
		private float toleranceRatio;

		// Token: 0x04000C6A RID: 3178
		private float toleranceRatioSquared;

		// Token: 0x04000C6B RID: 3179
		private float tolerance;

		// Token: 0x02000186 RID: 390
		public class ConfigAdaptive : BGCurveBaseMath.Config
		{
			// Token: 0x06000D3B RID: 3387 RVA: 0x0004B87F File Offset: 0x00049A7F
			public ConfigAdaptive(BGCurveBaseMath.Fields fields) : base(fields)
			{
			}

			// Token: 0x04000C6C RID: 3180
			public float Tolerance = 0.2f;
		}
	}
}
