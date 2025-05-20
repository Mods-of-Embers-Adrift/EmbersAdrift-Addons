using System;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace TriangleNet.Tools
{
	// Token: 0x02000111 RID: 273
	public class Statistic
	{
		// Token: 0x1700030E RID: 782
		// (get) Token: 0x060009C7 RID: 2503 RVA: 0x00049938 File Offset: 0x00047B38
		public double ShortestEdge
		{
			get
			{
				return this.minEdge;
			}
		}

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x060009C8 RID: 2504 RVA: 0x00049940 File Offset: 0x00047B40
		public double LongestEdge
		{
			get
			{
				return this.maxEdge;
			}
		}

		// Token: 0x17000310 RID: 784
		// (get) Token: 0x060009C9 RID: 2505 RVA: 0x00049948 File Offset: 0x00047B48
		public double ShortestAltitude
		{
			get
			{
				return this.minAspect;
			}
		}

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x060009CA RID: 2506 RVA: 0x00049950 File Offset: 0x00047B50
		public double LargestAspectRatio
		{
			get
			{
				return this.maxAspect;
			}
		}

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x060009CB RID: 2507 RVA: 0x00049958 File Offset: 0x00047B58
		public double SmallestArea
		{
			get
			{
				return this.minArea;
			}
		}

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x060009CC RID: 2508 RVA: 0x00049960 File Offset: 0x00047B60
		public double LargestArea
		{
			get
			{
				return this.maxArea;
			}
		}

		// Token: 0x17000314 RID: 788
		// (get) Token: 0x060009CD RID: 2509 RVA: 0x00049968 File Offset: 0x00047B68
		public double SmallestAngle
		{
			get
			{
				return this.minAngle;
			}
		}

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x060009CE RID: 2510 RVA: 0x00049970 File Offset: 0x00047B70
		public double LargestAngle
		{
			get
			{
				return this.maxAngle;
			}
		}

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x060009CF RID: 2511 RVA: 0x00049978 File Offset: 0x00047B78
		public int[] AngleHistogram
		{
			get
			{
				return this.angleTable;
			}
		}

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x060009D0 RID: 2512 RVA: 0x00049980 File Offset: 0x00047B80
		public int[] MinAngleHistogram
		{
			get
			{
				return this.minAngles;
			}
		}

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x060009D1 RID: 2513 RVA: 0x00049988 File Offset: 0x00047B88
		public int[] MaxAngleHistogram
		{
			get
			{
				return this.maxAngles;
			}
		}

		// Token: 0x060009D2 RID: 2514 RVA: 0x000C21B8 File Offset: 0x000C03B8
		private void GetAspectHistogram(Mesh mesh)
		{
			int[] array = new int[16];
			double[] array2 = new double[]
			{
				1.5,
				2.0,
				2.5,
				3.0,
				4.0,
				6.0,
				10.0,
				15.0,
				25.0,
				50.0,
				100.0,
				300.0,
				1000.0,
				10000.0,
				100000.0,
				0.0
			};
			Otri otri = default(Otri);
			Vertex[] array3 = new Vertex[3];
			double[] array4 = new double[3];
			double[] array5 = new double[3];
			double[] array6 = new double[3];
			otri.orient = 0;
			foreach (Triangle tri in mesh.triangles)
			{
				otri.tri = tri;
				array3[0] = otri.Org();
				array3[1] = otri.Dest();
				array3[2] = otri.Apex();
				double num = 0.0;
				for (int i = 0; i < 3; i++)
				{
					int num2 = Statistic.plus1Mod3[i];
					int num3 = Statistic.minus1Mod3[i];
					array4[i] = array3[num2].x - array3[num3].x;
					array5[i] = array3[num2].y - array3[num3].y;
					array6[i] = array4[i] * array4[i] + array5[i] * array5[i];
					if (array6[i] > num)
					{
						num = array6[i];
					}
				}
				double num4 = Math.Abs((array3[2].x - array3[0].x) * (array3[1].y - array3[0].y) - (array3[1].x - array3[0].x) * (array3[2].y - array3[0].y)) / 2.0;
				double num5 = num4 * num4 / num;
				double num6 = num / num5;
				int num7 = 0;
				while (num6 > array2[num7] * array2[num7] && num7 < 15)
				{
					num7++;
				}
				array[num7]++;
			}
		}

		// Token: 0x060009D3 RID: 2515 RVA: 0x000C23A4 File Offset: 0x000C05A4
		public void Update(Mesh mesh, int sampleDegrees)
		{
			Point[] array = new Point[3];
			sampleDegrees = 60;
			double[] array2 = new double[sampleDegrees / 2 - 1];
			double[] array3 = new double[3];
			double[] array4 = new double[3];
			double[] array5 = new double[3];
			double num = 3.141592653589793 / (double)sampleDegrees;
			double num2 = 57.29577951308232;
			this.angleTable = new int[sampleDegrees];
			this.minAngles = new int[sampleDegrees];
			this.maxAngles = new int[sampleDegrees];
			for (int i = 0; i < sampleDegrees / 2 - 1; i++)
			{
				array2[i] = Math.Cos(num * (double)(i + 1));
				array2[i] *= array2[i];
			}
			for (int j = 0; j < sampleDegrees; j++)
			{
				this.angleTable[j] = 0;
			}
			this.minAspect = mesh.bounds.Width + mesh.bounds.Height;
			this.minAspect *= this.minAspect;
			this.maxAspect = 0.0;
			this.minEdge = this.minAspect;
			this.maxEdge = 0.0;
			this.minArea = this.minAspect;
			this.maxArea = 0.0;
			this.minAngle = 0.0;
			this.maxAngle = 2.0;
			bool flag = true;
			bool flag2 = true;
			foreach (Triangle triangle in mesh.triangles)
			{
				double num3 = 0.0;
				double num4 = 1.0;
				array[0] = triangle.vertices[0];
				array[1] = triangle.vertices[1];
				array[2] = triangle.vertices[2];
				double num5 = 0.0;
				for (int k = 0; k < 3; k++)
				{
					int num6 = Statistic.plus1Mod3[k];
					int num7 = Statistic.minus1Mod3[k];
					array3[k] = array[num6].x - array[num7].x;
					array4[k] = array[num6].y - array[num7].y;
					array5[k] = array3[k] * array3[k] + array4[k] * array4[k];
					if (array5[k] > num5)
					{
						num5 = array5[k];
					}
					if (array5[k] > this.maxEdge)
					{
						this.maxEdge = array5[k];
					}
					if (array5[k] < this.minEdge)
					{
						this.minEdge = array5[k];
					}
				}
				double num8 = Math.Abs((array[2].x - array[0].x) * (array[1].y - array[0].y) - (array[1].x - array[0].x) * (array[2].y - array[0].y));
				if (num8 < this.minArea)
				{
					this.minArea = num8;
				}
				if (num8 > this.maxArea)
				{
					this.maxArea = num8;
				}
				double num9 = num8 * num8 / num5;
				if (num9 < this.minAspect)
				{
					this.minAspect = num9;
				}
				double num10 = num5 / num9;
				if (num10 > this.maxAspect)
				{
					this.maxAspect = num10;
				}
				int num13;
				for (int l = 0; l < 3; l++)
				{
					int num6 = Statistic.plus1Mod3[l];
					int num7 = Statistic.minus1Mod3[l];
					double num11 = array3[num6] * array3[num7] + array4[num6] * array4[num7];
					double num12 = num11 * num11 / (array5[num6] * array5[num7]);
					num13 = sampleDegrees / 2 - 1;
					for (int m = num13 - 1; m >= 0; m--)
					{
						if (num12 > array2[m])
						{
							num13 = m;
						}
					}
					if (num11 <= 0.0)
					{
						this.angleTable[num13]++;
						if (num12 > this.minAngle)
						{
							this.minAngle = num12;
						}
						if (flag && num12 < this.maxAngle)
						{
							this.maxAngle = num12;
						}
						if (num12 > num3)
						{
							num3 = num12;
						}
						if (flag2 && num12 < num4)
						{
							num4 = num12;
						}
					}
					else
					{
						this.angleTable[sampleDegrees - num13 - 1]++;
						if (flag || num12 > this.maxAngle)
						{
							this.maxAngle = num12;
							flag = false;
						}
						if (flag2 || num12 > num4)
						{
							num4 = num12;
							flag2 = false;
						}
					}
				}
				num13 = sampleDegrees / 2 - 1;
				for (int n = num13 - 1; n >= 0; n--)
				{
					if (num3 > array2[n])
					{
						num13 = n;
					}
				}
				this.minAngles[num13]++;
				num13 = sampleDegrees / 2 - 1;
				for (int num14 = num13 - 1; num14 >= 0; num14--)
				{
					if (num4 > array2[num14])
					{
						num13 = num14;
					}
				}
				if (flag2)
				{
					this.maxAngles[num13]++;
				}
				else
				{
					this.maxAngles[sampleDegrees - num13 - 1]++;
				}
				flag2 = true;
			}
			this.minEdge = Math.Sqrt(this.minEdge);
			this.maxEdge = Math.Sqrt(this.maxEdge);
			this.minAspect = Math.Sqrt(this.minAspect);
			this.maxAspect = Math.Sqrt(this.maxAspect);
			this.minArea *= 0.5;
			this.maxArea *= 0.5;
			if (this.minAngle >= 1.0)
			{
				this.minAngle = 0.0;
			}
			else
			{
				this.minAngle = num2 * Math.Acos(Math.Sqrt(this.minAngle));
			}
			if (this.maxAngle >= 1.0)
			{
				this.maxAngle = 180.0;
				return;
			}
			if (flag)
			{
				this.maxAngle = num2 * Math.Acos(Math.Sqrt(this.maxAngle));
				return;
			}
			this.maxAngle = 180.0 - num2 * Math.Acos(Math.Sqrt(this.maxAngle));
		}

		// Token: 0x060009D5 RID: 2517 RVA: 0x000C29A4 File Offset: 0x000C0BA4
		// Note: this type is marked as 'beforefieldinit'.
		static Statistic()
		{
			int[] array = new int[3];
			array[0] = 1;
			array[1] = 2;
			Statistic.plus1Mod3 = array;
			Statistic.minus1Mod3 = new int[]
			{
				2,
				0,
				1
			};
		}

		// Token: 0x04000A5F RID: 2655
		public static long InCircleCount = 0L;

		// Token: 0x04000A60 RID: 2656
		public static long InCircleAdaptCount = 0L;

		// Token: 0x04000A61 RID: 2657
		public static long CounterClockwiseCount = 0L;

		// Token: 0x04000A62 RID: 2658
		public static long CounterClockwiseAdaptCount = 0L;

		// Token: 0x04000A63 RID: 2659
		public static long Orient3dCount = 0L;

		// Token: 0x04000A64 RID: 2660
		public static long HyperbolaCount = 0L;

		// Token: 0x04000A65 RID: 2661
		public static long CircumcenterCount = 0L;

		// Token: 0x04000A66 RID: 2662
		public static long CircleTopCount = 0L;

		// Token: 0x04000A67 RID: 2663
		public static long RelocationCount = 0L;

		// Token: 0x04000A68 RID: 2664
		private double minEdge;

		// Token: 0x04000A69 RID: 2665
		private double maxEdge;

		// Token: 0x04000A6A RID: 2666
		private double minAspect;

		// Token: 0x04000A6B RID: 2667
		private double maxAspect;

		// Token: 0x04000A6C RID: 2668
		private double minArea;

		// Token: 0x04000A6D RID: 2669
		private double maxArea;

		// Token: 0x04000A6E RID: 2670
		private double minAngle;

		// Token: 0x04000A6F RID: 2671
		private double maxAngle;

		// Token: 0x04000A70 RID: 2672
		private int[] angleTable;

		// Token: 0x04000A71 RID: 2673
		private int[] minAngles;

		// Token: 0x04000A72 RID: 2674
		private int[] maxAngles;

		// Token: 0x04000A73 RID: 2675
		private static readonly int[] plus1Mod3;

		// Token: 0x04000A74 RID: 2676
		private static readonly int[] minus1Mod3;
	}
}
