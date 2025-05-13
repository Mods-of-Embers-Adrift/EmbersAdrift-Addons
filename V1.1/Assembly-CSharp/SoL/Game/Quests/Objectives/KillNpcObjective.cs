using System;
using System.Collections.Generic;
using SoL.Game.Interactives;
using SoL.Managers;
using SoL.Networking.Objects;
using UnityEngine;

namespace SoL.Game.Quests.Objectives
{
	// Token: 0x020007A4 RID: 1956
	[CreateAssetMenu(menuName = "SoL/Quests/Objectives/KillNpcObjective")]
	public class KillNpcObjective : OrderDrivenObjective<KillNpcObjective>
	{
		// Token: 0x060039A9 RID: 14761 RVA: 0x000670CF File Offset: 0x000652CF
		public bool Prevalidate(InteractiveNpc interactive, GameEntity attackingEntity)
		{
			if (!GameManager.IsServer)
			{
				return false;
			}
			if (interactive != null)
			{
				bool flag = this.m_selection.IsValid(interactive, attackingEntity);
				if (flag)
				{
					this.m_prevalidatedEntities.Add(interactive.GameEntity.NetworkEntity);
				}
				return flag;
			}
			return false;
		}

		// Token: 0x060039AA RID: 14762 RVA: 0x0006710C File Offset: 0x0006530C
		public override bool Validate(GameEntity sourceEntity, ObjectiveIterationCache cache, out string message)
		{
			if (cache.IterationsRequested > 1)
			{
				message = "Too many iterations requested.";
				return false;
			}
			message = string.Empty;
			return GameManager.IsServer && this.m_prevalidatedEntities.Remove(cache.NpcEntity);
		}

		// Token: 0x060039AB RID: 14763 RVA: 0x00045BCA File Offset: 0x00043DCA
		public bool Migrate()
		{
			return false;
		}

		// Token: 0x0400384B RID: 14411
		private const string kGroupNpcSelection = "NPC Selection";

		// Token: 0x0400384C RID: 14412
		[SerializeField]
		private NpcSelection m_selection;

		// Token: 0x0400384D RID: 14413
		private HashSet<NetworkEntity> m_prevalidatedEntities = new HashSet<NetworkEntity>();
	}
}
