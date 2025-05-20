using System;
using BehaviorDesigner.Runtime.Tasks;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004E4 RID: 1252
	[TaskDescription("The ReturnFailureAction task will return success.")]
	[TaskIcon("{SkinColor}ReturnFailureIcon.png")]
	public class ReturnFailureAction : BehaviorDesigner.Runtime.Tasks.Action
	{
		// Token: 0x060022D7 RID: 8919 RVA: 0x0004479C File Offset: 0x0004299C
		public override TaskStatus OnUpdate()
		{
			return TaskStatus.Failure;
		}
	}
}
