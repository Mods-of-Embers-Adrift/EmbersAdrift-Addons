using System;
using SoL.Game.Interactives;
using SoL.Utilities;
using SoL.Utilities.Extensions;
using UMA;
using UMA.Dynamics;
using UnityEngine;

namespace SoL.Game.UMA
{
	// Token: 0x02000623 RID: 1571
	public class SolUMAPhysicsAvatar : MonoBehaviour
	{
		// Token: 0x060031A8 RID: 12712 RVA: 0x0015D4D4 File Offset: 0x0015B6D4
		public void Init(UMAData _umaData, UMAPhysicsElement[] elements, int targetLayer, GameEntity entity)
		{
			if (this.m_initialized)
			{
				return;
			}
			if (_umaData == null)
			{
				_umaData = base.gameObject.GetComponent<UMAData>();
			}
			if (_umaData == null)
			{
				if (Debug.isDebugBuild)
				{
					Debug.LogError("CreatePhysicsObjects: umaData is null!");
				}
				return;
			}
			foreach (UMAPhysicsElement umaphysicsElement in elements)
			{
				if (umaphysicsElement != null)
				{
					GameObject boneGameObject = _umaData.GetBoneGameObject(umaphysicsElement.boneName);
					if (boneGameObject == null)
					{
						if (Debug.isDebugBuild)
						{
							Debug.LogWarning("UMAPhysics: " + umaphysicsElement.boneName + " not found!");
						}
					}
					else
					{
						boneGameObject.layer = targetLayer;
						foreach (ColliderDefinition colliderDefinition in umaphysicsElement.colliders)
						{
							if (colliderDefinition.colliderType == ColliderDefinition.ColliderType.Box)
							{
								BoxCollider boxCollider = boneGameObject.AddComponent<BoxCollider>();
								boxCollider.center = colliderDefinition.colliderCentre;
								boxCollider.size = colliderDefinition.boxDimensions;
								boxCollider.isTrigger = false;
							}
							else if (colliderDefinition.colliderType == ColliderDefinition.ColliderType.Sphere)
							{
								SphereCollider sphereCollider = boneGameObject.AddComponent<SphereCollider>();
								sphereCollider.center = colliderDefinition.colliderCentre;
								sphereCollider.radius = colliderDefinition.sphereRadius;
								sphereCollider.isTrigger = false;
							}
							else if (colliderDefinition.colliderType == ColliderDefinition.ColliderType.Capsule)
							{
								CapsuleCollider capsuleCollider = boneGameObject.AddComponent<CapsuleCollider>();
								capsuleCollider.center = colliderDefinition.colliderCentre;
								capsuleCollider.radius = colliderDefinition.capsuleRadius;
								capsuleCollider.height = colliderDefinition.capsuleHeight;
								capsuleCollider.isTrigger = false;
								switch (colliderDefinition.capsuleAlignment)
								{
								case ColliderDefinition.Direction.X:
									capsuleCollider.direction = 0;
									break;
								case ColliderDefinition.Direction.Y:
									capsuleCollider.direction = 1;
									break;
								case ColliderDefinition.Direction.Z:
									capsuleCollider.direction = 2;
									break;
								default:
									capsuleCollider.direction = 0;
									break;
								}
							}
						}
						if (entity != null)
						{
							InteractiveForwarder orAddComponent = boneGameObject.GetOrAddComponent<InteractiveForwarder>();
							if (orAddComponent != null)
							{
								orAddComponent.Init(entity.gameObject);
							}
							if (umaphysicsElement.boneName == "Head")
							{
								boneGameObject.AddComponent<UpperLineOfSightTarget>();
							}
						}
					}
				}
			}
			this.m_initialized = true;
		}

		// Token: 0x04003012 RID: 12306
		private const bool kIsTrigger = false;

		// Token: 0x04003013 RID: 12307
		private bool m_initialized;
	}
}
