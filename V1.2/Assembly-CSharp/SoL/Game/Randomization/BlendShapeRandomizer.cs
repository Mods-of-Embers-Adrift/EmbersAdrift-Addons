using System;
using SoL.Game.Objects;
using UnityEngine;

namespace SoL.Game.Randomization
{
	// Token: 0x02000760 RID: 1888
	public class BlendShapeRandomizer : BaseRandomizer
	{
		// Token: 0x0600382A RID: 14378 RVA: 0x0016CE44 File Offset: 0x0016B044
		private bool TryGetBlendShapeCustomization(int index, out BlendShapeRandomizer.BlendShapeCustomization customization)
		{
			customization = null;
			if (this.m_customSettings != null)
			{
				for (int i = 0; i < this.m_customSettings.Length; i++)
				{
					if (this.m_customSettings[i].Index == index)
					{
						customization = this.m_customSettings[i];
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600382B RID: 14379 RVA: 0x0016CE8C File Offset: 0x0016B08C
		protected override void RandomizeInternal(System.Random seed)
		{
			if (this.m_renderer == null)
			{
				return;
			}
			float delta = this.m_minMaxValue.Delta;
			float min = this.m_minMaxValue.Min;
			int blendShapeCount = this.m_renderer.sharedMesh.blendShapeCount;
			int i = 0;
			while (i < blendShapeCount)
			{
				float num = delta;
				float num2 = min;
				BlendShapeRandomizer.BlendShapeCustomization blendShapeCustomization;
				if (!this.TryGetBlendShapeCustomization(i, out blendShapeCustomization))
				{
					goto IL_7D;
				}
				if (!blendShapeCustomization.Exclude)
				{
					num = blendShapeCustomization.CustomRange.Delta;
					num2 = blendShapeCustomization.CustomRange.Min;
					goto IL_7D;
				}
				IL_C4:
				i++;
				continue;
				IL_7D:
				float value = num2 + (float)seed.NextDouble() * num;
				this.SetBlendShapeWeight(this.m_renderer, i, value);
				for (int j = 0; j < this.m_mirroredRenderers.Length; j++)
				{
					this.SetBlendShapeWeight(this.m_mirroredRenderers[j], i, value);
				}
				goto IL_C4;
			}
		}

		// Token: 0x0600382C RID: 14380 RVA: 0x00066441 File Offset: 0x00064641
		private void SetBlendShapeWeight(SkinnedMeshRenderer smr, int index, float value)
		{
			if (smr && index < smr.sharedMesh.blendShapeCount)
			{
				smr.SetBlendShapeWeight(index, value);
			}
		}

		// Token: 0x04003708 RID: 14088
		[SerializeField]
		private SkinnedMeshRenderer m_renderer;

		// Token: 0x04003709 RID: 14089
		[SerializeField]
		private SkinnedMeshRenderer[] m_mirroredRenderers;

		// Token: 0x0400370A RID: 14090
		[SerializeField]
		private MinMaxFloatRange m_minMaxValue = new MinMaxFloatRange(0f, 100f);

		// Token: 0x0400370B RID: 14091
		[SerializeField]
		private BlendShapeRandomizer.BlendShapeCustomization[] m_customSettings;

		// Token: 0x02000761 RID: 1889
		[Serializable]
		private class BlendShapeCustomization
		{
			// Token: 0x17000CD7 RID: 3287
			// (get) Token: 0x0600382E RID: 14382 RVA: 0x0006647E File Offset: 0x0006467E
			public int Index
			{
				get
				{
					return this.m_index;
				}
			}

			// Token: 0x17000CD8 RID: 3288
			// (get) Token: 0x0600382F RID: 14383 RVA: 0x00066486 File Offset: 0x00064686
			public bool Exclude
			{
				get
				{
					return this.m_exclude;
				}
			}

			// Token: 0x17000CD9 RID: 3289
			// (get) Token: 0x06003830 RID: 14384 RVA: 0x0006648E File Offset: 0x0006468E
			public MinMaxFloatRange CustomRange
			{
				get
				{
					return this.m_customRange;
				}
			}

			// Token: 0x0400370C RID: 14092
			[SerializeField]
			private int m_index;

			// Token: 0x0400370D RID: 14093
			[SerializeField]
			private bool m_exclude;

			// Token: 0x0400370E RID: 14094
			[SerializeField]
			private MinMaxFloatRange m_customRange = new MinMaxFloatRange(0f, 100f);
		}
	}
}
