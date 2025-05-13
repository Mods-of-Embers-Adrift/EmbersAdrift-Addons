using System;
using UnityEngine;

namespace SoL.Utilities.Extensions
{
	// Token: 0x02000327 RID: 807
	public static class BoundsExtensions
	{
		// Token: 0x06001648 RID: 5704 RVA: 0x000FF974 File Offset: 0x000FDB74
		public static bool OutOfBounds(this Bounds bounds, Vector3 position)
		{
			Vector3 vector = bounds.center + bounds.extents;
			Vector3 vector2 = bounds.center - bounds.extents;
			return position.x < vector2.x || position.x > vector.x || position.y < vector2.y || position.y > vector.y || position.z < vector2.z || position.z > vector.z;
		}

		// Token: 0x06001649 RID: 5705 RVA: 0x000518F2 File Offset: 0x0004FAF2
		public static bool WithinBounds(this Bounds bounds, Vector3 position)
		{
			return !bounds.OutOfBounds(position);
		}
	}
}
