using System;
using UnityEngine;

namespace SoL.Utilities.Extensions
{
	// Token: 0x02000341 RID: 833
	public static class VectorExtensions
	{
		// Token: 0x060016D4 RID: 5844 RVA: 0x00051FA9 File Offset: 0x000501A9
		public static Vector3 RotateAroundPivot(this Vector3 point, Vector3 pivot, Vector3 angle)
		{
			return Quaternion.Euler(angle) * (point - pivot) + pivot;
		}

		// Token: 0x060016D5 RID: 5845 RVA: 0x001014F0 File Offset: 0x000FF6F0
		public static float Max(this Vector3 value)
		{
			float num = float.MinValue;
			if (value.x > num)
			{
				num = value.x;
			}
			if (value.y > num)
			{
				num = value.y;
			}
			if (value.z > num)
			{
				num = value.z;
			}
			return num;
		}
	}
}
