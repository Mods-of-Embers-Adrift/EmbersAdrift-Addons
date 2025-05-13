using System;
using SoL.Game.Objects.Containers;
using SoL.Utilities;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B9E RID: 2974
	public class InteractivePersonalBank : BaseNetworkInteractiveStation
	{
		// Token: 0x170015AD RID: 5549
		// (get) Token: 0x06005C01 RID: 23553 RVA: 0x0007DBCD File Offset: 0x0007BDCD
		protected override string m_tooltipText
		{
			get
			{
				return "Personal Stash";
			}
		}

		// Token: 0x170015AE RID: 5550
		// (get) Token: 0x06005C02 RID: 23554 RVA: 0x000707F0 File Offset: 0x0006E9F0
		protected override ContainerType m_containerType
		{
			get
			{
				return ContainerType.PersonalBank;
			}
		}

		// Token: 0x170015AF RID: 5551
		// (get) Token: 0x06005C03 RID: 23555 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool AllowInteractionWhileMissingBag
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170015B0 RID: 5552
		// (get) Token: 0x06005C04 RID: 23556 RVA: 0x0007CE7E File Offset: 0x0007B07E
		protected override CursorType ActiveCursorType
		{
			get
			{
				return CursorType.MerchantCursor;
			}
		}

		// Token: 0x170015B1 RID: 5553
		// (get) Token: 0x06005C05 RID: 23557 RVA: 0x0007CE82 File Offset: 0x0007B082
		protected override CursorType InactiveCursorType
		{
			get
			{
				return CursorType.MerchantCursorInactive;
			}
		}
	}
}
