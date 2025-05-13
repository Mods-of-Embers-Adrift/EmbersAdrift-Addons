using System;
using System.Collections.Generic;
using SoL.Managers;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Audio
{
	// Token: 0x02000D1A RID: 3354
	public class TerrainSoundController : MonoBehaviour
	{
		// Token: 0x06006501 RID: 25857 RVA: 0x00083FEC File Offset: 0x000821EC
		public static bool TryGetTerrainSoundController(GameObject go, out TerrainSoundController controller)
		{
			controller = null;
			return TerrainSoundController.m_terrainSoundControllers != null && TerrainSoundController.m_terrainSoundControllers.TryGetValue(go, out controller);
		}

		// Token: 0x1700183C RID: 6204
		// (get) Token: 0x06006502 RID: 25858 RVA: 0x00084006 File Offset: 0x00082206
		public Dictionary<int, GroundSurfaceType> SurfaceTypeDict
		{
			get
			{
				return this.m_surfaceTypeDict;
			}
		}

		// Token: 0x1700183D RID: 6205
		// (get) Token: 0x06006503 RID: 25859 RVA: 0x0008400E File Offset: 0x0008220E
		public Terrain Terrain
		{
			get
			{
				return this.m_terrain;
			}
		}

		// Token: 0x06006504 RID: 25860 RVA: 0x0020B568 File Offset: 0x00209768
		private void Awake()
		{
			if (GameManager.IsServer)
			{
				return;
			}
			this.m_terrain = base.gameObject.GetComponent<Terrain>();
			if (this.m_terrain == null)
			{
				return;
			}
			if (TerrainSoundController.m_terrainSoundControllers == null)
			{
				TerrainSoundController.m_terrainSoundControllers = new Dictionary<GameObject, TerrainSoundController>();
			}
			TerrainSoundController.m_terrainSoundControllers.AddOrReplace(base.gameObject, this);
			this.m_surfaceTypeDict = new Dictionary<int, GroundSurfaceType>(this.m_surfaceTypes.Length);
			for (int i = 0; i < this.m_surfaceTypes.Length; i++)
			{
				this.SurfaceTypeDict.Add(i, this.m_surfaceTypes[i]);
			}
		}

		// Token: 0x06006505 RID: 25861 RVA: 0x00084016 File Offset: 0x00082216
		private void OnDestroy()
		{
			if (GameManager.IsServer)
			{
				return;
			}
			TerrainSoundController.m_terrainSoundControllers.Remove(base.gameObject);
		}

		// Token: 0x040057C8 RID: 22472
		private static Dictionary<GameObject, TerrainSoundController> m_terrainSoundControllers;

		// Token: 0x040057C9 RID: 22473
		[SerializeField]
		private GroundSurfaceType[] m_surfaceTypes;

		// Token: 0x040057CA RID: 22474
		private Dictionary<int, GroundSurfaceType> m_surfaceTypeDict;

		// Token: 0x040057CB RID: 22475
		private Terrain m_terrain;
	}
}
