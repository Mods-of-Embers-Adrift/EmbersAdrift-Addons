using System;
using SoL.Game.Interactives;
using SoL.Game.Objects.Containers;
using SoL.Utilities;

namespace SoL.Game.AuctionHouse
{
	// Token: 0x02000D41 RID: 3393
	public class InteractiveAuctionHouse : BaseNetworkInteractiveStation
	{
		// Token: 0x17001875 RID: 6261
		// (get) Token: 0x06006636 RID: 26166 RVA: 0x00084DB8 File Offset: 0x00082FB8
		protected override string m_tooltipText
		{
			get
			{
				return "Auction House";
			}
		}

		// Token: 0x17001876 RID: 6262
		// (get) Token: 0x06006637 RID: 26167 RVA: 0x00084CC4 File Offset: 0x00082EC4
		protected override ContainerType m_containerType
		{
			get
			{
				return ContainerType.AuctionOutgoing;
			}
		}

		// Token: 0x17001877 RID: 6263
		// (get) Token: 0x06006638 RID: 26168 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool AllowInteractionWhileMissingBag
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17001878 RID: 6264
		// (get) Token: 0x06006639 RID: 26169 RVA: 0x0007CE7E File Offset: 0x0007B07E
		protected override CursorType ActiveCursorType
		{
			get
			{
				return CursorType.MerchantCursor;
			}
		}

		// Token: 0x17001879 RID: 6265
		// (get) Token: 0x0600663A RID: 26170 RVA: 0x0007CE82 File Offset: 0x0007B082
		protected override CursorType InactiveCursorType
		{
			get
			{
				return CursorType.MerchantCursorInactive;
			}
		}
	}
}
