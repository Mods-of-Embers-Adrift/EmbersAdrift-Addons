using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.EffectSystem;
using SoL.Game.Messages;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.Game.HuntingLog
{
	// Token: 0x02000BCB RID: 3019
	[CreateAssetMenu(menuName = "SoL/Profiles/Hunting Log")]
	public class HuntingLogProfile : BaseArchetype
	{
		// Token: 0x170015FB RID: 5627
		// (get) Token: 0x06005D41 RID: 23873 RVA: 0x0007EAFC File Offset: 0x0007CCFC
		public HuntingLogSettingsProfile Settings
		{
			get
			{
				if (this.m_settingsOverride)
				{
					return this.m_settingsOverride;
				}
				if (GlobalSettings.Values && GlobalSettings.Values.HuntingLog != null)
				{
					return GlobalSettings.Values.HuntingLog.DefaultSettings;
				}
				return null;
			}
		}

		// Token: 0x170015FC RID: 5628
		// (get) Token: 0x06005D42 RID: 23874 RVA: 0x0007EB3B File Offset: 0x0007CD3B
		public int Version
		{
			get
			{
				return this.m_version;
			}
		}

		// Token: 0x170015FD RID: 5629
		// (get) Token: 0x06005D43 RID: 23875 RVA: 0x0007EB43 File Offset: 0x0007CD43
		public int SettingsVersion
		{
			get
			{
				if (!this.Settings)
				{
					return 0;
				}
				return this.Settings.Version;
			}
		}

		// Token: 0x170015FE RID: 5630
		// (get) Token: 0x06005D44 RID: 23876 RVA: 0x0007EB5F File Offset: 0x0007CD5F
		public string TitlePrefix
		{
			get
			{
				return this.m_titlePrefix;
			}
		}

		// Token: 0x170015FF RID: 5631
		// (get) Token: 0x06005D45 RID: 23877 RVA: 0x0007EB67 File Offset: 0x0007CD67
		public bool ShowInLog
		{
			get
			{
				return !this.m_hideInLog;
			}
		}

		// Token: 0x17001600 RID: 5632
		// (get) Token: 0x06005D46 RID: 23878 RVA: 0x0007EB72 File Offset: 0x0007CD72
		private HuntingLogPerkType[] m_validStatPerks
		{
			get
			{
				return HuntingLogExtensions.StatPerks;
			}
		}

		// Token: 0x17001601 RID: 5633
		// (get) Token: 0x06005D47 RID: 23879 RVA: 0x0007EB79 File Offset: 0x0007CD79
		internal HuntingLogPerkType[] StatPerks
		{
			get
			{
				return this.m_statPerks;
			}
		}

		// Token: 0x17001602 RID: 5634
		// (get) Token: 0x06005D48 RID: 23880 RVA: 0x0007EB81 File Offset: 0x0007CD81
		internal HuntingLogProfile.TierOverride[] TierOverrides
		{
			get
			{
				return this.m_tierOverrides;
			}
		}

		// Token: 0x06005D49 RID: 23881 RVA: 0x0007EB89 File Offset: 0x0007CD89
		private void ReinitializeTiers()
		{
			this.m_tiersInitialized = false;
			this.m_internalTiers = null;
		}

		// Token: 0x06005D4A RID: 23882 RVA: 0x0007EB99 File Offset: 0x0007CD99
		private void SortStatPerks()
		{
			Array.Sort<HuntingLogPerkType>(this.m_statPerks, delegate(HuntingLogPerkType a, HuntingLogPerkType b)
			{
				int num = (int)a;
				return num.CompareTo((int)b);
			});
		}

		// Token: 0x06005D4B RID: 23883 RVA: 0x001F3490 File Offset: 0x001F1690
		private void InitializeTiers()
		{
			if (this.ShowInLog && !this.m_tiersInitialized)
			{
				this.m_internalTiers = new Dictionary<int, HuntingLogTier>();
				if (this.Settings && this.Settings.Tiers != null)
				{
					for (int i = 0; i < this.Settings.Tiers.Length; i++)
					{
						if (this.Settings.Tiers[i] != null)
						{
							HuntingLogTier huntingLogTier = new HuntingLogTier(this, this.Settings.Tiers[i]);
							this.m_internalTiers.Add(huntingLogTier.Count, huntingLogTier);
						}
					}
				}
				this.m_tiersInitialized = true;
			}
		}

		// Token: 0x06005D4C RID: 23884 RVA: 0x001F352C File Offset: 0x001F172C
		public int TiersMet(int current)
		{
			this.InitializeTiers();
			int num = 0;
			if (this.m_internalTiers != null)
			{
				foreach (KeyValuePair<int, HuntingLogTier> keyValuePair in this.m_internalTiers)
				{
					if (current >= keyValuePair.Key)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x06005D4D RID: 23885 RVA: 0x0007EBC5 File Offset: 0x0007CDC5
		public bool Tier(int current)
		{
			this.InitializeTiers();
			return this.m_internalTiers != null && this.m_internalTiers.ContainsKey(current);
		}

		// Token: 0x06005D4E RID: 23886 RVA: 0x001F3598 File Offset: 0x001F1798
		public bool TitleUnlocked(int current, out string titleUnlocked)
		{
			titleUnlocked = string.Empty;
			this.InitializeTiers();
			HuntingLogTier huntingLogTier;
			if (this.m_internalTiers != null && this.m_internalTiers.TryGetValue(current, out huntingLogTier) && huntingLogTier.HasDefaultTitle && !string.IsNullOrEmpty(huntingLogTier.DefaultTitle))
			{
				titleUnlocked = huntingLogTier.DefaultTitle;
				return true;
			}
			return false;
		}

		// Token: 0x06005D4F RID: 23887 RVA: 0x001F35EC File Offset: 0x001F17EC
		public bool PerkUnlocked(int current, out string perkType)
		{
			this.InitializeTiers();
			perkType = string.Empty;
			HuntingLogTier huntingLogTier;
			if (this.m_internalTiers != null && this.m_internalTiers.TryGetValue(current, out huntingLogTier))
			{
				perkType = huntingLogTier.Type.ToString();
				return true;
			}
			return false;
		}

		// Token: 0x06005D50 RID: 23888 RVA: 0x001F3638 File Offset: 0x001F1838
		public void AddAcquiredTitles(HuntingLogEntry huntingLogEntry, List<string> currentTitles)
		{
			if (huntingLogEntry == null)
			{
				return;
			}
			this.InitializeTiers();
			if (this.m_internalTiers != null)
			{
				foreach (KeyValuePair<int, HuntingLogTier> keyValuePair in this.m_internalTiers)
				{
					if (huntingLogEntry.PerkCount >= keyValuePair.Key && keyValuePair.Value != null && keyValuePair.Value.HasDefaultTitle && !string.IsNullOrEmpty(keyValuePair.Value.DefaultTitle))
					{
						currentTitles.Add(keyValuePair.Value.DefaultTitle);
					}
				}
				if (huntingLogEntry.ActivePerks != null)
				{
					foreach (KeyValuePair<int, HuntingLogPerkType> keyValuePair2 in huntingLogEntry.ActivePerks)
					{
						HuntingLogTier huntingLogTier;
						if (this.m_internalTiers.TryGetValue(keyValuePair2.Key, out huntingLogTier) && huntingLogTier.Type == HuntingLogTier.PerkType.Title && huntingLogTier.PerkTitles != null && (int)keyValuePair2.Value < huntingLogTier.PerkTitles.Length)
						{
							int value = (int)keyValuePair2.Value;
							currentTitles.Add(huntingLogTier.PerkTitles[value]);
						}
					}
				}
			}
		}

		// Token: 0x06005D51 RID: 23889 RVA: 0x001F3780 File Offset: 0x001F1980
		public string GetSelectedTitle(int count, HuntingLogPerkType perkType)
		{
			this.InitializeTiers();
			HuntingLogTier huntingLogTier;
			if (perkType.IsTitle() && this.m_internalTiers.TryGetValue(count, out huntingLogTier) && huntingLogTier.Type == HuntingLogTier.PerkType.Title && huntingLogTier.PerkTitles != null && (int)perkType < huntingLogTier.PerkTitles.Length)
			{
				return huntingLogTier.PerkTitles[(int)perkType];
			}
			return string.Empty;
		}

		// Token: 0x06005D52 RID: 23890 RVA: 0x001F37D8 File Offset: 0x001F19D8
		public bool IsAcquiredTitle(int currentCount, string title, HuntingLogEntry huntingLogEntry)
		{
			this.InitializeTiers();
			if (this.m_internalTiers != null)
			{
				foreach (KeyValuePair<int, HuntingLogTier> keyValuePair in this.m_internalTiers)
				{
					if (currentCount >= keyValuePair.Key && keyValuePair.Value != null && keyValuePair.Value.HasDefaultTitle && !string.IsNullOrEmpty(keyValuePair.Value.DefaultTitle) && title.Equals(keyValuePair.Value.DefaultTitle))
					{
						return true;
					}
				}
				if (huntingLogEntry.ActivePerks != null)
				{
					foreach (KeyValuePair<int, HuntingLogPerkType> keyValuePair2 in huntingLogEntry.ActivePerks)
					{
						HuntingLogTier huntingLogTier;
						if (this.m_internalTiers.TryGetValue(keyValuePair2.Key, out huntingLogTier) && huntingLogTier.Type == HuntingLogTier.PerkType.Title && huntingLogTier.PerkTitles != null && (int)keyValuePair2.Value < huntingLogTier.PerkTitles.Length)
						{
							int value = (int)keyValuePair2.Value;
							string value2 = huntingLogTier.PerkTitles[value];
							if (title.Equals(value2))
							{
								return true;
							}
						}
					}
					return false;
				}
				return false;
			}
			return false;
		}

		// Token: 0x06005D53 RID: 23891 RVA: 0x001F392C File Offset: 0x001F1B2C
		private static int HuntingLogResultComparison(HuntingLogProfile.HuntingLogResults a, HuntingLogProfile.HuntingLogResults b)
		{
			int num = b.Count.CompareTo(a.Count);
			if (num != 0)
			{
				return num;
			}
			return string.Compare(a.Name, b.Name, StringComparison.InvariantCultureIgnoreCase);
		}

		// Token: 0x06005D54 RID: 23892 RVA: 0x001F3968 File Offset: 0x001F1B68
		public static void PrintHuntingLogToChat()
		{
			if (GlobalSettings.Values.HuntingLog.Disabled)
			{
				return;
			}
			if (LocalPlayer.GameEntity == null || LocalPlayer.GameEntity.CollectionController == null || LocalPlayer.GameEntity.CollectionController.Record == null)
			{
				return;
			}
			if (LocalPlayer.GameEntity.CollectionController.Record.HuntingLog == null)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "No hunting log entries!");
				return;
			}
			if (HuntingLogProfile.m_huntingLogResults == null)
			{
				HuntingLogProfile.m_huntingLogResults = new List<HuntingLogProfile.HuntingLogResults>(100);
			}
			else
			{
				HuntingLogProfile.m_huntingLogResults.Clear();
			}
			foreach (KeyValuePair<UniqueId, HuntingLogEntry> keyValuePair in LocalPlayer.GameEntity.CollectionController.Record.HuntingLog)
			{
				HuntingLogProfile huntingLogProfile;
				if (InternalGameDatabase.Archetypes.TryGetAsType<HuntingLogProfile>(keyValuePair.Key, out huntingLogProfile) && huntingLogProfile.ShowInLog)
				{
					huntingLogProfile.InitializeTiers();
					HuntingLogProfile.m_huntingLogResults.Add(new HuntingLogProfile.HuntingLogResults(huntingLogProfile.TitlePrefix, keyValuePair.Value.TotalCount));
				}
			}
			if (HuntingLogProfile.m_huntingLogResultSorter == null)
			{
				HuntingLogProfile.m_huntingLogResultSorter = new Comparison<HuntingLogProfile.HuntingLogResults>(HuntingLogProfile.HuntingLogResultComparison);
			}
			HuntingLogProfile.m_huntingLogResults.Sort(HuntingLogProfile.m_huntingLogResultSorter);
			using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
			{
				utf16ValueStringBuilder.Append("Hunting Log:\n");
				for (int i = 0; i < HuntingLogProfile.m_huntingLogResults.Count; i++)
				{
					utf16ValueStringBuilder.AppendFormat<string, ulong>("{0}: {1}", HuntingLogProfile.m_huntingLogResults[i].Name, HuntingLogProfile.m_huntingLogResults[i].Count);
					if (i != HuntingLogProfile.m_huntingLogResults.Count - 1)
					{
						utf16ValueStringBuilder.Append("\n");
					}
				}
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, utf16ValueStringBuilder.ToString());
			}
			HuntingLogProfile.m_huntingLogResults.Clear();
		}

		// Token: 0x06005D55 RID: 23893 RVA: 0x0007EBE3 File Offset: 0x0007CDE3
		public IEnumerable<HuntingLogTier> GetTiers()
		{
			this.InitializeTiers();
			foreach (KeyValuePair<int, HuntingLogTier> keyValuePair in this.m_internalTiers)
			{
				yield return keyValuePair.Value;
			}
			Dictionary<int, HuntingLogTier>.Enumerator enumerator = default(Dictionary<int, HuntingLogTier>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06005D56 RID: 23894 RVA: 0x0007EBF3 File Offset: 0x0007CDF3
		public bool TryGetTier(int value, out HuntingLogTier tier)
		{
			this.InitializeTiers();
			tier = null;
			return this.m_internalTiers.TryGetValue(value, out tier);
		}

		// Token: 0x06005D57 RID: 23895 RVA: 0x001F3B68 File Offset: 0x001F1D68
		public bool IsValidPerk(HuntingLogEntry entry, HuntingLogPerkType perkType, int tierCount)
		{
			this.InitializeTiers();
			HuntingLogTier huntingLogTier;
			if (entry != null && entry.PerkCount >= tierCount && this.m_internalTiers.TryGetValue(tierCount, out huntingLogTier))
			{
				HuntingLogTier.PerkType type = huntingLogTier.Type;
				if (type == HuntingLogTier.PerkType.Title)
				{
					return perkType.IsTitle();
				}
				if (type == HuntingLogTier.PerkType.Stat)
				{
					for (int i = 0; i < this.m_statPerks.Length; i++)
					{
						if (this.m_statPerks[i] == perkType)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06005D58 RID: 23896 RVA: 0x001F3BD0 File Offset: 0x001F1DD0
		public int GetNextLowerTierCount(int currentTier)
		{
			this.InitializeTiers();
			int num = 0;
			foreach (KeyValuePair<int, HuntingLogTier> keyValuePair in this.m_internalTiers)
			{
				if (keyValuePair.Key > num && keyValuePair.Key < currentTier)
				{
					num = keyValuePair.Key;
				}
			}
			return num;
		}

		// Token: 0x06005D59 RID: 23897 RVA: 0x001F3C44 File Offset: 0x001F1E44
		public bool HasStat(StatType statType, out int value)
		{
			value = 0;
			if (this.m_statPerks != null)
			{
				for (int i = 0; i < this.m_statPerks.Length; i++)
				{
					HuntingLogPerkType? huntingLogPerkType = statType.GetHuntingLogPerkType();
					HuntingLogPerkType huntingLogPerkType2 = this.m_statPerks[i];
					if (huntingLogPerkType.GetValueOrDefault() == huntingLogPerkType2 & huntingLogPerkType != null)
					{
						value++;
					}
				}
			}
			return value != 0;
		}

		// Token: 0x040050B7 RID: 20663
		[SerializeField]
		private HuntingLogSettingsProfile m_settingsOverride;

		// Token: 0x040050B8 RID: 20664
		[SerializeField]
		private int m_version;

		// Token: 0x040050B9 RID: 20665
		[SerializeField]
		private bool m_hideInLog;

		// Token: 0x040050BA RID: 20666
		[SerializeField]
		private string m_titlePrefix;

		// Token: 0x040050BB RID: 20667
		[SerializeField]
		private HuntingLogPerkType[] m_statPerks;

		// Token: 0x040050BC RID: 20668
		[FormerlySerializedAs("m_thresholdOverrides")]
		[SerializeField]
		private HuntingLogProfile.TierOverride[] m_tierOverrides;

		// Token: 0x040050BD RID: 20669
		[NonSerialized]
		private Dictionary<int, HuntingLogTier> m_internalTiers;

		// Token: 0x040050BE RID: 20670
		[NonSerialized]
		private bool m_tiersInitialized;

		// Token: 0x040050BF RID: 20671
		private static List<HuntingLogProfile.HuntingLogResults> m_huntingLogResults;

		// Token: 0x040050C0 RID: 20672
		private static Comparison<HuntingLogProfile.HuntingLogResults> m_huntingLogResultSorter;

		// Token: 0x02000BCC RID: 3020
		[Serializable]
		public class TierOverride
		{
			// Token: 0x17001603 RID: 5635
			// (get) Token: 0x06005D5B RID: 23899 RVA: 0x0007EC0B File Offset: 0x0007CE0B
			public int Count
			{
				get
				{
					return this.m_count;
				}
			}

			// Token: 0x17001604 RID: 5636
			// (get) Token: 0x06005D5C RID: 23900 RVA: 0x0007EC13 File Offset: 0x0007CE13
			public bool OverrideDefaultTitle
			{
				get
				{
					return this.m_overrideDefaultTitle;
				}
			}

			// Token: 0x17001605 RID: 5637
			// (get) Token: 0x06005D5D RID: 23901 RVA: 0x0007EC1B File Offset: 0x0007CE1B
			public HuntingLogTitle DefaultTitle
			{
				get
				{
					return this.m_defaultTitle;
				}
			}

			// Token: 0x17001606 RID: 5638
			// (get) Token: 0x06005D5E RID: 23902 RVA: 0x0007EC23 File Offset: 0x0007CE23
			public bool OverridePerkTitles
			{
				get
				{
					return this.m_overridePerkTitles;
				}
			}

			// Token: 0x17001607 RID: 5639
			// (get) Token: 0x06005D5F RID: 23903 RVA: 0x0007EC2B File Offset: 0x0007CE2B
			public HuntingLogTitle[] PerkTitles
			{
				get
				{
					return this.m_perkTitles;
				}
			}

			// Token: 0x040050C1 RID: 20673
			[SerializeField]
			private int m_count = 1;

			// Token: 0x040050C2 RID: 20674
			[SerializeField]
			private bool m_overrideDefaultTitle;

			// Token: 0x040050C3 RID: 20675
			[SerializeField]
			private HuntingLogTitle m_defaultTitle;

			// Token: 0x040050C4 RID: 20676
			[SerializeField]
			private bool m_overridePerkTitles;

			// Token: 0x040050C5 RID: 20677
			[SerializeField]
			private HuntingLogTitle[] m_perkTitles;
		}

		// Token: 0x02000BCD RID: 3021
		private struct HuntingLogResults
		{
			// Token: 0x06005D61 RID: 23905 RVA: 0x0007EC42 File Offset: 0x0007CE42
			public HuntingLogResults(string titlePrefix, ulong count)
			{
				this.Name = titlePrefix;
				this.Count = count;
			}

			// Token: 0x040050C6 RID: 20678
			public readonly string Name;

			// Token: 0x040050C7 RID: 20679
			public readonly ulong Count;
		}
	}
}
