using System;
using System.Collections.Generic;

namespace SoL.Game.Objects.Containers
{
	// Token: 0x02000A11 RID: 2577
	public struct ContainerTypeComparer : IEqualityComparer<ContainerType>
	{
		// Token: 0x06004EC4 RID: 20164 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(ContainerType x, ContainerType y)
		{
			return x == y;
		}

		// Token: 0x06004EC5 RID: 20165 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(ContainerType obj)
		{
			return (int)obj;
		}
	}
}
