using System;
using BehaviorDesigner.Runtime.Tasks;
using SoL.Game.NPCs;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004E8 RID: 1256
	[TaskCategory("SoL/Npc")]
	[TaskDescription("Try to execute a combat ability")]
	public class TryExecuteAbility : BaseNodeAction<NpcSkillsController>
	{
		// Token: 0x060022DF RID: 8927 RVA: 0x00059196 File Offset: 0x00057396
		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();
			if (!this.m_controller || !this.m_controller.TryExecuteCombatAbility(this.TargetEntity.Value))
			{
				return TaskStatus.Failure;
			}
			return TaskStatus.Success;
		}

		// Token: 0x040026BD RID: 9917
		public SharedGameEntity TargetEntity;
	}
}
