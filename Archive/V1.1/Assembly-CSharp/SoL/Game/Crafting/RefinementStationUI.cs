using System;
using System.Collections.Generic;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.UI;
using SoL.UI;
using SoL.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.Game.Crafting
{
	// Token: 0x02000CEE RID: 3310
	public class RefinementStationUI : DraggableUIWindow
	{
		// Token: 0x1700180D RID: 6157
		// (get) Token: 0x06006442 RID: 25666 RVA: 0x0008377E File Offset: 0x0008197E
		public UniversalContainerUI Input
		{
			get
			{
				return this.m_input;
			}
		}

		// Token: 0x1700180E RID: 6158
		// (get) Token: 0x06006443 RID: 25667 RVA: 0x00083786 File Offset: 0x00081986
		public UniversalContainerUI Output
		{
			get
			{
				return this.m_output;
			}
		}

		// Token: 0x1700180F RID: 6159
		// (get) Token: 0x06006444 RID: 25668 RVA: 0x0008378E File Offset: 0x0008198E
		// (set) Token: 0x06006445 RID: 25669 RVA: 0x00083796 File Offset: 0x00081996
		public InteractiveRefinementStation RefinementStation { get; set; }

		// Token: 0x06006446 RID: 25670 RVA: 0x00209500 File Offset: 0x00207700
		protected override void Awake()
		{
			base.Awake();
			this.m_refineButton.onClick.AddListener(new UnityAction(this.RefineClicked));
			this.m_cancelButton.onClick.AddListener(new UnityAction(this.CancelClicked));
			this.m_cancelButton.interactable = false;
			this.m_refineAllToggle.isOn = false;
		}

		// Token: 0x06006447 RID: 25671 RVA: 0x00209564 File Offset: 0x00207764
		protected override void Start()
		{
			base.Start();
			LocalPlayer.GameEntity.SkillsController.PendingExecutionChanged += this.SkillsControllerOnPendingExecutionChanged;
			LocalPlayer.GameEntity.CollectionController.Masteries.ContentsChanged += this.MasteriesOnContentsChanged;
			LocalPlayer.GameEntity.CollectionController.Recipes.ContentsChanged += this.RecipesOnContentsChanged;
			if (LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.RefinementInput, out this.m_inputContainerInstance))
			{
				this.m_inputContainerInstance.ContentsChanged += this.InputContainerInstanceOnContentsChanged;
			}
			this.InputContainerInstanceOnContentsChanged();
			LocalClientSkillsController.MasteryLevelChangedEvent += this.LocalClientSkillsControllerOnMasteryLevelChangedEvent;
			LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.RefinementOutput, out this.m_outputContainerInstance);
		}

		// Token: 0x06006448 RID: 25672 RVA: 0x00209634 File Offset: 0x00207834
		protected override void OnDestroy()
		{
			this.m_refineButton.onClick.RemoveListener(new UnityAction(this.RefineClicked));
			this.m_cancelButton.onClick.RemoveListener(new UnityAction(this.CancelClicked));
			LocalPlayer.GameEntity.SkillsController.PendingExecutionChanged -= this.SkillsControllerOnPendingExecutionChanged;
			LocalPlayer.GameEntity.CollectionController.Masteries.ContentsChanged -= this.MasteriesOnContentsChanged;
			LocalPlayer.GameEntity.CollectionController.Recipes.ContentsChanged -= this.RecipesOnContentsChanged;
			if (this.m_inputContainerInstance != null)
			{
				this.m_inputContainerInstance.ContentsChanged -= this.InputContainerInstanceOnContentsChanged;
			}
			LocalClientSkillsController.MasteryLevelChangedEvent -= this.LocalClientSkillsControllerOnMasteryLevelChangedEvent;
			base.OnDestroy();
		}

		// Token: 0x06006449 RID: 25673 RVA: 0x0020970C File Offset: 0x0020790C
		private void Update()
		{
			if (!base.Visible)
			{
				return;
			}
			if (this.m_shouldTriggerAnother)
			{
				this.m_shouldTriggerAnother = false;
				this.RefineClicked();
			}
			if (!LocalPlayer.GameEntity.SkillsController.PendingIsActive)
			{
				this.m_progressImage.fillAmount = 0f;
				this.m_progressText.text = null;
				return;
			}
			SkillsController.PendingExecution pending = LocalPlayer.GameEntity.SkillsController.Pending;
			float num = (pending.ExecutionTime - pending.ExecutionTimeRemaining) / pending.ExecutionTime;
			this.m_progressImage.fillAmount = 1f - num;
			this.m_progressText.SetFormattedTime(pending.ExecutionTimeRemaining, true);
		}

		// Token: 0x0600644A RID: 25674 RVA: 0x0008379F File Offset: 0x0008199F
		public override void Hide(bool skipTransition = false)
		{
			this.CancelClicked();
			base.Hide(skipTransition);
		}

		// Token: 0x0600644B RID: 25675 RVA: 0x000837AE File Offset: 0x000819AE
		public Recipe GetActiveRecipe()
		{
			return this.m_recipeListUi.GetActiveRecipe();
		}

		// Token: 0x0600644C RID: 25676 RVA: 0x002097B0 File Offset: 0x002079B0
		private void SkillsControllerOnPendingExecutionChanged(SkillsController.PendingExecution obj)
		{
			if (obj.Active && obj.Instance == this.m_archetypeInstance)
			{
				this.m_refineButton.interactable = false;
				this.ToggleLocks(true);
				this.m_cancelButton.interactable = true;
				return;
			}
			this.m_refineButton.interactable = true;
			this.m_archetypeInstance = null;
			this.ToggleLocks(false);
			this.m_cancelButton.interactable = false;
			if (obj.Status == SkillsController.PendingExecution.PendingStatus.CompleteReceived && this.m_refineAllToggle.isOn)
			{
				this.m_shouldTriggerAnother = true;
			}
		}

		// Token: 0x0600644D RID: 25677 RVA: 0x000837BB File Offset: 0x000819BB
		private void LocalClientSkillsControllerOnMasteryLevelChangedEvent(ArchetypeInstance obj)
		{
			this.UpdateRecipeList(false);
		}

		// Token: 0x0600644E RID: 25678 RVA: 0x00209838 File Offset: 0x00207A38
		private void CancelClicked()
		{
			if (this.m_archetypeInstance != null && LocalPlayer.GameEntity.SkillsController.PendingIsActive && LocalPlayer.GameEntity.SkillsController.Pending.Instance == this.m_archetypeInstance)
			{
				LocalPlayer.GameEntity.SkillsController.EscapePressed();
			}
		}

		// Token: 0x0600644F RID: 25679 RVA: 0x0020988C File Offset: 0x00207A8C
		private void RefineClicked()
		{
			this.m_archetypeInstance = null;
			Recipe activeRecipe = this.GetActiveRecipe();
			if (activeRecipe == null || activeRecipe.DefaultItemToCreate == null)
			{
				return;
			}
			LocalPlayer.GameEntity.CollectionController.Abilities.TryGetInstanceForArchetypeId(activeRecipe.Ability.Id, out this.m_archetypeInstance);
			if (this.m_archetypeInstance != null && LocalPlayer.GameEntity.SkillsController.BeginExecution(this.m_archetypeInstance))
			{
				this.ToggleLocks(true);
			}
		}

		// Token: 0x06006450 RID: 25680 RVA: 0x000837C4 File Offset: 0x000819C4
		private void InputContainerInstanceOnContentsChanged()
		{
			this.UpdateRecipeList(true);
		}

		// Token: 0x06006451 RID: 25681 RVA: 0x000837BB File Offset: 0x000819BB
		private void MasteriesOnContentsChanged()
		{
			this.UpdateRecipeList(false);
		}

		// Token: 0x06006452 RID: 25682 RVA: 0x000837BB File Offset: 0x000819BB
		private void RecipesOnContentsChanged()
		{
			this.UpdateRecipeList(false);
		}

		// Token: 0x06006453 RID: 25683 RVA: 0x0020990C File Offset: 0x00207B0C
		private void ToggleLocks(bool locked)
		{
			if (locked)
			{
				if (this.m_inputContainerInstance != null)
				{
					this.m_inputContainerInstance.LockFlags |= ContainerLockFlags.UI;
				}
				if (this.m_outputContainerInstance != null)
				{
					this.m_outputContainerInstance.LockFlags |= ContainerLockFlags.UI;
				}
			}
			else
			{
				if (this.m_inputContainerInstance != null)
				{
					this.m_inputContainerInstance.LockFlags &= ~ContainerLockFlags.UI;
				}
				if (this.m_outputContainerInstance != null)
				{
					this.m_outputContainerInstance.LockFlags &= ~ContainerLockFlags.UI;
				}
			}
			this.m_recipeListUi.ToggleLock(locked);
		}

		// Token: 0x06006454 RID: 25684 RVA: 0x000837CD File Offset: 0x000819CD
		public bool RecipesKnown()
		{
			return this.m_knownRecipeList.Count > 0;
		}

		// Token: 0x06006455 RID: 25685 RVA: 0x00209998 File Offset: 0x00207B98
		public void UpdateRecipeList(bool matchedOnly = false)
		{
			if (this.RefinementStation == null)
			{
				return;
			}
			ContainerInstance containerInstance;
			if (!LocalPlayer.GameEntity.CollectionController.TryGetInstance(ContainerType.RefinementInput, out containerInstance))
			{
				return;
			}
			IEnumerable<ArchetypeInstance> instances = containerInstance.Instances;
			IEnumerable<LearnableArchetype> learnables = LocalPlayer.GameEntity.CollectionController.Recipes.Learnables;
			if (matchedOnly)
			{
				this.RefreshMatchedRecipesOnly(instances, learnables);
			}
			else
			{
				this.RefreshRecipes(instances, learnables);
			}
			this.m_recipeListUi.UpdateRecipes(this.m_matchedRecipeList);
		}

		// Token: 0x06006456 RID: 25686 RVA: 0x00209A0C File Offset: 0x00207C0C
		private void RefreshRecipes(IEnumerable<ArchetypeInstance> inputItems, IEnumerable<LearnableArchetype> availableRecipes)
		{
			this.m_knownRecipeList.Clear();
			this.m_matchedRecipeList.Clear();
			using (IEnumerator<LearnableArchetype> enumerator = availableRecipes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Recipe recipe;
					ArchetypeInstance archetypeInstance;
					if (enumerator.Current.TryGetAsType(out recipe) && (this.RefinementStation.Profile == null || this.RefinementStation.Profile.Category == recipe.StationCategory) && LocalPlayer.GameEntity.CollectionController.Masteries.TryGetInstanceForArchetypeId(recipe.Mastery.Id, out archetypeInstance))
					{
						this.m_knownRecipeList.Add(recipe);
						List<ItemUsage> list;
						RecipeComponent recipeComponent;
						if (recipe.Match(inputItems, out list, out recipeComponent))
						{
							this.m_matchedRecipeList.Add(recipe);
						}
					}
				}
			}
		}

		// Token: 0x06006457 RID: 25687 RVA: 0x00209AE4 File Offset: 0x00207CE4
		private void RefreshMatchedRecipesOnly(IEnumerable<ArchetypeInstance> inputItems, IEnumerable<LearnableArchetype> availableRecipes)
		{
			this.m_matchedRecipeList.Clear();
			using (IEnumerator<LearnableArchetype> enumerator = availableRecipes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Recipe recipe;
					ArchetypeInstance archetypeInstance;
					List<ItemUsage> list;
					RecipeComponent recipeComponent;
					if (enumerator.Current.TryGetAsType(out recipe) && (this.RefinementStation.Profile == null || this.RefinementStation.Profile.Category == recipe.StationCategory) && LocalPlayer.GameEntity.CollectionController.Masteries.TryGetInstanceForArchetypeId(recipe.Mastery.Id, out archetypeInstance) && recipe.Match(inputItems, out list, out recipeComponent))
					{
						this.m_matchedRecipeList.Add(recipe);
					}
				}
			}
		}

		// Token: 0x0400570D RID: 22285
		[SerializeField]
		private UniversalContainerUI m_input;

		// Token: 0x0400570E RID: 22286
		[SerializeField]
		private UniversalContainerUI m_output;

		// Token: 0x0400570F RID: 22287
		[SerializeField]
		private SolButton m_refineButton;

		// Token: 0x04005710 RID: 22288
		[SerializeField]
		private SolButton m_cancelButton;

		// Token: 0x04005711 RID: 22289
		[SerializeField]
		private RecipeListUI m_recipeListUi;

		// Token: 0x04005712 RID: 22290
		[SerializeField]
		private SolToggle m_refineAllToggle;

		// Token: 0x04005713 RID: 22291
		[SerializeField]
		private Image m_progressImage;

		// Token: 0x04005714 RID: 22292
		[SerializeField]
		private TextMeshProUGUI m_progressText;

		// Token: 0x04005715 RID: 22293
		private ArchetypeInstance m_archetypeInstance;

		// Token: 0x04005716 RID: 22294
		private ContainerInstance m_inputContainerInstance;

		// Token: 0x04005717 RID: 22295
		private ContainerInstance m_outputContainerInstance;

		// Token: 0x04005718 RID: 22296
		private bool m_shouldTriggerAnother;

		// Token: 0x0400571A RID: 22298
		private readonly List<Recipe> m_knownRecipeList = new List<Recipe>();

		// Token: 0x0400571B RID: 22299
		private readonly List<Recipe> m_matchedRecipeList = new List<Recipe>();
	}
}
