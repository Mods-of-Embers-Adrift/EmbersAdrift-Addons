using System;
using System.Collections.Generic;
using RootMotion.FinalIK;
using SoL.Game.Animation;
using SoL.Game.EffectSystem;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Pooling;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UMA;
using UMA.CharacterSystem;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.Login.Client.Selection
{
	// Token: 0x02000B45 RID: 2885
	public class SelectionDirector : MonoBehaviour
	{
		// Token: 0x170014C5 RID: 5317
		// (get) Token: 0x060058BF RID: 22719 RVA: 0x0007B59C File Offset: 0x0007979C
		// (set) Token: 0x060058C0 RID: 22720 RVA: 0x0007B5A3 File Offset: 0x000797A3
		public static int MaxCharactersDisplayed { get; private set; } = 5;

		// Token: 0x170014C6 RID: 5318
		// (get) Token: 0x060058C1 RID: 22721 RVA: 0x0007B5AB File Offset: 0x000797AB
		public bool IsStageActive
		{
			get
			{
				return LoginController.Instance != null && LoginController.Instance.Stage == LoginStageType.CharacterSelection;
			}
		}

		// Token: 0x170014C7 RID: 5319
		// (get) Token: 0x060058C2 RID: 22722 RVA: 0x0007B5C9 File Offset: 0x000797C9
		public LoginStageCharacterSelection SelectionStage
		{
			get
			{
				return this.m_selectionStage;
			}
		}

		// Token: 0x060058C3 RID: 22723 RVA: 0x001E74A0 File Offset: 0x001E56A0
		private void Awake()
		{
			if (SelectionDirector.Instance != null)
			{
				UnityEngine.Object.Destroy(this);
				return;
			}
			SelectionDirector.Instance = this;
			SelectionDirector.MaxCharactersDisplayed = this.m_selectionData.Length;
			this.m_selectionStage.OnStageEnter += this.MSelectionStageOnStageEnter;
			this.m_selectionStage.OnStageRefresh += this.MSelectionStageOnRefreshStage;
			this.m_positionIndexMap = new Dictionary<int, SelectionDirector.SelectionData>();
			for (int i = 0; i < this.m_selectionData.Length; i++)
			{
				this.m_selectionData[i].Init(this, i);
				this.m_positionIndexMap.Add(this.m_selectionData[i].PositionIndex, this.m_selectionData[i]);
			}
			for (int j = 0; j < this.m_characterSelectButtons.Length; j++)
			{
				this.m_characterSelectButtons[j].Init(this, -1, null);
			}
			BackgroundBlockerSizeFitter componentInChildren = this.m_renameCharacterDialog.gameObject.GetComponentInChildren<BackgroundBlockerSizeFitter>();
			if (componentInChildren)
			{
				componentInChildren.enabled = true;
			}
			componentInChildren = this.m_setPrimaryCharactersDialog.gameObject.GetComponentInChildren<BackgroundBlockerSizeFitter>();
			if (componentInChildren)
			{
				componentInChildren.enabled = true;
			}
			if (this.m_setActiveButton)
			{
				this.m_setActiveButton.onClick.AddListener(new UnityAction(this.OpenSetPrimaryDialog));
			}
		}

		// Token: 0x060058C4 RID: 22724 RVA: 0x001E75DC File Offset: 0x001E57DC
		private void Update()
		{
			if (this.IsStageActive && this.m_selectionStage.AllowSelectionChange)
			{
				if (Input.GetKeyDown(KeyCode.LeftArrow))
				{
					this.SelectNextCharacter(false);
					return;
				}
				if (Input.GetKeyDown(KeyCode.RightArrow))
				{
					this.SelectNextCharacter(true);
					return;
				}
				if (Input.GetKeyDown(KeyCode.DownArrow))
				{
					this.SelectNextCharacterUpDown(true);
					return;
				}
				if (Input.GetKeyDown(KeyCode.UpArrow))
				{
					this.SelectNextCharacterUpDown(false);
				}
			}
		}

		// Token: 0x060058C5 RID: 22725 RVA: 0x001E7650 File Offset: 0x001E5850
		private void OnDestroy()
		{
			this.m_selectionStage.OnStageEnter -= this.MSelectionStageOnStageEnter;
			this.m_selectionStage.OnStageRefresh -= this.MSelectionStageOnRefreshStage;
			for (int i = 0; i < this.m_selectionData.Length; i++)
			{
				if (this.m_selectionData[i] != null)
				{
					this.m_selectionData[i].ReturnActiveHandheldItemsToPool();
				}
			}
			if (this.m_setActiveButton)
			{
				this.m_setActiveButton.onClick.RemoveListener(new UnityAction(this.OpenSetPrimaryDialog));
			}
		}

		// Token: 0x060058C6 RID: 22726 RVA: 0x0007B5D1 File Offset: 0x000797D1
		private void MSelectionStageOnStageEnter()
		{
			this.RefreshCharacters();
			this.m_renameCharacterDialog.Hide(true);
			this.m_setPrimaryCharactersDialog.Hide(true);
		}

		// Token: 0x060058C7 RID: 22727 RVA: 0x0007B5F1 File Offset: 0x000797F1
		private void MSelectionStageOnRefreshStage()
		{
			this.RefreshCharacters();
		}

		// Token: 0x060058C8 RID: 22728 RVA: 0x001E76E0 File Offset: 0x001E58E0
		public void RefreshCharacterButtons()
		{
			for (int i = 0; i < this.m_selectionData.Length; i++)
			{
				this.m_selectionData[i].RefreshButton();
			}
			this.RefreshCharacterButtonList();
		}

		// Token: 0x060058C9 RID: 22729 RVA: 0x001E7714 File Offset: 0x001E5914
		private void RefreshCharacterButtonList()
		{
			for (int i = 0; i < this.m_characterSelectButtons.Length; i++)
			{
				CharacterRecord character = (i < SessionData.Characters.Length) ? SessionData.Characters[i] : null;
				this.m_characterSelectButtons[i].SetCharacter(character);
			}
		}

		// Token: 0x060058CA RID: 22730 RVA: 0x001E7758 File Offset: 0x001E5958
		private void RefreshCharacters()
		{
			this.m_canSetActiveCharacter = false;
			this.m_selectionStage.ParentEnterWorldButton(null, null);
			if (SessionData.Characters == null || SessionData.Characters.Length == 0)
			{
				SessionData.SelectCharacter(null);
				for (int i = 0; i < this.m_selectionData.Length; i++)
				{
					this.m_selectionData[i].Record = null;
				}
				for (int j = 0; j < this.m_characterSelectButtons.Length; j++)
				{
					this.m_characterSelectButtons[j].SetCharacter(null);
				}
				return;
			}
			for (int k = 0; k < this.m_selectionData.Length; k++)
			{
				CharacterRecord record = null;
				for (int l = 0; l < SessionData.Characters.Length; l++)
				{
					if (SessionData.Characters[l].SelectionPositionIndex == this.m_selectionData[k].PositionIndex)
					{
						record = SessionData.Characters[l];
					}
					this.m_canSetActiveCharacter = (this.m_canSetActiveCharacter || SessionData.CharacterCanBeSetAsActive(record));
				}
				this.m_selectionData[k].Record = record;
			}
			this.RefreshCharacterButtonList();
			if (SessionData.Characters != null && SessionData.Characters.Length != 0)
			{
				if (SessionData.User != null && !SessionData.User.IsSubscriber() && SessionData.User.ActiveCharacters != null && SessionData.User.ActiveCharacters.Length == 1)
				{
					this.SelectCharacterByCharacterId(SessionData.User.ActiveCharacters[0]);
				}
				else if (SessionData.Characters.Length == 1)
				{
					this.SelectCharacterByCharacterRecord(SessionData.Characters[0]);
				}
				else if (SessionData.SelectLastCharacter)
				{
					this.SelectLastCharacter();
				}
				else if (!string.IsNullOrEmpty(SessionData.LastCreatedEditedCharacterId))
				{
					this.SelectCharacterByCharacterId(SessionData.LastCreatedEditedCharacterId);
				}
				else
				{
					bool flag = SessionData.SelectedCharacter != null && this.SelectCharacterByCharacterId(SessionData.SelectedCharacter.Id);
					if (!flag)
					{
						string @string = PlayerPrefs.GetString("LastPlayedCharacter", string.Empty);
						if (!string.IsNullOrEmpty(@string))
						{
							flag = this.SelectCharacterByCharacterName(@string);
						}
					}
					if (!flag)
					{
						this.SelectLastCharacter();
					}
				}
				SessionData.SelectLastCharacter = false;
				SessionData.LastCreatedEditedCharacterId = null;
			}
			this.m_setActiveButton.interactable = this.m_canSetActiveCharacter;
			this.m_setActiveButton.gameObject.SetActive(this.m_canSetActiveCharacter);
		}

		// Token: 0x060058CB RID: 22731 RVA: 0x001E796C File Offset: 0x001E5B6C
		private void SelectLastCharacter()
		{
			CharacterRecord characterRecord = null;
			if (SessionData.Characters != null)
			{
				for (int i = 0; i < SessionData.Characters.Length; i++)
				{
					if (SessionData.Characters[i] != null && SessionData.CharacterIsActive(SessionData.Characters[i]) && (characterRecord == null || characterRecord.Created < SessionData.Characters[i].Created))
					{
						characterRecord = SessionData.Characters[i];
					}
				}
			}
			this.SelectCharacterByCharacterRecord(characterRecord);
		}

		// Token: 0x060058CC RID: 22732 RVA: 0x001E79D8 File Offset: 0x001E5BD8
		private void SelectNextCharacter(bool movingRight)
		{
			if (SessionData.Characters.Length <= 1 || SessionData.SelectedCharacter == null)
			{
				return;
			}
			int num = -1;
			for (int i = 0; i < this.m_selectionData.Length; i++)
			{
				if (this.m_selectionData[i].Record != null && this.m_selectionData[i].Record.Id == SessionData.SelectedCharacter.Id)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				return;
			}
			int num2 = num;
			num2 += (movingRight ? 1 : -1);
			for (int j = 0; j < this.m_selectionData.Length; j++)
			{
				if (num2 < 0)
				{
					num2 = SelectionDirector.MaxCharactersDisplayed - 1;
				}
				else if (num2 >= SelectionDirector.MaxCharactersDisplayed)
				{
					num2 = 0;
				}
				if (this.m_selectionData[num2].Record != null && SessionData.CharacterIsActive(this.m_selectionData[num2].Record))
				{
					this.SelectCharacterByPositionIndex(this.m_selectionData[num2].PositionIndex);
					return;
				}
				num2 += (movingRight ? 1 : -1);
			}
		}

		// Token: 0x060058CD RID: 22733 RVA: 0x001E7AC0 File Offset: 0x001E5CC0
		private void SelectNextCharacterUpDown(bool movingDown)
		{
			if (SessionData.Characters.Length <= 1 || SessionData.SelectedCharacter == null)
			{
				return;
			}
			int num = -1;
			for (int i = 0; i < SessionData.Characters.Length; i++)
			{
				if (SessionData.Characters[i] != null && SessionData.Characters[i].Id == SessionData.SelectedCharacter.Id)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				return;
			}
			int num2 = num;
			num2 += (movingDown ? 1 : -1);
			for (int j = 0; j < SessionData.Characters.Length; j++)
			{
				if (num2 < 0)
				{
					num2 = SessionData.Characters.Length - 1;
				}
				else if (num2 >= SessionData.Characters.Length)
				{
					num2 = 0;
				}
				if (SessionData.CharacterIsActive(SessionData.Characters[num2]))
				{
					this.SelectCharacterByCharacterRecord(SessionData.Characters[num2]);
					return;
				}
				num2 += (movingDown ? 1 : -1);
			}
		}

		// Token: 0x060058CE RID: 22734 RVA: 0x001E7B84 File Offset: 0x001E5D84
		private void RefreshPortraits()
		{
			for (int i = 0; i < this.m_selectionData.Length; i++)
			{
				this.m_selectionData[i].RefreshPortrait();
			}
			for (int j = 0; j < this.m_characterSelectButtons.Length; j++)
			{
				this.m_characterSelectButtons[j].RefreshPortrait();
			}
		}

		// Token: 0x060058CF RID: 22735 RVA: 0x001E7BD4 File Offset: 0x001E5DD4
		public bool SelectCharacterByCharacterRecord(CharacterRecord record)
		{
			if (record != null && SessionData.User != null && SessionData.User.CharacterIsSelectable(record))
			{
				SessionData.SelectCharacter(record);
				bool flag = false;
				for (int i = 0; i < this.m_selectionData.Length; i++)
				{
					bool flag2 = this.m_selectionData[i].Record != null && this.m_selectionData[i].Record.Id == record.Id;
					this.m_selectionData[i].SetSelected(flag2);
					flag = (flag || flag2);
				}
				for (int j = 0; j < this.m_characterSelectButtons.Length; j++)
				{
					bool flag3 = this.m_characterSelectButtons[j].MatchesId(record.Id);
					this.m_characterSelectButtons[j].SetSelected(flag3);
					flag = (flag || flag3);
				}
				return flag;
			}
			SessionData.SelectCharacter(null);
			return false;
		}

		// Token: 0x060058D0 RID: 22736 RVA: 0x001E7CA4 File Offset: 0x001E5EA4
		public bool SelectCharacterByPositionIndex(int index)
		{
			if (index >= this.m_selectionData.Length)
			{
				return false;
			}
			for (int i = 0; i < this.m_selectionData.Length; i++)
			{
				if (index == this.m_selectionData[i].PositionIndex)
				{
					return this.SelectCharacterByCharacterRecord(this.m_selectionData[i].Record);
				}
			}
			return false;
		}

		// Token: 0x060058D1 RID: 22737 RVA: 0x001E7CF8 File Offset: 0x001E5EF8
		private bool SelectCharacterByCharacterName(string characterName)
		{
			if (!string.IsNullOrEmpty(characterName) && SessionData.Characters != null)
			{
				for (int i = 0; i < SessionData.Characters.Length; i++)
				{
					if (SessionData.Characters[i] != null && SessionData.Characters[i].Name.Equals(characterName, StringComparison.InvariantCultureIgnoreCase))
					{
						return this.SelectCharacterByCharacterRecord(SessionData.Characters[i]);
					}
				}
			}
			return false;
		}

		// Token: 0x060058D2 RID: 22738 RVA: 0x001E7D54 File Offset: 0x001E5F54
		private bool SelectCharacterByCharacterId(string characterId)
		{
			if (!string.IsNullOrEmpty(characterId) && SessionData.Characters != null)
			{
				for (int i = 0; i < SessionData.Characters.Length; i++)
				{
					if (SessionData.Characters[i] != null && SessionData.Characters[i].Id == characterId)
					{
						return this.SelectCharacterByCharacterRecord(SessionData.Characters[i]);
					}
				}
			}
			return false;
		}

		// Token: 0x060058D3 RID: 22739 RVA: 0x0007B5F9 File Offset: 0x000797F9
		private void DeleteCharacterByPositionIndex(int index)
		{
			if (index >= this.m_selectionData.Length || this.m_selectionData[index].Record == null)
			{
				return;
			}
			LoginApiManager.DeleteCharacter(this.m_selectionData[index].Record);
		}

		// Token: 0x060058D4 RID: 22740 RVA: 0x001E7DB0 File Offset: 0x001E5FB0
		public void MoveCharacterLeft(CharacterRecord record)
		{
			if (record == null)
			{
				return;
			}
			SelectionDirector.SelectionData selectionData;
			if (!this.m_positionIndexMap.TryGetValue(record.SelectionPositionIndex, out selectionData))
			{
				return;
			}
			int num = selectionData.SelectionIndex - 1;
			if (num < 0)
			{
				num = SelectionDirector.MaxCharactersDisplayed - 1;
			}
			this.UpdateCharacterPosition(record, this.m_selectionData[num].PositionIndex);
		}

		// Token: 0x060058D5 RID: 22741 RVA: 0x001E7E00 File Offset: 0x001E6000
		public void MoveCharacterRight(CharacterRecord record)
		{
			if (record == null)
			{
				return;
			}
			SelectionDirector.SelectionData selectionData;
			if (!this.m_positionIndexMap.TryGetValue(record.SelectionPositionIndex, out selectionData))
			{
				return;
			}
			int num = selectionData.SelectionIndex + 1;
			if (num >= SelectionDirector.MaxCharactersDisplayed)
			{
				num = 0;
			}
			this.UpdateCharacterPosition(record, this.m_selectionData[num].PositionIndex);
		}

		// Token: 0x060058D6 RID: 22742 RVA: 0x001E7E50 File Offset: 0x001E6050
		private void UpdateCharacterPosition(CharacterRecord record, int newIndex)
		{
			if (record == null)
			{
				return;
			}
			if (this.m_dirtyRecords == null)
			{
				this.m_dirtyRecords = new List<CharacterRecord>();
			}
			this.m_dirtyRecords.Clear();
			bool flag = false;
			for (int i = 0; i < SessionData.Characters.Length; i++)
			{
				if (SessionData.Characters[i] != null && SessionData.Characters[i].Id != record.Id && SessionData.Characters[i].SelectionPositionIndex == newIndex)
				{
					flag = this.ShouldSort(SessionData.Characters[i], record.SelectionPositionIndex);
					SessionData.Characters[i].SelectionPositionIndex = record.SelectionPositionIndex;
					this.m_dirtyRecords.Add(SessionData.Characters[i]);
					break;
				}
			}
			flag = (flag || this.ShouldSort(record, newIndex));
			record.SelectionPositionIndex = newIndex;
			this.m_dirtyRecords.Add(record);
			LoginApiManager.UpdateSelectionIndexes(this.m_dirtyRecords);
			if (flag)
			{
				SessionData.SortCharacters();
			}
			this.RefreshCharacters();
		}

		// Token: 0x060058D7 RID: 22743 RVA: 0x001E7F3C File Offset: 0x001E613C
		private bool ShouldSort(CharacterRecord record, int newIndex)
		{
			if (record == null)
			{
				return false;
			}
			int selectionPositionIndex = record.SelectionPositionIndex;
			return (selectionPositionIndex < 0 && newIndex >= 0) || (selectionPositionIndex >= 0 && newIndex < 0);
		}

		// Token: 0x060058D8 RID: 22744 RVA: 0x001E7F6C File Offset: 0x001E616C
		public void DeleteCharacterRequest(CharacterRecord record)
		{
			this.m_pendingDelete = record;
			DialogOptions opts = new DialogOptions
			{
				AllowDragging = false,
				BlockInteractions = true,
				BackgroundBlockerColor = new Color(0f, 0f, 0f, 0.9f),
				Text = "Are you sure you want to delete " + record.Name + "?",
				ConfirmationText = "Yes",
				CancelText = "No",
				ShowCloseButton = false,
				Title = "Delete Character",
				Callback = new Action<bool, object>(this.DeleteCharacterCallback)
			};
			ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
		}

		// Token: 0x060058D9 RID: 22745 RVA: 0x0007B628 File Offset: 0x00079828
		private void DeleteCharacterCallback(bool arg1, object arg2)
		{
			if (arg1 && this.m_pendingDelete != null)
			{
				LoginApiManager.DeleteCharacter(this.m_pendingDelete);
			}
			this.m_pendingDelete = null;
		}

		// Token: 0x060058DA RID: 22746 RVA: 0x0007B647 File Offset: 0x00079847
		public void OpenSetPrimaryDialog()
		{
			if (this.m_canSetActiveCharacter && this.m_setPrimaryCharactersDialog)
			{
				this.m_setPrimaryCharactersDialog.Init();
			}
		}

		// Token: 0x060058DB RID: 22747 RVA: 0x001E8028 File Offset: 0x001E6228
		public void RenameClicked(CharacterRecord record)
		{
			if (record != null && record.RequiresRenaming != null && record.RequiresRenaming.Value && this.m_renameCharacterDialog)
			{
				this.m_renameCharacterDialog.Init(record);
			}
		}

		// Token: 0x060058DC RID: 22748 RVA: 0x001E8074 File Offset: 0x001E6274
		public string ContextMenuForCharacter(CharacterRecord record, bool showMoveLeftRight)
		{
			if (record == null)
			{
				return null;
			}
			if (SessionData.CharacterCanBeSetAsActive(record))
			{
				ContextMenuUI.AddContextAction("Set Active Character", true, new Action(this.OpenSetPrimaryDialog), null, null);
				return record.Name;
			}
			if (record.RequiresRenaming != null && record.RequiresRenaming.Value)
			{
				ContextMenuUI.AddContextAction("Rename Character", true, delegate()
				{
					if (this.m_renameCharacterDialog)
					{
						this.m_renameCharacterDialog.Init(record);
					}
				}, null, null);
			}
			bool flag = SessionData.User.CharacterIsSelectable(record);
			if (flag)
			{
				ContextMenuUI.AddContextAction("<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>  Change Portrait", true, delegate()
				{
					ClientGameManager.UIManager.SelectPortraitWindow.Init(record);
				}, null, null);
				ContextMenuUI.AddContextAction("<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>  Edit Appearance", true, delegate()
				{
					if (LoginController.Instance != null && LoginController.Instance.CreationDirector != null)
					{
						LoginController.Instance.CreationDirector.LoadCharacter(record);
						LoginController.Instance.SetStage(LoginStageType.CharacterCreation);
					}
				}, null, null);
			}
			if (showMoveLeftRight)
			{
				ContextMenuUI.AddContextAction("<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>  Move Left", true, delegate()
				{
					this.MoveCharacterLeft(record);
				}, null, null);
				ContextMenuUI.AddContextAction("<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>  Move Right", true, delegate()
				{
					this.MoveCharacterRight(record);
				}, null, null);
			}
			if (flag)
			{
				ContextMenuUI.AddContextAction("<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font>  Delete Character", true, delegate()
				{
					this.DeleteCharacterRequest(record);
				}, null, null);
			}
			if (showMoveLeftRight)
			{
				for (int i = 0; i < SessionData.Characters.Length; i++)
				{
					CharacterRecord swapRecord = SessionData.Characters[i];
					if (swapRecord != null && swapRecord.SelectionPositionIndex < 0)
					{
						ContextMenuUI.AddContextAction("<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font> Swap with " + swapRecord.Name, true, delegate()
						{
							this.UpdateCharacterPosition(swapRecord, record.SelectionPositionIndex);
							this.SelectCharacterByCharacterRecord(swapRecord);
						}, null, null);
					}
				}
			}
			if (record.SelectionPositionIndex < 0)
			{
				List<int> fromPool = StaticListPool<int>.GetFromPool();
				for (int j = 0; j < SessionData.Characters.Length; j++)
				{
					if (SessionData.Characters[j] != null && SessionData.Characters[j].SelectionPositionIndex >= 0)
					{
						fromPool.Add(SessionData.Characters[j].SelectionPositionIndex);
					}
				}
				for (int k = 0; k < SelectionDirector.MaxCharactersDisplayed; k++)
				{
					int characterIndex = k;
					if (!fromPool.Contains(characterIndex))
					{
						ContextMenuUI.AddContextAction("<font=\"Font Awesome 5 Free-Solid-900 SDF\"></font> Add to Ember Ring.", true, delegate()
						{
							this.UpdateCharacterPosition(record, characterIndex);
							this.SelectCharacterByCharacterRecord(record);
						}, null, null);
						break;
					}
				}
				StaticListPool<int>.ReturnToPool(fromPool);
			}
			return record.Name;
		}

		// Token: 0x060058DD RID: 22749 RVA: 0x0007B669 File Offset: 0x00079869
		public void SetCharacterPortraitId(CharacterRecord record, UniqueId portraitId)
		{
			if (record == null)
			{
				return;
			}
			record.Settings.PortraitId = portraitId;
			this.RefreshPortraits();
			LoginApiManager.UpdatePortrait(record);
		}

		// Token: 0x060058DE RID: 22750 RVA: 0x001E82E0 File Offset: 0x001E64E0
		public void RefreshCharacterNames()
		{
			for (int i = 0; i < this.m_selectionData.Length; i++)
			{
				this.m_selectionData[i].RefreshName();
			}
			this.m_selectionStage.RefreshButtons();
		}

		// Token: 0x04004E1C RID: 19996
		public static SelectionDirector Instance = null;

		// Token: 0x04004E1E RID: 19998
		public const string kLastPlayedKey = "LastPlayedCharacter";

		// Token: 0x04004E1F RID: 19999
		[SerializeField]
		private LoginStageCharacterSelection m_selectionStage;

		// Token: 0x04004E20 RID: 20000
		[SerializeField]
		private GameObject m_selectionPrefab;

		// Token: 0x04004E21 RID: 20001
		[SerializeField]
		private SelectionDirector.SelectionData[] m_selectionData;

		// Token: 0x04004E22 RID: 20002
		[SerializeField]
		private CharacterSelectButton[] m_characterSelectButtons;

		// Token: 0x04004E23 RID: 20003
		[SerializeField]
		private SelectionDirector.SelectionData[] m_disabledSelectionData;

		// Token: 0x04004E24 RID: 20004
		[SerializeField]
		private RenameCharacterDialog m_renameCharacterDialog;

		// Token: 0x04004E25 RID: 20005
		[SerializeField]
		private SelectActiveCharactersDialog m_setPrimaryCharactersDialog;

		// Token: 0x04004E26 RID: 20006
		[SerializeField]
		private SolButton m_setActiveButton;

		// Token: 0x04004E27 RID: 20007
		private Dictionary<int, SelectionDirector.SelectionData> m_positionIndexMap;

		// Token: 0x04004E28 RID: 20008
		private bool m_canSetActiveCharacter;

		// Token: 0x04004E29 RID: 20009
		private List<CharacterRecord> m_dirtyRecords;

		// Token: 0x04004E2A RID: 20010
		private CharacterRecord m_pendingDelete;

		// Token: 0x02000B46 RID: 2886
		[Serializable]
		private class SelectionData
		{
			// Token: 0x170014C8 RID: 5320
			// (get) Token: 0x060058E1 RID: 22753 RVA: 0x0007B695 File Offset: 0x00079895
			private bool m_showIkStuff
			{
				get
				{
					return this.m_buttEffectorTarget != null;
				}
			}

			// Token: 0x170014C9 RID: 5321
			// (get) Token: 0x060058E2 RID: 22754 RVA: 0x0007B6A3 File Offset: 0x000798A3
			public int PositionIndex
			{
				get
				{
					return this.m_positionIndex;
				}
			}

			// Token: 0x170014CA RID: 5322
			// (get) Token: 0x060058E3 RID: 22755 RVA: 0x0007B6AB File Offset: 0x000798AB
			public int SelectionIndex
			{
				get
				{
					return this.m_selectionIndex;
				}
			}

			// Token: 0x170014CB RID: 5323
			// (get) Token: 0x060058E4 RID: 22756 RVA: 0x0007B6B3 File Offset: 0x000798B3
			// (set) Token: 0x060058E5 RID: 22757 RVA: 0x0007B6BB File Offset: 0x000798BB
			public CharacterRecord Record
			{
				get
				{
					return this.m_record;
				}
				set
				{
					if (this.m_record == value)
					{
						return;
					}
					this.m_record = value;
					this.RefreshAvatar();
				}
			}

			// Token: 0x060058E6 RID: 22758 RVA: 0x0007B6D4 File Offset: 0x000798D4
			public void Init(SelectionDirector director, int selectionIndex)
			{
				this.m_director = director;
				this.m_selectionIndex = selectionIndex;
				this.m_button.Init(this.m_director, this.m_positionIndex, this.m_parent);
				this.m_button.SetCharacter(null);
			}

			// Token: 0x060058E7 RID: 22759 RVA: 0x001E8318 File Offset: 0x001E6518
			public void ReturnActiveHandheldItemsToPool()
			{
				if (this.m_activeHandheldItems != null)
				{
					for (int i = 0; i < this.m_activeHandheldItems.Count; i++)
					{
						if (this.m_activeHandheldItems[i])
						{
							this.m_activeHandheldItems[i].ReturnToPool();
						}
					}
					this.m_activeHandheldItems.Clear();
				}
			}

			// Token: 0x060058E8 RID: 22760 RVA: 0x0007B70D File Offset: 0x0007990D
			public void SetSelected(bool isSelected)
			{
				this.m_button.SetSelected(isSelected);
			}

			// Token: 0x060058E9 RID: 22761 RVA: 0x001E8374 File Offset: 0x001E6574
			private void RefreshAvatar()
			{
				this.m_button.SetCharacter(this.m_record);
				if (this.m_dca != null)
				{
					UnityEngine.Object.Destroy(this.m_dca.gameObject);
				}
				this.ReturnActiveHandheldItemsToPool();
				if (this.m_record == null)
				{
					return;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_director.m_selectionPrefab, this.m_parent.gameObject.transform);
				this.m_dca = gameObject.GetComponent<DynamicCharacterAvatar>();
				this.m_dca.CharacterCreated.AddListener(new UnityAction<UMAData>(this.OnUmaCreated));
				if (this.m_buttEffectorTarget)
				{
					this.m_ikController = this.m_dca.gameObject.AddComponent<UMAIKController>();
				}
				List<ArchetypeInstance> list = null;
				ContainerRecord containerRecord;
				if (this.m_record.Storage != null && this.m_record.Storage.TryGetValue(ContainerType.Equipment, out containerRecord))
				{
					list = containerRecord.Instances;
					if (this.m_record.Settings.HideHelm && list != null)
					{
						for (int i = 0; i < list.Count; i++)
						{
							EquipableItem equipableItem;
							if (InternalGameDatabase.Archetypes.TryGetAsType<EquipableItem>(list[i].ArchetypeId, out equipableItem) && equipableItem.Type == EquipmentType.Head)
							{
								list.RemoveAt(i);
								break;
							}
						}
					}
				}
				UMAManager.BuildBaseDca(null, this.m_dca, this.m_record.Visuals, list, null);
				IAnimationController component = this.m_dca.GetComponent<IAnimationController>();
				if (component != null)
				{
					if (this.m_animationSet)
					{
						component.SetCurrentStanceId(this.m_animationSet.Id, false);
					}
					component.AssignSex(this.m_record.Visuals.Sex);
					if (this.m_animationSet == null && this.m_animationSequence)
					{
						component.StartSequence(this.m_animationSequence.ClipSequence, null);
						component.PreventIdleTicks = true;
					}
				}
			}

			// Token: 0x060058EA RID: 22762 RVA: 0x0007B71B File Offset: 0x0007991B
			public void RefreshPortrait()
			{
				this.m_button.RefreshPortrait();
			}

			// Token: 0x060058EB RID: 22763 RVA: 0x0007B728 File Offset: 0x00079928
			public void RefreshName()
			{
				this.m_button.RefreshName();
			}

			// Token: 0x060058EC RID: 22764 RVA: 0x0007B735 File Offset: 0x00079935
			public void RefreshButton()
			{
				this.m_button.SetCharacter(this.m_record);
			}

			// Token: 0x060058ED RID: 22765 RVA: 0x001E8544 File Offset: 0x001E6744
			private void OnUmaCreated(UMAData umaData)
			{
				this.m_dca.CharacterCreated.RemoveListener(new UnityAction<UMAData>(this.OnUmaCreated));
				if (umaData.GetRenderer(0) != null)
				{
					this.m_selectableCharacter = this.m_dca.gameObject.GetComponent<SelectableCharacter>();
					if (this.m_selectableCharacter != null)
					{
						this.m_selectableCharacter.Init(this.Record, this.m_director);
					}
					this.m_button.SelectableCharHighlight = this.m_selectableCharacter;
				}
				this.MountItems();
				if (this.m_ikController)
				{
					this.m_ikController.IkSetupCompleteEvent += this.SetEffectors;
					if (this.m_ikController.IkSetupComplete)
					{
						this.SetEffectors();
					}
				}
			}

			// Token: 0x060058EE RID: 22766 RVA: 0x001E8608 File Offset: 0x001E6808
			private void MountItems()
			{
				if (!this.m_dca)
				{
					return;
				}
				Animator component = this.m_dca.gameObject.GetComponent<Animator>();
				if (!component)
				{
					return;
				}
				HumanoidReferencePoints referencePoints = component.GetReferencePoints(this.m_record.Visuals.Sex, GlobalSettings.Values.Uma.CorrectMountPoints);
				ContainerRecord containerRecord;
				if (this.m_record.Storage != null && this.m_record.Storage.TryGetValue(ContainerType.Equipment, out containerRecord))
				{
					EquipmentSlot equipmentSlot = EquipmentSlot.PrimaryWeapon_MainHand;
					EquipmentSlot equipmentSlot2 = EquipmentSlot.PrimaryWeapon_OffHand;
					if (this.m_record.Settings.SecondaryWeaponsActive)
					{
						equipmentSlot = EquipmentSlot.SecondaryWeapon_MainHand;
						equipmentSlot2 = EquipmentSlot.SecondaryWeapon_OffHand;
					}
					int num = (int)(equipmentSlot | equipmentSlot2);
					PooledHandheldItem pooledHandheldItem = null;
					HandheldItemFlags mainHand = HandheldItemFlags.Empty;
					EquipmentSlot slot = EquipmentSlot.None;
					bool alternateAnimationSet = false;
					PooledHandheldItem pooledHandheldItem2 = null;
					HandheldItemFlags offHand = HandheldItemFlags.Empty;
					EquipmentSlot slot2 = EquipmentSlot.None;
					List<ArchetypeInstance> instances = containerRecord.Instances;
					for (int i = 0; i < instances.Count; i++)
					{
						WeaponItem weaponItem;
						if ((instances[i].Index & num) != 0 && InternalGameDatabase.Archetypes.TryGetAsType<WeaponItem>(instances[i].ArchetypeId, out weaponItem))
						{
							PooledHandheldItem handheldItem = weaponItem.GetHandheldItem(instances[i].ItemData.VisualIndex);
							if (handheldItem != null)
							{
								if (instances[i].Index == (int)equipmentSlot)
								{
									pooledHandheldItem = handheldItem;
									mainHand = this.GetHandheldItemFlag(weaponItem);
									slot = (EquipmentSlot)instances[i].Index;
									alternateAnimationSet = ((IHandheldItem)weaponItem).AlternateAnimationSet;
								}
								else if (instances[i].Index == (int)equipmentSlot2)
								{
									pooledHandheldItem2 = handheldItem;
									offHand = this.GetHandheldItemFlag(weaponItem);
									slot2 = (EquipmentSlot)instances[i].Index;
								}
							}
						}
					}
					AnimancerAnimationSet animationSet = GlobalSettings.Values.Animation.HumanoidWeaponAnimationSets.GetAnimationSet(new HandheldFlagConfig
					{
						MainHand = mainHand,
						OffHand = offHand,
						AlternateAnimationSet = alternateAnimationSet
					});
					if (!this.m_bypassWeaponMounting)
					{
						this.MountItem(referencePoints, pooledHandheldItem, slot, animationSet);
						this.MountItem(referencePoints, pooledHandheldItem2, slot2, animationSet);
					}
					EmberStone emberStone;
					if (!this.m_record.Settings.HideEmberStone && this.m_record.EmberStoneData != null && !this.m_record.EmberStoneData.StoneId.IsEmpty && InternalGameDatabase.Archetypes.TryGetAsType<EmberStone>(this.m_record.EmberStoneData.StoneId, out emberStone) && emberStone.HandHeldItem)
					{
						PooledHandheldItem pooledHandheldItem3 = this.MountItem(referencePoints, emberStone.HandHeldItem, EquipmentSlot.EmberStone, animationSet);
						if (pooledHandheldItem3 && pooledHandheldItem3.RendererToAlter && pooledHandheldItem3.RendererToAlter.material)
						{
							EmberStoneFillLevels fillLevel = emberStone.GetFillLevel(this.m_record.EmberStoneData.Count);
							pooledHandheldItem3.RendererToAlter.material.SetColor(ShaderExtensions.kEmissiveColorId, fillLevel.GetEmissiveColor(AlchemyPowerLevel.None));
						}
					}
				}
			}

			// Token: 0x060058EF RID: 22767 RVA: 0x0007B748 File Offset: 0x00079948
			private HandheldItemFlags GetHandheldItemFlag(WeaponItem weaponItem)
			{
				if (!weaponItem)
				{
					return HandheldItemFlags.Empty;
				}
				return ((IHandheldItem)weaponItem).HandheldItemFlag;
			}

			// Token: 0x060058F0 RID: 22768 RVA: 0x001E88F0 File Offset: 0x001E6AF0
			private PooledHandheldItem MountItem(HumanoidReferencePoints referencePoints, PooledHandheldItem pooledHandheldItem, EquipmentSlot slot, AnimancerAnimationSet combatSet)
			{
				PooledHandheldItem pooledHandheldItem2 = null;
				if (pooledHandheldItem)
				{
					pooledHandheldItem2 = pooledHandheldItem.GetPooledInstance<PooledHandheldItem>();
					if (pooledHandheldItem2)
					{
						pooledHandheldItem2.MountItem(new HumanoidReferencePoints?(referencePoints), slot, false, combatSet, null);
						this.m_activeHandheldItems.Add(pooledHandheldItem2);
					}
				}
				return pooledHandheldItem2;
			}

			// Token: 0x060058F1 RID: 22769 RVA: 0x001E8934 File Offset: 0x001E6B34
			private void SetEffectors()
			{
				if (this.m_ikController == null || this.m_dca == null)
				{
					return;
				}
				this.m_ikController.IkSetupCompleteEvent -= this.SetEffectors;
				if (this.m_buttEffectorTarget)
				{
					FullBodyBipedIK component = this.m_dca.gameObject.GetComponent<FullBodyBipedIK>();
					if (component && component.references != null && component.references.spine != null && component.references.spine.Length > 1)
					{
						if (this.m_dynamicButtEffectorTarget == null)
						{
							this.m_dynamicButtEffectorTarget = new GameObject("ButtEffectorTarget_" + this.m_positionIndex.ToString()).transform;
						}
						Transform parent = component.references.spine[0].parent;
						float num = component.references.spine[1].position.y - parent.position.y;
						Vector3 position = this.m_buttEffectorTarget.transform.position;
						position.y += num;
						this.m_dynamicButtEffectorTarget.position = position;
						component.solver.bodyEffector.target = this.m_dynamicButtEffectorTarget;
						component.solver.bodyEffector.positionWeight = 1f;
						component.solver.iterations = this.m_iterations;
						if (this.m_leftHandTarget && this.m_rightHandTarget)
						{
							component.solver.leftHandEffector.target = this.m_leftHandTarget;
							component.solver.leftHandEffector.positionWeight = 1f;
							component.solver.rightHandEffector.target = this.m_rightHandTarget;
							component.solver.rightHandEffector.positionWeight = 1f;
						}
						else if (this.m_disableArmWeight)
						{
							component.solver.leftArmMapping.weight = 0f;
							component.solver.rightArmMapping.weight = 0f;
						}
					}
					GrounderFBBIK component2 = this.m_dca.gameObject.GetComponent<GrounderFBBIK>();
					if (component2)
					{
						component2.solver.heightOffset = this.m_grounderHeightOffset;
					}
				}
			}

			// Token: 0x04004E2B RID: 20011
			private const string kIkGroup = "IK";

			// Token: 0x04004E2C RID: 20012
			[SerializeField]
			private int m_positionIndex;

			// Token: 0x04004E2D RID: 20013
			[SerializeField]
			private CharacterSelectButton m_button;

			// Token: 0x04004E2E RID: 20014
			[SerializeField]
			private GameObject m_parent;

			// Token: 0x04004E2F RID: 20015
			[SerializeField]
			private AnimancerAnimationSet m_animationSet;

			// Token: 0x04004E30 RID: 20016
			[SerializeField]
			private ScriptableAnimationSequence m_animationSequence;

			// Token: 0x04004E31 RID: 20017
			[SerializeField]
			private bool m_bypassWeaponMounting;

			// Token: 0x04004E32 RID: 20018
			[SerializeField]
			private Transform m_buttEffectorTarget;

			// Token: 0x04004E33 RID: 20019
			[SerializeField]
			private int m_iterations;

			// Token: 0x04004E34 RID: 20020
			[SerializeField]
			private float m_grounderHeightOffset;

			// Token: 0x04004E35 RID: 20021
			[SerializeField]
			private bool m_disableArmWeight;

			// Token: 0x04004E36 RID: 20022
			[SerializeField]
			private Transform m_leftHandTarget;

			// Token: 0x04004E37 RID: 20023
			[SerializeField]
			private Transform m_rightHandTarget;

			// Token: 0x04004E38 RID: 20024
			private int m_selectionIndex = -1;

			// Token: 0x04004E39 RID: 20025
			private DynamicCharacterAvatar m_dca;

			// Token: 0x04004E3A RID: 20026
			private SelectionDirector m_director;

			// Token: 0x04004E3B RID: 20027
			private SelectableCharacter m_selectableCharacter;

			// Token: 0x04004E3C RID: 20028
			private List<PooledHandheldItem> m_activeHandheldItems = new List<PooledHandheldItem>(2);

			// Token: 0x04004E3D RID: 20029
			private UMAIKController m_ikController;

			// Token: 0x04004E3E RID: 20030
			private Transform m_dynamicButtEffectorTarget;

			// Token: 0x04004E3F RID: 20031
			private CharacterRecord m_record;
		}
	}
}
