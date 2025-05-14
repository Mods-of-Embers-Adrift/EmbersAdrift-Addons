using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	// Token: 0x02000175 RID: 373
	[TaskDescription("Seek the target specified using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=3")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SeekIcon.png")]
	public class Seek : NavMeshMovement
	{
		// Token: 0x06000C7D RID: 3197 RVA: 0x0004B152 File Offset: 0x00049352
		public override void OnStart()
		{
			base.OnStart();
			this.SetDestination(this.Target());
		}

		// Token: 0x06000C7E RID: 3198 RVA: 0x0004B167 File Offset: 0x00049367
		public override TaskStatus OnUpdate()
		{
			if (this.HasArrived())
			{
				return TaskStatus.Success;
			}
			this.SetDestination(this.Target());
			return TaskStatus.Running;
		}

		// Token: 0x06000C7F RID: 3199 RVA: 0x0004B181 File Offset: 0x00049381
		private Vector3 Target()
		{
			if (this.target.Value != null)
			{
				return this.target.Value.transform.position;
			}
			return this.targetPosition.Value;
		}

		// Token: 0x06000C80 RID: 3200 RVA: 0x0004B1B7 File Offset: 0x000493B7
		public override void OnReset()
		{
			base.OnReset();
			this.target = null;
			this.targetPosition = Vector3.zero;
		}

		// Token: 0x04000BE2 RID: 3042
		[Tooltip("The GameObject that the agent is seeking")]
		public SharedGameObject target;

		// Token: 0x04000BE3 RID: 3043
		[Tooltip("If target is null then use the target position")]
		public SharedVector3 targetPosition;
	}
}
