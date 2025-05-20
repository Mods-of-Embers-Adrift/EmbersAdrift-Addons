using System;
using SoL.Managers;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.Quests.Actions
{
	// Token: 0x020007BD RID: 1981
	[CreateAssetMenu(menuName = "SoL/Quests/Actions/CenterScreenAnnounceAction")]
	public class CenterScreenAnnounceAction : QuestAction
	{
		// Token: 0x06003A2F RID: 14895 RVA: 0x00175B0C File Offset: 0x00173D0C
		public override void Execute(ObjectiveIterationCache cache, GameEntity sourceEntity)
		{
			base.Execute(cache, sourceEntity);
			if (GameManager.IsServer)
			{
				return;
			}
			foreach (CenterScreenAnnouncementOptions opts in this.m_options)
			{
				ClientGameManager.UIManager.InitCenterScreenAnnouncement(opts);
			}
		}

		// Token: 0x0400389E RID: 14494
		[SerializeField]
		private CenterScreenAnnouncementOptions[] m_options;
	}
}
