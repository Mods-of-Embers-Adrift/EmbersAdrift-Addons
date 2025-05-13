using System;
using UnityEngine;

namespace SoL.Game.States
{
	// Token: 0x02000661 RID: 1633
	public interface IState
	{
		// Token: 0x17000AD9 RID: 2777
		// (get) Token: 0x060032D7 RID: 13015
		GameObject gameObject { get; }

		// Token: 0x17000ADA RID: 2778
		// (get) Token: 0x060032D8 RID: 13016
		// (set) Token: 0x060032D9 RID: 13017
		int Key { get; set; }

		// Token: 0x060032DA RID: 13018
		byte GetState();

		// Token: 0x060032DB RID: 13019
		void SetState(byte value);
	}
}
