using System;
using SoL.Game.Messages;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A93 RID: 2707
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Recipe Item")]
	public class RecipeItem : ConsumableItem
	{
		// Token: 0x17001340 RID: 4928
		// (get) Token: 0x060053ED RID: 21485 RVA: 0x000781BA File Offset: 0x000763BA
		public override string DisplayName
		{
			get
			{
				if (!string.IsNullOrEmpty(base.DisplayName))
				{
					return base.DisplayName;
				}
				if (!(this.m_recipe != null))
				{
					return "Recipe: ???";
				}
				return "Recipe: " + this.m_recipe.DisplayName;
			}
		}

		// Token: 0x17001341 RID: 4929
		// (get) Token: 0x060053EE RID: 21486 RVA: 0x000781F9 File Offset: 0x000763F9
		public Recipe Recipe
		{
			get
			{
				return this.m_recipe;
			}
		}

		// Token: 0x060053EF RID: 21487 RVA: 0x001D9408 File Offset: 0x001D7608
		public override bool MatchesTextFilter(string filter)
		{
			if (this.Recipe)
			{
				if (this.Recipe.Mastery && this.Recipe.Mastery.DisplayName.Contains(filter, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
				if (this.Recipe.Ability && this.Recipe.Ability.DisplayName.Contains(filter, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return base.MatchesTextFilter(filter);
		}

		// Token: 0x17001342 RID: 4930
		// (get) Token: 0x060053F0 RID: 21488 RVA: 0x00045BCA File Offset: 0x00043DCA
		public override bool CanPlaceInPouch
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001343 RID: 4931
		// (get) Token: 0x060053F1 RID: 21489 RVA: 0x00062532 File Offset: 0x00060732
		protected override ReductionTaskType m_reductionTaskType
		{
			get
			{
				return ReductionTaskType.Consume;
			}
		}

		// Token: 0x17001344 RID: 4932
		// (get) Token: 0x060053F2 RID: 21490 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool IsLearning
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060053F3 RID: 21491 RVA: 0x001D9484 File Offset: 0x001D7684
		protected override bool ExecutionCheckInternal(ExecutionCache executionCache)
		{
			ArchetypeInstance archetypeInstance;
			ArchetypeInstance archetypeInstance2;
			if (!executionCache.SourceEntity.CollectionController.Masteries.TryGetInstanceForArchetypeId(this.m_recipe.Mastery.Id, out archetypeInstance) || !executionCache.SourceEntity.CollectionController.Abilities.TryGetInstanceForArchetypeId(this.m_recipe.Ability.Id, out archetypeInstance2))
			{
				executionCache.Message = "You can't make heads or tails of this thing!";
				executionCache.MsgType = MessageType.Skills;
				return false;
			}
			if (archetypeInstance2.GetAssociatedLevelInteger(executionCache.SourceEntity) < this.m_recipe.MinimumAbilityLevel)
			{
				executionCache.Message = "It looks like this is relevant to your " + this.m_recipe.Mastery.DisplayName.ToLower() + " expertise, but you're not quite ready yet.";
				executionCache.MsgType = MessageType.Skills;
				return false;
			}
			for (int i = 0; i < executionCache.SourceEntity.CollectionController.Recipes.Count; i++)
			{
				if (executionCache.SourceEntity.CollectionController.Recipes.GetIndex(i).Id == this.m_recipe.Id)
				{
					executionCache.Message = "You already know this.";
					executionCache.MsgType = MessageType.Skills;
					return false;
				}
			}
			return true;
		}

		// Token: 0x060053F4 RID: 21492 RVA: 0x00078201 File Offset: 0x00076401
		protected override void PostExecution(ExecutionCache executionCache)
		{
			base.PostExecution(executionCache);
			executionCache.SourceEntity.CollectionController.Recipes.Add(this.m_recipe, true);
		}

		// Token: 0x060053F5 RID: 21493 RVA: 0x001D95A8 File Offset: 0x001D77A8
		public bool CanUseAuctionHouse(GameEntity entity)
		{
			ArchetypeInstance archetypeInstance;
			ArchetypeInstance archetypeInstance2;
			LearnableArchetype learnableArchetype;
			return this.m_recipe && this.m_recipe.Mastery && this.m_recipe.Ability && entity && entity.CollectionController != null && entity.CollectionController.Masteries != null && entity.CollectionController.Abilities != null && entity.CollectionController.Recipes != null && entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(this.m_recipe.Mastery.Id, out archetypeInstance) && (!this.m_recipe.Ability.Specialization || (archetypeInstance.MasteryData != null && archetypeInstance.MasteryData.Specialization != null && !(archetypeInstance.MasteryData.Specialization.Value != this.m_recipe.Ability.Specialization.Id))) && entity.CollectionController.Abilities.TryGetInstanceForArchetypeId(this.m_recipe.Ability.Id, out archetypeInstance2) && !entity.CollectionController.Recipes.TryGetLearnableForId(this.m_recipe.Id, out learnableArchetype);
		}

		// Token: 0x060053F6 RID: 21494 RVA: 0x001D96F4 File Offset: 0x001D78F4
		public override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			base.FillTooltipBlocks(tooltip, instance, entity);
			if (!entity || entity.CollectionController == null || entity.CollectionController.Masteries == null || entity.CollectionController.Abilities == null || entity.CollectionController.Recipes == null)
			{
				return;
			}
			TooltipTextBlock requirementsBlock = tooltip.RequirementsBlock;
			ArchetypeInstance archetypeInstance;
			Color color = entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(this.m_recipe.Mastery.Id, out archetypeInstance) ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor;
			requirementsBlock.AppendLine("Required Profession:", this.m_recipe.Mastery.DisplayName.Color(color));
			bool flag = archetypeInstance != null;
			if (this.m_recipe.Ability.Specialization != null)
			{
				bool flag2 = archetypeInstance != null && archetypeInstance.MasteryData != null && archetypeInstance.MasteryData.Specialization != null && archetypeInstance.MasteryData.Specialization.Value == this.m_recipe.Ability.Specialization.Id;
				color = (flag2 ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor);
				requirementsBlock.AppendLine("Required Specialization:", this.m_recipe.Ability.Specialization.DisplayName.Color(color));
				flag = (flag && flag2);
			}
			ArchetypeInstance archetypeInstance2;
			color = ((entity.CollectionController.Abilities.TryGetInstanceForArchetypeId(this.m_recipe.Ability.Id, out archetypeInstance2) && archetypeInstance2.GetAssociatedLevelInteger(entity) >= this.m_recipe.MinimumAbilityLevel) ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor);
			requirementsBlock.AppendLine("Required Ability:", (this.m_recipe.Ability.DisplayName + " (" + this.m_recipe.MinimumAbilityLevel.ToString() + ")").Color(color));
			LearnableArchetype learnableArchetype;
			if (entity.CollectionController.Recipes.TryGetLearnableForId(this.m_recipe.Id, out learnableArchetype))
			{
				if (flag)
				{
					requirementsBlock.AppendLine("", 0);
					requirementsBlock.AppendLine("Already known".Color(UIManager.RequirementsNotMetColor), 0);
					return;
				}
				requirementsBlock.AppendLine("", 0);
				requirementsBlock.AppendLine("Previously known".Color(UIManager.RequirementsNotMetColor), 0);
			}
		}

		// Token: 0x04004AC0 RID: 19136
		[SerializeField]
		private Recipe m_recipe;
	}
}
