using System;
using SoL.Game.Quests;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x020005E9 RID: 1513
	[Serializable]
	public class UnlockableTitle
	{
		// Token: 0x17000A23 RID: 2595
		// (get) Token: 0x06002FE7 RID: 12263 RVA: 0x00061015 File Offset: 0x0005F215
		public string Title
		{
			get
			{
				return this.m_title;
			}
		}

		// Token: 0x17000A24 RID: 2596
		// (get) Token: 0x06002FE8 RID: 12264 RVA: 0x0006101D File Offset: 0x0005F21D
		public ProgressionRequirement Requirement
		{
			get
			{
				return this.m_requirement;
			}
		}

		// Token: 0x04002ECA RID: 11978
		[SerializeField]
		private string m_title;

		// Token: 0x04002ECB RID: 11979
		[SerializeField]
		private ProgressionRequirement m_requirement;
	}
}
