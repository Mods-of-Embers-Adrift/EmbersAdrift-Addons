using System;
using System.Collections;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Quests.Actions
{
	// Token: 0x020007C5 RID: 1989
	[CreateAssetMenu(menuName = "SoL/Quests/Actions/RewardAction")]
	public class RewardAction : QuestAction
	{
		// Token: 0x06003A3D RID: 14909 RVA: 0x00067709 File Offset: 0x00065909
		public override void Execute(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			base.Execute(cache, sourceEntity);
			bool isServer = GameManager.IsServer;
		}

		// Token: 0x06003A3E RID: 14910 RVA: 0x00049FFA File Offset: 0x000481FA
		private IEnumerable GetRewards()
		{
			return null;
		}

		// Token: 0x040038A7 RID: 14503
		[SerializeField]
		private Reward m_reward;
	}
}
