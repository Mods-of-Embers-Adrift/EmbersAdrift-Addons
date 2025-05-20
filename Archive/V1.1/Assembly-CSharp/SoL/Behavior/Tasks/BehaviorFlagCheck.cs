using System;
using BehaviorDesigner.Runtime.Tasks;
using SoL.Game.EffectSystem;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004D1 RID: 1233
	[TaskCategory("SoL/Npc")]
	public class BehaviorFlagCheck : BehaviorDesigner.Runtime.Tasks.Action
	{
		// Token: 0x060022A3 RID: 8867 RVA: 0x00058E80 File Offset: 0x00057080
		public override TaskStatus OnUpdate()
		{
			base.OnUpdate();
			if ((this.FlagsToCheckFor.Value.Flags & this.DynamicFlags.Value.Flags) <= BehaviorEffectTypeFlags.None)
			{
				return TaskStatus.Failure;
			}
			return TaskStatus.Success;
		}

		// Token: 0x060022A4 RID: 8868 RVA: 0x00058EB0 File Offset: 0x000570B0
		public override void OnReset()
		{
			this.FlagsToCheckFor.Value.Flags = BehaviorEffectTypeFlags.None;
			this.DynamicFlags.Value.Flags = BehaviorEffectTypeFlags.None;
		}

		// Token: 0x04002680 RID: 9856
		public SharedBehaviorEffectFlags FlagsToCheckFor;

		// Token: 0x04002681 RID: 9857
		public SharedBehaviorEffectFlags DynamicFlags;
	}
}
