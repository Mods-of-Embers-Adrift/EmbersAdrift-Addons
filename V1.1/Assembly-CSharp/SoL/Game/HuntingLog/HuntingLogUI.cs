using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Objects.Containers;
using SoL.Game.UI;
using SoL.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SoL.Game.HuntingLog
{
	// Token: 0x02000BDA RID: 3034
	public class HuntingLogUI : MonoBehaviour
	{
		// Token: 0x17001628 RID: 5672
		// (get) Token: 0x06005DD3 RID: 24019 RVA: 0x0007F197 File Offset: 0x0007D397
		internal HuntingLogUI.InternalUITier[] UITiers
		{
			get
			{
				return this.m_uiTiers;
			}
		}

		// Token: 0x17001629 RID: 5673
		// (get) Token: 0x06005DD4 RID: 24020 RVA: 0x0007F19F File Offset: 0x0007D39F
		// (set) Token: 0x06005DD5 RID: 24021 RVA: 0x0007F1A7 File Offset: 0x0007D3A7
		internal bool RespecConfirmationActive { get; set; }

		// Token: 0x1700162A RID: 5674
		// (get) Token: 0x06005DD6 RID: 24022 RVA: 0x0007F1B0 File Offset: 0x0007D3B0
		internal HuntingLogEntry SelectedEntry
		{
			get
			{
				return this.m_selectedEntry;
			}
		}

		// Token: 0x06005DD7 RID: 24023 RVA: 0x001F50B0 File Offset: 0x001F32B0
		private void Start()
		{
			this.m_activeIndicators = new List<HuntingLogTierIndicatorUI>(10);
			this.m_huntingLogList.Controller = this;
			this.m_huntingLogList.SelectionChanged += this.OnSelectionChanged;
			PlayerCollectionController.HuntingLogEntryAdded += this.OnHuntingLogEntryAdded;
			PlayerCollectionController.HuntingLogEntryModified += this.OnHuntingLogEntryModified;
			PlayerCollectionController.HuntingLogEntryRemoved += this.OnHuntingLogEntryRemoved;
			this.m_perkSelection.Controller = this;
			for (int i = 0; i < this.m_uiTiers.Length; i++)
			{
				this.m_uiTiers[i].Index = i;
			}
		}

		// Token: 0x06005DD8 RID: 24024 RVA: 0x001F5150 File Offset: 0x001F3350
		private void OnDestroy()
		{
			this.m_huntingLogList.SelectionChanged -= this.OnSelectionChanged;
			PlayerCollectionController.HuntingLogEntryAdded -= this.OnHuntingLogEntryAdded;
			PlayerCollectionController.HuntingLogEntryModified -= this.OnHuntingLogEntryModified;
			PlayerCollectionController.HuntingLogEntryRemoved -= this.OnHuntingLogEntryRemoved;
			this.m_huntingLogList.FullyInitialized -= this.UpdateList;
		}

		// Token: 0x06005DD9 RID: 24025 RVA: 0x0007F1B8 File Offset: 0x0007D3B8
		private void OnHuntingLogEntryModified()
		{
			this.m_huntingLogList.RefreshItems();
			this.RefreshVisuals();
		}

		// Token: 0x06005DDA RID: 24026 RVA: 0x0007F1CB File Offset: 0x0007D3CB
		private void OnHuntingLogEntryRemoved()
		{
			this.m_huntingLogList.RefreshItems();
			this.m_selectedEntry = null;
			this.RefreshVisuals();
		}

		// Token: 0x06005DDB RID: 24027 RVA: 0x0007F1E5 File Offset: 0x0007D3E5
		private void OnHuntingLogEntryAdded()
		{
			this.m_huntingLogList.RefreshItems();
		}

		// Token: 0x06005DDC RID: 24028 RVA: 0x0007F1F2 File Offset: 0x0007D3F2
		private void OnSelectionChanged(HuntingLogEntry entry)
		{
			this.m_selectedEntry = entry;
			this.RefreshVisuals();
		}

		// Token: 0x06005DDD RID: 24029 RVA: 0x001F51C0 File Offset: 0x001F33C0
		private int GetMaxTierCount()
		{
			if (this.m_uiTiers != null && this.m_uiTiers.Length != 0)
			{
				HuntingLogUI.InternalUITier internalUITier = this.m_uiTiers[this.m_uiTiers.Length - 1];
				if (internalUITier != null)
				{
					return internalUITier.Count;
				}
			}
			return 0;
		}

		// Token: 0x06005DDE RID: 24030 RVA: 0x001F51FC File Offset: 0x001F33FC
		internal int GetNextTierCount(int value)
		{
			int num = 0;
			HuntingLogProfile huntingLogProfile;
			if (this.SelectedEntry != null && this.SelectedEntry.TryGetProfile(out huntingLogProfile) && huntingLogProfile.Settings)
			{
				num = huntingLogProfile.Settings.MaxPerkCount;
			}
			if (value >= num)
			{
				return num;
			}
			if (this.m_uiTiers != null && this.m_uiTiers.Length != 0)
			{
				for (int i = 0; i < this.m_uiTiers.Length; i++)
				{
					if (value < this.m_uiTiers[i].Count)
					{
						return this.m_uiTiers[i].Count;
					}
				}
			}
			return 0;
		}

		// Token: 0x06005DDF RID: 24031 RVA: 0x001F5284 File Offset: 0x001F3484
		private HuntingLogTierIndicatorUI GetMatchingTierIndicator(int value, bool useTotal)
		{
			if (this.m_uiTiers != null && this.m_uiTiers.Length != 0)
			{
				int i = 0;
				while (i < this.m_uiTiers.Length)
				{
					if (value == this.m_uiTiers[i].Count)
					{
						if (!useTotal)
						{
							return this.m_uiTiers[i].Current;
						}
						return this.m_uiTiers[i].Total;
					}
					else
					{
						i++;
					}
				}
			}
			return null;
		}

		// Token: 0x06005DE0 RID: 24032 RVA: 0x0007F201 File Offset: 0x0007D401
		public void Show()
		{
			this.UpdateListWhenReady();
			this.RefreshVisuals();
		}

		// Token: 0x06005DE1 RID: 24033 RVA: 0x0007F20F File Offset: 0x0007D40F
		private void UpdateListWhenReady()
		{
			if (this.m_huntingLogList.IsFullyInitialized)
			{
				this.UpdateList();
				return;
			}
			this.m_huntingLogList.FullyInitialized += this.UpdateList;
		}

		// Token: 0x06005DE2 RID: 24034 RVA: 0x0007F1E5 File Offset: 0x0007D3E5
		private void UpdateList()
		{
			this.m_huntingLogList.RefreshItems();
		}

		// Token: 0x06005DE3 RID: 24035 RVA: 0x001F52E8 File Offset: 0x001F34E8
		internal void RefreshVisuals()
		{
			if (this.m_selectedEntry != null)
			{
				this.UpdateAndToggleTiers();
				this.AlignCurrentTierIndicators();
				HuntingLogProfile profile = this.m_selectedEntry.GetProfile();
				this.m_portraitImage.overrideSprite = profile.Icon;
				this.m_label.ZStringSetText(profile.TitlePrefix);
				this.m_totalKillCount.SetTextFormat("Kills: {0}", this.m_selectedEntry.TotalCount);
				int nextTierCount = this.GetNextTierCount(this.m_selectedEntry.PerkCount);
				float fillAmount = (float)this.m_selectedEntry.PerkCount / (float)nextTierCount;
				if (this.m_currentProgressFill != null)
				{
					this.m_currentProgressFill.fillAmount = fillAmount;
				}
				int maxTierCount = this.GetMaxTierCount();
				float fillAmount2 = (float)this.m_selectedEntry.PerkCount / (float)maxTierCount;
				if (this.m_totalProgressFill != null)
				{
					this.m_totalProgressFill.fillAmount = fillAmount2;
				}
				foreach (HuntingLogTier huntingLogTier in profile.GetTiers())
				{
					HuntingLogTierIndicatorUI matchingTierIndicator = this.GetMatchingTierIndicator(huntingLogTier.Count, true);
					if (matchingTierIndicator)
					{
						matchingTierIndicator.Init(this, profile, huntingLogTier);
					}
				}
				this.m_edgeLabel.SetText(this.m_selectedEntry.PerkCount);
				this.m_rightPanelData.SetActive(true);
				this.RefreshTallyText();
				return;
			}
			this.m_rightPanelData.SetActive(false);
			this.m_portraitImage.overrideSprite = null;
			this.m_label.text = string.Empty;
			this.m_totalKillCount.text = string.Empty;
		}

		// Token: 0x06005DE4 RID: 24036 RVA: 0x001F547C File Offset: 0x001F367C
		private void RefreshTallyText()
		{
			if (this.m_selectedEntry == null)
			{
				this.m_tallyLabel.ZStringSetText(string.Empty);
				return;
			}
			HuntingLogProfile profile = this.m_selectedEntry.GetProfile();
			if (profile == null || profile.StatPerks == null)
			{
				this.m_tallyLabel.ZStringSetText(string.Empty);
				return;
			}
			List<string> fromPool = StaticListPool<string>.GetFromPool();
			for (int i = 0; i < profile.StatPerks.Length; i++)
			{
				HuntingLogPerkType perkType = profile.StatPerks[i];
				int num;
				int arg = this.m_selectedEntry.TryGetPerkValue(perkType, out num) ? num : 0;
				fromPool.Add(ZString.Format<string, int>("{0} +{1}", perkType.GetTooltipDisplay(true), arg));
			}
			if (fromPool.Count > 0)
			{
				fromPool.Add("<size=90%><i>against this creature type</i></size>");
				string arg2 = string.Join("\n", fromPool).Replace("Debuff Resists", HuntingLogUI.kDebuffReplacementText);
				this.m_tallyLabel.ZStringSetText(arg2);
			}
			else
			{
				this.m_tallyLabel.ZStringSetText(string.Empty);
			}
			StaticListPool<string>.ReturnToPool(fromPool);
		}

		// Token: 0x06005DE5 RID: 24037 RVA: 0x001F5578 File Offset: 0x001F3778
		private void UpdateAndToggleTiers()
		{
			if (this.m_uiTiers == null || this.m_uiTiers.Length == 0)
			{
				return;
			}
			if (Application.isPlaying && this.m_selectedEntry == null)
			{
				return;
			}
			HuntingLogProfile huntingLogProfile;
			if (!this.m_selectedEntry.TryGetProfile(out huntingLogProfile))
			{
				return;
			}
			int num = 0;
			foreach (HuntingLogTier huntingLogTier in huntingLogProfile.GetTiers())
			{
				HuntingLogUI.InternalUITier internalUITier = this.m_uiTiers[num];
				if (internalUITier != null && num < this.m_uiTiers.Length)
				{
					internalUITier.Count = huntingLogTier.Count;
					if (internalUITier.Total)
					{
						internalUITier.Total.gameObject.SetActive(true);
					}
				}
				num++;
			}
			if (num < this.m_uiTiers.Length)
			{
				for (int i = num; i < this.m_uiTiers.Length; i++)
				{
					if (this.m_uiTiers[i] != null)
					{
						this.m_uiTiers[i].Count = 100000;
						if (this.m_uiTiers[i].Total)
						{
							this.m_uiTiers[i].Total.gameObject.SetActive(false);
						}
					}
				}
			}
		}

		// Token: 0x06005DE6 RID: 24038 RVA: 0x001F56AC File Offset: 0x001F38AC
		private void AlignCurrentTierIndicators()
		{
			if (this.m_uiTiers == null || this.m_uiTiers.Length == 0)
			{
				return;
			}
			if (Application.isPlaying && this.m_selectedEntry == null)
			{
				return;
			}
			List<HuntingLogTierIndicatorUI> activeIndicators = this.m_activeIndicators;
			if (activeIndicators != null)
			{
				activeIndicators.Clear();
			}
			int currentTierCount = Application.isPlaying ? this.GetNextTierCount(this.m_selectedEntry.PerkCount) : this.GetMaxTierCount();
			this.AlignTierIndicators(false, this.m_currentProgressSize, currentTierCount, false, true);
			if (this.m_activeIndicators != null && this.m_activeIndicators.Count > 0)
			{
				this.m_activeIndicators.Reverse();
				float num = 18f;
				for (int i = 0; i < this.m_activeIndicators.Count; i++)
				{
					this.m_activeIndicators[i].SetCountFontSize(num);
					num = Mathf.Clamp(num - this.m_fontDelta, 6f, 18f);
				}
			}
		}

		// Token: 0x06005DE7 RID: 24039 RVA: 0x001F5784 File Offset: 0x001F3984
		private void AlignTierIndicators(bool useTotal, int size, int currentTierCount, bool vertical, bool addToActive)
		{
			if (this.m_uiTiers == null || this.m_uiTiers.Length == 0)
			{
				return;
			}
			for (int i = 0; i < this.m_uiTiers.Length; i++)
			{
				HuntingLogTierIndicatorUI huntingLogTierIndicatorUI = useTotal ? this.m_uiTiers[i].Total : this.m_uiTiers[i].Current;
				bool flag = this.m_uiTiers[i].Count <= currentTierCount;
				huntingLogTierIndicatorUI.gameObject.SetActive(flag);
				if (addToActive && flag)
				{
					List<HuntingLogTierIndicatorUI> activeIndicators = this.m_activeIndicators;
					if (activeIndicators != null)
					{
						activeIndicators.Add(huntingLogTierIndicatorUI);
					}
				}
				float num = (float)this.m_uiTiers[i].Count / (float)currentTierCount;
				float num2 = (float)size * num;
				RectTransform rect = huntingLogTierIndicatorUI.Rect;
				rect.anchoredPosition = (vertical ? new Vector2(rect.anchoredPosition.x, -num2) : new Vector2(num2, rect.anchoredPosition.y));
				huntingLogTierIndicatorUI.InitCount(this.m_uiTiers[i].Count);
			}
		}

		// Token: 0x06005DE8 RID: 24040 RVA: 0x0007F23C File Offset: 0x0007D43C
		internal void ActivateClicked()
		{
			this.m_perkSelection.RefreshPerks(this, this.m_selectedEntry);
		}

		// Token: 0x0400510F RID: 20751
		private const string kTopBar = "Top";

		// Token: 0x04005110 RID: 20752
		private const string kCurrentProgress = "Current Progress";

		// Token: 0x04005111 RID: 20753
		private const string kTotalProgress = "Total Progress";

		// Token: 0x04005112 RID: 20754
		[SerializeField]
		private HuntingLogListUI m_huntingLogList;

		// Token: 0x04005113 RID: 20755
		[SerializeField]
		private HuntingLogPerkSelectionUI m_perkSelection;

		// Token: 0x04005114 RID: 20756
		[SerializeField]
		private GameObject m_rightPanelData;

		// Token: 0x04005115 RID: 20757
		[SerializeField]
		private TextMeshProUGUI m_edgeLabel;

		// Token: 0x04005116 RID: 20758
		[SerializeField]
		private TextMeshProUGUI m_tallyLabel;

		// Token: 0x04005117 RID: 20759
		[FormerlySerializedAs("m_tiers")]
		[FormerlySerializedAs("m_thresholds")]
		[FormerlySerializedAs("m_currentThresholds")]
		[FormerlySerializedAs("m_thresholdElements")]
		[SerializeField]
		private HuntingLogUI.InternalUITier[] m_uiTiers;

		// Token: 0x04005119 RID: 20761
		[SerializeField]
		private Image m_portraitImage;

		// Token: 0x0400511A RID: 20762
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x0400511B RID: 20763
		[SerializeField]
		private TextMeshProUGUI m_totalKillCount;

		// Token: 0x0400511C RID: 20764
		[FormerlySerializedAs("m_size")]
		[SerializeField]
		private int m_currentProgressSize = 332;

		// Token: 0x0400511D RID: 20765
		[SerializeField]
		private float m_fontDelta = 1.5f;

		// Token: 0x0400511E RID: 20766
		[FormerlySerializedAs("m_totalProgressFill")]
		[SerializeField]
		private FilledImage m_currentProgressFill;

		// Token: 0x0400511F RID: 20767
		[SerializeField]
		private int m_totalProgressSize = 370;

		// Token: 0x04005120 RID: 20768
		[SerializeField]
		private FilledImageVertical m_totalProgressFill;

		// Token: 0x04005121 RID: 20769
		private List<HuntingLogTierIndicatorUI> m_activeIndicators;

		// Token: 0x04005122 RID: 20770
		private HuntingLogEntry m_selectedEntry;

		// Token: 0x04005123 RID: 20771
		private const string kDebuffToReplaceText = "Debuff Resists";

		// Token: 0x04005124 RID: 20772
		private static string kDebuffReplacementText = string.Format("{0}{1}", "Debuff"[0], "Resists"[0]);

		// Token: 0x02000BDB RID: 3035
		[Serializable]
		internal class InternalUITier
		{
			// Token: 0x1700162B RID: 5675
			// (get) Token: 0x06005DEB RID: 24043 RVA: 0x0007F2AA File Offset: 0x0007D4AA
			// (set) Token: 0x06005DEC RID: 24044 RVA: 0x0007F2B2 File Offset: 0x0007D4B2
			public int Index { get; set; }

			// Token: 0x04005126 RID: 20774
			[FormerlySerializedAs("Threshold")]
			public int Count;

			// Token: 0x04005127 RID: 20775
			[FormerlySerializedAs("ThresholdElement")]
			public HuntingLogTierIndicatorUI Current;

			// Token: 0x04005128 RID: 20776
			public HuntingLogTierIndicatorUI Total;
		}
	}
}
