using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004E2 RID: 1250
	[TaskCategory("SoL/Npc")]
	[TaskDescription("Do Interaction")]
	public class NpcInteraction : BehaviorDesigner.Runtime.Tasks.Action
	{
		// Token: 0x060022D2 RID: 8914 RVA: 0x00127E14 File Offset: 0x00126014
		public override TaskStatus OnUpdate()
		{
			if (!this.Interactive.Value)
			{
				return TaskStatus.Failure;
			}
			if (this.Interactive.Value.ResetInitialPosition)
			{
				this.InitialPosition.Value = this.Interactive.Value.gameObject.transform.position;
			}
			this.waitTime.Value = this.Interactive.Value.WaitParameters.WaitTime;
			this.randomWait.Value = this.Interactive.Value.WaitParameters.RandomWait;
			this.randomWaitMin.Value = this.Interactive.Value.WaitParameters.RandomWaitTime.Min;
			this.randomWaitMax.Value = this.Interactive.Value.WaitParameters.RandomWaitTime.Max;
			return TaskStatus.Success;
		}

		// Token: 0x060022D3 RID: 8915 RVA: 0x00059137 File Offset: 0x00057337
		public override void OnBehaviorComplete()
		{
			base.OnBehaviorComplete();
			if (this.Interactive != null)
			{
				this.Interactive.Value = null;
			}
		}

		// Token: 0x040026B1 RID: 9905
		public SharedNpcInteractive Interactive;

		// Token: 0x040026B2 RID: 9906
		public SharedVector3 InitialPosition;

		// Token: 0x040026B3 RID: 9907
		public SharedFloat waitTime = 1f;

		// Token: 0x040026B4 RID: 9908
		public SharedBool randomWait = false;

		// Token: 0x040026B5 RID: 9909
		public SharedFloat randomWaitMin = 1f;

		// Token: 0x040026B6 RID: 9910
		public SharedFloat randomWaitMax = 1f;
	}
}
