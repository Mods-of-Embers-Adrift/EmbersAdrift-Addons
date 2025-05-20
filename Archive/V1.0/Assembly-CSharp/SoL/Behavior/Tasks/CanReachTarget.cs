using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using SoL.Utilities;
using UnityEngine.AI;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004D3 RID: 1235
	[TaskCategory("SoL/Npc")]
	[TaskDescription("Can Reach Target")]
	public class CanReachTarget : Conditional
	{
		// Token: 0x060022A9 RID: 8873 RVA: 0x0012734C File Offset: 0x0012554C
		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();
			NavMeshHit navMeshHit;
			if (this.TargetGameObject.Value != null && NavMeshUtilities.SamplePosition(this.TargetGameObject.Value.transform.position, out navMeshHit, this.StoppingDistance.Value * 1.05f, -1))
			{
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}

		// Token: 0x04002685 RID: 9861
		public SharedGameObject TargetGameObject;

		// Token: 0x04002686 RID: 9862
		public SharedFloat StoppingDistance;
	}
}
