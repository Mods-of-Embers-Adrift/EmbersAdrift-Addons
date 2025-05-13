using System;
using System.Collections.Generic;
using Cysharp.Text;
using SoL.Game.Crafting;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.UI.Skills;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.UI.Crafting
{
	// Token: 0x02000993 RID: 2451
	public class CraftingUI : DraggableUIWindow
	{
		// Token: 0x140000E0 RID: 224
		// (add) Token: 0x0600492E RID: 18734 RVA: 0x001AD0E8 File Offset: 0x001AB2E8
		// (remove) Token: 0x0600492F RID: 18735 RVA: 0x001AD120 File Offset: 0x001AB320
		public event Action<ArchetypeInstance> MasterySelectionChanged;

		// Token: 0x140000E1 RID: 225
		// (add) Token: 0x06004930 RID: 18736 RVA: 0x001AD158 File Offset: 0x001AB358
		// (remove) Token: 0x06004931 RID: 18737 RVA: 0x001AD190 File Offset: 0x001AB390
		public event Action<Recipe> RecipeSelectionChanged;

		// Token: 0x1700103D RID: 4157
		// (get) Token: 0x06004932 RID: 18738 RVA: 0x000712A0 File Offset: 0x0006F4A0
		public Recipe ExecutingRecipe
		{
			get
			{
				return this.m_executingRecipe;
			}
		}

		// Token: 0x1700103E RID: 4158
		// (get) Token: 0x06004933 RID: 18739 RVA: 0x000712A8 File Offset: 0x0006F4A8
		public int ExecutingTargetAbilityLevel
		{
			get
			{
				return this.m_executingTargetAbilityLevel;
			}
		}

		// Token: 0x1700103F RID: 4159
		// (get) Token: 0x06004934 RID: 18740 RVA: 0x000712B0 File Offset: 0x0006F4B0
		public List<ItemUsage> ExecutingComponentSelection
		{
			get
			{
				return this.m_executingComponentSelection;
			}
		}

		// Token: 0x17001040 RID: 4160
		// (get) Token: 0x06004935 RID: 18741 RVA: 0x000712B8 File Offset: 0x0006F4B8
		// (set) Token: 0x06004936 RID: 18742 RVA: 0x000712C0 File Offset: 0x0006F4C0
		public InteractiveRefinementStation RefinementStation { get; set; }

		// Token: 0x06004937 RID: 18743 RVA: 0x001AD1C8 File Offset: 0x001AB3C8
		protected override void Start()
		{
			base.Start();
			this.m_hideUncraftable.isOn = Options.GameOptions.HideUncraftable.Value;
			this.m_hideUncraftable.onValueChanged.AddListener(new UnityAction<bool>(this.OnHideUncraftableChanged));
			this.m_listUI.SelectionChanged += this.OnListItemSelected;
			ComponentsList componentsListUI = this.m_componentsListUI;
			componentsListUI.SelectionsChanged = (Action)Delegate.Combine(componentsListUI.SelectionsChanged, new Action(this.m_componentsListUI.MarkSelectionDirty));
			ComponentsList componentsListUI2 = this.m_componentsListUI;
			componentsListUI2.SelectionsChanged = (Action)Delegate.Combine(componentsListUI2.SelectionsChanged, new Action(this.OnComponentSelectionChanged));
			this.m_tabController.TabChanged += this.TabControllerOnTabChanged;
			this.m_refinementCountInput.onValueChanged.AddListener(new UnityAction<string>(this.OnRefinementCountInputChanged));
			this.m_refinementCountInput.onSelect.AddListener(new UnityAction<string>(this.OnRefinementCountFocused));
			this.m_refinementCountInput.onDeselect.AddListener(new UnityAction<string>(this.OnRefinementCountBlurred));
			this.m_decrementCount.onClick.AddListener(new UnityAction(this.OnDecrementCountClicked));
			this.m_incrementCount.onClick.AddListener(new UnityAction(this.OnIncrementCountClicked));
			this.MasterySelectionChanged += this.OnMasterySelectionChanged;
			this.RecipeSelectionChanged += this.OnRecipeSelectionChanged;
			if (LocalPlayer.IsInitialized)
			{
				this.LocalPlayerOnLocalPlayerInitialized();
			}
			else
			{
				LocalPlayer.LocalPlayerInitialized += this.LocalPlayerOnLocalPlayerInitialized;
			}
			this.m_downscaleSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnDownscaleSliderValueChanged));
			this.m_downscaleLockToMaxToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnDownscaleLockToMaxToggleChanged));
			this.m_craftButton.onClick.AddListener(new UnityAction(this.OnCraftClicked));
			this.m_craftAllButton.onClick.AddListener(new UnityAction(this.OnCraftAllClicked));
			this.m_deconstructButton.onClick.AddListener(new UnityAction(this.OnDeconstructClicked));
			this.m_downscaleSlider.minValue = 1f;
			this.m_downscaleSlider.maxValue = 50f;
			this.m_downscaleSlider.value = 50f;
			this.m_downscaleSlider.wholeNumbers = true;
			this.RefreshAvailableMasteries();
		}

		// Token: 0x06004938 RID: 18744 RVA: 0x001AD42C File Offset: 0x001AB62C
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.m_hideUncraftable.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnHideUncraftableChanged));
			this.m_listUI.SelectionChanged -= this.OnListItemSelected;
			ComponentsList componentsListUI = this.m_componentsListUI;
			componentsListUI.SelectionsChanged = (Action)Delegate.Remove(componentsListUI.SelectionsChanged, new Action(this.m_componentsListUI.MarkSelectionDirty));
			ComponentsList componentsListUI2 = this.m_componentsListUI;
			componentsListUI2.SelectionsChanged = (Action)Delegate.Remove(componentsListUI2.SelectionsChanged, new Action(this.OnComponentSelectionChanged));
			if (this.m_selectedMastery != null && this.m_selectedMastery.MasteryData != null)
			{
				this.m_selectedMastery.MasteryData.LevelDataChanged -= this.OnMasteryLevelChanged;
			}
			this.m_tabController.TabChanged -= this.TabControllerOnTabChanged;
			this.m_refinementCountInput.onValueChanged.RemoveListener(new UnityAction<string>(this.OnRefinementCountInputChanged));
			this.m_refinementCountInput.onSelect.RemoveListener(new UnityAction<string>(this.OnRefinementCountFocused));
			this.m_refinementCountInput.onDeselect.RemoveListener(new UnityAction<string>(this.OnRefinementCountBlurred));
			this.m_decrementCount.onClick.RemoveListener(new UnityAction(this.OnDecrementCountClicked));
			this.m_incrementCount.onClick.RemoveListener(new UnityAction(this.OnIncrementCountClicked));
			this.MasterySelectionChanged -= this.OnMasterySelectionChanged;
			this.RecipeSelectionChanged -= this.OnRecipeSelectionChanged;
			LocalPlayer.LocalPlayerInitialized -= this.LocalPlayerOnLocalPlayerInitialized;
			if (LocalPlayer.GameEntity)
			{
				ICollectionController collectionController = LocalPlayer.GameEntity.CollectionController;
				if (collectionController != null)
				{
					if (collectionController.Masteries != null)
					{
						collectionController.Masteries.ContentsChanged -= this.CollectionControllerOnMasteriesChanged;
					}
					if (collectionController.Recipes != null)
					{
						collectionController.Recipes.ContentsChanged -= this.CollectionControllerOnRecipesChanged;
					}
					if (collectionController.Inventory != null)
					{
						collectionController.Inventory.ContentsChanged -= this.CollectionControllerOnInventoryChanged;
						collectionController.Inventory.ContentsChanged -= this.m_listUI.OnInventoryChanged;
						collectionController.Inventory.InstanceAdded -= this.m_componentsListUI.OnInventoryAddition;
						collectionController.Inventory.InstanceRemoved -= this.m_componentsListUI.OnInventoryRemoval;
						collectionController.Inventory.InstanceRemoved -= this.CollectionControllerOnItemRemoved;
						collectionController.Inventory.QuantityOfItemChanged -= this.CollectionControllerOnQuantityChanged;
						collectionController.Inventory.QuantityOfItemChanged -= this.m_componentsListUI.OnQuantityOfItemChanged;
					}
					if (collectionController.Gathering != null)
					{
						collectionController.Gathering.ContentsChanged -= this.CollectionControllerOnInventoryChanged;
						collectionController.Gathering.ContentsChanged -= this.m_listUI.OnInventoryChanged;
						collectionController.Gathering.InstanceAdded -= this.m_componentsListUI.OnInventoryAddition;
						collectionController.Gathering.InstanceRemoved -= this.m_componentsListUI.OnInventoryRemoval;
						collectionController.Gathering.InstanceRemoved -= this.CollectionControllerOnItemRemoved;
						collectionController.Gathering.QuantityOfItemChanged -= this.CollectionControllerOnQuantityChanged;
						collectionController.Gathering.QuantityOfItemChanged -= this.m_componentsListUI.OnQuantityOfItemChanged;
					}
				}
				if (LocalPlayer.GameEntity.SkillsController != null)
				{
					LocalPlayer.GameEntity.SkillsController.PendingExecutionChanged -= this.SkillsControllerOnPendingExecutionChanged;
				}
				if (LocalPlayer.GameEntity.VitalsReplicator != null)
				{
					LocalPlayer.GameEntity.VitalsReplicator.CurrentStance.Changed -= this.CurrentStanceOnChanged;
				}
			}
			this.m_downscaleSlider.onValueChanged.RemoveListener(new UnityAction<float>(this.OnDownscaleSliderValueChanged));
			this.m_downscaleLockToMaxToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnDownscaleLockToMaxToggleChanged));
			this.m_craftButton.onClick.RemoveListener(new UnityAction(this.OnCraftClicked));
			this.m_craftAllButton.onClick.RemoveListener(new UnityAction(this.OnCraftAllClicked));
			this.m_deconstructButton.onClick.RemoveListener(new UnityAction(this.OnDeconstructClicked));
			this.m_listUI.Initialized -= this.UpdateList;
			this.m_listUI.Initialized -= this.m_listUI.DeselectAll;
			this.m_componentsListUI.Initialized -= this.m_componentsListUI.CloseDropdown;
			this.ExitDeconstructMode();
		}

		// Token: 0x06004939 RID: 18745 RVA: 0x000712C9 File Offset: 0x0006F4C9
		private void Update()
		{
			if (base.Visible && this.m_shouldTriggerAnother)
			{
				this.m_shouldTriggerAnother = false;
				this.Craft();
			}
		}

		// Token: 0x0600493A RID: 18746 RVA: 0x001AD8EC File Offset: 0x001ABAEC
		public override void Show(bool skipTransition = false)
		{
			base.Show(skipTransition);
			if (LocalPlayer.GameEntity)
			{
				if (LocalPlayer.GameEntity.CollectionController.RefinementStation == null)
				{
					this.m_windowLabel.ZStringSetText("Recipes");
				}
				else if (LocalPlayer.GameEntity.CollectionController.RefinementStation.Profile == null || LocalPlayer.GameEntity.CollectionController.RefinementStation.Profile.Category == CraftingStationCategory.None || LocalPlayer.GameEntity.CollectionController.RefinementStation.Profile.Category == CraftingStationCategory.General)
				{
					this.m_windowLabel.ZStringSetText("Crafting Station");
				}
				else
				{
					this.m_windowLabel.ZStringSetText(LocalPlayer.GameEntity.CollectionController.RefinementStation.Profile.Category.ToStringWithSpaces());
				}
			}
			this.RefreshAvailableMasteries();
			this.RefreshAvailableRecipes();
			this.UnlockUI();
			this.m_componentsListUI.UpdateRecipe(this.m_selectedRecipe);
		}

		// Token: 0x0600493B RID: 18747 RVA: 0x000712E8 File Offset: 0x0006F4E8
		public override void Hide(bool skipTransition = false)
		{
			base.Hide(skipTransition);
			this.ExitDeconstructMode();
			this.m_refinementCountInput.Deactivate();
		}

		// Token: 0x0600493C RID: 18748 RVA: 0x00071302 File Offset: 0x0006F502
		private void OnHideUncraftableChanged(bool isOn)
		{
			Options.GameOptions.HideUncraftable.Value = isOn;
			this.RefreshAvailableRecipes();
		}

		// Token: 0x0600493D RID: 18749 RVA: 0x00071315 File Offset: 0x0006F515
		private void OnListItemSelected(Recipe recipe)
		{
			this.m_selectedRecipe = recipe;
			Action<Recipe> recipeSelectionChanged = this.RecipeSelectionChanged;
			if (recipeSelectionChanged == null)
			{
				return;
			}
			recipeSelectionChanged(recipe);
		}

		// Token: 0x0600493E RID: 18750 RVA: 0x001AD9F4 File Offset: 0x001ABBF4
		private void TabControllerOnTabChanged()
		{
			if (!this.m_ignoreTabChangedEvent)
			{
				ArchetypeInstance selectedMastery = this.m_selectedMastery;
				this.m_selectedMastery.MasteryData.LevelDataChanged -= this.OnMasteryLevelChanged;
				this.m_selectedMastery = this.GetActiveMasteryInstance();
				this.m_selectedMastery.MasteryData.LevelDataChanged += this.OnMasteryLevelChanged;
				if (selectedMastery != this.m_selectedMastery)
				{
					Action<ArchetypeInstance> masterySelectionChanged = this.MasterySelectionChanged;
					if (masterySelectionChanged == null)
					{
						return;
					}
					masterySelectionChanged(this.m_selectedMastery);
				}
			}
		}

		// Token: 0x0600493F RID: 18751 RVA: 0x001ADA74 File Offset: 0x001ABC74
		private void OnMasteryLevelChanged()
		{
			this.m_downscaleSlider.maxValue = (float)((int)this.m_selectedMastery.GetAssociatedLevel(LocalPlayer.GameEntity));
			this.m_downscaleSlider.value = (this.m_downscaleLockToMaxToggle.isOn ? ((float)((int)this.m_downscaleSlider.maxValue)) : Math.Min(Math.Max(this.m_downscaleSlider.minValue, this.m_downscaleSlider.value), this.m_downscaleSlider.maxValue));
			this.UpdateTierMarkers();
			this.m_listUI.ReindexItems(this.m_selectedRecipe);
		}

		// Token: 0x06004940 RID: 18752 RVA: 0x001ADB08 File Offset: 0x001ABD08
		private void OnMasterySelectionChanged(ArchetypeInstance newSelection)
		{
			if (this.m_listUI.IsInitialized)
			{
				this.m_listUI.DeselectAll();
			}
			else
			{
				this.m_listUI.Initialized += this.m_listUI.DeselectAll;
			}
			this.m_dropdownContainer.gameObject.SetActive(false);
			this.RefreshAvailableRecipes();
			this.m_downscaleSlider.maxValue = ((newSelection != null && newSelection.MasteryData != null) ? ((float)((int)newSelection.GetAssociatedLevel(LocalPlayer.GameEntity))) : 50f);
			this.m_downscaleSlider.value = ((newSelection != null && newSelection.MasteryData != null) ? ((float)((int)newSelection.GetAssociatedLevel(LocalPlayer.GameEntity))) : 50f);
			this.m_downscaleLockToMaxToggle.isOn = true;
		}

		// Token: 0x06004941 RID: 18753 RVA: 0x001ADBC4 File Offset: 0x001ABDC4
		private void OnRecipeSelectionChanged(Recipe newSelection)
		{
			if (this.m_executingAbility != null && LocalPlayer.GameEntity != null)
			{
				LocalPlayer.GameEntity.SkillsController.EscapePressed();
			}
			else
			{
				this.m_amountToCraft = 1;
				this.m_refinementCountInput.text = this.m_amountToCraft.ToString();
			}
			this.m_componentsListUI.UpdateRecipe(newSelection);
			if (newSelection != null)
			{
				ArchetypeInstance archetypeInstance;
				bool flag = LocalPlayer.GameEntity.CollectionController.Abilities.TryGetInstanceForArchetypeId(newSelection.Ability.Id, out archetypeInstance);
				this.m_blockerPanel.SetActive(false);
				this.m_recipeLowLevelIndicator.gameObject.SetActive(flag && archetypeInstance.GetAssociatedLevelInteger(LocalPlayer.GameEntity) < newSelection.MinimumAbilityLevel);
				this.m_recipeIcon.sprite = newSelection.Icon;
				this.m_recipeIcon.color = newSelection.IconTint;
				this.m_recipeNameLabel.text = newSelection.DisplayName;
				this.m_recipeAbilityLabel.text = string.Format("{0} (Lvl {1})", newSelection.Ability.DisplayName, newSelection.MinimumAbilityLevel);
				if (!string.IsNullOrEmpty(newSelection.Description))
				{
					this.m_recipeDescriptionLabel.text = newSelection.Description;
				}
				else
				{
					this.m_recipeDescriptionLabel.text = "[ No description yet :( ]";
				}
				this.UpdateDownscaleSliderVisibility();
				this.RefreshCraftingButtonInteractibility();
			}
			else
			{
				this.m_blockerPanel.SetActive(true);
				this.m_craftingButtonTooltipRegion.gameObject.SetActive(false);
			}
			this.UpdateOutputPreview();
			this.UpdateTierMarkers();
		}

		// Token: 0x06004942 RID: 18754 RVA: 0x001ADD4C File Offset: 0x001ABF4C
		private void OnComponentSelectionChanged()
		{
			int num;
			int num2;
			int val;
			int num3;
			int num4;
			int num5;
			this.m_componentsListUI.FindItemsUsedInSelection().GetAggregateMaterialLevel(false, out num, out num2, out val, out num3, out num4, out num5);
			int num6 = (int)this.m_downscaleSlider.minValue;
			this.m_downscaleSlider.minValue = (float)Math.Max(val, 1);
			if (num6 != (int)this.m_downscaleSlider.minValue)
			{
				this.UpdateTierMarkers();
			}
			this.RefreshCanMakeCache();
			this.UpdateCanCraftCounter();
			this.RefreshCraftingButtonInteractibility();
			this.UpdateOutputPreview();
			this.UpdateDownscaleSliderVisibility();
		}

		// Token: 0x06004943 RID: 18755 RVA: 0x001ADDC8 File Offset: 0x001ABFC8
		private void LocalPlayerOnLocalPlayerInitialized()
		{
			LocalPlayer.GameEntity.CollectionController.Masteries.ContentsChanged += this.CollectionControllerOnMasteriesChanged;
			LocalPlayer.GameEntity.CollectionController.Recipes.ContentsChanged += this.CollectionControllerOnRecipesChanged;
			LocalPlayer.GameEntity.CollectionController.Inventory.ContentsChanged += this.CollectionControllerOnInventoryChanged;
			LocalPlayer.GameEntity.CollectionController.Inventory.ContentsChanged += this.m_listUI.OnInventoryChanged;
			LocalPlayer.GameEntity.CollectionController.Inventory.InstanceAdded += this.m_componentsListUI.OnInventoryAddition;
			LocalPlayer.GameEntity.CollectionController.Inventory.InstanceRemoved += this.m_componentsListUI.OnInventoryRemoval;
			LocalPlayer.GameEntity.CollectionController.Inventory.InstanceRemoved += this.CollectionControllerOnItemRemoved;
			LocalPlayer.GameEntity.CollectionController.Inventory.QuantityOfItemChanged += this.CollectionControllerOnQuantityChanged;
			LocalPlayer.GameEntity.CollectionController.Inventory.QuantityOfItemChanged += this.m_componentsListUI.OnQuantityOfItemChanged;
			LocalPlayer.GameEntity.CollectionController.Gathering.ContentsChanged += this.CollectionControllerOnInventoryChanged;
			LocalPlayer.GameEntity.CollectionController.Gathering.ContentsChanged += this.m_listUI.OnInventoryChanged;
			LocalPlayer.GameEntity.CollectionController.Gathering.InstanceAdded += this.m_componentsListUI.OnInventoryAddition;
			LocalPlayer.GameEntity.CollectionController.Gathering.InstanceRemoved += this.m_componentsListUI.OnInventoryRemoval;
			LocalPlayer.GameEntity.CollectionController.Gathering.InstanceRemoved += this.CollectionControllerOnItemRemoved;
			LocalPlayer.GameEntity.CollectionController.Gathering.QuantityOfItemChanged += this.CollectionControllerOnQuantityChanged;
			LocalPlayer.GameEntity.CollectionController.Gathering.QuantityOfItemChanged += this.m_componentsListUI.OnQuantityOfItemChanged;
			LocalPlayer.GameEntity.SkillsController.PendingExecutionChanged += this.SkillsControllerOnPendingExecutionChanged;
			LocalPlayer.GameEntity.VitalsReplicator.CurrentStance.Changed += this.CurrentStanceOnChanged;
			if (base.Visible)
			{
				this.RefreshAvailableMasteries();
				this.RefreshAvailableRecipes();
			}
		}

		// Token: 0x06004944 RID: 18756 RVA: 0x0007132F File Offset: 0x0006F52F
		private void CollectionControllerOnMasteriesChanged()
		{
			if (base.Visible)
			{
				this.RefreshAvailableMasteries();
			}
		}

		// Token: 0x06004945 RID: 18757 RVA: 0x0007133F File Offset: 0x0006F53F
		private void CollectionControllerOnRecipesChanged()
		{
			if (base.Visible)
			{
				this.RefreshAvailableRecipes();
			}
		}

		// Token: 0x06004946 RID: 18758 RVA: 0x0007134F File Offset: 0x0006F54F
		private void CollectionControllerOnInventoryChanged()
		{
			if (base.Visible)
			{
				this.RefreshAvailableRecipes();
				this.UpdateCanCraftCounter();
				this.RefreshCraftingButtonInteractibility();
				this.UpdateOutputPreview();
				this.m_componentsListUI.UpdateRecipe(this.m_selectedRecipe);
			}
		}

		// Token: 0x06004947 RID: 18759 RVA: 0x001AE04C File Offset: 0x001AC24C
		private void CollectionControllerOnItemRemoved(ArchetypeInstance instance)
		{
			if (base.Visible)
			{
				this.m_executingComponentSelection.RemoveAll((ItemUsage x) => x.Instance.InstanceId == instance.InstanceId);
			}
		}

		// Token: 0x06004948 RID: 18760 RVA: 0x00071382 File Offset: 0x0006F582
		private void CollectionControllerOnQuantityChanged()
		{
			if (base.Visible)
			{
				this.RefreshCanMakeCache();
				this.UpdateList();
				this.UpdateCanCraftCounter();
			}
		}

		// Token: 0x06004949 RID: 18761 RVA: 0x0007139E File Offset: 0x0006F59E
		private void CurrentStanceOnChanged(Stance newStance)
		{
			if (base.Visible)
			{
				this.RefreshCraftingButtonInteractibility();
			}
		}

		// Token: 0x0600494A RID: 18762 RVA: 0x001AE088 File Offset: 0x001AC288
		private ArchetypeInstance GetActiveMasteryInstance()
		{
			for (int i = 0; i < this.m_masteryTabs.Length; i++)
			{
				if (this.m_masteryTabs[i].Instance != null && this.m_masteryTabs[i].ToggleIsOn)
				{
					return this.m_masteryTabs[i].Instance;
				}
			}
			for (int j = 0; j < this.m_inverseMasteryTabs.Length; j++)
			{
				if (this.m_inverseMasteryTabs[j].Instance != null && this.m_inverseMasteryTabs[j].ToggleIsOn)
				{
					return this.m_inverseMasteryTabs[j].Instance;
				}
			}
			return null;
		}

		// Token: 0x0600494B RID: 18763 RVA: 0x001AE114 File Offset: 0x001AC314
		private void RefreshAvailableMasteries()
		{
			this.m_ignoreTabChangedEvent = true;
			ArchetypeInstance activeMasteryInstance = this.GetActiveMasteryInstance();
			MasteryTabUI masteryTabUI = this.m_masteryTabs[0];
			int num = 0;
			int num2 = 0;
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController != null && LocalPlayer.GameEntity.CollectionController.Masteries != null)
			{
				foreach (ArchetypeInstance archetypeInstance in LocalPlayer.GameEntity.CollectionController.Masteries.Instances)
				{
					MasteryArchetype masteryArchetype;
					if (archetypeInstance.Archetype.TryGetAsType(out masteryArchetype) && masteryArchetype.IsAtCorrectCraftingStation(LocalPlayer.GameEntity))
					{
						if (!masteryArchetype.InvertListing)
						{
							if (num < this.m_masteryTabs.Length)
							{
								this.m_masteryTabs[num].Instance = archetypeInstance;
							}
							if (activeMasteryInstance == archetypeInstance)
							{
								masteryTabUI = this.m_masteryTabs[num];
							}
							num++;
						}
						else
						{
							if (num2 < this.m_inverseMasteryTabs.Length)
							{
								this.m_inverseMasteryTabs[num2].Instance = archetypeInstance;
							}
							if (activeMasteryInstance == archetypeInstance)
							{
								masteryTabUI = this.m_inverseMasteryTabs[num2];
							}
							num2++;
						}
					}
				}
			}
			for (int i = num; i < this.m_masteryTabs.Length; i++)
			{
				this.m_masteryTabs[i].Instance = null;
				this.m_masteryTabs[i].ToggleIsOn = false;
			}
			for (int j = num2; j < this.m_inverseMasteryTabs.Length; j++)
			{
				this.m_inverseMasteryTabs[j].Instance = null;
				this.m_inverseMasteryTabs[j].ToggleIsOn = false;
			}
			if (masteryTabUI.Instance != null)
			{
				UniqueId instanceId = masteryTabUI.Instance.InstanceId;
				ArchetypeInstance selectedMastery = this.m_selectedMastery;
				if (!(instanceId != ((selectedMastery != null) ? new UniqueId?(selectedMastery.InstanceId) : null)))
				{
					goto IL_256;
				}
			}
			if (this.m_selectedMastery != null && this.m_selectedMastery.MasteryData != null)
			{
				this.m_selectedMastery.MasteryData.LevelDataChanged -= this.OnMasteryLevelChanged;
			}
			this.m_selectedMastery = masteryTabUI.Instance;
			if (this.m_selectedMastery != null && this.m_selectedMastery.MasteryData != null)
			{
				this.m_selectedMastery.MasteryData.LevelDataChanged += this.OnMasteryLevelChanged;
			}
			Action<ArchetypeInstance> masterySelectionChanged = this.MasterySelectionChanged;
			if (masterySelectionChanged != null)
			{
				masterySelectionChanged(this.m_selectedMastery);
			}
			IL_256:
			if (masteryTabUI.Instance != null)
			{
				masteryTabUI.ToggleIsOn = true;
			}
			this.m_ignoreTabChangedEvent = false;
		}

		// Token: 0x0600494C RID: 18764 RVA: 0x001AE3A0 File Offset: 0x001AC5A0
		private void RefreshAvailableRecipes()
		{
			this.m_availableRecipes.Clear();
			bool flag = false;
			if (this.m_selectedMastery != null)
			{
				using (IEnumerator<LearnableArchetype> enumerator = LocalPlayer.GameEntity.CollectionController.Recipes.Learnables.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Recipe recipe;
						if (enumerator.Current.TryGetAsType(out recipe) && recipe.Mastery.Id == this.m_selectedMastery.ArchetypeId)
						{
							this.m_availableRecipes.Add(recipe);
							if (this.m_selectedRecipe != null && recipe.Id == this.m_selectedRecipe.Id)
							{
								flag = true;
							}
						}
					}
				}
			}
			this.RefreshCanMakeCache();
			if (!flag)
			{
				this.m_selectedRecipe = null;
				Action<Recipe> recipeSelectionChanged = this.RecipeSelectionChanged;
				if (recipeSelectionChanged != null)
				{
					recipeSelectionChanged(this.m_selectedRecipe);
				}
			}
			if (this.m_listUI.IsInitialized)
			{
				this.UpdateList();
				return;
			}
			this.m_listUI.Initialized += this.UpdateList;
		}

		// Token: 0x0600494D RID: 18765 RVA: 0x001AE4B8 File Offset: 0x001AC6B8
		private void RefreshCanMakeCache()
		{
			this.m_canMakeCache.Clear();
			ContainerInstance containerInstance;
			ContainerInstance containerInstance2;
			if (LocalPlayer.GameEntity != null && LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.Inventory, out containerInstance) && LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.Gathering, out containerInstance2))
			{
				List<ArchetypeInstance> fromPool = StaticListPool<ArchetypeInstance>.GetFromPool();
				fromPool.Capacity = containerInstance.Count + containerInstance2.Count;
				fromPool.AddRange(containerInstance2.Instances);
				fromPool.AddRange(containerInstance.Instances);
				foreach (Recipe recipe in this.m_availableRecipes)
				{
					List<ItemUsage> list;
					RecipeComponent recipeComponent;
					this.m_canMakeCache.Add(recipe.Id, recipe.Match(fromPool, out list, out recipeComponent));
				}
				if (this.m_selectedRecipe != null)
				{
					List<ItemUsage> list;
					RecipeComponent recipeComponent;
					this.m_canMakeCache.AddOrReplace(this.m_selectedRecipe.Id, this.m_selectedRecipe.Match(fromPool, out list, out recipeComponent));
				}
				StaticListPool<ArchetypeInstance>.ReturnToPool(fromPool);
			}
		}

		// Token: 0x0600494E RID: 18766 RVA: 0x001AE5D8 File Offset: 0x001AC7D8
		private void RefreshCraftingButtonInteractibility()
		{
			ArchetypeInstance archetypeInstance = null;
			ContainerInstance containerInstance;
			if (LocalPlayer.GameEntity && LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.Inventory, out containerInstance) && (this.m_selectedRecipe == null || LocalPlayer.GameEntity.CollectionController.Abilities.TryGetInstanceForArchetypeId(this.m_selectedRecipe.Ability.Id, out archetypeInstance)))
			{
				bool flag = this.RefinementStation != null;
				bool flag2 = containerInstance.GetMaxCapacity() - containerInstance.Count >= 1;
				bool flag3 = this.m_componentsListUI.AllComponentsHaveSelection();
				bool flag4 = !this.m_componentsListUI.SelectionInvalid;
				bool flag5 = LocalPlayer.Animancer.Stance == Stance.Idle;
				bool flag6 = !(this.m_selectedRecipe != null) || archetypeInstance.GetAssociatedLevelInteger(LocalPlayer.GameEntity) >= this.m_selectedRecipe.MinimumAbilityLevel;
				bool flag7 = flag && (this.RefinementStation.Profile == null || (this.m_selectedRecipe && this.m_selectedRecipe.StationCategory.HasBitFlag(this.RefinementStation.Profile.Category)));
				if (this.m_craftingLocked)
				{
					this.m_craftButton.interactable = false;
					this.m_craftAllButton.interactable = false;
					this.m_craftingButtonTooltipRegion.gameObject.SetActive(false);
					this.m_deconstructButton.interactable = false;
					this.m_deconstructButtonTooltipRegion.gameObject.SetActive(false);
					return;
				}
				this.m_craftButton.interactable = (flag && flag2 && flag3 && flag4 && flag5 && flag6 && flag7);
				this.m_craftAllButton.interactable = (flag && flag2 && flag3 && flag4 && flag5 && flag6 && flag7);
				this.m_craftingButtonTooltipRegion.gameObject.SetActive(!flag || !flag2 || !flag3 || !flag4 || !flag5 || !flag6 || !flag7);
				this.m_deconstructButtonTooltipRegion.gameObject.SetActive(!flag);
				this.m_deconstructButton.interactable = flag;
				if (!flag)
				{
					this.m_craftingButtonTooltipRegion.Text = "Visit a crafting station to make items.";
					return;
				}
				if (!flag7)
				{
					this.m_craftingButtonTooltipRegion.Text = "This recipe cannot be crafted at this type of station.";
					return;
				}
				if (!flag2)
				{
					this.m_craftingButtonTooltipRegion.Text = "At least one space must be available in your inventory.";
					return;
				}
				if (!flag3)
				{
					this.m_craftingButtonTooltipRegion.Text = "You must select an item for all required components.";
					return;
				}
				if (!flag4)
				{
					this.m_craftingButtonTooltipRegion.Text = "Your currect selection is not craftable.";
					return;
				}
				if (!flag5)
				{
					this.m_craftingButtonTooltipRegion.Text = "You must be standing without a torch.";
					return;
				}
				if (!flag6)
				{
					this.m_craftingButtonTooltipRegion.Text = ZString.Format<string, int>("Your {0} ability must be level {1} to craft this item.", this.m_selectedRecipe.Ability.DisplayName, this.m_selectedRecipe.MinimumAbilityLevel);
				}
			}
		}

		// Token: 0x0600494F RID: 18767 RVA: 0x001AE898 File Offset: 0x001ACA98
		private int AmountCraftable()
		{
			if (LocalPlayer.GameEntity == null || this.m_selectedRecipe == null || !this.m_componentsListUI.AllComponentsHaveSelection())
			{
				return 0;
			}
			this.m_tempAmounts.Clear();
			this.m_tempAmountsUsed.Clear();
			List<ArchetypeInstance> fromPool = StaticListPool<ArchetypeInstance>.GetFromPool();
			fromPool.Capacity = LocalPlayer.GameEntity.CollectionController.Inventory.Count + LocalPlayer.GameEntity.CollectionController.Gathering.Count;
			fromPool.AddRange(LocalPlayer.GameEntity.CollectionController.Inventory.Instances);
			fromPool.AddRange(LocalPlayer.GameEntity.CollectionController.Gathering.Instances);
			foreach (ArchetypeInstance archetypeInstance in fromPool)
			{
				if (!this.m_tempAmounts.ContainsKey(archetypeInstance.CombinedTypeCode))
				{
					this.m_tempAmounts.Add(archetypeInstance.CombinedTypeCode, 0);
				}
				Dictionary<int, int> dictionary = this.m_tempAmounts;
				int combinedTypeCode = archetypeInstance.CombinedTypeCode;
				Dictionary<int, int> dictionary2 = dictionary;
				int key = combinedTypeCode;
				int num = dictionary[combinedTypeCode];
				ItemInstanceData itemData = archetypeInstance.ItemData;
				dictionary2[key] = num + (((itemData != null) ? itemData.Count : null) ?? 1);
			}
			StaticListPool<ArchetypeInstance>.ReturnToPool(fromPool);
			List<ItemUsage> list = this.m_componentsListUI.FindItemsUsedInSelection();
			foreach (ItemUsage itemUsage in list)
			{
				if (!this.m_tempAmountsUsed.ContainsKey(itemUsage.Instance.CombinedTypeCode))
				{
					this.m_tempAmountsUsed.Add(itemUsage.Instance.CombinedTypeCode, 0);
				}
				Dictionary<int, int> dictionary = this.m_tempAmountsUsed;
				int combinedTypeCode = itemUsage.Instance.CombinedTypeCode;
				dictionary[combinedTypeCode] += itemUsage.AmountUsed;
			}
			int num2 = 99;
			foreach (ItemUsage itemUsage2 in list)
			{
				num2 = ((this.m_tempAmountsUsed[itemUsage2.Instance.CombinedTypeCode] != 0) ? Math.Min(num2, this.m_tempAmounts[itemUsage2.Instance.CombinedTypeCode] / this.m_tempAmountsUsed[itemUsage2.Instance.CombinedTypeCode]) : 0);
			}
			return num2;
		}

		// Token: 0x06004950 RID: 18768 RVA: 0x001AEB44 File Offset: 0x001ACD44
		private void UpdateTierMarkers()
		{
			if (!(this.m_selectedRecipe != null) || (int)this.m_downscaleSlider.maxValue - (int)this.m_downscaleSlider.minValue <= 0)
			{
				foreach (Image image in this.m_tierMarkers)
				{
					image.gameObject.SetActive(false);
				}
				return;
			}
			OutputItemOverride outputItemOverride = this.m_selectedRecipe.FindApplicableOverride(this.m_componentsListUI.FindItemsUsedInSelection());
			RecipeOutputType outputType;
			TieredItemProfile tierProfile;
			if (outputItemOverride != null)
			{
				outputType = outputItemOverride.OutputType;
				tierProfile = outputItemOverride.TierProfile;
			}
			else
			{
				outputType = this.m_selectedRecipe.OutputType;
				tierProfile = this.m_selectedRecipe.TierProfile;
			}
			if (outputType != RecipeOutputType.AbilityTiered)
			{
				foreach (Image image2 in this.m_tierMarkers)
				{
					image2.gameObject.SetActive(false);
				}
				return;
			}
			int num = (int)this.m_downscaleSlider.minValue;
			int num2 = (int)this.m_downscaleSlider.maxValue;
			int i = 0;
			foreach (ItemTier itemTier in tierProfile.Tiers)
			{
				if (itemTier.MinimumLevel >= num && itemTier.MinimumLevel <= num2)
				{
					float num3 = ((RectTransform)this.m_initialTierMarker.rectTransform.parent).rect.width / (float)(num2 - num);
					int num4 = itemTier.MinimumLevel - num;
					Image image3 = null;
					if (i == this.m_tierMarkers.Count)
					{
						image3 = UnityEngine.Object.Instantiate<Image>(this.m_initialTierMarker, this.m_initialTierMarker.rectTransform.parent);
						this.m_tierMarkers.Add(image3);
					}
					else if (i < this.m_tierMarkers.Count)
					{
						image3 = this.m_tierMarkers[i];
					}
					image3.gameObject.SetActive(true);
					image3.rectTransform.anchoredPosition = new Vector2(num3 * (float)num4, 0f);
					i++;
				}
			}
			while (i < this.m_tierMarkers.Count)
			{
				this.m_tierMarkers[i].gameObject.SetActive(false);
				i++;
			}
		}

		// Token: 0x06004951 RID: 18769 RVA: 0x001AEDAC File Offset: 0x001ACFAC
		private void UpdateList()
		{
			this.m_availableRecipes.Sort(delegate(Recipe x, Recipe y)
			{
				int minimumAbilityLevel = x.MinimumAbilityLevel;
				int minimumAbilityLevel2 = y.MinimumAbilityLevel;
				return x.DisplayName.CompareTo(y.DisplayName);
			});
			if (this.m_hideUncraftable.isOn)
			{
				this.m_availableRecipes.RemoveAll((Recipe x) => !this.m_canMakeCache[x.Id] || !x.IsHighEnoughLevel(LocalPlayer.GameEntity) || !x.IsAtCorrectCraftingStation(LocalPlayer.GameEntity));
			}
			else
			{
				this.m_availableRecipes.RemoveAll((Recipe x) => !x.IsAtCorrectCraftingStation(LocalPlayer.GameEntity));
			}
			if (this.m_selectedRecipe && !this.m_availableRecipes.Contains(this.m_selectedRecipe))
			{
				this.m_selectedRecipe = null;
				Action<Recipe> recipeSelectionChanged = this.RecipeSelectionChanged;
				if (recipeSelectionChanged != null)
				{
					recipeSelectionChanged(null);
				}
			}
			this.m_listUI.UpdateItems(this.m_availableRecipes);
			this.m_listUI.ReindexItems(this.m_selectedRecipe);
		}

		// Token: 0x06004952 RID: 18770 RVA: 0x001AEE90 File Offset: 0x001AD090
		private void LockUI()
		{
			this.m_craftingLocked = true;
			this.RefreshCraftingButtonInteractibility();
			this.m_downscaleSlider.interactable = false;
			this.m_downscaleLockToMaxToggle.interactable = false;
			this.m_refinementCountInput.interactable = false;
			this.m_decrementCount.interactable = false;
			this.m_incrementCount.interactable = false;
			MasteryTabUI[] array = this.m_masteryTabs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].GetComponent<SolToggle>().interactable = false;
			}
			array = this.m_inverseMasteryTabs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].GetComponent<SolToggle>().interactable = false;
			}
			this.m_listUI.LockUI();
			this.m_componentsListUI.LockUI();
		}

		// Token: 0x06004953 RID: 18771 RVA: 0x001AEF44 File Offset: 0x001AD144
		private void UnlockUI()
		{
			this.m_craftingLocked = false;
			this.RefreshCraftingButtonInteractibility();
			this.m_downscaleSlider.interactable = true;
			this.m_downscaleLockToMaxToggle.interactable = true;
			this.m_refinementCountInput.interactable = true;
			this.m_decrementCount.interactable = true;
			this.m_incrementCount.interactable = true;
			MasteryTabUI[] array = this.m_masteryTabs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].GetComponent<SolToggle>().interactable = true;
			}
			array = this.m_inverseMasteryTabs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].GetComponent<SolToggle>().interactable = true;
			}
			this.m_listUI.UnlockUI();
			this.m_componentsListUI.UnlockUI();
		}

		// Token: 0x06004954 RID: 18772 RVA: 0x001AEFF8 File Offset: 0x001AD1F8
		private void OnRefinementCountInputChanged(string value)
		{
			int amountToCraft;
			if (int.TryParse(value, out amountToCraft))
			{
				this.m_amountToCraft = amountToCraft;
				this.m_refinementCountInput.text = amountToCraft.ToString();
			}
		}

		// Token: 0x06004955 RID: 18773 RVA: 0x00052A89 File Offset: 0x00050C89
		private void OnRefinementCountFocused(string value)
		{
			ClientGameManager.InputManager.SetInputPreventionFlag(InputPreventionFlags.InputField);
		}

		// Token: 0x06004956 RID: 18774 RVA: 0x00052A96 File Offset: 0x00050C96
		private void OnRefinementCountBlurred(string value)
		{
			ClientGameManager.InputManager.UnsetInputPreventionFlag(InputPreventionFlags.InputField);
		}

		// Token: 0x06004957 RID: 18775 RVA: 0x000713AE File Offset: 0x0006F5AE
		private void OnDecrementCountClicked()
		{
			if (this.m_amountToCraft > 1)
			{
				this.m_amountToCraft--;
				this.m_refinementCountInput.text = this.m_amountToCraft.ToString();
			}
		}

		// Token: 0x06004958 RID: 18776 RVA: 0x000713DD File Offset: 0x0006F5DD
		private void OnIncrementCountClicked()
		{
			if (this.m_amountToCraft < 99 && this.m_amountToCraft >= 0)
			{
				this.m_amountToCraft++;
				this.m_refinementCountInput.text = this.m_amountToCraft.ToString();
			}
		}

		// Token: 0x06004959 RID: 18777 RVA: 0x001AF028 File Offset: 0x001AD228
		private void SkillsControllerOnPendingExecutionChanged(SkillsController.PendingExecution obj)
		{
			if (obj.Instance == this.m_executingAbility)
			{
				SkillsController.PendingExecution.PendingStatus status = obj.Status;
				if (status == SkillsController.PendingExecution.PendingStatus.Active)
				{
					this.LockUI();
					return;
				}
				if (status != SkillsController.PendingExecution.PendingStatus.CompleteReceived)
				{
					if (status != SkillsController.PendingExecution.PendingStatus.CancelReceived)
					{
						return;
					}
					this.UnlockUI();
					this.m_executingAbility = null;
					this.m_executingRecipe = null;
					this.m_executingTargetAbilityLevel = 0;
					this.m_executingComponentSelection.Clear();
					this.m_amountToCraft = 1;
					this.m_refinementCountInput.text = this.m_amountToCraft.ToString();
					return;
				}
				else
				{
					this.m_executingAbility = null;
					if (this.m_amountToCraft < 0)
					{
						this.m_shouldTriggerAnother = true;
						return;
					}
					if (this.m_amountToCraft > 0)
					{
						this.m_amountToCraft--;
					}
					if (this.m_amountToCraft == 0)
					{
						this.UnlockUI();
						this.m_amountToCraft = 1;
						this.m_executingRecipe = null;
						this.m_executingTargetAbilityLevel = 0;
						this.m_executingComponentSelection.Clear();
					}
					else
					{
						if (this.m_downscaleLockToMaxToggle.isOn && (int)this.m_downscaleSlider.value != this.m_executingTargetAbilityLevel)
						{
							this.m_executingTargetAbilityLevel = (int)this.m_downscaleSlider.value;
						}
						this.m_shouldTriggerAnother = true;
					}
					this.UpdateCanCraftCounter();
					this.m_refinementCountInput.text = this.m_amountToCraft.ToString();
				}
			}
		}

		// Token: 0x0600495A RID: 18778 RVA: 0x001AF15C File Offset: 0x001AD35C
		private bool ShouldShowDownscaleSlider()
		{
			return this.m_selectedMastery != null && this.m_selectedRecipe != null && this.m_selectedMastery.GetAssociatedLevel(LocalPlayer.GameEntity) >= 10f && this.m_selectedRecipe.IsApplicableOverrideAbilityTiered(this.m_componentsListUI.FindItemsUsedInSelection()) && (int)this.m_downscaleSlider.maxValue - (int)this.m_downscaleSlider.minValue > 0;
		}

		// Token: 0x0600495B RID: 18779 RVA: 0x001AF1CC File Offset: 0x001AD3CC
		private void UpdateDownscaleSliderVisibility()
		{
			if (this.m_componentsPanelOriginalHeight == null)
			{
				this.m_componentsPanelOriginalHeight = new float?(this.m_componentsPanel.rectTransform.rect.height);
			}
			bool flag = this.ShouldShowDownscaleSlider();
			this.m_settingsPanel.gameObject.SetActive(flag);
			this.m_componentsPanel.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, flag ? this.m_componentsPanelOriginalHeight.Value : (this.m_componentsPanelOriginalHeight.Value + 50f));
		}

		// Token: 0x0600495C RID: 18780 RVA: 0x001AF254 File Offset: 0x001AD454
		private void OnDownscaleSliderValueChanged(float value)
		{
			if (this.m_downscaleSlider.value != this.m_downscaleSlider.maxValue)
			{
				this.m_downscaleLockToMaxToggle.isOn = false;
			}
			this.m_downscaleValueText.text = string.Format("{0}", (int)this.m_downscaleSlider.value);
			this.UpdateOutputPreview();
		}

		// Token: 0x0600495D RID: 18781 RVA: 0x00071416 File Offset: 0x0006F616
		private void OnDownscaleLockToMaxToggleChanged(bool value)
		{
			if (value)
			{
				this.m_downscaleSlider.value = this.m_downscaleSlider.maxValue;
			}
		}

		// Token: 0x0600495E RID: 18782 RVA: 0x00071431 File Offset: 0x0006F631
		private void OnCraftClicked()
		{
			this.m_amountToCraft = Math.Max(Math.Min(this.m_amountToCraft, this.AmountCraftable()), 1);
			this.m_refinementCountInput.text = this.m_amountToCraft.ToString();
			this.Craft();
		}

		// Token: 0x0600495F RID: 18783 RVA: 0x0007146C File Offset: 0x0006F66C
		private void OnCraftAllClicked()
		{
			this.m_amountToCraft = this.AmountCraftable();
			this.m_refinementCountInput.text = this.m_amountToCraft.ToString();
			this.Craft();
		}

		// Token: 0x06004960 RID: 18784 RVA: 0x00071496 File Offset: 0x0006F696
		private void OnDeconstructClicked()
		{
			CursorManager.ToggleGameMode(CursorGameMode.Deconstruct);
		}

		// Token: 0x06004961 RID: 18785 RVA: 0x0007149E File Offset: 0x0006F69E
		private void ExitDeconstructMode()
		{
			CursorManager.ExitGameMode(CursorGameMode.Deconstruct);
		}

		// Token: 0x06004962 RID: 18786 RVA: 0x001AF2B4 File Offset: 0x001AD4B4
		private void Craft()
		{
			if (this.m_executingRecipe == null)
			{
				this.m_executingRecipe = this.m_selectedRecipe;
				this.m_executingTargetAbilityLevel = (int)this.m_downscaleSlider.value;
			}
			this.m_executingComponentSelection.Clear();
			this.m_executingComponentSelection.AddRange(this.m_componentsListUI.FindItemsUsedInSelection());
			ArchetypeInstance archetypeInstance;
			if (LocalPlayer.GameEntity.CollectionController.Abilities.TryGetInstanceForArchetypeId(this.m_executingRecipe.Ability.Id, out archetypeInstance))
			{
				this.m_executingAbility = archetypeInstance;
				if (!LocalPlayer.GameEntity.SkillsController.BeginExecution(archetypeInstance))
				{
					this.UnlockUI();
					this.m_executingAbility = null;
					this.m_executingRecipe = null;
					this.m_executingTargetAbilityLevel = 0;
					this.m_executingComponentSelection.Clear();
					this.m_amountToCraft = 1;
					this.m_refinementCountInput.text = this.m_amountToCraft.ToString();
				}
			}
		}

		// Token: 0x06004963 RID: 18787 RVA: 0x001AF394 File Offset: 0x001AD594
		private void UpdateOutputPreview()
		{
			StaticPool<ArchetypeInstance>.ReturnToPool(this.m_outputPreviewInstance);
			this.m_outputPreviewInstance = null;
			this.m_outputPreview.PreviewInstance = null;
			if (this.m_selectedRecipe == null || !this.m_componentsListUI.AllComponentsHaveSelection() || !this.m_canMakeCache[this.m_selectedRecipe.Id])
			{
				return;
			}
			List<ItemUsage> itemsUsed = this.m_componentsListUI.FindItemsUsedInSelection();
			ItemArchetype itemToCreate = this.m_selectedRecipe.GetItemToCreate(itemsUsed, (float)((int)this.m_downscaleSlider.value));
			this.m_outputPreviewInstance = itemToCreate.CreateNewInstance();
			this.m_outputPreviewInstance.ItemData.DeriveComponentData(this.m_outputPreviewInstance, this.m_selectedRecipe, itemsUsed, (float)((int)this.m_downscaleSlider.value));
			this.m_outputPreviewInstance.ItemData.Count = new int?(this.m_selectedRecipe.GetAmountToProduce((float)((int)this.m_downscaleSlider.value), itemsUsed));
			if (!this.m_selectedRecipe.DisableCraftedTag)
			{
				this.m_outputPreviewInstance.AddItemFlag(ItemFlags.Crafted, null);
			}
			this.m_outputPreview.PreviewInstance = this.m_outputPreviewInstance;
		}

		// Token: 0x06004964 RID: 18788 RVA: 0x001AF4AC File Offset: 0x001AD6AC
		private void UpdateCanCraftCounter()
		{
			if (!this.m_componentsListUI.AllComponentsHaveSelection() || !(this.m_selectedRecipe != null))
			{
				this.m_canCraftCounter.ZStringSetText("N/A");
				return;
			}
			if (this.m_canMakeCache.ContainsKey(this.m_selectedRecipe.Id) && this.m_canMakeCache[this.m_selectedRecipe.Id])
			{
				this.m_canCraftCounter.ZStringSetText(this.AmountCraftable().ToString());
				return;
			}
			this.m_canCraftCounter.ZStringSetText("N/A");
		}

		// Token: 0x04004439 RID: 17465
		[SerializeField]
		private TextMeshProUGUI m_windowLabel;

		// Token: 0x0400443A RID: 17466
		[SerializeField]
		private SolToggle m_hideUncraftable;

		// Token: 0x0400443B RID: 17467
		[SerializeField]
		private RecipesList m_listUI;

		// Token: 0x0400443C RID: 17468
		[SerializeField]
		private GameObject m_blockerPanel;

		// Token: 0x0400443D RID: 17469
		[SerializeField]
		private TextTooltipTrigger m_craftingButtonTooltipRegion;

		// Token: 0x0400443E RID: 17470
		[SerializeField]
		private TextTooltipTrigger m_deconstructButtonTooltipRegion;

		// Token: 0x0400443F RID: 17471
		[SerializeField]
		private Image m_recipeIcon;

		// Token: 0x04004440 RID: 17472
		[SerializeField]
		private Image m_recipeLowLevelIndicator;

		// Token: 0x04004441 RID: 17473
		[SerializeField]
		private TextMeshProUGUI m_recipeNameLabel;

		// Token: 0x04004442 RID: 17474
		[SerializeField]
		private TextMeshProUGUI m_recipeAbilityLabel;

		// Token: 0x04004443 RID: 17475
		[SerializeField]
		private TextMeshProUGUI m_recipeDescriptionLabel;

		// Token: 0x04004444 RID: 17476
		[SerializeField]
		private Image m_componentsPanel;

		// Token: 0x04004445 RID: 17477
		[SerializeField]
		private ComponentsList m_componentsListUI;

		// Token: 0x04004446 RID: 17478
		[SerializeField]
		private Image m_dropdownContainer;

		// Token: 0x04004447 RID: 17479
		[SerializeField]
		private Image m_settingsPanel;

		// Token: 0x04004448 RID: 17480
		[SerializeField]
		private Slider m_downscaleSlider;

		// Token: 0x04004449 RID: 17481
		[SerializeField]
		private SolToggle m_downscaleLockToMaxToggle;

		// Token: 0x0400444A RID: 17482
		[SerializeField]
		private TextMeshProUGUI m_downscaleValueText;

		// Token: 0x0400444B RID: 17483
		[SerializeField]
		private TextMeshProUGUI m_canCraftCounter;

		// Token: 0x0400444C RID: 17484
		[SerializeField]
		private SolTMP_InputField m_refinementCountInput;

		// Token: 0x0400444D RID: 17485
		[SerializeField]
		private SolButton m_decrementCount;

		// Token: 0x0400444E RID: 17486
		[SerializeField]
		private SolButton m_incrementCount;

		// Token: 0x0400444F RID: 17487
		[SerializeField]
		private SolButton m_craftButton;

		// Token: 0x04004450 RID: 17488
		[SerializeField]
		private SolButton m_craftAllButton;

		// Token: 0x04004451 RID: 17489
		[SerializeField]
		private SolButton m_deconstructButton;

		// Token: 0x04004452 RID: 17490
		[SerializeField]
		private MasteryTabUI[] m_masteryTabs;

		// Token: 0x04004453 RID: 17491
		[SerializeField]
		private MasteryTabUI[] m_inverseMasteryTabs;

		// Token: 0x04004454 RID: 17492
		[SerializeField]
		private TabController m_tabController;

		// Token: 0x04004455 RID: 17493
		[SerializeField]
		private Image m_initialTierMarker;

		// Token: 0x04004456 RID: 17494
		[SerializeField]
		private OutputPreview m_outputPreview;

		// Token: 0x04004457 RID: 17495
		private bool m_craftingLocked;

		// Token: 0x04004458 RID: 17496
		private Recipe m_selectedRecipe;

		// Token: 0x04004459 RID: 17497
		private List<Recipe> m_availableRecipes = new List<Recipe>();

		// Token: 0x0400445A RID: 17498
		private Dictionary<UniqueId, bool> m_canMakeCache = new Dictionary<UniqueId, bool>();

		// Token: 0x0400445B RID: 17499
		private ArchetypeInstance m_selectedMastery;

		// Token: 0x0400445C RID: 17500
		private bool m_ignoreTabChangedEvent;

		// Token: 0x0400445D RID: 17501
		private ArchetypeInstance m_executingAbility;

		// Token: 0x0400445E RID: 17502
		private Recipe m_executingRecipe;

		// Token: 0x0400445F RID: 17503
		private int m_executingTargetAbilityLevel;

		// Token: 0x04004460 RID: 17504
		private List<ItemUsage> m_executingComponentSelection = new List<ItemUsage>();

		// Token: 0x04004461 RID: 17505
		private int m_amountToCraft;

		// Token: 0x04004462 RID: 17506
		private bool m_shouldTriggerAnother;

		// Token: 0x04004464 RID: 17508
		private List<Image> m_tierMarkers = new List<Image>();

		// Token: 0x04004465 RID: 17509
		private ArchetypeInstance m_outputPreviewInstance;

		// Token: 0x04004466 RID: 17510
		private const string kMsgNotAtStation = "Visit a crafting station to make items.";

		// Token: 0x04004467 RID: 17511
		private const string kMsgNoRoom = "At least one space must be available in your inventory.";

		// Token: 0x04004468 RID: 17512
		private const string kMsgSelectionNeeded = "You must select an item for all required components.";

		// Token: 0x04004469 RID: 17513
		private const string kMsgSelectionInvalid = "Your currect selection is not craftable.";

		// Token: 0x0400446A RID: 17514
		private const string kMsgWrongStance = "You must be standing without a torch.";

		// Token: 0x0400446B RID: 17515
		private const string kMsgTooLowLevel = "Your {0} ability must be level {1} to craft this item.";

		// Token: 0x0400446C RID: 17516
		private const string kMsgWrongStationType = "This recipe cannot be crafted at this type of station.";

		// Token: 0x0400446D RID: 17517
		private Dictionary<int, int> m_tempAmounts = new Dictionary<int, int>();

		// Token: 0x0400446E RID: 17518
		private Dictionary<int, int> m_tempAmountsUsed = new Dictionary<int, int>();

		// Token: 0x0400446F RID: 17519
		private float? m_componentsPanelOriginalHeight;
	}
}
