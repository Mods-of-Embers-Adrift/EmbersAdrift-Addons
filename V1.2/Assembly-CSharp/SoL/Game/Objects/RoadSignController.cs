using System;
using UnityEngine;

namespace SoL.Game.Objects
{
	// Token: 0x02000A01 RID: 2561
	public class RoadSignController : MonoBehaviour
	{
		// Token: 0x06004DCF RID: 19919 RVA: 0x00074AC6 File Offset: 0x00072CC6
		private void Awake()
		{
			this.UpdateAll();
		}

		// Token: 0x06004DD0 RID: 19920 RVA: 0x001C0EBC File Offset: 0x001BF0BC
		private void UpdateAll()
		{
			if (this.m_signSettings == null)
			{
				return;
			}
			for (int i = 0; i < this.m_signSettings.Length; i++)
			{
				this.m_signSettings[i].PropagateSettings();
			}
		}

		// Token: 0x04004745 RID: 18245
		[SerializeField]
		private RoadSignController.RoadSignCollection[] m_signSettings;

		// Token: 0x02000A02 RID: 2562
		[Serializable]
		private class RoadSignCollection
		{
			// Token: 0x06004DD2 RID: 19922 RVA: 0x001C0EF4 File Offset: 0x001BF0F4
			internal void PropagateSettings()
			{
				if (this.m_settings == null || this.m_signs == null)
				{
					return;
				}
				for (int i = 0; i < this.m_signs.Length; i++)
				{
					if (!(this.m_signs[i] == null))
					{
						this.m_signs[i].InitializeFromController(this.m_settings);
					}
				}
			}

			// Token: 0x04004746 RID: 18246
			[SerializeField]
			private RoadSignSettings m_settings;

			// Token: 0x04004747 RID: 18247
			[SerializeField]
			private RoadSign[] m_signs;
		}
	}
}
