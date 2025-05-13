using System;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x0200060E RID: 1550
	[Serializable]
	public class OverrideFloat
	{
		// Token: 0x06003149 RID: 12617 RVA: 0x00061FD7 File Offset: 0x000601D7
		public OverrideFloat(float defaultValue)
		{
			this.m_value = defaultValue;
		}

		// Token: 0x0600314A RID: 12618 RVA: 0x00061FE6 File Offset: 0x000601E6
		public float GetValue(float currentValue)
		{
			if (!this.m_override)
			{
				return currentValue;
			}
			return this.m_value;
		}

		// Token: 0x04002F9A RID: 12186
		[SerializeField]
		private bool m_override;

		// Token: 0x04002F9B RID: 12187
		[SerializeField]
		private float m_value;
	}
}
