using System;
using SoL.Game.Interactives;
using SoL.Game.NPCs;
using SoL.Game.Spawning;
using UnityEngine;

namespace SoL.Game.Crafting
{
	// Token: 0x02000CEF RID: 3311
	[CreateAssetMenu(menuName = "SoL/Profiles/Resource Node")]
	public class ResourceSpawnProfile : SpawnProfile
	{
		// Token: 0x06006459 RID: 25689 RVA: 0x00209C00 File Offset: 0x00207E00
		protected override void SpawnInternal(ISpawnController controller, GameEntity gameEntity)
		{
			base.SpawnInternal(controller, gameEntity);
			SpawnTier spawnTier = this.m_scaleBySpawnTier ? SpawnProfile.GetSpawnTier(this.m_spawnTierFlags) : SpawnTier.Normal;
			InteractiveGatheringNode interactiveGatheringNode;
			if (gameEntity.Interactive != null && gameEntity.Interactive.TryGetAsType(out interactiveGatheringNode))
			{
				interactiveGatheringNode.SpawnTier = spawnTier;
				InteractiveGatheringNode interactiveGatheringNode2 = interactiveGatheringNode;
				LootProfileProbabilityCollection lootProfiles = this.m_lootProfiles;
				LootProfile lootProfile;
				if (lootProfiles == null)
				{
					lootProfile = null;
				}
				else
				{
					LootProfileProbabilityEntry entry = lootProfiles.GetEntry(null, false);
					lootProfile = ((entry != null) ? entry.Obj : null);
				}
				interactiveGatheringNode2.LootProfile = lootProfile;
			}
		}

		// Token: 0x0400571C RID: 22300
		[SerializeField]
		private bool m_scaleBySpawnTier;

		// Token: 0x0400571D RID: 22301
		[SerializeField]
		private SpawnTierFlags m_spawnTierFlags;

		// Token: 0x0400571E RID: 22302
		[SerializeField]
		private LootProfileProbabilityCollection m_lootProfiles;
	}
}
