using System;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using SoL.Game.UI.Skills;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.UI.Recipes
{
	// Token: 0x02000938 RID: 2360
	public class RecipesUI : DraggableUIWindow
	{
		// Token: 0x140000CE RID: 206
		// (add) Token: 0x0600459A RID: 17818 RVA: 0x001A087C File Offset: 0x0019EA7C
		// (remove) Token: 0x0600459B RID: 17819 RVA: 0x001A08B4 File Offset: 0x0019EAB4
		public event Action<ArchetypeInstance> MasterySelectionChanged;

		// Token: 0x140000CF RID: 207
		// (add) Token: 0x0600459C RID: 17820 RVA: 0x001A08EC File Offset: 0x0019EAEC
		// (remove) Token: 0x0600459D RID: 17821 RVA: 0x001A0924 File Offset: 0x0019EB24
		public event Action<Recipe> RecipeSelectionChanged;

		// Token: 0x0600459E RID: 17822 RVA: 0x001A095C File Offset: 0x0019EB5C
		protected override void Start()
		{
			base.Start();
			this.m_listUI.RecipeSelected += this.RecipeListOnRecipeSelected;
			this.m_detailUI.SubscribeToEvents(this);
			this.m_tabController.TabChanged += this.TabControllerOnTabChanged;
			this.MasterySelectionChanged += this.OnMasterySelectionChanged;
			this.RecipeSelectionChanged += this.OnRecipeSelectionChanged;
			LocalPlayer.LocalPlayerInitialized += this.LocalPlayerOnLocalPlayerInitialized;
			LocalPlayer.GameEntity.CollectionController.Masteries.ContentsChanged += this.CollectionControllerOnMasteriesChanged;
			LocalPlayer.GameEntity.CollectionController.Recipes.ContentsChanged += this.CollectionControllerOnRecipesChanged;
			this.RefreshAvailableMasteries();
		}

		// Token: 0x0600459F RID: 17823 RVA: 0x001A0A24 File Offset: 0x0019EC24
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.m_listUI.RecipeSelected -= this.RecipeListOnRecipeSelected;
			this.m_detailUI.UnsubscribeFromEvents(this);
			this.m_tabController.TabChanged -= this.TabControllerOnTabChanged;
			this.MasterySelectionChanged -= this.OnMasterySelectionChanged;
			this.RecipeSelectionChanged -= this.OnRecipeSelectionChanged;
			LocalPlayer.LocalPlayerInitialized -= this.LocalPlayerOnLocalPlayerInitialized;
			LocalPlayer.GameEntity.CollectionController.Masteries.ContentsChanged -= this.CollectionControllerOnMasteriesChanged;
			LocalPlayer.GameEntity.CollectionController.Recipes.ContentsChanged -= this.CollectionControllerOnRecipesChanged;
		}

		// Token: 0x060045A0 RID: 17824 RVA: 0x0006ED83 File Offset: 0x0006CF83
		private void TabControllerOnTabChanged()
		{
			if (!this.m_ignoreTabChangedEvent)
			{
				ArchetypeInstance selectedMastery = this.m_selectedMastery;
				this.m_selectedMastery = this.GetActiveMasteryInstance();
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

		// Token: 0x060045A1 RID: 17825 RVA: 0x001A0AE8 File Offset: 0x0019ECE8
		private void RecipeListOnRecipeSelected(Recipe newSelection)
		{
			Recipe selectedRecipe = this.m_selectedRecipe;
			UniqueId? uniqueId = (selectedRecipe != null) ? new UniqueId?(selectedRecipe.Id) : null;
			if (uniqueId != ((newSelection != null) ? new UniqueId?(newSelection.Id) : null))
			{
				this.m_selectedRecipe = newSelection;
				Action<Recipe> recipeSelectionChanged = this.RecipeSelectionChanged;
				if (recipeSelectionChanged == null)
				{
					return;
				}
				recipeSelectionChanged(newSelection);
			}
		}

		// Token: 0x060045A2 RID: 17826 RVA: 0x0006EDBD File Offset: 0x0006CFBD
		private void OnMasterySelectionChanged(ArchetypeInstance newSelection)
		{
			this.RefreshAvailableRecipes();
		}

		// Token: 0x060045A3 RID: 17827 RVA: 0x001A0B7C File Offset: 0x0019ED7C
		private void OnRecipeSelectionChanged(Recipe newSelection)
		{
			if (this.m_thisRect == null)
			{
				this.m_thisRect = base.gameObject.GetComponent<RectTransform>();
			}
			if (this.m_listRect == null)
			{
				this.m_listRect = this.m_listUI.gameObject.GetComponent<RectTransform>();
				this.m_originalListViewHeight = this.m_listRect.rect.height;
			}
			if (newSelection != null)
			{
				this.m_listRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.m_originalListViewHeight);
				this.m_detailUI.gameObject.SetActive(true);
				return;
			}
			this.m_listRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.m_thisRect.rect.height - 70f);
			this.m_detailUI.gameObject.SetActive(false);
		}

		// Token: 0x060045A4 RID: 17828 RVA: 0x0006EDC5 File Offset: 0x0006CFC5
		private void LocalPlayerOnLocalPlayerInitialized()
		{
			this.RefreshAvailableMasteries();
			this.RefreshAvailableRecipes();
		}

		// Token: 0x060045A5 RID: 17829 RVA: 0x0006EDD3 File Offset: 0x0006CFD3
		private void CollectionControllerOnMasteriesChanged()
		{
			this.RefreshAvailableMasteries();
		}

		// Token: 0x060045A6 RID: 17830 RVA: 0x0006EDBD File Offset: 0x0006CFBD
		private void CollectionControllerOnRecipesChanged()
		{
			this.RefreshAvailableRecipes();
		}

		// Token: 0x060045A7 RID: 17831 RVA: 0x001A0C48 File Offset: 0x0019EE48
		private ArchetypeInstance GetActiveMasteryInstance()
		{
			for (int i = 0; i < this.m_masteryTabs.Length; i++)
			{
				if (this.m_masteryTabs[i].Instance != null && this.m_masteryTabs[i].ToggleIsOn)
				{
					return this.m_masteryTabs[i].Instance;
				}
			}
			return null;
		}

		// Token: 0x060045A8 RID: 17832 RVA: 0x001A0C98 File Offset: 0x0019EE98
		private void RefreshAvailableMasteries()
		{
			this.m_ignoreTabChangedEvent = true;
			ArchetypeInstance activeMasteryInstance = this.GetActiveMasteryInstance();
			MasteryTabUI masteryTabUI = this.m_masteryTabs[0];
			int num = 0;
			foreach (ArchetypeInstance archetypeInstance in LocalPlayer.GameEntity.CollectionController.Masteries.Instances)
			{
				MasteryArchetype masteryArchetype;
				if (archetypeInstance.Archetype.TryGetAsType(out masteryArchetype) && (masteryArchetype.Type == MasteryType.Harvesting || masteryArchetype.Type == MasteryType.Trade))
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
			}
			for (int i = num; i < this.m_masteryTabs.Length; i++)
			{
				this.m_masteryTabs[i].Instance = null;
				this.m_masteryTabs[i].ToggleIsOn = false;
			}
			if (masteryTabUI.Instance != null)
			{
				UniqueId instanceId = masteryTabUI.Instance.InstanceId;
				ArchetypeInstance selectedMastery = this.m_selectedMastery;
				if (!(instanceId != ((selectedMastery != null) ? new UniqueId?(selectedMastery.InstanceId) : null)))
				{
					goto IL_14C;
				}
			}
			this.m_selectedMastery = masteryTabUI.Instance;
			Action<ArchetypeInstance> masterySelectionChanged = this.MasterySelectionChanged;
			if (masterySelectionChanged != null)
			{
				masterySelectionChanged(this.m_selectedMastery);
			}
			IL_14C:
			if (masteryTabUI.Instance != null)
			{
				masteryTabUI.ToggleIsOn = true;
			}
			this.m_ignoreTabChangedEvent = false;
		}

		// Token: 0x060045A9 RID: 17833 RVA: 0x001A0E18 File Offset: 0x0019F018
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
			if (!flag)
			{
				this.m_selectedRecipe = null;
				Action<Recipe> recipeSelectionChanged = this.RecipeSelectionChanged;
				if (recipeSelectionChanged != null)
				{
					recipeSelectionChanged(this.m_selectedRecipe);
				}
			}
			this.m_listUI.UpdateRecipeList(this.m_availableRecipes, this.m_selectedRecipe);
		}

		// Token: 0x040041F4 RID: 16884
		[SerializeField]
		private RecipeListUI m_listUI;

		// Token: 0x040041F5 RID: 16885
		[SerializeField]
		private RecipeDetailUI m_detailUI;

		// Token: 0x040041F6 RID: 16886
		[SerializeField]
		private MasteryTabUI[] m_masteryTabs;

		// Token: 0x040041F7 RID: 16887
		[SerializeField]
		private TabController m_tabController;

		// Token: 0x040041F8 RID: 16888
		private Recipe m_selectedRecipe;

		// Token: 0x040041F9 RID: 16889
		private List<Recipe> m_availableRecipes = new List<Recipe>();

		// Token: 0x040041FA RID: 16890
		private ArchetypeInstance m_selectedMastery;

		// Token: 0x040041FB RID: 16891
		private bool m_ignoreTabChangedEvent;

		// Token: 0x040041FC RID: 16892
		private float m_originalListViewHeight;

		// Token: 0x040041FD RID: 16893
		private RectTransform m_thisRect;

		// Token: 0x040041FE RID: 16894
		private RectTransform m_listRect;
	}
}
