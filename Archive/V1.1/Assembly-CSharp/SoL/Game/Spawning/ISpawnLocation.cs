using System;
using UnityEngine;
using UnityEngine.AI;

namespace SoL.Game.Spawning
{
	// Token: 0x02000680 RID: 1664
	public interface ISpawnLocation
	{
		// Token: 0x17000B19 RID: 2841
		// (get) Token: 0x06003372 RID: 13170
		// (set) Token: 0x06003373 RID: 13171
		bool Occupied { get; set; }

		// Token: 0x06003374 RID: 13172
		void SetPosition(NavMeshHit hit);

		// Token: 0x06003375 RID: 13173
		Vector3 GetPosition();

		// Token: 0x06003376 RID: 13174
		Quaternion GetRotation();
	}
}
