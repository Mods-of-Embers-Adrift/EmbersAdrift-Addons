using System;

namespace SoL.Game.Culling
{
	// Token: 0x02000CC4 RID: 3268
	public static class CullingDistanceExt
	{
		// Token: 0x170017AD RID: 6061
		// (get) Token: 0x06006310 RID: 25360 RVA: 0x00082C35 File Offset: 0x00080E35
		public static CullingDistance[] CullingDistances
		{
			get
			{
				if (CullingDistanceExt.m_cullingDistances == null)
				{
					CullingDistanceExt.m_cullingDistances = (CullingDistance[])Enum.GetValues(typeof(CullingDistance));
				}
				return CullingDistanceExt.m_cullingDistances;
			}
		}

		// Token: 0x06006311 RID: 25361 RVA: 0x00206014 File Offset: 0x00204214
		public static float GetDistance(this CullingDistance d)
		{
			switch (d)
			{
			case CullingDistance.VeryNear:
				return 30f;
			case CullingDistance.Near:
				return 60f;
			case CullingDistance.Average:
				return 90f;
			case CullingDistance.Far:
				return 120f;
			case CullingDistance.VeryFar:
				return 150f;
			case CullingDistance.Extreme:
				return 300f;
			case CullingDistance.NotCulled:
				return 10000f;
			default:
				return 10f;
			}
		}

		// Token: 0x06006312 RID: 25362 RVA: 0x00082C5C File Offset: 0x00080E5C
		public static CullingDistance GetNextDistance(this CullingDistance d)
		{
			switch (d)
			{
			case CullingDistance.VeryNear:
				return CullingDistance.Near;
			case CullingDistance.Near:
				return CullingDistance.Average;
			case CullingDistance.Average:
				return CullingDistance.Far;
			case CullingDistance.Far:
				return CullingDistance.VeryFar;
			case CullingDistance.VeryFar:
				return CullingDistance.Extreme;
			case CullingDistance.Extreme:
				return CullingDistance.NotCulled;
			default:
				throw new ArgumentException("d");
			}
		}

		// Token: 0x06006313 RID: 25363 RVA: 0x00082C94 File Offset: 0x00080E94
		public static bool ShouldBeCulled(this CullingDistance d, CullingDistance currentBand)
		{
			return currentBand.GetDistance() > d.GetDistance();
		}

		// Token: 0x06006314 RID: 25364 RVA: 0x00082CA4 File Offset: 0x00080EA4
		public static bool IsLowestBand(this CullingDistance distance)
		{
			return distance.GetDistance() <= CullingDistance.VeryNear.GetDistance();
		}

		// Token: 0x04005640 RID: 22080
		private static CullingDistance[] m_cullingDistances;
	}
}
