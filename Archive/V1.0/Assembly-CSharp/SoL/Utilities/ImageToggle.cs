using System;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Utilities
{
	// Token: 0x020002F3 RID: 755
	[Serializable]
	public class ImageToggle : ObjectToggle<Image>
	{
		// Token: 0x06001570 RID: 5488 RVA: 0x0005114C File Offset: 0x0004F34C
		protected override void UpdateObject()
		{
			if (this.m_object != null && base.State != ToggleController.ToggleState.DEFAULT)
			{
				this.m_object.sprite = ((base.State == ToggleController.ToggleState.ON) ? this.m_onSprite : this.m_offSprite);
			}
		}

		// Token: 0x04001D8E RID: 7566
		[SerializeField]
		private Sprite m_onSprite;

		// Token: 0x04001D8F RID: 7567
		[SerializeField]
		private Sprite m_offSprite;
	}
}
