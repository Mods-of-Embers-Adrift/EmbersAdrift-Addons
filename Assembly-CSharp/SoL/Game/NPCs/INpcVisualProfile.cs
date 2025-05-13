using System;
using SoL.Networking.Database;
using UMA.CharacterSystem;

namespace SoL.Game.NPCs
{
	// Token: 0x020007FE RID: 2046
	public interface INpcVisualProfile
	{
		// Token: 0x17000D94 RID: 3476
		// (get) Token: 0x06003B52 RID: 15186
		WardrobeRecipePairEnsemble Ensemble { get; }

		// Token: 0x06003B53 RID: 15187
		bool TryGetSex(out CharacterSex sex);

		// Token: 0x06003B54 RID: 15188
		CharacterBuildType GetBuildType(Random seed);

		// Token: 0x06003B55 RID: 15189
		void SetDna(DynamicCharacterAvatar dca, Random seed, GameEntity entity);

		// Token: 0x06003B56 RID: 15190
		void SetColors(DynamicCharacterAvatar dca, Random seed);

		// Token: 0x06003B57 RID: 15191
		void SetCustomizations(DynamicCharacterAvatar dca, Random seed, CharacterSex sex);

		// Token: 0x06003B58 RID: 15192
		void LoadEquipmentData(DynamicCharacterAvatar dca);
	}
}
