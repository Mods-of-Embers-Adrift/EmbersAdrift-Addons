using System;
using System.Collections.Generic;
using SoL.Game;
using SoL.Game.Settings;
using UnityEngine;
using UnityEngine.AI;

namespace SoL.Utilities
{
	// Token: 0x02000250 RID: 592
	public static class CanReachCache
	{
		// Token: 0x06001344 RID: 4932 RVA: 0x000E90C4 File Offset: 0x000E72C4
		public static bool CanNpcReach(GameEntity target, GameEntity npcSource)
		{
			if (target && npcSource && npcSource.Type == GameEntityType.Npc)
			{
				int value = -1;
				Vector3 vector = npcSource.gameObject.transform.position;
				if (npcSource.Motor != null && npcSource.Motor.UnityNavAgent)
				{
					value = npcSource.Motor.UnityNavAgent.areaMask;
					vector -= Vector3.up * npcSource.Motor.UnityNavAgent.baseOffset;
				}
				return CanReachCache.CanReach(target.gameObject.transform.position, new Vector3?(vector), new int?(value));
			}
			return false;
		}

		// Token: 0x06001345 RID: 4933 RVA: 0x000E9178 File Offset: 0x000E7378
		private static bool CanReach(Vector3 targetPosition, Vector3? pathSource, int? mask)
		{
			if (CanReachCache.m_navResults == null)
			{
				CanReachCache.m_navPath = new NavMeshPath();
				CanReachCache.m_navResults = new Dictionary<int, bool>(1024);
			}
			IndexedVector3 indexedVector = new IndexedVector3(targetPosition, 2048, 1f);
			int key = (mask != null) ? (indexedVector.Index * 397 ^ mask.Value) : indexedVector.Index;
			int areaMask = (mask != null) ? mask.Value : -1;
			bool flag;
			if (!CanReachCache.m_navResults.TryGetValue(key, out flag))
			{
				NavMeshHit navMeshHit;
				flag = NavMeshUtilities.SamplePosition(indexedVector.Pos, out navMeshHit, GlobalSettings.Values.Npcs.CanHitSampleDistance, areaMask);
				if (flag && pathSource != null && !GlobalSettings.Values.Npcs.BypassCanHitPathCheck)
				{
					flag = NavMesh.CalculatePath(pathSource.Value, navMeshHit.position, areaMask, CanReachCache.m_navPath);
				}
				CanReachCache.m_navResults.Add(key, flag);
			}
			return flag;
		}

		// Token: 0x040010FC RID: 4348
		public const int kSize = 2048;

		// Token: 0x040010FD RID: 4349
		public const float kCellSize = 1f;

		// Token: 0x040010FE RID: 4350
		private static NavMeshPath m_navPath;

		// Token: 0x040010FF RID: 4351
		private static Dictionary<int, bool> m_navResults;
	}
}
