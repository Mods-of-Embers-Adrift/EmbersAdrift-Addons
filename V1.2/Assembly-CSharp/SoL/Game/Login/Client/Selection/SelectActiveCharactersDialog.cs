using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.Login.Client.Selection
{
	// Token: 0x02000B44 RID: 2884
	public class SelectActiveCharactersDialog : MonoBehaviour
	{
		// Token: 0x060058B3 RID: 22707 RVA: 0x001E6EBC File Offset: 0x001E50BC
		private void Awake()
		{
			this.m_acceptButton.onClick.AddListener(new UnityAction(this.OnAcceptClicked));
			this.m_cancelButton.onClick.AddListener(new UnityAction(this.OnCancelClicked));
			this.m_subscribeNowLabel.SetTextFormat("{0}<u>{1}</u></link>", "<link=\"activateSub\">", this.m_subscribeNowLabel.text);
			if (this.m_selectors.Length < 9)
			{
				List<ActiveCharacterSelector> list = new List<ActiveCharacterSelector>(9);
				for (int i = 0; i < this.m_selectors.Length; i++)
				{
					if (this.m_selectors[i])
					{
						list.Add(this.m_selectors[i]);
					}
				}
				int num = 9 - list.Count;
				for (int j = 0; j < num; j++)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_togglePrefab, this.m_contentPanel);
					list.Add(gameObject.GetComponent<ActiveCharacterSelector>());
				}
				this.m_selectors = list.ToArray();
			}
		}

		// Token: 0x060058B4 RID: 22708 RVA: 0x0007B539 File Offset: 0x00079739
		private void OnDestroy()
		{
			this.m_acceptButton.onClick.RemoveListener(new UnityAction(this.OnAcceptClicked));
			this.m_cancelButton.onClick.RemoveListener(new UnityAction(this.OnCancelClicked));
		}

		// Token: 0x060058B5 RID: 22709 RVA: 0x001E6FA8 File Offset: 0x001E51A8
		public void Init()
		{
			this.m_errorLabel.text = string.Empty;
			this.m_acceptButton.interactable = false;
			for (int i = 0; i < this.m_selectors.Length; i++)
			{
				CharacterRecord record = (i < SessionData.Characters.Length) ? SessionData.Characters[i] : null;
				this.m_selectors[i].Init(this, record);
			}
			string arg = (SessionData.MaxCharacters == 1) ? "Base accounts can only have 1 active character." : ZString.Format<int>("Base accounts can only have {0} active characters.", SessionData.MaxCharacters);
			this.m_descriptionLabel.SetTextFormat("{0}\nYou must designate one now to play.", arg);
			this.RefreshCount();
			this.m_window.Show(false);
		}

		// Token: 0x060058B6 RID: 22710 RVA: 0x001E704C File Offset: 0x001E524C
		public void RefreshCount()
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < this.m_selectors.Length; i++)
			{
				if (this.m_selectors[i].IsSelected)
				{
					num++;
				}
				if (this.m_selectors[i].IsLocked)
				{
					num2++;
				}
			}
			bool capMet = num >= 2;
			for (int j = 0; j < this.m_selectors.Length; j++)
			{
				this.m_selectors[j].CountChanged(capMet);
			}
			this.m_acceptButton.interactable = (num - num2 > 0);
			this.m_errorLabel.text = string.Empty;
		}

		// Token: 0x060058B7 RID: 22711 RVA: 0x001E70E8 File Offset: 0x001E52E8
		private void OnAcceptClicked()
		{
			List<CharacterRecord> unlockedSelectedCharacters = this.GetUnlockedSelectedCharacters();
			if (unlockedSelectedCharacters.Count <= 0)
			{
				this.m_errorLabel.text = "Invalid Selection!";
				return;
			}
			List<string> list = new List<string>(unlockedSelectedCharacters.Count);
			for (int i = 0; i < unlockedSelectedCharacters.Count; i++)
			{
				list.Add(unlockedSelectedCharacters[i].Name);
			}
			string str = string.Empty;
			switch (list.Count)
			{
			case 1:
				str = list[0];
				break;
			case 2:
				str = list[0] + " and " + list[1];
				break;
			case 3:
				str = string.Concat(new string[]
				{
					list[0],
					", ",
					list[1],
					", and ",
					list[2]
				});
				break;
			case 4:
				str = string.Concat(new string[]
				{
					list[0],
					", ",
					list[1],
					", ",
					list[2],
					", and ",
					list[3]
				});
				break;
			}
			string text = (list.Count == 1) ? ("Are you sure you would like to set " + str + " as an active character?") : ("Are you sure you would like to set " + str + " as active characters?");
			string text2 = (SessionData.MaxCharacters > 1) ? "Characters" : "Character";
			DialogOptions opts = new DialogOptions
			{
				AllowDragging = false,
				BlockInteractions = true,
				BackgroundBlockerColor = new Color(0f, 0f, 0f, 0.9f),
				Text = string.Concat(new string[]
				{
					text,
					"\n<size=80%>(You can have a maximum of ",
					SessionData.MaxCharacters.ToString(),
					" active ",
					text2.ToLower(),
					" without a subscription)</size>\n\n<b><size=120%>This decision is FINAL.</size></b>\n<size=80%>(Upon activating a subscription all characters will be unlocked)</size>"
				}),
				ConfirmationText = "Yes",
				CancelText = "No",
				ShowCloseButton = false,
				Title = "Set Active " + text2,
				Callback = new Action<bool, object>(this.SetActiveCharactersCallback)
			};
			ClientGameManager.UIManager.ConfirmationDialog.Init(opts);
		}

		// Token: 0x060058B8 RID: 22712 RVA: 0x001E7348 File Offset: 0x001E5548
		private void SetActiveCharactersCallback(bool arg1, object arg2)
		{
			if (arg1)
			{
				List<CharacterRecord> allSelectedCharacters = this.GetAllSelectedCharacters();
				if (allSelectedCharacters.Count > 0)
				{
					LoginApiManager.SetActiveCharacters(allSelectedCharacters, new Action<bool, string>(this.Callback));
					return;
				}
				this.m_errorLabel.text = "Invalid Selection!";
			}
		}

		// Token: 0x060058B9 RID: 22713 RVA: 0x001E738C File Offset: 0x001E558C
		private void Callback(bool result, string error)
		{
			if (result)
			{
				this.m_errorLabel.text = string.Empty;
				this.m_window.Hide(false);
				if (SelectionDirector.Instance)
				{
					SelectionDirector.Instance.RefreshCharacterButtons();
					return;
				}
			}
			else
			{
				this.m_errorLabel.text = error;
			}
		}

		// Token: 0x060058BA RID: 22714 RVA: 0x001E73DC File Offset: 0x001E55DC
		private List<CharacterRecord> GetAllSelectedCharacters()
		{
			List<CharacterRecord> list = new List<CharacterRecord>(2);
			for (int i = 0; i < this.m_selectors.Length; i++)
			{
				if (this.m_selectors[i].IsSelected && this.m_selectors[i].Record != null)
				{
					list.Add(this.m_selectors[i].Record);
				}
			}
			return list;
		}

		// Token: 0x060058BB RID: 22715 RVA: 0x001E7438 File Offset: 0x001E5638
		private List<CharacterRecord> GetUnlockedSelectedCharacters()
		{
			List<CharacterRecord> list = new List<CharacterRecord>(2);
			for (int i = 0; i < this.m_selectors.Length; i++)
			{
				if (this.m_selectors[i].IsSelected && !this.m_selectors[i].IsLocked && this.m_selectors[i].Record != null)
				{
					list.Add(this.m_selectors[i].Record);
				}
			}
			return list;
		}

		// Token: 0x060058BC RID: 22716 RVA: 0x0007B573 File Offset: 0x00079773
		private void OnCancelClicked()
		{
			this.m_window.Hide(false);
		}

		// Token: 0x060058BD RID: 22717 RVA: 0x0007B581 File Offset: 0x00079781
		public void Hide(bool bypassTransition)
		{
			if (this.m_window)
			{
				this.m_window.Hide(bypassTransition);
			}
		}

		// Token: 0x04004E13 RID: 19987
		[SerializeField]
		private UIWindow m_window;

		// Token: 0x04004E14 RID: 19988
		[SerializeField]
		private SolButton m_acceptButton;

		// Token: 0x04004E15 RID: 19989
		[SerializeField]
		private SolButton m_cancelButton;

		// Token: 0x04004E16 RID: 19990
		[SerializeField]
		private TextMeshProUGUI m_errorLabel;

		// Token: 0x04004E17 RID: 19991
		[SerializeField]
		private TextMeshProUGUI m_descriptionLabel;

		// Token: 0x04004E18 RID: 19992
		[SerializeField]
		private TextMeshProUGUI m_subscribeNowLabel;

		// Token: 0x04004E19 RID: 19993
		[SerializeField]
		private RectTransform m_contentPanel;

		// Token: 0x04004E1A RID: 19994
		[SerializeField]
		private GameObject m_togglePrefab;

		// Token: 0x04004E1B RID: 19995
		[SerializeField]
		private ActiveCharacterSelector[] m_selectors;
	}
}
