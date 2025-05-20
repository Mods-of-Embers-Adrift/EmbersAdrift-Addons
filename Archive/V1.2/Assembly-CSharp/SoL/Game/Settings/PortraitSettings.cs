using System;
using System.Collections.Generic;

namespace SoL.Game.Settings
{
	// Token: 0x02000739 RID: 1849
	[Serializable]
	public class PortraitSettings
	{
		// Token: 0x17000C73 RID: 3187
		// (get) Token: 0x06003751 RID: 14161 RVA: 0x0016B530 File Offset: 0x00169730
		public List<IdentifiableSprite> BasePortraits
		{
			get
			{
				if (PortraitSettings.m_basePortraits == null)
				{
					PortraitSettings.m_basePortraits = new List<IdentifiableSprite>(9);
					if (this.PlayerPortraits)
					{
						int num = 0;
						foreach (IdentifiableSprite item in this.PlayerPortraits.GetObjects())
						{
							PortraitSettings.m_basePortraits.Add(item);
							num++;
							if (num >= 9)
							{
								break;
							}
						}
					}
				}
				return PortraitSettings.m_basePortraits;
			}
		}

		// Token: 0x17000C74 RID: 3188
		// (get) Token: 0x06003752 RID: 14162 RVA: 0x0016B5B8 File Offset: 0x001697B8
		public HashSet<UniqueId> BasePortraitIds
		{
			get
			{
				if (PortraitSettings.m_basePortraitIds == null)
				{
					PortraitSettings.m_basePortraitIds = new HashSet<UniqueId>(default(UniqueIdComparer));
					for (int i = 0; i < this.BasePortraits.Count; i++)
					{
						PortraitSettings.m_basePortraitIds.Add(this.BasePortraits[i].Id);
					}
				}
				return PortraitSettings.m_basePortraitIds;
			}
		}

		// Token: 0x17000C75 RID: 3189
		// (get) Token: 0x06003753 RID: 14163 RVA: 0x0016B61C File Offset: 0x0016981C
		public HashSet<UniqueId> PlayerPortraitIds
		{
			get
			{
				if (PortraitSettings.m_playerPortraitIds == null)
				{
					PortraitSettings.m_playerPortraitIds = new HashSet<UniqueId>(default(UniqueIdComparer));
					if (this.PlayerPortraits)
					{
						foreach (IdentifiableSprite identifiableSprite in this.PlayerPortraits.GetObjects())
						{
							PortraitSettings.m_playerPortraitIds.Add(identifiableSprite.Id);
						}
					}
				}
				return PortraitSettings.m_playerPortraitIds;
			}
		}

		// Token: 0x04003612 RID: 13842
		private const int kMaxBasePortraits = 9;

		// Token: 0x04003613 RID: 13843
		public IdentifiableSpriteCollection AllPortraits;

		// Token: 0x04003614 RID: 13844
		public IdentifiableSpriteCollection PlayerPortraits;

		// Token: 0x04003615 RID: 13845
		[NonSerialized]
		private static List<IdentifiableSprite> m_basePortraits;

		// Token: 0x04003616 RID: 13846
		[NonSerialized]
		private static HashSet<UniqueId> m_basePortraitIds;

		// Token: 0x04003617 RID: 13847
		[NonSerialized]
		private static HashSet<UniqueId> m_playerPortraitIds;
	}
}
