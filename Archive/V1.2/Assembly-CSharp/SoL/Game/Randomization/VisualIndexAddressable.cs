using System;
using SoL.Game.AssetBundles;
using SoL.Managers;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SoL.Game.Randomization
{
	// Token: 0x02000778 RID: 1912
	public class VisualIndexAddressable : GameEntityComponent
	{
		// Token: 0x06003867 RID: 14439 RVA: 0x0016D8B8 File Offset: 0x0016BAB8
		private void Start()
		{
			if (GameManager.IsServer)
			{
				return;
			}
			int num = (int)((base.GameEntity != null && base.GameEntity.SeedReplicator != null && base.GameEntity.SeedReplicator.VisualIndexOverride != null && (int)base.GameEntity.SeedReplicator.VisualIndexOverride.Value < this.m_references.Length) ? base.GameEntity.SeedReplicator.VisualIndexOverride.Value : 0);
			if (this.m_references[num] != null && this.m_references[num].RuntimeKeyIsValid())
			{
				AddressableManager.SpawnInstance(this.m_references[num], (this.m_parentTransformOverride == null) ? base.gameObject.transform : this.m_parentTransformOverride, null);
			}
		}

		// Token: 0x04003743 RID: 14147
		[SerializeField]
		private Transform m_parentTransformOverride;

		// Token: 0x04003744 RID: 14148
		[SerializeField]
		private AssetReference[] m_references;
	}
}
