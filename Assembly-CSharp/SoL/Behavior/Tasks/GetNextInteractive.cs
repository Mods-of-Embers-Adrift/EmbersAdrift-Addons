using System;
using BehaviorDesigner.Runtime.Tasks;
using SoL.Game;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004D8 RID: 1240
	[TaskCategory("SoL/Npc")]
	[TaskDescription("Get Next Interactive")]
	public class GetNextInteractive : BaseNodeAction<ServerNpcController>
	{
		// Token: 0x060022B5 RID: 8885 RVA: 0x00127640 File Offset: 0x00125840
		public override TaskStatus OnUpdate()
		{
			if (!this.Interactive.Value)
			{
				return TaskStatus.Failure;
			}
			this.Interactive.Value = (this.m_controller ? this.Interactive.Value.GetNextInteractive(this.m_controller.GameEntity) : null);
			return TaskStatus.Success;
		}

		// Token: 0x060022B6 RID: 8886 RVA: 0x00058FCF File Offset: 0x000571CF
		public override void OnBehaviorComplete()
		{
			base.OnBehaviorComplete();
			if (this.Interactive != null)
			{
				this.Interactive.Value = null;
			}
		}

		// Token: 0x04002692 RID: 9874
		public SharedNpcInteractive Interactive;
	}
}
