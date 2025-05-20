using System;
using System.Collections.Generic;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI.Social
{
	// Token: 0x0200091B RID: 2331
	public class SocialUI : ResizableWindow
	{
		// Token: 0x17000F66 RID: 3942
		// (get) Token: 0x060044A9 RID: 17577 RVA: 0x0006E693 File Offset: 0x0006C893
		public LfgLfmUI LfgLfmUI
		{
			get
			{
				return this.m_lfgLfmUI;
			}
		}

		// Token: 0x060044AA RID: 17578 RVA: 0x0019D114 File Offset: 0x0019B314
		protected override void Start()
		{
			base.Start();
			this.m_showGMToggle.gameObject.SetActive(false);
			if (LocalPlayer.IsInitialized)
			{
				this.OnLocalPlayerInitialized();
			}
			else
			{
				LocalPlayer.LocalPlayerInitialized += this.OnLocalPlayerInitialized;
			}
			this.m_grouped = ClientGameManager.GroupManager.IsGrouped;
			if (this.m_dualPane)
			{
				this.m_lfgBlocker.SetActive(false);
				this.m_groupTabBlocker.SetActive(false);
				if (this.m_lfgPanel)
				{
					this.m_lfgPanel.SetActive(!this.m_grouped);
				}
				if (this.m_lfmPanel)
				{
					this.m_lfmPanel.SetActive(this.m_grouped);
				}
				this.m_inviteButton.interactable = (!this.m_grouped || ClientGameManager.GroupManager.IsLeader);
				this.m_inviteButtonTooltipRegion.gameObject.SetActive(this.m_grouped && !ClientGameManager.GroupManager.IsLeader);
			}
			else
			{
				this.m_lfgBlocker.SetActive(this.m_grouped);
				this.m_groupTabBlocker.SetActive(!this.m_grouped);
			}
			this.m_nonLeaderBlocker.SetActive(this.m_grouped && !ClientGameManager.GroupManager.IsLeader);
			this.m_inviteButton.onClick.AddListener(new UnityAction(this.OnInviteClicked));
			this.m_tabs.TabChanged += this.OnTabChanged;
			ClientGameManager.GroupManager.RefreshGroup += this.OnGroupUpdate;
		}

		// Token: 0x060044AB RID: 17579 RVA: 0x0019D2A8 File Offset: 0x0019B4A8
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.m_presenceDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.OnPresenceDropdownChanged));
			this.m_showGMToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnShowGMUpdated));
			this.m_inviteButton.onClick.RemoveListener(new UnityAction(this.OnInviteClicked));
			this.m_tabs.TabChanged -= this.OnTabChanged;
			ClientGameManager.GroupManager.RefreshGroup -= this.OnGroupUpdate;
			LocalPlayer.LocalPlayerInitialized -= this.OnLocalPlayerInitialized;
			if (LocalPlayer.GameEntity)
			{
				LocalPlayer.GameEntity.CharacterData.PresenceChanged -= this.UpdatePresence;
			}
		}

		// Token: 0x060044AC RID: 17580 RVA: 0x0019D374 File Offset: 0x0019B574
		public override void Show(bool skipTransition = false)
		{
			base.Show(skipTransition);
			this.UpdatePresence();
			if (this.m_friendsUI.TabContent.activeInHierarchy)
			{
				this.m_friendsUI.OnShow();
			}
			if (this.m_guildUI.TabContent.activeInHierarchy)
			{
				this.m_guildUI.OnShow();
			}
			if (this.m_lfgLfmUI.TabContent.activeInHierarchy)
			{
				this.m_lfgLfmUI.OnShow();
			}
		}

		// Token: 0x060044AD RID: 17581 RVA: 0x0019D3E8 File Offset: 0x0019B5E8
		public override void Hide(bool skipTransition = false)
		{
			base.Hide(skipTransition);
			if (this.m_friendsUI.IsShown)
			{
				this.m_friendsUI.OnHide();
			}
			if (this.m_guildUI.IsShown)
			{
				this.m_guildUI.OnHide();
			}
			if (this.m_lfgLfmUI.IsShown)
			{
				this.m_lfgLfmUI.OnHide();
			}
			this.m_inviteTextInput.Deactivate();
		}

		// Token: 0x060044AE RID: 17582 RVA: 0x0006E69B File Offset: 0x0006C89B
		public void ShowTab(SocialTab tab)
		{
			this.m_tabs.SwitchToTab((int)tab);
			this.Show(false);
		}

		// Token: 0x060044AF RID: 17583 RVA: 0x0006E6B0 File Offset: 0x0006C8B0
		public void ShowTab(FriendsTab tab)
		{
			this.m_tabs.SwitchToTab(0);
			this.m_friendsUI.SwitchToTab(tab);
			this.Show(false);
		}

		// Token: 0x060044B0 RID: 17584 RVA: 0x0019D450 File Offset: 0x0019B650
		private void OnLocalPlayerInitialized()
		{
			int value = 0;
			Presence presence = LocalPlayer.GameEntity.CharacterData.Presence;
			List<string> fromPool = StaticListPool<string>.GetFromPool();
			this.m_presenceDropdown.ClearOptions();
			for (int i = 0; i < this.m_presenceValues.Length; i++)
			{
				fromPool.Add(this.m_presenceValues[i].ToStringWithSpaces());
				if (this.m_presenceValues[i] == presence)
				{
					value = i;
				}
			}
			this.m_presenceDropdown.AddOptions(fromPool);
			this.m_presenceDropdown.value = value;
			StaticListPool<string>.ReturnToPool(fromPool);
			this.m_presenceDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnPresenceDropdownChanged));
			this.m_showGMToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnShowGMUpdated));
			LocalPlayer.GameEntity.CharacterData.PresenceChanged += this.UpdatePresence;
		}

		// Token: 0x060044B1 RID: 17585 RVA: 0x0006E6D1 File Offset: 0x0006C8D1
		private void OnPresenceDropdownChanged(int value)
		{
			LocalPlayer.GameEntity.CharacterData.Presence = this.m_presenceValues[value];
		}

		// Token: 0x060044B2 RID: 17586 RVA: 0x0004475B File Offset: 0x0004295B
		private void OnShowGMUpdated(bool isOn)
		{
		}

		// Token: 0x060044B3 RID: 17587 RVA: 0x0019D52C File Offset: 0x0019B72C
		private void OnTabChanged()
		{
			if (this.m_lfgLfmUI.TabContent.activeInHierarchy)
			{
				if (this.m_friendsUI.IsShown)
				{
					this.m_friendsUI.OnHide();
				}
				if (this.m_guildUI.IsShown)
				{
					this.m_guildUI.OnHide();
				}
				this.m_lfgLfmUI.OnShow();
				return;
			}
			if (this.m_friendsUI.TabContent.activeInHierarchy)
			{
				this.m_friendsUI.OnShow();
				if (this.m_guildUI.IsShown)
				{
					this.m_guildUI.OnHide();
				}
				if (this.m_lfgLfmUI.IsShown)
				{
					this.m_lfgLfmUI.OnHide();
					return;
				}
			}
			else if (this.m_guildUI.TabContent.activeInHierarchy)
			{
				if (this.m_friendsUI.IsShown)
				{
					this.m_friendsUI.OnHide();
				}
				this.m_guildUI.OnShow();
				if (this.m_lfgLfmUI.IsShown)
				{
					this.m_lfgLfmUI.OnHide();
				}
			}
		}

		// Token: 0x060044B4 RID: 17588 RVA: 0x0019D624 File Offset: 0x0019B824
		private void OnGroupUpdate()
		{
			if (!this.m_dualPane)
			{
				if (this.m_grouped != ClientGameManager.GroupManager.IsGrouped)
				{
					this.m_grouped = ClientGameManager.GroupManager.IsGrouped;
					this.m_lfgBlocker.SetActive(this.m_grouped);
					this.m_groupTabBlocker.SetActive(!this.m_grouped);
				}
				this.m_nonLeaderBlocker.SetActive(this.m_grouped && !ClientGameManager.GroupManager.IsLeader);
				return;
			}
			this.m_grouped = ClientGameManager.GroupManager.IsGrouped;
			if (this.m_grouped)
			{
				if (this.m_lfgPanel)
				{
					this.m_lfgPanel.SetActive(false);
				}
				if (this.m_lfmPanel)
				{
					this.m_lfmPanel.SetActive(true);
				}
				this.m_nonLeaderBlocker.SetActive(!ClientGameManager.GroupManager.IsLeader);
				this.m_inviteButton.interactable = ClientGameManager.GroupManager.IsLeader;
				this.m_inviteButtonTooltipRegion.gameObject.SetActive(!ClientGameManager.GroupManager.IsLeader);
				return;
			}
			if (this.m_lfgPanel)
			{
				this.m_lfgPanel.SetActive(true);
			}
			if (this.m_lfmPanel)
			{
				this.m_lfmPanel.SetActive(false);
			}
			this.m_nonLeaderBlocker.SetActive(false);
			this.m_inviteButton.interactable = true;
			this.m_inviteButtonTooltipRegion.gameObject.SetActive(false);
		}

		// Token: 0x060044B5 RID: 17589 RVA: 0x0019D798 File Offset: 0x0019B998
		private void UpdatePresence()
		{
			int valueWithoutNotify = 0;
			Presence presence = LocalPlayer.GameEntity.CharacterData.Presence;
			for (int i = 0; i < this.m_presenceValues.Length; i++)
			{
				if (this.m_presenceValues[i] == presence)
				{
					valueWithoutNotify = i;
				}
			}
			this.m_presenceDropdown.SetValueWithoutNotify(valueWithoutNotify);
			this.m_showGMToggle.SetIsOnWithoutNotify((LocalPlayer.GameEntity.CharacterData.PresenceFlags & PresenceFlags.GM) == PresenceFlags.GM);
		}

		// Token: 0x060044B6 RID: 17590 RVA: 0x0006E6EA File Offset: 0x0006C8EA
		private void OnInviteClicked()
		{
			ClientGameManager.GroupManager.InviteNewMember(this.m_inviteTextInput.text);
			this.m_inviteTextInput.text = string.Empty;
		}

		// Token: 0x04004139 RID: 16697
		[SerializeField]
		private TMP_Dropdown m_presenceDropdown;

		// Token: 0x0400413A RID: 16698
		[SerializeField]
		private SolToggle m_showGMToggle;

		// Token: 0x0400413B RID: 16699
		[SerializeField]
		private TabController m_tabs;

		// Token: 0x0400413C RID: 16700
		[SerializeField]
		private FriendsUI m_friendsUI;

		// Token: 0x0400413D RID: 16701
		[SerializeField]
		private GuildUI m_guildUI;

		// Token: 0x0400413E RID: 16702
		[SerializeField]
		private LfgLfmUI m_lfgLfmUI;

		// Token: 0x0400413F RID: 16703
		[SerializeField]
		private GameObject m_lfgBlocker;

		// Token: 0x04004140 RID: 16704
		[SerializeField]
		private GameObject m_groupTabBlocker;

		// Token: 0x04004141 RID: 16705
		[SerializeField]
		private GameObject m_nonLeaderBlocker;

		// Token: 0x04004142 RID: 16706
		[SerializeField]
		private bool m_dualPane;

		// Token: 0x04004143 RID: 16707
		[SerializeField]
		private GameObject m_lfgPanel;

		// Token: 0x04004144 RID: 16708
		[SerializeField]
		private GameObject m_lfmPanel;

		// Token: 0x04004145 RID: 16709
		[SerializeField]
		private SolButton m_inviteButton;

		// Token: 0x04004146 RID: 16710
		[SerializeField]
		private TextTooltipTrigger m_inviteButtonTooltipRegion;

		// Token: 0x04004147 RID: 16711
		[SerializeField]
		private SolTMP_InputField m_inviteTextInput;

		// Token: 0x04004148 RID: 16712
		private readonly Presence[] m_presenceValues = new Presence[]
		{
			Presence.Online,
			Presence.Away,
			Presence.DoNotDisturb,
			Presence.Anonymous
		};

		// Token: 0x04004149 RID: 16713
		private bool m_grouped;

		// Token: 0x0200091C RID: 2332
		public enum TimePeriod
		{
			// Token: 0x0400414B RID: 16715
			Invalid,
			// Token: 0x0400414C RID: 16716
			Never,
			// Token: 0x0400414D RID: 16717
			LessThanAMinute,
			// Token: 0x0400414E RID: 16718
			FewMinutes,
			// Token: 0x0400414F RID: 16719
			LessThanAnHour,
			// Token: 0x04004150 RID: 16720
			FewHours,
			// Token: 0x04004151 RID: 16721
			EarlierToday,
			// Token: 0x04004152 RID: 16722
			Yesterday,
			// Token: 0x04004153 RID: 16723
			FewDays,
			// Token: 0x04004154 RID: 16724
			ThisYear,
			// Token: 0x04004155 RID: 16725
			Absolute
		}
	}
}
