using System;
using System.Collections;
using Cysharp.Text;
using SoL.Game.EffectSystem;
using SoL.Managers;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes.Abilities
{
	// Token: 0x02000AF4 RID: 2804
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Skills/Abilities/Reagent")]
	public class ReagentAbility : AppliableEffectAbility
	{
		// Token: 0x1700144A RID: 5194
		// (get) Token: 0x060056CA RID: 22218 RVA: 0x00079C58 File Offset: 0x00077E58
		public ReagentType Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x1700144B RID: 5195
		// (get) Token: 0x060056CB RID: 22219 RVA: 0x00079618 File Offset: 0x00077818
		private IEnumerable GetReagentDropdownItems
		{
			get
			{
				return ReagentTypeExtensions.GetReagentDropdownItems();
			}
		}

		// Token: 0x1700144C RID: 5196
		// (get) Token: 0x060056CC RID: 22220 RVA: 0x00079C60 File Offset: 0x00077E60
		public bool HasInstant
		{
			get
			{
				TieredAbilityParameters baseParams = this.m_baseParams;
				bool flag;
				if (baseParams == null)
				{
					flag = (null != null);
				}
				else
				{
					CombatEffect combatEffect = baseParams.CombatEffect;
					flag = (((combatEffect != null) ? combatEffect.Effects : null) != null);
				}
				return flag && this.m_baseParams.CombatEffect.Effects.HasInstantVitals;
			}
		}

		// Token: 0x1700144D RID: 5197
		// (get) Token: 0x060056CD RID: 22221 RVA: 0x00079C99 File Offset: 0x00077E99
		public bool HasDuration
		{
			get
			{
				TieredAbilityParameters baseParams = this.m_baseParams;
				bool flag;
				if (baseParams == null)
				{
					flag = (null != null);
				}
				else
				{
					CombatEffect combatEffect = baseParams.CombatEffect;
					flag = (((combatEffect != null) ? combatEffect.Effects : null) != null);
				}
				return flag && this.m_baseParams.CombatEffect.Effects.HasLasting;
			}
		}

		// Token: 0x1700144E RID: 5198
		// (get) Token: 0x060056CE RID: 22222 RVA: 0x00079CD2 File Offset: 0x00077ED2
		public bool HasOverTime
		{
			get
			{
				TieredAbilityParameters baseParams = this.m_baseParams;
				bool flag;
				if (baseParams == null)
				{
					flag = (null != null);
				}
				else
				{
					CombatEffect combatEffect = baseParams.CombatEffect;
					flag = (((combatEffect != null) ? combatEffect.Effects : null) != null);
				}
				return flag && this.m_baseParams.CombatEffect.Effects.HasOverTimeVitals;
			}
		}

		// Token: 0x1700144F RID: 5199
		// (get) Token: 0x060056CF RID: 22223 RVA: 0x00079D0B File Offset: 0x00077F0B
		public bool HasStatusEffects
		{
			get
			{
				TieredAbilityParameters baseParams = this.m_baseParams;
				bool flag;
				if (baseParams == null)
				{
					flag = (null != null);
				}
				else
				{
					CombatEffect combatEffect = baseParams.CombatEffect;
					flag = (((combatEffect != null) ? combatEffect.Effects : null) != null);
				}
				return flag && this.m_baseParams.CombatEffect.Effects.HasStatusEffects;
			}
		}

		// Token: 0x17001450 RID: 5200
		// (get) Token: 0x060056D0 RID: 22224 RVA: 0x00079D44 File Offset: 0x00077F44
		public bool HasAoe
		{
			get
			{
				TieredAbilityParameters baseParams = this.m_baseParams;
				return ((baseParams != null) ? baseParams.TargetingParams : null) != null && this.m_baseParams.TargetingParams.TargetType.IsAOE();
			}
		}

		// Token: 0x17001451 RID: 5201
		// (get) Token: 0x060056D1 RID: 22225 RVA: 0x00079D71 File Offset: 0x00077F71
		public bool IsTriggerBased
		{
			get
			{
				TieredAbilityParameters baseParams = this.m_baseParams;
				return ((baseParams != null) ? baseParams.CombatEffect : null) != null && this.m_baseParams.CombatEffect.IsTriggerBased;
			}
		}

		// Token: 0x060056D2 RID: 22226 RVA: 0x001E13F8 File Offset: 0x001DF5F8
		protected override bool ExecutionCheck(ExecutionCache executionCache, float executionProgress)
		{
			if (((executionCache != null && executionCache.IsInstant) || (GameManager.IsServer && executionProgress >= 1f) || (!GameManager.IsServer && executionProgress <= 0f)) && this.m_type != ReagentType.None && executionCache != null && executionCache.SourceEntity != null)
			{
				ArchetypeInstance reagentForEntity = this.m_type.GetReagentForEntity(executionCache.SourceEntity, executionCache.MasteryLevel);
				ReagentItem reagentItem;
				if (reagentForEntity != null && reagentForEntity.Archetype != null && reagentForEntity.Archetype.TryGetAsType(out reagentItem) && reagentForEntity.ItemData.Count != null && reagentForEntity.ItemData.Count.Value > 0)
				{
					executionCache.ReagentItem = reagentItem;
					executionCache.AddReductionTask(ReductionTaskType.Count, reagentForEntity, 1);
				}
			}
			return base.ExecutionCheck(executionCache, executionProgress);
		}

		// Token: 0x060056D3 RID: 22227 RVA: 0x001E14DC File Offset: 0x001DF6DC
		protected override void FillTooltipTieredParams(GameEntity entity, float level)
		{
			base.FillTooltipTieredParams(entity, level);
			ArchetypeInstance reagentForEntity = this.m_type.GetReagentForEntity(entity, level);
			ReagentItem tt_reagentItem;
			if (reagentForEntity != null && reagentForEntity.Archetype != null && reagentForEntity.Archetype.TryGetAsType(out tt_reagentItem))
			{
				AbilityArchetype.tt_reagentItem = tt_reagentItem;
			}
		}

		// Token: 0x060056D4 RID: 22228 RVA: 0x001E1528 File Offset: 0x001DF728
		protected override void FillTooltipBlocksInternal(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity, int masteryLevel, int abilityLevel)
		{
			base.FillTooltipBlocksInternal(tooltip, instance, entity, masteryLevel, abilityLevel);
			string txt = AbilityArchetype.tt_reagentItem ? ZString.Format<string, string>("<color={0}>{1}</color> (active)", UIManager.ReagentBonusColor.ToHex(), this.m_type.ToStringWithSpaces()) : ZString.Format<string>("{0} (optional)", this.m_type.ToStringWithSpaces());
			tooltip.RequirementsBlock.AppendLine(txt, 0);
		}

		// Token: 0x04004C85 RID: 19589
		[SerializeField]
		private ReagentType m_type;
	}
}
