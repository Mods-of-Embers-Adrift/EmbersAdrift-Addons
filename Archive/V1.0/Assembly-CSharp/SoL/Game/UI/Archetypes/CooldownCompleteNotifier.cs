using System;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI.Archetypes
{
	// Token: 0x020009CA RID: 2506
	public class CooldownCompleteNotifier : MonoBehaviour
	{
		// Token: 0x06004C52 RID: 19538 RVA: 0x00073A6C File Offset: 0x00071C6C
		private void Awake()
		{
			if (this.m_availableNotifier)
			{
				this.m_availableNotifier.enabled = false;
			}
		}

		// Token: 0x06004C53 RID: 19539 RVA: 0x001BC650 File Offset: 0x001BA850
		private void Update()
		{
			if (!this.m_availableNotifier || !this.m_cooldownUI)
			{
				return;
			}
			if (this.m_wasActiveLastFrame != this.m_cooldownUI.IsActive)
			{
				if (this.m_cooldownUI.IsActive)
				{
					this.m_availableNotifier.enabled = false;
					this.m_availableNotifier.color = Color.white;
				}
				else
				{
					this.m_availableNotifier.color = Color.white;
					this.m_availableNotifier.enabled = true;
					this.m_availableNotifier.CrossFadeAlpha(0f, 1f, true);
				}
			}
			this.m_wasActiveLastFrame = this.m_cooldownUI.IsActive;
		}

		// Token: 0x0400465B RID: 18011
		[SerializeField]
		private BaseCooldownUI m_cooldownUI;

		// Token: 0x0400465C RID: 18012
		[SerializeField]
		private Image m_availableNotifier;

		// Token: 0x0400465D RID: 18013
		private bool m_wasActiveLastFrame;
	}
}
