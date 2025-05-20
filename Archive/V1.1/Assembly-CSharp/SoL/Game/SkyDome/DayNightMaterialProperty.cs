using System;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.SkyDome
{
	// Token: 0x020006EA RID: 1770
	public class DayNightMaterialProperty : MonoBehaviour, IDayNightToggle
	{
		// Token: 0x17000BD8 RID: 3032
		// (get) Token: 0x06003589 RID: 13705 RVA: 0x000649C7 File Offset: 0x00062BC7
		private DayNightEnableCondition DayNightCondition
		{
			get
			{
				if (!this.m_useZoneDayNightCondition || !(ZoneSettings.SettingsProfile != null))
				{
					return this.m_dayNightCondition;
				}
				return ZoneSettings.SettingsProfile.DayNightCondition;
			}
		}

		// Token: 0x0600358A RID: 13706 RVA: 0x000649EF File Offset: 0x00062BEF
		private void OnEnable()
		{
			if (!GameManager.IsServer && this.DayNightCondition != DayNightEnableCondition.Always)
			{
				SkyDomeManager.RegisterFX(this);
			}
		}

		// Token: 0x0600358B RID: 13707 RVA: 0x00064A06 File Offset: 0x00062C06
		private void OnDisable()
		{
			if (!GameManager.IsServer && this.DayNightCondition != DayNightEnableCondition.Always)
			{
				SkyDomeManager.UnregisterFX(this);
			}
		}

		// Token: 0x0600358C RID: 13708 RVA: 0x00166FCC File Offset: 0x001651CC
		private void Start()
		{
			if (!GameManager.IsServer && this.m_properties != null)
			{
				for (int i = 0; i < this.m_properties.Length; i++)
				{
					DayNightMaterialProperty.DayNightProperty dayNightProperty = this.m_properties[i];
					if (dayNightProperty != null)
					{
						dayNightProperty.Init();
					}
				}
			}
		}

		// Token: 0x0600358D RID: 13709 RVA: 0x00167010 File Offset: 0x00165210
		private void OnDestroy()
		{
			if (!GameManager.IsServer && this.m_properties != null)
			{
				for (int i = 0; i < this.m_properties.Length; i++)
				{
					DayNightMaterialProperty.DayNightProperty dayNightProperty = this.m_properties[i];
					if (dayNightProperty != null)
					{
						dayNightProperty.OnDestroy();
					}
				}
			}
		}

		// Token: 0x17000BD9 RID: 3033
		// (get) Token: 0x0600358E RID: 13710 RVA: 0x00064A1D File Offset: 0x00062C1D
		DayNightEnableCondition IDayNightToggle.DayNightEnableCondition
		{
			get
			{
				return this.DayNightCondition;
			}
		}

		// Token: 0x0600358F RID: 13711 RVA: 0x00167054 File Offset: 0x00165254
		void IDayNightToggle.Toggle(bool isEnabled)
		{
			if (this.m_properties != null && this.DayNightCondition != DayNightEnableCondition.Always)
			{
				for (int i = 0; i < this.m_properties.Length; i++)
				{
					DayNightMaterialProperty.DayNightProperty dayNightProperty = this.m_properties[i];
					if (dayNightProperty != null)
					{
						dayNightProperty.RefreshStatus();
					}
				}
			}
		}

		// Token: 0x04003387 RID: 13191
		[SerializeField]
		private bool m_useZoneDayNightCondition;

		// Token: 0x04003388 RID: 13192
		[SerializeField]
		private DayNightEnableCondition m_dayNightCondition;

		// Token: 0x04003389 RID: 13193
		[SerializeField]
		private DayNightMaterialProperty.DayNightProperty[] m_properties;

		// Token: 0x020006EB RID: 1771
		[Serializable]
		private class DayNightProperty
		{
			// Token: 0x06003591 RID: 13713 RVA: 0x00064A25 File Offset: 0x00062C25
			public void Init()
			{
				if (this.m_material && this.m_property != null)
				{
					this.m_property.Init(this.m_material);
					this.RefreshStatus();
				}
			}

			// Token: 0x06003592 RID: 13714 RVA: 0x00064A53 File Offset: 0x00062C53
			public void OnDestroy()
			{
				FloatMaterialProperty property = this.m_property;
				if (property == null)
				{
					return;
				}
				property.ResetDefaultValue();
			}

			// Token: 0x06003593 RID: 13715 RVA: 0x00167098 File Offset: 0x00165298
			public void RefreshStatus()
			{
				if (this.m_material && this.m_property != null)
				{
					float value = SkyDomeManager.IsDay() ? this.m_dayValue : this.m_nightValue;
					this.m_property.SetValue(value);
				}
			}

			// Token: 0x0400338A RID: 13194
			[SerializeField]
			private Material m_material;

			// Token: 0x0400338B RID: 13195
			[SerializeField]
			private FloatMaterialProperty m_property;

			// Token: 0x0400338C RID: 13196
			[SerializeField]
			private float m_dayValue;

			// Token: 0x0400338D RID: 13197
			[SerializeField]
			private float m_nightValue;
		}
	}
}
