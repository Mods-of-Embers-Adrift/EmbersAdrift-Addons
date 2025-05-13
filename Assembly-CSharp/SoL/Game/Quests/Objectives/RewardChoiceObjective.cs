using System;
using System.Collections;
using System.Collections.Generic;
using SoL.Game.NPCs;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Quests.Objectives
{
	// Token: 0x020007B7 RID: 1975
	[CreateAssetMenu(menuName = "SoL/Quests/Objectives/RewardChoiceObjective")]
	public class RewardChoiceObjective : QuestObjective
	{
		// Token: 0x17000D54 RID: 3412
		// (get) Token: 0x06003A0C RID: 14860 RVA: 0x000675A1 File Offset: 0x000657A1
		public NpcProfile NpcProfile
		{
			get
			{
				return this.m_npcProfile;
			}
		}

		// Token: 0x17000D55 RID: 3413
		// (get) Token: 0x06003A0D RID: 14861 RVA: 0x000675A9 File Offset: 0x000657A9
		public Reward Reward
		{
			get
			{
				return this.m_reward;
			}
		}

		// Token: 0x17000D56 RID: 3414
		// (get) Token: 0x06003A0E RID: 14862 RVA: 0x000675B1 File Offset: 0x000657B1
		public bool WaitForDialogueTag
		{
			get
			{
				return this.m_waitForDialogueTag;
			}
		}

		// Token: 0x06003A0F RID: 14863 RVA: 0x0017563C File Offset: 0x0017383C
		public override bool Validate(GameEntity sourceEntity, ObjectiveIterationCache cache, out string message)
		{
			if (cache.IterationsRequested > 1)
			{
				message = "Too many iterations requested.";
				return false;
			}
			message = string.Empty;
			List<RewardItem> rewards;
			if (this.m_reward.TryGetRewards(sourceEntity, cache.RewardChoiceId, out rewards, false) && rewards.EntityCanAcquire(sourceEntity, out message))
			{
				message = string.Empty;
				return true;
			}
			return false;
		}

		// Token: 0x06003A10 RID: 14864 RVA: 0x000675B9 File Offset: 0x000657B9
		public override void OnEnterStep(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			if (GameManager.IsServer)
			{
				this.m_reward.GrantReward(sourceEntity, cache.RewardChoiceId, false);
			}
		}

		// Token: 0x06003A11 RID: 14865 RVA: 0x00049FFA File Offset: 0x000481FA
		private IEnumerable GetNpcs()
		{
			return null;
		}

		// Token: 0x06003A12 RID: 14866 RVA: 0x00049FFA File Offset: 0x000481FA
		private IEnumerable GetRewards()
		{
			return null;
		}

		// Token: 0x0400388F RID: 14479
		[SerializeField]
		private NpcProfile m_npcProfile;

		// Token: 0x04003890 RID: 14480
		[SerializeField]
		private Reward m_reward;

		// Token: 0x04003891 RID: 14481
		[SerializeField]
		private bool m_waitForDialogueTag;
	}
}
