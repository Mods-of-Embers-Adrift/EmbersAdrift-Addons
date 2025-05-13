using System;
using SoL.Game.EffectSystem;
using SoL.Game.Quests.Objectives;
using SoL.Managers;
using SoL.Networking.Objects;
using UnityEngine;

namespace SoL.Game.Quests
{
	// Token: 0x02000796 RID: 1942
	public class InteractiveQuestTriggerHit : GameEntityComponent
	{
		// Token: 0x06003953 RID: 14675 RVA: 0x00172A04 File Offset: 0x00170C04
		private void Start()
		{
			if (GameManager.IsServer)
			{
				base.GameEntity.TargetController.ThreatReceived += this.TargetControllerOnThreatReceived;
				return;
			}
			if (GameManager.QuestManager != null)
			{
				GameManager.QuestManager.QuestsUpdated += this.UpdateColliders;
			}
			LocalPlayer.LocalPlayerInitialized += this.LocalPlayerInitialized;
		}

		// Token: 0x06003954 RID: 14676 RVA: 0x00172A6C File Offset: 0x00170C6C
		private void OnDestroy()
		{
			if (GameManager.IsServer)
			{
				base.GameEntity.TargetController.ThreatReceived -= this.TargetControllerOnThreatReceived;
				return;
			}
			if (GameManager.QuestManager != null)
			{
				GameManager.QuestManager.QuestsUpdated -= this.UpdateColliders;
			}
		}

		// Token: 0x06003955 RID: 14677 RVA: 0x00066CEA File Offset: 0x00064EEA
		private void LocalPlayerInitialized()
		{
			LocalPlayer.LocalPlayerInitialized -= this.LocalPlayerInitialized;
			this.UpdateColliders();
		}

		// Token: 0x06003956 RID: 14678 RVA: 0x00172AC0 File Offset: 0x00170CC0
		private void UpdateColliders()
		{
			bool enabled = this.m_quest.IsObjectiveActive(this.m_objective.Id, null);
			for (int i = 0; i < this.m_targetColliders.Length; i++)
			{
				if (this.m_targetColliders[i] != null)
				{
					this.m_targetColliders[i].enabled = enabled;
				}
			}
		}

		// Token: 0x06003957 RID: 14679 RVA: 0x00172B18 File Offset: 0x00170D18
		private void TargetControllerOnThreatReceived(NetworkEntity sourceEntity, EffectProcessingResult effectProcessingResult)
		{
			if (!GameManager.IsServer || effectProcessingResult.DamageDone >= 0f)
			{
				return;
			}
			int hash;
			if (this.m_quest.TryGetObjectiveHashForActiveObjective(this.m_objective.Id, sourceEntity.GameEntity, out hash))
			{
				GameManager.QuestManager.Progress(new ObjectiveIterationCache
				{
					QuestId = this.m_quest.Id,
					ObjectiveHashes = ObjectiveIterationCache.SharedSingleItemArray(hash)
				}, sourceEntity.GameEntity, false);
			}
		}

		// Token: 0x04003810 RID: 14352
		[SerializeField]
		protected Quest m_quest;

		// Token: 0x04003811 RID: 14353
		[SerializeField]
		private QuestObjective m_objective;

		// Token: 0x04003812 RID: 14354
		[SerializeField]
		private Collider[] m_targetColliders;
	}
}
