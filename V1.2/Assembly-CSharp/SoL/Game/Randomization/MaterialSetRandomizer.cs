using System;
using UnityEngine;

namespace SoL.Game.Randomization
{
	// Token: 0x0200076E RID: 1902
	public class MaterialSetRandomizer : MaterialRandomizerBase
	{
		// Token: 0x06003850 RID: 14416 RVA: 0x0016D4B4 File Offset: 0x0016B6B4
		protected override void RandomizeInternal(System.Random seed)
		{
			if (this.m_renderer == null || this.m_materialSets == null || this.m_materialSets.Length == 0)
			{
				return;
			}
			Material[] materials = this.m_renderer.materials;
			int materialIndex = base.GetMaterialIndex(seed, this.m_materialSets.Length);
			MaterialSetRandomizer.MaterialSet materialSet = this.m_materialSets[materialIndex];
			if (materialSet.Materials.Length == materials.Length)
			{
				for (int i = 0; i < materials.Length; i++)
				{
					materials[i] = materialSet.Materials[i];
				}
				this.m_renderer.materials = materials;
			}
		}

		// Token: 0x04003730 RID: 14128
		[SerializeField]
		private Renderer m_renderer;

		// Token: 0x04003731 RID: 14129
		[SerializeField]
		private MaterialSetRandomizer.MaterialSet[] m_materialSets;

		// Token: 0x0200076F RID: 1903
		[Serializable]
		private class MaterialSet
		{
			// Token: 0x04003732 RID: 14130
			public Material[] Materials;
		}
	}
}
