using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	// Token: 0x02000171 RID: 369
	[TaskDescription("Pursue the target specified using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=5")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}PursueIcon.png")]
	public class Pursue : NavMeshMovement
	{
		// Token: 0x06000C6A RID: 3178 RVA: 0x000D0DBC File Offset: 0x000CEFBC
		public override void OnStart()
		{
			base.OnStart();
			if (this.target.Value != null)
			{
				this.targetPosition = this.target.Value.transform.position;
			}
			this.SetDestination(this.Target());
		}

		// Token: 0x06000C6B RID: 3179 RVA: 0x0004B070 File Offset: 0x00049270
		public override TaskStatus OnUpdate()
		{
			if (this.HasArrived())
			{
				return TaskStatus.Success;
			}
			base.UpdateSpeed();
			if (base.ShouldInterrupt())
			{
				return TaskStatus.Failure;
			}
			this.SetDestination(this.Target());
			return TaskStatus.Running;
		}

		// Token: 0x06000C6C RID: 3180 RVA: 0x000D0E0C File Offset: 0x000CF00C
		private Vector3 Target()
		{
			if (this.target == null || this.target.Value == null)
			{
				return this.targetPosition;
			}
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
			Vector3 vector = this.targetPosition + (this.targetPosition - b) * d;
			if (base.gameEntity && base.gameEntity.PrimaryTargetPointDistance > 0f)
			{
				float num = Mathf.Clamp01(base.gameEntity.PrimaryTargetPointDistance / magnitude);
				vector = Vector3.Lerp(this.transform.position, vector, 1f - num);
			}
			return vector;
		}

		// Token: 0x06000C6D RID: 3181 RVA: 0x0004B09A File Offset: 0x0004929A
		public override void OnReset()
		{
			base.OnReset();
			this.targetDistPrediction = 20f;
			this.targetDistPredictionMult = 20f;
			this.target = null;
		}

		// Token: 0x04000BBC RID: 3004
		[Tooltip("How far to predict the distance ahead of the target. Lower values indicate less distance should be predicated")]
		public SharedFloat targetDistPrediction = 20f;

		// Token: 0x04000BBD RID: 3005
		[Tooltip("Multiplier for predicting the look ahead distance")]
		public SharedFloat targetDistPredictionMult = 20f;

		// Token: 0x04000BBE RID: 3006
		[Tooltip("The GameObject that the agent is pursuing")]
		public SharedGameObject target;

		// Token: 0x04000BBF RID: 3007
		private Vector3 targetPosition;
	}
}
