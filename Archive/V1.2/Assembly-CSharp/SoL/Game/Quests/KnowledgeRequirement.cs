using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SoL.Game.NPCs;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.Quests
{
	// Token: 0x02000784 RID: 1924
	[Serializable]
	public class KnowledgeRequirement : IRequirement
	{
		// Token: 0x060038CB RID: 14539 RVA: 0x00170760 File Offset: 0x0016E960
		public bool MeetsAllRequirements(GameEntity entity)
		{
			bool flag = true;
			if (this.m_known != null)
			{
				foreach (string label in this.m_known)
				{
					flag = (flag && entity.CharacterData.Knows(label));
				}
			}
			if (this.m_notKnown != null)
			{
				foreach (string label2 in this.m_notKnown)
				{
					flag = (flag && !entity.CharacterData.Knows(label2));
				}
			}
			return flag;
		}

		// Token: 0x17000CFA RID: 3322
		// (get) Token: 0x060038CC RID: 14540 RVA: 0x0006689E File Offset: 0x00064A9E
		private IEnumerable m_labels
		{
			get
			{
				return this.FindAllLabels();
			}
		}

		// Token: 0x060038CD RID: 14541 RVA: 0x001707E0 File Offset: 0x0016E9E0
		private IEnumerable FindAllLabels()
		{
			List<IValueDropdownItem> list = new List<IValueDropdownItem>();
			foreach (BaseArchetype baseArchetype in InternalGameDatabase.Archetypes.GetAllItems())
			{
				NpcProfile npcProfile = baseArchetype as NpcProfile;
				if (npcProfile != null && npcProfile.KnowledgeLabels != null)
				{
					foreach (string text in npcProfile.KnowledgeLabels)
					{
						list.Add(new ValueDropdownItem(text, text));
					}
				}
			}
			return list;
		}

		// Token: 0x040037AE RID: 14254
		[SerializeField]
		private string[] m_known;

		// Token: 0x040037AF RID: 14255
		[SerializeField]
		private string[] m_notKnown;
	}
}
