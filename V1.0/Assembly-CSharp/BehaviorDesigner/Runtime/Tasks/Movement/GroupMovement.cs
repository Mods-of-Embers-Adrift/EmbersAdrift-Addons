using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	// Token: 0x02000162 RID: 354
	public abstract class GroupMovement : Action
	{
		// Token: 0x06000C06 RID: 3078
		protected abstract bool SetDestination(int index, Vector3 target);

		// Token: 0x06000C07 RID: 3079
		protected abstract Vector3 Velocity(int index);
	}
}
