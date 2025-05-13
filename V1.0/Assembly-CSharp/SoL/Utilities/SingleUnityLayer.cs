using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002DE RID: 734
	[Serializable]
	public class SingleUnityLayer
	{
		// Token: 0x17000520 RID: 1312
		// (get) Token: 0x06001527 RID: 5415 RVA: 0x00050D75 File Offset: 0x0004EF75
		public int LayerIndex
		{
			get
			{
				return this.m_layerIndex;
			}
		}

		// Token: 0x17000521 RID: 1313
		// (get) Token: 0x06001528 RID: 5416 RVA: 0x00050D7D File Offset: 0x0004EF7D
		public int Mask
		{
			get
			{
				return 1 << this.m_layerIndex;
			}
		}

		// Token: 0x06001529 RID: 5417 RVA: 0x00050D8A File Offset: 0x0004EF8A
		public void Set(int _layerIndex)
		{
			if (_layerIndex > 0 && _layerIndex < 32)
			{
				this.m_layerIndex = _layerIndex;
			}
		}

		// Token: 0x04001D62 RID: 7522
		[SerializeField]
		private int m_layerIndex;
	}
}
