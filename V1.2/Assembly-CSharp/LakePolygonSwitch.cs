using System;
using UnityEngine;

// Token: 0x02000017 RID: 23
public class LakePolygonSwitch : MonoBehaviour
{
	// Token: 0x06000052 RID: 82 RVA: 0x0004497C File Offset: 0x00042B7C
	public void Switch()
	{
		this.SetProfile(this.Profile);
		this.Polygon.GeneratePolygon(false);
	}

	// Token: 0x06000053 RID: 83 RVA: 0x0008C714 File Offset: 0x0008A914
	public void SetProfile(LakePolygonProfile lakeProfile)
	{
		this.Polygon.currentProfile = lakeProfile;
		this.Polygon.GetComponent<MeshRenderer>().sharedMaterial = this.Polygon.currentProfile.lakeMaterial;
		this.Polygon.terrainCarve = new AnimationCurve(this.Polygon.currentProfile.terrainCarve.keys);
		this.Polygon.terrainPaintCarve = new AnimationCurve(this.Polygon.currentProfile.terrainPaintCarve.keys);
		this.Polygon.distSmooth = this.Polygon.currentProfile.distSmooth;
		this.Polygon.uvScale = this.Polygon.currentProfile.uvScale;
		this.Polygon.terrainSmoothMultiplier = this.Polygon.currentProfile.terrainSmoothMultiplier;
		this.Polygon.currentSplatMap = this.Polygon.currentProfile.currentSplatMap;
		this.Polygon.maximumTriangleSize = this.Polygon.currentProfile.maximumTriangleSize;
		this.Polygon.traingleDensity = this.Polygon.currentProfile.traingleDensity;
		this.Polygon.receiveShadows = this.Polygon.currentProfile.receiveShadows;
		this.Polygon.shadowCastingMode = this.Polygon.currentProfile.shadowCastingMode;
		this.Polygon.automaticFlowMapScale = this.Polygon.currentProfile.automaticFlowMapScale;
		this.Polygon.noiseflowMap = this.Polygon.currentProfile.noiseflowMap;
		this.Polygon.noiseMultiplierflowMap = this.Polygon.currentProfile.noiseMultiplierflowMap;
		this.Polygon.noiseSizeXflowMap = this.Polygon.currentProfile.noiseSizeXflowMap;
		this.Polygon.noiseSizeZflowMap = this.Polygon.currentProfile.noiseSizeZflowMap;
		this.Polygon.noiseCarve = this.Polygon.currentProfile.noiseCarve;
		this.Polygon.noiseMultiplierInside = this.Polygon.currentProfile.noiseMultiplierInside;
		this.Polygon.noiseMultiplierOutside = this.Polygon.currentProfile.noiseMultiplierOutside;
		this.Polygon.noiseSizeX = this.Polygon.currentProfile.noiseSizeX;
		this.Polygon.noiseSizeZ = this.Polygon.currentProfile.noiseSizeZ;
		this.Polygon.noisePaint = this.Polygon.currentProfile.noisePaint;
		this.Polygon.noiseMultiplierInsidePaint = this.Polygon.currentProfile.noiseMultiplierInsidePaint;
		this.Polygon.noiseMultiplierOutsidePaint = this.Polygon.currentProfile.noiseMultiplierOutsidePaint;
		this.Polygon.noiseSizeXPaint = this.Polygon.currentProfile.noiseSizeXPaint;
		this.Polygon.noiseSizeZPaint = this.Polygon.currentProfile.noiseSizeZPaint;
		this.Polygon.mixTwoSplatMaps = this.Polygon.currentProfile.mixTwoSplatMaps;
		this.Polygon.secondSplatMap = this.Polygon.currentProfile.secondSplatMap;
		this.Polygon.addCliffSplatMap = this.Polygon.currentProfile.addCliffSplatMap;
		this.Polygon.cliffSplatMap = this.Polygon.currentProfile.cliffSplatMap;
		this.Polygon.cliffAngle = this.Polygon.currentProfile.cliffAngle;
		this.Polygon.cliffBlend = this.Polygon.currentProfile.cliffBlend;
		this.Polygon.cliffSplatMapOutside = this.Polygon.currentProfile.cliffSplatMapOutside;
		this.Polygon.cliffAngleOutside = this.Polygon.currentProfile.cliffAngleOutside;
		this.Polygon.cliffBlendOutside = this.Polygon.currentProfile.cliffBlendOutside;
		this.Polygon.distanceClearFoliage = this.Polygon.currentProfile.distanceClearFoliage;
		this.Polygon.distanceClearFoliageTrees = this.Polygon.currentProfile.distanceClearFoliageTrees;
		this.Polygon.oldProfile = this.Polygon.currentProfile;
	}

	// Token: 0x040000C2 RID: 194
	public LakePolygon Polygon;

	// Token: 0x040000C3 RID: 195
	public LakePolygonProfile Profile;
}
