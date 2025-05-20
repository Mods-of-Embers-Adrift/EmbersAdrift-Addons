using System;
using SoL.Game.Objects;
using UnityEngine;

namespace SoL.Game.NPCs.Interactions
{
	// Token: 0x0200083A RID: 2106
	[Serializable]
	public class NpcInteractiveWaitParameters
	{
		// Token: 0x17000E06 RID: 3590
		// (get) Token: 0x06003CEA RID: 15594 RVA: 0x0006949A File Offset: 0x0006769A
		public bool RandomWait
		{
			get
			{
				return this.m_randomWait;
			}
		}

		// Token: 0x17000E07 RID: 3591
		// (get) Token: 0x06003CEB RID: 15595 RVA: 0x000694A2 File Offset: 0x000676A2
		public float WaitTime
		{
			get
			{
				return this.m_waitTime;
			}
		}

		// Token: 0x17000E08 RID: 3592
		// (get) Token: 0x06003CEC RID: 15596 RVA: 0x000694AA File Offset: 0x000676AA
		public MinMaxFloatRange RandomWaitTime
		{
			get
			{
				return this.m_randomWaitTime;
			}
		}

		// Token: 0x04003BB6 RID: 15286
		[SerializeField]
		private bool m_randomWait;

		// Token: 0x04003BB7 RID: 15287
		[SerializeField]
		private float m_waitTime = 2f;

		// Token: 0x04003BB8 RID: 15288
		[SerializeField]
		private MinMaxFloatRange m_randomWaitTime = new MinMaxFloatRange(2f, 5f);
	}
}
