using System;
using SoL.Game.EffectSystem;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.UI.Skills
{
	// Token: 0x0200092A RID: 2346
	public class AlchemyPulser : MonoBehaviour
	{
		// Token: 0x17000F7B RID: 3963
		// (get) Token: 0x06004501 RID: 17665 RVA: 0x0006E946 File Offset: 0x0006CB46
		// (set) Token: 0x06004502 RID: 17666 RVA: 0x0006E94E File Offset: 0x0006CB4E
		internal AlchemyPowerLevel PowerLevel
		{
			get
			{
				return this.m_alchemyPowerLevel;
			}
			set
			{
				this.m_alchemyPowerLevel = value;
			}
		}

		// Token: 0x06004503 RID: 17667 RVA: 0x0006E957 File Offset: 0x0006CB57
		private void Awake()
		{
			if (!this.m_canvasGroup)
			{
				base.enabled = false;
			}
		}

		// Token: 0x06004504 RID: 17668 RVA: 0x0006E96D File Offset: 0x0006CB6D
		private void Update()
		{
			if (this.m_canvasGroup)
			{
				this.m_canvasGroup.alpha = ((this.PowerLevel == AlchemyPowerLevel.I) ? UIManager.AlchemyPulseAlpha_I : UIManager.AlchemyPulseAlpha_II);
			}
		}

		// Token: 0x0400418C RID: 16780
		[SerializeField]
		private AlchemyPowerLevel m_alchemyPowerLevel = AlchemyPowerLevel.I;

		// Token: 0x0400418D RID: 16781
		[SerializeField]
		private CanvasGroup m_canvasGroup;
	}
}
