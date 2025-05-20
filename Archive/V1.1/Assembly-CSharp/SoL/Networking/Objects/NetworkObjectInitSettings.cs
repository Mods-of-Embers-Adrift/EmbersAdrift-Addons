using System;
using UnityEngine;

namespace SoL.Networking.Objects
{
	// Token: 0x020004B8 RID: 1208
	[Serializable]
	public struct NetworkObjectInitSettings
	{
		// Token: 0x04002624 RID: 9764
		public GameObject Object;

		// Token: 0x04002625 RID: 9765
		public NetworkInclusionFlags InclusionFlags;
	}
}
