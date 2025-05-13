using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000276 RID: 630
	public class DisableRendererOnAwake : MonoBehaviour
	{
		// Token: 0x060013C2 RID: 5058 RVA: 0x0004FE11 File Offset: 0x0004E011
		private void Awake()
		{
			if (this.m_renderer != null)
			{
				this.m_renderer.enabled = false;
			}
		}

		// Token: 0x04001BFE RID: 7166
		[SerializeField]
		private Renderer m_renderer;
	}
}
