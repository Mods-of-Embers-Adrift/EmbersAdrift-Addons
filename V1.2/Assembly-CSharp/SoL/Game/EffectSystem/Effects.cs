using System;
using System.Collections;
using SoL.Utilities;
using UnityEngine;

namespace SoL.Game.EffectSystem
{
	// Token: 0x02000C13 RID: 3091
	[Serializable]
	public class Effects
	{
		// Token: 0x170016A9 RID: 5801
		// (get) Token: 0x06005F4C RID: 24396 RVA: 0x000802B2 File Offset: 0x0007E4B2
		private bool m_showConditional
		{
			get
			{
				return this.m_hasInstantVitals && this.IsTimeBased;
			}
		}

		// Token: 0x170016AA RID: 5802
		// (get) Token: 0x06005F4D RID: 24397 RVA: 0x000802C4 File Offset: 0x0007E4C4
		public bool IsTimeBased
		{
			get
			{
				return this.HasLasting;
			}
		}

		// Token: 0x170016AB RID: 5803
		// (get) Token: 0x06005F4E RID: 24398 RVA: 0x000802CC File Offset: 0x0007E4CC
		public bool HasLasting
		{
			get
			{
				return this.m_hasOverTimeVitals || this.m_hasStatusEffects || this.m_hasBehaviorEffects || this.m_hasTriggerEffects || this.m_hasCombatFlags;
			}
		}

		// Token: 0x170016AC RID: 5804
		// (get) Token: 0x06005F4F RID: 24399 RVA: 0x000802F6 File Offset: 0x0007E4F6
		public CombatTriggerCondition LastingConditional
		{
			get
			{
				return this.m_lastingConditional;
			}
		}

		// Token: 0x170016AD RID: 5805
		// (get) Token: 0x06005F50 RID: 24400 RVA: 0x000802FE File Offset: 0x0007E4FE
		public bool HasInstantVitals
		{
			get
			{
				return this.m_hasInstantVitals;
			}
		}

		// Token: 0x170016AE RID: 5806
		// (get) Token: 0x06005F51 RID: 24401 RVA: 0x00080306 File Offset: 0x0007E506
		public VitalsEffect_Instant[] Instant
		{
			get
			{
				return this.m_instantVitals;
			}
		}

		// Token: 0x170016AF RID: 5807
		// (get) Token: 0x06005F52 RID: 24402 RVA: 0x0008030E File Offset: 0x0007E50E
		public bool HasOverTimeVitals
		{
			get
			{
				return this.m_hasOverTimeVitals;
			}
		}

		// Token: 0x170016B0 RID: 5808
		// (get) Token: 0x06005F53 RID: 24403 RVA: 0x00080316 File Offset: 0x0007E516
		public int TickRate
		{
			get
			{
				return this.m_tickRate;
			}
		}

		// Token: 0x170016B1 RID: 5809
		// (get) Token: 0x06005F54 RID: 24404 RVA: 0x0008031E File Offset: 0x0007E51E
		public VitalsEffect_OverTime[] OverTime
		{
			get
			{
				return this.m_overTimeVitals;
			}
		}

		// Token: 0x170016B2 RID: 5810
		// (get) Token: 0x06005F55 RID: 24405 RVA: 0x00080326 File Offset: 0x0007E526
		public bool HasStatusEffects
		{
			get
			{
				return this.m_hasStatusEffects;
			}
		}

		// Token: 0x170016B3 RID: 5811
		// (get) Token: 0x06005F56 RID: 24406 RVA: 0x0008032E File Offset: 0x0007E52E
		public StatusEffect StatusEffect
		{
			get
			{
				return this.m_status;
			}
		}

		// Token: 0x170016B4 RID: 5812
		// (get) Token: 0x06005F57 RID: 24407 RVA: 0x00080336 File Offset: 0x0007E536
		public bool HasBehaviorEffects
		{
			get
			{
				return this.m_hasBehaviorEffects;
			}
		}

		// Token: 0x170016B5 RID: 5813
		// (get) Token: 0x06005F58 RID: 24408 RVA: 0x0008033E File Offset: 0x0007E53E
		public BehaviorEffectTypes BehaviorType
		{
			get
			{
				return this.m_behavior;
			}
		}

		// Token: 0x170016B6 RID: 5814
		// (get) Token: 0x06005F59 RID: 24409 RVA: 0x00080346 File Offset: 0x0007E546
		public bool HasTriggerEffects
		{
			get
			{
				return this.m_hasTriggerEffects;
			}
		}

		// Token: 0x170016B7 RID: 5815
		// (get) Token: 0x06005F5A RID: 24410 RVA: 0x0008034E File Offset: 0x0007E54E
		public ScriptableCombatEffect TriggeredEffect
		{
			get
			{
				return this.m_triggeredEffect;
			}
		}

		// Token: 0x170016B8 RID: 5816
		// (get) Token: 0x06005F5B RID: 24411 RVA: 0x00080356 File Offset: 0x0007E556
		public bool HasCombatFlags
		{
			get
			{
				return this.m_hasCombatFlags;
			}
		}

		// Token: 0x170016B9 RID: 5817
		// (get) Token: 0x06005F5C RID: 24412 RVA: 0x0008035E File Offset: 0x0007E55E
		public CombatFlags CombatFlags
		{
			get
			{
				return this.m_combatFlags;
			}
		}

		// Token: 0x170016BA RID: 5818
		// (get) Token: 0x06005F5D RID: 24413 RVA: 0x00080366 File Offset: 0x0007E566
		public bool ShowLastingConditional
		{
			get
			{
				return this.m_showConditional;
			}
		}

		// Token: 0x06005F5E RID: 24414 RVA: 0x0008036E File Offset: 0x0007E56E
		private IEnumerable GetTriggeredEffects()
		{
			return SolOdinUtilities.GetDropdownItems<ScriptableCombatEffect>();
		}

		// Token: 0x06005F5F RID: 24415 RVA: 0x00080375 File Offset: 0x0007E575
		private void ValidateNumbers()
		{
			this.m_tickRate = Mathf.Clamp(this.m_tickRate, 1, int.MaxValue);
		}

		// Token: 0x04005253 RID: 21075
		private const string kConditionalBase = "m_showConditional";

		// Token: 0x04005254 RID: 21076
		private const string kConditionalGroup = "m_showConditional/Lasting Conditional";

		// Token: 0x04005255 RID: 21077
		private const string kListDrawerElementLabelName = "GetTitleText";

		// Token: 0x04005256 RID: 21078
		private const bool kCollapseOthersOnExpand = false;

		// Token: 0x04005257 RID: 21079
		[SerializeField]
		private bool m_hasInstantVitals;

		// Token: 0x04005258 RID: 21080
		[SerializeField]
		private VitalsEffect_Instant[] m_instantVitals;

		// Token: 0x04005259 RID: 21081
		[SerializeField]
		private CombatTriggerCondition m_lastingConditional;

		// Token: 0x0400525A RID: 21082
		[SerializeField]
		private bool m_hasOverTimeVitals;

		// Token: 0x0400525B RID: 21083
		[SerializeField]
		private int m_tickRate = 1;

		// Token: 0x0400525C RID: 21084
		[SerializeField]
		private VitalsEffect_OverTime[] m_overTimeVitals;

		// Token: 0x0400525D RID: 21085
		[SerializeField]
		private bool m_hasStatusEffects;

		// Token: 0x0400525E RID: 21086
		[SerializeField]
		private StatusEffect m_status;

		// Token: 0x0400525F RID: 21087
		[SerializeField]
		private bool m_hasBehaviorEffects;

		// Token: 0x04005260 RID: 21088
		[SerializeField]
		private BehaviorEffectTypes m_behavior;

		// Token: 0x04005261 RID: 21089
		[SerializeField]
		private bool m_hasTriggerEffects;

		// Token: 0x04005262 RID: 21090
		[SerializeField]
		private ScriptableCombatEffect m_triggeredEffect;

		// Token: 0x04005263 RID: 21091
		[SerializeField]
		private bool m_hasCombatFlags;

		// Token: 0x04005264 RID: 21092
		[SerializeField]
		private CombatFlags m_combatFlags;
	}
}
