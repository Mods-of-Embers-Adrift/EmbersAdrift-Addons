using System;
using System.Collections;
using SoL.Game.EffectSystem;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000AE7 RID: 2791
	[Serializable]
	public class ReagentData
	{
		// Token: 0x170013F9 RID: 5113
		// (get) Token: 0x06005607 RID: 22023 RVA: 0x000795FB File Offset: 0x000777FB
		private bool m_isValidReagent
		{
			get
			{
				return this.m_type > ReagentType.None;
			}
		}

		// Token: 0x170013FA RID: 5114
		// (get) Token: 0x06005608 RID: 22024 RVA: 0x00079606 File Offset: 0x00077806
		private bool m_hasCombatEffect
		{
			get
			{
				return this.m_isValidReagent && this.m_providesCombatEffect;
			}
		}

		// Token: 0x170013FB RID: 5115
		// (get) Token: 0x06005609 RID: 22025 RVA: 0x00079618 File Offset: 0x00077818
		private IEnumerable GetReagentDropdownItems
		{
			get
			{
				return ReagentTypeExtensions.GetReagentDropdownItems();
			}
		}

		// Token: 0x170013FC RID: 5116
		// (get) Token: 0x0600560A RID: 22026 RVA: 0x0007961F File Offset: 0x0007781F
		public ReagentType Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x170013FD RID: 5117
		// (get) Token: 0x0600560B RID: 22027 RVA: 0x00079627 File Offset: 0x00077827
		public LevelRequirement LevelRequirement
		{
			get
			{
				return this.m_levelRequirement;
			}
		}

		// Token: 0x0600560C RID: 22028 RVA: 0x0007962F File Offset: 0x0007782F
		public bool TryGetCombatEffect(out CombatEffect effect)
		{
			effect = ((this.m_isValidReagent && this.m_providesCombatEffect) ? this.m_effect : null);
			return effect != null;
		}

		// Token: 0x0600560D RID: 22029 RVA: 0x00079651 File Offset: 0x00077851
		public bool TryGetTargetingParams(out TargetingParams targetingParams)
		{
			targetingParams = ((this.m_isValidReagent && this.m_overrideTargetingParams) ? this.m_targetingParams : null);
			return targetingParams != null;
		}

		// Token: 0x170013FE RID: 5118
		// (get) Token: 0x0600560E RID: 22030 RVA: 0x00079673 File Offset: 0x00077873
		private IEnumerable GetAbilities
		{
			get
			{
				return SolOdinUtilities.GetDropdownItems<AppliableEffectAbility>();
			}
		}

		// Token: 0x04004C4F RID: 19535
		private const string kGroupName = "Reagent Settings";

		// Token: 0x04004C50 RID: 19536
		[SerializeField]
		private ReagentType m_type;

		// Token: 0x04004C51 RID: 19537
		[SerializeField]
		private LevelRequirement m_levelRequirement;

		// Token: 0x04004C52 RID: 19538
		[SerializeField]
		private bool m_providesCombatEffect;

		// Token: 0x04004C53 RID: 19539
		[SerializeField]
		private CombatEffect m_effect;

		// Token: 0x04004C54 RID: 19540
		[SerializeField]
		private bool m_overrideTargetingParams;

		// Token: 0x04004C55 RID: 19541
		[SerializeField]
		private TargetingParamsSpatial m_targetingParams;

		// Token: 0x04004C56 RID: 19542
		private const string kExtractionGroup = "Reagent Settings/Extraction";

		// Token: 0x04004C57 RID: 19543
		[SerializeField]
		private AppliableEffectAbility m_abilityToExtractFrom;

		// Token: 0x04004C58 RID: 19544
		[Range(1f, 50f)]
		[SerializeField]
		private int m_extractionLevel = 1;
	}
}
