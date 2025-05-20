using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	// Token: 0x0200016A RID: 362
	[TaskDescription("Flock around the scene using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=13")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}FlockIcon.png")]
	public class Flock : NavMeshGroupMovement
	{
		// Token: 0x06000C39 RID: 3129 RVA: 0x000D00FC File Offset: 0x000CE2FC
		public override TaskStatus OnUpdate()
		{
			for (int i = 0; i < this.agents.Length; i++)
			{
				Vector3 a;
				Vector3 a2;
				Vector3 a3;
				this.DetermineFlockParameters(i, out a, out a2, out a3);
				Vector3 a4 = a * this.alignmentWeight.Value + a2 * this.cohesionWeight.Value + a3 * this.separationWeight.Value;
				if (!this.SetDestination(i, this.transforms[i].position + a4 * this.lookAheadDistance.Value))
				{
					a4 *= -1f;
					this.SetDestination(i, this.transforms[i].position + a4 * this.lookAheadDistance.Value);
				}
			}
			return TaskStatus.Running;
		}

		// Token: 0x06000C3A RID: 3130 RVA: 0x000D01D8 File Offset: 0x000CE3D8
		private void DetermineFlockParameters(int index, out Vector3 alignment, out Vector3 cohesion, out Vector3 separation)
		{
			alignment = (cohesion = (separation = Vector3.zero));
			int num = 0;
			Transform transform = this.transforms[index];
			for (int i = 0; i < this.agents.Length; i++)
			{
				if (index != i && Vector3.Magnitude(this.transforms[i].position - transform.position) < this.neighborDistance.Value)
				{
					alignment += this.Velocity(i);
					cohesion += this.transforms[i].position;
					separation += this.transforms[i].position - transform.position;
					num++;
				}
			}
			if (num == 0)
			{
				return;
			}
			alignment = (alignment / (float)num).normalized;
			cohesion = (cohesion / (float)num - transform.position).normalized;
			separation = (separation / (float)num * -1f).normalized;
		}

		// Token: 0x06000C3B RID: 3131 RVA: 0x000D032C File Offset: 0x000CE52C
		public override void OnReset()
		{
			base.OnReset();
			this.neighborDistance = 100f;
			this.lookAheadDistance = 5f;
			this.alignmentWeight = 0.4f;
			this.cohesionWeight = 0.5f;
			this.separationWeight = 0.6f;
		}

		// Token: 0x04000B91 RID: 2961
		[Tooltip("Agents less than this distance apart are neighbors")]
		public SharedFloat neighborDistance = 100f;

		// Token: 0x04000B92 RID: 2962
		[Tooltip("How far the agent should look ahead when determine its pathfinding destination")]
		public SharedFloat lookAheadDistance = 5f;

		// Token: 0x04000B93 RID: 2963
		[Tooltip("The greater the alignmentWeight is the more likely it is that the agents will be facing the same direction")]
		public SharedFloat alignmentWeight = 0.4f;

		// Token: 0x04000B94 RID: 2964
		[Tooltip("The greater the cohesionWeight is the more likely it is that the agents will be moving towards a common position")]
		public SharedFloat cohesionWeight = 0.5f;

		// Token: 0x04000B95 RID: 2965
		[Tooltip("The greater the separationWeight is the more likely it is that the agents will be separated")]
		public SharedFloat separationWeight = 0.6f;
	}
}
