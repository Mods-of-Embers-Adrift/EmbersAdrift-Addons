using System;
using System.Collections.Generic;
using AwesomeTechnologies;
using AwesomeTechnologies.VegetationSystem.Biomes;
using PepijnWillekens.EasyWallColliderUnity;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x0200024C RID: 588
	public class BorderMatchMask : MonoBehaviour
	{
		// Token: 0x0600133A RID: 4922 RVA: 0x000E8DAC File Offset: 0x000E6FAC
		private void CopyMaskPointsToCollider()
		{
			if (this.m_easyWallCollider == null)
			{
				return;
			}
			foreach (object obj in this.m_biomeMaskArea.gameObject.transform)
			{
				UnityEngine.Object.DestroyImmediate(((Transform)obj).gameObject);
			}
			float y = this.m_easyWallCollider.gameObject.transform.position.y;
			if (this.m_biomeMaskArea)
			{
				using (List<AwesomeTechnologies.VegetationSystem.Biomes.Node>.Enumerator enumerator2 = this.m_biomeMaskArea.Nodes.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						AwesomeTechnologies.VegetationSystem.Biomes.Node node = enumerator2.Current;
						Vector3 position = this.m_biomeMaskArea.gameObject.transform.TransformPoint(new Vector3(node.Position.x, y, node.Position.z));
						Quaternion identity = Quaternion.identity;
						GameObject gameObject = new GameObject("Point");
						gameObject.transform.SetPositionAndRotation(position, identity);
						gameObject.transform.SetParent(this.m_easyWallCollider.gameObject.transform);
					}
					return;
				}
			}
			if (this.m_vegetationMask)
			{
				foreach (AwesomeTechnologies.Node node2 in this.m_vegetationMask.Nodes)
				{
					Vector3 position2 = this.m_vegetationMask.gameObject.transform.TransformPoint(new Vector3(node2.Position.x, y, node2.Position.z));
					Quaternion identity2 = Quaternion.identity;
					GameObject gameObject2 = new GameObject("Point");
					gameObject2.transform.SetPositionAndRotation(position2, identity2);
					gameObject2.transform.SetParent(this.m_easyWallCollider.gameObject.transform);
				}
			}
		}

		// Token: 0x040010F3 RID: 4339
		[SerializeField]
		private EasyWallCollider m_easyWallCollider;

		// Token: 0x040010F4 RID: 4340
		[SerializeField]
		private BiomeMaskArea m_biomeMaskArea;

		// Token: 0x040010F5 RID: 4341
		[SerializeField]
		private VegetationMask m_vegetationMask;
	}
}
