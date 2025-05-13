using System;
using SoL.Managers;

namespace SoL.Game.SkyDome
{
	// Token: 0x02000705 RID: 1797
	public class ServerSkyController : BaseSkyController
	{
		// Token: 0x060035FB RID: 13819 RVA: 0x00064FE7 File Offset: 0x000631E7
		private void Start()
		{
			if (!GameManager.IsServer)
			{
				base.enabled = false;
				return;
			}
			this.m_internalDateTime.DateTime = SkyDomeManager.GetCorrectedGameDateTime();
			this.Initialize();
			base.InvokeRepeating("UpdateServer", 60f, 60f);
		}

		// Token: 0x060035FC RID: 13820 RVA: 0x00065023 File Offset: 0x00063223
		private void OnDestroy()
		{
			base.CancelInvoke("UpdateServer");
		}

		// Token: 0x060035FD RID: 13821 RVA: 0x00065030 File Offset: 0x00063230
		private void UpdateServer()
		{
			this.m_internalDateTime.DateTime = SkyDomeManager.GetCorrectedGameDateTime();
			base.UpdateCelestials();
		}

		// Token: 0x060035FE RID: 13822 RVA: 0x00065048 File Offset: 0x00063248
		protected override void SetTime(DateTime time, bool updateReflections)
		{
			base.SetTime(time, updateReflections);
			if (base.IsInitialized)
			{
				base.UpdateCelestials();
			}
		}

		// Token: 0x060035FF RID: 13823 RVA: 0x00064FAA File Offset: 0x000631AA
		protected override void UpdateIsDayInternal()
		{
			base.UpdateIsDayInternal();
			base.IsDay = (base.SunAltitude > 0f);
		}

		// Token: 0x040033FE RID: 13310
		private const float kCadence = 60f;
	}
}
