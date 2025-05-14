using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace SoL.Game.Behavior
{
	// Token: 0x020007F2 RID: 2034
	public static class BehaviorTreeExtensions
	{
		// Token: 0x06003B2F RID: 15151 RVA: 0x0017ABF8 File Offset: 0x00178DF8
		public static bool LoadTreeToTask(this BehaviorTree tree, string name, ExternalBehaviorTree externalTreeReference)
		{
			BehaviorTreeReference behaviorTreeReference = tree.FindTaskWithName(name) as BehaviorTreeReference;
			if (behaviorTreeReference == null)
			{
				return false;
			}
			if (behaviorTreeReference.externalBehaviors == null || behaviorTreeReference.externalBehaviors.Length == 0)
			{
				behaviorTreeReference.externalBehaviors = new ExternalBehavior[1];
			}
			behaviorTreeReference.externalBehaviors[0] = externalTreeReference;
			return true;
		}

		// Token: 0x06003B30 RID: 15152 RVA: 0x0006815D File Offset: 0x0006635D
		public static T GetSharedVariable<T>(this BehaviorTree tree, string varName) where T : SharedVariable
		{
			return tree.GetVariable(varName) as T;
		}
	}
}
