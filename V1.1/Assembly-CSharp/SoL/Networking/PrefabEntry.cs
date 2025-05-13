using System;
using UnityEngine;

namespace SoL.Networking
{
	// Token: 0x020003C9 RID: 969
	[Serializable]
	public class PrefabEntry
	{
		// Token: 0x06001A01 RID: 6657 RVA: 0x000545BF File Offset: 0x000527BF
		public void Reset()
		{
			this.Name = null;
			this.Prefab = null;
			this.Id = UniqueId.Empty;
		}

		// Token: 0x17000629 RID: 1577
		// (get) Token: 0x06001A02 RID: 6658 RVA: 0x000545DA File Offset: 0x000527DA
		public bool IsEmpty
		{
			get
			{
				return this.Prefab == null;
			}
		}

		// Token: 0x04002125 RID: 8485
		public GameObject Prefab;

		// Token: 0x04002126 RID: 8486
		public string Name;

		// Token: 0x04002127 RID: 8487
		public UniqueId Id;
	}
}
