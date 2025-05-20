using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002CD RID: 717
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public class BetterLitGlobals : MonoBehaviour
	{
		// Token: 0x060014D6 RID: 5334 RVA: 0x000FBA38 File Offset: 0x000F9C38
		private void Update()
		{
			Shader.SetGlobalColor(BetterLitGlobals.kAlbedoTint, this.m_color);
			Vector2 v = (BetterLitGlobals.WetnessOverride != null) ? (Vector2.one * BetterLitGlobals.WetnessOverride.Value) : this.m_wetnessParams;
			Shader.SetGlobalVector(BetterLitGlobals.kWetness, v);
			float value = (BetterLitGlobals.SnowOverride != null) ? BetterLitGlobals.SnowOverride.Value : this.m_snowLevel;
			Shader.SetGlobalFloat(BetterLitGlobals.kSnow, value);
			Shader.SetGlobalFloat(BetterLitGlobals.kLayerHeightMultiplier, this.m_layerHeightMultiplier);
		}

		// Token: 0x04001D1F RID: 7455
		internal static float? WetnessOverride = null;

		// Token: 0x04001D20 RID: 7456
		internal static float? SnowOverride = null;

		// Token: 0x04001D21 RID: 7457
		private const string kColorPalette = "Rock";

		// Token: 0x04001D22 RID: 7458
		private static readonly int kWetness = Shader.PropertyToID("_Global_WetnessParams");

		// Token: 0x04001D23 RID: 7459
		private static readonly int kSnow = Shader.PropertyToID("_Global_SnowLevel");

		// Token: 0x04001D24 RID: 7460
		private static readonly int kLayerHeightMultiplier = Shader.PropertyToID("_Global_LayerHeightMultiplier");

		// Token: 0x04001D25 RID: 7461
		private static readonly int kAlbedoTint = Shader.PropertyToID("_Global_AlbedoTint");

		// Token: 0x04001D26 RID: 7462
		[SerializeField]
		private Color m_color = Color.white;

		// Token: 0x04001D27 RID: 7463
		[Range(0f, 2f)]
		[SerializeField]
		private float m_layerHeightMultiplier;

		// Token: 0x04001D28 RID: 7464
		[SerializeField]
		private Vector2 m_wetnessParams = Vector2.zero;

		// Token: 0x04001D29 RID: 7465
		[Range(0f, 1f)]
		[SerializeField]
		private float m_snowLevel;
	}
}
