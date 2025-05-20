using System;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A2D RID: 2605
	public interface IArmorClass
	{
		// Token: 0x170011EB RID: 4587
		// (get) Token: 0x060050A0 RID: 20640
		EquipmentType Type { get; }

		// Token: 0x170011EC RID: 4588
		// (get) Token: 0x060050A1 RID: 20641
		int BaseArmorClass { get; }

		// Token: 0x170011ED RID: 4589
		// (get) Token: 0x060050A2 RID: 20642
		int MaxDamageAbsorption { get; }

		// Token: 0x170011EE RID: 4590
		// (get) Token: 0x060050A3 RID: 20643
		int ArmorCost { get; }

		// Token: 0x060050A4 RID: 20644
		int GetCurrentArmorClass(float damageAbsorbed);

		// Token: 0x060050A5 RID: 20645
		float GetCurrentDurability(float damageAbsorbed);
	}
}
