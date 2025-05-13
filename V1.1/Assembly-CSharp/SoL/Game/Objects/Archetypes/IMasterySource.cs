using System;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A24 RID: 2596
	public interface IMasterySource
	{
		// Token: 0x0600503D RID: 20541
		bool TryGetMastery(GameEntity entity, out MasteryArchetype mastery);

		// Token: 0x170011C7 RID: 4551
		// (get) Token: 0x0600503E RID: 20542
		float CreditFactor { get; }

		// Token: 0x170011C8 RID: 4552
		// (get) Token: 0x0600503F RID: 20543
		bool AddGroupBonus { get; }
	}
}
