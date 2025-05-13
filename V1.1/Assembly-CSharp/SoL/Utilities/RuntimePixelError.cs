using System;
using SoL.Game;
using SoL.Managers;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002C0 RID: 704
	public class RuntimePixelError : MonoBehaviour
	{
		// Token: 0x060014BC RID: 5308 RVA: 0x00050739 File Offset: 0x0004E939
		private void Awake()
		{
			if (this.m_terrain && GameManager.IsOnline)
			{
				SceneCompositionManager.ZoneLoaded += this.SceneCompositionManagerOnZoneLoaded;
			}
		}

		// Token: 0x060014BD RID: 5309 RVA: 0x00050760 File Offset: 0x0004E960
		private void OnDestroy()
		{
			if (this.m_terrain && GameManager.IsOnline)
			{
				SceneCompositionManager.ZoneLoaded -= this.SceneCompositionManagerOnZoneLoaded;
			}
		}

		// Token: 0x060014BE RID: 5310 RVA: 0x000FB698 File Offset: 0x000F9898
		private void SceneCompositionManagerOnZoneLoaded(ZoneId zoneId)
		{
			for (int i = 0; i < this.m_settings.Length; i++)
			{
				if (this.m_settings[i].ZoneId == zoneId)
				{
					this.m_terrain.heightmapPixelError = (float)this.m_settings[i].PixelError;
					Debug.Log(string.Concat(new string[]
					{
						"Setting Pixel Error on ",
						this.m_terrain.name,
						" to ",
						this.m_settings[i].PixelError.ToString(),
						" for ",
						zoneId.ToString()
					}));
					return;
				}
			}
		}

		// Token: 0x04001CF5 RID: 7413
		[SerializeField]
		private Terrain m_terrain;

		// Token: 0x04001CF6 RID: 7414
		[SerializeField]
		private RuntimePixelError.PixelErrorZone[] m_settings;

		// Token: 0x020002C1 RID: 705
		[Serializable]
		private class PixelErrorZone
		{
			// Token: 0x17000510 RID: 1296
			// (get) Token: 0x060014C0 RID: 5312 RVA: 0x00050787 File Offset: 0x0004E987
			public int PixelError
			{
				get
				{
					return this.m_pixelError;
				}
			}

			// Token: 0x17000511 RID: 1297
			// (get) Token: 0x060014C1 RID: 5313 RVA: 0x0005078F File Offset: 0x0004E98F
			public ZoneId ZoneId
			{
				get
				{
					return this.m_zoneId;
				}
			}

			// Token: 0x04001CF7 RID: 7415
			[Range(10f, 200f)]
			[SerializeField]
			private int m_pixelError = 10;

			// Token: 0x04001CF8 RID: 7416
			[SerializeField]
			private ZoneId m_zoneId;
		}
	}
}
