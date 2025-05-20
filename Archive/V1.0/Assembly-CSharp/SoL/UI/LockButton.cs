using System;
using SoL.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoL.UI
{
	// Token: 0x02000369 RID: 873
	public class LockButton : ToggleController
	{
		// Token: 0x170005BC RID: 1468
		// (get) Token: 0x060017D6 RID: 6102 RVA: 0x00052AA3 File Offset: 0x00050CA3
		public bool Locked
		{
			get
			{
				return base.State == ToggleController.ToggleState.ON;
			}
		}

		// Token: 0x170005BD RID: 1469
		// (get) Token: 0x060017D7 RID: 6103 RVA: 0x00052AAE File Offset: 0x00050CAE
		// (set) Token: 0x060017D8 RID: 6104 RVA: 0x00052ABB File Offset: 0x00050CBB
		public bool Interactable
		{
			get
			{
				return this.m_button.interactable;
			}
			set
			{
				this.m_button.interactable = value;
			}
		}

		// Token: 0x060017D9 RID: 6105 RVA: 0x00052AC9 File Offset: 0x00050CC9
		private void Awake()
		{
			this.m_button.onClick.AddListener(new UnityAction(this.LockButtonPressed));
		}

		// Token: 0x060017DA RID: 6106 RVA: 0x00052AE7 File Offset: 0x00050CE7
		private void OnDestroy()
		{
			this.m_button.onClick.RemoveListener(new UnityAction(this.LockButtonPressed));
		}

		// Token: 0x060017DB RID: 6107 RVA: 0x000510A3 File Offset: 0x0004F2A3
		private void LockButtonPressed()
		{
			base.State = ((base.State == ToggleController.ToggleState.ON) ? ToggleController.ToggleState.OFF : ToggleController.ToggleState.ON);
		}

		// Token: 0x04001F68 RID: 8040
		[SerializeField]
		private Button m_button;
	}
}
