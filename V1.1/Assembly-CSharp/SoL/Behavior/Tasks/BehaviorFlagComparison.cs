using System;
using BehaviorDesigner.Runtime.Tasks;
using SoL.Game.EffectSystem;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004D2 RID: 1234
	[TaskCategory("SoL/Npc")]
	[TaskDescription("Check for behavior flags")]
	public class BehaviorFlagComparison : Conditional
	{
		// Token: 0x060022A6 RID: 8870 RVA: 0x00058ED4 File Offset: 0x000570D4
		public override TaskStatus OnUpdate()
		{
			if ((this.FlagsToCheckFor.Value.Flags & this.DynamicFlags.Value.Flags) > BehaviorEffectTypeFlags.None != this.DesiredResult)
			{
				return TaskStatus.Failure;
			}
			return TaskStatus.Success;
		}

		// Token: 0x060022A7 RID: 8871 RVA: 0x00058F05 File Offset: 0x00057105
		public override void OnReset()
		{
			this.FlagsToCheckFor.Value.Flags = BehaviorEffectTypeFlags.None;
			this.DynamicFlags.Value.Flags = BehaviorEffectTypeFlags.None;
		}

		// Token: 0x04002682 RID: 9858
		public SharedBehaviorEffectFlags FlagsToCheckFor;

		// Token: 0x04002683 RID: 9859
		public SharedBehaviorEffectFlags DynamicFlags;

		// Token: 0x04002684 RID: 9860
		public bool DesiredResult;
	}
}
