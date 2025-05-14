using System;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.Tools
{
	// Token: 0x0200010D RID: 269
	public class QualityMeasure
	{
		// Token: 0x060009AC RID: 2476 RVA: 0x00049738 File Offset: 0x00047938
		public QualityMeasure()
		{
			this.areaMeasure = new QualityMeasure.AreaMeasure();
			this.alphaMeasure = new QualityMeasure.AlphaMeasure();
			this.qMeasure = new QualityMeasure.Q_Measure();
		}

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x060009AD RID: 2477 RVA: 0x00049761 File Offset: 0x00047961
		public double AreaMinimum
		{
			get
			{
				return this.areaMeasure.area_min;
			}
		}

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x060009AE RID: 2478 RVA: 0x0004976E File Offset: 0x0004796E
		public double AreaMaximum
		{
			get
			{
				return this.areaMeasure.area_max;
			}
		}

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x060009AF RID: 2479 RVA: 0x0004977B File Offset: 0x0004797B
		public double AreaRatio
		{
			get
			{
				return this.areaMeasure.area_max / this.areaMeasure.area_min;
			}
		}

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x060009B0 RID: 2480 RVA: 0x00049794 File Offset: 0x00047994
		public double AlphaMinimum
		{
			get
			{
				return this.alphaMeasure.alpha_min;
			}
		}

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x060009B1 RID: 2481 RVA: 0x000497A1 File Offset: 0x000479A1
		public double AlphaMaximum
		{
			get
			{
				return this.alphaMeasure.alpha_max;
			}
		}

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x060009B2 RID: 2482 RVA: 0x000497AE File Offset: 0x000479AE
		public double AlphaAverage
		{
			get
			{
				return this.alphaMeasure.alpha_ave;
			}
		}

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x060009B3 RID: 2483 RVA: 0x000497BB File Offset: 0x000479BB
		public double AlphaArea
		{
			get
			{
				return this.alphaMeasure.alpha_area;
			}
		}

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x060009B4 RID: 2484 RVA: 0x000497C8 File Offset: 0x000479C8
		public double Q_Minimum
		{
			get
			{
				return this.qMeasure.q_min;
			}
		}

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x060009B5 RID: 2485 RVA: 0x000497D5 File Offset: 0x000479D5
		public double Q_Maximum
		{
			get
			{
				return this.qMeasure.q_max;
			}
		}

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x060009B6 RID: 2486 RVA: 0x000497E2 File Offset: 0x000479E2
		public double Q_Average
		{
			get
			{
				return this.qMeasure.q_ave;
			}
		}

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x060009B7 RID: 2487 RVA: 0x000497EF File Offset: 0x000479EF
		public double Q_Area
		{
			get
			{
				return this.qMeasure.q_area;
			}
		}

		// Token: 0x060009B8 RID: 2488 RVA: 0x000497FC File Offset: 0x000479FC
		public void Update(Mesh mesh)
		{
			this.mesh = mesh;
			this.areaMeasure.Reset();
			this.alphaMeasure.Reset();
			this.qMeasure.Reset();
			this.Compute();
		}

		// Token: 0x060009B9 RID: 2489 RVA: 0x000C1C30 File Offset: 0x000BFE30
		private void Compute()
		{
			int num = 0;
			foreach (Triangle triangle in this.mesh.triangles)
			{
				num++;
				Point point = triangle.vertices[0];
				Point point2 = triangle.vertices[1];
				Point point3 = triangle.vertices[2];
				double num2 = point.x - point2.x;
				double num3 = point.y - point2.y;
				double ab = Math.Sqrt(num2 * num2 + num3 * num3);
				double num4 = point2.x - point3.x;
				num3 = point2.y - point3.y;
				double bc = Math.Sqrt(num4 * num4 + num3 * num3);
				double num5 = point3.x - point.x;
				num3 = point3.y - point.y;
				double ca = Math.Sqrt(num5 * num5 + num3 * num3);
				double area = this.areaMeasure.Measure(point, point2, point3);
				this.alphaMeasure.Measure(ab, bc, ca, area);
				this.qMeasure.Measure(ab, bc, ca, area);
			}
			this.alphaMeasure.Normalize(num, this.areaMeasure.area_total);
			this.qMeasure.Normalize(num, this.areaMeasure.area_total);
		}

		// Token: 0x060009BA RID: 2490 RVA: 0x000C1D90 File Offset: 0x000BFF90
		public int Bandwidth()
		{
			if (this.mesh == null)
			{
				return 0;
			}
			int num = 0;
			int num2 = 0;
			foreach (Triangle triangle in this.mesh.triangles)
			{
				for (int i = 0; i < 3; i++)
				{
					int id = triangle.GetVertex(i).id;
					for (int j = 0; j < 3; j++)
					{
						int id2 = triangle.GetVertex(j).id;
						num2 = Math.Max(num2, id2 - id);
						num = Math.Max(num, id - id2);
					}
				}
			}
			return num + 1 + num2;
		}

		// Token: 0x04000A4F RID: 2639
		private QualityMeasure.AreaMeasure areaMeasure;

		// Token: 0x04000A50 RID: 2640
		private QualityMeasure.AlphaMeasure alphaMeasure;

		// Token: 0x04000A51 RID: 2641
		private QualityMeasure.Q_Measure qMeasure;

		// Token: 0x04000A52 RID: 2642
		private Mesh mesh;

		// Token: 0x0200010E RID: 270
		private class AreaMeasure
		{
			// Token: 0x060009BB RID: 2491 RVA: 0x0004982C File Offset: 0x00047A2C
			public void Reset()
			{
				this.area_min = double.MaxValue;
				this.area_max = double.MinValue;
				this.area_total = 0.0;
				this.area_zero = 0;
			}

			// Token: 0x060009BC RID: 2492 RVA: 0x000C1E48 File Offset: 0x000C0048
			public double Measure(Point a, Point b, Point c)
			{
				double num = 0.5 * Math.Abs(a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y));
				this.area_min = Math.Min(this.area_min, num);
				this.area_max = Math.Max(this.area_max, num);
				this.area_total += num;
				if (num == 0.0)
				{
					this.area_zero++;
				}
				return num;
			}

			// Token: 0x04000A53 RID: 2643
			public double area_min = double.MaxValue;

			// Token: 0x04000A54 RID: 2644
			public double area_max = double.MinValue;

			// Token: 0x04000A55 RID: 2645
			public double area_total;

			// Token: 0x04000A56 RID: 2646
			public int area_zero;
		}

		// Token: 0x0200010F RID: 271
		private class AlphaMeasure
		{
			// Token: 0x060009BE RID: 2494 RVA: 0x00049888 File Offset: 0x00047A88
			public void Reset()
			{
				this.alpha_min = double.MaxValue;
				this.alpha_max = double.MinValue;
				this.alpha_ave = 0.0;
				this.alpha_area = 0.0;
			}

			// Token: 0x060009BF RID: 2495 RVA: 0x000498C6 File Offset: 0x00047AC6
			private double acos(double c)
			{
				if (c <= -1.0)
				{
					return 3.141592653589793;
				}
				if (1.0 <= c)
				{
					return 0.0;
				}
				return Math.Acos(c);
			}

			// Token: 0x060009C0 RID: 2496 RVA: 0x000C1EF0 File Offset: 0x000C00F0
			public double Measure(double ab, double bc, double ca, double area)
			{
				double num = double.MaxValue;
				double num2 = ab * ab;
				double num3 = bc * bc;
				double num4 = ca * ca;
				double val;
				double val2;
				double val3;
				if (ab == 0.0 && bc == 0.0 && ca == 0.0)
				{
					val = 2.0943951023931953;
					val2 = 2.0943951023931953;
					val3 = 2.0943951023931953;
				}
				else
				{
					if (ca == 0.0 || ab == 0.0)
					{
						val = 3.141592653589793;
					}
					else
					{
						val = this.acos((num4 + num2 - num3) / (2.0 * ca * ab));
					}
					if (ab == 0.0 || bc == 0.0)
					{
						val2 = 3.141592653589793;
					}
					else
					{
						val2 = this.acos((num2 + num3 - num4) / (2.0 * ab * bc));
					}
					if (bc == 0.0 || ca == 0.0)
					{
						val3 = 3.141592653589793;
					}
					else
					{
						val3 = this.acos((num3 + num4 - num2) / (2.0 * bc * ca));
					}
				}
				num = Math.Min(num, val);
				num = Math.Min(num, val2);
				num = Math.Min(num, val3);
				num = num * 3.0 / 3.141592653589793;
				this.alpha_ave += num;
				this.alpha_area += area * num;
				this.alpha_min = Math.Min(num, this.alpha_min);
				this.alpha_max = Math.Max(num, this.alpha_max);
				return num;
			}

			// Token: 0x060009C1 RID: 2497 RVA: 0x000C2094 File Offset: 0x000C0294
			public void Normalize(int n, double area_total)
			{
				if (n > 0)
				{
					this.alpha_ave /= (double)n;
				}
				else
				{
					this.alpha_ave = 0.0;
				}
				if (0.0 < area_total)
				{
					this.alpha_area /= area_total;
					return;
				}
				this.alpha_area = 0.0;
			}

			// Token: 0x04000A57 RID: 2647
			public double alpha_min;

			// Token: 0x04000A58 RID: 2648
			public double alpha_max;

			// Token: 0x04000A59 RID: 2649
			public double alpha_ave;

			// Token: 0x04000A5A RID: 2650
			public double alpha_area;
		}

		// Token: 0x02000110 RID: 272
		private class Q_Measure
		{
			// Token: 0x060009C3 RID: 2499 RVA: 0x000498FA File Offset: 0x00047AFA
			public void Reset()
			{
				this.q_min = double.MaxValue;
				this.q_max = double.MinValue;
				this.q_ave = 0.0;
				this.q_area = 0.0;
			}

			// Token: 0x060009C4 RID: 2500 RVA: 0x000C20F0 File Offset: 0x000C02F0
			public double Measure(double ab, double bc, double ca, double area)
			{
				double num = (bc + ca - ab) * (ca + ab - bc) * (ab + bc - ca) / (ab * bc * ca);
				this.q_min = Math.Min(this.q_min, num);
				this.q_max = Math.Max(this.q_max, num);
				this.q_ave += num;
				this.q_area += num * area;
				return num;
			}

			// Token: 0x060009C5 RID: 2501 RVA: 0x000C215C File Offset: 0x000C035C
			public void Normalize(int n, double area_total)
			{
				if (n > 0)
				{
					this.q_ave /= (double)n;
				}
				else
				{
					this.q_ave = 0.0;
				}
				if (area_total > 0.0)
				{
					this.q_area /= area_total;
					return;
				}
				this.q_area = 0.0;
			}

			// Token: 0x04000A5B RID: 2651
			public double q_min;

			// Token: 0x04000A5C RID: 2652
			public double q_max;

			// Token: 0x04000A5D RID: 2653
			public double q_ave;

			// Token: 0x04000A5E RID: 2654
			public double q_area;
		}
	}
}
