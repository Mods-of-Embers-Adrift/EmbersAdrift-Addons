using System;
using System.Collections.Generic;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Game.SkyDome
{
	// Token: 0x020006E8 RID: 1768
	public class ColorSyncManager : MonoBehaviour
	{
		// Token: 0x06003581 RID: 13697 RVA: 0x0006494E File Offset: 0x00062B4E
		public static void RegisterLight(HDAdditionalLightData light)
		{
			if (ColorSyncManager.m_lights == null)
			{
				ColorSyncManager.m_lights = new List<HDAdditionalLightData>
				{
					light
				};
				return;
			}
			if (!ColorSyncManager.m_lights.Contains(light))
			{
				ColorSyncManager.m_lights.Add(light);
			}
		}

		// Token: 0x06003582 RID: 13698 RVA: 0x00064981 File Offset: 0x00062B81
		public static void DeregisterLight(HDAdditionalLightData light)
		{
			List<HDAdditionalLightData> lights = ColorSyncManager.m_lights;
			if (lights == null)
			{
				return;
			}
			lights.Remove(light);
		}

		// Token: 0x06003583 RID: 13699 RVA: 0x00166E9C File Offset: 0x0016509C
		private void Update()
		{
			if (SkyDomeManager.SkyDomeController == null)
			{
				return;
			}
			Color b = this.m_colorGradient.Evaluate(SkyDomeUtilities.GetDayNightCycleFraction());
			if (this.m_skyLightMaterial)
			{
				Color color = Color.Lerp(this.m_skyLightMaterial.GetMainColor(), b, Time.deltaTime);
				this.m_skyLightMaterial.SetMainColor(color);
				this.m_skyLightMaterial.SetEmissionColor(color);
			}
			if (ColorSyncManager.m_lights != null && ColorSyncManager.m_lights.Count > 0)
			{
				Color color2 = Color.Lerp(ColorSyncManager.m_lights[0] ? ColorSyncManager.m_lights[0].color : Color.white, b, Time.deltaTime);
				for (int i = 0; i < ColorSyncManager.m_lights.Count; i++)
				{
					if (ColorSyncManager.m_lights[i])
					{
						ColorSyncManager.m_lights[i].SetColor(color2, -1f);
					}
				}
			}
		}

		// Token: 0x04003381 RID: 13185
		private static List<HDAdditionalLightData> m_lights;

		// Token: 0x04003382 RID: 13186
		[SerializeField]
		private Material m_skyLightMaterial;

		// Token: 0x04003383 RID: 13187
		[Tooltip("0.00 midnight\n0.25 sunrise\n0.50 noon\n0.75 sunset\n1.00 midnight")]
		[SerializeField]
		private Gradient m_colorGradient;
	}
}
