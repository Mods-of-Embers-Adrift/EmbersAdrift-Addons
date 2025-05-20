using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004E6 RID: 1254
	[TaskCategory("SoL/Npc")]
	[TaskDescription("Save Position")]
	public class SavePosition : BehaviorDesigner.Runtime.Tasks.Action
	{
		// Token: 0x060022DB RID: 8923 RVA: 0x00059153 File Offset: 0x00057353
		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();
			this.SavedPosition.Value = this.gameObject.transform.position;
			return TaskStatus.Success;
		}

		// Token: 0x040026B9 RID: 9913
		public SharedVector3 SavedPosition;
	}
}
