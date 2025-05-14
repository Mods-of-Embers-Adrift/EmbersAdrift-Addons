using System;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Login.Client
{
	// Token: 0x02000B36 RID: 2870
	public class LoginStageCharacterCreation : LoginStage
	{
		// Token: 0x06005833 RID: 22579 RVA: 0x0007AD9F File Offset: 0x00078F9F
		public override void StageEnter()
		{
			base.StageEnter();
			base.StatusUpdate(string.Empty);
			UMAGlibManager.AtCreation = true;
		}

		// Token: 0x06005834 RID: 22580 RVA: 0x0007ADB8 File Offset: 0x00078FB8
		public override void StageExit()
		{
			base.StageExit();
			UMAGlibManager.AtCreation = false;
		}

		// Token: 0x06005835 RID: 22581 RVA: 0x001E5590 File Offset: 0x001E3790
		protected override void UpdateTogglesInternal()
		{
			base.UpdateTogglesInternal();
			if (this.m_firstEnterEnable && !this.m_hasEnabled)
			{
				if (base.State == ToggleController.ToggleState.ON)
				{
					this.m_firstEnterEnable.SetActive(true);
					this.m_hasEnabled = true;
					return;
				}
				this.m_firstEnterEnable.SetActive(false);
			}
		}

		// Token: 0x04004DA9 RID: 19881
		[SerializeField]
		private GameObject m_firstEnterEnable;

		// Token: 0x04004DAA RID: 19882
		private bool m_hasEnabled;
	}
}
