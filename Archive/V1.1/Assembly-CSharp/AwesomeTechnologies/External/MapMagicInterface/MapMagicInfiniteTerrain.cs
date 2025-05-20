using System;
using AwesomeTechnologies.VegetationSystem;
using UnityEngine;

namespace AwesomeTechnologies.External.MapMagicInterface
{
	// Token: 0x020001F9 RID: 505
	public class MapMagicInfiniteTerrain : MonoBehaviour
	{
		// Token: 0x06001165 RID: 4453 RVA: 0x0004475B File Offset: 0x0004295B
		private void OnEnable()
		{
		}

		// Token: 0x06001166 RID: 4454 RVA: 0x0004475B File Offset: 0x0004295B
		private void OnDisable()
		{
		}

		// Token: 0x06001167 RID: 4455 RVA: 0x000E3A70 File Offset: 0x000E1C70
		private void OnGenerateCompleted(Terrain terrain)
		{
			UnityTerrain unityTerrain = terrain.gameObject.GetComponent<UnityTerrain>();
			if (!unityTerrain)
			{
				unityTerrain = terrain.gameObject.AddComponent<UnityTerrain>();
			}
			unityTerrain.TerrainPosition = terrain.transform.position;
			unityTerrain.AutoAddToVegegetationSystem = true;
			unityTerrain.AddTerrainToVegetationSystem();
		}
	}
}
