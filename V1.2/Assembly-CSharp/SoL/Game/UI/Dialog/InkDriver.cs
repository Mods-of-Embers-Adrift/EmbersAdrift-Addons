using System;
using Ink.Runtime;
using SoL.Game.Interactives;
using SoL.Game.Quests;
using SoL.Game.UI.Quests;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI.Dialog
{
	// Token: 0x0200098C RID: 2444
	public class InkDriver : MonoBehaviour
	{
		// Token: 0x1700102B RID: 4139
		// (get) Token: 0x060048C9 RID: 18633 RVA: 0x00070EA1 File Offset: 0x0006F0A1
		// (set) Token: 0x060048CA RID: 18634 RVA: 0x001AB06C File Offset: 0x001A926C
		private bool IsWriting
		{
			get
			{
				return this.m_isWriting;
			}
			set
			{
				if (value == this.m_isWriting)
				{
					return;
				}
				this.m_isWriting = value;
				if (this.m_scribbleAudioSource == null)
				{
					return;
				}
				if (this.m_isWriting)
				{
					this.m_scribbleAudioSource.time = UnityEngine.Random.Range(0f, this.m_scribbleAudioSource.clip.length);
					this.m_scribbleAudioSource.Play();
					return;
				}
				this.m_scribbleAudioSource.Stop();
			}
		}

		// Token: 0x060048CB RID: 18635 RVA: 0x001AB0E0 File Offset: 0x001A92E0
		private void Awake()
		{
			this.m_window.HideCallback = new Action(this.HideCallback);
			this.m_endDialogButton.onClick.AddListener(new UnityAction(this.OnEndDialogClicked));
			LocalPlayer.LocalPlayerHealthStateUpdated += this.LocalPlayerOnLocalPlayerHealthStateUpdated;
			DialogueManager.DialogueUpdated += this.OnDialogueUpdated;
			DialogueManager.RewardAvailable += this.OnRewardAvailable;
			DialogueManager.DialogueTerminated += this.OnEndDialogClicked;
			this.m_rewardChoiceUi.RewardChosen += DialogueManager.OnRewardChosen;
			this.m_rewardChoiceUi.RewardChosen += this.OnRewardChosen;
			this.m_rewardsUI.RewardChosen += DialogueManager.OnRewardChosen;
			this.m_rewardsUI.RewardChosen += this.OnRewardChosen;
			this.m_displayText.text = null;
			this.m_displayText.fontSize = (float)Options.GameOptions.DialogueFontSize.Value;
			this.m_choicesList.Chosen += DialogueManager.Choose;
			if (this.m_scribbleAudioSource != null)
			{
				this.m_scribbleAudioSource.ConfigureAudioSourceForUI();
				this.m_scribbleAudioSource.loop = true;
			}
		}

		// Token: 0x060048CC RID: 18636 RVA: 0x001AB220 File Offset: 0x001A9420
		private void Start()
		{
			this.m_rewardChoiceStartingPos = this.m_rewardChoiceUi.Window.RectTransform.anchoredPosition;
			this.m_rewardsUIStartingPos = this.m_rewardsUI.RectTransform.anchoredPosition;
			this.m_blockerPanel.gameObject.SetActive(false);
		}

		// Token: 0x060048CD RID: 18637 RVA: 0x00070EA9 File Offset: 0x0006F0A9
		private void HideCallback()
		{
			this.IsWriting = false;
			if (DialogueManager.StoryActive)
			{
				DialogueManager.TerminateDialogue();
			}
			ClientGameManager.InputManager.UnsetInputPreventionFlag(InputPreventionFlags.QuestDialog);
		}

		// Token: 0x060048CE RID: 18638 RVA: 0x001AB270 File Offset: 0x001A9470
		private void OnDestroy()
		{
			if (this.m_window.Visible)
			{
				this.OnEndDialogClicked();
			}
			this.m_endDialogButton.onClick.RemoveListener(new UnityAction(this.OnEndDialogClicked));
			LocalPlayer.LocalPlayerHealthStateUpdated -= this.LocalPlayerOnLocalPlayerHealthStateUpdated;
			DialogueManager.DialogueUpdated -= this.OnDialogueUpdated;
			DialogueManager.RewardAvailable -= this.OnRewardAvailable;
			DialogueManager.DialogueTerminated -= this.OnEndDialogClicked;
			this.m_rewardChoiceUi.RewardChosen -= DialogueManager.OnRewardChosen;
			this.m_rewardChoiceUi.RewardChosen -= this.OnRewardChosen;
			this.m_rewardsUI.RewardChosen -= DialogueManager.OnRewardChosen;
			this.m_rewardsUI.RewardChosen -= this.OnRewardChosen;
			this.m_choicesList.Chosen -= DialogueManager.Choose;
		}

		// Token: 0x060048CF RID: 18639 RVA: 0x001AB364 File Offset: 0x001A9564
		private void Update()
		{
			if (!DialogueManager.StoryActive)
			{
				return;
			}
			if (DialogueManager.CurrentInteractive != null && !((IInteractiveBase)DialogueManager.CurrentInteractive).Settings.IsWithinRange(DialogueManager.CurrentInteractive.gameObject, LocalPlayer.GameEntity))
			{
				DialogueManager.TerminateDialogue();
				return;
			}
			if (DialogueManager.CurrentWorldObject != null && !((IInteractiveBase)DialogueManager.CurrentWorldObject).Settings.IsWithinRange(DialogueManager.CurrentWorldObject.gameObject, LocalPlayer.GameEntity))
			{
				DialogueManager.TerminateDialogue();
				return;
			}
			int characterCount = this.m_displayText.textInfo.characterCount;
			if (this.m_maxLetterCount < (float)characterCount)
			{
				if (DialogueManager.CurrentSourceType == DialogSourceType.NPC)
				{
					this.IsWriting = true;
					float num = (float)Options.GameOptions.DialogSpeed.Value * Time.deltaTime;
					this.m_maxLetterCount = Mathf.Clamp(this.m_maxLetterCount + num, 0f, (float)characterCount);
				}
				else
				{
					this.m_maxLetterCount = (float)this.m_displayText.textInfo.characterCount;
				}
				this.m_displayText.maxVisibleCharacters = characterCount;
			}
			else if (this.IsWriting)
			{
				this.IsWriting = false;
			}
			Vector2 target = this.m_rewardChoiceUiShown ? Vector2.zero : this.m_rewardChoiceStartingPos;
			Vector2 anchoredPosition = Vector2.MoveTowards(this.m_rewardChoiceUi.Window.RectTransform.anchoredPosition, target, Time.deltaTime * this.m_rewardOpenSpeed);
			this.m_rewardChoiceUi.Window.RectTransform.anchoredPosition = anchoredPosition;
			target = (this.m_rewardsUIShown ? Vector2.zero : this.m_rewardsUIStartingPos);
			anchoredPosition = Vector2.MoveTowards(this.m_rewardsUI.RectTransform.anchoredPosition, target, Time.deltaTime * this.m_rewardOpenSpeed);
			this.m_rewardsUI.RectTransform.anchoredPosition = anchoredPosition;
		}

		// Token: 0x060048D0 RID: 18640 RVA: 0x001AB510 File Offset: 0x001A9710
		private void OnDialogueUpdated()
		{
			if (!this.m_window.Visible)
			{
				this.m_window.Show(false);
				this.m_choicesList.UpdateItems(Array.Empty<Choice>());
				this.m_choicesList.gameObject.SetActive(false);
			}
			if (DialogueManager.IsChoice)
			{
				this.m_maxLetterCount = (float)this.m_displayText.textInfo.characterCount;
				this.m_displayText.maxVisibleCharacters = (int)this.m_maxLetterCount;
				this.IsWriting = false;
				this.m_choicesList.UpdateItems(Array.Empty<Choice>());
				this.m_choicesList.gameObject.SetActive(false);
			}
			else
			{
				this.m_maxLetterCount = 0f;
			}
			if (DialogueManager.Choices.Count > 0)
			{
				this.m_choicesList.gameObject.SetActive(true);
				this.m_choicesList.UpdateItems(DialogueManager.Choices);
				this.m_choicesListLayout.minHeight = Math.Min(this.m_choicesList.CombinedItemHeights + 30f, 140f);
			}
			this.m_displayText.text = DialogueManager.BuildDialogText();
			this.m_scroll.verticalNormalizedPosition = 0f;
			LayoutRebuilder.MarkLayoutForRebuild(this.m_layoutRefresh);
		}

		// Token: 0x060048D1 RID: 18641 RVA: 0x001AB63C File Offset: 0x001A983C
		private void OnRewardAvailable(Reward reward, bool reissue)
		{
			if (!this.m_window.Visible)
			{
				this.m_window.Show(false);
				this.ToggleButtons(false);
			}
			this.m_rewardsUI.InitChoiceList(reward, reissue);
			this.m_rewardsUI.Show(true);
			this.m_rewardsUIShown = true;
			this.m_blockerPanel.gameObject.SetActive(true);
		}

		// Token: 0x060048D2 RID: 18642 RVA: 0x00070EC9 File Offset: 0x0006F0C9
		private void OnRewardChosen(UniqueId rewardChoiceId)
		{
			this.m_rewardsUIShown = false;
			this.m_rewardChoiceUiShown = false;
			this.m_blockerPanel.gameObject.SetActive(false);
		}

		// Token: 0x060048D3 RID: 18643 RVA: 0x00070EEA File Offset: 0x0006F0EA
		private void LocalPlayerOnLocalPlayerHealthStateUpdated(HealthState obj)
		{
			if ((DialogueManager.StoryActive || this.m_window.Visible) && obj != HealthState.Alive)
			{
				this.OnEndDialogClicked();
			}
		}

		// Token: 0x060048D4 RID: 18644 RVA: 0x001AB69C File Offset: 0x001A989C
		private void OnEndDialogClicked()
		{
			if (this.m_window != null)
			{
				this.m_choicesList.UpdateItems(Array.Empty<Choice>());
				this.m_choicesList.gameObject.SetActive(false);
				this.m_window.Hide(false);
				this.m_rewardsUIShown = false;
				this.m_rewardsUI.Hide(true);
				this.m_rewardsUI.RectTransform.anchoredPosition = this.m_rewardChoiceStartingPos;
				this.m_rewardChoiceUiShown = false;
				this.m_rewardChoiceUi.Window.Hide(true);
				this.m_rewardChoiceUi.Window.RectTransform.anchoredPosition = this.m_rewardChoiceStartingPos;
				this.m_blockerPanel.gameObject.SetActive(false);
			}
		}

		// Token: 0x060048D5 RID: 18645 RVA: 0x00070F0A File Offset: 0x0006F10A
		private void OnMouseEnter(SolButton obj)
		{
			obj.SetTextColor(InkDriver.m_selectedChoiceColor);
			obj.UnderlineText();
		}

		// Token: 0x060048D6 RID: 18646 RVA: 0x00070F1D File Offset: 0x0006F11D
		private void OnMouseExit(SolButton obj)
		{
			obj.SetTextColor(InkDriver.m_choiceColor);
			obj.RemoveUnderlineText();
		}

		// Token: 0x060048D7 RID: 18647 RVA: 0x001AB754 File Offset: 0x001A9954
		private void ToggleButtons(bool enabled)
		{
			for (int i = 0; i < this.m_responseButtons.Length; i++)
			{
				this.m_responseButtons[i].gameObject.SetActive(enabled);
			}
		}

		// Token: 0x040043F3 RID: 17395
		private const string kMeetsRequirementsKey = "meets_requirements";

		// Token: 0x040043F4 RID: 17396
		private const string kQuestStatusKey = "quest_status";

		// Token: 0x040043F5 RID: 17397
		[SerializeField]
		private ScrollRect m_scroll;

		// Token: 0x040043F6 RID: 17398
		[SerializeField]
		private TextMeshProUGUI m_displayText;

		// Token: 0x040043F7 RID: 17399
		[SerializeField]
		private SolButton[] m_responseButtons;

		// Token: 0x040043F8 RID: 17400
		[SerializeField]
		private ChoicesList m_choicesList;

		// Token: 0x040043F9 RID: 17401
		[SerializeField]
		private LayoutElement m_choicesListLayout;

		// Token: 0x040043FA RID: 17402
		[SerializeField]
		private Image m_blockerPanel;

		// Token: 0x040043FB RID: 17403
		[SerializeField]
		private SolButton m_endDialogButton;

		// Token: 0x040043FC RID: 17404
		[SerializeField]
		private AudioSource m_scribbleAudioSource;

		// Token: 0x040043FD RID: 17405
		[SerializeField]
		private RectTransform m_layoutRefresh;

		// Token: 0x040043FE RID: 17406
		[SerializeField]
		private UIWindow m_window;

		// Token: 0x040043FF RID: 17407
		[SerializeField]
		private RewardChoiceGrid m_rewardChoiceUi;

		// Token: 0x04004400 RID: 17408
		[SerializeField]
		private RewardsUI m_rewardsUI;

		// Token: 0x04004401 RID: 17409
		[SerializeField]
		private float m_rewardOpenSpeed = 400f;

		// Token: 0x04004402 RID: 17410
		private bool m_rewardChoiceUiShown;

		// Token: 0x04004403 RID: 17411
		private bool m_rewardsUIShown;

		// Token: 0x04004404 RID: 17412
		private Vector2 m_rewardChoiceStartingPos = Vector2.zero;

		// Token: 0x04004405 RID: 17413
		private Vector2 m_rewardsUIStartingPos = Vector2.zero;

		// Token: 0x04004406 RID: 17414
		public static readonly Color m_emotiveColor = Colors.Gold;

		// Token: 0x04004407 RID: 17415
		public static readonly Color m_stageDirectionColor = Colors.RoyalPurple;

		// Token: 0x04004408 RID: 17416
		public static readonly Color m_choiceColor = Colors.Watermelon;

		// Token: 0x04004409 RID: 17417
		public static readonly Color m_selectedChoiceColor = Colors.CornflowerBlue;

		// Token: 0x0400440A RID: 17418
		public static readonly Color m_warningColor = Color.red;

		// Token: 0x0400440B RID: 17419
		private float m_maxLetterCount;

		// Token: 0x0400440C RID: 17420
		private bool m_isWriting;
	}
}
