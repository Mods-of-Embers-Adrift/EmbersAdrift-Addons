using System;
using SoL.Managers;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.Quests.Actions
{
	// Token: 0x020007C7 RID: 1991
	[CreateAssetMenu(menuName = "SoL/Quests/Actions/TutorialPopupsQuestAction")]
	public class TutorialPopupsQuestAction : QuestAction
	{
		// Token: 0x06003A42 RID: 14914 RVA: 0x00175B34 File Offset: 0x00173D34
		public override void Execute(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			base.Execute(cache, sourceEntity);
			if (GameManager.IsServer || (this.m_respectHideTutorialsOption && Options.GameOptions.HideTutorialPopups.Value))
			{
				return;
			}
			foreach (TutorialPopupOptions opts in this.m_options)
			{
				ClientGameManager.UIManager.InitTutorialPopup(opts);
			}
		}

		// Token: 0x040038A9 RID: 14505
		[SerializeField]
		private TutorialPopupOptions[] m_options;

		// Token: 0x040038AA RID: 14506
		[SerializeField]
		private bool m_respectHideTutorialsOption = true;
	}
}
