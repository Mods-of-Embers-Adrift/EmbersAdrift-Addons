using System;
using SoL.Managers;
using UnityEngine;

namespace SoL.Tests
{
	// Token: 0x02000DB5 RID: 3509
	public class SampleClosestPlayerSpawn : MonoBehaviour
	{
		// Token: 0x060068FF RID: 26879 RVA: 0x00086719 File Offset: 0x00084919
		private void Cache()
		{
			LocalZoneManager.CacheClosestPlayerSpawn(base.gameObject.transform.position);
		}

		// Token: 0x06006900 RID: 26880 RVA: 0x00086730 File Offset: 0x00084930
		private void Sample()
		{
			LocalZoneManager.GetClosestPlayerSpawn(base.gameObject.transform.position);
		}
	}
}
