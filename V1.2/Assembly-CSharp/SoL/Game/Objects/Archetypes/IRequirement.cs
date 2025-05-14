using System;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A94 RID: 2708
	public interface IRequirement
	{
		// Token: 0x060053F8 RID: 21496
		bool MeetsAllRequirements(GameEntity entity);
	}
}
