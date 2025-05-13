using System;
using SoL.Utilities;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AAC RID: 2732
	public interface IUtilityItem
	{
		// Token: 0x17001365 RID: 4965
		// (get) Token: 0x0600546D RID: 21613
		BaseArchetype Archetype { get; }

		// Token: 0x0600546E RID: 21614
		CursorType GetCursorType();

		// Token: 0x0600546F RID: 21615
		void ClientRequestExecuteUtility(GameEntity entity, ArchetypeInstance sourceInstance, ArchetypeInstance targetInstance);

		// Token: 0x06005470 RID: 21616
		void ExecuteUtility(GameEntity entity, ArchetypeInstance sourceInstance, ArchetypeInstance targetInstance);

		// Token: 0x06005471 RID: 21617
		void AugmentUsed(GameEntity sourceEntity, ArchetypeInstance itemInstance, int amount);

		// Token: 0x06005472 RID: 21618
		void PlayAudioClip();

		// Token: 0x17001366 RID: 4966
		// (get) Token: 0x06005473 RID: 21619
		bool ResetCursorGameMode { get; }
	}
}
