using System;
using SoL.Game.Quests;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A34 RID: 2612
	[CreateAssetMenu(menuName = "SoL/Profiles/Dialogue Source")]
	public class DialogueSource : BaseArchetype
	{
		// Token: 0x1700120B RID: 4619
		// (get) Token: 0x060050DE RID: 20702 RVA: 0x000760CC File Offset: 0x000742CC
		public InkEntry DefaultDialogue
		{
			get
			{
				return this.m_defaultDialogue;
			}
		}

		// Token: 0x1700120C RID: 4620
		// (get) Token: 0x060050DF RID: 20703 RVA: 0x000760D4 File Offset: 0x000742D4
		public bool HasDefaultDialogue
		{
			get
			{
				InkEntry defaultDialogue = this.m_defaultDialogue;
				if (((defaultDialogue != null) ? defaultDialogue.InkStory : null) != null)
				{
					InkEntry defaultDialogue2 = this.m_defaultDialogue;
					return !string.IsNullOrEmpty((defaultDialogue2 != null) ? defaultDialogue2.TargetPath : null);
				}
				return false;
			}
		}

		// Token: 0x1700120D RID: 4621
		// (get) Token: 0x060050E0 RID: 20704 RVA: 0x0007610C File Offset: 0x0007430C
		public bool HasQuestDialogue
		{
			get
			{
				return GameManager.QuestManager.HasDialogueState(base.Id);
			}
		}

		// Token: 0x1700120E RID: 4622
		// (get) Token: 0x060050E1 RID: 20705 RVA: 0x0007611E File Offset: 0x0007431E
		public bool HasAnyDialogue
		{
			get
			{
				return this.HasDefaultDialogue || this.HasQuestDialogue;
			}
		}

		// Token: 0x04004868 RID: 18536
		public const string kPlayerProgressionGroup = "Player Progression";

		// Token: 0x04004869 RID: 18537
		[SerializeField]
		private InkEntry m_defaultDialogue;
	}
}
