using System;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x020006D7 RID: 1751
	[Serializable]
	public class NavAreaSelector
	{
		// Token: 0x17000BAC RID: 2988
		// (get) Token: 0x06003514 RID: 13588 RVA: 0x000645A8 File Offset: 0x000627A8
		public int AreaMask
		{
			get
			{
				return this.m_areaMask;
			}
		}

		// Token: 0x0400334A RID: 13130
		[SerializeField]
		private int m_areaMask = -1;
	}
}
