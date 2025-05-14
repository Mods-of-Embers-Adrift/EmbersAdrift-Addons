using System;
using UnityEngine;

namespace SoL.Game.Quests.Actions
{
	// Token: 0x020007C3 RID: 1987
	[CreateAssetMenu(menuName = "SoL/Quests/Actions/QuestResetAction")]
	public class QuestResetAction : QuestAction
	{
		// Token: 0x06003A39 RID: 14905 RVA: 0x0004475B File Offset: 0x0004295B
		public override void Execute(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
		}

		// Token: 0x040038A6 RID: 14502
		[SerializeField]
		private Quest[] m_questsToReset;
	}
}
