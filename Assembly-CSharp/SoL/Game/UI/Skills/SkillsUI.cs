using System;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.UI.Skills
{
	// Token: 0x02000932 RID: 2354
	public class SkillsUI : DraggableUIWindow
	{
		// Token: 0x17000F8C RID: 3980
		// (get) Token: 0x06004567 RID: 17767 RVA: 0x0006EBC5 File Offset: 0x0006CDC5
		public SkillsMasteryUI MasteryUI
		{
			get
			{
				return this.m_masteryUi;
			}
		}

		// Token: 0x17000F8D RID: 3981
		// (get) Token: 0x06004568 RID: 17768 RVA: 0x0006EBCD File Offset: 0x0006CDCD
		public SkillsAbilityUI AbilityUI
		{
			get
			{
				return this.m_abilityUi;
			}
		}

		// Token: 0x040041CE RID: 16846
		[SerializeField]
		private SkillsMasteryUI m_masteryUi;

		// Token: 0x040041CF RID: 16847
		[SerializeField]
		private SkillsAbilityUI m_abilityUi;
	}
}
