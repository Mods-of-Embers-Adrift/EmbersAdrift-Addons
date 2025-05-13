using System;
using System.Collections.Generic;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Quests.Objectives;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Quests
{
	// Token: 0x0200077A RID: 1914
	[CreateAssetMenu(menuName = "SoL/BulletinBoards/BBTask")]
	public class BBTask : Identifiable
	{
		// Token: 0x17000CE0 RID: 3296
		// (get) Token: 0x0600386B RID: 14443 RVA: 0x0006663C File Offset: 0x0006483C
		public string Title
		{
			get
			{
				return this.m_title;
			}
		}

		// Token: 0x17000CE1 RID: 3297
		// (get) Token: 0x0600386C RID: 14444 RVA: 0x00066644 File Offset: 0x00064844
		public string Description
		{
			get
			{
				return this.m_description;
			}
		}

		// Token: 0x17000CE2 RID: 3298
		// (get) Token: 0x0600386D RID: 14445 RVA: 0x0006664C File Offset: 0x0006484C
		public Vector2Int LevelRange
		{
			get
			{
				return this.m_levelRange;
			}
		}

		// Token: 0x17000CE3 RID: 3299
		// (get) Token: 0x0600386E RID: 14446 RVA: 0x00066654 File Offset: 0x00064854
		public BulletinBoard BulletinBoard
		{
			get
			{
				return this.m_bulletinBoard;
			}
		}

		// Token: 0x17000CE4 RID: 3300
		// (get) Token: 0x0600386F RID: 14447 RVA: 0x0006665C File Offset: 0x0006485C
		public BBTaskType Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x17000CE5 RID: 3301
		// (get) Token: 0x06003870 RID: 14448 RVA: 0x00066664 File Offset: 0x00064864
		public BBTaskLabel Label
		{
			get
			{
				return this.m_label;
			}
		}

		// Token: 0x17000CE6 RID: 3302
		// (get) Token: 0x06003871 RID: 14449 RVA: 0x0006666C File Offset: 0x0006486C
		public BaseArchetype AssociatedMastery
		{
			get
			{
				return this.m_associatedMastery;
			}
		}

		// Token: 0x17000CE7 RID: 3303
		// (get) Token: 0x06003872 RID: 14450 RVA: 0x00066674 File Offset: 0x00064874
		public bool Enabled
		{
			get
			{
				return this.m_enabled.HasBitFlag(DeploymentBranchFlagsExtensions.GetBranchFlags()) && !this.m_gmOnly;
			}
		}

		// Token: 0x17000CE8 RID: 3304
		// (get) Token: 0x06003873 RID: 14451 RVA: 0x00066693 File Offset: 0x00064893
		public RoleLevelRequirement Requirements
		{
			get
			{
				return this.m_requirements;
			}
		}

		// Token: 0x17000CE9 RID: 3305
		// (get) Token: 0x06003874 RID: 14452 RVA: 0x0006669B File Offset: 0x0006489B
		public ZoneId[] ValidZones
		{
			get
			{
				return this.m_validZones;
			}
		}

		// Token: 0x17000CEA RID: 3306
		// (get) Token: 0x06003875 RID: 14453 RVA: 0x000666A3 File Offset: 0x000648A3
		public bool HideCompletedObjectives
		{
			get
			{
				return this.m_hideCompletedObjectives;
			}
		}

		// Token: 0x17000CEB RID: 3307
		// (get) Token: 0x06003876 RID: 14454 RVA: 0x000666AB File Offset: 0x000648AB
		public bool OverrideObjectiveText
		{
			get
			{
				return this.m_overrideObjectiveText;
			}
		}

		// Token: 0x17000CEC RID: 3308
		// (get) Token: 0x06003877 RID: 14455 RVA: 0x000666B3 File Offset: 0x000648B3
		public string ObjectiveOverrideText
		{
			get
			{
				return this.m_objectiveOverrideText;
			}
		}

		// Token: 0x17000CED RID: 3309
		// (get) Token: 0x06003878 RID: 14456 RVA: 0x000666BB File Offset: 0x000648BB
		public List<QuestObjective> Objectives
		{
			get
			{
				return this.m_objectives;
			}
		}

		// Token: 0x06003879 RID: 14457 RVA: 0x000666C3 File Offset: 0x000648C3
		public bool CanStart(GameEntity entity = null)
		{
			if (!this.Enabled)
			{
				return false;
			}
			if (!GameManager.IsServer)
			{
				entity = LocalPlayer.GameEntity;
			}
			RoleLevelRequirement requirements = this.m_requirements;
			return (requirements == null || requirements.MeetsAllRequirements(entity)) && !this.HasConflictingTask(entity);
		}

		// Token: 0x0600387A RID: 14458 RVA: 0x0016DA34 File Offset: 0x0016BC34
		public bool IsOnTask(GameEntity entity = null)
		{
			if (!this.Enabled)
			{
				return false;
			}
			if (!GameManager.IsServer)
			{
				entity = LocalPlayer.GameEntity;
			}
			return entity && entity.CollectionController != null && entity.CollectionController.Record != null && entity.CollectionController.Record.Progression != null && entity.CollectionController.Record.Progression.BBTasks != null && entity.CollectionController.Record.Progression.BBTasks.ContainsKey(base.Id);
		}

		// Token: 0x0600387B RID: 14459 RVA: 0x0016DAC4 File Offset: 0x0016BCC4
		public bool HasConflictingTask(GameEntity entity = null)
		{
			if (!GameManager.IsServer)
			{
				entity = LocalPlayer.GameEntity;
			}
			if (entity && entity.CollectionController != null && entity.CollectionController.Record != null && entity.CollectionController.Record.Progression != null && entity.CollectionController.Record.Progression.BBTasks != null)
			{
				foreach (UniqueId id in entity.CollectionController.Record.Progression.BBTasks.Keys)
				{
					BBTask bbtask;
					if (InternalGameDatabase.BBTasks.TryGetItem(id, out bbtask) && bbtask.Enabled && bbtask.BulletinBoard.Id == this.BulletinBoard.Id && bbtask.Type == this.Type)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x0600387C RID: 14460 RVA: 0x0016DBD0 File Offset: 0x0016BDD0
		public bool TryGetObjective(UniqueId objectiveId, out QuestObjective objective)
		{
			objective = null;
			foreach (QuestObjective questObjective in this.m_objectives)
			{
				if (questObjective.Id == objectiveId)
				{
					objective = questObjective;
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600387D RID: 14461 RVA: 0x0016DC38 File Offset: 0x0016BE38
		public bool TryGetObjective(int hash, out QuestObjective objective)
		{
			objective = null;
			foreach (QuestObjective questObjective in this.m_objectives)
			{
				if (questObjective.CombinedId(base.Id) == hash)
				{
					objective = questObjective;
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600387E RID: 14462 RVA: 0x000666FE File Offset: 0x000648FE
		public bool TryGetProgress(out BBTaskProgressionData progress)
		{
			if (GameManager.IsServer)
			{
				throw new InvalidOperationException();
			}
			return this.TryGetProgress(LocalPlayer.GameEntity, out progress);
		}

		// Token: 0x0600387F RID: 14463 RVA: 0x0016DCA0 File Offset: 0x0016BEA0
		public bool TryGetProgress(GameEntity entity, out BBTaskProgressionData progress)
		{
			if (!GameManager.IsServer)
			{
				entity = LocalPlayer.GameEntity;
			}
			progress = null;
			return entity && entity.CollectionController != null && entity.CollectionController.Record != null && entity.CollectionController.Record.Progression != null && entity.CollectionController.Record.Progression.BBTasks != null && entity.CollectionController.Record.Progression.BBTasks.TryGetValue(base.Id, out progress);
		}

		// Token: 0x06003880 RID: 14464 RVA: 0x0016DD28 File Offset: 0x0016BF28
		public bool IsReadyForTurnIn(GameEntity entity = null)
		{
			if (!GameManager.IsServer)
			{
				entity = LocalPlayer.GameEntity;
			}
			if (entity == null)
			{
				return false;
			}
			bool result = false;
			BBTaskProgressionData bbtaskProgressionData;
			if (this.TryGetProgress(entity, out bbtaskProgressionData))
			{
				result = true;
				foreach (QuestObjective questObjective in this.m_objectives)
				{
					ObjectiveProgressionData objectiveProgressionData;
					if (bbtaskProgressionData.TryGetObjective(questObjective.Id, out objectiveProgressionData))
					{
						if (objectiveProgressionData.IterationsCompleted < questObjective.IterationsRequired)
						{
							result = false;
						}
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		// Token: 0x06003881 RID: 14465 RVA: 0x0016DDC4 File Offset: 0x0016BFC4
		public void OnEntityInitialized(GameEntity sourceEntity)
		{
			if (this.m_objectives != null)
			{
				foreach (QuestObjective questObjective in this.m_objectives)
				{
					questObjective.OnEntityInitializedWhenActive(sourceEntity, base.Id);
				}
			}
		}

		// Token: 0x06003882 RID: 14466 RVA: 0x0016DE24 File Offset: 0x0016C024
		public void OnEntityDestroyed(GameEntity sourceEntity)
		{
			if (this.m_objectives != null)
			{
				foreach (QuestObjective questObjective in this.m_objectives)
				{
					questObjective.OnEntityDestroyedWhenActive(sourceEntity, base.Id);
				}
			}
		}

		// Token: 0x06003883 RID: 14467 RVA: 0x0016DE84 File Offset: 0x0016C084
		public void OnEntry(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			if (this.m_objectives != null)
			{
				foreach (QuestObjective questObjective in this.m_objectives)
				{
					questObjective.OnActivate(cache, sourceEntity);
				}
			}
		}

		// Token: 0x06003884 RID: 14468 RVA: 0x0016DEE0 File Offset: 0x0016C0E0
		public void OnExit(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			if (this.m_objectives != null)
			{
				foreach (QuestObjective questObjective in this.m_objectives)
				{
					questObjective.OnEnterStep(cache, sourceEntity);
					questObjective.OnDeactivate(cache, sourceEntity);
				}
			}
		}

		// Token: 0x06003885 RID: 14469 RVA: 0x00066719 File Offset: 0x00064919
		private bool ValidateMastery(BaseArchetype archetype, ref string errorMessage)
		{
			if (archetype != null && !(archetype is MasteryArchetype) && !(archetype is SpecializedRole))
			{
				errorMessage = "Associated Mastery must be a valid MasteryArchetype or SpecializedRole!";
				return false;
			}
			return true;
		}

		// Token: 0x04003746 RID: 14150
		[SerializeField]
		private string m_title;

		// Token: 0x04003747 RID: 14151
		[TextArea]
		[SerializeField]
		private string m_description;

		// Token: 0x04003748 RID: 14152
		[SerializeField]
		private Vector2Int m_levelRange = new Vector2Int(1, 50);

		// Token: 0x04003749 RID: 14153
		[SerializeField]
		private BulletinBoard m_bulletinBoard;

		// Token: 0x0400374A RID: 14154
		[SerializeField]
		private BBTaskType m_type;

		// Token: 0x0400374B RID: 14155
		[SerializeField]
		private BBTaskLabel m_label;

		// Token: 0x0400374C RID: 14156
		[SerializeField]
		private BaseArchetype m_associatedMastery;

		// Token: 0x0400374D RID: 14157
		[SerializeField]
		private DeploymentBranchFlags m_enabled = DeploymentBranchFlags.DEV | DeploymentBranchFlags.QA | DeploymentBranchFlags.LIVE;

		// Token: 0x0400374E RID: 14158
		[SerializeField]
		private bool m_gmOnly;

		// Token: 0x0400374F RID: 14159
		[SerializeField]
		private RoleLevelRequirement m_requirements;

		// Token: 0x04003750 RID: 14160
		[SerializeField]
		private ZoneId[] m_validZones;

		// Token: 0x04003751 RID: 14161
		[SerializeField]
		private bool m_hideCompletedObjectives;

		// Token: 0x04003752 RID: 14162
		[SerializeField]
		private bool m_overrideObjectiveText;

		// Token: 0x04003753 RID: 14163
		[SerializeField]
		private string m_objectiveOverrideText;

		// Token: 0x04003754 RID: 14164
		[SerializeField]
		private List<QuestObjective> m_objectives;
	}
}
