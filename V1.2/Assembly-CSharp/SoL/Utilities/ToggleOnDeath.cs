using System;
using SoL.Game;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002F8 RID: 760
	public class ToggleOnDeath : GameEntityComponent
	{
		// Token: 0x0600157D RID: 5501 RVA: 0x000FCA0C File Offset: 0x000FAC0C
		private void Start()
		{
			if (base.GameEntity != null && base.GameEntity.VitalsReplicator)
			{
				base.GameEntity.VitalsReplicator.CurrentHealthState.Changed += this.CurrentHealthStateOnChanged;
				this.CurrentHealthStateOnChanged(base.GameEntity.VitalsReplicator.CurrentHealthState.Value);
			}
		}

		// Token: 0x0600157E RID: 5502 RVA: 0x000FCA78 File Offset: 0x000FAC78
		private void OnDestroy()
		{
			if (base.GameEntity != null && base.GameEntity.VitalsReplicator)
			{
				base.GameEntity.VitalsReplicator.CurrentHealthState.Changed -= this.CurrentHealthStateOnChanged;
			}
		}

		// Token: 0x0600157F RID: 5503 RVA: 0x000511F1 File Offset: 0x0004F3F1
		private void CurrentHealthStateOnChanged(HealthState obj)
		{
			if (obj == HealthState.Unconscious || obj == HealthState.Dead)
			{
				this.ToggleThings(this.m_enableOnDeath);
				return;
			}
			this.ToggleThings(!this.m_enableOnDeath);
		}

		// Token: 0x06001580 RID: 5504 RVA: 0x000FCAC8 File Offset: 0x000FACC8
		private void ToggleThings(bool isEnabled)
		{
			if (this.m_objects == null || this.m_objects.Length == 0)
			{
				return;
			}
			for (int i = 0; i < this.m_objects.Length; i++)
			{
				if (this.m_objects[i])
				{
					this.m_objects[i].SetActive(isEnabled);
				}
			}
		}

		// Token: 0x04001D96 RID: 7574
		[SerializeField]
		private bool m_enableOnDeath;

		// Token: 0x04001D97 RID: 7575
		[SerializeField]
		private GameObject[] m_objects;
	}
}
