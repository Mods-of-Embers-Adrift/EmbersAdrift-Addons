using System;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.SkyDome
{
	// Token: 0x02000712 RID: 1810
	public class RuptureLerper : SunAltitudeLerper
	{
		// Token: 0x06003666 RID: 13926 RVA: 0x00065445 File Offset: 0x00063645
		private void Start()
		{
			FloatMaterialProperty ringEmission = this.m_ringEmission;
			if (ringEmission != null)
			{
				ringEmission.Init(this.m_material);
			}
			FloatMaterialProperty baseEmission = this.m_baseEmission;
			if (baseEmission == null)
			{
				return;
			}
			baseEmission.Init(this.m_material);
		}

		// Token: 0x06003667 RID: 13927 RVA: 0x00065474 File Offset: 0x00063674
		private void OnDestroy()
		{
			FloatMaterialProperty ringEmission = this.m_ringEmission;
			if (ringEmission != null)
			{
				ringEmission.ResetDefaultValue();
			}
			FloatMaterialProperty baseEmission = this.m_baseEmission;
			if (baseEmission == null)
			{
				return;
			}
			baseEmission.ResetDefaultValue();
		}

		// Token: 0x06003668 RID: 13928 RVA: 0x00169840 File Offset: 0x00167A40
		protected override void UpdateInternal(float dayNightCycleFraction)
		{
			if (this.m_ringEmission != null && this.m_ringEmission.HasMaterial)
			{
				float value = this.m_ringEmissionCurve.Evaluate(dayNightCycleFraction);
				this.m_ringEmission.SetValue(value);
			}
			if (this.m_baseEmission != null && this.m_baseEmission.HasMaterial)
			{
				float value2 = this.m_baseEmissionCurve.Evaluate(dayNightCycleFraction);
				this.m_baseEmission.SetValue(value2);
			}
		}

		// Token: 0x04003455 RID: 13397
		[SerializeField]
		private Material m_material;

		// Token: 0x04003456 RID: 13398
		[SerializeField]
		private FloatMaterialProperty m_ringEmission;

		// Token: 0x04003457 RID: 13399
		[Tooltip("0.00 midnight\n0.25 sunrise\n0.50 noon\n0.75 sunset\n1.00 midnight")]
		[SerializeField]
		private AnimationCurve m_ringEmissionCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

		// Token: 0x04003458 RID: 13400
		[SerializeField]
		private FloatMaterialProperty m_baseEmission;

		// Token: 0x04003459 RID: 13401
		[Tooltip("0.00 midnight\n0.25 sunrise\n0.50 noon\n0.75 sunset\n1.00 midnight")]
		[SerializeField]
		private AnimationCurve m_baseEmissionCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);
	}
}
