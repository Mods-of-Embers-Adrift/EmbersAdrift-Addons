using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	// Token: 0x02000164 RID: 356
	public static class MovementUtility
	{
		// Token: 0x06000C10 RID: 3088 RVA: 0x000CED8C File Offset: 0x000CCF8C
		public static GameObject WithinSight(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance, LayerMask objectLayerMask, Vector3 targetOffset, LayerMask ignoreLayerMask, bool useTargetBone, HumanBodyBones targetBone)
		{
			GameObject result = null;
			Collider[] array = Physics.OverlapSphere(transform.position, viewDistance, objectLayerMask);
			if (array != null)
			{
				float num = float.PositiveInfinity;
				for (int i = 0; i < array.Length; i++)
				{
					float num2;
					GameObject gameObject;
					if ((gameObject = MovementUtility.WithinSight(transform, positionOffset, fieldOfViewAngle, viewDistance, array[i].gameObject, targetOffset, false, 0f, out num2, ignoreLayerMask, useTargetBone, targetBone)) != null && num2 < num)
					{
						num = num2;
						result = gameObject;
					}
				}
			}
			return result;
		}

		// Token: 0x06000C11 RID: 3089 RVA: 0x000CEE04 File Offset: 0x000CD004
		public static GameObject WithinSight2D(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance, LayerMask objectLayerMask, Vector3 targetOffset, float angleOffset2D, LayerMask ignoreLayerMask)
		{
			GameObject result = null;
			Collider2D[] array = Physics2D.OverlapCircleAll(transform.position, viewDistance, objectLayerMask);
			if (array != null)
			{
				float num = float.PositiveInfinity;
				for (int i = 0; i < array.Length; i++)
				{
					float num2;
					GameObject gameObject;
					if ((gameObject = MovementUtility.WithinSight(transform, positionOffset, fieldOfViewAngle, viewDistance, array[i].gameObject, targetOffset, true, angleOffset2D, out num2, ignoreLayerMask, false, HumanBodyBones.Hips)) != null && num2 < num)
					{
						num = num2;
						result = gameObject;
					}
				}
			}
			return result;
		}

		// Token: 0x06000C12 RID: 3090 RVA: 0x000CEE7C File Offset: 0x000CD07C
		public static GameObject WithinSight(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance, GameObject targetObject, Vector3 targetOffset, LayerMask ignoreLayerMask, bool useTargetBone, HumanBodyBones targetBone)
		{
			float num;
			return MovementUtility.WithinSight(transform, positionOffset, fieldOfViewAngle, viewDistance, targetObject, targetOffset, false, 0f, out num, ignoreLayerMask, useTargetBone, targetBone);
		}

		// Token: 0x06000C13 RID: 3091 RVA: 0x000CEEAC File Offset: 0x000CD0AC
		public static GameObject WithinSight2D(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance, GameObject targetObject, Vector3 targetOffset, float angleOffset2D, LayerMask ignoreLayerMask, bool useTargetBone, HumanBodyBones targetBone)
		{
			float num;
			return MovementUtility.WithinSight(transform, positionOffset, fieldOfViewAngle, viewDistance, targetObject, targetOffset, true, angleOffset2D, out num, ignoreLayerMask, useTargetBone, targetBone);
		}

		// Token: 0x06000C14 RID: 3092 RVA: 0x000CEED8 File Offset: 0x000CD0D8
		public static GameObject WithinSight(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance, GameObject targetObject, Vector3 targetOffset, bool usePhysics2D, float angleOffset2D, out float angle, int ignoreLayerMask, bool useTargetBone, HumanBodyBones targetBone)
		{
			if (targetObject == null)
			{
				angle = 0f;
				return null;
			}
			Animator componentForType;
			if (useTargetBone && (componentForType = MovementUtility.GetComponentForType<Animator>(targetObject)) != null)
			{
				Transform boneTransform = componentForType.GetBoneTransform(targetBone);
				if (boneTransform != null)
				{
					targetObject = boneTransform.gameObject;
				}
			}
			Vector3 from = targetObject.transform.position - transform.TransformPoint(positionOffset);
			if (usePhysics2D)
			{
				Vector3 eulerAngles = transform.eulerAngles;
				eulerAngles.z -= angleOffset2D;
				angle = Vector3.Angle(from, Quaternion.Euler(eulerAngles) * Vector3.up);
				from.z = 0f;
			}
			else
			{
				angle = Vector3.Angle(from, transform.forward);
				from.y = 0f;
			}
			if (from.magnitude < viewDistance && angle < fieldOfViewAngle * 0.5f)
			{
				if (MovementUtility.LineOfSight(transform, positionOffset, targetObject, targetOffset, usePhysics2D, ignoreLayerMask) != null)
				{
					return targetObject;
				}
				if (MovementUtility.GetComponentForType<Collider>(targetObject) == null && MovementUtility.GetComponentForType<Collider2D>(targetObject) == null && targetObject.gameObject.activeSelf)
				{
					return targetObject;
				}
			}
			return null;
		}

		// Token: 0x06000C15 RID: 3093 RVA: 0x000CEFFC File Offset: 0x000CD1FC
		public static GameObject LineOfSight(Transform transform, Vector3 positionOffset, GameObject targetObject, Vector3 targetOffset, bool usePhysics2D, int ignoreLayerMask)
		{
			RaycastHit raycastHit;
			if (usePhysics2D)
			{
				RaycastHit2D raycastHit2D;
				if ((raycastHit2D = Physics2D.Linecast(transform.TransformPoint(positionOffset), targetObject.transform.TransformPoint(targetOffset), ~ignoreLayerMask)) && (raycastHit2D.transform.IsChildOf(targetObject.transform) || targetObject.transform.IsChildOf(raycastHit2D.transform)))
				{
					return targetObject;
				}
			}
			else if (Physics.Linecast(transform.TransformPoint(positionOffset), targetObject.transform.TransformPoint(targetOffset), out raycastHit, ~ignoreLayerMask) && (raycastHit.transform.IsChildOf(targetObject.transform) || targetObject.transform.IsChildOf(raycastHit.transform)))
			{
				return targetObject;
			}
			return null;
		}

		// Token: 0x06000C16 RID: 3094 RVA: 0x000CF0B0 File Offset: 0x000CD2B0
		public static GameObject WithinHearingRange(Transform transform, Vector3 positionOffset, float audibilityThreshold, float hearingRadius, LayerMask objectLayerMask)
		{
			GameObject result = null;
			Collider[] array = Physics.OverlapSphere(transform.TransformPoint(positionOffset), hearingRadius, objectLayerMask);
			if (array != null)
			{
				float num = 0f;
				for (int i = 0; i < array.Length; i++)
				{
					float num2 = 0f;
					GameObject gameObject;
					if ((gameObject = MovementUtility.WithinHearingRange(transform, positionOffset, audibilityThreshold, array[i].gameObject, ref num2)) != null && num2 > num)
					{
						num = num2;
						result = gameObject;
					}
				}
			}
			return result;
		}

		// Token: 0x06000C17 RID: 3095 RVA: 0x000CF11C File Offset: 0x000CD31C
		public static GameObject WithinHearingRange2D(Transform transform, Vector3 positionOffset, float audibilityThreshold, float hearingRadius, LayerMask objectLayerMask)
		{
			GameObject result = null;
			Collider2D[] array = Physics2D.OverlapCircleAll(transform.TransformPoint(positionOffset), hearingRadius, objectLayerMask);
			if (array != null)
			{
				float num = 0f;
				for (int i = 0; i < array.Length; i++)
				{
					float num2 = 0f;
					GameObject gameObject;
					if ((gameObject = MovementUtility.WithinHearingRange(transform, positionOffset, audibilityThreshold, array[i].gameObject, ref num2)) != null && num2 > num)
					{
						num = num2;
						result = gameObject;
					}
				}
			}
			return result;
		}

		// Token: 0x06000C18 RID: 3096 RVA: 0x000CF18C File Offset: 0x000CD38C
		public static GameObject WithinHearingRange(Transform transform, Vector3 positionOffset, float audibilityThreshold, GameObject targetObject)
		{
			float num = 0f;
			return MovementUtility.WithinHearingRange(transform, positionOffset, audibilityThreshold, targetObject, ref num);
		}

		// Token: 0x06000C19 RID: 3097 RVA: 0x000CF1AC File Offset: 0x000CD3AC
		public static GameObject WithinHearingRange(Transform transform, Vector3 positionOffset, float audibilityThreshold, GameObject targetObject, ref float audibility)
		{
			AudioSource[] componentsForType;
			if ((componentsForType = MovementUtility.GetComponentsForType<AudioSource>(targetObject)) != null)
			{
				for (int i = 0; i < componentsForType.Length; i++)
				{
					if (componentsForType[i].isPlaying)
					{
						float num = Vector3.Distance(transform.position, targetObject.transform.position);
						if (componentsForType[i].rolloffMode == AudioRolloffMode.Logarithmic)
						{
							audibility = componentsForType[i].volume / Mathf.Max(componentsForType[i].minDistance, num - componentsForType[i].minDistance);
						}
						else
						{
							audibility = componentsForType[i].volume * Mathf.Clamp01((num - componentsForType[i].minDistance) / (componentsForType[i].maxDistance - componentsForType[i].minDistance));
						}
						if (audibility > audibilityThreshold)
						{
							return targetObject;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06000C1A RID: 3098 RVA: 0x0004475B File Offset: 0x0004295B
		public static void DrawLineOfSight(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float angleOffset, float viewDistance, bool usePhysics2D)
		{
		}

		// Token: 0x06000C1B RID: 3099 RVA: 0x000CF260 File Offset: 0x000CD460
		public static T GetComponentForType<T>(GameObject target) where T : Component
		{
			Dictionary<Type, Component> dictionary;
			Component component;
			if (MovementUtility.gameObjectComponentMap.TryGetValue(target, out dictionary))
			{
				if (dictionary.TryGetValue(typeof(T), out component))
				{
					return component as T;
				}
			}
			else
			{
				dictionary = new Dictionary<Type, Component>();
				MovementUtility.gameObjectComponentMap.Add(target, dictionary);
			}
			component = target.GetComponent<T>();
			dictionary.Add(typeof(T), component);
			return component as T;
		}

		// Token: 0x06000C1C RID: 3100 RVA: 0x000CF2D8 File Offset: 0x000CD4D8
		public static T[] GetComponentsForType<T>(GameObject target) where T : Component
		{
			Dictionary<Type, Component[]> dictionary;
			Component[] array;
			if (MovementUtility.gameObjectComponentsMap.TryGetValue(target, out dictionary))
			{
				if (dictionary.TryGetValue(typeof(T), out array))
				{
					return array as T[];
				}
			}
			else
			{
				dictionary = new Dictionary<Type, Component[]>();
				MovementUtility.gameObjectComponentsMap.Add(target, dictionary);
			}
			Component[] components = target.GetComponents<T>();
			array = components;
			dictionary.Add(typeof(T), array);
			return array as T[];
		}

		// Token: 0x06000C1D RID: 3101 RVA: 0x0004AC0B File Offset: 0x00048E0B
		public static void ClearCache()
		{
			MovementUtility.gameObjectComponentMap.Clear();
			MovementUtility.gameObjectComponentsMap.Clear();
		}

		// Token: 0x04000B62 RID: 2914
		private static Dictionary<GameObject, Dictionary<Type, Component>> gameObjectComponentMap = new Dictionary<GameObject, Dictionary<Type, Component>>();

		// Token: 0x04000B63 RID: 2915
		private static Dictionary<GameObject, Dictionary<Type, Component[]>> gameObjectComponentsMap = new Dictionary<GameObject, Dictionary<Type, Component[]>>();
	}
}
