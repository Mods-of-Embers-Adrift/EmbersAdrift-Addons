using System;
using SoL.Game.Quests.Objectives;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Quests
{
	// Token: 0x02000797 RID: 1943
	public class QuestVolumeTrigger : WorldObject
	{
		// Token: 0x17000D27 RID: 3367
		// (get) Token: 0x06003959 RID: 14681 RVA: 0x00066D03 File Offset: 0x00064F03
		public bool TriggerIfQuestNotPresent
		{
			get
			{
				return this.m_triggerIfQuestNotPresent;
			}
		}

		// Token: 0x0600395A RID: 14682 RVA: 0x00172B94 File Offset: 0x00170D94
		protected override void Awake()
		{
			if (this.m_collider == null || (this.m_quest == null && this.m_task == null) || this.m_objective == null)
			{
				Debug.LogWarning("Invalid QuestVolumeTrigger " + base.gameObject.name + "!");
				base.gameObject.SetActive(false);
				return;
			}
			base.Awake();
			this.m_colliderBounds = this.m_collider.bounds;
			if (GameManager.IsServer)
			{
				this.m_collider.enabled = false;
			}
			else
			{
				this.m_collider.isTrigger = !this.m_objective.Passive;
			}
			if (this.m_objective.Passive)
			{
				int objectiveHash = 0;
				if (this.m_quest != null)
				{
					if (!this.m_quest.TryGetObjectiveHashForActiveObjective(this.m_objective.Id, out objectiveHash) && this.m_quest.Start.Objectives.Contains(this.m_objective))
					{
						objectiveHash = this.m_objective.CombinedId(this.m_quest.Start.Id);
					}
				}
				else if (this.m_task != null)
				{
					objectiveHash = this.m_objective.CombinedId(this.m_task.Id);
				}
				WorldObjectQuestObjective.RegisterWorldObjectForPassiveObjective(objectiveHash, base.WorldId);
			}
			this.m_collider.gameObject.layer = LayerMap.Detection.Layer;
		}

		// Token: 0x0600395B RID: 14683 RVA: 0x00172D10 File Offset: 0x00170F10
		private void OnTriggerEnter(Collider other)
		{
			if (GameManager.IsServer)
			{
				return;
			}
			GameEntity gameEntity;
			int num;
			if (this.m_quest && DetectionCollider.TryGetEntityForCollider(other, out gameEntity) && gameEntity.Type == GameEntityType.Player && gameEntity == LocalPlayer.GameEntity && (this.m_quest.TryGetObjectiveHashForActiveObjective(this.m_objective.Id, out num) || this.m_triggerIfQuestNotPresent))
			{
				if (num == 0 && this.m_quest.Start.Objectives.Contains(this.m_objective))
				{
					num = this.m_objective.CombinedId(this.m_quest.Start.Id);
				}
				QuestManager questManager = GameManager.QuestManager;
				ObjectiveIterationCache cache = new ObjectiveIterationCache
				{
					QuestId = this.m_quest.Id,
					ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray((num != 0) ? num : 0),
					WorldId = base.WorldId,
					StartQuestIfNotPresent = this.m_triggerIfQuestNotPresent
				};
				questManager.Progress(cache, null, false);
			}
			if (this.m_task && DetectionCollider.TryGetEntityForCollider(other, out gameEntity) && gameEntity.Type == GameEntityType.Player && gameEntity == LocalPlayer.GameEntity)
			{
				QuestManager questManager2 = GameManager.QuestManager;
				ObjectiveIterationCache cache = new ObjectiveIterationCache
				{
					QuestId = this.m_task.Id,
					ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(this.m_objective.CombinedId(this.m_task.Id)),
					WorldId = base.WorldId
				};
				questManager2.ProgressTask(cache, null, false);
			}
		}

		// Token: 0x0600395C RID: 14684 RVA: 0x00172E98 File Offset: 0x00171098
		protected override bool Validate(GameEntity entity)
		{
			Vector3 position = entity.gameObject.transform.position;
			Bounds colliderBounds = this.m_colliderBounds;
			if (this.m_objective.Passive)
			{
				colliderBounds.extents *= 1.2f;
				return colliderBounds.Contains(position);
			}
			if (colliderBounds.Contains(position))
			{
				return true;
			}
			colliderBounds.extents *= 2f;
			return colliderBounds.Contains(position);
		}

		// Token: 0x04003813 RID: 14355
		[SerializeField]
		private Collider m_collider;

		// Token: 0x04003814 RID: 14356
		[SerializeField]
		private bool m_triggerIfQuestNotPresent;

		// Token: 0x04003815 RID: 14357
		[SerializeField]
		private Quest m_quest;

		// Token: 0x04003816 RID: 14358
		[SerializeField]
		private BBTask m_task;

		// Token: 0x04003817 RID: 14359
		[SerializeField]
		private WorldObjectQuestObjective m_objective;

		// Token: 0x04003818 RID: 14360
		private Bounds m_colliderBounds;
	}
}
