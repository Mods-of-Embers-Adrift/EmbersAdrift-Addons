using System;
using Cysharp.Text;
using SoL.Game.States;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x02000551 RID: 1361
	public class ActivatedMonolithStateTrigger : MonoBehaviour
	{
		// Token: 0x0600295C RID: 10588 RVA: 0x00141330 File Offset: 0x0013F530
		private void Awake()
		{
			if (this.m_requirements != null)
			{
				for (int i = 0; i < this.m_requirements.Length; i++)
				{
					if (this.m_requirements[i] != null && !GameManager.IsServer)
					{
						this.m_requirements[i].InjectTooltip();
					}
				}
			}
			if (!GameManager.IsServer)
			{
				base.enabled = false;
			}
		}

		// Token: 0x04002A4C RID: 10828
		internal const int kMaxPedestalState = 3;

		// Token: 0x04002A4D RID: 10829
		[SerializeField]
		private ActivatedMonolithStateTrigger.MonolithStateSetting[] m_requirements;

		// Token: 0x04002A4E RID: 10830
		[SerializeField]
		private StateSetting[] m_toTrigger;

		// Token: 0x04002A4F RID: 10831
		[SerializeField]
		private DelayedStateSet[] m_delayedToTrigger;

		// Token: 0x02000552 RID: 1362
		[Serializable]
		private class MonolithStateSetting
		{
			// Token: 0x17000879 RID: 2169
			// (get) Token: 0x0600295E RID: 10590 RVA: 0x0005C9F9 File Offset: 0x0005ABF9
			public BaseState State
			{
				get
				{
					return this.m_state;
				}
			}

			// Token: 0x0600295F RID: 10591 RVA: 0x00045BCA File Offset: 0x00043DCA
			public byte GetCurrentState()
			{
				return 0;
			}

			// Token: 0x06002960 RID: 10592 RVA: 0x00141384 File Offset: 0x0013F584
			public void InjectTooltip()
			{
				if (this.m_profile && this.m_zoneId != ZoneId.None)
				{
					InteractiveState interactiveState = this.m_state as InteractiveState;
					if (interactiveState != null)
					{
						string formattedZoneName = LocalZoneManager.GetFormattedZoneName(this.m_zoneId, SubZoneId.None);
						if (!string.IsNullOrEmpty(formattedZoneName))
						{
							string txt = ZString.Format<string, string>("{0} {1}", formattedZoneName, "Ley Anchor");
							interactiveState.InjectTooltipText(txt);
						}
					}
				}
			}

			// Token: 0x04002A50 RID: 10832
			[SerializeField]
			private BaseState m_state;

			// Token: 0x04002A51 RID: 10833
			[SerializeField]
			private ZoneSettingsProfile m_profile;

			// Token: 0x04002A52 RID: 10834
			[SerializeField]
			private ZoneId m_zoneId;
		}
	}
}
