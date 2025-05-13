using System;
using System.Collections.Generic;
using BansheeGz.BGSpline.Curve;
using UnityEngine;

namespace BansheeGz.BGSpline.Components
{
	// Token: 0x020001CD RID: 461
	public class BGPolylineSplitter
	{
		// Token: 0x06001087 RID: 4231 RVA: 0x000DF7B8 File Offset: 0x000DD9B8
		public void Bind(List<Vector3> positions, BGCcMath math, BGPolylineSplitter.Config config, List<BGCcSplitterPolyline.PolylinePoint> points)
		{
			positions.Clear();
			points.Clear();
			bool flag = math.IsCalculated(BGCurveBaseMath.Field.Tangent);
			BGCurveBaseMath math2 = math.Math;
			int sectionsCount = math2.SectionsCount;
			int straightLinesCount = 0;
			if (!config.DoNotOptimizeStraightLines)
			{
				if (this.straightBits == null || this.straightBits.Length < sectionsCount)
				{
					Array.Resize<bool>(ref this.straightBits, sectionsCount);
				}
				straightLinesCount = BGPolylineSplitter.CountStraightLines(math2, this.straightBits);
			}
			this.InitProvider(ref this.positionsProvider, math, config).Build(positions, straightLinesCount, this.straightBits, points);
			if (!config.UseLocal)
			{
				return;
			}
			Matrix4x4 worldToLocalMatrix = config.Transform.worldToLocalMatrix;
			int count = positions.Count;
			for (int i = 0; i < count; i++)
			{
				Vector3 vector = worldToLocalMatrix.MultiplyPoint(positions[i]);
				positions[i] = vector;
				BGCcSplitterPolyline.PolylinePoint polylinePoint = points[i];
				points[i] = new BGCcSplitterPolyline.PolylinePoint(vector, polylinePoint.Distance, flag ? config.Transform.InverseTransformDirection(polylinePoint.Tangent) : Vector3.zero);
			}
		}

		// Token: 0x06001088 RID: 4232 RVA: 0x000DF8C4 File Offset: 0x000DDAC4
		public static int CountStraightLines(BGCurveBaseMath math, bool[] straight)
		{
			BGCurve curve = math.Curve;
			BGCurvePointI[] points = curve.Points;
			if (points.Length == 0)
			{
				return 0;
			}
			int count = math.SectionInfos.Count;
			bool flag = straight != null;
			int num = 0;
			bool flag2 = points[0].ControlType == BGCurvePoint.ControlTypeEnum.Absent;
			for (int i = 0; i < count; i++)
			{
				bool flag3 = ((curve.Closed && i == count - 1) ? points[0] : points[i + 1]).ControlType == BGCurvePoint.ControlTypeEnum.Absent;
				if (flag2 && flag3)
				{
					if (flag)
					{
						straight[i] = true;
					}
					num++;
				}
				else if (flag)
				{
					straight[i] = false;
				}
				flag2 = flag3;
			}
			return num;
		}

		// Token: 0x06001089 RID: 4233 RVA: 0x000DF960 File Offset: 0x000DDB60
		private BGPolylineSplitter.PositionsProvider InitProvider(ref BGPolylineSplitter.PositionsProvider positionsProvider, BGCcMath math, BGPolylineSplitter.Config config)
		{
			BGCcSplitterPolyline.SplitModeEnum splitMode = config.SplitMode;
			bool flag = positionsProvider == null || !positionsProvider.Comply(splitMode);
			if (splitMode != BGCcSplitterPolyline.SplitModeEnum.PartsTotal)
			{
				if (splitMode != BGCcSplitterPolyline.SplitModeEnum.PartsPerSection)
				{
					if (flag)
					{
						if (this.providerMath == null)
						{
							this.providerMath = new BGPolylineSplitter.PositionsProviderMath();
						}
						positionsProvider = this.providerMath;
					}
					this.providerMath.Init(math);
				}
				else
				{
					if (flag)
					{
						if (this.providerPartsPerSection == null)
						{
							this.providerPartsPerSection = new BGPolylineSplitter.PositionsProviderPartsPerSection();
						}
						positionsProvider = this.providerPartsPerSection;
					}
					this.providerPartsPerSection.Init(math, config.PartsPerSection);
				}
			}
			else
			{
				if (flag)
				{
					if (this.providerTotalParts == null)
					{
						this.providerTotalParts = new BGPolylineSplitter.PositionsProviderTotalParts();
					}
					positionsProvider = this.providerTotalParts;
				}
				this.providerTotalParts.Init(math, config.PartsTotal);
			}
			if (config.DistanceMin > 0f || config.DistanceMax > 0f)
			{
				if (splitMode != BGCcSplitterPolyline.SplitModeEnum.UseMathData)
				{
					throw new Exception("DistanceMin and DistanceMax supported by SplitModeEnum.UseMathData mode only");
				}
				positionsProvider.DistanceMin = config.DistanceMin;
				positionsProvider.DistanceMax = config.DistanceMax;
			}
			return positionsProvider;
		}

		// Token: 0x04000DCE RID: 3534
		private BGPolylineSplitter.PositionsProvider positionsProvider;

		// Token: 0x04000DCF RID: 3535
		private bool[] straightBits;

		// Token: 0x04000DD0 RID: 3536
		private BGPolylineSplitter.PositionsProviderMath providerMath;

		// Token: 0x04000DD1 RID: 3537
		private BGPolylineSplitter.PositionsProviderPartsPerSection providerPartsPerSection;

		// Token: 0x04000DD2 RID: 3538
		private BGPolylineSplitter.PositionsProviderTotalParts providerTotalParts;

		// Token: 0x020001CE RID: 462
		public class Config
		{
			// Token: 0x04000DD3 RID: 3539
			public bool DoNotOptimizeStraightLines;

			// Token: 0x04000DD4 RID: 3540
			public BGCcSplitterPolyline.SplitModeEnum SplitMode;

			// Token: 0x04000DD5 RID: 3541
			public int PartsTotal;

			// Token: 0x04000DD6 RID: 3542
			public int PartsPerSection;

			// Token: 0x04000DD7 RID: 3543
			public bool UseLocal;

			// Token: 0x04000DD8 RID: 3544
			public Transform Transform;

			// Token: 0x04000DD9 RID: 3545
			public float DistanceMin;

			// Token: 0x04000DDA RID: 3546
			public float DistanceMax;
		}

		// Token: 0x020001CF RID: 463
		public abstract class PositionsProvider
		{
			// Token: 0x1700048E RID: 1166
			// (get) Token: 0x0600108C RID: 4236 RVA: 0x0004DD28 File Offset: 0x0004BF28
			// (set) Token: 0x0600108D RID: 4237 RVA: 0x0004DD30 File Offset: 0x0004BF30
			public float DistanceMin
			{
				get
				{
					return this.distanceMin;
				}
				set
				{
					this.distanceMin = value;
					this.DistanceMinConstrained = (value > 0f);
				}
			}

			// Token: 0x1700048F RID: 1167
			// (get) Token: 0x0600108E RID: 4238 RVA: 0x0004DD47 File Offset: 0x0004BF47
			// (set) Token: 0x0600108F RID: 4239 RVA: 0x0004DD4F File Offset: 0x0004BF4F
			public float DistanceMax
			{
				get
				{
					return this.distanceMax;
				}
				set
				{
					this.distanceMax = value;
					this.DistanceMaxConstrained = (value > 0f);
				}
			}

			// Token: 0x06001090 RID: 4240 RVA: 0x0004DD66 File Offset: 0x0004BF66
			public virtual void Init(BGCcMath math)
			{
				this.Math = math;
				this.LastPointAdded = false;
				this.calculatingTangents = this.Math.IsCalculated(BGCurveBaseMath.Field.Tangent);
			}

			// Token: 0x06001091 RID: 4241
			public abstract bool Comply(BGCcSplitterPolyline.SplitModeEnum splitMode);

			// Token: 0x06001092 RID: 4242 RVA: 0x000DFA64 File Offset: 0x000DDC64
			public virtual void Build(List<Vector3> positions, int straightLinesCount, bool[] straightBits, List<BGCcSplitterPolyline.PolylinePoint> points)
			{
				BGCurveBaseMath math = this.Math.Math;
				List<BGCurveBaseMath.SectionInfo> sectionInfos = math.SectionInfos;
				int count = sectionInfos.Count;
				if (!this.DistanceMinConstrained)
				{
					BGCurveBaseMath.SectionPointInfo sectionPointInfo = math[0][0];
					positions.Add(sectionPointInfo.Position);
					points.Add(new BGCcSplitterPolyline.PolylinePoint(sectionPointInfo.Position, 0f, sectionPointInfo.Tangent));
				}
				for (int i = 0; i < count; i++)
				{
					BGCurveBaseMath.SectionInfo sectionInfo = sectionInfos[i];
					if ((!this.DistanceMinConstrained || sectionInfo.DistanceFromEndToOrigin >= this.DistanceMin) && (!this.DistanceMaxConstrained || sectionInfo.DistanceFromStartToOrigin <= this.DistanceMax))
					{
						if (straightLinesCount != 0 && straightBits[i])
						{
							BGCurveBaseMath.SectionPointInfo sectionPointInfo2 = sectionInfo[sectionInfo.PointsCount - 1];
							BGCurveBaseMath.SectionPointInfo previousPoint = sectionInfo[sectionInfo.PointsCount - 2];
							if (this.DistanceMinConstrained && positions.Count == 0)
							{
								this.AddFirstPointIfNeeded(positions, sectionInfo, sectionPointInfo2, previousPoint, points);
							}
							if (this.DistanceMaxConstrained && !this.LastPointAdded && sectionPointInfo2.DistanceToSectionStart + sectionInfo.DistanceFromStartToOrigin > this.DistanceMax && this.AddLastPointIfNeeded(positions, sectionInfo, sectionPointInfo2, previousPoint, points))
							{
								break;
							}
							positions.Add(sectionPointInfo2.Position);
							points.Add(new BGCcSplitterPolyline.PolylinePoint(sectionPointInfo2.Position, sectionInfo.DistanceFromEndToOrigin, sectionPointInfo2.Tangent));
						}
						else
						{
							this.FillInSplitSection(sectionInfo, positions, points);
						}
					}
				}
			}

			// Token: 0x06001093 RID: 4243 RVA: 0x000DFBE4 File Offset: 0x000DDDE4
			protected void AddFirstPointIfNeeded(List<Vector3> positions, BGCurveBaseMath.SectionInfo section, BGCurveBaseMath.SectionPointInfo firstPointInRange, BGCurveBaseMath.SectionPointInfo previousPoint, List<BGCcSplitterPolyline.PolylinePoint> points)
			{
				float num = firstPointInRange.DistanceToSectionStart + section.DistanceFromStartToOrigin - this.DistanceMin;
				if (num <= 1E-05f)
				{
					return;
				}
				float ratio = 1f - num / (firstPointInRange.DistanceToSectionStart - previousPoint.DistanceToSectionStart);
				this.Add(section, positions, points, previousPoint, firstPointInRange, ratio);
			}

			// Token: 0x06001094 RID: 4244 RVA: 0x000DFC34 File Offset: 0x000DDE34
			protected bool AddLastPointIfNeeded(List<Vector3> positions, BGCurveBaseMath.SectionInfo section, BGCurveBaseMath.SectionPointInfo currentPoint, BGCurveBaseMath.SectionPointInfo previousPoint, List<BGCcSplitterPolyline.PolylinePoint> points)
			{
				float num = currentPoint.DistanceToSectionStart + section.DistanceFromStartToOrigin;
				if (num <= this.DistanceMax)
				{
					return false;
				}
				float num2 = Vector3.SqrMagnitude(positions[positions.Count - 1] - currentPoint.Position);
				float num3 = Vector3.SqrMagnitude(previousPoint.Position - currentPoint.Position);
				if (num2 > num3)
				{
					float num4 = num - this.DistanceMax;
					this.LastPointAdded = true;
					float ratio = num4 / (currentPoint.DistanceToSectionStart - previousPoint.DistanceToSectionStart);
					this.Add(section, positions, points, previousPoint, currentPoint, ratio);
				}
				else
				{
					float num5 = Mathf.Sqrt(num2);
					float num6 = num - this.DistanceMax;
					float ratio2 = 1f - num6 / num5;
					this.Add(section, positions, points, points[positions.Count - 1], currentPoint, ratio2);
				}
				return true;
			}

			// Token: 0x06001095 RID: 4245 RVA: 0x000DFD00 File Offset: 0x000DDF00
			private void Add(BGCurveBaseMath.SectionInfo section, List<Vector3> positions, List<BGCcSplitterPolyline.PolylinePoint> points, BGCcSplitterPolyline.PolylinePoint previousPoint, BGCurveBaseMath.SectionPointInfo nextPoint, float ratio)
			{
				Vector3 vector = Vector3.Lerp(previousPoint.Position, nextPoint.Position, ratio);
				positions.Add(vector);
				points.Add(new BGCcSplitterPolyline.PolylinePoint(vector, Mathf.Lerp(previousPoint.Distance, section.DistanceFromStartToOrigin + nextPoint.DistanceToSectionStart, ratio), this.calculatingTangents ? Vector3.Lerp(previousPoint.Tangent, nextPoint.Tangent, ratio) : Vector3.zero));
			}

			// Token: 0x06001096 RID: 4246 RVA: 0x000DFD78 File Offset: 0x000DDF78
			private void Add(BGCurveBaseMath.SectionInfo section, List<Vector3> positions, List<BGCcSplitterPolyline.PolylinePoint> points, BGCurveBaseMath.SectionPointInfo previousPoint, BGCurveBaseMath.SectionPointInfo nextPoint, float ratio)
			{
				Vector3 vector = Vector3.Lerp(previousPoint.Position, nextPoint.Position, ratio);
				positions.Add(vector);
				points.Add(new BGCcSplitterPolyline.PolylinePoint(vector, section.DistanceFromStartToOrigin + Mathf.Lerp(previousPoint.DistanceToSectionStart, nextPoint.DistanceToSectionStart, ratio), this.calculatingTangents ? Vector3.Lerp(previousPoint.Tangent, nextPoint.Tangent, ratio) : Vector3.zero));
			}

			// Token: 0x06001097 RID: 4247 RVA: 0x000DFDF0 File Offset: 0x000DDFF0
			protected void FillIn(BGCurveBaseMath.SectionInfo section, List<Vector3> result, int parts, List<BGCcSplitterPolyline.PolylinePoint> points)
			{
				float num = section.Distance / (float)parts;
				for (int i = 1; i <= parts; i++)
				{
					float num2 = num * (float)i;
					Vector3 vector;
					Vector3 tangent;
					section.CalcByDistance(num2, out vector, out tangent, true, this.calculatingTangents);
					result.Add(vector);
					points.Add(new BGCcSplitterPolyline.PolylinePoint(vector, section.DistanceFromStartToOrigin + num2, tangent));
				}
			}

			// Token: 0x06001098 RID: 4248
			protected abstract void FillInSplitSection(BGCurveBaseMath.SectionInfo section, List<Vector3> result, List<BGCcSplitterPolyline.PolylinePoint> points);

			// Token: 0x04000DDB RID: 3547
			protected BGCcMath Math;

			// Token: 0x04000DDC RID: 3548
			private float distanceMin = -1f;

			// Token: 0x04000DDD RID: 3549
			private float distanceMax = -1f;

			// Token: 0x04000DDE RID: 3550
			protected bool LastPointAdded;

			// Token: 0x04000DDF RID: 3551
			protected bool DistanceMinConstrained;

			// Token: 0x04000DE0 RID: 3552
			protected bool DistanceMaxConstrained;

			// Token: 0x04000DE1 RID: 3553
			protected bool calculatingTangents;
		}

		// Token: 0x020001D0 RID: 464
		public sealed class PositionsProviderTotalParts : BGPolylineSplitter.PositionsProvider
		{
			// Token: 0x0600109A RID: 4250 RVA: 0x0004DDA6 File Offset: 0x0004BFA6
			public void Init(BGCcMath math, int parts)
			{
				base.Init(math);
				this.parts = parts;
			}

			// Token: 0x0600109B RID: 4251 RVA: 0x0004DDB6 File Offset: 0x0004BFB6
			public override bool Comply(BGCcSplitterPolyline.SplitModeEnum splitMode)
			{
				return splitMode == BGCcSplitterPolyline.SplitModeEnum.PartsTotal;
			}

			// Token: 0x0600109C RID: 4252 RVA: 0x000DFE4C File Offset: 0x000DE04C
			public override void Build(List<Vector3> positions, int straightLinesCount, bool[] straightBits, List<BGCcSplitterPolyline.PolylinePoint> points)
			{
				BGCurve curve = this.Math.Curve;
				List<BGCurveBaseMath.SectionInfo> sectionInfos = this.Math.Math.SectionInfos;
				int count = sectionInfos.Count;
				float f = (float)(this.parts - straightLinesCount) / (float)(count - straightLinesCount);
				this.reminderForCurved = (int)((float)(this.parts - straightLinesCount) % (float)(count - straightLinesCount));
				this.partsPerSectionFloor = Mathf.FloorToInt(f);
				if (this.parts >= count)
				{
					base.Build(positions, straightLinesCount, straightBits, points);
					return;
				}
				float distance = this.Math.GetDistance(-1);
				if (this.parts == 1)
				{
					BGCurveBaseMath.SectionPointInfo sectionPointInfo = sectionInfos[0][0];
					positions.Add(sectionPointInfo.Position);
					points.Add(new BGCcSplitterPolyline.PolylinePoint(sectionPointInfo.Position, 0f, sectionPointInfo.Tangent));
					if (curve.Closed)
					{
						Vector3 tangent;
						Vector3 vector = this.Math.CalcByDistanceRatio(0.5f, out tangent, false);
						positions.Add(vector);
						points.Add(new BGCcSplitterPolyline.PolylinePoint(vector, distance * 0.5f, tangent));
						return;
					}
					BGCurveBaseMath.SectionInfo sectionInfo = sectionInfos[count - 1];
					BGCurveBaseMath.SectionPointInfo sectionPointInfo2 = sectionInfo[sectionInfo.PointsCount - 1];
					positions.Add(sectionPointInfo2.Position);
					points.Add(new BGCcSplitterPolyline.PolylinePoint(sectionPointInfo2.Position, sectionInfo.DistanceFromEndToOrigin, sectionPointInfo2.Tangent));
					return;
				}
				else
				{
					if (this.parts == 2 && curve.Closed)
					{
						BGCurveBaseMath.SectionPointInfo sectionPointInfo3 = sectionInfos[0][0];
						positions.Add(sectionPointInfo3.Position);
						points.Add(new BGCcSplitterPolyline.PolylinePoint(sectionPointInfo3.Position, 0f, sectionPointInfo3.Tangent));
						float num = 0.33333334f;
						Vector3 tangent2;
						Vector3 vector2 = this.Math.CalcByDistanceRatio(num, out tangent2, false);
						positions.Add(vector2);
						points.Add(new BGCcSplitterPolyline.PolylinePoint(vector2, distance * num, tangent2));
						float num2 = 0.6666667f;
						Vector3 tangent3;
						Vector3 vector3 = this.Math.CalcByDistanceRatio(num2, out tangent3, false);
						positions.Add(vector3);
						points.Add(new BGCcSplitterPolyline.PolylinePoint(vector3, distance * num2, tangent3));
						return;
					}
					for (int i = 0; i <= this.parts; i++)
					{
						float num3 = (float)i / (float)this.parts;
						Vector3 tangent4;
						Vector3 vector4 = this.Math.CalcByDistanceRatio(num3, out tangent4, false);
						positions.Add(vector4);
						points.Add(new BGCcSplitterPolyline.PolylinePoint(vector4, distance * num3, tangent4));
					}
					return;
				}
			}

			// Token: 0x0600109D RID: 4253 RVA: 0x000E00B4 File Offset: 0x000DE2B4
			protected override void FillInSplitSection(BGCurveBaseMath.SectionInfo section, List<Vector3> result, List<BGCcSplitterPolyline.PolylinePoint> points)
			{
				int num = this.partsPerSectionFloor;
				if (this.reminderForCurved > 0)
				{
					num++;
					this.reminderForCurved--;
				}
				base.FillIn(section, result, num, points);
			}

			// Token: 0x04000DE2 RID: 3554
			private int parts;

			// Token: 0x04000DE3 RID: 3555
			private int reminderForCurved;

			// Token: 0x04000DE4 RID: 3556
			private int partsPerSectionFloor;
		}

		// Token: 0x020001D1 RID: 465
		public sealed class PositionsProviderPartsPerSection : BGPolylineSplitter.PositionsProvider
		{
			// Token: 0x0600109F RID: 4255 RVA: 0x0004DDC4 File Offset: 0x0004BFC4
			public void Init(BGCcMath math, int partsPerSection)
			{
				base.Init(math);
				this.parts = partsPerSection;
			}

			// Token: 0x060010A0 RID: 4256 RVA: 0x0004DDD4 File Offset: 0x0004BFD4
			public override bool Comply(BGCcSplitterPolyline.SplitModeEnum splitMode)
			{
				return splitMode == BGCcSplitterPolyline.SplitModeEnum.PartsPerSection;
			}

			// Token: 0x060010A1 RID: 4257 RVA: 0x0004DDDA File Offset: 0x0004BFDA
			protected override void FillInSplitSection(BGCurveBaseMath.SectionInfo section, List<Vector3> result, List<BGCcSplitterPolyline.PolylinePoint> points)
			{
				base.FillIn(section, result, this.parts, points);
			}

			// Token: 0x04000DE5 RID: 3557
			private int parts;
		}

		// Token: 0x020001D2 RID: 466
		public sealed class PositionsProviderMath : BGPolylineSplitter.PositionsProvider
		{
			// Token: 0x060010A3 RID: 4259 RVA: 0x0004DDEB File Offset: 0x0004BFEB
			public override bool Comply(BGCcSplitterPolyline.SplitModeEnum splitMode)
			{
				return splitMode == BGCcSplitterPolyline.SplitModeEnum.UseMathData;
			}

			// Token: 0x060010A4 RID: 4260 RVA: 0x000E00F0 File Offset: 0x000DE2F0
			protected override void FillInSplitSection(BGCurveBaseMath.SectionInfo section, List<Vector3> result, List<BGCcSplitterPolyline.PolylinePoint> points)
			{
				if (this.LastPointAdded)
				{
					return;
				}
				List<BGCurveBaseMath.SectionPointInfo> points2 = section.Points;
				int count = points2.Count;
				for (int i = 1; i < count; i++)
				{
					BGCurveBaseMath.SectionPointInfo sectionPointInfo = points2[i];
					if (this.DistanceMinConstrained && result.Count == 0)
					{
						base.AddFirstPointIfNeeded(result, section, sectionPointInfo, points2[i - 1], points);
					}
					if (this.DistanceMaxConstrained && base.AddLastPointIfNeeded(result, section, sectionPointInfo, points2[i - 1], points))
					{
						break;
					}
					result.Add(sectionPointInfo.Position);
					points.Add(new BGCcSplitterPolyline.PolylinePoint(sectionPointInfo.Position, section.DistanceFromStartToOrigin + sectionPointInfo.DistanceToSectionStart, this.calculatingTangents ? sectionPointInfo.Tangent : Vector3.zero));
				}
			}
		}
	}
}
