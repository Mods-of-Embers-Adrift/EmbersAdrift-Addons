using System;
using SoL.Game.Quests;
using SoL.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI.Dialog
{
	// Token: 0x0200098D RID: 2445
	public class RewardChoiceGrid : MonoBehaviour
	{
		// Token: 0x140000DE RID: 222
		// (add) Token: 0x060048DA RID: 18650 RVA: 0x001AB788 File Offset: 0x001A9988
		// (remove) Token: 0x060048DB RID: 18651 RVA: 0x001AB7C0 File Offset: 0x001A99C0
		public event Action<UniqueId> RewardChosen;

		// Token: 0x1700102C RID: 4140
		// (get) Token: 0x060048DC RID: 18652 RVA: 0x00070F8D File Offset: 0x0006F18D
		public UIWindow Window
		{
			get
			{
				return this.m_window;
			}
		}

		// Token: 0x060048DD RID: 18653 RVA: 0x001AB7F8 File Offset: 0x001A99F8
		private void Awake()
		{
			for (int i = 0; i < this.m_items.Length; i++)
			{
				this.m_items[i].Init(this.m_toggleGroup);
				this.m_items[i].gameObject.SetActive(false);
				this.m_items[i].ChoiceChanged += this.RefreshButton;
			}
			this.m_toggleGroup.allowSwitchOff = true;
			this.RefreshButton();
			this.m_confirmButton.onClick.AddListener(new UnityAction(this.ConfirmButtonClicked));
			this.m_window.WindowClosed += this.OnClose;
		}

		// Token: 0x060048DE RID: 18654 RVA: 0x001AB89C File Offset: 0x001A9A9C
		private void OnDestroy()
		{
			for (int i = 0; i < this.m_items.Length; i++)
			{
				this.m_items[i].ChoiceChanged -= this.RefreshButton;
			}
			this.m_confirmButton.onClick.RemoveListener(new UnityAction(this.ConfirmButtonClicked));
			this.m_window.WindowClosed -= this.OnClose;
		}

		// Token: 0x060048DF RID: 18655 RVA: 0x001AB908 File Offset: 0x001A9B08
		private void OnClose()
		{
			if (LocalPlayer.GameEntity)
			{
				LocalPlayer.GameEntity.CollectionController.ReagentPouch.ContentsChanged -= this.OnInventoryChanged;
				LocalPlayer.GameEntity.CollectionController.Pouch.ContentsChanged -= this.OnInventoryChanged;
				LocalPlayer.GameEntity.CollectionController.Gathering.ContentsChanged -= this.OnInventoryChanged;
				LocalPlayer.GameEntity.CollectionController.Inventory.ContentsChanged -= this.OnInventoryChanged;
			}
		}

		// Token: 0x060048E0 RID: 18656 RVA: 0x00070F95 File Offset: 0x0006F195
		public void InitChoiceList(Reward reward)
		{
			if (!LocalPlayer.GameEntity)
			{
				Debug.LogError("Attempted to init reward choice list without LocalPlayer.GameEntity!");
				return;
			}
			this.m_toggleGroup.SetAllTogglesOff(true);
			throw new NotImplementedException("Rewards V1 has been disabled and this class should no longer be used!");
		}

		// Token: 0x060048E1 RID: 18657 RVA: 0x001AB9A4 File Offset: 0x001A9BA4
		private void ConfirmButtonClicked()
		{
			int i = 0;
			while (i < this.m_items.Length)
			{
				if (this.m_items[i].IsSelected)
				{
					this.m_confirmButton.interactable = false;
					for (int j = 0; j < this.m_items.Length; j++)
					{
						this.m_items[j].Interactable = false;
					}
					Action<UniqueId> rewardChosen = this.RewardChosen;
					if (rewardChosen == null)
					{
						return;
					}
					rewardChosen(this.m_items[i].Reward.Archetype.Id);
					return;
				}
				else
				{
					i++;
				}
			}
		}

		// Token: 0x060048E2 RID: 18658 RVA: 0x00070FC4 File Offset: 0x0006F1C4
		private void RefreshButton()
		{
			this.m_confirmButton.interactable = this.m_toggleGroup.AnyTogglesOn();
		}

		// Token: 0x060048E3 RID: 18659 RVA: 0x001ABA28 File Offset: 0x001A9C28
		private void OnInventoryChanged()
		{
			RewardChoiceItem[] items = this.m_items;
			for (int i = 0; i < items.Length; i++)
			{
				items[i].UpdateItemInteractable();
			}
		}

		// Token: 0x0400440D RID: 17421
		[SerializeField]
		private UIWindow m_window;

		// Token: 0x0400440E RID: 17422
		[SerializeField]
		private ToggleGroup m_toggleGroup;

		// Token: 0x0400440F RID: 17423
		[SerializeField]
		private SolButton m_confirmButton;

		// Token: 0x04004410 RID: 17424
		[SerializeField]
		private RewardChoiceItem[] m_items;
	}
}
