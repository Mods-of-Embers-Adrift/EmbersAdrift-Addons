using System;
using UnityEngine;

namespace BansheeGz.BGSpline.Curve
{
	// Token: 0x02000193 RID: 403
	[Obsolete("Use BGCurveBaseMath. This class is for testing purpose only")]
	public class BGCurveFormulaMath : BGCurveBaseMath
	{
		// Token: 0x06000DA2 RID: 3490 RVA: 0x0004B854 File Offset: 0x00049A54
		public BGCurveFormulaMath(BGCurve curve, BGCurveBaseMath.Config config) : base(curve, config)
		{
		}

		// Token: 0x06000DA3 RID: 3491 RVA: 0x000D8210 File Offset: 0x000D6410
		protected override void AfterInit(BGCurveBaseMath.Config config)
		{
			int parts = config.Parts;
			int newSize = parts + 1;
			Array.Resize<float>(ref this.bakedT, newSize);
			Array.Resize<float>(ref this.bakedT2, newSize);
			Array.Resize<float>(ref this.bakedTr2, newSize);
			Array.Resize<float>(ref this.bakedT3, newSize);
			Array.Resize<float>(ref this.bakedTr3, newSize);
			Array.Resize<float>(ref this.bakedTr2xTx3, newSize);
			Array.Resize<float>(ref this.bakedT2xTrx3, newSize);
			Array.Resize<float>(ref this.bakedTxTrx2, newSize);
			if (base.NeedTangentFormula)
			{
				Array.Resize<float>(ref this.bakedTr2x3, newSize);
				Array.Resize<float>(ref this.bakedTxTrx6, newSize);
				Array.Resize<float>(ref this.bakedT2x3, newSize);
				Array.Resize<float>(ref this.bakedTx2, newSize);
				Array.Resize<float>(ref this.bakedTrx2, newSize);
			}
			for (int i = 0; i <= parts; i++)
			{
				float num = (float)i / (float)parts;
				float num2 = 1f - num;
				float num3 = num * num;
				float num4 = num2 * num2;
				this.bakedT[i] = num;
				this.bakedT2[i] = num3;
				this.bakedTr2[i] = num4;
				this.bakedT3[i] = num3 * num;
				this.bakedTr3[i] = num4 * num2;
				this.bakedTr2xTx3[i] = 3f * num4 * num;
				this.bakedT2xTrx3[i] = 3f * num2 * num3;
				this.bakedTxTrx2[i] = 2f * num2 * num;
				if (base.NeedTangentFormula)
				{
					this.bakedTr2x3[i] = 3f * num4;
					this.bakedTxTrx6[i] = 6f * num2 * num;
					this.bakedT2x3[i] = 3f * num3;
					this.bakedTx2[i] = 2f * num;
					this.bakedTrx2[i] = 2f * num2;
				}
			}
		}

		// Token: 0x06000DA4 RID: 3492 RVA: 0x000D83C0 File Offset: 0x000D65C0
		public override void Dispose()
		{
			base.Dispose();
			float[] array = new float[0];
			this.bakedT = array;
			this.bakedT2 = array;
			this.bakedTr2 = array;
			this.bakedT3 = array;
			this.bakedTr3 = array;
			this.bakedTr2xTx3 = array;
			this.bakedT2xTrx3 = array;
			this.bakedTxTrx2 = array;
			this.bakedTr2x3 = array;
			this.bakedTxTrx6 = array;
			this.bakedT2x3 = array;
			this.bakedTx2 = array;
			this.bakedTrx2 = array;
		}

		// Token: 0x06000DA5 RID: 3493 RVA: 0x000D8438 File Offset: 0x000D6638
		protected override void CalculateSplitSection(BGCurveBaseMath.SectionInfo section, BGCurvePointI from, BGCurvePointI to)
		{
			base.Resize(section.points, this.config.Parts + 1);
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
			Vector3 vector2 = Vector3.zero;
			Vector3 tangent = Vector3.zero;
			if (flag3)
			{
				vector2 = originalTo - originalFrom;
				if (this.cacheTangent)
				{
					tangent = (to.PositionWorld - from.PositionWorld).normalized;
				}
			}
			Vector3 vector3 = Vector3.zero;
			Vector3 vector4 = Vector3.zero;
			Vector3 vector5 = Vector3.zero;
			Vector3 vector6 = Vector3.zero;
			if (!this.config.UsePointPositionsToCalcTangents && this.cacheTangent)
			{
				vector3 = vector - originalFrom;
				if (flag4)
				{
					vector4 = originalToControl - vector;
					vector5 = originalTo - originalToControl;
				}
				else
				{
					vector6 = originalTo - vector;
				}
			}
			int num = this.bakedT.Length;
			for (int i = 0; i < num; i++)
			{
				BGCurveBaseMath.SectionPointInfo sectionPointInfo;
				if ((sectionPointInfo = section.points[i]) == null)
				{
					sectionPointInfo = (section.points[i] = new BGCurveBaseMath.SectionPointInfo());
				}
				BGCurveBaseMath.SectionPointInfo sectionPointInfo2 = sectionPointInfo;
				Vector3 vector7;
				if (flag3)
				{
					float num2 = this.bakedT[i];
					vector7 = new Vector3(originalFrom.x + vector2.x * num2, originalFrom.y + vector2.y * num2, originalFrom.z + vector2.z * num2);
					if (flag5)
					{
						this.curve.ApplySnapping(ref vector7);
					}
					sectionPointInfo2.Position = vector7;
					if (this.cacheTangent)
					{
						sectionPointInfo2.Tangent = tangent;
					}
				}
				else
				{
					if (flag4)
					{
						float num3 = this.bakedTr3[i];
						float num4 = this.bakedTr2xTx3[i];
						float num5 = this.bakedT2xTrx3[i];
						float num6 = this.bakedT3[i];
						vector7 = new Vector3(num3 * originalFrom.x + num4 * vector.x + num5 * originalToControl.x + num6 * originalTo.x, num3 * originalFrom.y + num4 * vector.y + num5 * originalToControl.y + num6 * originalTo.y, num3 * originalFrom.z + num4 * vector.z + num5 * originalToControl.z + num6 * originalTo.z);
					}
					else
					{
						float num7 = this.bakedTr2[i];
						float num8 = this.bakedTxTrx2[i];
						float num9 = this.bakedT2[i];
						vector7 = new Vector3(num7 * originalFrom.x + num8 * vector.x + num9 * originalTo.x, num7 * originalFrom.y + num8 * vector.y + num9 * originalTo.y, num7 * originalFrom.z + num8 * vector.z + num9 * originalTo.z);
					}
					if (flag5)
					{
						this.curve.ApplySnapping(ref vector7);
					}
					sectionPointInfo2.Position = vector7;
					if (this.cacheTangent)
					{
						if (this.config.UsePointPositionsToCalcTangents)
						{
							if (i != 0)
							{
								BGCurveBaseMath.SectionPointInfo sectionPointInfo3 = section[i - 1];
								Vector3 position = sectionPointInfo3.Position;
								Vector3 vector8 = new Vector3(vector7.x - position.x, vector7.y - position.y, vector7.z - position.z);
								float num10 = (float)Math.Sqrt((double)vector8.x * (double)vector8.x + (double)vector8.y * (double)vector8.y + (double)vector8.z * (double)vector8.z);
								vector8 = (((double)num10 > 9.99999974737875E-06) ? new Vector3(vector8.x / num10, vector8.y / num10, vector8.z / num10) : Vector3.zero);
								sectionPointInfo3.Tangent = vector8;
								if (i == this.config.Parts)
								{
									sectionPointInfo2.Tangent = sectionPointInfo3.Tangent;
								}
							}
						}
						else
						{
							Vector3 vector9;
							if (flag4)
							{
								float num11 = this.bakedTr2x3[i];
								float num12 = this.bakedTxTrx6[i];
								float num13 = this.bakedT2x3[i];
								vector9 = new Vector3(num11 * vector3.x + num12 * vector4.x + num13 * vector5.x, num11 * vector3.y + num12 * vector4.y + num13 * vector5.y, num11 * vector3.z + num12 * vector4.z + num13 * vector5.z);
							}
							else
							{
								float num14 = this.bakedTrx2[i];
								float num15 = this.bakedTx2[i];
								vector9 = new Vector3(num14 * vector3.x + num15 * vector6.x, num14 * vector3.y + num15 * vector6.y, num14 * vector3.z + num15 * vector6.z);
							}
							float num16 = (float)Math.Sqrt((double)vector9.x * (double)vector9.x + (double)vector9.y * (double)vector9.y + (double)vector9.z * (double)vector9.z);
							vector9 = (((double)num16 > 9.99999974737875E-06) ? new Vector3(vector9.x / num16, vector9.y / num16, vector9.z / num16) : Vector3.zero);
							sectionPointInfo2.Tangent = vector9;
						}
					}
				}
				if (i != 0)
				{
					Vector3 position2 = section[i - 1].Position;
					double num17 = (double)(vector7.x - position2.x);
					double num18 = (double)(vector7.y - position2.y);
					double num19 = (double)(vector7.z - position2.z);
					sectionPointInfo2.DistanceToSectionStart = section[i - 1].DistanceToSectionStart + (float)Math.Sqrt(num17 * num17 + num18 * num18 + num19 * num19);
				}
			}
		}

		// Token: 0x04000CAD RID: 3245
		private float[] bakedT;

		// Token: 0x04000CAE RID: 3246
		private float[] bakedT2;

		// Token: 0x04000CAF RID: 3247
		private float[] bakedTr2;

		// Token: 0x04000CB0 RID: 3248
		private float[] bakedT3;

		// Token: 0x04000CB1 RID: 3249
		private float[] bakedTr3;

		// Token: 0x04000CB2 RID: 3250
		private float[] bakedTr2xTx3;

		// Token: 0x04000CB3 RID: 3251
		private float[] bakedT2xTrx3;

		// Token: 0x04000CB4 RID: 3252
		private float[] bakedTxTrx2;

		// Token: 0x04000CB5 RID: 3253
		private float[] bakedTr2x3;

		// Token: 0x04000CB6 RID: 3254
		private float[] bakedTxTrx6;

		// Token: 0x04000CB7 RID: 3255
		private float[] bakedT2x3;

		// Token: 0x04000CB8 RID: 3256
		private float[] bakedTx2;

		// Token: 0x04000CB9 RID: 3257
		private float[] bakedTrx2;
	}
}
