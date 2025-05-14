using System;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	// Token: 0x0200016C RID: 364
	[TaskDescription("Follow the leader using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=14")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}LeaderFollowIcon.png")]
	public class LeaderFollow : NavMeshGroupMovement
	{
		// Token: 0x06000C41 RID: 3137 RVA: 0x0004ADCD File Offset: 0x00048FCD
		public override void OnStart()
		{
			this.leaderTransform = this.leader.Value.transform;
			this.leaderAgent = this.leader.Value.GetComponent<NavMeshAgent>();
			base.OnStart();
		}

		// Token: 0x06000C42 RID: 3138 RVA: 0x000D0520 File Offset: 0x000CE720
		public override TaskStatus OnUpdate()
		{
			Vector3 a = this.LeaderBehindPosition();
			for (int i = 0; i < this.agents.Length; i++)
			{
				if (this.LeaderLookingAtAgent(i) && Vector3.Magnitude(this.leaderTransform.position - this.transforms[i].position) < this.aheadDistance.Value)
				{
					this.SetDestination(i, this.transforms[i].position + (this.transforms[i].position - this.leaderTransform.position).normalized * this.aheadDistance.Value);
				}
				else
				{
					this.SetDestination(i, a + this.DetermineSeparation(i));
				}
			}
			return TaskStatus.Running;
		}

		// Token: 0x06000C43 RID: 3139 RVA: 0x000D05F0 File Offset: 0x000CE7F0
		private Vector3 LeaderBehindPosition()
		{
			return this.leaderTransform.position + (-this.leaderAgent.velocity).normalized * this.leaderBehindDistance.Value;
		}

		// Token: 0x06000C44 RID: 3140 RVA: 0x000D0638 File Offset: 0x000CE838
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

		// Token: 0x06000C45 RID: 3141 RVA: 0x0004AE01 File Offset: 0x00049001
		public bool LeaderLookingAtAgent(int agentIndex)
		{
			return Vector3.Dot(this.leaderTransform.forward, this.transforms[agentIndex].forward) < -0.5f;
		}

		// Token: 0x06000C46 RID: 3142 RVA: 0x000D06F0 File Offset: 0x000CE8F0
		public override void OnReset()
		{
			base.OnReset();
			this.neighborDistance = 10f;
			this.leaderBehindDistance = 2f;
			this.separationDistance = 2f;
			this.aheadDistance = 2f;
			this.leader = null;
		}

		// Token: 0x04000B9A RID: 2970
		[Tooltip("Agents less than this distance apart are neighbors")]
		public SharedFloat neighborDistance = 10f;

		// Token: 0x04000B9B RID: 2971
		[Tooltip("How far behind the leader the agents should follow the leader")]
		public SharedFloat leaderBehindDistance = 2f;

		// Token: 0x04000B9C RID: 2972
		[Tooltip("The distance that the agents should be separated")]
		public SharedFloat separationDistance = 2f;

		// Token: 0x04000B9D RID: 2973
		[Tooltip("The agent is getting too close to the front of the leader if they are within the aheadDistance")]
		public SharedFloat aheadDistance = 2f;

		// Token: 0x04000B9E RID: 2974
		[Tooltip("The leader to follow")]
		public SharedGameObject leader;

		// Token: 0x04000B9F RID: 2975
		private Transform leaderTransform;

		// Token: 0x04000BA0 RID: 2976
		private NavMeshAgent leaderAgent;
	}
}
