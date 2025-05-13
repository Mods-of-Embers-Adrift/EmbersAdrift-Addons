using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x0200084E RID: 2126
	public class ActionBarUI : MonoBehaviour
	{
		// Token: 0x17000E2D RID: 3629
		// (get) Token: 0x06003D4E RID: 15694 RVA: 0x0006984A File Offset: 0x00067A4A
		public ActionBarSlotUI[] ActionBarSlots
		{
			get
			{
				return this.m_actionBarSlots;
			}
		}

		// Token: 0x17000E2E RID: 3630
		// (get) Token: 0x06003D4F RID: 15695 RVA: 0x00069852 File Offset: 0x00067A52
		public ActionBarSlotUI AutoAttackSlot
		{
			get
			{
				return this.m_autoAttackSlot;
			}
		}

		// Token: 0x17000E2F RID: 3631
		// (get) Token: 0x06003D50 RID: 15696 RVA: 0x0006985A File Offset: 0x00067A5A
		public UniversalContainerUI Pouch
		{
			get
			{
				return this.m_pouch;
			}
		}

		// Token: 0x17000E30 RID: 3632
		// (get) Token: 0x06003D51 RID: 15697 RVA: 0x00069862 File Offset: 0x00067A62
		public UniversalContainerUI ReagentPouch
		{
			get
			{
				return this.m_reagentPouch;
			}
		}

		// Token: 0x06003D52 RID: 15698 RVA: 0x0006986A File Offset: 0x00067A6A
		private void Awake()
		{
			if (this.m_pouch)
			{
				this.m_consumablePouchCanvas = this.m_pouch.GetComponent<CanvasGroup>();
			}
		}

		// Token: 0x06003D53 RID: 15699 RVA: 0x0006988A File Offset: 0x00067A8A
		public void TriggerActionBarIndex(int index)
		{
			if (ClientGameManager.UIManager && ClientGameManager.UIManager.AbilityUI)
			{
				ClientGameManager.UIManager.AbilityUI.TriggerIndex(index);
			}
		}

		// Token: 0x06003D54 RID: 15700 RVA: 0x000698B9 File Offset: 0x00067AB9
		public void TriggerConsumableIndex(int index)
		{
			if (this.m_pouch)
			{
				this.m_pouch.TriggerIndex(index);
			}
		}

		// Token: 0x06003D55 RID: 15701 RVA: 0x000698D4 File Offset: 0x00067AD4
		public void TriggerReagentIndex(int index)
		{
			if (this.m_reagentPouch)
			{
				this.m_reagentPouch.TriggerIndex(index);
			}
		}

		// Token: 0x04003C12 RID: 15378
		[SerializeField]
		private ActionBarSlotUI[] m_actionBarSlots;

		// Token: 0x04003C13 RID: 15379
		[SerializeField]
		private ActionBarSlotUI m_autoAttackSlot;

		// Token: 0x04003C14 RID: 15380
		[SerializeField]
		private UniversalContainerUI m_pouch;

		// Token: 0x04003C15 RID: 15381
		[SerializeField]
		private UniversalContainerUI m_reagentPouch;

		// Token: 0x04003C16 RID: 15382
		private CanvasGroup m_consumablePouchCanvas;
	}
}
