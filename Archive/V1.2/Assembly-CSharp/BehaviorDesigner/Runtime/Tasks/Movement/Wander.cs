using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	// Token: 0x02000176 RID: 374
	[TaskDescription("Wander using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=9")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}WanderIcon.png")]
	public class Wander : NavMeshMovement
	{
		// Token: 0x06000C82 RID: 3202 RVA: 0x000D16D4 File Offset: 0x000CF8D4
		public override TaskStatus OnUpdate()
		{
			if (this.HasArrived())
			{
				if (this.maxPauseDuration.Value > 0f)
				{
					if (this.destinationReachTime == -1f)
					{
						this.destinationReachTime = Time.time;
						this.pauseTime = UnityEngine.Random.Range(this.minPauseDuration.Value, this.maxPauseDuration.Value);
					}
					if (this.destinationReachTime + this.pauseTime <= Time.time && this.TrySetTarget())
					{
						this.destinationReachTime = -1f;
					}
				}
				else
				{
					this.TrySetTarget();
				}
			}
			this.DestinationReachTime.Value = this.destinationReachTime;
			base.UpdateSpeed();
			if (base.ShouldInterrupt())
			{
				return TaskStatus.Failure;
			}
			return TaskStatus.Running;
		}

		// Token: 0x06000C83 RID: 3203 RVA: 0x000D1788 File Offset: 0x000CF988
		protected virtual bool TrySetTarget()
		{
			Vector3 a = this.transform.forward;
			bool flag = false;
			int num = this.targetRetries.Value;
			Vector3 vector = this.transform.position;
			while (!flag && num > 0)
			{
				a += UnityEngine.Random.insideUnitSphere * this.wanderRate.Value;
				vector = this.transform.position + a.normalized * UnityEngine.Random.Range(this.minWanderDistance.Value, this.maxWanderDistance.Value);
				flag = base.SamplePosition(vector);
				num--;
			}
			if (flag)
			{
				this.SetDestination(vector);
			}
			return flag;
		}

		// Token: 0x06000C84 RID: 3204 RVA: 0x000D1830 File Offset: 0x000CFA30
		public override void OnReset()
		{
			this.minWanderDistance = 20f;
			this.maxWanderDistance = 20f;
			this.wanderRate = 2f;
			this.minPauseDuration = 0f;
			this.maxPauseDuration = 0f;
			this.targetRetries = 1;
		}

		// Token: 0x04000BE4 RID: 3044
		[Tooltip("Minimum distance ahead of the current position to look ahead for a destination")]
		public SharedFloat minWanderDistance = 20f;

		// Token: 0x04000BE5 RID: 3045
		[Tooltip("Maximum distance ahead of the current position to look ahead for a destination")]
		public SharedFloat maxWanderDistance = 20f;

		// Token: 0x04000BE6 RID: 3046
		[Tooltip("The amount that the agent rotates direction")]
		public SharedFloat wanderRate = 2f;

		// Token: 0x04000BE7 RID: 3047
		[Tooltip("The minimum length of time that the agent should pause at each destination")]
		public SharedFloat minPauseDuration = 0f;

		// Token: 0x04000BE8 RID: 3048
		[Tooltip("The maximum length of time that the agent should pause at each destination (zero to disable)")]
		public SharedFloat maxPauseDuration = 0f;

		// Token: 0x04000BE9 RID: 3049
		[Tooltip("The maximum number of retries per tick (set higher if using a slow tick time)")]
		public SharedInt targetRetries = 1;

		// Token: 0x04000BEA RID: 3050
		public SharedFloat DestinationReachTime;

		// Token: 0x04000BEB RID: 3051
		private float pauseTime;

		// Token: 0x04000BEC RID: 3052
		private float destinationReachTime;
	}
}
