using System;
using SoL.Game;
using SoL.Game.SkyDome;
using SoL.Managers;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x0200029A RID: 666
	public class MaterialSwapperDayNight : MaterialSwapper, IDayNightToggle
	{
		// Token: 0x17000503 RID: 1283
		// (get) Token: 0x06001426 RID: 5158 RVA: 0x00050220 File Offset: 0x0004E420
		private DayNightEnableCondition DayNightCondition
		{
			get
			{
				if (!this.m_useZoneDayNightCondition || !(ZoneSettings.SettingsProfile != null))
				{
					return this.m_dayNightEnableCondition;
				}
				return ZoneSettings.SettingsProfile.DayNightCondition;
			}
		}

		// Token: 0x06001427 RID: 5159 RVA: 0x00050248 File Offset: 0x0004E448
		private void OnEnable()
		{
			if (!GameManager.IsServer && this.DayNightCondition != DayNightEnableCondition.Always)
			{
				SkyDomeManager.RegisterFX(this);
			}
		}

		// Token: 0x06001428 RID: 5160 RVA: 0x0005025F File Offset: 0x0004E45F
		private void OnDisable()
		{
			if (!GameManager.IsServer && this.DayNightCondition != DayNightEnableCondition.Always)
			{
				SkyDomeManager.UnregisterFX(this);
			}
		}

		// Token: 0x17000504 RID: 1284
		// (get) Token: 0x06001429 RID: 5161 RVA: 0x00050276 File Offset: 0x0004E476
		DayNightEnableCondition IDayNightToggle.DayNightEnableCondition
		{
			get
			{
				return this.DayNightCondition;
			}
		}

		// Token: 0x0600142A RID: 5162 RVA: 0x0005027E File Offset: 0x0004E47E
		void IDayNightToggle.Toggle(bool isEnabled)
		{
			base.ToggleMaterial(isEnabled);
		}

		// Token: 0x04001C7C RID: 7292
		[SerializeField]
		private bool m_useZoneDayNightCondition;

		// Token: 0x04001C7D RID: 7293
		[SerializeField]
		private DayNightEnableCondition m_dayNightEnableCondition;
	}
}
