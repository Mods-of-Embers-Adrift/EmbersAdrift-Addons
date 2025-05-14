using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	// Token: 0x02000092 RID: 146
	[Serializable]
	public class IvyParametersGUI : ScriptableObject
	{
		// Token: 0x060005C1 RID: 1473 RVA: 0x00046FBB File Offset: 0x000451BB
		public void CopyFrom(IvyPreset ivyPreset)
		{
			this.CopyFrom(ivyPreset.ivyParameters);
		}

		// Token: 0x060005C2 RID: 1474 RVA: 0x000A31C4 File Offset: 0x000A13C4
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
			this.gravityX = copyFrom.gravity.x;
			this.gravityY = copyFrom.gravity.y;
			this.gravityZ = copyFrom.gravity.z;
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
			this.uvScaleX = copyFrom.uvScale.x;
			this.uvScaleY = copyFrom.uvScale.y;
			this.uvOffsetX = copyFrom.uvOffset.x;
			this.uvOffsetY = copyFrom.uvOffset.y;
			this.minScale = copyFrom.minScale;
			this.maxScale = copyFrom.maxScale;
			this.globalOrientation = copyFrom.globalOrientation;
			this.globalRotationX = copyFrom.globalRotation.x;
			this.globalRotationY = copyFrom.globalRotation.y;
			this.globalRotationZ = copyFrom.globalRotation.z;
			this.rotationX = copyFrom.rotation.x;
			this.rotationY = copyFrom.rotation.y;
			this.rotationZ = copyFrom.rotation.z;
			this.randomRotationX = copyFrom.randomRotation.x;
			this.randomRotationY = copyFrom.randomRotation.y;
			this.randomRotationZ = copyFrom.randomRotation.z;
			this.randomRotationX = copyFrom.randomRotation.x;
			this.randomRotationY = copyFrom.randomRotation.y;
			this.randomRotationZ = copyFrom.randomRotation.z;
			this.offsetX = copyFrom.offset.x;
			this.offsetY = copyFrom.offset.y;
			this.offsetZ = copyFrom.offset.z;
			this.LMUVPadding = copyFrom.LMUVPadding;
			this.generateBranches = copyFrom.generateBranches;
			this.generateLeaves = copyFrom.generateLeaves;
			this.generateLightmapUVs = copyFrom.generateLightmapUVs;
			this.branchesMaterial = copyFrom.branchesMaterial;
			this.leavesProb.Clear();
			for (int i = 0; i < copyFrom.leavesProb.Length; i++)
			{
				this.leavesProb.Add(copyFrom.leavesProb[i]);
			}
			this.leavesPrefabs.Clear();
			for (int j = 0; j < copyFrom.leavesPrefabs.Length; j++)
			{
				this.leavesPrefabs.Add(copyFrom.leavesPrefabs[j]);
			}
		}

		// Token: 0x0400064B RID: 1611
		public IvyParameterFloat stepSize = 0.1f;

		// Token: 0x0400064C RID: 1612
		public IvyParameterFloat branchProvability = 0.05f;

		// Token: 0x0400064D RID: 1613
		public IvyParameterInt maxBranchs = 5;

		// Token: 0x0400064E RID: 1614
		public LayerMask layerMask = -1;

		// Token: 0x0400064F RID: 1615
		public IvyParameterFloat minDistanceToSurface = 0.01f;

		// Token: 0x04000650 RID: 1616
		public IvyParameterFloat maxDistanceToSurface = 0.03f;

		// Token: 0x04000651 RID: 1617
		public IvyParameterFloat DTSFrequency = 1f;

		// Token: 0x04000652 RID: 1618
		public IvyParameterFloat DTSRandomness = 0.2f;

		// Token: 0x04000653 RID: 1619
		public IvyParameterFloat directionFrequency = 1f;

		// Token: 0x04000654 RID: 1620
		public IvyParameterFloat directionAmplitude = 20f;

		// Token: 0x04000655 RID: 1621
		public IvyParameterFloat directionRandomness = 1f;

		// Token: 0x04000656 RID: 1622
		public IvyParameterFloat gravityX = 0f;

		// Token: 0x04000657 RID: 1623
		public IvyParameterFloat gravityY = -1f;

		// Token: 0x04000658 RID: 1624
		public IvyParameterFloat gravityZ = 0f;

		// Token: 0x04000659 RID: 1625
		public IvyParameterFloat grabProvabilityOnFall = 0.1f;

		// Token: 0x0400065A RID: 1626
		public IvyParameterFloat stiffness = 0.03f;

		// Token: 0x0400065B RID: 1627
		public IvyParameterFloat optAngleBias = 15f;

		// Token: 0x0400065C RID: 1628
		public IvyParameterInt leaveEvery = 1;

		// Token: 0x0400065D RID: 1629
		public IvyParameterInt randomLeaveEvery = 1;

		// Token: 0x0400065E RID: 1630
		public bool buffer32Bits;

		// Token: 0x0400065F RID: 1631
		public bool halfgeom;

		// Token: 0x04000660 RID: 1632
		public IvyParameterInt sides = 3;

		// Token: 0x04000661 RID: 1633
		public IvyParameterFloat minRadius = 0.025f;

		// Token: 0x04000662 RID: 1634
		public IvyParameterFloat maxRadius = 0.05f;

		// Token: 0x04000663 RID: 1635
		public IvyParameterFloat radiusVarFreq = 1f;

		// Token: 0x04000664 RID: 1636
		public IvyParameterFloat radiusVarOffset = 0f;

		// Token: 0x04000665 RID: 1637
		public IvyParameterFloat tipInfluence = 0.5f;

		// Token: 0x04000666 RID: 1638
		public IvyParameterFloat uvScaleX = 1f;

		// Token: 0x04000667 RID: 1639
		public IvyParameterFloat uvScaleY = 1f;

		// Token: 0x04000668 RID: 1640
		public IvyParameterFloat uvOffsetX = 0f;

		// Token: 0x04000669 RID: 1641
		public IvyParameterFloat uvOffsetY = 0f;

		// Token: 0x0400066A RID: 1642
		public IvyParameterFloat minScale = 0.7f;

		// Token: 0x0400066B RID: 1643
		public IvyParameterFloat maxScale = 1.2f;

		// Token: 0x0400066C RID: 1644
		public bool globalOrientation;

		// Token: 0x0400066D RID: 1645
		public IvyParameterFloat globalRotationX = 0f;

		// Token: 0x0400066E RID: 1646
		public IvyParameterFloat globalRotationY = -1f;

		// Token: 0x0400066F RID: 1647
		public IvyParameterFloat globalRotationZ = 0f;

		// Token: 0x04000670 RID: 1648
		public IvyParameterFloat rotationX = 0f;

		// Token: 0x04000671 RID: 1649
		public IvyParameterFloat rotationY = 0f;

		// Token: 0x04000672 RID: 1650
		public IvyParameterFloat rotationZ = 0f;

		// Token: 0x04000673 RID: 1651
		public IvyParameterFloat randomRotationX = 0f;

		// Token: 0x04000674 RID: 1652
		public IvyParameterFloat randomRotationY = 0f;

		// Token: 0x04000675 RID: 1653
		public IvyParameterFloat randomRotationZ = 0f;

		// Token: 0x04000676 RID: 1654
		public IvyParameterFloat offsetX = 0f;

		// Token: 0x04000677 RID: 1655
		public IvyParameterFloat offsetY = 0f;

		// Token: 0x04000678 RID: 1656
		public IvyParameterFloat offsetZ = 0f;

		// Token: 0x04000679 RID: 1657
		public float LMUVPadding = 0.002f;

		// Token: 0x0400067A RID: 1658
		public Material branchesMaterial;

		// Token: 0x0400067B RID: 1659
		public List<GameObject> leavesPrefabs = new List<GameObject>();

		// Token: 0x0400067C RID: 1660
		public List<float> leavesProb = new List<float>();

		// Token: 0x0400067D RID: 1661
		public bool generateBranches;

		// Token: 0x0400067E RID: 1662
		public bool generateLeaves;

		// Token: 0x0400067F RID: 1663
		public bool generateLightmapUVs;
	}
}
