using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	// Token: 0x0200016B RID: 363
	[TaskDescription("Follows the specified target using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=23")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}FollowIcon.png")]
	public class Follow : NavMeshMovement
	{
		// Token: 0x06000C3D RID: 3133 RVA: 0x000D03F4 File Offset: 0x000CE5F4
		public override void OnStart()
		{
			base.OnStart();
			this.lastTargetPosition = this.target.Value.transform.position + Vector3.one * (this.moveDistance.Value + 1f);
			this.hasMoved = false;
		}

		// Token: 0x06000C3E RID: 3134 RVA: 0x000D044C File Offset: 0x000CE64C
		public override TaskStatus OnUpdate()
		{
			if (this.target.Value == null)
			{
				return TaskStatus.Failure;
			}
			Vector3 position = this.target.Value.transform.position;
			if ((position - this.lastTargetPosition).magnitude >= this.moveDistance.Value)
			{
				this.SetDestination(position);
				this.lastTargetPosition = position;
				this.hasMoved = true;
			}
			else if (this.hasMoved && (position - this.transform.position).magnitude < this.moveDistance.Value)
			{
				this.Stop();
				this.hasMoved = false;
				this.lastTargetPosition = position;
			}
			return TaskStatus.Running;
		}

		// Token: 0x06000C3F RID: 3135 RVA: 0x0004AD96 File Offset: 0x00048F96
		public override void OnReset()
		{
			base.OnReset();
			this.target = null;
			this.moveDistance = 2f;
		}

		// Token: 0x04000B96 RID: 2966
		[Tooltip("The GameObject that the agent is following")]
		public SharedGameObject target;

		// Token: 0x04000B97 RID: 2967
		[Tooltip("Start moving towards the target if the target is further than the specified distance")]
		public SharedFloat moveDistance = 2f;

		// Token: 0x04000B98 RID: 2968
		private Vector3 lastTargetPosition;

		// Token: 0x04000B99 RID: 2969
		private bool hasMoved;
	}
}
