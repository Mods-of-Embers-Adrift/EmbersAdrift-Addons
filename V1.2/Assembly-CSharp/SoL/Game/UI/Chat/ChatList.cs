using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Cysharp.Text;
using SoL.Game.Messages;
using TMPro;
using UnityEngine;

namespace SoL.Game.UI.Chat
{
	// Token: 0x020009A4 RID: 2468
	public class ChatList : OSA<BaseParamsWithPrefab, ChatItemViewsHolder>
	{
		// Token: 0x17001058 RID: 4184
		// (get) Token: 0x060049D9 RID: 18905 RVA: 0x000719EE File Offset: 0x0006FBEE
		// (set) Token: 0x060049DA RID: 18906 RVA: 0x000719F6 File Offset: 0x0006FBF6
		private bool ForceRecalculateAllPreferredHeights
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

		// Token: 0x17001059 RID: 4185
		// (get) Token: 0x060049DB RID: 18907 RVA: 0x00071A12 File Offset: 0x0006FC12
		public bool ShouldLockToBottom
		{
			get
			{
				return base.VisibleItemsCount <= 0 || base.GetContentSize() <= base.GetViewportSize() || base.GetNormalizedPosition() <= 0.1;
			}
		}

		// Token: 0x1700105A RID: 4186
		// (get) Token: 0x060049DC RID: 18908 RVA: 0x00071A41 File Offset: 0x0006FC41
		// (set) Token: 0x060049DD RID: 18909 RVA: 0x00071A49 File Offset: 0x0006FC49
		public bool LockedToBottom
		{
			get
			{
				return this.m_lockedToBottom;
			}
			set
			{
				if (!this.m_lockedToBottom && value)
				{
					this.ScrollToBottom(true);
					this.m_parent.NewMessageAlert.gameObject.SetActive(false);
				}
				this.m_lockedToBottom = value;
			}
		}

		// Token: 0x060049DE RID: 18910 RVA: 0x001B1048 File Offset: 0x001AF248
		public void AddSingleItem(ChatMessage item)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			double normalizedPosition = base.GetNormalizedPosition();
			this.m_items.Add(item);
			item.PreferredHeight = this.GetPreferredHeightForContent(item.GetCachedFormattedMessage(this.m_parent.ShowTimestamps));
			this.m_heights.Add(item.PreferredHeight);
			this.m_ignoreScrollEvent = true;
			this.InsertItems(this.m_items.Count - 1, 1, false, false);
			base.RequestChangeItemSizeAndUpdateLayout(this.m_items.Count - 1, item.PreferredHeight, false, true, false, false);
			if (this.LockedToBottom)
			{
				this.ScrollToBottom(true);
			}
			else
			{
				base.SetNormalizedPosition(normalizedPosition);
				this.m_parent.NewMessageAlert.gameObject.SetActive(true);
			}
			this.m_ignoreScrollEvent = false;
		}

		// Token: 0x060049DF RID: 18911 RVA: 0x001B1114 File Offset: 0x001AF314
		public void RemoveItem(ChatMessage item)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			int num = this.m_items.IndexOf(item);
			if (num < 0)
			{
				return;
			}
			double normalizedPosition = base.GetNormalizedPosition();
			this.m_items.RemoveAt(num);
			this.m_heights.RemoveAt(num);
			this.RemoveItems(num, 1, false, false);
			if (this.LockedToBottom)
			{
				this.ScrollToBottom(true);
				return;
			}
			base.SetNormalizedPosition(normalizedPosition);
		}

		// Token: 0x060049E0 RID: 18912 RVA: 0x001B1184 File Offset: 0x001AF384
		public void UpdateItems(ChatWindowUI parent, ICollection<ChatMessage> items)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.m_parent != null)
			{
				this.m_parent.ResizeBegin -= this.OnResizeBegin;
				this.m_parent.ResizeDrag -= this.OnResizeDrag;
				this.m_parent.ResizeFinish -= this.OnResizeFinish;
			}
			this.m_parent = parent;
			this.m_parent.ResizeBegin += this.OnResizeBegin;
			this.m_parent.ResizeDrag += this.OnResizeDrag;
			this.m_parent.ResizeFinish += this.OnResizeFinish;
			if (this.m_items == null)
			{
				this.m_items = new List<ChatMessage>(items.Count);
			}
			if (this.m_heights == null)
			{
				this.m_heights = new List<float>(items.Count);
			}
			this.m_items.Clear();
			this.m_heights.Clear();
			this.m_items.AddRange(items);
			for (int i = 0; i < this.m_items.Count; i++)
			{
				this.m_heights.Add(this.m_items[i].PreferredHeight);
			}
			this.RecaulculateHeights();
			this.ResetItems(this.m_items.Count, false, false);
			this.LockedToBottom = true;
		}

		// Token: 0x060049E1 RID: 18913 RVA: 0x00071A7C File Offset: 0x0006FC7C
		public void ScrollToBottom(bool ignoreEvent = false)
		{
			if (ignoreEvent)
			{
				this.m_ignoreScrollEvent = true;
			}
			base.SetNormalizedPosition(0.0);
			if (ignoreEvent)
			{
				this.m_ignoreScrollEvent = false;
			}
		}

		// Token: 0x060049E2 RID: 18914 RVA: 0x00071AA1 File Offset: 0x0006FCA1
		protected override ChatItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			ChatItemViewsHolder chatItemViewsHolder = new ChatItemViewsHolder();
			chatItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return chatItemViewsHolder;
		}

		// Token: 0x060049E3 RID: 18915 RVA: 0x00071AC7 File Offset: 0x0006FCC7
		protected override void UpdateViewsHolder(ChatItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateItem(this.m_parent, this.m_items[newOrRecycled.ItemIndex]);
		}

		// Token: 0x060049E4 RID: 18916 RVA: 0x001B12E4 File Offset: 0x001AF4E4
		protected override void CollectItemsSizes(ItemCountChangeMode changeMode, int count, int indexIfInsertingOrRemoving, ItemsDescriptor itemsDesc)
		{
			base.CollectItemsSizes(changeMode, count, indexIfInsertingOrRemoving, itemsDesc);
			if (changeMode == ItemCountChangeMode.REMOVE)
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
				itemsDesc[i] = (double)this.m_heights[i];
			}
			itemsDesc.EndChangingItemsSizes();
		}

		// Token: 0x060049E5 RID: 18917 RVA: 0x00071AE6 File Offset: 0x0006FCE6
		protected override void Start()
		{
			base.Start();
			Options.GameOptions.ChatFontSize.Changed += this.OnChatFontSizeChanged;
		}

		// Token: 0x060049E6 RID: 18918 RVA: 0x001B133C File Offset: 0x001AF53C
		protected override void OnDestroy()
		{
			base.OnDestroy();
			Options.GameOptions.ChatFontSize.Changed -= this.OnChatFontSizeChanged;
			if (this.m_parent != null)
			{
				this.m_parent.ResizeBegin -= this.OnResizeBegin;
				this.m_parent.ResizeDrag -= this.OnResizeDrag;
				this.m_parent.ResizeFinish -= this.OnResizeFinish;
			}
		}

		// Token: 0x060049E7 RID: 18919 RVA: 0x001B13B8 File Offset: 0x001AF5B8
		private void RecaulculateHeights()
		{
			if (this.m_items != null)
			{
				for (int i = 0; i < this.m_items.Count; i++)
				{
					ChatMessage chatMessage = this.m_items[i];
					if (this.ShouldRecalculateHeight(chatMessage))
					{
						chatMessage.PreferredHeight = this.GetPreferredHeightForContent(chatMessage.GetCachedFormattedMessage(this.m_parent.ShowTimestamps));
					}
					this.m_heights[i] = chatMessage.PreferredHeight;
				}
			}
		}

		// Token: 0x060049E8 RID: 18920 RVA: 0x00071B04 File Offset: 0x0006FD04
		private bool ShouldRecalculateHeight(ChatMessage msg)
		{
			return this.ForceRecalculateAllPreferredHeights || msg.ShouldRecalculatePreferredHeight(this.m_lastRecalculatePreferredHeights);
		}

		// Token: 0x060049E9 RID: 18921 RVA: 0x001B1428 File Offset: 0x001AF628
		private float GetPreferredHeightForContent(string content)
		{
			TextMeshProUGUI samplerText = this.m_samplerText;
			if ((int)samplerText.fontSize != Options.GameOptions.ChatFontSize.Value)
			{
				samplerText.fontSize = (float)Options.GameOptions.ChatFontSize.Value;
			}
			samplerText.ZStringSetText(content);
			samplerText.ForceMeshUpdate(true, false);
			Vector4 margin = samplerText.margin;
			RectOffset contentPadding = base.Parameters.ContentPadding;
			float width = samplerText.rectTransform.rect.width - margin.x - margin.z - (float)contentPadding.left - (float)contentPadding.right;
			return samplerText.GetPreferredValues(width, 24f).y;
		}

		// Token: 0x060049EA RID: 18922 RVA: 0x00071B1C File Offset: 0x0006FD1C
		protected override void OnScrollPositionChanged(double position)
		{
			if (!this.m_ignoreScrollEvent)
			{
				this.LockedToBottom = this.ShouldLockToBottom;
			}
		}

		// Token: 0x060049EB RID: 18923 RVA: 0x00071B32 File Offset: 0x0006FD32
		private void OnResizeBegin()
		{
			this.m_cachedVerticalPosition = new double?(base.GetNormalizedPosition());
		}

		// Token: 0x060049EC RID: 18924 RVA: 0x001B14C8 File Offset: 0x001AF6C8
		private void OnResizeDrag()
		{
			this.ForceRecalculateAllPreferredHeights = true;
			this.RecaulculateHeights();
			if (this.LockedToBottom)
			{
				this.ScrollToBottom(true);
			}
			else if (this.m_cachedVerticalPosition != null)
			{
				base.SetNormalizedPosition(this.m_cachedVerticalPosition.Value);
			}
			this.ForceRecalculateAllPreferredHeights = false;
		}

		// Token: 0x060049ED RID: 18925 RVA: 0x00071B45 File Offset: 0x0006FD45
		private void OnResizeFinish()
		{
			this.m_cachedVerticalPosition = null;
		}

		// Token: 0x060049EE RID: 18926 RVA: 0x001B1518 File Offset: 0x001AF718
		public void ShowTimestampsChanged(bool show)
		{
			for (int i = 0; i < base.VisibleItemsCount; i++)
			{
				ChatItemViewsHolder itemViewsHolder = base.GetItemViewsHolder(i);
				if (itemViewsHolder != null)
				{
					itemViewsHolder.RefreshLineContent(show);
				}
			}
			this.ForceRecalculateAllPreferredHeights = true;
			this.RecaulculateHeights();
			this.ForceRecalculateAllPreferredHeights = false;
		}

		// Token: 0x060049EF RID: 18927 RVA: 0x001B1560 File Offset: 0x001AF760
		private void OnChatFontSizeChanged()
		{
			this.ForceRecalculateAllPreferredHeights = true;
			double normalizedPosition = base.GetNormalizedPosition();
			for (int i = 0; i < base.VisibleItemsCount; i++)
			{
				ChatItemViewsHolder itemViewsHolder = base.GetItemViewsHolder(i);
				if (itemViewsHolder != null)
				{
					itemViewsHolder.UpdateFontSize();
				}
			}
			this.RecaulculateHeights();
			if (this.LockedToBottom)
			{
				this.ScrollToBottom(true);
			}
			else
			{
				base.SetNormalizedPosition(normalizedPosition);
			}
			this.ForceRecalculateAllPreferredHeights = false;
			this.Refresh(false, false);
		}

		// Token: 0x040044E4 RID: 17636
		[SerializeField]
		private TextMeshProUGUI m_samplerText;

		// Token: 0x040044E5 RID: 17637
		private List<ChatMessage> m_items;

		// Token: 0x040044E6 RID: 17638
		private List<float> m_heights;

		// Token: 0x040044E7 RID: 17639
		private ChatWindowUI m_parent;

		// Token: 0x040044E8 RID: 17640
		private double? m_cachedVerticalPosition;

		// Token: 0x040044E9 RID: 17641
		private float m_lastRecalculatePreferredHeights = -1f;

		// Token: 0x040044EA RID: 17642
		private bool m_recalculatePreferredHeights;

		// Token: 0x040044EB RID: 17643
		private bool m_lockedToBottom = true;

		// Token: 0x040044EC RID: 17644
		private bool m_ignoreScrollEvent;
	}
}
