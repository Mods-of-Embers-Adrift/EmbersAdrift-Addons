using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x020002C7 RID: 711
	public class ServerDisabler : MonoBehaviour
	{
		// Token: 0x060014CC RID: 5324 RVA: 0x0005081B File Offset: 0x0004EA1B
		private void Awake()
		{
			if (GameManager.IsServer && GameManager.IsOnline)
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
