using System;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C81 RID: 3201
	public class SerializationTester_Ability : MonoBehaviour, ICombatEffectSource
	{
		// Token: 0x17001759 RID: 5977
		// (get) Token: 0x0600617E RID: 24958 RVA: 0x00081BD2 File Offset: 0x0007FDD2
		UniqueId ICombatEffectSource.ArchetypeId
		{
			get
			{
				return UniqueId.Empty;
			}
		}

		// Token: 0x1700175A RID: 5978
		// (get) Token: 0x0600617F RID: 24959 RVA: 0x00081BD9 File Offset: 0x0007FDD9
		public DeliveryParams DeliveryParams
		{
			get
			{
				return this.m_delivery;
			}
		}

		// Token: 0x06006180 RID: 24960 RVA: 0x00081BE1 File Offset: 0x0007FDE1
		TargetingParams ICombatEffectSource.GetTargetingParams(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return this.m_targetingParams;
		}

		// Token: 0x06006181 RID: 24961 RVA: 0x00081BE9 File Offset: 0x0007FDE9
		KinematicParameters ICombatEffectSource.GetKinematicParams(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return this.m_kinmatic;
		}

		// Token: 0x06006182 RID: 24962 RVA: 0x00081BF1 File Offset: 0x0007FDF1
		CombatEffect ICombatEffectSource.GetCombatEffect(float level, AlchemyPowerLevel alchemyPowerLevel)
		{
			return this.m_effect;
		}

		// Token: 0x040054F8 RID: 21752
		[SerializeField]
		private ExecutionParams m_execution;

		// Token: 0x040054F9 RID: 21753
		[SerializeField]
		private TargetingParams m_targetingParams;

		// Token: 0x040054FA RID: 21754
		[SerializeField]
		private DeliveryParams m_delivery;

		// Token: 0x040054FB RID: 21755
		[SerializeField]
		private KinematicParameters m_kinmatic;

		// Token: 0x040054FC RID: 21756
		[SerializeField]
		private CombatEffectWithSecondary m_effect;
	}
}
