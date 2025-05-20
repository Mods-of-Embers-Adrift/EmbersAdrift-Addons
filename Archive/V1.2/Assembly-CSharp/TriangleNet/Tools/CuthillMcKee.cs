using System;

namespace TriangleNet.Tools
{
	// Token: 0x02000109 RID: 265
	public class CuthillMcKee
	{
		// Token: 0x06000994 RID: 2452 RVA: 0x000496B1 File Offset: 0x000478B1
		public int[] Renumber(Mesh mesh)
		{
			mesh.Renumber(NodeNumbering.Linear);
			return this.Renumber(new AdjacencyMatrix(mesh));
		}

		// Token: 0x06000995 RID: 2453 RVA: 0x000C0C7C File Offset: 0x000BEE7C
		public int[] Renumber(AdjacencyMatrix matrix)
		{
			this.matrix = matrix;
			int num = matrix.Bandwidth();
			int[] columnPointers = matrix.ColumnPointers;
			this.Shift(columnPointers, true);
			int[] perm = this.GenerateRcm();
			int[] array = this.PermInverse(perm);
			int num2 = this.PermBandwidth(perm, array);
			if (Log.Verbose)
			{
				Log.Instance.Info(string.Format("Reverse Cuthill-McKee (Bandwidth: {0} > {1})", num, num2));
			}
			this.Shift(columnPointers, false);
			return array;
		}

		// Token: 0x06000996 RID: 2454 RVA: 0x000C0CF0 File Offset: 0x000BEEF0
		private int[] GenerateRcm()
		{
			int n = this.matrix.N;
			int[] array = new int[n];
			int num = 0;
			int num2 = 0;
			int[] level_row = new int[n + 1];
			int[] array2 = new int[n];
			for (int i = 0; i < n; i++)
			{
				array2[i] = 1;
			}
			int num3 = 1;
			for (int i = 0; i < n; i++)
			{
				if (array2[i] != 0)
				{
					int root = i;
					this.FindRoot(ref root, array2, ref num2, level_row, array, num3 - 1);
					this.Rcm(root, array2, array, num3 - 1, ref num);
					num3 += num;
					if (n < num3)
					{
						return array;
					}
				}
			}
			return array;
		}

		// Token: 0x06000997 RID: 2455 RVA: 0x000C0D80 File Offset: 0x000BEF80
		private void Rcm(int root, int[] mask, int[] perm, int offset, ref int iccsze)
		{
			int[] columnPointers = this.matrix.ColumnPointers;
			int[] rowIndices = this.matrix.RowIndices;
			int[] array = new int[this.matrix.N];
			this.Degree(root, mask, array, ref iccsze, perm, offset);
			mask[root] = 0;
			if (iccsze <= 1)
			{
				return;
			}
			int i = 0;
			int num = 1;
			while (i < num)
			{
				int num2 = i + 1;
				i = num;
				for (int j = num2; j <= i; j++)
				{
					int num3 = perm[offset + j - 1];
					int num4 = columnPointers[num3];
					int num5 = columnPointers[num3 + 1] - 1;
					int k = num + 1;
					for (int l = num4; l <= num5; l++)
					{
						int num6 = rowIndices[l - 1];
						if (mask[num6] != 0)
						{
							num++;
							mask[num6] = 0;
							perm[offset + num - 1] = num6;
						}
					}
					if (num > k)
					{
						int m = k;
						while (m < num)
						{
							int num7 = m;
							m++;
							int num6 = perm[offset + m - 1];
							while (k < num7)
							{
								int num8 = perm[offset + num7 - 1];
								if (array[num8 - 1] <= array[num6 - 1])
								{
									break;
								}
								perm[offset + num7] = num8;
								num7--;
							}
							perm[offset + num7] = num6;
						}
					}
				}
			}
			this.ReverseVector(perm, offset, iccsze);
		}

		// Token: 0x06000998 RID: 2456 RVA: 0x000C0EC0 File Offset: 0x000BF0C0
		private void FindRoot(ref int root, int[] mask, ref int level_num, int[] level_row, int[] level, int offset)
		{
			int[] columnPointers = this.matrix.ColumnPointers;
			int[] rowIndices = this.matrix.RowIndices;
			int num = 0;
			this.GetLevelSet(ref root, mask, ref level_num, level_row, level, offset);
			int num2 = level_row[level_num] - 1;
			if (level_num == 1 || level_num == num2)
			{
				return;
			}
			do
			{
				int num3 = num2;
				int num4 = level_row[level_num - 1];
				root = level[offset + num4 - 1];
				if (num4 < num2)
				{
					for (int i = num4; i <= num2; i++)
					{
						int num5 = level[offset + i - 1];
						int num6 = 0;
						int num7 = columnPointers[num5 - 1];
						int num8 = columnPointers[num5] - 1;
						for (int j = num7; j <= num8; j++)
						{
							int num9 = rowIndices[j - 1];
							if (mask[num9] > 0)
							{
								num6++;
							}
						}
						if (num6 < num3)
						{
							root = num5;
							num3 = num6;
						}
					}
				}
				this.GetLevelSet(ref root, mask, ref num, level_row, level, offset);
				if (num <= level_num)
				{
					break;
				}
				level_num = num;
			}
			while (num2 > level_num);
		}

		// Token: 0x06000999 RID: 2457 RVA: 0x000C0FAC File Offset: 0x000BF1AC
		private void GetLevelSet(ref int root, int[] mask, ref int level_num, int[] level_row, int[] level, int offset)
		{
			int[] columnPointers = this.matrix.ColumnPointers;
			int[] rowIndices = this.matrix.RowIndices;
			mask[root] = 0;
			level[offset] = root;
			level_num = 0;
			int num = 0;
			int num2 = 1;
			do
			{
				int num3 = num + 1;
				num = num2;
				level_num++;
				level_row[level_num - 1] = num3;
				for (int i = num3; i <= num; i++)
				{
					int num4 = level[offset + i - 1];
					int num5 = columnPointers[num4];
					int num6 = columnPointers[num4 + 1] - 1;
					for (int j = num5; j <= num6; j++)
					{
						int num7 = rowIndices[j - 1];
						if (mask[num7] != 0)
						{
							num2++;
							level[offset + num2 - 1] = num7;
							mask[num7] = 0;
						}
					}
				}
			}
			while (num2 - num > 0);
			level_row[level_num] = num + 1;
			for (int i = 0; i < num2; i++)
			{
				mask[level[offset + i]] = 1;
			}
		}

		// Token: 0x0600099A RID: 2458 RVA: 0x000C1080 File Offset: 0x000BF280
		private void Degree(int root, int[] mask, int[] deg, ref int iccsze, int[] ls, int offset)
		{
			int[] columnPointers = this.matrix.ColumnPointers;
			int[] rowIndices = this.matrix.RowIndices;
			int i = 1;
			ls[offset] = root;
			columnPointers[root] = -columnPointers[root];
			int num = 0;
			iccsze = 1;
			while (i > 0)
			{
				int num2 = num + 1;
				num = iccsze;
				for (int j = num2; j <= num; j++)
				{
					int num3 = ls[offset + j - 1];
					int num4 = -columnPointers[num3];
					int num5 = Math.Abs(columnPointers[num3 + 1]) - 1;
					int num6 = 0;
					for (int k = num4; k <= num5; k++)
					{
						int num7 = rowIndices[k - 1];
						if (mask[num7] != 0)
						{
							num6++;
							if (0 <= columnPointers[num7])
							{
								columnPointers[num7] = -columnPointers[num7];
								iccsze++;
								ls[offset + iccsze - 1] = num7;
							}
						}
					}
					deg[num3] = num6;
				}
				i = iccsze - num;
			}
			for (int j = 0; j < iccsze; j++)
			{
				int num3 = ls[offset + j];
				columnPointers[num3] = -columnPointers[num3];
			}
		}

		// Token: 0x0600099B RID: 2459 RVA: 0x000C1174 File Offset: 0x000BF374
		private int PermBandwidth(int[] perm, int[] perm_inv)
		{
			int[] columnPointers = this.matrix.ColumnPointers;
			int[] rowIndices = this.matrix.RowIndices;
			int num = 0;
			int num2 = 0;
			int n = this.matrix.N;
			for (int i = 0; i < n; i++)
			{
				for (int j = columnPointers[perm[i]]; j < columnPointers[perm[i] + 1]; j++)
				{
					int num3 = perm_inv[rowIndices[j - 1]];
					num = Math.Max(num, i - num3);
					num2 = Math.Max(num2, num3 - i);
				}
			}
			return num + 1 + num2;
		}

		// Token: 0x0600099C RID: 2460 RVA: 0x000C11FC File Offset: 0x000BF3FC
		private int[] PermInverse(int[] perm)
		{
			int n = this.matrix.N;
			int[] array = new int[n];
			for (int i = 0; i < n; i++)
			{
				array[perm[i]] = i;
			}
			return array;
		}

		// Token: 0x0600099D RID: 2461 RVA: 0x000C1230 File Offset: 0x000BF430
		private void ReverseVector(int[] a, int offset, int size)
		{
			for (int i = 0; i < size / 2; i++)
			{
				int num = a[offset + i];
				a[offset + i] = a[offset + size - 1 - i];
				a[offset + size - 1 - i] = num;
			}
		}

		// Token: 0x0600099E RID: 2462 RVA: 0x000C126C File Offset: 0x000BF46C
		private void Shift(int[] a, bool up)
		{
			int num = a.Length;
			if (up)
			{
				for (int i = 0; i < num; i++)
				{
					a[i]++;
				}
				return;
			}
			for (int j = 0; j < num; j++)
			{
				a[j]--;
			}
		}

		// Token: 0x04000A4E RID: 2638
		private AdjacencyMatrix matrix;
	}
}
