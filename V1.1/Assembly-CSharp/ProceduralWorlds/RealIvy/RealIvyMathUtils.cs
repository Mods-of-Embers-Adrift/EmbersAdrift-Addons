using System;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x02000098 RID: 152
	public static class RealIvyMathUtils
	{
		// Token: 0x060005DD RID: 1501 RVA: 0x000A4A50 File Offset: 0x000A2C50
		public static float DistanceBetweenPointAndSegmentSS(Vector2 point, Vector2 a, Vector2 b)
		{
			float num = (point.x - a.x) * (b.x - a.x) + (point.y - a.y) * (b.y - a.y);
			num /= (b.x - a.x) * (b.x - a.x) + (b.y - a.y) * (b.y - a.y);
			float sqrMagnitude;
			if (num < 0f)
			{
				sqrMagnitude = (point - a).sqrMagnitude;
			}
			else if (num >= 0f && num <= 1f)
			{
				Vector2 b2 = new Vector2(a.x + num * (b.x - a.x), a.y + num * (b.y - a.y));
				sqrMagnitude = (point - b2).sqrMagnitude;
			}
			else
			{
				sqrMagnitude = (point - b).sqrMagnitude;
			}
			return sqrMagnitude;
		}

		// Token: 0x02000099 RID: 153
		public struct Segment
		{
			// Token: 0x040006C6 RID: 1734
			public Vector2 a;

			// Token: 0x040006C7 RID: 1735
			public Vector2 b;
		}
	}
}
