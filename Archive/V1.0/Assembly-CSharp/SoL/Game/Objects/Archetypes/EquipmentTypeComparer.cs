using System;
using System.Collections.Generic;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A79 RID: 2681
	public struct EquipmentTypeComparer : IEqualityComparer<EquipmentType>
	{
		// Token: 0x060052E4 RID: 21220 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(EquipmentType x, EquipmentType y)
		{
			return x == y;
		}

		// Token: 0x060052E5 RID: 21221 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(EquipmentType obj)
		{
			return (int)obj;
		}
	}
}
