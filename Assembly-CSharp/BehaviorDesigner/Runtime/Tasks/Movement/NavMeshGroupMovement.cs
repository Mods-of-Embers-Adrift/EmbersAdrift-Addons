using System;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	// Token: 0x0200016E RID: 366
	public abstract class NavMeshGroupMovement : GroupMovement
	{
		// Token: 0x06000C4C RID: 3148 RVA: 0x000D0850 File Offset: 0x000CEA50
		public override void OnStart()
		{
			this.navMeshAgents = new NavMeshAgent[this.agents.Length];
			this.transforms = new Transform[this.agents.Length];
			for (int i = 0; i < this.agents.Length; i++)
			{
				this.transforms[i] = this.agents[i].Value.transform;
				this.navMeshAgents[i] = this.agents[i].Value.GetComponent<NavMeshAgent>();
				this.navMeshAgents[i].speed = this.speed.Value;
				this.navMeshAgents[i].angularSpeed = this.angularSpeed.Value;
				this.navMeshAgents[i].isStopped = false;
			}
		}

		// Token: 0x06000C4D RID: 3149 RVA: 0x0004AEA7 File Offset: 0x000490A7
		protected override bool SetDestination(int index, Vector3 target)
		{
			return this.navMeshAgents[index].destination == target || this.navMeshAgents[index].SetDestination(target);
		}

		// Token: 0x06000C4E RID: 3150 RVA: 0x0004AECE File Offset: 0x000490CE
		protected override Vector3 Velocity(int index)
		{
			return this.navMeshAgents[index].velocity;
		}

		// Token: 0x06000C4F RID: 3151 RVA: 0x000D090C File Offset: 0x000CEB0C
		public override void OnEnd()
		{
			for (int i = 0; i < this.navMeshAgents.Length; i++)
			{
				if (this.navMeshAgents[i] != null)
				{
					this.navMeshAgents[i].isStopped = true;
				}
			}
		}

		// Token: 0x06000C50 RID: 3152 RVA: 0x0004AEDD File Offset: 0x000490DD
		public override void OnReset()
		{
			this.agents = null;
		}

		// Token: 0x04000BA7 RID: 2983
		[Tooltip("All of the agents")]
		public SharedGameObject[] agents;

		// Token: 0x04000BA8 RID: 2984
		[Tooltip("The speed of the agents")]
		public SharedFloat speed = 10f;

		// Token: 0x04000BA9 RID: 2985
		[Tooltip("The angular speed of the agents")]
		public SharedFloat angularSpeed = 120f;

		// Token: 0x04000BAA RID: 2986
		private NavMeshAgent[] navMeshAgents;

		// Token: 0x04000BAB RID: 2987
		protected Transform[] transforms;
	}
}
