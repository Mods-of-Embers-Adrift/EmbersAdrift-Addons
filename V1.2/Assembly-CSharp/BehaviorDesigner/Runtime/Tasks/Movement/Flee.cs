using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	// Token: 0x02000169 RID: 361
	[TaskDescription("Flee from the target specified using the Unity NavMesh.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=4")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}FleeIcon.png")]
	public class Flee : NavMeshMovement
	{
		// Token: 0x17000377 RID: 887
		// (get) Token: 0x06000C32 RID: 3122 RVA: 0x0004ACBE File Offset: 0x00048EBE
		private Vector3 TargetPosition
		{
			get
			{
				if (!(this.target.Value == null))
				{
					return this.target.Value.transform.position;
				}
				return this.localFleePosition;
			}
		}

		// Token: 0x06000C33 RID: 3123 RVA: 0x0004ACEF File Offset: 0x00048EEF
		public override void OnStart()
		{
			base.OnStart();
			this.hasMoved = false;
			this.localFleePosition = this.gameObject.transform.position + UnityEngine.Random.insideUnitSphere;
			this.SetDestination(this.Target());
		}

		// Token: 0x06000C34 RID: 3124 RVA: 0x000D001C File Offset: 0x000CE21C
		public override TaskStatus OnUpdate()
		{
			if (this.transform == null)
			{
				return TaskStatus.Failure;
			}
			if (Vector3.Magnitude(this.transform.position - this.TargetPosition) > this.fleedDistance.Value)
			{
				return TaskStatus.Success;
			}
			if (this.HasArrived())
			{
				if (!this.hasMoved)
				{
					return TaskStatus.Failure;
				}
				if (!this.SetDestination(this.Target()))
				{
					return TaskStatus.Failure;
				}
				this.hasMoved = false;
			}
			else
			{
				float sqrMagnitude = this.Velocity().sqrMagnitude;
				if (this.hasMoved && sqrMagnitude <= 0f)
				{
					return TaskStatus.Failure;
				}
				this.hasMoved = (sqrMagnitude > 0f);
				base.UpdateSpeed();
				if (base.ShouldInterrupt())
				{
					return TaskStatus.Failure;
				}
			}
			return TaskStatus.Running;
		}

		// Token: 0x06000C35 RID: 3125 RVA: 0x000D00D0 File Offset: 0x000CE2D0
		private Vector3 Target()
		{
			return this.transform.position + (this.transform.position - this.TargetPosition).normalized * this.lookAheadDistance.Value;
		}

		// Token: 0x06000C36 RID: 3126 RVA: 0x0004AD2B File Offset: 0x00048F2B
		protected override bool SetDestination(Vector3 destination)
		{
			return base.SamplePosition(destination) && base.SetDestination(destination);
		}

		// Token: 0x06000C37 RID: 3127 RVA: 0x0004AD3F File Offset: 0x00048F3F
		public override void OnReset()
		{
			base.OnReset();
			this.fleedDistance = 20f;
			this.lookAheadDistance = 5f;
			this.target = null;
		}

		// Token: 0x04000B8C RID: 2956
		[Tooltip("The agent has fleed when the magnitude is greater than this value")]
		public SharedFloat fleedDistance = 20f;

		// Token: 0x04000B8D RID: 2957
		[Tooltip("The distance to look ahead when fleeing")]
		public SharedFloat lookAheadDistance = 5f;

		// Token: 0x04000B8E RID: 2958
		[Tooltip("The GameObject that the agent is fleeing from")]
		public SharedGameObject target;

		// Token: 0x04000B8F RID: 2959
		private bool hasMoved;

		// Token: 0x04000B90 RID: 2960
		private Vector3 localFleePosition;
	}
}
