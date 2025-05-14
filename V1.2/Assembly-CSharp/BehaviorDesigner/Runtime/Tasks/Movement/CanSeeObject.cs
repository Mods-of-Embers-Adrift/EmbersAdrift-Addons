using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	// Token: 0x02000166 RID: 358
	[TaskDescription("Check to see if the any objects are within sight of the agent.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=11")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CanSeeObjectIcon.png")]
	public class CanSeeObject : Conditional
	{
		// Token: 0x06000C24 RID: 3108 RVA: 0x000CF574 File Offset: 0x000CD774
		public override TaskStatus OnUpdate()
		{
			if (this.usePhysics2D)
			{
				if (this.targetObjects.Value != null && this.targetObjects.Value.Count > 0)
				{
					GameObject value = null;
					float num = float.PositiveInfinity;
					for (int i = 0; i < this.targetObjects.Value.Count; i++)
					{
						float num2;
						GameObject gameObject;
						if ((gameObject = MovementUtility.WithinSight(this.transform, this.offset.Value, this.fieldOfViewAngle.Value, this.viewDistance.Value, this.targetObjects.Value[i], this.targetOffset.Value, true, this.angleOffset2D.Value, out num2, this.ignoreLayerMask, this.useTargetBone.Value, this.targetBone)) != null && num2 < num)
						{
							num = num2;
							value = gameObject;
						}
					}
					this.returnedObject.Value = value;
				}
				else if (this.targetObject.Value == null)
				{
					this.returnedObject.Value = MovementUtility.WithinSight2D(this.transform, this.offset.Value, this.fieldOfViewAngle.Value, this.viewDistance.Value, this.objectLayerMask, this.targetOffset.Value, this.angleOffset2D.Value, this.ignoreLayerMask);
				}
				else if (!string.IsNullOrEmpty(this.targetTag.Value))
				{
					this.returnedObject.Value = MovementUtility.WithinSight2D(this.transform, this.offset.Value, this.fieldOfViewAngle.Value, this.viewDistance.Value, GameObject.FindGameObjectWithTag(this.targetTag.Value), this.targetOffset.Value, this.angleOffset2D.Value, this.ignoreLayerMask, this.useTargetBone.Value, this.targetBone);
				}
				else
				{
					this.returnedObject.Value = MovementUtility.WithinSight2D(this.transform, this.offset.Value, this.fieldOfViewAngle.Value, this.viewDistance.Value, this.targetObject.Value, this.targetOffset.Value, this.angleOffset2D.Value, this.ignoreLayerMask, this.useTargetBone.Value, this.targetBone);
				}
			}
			else if (this.targetObjects.Value != null && this.targetObjects.Value.Count > 0)
			{
				GameObject value2 = null;
				float num3 = float.PositiveInfinity;
				for (int j = 0; j < this.targetObjects.Value.Count; j++)
				{
					float num4;
					GameObject gameObject2;
					if ((gameObject2 = MovementUtility.WithinSight(this.transform, this.offset.Value, this.fieldOfViewAngle.Value, this.viewDistance.Value, this.targetObjects.Value[j], this.targetOffset.Value, false, this.angleOffset2D.Value, out num4, this.ignoreLayerMask, this.useTargetBone.Value, this.targetBone)) != null && num4 < num3)
					{
						num3 = num4;
						value2 = gameObject2;
					}
				}
				this.returnedObject.Value = value2;
			}
			else if (this.targetObject.Value == null)
			{
				this.returnedObject.Value = MovementUtility.WithinSight(this.transform, this.offset.Value, this.fieldOfViewAngle.Value, this.viewDistance.Value, this.objectLayerMask, this.targetOffset.Value, this.ignoreLayerMask, this.useTargetBone.Value, this.targetBone);
			}
			else if (!string.IsNullOrEmpty(this.targetTag.Value))
			{
				this.returnedObject.Value = MovementUtility.WithinSight(this.transform, this.offset.Value, this.fieldOfViewAngle.Value, this.viewDistance.Value, GameObject.FindGameObjectWithTag(this.targetTag.Value), this.targetOffset.Value, this.ignoreLayerMask, this.useTargetBone.Value, this.targetBone);
			}
			else
			{
				this.returnedObject.Value = MovementUtility.WithinSight(this.transform, this.offset.Value, this.fieldOfViewAngle.Value, this.viewDistance.Value, this.targetObject.Value, this.targetOffset.Value, this.ignoreLayerMask, this.useTargetBone.Value, this.targetBone);
			}
			if (this.returnedObject.Value != null)
			{
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}

		// Token: 0x06000C25 RID: 3109 RVA: 0x000CFA44 File Offset: 0x000CDC44
		public override void OnReset()
		{
			this.fieldOfViewAngle = 90f;
			this.viewDistance = 1000f;
			this.offset = Vector3.zero;
			this.targetOffset = Vector3.zero;
			this.angleOffset2D = 0f;
			this.targetTag = "";
		}

		// Token: 0x06000C26 RID: 3110 RVA: 0x000CFAB4 File Offset: 0x000CDCB4
		public override void OnDrawGizmos()
		{
			MovementUtility.DrawLineOfSight(base.Owner.transform, this.offset.Value, this.fieldOfViewAngle.Value, this.angleOffset2D.Value, this.viewDistance.Value, this.usePhysics2D);
		}

		// Token: 0x06000C27 RID: 3111 RVA: 0x0004AC59 File Offset: 0x00048E59
		public override void OnBehaviorComplete()
		{
			base.OnBehaviorComplete();
			MovementUtility.ClearCache();
		}

		// Token: 0x04000B6D RID: 2925
		[Tooltip("Should the 2D version be used?")]
		public bool usePhysics2D;

		// Token: 0x04000B6E RID: 2926
		[Tooltip("The object that we are searching for")]
		public SharedGameObject targetObject;

		// Token: 0x04000B6F RID: 2927
		[Tooltip("The objects that we are searching for")]
		public SharedGameObjectList targetObjects;

		// Token: 0x04000B70 RID: 2928
		[Tooltip("The tag of the object that we are searching for")]
		public SharedString targetTag;

		// Token: 0x04000B71 RID: 2929
		[Tooltip("The LayerMask of the objects that we are searching for")]
		public LayerMask objectLayerMask;

		// Token: 0x04000B72 RID: 2930
		[Tooltip("The LayerMask of the objects to ignore when performing the line of sight check")]
		public LayerMask ignoreLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");

		// Token: 0x04000B73 RID: 2931
		[Tooltip("The field of view angle of the agent (in degrees)")]
		public SharedFloat fieldOfViewAngle = 90f;

		// Token: 0x04000B74 RID: 2932
		[Tooltip("The distance that the agent can see")]
		public SharedFloat viewDistance = 1000f;

		// Token: 0x04000B75 RID: 2933
		[Tooltip("The raycast offset relative to the pivot position")]
		public SharedVector3 offset;

		// Token: 0x04000B76 RID: 2934
		[Tooltip("The target raycast offset relative to the pivot position")]
		public SharedVector3 targetOffset;

		// Token: 0x04000B77 RID: 2935
		[Tooltip("The offset to apply to 2D angles")]
		public SharedFloat angleOffset2D;

		// Token: 0x04000B78 RID: 2936
		[Tooltip("Should the target bone be used?")]
		public SharedBool useTargetBone;

		// Token: 0x04000B79 RID: 2937
		[Tooltip("The target's bone if the target is a humanoid")]
		public HumanBodyBones targetBone;

		// Token: 0x04000B7A RID: 2938
		[Tooltip("The object that is within sight")]
		public SharedGameObject returnedObject;
	}
}
