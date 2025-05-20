using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002DD RID: 733
	public static class ShaderExtensions
	{
		// Token: 0x06001517 RID: 5399 RVA: 0x00050C68 File Offset: 0x0004EE68
		public static void SetMainColor(this Renderer renderer, Color color)
		{
			ShaderExtensions.SetMaterialColor(renderer, ShaderExtensions.kMaterialColorID, color);
		}

		// Token: 0x06001518 RID: 5400 RVA: 0x00050C76 File Offset: 0x0004EE76
		public static void SetEmissionColor(this Renderer renderer, Color color)
		{
			ShaderExtensions.SetMaterialColor(renderer, ShaderExtensions.kEmissionColorID, color);
		}

		// Token: 0x06001519 RID: 5401 RVA: 0x000FC0A4 File Offset: 0x000FA2A4
		public static void SetMaterialColor(Renderer renderer, int colorID, Color color)
		{
			if (!renderer)
			{
				return;
			}
			MaterialPropertyBlock materialPropertyBlock = MaterialPropertyBlockCache.GetMaterialPropertyBlock(renderer);
			renderer.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetColor(colorID, color);
			renderer.SetPropertyBlock(materialPropertyBlock);
		}

		// Token: 0x0600151A RID: 5402 RVA: 0x00050C84 File Offset: 0x0004EE84
		public static void SetEmissionIntensity(this Renderer renderer, float value)
		{
			ShaderExtensions.SetMaterialFloat(renderer, ShaderExtensions.kEmissionIntensityID, value);
		}

		// Token: 0x0600151B RID: 5403 RVA: 0x000FC0D8 File Offset: 0x000FA2D8
		private static void SetMaterialFloat(Renderer renderer, int valueID, float value)
		{
			if (!renderer)
			{
				return;
			}
			MaterialPropertyBlock materialPropertyBlock = MaterialPropertyBlockCache.GetMaterialPropertyBlock(renderer);
			renderer.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetFloat(valueID, value);
			renderer.SetPropertyBlock(materialPropertyBlock);
		}

		// Token: 0x0600151C RID: 5404 RVA: 0x00050C92 File Offset: 0x0004EE92
		public static Color GetMainColor(this Material material)
		{
			return ShaderExtensions.GetMaterialColor(material, ShaderExtensions.kMaterialColorID);
		}

		// Token: 0x0600151D RID: 5405 RVA: 0x00050C9F File Offset: 0x0004EE9F
		public static Color GetEmissionColor(this Material material)
		{
			return ShaderExtensions.GetMaterialColor(material, ShaderExtensions.kEmissionColorID);
		}

		// Token: 0x0600151E RID: 5406 RVA: 0x00050CAC File Offset: 0x0004EEAC
		private static Color GetMaterialColor(Material material, int colorID)
		{
			if (material)
			{
				return material.GetColor(colorID);
			}
			return Color.white;
		}

		// Token: 0x0600151F RID: 5407 RVA: 0x00050CC3 File Offset: 0x0004EEC3
		public static void SetMainColor(this Material material, Color color)
		{
			ShaderExtensions.SetMaterialColor(material, ShaderExtensions.kMaterialColorID, color);
		}

		// Token: 0x06001520 RID: 5408 RVA: 0x00050CD1 File Offset: 0x0004EED1
		public static void SetEmissionColor(this Material material, Color color)
		{
			ShaderExtensions.SetMaterialColor(material, ShaderExtensions.kEmissionColorID, color);
		}

		// Token: 0x06001521 RID: 5409 RVA: 0x00050CDF File Offset: 0x0004EEDF
		private static void SetMaterialColor(Material material, int colorID, Color color)
		{
			if (!material)
			{
				return;
			}
			material.SetColor(colorID, color);
		}

		// Token: 0x06001522 RID: 5410 RVA: 0x00050CF2 File Offset: 0x0004EEF2
		public static void SetEmissionIntensity(this Material material, float value)
		{
			ShaderExtensions.SetMaterialFloat(material, ShaderExtensions.kEmissionIntensityID, value);
		}

		// Token: 0x06001523 RID: 5411 RVA: 0x00050D00 File Offset: 0x0004EF00
		public static float GetEmissionIntensity(this Material material)
		{
			return ShaderExtensions.GetMaterialFloat(material, ShaderExtensions.kEmissionIntensityID);
		}

		// Token: 0x06001524 RID: 5412 RVA: 0x00050D0D File Offset: 0x0004EF0D
		private static float GetMaterialFloat(Material material, int valueID)
		{
			if (material)
			{
				return material.GetFloat(valueID);
			}
			return 0f;
		}

		// Token: 0x06001525 RID: 5413 RVA: 0x00050D24 File Offset: 0x0004EF24
		private static void SetMaterialFloat(Material material, int valueID, float value)
		{
			if (!material)
			{
				return;
			}
			material.SetFloat(valueID, value);
		}

		// Token: 0x04001D5A RID: 7514
		private const string kMaterialColor = "_Color";

		// Token: 0x04001D5B RID: 7515
		private const string kMaterialEmissionColor = "_EmissionColor";

		// Token: 0x04001D5C RID: 7516
		private const string kMaterialEmissionIntensity = "_EmissionIntensity";

		// Token: 0x04001D5D RID: 7517
		private const string kMaterialEmissiveColor = "_EmissiveColor";

		// Token: 0x04001D5E RID: 7518
		public static readonly int kMaterialColorID = Shader.PropertyToID("_Color");

		// Token: 0x04001D5F RID: 7519
		public static readonly int kEmissionColorID = Shader.PropertyToID("_EmissionColor");

		// Token: 0x04001D60 RID: 7520
		public static readonly int kEmissionIntensityID = Shader.PropertyToID("_EmissionIntensity");

		// Token: 0x04001D61 RID: 7521
		public static readonly int kEmissiveColorId = Shader.PropertyToID("_EmissiveColor");
	}
}
