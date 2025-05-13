using System;
using Cysharp.Text;
using SoL.Managers;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.HuntingLog
{
	// Token: 0x02000BD5 RID: 3029
	public class HuntingLogListItem : Selectable
	{
		// Token: 0x1700161A RID: 5658
		// (get) Token: 0x06005D8D RID: 23949 RVA: 0x0007EE20 File Offset: 0x0007D020
		public int Index
		{
			get
			{
				return this.m_itemIndex;
			}
		}

		// Token: 0x1700161B RID: 5659
		// (get) Token: 0x06005D8E RID: 23950 RVA: 0x0007EE28 File Offset: 0x0007D028
		public bool IsSelected
		{
			get
			{
				return this.m_itemIndex == this.m_parent.SelectedIndex;
			}
		}

		// Token: 0x1700161C RID: 5660
		// (get) Token: 0x06005D8F RID: 23951 RVA: 0x0007EE3D File Offset: 0x0007D03D
		// (set) Token: 0x06005D90 RID: 23952 RVA: 0x0007EE45 File Offset: 0x0007D045
		public bool Hovered
		{
			get
			{
				return this.m_isHovered;
			}
			private set
			{
				this.m_isHovered = value;
				this.RefreshHighlightVisuals();
			}
		}

		// Token: 0x06005D91 RID: 23953 RVA: 0x0007EE54 File Offset: 0x0007D054
		public void Init(HuntingLogListUI parent, HuntingLogEntry entry, int index)
		{
			this.m_parent = parent;
			this.m_entry = entry;
			this.m_profile = entry.GetProfile();
			this.m_itemIndex = index;
			this.RefreshVisuals();
		}

		// Token: 0x06005D92 RID: 23954 RVA: 0x0007EE7D File Offset: 0x0007D07D
		public void RefreshVisuals()
		{
			this.RefreshLeftLabel();
			this.RefreshCenterLabel();
			this.RefreshRightLabel();
			this.RefreshHighlightVisuals();
		}

		// Token: 0x06005D93 RID: 23955 RVA: 0x0007EE97 File Offset: 0x0007D097
		private void RefreshLeftLabel()
		{
			if (this.m_labelLeft)
			{
				this.m_labelLeft.ZStringSetText(this.m_profile ? this.m_profile.TitlePrefix : "Unknown");
			}
		}

		// Token: 0x06005D94 RID: 23956 RVA: 0x001F4114 File Offset: 0x001F2314
		private void RefreshCenterLabel()
		{
			if (this.m_labelCenter && this.m_profile && this.m_entry != null)
			{
				int perkCount = this.m_entry.PerkCount;
				int num = 0;
				int nextTierCount = this.m_parent.Controller.GetNextTierCount(perkCount);
				foreach (HuntingLogTier huntingLogTier in this.m_profile.GetTiers())
				{
					if (huntingLogTier != null && huntingLogTier.Count <= perkCount && (this.m_entry.ActivePerks == null || !this.m_entry.ActivePerks.ContainsKey(huntingLogTier.Count)))
					{
						num++;
					}
				}
				if (num > 0)
				{
					string arg = (num > 1) ? "Perks" : "Perk";
					this.m_labelCenter.SetTextFormat("<color={0}>{1} Available</color>", UIManager.BlueColor.ToHex(), arg);
					return;
				}
				this.m_labelCenter.SetTextFormat("{0}/{1}", perkCount, nextTierCount);
			}
		}

		// Token: 0x06005D95 RID: 23957 RVA: 0x001F4228 File Offset: 0x001F2428
		private void RefreshRightLabel()
		{
			if (this.m_labelRight && this.m_entry != null)
			{
				ulong totalCount = this.m_entry.TotalCount;
				this.m_labelRight.ZStringSetText(totalCount);
			}
		}

		// Token: 0x06005D96 RID: 23958 RVA: 0x001F4264 File Offset: 0x001F2464
		private void RefreshHighlightVisuals()
		{
			if (this.IsSelected)
			{
				this.m_highlight.gameObject.SetActive(true);
				this.m_highlight.color = this.m_selectedColor;
				return;
			}
			if (this.m_isHovered)
			{
				this.m_highlight.gameObject.SetActive(true);
				this.m_highlight.color = this.m_highlightColor;
				return;
			}
			this.m_highlight.gameObject.SetActive(false);
		}

		// Token: 0x06005D97 RID: 23959 RVA: 0x0007EED0 File Offset: 0x0007D0D0
		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!this.IsSelected)
			{
				this.m_parent.Select(this.m_itemIndex);
			}
		}

		// Token: 0x06005D98 RID: 23960 RVA: 0x0007EEEB File Offset: 0x0007D0EB
		public override void OnPointerEnter(PointerEventData eventData)
		{
			this.Hovered = true;
		}

		// Token: 0x06005D99 RID: 23961 RVA: 0x0007EEF4 File Offset: 0x0007D0F4
		public override void OnPointerExit(PointerEventData eventData)
		{
			this.Hovered = false;
		}

		// Token: 0x040050E3 RID: 20707
		private const string kPerkSpacing = "    ";

		// Token: 0x040050E4 RID: 20708
		[SerializeField]
		private Image m_highlight;

		// Token: 0x040050E5 RID: 20709
		[SerializeField]
		private TextMeshProUGUI m_labelLeft;

		// Token: 0x040050E6 RID: 20710
		[SerializeField]
		private TextMeshProUGUI m_labelCenter;

		// Token: 0x040050E7 RID: 20711
		[SerializeField]
		private TextMeshProUGUI m_labelRight;

		// Token: 0x040050E8 RID: 20712
		private HuntingLogListUI m_parent;

		// Token: 0x040050E9 RID: 20713
		private HuntingLogProfile m_profile;

		// Token: 0x040050EA RID: 20714
		private HuntingLogEntry m_entry;

		// Token: 0x040050EB RID: 20715
		private int m_itemIndex;

		// Token: 0x040050EC RID: 20716
		private Color m_selectedColor = Colors.CadetGrey;

		// Token: 0x040050ED RID: 20717
		private Color m_highlightColor = Colors.GoldMetallic;

		// Token: 0x040050EE RID: 20718
		private bool m_isHovered;
	}
}
