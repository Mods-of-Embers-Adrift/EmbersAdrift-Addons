using System;
using SoL.Game;
using SoL.Networking.REST;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000DC7 RID: 3527
	public class ZoneServerNotifier : MonoBehaviour
	{
		// Token: 0x06006946 RID: 26950 RVA: 0x0008695B File Offset: 0x00084B5B
		private void Notify()
		{
			ServerCommunicator.GET(this.m_zone, "status", null);
		}

		// Token: 0x04005BA3 RID: 23459
		[SerializeField]
		private ZoneId m_zone;
	}
}
