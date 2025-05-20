using System;
using SoL.Managers;
using SoL.Networking.Objects;
using UnityEngine;

namespace SoL.Game.Quests.Objectives
{
	// Token: 0x0200079D RID: 1949
	[CreateAssetMenu(menuName = "SoL/Quests/Objectives/DialogueNpcObjective")]
	public class DialogueNpcObjective : QuestObjective
	{
		// Token: 0x06003989 RID: 14729 RVA: 0x0017349C File Offset: 0x0017169C
		public override bool Validate(GameEntity sourceEntity, ObjectiveIterationCache cache, out string message)
		{
			message = string.Empty;
			if (cache.IterationsRequested > 1)
			{
				message = "Too many iterations requested.";
				return false;
			}
			if (cache.NpcEntity != null)
			{
				NetworkEntity npcEntity = cache.NpcEntity;
				IDialogueNpc dialogueNpc = (npcEntity != null) ? npcEntity.GetComponent<IDialogueNpc>() : null;
				if (dialogueNpc != null)
				{
					if (!dialogueNpc.CanConverseWith(sourceEntity))
					{
						message = "Objective validation failed. Move closer to the NPC and try again. If this continues, please report this issue.";
						return false;
					}
					return true;
				}
			}
			IWorldObject worldObject;
			if (cache.WorldId.IsEmpty || !LocalZoneManager.TryGetWorldObject(cache.WorldId, out worldObject))
			{
				message = "Unable to find validation criteria for objective!";
				return false;
			}
			if (!worldObject.Validate(sourceEntity))
			{
				message = "Objective validation failed. Move closer to the object and try again. If this continues, please report this issue.";
				return false;
			}
			return true;
		}
	}
}
