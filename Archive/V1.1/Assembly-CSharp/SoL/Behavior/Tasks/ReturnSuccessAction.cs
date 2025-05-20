using System;
using BehaviorDesigner.Runtime.Tasks;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004E5 RID: 1253
	[TaskDescription("The ReturnSuccessAction task will return success.")]
	[TaskIcon("{SkinColor}ReturnSuccessIcon.png")]
	public class ReturnSuccessAction : BehaviorDesigner.Runtime.Tasks.Action
	{
		// Token: 0x060022D9 RID: 8921 RVA: 0x00053500 File Offset: 0x00051700
		public override TaskStatus OnUpdate()
		{
			return TaskStatus.Success;
		}
	}
}
