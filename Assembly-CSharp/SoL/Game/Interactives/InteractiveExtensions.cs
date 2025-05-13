using System;
using System.Collections.Generic;
using SoL.Game.Crafting;
using SoL.Game.Objects.Archetypes;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B94 RID: 2964
	public static class InteractiveExtensions
	{
		// Token: 0x06005B43 RID: 23363 RVA: 0x00051849 File Offset: 0x0004FA49
		public static bool TryGetAsType<T>(this IInteractive interactive, out T outType) where T : class, IInteractive
		{
			outType = (interactive as T);
			return outType != null;
		}

		// Token: 0x1700155F RID: 5471
		// (get) Token: 0x06005B44 RID: 23364 RVA: 0x0007D53F File Offset: 0x0007B73F
		private static Collider[] m_hits
		{
			get
			{
				return Hits.Colliders50;
			}
		}

		// Token: 0x06005B45 RID: 23365 RVA: 0x001EE4E8 File Offset: 0x001EC6E8
		public static void GetNearbyInteractives(Vector3 position, float range, List<IInteractive> interactives)
		{
			InteractiveExtensions.m_uniqueGameObjects.Clear();
			interactives.Clear();
			int num = Physics.OverlapSphereNonAlloc(position, range, InteractiveExtensions.m_hits, LayerMap.Interaction.LayerMask, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < num; i++)
			{
				IInteractive interactive;
				if (InteractiveExtensions.m_hits[i].gameObject.TryGetComponent<IInteractive>(out interactive) && interactive.gameObject && !InteractiveExtensions.m_uniqueGameObjects.Contains(interactive.gameObject))
				{
					interactives.Add(interactive);
				}
			}
			InteractiveExtensions.m_uniqueGameObjects.Clear();
		}

		// Token: 0x06005B46 RID: 23366 RVA: 0x001EE574 File Offset: 0x001EC774
		public static GroundTorch GetNearbyGroundTorch(Vector3 position, float range)
		{
			int num = Physics.OverlapSphereNonAlloc(position, range, InteractiveExtensions.m_hits, LayerMap.Detection.LayerMask, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < num; i++)
			{
				GameEntity gameEntity;
				GroundTorch result;
				if (DetectionCollider.TryGetEntityForCollider(InteractiveExtensions.m_hits[i], out gameEntity) && gameEntity.Type == GameEntityType.Interactive && gameEntity.gameObject.TryGetComponent<GroundTorch>(out result))
				{
					return result;
				}
			}
			return null;
		}

		// Token: 0x06005B47 RID: 23367 RVA: 0x001EE5D8 File Offset: 0x001EC7D8
		public static IRefinementStation GetNearbyRefinementStation(Vector3 position, float range)
		{
			int num = Physics.OverlapSphereNonAlloc(position, range, InteractiveExtensions.m_hits, LayerMap.Detection.LayerMask, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < num; i++)
			{
				GameEntity gameEntity;
				IRefinementStation result;
				if (DetectionCollider.TryGetEntityForCollider(InteractiveExtensions.m_hits[i], out gameEntity) && gameEntity.Type == GameEntityType.Interactive && gameEntity.gameObject.TryGetComponent<IRefinementStation>(out result))
				{
					return result;
				}
			}
			return null;
		}

		// Token: 0x04004FBE RID: 20414
		private static readonly HashSet<GameObject> m_uniqueGameObjects = new HashSet<GameObject>(10);
	}
}
