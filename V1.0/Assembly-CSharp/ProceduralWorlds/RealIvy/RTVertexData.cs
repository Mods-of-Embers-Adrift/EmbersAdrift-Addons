using System;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x020000A6 RID: 166
	[Serializable]
	public struct RTVertexData
	{
		// Token: 0x06000676 RID: 1654 RVA: 0x000476E2 File Offset: 0x000458E2
		public RTVertexData(Vector3 vertex, Vector3 normal, Vector2 uv, Vector2 uv2, Color color)
		{
			this.vertex = vertex;
			this.normal = normal;
			this.uv = uv;
			this.uv2 = uv2;
			this.color = color;
		}

		// Token: 0x0400076C RID: 1900
		public Vector3 vertex;

		// Token: 0x0400076D RID: 1901
		public Vector3 normal;

		// Token: 0x0400076E RID: 1902
		public Vector2 uv;

		// Token: 0x0400076F RID: 1903
		public Vector2 uv2;

		// Token: 0x04000770 RID: 1904
		public Color color;
	}
}
