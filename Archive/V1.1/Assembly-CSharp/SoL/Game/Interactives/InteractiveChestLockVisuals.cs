using System;
using SoL.Game.Crafting;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B83 RID: 2947
	public class InteractiveChestLockVisuals : GameEntityComponent
	{
		// Token: 0x06005AD8 RID: 23256 RVA: 0x001ED9F4 File Offset: 0x001EBBF4
		private void Start()
		{
			if (!GameManager.IsServer && base.GameEntity != null && base.GameEntity.Interactive != null && base.GameEntity.Interactive.TryGetAsType(out this.m_node) && this.m_node.SyncInteractiveFlags != null)
			{
				this.m_gatheringNode = this.m_node;
				this.m_node.SyncInteractiveFlags.Changed += this.InteractiveFlagsOnChanged;
				this.RefreshLockVisuals();
			}
		}

		// Token: 0x06005AD9 RID: 23257 RVA: 0x0007D034 File Offset: 0x0007B234
		private void OnDestroy()
		{
			if (!GameManager.IsServer && this.m_node && this.m_node.SyncInteractiveFlags != null)
			{
				this.m_node.SyncInteractiveFlags.Changed -= this.InteractiveFlagsOnChanged;
			}
		}

		// Token: 0x06005ADA RID: 23258 RVA: 0x0007D073 File Offset: 0x0007B273
		private void InteractiveFlagsOnChanged(InteractiveFlags obj)
		{
			this.RefreshLockVisuals();
		}

		// Token: 0x06005ADB RID: 23259 RVA: 0x001EDA78 File Offset: 0x001EBC78
		private void RefreshLockVisuals()
		{
			if (this.m_lockVisuals && this.m_gatheringNode != null && this.m_gatheringNode.RequiredItemId != null && this.m_gatheringNode.RemoveRequiredItemOnUse)
			{
				this.m_lockVisuals.SetActive(!this.m_gatheringNode.InteractiveFlags.HasBitFlag(InteractiveFlags.RecordGenerated));
			}
		}

		// Token: 0x04004F99 RID: 20377
		[SerializeField]
		private GameObject m_lockVisuals;

		// Token: 0x04004F9A RID: 20378
		private InteractiveGatheringNode m_node;

		// Token: 0x04004F9B RID: 20379
		private IGatheringNode m_gatheringNode;
	}
}
