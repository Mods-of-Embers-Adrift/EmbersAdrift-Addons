using System;
using UnityEngine;

namespace SoL.Game.States
{
	// Token: 0x02000663 RID: 1635
	[Serializable]
	internal class StateSetting
	{
		// Token: 0x17000ADC RID: 2780
		// (get) Token: 0x060032EF RID: 13039 RVA: 0x000630F7 File Offset: 0x000612F7
		private int GetMaxValue
		{
			get
			{
				if (!this.m_state)
				{
					return 255;
				}
				return this.m_state.MaxState;
			}
		}

		// Token: 0x17000ADD RID: 2781
		// (get) Token: 0x060032F0 RID: 13040 RVA: 0x00063117 File Offset: 0x00061317
		public BaseState State
		{
			get
			{
				return this.m_state;
			}
		}

		// Token: 0x17000ADE RID: 2782
		// (get) Token: 0x060032F1 RID: 13041 RVA: 0x0006311F File Offset: 0x0006131F
		public byte Value
		{
			get
			{
				return (byte)this.m_value;
			}
		}

		// Token: 0x04003130 RID: 12592
		[SerializeField]
		private BaseState m_state;

		// Token: 0x04003131 RID: 12593
		[SerializeField]
		private int m_value;
	}
}
