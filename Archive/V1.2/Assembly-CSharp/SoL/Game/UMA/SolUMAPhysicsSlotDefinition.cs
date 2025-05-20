using System;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UMA;
using UMA.Dynamics;
using UnityEngine;

namespace SoL.Game.UMA
{
	// Token: 0x02000624 RID: 1572
	public class SolUMAPhysicsSlotDefinition : MonoBehaviour
	{
		// Token: 0x060031AA RID: 12714 RVA: 0x0015D6E8 File Offset: 0x0015B8E8
		public void OnSkeletonAvailable(UMAData umaData)
		{
			GameEntity componentInParent = umaData.GetComponentInParent<GameEntity>();
			umaData.gameObject.GetOrAddComponent<SolUMAPhysicsAvatar>().Init(umaData, this.m_physicsElements, this.m_layer.LayerIndex, componentInParent);
		}

		// Token: 0x04003014 RID: 12308
		[SerializeField]
		private UMAPhysicsElement[] m_physicsElements;

		// Token: 0x04003015 RID: 12309
		[SerializeField]
		private SingleUnityLayer m_layer;
	}
}
