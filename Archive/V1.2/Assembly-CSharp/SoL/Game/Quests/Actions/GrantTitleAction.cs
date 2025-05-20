using System;
using UnityEngine;

namespace SoL.Game.Quests.Actions
{
	// Token: 0x020007C1 RID: 1985
	[CreateAssetMenu(menuName = "SoL/Quests/Actions/GrantTitleAction")]
	public class GrantTitleAction : QuestAction
	{
		// Token: 0x06003A35 RID: 14901 RVA: 0x0004475B File Offset: 0x0004295B
		public override void Execute(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
		}

		// Token: 0x040038A5 RID: 14501
		[SerializeField]
		private string m_title;
	}
}
