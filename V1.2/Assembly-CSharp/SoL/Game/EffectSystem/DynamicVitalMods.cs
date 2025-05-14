using System;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C12 RID: 3090
	public class DynamicVitalMods : VitalMods
	{
		// Token: 0x06005F47 RID: 24391 RVA: 0x00080269 File Offset: 0x0007E469
		public DynamicVitalMods()
		{
		}

		// Token: 0x06005F48 RID: 24392 RVA: 0x00080271 File Offset: 0x0007E471
		public DynamicVitalMods(DynamicVitalMods other) : base(other)
		{
		}

		// Token: 0x06005F49 RID: 24393 RVA: 0x0008027A File Offset: 0x0007E47A
		public void Reset()
		{
			this.m_combatFlags = CombatFlags.None;
			this.m_valueAdditive = 0;
			this.m_valueMultiplier = 1f;
			this.m_threatMultiplier = 1f;
			this.m_hitModifier = 0;
			this.m_armorModifier = 0f;
		}

		// Token: 0x06005F4A RID: 24394 RVA: 0x001F9424 File Offset: 0x001F7624
		public void CopyFromExecutionCache(ExecutionCache executionCache)
		{
			if (executionCache == null || executionCache.ExternalMods == null)
			{
				return;
			}
			this.Reset();
			DynamicVitalMods externalMods = executionCache.ExternalMods;
			this.m_combatFlags = externalMods.CombatFlags;
			this.m_valueAdditive = externalMods.m_valueAdditive;
			this.m_valueMultiplier = externalMods.m_valueMultiplier;
			this.m_threatMultiplier = externalMods.m_threatMultiplier;
			this.m_hitModifier = externalMods.m_hitModifier;
			this.m_armorModifier = externalMods.m_armorModifier;
		}

		// Token: 0x06005F4B RID: 24395 RVA: 0x001F9494 File Offset: 0x001F7694
		public void CombineMods(VitalMods mods)
		{
			this.m_combatFlags |= mods.CombatFlags;
			this.m_valueAdditive += mods.ValueAdditive;
			this.m_valueMultiplier *= mods.ValueMultiplier;
			this.m_threatMultiplier *= mods.ThreatMultiplier;
			this.m_hitModifier += mods.HitModifier;
			this.m_armorModifier += mods.ArmorModifier;
		}
	}
}
