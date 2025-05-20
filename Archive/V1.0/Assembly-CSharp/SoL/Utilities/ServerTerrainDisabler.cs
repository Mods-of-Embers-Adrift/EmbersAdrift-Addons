using System;
using AwesomeTechnologies.VegetationSystem;
using JBooth.MicroSplat;
using SoL.Managers;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002C9 RID: 713
	public class ServerTerrainDisabler : MonoBehaviour
	{
		// Token: 0x060014D0 RID: 5328 RVA: 0x000FB8E4 File Offset: 0x000F9AE4
		private void Awake()
		{
			if (!GameManager.IsServer)
			{
				return;
			}
			if (this.m_terrain)
			{
				this.m_terrain.enabled = false;
			}
			if (this.m_vspTerrain)
			{
				this.m_vspTerrain.enabled = false;
			}
			if (this.m_microSplatTerrain)
			{
				this.m_microSplatTerrain.enabled = false;
			}
		}

		// Token: 0x060014D1 RID: 5329 RVA: 0x0005084A File Offset: 0x0004EA4A
		private void GrabObjects()
		{
			this.m_terrain = base.gameObject.GetComponent<Terrain>();
			this.m_vspTerrain = base.gameObject.GetComponent<UnityTerrain>();
			this.m_microSplatTerrain = base.gameObject.GetComponent<MicroSplatObject>();
		}

		// Token: 0x04001D10 RID: 7440
		[SerializeField]
		private Terrain m_terrain;

		// Token: 0x04001D11 RID: 7441
		[SerializeField]
		private UnityTerrain m_vspTerrain;

		// Token: 0x04001D12 RID: 7442
		[SerializeField]
		private MicroSplatObject m_microSplatTerrain;
	}
}
