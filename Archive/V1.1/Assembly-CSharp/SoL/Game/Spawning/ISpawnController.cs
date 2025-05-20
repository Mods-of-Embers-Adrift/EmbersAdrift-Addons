using System;
using SoL.Game.NPCs.Interactions;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Spawning.Behavior;
using UnityEngine;

namespace SoL.Game.Spawning
{
	// Token: 0x0200067D RID: 1661
	public interface ISpawnController
	{
		// Token: 0x0600335A RID: 13146
		bool TryGetBehaviorProfile(out BehaviorProfile profile);

		// Token: 0x0600335B RID: 13147
		bool TryGetLevel(out int level);

		// Token: 0x17000B0B RID: 2827
		// (get) Token: 0x0600335C RID: 13148
		BehaviorSubTreeCollection BehaviorOverrides { get; }

		// Token: 0x17000B0C RID: 2828
		// (get) Token: 0x0600335D RID: 13149
		DialogueSource OverrideDialogue { get; }

		// Token: 0x0600335E RID: 13150
		int GetLevel();

		// Token: 0x17000B0D RID: 2829
		// (get) Token: 0x0600335F RID: 13151
		float? LeashDistance { get; }

		// Token: 0x17000B0E RID: 2830
		// (get) Token: 0x06003360 RID: 13152
		float? ResetDistance { get; }

		// Token: 0x17000B0F RID: 2831
		// (get) Token: 0x06003361 RID: 13153
		bool DespawnOnDeath { get; }

		// Token: 0x17000B10 RID: 2832
		// (get) Token: 0x06003362 RID: 13154
		bool CallForHelpRequiresLos { get; }

		// Token: 0x17000B11 RID: 2833
		// (get) Token: 0x06003363 RID: 13155
		bool ForceIndoorProfiles { get; }

		// Token: 0x17000B12 RID: 2834
		// (get) Token: 0x06003364 RID: 13156
		bool MatchAttackerLevel { get; }

		// Token: 0x17000B13 RID: 2835
		// (get) Token: 0x06003365 RID: 13157
		bool LogSpawns { get; }

		// Token: 0x06003366 RID: 13158
		bool OverrideInteractionFlags(out NpcInteractionFlags flags);

		// Token: 0x17000B14 RID: 2836
		// (get) Token: 0x06003367 RID: 13159
		MinMaxIntRange LevelRange { get; }

		// Token: 0x17000B15 RID: 2837
		// (get) Token: 0x06003368 RID: 13160
		Vector3? CurrentPosition { get; }

		// Token: 0x06003369 RID: 13161
		void NotifyOfDeath(GameEntity entity);

		// Token: 0x17000B16 RID: 2838
		// (get) Token: 0x0600336A RID: 13162
		int XpAdjustment { get; }

		// Token: 0x0600336B RID: 13163
		bool TryGetOverrideData(SpawnProfile spawnProfile, out SpawnControllerOverrideData data);
	}
}
