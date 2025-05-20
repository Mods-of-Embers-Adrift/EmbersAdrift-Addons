using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	// Token: 0x02000168 RID: 360
	[TaskDescription("Evade the target specified using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=6")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}EvadeIcon.png")]
	public class Evade : NavMeshMovement
	{
		// Token: 0x06000C2D RID: 3117 RVA: 0x0004AC8E File Offset: 0x00048E8E
		public override void OnStart()
		{
			base.OnStart();
			this.targetPosition = this.target.Value.transform.position;
			this.SetDestination(this.Target());
		}

		// Token: 0x06000C2E RID: 3118 RVA: 0x000CFE1C File Offset: 0x000CE01C
		public override TaskStatus OnUpdate()
		{
			if (Vector3.Magnitude(this.transform.position - this.target.Value.transform.position) > this.evadeDistance.Value)
			{
				return TaskStatus.Success;
			}
			this.SetDestination(this.Target());
			return TaskStatus.Running;
		}

		// Token: 0x06000C2F RID: 3119 RVA: 0x000CFE70 File Offset: 0x000CE070
		private Vector3 Target()
		{
			float magnitude = (this.target.Value.transform.position - this.transform.position).magnitude;
			float magnitude2 = this.Velocity().magnitude;
			float d;
			if (magnitude2 <= magnitude / this.targetDistPrediction.Value)
			{
				d = this.targetDistPrediction.Value;
			}
			else
			{
				d = magnitude / magnitude2 * this.targetDistPredictionMult.Value;
			}
			Vector3 b = this.targetPosition;
			this.targetPosition = this.target.Value.transform.position;
			Vector3 b2 = this.targetPosition + (this.targetPosition - b) * d;
			return this.transform.position + (this.transform.position - b2).normalized * this.lookAheadDistance.Value;
		}

		// Token: 0x06000C30 RID: 3120 RVA: 0x000CFF6C File Offset: 0x000CE16C
		public override void OnReset()
		{
			base.OnReset();
			this.evadeDistance = 10f;
			this.lookAheadDistance = 5f;
			this.targetDistPrediction = 20f;
			this.targetDistPredictionMult = 20f;
			this.target = null;
		}

		// Token: 0x04000B86 RID: 2950
		[Tooltip("The agent has evaded when the magnitude is greater than this value")]
		public SharedFloat evadeDistance = 10f;

		// Token: 0x04000B87 RID: 2951
		[Tooltip("The distance to look ahead when evading")]
		public SharedFloat lookAheadDistance = 5f;

		// Token: 0x04000B88 RID: 2952
		[Tooltip("How far to predict the distance ahead of the target. Lower values indicate less distance should be predicated")]
		public SharedFloat targetDistPrediction = 20f;

		// Token: 0x04000B89 RID: 2953
		[Tooltip("Multiplier for predicting the look ahead distance")]
		public SharedFloat targetDistPredictionMult = 20f;

		// Token: 0x04000B8A RID: 2954
		[Tooltip("The GameObject that the agent is evading")]
		public SharedGameObject target;

		// Token: 0x04000B8B RID: 2955
		private Vector3 targetPosition;
	}
}
