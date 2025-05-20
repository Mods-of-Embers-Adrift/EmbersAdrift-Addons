using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Game.SkyDome
{
	// Token: 0x020006FE RID: 1790
	public class LightColorSync : MonoBehaviour
	{
		// Token: 0x060035F4 RID: 13812 RVA: 0x00167E1C File Offset: 0x0016601C
		private void OnEnable()
		{
			if (this.m_light)
			{
				if (this.m_lightData == null)
				{
					this.m_lightData = this.m_light.gameObject.GetComponent<HDAdditionalLightData>();
				}
				if (this.m_lightData)
				{
					ColorSyncManager.RegisterLight(this.m_lightData);
				}
			}
		}

		// Token: 0x060035F5 RID: 13813 RVA: 0x00064FCD File Offset: 0x000631CD
		private void OnDisable()
		{
			if (this.m_lightData)
			{
				ColorSyncManager.DeregisterLight(this.m_lightData);
			}
		}

		// Token: 0x040033E0 RID: 13280
		[SerializeField]
		private Light m_light;

		// Token: 0x040033E1 RID: 13281
		private HDAdditionalLightData m_lightData;
	}
}
