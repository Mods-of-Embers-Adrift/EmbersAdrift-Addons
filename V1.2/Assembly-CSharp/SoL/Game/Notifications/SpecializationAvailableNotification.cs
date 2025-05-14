using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.UI.Skills;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Notifications
{
	// Token: 0x02000848 RID: 2120
	[CreateAssetMenu(menuName = "SoL/Notifications/Specialization Available")]
	public class SpecializationAvailableNotification : BaseNotification
	{
		// Token: 0x17000E1E RID: 3614
		// (get) Token: 0x06003D1E RID: 15646 RVA: 0x0004479C File Offset: 0x0004299C
		public override bool CanOpen
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003D1F RID: 15647 RVA: 0x00181C18 File Offset: 0x0017FE18
		public override void Open(object data = null)
		{
			MasteryArchetype masteryArchetype = data as MasteryArchetype;
			if (masteryArchetype != null)
			{
				UIManager uimanager = ClientGameManager.UIManager;
				if (uimanager != null)
				{
					SkillsUI skillsUI = uimanager.SkillsUI;
					if (skillsUI != null)
					{
						skillsUI.Show(false);
					}
				}
				UIManager uimanager2 = ClientGameManager.UIManager;
				if (uimanager2 == null)
				{
					return;
				}
				SkillsUI skillsUI2 = uimanager2.SkillsUI;
				if (skillsUI2 == null)
				{
					return;
				}
				SkillsMasteryUI masteryUI = skillsUI2.MasteryUI;
				if (masteryUI == null)
				{
					return;
				}
				masteryUI.SwitchToMastery(masteryArchetype.Id);
			}
		}
	}
}
