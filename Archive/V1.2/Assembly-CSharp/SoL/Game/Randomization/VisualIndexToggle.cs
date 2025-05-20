using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Randomization
{
	// Token: 0x02000779 RID: 1913
	public class VisualIndexToggle : GameEntityComponent
	{
		// Token: 0x06003869 RID: 14441 RVA: 0x0016D990 File Offset: 0x0016BB90
		private void Start()
		{
			if (GameManager.IsServer)
			{
				return;
			}
			int num = (int)((base.GameEntity != null && base.GameEntity.SeedReplicator != null && base.GameEntity.SeedReplicator.VisualIndexOverride != null && (int)base.GameEntity.SeedReplicator.VisualIndexOverride.Value < this.m_objects.Length) ? base.GameEntity.SeedReplicator.VisualIndexOverride.Value : 0);
			for (int i = 0; i < this.m_objects.Length; i++)
			{
				if (this.m_objects[i])
				{
					this.m_objects[i].SetActive(i == num);
				}
			}
		}

		// Token: 0x04003745 RID: 14149
		[SerializeField]
		private GameObject[] m_objects;
	}
}
