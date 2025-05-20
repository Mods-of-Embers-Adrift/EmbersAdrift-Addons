using System;
using System.Collections.Generic;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.HuntingLog
{
	// Token: 0x02000BD2 RID: 3026
	[Serializable]
	public class HuntingLogTier
	{
		// Token: 0x17001613 RID: 5651
		// (get) Token: 0x06005D7C RID: 23932 RVA: 0x0007ED2B File Offset: 0x0007CF2B
		public int Count
		{
			get
			{
				return this.m_count;
			}
		}

		// Token: 0x17001614 RID: 5652
		// (get) Token: 0x06005D7D RID: 23933 RVA: 0x0007ED33 File Offset: 0x0007CF33
		internal HuntingLogTier.PerkType Type
		{
			get
			{
				return this.m_perkType;
			}
		}

		// Token: 0x17001615 RID: 5653
		// (get) Token: 0x06005D7E RID: 23934 RVA: 0x0007ED3B File Offset: 0x0007CF3B
		public bool HasDefaultTitle
		{
			get
			{
				return this.m_defaultTitle != null && this.m_defaultTitle.HasValue;
			}
		}

		// Token: 0x17001616 RID: 5654
		// (get) Token: 0x06005D7F RID: 23935 RVA: 0x0007ED52 File Offset: 0x0007CF52
		// (set) Token: 0x06005D80 RID: 23936 RVA: 0x0007ED5A File Offset: 0x0007CF5A
		public string DefaultTitle { get; private set; }

		// Token: 0x17001617 RID: 5655
		// (get) Token: 0x06005D81 RID: 23937 RVA: 0x0007ED63 File Offset: 0x0007CF63
		// (set) Token: 0x06005D82 RID: 23938 RVA: 0x0007ED6B File Offset: 0x0007CF6B
		public string[] PerkTitles { get; private set; }

		// Token: 0x06005D83 RID: 23939 RVA: 0x0007ED74 File Offset: 0x0007CF74
		public HuntingLogTier()
		{
		}

		// Token: 0x06005D84 RID: 23940 RVA: 0x001F3EE4 File Offset: 0x001F20E4
		public HuntingLogTier(HuntingLogProfile profile, HuntingLogTier other)
		{
			this.m_profile = profile;
			this.m_count = other.m_count;
			this.m_perkType = other.m_perkType;
			this.m_defaultTitle = other.m_defaultTitle;
			bool flag = false;
			if (profile && profile.TierOverrides != null)
			{
				int i = 0;
				while (i < profile.TierOverrides.Length)
				{
					HuntingLogProfile.TierOverride tierOverride = profile.TierOverrides[i];
					if (tierOverride.Count == this.m_count)
					{
						if (tierOverride.OverrideDefaultTitle)
						{
							this.m_defaultTitle = new HuntingLogTitle(tierOverride.DefaultTitle);
						}
						if (tierOverride.OverridePerkTitles)
						{
							flag = true;
							this.m_perkTitles = new HuntingLogTitle[tierOverride.PerkTitles.Length];
							for (int j = 0; j < tierOverride.PerkTitles.Length; j++)
							{
								this.m_perkTitles[j] = new HuntingLogTitle(tierOverride.PerkTitles[j]);
							}
							break;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
			if (!flag)
			{
				if (other.m_perkTitles != null)
				{
					this.m_perkTitles = new HuntingLogTitle[other.m_perkTitles.Length];
					for (int k = 0; k < other.m_perkTitles.Length; k++)
					{
						this.m_perkTitles[k] = other.m_perkTitles[k];
					}
				}
				else
				{
					this.m_perkTitles = new HuntingLogTitle[0];
				}
			}
			this.InitializeTitles();
		}

		// Token: 0x06005D85 RID: 23941 RVA: 0x001F4030 File Offset: 0x001F2230
		private void InitializeTitles()
		{
			if (!this.m_profile)
			{
				return;
			}
			if (this.HasDefaultTitle)
			{
				this.DefaultTitle = this.m_defaultTitle.GetTitle(this.m_profile);
			}
			this.PerkTitles = new string[this.m_perkTitles.Length];
			for (int i = 0; i < this.m_perkTitles.Length; i++)
			{
				this.PerkTitles[i] = this.m_perkTitles[i].GetTitle(this.m_profile);
			}
		}

		// Token: 0x06005D86 RID: 23942 RVA: 0x001F40AC File Offset: 0x001F22AC
		public string GetPerks()
		{
			HuntingLogTier.PerkType perkType = this.m_perkType;
			if (perkType != HuntingLogTier.PerkType.Title)
			{
				if (perkType == HuntingLogTier.PerkType.Stat)
				{
					if (this.m_profile && this.m_profile.StatPerks != null && this.m_profile.StatPerks.Length != 0)
					{
						List<string> fromPool = StaticListPool<string>.GetFromPool();
						for (int i = 0; i < this.m_profile.StatPerks.Length; i++)
						{
							fromPool.Add(this.m_profile.StatPerks[i].GetPerkDescription(this.m_profile.Settings));
						}
						string result = string.Join("\n", fromPool);
						StaticListPool<string>.ReturnToPool(fromPool);
						return result;
					}
				}
			}
			else if (this.PerkTitles != null && this.PerkTitles.Length != 0)
			{
				return string.Join("\n", this.PerkTitles);
			}
			return "None";
		}

		// Token: 0x040050D7 RID: 20695
		[NonSerialized]
		private readonly HuntingLogProfile m_profile;

		// Token: 0x040050D8 RID: 20696
		[SerializeField]
		private int m_count = 1;

		// Token: 0x040050D9 RID: 20697
		[SerializeField]
		private HuntingLogTier.PerkType m_perkType = HuntingLogTier.PerkType.Stat;

		// Token: 0x040050DA RID: 20698
		[SerializeField]
		private HuntingLogTitle m_defaultTitle;

		// Token: 0x040050DB RID: 20699
		[SerializeField]
		private HuntingLogTitle[] m_perkTitles;

		// Token: 0x02000BD3 RID: 3027
		internal enum PerkType
		{
			// Token: 0x040050DF RID: 20703
			Title,
			// Token: 0x040050E0 RID: 20704
			Stat
		}
	}
}
