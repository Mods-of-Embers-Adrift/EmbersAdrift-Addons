using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.GameCamera
{
	// Token: 0x02000DE8 RID: 3560
	internal class MouseDragPrevention
	{
		// Token: 0x1700194E RID: 6478
		// (get) Token: 0x06006A2E RID: 27182 RVA: 0x0008721C File Offset: 0x0008541C
		public bool Prevent
		{
			get
			{
				return this.m_prevent;
			}
		}

		// Token: 0x06006A2F RID: 27183 RVA: 0x00087224 File Offset: 0x00085424
		public MouseDragPrevention(int mouseButton)
		{
			this.m_mouseButton = mouseButton;
		}

		// Token: 0x06006A30 RID: 27184 RVA: 0x0021A354 File Offset: 0x00218554
		public void UpdateExternal()
		{
			if (Input.GetMouseButtonDown(this.m_mouseButton))
			{
				this.m_mouseIsDown = true;
				this.m_prevent = (UIManager.EventSystem && UIManager.EventSystem.IsPointerOverGameObject());
				if (this.m_prevent && InteractionManager.HoveredUIElement)
				{
					this.m_prevent = !InteractionManager.HoveredUIElement.CompareTag("AllowCameraMovement");
				}
			}
			if (this.m_mouseIsDown && Input.GetMouseButtonUp(this.m_mouseButton))
			{
				this.m_mouseIsDown = false;
				this.m_prevent = false;
			}
		}

		// Token: 0x04005C66 RID: 23654
		private const string kAllowCameraMovementTag = "AllowCameraMovement";

		// Token: 0x04005C67 RID: 23655
		private readonly int m_mouseButton;

		// Token: 0x04005C68 RID: 23656
		private bool m_mouseIsDown;

		// Token: 0x04005C69 RID: 23657
		private bool m_prevent;
	}
}
