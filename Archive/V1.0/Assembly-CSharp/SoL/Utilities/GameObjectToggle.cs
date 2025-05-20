using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002F2 RID: 754
	[Serializable]
	public class GameObjectToggle : ObjectToggle<GameObject>
	{
		// Token: 0x0600156E RID: 5486 RVA: 0x000510FE File Offset: 0x0004F2FE
		protected override void UpdateObject()
		{
			if (this.m_object != null && base.State != ToggleController.ToggleState.DEFAULT)
			{
				this.m_object.SetActive(this.m_activeWhenOn ? (base.State == ToggleController.ToggleState.ON) : (base.State == ToggleController.ToggleState.OFF));
			}
		}

		// Token: 0x04001D8D RID: 7565
		[SerializeField]
		private bool m_activeWhenOn = true;
	}
}
