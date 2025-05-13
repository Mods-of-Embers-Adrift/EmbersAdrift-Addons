using System;
using SoL.Game.Audio;
using SoL.Game.NPCs;
using SoL.Game.Objects;
using SoL.Managers;
using SoL.Networking.Database;
using UMA.CharacterSystem;

namespace SoL.Game.Randomization
{
	// Token: 0x02000773 RID: 1907
	public class NetworkedRandomizerUMA : NetworkedRandomizer
	{
		// Token: 0x06003858 RID: 14424 RVA: 0x0016D5C8 File Offset: 0x0016B7C8
		protected override void Randomize(int seed)
		{
			this.m_dca = base.GameEntity.DCAController.DCA;
			NpcInitData npcInitData = base.GameEntity.CharacterData.NpcInitData;
			VisualProfileType profileType = npcInitData.ProfileType;
			NpcProfile profile2;
			if (profileType != VisualProfileType.Individual)
			{
				NpcPopulationVisualsProfile profile;
				if (profileType != VisualProfileType.Population)
				{
					base.Randomize(seed);
					UMAManager.BuildRandomNpc(base.GameEntity, this.m_dca, this.m_random, npcInitData.EnsembleId, null);
				}
				else if (InternalGameDatabase.Archetypes.TryGetAsType<NpcPopulationVisualsProfile>(npcInitData.ProfileId, out profile))
				{
					base.Randomize(seed);
					UMAManager.BuildNpcFromPopulationVisualsProfile(base.GameEntity, this.m_dca, profile, npcInitData.EnsembleId, this.m_random);
				}
			}
			else if (InternalGameDatabase.Archetypes.TryGetAsType<NpcProfile>(npcInitData.ProfileId, out profile2))
			{
				base.Randomize(seed);
				UMAManager.BuildNpcFromProfile(base.GameEntity, this.m_dca, profile2, this.m_random);
			}
			if (base.GameEntity.CharacterData.Sex == CharacterSex.Female)
			{
				EntityAudioController componentInChildren = base.gameObject.GetComponentInChildren<EntityAudioController>();
				if (componentInChildren != null)
				{
					componentInChildren.LoadFemaleOverrides = true;
				}
			}
		}

		// Token: 0x04003736 RID: 14134
		private DynamicCharacterAvatar m_dca;
	}
}
