using System;
using SoL.Game.Objects.Containers;
using SoL.Utilities;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B96 RID: 2966
	public class InteractiveLostAndFound : BaseNetworkInteractiveStation
	{
		// Token: 0x17001564 RID: 5476
		// (get) Token: 0x06005B59 RID: 23385 RVA: 0x0007D5D7 File Offset: 0x0007B7D7
		protected override string m_tooltipText
		{
			get
			{
				return "Lost & Found";
			}
		}

		// Token: 0x17001565 RID: 5477
		// (get) Token: 0x06005B5A RID: 23386 RVA: 0x0005D9AA File Offset: 0x0005BBAA
		protected override ContainerType m_containerType
		{
			get
			{
				return ContainerType.LostAndFound;
			}
		}

		// Token: 0x17001566 RID: 5478
		// (get) Token: 0x06005B5B RID: 23387 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool AllowInteractionWhileMissingBag
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17001567 RID: 5479
		// (get) Token: 0x06005B5C RID: 23388 RVA: 0x0007CE7E File Offset: 0x0007B07E
		protected override CursorType ActiveCursorType
		{
			get
			{
				return CursorType.MerchantCursor;
			}
		}

		// Token: 0x17001568 RID: 5480
		// (get) Token: 0x06005B5D RID: 23389 RVA: 0x0007CE82 File Offset: 0x0007B082
		protected override CursorType InactiveCursorType
		{
			get
			{
				return CursorType.MerchantCursorInactive;
			}
		}
	}
}
