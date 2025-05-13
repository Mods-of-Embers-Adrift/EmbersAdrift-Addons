using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004E1 RID: 1249
	[TaskCategory("SoL/Npc")]
	[TaskDescription("SearchArea")]
	public class MoveToPosition : NavMeshMovement
	{
		// Token: 0x060022D0 RID: 8912 RVA: 0x00127DB4 File Offset: 0x00125FB4
		public override TaskStatus OnUpdate()
		{
			this.SetDestination(this.TargetPosition.Value);
			bool flag = this.HasArrived();
			if (!flag)
			{
				base.UpdateSpeed();
				if (base.ShouldInterrupt())
				{
					return TaskStatus.Failure;
				}
			}
			if (!flag)
			{
				return TaskStatus.Running;
			}
			return TaskStatus.Success;
		}

		// Token: 0x040026B0 RID: 9904
		public SharedVector3 TargetPosition;
	}
}
