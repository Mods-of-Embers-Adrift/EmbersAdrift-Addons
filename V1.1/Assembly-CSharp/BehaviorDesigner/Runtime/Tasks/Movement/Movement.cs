using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	// Token: 0x02000163 RID: 355
	public abstract class Movement : Action
	{
		// Token: 0x06000C09 RID: 3081
		protected abstract bool SetDestination(Vector3 destination);

		// Token: 0x06000C0A RID: 3082
		protected abstract void UpdateRotation(bool update);

		// Token: 0x06000C0B RID: 3083
		protected abstract bool HasPath();

		// Token: 0x06000C0C RID: 3084
		protected abstract Vector3 Velocity();

		// Token: 0x06000C0D RID: 3085
		protected abstract bool HasArrived();

		// Token: 0x06000C0E RID: 3086
		protected abstract void Stop();
	}
}
