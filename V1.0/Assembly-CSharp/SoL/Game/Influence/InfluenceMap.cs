using System;
using System.Collections.Generic;
using SoL.Managers;
using SoL.Networking.Objects;
using UnityEngine;

namespace SoL.Game.Influence
{
	// Token: 0x02000BC2 RID: 3010
	public static class InfluenceMap
	{
		// Token: 0x06005D1F RID: 23839 RVA: 0x001F2C00 File Offset: 0x001F0E00
		public static float SampleValueAtPoint(Vector3 samplePoint, float maxRadius, InfluenceFlags flags, bool runQuery = true)
		{
			if (ServerGameManager.SpatialManager == null)
			{
				return 0f;
			}
			if (runQuery)
			{
				ServerGameManager.SpatialManager.QueryRadius(samplePoint, 2f * maxRadius, InfluenceMap.m_results);
			}
			float num = 0f;
			for (int i = 0; i < InfluenceMap.m_results.Count; i++)
			{
				if (InfluenceMap.m_results[i].GameEntity != null && InfluenceMap.m_results[i].GameEntity.InfluenceSource != null)
				{
					num += InfluenceMap.m_results[i].GameEntity.InfluenceSource.GetInfluence(samplePoint, flags);
				}
			}
			return num;
		}

		// Token: 0x0400508B RID: 20619
		private const float kRadiusExtension = 2f;

		// Token: 0x0400508C RID: 20620
		private static readonly List<NetworkEntity> m_results = new List<NetworkEntity>(10);
	}
}
