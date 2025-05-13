using System;
using System.Collections.Generic;
using SoL.Game.Messages;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Transactions;
using SoL.Game.UI.Merchants;
using SoL.Managers;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI.Social
{
	// Token: 0x02000911 RID: 2321
	public class MailboxUI : BaseMerchantUI<InteractiveMailbox>
	{
		// Token: 0x17000F5B RID: 3931
		// (get) Token: 0x06004441 RID: 17473 RVA: 0x0006E13A File Offset: 0x0006C33A
		public MailDetailUI MailDetail
		{
			get
			{
				return this.m_mailDetail;
			}
		}

		// Token: 0x17000F5C RID: 3932
		// (get) Token: 0x06004442 RID: 17474 RVA: 0x0006E142 File Offset: 0x0006C342
		public UniversalContainerUI Attachments
		{
			get
			{
				return this.m_attachments;
			}
		}

		// Token: 0x17000F5D RID: 3933
		// (get) Token: 0x06004443 RID: 17475 RVA: 0x0006E14A File Offset: 0x0006C34A
		public bool IsSendingMoney
		{
			get
			{
				return this.m_moneyDropdown.value == 0;
			}
		}

		// Token: 0x17000F5E RID: 3934
		// (get) Token: 0x06004444 RID: 17476 RVA: 0x000447A6 File Offset: 0x000429A6
		protected override ContainerType m_containerType
		{
			get
			{
				return ContainerType.PostOutgoing;
			}
		}

		// Token: 0x06004445 RID: 17477 RVA: 0x0019ADD8 File Offset: 0x00198FD8
		protected override void Start()
		{
			base.Start();
			this.m_inboxList.ViewRequested += this.OnViewRequested;
			this.m_outboxList.ViewRequested += this.OnViewRequested;
			this.m_moneyDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnMoneyDropdownChanged));
			this.m_editMoneyButton.onClick.AddListener(new UnityAction(this.OnEditMoneyButtonClicked));
			this.m_cancelButton.onClick.AddListener(new UnityAction(this.OnCancelButtonClicked));
			this.m_sendButton.onClick.AddListener(new UnityAction(this.OnSendButtonClicked));
			if (ClientGameManager.SocialManager)
			{
				ClientGameManager.SocialManager.PostInboxUpdated += this.UpdateInboxListWhenReady;
				ClientGameManager.SocialManager.PostOutboxUpdated += this.UpdateOutboxListWhenReady;
				ClientGameManager.SocialManager.NameCheckResponded += this.OnNameCheckResponse;
				ClientGameManager.SocialManager.UnreadMailUpdated += this.OnUnreadMailUpdated;
			}
			else
			{
				Debug.LogError("Unable to register MailboxUI subscriptions, no SocialManager initialized.");
			}
			if (LocalPlayer.IsInitialized)
			{
				this.OnLocalPlayerInitialized();
			}
			else
			{
				LocalPlayer.LocalPlayerInitialized += this.OnLocalPlayerInitialized;
			}
			base.UIWindow.ShowCalled += this.WindowShown;
			base.UIWindow.HideCalled += this.WindowHidden;
			this.m_tabController.TabChanged += this.OnTabChanged;
			this.m_mailDetail.ReplyClicked += this.OnReply;
			this.m_mailDetail.gameObject.SetActive(false);
		}

		// Token: 0x06004446 RID: 17478 RVA: 0x0019AF88 File Offset: 0x00199188
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.m_inboxList.Initialized -= this.UpdateInboxList;
			this.m_outboxList.Initialized -= this.UpdateOutboxList;
			this.m_inboxList.ViewRequested -= this.OnViewRequested;
			this.m_outboxList.ViewRequested -= this.OnViewRequested;
			this.m_moneyDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.OnMoneyDropdownChanged));
			this.m_editMoneyButton.onClick.RemoveListener(new UnityAction(this.OnEditMoneyButtonClicked));
			this.m_cancelButton.onClick.RemoveListener(new UnityAction(this.OnCancelButtonClicked));
			this.m_sendButton.onClick.RemoveListener(new UnityAction(this.OnSendButtonClicked));
			if (this.m_attachments.ContainerInstance != null)
			{
				this.m_attachments.ContainerInstance.ContentsChanged -= this.OnAttachmentsChanged;
			}
			if (ClientGameManager.SocialManager)
			{
				ClientGameManager.SocialManager.PostInboxUpdated -= this.UpdateInboxListWhenReady;
				ClientGameManager.SocialManager.PostOutboxUpdated -= this.UpdateOutboxListWhenReady;
				ClientGameManager.SocialManager.NameCheckResponded -= this.OnNameCheckResponse;
				ClientGameManager.SocialManager.UnreadMailUpdated -= this.OnUnreadMailUpdated;
			}
			if (LocalPlayer.GameEntity)
			{
				LocalPlayer.GameEntity.CollectionController.Inventory.CurrencyChanged -= this.OnCurrencyChanged;
				LocalPlayer.GameEntity.CollectionController.PersonalBank.CurrencyChanged -= this.OnCurrencyChanged;
			}
			base.UIWindow.ShowCalled -= this.WindowShown;
			base.UIWindow.HideCalled -= this.WindowHidden;
			this.m_tabController.TabChanged -= this.OnTabChanged;
			this.m_mailDetail.ReplyClicked -= this.OnReply;
		}

		// Token: 0x06004447 RID: 17479 RVA: 0x0019B19C File Offset: 0x0019939C
		private void Update()
		{
			if (!base.UIWindow || !base.UIWindow.Visible || !this.m_inboxList || this.m_inboxList.gameObject.activeInHierarchy || !this.m_outboxList || this.m_outboxList.gameObject.activeInHierarchy)
			{
				return;
			}
			Selectable selectable = null;
			if (Input.GetKeyDown(KeyCode.Tab) && UIManager.EventSystem && UIManager.EventSystem.currentSelectedGameObject)
			{
				selectable = ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? UIManager.EventSystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp() : UIManager.EventSystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown());
			}
			if (selectable)
			{
				UIManager.EventSystem.SetSelectedGameObject(selectable.gameObject, new BaseEventData(UIManager.EventSystem));
			}
		}

		// Token: 0x06004448 RID: 17480 RVA: 0x0019B290 File Offset: 0x00199490
		private void WindowShown()
		{
			base.RefreshAvailableCurrency();
			this.UpdatePostage();
			this.RefreshSendButtonInteractibility();
			if (this.m_inboxList.gameObject.activeInHierarchy)
			{
				this.UpdateInboxListWhenReady();
			}
			if (this.m_outboxList.gameObject.activeInHierarchy)
			{
				this.UpdateOutboxListWhenReady();
			}
		}

		// Token: 0x06004449 RID: 17481 RVA: 0x0006E15A File Offset: 0x0006C35A
		private void WindowHidden()
		{
			this.ClearComposeForm();
			this.m_to.Deactivate();
			this.m_subject.Deactivate();
			this.m_body.Deactivate();
			this.m_mailDetail.gameObject.SetActive(false);
		}

		// Token: 0x0600444A RID: 17482 RVA: 0x0006E194 File Offset: 0x0006C394
		public void UpdateInboxListWhenReady()
		{
			if (this.m_inboxList.IsInitialized)
			{
				this.UpdateInboxList();
				return;
			}
			this.m_inboxList.Initialized += this.UpdateInboxList;
		}

		// Token: 0x0600444B RID: 17483 RVA: 0x0019B2E0 File Offset: 0x001994E0
		public void UpdateInboxList()
		{
			List<Mail> fromPool = StaticListPool<Mail>.GetFromPool();
			fromPool.AddRange(ClientGameManager.SocialManager.PostInbox.Values);
			fromPool.Sort((Mail a, Mail b) => a.Created.CompareTo(b.Created));
			this.m_inboxList.UpdateItems(fromPool);
			this.m_inboxList.Reindex();
			this.m_inboxEmpty.gameObject.SetActive(fromPool.Count == 0);
			StaticListPool<Mail>.ReturnToPool(fromPool);
		}

		// Token: 0x0600444C RID: 17484 RVA: 0x0006E1C1 File Offset: 0x0006C3C1
		public void UpdateOutboxListWhenReady()
		{
			if (this.m_outboxList.IsInitialized)
			{
				this.UpdateOutboxList();
				return;
			}
			this.m_outboxList.Initialized += this.UpdateOutboxList;
		}

		// Token: 0x0600444D RID: 17485 RVA: 0x0019B364 File Offset: 0x00199564
		public void UpdateOutboxList()
		{
			List<Mail> fromPool = StaticListPool<Mail>.GetFromPool();
			fromPool.AddRange(ClientGameManager.SocialManager.PostOutbox.Values);
			fromPool.Sort((Mail a, Mail b) => a.Created.CompareTo(b.Created));
			this.m_outboxList.UpdateItems(fromPool);
			this.m_outboxList.Reindex();
			this.m_outboxEmpty.SetActive(fromPool.Count == 0);
			StaticListPool<Mail>.ReturnToPool(fromPool);
		}

		// Token: 0x0600444E RID: 17486 RVA: 0x0019B3E4 File Offset: 0x001995E4
		public void LockComposeForm()
		{
			this.m_to.interactable = false;
			this.m_subject.interactable = false;
			this.m_body.interactable = false;
			this.m_attachments.ContainerInstance.LockFlags |= ContainerLockFlags.Trade;
			this.m_moneyDropdown.interactable = false;
			this.m_editMoneyButton.interactable = false;
			this.m_cancelButton.interactable = false;
			this.m_sendButton.interactable = false;
			this.m_composeLocked = true;
		}

		// Token: 0x0600444F RID: 17487 RVA: 0x0019B464 File Offset: 0x00199664
		public void UnlockComposeForm()
		{
			this.m_to.interactable = true;
			this.m_subject.interactable = true;
			this.m_body.interactable = true;
			this.m_attachments.ContainerInstance.LockFlags &= ~ContainerLockFlags.Trade;
			this.m_moneyDropdown.interactable = true;
			this.m_editMoneyButton.interactable = true;
			this.m_cancelButton.interactable = true;
			this.m_sendButton.interactable = true;
			this.m_composeLocked = false;
		}

		// Token: 0x06004450 RID: 17488 RVA: 0x0019B4E8 File Offset: 0x001996E8
		private void UpdatePostage()
		{
			this.m_postage = Mail.GetPostage(LocalPlayer.GameEntity, this.m_attachments.ContainerInstance.Instances);
			this.m_postageDisplay.UpdateCoin(this.m_postage);
			this.UpdateMaxSendableMoney();
			if (this.m_moneyDropdown.value == 0)
			{
				this.ModifyCurrencyClamped(this.m_attachments.ContainerInstance.Currency);
			}
		}

		// Token: 0x06004451 RID: 17489 RVA: 0x0019B550 File Offset: 0x00199750
		private void UpdateMaxSendableMoney()
		{
			CurrencySources currencySources;
			ulong availableCurrencyForInteractiveStation = LocalPlayer.GameEntity.GetAvailableCurrencyForInteractiveStation(out currencySources);
			this.m_maxSendableMoney = ((availableCurrencyForInteractiveStation > this.m_postage) ? (availableCurrencyForInteractiveStation - this.m_postage) : 0UL);
		}

		// Token: 0x06004452 RID: 17490 RVA: 0x0019B588 File Offset: 0x00199788
		private void ModifyCurrencyClamped(ulong currency)
		{
			currency = Math.Min(currency, this.m_maxSendableMoney);
			if (this.m_moneyDropdown.value == 0 && currency != this.m_attachments.ContainerInstance.Currency)
			{
				this.m_attachments.ContainerInstance.ModifyCurrency(currency);
			}
		}

		// Token: 0x06004453 RID: 17491 RVA: 0x0019B5D4 File Offset: 0x001997D4
		private void OnLocalPlayerInitialized()
		{
			LocalPlayer.LocalPlayerInitialized -= this.OnLocalPlayerInitialized;
			LocalPlayer.GameEntity.CollectionController.Inventory.CurrencyChanged += this.OnCurrencyChanged;
			LocalPlayer.GameEntity.CollectionController.PersonalBank.CurrencyChanged += this.OnCurrencyChanged;
			if (this.m_attachments.ContainerInstance != null)
			{
				this.m_attachments.ContainerInstance.ContentsChanged += this.OnAttachmentsChanged;
				return;
			}
			Debug.LogError("Unable to register MailboxUI subscriptions, PostOutgoing container not initialized.");
		}

		// Token: 0x06004454 RID: 17492 RVA: 0x0006E1EE File Offset: 0x0006C3EE
		private void OnAttachmentsChanged()
		{
			this.UpdatePostage();
			this.RefreshSendButtonInteractibility();
		}

		// Token: 0x06004455 RID: 17493 RVA: 0x0006E1FC File Offset: 0x0006C3FC
		private void OnMoneyDropdownChanged(int value)
		{
			if (value == 0)
			{
				this.ModifyCurrencyClamped(this.m_attachments.ContainerInstance.Currency);
			}
			this.RefreshSendButtonInteractibility();
		}

		// Token: 0x06004456 RID: 17494 RVA: 0x0019B668 File Offset: 0x00199868
		private void OnEditMoneyButtonClicked()
		{
			Vector3? vector;
			if (this.m_editMoneyButton)
			{
				RectTransform rectTransform = this.m_editMoneyButton.transform as RectTransform;
				if (rectTransform != null)
				{
					vector = new Vector3?(rectTransform.GetWorldCorner(RectTransformCorner.LowerLeft));
					goto IL_38;
				}
			}
			vector = null;
			IL_38:
			Vector3? posOverride = vector;
			SelectCurrencyOptions opts = new SelectCurrencyOptions
			{
				Title = "Set Amount",
				AllowableCurrency = ((this.m_moneyDropdown.value == 0) ? this.m_maxSendableMoney : 9999999UL),
				InitialCurrency = this.m_attachments.ContainerInstance.Currency,
				Callback = new Action<bool, object>(this.EditMoneyCallback),
				ParentWindow = base.UIWindow,
				PosOverride = posOverride
			};
			ClientGameManager.UIManager.CurrencyPickerDialog.Init(opts);
		}

		// Token: 0x06004457 RID: 17495 RVA: 0x0019B738 File Offset: 0x00199938
		private void EditMoneyCallback(bool answer, object obj)
		{
			if (answer)
			{
				ulong currency = (ulong)obj;
				if (LocalPlayer.GameEntity)
				{
					if (this.m_moneyDropdown.value == 0)
					{
						this.ModifyCurrencyClamped(currency);
					}
					else
					{
						this.m_attachments.ContainerInstance.ModifyCurrency(currency);
					}
					this.RefreshSendButtonInteractibility();
				}
			}
		}

		// Token: 0x06004458 RID: 17496 RVA: 0x0006E21D File Offset: 0x0006C41D
		private void OnCancelButtonClicked()
		{
			this.ClearComposeForm();
		}

		// Token: 0x06004459 RID: 17497 RVA: 0x0019B788 File Offset: 0x00199988
		private void OnSendButtonClicked()
		{
			if (ClientGameManager.SocialManager)
			{
				CurrencySources currencySources;
				if (LocalPlayer.GameEntity.GetAvailableCurrencyForInteractiveStation(out currencySources) >= (this.IsSendingMoney ? (Mail.GetPostage(LocalPlayer.GameEntity, this.m_attachments.ContainerInstance.Instances) + this.m_attachments.ContainerInstance.Currency) : Mail.GetPostage(LocalPlayer.GameEntity, this.m_attachments.ContainerInstance.Instances)))
				{
					this.LockComposeForm();
					ClientGameManager.SocialManager.NameCheck(this.m_to.text.Trim());
					return;
				}
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "Insufficient funds to cover postage and/or sent currency.");
			}
		}

		// Token: 0x0600445A RID: 17498 RVA: 0x0019B834 File Offset: 0x00199A34
		private void OnNameCheckResponse(CharacterIdentification ident, int attachmentsRemaining)
		{
			if (ident != null && attachmentsRemaining <= 0 && (this.m_attachments.ContainerInstance.Count > 0 || this.m_attachments.ContainerInstance.Currency > 0UL))
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "You may not send attachments to that player, they have reached their attachment limit and must clear some mail items containing attachments from their inbox.");
				this.UnlockComposeForm();
				return;
			}
			if (ident != null && !ident.IsDeleted && ident.Name.Equals(this.m_to.text.Trim(), StringComparison.CurrentCultureIgnoreCase) && ident.Name != LocalPlayer.GameEntity.CharacterData.Name.Value)
			{
				List<ArchetypeInstance> fromPool = StaticListPool<ArchetypeInstance>.GetFromPool();
				foreach (ArchetypeInstance item in this.m_attachments.ContainerInstance.Instances)
				{
					fromPool.Add(item);
				}
				ulong? currencyAttachment = (this.m_moneyDropdown.value == 0) ? new ulong?(this.m_attachments.ContainerInstance.Currency) : null;
				ulong? cashOnDelivery = (this.m_moneyDropdown.value == 1 && this.m_attachments.ContainerInstance.Currency > 0UL) ? new ulong?(this.m_attachments.ContainerInstance.Currency) : null;
				SocialManager.PendingOutgoingMail = new Mail
				{
					Type = MailType.Post,
					Sender = LocalPlayer.GameEntity.CharacterData.CharacterId,
					Recipient = ident._id,
					Subject = this.m_subject.text.Trim(),
					Message = this.m_body.text.Trim(),
					ItemAttachments = fromPool,
					CurrencyAttachment = currencyAttachment,
					CashOnDelivery = cashOnDelivery,
					Returned = new bool?(false)
				};
				if (LocalPlayer.NetworkEntity && LocalPlayer.NetworkEntity.PlayerRpcHandler)
				{
					SendMailTransaction sendMailTransaction = new SendMailTransaction
					{
						Recipient = ident._id,
						Subject = this.m_subject.text,
						Message = this.m_body.text,
						CurrencyAttachment = currencyAttachment,
						CashOnDelivery = cashOnDelivery
					};
					LocalPlayer.NetworkEntity.PlayerRpcHandler.SendMailRequest(sendMailTransaction);
				}
				this.ClearComposeForm();
				this.m_tabController.SwitchToTab(1);
				return;
			}
			this.UnlockComposeForm();
		}

		// Token: 0x0600445B RID: 17499 RVA: 0x0006E225 File Offset: 0x0006C425
		private void OnViewRequested(Mail mail)
		{
			this.m_mailDetail.Init(mail);
		}

		// Token: 0x0600445C RID: 17500 RVA: 0x0006E233 File Offset: 0x0006C433
		private void OnCurrencyChanged(ulong obj)
		{
			base.RefreshAvailableCurrency();
			this.RefreshSendButtonInteractibility();
			this.UpdateMaxSendableMoney();
		}

		// Token: 0x0600445D RID: 17501 RVA: 0x0006E247 File Offset: 0x0006C447
		private void OnTabChanged()
		{
			if (this.m_inboxList.gameObject.activeInHierarchy)
			{
				this.UpdateInboxListWhenReady();
			}
			if (this.m_outboxList.gameObject.activeInHierarchy)
			{
				this.UpdateOutboxListWhenReady();
			}
		}

		// Token: 0x0600445E RID: 17502 RVA: 0x0006E279 File Offset: 0x0006C479
		private void OnUnreadMailUpdated()
		{
			if (this.m_inboxList.IsInitialized)
			{
				this.m_inboxList.Reindex();
			}
		}

		// Token: 0x0600445F RID: 17503 RVA: 0x0006E293 File Offset: 0x0006C493
		private void OnReply(string recipient, string subject)
		{
			this.m_to.text = recipient;
			this.m_subject.text = subject;
			this.m_mailDetail.gameObject.SetActive(false);
			this.m_tabController.SwitchToTab(2);
		}

		// Token: 0x06004460 RID: 17504 RVA: 0x0006E2CA File Offset: 0x0006C4CA
		private void ClearComposeForm()
		{
			this.m_to.text = null;
			this.m_subject.text = null;
			this.m_body.text = null;
		}

		// Token: 0x06004461 RID: 17505 RVA: 0x0019BAC0 File Offset: 0x00199CC0
		private void RefreshSendButtonInteractibility()
		{
			if (LocalPlayer.GameEntity)
			{
				CurrencySources currencySources;
				ulong availableCurrencyForInteractiveStation = LocalPlayer.GameEntity.GetAvailableCurrencyForInteractiveStation(out currencySources);
				bool flag = availableCurrencyForInteractiveStation >= this.m_postage;
				bool flag2 = availableCurrencyForInteractiveStation >= (this.IsSendingMoney ? (this.m_postage + this.m_attachments.ContainerInstance.Currency) : this.m_postage);
				bool flag3 = this.m_moneyDropdown.value == 1 && this.m_attachments.ContainerInstance.Currency > 0UL;
				bool flag4 = this.m_attachments.ContainerInstance.Count > 0;
				if (this.m_composeLocked)
				{
					this.m_sendButton.interactable = false;
					return;
				}
				this.m_sendButton.interactable = (flag2 && (!flag3 || flag4));
				this.m_sendButtonTooltip.Text = null;
				if (this.m_attachments.ContainerInstance.Currency > 0UL && !flag2)
				{
					this.m_sendButtonTooltip.Text = "Not enough money to cover both the postage and the sent amount.";
					return;
				}
				if (!flag)
				{
					this.m_sendButtonTooltip.Text = "Not enough money to cover postage.";
					return;
				}
				if (flag3 && !flag4)
				{
					this.m_sendButtonTooltip.Text = "You cannot require cash on delivery if no attachments are included.";
				}
			}
		}

		// Token: 0x06004462 RID: 17506 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool ButtonClickedInternal()
		{
			return false;
		}

		// Token: 0x040040D1 RID: 16593
		[SerializeField]
		private TabController m_tabController;

		// Token: 0x040040D2 RID: 16594
		[SerializeField]
		private MailList m_inboxList;

		// Token: 0x040040D3 RID: 16595
		[SerializeField]
		private MailList m_outboxList;

		// Token: 0x040040D4 RID: 16596
		[SerializeField]
		private TextMeshProUGUI m_inboxEmpty;

		// Token: 0x040040D5 RID: 16597
		[SerializeField]
		private GameObject m_outboxEmpty;

		// Token: 0x040040D6 RID: 16598
		[SerializeField]
		private MailDetailUI m_mailDetail;

		// Token: 0x040040D7 RID: 16599
		[SerializeField]
		private SolTMP_InputField m_to;

		// Token: 0x040040D8 RID: 16600
		[SerializeField]
		private SolTMP_InputField m_subject;

		// Token: 0x040040D9 RID: 16601
		[SerializeField]
		private SolTMP_InputField m_body;

		// Token: 0x040040DA RID: 16602
		[SerializeField]
		private UniversalContainerUI m_attachments;

		// Token: 0x040040DB RID: 16603
		[SerializeField]
		private TMP_Dropdown m_moneyDropdown;

		// Token: 0x040040DC RID: 16604
		[SerializeField]
		private SolButton m_editMoneyButton;

		// Token: 0x040040DD RID: 16605
		[SerializeField]
		private SolButton m_cancelButton;

		// Token: 0x040040DE RID: 16606
		[SerializeField]
		private CurrencyDisplayPanelUI m_postageDisplay;

		// Token: 0x040040DF RID: 16607
		[SerializeField]
		private SolButton m_sendButton;

		// Token: 0x040040E0 RID: 16608
		[SerializeField]
		private TextTooltipTrigger m_sendButtonTooltip;

		// Token: 0x040040E1 RID: 16609
		private ulong m_maxSendableMoney;

		// Token: 0x040040E2 RID: 16610
		private ulong m_postage;

		// Token: 0x040040E3 RID: 16611
		private bool m_composeLocked;

		// Token: 0x040040E4 RID: 16612
		private const string kMsgNotEnoughMoney_Postage = "Not enough money to cover postage.";

		// Token: 0x040040E5 RID: 16613
		private const string kMsgNotEnoughMoney_PostageAndSentAmount = "Not enough money to cover both the postage and the sent amount.";

		// Token: 0x040040E6 RID: 16614
		private const string kMsgCannotRequireCodWithoutAttachments = "You cannot require cash on delivery if no attachments are included.";
	}
}
