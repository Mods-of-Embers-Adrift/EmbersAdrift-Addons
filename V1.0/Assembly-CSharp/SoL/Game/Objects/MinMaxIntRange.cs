using System;
using UnityEngine;

namespace SoL.Game.Objects
{
	// Token: 0x020009FD RID: 2557
	[Serializable]
	public struct MinMaxIntRange : IEquatable<MinMaxIntRange>
	{
		// Token: 0x17001127 RID: 4391
		// (get) Token: 0x06004DB1 RID: 19889 RVA: 0x000748DF File Offset: 0x00072ADF
		public int Min
		{
			get
			{
				return this.m_min;
			}
		}

		// Token: 0x17001128 RID: 4392
		// (get) Token: 0x06004DB2 RID: 19890 RVA: 0x000748E7 File Offset: 0x00072AE7
		public int Max
		{
			get
			{
				return this.m_max;
			}
		}

		// Token: 0x17001129 RID: 4393
		// (get) Token: 0x06004DB3 RID: 19891 RVA: 0x000748EF File Offset: 0x00072AEF
		public int Delta
		{
			get
			{
				return this.m_max - this.m_min;
			}
		}

		// Token: 0x1700112A RID: 4394
		// (get) Token: 0x06004DB4 RID: 19892 RVA: 0x000748FE File Offset: 0x00072AFE
		public bool IsZero
		{
			get
			{
				return this.m_min == 0 && this.m_max == 0;
			}
		}

		// Token: 0x06004DB5 RID: 19893 RVA: 0x00074913 File Offset: 0x00072B13
		public MinMaxIntRange(int min, int max)
		{
			this.m_min = min;
			this.m_max = max;
		}

		// Token: 0x06004DB6 RID: 19894 RVA: 0x00074923 File Offset: 0x00072B23
		public MinMaxIntRange(Vector2 values)
		{
			this.m_min = (int)values.x;
			this.m_max = (int)values.y;
		}

		// Token: 0x06004DB7 RID: 19895 RVA: 0x0007493F File Offset: 0x00072B3F
		public bool WithinRange(int value)
		{
			return value >= this.m_min && value <= this.m_max;
		}

		// Token: 0x06004DB8 RID: 19896 RVA: 0x00074958 File Offset: 0x00072B58
		public int Clamp(int value)
		{
			if (value < this.m_min)
			{
				return this.m_min;
			}
			if (value > this.m_max)
			{
				return this.m_max;
			}
			return value;
		}

		// Token: 0x06004DB9 RID: 19897 RVA: 0x0007497B File Offset: 0x00072B7B
		public int RandomWithinRange()
		{
			return UnityEngine.Random.Range(this.m_min, this.m_max + 1);
		}

		// Token: 0x06004DBA RID: 19898 RVA: 0x00074990 File Offset: 0x00072B90
		public float RandomWithinRange(System.Random random)
		{
			return (float)random.Next(this.m_min, this.m_max + 1);
		}

		// Token: 0x06004DBB RID: 19899 RVA: 0x001C0DA4 File Offset: 0x001BEFA4
		public int GetScaledValue(MinMaxIntRange range, float value)
		{
			if (value <= (float)range.Min)
			{
				return this.m_min;
			}
			if (value >= (float)range.Max)
			{
				return this.m_max;
			}
			float t = (value - (float)range.Min) / (float)range.Delta;
			return (int)Mathf.Lerp((float)this.m_min, (float)this.m_max, t);
		}

		// Token: 0x06004DBC RID: 19900 RVA: 0x000749A7 File Offset: 0x00072BA7
		public override string ToString()
		{
			return this.m_min.ToString() + "-" + this.m_max.ToString();
		}

		// Token: 0x06004DBD RID: 19901 RVA: 0x000749C9 File Offset: 0x00072BC9
		public bool Equals(MinMaxIntRange other)
		{
			return this.m_min == other.m_min && this.m_max == other.m_max;
		}

		// Token: 0x06004DBE RID: 19902 RVA: 0x001C0E00 File Offset: 0x001BF000
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is MinMaxIntRange)
			{
				MinMaxIntRange other = (MinMaxIntRange)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06004DBF RID: 19903 RVA: 0x000749E9 File Offset: 0x00072BE9
		public override int GetHashCode()
		{
			return this.m_min * 397 ^ this.m_max;
		}

		// Token: 0x06004DC0 RID: 19904 RVA: 0x000749FE File Offset: 0x00072BFE
		public static bool operator ==(MinMaxIntRange a, MinMaxIntRange b)
		{
			return a.Min == b.Min && a.Max == b.Max;
		}

		// Token: 0x06004DC1 RID: 19905 RVA: 0x00074A22 File Offset: 0x00072C22
		public static bool operator !=(MinMaxIntRange a, MinMaxIntRange b)
		{
			return !(a == b);
		}

		// Token: 0x04004738 RID: 18232
		[SerializeField]
		private int m_min;

		// Token: 0x04004739 RID: 18233
		[SerializeField]
		private int m_max;
	}
}
