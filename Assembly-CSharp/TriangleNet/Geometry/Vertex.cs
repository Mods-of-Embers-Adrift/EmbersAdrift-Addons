using System;
using TriangleNet.Topology;

namespace TriangleNet.Geometry
{
	// Token: 0x02000150 RID: 336
	public class Vertex : Point
	{
		// Token: 0x06000B82 RID: 2946 RVA: 0x0004A6BD File Offset: 0x000488BD
		public Vertex() : this(0.0, 0.0, 0)
		{
		}

		// Token: 0x06000B83 RID: 2947 RVA: 0x0004A6D8 File Offset: 0x000488D8
		public Vertex(double x, double y) : this(x, y, 0)
		{
		}

		// Token: 0x06000B84 RID: 2948 RVA: 0x0004A6E3 File Offset: 0x000488E3
		public Vertex(double x, double y, int mark) : base(x, y, mark)
		{
			this.type = VertexType.InputVertex;
		}

		// Token: 0x17000366 RID: 870
		// (get) Token: 0x06000B85 RID: 2949 RVA: 0x0004A6F5 File Offset: 0x000488F5
		public VertexType Type
		{
			get
			{
				return this.type;
			}
		}

		// Token: 0x17000367 RID: 871
		public double this[int i]
		{
			get
			{
				if (i == 0)
				{
					return this.x;
				}
				if (i == 1)
				{
					return this.y;
				}
				throw new ArgumentOutOfRangeException("Index must be 0 or 1.");
			}
		}

		// Token: 0x06000B87 RID: 2951 RVA: 0x0004A71E File Offset: 0x0004891E
		public override int GetHashCode()
		{
			return this.hash;
		}

		// Token: 0x04000B24 RID: 2852
		internal int hash;

		// Token: 0x04000B25 RID: 2853
		internal VertexType type;

		// Token: 0x04000B26 RID: 2854
		internal Otri tri;
	}
}
