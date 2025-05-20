using System;
using UnityEngine;

namespace SoL.Game.SkyDome
{
	// Token: 0x020006EC RID: 1772
	public class EmberRingMaterialLerp : MonoBehaviour
	{
		// Token: 0x06003595 RID: 13717 RVA: 0x001670FC File Offset: 0x001652FC
		private void Start()
		{
			for (int i = 0; i < this.m_sets.Length; i++)
			{
				this.m_sets[i].GetOriginalValues();
			}
			this.SetCurrentHour();
			EmberRingMaterialLerp.TimePeriod fixedTimePeriod = this.GetFixedTimePeriod(this.m_currentHour);
			this.UpdateMaterialValues(fixedTimePeriod, fixedTimePeriod, 1f);
		}

		// Token: 0x06003596 RID: 13718 RVA: 0x0016714C File Offset: 0x0016534C
		private void OnDestroy()
		{
			for (int i = 0; i < this.m_sets.Length; i++)
			{
				this.m_sets[i].ResetToOriginalValue();
			}
		}

		// Token: 0x06003597 RID: 13719 RVA: 0x0016717C File Offset: 0x0016537C
		private void Update()
		{
			if (SkyDomeManager.SkyDomeController == null)
			{
				return;
			}
			if (Time.time > this.m_nextTimeRefresh)
			{
				this.SetCurrentHour();
			}
			float? num = null;
			EmberRingMaterialLerp.TimePeriod timePeriod = EmberRingMaterialLerp.TimePeriod.Midnight;
			EmberRingMaterialLerp.TimePeriod targetTimePeriod = EmberRingMaterialLerp.TimePeriod.Midnight;
			for (int i = 0; i < this.m_transitionRanges.Length; i++)
			{
				if (this.m_transitionRanges[i].IsWithinTimeRange(this.m_currentHour))
				{
					num = new float?(this.m_transitionRanges[i].GetTransitionFraction(this.m_currentHour));
					timePeriod = this.m_transitionRanges[i].SourceTimePeriod;
					targetTimePeriod = this.m_transitionRanges[i].TargetTimePeriod;
					break;
				}
			}
			if (num == null)
			{
				num = new float?(1f);
				timePeriod = this.GetFixedTimePeriod(this.m_currentHour);
				targetTimePeriod = timePeriod;
			}
			this.UpdateMaterialValues(timePeriod, targetTimePeriod, num.Value);
		}

		// Token: 0x06003598 RID: 13720 RVA: 0x00167244 File Offset: 0x00165444
		private void SetCurrentHour()
		{
			DateTime dateTime = (SkyDomeManager.SkyDomeController != null) ? SkyDomeManager.SkyDomeController.GetTime() : GameDateTime.UtcNow.DateTime;
			this.m_currentHour = (float)dateTime.Hour + (float)dateTime.Minute / 60f;
			this.m_nextTimeRefresh = Time.time + 1f;
		}

		// Token: 0x06003599 RID: 13721 RVA: 0x001672A0 File Offset: 0x001654A0
		private EmberRingMaterialLerp.TimePeriod GetFixedTimePeriod(float hour)
		{
			for (int i = 0; i < this.m_timePeriodRange.Length; i++)
			{
				if (this.m_timePeriodRange[i].IsWithinTimeRange(hour))
				{
					return this.m_timePeriodRange[i].TimePeriod;
				}
			}
			return EmberRingMaterialLerp.TimePeriod.Midnight;
		}

		// Token: 0x0600359A RID: 13722 RVA: 0x001672E0 File Offset: 0x001654E0
		private void UpdateMaterialValues(EmberRingMaterialLerp.TimePeriod sourceTimePeriod, EmberRingMaterialLerp.TimePeriod targetTimePeriod, float lerpValue)
		{
			for (int i = 0; i < this.m_sets.Length; i++)
			{
				this.m_sets[i].UpdateValues(sourceTimePeriod, targetTimePeriod, lerpValue);
			}
		}

		// Token: 0x0400338E RID: 13198
		private const string kEmission = "_Emission";

		// Token: 0x0400338F RID: 13199
		private const string kEmissionPower = "_Emissionpower";

		// Token: 0x04003390 RID: 13200
		private static readonly int kEmissionId = Shader.PropertyToID("_Emission");

		// Token: 0x04003391 RID: 13201
		private static readonly int kEmissionPowerId = Shader.PropertyToID("_Emissionpower");

		// Token: 0x04003392 RID: 13202
		private const float kTimeRefresh = 1f;

		// Token: 0x04003393 RID: 13203
		[SerializeField]
		private EmberRingMaterialLerp.MaterialSettingSet[] m_sets;

		// Token: 0x04003394 RID: 13204
		[SerializeField]
		private EmberRingMaterialLerp.TimePeriodRange[] m_timePeriodRange;

		// Token: 0x04003395 RID: 13205
		[SerializeField]
		private EmberRingMaterialLerp.TimePeriodTransitionRange[] m_transitionRanges;

		// Token: 0x04003396 RID: 13206
		private float m_nextTimeRefresh;

		// Token: 0x04003397 RID: 13207
		private float m_currentHour;

		// Token: 0x020006ED RID: 1773
		private enum TimePeriod
		{
			// Token: 0x04003399 RID: 13209
			DawnDusk,
			// Token: 0x0400339A RID: 13210
			Midnight,
			// Token: 0x0400339B RID: 13211
			Noon
		}

		// Token: 0x020006EE RID: 1774
		[Serializable]
		private class MaterialSetting
		{
			// Token: 0x17000BDA RID: 3034
			// (get) Token: 0x0600359D RID: 13725 RVA: 0x00064A85 File Offset: 0x00062C85
			public float EmissionPower
			{
				get
				{
					return this.m_emissionPower;
				}
			}

			// Token: 0x17000BDB RID: 3035
			// (get) Token: 0x0600359E RID: 13726 RVA: 0x00064A8D File Offset: 0x00062C8D
			public float Emission
			{
				get
				{
					return this.m_emission;
				}
			}

			// Token: 0x0400339C RID: 13212
			[SerializeField]
			private float m_emissionPower;

			// Token: 0x0400339D RID: 13213
			[SerializeField]
			private float m_emission;
		}

		// Token: 0x020006EF RID: 1775
		[Serializable]
		private class MaterialSettingSet
		{
			// Token: 0x17000BDC RID: 3036
			// (get) Token: 0x060035A0 RID: 13728 RVA: 0x00064A95 File Offset: 0x00062C95
			public string IndexName
			{
				get
				{
					if (!this.m_material)
					{
						return "NONE";
					}
					return this.m_material.name;
				}
			}

			// Token: 0x060035A1 RID: 13729 RVA: 0x00064AB5 File Offset: 0x00062CB5
			private EmberRingMaterialLerp.MaterialSetting GetMaterialSetting(EmberRingMaterialLerp.TimePeriod timePeriod)
			{
				switch (timePeriod)
				{
				case EmberRingMaterialLerp.TimePeriod.DawnDusk:
					return this.m_dawnDusk;
				case EmberRingMaterialLerp.TimePeriod.Midnight:
					return this.m_midnight;
				case EmberRingMaterialLerp.TimePeriod.Noon:
					return this.m_noon;
				default:
					throw new ArgumentException("timePeriod");
				}
			}

			// Token: 0x060035A2 RID: 13730 RVA: 0x00064AEA File Offset: 0x00062CEA
			public void GetOriginalValues()
			{
				if (!this.m_material)
				{
					return;
				}
				this.m_originalEmissionPower = this.m_material.GetFloat(EmberRingMaterialLerp.kEmissionPowerId);
				this.m_originalEmission = this.m_material.GetFloat(EmberRingMaterialLerp.kEmissionId);
			}

			// Token: 0x060035A3 RID: 13731 RVA: 0x00064B26 File Offset: 0x00062D26
			public void ResetToOriginalValue()
			{
				if (!this.m_material)
				{
					return;
				}
				this.m_material.SetFloat(EmberRingMaterialLerp.kEmissionPowerId, this.m_originalEmissionPower);
				this.m_material.SetFloat(EmberRingMaterialLerp.kEmissionId, this.m_originalEmission);
			}

			// Token: 0x060035A4 RID: 13732 RVA: 0x00167310 File Offset: 0x00165510
			public void UpdateValues(EmberRingMaterialLerp.TimePeriod sourceTime, EmberRingMaterialLerp.TimePeriod targetTime, float lerpValue)
			{
				if (!this.m_material)
				{
					return;
				}
				if (sourceTime == this.m_previousSourceTimePeriod && targetTime == this.m_previousTargetTimePeriod && lerpValue == this.m_previousLerpValue)
				{
					return;
				}
				EmberRingMaterialLerp.MaterialSetting materialSetting = this.GetMaterialSetting(sourceTime);
				EmberRingMaterialLerp.MaterialSetting materialSetting2 = this.GetMaterialSetting(targetTime);
				float value = Mathf.Lerp(materialSetting.EmissionPower, materialSetting2.EmissionPower, lerpValue);
				float value2 = Mathf.Lerp(materialSetting.Emission, materialSetting2.Emission, lerpValue);
				this.m_material.SetFloat(EmberRingMaterialLerp.kEmissionPowerId, value);
				this.m_material.SetFloat(EmberRingMaterialLerp.kEmissionId, value2);
				this.m_previousSourceTimePeriod = sourceTime;
				this.m_previousTargetTimePeriod = targetTime;
				this.m_previousLerpValue = lerpValue;
			}

			// Token: 0x0400339E RID: 13214
			[SerializeField]
			private Material m_material;

			// Token: 0x0400339F RID: 13215
			[SerializeField]
			private EmberRingMaterialLerp.MaterialSetting m_noon;

			// Token: 0x040033A0 RID: 13216
			[SerializeField]
			private EmberRingMaterialLerp.MaterialSetting m_midnight;

			// Token: 0x040033A1 RID: 13217
			[SerializeField]
			private EmberRingMaterialLerp.MaterialSetting m_dawnDusk;

			// Token: 0x040033A2 RID: 13218
			private float m_originalEmissionPower;

			// Token: 0x040033A3 RID: 13219
			private float m_originalEmission;

			// Token: 0x040033A4 RID: 13220
			private EmberRingMaterialLerp.TimePeriod m_previousSourceTimePeriod = EmberRingMaterialLerp.TimePeriod.Midnight;

			// Token: 0x040033A5 RID: 13221
			private EmberRingMaterialLerp.TimePeriod m_previousTargetTimePeriod = EmberRingMaterialLerp.TimePeriod.Midnight;

			// Token: 0x040033A6 RID: 13222
			private float m_previousLerpValue = -1f;
		}

		// Token: 0x020006F0 RID: 1776
		[Serializable]
		private class TimePeriodTransitionRange
		{
			// Token: 0x17000BDD RID: 3037
			// (get) Token: 0x060035A6 RID: 13734 RVA: 0x00064B83 File Offset: 0x00062D83
			public EmberRingMaterialLerp.TimePeriod SourceTimePeriod
			{
				get
				{
					return this.m_sourceTimePeriod;
				}
			}

			// Token: 0x17000BDE RID: 3038
			// (get) Token: 0x060035A7 RID: 13735 RVA: 0x00064B8B File Offset: 0x00062D8B
			public EmberRingMaterialLerp.TimePeriod TargetTimePeriod
			{
				get
				{
					return this.m_targetTimePeriod;
				}
			}

			// Token: 0x17000BDF RID: 3039
			// (get) Token: 0x060035A8 RID: 13736 RVA: 0x001673B4 File Offset: 0x001655B4
			public string IndexName
			{
				get
				{
					return string.Concat(new string[]
					{
						"[",
						this.m_transitionTime.x.ToString(),
						"-",
						this.m_transitionTime.y.ToString(),
						"] ",
						this.m_sourceTimePeriod.ToString(),
						" to ",
						this.m_targetTimePeriod.ToString()
					});
				}
			}

			// Token: 0x060035A9 RID: 13737 RVA: 0x00064B93 File Offset: 0x00062D93
			public bool IsWithinTimeRange(float hour)
			{
				return hour >= this.m_transitionTime.x && hour < this.m_transitionTime.y;
			}

			// Token: 0x060035AA RID: 13738 RVA: 0x0016743C File Offset: 0x0016563C
			public float GetTransitionFraction(float hour)
			{
				float num = this.m_transitionTime.y - this.m_transitionTime.x;
				return (hour - this.m_transitionTime.x) / num;
			}

			// Token: 0x040033A7 RID: 13223
			[SerializeField]
			private EmberRingMaterialLerp.TimePeriod m_sourceTimePeriod = EmberRingMaterialLerp.TimePeriod.Midnight;

			// Token: 0x040033A8 RID: 13224
			[SerializeField]
			private EmberRingMaterialLerp.TimePeriod m_targetTimePeriod = EmberRingMaterialLerp.TimePeriod.Midnight;

			// Token: 0x040033A9 RID: 13225
			[SerializeField]
			private Vector2 m_transitionTime = Vector2.zero;
		}

		// Token: 0x020006F1 RID: 1777
		[Serializable]
		private class TimePeriodRange
		{
			// Token: 0x17000BE0 RID: 3040
			// (get) Token: 0x060035AC RID: 13740 RVA: 0x00167470 File Offset: 0x00165670
			public string IndexName
			{
				get
				{
					return string.Concat(new string[]
					{
						"[",
						this.m_timeRange.x.ToString(),
						"-",
						this.m_timeRange.y.ToString(),
						"] ",
						this.m_timePeriod.ToString()
					});
				}
			}

			// Token: 0x17000BE1 RID: 3041
			// (get) Token: 0x060035AD RID: 13741 RVA: 0x00064BD4 File Offset: 0x00062DD4
			public EmberRingMaterialLerp.TimePeriod TimePeriod
			{
				get
				{
					return this.m_timePeriod;
				}
			}

			// Token: 0x060035AE RID: 13742 RVA: 0x00064BDC File Offset: 0x00062DDC
			public bool IsWithinTimeRange(float hour)
			{
				return hour >= this.m_timeRange.x && hour < this.m_timeRange.y;
			}

			// Token: 0x040033AA RID: 13226
			[SerializeField]
			private EmberRingMaterialLerp.TimePeriod m_timePeriod = EmberRingMaterialLerp.TimePeriod.Midnight;

			// Token: 0x040033AB RID: 13227
			[SerializeField]
			private Vector2 m_timeRange = Vector2.zero;
		}
	}
}
