using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	// Token: 0x02000172 RID: 370
	[TaskDescription("Queue in a line using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=15")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}QueueIcon.png")]
	public class Queue : NavMeshGroupMovement
	{
		// Token: 0x06000C6F RID: 3183 RVA: 0x000D0F60 File Offset: 0x000CF160
		public override TaskStatus OnUpdate()
		{
			for (int i = 0; i < this.agents.Length; i++)
			{
				if (this.AgentAhead(i))
				{
					this.SetDestination(i, this.transforms[i].position + this.transforms[i].forward * this.slowDownSpeed.Value + this.DetermineSeparation(i));
				}
				else
				{
					this.SetDestination(i, this.target.Value.transform.position);
				}
			}
			return TaskStatus.Running;
		}

		// Token: 0x06000C70 RID: 3184 RVA: 0x000D0FEC File Offset: 0x000CF1EC
		private bool AgentAhead(int index)
		{
			Vector3 a = this.Velocity(index) * this.maxQueueAheadDistance.Value;
			for (int i = 0; i < this.agents.Length; i++)
			{
				if (index != i && Vector3.SqrMagnitude(a - this.transforms[i].position) < this.maxQueueRadius.Value)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000C71 RID: 3185 RVA: 0x000D1050 File Offset: 0x000CF250
		private Vector3 DetermineSeparation(int agentIndex)
		{
			Vector3 a = Vector3.zero;
			int num = 0;
			Transform transform = this.transforms[agentIndex];
			for (int i = 0; i < this.agents.Length; i++)
			{
				if (agentIndex != i && Vector3.SqrMagnitude(this.transforms[i].position - transform.position) < this.neighborDistance.Value)
				{
					a += this.transforms[i].position - transform.position;
					num++;
				}
			}
			if (num == 0)
			{
				return Vector3.zero;
			}
			return (a / (float)num * -1f).normalized * this.separationDistance.Value;
		}

		// Token: 0x06000C72 RID: 3186 RVA: 0x000D1108 File Offset: 0x000CF308
		public override void OnReset()
		{
			base.OnReset();
			this.neighborDistance = 10f;
			this.separationDistance = 2f;
			this.maxQueueAheadDistance = 2f;
			this.maxQueueRadius = 20f;
			this.slowDownSpeed = 0.15f;
		}

		// Token: 0x04000BC0 RID: 3008
		[Tooltip("Agents less than this distance apart are neighbors")]
		public SharedFloat neighborDistance = 10f;

		// Token: 0x04000BC1 RID: 3009
		[Tooltip("The distance that the agents should be separated")]
		public SharedFloat separationDistance = 2f;

		// Token: 0x04000BC2 RID: 3010
		[Tooltip("The distance the the agent should look ahead to see if another agent is in the way")]
		public SharedFloat maxQueueAheadDistance = 2f;

		// Token: 0x04000BC3 RID: 3011
		[Tooltip("The radius that the agent should check to see if another agent is in the way")]
		public SharedFloat maxQueueRadius = 20f;

		// Token: 0x04000BC4 RID: 3012
		[Tooltip("The multiplier to slow down if an agent is in front of the current agent")]
		public SharedFloat slowDownSpeed = 0.15f;

		// Token: 0x04000BC5 RID: 3013
		[Tooltip("The target to seek towards")]
		public SharedGameObject target;
	}
}
