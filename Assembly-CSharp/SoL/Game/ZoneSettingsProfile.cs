using System;
using JBooth.MicroSplat;
using SoL.Game.Objects;
using SoL.Game.SkyDome;
using SoL.Networking.Proximity;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoL.Game
{
	// Token: 0x0200060F RID: 1551
	[CreateAssetMenu(menuName = "SoL/Profiles/Zone")]
	public class ZoneSettingsProfile : ScriptableObject
	{
		// Token: 0x17000A7C RID: 2684
		// (get) Token: 0x0600314B RID: 12619 RVA: 0x00061FF8 File Offset: 0x000601F8
		public int Latitude
		{
			get
			{
				return this.m_latitude;
			}
		}

		// Token: 0x17000A7D RID: 2685
		// (get) Token: 0x0600314C RID: 12620 RVA: 0x00062000 File Offset: 0x00060200
		public int Longitude
		{
			get
			{
				return this.m_longitude;
			}
		}

		// Token: 0x17000A7E RID: 2686
		// (get) Token: 0x0600314D RID: 12621 RVA: 0x00062008 File Offset: 0x00060208
		public DayNightEnableCondition DayNightCondition
		{
			get
			{
				return this.m_dayNightEnableCondition;
			}
		}

		// Token: 0x0600314E RID: 12622 RVA: 0x0015C5F0 File Offset: 0x0015A7F0
		public MinMaxFloatRange GetClipRange(float currentMaxClip)
		{
			float value = this.m_minClipPlane.GetValue(250f);
			float value2 = this.m_maxClipPlane.GetValue(currentMaxClip);
			return new MinMaxFloatRange(value, value2);
		}

		// Token: 0x0600314F RID: 12623 RVA: 0x00062010 File Offset: 0x00060210
		public bool TryGetCloudCoverageRange(out float min, out float max)
		{
			min = 0f;
			max = 0f;
			if (this.m_overrideCloudCoverage)
			{
				min = this.m_cloudCoverageRange.Min;
				max = this.m_cloudCoverageRange.Max;
			}
			return this.m_overrideCloudCoverage;
		}

		// Token: 0x06003150 RID: 12624 RVA: 0x00062048 File Offset: 0x00060248
		public bool TryGetCloudHeight(out float cloudHeight)
		{
			cloudHeight = (this.m_overrideCloudHeight ? this.m_cloudHeight : 0f);
			return this.m_overrideCloudHeight;
		}

		// Token: 0x17000A7F RID: 2687
		// (get) Token: 0x06003151 RID: 12625 RVA: 0x00062067 File Offset: 0x00060267
		public bool NpcsCheckForWater
		{
			get
			{
				return this.m_npcsCheckForWater;
			}
		}

		// Token: 0x17000A80 RID: 2688
		// (get) Token: 0x06003152 RID: 12626 RVA: 0x0006206F File Offset: 0x0006026F
		public bool IndoorCallForHelp
		{
			get
			{
				return this.m_indoorCallForHelp;
			}
		}

		// Token: 0x17000A81 RID: 2689
		// (get) Token: 0x06003153 RID: 12627 RVA: 0x00062077 File Offset: 0x00060277
		public bool IndoorSensorProfiles
		{
			get
			{
				return this.m_indoorSensorProfiles;
			}
		}

		// Token: 0x06003154 RID: 12628 RVA: 0x0006207F File Offset: 0x0006027F
		public float GetNavigationCellSize()
		{
			return this.m_navigationCellSize.GetValue(10f);
		}

		// Token: 0x06003155 RID: 12629 RVA: 0x00062091 File Offset: 0x00060291
		public bool TryGetNpcDistanceBandOverrides(out DistanceBand[] npcDistanceBands)
		{
			npcDistanceBands = null;
			if (this.m_overrideNpcDistanceBands)
			{
				npcDistanceBands = this.m_npcDistanceBands;
			}
			return npcDistanceBands != null && npcDistanceBands.Length != 0;
		}

		// Token: 0x17000A82 RID: 2690
		// (get) Token: 0x06003156 RID: 12630 RVA: 0x000620B2 File Offset: 0x000602B2
		public ZoneSettingsProfile.GlobalElement SeasonalColorsEvergreen
		{
			get
			{
				return this.m_seasonalColorsEvergreen;
			}
		}

		// Token: 0x17000A83 RID: 2691
		// (get) Token: 0x06003157 RID: 12631 RVA: 0x000620BA File Offset: 0x000602BA
		public ZoneSettingsProfile.GlobalElement SeasonalColorsDeciduous
		{
			get
			{
				return this.m_seasonalColorsDeciduous;
			}
		}

		// Token: 0x17000A84 RID: 2692
		// (get) Token: 0x06003158 RID: 12632 RVA: 0x000620C2 File Offset: 0x000602C2
		public ZoneSettingsProfile.GlobalElement SeasonalAlpha
		{
			get
			{
				return this.m_seasonalAlpha;
			}
		}

		// Token: 0x06003159 RID: 12633 RVA: 0x0015C620 File Offset: 0x0015A820
		public void UpdateTerrainTextures(Color tint)
		{
			if (!this.m_tintTerrainTextures || this.m_terrainTextureIndexes == null || this.m_terrainTextureIndexes.Length == 0 || !this.m_terrainPropData || !this.m_terrainPropData.propTex)
			{
				return;
			}
			tint.a = 0f;
			for (int i = 0; i < this.m_terrainTextureIndexes.Length; i++)
			{
				this.m_terrainPropData.SetValue(this.m_terrainTextureIndexes[i], MicroSplatPropData.PerTexColor.Tint, tint);
			}
			this.m_terrainPropData.propTex.SetPixels(this.m_terrainPropData.values);
			this.m_terrainPropData.propTex.Apply();
		}

		// Token: 0x0600315A RID: 12634 RVA: 0x000620CA File Offset: 0x000602CA
		public void ResetTerrainTextures()
		{
			this.UpdateTerrainTextures(Color.white);
		}

		// Token: 0x17000A85 RID: 2693
		// (get) Token: 0x0600315B RID: 12635 RVA: 0x000620D7 File Offset: 0x000602D7
		public bool ExtendAlchemyCooldowns
		{
			get
			{
				return this.m_extendAlchemyCooldowns;
			}
		}

		// Token: 0x0600315C RID: 12636 RVA: 0x0015C6C8 File Offset: 0x0015A8C8
		public void PostInitSkybox(ISkyDomeController skydomeController)
		{
			if (this.m_staticTime)
			{
				skydomeController.ProgressTime = false;
				DateTime time = GameDateTime.UtcNow.DateTime;
				time = new DateTime(time.Year, time.Month, time.Day, 0, 0, 0);
				time = time.AddHours((double)this.m_staticTimeValue);
				SkyDomeManager.SkyDomeController.SetTime(time);
			}
		}

		// Token: 0x04002F9C RID: 12188
		private const string kCameraGroup = "Camera";

		// Token: 0x04002F9D RID: 12189
		public OverrideFloat FieldOfView = new OverrideFloat(40f);

		// Token: 0x04002F9E RID: 12190
		[SerializeField]
		private OverrideFloat m_minClipPlane = new OverrideFloat(250f);

		// Token: 0x04002F9F RID: 12191
		[SerializeField]
		private OverrideFloat m_maxClipPlane = new OverrideFloat(1000f);

		// Token: 0x04002FA0 RID: 12192
		private const string kTimeGroup = "Time";

		// Token: 0x04002FA1 RID: 12193
		[SerializeField]
		private bool m_staticTime;

		// Token: 0x04002FA2 RID: 12194
		[Range(0f, 24f)]
		[SerializeField]
		private float m_staticTimeValue = 7f;

		// Token: 0x04002FA3 RID: 12195
		private const string kSkyDomeGroup = "Sky Dome";

		// Token: 0x04002FA4 RID: 12196
		[Range(-90f, 90f)]
		[Tooltip("Latitude coordinate of the zone.")]
		[SerializeField]
		private int m_latitude = 33;

		// Token: 0x04002FA5 RID: 12197
		[Range(-180f, 180f)]
		[Tooltip("Longitude coordinate of the zone.")]
		public int m_longitude;

		// Token: 0x04002FA6 RID: 12198
		[SerializeField]
		private DayNightEnableCondition m_dayNightEnableCondition;

		// Token: 0x04002FA7 RID: 12199
		[SerializeField]
		private bool m_overrideCloudCoverage;

		// Token: 0x04002FA8 RID: 12200
		[Tooltip("0.3f is 'regular' coverage.\n0.26 is a very light coverage.\n0.34 is almost heavy coverage.")]
		[SerializeField]
		private MinMaxFloatRange m_cloudCoverageRange = new MinMaxFloatRange(0.26f, 0.34f);

		// Token: 0x04002FA9 RID: 12201
		[SerializeField]
		private bool m_overrideCloudHeight;

		// Token: 0x04002FAA RID: 12202
		[SerializeField]
		private float m_cloudHeight = 1500f;

		// Token: 0x04002FAB RID: 12203
		private const string kNpcSettings = "Npcs";

		// Token: 0x04002FAC RID: 12204
		[SerializeField]
		private OverrideFloat m_navigationCellSize = new OverrideFloat(10f);

		// Token: 0x04002FAD RID: 12205
		[SerializeField]
		private bool m_npcsCheckForWater;

		// Token: 0x04002FAE RID: 12206
		[SerializeField]
		private bool m_indoorCallForHelp;

		// Token: 0x04002FAF RID: 12207
		[SerializeField]
		private bool m_indoorSensorProfiles;

		// Token: 0x04002FB0 RID: 12208
		[SerializeField]
		private bool m_overrideNpcDistanceBands;

		// Token: 0x04002FB1 RID: 12209
		[SerializeField]
		private DistanceBand[] m_npcDistanceBands;

		// Token: 0x04002FB2 RID: 12210
		private const string kVegetation = "Vegetation";

		// Token: 0x04002FB3 RID: 12211
		[FormerlySerializedAs("m_seasonalColors")]
		[SerializeField]
		private ZoneSettingsProfile.GlobalElement m_seasonalColorsEvergreen;

		// Token: 0x04002FB4 RID: 12212
		[SerializeField]
		private ZoneSettingsProfile.GlobalElement m_seasonalColorsDeciduous;

		// Token: 0x04002FB5 RID: 12213
		[SerializeField]
		private ZoneSettingsProfile.GlobalElement m_seasonalAlpha;

		// Token: 0x04002FB6 RID: 12214
		private const string kTerrain = "Terrain";

		// Token: 0x04002FB7 RID: 12215
		[SerializeField]
		private bool m_tintTerrainTextures;

		// Token: 0x04002FB8 RID: 12216
		[SerializeField]
		private MicroSplatPropData m_terrainPropData;

		// Token: 0x04002FB9 RID: 12217
		[SerializeField]
		private int[] m_terrainTextureIndexes;

		// Token: 0x04002FBA RID: 12218
		private const string kCityMonolithGrp = "City Monolith";

		// Token: 0x04002FBB RID: 12219
		private const int kMinutesInDay = 1440;

		// Token: 0x04002FBC RID: 12220
		[Range(0f, 1439f)]
		[SerializeField]
		private int m_timeOffset;

		// Token: 0x04002FBD RID: 12221
		[Range(0f, 1f)]
		[SerializeField]
		private float m_dayFractionDebug;

		// Token: 0x04002FBE RID: 12222
		private const string kMisc = "Misc";

		// Token: 0x04002FBF RID: 12223
		[SerializeField]
		private bool m_extendAlchemyCooldowns;

		// Token: 0x02000610 RID: 1552
		[Serializable]
		public class GlobalElement
		{
			// Token: 0x0600315E RID: 12638 RVA: 0x0015C7B4 File Offset: 0x0015A9B4
			public void SetupElement(GameObject obj)
			{
				if (!obj)
				{
					return;
				}
				if (this.m_materialOverride != null)
				{
					MeshRenderer component = obj.GetComponent<MeshRenderer>();
					if (component)
					{
						component.sharedMaterial = this.m_materialOverride;
					}
				}
				obj.SetActive(this.m_active);
			}

			// Token: 0x04002FC0 RID: 12224
			[SerializeField]
			private bool m_active;

			// Token: 0x04002FC1 RID: 12225
			[SerializeField]
			private Material m_materialOverride;
		}
	}
}
