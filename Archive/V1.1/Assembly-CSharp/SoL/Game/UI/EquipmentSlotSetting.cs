using System;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.UI
{
	// Token: 0x0200087E RID: 2174
	[Serializable]
	public struct EquipmentSlotSetting
	{
		// Token: 0x04003D28 RID: 15656
		public EquipmentSlot Type;

		// Token: 0x04003D29 RID: 15657
		public EquipmentSlotUI UI;

		// Token: 0x04003D2A RID: 15658
		public Sprite Icon;

		// Token: 0x04003D2B RID: 15659
		public Vector3 Rotation;
	}
}
