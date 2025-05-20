using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;

namespace SoL.Game.HuntingLog
{
	// Token: 0x02000BD6 RID: 3030
	public class HuntingLogListUI : OSA<BaseParamsWithPrefab, HuntingLogItemViewsHolder>
	{
		// Token: 0x1700161D RID: 5661
		// (get) Token: 0x06005D9B RID: 23963 RVA: 0x0007EF1B File Offset: 0x0007D11B
		// (set) Token: 0x06005D9C RID: 23964 RVA: 0x0007EF23 File Offset: 0x0007D123
		public int SelectedIndex { get; private set; } = -1;

		// Token: 0x1700161E RID: 5662
		// (get) Token: 0x06005D9D RID: 23965 RVA: 0x0007EF2C File Offset: 0x0007D12C
		// (set) Token: 0x06005D9E RID: 23966 RVA: 0x0007EF34 File Offset: 0x0007D134
		public HuntingLogUI Controller { get; internal set; }

		// Token: 0x1400011F RID: 287
		// (add) Token: 0x06005D9F RID: 23967 RVA: 0x001F4338 File Offset: 0x001F2538
		// (remove) Token: 0x06005DA0 RID: 23968 RVA: 0x001F4370 File Offset: 0x001F2570
		public event Action FullyInitialized;

		// Token: 0x14000120 RID: 288
		// (add) Token: 0x06005DA1 RID: 23969 RVA: 0x001F43A8 File Offset: 0x001F25A8
		// (remove) Token: 0x06005DA2 RID: 23970 RVA: 0x001F43E0 File Offset: 0x001F25E0
		public event Action<HuntingLogEntry> SelectionChanged;

		// Token: 0x1700161F RID: 5663
		// (get) Token: 0x06005DA3 RID: 23971 RVA: 0x0007EF3D File Offset: 0x0007D13D
		public bool IsFullyInitialized
		{
			get
			{
				if (!base.IsInitialized)
				{
					this.Initialized += this.OnListInitialized;
					return false;
				}
				return true;
			}
		}

		// Token: 0x06005DA4 RID: 23972 RVA: 0x0007EF5C File Offset: 0x0007D15C
		private void OnListInitialized()
		{
			this.Initialized -= this.OnInitialized;
			Action fullyInitialized = this.FullyInitialized;
			if (fullyInitialized == null)
			{
				return;
			}
			fullyInitialized();
		}

		// Token: 0x06005DA5 RID: 23973 RVA: 0x0007EF81 File Offset: 0x0007D181
		protected override HuntingLogItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			HuntingLogItemViewsHolder huntingLogItemViewsHolder = new HuntingLogItemViewsHolder();
			huntingLogItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return huntingLogItemViewsHolder;
		}

		// Token: 0x06005DA6 RID: 23974 RVA: 0x0007EFA7 File Offset: 0x0007D1A7
		protected override void UpdateViewsHolder(HuntingLogItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateItem(this, this.m_items[newOrRecycled.ItemIndex]);
		}

		// Token: 0x06005DA7 RID: 23975 RVA: 0x001F4418 File Offset: 0x001F2618
		public void Select(int index)
		{
			int selectedIndex = this.SelectedIndex;
			this.SelectedIndex = index;
			HuntingLogItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(index);
			if (selectedIndex != index)
			{
				if (itemViewsHolderIfVisible != null)
				{
					Action<HuntingLogEntry> selectionChanged = this.SelectionChanged;
					if (selectionChanged != null)
					{
						selectionChanged(itemViewsHolderIfVisible.Entry);
					}
				}
				if (selectedIndex != -1)
				{
					HuntingLogItemViewsHolder itemViewsHolderIfVisible2 = base.GetItemViewsHolderIfVisible(selectedIndex);
					if (itemViewsHolderIfVisible2 == null)
					{
						return;
					}
					itemViewsHolderIfVisible2.ListItem.RefreshVisuals();
				}
			}
		}

		// Token: 0x06005DA8 RID: 23976 RVA: 0x001F4474 File Offset: 0x001F2674
		public void DeselectAll(bool suppressEvents = false)
		{
			int selectedIndex = this.SelectedIndex;
			this.SelectedIndex = -1;
			if (selectedIndex != -1 && !suppressEvents)
			{
				Action<HuntingLogEntry> selectionChanged = this.SelectionChanged;
				if (selectionChanged != null)
				{
					selectionChanged(null);
				}
			}
			for (int i = 0; i < this.GetItemsCount(); i++)
			{
				HuntingLogItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.ListItem.RefreshVisuals();
				}
			}
		}

		// Token: 0x06005DA9 RID: 23977 RVA: 0x001F44D0 File Offset: 0x001F26D0
		public void RefreshItems()
		{
			HuntingLogEntry huntingLogEntry = (this.SelectedIndex != -1 && this.SelectedIndex < this.m_items.Count) ? this.m_items[this.SelectedIndex] : null;
			this.m_items.Clear();
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Record != null && LocalPlayer.GameEntity.CollectionController.Record.HuntingLog != null)
			{
				foreach (KeyValuePair<UniqueId, HuntingLogEntry> keyValuePair in LocalPlayer.GameEntity.CollectionController.Record.HuntingLog)
				{
					HuntingLogProfile profile = keyValuePair.Value.GetProfile();
					if (profile && profile.ShowInLog)
					{
						this.m_items.Add(keyValuePair.Value);
					}
				}
				this.m_items.Sort(new Comparison<HuntingLogEntry>(HuntingLogListUI.LogEntryComparison));
			}
			this.ResetItems(this.m_items.Count, false, false);
			if (huntingLogEntry == null)
			{
				this.DeselectAll(false);
				return;
			}
			for (int i = 0; i < this.m_items.Count; i++)
			{
				if (this.m_items[i] == huntingLogEntry)
				{
					HuntingLogItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
					if (itemViewsHolderIfVisible != null && itemViewsHolderIfVisible.ListItem)
					{
						this.Select(itemViewsHolderIfVisible.ListItem.Index);
						return;
					}
				}
			}
		}

		// Token: 0x06005DAA RID: 23978 RVA: 0x001F4670 File Offset: 0x001F2870
		private static int LogEntryComparison(HuntingLogEntry x, HuntingLogEntry y)
		{
			HuntingLogProfile profile = x.GetProfile();
			HuntingLogProfile profile2 = y.GetProfile();
			return string.Compare(profile.TitlePrefix, profile2.TitlePrefix, StringComparison.InvariantCultureIgnoreCase);
		}

		// Token: 0x040050EF RID: 20719
		private readonly List<HuntingLogEntry> m_items = new List<HuntingLogEntry>(10);
	}
}
