using System;
using Cysharp.Text;
using SoL.Game.Interactives;
using SoL.Game.Loot;
using SoL.Game.Messages;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Game.UI.Archetypes;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI.Loot
{
	// Token: 0x02000981 RID: 2433
	public class LootRollItemUI : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x1700101D RID: 4125
		// (get) Token: 0x0600487C RID: 18556 RVA: 0x00070BFC File Offset: 0x0006EDFC
		// (set) Token: 0x0600487D RID: 18557 RVA: 0x00070C04 File Offset: 0x0006EE04
		private string CannotRollNeedReason
		{
			get
			{
				return this.m_cannotRollNeedReason;
			}
			set
			{
				this.m_cannotRollNeedReason = value;
				if (this.m_tooltips != null && this.m_tooltips[0])
				{
					this.m_tooltips[0].AdditionalLineText = this.m_cannotRollNeedReason;
				}
			}
		}

		// Token: 0x1700101E RID: 4126
		// (get) Token: 0x0600487E RID: 18558 RVA: 0x00070C37 File Offset: 0x0006EE37
		public bool Occupied
		{
			get
			{
				return this.m_item != null;
			}
		}

		// Token: 0x1700101F RID: 4127
		// (get) Token: 0x0600487F RID: 18559 RVA: 0x00070C42 File Offset: 0x0006EE42
		public UniqueId Id
		{
			get
			{
				if (this.m_item != null)
				{
					return this.m_item.Id;
				}
				return UniqueId.Empty;
			}
		}

		// Token: 0x17001020 RID: 4128
		// (get) Token: 0x06004880 RID: 18560 RVA: 0x00070C5D File Offset: 0x0006EE5D
		// (set) Token: 0x06004881 RID: 18561 RVA: 0x00070C65 File Offset: 0x0006EE65
		public LootRollWindow Controller { get; set; }

		// Token: 0x17001021 RID: 4129
		// (get) Token: 0x06004882 RID: 18562 RVA: 0x00070C6E File Offset: 0x0006EE6E
		// (set) Token: 0x06004883 RID: 18563 RVA: 0x00070C76 File Offset: 0x0006EE76
		public bool IsFirst
		{
			get
			{
				return this.m_isFirst;
			}
			set
			{
				this.m_isFirst = value;
				this.RefreshIsFirstData();
			}
		}

		// Token: 0x06004884 RID: 18564 RVA: 0x001A9C1C File Offset: 0x001A7E1C
		private void Awake()
		{
			this.ResetInternalData();
			this.m_need.onClick.AddListener(new UnityAction(this.OnNeedClicked));
			this.m_greed.onClick.AddListener(new UnityAction(this.OnGreedClicked));
			this.m_pass.onClick.AddListener(new UnityAction(this.OnPassClicked));
			this.m_tooltips = new TogglableKeybindTooltip[3];
			this.m_tooltips[0] = this.m_need.gameObject.GetComponent<TogglableKeybindTooltip>();
			this.m_tooltips[1] = this.m_greed.gameObject.GetComponent<TogglableKeybindTooltip>();
			this.m_tooltips[2] = this.m_pass.gameObject.GetComponent<TogglableKeybindTooltip>();
			this.RefreshIsFirstData();
			this.m_needIcon.color = GlobalSettings.Values.Group.LootRollColorNeed;
			this.m_greedIcon.color = GlobalSettings.Values.Group.LootRollColorGreed;
			this.m_passIcon.color = GlobalSettings.Values.Group.LootRollColorPass;
			this.m_timeRemainingFill.color = GlobalSettings.Values.Group.LootRollColorPass;
		}

		// Token: 0x06004885 RID: 18565 RVA: 0x001A9D48 File Offset: 0x001A7F48
		private void OnDestroy()
		{
			this.m_need.onClick.RemoveListener(new UnityAction(this.OnNeedClicked));
			this.m_greed.onClick.RemoveListener(new UnityAction(this.OnGreedClicked));
			this.m_pass.onClick.RemoveListener(new UnityAction(this.OnPassClicked));
		}

		// Token: 0x06004886 RID: 18566 RVA: 0x001A9DAC File Offset: 0x001A7FAC
		private void Update()
		{
			if (this.m_item != null)
			{
				float num = (float)(DateTime.UtcNow - this.m_timeInitialized).TotalSeconds;
				float num2 = GlobalSettings.Values.Group.LootRollClientTimeout * this.m_item.ExpirationMultiplier;
				this.m_timeRemaining = num2 - num;
				float num3 = 1f - num / num2;
				this.m_timeRemainingFill.fillAmount = num3;
				if (num3 <= 0f)
				{
					this.OnPassClicked();
				}
			}
		}

		// Token: 0x06004887 RID: 18567 RVA: 0x001A9E24 File Offset: 0x001A8024
		public void Init(LootRollItem item)
		{
			this.m_item = item;
			this.m_canRollNeed = true;
			if (ClientGroupManager.AutoLootRoll != LootRollChoice.Unanswered)
			{
				this.SendResponse(ClientGroupManager.AutoLootRoll, false);
				this.ResetInternalData();
				return;
			}
			if (this.m_item != null && this.m_item.Instance != null && this.m_item.Instance.Archetype)
			{
				bool flag = this.m_item.Instance.ItemData != null && this.m_item.Instance.ItemData.IsNoTrade;
				RecipeItem recipeItem = this.m_item.Instance.Archetype as RecipeItem;
				if (recipeItem != null)
				{
					if (recipeItem.Recipe && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null)
					{
						bool flag2 = recipeItem.Recipe && LocalPlayer.GameEntity.CollectionController.Recipes != null && LocalPlayer.GameEntity.CollectionController.Recipes.Contains(recipeItem.Recipe.Id);
						ArchetypeInstance archetypeInstance;
						bool flag3 = recipeItem.Recipe && recipeItem.Recipe.Ability && recipeItem.Recipe.Ability.Mastery && LocalPlayer.GameEntity.CollectionController.Masteries != null && LocalPlayer.GameEntity.CollectionController.Masteries.TryGetInstanceForArchetypeId(recipeItem.Recipe.Ability.Mastery.Id, out archetypeInstance);
						AutoRollOption option;
						if (LootRollChoiceExtensions.TryGetAutoRollValue(Options.GameOptions.AutoRollKnownRecipes.Value, out option) && flag2 && flag3)
						{
							this.SendResponse(option.GetChoiceForOption(), false);
							this.ResetInternalData();
							return;
						}
						if (LootRollChoiceExtensions.TryGetAutoRollValue(Options.GameOptions.AutoRollNonUsableRecipes.Value, out option) && !flag3)
						{
							this.SendResponse(option.GetChoiceForOption(), false);
							this.ResetInternalData();
							return;
						}
						if (flag)
						{
							this.m_canRollNeed = (!flag2 && flag3);
							if (!this.m_canRollNeed)
							{
								this.CannotRollNeedReason = (flag2 ? "You already know this recipe!" : "You do not have this profession!");
							}
						}
					}
				}
				else
				{
					if (flag)
					{
						AbilityRecipeItem abilityRecipeItem = this.m_item.Instance.Archetype as AbilityRecipeItem;
						if (abilityRecipeItem != null)
						{
							string cannotRollNeedReason;
							this.m_canRollNeed = abilityRecipeItem.CanRollNeed(LocalPlayer.GameEntity, out cannotRollNeedReason);
							if (!this.m_canRollNeed)
							{
								this.CannotRollNeedReason = cannotRollNeedReason;
								goto IL_43F;
							}
							goto IL_43F;
						}
					}
					ReagentItem reagentItem = this.m_item.Instance.Archetype as ReagentItem;
					AutoRollOption option3;
					if (reagentItem != null)
					{
						if (reagentItem.AssociatedAbility && reagentItem.AssociatedAbility.Mastery && LocalPlayer.GameEntity && LocalPlayer.GameEntity.CharacterData)
						{
							bool flag4 = LocalPlayer.GameEntity.CharacterData.ActiveMasteryId.IsEmpty || LocalPlayer.GameEntity.CharacterData.ActiveMasteryId == reagentItem.AssociatedAbility.Mastery.Id;
							bool flag5 = !reagentItem.AssociatedAbility.Specialization || LocalPlayer.GameEntity.CharacterData.SpecializedRoleId.IsEmpty || LocalPlayer.GameEntity.CharacterData.SpecializedRoleId == reagentItem.AssociatedAbility.Specialization.Id;
							AutoRollOption option2;
							if (LootRollChoiceExtensions.TryGetAutoRollValue(Options.GameOptions.AutoRollNonUsableReagents.Value, out option2) && (!flag4 || !flag5))
							{
								this.SendResponse(option2.GetChoiceForOption(), false);
								this.ResetInternalData();
								return;
							}
							if (flag)
							{
								this.m_canRollNeed = (flag4 && flag5);
								if (!this.m_canRollNeed)
								{
									this.CannotRollNeedReason = ((!flag5) ? "Wrong Specialization!" : "Wrong Role!");
								}
							}
						}
					}
					else if (this.m_item.Instance.Archetype.ItemCategory && this.m_item.Instance.Archetype.ItemCategory == GlobalSettings.Values.UI.FallbackItemCategory && LootRollChoiceExtensions.TryGetAutoRollValue(Options.GameOptions.AutoRollBasicItems.Value, out option3))
					{
						this.SendResponse(option3.GetChoiceForOption(), false);
						this.ResetInternalData();
						return;
					}
				}
				IL_43F:
				IEquipable equipable;
				if (flag && this.m_canRollNeed && this.m_item.Instance.Archetype.TryGetAsType(out equipable))
				{
					this.m_canRollNeed = (equipable.MeetsRoleRequirements(LocalPlayer.GameEntity) && equipable.HasRequiredTrade(LocalPlayer.GameEntity));
					if (!this.m_canRollNeed)
					{
						this.CannotRollNeedReason = "Cannot Equip Item!";
					}
				}
				this.m_timeInitialized = DateTime.UtcNow;
				string text = item.Instance.Archetype.GetModifiedDisplayName(item.Instance);
				Color color;
				if (item.Instance.Archetype.TryGetItemCategoryColor(ItemCategory.ColorFlags.DisplayName, out color))
				{
					text = text.Color(color);
				}
				this.m_label.ZStringSetText(text);
				this.m_archetypeIcon.SetIcon(item.Instance.Archetype, new Color?(item.Instance.Archetype.GetInstanceColor(item.Instance)));
				this.m_need.interactable = this.m_canRollNeed;
				this.m_greed.interactable = true;
				this.m_pass.interactable = true;
				if (item.Instance.ItemData != null)
				{
					if (item.Instance.Archetype.ArchetypeHasCount() && item.Instance.ItemData.Count != null)
					{
						this.m_countLabel.text = item.Instance.ItemData.Count.Value.ToString();
					}
					else if (item.Instance.Archetype.ArchetypeHasCharges() && item.Instance.ItemData.Charges != null)
					{
						this.m_countLabel.text = item.Instance.ItemData.Charges.Value.ToString();
					}
					else
					{
						this.m_countLabel.text = "";
					}
				}
				base.gameObject.transform.SetAsLastSibling();
				base.gameObject.SetActive(true);
				return;
			}
			this.ResetInternalData();
		}

		// Token: 0x06004888 RID: 18568 RVA: 0x001AA46C File Offset: 0x001A866C
		private void ResetInternalData()
		{
			this.m_item = null;
			this.m_timeInitialized = DateTime.MinValue;
			this.m_timeRemaining = 0f;
			this.m_label.text = "";
			this.m_countLabel.text = "";
			this.m_need.interactable = false;
			this.m_greed.interactable = false;
			this.m_pass.interactable = false;
			this.m_canRollNeed = false;
			this.CannotRollNeedReason = null;
			base.gameObject.SetActive(false);
			if (this.Controller != null)
			{
				this.Controller.RefreshFromQueue();
			}
		}

		// Token: 0x06004889 RID: 18569 RVA: 0x001AA510 File Offset: 0x001A8710
		public void ExternalChoice(LootRollChoice choice)
		{
			if (!this.IsFirst)
			{
				return;
			}
			switch (choice)
			{
			case LootRollChoice.Pass:
				if (!this.m_pass.interactable)
				{
					return;
				}
				break;
			case LootRollChoice.Need:
				if (!this.m_need.interactable)
				{
					if (!string.IsNullOrEmpty(this.CannotRollNeedReason))
					{
						UIManager.TriggerCannotPerform(this.CannotRollNeedReason);
					}
					return;
				}
				break;
			case LootRollChoice.Greed:
				if (!this.m_greed.interactable)
				{
					return;
				}
				break;
			default:
				return;
			}
			this.SendResponse(choice, true);
		}

		// Token: 0x0600488A RID: 18570 RVA: 0x00070C85 File Offset: 0x0006EE85
		private void OnNeedClicked()
		{
			this.SendResponse(LootRollChoice.Need, true);
		}

		// Token: 0x0600488B RID: 18571 RVA: 0x00070C8F File Offset: 0x0006EE8F
		private void OnGreedClicked()
		{
			this.SendResponse(LootRollChoice.Greed, true);
		}

		// Token: 0x0600488C RID: 18572 RVA: 0x00070C99 File Offset: 0x0006EE99
		private void OnPassClicked()
		{
			this.SendResponse(LootRollChoice.Pass, false);
		}

		// Token: 0x0600488D RID: 18573 RVA: 0x001AA588 File Offset: 0x001A8788
		private void SendResponse(LootRollChoice choice, bool noTradeConfirmation = false)
		{
			if (this.m_item == null)
			{
				return;
			}
			if (noTradeConfirmation && this.m_item.Instance != null && this.m_item.Instance.ItemData != null && this.m_item.Instance.ItemData.IsNoTrade)
			{
				LootRollChoice choice2 = choice;
				if (choice2 - LootRollChoice.Need <= 1)
				{
					string arg = this.m_item.Instance.ItemData.IsSoulbound ? ArchetypeInstanceUI.kSoulbound : ArchetypeInstanceUI.kNoTrade;
					string text = ZString.Format<string, string>("This item is marked as {0}! Are you sure you want to select {1}? Once looted you can no longer trade this item.", arg, choice.GetLootRollChoiceDescription());
					DialogOptions opts = new DialogOptions
					{
						Title = "No Trade Loot Roll",
						Text = text,
						ConfirmationText = "Yes",
						CancelText = "NO",
						Instance = this.m_item.Instance,
						AutoCancel = (() => this.m_item == null),
						Callback = delegate(bool answer, object obj)
						{
							if (answer)
							{
								this.SendResponse(choice, false);
								return;
							}
							if (this.m_item != null)
							{
								this.m_need.interactable = this.m_canRollNeed;
								this.m_greed.interactable = true;
								this.m_pass.interactable = true;
							}
						}
					};
					this.m_need.interactable = false;
					this.m_greed.interactable = false;
					this.m_pass.interactable = false;
					ClientGameManager.UIManager.ItemConfirmationDialog.Init(opts);
					return;
				}
			}
			LootRollItemResponse response = new LootRollItemResponse
			{
				Id = this.m_item.Id,
				Choice = choice
			};
			if (choice != LootRollChoice.Unanswered && this.m_item.Instance != null && this.m_item.Instance.Archetype != null)
			{
				string arg2 = SoL.Utilities.Extensions.TextMeshProExtensions.CreateInstanceLink(this.m_item.Instance);
				MessageManager.AddLinkedInstance(this.m_item.Instance, false);
				MessageManager.ChatQueue.AddToQueue(MessageType.Loot, ZString.Format<string, string>("{0} selected for {1}", choice.GetLootRollChoiceDescription(), arg2));
			}
			LocalPlayer.NetworkEntity.PlayerRpcHandler.LootRollResponse(response);
			this.ResetInternalData();
		}

		// Token: 0x0600488E RID: 18574 RVA: 0x001AA7A4 File Offset: 0x001A89A4
		private void RefreshIsFirstData()
		{
			if (this.m_tooltips != null)
			{
				for (int i = 0; i < this.m_tooltips.Length; i++)
				{
					if (this.m_tooltips[i] != null)
					{
						this.m_tooltips[i].KeybindTooltipEnabled = this.IsFirst;
					}
				}
			}
			if (this.m_highlight)
			{
				this.m_highlight.enabled = this.IsFirst;
			}
		}

		// Token: 0x0600488F RID: 18575 RVA: 0x001AA810 File Offset: 0x001A8A10
		internal ITooltipParameter GetTooltipParameter()
		{
			if (this.m_item == null || this.m_item.Instance == null)
			{
				return null;
			}
			return new ArchetypeTooltipParameter
			{
				Instance = this.m_item.Instance,
				AdditionalText = ZString.Format<string>("Loot Roll Time Remaining: {0}", this.m_timeRemaining.FormattedTime(0))
			};
		}

		// Token: 0x17001022 RID: 4130
		// (get) Token: 0x06004890 RID: 18576 RVA: 0x00070CA3 File Offset: 0x0006EEA3
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17001023 RID: 4131
		// (get) Token: 0x06004891 RID: 18577 RVA: 0x00070CB1 File Offset: 0x0006EEB1
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x17001024 RID: 4132
		// (get) Token: 0x06004892 RID: 18578 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06004894 RID: 18580 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x040043BE RID: 17342
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x040043BF RID: 17343
		[SerializeField]
		private TextMeshProUGUI m_label;

		// Token: 0x040043C0 RID: 17344
		[SerializeField]
		private TextMeshProUGUI m_countLabel;

		// Token: 0x040043C1 RID: 17345
		[SerializeField]
		private ArchetypeIconUI m_archetypeIcon;

		// Token: 0x040043C2 RID: 17346
		[SerializeField]
		private Image m_timeRemainingFill;

		// Token: 0x040043C3 RID: 17347
		[SerializeField]
		private Image m_highlight;

		// Token: 0x040043C4 RID: 17348
		[SerializeField]
		private SolButton m_need;

		// Token: 0x040043C5 RID: 17349
		[SerializeField]
		private SolButton m_greed;

		// Token: 0x040043C6 RID: 17350
		[SerializeField]
		private SolButton m_pass;

		// Token: 0x040043C7 RID: 17351
		[SerializeField]
		private Image m_needIcon;

		// Token: 0x040043C8 RID: 17352
		[SerializeField]
		private Image m_greedIcon;

		// Token: 0x040043C9 RID: 17353
		[SerializeField]
		private Image m_passIcon;

		// Token: 0x040043CA RID: 17354
		private LootRollItem m_item;

		// Token: 0x040043CB RID: 17355
		private DateTime m_timeInitialized = DateTime.MinValue;

		// Token: 0x040043CC RID: 17356
		private float m_timeRemaining;

		// Token: 0x040043CD RID: 17357
		private TogglableKeybindTooltip[] m_tooltips;

		// Token: 0x040043CE RID: 17358
		private bool m_canRollNeed = true;

		// Token: 0x040043CF RID: 17359
		private string m_cannotRollNeedReason;

		// Token: 0x040043D1 RID: 17361
		private bool m_isFirst;
	}
}
