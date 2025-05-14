using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SoL.Game.Settings
{
	// Token: 0x0200072D RID: 1837
	[Serializable]
	public class MapSettings
	{
		// Token: 0x0600370F RID: 14095 RVA: 0x0016AD1C File Offset: 0x00168F1C
		public bool TryGetMapPrefab(ZoneId zid, out AssetReference mapObj)
		{
			mapObj = null;
			if (this.m_mapData != null)
			{
				for (int i = 0; i < this.m_mapData.Length; i++)
				{
					if (this.m_mapData[i].Id == zid)
					{
						mapObj = this.m_mapData[i].Map;
						break;
					}
				}
			}
			return mapObj != null && mapObj.RuntimeKeyIsValid();
		}

		// Token: 0x17000C52 RID: 3154
		// (get) Token: 0x06003710 RID: 14096 RVA: 0x00065B86 File Offset: 0x00063D86
		public AssetReference WorldMap
		{
			get
			{
				return this.m_worldMap;
			}
		}

		// Token: 0x17000C53 RID: 3155
		// (get) Token: 0x06003711 RID: 14097 RVA: 0x00065B8E File Offset: 0x00063D8E
		public GameObject TeleportButton
		{
			get
			{
				return this.m_teleportButton;
			}
		}

		// Token: 0x0400359E RID: 13726
		[SerializeField]
		private AssetReference m_worldMap;

		// Token: 0x0400359F RID: 13727
		[SerializeField]
		private MapSettings.MapData[] m_mapData;

		// Token: 0x040035A0 RID: 13728
		[SerializeField]
		private GameObject m_teleportButton;

		// Token: 0x0200072E RID: 1838
		[Serializable]
		private class MapData
		{
			// Token: 0x17000C54 RID: 3156
			// (get) Token: 0x06003713 RID: 14099 RVA: 0x00065B96 File Offset: 0x00063D96
			public ZoneId Id
			{
				get
				{
					return this.m_zoneId;
				}
			}

			// Token: 0x17000C55 RID: 3157
			// (get) Token: 0x06003714 RID: 14100 RVA: 0x00065B9E File Offset: 0x00063D9E
			public AssetReference Map
			{
				get
				{
					return this.m_map;
				}
			}

			// Token: 0x040035A1 RID: 13729
			[SerializeField]
			private ZoneId m_zoneId;

			// Token: 0x040035A2 RID: 13730
			[SerializeField]
			private AssetReference m_map;
		}
	}
}
