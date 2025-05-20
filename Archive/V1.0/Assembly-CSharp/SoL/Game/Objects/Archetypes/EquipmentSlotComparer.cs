using System;
using System.Collections.Generic;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A78 RID: 2680
	public struct EquipmentSlotComparer : IEqualityComparer<EquipmentSlot>
	{
		// Token: 0x060052E2 RID: 21218 RVA: 0x0004FB72 File Offset: 0x0004DD72
		public bool Equals(EquipmentSlot x, EquipmentSlot y)
		{
			return x == y;
		}

		// Token: 0x060052E3 RID: 21219 RVA: 0x00049A92 File Offset: 0x00047C92
		public int GetHashCode(EquipmentSlot obj)
		{
			return (int)obj;
		}
	}
}
