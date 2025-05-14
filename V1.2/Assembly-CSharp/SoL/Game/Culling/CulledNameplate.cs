using System;

namespace SoL.Game.Culling
{
	// Token: 0x02000CBB RID: 3259
	public class CulledNameplate : CulledObject
	{
		// Token: 0x17001797 RID: 6039
		// (get) Token: 0x060062C5 RID: 25285 RVA: 0x000828CF File Offset: 0x00080ACF
		public bool IsVisible
		{
			get
			{
				return !this.IsCulled();
			}
		}
	}
}
