using System;
using SoL.Game.Objects.Containers;
using SoL.Managers;

namespace SoL.Game.Interactives
{
	// Token: 0x02000BA2 RID: 2978
	public abstract class InteractiveRemoteContainer : BaseNetworkedInteractive
	{
		// Token: 0x06005C2E RID: 23598 RVA: 0x0007DDA1 File Offset: 0x0007BFA1
		public override bool CanInteract(GameEntity entity)
		{
			return entity != null && !entity.CharacterData.CharacterFlags.Value.BlockRemoteContainerInteractions(base.PreventInteractionsWhileMissingBag, this.m_notifyOnCanInteractFail) && base.CanInteract(entity);
		}

		// Token: 0x06005C2F RID: 23599 RVA: 0x001F0C98 File Offset: 0x001EEE98
		protected bool OpenRemoteContainer(GameEntity interactionSource, ContainerRecord record)
		{
			if (!GameManager.IsServer || record == null || interactionSource == null)
			{
				return false;
			}
			PlayerCollectionController playerCollectionController;
			if (interactionSource.CollectionController != null && interactionSource.CollectionController.TryGetAsType(out playerCollectionController))
			{
				playerCollectionController.OpenRemoteContainer(this, record);
				interactionSource.CharacterData.CharacterFlags.Value |= PlayerFlags.RemoteContainer;
				interactionSource.NetworkEntity.PlayerRpcHandler.OpenRemoteContainer(base.GameEntity.NetworkEntity, record);
				return true;
			}
			return false;
		}

		// Token: 0x06005C30 RID: 23600 RVA: 0x001F0D14 File Offset: 0x001EEF14
		protected void CloseRemoteContainer(GameEntity interactionSource, string containerId, bool clientIsEnding)
		{
			PlayerCollectionController playerCollectionController;
			if (interactionSource != null && interactionSource.CollectionController != null && interactionSource.CollectionController.TryGetAsType(out playerCollectionController))
			{
				playerCollectionController.CloseRemoteContainer(containerId);
			}
			if (GameManager.IsServer && !clientIsEnding)
			{
				interactionSource.NetworkEntity.PlayerRpcHandler.Server_CancelInteraction(base.GameEntity.NetworkEntity);
			}
			else if (!GameManager.IsServer && clientIsEnding)
			{
				interactionSource.NetworkEntity.PlayerRpcHandler.Client_CancelInteraction(base.GameEntity.NetworkEntity);
			}
			if (GameManager.IsServer && interactionSource && interactionSource.CharacterData)
			{
				interactionSource.CharacterData.CharacterFlags.Value &= ~PlayerFlags.RemoteContainer;
			}
		}
	}
}
