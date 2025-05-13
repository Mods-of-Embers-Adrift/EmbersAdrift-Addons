using System;
using UnityEngine;

// Token: 0x02000037 RID: 55
[ExecuteInEditMode]
public class NM_Wind : MonoBehaviour
{
	// Token: 0x06000119 RID: 281 RVA: 0x00044FC7 File Offset: 0x000431C7
	private void Start()
	{
		this.ApplySettings();
	}

	// Token: 0x0600011A RID: 282 RVA: 0x00044FC7 File Offset: 0x000431C7
	private void Update()
	{
		this.ApplySettings();
	}

	// Token: 0x0600011B RID: 283 RVA: 0x00044FC7 File Offset: 0x000431C7
	private void OnValidate()
	{
		this.ApplySettings();
	}

	// Token: 0x0600011C RID: 284 RVA: 0x00097308 File Offset: 0x00095508
	private void ApplySettings()
	{
		Shader.SetGlobalTexture("WIND_SETTINGS_TexNoise", this.NoiseTexture);
		Shader.SetGlobalTexture("WIND_SETTINGS_TexGust", this.GustMaskTexture);
		Shader.SetGlobalVector("WIND_SETTINGS_WorldDirectionAndSpeed", this.GetDirectionAndSpeed());
		Shader.SetGlobalFloat("WIND_SETTINGS_FlexNoiseScale", 1f / Mathf.Max(0.01f, this.FlexNoiseWorldSize));
		Shader.SetGlobalFloat("WIND_SETTINGS_ShiverNoiseScale", 1f / Mathf.Max(0.01f, this.ShiverNoiseWorldSize));
		Shader.SetGlobalFloat("WIND_SETTINGS_Turbulence", this.WindSpeed * this.Turbulence);
		Shader.SetGlobalFloat("WIND_SETTINGS_GustSpeed", this.GustSpeed);
		Shader.SetGlobalFloat("WIND_SETTINGS_GustScale", this.GustScale);
		Shader.SetGlobalFloat("WIND_SETTINGS_GustWorldScale", 1f / Mathf.Max(0.01f, this.GustWorldSize));
	}

	// Token: 0x0600011D RID: 285 RVA: 0x000973DC File Offset: 0x000955DC
	private Vector4 GetDirectionAndSpeed()
	{
		Vector3 normalized = base.transform.forward.normalized;
		return new Vector4(normalized.x, normalized.y, normalized.z, this.WindSpeed * 0.2777f);
	}

	// Token: 0x040002A5 RID: 677
	[Header("General Parameters")]
	[Tooltip("Wind Speed in Kilometers per hour")]
	public float WindSpeed = 30f;

	// Token: 0x040002A6 RID: 678
	[Range(0f, 2f)]
	[Tooltip("Wind Turbulence in percentage of wind Speed")]
	public float Turbulence = 0.25f;

	// Token: 0x040002A7 RID: 679
	[Header("Noise Parameters")]
	[Tooltip("Texture used for wind turbulence")]
	public Texture2D NoiseTexture;

	// Token: 0x040002A8 RID: 680
	[Tooltip("Size of one world tiling patch of the Noise Texture, for bending trees")]
	public float FlexNoiseWorldSize = 175f;

	// Token: 0x040002A9 RID: 681
	[Tooltip("Size of one world tiling patch of the Noise Texture, for leaf shivering")]
	public float ShiverNoiseWorldSize = 10f;

	// Token: 0x040002AA RID: 682
	[Header("Gust Parameters")]
	[Tooltip("Texture used for wind gusts")]
	public Texture2D GustMaskTexture;

	// Token: 0x040002AB RID: 683
	[Tooltip("Size of one world tiling patch of the Gust Texture, for leaf shivering")]
	public float GustWorldSize = 600f;

	// Token: 0x040002AC RID: 684
	[Tooltip("Wind Gust Speed in Kilometers per hour")]
	public float GustSpeed = 50f;

	// Token: 0x040002AD RID: 685
	[Tooltip("Wind Gust Influence on trees")]
	public float GustScale = 1f;
}
