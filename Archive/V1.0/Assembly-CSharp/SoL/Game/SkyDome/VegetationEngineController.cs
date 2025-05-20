using System;
using SoL.Utilities.Extensions;
using TheVisualEngine;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.Game.SkyDome
{
	// Token: 0x02000716 RID: 1814
	public class VegetationEngineController : MonoBehaviour
	{
		// Token: 0x06003673 RID: 13939 RVA: 0x0016998C File Offset: 0x00167B8C
		private void Start()
		{
			if (ZoneSettings.SettingsProfile != null)
			{
				if (this.m_seasonalColorEvergreen)
				{
					ZoneSettingsProfile.GlobalElement seasonalColorsEvergreen = ZoneSettings.SettingsProfile.SeasonalColorsEvergreen;
					if (seasonalColorsEvergreen != null)
					{
						seasonalColorsEvergreen.SetupElement(this.m_seasonalColorEvergreen);
					}
				}
				if (this.m_seasonalColorDeciduous)
				{
					ZoneSettingsProfile.GlobalElement seasonalColorsDeciduous = ZoneSettings.SettingsProfile.SeasonalColorsDeciduous;
					if (seasonalColorsDeciduous != null)
					{
						seasonalColorsDeciduous.SetupElement(this.m_seasonalColorDeciduous);
					}
				}
				if (this.m_seasonalAlpha)
				{
					ZoneSettingsProfile.GlobalElement seasonalAlpha = ZoneSettings.SettingsProfile.SeasonalAlpha;
					if (seasonalAlpha != null)
					{
						seasonalAlpha.SetupElement(this.m_seasonalAlpha);
					}
				}
			}
			this.UpdateSeason();
		}

		// Token: 0x06003674 RID: 13940 RVA: 0x000654E8 File Offset: 0x000636E8
		private void Update()
		{
			this.UpdateSeason();
		}

		// Token: 0x06003675 RID: 13941 RVA: 0x00169A24 File Offset: 0x00167C24
		private void CopyTransform(GameObject source, GameObject target)
		{
			if (source && target)
			{
				target.transform.position = source.transform.position;
				target.transform.rotation = source.transform.rotation;
				target.transform.localScale = source.transform.localScale;
			}
		}

		// Token: 0x06003676 RID: 13942 RVA: 0x00169A84 File Offset: 0x00167C84
		private void UpdateSeason()
		{
			if (!this.m_updateSeasons)
			{
				return;
			}
			DateTime correctedGameDateTime = SkyDomeManager.GetCorrectedGameDateTime();
			float num = Mathf.Lerp(0f, 4f, correctedGameDateTime.GetYearFraction());
			if (this.m_tveManager)
			{
				this.m_tveManager.seasonControl = num;
			}
			if (ZoneSettings.SettingsProfile)
			{
				Color tint = Color.white;
				if (num >= 0f && num < 1f)
				{
					tint = Color.Lerp(this.m_winterTerrainTint, this.m_springTerrainTint, num);
				}
				else if (num >= 1f && num < 2f)
				{
					tint = Color.Lerp(this.m_springTerrainTint, this.m_summerTerrainTint, num - 1f);
				}
				else if (num >= 2f && num < 3f)
				{
					tint = Color.Lerp(this.m_summerTerrainTint, this.m_fallTerrainTint, num - 2f);
				}
				else if (num >= 3f && num <= 4f)
				{
					tint = Color.Lerp(this.m_fallTerrainTint, this.m_winterTerrainTint, num - 3f);
				}
				ZoneSettings.SettingsProfile.UpdateTerrainTextures(tint);
			}
		}

		// Token: 0x06003677 RID: 13943 RVA: 0x000654F0 File Offset: 0x000636F0
		internal void SetWetness(float wetness)
		{
			if (this.m_tveManager && this.m_tveManager.globalAtmoData != null)
			{
				this.m_tveManager.globalAtmoData.wetnessIntensity = wetness;
			}
		}

		// Token: 0x06003678 RID: 13944 RVA: 0x0006551D File Offset: 0x0006371D
		internal void SetSnow(float snow)
		{
			if (this.m_tveManager && this.m_tveManager.globalAtmoData != null)
			{
				this.m_tveManager.globalAtmoData.overlayIntensity = snow;
			}
		}

		// Token: 0x0400345C RID: 13404
		private const string kTveObjects = "TVE Objects";

		// Token: 0x0400345D RID: 13405
		private const string kTerrainTint = "Terrain Tint";

		// Token: 0x0400345E RID: 13406
		[SerializeField]
		private bool m_updateSeasons;

		// Token: 0x0400345F RID: 13407
		[FormerlySerializedAs("m_seasonalColor")]
		[SerializeField]
		private GameObject m_seasonalColorEvergreen;

		// Token: 0x04003460 RID: 13408
		[SerializeField]
		private GameObject m_seasonalColorDeciduous;

		// Token: 0x04003461 RID: 13409
		[SerializeField]
		private GameObject m_seasonalAlpha;

		// Token: 0x04003462 RID: 13410
		[SerializeField]
		private Color m_winterTerrainTint = Color.white;

		// Token: 0x04003463 RID: 13411
		[SerializeField]
		private Color m_springTerrainTint = Color.white;

		// Token: 0x04003464 RID: 13412
		[SerializeField]
		private Color m_summerTerrainTint = Color.white;

		// Token: 0x04003465 RID: 13413
		[SerializeField]
		private Color m_fallTerrainTint = Color.white;

		// Token: 0x04003466 RID: 13414
		[SerializeField]
		private TVEManager m_tveManager;
	}
}
