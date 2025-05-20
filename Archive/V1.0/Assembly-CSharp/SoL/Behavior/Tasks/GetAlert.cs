using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using SoL.Game.NPCs;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004D7 RID: 1239
	[TaskCategory("SoL/Npc")]
	[TaskDescription("Get Alert")]
	public class GetAlert : BaseNodeAction<NpcTargetController>
	{
		// Token: 0x060022B2 RID: 8882 RVA: 0x00127584 File Offset: 0x00125784
		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();
			if (this.m_controller != null)
			{
				NpcTarget alertTarget = this.m_controller.AlertTarget;
				NpcAlert npcAlert = (alertTarget != null) ? alertTarget.Alert : null;
				if (npcAlert != null && npcAlert.Entity != null)
				{
					this.Position.Value = npcAlert.Position;
					this.AlertGameObject.Value = npcAlert.Entity.gameObject;
					this.AlertNetworkId.Value = (int)alertTarget.NetworkId;
					return TaskStatus.Success;
				}
			}
			this.AlertNetworkId.Value = 0;
			this.Position.Value = this.gameObject.transform.position;
			this.AlertGameObject.Value = null;
			return TaskStatus.Failure;
		}

		// Token: 0x060022B3 RID: 8883 RVA: 0x00058FB3 File Offset: 0x000571B3
		public override void OnBehaviorComplete()
		{
			base.OnBehaviorComplete();
			if (this.AlertGameObject != null)
			{
				this.AlertGameObject.Value = null;
			}
		}

		// Token: 0x0400268F RID: 9871
		public SharedVector3 Position;

		// Token: 0x04002690 RID: 9872
		public SharedGameObject AlertGameObject;

		// Token: 0x04002691 RID: 9873
		public SharedInt AlertNetworkId;
	}
}
