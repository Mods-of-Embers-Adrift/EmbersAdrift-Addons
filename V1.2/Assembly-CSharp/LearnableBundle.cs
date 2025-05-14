using System;
using System.Collections.Generic;
using SoL;
using SoL.Game;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Containers;
using SoL.Game.Transactions;
using SoL.Managers;
using SoL.Networking;
using SoL.Utilities;
using UnityEngine;

// Token: 0x0200000C RID: 12
[Serializable]
public class LearnableBundle
{
	// Token: 0x06000027 RID: 39 RVA: 0x00087C60 File Offset: 0x00085E60
	public void TeachToPlayer(GameEntity entity)
	{
		LearnableContainerInstance learnableContainerInstance;
		if (GameManager.IsServer && this.m_learnables != null && entity.CollectionController.TryGetLearnableInstance(this.m_containerType, out learnableContainerInstance))
		{
			List<UniqueId> fromPool = StaticListPool<UniqueId>.GetFromPool();
			for (int i = 0; i < this.m_learnables.Length; i++)
			{
				if (this.m_learnables[i] != null && !learnableContainerInstance.Contains(this.m_learnables[i].Id))
				{
					learnableContainerInstance.Add(this.m_learnables[i], true);
					fromPool.Add(this.m_learnables[i].Id);
				}
			}
			if (fromPool.Count > 0)
			{
				LearnablesAddedTransaction transaction = new LearnablesAddedTransaction
				{
					Op = OpCodes.Ok,
					LearnableIds = fromPool.ToArray(),
					TargetContainer = learnableContainerInstance.Id
				};
				entity.NetworkEntity.PlayerRpcHandler.LearnablesAdded(transaction);
			}
			StaticListPool<UniqueId>.ReturnToPool(fromPool);
		}
	}

	// Token: 0x04000019 RID: 25
	[SerializeField]
	private LearnableArchetype[] m_learnables;

	// Token: 0x0400001A RID: 26
	[SerializeField]
	private ContainerType m_containerType = ContainerType.Recipes;
}
