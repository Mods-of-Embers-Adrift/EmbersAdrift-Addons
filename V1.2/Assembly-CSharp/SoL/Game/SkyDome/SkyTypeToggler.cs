using System;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine.Rendering.HighDefinition;

namespace SoL.Game.SkyDome
{
	// Token: 0x02000709 RID: 1801
	public class SkyTypeToggler : VolumeComponentModifier<VisualEnvironment>
	{
		// Token: 0x0600363F RID: 13887 RVA: 0x0006523F File Offset: 0x0006343F
		protected override void Awake()
		{
			base.Awake();
			base.Component.skyType.overrideState = false;
			SceneCompositionManager.SceneCompositionLoaded += this.Initialize;
		}

		// Token: 0x06003640 RID: 13888 RVA: 0x00065269 File Offset: 0x00063469
		private void Initialize()
		{
			base.Component.skyType.overrideState = true;
			SceneCompositionManager.SceneCompositionLoaded -= this.Initialize;
		}
	}
}
