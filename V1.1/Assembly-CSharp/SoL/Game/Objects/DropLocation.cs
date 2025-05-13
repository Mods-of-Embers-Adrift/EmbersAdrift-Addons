using System;
using System.Collections.Generic;
using SoL.Game.Quests;
using SoL.Game.Quests.Objectives;
using UnityEngine;

namespace SoL.Game.Objects
{
	// Token: 0x020009F1 RID: 2545
	[CreateAssetMenu(menuName = "SoL/Profiles/Drop Location")]
	public class DropLocation : ScriptableObject
	{
		// Token: 0x06004D71 RID: 19825 RVA: 0x001C04F0 File Offset: 0x001BE6F0
		public bool HasActiveIncompleteObjective(GameEntity entity)
		{
			if (entity && entity.CollectionController != null && entity.CollectionController.Record != null && entity.CollectionController.Record.Progression != null)
			{
				if (entity.CollectionController.Record.Progression.Quests != null)
				{
					foreach (KeyValuePair<UniqueId, QuestProgressionData> keyValuePair in entity.CollectionController.Record.Progression.Quests)
					{
						Quest quest;
						QuestStep questStep;
						if (InternalGameDatabase.Quests.TryGetItem(keyValuePair.Key, out quest) && quest.TryGetStep(keyValuePair.Value.CurrentNodeId, out questStep))
						{
							foreach (QuestStep questStep2 in questStep.NextSteps)
							{
								foreach (QuestObjective questObjective in questStep2.Objectives)
								{
									LootObjective lootObjective = questObjective as LootObjective;
									ObjectiveProgressionData objectiveProgressionData;
									if (lootObjective != null && lootObjective.DropLocations.Contains(this) && (!keyValuePair.Value.TryGetObjective(questObjective.Id, out objectiveProgressionData) || objectiveProgressionData.IterationsCompleted < questObjective.IterationsRequired))
									{
										return true;
									}
								}
							}
						}
					}
				}
				if (entity.CollectionController.Record.Progression.BBTasks != null)
				{
					foreach (KeyValuePair<UniqueId, BBTaskProgressionData> keyValuePair2 in entity.CollectionController.Record.Progression.BBTasks)
					{
						BBTask bbtask;
						if (InternalGameDatabase.BBTasks.TryGetItem(keyValuePair2.Key, out bbtask))
						{
							foreach (QuestObjective questObjective2 in bbtask.Objectives)
							{
								LootObjective lootObjective2 = questObjective2 as LootObjective;
								ObjectiveProgressionData objectiveProgressionData2;
								if (lootObjective2 != null && lootObjective2.DropLocations.Contains(this) && (!keyValuePair2.Value.TryGetObjective(questObjective2.Id, out objectiveProgressionData2) || objectiveProgressionData2.IterationsCompleted < questObjective2.IterationsRequired))
								{
									return true;
								}
							}
						}
					}
					return false;
				}
				return false;
			}
			return false;
		}

		// Token: 0x06004D72 RID: 19826 RVA: 0x001C07AC File Offset: 0x001BE9AC
		public bool TryDrop(GameEntity entity)
		{
			bool flag = false;
			if (entity && entity.CollectionController != null && entity.CollectionController.Record != null && entity.CollectionController.Record.Progression != null)
			{
				if (entity.CollectionController.Record.Progression.Quests != null)
				{
					foreach (KeyValuePair<UniqueId, QuestProgressionData> keyValuePair in entity.CollectionController.Record.Progression.Quests)
					{
						Quest quest;
						QuestStep questStep;
						if (InternalGameDatabase.Quests.TryGetItem(keyValuePair.Key, out quest) && quest.TryGetStep(keyValuePair.Value.CurrentNodeId, out questStep))
						{
							foreach (QuestStep questStep2 in questStep.NextSteps)
							{
								foreach (QuestObjective questObjective in questStep2.Objectives)
								{
									LootObjective lootObjective = questObjective as LootObjective;
									if (lootObjective != null && lootObjective.DropLocations.Contains(this))
									{
										flag = (flag || lootObjective.TryAdvance(keyValuePair.Key, entity));
									}
								}
							}
						}
					}
				}
				if (entity.CollectionController.Record.Progression.BBTasks != null)
				{
					foreach (KeyValuePair<UniqueId, BBTaskProgressionData> keyValuePair2 in entity.CollectionController.Record.Progression.BBTasks)
					{
						BBTask bbtask;
						if (InternalGameDatabase.BBTasks.TryGetItem(keyValuePair2.Key, out bbtask))
						{
							foreach (QuestObjective questObjective2 in bbtask.Objectives)
							{
								LootObjective lootObjective2 = questObjective2 as LootObjective;
								if (lootObjective2 != null && lootObjective2.DropLocations.Contains(this))
								{
									flag = (flag || lootObjective2.TryAdvance(keyValuePair2.Key, entity));
								}
							}
						}
					}
				}
			}
			return flag;
		}
	}
}
