using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x02000095 RID: 149
	[Serializable]
	public class IvyParameters
	{
		// Token: 0x060005CE RID: 1486 RVA: 0x000A3A74 File Offset: 0x000A1C74
		public IvyParameters()
		{
		}

		// Token: 0x060005CF RID: 1487 RVA: 0x000A3BF4 File Offset: 0x000A1DF4
		public IvyParameters(IvyParameters copyFrom)
		{
			this.CopyFrom(copyFrom);
		}

		// Token: 0x060005D0 RID: 1488 RVA: 0x0004707E File Offset: 0x0004527E
		public void CopyFrom(IvyPreset ivyPreset)
		{
			this.CopyFrom(ivyPreset.ivyParameters);
		}

		// Token: 0x060005D1 RID: 1489 RVA: 0x000A3D7C File Offset: 0x000A1F7C
		public void CopyFrom(IvyParametersGUI copyFrom)
		{
			this.stepSize = copyFrom.stepSize;
			this.branchProvability = copyFrom.branchProvability;
			this.maxBranchs = copyFrom.maxBranchs;
			this.layerMask = copyFrom.layerMask;
			this.minDistanceToSurface = copyFrom.minDistanceToSurface;
			this.maxDistanceToSurface = copyFrom.maxDistanceToSurface;
			this.DTSFrequency = copyFrom.DTSFrequency;
			this.DTSRandomness = copyFrom.DTSRandomness;
			this.directionFrequency = copyFrom.directionFrequency;
			this.directionAmplitude = copyFrom.directionAmplitude;
			this.directionRandomness = copyFrom.directionRandomness;
			this.gravity = new Vector3(copyFrom.gravityX, copyFrom.gravityY, copyFrom.gravityZ);
			this.grabProvabilityOnFall = copyFrom.grabProvabilityOnFall;
			this.stiffness = copyFrom.stiffness;
			this.optAngleBias = copyFrom.optAngleBias;
			this.leaveEvery = copyFrom.leaveEvery;
			this.randomLeaveEvery = copyFrom.randomLeaveEvery;
			this.buffer32Bits = copyFrom.buffer32Bits;
			this.halfgeom = copyFrom.halfgeom;
			this.sides = copyFrom.sides;
			this.minRadius = copyFrom.minRadius;
			this.maxRadius = copyFrom.maxRadius;
			this.radiusVarFreq = copyFrom.radiusVarFreq;
			this.radiusVarOffset = copyFrom.radiusVarOffset;
			this.tipInfluence = copyFrom.tipInfluence;
			this.uvScale = new Vector2(copyFrom.uvScaleX, copyFrom.uvScaleY);
			this.uvOffset = new Vector2(copyFrom.uvOffsetX, copyFrom.uvOffsetY);
			this.minScale = copyFrom.minScale;
			this.maxScale = copyFrom.maxScale;
			this.globalOrientation = copyFrom.globalOrientation;
			this.globalRotation = new Vector3(copyFrom.globalRotationX, copyFrom.globalRotationY, copyFrom.globalRotationZ);
			this.rotation = new Vector3(copyFrom.rotationX, copyFrom.rotationY, copyFrom.rotationZ);
			this.randomRotation = new Vector3(copyFrom.randomRotationX, copyFrom.randomRotationY, copyFrom.randomRotationZ);
			this.offset = new Vector3(copyFrom.offsetX, copyFrom.offsetY, copyFrom.offsetZ);
			this.LMUVPadding = copyFrom.LMUVPadding;
			this.generateBranches = copyFrom.generateBranches;
			this.generateLeaves = copyFrom.generateLeaves;
			this.generateLightmapUVs = copyFrom.generateLightmapUVs;
			this.branchesMaterial = copyFrom.branchesMaterial;
			this.leavesPrefabs = new GameObject[copyFrom.leavesPrefabs.Count];
			for (int i = 0; i < copyFrom.leavesPrefabs.Count; i++)
			{
				this.leavesPrefabs[i] = copyFrom.leavesPrefabs[i];
			}
			this.leavesProb = new float[copyFrom.leavesProb.Count];
			for (int j = 0; j < copyFrom.leavesProb.Count; j++)
			{
				this.leavesProb[j] = copyFrom.leavesProb[j];
			}
		}

		// Token: 0x060005D2 RID: 1490 RVA: 0x000A411C File Offset: 0x000A231C
		public void CopyFrom(IvyParameters copyFrom)
		{
			this.stepSize = copyFrom.stepSize;
			this.branchProvability = copyFrom.branchProvability;
			this.maxBranchs = copyFrom.maxBranchs;
			this.layerMask = copyFrom.layerMask;
			this.minDistanceToSurface = copyFrom.minDistanceToSurface;
			this.maxDistanceToSurface = copyFrom.maxDistanceToSurface;
			this.DTSFrequency = copyFrom.DTSFrequency;
			this.DTSRandomness = copyFrom.DTSRandomness;
			this.directionFrequency = copyFrom.directionFrequency;
			this.directionAmplitude = copyFrom.directionAmplitude;
			this.directionRandomness = copyFrom.directionRandomness;
			this.gravity = copyFrom.gravity;
			this.grabProvabilityOnFall = copyFrom.grabProvabilityOnFall;
			this.stiffness = copyFrom.stiffness;
			this.optAngleBias = copyFrom.optAngleBias;
			this.leaveEvery = copyFrom.leaveEvery;
			this.randomLeaveEvery = copyFrom.randomLeaveEvery;
			this.halfgeom = copyFrom.halfgeom;
			this.sides = copyFrom.sides;
			this.minRadius = copyFrom.minRadius;
			this.maxRadius = copyFrom.maxRadius;
			this.radiusVarFreq = copyFrom.radiusVarFreq;
			this.radiusVarOffset = copyFrom.radiusVarOffset;
			this.tipInfluence = copyFrom.tipInfluence;
			this.uvScale = copyFrom.uvScale;
			this.uvOffset = copyFrom.uvOffset;
			this.minScale = copyFrom.minScale;
			this.maxScale = copyFrom.maxScale;
			this.globalOrientation = copyFrom.globalOrientation;
			this.globalRotation = copyFrom.globalRotation;
			this.rotation = copyFrom.rotation;
			this.randomRotation = copyFrom.randomRotation;
			this.offset = copyFrom.offset;
			this.LMUVPadding = copyFrom.LMUVPadding;
			this.generateBranches = copyFrom.generateBranches;
			this.generateLeaves = copyFrom.generateLeaves;
			this.generateLightmapUVs = copyFrom.generateLightmapUVs;
			this.branchesMaterial = copyFrom.branchesMaterial;
			this.leavesPrefabs = new GameObject[copyFrom.leavesPrefabs.Length];
			for (int i = 0; i < copyFrom.leavesPrefabs.Length; i++)
			{
				this.leavesPrefabs[i] = copyFrom.leavesPrefabs[i];
			}
			this.leavesProb = new float[copyFrom.leavesProb.Length];
			for (int j = 0; j < copyFrom.leavesProb.Length; j++)
			{
				this.leavesProb[j] = copyFrom.leavesProb[j];
			}
		}

		// Token: 0x060005D3 RID: 1491 RVA: 0x000A4360 File Offset: 0x000A2560
		public bool IsEqualTo(IvyParameters compareTo)
		{
			return this.stepSize == compareTo.stepSize && this.branchProvability == compareTo.branchProvability && this.maxBranchs == compareTo.maxBranchs && this.layerMask == compareTo.layerMask && this.minDistanceToSurface == compareTo.minDistanceToSurface && this.maxDistanceToSurface == compareTo.maxDistanceToSurface && this.DTSFrequency == compareTo.DTSFrequency && this.DTSRandomness == compareTo.DTSRandomness && this.directionFrequency == compareTo.directionFrequency && this.directionAmplitude == compareTo.directionAmplitude && this.directionRandomness == compareTo.directionRandomness && this.gravity == compareTo.gravity && this.grabProvabilityOnFall == compareTo.grabProvabilityOnFall && this.stiffness == compareTo.stiffness && this.optAngleBias == compareTo.optAngleBias && this.leaveEvery == compareTo.leaveEvery && this.randomLeaveEvery == compareTo.randomLeaveEvery && this.buffer32Bits == compareTo.buffer32Bits && this.halfgeom == compareTo.halfgeom && this.sides == compareTo.sides && this.minRadius == compareTo.minRadius && this.maxRadius == compareTo.maxRadius && this.radiusVarFreq == compareTo.radiusVarFreq && this.radiusVarOffset == compareTo.radiusVarOffset && this.tipInfluence == compareTo.tipInfluence && this.uvScale == compareTo.uvScale && this.uvOffset == compareTo.uvOffset && this.minScale == compareTo.minScale && this.maxScale == compareTo.maxScale && this.globalOrientation == compareTo.globalOrientation && this.globalRotation == compareTo.globalRotation && this.rotation == compareTo.rotation && this.randomRotation == compareTo.randomRotation && this.offset == compareTo.offset && this.LMUVPadding == compareTo.LMUVPadding && this.branchesMaterial == compareTo.branchesMaterial && this.leavesPrefabs.SequenceEqual(compareTo.leavesPrefabs) && this.leavesProb.SequenceEqual(compareTo.leavesProb) && this.generateBranches == compareTo.generateBranches && this.generateLeaves == compareTo.generateLeaves && this.generateLightmapUVs == compareTo.generateLightmapUVs;
		}

		// Token: 0x04000688 RID: 1672
		public float stepSize = 0.1f;

		// Token: 0x04000689 RID: 1673
		public int randomSeed;

		// Token: 0x0400068A RID: 1674
		public float branchProvability = 0.05f;

		// Token: 0x0400068B RID: 1675
		public int maxBranchs = 5;

		// Token: 0x0400068C RID: 1676
		public LayerMask layerMask = -1;

		// Token: 0x0400068D RID: 1677
		public float minDistanceToSurface = 0.01f;

		// Token: 0x0400068E RID: 1678
		public float maxDistanceToSurface = 0.03f;

		// Token: 0x0400068F RID: 1679
		public float DTSFrequency = 1f;

		// Token: 0x04000690 RID: 1680
		public float DTSRandomness = 0.2f;

		// Token: 0x04000691 RID: 1681
		public float directionFrequency = 1f;

		// Token: 0x04000692 RID: 1682
		public float directionAmplitude = 20f;

		// Token: 0x04000693 RID: 1683
		public float directionRandomness = 1f;

		// Token: 0x04000694 RID: 1684
		public Vector3 gravity;

		// Token: 0x04000695 RID: 1685
		public float grabProvabilityOnFall = 0.1f;

		// Token: 0x04000696 RID: 1686
		public float stiffness = 0.03f;

		// Token: 0x04000697 RID: 1687
		public float optAngleBias = 15f;

		// Token: 0x04000698 RID: 1688
		public int leaveEvery = 1;

		// Token: 0x04000699 RID: 1689
		public int randomLeaveEvery = 1;

		// Token: 0x0400069A RID: 1690
		public bool buffer32Bits;

		// Token: 0x0400069B RID: 1691
		public bool halfgeom;

		// Token: 0x0400069C RID: 1692
		public int sides = 3;

		// Token: 0x0400069D RID: 1693
		public float minRadius = 0.025f;

		// Token: 0x0400069E RID: 1694
		public float maxRadius = 0.05f;

		// Token: 0x0400069F RID: 1695
		public float radiusVarFreq = 1f;

		// Token: 0x040006A0 RID: 1696
		public float radiusVarOffset;

		// Token: 0x040006A1 RID: 1697
		public float tipInfluence = 0.5f;

		// Token: 0x040006A2 RID: 1698
		public Vector2 uvScale = new Vector2(1f, 1f);

		// Token: 0x040006A3 RID: 1699
		public Vector2 uvOffset = new Vector2(0f, 0f);

		// Token: 0x040006A4 RID: 1700
		public float minScale = 0.7f;

		// Token: 0x040006A5 RID: 1701
		public float maxScale = 1.2f;

		// Token: 0x040006A6 RID: 1702
		public bool globalOrientation;

		// Token: 0x040006A7 RID: 1703
		public Vector3 globalRotation = -Vector3.up;

		// Token: 0x040006A8 RID: 1704
		public Vector3 rotation = Vector3.zero;

		// Token: 0x040006A9 RID: 1705
		public Vector3 randomRotation = Vector3.zero;

		// Token: 0x040006AA RID: 1706
		public Vector3 offset = Vector3.zero;

		// Token: 0x040006AB RID: 1707
		public float LMUVPadding = 0.002f;

		// Token: 0x040006AC RID: 1708
		public Material branchesMaterial;

		// Token: 0x040006AD RID: 1709
		public GameObject[] leavesPrefabs = new GameObject[0];

		// Token: 0x040006AE RID: 1710
		public float[] leavesProb = new float[0];

		// Token: 0x040006AF RID: 1711
		public Dictionary<int, Vector2> UV2IslesSizes;

		// Token: 0x040006B0 RID: 1712
		public bool generateBranches;

		// Token: 0x040006B1 RID: 1713
		public bool generateLeaves;

		// Token: 0x040006B2 RID: 1714
		public bool generateLightmapUVs;
	}
}
