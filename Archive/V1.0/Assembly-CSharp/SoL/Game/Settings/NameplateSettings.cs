using System;
using System.Collections;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x0200072F RID: 1839
	[Serializable]
	public class NameplateSettings
	{
		// Token: 0x17000C56 RID: 3158
		// (get) Token: 0x06003716 RID: 14102 RVA: 0x00065BA6 File Offset: 0x00063DA6
		public Color GroupColor
		{
			get
			{
				return this.m_groupColor;
			}
		}

		// Token: 0x17000C57 RID: 3159
		// (get) Token: 0x06003717 RID: 14103 RVA: 0x00065BAE File Offset: 0x00063DAE
		public Color InteractiveColor
		{
			get
			{
				return this.m_interactiveColor;
			}
		}

		// Token: 0x06003718 RID: 14104 RVA: 0x00065BB6 File Offset: 0x00063DB6
		public GameObject GetOverheadPrefab(OverheadNameplateMode mode)
		{
			return ((mode == OverheadNameplateMode.WorldSpace) ? this.m_worldSpace : this.m_uiSpace).OverheadPrefab;
		}

		// Token: 0x06003719 RID: 14105 RVA: 0x0016AD58 File Offset: 0x00168F58
		public float GetOverheadScale(float defaultScale, float distanceFraction, OverheadNameplateMode mode)
		{
			NameplateSettings.OverheadNameplateConfig overheadNameplateConfig = (mode == OverheadNameplateMode.WorldSpace) ? this.m_worldSpace : this.m_uiSpace;
			Vector2 vector = defaultScale * overheadNameplateConfig.ScaleRange;
			return Mathf.Lerp(vector.x, vector.y, distanceFraction) * Options.GameOptions.OverheadNameplateScale;
		}

		// Token: 0x0600371A RID: 14106 RVA: 0x0016ADA4 File Offset: 0x00168FA4
		public float GetOverheadAlpha(float maxAlpha, float distanceFraction, OverheadNameplateMode mode)
		{
			float fadeDistanceThreshold = ((mode == OverheadNameplateMode.WorldSpace) ? this.m_worldSpace : this.m_uiSpace).FadeDistanceThreshold;
			if (distanceFraction > fadeDistanceThreshold)
			{
				float num = 1f - fadeDistanceThreshold;
				float b = Mathf.Clamp(fadeDistanceThreshold, 0f, maxAlpha);
				float t = (distanceFraction - fadeDistanceThreshold) / num;
				return Mathf.Lerp(maxAlpha, b, t);
			}
			return maxAlpha;
		}

		// Token: 0x0600371B RID: 14107 RVA: 0x0016ADF4 File Offset: 0x00168FF4
		public float GetOverheadChatScale(float defaultScale, float distanceFraction, OverheadNameplateMode mode)
		{
			NameplateSettings.OverheadNameplateConfig overheadNameplateConfig = (mode == OverheadNameplateMode.WorldSpace) ? this.m_worldSpace : this.m_uiSpace;
			Vector2 vector = defaultScale * overheadNameplateConfig.ChatScaleRange;
			return Mathf.Lerp(vector.x, vector.y, distanceFraction);
		}

		// Token: 0x17000C58 RID: 3160
		// (get) Token: 0x0600371C RID: 14108 RVA: 0x0004F9FB File Offset: 0x0004DBFB
		private IEnumerable GetColorValues
		{
			get
			{
				return SolOdinUtilities.GetColorValues();
			}
		}

		// Token: 0x040035A3 RID: 13731
		[SerializeField]
		private NameplateSettings.OverheadNameplateConfig m_worldSpace;

		// Token: 0x040035A4 RID: 13732
		[SerializeField]
		private NameplateSettings.OverheadNameplateConfig m_uiSpace;

		// Token: 0x040035A5 RID: 13733
		[SerializeField]
		private Color m_groupColor;

		// Token: 0x040035A6 RID: 13734
		[SerializeField]
		private Color m_interactiveColor;

		// Token: 0x02000730 RID: 1840
		[Serializable]
		private class OverheadNameplateConfig
		{
			// Token: 0x17000C59 RID: 3161
			// (get) Token: 0x0600371E RID: 14110 RVA: 0x00065BCE File Offset: 0x00063DCE
			public GameObject OverheadPrefab
			{
				get
				{
					return this.m_overheadPrefab;
				}
			}

			// Token: 0x17000C5A RID: 3162
			// (get) Token: 0x0600371F RID: 14111 RVA: 0x00065BD6 File Offset: 0x00063DD6
			public float FadeDistanceThreshold
			{
				get
				{
					return this.m_fadeDistanceThreshold;
				}
			}

			// Token: 0x17000C5B RID: 3163
			// (get) Token: 0x06003720 RID: 14112 RVA: 0x00065BDE File Offset: 0x00063DDE
			public Vector2 ScaleRange
			{
				get
				{
					return this.m_scaleRange;
				}
			}

			// Token: 0x17000C5C RID: 3164
			// (get) Token: 0x06003721 RID: 14113 RVA: 0x00065BE6 File Offset: 0x00063DE6
			public Vector2 ChatScaleRange
			{
				get
				{
					return this.m_chatScaleRange;
				}
			}

			// Token: 0x06003722 RID: 14114 RVA: 0x00065BEE File Offset: 0x00063DEE
			public OverheadNameplateConfig()
			{
				this.m_fadeDistanceThreshold = 1f;
				this.m_scaleRange = Vector2.one;
				this.m_chatScaleRange = Vector2.one;
			}

			// Token: 0x040035A7 RID: 13735
			[SerializeField]
			private GameObject m_overheadPrefab;

			// Token: 0x040035A8 RID: 13736
			[Range(0f, 1f)]
			[SerializeField]
			private float m_fadeDistanceThreshold;

			// Token: 0x040035A9 RID: 13737
			[SerializeField]
			private Vector2 m_scaleRange;

			// Token: 0x040035AA RID: 13738
			[SerializeField]
			private Vector2 m_chatScaleRange;
		}
	}
}
