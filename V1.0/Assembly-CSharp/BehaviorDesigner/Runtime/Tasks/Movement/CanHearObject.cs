using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	// Token: 0x02000165 RID: 357
	[TaskDescription("Check to see if the any objects are within hearing range of the current agent.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=12")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CanHearObjectIcon.png")]
	public class CanHearObject : Conditional
	{
		// Token: 0x06000C1F RID: 3103 RVA: 0x000CF324 File Offset: 0x000CD524
		public override TaskStatus OnUpdate()
		{
			if (this.targetObjects.Value != null && this.targetObjects.Value.Count > 0)
			{
				GameObject value = null;
				for (int i = 0; i < this.targetObjects.Value.Count; i++)
				{
					float num = 0f;
					GameObject gameObject;
					if (Vector3.Distance(this.targetObjects.Value[i].transform.position, this.transform.position) < this.hearingRadius.Value && (gameObject = MovementUtility.WithinHearingRange(this.transform, this.offset.Value, this.audibilityThreshold.Value, this.targetObjects.Value[i], ref num)) != null)
					{
						value = gameObject;
					}
				}
				this.returnedObject.Value = value;
			}
			else if (this.targetObject.Value == null)
			{
				if (this.usePhysics2D)
				{
					this.returnedObject.Value = MovementUtility.WithinHearingRange2D(this.transform, this.offset.Value, this.audibilityThreshold.Value, this.hearingRadius.Value, this.objectLayerMask);
				}
				else
				{
					this.returnedObject.Value = MovementUtility.WithinHearingRange(this.transform, this.offset.Value, this.audibilityThreshold.Value, this.hearingRadius.Value, this.objectLayerMask);
				}
			}
			else
			{
				GameObject gameObject2;
				if (!string.IsNullOrEmpty(this.targetTag.Value))
				{
					gameObject2 = GameObject.FindGameObjectWithTag(this.targetTag.Value);
				}
				else
				{
					gameObject2 = this.targetObject.Value;
				}
				if (Vector3.Distance(gameObject2.transform.position, this.transform.position) < this.hearingRadius.Value)
				{
					this.returnedObject.Value = MovementUtility.WithinHearingRange(this.transform, this.offset.Value, this.audibilityThreshold.Value, this.targetObject.Value);
				}
			}
			if (this.returnedObject.Value != null)
			{
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}

		// Token: 0x06000C20 RID: 3104 RVA: 0x0004AC37 File Offset: 0x00048E37
		public override void OnReset()
		{
			this.hearingRadius = 50f;
			this.audibilityThreshold = 0.05f;
		}

		// Token: 0x06000C21 RID: 3105 RVA: 0x0004475B File Offset: 0x0004295B
		public override void OnDrawGizmos()
		{
		}

		// Token: 0x06000C22 RID: 3106 RVA: 0x0004AC59 File Offset: 0x00048E59
		public override void OnBehaviorComplete()
		{
			base.OnBehaviorComplete();
			MovementUtility.ClearCache();
		}

		// Token: 0x04000B64 RID: 2916
		[Tooltip("Should the 2D version be used?")]
		public bool usePhysics2D;

		// Token: 0x04000B65 RID: 2917
		[Tooltip("The object that we are searching for")]
		public SharedGameObject targetObject;

		// Token: 0x04000B66 RID: 2918
		[Tooltip("The objects that we are searching for")]
		public SharedGameObjectList targetObjects;

		// Token: 0x04000B67 RID: 2919
		[Tooltip("The tag of the object that we are searching for")]
		public SharedString targetTag;

		// Token: 0x04000B68 RID: 2920
		[Tooltip("The LayerMask of the objects that we are searching for")]
		public LayerMask objectLayerMask;

		// Token: 0x04000B69 RID: 2921
		[Tooltip("How far away the unit can hear")]
		public SharedFloat hearingRadius = 50f;

		// Token: 0x04000B6A RID: 2922
		[Tooltip("The further away a sound source is the less likely the agent will be able to hear it. Set a threshold for the the minimum audibility level that the agent can hear")]
		public SharedFloat audibilityThreshold = 0.05f;

		// Token: 0x04000B6B RID: 2923
		[Tooltip("The hearing offset relative to the pivot position")]
		public SharedVector3 offset;

		// Token: 0x04000B6C RID: 2924
		[Tooltip("The returned object that is heard")]
		public SharedGameObject returnedObject;
	}
}
