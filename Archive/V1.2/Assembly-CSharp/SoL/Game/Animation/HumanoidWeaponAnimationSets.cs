using System;
using System.Collections;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Settings;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Animation
{
	// Token: 0x02000D7D RID: 3453
	[CreateAssetMenu(menuName = "SoL/Collections/Weapon Set Animations")]
	public class HumanoidWeaponAnimationSets : ScriptableObject
	{
		// Token: 0x060067FC RID: 26620 RVA: 0x00213928 File Offset: 0x00211B28
		public AnimancerAnimationSet GetAnimationSet(GameEntity entity)
		{
			HandheldFlagConfig handheldFlagConfig = entity.GetHandheldFlagConfig();
			return this.GetAnimationSet(handheldFlagConfig);
		}

		// Token: 0x060067FD RID: 26621 RVA: 0x00213944 File Offset: 0x00211B44
		public AnimancerAnimationSet GetAnimationSet(HandheldFlagConfig config)
		{
			for (int i = 0; i < this.m_weaponSetAnimations.Length; i++)
			{
				AnimancerAnimationSet result;
				if (this.m_weaponSetAnimations[i].TryGetAnimationSet(config, out result))
				{
					return result;
				}
			}
			return GlobalSettings.Values.Animation.FallbackCombatSet;
		}

		// Token: 0x04005A5C RID: 23132
		[SerializeField]
		private HumanoidWeaponAnimationSets.WeaponSetAnimation[] m_weaponSetAnimations;

		// Token: 0x02000D7E RID: 3454
		[Serializable]
		private class WeaponSetAnimation
		{
			// Token: 0x060067FF RID: 26623 RVA: 0x00213988 File Offset: 0x00211B88
			public bool TryGetAnimationSet(HandheldFlagConfig config, out AnimancerAnimationSet set)
			{
				set = null;
				if (this.m_handheldItemRequirement.MeetsRequirements(config))
				{
					set = ((config.AlternateAnimationSet && this.m_hasAlternate && this.m_alternateAnimationSet) ? this.m_alternateAnimationSet : this.m_animationSet);
				}
				return set != null;
			}

			// Token: 0x06006800 RID: 26624 RVA: 0x000636CE File Offset: 0x000618CE
			protected IEnumerable GetAnimationSets()
			{
				return SolOdinUtilities.GetDropdownItems<AnimancerAnimationSet>();
			}

			// Token: 0x04005A5D RID: 23133
			private const string kAlternateGroup = "Alternate";

			// Token: 0x04005A5E RID: 23134
			[SerializeField]
			private AnimancerAnimationSet m_animationSet;

			// Token: 0x04005A5F RID: 23135
			[SerializeField]
			private HandheldItemRequirement m_handheldItemRequirement;

			// Token: 0x04005A60 RID: 23136
			[SerializeField]
			private bool m_hasAlternate;

			// Token: 0x04005A61 RID: 23137
			[SerializeField]
			private AnimancerAnimationSet m_alternateAnimationSet;
		}
	}
}
