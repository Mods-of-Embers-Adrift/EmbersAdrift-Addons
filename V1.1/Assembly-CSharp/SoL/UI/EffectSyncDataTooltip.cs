using System;
using SoL.Game.Objects.Archetypes;
using SoL.Game.Objects.Archetypes.Abilities;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.UI
{
	// Token: 0x0200037C RID: 892
	public class EffectSyncDataTooltip : ArchetypeTooltip
	{
		// Token: 0x06001898 RID: 6296 RVA: 0x00104FE0 File Offset: 0x001031E0
		protected override void SetData()
		{
			BaseTooltip.GetTooltipParameter parameterGetter = this.m_parameterGetter;
			ITooltipParameter tooltipParameter = (parameterGetter != null) ? parameterGetter() : null;
			if (tooltipParameter == null || !(tooltipParameter is EffectSyncDataTooltipParameter))
			{
				return;
			}
			EffectSyncDataTooltipParameter effectSyncDataTooltipParameter = (EffectSyncDataTooltipParameter)tooltipParameter;
			bool combatEffect = effectSyncDataTooltipParameter.SyncData.CombatEffect != null;
			BaseArchetype archetype = effectSyncDataTooltipParameter.Archetype;
			if (!combatEffect || archetype == null)
			{
				return;
			}
			base.SetDurability(null);
			float value = Mathf.Clamp((float)(effectSyncDataTooltipParameter.SyncData.ExpirationTime - DateTime.UtcNow).TotalSeconds, 0f, float.MaxValue);
			byte level = effectSyncDataTooltipParameter.SyncData.Level;
			TooltipTextBlock dataBlock = base.DataBlock;
			this.m_title.text = archetype.DisplayName;
			this.m_archetypeIcon.SetIcon(archetype, null);
			dataBlock.AppendLine("Applied by: " + effectSyncDataTooltipParameter.SyncData.ApplicatorName, 0);
			if (level > 0)
			{
				dataBlock.AppendLine("Level: " + level.ToString(), 0);
			}
			AuraAbility auraAbility;
			if (!archetype.TryGetAsType(out auraAbility))
			{
				dataBlock.AppendLine("Duration: " + effectSyncDataTooltipParameter.SyncData.Duration.GetFormattedTime(true), 0);
				dataBlock.AppendLine("Time remaining: " + value.GetFormattedTime(true), 0);
			}
			base.ToggleBlocks();
		}
	}
}
