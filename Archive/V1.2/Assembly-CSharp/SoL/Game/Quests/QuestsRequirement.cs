using System;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.Quests
{
	// Token: 0x0200078D RID: 1933
	[Serializable]
	public class QuestsRequirement : IRequirement
	{
		// Token: 0x0600390B RID: 14603 RVA: 0x00171994 File Offset: 0x0016FB94
		public bool MeetsAllRequirements(GameEntity entity)
		{
			bool flag = true;
			if (this.m_questRequirements != null)
			{
				foreach (QuestRequirement questRequirement in this.m_questRequirements)
				{
					flag = (flag && questRequirement.MeetsAllRequirements(entity));
				}
			}
			return flag;
		}

		// Token: 0x040037E0 RID: 14304
		[SerializeField]
		private QuestRequirement[] m_questRequirements;
	}
}
