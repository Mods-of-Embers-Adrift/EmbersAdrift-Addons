using System;
using UnityEngine;

namespace SoL.Game.Culling
{
	// Token: 0x02000CBA RID: 3258
	public class CulledMonoBehaviour : CulledObject
	{
		// Token: 0x060062C3 RID: 25283 RVA: 0x002059E0 File Offset: 0x00203BE0
		protected override void RefreshCullee()
		{
			base.RefreshCullee();
			if (!this.m_script)
			{
				return;
			}
			if (this.IsCulled())
			{
				if (this.m_script.enabled)
				{
					this.m_script.enabled = false;
					return;
				}
			}
			else if (!this.m_script.enabled)
			{
				this.m_script.enabled = true;
			}
		}

		// Token: 0x04005620 RID: 22048
		[SerializeField]
		private MonoBehaviour m_script;
	}
}
