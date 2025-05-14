using System;
using UnityEngine;
using UnityEngine.AI;

namespace SoL.Game.NPCs
{
	// Token: 0x02000809 RID: 2057
	public static class NpcMovementModeExtensions
	{
		// Token: 0x06003B96 RID: 15254 RVA: 0x0017C25C File Offset: 0x0017A45C
		public static int GetNavMeshAgentPriority(this NpcMovementMode mode)
		{
			switch (mode)
			{
			case NpcMovementMode.Idle:
				return UnityEngine.Random.Range(90, 100);
			case NpcMovementMode.Wander:
			case NpcMovementMode.Interact:
				return UnityEngine.Random.Range(70, 90);
			case NpcMovementMode.Return:
			case NpcMovementMode.Reset:
				return UnityEngine.Random.Range(50, 70);
			case NpcMovementMode.Search:
				return UnityEngine.Random.Range(30, 50);
			case NpcMovementMode.Pursue:
			case NpcMovementMode.Combat:
			case NpcMovementMode.MoveBack:
				return UnityEngine.Random.Range(0, 30);
			default:
				return 99;
			}
		}

		// Token: 0x06003B97 RID: 15255 RVA: 0x000684FF File Offset: 0x000666FF
		public static ObstacleAvoidanceType GetAvoidanceType(this NpcMovementMode mode)
		{
			if (mode == NpcMovementMode.Pursue || mode - NpcMovementMode.Combat <= 1)
			{
				return ObstacleAvoidanceType.LowQualityObstacleAvoidance;
			}
			return ObstacleAvoidanceType.NoObstacleAvoidance;
		}

		// Token: 0x06003B98 RID: 15256 RVA: 0x0006850E File Offset: 0x0006670E
		public static bool IsCombatVariant(this NpcMovementMode mode)
		{
			return mode - NpcMovementMode.Combat <= 1;
		}

		// Token: 0x06003B99 RID: 15257 RVA: 0x00068519 File Offset: 0x00066719
		public static bool CanAttemptPursueAbility(this NpcMovementMode mode)
		{
			return mode == NpcMovementMode.Pursue;
		}
	}
}
