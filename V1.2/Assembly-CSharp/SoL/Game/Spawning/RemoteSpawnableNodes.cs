using System;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x020006B8 RID: 1720
	[CreateAssetMenu(menuName = "SoL/Spawning/Remote Nodes")]
	public class RemoteSpawnableNodes : RemoteSpawnableBase
	{
		// Token: 0x17000B6B RID: 2923
		// (get) Token: 0x06003469 RID: 13417 RVA: 0x0004479C File Offset: 0x0004299C
		protected override RemoteSpawnProfile.RemoteSpawnProfileType Type
		{
			get
			{
				return RemoteSpawnProfile.RemoteSpawnProfileType.Node;
			}
		}
	}
}
