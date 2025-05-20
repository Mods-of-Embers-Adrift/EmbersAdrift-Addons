using System;
using TriangleNet.Geometry;
using TriangleNet.Tools;
using TriangleNet.Topology;

namespace TriangleNet
{
	// Token: 0x020000EE RID: 238
	internal class NewLocation
	{
		// Token: 0x06000872 RID: 2162 RVA: 0x000B2B0C File Offset: 0x000B0D0C
		public NewLocation(Mesh mesh, IPredicates predicates)
		{
			this.mesh = mesh;
			this.predicates = predicates;
			this.behavior = mesh.behavior;
		}

		// Token: 0x06000873 RID: 2163 RVA: 0x000489AC File Offset: 0x00046BAC
		public Point FindLocation(Vertex org, Vertex dest, Vertex apex, ref double xi, ref double eta, bool offcenter, Otri badotri)
		{
			if (this.behavior.MaxAngle == 0.0)
			{
				return this.FindNewLocationWithoutMaxAngle(org, dest, apex, ref xi, ref eta, true, badotri);
			}
			return this.FindNewLocation(org, dest, apex, ref xi, ref eta, true, badotri);
		}

		// Token: 0x06000874 RID: 2164 RVA: 0x000B2BD8 File Offset: 0x000B0DD8
		private Point FindNewLocationWithoutMaxAngle(Vertex torg, Vertex tdest, Vertex tapex, ref double xi, ref double eta, bool offcenter, Otri badotri)
		{
			double offconstant = this.behavior.offconstant;
			int num = 0;
			Otri otri = default(Otri);
			double[] array = new double[2];
			double num2 = 0.0;
			double num3 = 0.0;
			double[] array2 = new double[5];
			double[] array3 = new double[4];
			double num4 = 0.06;
			double num5 = 1.0;
			double num6 = 1.0;
			int num7 = 0;
			double[] array4 = new double[2];
			double num8 = 0.0;
			double num9 = 0.0;
			Statistic.CircumcenterCount += 1L;
			double num10 = tdest.x - torg.x;
			double num11 = tdest.y - torg.y;
			double num12 = tapex.x - torg.x;
			double num13 = tapex.y - torg.y;
			double num14 = tapex.x - tdest.x;
			double num15 = tapex.y - tdest.y;
			double num16 = num10 * num10 + num11 * num11;
			double num17 = num12 * num12 + num13 * num13;
			double num18 = (tdest.x - tapex.x) * (tdest.x - tapex.x) + (tdest.y - tapex.y) * (tdest.y - tapex.y);
			double num19;
			if (Behavior.NoExact)
			{
				num19 = 0.5 / (num10 * num13 - num12 * num11);
			}
			else
			{
				num19 = 0.5 / this.predicates.CounterClockwise(tdest, tapex, torg);
				Statistic.CounterClockwiseCount -= 1L;
			}
			double num20 = (num13 * num16 - num11 * num17) * num19;
			double num21 = (num10 * num17 - num12 * num16) * num19;
			Point point = new Point(torg.x + num20, torg.y + num21);
			Otri badotri2 = badotri;
			int num22 = this.LongestShortestEdge(num17, num18, num16);
			double num23;
			double num24;
			double num25;
			double num26;
			double num27;
			Point point2;
			Point point3;
			Point point4;
			if (num22 <= 213)
			{
				if (num22 == 123)
				{
					num23 = num12;
					num24 = num13;
					num25 = num17;
					num26 = num18;
					num27 = num16;
					point2 = tdest;
					point3 = torg;
					point4 = tapex;
					goto IL_2FF;
				}
				if (num22 == 132)
				{
					num23 = num12;
					num24 = num13;
					num25 = num17;
					num26 = num16;
					num27 = num18;
					point2 = tdest;
					point3 = tapex;
					point4 = torg;
					goto IL_2FF;
				}
				if (num22 == 213)
				{
					num23 = num14;
					num24 = num15;
					num25 = num18;
					num26 = num17;
					num27 = num16;
					point2 = torg;
					point3 = tdest;
					point4 = tapex;
					goto IL_2FF;
				}
			}
			else
			{
				if (num22 == 231)
				{
					num23 = num14;
					num24 = num15;
					num25 = num18;
					num26 = num16;
					num27 = num17;
					point2 = torg;
					point3 = tapex;
					point4 = tdest;
					goto IL_2FF;
				}
				if (num22 == 312)
				{
					num23 = num10;
					num24 = num11;
					num25 = num16;
					num26 = num17;
					num27 = num18;
					point2 = tapex;
					point3 = tdest;
					point4 = torg;
					goto IL_2FF;
				}
				if (num22 != 321)
				{
				}
			}
			num23 = num10;
			num24 = num11;
			num25 = num16;
			num26 = num18;
			num27 = num17;
			point2 = tapex;
			point3 = torg;
			point4 = tdest;
			IL_2FF:
			if (offcenter && offconstant > 0.0)
			{
				if (num22 == 213 || num22 == 231)
				{
					double num28 = 0.5 * num23 - offconstant * num24;
					double num29 = 0.5 * num24 + offconstant * num23;
					if (num28 * num28 + num29 * num29 < (num20 - num10) * (num20 - num10) + (num21 - num11) * (num21 - num11))
					{
						num20 = num10 + num28;
						num21 = num11 + num29;
					}
					else
					{
						num = 1;
					}
				}
				else if (num22 == 123 || num22 == 132)
				{
					double num28 = 0.5 * num23 + offconstant * num24;
					double num29 = 0.5 * num24 - offconstant * num23;
					if (num28 * num28 + num29 * num29 < num20 * num20 + num21 * num21)
					{
						num20 = num28;
						num21 = num29;
					}
					else
					{
						num = 1;
					}
				}
				else
				{
					double num28 = 0.5 * num23 - offconstant * num24;
					double num29 = 0.5 * num24 + offconstant * num23;
					if (num28 * num28 + num29 * num29 < num20 * num20 + num21 * num21)
					{
						num20 = num28;
						num21 = num29;
					}
					else
					{
						num = 1;
					}
				}
			}
			if (num == 1)
			{
				double num30 = (num26 + num25 - num27) / (2.0 * Math.Sqrt(num26) * Math.Sqrt(num25));
				bool flag = num30 < 0.0 || Math.Abs(num30 - 0.0) <= 1E-50;
				num7 = this.DoSmoothing(badotri2, torg, tdest, tapex, ref array4);
				if (num7 > 0)
				{
					Statistic.RelocationCount += 1L;
					num20 = array4[0] - torg.x;
					num21 = array4[1] - torg.y;
					num8 = torg.x;
					num9 = torg.y;
					switch (num7)
					{
					case 1:
						this.mesh.DeleteVertex(ref badotri2);
						break;
					case 2:
						badotri2.Lnext();
						this.mesh.DeleteVertex(ref badotri2);
						break;
					case 3:
						badotri2.Lprev();
						this.mesh.DeleteVertex(ref badotri2);
						break;
					}
				}
				else
				{
					double num31 = Math.Sqrt(num25) / (2.0 * Math.Sin(this.behavior.MinAngle * 3.141592653589793 / 180.0));
					double num32 = (point3.x + point4.x) / 2.0;
					double num33 = (point3.y + point4.y) / 2.0;
					double num34 = num32 + Math.Sqrt(num31 * num31 - num25 / 4.0) * (point3.y - point4.y) / Math.Sqrt(num25);
					double num35 = num33 + Math.Sqrt(num31 * num31 - num25 / 4.0) * (point4.x - point3.x) / Math.Sqrt(num25);
					double num36 = num32 - Math.Sqrt(num31 * num31 - num25 / 4.0) * (point3.y - point4.y) / Math.Sqrt(num25);
					double num37 = num33 - Math.Sqrt(num31 * num31 - num25 / 4.0) * (point4.x - point3.x) / Math.Sqrt(num25);
					double num38 = (num34 - point2.x) * (num34 - point2.x);
					double num39 = (num35 - point2.y) * (num35 - point2.y);
					double num40 = (num36 - point2.x) * (num36 - point2.x);
					double num41 = (num37 - point2.y) * (num37 - point2.y);
					double x;
					double y;
					if (num38 + num39 <= num40 + num41)
					{
						x = num34;
						y = num35;
					}
					else
					{
						x = num36;
						y = num37;
					}
					bool neighborsVertex = this.GetNeighborsVertex(badotri, point3.x, point3.y, point2.x, point2.y, ref array, ref otri);
					double num42 = num20;
					double num43 = num21;
					if (!neighborsVertex)
					{
						Vertex org = otri.Org();
						Vertex dest = otri.Dest();
						Vertex apex = otri.Apex();
						Point point5 = this.predicates.FindCircumcenter(org, dest, apex, ref num2, ref num3);
						double num44 = point3.y - point2.y;
						double num45 = point2.x - point3.x;
						num44 = point.x + num44;
						num45 = point.y + num45;
						this.CircleLineIntersection(point.x, point.y, num44, num45, x, y, num31, ref array2);
						double x2 = (point3.x + point2.x) / 2.0;
						double y2 = (point3.y + point2.y) / 2.0;
						double num46;
						double num47;
						if (this.ChooseCorrectPoint(x2, y2, array2[3], array2[4], point.x, point.y, flag))
						{
							num46 = array2[3];
							num47 = array2[4];
						}
						else
						{
							num46 = array2[1];
							num47 = array2[2];
						}
						this.PointBetweenPoints(num46, num47, point.x, point.y, point5.x, point5.y, ref array3);
						if (array2[0] > 0.0)
						{
							if (Math.Abs(array3[0] - 1.0) <= 1E-50)
							{
								if (this.IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, point5.x, point5.y))
								{
									num42 = num20;
									num43 = num21;
								}
								else
								{
									num42 = array3[2] - torg.x;
									num43 = array3[3] - torg.y;
								}
							}
							else if (this.IsBadTriangleAngle(point4.x, point4.y, point3.x, point3.y, num46, num47))
							{
								double num48 = Math.Sqrt((num46 - point.x) * (num46 - point.x) + (num47 - point.y) * (num47 - point.y));
								double num49 = point.x - num46;
								double num50 = point.y - num47;
								num49 /= num48;
								num50 /= num48;
								num46 += num49 * num4 * Math.Sqrt(num25);
								num47 += num50 * num4 * Math.Sqrt(num25);
								if (this.IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num46, num47))
								{
									num42 = num20;
									num43 = num21;
								}
								else
								{
									num42 = num46 - torg.x;
									num43 = num47 - torg.y;
								}
							}
							else
							{
								num42 = num46 - torg.x;
								num43 = num47 - torg.y;
							}
							if ((point2.x - point.x) * (point2.x - point.x) + (point2.y - point.y) * (point2.y - point.y) > num5 * ((point2.x - (num42 + torg.x)) * (point2.x - (num42 + torg.x)) + (point2.y - (num43 + torg.y)) * (point2.y - (num43 + torg.y))))
							{
								num42 = num20;
								num43 = num21;
							}
						}
					}
					bool neighborsVertex2 = this.GetNeighborsVertex(badotri, point4.x, point4.y, point2.x, point2.y, ref array, ref otri);
					double num51 = num20;
					double num52 = num21;
					if (!neighborsVertex2)
					{
						Vertex org = otri.Org();
						Vertex dest = otri.Dest();
						Vertex apex = otri.Apex();
						Point point5 = this.predicates.FindCircumcenter(org, dest, apex, ref num2, ref num3);
						double num44 = point4.y - point2.y;
						double num45 = point2.x - point4.x;
						num44 = point.x + num44;
						num45 = point.y + num45;
						this.CircleLineIntersection(point.x, point.y, num44, num45, x, y, num31, ref array2);
						double x3 = (point4.x + point2.x) / 2.0;
						double y3 = (point4.y + point2.y) / 2.0;
						double num46;
						double num47;
						if (this.ChooseCorrectPoint(x3, y3, array2[3], array2[4], point.x, point.y, false))
						{
							num46 = array2[3];
							num47 = array2[4];
						}
						else
						{
							num46 = array2[1];
							num47 = array2[2];
						}
						this.PointBetweenPoints(num46, num47, point.x, point.y, point5.x, point5.y, ref array3);
						if (array2[0] > 0.0)
						{
							if (Math.Abs(array3[0] - 1.0) <= 1E-50)
							{
								if (this.IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, point5.x, point5.y))
								{
									num51 = num20;
									num52 = num21;
								}
								else
								{
									num51 = array3[2] - torg.x;
									num52 = array3[3] - torg.y;
								}
							}
							else if (this.IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num46, num47))
							{
								double num48 = Math.Sqrt((num46 - point.x) * (num46 - point.x) + (num47 - point.y) * (num47 - point.y));
								double num49 = point.x - num46;
								double num50 = point.y - num47;
								num49 /= num48;
								num50 /= num48;
								num46 += num49 * num4 * Math.Sqrt(num25);
								num47 += num50 * num4 * Math.Sqrt(num25);
								if (this.IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num46, num47))
								{
									num51 = num20;
									num52 = num21;
								}
								else
								{
									num51 = num46 - torg.x;
									num52 = num47 - torg.y;
								}
							}
							else
							{
								num51 = num46 - torg.x;
								num52 = num47 - torg.y;
							}
							if ((point2.x - point.x) * (point2.x - point.x) + (point2.y - point.y) * (point2.y - point.y) > num5 * ((point2.x - (num51 + torg.x)) * (point2.x - (num51 + torg.x)) + (point2.y - (num52 + torg.y)) * (point2.y - (num52 + torg.y))))
							{
								num51 = num20;
								num52 = num21;
							}
						}
					}
					if (flag)
					{
						num20 = num42;
						num21 = num43;
					}
					else if (num6 * ((point2.x - (num51 + torg.x)) * (point2.x - (num51 + torg.x)) + (point2.y - (num52 + torg.y)) * (point2.y - (num52 + torg.y))) > (point2.x - (num42 + torg.x)) * (point2.x - (num42 + torg.x)) + (point2.y - (num43 + torg.y)) * (point2.y - (num43 + torg.y)))
					{
						num20 = num51;
						num21 = num52;
					}
					else
					{
						num20 = num42;
						num21 = num43;
					}
				}
			}
			Point point6 = new Point();
			if (num7 <= 0)
			{
				point6.x = torg.x + num20;
				point6.y = torg.y + num21;
			}
			else
			{
				point6.x = num8 + num20;
				point6.y = num9 + num21;
			}
			xi = (num13 * num20 - num12 * num21) * (2.0 * num19);
			eta = (num10 * num21 - num11 * num20) * (2.0 * num19);
			return point6;
		}

		// Token: 0x06000875 RID: 2165 RVA: 0x000B3B0C File Offset: 0x000B1D0C
		private Point FindNewLocation(Vertex torg, Vertex tdest, Vertex tapex, ref double xi, ref double eta, bool offcenter, Otri badotri)
		{
			double offconstant = this.behavior.offconstant;
			int num = 0;
			Otri otri = default(Otri);
			double[] array = new double[2];
			double num2 = 0.0;
			double num3 = 0.0;
			double[] array2 = new double[5];
			double[] array3 = new double[4];
			double num4 = 0.06;
			double num5 = 1.0;
			double num6 = 1.0;
			int num7 = 0;
			double[] array4 = new double[2];
			double num8 = 0.0;
			double num9 = 0.0;
			double num10 = 0.0;
			double num11 = 0.0;
			double[] array5 = new double[3];
			double[] array6 = new double[4];
			Statistic.CircumcenterCount += 1L;
			double num12 = tdest.x - torg.x;
			double num13 = tdest.y - torg.y;
			double num14 = tapex.x - torg.x;
			double num15 = tapex.y - torg.y;
			double num16 = tapex.x - tdest.x;
			double num17 = tapex.y - tdest.y;
			double num18 = num12 * num12 + num13 * num13;
			double num19 = num14 * num14 + num15 * num15;
			double num20 = (tdest.x - tapex.x) * (tdest.x - tapex.x) + (tdest.y - tapex.y) * (tdest.y - tapex.y);
			double num21;
			if (Behavior.NoExact)
			{
				num21 = 0.5 / (num12 * num15 - num14 * num13);
			}
			else
			{
				num21 = 0.5 / this.predicates.CounterClockwise(tdest, tapex, torg);
				Statistic.CounterClockwiseCount -= 1L;
			}
			double num22 = (num15 * num18 - num13 * num19) * num21;
			double num23 = (num12 * num19 - num14 * num18) * num21;
			Point point = new Point(torg.x + num22, torg.y + num23);
			Otri badotri2 = badotri;
			int num24 = this.LongestShortestEdge(num19, num20, num18);
			double num25;
			double num26;
			double num27;
			double num28;
			double num29;
			Point point2;
			Point point3;
			Point point4;
			if (num24 <= 213)
			{
				if (num24 == 123)
				{
					num25 = num14;
					num26 = num15;
					num27 = num19;
					num28 = num20;
					num29 = num18;
					point2 = tdest;
					point3 = torg;
					point4 = tapex;
					goto IL_325;
				}
				if (num24 == 132)
				{
					num25 = num14;
					num26 = num15;
					num27 = num19;
					num28 = num18;
					num29 = num20;
					point2 = tdest;
					point3 = tapex;
					point4 = torg;
					goto IL_325;
				}
				if (num24 == 213)
				{
					num25 = num16;
					num26 = num17;
					num27 = num20;
					num28 = num19;
					num29 = num18;
					point2 = torg;
					point3 = tdest;
					point4 = tapex;
					goto IL_325;
				}
			}
			else
			{
				if (num24 == 231)
				{
					num25 = num16;
					num26 = num17;
					num27 = num20;
					num28 = num18;
					num29 = num19;
					point2 = torg;
					point3 = tapex;
					point4 = tdest;
					goto IL_325;
				}
				if (num24 == 312)
				{
					num25 = num12;
					num26 = num13;
					num27 = num18;
					num28 = num19;
					num29 = num20;
					point2 = tapex;
					point3 = tdest;
					point4 = torg;
					goto IL_325;
				}
				if (num24 != 321)
				{
				}
			}
			num25 = num12;
			num26 = num13;
			num27 = num18;
			num28 = num20;
			num29 = num19;
			point2 = tapex;
			point3 = torg;
			point4 = tdest;
			IL_325:
			if (offcenter && offconstant > 0.0)
			{
				if (num24 == 213 || num24 == 231)
				{
					double num30 = 0.5 * num25 - offconstant * num26;
					double num31 = 0.5 * num26 + offconstant * num25;
					if (num30 * num30 + num31 * num31 < (num22 - num12) * (num22 - num12) + (num23 - num13) * (num23 - num13))
					{
						num22 = num12 + num30;
						num23 = num13 + num31;
					}
					else
					{
						num = 1;
					}
				}
				else if (num24 == 123 || num24 == 132)
				{
					double num30 = 0.5 * num25 + offconstant * num26;
					double num31 = 0.5 * num26 - offconstant * num25;
					if (num30 * num30 + num31 * num31 < num22 * num22 + num23 * num23)
					{
						num22 = num30;
						num23 = num31;
					}
					else
					{
						num = 1;
					}
				}
				else
				{
					double num30 = 0.5 * num25 - offconstant * num26;
					double num31 = 0.5 * num26 + offconstant * num25;
					if (num30 * num30 + num31 * num31 < num22 * num22 + num23 * num23)
					{
						num22 = num30;
						num23 = num31;
					}
					else
					{
						num = 1;
					}
				}
			}
			if (num == 1)
			{
				double num32 = (num28 + num27 - num29) / (2.0 * Math.Sqrt(num28) * Math.Sqrt(num27));
				bool flag = num32 < 0.0 || Math.Abs(num32 - 0.0) <= 1E-50;
				num7 = this.DoSmoothing(badotri2, torg, tdest, tapex, ref array4);
				if (num7 > 0)
				{
					Statistic.RelocationCount += 1L;
					num22 = array4[0] - torg.x;
					num23 = array4[1] - torg.y;
					num8 = torg.x;
					num9 = torg.y;
					switch (num7)
					{
					case 1:
						this.mesh.DeleteVertex(ref badotri2);
						break;
					case 2:
						badotri2.Lnext();
						this.mesh.DeleteVertex(ref badotri2);
						break;
					case 3:
						badotri2.Lprev();
						this.mesh.DeleteVertex(ref badotri2);
						break;
					}
				}
				else
				{
					double num33 = Math.Acos((num28 + num29 - num27) / (2.0 * Math.Sqrt(num28) * Math.Sqrt(num29))) * 180.0 / 3.141592653589793;
					if (this.behavior.MinAngle > num33)
					{
						num33 = this.behavior.MinAngle;
					}
					else
					{
						num33 += 0.5;
					}
					double num34 = Math.Sqrt(num27) / (2.0 * Math.Sin(num33 * 3.141592653589793 / 180.0));
					double num35 = (point3.x + point4.x) / 2.0;
					double num36 = (point3.y + point4.y) / 2.0;
					double num37 = num35 + Math.Sqrt(num34 * num34 - num27 / 4.0) * (point3.y - point4.y) / Math.Sqrt(num27);
					double num38 = num36 + Math.Sqrt(num34 * num34 - num27 / 4.0) * (point4.x - point3.x) / Math.Sqrt(num27);
					double num39 = num35 - Math.Sqrt(num34 * num34 - num27 / 4.0) * (point3.y - point4.y) / Math.Sqrt(num27);
					double num40 = num36 - Math.Sqrt(num34 * num34 - num27 / 4.0) * (point4.x - point3.x) / Math.Sqrt(num27);
					double num41 = (num37 - point2.x) * (num37 - point2.x);
					double num42 = (num38 - point2.y) * (num38 - point2.y);
					double num43 = (num39 - point2.x) * (num39 - point2.x);
					double num44 = (num40 - point2.y) * (num40 - point2.y);
					double num45;
					double num46;
					if (num41 + num42 <= num43 + num44)
					{
						num45 = num37;
						num46 = num38;
					}
					else
					{
						num45 = num39;
						num46 = num40;
					}
					bool neighborsVertex = this.GetNeighborsVertex(badotri, point3.x, point3.y, point2.x, point2.y, ref array, ref otri);
					double num47 = num22;
					double num48 = num23;
					double num49 = Math.Sqrt((num45 - num35) * (num45 - num35) + (num46 - num36) * (num46 - num36));
					double num50 = (num45 - num35) / num49;
					double num51 = (num46 - num36) / num49;
					double num52 = num45 + num50 * num34;
					double num53 = num46 + num51 * num34;
					double num54 = (2.0 * this.behavior.MaxAngle + num33 - 180.0) * 3.141592653589793 / 180.0;
					double num55 = num52 * Math.Cos(num54) + num53 * Math.Sin(num54) + num45 - num45 * Math.Cos(num54) - num46 * Math.Sin(num54);
					double num56 = -num52 * Math.Sin(num54) + num53 * Math.Cos(num54) + num46 + num45 * Math.Sin(num54) - num46 * Math.Cos(num54);
					double num57 = num52 * Math.Cos(num54) - num53 * Math.Sin(num54) + num45 - num45 * Math.Cos(num54) + num46 * Math.Sin(num54);
					double num58 = num52 * Math.Sin(num54) + num53 * Math.Cos(num54) + num46 - num45 * Math.Sin(num54) - num46 * Math.Cos(num54);
					double num59;
					double num60;
					double num61;
					double num62;
					if (this.ChooseCorrectPoint(num57, num58, point3.x, point3.y, num55, num56, true))
					{
						num59 = num55;
						num60 = num56;
						num61 = num57;
						num62 = num58;
					}
					else
					{
						num59 = num57;
						num60 = num58;
						num61 = num55;
						num62 = num56;
					}
					double num63 = (point3.x + point2.x) / 2.0;
					double num64 = (point3.y + point2.y) / 2.0;
					if (!neighborsVertex)
					{
						Vertex org = otri.Org();
						Vertex dest = otri.Dest();
						Vertex apex = otri.Apex();
						Point point5 = this.predicates.FindCircumcenter(org, dest, apex, ref num2, ref num3);
						double num65 = point3.y - point2.y;
						double num66 = point2.x - point3.x;
						num65 = point.x + num65;
						num66 = point.y + num66;
						this.CircleLineIntersection(point.x, point.y, num65, num66, num45, num46, num34, ref array2);
						double num67;
						double num68;
						if (this.ChooseCorrectPoint(num63, num64, array2[3], array2[4], point.x, point.y, flag))
						{
							num67 = array2[3];
							num68 = array2[4];
						}
						else
						{
							num67 = array2[1];
							num68 = array2[2];
						}
						double x = point3.x;
						double y = point3.y;
						num50 = point4.x - point3.x;
						num51 = point4.y - point3.y;
						double x2 = num59;
						double y2 = num60;
						this.LineLineIntersection(point.x, point.y, num65, num66, x, y, x2, y2, ref array5);
						if (array5[0] > 0.0)
						{
							num10 = array5[1];
							num11 = array5[2];
						}
						this.PointBetweenPoints(num67, num68, point.x, point.y, point5.x, point5.y, ref array3);
						if (array2[0] > 0.0)
						{
							if (Math.Abs(array3[0] - 1.0) <= 1E-50)
							{
								this.PointBetweenPoints(array3[2], array3[3], point.x, point.y, num10, num11, ref array6);
								if (Math.Abs(array6[0] - 1.0) <= 1E-50 && array5[0] > 0.0)
								{
									if ((point2.x - num59) * (point2.x - num59) + (point2.y - num60) * (point2.y - num60) > num5 * ((point2.x - num10) * (point2.x - num10) + (point2.y - num11) * (point2.y - num11)) && this.IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num59, num60) && this.MinDistanceToNeighbor(num59, num60, ref otri) > this.MinDistanceToNeighbor(num10, num11, ref otri))
									{
										num47 = num59 - torg.x;
										num48 = num60 - torg.y;
									}
									else if (this.IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num10, num11))
									{
										double num69 = Math.Sqrt((num10 - point.x) * (num10 - point.x) + (num11 - point.y) * (num11 - point.y));
										double num70 = point.x - num10;
										double num71 = point.y - num11;
										num70 /= num69;
										num71 /= num69;
										num10 += num70 * num4 * Math.Sqrt(num27);
										num11 += num71 * num4 * Math.Sqrt(num27);
										if (this.IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num10, num11))
										{
											num47 = num22;
											num48 = num23;
										}
										else
										{
											num47 = num10 - torg.x;
											num48 = num11 - torg.y;
										}
									}
									else
									{
										num47 = array6[2] - torg.x;
										num48 = array6[3] - torg.y;
									}
								}
								else if (this.IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, point5.x, point5.y))
								{
									num47 = num22;
									num48 = num23;
								}
								else
								{
									num47 = array3[2] - torg.x;
									num48 = array3[3] - torg.y;
								}
							}
							else
							{
								this.PointBetweenPoints(num67, num68, point.x, point.y, num10, num11, ref array6);
								if (Math.Abs(array6[0] - 1.0) <= 1E-50 && array5[0] > 0.0)
								{
									if ((point2.x - num59) * (point2.x - num59) + (point2.y - num60) * (point2.y - num60) > num5 * ((point2.x - num10) * (point2.x - num10) + (point2.y - num11) * (point2.y - num11)) && this.IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num59, num60) && this.MinDistanceToNeighbor(num59, num60, ref otri) > this.MinDistanceToNeighbor(num10, num11, ref otri))
									{
										num47 = num59 - torg.x;
										num48 = num60 - torg.y;
									}
									else if (this.IsBadTriangleAngle(point4.x, point4.y, point3.x, point3.y, num10, num11))
									{
										double num69 = Math.Sqrt((num10 - point.x) * (num10 - point.x) + (num11 - point.y) * (num11 - point.y));
										double num70 = point.x - num10;
										double num71 = point.y - num11;
										num70 /= num69;
										num71 /= num69;
										num10 += num70 * num4 * Math.Sqrt(num27);
										num11 += num71 * num4 * Math.Sqrt(num27);
										if (this.IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num10, num11))
										{
											num47 = num22;
											num48 = num23;
										}
										else
										{
											num47 = num10 - torg.x;
											num48 = num11 - torg.y;
										}
									}
									else
									{
										num47 = array6[2] - torg.x;
										num48 = array6[3] - torg.y;
									}
								}
								else if (this.IsBadTriangleAngle(point4.x, point4.y, point3.x, point3.y, num67, num68))
								{
									double num69 = Math.Sqrt((num67 - point.x) * (num67 - point.x) + (num68 - point.y) * (num68 - point.y));
									double num70 = point.x - num67;
									double num71 = point.y - num68;
									num70 /= num69;
									num71 /= num69;
									num67 += num70 * num4 * Math.Sqrt(num27);
									num68 += num71 * num4 * Math.Sqrt(num27);
									if (this.IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num67, num68))
									{
										num47 = num22;
										num48 = num23;
									}
									else
									{
										num47 = num67 - torg.x;
										num48 = num68 - torg.y;
									}
								}
								else
								{
									num47 = num67 - torg.x;
									num48 = num68 - torg.y;
								}
							}
							if ((point2.x - point.x) * (point2.x - point.x) + (point2.y - point.y) * (point2.y - point.y) > num5 * ((point2.x - (num47 + torg.x)) * (point2.x - (num47 + torg.x)) + (point2.y - (num48 + torg.y)) * (point2.y - (num48 + torg.y))))
							{
								num47 = num22;
								num48 = num23;
							}
						}
					}
					bool neighborsVertex2 = this.GetNeighborsVertex(badotri, point4.x, point4.y, point2.x, point2.y, ref array, ref otri);
					double num72 = num22;
					double num73 = num23;
					double num74 = (point4.x + point2.x) / 2.0;
					double num75 = (point4.y + point2.y) / 2.0;
					if (!neighborsVertex2)
					{
						Vertex org = otri.Org();
						Vertex dest = otri.Dest();
						Vertex apex = otri.Apex();
						Point point5 = this.predicates.FindCircumcenter(org, dest, apex, ref num2, ref num3);
						double num65 = point4.y - point2.y;
						double num66 = point2.x - point4.x;
						num65 = point.x + num65;
						num66 = point.y + num66;
						this.CircleLineIntersection(point.x, point.y, num65, num66, num45, num46, num34, ref array2);
						double num67;
						double num68;
						if (this.ChooseCorrectPoint(num74, num75, array2[3], array2[4], point.x, point.y, false))
						{
							num67 = array2[3];
							num68 = array2[4];
						}
						else
						{
							num67 = array2[1];
							num68 = array2[2];
						}
						double x = point4.x;
						double y = point4.y;
						num50 = point3.x - point4.x;
						num51 = point3.y - point4.y;
						double x2 = num61;
						double y2 = num62;
						this.LineLineIntersection(point.x, point.y, num65, num66, x, y, x2, y2, ref array5);
						if (array5[0] > 0.0)
						{
							num10 = array5[1];
							num11 = array5[2];
						}
						this.PointBetweenPoints(num67, num68, point.x, point.y, point5.x, point5.y, ref array3);
						if (array2[0] > 0.0)
						{
							if (Math.Abs(array3[0] - 1.0) <= 1E-50)
							{
								this.PointBetweenPoints(array3[2], array3[3], point.x, point.y, num10, num11, ref array6);
								if (Math.Abs(array6[0] - 1.0) <= 1E-50 && array5[0] > 0.0)
								{
									if ((point2.x - num61) * (point2.x - num61) + (point2.y - num62) * (point2.y - num62) > num5 * ((point2.x - num10) * (point2.x - num10) + (point2.y - num11) * (point2.y - num11)) && this.IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num61, num62) && this.MinDistanceToNeighbor(num61, num62, ref otri) > this.MinDistanceToNeighbor(num10, num11, ref otri))
									{
										num72 = num61 - torg.x;
										num73 = num62 - torg.y;
									}
									else if (this.IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num10, num11))
									{
										double num69 = Math.Sqrt((num10 - point.x) * (num10 - point.x) + (num11 - point.y) * (num11 - point.y));
										double num70 = point.x - num10;
										double num71 = point.y - num11;
										num70 /= num69;
										num71 /= num69;
										num10 += num70 * num4 * Math.Sqrt(num27);
										num11 += num71 * num4 * Math.Sqrt(num27);
										if (this.IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num10, num11))
										{
											num72 = num22;
											num73 = num23;
										}
										else
										{
											num72 = num10 - torg.x;
											num73 = num11 - torg.y;
										}
									}
									else
									{
										num72 = array6[2] - torg.x;
										num73 = array6[3] - torg.y;
									}
								}
								else if (this.IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, point5.x, point5.y))
								{
									num72 = num22;
									num73 = num23;
								}
								else
								{
									num72 = array3[2] - torg.x;
									num73 = array3[3] - torg.y;
								}
							}
							else
							{
								this.PointBetweenPoints(num67, num68, point.x, point.y, num10, num11, ref array6);
								if (Math.Abs(array6[0] - 1.0) <= 1E-50 && array5[0] > 0.0)
								{
									if ((point2.x - num61) * (point2.x - num61) + (point2.y - num62) * (point2.y - num62) > num5 * ((point2.x - num10) * (point2.x - num10) + (point2.y - num11) * (point2.y - num11)) && this.IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num61, num62) && this.MinDistanceToNeighbor(num61, num62, ref otri) > this.MinDistanceToNeighbor(num10, num11, ref otri))
									{
										num72 = num61 - torg.x;
										num73 = num62 - torg.y;
									}
									else if (this.IsBadTriangleAngle(point4.x, point4.y, point3.x, point3.y, num10, num11))
									{
										double num69 = Math.Sqrt((num10 - point.x) * (num10 - point.x) + (num11 - point.y) * (num11 - point.y));
										double num70 = point.x - num10;
										double num71 = point.y - num11;
										num70 /= num69;
										num71 /= num69;
										num10 += num70 * num4 * Math.Sqrt(num27);
										num11 += num71 * num4 * Math.Sqrt(num27);
										if (this.IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num10, num11))
										{
											num72 = num22;
											num73 = num23;
										}
										else
										{
											num72 = num10 - torg.x;
											num73 = num11 - torg.y;
										}
									}
									else
									{
										num72 = array6[2] - torg.x;
										num73 = array6[3] - torg.y;
									}
								}
								else if (this.IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num67, num68))
								{
									double num69 = Math.Sqrt((num67 - point.x) * (num67 - point.x) + (num68 - point.y) * (num68 - point.y));
									double num70 = point.x - num67;
									double num71 = point.y - num68;
									num70 /= num69;
									num71 /= num69;
									num67 += num70 * num4 * Math.Sqrt(num27);
									num68 += num71 * num4 * Math.Sqrt(num27);
									if (this.IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num67, num68))
									{
										num72 = num22;
										num73 = num23;
									}
									else
									{
										num72 = num67 - torg.x;
										num73 = num68 - torg.y;
									}
								}
								else
								{
									num72 = num67 - torg.x;
									num73 = num68 - torg.y;
								}
							}
							if ((point2.x - point.x) * (point2.x - point.x) + (point2.y - point.y) * (point2.y - point.y) > num5 * ((point2.x - (num72 + torg.x)) * (point2.x - (num72 + torg.x)) + (point2.y - (num73 + torg.y)) * (point2.y - (num73 + torg.y))))
							{
								num72 = num22;
								num73 = num23;
							}
						}
					}
					if (flag)
					{
						if (neighborsVertex && neighborsVertex2)
						{
							if (num6 * ((point2.x - num74) * (point2.x - num74) + (point2.y - num75) * (point2.y - num75)) > (point2.x - num63) * (point2.x - num63) + (point2.y - num64) * (point2.y - num64))
							{
								num22 = num72;
								num23 = num73;
							}
							else
							{
								num22 = num47;
								num23 = num48;
							}
						}
						else if (neighborsVertex)
						{
							if (num6 * ((point2.x - (num72 + torg.x)) * (point2.x - (num72 + torg.x)) + (point2.y - (num73 + torg.y)) * (point2.y - (num73 + torg.y))) > (point2.x - num63) * (point2.x - num63) + (point2.y - num64) * (point2.y - num64))
							{
								num22 = num72;
								num23 = num73;
							}
							else
							{
								num22 = num47;
								num23 = num48;
							}
						}
						else if (neighborsVertex2)
						{
							if (num6 * ((point2.x - num74) * (point2.x - num74) + (point2.y - num75) * (point2.y - num75)) > (point2.x - (num47 + torg.x)) * (point2.x - (num47 + torg.x)) + (point2.y - (num48 + torg.y)) * (point2.y - (num48 + torg.y)))
							{
								num22 = num72;
								num23 = num73;
							}
							else
							{
								num22 = num47;
								num23 = num48;
							}
						}
						else if (num6 * ((point2.x - (num72 + torg.x)) * (point2.x - (num72 + torg.x)) + (point2.y - (num73 + torg.y)) * (point2.y - (num73 + torg.y))) > (point2.x - (num47 + torg.x)) * (point2.x - (num47 + torg.x)) + (point2.y - (num48 + torg.y)) * (point2.y - (num48 + torg.y)))
						{
							num22 = num72;
							num23 = num73;
						}
						else
						{
							num22 = num47;
							num23 = num48;
						}
					}
					else if (neighborsVertex && neighborsVertex2)
					{
						if (num6 * ((point2.x - num74) * (point2.x - num74) + (point2.y - num75) * (point2.y - num75)) > (point2.x - num63) * (point2.x - num63) + (point2.y - num64) * (point2.y - num64))
						{
							num22 = num72;
							num23 = num73;
						}
						else
						{
							num22 = num47;
							num23 = num48;
						}
					}
					else if (neighborsVertex)
					{
						if (num6 * ((point2.x - (num72 + torg.x)) * (point2.x - (num72 + torg.x)) + (point2.y - (num73 + torg.y)) * (point2.y - (num73 + torg.y))) > (point2.x - num63) * (point2.x - num63) + (point2.y - num64) * (point2.y - num64))
						{
							num22 = num72;
							num23 = num73;
						}
						else
						{
							num22 = num47;
							num23 = num48;
						}
					}
					else if (neighborsVertex2)
					{
						if (num6 * ((point2.x - num74) * (point2.x - num74) + (point2.y - num75) * (point2.y - num75)) > (point2.x - (num47 + torg.x)) * (point2.x - (num47 + torg.x)) + (point2.y - (num48 + torg.y)) * (point2.y - (num48 + torg.y)))
						{
							num22 = num72;
							num23 = num73;
						}
						else
						{
							num22 = num47;
							num23 = num48;
						}
					}
					else if (num6 * ((point2.x - (num72 + torg.x)) * (point2.x - (num72 + torg.x)) + (point2.y - (num73 + torg.y)) * (point2.y - (num73 + torg.y))) > (point2.x - (num47 + torg.x)) * (point2.x - (num47 + torg.x)) + (point2.y - (num48 + torg.y)) * (point2.y - (num48 + torg.y)))
					{
						num22 = num72;
						num23 = num73;
					}
					else
					{
						num22 = num47;
						num23 = num48;
					}
				}
			}
			Point point6 = new Point();
			if (num7 <= 0)
			{
				point6.x = torg.x + num22;
				point6.y = torg.y + num23;
			}
			else
			{
				point6.x = num8 + num22;
				point6.y = num9 + num23;
			}
			xi = (num15 * num22 - num14 * num23) * (2.0 * num21);
			eta = (num12 * num23 - num13 * num22) * (2.0 * num21);
			return point6;
		}

		// Token: 0x06000876 RID: 2166 RVA: 0x000B59C0 File Offset: 0x000B3BC0
		private int LongestShortestEdge(double aodist, double dadist, double dodist)
		{
			int num;
			int num2;
			int num3;
			if (dodist < aodist && dodist < dadist)
			{
				num = 3;
				if (aodist < dadist)
				{
					num2 = 2;
					num3 = 1;
				}
				else
				{
					num2 = 1;
					num3 = 2;
				}
			}
			else if (aodist < dadist)
			{
				num = 1;
				if (dodist < dadist)
				{
					num2 = 2;
					num3 = 3;
				}
				else
				{
					num2 = 3;
					num3 = 2;
				}
			}
			else
			{
				num = 2;
				if (aodist < dodist)
				{
					num2 = 3;
					num3 = 1;
				}
				else
				{
					num2 = 1;
					num3 = 3;
				}
			}
			return num * 100 + num3 * 10 + num2;
		}

		// Token: 0x06000877 RID: 2167 RVA: 0x000B5A20 File Offset: 0x000B3C20
		private int DoSmoothing(Otri badotri, Vertex torg, Vertex tdest, Vertex tapex, ref double[] newloc)
		{
			double[] array = new double[6];
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int starPoints = this.GetStarPoints(badotri, torg, tdest, tapex, 1, ref this.points_p);
			if (torg.type == VertexType.FreeVertex && starPoints != 0 && this.ValidPolygonAngles(starPoints, this.points_p))
			{
				bool flag;
				if (this.behavior.MaxAngle == 0.0)
				{
					flag = this.GetWedgeIntersectionWithoutMaxAngle(starPoints, this.points_p, ref newloc);
				}
				else
				{
					flag = this.GetWedgeIntersection(starPoints, this.points_p, ref newloc);
				}
				if (flag)
				{
					array[0] = newloc[0];
					array[1] = newloc[1];
					num++;
					num2 = 1;
				}
			}
			int starPoints2 = this.GetStarPoints(badotri, torg, tdest, tapex, 2, ref this.points_q);
			if (tdest.type == VertexType.FreeVertex && starPoints2 != 0 && this.ValidPolygonAngles(starPoints2, this.points_q))
			{
				bool flag;
				if (this.behavior.MaxAngle == 0.0)
				{
					flag = this.GetWedgeIntersectionWithoutMaxAngle(starPoints2, this.points_q, ref newloc);
				}
				else
				{
					flag = this.GetWedgeIntersection(starPoints2, this.points_q, ref newloc);
				}
				if (flag)
				{
					array[2] = newloc[0];
					array[3] = newloc[1];
					num++;
					num3 = 2;
				}
			}
			int starPoints3 = this.GetStarPoints(badotri, torg, tdest, tapex, 3, ref this.points_r);
			if (tapex.type == VertexType.FreeVertex && starPoints3 != 0 && this.ValidPolygonAngles(starPoints3, this.points_r))
			{
				bool flag;
				if (this.behavior.MaxAngle == 0.0)
				{
					flag = this.GetWedgeIntersectionWithoutMaxAngle(starPoints3, this.points_r, ref newloc);
				}
				else
				{
					flag = this.GetWedgeIntersection(starPoints3, this.points_r, ref newloc);
				}
				if (flag)
				{
					array[4] = newloc[0];
					array[5] = newloc[1];
					num++;
					num4 = 3;
				}
			}
			if (num > 0)
			{
				if (num2 > 0)
				{
					newloc[0] = array[0];
					newloc[1] = array[1];
					return num2;
				}
				if (num3 > 0)
				{
					newloc[0] = array[2];
					newloc[1] = array[3];
					return num3;
				}
				if (num4 > 0)
				{
					newloc[0] = array[4];
					newloc[1] = array[5];
					return num4;
				}
			}
			return 0;
		}

		// Token: 0x06000878 RID: 2168 RVA: 0x000B5C28 File Offset: 0x000B3E28
		private int GetStarPoints(Otri badotri, Vertex p, Vertex q, Vertex r, int whichPoint, ref double[] points)
		{
			Otri otri = default(Otri);
			double first_x = 0.0;
			double first_y = 0.0;
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			double[] array = new double[2];
			int num5 = 0;
			switch (whichPoint)
			{
			case 1:
				first_x = p.x;
				first_y = p.y;
				num = r.x;
				num2 = r.y;
				num3 = q.x;
				num4 = q.y;
				break;
			case 2:
				first_x = q.x;
				first_y = q.y;
				num = p.x;
				num2 = p.y;
				num3 = r.x;
				num4 = r.y;
				break;
			case 3:
				first_x = r.x;
				first_y = r.y;
				num = q.x;
				num2 = q.y;
				num3 = p.x;
				num4 = p.y;
				break;
			}
			Otri badotri2 = badotri;
			points[num5] = num;
			num5++;
			points[num5] = num2;
			num5++;
			array[0] = num;
			array[1] = num2;
			while (!this.GetNeighborsVertex(badotri2, first_x, first_y, num, num2, ref array, ref otri))
			{
				badotri2 = otri;
				num = array[0];
				num2 = array[1];
				points[num5] = array[0];
				num5++;
				points[num5] = array[1];
				num5++;
				if (Math.Abs(array[0] - num3) <= 1E-50 && Math.Abs(array[1] - num4) <= 1E-50)
				{
					IL_1A1:
					return num5 / 2;
				}
			}
			num5 = 0;
			goto IL_1A1;
		}

		// Token: 0x06000879 RID: 2169 RVA: 0x000B5DDC File Offset: 0x000B3FDC
		private bool GetNeighborsVertex(Otri badotri, double first_x, double first_y, double second_x, double second_y, ref double[] thirdpoint, ref Otri neighotri)
		{
			Otri otri = default(Otri);
			bool result = false;
			Vertex vertex = null;
			Vertex vertex2 = null;
			Vertex vertex3 = null;
			int num = 0;
			int num2 = 0;
			badotri.orient = 0;
			while (badotri.orient < 3)
			{
				badotri.Sym(ref otri);
				if (otri.tri.id != -1)
				{
					vertex = otri.Org();
					vertex2 = otri.Dest();
					vertex3 = otri.Apex();
					if ((vertex.x != vertex2.x || vertex.y != vertex2.y) && (vertex2.x != vertex3.x || vertex2.y != vertex3.y) && (vertex.x != vertex3.x || vertex.y != vertex3.y))
					{
						num = 0;
						if (Math.Abs(first_x - vertex.x) < 1E-50 && Math.Abs(first_y - vertex.y) < 1E-50)
						{
							num = 11;
						}
						else if (Math.Abs(first_x - vertex2.x) < 1E-50 && Math.Abs(first_y - vertex2.y) < 1E-50)
						{
							num = 12;
						}
						else if (Math.Abs(first_x - vertex3.x) < 1E-50 && Math.Abs(first_y - vertex3.y) < 1E-50)
						{
							num = 13;
						}
						num2 = 0;
						if (Math.Abs(second_x - vertex.x) < 1E-50 && Math.Abs(second_y - vertex.y) < 1E-50)
						{
							num2 = 21;
						}
						else if (Math.Abs(second_x - vertex2.x) < 1E-50 && Math.Abs(second_y - vertex2.y) < 1E-50)
						{
							num2 = 22;
						}
						else if (Math.Abs(second_x - vertex3.x) < 1E-50 && Math.Abs(second_y - vertex3.y) < 1E-50)
						{
							num2 = 23;
						}
					}
				}
				if ((num == 11 && (num2 == 22 || num2 == 23)) || (num == 12 && (num2 == 21 || num2 == 23)) || (num == 13 && (num2 == 21 || num2 == 22)))
				{
					break;
				}
				badotri.orient++;
			}
			if (num != 0)
			{
				switch (num)
				{
				case 11:
					if (num2 == 22)
					{
						thirdpoint[0] = vertex3.x;
						thirdpoint[1] = vertex3.y;
					}
					else if (num2 == 23)
					{
						thirdpoint[0] = vertex2.x;
						thirdpoint[1] = vertex2.y;
					}
					else
					{
						result = true;
					}
					break;
				case 12:
					if (num2 == 21)
					{
						thirdpoint[0] = vertex3.x;
						thirdpoint[1] = vertex3.y;
					}
					else if (num2 == 23)
					{
						thirdpoint[0] = vertex.x;
						thirdpoint[1] = vertex.y;
					}
					else
					{
						result = true;
					}
					break;
				case 13:
					if (num2 == 21)
					{
						thirdpoint[0] = vertex2.x;
						thirdpoint[1] = vertex2.y;
					}
					else if (num2 == 22)
					{
						thirdpoint[0] = vertex.x;
						thirdpoint[1] = vertex.y;
					}
					else
					{
						result = true;
					}
					break;
				default:
					if (num2 == 0)
					{
						result = true;
					}
					break;
				}
			}
			else
			{
				result = true;
			}
			neighotri = otri;
			return result;
		}

		// Token: 0x0600087A RID: 2170 RVA: 0x000B6144 File Offset: 0x000B4344
		private bool GetWedgeIntersectionWithoutMaxAngle(int numpoints, double[] points, ref double[] newloc)
		{
			if (2 * numpoints > this.petalx.Length)
			{
				this.petalx = new double[2 * numpoints];
				this.petaly = new double[2 * numpoints];
				this.petalr = new double[2 * numpoints];
				this.wedges = new double[2 * numpoints * 16 + 36];
			}
			double[] array = new double[3];
			int num = 0;
			double num2 = points[2 * numpoints - 4];
			double num3 = points[2 * numpoints - 3];
			double num4 = points[2 * numpoints - 2];
			double num5 = points[2 * numpoints - 1];
			double num6 = this.behavior.MinAngle * 3.141592653589793 / 180.0;
			double num7;
			double num8;
			if (this.behavior.goodAngle == 1.0)
			{
				num7 = 0.0;
				num8 = 0.0;
			}
			else
			{
				num7 = 0.5 / Math.Tan(num6);
				num8 = 0.5 / Math.Sin(num6);
			}
			for (int i = 0; i < numpoints * 2; i += 2)
			{
				double num9 = points[i];
				double num10 = points[i + 1];
				double num11 = num4 - num2;
				double num12 = num5 - num3;
				double num13 = Math.Sqrt(num11 * num11 + num12 * num12);
				this.petalx[i / 2] = num2 + 0.5 * num11 - num7 * num12;
				this.petaly[i / 2] = num3 + 0.5 * num12 + num7 * num11;
				this.petalr[i / 2] = num8 * num13;
				this.petalx[numpoints + i / 2] = this.petalx[i / 2];
				this.petaly[numpoints + i / 2] = this.petaly[i / 2];
				this.petalr[numpoints + i / 2] = this.petalr[i / 2];
				double num14 = (num2 + num4) / 2.0;
				double num15 = (num3 + num5) / 2.0;
				double num16 = Math.Sqrt((this.petalx[i / 2] - num14) * (this.petalx[i / 2] - num14) + (this.petaly[i / 2] - num15) * (this.petaly[i / 2] - num15));
				double num17 = (this.petalx[i / 2] - num14) / num16;
				double num18 = (this.petaly[i / 2] - num15) / num16;
				double num19 = this.petalx[i / 2] + num17 * this.petalr[i / 2];
				double num20 = this.petaly[i / 2] + num18 * this.petalr[i / 2];
				num17 = num4 - num2;
				num18 = num5 - num3;
				double num21 = num4 * Math.Cos(num6) - num5 * Math.Sin(num6) + num2 - num2 * Math.Cos(num6) + num3 * Math.Sin(num6);
				double num22 = num4 * Math.Sin(num6) + num5 * Math.Cos(num6) + num3 - num2 * Math.Sin(num6) - num3 * Math.Cos(num6);
				this.wedges[i * 16] = num2;
				this.wedges[i * 16 + 1] = num3;
				this.wedges[i * 16 + 2] = num21;
				this.wedges[i * 16 + 3] = num22;
				num17 = num2 - num4;
				num18 = num3 - num5;
				double num23 = num2 * Math.Cos(num6) + num3 * Math.Sin(num6) + num4 - num4 * Math.Cos(num6) - num5 * Math.Sin(num6);
				double num24 = -num2 * Math.Sin(num6) + num3 * Math.Cos(num6) + num5 + num4 * Math.Sin(num6) - num5 * Math.Cos(num6);
				this.wedges[i * 16 + 4] = num23;
				this.wedges[i * 16 + 5] = num24;
				this.wedges[i * 16 + 6] = num4;
				this.wedges[i * 16 + 7] = num5;
				num17 = num19 - this.petalx[i / 2];
				num18 = num20 - this.petaly[i / 2];
				double num25 = num19;
				double num26 = num20;
				for (int j = 1; j < 4; j++)
				{
					double num27 = num19 * Math.Cos((1.0471975511965976 - num6) * (double)j) + num20 * Math.Sin((1.0471975511965976 - num6) * (double)j) + this.petalx[i / 2] - this.petalx[i / 2] * Math.Cos((1.0471975511965976 - num6) * (double)j) - this.petaly[i / 2] * Math.Sin((1.0471975511965976 - num6) * (double)j);
					double num28 = -num19 * Math.Sin((1.0471975511965976 - num6) * (double)j) + num20 * Math.Cos((1.0471975511965976 - num6) * (double)j) + this.petaly[i / 2] + this.petalx[i / 2] * Math.Sin((1.0471975511965976 - num6) * (double)j) - this.petaly[i / 2] * Math.Cos((1.0471975511965976 - num6) * (double)j);
					this.wedges[i * 16 + 8 + 4 * (j - 1)] = num27;
					this.wedges[i * 16 + 9 + 4 * (j - 1)] = num28;
					this.wedges[i * 16 + 10 + 4 * (j - 1)] = num25;
					this.wedges[i * 16 + 11 + 4 * (j - 1)] = num26;
					num25 = num27;
					num26 = num28;
				}
				num25 = num19;
				num26 = num20;
				for (int j = 1; j < 4; j++)
				{
					double num29 = num19 * Math.Cos((1.0471975511965976 - num6) * (double)j) - num20 * Math.Sin((1.0471975511965976 - num6) * (double)j) + this.petalx[i / 2] - this.petalx[i / 2] * Math.Cos((1.0471975511965976 - num6) * (double)j) + this.petaly[i / 2] * Math.Sin((1.0471975511965976 - num6) * (double)j);
					double num30 = num19 * Math.Sin((1.0471975511965976 - num6) * (double)j) + num20 * Math.Cos((1.0471975511965976 - num6) * (double)j) + this.petaly[i / 2] - this.petalx[i / 2] * Math.Sin((1.0471975511965976 - num6) * (double)j) - this.petaly[i / 2] * Math.Cos((1.0471975511965976 - num6) * (double)j);
					this.wedges[i * 16 + 20 + 4 * (j - 1)] = num25;
					this.wedges[i * 16 + 21 + 4 * (j - 1)] = num26;
					this.wedges[i * 16 + 22 + 4 * (j - 1)] = num29;
					this.wedges[i * 16 + 23 + 4 * (j - 1)] = num30;
					num25 = num29;
					num26 = num30;
				}
				if (i == 0)
				{
					this.LineLineIntersection(num2, num3, num21, num22, num4, num5, num23, num24, ref array);
					if (array[0] == 1.0)
					{
						this.initialConvexPoly[0] = array[1];
						this.initialConvexPoly[1] = array[2];
						this.initialConvexPoly[2] = this.wedges[i * 16 + 16];
						this.initialConvexPoly[3] = this.wedges[i * 16 + 17];
						this.initialConvexPoly[4] = this.wedges[i * 16 + 12];
						this.initialConvexPoly[5] = this.wedges[i * 16 + 13];
						this.initialConvexPoly[6] = this.wedges[i * 16 + 8];
						this.initialConvexPoly[7] = this.wedges[i * 16 + 9];
						this.initialConvexPoly[8] = num19;
						this.initialConvexPoly[9] = num20;
						this.initialConvexPoly[10] = this.wedges[i * 16 + 22];
						this.initialConvexPoly[11] = this.wedges[i * 16 + 23];
						this.initialConvexPoly[12] = this.wedges[i * 16 + 26];
						this.initialConvexPoly[13] = this.wedges[i * 16 + 27];
						this.initialConvexPoly[14] = this.wedges[i * 16 + 30];
						this.initialConvexPoly[15] = this.wedges[i * 16 + 31];
					}
				}
				num2 = num4;
				num3 = num5;
				num4 = num9;
				num5 = num10;
			}
			if (numpoints != 0)
			{
				int num31 = (numpoints - 1) / 2 + 1;
				int num32 = 0;
				int k = 0;
				int i = 1;
				int numvertices = 8;
				for (int j = 0; j < 32; j += 4)
				{
					num = this.HalfPlaneIntersection(numvertices, ref this.initialConvexPoly, this.wedges[32 * num31 + j], this.wedges[32 * num31 + 1 + j], this.wedges[32 * num31 + 2 + j], this.wedges[32 * num31 + 3 + j]);
					if (num == 0)
					{
						return false;
					}
					numvertices = num;
				}
				for (k++; k < numpoints - 1; k++)
				{
					for (int j = 0; j < 32; j += 4)
					{
						num = this.HalfPlaneIntersection(numvertices, ref this.initialConvexPoly, this.wedges[32 * (i + num31 * num32) + j], this.wedges[32 * (i + num31 * num32) + 1 + j], this.wedges[32 * (i + num31 * num32) + 2 + j], this.wedges[32 * (i + num31 * num32) + 3 + j]);
						if (num == 0)
						{
							return false;
						}
						numvertices = num;
					}
					i += num32;
					num32 = (num32 + 1) % 2;
				}
				this.FindPolyCentroid(num, this.initialConvexPoly, ref newloc);
				if (!this.behavior.fixedArea)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600087B RID: 2171 RVA: 0x000B6B18 File Offset: 0x000B4D18
		private bool GetWedgeIntersection(int numpoints, double[] points, ref double[] newloc)
		{
			if (2 * numpoints > this.petalx.Length)
			{
				this.petalx = new double[2 * numpoints];
				this.petaly = new double[2 * numpoints];
				this.petalr = new double[2 * numpoints];
				this.wedges = new double[2 * numpoints * 20 + 40];
			}
			double[] array = new double[3];
			double[] array2 = new double[3];
			double[] array3 = new double[3];
			double[] array4 = new double[3];
			int num = 0;
			int num2 = 0;
			double num3 = points[2 * numpoints - 4];
			double num4 = points[2 * numpoints - 3];
			double num5 = points[2 * numpoints - 2];
			double num6 = points[2 * numpoints - 1];
			double num7 = this.behavior.MinAngle * 3.141592653589793 / 180.0;
			double num8 = Math.Sin(num7);
			double num9 = Math.Cos(num7);
			double num10 = this.behavior.MaxAngle * 3.141592653589793 / 180.0;
			double num11 = Math.Sin(num10);
			double num12 = Math.Cos(num10);
			double num13;
			double num14;
			if (this.behavior.goodAngle == 1.0)
			{
				num13 = 0.0;
				num14 = 0.0;
			}
			else
			{
				num13 = 0.5 / Math.Tan(num7);
				num14 = 0.5 / Math.Sin(num7);
			}
			for (int i = 0; i < numpoints * 2; i += 2)
			{
				double num15 = points[i];
				double num16 = points[i + 1];
				double num17 = num5 - num3;
				double num18 = num6 - num4;
				double num19 = Math.Sqrt(num17 * num17 + num18 * num18);
				this.petalx[i / 2] = num3 + 0.5 * num17 - num13 * num18;
				this.petaly[i / 2] = num4 + 0.5 * num18 + num13 * num17;
				this.petalr[i / 2] = num14 * num19;
				this.petalx[numpoints + i / 2] = this.petalx[i / 2];
				this.petaly[numpoints + i / 2] = this.petaly[i / 2];
				this.petalr[numpoints + i / 2] = this.petalr[i / 2];
				double num20 = (num3 + num5) / 2.0;
				double num21 = (num4 + num6) / 2.0;
				double num22 = Math.Sqrt((this.petalx[i / 2] - num20) * (this.petalx[i / 2] - num20) + (this.petaly[i / 2] - num21) * (this.petaly[i / 2] - num21));
				double num23 = (this.petalx[i / 2] - num20) / num22;
				double num24 = (this.petaly[i / 2] - num21) / num22;
				double num25 = this.petalx[i / 2] + num23 * this.petalr[i / 2];
				double num26 = this.petaly[i / 2] + num24 * this.petalr[i / 2];
				num23 = num5 - num3;
				num24 = num6 - num4;
				double num27 = num5 * num9 - num6 * num8 + num3 - num3 * num9 + num4 * num8;
				double num28 = num5 * num8 + num6 * num9 + num4 - num3 * num8 - num4 * num9;
				this.wedges[i * 20] = num3;
				this.wedges[i * 20 + 1] = num4;
				this.wedges[i * 20 + 2] = num27;
				this.wedges[i * 20 + 3] = num28;
				num23 = num3 - num5;
				num24 = num4 - num6;
				double num29 = num3 * num9 + num4 * num8 + num5 - num5 * num9 - num6 * num8;
				double num30 = -num3 * num8 + num4 * num9 + num6 + num5 * num8 - num6 * num9;
				this.wedges[i * 20 + 4] = num29;
				this.wedges[i * 20 + 5] = num30;
				this.wedges[i * 20 + 6] = num5;
				this.wedges[i * 20 + 7] = num6;
				num23 = num25 - this.petalx[i / 2];
				num24 = num26 - this.petaly[i / 2];
				double num31 = num25;
				double num32 = num26;
				num7 = 2.0 * this.behavior.MaxAngle + this.behavior.MinAngle - 180.0;
				double num33;
				double num34;
				if (num7 <= 0.0)
				{
					num2 = 4;
					num33 = 1.0;
					num34 = 1.0;
				}
				else if (num7 <= 5.0)
				{
					num2 = 6;
					num33 = 2.0;
					num34 = 2.0;
				}
				else if (num7 <= 10.0)
				{
					num2 = 8;
					num33 = 3.0;
					num34 = 3.0;
				}
				else
				{
					num2 = 10;
					num33 = 4.0;
					num34 = 4.0;
				}
				num7 = num7 * 3.141592653589793 / 180.0;
				int j = 1;
				while ((double)j < num33)
				{
					if (num33 != 1.0)
					{
						double num35 = num25 * Math.Cos(num7 / (num33 - 1.0) * (double)j) + num26 * Math.Sin(num7 / (num33 - 1.0) * (double)j) + this.petalx[i / 2] - this.petalx[i / 2] * Math.Cos(num7 / (num33 - 1.0) * (double)j) - this.petaly[i / 2] * Math.Sin(num7 / (num33 - 1.0) * (double)j);
						double num36 = -num25 * Math.Sin(num7 / (num33 - 1.0) * (double)j) + num26 * Math.Cos(num7 / (num33 - 1.0) * (double)j) + this.petaly[i / 2] + this.petalx[i / 2] * Math.Sin(num7 / (num33 - 1.0) * (double)j) - this.petaly[i / 2] * Math.Cos(num7 / (num33 - 1.0) * (double)j);
						this.wedges[i * 20 + 8 + 4 * (j - 1)] = num35;
						this.wedges[i * 20 + 9 + 4 * (j - 1)] = num36;
						this.wedges[i * 20 + 10 + 4 * (j - 1)] = num31;
						this.wedges[i * 20 + 11 + 4 * (j - 1)] = num32;
						num31 = num35;
						num32 = num36;
					}
					j++;
				}
				num23 = num3 - num5;
				num24 = num4 - num6;
				double num37 = num3 * num12 + num4 * num11 + num5 - num5 * num12 - num6 * num11;
				double num38 = -num3 * num11 + num4 * num12 + num6 + num5 * num11 - num6 * num12;
				this.wedges[i * 20 + 20] = num5;
				this.wedges[i * 20 + 21] = num6;
				this.wedges[i * 20 + 22] = num37;
				this.wedges[i * 20 + 23] = num38;
				num31 = num25;
				num32 = num26;
				j = 1;
				while ((double)j < num34)
				{
					if (num34 != 1.0)
					{
						double num39 = num25 * Math.Cos(num7 / (num34 - 1.0) * (double)j) - num26 * Math.Sin(num7 / (num34 - 1.0) * (double)j) + this.petalx[i / 2] - this.petalx[i / 2] * Math.Cos(num7 / (num34 - 1.0) * (double)j) + this.petaly[i / 2] * Math.Sin(num7 / (num34 - 1.0) * (double)j);
						double num40 = num25 * Math.Sin(num7 / (num34 - 1.0) * (double)j) + num26 * Math.Cos(num7 / (num34 - 1.0) * (double)j) + this.petaly[i / 2] - this.petalx[i / 2] * Math.Sin(num7 / (num34 - 1.0) * (double)j) - this.petaly[i / 2] * Math.Cos(num7 / (num34 - 1.0) * (double)j);
						this.wedges[i * 20 + 24 + 4 * (j - 1)] = num31;
						this.wedges[i * 20 + 25 + 4 * (j - 1)] = num32;
						this.wedges[i * 20 + 26 + 4 * (j - 1)] = num39;
						this.wedges[i * 20 + 27 + 4 * (j - 1)] = num40;
						num31 = num39;
						num32 = num40;
					}
					j++;
				}
				num23 = num5 - num3;
				num24 = num6 - num4;
				double num41 = num5 * num12 - num6 * num11 + num3 - num3 * num12 + num4 * num11;
				double num42 = num5 * num11 + num6 * num12 + num4 - num3 * num11 - num4 * num12;
				this.wedges[i * 20 + 36] = num41;
				this.wedges[i * 20 + 37] = num42;
				this.wedges[i * 20 + 38] = num3;
				this.wedges[i * 20 + 39] = num4;
				if (i == 0)
				{
					switch (num2)
					{
					case 4:
						this.LineLineIntersection(num3, num4, num27, num28, num5, num6, num29, num30, ref array);
						this.LineLineIntersection(num3, num4, num27, num28, num5, num6, num37, num38, ref array2);
						this.LineLineIntersection(num3, num4, num41, num42, num5, num6, num37, num38, ref array3);
						this.LineLineIntersection(num3, num4, num41, num42, num5, num6, num29, num30, ref array4);
						if (array[0] == 1.0 && array2[0] == 1.0 && array3[0] == 1.0 && array4[0] == 1.0)
						{
							this.initialConvexPoly[0] = array[1];
							this.initialConvexPoly[1] = array[2];
							this.initialConvexPoly[2] = array2[1];
							this.initialConvexPoly[3] = array2[2];
							this.initialConvexPoly[4] = array3[1];
							this.initialConvexPoly[5] = array3[2];
							this.initialConvexPoly[6] = array4[1];
							this.initialConvexPoly[7] = array4[2];
						}
						break;
					case 6:
						this.LineLineIntersection(num3, num4, num27, num28, num5, num6, num29, num30, ref array);
						this.LineLineIntersection(num3, num4, num27, num28, num5, num6, num37, num38, ref array2);
						this.LineLineIntersection(num3, num4, num41, num42, num5, num6, num29, num30, ref array3);
						if (array[0] == 1.0 && array2[0] == 1.0 && array3[0] == 1.0)
						{
							this.initialConvexPoly[0] = array[1];
							this.initialConvexPoly[1] = array[2];
							this.initialConvexPoly[2] = array2[1];
							this.initialConvexPoly[3] = array2[2];
							this.initialConvexPoly[4] = this.wedges[i * 20 + 8];
							this.initialConvexPoly[5] = this.wedges[i * 20 + 9];
							this.initialConvexPoly[6] = num25;
							this.initialConvexPoly[7] = num26;
							this.initialConvexPoly[8] = this.wedges[i * 20 + 26];
							this.initialConvexPoly[9] = this.wedges[i * 20 + 27];
							this.initialConvexPoly[10] = array3[1];
							this.initialConvexPoly[11] = array3[2];
						}
						break;
					case 8:
						this.LineLineIntersection(num3, num4, num27, num28, num5, num6, num29, num30, ref array);
						this.LineLineIntersection(num3, num4, num27, num28, num5, num6, num37, num38, ref array2);
						this.LineLineIntersection(num3, num4, num41, num42, num5, num6, num29, num30, ref array3);
						if (array[0] == 1.0 && array2[0] == 1.0 && array3[0] == 1.0)
						{
							this.initialConvexPoly[0] = array[1];
							this.initialConvexPoly[1] = array[2];
							this.initialConvexPoly[2] = array2[1];
							this.initialConvexPoly[3] = array2[2];
							this.initialConvexPoly[4] = this.wedges[i * 20 + 12];
							this.initialConvexPoly[5] = this.wedges[i * 20 + 13];
							this.initialConvexPoly[6] = this.wedges[i * 20 + 8];
							this.initialConvexPoly[7] = this.wedges[i * 20 + 9];
							this.initialConvexPoly[8] = num25;
							this.initialConvexPoly[9] = num26;
							this.initialConvexPoly[10] = this.wedges[i * 20 + 26];
							this.initialConvexPoly[11] = this.wedges[i * 20 + 27];
							this.initialConvexPoly[12] = this.wedges[i * 20 + 30];
							this.initialConvexPoly[13] = this.wedges[i * 20 + 31];
							this.initialConvexPoly[14] = array3[1];
							this.initialConvexPoly[15] = array3[2];
						}
						break;
					case 10:
						this.LineLineIntersection(num3, num4, num27, num28, num5, num6, num29, num30, ref array);
						this.LineLineIntersection(num3, num4, num27, num28, num5, num6, num37, num38, ref array2);
						this.LineLineIntersection(num3, num4, num41, num42, num5, num6, num29, num30, ref array3);
						if (array[0] == 1.0 && array2[0] == 1.0 && array3[0] == 1.0)
						{
							this.initialConvexPoly[0] = array[1];
							this.initialConvexPoly[1] = array[2];
							this.initialConvexPoly[2] = array2[1];
							this.initialConvexPoly[3] = array2[2];
							this.initialConvexPoly[4] = this.wedges[i * 20 + 16];
							this.initialConvexPoly[5] = this.wedges[i * 20 + 17];
							this.initialConvexPoly[6] = this.wedges[i * 20 + 12];
							this.initialConvexPoly[7] = this.wedges[i * 20 + 13];
							this.initialConvexPoly[8] = this.wedges[i * 20 + 8];
							this.initialConvexPoly[9] = this.wedges[i * 20 + 9];
							this.initialConvexPoly[10] = num25;
							this.initialConvexPoly[11] = num26;
							this.initialConvexPoly[12] = this.wedges[i * 20 + 28];
							this.initialConvexPoly[13] = this.wedges[i * 20 + 29];
							this.initialConvexPoly[14] = this.wedges[i * 20 + 32];
							this.initialConvexPoly[15] = this.wedges[i * 20 + 33];
							this.initialConvexPoly[16] = this.wedges[i * 20 + 34];
							this.initialConvexPoly[17] = this.wedges[i * 20 + 35];
							this.initialConvexPoly[18] = array3[1];
							this.initialConvexPoly[19] = array3[2];
						}
						break;
					}
				}
				num3 = num5;
				num4 = num6;
				num5 = num15;
				num6 = num16;
			}
			if (numpoints != 0)
			{
				int num43 = (numpoints - 1) / 2 + 1;
				int num44 = 0;
				int k = 0;
				int i = 1;
				int numvertices = num2;
				for (int j = 0; j < 40; j += 4)
				{
					if ((num2 != 4 || (j != 8 && j != 12 && j != 16 && j != 24 && j != 28 && j != 32)) && (num2 != 6 || (j != 12 && j != 16 && j != 28 && j != 32)) && (num2 != 8 || (j != 16 && j != 32)))
					{
						num = this.HalfPlaneIntersection(numvertices, ref this.initialConvexPoly, this.wedges[40 * num43 + j], this.wedges[40 * num43 + 1 + j], this.wedges[40 * num43 + 2 + j], this.wedges[40 * num43 + 3 + j]);
						if (num == 0)
						{
							return false;
						}
						numvertices = num;
					}
				}
				for (k++; k < numpoints - 1; k++)
				{
					for (int j = 0; j < 40; j += 4)
					{
						if ((num2 != 4 || (j != 8 && j != 12 && j != 16 && j != 24 && j != 28 && j != 32)) && (num2 != 6 || (j != 12 && j != 16 && j != 28 && j != 32)) && (num2 != 8 || (j != 16 && j != 32)))
						{
							num = this.HalfPlaneIntersection(numvertices, ref this.initialConvexPoly, this.wedges[40 * (i + num43 * num44) + j], this.wedges[40 * (i + num43 * num44) + 1 + j], this.wedges[40 * (i + num43 * num44) + 2 + j], this.wedges[40 * (i + num43 * num44) + 3 + j]);
							if (num == 0)
							{
								return false;
							}
							numvertices = num;
						}
					}
					i += num44;
					num44 = (num44 + 1) % 2;
				}
				this.FindPolyCentroid(num, this.initialConvexPoly, ref newloc);
				if (this.behavior.MaxAngle == 0.0)
				{
					return true;
				}
				int num45 = 0;
				for (int j = 0; j < numpoints * 2 - 2; j += 2)
				{
					if (this.IsBadTriangleAngle(newloc[0], newloc[1], points[j], points[j + 1], points[j + 2], points[j + 3]))
					{
						num45++;
					}
				}
				if (this.IsBadTriangleAngle(newloc[0], newloc[1], points[0], points[1], points[numpoints * 2 - 2], points[numpoints * 2 - 1]))
				{
					num45++;
				}
				if (num45 == 0)
				{
					return true;
				}
				int num46 = (numpoints <= 2) ? 20 : 30;
				for (int l = 0; l < 2 * numpoints; l += 2)
				{
					for (int m = 1; m < num46; m++)
					{
						newloc[0] = 0.0;
						newloc[1] = 0.0;
						for (i = 0; i < 2 * numpoints; i += 2)
						{
							double num47 = 1.0 / (double)numpoints;
							if (i == l)
							{
								newloc[0] = newloc[0] + 0.1 * (double)m * num47 * points[i];
								newloc[1] = newloc[1] + 0.1 * (double)m * num47 * points[i + 1];
							}
							else
							{
								num47 = (1.0 - 0.1 * (double)m * num47) / ((double)numpoints - 1.0);
								newloc[0] = newloc[0] + num47 * points[i];
								newloc[1] = newloc[1] + num47 * points[i + 1];
							}
						}
						num45 = 0;
						for (int j = 0; j < numpoints * 2 - 2; j += 2)
						{
							if (this.IsBadTriangleAngle(newloc[0], newloc[1], points[j], points[j + 1], points[j + 2], points[j + 3]))
							{
								num45++;
							}
						}
						if (this.IsBadTriangleAngle(newloc[0], newloc[1], points[0], points[1], points[numpoints * 2 - 2], points[numpoints * 2 - 1]))
						{
							num45++;
						}
						if (num45 == 0)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x0600087C RID: 2172 RVA: 0x000B7E80 File Offset: 0x000B6080
		private bool ValidPolygonAngles(int numpoints, double[] points)
		{
			for (int i = 0; i < numpoints; i++)
			{
				if (i == numpoints - 1)
				{
					if (this.IsBadPolygonAngle(points[i * 2], points[i * 2 + 1], points[0], points[1], points[2], points[3]))
					{
						return false;
					}
				}
				else if (i == numpoints - 2)
				{
					if (this.IsBadPolygonAngle(points[i * 2], points[i * 2 + 1], points[(i + 1) * 2], points[(i + 1) * 2 + 1], points[0], points[1]))
					{
						return false;
					}
				}
				else if (this.IsBadPolygonAngle(points[i * 2], points[i * 2 + 1], points[(i + 1) * 2], points[(i + 1) * 2 + 1], points[(i + 2) * 2], points[(i + 2) * 2 + 1]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600087D RID: 2173 RVA: 0x000B7F30 File Offset: 0x000B6130
		private bool IsBadPolygonAngle(double x1, double y1, double x2, double y2, double x3, double y3)
		{
			double num = x1 - x2;
			double num2 = y1 - y2;
			double num3 = x2 - x3;
			double num4 = y2 - y3;
			double num5 = x3 - x1;
			double num6 = y3 - y1;
			double num7 = num * num + num2 * num2;
			double num8 = num3 * num3 + num4 * num4;
			double num9 = num5 * num5 + num6 * num6;
			return Math.Acos((num7 + num8 - num9) / (2.0 * Math.Sqrt(num7) * Math.Sqrt(num8))) < 2.0 * Math.Acos(Math.Sqrt(this.behavior.goodAngle));
		}

		// Token: 0x0600087E RID: 2174 RVA: 0x000B7FC4 File Offset: 0x000B61C4
		private void LineLineIntersection(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, ref double[] p)
		{
			double num = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
			double num2 = (x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3);
			double num3 = (x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3);
			if (Math.Abs(num - 0.0) < 1E-50 && Math.Abs(num3 - 0.0) < 1E-50 && Math.Abs(num2 - 0.0) < 1E-50)
			{
				p[0] = 0.0;
				return;
			}
			if (Math.Abs(num - 0.0) < 1E-50)
			{
				p[0] = 0.0;
				return;
			}
			p[0] = 1.0;
			num2 /= num;
			num3 /= num;
			p[1] = x1 + num2 * (x2 - x1);
			p[2] = y1 + num2 * (y2 - y1);
		}

		// Token: 0x0600087F RID: 2175 RVA: 0x000B80C8 File Offset: 0x000B62C8
		private int HalfPlaneIntersection(int numvertices, ref double[] convexPoly, double x1, double y1, double x2, double y2)
		{
			double[] array = null;
			int num = 0;
			int num2 = 0;
			double num3 = x2 - x1;
			double num4 = y2 - y1;
			int num5 = this.SplitConvexPolygon(numvertices, convexPoly, x1, y1, x2, y2, this.polys);
			if (num5 == 3)
			{
				num = numvertices;
			}
			else
			{
				for (int i = 0; i < num5; i++)
				{
					double num6 = double.MaxValue;
					double num7 = double.MinValue;
					int num8 = 1;
					double num9;
					while ((double)num8 <= 2.0 * this.polys[i][0] - 1.0)
					{
						num9 = num3 * (this.polys[i][num8 + 1] - y1) - num4 * (this.polys[i][num8] - x1);
						num6 = ((num9 < num6) ? num9 : num6);
						num7 = ((num9 > num7) ? num9 : num7);
						num8 += 2;
					}
					num9 = ((Math.Abs(num6) > Math.Abs(num7)) ? num6 : num7);
					if (num9 > 0.0)
					{
						array = this.polys[i];
						num2 = 1;
						break;
					}
				}
				if (num2 == 1)
				{
					while ((double)num < array[0])
					{
						convexPoly[2 * num] = array[2 * num + 1];
						convexPoly[2 * num + 1] = array[2 * num + 2];
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x06000880 RID: 2176 RVA: 0x000B8210 File Offset: 0x000B6410
		private int SplitConvexPolygon(int numvertices, double[] convexPoly, double x1, double y1, double x2, double y2, double[][] polys)
		{
			int num = 0;
			double[] array = new double[3];
			int num2 = 0;
			int num3 = 0;
			double num4 = 1E-12;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			int num10 = 0;
			int num11 = 0;
			int num12 = 0;
			for (int i = 0; i < 2 * numvertices; i += 2)
			{
				int num13 = (i + 2 >= 2 * numvertices) ? 0 : (i + 2);
				this.LineLineSegmentIntersection(x1, y1, x2, y2, convexPoly[i], convexPoly[i + 1], convexPoly[num13], convexPoly[num13 + 1], ref array);
				if (Math.Abs(array[0] - 0.0) <= num4)
				{
					if (num == 1)
					{
						num3++;
						this.poly2[2 * num3 - 1] = convexPoly[num13];
						this.poly2[2 * num3] = convexPoly[num13 + 1];
					}
					else
					{
						num2++;
						this.poly1[2 * num2 - 1] = convexPoly[num13];
						this.poly1[2 * num2] = convexPoly[num13 + 1];
					}
					num5++;
				}
				else if (Math.Abs(array[0] - 2.0) <= num4)
				{
					num2++;
					this.poly1[2 * num2 - 1] = convexPoly[num13];
					this.poly1[2 * num2] = convexPoly[num13 + 1];
					num6++;
				}
				else
				{
					num7++;
					if (Math.Abs(array[1] - convexPoly[num13]) <= num4 && Math.Abs(array[2] - convexPoly[num13 + 1]) <= num4)
					{
						num8++;
						if (num == 1)
						{
							num3++;
							this.poly2[2 * num3 - 1] = convexPoly[num13];
							this.poly2[2 * num3] = convexPoly[num13 + 1];
							num2++;
							this.poly1[2 * num2 - 1] = convexPoly[num13];
							this.poly1[2 * num2] = convexPoly[num13 + 1];
							num++;
						}
						else if (num == 0)
						{
							num11++;
							num2++;
							this.poly1[2 * num2 - 1] = convexPoly[num13];
							this.poly1[2 * num2] = convexPoly[num13 + 1];
							if (i + 4 < 2 * numvertices)
							{
								int num14 = this.LinePointLocation(x1, y1, x2, y2, convexPoly[i], convexPoly[i + 1]);
								int num15 = this.LinePointLocation(x1, y1, x2, y2, convexPoly[i + 4], convexPoly[i + 5]);
								if (num14 != num15 && num14 != 0 && num15 != 0)
								{
									num12++;
									num3++;
									this.poly2[2 * num3 - 1] = convexPoly[num13];
									this.poly2[2 * num3] = convexPoly[num13 + 1];
									num++;
								}
							}
						}
					}
					else if (Math.Abs(array[1] - convexPoly[i]) > num4 || Math.Abs(array[2] - convexPoly[i + 1]) > num4)
					{
						num9++;
						num2++;
						this.poly1[2 * num2 - 1] = array[1];
						this.poly1[2 * num2] = array[2];
						num3++;
						this.poly2[2 * num3 - 1] = array[1];
						this.poly2[2 * num3] = array[2];
						if (num == 1)
						{
							num2++;
							this.poly1[2 * num2 - 1] = convexPoly[num13];
							this.poly1[2 * num2] = convexPoly[num13 + 1];
						}
						else if (num == 0)
						{
							num3++;
							this.poly2[2 * num3 - 1] = convexPoly[num13];
							this.poly2[2 * num3] = convexPoly[num13 + 1];
						}
						num++;
					}
					else
					{
						num10++;
						if (num == 1)
						{
							num3++;
							this.poly2[2 * num3 - 1] = convexPoly[num13];
							this.poly2[2 * num3] = convexPoly[num13 + 1];
						}
						else
						{
							num2++;
							this.poly1[2 * num2 - 1] = convexPoly[num13];
							this.poly1[2 * num2] = convexPoly[num13 + 1];
						}
					}
				}
			}
			int result;
			if (num != 0 && num != 2)
			{
				result = 3;
			}
			else
			{
				result = ((num == 0) ? 1 : 2);
				this.poly1[0] = (double)num2;
				this.poly2[0] = (double)num3;
				polys[0] = this.poly1;
				if (num == 2)
				{
					polys[1] = this.poly2;
				}
			}
			return result;
		}

		// Token: 0x06000881 RID: 2177 RVA: 0x000B85F0 File Offset: 0x000B67F0
		private int LinePointLocation(double x1, double y1, double x2, double y2, double x, double y)
		{
			if (Math.Atan((y2 - y1) / (x2 - x1)) * 180.0 / 3.141592653589793 == 90.0)
			{
				if (Math.Abs(x1 - x) <= 1E-11)
				{
					return 0;
				}
			}
			else if (Math.Abs(y1 + (y2 - y1) * (x - x1) / (x2 - x1) - y) <= 1E-50)
			{
				return 0;
			}
			double num = (x2 - x1) * (y - y1) - (y2 - y1) * (x - x1);
			if (Math.Abs(num - 0.0) <= 1E-11)
			{
				return 0;
			}
			if (num > 0.0)
			{
				return 1;
			}
			return 2;
		}

		// Token: 0x06000882 RID: 2178 RVA: 0x000B86A4 File Offset: 0x000B68A4
		private void LineLineSegmentIntersection(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, ref double[] p)
		{
			double num = 1E-13;
			double num2 = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
			double num3 = (x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3);
			double num4 = (x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3);
			if (Math.Abs(num2 - 0.0) < num)
			{
				if (Math.Abs(num4 - 0.0) < num && Math.Abs(num3 - 0.0) < num)
				{
					p[0] = 2.0;
					return;
				}
				p[0] = 0.0;
				return;
			}
			else
			{
				num4 /= num2;
				num3 /= num2;
				if (num4 < -num || num4 > 1.0 + num)
				{
					p[0] = 0.0;
					return;
				}
				p[0] = 1.0;
				p[1] = x1 + num3 * (x2 - x1);
				p[2] = y1 + num3 * (y2 - y1);
				return;
			}
		}

		// Token: 0x06000883 RID: 2179 RVA: 0x000B87A4 File Offset: 0x000B69A4
		private void FindPolyCentroid(int numpoints, double[] points, ref double[] centroid)
		{
			centroid[0] = 0.0;
			centroid[1] = 0.0;
			for (int i = 0; i < 2 * numpoints; i += 2)
			{
				centroid[0] = centroid[0] + points[i];
				centroid[1] = centroid[1] + points[i + 1];
			}
			centroid[0] = centroid[0] / (double)numpoints;
			centroid[1] = centroid[1] / (double)numpoints;
		}

		// Token: 0x06000884 RID: 2180 RVA: 0x000B880C File Offset: 0x000B6A0C
		private void CircleLineIntersection(double x1, double y1, double x2, double y2, double x3, double y3, double r, ref double[] p)
		{
			double num = (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);
			double num2 = 2.0 * ((x2 - x1) * (x1 - x3) + (y2 - y1) * (y1 - y3));
			double num3 = x3 * x3 + y3 * y3 + x1 * x1 + y1 * y1 - 2.0 * (x3 * x1 + y3 * y1) - r * r;
			double num4 = num2 * num2 - 4.0 * num * num3;
			if (num4 < 0.0)
			{
				p[0] = 0.0;
				return;
			}
			if (Math.Abs(num4 - 0.0) < 1E-50)
			{
				p[0] = 1.0;
				double num5 = -num2 / (2.0 * num);
				p[1] = x1 + num5 * (x2 - x1);
				p[2] = y1 + num5 * (y2 - y1);
				return;
			}
			if (num4 > 0.0 && Math.Abs(num - 0.0) >= 1E-50)
			{
				p[0] = 2.0;
				double num5 = (-num2 + Math.Sqrt(num4)) / (2.0 * num);
				p[1] = x1 + num5 * (x2 - x1);
				p[2] = y1 + num5 * (y2 - y1);
				num5 = (-num2 - Math.Sqrt(num4)) / (2.0 * num);
				p[3] = x1 + num5 * (x2 - x1);
				p[4] = y1 + num5 * (y2 - y1);
				return;
			}
			p[0] = 0.0;
		}

		// Token: 0x06000885 RID: 2181 RVA: 0x000B89A0 File Offset: 0x000B6BA0
		private bool ChooseCorrectPoint(double x1, double y1, double x2, double y2, double x3, double y3, bool isObtuse)
		{
			double num = (x2 - x3) * (x2 - x3) + (y2 - y3) * (y2 - y3);
			double num2 = (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);
			bool result;
			if (isObtuse)
			{
				result = (num2 >= num);
			}
			else
			{
				result = (num2 < num);
			}
			return result;
		}

		// Token: 0x06000886 RID: 2182 RVA: 0x000B89F0 File Offset: 0x000B6BF0
		private void PointBetweenPoints(double x1, double y1, double x2, double y2, double x, double y, ref double[] p)
		{
			if ((x2 - x) * (x2 - x) + (y2 - y) * (y2 - y) < (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1))
			{
				p[0] = 1.0;
				p[1] = (x - x2) * (x - x2) + (y - y2) * (y - y2);
				p[2] = x;
				p[3] = y;
				return;
			}
			p[0] = 0.0;
			p[1] = 0.0;
			p[2] = 0.0;
			p[3] = 0.0;
		}

		// Token: 0x06000887 RID: 2183 RVA: 0x000B8A94 File Offset: 0x000B6C94
		private bool IsBadTriangleAngle(double x1, double y1, double x2, double y2, double x3, double y3)
		{
			double num = x1 - x2;
			double num2 = y1 - y2;
			double num3 = x2 - x3;
			double num4 = y2 - y3;
			double num5 = x3 - x1;
			double num6 = y3 - y1;
			double num7 = num * num;
			double num8 = num2 * num2;
			double num9 = num3 * num3;
			double num10 = num4 * num4;
			double num11 = num5 * num5;
			double num12 = num6 * num6;
			double num13 = num7 + num8;
			double num14 = num9 + num10;
			double num15 = num11 + num12;
			double num16;
			if (num13 < num14 && num13 < num15)
			{
				num16 = num3 * num5 + num4 * num6;
				num16 = num16 * num16 / (num14 * num15);
			}
			else if (num14 < num15)
			{
				num16 = num * num5 + num2 * num6;
				num16 = num16 * num16 / (num13 * num15);
			}
			else
			{
				num16 = num * num3 + num2 * num4;
				num16 = num16 * num16 / (num13 * num14);
			}
			double num17;
			if (num13 > num14 && num13 > num15)
			{
				num17 = (num14 + num15 - num13) / (2.0 * Math.Sqrt(num14 * num15));
			}
			else if (num14 > num15)
			{
				num17 = (num13 + num15 - num14) / (2.0 * Math.Sqrt(num13 * num15));
			}
			else
			{
				num17 = (num13 + num14 - num15) / (2.0 * Math.Sqrt(num13 * num14));
			}
			return num16 > this.behavior.goodAngle || (this.behavior.MaxAngle != 0.0 && num17 < this.behavior.maxGoodAngle);
		}

		// Token: 0x06000888 RID: 2184 RVA: 0x000B8BFC File Offset: 0x000B6DFC
		private double MinDistanceToNeighbor(double newlocX, double newlocY, ref Otri searchtri)
		{
			Otri otri = default(Otri);
			LocateResult locateResult = LocateResult.Outside;
			Point point = new Point(newlocX, newlocY);
			Vertex vertex = searchtri.Org();
			Vertex vertex2 = searchtri.Dest();
			if (vertex.x == point.x && vertex.y == point.y)
			{
				locateResult = LocateResult.OnVertex;
				searchtri.Copy(ref otri);
			}
			else if (vertex2.x == point.x && vertex2.y == point.y)
			{
				searchtri.Lnext();
				locateResult = LocateResult.OnVertex;
				searchtri.Copy(ref otri);
			}
			else
			{
				double num = this.predicates.CounterClockwise(vertex, vertex2, point);
				if (num < 0.0)
				{
					searchtri.Sym();
					searchtri.Copy(ref otri);
					locateResult = this.mesh.locator.PreciseLocate(point, ref otri, false);
				}
				else if (num == 0.0)
				{
					if (vertex.x < point.x == point.x < vertex2.x && vertex.y < point.y == point.y < vertex2.y)
					{
						locateResult = LocateResult.OnEdge;
						searchtri.Copy(ref otri);
					}
				}
				else
				{
					searchtri.Copy(ref otri);
					locateResult = this.mesh.locator.PreciseLocate(point, ref otri, false);
				}
			}
			if (locateResult == LocateResult.OnVertex || locateResult == LocateResult.Outside)
			{
				return 0.0;
			}
			Vertex vertex3 = otri.Org();
			Vertex vertex4 = otri.Dest();
			Vertex vertex5 = otri.Apex();
			double num2 = (vertex3.x - point.x) * (vertex3.x - point.x) + (vertex3.y - point.y) * (vertex3.y - point.y);
			double num3 = (vertex4.x - point.x) * (vertex4.x - point.x) + (vertex4.y - point.y) * (vertex4.y - point.y);
			double num4 = (vertex5.x - point.x) * (vertex5.x - point.x) + (vertex5.y - point.y) * (vertex5.y - point.y);
			if (num2 <= num3 && num2 <= num4)
			{
				return num2;
			}
			if (num3 <= num4)
			{
				return num3;
			}
			return num4;
		}

		// Token: 0x040009B3 RID: 2483
		private const double EPS = 1E-50;

		// Token: 0x040009B4 RID: 2484
		private IPredicates predicates;

		// Token: 0x040009B5 RID: 2485
		private Mesh mesh;

		// Token: 0x040009B6 RID: 2486
		private Behavior behavior;

		// Token: 0x040009B7 RID: 2487
		private double[] petalx = new double[20];

		// Token: 0x040009B8 RID: 2488
		private double[] petaly = new double[20];

		// Token: 0x040009B9 RID: 2489
		private double[] petalr = new double[20];

		// Token: 0x040009BA RID: 2490
		private double[] wedges = new double[500];

		// Token: 0x040009BB RID: 2491
		private double[] initialConvexPoly = new double[500];

		// Token: 0x040009BC RID: 2492
		private double[] points_p = new double[500];

		// Token: 0x040009BD RID: 2493
		private double[] points_q = new double[500];

		// Token: 0x040009BE RID: 2494
		private double[] points_r = new double[500];

		// Token: 0x040009BF RID: 2495
		private double[] poly1 = new double[100];

		// Token: 0x040009C0 RID: 2496
		private double[] poly2 = new double[100];

		// Token: 0x040009C1 RID: 2497
		private double[][] polys = new double[3][];
	}
}
