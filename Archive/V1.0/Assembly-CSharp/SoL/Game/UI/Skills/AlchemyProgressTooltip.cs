using System;
using Cysharp.Text;
using SoL.Game.EffectSystem;
using SoL.Game.Interactives;
using SoL.Game.Settings;
using SoL.UI;
using SoL.Utilities.Extensions;
using UnityEngine;

namespace SoL.Game.UI.Skills
{
	// Token: 0x02000929 RID: 2345
	public class AlchemyProgressTooltip : MonoBehaviour, ITooltip, IInteractiveBase
	{
		// Token: 0x060044FB RID: 17659 RVA: 0x0019E398 File Offset: 0x0019C598
		private ITooltipParameter GetTooltipParameter()
		{
			if (this.m_slot && this.m_slot.AbilityInstance != null && this.m_slot.AbilityInstance.AbilityData != null && this.m_slot.AbilityInstance.Archetype && LocalPlayer.GameEntity && GlobalSettings.Values.Ashen.AlchemyAvailableForEntity(LocalPlayer.GameEntity))
			{
				int usageCount = this.m_slot.AbilityInstance.AbilityData.GetUsageCount(AlchemyPowerLevel.I);
				int alchemyUsageThreshold = GlobalSettings.Values.Ashen.GetAlchemyUsageThreshold(AlchemyPowerLevel.II);
				float num = (float)usageCount / (float)alchemyUsageThreshold;
				string modifiedDisplayName = this.m_slot.AbilityInstance.Archetype.GetModifiedDisplayName(this.m_slot.AbilityInstance);
				string txt = (num >= 1f) ? ZString.Format<string>("Alchemy II Unlocked for {0}!", modifiedDisplayName) : ZString.Format<string, int, int, string>("{0}% progress ({1}/{2}) towards unlocking Alchemy II {3}", num.GetAsPercentage(), usageCount, alchemyUsageThreshold, modifiedDisplayName);
				return new ObjectTextTooltipParameter(this, txt, false);
			}
			return null;
		}

		// Token: 0x17000F78 RID: 3960
		// (get) Token: 0x060044FC RID: 17660 RVA: 0x00049FFA File Offset: 0x000481FA
		InteractionSettings IInteractiveBase.Settings
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000F79 RID: 3961
		// (get) Token: 0x060044FD RID: 17661 RVA: 0x0006E930 File Offset: 0x0006CB30
		BaseTooltip.GetTooltipParameter ITooltip.GetTooltipParameter
		{
			get
			{
				return new BaseTooltip.GetTooltipParameter(this.GetTooltipParameter);
			}
		}

		// Token: 0x17000F7A RID: 3962
		// (get) Token: 0x060044FE RID: 17662 RVA: 0x0006E93E File Offset: 0x0006CB3E
		TooltipSettings ITooltip.TooltipSettings
		{
			get
			{
				return this.m_tooltipSettings;
			}
		}

		// Token: 0x06004500 RID: 17664 RVA: 0x00052028 File Offset: 0x00050228
		GameObject IInteractiveBase.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x0400418A RID: 16778
		[SerializeField]
		private TooltipSettings m_tooltipSettings;

		// Token: 0x0400418B RID: 16779
		[SerializeField]
		private AbilitySlot m_slot;
	}
}
