using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Managers;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Crafting
{
	// Token: 0x02000CD9 RID: 3289
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Abilities/Crafting/Gathering")]
	public class GatheringAbility : AbilityArchetype
	{
		// Token: 0x170017C2 RID: 6082
		// (get) Token: 0x06006383 RID: 25475 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool MustBeMemorizedToExecute
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170017C3 RID: 6083
		// (get) Token: 0x06006384 RID: 25476 RVA: 0x00045BCA File Offset: 0x00043DCA
		public override bool ConsiderHaste
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170017C4 RID: 6084
		// (get) Token: 0x06006385 RID: 25477 RVA: 0x00045BCA File Offset: 0x00043DCA
		public override bool AllowAlchemy
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170017C5 RID: 6085
		// (get) Token: 0x06006386 RID: 25478 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool m_addGroupBonus
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170017C6 RID: 6086
		// (get) Token: 0x06006387 RID: 25479 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool m_allowStaminaRegenDuringExecution
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06006388 RID: 25480 RVA: 0x00207398 File Offset: 0x00205598
		protected override bool ExecutionCheck(ExecutionCache executionCache, float executionProgress)
		{
			if (!base.ExecutionCheck(executionCache, executionProgress))
			{
				return false;
			}
			bool flag = executionProgress <= 0f;
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
			if (gatheringNode.RequiredTool == CraftingToolType.None)
			{
				executionCache.Message = "Invalid tool requirement on node!";
				return false;
			}
			int num;
			string message;
			if (!CraftingToolItem.TryGetValidTool(executionCache.SourceEntity, gatheringNode, out executionCache.ToolItem, out executionCache.ToolInstance, out num, out message))
			{
				executionCache.Message = message;
				return false;
			}
			ArchetypeInstance masteryInstance;
			if (!executionCache.SourceEntity.CollectionController.Masteries.TryGetInstanceForArchetypeId(this.m_mastery.Id, out masteryInstance))
			{
				executionCache.Message = this.m_mastery.DisplayName + " is required to harvest from this node!";
				return false;
			}
			executionCache.MasteryInstance = masteryInstance;
			if (executionCache.AbilityLevel < (float)gatheringNode.ResourceLevel)
			{
				executionCache.Message = "You must be better at " + this.DisplayName + " to harvest this node!";
				return false;
			}
			if (gatheringNode.RequiredTool == CraftingToolType.None)
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
			if (flag && gatheringNode.RequiredItemId != null && gatheringNode.RemoveRequiredItemOnUse && archetypeInstance != null)
			{
				ReductionTaskType type = (archetypeInstance.Archetype && archetypeInstance.Archetype.ArchetypeHasCount()) ? ReductionTaskType.Count : ReductionTaskType.Consume;
				executionCache.AddReductionTask(type, archetypeInstance, 1);
			}
			return true;
		}

		// Token: 0x06006389 RID: 25481 RVA: 0x00207574 File Offset: 0x00205774
		protected override void PostExecution(ExecutionCache executionCache)
		{
			base.PostExecution(executionCache);
			if (GameManager.IsServer)
			{
				IGatheringNode component = executionCache.TargetNetworkEntity.GetComponent<IGatheringNode>();
				if (component != null)
				{
					component.BeginInteraction(executionCache.SourceEntity);
				}
				if (executionCache.ToolInstance != null)
				{
					int maxDamageAbsorption = executionCache.ToolItem.MaxDamageAbsorption;
					int absorbed = executionCache.ToolInstance.ItemData.Durability.Absorbed;
					int toolDecayPerUse = GlobalSettings.Values.Gathering.ToolDecayPerUse;
					int num = Mathf.Clamp(absorbed + toolDecayPerUse, 0, maxDamageAbsorption);
					if (absorbed != num)
					{
						executionCache.ToolInstance.ItemData.Durability.Absorbed = num;
						executionCache.SourceEntity.NetworkEntity.PlayerRpcHandler.ModifyEquipmentAbsorbed(executionCache.ToolInstance.InstanceId, executionCache.ToolInstance.ItemData.Durability.Absorbed);
					}
				}
				ProgressionCalculator.OnGatheringSuccess(executionCache.SourceEntity, executionCache.MasteryInstance, (component != null) ? component.ResourceLevel : 1, 1f);
			}
		}
	}
}
