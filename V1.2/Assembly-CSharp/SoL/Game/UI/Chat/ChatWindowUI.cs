using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Cysharp.Text;
using Newtonsoft.Json;
using SoL.Game.Dueling;
using SoL.Game.GM;
using SoL.Game.Grouping;
using SoL.Game.HuntingLog;
using SoL.Game.Messages;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Game.SkyDome;
using SoL.Managers;
using SoL.Networking;
using SoL.Networking.Database;
using SoL.Networking.Objects;
using SoL.Networking.SolServer;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI.Chat
{
	// Token: 0x020009B1 RID: 2481
	public class ChatWindowUI : ResizableWindow
	{
		// Token: 0x17001075 RID: 4213
		// (get) Token: 0x06004A85 RID: 19077 RVA: 0x0007236B File Offset: 0x0007056B
		// (set) Token: 0x06004A86 RID: 19078 RVA: 0x00072372 File Offset: 0x00070572
		public static int DesiredWindowCount { get; private set; } = 0;

		// Token: 0x17001076 RID: 4214
		// (get) Token: 0x06004A87 RID: 19079 RVA: 0x0007237A File Offset: 0x0007057A
		// (set) Token: 0x06004A88 RID: 19080 RVA: 0x00072382 File Offset: 0x00070582
		public ChatWindowSettings CurrentSettings
		{
			get
			{
				return this.m_currentSettings;
			}
			set
			{
				this.m_currentSettings = value;
			}
		}

		// Token: 0x17001077 RID: 4215
		// (get) Token: 0x06004A89 RID: 19081 RVA: 0x0007238B File Offset: 0x0007058B
		public bool WasFocusedLastFrame
		{
			get
			{
				return this.m_wasFocusedLastFrame;
			}
		}

		// Token: 0x17001078 RID: 4216
		// (get) Token: 0x06004A8A RID: 19082 RVA: 0x00072393 File Offset: 0x00070593
		public int FocusFrame
		{
			get
			{
				return this.m_focusFrame;
			}
		}

		// Token: 0x17001079 RID: 4217
		// (get) Token: 0x06004A8B RID: 19083 RVA: 0x0007239B File Offset: 0x0007059B
		public bool InputFocused
		{
			get
			{
				return this.m_input.isFocused;
			}
		}

		// Token: 0x1700107A RID: 4218
		// (get) Token: 0x06004A8C RID: 19084 RVA: 0x000723A8 File Offset: 0x000705A8
		public bool ShowTimestamps
		{
			get
			{
				return this.m_showTimestamps.isOn;
			}
		}

		// Token: 0x1700107B RID: 4219
		// (get) Token: 0x06004A8D RID: 19085 RVA: 0x000723B5 File Offset: 0x000705B5
		public ChatTabController TabController
		{
			get
			{
				return this.m_tabController;
			}
		}

		// Token: 0x1700107C RID: 4220
		// (get) Token: 0x06004A8E RID: 19086 RVA: 0x000723BD File Offset: 0x000705BD
		public ChatList ChatList
		{
			get
			{
				return this.m_chatList;
			}
		}

		// Token: 0x1700107D RID: 4221
		// (get) Token: 0x06004A8F RID: 19087 RVA: 0x000723C5 File Offset: 0x000705C5
		public ChatTabFilterList ChatTabFilterList
		{
			get
			{
				return this.m_filterList;
			}
		}

		// Token: 0x1700107E RID: 4222
		// (get) Token: 0x06004A90 RID: 19088 RVA: 0x000723CD File Offset: 0x000705CD
		public Image NewMessageAlert
		{
			get
			{
				return this.m_newMessageAlert;
			}
		}

		// Token: 0x1700107F RID: 4223
		// (get) Token: 0x06004A91 RID: 19089 RVA: 0x000723D5 File Offset: 0x000705D5
		public bool InputActiveInHierarchy
		{
			get
			{
				return this.m_input != null && this.m_input.gameObject.activeInHierarchy;
			}
		}

		// Token: 0x17001080 RID: 4224
		// (get) Token: 0x06004A92 RID: 19090 RVA: 0x000723F7 File Offset: 0x000705F7
		internal ChatTab ActiveTab
		{
			get
			{
				return this.m_tabController.ActiveTab;
			}
		}

		// Token: 0x06004A93 RID: 19091 RVA: 0x00072404 File Offset: 0x00070604
		public void StartWhisper(string target)
		{
			this.m_input.text = "/tell " + target + " ";
			this.m_input.Activate();
		}

		// Token: 0x06004A94 RID: 19092 RVA: 0x001B4518 File Offset: 0x001B2718
		protected override void Start()
		{
			this.m_mode.ClearOptions();
			List<string> fromPool = StaticListPool<string>.GetFromPool();
			foreach (string item in Enum.GetNames(typeof(ChatTabMode)))
			{
				fromPool.Add(item);
			}
			this.m_mode.AddOptions(fromPool);
			StaticListPool<string>.ReturnToPool(fromPool);
			this.m_input.onValueChanged.AddListener(new UnityAction<string>(this.OnInputChanged));
			this.m_addTabButton.onClick.AddListener(new UnityAction(this.OnAddTabClicked));
			this.m_settingsButton.onClick.AddListener(new UnityAction(this.OnSettingsClicked));
			this.m_toBottomButton.onClick.AddListener(new UnityAction(this.OnToBottomClicked));
			this.m_showTimestamps.onValueChanged.AddListener(new UnityAction<bool>(this.OnShowTimestampsChanged));
			this.m_mode.onValueChanged.AddListener(new UnityAction<int>(this.OnTabModeChanged));
			this.m_opacitySettingSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnOpacityChanged));
			this.m_tabController.TabAdded += this.OnTabAdded;
			this.m_tabController.TabChanged += this.OnTabChanged;
			this.m_tabController.TabRemoved += this.OnTabRemoved;
			this.m_filterList.FilterChanged += this.OnFilterSettingChanged;
			MessageTypeExtensions.CustomColorsChanged += this.MessageTypeExtensionsOnCustomColorsChanged;
			UIManager.RegisterChatWindow(this);
			if (this.ShouldLoadFromSettings)
			{
				this.LoadSettings();
			}
			else
			{
				ChatWindowUI.DesiredWindowCount++;
			}
			if (this.m_currentSettings != null && !this.m_currentSettings.Enabled)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				this.m_skipSettingsSaveOnDestroy = true;
			}
			if (this.m_currentSettings != null && this.m_currentSettings.TabSettings.Count == 0)
			{
				this.m_tabController.AddTab(ChatTabMode.Chat);
			}
			if (this.CreateAtPosition != Vector3.zero)
			{
				base.RectTransform.SetPositionAndRotation(this.CreateAtPosition, Quaternion.identity);
				base.RectTransform.ClampToScreen();
				base.SaveWindowPositionSize();
			}
			this.m_noTabOpenLabel.gameObject.SetActive(this.m_tabController.Tabs.Count == 0);
			this.m_newMessageAlert.gameObject.SetActive(false);
			base.Start();
			this.ToggleSubmissionArea(this.m_tabController.ActiveTab != null && this.m_tabController.ActiveTab.Mode == ChatTabMode.Chat);
			if (this.m_tabController.ActiveTab != null && this.m_tabController.ActiveTab.Mode == ChatTabMode.Chat)
			{
				this.RefreshChannelPrompt();
			}
			this.UpdateFilterListWhenReady();
			this.UpdateChatListWhenReady();
			if (!CommandRegistry.IsRegistered("?", true))
			{
				this.RegisterCommands();
			}
		}

		// Token: 0x06004A95 RID: 19093 RVA: 0x001B4800 File Offset: 0x001B2A00
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (!this.m_skipSettingsSaveOnDestroy)
			{
				this.SaveSettings(true);
			}
			UIManager.UnregisterChatWindow(this);
			this.m_input.onValueChanged.RemoveListener(new UnityAction<string>(this.OnInputChanged));
			this.m_addTabButton.onClick.RemoveListener(new UnityAction(this.OnAddTabClicked));
			this.m_settingsButton.onClick.RemoveListener(new UnityAction(this.OnSettingsClicked));
			this.m_toBottomButton.onClick.RemoveListener(new UnityAction(this.OnToBottomClicked));
			this.m_showTimestamps.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnShowTimestampsChanged));
			this.m_mode.onValueChanged.RemoveListener(new UnityAction<int>(this.OnTabModeChanged));
			this.m_opacitySettingSlider.onValueChanged.RemoveListener(new UnityAction<float>(this.OnOpacityChanged));
			this.m_tabController.TabAdded -= this.OnTabAdded;
			this.m_tabController.TabChanged -= this.OnTabChanged;
			this.m_tabController.TabRemoved -= this.OnTabRemoved;
			this.m_filterList.FilterChanged -= this.OnFilterSettingChanged;
			MessageTypeExtensions.CustomColorsChanged -= this.MessageTypeExtensionsOnCustomColorsChanged;
			this.m_filterList.Initialized -= this.UpdateFilterListWhenReady;
			this.m_chatList.Initialized -= this.UpdateChatListWhenReady;
		}

		// Token: 0x06004A96 RID: 19094 RVA: 0x001B4988 File Offset: 0x001B2B88
		private void Update()
		{
			if (!LocalPlayer.GameEntity || this.m_tabController.ActiveTab == null)
			{
				return;
			}
			DateTime now = DateTime.Now;
			if (this.m_currentSettings.IsDirty() && now > this.m_nextSettingsSync)
			{
				this.SaveSettings(false);
				this.m_nextSettingsSync = now.Add(this.m_settingsSyncInterval);
			}
			if (this.m_scrollToBottomFrameDelay > 0)
			{
				this.m_scrollToBottomFrameDelay--;
			}
			else if (this.m_scrollToBottomFrameDelay == 0)
			{
				this.m_chatList.ScrollToBottom(true);
				this.m_scrollToBottomFrameDelay--;
			}
			bool flag = UIManager.EventSystem.currentSelectedGameObject == this.m_input.gameObject && this.m_focusFrame < Time.frameCount;
			if (this.m_input.isFocused && !this.m_wasFocusedLastFrame && this.m_tabController.ActiveTab.Mode == ChatTabMode.Chat)
			{
				UIManager.SetLastActiveChat(this, this.m_tabController.ActiveTab);
			}
			this.m_wasFocusedLastFrame = this.m_input.isFocused;
			if (flag)
			{
				if (ClientGameManager.InputManager != null && ClientGameManager.InputManager.EnterDown)
				{
					this.SubmitInput();
				}
				if (Input.GetKeyDown(KeyCode.Escape))
				{
					this.ClearAndDeactivateInput();
				}
				if (this.m_history.Count > 0)
				{
					if (Input.GetKeyDown(KeyCode.UpArrow))
					{
						this.m_input.text = this.m_history[this.m_historyIndex];
						this.m_input.CaretToEnd();
						this.m_historyIndex++;
						if (this.m_historyIndex >= this.m_history.Count)
						{
							this.m_historyIndex = 0;
							return;
						}
					}
					else if (Input.GetKeyDown(KeyCode.DownArrow))
					{
						this.m_historyIndex--;
						if (this.m_historyIndex < 0)
						{
							this.m_historyIndex = this.m_history.Count - 1;
						}
						this.m_input.text = this.m_history[this.m_historyIndex];
						this.m_input.CaretToEnd();
					}
				}
			}
		}

		// Token: 0x06004A97 RID: 19095 RVA: 0x0007242C File Offset: 0x0007062C
		public bool IsTabFocused(ChatTab tab)
		{
			return this.m_tabController.ActiveTab == tab;
		}

		// Token: 0x06004A98 RID: 19096 RVA: 0x001B4B98 File Offset: 0x001B2D98
		public bool FocusTab(ChatTab tab)
		{
			int num = this.m_tabController.Tabs.IndexOf(tab);
			if (num != -1)
			{
				this.m_tabController.FocusTab(num);
				return true;
			}
			return false;
		}

		// Token: 0x06004A99 RID: 19097 RVA: 0x001B4BCC File Offset: 0x001B2DCC
		public bool FocusChatTab()
		{
			for (int i = 0; i < this.m_tabController.Tabs.Count; i++)
			{
				if (this.m_tabController.Tabs[i].Mode == ChatTabMode.Chat)
				{
					this.m_tabController.FocusTab(i);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004A9A RID: 19098 RVA: 0x0007243F File Offset: 0x0007063F
		public void AddDefaultTab()
		{
			this.OnAddTabClicked();
		}

		// Token: 0x06004A9B RID: 19099 RVA: 0x00072447 File Offset: 0x00070647
		private bool AllowPressedEvent()
		{
			this.m_focusFrame = Time.frameCount;
			return UIManager.EventSystem.currentSelectedGameObject != this.m_input.gameObject && !ClientGameManager.InputManager.InputPreventionFlags.HasBitFlag(InputPreventionFlags.InputField);
		}

		// Token: 0x06004A9C RID: 19100 RVA: 0x00072485 File Offset: 0x00070685
		public void EnterPressed()
		{
			if (this.AllowPressedEvent())
			{
				this.m_input.Activate();
			}
		}

		// Token: 0x06004A9D RID: 19101 RVA: 0x0007249A File Offset: 0x0007069A
		public void SlashPressed()
		{
			if (this.AllowPressedEvent() && string.IsNullOrEmpty(this.m_input.text))
			{
				this.m_input.text = "/";
				this.m_input.Activate();
			}
		}

		// Token: 0x06004A9E RID: 19102 RVA: 0x001B4C1C File Offset: 0x001B2E1C
		public void TellPressed()
		{
			if (this.AllowPressedEvent())
			{
				if (string.IsNullOrEmpty(ChatHandler.LastTellReceivedFrom))
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Tell, "You have not received any tells");
					return;
				}
				this.m_input.text = "/tell " + ChatHandler.LastTellReceivedFrom + " ";
				this.m_input.Activate();
			}
		}

		// Token: 0x06004A9F RID: 19103 RVA: 0x001B4C7C File Offset: 0x001B2E7C
		public void AddArchetypeLink(BaseArchetype archetype)
		{
			if (!archetype)
			{
				return;
			}
			int num = 0;
			bool flag = true;
			foreach (ValueTuple<BaseArchetype, int> valueTuple in ChatWindowUI.m_archetypeStubs)
			{
				if (valueTuple.Item1.Id == archetype.Id)
				{
					flag = false;
				}
				else if (valueTuple.Item1.DisplayName == archetype.DisplayName)
				{
					num++;
				}
			}
			if (flag)
			{
				ChatWindowUI.m_archetypeStubs.Add(new ValueTuple<BaseArchetype, int>(archetype, num));
			}
			string str = (num > 0) ? string.Format("{0} ({1})", archetype.DisplayName, num) : (archetype.DisplayName ?? "");
			if (this.m_input.text.Length > 0 && this.m_input.text[this.m_input.text.Length - 1] != ' ')
			{
				SolTMP_InputField input = this.m_input;
				input.text = input.text + " [" + str + "]";
			}
			else
			{
				SolTMP_InputField input2 = this.m_input;
				input2.text = input2.text + "[" + str + "]";
			}
			this.m_input.Activate();
		}

		// Token: 0x06004AA0 RID: 19104 RVA: 0x001B4DD8 File Offset: 0x001B2FD8
		public void AddInstanceLink(ArchetypeInstance instance)
		{
			if (instance == null || !instance.Archetype || !this.m_input)
			{
				return;
			}
			string modifiedDisplayName = instance.Archetype.GetModifiedDisplayName(instance);
			int num = 0;
			bool flag = true;
			foreach (ValueTuple<ArchetypeInstance, int> valueTuple in ChatWindowUI.m_instanceStubs)
			{
				if (valueTuple.Item1 != null && valueTuple.Item1.Archetype)
				{
					string modifiedDisplayName2 = valueTuple.Item1.Archetype.GetModifiedDisplayName(valueTuple.Item1);
					if (valueTuple.Item1.InstanceId == instance.InstanceId)
					{
						flag = false;
					}
					else if (modifiedDisplayName2 == modifiedDisplayName)
					{
						num++;
					}
				}
			}
			if (flag)
			{
				ArchetypeInstance fromPool = StaticPool<ArchetypeInstance>.GetFromPool();
				fromPool.CopyDataFrom(instance);
				ChatWindowUI.m_instanceStubs.Add(new ValueTuple<ArchetypeInstance, int>(fromPool, num));
			}
			string str = (num > 0) ? string.Format("{0} ({1})", modifiedDisplayName, num) : (modifiedDisplayName ?? "");
			if (this.m_input.text.Length > 0 && this.m_input.text[this.m_input.text.Length - 1] != ' ')
			{
				SolTMP_InputField input = this.m_input;
				input.text = input.text + " [" + str + "]";
			}
			else
			{
				SolTMP_InputField input2 = this.m_input;
				input2.text = input2.text + "[" + str + "]";
			}
			this.m_input.Activate();
		}

		// Token: 0x06004AA1 RID: 19105 RVA: 0x001B4F84 File Offset: 0x001B3184
		private void LoadSettings()
		{
			if (ChatWindowUI.DesiredWindowCount == 0)
			{
				ChatWindowUI.DesiredWindowCount = PlayerPrefs.GetInt("ChatWindowCount", 1);
			}
			if (this.m_settingsPrefsKey == null)
			{
				this.m_settingsPrefsKey = base.PlayerPrefsKey + "_SettingsV2";
			}
			try
			{
				if (PlayerPrefs.HasKey(this.m_settingsPrefsKey))
				{
					this.m_currentSettings = JsonConvert.DeserializeObject<ChatWindowSettings>(PlayerPrefs.GetString(this.m_settingsPrefsKey, string.Empty));
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			if (this.m_currentSettings == null)
			{
				this.m_currentSettings = new ChatWindowSettings(this.m_defaultSettings);
				this.m_currentSettings.SetAsCurrentVersion();
			}
			if (this.m_currentSettings != null)
			{
				if (this.m_currentSettings.CheckVersionAndUpdate())
				{
					this.SaveSettings(false);
				}
				this.m_ignoreSettingsChanges = true;
				this.m_showTimestamps.isOn = this.m_currentSettings.ShowTimestamps;
				this.m_opacitySettingSlider.value = this.m_currentSettings.Opacity;
				foreach (ChatTabSettings chatTabSettings in this.m_currentSettings.TabSettings)
				{
					this.m_tabController.AddTab(chatTabSettings.Mode);
					this.m_tabController.ActiveTab.Name = chatTabSettings.Name;
					this.m_tabController.ActiveTab.ChatFilter = chatTabSettings.ChatFilter;
					this.m_tabController.ActiveTab.CombatFilter = chatTabSettings.CombatFilter;
					if (chatTabSettings.InputChannel.CanBeDefaultChannel() && chatTabSettings.InputChannel.CanSpeakInChannel())
					{
						this.m_tabController.ActiveTab.SetInputChannel(chatTabSettings.InputChannel);
					}
				}
				this.m_tabController.FocusTab(0);
				this.m_ignoreSettingsChanges = false;
			}
		}

		// Token: 0x06004AA2 RID: 19106 RVA: 0x001B515C File Offset: 0x001B335C
		public void SaveSettings(bool fromInternal = false)
		{
			if (fromInternal)
			{
				if (this.m_currentSettings == null)
				{
					this.m_currentSettings = new ChatWindowSettings();
					this.m_currentSettings.SetAsCurrentVersion();
				}
				this.m_currentSettings.Enabled = (this.m_tabController.Tabs.Count > 0 || UIManager.ChatWindows.Count == 1);
				this.m_currentSettings.ShowTimestamps = this.m_showTimestamps.isOn;
				this.m_currentSettings.Opacity = this.m_opacitySettingSlider.value;
				if (this.m_currentSettings.TabSettings == null)
				{
					this.m_currentSettings.TabSettings = new List<ChatTabSettings>();
				}
				else
				{
					this.m_currentSettings.TabSettings.Clear();
				}
				foreach (ChatTab chatTab in this.m_tabController.Tabs)
				{
					this.m_currentSettings.TabSettings.Add(new ChatTabSettings
					{
						Mode = chatTab.Mode,
						Name = chatTab.Name,
						ChatFilter = chatTab.ChatFilter,
						CombatFilter = chatTab.CombatFilter,
						InputChannel = chatTab.InputChannel
					});
				}
			}
			if (this.m_settingsPrefsKey == null)
			{
				this.m_settingsPrefsKey = base.PlayerPrefsKey + "_SettingsV2";
			}
			if (this.m_currentSettings != null)
			{
				this.m_currentSettings.MarkAsClean();
				try
				{
					PlayerPrefs.SetInt("ChatWindowCount", ChatWindowUI.DesiredWindowCount);
					PlayerPrefs.SetString(this.m_settingsPrefsKey, JsonConvert.SerializeObject(this.m_currentSettings));
				}
				catch (Exception message)
				{
					Debug.LogError(message);
				}
			}
		}

		// Token: 0x06004AA3 RID: 19107 RVA: 0x001B5318 File Offset: 0x001B3518
		private void OnTabAdded(ChatTab newTab)
		{
			if (!this.m_ignoreSettingsChanges)
			{
				this.m_currentSettings.TabSettings.Add(new ChatTabSettings
				{
					Mode = newTab.Mode,
					ChatFilter = newTab.ChatFilter,
					CombatFilter = newTab.CombatFilter,
					InputChannel = newTab.InputChannel
				});
				this.m_currentSettings.MarkAsDirty();
			}
			if (newTab == this.m_tabController.ActiveTab)
			{
				this.OnTabChanged(newTab);
			}
			this.m_noTabOpenLabel.gameObject.SetActive(false);
		}

		// Token: 0x06004AA4 RID: 19108 RVA: 0x001B53A8 File Offset: 0x001B35A8
		private void OnTabRemoved(int index)
		{
			if (!this.m_ignoreSettingsChanges)
			{
				this.m_currentSettings.TabSettings.RemoveAt(index);
				this.m_currentSettings.MarkAsDirty();
			}
			if (this.m_tabController.ActiveTab == null && UIManager.ChatWindows.Count == 1)
			{
				this.ToggleSubmissionArea(false);
				this.UpdateFilterListWhenReady();
				this.UpdateChatListWhenReady();
				this.m_noTabOpenLabel.gameObject.SetActive(true);
				return;
			}
			if (this.m_tabController.ActiveTab == null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				this.m_currentSettings.Enabled = false;
				ChatWindowUI.DesiredWindowCount--;
				this.SaveSettings(false);
			}
		}

		// Token: 0x06004AA5 RID: 19109 RVA: 0x001B545C File Offset: 0x001B365C
		private void OnTabChanged(ChatTab newTab)
		{
			this.ToggleSubmissionArea(newTab.Mode == ChatTabMode.Chat);
			this.m_scrollToBottomFrameDelay = 1;
			this.m_ignoreModeChanges = true;
			this.m_mode.value = (int)newTab.Mode;
			this.m_ignoreModeChanges = false;
			this.m_filterList.Filter = newTab.CurrentFilter;
			this.RefreshChannelPrompt();
			if (this.m_filterList.IsInitialized)
			{
				this.UpdateFilterList(newTab);
			}
			else
			{
				this.UpdateFilterListWhenReady();
			}
			if (this.m_chatList.IsInitialized)
			{
				this.UpdateChatList(newTab);
				return;
			}
			this.UpdateChatListWhenReady();
		}

		// Token: 0x06004AA6 RID: 19110 RVA: 0x000724D1 File Offset: 0x000706D1
		private void OnShowTimestampsChanged(bool isOn)
		{
			if (!this.m_ignoreSettingsChanges)
			{
				this.m_chatList.ShowTimestampsChanged(isOn);
				this.m_currentSettings.ShowTimestamps = isOn;
				this.m_currentSettings.MarkAsDirty();
			}
		}

		// Token: 0x06004AA7 RID: 19111 RVA: 0x000724FE File Offset: 0x000706FE
		private void OnOpacityChanged(float value)
		{
			if (!this.m_ignoreSettingsChanges)
			{
				this.m_currentSettings.Opacity = value;
				this.m_currentSettings.MarkAsDirty();
			}
		}

		// Token: 0x06004AA8 RID: 19112 RVA: 0x001B54EC File Offset: 0x001B36EC
		private void OnTabModeChanged(int value)
		{
			if (this.m_ignoreModeChanges || this.m_tabController.ActiveTab == null)
			{
				return;
			}
			if (value == 0)
			{
				this.m_tabController.ActiveTab.Queue = MessageManager.ChatQueue;
			}
			else if (value == 1)
			{
				this.m_tabController.ActiveTab.Queue = MessageManager.CombatQueue;
			}
			if (!this.m_ignoreSettingsChanges)
			{
				this.m_currentSettings.TabSettings[this.m_tabController.ActiveTabIndex].Mode = this.m_tabController.ActiveTab.Mode;
				this.m_currentSettings.MarkAsDirty();
			}
			this.ToggleSubmissionArea(this.m_tabController.ActiveTab.Mode == ChatTabMode.Chat);
			this.RefreshChannelPrompt();
			this.UpdateFilterListWhenReady();
			this.UpdateChatListWhenReady();
		}

		// Token: 0x06004AA9 RID: 19113 RVA: 0x001B55B8 File Offset: 0x001B37B8
		private void ToggleSubmissionArea(bool isActive)
		{
			this.m_submissionArea.gameObject.SetActive(isActive);
			this.m_contentArea.rectTransform.offsetMin = new Vector2(this.m_contentArea.rectTransform.offsetMin.x, (float)(isActive ? 30 : 0));
			if (!isActive)
			{
				UIManager.UnregisterActiveChatInput(this);
			}
		}

		// Token: 0x06004AAA RID: 19114 RVA: 0x001B5614 File Offset: 0x001B3814
		private void OnFilterSettingChanged(int newFilter)
		{
			if (this.m_tabController.ActiveTab == null)
			{
				return;
			}
			if (this.m_mode.value == 0)
			{
				this.m_tabController.ActiveTab.ChatFilter = (ChatFilter)newFilter;
				if (!this.m_ignoreSettingsChanges)
				{
					this.m_currentSettings.TabSettings[this.m_tabController.ActiveTabIndex].ChatFilter = (ChatFilter)newFilter;
				}
			}
			else
			{
				this.m_tabController.ActiveTab.CombatFilter = (CombatFilter)newFilter;
				if (!this.m_ignoreSettingsChanges)
				{
					this.m_currentSettings.TabSettings[this.m_tabController.ActiveTabIndex].CombatFilter = (CombatFilter)newFilter;
				}
			}
			if (!this.m_ignoreSettingsChanges)
			{
				this.m_currentSettings.MarkAsDirty();
			}
		}

		// Token: 0x06004AAB RID: 19115 RVA: 0x001B56CC File Offset: 0x001B38CC
		private void OnInputChanged(string value)
		{
			if (!string.IsNullOrEmpty(value) && value[0] == '/' && value[value.Length - 1] == ' ' && this.m_tabController.ActiveTab != null)
			{
				MessageType messageTypeForShortcutText = MessageTypeExtensions.GetMessageTypeForShortcutText(value.Substring(1, value.Length - 2).ToLower());
				if (messageTypeForShortcutText != MessageType.None)
				{
					this.m_tabController.ActiveTab.SetInputChannel(messageTypeForShortcutText);
					if (!messageTypeForShortcutText.CanSpeakInChannel())
					{
						return;
					}
					if (!this.m_ignoreSettingsChanges && messageTypeForShortcutText.CanBeDefaultChannel() && messageTypeForShortcutText.CanSpeakInChannel())
					{
						this.m_currentSettings.TabSettings[this.m_tabController.ActiveTabIndex].InputChannel = messageTypeForShortcutText;
						this.m_currentSettings.MarkAsDirty();
					}
					this.m_input.text = string.Empty;
					this.RefreshChannelPrompt();
					return;
				}
				else
				{
					for (int i = 0; i < ChatWindowUI.m_tellPatterns.Length; i++)
					{
						Match match = ChatWindowUI.m_tellPatterns[i].Match(value);
						if (match.Success)
						{
							this.m_tellTarget = match.Groups[1].Value;
							this.m_tabController.ActiveTab.SetInputChannel(MessageType.Tell);
							this.m_input.text = string.Empty;
							this.RefreshChannelPrompt();
							return;
						}
					}
				}
			}
		}

		// Token: 0x06004AAC RID: 19116 RVA: 0x0007251F File Offset: 0x0007071F
		private void SubmitInput()
		{
			this.ProcessInput(this.m_input.text, true);
		}

		// Token: 0x06004AAD RID: 19117 RVA: 0x00072533 File Offset: 0x00070733
		public void SubmitMacroInput(string macro)
		{
			this.ProcessInput(macro, false);
		}

		// Token: 0x06004AAE RID: 19118 RVA: 0x0007253D File Offset: 0x0007073D
		private void OnAddTabClicked()
		{
			this.m_tabController.AddTab(ChatTabMode.Chat);
		}

		// Token: 0x06004AAF RID: 19119 RVA: 0x0007254B File Offset: 0x0007074B
		private void OnSettingsClicked()
		{
			this.m_settingsPanel.gameObject.SetActive(!this.m_settingsPanel.gameObject.activeInHierarchy);
			this.UpdateFilterListWhenReady();
		}

		// Token: 0x06004AB0 RID: 19120 RVA: 0x00072576 File Offset: 0x00070776
		private void OnToBottomClicked()
		{
			this.m_chatList.SetNormalizedPosition(0.0);
		}

		// Token: 0x06004AB1 RID: 19121 RVA: 0x0007258C File Offset: 0x0007078C
		public void UpdateFilterListWhenReady()
		{
			if (!this.m_filterList.IsInitialized)
			{
				this.m_filterList.Initialized += this.UpdateFilterListWhenReady;
				return;
			}
			this.UpdateFilterList(this.m_tabController.ActiveTab);
		}

		// Token: 0x06004AB2 RID: 19122 RVA: 0x001B5818 File Offset: 0x001B3A18
		private void UpdateFilterList(ChatTab tab)
		{
			if (tab == null)
			{
				this.m_filterList.UpdateItems(Array.Empty<ChatFilter>());
				return;
			}
			this.m_filterList.Filter = tab.CurrentFilter;
			ChatTabMode mode = tab.Mode;
			if (mode == ChatTabMode.Chat)
			{
				this.m_filterList.UpdateItems(ChatFilterExtensions.ChatFilters);
				return;
			}
			if (mode != ChatTabMode.Combat)
			{
				Debug.Log("Unexpected ChatTabMode in UpdateFilterList");
				return;
			}
			this.m_filterList.UpdateItems(CombatFilterExtensions.CombatFilters);
		}

		// Token: 0x06004AB3 RID: 19123 RVA: 0x001B588C File Offset: 0x001B3A8C
		private void MessageTypeExtensionsOnCustomColorsChanged()
		{
			ChatTab activeTab = this.m_tabController.ActiveTab;
			if (activeTab && activeTab.Mode == ChatTabMode.Chat)
			{
				for (int i = 0; i < this.m_filterList.GetItemsCount(); i++)
				{
					ChatTabFilterListItemViewsHolder cellViewsHolderIfVisible = this.m_filterList.GetCellViewsHolderIfVisible(i);
					if (cellViewsHolderIfVisible != null)
					{
						cellViewsHolderIfVisible.ListItem.Refresh();
					}
				}
				for (int j = 0; j < this.m_chatList.VisibleItemsCount; j++)
				{
					ChatItemViewsHolder itemViewsHolder = this.m_chatList.GetItemViewsHolder(j);
					if (itemViewsHolder != null)
					{
						itemViewsHolder.RefreshLineContent(this.m_currentSettings.ShowTimestamps);
					}
				}
			}
			this.m_tabController.ChatColorsChanged();
		}

		// Token: 0x06004AB4 RID: 19124 RVA: 0x001B592C File Offset: 0x001B3B2C
		private void RefreshChannelPrompt()
		{
			ChatTab activeTab = this.m_tabController.ActiveTab;
			bool flag;
			if (activeTab == null)
			{
				flag = true;
			}
			else
			{
				MessageType inputChannel = activeTab.InputChannel;
				flag = false;
			}
			if (flag)
			{
				this.m_prompt.gameObject.SetActive(false);
				return;
			}
			MessageType inputChannel2 = this.m_tabController.ActiveTab.InputChannel;
			Color color;
			inputChannel2.GetColor(out color, false);
			this.m_prompt.gameObject.SetActive(true);
			this.m_prompt.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200f);
			if (inputChannel2 == MessageType.Tell)
			{
				this.m_prompt.text = string.Format("{0} {1}: ", inputChannel2, this.m_tellTarget);
			}
			else
			{
				this.m_prompt.text = string.Format("{0}: ", inputChannel2);
			}
			this.m_prompt.color = color;
			this.m_prompt.ForceMeshUpdate(true, false);
			this.m_prompt.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.m_prompt.textBounds.size.x);
			if (this.m_inputRect == null)
			{
				this.m_inputRect = this.m_input.GetComponent<RectTransform>();
			}
			this.m_inputRect.offsetMin = new Vector2(this.m_prompt.textBounds.size.x + 10f, this.m_inputRect.offsetMin.y);
		}

		// Token: 0x06004AB5 RID: 19125 RVA: 0x001B5A8C File Offset: 0x001B3C8C
		private void ProcessInput(string value, bool isTextInput)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				if (isTextInput)
				{
					this.ClearAndDeactivateInput();
				}
				return;
			}
			if (value[0] == '/')
			{
				if (value.Length == 1)
				{
					if (isTextInput)
					{
						this.ClearAndDeactivateInput();
					}
					return;
				}
				string[] array = value.Substring(1).Split(ChatWindowUI.m_delimiterArray, StringSplitOptions.RemoveEmptyEntries);
				if (isTextInput && array.Length == 1)
				{
					MessageType messageTypeForShortcutText = MessageTypeExtensions.GetMessageTypeForShortcutText(array[0].ToLower());
					if (messageTypeForShortcutText != MessageType.None)
					{
						this.m_tabController.ActiveTab.SetInputChannel(messageTypeForShortcutText);
						if (!this.m_ignoreSettingsChanges && messageTypeForShortcutText.CanBeDefaultChannel() && messageTypeForShortcutText.CanSpeakInChannel())
						{
							this.m_currentSettings.TabSettings[this.m_tabController.ActiveTabIndex].InputChannel = messageTypeForShortcutText;
							this.m_currentSettings.MarkAsDirty();
						}
						this.m_input.text = string.Empty;
						this.RefreshChannelPrompt();
						return;
					}
				}
				if (array.Length == 1)
				{
					this.ProcessCommand(array[0].ToLower(), Array.Empty<string>());
				}
				else if (array.Length > 1)
				{
					List<string> fromPool = StaticListPool<string>.GetFromPool();
					StringBuilder fromPool2 = StringBuilderExtensions.GetFromPool();
					bool flag = false;
					for (int i = 1; i < array.Length; i++)
					{
						foreach (char c in array[i])
						{
							if (c == '"')
							{
								if (fromPool2.Length > 0)
								{
									fromPool.Add(fromPool2.ToString());
									fromPool2.Clear();
								}
								flag = !flag;
							}
							else
							{
								fromPool2.Append(c);
							}
						}
						if (flag)
						{
							fromPool2.Append(" ");
						}
						else if (fromPool2.Length > 0)
						{
							fromPool.Add(fromPool2.ToString());
							fromPool2.Clear();
						}
					}
					fromPool2.ReturnToPool();
					this.ProcessCommand(array[0].ToLower(), fromPool.ToArray());
					StaticListPool<string>.ReturnToPool(fromPool);
				}
			}
			else
			{
				if (!SolServerConnectionManager.IsOnline)
				{
					this.FailedToSend();
					return;
				}
				value = this.LinkifyArchetypeStubs(value);
				value = this.LinkifyInstanceStubs(value);
				string text2 = MessageManager.FindAndEncodeInstanceAttachments(value);
				if (text2 == "toolong")
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Link-attached content too long, please use fewer links.");
					return;
				}
				MessageType inputChannel = this.m_tabController.ActiveTab.InputChannel;
				SoL.Networking.SolServer.CommandType commandType = inputChannel.GetCommandType();
				SolServerCommand solServerCommand = CommandClass.chat.NewCommand(commandType);
				if (text2 != null)
				{
					solServerCommand.Args.Add("instances", text2);
				}
				solServerCommand.Args.Add("Message", value);
				if (inputChannel == MessageType.Tell)
				{
					solServerCommand.Args.Add("Receiver", this.m_tellTarget);
				}
				solServerCommand.Send();
				if (isTextInput && !inputChannel.CanBeDefaultChannel())
				{
					this.m_tabController.ActiveTab.RevertInputChannel();
					this.RefreshChannelPrompt();
				}
			}
			this.AppendHistory();
			if (isTextInput)
			{
				this.ClearAndDeactivateInput();
			}
			LocalPlayer.UpdateTimeOfLastInput();
		}

		// Token: 0x06004AB6 RID: 19126 RVA: 0x001B5D54 File Offset: 0x001B3F54
		private string LinkifyArchetypeStubs(string value)
		{
			foreach (ValueTuple<BaseArchetype, int> valueTuple in ChatWindowUI.m_archetypeStubs)
			{
				string oldValue = (valueTuple.Item2 > 0) ? string.Format("[{0} ({1})]", valueTuple.Item1.DisplayName, valueTuple.Item2) : ("[" + valueTuple.Item1.DisplayName + "]");
				value = value.Replace(oldValue, SoL.Utilities.Extensions.TextMeshProExtensions.CreateArchetypeLink(valueTuple.Item1));
			}
			return value;
		}

		// Token: 0x06004AB7 RID: 19127 RVA: 0x001B5DFC File Offset: 0x001B3FFC
		private string LinkifyInstanceStubs(string value)
		{
			foreach (ValueTuple<ArchetypeInstance, int> valueTuple in ChatWindowUI.m_instanceStubs)
			{
				if (valueTuple.Item1.Archetype == null)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Cannot link destroyed items. Link disabled.");
				}
				else
				{
					MessageManager.AddLinkedInstance(valueTuple.Item1, true);
					string oldValue = (valueTuple.Item2 > 0) ? string.Format("[{0} ({1})]", valueTuple.Item1.Archetype.GetModifiedDisplayName(valueTuple.Item1), valueTuple.Item2) : ("[" + valueTuple.Item1.Archetype.GetModifiedDisplayName(valueTuple.Item1) + "]");
					value = value.Replace(oldValue, SoL.Utilities.Extensions.TextMeshProExtensions.CreateInstanceLink(valueTuple.Item1));
				}
			}
			return value;
		}

		// Token: 0x06004AB8 RID: 19128 RVA: 0x000725C4 File Offset: 0x000707C4
		private void ProcessCommand(string command, string[] args)
		{
			if (CommandRegistry.IsRegistered(command, true))
			{
				CommandRegistry.Execute(command, args);
				return;
			}
			if (!Emote.AttemptEmote(LocalPlayer.GameEntity, command))
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Unrecognized Command: " + command);
			}
		}

		// Token: 0x06004AB9 RID: 19129 RVA: 0x001B5EF0 File Offset: 0x001B40F0
		private void ClearAndDeactivateInput()
		{
			ChatWindowUI.m_archetypeStubs.Clear();
			foreach (ValueTuple<ArchetypeInstance, int> valueTuple in ChatWindowUI.m_instanceStubs)
			{
				if (valueTuple.Item1 != null)
				{
					StaticPool<ArchetypeInstance>.ReturnToPool(valueTuple.Item1);
				}
			}
			ChatWindowUI.m_instanceStubs.Clear();
			this.m_input.text = string.Empty;
			this.m_input.Deactivate();
		}

		// Token: 0x06004ABA RID: 19130 RVA: 0x000725FB File Offset: 0x000707FB
		private void FailedToSend()
		{
			MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Failed to send...");
			this.m_input.Deactivate();
		}

		// Token: 0x06004ABB RID: 19131 RVA: 0x001B5F80 File Offset: 0x001B4180
		private void AppendHistory()
		{
			this.m_historyIndex = 0;
			if (this.m_history.Count <= 0 || !this.m_history[0].Equals(this.m_input.text))
			{
				this.m_history.Insert(0, this.m_input.text);
			}
			while (this.m_history.Count > 20)
			{
				this.m_history.RemoveAt(this.m_history.Count - 1);
			}
		}

		// Token: 0x06004ABC RID: 19132 RVA: 0x001B6000 File Offset: 0x001B4200
		private void RegisterCommands()
		{
			CommandRegistry.Register("?", delegate(string[] args)
			{
				if (args.Length == 1)
				{
					args[0] = args[0].Trim('/');
					if (CommandRegistry.IsRegistered(args[0], true))
					{
						Command command = CommandRegistry.GetCommand(args[0], true);
						StringBuilder fromPool = StringBuilderExtensions.GetFromPool();
						fromPool.Append("/");
						fromPool.Append(command.CommandText);
						fromPool.Append(" - ");
						if (!string.IsNullOrEmpty(command.Description))
						{
							fromPool.Append(command.Description);
						}
						else if (!string.IsNullOrEmpty(command.ShortDescription))
						{
							fromPool.Append(command.ShortDescription);
						}
						else
						{
							fromPool.Append("No description found");
						}
						if (command.Aliases != null && command.Aliases.Length != 0)
						{
							fromPool.Append("\nAliases:");
							foreach (string value in command.Aliases)
							{
								fromPool.Append(" /");
								fromPool.Append(value);
							}
						}
						MessageManager.ChatQueue.AddToQueue(MessageType.Notification, fromPool.ToString_ReturnToPool());
						return;
					}
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "No command found by that name");
					return;
				}
				else
				{
					if (args.Length > 1)
					{
						MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Usage: /? OR /? [command]");
						return;
					}
					if (ChatWindowUI.m_commandListOutput == null)
					{
						IEnumerable<Command> commandList = CommandRegistry.GetCommandList();
						StringBuilder fromPool2 = StringBuilderExtensions.GetFromPool();
						fromPool2.Append("/? or /list to show this output\n\n");
						fromPool2.Append("/? [command] to show a longer description for a command (if one is available)\n\n");
						foreach (Command command2 in commandList)
						{
							if (!(command2.CommandText == "?") && !(command2.CommandText == "list"))
							{
								if (!string.IsNullOrEmpty(command2.ShortDescription))
								{
									fromPool2.AppendLine("/" + command2.CommandText + " - " + command2.ShortDescription);
								}
								else
								{
									fromPool2.AppendLine("/" + command2.CommandText);
								}
							}
						}
						ChatWindowUI.m_commandListOutput = fromPool2.ToString_ReturnToPool();
					}
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, ChatWindowUI.m_commandListOutput);
					return;
				}
			}, "Show a list of commands", "Show a list of commands with \"/?\" or show the description for a specific command with \"/? [command]\"", new string[]
			{
				"list"
			});
			CommandRegistry.Register("tell", delegate(string[] args)
			{
				if (args.Length < 1)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Usage: /tell [player name] [message]");
					return;
				}
				string text = string.Join(" ", new ReadOnlySpan<string>(args, 1, args.Length - 1).ToArray());
				text = this.LinkifyArchetypeStubs(text);
				text = this.LinkifyInstanceStubs(text);
				string text2 = MessageManager.FindAndEncodeInstanceAttachments(text);
				if (text2 == "toolong")
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Link-attached content too long, please use fewer links.");
					return;
				}
				SolServerCommand solServerCommand = CommandClass.chat.NewCommand(SoL.Networking.SolServer.CommandType.tell);
				solServerCommand.Args.Add("Receiver", args[0]);
				if (text2 != null)
				{
					solServerCommand.Args.Add("instances", text2);
				}
				solServerCommand.Args.Add("Message", text);
				solServerCommand.Send();
			}, "Send a tell to another player", "Send a tell to a player by the given name (Usage: /tell [player name] [message])", new string[]
			{
				"t"
			});
			CommandRegistry.Register("r", delegate(string[] args)
			{
				if (string.IsNullOrEmpty(ChatHandler.LastTellReceivedFrom))
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Tell, "You Have not received any Tells");
					return;
				}
				string text = string.Join(" ", args);
				text = this.LinkifyArchetypeStubs(text);
				text = this.LinkifyInstanceStubs(text);
				string text2 = MessageManager.FindAndEncodeInstanceAttachments(text);
				if (text2 == "toolong")
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Link-attached content too long, please use fewer links.");
					return;
				}
				SolServerCommand solServerCommand = CommandClass.chat.NewCommand(SoL.Networking.SolServer.CommandType.tell);
				solServerCommand.Args.Add("Receiver", ChatHandler.LastTellReceivedFrom);
				if (text2 != null)
				{
					solServerCommand.Args.Add("instances", text2);
				}
				solServerCommand.Args.Add("Message", text);
				solServerCommand.Send();
			}, "Reply to the last tell", null, null);
			MessageTypeExtensions.RegisterChannelShortcutsAsCommands(delegate(string value)
			{
				value = this.LinkifyArchetypeStubs(value);
				value = this.LinkifyInstanceStubs(value);
				string text = MessageManager.FindAndEncodeInstanceAttachments(value);
				if (text == "toolong")
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Link-attached content too long, please use fewer links.");
					return new ValueTuple<string, string>(null, null);
				}
				return new ValueTuple<string, string>(value, text);
			});
			CommandRegistry.Register("combat", delegate(string[] args)
			{
				LocalPlayer.Animancer.ToggleCombat();
			}, "Toggle Combat Stance", null, null);
			CommandRegistry.Register("sit", delegate(string[] args)
			{
				LocalPlayer.Animancer.ToggleSit();
			}, "Toggle Sitting", null, null);
			CommandRegistry.Register("torch", delegate(string[] args)
			{
				LocalPlayer.Animancer.ToggleLight();
			}, "Toggle Torch Stance", null, null);
			CommandRegistry.Register("online", delegate(string[] args)
			{
				LocalPlayer.GameEntity.CharacterData.Presence = Presence.Online;
			}, "Set yourself as \"Online\"", null, null);
			CommandRegistry.Register("afk", delegate(string[] args)
			{
				LocalPlayer.GameEntity.CharacterData.Presence = ((LocalPlayer.GameEntity.CharacterData.Presence != Presence.Away) ? Presence.Away : Presence.Online);
			}, "Toggle AFK", null, null);
			CommandRegistry.Register("dnd", delegate(string[] args)
			{
				LocalPlayer.GameEntity.CharacterData.Presence = ((LocalPlayer.GameEntity.CharacterData.Presence != Presence.DoNotDisturb) ? Presence.DoNotDisturb : Presence.Online);
			}, "Toggle Do Not Disturb", null, null);
			CommandRegistry.Register("anon", delegate(string[] args)
			{
				LocalPlayer.GameEntity.CharacterData.Presence = ((LocalPlayer.GameEntity.CharacterData.Presence != Presence.Anonymous) ? Presence.Anonymous : Presence.Online);
			}, "Toggle Anonymous", null, null);
			CommandRegistry.Register("lfg", delegate(string[] args)
			{
				if (ClientGameManager.SocialManager.IsLookingForGroup())
				{
					ClientGameManager.SocialManager.StopLooking(true);
					return;
				}
				if (ClientGameManager.GroupManager.IsGrouped)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Please leave your current group before marking yourself as LFG.");
					return;
				}
				string note = null;
				if (args.Length != 0)
				{
					note = string.Join(' ', args);
				}
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
				ClientGameManager.SocialManager.StartOrUpdateLooking(new LookingFor
				{
					Key = LocalPlayer.GameEntity.CharacterData.CharacterId,
					ContactName = LocalPlayer.GameEntity.CharacterData.Name.Value,
					MinLevel = LocalPlayer.GameEntity.CharacterData.AdventuringLevel,
					MaxLevel = LocalPlayer.GameEntity.CharacterData.AdventuringLevel,
					Tags = ((ClientGameManager.SocialManager.OwnLfgLfmEntry != null) ? ClientGameManager.SocialManager.OwnLfgLfmEntry.Tags : lookingTags),
					Note = note,
					Type = LookingType.Lfg,
					Created = GameTimeReplicator.GetServerCorrectedDateTimeUtc()
				});
			}, null, null, null);
			CommandRegistry.Register("lfm", delegate(string[] args)
			{
				if (!ClientGameManager.GroupManager.IsGrouped)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "You must be in a group to utilize LFM.");
					return;
				}
				if (!ClientGameManager.GroupManager.IsLeader)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "You must be group leader to utilize LFM.");
					return;
				}
				if (ClientGameManager.SocialManager.IsLeaderOfGroupLookingForMember())
				{
					ClientGameManager.SocialManager.StopLooking(true);
					return;
				}
				string note = null;
				if (args.Length != 0)
				{
					note = string.Join(' ', args);
				}
				ClientGameManager.SocialManager.StartOrUpdateLooking(new LookingFor
				{
					Key = LocalPlayer.GameEntity.CharacterData.GroupId,
					ContactName = LocalPlayer.GameEntity.CharacterData.Name.Value,
					MinLevel = LocalPlayer.GameEntity.CharacterData.AdventuringLevel,
					MaxLevel = LocalPlayer.GameEntity.CharacterData.AdventuringLevel,
					Tags = LookingTags.None,
					Note = note,
					Type = LookingType.Lfm,
					Created = GameTimeReplicator.GetServerCorrectedDateTimeUtc()
				});
			}, null, null, null);
			CommandRegistry.Register("quit", delegate(string[] args)
			{
				if (ClientGameManager.CampManager)
				{
					ClientGameManager.CampManager.Quit();
				}
			}, "Quit the game", null, new string[]
			{
				"exit"
			});
			CommandRegistry.Register("camp", delegate(string[] args)
			{
				if (ClientGameManager.CampManager)
				{
					ClientGameManager.CampManager.InitiateCamp(true);
				}
			}, "Camp out to character selection", null, null);
			CommandRegistry.Register("duel", delegate(string[] args)
			{
				DuelExtensions.ClientRequestDuelAttempt();
			}, "Challenge your defensive target to a duel by death roll!", null, new string[]
			{
				"deathroll"
			});
			CommandRegistry.Register("time", delegate(string[] args)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, SkyDomeManager.GetGameTimeChatCommand());
			}, "Displays the in-game time", null, null);
			CommandRegistry.Register("localtime", delegate(string[] args)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, ZString.Format<string, string>("<color={0}>{1} (LOCAL)</color>", UIManager.GrayColor.ToHex(), SkyDomeManager.GetLocalDisplayTime()));
			}, "Displays the IRL time", null, null);
			CommandRegistry.Register("utctime", delegate(string[] args)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, ZString.Format<string, string>("<color={0}>{1} (UTC)</color>", UIManager.GrayColor.ToHex(), SkyDomeManager.GetUtcDisplayTime()));
			}, "Displays the IRL UTC time", null, null);
			CommandRegistry.Register("servertime", delegate(string[] args)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, ZString.Format<string, string>("<color={0}>{1} (SERVER)</color>", UIManager.GrayColor.ToHex(), SkyDomeManager.GetServerDisplayTime()));
			}, "Displays the IRL time for the server (CST)", null, null);
			CommandRegistry.Register("played", delegate(string[] args)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, LocalPlayer.GetTimePlayed());
			}, "Displays the amount of time you have played this session and overall", null, null);
			CommandRegistry.Register("stuck", delegate(string[] args)
			{
				LocalPlayer.Motor.StuckRequested();
			}, "Attempts to automatically move your character to a location where they will not be stuck", null, null);
			CommandRegistry.Register("rope", delegate(string[] args)
			{
				LocalPlayer.Motor.RopeRequested();
			}, "Pulls you to your defensive target if they are close enough. Use this when /stuck does not work.", null, null);
			CommandRegistry.Register("follow", delegate(string[] args)
			{
				if (!LocalPlayer.GameEntity || !LocalPlayer.GameEntity.Subscriber)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "The follow feature is reserved for subscribers.");
					return;
				}
				GameEntity followTarget = null;
				string text;
				if (args.Length == 1 && args[0].TryParseArgs(out text))
				{
					text = text.Trim();
					Collider[] colliders = Hits.Colliders25;
					int num = Physics.OverlapSphereNonAlloc(LocalPlayer.GameEntity.gameObject.transform.position, 12f, colliders, LayerMap.Detection.LayerMask, QueryTriggerInteraction.Ignore);
					for (int i = 0; i < num; i++)
					{
						GameEntity gameEntity;
						if (DetectionCollider.TryGetEntityForCollider(colliders[i], out gameEntity) && gameEntity.Type == GameEntityType.Player && gameEntity.CharacterData && gameEntity.CharacterData.Name.Value.Equals(text, StringComparison.InvariantCultureIgnoreCase))
						{
							followTarget = gameEntity;
							break;
						}
					}
				}
				else if (LocalPlayer.GameEntity.TargetController && LocalPlayer.GameEntity.TargetController.DefensiveTarget)
				{
					followTarget = LocalPlayer.GameEntity.TargetController.DefensiveTarget;
				}
				LocalPlayer.SetFollowTarget(followTarget);
			}, "Follow a group member. (Subscriber only feature)", null, null);
			CommandRegistry.Register("bag", delegate(string[] args)
			{
				CorpseManager.Client_AttemptToDragCorpse(args);
			}, "Attempts to drag your bag to your position.", null, new string[]
			{
				"corpse"
			});
			CommandRegistry.Register("groupconsent", delegate(string[] args)
			{
				if (LocalPlayer.NetworkEntity && LocalPlayer.NetworkEntity.PlayerRpcHandler)
				{
					LocalPlayer.NetworkEntity.PlayerRpcHandler.ToggleGroupConsent();
				}
			}, "Toggle bag drag permissions for your group members", null, null);
			CommandRegistry.Register("inspect", delegate(string[] args)
			{
				NetworkEntity networkEntity = null;
				if (args.Length == 1)
				{
					Collider[] colliders = Hits.Colliders25;
					int num = Physics.OverlapSphereNonAlloc(LocalPlayer.GameEntity.gameObject.transform.position, GlobalSettings.Values.General.GlobalInteractionDistance, colliders, LayerMap.Detection.LayerMask, QueryTriggerInteraction.Ignore);
					for (int i = 0; i < num; i++)
					{
						GameEntity gameEntity;
						if (DetectionCollider.TryGetEntityForCollider(colliders[i], out gameEntity) && gameEntity.Type == GameEntityType.Player && gameEntity.CharacterData && gameEntity.CharacterData.Name.Value.Equals(args[0], StringComparison.InvariantCultureIgnoreCase))
						{
							networkEntity = gameEntity.NetworkEntity;
							break;
						}
					}
					if (!networkEntity)
					{
						MessageManager.ChatQueue.AddToQueue(MessageType.Notification, ZString.Format<string>("Unable to find {0} nearby to inspect.", args[0]));
						return;
					}
				}
				else if (LocalPlayer.GameEntity.TargetController.DefensiveTarget)
				{
					networkEntity = LocalPlayer.GameEntity.TargetController.DefensiveTarget.NetworkEntity;
				}
				if (!networkEntity)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "No one to inspect! Please select a defensive target or pass their name (/inspect PlayerName).");
					return;
				}
				if (networkEntity.GameEntity.CharacterData.BlockInspections)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, ZString.Format<string>("{0} does not currently allow inspections.", networkEntity.GameEntity.CharacterData.Name.Value));
					return;
				}
				if (Vector3.Distance(networkEntity.gameObject.transform.position, LocalPlayer.GameEntity.gameObject.transform.position) > GlobalSettings.Values.General.GlobalInteractionDistance * 1.1f)
				{
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, ZString.Format<string>("{0} is too far away to inspect.", networkEntity.GameEntity.CharacterData.Name.Value));
					return;
				}
				LocalPlayer.GameEntity.NetworkEntity.PlayerRpcHandler.InspectRequest(networkEntity);
			}, "Inspect another player's equipment.", null, null);
			if (GlobalSettings.Values.HuntingLog.Enabled)
			{
				CommandRegistry.Register("huntinglog", delegate(string[] args)
				{
					HuntingLogProfile.PrintHuntingLogToChat();
				}, "Print hunting log status.", null, null);
			}
			CommandRegistry.Register("report", delegate(string[] args)
			{
				DialogOptions dialogOptions = default(DialogOptions);
				dialogOptions.Title = "Report";
				dialogOptions.Text = string.Join(" ", args);
				dialogOptions.ConfirmationText = "Submit";
				dialogOptions.CancelText = "Cancel";
				dialogOptions.Callback = delegate(bool answer, object result)
				{
					string content = "Report Cancelled";
					if (answer)
					{
						string value = (string)result;
						if (!string.IsNullOrEmpty(value))
						{
							LocalPlayer.NetworkEntity.PlayerRpcHandler.SendReport(new LongString
							{
								Value = value
							});
							content = "Report Submitted";
						}
					}
					MessageManager.ChatQueue.AddToQueue(MessageType.Notification, content);
				};
				DialogOptions opts = dialogOptions;
				ClientGameManager.UIManager.TextEntryDialog.Init(opts);
			}, "Sends a report to Stormhaven Studios", "Sends a report to Stormhaven Studios containing your message as well as your current zone and location", null);
			CommandRegistry.Register("debugposition", delegate(string[] args)
			{
				LocalPlayer.CopyTeleportStringToClipboard();
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Debug position copied to clipboard");
			}, "Saves your current location to your clipboard", "Saves your current location to your clipboard so you may easily send it to a Stormhaven developer", null);
			CommandRegistry.Register("friend", delegate(string[] args)
			{
				ClientGameManager.SocialManager.Friend(string.Join(" ", args));
			}, "Sends or accepts a friend request", "Sends a friend request to the given player or accepts a friend request from them (Usage /friend [player name])", null);
			CommandRegistry.Register("unfriend", delegate(string[] args)
			{
				ClientGameManager.SocialManager.Unfriend(string.Join(" ", args));
			}, "Unfriends a player", "Removes the given player from your friends list (Usage: /unfriend [player name])", null);
			CommandRegistry.Register("block", delegate(string[] args)
			{
				ClientGameManager.SocialManager.Block(string.Join(" ", args));
			}, "Blocks a player", "Blocks the player by the given name (Usage: /block [player name])", null);
			CommandRegistry.Register("unblock", delegate(string[] args)
			{
				ClientGameManager.SocialManager.Unblock(string.Join(" ", args));
			}, "Unblocks a player", "Unblocks the player by the given name (Usage: /unblock [player name])", null);
			CommandRegistry.Register("gcreate", delegate(string[] args)
			{
				ClientGameManager.SocialManager.CreateNewGuild(string.Join(" ", args));
			}, "Creates a guild", "Creates a guild by the given name if the name is not in use or otherwise disallowed (Usage: /gcreate [new guild name])", null);
			CommandRegistry.Register("ginvite", delegate(string[] args)
			{
				ClientGameManager.SocialManager.InviteToGuild(string.Join(" ", args));
			}, "Invites a player to your guild", "Invites the player by the given name to your current guild (Usage: /ginvite [player name])", null);
			CommandRegistry.Register("gpromote", delegate(string[] args)
			{
				ClientGameManager.SocialManager.PromoteGuildMember(string.Join(" ", args));
			}, "Promotes a player within your guild", "Promotes the player by the given name if you are capable (Usage: /gpromote [player name])", null);
			CommandRegistry.Register("gdemote", delegate(string[] args)
			{
				ClientGameManager.SocialManager.DemoteGuildMember(string.Join(" ", args));
			}, "Demotes a player within your guild", "Demotes the player by the given name if you are capable (Usage: /gdemote [player name])", null);
			CommandRegistry.Register("gkick", delegate(string[] args)
			{
				ClientGameManager.SocialManager.KickGuildMember(string.Join(" ", args));
			}, "Kicks a player from your guild", "Kicks the player by the given name if you are capable (Usage: /gkick [player name])", null);
			CommandRegistry.Register("gleave", delegate(string[] args)
			{
				ClientGameManager.SocialManager.LeaveGuild();
			}, "Leaves your current guild", null, null);
			CommandRegistry.Register("gdisband", delegate(string[] args)
			{
				ClientGameManager.SocialManager.DisbandGuild();
			}, "Disbands your current guild if you are capable", null, null);
			CommandRegistry.Register("who", delegate(string[] args)
			{
				this.RemoteCommand(CommandClass.chat, SoL.Networking.SolServer.CommandType.who, "Message", args);
			}, "Displays a list of online players", "Displays a list of online players in your current zone, \"/who gm\" to list all GMs, \"/who all\" to list players in all zones, \"/who [something else]\" to list all players whose names contain the given text", null);
			CommandRegistry.Register("roll", delegate(string[] args)
			{
				this.RemoteCommand(CommandClass.chat, SoL.Networking.SolServer.CommandType.roll, "Message", args);
			}, "Rolls a d100", "Rolls a d100 for players around you to see the result of. \"/roll [dice]\" with the standard nomenclature of 1d6, 2d10, etc. to roll dice of any combination (also supports modifiers: 2d20+5, 1d8-1, etc.)", null);
			CommandRegistry.Register("emote", delegate(string[] args)
			{
				this.RemoteCommand(CommandClass.chat, SoL.Networking.SolServer.CommandType.emote, "Message", args);
			}, "Sends a message as emotive text", "Displays the following message as emotive text (Usage: /emote [message])", new string[]
			{
				"e",
				"em",
				"me"
			});
			CommandRegistry.Register("invite", delegate(string[] args)
			{
				if (args.Length == 0 && LocalPlayer.GameEntity && LocalPlayer.GameEntity.TargetController && LocalPlayer.GameEntity.TargetController.DefensiveTarget && LocalPlayer.GameEntity.TargetController.DefensiveTarget.Type == GameEntityType.Player && LocalPlayer.GameEntity.TargetController.DefensiveTarget.CharacterData)
				{
					args = new string[]
					{
						LocalPlayer.GameEntity.TargetController.DefensiveTarget.CharacterData.Name.Value
					};
				}
				this.RemoteCommand(CommandClass.group, SoL.Networking.SolServer.CommandType.invite, "Receiver", args);
			}, "Invites a player to group", "Invites the player by the given name to your group (Usage: /invite [player name])", null);
			CommandRegistry.Register("accept", delegate(string[] args)
			{
				this.RemoteCommand(CommandClass.group, SoL.Networking.SolServer.CommandType.accept, "Receiver", args);
			}, "Accepts the pending group invite", null, null);
			CommandRegistry.Register("decline", delegate(string[] args)
			{
				this.RemoteCommand(CommandClass.group, SoL.Networking.SolServer.CommandType.decline, "Receiver", args);
			}, "Declines the pending group invite", null, null);
			CommandRegistry.Register("promote", delegate(string[] args)
			{
				this.RemoteCommand(CommandClass.group, SoL.Networking.SolServer.CommandType.promote, "Receiver", args);
			}, "Promotes a player to group leader", "Promotes the player with the given name to group leader (Usage: /promote [player name])", null);
			CommandRegistry.Register("kick", delegate(string[] args)
			{
				this.RemoteCommand(CommandClass.group, SoL.Networking.SolServer.CommandType.kick, "Receiver", args);
			}, "Kicks a player from the group", "Kicks the player with the given name from the group (Usage: /kick [player name])", null);
			CommandRegistry.Register("leave", delegate(string[] args)
			{
				this.RemoteCommand(CommandClass.group, SoL.Networking.SolServer.CommandType.leave, "Receiver", args);
			}, "Leaves the group", null, null);
			CommandRegistry.Register("instance", delegate(string[] args)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "You are in instance " + LocalZoneManager.ZoneRecord.InstanceId.ToString());
			}, "Displays the current zone instance (Usage: /instance)", null, null);
			CommandRegistry.Register("switchinstance", delegate(string[] args)
			{
				int instanceId;
				if (args.Length == 1 && args[0].TryParseArgs(out instanceId))
				{
					GameManager.SceneCompositionManager.RequestInstanceChange(instanceId);
				}
			}, "Switch to a specific instance if available (Usage: /switchinstance 1)", null, null);
			CommandRegistry.Register("rinvite", delegate(string[] args)
			{
				ClientGameManager.SocialManager.SendRaidInvite(string.Join(" ", args));
			}, "Invite a player's group to a raid", "Invites the given player's group to the raid. Their group leader will receive the invite. The player must be in a group. (Usage: /rinvite [player name])", null);
			CommandRegistry.Register("raccept", delegate(string[] args)
			{
				ClientGameManager.SocialManager.AcceptRaidInvite();
			}, "Accepts the pending raid invite", null, null);
			CommandRegistry.Register("rdecline", delegate(string[] args)
			{
				ClientGameManager.SocialManager.DeclineRaidInvite();
			}, "Declines the pending raid invite", null, null);
			CommandRegistry.Register("rleave", delegate(string[] args)
			{
				this.RemoteCommand(CommandClass.group, SoL.Networking.SolServer.CommandType.raidleave, "Receiver", args);
			}, "Removes your group from the current raid.", "Removes your group from the current raid. You must be group leader to perform this action. (Usage: /rleave)", null);
			CommandRegistry.Register("rpromote", delegate(string[] args)
			{
				this.RemoteCommand(CommandClass.group, SoL.Networking.SolServer.CommandType.raidpromote, "character", args);
			}, "Promotes the group containing the specified player to lead group.", "Promotes the group containing the specified player to lead group. You must be raid leader to perform this action. (Usage: /rpromote [player name])", null);
			CommandRegistry.Register("rkick", delegate(string[] args)
			{
				this.RemoteCommand(CommandClass.group, SoL.Networking.SolServer.CommandType.raidkick, "character", args);
			}, "Kicks the group containing the specified player from the raid.", "Kicks the group containing the specified player from the raid. You must be raid leader to perform this action. (Usage: /rkick [player name])", null);
			CommandRegistry.Register("rdisband", delegate(string[] args)
			{
				this.RemoteCommand(CommandClass.group, SoL.Networking.SolServer.CommandType.raiddisband, "Receiver", args);
			}, "Disbands the current raid.", "Disbands the current raid. You must be raid leader to perform this action. Individual groups will not be disbanded, only the raid itself. (Usage: /rdisband)", null);
			CommandRegistry.Register("reloadchatcolors", delegate(string[] args)
			{
				MessageTypeExtensions.ReloadCustomColors();
			}, "Reload custom chat colors in chatColors.json", null, null);
			CommandRegistry.Register("resetskybox", delegate(string[] args)
			{
				ISkyDomeController skyDomeController = SkyDomeManager.SkyDomeController;
				if (skyDomeController == null)
				{
					return;
				}
				skyDomeController.ResetSkybox();
			}, "reset the skybox", null, null);
			QACommands.RegisterQACommands();
		}

		// Token: 0x06004ABD RID: 19133 RVA: 0x001B6B10 File Offset: 0x001B4D10
		private void RemoteCommand(CommandClass cmdClass, SoL.Networking.SolServer.CommandType cmdType, string argName, string[] args)
		{
			SolServerCommand solServerCommand = cmdClass.NewCommand(cmdType);
			solServerCommand.Args.Add(argName, string.Join(" ", args));
			solServerCommand.Send();
		}

		// Token: 0x06004ABE RID: 19134 RVA: 0x00072619 File Offset: 0x00070819
		public void UpdateChatListWhenReady()
		{
			if (!this.m_chatList.IsInitialized)
			{
				this.m_chatList.Initialized += this.UpdateChatListWhenReady;
				return;
			}
			this.UpdateChatList(this.m_tabController.ActiveTab);
		}

		// Token: 0x06004ABF RID: 19135 RVA: 0x001B6B44 File Offset: 0x001B4D44
		public void UpdateChatList(ChatTab tab)
		{
			if (((tab != null) ? tab.Queue : null) == null)
			{
				this.m_chatList.UpdateItems(this, Array.Empty<ChatMessage>());
				return;
			}
			List<ChatMessage> fromPool = StaticListPool<ChatMessage>.GetFromPool();
			MessageType currentMessageTypeFilter = tab.CurrentMessageTypeFilter;
			for (int i = 0; i < tab.Queue.Count; i++)
			{
				ChatMessage messageAtIndex = tab.Queue.GetMessageAtIndex(i);
				if (messageAtIndex != null)
				{
					MessageType messageType = messageAtIndex.Type;
					if (messageType.HasBitFlag(MessageType.WarlordSong))
					{
						if (!currentMessageTypeFilter.HasBitFlag(MessageType.WarlordSong))
						{
							goto IL_7F;
						}
						messageType &= ~MessageType.WarlordSong;
					}
					if (currentMessageTypeFilter.HasBitFlag(messageType))
					{
						fromPool.Add(messageAtIndex);
					}
				}
				IL_7F:;
			}
			this.m_chatList.UpdateItems(this, fromPool);
			StaticListPool<ChatMessage>.ReturnToPool(fromPool);
		}

		// Token: 0x0400454D RID: 17741
		[SerializeField]
		private ChatTabController m_tabController;

		// Token: 0x0400454E RID: 17742
		[SerializeField]
		private ChatList m_chatList;

		// Token: 0x0400454F RID: 17743
		[SerializeField]
		private Image m_contentArea;

		// Token: 0x04004550 RID: 17744
		[SerializeField]
		private TextMeshProUGUI m_noTabOpenLabel;

		// Token: 0x04004551 RID: 17745
		[SerializeField]
		private Image m_newMessageAlert;

		// Token: 0x04004552 RID: 17746
		[SerializeField]
		private Image m_submissionArea;

		// Token: 0x04004553 RID: 17747
		[SerializeField]
		private TextMeshProUGUI m_prompt;

		// Token: 0x04004554 RID: 17748
		[SerializeField]
		private SolTMP_InputField m_input;

		// Token: 0x04004555 RID: 17749
		[SerializeField]
		private SolButton m_addTabButton;

		// Token: 0x04004556 RID: 17750
		[SerializeField]
		private SolButton m_settingsButton;

		// Token: 0x04004557 RID: 17751
		[SerializeField]
		private SolButton m_toBottomButton;

		// Token: 0x04004558 RID: 17752
		[SerializeField]
		private Image m_settingsPanel;

		// Token: 0x04004559 RID: 17753
		[SerializeField]
		private SolToggle m_showTimestamps;

		// Token: 0x0400455A RID: 17754
		[SerializeField]
		private TMP_Dropdown m_mode;

		// Token: 0x0400455B RID: 17755
		[SerializeField]
		private ChatTabFilterList m_filterList;

		// Token: 0x0400455C RID: 17756
		[SerializeField]
		private Slider m_opacitySettingSlider;

		// Token: 0x0400455D RID: 17757
		[SerializeField]
		private ChatWindowSettings m_defaultSettings;

		// Token: 0x0400455F RID: 17759
		private bool m_ignoreModeChanges;

		// Token: 0x04004560 RID: 17760
		private bool m_skipSettingsSaveOnDestroy;

		// Token: 0x04004561 RID: 17761
		public bool ShouldLoadFromSettings = true;

		// Token: 0x04004562 RID: 17762
		public Vector3 CreateAtPosition = Vector3.zero;

		// Token: 0x04004563 RID: 17763
		private string m_settingsPrefsKey;

		// Token: 0x04004564 RID: 17764
		private ChatWindowSettings m_currentSettings;

		// Token: 0x04004565 RID: 17765
		private bool m_ignoreSettingsChanges;

		// Token: 0x04004566 RID: 17766
		private DateTime m_nextSettingsSync = DateTime.MinValue;

		// Token: 0x04004567 RID: 17767
		private readonly TimeSpan m_settingsSyncInterval = TimeSpan.FromMinutes(1.0);

		// Token: 0x04004568 RID: 17768
		public const string kSettingsPrefsKeySuffix = "_SettingsV2";

		// Token: 0x04004569 RID: 17769
		private const string kChatWindowCountPref = "ChatWindowCount";

		// Token: 0x0400456A RID: 17770
		private const int kMaxHistory = 20;

		// Token: 0x0400456B RID: 17771
		private int m_historyIndex;

		// Token: 0x0400456C RID: 17772
		private readonly List<string> m_history = new List<string>();

		// Token: 0x0400456D RID: 17773
		private bool m_wasFocusedLastFrame;

		// Token: 0x0400456E RID: 17774
		private int m_focusFrame;

		// Token: 0x0400456F RID: 17775
		private int m_scrollToBottomFrameDelay = -1;

		// Token: 0x04004570 RID: 17776
		private static readonly List<ValueTuple<BaseArchetype, int>> m_archetypeStubs = new List<ValueTuple<BaseArchetype, int>>();

		// Token: 0x04004571 RID: 17777
		private static readonly List<ValueTuple<ArchetypeInstance, int>> m_instanceStubs = new List<ValueTuple<ArchetypeInstance, int>>();

		// Token: 0x04004572 RID: 17778
		private string m_tellTarget;

		// Token: 0x04004573 RID: 17779
		private static readonly Regex[] m_tellPatterns = new Regex[]
		{
			new Regex("^\\/t (\\w+) $", RegexOptions.IgnoreCase | RegexOptions.Compiled),
			new Regex("^\\/tell (\\w+) $", RegexOptions.IgnoreCase | RegexOptions.Compiled)
		};

		// Token: 0x04004574 RID: 17780
		private RectTransform m_inputRect;

		// Token: 0x04004575 RID: 17781
		private static char[] m_delimiterArray = new char[]
		{
			' '
		};

		// Token: 0x04004576 RID: 17782
		private static string m_commandListOutput = null;
	}
}
