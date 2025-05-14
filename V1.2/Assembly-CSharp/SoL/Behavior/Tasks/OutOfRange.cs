using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004E3 RID: 1251
	[TaskCategory("SoL/Npc")]
	[TaskDescription("Check to see if the NPC is out of range")]
	public class OutOfRange : Conditional
	{
		// Token: 0x060022D5 RID: 8917 RVA: 0x00127F50 File Offset: 0x00126150
		public override TaskStatus OnUpdate()
		{
			if ((this.Position.Value - this.gameObject.transform.position).sqrMagnitude <= this.Radius.Value * this.Radius.Value)
			{
				return TaskStatus.Failure;
			}
			return TaskStatus.Success;
		}

		// Token: 0x040026B7 RID: 9911
		public SharedVector3 Position;

		// Token: 0x040026B8 RID: 9912
		public SharedFloat Radius;
	}
}
