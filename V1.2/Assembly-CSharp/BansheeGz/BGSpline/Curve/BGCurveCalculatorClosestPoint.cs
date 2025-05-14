using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGSpline.Curve
{
	// Token: 0x0200018F RID: 399
	public class BGCurveCalculatorClosestPoint
	{
		// Token: 0x06000D8F RID: 3471 RVA: 0x0004BC47 File Offset: 0x00049E47
		public BGCurveCalculatorClosestPoint(BGCurveBaseMath math)
		{
			this.math = math;
		}

		// Token: 0x06000D90 RID: 3472 RVA: 0x000D7570 File Offset: 0x000D5770
		public Vector3 CalcPositionByClosestPoint(Vector3 targetPoint, out float distance, out Vector3 tangent, bool skipSectionsOptimization = false, bool skipPointsOptimization = false)
		{
			List<BGCurveBaseMath.SectionInfo> sectionInfos = this.math.SectionInfos;
			int count = sectionInfos.Count;
			if (count != 0)
			{
				bool flag = !skipSectionsOptimization && this.math.Configuration.Parts > 8;
				int num = 0;
				float num2 = float.MaxValue;
				if (flag)
				{
					Array.Resize<bool>(ref this.excludedSections, count);
					Array.Resize<float>(ref this.minSectionDistances, count);
					float num3 = float.MaxValue;
					for (int i = 0; i < count; i++)
					{
						float num4 = this.math.GetBoundingBox(i, sectionInfos[i]).SqrDistance(targetPoint);
						this.excludedSections[i] = false;
						this.minSectionDistances[i] = num4;
						if (num3 > num4)
						{
							num3 = num4;
							num = i;
						}
					}
					num2 = BGCurveCalculatorClosestPoint.MaxDistance(sectionInfos[num], targetPoint) - 1E-05f;
					int num5 = 0;
					bool flag2 = false;
					int num6 = num;
					int num7 = Mathf.Max(num6 + 1, count - num6);
					for (int j = 1; j < num7; j++)
					{
						int num8 = num6 + j;
						if (num8 < count)
						{
							BGCurveBaseMath.SectionInfo section = sectionInfos[num8];
							if (this.minSectionDistances[num8] > num2)
							{
								this.excludedSections[num8] = true;
							}
							else
							{
								float num9 = BGCurveCalculatorClosestPoint.MaxDistance(section, targetPoint);
								if (num2 > num9)
								{
									num2 = num9;
									flag2 = true;
								}
								num5++;
							}
						}
						num8 = num6 - j;
						if (num8 >= 0)
						{
							BGCurveBaseMath.SectionInfo section2 = sectionInfos[num8];
							if (this.minSectionDistances[num8] > num2)
							{
								this.excludedSections[num8] = true;
							}
							else
							{
								float num10 = BGCurveCalculatorClosestPoint.MaxDistance(section2, targetPoint);
								if (num2 > num10)
								{
									num2 = num10;
									flag2 = true;
								}
								num5++;
							}
						}
					}
					if (flag2 && num5 > 1)
					{
						for (int k = 0; k < count; k++)
						{
							if (!this.excludedSections[k] && this.minSectionDistances[k] > num2)
							{
								this.excludedSections[k] = true;
							}
						}
					}
				}
				bool flag3 = !skipPointsOptimization;
				float num11 = float.MaxValue;
				float num12 = float.MaxValue;
				bool flag4 = true;
				bool flag5 = true;
				if (flag3)
				{
					for (int l = 0; l < count; l++)
					{
						if (flag && this.excludedSections[l])
						{
							flag5 = true;
						}
						else
						{
							BGCurveBaseMath.SectionInfo sectionInfo = sectionInfos[l];
							List<BGCurveBaseMath.SectionPointInfo> points = sectionInfo.Points;
							int count2 = points.Count;
							if (flag5)
							{
								flag5 = false;
								num12 = Vector3.SqrMagnitude(sectionInfo[0].Position - targetPoint);
								flag4 = (num12 <= num11);
							}
							for (int m = 1; m < count2; m++)
							{
								Vector3 position = points[m].Position;
								float num13 = position.x - targetPoint.x;
								float num14 = position.y - targetPoint.y;
								float num15 = position.z - targetPoint.z;
								float num16 = num13 * num13 + num14 * num14 + num15 * num15;
								bool flag6 = num16 <= num11;
								if (flag4 && flag6)
								{
									num11 = ((num12 > num16) ? num12 : num16);
								}
								flag4 = flag6;
								num12 = num16;
							}
						}
					}
					if (flag && num11 < num2)
					{
						for (int n = 0; n < count; n++)
						{
							if (!this.excludedSections[n] && this.minSectionDistances[n] > num11)
							{
								this.excludedSections[n] = true;
							}
						}
					}
				}
				BGCurveBaseMath.SectionPointInfo sectionPointInfo = sectionInfos[0][0];
				Vector3 position2 = sectionPointInfo.Position;
				float num17 = Vector3.SqrMagnitude(targetPoint - sectionPointInfo.Position);
				int index = 0;
				int num18 = 0;
				float num19 = 0f;
				int num20 = -1;
				bool flag7 = true;
				Vector3 zero = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				if (flag3)
				{
					float num21 = (float)Math.Sqrt((double)num11);
					zero = new Vector3(targetPoint.x - num21, targetPoint.y - num21, targetPoint.z - num21);
					zero2 = new Vector3(targetPoint.x + num21, targetPoint.y + num21, targetPoint.z + num21);
				}
				for (int num22 = 0; num22 < count; num22++)
				{
					if (flag && this.excludedSections[num22])
					{
						flag7 = true;
					}
					else
					{
						BGCurveBaseMath.SectionInfo sectionInfo2 = sectionInfos[num22];
						List<BGCurveBaseMath.SectionPointInfo> points2 = sectionInfo2.Points;
						if (flag7)
						{
							flag7 = false;
							sectionPointInfo = sectionInfo2[0];
							if (flag3)
							{
								Vector3 position3 = sectionPointInfo.Position;
								bool flag8 = false;
								if (position3.x < zero.x)
								{
									if (position3.y < zero.y)
									{
										num20 = 0;
									}
									else if (position3.y > zero2.y)
									{
										num20 = 2;
									}
									else
									{
										num20 = 1;
									}
								}
								else if (position3.x > zero2.x)
								{
									if (position3.y < zero.y)
									{
										num20 = 6;
									}
									else if (position3.y > zero2.y)
									{
										num20 = 4;
									}
									else
									{
										num20 = 5;
									}
								}
								else if (position3.y < zero.y)
								{
									num20 = 7;
								}
								else if (position3.y > zero2.y)
								{
									num20 = 3;
								}
								else
								{
									if (position3.z > zero2.z)
									{
										num20 = 26;
									}
									else if (position3.z < zero.z)
									{
										num20 = 24;
									}
									else
									{
										num20 = 25;
									}
									flag8 = true;
								}
								if (!flag8)
								{
									if (position3.z > zero2.z)
									{
										num20 |= 16;
									}
									else if (position3.z > zero.z)
									{
										num20 |= 8;
									}
								}
							}
						}
						int count3 = points2.Count;
						for (int num23 = 1; num23 < count3; num23++)
						{
							BGCurveBaseMath.SectionPointInfo sectionPointInfo2 = points2[num23];
							Vector3 position4 = sectionPointInfo2.Position;
							bool flag9 = false;
							int num24 = -1;
							if (flag3)
							{
								bool flag10 = false;
								if (position4.x < zero.x)
								{
									if (position4.y < zero.y)
									{
										num24 = 0;
									}
									else if (position4.y > zero2.y)
									{
										num24 = 2;
									}
									else
									{
										num24 = 1;
									}
								}
								else if (position4.x > zero2.x)
								{
									if (position4.y < zero.y)
									{
										num24 = 6;
									}
									else if (position4.y > zero2.y)
									{
										num24 = 4;
									}
									else
									{
										num24 = 5;
									}
								}
								else if (position4.y < zero.y)
								{
									num24 = 7;
								}
								else if (position4.y > zero2.y)
								{
									num24 = 3;
								}
								else
								{
									if (position4.z > zero2.z)
									{
										num24 = 26;
									}
									else if (position4.z < zero.z)
									{
										num24 = 24;
									}
									else
									{
										num24 = 25;
									}
									flag10 = true;
								}
								if (!flag10)
								{
									if (position4.z > zero2.z)
									{
										num24 |= 16;
									}
									else if (position4.z > zero.z)
									{
										num24 |= 8;
									}
								}
								flag9 = ((BGCurveCalculatorClosestPoint.TransitionsForPartitions[num20] & 1 << num24) != 0);
							}
							if (!flag9)
							{
								bool flag11 = false;
								Vector3 position5 = sectionPointInfo.Position;
								Vector3 position6 = sectionPointInfo2.Position;
								double num25 = (double)targetPoint.x - (double)position5.x;
								double num26 = (double)targetPoint.y - (double)position5.y;
								double num27 = (double)targetPoint.z - (double)position5.z;
								double num28 = (double)position6.x - (double)position5.x;
								double num29 = (double)position6.y - (double)position5.y;
								double num30 = (double)position6.z - (double)position5.z;
								float num31 = (float)(num28 * num28 + num29 * num29 + num30 * num30);
								float num32;
								double num33;
								double num34;
								double num35;
								if (Math.Abs(num31) < 1E-05f)
								{
									flag11 = true;
									num32 = 1f;
									num33 = (double)position6.x;
									num34 = (double)position6.y;
									num35 = (double)position6.z;
								}
								else
								{
									float num36 = (float)(num25 * num28 + num26 * num29 + num27 * num30);
									if (num36 < 0f)
									{
										num32 = 0f;
										num33 = (double)position5.x;
										num34 = (double)position5.y;
										num35 = (double)position5.z;
									}
									else if (num36 > num31)
									{
										flag11 = true;
										num32 = 1f;
										num33 = (double)position6.x;
										num34 = (double)position6.y;
										num35 = (double)position6.z;
									}
									else
									{
										float num37 = num36 / num31;
										num32 = num37;
										num33 = (double)position5.x + num28 * (double)num37;
										num34 = (double)position5.y + num29 * (double)num37;
										num35 = (double)position5.z + num30 * (double)num37;
									}
								}
								double num38 = (double)targetPoint.x - num33;
								double num39 = (double)targetPoint.y - num34;
								double num40 = (double)targetPoint.z - num35;
								float num41 = (float)(num38 * num38 + num39 * num39 + num40 * num40);
								if (num41 < num17)
								{
									num17 = num41;
									position2.x = (float)num33;
									position2.y = (float)num34;
									position2.z = (float)num35;
									index = num22;
									if (flag11)
									{
										if (num23 == count3 - 1 && num22 < count - 1)
										{
											index = num22 + 1;
											num18 = 0;
										}
										else
										{
											num18 = num23;
										}
										num19 = 0f;
									}
									else
									{
										num18 = num23 - 1;
										num19 = num32;
									}
								}
							}
							sectionPointInfo = sectionPointInfo2;
							num20 = num24;
						}
					}
				}
				BGCurveBaseMath.SectionInfo sectionInfo3 = sectionInfos[index];
				if (num19 >= 0f && num19 <= 1f && num18 < sectionInfo3.PointsCount - 1)
				{
					BGCurveBaseMath.SectionPointInfo sectionPointInfo3 = sectionInfo3[num18];
					BGCurveBaseMath.SectionPointInfo sectionPointInfo4 = sectionInfo3[num18 + 1];
					tangent = Vector3.Lerp(sectionPointInfo3.Tangent, sectionPointInfo4.Tangent, num19);
					distance = sectionInfos[index].DistanceFromStartToOrigin + Mathf.Lerp(sectionPointInfo3.DistanceToSectionStart, sectionPointInfo4.DistanceToSectionStart, num19);
				}
				else
				{
					BGCurveBaseMath.SectionPointInfo sectionPointInfo5 = sectionInfos[index][num18];
					tangent = sectionPointInfo5.Tangent;
					distance = sectionInfos[index].DistanceFromStartToOrigin + sectionPointInfo5.DistanceToSectionStart;
				}
				return position2;
			}
			distance = 0f;
			tangent = Vector3.zero;
			if (this.math.Curve.PointsCount != 1)
			{
				return Vector3.zero;
			}
			return this.math.Curve[0].PositionWorld;
		}

		// Token: 0x06000D91 RID: 3473 RVA: 0x000D7F54 File Offset: 0x000D6154
		private static float MaxDistance(BGCurveBaseMath.SectionInfo section, Vector3 position)
		{
			float num = section.OriginalFrom.x - position.x;
			float num2 = section.OriginalFrom.y - position.y;
			float num3 = section.OriginalFrom.z - position.z;
			float a = num * num + num2 * num2 + num3 * num3;
			float num4 = section.OriginalTo.x - position.x;
			num2 = section.OriginalTo.y - position.y;
			num3 = section.OriginalTo.z - position.z;
			float b = num4 * num4 + num2 * num2 + num3 * num3;
			bool flag = section.OriginalFromControlType == BGCurvePoint.ControlTypeEnum.Absent;
			bool flag2 = section.OriginalToControlType == BGCurvePoint.ControlTypeEnum.Absent;
			float result;
			if (flag && flag2)
			{
				result = Mathf.Max(a, b);
			}
			else if (flag)
			{
				float num5 = section.OriginalToControl.x - position.x;
				num2 = section.OriginalToControl.y - position.y;
				num3 = section.OriginalToControl.z - position.z;
				float b2 = num5 * num5 + num2 * num2 + num3 * num3;
				result = Mathf.Max(Mathf.Max(a, b), b2);
			}
			else if (flag2)
			{
				float num6 = section.OriginalFromControl.x - position.x;
				num2 = section.OriginalFromControl.y - position.y;
				num3 = section.OriginalFromControl.z - position.z;
				float b3 = num6 * num6 + num2 * num2 + num3 * num3;
				result = Mathf.Max(Mathf.Max(a, b), b3);
			}
			else
			{
				float num7 = section.OriginalFromControl.x - position.x;
				num2 = section.OriginalFromControl.y - position.y;
				num3 = section.OriginalFromControl.z - position.z;
				float b4 = num7 * num7 + num2 * num2 + num3 * num3;
				float num8 = section.OriginalToControl.x - position.x;
				num2 = section.OriginalToControl.y - position.y;
				num3 = section.OriginalToControl.z - position.z;
				float b5 = num8 * num8 + num2 * num2 + num3 * num3;
				result = Mathf.Max(Mathf.Max(Mathf.Max(a, b), b4), b5);
			}
			return result;
		}

		// Token: 0x04000C99 RID: 3225
		private static readonly int[] TransitionsForPartitions = new int[]
		{
			29870079,
			17238015,
			18817023,
			18619647,
			24935679,
			24146175,
			32633343,
			29475327,
			13092807,
			460551,
			2039583,
			1842204,
			8158332,
			7368816,
			15856113,
			12698049,
			83871687,
			83822343,
			83828511,
			83827740,
			83852412,
			83849328,
			83882481,
			83870145,
			16777471,
			0,
			83820544
		};

		// Token: 0x04000C9A RID: 3226
		private readonly BGCurveBaseMath math;

		// Token: 0x04000C9B RID: 3227
		private bool[] excludedSections;

		// Token: 0x04000C9C RID: 3228
		private float[] minSectionDistances;
	}
}
