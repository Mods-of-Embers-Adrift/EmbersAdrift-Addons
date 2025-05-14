using System;
using System.Collections.Generic;
using SoL.Game.Quests;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x0200074A RID: 1866
	[Serializable]
	public class TitleSettings
	{
		// Token: 0x17000C94 RID: 3220
		// (get) Token: 0x060037A1 RID: 14241 RVA: 0x000660E7 File Offset: 0x000642E7
		public UnlockableTitle[] UnlockableTitles
		{
			get
			{
				return this.m_unlockableTitles;
			}
		}

		// Token: 0x060037A2 RID: 14242 RVA: 0x0016BF94 File Offset: 0x0016A194
		public bool TryGetTitleRequirement(string title, out ProgressionRequirement requirement)
		{
			if (this.m_unlockableTitlesDict == null)
			{
				UnlockableTitle[] unlockableTitles = this.m_unlockableTitles;
				this.m_unlockableTitlesDict = new Dictionary<string, ProgressionRequirement>((unlockableTitles != null) ? unlockableTitles.Length : 0);
				if (this.m_unlockableTitles != null)
				{
					foreach (UnlockableTitle unlockableTitle in this.m_unlockableTitles)
					{
						this.m_unlockableTitlesDict.AddOrReplace(unlockableTitle.Title, unlockableTitle.Requirement);
					}
				}
			}
			return this.m_unlockableTitlesDict.TryGetValue(title, out requirement);
		}

		// Token: 0x04003686 RID: 13958
		[SerializeField]
		private UnlockableTitle[] m_unlockableTitles;

		// Token: 0x04003687 RID: 13959
		private Dictionary<string, ProgressionRequirement> m_unlockableTitlesDict;
	}
}
