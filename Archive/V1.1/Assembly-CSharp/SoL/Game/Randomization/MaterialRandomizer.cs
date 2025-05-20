using System;
using UnityEngine;

namespace SoL.Game.Randomization
{
	// Token: 0x0200076C RID: 1900
	public class MaterialRandomizer : MaterialRandomizerBase
	{
		// Token: 0x17000CDD RID: 3293
		// (get) Token: 0x06003849 RID: 14409 RVA: 0x0006655E File Offset: 0x0006475E
		private bool m_showMaterialIndex
		{
			get
			{
				return this.m_renderer != null && this.m_renderer.sharedMaterials.Length > 1;
			}
		}

		// Token: 0x0600384A RID: 14410 RVA: 0x0016D308 File Offset: 0x0016B508
		protected override void RandomizeInternal(System.Random seed)
		{
			if (this.m_renderer == null || this.m_materials == null || this.m_materials.Length == 0)
			{
				return;
			}
			int materialIndex = base.GetMaterialIndex(seed, this.m_materials.Length);
			Material material = this.m_materials[materialIndex];
			if (material == null)
			{
				return;
			}
			if (this.m_materialIndex > 0)
			{
				this.SetMaterial(this.m_renderer, material, this.m_materialIndex);
				if (this.m_additionalRenderers != null)
				{
					for (int i = 0; i < this.m_additionalRenderers.Length; i++)
					{
						this.SetMaterial(this.m_additionalRenderers[i], material, this.m_materialIndex);
					}
					return;
				}
			}
			else
			{
				this.SetMaterial(this.m_renderer, material);
				if (this.m_additionalRenderers != null)
				{
					for (int j = 0; j < this.m_additionalRenderers.Length; j++)
					{
						this.SetMaterial(this.m_additionalRenderers[j], material);
					}
				}
			}
		}

		// Token: 0x0600384B RID: 14411 RVA: 0x00066580 File Offset: 0x00064780
		private void SetMaterial(Renderer renderer, Material newMaterial)
		{
			if (renderer == null || newMaterial == null)
			{
				return;
			}
			renderer.material = newMaterial;
		}

		// Token: 0x0600384C RID: 14412 RVA: 0x0016D3DC File Offset: 0x0016B5DC
		private void SetMaterial(Renderer renderer, Material newMaterial, int materialIndex)
		{
			if (renderer == null || newMaterial == null)
			{
				return;
			}
			Material[] materials = this.m_renderer.materials;
			materials[materialIndex] = newMaterial;
			this.m_renderer.materials = materials;
		}

		// Token: 0x04003729 RID: 14121
		private const string kRendererGroupName = "Renderer";

		// Token: 0x0400372A RID: 14122
		private const string kMaterialGroupName = "Materials";

		// Token: 0x0400372B RID: 14123
		[SerializeField]
		private Renderer m_renderer;

		// Token: 0x0400372C RID: 14124
		[SerializeField]
		private Renderer[] m_additionalRenderers;

		// Token: 0x0400372D RID: 14125
		[SerializeField]
		private int m_materialIndex;

		// Token: 0x0400372E RID: 14126
		[SerializeField]
		private Material[] m_materials;
	}
}
