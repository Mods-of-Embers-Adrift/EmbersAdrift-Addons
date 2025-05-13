using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	// Token: 0x02000174 RID: 372
	[TaskDescription("Search for a target by combining the wander, within hearing range, and the within seeing range tasks using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=10")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SearchIcon.png")]
	public class Search : NavMeshMovement
	{
		// Token: 0x06000C79 RID: 3193 RVA: 0x000D1314 File Offset: 0x000CF514
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
			this.returnedObject.Value = MovementUtility.WithinSight(this.transform, this.offset.Value, this.fieldOfViewAngle.Value, this.viewDistance.Value, this.objectLayerMask, this.targetOffset.Value, this.ignoreLayerMask, this.useTargetBone.Value, this.targetBone);
			if (this.returnedObject.Value != null)
			{
				return TaskStatus.Success;
			}
			if (this.senseAudio.Value)
			{
				this.returnedObject.Value = MovementUtility.WithinHearingRange(this.transform, this.offset.Value, this.audibilityThreshold.Value, this.hearingRadius.Value, this.objectLayerMask);
				if (this.returnedObject.Value != null)
				{
					return TaskStatus.Success;
				}
			}
			return TaskStatus.Running;
		}

		// Token: 0x06000C7A RID: 3194 RVA: 0x000D1478 File Offset: 0x000CF678
		private bool TrySetTarget()
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

		// Token: 0x06000C7B RID: 3195 RVA: 0x000D1520 File Offset: 0x000CF720
		public override void OnReset()
		{
			base.OnReset();
			this.minWanderDistance = 20f;
			this.maxWanderDistance = 20f;
			this.wanderRate = 2f;
			this.minPauseDuration = 0f;
			this.maxPauseDuration = 0f;
			this.targetRetries = 1;
			this.fieldOfViewAngle = 90f;
			this.viewDistance = 30f;
			this.senseAudio = true;
			this.hearingRadius = 30f;
			this.audibilityThreshold = 0.05f;
		}

		// Token: 0x04000BCE RID: 3022
		[Tooltip("Minimum distance ahead of the current position to look ahead for a destination")]
		public SharedFloat minWanderDistance = 20f;

		// Token: 0x04000BCF RID: 3023
		[Tooltip("Maximum distance ahead of the current position to look ahead for a destination")]
		public SharedFloat maxWanderDistance = 20f;

		// Token: 0x04000BD0 RID: 3024
		[Tooltip("The amount that the agent rotates direction")]
		public SharedFloat wanderRate = 1f;

		// Token: 0x04000BD1 RID: 3025
		[Tooltip("The minimum length of time that the agent should pause at each destination")]
		public SharedFloat minPauseDuration = 0f;

		// Token: 0x04000BD2 RID: 3026
		[Tooltip("The maximum length of time that the agent should pause at each destination (zero to disable)")]
		public SharedFloat maxPauseDuration = 0f;

		// Token: 0x04000BD3 RID: 3027
		[Tooltip("The maximum number of retries per tick (set higher if using a slow tick time)")]
		public SharedInt targetRetries = 1;

		// Token: 0x04000BD4 RID: 3028
		[Tooltip("The field of view angle of the agent (in degrees)")]
		public SharedFloat fieldOfViewAngle = 90f;

		// Token: 0x04000BD5 RID: 3029
		[Tooltip("The distance that the agent can see")]
		public SharedFloat viewDistance = 30f;

		// Token: 0x04000BD6 RID: 3030
		[Tooltip("The LayerMask of the objects to ignore when performing the line of sight check")]
		public LayerMask ignoreLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");

		// Token: 0x04000BD7 RID: 3031
		[Tooltip("Should the search end if audio was heard?")]
		public SharedBool senseAudio = true;

		// Token: 0x04000BD8 RID: 3032
		[Tooltip("How far away the unit can hear")]
		public SharedFloat hearingRadius = 30f;

		// Token: 0x04000BD9 RID: 3033
		[Tooltip("The raycast offset relative to the pivot position")]
		public SharedVector3 offset;

		// Token: 0x04000BDA RID: 3034
		[Tooltip("The target raycast offset relative to the pivot position")]
		public SharedVector3 targetOffset;

		// Token: 0x04000BDB RID: 3035
		[Tooltip("The LayerMask of the objects that we are searching for")]
		public LayerMask objectLayerMask;

		// Token: 0x04000BDC RID: 3036
		[Tooltip("Should the target bone be used?")]
		public SharedBool useTargetBone;

		// Token: 0x04000BDD RID: 3037
		[Tooltip("The target's bone if the target is a humanoid")]
		public HumanBodyBones targetBone;

		// Token: 0x04000BDE RID: 3038
		[Tooltip("The further away a sound source is the less likely the agent will be able to hear it. Set a threshold for the the minimum audibility level that the agent can hear")]
		public SharedFloat audibilityThreshold = 0.05f;

		// Token: 0x04000BDF RID: 3039
		[Tooltip("The object that is found")]
		public SharedGameObject returnedObject;

		// Token: 0x04000BE0 RID: 3040
		private float pauseTime;

		// Token: 0x04000BE1 RID: 3041
		private float destinationReachTime;
	}
}
