using System;
using SoL.Game.Animation;
using SoL.Game.Crafting;
using SoL.Managers;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes.Abilities
{
	// Token: 0x02000AF2 RID: 2802
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Abilities/Interaction")]
	public class InteractionAbility : DynamicAbility
	{
		// Token: 0x060056B3 RID: 22195 RVA: 0x00079A8C File Offset: 0x00077C8C
		public override string GetInstanceId()
		{
			return base.Id;
		}

		// Token: 0x17001444 RID: 5188
		// (get) Token: 0x060056B4 RID: 22196 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool CreateInstanceUI
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060056B5 RID: 22197 RVA: 0x001E1310 File Offset: 0x001DF510
		protected override bool PreExecution(ExecutionCache executionCache, bool initial)
		{
			if (!base.PreExecution(executionCache, initial))
			{
				return false;
			}
			if (!GameManager.IsServer)
			{
				executionCache.SetTargetNetworkEntity(executionCache.SourceEntity.CollectionController.GatheringNode.GameEntity.NetworkEntity);
			}
			if (executionCache.TargetNetworkEntity == null)
			{
				executionCache.Message = "Invalid target!";
				return false;
			}
			IGatheringNode gatheringNode = GameManager.IsServer ? executionCache.TargetNetworkEntity.GetComponent<IGatheringNode>() : executionCache.SourceEntity.CollectionController.GatheringNode;
			if (gatheringNode == null)
			{
				executionCache.Message = "Invalid node!";
				return false;
			}
			executionCache.SetExecutionTime(gatheringNode.GatherTime);
			executionCache.MasteryInstance = null;
			if (executionCache.SourceEntity.CharacterData.AdventuringLevel < gatheringNode.ResourceLevel)
			{
				executionCache.Message = "You must be a bit stronger to interact with this!";
				return false;
			}
			if (gatheringNode.RequiredTool != CraftingToolType.None)
			{
				executionCache.Message = "Invalid node type!";
				return false;
			}
			ArchetypeInstance archetypeInstance;
			if (!gatheringNode.CanInteract(executionCache.SourceEntity, out archetypeInstance))
			{
				executionCache.Message = "Cannot interact!";
				return false;
			}
			if (initial && gatheringNode.RequiredItemId != null && gatheringNode.RemoveRequiredItemOnUse && archetypeInstance != null)
			{
				ReductionTaskType type = (archetypeInstance.Archetype && archetypeInstance.Archetype.ArchetypeHasCount()) ? ReductionTaskType.Count : ReductionTaskType.Consume;
				executionCache.AddReductionTask(type, archetypeInstance, 1);
			}
			return true;
		}

		// Token: 0x060056B6 RID: 22198 RVA: 0x00079C17 File Offset: 0x00077E17
		protected override void PostExecution(ExecutionCache executionCache)
		{
			if (GameManager.IsServer)
			{
				executionCache.PerformReduction();
				IGatheringNode component = executionCache.TargetNetworkEntity.GetComponent<IGatheringNode>();
				if (component == null)
				{
					return;
				}
				component.BeginInteraction(executionCache.SourceEntity);
			}
		}

		// Token: 0x060056B7 RID: 22199 RVA: 0x0005BFF3 File Offset: 0x0005A1F3
		protected override bool TryGetMastery(GameEntity entity, out MasteryArchetype mastery)
		{
			mastery = null;
			return false;
		}

		// Token: 0x060056B8 RID: 22200 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool MeetsRequirementsForUI(GameEntity entity)
		{
			return true;
		}

		// Token: 0x060056B9 RID: 22201 RVA: 0x00079C41 File Offset: 0x00077E41
		protected override bool TryGetAbilityAnimation(GameEntity entity, out AbilityAnimation animation)
		{
			animation = this.m_animation;
			return animation != null;
		}

		// Token: 0x04004C82 RID: 19586
		protected const string kAnimGroupName = "Animations";

		// Token: 0x04004C83 RID: 19587
		[SerializeField]
		protected AbilityAnimation m_animation;
	}
}
