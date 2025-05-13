using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Game.Discovery
{
	// Token: 0x02000CB0 RID: 3248
	public class Monolith : MonoBehaviour
	{
		// Token: 0x06006283 RID: 25219 RVA: 0x000824EB File Offset: 0x000806EB
		private void Start()
		{
			LocalZoneManager.RegisterMonolith(this);
		}

		// Token: 0x06006284 RID: 25220 RVA: 0x000824F3 File Offset: 0x000806F3
		private void OnDestroy()
		{
			LocalZoneManager.DeregisterMonolith(this);
		}
	}
}
