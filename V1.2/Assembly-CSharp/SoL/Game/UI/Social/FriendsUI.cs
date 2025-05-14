using System;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.Networking.SolServer;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.UI.Social
{
	// Token: 0x020008F9 RID: 2297
	public class FriendsUI : MonoBehaviour
	{
		// Token: 0x17000F44 RID: 3908
		// (get) Token: 0x06004356 RID: 17238 RVA: 0x0006D6E4 File Offset: 0x0006B8E4
		public GameObject TabContent
		{
			get
			{
				return this.m_tabContent;
			}
		}

		// Token: 0x17000F45 RID: 3909
		// (get) Token: 0x06004357 RID: 17239 RVA: 0x0006D6EC File Offset: 0x0006B8EC
		public bool IsShown
		{
			get
			{
				return this.m_isShown;
			}
		}

		// Token: 0x06004358 RID: 17240 RVA: 0x00195CA0 File Offset: 0x00193EA0
		private void Start()
		{
			this.PopulateSortDropdown();
			this.PlayerPrefsKey = this.m_socialUI.PlayerPrefsKey + "_Friends";
			this.m_friendSortDropdown.SetValueWithoutNotify(PlayerPrefs.GetInt(this.PlayerPrefsKey + "_SortFactor", 0));
			this.m_sortDirection = (PlayerPrefs.GetInt(this.PlayerPrefsKey + "_SortDirection", 0) != 0);
			this.m_friendSortDirectionButton.image.sprite = (this.m_sortDirection ? this.m_sortDescendingIcon : this.m_sortAscendingIcon);
			this.m_friendShowOfflineToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt(this.PlayerPrefsKey + "_ShowOFfline", 1) != 0);
			ClientGameManager.SocialManager.FriendsListUpdated += this.OnFriendsChanged;
			ClientGameManager.SocialManager.FriendRequestsUpdated += this.OnFriendRequestsChanged;
			ClientGameManager.SocialManager.BlockListUpdated += this.OnBlockListChanged;
			ClientGameManager.SocialManager.PlayerStatusesChanged += this.OnStatusUpdate;
			this.m_tabs.TabChanged += this.OnTabChanged;
			this.m_addFriendButton.onClick.AddListener(new UnityAction(this.OnAddFriendClicked));
			this.m_blockPlayerButton.onClick.AddListener(new UnityAction(this.OnBlockPlayerClicked));
			this.m_friendSortDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnSortFactorChanged));
			this.m_friendSortDirectionButton.onClick.AddListener(new UnityAction(this.OnSortDirectionClicked));
			this.m_friendShowOfflineToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnShowOfflineChanged));
		}

		// Token: 0x06004359 RID: 17241 RVA: 0x00195E5C File Offset: 0x0019405C
		private void OnDestroy()
		{
			ClientGameManager.SocialManager.FriendsListUpdated -= this.OnFriendsChanged;
			ClientGameManager.SocialManager.FriendRequestsUpdated -= this.OnFriendRequestsChanged;
			ClientGameManager.SocialManager.BlockListUpdated -= this.OnBlockListChanged;
			ClientGameManager.SocialManager.PlayerStatusesChanged -= this.OnStatusUpdate;
			this.m_tabs.TabChanged -= this.OnTabChanged;
			this.m_addFriendButton.onClick.RemoveAllListeners();
			this.m_blockPlayerButton.onClick.RemoveAllListeners();
			this.m_friendSortDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.OnSortFactorChanged));
			this.m_friendSortDirectionButton.onClick.RemoveListener(new UnityAction(this.OnSortDirectionClicked));
			this.m_friendShowOfflineToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnShowOfflineChanged));
			this.m_friendsListUI.Initialized -= this.UpdateFriends;
			this.m_pendingListUI.Initialized -= this.UpdatePending;
			this.m_blockListUI.Initialized -= this.UpdateBlockList;
		}

		// Token: 0x0600435A RID: 17242 RVA: 0x00195F94 File Offset: 0x00194194
		private void Update()
		{
			if (this.m_isShown)
			{
				DateTime utcNow = DateTime.UtcNow;
				if (utcNow - this.m_lastListUpdate > this.m_listUpdateInterval)
				{
					this.UpdateLists();
					this.m_lastListUpdate = utcNow;
				}
			}
		}

		// Token: 0x0600435B RID: 17243 RVA: 0x0006D6F4 File Offset: 0x0006B8F4
		public void OnShow()
		{
			this.m_isShown = true;
			this.UpdateLists();
			this.m_lastListUpdate = DateTime.UtcNow;
		}

		// Token: 0x0600435C RID: 17244 RVA: 0x0006D70E File Offset: 0x0006B90E
		public void OnHide()
		{
			this.m_isShown = false;
		}

		// Token: 0x0600435D RID: 17245 RVA: 0x00195FD8 File Offset: 0x001941D8
		private void PopulateSortDropdown()
		{
			List<string> fromPool = StaticListPool<string>.GetFromPool();
			foreach (object obj in Enum.GetValues(typeof(FriendsUI.FriendSortFactor)))
			{
				FriendsUI.FriendSortFactor friendSortFactor = (FriendsUI.FriendSortFactor)obj;
				fromPool.Add(friendSortFactor.ToStringWithSpaces());
			}
			this.m_friendSortDropdown.AddOptions(fromPool);
			StaticListPool<string>.ReturnToPool(fromPool);
		}

		// Token: 0x0600435E RID: 17246 RVA: 0x0006D717 File Offset: 0x0006B917
		public void SwitchToTab(FriendsTab tab)
		{
			this.m_tabs.SwitchToTab((int)tab);
		}

		// Token: 0x0600435F RID: 17247 RVA: 0x0019605C File Offset: 0x0019425C
		private void UpdateLists()
		{
			if (this.m_friendsListUI.IsInitialized)
			{
				this.UpdateFriends();
			}
			else
			{
				this.m_friendsListUI.Initialized += this.UpdateFriends;
			}
			if (this.m_pendingListUI.IsInitialized)
			{
				this.UpdatePending();
			}
			else
			{
				this.m_pendingListUI.Initialized += this.UpdatePending;
			}
			if (this.m_blockListUI.IsInitialized)
			{
				this.UpdateBlockList();
				return;
			}
			this.m_blockListUI.Initialized += this.UpdateBlockList;
		}

		// Token: 0x06004360 RID: 17248 RVA: 0x001960EC File Offset: 0x001942EC
		private void UpdateFriends()
		{
			List<Relation> fromPool = StaticListPool<Relation>.GetFromPool();
			fromPool.AddRange(ClientGameManager.SocialManager.Friends.Values);
			if (!this.m_friendShowOfflineToggle.isOn)
			{
				fromPool.RemoveAll(delegate(Relation x)
				{
					PlayerStatus playerStatus;
					return ClientGameManager.SocialManager.TryGetLatestStatus(x.OtherName, out playerStatus) && (playerStatus.ZoneId == -1 || playerStatus.PresenceFlags.HasBitFlag(PresenceFlags.Invisible));
				});
			}
			fromPool.Sort(delegate(Relation x, Relation y)
			{
				int num = 0;
				switch (this.m_friendSortDropdown.value)
				{
				case 0:
					num = x.OtherName.CompareTo(y.OtherName);
					break;
				case 1:
				{
					PlayerStatus playerStatus;
					PlayerStatus playerStatus2;
					if (ClientGameManager.SocialManager.TryGetLatestStatus(x.OtherName, out playerStatus) && ClientGameManager.SocialManager.TryGetLatestStatus(y.OtherName, out playerStatus2))
					{
						BaseArchetype roleFromPacked = GlobalSettings.Values.Roles.GetRoleFromPacked(playerStatus.RolePacked);
						if (roleFromPacked != null)
						{
							BaseArchetype roleFromPacked2 = GlobalSettings.Values.Roles.GetRoleFromPacked(playerStatus2.RolePacked);
							if (roleFromPacked2 != null)
							{
								num = roleFromPacked.DisplayName.CompareTo(roleFromPacked2.DisplayName);
							}
						}
					}
					break;
				}
				case 2:
				{
					PlayerStatus playerStatus3 = null;
					PlayerStatus playerStatus4 = null;
					ClientGameManager.SocialManager.TryGetLatestStatus(x.OtherName, out playerStatus3);
					ClientGameManager.SocialManager.TryGetLatestStatus(y.OtherName, out playerStatus4);
					byte? b = (playerStatus3 != null) ? new byte?(playerStatus3.Level) : null;
					float num2 = (b != null) ? ((float)b.GetValueOrDefault()) : 50f;
					byte? b2 = (playerStatus4 != null) ? new byte?(playerStatus4.Level) : null;
					num = num2.CompareTo((b2 != null) ? ((float)b2.GetValueOrDefault()) : 50f);
					break;
				}
				case 3:
				{
					int num3;
					byte subZoneId;
					ClientGameManager.SocialManager.TryGetLatestZone(x.OtherName, out num3, out subZoneId);
					int num4;
					byte subZoneId2;
					ClientGameManager.SocialManager.TryGetLatestZone(y.OtherName, out num4, out subZoneId2);
					ZoneRecord zoneRecord = SessionData.GetZoneRecord((ZoneId)num3);
					ZoneRecord zoneRecord2 = SessionData.GetZoneRecord((ZoneId)num4);
					string text = string.Empty;
					string text2 = string.Empty;
					if (zoneRecord != null)
					{
						text = LocalZoneManager.GetFormattedZoneName(zoneRecord.DisplayName, (SubZoneId)subZoneId);
					}
					else if (num3 == 0)
					{
						text = "zzzzzzzzzzzzzzzzzzzzzzzzzzzz";
					}
					else if (num3 == -1)
					{
						text = "zzzzzzzzzzzzzzzzzzzzzzzzzzzy";
					}
					else if (num3 == -2)
					{
						text = "zzzzzzzzzzzzzzzzzzzzzzzzzzzx";
					}
					else
					{
						text = "zzzzzzzzzzzzzzzzzzzzzzzzzzzz";
					}
					if (zoneRecord2 != null)
					{
						text2 = LocalZoneManager.GetFormattedZoneName(zoneRecord2.DisplayName, (SubZoneId)subZoneId2);
					}
					else if (num4 == 0)
					{
						text2 = "zzzzzzzzzzzzzzzzzzzzzzzzzzzz";
					}
					else if (num4 == -1)
					{
						text2 = "zzzzzzzzzzzzzzzzzzzzzzzzzzzy";
					}
					else if (num4 == -2)
					{
						text2 = "zzzzzzzzzzzzzzzzzzzzzzzzzzzx";
					}
					else
					{
						text2 = "zzzzzzzzzzzzzzzzzzzzzzzzzzzz";
					}
					num = text.ToLower().CompareTo(text2.ToLower());
					break;
				}
				case 4:
				{
					PlayerStatus playerStatus5 = null;
					PlayerStatus playerStatus6 = null;
					ClientGameManager.SocialManager.TryGetLatestStatus(x.OtherName, out playerStatus5);
					ClientGameManager.SocialManager.TryGetLatestStatus(y.OtherName, out playerStatus6);
					num = (((playerStatus5 != null) ? playerStatus5.LastOnline : null) ?? DateTime.MinValue).CompareTo(((playerStatus6 != null) ? playerStatus6.LastOnline : null) ?? DateTime.MinValue);
					break;
				}
				}
				if (this.m_sortDirection)
				{
					num *= -1;
				}
				return num;
			});
			this.m_friendsListUI.UpdateItems(fromPool);
			this.m_friendsListUI.Reindex();
			StaticListPool<Relation>.ReturnToPool(fromPool);
		}

		// Token: 0x06004361 RID: 17249 RVA: 0x00196178 File Offset: 0x00194378
		private void UpdatePending()
		{
			List<Mail> fromPool = StaticListPool<Mail>.GetFromPool();
			fromPool.AddRange(ClientGameManager.SocialManager.PendingIncomingFriendRequests.Values);
			fromPool.AddRange(ClientGameManager.SocialManager.PendingOutgoingFriendRequests.Values);
			fromPool.Sort((Mail x, Mail y) => x.Created.CompareTo(y.Created));
			this.m_pendingListUI.UpdateItems(fromPool);
			this.m_pendingListUI.Reindex();
			StaticListPool<Mail>.ReturnToPool(fromPool);
		}

		// Token: 0x06004362 RID: 17250 RVA: 0x001961F8 File Offset: 0x001943F8
		private void UpdateBlockList()
		{
			List<Relation> fromPool = StaticListPool<Relation>.GetFromPool();
			fromPool.AddRange(ClientGameManager.SocialManager.BlockList.Values);
			fromPool.Sort((Relation x, Relation y) => x.OtherName.CompareTo(y.OtherName));
			this.m_blockListUI.UpdateItems(fromPool);
			this.m_blockListUI.Reindex();
			StaticListPool<Relation>.ReturnToPool(fromPool);
		}

		// Token: 0x06004363 RID: 17251 RVA: 0x0006D725 File Offset: 0x0006B925
		private void OnFriendsChanged()
		{
			if (this.m_friendsListUI.IsInitialized)
			{
				this.UpdateFriends();
			}
		}

		// Token: 0x06004364 RID: 17252 RVA: 0x0006D73A File Offset: 0x0006B93A
		private void OnFriendRequestsChanged()
		{
			if (this.m_pendingListUI.IsInitialized)
			{
				this.UpdatePending();
			}
		}

		// Token: 0x06004365 RID: 17253 RVA: 0x0006D74F File Offset: 0x0006B94F
		private void OnBlockListChanged()
		{
			if (this.m_blockListUI.IsInitialized)
			{
				this.UpdateBlockList();
			}
		}

		// Token: 0x06004366 RID: 17254 RVA: 0x0006D725 File Offset: 0x0006B925
		private void OnStatusUpdate()
		{
			if (this.m_friendsListUI.IsInitialized)
			{
				this.UpdateFriends();
			}
		}

		// Token: 0x06004367 RID: 17255 RVA: 0x0006D764 File Offset: 0x0006B964
		private void OnTabChanged()
		{
			this.OnShow();
		}

		// Token: 0x06004368 RID: 17256 RVA: 0x0006D76C File Offset: 0x0006B96C
		private void OnAddFriendClicked()
		{
			ClientGameManager.SocialManager.Friend(this.m_addFriendTextInput.text);
			this.m_addFriendTextInput.text = string.Empty;
		}

		// Token: 0x06004369 RID: 17257 RVA: 0x0006D793 File Offset: 0x0006B993
		private void OnBlockPlayerClicked()
		{
			ClientGameManager.SocialManager.Block(this.m_blockPlayerTextInput.text);
			this.m_blockPlayerTextInput.text = string.Empty;
		}

		// Token: 0x0600436A RID: 17258 RVA: 0x00196264 File Offset: 0x00194464
		private void OnSortFactorChanged(int index)
		{
			if (this.m_friendsListUI.IsInitialized)
			{
				this.UpdateFriends();
			}
			else
			{
				this.m_friendsListUI.Initialized += this.UpdateFriends;
			}
			PlayerPrefs.SetInt(this.PlayerPrefsKey + "_SortFactor", index);
		}

		// Token: 0x0600436B RID: 17259 RVA: 0x001962B4 File Offset: 0x001944B4
		private void OnSortDirectionClicked()
		{
			this.m_friendSortDirectionButton.image.sprite = (this.m_sortDirection ? this.m_sortAscendingIcon : this.m_sortDescendingIcon);
			this.m_sortDirection = !this.m_sortDirection;
			if (this.m_friendsListUI.IsInitialized)
			{
				this.UpdateFriends();
			}
			else
			{
				this.m_friendsListUI.Initialized += this.UpdateFriends;
			}
			PlayerPrefs.SetInt(this.PlayerPrefsKey + "_SortDirection", this.m_sortDirection ? 1 : 0);
		}

		// Token: 0x0600436C RID: 17260 RVA: 0x00196344 File Offset: 0x00194544
		private void OnShowOfflineChanged(bool value)
		{
			if (this.m_friendsListUI.IsInitialized)
			{
				this.UpdateFriends();
			}
			else
			{
				this.m_friendsListUI.Initialized += this.UpdateFriends;
			}
			PlayerPrefs.SetInt(this.PlayerPrefsKey + "_ShowOFfline", value ? 1 : 0);
		}

		// Token: 0x04003FF9 RID: 16377
		private readonly TimeSpan m_listUpdateInterval = TimeSpan.FromSeconds(10.0);

		// Token: 0x04003FFA RID: 16378
		[SerializeField]
		private SocialUI m_socialUI;

		// Token: 0x04003FFB RID: 16379
		[SerializeField]
		private GameObject m_tabContent;

		// Token: 0x04003FFC RID: 16380
		[SerializeField]
		private FriendsList m_friendsListUI;

		// Token: 0x04003FFD RID: 16381
		[SerializeField]
		private PendingFriendsList m_pendingListUI;

		// Token: 0x04003FFE RID: 16382
		[SerializeField]
		private BlockList m_blockListUI;

		// Token: 0x04003FFF RID: 16383
		[SerializeField]
		private TMP_Dropdown m_friendSortDropdown;

		// Token: 0x04004000 RID: 16384
		[SerializeField]
		private SolButton m_friendSortDirectionButton;

		// Token: 0x04004001 RID: 16385
		[SerializeField]
		private Sprite m_sortAscendingIcon;

		// Token: 0x04004002 RID: 16386
		[SerializeField]
		private Sprite m_sortDescendingIcon;

		// Token: 0x04004003 RID: 16387
		[SerializeField]
		private SolToggle m_friendShowOfflineToggle;

		// Token: 0x04004004 RID: 16388
		[SerializeField]
		private TabController m_tabs;

		// Token: 0x04004005 RID: 16389
		[SerializeField]
		private TMP_InputField m_addFriendTextInput;

		// Token: 0x04004006 RID: 16390
		[SerializeField]
		private SolButton m_addFriendButton;

		// Token: 0x04004007 RID: 16391
		[SerializeField]
		private TMP_InputField m_blockPlayerTextInput;

		// Token: 0x04004008 RID: 16392
		[SerializeField]
		private SolButton m_blockPlayerButton;

		// Token: 0x04004009 RID: 16393
		[NonSerialized]
		public string PlayerPrefsKey;

		// Token: 0x0400400A RID: 16394
		private bool m_sortDirection;

		// Token: 0x0400400B RID: 16395
		private bool m_isShown;

		// Token: 0x0400400C RID: 16396
		private DateTime m_lastListUpdate = DateTime.UtcNow;

		// Token: 0x020008FA RID: 2298
		private enum FriendSortFactor
		{
			// Token: 0x0400400E RID: 16398
			CharacterName,
			// Token: 0x0400400F RID: 16399
			Role,
			// Token: 0x04004010 RID: 16400
			Level,
			// Token: 0x04004011 RID: 16401
			Zone,
			// Token: 0x04004012 RID: 16402
			LastOnline
		}
	}
}
