using System;
using SoL.Managers;
using TMPro;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x02000372 RID: 882
	public class SolTMP_InputField : TMP_InputField
	{
		// Token: 0x0600182C RID: 6188 RVA: 0x00052F6D File Offset: 0x0005116D
		public void CaretToEnd()
		{
			base.caretPosition = base.text.Length;
		}

		// Token: 0x0600182D RID: 6189 RVA: 0x00052F80 File Offset: 0x00051180
		public void Activate()
		{
			this.Select();
			base.ActivateInputField();
			this.m_moveToNextFrameCount = new int?(Time.frameCount);
		}

		// Token: 0x0600182E RID: 6190 RVA: 0x00052F9E File Offset: 0x0005119E
		public void Deactivate()
		{
			base.DeactivateInputField(true);
			if (UIManager.EventSystem != null && UIManager.EventSystem.currentSelectedGameObject == base.gameObject)
			{
				UIManager.EventSystem.SetSelectedGameObject(null);
			}
		}

		// Token: 0x0600182F RID: 6191 RVA: 0x00052FD6 File Offset: 0x000511D6
		protected override void LateUpdate()
		{
			base.LateUpdate();
			if (this.m_moveToNextFrameCount != null && Time.frameCount > this.m_moveToNextFrameCount.Value)
			{
				base.MoveTextEnd(true);
				this.m_moveToNextFrameCount = null;
			}
		}

		// Token: 0x04001F8C RID: 8076
		private int? m_moveToNextFrameCount;
	}
}
