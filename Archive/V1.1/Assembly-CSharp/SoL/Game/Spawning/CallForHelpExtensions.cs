using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x02000670 RID: 1648
	public static class CallForHelpExtensions
	{
		// Token: 0x06003324 RID: 13092 RVA: 0x0004FB40 File Offset: 0x0004DD40
		public static bool HasBitFlag(this CallForHelpFlags a, CallForHelpFlags b)
		{
			return (a & b) == b;
		}

		// Token: 0x06003325 RID: 13093 RVA: 0x0006334C File Offset: 0x0006154C
		public static void SortListBySqrMagnitudeDistance(GameEntity sourceEntity, List<GameEntity> listToSort)
		{
			if (!CallForHelpExtensions.m_sourceEntity || listToSort == null || listToSort.Count <= 0)
			{
				return;
			}
			CallForHelpExtensions.m_sourceEntity = sourceEntity;
			listToSort.Sort(CallForHelpExtensions.m_distanceSorter);
		}

		// Token: 0x06003326 RID: 13094 RVA: 0x00162050 File Offset: 0x00160250
		private static int SortBySqrMagnitudeDistance(GameEntity x, GameEntity y)
		{
			if (!x || !y)
			{
				return 0;
			}
			Vector3 position = CallForHelpExtensions.m_sourceEntity.gameObject.transform.position;
			float sqrMagnitude = (x.gameObject.transform.position - position).sqrMagnitude;
			float sqrMagnitude2 = (y.gameObject.transform.position - position).sqrMagnitude;
			x.SqrDistanceFromLastNpcCallForHelp = sqrMagnitude;
			y.SqrDistanceFromLastNpcCallForHelp = sqrMagnitude2;
			return sqrMagnitude.CompareTo(sqrMagnitude2);
		}

		// Token: 0x04003161 RID: 12641
		private static GameEntity m_sourceEntity = null;

		// Token: 0x04003162 RID: 12642
		private static readonly Comparer<GameEntity> m_distanceSorter = Comparer<GameEntity>.Create(new Comparison<GameEntity>(CallForHelpExtensions.SortBySqrMagnitudeDistance));
	}
}
