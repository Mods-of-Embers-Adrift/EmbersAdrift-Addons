using System;
using System.Collections.Generic;
using SoL.Game.Quests;
using SoL.Game.UI.Dialog;
using SoL.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI.Quests
{
	// Token: 0x0200094D RID: 2381
	public class RewardsUI : UIWindow
	{
		// Token: 0x140000D6 RID: 214
		// (add) Token: 0x06004666 RID: 18022 RVA: 0x001A3890 File Offset: 0x001A1A90
		// (remove) Token: 0x06004667 RID: 18023 RVA: 0x001A38C8 File Offset: 0x001A1AC8
		public event Action<UniqueId> RewardChosen;

		// Token: 0x06004668 RID: 18024 RVA: 0x001A3900 File Offset: 0x001A1B00
		protected override void Awake()
		{
			base.Awake();
			foreach (RewardChoiceItem rewardChoiceItem in this.m_grantedItemSlots)
			{
				rewardChoiceItem.Init(null);
				rewardChoiceItem.Interactable = false;
				rewardChoiceItem.gameObject.SetActive(false);
			}
			foreach (RewardChoiceItem rewardChoiceItem2 in this.m_choiceItemSlots)
			{
				rewardChoiceItem2.Init(this.m_toggleGroup);
				rewardChoiceItem2.gameObject.SetActive(false);
				rewardChoiceItem2.ChoiceChanged += this.RefreshButton;
			}
			this.m_toggleGroup.allowSwitchOff = true;
			this.m_confirmButton.onClick.AddListener(new UnityAction(this.OnConfirmClicked));
		}

		// Token: 0x06004669 RID: 18025 RVA: 0x001A39B0 File Offset: 0x001A1BB0
		protected override void OnDestroy()
		{
			base.OnDestroy();
			RewardChoiceItem[] choiceItemSlots = this.m_choiceItemSlots;
			for (int i = 0; i < choiceItemSlots.Length; i++)
			{
				choiceItemSlots[i].ChoiceChanged -= this.RefreshButton;
			}
			this.m_confirmButton.onClick.RemoveListener(new UnityAction(this.OnConfirmClicked));
		}

		// Token: 0x0600466A RID: 18026 RVA: 0x001A3A08 File Offset: 0x001A1C08
		public override void Hide(bool skipTransition = false)
		{
			base.Hide(skipTransition);
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.ReagentPouch != null && LocalPlayer.GameEntity.CollectionController.Pouch != null && LocalPlayer.GameEntity.CollectionController.Gathering != null && LocalPlayer.GameEntity.CollectionController.Inventory != null)
			{
				LocalPlayer.GameEntity.CollectionController.ReagentPouch.ContentsChanged -= this.OnInventoryChanged;
				LocalPlayer.GameEntity.CollectionController.Pouch.ContentsChanged -= this.OnInventoryChanged;
				LocalPlayer.GameEntity.CollectionController.Gathering.ContentsChanged -= this.OnInventoryChanged;
				LocalPlayer.GameEntity.CollectionController.Inventory.ContentsChanged -= this.OnInventoryChanged;
			}
		}

		// Token: 0x0600466B RID: 18027 RVA: 0x001A3B0C File Offset: 0x001A1D0C
		public void InitChoiceList(Reward reward, bool reissue)
		{
			if (!LocalPlayer.GameEntity)
			{
				Debug.LogError("Attempted to init reward choice list without LocalPlayer.GameEntity!");
				return;
			}
			this.m_toggleGroup.SetAllTogglesOff(true);
			if (reward.ContainsItems)
			{
				LocalPlayer.GameEntity.CollectionController.ReagentPouch.ContentsChanged += this.OnInventoryChanged;
				LocalPlayer.GameEntity.CollectionController.Pouch.ContentsChanged += this.OnInventoryChanged;
				LocalPlayer.GameEntity.CollectionController.Gathering.ContentsChanged += this.OnInventoryChanged;
				LocalPlayer.GameEntity.CollectionController.Inventory.ContentsChanged += this.OnInventoryChanged;
			}
			ulong currency = reward.Currency.GetCurrency();
			this.m_grantsEnabled = 0;
			this.m_choicesEnabled = 0;
			this.m_isReissue = reissue;
			for (int i = 0; i < reward.Granted.Length; i++)
			{
				string text;
				if (!reissue || reward.Granted[i].CanBeReissuedToEntity(LocalPlayer.GameEntity, out text))
				{
					this.m_grantedItemSlots[i].InitItem(reward.Granted[i].Acquisition(LocalPlayer.GameEntity), reward.Granted[i].Amount);
					this.m_grantsEnabled++;
				}
			}
			for (int j = 0; j < this.m_grantedItemSlots.Length; j++)
			{
				this.m_grantedItemSlots[j].gameObject.SetActive(j < this.m_grantsEnabled);
			}
			this.m_grantedSection.gameObject.SetActive(this.m_grantsEnabled > 0 || currency > 0UL);
			if (this.m_grantsEnabled > 0 || (currency > 0UL && !reissue))
			{
				this.m_currencyPanel.gameObject.SetActive(currency > 0UL && !reissue);
				if (currency > 0UL && !reissue)
				{
					this.m_currencyPanel.UpdateCoin(currency);
				}
			}
			for (int k = 0; k < reward.Choices.Length; k++)
			{
				string text;
				if (!reissue || reward.Choices[k].CanBeReissuedToEntity(LocalPlayer.GameEntity, out text))
				{
					this.m_choiceItemSlots[k].InitItem(reward.Choices[k].Acquisition(LocalPlayer.GameEntity), reward.Choices[k].Amount);
					this.m_choicesEnabled++;
				}
			}
			for (int l = 0; l < this.m_choiceItemSlots.Length; l++)
			{
				this.m_choiceItemSlots[l].gameObject.SetActive(l < this.m_choicesEnabled);
			}
			this.m_choiceSection.gameObject.SetActive(this.m_choicesEnabled > 0);
			this.m_currentReward = reward;
			this.RefreshButton();
		}

		// Token: 0x0600466C RID: 18028 RVA: 0x001A3DA8 File Offset: 0x001A1FA8
		private void OnConfirmClicked()
		{
			if (this.m_choicesEnabled > 0)
			{
				int i = 0;
				while (i < this.m_choiceItemSlots.Length)
				{
					if (this.m_choiceItemSlots[i].IsSelected)
					{
						this.m_confirmButton.interactable = false;
						for (int j = 0; j < this.m_choiceItemSlots.Length; j++)
						{
							this.m_choiceItemSlots[j].Interactable = false;
						}
						Action<UniqueId> rewardChosen = this.RewardChosen;
						if (rewardChosen == null)
						{
							return;
						}
						rewardChosen(this.m_choiceItemSlots[i].Reward.Archetype.Id);
						return;
					}
					else
					{
						i++;
					}
				}
				return;
			}
			if (this.m_grantsEnabled > 0 || this.m_currentReward.Currency.GetCurrency() > 0UL)
			{
				this.m_confirmButton.interactable = false;
				Action<UniqueId> rewardChosen2 = this.RewardChosen;
				if (rewardChosen2 == null)
				{
					return;
				}
				rewardChosen2(UniqueId.Empty);
			}
		}

		// Token: 0x0600466D RID: 18029 RVA: 0x001A3E74 File Offset: 0x001A2074
		private void RefreshButton()
		{
			UniqueId rewardId = UniqueId.Empty;
			if (this.m_choicesEnabled > 0)
			{
				for (int i = 0; i < this.m_choiceItemSlots.Length; i++)
				{
					if (this.m_choiceItemSlots[i].IsSelected)
					{
						rewardId = this.m_choiceItemSlots[i].Reward.Archetype.Id;
					}
				}
				if (rewardId.IsEmpty)
				{
					this.m_confirmButton.interactable = false;
					this.m_confirmTooltip.gameObject.SetActive(true);
					this.m_confirmTooltip.Text = "Please select a reward.";
					return;
				}
			}
			string text = null;
			List<RewardItem> rewards;
			if (this.m_currentReward.TryGetRewards(LocalPlayer.GameEntity, rewardId, out rewards, this.m_isReissue) && rewards.EntityCanAcquire(LocalPlayer.GameEntity, out text))
			{
				this.m_confirmButton.interactable = true;
				this.m_confirmTooltip.gameObject.SetActive(false);
				return;
			}
			this.m_confirmButton.interactable = false;
			this.m_confirmTooltip.gameObject.SetActive(true);
			this.m_confirmTooltip.Text = text;
		}

		// Token: 0x0600466E RID: 18030 RVA: 0x001A3F74 File Offset: 0x001A2174
		private void OnInventoryChanged()
		{
			RewardChoiceItem[] array = this.m_grantedItemSlots;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].UpdateItemInteractable();
			}
			array = this.m_choiceItemSlots;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].UpdateItemInteractable();
			}
			this.RefreshButton();
		}

		// Token: 0x04004273 RID: 17011
		[SerializeField]
		private Image m_grantedSection;

		// Token: 0x04004274 RID: 17012
		[SerializeField]
		private RewardChoiceItem[] m_grantedItemSlots;

		// Token: 0x04004275 RID: 17013
		[SerializeField]
		private Image m_choiceSection;

		// Token: 0x04004276 RID: 17014
		[SerializeField]
		private CurrencyDisplayPanelUI m_currencyPanel;

		// Token: 0x04004277 RID: 17015
		[SerializeField]
		private RewardChoiceItem[] m_choiceItemSlots;

		// Token: 0x04004278 RID: 17016
		[SerializeField]
		private SolButton m_confirmButton;

		// Token: 0x04004279 RID: 17017
		[SerializeField]
		private TextTooltipTrigger m_confirmTooltip;

		// Token: 0x0400427A RID: 17018
		[SerializeField]
		private ToggleGroup m_toggleGroup;

		// Token: 0x0400427C RID: 17020
		private Reward m_currentReward;

		// Token: 0x0400427D RID: 17021
		private int m_grantsEnabled;

		// Token: 0x0400427E RID: 17022
		private int m_choicesEnabled;

		// Token: 0x0400427F RID: 17023
		private bool m_isReissue;
	}
}
