using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	// Token: 0x02000170 RID: 368
	[TaskDescription("Patrol around the specified waypoints using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=7")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}PatrolIcon.png")]
	public class Patrol : NavMeshMovement
	{
		// Token: 0x06000C64 RID: 3172 RVA: 0x000D0BE8 File Offset: 0x000CEDE8
		public override void OnStart()
		{
			base.OnStart();
			float num = float.PositiveInfinity;
			for (int i = 0; i < this.waypoints.Value.Count; i++)
			{
				float num2;
				if ((num2 = Vector3.Magnitude(this.transform.position - this.waypoints.Value[i].transform.position)) < num)
				{
					num = num2;
					this.waypointIndex = i;
				}
			}
			this.waypointReachedTime = -1f;
			this.SetDestination(this.Target());
		}

		// Token: 0x06000C65 RID: 3173 RVA: 0x000D0C74 File Offset: 0x000CEE74
		public override TaskStatus OnUpdate()
		{
			if (this.waypoints.Value.Count == 0)
			{
				return TaskStatus.Failure;
			}
			if (this.HasArrived())
			{
				if (this.waypointReachedTime == -1f)
				{
					this.waypointReachedTime = Time.time;
				}
				if (this.waypointReachedTime + this.waypointPauseDuration.Value <= Time.time)
				{
					if (this.randomPatrol.Value)
					{
						if (this.waypoints.Value.Count == 1)
						{
							this.waypointIndex = 0;
						}
						else
						{
							int num;
							for (num = this.waypointIndex; num == this.waypointIndex; num = UnityEngine.Random.Range(0, this.waypoints.Value.Count))
							{
							}
							this.waypointIndex = num;
						}
					}
					else
					{
						this.waypointIndex = (this.waypointIndex + 1) % this.waypoints.Value.Count;
					}
					this.SetDestination(this.Target());
					this.waypointReachedTime = -1f;
				}
			}
			return TaskStatus.Running;
		}

		// Token: 0x06000C66 RID: 3174 RVA: 0x000D0D68 File Offset: 0x000CEF68
		private Vector3 Target()
		{
			if (this.waypointIndex >= this.waypoints.Value.Count)
			{
				return this.transform.position;
			}
			return this.waypoints.Value[this.waypointIndex].transform.position;
		}

		// Token: 0x06000C67 RID: 3175 RVA: 0x0004B021 File Offset: 0x00049221
		public override void OnReset()
		{
			base.OnReset();
			this.randomPatrol = false;
			this.waypointPauseDuration = 0f;
			this.waypoints = null;
		}

		// Token: 0x06000C68 RID: 3176 RVA: 0x0004475B File Offset: 0x0004295B
		public override void OnDrawGizmos()
		{
		}

		// Token: 0x04000BB7 RID: 2999
		[Tooltip("Should the agent patrol the waypoints randomly?")]
		public SharedBool randomPatrol = false;

		// Token: 0x04000BB8 RID: 3000
		[Tooltip("The length of time that the agent should pause when arriving at a waypoint")]
		public SharedFloat waypointPauseDuration = 0f;

		// Token: 0x04000BB9 RID: 3001
		[Tooltip("The waypoints to move to")]
		public SharedGameObjectList waypoints;

		// Token: 0x04000BBA RID: 3002
		private int waypointIndex;

		// Token: 0x04000BBB RID: 3003
		private float waypointReachedTime;
	}
}
