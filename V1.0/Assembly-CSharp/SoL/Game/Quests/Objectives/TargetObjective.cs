using System;
using SoL.Game.Interactives;
using UnityEngine;

namespace SoL.Game.Quests.Objectives
{
	// Token: 0x020007BB RID: 1979
	[CreateAssetMenu(menuName = "SoL/Quests/Objectives/TargetObjective")]
	public class TargetObjective : QuestObjective
	{
		// Token: 0x17000D59 RID: 3417
		// (get) Token: 0x06003A26 RID: 14886 RVA: 0x00045BCA File Offset: 0x00043DCA
		public override bool CanBeActive
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000D5A RID: 3418
		// (get) Token: 0x06003A27 RID: 14887 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool CanBePassive
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003A28 RID: 14888 RVA: 0x00175924 File Offset: 0x00173B24
		public override bool Validate(GameEntity sourceEntity, ObjectiveIterationCache cache, out string message)
		{
			if (cache.IterationsRequested > 1)
			{
				message = "Too many iterations requested.";
				return false;
			}
			message = string.Empty;
			if (!sourceEntity)
			{
				return false;
			}
			InteractiveNpc interactiveNpc = null;
			InteractiveNpc interactiveNpc2 = null;
			InteractiveNpc interactiveNpc3 = null;
			if (sourceEntity.TargetController.OffensiveTarget != null && sourceEntity.TargetController.OffensiveTarget.Targetable.IsNpc)
			{
				sourceEntity.TargetController.OffensiveTarget.Interactive.TryGetAsType(out interactiveNpc);
			}
			if (sourceEntity.TargetController.DefensiveTarget != null && sourceEntity.TargetController.DefensiveTarget.Targetable.IsNpc)
			{
				sourceEntity.TargetController.DefensiveTarget.Interactive.TryGetAsType(out interactiveNpc2);
			}
			if (interactiveNpc == null && interactiveNpc2 == null)
			{
				message = "Nothing targeted.";
				return false;
			}
			if (interactiveNpc != null && this.m_selection.IsValid(interactiveNpc, sourceEntity))
			{
				interactiveNpc3 = interactiveNpc;
			}
			if (interactiveNpc2 != null && this.m_selection.IsValid(interactiveNpc2, sourceEntity))
			{
				interactiveNpc3 = interactiveNpc2;
			}
			if (interactiveNpc3 == null)
			{
				message = "Invalid target.";
				return false;
			}
			if (this.m_requireAlive && !interactiveNpc3.GameEntity.IsAlive)
			{
				message = "Target must be alive.";
				return false;
			}
			if (this.m_limitToInteractionDistance && !interactiveNpc3.IsWithinDistance(sourceEntity))
			{
				message = "Target is too far away.";
				return false;
			}
			return true;
		}

		// Token: 0x04003898 RID: 14488
		private const string kGroupNpcSelection = "NPC Selection";

		// Token: 0x04003899 RID: 14489
		private const string kGroupTargetingRequirements = "Targeting Requirements";

		// Token: 0x0400389A RID: 14490
		[SerializeField]
		private NpcSelection m_selection;

		// Token: 0x0400389B RID: 14491
		[SerializeField]
		private bool m_requireAlive;

		// Token: 0x0400389C RID: 14492
		[SerializeField]
		private bool m_limitToInteractionDistance;
	}
}
