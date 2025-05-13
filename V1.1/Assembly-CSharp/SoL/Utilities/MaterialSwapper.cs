using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000299 RID: 665
	public class MaterialSwapper : MonoBehaviour
	{
		// Token: 0x06001420 RID: 5152 RVA: 0x000501C3 File Offset: 0x0004E3C3
		private void Awake()
		{
			if (!this.m_renderer || !this.m_enabledMat || !this.m_disabledMat)
			{
				base.enabled = false;
				return;
			}
			this.ToggleMaterial(this.m_startingState);
		}

		// Token: 0x06001421 RID: 5153 RVA: 0x000F942C File Offset: 0x000F762C
		protected void ToggleMaterial(bool isOn)
		{
			if (!this.m_renderer || !this.m_enabledMat || !this.m_disabledMat)
			{
				return;
			}
			this.m_renderer.sharedMaterial = (isOn ? this.m_enabledMat : this.m_disabledMat);
		}

		// Token: 0x06001422 RID: 5154 RVA: 0x00050200 File Offset: 0x0004E400
		private void EnableMaterial()
		{
			this.ToggleMaterial(true);
		}

		// Token: 0x06001423 RID: 5155 RVA: 0x00050209 File Offset: 0x0004E409
		private void DisableMaterial()
		{
			this.ToggleMaterial(false);
		}

		// Token: 0x06001424 RID: 5156 RVA: 0x00050212 File Offset: 0x0004E412
		private void DefaultMaterial()
		{
			this.ToggleMaterial(this.m_startingState);
		}

		// Token: 0x04001C78 RID: 7288
		[SerializeField]
		private MeshRenderer m_renderer;

		// Token: 0x04001C79 RID: 7289
		[SerializeField]
		private Material m_enabledMat;

		// Token: 0x04001C7A RID: 7290
		[SerializeField]
		private Material m_disabledMat;

		// Token: 0x04001C7B RID: 7291
		[SerializeField]
		private bool m_startingState;
	}
}
