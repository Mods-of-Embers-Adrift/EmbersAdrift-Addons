using System;
using SoL.Game.Objects;
using SoL.Game.Objects.Archetypes;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C44 RID: 3140
	[CreateAssetMenu(menuName = "SoL/Objects/Itemization/Scriptable Effect")]
	public class ScriptableEffectData : BaseArchetype, ICombatEffectSource, IVfxSource
	{
		// Token: 0x17001744 RID: 5956
		// (get) Token: 0x060060DD RID: 24797 RVA: 0x000813B2 File Offset: 0x0007F5B2
		private bool m_showVfx
		{
			get
			{
				return this.m_vfxOverride == null;
			}
		}

		// Token: 0x17001745 RID: 5957
		// (get) Token: 0x060060DE RID: 24798 RVA: 0x00074515 File Offset: 0x00072715
		UniqueId ICombatEffectSource.ArchetypeId
		{
			get
			{
				return this.m_id;
			}
		}

		// Token: 0x17001746 RID: 5958
		// (get) Token: 0x060060DF RID: 24799 RVA: 0x00049FFA File Offset: 0x000481FA
		DeliveryParams ICombatEffectSource.DeliveryParams
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001747 RID: 5959
		// (get) Token: 0x060060E0 RID: 24800 RVA: 0x000813C0 File Offset: 0x0007F5C0
		public CombatEffect Effect
		{
			get
			{
				return this.m_effect;
			}
		}

		// Token: 0x060060E1 RID: 24801 RVA: 0x00049FFA File Offset: 0x000481FA
		TargetingParams ICombatEffectSource.GetTargetingParams(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return null;
		}

		// Token: 0x060060E2 RID: 24802 RVA: 0x00049FFA File Offset: 0x000481FA
		KinematicParameters ICombatEffectSource.GetKinematicParams(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return null;
		}

		// Token: 0x060060E3 RID: 24803 RVA: 0x000813C0 File Offset: 0x0007F5C0
		CombatEffect ICombatEffectSource.GetCombatEffect(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return this.m_effect;
		}

		// Token: 0x060060E4 RID: 24804 RVA: 0x000813C8 File Offset: 0x0007F5C8
		bool IVfxSource.TryGetEffects(int abilityLevel, AlchemyPowerLevel alchemyPowerLevel, bool isSecondary, out AbilityVFX effects)
		{
			effects = (this.m_vfxOverride ? this.m_vfxOverride.VFX : this.m_vfx);
			return effects != null;
		}

		// Token: 0x04005381 RID: 21377
		private static MinMaxIntRange kLevelRange = new MinMaxIntRange(1, 1);

		// Token: 0x04005382 RID: 21378
		[SerializeField]
		protected AbilityVFXScriptable m_vfxOverride;

		// Token: 0x04005383 RID: 21379
		[SerializeField]
		protected AbilityVFX m_vfx;

		// Token: 0x04005384 RID: 21380
		[SerializeField]
		private CombatEffect m_effect;
	}
}
