using System;
using UnityEngine;

namespace SoL.Game.Pooling
{
	// Token: 0x020007D2 RID: 2002
	public class MountPointGizmos : MonoBehaviour
	{
		// Token: 0x040038EB RID: 14571
		[SerializeField]
		private MountPointGizmos.OffHandType m_offHandType;

		// Token: 0x040038EC RID: 14572
		[SerializeField]
		private MountPointGizmos.MountPointType m_type;

		// Token: 0x020007D3 RID: 2003
		private enum MountPointType
		{
			// Token: 0x040038EE RID: 14574
			Attached,
			// Token: 0x040038EF RID: 14575
			Detached,
			// Token: 0x040038F0 RID: 14576
			IKTarget
		}

		// Token: 0x020007D4 RID: 2004
		private enum OffHandType
		{
			// Token: 0x040038F2 RID: 14578
			None,
			// Token: 0x040038F3 RID: 14579
			BigTwoHander,
			// Token: 0x040038F4 RID: 14580
			Halberd,
			// Token: 0x040038F5 RID: 14581
			Long
		}
	}
}
