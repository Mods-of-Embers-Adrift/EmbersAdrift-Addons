using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	// Token: 0x02000177 RID: 375
	[TaskDescription("Check to see if the any object specified by the object list or tag is within the distance specified of the current agent.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=18")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}WithinDistanceIcon.png")]
	public class WithinDistance : Conditional
	{
		// Token: 0x06000C86 RID: 3206 RVA: 0x000D190C File Offset: 0x000CFB0C
		public override void OnStart()
		{
			this.sqrMagnitude = this.magnitude.Value * this.magnitude.Value;
			if (this.objects != null)
			{
				this.objects.Clear();
			}
			else
			{
				this.objects = new List<GameObject>();
			}
			if (!(this.targetObject.Value == null))
			{
				this.objects.Add(this.targetObject.Value);
				return;
			}
			if (!string.IsNullOrEmpty(this.targetTag.Value))
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag(this.targetTag.Value);
				for (int i = 0; i < array.Length; i++)
				{
					this.objects.Add(array[i]);
				}
				return;
			}
			Collider[] array2 = Physics.OverlapSphere(this.transform.position, this.magnitude.Value, this.objectLayerMask.value);
			for (int j = 0; j < array2.Length; j++)
			{
				this.objects.Add(array2[j].gameObject);
			}
		}

		// Token: 0x06000C87 RID: 3207 RVA: 0x000D1A0C File Offset: 0x000CFC0C
		public override TaskStatus OnUpdate()
		{
			if (this.transform == null || this.objects == null)
			{
				return TaskStatus.Failure;
			}
			for (int i = 0; i < this.objects.Count; i++)
			{
				if (!(this.objects[i] == null) && Vector3.SqrMagnitude(this.objects[i].transform.position - (this.transform.position + this.offset.Value)) < this.sqrMagnitude)
				{
					if (!this.lineOfSight.Value)
					{
						this.returnedObject.Value = this.objects[i];
						return TaskStatus.Success;
					}
					if (MovementUtility.LineOfSight(this.transform, this.offset.Value, this.objects[i], this.targetOffset.Value, this.usePhysics2D, this.ignoreLayerMask.value))
					{
						this.returnedObject.Value = this.objects[i];
						return TaskStatus.Success;
					}
				}
			}
			return TaskStatus.Failure;
		}

		// Token: 0x06000C88 RID: 3208 RVA: 0x000D1B30 File Offset: 0x000CFD30
		public override void OnReset()
		{
			this.usePhysics2D = false;
			this.targetObject = null;
			this.targetTag = string.Empty;
			this.objectLayerMask = 0;
			this.magnitude = 5f;
			this.lineOfSight = true;
			this.ignoreLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
			this.offset = Vector3.zero;
			this.targetOffset = Vector3.zero;
		}

		// Token: 0x06000C89 RID: 3209 RVA: 0x0004475B File Offset: 0x0004295B
		public override void OnDrawGizmos()
		{
		}

		// Token: 0x04000BED RID: 3053
		[Tooltip("Should the 2D version be used?")]
		public bool usePhysics2D;

		// Token: 0x04000BEE RID: 3054
		[Tooltip("The object that we are searching for")]
		public SharedGameObject targetObject;

		// Token: 0x04000BEF RID: 3055
		[Tooltip("The tag of the object that we are searching for")]
		public SharedString targetTag;

		// Token: 0x04000BF0 RID: 3056
		[Tooltip("The LayerMask of the objects that we are searching for")]
		public LayerMask objectLayerMask;

		// Token: 0x04000BF1 RID: 3057
		[Tooltip("The distance that the object needs to be within")]
		public SharedFloat magnitude = 5f;

		// Token: 0x04000BF2 RID: 3058
		[Tooltip("If true, the object must be within line of sight to be within distance. For example, if this option is enabled then an object behind a wall will not be within distance even though it may be physically close to the other object")]
		public SharedBool lineOfSight;

		// Token: 0x04000BF3 RID: 3059
		[Tooltip("The LayerMask of the objects to ignore when performing the line of sight check")]
		public LayerMask ignoreLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");

		// Token: 0x04000BF4 RID: 3060
		[Tooltip("The raycast offset relative to the pivot position")]
		public SharedVector3 offset;

		// Token: 0x04000BF5 RID: 3061
		[Tooltip("The target raycast offset relative to the pivot position")]
		public SharedVector3 targetOffset;

		// Token: 0x04000BF6 RID: 3062
		[Tooltip("The object variable that will be set when a object is found what the object is")]
		public SharedGameObject returnedObject;

		// Token: 0x04000BF7 RID: 3063
		private List<GameObject> objects;

		// Token: 0x04000BF8 RID: 3064
		private float sqrMagnitude;
	}
}
