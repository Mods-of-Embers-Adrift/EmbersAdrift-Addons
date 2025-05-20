using System;
using UnityEngine;

namespace SoL.Game
{
	// Token: 0x0200060C RID: 1548
	[Serializable]
	public class OverrideBase<T>
	{
		// Token: 0x06003146 RID: 12614 RVA: 0x00061FAD File Offset: 0x000601AD
		public OverrideBase(T defaultValue)
		{
			this.m_value = defaultValue;
		}

		// Token: 0x06003147 RID: 12615 RVA: 0x00061FBC File Offset: 0x000601BC
		public T GetValue(T currentValue)
		{
			if (!this.m_override)
			{
				return currentValue;
			}
			return this.m_value;
		}

		// Token: 0x04002F98 RID: 12184
		[SerializeField]
		private bool m_override;

		// Token: 0x04002F99 RID: 12185
		[SerializeField]
		private T m_value;
	}
}
