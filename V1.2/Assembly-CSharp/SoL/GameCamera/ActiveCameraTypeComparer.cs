using System;
using System.Collections.Generic;

namespace SoL.GameCamera
{
	// Token: 0x02000DE2 RID: 3554
	public struct ActiveCameraTypeComparer : IEqualityComparer<ActiveCameraTypes>
	{
		// Token: 0x060069EC RID: 27116 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(ActiveCameraTypes x, ActiveCameraTypes y)
		{
			return x == y;
		}

		// Token: 0x060069ED RID: 27117 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(ActiveCameraTypes obj)
		{
			return (int)obj;
		}
	}
}
