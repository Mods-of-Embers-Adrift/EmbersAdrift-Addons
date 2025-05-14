using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
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
	// Token: 0x020009B5 RID: 2485
	public class OptimizedChatList : OSA<ChatLineParams, ChatLineViewsHolder>
	{
		// Token: 0x17001082 RID: 4226
		// (get) Token: 0x06004B06 RID: 19206 RVA: 0x00072AC9 File Offset: 0x00070CC9
		// (set) Token: 0x06004B07 RID: 19207 RVA: 0x00072AD1 File Offset: 0x00070CD1
		private bool RecalculatePreferredHeights
		{
			get
			{
				return this.m_recalculatePreferredHeights;
			}
			set
			{
				this.m_recalculatePreferredHeights = value;
				if (this.m_recalculatePreferredHeights)
				{
					this.m_lastRecalculatePreferredHeights = UnityEngine.Time.time;
				}
			}
		}

		// Token: 0x06004B08 RID: 19208 RVA: 0x001B7834 File Offset: 0x001B5A34
		protected override void Start()
		{
			base.Start();
			this.m_queue = this.m_chatTab.Queue;
			this.SetupPlayerPrefKeys();
			this.m_chatTab.ResizeBegin += this.ChatTabOnResizeBegin;
			this.m_chatTab.ResizeDrag += this.ChatTabOnResizeDrag;
			this.m_chatTab.ResizeFinish += this.ChatTabOnResizeFinish;
			this.m_showTimestampToggle.isOn = this.ShowTimestampsPref;
			this.m_showTimestampToggle.onValueChanged.AddListener(new UnityAction<bool>(this.ShowTimestampsChanged));
			this.m_samplerText.gameObject.SetActive(false);
			Options.GameOptions.ChatFontSize.Changed += this.ChatFontSizeOnChanged;
			this.m_samplerText.fontSize = (float)Options.GameOptions.ChatFontSize.Value;
			MessageType messageFilterPref = this.MessageFilterPref;
			this.SetupFilters(messageFilterPref);
			this.m_data = new OptimizedChatList.InternalMessageData(this);
			this.m_data.Filter = messageFilterPref;
			Transform[] componentsInChildren = this.m_filterWindow.gameObject.GetComponentsInChildren<Transform>();
			this.m_nestedMenuGameObjects = new HashSet<GameObject>
			{
				this.m_filterWindow.gameObject
			};
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				this.m_nestedMenuGameObjects.Add(componentsInChildren[i].gameObject);
			}
		}

		// Token: 0x06004B09 RID: 19209 RVA: 0x001B7984 File Offset: 0x001B5B84
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.m_data.OnDestroy();
			this.m_chatTab.ResizeBegin -= this.ChatTabOnResizeBegin;
			this.m_chatTab.ResizeDrag -= this.ChatTabOnResizeDrag;
			this.m_chatTab.ResizeFinish -= this.ChatTabOnResizeFinish;
			this.m_showTimestampToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.ShowTimestampsChanged));
			Options.GameOptions.ChatFontSize.Changed -= this.ChatFontSizeOnChanged;
			this.DestroyFilters();
		}

		// Token: 0x06004B0A RID: 19210 RVA: 0x001B7A20 File Offset: 0x001B5C20
		protected override void Update()
		{
			base.Update();
			if (this.m_filterWindow.Visible && !this.m_filterWindow.CursorInside && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && (InteractionManager.HoveredUIElement == null || !this.m_nestedMenuGameObjects.Contains(InteractionManager.HoveredUIElement)))
			{
				this.m_filterWindow.Hide(false);
			}
		}

		// Token: 0x06004B0B RID: 19211 RVA: 0x00072AED File Offset: 0x00070CED
		private void SetupPlayerPrefKeys()
		{
			this.m_messageFilterPlayerPrefsKey = this.m_chatTab.PlayerPrefsKey + "_MessageFilter";
			this.m_showTimestampsPlayerPrefsKey = this.m_chatTab.PlayerPrefsKey + "_ShowTimestamps";
		}

		// Token: 0x17001083 RID: 4227
		// (get) Token: 0x06004B0C RID: 19212 RVA: 0x00072B25 File Offset: 0x00070D25
		// (set) Token: 0x06004B0D RID: 19213 RVA: 0x00072B45 File Offset: 0x00070D45
		private bool ShowTimestampsPref
		{
			get
			{
				return !string.IsNullOrEmpty(this.m_showTimestampsPlayerPrefsKey) && PlayerPrefs.GetInt(this.m_showTimestampsPlayerPrefsKey, 0) == 1;
			}
			set
			{
				if (!string.IsNullOrEmpty(this.m_showTimestampsPlayerPrefsKey))
				{
					PlayerPrefs.SetInt(this.m_showTimestampsPlayerPrefsKey, value ? 1 : 0);
				}
			}
		}

		// Token: 0x17001084 RID: 4228
		// (get) Token: 0x06004B0E RID: 19214 RVA: 0x00072B66 File Offset: 0x00070D66
		// (set) Token: 0x06004B0F RID: 19215 RVA: 0x00072B8B File Offset: 0x00070D8B
		private MessageType MessageFilterPref
		{
			get
			{
				if (string.IsNullOrEmpty(this.m_messageFilterPlayerPrefsKey))
				{
					return MessageType.All;
				}
				return (MessageType)PlayerPrefs.GetInt(this.m_messageFilterPlayerPrefsKey, 1107296255);
			}
			set
			{
				if (this.m_data != null && !string.IsNullOrEmpty(this.m_messageFilterPlayerPrefsKey))
				{
					PlayerPrefs.SetInt(this.m_messageFilterPlayerPrefsKey, (int)value);
				}
			}
		}

		// Token: 0x06004B10 RID: 19216 RVA: 0x001B7A88 File Offset: 0x001B5C88
		private void SetupFilters(MessageType activeFilter)
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
						this.m_filterToggles[num].isOn = activeFilter.HasBitFlag(messageType);
						this.m_filterToggles[num].onValueChanged.AddListener(new UnityAction<bool>(this.ToggleChanged));
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

		// Token: 0x06004B11 RID: 19217 RVA: 0x001B7BB8 File Offset: 0x001B5DB8
		private void DestroyFilters()
		{
			this.m_filterButton.onClick.RemoveListener(new UnityAction(this.FilterButtonClicked));
			for (int i = 0; i < this.m_filters.Count; i++)
			{
				if (this.m_filterToggles[i] != null)
				{
					this.m_filterToggles[i].onValueChanged.RemoveListener(new UnityAction<bool>(this.ToggleChanged));
				}
			}
		}

		// Token: 0x06004B12 RID: 19218 RVA: 0x001B7C28 File Offset: 0x001B5E28
		private void ToggleChanged(bool isOn)
		{
			MessageType messageType = MessageType.All;
			for (int i = 0; i < this.m_filters.Count; i++)
			{
				if (!this.m_filterToggles[i].isOn)
				{
					messageType &= ~this.m_filters[i];
				}
			}
			this.m_data.Filter = messageType;
			this.MessageFilterPref = messageType;
		}

		// Token: 0x06004B13 RID: 19219 RVA: 0x00072BAE File Offset: 0x00070DAE
		private void ChatTabOnResizeBegin()
		{
			this.m_cachedVerticalPosition = new double?(base.GetNormalizedPosition());
		}

		// Token: 0x06004B14 RID: 19220 RVA: 0x00072BC1 File Offset: 0x00070DC1
		private void ChatTabOnResizeDrag()
		{
			this.RecalculatePreferredHeights = true;
			this.m_data.RefreshContents();
			if (this.m_cachedVerticalPosition != null)
			{
				base.SetNormalizedPosition(this.m_cachedVerticalPosition.Value);
			}
			this.RecalculatePreferredHeights = false;
		}

		// Token: 0x06004B15 RID: 19221 RVA: 0x00072BFA File Offset: 0x00070DFA
		private void ChatTabOnResizeFinish()
		{
			this.m_cachedVerticalPosition = null;
		}

		// Token: 0x06004B16 RID: 19222 RVA: 0x00072C08 File Offset: 0x00070E08
		private void ShowTimestampsChanged(bool arg0)
		{
			this.RecalculatePreferredHeights = true;
			this.m_data.RefreshContents();
			this.RecalculatePreferredHeights = false;
			this.ShowTimestampsPref = arg0;
		}

		// Token: 0x06004B17 RID: 19223 RVA: 0x001B7C84 File Offset: 0x001B5E84
		private void ChatFontSizeOnChanged()
		{
			this.RecalculatePreferredHeights = true;
			double normalizedPosition = base.GetNormalizedPosition();
			for (int i = 0; i < base.VisibleItemsCount; i++)
			{
				ChatLineViewsHolder itemViewsHolder = base.GetItemViewsHolder(i);
				if (itemViewsHolder != null)
				{
					itemViewsHolder.UpdateFontSize();
				}
			}
			this.m_samplerText.fontSize = (float)Options.GameOptions.ChatFontSize.Value;
			this.m_data.RefreshContents();
			base.SetNormalizedPosition(normalizedPosition);
			this.RecalculatePreferredHeights = false;
		}

		// Token: 0x06004B18 RID: 19224 RVA: 0x00072C2A File Offset: 0x00070E2A
		private void FilterButtonClicked()
		{
			this.m_filterWindow.ToggleWindow();
		}

		// Token: 0x06004B19 RID: 19225 RVA: 0x00072C37 File Offset: 0x00070E37
		protected override ChatLineViewsHolder CreateViewsHolder(int itemIndex)
		{
			ChatLineViewsHolder chatLineViewsHolder = new ChatLineViewsHolder();
			chatLineViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return chatLineViewsHolder;
		}

		// Token: 0x06004B1A RID: 19226 RVA: 0x00072C5D File Offset: 0x00070E5D
		protected override void UpdateViewsHolder(ChatLineViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateChatLine(this.m_data.GetContentsForIndex(newOrRecycled.ItemIndex));
		}

		// Token: 0x06004B1B RID: 19227 RVA: 0x001B7CF4 File Offset: 0x001B5EF4
		protected override void CollectItemsSizes(ItemCountChangeMode changeMode, int count, int indexIfInsertingOrRemoving, ItemsDescriptor itemsDesc)
		{
			base.CollectItemsSizes(changeMode, count, indexIfInsertingOrRemoving, itemsDesc);
			if (changeMode != ItemCountChangeMode.RESET)
			{
				return;
			}
			if (count == 0)
			{
				return;
			}
			int num = 0;
			int num2 = num + count;
			itemsDesc.BeginChangingItemsSizes(num);
			for (int i = num; i < num2; i++)
			{
				itemsDesc[i] = (double)this.m_data.GetHeightForIndex(i);
			}
			itemsDesc.EndChangingItemsSizes();
		}

		// Token: 0x040045AD RID: 17837
		private const string kSettingsCog = "Settings Cog";

		// Token: 0x040045AE RID: 17838
		[SerializeField]
		private ChatTabUI m_chatTab;

		// Token: 0x040045AF RID: 17839
		private ChatMessageQueue m_queue;

		// Token: 0x040045B0 RID: 17840
		[SerializeField]
		private MessageType m_availableFilters;

		// Token: 0x040045B1 RID: 17841
		[SerializeField]
		private UIWindow m_filterWindow;

		// Token: 0x040045B2 RID: 17842
		[SerializeField]
		private SolButton m_filterButton;

		// Token: 0x040045B3 RID: 17843
		[SerializeField]
		private SolToggle[] m_filterToggles;

		// Token: 0x040045B4 RID: 17844
		[SerializeField]
		private VerticalLayoutGroup m_layoutGroup;

		// Token: 0x040045B5 RID: 17845
		[SerializeField]
		private ContentSizeFitter m_contentSizeFitter;

		// Token: 0x040045B6 RID: 17846
		[SerializeField]
		private SolToggle m_showTimestampToggle;

		// Token: 0x040045B7 RID: 17847
		[SerializeField]
		private TextMeshProUGUI m_samplerText;

		// Token: 0x040045B8 RID: 17848
		private List<MessageType> m_filters;

		// Token: 0x040045B9 RID: 17849
		private double? m_cachedVerticalPosition;

		// Token: 0x040045BA RID: 17850
		private string m_messageFilterPlayerPrefsKey;

		// Token: 0x040045BB RID: 17851
		private string m_showTimestampsPlayerPrefsKey;

		// Token: 0x040045BC RID: 17852
		private HashSet<GameObject> m_nestedMenuGameObjects;

		// Token: 0x040045BD RID: 17853
		private float m_lastRecalculatePreferredHeights = -1f;

		// Token: 0x040045BE RID: 17854
		private bool m_recalculatePreferredHeights;

		// Token: 0x040045BF RID: 17855
		private OptimizedChatList.InternalMessageData m_data;

		// Token: 0x020009B6 RID: 2486
		private class InternalMessageData
		{
			// Token: 0x17001085 RID: 4229
			// (get) Token: 0x06004B1D RID: 19229 RVA: 0x00072C89 File Offset: 0x00070E89
			// (set) Token: 0x06004B1E RID: 19230 RVA: 0x00072C91 File Offset: 0x00070E91
			public MessageType Filter
			{
				get
				{
					return this.m_filter;
				}
				set
				{
					if (this.m_filter == value)
					{
						return;
					}
					this.m_filter = value;
					this.GatherFilteredData();
				}
			}

			// Token: 0x06004B1F RID: 19231 RVA: 0x001B7D4C File Offset: 0x001B5F4C
			public InternalMessageData(OptimizedChatList controller)
			{
				this.m_controller = controller;
				this.m_source = new List<ChatMessage>();
				this.m_contents = new List<string>();
				this.m_heights = new List<float>();
				this.m_controller.m_queue.QueueCleared += this.QueueOnQueueCleared;
				this.m_controller.m_queue.MessageAddedQueueTrimmed += this.QueueOnMessageAddedQueueTrimmed;
				this.GatherFilteredData();
			}

			// Token: 0x06004B20 RID: 19232 RVA: 0x00072CAA File Offset: 0x00070EAA
			public void OnDestroy()
			{
				this.m_controller.m_queue.QueueCleared -= this.QueueOnQueueCleared;
				this.m_controller.m_queue.MessageAddedQueueTrimmed -= this.QueueOnMessageAddedQueueTrimmed;
			}

			// Token: 0x06004B21 RID: 19233 RVA: 0x00072CE4 File Offset: 0x00070EE4
			private void QueueOnMessageAddedQueueTrimmed(ChatMessage msg, bool trimmed)
			{
				if (msg == null)
				{
					return;
				}
				if (trimmed)
				{
					this.GatherFilteredData();
					return;
				}
				if (this.Filter.HasBitFlag(msg.Type))
				{
					this.AddMessage(msg);
					this.Refresh();
				}
			}

			// Token: 0x06004B22 RID: 19234 RVA: 0x00072D14 File Offset: 0x00070F14
			private void QueueOnQueueCleared()
			{
				this.ClearData();
				this.Refresh();
			}

			// Token: 0x06004B23 RID: 19235 RVA: 0x001B7DD0 File Offset: 0x001B5FD0
			private void Refresh()
			{
				bool flag = false;
				if (this.m_controller.VisibleItemsCount > 0)
				{
					ChatLineViewsHolder itemViewsHolder = this.m_controller.GetItemViewsHolder(this.m_controller.VisibleItemsCount - 1);
					flag = (itemViewsHolder != null && (itemViewsHolder.ItemIndex == this.m_source.Count - 1 || itemViewsHolder.ItemIndex == this.m_source.Count - 2));
				}
				this.m_controller.ResetItems(this.m_source.Count, false, false);
				if (flag)
				{
					this.m_controller.SetNormalizedPosition(0.0);
				}
			}

			// Token: 0x06004B24 RID: 19236 RVA: 0x00072D22 File Offset: 0x00070F22
			private void ClearData()
			{
				this.m_source.Clear();
				this.m_contents.Clear();
				this.m_heights.Clear();
			}

			// Token: 0x06004B25 RID: 19237 RVA: 0x001B7E6C File Offset: 0x001B606C
			private void AddMessage(ChatMessage msg)
			{
				this.m_source.Add(msg);
				string cachedFormattedMessage = msg.GetCachedFormattedMessage(this.m_controller.m_showTimestampToggle.isOn);
				this.m_contents.Add(cachedFormattedMessage);
				if (this.RecalculateHeight(msg))
				{
					msg.PreferredHeight = this.GetPreferredHeightForContent(cachedFormattedMessage);
				}
				this.m_heights.Add(msg.PreferredHeight);
			}

			// Token: 0x06004B26 RID: 19238 RVA: 0x001B7ED0 File Offset: 0x001B60D0
			private void GatherFilteredData()
			{
				this.ClearData();
				for (int i = 0; i < this.m_controller.m_queue.Count; i++)
				{
					ChatMessage messageAtIndex = this.m_controller.m_queue.GetMessageAtIndex(i);
					if (messageAtIndex != null && this.Filter.HasBitFlag(messageAtIndex.Type))
					{
						this.AddMessage(messageAtIndex);
					}
				}
				this.Refresh();
			}

			// Token: 0x06004B27 RID: 19239 RVA: 0x001B7F34 File Offset: 0x001B6134
			public void RefreshContents()
			{
				for (int i = 0; i < this.m_source.Count; i++)
				{
					this.m_contents[i] = this.m_source[i].GetCachedFormattedMessage(this.m_controller.m_showTimestampToggle.isOn);
					if (this.RecalculateHeight(this.m_source[i]))
					{
						this.m_source[i].PreferredHeight = this.GetPreferredHeightForContent(this.m_contents[i]);
					}
					this.m_heights[i] = this.m_source[i].PreferredHeight;
				}
				this.Refresh();
			}

			// Token: 0x06004B28 RID: 19240 RVA: 0x00072D45 File Offset: 0x00070F45
			public ChatMessage GetSourceForIndex(int index)
			{
				return this.m_source[index];
			}

			// Token: 0x06004B29 RID: 19241 RVA: 0x00072D53 File Offset: 0x00070F53
			public string GetContentsForIndex(int index)
			{
				return this.m_contents[index];
			}

			// Token: 0x06004B2A RID: 19242 RVA: 0x00072D61 File Offset: 0x00070F61
			public float GetHeightForIndex(int index)
			{
				return this.m_heights[index];
			}

			// Token: 0x06004B2B RID: 19243 RVA: 0x00072D6F File Offset: 0x00070F6F
			private bool RecalculateHeight(ChatMessage msg)
			{
				return this.m_controller.RecalculatePreferredHeights || msg.ShouldRecalculatePreferredHeight(this.m_controller.m_lastRecalculatePreferredHeights);
			}

			// Token: 0x06004B2C RID: 19244 RVA: 0x001B7FE4 File Offset: 0x001B61E4
			private float GetPreferredHeightForContent(string content)
			{
				TextMeshProUGUI samplerText = this.m_controller.m_samplerText;
				samplerText.ZStringSetText(content);
				samplerText.ForceMeshUpdate(true, false);
				Vector4 margin = samplerText.margin;
				RectOffset contentPadding = this.m_controller.Parameters.ContentPadding;
				float width = samplerText.rectTransform.rect.width - margin.x - margin.z - (float)contentPadding.left - (float)contentPadding.right;
				return samplerText.GetPreferredValues(width, 24f).y;
			}

			// Token: 0x040045C0 RID: 17856
			private readonly OptimizedChatList m_controller;

			// Token: 0x040045C1 RID: 17857
			private readonly List<ChatMessage> m_source;

			// Token: 0x040045C2 RID: 17858
			private readonly List<string> m_contents;

			// Token: 0x040045C3 RID: 17859
			private readonly List<float> m_heights;

			// Token: 0x040045C4 RID: 17860
			private MessageType m_filter = MessageType.All;
		}
	}
}
