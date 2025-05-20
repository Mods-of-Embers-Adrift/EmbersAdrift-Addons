using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000016 RID: 22
[CreateAssetMenu(fileName = "LakePolygonProfile", menuName = "LakePolygonProfile", order = 1)]
public class LakePolygonProfile : ScriptableObject
{
	// Token: 0x0400009C RID: 156
	public Material lakeMaterial;

	// Token: 0x0400009D RID: 157
	public float distSmooth = 5f;

	// Token: 0x0400009E RID: 158
	public float uvScale = 1f;

	// Token: 0x0400009F RID: 159
	public float maximumTriangleSize = 50f;

	// Token: 0x040000A0 RID: 160
	public float traingleDensity = 0.2f;

	// Token: 0x040000A1 RID: 161
	public bool receiveShadows;

	// Token: 0x040000A2 RID: 162
	public ShadowCastingMode shadowCastingMode;

	// Token: 0x040000A3 RID: 163
	public float automaticFlowMapScale = 0.2f;

	// Token: 0x040000A4 RID: 164
	public bool noiseflowMap;

	// Token: 0x040000A5 RID: 165
	public float noiseMultiplierflowMap = 1f;

	// Token: 0x040000A6 RID: 166
	public float noiseSizeXflowMap = 0.2f;

	// Token: 0x040000A7 RID: 167
	public float noiseSizeZflowMap = 0.2f;

	// Token: 0x040000A8 RID: 168
	public AnimationCurve terrainCarve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(10f, -2f)
	});

	// Token: 0x040000A9 RID: 169
	public float terrainSmoothMultiplier = 1f;

	// Token: 0x040000AA RID: 170
	public AnimationCurve terrainPaintCarve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x040000AB RID: 171
	public bool noiseCarve;

	// Token: 0x040000AC RID: 172
	public float noiseMultiplierInside = 1f;

	// Token: 0x040000AD RID: 173
	public float noiseMultiplierOutside = 0.25f;

	// Token: 0x040000AE RID: 174
	public float noiseSizeX = 0.2f;

	// Token: 0x040000AF RID: 175
	public float noiseSizeZ = 0.2f;

	// Token: 0x040000B0 RID: 176
	public int currentSplatMap = 1;

	// Token: 0x040000B1 RID: 177
	public bool noisePaint;

	// Token: 0x040000B2 RID: 178
	public float noiseMultiplierInsidePaint = 1f;

	// Token: 0x040000B3 RID: 179
	public float noiseMultiplierOutsidePaint = 0.5f;

	// Token: 0x040000B4 RID: 180
	public float noiseSizeXPaint = 0.2f;

	// Token: 0x040000B5 RID: 181
	public float noiseSizeZPaint = 0.2f;

	// Token: 0x040000B6 RID: 182
	public bool mixTwoSplatMaps;

	// Token: 0x040000B7 RID: 183
	public int secondSplatMap = 1;

	// Token: 0x040000B8 RID: 184
	public bool addCliffSplatMap;

	// Token: 0x040000B9 RID: 185
	public int cliffSplatMap = 1;

	// Token: 0x040000BA RID: 186
	public float cliffAngle = 25f;

	// Token: 0x040000BB RID: 187
	public float cliffBlend = 1f;

	// Token: 0x040000BC RID: 188
	public int cliffSplatMapOutside = 1;

	// Token: 0x040000BD RID: 189
	public float cliffAngleOutside = 25f;

	// Token: 0x040000BE RID: 190
	public float cliffBlendOutside = 1f;

	// Token: 0x040000BF RID: 191
	public float distanceClearFoliage = 1f;

	// Token: 0x040000C0 RID: 192
	public float distanceClearFoliageTrees = 1f;

	// Token: 0x040000C1 RID: 193
	public int biomeType;
}
