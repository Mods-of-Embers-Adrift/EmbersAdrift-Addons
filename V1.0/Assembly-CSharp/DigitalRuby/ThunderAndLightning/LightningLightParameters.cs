using System;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	// Token: 0x020000AD RID: 173
	[Serializable]
	public class LightningLightParameters
	{
		// Token: 0x17000273 RID: 627
		// (get) Token: 0x06000681 RID: 1665 RVA: 0x0004773F File Offset: 0x0004593F
		public bool HasLight
		{
			get
			{
				return this.LightColor.a > 0f && this.LightIntensity >= 0.01f && this.LightPercent >= 1E-07f && this.LightRange > 0.01f;
			}
		}

		// Token: 0x0400078D RID: 1933
		[Tooltip("Light render mode - leave as auto unless you have special use cases")]
		[HideInInspector]
		public LightRenderMode RenderMode;

		// Token: 0x0400078E RID: 1934
		[Tooltip("Color of the light")]
		public Color LightColor = Color.white;

		// Token: 0x0400078F RID: 1935
		[Tooltip("What percent of segments should have a light? For performance you may want to keep this small.")]
		[Range(0f, 1f)]
		public float LightPercent = 1E-06f;

		// Token: 0x04000790 RID: 1936
		[Tooltip("What percent of lights created should cast shadows?")]
		[Range(0f, 1f)]
		public float LightShadowPercent;

		// Token: 0x04000791 RID: 1937
		[Tooltip("Light intensity")]
		[Range(0f, 8f)]
		public float LightIntensity = 0.5f;

		// Token: 0x04000792 RID: 1938
		[Tooltip("Bounce intensity")]
		[Range(0f, 8f)]
		public float BounceIntensity;

		// Token: 0x04000793 RID: 1939
		[Tooltip("Shadow strength, 0 means all light, 1 means all shadow")]
		[Range(0f, 1f)]
		public float ShadowStrength = 1f;

		// Token: 0x04000794 RID: 1940
		[Tooltip("Shadow bias, 0 - 2")]
		[Range(0f, 2f)]
		public float ShadowBias = 0.05f;

		// Token: 0x04000795 RID: 1941
		[Tooltip("Shadow normal bias, 0 - 3")]
		[Range(0f, 3f)]
		public float ShadowNormalBias = 0.4f;

		// Token: 0x04000796 RID: 1942
		[Tooltip("The range of each light created")]
		public float LightRange;

		// Token: 0x04000797 RID: 1943
		[Tooltip("Only light objects that match this layer mask")]
		public LayerMask CullingMask = -1;

		// Token: 0x04000798 RID: 1944
		[Tooltip("Offset from camera position when in orthographic mode")]
		[Range(-1000f, 1000f)]
		public float OrthographicOffset;

		// Token: 0x04000799 RID: 1945
		[Tooltip("Increase the duration of light fade in compared to the lightning fade.")]
		[Range(0f, 20f)]
		public float FadeInMultiplier = 1f;

		// Token: 0x0400079A RID: 1946
		[Tooltip("Increase the duration of light fully lit compared to the lightning fade.")]
		[Range(0f, 20f)]
		public float FadeFullyLitMultiplier = 1f;

		// Token: 0x0400079B RID: 1947
		[Tooltip("Increase the duration of light fade out compared to the lightning fade.")]
		[Range(0f, 20f)]
		public float FadeOutMultiplier = 1f;
	}
}
