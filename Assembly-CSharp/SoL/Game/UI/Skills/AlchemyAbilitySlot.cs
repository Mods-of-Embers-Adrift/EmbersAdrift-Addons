using System;
using SoL.Game.EffectSystem;
using SoL.Game.Settings;
using SoL.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace SoL.Game.UI.Skills
{
	// Token: 0x02000924 RID: 2340
	public class AlchemyAbilitySlot : MonoBehaviour
	{
		// Token: 0x060044EE RID: 17646 RVA: 0x0019E238 File Offset: 0x0019C438
		internal void RefreshProgressDisplay(AbilitySlot slot)
		{
			IExecutable executable;
			if (slot && slot.AbilityInstance != null && slot.AbilityInstance.AbilityData != null && slot.AbilityInstance.Archetype != null && slot.AbilityInstance.Archetype.TryGetAsType(out executable) && executable.AllowAlchemy && LocalPlayer.GameEntity && GlobalSettings.Values.Ashen.AlchemyAvailableForEntity(LocalPlayer.GameEntity))
			{
				float usageCount = (float)slot.AbilityInstance.AbilityData.GetUsageCount(AlchemyPowerLevel.I);
				int alchemyUsageThreshold = GlobalSettings.Values.Ashen.GetAlchemyUsageThreshold(AlchemyPowerLevel.II);
				float num = usageCount / (float)alchemyUsageThreshold;
				if (this.m_progressFill)
				{
					this.m_progressFill.fillAmount = num;
				}
				if (this.m_alchemyIIbgImage)
				{
					bool enabled = num >= 1f;
					this.m_alchemyIIbgImage.enabled = enabled;
				}
				base.gameObject.SetActive(true);
				return;
			}
			base.gameObject.SetActive(false);
		}

		// Token: 0x04004183 RID: 16771
		[SerializeField]
		private Image m_alchemyIIbgImage;

		// Token: 0x04004184 RID: 16772
		[SerializeField]
		private Image m_progressFill;
	}
}
