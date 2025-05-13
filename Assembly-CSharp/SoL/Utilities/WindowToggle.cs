using System;
using SoL.UI;

namespace SoL.Utilities
{
	// Token: 0x020002F4 RID: 756
	[Serializable]
	public class WindowToggle : ObjectToggle<UIWindow>
	{
		// Token: 0x06001572 RID: 5490 RVA: 0x000FC82C File Offset: 0x000FAA2C
		protected override void UpdateObject()
		{
			if (base.State == ToggleController.ToggleState.ON && !this.m_object.Visible)
			{
				this.m_object.Show(false);
			}
			if (base.State == ToggleController.ToggleState.OFF && this.m_object.Visible)
			{
				this.m_object.Hide(false);
			}
		}
	}
}
