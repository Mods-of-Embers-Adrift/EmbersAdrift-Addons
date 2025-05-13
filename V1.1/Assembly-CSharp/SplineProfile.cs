using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200001C RID: 28
[CreateAssetMenu(fileName = "SplineProfile", menuName = "SplineProfile", order = 1)]
public class SplineProfile : ScriptableObject
{
	// Token: 0x04000172 RID: 370
	public Material splineMaterial;

	// Token: 0x04000173 RID: 371
	public AnimationCurve meshCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x04000174 RID: 372
	public float minVal = 0.5f;

	// Token: 0x04000175 RID: 373
	public float maxVal = 0.5f;

	// Token: 0x04000176 RID: 374
	public int vertsInShape = 3;

	// Token: 0x04000177 RID: 375
	public float traingleDensity = 0.2f;

	// Token: 0x04000178 RID: 376
	public float uvScale = 3f;

	// Token: 0x04000179 RID: 377
	public bool uvRotation = true;

	// Token: 0x0400017A RID: 378
	public bool receiveShadows;

	// Token: 0x0400017B RID: 379
	public ShadowCastingMode shadowCastingMode;

	// Token: 0x0400017C RID: 380
	public AnimationCurve flowFlat = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0.025f),
		new Keyframe(0.5f, 0.05f),
		new Keyframe(1f, 0.025f)
	});

	// Token: 0x0400017D RID: 381
	public AnimationCurve flowWaterfall = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0.25f),
		new Keyframe(1f, 0.25f)
	});

	// Token: 0x0400017E RID: 382
	public bool noiseflowMap;

	// Token: 0x0400017F RID: 383
	public float noiseMultiplierflowMap = 0.1f;

	// Token: 0x04000180 RID: 384
	public float noiseSizeXflowMap = 2f;

	// Token: 0x04000181 RID: 385
	public float noiseSizeZflowMap = 2f;

	// Token: 0x04000182 RID: 386
	public float floatSpeed = 10f;

	// Token: 0x04000183 RID: 387
	public AnimationCurve terrainCarve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(10f, -2f)
	});

	// Token: 0x04000184 RID: 388
	public float distSmooth = 5f;

	// Token: 0x04000185 RID: 389
	public float distSmoothStart = 1f;

	// Token: 0x04000186 RID: 390
	public AnimationCurve terrainPaintCarve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04000187 RID: 391
	public LayerMask maskCarve;

	// Token: 0x04000188 RID: 392
	public bool noiseCarve;

	// Token: 0x04000189 RID: 393
	public float noiseMultiplierInside = 1f;

	// Token: 0x0400018A RID: 394
	public float noiseMultiplierOutside = 0.25f;

	// Token: 0x0400018B RID: 395
	public float noiseSizeX = 0.2f;

	// Token: 0x0400018C RID: 396
	public float noiseSizeZ = 0.2f;

	// Token: 0x0400018D RID: 397
	public float terrainSmoothMultiplier = 5f;

	// Token: 0x0400018E RID: 398
	public int currentSplatMap = 1;

	// Token: 0x0400018F RID: 399
	public bool mixTwoSplatMaps;

	// Token: 0x04000190 RID: 400
	public int secondSplatMap = 1;

	// Token: 0x04000191 RID: 401
	public bool addCliffSplatMap;

	// Token: 0x04000192 RID: 402
	public int cliffSplatMap = 1;

	// Token: 0x04000193 RID: 403
	public float cliffAngle = 45f;

	// Token: 0x04000194 RID: 404
	public float cliffBlend = 1f;

	// Token: 0x04000195 RID: 405
	public int cliffSplatMapOutside = 1;

	// Token: 0x04000196 RID: 406
	public float cliffAngleOutside = 45f;

	// Token: 0x04000197 RID: 407
	public float cliffBlendOutside = 1f;

	// Token: 0x04000198 RID: 408
	public float distanceClearFoliage = 1f;

	// Token: 0x04000199 RID: 409
	public float distanceClearFoliageTrees = 1f;

	// Token: 0x0400019A RID: 410
	public bool noisePaint;

	// Token: 0x0400019B RID: 411
	public float noiseMultiplierInsidePaint = 0.25f;

	// Token: 0x0400019C RID: 412
	public float noiseMultiplierOutsidePaint = 0.25f;

	// Token: 0x0400019D RID: 413
	public float noiseSizeXPaint = 0.2f;

	// Token: 0x0400019E RID: 414
	public float noiseSizeZPaint = 0.2f;

	// Token: 0x0400019F RID: 415
	public float simulatedRiverLength = 100f;

	// Token: 0x040001A0 RID: 416
	public int simulatedRiverPoints = 10;

	// Token: 0x040001A1 RID: 417
	public float simulatedMinStepSize = 1f;

	// Token: 0x040001A2 RID: 418
	public bool simulatedNoUp;

	// Token: 0x040001A3 RID: 419
	public bool simulatedBreakOnUp = true;

	// Token: 0x040001A4 RID: 420
	public bool noiseWidth;

	// Token: 0x040001A5 RID: 421
	public float noiseMultiplierWidth = 4f;

	// Token: 0x040001A6 RID: 422
	public float noiseSizeWidth = 0.5f;

	// Token: 0x040001A7 RID: 423
	public int biomeType;
}
