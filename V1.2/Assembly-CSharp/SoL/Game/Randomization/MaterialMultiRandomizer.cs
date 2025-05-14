using System;
using UnityEngine;

namespace SoL.Game.Randomization
{
	// Token: 0x0200076A RID: 1898
	public class MaterialMultiRandomizer : MaterialRandomizerBase
	{
		// Token: 0x06003844 RID: 14404 RVA: 0x0016D2C0 File Offset: 0x0016B4C0
		protected override void RandomizeInternal(System.Random seed)
		{
			if (this.m_data == null || this.m_data.Length == 0 || this.m_data[0].MaterialCount <= 0)
			{
				return;
			}
			int materialIndex = base.GetMaterialIndex(seed, this.m_data[0].MaterialCount);
			for (int i = 0; i < this.m_data.Length; i++)
			{
				this.m_data[i].SelectIndex(materialIndex);
			}
		}

		// Token: 0x04003726 RID: 14118
		[SerializeField]
		private MaterialMultiRandomizer.RandomizerMaterialData[] m_data;

		// Token: 0x0200076B RID: 1899
		[Serializable]
		private class RandomizerMaterialData
		{
			// Token: 0x17000CDC RID: 3292
			// (get) Token: 0x06003846 RID: 14406 RVA: 0x00066513 File Offset: 0x00064713
			public int MaterialCount
			{
				get
				{
					if (this.m_materials != null)
					{
						return this.m_materials.Length;
					}
					return 0;
				}
			}

			// Token: 0x06003847 RID: 14407 RVA: 0x00066527 File Offset: 0x00064727
			public void SelectIndex(int index)
			{
				if (this.m_renderer == null || this.m_materials == null || index >= this.m_materials.Length)
				{
					return;
				}
				this.m_renderer.material = this.m_materials[index];
			}

			// Token: 0x04003727 RID: 14119
			[SerializeField]
			private Renderer m_renderer;

			// Token: 0x04003728 RID: 14120
			[SerializeField]
			private Material[] m_materials;
		}
	}
}
