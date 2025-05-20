using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Quests.Actions
{
	// Token: 0x020007C4 RID: 1988
	[CreateAssetMenu(menuName = "SoL/Quests/Actions/RefreshTitlesAction")]
	public class RefreshTitlesAction : QuestAction
	{
		// Token: 0x06003A3B RID: 14907 RVA: 0x00067719 File Offset: 0x00065919
		public override void Execute(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			TitleManager.InvokeTitlesChangedEvent();
		}
	}
}
