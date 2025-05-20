using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Messages;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoL.Game.UI.Chat
{
	// Token: 0x020009AA RID: 2474
	public class ChatTab : MonoBehaviour, IContextMenu, IInteractiveBase, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDraggable
	{
		// Token: 0x1700105C RID: 4188
		// (get) Token: 0x06004A06 RID: 18950 RVA: 0x00071CD4 File Offset: 0x0006FED4
		public SolToggle Toggle
		{
			get
			{
				return this.m_toggle;
			}
		}

		// Token: 0x1700105D RID: 4189
		// (get) Token: 0x06004A07 RID: 18951 RVA: 0x00071CDC File Offset: 0x0006FEDC
		public ChatWindowUI ParentWindow
		{
			get
			{
				return this.m_parentWindow;
			}
		}

		// Token: 0x1700105E RID: 4190
		// (get) Token: 0x06004A08 RID: 18952 RVA: 0x00071CE4 File Offset: 0x0006FEE4
		public MessageType InputChannel
		{
			get
			{
				return this.m_inputChannel;
			}
		}

		// Token: 0x1700105F RID: 4191
		// (get) Token: 0x06004A09 RID: 18953 RVA: 0x00071CEC File Offset: 0x0006FEEC
		// (set) Token: 0x06004A0A RID: 18954 RVA: 0x00071CF4 File Offset: 0x0006FEF4
		public string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
				this.RefreshTabVisuals(this.m_toggle.isOn);
				Action<ChatTab> renamed = this.Renamed;
				if (renamed == null)
				{
					return;
				}
				renamed(this);
			}
		}

		// Token: 0x17001060 RID: 4192
		// (get) Token: 0x06004A0B RID: 18955 RVA: 0x00071D1F File Offset: 0x0006FF1F
		// (set) Token: 0x06004A0C RID: 18956 RVA: 0x00071D27 File Offset: 0x0006FF27
		public ChatFilter ChatFilter
		{
			get
			{
				return this.m_chatFilter;
			}
			set
			{
				this.m_chatFilter = value;
				if (this.m_queue == MessageManager.ChatQueue && this.m_parentWindow)
				{
					this.m_parentWindow.UpdateChatListWhenReady();
				}
			}
		}

		// Token: 0x17001061 RID: 4193
		// (get) Token: 0x06004A0D RID: 18957 RVA: 0x00071D55 File Offset: 0x0006FF55
		// (set) Token: 0x06004A0E RID: 18958 RVA: 0x00071D5D File Offset: 0x0006FF5D
		public CombatFilter CombatFilter
		{
			get
			{
				return this.m_combatFilter;
			}
			set
			{
				this.m_combatFilter = value;
				if (this.m_queue == MessageManager.CombatQueue && this.m_parentWindow)
				{
					this.m_parentWindow.UpdateChatListWhenReady();
				}
			}
		}

		// Token: 0x17001062 RID: 4194
		// (get) Token: 0x06004A0F RID: 18959 RVA: 0x00071D8B File Offset: 0x0006FF8B
		// (set) Token: 0x06004A10 RID: 18960 RVA: 0x00071DA7 File Offset: 0x0006FFA7
		public int CurrentFilter
		{
			get
			{
				if (this.m_queue != MessageManager.ChatQueue)
				{
					return (int)this.m_combatFilter;
				}
				return (int)this.m_chatFilter;
			}
			set
			{
				if (this.m_queue == MessageManager.ChatQueue)
				{
					this.m_chatFilter = (ChatFilter)value;
					return;
				}
				this.m_combatFilter = (CombatFilter)value;
			}
		}

		// Token: 0x17001063 RID: 4195
		// (get) Token: 0x06004A11 RID: 18961 RVA: 0x00071DC5 File Offset: 0x0006FFC5
		public MessageType CurrentMessageTypeFilter
		{
			get
			{
				if (this.m_queue != MessageManager.ChatQueue)
				{
					return this.m_combatFilter.GetMessageTypeFlags();
				}
				return this.m_chatFilter.GetMessageTypeFlags();
			}
		}

		// Token: 0x17001064 RID: 4196
		// (get) Token: 0x06004A12 RID: 18962 RVA: 0x00071DEB File Offset: 0x0006FFEB
		// (set) Token: 0x06004A13 RID: 18963 RVA: 0x001B1794 File Offset: 0x001AF994
		public ChatMessageQueue Queue
		{
			get
			{
				return this.m_queue;
			}
			set
			{
				if (this.m_queue != null)
				{
					this.m_queue.QueueCleared -= this.OnQueueCleared;
					this.m_queue.MessageAddedQueueTrimmed -= this.OnMessageAddedQueueTrimmed;
					this.m_queue.MessageRemoved -= this.OnMessageRemoved;
				}
				this.m_queue = value;
				if (this.m_queue != null)
				{
					this.m_queue.QueueCleared += this.OnQueueCleared;
					this.m_queue.MessageAddedQueueTrimmed += this.OnMessageAddedQueueTrimmed;
					this.m_queue.MessageRemoved += this.OnMessageRemoved;
				}
				this.RefreshTabVisuals(this.m_toggle.isOn);
				if (this.m_toggle.isOn && this.m_parentWindow)
				{
					this.m_parentWindow.UpdateChatListWhenReady();
				}
			}
		}

		// Token: 0x17001065 RID: 4197
		// (get) Token: 0x06004A14 RID: 18964 RVA: 0x00071DF3 File Offset: 0x0006FFF3
		public ChatTabMode Mode
		{
			get
			{
				if (this.m_queue != MessageManager.ChatQueue)
				{
					return ChatTabMode.Combat;
				}
				return ChatTabMode.Chat;
			}
		}

		// Token: 0x140000E5 RID: 229
		// (add) Token: 0x06004A15 RID: 18965 RVA: 0x001B1878 File Offset: 0x001AFA78
		// (remove) Token: 0x06004A16 RID: 18966 RVA: 0x001B18B0 File Offset: 0x001AFAB0
		public event Action<ChatTab> TabChanged;

		// Token: 0x140000E6 RID: 230
		// (add) Token: 0x06004A17 RID: 18967 RVA: 0x001B18E8 File Offset: 0x001AFAE8
		// (remove) Token: 0x06004A18 RID: 18968 RVA: 0x001B1920 File Offset: 0x001AFB20
		public event Action<ChatTab> CloseClicked;

		// Token: 0x140000E7 RID: 231
		// (add) Token: 0x06004A19 RID: 18969 RVA: 0x001B1958 File Offset: 0x001AFB58
		// (remove) Token: 0x06004A1A RID: 18970 RVA: 0x001B1990 File Offset: 0x001AFB90
		public event Action<ChatTab> Renamed;

		// Token: 0x06004A1B RID: 18971 RVA: 0x00071E05 File Offset: 0x00070005
		private void Awake()
		{
			this.m_toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleChanged));
			this.m_internalStartingPivot = this.RectTransform.pivot;
			this.m_originalPosition = this.RectTransform.position;
		}

		// Token: 0x06004A1C RID: 18972 RVA: 0x001B19C8 File Offset: 0x001AFBC8
		private void OnDestroy()
		{
			this.m_toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnToggleChanged));
			if (this.m_queue != null)
			{
				this.m_queue.QueueCleared -= this.OnQueueCleared;
				this.m_queue.MessageAddedQueueTrimmed -= this.OnMessageAddedQueueTrimmed;
				this.m_queue.MessageRemoved -= this.OnMessageRemoved;
			}
		}

		// Token: 0x06004A1D RID: 18973 RVA: 0x00071E45 File Offset: 0x00070045
		public void Init(ChatWindowUI parentWindow, ChatMessageQueue queue, ToggleGroup group)
		{
			this.m_parentWindow = parentWindow;
			this.Queue = queue;
			this.m_toggle.isOn = false;
			this.m_toggle.group = group;
			this.RefreshTabVisuals(this.m_toggle.isOn);
		}

		// Token: 0x06004A1E RID: 18974 RVA: 0x00071E7E File Offset: 0x0007007E
		public void Reset()
		{
			this.Queue = null;
			this.m_name = null;
			this.m_chatFilter = ChatFilter.All;
			this.m_combatFilter = CombatFilter.All;
			this.m_inputChannel = MessageType.Say;
			this.m_previousInputChannel = MessageType.None;
		}

		// Token: 0x06004A1F RID: 18975 RVA: 0x001B1A40 File Offset: 0x001AFC40
		public void SetInputChannel(MessageType type)
		{
			if (type.CanSpeakInChannel())
			{
				if (this.m_inputChannel.CanBeDefaultChannel())
				{
					this.m_previousInputChannel = this.m_inputChannel;
				}
				this.m_inputChannel = type;
				this.RefreshTabVisuals(this.m_toggle.isOn);
				return;
			}
			if ((type == MessageType.World || type == MessageType.Trade) && SessionData.IsTrial)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "You must purchase the game to use that channel. Use \"/help\" instead.");
				return;
			}
			if (type == MessageType.Subscriber && !SessionData.IsSubscriber)
			{
				MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "You must have an active subscription to use that channel.");
				return;
			}
			MessageManager.ChatQueue.AddToQueue(MessageType.Notification, "You do not currently have access to that channel.");
		}

		// Token: 0x06004A20 RID: 18976 RVA: 0x00071EB0 File Offset: 0x000700B0
		public void RevertInputChannel()
		{
			this.m_inputChannel = this.m_previousInputChannel;
			this.RefreshTabVisuals(this.m_toggle.isOn);
		}

		// Token: 0x06004A21 RID: 18977 RVA: 0x001B1AE4 File Offset: 0x001AFCE4
		public void RefreshTabVisuals(bool isOn)
		{
			if (this.m_toggleRect == null)
			{
				this.m_toggle.TryGetComponent<RectTransform>(out this.m_toggleRect);
			}
			if (!string.IsNullOrWhiteSpace(this.m_name))
			{
				this.m_toggleText.text = this.m_name;
			}
			else if (this.Mode == ChatTabMode.Chat)
			{
				Color color;
				this.InputChannel.GetColor(out color, false);
				this.m_toggleText.text = string.Format("{0} - <color={1}>{2}", this.Mode, color.ToHex(), this.InputChannel);
			}
			else
			{
				this.m_toggleText.text = this.Mode.ToString();
			}
			this.m_toggleText.ForceMeshUpdate(true, false);
			this.m_toggleText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Min(this.m_toggleText.preferredWidth, 200f));
			this.m_toggleRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Min(this.m_toggleText.preferredWidth, 200f) + 25f);
			float a = isOn ? 1f : 0.2f;
			this.m_toggleText.color = new Color(this.m_toggleText.color.r, this.m_toggleText.color.g, this.m_toggleText.color.b, a);
			this.m_closeButtonImage.color = new Color(this.m_closeButtonImage.color.r, this.m_closeButtonImage.color.g, this.m_closeButtonImage.color.b, a);
			this.m_frame.color = new Color(this.m_frame.color.r, this.m_frame.color.g, this.m_frame.color.b, a);
		}

		// Token: 0x06004A22 RID: 18978 RVA: 0x00071ECF File Offset: 0x000700CF
		private void OnToggleChanged(bool isOn)
		{
			this.RefreshTabVisuals(isOn);
			if (isOn)
			{
				Action<ChatTab> tabChanged = this.TabChanged;
				if (tabChanged == null)
				{
					return;
				}
				tabChanged(this);
			}
		}

		// Token: 0x06004A23 RID: 18979 RVA: 0x00071EEC File Offset: 0x000700EC
		private void OnCloseClicked()
		{
			Action<ChatTab> closeClicked = this.CloseClicked;
			if (closeClicked == null)
			{
				return;
			}
			closeClicked(this);
		}

		// Token: 0x06004A24 RID: 18980 RVA: 0x00071EFF File Offset: 0x000700FF
		private void OnQueueCleared()
		{
			if (!this.m_toggle.isOn)
			{
				return;
			}
			this.m_parentWindow.UpdateChatListWhenReady();
		}

		// Token: 0x06004A25 RID: 18981 RVA: 0x001B1CC8 File Offset: 0x001AFEC8
		private void OnMessageAddedQueueTrimmed(ChatMessage message, bool trimmed)
		{
			if (message == null || !this.m_toggle.isOn)
			{
				return;
			}
			if (this.CurrentMessageTypeFilter.HasBitFlag(message.Type))
			{
				if (this.m_parentWindow.ChatList.IsInitialized)
				{
					this.m_parentWindow.ChatList.AddSingleItem(message);
					return;
				}
				this.m_parentWindow.UpdateChatList(this);
			}
		}

		// Token: 0x06004A26 RID: 18982 RVA: 0x00071F1A File Offset: 0x0007011A
		private void OnMessageRemoved(ChatMessage message)
		{
			this.m_parentWindow.ChatList.RemoveItem(message);
		}

		// Token: 0x06004A27 RID: 18983 RVA: 0x001B1D2C File Offset: 0x001AFF2C
		private void RenameTab()
		{
			DialogOptions opts = new DialogOptions
			{
				Title = "Rename Tab",
				Text = this.m_name,
				ConfirmationText = "Ok",
				CancelText = "Cancel",
				ShowCloseButton = false,
				Callback = new Action<bool, object>(this.OnRenameTabConfirmed),
				AsciiOnly = true
			};
			ClientGameManager.UIManager.TextEntryDialog.Init(opts);
		}

		// Token: 0x06004A28 RID: 18984 RVA: 0x00071F2D File Offset: 0x0007012D
		private void OnRenameTabConfirmed(bool answer, object obj)
		{
			if (answer)
			{
				this.Name = (string)obj;
			}
		}

		// Token: 0x06004A29 RID: 18985 RVA: 0x00071F3E File Offset: 0x0007013E
		private void ClearName()
		{
			this.Name = null;
		}

		// Token: 0x06004A2A RID: 18986 RVA: 0x001B1DA8 File Offset: 0x001AFFA8
		private void SetPivot(PointerEventData eventData)
		{
			this.m_previousPivot = new Vector2?(this.RectTransform.pivot);
			Vector2 vector;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.RectTransform, eventData.position, eventData.pressEventCamera, out vector);
			vector.x = vector.x / this.RectTransform.sizeDelta.x + this.RectTransform.pivot.x;
			vector.y = vector.y / this.RectTransform.sizeDelta.y + this.RectTransform.pivot.y;
			this.RectTransform.SetPivot(vector);
		}

		// Token: 0x06004A2B RID: 18987 RVA: 0x00071F47 File Offset: 0x00070147
		private void SetPivot(Vector2 newPivot)
		{
			this.m_previousPivot = new Vector2?(this.RectTransform.pivot);
			this.RectTransform.SetPivot(newPivot);
		}

		// Token: 0x06004A2C RID: 18988 RVA: 0x001B1E50 File Offset: 0x001B0050
		private void ResetPivot()
		{
			if (this.m_previousPivot != null)
			{
				this.RectTransform.SetPivot(this.m_previousPivot.Value);
				this.m_previousPivot = null;
				return;
			}
			this.RectTransform.SetPivot(this.m_internalStartingPivot);
		}

		// Token: 0x17001066 RID: 4198
		// (get) Token: 0x06004A2D RID: 18989 RVA: 0x00071F6B File Offset: 0x0007016B
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return this.m_interactionSettings;
			}
		}

		// Token: 0x06004A2E RID: 18990 RVA: 0x001B1EA0 File Offset: 0x001B00A0
		string IContextMenu.FillActionsGetTitle()
		{
			ContextMenuUI.AddContextAction("Set Custom Name", true, new Action(this.RenameTab), null, null);
			ContextMenuUI.AddContextAction("Clear Custom Name", this.m_name != null, new Action(this.ClearName), null, null);
			ContextMenuUI.AddContextAction(ZString.Format<string>("{0} Close Tab", "<sprite=\"SolIcons\" name=\"PassIcon\" tint=1>"), true, new Action(this.OnCloseClicked), null, null);
			if (string.IsNullOrWhiteSpace(this.m_name))
			{
				return string.Format("{0} - {1}", this.Mode, this.InputChannel);
			}
			return this.m_name;
		}

		// Token: 0x17001067 RID: 4199
		// (get) Token: 0x06004A2F RID: 18991 RVA: 0x00071F73 File Offset: 0x00070173
		public RectTransform RectTransform
		{
			get
			{
				return this.m_self.rectTransform;
			}
		}

		// Token: 0x06004A30 RID: 18992 RVA: 0x00071F80 File Offset: 0x00070180
		public void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				this.SetPivot(eventData);
				return;
			}
			if (this.m_isDragging)
			{
				this.m_isDragging = false;
				this.ResetPivot();
				this.RectTransform.SetPositionAndRotation(this.m_originalPosition, Quaternion.identity);
			}
		}

		// Token: 0x06004A31 RID: 18993 RVA: 0x00071FBD File Offset: 0x000701BD
		public void OnPointerUp(PointerEventData eventData)
		{
			this.ResetPivot();
		}

		// Token: 0x06004A32 RID: 18994 RVA: 0x001B1F40 File Offset: 0x001B0140
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left && !this.m_parentWindow.Locked)
			{
				base.gameObject.transform.position = eventData.position;
				this.m_originalPosition = this.RectTransform.position;
				this.m_windowPositionOffset = this.m_parentWindow.RectTransform.position - this.m_originalPosition;
				if (ClientGameManager.UIManager != null)
				{
					ClientGameManager.UIManager.RegisterDrag(this);
				}
				this.m_isDragging = true;
			}
		}

		// Token: 0x06004A33 RID: 18995 RVA: 0x001B1FD0 File Offset: 0x001B01D0
		public void OnEndDrag(PointerEventData eventData)
		{
			if (this.m_isDragging)
			{
				if (ClientGameManager.UIManager != null)
				{
					ClientGameManager.UIManager.DeregisterDrag(false);
				}
				this.m_isDragging = false;
				if (this.m_insertionHighlightsNeedHiding)
				{
					foreach (ChatWindowUI chatWindowUI in UIManager.ChatWindows)
					{
						foreach (ChatTab chatTab in chatWindowUI.TabController.Tabs)
						{
							chatTab.SetInsertionArrow(false, false);
						}
					}
					this.m_insertionHighlightsNeedHiding = false;
				}
				ChatWindowUI chatWindowUI2;
				int num;
				ChatTab chatTab2;
				int num2;
				this.CalculateDragResult(out chatWindowUI2, out num, out chatTab2, out num2);
				if (chatWindowUI2 == null && this.m_parentWindow.TabController.Tabs.Count == 1)
				{
					this.m_parentWindow.RectTransform.SetPositionAndRotation(new Vector3(eventData.position.x, eventData.position.y, 0f) + this.m_windowPositionOffset, Quaternion.identity);
					this.m_parentWindow.RectTransform.ClampToScreen();
					return;
				}
				if (chatWindowUI2 == null && this.m_parentWindow.TabController.Tabs.Count > 1 && UIManager.ChatWindows.Count < 10)
				{
					this.m_queue.QueueCleared -= this.OnQueueCleared;
					this.m_queue.MessageAddedQueueTrimmed -= this.OnMessageAddedQueueTrimmed;
					this.m_queue.MessageRemoved -= this.OnMessageRemoved;
					this.m_parentWindow.TabController.TransferTab(this, new Vector3(eventData.position.x, eventData.position.y, 0f) + this.m_windowPositionOffset);
					return;
				}
				if (chatWindowUI2 != null && (chatWindowUI2 != this.m_parentWindow || (chatWindowUI2 == this.m_parentWindow && this.m_parentWindow.TabController.Tabs.Count > 1) || (chatWindowUI2 == this.m_parentWindow && this.m_parentWindow.TabController.Tabs.IndexOf(this) != num && num != -1)))
				{
					this.m_queue.QueueCleared -= this.OnQueueCleared;
					this.m_queue.MessageAddedQueueTrimmed -= this.OnMessageAddedQueueTrimmed;
					this.m_queue.MessageRemoved -= this.OnMessageRemoved;
					this.m_parentWindow.TabController.TransferTab(this, chatWindowUI2, num);
				}
			}
		}

		// Token: 0x06004A34 RID: 18996 RVA: 0x001B2298 File Offset: 0x001B0498
		public void OnDrag(PointerEventData eventData)
		{
			if (this.m_isDragging)
			{
				base.gameObject.transform.position = eventData.position;
				this.RectTransform.ClampToScreen();
				ChatWindowUI chatWindowUI;
				int num;
				ChatTab chatTab;
				int num2;
				this.CalculateDragResult(out chatWindowUI, out num, out chatTab, out num2);
				if (chatWindowUI != null)
				{
					int count = chatWindowUI.TabController.Tabs.Count;
					ChatTab chatTab2 = null;
					if (count >= 1)
					{
						List<ChatTab> tabs = chatWindowUI.TabController.Tabs;
						if (tabs[tabs.Count - 1] != this)
						{
							List<ChatTab> tabs2 = chatWindowUI.TabController.Tabs;
							chatTab2 = tabs2[tabs2.Count - 1];
							goto IL_E0;
						}
					}
					if (count > 2)
					{
						List<ChatTab> tabs3 = chatWindowUI.TabController.Tabs;
						if (tabs3[tabs3.Count - 1] == this)
						{
							List<ChatTab> tabs4 = chatWindowUI.TabController.Tabs;
							chatTab2 = tabs4[tabs4.Count - 2];
						}
					}
					IL_E0:
					if (this.m_previouslyHighlightedTab != null)
					{
						this.m_previouslyHighlightedTab.SetInsertionArrow(false, false);
					}
					if (chatTab != null)
					{
						if (num == num2)
						{
							chatTab.SetInsertionArrow(true, false);
							this.m_previouslyHighlightedTab = chatTab;
							this.m_insertionHighlightsNeedHiding = true;
							return;
						}
						if (num > num2)
						{
							chatTab.SetInsertionArrow(false, true);
							this.m_previouslyHighlightedTab = chatTab;
							this.m_insertionHighlightsNeedHiding = true;
							return;
						}
					}
					else
					{
						if (chatTab2 != null)
						{
							chatTab2.SetInsertionArrow(false, true);
							this.m_previouslyHighlightedTab = chatTab2;
							this.m_insertionHighlightsNeedHiding = true;
							return;
						}
						this.m_previouslyHighlightedTab = null;
						this.m_insertionHighlightsNeedHiding = false;
						return;
					}
				}
				else if (this.m_insertionHighlightsNeedHiding)
				{
					foreach (ChatWindowUI chatWindowUI2 in UIManager.ChatWindows)
					{
						foreach (ChatTab chatTab3 in chatWindowUI2.TabController.Tabs)
						{
							chatTab3.SetInsertionArrow(false, false);
						}
					}
					this.m_previouslyHighlightedTab = null;
					this.m_insertionHighlightsNeedHiding = false;
				}
			}
		}

		// Token: 0x06004A35 RID: 18997 RVA: 0x00071FC5 File Offset: 0x000701C5
		public void SetInsertionArrow(bool left, bool right)
		{
			this.m_insertIconLeft.gameObject.SetActive(left);
			this.m_insertIconRight.gameObject.SetActive(right);
		}

		// Token: 0x17001068 RID: 4200
		// (get) Token: 0x06004A36 RID: 18998 RVA: 0x00045BCA File Offset: 0x00043DCA
		public bool ExternallyHandlePositionUpdate
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06004A37 RID: 18999 RVA: 0x0004475B File Offset: 0x0004295B
		public void CompleteDrag(bool canceled)
		{
		}

		// Token: 0x06004A38 RID: 19000 RVA: 0x001B24B0 File Offset: 0x001B06B0
		private void CalculateDragResult(out ChatWindowUI destinationWindow, out int insertionIndex, out ChatTab destinationTab, out int tabIndex)
		{
			Vector3 mousePosition = Input.mousePosition;
			destinationWindow = null;
			insertionIndex = -1;
			destinationTab = null;
			tabIndex = -1;
			foreach (ChatWindowUI chatWindowUI in UIManager.ChatWindows)
			{
				chatWindowUI.RectTransform.GetWorldCorners(this.m_fourCorners);
				if (mousePosition.x > this.m_fourCorners[0].x && mousePosition.y > this.m_fourCorners[0].y && mousePosition.x < this.m_fourCorners[2].x && mousePosition.y < this.m_fourCorners[2].y)
				{
					destinationWindow = chatWindowUI;
					destinationWindow.TabController.ScrollRect.content.GetWorldCorners(this.m_fourCorners);
					if (mousePosition.x > this.m_fourCorners[0].x && mousePosition.y > this.m_fourCorners[0].y && mousePosition.x < this.m_fourCorners[2].x && mousePosition.y < this.m_fourCorners[2].y)
					{
						bool flag = false;
						for (int i = 0; i < destinationWindow.TabController.Tabs.Count; i++)
						{
							if (destinationWindow.TabController.Tabs[i] == this)
							{
								flag = true;
							}
							else
							{
								destinationWindow.TabController.Tabs[i].RectTransform.GetWorldCorners(this.m_fourCorners);
								if (mousePosition.x > this.m_fourCorners[0].x && mousePosition.y > this.m_fourCorners[0].y && mousePosition.x < this.m_fourCorners[2].x && mousePosition.y < this.m_fourCorners[2].y)
								{
									destinationTab = destinationWindow.TabController.Tabs[i];
									tabIndex = (flag ? (i - 1) : i);
									float num = this.m_fourCorners[0].x + (this.m_fourCorners[2].x - this.m_fourCorners[0].x) / 2f;
									if (mousePosition.x < num)
									{
										insertionIndex = (flag ? (i - 1) : i);
										break;
									}
									insertionIndex = (flag ? i : (i + 1));
									break;
								}
							}
						}
						break;
					}
					insertionIndex = ((this.m_parentWindow == destinationWindow) ? (destinationWindow.TabController.Tabs.Count - 1) : destinationWindow.TabController.Tabs.Count);
					break;
				}
			}
		}

		// Token: 0x06004A3A RID: 19002 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004505 RID: 17669
		[SerializeField]
		private SolToggle m_toggle;

		// Token: 0x04004506 RID: 17670
		[SerializeField]
		private TextMeshProUGUI m_toggleText;

		// Token: 0x04004507 RID: 17671
		[SerializeField]
		private Image m_closeButtonImage;

		// Token: 0x04004508 RID: 17672
		[SerializeField]
		private Image m_frame;

		// Token: 0x04004509 RID: 17673
		[SerializeField]
		private Image m_self;

		// Token: 0x0400450A RID: 17674
		[SerializeField]
		private Image m_insertIconLeft;

		// Token: 0x0400450B RID: 17675
		[SerializeField]
		private Image m_insertIconRight;

		// Token: 0x0400450C RID: 17676
		[SerializeField]
		private InteractionSettings m_interactionSettings;

		// Token: 0x0400450D RID: 17677
		private const float kMaxTabWidth = 200f;

		// Token: 0x0400450E RID: 17678
		private ChatWindowUI m_parentWindow;

		// Token: 0x0400450F RID: 17679
		private ChatMessageQueue m_queue;

		// Token: 0x04004510 RID: 17680
		private string m_name;

		// Token: 0x04004511 RID: 17681
		private ChatFilter m_chatFilter = ChatFilter.All;

		// Token: 0x04004512 RID: 17682
		private CombatFilter m_combatFilter = CombatFilter.All;

		// Token: 0x04004513 RID: 17683
		private MessageType m_inputChannel = MessageType.Say;

		// Token: 0x04004514 RID: 17684
		private MessageType m_previousInputChannel;

		// Token: 0x04004518 RID: 17688
		private RectTransform m_toggleRect;

		// Token: 0x04004519 RID: 17689
		private Vector2 m_internalStartingPivot = Vector2.zero;

		// Token: 0x0400451A RID: 17690
		private Vector3 m_originalPosition = Vector3.zero;

		// Token: 0x0400451B RID: 17691
		private Vector3 m_windowPositionOffset = Vector3.zero;

		// Token: 0x0400451C RID: 17692
		private Vector2? m_previousPivot;

		// Token: 0x0400451D RID: 17693
		private bool m_isDragging;

		// Token: 0x0400451E RID: 17694
		private bool m_insertionHighlightsNeedHiding;

		// Token: 0x0400451F RID: 17695
		private ChatTab m_previouslyHighlightedTab;

		// Token: 0x04004520 RID: 17696
		private readonly Vector3[] m_fourCorners = new Vector3[4];
	}
}
