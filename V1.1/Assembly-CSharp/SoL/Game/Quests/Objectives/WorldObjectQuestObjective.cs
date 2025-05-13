using System;
using System.Collections.Generic;
using SoL.Managers;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Quests.Objectives
{
	// Token: 0x020007BC RID: 1980
	[CreateAssetMenu(menuName = "SoL/Quests/Objectives/WorldObjectQuestObjective")]
	public class WorldObjectQuestObjective : QuestObjective
	{
		// Token: 0x06003A2A RID: 14890 RVA: 0x000676E7 File Offset: 0x000658E7
		public static void RegisterWorldObjectForPassiveObjective(int objectiveHash, UniqueId worldObjectId)
		{
			WorldObjectQuestObjective.m_worldObjectRegistry.AddOrReplace(objectiveHash, worldObjectId);
		}

		// Token: 0x17000D5B RID: 3419
		// (get) Token: 0x06003A2B RID: 14891 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool CanBePassive
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003A2C RID: 14892 RVA: 0x00175A78 File Offset: 0x00173C78
		public override bool Validate(GameEntity sourceEntity, ObjectiveIterationCache cache, out string message)
		{
			if (cache.IterationsRequested > 1)
			{
				message = "Too many iterations requested.";
				return false;
			}
			message = string.Empty;
			IWorldObject worldObject2;
			if (base.Passive)
			{
				UniqueId id;
				IWorldObject worldObject;
				if (WorldObjectQuestObjective.m_worldObjectRegistry.TryGetValue(base.CombinedId(cache.QuestId), out id) && LocalZoneManager.TryGetWorldObject(id, out worldObject))
				{
					return worldObject.Validate(sourceEntity);
				}
			}
			else if (LocalZoneManager.TryGetWorldObject(cache.WorldId, out worldObject2))
			{
				return worldObject2.Validate(sourceEntity);
			}
			return false;
		}

		// Token: 0x0400389D RID: 14493
		private static Dictionary<int, UniqueId> m_worldObjectRegistry = new Dictionary<int, UniqueId>();
	}
}
