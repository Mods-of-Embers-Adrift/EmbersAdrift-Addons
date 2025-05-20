using System;
using System.Collections.Generic;
using System.Linq;
using SoL.Game.Grouping;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI.Social
{
	// Token: 0x0200090C RID: 2316
	public class LfgLfmUI : MonoBehaviour
	{
		// Token: 0x17000F58 RID: 3928
		// (get) Token: 0x0600440C RID: 17420 RVA: 0x0006DFAA File Offset: 0x0006C1AA
		// (set) Token: 0x0600440D RID: 17421 RVA: 0x00199450 File Offset: 0x00197650
		private LfgLfmUI.FindFilterModes FindFilterMode
		{
			get
			{
				return this.m_findFilterMode;
			}
			set
			{
				if (this.m_findFilterMode == value)
				{
					return;
				}
				this.m_findFilterMode = value;
				this.m_preventFindFilterToggleRefresh = true;
				switch (this.m_findFilterMode)
				{
				default:
					this.m_listAllToggle.isOn = true;
					this.m_listAllToggle.interactable = false;
					this.m_listIndividualsToggle.isOn = false;
					this.m_listIndividualsToggle.interactable = true;
					this.m_listGroupsToggle.isOn = false;
					this.m_listGroupsToggle.interactable = true;
					break;
				case LfgLfmUI.FindFilterModes.Individual:
					this.m_listAllToggle.isOn = false;
					this.m_listAllToggle.interactable = true;
					this.m_listIndividualsToggle.isOn = true;
					this.m_listIndividualsToggle.interactable = false;
					this.m_listGroupsToggle.isOn = false;
					this.m_listGroupsToggle.interactable = true;
					break;
				case LfgLfmUI.FindFilterModes.Group:
					this.m_listAllToggle.isOn = false;
					this.m_listAllToggle.interactable = true;
					this.m_listIndividualsToggle.isOn = false;
					this.m_listIndividualsToggle.interactable = true;
					this.m_listGroupsToggle.isOn = true;
					this.m_listGroupsToggle.interactable = false;
					break;
				}
				this.m_preventFindFilterToggleRefresh = false;
				if (this.m_listUI.IsInitialized)
				{
					this.RefreshListFiltering();
					return;
				}
				this.m_listUI.Initialized += this.RefreshListFiltering;
			}
		}

		// Token: 0x17000F59 RID: 3929
		// (get) Token: 0x0600440E RID: 17422 RVA: 0x0006DFB2 File Offset: 0x0006C1B2
		public GameObject TabContent
		{
			get
			{
				return this.m_tabContent;
			}
		}

		// Token: 0x17000F5A RID: 3930
		// (get) Token: 0x0600440F RID: 17423 RVA: 0x0006DFBA File Offset: 0x0006C1BA
		public bool IsShown
		{
			get
			{
				return this.m_isShown;
			}
		}

		// Token: 0x06004410 RID: 17424 RVA: 0x001995A4 File Offset: 0x001977A4
		private void Awake()
		{
			this.m_lfmToggle.interactable = (ClientGameManager.GroupManager.IsGrouped && ClientGameManager.GroupManager.IsLeader);
			this.PopulateZones();
			List<string> options = (from x in this.m_dropDownZones
			select x.DisplayName).ToList<string>();
			this.m_lfgZoneDropdown.AddOptions(options);
			this.m_lfmZoneDropdown.AddOptions(options);
			this.m_listZoneDropdown.AddOptions(options);
			this.PopulateLevelDropdown(this.m_lfgLevelDropdown);
			this.PopulateLevelDropdown(this.m_lfmLevelDropdown);
			this.PopulateLevelDropdown(this.m_listLevelDropdown);
			this.m_lfmLevelMin.SetTextWithoutNotify("1");
			this.m_lfmLevelMax.SetTextWithoutNotify(this.MaxLevelAsString);
			this.m_lfgToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnLfg));
			this.m_lfgTagsButton.onClick.AddListener(new UnityAction(this.OnLfgTagsButtonClicked));
			this.m_lfgNoteButton.onClick.AddListener(new UnityAction(this.OnLfgNoteButtonClicked));
			this.m_lfmToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnLfm));
			this.m_lfmLevelMin.onEndEdit.AddListener(new UnityAction<string>(this.OnLfmLevelMinChanged));
			this.m_lfmLevelMax.onEndEdit.AddListener(new UnityAction<string>(this.OnLfmLevelMaxChanged));
			this.m_lfmTagsButton.onClick.AddListener(new UnityAction(this.OnLfmTagsButtonClicked));
			this.m_lfmNoteButton.onClick.AddListener(new UnityAction(this.OnLfmNoteButtonClicked));
			this.m_listAllToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnListAllToggleChanged));
			this.m_listIndividualsToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnListIndividualChanged));
			this.m_listGroupsToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnListGroupChanged));
			this.m_listLevelDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnListLevelChanged));
			this.m_listTagsButton.onClick.AddListener(new UnityAction(this.OnListTagsButtonClicked));
			this.m_refreshButton.onClick.AddListener(new UnityAction(this.RequestRefresh));
			this.m_tagsViewOkButton.onClick.AddListener(new UnityAction(this.OnTagsViewOkButtonClicked));
			this.m_tagsViewCancelButton.onClick.AddListener(new UnityAction(this.OnTagsViewCancelButtonClicked));
			this.m_tagsViewAllButton.onClick.AddListener(new UnityAction(this.OnTagsViewAllButtonClicked));
			this.m_tagsViewNoneButton.onClick.AddListener(new UnityAction(this.OnTagsViewNoneButtonClicked));
		}

		// Token: 0x06004411 RID: 17425 RVA: 0x0019986C File Offset: 0x00197A6C
		private void Start()
		{
			this.m_list_tags = LookingTags.All;
			this.m_listTagsButton.text = this.m_list_tags.ToString();
			this.UpdateOwnEntry(ClientGameManager.SocialManager.OwnLfgLfmEntry);
			this.RefreshSelfShelf();
			this.m_lfgNoteButton.text = (string.IsNullOrEmpty(this.m_lfg_note) ? "[Empty]" : this.m_lfg_note);
			this.m_lfmNoteButton.text = (string.IsNullOrEmpty(this.m_lfm_note) ? "[Empty]" : this.m_lfm_note);
			if (SessionData.IsTrial)
			{
				this.m_lfgNoteButton.text = "[Empty] (Purchase Required)";
				this.m_lfgNoteButton.interactable = false;
				this.m_lfmNoteButton.text = "[Empty] (Purchase Required)";
				this.m_lfmNoteButton.interactable = false;
			}
			ClientGameManager.GroupManager.RefreshGroup += this.OnGroupUpdate;
			ClientGameManager.SocialManager.LookingUpdated += this.OnLookingUpdated;
			ClientGameManager.SocialManager.LookingListUpdated += this.RefreshListFiltering;
			if (LocalPlayer.IsInitialized)
			{
				this.Init();
				return;
			}
			LocalPlayer.LocalPlayerInitialized += this.OnLocalPlayerInitialized;
		}

		// Token: 0x06004412 RID: 17426 RVA: 0x001999A0 File Offset: 0x00197BA0
		private void OnDestroy()
		{
			this.m_lfgToggle.onValueChanged.RemoveAllListeners();
			this.m_lfgZoneDropdown.onValueChanged.RemoveAllListeners();
			this.m_lfgLevelDropdown.onValueChanged.RemoveAllListeners();
			this.m_lfgTagsButton.onClick.RemoveAllListeners();
			this.m_lfgNoteButton.onClick.RemoveAllListeners();
			this.m_lfmToggle.onValueChanged.RemoveAllListeners();
			this.m_lfmZoneDropdown.onValueChanged.RemoveAllListeners();
			this.m_lfmLevelDropdown.onValueChanged.RemoveAllListeners();
			this.m_lfmLevelMin.onValueChanged.RemoveAllListeners();
			this.m_lfmLevelMax.onValueChanged.RemoveAllListeners();
			this.m_lfmTagsButton.onClick.RemoveAllListeners();
			this.m_lfmNoteButton.onClick.RemoveAllListeners();
			this.m_listAllToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnListAllToggleChanged));
			this.m_listIndividualsToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnListIndividualChanged));
			this.m_listGroupsToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnListGroupChanged));
			this.m_listZoneDropdown.onValueChanged.RemoveAllListeners();
			this.m_listLevelDropdown.onValueChanged.RemoveAllListeners();
			this.m_listTagsButton.onClick.RemoveAllListeners();
			this.m_refreshButton.onClick.RemoveAllListeners();
			this.m_tagsViewOkButton.onClick.RemoveAllListeners();
			this.m_tagsViewCancelButton.onClick.RemoveAllListeners();
			this.m_tagsViewAllButton.onClick.RemoveAllListeners();
			this.m_tagsViewNoneButton.onClick.RemoveAllListeners();
			if (ClientGameManager.GroupManager)
			{
				ClientGameManager.GroupManager.RefreshGroup -= this.OnGroupUpdate;
			}
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Masteries != null)
			{
				LocalPlayer.GameEntity.CollectionController.Masteries.ContentsChanged -= this.OnMasteriesChanged;
			}
			if (ClientGameManager.SocialManager)
			{
				ClientGameManager.SocialManager.LookingUpdated -= this.OnLookingUpdated;
				ClientGameManager.SocialManager.LookingListUpdated -= this.RefreshListFiltering;
			}
			this.m_listUI.Initialized -= this.RefreshListFiltering;
		}

		// Token: 0x06004413 RID: 17427 RVA: 0x0006DFC2 File Offset: 0x0006C1C2
		private void Update()
		{
			if (this.m_isShown)
			{
				ClientGameManager.SocialManager.CheckLooking(false);
			}
		}

		// Token: 0x06004414 RID: 17428 RVA: 0x0006DFD7 File Offset: 0x0006C1D7
		private void OnLocalPlayerInitialized()
		{
			LocalPlayer.LocalPlayerInitialized -= this.OnLocalPlayerInitialized;
			this.Init();
		}

		// Token: 0x06004415 RID: 17429 RVA: 0x00199BFC File Offset: 0x00197DFC
		private void Init()
		{
			this.m_lfg_tags = this.TagsFromMasteries();
			this.m_lfgTagsButton.text = this.m_lfg_tags.ToString();
			this.m_lfmTagsButton.text = LookingTags.None.ToString();
			this.m_lfg_maxLevel = this.MaxLevelFromCurrent();
			this.m_lfg_minLevel = this.m_lfg_maxLevel;
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Masteries != null)
			{
				LocalPlayer.GameEntity.CollectionController.Masteries.ContentsChanged += this.OnMasteriesChanged;
			}
		}

		// Token: 0x06004416 RID: 17430 RVA: 0x0006DFF0 File Offset: 0x0006C1F0
		public void OnShow()
		{
			this.m_isShown = true;
			ClientGameManager.SocialManager.CheckLooking(false);
			if (this.FindFilterMode == LfgLfmUI.FindFilterModes.None)
			{
				this.FindFilterMode = LfgLfmUI.FindFilterModes.All;
			}
		}

		// Token: 0x06004417 RID: 17431 RVA: 0x0006E013 File Offset: 0x0006C213
		public void OnHide()
		{
			this.m_isShown = false;
			this.m_lfmLevelMin.Deactivate();
			this.m_lfmLevelMax.Deactivate();
		}

		// Token: 0x06004418 RID: 17432 RVA: 0x0006E032 File Offset: 0x0006C232
		public void RequestRefresh()
		{
			ClientGameManager.SocialManager.CheckLooking(true);
		}

		// Token: 0x06004419 RID: 17433 RVA: 0x00199CAC File Offset: 0x00197EAC
		private void RefreshListFiltering()
		{
			Dictionary<string, LookingFor> lookingList = ClientGameManager.SocialManager.LookingList;
			LookingFor[] array = (lookingList != null) ? this.FilterList(lookingList.Values).ToArray() : LfgLfmUI.m_emptyLookingList;
			int num = (lookingList != null) ? lookingList.Count : 0;
			bool flag = lookingList.ContainsKey(LocalPlayer.GameEntity.CharacterData.CharacterId) || (ClientGameManager.GroupManager.IsGrouped && lookingList.ContainsKey(ClientGameManager.GroupManager.GroupId));
			this.m_listOverfilteredBlocker.SetActive(array.Length == 0 && num > 0 && (!flag || num != 1));
			this.m_listUI.UpdateItems(array);
			this.m_listUI.Reindex();
		}

		// Token: 0x0600441A RID: 17434 RVA: 0x00199D6C File Offset: 0x00197F6C
		private List<LookingFor> FilterList(ICollection<LookingFor> unfilteredList)
		{
			LfgLfmUI.m_filteredList.Clear();
			this.m_lfmToggle.isOn = ClientGameManager.SocialManager.IsGroupLookingForMember();
			foreach (LookingFor lookingFor in unfilteredList)
			{
				bool flag = lookingFor.Key == LocalPlayer.GameEntity.CharacterData.CharacterId || (ClientGameManager.GroupManager.IsGrouped && lookingFor.Key == ClientGameManager.GroupManager.GroupId);
				bool flag2 = !flag && (this.m_listLevelDropdown.value == 0 || (lookingFor.MaxLevel <= this.m_list_maxLevel && lookingFor.MinLevel >= this.m_list_minLevel)) && (lookingFor.Tags == LookingTags.None || (lookingFor.Tags & this.m_list_tags) > LookingTags.None);
				if ((this.FindFilterMode == LfgLfmUI.FindFilterModes.Individual && lookingFor.Type != LookingType.Lfg) || (this.FindFilterMode == LfgLfmUI.FindFilterModes.Group && lookingFor.Type != LookingType.Lfm))
				{
					flag2 = false;
				}
				if (flag)
				{
					this.UpdateOwnEntry(lookingFor);
				}
				if (flag2)
				{
					LfgLfmUI.m_filteredList.Add(lookingFor);
				}
			}
			return LfgLfmUI.m_filteredList;
		}

		// Token: 0x0600441B RID: 17435 RVA: 0x00199EAC File Offset: 0x001980AC
		private void UpdateOwnEntry(LookingFor entry)
		{
			if (entry != null)
			{
				if (entry.Type == LookingType.Lfg)
				{
					this.m_lfgToggle.SetIsOnWithoutNotify(true);
					this.m_lfg_minLevel = entry.MinLevel;
					this.m_lfg_maxLevel = entry.MaxLevel;
					this.m_lfg_tags = entry.Tags;
					string note = entry.Note;
					this.m_lfg_note = ((note != null) ? note.Replace("<noparse>", "").Replace("</noparse>", "") : null);
					this.m_lfgLevelDropdown.value = ((entry.MinLevel == entry.MaxLevel) ? 0 : (entry.MaxLevel / 5));
					this.m_lfgTagsButton.text = entry.Tags.ToStringWithSpaces();
					return;
				}
				if (entry.Type == LookingType.Lfm)
				{
					this.m_lfmToggle.SetIsOnWithoutNotify(true);
					this.m_lfm_minLevel = entry.MinLevel;
					this.m_lfm_maxLevel = entry.MaxLevel;
					this.m_lfm_tags = entry.Tags;
					string note2 = entry.Note;
					this.m_lfm_note = ((note2 != null) ? note2.Replace("<noparse>", "").Replace("</noparse>", "") : null);
					this.m_lfmLevelDropdown.value = ((entry.MaxLevel == 0) ? 0 : (entry.MaxLevel / 5));
					this.m_lfmTagsButton.text = entry.Tags.ToStringWithSpaces();
					return;
				}
			}
			else
			{
				this.m_lfgToggle.SetIsOnWithoutNotify(false);
				this.m_lfmToggle.SetIsOnWithoutNotify(false);
			}
		}

		// Token: 0x0600441C RID: 17436 RVA: 0x0019A02C File Offset: 0x0019822C
		private void OnGroupUpdate()
		{
			this.m_lfgToggle.SetIsOnWithoutNotify(ClientGameManager.SocialManager.IsLookingForGroup());
			this.m_lfmToggle.SetIsOnWithoutNotify(ClientGameManager.SocialManager.IsGroupLookingForMember());
			this.m_lfgToggle.interactable = !ClientGameManager.GroupManager.IsGrouped;
			this.m_lfmToggle.interactable = (ClientGameManager.GroupManager.IsGrouped && ClientGameManager.GroupManager.IsLeader);
			this.RequestRefresh();
		}

		// Token: 0x0600441D RID: 17437 RVA: 0x0019A0A8 File Offset: 0x001982A8
		private void OnMasteriesChanged()
		{
			LookingTags lookingTags = this.TagsFromMasteries();
			if (((this.m_lfg_tags & ~(LookingTags.Defender | LookingTags.Supporter | LookingTags.Striker)) | lookingTags) == this.m_lfg_tags)
			{
				return;
			}
			this.m_lfg_tags = ((this.m_lfg_tags & ~(LookingTags.Defender | LookingTags.Supporter | LookingTags.Striker)) | lookingTags);
			this.m_lfgTagsButton.text = this.m_lfg_tags.ToStringWithSpaces();
			if (this.m_tagsView.activeInHierarchy && this.m_tagsViewMode == LfgLfmUI.TagsViewMode.Lfg)
			{
				foreach (LfgLfmUI.TagToggle tagToggle in this.m_tagsViewToggles)
				{
					if (!tagToggle.Toggle.interactable)
					{
						tagToggle.Toggle.isOn = ((this.m_lfg_tags & tagToggle.Tags) > LookingTags.None);
					}
				}
			}
			if (ClientGameManager.SocialManager.IsLookingForGroup())
			{
				ClientGameManager.SocialManager.OwnLfgLfmEntry.Tags = this.m_lfg_tags;
				ClientGameManager.SocialManager.StartOrUpdateLooking(null);
			}
		}

		// Token: 0x0600441E RID: 17438 RVA: 0x0006E03F File Offset: 0x0006C23F
		private void OnLfg(bool isLooking)
		{
			if (isLooking)
			{
				this.StartOrUpdateLooking();
				return;
			}
			ClientGameManager.SocialManager.StopLooking(true);
		}

		// Token: 0x0600441F RID: 17439 RVA: 0x0006E03F File Offset: 0x0006C23F
		private void OnLfm(bool isLooking)
		{
			if (isLooking)
			{
				this.StartOrUpdateLooking();
				return;
			}
			ClientGameManager.SocialManager.StopLooking(true);
		}

		// Token: 0x06004420 RID: 17440 RVA: 0x0006E056 File Offset: 0x0006C256
		private void OnLookingUpdated()
		{
			this.UpdateOwnEntry(ClientGameManager.SocialManager.OwnLfgLfmEntry);
			this.RefreshSelfShelf();
		}

		// Token: 0x06004421 RID: 17441 RVA: 0x0019A1A8 File Offset: 0x001983A8
		private void StartOrUpdateLooking()
		{
			bool isGrouped = ClientGameManager.GroupManager.IsGrouped;
			ClientGameManager.SocialManager.StartOrUpdateLooking(new LookingFor
			{
				Key = (isGrouped ? ClientGameManager.GroupManager.GroupId : LocalPlayer.GameEntity.CharacterData.CharacterId),
				ContactName = LocalPlayer.GameEntity.CharacterData.Name,
				MinLevel = (isGrouped ? this.m_lfm_minLevel : this.MaxLevelFromCurrent()),
				MaxLevel = (isGrouped ? this.m_lfm_maxLevel : this.MaxLevelFromCurrent()),
				Tags = (isGrouped ? this.m_lfm_tags : this.m_lfg_tags),
				Note = (isGrouped ? this.m_lfm_note : this.m_lfg_note),
				Type = (isGrouped ? LookingType.Lfm : LookingType.Lfg),
				Created = GameTimeReplicator.GetServerCorrectedDateTimeUtc()
			});
		}

		// Token: 0x06004422 RID: 17442 RVA: 0x0019A288 File Offset: 0x00198488
		private void OnLfgLevelChanged(int index)
		{
			if (index == 0)
			{
				this.m_lfg_maxLevel = this.MaxLevelFromCurrent();
				this.m_lfg_minLevel = this.MaxLevelFromCurrent();
			}
			else
			{
				this.m_lfg_maxLevel = index * 5;
				this.m_lfg_minLevel = this.m_lfg_maxLevel - 4;
			}
			if (ClientGameManager.SocialManager.IsLookingForGroup())
			{
				LookingFor ownLfgLfmEntry = ClientGameManager.SocialManager.OwnLfgLfmEntry;
				ownLfgLfmEntry.MaxLevel = this.m_lfg_maxLevel;
				ownLfgLfmEntry.MinLevel = this.m_lfg_minLevel;
				ClientGameManager.SocialManager.StartOrUpdateLooking(null);
			}
		}

		// Token: 0x06004423 RID: 17443 RVA: 0x0019A304 File Offset: 0x00198504
		private void OnLfmLevelChanged(int index)
		{
			if (index == 0)
			{
				this.m_lfm_maxLevel = 0;
				this.m_lfm_minLevel = 0;
			}
			else
			{
				this.m_lfm_maxLevel = index * 5;
				this.m_lfm_minLevel = this.m_lfm_maxLevel - 4;
			}
			if (ClientGameManager.SocialManager.IsLeaderOfGroupLookingForMember())
			{
				LookingFor ownLfgLfmEntry = ClientGameManager.SocialManager.OwnLfgLfmEntry;
				ownLfgLfmEntry.MaxLevel = this.m_lfm_maxLevel;
				ownLfgLfmEntry.MinLevel = this.m_lfm_minLevel;
				ClientGameManager.SocialManager.StartOrUpdateLooking(null);
			}
		}

		// Token: 0x06004424 RID: 17444 RVA: 0x0019A374 File Offset: 0x00198574
		private void OnLfmLevelMinChanged(string value)
		{
			int num;
			int num2;
			if (int.TryParse(value, out num) && int.TryParse(this.m_lfmLevelMax.text, out num2))
			{
				if (num < 1)
				{
					num = 1;
					this.m_lfmLevelMin.SetTextWithoutNotify("1");
				}
				if (num > 50)
				{
					num = 50;
					this.m_lfmLevelMin.SetTextWithoutNotify(this.MaxLevelAsString);
				}
				if (num > num2)
				{
					num2 = num;
					this.m_lfmLevelMax.SetTextWithoutNotify(num.ToString());
				}
				this.m_lfm_minLevel = num;
				this.m_lfm_maxLevel = num2;
			}
			if (ClientGameManager.SocialManager.IsLeaderOfGroupLookingForMember())
			{
				LookingFor ownLfgLfmEntry = ClientGameManager.SocialManager.OwnLfgLfmEntry;
				ownLfgLfmEntry.MaxLevel = this.m_lfm_maxLevel;
				ownLfgLfmEntry.MinLevel = this.m_lfm_minLevel;
				ClientGameManager.SocialManager.StartOrUpdateLooking(null);
			}
		}

		// Token: 0x06004425 RID: 17445 RVA: 0x0019A42C File Offset: 0x0019862C
		private void OnLfmLevelMaxChanged(string value)
		{
			int num;
			int num2;
			if (int.TryParse(this.m_lfmLevelMin.text, out num) && int.TryParse(value, out num2))
			{
				if (num2 < 1)
				{
					num2 = 1;
					this.m_lfmLevelMax.SetTextWithoutNotify("1");
				}
				if (num2 > 50)
				{
					num2 = 50;
					this.m_lfmLevelMax.SetTextWithoutNotify(this.MaxLevelAsString);
				}
				if (num > num2)
				{
					num = num2;
					this.m_lfmLevelMin.SetTextWithoutNotify(num2.ToString());
				}
				this.m_lfm_minLevel = num;
				this.m_lfm_maxLevel = num2;
			}
			if (ClientGameManager.SocialManager.IsLeaderOfGroupLookingForMember())
			{
				LookingFor ownLfgLfmEntry = ClientGameManager.SocialManager.OwnLfgLfmEntry;
				ownLfgLfmEntry.MaxLevel = this.m_lfm_maxLevel;
				ownLfgLfmEntry.MinLevel = this.m_lfm_minLevel;
				ClientGameManager.SocialManager.StartOrUpdateLooking(null);
			}
		}

		// Token: 0x06004426 RID: 17446 RVA: 0x0006E06E File Offset: 0x0006C26E
		private void OnListAllToggleChanged(bool value)
		{
			if (this.m_preventFindFilterToggleRefresh)
			{
				return;
			}
			if (value)
			{
				this.FindFilterMode = LfgLfmUI.FindFilterModes.All;
			}
		}

		// Token: 0x06004427 RID: 17447 RVA: 0x0006E083 File Offset: 0x0006C283
		private void OnListIndividualChanged(bool value)
		{
			if (this.m_preventFindFilterToggleRefresh)
			{
				return;
			}
			if (value)
			{
				this.FindFilterMode = LfgLfmUI.FindFilterModes.Individual;
			}
		}

		// Token: 0x06004428 RID: 17448 RVA: 0x0006E098 File Offset: 0x0006C298
		private void OnListGroupChanged(bool value)
		{
			if (this.m_preventFindFilterToggleRefresh)
			{
				return;
			}
			if (value)
			{
				this.FindFilterMode = LfgLfmUI.FindFilterModes.Group;
			}
		}

		// Token: 0x06004429 RID: 17449 RVA: 0x0006E0AD File Offset: 0x0006C2AD
		private void OnListLevelChanged(int index)
		{
			this.m_list_maxLevel = index * 5;
			this.m_list_minLevel = this.m_list_maxLevel - 4;
			this.RefreshListFiltering();
		}

		// Token: 0x0600442A RID: 17450 RVA: 0x0019A4E4 File Offset: 0x001986E4
		private void OnLfgTagsButtonClicked()
		{
			foreach (LfgLfmUI.TagToggle tagToggle in this.m_tagsViewToggles)
			{
				tagToggle.Toggle.isOn = ((this.m_lfg_tags & tagToggle.Tags) > LookingTags.None);
				if ((tagToggle.Tags & LookingTags.AllRoles) != LookingTags.None)
				{
					tagToggle.Toggle.interactable = false;
				}
			}
			this.m_tagsViewMode = LfgLfmUI.TagsViewMode.Lfg;
			this.m_tagsView.SetActive(true);
		}

		// Token: 0x0600442B RID: 17451 RVA: 0x0019A578 File Offset: 0x00198778
		private void OnLfmTagsButtonClicked()
		{
			foreach (LfgLfmUI.TagToggle tagToggle in this.m_tagsViewToggles)
			{
				tagToggle.Toggle.isOn = ((this.m_lfm_tags & tagToggle.Tags) > LookingTags.None);
				tagToggle.Toggle.interactable = true;
			}
			this.m_tagsViewMode = LfgLfmUI.TagsViewMode.Lfm;
			this.m_tagsView.SetActive(true);
		}

		// Token: 0x0600442C RID: 17452 RVA: 0x0019A600 File Offset: 0x00198800
		private void OnLfgNoteButtonClicked()
		{
			DialogOptions opts = new DialogOptions
			{
				Title = "Set LFG Note",
				Text = this.m_lfg_note,
				ConfirmationText = "Ok",
				CancelText = "Cancel",
				ShowCloseButton = false,
				CharacterLimit = GlobalSettings.Values.Social.LfgLfmNotesCharacterLimit,
				LineLimit = GlobalSettings.Values.Social.LfgLfmLineLimit,
				Callback = new Action<bool, object>(this.OnLfgNoteEditConfirmed)
			};
			ClientGameManager.UIManager.TextEntryDialog.Init(opts);
		}

		// Token: 0x0600442D RID: 17453 RVA: 0x0019A6A0 File Offset: 0x001988A0
		private void OnLfgNoteEditConfirmed(bool answer, object result)
		{
			if (answer)
			{
				this.m_lfg_note = ((string)result).Trim();
				this.m_lfgNoteButton.text = (string.IsNullOrEmpty(this.m_lfg_note) ? "[Empty]" : this.m_lfg_note);
				if (ClientGameManager.SocialManager.IsLookingForGroup())
				{
					ClientGameManager.SocialManager.OwnLfgLfmEntry.Note = this.m_lfg_note;
					ClientGameManager.SocialManager.StartOrUpdateLooking(null);
					this.RefreshSelfShelf();
				}
			}
		}

		// Token: 0x0600442E RID: 17454 RVA: 0x0019A718 File Offset: 0x00198918
		private void OnLfmNoteButtonClicked()
		{
			DialogOptions opts = new DialogOptions
			{
				Title = "Set LFM Note",
				Text = this.m_lfm_note,
				ConfirmationText = "Ok",
				CancelText = "Cancel",
				ShowCloseButton = false,
				CharacterLimit = GlobalSettings.Values.Social.LfgLfmNotesCharacterLimit,
				LineLimit = GlobalSettings.Values.Social.LfgLfmLineLimit,
				Callback = new Action<bool, object>(this.OnLfmNoteEditConfirmed)
			};
			ClientGameManager.UIManager.TextEntryDialog.Init(opts);
		}

		// Token: 0x0600442F RID: 17455 RVA: 0x0019A7B8 File Offset: 0x001989B8
		private void OnLfmNoteEditConfirmed(bool answer, object result)
		{
			if (answer)
			{
				this.m_lfm_note = ((string)result).Trim();
				this.m_lfmNoteButton.text = (string.IsNullOrEmpty(this.m_lfm_note) ? "[Empty]" : this.m_lfm_note);
				if (ClientGameManager.SocialManager.IsGroupLookingForMember())
				{
					ClientGameManager.SocialManager.OwnLfgLfmEntry.Note = this.m_lfm_note;
					ClientGameManager.SocialManager.StartOrUpdateLooking(null);
					this.RefreshSelfShelf();
				}
			}
		}

		// Token: 0x06004430 RID: 17456 RVA: 0x0019A830 File Offset: 0x00198A30
		private void OnListTagsButtonClicked()
		{
			foreach (LfgLfmUI.TagToggle tagToggle in this.m_tagsViewToggles)
			{
				tagToggle.Toggle.isOn = ((this.m_list_tags & tagToggle.Tags) > LookingTags.None);
				tagToggle.Toggle.interactable = true;
			}
			this.m_tagsViewMode = LfgLfmUI.TagsViewMode.List;
			this.m_tagsView.SetActive(true);
		}

		// Token: 0x06004431 RID: 17457 RVA: 0x0019A8B8 File Offset: 0x00198AB8
		private void OnTagsViewOkButtonClicked()
		{
			LookingTags lookingTags = LookingTags.None;
			foreach (LfgLfmUI.TagToggle tagToggle in this.m_tagsViewToggles)
			{
				if (tagToggle.Toggle.gameObject.activeInHierarchy && tagToggle.Toggle.isOn)
				{
					lookingTags |= tagToggle.Tags;
				}
			}
			switch (this.m_tagsViewMode)
			{
			case LfgLfmUI.TagsViewMode.List:
				this.m_list_tags = lookingTags;
				this.m_listTagsButton.text = this.m_list_tags.ToStringWithSpaces();
				this.RefreshListFiltering();
				break;
			case LfgLfmUI.TagsViewMode.Lfg:
				this.m_lfg_tags = lookingTags;
				this.m_lfgTagsButton.text = this.m_lfg_tags.ToStringWithSpaces();
				if (ClientGameManager.SocialManager.IsLookingForGroup())
				{
					ClientGameManager.SocialManager.OwnLfgLfmEntry.Tags = this.m_lfg_tags;
					ClientGameManager.SocialManager.StartOrUpdateLooking(null);
					this.RefreshSelfShelf();
				}
				break;
			case LfgLfmUI.TagsViewMode.Lfm:
				this.m_lfm_tags = lookingTags;
				this.m_lfmTagsButton.text = this.m_lfm_tags.ToStringWithSpaces();
				if (ClientGameManager.SocialManager.IsLeaderOfGroupLookingForMember())
				{
					ClientGameManager.SocialManager.OwnLfgLfmEntry.Tags = this.m_lfm_tags;
					ClientGameManager.SocialManager.StartOrUpdateLooking(null);
					this.RefreshSelfShelf();
				}
				break;
			}
			this.m_tagsView.SetActive(false);
		}

		// Token: 0x06004432 RID: 17458 RVA: 0x0006E0CC File Offset: 0x0006C2CC
		private void OnTagsViewCancelButtonClicked()
		{
			this.m_tagsView.SetActive(false);
		}

		// Token: 0x06004433 RID: 17459 RVA: 0x0019AA30 File Offset: 0x00198C30
		private void OnTagsViewAllButtonClicked()
		{
			foreach (LfgLfmUI.TagToggle tagToggle in this.m_tagsViewToggles)
			{
				if (tagToggle.Toggle.interactable)
				{
					tagToggle.Toggle.isOn = true;
				}
			}
		}

		// Token: 0x06004434 RID: 17460 RVA: 0x0019AA98 File Offset: 0x00198C98
		private void OnTagsViewNoneButtonClicked()
		{
			foreach (LfgLfmUI.TagToggle tagToggle in this.m_tagsViewToggles)
			{
				if (tagToggle.Toggle.interactable)
				{
					tagToggle.Toggle.isOn = false;
				}
			}
		}

		// Token: 0x06004435 RID: 17461 RVA: 0x0006E0DA File Offset: 0x0006C2DA
		private void OnFilterResetClicked()
		{
			this.m_list_minLevel = 0;
			this.m_list_maxLevel = 0;
			this.m_list_tags = LookingTags.None;
			this.m_listZoneDropdown.SetValueWithoutNotify(0);
			this.m_listLevelDropdown.SetValueWithoutNotify(0);
			this.RefreshListFiltering();
		}

		// Token: 0x06004436 RID: 17462 RVA: 0x0019AB00 File Offset: 0x00198D00
		private void RefreshSelfShelf()
		{
			bool flag = ClientGameManager.SocialManager.OwnLfgLfmEntry != null;
			if (this.m_listRect == null)
			{
				this.m_listRect = this.m_listUI.gameObject.GetComponent<RectTransform>();
				this.m_defaultOffsetMax = this.m_listRect.offsetMax;
			}
			if (flag)
			{
				this.m_selfViewItem.Init(ClientGameManager.SocialManager.OwnLfgLfmEntry);
			}
			this.m_selfView.SetActive(flag);
			Vector2 defaultOffsetMax = this.m_defaultOffsetMax;
			if (flag)
			{
				defaultOffsetMax.y -= 60f;
			}
			this.m_listRect.offsetMax = defaultOffsetMax;
		}

		// Token: 0x06004437 RID: 17463 RVA: 0x0019AB9C File Offset: 0x00198D9C
		private LookingTags TagsFromMasteries()
		{
			LookingTags lookingTags = LookingTags.None;
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Masteries != null)
			{
				foreach (ArchetypeInstance archetypeInstance in LocalPlayer.GameEntity.CollectionController.Masteries.Instances)
				{
					if (archetypeInstance.Archetype.name.Contains("Striker"))
					{
						lookingTags |= LookingTags.Striker;
					}
					if (archetypeInstance.Archetype.name.Contains("Defender"))
					{
						lookingTags |= LookingTags.Defender;
					}
					if (archetypeInstance.Archetype.name.Contains("Supporter"))
					{
						lookingTags |= LookingTags.Supporter;
					}
				}
			}
			return lookingTags;
		}

		// Token: 0x06004438 RID: 17464 RVA: 0x0019AC74 File Offset: 0x00198E74
		private int MaxLevelFromCurrent()
		{
			float num = 0f;
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Masteries != null)
			{
				foreach (ArchetypeInstance archetypeInstance in LocalPlayer.GameEntity.CollectionController.Masteries.Instances)
				{
					if (archetypeInstance.Mastery.Type.GetMasterySphere() == MasterySphere.Adventuring)
					{
						num = Math.Max(num, archetypeInstance.GetAssociatedLevel(LocalPlayer.GameEntity));
					}
				}
			}
			return (int)num;
		}

		// Token: 0x06004439 RID: 17465 RVA: 0x0019AD20 File Offset: 0x00198F20
		private void PopulateZones()
		{
			foreach (object obj in Enum.GetValues(typeof(ZoneId)))
			{
				ZoneRecord zoneRecord = SessionData.GetZoneRecord((ZoneId)obj);
				if (zoneRecord != null && AccessFlagsExtensions.HasAccess(zoneRecord.Flags, (int)LocalPlayer.GameEntity.UserFlags))
				{
					this.m_dropDownZones.Add(zoneRecord);
				}
			}
		}

		// Token: 0x0600443A RID: 17466 RVA: 0x0019ADA8 File Offset: 0x00198FA8
		private void PopulateLevelDropdown(TMP_Dropdown dropdown)
		{
			int num = 10;
			List<string> list = new List<string>(num);
			for (int i = 1; i <= num; i++)
			{
				int num2 = i * 5;
				list.Add(string.Format("{0}-{1}", num2 - 4, num2));
			}
			dropdown.AddOptions(list);
		}

		// Token: 0x0400408B RID: 16523
		private const int kLevelDropdownIncrement = 5;

		// Token: 0x0400408C RID: 16524
		private const string kEmptyNote = "[Empty]";

		// Token: 0x0400408D RID: 16525
		private const string kEmptyNonSubscriberNote = "[Empty] (Purchase Required)";

		// Token: 0x0400408E RID: 16526
		private bool m_preventFindFilterToggleRefresh;

		// Token: 0x0400408F RID: 16527
		private LfgLfmUI.FindFilterModes m_findFilterMode;

		// Token: 0x04004090 RID: 16528
		[SerializeField]
		private GameObject m_tabContent;

		// Token: 0x04004091 RID: 16529
		[SerializeField]
		private SolToggle m_lfgToggle;

		// Token: 0x04004092 RID: 16530
		[SerializeField]
		private SolButton m_lfgTagsButton;

		// Token: 0x04004093 RID: 16531
		[SerializeField]
		private SolButton m_lfgNoteButton;

		// Token: 0x04004094 RID: 16532
		[SerializeField]
		private Image m_lfgNoteBorder;

		// Token: 0x04004095 RID: 16533
		[SerializeField]
		private TMP_Dropdown m_lfgZoneDropdown;

		// Token: 0x04004096 RID: 16534
		[SerializeField]
		private TMP_Dropdown m_lfgLevelDropdown;

		// Token: 0x04004097 RID: 16535
		[SerializeField]
		private SolToggle m_lfmToggle;

		// Token: 0x04004098 RID: 16536
		[SerializeField]
		private SolButton m_lfmTagsButton;

		// Token: 0x04004099 RID: 16537
		[SerializeField]
		private SolButton m_lfmNoteButton;

		// Token: 0x0400409A RID: 16538
		[SerializeField]
		private Image m_lfmNoteBorder;

		// Token: 0x0400409B RID: 16539
		[SerializeField]
		private TMP_Dropdown m_lfmZoneDropdown;

		// Token: 0x0400409C RID: 16540
		[SerializeField]
		private TMP_Dropdown m_lfmLevelDropdown;

		// Token: 0x0400409D RID: 16541
		[SerializeField]
		private SolTMP_InputField m_lfmLevelMin;

		// Token: 0x0400409E RID: 16542
		[SerializeField]
		private SolTMP_InputField m_lfmLevelMax;

		// Token: 0x0400409F RID: 16543
		[SerializeField]
		private SolButton m_refreshButton;

		// Token: 0x040040A0 RID: 16544
		[SerializeField]
		private LfgLfmList m_listUI;

		// Token: 0x040040A1 RID: 16545
		[SerializeField]
		private SolToggle m_listAllToggle;

		// Token: 0x040040A2 RID: 16546
		[SerializeField]
		private SolToggle m_listIndividualsToggle;

		// Token: 0x040040A3 RID: 16547
		[SerializeField]
		private SolToggle m_listGroupsToggle;

		// Token: 0x040040A4 RID: 16548
		[SerializeField]
		private SolButton m_listTagsButton;

		// Token: 0x040040A5 RID: 16549
		[SerializeField]
		private TMP_Dropdown m_listZoneDropdown;

		// Token: 0x040040A6 RID: 16550
		[SerializeField]
		private TMP_Dropdown m_listLevelDropdown;

		// Token: 0x040040A7 RID: 16551
		[SerializeField]
		private GameObject m_listOverfilteredBlocker;

		// Token: 0x040040A8 RID: 16552
		[SerializeField]
		private GameObject m_selfView;

		// Token: 0x040040A9 RID: 16553
		[SerializeField]
		private LfgLfmListItem m_selfViewItem;

		// Token: 0x040040AA RID: 16554
		[SerializeField]
		private GameObject m_tagsView;

		// Token: 0x040040AB RID: 16555
		[SerializeField]
		private SolButton m_tagsViewOkButton;

		// Token: 0x040040AC RID: 16556
		[SerializeField]
		private SolButton m_tagsViewCancelButton;

		// Token: 0x040040AD RID: 16557
		[SerializeField]
		private SolButton m_tagsViewAllButton;

		// Token: 0x040040AE RID: 16558
		[SerializeField]
		private SolButton m_tagsViewNoneButton;

		// Token: 0x040040AF RID: 16559
		[SerializeField]
		private List<LfgLfmUI.TagToggle> m_tagsViewToggles;

		// Token: 0x040040B0 RID: 16560
		private readonly List<ZoneRecord> m_dropDownZones = new List<ZoneRecord>();

		// Token: 0x040040B1 RID: 16561
		private LfgLfmUI.TagsViewMode m_tagsViewMode;

		// Token: 0x040040B2 RID: 16562
		private const int kMaxLevelAsInt = 50;

		// Token: 0x040040B3 RID: 16563
		private readonly string MaxLevelAsString = 50.ToString();

		// Token: 0x040040B4 RID: 16564
		private int m_lfg_minLevel;

		// Token: 0x040040B5 RID: 16565
		private int m_lfg_maxLevel;

		// Token: 0x040040B6 RID: 16566
		private LookingTags m_lfg_tags;

		// Token: 0x040040B7 RID: 16567
		private string m_lfg_note;

		// Token: 0x040040B8 RID: 16568
		private int m_lfm_minLevel = 1;

		// Token: 0x040040B9 RID: 16569
		private int m_lfm_maxLevel = 50;

		// Token: 0x040040BA RID: 16570
		private LookingTags m_lfm_tags;

		// Token: 0x040040BB RID: 16571
		private string m_lfm_note;

		// Token: 0x040040BC RID: 16572
		private int m_list_minLevel;

		// Token: 0x040040BD RID: 16573
		private int m_list_maxLevel;

		// Token: 0x040040BE RID: 16574
		private LookingTags m_list_tags;

		// Token: 0x040040BF RID: 16575
		private bool m_isShown;

		// Token: 0x040040C0 RID: 16576
		private static readonly LookingFor[] m_emptyLookingList = new LookingFor[0];

		// Token: 0x040040C1 RID: 16577
		private static readonly List<LookingFor> m_filteredList = new List<LookingFor>();

		// Token: 0x040040C2 RID: 16578
		private Vector2 m_defaultOffsetMax;

		// Token: 0x040040C3 RID: 16579
		private RectTransform m_listRect;

		// Token: 0x0200090D RID: 2317
		[Serializable]
		private class TagToggle
		{
			// Token: 0x040040C4 RID: 16580
			public SolToggle Toggle;

			// Token: 0x040040C5 RID: 16581
			public LookingTags Tags;
		}

		// Token: 0x0200090E RID: 2318
		private enum TagsViewMode
		{
			// Token: 0x040040C7 RID: 16583
			List,
			// Token: 0x040040C8 RID: 16584
			Lfg,
			// Token: 0x040040C9 RID: 16585
			Lfm
		}

		// Token: 0x0200090F RID: 2319
		private enum FindFilterModes
		{
			// Token: 0x040040CB RID: 16587
			None,
			// Token: 0x040040CC RID: 16588
			All,
			// Token: 0x040040CD RID: 16589
			Individual,
			// Token: 0x040040CE RID: 16590
			Group
		}
	}
}
