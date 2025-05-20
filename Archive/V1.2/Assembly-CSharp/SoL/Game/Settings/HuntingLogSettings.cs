using System;
using SoL.Game.HuntingLog;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x0200072B RID: 1835
	[Serializable]
	public class HuntingLogSettings
	{
		// Token: 0x17000C4E RID: 3150
		// (get) Token: 0x06003709 RID: 14089 RVA: 0x00065B19 File Offset: 0x00063D19
		public bool Enabled
		{
			get
			{
				return this.m_enabled;
			}
		}

		// Token: 0x17000C4F RID: 3151
		// (get) Token: 0x0600370A RID: 14090 RVA: 0x00065B21 File Offset: 0x00063D21
		public bool Disabled
		{
			get
			{
				return !this.m_enabled;
			}
		}

		// Token: 0x17000C50 RID: 3152
		// (get) Token: 0x0600370B RID: 14091 RVA: 0x00065B2C File Offset: 0x00063D2C
		public int Version
		{
			get
			{
				return this.m_version;
			}
		}

		// Token: 0x17000C51 RID: 3153
		// (get) Token: 0x0600370C RID: 14092 RVA: 0x00065B34 File Offset: 0x00063D34
		public HuntingLogSettingsProfile DefaultSettings
		{
			get
			{
				return this.m_defaultSettings;
			}
		}

		// Token: 0x04003596 RID: 13718
		[SerializeField]
		private bool m_enabled = true;

		// Token: 0x04003597 RID: 13719
		[SerializeField]
		private int m_version = 1;

		// Token: 0x04003598 RID: 13720
		[SerializeField]
		private HuntingLogSettingsProfile m_defaultSettings;
	}
}
