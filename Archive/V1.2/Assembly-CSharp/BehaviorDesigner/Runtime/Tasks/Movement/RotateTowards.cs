using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	// Token: 0x02000173 RID: 371
	[TaskDescription("Rotates towards the specified rotation. The rotation can either be specified by a transform or rotation. If the transform is used then the rotation will not be used.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=2")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}RotateTowardsIcon.png")]
	public class RotateTowards : Action
	{
		// Token: 0x06000C74 RID: 3188 RVA: 0x0004B0F1 File Offset: 0x000492F1
		public override void OnAwake()
		{
			this.m_transformToRotate = (this.rotateParent ? this.gameObject.transform.parent.transform : this.gameObject.transform);
		}

		// Token: 0x06000C75 RID: 3189 RVA: 0x000D11D0 File Offset: 0x000CF3D0
		public override TaskStatus OnUpdate()
		{
			Quaternion quaternion = this.Target();
			if (Quaternion.Angle(this.m_transformToRotate.rotation, quaternion) < this.rotationEpsilon.Value)
			{
				return TaskStatus.Success;
			}
			this.m_transformToRotate.rotation = Quaternion.RotateTowards(this.m_transformToRotate.rotation, quaternion, this.maxLookAtRotationDelta.Value);
			return TaskStatus.Running;
		}

		// Token: 0x06000C76 RID: 3190 RVA: 0x000D122C File Offset: 0x000CF42C
		private Quaternion Target()
		{
			if (this.target == null || this.target.Value == null)
			{
				return Quaternion.Euler(this.targetRotation.Value);
			}
			Vector3 vector = this.target.Value.transform.position - this.m_transformToRotate.position;
			if (this.onlyY.Value)
			{
				vector.y = 0f;
			}
			if (this.usePhysics2D)
			{
				return Quaternion.AngleAxis(Mathf.Atan2(vector.y, vector.x) * 57.29578f, Vector3.forward);
			}
			return Quaternion.LookRotation(vector);
		}

		// Token: 0x06000C77 RID: 3191 RVA: 0x000D12D4 File Offset: 0x000CF4D4
		public override void OnReset()
		{
			this.usePhysics2D = false;
			this.rotationEpsilon = 0.5f;
			this.maxLookAtRotationDelta = 1f;
			this.onlyY = false;
			this.target = null;
			this.targetRotation = Vector3.zero;
			this.m_transformToRotate = null;
		}

		// Token: 0x04000BC6 RID: 3014
		[Tooltip("Should the 2D version be used?")]
		public bool usePhysics2D;

		// Token: 0x04000BC7 RID: 3015
		[Tooltip("The agent is done rotating when the angle is less than this value")]
		public SharedFloat rotationEpsilon = 0.5f;

		// Token: 0x04000BC8 RID: 3016
		[Tooltip("The maximum number of angles the agent can rotate in a single tick")]
		public SharedFloat maxLookAtRotationDelta = 1f;

		// Token: 0x04000BC9 RID: 3017
		[Tooltip("Should the rotation only affect the Y axis?")]
		public SharedBool onlyY;

		// Token: 0x04000BCA RID: 3018
		[Tooltip("The GameObject that the agent is rotating towards")]
		public SharedGameObject target;

		// Token: 0x04000BCB RID: 3019
		[Tooltip("If target is null then use the target rotation")]
		public SharedVector3 targetRotation;

		// Token: 0x04000BCC RID: 3020
		public bool rotateParent = true;

		// Token: 0x04000BCD RID: 3021
		private Transform m_transformToRotate;
	}
}
