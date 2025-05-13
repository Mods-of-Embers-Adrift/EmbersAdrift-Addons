using System;
using SoL.Game.Interactives;
using SoL.Game.Objects.Containers;

namespace SoL.Game.UI.Merchants
{
	// Token: 0x02000974 RID: 2420
	public class PersonalBankUI : BaseMerchantUI<InteractivePersonalBank>
	{
		// Token: 0x17000FF7 RID: 4087
		// (get) Token: 0x06004804 RID: 18436 RVA: 0x000707F0 File Offset: 0x0006E9F0
		protected override ContainerType m_containerType
		{
			get
			{
				return ContainerType.PersonalBank;
			}
		}

		// Token: 0x06004805 RID: 18437 RVA: 0x00045BCA File Offset: 0x00043DCA
		protected override bool ButtonClickedInternal()
		{
			return false;
		}
	}
}
