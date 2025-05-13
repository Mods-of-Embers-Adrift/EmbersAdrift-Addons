using System;
using UnityEngine;

namespace SoL.Utilities
{
	// Token: 0x02000275 RID: 629
	public class DisableOnAwake : MonoBehaviour
	{
		// Token: 0x060013C0 RID: 5056 RVA: 0x0004FE03 File Offset: 0x0004E003
		private void Awake()
		{
			base.gameObject.SetActive(false);
		}
	}
}
