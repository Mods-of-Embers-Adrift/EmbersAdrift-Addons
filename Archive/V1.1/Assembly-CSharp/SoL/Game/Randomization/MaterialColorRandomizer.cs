using System;
using System.Collections;
using SoL.Game.Login.Client.Creation.NewCreation;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Randomization
{
	// Token: 0x02000768 RID: 1896
	public class MaterialColorRandomizer : BaseRandomizer
	{
		// Token: 0x17000CDA RID: 3290
		// (get) Token: 0x0600383F RID: 14399 RVA: 0x0004F9FB File Offset: 0x0004DBFB
		private IEnumerable GetColors
		{
			get
			{
				return SolOdinUtilities.GetColorValues();
			}
		}

		// Token: 0x17000CDB RID: 3291
		// (get) Token: 0x06003840 RID: 14400 RVA: 0x0004FA02 File Offset: 0x0004DC02
		private IEnumerable GetColorSamplers
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<ColorSampler>();
			}
		}

		// Token: 0x06003841 RID: 14401 RVA: 0x0016D1A4 File Offset: 0x0016B3A4
		protected override void RandomizeInternal(System.Random seed)
		{
			if (Application.isPlaying && this.m_renderer != null)
			{
				Color color = Color.white;
				switch (this.m_colorSource)
				{
				case MaterialColorRandomizer.ColorSource.Random:
					color = new Color((float)seed.NextDouble(), (float)seed.NextDouble(), (float)seed.NextDouble(), 1f);
					break;
				case MaterialColorRandomizer.ColorSource.ColorNames:
					color = Colors.GetRandomColor(seed);
					break;
				case MaterialColorRandomizer.ColorSource.UserDefined:
					if (this.m_colors != null && this.m_colors.Length != 0)
					{
						int num = seed.Next(this.m_colors.Length);
						color = this.m_colors[num];
					}
					break;
				case MaterialColorRandomizer.ColorSource.Sampler:
					if (this.m_colorSampler)
					{
						color = this.m_colorSampler.GetRandomColor(seed);
					}
					break;
				}
				this.SetRendererColor(this.m_renderer, color);
				if (this.m_additionalRenderers != null)
				{
					for (int i = 0; i < this.m_additionalRenderers.Length; i++)
					{
						this.SetRendererColor(this.m_additionalRenderers[i], color);
					}
				}
			}
		}

		// Token: 0x06003842 RID: 14402 RVA: 0x000664E7 File Offset: 0x000646E7
		private void SetRendererColor(Renderer rendererToModify, Color color)
		{
			if (rendererToModify)
			{
				ShaderExtensions.SetMaterialColor(rendererToModify, Shader.PropertyToID(this.m_propertyName), color);
			}
		}

		// Token: 0x0400371B RID: 14107
		[SerializeField]
		private Renderer m_renderer;

		// Token: 0x0400371C RID: 14108
		[SerializeField]
		private Renderer[] m_additionalRenderers;

		// Token: 0x0400371D RID: 14109
		[SerializeField]
		private string m_propertyName;

		// Token: 0x0400371E RID: 14110
		[SerializeField]
		private MaterialColorRandomizer.ColorSource m_colorSource;

		// Token: 0x0400371F RID: 14111
		[SerializeField]
		private Color[] m_colors;

		// Token: 0x04003720 RID: 14112
		[SerializeField]
		private ColorSampler m_colorSampler;

		// Token: 0x02000769 RID: 1897
		private enum ColorSource
		{
			// Token: 0x04003722 RID: 14114
			Random,
			// Token: 0x04003723 RID: 14115
			ColorNames,
			// Token: 0x04003724 RID: 14116
			UserDefined,
			// Token: 0x04003725 RID: 14117
			Sampler
		}
	}
}
