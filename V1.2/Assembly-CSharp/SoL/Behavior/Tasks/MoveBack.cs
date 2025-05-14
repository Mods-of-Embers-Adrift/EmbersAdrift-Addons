using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using SoL.Game.NPCs;
using UnityEngine;

namespace SoL.Behavior.Tasks
{
	// Token: 0x020004DF RID: 1247
	[TaskDescription("Move back from the target specified using the Unity NavMesh.")]
	[TaskCategory("SoL/Npc")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}FleeIcon.png")]
	public class MoveBack : BaseNodeMovement<NpcSkillsController>
	{
		// Token: 0x060022C9 RID: 8905 RVA: 0x000590B6 File Offset: 0x000572B6
		private Vector3 GetMyPos()
		{
			if (!this.m_controller)
			{
				return this.transform.position;
			}
			return this.m_controller.PrimaryTargetPoint.transform.position;
		}

		// Token: 0x060022CA RID: 8906 RVA: 0x000590E6 File Offset: 0x000572E6
		public override void OnStart()
		{
			base.OnStart();
			this.hasMoved = false;
			this.SetDestination(this.Target());
		}

		// Token: 0x060022CB RID: 8907 RVA: 0x00127C1C File Offset: 0x00125E1C
		private Vector3 Target()
		{
			if (this.target.Value == null)
			{
				return this.transform.position;
			}
			Vector3 myPos = this.GetMyPos();
			Vector3 a = myPos - this.target.Value.transform.position;
			float magnitude = a.magnitude;
			Vector3 a2 = ((double)magnitude > 9.999999747378752E-06) ? (a / magnitude) : Vector3.zero;
			float d = Mathf.Clamp(this.MoveBackDistance.Value * 1.02f - magnitude, 0f, float.MaxValue);
			return myPos + a2 * d;
		}

		// Token: 0x060022CC RID: 8908 RVA: 0x00127CC0 File Offset: 0x00125EC0
		public override TaskStatus OnUpdate()
		{
			if (this.transform == null || this.target.Value == null)
			{
				return TaskStatus.Failure;
			}
			if (Vector3.Magnitude(this.GetMyPos() - this.target.Value.transform.position) >= this.MoveBackDistance.Value)
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
			if (this.AttackWhileMovingBack.Value && this.TargetEntity.Value && this.m_controller)
			{
				this.m_controller.TryExecuteCombatAbility(this.TargetEntity.Value);
			}
			return TaskStatus.Running;
		}

		// Token: 0x040026AA RID: 9898
		public SharedGameObject target;

		// Token: 0x040026AB RID: 9899
		public SharedGameEntity TargetEntity;

		// Token: 0x040026AC RID: 9900
		public SharedFloat MoveBackDistance;

		// Token: 0x040026AD RID: 9901
		public SharedBool AttackWhileMovingBack;

		// Token: 0x040026AE RID: 9902
		private bool hasMoved;
	}
}
