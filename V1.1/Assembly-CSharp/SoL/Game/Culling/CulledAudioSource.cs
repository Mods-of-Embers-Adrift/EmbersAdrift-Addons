using System;
using SoL.Managers;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Culling
{
	// Token: 0x02000CB4 RID: 3252
	public class CulledAudioSource : CulledObject
	{
		// Token: 0x060062A6 RID: 25254 RVA: 0x002054B8 File Offset: 0x002036B8
		private void Awake()
		{
			if (!this.m_source || GameManager.IsServer)
			{
				base.gameObject.SetActive(false);
				return;
			}
			if (this.m_cullingDistance != CullingDistance.NotCulled)
			{
				while (this.m_source.maxDistance > this.m_cullingDistance.GetDistance())
				{
					this.m_cullingDistance = this.m_cullingDistance.GetNextDistance();
				}
			}
			this.m_source.RefreshMixerGroup();
		}

		// Token: 0x060062A7 RID: 25255 RVA: 0x00205528 File Offset: 0x00203728
		protected override void RefreshCullee()
		{
			base.RefreshCullee();
			if (!this.m_source)
			{
				return;
			}
			if (this.IsCulled())
			{
				if (this.m_source.enabled)
				{
					this.m_source.enabled = false;
					return;
				}
			}
			else if (!this.m_source.enabled)
			{
				this.m_source.enabled = true;
			}
		}

		// Token: 0x04005609 RID: 22025
		[SerializeField]
		private AudioSource m_source;
	}
}
