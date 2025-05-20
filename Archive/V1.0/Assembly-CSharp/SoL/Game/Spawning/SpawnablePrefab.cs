using System;
using SoL.Game.Objects;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.Game.Spawning
{
	// Token: 0x020006B3 RID: 1715
	[CreateAssetMenu(menuName = "SoL/Spawning/Spawnable Prefab Reference")]
	public class SpawnablePrefab : ScriptableObject
	{
		// Token: 0x17000B54 RID: 2900
		// (get) Token: 0x06003438 RID: 13368 RVA: 0x00063CF5 File Offset: 0x00061EF5
		public GameObject Prefab
		{
			get
			{
				return this.m_prefab;
			}
		}

		// Token: 0x17000B55 RID: 2901
		// (get) Token: 0x06003439 RID: 13369 RVA: 0x00063CFD File Offset: 0x00061EFD
		public MinMaxFloatRange ScaleRange
		{
			get
			{
				return this.m_scaleRange;
			}
		}

		// Token: 0x17000B56 RID: 2902
		// (get) Token: 0x0600343A RID: 13370 RVA: 0x00063D05 File Offset: 0x00061F05
		public string[] IndexDescriptions
		{
			get
			{
				return this.m_indexDescriptions;
			}
		}

		// Token: 0x0400325D RID: 12893
		[SerializeField]
		private GameObject m_prefab;

		// Token: 0x0400325E RID: 12894
		[SerializeField]
		private MinMaxFloatRange m_scaleRange = new MinMaxFloatRange(1f, 1f);

		// Token: 0x0400325F RID: 12895
		[FormerlySerializedAs("m_colorIndexDescriptions")]
		[SerializeField]
		private string[] m_indexDescriptions;
	}
}
