using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Quests.Actions
{
	// Token: 0x020007C8 RID: 1992
	[CreateAssetMenu(menuName = "SoL/Quests/Actions/TutorialsQuestAction")]
	public class TutorialsQuestAction : QuestAction
	{
		// Token: 0x06003A44 RID: 14916 RVA: 0x00175B90 File Offset: 0x00173D90
		public override void Execute(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			base.Execute(cache, sourceEntity);
			if (GameManager.IsServer || Options.GameOptions.HideTutorialPopups.Value)
			{
				return;
			}
			foreach (TutorialProgress progress in this.m_tutorials)
			{
				ClientGameManager.NotificationsManager.TryShowTutorial(progress);
			}
		}

		// Token: 0x040038AB RID: 14507
		[SerializeField]
		private TutorialProgress[] m_tutorials;
	}
}
