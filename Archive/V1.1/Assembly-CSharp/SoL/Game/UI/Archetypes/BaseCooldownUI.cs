using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI.Archetypes
{
	// Token: 0x020009C9 RID: 2505
	public abstract class BaseCooldownUI : MonoBehaviour
	{
		// Token: 0x170010D8 RID: 4312
		// (get) Token: 0x06004C4A RID: 19530
		// (set) Token: 0x06004C4B RID: 19531
		public abstract bool IsActive { get; protected set; }

		// Token: 0x06004C4C RID: 19532 RVA: 0x000739DC File Offset: 0x00071BDC
		public void Init(ArchetypeInstanceUI ui, AbilityCooldownFlags flagBit)
		{
			this.m_ui = ui;
			this.m_instance = ui.Instance;
			this.m_flagBit = flagBit;
			base.gameObject.SetActive(true);
		}

		// Token: 0x06004C4D RID: 19533 RVA: 0x00073A04 File Offset: 0x00071C04
		private void Update()
		{
			if (this.m_flagBit == AbilityCooldownFlags.None || this.m_instance == null)
			{
				return;
			}
			this.UpdateInternal();
		}

		// Token: 0x06004C4E RID: 19534 RVA: 0x0004475B File Offset: 0x0004295B
		protected virtual void UpdateInternal()
		{
		}

		// Token: 0x06004C4F RID: 19535 RVA: 0x00073A1D File Offset: 0x00071C1D
		public virtual void ClearCooldown()
		{
			this.m_overlay.fillAmount = 0f;
			if (this.IsActive)
			{
				this.m_ui.CenterLabel.SetText(string.Empty);
			}
		}

		// Token: 0x06004C50 RID: 19536 RVA: 0x00073A4C File Offset: 0x00071C4C
		public virtual void ResetCooldown()
		{
			this.m_overlay.fillAmount = 0f;
			this.m_instance = null;
			this.m_flagBit = AbilityCooldownFlags.None;
		}

		// Token: 0x04004657 RID: 18007
		[SerializeField]
		protected Image m_overlay;

		// Token: 0x04004658 RID: 18008
		protected ArchetypeInstanceUI m_ui;

		// Token: 0x04004659 RID: 18009
		protected ArchetypeInstance m_instance;

		// Token: 0x0400465A RID: 18010
		protected AbilityCooldownFlags m_flagBit;
	}
}
