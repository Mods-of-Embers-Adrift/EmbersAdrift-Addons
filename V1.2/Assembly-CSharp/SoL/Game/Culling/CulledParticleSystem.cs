using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Culling
{
	// Token: 0x02000CBC RID: 3260
	public class CulledParticleSystem : CulledObject
	{
		// Token: 0x17001798 RID: 6040
		// (get) Token: 0x060062C7 RID: 25287 RVA: 0x000828DA File Offset: 0x00080ADA
		// (set) Token: 0x060062C8 RID: 25288 RVA: 0x000828E2 File Offset: 0x00080AE2
		public bool ExternallyStopped
		{
			get
			{
				return this.m_externallyStopped;
			}
			set
			{
				this.m_externallyStopped = value;
				this.RefreshCullee();
			}
		}

		// Token: 0x060062C9 RID: 25289 RVA: 0x000828F1 File Offset: 0x00080AF1
		private void Awake()
		{
			if (!this.m_particleSystem || GameManager.IsServer)
			{
				base.gameObject.SetActive(false);
				return;
			}
		}

		// Token: 0x060062CA RID: 25290 RVA: 0x00205A9C File Offset: 0x00203C9C
		[ContextMenu("Refresh Cullee")]
		protected override void RefreshCullee()
		{
			base.RefreshCullee();
			if (!this.m_particleSystem)
			{
				return;
			}
			if (this.IsCulled() || this.ExternallyStopped)
			{
				this.m_particleSystem.Stop(true);
				return;
			}
			if (!this.m_particleSystem.isPlaying)
			{
				this.m_particleSystem.Play(true);
			}
		}

		// Token: 0x17001799 RID: 6041
		// (get) Token: 0x060062CB RID: 25291 RVA: 0x00082914 File Offset: 0x00080B14
		private bool m_showGetSystem
		{
			get
			{
				return this.m_particleSystem == null;
			}
		}

		// Token: 0x060062CC RID: 25292 RVA: 0x00082922 File Offset: 0x00080B22
		private void GetSystem()
		{
			if (this.m_particleSystem == null)
			{
				this.m_particleSystem = base.gameObject.GetComponent<ParticleSystem>();
			}
		}

		// Token: 0x04005621 RID: 22049
		[SerializeField]
		private ParticleSystem m_particleSystem;

		// Token: 0x04005622 RID: 22050
		private bool m_externallyStopped;
	}
}
