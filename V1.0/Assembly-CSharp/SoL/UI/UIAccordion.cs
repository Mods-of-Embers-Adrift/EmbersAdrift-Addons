using System;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.UI
{
	// Token: 0x02000390 RID: 912
	public class UIAccordion : ToggleGroup
	{
		// Token: 0x170005FA RID: 1530
		// (get) Token: 0x060018F7 RID: 6391 RVA: 0x000537AD File Offset: 0x000519AD
		public float Duration
		{
			get
			{
				return this.m_duration;
			}
		}

		// Token: 0x04002007 RID: 8199
		[SerializeField]
		private float m_duration = 0.3f;
	}
}
