using System;
using SoL.Game.Discovery;
using SoL.Managers;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Interactives
{
	// Token: 0x02000B84 RID: 2948
	public class InteractiveDocument : BaseNetworkedInteractive, IInteractive, IInteractiveBase, ICursor
	{
		// Token: 0x17001541 RID: 5441
		// (get) Token: 0x06005ADD RID: 23261 RVA: 0x0004479C File Offset: 0x0004299C
		protected override bool AllowInteractionWhileMissingBag
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06005ADE RID: 23262 RVA: 0x001EDB3C File Offset: 0x001EBD3C
		public override bool ClientInteraction()
		{
			if (this.m_loreProfile != null && LocalPlayer.GameEntity && base.ClientInteraction())
			{
				ClientGameManager.UIManager.DocumentUI.CurrentInteractive = this;
				ClientGameManager.UIManager.DocumentUI.Show(false);
				ClientGameManager.UIManager.DocumentUI.SetInkFile(this.m_loreProfile.InkFile);
				return true;
			}
			return false;
		}

		// Token: 0x17001542 RID: 5442
		// (get) Token: 0x06005ADF RID: 23263 RVA: 0x00061BE2 File Offset: 0x0005FDE2
		protected override CursorType ActiveCursorType
		{
			get
			{
				return CursorType.IdentifyingGlassCursor;
			}
		}

		// Token: 0x17001543 RID: 5443
		// (get) Token: 0x06005AE0 RID: 23264 RVA: 0x0004479C File Offset: 0x0004299C
		protected override CursorType InactiveCursorType
		{
			get
			{
				return CursorType.MainCursor;
			}
		}

		// Token: 0x06005AE2 RID: 23266 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x04004F9C RID: 20380
		[SerializeField]
		private LoreProfile m_loreProfile;
	}
}
