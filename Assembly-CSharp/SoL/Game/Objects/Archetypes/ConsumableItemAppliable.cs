using System;
using SoL.Game.EffectSystem;
using SoL.UI;
using UnityEngine;

namespace SoL.Game.Objects.Archetypes
{
	// Token: 0x02000A5E RID: 2654
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Items/Consumable Appliable")]
	public class ConsumableItemAppliable : ConsumableItemStackable, ICombatEffectSource
	{
		// Token: 0x170012A6 RID: 4774
		// (get) Token: 0x0600524B RID: 21067 RVA: 0x00074515 File Offset: 0x00072715
		UniqueId ICombatEffectSource.ArchetypeId
		{
			get
			{
				return this.m_id;
			}
		}

		// Token: 0x170012A7 RID: 4775
		// (get) Token: 0x0600524C RID: 21068 RVA: 0x00076EE1 File Offset: 0x000750E1
		DeliveryParams ICombatEffectSource.DeliveryParams
		{
			get
			{
				return this.m_delivery;
			}
		}

		// Token: 0x0600524D RID: 21069 RVA: 0x00076EE9 File Offset: 0x000750E9
		TargetingParams ICombatEffectSource.GetTargetingParams(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return this.m_target;
		}

		// Token: 0x0600524E RID: 21070 RVA: 0x00049FFA File Offset: 0x000481FA
		KinematicParameters ICombatEffectSource.GetKinematicParams(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return null;
		}

		// Token: 0x0600524F RID: 21071 RVA: 0x00076EF1 File Offset: 0x000750F1
		CombatEffect ICombatEffectSource.GetCombatEffect(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return this.m_effect;
		}

		// Token: 0x06005250 RID: 21072 RVA: 0x00076EF9 File Offset: 0x000750F9
		protected override bool ExecutionCheckInternal(ExecutionCache executionCache)
		{
			return base.ExecutionCheckInternal(executionCache) && this.m_target.ExecutionCheck(executionCache, 0f);
		}

		// Token: 0x06005251 RID: 21073 RVA: 0x00076F17 File Offset: 0x00075117
		public override void FillTooltipBlocks(ArchetypeTooltip tooltip, ArchetypeInstance instance, GameEntity entity)
		{
			base.FillTooltipBlocks(tooltip, instance, entity);
			CombatEffectExtensions.FillTooltipEffectsBlock(this, tooltip, entity, 50f);
			TargetingParams target = this.m_target;
			if (target == null)
			{
				return;
			}
			target.FillTooltipTargetingBlock(tooltip, entity);
		}

		// Token: 0x06005252 RID: 21074 RVA: 0x001D3500 File Offset: 0x001D1700
		protected override void FillTooltipDistanceAngle(ArchetypeTooltip tooltip, GameEntity entity)
		{
			base.FillTooltipDistanceAngle(tooltip, entity);
			string left;
			string right;
			if (this.m_target.TryGetTargetDistanceAngleForTooltip(entity, out left, out right))
			{
				tooltip.RequirementsBlock.AppendLine(left, right);
			}
		}

		// Token: 0x0400499F RID: 18847
		private const float kMasteryLevel = 50f;

		// Token: 0x040049A0 RID: 18848
		[SerializeField]
		private TargetingParams m_target;

		// Token: 0x040049A1 RID: 18849
		[SerializeField]
		private DeliveryParams m_delivery;

		// Token: 0x040049A2 RID: 18850
		[SerializeField]
		private CombatEffect m_effect;
	}
}
