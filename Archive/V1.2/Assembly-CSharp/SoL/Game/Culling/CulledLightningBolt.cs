using System;
using DigitalRuby.ThunderAndLightning;
using UnityEngine;

namespace SoL.Game.Culling
{
	// Token: 0x02000CB9 RID: 3257
	public class CulledLightningBolt : CulledObject
	{
		// Token: 0x060062C1 RID: 25281 RVA: 0x000828A9 File Offset: 0x00080AA9
		protected override void RefreshCullee()
		{
			base.RefreshCullee();
			if (this.m_lightning)
			{
				this.m_lightning.ManualMode = this.IsCulled();
			}
		}

		// Token: 0x0400561F RID: 22047
		[SerializeField]
		private LightningBoltPrefabScriptBase m_lightning;
	}
}
