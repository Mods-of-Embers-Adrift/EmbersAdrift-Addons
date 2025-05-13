using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.Game
{
	// Token: 0x0200061C RID: 1564
	public class PrefabSpawner : MonoBehaviour
	{
		// Token: 0x06003193 RID: 12691 RVA: 0x0015D274 File Offset: 0x0015B474
		private void Awake()
		{
			if (this.m_prefab != null)
			{
				Transform parent = this.m_transformOverride ? this.m_transformOverride : base.gameObject.transform;
				UnityEngine.Object.Instantiate<GameObject>(this.m_prefab, parent).transform.localPosition = this.m_localPosition;
			}
		}

		// Token: 0x04002FFB RID: 12283
		[FormerlySerializedAs("m_nameplatePrefab")]
		[SerializeField]
		private GameObject m_prefab;

		// Token: 0x04002FFC RID: 12284
		[SerializeField]
		private Transform m_transformOverride;

		// Token: 0x04002FFD RID: 12285
		[FormerlySerializedAs("m_position")]
		[SerializeField]
		private Vector3 m_localPosition = new Vector3(0f, 1.8f, 0f);
	}
}
