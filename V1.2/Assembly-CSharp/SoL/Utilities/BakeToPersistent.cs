using System;
using AwesomeTechnologies.Vegetation.PersistentStorage;
using AwesomeTechnologies.VegetationSystem;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000249 RID: 585
	public class BakeToPersistent : MonoBehaviour
	{
		// Token: 0x06001331 RID: 4913 RVA: 0x0004475B File Offset: 0x0004295B
		private void Bake()
		{
		}

		// Token: 0x06001332 RID: 4914 RVA: 0x000E8D44 File Offset: 0x000E6F44
		private VegetationItemInfoPro GetVegetationId(GameObject prefabRef)
		{
			foreach (VegetationItemInfoPro vegetationItemInfoPro in this.m_vegetationPackage.VegetationInfoList)
			{
				if (vegetationItemInfoPro.VegetationPrefab == prefabRef)
				{
					return vegetationItemInfoPro;
				}
			}
			return null;
		}

		// Token: 0x040010ED RID: 4333
		[SerializeField]
		private GameObject m_parent;

		// Token: 0x040010EE RID: 4334
		[SerializeField]
		private VegetationPackagePro m_vegetationPackage;

		// Token: 0x040010EF RID: 4335
		[SerializeField]
		private PersistentVegetationStorage m_persistentVegetationStorage;

		// Token: 0x040010F0 RID: 4336
		private const byte kSourceId = 200;
	}
}
