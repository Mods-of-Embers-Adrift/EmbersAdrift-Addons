using System;
using UnityEngine;

namespace SoL.Game.Settings
{
	// Token: 0x02000721 RID: 1825
	[Serializable]
	public class HealthThreatSettings
	{
		// Token: 0x17000C3B RID: 3131
		// (get) Token: 0x060036E1 RID: 14049 RVA: 0x000658D6 File Offset: 0x00063AD6
		public bool RequireLineOfSight
		{
			get
			{
				return this.m_requireLineOfSight;
			}
		}

		// Token: 0x17000C3C RID: 3132
		// (get) Token: 0x060036E2 RID: 14050 RVA: 0x000658DE File Offset: 0x00063ADE
		public float UnknownMultiplier
		{
			get
			{
				return this.m_unknownMultiplier;
			}
		}

		// Token: 0x17000C3D RID: 3133
		// (get) Token: 0x060036E3 RID: 14051 RVA: 0x000658E6 File Offset: 0x00063AE6
		public float KnownMultiplier
		{
			get
			{
				return this.m_knownMultiplier;
			}
		}

		// Token: 0x17000C3E RID: 3134
		// (get) Token: 0x060036E4 RID: 14052 RVA: 0x000658EE File Offset: 0x00063AEE
		public float HostileMultiplier
		{
			get
			{
				return this.m_hostileMultiplier;
			}
		}

		// Token: 0x17000C3F RID: 3135
		// (get) Token: 0x060036E5 RID: 14053 RVA: 0x000658F6 File Offset: 0x00063AF6
		public float HotMultiplier
		{
			get
			{
				return this.m_hotMultiplier;
			}
		}

		// Token: 0x17000C40 RID: 3136
		// (get) Token: 0x060036E6 RID: 14054 RVA: 0x000658FE File Offset: 0x00063AFE
		public float OverallMultiplier
		{
			get
			{
				return this.m_overallMultiplier;
			}
		}

		// Token: 0x04003516 RID: 13590
		[SerializeField]
		private bool m_requireLineOfSight;

		// Token: 0x04003517 RID: 13591
		[SerializeField]
		private float m_unknownMultiplier = 0.1f;

		// Token: 0x04003518 RID: 13592
		[SerializeField]
		private float m_knownMultiplier = 1f;

		// Token: 0x04003519 RID: 13593
		[SerializeField]
		private float m_hostileMultiplier = 1.2f;

		// Token: 0x0400351A RID: 13594
		[SerializeField]
		private float m_hotMultiplier = 0.5f;

		// Token: 0x0400351B RID: 13595
		[SerializeField]
		private float m_overallMultiplier = 1f;
	}
}
