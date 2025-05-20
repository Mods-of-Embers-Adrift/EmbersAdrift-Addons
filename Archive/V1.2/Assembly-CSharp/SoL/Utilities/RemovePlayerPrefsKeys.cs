using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002B7 RID: 695
	public class RemovePlayerPrefsKeys : MonoBehaviour
	{
		// Token: 0x060014A5 RID: 5285 RVA: 0x000FB428 File Offset: 0x000F9628
		private void Awake()
		{
			if (this.m_keysToRemove == null)
			{
				return;
			}
			for (int i = 0; i < this.m_keysToRemove.Count; i++)
			{
				PlayerPrefs.DeleteKey(this.m_keysToRemove[i]);
			}
		}

		// Token: 0x04001CE2 RID: 7394
		[SerializeField]
		private List<string> m_keysToRemove;
	}
}
