using System;
using BehaviorDesigner.Runtime;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004C9 RID: 1225
	[Serializable]
	public class SharedBehaviorEffectFlags : SharedVariable<BehaviorEffectClassWrapper>
	{
		// Token: 0x0600228A RID: 8842 RVA: 0x00058DC8 File Offset: 0x00056FC8
		public static implicit operator SharedBehaviorEffectFlags(BehaviorEffectClassWrapper value)
		{
			return new SharedBehaviorEffectFlags
			{
				Value = value
			};
		}
	}
}
