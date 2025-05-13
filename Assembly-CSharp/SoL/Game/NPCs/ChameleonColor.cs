using System;
using SoL.Game.Objects;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.NPCs
{
	// Token: 0x020007F6 RID: 2038
	public class ChameleonColor : GameEntityComponent
	{
		// Token: 0x06003B38 RID: 15160 RVA: 0x0017AC84 File Offset: 0x00178E84
		private void Start()
		{
			if (this.m_renderer == null || base.GameEntity == null || base.GameEntity.Vitals == null)
			{
				base.enabled = false;
				return;
			}
			this.m_shaderPropertyId = Shader.PropertyToID(this.m_propertyName);
			this.m_propertyBlock = MaterialPropertyBlockCache.GetMaterialPropertyBlock(this.m_renderer);
			this.m_currentSaturation = this.m_saturationRange.Max;
		}

		// Token: 0x06003B39 RID: 15161 RVA: 0x0017ACFC File Offset: 0x00178EFC
		private void Update()
		{
			if (this.m_renderer && base.GameEntity && base.GameEntity.Vitals)
			{
				float healthPercent = base.GameEntity.Vitals.HealthPercent;
				float num = this.m_saturationRange.Max;
				if (healthPercent < this.m_vitalsRange.Max)
				{
					float t = (healthPercent - this.m_vitalsRange.Min) / this.m_vitalsRange.Max;
					num = Mathf.Lerp(this.m_saturationRange.Min, this.m_saturationRange.Max, t);
				}
				if (!Mathf.Approximately(num, this.m_currentSaturation))
				{
					this.m_currentSaturation = Mathf.MoveTowards(this.m_currentSaturation, num, this.m_lerpSpeed);
					this.SetSaturation();
					return;
				}
				if (base.GameEntity.Vitals.GetCurrentHealthState() == HealthState.Dead)
				{
					this.m_currentSaturation = this.m_saturationRange.Min;
					this.SetSaturation();
					base.enabled = false;
				}
			}
		}

		// Token: 0x06003B3A RID: 15162 RVA: 0x00068188 File Offset: 0x00066388
		private void SetSaturation()
		{
			if (this.m_renderer && this.m_propertyBlock != null)
			{
				this.m_propertyBlock.SetFloat(this.m_shaderPropertyId, this.m_currentSaturation);
				this.m_renderer.SetPropertyBlock(this.m_propertyBlock);
			}
		}

		// Token: 0x040039A7 RID: 14759
		[SerializeField]
		private MinMaxFloatRange m_vitalsRange = new MinMaxFloatRange(0f, 0.5f);

		// Token: 0x040039A8 RID: 14760
		[SerializeField]
		private MinMaxFloatRange m_saturationRange = new MinMaxFloatRange(0f, 1f);

		// Token: 0x040039A9 RID: 14761
		[SerializeField]
		private Renderer m_renderer;

		// Token: 0x040039AA RID: 14762
		[SerializeField]
		private string m_propertyName;

		// Token: 0x040039AB RID: 14763
		[SerializeField]
		private float m_lerpSpeed = 0.01f;

		// Token: 0x040039AC RID: 14764
		private MaterialPropertyBlock m_propertyBlock;

		// Token: 0x040039AD RID: 14765
		private int m_shaderPropertyId;

		// Token: 0x040039AE RID: 14766
		private float m_currentSaturation = 1f;
	}
}
