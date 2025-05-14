using System;
using BehaviorDesigner.Runtime.Tasks;
using SoL.Game;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004E0 RID: 1248
	[TaskCategory("SoL/Npc")]
	[TaskDescription("Set NpcMotor Movement Mode")]
	public class MovementMode : BaseNodeAction<ServerNpcController>
	{
		// Token: 0x060022CE RID: 8910 RVA: 0x0005910A File Offset: 0x0005730A
		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();
			if (this.m_controller)
			{
				this.m_controller.MovementMode = this.TargetMovementMode.Value;
			}
			return TaskStatus.Success;
		}

		// Token: 0x040026AF RID: 9903
		public SharedNpcMovementMode TargetMovementMode;
	}
}
