using System;
using TriangleNet.Geometry;
using TriangleNet.Tools;

namespace TriangleNet
{
	// Token: 0x020000EF RID: 239
	public class RobustPredicates : IPredicates
	{
		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x06000889 RID: 2185 RVA: 0x000B8E54 File Offset: 0x000B7054
		public static RobustPredicates Default
		{
			get
			{
				if (RobustPredicates._default == null)
				{
					object obj = RobustPredicates.creationLock;
					lock (obj)
					{
						if (RobustPredicates._default == null)
						{
							RobustPredicates._default = new RobustPredicates();
						}
					}
				}
				return RobustPredicates._default;
			}
		}

		// Token: 0x0600088A RID: 2186 RVA: 0x000B8EAC File Offset: 0x000B70AC
		static RobustPredicates()
		{
			bool flag = true;
			double num = 0.5;
			RobustPredicates.epsilon = 1.0;
			RobustPredicates.splitter = 1.0;
			double num2 = 1.0;
			double num3;
			do
			{
				num3 = num2;
				RobustPredicates.epsilon *= num;
				if (flag)
				{
					RobustPredicates.splitter *= 2.0;
				}
				flag = !flag;
				num2 = 1.0 + RobustPredicates.epsilon;
			}
			while (num2 != 1.0 && num2 != num3);
			RobustPredicates.splitter += 1.0;
			RobustPredicates.resulterrbound = (3.0 + 8.0 * RobustPredicates.epsilon) * RobustPredicates.epsilon;
			RobustPredicates.ccwerrboundA = (3.0 + 16.0 * RobustPredicates.epsilon) * RobustPredicates.epsilon;
			RobustPredicates.ccwerrboundB = (2.0 + 12.0 * RobustPredicates.epsilon) * RobustPredicates.epsilon;
			RobustPredicates.ccwerrboundC = (9.0 + 64.0 * RobustPredicates.epsilon) * RobustPredicates.epsilon * RobustPredicates.epsilon;
			RobustPredicates.iccerrboundA = (10.0 + 96.0 * RobustPredicates.epsilon) * RobustPredicates.epsilon;
			RobustPredicates.iccerrboundB = (4.0 + 48.0 * RobustPredicates.epsilon) * RobustPredicates.epsilon;
			RobustPredicates.iccerrboundC = (44.0 + 576.0 * RobustPredicates.epsilon) * RobustPredicates.epsilon * RobustPredicates.epsilon;
		}

		// Token: 0x0600088B RID: 2187 RVA: 0x000489E5 File Offset: 0x00046BE5
		public RobustPredicates()
		{
			this.AllocateWorkspace();
		}

		// Token: 0x0600088C RID: 2188 RVA: 0x000B905C File Offset: 0x000B725C
		public double CounterClockwise(Point pa, Point pb, Point pc)
		{
			Statistic.CounterClockwiseCount += 1L;
			double num = (pa.x - pc.x) * (pb.y - pc.y);
			double num2 = (pa.y - pc.y) * (pb.x - pc.x);
			double num3 = num - num2;
			if (Behavior.NoExact)
			{
				return num3;
			}
			double num4;
			if (num > 0.0)
			{
				if (num2 <= 0.0)
				{
					return num3;
				}
				num4 = num + num2;
			}
			else
			{
				if (num >= 0.0)
				{
					return num3;
				}
				if (num2 >= 0.0)
				{
					return num3;
				}
				num4 = -num - num2;
			}
			double num5 = RobustPredicates.ccwerrboundA * num4;
			if (num3 >= num5 || -num3 >= num5)
			{
				return num3;
			}
			Statistic.CounterClockwiseAdaptCount += 1L;
			return this.CounterClockwiseAdapt(pa, pb, pc, num4);
		}

		// Token: 0x0600088D RID: 2189 RVA: 0x000B912C File Offset: 0x000B732C
		public double InCircle(Point pa, Point pb, Point pc, Point pd)
		{
			Statistic.InCircleCount += 1L;
			double num = pa.x - pd.x;
			double num2 = pb.x - pd.x;
			double num3 = pc.x - pd.x;
			double num4 = pa.y - pd.y;
			double num5 = pb.y - pd.y;
			double num6 = pc.y - pd.y;
			double num7 = num2 * num6;
			double num8 = num3 * num5;
			double num9 = num * num + num4 * num4;
			double num10 = num3 * num4;
			double num11 = num * num6;
			double num12 = num2 * num2 + num5 * num5;
			double num13 = num * num5;
			double num14 = num2 * num4;
			double num15 = num3 * num3 + num6 * num6;
			double num16 = num9 * (num7 - num8) + num12 * (num10 - num11) + num15 * (num13 - num14);
			if (Behavior.NoExact)
			{
				return num16;
			}
			double num17 = (Math.Abs(num7) + Math.Abs(num8)) * num9 + (Math.Abs(num10) + Math.Abs(num11)) * num12 + (Math.Abs(num13) + Math.Abs(num14)) * num15;
			double num18 = RobustPredicates.iccerrboundA * num17;
			if (num16 > num18 || -num16 > num18)
			{
				return num16;
			}
			Statistic.InCircleAdaptCount += 1L;
			return this.InCircleAdapt(pa, pb, pc, pd, num17);
		}

		// Token: 0x0600088E RID: 2190 RVA: 0x000489F3 File Offset: 0x00046BF3
		public double NonRegular(Point pa, Point pb, Point pc, Point pd)
		{
			return this.InCircle(pa, pb, pc, pd);
		}

		// Token: 0x0600088F RID: 2191 RVA: 0x000B9270 File Offset: 0x000B7470
		public Point FindCircumcenter(Point org, Point dest, Point apex, ref double xi, ref double eta, double offconstant)
		{
			Statistic.CircumcenterCount += 1L;
			double num = dest.x - org.x;
			double num2 = dest.y - org.y;
			double num3 = apex.x - org.x;
			double num4 = apex.y - org.y;
			double num5 = num * num + num2 * num2;
			double num6 = num3 * num3 + num4 * num4;
			double num7 = (dest.x - apex.x) * (dest.x - apex.x) + (dest.y - apex.y) * (dest.y - apex.y);
			double num8;
			if (Behavior.NoExact)
			{
				num8 = 0.5 / (num * num4 - num3 * num2);
			}
			else
			{
				num8 = 0.5 / this.CounterClockwise(dest, apex, org);
				Statistic.CounterClockwiseCount -= 1L;
			}
			double num9 = (num4 * num5 - num2 * num6) * num8;
			double num10 = (num * num6 - num3 * num5) * num8;
			if (num5 < num6 && num5 < num7)
			{
				if (offconstant > 0.0)
				{
					double num11 = 0.5 * num - offconstant * num2;
					double num12 = 0.5 * num2 + offconstant * num;
					if (num11 * num11 + num12 * num12 < num9 * num9 + num10 * num10)
					{
						num9 = num11;
						num10 = num12;
					}
				}
			}
			else if (num6 < num7)
			{
				if (offconstant > 0.0)
				{
					double num11 = 0.5 * num3 + offconstant * num4;
					double num12 = 0.5 * num4 - offconstant * num3;
					if (num11 * num11 + num12 * num12 < num9 * num9 + num10 * num10)
					{
						num9 = num11;
						num10 = num12;
					}
				}
			}
			else if (offconstant > 0.0)
			{
				double num11 = 0.5 * (apex.x - dest.x) - offconstant * (apex.y - dest.y);
				double num12 = 0.5 * (apex.y - dest.y) + offconstant * (apex.x - dest.x);
				if (num11 * num11 + num12 * num12 < (num9 - num) * (num9 - num) + (num10 - num2) * (num10 - num2))
				{
					num9 = num + num11;
					num10 = num2 + num12;
				}
			}
			xi = (num4 * num9 - num3 * num10) * (2.0 * num8);
			eta = (num * num10 - num2 * num9) * (2.0 * num8);
			return new Point(org.x + num9, org.y + num10);
		}

		// Token: 0x06000890 RID: 2192 RVA: 0x000B950C File Offset: 0x000B770C
		public Point FindCircumcenter(Point org, Point dest, Point apex, ref double xi, ref double eta)
		{
			Statistic.CircumcenterCount += 1L;
			double num = dest.x - org.x;
			double num2 = dest.y - org.y;
			double num3 = apex.x - org.x;
			double num4 = apex.y - org.y;
			double num5 = num * num + num2 * num2;
			double num6 = num3 * num3 + num4 * num4;
			double num7;
			if (Behavior.NoExact)
			{
				num7 = 0.5 / (num * num4 - num3 * num2);
			}
			else
			{
				num7 = 0.5 / this.CounterClockwise(dest, apex, org);
				Statistic.CounterClockwiseCount -= 1L;
			}
			double num8 = (num4 * num5 - num2 * num6) * num7;
			double num9 = (num * num6 - num3 * num5) * num7;
			xi = (num4 * num8 - num3 * num9) * (2.0 * num7);
			eta = (num * num9 - num2 * num8) * (2.0 * num7);
			return new Point(org.x + num8, org.y + num9);
		}

		// Token: 0x06000891 RID: 2193 RVA: 0x000B9614 File Offset: 0x000B7814
		private int FastExpansionSumZeroElim(int elen, double[] e, int flen, double[] f, double[] h)
		{
			double num = e[0];
			double num2 = f[0];
			int j;
			int i = j = 0;
			double num3;
			if (num2 > num == num2 > -num)
			{
				num3 = num;
				num = e[++j];
			}
			else
			{
				num3 = num2;
				num2 = f[++i];
			}
			int num4 = 0;
			if (j < elen && i < flen)
			{
				double num5;
				double num7;
				if (num2 > num == num2 > -num)
				{
					num5 = num + num3;
					double num6 = num5 - num;
					num7 = num3 - num6;
					num = e[++j];
				}
				else
				{
					num5 = num2 + num3;
					double num6 = num5 - num2;
					num7 = num3 - num6;
					num2 = f[++i];
				}
				num3 = num5;
				if (num7 != 0.0)
				{
					h[num4++] = num7;
				}
				while (j < elen)
				{
					if (i >= flen)
					{
						break;
					}
					if (num2 > num == num2 > -num)
					{
						num5 = num3 + num;
						double num6 = num5 - num3;
						double num8 = num5 - num6;
						double num9 = num - num6;
						num7 = num3 - num8 + num9;
						num = e[++j];
					}
					else
					{
						num5 = num3 + num2;
						double num6 = num5 - num3;
						double num8 = num5 - num6;
						double num9 = num2 - num6;
						num7 = num3 - num8 + num9;
						num2 = f[++i];
					}
					num3 = num5;
					if (num7 != 0.0)
					{
						h[num4++] = num7;
					}
				}
			}
			while (j < elen)
			{
				double num5 = num3 + num;
				double num6 = num5 - num3;
				double num8 = num5 - num6;
				double num9 = num - num6;
				double num7 = num3 - num8 + num9;
				num = e[++j];
				num3 = num5;
				if (num7 != 0.0)
				{
					h[num4++] = num7;
				}
			}
			while (i < flen)
			{
				double num5 = num3 + num2;
				double num6 = num5 - num3;
				double num8 = num5 - num6;
				double num9 = num2 - num6;
				double num7 = num3 - num8 + num9;
				num2 = f[++i];
				num3 = num5;
				if (num7 != 0.0)
				{
					h[num4++] = num7;
				}
			}
			if (num3 != 0.0 || num4 == 0)
			{
				h[num4++] = num3;
			}
			return num4;
		}

		// Token: 0x06000892 RID: 2194 RVA: 0x000B9814 File Offset: 0x000B7A14
		private int ScaleExpansionZeroElim(int elen, double[] e, double b, double[] h)
		{
			double num = RobustPredicates.splitter * b;
			double num2 = num - b;
			double num3 = num - num2;
			double num4 = b - num3;
			double num5 = e[0] * b;
			double num6 = RobustPredicates.splitter * e[0];
			num2 = num6 - e[0];
			double num7 = num6 - num2;
			double num8 = e[0] - num7;
			double num9 = num5 - num7 * num3 - num8 * num3 - num7 * num4;
			double num10 = num8 * num4 - num9;
			int num11 = 0;
			if (num10 != 0.0)
			{
				h[num11++] = num10;
			}
			for (int i = 1; i < elen; i++)
			{
				double num12 = e[i];
				double num13 = num12 * b;
				double num14 = RobustPredicates.splitter * num12;
				num2 = num14 - num12;
				num7 = num14 - num2;
				num8 = num12 - num7;
				num9 = num13 - num7 * num3 - num8 * num3 - num7 * num4;
				double num15 = num8 * num4 - num9;
				double num16 = num5 + num15;
				double num17 = num16 - num5;
				double num18 = num16 - num17;
				double num19 = num15 - num17;
				num10 = num5 - num18 + num19;
				if (num10 != 0.0)
				{
					h[num11++] = num10;
				}
				num5 = num13 + num16;
				num17 = num5 - num13;
				num10 = num16 - num17;
				if (num10 != 0.0)
				{
					h[num11++] = num10;
				}
			}
			if (num5 != 0.0 || num11 == 0)
			{
				h[num11++] = num5;
			}
			return num11;
		}

		// Token: 0x06000893 RID: 2195 RVA: 0x000B9970 File Offset: 0x000B7B70
		private double Estimate(int elen, double[] e)
		{
			double num = e[0];
			for (int i = 1; i < elen; i++)
			{
				num += e[i];
			}
			return num;
		}

		// Token: 0x06000894 RID: 2196 RVA: 0x000B9994 File Offset: 0x000B7B94
		private double CounterClockwiseAdapt(Point pa, Point pb, Point pc, double detsum)
		{
			double[] array = new double[5];
			double[] array2 = new double[5];
			double[] array3 = new double[8];
			double[] array4 = new double[12];
			double[] array5 = new double[16];
			double num = pa.x - pc.x;
			double num2 = pb.x - pc.x;
			double num3 = pa.y - pc.y;
			double num4 = pb.y - pc.y;
			double num5 = num * num4;
			double num6 = RobustPredicates.splitter * num;
			double num7 = num6 - num;
			double num8 = num6 - num7;
			double num9 = num - num8;
			double num10 = RobustPredicates.splitter * num4;
			num7 = num10 - num4;
			double num11 = num10 - num7;
			double num12 = num4 - num11;
			double num13 = num5 - num8 * num11 - num9 * num11 - num8 * num12;
			double num14 = num9 * num12 - num13;
			double num15 = num3 * num2;
			double num16 = RobustPredicates.splitter * num3;
			num7 = num16 - num3;
			num8 = num16 - num7;
			num9 = num3 - num8;
			double num17 = RobustPredicates.splitter * num2;
			num7 = num17 - num2;
			num11 = num17 - num7;
			num12 = num2 - num11;
			num13 = num15 - num8 * num11 - num9 * num11 - num8 * num12;
			double num18 = num9 * num12 - num13;
			double num19 = num14 - num18;
			double num20 = num14 - num19;
			double num21 = num19 + num20;
			double num22 = num20 - num18;
			double num23 = num14 - num21;
			array[0] = num23 + num22;
			double num24 = num5 + num19;
			num20 = num24 - num5;
			num21 = num24 - num20;
			num22 = num19 - num20;
			num23 = num5 - num21;
			double num25 = num23 + num22;
			num19 = num25 - num15;
			num20 = num25 - num19;
			num21 = num19 + num20;
			num22 = num20 - num15;
			num23 = num25 - num21;
			array[1] = num23 + num22;
			double num26 = num24 + num19;
			num20 = num26 - num24;
			num21 = num26 - num20;
			num22 = num19 - num20;
			num23 = num24 - num21;
			array[2] = num23 + num22;
			array[3] = num26;
			double num27 = this.Estimate(4, array);
			double num28 = RobustPredicates.ccwerrboundB * detsum;
			if (num27 >= num28 || -num27 >= num28)
			{
				return num27;
			}
			num20 = pa.x - num;
			num21 = num + num20;
			num22 = num20 - pc.x;
			num23 = pa.x - num21;
			double num29 = num23 + num22;
			num20 = pb.x - num2;
			num21 = num2 + num20;
			num22 = num20 - pc.x;
			num23 = pb.x - num21;
			double num30 = num23 + num22;
			num20 = pa.y - num3;
			num21 = num3 + num20;
			num22 = num20 - pc.y;
			num23 = pa.y - num21;
			double num31 = num23 + num22;
			num20 = pb.y - num4;
			num21 = num4 + num20;
			num22 = num20 - pc.y;
			num23 = pb.y - num21;
			double num32 = num23 + num22;
			if (num29 == 0.0 && num31 == 0.0 && num30 == 0.0 && num32 == 0.0)
			{
				return num27;
			}
			num28 = RobustPredicates.ccwerrboundC * detsum + RobustPredicates.resulterrbound * ((num27 >= 0.0) ? num27 : (-num27));
			num27 += num * num32 + num4 * num29 - (num3 * num30 + num2 * num31);
			if (num27 >= num28 || -num27 >= num28)
			{
				return num27;
			}
			double num33 = num29 * num4;
			double num34 = RobustPredicates.splitter * num29;
			num7 = num34 - num29;
			num8 = num34 - num7;
			num9 = num29 - num8;
			double num35 = RobustPredicates.splitter * num4;
			num7 = num35 - num4;
			num11 = num35 - num7;
			num12 = num4 - num11;
			num13 = num33 - num8 * num11 - num9 * num11 - num8 * num12;
			double num36 = num9 * num12 - num13;
			double num37 = num31 * num2;
			double num38 = RobustPredicates.splitter * num31;
			num7 = num38 - num31;
			num8 = num38 - num7;
			num9 = num31 - num8;
			double num39 = RobustPredicates.splitter * num2;
			num7 = num39 - num2;
			num11 = num39 - num7;
			num12 = num2 - num11;
			num13 = num37 - num8 * num11 - num9 * num11 - num8 * num12;
			double num40 = num9 * num12 - num13;
			num19 = num36 - num40;
			num20 = num36 - num19;
			num21 = num19 + num20;
			num22 = num20 - num40;
			num23 = num36 - num21;
			array2[0] = num23 + num22;
			num24 = num33 + num19;
			num20 = num24 - num33;
			num21 = num24 - num20;
			num22 = num19 - num20;
			num23 = num33 - num21;
			double num41 = num23 + num22;
			num19 = num41 - num37;
			num20 = num41 - num19;
			num21 = num19 + num20;
			num22 = num20 - num37;
			num23 = num41 - num21;
			array2[1] = num23 + num22;
			double num42 = num24 + num19;
			num20 = num42 - num24;
			num21 = num42 - num20;
			num22 = num19 - num20;
			num23 = num24 - num21;
			array2[2] = num23 + num22;
			array2[3] = num42;
			int elen = this.FastExpansionSumZeroElim(4, array, 4, array2, array3);
			num33 = num * num32;
			double num43 = RobustPredicates.splitter * num;
			num7 = num43 - num;
			num8 = num43 - num7;
			num9 = num - num8;
			double num44 = RobustPredicates.splitter * num32;
			num7 = num44 - num32;
			num11 = num44 - num7;
			num12 = num32 - num11;
			num13 = num33 - num8 * num11 - num9 * num11 - num8 * num12;
			double num45 = num9 * num12 - num13;
			num37 = num3 * num30;
			double num46 = RobustPredicates.splitter * num3;
			num7 = num46 - num3;
			num8 = num46 - num7;
			num9 = num3 - num8;
			double num47 = RobustPredicates.splitter * num30;
			num7 = num47 - num30;
			num11 = num47 - num7;
			num12 = num30 - num11;
			num13 = num37 - num8 * num11 - num9 * num11 - num8 * num12;
			num40 = num9 * num12 - num13;
			num19 = num45 - num40;
			num20 = num45 - num19;
			num21 = num19 + num20;
			num22 = num20 - num40;
			num23 = num45 - num21;
			array2[0] = num23 + num22;
			num24 = num33 + num19;
			num20 = num24 - num33;
			num21 = num24 - num20;
			num22 = num19 - num20;
			num23 = num33 - num21;
			double num48 = num23 + num22;
			num19 = num48 - num37;
			num20 = num48 - num19;
			num21 = num19 + num20;
			num22 = num20 - num37;
			num23 = num48 - num21;
			array2[1] = num23 + num22;
			num42 = num24 + num19;
			num20 = num42 - num24;
			num21 = num42 - num20;
			num22 = num19 - num20;
			num23 = num24 - num21;
			array2[2] = num23 + num22;
			array2[3] = num42;
			int elen2 = this.FastExpansionSumZeroElim(elen, array3, 4, array2, array4);
			num33 = num29 * num32;
			double num49 = RobustPredicates.splitter * num29;
			num7 = num49 - num29;
			num8 = num49 - num7;
			num9 = num29 - num8;
			double num50 = RobustPredicates.splitter * num32;
			num7 = num50 - num32;
			num11 = num50 - num7;
			num12 = num32 - num11;
			num13 = num33 - num8 * num11 - num9 * num11 - num8 * num12;
			double num51 = num9 * num12 - num13;
			num37 = num31 * num30;
			double num52 = RobustPredicates.splitter * num31;
			num7 = num52 - num31;
			num8 = num52 - num7;
			num9 = num31 - num8;
			double num53 = RobustPredicates.splitter * num30;
			num7 = num53 - num30;
			num11 = num53 - num7;
			num12 = num30 - num11;
			num13 = num37 - num8 * num11 - num9 * num11 - num8 * num12;
			num40 = num9 * num12 - num13;
			num19 = num51 - num40;
			num20 = num51 - num19;
			num21 = num19 + num20;
			num22 = num20 - num40;
			num23 = num51 - num21;
			array2[0] = num23 + num22;
			num24 = num33 + num19;
			num20 = num24 - num33;
			num21 = num24 - num20;
			num22 = num19 - num20;
			num23 = num33 - num21;
			double num54 = num23 + num22;
			num19 = num54 - num37;
			num20 = num54 - num19;
			num21 = num19 + num20;
			num22 = num20 - num37;
			num23 = num54 - num21;
			array2[1] = num23 + num22;
			num42 = num24 + num19;
			num20 = num42 - num24;
			num21 = num42 - num20;
			num22 = num19 - num20;
			num23 = num24 - num21;
			array2[2] = num23 + num22;
			array2[3] = num42;
			int num55 = this.FastExpansionSumZeroElim(elen2, array4, 4, array2, array5);
			return array5[num55 - 1];
		}

		// Token: 0x06000895 RID: 2197 RVA: 0x000BA144 File Offset: 0x000B8344
		private double InCircleAdapt(Point pa, Point pb, Point pc, Point pd, double permanent)
		{
			double[] array = new double[4];
			double[] array2 = new double[4];
			double[] array3 = new double[4];
			double[] array4 = new double[4];
			double[] array5 = new double[4];
			double[] array6 = new double[4];
			double[] array7 = new double[5];
			double[] array8 = new double[5];
			double[] array9 = new double[8];
			double[] array10 = new double[8];
			double[] array11 = new double[8];
			double[] array12 = new double[8];
			double[] array13 = new double[8];
			double[] array14 = new double[8];
			double[] array15 = new double[8];
			double[] array16 = new double[8];
			double[] array17 = new double[8];
			double[] array18 = new double[8];
			double[] array19 = new double[8];
			double[] array20 = new double[8];
			double[] array21 = new double[8];
			double[] array22 = new double[8];
			double[] array23 = new double[8];
			double[] array24 = new double[8];
			double[] array25 = new double[8];
			double[] array26 = new double[8];
			int elen = 0;
			int elen2 = 0;
			int elen3 = 0;
			int elen4 = 0;
			int elen5 = 0;
			int elen6 = 0;
			double[] array27 = new double[16];
			double[] array28 = new double[16];
			double[] array29 = new double[16];
			double[] array30 = new double[16];
			double[] array31 = new double[16];
			double[] array32 = new double[16];
			double[] array33 = new double[8];
			double[] array34 = new double[8];
			double[] array35 = new double[8];
			double[] array36 = new double[8];
			double[] array37 = new double[8];
			double[] array38 = new double[8];
			double[] array39 = new double[8];
			double[] array40 = new double[8];
			double[] array41 = new double[8];
			double[] array42 = new double[4];
			double[] array43 = new double[4];
			double[] array44 = new double[4];
			double num = pa.x - pd.x;
			double num2 = pb.x - pd.x;
			double num3 = pc.x - pd.x;
			double num4 = pa.y - pd.y;
			double num5 = pb.y - pd.y;
			double num6 = pc.y - pd.y;
			num = pa.x - pd.x;
			num2 = pb.x - pd.x;
			num3 = pc.x - pd.x;
			num4 = pa.y - pd.y;
			num5 = pb.y - pd.y;
			num6 = pc.y - pd.y;
			double num7 = num2 * num6;
			double num8 = RobustPredicates.splitter * num2;
			double num9 = num8 - num2;
			double num10 = num8 - num9;
			double num11 = num2 - num10;
			double num12 = RobustPredicates.splitter * num6;
			num9 = num12 - num6;
			double num13 = num12 - num9;
			double num14 = num6 - num13;
			double num15 = num7 - num10 * num13 - num11 * num13 - num10 * num14;
			double num16 = num11 * num14 - num15;
			double num17 = num3 * num5;
			double num18 = RobustPredicates.splitter * num3;
			num9 = num18 - num3;
			num10 = num18 - num9;
			num11 = num3 - num10;
			double num19 = RobustPredicates.splitter * num5;
			num9 = num19 - num5;
			num13 = num19 - num9;
			num14 = num5 - num13;
			num15 = num17 - num10 * num13 - num11 * num13 - num10 * num14;
			double num20 = num11 * num14 - num15;
			double num21 = num16 - num20;
			double num22 = num16 - num21;
			double num23 = num21 + num22;
			double num24 = num22 - num20;
			double num25 = num16 - num23;
			array[0] = num25 + num24;
			double num26 = num7 + num21;
			num22 = num26 - num7;
			num23 = num26 - num22;
			num24 = num21 - num22;
			num25 = num7 - num23;
			double num27 = num25 + num24;
			num21 = num27 - num17;
			num22 = num27 - num21;
			num23 = num21 + num22;
			num24 = num22 - num17;
			num25 = num27 - num23;
			array[1] = num25 + num24;
			double num28 = num26 + num21;
			num22 = num28 - num26;
			num23 = num28 - num22;
			num24 = num21 - num22;
			num25 = num26 - num23;
			array[2] = num25 + num24;
			array[3] = num28;
			int elen7 = this.ScaleExpansionZeroElim(4, array, num, this.axbc);
			int elen8 = this.ScaleExpansionZeroElim(elen7, this.axbc, num, this.axxbc);
			int elen9 = this.ScaleExpansionZeroElim(4, array, num4, this.aybc);
			int flen = this.ScaleExpansionZeroElim(elen9, this.aybc, num4, this.ayybc);
			int elen10 = this.FastExpansionSumZeroElim(elen8, this.axxbc, flen, this.ayybc, this.adet);
			double num29 = num3 * num4;
			double num30 = RobustPredicates.splitter * num3;
			num9 = num30 - num3;
			num10 = num30 - num9;
			num11 = num3 - num10;
			double num31 = RobustPredicates.splitter * num4;
			num9 = num31 - num4;
			num13 = num31 - num9;
			num14 = num4 - num13;
			num15 = num29 - num10 * num13 - num11 * num13 - num10 * num14;
			double num32 = num11 * num14 - num15;
			double num33 = num * num6;
			double num34 = RobustPredicates.splitter * num;
			num9 = num34 - num;
			num10 = num34 - num9;
			num11 = num - num10;
			double num35 = RobustPredicates.splitter * num6;
			num9 = num35 - num6;
			num13 = num35 - num9;
			num14 = num6 - num13;
			num15 = num33 - num10 * num13 - num11 * num13 - num10 * num14;
			double num36 = num11 * num14 - num15;
			num21 = num32 - num36;
			num22 = num32 - num21;
			num23 = num21 + num22;
			num24 = num22 - num36;
			num25 = num32 - num23;
			array2[0] = num25 + num24;
			num26 = num29 + num21;
			num22 = num26 - num29;
			num23 = num26 - num22;
			num24 = num21 - num22;
			num25 = num29 - num23;
			num27 = num25 + num24;
			num21 = num27 - num33;
			num22 = num27 - num21;
			num23 = num21 + num22;
			num24 = num22 - num33;
			num25 = num27 - num23;
			array2[1] = num25 + num24;
			double num37 = num26 + num21;
			num22 = num37 - num26;
			num23 = num37 - num22;
			num24 = num21 - num22;
			num25 = num26 - num23;
			array2[2] = num25 + num24;
			array2[3] = num37;
			int elen11 = this.ScaleExpansionZeroElim(4, array2, num2, this.bxca);
			int elen12 = this.ScaleExpansionZeroElim(elen11, this.bxca, num2, this.bxxca);
			int elen13 = this.ScaleExpansionZeroElim(4, array2, num5, this.byca);
			int flen2 = this.ScaleExpansionZeroElim(elen13, this.byca, num5, this.byyca);
			int flen3 = this.FastExpansionSumZeroElim(elen12, this.bxxca, flen2, this.byyca, this.bdet);
			double num38 = num * num5;
			double num39 = RobustPredicates.splitter * num;
			num9 = num39 - num;
			num10 = num39 - num9;
			num11 = num - num10;
			double num40 = RobustPredicates.splitter * num5;
			num9 = num40 - num5;
			num13 = num40 - num9;
			num14 = num5 - num13;
			num15 = num38 - num10 * num13 - num11 * num13 - num10 * num14;
			double num41 = num11 * num14 - num15;
			double num42 = num2 * num4;
			double num43 = RobustPredicates.splitter * num2;
			num9 = num43 - num2;
			num10 = num43 - num9;
			num11 = num2 - num10;
			double num44 = RobustPredicates.splitter * num4;
			num9 = num44 - num4;
			num13 = num44 - num9;
			num14 = num4 - num13;
			num15 = num42 - num10 * num13 - num11 * num13 - num10 * num14;
			double num45 = num11 * num14 - num15;
			num21 = num41 - num45;
			num22 = num41 - num21;
			num23 = num21 + num22;
			num24 = num22 - num45;
			num25 = num41 - num23;
			array3[0] = num25 + num24;
			num26 = num38 + num21;
			num22 = num26 - num38;
			num23 = num26 - num22;
			num24 = num21 - num22;
			num25 = num38 - num23;
			num27 = num25 + num24;
			num21 = num27 - num42;
			num22 = num27 - num21;
			num23 = num21 + num22;
			num24 = num22 - num42;
			num25 = num27 - num23;
			array3[1] = num25 + num24;
			double num46 = num26 + num21;
			num22 = num46 - num26;
			num23 = num46 - num22;
			num24 = num21 - num22;
			num25 = num26 - num23;
			array3[2] = num25 + num24;
			array3[3] = num46;
			int elen14 = this.ScaleExpansionZeroElim(4, array3, num3, this.cxab);
			int elen15 = this.ScaleExpansionZeroElim(elen14, this.cxab, num3, this.cxxab);
			int elen16 = this.ScaleExpansionZeroElim(4, array3, num6, this.cyab);
			int flen4 = this.ScaleExpansionZeroElim(elen16, this.cyab, num6, this.cyyab);
			int flen5 = this.FastExpansionSumZeroElim(elen15, this.cxxab, flen4, this.cyyab, this.cdet);
			int elen17 = this.FastExpansionSumZeroElim(elen10, this.adet, flen3, this.bdet, this.abdet);
			int num47 = this.FastExpansionSumZeroElim(elen17, this.abdet, flen5, this.cdet, this.fin1);
			double num48 = this.Estimate(num47, this.fin1);
			double num49 = RobustPredicates.iccerrboundB * permanent;
			if (num48 >= num49 || -num48 >= num49)
			{
				return num48;
			}
			num22 = pa.x - num;
			num23 = num + num22;
			num24 = num22 - pd.x;
			num25 = pa.x - num23;
			double num50 = num25 + num24;
			num22 = pa.y - num4;
			num23 = num4 + num22;
			num24 = num22 - pd.y;
			num25 = pa.y - num23;
			double num51 = num25 + num24;
			num22 = pb.x - num2;
			num23 = num2 + num22;
			num24 = num22 - pd.x;
			num25 = pb.x - num23;
			double num52 = num25 + num24;
			num22 = pb.y - num5;
			num23 = num5 + num22;
			num24 = num22 - pd.y;
			num25 = pb.y - num23;
			double num53 = num25 + num24;
			num22 = pc.x - num3;
			num23 = num3 + num22;
			num24 = num22 - pd.x;
			num25 = pc.x - num23;
			double num54 = num25 + num24;
			num22 = pc.y - num6;
			num23 = num6 + num22;
			num24 = num22 - pd.y;
			num25 = pc.y - num23;
			double num55 = num25 + num24;
			if (num50 == 0.0 && num52 == 0.0 && num54 == 0.0 && num51 == 0.0 && num53 == 0.0 && num55 == 0.0)
			{
				return num48;
			}
			num49 = RobustPredicates.iccerrboundC * permanent + RobustPredicates.resulterrbound * ((num48 >= 0.0) ? num48 : (-num48));
			num48 += (num * num + num4 * num4) * (num2 * num55 + num6 * num52 - (num5 * num54 + num3 * num53)) + 2.0 * (num * num50 + num4 * num51) * (num2 * num6 - num5 * num3) + ((num2 * num2 + num5 * num5) * (num3 * num51 + num4 * num54 - (num6 * num50 + num * num55)) + 2.0 * (num2 * num52 + num5 * num53) * (num3 * num4 - num6 * num)) + ((num3 * num3 + num6 * num6) * (num * num53 + num5 * num50 - (num4 * num52 + num2 * num51)) + 2.0 * (num3 * num54 + num6 * num55) * (num * num5 - num4 * num2));
			if (num48 >= num49 || -num48 >= num49)
			{
				return num48;
			}
			double[] array45 = this.fin1;
			double[] array46 = this.fin2;
			if (num52 != 0.0 || num53 != 0.0 || num54 != 0.0 || num55 != 0.0)
			{
				double num56 = num * num;
				double num57 = RobustPredicates.splitter * num;
				num9 = num57 - num;
				num10 = num57 - num9;
				num11 = num - num10;
				num15 = num56 - num10 * num10 - (num10 + num10) * num11;
				double num58 = num11 * num11 - num15;
				double num59 = num4 * num4;
				double num60 = RobustPredicates.splitter * num4;
				num9 = num60 - num4;
				num10 = num60 - num9;
				num11 = num4 - num10;
				num15 = num59 - num10 * num10 - (num10 + num10) * num11;
				double num61 = num11 * num11 - num15;
				num21 = num58 + num61;
				num22 = num21 - num58;
				num23 = num21 - num22;
				num24 = num61 - num22;
				num25 = num58 - num23;
				array4[0] = num25 + num24;
				num26 = num56 + num21;
				num22 = num26 - num56;
				num23 = num26 - num22;
				num24 = num21 - num22;
				num25 = num56 - num23;
				num27 = num25 + num24;
				num21 = num27 + num59;
				num22 = num21 - num27;
				num23 = num21 - num22;
				num24 = num59 - num22;
				num25 = num27 - num23;
				array4[1] = num25 + num24;
				double num62 = num26 + num21;
				num22 = num62 - num26;
				num23 = num62 - num22;
				num24 = num21 - num22;
				num25 = num26 - num23;
				array4[2] = num25 + num24;
				array4[3] = num62;
			}
			if (num54 != 0.0 || num55 != 0.0 || num50 != 0.0 || num51 != 0.0)
			{
				double num63 = num2 * num2;
				double num64 = RobustPredicates.splitter * num2;
				num9 = num64 - num2;
				num10 = num64 - num9;
				num11 = num2 - num10;
				num15 = num63 - num10 * num10 - (num10 + num10) * num11;
				double num65 = num11 * num11 - num15;
				double num66 = num5 * num5;
				double num67 = RobustPredicates.splitter * num5;
				num9 = num67 - num5;
				num10 = num67 - num9;
				num11 = num5 - num10;
				num15 = num66 - num10 * num10 - (num10 + num10) * num11;
				double num68 = num11 * num11 - num15;
				num21 = num65 + num68;
				num22 = num21 - num65;
				num23 = num21 - num22;
				num24 = num68 - num22;
				num25 = num65 - num23;
				array5[0] = num25 + num24;
				num26 = num63 + num21;
				num22 = num26 - num63;
				num23 = num26 - num22;
				num24 = num21 - num22;
				num25 = num63 - num23;
				num27 = num25 + num24;
				num21 = num27 + num66;
				num22 = num21 - num27;
				num23 = num21 - num22;
				num24 = num66 - num22;
				num25 = num27 - num23;
				array5[1] = num25 + num24;
				double num69 = num26 + num21;
				num22 = num69 - num26;
				num23 = num69 - num22;
				num24 = num21 - num22;
				num25 = num26 - num23;
				array5[2] = num25 + num24;
				array5[3] = num69;
			}
			if (num50 != 0.0 || num51 != 0.0 || num52 != 0.0 || num53 != 0.0)
			{
				double num70 = num3 * num3;
				double num71 = RobustPredicates.splitter * num3;
				num9 = num71 - num3;
				num10 = num71 - num9;
				num11 = num3 - num10;
				num15 = num70 - num10 * num10 - (num10 + num10) * num11;
				double num72 = num11 * num11 - num15;
				double num73 = num6 * num6;
				double num74 = RobustPredicates.splitter * num6;
				num9 = num74 - num6;
				num10 = num74 - num9;
				num11 = num6 - num10;
				num15 = num73 - num10 * num10 - (num10 + num10) * num11;
				double num75 = num11 * num11 - num15;
				num21 = num72 + num75;
				num22 = num21 - num72;
				num23 = num21 - num22;
				num24 = num75 - num22;
				num25 = num72 - num23;
				array6[0] = num25 + num24;
				num26 = num70 + num21;
				num22 = num26 - num70;
				num23 = num26 - num22;
				num24 = num21 - num22;
				num25 = num70 - num23;
				num27 = num25 + num24;
				num21 = num27 + num73;
				num22 = num21 - num27;
				num23 = num21 - num22;
				num24 = num73 - num22;
				num25 = num27 - num23;
				array6[1] = num25 + num24;
				double num76 = num26 + num21;
				num22 = num76 - num26;
				num23 = num76 - num22;
				num24 = num21 - num22;
				num25 = num26 - num23;
				array6[2] = num25 + num24;
				array6[3] = num76;
			}
			if (num50 != 0.0)
			{
				elen = this.ScaleExpansionZeroElim(4, array, num50, array21);
				int num77 = this.ScaleExpansionZeroElim(elen, array21, 2.0 * num, this.temp16a);
				int elen18 = this.ScaleExpansionZeroElim(4, array6, num50, array10);
				int flen6 = this.ScaleExpansionZeroElim(elen18, array10, num5, this.temp16b);
				int elen19 = this.ScaleExpansionZeroElim(4, array5, num50, array9);
				int elen20 = this.ScaleExpansionZeroElim(elen19, array9, -num6, this.temp16c);
				int num78 = this.FastExpansionSumZeroElim(num77, this.temp16a, flen6, this.temp16b, this.temp32a);
				int flen7 = this.FastExpansionSumZeroElim(elen20, this.temp16c, num78, this.temp32a, this.temp48);
				num47 = this.FastExpansionSumZeroElim(num47, array45, flen7, this.temp48, array46);
				double[] array47 = array45;
				array45 = array46;
				array46 = array47;
			}
			if (num51 != 0.0)
			{
				elen2 = this.ScaleExpansionZeroElim(4, array, num51, array22);
				int num77 = this.ScaleExpansionZeroElim(elen2, array22, 2.0 * num4, this.temp16a);
				int elen21 = this.ScaleExpansionZeroElim(4, array5, num51, array11);
				int flen6 = this.ScaleExpansionZeroElim(elen21, array11, num3, this.temp16b);
				int elen22 = this.ScaleExpansionZeroElim(4, array6, num51, array12);
				int elen20 = this.ScaleExpansionZeroElim(elen22, array12, -num2, this.temp16c);
				int num78 = this.FastExpansionSumZeroElim(num77, this.temp16a, flen6, this.temp16b, this.temp32a);
				int flen7 = this.FastExpansionSumZeroElim(elen20, this.temp16c, num78, this.temp32a, this.temp48);
				num47 = this.FastExpansionSumZeroElim(num47, array45, flen7, this.temp48, array46);
				double[] array48 = array45;
				array45 = array46;
				array46 = array48;
			}
			if (num52 != 0.0)
			{
				elen3 = this.ScaleExpansionZeroElim(4, array2, num52, array23);
				int num77 = this.ScaleExpansionZeroElim(elen3, array23, 2.0 * num2, this.temp16a);
				int elen23 = this.ScaleExpansionZeroElim(4, array4, num52, array13);
				int flen6 = this.ScaleExpansionZeroElim(elen23, array13, num6, this.temp16b);
				int elen24 = this.ScaleExpansionZeroElim(4, array6, num52, array14);
				int elen20 = this.ScaleExpansionZeroElim(elen24, array14, -num4, this.temp16c);
				int num78 = this.FastExpansionSumZeroElim(num77, this.temp16a, flen6, this.temp16b, this.temp32a);
				int flen7 = this.FastExpansionSumZeroElim(elen20, this.temp16c, num78, this.temp32a, this.temp48);
				num47 = this.FastExpansionSumZeroElim(num47, array45, flen7, this.temp48, array46);
				double[] array49 = array45;
				array45 = array46;
				array46 = array49;
			}
			if (num53 != 0.0)
			{
				elen4 = this.ScaleExpansionZeroElim(4, array2, num53, array24);
				int num77 = this.ScaleExpansionZeroElim(elen4, array24, 2.0 * num5, this.temp16a);
				int elen25 = this.ScaleExpansionZeroElim(4, array6, num53, array16);
				int flen6 = this.ScaleExpansionZeroElim(elen25, array16, num, this.temp16b);
				int elen26 = this.ScaleExpansionZeroElim(4, array4, num53, array15);
				int elen20 = this.ScaleExpansionZeroElim(elen26, array15, -num3, this.temp16c);
				int num78 = this.FastExpansionSumZeroElim(num77, this.temp16a, flen6, this.temp16b, this.temp32a);
				int flen7 = this.FastExpansionSumZeroElim(elen20, this.temp16c, num78, this.temp32a, this.temp48);
				num47 = this.FastExpansionSumZeroElim(num47, array45, flen7, this.temp48, array46);
				double[] array50 = array45;
				array45 = array46;
				array46 = array50;
			}
			if (num54 != 0.0)
			{
				elen5 = this.ScaleExpansionZeroElim(4, array3, num54, array25);
				int num77 = this.ScaleExpansionZeroElim(elen5, array25, 2.0 * num3, this.temp16a);
				int elen27 = this.ScaleExpansionZeroElim(4, array5, num54, array18);
				int flen6 = this.ScaleExpansionZeroElim(elen27, array18, num4, this.temp16b);
				int elen28 = this.ScaleExpansionZeroElim(4, array4, num54, array17);
				int elen20 = this.ScaleExpansionZeroElim(elen28, array17, -num5, this.temp16c);
				int num78 = this.FastExpansionSumZeroElim(num77, this.temp16a, flen6, this.temp16b, this.temp32a);
				int flen7 = this.FastExpansionSumZeroElim(elen20, this.temp16c, num78, this.temp32a, this.temp48);
				num47 = this.FastExpansionSumZeroElim(num47, array45, flen7, this.temp48, array46);
				double[] array51 = array45;
				array45 = array46;
				array46 = array51;
			}
			if (num55 != 0.0)
			{
				elen6 = this.ScaleExpansionZeroElim(4, array3, num55, array26);
				int num77 = this.ScaleExpansionZeroElim(elen6, array26, 2.0 * num6, this.temp16a);
				int elen29 = this.ScaleExpansionZeroElim(4, array4, num55, array19);
				int flen6 = this.ScaleExpansionZeroElim(elen29, array19, num2, this.temp16b);
				int elen30 = this.ScaleExpansionZeroElim(4, array5, num55, array20);
				int elen20 = this.ScaleExpansionZeroElim(elen30, array20, -num, this.temp16c);
				int num78 = this.FastExpansionSumZeroElim(num77, this.temp16a, flen6, this.temp16b, this.temp32a);
				int flen7 = this.FastExpansionSumZeroElim(elen20, this.temp16c, num78, this.temp32a, this.temp48);
				num47 = this.FastExpansionSumZeroElim(num47, array45, flen7, this.temp48, array46);
				double[] array52 = array45;
				array45 = array46;
				array46 = array52;
			}
			if (num50 != 0.0 || num51 != 0.0)
			{
				int elen31;
				int elen32;
				if (num52 != 0.0 || num53 != 0.0 || num54 != 0.0 || num55 != 0.0)
				{
					double num79 = num52 * num6;
					double num80 = RobustPredicates.splitter * num52;
					num9 = num80 - num52;
					num10 = num80 - num9;
					num11 = num52 - num10;
					double num81 = RobustPredicates.splitter * num6;
					num9 = num81 - num6;
					num13 = num81 - num9;
					num14 = num6 - num13;
					num15 = num79 - num10 * num13 - num11 * num13 - num10 * num14;
					double num82 = num11 * num14 - num15;
					double num83 = num2 * num55;
					double num84 = RobustPredicates.splitter * num2;
					num9 = num84 - num2;
					num10 = num84 - num9;
					num11 = num2 - num10;
					double num85 = RobustPredicates.splitter * num55;
					num9 = num85 - num55;
					num13 = num85 - num9;
					num14 = num55 - num13;
					num15 = num83 - num10 * num13 - num11 * num13 - num10 * num14;
					double num86 = num11 * num14 - num15;
					num21 = num82 + num86;
					num22 = num21 - num82;
					num23 = num21 - num22;
					num24 = num86 - num22;
					num25 = num82 - num23;
					array7[0] = num25 + num24;
					num26 = num79 + num21;
					num22 = num26 - num79;
					num23 = num26 - num22;
					num24 = num21 - num22;
					num25 = num79 - num23;
					num27 = num25 + num24;
					num21 = num27 + num83;
					num22 = num21 - num27;
					num23 = num21 - num22;
					num24 = num83 - num22;
					num25 = num27 - num23;
					array7[1] = num25 + num24;
					double num87 = num26 + num21;
					num22 = num87 - num26;
					num23 = num87 - num22;
					num24 = num21 - num22;
					num25 = num26 - num23;
					array7[2] = num25 + num24;
					array7[3] = num87;
					double num88 = -num5;
					num79 = num54 * num88;
					double num89 = RobustPredicates.splitter * num54;
					num9 = num89 - num54;
					num10 = num89 - num9;
					num11 = num54 - num10;
					double num90 = RobustPredicates.splitter * num88;
					num9 = num90 - num88;
					num13 = num90 - num9;
					num14 = num88 - num13;
					num15 = num79 - num10 * num13 - num11 * num13 - num10 * num14;
					num82 = num11 * num14 - num15;
					num88 = -num53;
					num83 = num3 * num88;
					double num91 = RobustPredicates.splitter * num3;
					num9 = num91 - num3;
					num10 = num91 - num9;
					num11 = num3 - num10;
					double num92 = RobustPredicates.splitter * num88;
					num9 = num92 - num88;
					num13 = num92 - num9;
					num14 = num88 - num13;
					num15 = num83 - num10 * num13 - num11 * num13 - num10 * num14;
					num86 = num11 * num14 - num15;
					num21 = num82 + num86;
					num22 = num21 - num82;
					num23 = num21 - num22;
					num24 = num86 - num22;
					num25 = num82 - num23;
					array8[0] = num25 + num24;
					num26 = num79 + num21;
					num22 = num26 - num79;
					num23 = num26 - num22;
					num24 = num21 - num22;
					num25 = num79 - num23;
					num27 = num25 + num24;
					num21 = num27 + num83;
					num22 = num21 - num27;
					num23 = num21 - num22;
					num24 = num83 - num22;
					num25 = num27 - num23;
					array8[1] = num25 + num24;
					double num93 = num26 + num21;
					num22 = num93 - num26;
					num23 = num93 - num22;
					num24 = num21 - num22;
					num25 = num26 - num23;
					array8[2] = num25 + num24;
					array8[3] = num93;
					elen31 = this.FastExpansionSumZeroElim(4, array7, 4, array8, array40);
					num79 = num52 * num55;
					double num94 = RobustPredicates.splitter * num52;
					num9 = num94 - num52;
					num10 = num94 - num9;
					num11 = num52 - num10;
					double num95 = RobustPredicates.splitter * num55;
					num9 = num95 - num55;
					num13 = num95 - num9;
					num14 = num55 - num13;
					num15 = num79 - num10 * num13 - num11 * num13 - num10 * num14;
					num82 = num11 * num14 - num15;
					num83 = num54 * num53;
					double num96 = RobustPredicates.splitter * num54;
					num9 = num96 - num54;
					num10 = num96 - num9;
					num11 = num54 - num10;
					double num97 = RobustPredicates.splitter * num53;
					num9 = num97 - num53;
					num13 = num97 - num9;
					num14 = num53 - num13;
					num15 = num83 - num10 * num13 - num11 * num13 - num10 * num14;
					num86 = num11 * num14 - num15;
					num21 = num82 - num86;
					num22 = num82 - num21;
					num23 = num21 + num22;
					num24 = num22 - num86;
					num25 = num82 - num23;
					array43[0] = num25 + num24;
					num26 = num79 + num21;
					num22 = num26 - num79;
					num23 = num26 - num22;
					num24 = num21 - num22;
					num25 = num79 - num23;
					num27 = num25 + num24;
					num21 = num27 - num83;
					num22 = num27 - num21;
					num23 = num21 + num22;
					num24 = num22 - num83;
					num25 = num27 - num23;
					array43[1] = num25 + num24;
					double num98 = num26 + num21;
					num22 = num98 - num26;
					num23 = num98 - num22;
					num24 = num21 - num22;
					num25 = num26 - num23;
					array43[2] = num25 + num24;
					array43[3] = num98;
					elen32 = 4;
				}
				else
				{
					array40[0] = 0.0;
					elen31 = 1;
					array43[0] = 0.0;
					elen32 = 1;
				}
				if (num50 != 0.0)
				{
					int num77 = this.ScaleExpansionZeroElim(elen, array21, num50, this.temp16a);
					int elen33 = this.ScaleExpansionZeroElim(elen31, array40, num50, array27);
					int num78 = this.ScaleExpansionZeroElim(elen33, array27, 2.0 * num, this.temp32a);
					int flen7 = this.FastExpansionSumZeroElim(num77, this.temp16a, num78, this.temp32a, this.temp48);
					num47 = this.FastExpansionSumZeroElim(num47, array45, flen7, this.temp48, array46);
					double[] array53 = array45;
					array45 = array46;
					array46 = array53;
					if (num53 != 0.0)
					{
						int elen34 = this.ScaleExpansionZeroElim(4, array6, num50, this.temp8);
						num77 = this.ScaleExpansionZeroElim(elen34, this.temp8, num53, this.temp16a);
						num47 = this.FastExpansionSumZeroElim(num47, array45, num77, this.temp16a, array46);
						double[] array54 = array45;
						array45 = array46;
						array46 = array54;
					}
					if (num55 != 0.0)
					{
						int elen34 = this.ScaleExpansionZeroElim(4, array5, -num50, this.temp8);
						num77 = this.ScaleExpansionZeroElim(elen34, this.temp8, num55, this.temp16a);
						num47 = this.FastExpansionSumZeroElim(num47, array45, num77, this.temp16a, array46);
						double[] array55 = array45;
						array45 = array46;
						array46 = array55;
					}
					num78 = this.ScaleExpansionZeroElim(elen33, array27, num50, this.temp32a);
					int elen35 = this.ScaleExpansionZeroElim(elen32, array43, num50, array33);
					num77 = this.ScaleExpansionZeroElim(elen35, array33, 2.0 * num, this.temp16a);
					int flen6 = this.ScaleExpansionZeroElim(elen35, array33, num50, this.temp16b);
					int flen8 = this.FastExpansionSumZeroElim(num77, this.temp16a, flen6, this.temp16b, this.temp32b);
					int flen9 = this.FastExpansionSumZeroElim(num78, this.temp32a, flen8, this.temp32b, this.temp64);
					num47 = this.FastExpansionSumZeroElim(num47, array45, flen9, this.temp64, array46);
					double[] array56 = array45;
					array45 = array46;
					array46 = array56;
				}
				if (num51 != 0.0)
				{
					int num77 = this.ScaleExpansionZeroElim(elen2, array22, num51, this.temp16a);
					int elen36 = this.ScaleExpansionZeroElim(elen31, array40, num51, array28);
					int num78 = this.ScaleExpansionZeroElim(elen36, array28, 2.0 * num4, this.temp32a);
					int flen7 = this.FastExpansionSumZeroElim(num77, this.temp16a, num78, this.temp32a, this.temp48);
					num47 = this.FastExpansionSumZeroElim(num47, array45, flen7, this.temp48, array46);
					double[] array57 = array45;
					array45 = array46;
					array46 = array57;
					num78 = this.ScaleExpansionZeroElim(elen36, array28, num51, this.temp32a);
					int elen37 = this.ScaleExpansionZeroElim(elen32, array43, num51, array34);
					num77 = this.ScaleExpansionZeroElim(elen37, array34, 2.0 * num4, this.temp16a);
					int flen6 = this.ScaleExpansionZeroElim(elen37, array34, num51, this.temp16b);
					int flen8 = this.FastExpansionSumZeroElim(num77, this.temp16a, flen6, this.temp16b, this.temp32b);
					int flen9 = this.FastExpansionSumZeroElim(num78, this.temp32a, flen8, this.temp32b, this.temp64);
					num47 = this.FastExpansionSumZeroElim(num47, array45, flen9, this.temp64, array46);
					double[] array58 = array45;
					array45 = array46;
					array46 = array58;
				}
			}
			if (num52 != 0.0 || num53 != 0.0)
			{
				int elen38;
				int elen39;
				if (num54 != 0.0 || num55 != 0.0 || num50 != 0.0 || num51 != 0.0)
				{
					double num79 = num54 * num4;
					double num99 = RobustPredicates.splitter * num54;
					num9 = num99 - num54;
					num10 = num99 - num9;
					num11 = num54 - num10;
					double num100 = RobustPredicates.splitter * num4;
					num9 = num100 - num4;
					num13 = num100 - num9;
					num14 = num4 - num13;
					num15 = num79 - num10 * num13 - num11 * num13 - num10 * num14;
					double num82 = num11 * num14 - num15;
					double num83 = num3 * num51;
					double num101 = RobustPredicates.splitter * num3;
					num9 = num101 - num3;
					num10 = num101 - num9;
					num11 = num3 - num10;
					double num102 = RobustPredicates.splitter * num51;
					num9 = num102 - num51;
					num13 = num102 - num9;
					num14 = num51 - num13;
					num15 = num83 - num10 * num13 - num11 * num13 - num10 * num14;
					double num86 = num11 * num14 - num15;
					num21 = num82 + num86;
					num22 = num21 - num82;
					num23 = num21 - num22;
					num24 = num86 - num22;
					num25 = num82 - num23;
					array7[0] = num25 + num24;
					num26 = num79 + num21;
					num22 = num26 - num79;
					num23 = num26 - num22;
					num24 = num21 - num22;
					num25 = num79 - num23;
					num27 = num25 + num24;
					num21 = num27 + num83;
					num22 = num21 - num27;
					num23 = num21 - num22;
					num24 = num83 - num22;
					num25 = num27 - num23;
					array7[1] = num25 + num24;
					double num87 = num26 + num21;
					num22 = num87 - num26;
					num23 = num87 - num22;
					num24 = num21 - num22;
					num25 = num26 - num23;
					array7[2] = num25 + num24;
					array7[3] = num87;
					double num88 = -num6;
					num79 = num50 * num88;
					double num103 = RobustPredicates.splitter * num50;
					num9 = num103 - num50;
					num10 = num103 - num9;
					num11 = num50 - num10;
					double num104 = RobustPredicates.splitter * num88;
					num9 = num104 - num88;
					num13 = num104 - num9;
					num14 = num88 - num13;
					num15 = num79 - num10 * num13 - num11 * num13 - num10 * num14;
					num82 = num11 * num14 - num15;
					num88 = -num55;
					num83 = num * num88;
					double num105 = RobustPredicates.splitter * num;
					num9 = num105 - num;
					num10 = num105 - num9;
					num11 = num - num10;
					double num106 = RobustPredicates.splitter * num88;
					num9 = num106 - num88;
					num13 = num106 - num9;
					num14 = num88 - num13;
					num15 = num83 - num10 * num13 - num11 * num13 - num10 * num14;
					num86 = num11 * num14 - num15;
					num21 = num82 + num86;
					num22 = num21 - num82;
					num23 = num21 - num22;
					num24 = num86 - num22;
					num25 = num82 - num23;
					array8[0] = num25 + num24;
					num26 = num79 + num21;
					num22 = num26 - num79;
					num23 = num26 - num22;
					num24 = num21 - num22;
					num25 = num79 - num23;
					num27 = num25 + num24;
					num21 = num27 + num83;
					num22 = num21 - num27;
					num23 = num21 - num22;
					num24 = num83 - num22;
					num25 = num27 - num23;
					array8[1] = num25 + num24;
					double num93 = num26 + num21;
					num22 = num93 - num26;
					num23 = num93 - num22;
					num24 = num21 - num22;
					num25 = num26 - num23;
					array8[2] = num25 + num24;
					array8[3] = num93;
					elen38 = this.FastExpansionSumZeroElim(4, array7, 4, array8, array41);
					num79 = num54 * num51;
					double num107 = RobustPredicates.splitter * num54;
					num9 = num107 - num54;
					num10 = num107 - num9;
					num11 = num54 - num10;
					double num108 = RobustPredicates.splitter * num51;
					num9 = num108 - num51;
					num13 = num108 - num9;
					num14 = num51 - num13;
					num15 = num79 - num10 * num13 - num11 * num13 - num10 * num14;
					num82 = num11 * num14 - num15;
					num83 = num50 * num55;
					double num109 = RobustPredicates.splitter * num50;
					num9 = num109 - num50;
					num10 = num109 - num9;
					num11 = num50 - num10;
					double num110 = RobustPredicates.splitter * num55;
					num9 = num110 - num55;
					num13 = num110 - num9;
					num14 = num55 - num13;
					num15 = num83 - num10 * num13 - num11 * num13 - num10 * num14;
					num86 = num11 * num14 - num15;
					num21 = num82 - num86;
					num22 = num82 - num21;
					num23 = num21 + num22;
					num24 = num22 - num86;
					num25 = num82 - num23;
					array44[0] = num25 + num24;
					num26 = num79 + num21;
					num22 = num26 - num79;
					num23 = num26 - num22;
					num24 = num21 - num22;
					num25 = num79 - num23;
					num27 = num25 + num24;
					num21 = num27 - num83;
					num22 = num27 - num21;
					num23 = num21 + num22;
					num24 = num22 - num83;
					num25 = num27 - num23;
					array44[1] = num25 + num24;
					double num111 = num26 + num21;
					num22 = num111 - num26;
					num23 = num111 - num22;
					num24 = num21 - num22;
					num25 = num26 - num23;
					array44[2] = num25 + num24;
					array44[3] = num111;
					elen39 = 4;
				}
				else
				{
					array41[0] = 0.0;
					elen38 = 1;
					array44[0] = 0.0;
					elen39 = 1;
				}
				if (num52 != 0.0)
				{
					int num77 = this.ScaleExpansionZeroElim(elen3, array23, num52, this.temp16a);
					int elen40 = this.ScaleExpansionZeroElim(elen38, array41, num52, array29);
					int num78 = this.ScaleExpansionZeroElim(elen40, array29, 2.0 * num2, this.temp32a);
					int flen7 = this.FastExpansionSumZeroElim(num77, this.temp16a, num78, this.temp32a, this.temp48);
					num47 = this.FastExpansionSumZeroElim(num47, array45, flen7, this.temp48, array46);
					double[] array59 = array45;
					array45 = array46;
					array46 = array59;
					if (num55 != 0.0)
					{
						int elen34 = this.ScaleExpansionZeroElim(4, array4, num52, this.temp8);
						num77 = this.ScaleExpansionZeroElim(elen34, this.temp8, num55, this.temp16a);
						num47 = this.FastExpansionSumZeroElim(num47, array45, num77, this.temp16a, array46);
						double[] array60 = array45;
						array45 = array46;
						array46 = array60;
					}
					if (num51 != 0.0)
					{
						int elen34 = this.ScaleExpansionZeroElim(4, array6, -num52, this.temp8);
						num77 = this.ScaleExpansionZeroElim(elen34, this.temp8, num51, this.temp16a);
						num47 = this.FastExpansionSumZeroElim(num47, array45, num77, this.temp16a, array46);
						double[] array61 = array45;
						array45 = array46;
						array46 = array61;
					}
					num78 = this.ScaleExpansionZeroElim(elen40, array29, num52, this.temp32a);
					int elen41 = this.ScaleExpansionZeroElim(elen39, array44, num52, array35);
					num77 = this.ScaleExpansionZeroElim(elen41, array35, 2.0 * num2, this.temp16a);
					int flen6 = this.ScaleExpansionZeroElim(elen41, array35, num52, this.temp16b);
					int flen8 = this.FastExpansionSumZeroElim(num77, this.temp16a, flen6, this.temp16b, this.temp32b);
					int flen9 = this.FastExpansionSumZeroElim(num78, this.temp32a, flen8, this.temp32b, this.temp64);
					num47 = this.FastExpansionSumZeroElim(num47, array45, flen9, this.temp64, array46);
					double[] array62 = array45;
					array45 = array46;
					array46 = array62;
				}
				if (num53 != 0.0)
				{
					int num77 = this.ScaleExpansionZeroElim(elen4, array24, num53, this.temp16a);
					int elen42 = this.ScaleExpansionZeroElim(elen38, array41, num53, array30);
					int num78 = this.ScaleExpansionZeroElim(elen42, array30, 2.0 * num5, this.temp32a);
					int flen7 = this.FastExpansionSumZeroElim(num77, this.temp16a, num78, this.temp32a, this.temp48);
					num47 = this.FastExpansionSumZeroElim(num47, array45, flen7, this.temp48, array46);
					double[] array63 = array45;
					array45 = array46;
					array46 = array63;
					num78 = this.ScaleExpansionZeroElim(elen42, array30, num53, this.temp32a);
					int elen43 = this.ScaleExpansionZeroElim(elen39, array44, num53, array36);
					num77 = this.ScaleExpansionZeroElim(elen43, array36, 2.0 * num5, this.temp16a);
					int flen6 = this.ScaleExpansionZeroElim(elen43, array36, num53, this.temp16b);
					int flen8 = this.FastExpansionSumZeroElim(num77, this.temp16a, flen6, this.temp16b, this.temp32b);
					int flen9 = this.FastExpansionSumZeroElim(num78, this.temp32a, flen8, this.temp32b, this.temp64);
					num47 = this.FastExpansionSumZeroElim(num47, array45, flen9, this.temp64, array46);
					double[] array64 = array45;
					array45 = array46;
					array46 = array64;
				}
			}
			if (num54 != 0.0 || num55 != 0.0)
			{
				int elen44;
				int elen45;
				if (num50 != 0.0 || num51 != 0.0 || num52 != 0.0 || num53 != 0.0)
				{
					double num79 = num50 * num5;
					double num112 = RobustPredicates.splitter * num50;
					num9 = num112 - num50;
					num10 = num112 - num9;
					num11 = num50 - num10;
					double num113 = RobustPredicates.splitter * num5;
					num9 = num113 - num5;
					num13 = num113 - num9;
					num14 = num5 - num13;
					num15 = num79 - num10 * num13 - num11 * num13 - num10 * num14;
					double num82 = num11 * num14 - num15;
					double num83 = num * num53;
					double num114 = RobustPredicates.splitter * num;
					num9 = num114 - num;
					num10 = num114 - num9;
					num11 = num - num10;
					double num115 = RobustPredicates.splitter * num53;
					num9 = num115 - num53;
					num13 = num115 - num9;
					num14 = num53 - num13;
					num15 = num83 - num10 * num13 - num11 * num13 - num10 * num14;
					double num86 = num11 * num14 - num15;
					num21 = num82 + num86;
					num22 = num21 - num82;
					num23 = num21 - num22;
					num24 = num86 - num22;
					num25 = num82 - num23;
					array7[0] = num25 + num24;
					num26 = num79 + num21;
					num22 = num26 - num79;
					num23 = num26 - num22;
					num24 = num21 - num22;
					num25 = num79 - num23;
					num27 = num25 + num24;
					num21 = num27 + num83;
					num22 = num21 - num27;
					num23 = num21 - num22;
					num24 = num83 - num22;
					num25 = num27 - num23;
					array7[1] = num25 + num24;
					double num87 = num26 + num21;
					num22 = num87 - num26;
					num23 = num87 - num22;
					num24 = num21 - num22;
					num25 = num26 - num23;
					array7[2] = num25 + num24;
					array7[3] = num87;
					double num88 = -num4;
					num79 = num52 * num88;
					double num116 = RobustPredicates.splitter * num52;
					num9 = num116 - num52;
					num10 = num116 - num9;
					num11 = num52 - num10;
					double num117 = RobustPredicates.splitter * num88;
					num9 = num117 - num88;
					num13 = num117 - num9;
					num14 = num88 - num13;
					num15 = num79 - num10 * num13 - num11 * num13 - num10 * num14;
					num82 = num11 * num14 - num15;
					num88 = -num51;
					num83 = num2 * num88;
					double num118 = RobustPredicates.splitter * num2;
					num9 = num118 - num2;
					num10 = num118 - num9;
					num11 = num2 - num10;
					double num119 = RobustPredicates.splitter * num88;
					num9 = num119 - num88;
					num13 = num119 - num9;
					num14 = num88 - num13;
					num15 = num83 - num10 * num13 - num11 * num13 - num10 * num14;
					num86 = num11 * num14 - num15;
					num21 = num82 + num86;
					num22 = num21 - num82;
					num23 = num21 - num22;
					num24 = num86 - num22;
					num25 = num82 - num23;
					array8[0] = num25 + num24;
					num26 = num79 + num21;
					num22 = num26 - num79;
					num23 = num26 - num22;
					num24 = num21 - num22;
					num25 = num79 - num23;
					num27 = num25 + num24;
					num21 = num27 + num83;
					num22 = num21 - num27;
					num23 = num21 - num22;
					num24 = num83 - num22;
					num25 = num27 - num23;
					array8[1] = num25 + num24;
					double num93 = num26 + num21;
					num22 = num93 - num26;
					num23 = num93 - num22;
					num24 = num21 - num22;
					num25 = num26 - num23;
					array8[2] = num25 + num24;
					array8[3] = num93;
					elen44 = this.FastExpansionSumZeroElim(4, array7, 4, array8, array39);
					num79 = num50 * num53;
					double num120 = RobustPredicates.splitter * num50;
					num9 = num120 - num50;
					num10 = num120 - num9;
					num11 = num50 - num10;
					double num121 = RobustPredicates.splitter * num53;
					num9 = num121 - num53;
					num13 = num121 - num9;
					num14 = num53 - num13;
					num15 = num79 - num10 * num13 - num11 * num13 - num10 * num14;
					num82 = num11 * num14 - num15;
					num83 = num52 * num51;
					double num122 = RobustPredicates.splitter * num52;
					num9 = num122 - num52;
					num10 = num122 - num9;
					num11 = num52 - num10;
					double num123 = RobustPredicates.splitter * num51;
					num9 = num123 - num51;
					num13 = num123 - num9;
					num14 = num51 - num13;
					num15 = num83 - num10 * num13 - num11 * num13 - num10 * num14;
					num86 = num11 * num14 - num15;
					num21 = num82 - num86;
					num22 = num82 - num21;
					num23 = num21 + num22;
					num24 = num22 - num86;
					num25 = num82 - num23;
					array42[0] = num25 + num24;
					num26 = num79 + num21;
					num22 = num26 - num79;
					num23 = num26 - num22;
					num24 = num21 - num22;
					num25 = num79 - num23;
					num27 = num25 + num24;
					num21 = num27 - num83;
					num22 = num27 - num21;
					num23 = num21 + num22;
					num24 = num22 - num83;
					num25 = num27 - num23;
					array42[1] = num25 + num24;
					double num124 = num26 + num21;
					num22 = num124 - num26;
					num23 = num124 - num22;
					num24 = num21 - num22;
					num25 = num26 - num23;
					array42[2] = num25 + num24;
					array42[3] = num124;
					elen45 = 4;
				}
				else
				{
					array39[0] = 0.0;
					elen44 = 1;
					array42[0] = 0.0;
					elen45 = 1;
				}
				if (num54 != 0.0)
				{
					int num77 = this.ScaleExpansionZeroElim(elen5, array25, num54, this.temp16a);
					int elen46 = this.ScaleExpansionZeroElim(elen44, array39, num54, array31);
					int num78 = this.ScaleExpansionZeroElim(elen46, array31, 2.0 * num3, this.temp32a);
					int flen7 = this.FastExpansionSumZeroElim(num77, this.temp16a, num78, this.temp32a, this.temp48);
					num47 = this.FastExpansionSumZeroElim(num47, array45, flen7, this.temp48, array46);
					double[] array65 = array45;
					array45 = array46;
					array46 = array65;
					if (num51 != 0.0)
					{
						int elen34 = this.ScaleExpansionZeroElim(4, array5, num54, this.temp8);
						num77 = this.ScaleExpansionZeroElim(elen34, this.temp8, num51, this.temp16a);
						num47 = this.FastExpansionSumZeroElim(num47, array45, num77, this.temp16a, array46);
						double[] array66 = array45;
						array45 = array46;
						array46 = array66;
					}
					if (num53 != 0.0)
					{
						int elen34 = this.ScaleExpansionZeroElim(4, array4, -num54, this.temp8);
						num77 = this.ScaleExpansionZeroElim(elen34, this.temp8, num53, this.temp16a);
						num47 = this.FastExpansionSumZeroElim(num47, array45, num77, this.temp16a, array46);
						double[] array67 = array45;
						array45 = array46;
						array46 = array67;
					}
					num78 = this.ScaleExpansionZeroElim(elen46, array31, num54, this.temp32a);
					int elen47 = this.ScaleExpansionZeroElim(elen45, array42, num54, array37);
					num77 = this.ScaleExpansionZeroElim(elen47, array37, 2.0 * num3, this.temp16a);
					int flen6 = this.ScaleExpansionZeroElim(elen47, array37, num54, this.temp16b);
					int flen8 = this.FastExpansionSumZeroElim(num77, this.temp16a, flen6, this.temp16b, this.temp32b);
					int flen9 = this.FastExpansionSumZeroElim(num78, this.temp32a, flen8, this.temp32b, this.temp64);
					num47 = this.FastExpansionSumZeroElim(num47, array45, flen9, this.temp64, array46);
					double[] array68 = array45;
					array45 = array46;
					array46 = array68;
				}
				if (num55 != 0.0)
				{
					int num77 = this.ScaleExpansionZeroElim(elen6, array26, num55, this.temp16a);
					int elen48 = this.ScaleExpansionZeroElim(elen44, array39, num55, array32);
					int num78 = this.ScaleExpansionZeroElim(elen48, array32, 2.0 * num6, this.temp32a);
					int flen7 = this.FastExpansionSumZeroElim(num77, this.temp16a, num78, this.temp32a, this.temp48);
					num47 = this.FastExpansionSumZeroElim(num47, array45, flen7, this.temp48, array46);
					double[] array69 = array45;
					array45 = array46;
					array46 = array69;
					num78 = this.ScaleExpansionZeroElim(elen48, array32, num55, this.temp32a);
					int elen49 = this.ScaleExpansionZeroElim(elen45, array42, num55, array38);
					num77 = this.ScaleExpansionZeroElim(elen49, array38, 2.0 * num6, this.temp16a);
					int flen6 = this.ScaleExpansionZeroElim(elen49, array38, num55, this.temp16b);
					int flen8 = this.FastExpansionSumZeroElim(num77, this.temp16a, flen6, this.temp16b, this.temp32b);
					int flen9 = this.FastExpansionSumZeroElim(num78, this.temp32a, flen8, this.temp32b, this.temp64);
					num47 = this.FastExpansionSumZeroElim(num47, array45, flen9, this.temp64, array46);
					array45 = array46;
				}
			}
			return array45[num47 - 1];
		}

		// Token: 0x06000896 RID: 2198 RVA: 0x000BCDDC File Offset: 0x000BAFDC
		private void AllocateWorkspace()
		{
			this.fin1 = new double[1152];
			this.fin2 = new double[1152];
			this.abdet = new double[64];
			this.axbc = new double[8];
			this.axxbc = new double[16];
			this.aybc = new double[8];
			this.ayybc = new double[16];
			this.adet = new double[32];
			this.bxca = new double[8];
			this.bxxca = new double[16];
			this.byca = new double[8];
			this.byyca = new double[16];
			this.bdet = new double[32];
			this.cxab = new double[8];
			this.cxxab = new double[16];
			this.cyab = new double[8];
			this.cyyab = new double[16];
			this.cdet = new double[32];
			this.temp8 = new double[8];
			this.temp16a = new double[16];
			this.temp16b = new double[16];
			this.temp16c = new double[16];
			this.temp32a = new double[32];
			this.temp32b = new double[32];
			this.temp48 = new double[48];
			this.temp64 = new double[64];
		}

		// Token: 0x06000897 RID: 2199 RVA: 0x0004475B File Offset: 0x0004295B
		private void ClearWorkspace()
		{
		}

		// Token: 0x040009C2 RID: 2498
		private static readonly object creationLock = new object();

		// Token: 0x040009C3 RID: 2499
		private static RobustPredicates _default;

		// Token: 0x040009C4 RID: 2500
		private static double epsilon;

		// Token: 0x040009C5 RID: 2501
		private static double splitter;

		// Token: 0x040009C6 RID: 2502
		private static double resulterrbound;

		// Token: 0x040009C7 RID: 2503
		private static double ccwerrboundA;

		// Token: 0x040009C8 RID: 2504
		private static double ccwerrboundB;

		// Token: 0x040009C9 RID: 2505
		private static double ccwerrboundC;

		// Token: 0x040009CA RID: 2506
		private static double iccerrboundA;

		// Token: 0x040009CB RID: 2507
		private static double iccerrboundB;

		// Token: 0x040009CC RID: 2508
		private static double iccerrboundC;

		// Token: 0x040009CD RID: 2509
		private double[] fin1;

		// Token: 0x040009CE RID: 2510
		private double[] fin2;

		// Token: 0x040009CF RID: 2511
		private double[] abdet;

		// Token: 0x040009D0 RID: 2512
		private double[] axbc;

		// Token: 0x040009D1 RID: 2513
		private double[] axxbc;

		// Token: 0x040009D2 RID: 2514
		private double[] aybc;

		// Token: 0x040009D3 RID: 2515
		private double[] ayybc;

		// Token: 0x040009D4 RID: 2516
		private double[] adet;

		// Token: 0x040009D5 RID: 2517
		private double[] bxca;

		// Token: 0x040009D6 RID: 2518
		private double[] bxxca;

		// Token: 0x040009D7 RID: 2519
		private double[] byca;

		// Token: 0x040009D8 RID: 2520
		private double[] byyca;

		// Token: 0x040009D9 RID: 2521
		private double[] bdet;

		// Token: 0x040009DA RID: 2522
		private double[] cxab;

		// Token: 0x040009DB RID: 2523
		private double[] cxxab;

		// Token: 0x040009DC RID: 2524
		private double[] cyab;

		// Token: 0x040009DD RID: 2525
		private double[] cyyab;

		// Token: 0x040009DE RID: 2526
		private double[] cdet;

		// Token: 0x040009DF RID: 2527
		private double[] temp8;

		// Token: 0x040009E0 RID: 2528
		private double[] temp16a;

		// Token: 0x040009E1 RID: 2529
		private double[] temp16b;

		// Token: 0x040009E2 RID: 2530
		private double[] temp16c;

		// Token: 0x040009E3 RID: 2531
		private double[] temp32a;

		// Token: 0x040009E4 RID: 2532
		private double[] temp32b;

		// Token: 0x040009E5 RID: 2533
		private double[] temp48;

		// Token: 0x040009E6 RID: 2534
		private double[] temp64;
	}
}
