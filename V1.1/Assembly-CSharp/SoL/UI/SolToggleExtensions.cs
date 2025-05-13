using System;

namespace SoL.UI
{
	// Token: 0x02000374 RID: 884
	public static class SolToggleExtensions
	{
		// Token: 0x0600183F RID: 6207 RVA: 0x00053092 File Offset: 0x00051292
		public static bool InteractableAndActiveInHierarchy(this SolToggle toggle)
		{
			return toggle && toggle.interactable && toggle.gameObject.activeInHierarchy;
		}
	}
}
