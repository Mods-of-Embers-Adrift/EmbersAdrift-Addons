using System;
using SoL.Game.EffectSystem;
using SoL.Networking.Database;
using UMA.CharacterSystem;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A2B RID: 2603
	public interface IEquipable
	{
		// Token: 0x170011E7 RID: 4583
		// (get) Token: 0x06005094 RID: 20628
		EquipmentType Type { get; }

		// Token: 0x06005095 RID: 20629
		void OnEquipVisuals(CharacterSex sex, DynamicCharacterAvatar dca, int index, byte? visualIndex, byte? colorIndex, bool refresh = true);

		// Token: 0x06005096 RID: 20630
		void OnUnequipVisuals(CharacterSex sex, DynamicCharacterAvatar dca, int index, byte? visualIndex, byte? colorIndex, bool refresh = true);

		// Token: 0x06005097 RID: 20631
		void OnEquip();

		// Token: 0x06005098 RID: 20632
		void OnUnequip();

		// Token: 0x06005099 RID: 20633
		EquipmentSlot GetTargetEquipmentSlot(GameEntity entity);

		// Token: 0x170011E8 RID: 4584
		// (get) Token: 0x0600509A RID: 20634
		StatModifier[] StatModifiers { get; }

		// Token: 0x0600509B RID: 20635
		bool CanEquip(GameEntity entity);

		// Token: 0x0600509C RID: 20636
		bool MeetsRoleRequirements(GameEntity entity);

		// Token: 0x0600509D RID: 20637
		bool HasRequiredTrade(GameEntity entity);

		// Token: 0x170011E9 RID: 4585
		// (get) Token: 0x0600509E RID: 20638
		SetBonusProfile SetBonus { get; }
	}
}
