using System;
using System.Globalization;
using Cysharp.Text;
using UnityEngine;

namespace SoL.Game.Objects
{
	// Token: 0x020009FC RID: 2556
	[Serializable]
	public struct MinMaxFloatRange : IEquatable<MinMaxFloatRange>
	{
		// Token: 0x17001123 RID: 4387
		// (get) Token: 0x06004D9F RID: 19871 RVA: 0x00074786 File Offset: 0x00072986
		public float Min
		{
			get
			{
				return this.m_min;
			}
		}

		// Token: 0x17001124 RID: 4388
		// (get) Token: 0x06004DA0 RID: 19872 RVA: 0x0007478E File Offset: 0x0007298E
		public float Max
		{
			get
			{
				return this.m_max;
			}
		}

		// Token: 0x17001125 RID: 4389
		// (get) Token: 0x06004DA1 RID: 19873 RVA: 0x00074796 File Offset: 0x00072996
		public float Delta
		{
			get
			{
				return this.m_max - this.m_min;
			}
		}

		// Token: 0x17001126 RID: 4390
		// (get) Token: 0x06004DA2 RID: 19874 RVA: 0x000747A5 File Offset: 0x000729A5
		public bool IsZero
		{
			get
			{
				return this.m_min == 0f && this.m_max == 0f;
			}
		}

		// Token: 0x06004DA3 RID: 19875 RVA: 0x000747C3 File Offset: 0x000729C3
		public MinMaxFloatRange(float min, float max)
		{
			this.m_min = min;
			this.m_max = max;
		}

		// Token: 0x06004DA4 RID: 19876 RVA: 0x000747D3 File Offset: 0x000729D3
		public MinMaxFloatRange(Vector2 values)
		{
			this.m_min = values.x;
			this.m_max = values.y;
		}

		// Token: 0x06004DA5 RID: 19877 RVA: 0x000747ED File Offset: 0x000729ED
		public bool WithinRange(float value)
		{
			return value >= this.m_min && value <= this.m_max;
		}

		// Token: 0x06004DA6 RID: 19878 RVA: 0x00074806 File Offset: 0x00072A06
		public float Clamp(float value)
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

		// Token: 0x06004DA7 RID: 19879 RVA: 0x00074829 File Offset: 0x00072A29
		public float RandomWithinRange()
		{
			return UnityEngine.Random.Range(this.m_min, this.m_max);
		}

		// Token: 0x06004DA8 RID: 19880 RVA: 0x001C0CFC File Offset: 0x001BEEFC
		public float RandomWithinRange(System.Random random)
		{
			float t = (float)random.NextDouble();
			return Mathf.Lerp(this.m_min, this.m_max, t);
		}

		// Token: 0x06004DA9 RID: 19881 RVA: 0x001C0D24 File Offset: 0x001BEF24
		public float GetScaledValueForMasteryLevel(MinMaxIntRange range, float value)
		{
			float num = Mathf.Floor(value);
			if (num <= (float)range.Min)
			{
				return this.m_min;
			}
			if (num >= (float)range.Max)
			{
				return this.m_max;
			}
			float t = (num - (float)range.Min) / (float)range.Delta;
			return Mathf.Lerp(this.m_min, this.m_max, t);
		}

		// Token: 0x06004DAA RID: 19882 RVA: 0x0007483C File Offset: 0x00072A3C
		public override string ToString()
		{
			return this.m_min.ToString(CultureInfo.InvariantCulture) + "-" + this.m_max.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x06004DAB RID: 19883 RVA: 0x001C0D84 File Offset: 0x001BEF84
		public string GetRangeDisplay()
		{
			if (this.m_min != 0f)
			{
				return ZString.Format<int, int>("<b>{0}-{1}m</b> Range", Mathf.FloorToInt(this.m_min), Mathf.FloorToInt(this.m_max));
			}
			return ZString.Format<int>("<b>{0}m</b> Range", Mathf.FloorToInt(this.m_max));
		}

		// Token: 0x06004DAC RID: 19884 RVA: 0x00074868 File Offset: 0x00072A68
		public bool Equals(MinMaxFloatRange other)
		{
			return this.m_min.Equals(other.m_min) && this.m_max.Equals(other.m_max);
		}

		// Token: 0x06004DAD RID: 19885 RVA: 0x001C0DD4 File Offset: 0x001BEFD4
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is MinMaxFloatRange)
			{
				MinMaxFloatRange other = (MinMaxFloatRange)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06004DAE RID: 19886 RVA: 0x00074890 File Offset: 0x00072A90
		public override int GetHashCode()
		{
			return this.m_min.GetHashCode() * 397 ^ this.m_max.GetHashCode();
		}

		// Token: 0x06004DAF RID: 19887 RVA: 0x000748AF File Offset: 0x00072AAF
		public static bool operator ==(MinMaxFloatRange a, MinMaxFloatRange b)
		{
			return a.Min == b.Min && a.Max == b.Max;
		}

		// Token: 0x06004DB0 RID: 19888 RVA: 0x000748D3 File Offset: 0x00072AD3
		public static bool operator !=(MinMaxFloatRange a, MinMaxFloatRange b)
		{
			return !(a == b);
		}

		// Token: 0x04004736 RID: 18230
		[SerializeField]
		private float m_min;

		// Token: 0x04004737 RID: 18231
		[SerializeField]
		private float m_max;
	}
}
