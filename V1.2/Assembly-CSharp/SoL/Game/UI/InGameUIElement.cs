using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x02000891 RID: 2193
	public abstract class InGameUIElement : MonoBehaviour
	{
		// Token: 0x06003FDC RID: 16348 RVA: 0x00189E68 File Offset: 0x00188068
		protected virtual void Start()
		{
			SceneCompositionManager.ZoneLoadStarted += this.SceneCompositionManagerOnZoneLoadStarted;
			ClientGameManager.UIManager.ResetUIEvent += this.UiManagerOnResetUi;
			LocalPlayer.LocalPlayerInitialized += this.LocalPlayerOnLocalPlayerInitialized;
			LocalPlayer.FadeLoadingScreenShowUi += this.LocalPlayerOnFadeLoadingScreenShowUi;
		}

		// Token: 0x06003FDD RID: 16349 RVA: 0x00189EC4 File Offset: 0x001880C4
		protected virtual void OnDestroy()
		{
			SceneCompositionManager.ZoneLoadStarted -= this.SceneCompositionManagerOnZoneLoadStarted;
			ClientGameManager.UIManager.ResetUIEvent -= this.UiManagerOnResetUi;
			LocalPlayer.LocalPlayerInitialized -= this.LocalPlayerOnLocalPlayerInitialized;
			LocalPlayer.FadeLoadingScreenShowUi -= this.LocalPlayerOnFadeLoadingScreenShowUi;
		}

		// Token: 0x06003FDE RID: 16350
		protected abstract void UiManagerOnResetUi();

		// Token: 0x06003FDF RID: 16351
		protected abstract void SceneCompositionManagerOnZoneLoadStarted(ZoneId obj);

		// Token: 0x06003FE0 RID: 16352
		protected abstract void LocalPlayerOnLocalPlayerInitialized();

		// Token: 0x06003FE1 RID: 16353
		protected abstract void LocalPlayerOnFadeLoadingScreenShowUi();
	}
}
