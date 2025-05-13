using System;
using System.Collections.Generic;
using SoL.Game.EffectSystem;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Managers;
using SoL.Networking.Replication;
using UnityEngine;

namespace SoL.Game.Quests.Objectives
{
	// Token: 0x020007B8 RID: 1976
	[CreateAssetMenu(menuName = "SoL/Quests/Objectives/StatusEffectObjective")]
	public class StatusEffectObjective : QuestObjective
	{
		// Token: 0x17000D57 RID: 3415
		// (get) Token: 0x06003A14 RID: 14868 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool CanBeActive
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000D58 RID: 3416
		// (get) Token: 0x06003A15 RID: 14869 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool CanBePassive
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003A16 RID: 14870 RVA: 0x000675D5 File Offset: 0x000657D5
		public override bool Validate(GameEntity sourceEntity, ObjectiveIterationCache cache, out string message)
		{
			if (cache.IterationsRequested > 1)
			{
				message = "Too many iterations requested.";
				return false;
			}
			return this.MeetsRequirements(sourceEntity, out message);
		}

		// Token: 0x06003A17 RID: 14871 RVA: 0x000675F1 File Offset: 0x000657F1
		public override void OnEntityInitializedWhenActive(GameEntity sourceEntity, UniqueId questOrTaskId)
		{
			base.OnEntityInitializedWhenActive(sourceEntity, questOrTaskId);
			if (GameManager.IsServer)
			{
				return;
			}
			this.Subscribe(sourceEntity);
			StatusEffectObjective.m_orders.Add(new ValueTuple<UniqueId, StatusEffectObjective>(questOrTaskId, this));
		}

		// Token: 0x06003A18 RID: 14872 RVA: 0x00175690 File Offset: 0x00173890
		public override void OnEntityDestroyedWhenActive(GameEntity sourceEntity, UniqueId questOrTaskId)
		{
			base.OnEntityDestroyedWhenActive(sourceEntity, questOrTaskId);
			if (GameManager.IsServer)
			{
				return;
			}
			this.Unsubscribe(sourceEntity);
			StatusEffectObjective.m_orders.RemoveAll((ValueTuple<UniqueId, StatusEffectObjective> x) => x.Item1 == questOrTaskId && x.Item2 == this);
		}

		// Token: 0x06003A19 RID: 14873 RVA: 0x0006761B File Offset: 0x0006581B
		public override void OnActivate(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			base.OnActivate(cache, sourceEntity);
			if (GameManager.IsServer)
			{
				return;
			}
			this.Subscribe(sourceEntity);
			StatusEffectObjective.m_orders.Add(new ValueTuple<UniqueId, StatusEffectObjective>(cache.QuestId, this));
		}

		// Token: 0x06003A1A RID: 14874 RVA: 0x001756E4 File Offset: 0x001738E4
		public override void OnDeactivate(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			base.OnDeactivate(cache, sourceEntity);
			if (GameManager.IsServer)
			{
				return;
			}
			this.Unsubscribe(sourceEntity);
			StatusEffectObjective.m_orders.RemoveAll((ValueTuple<UniqueId, StatusEffectObjective> x) => x.Item1 == cache.QuestId && x.Item2 == this);
		}

		// Token: 0x06003A1B RID: 14875 RVA: 0x0006764A File Offset: 0x0006584A
		private void Subscribe(GameEntity entity)
		{
			entity.VitalsReplicator.Effects.Changed += this.OnEffectsChanged;
		}

		// Token: 0x06003A1C RID: 14876 RVA: 0x00067668 File Offset: 0x00065868
		private void Unsubscribe(GameEntity entity)
		{
			entity.VitalsReplicator.Effects.Changed -= this.OnEffectsChanged;
		}

		// Token: 0x06003A1D RID: 14877 RVA: 0x00175738 File Offset: 0x00173938
		private void OnEffectsChanged(SynchronizedCollection<UniqueId, EffectSyncData>.Operation op, UniqueId id, EffectSyncData previous, EffectSyncData current)
		{
			if (previous.ArchetypeId.IsEmpty && current.ArchetypeId == this.m_effectSource.Id)
			{
				foreach (ValueTuple<UniqueId, StatusEffectObjective> valueTuple in StatusEffectObjective.m_orders)
				{
					if (valueTuple.Item2 == this)
					{
						this.TryAdvance(valueTuple.Item1, LocalPlayer.GameEntity);
					}
				}
			}
		}

		// Token: 0x06003A1E RID: 14878 RVA: 0x001757CC File Offset: 0x001739CC
		private bool TryAdvance(UniqueId questId, GameEntity entity)
		{
			string text;
			Quest quest;
			int hash;
			if (this.MeetsRequirements(entity, out text) && InternalGameDatabase.Quests.TryGetItem(questId, out quest) && quest.TryGetObjectiveHashForActiveObjective(base.Id, out hash))
			{
				GameManager.QuestManager.Progress(new ObjectiveIterationCache
				{
					QuestId = questId,
					ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(hash)
				}, null, false);
				return true;
			}
			BBTask bbtask;
			if (this.MeetsRequirements(entity, out text) && InternalGameDatabase.BBTasks.TryGetItem(questId, out bbtask))
			{
				GameManager.QuestManager.ProgressTask(new ObjectiveIterationCache
				{
					QuestId = questId,
					ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(base.CombinedId(questId))
				}, null, false);
				return true;
			}
			return false;
		}

		// Token: 0x06003A1F RID: 14879 RVA: 0x0017587C File Offset: 0x00173A7C
		private bool MeetsRequirements(GameEntity sourceEntity, out string message)
		{
			message = string.Empty;
			if (!sourceEntity || sourceEntity.VitalsReplicator == null || sourceEntity.VitalsReplicator.Effects == null)
			{
				message = "Game not fully initialized.";
				return false;
			}
			using (IEnumerator<EffectSyncData> enumerator = sourceEntity.VitalsReplicator.Effects.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ArchetypeId == this.m_effectSource.Id)
					{
						return true;
					}
				}
			}
			message = "No matching effect found.";
			return false;
		}

		// Token: 0x04003892 RID: 14482
		[SerializeField]
		private BaseArchetype m_effectSource;

		// Token: 0x04003893 RID: 14483
		private static List<ValueTuple<UniqueId, StatusEffectObjective>> m_orders = new List<ValueTuple<UniqueId, StatusEffectObjective>>();
	}
}
