using System;
using SoL.Game.Interactives;
using SoL.Game.Objects.Containers;

namespace SoL.Game.UI.Merchants
{
	// Token: 0x02000967 RID: 2407
	public class LostAndFoundUI : BaseMerchantUI<InteractiveLostAndFound>
	{
		// Token: 0x17000FDB RID: 4059
		// (get) Token: 0x0600477C RID: 18300 RVA: 0x0005D9AA File Offset: 0x0005BBAA
		protected override ContainerType m_containerType
		{
			get
			{
				return ContainerType.LostAndFound;
			}
		}

		// Token: 0x0600477D RID: 18301 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool ButtonClickedInternal()
		{
			return false;
		}
	}
}
