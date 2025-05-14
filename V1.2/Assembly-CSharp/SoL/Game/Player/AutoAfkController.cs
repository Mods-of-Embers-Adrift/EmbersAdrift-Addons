using System;
using SoL.Networking.Database;
using UnityEngine;

namespace SoL.Game.Player
{
	// Token: 0x020007E7 RID: 2023
	public class AutoAfkController : MonoBehaviour
	{
		// Token: 0x06003ADC RID: 15068 RVA: 0x00067E02 File Offset: 0x00066002
		public void Init(PlayerCharacterData controller)
		{
			this.m_controller = controller;
		}

		// Token: 0x06003ADD RID: 15069 RVA: 0x0017978C File Offset: 0x0017798C
		private void Update()
		{
			if (this.m_controller)
			{
				bool flag = Time.time - LocalPlayer.TimeOfLastInput > 300f;
				bool flag2 = this.m_controller.PresenceFlags.HasBitFlag(PresenceFlags.AwayAutomatic);
				if (flag && !flag2)
				{
					this.m_controller.PresenceFlags |= PresenceFlags.AwayAutomatic;
					return;
				}
				if (!flag && flag2)
				{
					this.m_controller.PresenceFlags &= ~PresenceFlags.AwayAutomatic;
				}
			}
		}

		// Token: 0x04003968 RID: 14696
		private const PresenceFlags kAutoAfkFlag = PresenceFlags.AwayAutomatic;

		// Token: 0x04003969 RID: 14697
		private PlayerCharacterData m_controller;
	}
}
