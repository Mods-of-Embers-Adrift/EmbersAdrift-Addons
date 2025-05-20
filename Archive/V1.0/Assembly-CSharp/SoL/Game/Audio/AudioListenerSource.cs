using System;
using UnityEngine;

namespace SoL.Game.Audio
{
	// Token: 0x02000D07 RID: 3335
	public class AudioListenerSource : MonoBehaviour
	{
		// Token: 0x1700182E RID: 6190
		// (get) Token: 0x060064B8 RID: 25784 RVA: 0x00083C45 File Offset: 0x00081E45
		public bool IsFallback
		{
			get
			{
				return this.m_isFallback;
			}
		}

		// Token: 0x060064B9 RID: 25785 RVA: 0x00083C4D File Offset: 0x00081E4D
		private void Awake()
		{
			if (this.m_isFallback && this.m_audioListener)
			{
				this.m_audioListener.enabled = false;
			}
		}

		// Token: 0x060064BA RID: 25786 RVA: 0x00083C70 File Offset: 0x00081E70
		private void Start()
		{
			AudioListenerManager.RegisterAudioListener(this);
		}

		// Token: 0x060064BB RID: 25787 RVA: 0x00083C78 File Offset: 0x00081E78
		private void OnDestroy()
		{
			AudioListenerManager.UnregisterAudioListener(this);
		}

		// Token: 0x1700182F RID: 6191
		// (get) Token: 0x060064BC RID: 25788 RVA: 0x00083C80 File Offset: 0x00081E80
		// (set) Token: 0x060064BD RID: 25789 RVA: 0x00083C9C File Offset: 0x00081E9C
		public bool ListenerEnabled
		{
			get
			{
				return this.m_audioListener && this.m_audioListener.enabled;
			}
			set
			{
				if (this.m_audioListener)
				{
					this.m_audioListener.enabled = value;
				}
			}
		}

		// Token: 0x0400576F RID: 22383
		[SerializeField]
		private AudioListener m_audioListener;

		// Token: 0x04005770 RID: 22384
		[SerializeField]
		private bool m_isFallback;
	}
}
