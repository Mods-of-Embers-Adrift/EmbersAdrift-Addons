using System;
using TMPro;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002A5 RID: 677
	public class NetworkStatsDisplay : MonoBehaviour
	{
		// Token: 0x17000506 RID: 1286
		// (get) Token: 0x06001444 RID: 5188 RVA: 0x0005032B File Offset: 0x0004E52B
		public TextMeshProUGUI Text
		{
			get
			{
				return this.m_text;
			}
		}

		// Token: 0x04001C95 RID: 7317
		[SerializeField]
		private TextMeshProUGUI m_text;
	}
}
