using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using SoL.Game.NPCs;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004DA RID: 1242
	[TaskCategory("SoL/Npc")]
	[TaskDescription("Get Target")]
	public class GetTarget : BaseNodeAction<NpcTargetController>
	{
		// Token: 0x060022BB RID: 8891 RVA: 0x00127744 File Offset: 0x00125944
		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();
			NpcTarget npcTarget = this.m_controller ? this.m_controller.GetCurrentTarget() : null;
			if (npcTarget == null || !npcTarget.Entity)
			{
				this.TargetEntity.Value = null;
				this.TargetGameObject.Value = null;
				this.Target.Value = null;
				return TaskStatus.Failure;
			}
			this.TargetEntity.Value = npcTarget.Entity;
			this.TargetGameObject.Value = npcTarget.Entity.gameObject;
			this.Target.Value = npcTarget;
			return TaskStatus.Success;
		}

		// Token: 0x04002697 RID: 9879
		public SharedGameEntity TargetEntity;

		// Token: 0x04002698 RID: 9880
		public SharedGameObject TargetGameObject;

		// Token: 0x04002699 RID: 9881
		public SharedNpcTarget Target;
	}
}
