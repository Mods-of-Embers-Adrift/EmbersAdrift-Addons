using System;
using SoL.Game.Messages;
using SoL.Game.Objects.Containers;
using SoL.Game.Transactions;
using SoL.Managers;
using SoL.Networking;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A44 RID: 2628
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Ability Recipe Item")]
	public class AbilityRecipeItem : ConsumableItem
	{
		// Token: 0x17001229 RID: 4649
		// (get) Token: 0x06005166 RID: 20838 RVA: 0x0007658D File Offset: 0x0007478D
		public override string DisplayName
		{
			get
			{
				if (!string.IsNullOrEmpty(base.DisplayName))
				{
					return base.DisplayName;
				}
				if (!(this.m_ability != null))
				{
					return "Ability: ???";
				}
				return "Ability: " + this.m_ability.DisplayName;
			}
		}

		// Token: 0x1700122A RID: 4650
		// (get) Token: 0x06005167 RID: 20839 RVA: 0x00045BCA File Offset: 0x00043DCA
		public override bool CanPlaceInPouch
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700122B RID: 4651
		// (get) Token: 0x06005168 RID: 20840 RVA: 0x00062532 File Offset: 0x00060732
		protected override ReductionTaskType m_reductionTaskType
		{
			get
			{
				return ReductionTaskType.Consume;
			}
		}

		// Token: 0x1700122C RID: 4652
		// (get) Token: 0x06005169 RID: 20841 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool IsLearning
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600516A RID: 20842 RVA: 0x001D0434 File Offset: 0x001CE634
		protected override bool ExecutionCheckInternal(ExecutionCache executionCache)
		{
			if (executionCache == null)
			{
				return false;
			}
			string message;
			if (!AbilityRecipeItem.CanLearn(this.m_ability, executionCache.SourceEntity, true, out message))
			{
				executionCache.Message = message;
				executionCache.MsgType = MessageType.Skills;
				return false;
			}
			return true;
		}

		// Token: 0x0600516B RID: 20843 RVA: 0x001D0470 File Offset: 0x001CE670
		protected override void PostExecution(ExecutionCache executionCache)
		{
			if (!GameManager.IsServer)
			{
				base.PostExecution(executionCache);
				return;
			}
			string text;
			if (executionCache == null || executionCache.SourceEntity == null || executionCache.SourceEntity.CollectionController == null || executionCache.SourceEntity.CollectionController.Abilities == null || !AbilityRecipeItem.CanLearn(this.m_ability, executionCache.SourceEntity, true, out text))
			{
				return;
			}
			base.PostExecution(executionCache);
			if (GameManager.IsServer)
			{
				ArchetypeInstance archetypeInstance = this.m_ability.CreateNewInstance();
				executionCache.SourceEntity.CollectionController.Abilities.Add(archetypeInstance, true);
				ArchetypeAddedTransaction response = new ArchetypeAddedTransaction
				{
					Op = OpCodes.Ok,
					Context = ItemAddContext.Training,
					Instance = archetypeInstance,
					TargetContainer = archetypeInstance.ContainerInstance.Id
				};
				executionCache.SourceEntity.NetworkEntity.PlayerRpcHandler.AddItemResponse(response);
			}
		}

		// Token: 0x0600516C RID: 20844 RVA: 0x000765CC File Offset: 0x000747CC
		public bool CanRollNeed(GameEntity sourceEntity, out string cannotNeedReason)
		{
			return AbilityRecipeItem.CanLearn(this.m_ability, sourceEntity, false, out cannotNeedReason);
		}

		// Token: 0x0600516D RID: 20845 RVA: 0x001D0550 File Offset: 0x001CE750
		private static bool CanLearn(AbilityArchetype ability, GameEntity sourceEntity, bool checkLevel, out string errorMessage)
		{
			errorMessage = string.Empty;
			if (ability == null || sourceEntity == null)
			{
				errorMessage = "Well this is embarrassing...";
				return false;
			}
			if (sourceEntity.CollectionController == null || sourceEntity.CollectionController.Masteries == null || sourceEntity.CollectionController.Abilities == null)
			{
				errorMessage = "Well this is embarrassing...";
				return false;
			}
			ICollectionController collectionController = sourceEntity.CollectionController;
			ContainerInstance masteries = collectionController.Masteries;
			ArchetypeInstance archetypeInstance;
			if (collectionController.Abilities.TryGetInstanceForArchetypeId(ability.Id, out archetypeInstance))
			{
				errorMessage = "You already know this!";
				return false;
			}
			ArchetypeInstance archetypeInstance2;
			if (!masteries.TryGetInstanceForArchetypeId(ability.Mastery.Id, out archetypeInstance2) || archetypeInstance2.MasteryData == null || archetypeInstance2.Mastery == null)
			{
				errorMessage = "You can't make heads or tails of this thing!";
				return false;
			}
			if (checkLevel && archetypeInstance2.MasteryData.BaseLevel < (float)ability.MinimumLevel)
			{
				errorMessage = "It looks like this is relevant to your " + archetypeInstance2.Mastery.DisplayName + " expertise, but you're not quite ready yet.";
				return false;
			}
			if (ability.Specialization != null)
			{
				if (archetypeInstance2.MasteryData.Specialization == null)
				{
					errorMessage = "You do not have the expertise to learn this!";
					return false;
				}
				if (archetypeInstance2.MasteryData.Specialization.Value != ability.Specialization.Id)
				{
					errorMessage = "This doesn't look to be within your realm of expertise!";
					return false;
				}
				if (checkLevel && archetypeInstance2.MasteryData.SpecializationLevel < (float)ability.MinimumLevel)
				{
					errorMessage = "It looks like this is relevant to your " + ability.Specialization.DisplayName + " expertise, but you're not quite ready yet.";
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600516E RID: 20846 RVA: 0x001D06D0 File Offset: 0x001CE8D0
		public override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			base.FillTooltipBlocks(tooltip, instance, entity);
			if (this.m_ability == null)
			{
				return;
			}
			TooltipTextBlock requirementsBlock = tooltip.RequirementsBlock;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			ArchetypeInstance archetypeInstance = null;
			if (entity && entity.CollectionController != null)
			{
				if (entity.CollectionController.Masteries != null && entity.CollectionController.Masteries.TryGetInstanceForArchetypeId(this.m_ability.Mastery.Id, out archetypeInstance) && archetypeInstance != null && archetypeInstance.MasteryData != null)
				{
					flag = true;
					flag2 = (archetypeInstance.MasteryData.BaseLevel >= (float)this.m_ability.MinimumLevel);
					flag3 = (this.m_ability.Specialization == null || (archetypeInstance.MasteryData.Specialization != null && archetypeInstance.MasteryData.Specialization.Value == this.m_ability.Specialization.Id));
				}
				ArchetypeInstance archetypeInstance2;
				if (entity.CollectionController.Abilities != null && entity.CollectionController.Abilities.TryGetInstanceForArchetypeId(this.m_ability.Id, out archetypeInstance2))
				{
					flag4 = true;
				}
			}
			Color color = UIManager.RequirementsNotMetColor;
			color = (flag ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor);
			requirementsBlock.AppendLine("Required Role:", this.m_ability.Mastery.DisplayName.Color(color));
			if (this.m_ability.Specialization != null)
			{
				color = (flag3 ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor);
				requirementsBlock.AppendLine("Required Specialization:", this.m_ability.Specialization.DisplayName.Color(color));
			}
			color = (flag2 ? UIManager.RequirementsMetColor : UIManager.RequirementsNotMetColor);
			requirementsBlock.AppendLine("Required Level:", this.m_ability.MinimumLevel.ToString().Color(color));
			if (flag4)
			{
				requirementsBlock.AppendLine("", 0);
				requirementsBlock.AppendLine("Already known".Color(UIManager.RequirementsNotMetColor), 0);
			}
			requirementsBlock.AppendLine(this.m_ability.Description.Italicize(), 0);
		}

		// Token: 0x040048AD RID: 18605
		[SerializeField]
		private AbilityArchetype m_ability;

		// Token: 0x040048AE RID: 18606
		private const string kEmbarrassingError = "Well this is embarrassing...";
	}
}
