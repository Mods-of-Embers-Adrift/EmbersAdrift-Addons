using System;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.Core;
using Cysharp.Text;
using NetStack.Serialization;
using SoL.Game.Audio;
using SoL.Game.Objects.Archetypes;
using SoL.Game.UI;
using SoL.Managers;
using SoL.Networking;
using SoL.Networking.Database;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SoL.Game.AuctionHouse
{
	// Token: 0x02000D34 RID: 3380
	public class AuctionHouseForSaleList : OSA<AuctionHouseItemParam, AuctionHouseItemViewsHolder>
	{
		// Token: 0x14000124 RID: 292
		// (add) Token: 0x06006588 RID: 25992 RVA: 0x0020D0A4 File Offset: 0x0020B2A4
		// (remove) Token: 0x06006589 RID: 25993 RVA: 0x0020D0DC File Offset: 0x0020B2DC
		public event Action SelectedAuctionChanged;

		// Token: 0x1700185E RID: 6238
		// (get) Token: 0x0600658A RID: 25994 RVA: 0x00084660 File Offset: 0x00082860
		public UIWindow Window
		{
			get
			{
				return this.m_window;
			}
		}

		// Token: 0x1700185F RID: 6239
		// (get) Token: 0x0600658B RID: 25995 RVA: 0x00084668 File Offset: 0x00082868
		// (set) Token: 0x0600658C RID: 25996 RVA: 0x00084670 File Offset: 0x00082870
		internal AuctionRecord SelectedAuction
		{
			get
			{
				return this.m_selectedAuction;
			}
			private set
			{
				if (this.m_selectedAuction != null && this.m_selectedAuction.Equals(value))
				{
					return;
				}
				this.m_selectedAuction = value;
				Action selectedAuctionChanged = this.SelectedAuctionChanged;
				if (selectedAuctionChanged == null)
				{
					return;
				}
				selectedAuctionChanged();
			}
		}

		// Token: 0x17001860 RID: 6240
		// (get) Token: 0x0600658D RID: 25997 RVA: 0x000846A0 File Offset: 0x000828A0
		// (set) Token: 0x0600658E RID: 25998 RVA: 0x000846A8 File Offset: 0x000828A8
		internal bool HoldingShift { get; private set; }

		// Token: 0x0600658F RID: 25999 RVA: 0x0020D114 File Offset: 0x0020B314
		protected override void Awake()
		{
			base.Awake();
			this.PopulateSorters();
			this.m_allAuctions = new List<AuctionRecord>(256);
			this.m_filteredAuctions = new List<AuctionRecord>(256);
			this.RefreshShownStatsLabel();
			this.RefreshMyStatLabel();
			if (this.m_filter)
			{
				this.m_filter.InputChanged += this.UpdateFilter;
			}
			if (this.m_auctionsToggle)
			{
				this.m_auctionsToggle.isOn = true;
				this.m_auctionsToggle.onValueChanged.AddListener(new UnityAction<bool>(this.UpdateAuctionsToggle));
			}
			if (this.m_buyItNowToggle)
			{
				this.m_auctionsToggle.isOn = true;
				this.m_buyItNowToggle.onValueChanged.AddListener(new UnityAction<bool>(this.UpdateBuyItNowToggle));
			}
			if (this.m_myAuctionsToggle)
			{
				this.m_myAuctionsToggle.isOn = false;
				this.m_myAuctionsToggle.onValueChanged.AddListener(new UnityAction<bool>(this.UpdateMyAuctionsToggle));
			}
			if (this.m_isWinningToggle)
			{
				this.m_isWinningToggle.isOn = false;
				this.m_isWinningToggle.onValueChanged.AddListener(new UnityAction<bool>(this.UpdateIsWinningToggle));
			}
			if (this.m_canUseToggle)
			{
				this.m_canUseToggle.isOn = false;
				this.m_canUseToggle.onValueChanged.AddListener(new UnityAction<bool>(this.UpdateCanUseToggle));
			}
			if (this.m_levelSlider)
			{
				if (this.m_levelSlider.Slider)
				{
					if (this.m_levelSlider.Label)
					{
						this.m_levelSlider.Label.ZStringSetText("1");
					}
					this.m_levelSlider.Slider.minValue = 1f;
					this.m_levelSlider.Slider.maxValue = 50f;
					this.m_levelSlider.Slider.wholeNumbers = true;
					this.m_levelSlider.Slider.onValueChanged.AddListener(new UnityAction<float>(this.LevelSliderChanged));
				}
				if (this.m_levelSlider.ResetButton)
				{
					this.m_levelSlider.ResetButton.onClick.AddListener(new UnityAction(this.LevelSliderResetClicked));
				}
			}
			if (this.m_sortTypeDropdown)
			{
				this.m_sortTypeDropdown.ClearOptions();
				this.m_sortTypeDropdown.AddOptions(new List<string>
				{
					"Item Name",
					"Expires Within",
					"Seller Name",
					"Current Bid",
					"Buy It Now Price"
				});
				this.m_sortTypeDropdown.value = 1;
				this.m_sortTypeDropdown.onValueChanged.AddListener(new UnityAction<int>(this.SortDropdownChanged));
			}
			if (this.m_sortDirectionButton)
			{
				this.m_sortDescending = false;
				this.m_sortDirectionButton.onClick.AddListener(new UnityAction(this.SortDirectionChanged));
				this.RefreshSortButtonIcon();
			}
		}

		// Token: 0x06006590 RID: 26000 RVA: 0x0020D420 File Offset: 0x0020B620
		protected override void Update()
		{
			base.Update();
			if (this.m_window && this.m_window.Visible)
			{
				if (UnityEngine.Time.time >= this.m_timeOfNextLabelRefresh)
				{
					this.m_timeOfNextLabelRefresh = UnityEngine.Time.time + 30f;
					for (int i = 0; i < this.m_filteredAuctions.Count; i++)
					{
						AuctionHouseItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
						if (itemViewsHolderIfVisible != null)
						{
							itemViewsHolderIfVisible.RefreshTimeLeftLabel();
						}
					}
				}
				bool holdingShift = this.HoldingShift;
				this.HoldingShift = (ClientGameManager.InputManager != null && ClientGameManager.InputManager.HoldingShift);
				if (this.HoldingShift != holdingShift)
				{
					this.RefreshHoldingShiftForStackables();
				}
			}
		}

		// Token: 0x06006591 RID: 26001 RVA: 0x0020D4C8 File Offset: 0x0020B6C8
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (this.m_filter)
			{
				this.m_filter.InputChanged -= this.UpdateFilter;
			}
			if (this.m_auctionsToggle)
			{
				this.m_auctionsToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.UpdateAuctionsToggle));
			}
			if (this.m_buyItNowToggle)
			{
				this.m_buyItNowToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.UpdateBuyItNowToggle));
			}
			if (this.m_myAuctionsToggle)
			{
				this.m_myAuctionsToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.UpdateMyAuctionsToggle));
			}
			if (this.m_isWinningToggle)
			{
				this.m_isWinningToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.UpdateIsWinningToggle));
			}
			if (this.m_canUseToggle)
			{
				this.m_canUseToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.UpdateCanUseToggle));
			}
			if (this.m_levelSlider)
			{
				if (this.m_levelSlider.Slider)
				{
					this.m_levelSlider.Slider.onValueChanged.RemoveListener(new UnityAction<float>(this.LevelSliderChanged));
				}
				if (this.m_levelSlider.ResetButton)
				{
					this.m_levelSlider.ResetButton.onClick.AddListener(new UnityAction(this.LevelSliderResetClicked));
				}
			}
			if (this.m_sortTypeDropdown)
			{
				this.m_sortTypeDropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.SortDropdownChanged));
			}
			if (this.m_sortDirectionButton)
			{
				this.m_sortDirectionButton.onClick.RemoveListener(new UnityAction(this.SortDirectionChanged));
			}
		}

		// Token: 0x06006592 RID: 26002 RVA: 0x0020D694 File Offset: 0x0020B894
		private void PopulateSorters()
		{
			Dictionary<int, Comparison<AuctionRecord>> dictionary = new Dictionary<int, Comparison<AuctionRecord>>();
			dictionary.Add(0, delegate(AuctionRecord a, AuctionRecord b)
			{
				int num = string.Compare(a.CachedItemName, b.CachedItemName, StringComparison.InvariantCultureIgnoreCase);
				if (num != 0)
				{
					return num;
				}
				return a.Expiration.CompareTo(b.Expiration);
			});
			dictionary.Add(1, (AuctionRecord a, AuctionRecord b) => a.Expiration.CompareTo(b.Expiration));
			dictionary.Add(2, delegate(AuctionRecord a, AuctionRecord b)
			{
				int num = string.Compare(a.SellerName, b.SellerName, StringComparison.InvariantCultureIgnoreCase);
				if (num != 0)
				{
					return num;
				}
				return a.Expiration.CompareTo(b.Expiration);
			});
			dictionary.Add(3, delegate(AuctionRecord a, AuctionRecord b)
			{
				if (a.CurrentBid != null && b.CurrentBid != null)
				{
					int num = a.CurrentBid.Value.CompareTo(b.CurrentBid.Value);
					if (num != 0)
					{
						return num;
					}
					return a.Expiration.CompareTo(b.Expiration);
				}
				else
				{
					if (a.CurrentBid != null)
					{
						return -1;
					}
					if (b.CurrentBid != null)
					{
						return 1;
					}
					return a.Expiration.CompareTo(b.Expiration);
				}
			});
			dictionary.Add(4, delegate(AuctionRecord a, AuctionRecord b)
			{
				if (a.BuyNowPrice != null && b.BuyNowPrice != null)
				{
					int num = a.BuyNowPrice.Value.CompareTo(b.BuyNowPrice.Value);
					if (num != 0)
					{
						return num;
					}
					return a.Expiration.CompareTo(b.Expiration);
				}
				else
				{
					if (a.BuyNowPrice != null)
					{
						return -1;
					}
					if (b.BuyNowPrice != null)
					{
						return 1;
					}
					return a.Expiration.CompareTo(b.Expiration);
				}
			});
			this.m_sorters = dictionary;
		}

		// Token: 0x06006593 RID: 26003 RVA: 0x000846B1 File Offset: 0x000828B1
		internal void WindowClosed()
		{
			this.m_filter.SetText(string.Empty);
		}

		// Token: 0x06006594 RID: 26004 RVA: 0x000846C3 File Offset: 0x000828C3
		internal void UpdateList(List<AuctionRecord> records)
		{
			this.SelectedAuction = null;
			this.m_allAuctions.Clear();
			if (records != null)
			{
				this.m_allAuctions.AddRange(records);
			}
			this.RefreshFilteredList();
			this.RefreshMyStatLabel();
		}

		// Token: 0x06006595 RID: 26005 RVA: 0x000846F2 File Offset: 0x000828F2
		internal void SelectItem(AuctionRecord auction)
		{
			if (this.SelectedAuction != null && auction != null && this.SelectedAuction.Id == auction.Id)
			{
				this.SelectedAuction = null;
			}
			else
			{
				this.SelectedAuction = auction;
			}
			this.RefreshSelected();
		}

		// Token: 0x06006596 RID: 26006 RVA: 0x0020D76C File Offset: 0x0020B96C
		private void RefreshSelected()
		{
			for (int i = 0; i < this.m_filteredAuctions.Count; i++)
			{
				AuctionHouseItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.RefreshSelected();
				}
			}
		}

		// Token: 0x06006597 RID: 26007 RVA: 0x0020D7A4 File Offset: 0x0020B9A4
		public void RefreshHoldingShiftForStackables()
		{
			if (this.m_filteredAuctions == null || this.m_filteredAuctions.Count <= 0 || !this.m_window.Visible)
			{
				return;
			}
			for (int i = 0; i < this.m_filteredAuctions.Count; i++)
			{
				AuctionHouseItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(i);
				if (itemViewsHolderIfVisible != null)
				{
					itemViewsHolderIfVisible.RefreshHoldingShift();
				}
			}
		}

		// Token: 0x06006598 RID: 26008 RVA: 0x0008472D File Offset: 0x0008292D
		private void SortDirectionChanged()
		{
			this.m_sortDescending = !this.m_sortDescending;
			this.RefreshSortButtonIcon();
			this.RefreshFilteredList();
		}

		// Token: 0x06006599 RID: 26009 RVA: 0x0020D800 File Offset: 0x0020BA00
		private void RefreshSortButtonIcon()
		{
			if (this.m_sortDirectionButton && this.m_sortDirectionButton.image)
			{
				this.m_sortDirectionButton.image.overrideSprite = (this.m_sortDescending ? this.m_sortDescendingIcon : this.m_sortAscendingIcon);
			}
		}

		// Token: 0x0600659A RID: 26010 RVA: 0x0008474A File Offset: 0x0008294A
		private void SortDropdownChanged(int arg0)
		{
			this.RefreshFilteredList();
		}

		// Token: 0x0600659B RID: 26011 RVA: 0x00084752 File Offset: 0x00082952
		private void LevelSliderChanged(float arg0)
		{
			if (this.m_levelSlider != null && this.m_levelSlider.Label)
			{
				this.m_levelSlider.Label.ZStringSetText(arg0);
			}
			this.UpdateLevelFilter(Mathf.FloorToInt(arg0));
		}

		// Token: 0x0600659C RID: 26012 RVA: 0x00084791 File Offset: 0x00082991
		private void LevelSliderResetClicked()
		{
			if (this.m_levelSlider && this.m_levelSlider.Slider)
			{
				this.m_levelSlider.Slider.value = 1f;
			}
		}

		// Token: 0x0600659D RID: 26013 RVA: 0x000847C7 File Offset: 0x000829C7
		private void UpdateFilter(string filter)
		{
			this.m_filterText = filter;
			this.RefreshFilteredList();
		}

		// Token: 0x0600659E RID: 26014 RVA: 0x000847D6 File Offset: 0x000829D6
		private void UpdateAuctionsToggle(bool include)
		{
			this.m_includeAuctions = include;
			this.RefreshFilteredList();
		}

		// Token: 0x0600659F RID: 26015 RVA: 0x000847E5 File Offset: 0x000829E5
		private void UpdateBuyItNowToggle(bool include)
		{
			this.m_includeBuyItNow = include;
			this.RefreshFilteredList();
		}

		// Token: 0x060065A0 RID: 26016 RVA: 0x000847F4 File Offset: 0x000829F4
		private void UpdateMyAuctionsToggle(bool include)
		{
			this.m_myAuctions = include;
			this.RefreshFilteredList();
		}

		// Token: 0x060065A1 RID: 26017 RVA: 0x00084803 File Offset: 0x00082A03
		private void UpdateIsWinningToggle(bool include)
		{
			this.m_isWinning = include;
			this.RefreshFilteredList();
		}

		// Token: 0x060065A2 RID: 26018 RVA: 0x00084812 File Offset: 0x00082A12
		private void UpdateCanUseToggle(bool include)
		{
			this.m_canUse = include;
			this.RefreshFilteredList();
		}

		// Token: 0x060065A3 RID: 26019 RVA: 0x00084821 File Offset: 0x00082A21
		private void UpdateLevelFilter(int value)
		{
			this.m_levelFilter = value;
			this.RefreshFilteredList();
		}

		// Token: 0x060065A4 RID: 26020 RVA: 0x0020D854 File Offset: 0x0020BA54
		private void RefreshFilteredList()
		{
			if (!base.gameObject.activeInHierarchy || this.m_allAuctions == null || this.m_filteredAuctions == null)
			{
				return;
			}
			string text = this.m_filterText;
			bool flag = !string.IsNullOrEmpty(text);
			if (flag)
			{
				text = text.Trim();
			}
			this.m_filteredAuctions.Clear();
			for (int i = 0; i < this.m_allAuctions.Count; i++)
			{
				if (this.m_allAuctions[i].Instance != null && this.m_allAuctions[i].Instance.Archetype && (this.m_includeAuctions || this.m_includeBuyItNow))
				{
					bool flag2 = this.m_allAuctions[i].CurrentBid != null && this.m_allAuctions[i].BuyNowPrice == null;
					bool flag3 = this.m_allAuctions[i].BuyNowPrice != null && this.m_allAuctions[i].CurrentBid == null;
					if ((!flag2 || this.m_includeAuctions) && (!flag3 || this.m_includeBuyItNow) && (!flag || AuctionHouseForSaleList.MatchesTextFilter(this.m_allAuctions[i], text)))
					{
						ItemArchetype itemArchetype;
						if ((this.m_levelFilter > 1 || this.m_canUse) && this.m_allAuctions[i].Instance.Archetype.TryGetAsType(out itemArchetype))
						{
							ConsumableItem consumableItem = itemArchetype as ConsumableItem;
							if (consumableItem != null)
							{
								if (consumableItem.LevelRequirement != null && consumableItem.LevelRequirement.Level < this.m_levelFilter)
								{
									goto IL_347;
								}
								AbilityRecipeItem abilityRecipeItem = consumableItem as AbilityRecipeItem;
								string text2;
								if (abilityRecipeItem != null && !abilityRecipeItem.CanRollNeed(LocalPlayer.GameEntity, out text2))
								{
									goto IL_347;
								}
								RecipeItem recipeItem = consumableItem as RecipeItem;
								if (recipeItem != null && !recipeItem.CanUseAuctionHouse(LocalPlayer.GameEntity))
								{
									goto IL_347;
								}
								QuestStarterItem questStarterItem = consumableItem as QuestStarterItem;
								if (questStarterItem != null && (!questStarterItem.Quest || questStarterItem.Quest.IsOnQuest(null) || !questStarterItem.Quest.CanStartQuest(null)))
								{
									goto IL_347;
								}
								EmoteItem emoteItem = consumableItem as EmoteItem;
								if (emoteItem != null && !emoteItem.CanUseAuctionHouse(LocalPlayer.GameEntity))
								{
									goto IL_347;
								}
								LearnableTitleItem learnableTitleItem = consumableItem as LearnableTitleItem;
								if (learnableTitleItem != null && !learnableTitleItem.CanUseAuctionHouse(LocalPlayer.GameEntity))
								{
									goto IL_347;
								}
							}
							else
							{
								ReagentItem reagentItem = itemArchetype as ReagentItem;
								if (reagentItem != null)
								{
									if (reagentItem.LevelRequirementLevel < this.m_levelFilter)
									{
										goto IL_347;
									}
									if (this.m_canUse && !reagentItem.HasRequiredRoleAndSpec(LocalPlayer.GameEntity))
									{
										goto IL_347;
									}
								}
								else
								{
									EquipableItem equipableItem = itemArchetype as EquipableItem;
									if (equipableItem != null)
									{
										if (equipableItem.LevelRequirementLevel < this.m_levelFilter)
										{
											goto IL_347;
										}
										if (this.m_canUse && !equipableItem.CanEquip(LocalPlayer.GameEntity))
										{
											goto IL_347;
										}
									}
									else
									{
										int? minimumMaterialLevel = itemArchetype.MinimumMaterialLevel;
										if (minimumMaterialLevel != null && minimumMaterialLevel.Value < this.m_levelFilter)
										{
											goto IL_347;
										}
									}
								}
							}
						}
						if ((!this.m_myAuctions || SessionData.IsMyCharacter(this.m_allAuctions[i].SellerCharacterId)) && (!this.m_isWinning || SessionData.IsMyCharacter(this.m_allAuctions[i].BuyerCharacterId)))
						{
							this.m_filteredAuctions.Add(this.m_allAuctions[i]);
						}
					}
				}
				IL_347:;
			}
			Comparison<AuctionRecord> comparison;
			if (this.m_sortTypeDropdown && this.m_sorters != null && this.m_sorters.TryGetValue(this.m_sortTypeDropdown.value, out comparison))
			{
				this.m_filteredAuctions.Sort(comparison);
				if (this.m_sortDescending)
				{
					this.m_filteredAuctions.Reverse();
				}
			}
			this.ResetItems(this.m_filteredAuctions.Count, false, false);
			this.RefreshShownStatsLabel();
			this.m_timeOfNextLabelRefresh = UnityEngine.Time.time + 30f;
		}

		// Token: 0x060065A5 RID: 26021 RVA: 0x0020DC38 File Offset: 0x0020BE38
		private static bool MatchesTextFilter(AuctionRecord auction, string textFilter)
		{
			if (auction != null && auction.Instance != null && auction.Instance.Archetype)
			{
				if (auction.SellerName.Equals(textFilter, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
				if (auction.Instance.Archetype.GetModifiedDisplayName(auction.Instance).Contains(textFilter, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
				if (auction.Instance.Archetype.MatchesTextFilter(textFilter))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060065A6 RID: 26022 RVA: 0x0020DCAC File Offset: 0x0020BEAC
		internal void ProcessAuctionHouseUpdate(BitBuffer buffer)
		{
			if (buffer == null)
			{
				return;
			}
			AudioClipCollection audioClipCollection = null;
			switch (buffer.ReadEnum<AuctionUpdateType>())
			{
			case AuctionUpdateType.New:
			{
				AuctionRecord auctionRecord = new AuctionRecord();
				auctionRecord.ReadData(buffer);
				this.m_allAuctions.Add(auctionRecord);
				this.RefreshFilteredList();
				if (this.m_filteredAuctions.Contains(auctionRecord))
				{
					audioClipCollection = this.m_newListingClips;
				}
				break;
			}
			case AuctionUpdateType.Update:
			{
				UpdateAuction updateAuction = default(UpdateAuction);
				updateAuction.ReadData(buffer);
				int num;
				AuctionRecord auctionRecord2;
				if (this.TryGetAuctionRecordById(updateAuction.Id, false, out num, out auctionRecord2))
				{
					bool flag = SessionData.IsMyCharacter(auctionRecord2.BuyerCharacterId);
					auctionRecord2.Updated = GameTimeReplicator.GetServerCorrectedDateTimeUtc();
					auctionRecord2.BuyerCharacterId = updateAuction.BuyerCharacterId;
					auctionRecord2.CurrentBid = updateAuction.CurrentBid;
					auctionRecord2.BidCount = updateAuction.BidCount;
					bool flag2 = SessionData.IsMyCharacter(auctionRecord2.BuyerCharacterId);
					int withItemIndex;
					if (this.TryGetAuctionRecordById(updateAuction.Id, true, out withItemIndex, out auctionRecord2))
					{
						AuctionHouseItemViewsHolder itemViewsHolderIfVisible = base.GetItemViewsHolderIfVisible(withItemIndex);
						if (itemViewsHolderIfVisible != null)
						{
							itemViewsHolderIfVisible.RefreshAvailability();
							audioClipCollection = this.m_updatedListingClips;
						}
						if (this.SelectedAuction != null && this.SelectedAuction.Id == updateAuction.Id)
						{
							Action selectedAuctionChanged = this.SelectedAuctionChanged;
							if (selectedAuctionChanged != null)
							{
								selectedAuctionChanged();
							}
						}
						if (flag != flag2)
						{
							audioClipCollection = (flag2 ? this.m_updatedIsWinningClips : this.m_updatedWasWinningClips);
						}
					}
				}
				break;
			}
			case AuctionUpdateType.Expirations:
			{
				bool flag3 = false;
				bool flag4 = false;
				int num2 = buffer.ReadInt();
				for (int i = 0; i < num2; i++)
				{
					string text = buffer.ReadString();
					int num3;
					AuctionRecord item;
					if (this.TryGetAuctionRecordById(text, false, out num3, out item) && num3 >= 0)
					{
						if (this.SelectedAuction != null && this.SelectedAuction.Id == text)
						{
							this.SelectedAuction = null;
						}
						this.m_allAuctions.RemoveAt(num3);
						flag3 = true;
						flag4 = (flag4 || this.m_filteredAuctions.Contains(item));
					}
				}
				if (flag3)
				{
					this.RefreshFilteredList();
					if (flag4)
					{
						audioClipCollection = this.m_expiredListingClips;
					}
				}
				break;
			}
			}
			this.RefreshMyStatLabel();
			if (audioClipCollection && ClientGameManager.UIManager)
			{
				ClientGameManager.UIManager.PlayRandomClip(audioClipCollection, null);
			}
		}

		// Token: 0x060065A7 RID: 26023 RVA: 0x0020DEDC File Offset: 0x0020C0DC
		private bool TryGetAuctionRecordById(string auctionId, bool getFilteredIndex, out int index, out AuctionRecord auction)
		{
			index = -1;
			auction = null;
			List<AuctionRecord> list = getFilteredIndex ? this.m_filteredAuctions : this.m_allAuctions;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] != null && list[i].Id == auctionId)
				{
					index = i;
					auction = list[i];
					return true;
				}
			}
			return false;
		}

		// Token: 0x060065A8 RID: 26024 RVA: 0x0020DF40 File Offset: 0x0020C140
		private void RefreshMyStatLabel()
		{
			TextMeshProUGUI bottomRightStatsLabel = this.m_bottomRightStatsLabel;
			if (!bottomRightStatsLabel)
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < this.m_allAuctions.Count; i++)
			{
				if (this.m_allAuctions[i] != null)
				{
					if (SessionData.IsMyCharacter(this.m_allAuctions[i].SellerCharacterId))
					{
						num++;
					}
					if (this.m_allAuctions[i].CurrentBid != null && !string.IsNullOrEmpty(this.m_allAuctions[i].BuyerCharacterId) && SessionData.IsMyCharacter(this.m_allAuctions[i].BuyerCharacterId))
					{
						num2++;
					}
				}
			}
			if (num2 > 0)
			{
				bottomRightStatsLabel.SetTextFormat("Currently Winning: {0}\n{1}: {2}/{3}", num2, "Your Active Auctions", num, ServerAuctionHouseManager.GetMaxListingsPerUser(LocalPlayer.GameEntity));
				return;
			}
			bottomRightStatsLabel.SetTextFormat("\n{0}: {1}/{2}", "Your Active Auctions", num, ServerAuctionHouseManager.GetMaxListingsPerUser(LocalPlayer.GameEntity));
		}

		// Token: 0x060065A9 RID: 26025 RVA: 0x0020E02C File Offset: 0x0020C22C
		private void RefreshShownStatsLabel()
		{
			TextMeshProUGUI bottomLeftStatsLabel = this.m_bottomLeftStatsLabel;
			if (!bottomLeftStatsLabel)
			{
				return;
			}
			bottomLeftStatsLabel.SetTextFormat("{0}/{1} Shown", this.m_filteredAuctions.Count, this.m_allAuctions.Count);
		}

		// Token: 0x060065AA RID: 26026 RVA: 0x00084830 File Offset: 0x00082A30
		protected override AuctionHouseItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			AuctionHouseItemViewsHolder auctionHouseItemViewsHolder = new AuctionHouseItemViewsHolder();
			auctionHouseItemViewsHolder.Init(this._Params.ItemPrefab, this._Params.Content, itemIndex, true, true);
			return auctionHouseItemViewsHolder;
		}

		// Token: 0x060065AB RID: 26027 RVA: 0x00084856 File Offset: 0x00082A56
		protected override void UpdateViewsHolder(AuctionHouseItemViewsHolder newOrRecycled)
		{
			newOrRecycled.UpdateSaleItem(this, this.m_filteredAuctions[newOrRecycled.ItemIndex]);
		}

		// Token: 0x04005850 RID: 22608
		private const float kTimeLeftUpdateCadence = 30f;

		// Token: 0x04005852 RID: 22610
		[SerializeField]
		private UIWindow m_window;

		// Token: 0x04005853 RID: 22611
		[SerializeField]
		private TextInputFilter m_filter;

		// Token: 0x04005854 RID: 22612
		[SerializeField]
		private SolToggle m_auctionsToggle;

		// Token: 0x04005855 RID: 22613
		[SerializeField]
		private SolToggle m_buyItNowToggle;

		// Token: 0x04005856 RID: 22614
		[SerializeField]
		private SolToggle m_myAuctionsToggle;

		// Token: 0x04005857 RID: 22615
		[SerializeField]
		private SolToggle m_isWinningToggle;

		// Token: 0x04005858 RID: 22616
		[SerializeField]
		private SolToggle m_canUseToggle;

		// Token: 0x04005859 RID: 22617
		[SerializeField]
		private OptionsSlider m_levelSlider;

		// Token: 0x0400585A RID: 22618
		[SerializeField]
		private TMP_Dropdown m_sortTypeDropdown;

		// Token: 0x0400585B RID: 22619
		[SerializeField]
		private SolButton m_sortDirectionButton;

		// Token: 0x0400585C RID: 22620
		[SerializeField]
		private Sprite m_sortAscendingIcon;

		// Token: 0x0400585D RID: 22621
		[SerializeField]
		private Sprite m_sortDescendingIcon;

		// Token: 0x0400585E RID: 22622
		[SerializeField]
		private TextMeshProUGUI m_bottomLeftStatsLabel;

		// Token: 0x0400585F RID: 22623
		[SerializeField]
		private TextMeshProUGUI m_bottomRightStatsLabel;

		// Token: 0x04005860 RID: 22624
		[SerializeField]
		private AudioClipCollection m_newListingClips;

		// Token: 0x04005861 RID: 22625
		[SerializeField]
		private AudioClipCollection m_expiredListingClips;

		// Token: 0x04005862 RID: 22626
		[SerializeField]
		private AudioClipCollection m_updatedListingClips;

		// Token: 0x04005863 RID: 22627
		[SerializeField]
		private AudioClipCollection m_updatedIsWinningClips;

		// Token: 0x04005864 RID: 22628
		[SerializeField]
		private AudioClipCollection m_updatedWasWinningClips;

		// Token: 0x04005865 RID: 22629
		private List<AuctionRecord> m_allAuctions;

		// Token: 0x04005866 RID: 22630
		private List<AuctionRecord> m_filteredAuctions;

		// Token: 0x04005867 RID: 22631
		private AuctionRecord m_selectedAuction;

		// Token: 0x04005868 RID: 22632
		private string m_filterText;

		// Token: 0x04005869 RID: 22633
		private bool m_includeAuctions = true;

		// Token: 0x0400586A RID: 22634
		private bool m_includeBuyItNow = true;

		// Token: 0x0400586B RID: 22635
		private bool m_myAuctions;

		// Token: 0x0400586C RID: 22636
		private bool m_isWinning;

		// Token: 0x0400586D RID: 22637
		private bool m_canUse;

		// Token: 0x0400586E RID: 22638
		private int m_levelFilter = 1;

		// Token: 0x0400586F RID: 22639
		private bool m_sortDescending;

		// Token: 0x04005870 RID: 22640
		private float m_timeOfNextLabelRefresh;

		// Token: 0x04005871 RID: 22641
		private Dictionary<int, Comparison<AuctionRecord>> m_sorters;

		// Token: 0x02000D35 RID: 3381
		private enum SortType
		{
			// Token: 0x04005874 RID: 22644
			ItemName,
			// Token: 0x04005875 RID: 22645
			Expires,
			// Token: 0x04005876 RID: 22646
			SellerName,
			// Token: 0x04005877 RID: 22647
			CurrentBid,
			// Token: 0x04005878 RID: 22648
			BuyItNowPrice
		}

		// Token: 0x02000D36 RID: 3382
		private struct SortTypeComparer : IEqualityComparer<AuctionHouseForSaleList.SortType>
		{
			// Token: 0x060065AD RID: 26029 RVA: 0x0004FB72 File Offset: 0x0004DD72
			public bool Equals(AuctionHouseForSaleList.SortType x, AuctionHouseForSaleList.SortType y)
			{
				return x == y;
			}

			// Token: 0x060065AE RID: 26030 RVA: 0x00049A92 File Offset: 0x00047C92
			public int GetHashCode(AuctionHouseForSaleList.SortType obj)
			{
				return (int)obj;
			}
		}
	}
}
