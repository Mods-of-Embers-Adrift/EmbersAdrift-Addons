using System;
using UnityEngine;

namespace SoL.Game.Audio.Ambient
{
	// Token: 0x02000D28 RID: 3368
	public class AmbientAudioInjectorManual : MonoBehaviour, IAmbientAudioZone
	{
		// Token: 0x17001850 RID: 6224
		// (get) Token: 0x0600655B RID: 25947 RVA: 0x000843F2 File Offset: 0x000825F2
		public AmbientAudioZoneProfile Profile
		{
			get
			{
				return this.m_profile;
			}
		}

		// Token: 0x17001851 RID: 6225
		// (get) Token: 0x0600655C RID: 25948 RVA: 0x00082BC2 File Offset: 0x00080DC2
		public int Key
		{
			get
			{
				return this.GetHashCode();
			}
		}

		// Token: 0x0600655D RID: 25949 RVA: 0x000843FA File Offset: 0x000825FA
		private void Start()
		{
			if (this.m_controller != null)
			{
				this.m_controller.EnterZone(this);
			}
		}

		// Token: 0x0600655E RID: 25950 RVA: 0x00084416 File Offset: 0x00082616
		private void OnDestroy()
		{
			if (this.m_controller != null)
			{
				this.m_controller.ExitZone(this);
			}
		}

		// Token: 0x0400581E RID: 22558
		[SerializeField]
		private AmbientAudioController m_controller;

		// Token: 0x0400581F RID: 22559
		[SerializeField]
		private AmbientAudioZoneProfile m_profile;
	}
}
