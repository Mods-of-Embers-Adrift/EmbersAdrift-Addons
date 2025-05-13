using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SoL.Utilities
{
	// Token: 0x02000300 RID: 768
	public abstract class VolumeComponentModifier<T> : MonoBehaviour where T : VolumeComponent
	{
		// Token: 0x1700052A RID: 1322
		// (get) Token: 0x060015A1 RID: 5537 RVA: 0x000513CB File Offset: 0x0004F5CB
		protected T Component
		{
			get
			{
				return this.m_instance;
			}
		}

		// Token: 0x060015A2 RID: 5538 RVA: 0x000FD028 File Offset: 0x000FB228
		protected virtual void Awake()
		{
			if (this.m_volume == null || this.m_volume.sharedProfile == null)
			{
				base.enabled = false;
				return;
			}
			this.m_volume.profile.TryGet<T>(out this.m_instance);
		}

		// Token: 0x04001DA4 RID: 7588
		private T m_instance;

		// Token: 0x04001DA5 RID: 7589
		[SerializeField]
		private Volume m_volume;
	}
}
