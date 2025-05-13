using System;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x02000899 RID: 2201
	public class InGameUIWindow : InGameUIElement
	{
		// Token: 0x06004027 RID: 16423 RVA: 0x0006B703 File Offset: 0x00069903
		private void Awake()
		{
			this.m_gameUI.gameObject.SetActive(true);
		}

		// Token: 0x06004028 RID: 16424 RVA: 0x0006B716 File Offset: 0x00069916
		protected override void UiManagerOnResetUi()
		{
			if (this.m_gameUI.Visible)
			{
				this.m_gameUI.Hide(true);
			}
		}

		// Token: 0x06004029 RID: 16425 RVA: 0x0006B716 File Offset: 0x00069916
		protected override void SceneCompositionManagerOnZoneLoadStarted(ZoneId obj)
		{
			if (this.m_gameUI.Visible)
			{
				this.m_gameUI.Hide(true);
			}
		}

		// Token: 0x0600402A RID: 16426 RVA: 0x0006B731 File Offset: 0x00069931
		protected override void LocalPlayerOnLocalPlayerInitialized()
		{
			if (!this.m_gameUI.Visible)
			{
				this.m_gameUI.Show(true);
			}
		}

		// Token: 0x0600402B RID: 16427 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void LocalPlayerOnFadeLoadingScreenShowUi()
		{
		}

		// Token: 0x04003E11 RID: 15889
		[SerializeField]
		private UIWindow m_gameUI;
	}
}
