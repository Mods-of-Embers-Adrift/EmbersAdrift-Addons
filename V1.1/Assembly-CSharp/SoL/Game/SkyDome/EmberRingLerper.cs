using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SoL.Game.SkyDome
{
	// Token: 0x0200070C RID: 1804
	public class EmberRingLerper : SunAltitudeLerper
	{
		// Token: 0x0600364D RID: 13901 RVA: 0x00065367 File Offset: 0x00063567
		public IEnumerator Start()
		{
			int num;
			for (int i = 0; i < this.m_materials.Length; i = num + 1)
			{
				if (this.m_materials[i] != null)
				{
					yield return this.m_materials[i].Init();
				}
				num = i;
			}
			yield break;
		}

		// Token: 0x0600364E RID: 13902 RVA: 0x00169480 File Offset: 0x00167680
		public void OnDestroy()
		{
			for (int i = 0; i < this.m_materials.Length; i++)
			{
				if (this.m_materials[i] != null)
				{
					this.m_materials[i].OnDestroy();
				}
			}
		}

		// Token: 0x0600364F RID: 13903 RVA: 0x001694B8 File Offset: 0x001676B8
		protected override void UpdateInternal(float dayNightCycleFraction)
		{
			for (int i = 0; i < this.m_materials.Length; i++)
			{
				if (this.m_materials[i] != null)
				{
					this.m_materials[i].Update(dayNightCycleFraction);
				}
			}
		}

		// Token: 0x04003438 RID: 13368
		private const string kEmission = "_Emission";

		// Token: 0x04003439 RID: 13369
		private const string kEmissionPower = "_Emissionpower";

		// Token: 0x0400343A RID: 13370
		private static readonly int kEmissionId = Shader.PropertyToID("_Emission");

		// Token: 0x0400343B RID: 13371
		private static readonly int kEmissionPowerId = Shader.PropertyToID("_Emissionpower");

		// Token: 0x0400343C RID: 13372
		[SerializeField]
		private EmberRingLerper.MaterialSetting[] m_materials;

		// Token: 0x0200070D RID: 1805
		[Serializable]
		private class MaterialSetting
		{
			// Token: 0x06003652 RID: 13906 RVA: 0x0006539E File Offset: 0x0006359E
			public IEnumerator Init()
			{
				if (this.m_materialReference != null && this.m_materialReference.RuntimeKeyIsValid())
				{
					this.m_materialReferenceHandle = Addressables.LoadAssetAsync<Material>(this.m_materialReference);
					yield return this.m_materialReferenceHandle;
					if (this.m_materialReferenceHandle.Status == AsyncOperationStatus.Succeeded)
					{
						this.m_addressablesMaterial = this.m_materialReferenceHandle.Result;
					}
				}
				if (this.m_material)
				{
					this.m_originalEmission = this.m_material.GetFloat(EmberRingLerper.kEmissionId);
					this.m_originalEmissionPower = this.m_material.GetFloat(EmberRingLerper.kEmissionPowerId);
				}
				yield break;
			}

			// Token: 0x06003653 RID: 13907 RVA: 0x001694F0 File Offset: 0x001676F0
			public void OnDestroy()
			{
				if (this.m_material)
				{
					this.m_material.SetFloat(EmberRingLerper.kEmissionId, this.m_originalEmission);
					this.m_material.SetFloat(EmberRingLerper.kEmissionPowerId, this.m_originalEmissionPower);
				}
				if (this.m_addressablesMaterial)
				{
					this.m_addressablesMaterial.SetFloat(EmberRingLerper.kEmissionId, this.m_originalEmission);
					this.m_addressablesMaterial.SetFloat(EmberRingLerper.kEmissionPowerId, this.m_originalEmissionPower);
					Addressables.Release<Material>(this.m_materialReferenceHandle);
					this.m_addressablesMaterial = null;
				}
			}

			// Token: 0x06003654 RID: 13908 RVA: 0x00169584 File Offset: 0x00167784
			public void Update(float dayNightCycleFraction)
			{
				float value = this.m_emission.Evaluate(dayNightCycleFraction);
				float value2 = this.m_emissionPower.Evaluate(dayNightCycleFraction);
				if (this.m_material)
				{
					this.m_material.SetFloat(EmberRingLerper.kEmissionId, value);
					this.m_material.SetFloat(EmberRingLerper.kEmissionPowerId, value2);
				}
				if (this.m_addressablesMaterial)
				{
					this.m_addressablesMaterial.SetFloat(EmberRingLerper.kEmissionId, value);
					this.m_addressablesMaterial.SetFloat(EmberRingLerper.kEmissionPowerId, value2);
				}
			}

			// Token: 0x0400343D RID: 13373
			private float m_originalEmission;

			// Token: 0x0400343E RID: 13374
			private float m_originalEmissionPower;

			// Token: 0x0400343F RID: 13375
			[SerializeField]
			private AssetReference m_materialReference;

			// Token: 0x04003440 RID: 13376
			private AsyncOperationHandle<Material> m_materialReferenceHandle;

			// Token: 0x04003441 RID: 13377
			[SerializeField]
			private Material m_material;

			// Token: 0x04003442 RID: 13378
			[Tooltip("0.00 midnight\n0.25 sunrise\n0.50 noon\n0.75 sunset\n1.00 midnight")]
			[SerializeField]
			private AnimationCurve m_emission = AnimationCurve.Linear(0f, 0f, 1f, 0f);

			// Token: 0x04003443 RID: 13379
			[Tooltip("0.00 midnight\n0.25 sunrise\n0.50 noon\n0.75 sunset\n1.00 midnight")]
			[SerializeField]
			private AnimationCurve m_emissionPower = AnimationCurve.Linear(0f, 0f, 1f, 0f);

			// Token: 0x04003444 RID: 13380
			[NonSerialized]
			private Material m_addressablesMaterial;
		}
	}
}
