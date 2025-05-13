using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	// Token: 0x0200016D RID: 365
	[TaskDescription("Move towards the specified position. The position can either be specified by a transform or position. If the transform is used then the position will not be used.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=1")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}MoveTowardsIcon.png")]
	public class MoveTowards : Action
	{
		// Token: 0x06000C48 RID: 3144 RVA: 0x000D0780 File Offset: 0x000CE980
		public override TaskStatus OnUpdate()
		{
			Vector3 vector = this.Target();
			if (Vector3.Magnitude(this.transform.position - vector) < this.arriveDistance.Value)
			{
				return TaskStatus.Success;
			}
			this.transform.position = Vector3.MoveTowards(this.transform.position, vector, this.speed.Value * Time.deltaTime);
			if (this.lookAtTarget.Value && (vector - this.transform.position).sqrMagnitude > 0.01f)
			{
				this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(vector - this.transform.position), this.maxLookAtRotationDelta.Value);
			}
			return TaskStatus.Running;
		}

		// Token: 0x06000C49 RID: 3145 RVA: 0x0004AE27 File Offset: 0x00049027
		private Vector3 Target()
		{
			if (this.target == null || this.target.Value == null)
			{
				return this.targetPosition.Value;
			}
			return this.target.Value.transform.position;
		}

		// Token: 0x06000C4A RID: 3146 RVA: 0x0004AE65 File Offset: 0x00049065
		public override void OnReset()
		{
			this.arriveDistance = 0.1f;
			this.lookAtTarget = true;
		}

		// Token: 0x04000BA1 RID: 2977
		[Tooltip("The speed of the agent")]
		public SharedFloat speed;

		// Token: 0x04000BA2 RID: 2978
		[Tooltip("The agent has arrived when the magnitude is less than this value")]
		public SharedFloat arriveDistance = 0.1f;

		// Token: 0x04000BA3 RID: 2979
		[Tooltip("Should the agent be looking at the target position?")]
		public SharedBool lookAtTarget = true;

		// Token: 0x04000BA4 RID: 2980
		[Tooltip("Max rotation delta if lookAtTarget is enabled")]
		public SharedFloat maxLookAtRotationDelta;

		// Token: 0x04000BA5 RID: 2981
		[Tooltip("The GameObject that the agent is moving towards")]
		public SharedGameObject target;

		// Token: 0x04000BA6 RID: 2982
		[Tooltip("If target is null then use the target position")]
		public SharedVector3 targetPosition;
	}
}
