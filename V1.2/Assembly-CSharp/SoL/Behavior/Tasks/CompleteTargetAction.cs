using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using SoL.Game.NPCs;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004D4 RID: 1236
	[TaskDescription("Complete Npc Action on Target")]
	[TaskCategory("SoL/Npc")]
	public class CompleteTargetAction : BaseNodeAction<NpcTargetController>
	{
		// Token: 0x060022AB RID: 8875 RVA: 0x001273C8 File Offset: 0x001255C8
		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();
			if (this.Target.Value != null)
			{
				if (this.Target.Value.NetworkId == 0U || !this.m_controller || !this.m_controller.CompleteAction(this.Target.Value.NetworkId))
				{
					return TaskStatus.Failure;
				}
				return TaskStatus.Success;
			}
			else
			{
				if (this.TargetNetworkId.Value < 0)
				{
					return TaskStatus.Failure;
				}
				if (!this.m_controller || !this.m_controller.CompleteAction((uint)this.TargetNetworkId.Value))
				{
					return TaskStatus.Failure;
				}
				return TaskStatus.Success;
			}
		}

		// Token: 0x04002687 RID: 9863
		public SharedNpcTarget Target;

		// Token: 0x04002688 RID: 9864
		public SharedInt TargetNetworkId;
	}
}
