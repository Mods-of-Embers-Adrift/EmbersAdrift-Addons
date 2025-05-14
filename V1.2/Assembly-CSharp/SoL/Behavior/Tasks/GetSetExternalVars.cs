using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using SoL.Game.NPCs;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004D9 RID: 1241
	[TaskCategory("SoL/Npc")]
	[TaskDescription("GetSet External NPC vars")]
	public class GetSetExternalVars : BaseNodeAction<NpcTargetController>
	{
		// Token: 0x060022B8 RID: 8888 RVA: 0x00058FEB File Offset: 0x000571EB
		public override void OnAwake()
		{
			base.OnAwake();
			this.ReactionTime.Value = 1f;
			this.HostileTargetCount.Value = 0;
			this.NeutralTargetCount.Value = 0;
			this.AlertCount.Value = 0;
		}

		// Token: 0x060022B9 RID: 8889 RVA: 0x001276B8 File Offset: 0x001258B8
		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();
			if (this.m_controller)
			{
				this.HostileTargetCount.Value = this.m_controller.HostileTargetCount;
				this.NeutralTargetCount.Value = this.m_controller.NeutralTargetCount;
				this.AlertCount.Value = this.m_controller.AlertCount;
			}
			else
			{
				this.HostileTargetCount.Value = 0;
				this.NeutralTargetCount.Value = 0;
				this.AlertCount.Value = 0;
			}
			return TaskStatus.Success;
		}

		// Token: 0x04002693 RID: 9875
		public SharedFloat ReactionTime;

		// Token: 0x04002694 RID: 9876
		public SharedInt HostileTargetCount;

		// Token: 0x04002695 RID: 9877
		public SharedInt NeutralTargetCount;

		// Token: 0x04002696 RID: 9878
		public SharedInt AlertCount;
	}
}
