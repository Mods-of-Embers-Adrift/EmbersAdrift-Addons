using System;
using SoL.Game.Audio;
using SoL.Managers;
using SoL.Networking;
using SoL.UI;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AAF RID: 2735
	public abstract class StackableUtilityItem : StackableItem, IUtilityItem
	{
		// Token: 0x17001368 RID: 4968
		// (get) Token: 0x0600547F RID: 21631 RVA: 0x0004BC2B File Offset: 0x00049E2B
		public BaseArchetype Archetype
		{
			get
			{
				return this;
			}
		}

		// Token: 0x06005480 RID: 21632
		protected abstract bool IsValidItem(ArchetypeInstance targetInstance);

		// Token: 0x06005481 RID: 21633
		public abstract CursorType GetCursorType();

		// Token: 0x06005482 RID: 21634
		protected abstract void ClientRequestExecuteUtilityInternal(GameEntity entity, ArchetypeInstance sourceItemInstance, ArchetypeInstance targetItemInstance);

		// Token: 0x06005483 RID: 21635
		protected abstract bool ExecuteUtilityInternal(GameEntity entity, ArchetypeInstance targetItemInstance);

		// Token: 0x17001369 RID: 4969
		// (get) Token: 0x06005484 RID: 21636 RVA: 0x00049FFA File Offset: 0x000481FA
		protected virtual AudioClipCollection ClipCollection
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700136A RID: 4970
		// (get) Token: 0x06005485 RID: 21637 RVA: 0x00045BCA File Offset: 0x00043DCA
		public virtual bool ResetCursorGameMode
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06005486 RID: 21638 RVA: 0x001DAA7C File Offset: 0x001D8C7C
		public void ClientRequestExecuteUtility(GameEntity entity, ArchetypeInstance sourceItemInstance, ArchetypeInstance targetItemInstance)
		{
			if (this.m_levelRequirement != null && !this.m_levelRequirement.MeetsAllRequirements(entity))
			{
				Debug.Log("Do not meet level requirements!");
				return;
			}
			if (sourceItemInstance == null || !this.IsValidItem(targetItemInstance))
			{
				Debug.Log("Invalid item to augment!");
				return;
			}
			this.ClientRequestExecuteUtilityInternal(entity, sourceItemInstance, targetItemInstance);
		}

		// Token: 0x06005487 RID: 21639 RVA: 0x001DAACC File Offset: 0x001D8CCC
		public void ExecuteUtility(GameEntity entity, ArchetypeInstance sourceItemInstance, ArchetypeInstance targetItemInstance)
		{
			OpCodes op = OpCodes.Error;
			bool consumed = false;
			if (this.m_levelRequirement != null && !this.m_levelRequirement.MeetsAllRequirements(entity))
			{
				Debug.Log("Do not meet level requirements!");
			}
			else if (this.IsValidItem(targetItemInstance) && this.ExecuteUtilityInternal(entity, targetItemInstance))
			{
				this.PlayAudioClip();
				op = OpCodes.Ok;
				if (GameManager.IsServer && sourceItemInstance != null)
				{
					sourceItemInstance.ReduceCountBy(1, entity);
					consumed = (sourceItemInstance.ArchetypeId == UniqueId.Empty);
				}
			}
			else
			{
				Debug.Log("Invalid item!");
			}
			if (GameManager.IsServer)
			{
				entity.NetworkEntity.PlayerRpcHandler.Server_ExecuteUtilityResponse(op, base.Id, targetItemInstance.ContainerInstance.ContainerType, targetItemInstance.InstanceId, consumed);
			}
		}

		// Token: 0x06005488 RID: 21640 RVA: 0x001DAB7C File Offset: 0x001D8D7C
		protected void SendExecuteUtilityRequest(GameEntity entity, ArchetypeInstance sourceItemInstance, ArchetypeInstance targetItemInstance)
		{
			if (!GameManager.IsServer && entity != null && sourceItemInstance != null && targetItemInstance != null)
			{
				entity.NetworkEntity.PlayerRpcHandler.Client_RequestExecuteUtility(sourceItemInstance.ContainerInstance.ContainerType, sourceItemInstance.InstanceId, targetItemInstance.ContainerInstance.ContainerType, targetItemInstance.InstanceId);
			}
		}

		// Token: 0x06005489 RID: 21641 RVA: 0x0004475B File Offset: 0x0004295B
		public virtual void AugmentUsed(GameEntity sourceEntity, ArchetypeInstance itemInstance, int amount)
		{
		}

		// Token: 0x0600548A RID: 21642 RVA: 0x001DABD4 File Offset: 0x001D8DD4
		public void PlayAudioClip()
		{
			if (!GameManager.IsServer && ClientGameManager.UIManager != null)
			{
				ClientGameManager.UIManager.PlayRandomClip(this.ClipCollection, null);
			}
		}

		// Token: 0x0600548B RID: 21643 RVA: 0x00078819 File Offset: 0x00076A19
		public override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			base.FillTooltipBlocks(tooltip, instance, entity);
			TooltipExtensions.AddRoleLevelRequirement(tooltip, instance, entity, this.m_levelRequirement, null);
		}

		// Token: 0x04004B19 RID: 19225
		[SerializeField]
		protected LevelRequirement m_levelRequirement;
	}
}
