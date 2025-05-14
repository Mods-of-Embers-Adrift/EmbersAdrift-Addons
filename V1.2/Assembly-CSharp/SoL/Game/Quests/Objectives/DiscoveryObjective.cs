using System;
using System.Collections.Generic;
using SoL.Game.Discovery;
using SoL.Game.Objects;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Quests.Objectives
{
	// Token: 0x0200079F RID: 1951
	[CreateAssetMenu(menuName = "SoL/Quests/Objectives/DiscoveryObjective")]
	public class DiscoveryObjective : OrderDrivenObjective<DiscoveryObjective>
	{
		// Token: 0x17000D3A RID: 3386
		// (get) Token: 0x0600398B RID: 14731 RVA: 0x00066F48 File Offset: 0x00065148
		public List<DiscoveryProfile> Discoveries
		{
			get
			{
				return this.m_discoveries;
			}
		}

		// Token: 0x0600398C RID: 14732 RVA: 0x00066F50 File Offset: 0x00065150
		public override bool Validate(GameEntity sourceEntity, ObjectiveIterationCache cache, out string message)
		{
			if (cache.IterationsRequested > 1)
			{
				message = "Too many iterations requested.";
				return false;
			}
			message = string.Empty;
			if (!this.HasDiscovered(sourceEntity))
			{
				message = "Not all discoveries have been found!";
				return false;
			}
			return true;
		}

		// Token: 0x0600398D RID: 14733 RVA: 0x00173554 File Offset: 0x00171754
		private bool HasDiscovered(GameEntity entity)
		{
			if (((entity != null) ? entity.CollectionController.Record.Discoveries : null) == null)
			{
				return false;
			}
			List<UniqueId> fromPool = StaticListPool<UniqueId>.GetFromPool();
			foreach (List<UniqueId> collection in entity.CollectionController.Record.Discoveries.Values)
			{
				fromPool.AddRange(collection);
			}
			foreach (DiscoveryProfile discoveryProfile in this.m_discoveries)
			{
				if (!fromPool.Contains(discoveryProfile.Id))
				{
					StaticListPool<UniqueId>.ReturnToPool(fromPool);
					return false;
				}
				if (this.m_criteria == DiscoveryCriteria.Any)
				{
					StaticListPool<UniqueId>.ReturnToPool(fromPool);
					return true;
				}
			}
			StaticListPool<UniqueId>.ReturnToPool(fromPool);
			return true;
		}

		// Token: 0x0600398E RID: 14734 RVA: 0x0017364C File Offset: 0x0017184C
		public bool TryAdvance(UniqueId questId, GameEntity entity)
		{
			Quest quest;
			int hash;
			if (this.HasDiscovered(entity) && InternalGameDatabase.Quests.TryGetItem(questId, out quest) && quest.TryGetObjectiveHashForActiveObjective(base.Id, entity, out hash))
			{
				GameManager.QuestManager.Progress(new ObjectiveIterationCache
				{
					QuestId = questId,
					ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(hash)
				}, entity, false);
				return true;
			}
			BBTask bbtask;
			if (this.HasDiscovered(entity) && InternalGameDatabase.BBTasks.TryGetItem(questId, out bbtask))
			{
				GameManager.QuestManager.ProgressTask(new ObjectiveIterationCache
				{
					QuestId = questId,
					ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(base.CombinedId(questId))
				}, entity, false);
				return true;
			}
			return false;
		}

		// Token: 0x0600398F RID: 14735 RVA: 0x001736FC File Offset: 0x001718FC
		public List<DiscoveryProfile> ListUndiscovered(GameEntity entity)
		{
			this.m_tempUndiscovered.Clear();
			List<UniqueId> fromPool = StaticListPool<UniqueId>.GetFromPool();
			if (((entity != null) ? entity.CollectionController.Record.Discoveries : null) != null)
			{
				foreach (List<UniqueId> collection in entity.CollectionController.Record.Discoveries.Values)
				{
					fromPool.AddRange(collection);
				}
			}
			foreach (DiscoveryProfile discoveryProfile in this.m_discoveries)
			{
				if (!fromPool.Contains(discoveryProfile.Id))
				{
					this.m_tempUndiscovered.Add(discoveryProfile);
				}
			}
			StaticListPool<UniqueId>.ReturnToPool(fromPool);
			return this.m_tempUndiscovered;
		}

		// Token: 0x04003840 RID: 14400
		[SerializeField]
		private DiscoveryCriteria m_criteria;

		// Token: 0x04003841 RID: 14401
		[SerializeField]
		private List<DiscoveryProfile> m_discoveries;

		// Token: 0x04003842 RID: 14402
		private List<DiscoveryProfile> m_tempUndiscovered = new List<DiscoveryProfile>();
	}
}
