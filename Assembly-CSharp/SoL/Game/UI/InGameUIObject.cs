using System;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x02000894 RID: 2196
	public class InGameUIObject : InGameUIElement
	{
		// Token: 0x06003FF4 RID: 16372 RVA: 0x0006B40D File Offset: 0x0006960D
		private void Awake()
		{
			this.m_obj.SetActive(false);
		}

		// Token: 0x06003FF5 RID: 16373 RVA: 0x0006B40D File Offset: 0x0006960D
		protected override void UiManagerOnResetUi()
		{
			this.m_obj.SetActive(false);
		}

		// Token: 0x06003FF6 RID: 16374 RVA: 0x0006B40D File Offset: 0x0006960D
		protected override void SceneCompositionManagerOnZoneLoadStarted(ZoneId obj)
		{
			this.m_obj.SetActive(false);
		}

		// Token: 0x06003FF7 RID: 16375 RVA: 0x0006B41B File Offset: 0x0006961B
		protected override void LocalPlayerOnLocalPlayerInitialized()
		{
			this.m_obj.SetActive(true);
		}

		// Token: 0x06003FF8 RID: 16376 RVA: 0x0004475B File Offset: 0x0004295B
		protected override void LocalPlayerOnFadeLoadingScreenShowUi()
		{
		}

		// Token: 0x04003D86 RID: 15750
		[SerializeField]
		private GameObject m_obj;
	}
}
