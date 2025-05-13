using System;
using UnityEngine;

// Token: 0x02000018 RID: 24
public class RamSwitch : MonoBehaviour
{
	// Token: 0x06000055 RID: 85 RVA: 0x00044996 File Offset: 0x00042B96
	public void Switch()
	{
		this.SetProfile(this.Profile);
		this.spline.GenerateSpline(null);
	}

	// Token: 0x06000056 RID: 86 RVA: 0x0008CB24 File Offset: 0x0008AD24
	public void SetProfile(SplineProfile splineProfile)
	{
		this.spline.currentProfile = splineProfile;
		this.spline.meshCurve = new AnimationCurve(this.spline.currentProfile.meshCurve.keys);
		this.spline.flowFlat = new AnimationCurve(this.spline.currentProfile.flowFlat.keys);
		this.spline.flowWaterfall = new AnimationCurve(this.spline.currentProfile.flowWaterfall.keys);
		this.spline.terrainCarve = new AnimationCurve(this.spline.currentProfile.terrainCarve.keys);
		this.spline.terrainPaintCarve = new AnimationCurve(this.spline.currentProfile.terrainPaintCarve.keys);
		for (int i = 0; i < this.spline.controlPointsMeshCurves.Count; i++)
		{
			this.spline.controlPointsMeshCurves[i] = new AnimationCurve(this.spline.meshCurve.keys);
		}
		this.spline.GetComponent<MeshRenderer>().sharedMaterial = this.spline.currentProfile.splineMaterial;
		this.spline.minVal = this.spline.currentProfile.minVal;
		this.spline.maxVal = this.spline.currentProfile.maxVal;
		this.spline.traingleDensity = this.spline.currentProfile.traingleDensity;
		this.spline.vertsInShape = this.spline.currentProfile.vertsInShape;
		this.spline.uvScale = this.spline.currentProfile.uvScale;
		this.spline.uvRotation = this.spline.currentProfile.uvRotation;
		this.spline.noiseflowMap = this.spline.currentProfile.noiseflowMap;
		this.spline.noiseMultiplierflowMap = this.spline.currentProfile.noiseMultiplierflowMap;
		this.spline.noiseSizeXflowMap = this.spline.currentProfile.noiseSizeXflowMap;
		this.spline.noiseSizeZflowMap = this.spline.currentProfile.noiseSizeZflowMap;
		this.spline.floatSpeed = this.spline.currentProfile.floatSpeed;
		this.spline.distSmooth = this.spline.currentProfile.distSmooth;
		this.spline.distSmoothStart = this.spline.currentProfile.distSmoothStart;
		this.spline.noiseCarve = this.spline.currentProfile.noiseCarve;
		this.spline.noiseMultiplierInside = this.spline.currentProfile.noiseMultiplierInside;
		this.spline.noiseMultiplierOutside = this.spline.currentProfile.noiseMultiplierOutside;
		this.spline.noiseSizeX = this.spline.currentProfile.noiseSizeX;
		this.spline.noiseSizeZ = this.spline.currentProfile.noiseSizeZ;
		this.spline.terrainSmoothMultiplier = this.spline.currentProfile.terrainSmoothMultiplier;
		this.spline.currentSplatMap = this.spline.currentProfile.currentSplatMap;
		this.spline.mixTwoSplatMaps = this.spline.currentProfile.mixTwoSplatMaps;
		this.spline.secondSplatMap = this.spline.currentProfile.secondSplatMap;
		this.spline.addCliffSplatMap = this.spline.currentProfile.addCliffSplatMap;
		this.spline.cliffSplatMap = this.spline.currentProfile.cliffSplatMap;
		this.spline.cliffAngle = this.spline.currentProfile.cliffAngle;
		this.spline.cliffBlend = this.spline.currentProfile.cliffBlend;
		this.spline.cliffSplatMapOutside = this.spline.currentProfile.cliffSplatMapOutside;
		this.spline.cliffAngleOutside = this.spline.currentProfile.cliffAngleOutside;
		this.spline.cliffBlendOutside = this.spline.currentProfile.cliffBlendOutside;
		this.spline.distanceClearFoliage = this.spline.currentProfile.distanceClearFoliage;
		this.spline.distanceClearFoliageTrees = this.spline.currentProfile.distanceClearFoliageTrees;
		this.spline.noisePaint = this.spline.currentProfile.noisePaint;
		this.spline.noiseMultiplierInsidePaint = this.spline.currentProfile.noiseMultiplierInsidePaint;
		this.spline.noiseMultiplierOutsidePaint = this.spline.currentProfile.noiseMultiplierOutsidePaint;
		this.spline.noiseSizeXPaint = this.spline.currentProfile.noiseSizeXPaint;
		this.spline.noiseSizeZPaint = this.spline.currentProfile.noiseSizeZPaint;
		this.spline.simulatedRiverLength = this.spline.currentProfile.simulatedRiverLength;
		this.spline.simulatedRiverPoints = this.spline.currentProfile.simulatedRiverPoints;
		this.spline.simulatedMinStepSize = this.spline.currentProfile.simulatedMinStepSize;
		this.spline.simulatedNoUp = this.spline.currentProfile.simulatedNoUp;
		this.spline.simulatedBreakOnUp = this.spline.currentProfile.simulatedBreakOnUp;
		this.spline.noiseWidth = this.spline.currentProfile.noiseWidth;
		this.spline.noiseMultiplierWidth = this.spline.currentProfile.noiseMultiplierWidth;
		this.spline.noiseSizeWidth = this.spline.currentProfile.noiseSizeWidth;
		this.spline.receiveShadows = this.spline.currentProfile.receiveShadows;
		this.spline.shadowCastingMode = this.spline.currentProfile.shadowCastingMode;
		this.spline.GenerateSpline(null);
		this.spline.oldProfile = this.spline.currentProfile;
	}

	// Token: 0x040000C4 RID: 196
	public RamSpline spline;

	// Token: 0x040000C5 RID: 197
	public SplineProfile Profile;
}
