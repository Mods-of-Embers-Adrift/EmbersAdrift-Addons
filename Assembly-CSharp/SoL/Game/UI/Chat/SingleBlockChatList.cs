using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Messages;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI.Chat
{
	// Token: 0x020009B9 RID: 2489
	public class SingleBlockChatList : MonoBehaviour
	{
		// Token: 0x06004B33 RID: 19251 RVA: 0x001B800C File Offset: 0x001B620C
		private void Start()
		{
			this.m_zsb = ZString.CreateStringBuilder();
			this.m_queue = this.m_chatTab.Queue;
			this.m_queue.QueueCleared += this.QueueOnQueueCleared;
			this.m_queue.MessageAddedQueueTrimmed += this.QueueOnMessageAddedQueueTrimmed;
			this.m_showTimestampToggle.isOn = false;
			this.m_showTimestampToggle.onValueChanged.AddListener(new UnityAction<bool>(this.ShowTimestampsChanged));
			this.m_chatTab.ResizeBegin += this.ChatTabOnResizeBegin;
			this.m_chatTab.ResizeDrag += this.ChatTabOnResizeDrag;
			this.m_chatTab.ResizeFinish += this.ChatTabOnResizeFinish;
			Options.GameOptions.ChatFontSize.Changed += this.ChatFontSizeOnChanged;
			this.ChatFontSizeOnChanged();
			this.SetupFilters();
			this.RefreshContents();
		}

		// Token: 0x06004B34 RID: 19252 RVA: 0x001B80F8 File Offset: 0x001B62F8
		private void OnDestroy()
		{
			this.m_queue.QueueCleared -= this.QueueOnQueueCleared;
			this.m_queue.MessageAddedQueueTrimmed -= this.QueueOnMessageAddedQueueTrimmed;
			this.m_showTimestampToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.ShowTimestampsChanged));
			this.m_chatTab.ResizeBegin -= this.ChatTabOnResizeBegin;
			this.m_chatTab.ResizeDrag -= this.ChatTabOnResizeDrag;
			this.m_chatTab.ResizeFinish -= this.ChatTabOnResizeFinish;
			Options.GameOptions.ChatFontSize.Changed -= this.ChatFontSizeOnChanged;
			this.DestroyFilters();
			this.m_zsb.Dispose();
		}

		// Token: 0x06004B35 RID: 19253 RVA: 0x001B81BC File Offset: 0x001B63BC
		private void Update()
		{
			if (this.m_filterWindow.Visible && !this.m_filterWindow.CursorInside && InteractionManager.HoveredUIElement != this.m_filterButton.gameObject && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
			{
				this.m_filterWindow.Hide(false);
			}
			if (this.m_updateScrollFrame != null && this.m_updateScrollFrame.Value < Time.frameCount)
			{
				this.m_scrollRect.verticalNormalizedPosition = 0f;
				this.m_updateScrollFrame = null;
			}
		}

		// Token: 0x06004B36 RID: 19254 RVA: 0x00072E18 File Offset: 0x00071018
		private void ChatFontSizeOnChanged()
		{
			this.m_text.fontSize = (float)Options.GameOptions.ChatFontSize.Value;
			this.ConditionallyScrollToBottom();
			this.RefreshSize();
		}

		// Token: 0x06004B37 RID: 19255 RVA: 0x00072E3C File Offset: 0x0007103C
		private void ChatTabOnResizeBegin()
		{
			this.m_cachedVerticalPosition = new float?(this.m_scrollRect.verticalNormalizedPosition);
		}

		// Token: 0x06004B38 RID: 19256 RVA: 0x00072E54 File Offset: 0x00071054
		private void ChatTabOnResizeDrag()
		{
			if (this.m_cachedVerticalPosition != null)
			{
				this.m_scrollRect.verticalNormalizedPosition = this.m_cachedVerticalPosition.Value;
			}
		}

		// Token: 0x06004B39 RID: 19257 RVA: 0x00072E79 File Offset: 0x00071079
		private void ChatTabOnResizeFinish()
		{
			this.m_cachedVerticalPosition = null;
		}

		// Token: 0x06004B3A RID: 19258 RVA: 0x001B8254 File Offset: 0x001B6454
		private void QueueOnMessageAddedQueueTrimmed(ChatMessage msg, bool trimmed)
		{
			if (msg == null)
			{
				return;
			}
			if (trimmed)
			{
				this.RefreshContents();
				return;
			}
			if (this.m_filter.HasBitFlag(msg.Type))
			{
				this.m_zsb.AppendLine(msg.GetCachedFormattedMessage(this.m_showTimestampToggle.isOn));
				this.m_text.SetText(this.m_zsb);
				this.RefreshSize();
				this.ConditionallyScrollToBottom();
			}
		}

		// Token: 0x06004B3B RID: 19259 RVA: 0x00072E87 File Offset: 0x00071087
		private void QueueOnQueueCleared()
		{
			this.RefreshContents();
		}

		// Token: 0x06004B3C RID: 19260 RVA: 0x00072E87 File Offset: 0x00071087
		private void ShowTimestampsChanged(bool arg0)
		{
			this.RefreshContents();
		}

		// Token: 0x06004B3D RID: 19261 RVA: 0x001B82BC File Offset: 0x001B64BC
		private void SetupFilters()
		{
			this.m_filterButton.onClick.AddListener(new UnityAction(this.FilterButtonClicked));
			this.m_filters = new List<MessageType>();
			int num = 0;
			for (int i = 0; i < MessageTypeExtensions.MessageTypes.Length; i++)
			{
				MessageType messageType = MessageTypeExtensions.MessageTypes[i];
				if (messageType != MessageType.None && this.m_availableFilters.HasBitFlag(messageType))
				{
					string text = messageType.GetFilterDisplayChannel();
					if (!string.IsNullOrEmpty(text))
					{
						Color color;
						if (messageType.GetColor(out color, false))
						{
							text = text.Color(color);
						}
						this.m_filters.Add(messageType);
						this.m_filterToggles[num].text = text;
						this.m_filterToggles[num].onValueChanged.AddListener(new UnityAction<bool>(this.FilterChanged));
						num++;
					}
				}
			}
			for (int j = num; j < this.m_filterToggles.Length; j++)
			{
				this.m_filterToggles[j].gameObject.SetActive(false);
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.m_filterWindow.RectTransform);
			this.m_contentSizeFitter.enabled = false;
			this.m_layoutGroup.enabled = false;
		}

		// Token: 0x06004B3E RID: 19262 RVA: 0x001B83D4 File Offset: 0x001B65D4
		private void DestroyFilters()
		{
			this.m_filterButton.onClick.RemoveListener(new UnityAction(this.FilterButtonClicked));
			for (int i = 0; i < this.m_filters.Count; i++)
			{
				if (this.m_filterToggles[i] != null)
				{
					this.m_filterToggles[i].onValueChanged.RemoveListener(new UnityAction<bool>(this.FilterChanged));
				}
			}
		}

		// Token: 0x06004B3F RID: 19263 RVA: 0x00072E8F File Offset: 0x0007108F
		private void FilterButtonClicked()
		{
			this.m_filterWindow.ToggleWindow();
		}

		// Token: 0x06004B40 RID: 19264 RVA: 0x001B8444 File Offset: 0x001B6644
		private void FilterChanged(bool isOn)
		{
			MessageType messageType = MessageType.All;
			for (int i = 0; i < this.m_filters.Count; i++)
			{
				if (!this.m_filterToggles[i].isOn)
				{
					messageType &= ~this.m_filters[i];
				}
			}
			if (this.m_filter != messageType)
			{
				this.m_filter = messageType;
				this.RefreshContents();
			}
		}

		// Token: 0x06004B41 RID: 19265 RVA: 0x001B84A4 File Offset: 0x001B66A4
		private void RefreshContents()
		{
			this.m_zsb.Clear();
			for (int i = 0; i < this.m_queue.Count; i++)
			{
				ChatMessage messageAtIndex = this.m_queue.GetMessageAtIndex(i);
				if (messageAtIndex != null && this.m_filter.HasBitFlag(messageAtIndex.Type))
				{
					this.m_zsb.AppendLine(messageAtIndex.GetCachedFormattedMessage(this.m_showTimestampToggle.isOn));
				}
			}
			this.m_text.SetText(this.m_zsb);
			this.RefreshSize();
			this.ConditionallyScrollToBottom();
		}

		// Token: 0x06004B42 RID: 19266 RVA: 0x001B8530 File Offset: 0x001B6730
		private void RefreshSize()
		{
			float num = Mathf.Lerp(1.21f, 4.81f, (this.m_text.fontSize - 6f) / 18f);
			float num2 = this.m_text.fontSize * this.m_text.lineSpacing * 0.01f;
			float num3 = (float)this.m_queue.Count * (this.m_text.fontSize + num);
			num3 += (float)(this.m_queue.Count - 1) * num2;
			num3 += this.m_text.margin.y + this.m_text.margin.w;
			this.m_text.rectTransform.sizeDelta = new Vector2(0f, num3);
		}

		// Token: 0x06004B43 RID: 19267 RVA: 0x00072E9C File Offset: 0x0007109C
		private void ConditionallyScrollToBottom()
		{
			if (this.m_scrollRect.verticalNormalizedPosition <= 0.1f)
			{
				this.m_updateScrollFrame = new int?(Time.frameCount + 1);
			}
		}

		// Token: 0x040045C7 RID: 17863
		private const float kSnapToBottomThreshold = 0.1f;

		// Token: 0x040045C8 RID: 17864
		[SerializeField]
		private UIWindow m_filterWindow;

		// Token: 0x040045C9 RID: 17865
		[SerializeField]
		private ChatTabUI m_chatTab;

		// Token: 0x040045CA RID: 17866
		[SerializeField]
		private ScrollRect m_scrollRect;

		// Token: 0x040045CB RID: 17867
		[SerializeField]
		private TextMeshProUGUI m_text;

		// Token: 0x040045CC RID: 17868
		[SerializeField]
		private MessageType m_availableFilters;

		// Token: 0x040045CD RID: 17869
		[SerializeField]
		private SolButton m_filterButton;

		// Token: 0x040045CE RID: 17870
		[SerializeField]
		private SolToggle[] m_filterToggles;

		// Token: 0x040045CF RID: 17871
		[SerializeField]
		private VerticalLayoutGroup m_layoutGroup;

		// Token: 0x040045D0 RID: 17872
		[SerializeField]
		private ContentSizeFitter m_contentSizeFitter;

		// Token: 0x040045D1 RID: 17873
		[SerializeField]
		private SolToggle m_showTimestampToggle;

		// Token: 0x040045D2 RID: 17874
		private ChatMessageQueue m_queue;

		// Token: 0x040045D3 RID: 17875
		private List<MessageType> m_filters;

		// Token: 0x040045D4 RID: 17876
		private MessageType m_filter = MessageType.All;

		// Token: 0x040045D5 RID: 17877
		private int? m_updateScrollFrame;

		// Token: 0x040045D6 RID: 17878
		private float? m_cachedVerticalPosition;

		// Token: 0x040045D7 RID: 17879
		private Utf16ValueStringBuilder m_zsb;
	}
}
